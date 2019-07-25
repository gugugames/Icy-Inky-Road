// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountdownTimer.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
// This is a basic CountdownTimer. In order to start the timer, the MasterClient can add a certain entry to the Custom Room Properties,
// which contains the property's name 'StartTime' and the actual start time describing the moment, the timer has been started.
// To have a synchronized timer, the best practice is to use PhotonNetwork.Time.
// In order to subscribe to the CountdownTimerHasExpired event you can call CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
// from Unity's OnEnable function for example. For unsubscribing simply call CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;.
// You can do this from Unity's OnDisable function for example.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using ClientLibrary;

namespace Photon.Pun.UtilityScripts
{
    public class CountdownTimer : MonoBehaviourPunCallbacks
    {
        // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
        public static CountdownTimer instance
        {
            get
            {
                // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
                if (m_instance == null)
                {
                    // 씬에서 GameManager 오브젝트를 찾아 할당
                    m_instance = FindObjectOfType<CountdownTimer>();
                }

                // 싱글톤 오브젝트를 반환
                return m_instance;
            }
        }

        private static CountdownTimer m_instance; // 싱글톤이 할당될 static 변수
        private float startTime;

        public bool playerA = false;
        public bool playerB = false;
        public GameObject winPanel;
        public GameObject losePanel;
        public GameObject drawPanel;
        public Text AScore;
        public Text BScore;

        bool flag = false;

        PlayerCtrl[] player;
        ClientLibrary.Grid grid;

        [Header("Reference to a Text component for visualizing the countdown")]
        public Text Text;

        [Header("Countdown time in seconds")]
        public float Countdown = 60.0f;

        float countdown = 60;

        public void Start()
        {
            if (Text == null)
            {
                Debug.LogError("Reference to 'Text' is not set. Please set a valid reference.", this);
                return;
            }
            startTime = (float)PhotonNetwork.Time;

            grid = FindObjectOfType<ClientLibrary.Grid>();
        }

        public void Update()
        {
            if (countdown > 0)
            {
                AScore.text = PhotonNetwork.MasterClient.CustomProperties["ScoreA"].ToString();
                BScore.text = PhotonNetwork.MasterClient.CustomProperties["ScoreB"].ToString();

                float timer = (float)PhotonNetwork.Time - startTime;
                countdown = Countdown - timer;

                Text.text = string.Format(countdown.ToString("n0"));
            }
            else
            {
                if (Input.anyKeyDown)
                {
                    PhotonNetwork.LeaveRoom();
                    PhotonNetwork.LoadLevel(0);
                }
            }

            // 게임 종료
            if (countdown < 0 && !flag)
            {
                flag = true;
                // 모두 멈추게하고 승리 UI 띄우고
                player = FindObjectsOfType<PlayerCtrl>();

                for (int i = 0; i < player.Length; i++)
                {
                    //player[i].StopMove();
                    player[i].enabled = false;
                }
                grid.enabled = false;

                int teamA, teamB;
                teamA = Convert.ToInt32(PhotonNetwork.MasterClient.CustomProperties["ScoreA"].ToString());
                teamB = Convert.ToInt32(PhotonNetwork.MasterClient.CustomProperties["ScoreB"].ToString());

                if (playerA)
                {
                    if (teamA > teamB)
                        winPanel.SetActive(true);
                    else if(teamA == teamB)
                        drawPanel.SetActive(true);
                    else
                        losePanel.SetActive(true);
                }

                if (playerB)
                {
                    if (teamA < teamB)
                        winPanel.SetActive(true);
                    else if (teamA == teamB)
                        drawPanel.SetActive(true);
                    else
                        losePanel.SetActive(true);
                }
            }
        }
    }
}