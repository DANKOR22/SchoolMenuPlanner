using Microsoft.EntityFrameworkCore;
using SchoolMenuPlanner.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMenuPlanner.Classes
{
    public class MenuDataService
    {
        private static MenuDataService _instance;
        public static MenuDataService Instance => _instance ??= new MenuDataService();

        public ObservableCollection<Dish> HotDishesItems { get; } = new ObservableCollection<Dish>();
        public ObservableCollection<Dish> HotFirstItems { get; } = new ObservableCollection<Dish>();
        public ObservableCollection<Dish> SecondDishesItems { get; } = new ObservableCollection<Dish>();
        public ObservableCollection<Dish> GarnishItems { get; } = new ObservableCollection<Dish>();
        public ObservableCollection<Dish> SaladsItems { get; } = new ObservableCollection<Dish>();
        public ObservableCollection<Dish> DrinksItems { get; } = new ObservableCollection<Dish>();
        public ObservableCollection<Dish> FruitsItems { get; } = new ObservableCollection<Dish>();

        private bool _isDataLoaded = false;

        public async Task LoadDataAsync()
        {
            if (_isDataLoaded) return;

            try
            {
                using var context = new PlannerContext();
                var dishesWithTypes = await context.Dishes
                    .Include(d => d.CourseType)
                    .ToListAsync();

                ClearAllCollections();

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

                _isDataLoaded = true;
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
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

        public void RefreshData()
        {
            _isDataLoaded = false;
        }
    }
}