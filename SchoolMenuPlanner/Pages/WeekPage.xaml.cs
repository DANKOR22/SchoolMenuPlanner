using Microsoft.EntityFrameworkCore;
using SchoolMenuPlanner.Classes;
using SchoolMenuPlanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SchoolMenuPlanner
{
    public partial class WeekPage : Page
    {
        // Коллекции для каждой категории блюд (используем общий сервис)
        public ObservableCollection<Dish> HotDishesItems => MenuDataService.Instance.HotDishesItems;
        public ObservableCollection<Dish> HotFirstItems => MenuDataService.Instance.HotFirstItems;
        public ObservableCollection<Dish> SecondDishesItems => MenuDataService.Instance.SecondDishesItems;
        public ObservableCollection<Dish> GarnishItems => MenuDataService.Instance.GarnishItems;
        public ObservableCollection<Dish> SaladsItems => MenuDataService.Instance.SaladsItems;
        public ObservableCollection<Dish> DrinksItems => MenuDataService.Instance.DrinksItems;
        public ObservableCollection<Dish> FruitsItems => MenuDataService.Instance.FruitsItems;

        private PlannerContext _context;
        private int _currentWeekNumber;
        private int _currentYear;
        private User? _currentUser;

        public WeekPage()
        {
            InitializeComponent();
            this.DataContext = this;

            _context = new PlannerContext();

            // Получаем текущего пользователя из статического свойства MainWindow
            _currentUser = MainWindow.CurrentUser;

            // Устанавливаем текущую неделю и год
            var currentDate = DateTime.Now;
            _currentWeekNumber = GetIso8601WeekOfYear(currentDate);
            _currentYear = currentDate.Year;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await MenuDataService.Instance.LoadDataAsync();
            LoadSavedMenu();
            ApplyUserRestrictions();
        }

        private void ApplyUserRestrictions()
        {
            // Если пользователь не админ, блокируем ComboBox'ы
            if (_currentUser?.IsAdmin == false)
            {
                SetComboBoxesReadOnly();
            }
        }

        private void SetComboBoxesReadOnly()
        {
            // Блокируем все ComboBox'ы на странице
            var comboBoxes = FindVisualChildren<ComboBox>(this);
            foreach (var comboBox in comboBoxes)
            {
                comboBox.IsEnabled = false;
                comboBox.IsHitTestVisible = false;
                comboBox.Focusable = false;
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject? depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T typedChild)
                    {
                        yield return typedChild;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void LoadSavedMenu()
        {
            try
            {
                // Загружаем сохраненное меню для текущей недели
                var savedMenu = _context.DailyMenus
                    .Include(dm => dm.Dish)
                    .Where(dm => dm.WeekNumber == _currentWeekNumber && dm.Year == _currentYear)
                    .ToList();

                // Восстанавливаем выбранные блюда в ComboBox
                foreach (var menuItem in savedMenu)
                {
                    SetSelectedDish(menuItem.DayOfWeek, menuItem.MealType, menuItem.CategoryType, menuItem.DishId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сохраненного меню: {ex.Message}");
            }
        }

        private void SetSelectedDish(int dayOfWeek, string mealType, string categoryType, int dishId)
        {
            string comboBoxName = GetComboBoxName(dayOfWeek, mealType, categoryType);
            var comboBox = FindName(comboBoxName) as ComboBox;

            if (comboBox != null)
            {
                var dish = comboBox.Items.OfType<Dish>().FirstOrDefault(d => d.Id == dishId);
                if (dish != null)
                {
                    comboBox.SelectedItem = dish;
                }
            }
        }

        private string GetComboBoxName(int dayOfWeek, string mealType, string categoryType)
        {
            string dayName = dayOfWeek switch
            {
                1 => "Monday",
                2 => "Tuesday",
                3 => "Wednesday",
                4 => "Thursday",
                5 => "Friday",
                _ => "Monday"
            };

            string meal = mealType == "Breakfast" ? "Breakfast" : "Lunch";
            string category = categoryType switch
            {
                "HotDish" => "HotDish",
                "Drink" => "Drink",
                "Fruit" => "Fruit",
                "HotFirst" => "HotFirst",
                "Second" => "Second",
                "Garnish" => "Garnish",
                "Salad" => "Salad",
                _ => categoryType
            };

            return $"{dayName}{meal}{category}";
        }

        private async void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            // Если пользователь не админ, запрещаем сохранение
            if (_currentUser?.IsAdmin == false)
            {
                MessageBox.Show("У вас нет прав для сохранения меню.", "Доступ запрещен",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Удаляем старое меню для текущей недели
                var oldMenu = _context.DailyMenus
                    .Where(dm => dm.WeekNumber == _currentWeekNumber && dm.Year == _currentYear);
                _context.DailyMenus.RemoveRange(oldMenu);

                // Сохраняем новое меню
                SaveDayMenu(1, "Monday"); // Понедельник
                SaveDayMenu(2, "Tuesday"); // Вторник
                SaveDayMenu(3, "Wednesday"); // Среда
                SaveDayMenu(4, "Thursday"); // Четверг
                SaveDayMenu(5, "Friday"); // Пятница

                await _context.SaveChangesAsync();
                MessageBox.Show("Меню успешно сохранено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении меню: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveDayMenu(int dayOfWeek, string dayName)
        {
            // Сохраняем завтрак
            SaveMealSelection(dayOfWeek, "Breakfast", dayName + "BreakfastHotDish", "HotDish");
            SaveMealSelection(dayOfWeek, "Breakfast", dayName + "BreakfastDrink", "Drink");
            SaveMealSelection(dayOfWeek, "Breakfast", dayName + "BreakfastFruit", "Fruit");

            // Сохраняем обед
            SaveMealSelection(dayOfWeek, "Lunch", dayName + "LunchHotFirst", "HotFirst");
            SaveMealSelection(dayOfWeek, "Lunch", dayName + "LunchSecond", "Second");
            SaveMealSelection(dayOfWeek, "Lunch", dayName + "LunchGarnish", "Garnish");
            SaveMealSelection(dayOfWeek, "Lunch", dayName + "LunchSalad", "Salad");
            SaveMealSelection(dayOfWeek, "Lunch", dayName + "LunchDrink", "Drink");
        }

        private void SaveMealSelection(int dayOfWeek, string mealType, string comboBoxName, string categoryType)
        {
            var comboBox = FindName(comboBoxName) as ComboBox;
            if (comboBox?.SelectedItem is Dish selectedDish)
            {
                var dailyMenu = new DailyMenu
                {
                    DayOfWeek = dayOfWeek,
                    MealType = mealType,
                    DishId = selectedDish.Id,
                    CategoryType = categoryType,
                    WeekNumber = _currentWeekNumber,
                    Year = _currentYear
                };
                _context.DailyMenus.Add(dailyMenu);
            }
        }

        // Метод для получения номера недели по ISO 8601
        private int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private void ComboBoxItemMenu_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxItemMenu.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                MainWindow? mainWindow = Window.GetWindow(this) as MainWindow;
                string pageType = selectedItem.Tag.ToString() ?? "";

                switch (pageType)
                {
                    case "CatalogPage":
                        // Скрываем каталог для обычных пользователей
                        if (_currentUser?.IsAdmin == false)
                        {
                            MessageBox.Show("У вас нет доступа к каталогу блюд.", "Доступ запрещен",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            ComboBoxItemMenu.SelectedIndex = 0; // Возвращаем на текущую неделю
                            return;
                        }
                        if (mainWindow != null)
                            mainWindow.MainFramePublic.Content = new CatalogPage();
                        break;
                    case "NextWeekPage":
                        if (mainWindow != null)
                            mainWindow.MainFramePublic.Content = new NextWeekPage();
                        break;
                        // "WeekPage" не обрабатываем - мы уже на этой странице
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}