using UnityEngine;
using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Example : MonoBehaviour 
{
    public string poolName;
    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            EasyObjectPool.instance.GetObjectFromPool(poolName, Vector3.zero, Quaternion.identity);
        }
    }
}
