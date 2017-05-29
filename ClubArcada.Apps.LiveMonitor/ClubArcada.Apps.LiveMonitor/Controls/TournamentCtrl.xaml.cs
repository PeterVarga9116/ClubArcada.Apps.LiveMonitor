using ClubArcada.Common;
using ClubArcada.Common.BusinessObjects.DataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;

namespace ClubArcada.Apps.LiveMonitor.Controls
{
    public partial class TournamentCtrl : UserControl, ISlideControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Tournament> _tournaments { get; set; }
        public ObservableCollection<Tournament> Tournaments
        {
            get
            {
                return _tournaments;
            }
            set
            {
                _tournaments = value;
                PropertyChanged.Raise(() => Tournaments);
            }
        }

        public TournamentCtrl()
        {
            InitializeComponent();
            DataContext = this;
            Canvas.SetLeft(this, 1920);
            StartSession();
        }

        private void StartSession()
        {
            Tournaments = new ObservableCollection<Tournament>();
            LoadTournaments();

            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 20);
            timer.Tick += delegate { LoadTournaments(); };
            timer.Start();
        }

        private void LoadTournaments()
        {
            var tournaments = new List<Tournament>();

            var bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                tournaments = Common.BusinessObjects.Data.TournamentData.GetUpcomingList(App.CR).OrderBy(t => t.Date).ToList();
            };

            bw.RunWorkerCompleted += delegate
            {
                Tournaments.Clear();
                Tournaments.AddRange(tournaments);
            };

            bw.RunWorkerAsync();
        }
    }
}
