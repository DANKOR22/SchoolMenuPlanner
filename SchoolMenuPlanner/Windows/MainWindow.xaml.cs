using SchoolMenuPlanner.Classes;
using SchoolMenuPlanner.Pages;
using System.Windows;
using System.Windows.Controls;

namespace SchoolMenuPlanner
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        public Frame MainFramePublic => MainFrame;

        // Статическое свойство для глобального доступа к текущему пользователю
        public static User? CurrentUser { get; private set; }

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            CurrentUser = user; // Сохраняем в статическое свойство
            InitializeUserInterface();
        }

        private void InitializeUserInterface()
        {
            this.Title = $"School Menu Planner - {_currentUser.full_name}";

            // Показываем кнопки навигации в зависимости от роли
            SetupNavigation();

            LoadInitialPage();
        }

        private void SetupNavigation()
        {
            // Здесь можно добавить меню навигации если нужно
        }

        private void LoadInitialPage()
        {
            try
            {
                // Загружаем WeekPage из папки Pages
                var weekPage = new WeekPage();
                MainFramePublic.Navigate(weekPage);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки страницы: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                // Запасной вариант - простая страница
                LoadFallbackPage();
            }
        }

        private void LoadFallbackPage()
        {
            var fallbackPage = new Page();
            fallbackPage.Content = new TextBlock
            {
                Text = "Добро пожаловать в School Menu Planner!",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            MainFramePublic.Navigate(fallbackPage);
        }

        // Методы для навигации между страницами
        public void NavigateToWeekPage()
        {
            var weekPage = new WeekPage();
            MainFramePublic.Navigate(weekPage);
        }

        public void NavigateToCatalogPage()
        {
            var catalogPage = new CatalogPage();
            MainFramePublic.Content = catalogPage;
        }

        public void NavigateToNextWeekPage()
        {
            var nextWeekPage = new NextWeekPage();
            MainFramePublic.Navigate(nextWeekPage);
        }
    }
}