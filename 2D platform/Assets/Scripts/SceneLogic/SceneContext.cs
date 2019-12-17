using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//保存当前的场景状态，并传递信息
public class SceneContext 
{
    SceneState m_State = null;

    //对于场景进行操作
    public void Request(int Value)
    {
        m_State.Handle(Value);
    }

    public void SetState(SceneState theState)
    {
        Debug.Log("Context.SetState:" + theState);
        m_State = theState;
    }
}
