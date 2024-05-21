using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftScreenController : ScreenController
{
    public TMP_Dropdown frameDropDown;

    private string chosenFrame;

    // Start is called before the first frame update
    void Start()
    {
        frameDropDown.ClearOptions();

        UpdateFrames();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
    }

    private void UpdateFrames()
    {
        List<string> frameNames = new List<string>();

        foreach (Frame frame in InventoryController.instance.frameList)
        {
            frameNames.Add(frame.name);
        }

        frameDropDown.AddOptions(frameNames);
    }

    public void ChooseWeapon()
    {
        InventoryController.instance.ChangeFrame(frameDropDown.value);
    }

    public void ShowWeaponPreview()
    {
        Debug.Log("Show");
    }
    public void HideWeaponPreview()
    {
        Debug.Log("Hide");
    }
}
