using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create PlayerData")]
public class PlayerStatusScrObj : ScriptableObject
{
	public string name;
    
	public List<PlayerParam> list = new List<PlayerParam>();

	[System.Serializable]
	public class PlayerParam
	{
		public int level;
		public int maxHp;
		public int maxStamina;
        public int nextExp;
		public int atk;
		public int def;
	}

	private void OnValidate(){
		for (int i = 0; i < list.Count; i++){
            int level = i+1;
			list[i].level = level;
			list[i].maxHp = level*20+50;
            list[i].maxStamina = list[i].maxHp;
			list[i].nextExp = level*15;
			list[i].atk = level*10;
			list[i].def = level*2;
		}
	}
}
