using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{

    public static void Delay(Action callback)
    {
        GameManager.Instance.StartCoroutine(Waiter());
        IEnumerator Waiter()
        {
            yield return null;
            callback.Invoke();
        }
    }
}
