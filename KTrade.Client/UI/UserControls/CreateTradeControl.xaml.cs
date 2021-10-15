using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KTrade.Client.UI.UserControls
{
    /// <summary>
    /// Логика взаимодействия для CreateDealControl.xaml
    /// </summary>
    public partial class CreateDealControl : UserControl
    {
        public CreateDealControl()
        {
            InitializeComponent();
        }

        private void PreviewDigitInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
