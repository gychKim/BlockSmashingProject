using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollScript : ScrollRect
{
    private bool forParent; // 부모를 위한 작업인지

    private NestedScrollManager nestedScrollManager; // public으로 해도 Insepctor에 안보임 > ScrollRect를 상속받은 스크립트라 안보이게 됨
    private ScrollRect parentScrollRect; // 부모 scrollRect
    protected override void Start()
    {
        // 부모가 지니고 있는 NestedScrollManager를 가져온다.
        nestedScrollManager = GetComponentInParent<NestedScrollManager>();

        // 부모가 지니고 있는 ScrollRect를 가져온다.
        parentScrollRect = nestedScrollManager.GetComponent<ScrollRect>();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        // 수평으로 이동하는 스크롤 뷰라면
        if(horizontal)
        {
            // 드래그 시작하는 순간 수직이 크면 부모가 드래그 시작한 것 / 수평 이동이 크면 자식이 드래그 시작한 것
            forParent = Mathf.Abs(eventData.delta.x) < Mathf.Abs(eventData.delta.y);

            // 부모를 즉, 수평 스크롤을 했다면, 부모의 로직을 실행
            if (forParent)
            {
                nestedScrollManager.OnBeginDrag(eventData);
                parentScrollRect.OnBeginDrag(eventData);
                return;
            }
        }
        // 수직이라면
        else
        {
            // 드래그 시작하는 순간 수평이동이 크면 부모가 드래그 시작한 것 / 수직 이동이 크면 자식이 드래그 시작한 것
            forParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

            // 부모를 즉, 수평 스크롤을 했다면, 부모의 로직을 실행
            if (forParent)
            {
                nestedScrollManager.OnBeginDrag(eventData);
                parentScrollRect.OnBeginDrag(eventData);
                return;
            }
        }

        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (forParent)
        {
            nestedScrollManager.OnDrag(eventData);
            parentScrollRect.OnDrag(eventData);
            return;
        }

        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (forParent)
        {
            nestedScrollManager.OnEndDrag(eventData);
            parentScrollRect.OnEndDrag(eventData);

            return;
        }

        base.OnEndDrag(eventData);
    }

    /// <summary>
    /// 스크롤 바 리셋
    /// </summary>
    public void ResetScrollBar()
    {
        if (horizontal)
            horizontalScrollbar.value = 1f;
        else
            verticalScrollbar.value = 1f;
    }
}
