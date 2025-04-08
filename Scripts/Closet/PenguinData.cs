using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Penguin Data", menuName = "Scriptable Object/Penguin Data", order = 0)]
public class PenguinData : ScriptableObject
{
    [Header("Name")]
    [SerializeField] private int penguinId;
    [SerializeField] private string penguinName;

    /// <summary>
    /// 특징과 별 갯수
    /// </summary>
    [Header("Individuality")]
    [SerializeField] private string individuality_1_name;
    [Range(0, 5)][SerializeField] private int individuality_1_value;
    [Space(10f)]
    [SerializeField] private string individuality_2_name;
    [Range(0, 5)][SerializeField] private int individuality_2_value;

    /// <summary>
    /// 스킬 별 아이콘, 이름, 설명
    /// </summary>
    [Header("Ability")]
    [SerializeField] private Sprite passiveAbility_sprite;
    [SerializeField] private string passiveAbility_name;
    [SerializeField] private string passiveAbility;
    [Space(10f)]
    [SerializeField] private Sprite feverAbility_sprite;
    [SerializeField] private string feverAbility_name;
    [SerializeField] private string feverAbility;
    [Space(10f)]
    [SerializeField] private Sprite battleAbility_sprite;
    [SerializeField] private string battleAbility_name;
    [SerializeField] private string battleAbility;

    [Header("Prefab")]
    [SerializeField] private GameObject penguinPrefab;

    #region 프로퍼티
    public int PenguinId => penguinId;
    public string PenguinName => penguinName;

    public string Individuality_1_name => individuality_1_name;
    public int Individuality_1_value => individuality_1_value;

    public string Individuality_2_name => individuality_2_name;
    public int Individuality_2_value => individuality_2_value;

    public Sprite PassiveAbility_sprite => passiveAbility_sprite;
    public string PassiveAbility_name => passiveAbility_name;
    public string PassiveAbility => passiveAbility;

    public Sprite FeverAbility_sprite => feverAbility_sprite;
    public string FeverAbility_name => feverAbility_name;
    public string FeverAbility => feverAbility;

    public Sprite BattleAbility_sprite => battleAbility_sprite;
    public string BattleAbility_name => battleAbility_name;
    public string BattleAbility => battleAbility;

    public GameObject PenguinPrefab => penguinPrefab;
    #endregion 
}