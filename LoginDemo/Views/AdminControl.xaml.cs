using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoginDemo.Views
{
    public partial class AdminControl : UserControl
    {
        private string dbPath = @"Data Source=C:\Users\user\authdemo.db";
        public AdminControl()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using var connection = new SqliteConnection(dbPath);
            connection.Open();
            using var command = new SqliteCommand("SELECT * FROM Users", connection);

            using var reader = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            UsersGrid.ItemsSource = dt.DefaultView;
        }
        private void UsersGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow)?.SwitchToLogin();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            string login = InputLogin.Text;
            string password = InputPassword.Text;
            string role = (InputRole.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            using var connection = new SqliteConnection(dbPath);
            connection.Open();

            using var command = new SqliteCommand(
                "INSERT INTO Users (Login, Password, Role) VALUES ($l, $p, $r)", connection);

            command.Parameters.AddWithValue("$l", login);
            command.Parameters.AddWithValue("$p", password);
            command.Parameters.AddWithValue("$r", role);

            command.ExecuteNonQuery();
            InputLogin.Clear();
            InputPassword.Clear();
            InputRole.SelectedIndex = -1;

            LoadUsers();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView row)
            {
                string login = row["Login"].ToString();
                string password = row["Password"].ToString();
                int id = Convert.ToInt32(row["Id"]);

                if (login == "admin" && password == "admin")
                {
                    MessageBox.Show("Главного администратора удалять нельзя!");
                    return;
                }

                if (MessageBox.Show(
                    $"Удалить пользователя ID={id}?",
                    "Удаление",
                    MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    return;

                using var connection = new SqliteConnection(dbPath);
                connection.Open();

                using var command = new SqliteCommand(
                    "DELETE FROM Users WHERE Id = $id", connection);
                command.Parameters.AddWithValue("$id", id);
                command.ExecuteNonQuery();
                LoadUsers();
            }
        }
    }
}
