using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HUDScreenController : ScreenController
{
    public TMP_Text ammoText;

    private void Awake()
    {
        Frame.OnFrameDataChange += OnFrameDataChange;
        Weapon.OnFrameChange += OnFrameChange;
    }

    public override void Initialize(UIController parent)
    {
        base.Initialize(parent);
    }

    private void OnFrameChange(Frame frame, List<Mod> mods)
    {
        ammoText.text = frame.currentAmmo + "/" + frame.currentStats.readyAmmo;
    }
    private void OnFrameDataChange(Frame frame)
    {
        ammoText.text = frame.currentAmmo + "/" + frame.currentStats.readyAmmo;
    }
}
