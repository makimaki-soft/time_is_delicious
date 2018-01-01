using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

// Player UI 全体のView
public class PlayersUIWindowController : MonoBehaviour {

    [SerializeField]
    private GameObject playerUIPrefab; // 将来的にはPrefabからInstantiateする

    private int numberOfPlayers;
    private PlayersUIWindowVM _playersWindowVM;
    private List<GameObject> _playerViewList; // 子Viewのリスト
    private int _maxNumberOfViewList;
    private int _viewComplete = 0;

    public bool UIRready { get; private set; }

    private Dictionary<string, int> NameUIMap;

    // Use this for initialization
    void Start () {
        UIRready = false;
        _playerViewList = new List<GameObject>();
        numberOfPlayers = 0;
        _maxNumberOfViewList = 0;

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

        var pObj = GameObject.Find("PermanentObj")?.GetComponent<PermanentObj>();
        
        _playersWindowVM = new PlayersUIWindowVM();
        _playersWindowVM.Permanent = pObj;
        _playersWindowVM.PlayerListVM.CollectionChanged += PlayerListVM_CollectionChanged;
        _playersWindowVM.PropertyChanged += PlayersWindowVM_PropertyChanged;
    }

    private void PlayersWindowVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var playersUIVM = sender as PlayersUIWindowVM;
        switch (e.PropertyName)
        {
            case "NumberOfPlayers":
                _maxNumberOfViewList = playersUIVM.NumberOfPlayers;
                break;
            case "CurrentPlayer":
                var startIndex =_playerViewList.FindIndex(go=>go.GetComponent<PlayerUIController>().PlayerID == playersUIVM.CurrentPlayer.ID);
                for(int idx = 0; idx< _playerViewList.Count; idx++)
                {
                    int order = (startIndex + idx) % _playerViewList.Count;
                    _playerViewList[order].GetComponent<PlayerUIController>().ChangePosision(idx, (msg)=> { });
                }

                break;
        }
    }

    private void PlayerListVM_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:

                foreach (var item in e.NewItems)
                {
                    // Instanciateの位置調整がうまくいかないのでひとまず静的配置からFind
                    var vm = (PlayerVM)item;
                    var name = "PlayerPrefab_" + NameUIMap[vm.Name];
                    var UI = transform.Find(name).gameObject;
                    //    UI.transform.parent = transform;
                    //    var rectTrans = (RectTransform)UI.transform;
                    //    rectTrans.anchorMin = new Vector2(0f, 2/9f);
                    //    rectTrans.anchorMax = new Vector2(1f, 1f);
                    //    rectTrans.offsetMin = new Vector2(0.2f, 0.2f);
                    //    rectTrans.offsetMax = new Vector2(0.8f, 0.8f);
                    UI.SetActive(true);
                    UI.GetComponent<PlayerUIController>().setViewModel((PlayerVM)item);

                    vm.PlayerUI = UI.GetComponent<PlayerUIController>();

                    UI.GetComponent<PlayerUIController>().ChangePosision(vm.ID, (msg)=>
                    {
                        Debug.Log("UI View " + msg + " Finish");
                        if(++_viewComplete == _maxNumberOfViewList )
                        {
                            // 人数分UIの描画ができたら準備完了
                            UIRready = true;
                        }
                    });
                    _playerViewList.Add(UI);
                    numberOfPlayers++;
                }
                Debug.Log("CurrentFoodCardsVM Add");
                break;
            case NotifyCollectionChangedAction.Move:
                Debug.Log("CurrentFoodCardsVM Move");
                break;
            case NotifyCollectionChangedAction.Remove:
                Debug.Log("CurrentFoodCardsVM Remove");
                break;
            case NotifyCollectionChangedAction.Replace:
                Debug.Log("CurrentFoodCardsVM Replace");
                break;
            case NotifyCollectionChangedAction.Reset:
                Debug.Log("CurrentFoodCardsVM Reset");
                break;
        }
    }
}
