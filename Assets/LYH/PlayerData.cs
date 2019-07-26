using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {

    public bool dead { get; protected set; } // 사망 상태
    public float speed { get; protected set; } // 속도
}
