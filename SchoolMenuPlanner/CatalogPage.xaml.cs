using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SchoolMenuPlanner
{
    public partial class CatalogPage : Page
    {
        public ObservableCollection<ItemDish> BreakfastItems { get; set; }
        public ObservableCollection<ItemDish> LunchItems { get; set; }

        public CatalogPage()
        {
            // Инициализация свойств ДО InitializeComponent()
            BreakfastItems = new ObservableCollection<ItemDish>();
            LunchItems = new ObservableCollection<ItemDish>();

            InitializeComponent();

            // Заполнение данными после инициализации
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            // Данные для завтрака
            BreakfastItems.Add(new ItemDish { StringWithDish = "Каша овсяная" });
            BreakfastItems.Add(new ItemDish { StringWithDish = "Бутерброд с сыром" });
            BreakfastItems.Add(new ItemDish { StringWithDish = "Чай" });

            // Данные для обеда
            LunchItems.Add(new ItemDish { StringWithDish = "Борщ" });
            LunchItems.Add(new ItemDish { StringWithDish = "Котлета куриная" });
            LunchItems.Add(new ItemDish { StringWithDish = "Пюре картофельное" });
            LunchItems.Add(new ItemDish { StringWithDish = "Компот" });

            // Привязка данных
            BreakfastListView.ItemsSource = BreakfastItems;
            LunchListView.ItemsSource = LunchItems;

            UpdateTotalCount();
        }

        private void AddTextBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                string? listType = textBlock.Tag as string;

                var newItem = new ItemDish { StringWithDish = "Новое блюдо" };

                if (listType == "Breakfast")
                {
                    BreakfastItems.Add(newItem);
                }
                else if (listType == "Lunch")
                {
                    LunchItems.Add(newItem);
                }

                UpdateTotalCount();
            }
        }

        private void RemoveItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock removeButton && removeButton.DataContext is ItemDish item)
            {
                if (BreakfastItems.Contains(item))
                {
                    BreakfastItems.Remove(item);
                }
                else if (LunchItems.Contains(item))
                {
                    LunchItems.Remove(item);
                }

                UpdateTotalCount();
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
    }
}