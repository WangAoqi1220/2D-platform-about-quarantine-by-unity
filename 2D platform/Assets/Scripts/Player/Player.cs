using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(PlayerMove))]
	public class Player : MonoBehaviour
	{
		#region 子组件
		public PlayerMove Move { get; private set; }
		#endregion

		#region MonoBehaviour生命周期
		private void Awake()
		{
			//初始化对象组件
			this.Move = gameObject.GetComponent<PlayerMove>();
		
			
		}

		private void Start()
		{
			gameObject.tag = "Player";
		}

		#endregion

	}
}


