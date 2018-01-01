using RuleManager;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UniRx;

public class PlayersUIWindowVM : VMBase {

    private MainModel _singletonMainModel;

    public PlayersUIWindowVM()
    {
        _playerListVM = new ObservableCollection<PlayerVM>();

        _singletonMainModel = MainModel.Instance;

        _singletonMainModel.NumberOfPlayers.Subscribe(val =>
        {
            NumberOfPlayers = val;
        });

        _singletonMainModel.CurrentPlayer
                           .Where((arg) => arg != null)
                           .Subscribe(val =>
        {
            CurrentPlayer = _playerListVM.FirstOrDefault(v => v.ID == _singletonMainModel.CurrentPlayer.Value.ID);
        });

        _singletonMainModel.CurrentStatus.Subscribe(val =>
        {
            if (_singletonMainModel.CurrentStatus.Value == MainModel.Status.GameEnd)
            {
                if (Permanent != null)
                {
                    Permanent.playerNum = _singletonMainModel.NumberOfPlayers.Value;
                    Permanent.players = _singletonMainModel.Players.Select(player => player.ToPlayerScore()).ToArray();
                }
                FadeManager.Instance.LoadScene("GameEnd", 1.0f);
            }
        });

        _singletonMainModel.Players.ObserveAdd().Subscribe(item => 
        {
            _playerListVM.Add(new PlayerVM(item.Value));
        });
    }

    public PermanentObj Permanent { get; set; }

    private ObservableCollection<PlayerVM> _playerListVM;
    public ObservableCollection<PlayerVM> PlayerListVM
    {
        get { return _playerListVM; }
    }

    private int _numberOfPlayers;
    public int NumberOfPlayers
    {
        get { return _numberOfPlayers; }
        set
        {
            if (_numberOfPlayers != value)
            {
                _numberOfPlayers = value;
                NotifyPropertyChanged();
            }

        }
    }

    private PlayerVM _currentPlayer;
    public PlayerVM CurrentPlayer
    {
        get { return _currentPlayer; }
        set
        {
            if (_currentPlayer != value)
            {
                _currentPlayer = value;
                NotifyPropertyChanged();
            }
        }
    }
}
