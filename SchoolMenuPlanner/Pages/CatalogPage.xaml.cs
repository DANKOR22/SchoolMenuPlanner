using Microsoft.EntityFrameworkCore;
using SchoolMenuPlanner.Classes;
using SchoolMenuPlanner.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SchoolMenuPlanner
{
    public partial class CatalogPage : Page
    {
        // Коллекции для каждой категории согласно структуре БД
        public ObservableCollection<Dish> HotDishesItems { get; set; }
        public ObservableCollection<Dish> HotFirstItems { get; set; }
        public ObservableCollection<Dish> SecondDishesItems { get; set; }
        public ObservableCollection<Dish> GarnishItems { get; set; }
        public ObservableCollection<Dish> SaladsItems { get; set; }
        public ObservableCollection<Dish> DrinksItems { get; set; }
        public ObservableCollection<Dish> FruitsItems { get; set; }

        private Dish? _newlyAddedDish;

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

            // Загружаем данные при создании страницы
            Loaded += (s, e) => LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                using var context = new PlannerContext();

                // Очищаем все коллекции
                ClearAllCollections();

                // Загружаем блюда с типами из базы данных
                var dishesWithTypes = context.Dishes
                    .Include(d => d.CourseType)
                    .ToList();

                // Распределяем блюда по категориям согласно course_type_id
                foreach (var dish in dishesWithTypes)
                {
                    switch (dish.CourseTypeId)
                    {
                        case 1: HotDishesItems.Add(dish); break;
                        case 2: HotFirstItems.Add(dish); break;
                        case 3: SecondDishesItems.Add(dish); break;
                        case 4: GarnishItems.Add(dish); break;
                        case 5: SaladsItems.Add(dish); break;
                        case 6: DrinksItems.Add(dish); break;
                        case 7: FruitsItems.Add(dish); break;
                    }
                }

                // Привязываем данные к ListView
                BindDataToViews();

                UpdateTotalCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных из базы: {ex.Message}");
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

                    // Используем отдельный контекст для добавления
                    using (var context = new PlannerContext())
                    {
                        context.Dishes.Add(newDish);
                        await context.SaveChangesAsync();
                    }

                    // Добавляем в соответствующую коллекцию В НАЧАЛО (сверху)
                    AddDishToCollectionAtTop(newDish, categoryTag);

                    // Запоминаем только что добавленное блюдо
                    _newlyAddedDish = newDish;

                    UpdateTotalCount();

                    // Фокусируемся на новом TextBox
                    FocusNewTextBox(newDish, categoryTag);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении блюда: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddDishToCollectionAtTop(Dish dish, string categoryTag)
        {
            switch (categoryTag)
            {
                case "HotDishes":
                    HotDishesItems.Insert(0, dish);
                    break;
                case "HotFirst":
                    HotFirstItems.Insert(0, dish);
                    break;
                case "SecondDishes":
                    SecondDishesItems.Insert(0, dish);
                    break;
                case "Garnish":
                    GarnishItems.Insert(0, dish);
                    break;
                case "Salads":
                    SaladsItems.Insert(0, dish);
                    break;
                case "Drinks":
                    DrinksItems.Insert(0, dish);
                    break;
                case "Fruits":
                    FruitsItems.Insert(0, dish);
                    break;
            }
        }

        private void FocusNewTextBox(Dish dish, string categoryTag)
        {
            // Находим соответствующий ListView
            ListView? listView = GetListViewByCategory(categoryTag);
            if (listView != null)
            {
                // Обновляем ItemsSource чтобы убедиться, что данные актуальны
                listView.Items.Refresh();

                // Находим TextBox для нового блюда
                var itemContainer = listView.ItemContainerGenerator.ContainerFromItem(dish) as ListViewItem;
                if (itemContainer != null)
                {
                    var textBox = FindVisualChild<TextBox>(itemContainer);
                    if (textBox != null)
                    {
                        // Фокусируемся и выделяем весь текст
                        textBox.Focus();
                        textBox.SelectAll();

                        // Подписываемся на событие KeyDown для обработки Enter
                        textBox.KeyDown += NewTextBox_KeyDown;
                    }
                }
            }
        }

        private ListView? GetListViewByCategory(string categoryTag)
        {
            return categoryTag switch
            {
                "HotDishes" => HotDishesListView,
                "HotFirst" => HotFirstListView,
                "SecondDishes" => SecondDishesListView,
                "Garnish" => GarnishListView,
                "Salads" => SaladsListView,
                "Drinks" => DrinksListView,
                "Fruits" => FruitsListView,
                _ => null
            };
        }

        private void NewTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox && textBox.DataContext is Dish dish)
            {
                // Если это только что добавленное блюдо, перемещаем его в конец
                if (dish == _newlyAddedDish)
                {
                    MoveDishToBottom(dish);
                    _newlyAddedDish = null;
                }

                // Снимаем фокус
                Keyboard.ClearFocus();

                // Отписываемся от события
                textBox.KeyDown -= NewTextBox_KeyDown;
            }
        }

        private void MoveDishToBottom(Dish dish)
        {
            // Удаляем блюдо из всех коллекций и добавляем в конец нужной
            RemoveDishFromAllCollections(dish);

            // Определяем категорию блюда и добавляем в конец соответствующей коллекции
            switch (dish.CourseTypeId)
            {
                case 1:
                    HotDishesItems.Add(dish);
                    break;
                case 2:
                    HotFirstItems.Add(dish);
                    break;
                case 3:
                    SecondDishesItems.Add(dish);
                    break;
                case 4:
                    GarnishItems.Add(dish);
                    break;
                case 5:
                    SaladsItems.Add(dish);
                    break;
                case 6:
                    DrinksItems.Add(dish);
                    break;
                case 7:
                    FruitsItems.Add(dish);
                    break;
            }

            // Обновляем привязки
            BindDataToViews();
        }

        private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result)
                    return result;
                else
                {
                    var descendant = FindVisualChild<T>(child);
                    if (descendant != null)
                        return descendant;
                }
            }
            return null;
        }

        private static int GetCourseTypeIdByTag(string categoryTag)
        {
            return categoryTag switch
            {
                "HotDishes" => 1,
                "HotFirst" => 2,
                "SecondDishes" => 3,
                "Garnish" => 4,
                "Salads" => 5,
                "Drinks" => 6,
                "Fruits" => 7,
                _ => 1
            };
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
                        // Используем отдельный контекст для удаления
                        using (var context = new PlannerContext())
                        {
                            // Находим блюдо в базе по ID
                            var dishToDelete = context.Dishes.Find(dish.Id);
                            if (dishToDelete != null)
                            {
                                context.Dishes.Remove(dishToDelete);
                                await context.SaveChangesAsync();
                            }
                        }

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
                    // Используем отдельный контекст для обновления
                    using (var context = new PlannerContext())
                    {
                        var dishToUpdate = context.Dishes.Find(dish.Id);
                        if (dishToUpdate != null)
                        {
                            dishToUpdate.Name = textBox.Text.Trim();
                            await context.SaveChangesAsync();

                            // Обновляем объект в коллекции
                            dish.Name = dishToUpdate.Name;
                        }
                    }

                    // Отписываемся от события KeyDown если это было новое блюдо
                    textBox.KeyDown -= NewTextBox_KeyDown;
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
            catch (Exception)
            {
                TotalDishesText.Text = "Всего блюд: 0";
            }
        }

        // Обновление данных при загрузке страницы
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataFromDatabase();
        }

        // Обработчик для обновления данных (можно вызвать извне)
        public void RefreshData()
        {
            LoadDataFromDatabase();
        }

        private void ComboBoxItemMenu_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxItemMenu.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is string pageType)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    switch (pageType)
                    {
                        case "WeekPage":
                            mainWindow.MainFramePublic.Content = new WeekPage();
                            break;
                        case "NextWeekPage":
                            mainWindow.MainFramePublic.Content = new NextWeekPage();
                            break;
                    }
                }
            }
        }
    }
}