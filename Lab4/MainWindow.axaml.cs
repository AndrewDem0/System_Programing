using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab4Avalonia
{
    public partial class MainWindow : Window
    {
        // Маркер кооперативного скасування (Завдання 4)
        private CancellationTokenSource _cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Точка входу обробника події. 
        // async void є архітектурно допустимим виключно для top-level обробників подій UI.
        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            // Перемикання станів елементів керування
            BtnStart.IsEnabled = false;
            BtnCancel.IsEnabled = true;
            PbTaskProgress.Value = 0;
            TxtProgressPercent.Text = "0%";
            TxtResult.Text = "Результат: Обчислення...";
            TxtStatus.Text = "Стан: Фонова задача виконується";

            _cancellationTokenSource = new CancellationTokenSource();

            // Ініціалізація диспетчера прогресу у головному потоці (UI Thread)
            var progressHandler = new Progress<int>(percent =>
            {
                PbTaskProgress.Value = percent;
                TxtProgressPercent.Text = $"{percent}%";
            });

            try
            {
                // Завдання 2: Отримання результату від асинхронної операції.
                // Оператор await розблоковує потік Avalonia до завершення обчислень.
                int computedResult = await ProcessLongTaskAsync(100, progressHandler, _cancellationTokenSource.Token);
                
                TxtResult.Text = $"Результат (Сума): {computedResult}";
                TxtStatus.Text = "Стан: Успішно завершено";
            }
            catch (OperationCanceledException)
            {
                TxtResult.Text = "Результат: N/A";
                TxtStatus.Text = "Стан: Перервано сигналом Cancellation Token";
                PbTaskProgress.Value = 0;
                TxtProgressPercent.Text = "Скасовано";
            }
            finally
            {
                // Гарантоване відновлення початкового стану незалежно від результату
                BtnStart.IsEnabled = true;
                BtnCancel.IsEnabled = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                // Переведення токена у стан скасованого
                _cancellationTokenSource.Cancel();
                BtnCancel.IsEnabled = false;
                TxtStatus.Text = "Стан: Ініціація переривання фонового потоку...";
            }
        }

        /// <summary>
        /// Інкапсулює ресурсомістку задачу, делегуючи її пулу потоків.
        /// </summary>
        private Task<int> ProcessLongTaskAsync(int totalSteps, IProgress<int> progress, CancellationToken token)
        {
            return Task.Run(() =>
            {
                int sum = 0;

                for (int i = 1; i <= totalSteps; i++)
                {
                    // Перевірка стану маркера. Якщо викликано Cancel(), генерується виняток.
                    token.ThrowIfCancellationRequested();

                    Thread.Sleep(100); 
                    sum += i;

                    // Відправка проміжного результату до головного потоку
                    progress?.Report(i);
                }

                return sum;
            }, token);
        }
    }
}