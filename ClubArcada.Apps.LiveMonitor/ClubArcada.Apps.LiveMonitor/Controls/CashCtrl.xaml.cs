using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ClubArcada.Apps.LiveMonitor.Controls
{
    /// <summary>
    /// Interaction logic for CashCtrl.xaml
    /// </summary>
    public partial class CashCtrl : UserControl, ISlideControl
    {
        public CashCtrl()
        {
            InitializeComponent();
            Background = Brushes.Blue;
            Canvas.SetLeft(this, 1920);
        }
    }
}
