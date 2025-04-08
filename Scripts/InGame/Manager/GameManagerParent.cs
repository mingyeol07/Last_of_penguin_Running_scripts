using Lop.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최상위 게임매니저
/// </summary>
public abstract class GameManagerParent : MonoBehaviour
{
    protected static GameManagerParent instance;
    public static GameManagerParent Instance { get { return instance; } }

    protected virtual void Awake() {
        instance = this;
    }

    /// <summary>
    /// 점수 아이템을 먹었을 때
    /// </summary>
    /// <param name="type">먹은 점수 아이템의 타입</param>
    /// <param name="customValue">원하는 점수를 얻고 싶을 때</param>
    /// <param name="playerNumber">멀티게임에서의 플레이어 식별 넘버</param>
    public abstract void GetScore(ScoreType type, int customValue = 0, int playerNumber = 0);

    /// <summary>
    /// 체력(시간) 중 쉴드 생성
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="playerNumber"></param>
    public abstract void GetShield(float percent, int playerNumber = 0);

    /// <summary>
    /// 시계 아이템을 먹었을 때
    /// </summary>
    /// <param name="percent"></param>
    /// <param name="playerNumber">멀티게임에서의 플레이어 식별 넘버</param>
    public abstract void GetWatch(float percent = 999, int playerNumber = 0);

    /// <summary>
    /// 피버스타를 먹었을 때
    /// </summary>
    /// <param name="playerNumber">멀티게임에서의 플레이어 식별 넘버</param>
    public abstract void GetFeverStar(int playerNumber = 0);

    /// <summary>
    /// 플레이어가 데미지를 받았을 때
    /// </summary>
    /// <param name="playerNumber">멀티게임에서의 플레이어 식별 넘버</param>
    public abstract void PlayHit(int playerNumber = 0);

    /// <summary>
    /// 플레이어가 떨어졌을 때
    /// </summary>
    /// <param name="playerNumber">멀티게임에서의 플레이어 식별 넘버</param>
    public abstract void PlayFall(int playerNumber = 0);


    public abstract IEnumerator Co_HitEffect(int playerNumber = 0);
    public abstract void Gameover(int playerNumber = 0);
    public abstract void GameClear(int playerNumber = 0); // 깃발에 닿앗을 때

    /// <summary>
    /// 황제가 죽었을 때 호출하는 부활스킬 함수
    /// </summary>
    /// <param name="playerNumber"></param>
    public abstract void EmperorRevive(int playerNumber = 0);
    public abstract void RockhopperAddFeverTime(float value, int playerNumber);

    /// <summary>
    /// 멀티플레이에서 식별 넘버에 해당하는 펭귄 오브젝트를 반환
    /// </summary>
    /// <param name="playerNumber"></param>
    /// <returns></returns>
    public abstract GameObject GetMyPenguin(int playerNumber = 0);

    //멀티 에서 떨어지는거 구연할때 abstract 로 바꾸세요
    public virtual void SetBackgroundMove(bool value) { }
}
