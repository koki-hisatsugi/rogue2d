using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create EnemyData")]
public class EnemyStatusScrObj : ScriptableObject
{
	public string name;
	public List<EnemyParam> list = new List<EnemyParam>();

	[System.Serializable]
	public class EnemyParam
	{
		public int id;
		public int level;
		public int maxHp;
		public int atk;
		public int def;
		public int giveExp;
	}

	private void OnValidate(){
		for (int i = 0; i < list.Count; i++){
			list[i].id = i+1;
			list[i].level = i+1;
			list[i].maxHp = list[i].level*3+10;
			list[i].atk = list[i].level*4 - (list[i].level*2);
			if(list[i].atk < 2){
				list[i].atk = 2;
			}
			list[i].def = list[i].level*4 - (list[i].level*3);
			if(list[i].def < 2){
				list[i].def = 2;
			}
			list[i].giveExp = list[i].level*2;
		}
	}
}
