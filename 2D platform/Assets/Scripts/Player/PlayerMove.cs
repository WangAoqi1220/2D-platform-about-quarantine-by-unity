using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class PlayerMove : MonoBehaviour
	{
		private Player player;	//player脚本的索引

		#region 基于射线检测的碰撞模拟系统

		/// <summary>
		/// 射线的位置信息
		/// 下方发射射线的起点位-bottomStart
		/// 判断是否可攀爬的高度位：-bottomStart + shinHeight
		/// width 决定了两侧射线（包括两侧的向下发射的射线和两侧的水平方向射线）发射的水平坐标
		/// </summary>
		[System.Serializable]
		private struct RayInfo
		{
			[Range(0.1f, 10f)]
			public float bottomStart;	//下方射线的发射起点
			[Range(0.1f, 10f)]
			public float width;			//玩家的逻辑宽度
			[Range(0.1f, 10f)]
			public float rayLength;		//射线的长度
			[Range(0.1f, 20f)]
			public float shinHeight;	//可攀爬障碍物的最大高度
		};
		[SerializeField]
		private RayInfo rayInfo = new RayInfo
		{
			bottomStart = 0.5f,
			width = 0.5f,
			rayLength = 0.2f,
			shinHeight = 0.5f
		};

		/// <summary>
		///  射线种类索引表
		///  记录了模拟碰撞检测所用的所有射线
		/// </summary>
		private enum RayIndexTable
		{
			bottom,bottomLeft,bottomRight,left,upLeft,right,upRight,
			TABLE_LENGTH
		}

		/// <summary>
		/// 碰撞检测结果信息
		/// </summary>
		RaycastHit2D[][] hits = new RaycastHit2D[(int)RayIndexTable.TABLE_LENGTH][];
		 	

		/// <summary>
		/// 根据射线类型获取射线
		/// </summary>
		/// <param name="index"></param> 需要的射线类型
		/// <returns></returns> 获取的射线
		private Ray2D GetRay(RayIndexTable index)
		{
			Ray2D ray = new Ray2D();
			Vector2 origin = Vector2.zero;
			Vector2 direction = Vector2.zero;
			switch (index)
			{
				case RayIndexTable.bottom:
					origin = new Vector2(0, -rayInfo.bottomStart);
					direction = Vector2.down;
					break;
				case RayIndexTable.bottomLeft:
					origin = new Vector2(-rayInfo.width, -rayInfo.bottomStart);
					direction = Vector2.down;
					break;
				case RayIndexTable.bottomRight:
					origin = new Vector2(rayInfo.width, -rayInfo.bottomStart);
					direction = Vector2.down;
					break;
				case RayIndexTable.left:
					origin = new Vector2(-rayInfo.width, -rayInfo.bottomStart);
					direction = Vector2.left;
					break;
				case RayIndexTable.upLeft:
					origin = new Vector2(-rayInfo.width, -rayInfo.bottomStart + rayInfo.shinHeight);
					direction = Vector2.left;
					break;
				case RayIndexTable.right:
					origin = new Vector2(rayInfo.width, -rayInfo.bottomStart);
					direction = Vector2.right;
					break;
				case RayIndexTable.upRight:
					origin = new Vector2(rayInfo.width, -rayInfo.bottomStart + rayInfo.shinHeight);
					direction = Vector2.right;
					break;
				case RayIndexTable.TABLE_LENGTH:
					Debug.LogError("不允许传入TABLE_LENGTH");
					break;
			}

			origin.x += transform.position.x;
			origin.y += transform.position.y;
			ray.origin = origin;
			ray.direction = direction;
			return ray;
		}
		
		private Ray2D GetRay(int index)
		{
			return this.GetRay((RayIndexTable)index);
		}

		/// <summary>
		/// 每帧调用，执行一次碰撞检测
		/// </summary>
		private void CastRays()
		{
			for(int i = 0; i < (int)RayIndexTable.TABLE_LENGTH; i++)
			{
				Ray2D ray = this.GetRay(i);
				hits[i] = Physics2D.RaycastAll(ray.origin, ray.direction, rayInfo.rayLength);
			}
		}

		#endregion

		#region MonoBehaviour生命周期
		private void Awake()
		{
			this.hits = new RaycastHit2D[(int)RayIndexTable.TABLE_LENGTH][];

			this.player = gameObject.GetComponent<Player>();
		}

		private void Start()
		{
			
		}

		private void Update()
		{
			this.MoveUpdate();
		}

		private void FixedUpdate()
		{
			this.CastRays();
		}

		#endregion

		#region 玩家移动相关逻辑

		/*
		 * 水平以右为正方向
		 * 垂直以上为正方向
		 */

		private const float gravity = 9.8f;

		private float hVelocity;		//水平速度
		private float vVelocity;        //垂直速度
		private bool isRight;           //是否朝向右方
		private bool isGround;          //是否脚踏实地

		[SerializeField]
		[Header("水平方向最大速度")]
		private float hMaxVelocity = 1;

		private void MoveUpdate()
		{
			//判断玩家是否在地面上
			this.isGround = false;
			foreach (var hit in hits[(int)RayIndexTable.bottom])
			{
				if (hit.transform.CompareTag("Entity"))
				{
					this.isGround = true;
					//更新y坐标
					Vector3 _pos = transform.position;
					_pos.y = hit.point.y + rayInfo.bottomStart + rayInfo.rayLength;
					transform.position = _pos;
				}
			}

			if (this.isGround == true)
			{
				//将垂直速度置为0
				this.vVelocity = 0;
				//检查平移输入
				float hInput = Input.GetAxis("Horizontal");
				this.hVelocity = hInput * this.hMaxVelocity;
				if(this.hVelocity > 0)
				{
					this.isRight = true;
				}
				if(this.hVelocity < 0)
				{
					this.isRight = false;
				}

			}
			else
			{
				this.hVelocity = 0;
				//应用重力
				this.vVelocity -= gravity * Time.deltaTime;
			}

			//根据速度移动人物
			transform.Translate(Vector3.right * hVelocity * Time.deltaTime);
			transform.Translate(Vector3.up * vVelocity * Time.deltaTime);
			
		}

		#endregion

		#region Gizmos相关

		private void OnDrawGizmos()
		{
			this.DrawRayGizmos();
		}

		private void DrawRayGizmos()
		{
			Gizmos.color = Color.red;
			//遍历射线索引表
			for (int i = 0; i < (int)RayIndexTable.TABLE_LENGTH; i++)
			{
				Ray2D ray = this.GetRay((RayIndexTable)i);
				Vector3 from = (Vector3)ray.origin;
				Vector3 to = from + (Vector3)ray.direction * rayInfo.rayLength;
				Gizmos.DrawLine(from, to);
			}
		}

		#endregion

	}

}
