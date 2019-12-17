using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState :SceneState
{
    public GameState(SceneContext theContext) : base(theContext)
    { }

    public override void Handle(int Value)
    {
        //游戏进行时的状态判定

        //可根据Value值来进行状态直接的直接切换
        //if (Value > 10)
        //    m_Context.SetState(new ConcreteStateB(m_Context));
    }
}
