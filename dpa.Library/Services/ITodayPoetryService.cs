using dpa.Library.Models;

namespace dpa.Library.Services;

public interface ITodayPoetryService {
    Task<TodayPoetry> GetTodayPoetryAsync();
}

public static class TodayPoetrySources {
    public const string Jinrishici = nameof(Jinrishici);

    public const string Local = nameof(Local);
}