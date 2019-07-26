using Photon.Pun;
using UnityEngine;

public class ExItem : MonoBehaviourPun, IItem
{
    public void Use(GameObject target)
    {
        // 각 아이탬 별 효과 추가
        // 예를들어 속도를 증가시키는 아이탬이라면 target의 속도를 증가시킴
    }
}
