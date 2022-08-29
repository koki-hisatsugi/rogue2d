using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create PlayerData")]
public class PlayerStatusScrObj : ScriptableObject
{
	public string PlayerName;
    
	[Header("ステータスを自動で補完したいときにチェック")]public bool AutoSettingParam;
	[Header("乗算したい攻撃力")]public int AddAtk;
	[Header("加算したい防御力")]public int AddDef;
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
		if(AutoSettingParam){
			for (int i = 0; i < list.Count; i++){
				int level = i+1;
				list[i].level = level;
				list[i].maxHp = level*20+50;
				list[i].maxStamina = list[i].maxHp;
				list[i].nextExp = level*15;
				list[i].atk = level*AddAtk;
				list[i].def = level+AddDef;
			}
		}
	}
}
