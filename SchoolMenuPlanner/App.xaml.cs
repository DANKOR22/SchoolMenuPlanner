using SchoolMenuPlanner.Classes;
using System.Windows;

namespace SchoolMenuPlanner
{
    public partial class App : Application
    {
        public static User? CurrentUser { get; private set; }
    }
}