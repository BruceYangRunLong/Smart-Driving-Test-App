



using System.Data.SQLite;
using dpa.Library.Models;

namespace dpa.Library.Helpers
{

    public class DatabaseHelper
    {
        public const string dbpath1 = "poetrydb.sqlite3";
        public static readonly string connectionString="Data Source="+PathHelper.GetLocalFilePath(dbpath1)+";"+"Version=3;";
        public void InsertRecord(int right, int wrong)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    //插入记录
                    string query = "INSERT INTO records (right, wrong, date) " +
                                                 "VALUES (@right, @wrong, @date)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@right", right);
                        command.Parameters.AddWithValue("@wrong", wrong);
                        command.Parameters.AddWithValue("@date", DateTime.Now.ToString("d"));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void InsertExercise(string questionId, string question, string answer, string userAnswer, string item1, string item2, string item3, string item4, string explains, string url)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // 检查是否已存在该 questionId
                    string checkQuery = "SELECT COUNT(1) FROM exercises WHERE Id = @Id";
                    using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Id", questionId);
                        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            // 如果已经存在，则不插入并返回
                            return;
                        }
                    }


                    string query = "INSERT INTO exercises (Id, question, answer, user_answer, item1, item2, item3, item4, explains, url) " +
                                                 "VALUES (@questionId, @question, @answer, @userAnswer, @item1, @item2, @item3, @item4, @explains, @url)";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@questionId", questionId);
                        command.Parameters.AddWithValue("@question", question);
                        command.Parameters.AddWithValue("@answer", answer);
                        command.Parameters.AddWithValue("@userAnswer", userAnswer);
                        command.Parameters.AddWithValue("@item1", item1);
                        command.Parameters.AddWithValue("@item2", item2);
                        command.Parameters.AddWithValue("@item3", item3);
                        command.Parameters.AddWithValue("@item4", item4);
                        command.Parameters.AddWithValue("@explains", explains);
                        command.Parameters.AddWithValue("@url", url);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

}
