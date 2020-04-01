﻿using System;
using System.Collections.Generic;

namespace CatanSvcTestClient
{
    public enum ResourceType { Sheep, Wood, Ore, Wheat, Brick, Desert, Back, None, Sea, GoldMine };
    public enum DevCardType { Knight, VictoryPoint, YearOfPlenty, RoadBuilding, Monopoly };

    public class ResourceCountClass
    {
        public int Wheat { get; set; }
        public int Wood { get; set; }
        public int Ore { get; set; }
        public int Sheep { get; set; }
        public int Brick { get; set; }
        public int GoldMine { get; set; }
        public int Desert { get; set; }
        public int Back { get; set; }
        public int None { get; set; }
        public int Sea { get; set; }
    }

    public class UserId : IEqualityComparer<UserId>
    {
        public string GameName { get; set; }
        public string UserName { get; set; }
        public override string ToString()
        {
            return $"{GameName} - {UserName}";
        }
        public bool Equals(UserId x, UserId y)
        {
            if (x is null || y is null) return false;

            if ((x.GameName == y.GameName) && (x.UserName == y.UserName)) return true;

            return false;
        }

        public int GetHashCode(UserId obj)
        {
            return obj.GameName.GetHashCode() + obj.UserName.GetHashCode();
        }
    }

    public class UserResources
    {
        public Dictionary<DevCardType, int> DevCards { get; } = new Dictionary<DevCardType, int>();
        public Dictionary<ResourceType, int> ResourceCards { get; } = new Dictionary<ResourceType, int>();

        public UserResources()
        {
            foreach (ResourceType key in Enum.GetValues(typeof(ResourceType)))
            {
                ResourceCards[key] = 0;
            }

            foreach (DevCardType key in Enum.GetValues(typeof(DevCardType)))
            {
                DevCards[key] = 0;
            }
        }
    }
}
