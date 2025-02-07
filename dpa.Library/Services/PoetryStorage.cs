using System.Linq.Expressions;
using System.Text;
using dpa.Library.Helpers;
using dpa.Library.Models;
using SQLite;

namespace dpa.Library.Services;

public class PoetryStorage : IPoetryStorage {
       //为什么连不上数据库呢？
    public const int NumberPoetry = 30;

    public const string DbName = "poetrydb.sqlite3";

    public static readonly string PoetryDbPath =
        PathHelper.GetLocalFilePath(DbName);

    private SQLiteAsyncConnection _connection;

    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(PoetryDbPath);

    private readonly IPreferenceStorage _preferenceStorage;
    
    public PoetryStorage(IPreferenceStorage preferenceStorage) {
        _preferenceStorage = preferenceStorage;
        if (!File.Exists(PoetryDbPath))
        {
            InitializeAsync().Wait();
        }
    }

    public bool IsInitialized =>
        _preferenceStorage.Get(PoetryStorageConstant.VersionKey,
            default(int)) == PoetryStorageConstant.Version;

    public async Task InitializeAsync() {
        await using var dbFileStream =
            new FileStream(PoetryDbPath, FileMode.OpenOrCreate);
        await using var dbAssetStream =
            typeof(PoetryStorage).Assembly.GetManifestResourceStream(DbName);
        await dbAssetStream.CopyToAsync(dbFileStream);
        
        _preferenceStorage.Set(PoetryStorageConstant.VersionKey,
            PoetryStorageConstant.Version);

        await Connection.CloseAsync();
    }

    public async Task<Poetry> GetPoetryAsync(int id) =>
        await Connection.Table<Poetry>().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IList<Poetry>> GetPoetriesAsync(
        Expression<Func<Poetry, bool>> where, int skip, int take) =>
        await Connection.Table<Poetry>().Where(where).Skip(skip).Take(take)
            .ToListAsync();
    
    //得到错题列表的方法
    public async Task<List<Exercise>> GetExerciseQuestionsAsync(Expression<Func<Exercise, bool>> where, int skip, int take)
    {
        // 如果没有筛选条件，则查询所有记录
        var exercisesQuery = Connection.Table<Exercise>();

        if (where != null)
        {
            exercisesQuery = exercisesQuery.Where(where);  // 应用筛选条件
        }

        var exercises = await exercisesQuery.Skip(skip).Take(take).ToListAsync();  // 执行分页

        return exercises;  // 直接返回 Exercise 对象列表
    }
    
    public async Task<Exercise> GetWrongQuestionByIdAsync(int id)
    {
        var exerciseQuery = Connection.Table<Exercise>();
    
        var exercise = await exerciseQuery.Where(e => e.Id == id).FirstOrDefaultAsync();
        
        return exercise;  // 返回匹配的 Exercise 对象，如果没找到则返回 null
    }
    //得到答题记录的方法
    public async Task<List<Record>> GetRecordsAsync(Expression<Func<Record, bool>> where, int skip, int take)
    {
        // 如果没有筛选条件，则查询所有记录
        var recordsQuery = Connection.Table<Record>();
        if (where != null)
        {
            recordsQuery = recordsQuery.Where(where);  // 应用筛选条件
        }

        var records = await recordsQuery.Skip(skip).Take(take).ToListAsync();  // 执行分页
        var records_result = new List<Record>();
        foreach (var record in records)
        {
            var record_i=GetRecordBydate(records_result, record.date);
            if (record_i == null)
            {
                records_result.Add(record);
            }
            else
            {
                record_i.right += record.right;
                record_i.wrong += record.wrong;
            }
        }
        return records_result;  // 直接返回 Exercise 对象列表
    }
    //查询日期，如果有则返回，没有则空指针
    public Record GetRecordBydate(List<Record> records,string date)
    {
        foreach (var record in records)
        {
            if (record.date.Equals(date))
            {
                return record;
            }
        }
        return null;
    }
    
    public async Task CloseAsync() => await Connection.CloseAsync();
}

public static class PoetryStorageConstant {
    public const int Version = 1;

    public const string VersionKey =
        nameof(PoetryStorageConstant) + "." + nameof(Version);
    // PoetryStorageConstant.Version
}
