using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using dpa.Library.Services;

namespace dpa.Library.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    private readonly IPoetryStorage _poetryStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IMenuNavigationService _menuNavigationService;

    public MainWindowViewModel(IPoetryStorage poetryStorage,
        IRootNavigationService rootNavigationService,
        IMenuNavigationService menuNavigationService) {
        _poetryStorage = poetryStorage;
        _rootNavigationService = rootNavigationService;
        _menuNavigationService = menuNavigationService;

        OnInitializedCommand = new RelayCommand(OnInitialized);
    }

    private ViewModelBase _content;

    public ViewModelBase Content {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    public ICommand OnInitializedCommand { get; }

    public void OnInitialized() {
        if (!_poetryStorage.IsInitialized) {
            _rootNavigationService.NavigateTo(RootNavigationConstant
                .InitializationView);
        } else {
            _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
            _menuNavigationService.NavigateTo(MenuNavigationConstant.AnswerView);
        }
    }
}