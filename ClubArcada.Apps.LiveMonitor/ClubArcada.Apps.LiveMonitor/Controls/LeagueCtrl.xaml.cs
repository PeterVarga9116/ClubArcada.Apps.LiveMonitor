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
    public partial class LeagueCtrl : UserControl, ISlideControl
    {
        public LeagueCtrl()
        {
            InitializeComponent();
            Background = Brushes.Green;
            Canvas.SetLeft(this, 1920);
        }
    }
}
