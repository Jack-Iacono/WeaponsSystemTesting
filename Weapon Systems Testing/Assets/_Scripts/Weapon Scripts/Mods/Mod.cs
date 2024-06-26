using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Mod/NormalMod", order = 1)]
public class Mod : ScriptableObject
{
    public string modName;
    public int ID;
    [TextArea]
    public string tooltip;

    public int equipCost;

    [SerializeField]
    public FrameStats frameStats;
}
