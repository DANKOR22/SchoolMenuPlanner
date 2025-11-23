using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SchoolMenuPlanner.Pages
{
    /// <summary>
    /// Логика взаимодействия для NextWeekPage.xaml
    /// </summary>
    public partial class NextWeekPage : Page
    {
        public NextWeekPage()
        {
            InitializeComponent();
        }

        private void ComboBoxItemMenu_Selected(object sender, RoutedEventArgs e)
        {
            if (ComboBoxItemMenu.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                string pageType = selectedItem.Tag.ToString();

                switch (pageType)
                {
                    case "WeekPage":
                        mainWindow.MainFramePublic.Content = new WeekPage();
                        break;
                    case "CatalogPage":
                        mainWindow.MainFramePublic.Content = new CatalogPage();
                        break;
                }
            }
        }
    }
}
