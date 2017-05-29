using ClubArcada.Common;
using ClubArcada.Common.BusinessObjects.DataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ClubArcada.Apps.LiveMonitor.Controls
{
    /// <summary>
    /// Interaction logic for CashCtrl.xaml
    /// </summary>
    public partial class CashCtrl : UserControl, ISlideControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<sp_get_cash_league_ladderResult> _players;
        public ObservableCollection<sp_get_cash_league_ladderResult> Players
        {
            get { return _players; }
            set { _players = value; PropertyChanged.Raise(() => Players); }
        }

        public ObservableCollection<CashPlayerDisplayItem> _playersLeft;
        public ObservableCollection<CashPlayerDisplayItem> PlayersLeft
        {
            get { return _playersLeft; }
            set { _playersLeft = value; PropertyChanged.Raise(() => PlayersLeft); }
        }

        public ObservableCollection<CashPlayerDisplayItem> _playersRight;
        public ObservableCollection<CashPlayerDisplayItem> PlayersRight
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

        public CashCtrl()
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
            Players = new ObservableCollection<sp_get_cash_league_ladderResult>();
            PlayersLeft = new ObservableCollection<CashPlayerDisplayItem>();
            PlayersRight = new ObservableCollection<CashPlayerDisplayItem>();
            LoadPlayers();

            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 20);
            timer.Tick += delegate { LoadPlayers(); };
            timer.Start();
        }

        private void LoadPlayers()
        {
            var players = new List<sp_get_cash_league_ladderResult>();

            var bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                var league = Common.BusinessObjects.Data.LeagueData.GetActiveLeague(App.CR);
                players = Common.BusinessObjects.Data.CashPlayerData.GetCashLadder(App.CR, league.Id, 18).OrderByDescending(p => p.Points).ToList();
            };

            bw.RunWorkerCompleted += delegate
            {
                Players.Clear();
                PlayersLeft.Clear();
                PlayersRight.Clear();

                Players.AddRange(players);
                Players.OrderByDescending(p => p.Points).ToList();
                PlayersLeft.AddRange(Players.Take(9).Select(p => CashPlayerDisplayItem.New(p)).ToList());
                PlayersRight.AddRange(Players.Skip(9).Select(p => CashPlayerDisplayItem.New(p)).ToList());

                PlayersLeft.ForEach(p => p.Refresh());
                PlayersRight.ForEach(p => p.Refresh());
            };

            bw.RunWorkerAsync();
        }

    }

    public class CashPlayerDisplayItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Refresh()
        {
            PropertyChanged.Raise(() => LiveVisibility);
            PropertyChanged.Raise(() => DisplayName);
            PropertyChanged.Raise(() => PointInfo);
            PropertyChanged.Raise(() => ForeGround);
        }

        public static CashPlayerDisplayItem New(sp_get_cash_league_ladderResult result)
        {
            return new CashPlayerDisplayItem()
            {
                NickName = result.NickName,
                Points = result.Points.ToString(),
                PointsInt = result.Points.Value,
                IsLive = result.State.HasValue && result.State.Value == 1,
                Rank = ((int)result.Rank.Value),
                PlayCount = result.PlayCount.Value.ToString()
            };

        }

        public bool IsLive { get; set; }

        public int PointsInt { get; set; }

        public string Points { get; set; }

        public string PlayCount { get; set; }

        public string NickName { get; set; }

        public int Rank { get; set; }

        public string DisplayName { get { return string.Format("{0}. {1}", Rank.ToString("00"), NickName); } set { } }

        public string PointInfo { get { return string.Format("{0}", Points); } set { } }

        public Visibility LiveVisibility { get { return IsLive ? Visibility.Visible : Visibility.Collapsed; } set { } }

        public Brush ForeGround { get { return PointsInt < 1000 ? Brushes.Red : Brushes.LightGreen;  } }

    }
}
