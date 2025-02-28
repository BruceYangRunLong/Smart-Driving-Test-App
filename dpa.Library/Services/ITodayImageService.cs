using dpa.Library.Models;

namespace dpa.Library.Services;

public interface ITodayImageService {
    Task<TodayImage> GetTodayImageAsync();

    Task<TodayImageServiceCheckUpdateResult> CheckUpdateAsync();
}

public class TodayImageServiceCheckUpdateResult {
    public bool HasUpdate { get; set; }

    public TodayImage TodayImage { get; set; } = new();
}