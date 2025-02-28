using System;
using dpa.Library.Services;
using dpa.Library.ViewModels;

namespace dpa.Services;

public class ContentNavigationService : IContentNavigationService {
    public void NavigateTo(string view, object parameter = null) {
        ViewModelBase viewModel = view switch {
            ContentNavigationConstant.TodayDetailView => ServiceLocator.Current
                .TodayDetailViewModel,
            ContentNavigationConstant.ResultView => ServiceLocator.Current
                .ResultViewModel,
            // ContentNavigationConstant.DetailView => ServiceLocator.Current
            //     .DetailViewModel,
            _ => throw new Exception("未知的视图。")
        };
        
        viewModel.SetParameter(parameter);
        
        ServiceLocator.Current.MainViewModel.PushContent(viewModel);
    }
}