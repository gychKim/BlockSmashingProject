using UniRx;
using UnityEditor.Overlays;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class GameItemSelectPresenter : BasePresenter<GameItemSelectView, GameItemSelectModel>
{
    public GameItemSelectPresenter(GameItemSelectView view, GameItemSelectModel model) : base(view, model)
    {
    }

    public override void Bind()
    {
        // 기존에 넣어져 있던 데이터 Binding
        foreach (var item in model.Items)
        {
            ItemBinding(item);
        }

        // 아이템 생성 및 추가될 시 > 이런 일이 어지간 하면 없지만 혹시나 모르니까
        model.Items
            .ObserveAdd()
            .Subscribe(item =>
            {
                ItemBinding(item.Value);

            }).AddTo(view.Disposables);

        // 아이템 사용 버튼
        view.UseButton
            .OnClickAsObservable()
            .Subscribe(async _ =>
            {
                int selectedItemID = model.SelectedItem.ID;
                var itemSaveData = await SaveManager.Instance.ItemSaveManager.GetReadOnly(selectedItemID);
                if(itemSaveData.itemCount <= 0)
                {
                    DebugX.Log("0개 이하인 아이템은 사용할 수 없습니다.");
                    return;
                }

                bool result = MainGameManager.Instance.AddShopItemEffect(selectedItemID); // 현재 게임 시작 시 사용된 아이템 효과 적용
                if (result)
                    await SaveManager.Instance.ItemSaveManager.UpdatePurchase(selectedItemID, -1);
                else
                    DebugX.Log("이미 사용된 아이템입니다.");

                //var data = SaveManager.Instance.ItemSaveManager.ItemSaveDataList.Find((item) => item.itemID == model.SelectedItem.ID);
                model.SelectedItem.CountRP.Value = itemSaveData.itemCount.ToString();

                model.ResetSelect();
                view.CloseInfoUI();
                view.CloseUseButton();

            }).AddTo(view.Disposables);
    }

    /// <summary>
    /// Item의 데이터 Binding
    /// </summary>
    /// <param name="item"></param>
    private void ItemBinding(GameSelectItem item)
    {
        view.AddItem(item.gameObject); // 아이템을 Content에 넣는다.

        item.transform.localScale = Vector3.one; // 아이템의 스케일 조정

        item.SelectButton
            .OnClickAsObservable()
            .Subscribe(async _ =>
            {
                var itemData = DBManager.Instance.GetShopItemData(item.ID);
                var saveData = await SaveManager.Instance.ItemSaveManager.GetReadOnly(item.ID);

                if (saveData == null)
                {
                    DebugX.Log("이상하다... 이럴 리 없는데...?");
                    return;
                }

                view.SetItem(itemData, saveData.itemCount); // Info창 갱신 및 사용버튼 활성화

                model.SelectItem(item); // 아이템 선택

            }).AddTo(view.Disposables);
    }

    public override void Destroy()
    {
        
    }
}
