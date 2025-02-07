using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Mime;
using System.Windows.Input;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.Input;
using dpa.Library.Helpers;
using dpa.Library.Models;
using dpa.Library.Services;
using Newtonsoft.Json;
using Webiat;

namespace dpa.Library.ViewModels;

public class AnswerViewModel : ViewModelBase
{
    /////////////////////////////////////////////////////////////////////////////////
    //py做的部分
     /// ////////////////////////////////////////////////////////////////////////////
    private AIReplyService _aiReplyService;//ai封装服务
    private AdviseService _adviseService;//交通新闻服务
    private CloudService _cloudService;//云存储服务
    
    private string _answer_exercise;//习题答案
    public string answer_exercise {
        get => _answer_exercise;
        set => SetProperty(ref _answer_exercise, value);
    }
    private string _advise_content;//新闻内容
    public string advise_content {
        get => _advise_content;
        set => SetProperty(ref _advise_content, value);
    }
    private string _advise_picture;//新闻图片
    public string advise_picture {
        get => _advise_picture;
        set => SetProperty(ref _advise_picture, value);
    }
    private string _advise_href;//新闻链接
    public string advise_href {
        get => _advise_href;
        set => SetProperty(ref _advise_href, value);
    }

    private string _advise_picture2;//图片链接

    private string Advise_picture
    {
        get=> _advise_picture2;
        set => SetProperty(ref _advise_picture2, value);
    }
    private string _answer_ai;//ai回答
    public string answer_ai {
        get => _answer_ai;
        set => SetProperty(ref _answer_ai, value);
    }
    private string _question_user;//用户问题
    public string question_user {
        get => _question_user;
        set => SetProperty(ref _question_user, value);
    }
    private Boolean _isFocused;//文本框焦点
    public Boolean isFocused {
        get => _isFocused;
        set => SetProperty(ref _isFocused, value);
    }
    private Boolean _isPaneOpened;//ai页面显示
    public Boolean isPaneOpened {
        get => _isPaneOpened;
        set => SetProperty(ref _isPaneOpened, value);
    }
    public ICommand SubmitCommand { get;}//提交问题command
    public ICommand ClearCommand { get;}//清除记忆command
    public ICommand AIPaneCommand { get;}//ai页面显示开关
    Thread _AILoading;//ai转圈线程
    ManualResetEvent ma;
    public void AILoading()//ai转圈
    {
        while (true)
        {
            //状态1
            ma.WaitOne();
            answer_ai = "AI分析中，请稍候";
            Thread.Sleep(300);
            //状态2
            ma.WaitOne();
            answer_ai = "AI分析中，请稍候.";
            Thread.Sleep(300);
            //状态3
            ma.WaitOne();
            answer_ai = "AI分析中，请稍候..";
            Thread.Sleep(300);
            //状态4
            ma.WaitOne();
            answer_ai = "AI分析中，请稍候...";
            Thread.Sleep(300);
        }
    }
    public async void Submit()//提交问题函数
    {
        answer_ai = "AI分析中，请稍候...";
        string i = question_user;
        question_user = String.Empty;
        isFocused = false;
        ma.Set();
        string answer_ai_1=await _aiReplyService.reply(i,300);
        ma.Reset();
        answer_ai=answer_ai_1;
        isFocused = true;
    }
    public void Clear_1() //清除记忆函数
    {
        _aiReplyService.history_clear();
        answer_ai = "";
    }
    public async void AIPane() //ai页面函数
    {
        isPaneOpened = !isPaneOpened;
    }
    //初始化
    public AnswerViewModel() {
        _aiReplyService = new AIReplyService();
        _adviseService = new AdviseService();
        _cloudService = new CloudService();
        
        OnInitializedCommand = new RelayCommand(OnInitialized);
        NextQuestionCommand = new RelayCommand(GenerateRandomQuestion);
        SubmitCommand1 = new RelayCommand(SubmitData);
        RadioSelectionCommand = new RelayCommand<string>(OnRadioSelection);

        SubmitCommand = new RelayCommand(Submit);
        ClearCommand = new RelayCommand(Clear_1);
        AIPaneCommand = new RelayCommand(AIPane);
        isFocused = true;
        isPaneOpened = false;
        _AILoading = new Thread(AILoading);
        ma = new ManualResetEvent(false);
        ma.Reset();
        _AILoading.Start();
        _advise_picture2 = "D:\\advise.jpg";
        AdviseInitial();//新闻内容拉取
    }
    //新闻内容初始化
    public void AdviseInitial()
    {
        string[] str = _adviseService.getadvise();
        _advise_content = str[0];
        _advise_picture = str[1];
        _advise_href = str[2];
        WebClient webClient = new WebClient();
        string _advise_picture1 = str[1];
        webClient.DownloadFile(_advise_picture1, _advise_picture2);
    }
    
    
    
    
    
    
    
    
    /// /////////////////////////////////////////////////////////////////////////
    //杨润龙做的
    /// /// /////////////////////////////////////////////////////////////////////////
    public ICommand SubmitCommand1 { get; }
    public ICommand RadioSelectionCommand { get; }

