using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : ActorManager
{
    [SerializeField] protected EnemyStatusScrObj _EnemyStatus;

    protected void Start() {
        base.Start();
        // _EnemyStatus = Resources.Load<EnemyStatusScrObj>("EnemyStatusDataTable");
        _HPbar = transform.Find("HPberCanvas").gameObject.transform.Find("HPbar").gameObject.GetComponent<Slider>();
        if(_Level-1 < _EnemyStatus.list.Count){
            _HPbar.maxValue = _EnemyStatus.list[_Level-1].maxHp;
            _HP = _EnemyStatus.list[_Level-1].maxHp;
            _Atk = _EnemyStatus.list[_Level-1].atk;
            _Def = _EnemyStatus.list[_Level-1].def;
        }else{
            _HPbar.maxValue = _EnemyStatus.list[_EnemyStatus.list.Count-1].maxHp;
            _HP = _EnemyStatus.list[_EnemyStatus.list.Count-1].maxHp;
            _Atk = _EnemyStatus.list[_EnemyStatus.list.Count-1].atk;
            _Def = _EnemyStatus.list[_EnemyStatus.list.Count-1].def;
        }
        _HPbar.value = _HP;
    }

    public void HpHelth(int helth){
        if(_HP < _EnemyStatus.list[_Level-1].maxHp){
            _HP = _HP + helth;
        }
        _AudioSource.PlayOneShot(_HPHeal);
        _HPbar.value = _HP;
    }
    
    public void HpHelth(){
        if(_HP < _EnemyStatus.list[_Level-1].maxHp){
            _HP++;
        }
        _HPbar.value = _HP;
    }
    
    public void setEnemyStatus(){
        if(_EnemyStatus == null) _EnemyStatus = Resources.Load<EnemyStatusScrObj>("EnemyStatusDataTable");
        _Name = _EnemyStatus.EnemyName;
        if(_HPbar == null) _HPbar = transform.Find("HPberCanvas").gameObject.transform.Find("HPbar").gameObject.GetComponent<Slider>();
        Debug.Log(_Level);
        if(_Level-1 >= 0 && _Level-1 < _EnemyStatus.list.Count){
            _HPbar.maxValue = _EnemyStatus.list[_Level-1].maxHp;
            _HP = _EnemyStatus.list[_Level-1].maxHp;
            _Atk = _EnemyStatus.list[_Level-1].atk;
            _Def = _EnemyStatus.list[_Level-1].def;
            _Exp = _EnemyStatus.list[_Level-1].giveExp;
        }else{
            _HPbar.maxValue = _EnemyStatus.list[_EnemyStatus.list.Count-1].maxHp;
            _HP = _EnemyStatus.list[_EnemyStatus.list.Count-1].maxHp;
            _Atk = _EnemyStatus.list[_EnemyStatus.list.Count-1].atk;
            _Def = _EnemyStatus.list[_EnemyStatus.list.Count-1].def;
            _Exp = _EnemyStatus.list[_EnemyStatus.list.Count-1].giveExp;
        }
        _HPbar.value = _HP;
    }
}
