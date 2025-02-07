using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Drawing;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using dpa.Library.Models;
using dpa.Library.Services;

using Webiat;


namespace dpa.Library.ViewModels;

public class ProgressViewModel: ViewModelBase,INotifyPropertyChanged
{
    private readonly IPoetryStorage _poetryStorage;
    private PredictService _predictService;
    private ObservableCollection<Record> _records;
    private ObservableCollection<double> _scores;
    public ObservableCollection<Record> records
    {
        get => _records;
        set => SetProperty(ref _records, value);
    }

    public ObservableCollection<double> scores
    {
        get => _scores;
        set => SetProperty(ref _scores, value);
    }
    private double predict_score;

    public double PredictScore
    {
        get => predict_score;
        set => SetProperty(ref predict_score, value);
    }
    public ProgressViewModel(IPoetryStorage poetryStorage)
    {
        predict_score = 0;
        _poetryStorage = poetryStorage;
        _predictService = new PredictService();
        Task.Run(async () => await LoadRecordsasync());
    }
    private async Task LoadRecordsasync()
    {
        var records0 = await _poetryStorage.GetRecordsAsync(null, 0, 1000);
        records = new ObservableCollection<Record>(records0);
        scores = new ObservableCollection<double>();
        foreach (var i in records)
        {
            scores.Add(((double)i.right)/(((double)i.right)+((double)i.wrong)));
        }
        PredictScore = _predictService.Calculate(scores);
    }
}
