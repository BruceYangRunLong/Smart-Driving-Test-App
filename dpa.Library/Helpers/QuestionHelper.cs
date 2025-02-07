using Avalonia.Media.Imaging;
using Newtonsoft.Json;

using System.ComponentModel;

namespace dpa.Library.Helpers
{

    public class Question : INotifyPropertyChanged
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("question")]
        public string QuestionText { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("item1")]
        public string Item1 { get; set; }

        [JsonProperty("item2")]
        public string Item2 { get; set; }

        [JsonProperty("item3")]
        public string Item3 { get; set; }

        [JsonProperty("item4")]
        public string Item4 { get; set; }


        private string _explains;
        [JsonProperty("explains")]
        // Property for Explains
        public string Explains
        {
            get => _explains;
            set
            {
                if (_explains != value)
                {
                    _explains = value;
                    OnPropertyChanged(nameof(Explains));  // Notify UI of the change
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [JsonProperty("url")]
        public string Url { get; set; }

        public async Task<Bitmap> GetImageSourceAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var imageBytes = await client.GetByteArrayAsync(Url);
                    using (MemoryStream stream = new MemoryStream(imageBytes))
                    {
                        // Convert the image data to Avalonia's Bitmap
                        return new Bitmap(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                // Return a default or null image in case of failure
                return null;
            }
        }
    }
    public class QuestionWrapper
    {
        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("result")]
        public List<Question> Result { get; set; }

        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }
    }


    public class QuizService
    {
        private List<Question> _questions;

        public QuizService(string jsonData)
        {
            var wrapper = JsonConvert.DeserializeObject<QuestionWrapper>(jsonData);
            _questions = wrapper.Result;

        }

        public Question GetRandomQuestion()
        {
            if (_questions == null || _questions.Count == 0)
            {
                throw new InvalidOperationException("Question pool is empty.");
            }

            Random random = new Random();
            int index = random.Next(_questions.Count);
            return _questions[index];
        }
    }

}
