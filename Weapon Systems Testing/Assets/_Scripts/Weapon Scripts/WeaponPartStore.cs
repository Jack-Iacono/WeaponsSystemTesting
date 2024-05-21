using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponPartStore : MonoBehaviour
{
    public static WeaponPartStore instance;

    public List<Frame> frameList = new List<Frame>();
    public List<Mod> modList = new List<Mod>();

    public Dictionary<int, Frame> frames = new Dictionary<int, Frame>();
    public Dictionary<int, Mod> mods = new Dictionary<int, Mod>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        frames.Clear();
        mods.Clear();

        foreach(Frame frame in frameList)
        {
            if(!frames.ContainsKey(frame.ID))
                frames.Add(frame.ID, Instantiate(frame));
        }
        foreach (Mod mod in modList)
        {
            if (!mods.ContainsKey(mod.ID))
                mods.Add(mod.ID, mod);
        }
    }
}
