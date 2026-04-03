using UnityEngine;

public class MainGameBinder : BaseBinder<MainGameView, MainGamePresentor, MainGameModel>
{

    //[LabelColor("Model 필요 데이터", 255, 0, 0)]
    //[SerializeField]
    //private BlockContainer leftBlockContainer; // 좌측블록 컨테이너
    //[SerializeField]
    //private BlockContainer rightBlockContainer; // 우측블록 컨테이너
    //[SerializeField]
    //private CurrentBlockFrame currentLeftBlockFrame; // 좌측 현재 블록 프레임
    //[SerializeField]
    //private CurrentBlockFrame currentRightBlockFrame; // 우측 현재 블록 프레임

    protected override MainGameModel CreateModel()
    {
        return new MainGameModel();
    }

    protected override MainGamePresentor CreatePresenter(MainGameView view, MainGameModel model)
    {
        return new MainGamePresentor(view, model);
    }
}
