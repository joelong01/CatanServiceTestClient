﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace CatanSharedModels
{
    public enum TileOrientation { FaceDown, FaceUp, None };
    public enum HarborType { Sheep, Wood, Ore, Wheat, Brick, ThreeForOne, Uninitialized, None };

    public enum Entitlement { Undefined, DevCard, Settlement, City, Road }

    public enum ResourceType { Sheep, Wood, Ore, Wheat, Brick, Desert, Back, None, Sea, GoldMine, VictoryPoint, Knight, YearOfPlenty, RoadBuilding, Monopoly };
    public enum DevCardType { Knight, VictoryPoint, YearOfPlenty, RoadBuilding, Monopoly, Unknown };

    public class GameInfo
    {
        public int MaxRoads { get; set; } = 15;
        public int MaxCities { get; set; } = 4;
        public int MaxSettlements { get; set; } = 5;
        public int MaxResourceAllocated { get; set; } = 19; // most aggregate resource per type
        public bool AllowShips { get; set; } = false;
        public int Knights { get; set; } = 14;
        public int VictoryCards { get; set; } = 5;
        public int YearOfPlenty { get; set; } = 2;
        public int RoadBuilding { get; set; } = 2;
        public int Monopoly { get; set; } = 2;
        public static bool operator ==(GameInfo a, GameInfo b)
        {
            return
                (
                    a.MaxRoads == b.MaxRoads &&
                    a.MaxCities == b.MaxCities &&
                    a.MaxSettlements == b.MaxSettlements &&
                    a.Knights == b.Knights &&
                    a.MaxResourceAllocated == b.MaxResourceAllocated &&
                    a.AllowShips == b.AllowShips &&
                    a.VictoryCards == b.VictoryCards &&
                    a.YearOfPlenty == b.YearOfPlenty &&
                    a.RoadBuilding == b.RoadBuilding &&
                    a.Monopoly == b.Monopoly
                );
        }
        public static bool operator !=(GameInfo a, GameInfo b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return (GameInfo)obj == this;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class CatanResult
    {
        public List<KeyValuePair<string, object>> ExtendedInformation { get; } = new List<KeyValuePair<string, object>>();
        public string Description { get; set; }
        public string FunctionName { get; set; }
        public string FilePath { get; set; }
        public int LineNumber { get; set; }
        public string Request { get; set; }
        public CatanResult([CallerMemberName] string fName = "", [CallerFilePath] string codeFile = "", [CallerLineNumber] int lineNumber = -1)
        {
            FunctionName = fName;
            FilePath = codeFile;
            LineNumber = lineNumber;
        }
    }
    public class CatanResultWithBody<T> : CatanResult
    {
        public T Body { get; set; }
        public CatanResultWithBody(T body, [CallerMemberName] string fName = "", [CallerFilePath] string codeFile = "", [CallerLineNumber] int lineNumber = -1) : base(fName, codeFile, lineNumber)
        {
            Body = body;
        }

    }

    public static class CatanSerializer
    {
        static public string Serialize<T>(T obj, bool indented = false)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = indented

            };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Serialize<T>(obj, options);
        }
        static public T Deserialize<T>(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<T>(json, options);
        }
    }

    public class DevelopmentCard
    {
        public DevCardType DevCard { get; set; } = DevCardType.Unknown;
        public bool Played { get; set; } = false;
    }

    public class TradeResources : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _wheat = 0;
        private int _wood = 0;
        private int _ore = 0;
        private int _sheep = 0;
        private int _brick = 0;
        private int _goldMine = 0;

        public TradeResources() { }

        public TradeResources(TradeResources tradeResources)
        {
            Wheat = this.Wheat;
            Wood = this.Wood;
            Brick = this.Brick;
            Ore = this.Ore;
            Sheep = this.Sheep;
            GoldMine = this.GoldMine;

        }

        public int Wheat
        {
            get
            {
                return _wheat;
            }
            set
            {
                if (value != _wheat)
                {
                    _wheat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Wood
        {
            get
            {
                return _wood;
            }
            set
            {
                if (value != _wood)
                {
                    _wood = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Ore
        {
            get
            {
                return _ore;
            }
            set
            {
                if (value != _ore)
                {
                    _ore = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int Sheep
        {
            get
            {
                return _sheep;
            }
            set
            {
                if (value != _sheep)
                {
                    _sheep = value;
                }
            }
        }

        public int Brick
        {
            get
            {
                return _brick;
            }
            set
            {
                if (value != _brick)
                {
                    _brick = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int GoldMine
        {
            get
            {
                return _goldMine;
            }
            set
            {
                if (value != _goldMine)
                {
                    _goldMine = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        public TradeResources GetNegated()
        {
            return new TradeResources()
            {
                Wheat = -Wheat,
                Wood = -Wood,
                Ore = -Ore,
                Sheep = -Sheep,
                Brick = -Brick,
            };
        }
    }

    public class PlayerResources
    {
        public string PlayerName { get; set; } = "";
        public string GameName { get; set; } = "";
        public int Wheat { get; set; } = 0;
        public int Wood { get; set; } = 0;
        public int Ore { get; set; } = 0;
        public int Sheep { get; set; } = 0;
        public int Brick { get; set; } = 0;
        public int GoldMine { get; set; } = 0;
        public List<DevelopmentCard> DevCards { get; set; } = new List<DevelopmentCard>();
        public List<Entitlement> Entitlements { get; set; } = new List<Entitlement>();

        [JsonIgnore]
        public int TotalResources => Wheat + Wood + Brick + Ore + Sheep + GoldMine;

        public PlayerResources() { }

    }
}
