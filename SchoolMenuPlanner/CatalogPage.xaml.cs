using Microsoft.EntityFrameworkCore;
using SchoolMenuPlanner.Data;
using SchoolMenuPlanner.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SchoolMenuPlanner
{
    public partial class CatalogPage : Page
    {
        public ObservableCollection<Dish> BreakfastItems { get; set; }
        public ObservableCollection<Dish> LunchItems { get; set; }
        private MenuPlannerContext _context;

        public CatalogPage()
        {
            BreakfastItems = new ObservableCollection<Dish>();
            LunchItems = new ObservableCollection<Dish>();

            InitializeComponent();

            _context = new MenuPlannerContext();
            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                BreakfastItems.Clear();
                LunchItems.Clear();

                // Проверяем существование таблицы
                if (!_context.Database.CanConnect())
                {
                    AddTestData();
                    return;
                }

                // Получаем все блюда из базы
                var allDishes = _context.Dishes.ToList();

                if (allDishes.Any())
                {
                    // Если есть данные в БД - используем их
                    // Распределяем блюда между завтраком и обедом
                    for (int i = 0; i < allDishes.Count; i++)
                    {
                        if (i % 2 == 0)
                            BreakfastItems.Add(allDishes[i]);
                        else
                            LunchItems.Add(allDishes[i]);
                    }
                }
                else
                {
                    // Если таблица пустая - добавляем тестовые данные
                    AddInitialDataToDatabase();
                    LoadDataFromDatabase(); // Перезагружаем
                    return;
                }

                BreakfastListView.ItemsSource = BreakfastItems;
                LunchListView.ItemsSource = LunchItems;
                UpdateTotalCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                AddTestData();
            }
        }

        private void AddInitialDataToDatabase()
        {
            try
            {
                var initialDishes = new List<Dish>
                {
                    new Dish { Name = "Каша овсяная" },
                    new Dish { Name = "Бутерброд с сыром" },
                    new Dish { Name = "Чай" },
                    new Dish { Name = "Борщ" },
                    new Dish { Name = "Котлета куриная" },
                    new Dish { Name = "Пюре картофельное" }
                };

                _context.Dishes.AddRange(initialDishes);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления初始数据: {ex.Message}");
            }
        }

        private void AddTestData()
        {
            // Добавляем тестовые данные в память
            BreakfastItems.Add(new Dish { Id = 1, Name = "Каша овсяная" });
            BreakfastItems.Add(new Dish { Id = 2, Name = "Бутерброд с сыром" });
            BreakfastItems.Add(new Dish { Id = 3, Name = "Чай" });

            LunchItems.Add(new Dish { Id = 4, Name = "Борщ" });
            LunchItems.Add(new Dish { Id = 5, Name = "Котлета куриная" });
            LunchItems.Add(new Dish { Id = 6, Name = "Пюре картофельное" });

            UpdateTotalCount();
        }

        private async void AddTextBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.Tag is string listType)
            {
                var newDish = new Dish { Name = "Новое блюдо" };

                try
                {
                    _context.Dishes.Add(newDish);
                    await _context.SaveChangesAsync();

                    if (listType == "Breakfast")
                        BreakfastItems.Add(newDish);
                    else if (listType == "Lunch")
                        LunchItems.Add(newDish);

                    UpdateTotalCount();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка добавления: {ex.Message}");
                }
            }
        }

        private async void RemoveItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock removeButton && removeButton.DataContext is Dish dish)
            {
                try
                {
                    _context.Dishes.Remove(dish);
                    await _context.SaveChangesAsync();

                    BreakfastItems.Remove(dish);
                    LunchItems.Remove(dish);

                    UpdateTotalCount();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private async void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is Dish dish)
            {
                try
                {
                    dish.Name = textBox.Text;
                    _context.Dishes.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                }
            }
        }

        private void UpdateTotalCount()
        {
            int totalCount = BreakfastItems.Count + LunchItems.Count;

            foreach (var item in MainStatusBar.Items)  // ← Используем имя MainStatusBar
            {
                if (item is StatusBarItem statusBarItem)
                {
                    if (statusBarItem.Content is TextBlock textBlock && textBlock.Text.StartsWith("Всего блюд:"))
                    {
                        textBlock.Text = $"Всего блюд: {totalCount}";
                        break;
                    }
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}