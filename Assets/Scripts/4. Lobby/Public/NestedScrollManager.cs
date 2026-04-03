using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NestedScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private const int SIZE = 3; // Panel 개수 > Content 자식 개수

    [SerializeField]
    private Scrollbar scrollbar; // 스크롤 바
    [SerializeField]
    private Transform contentTrans; // Content의 Transform
    [SerializeField]
    private Slider tabSlider; // Tab Slider
    [SerializeField]
    private RectTransform[] buttonRectArray; // 탭 버튼 RectTrans 배열
    [SerializeField]
    private RectTransform[] buttonImageRectArray; // 버튼 이미지 RectTrans 배열

    private float[] pos = new float[SIZE]; // Panel의 위치값 > ScrollBar의 Value값 저장
    private float distance; // Panel들 간의 간격
    [ReadOnly(true), SerializeField]
    private float targetPos; // pos배열에서 현재 Panel이 향해야 하는 pos값
    [ReadOnly(true), SerializeField]
    private float currentPos; // pos배열에서 현재 Panel이 있는 pos값

    private float buttonBigSize;
    private float buttonSmallSize;

    [ReadOnly(true), SerializeField]
    private ReactiveProperty<int> TargetIndex; // pos배열에서 현재 Panel의 index값

    [SerializeField]
    private float scrollSpeed;
    private bool isDragging = false;
    void Start()
    {
        // distance 초기화
        // Panel들간 간격이며, 1 / (Panel개수 - 1)이다.
        distance = 1f / (SIZE - 1);

        // Panel의 위치값 초기화
        // 0     0.5      1 > Panel 개수 3개
        // lㅡㅡㅡㅣㅡㅡㅡㅣ
        for (int i = 0; i < SIZE; i++)
            pos[i] = distance * i;

        InitButtonSize();

        TargetIndex
            .DistinctUntilChanged()
            .Subscribe(value =>
            {
                EventManager.Instance.Publish(UIEventType.SwapMainUI);
            });
    }

    void Update()
    {
        // 탭 슬라이더의 값도 변경한다.
        tabSlider.value = scrollbar.value;

        // 드래그를 하지 않을 때
        if (!isDragging)
        {
            // scrollbar의 값을 targetPos로 Lerp하게 변경한다.
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetPos, scrollSpeed * Time.deltaTime);

            // 목표 버튼의 크기가 커진다.
            for(int i = 0; i < SIZE; i++)
                buttonRectArray[i].sizeDelta = new Vector2(i == TargetIndex.Value ? buttonBigSize : buttonSmallSize, buttonRectArray[i].sizeDelta.y);
        }

        // 최초 이미지가 모이는 게 보이는데, 이를 방지하기 위해 0.1초 대기한 다음, 진행한다.
        if (Time.time < 0.1f)
            return;

        // DOTween과, buttonImageRectArray의 Position3D값을 ReactiveProperty로 구독받게 하는 클래스를 만들고, 그걸 이용해야 하는데... 그럴바에 일단 이런식으로 Update에 for를 넣어주는게 더 좋을듯
        // 만약 성능에 문제가 생긴다면, 위 주석 내용을 시도해보자.
        for (int i = 0; i < SIZE; i++)
        {
            // i번째 버튼의 위치와 크기를 가져온다
            Vector3 buttonTargetPos = buttonRectArray[i].anchoredPosition3D;
            Vector3 buttonTargetScale = Vector3.one;
            bool textActive = false;

            // i가 TargetIndex.Value와 같다면, y를 -23으로, scale을 1.2로 키운다.
            if (i == TargetIndex.Value)
            {
                buttonTargetPos.y = -23f;
                buttonTargetScale = new Vector3(1.2f, 1.2f, 1f);
                textActive = true;
            }

            // i번째 버튼의 위치와 크기를 가져와, i번째 이미지의 위치와 크기에 적용시킨다.
            buttonImageRectArray[i].anchoredPosition3D = Vector3.Lerp(buttonImageRectArray[i].anchoredPosition3D, buttonTargetPos, 0.25f);
            buttonImageRectArray[i].localScale = Vector3.Lerp(buttonImageRectArray[i].localScale, buttonTargetScale, 0.25f);
            buttonImageRectArray[i].transform.GetChild(0).gameObject.SetActive(textActive);
        }

    }


    /// <summary>
    /// 버튼 크기 조절
    /// 버튼 개수에 따라 자동으로 조절된다.
    /// </summary>
    private void InitButtonSize()
    {
        float buttonDefaultSize = 1080f / buttonRectArray.Length;
        float buttonScaleRatio = buttonDefaultSize * 0.1f;
        buttonBigSize = buttonDefaultSize + buttonScaleRatio * (buttonRectArray.Length - 1);
        buttonSmallSize = buttonDefaultSize - buttonScaleRatio;

        for(int i = 0; i < buttonRectArray.Length; i++)
        {
            if(i == TargetIndex.Value)
            {
                buttonRectArray[i].sizeDelta = new Vector2(buttonBigSize, buttonRectArray[i].sizeDelta.y);
            }
            else
            {
                buttonRectArray[i].sizeDelta = new Vector2(buttonSmallSize, buttonRectArray[i].sizeDelta.y);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // eventData.position 이벤트 발생 위치
        // eventData.delta 드래그 했을 시 순간 변화율

        // 드래그 시작
        isDragging = true;

        currentPos = SetPos();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //isDragging = true;
    }

    /// <summary>
    /// 드래그 종료했을 시 호울
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        targetPos = SetPos();

        // 현재위치과 목표 위치가 같을 때
        if(currentPos == targetPos)
        {
            // 좌=>우로 빠르게 이동할 시
            if(eventData.delta.x > 18 && currentPos - distance >= 0)
            {
                // 좌측으로 데이터 조정
                TargetIndex.Value--;
                targetPos = currentPos - distance;
            }
            // 우=>좌로 빠르게 이동할 시
            else if(eventData.delta.x < -18 && currentPos + distance <= 1.01f)
            {
                // 우측으로 데이터 조정
                TargetIndex.Value++;
                targetPos = currentPos + distance;
            }
        }

        for(int i = 0; i < SIZE; i++)
        {
            // contentTrans의 자식이 ScrollScript를 지니고 있으며 현재 Panel이  자식이 아니고 목표가 자식이라면, 자식 스크롤바 초기화
            var scroll = contentTrans.GetChild(i).GetComponentInChildren<ScrollScript>();
            if (scroll != null && currentPos != pos[i] && targetPos == pos[i])
            {
                scroll.ResetScrollBar();
            }
        }

        // 드래그 종료
        isDragging = false;
    }

    /// <summary>
    /// Panel이 Scrollbar의 값에 따라 현재 있어야 하는 Pos를 리턴한다.
    /// </summary>
    /// <returns></returns>
    private float SetPos()
    {
        for (int i = 0; i < SIZE; i++)
        {
            // 절반거리를 기준으로 가까운 위치를 리턴
            if (scrollbar.value < pos[i] + distance * 0.5f && scrollbar.value > pos[i] - distance * 0.5f)
            {
                TargetIndex.Value = i;
                return pos[i];
            }
        }
        return 0f;
    }

    public void ClickTab(int num)
    {
        TargetIndex.Value = num;
        targetPos = pos[num];
    }
}
