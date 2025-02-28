using System.Linq.Expressions;
using System.Text.Json;
using dpa.Library.Helpers;
using dpa.Library.Models;

namespace dpa.Library.Services;

public class JinrishiciService : ITodayPoetryService {
    private readonly IAlertService _alertService;

    private readonly IPreferenceStorage _preferenceStorage;

    private string _domainName;

    private readonly IPoetryStorage _poetryStorage;

    public JinrishiciService(IAlertService alertService,
        IPreferenceStorage preferenceStorage,
        IPoetryStorage poetryStorage,
        string domainName = "v2.jinrishici.com") {
        _alertService = alertService;
        _preferenceStorage = preferenceStorage;
        _poetryStorage = poetryStorage;
        _domainName = domainName;
    }

    private string _token = string.Empty;

    public static readonly string JinrishiciTokenKey =
        $"{nameof(JinrishiciService)}.Token";
    
    private const string Server = "今日诗词服务器";

    public async Task<string> GetTokenAsync() {
        if (!string.IsNullOrWhiteSpace(_token)) {
            return _token;
        }

        _token = _preferenceStorage.Get(JinrishiciTokenKey, String.Empty);

        if (!string.IsNullOrWhiteSpace(_token)) {
            return _token;
        }

        using var httpClient = new HttpClient();
        HttpResponseMessage response;

        try {
            response =
                await httpClient.GetAsync(
                    $"https://{_domainName}/token");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(Server, e.Message));

            return _token;
        }

        var json =
            await response.Content.ReadAsStringAsync();
        var jinrishiciToken =
            JsonSerializer.Deserialize<JinrishiciToken>(json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }
            );

        _token = jinrishiciToken.Data;
        _preferenceStorage.Set(JinrishiciTokenKey, _token);

        return _token;
    }
    
    public async Task<TodayPoetry> GetTodayPoetryAsync() {
        var token = await GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token)) {
            return await GetRandomPoetryAsync();
        }

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-User-Token", token);

        HttpResponseMessage response;
        try {
            response =
                await httpClient.GetAsync("https://v2.jinrishici.com/sentence");
            response.EnsureSuccessStatusCode();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(Server, e.Message));
            return await GetRandomPoetryAsync();
        }

        var json = await response.Content.ReadAsStringAsync();
        JinrishiciSentence jinrishiciSentence;
        try {
            jinrishiciSentence = JsonSerializer.Deserialize<JinrishiciSentence>(
                json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new JsonException();
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(Server,
                    e.Message));
            return await GetRandomPoetryAsync();
        }

        try {
            return new TodayPoetry {
                Snippet =
                    jinrishiciSentence.Data?.Content ??
                    throw new JsonException(),
                Name =
                    jinrishiciSentence.Data.Origin?.Title ??
                    throw new JsonException(),
                Dynasty =
                    jinrishiciSentence.Data.Origin.Dynasty ??
                    throw new JsonException(),
                Author =
                    jinrishiciSentence.Data.Origin.Author ??
                    throw new JsonException(),
                Content =
                    string.Join("\n",
                        jinrishiciSentence.Data.Origin.Content ??
                        throw new JsonException()),
                Source = TodayPoetrySources.Jinrishici
            };
        } catch (Exception e) {
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(Server,
                    e.Message));
            return await GetRandomPoetryAsync();
        }
    }
    
    public async Task<TodayPoetry> GetRandomPoetryAsync() {
        var poetries = await _poetryStorage.GetPoetriesAsync(
            Expression.Lambda<Func<Poetry, bool>>(Expression.Constant(true),
                Expression.Parameter(typeof(Poetry), "p")),
            new Random().Next(PoetryStorage.NumberPoetry), 1);
        var poetry = poetries.First();
        return new TodayPoetry {
            Snippet = poetry.Snippet,
            Name = poetry.Name,
            Dynasty = poetry.Dynasty,
            Author = poetry.Author,
            Content = poetry.Content,
            Source = TodayPoetrySources.Local
        };
    }
}

public class JinrishiciToken {
    public string Data { get; set; }
}

public class JinrishiciOrigin {
    public string Title { get; set; } = string.Empty;
    public string Dynasty { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public List<string> Content { get; set; } = [];
}

public class JinrishiciData {
    public string Content { get; set; } = string.Empty;
    public JinrishiciOrigin Origin { get; set; } = new();
}

public class JinrishiciSentence {
    public JinrishiciData Data { get; set; } = new();
}