    private void OnRadioSelection(string selectedValue)
    {
        //答案显示
        answer_exercise = _currentQuestion.Explains;
        UserAnswer = selectedValue;
        UserAnswer_Answer = UserAnswer + "_" + CurrentQuestion.Answer;
        var dbHelper = new DatabaseHelper();

        int right = 0;
        int wrong = 0;
        if (UserAnswer == CurrentQuestion.Answer)
        {
            right = 1;
        }
        else
        {
            wrong = 1;
        }
        //答题记录
        dbHelper.InsertRecord(right, wrong);
        IsExplainsVisible = true;  // ÉèÖÃ½âÎöÏÔÊ¾
    }
    

        // ÊÂ¼þ£º±£´æ³É¹¦µÄÍ¨Öª
        public event Action SaveSuccessNotification;
        private void SubmitData()
        {
            var dbHelper = new DatabaseHelper();
            dbHelper.InsertExercise(CurrentQuestion.Id, CurrentQuestion.QuestionText, CurrentQuestion.Answer, UserAnswer,
                                    CurrentQuestion.Item1, CurrentQuestion.Item2, CurrentQuestion.Item3, CurrentQuestion.Item4,
                                    CurrentQuestion.Explains, CurrentQuestion.Url);
            //云存储
            Exercise exercise = new Exercise();
            exercise.Id = int.Parse(CurrentQuestion.Id);
            exercise.question = CurrentQuestion.QuestionText;
            exercise.answer = CurrentQuestion.Answer;
            exercise.user_answer = UserAnswer;
            exercise.item1 = CurrentQuestion.Item1;
            exercise.item2 = CurrentQuestion.Item2;
            exercise.item3 = CurrentQuestion.Item3;
            exercise.item4 = CurrentQuestion.Item4;
            exercise.explains = CurrentQuestion.Explains;
            exercise.url = CurrentQuestion.Url;
            _cloudService.upload(exercise);
        }


        private bool _isExplainsVisible;
        public bool IsExplainsVisible
        {
            get => _isExplainsVisible;
            set
            {
                if (_isExplainsVisible != value)
                {
                    _isExplainsVisible = value;
                    OnPropertyChanged(nameof(IsExplainsVisible));
                }
            }
        }
        
        private string _userAnswer;
        public string UserAnswer
        {
            get => _userAnswer;
            set => SetProperty(ref _userAnswer, value);
        }

        private string _userAnswer_Answer;
        public string UserAnswer_Answer
        {
            get => _userAnswer_Answer;
            set => SetProperty(ref _userAnswer_Answer, value);
        }

        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set => SetProperty(ref _currentQuestion, value);
        }

        private List<Question> _questions;

        public ICommand OnInitializedCommand { get; }
        public ICommand NextQuestionCommand { get; }

        public void OnInitialized()
        {
            dynamic responseData = GetApiResponse();
            if (responseData != null)
            {
                _questions = JsonConvert.DeserializeObject<List<Question>>(responseData.result.ToString());
                if (_questions != null && _questions.Count > 0)
                {
                    GenerateRandomQuestion();
                }
            }
        }

        private void GenerateRandomQuestion()
        {
            answer_exercise = "";
            if (_questions != null && _questions.Count > 0)
            {
                UserAnswer = null; // Clear previous answer to reset selection
                CurrentQuestion = _questions[new Random().Next(_questions.Count)];
                //LoadQuestionImageAsync(CurrentQuestion); // Òì²½¼ÓÔØÍ¼Ïñ
                IsExplainsVisible = false;  // ÉèÖÃ½âÎöÏÔÊ¾
                UserAnswer_Answer = null;
            }
        }

        public static dynamic GetApiResponse()
        {
            string url = "http://v.juhe.cn/jztk/query";
            string apiKey = "b4e109461e91c83fcfe103fc8cf241ad";
            //string apiKey = "98fa1b33ae85d5530ff21a0f90383e95";
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "key", apiKey },
                { "subject", "1" },
                { "model", "c1" },
                { "testType", "" }
            };

            using (WebClient client = new WebClient())
            {
                string fullUrl = url + "?" + string.Join("&", data.Select(x => x.Key + "=" + x.Value));

                try
                {
                    string responseContent = client.DownloadString(fullUrl);
                    dynamic responseData = JsonConvert.DeserializeObject(responseContent);
                    return responseData;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    /// /////////////////////////////////////////////////////////////////////////
}
