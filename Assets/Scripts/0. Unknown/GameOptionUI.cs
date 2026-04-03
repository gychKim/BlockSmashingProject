using UnityEngine;
using UnityEngine.UI;

public class GameOptionUI : MonoBehaviour
{
    /// <summary>
    /// 옵션 UI 오브젝트
    /// </summary>
    public GameObject OptionPanel => optionUI;
    [SerializeField]
    private GameObject optionUI;

    /// <summary>
    /// 옵션 클릭 버튼
    /// </summary>
    public Button OptionButton => optionButton;
    [SerializeField]
    private Button optionButton;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
