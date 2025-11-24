using SchoolMenuPlanner.Classes;
using SchoolMenuPlanner.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace SchoolMenuPlanner.Pages
{
    public abstract class BaseWeekPage : Page
    {
        // Ссылки на общие коллекции
        public ObservableCollection<Dish> HotDishesItems => MenuDataService.Instance.HotDishesItems;
        public ObservableCollection<Dish> HotFirstItems => MenuDataService.Instance.HotFirstItems;
        public ObservableCollection<Dish> SecondDishesItems => MenuDataService.Instance.SecondDishesItems;
        public ObservableCollection<Dish> GarnishItems => MenuDataService.Instance.GarnishItems;
        public ObservableCollection<Dish> SaladsItems => MenuDataService.Instance.SaladsItems;
        public ObservableCollection<Dish> DrinksItems => MenuDataService.Instance.DrinksItems;
        public ObservableCollection<Dish> FruitsItems => MenuDataService.Instance.FruitsItems;

        protected PlannerContext _context;

        protected BaseWeekPage()
        {
            _context = new PlannerContext();
        }

        protected abstract void LoadSavedMenu();

        protected virtual void DisposeContext()
        {
            _context?.Dispose();
        }
    }
}