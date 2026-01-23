using Microsoft.EntityFrameworkCore;
using SchoolMenuPlanner.Classes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SchoolMenuPlanner
{
    public partial class AuthorizationWindow : Window
    {
        private bool _isUsernamePlaceholder = true;
        private bool _isPasswordPlaceholder = true;

        public AuthorizationWindow()
        {
            InitializeComponent();

            SetUsernamePlaceholder();
            SetPasswordPlaceholder();
            Loaded += (s, e) => UsernameTextBox.Focus();
            UsernameTextBox.KeyDown += TextBox_KeyDown;
            PasswordBox.KeyDown += PasswordBox_KeyDown;
        }

        private void SetUsernamePlaceholder()
        {
            if (_isUsernamePlaceholder)
            {
                UsernameTextBox.Text = "Введите логин";
                UsernameTextBox.Foreground = Brushes.Gray;
            }
        }

        private void SetPasswordPlaceholder()
        {
            if (_isPasswordPlaceholder)
            {
                PasswordBox.Password = "";
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender == UsernameTextBox)
                {
                    PasswordBox.Focus();
                }
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_isUsernamePlaceholder)
            {
                UsernameTextBox.Text = "";
                UsernameTextBox.Foreground = new SolidColorBrush(Color.FromRgb(45, 55, 72));
                _isUsernamePlaceholder = false;
            }
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                _isUsernamePlaceholder = true;
                SetUsernamePlaceholder();
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_isPasswordPlaceholder)
            {
                PasswordBox.Password = "";
                _isPasswordPlaceholder = false;
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                _isPasswordPlaceholder = true;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = _isUsernamePlaceholder ? "" : UsernameTextBox.Text.Trim();
            string password = _isPasswordPlaceholder ? "" : PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Пожалуйста, заполните все поля");
                return;
            }

            SetLoginState(true);
            CheckCredentials(username, password);
        }

        private void CheckCredentials(string username, string password)
        {
            try
            {
                using (var context = new PlannerContext())
                {
                    // Используем правильные названия свойств: login и password
                    var user = context.Users
                        .FirstOrDefault(u => u.login == username && u.password == password);

                    if (user != null)
                    {
                        OpenMainWindow(user);
                        this.Close();
                    }
                    else
                    {
                        ShowError("Неверный логин или пароль");
                        ResetLoginState();
                    }
                }
            }
            catch (System.Exception ex)
            {
                ShowError($"Ошибка подключения к базе данных: {ex.Message}");
                ResetLoginState();
            }
        }

        private void SetLoginState(bool isLoading)
        {
            if (isLoading)
            {
                LoginButton.Content = "Вход...";
                LoginButton.IsEnabled = false;
                UsernameTextBox.IsEnabled = false;
                PasswordBox.IsEnabled = false;
            }
            else
            {
                LoginButton.Content = "Войти";
                LoginButton.IsEnabled = true;
                UsernameTextBox.IsEnabled = true;
                PasswordBox.IsEnabled = true;
            }
        }

        private void ResetLoginState()
        {
            SetLoginState(false);
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }

        private void OpenMainWindow(User user)
        {
            // Создаем MainWindow и передаем пользователя
            MainWindow mainWindow = new MainWindow(user);
            Application.Current.MainWindow = mainWindow; // Устанавливаем как главное окно
            mainWindow.Show();
            this.Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}