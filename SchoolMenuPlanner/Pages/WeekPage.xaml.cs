using Microsoft.EntityFrameworkCore;
using SchoolMenuPlanner.Classes;
using SchoolMenuPlanner.Models;
using SchoolMenuPlanner.Pages;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SchoolMenuPlanner
{
    public partial class WeekPage : Page
    {
        // Коллекции для каждой категории блюд
        public ObservableCollection<Dish> HotDishesItems { get; set; }
        public ObservableCollection<Dish> HotFirstItems { get; set; }
        public ObservableCollection<Dish> SecondDishesItems { get; set; }
        public ObservableCollection<Dish> GarnishItems { get; set; }
        public ObservableCollection<Dish> SaladsItems { get; set; }
        public ObservableCollection<Dish> DrinksItems { get; set; }
        public ObservableCollection<Dish> FruitsItems { get; set; }

        private PlannerContext _context;
        private int _currentWeekNumber;
        private int _currentYear;

        public WeekPage()
        {
            // Инициализация коллекций
            HotDishesItems = new ObservableCollection<Dish>();
            HotFirstItems = new ObservableCollection<Dish>();
            SecondDishesItems = new ObservableCollection<Dish>();
            GarnishItems = new ObservableCollection<Dish>();
            SaladsItems = new ObservableCollection<Dish>();
            DrinksItems = new ObservableCollection<Dish>();
            FruitsItems = new ObservableCollection<Dish>();

            InitializeComponent();
            this.DataContext = this;

            _context = new PlannerContext();

            // Устанавливаем текущую неделю и год
            var currentDate = DateTime.Now;
            _currentWeekNumber = GetIso8601WeekOfYear(currentDate);
            _currentYear = currentDate.Year;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataFromDatabase();
            LoadSavedMenu();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                // Загружаем блюда с типами из базы данных
                var dishesWithTypes = _context.Dishes
                    .Include(d => d.CourseType)
                    .ToList();

                // Очищаем коллекции
                ClearAllCollections();

                // Распределяем блюда по категориям
                foreach (var dish in dishesWithTypes)
                {
                    switch (dish.CourseTypeId)
                    {
                        case 1: HotDishesItems.Add(dish); break;      // Горячее блюдо
                        case 2: HotFirstItems.Add(dish); break;       // Горячее первое
                        case 3: SecondDishesItems.Add(dish); break;   // Второе
                        case 4: GarnishItems.Add(dish); break;        // Гарнир
                        case 5: SaladsItems.Add(dish); break;         // Салат
                        case 6: DrinksItems.Add(dish); break;         // Напиток
                        case 7: FruitsItems.Add(dish); break;         // Фрукты
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных из базы: {ex.Message}");
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
            DayOfWeek day = CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private void ClearAllCollections()
        {
            HotDishesItems.Clear();
            HotFirstItems.Clear();
            SecondDishesItems.Clear();
            GarnishItems.Clear();
            SaladsItems.Clear();
            DrinksItems.Clear();
            FruitsItems.Clear();
        }

        private void ComboBoxItemMenu_Selected(object sender, RoutedEventArgs e)
        {
            if (ComboBoxItemMenu.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                string pageType = selectedItem.Tag.ToString();

                switch (pageType)
                {
                    case "CatalogPage":
                        mainWindow.MainFramePublic.Content = new CatalogPage();
                        break;
                    case "NextWeekPage":
                        mainWindow.MainFramePublic.Content = new NextWeekPage();
                        break;
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}