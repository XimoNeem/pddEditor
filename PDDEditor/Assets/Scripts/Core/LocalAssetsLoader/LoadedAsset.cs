using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedAsset : MonoBehaviour
{
    [HideInInspector] public LocalAssetProvider AssetContainer;

    public virtual void Initialize(params object[] values)
    {
        
    }
}
