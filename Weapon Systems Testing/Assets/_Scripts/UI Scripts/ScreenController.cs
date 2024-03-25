using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    protected Animator anim;
    [Tooltip("The time (in seconds) that the show animation takes")]
    public float showAnimTime;
    [Tooltip("The time (in seconds) that the hide animation takes")]
    public float hideAnimTime;
    protected UIController parentController;

    public virtual void Initialize(UIController parent) 
    { 
        parentController = parent;
        anim = GetComponent<Animator>();
    }

    public virtual void ShowScreen()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(ShowScreenCoroutine());
    }
    public virtual void HideScreen()
    {
        StopAllCoroutines();
        if(isActiveAndEnabled)
            StartCoroutine(HideScreenCoroutine());
    }

    protected IEnumerator ShowScreenCoroutine()
    {
        anim?.SetTrigger("Show");

        yield return new WaitForSeconds(showAnimTime);

        parentController.ScreenHideCallback();
        Debug.Log("Show Screen Finish" + name);
    }
    protected IEnumerator HideScreenCoroutine()
    {
        anim?.SetTrigger("Hide");

        yield return new WaitForSeconds(hideAnimTime);

        gameObject.SetActive(false);

        parentController.ScreenShowCallback();
    }
}
