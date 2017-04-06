using ClubArcada.Apps.LiveMonitor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ClubArcada.Apps.LiveMonitor
{
    public partial class MainWindow : Window
    {
        private List<ISlideControl> Controls { get; set; }

        private DispatcherTimer timerImageChange;


        public MainWindow()
        {
            InitializeComponent();

            timerImageChange = new DispatcherTimer();
            timerImageChange.Interval = new TimeSpan(0, 0, 3);
            timerImageChange.Tick += new EventHandler(timerImageChange_Tick);

            Controls = new List<ISlideControl>();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Controls.Add(new TournamentCtrl() { Background = Brushes.Red, Width = gridSlideContainer.ActualWidth, Height = gridSlideContainer.ActualHeight });
            Controls.Add(new TournamentCtrl() { Background = Brushes.Green, Width = gridSlideContainer.ActualWidth, Height = gridSlideContainer.ActualHeight });
            Controls.Add(new TournamentCtrl() { Background = Brushes.Purple, Width = gridSlideContainer.ActualWidth, Height = gridSlideContainer.ActualHeight });

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

        private ISlideControl _currentItem;
        private ISlideControl _nextItem;

        private void PlaySlideShow()
        {
            if (_currentItem == null)
            {
                _currentItem = Controls.First();

                Storyboard StboardFadeIn01 = (Resources["FadeIn"] as Storyboard).Clone();
                StboardFadeIn01.Begin(_currentItem as FrameworkElement);
                _index = 1;
            }
            else
            {

                if (_nextItem != null)
                    _currentItem = _nextItem;

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
                StboardFadeOut.Begin(_currentItem as FrameworkElement);

                Storyboard StboardFadeIn = (Resources["SlideIn"] as Storyboard).Clone();
                StboardFadeIn.Begin(_nextItem as FrameworkElement);

                _currentItem = _nextItem;
            }
        }


    }
}

