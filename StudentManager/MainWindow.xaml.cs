using System.Windows;
using System.Data;
using Microsoft.Data.Sqlite;

namespace StudentManager
{
    public partial class MainWindow : Window
    {
        private const string ConectionString = @"Data Source = C:\Users\user\students2.db";
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            using var connection = new SqliteConnection(ConectionString);
            connection.Open();
            using var command = new SqliteCommand("SELECT ID, Name FROM Students ORDER BY ID", connection);
            using var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            DataGridPeople.ItemsSource = dt.DefaultView;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var name = InputName.Text?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show(
                    "Введите имя перед добавлением.",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            try
            {
                using var connection = new SqliteConnection(ConectionString);
                connection.Open();
                using var command = new SqliteCommand($"INSERT INTO Students (Name) VALUES ($name);", connection);
                command.Parameters.AddWithValue("$name", name);
                command.ExecuteNonQuery();
                LoadData();
                InputName.Clear();
            }
            catch (Exception ex)
            {

                MessageBox.Show(
                    $"Ошибка при добавлении: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }


        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPeople.SelectedItem is not DataRowView row)
            {
                MessageBox.Show(
                    "Выберите запись для удаления.",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            //var row = (DataRowView)DataGridPeople.SelectedItem;
            long idLong;
            try
            {
                idLong = Convert.ToInt64(row["ID"]);
            }
            catch
            {
                MessageBox.Show(
                    "Не удалось прочитать ID выбранной записи.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            var answer = MessageBox.Show(
                $"Удалить запись с ID = {idLong}?",
                "Подтвердите удаление",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (answer != MessageBoxResult.Yes) return;

            try
            {
                using var connection = new SqliteConnection(ConectionString);
                connection.Open();
                using var command = new SqliteCommand($"DELETE FROM Students WHERE ID = $id;", connection);
                command.Parameters.AddWithValue("$id", idLong);

                var affected = command.ExecuteNonQuery();
                if (affected == 0)
                {
                    MessageBox.Show(
                        "Ничего не удалено (запись не найдена).",
                        "Информация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ;
        }
    }
}