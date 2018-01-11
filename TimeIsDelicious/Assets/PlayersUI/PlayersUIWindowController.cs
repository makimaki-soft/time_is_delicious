using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UniRx;

// Player UI 全体のView
public class PlayersUIWindowController : MonoBehaviour {

    [SerializeField]
    private GameObject playerUIPrefab; // 将来的にはPrefabからInstantiateする

    private int numberOfPlayers;
    private List<GameObject> _playerViewList; // 子Viewのリスト
    public int MaxNumberOfViewList { get; set; }
    private int _viewComplete = 0;

    public IObservable<Unit> OnGUIAsObservable
    {
        get { return onGUISubject; }
    }
    private Subject<UniRx.Unit> onGUISubject = new Subject<Unit>();


    private Dictionary<string, int> NameUIMap;

    // Use this for initialization
    void Start () {
        _playerViewList = new List<GameObject>();
        numberOfPlayers = 0;
        MaxNumberOfViewList = 0;

        // プレイヤー名とプレハブの紐づけ
        NameUIMap = new Dictionary<string, int>();
        NameUIMap["Chouette"] = 0;
        NameUIMap["鈴木精肉店"] = 1;
        NameUIMap["マザーミート"] = 2;
        NameUIMap["王丸農場"] = 3;

        //// 一度すべて非アクティブ
        for (int i = 0; i < 4; i++)
        {
            var name = "PlayerPrefab_" + i.ToString();
            var UI = transform.Find(name).gameObject;
            UI.SetActive(false);
        }
    }

    public void SetCurrentPlayer(int ID)
    {
        var startIndex = _playerViewList.FindIndex(go => go.GetComponent<PlayerUIController>().PlayerID == ID);
        for (int idx = 0; idx < _playerViewList.Count; idx++)
        {
            int order = (startIndex + idx) % _playerViewList.Count;
            _playerViewList[order].GetComponent<PlayerUIController>().ChangePosision(idx);
        }
    }

    public PlayerUIController AddPlayer(string Name, int ID)
    {
        // Instanciateの位置調整がうまくいかないのでひとまず静的配置からFind
        var name = "PlayerPrefab_" + NameUIMap[Name];
        var UI = transform.Find(name).gameObject;
        //    UI.transform.parent = transform;
        //    var rectTrans = (RectTransform)UI.transform;
        //    rectTrans.anchorMin = new Vector2(0f, 2/9f);
        //    rectTrans.anchorMax = new Vector2(1f, 1f);
        //    rectTrans.offsetMin = new Vector2(0.2f, 0.2f);
        //    rectTrans.offsetMax = new Vector2(0.8f, 0.8f);
        UI.SetActive(true);
        UI.GetComponent<PlayerUIController>().PlayerID = ID;
        UI.GetComponent<PlayerUIController>().ChangePosision(ID).Subscribe(_=>
        {
            Debug.Log("UI View " + ID.ToString() + " Finish");
            if (++_viewComplete == MaxNumberOfViewList)
            {
                // 人数分UIの描画ができたら準備完了
                onGUISubject.OnNext(Unit.Default);
            }
        });

        _playerViewList.Add(UI);
        numberOfPlayers++;

        return UI.GetComponent<PlayerUIController>();
    }
}
