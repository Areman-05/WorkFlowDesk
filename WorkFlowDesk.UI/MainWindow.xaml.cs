using System.Windows;
using WorkFlowDesk.UI.Services;

namespace WorkFlowDesk.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NavigationService _navigationService;

        public MainWindow()
        {
            InitializeComponent();
            _navigationService = new NavigationService();
            _navigationService.Initialize(ContentArea);
        }
    }
}