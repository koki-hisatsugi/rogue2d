using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create EnemyData")]
public class ActorStatusScrObj : ScriptableObject
{
	public List<EnemyParam> list = new List<EnemyParam>();

	[System.Serializable]
	public class EnemyParam
	{
		public string name;
		public int num;
		public int maxHp;
		public int attPower;
	}

    public PlayerParam playerParams = new PlayerParam();

    [System.Serializable]
    public class PlayerParam
	{
		public string name;
		public int num;
		public int maxHp;
		public int attPower;
		public int maxStamina;
	}
}
