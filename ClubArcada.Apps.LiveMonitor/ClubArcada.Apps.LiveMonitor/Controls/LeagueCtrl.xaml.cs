using ClubArcada.Common;
using ClubArcada.Common.BusinessObjects.DataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ClubArcada.Apps.LiveMonitor.Controls
{
    public partial class LeagueCtrl : UserControl, ISlideControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<sp_get_poker_league_ladderResult> _players;
        public ObservableCollection<sp_get_poker_league_ladderResult> Players
        {
            get { return _players; }
            set { _players = value; PropertyChanged.Raise(() => Players); }
        }

        public ObservableCollection<TournamentPlayerDisplayItem> _playersLeft;
        public ObservableCollection<TournamentPlayerDisplayItem> PlayersLeft
        {
            get { return _playersLeft; }
            set { _playersLeft = value; PropertyChanged.Raise(() => PlayersLeft); }
        }

        public ObservableCollection<TournamentPlayerDisplayItem> _playersRight;
        public ObservableCollection<TournamentPlayerDisplayItem> PlayersRight
        {
            get { return _playersRight; }
            set { _playersRight = value; PropertyChanged.Raise(() => PlayersRight); }
        }

        public double _itemHeight;
        public double ItemHeight
        {
            get
            {
                return _itemHeight;
            }
            set
            {
                _itemHeight = value;
                PropertyChanged.Raise(() => ItemHeight);
            }
        }

        public LeagueCtrl()
        {
            InitializeComponent();
            Canvas.SetLeft(this, 1920);
            DataContext = this;
            StartSession();
            
            Loaded += LeagueCtrl_Loaded;
        }

        private void LeagueCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            ItemHeight = gridMain.ActualHeight / 9;
        }

        private void StartSession()
        {
            Players = new ObservableCollection<sp_get_poker_league_ladderResult>();
            PlayersLeft = new ObservableCollection<TournamentPlayerDisplayItem>();
            PlayersRight = new ObservableCollection<TournamentPlayerDisplayItem>();
            LoadPlayers();

            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 20);
            timer.Tick += delegate { LoadPlayers(); };
            timer.Start();
        }

        private void LoadPlayers()
        {
            var players = new List<sp_get_poker_league_ladderResult>();

            var bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                var league = Common.BusinessObjects.Data.LeagueData.GetActiveLeague(App.CR);
                players = Common.BusinessObjects.Data.TournamentPlayerData.GetTournamentLadder(App.CR, league.Id, 18).OrderByDescending(p => p.Points).ToList();
            };

            bw.RunWorkerCompleted += delegate
            {
                Players.Clear();
                PlayersLeft.Clear();
                PlayersRight.Clear();

                Players.AddRange(players);
                Players.OrderByDescending(p => p.Points).ToList();
                PlayersLeft.AddRange(Players.Take(9).Select(p => TournamentPlayerDisplayItem.New(p)).ToList());
                PlayersRight.AddRange(Players.Skip(9).Select(p => TournamentPlayerDisplayItem.New(p)).ToList());

                PlayersLeft.ForEach(p => p.Refresh());
                PlayersRight.ForEach(p => p.Refresh());
            };

            bw.RunWorkerAsync();
        }
    }

    public class TournamentPlayerDisplayItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Refresh()
        {
            PropertyChanged.Raise(() => LiveVisibility);
            PropertyChanged.Raise(() => DisplayName);
            PropertyChanged.Raise(() => PointInfo);
        }

        public static TournamentPlayerDisplayItem New(sp_get_poker_league_ladderResult result)
        {
            return new TournamentPlayerDisplayItem()
            {
                NickName = result.NickName,
                Points = result.Points.ToString(),
                IsLive = result.state.HasValue && result.state.Value == 1,
                PlayCount = result.PlayCount.ToString(),
                Rank = ((int)result.Rank.Value)
            };
        }

        public bool IsLive { get; set; }

        public string Points { get; set; }

        public string PlayCount { get; set; }

        public string NickName { get; set; }

        public int Rank { get; set; }

        public string DisplayName { get { return string.Format("{0}. {1}", Rank.ToString("00"), NickName); } set { } }

        public string PointInfo { get { return string.Format("{0} - {1}", PlayCount, Points); } set { } }

        public Visibility LiveVisibility { get { return IsLive ? Visibility.Visible : Visibility.Collapsed; } set { } }

        public override string ToString()
        {
            return string.Format("{0} - IsLive: {1}", NickName, IsLive);
        }

    }
}
