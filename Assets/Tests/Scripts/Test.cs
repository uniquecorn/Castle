using System.Collections;
using System.Collections.Generic;
using Castle.Core;
using Castle.Core.Save;
using UnityEngine;

public class Test : MonoBehaviour
{

    public void Authenticate()
    {
        CastleKit.AuthenticateLocalPlayer(TestCallback,TestCallback,TestCallback);
    }

    public void TestCallback(CastleResult result)
    {
        
    }
}
