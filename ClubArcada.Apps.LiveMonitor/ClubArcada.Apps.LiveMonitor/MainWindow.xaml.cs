using ClubArcada.Apps.LiveMonitor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ClubArcada.Common;

namespace ClubArcada.Apps.LiveMonitor
{
    public partial class MainWindow : Window
    {
        private List<ISlideControl> Controls { get; set; }
        private DispatcherTimer timerImageChange;

        static Random rnd = new Random();

        public MainWindow()
        {
            Videos.Add(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"\Images\001.mp4"));
            Videos.Add(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"\Images\002.mp4"));
            Videos.Add(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"\Images\003.mp4"));
            Videos.Add(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"\Images\004.mp4"));
            Videos.Add(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"\Images\005.mp4"));
            Videos.Add(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"\Images\006.mp4"));
            Videos.Add(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"\Images\007.mp4"));

            InitializeComponent();

            timerImageChange = new DispatcherTimer();
            timerImageChange.Interval = new TimeSpan(0, 0, 60);
            timerImageChange.Tick += new EventHandler(timerImageChange_Tick);

            Controls = new List<ISlideControl>();

            meVideoBg.Source = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"\Images\001.mp4");
            meVideoBg.Play();
            meVideoBg.MediaEnded += delegate 
            {
                meVideoBg.Source = Videos[rnd.Next(Videos.Count)];
                meVideoBg.Position = new TimeSpan(0, 0, 0);
                meVideoBg.Play(); };

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Controls.Add(new TournamentCtrl() { Width = gridSlideContainer.ActualWidth, Height = gridSlideContainer.ActualHeight });
            Controls.Add(new CashCtrl() { Width = gridSlideContainer.ActualWidth, Height = gridSlideContainer.ActualHeight });
            Controls.Add(new LeagueCtrl() { Width = gridSlideContainer.ActualWidth, Height = gridSlideContainer.ActualHeight });

            foreach (var c in Controls)
                gridSlideContainer.Children.Add(c as FrameworkElement);

            PlaySlideShow();
            timerImageChange.IsEnabled = true;
        }

        private void timerImageChange_Tick(object sender, EventArgs e)
        {
            PlaySlideShow();
        }

        private int _index = 0;

        private void SetLogo()
        {
            if(CurrentItem.GetType() == typeof(LeagueCtrl))
            {
                imgLogo.Visibility = Visibility.Visible;
                imgLogo.Source = new BitmapImage(new Uri(@"/Images/logo_lvpt.png", UriKind.Relative));
            }
            if (CurrentItem.GetType() == typeof(TournamentCtrl))
            {
                imgLogo.Visibility = Visibility.Collapsed;
                //txtLogo.Text = "Upcoming Tournaments";
            }
            if (CurrentItem.GetType() == typeof(CashCtrl))
            {
                imgLogo.Visibility = Visibility.Visible;
                imgLogo.Source = new BitmapImage(new Uri(@"/Images/logo_cgl.png", UriKind.Relative));
            }
        }

        private ISlideControl _currentItem;
        private ISlideControl CurrentItem
        {
            get
            {
                return _currentItem;
            }
            set
            {
                _currentItem = value;
                SetLogo();
            }
        }


        private ISlideControl _nextItem;

        private void PlaySlideShow()
        {
            if (CurrentItem == null)
            {
                CurrentItem = Controls.First();

                Storyboard StboardFadeIn01 = (Resources["FadeIn"] as Storyboard).Clone();
                StboardFadeIn01.Begin(CurrentItem as FrameworkElement);
                _index = 1;
            }
            else
            {

                if (_nextItem != null)
                    CurrentItem = _nextItem;

                if (_index < Controls.Count)
                {
                    _nextItem = Controls[_index];
                }
                else
                {
                    _nextItem = Controls[0];
                    _index = 0;

                }

                _index++;

                Storyboard StboardFadeOut = (Resources["SlideOut"] as Storyboard).Clone();
                StboardFadeOut.Begin(CurrentItem as FrameworkElement);

                Storyboard StboardFadeIn = (Resources["SlideIn"] as Storyboard).Clone();
                StboardFadeIn.Begin(_nextItem as FrameworkElement);

                CurrentItem = _nextItem;
            }
        }

        public List<Uri> Videos = new List<Uri>();

    }
}

