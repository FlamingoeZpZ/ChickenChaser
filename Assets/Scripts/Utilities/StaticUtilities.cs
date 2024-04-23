using UnityEngine;

public static class StaticUtilities
{
    public static readonly int WallLayer = 1 << LayerMask.NameToLayer("Default");
    public static readonly int HumanLayer = 1 << LayerMask.NameToLayer("Human");
    public static readonly int PlayerLayer = 1 << LayerMask.NameToLayer("Player");
    
    public static readonly int VisibilityLayer =  WallLayer | HumanLayer ;
    
}
