using Unity.Netcode.Components;
using UnityEngine;

public class ClientTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()   {   return false;    }
}
