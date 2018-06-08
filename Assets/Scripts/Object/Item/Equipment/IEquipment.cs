using UnityEngine;
using System.Collections.Generic;

public interface IEquipment
{
    PartsType PartsType { get; }
}

public enum PartsType
{
    BODY,
    ARM,
    LEG,
    HEAD,
    WEAPON,
    MAX
}