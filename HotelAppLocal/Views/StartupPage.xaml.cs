using HotelAppLocal.Data;
using Microsoft.Extensions.DependencyInjection;

namespace HotelAppLocal.Views;

public partial class StartupPage : ContentPage
{
    private readonly DbHealthService _dbHealth;

    private bool _isBusy = false;
    private bool _dbAvailable = false;

    public StartupPage(DbHealthService dbHealth)
    {
        InitializeComponent();
        _dbHealth = dbHealth;
        UpdateUi();
    }

    private void SetBusy(bool busy)
    {
        _isBusy = busy;
        UpdateUi();
    }

    private void SetDbAvailable(bool available)
    {
        _dbAvailable = available;
        UpdateUi();
    }

    private void UpdateUi()
    {
        BusyIndicator.IsRunning = _isBusy;
        BusyIndicator.IsVisible = _isBusy;

        CheckButton.IsEnabled = !_isBusy;
        InitButton.IsEnabled = !_isBusy;

        // "Продолжить" доступна только если база доступна и нет активной операции
        ContinueButton.IsEnabled = _dbAvailable && !_isBusy;
    }

    private async void OnCheckClicked(object sender, EventArgs e)
    {
        if (_isBusy) return;

        SetBusy(true);
        SetDbAvailable(false);

        StatusLabel.Text = "Проверяем доступность сервера...";
        StatusLabel.TextColor = Color.FromArgb("#6B7280");

        // Даем UI возможность отрисовать индикатор и текст
        await Task.Yield();
        // Если вдруг этого мало, можно заменить на:
        // await Task.Delay(50);

        var ok = await _dbHealth.CheckConnectionAsync();

        if (ok)
        {
            StatusLabel.Text = "✅ Сервер доступен. Можно использовать существующую базу или создать новую.";
            StatusLabel.TextColor = Color.FromArgb("#16A34A");
            SetDbAvailable(true);
        }
        else
        {
            var msg = _dbHealth.LastError?.Message ?? "Неизвестная ошибка";
            StatusLabel.Text = $"❌ Ошибка подключения: {msg}";
            StatusLabel.TextColor = Color.FromArgb("#DC2626");
            SetDbAvailable(false);
        }

        SetBusy(false);
    }

    private async void OnInitClicked(object sender, EventArgs e)
    {
        if (_isBusy) return;

        SetBusy(true);
        SetDbAvailable(false);

        StatusLabel.Text = "Создаём тестовую базу данных (пересоздаём)...";
        StatusLabel.TextColor = Color.FromArgb("#6B7280");

        // То же самое — сперва даём UI перерисоваться
        await Task.Yield();
        // или await Task.Delay(50);

        var ok = await _dbHealth.InitializeTestDatabaseAsync();

        if (ok)
        {
            StatusLabel.Text = "✅ Тестовая база данных успешно создана.";
            StatusLabel.TextColor = Color.FromArgb("#16A34A");
            SetDbAvailable(true);
        }
        else
        {
            var msg = _dbHealth.LastError?.Message ?? "Неизвестная ошибка";
            StatusLabel.Text = $"❌ Ошибка при создании тестовой БД: {msg}";
            StatusLabel.TextColor = Color.FromArgb("#DC2626");
            SetDbAvailable(false);
        }

        SetBusy(false);
    }

    private void OnContinueClicked(object sender, EventArgs e)
    {
        if (!_dbAvailable || _isBusy)
            return;

        var services = App.Current?.Handler?.MauiContext?.Services;
        if (services is null)
        {
            StatusLabel.Text = "Внутренняя ошибка: сервисы не доступны.";
            StatusLabel.TextColor = Color.FromArgb("#DC2626");
            return;
        }

        var loginPage = services.GetRequiredService<LoginPage>();
        Application.Current.MainPage = new NavigationPage(loginPage);
    }
}
