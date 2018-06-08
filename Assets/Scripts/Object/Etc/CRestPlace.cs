using UnityEngine;
using UnityEngine.UI;

public class CRestPlace : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        CUnit unit = other.GetComponent<CUnit>();

        if (unit == null || !(unit is CPlayerUnit))
            return;

        //TODO: 플레이어의 메세지 UI를 활성화 시키고 UI에 휴식이 가능하도록 전달
        //Button button;
        //button.onClick.RemoveAllListeners();
    }

    void OnTriggerExit(Collider other)
    {
        //TODO: 플레이어의 메세지 UI를 비활성화 시킨다.
    }

    public void RestPlayer(CPlayerUnit player)
    {
        //TODO: 플레이어의 체력을 회복 시키고 일정시간동안 못 움직이게 한다.
        if (player.Status.CurState == State.Rest)
            return;

        player.ResetUnit();
    }

}
