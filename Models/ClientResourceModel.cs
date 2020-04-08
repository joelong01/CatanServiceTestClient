using CatanServiceMonitor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CatanSharedModels
{
    /// <summary>
    ///     This class contains all data needed to create the UI with binding
    /// </summary>
    public class ClientResourceModel : INotifyPropertyChanged
    {
      
        public event PropertyChangedEventHandler PropertyChanged;
        private PlayerResources _playerResources = new PlayerResources();
        private TradeResources _tradeResourcs = new TradeResources();
        public ObservableCollection<string> Players { get; } = new ObservableCollection<string>();
        public ClientResourceModel This => this;
        public PlayerResources PlayerResources
        {
            get
            {
                return _playerResources;
            }
            set
            {
                if (value != _playerResources)
                {
                    _playerResources = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("CanBuyDevCard");
                    NotifyPropertyChanged("CanBuySettlement");
                    NotifyPropertyChanged("CanBuyCity");
                    NotifyPropertyChanged("CanBuyRoad");
                    NotifyPropertyChanged("CanTradeGold");


                }
            }
        }
        public TradeResources TradeResources
        {
            get
            {
                return _tradeResourcs;
            }
            set
            {
                if (value != _tradeResourcs)
                {
                    _tradeResourcs = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int GetDevCardCount(DevCardType cardType, bool played)
        {
            int count = 0;
            foreach (var cards in PlayerResources.DevCards)
            {
                if (cards.DevCard == cardType && cards.Played == played)
                {
                    count++;
                }
            }
            return count;
        }
        public int VictoryPoints => GetDevCardCount(DevCardType.VictoryPoint, false);
        public int KnightsPlayed => GetDevCardCount(DevCardType.Knight, true);
        public int MonopolyPlayed => GetDevCardCount(DevCardType.Monopoly, true);
        public int RoadBuildingPlayed => GetDevCardCount(DevCardType.RoadBuilding, true);
        public int YearOfPlentyPlayed => GetDevCardCount(DevCardType.YearOfPlenty, true);
        public int KnightsNotPlayed => GetDevCardCount(DevCardType.Knight, false);
        public int MonopolyNotPlayed => GetDevCardCount(DevCardType.Monopoly, false);
        public int RoadBuildingNotPlayed => GetDevCardCount(DevCardType.RoadBuilding, false);
        public int YearOfPlentyNotPlayed => GetDevCardCount(DevCardType.YearOfPlenty, false);
        public int TotalNotPlayed => KnightsNotPlayed + RoadBuildingNotPlayed + MonopolyNotPlayed + YearOfPlentyNotPlayed;
        public int ResourceCount(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Sheep:
                    return this.PlayerResources.Sheep;
                case ResourceType.Wood:
                    return this.PlayerResources.Wood;
                case ResourceType.Ore:
                    return this.PlayerResources.Ore;
                case ResourceType.Wheat:
                    return this.PlayerResources.Wheat;
                case ResourceType.Brick:
                    return this.PlayerResources.Brick;
                case ResourceType.GoldMine:
                    return this.PlayerResources.GoldMine;
                default:
                    throw new Exception($"Unexpected resource type passed into ResourceCount {resourceType}");
            }
        }
        
        public bool CanBuyDevCard => (PlayerResources.Wheat > 0 && PlayerResources.Sheep > 0 && PlayerResources.Ore > 0);
        
        public bool CanBuySettlement => (PlayerResources.Wheat > 0 && PlayerResources.Sheep > 0 && PlayerResources.Brick > 0 && PlayerResources.Wood > 0);
        
        public bool CanBuyCity => (PlayerResources.Wheat > 1 && PlayerResources.Ore > 2);
        public bool CanBuyRoad => (PlayerResources.Brick > 0 && PlayerResources.Wood > 0);
        public bool CanTradeGold => (PlayerResources.GoldMine > 0);
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }

}
