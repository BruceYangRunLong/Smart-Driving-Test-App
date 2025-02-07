using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using dpa.Library.Models;
using dpa.Library.Services;
using dpa.Library.ViewModels;
using Moq;
using Xunit;

namespace dpa.UnitTest.ViewModels
{
    public class WrongViewModelTest
    {
        private readonly Mock<IPoetryStorage> _poetryStorageMock;
        private readonly WrongViewModel _viewModel;

        public WrongViewModelTest()
        {
            // 初始化 Mock 对象
            _poetryStorageMock = new Mock<IPoetryStorage>();
            _viewModel = new WrongViewModel(_poetryStorageMock.Object);
        }

        [Fact]
        public async Task LoadExerciseQuestions_PopulatesExerciseQuestionsCorrectly()
        {
            // Arrange
            var mockData = new[]
            {
                new Exercise { Id = 1, question = "Q 1" },
                new Exercise { Id = 2, question = "Q 2" }
            };

            _poetryStorageMock
                .Setup(p => p.GetExerciseQuestionsAsync(null, 0, It.IsAny<int>()))
                .ReturnsAsync(mockData.ToList());

            // Act
            await _viewModel.LoadExerciseQuestions();

            // Assert
            Assert.NotNull(_viewModel.ExerciseQuestions);
            Assert.Equal(2, _viewModel.ExerciseQuestions.Count);
            Assert.Equal("（1）Q 1", _viewModel.ExerciseQuestions[0].question);
            Assert.Equal("（2）Q 2", _viewModel.ExerciseQuestions[1].question);
        }

        [Fact]
        public async Task LoadExerciseQuestions_SetsStatusCorrectly_WhenNoResults()
        {
            // Arrange
            _poetryStorageMock
                .Setup(p => p.GetExerciseQuestionsAsync(null, 0, It.IsAny<int>()))
                .ReturnsAsync(new List<Exercise>());

            // Act
            await _viewModel.LoadExerciseQuestions();

            // Assert
            Assert.Equal(WrongViewModel.NoResult, _viewModel.Status);
        }

        [Fact]
        public async Task OnQuestionClicked_SetsCurrentQuestionCorrectly()
        {
            // Arrange
            var exerciseList = new[]
            {
                new Exercise { Id = 1, question = "Q 1" },
                new Exercise { Id = 2, question = "Q 2" }
            };

            _viewModel.ExerciseQuestions = new ObservableCollection<Exercise>(exerciseList);
            _poetryStorageMock
                .Setup(p => p.GetWrongQuestionByIdAsync(1))
                .ReturnsAsync(new Exercise { Id = 1, question = "Q 1" });

            // Act
            _viewModel.OnQuestionClicked(1);

            // Assert
            Assert.Equal("Q 1", _viewModel.CurrentQuestion.question);
            Assert.Equal(0, _viewModel.CurrentIndex); // id - 1
        }

        [Fact]
        public async Task NextQuestion_UpdatesToNextQuestion()
        {
            // Arrange
            _viewModel.ExerciseQuestions = new ObservableCollection<Exercise>
            {
                new Exercise { Id = 1, question = "Q 1" },
                new Exercise { Id = 2, question = "Q 2" }
            };
            _viewModel.CurrentIndex = 0;

            // Act
            await _viewModel.NextQuestion();

            // Assert
            Assert.Equal(1, _viewModel.CurrentIndex);
            Assert.Equal("Q 2", _viewModel.CurrentQuestion.question);
        }

        [Fact]
        public async Task PreviousQuestion_UpdatesToPreviousQuestion()
        {
            // Arrange
            _viewModel.ExerciseQuestions = new ObservableCollection<Exercise>
            {
                new Exercise { Id = 1, question = "Q 1" },
                new Exercise { Id = 2, question = "Q 2" }
            };
            _viewModel.CurrentIndex = 1;

            // Act
            await _viewModel.PreviousQuestion();

            // Assert
            Assert.Equal(0, _viewModel.CurrentIndex);
            Assert.Equal("Q 1", _viewModel.CurrentQuestion.question);
        }

        [Fact]
        public async Task LoadExerciseQuestions_InitializesCurrentQuestion()
        {
            // Arrange
            var mockData = new[]
            {
                new Exercise { Id = 1, question = "Q 1" },
                new Exercise { Id = 2, question = "Q 2" }
            };

            _poetryStorageMock
                .Setup(p => p.GetExerciseQuestionsAsync(null, 0, It.IsAny<int>()))
                .ReturnsAsync(mockData.ToList());

            _poetryStorageMock
                .Setup(p => p.GetWrongQuestionByIdAsync(1))
                .ReturnsAsync(new Exercise { Id = 1, question = "Q 1" });

            // Act
            await _viewModel.LoadExerciseQuestions();

            // Assert
            Assert.NotNull(_viewModel.CurrentQuestion);
            Assert.Equal("Q 1", _viewModel.CurrentQuestion.question);
        }
    }
}
