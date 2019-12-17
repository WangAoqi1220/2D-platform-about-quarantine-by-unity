using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//封装SceneContext处于特定状态下的行为
public abstract class SceneState 
{
    protected SceneContext m_Context = null;

    public SceneState(SceneContext theContext)
    {
        m_Context = theContext;
    }
    public abstract void Handle(int Value);
}
