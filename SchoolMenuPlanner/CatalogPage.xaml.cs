using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SchoolMenuPlanner.Data;
using SchoolMenuPlanner.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows.Navigation;

namespace SchoolMenuPlanner
{
    public partial class CatalogPage : Page
    {
        // Коллекции для каждой категории согласно структуре БД
        public ObservableCollection<Dish> HotDishesItems { get; set; }           // Горячее блюдо (course_type_id = 1)
        public ObservableCollection<Dish> HotFirstItems { get; set; }            // Горячее первое (course_type_id = 2)
        public ObservableCollection<Dish> SecondDishesItems { get; set; }        // Второе (course_type_id = 3)
        public ObservableCollection<Dish> GarnishItems { get; set; }             // Гарнир (course_type_id = 4)
        public ObservableCollection<Dish> SaladsItems { get; set; }              // Салат (course_type_id = 5)
        public ObservableCollection<Dish> DrinksItems { get; set; }              // Напиток (course_type_id = 6)
        public ObservableCollection<Dish> FruitsItems { get; set; }              // Фрукты (course_type_id = 7)

        private PlannerContext _context;

        public CatalogPage()
        {
            // Инициализация всех коллекций
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

            // Загружаем данные при создании страницы
            Loaded += (s, e) => LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                // Очищаем все коллекции
                ClearAllCollections();

                // Загружаем блюда с типами из базы данных
                var dishesWithTypes = _context.Dishes
                    .Include(d => d.CourseType)
                    .ToList();

                // Отладочная информация
                Console.WriteLine($"Загружено блюд: {dishesWithTypes.Count}");
                foreach (var dish in dishesWithTypes)
                {
                    Console.WriteLine($"Блюдо: {dish.Name}, CourseTypeId: {dish.CourseTypeId}");
                }

                // Распределяем блюда по категориям согласно course_type_id
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

                // Отладочная информация о распределении
                Console.WriteLine($"HotDishes: {HotDishesItems.Count}");
                Console.WriteLine($"HotFirst: {HotFirstItems.Count}");
                Console.WriteLine($"SecondDishes: {SecondDishesItems.Count}");
                Console.WriteLine($"Garnish: {GarnishItems.Count}");
                Console.WriteLine($"Salads: {SaladsItems.Count}");
                Console.WriteLine($"Drinks: {DrinksItems.Count}");
                Console.WriteLine($"Fruits: {FruitsItems.Count}");

                // Привязываем данные к ListView
                BindDataToViews();

                UpdateTotalCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных из базы: {ex.Message}");
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
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

        private void BindDataToViews()
        {
            HotDishesListView.ItemsSource = HotDishesItems;
            HotFirstListView.ItemsSource = HotFirstItems;
            SecondDishesListView.ItemsSource = SecondDishesItems;
            GarnishListView.ItemsSource = GarnishItems;
            SaladsListView.ItemsSource = SaladsItems;
            DrinksListView.ItemsSource = DrinksItems;
            FruitsListView.ItemsSource = FruitsItems;
        }

        private async void AddTextBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.Tag is string categoryTag)
            {
                try
                {
                    // Определяем course_type_id по тегу
                    int courseTypeId = GetCourseTypeIdByTag(categoryTag);

                    // Создаем новое блюдо
                    var newDish = new Dish
                    {
                        Name = "Новое блюдо",
                        CourseTypeId = courseTypeId
                    };

                    // Добавляем в базу данных
                    _context.Dishes.Add(newDish);
                    await _context.SaveChangesAsync();

                    // Добавляем в соответствующую коллекцию
                    AddDishToCollection(newDish, categoryTag);

                    UpdateTotalCount();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении блюда: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private int GetCourseTypeIdByTag(string categoryTag)
        {
            return categoryTag switch
            {
                "HotDishes" => 1,      // Горячее блюдо
                "HotFirst" => 2,       // Горячее первое
                "SecondDishes" => 3,   // Второе
                "Garnish" => 4,        // Гарнир
                "Salads" => 5,         // Салат
                "Drinks" => 6,         // Напиток
                "Fruits" => 7,         // Фрукты
                _ => 1
            };
        }

        private void AddDishToCollection(Dish dish, string categoryTag)
        {
            switch (categoryTag)
            {
                case "HotDishes":
                    HotDishesItems.Add(dish);
                    break;
                case "HotFirst":
                    HotFirstItems.Add(dish);
                    break;
                case "SecondDishes":
                    SecondDishesItems.Add(dish);
                    break;
                case "Garnish":
                    GarnishItems.Add(dish);
                    break;
                case "Salads":
                    SaladsItems.Add(dish);
                    break;
                case "Drinks":
                    DrinksItems.Add(dish);
                    break;
                case "Fruits":
                    FruitsItems.Add(dish);
                    break;
            }
        }

        private async void RemoveItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock removeButton && removeButton.DataContext is Dish dish)
            {
                try
                {
                    // Подтверждение удаления
                    var result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить блюдо \"{dish.Name}\"?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Удаляем из базы данных
                        _context.Dishes.Remove(dish);
                        await _context.SaveChangesAsync();

                        // Удаляем из всех коллекций
                        RemoveDishFromAllCollections(dish);

                        UpdateTotalCount();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении блюда: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveDishFromAllCollections(Dish dish)
        {
            HotDishesItems.Remove(dish);
            HotFirstItems.Remove(dish);
            SecondDishesItems.Remove(dish);
            GarnishItems.Remove(dish);
            SaladsItems.Remove(dish);
            DrinksItems.Remove(dish);
            FruitsItems.Remove(dish);
        }

        private async void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is Dish dish)
            {
                try
                {
                    // Сохраняем изменения в базе данных
                    dish.Name = textBox.Text;
                    _context.Dishes.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateTotalCount()
        {
            try
            {
                int totalCount = HotDishesItems.Count + HotFirstItems.Count +
                               SecondDishesItems.Count + GarnishItems.Count +
                               SaladsItems.Count + DrinksItems.Count +
                               FruitsItems.Count;

                TotalDishesText.Text = $"Всего блюд: {totalCount}";
            }
            catch (Exception ex)
            {
                // В случае ошибки просто показываем базовый текст
                TotalDishesText.Text = "Всего блюд: 0";
            }
        }

        // Обновление данных при загрузке страницы
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataFromDatabase();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }

        // Обработчик для обновления данных (можно вызвать извне)
        public void RefreshData()
        {
            LoadDataFromDatabase();
        }
    }
}