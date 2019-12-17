using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoad : SceneState
{
    public SceneLoad(SceneContext theContext) : base(theContext)
    { }

    public override void Handle(int Value)
    {
        //进行数据加载

        //可根据Value值来进行状态直接的直接切换
        //if (Value > 10)
        //    m_Context.SetState(new ConcreteStateB(m_Context));
    }
}
