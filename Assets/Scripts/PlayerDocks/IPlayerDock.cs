using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public abstract class IPlayerDock : MonoBehaviour
{
    public abstract void Refresh(Player player);
    
}
