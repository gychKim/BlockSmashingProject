using UnityEngine;

public class BlockSelectUIBinder : BaseBinder<BlockSelectUIView, BlockSelectUIPresenter, BlockSelectUIModel>
{
    protected override BlockSelectUIModel CreateModel()
    {
        return new BlockSelectUIModel();
    }

    protected override BlockSelectUIPresenter CreatePresenter(BlockSelectUIView view, BlockSelectUIModel model)
    {
        return new BlockSelectUIPresenter(view, model);
    }
}
