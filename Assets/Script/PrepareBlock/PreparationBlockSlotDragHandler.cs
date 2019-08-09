﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PreparationBlockSlotDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler {

    //slot 오브젝트가 벗어 날 수 있는 범위
    public float m_range;

    // Slot에 저장돼 있는 오브젝트
    public GameObject m_SlotObject;

    // 현재 드래그 중인 Plane
    protected RectTransform m_CurrentDraggingPlane;

    // 드래그 시작시 원래 위치 저장
    private Vector3 initPosition;

    private bool m_DragHasBegan = false;


    /// <summary>
    /// 오브젝트가 눌렸을때 실행되는 메서드
    /// 초기 데이터 저장 용도
    /// 
    /// 드래그를 시작하면 
    /// 1. 해당 오브젝트가 어떤건지 가져옴
    /// 2. 오브젝트의 위치값을 마우스 포지션과 일치시킴
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Canvas canvas = FindObjectOfType<Canvas>();//UIUtility.FindInParents<Canvas>(this.gameObject);

        if (canvas == null)
            return;

        // Start the drag
        m_DragHasBegan = true;

        // Save the dragging plane
        m_CurrentDraggingPlane = canvas.transform as RectTransform;

        // 초기 위치 저장
        initPosition = transform.position;

        // Update the icon position
        UpdateDraggedPosition(eventData);

        eventData.Use();

    }

    /// <summary>
    /// 오브젝트 드래그하는 동안 실행되는 메서드
    /// 드래그해서 일정 범위 이상 벗어나면 이미지는 돌아오고 템플릿 오브젝트가 생성된다.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // Check if the dragging has been started
        if (m_DragHasBegan)
        {
            // Update the dragged object's position
            if (transform != null && !CheckOutOfRanage(m_range))                
                this.UpdateDraggedPosition(eventData);
            else
            {
                transform.position = initPosition;
                m_DragHasBegan = false;
            }
        }

    }

    /// <summary>
    /// 오브젝트 드래그를 해제 했을 때 실행되는 메서드
    /// 일정 범위 안에서 드래그를 해제 : 오브젝트 생성 x 이미지만 돌아온다.
    /// 오브젝트 생성중 드래그 해제 : 오브젝트 제어 권한 삭제
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {

        // Reset the drag begin bool
        m_DragHasBegan = false;

        // 초기 위치로 이동
        transform.position = initPosition;

    }

    /// <summary>
    /// 드래그시 오브젝트의 position과 해당 plane 에 맞게 rotation되는 메서드
    /// </summary>
    /// <param name="data"></param>
    private void UpdateDraggedPosition(PointerEventData data)
    {
        var rt = this.transform.GetComponent<RectTransform>();
        Vector3 globalMousePos;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_CurrentDraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = this.m_CurrentDraggingPlane.rotation;
        }
    }

    /// <summary>
    /// 제한 범위를 체크하여 나갔을때 true 값 전달
    /// </summary>
    /// <param name="range">제한 범위</param>
    private bool CheckOutOfRanage(float range)
    {
        // 좌우변 체크
        if (transform.position.x < initPosition.x - range || transform.position.x > initPosition.x + range)
        {
            return true;
        }

        // 상하변 체크
        if (transform.position.y < initPosition.y - range || transform.position.y > initPosition.y + range)
            return true;

        return false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
}