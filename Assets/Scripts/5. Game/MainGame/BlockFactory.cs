
public class BlockFactory
{
    private MainGameController controller; // 게임 모델

    /// <summary>
    /// 생성자
    /// Model을 매개변수로 받아 등록한다.
    /// </summary>
    /// <param name="model"></param>
    public BlockFactory(MainGameController controller)
    {
        this.controller = controller;
    }

    /// <summary>
    /// 블록 생성
    /// </summary>
    /// <returns>각각 좌,우 블록</returns>
    public (BlockObject, BlockObject) CreateBlock()
    {
        var leftBlockObject = CreateBlockObject();
        var rightBlockObject = CreateBlockObject();

        var (leftBlockData, rightBlockData) = CreateBlockData();

        leftBlockObject.Init(leftBlockData);
        rightBlockObject.Init(rightBlockData);

        return (leftBlockObject, rightBlockObject);
    }

    /// <summary>
    /// 블록 데이터를 생성한다.
    /// 어떤 블록 데이터를 생성할 지 게임 상황이나 시간, 블록사용한 개수 등 여러변수를 연관시켜서 최적의 블록을 생성해야한다.
    /// </summary>
    /// <returns></returns>
    private (BlockGameData, BlockGameData) CreateBlockData()
    {
        BlockGameData leftBlockData, rightBlockData;

        if (controller.IsFever.Value) // 피버타임이라면 파워업 블록을 생성 후 리턴시킨다.
        {
            leftBlockData = DBManager.Instance.GetBlockGameData(BlockType.PowerUp);
            rightBlockData = DBManager.Instance.GetBlockGameData(BlockType.PowerUp);
            return (leftBlockData, rightBlockData);
        }

        bool rand = Random.Bool();
        bool createItem = Random.Gacha(controller.ItemSpawnChance);

        if (rand)
        {
            if (createItem)
            {
                int itemNum = Random.Range(2, (int)BlockType.End - 1);
                //itemNum = 3;

                leftBlockData = DBManager.Instance.GetBlockGameData((BlockType)itemNum);

                rand = Random.Bool();

                if (rand)
                    rightBlockData = DBManager.Instance.GetBlockGameData(BlockType.PowerUp);
                else
                    rightBlockData = DBManager.Instance.GetBlockGameData(BlockType.PowerDown);
            }
            else
            {
                leftBlockData = DBManager.Instance.GetBlockGameData(BlockType.PowerUp);
                rightBlockData = DBManager.Instance.GetBlockGameData(BlockType.PowerDown);
            }
        }
        else
        {
            if (createItem)
            {
                int itemNum = Random.Range(2, (int)BlockType.End - 1);
                //itemNum = 3;
                rightBlockData = DBManager.Instance.GetBlockGameData((BlockType)itemNum);

                rand = Random.Bool();

                if (rand)
                    leftBlockData = DBManager.Instance.GetBlockGameData(BlockType.PowerUp);
                else
                    leftBlockData = DBManager.Instance.GetBlockGameData(BlockType.PowerDown);
            }
            else
            {
                leftBlockData = DBManager.Instance.GetBlockGameData(BlockType.PowerDown);
                rightBlockData = DBManager.Instance.GetBlockGameData(BlockType.PowerUp);
            }
        }
        return (leftBlockData, rightBlockData);
    }

    public void ClearData()
    {

    }

    /// <summary>
    /// 데이터 클리어 > 현재 PoolManager의 BlockObjectPool을 초기화 해 주는 역할만 한다.
    /// </summary>
    public void Dispose()
    {
        PoolManager.Instance.Clear(PoolObjectType.BlockObject);
    }

    /// <summary>
    /// 블록 오브젝트 생성 > 그냥 Pool에서 가져오는 것
    /// </summary>
    /// <returns></returns>
    private BlockObject CreateBlockObject()
    {
        var obj = PoolManager.Instance.Rent(PoolObjectType.BlockObject);
        if (obj != null)
            return obj.GetComponent<BlockObject>();

        return null;
        //return PoolManager.Instance.Rent(PoolObjectType.BlockObject) as BlockObject;
    }
}
