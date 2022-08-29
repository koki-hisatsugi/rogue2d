using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : ActorManager
{

    [SerializeField] private int _Stamina;
    [SerializeField] private Slider _STbar;
    [SerializeField] private Text _PlayerLevelText;
    [SerializeField] private AudioClip _STHeal;
    [SerializeField] private AudioClip _LevelUp;
    public int GetStamina
    {
        get { return _Stamina; }
    }
    
    [SerializeField] private PlayerStatusScrObj _PlayerStatus;

    protected void Start() {
        base.Start();
        // ActorManagerがアタッチされているオブジェクトがPlayerなら、
        // HPバーのスライダーを取得する
        _Level = 1;
        _Name = _PlayerStatus.PlayerName;
        _HPbar = GameObject.Find("HPbar").GetComponent<Slider>();
        _HPbar.maxValue = _PlayerStatus.list[0].maxHp;
        _HP = _PlayerStatus.list[0].maxHp;
        _HPbar.value = _HP;
        // スタミナのスライダーを取得する
        _STbar = GameObject.Find("STbar").GetComponent<Slider>();
        _STbar.maxValue = _PlayerStatus.list[0].maxStamina;
        _Stamina = _PlayerStatus.list[0].maxStamina;
        _STbar.value = _Stamina;
        _Atk = _PlayerStatus.list[0].atk;
        _Def = _PlayerStatus.list[0].def;
        _PlayerLevelText = GameObject.Find("PlayerLevelText").GetComponent<Text>();
        _PlayerLevelText.text = "LV:"+_Level;
    }

    public void StaminaCost(){
        _Stamina--;
        if(_STbar != null){
            _STbar.value = _Stamina;
        }
    }

    public void HpHelth(){
        if(_HP < _PlayerStatus.list[_Level-1].maxHp){
            _HP++;
        }
        _HPbar.value = _HP;
    }

    public void HpHelth(int helth){
        if(_HP < _PlayerStatus.list[_Level-1].maxHp){
            _HP = _HP + helth;
        }
        _AudioSource.PlayOneShot(_HPHeal);
        _HPbar.value = _HP;
    }
    
    public void HpLost(){
        if(_HP > 0){
            _HP--;
        }
        _HPbar.value = _HP;
    }

    public void StaminaCharge(int charge){
        _Stamina += charge;
        if(_Stamina > _PlayerStatus.list[_Level-1].maxStamina){
            _Stamina = _PlayerStatus.list[_Level-1].maxStamina;
        }
        if(_STbar != null){
            _STbar.value = _Stamina;
        } 
        _AudioSource.PlayOneShot(_STHeal);
    }

    public bool AddExp(int Exp){
        bool result = false;
        _Exp = _Exp + Exp;
        if(_Exp >= _PlayerStatus.list[_Level-1].nextExp){
            _Exp = _Exp - _PlayerStatus.list[_Level-1].nextExp;
            _Level++;
            _Atk = _PlayerStatus.list[_Level-1].atk;
            _Def = _PlayerStatus.list[_Level-1].def;
            _HPbar.maxValue = _PlayerStatus.list[_Level-1].maxHp;
            _STbar.maxValue = _PlayerStatus.list[_Level-1].maxStamina;
            opl.OutputLog(_Name + "は" + _Level + "レベルになった");
            _PlayerLevelText.text = "LV:"+_Level;
            _AudioSource.PlayOneShot(_LevelUp);
            result = true;
        }

        return result;
    }
}
