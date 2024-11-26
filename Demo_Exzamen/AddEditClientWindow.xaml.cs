using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Demo_Exzamen
{
    public partial class AddEditClientWindow : Window
    {
        private static bool _isAddEditWindowOpen = false;
        private RPM_DEMO_PROBAEntities1 _context;
        private Clients _client;
        private bool _isNewClient;

        public AddEditClientWindow(RPM_DEMO_PROBAEntities1 context, Clients client = null)
        {
            if (_isAddEditWindowOpen)
            {
                MessageBox.Show("Окно редактирования уже открыто.");
                return;
            }

            InitializeComponent();
            _isAddEditWindowOpen = true;
            _context = context;
            _client = client;
            _isNewClient = client == null;

            if (_isNewClient)
            {
                _client = new Clients();
                IdTextBox.Text = "Новый клиент";
            }
            else
            {
                LoadClientData();
            }
        }

        private void LoadClientData()
        {
            IdTextBox.Text = _client.ID.ToString();
            LastNameTextBox.Text = _client.LastName;
            FirstNameTextBox.Text = _client.FirstName;
            PatronymicTextBox.Text = _client.Patronymic;
            EmailTextBox.Text = _client.Email;
            PhoneTextBox.Text = _client.Phone;
            BirthdayDatePicker.SelectedDate = _client.Birthday;
            MaleRadioButton.IsChecked = _client.GenderCode == "1";
            FemaleRadioButton.IsChecked = _client.GenderCode == "2";

            if (!string.IsNullOrEmpty(_client.PhotoPath) && File.Exists(_client.PhotoPath))
            {
                PhotoImage.Source = new BitmapImage(new Uri(_client.PhotoPath));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            _client.LastName = LastNameTextBox.Text;
            _client.FirstName = FirstNameTextBox.Text;
            _client.Patronymic = PatronymicTextBox.Text;
            _client.Email = EmailTextBox.Text;
            _client.Phone = PhoneTextBox.Text;

            // Преобразование DateTime в DateTime?
            if (BirthdayDatePicker.SelectedDate.HasValue)
            {
                _client.Birthday = BirthdayDatePicker.SelectedDate.Value.Date;
            }
            else
            {
                _client.Birthday = null;
            }

            _client.GenderCode = MaleRadioButton.IsChecked == true ? "1" : "2";

            if (_isNewClient)
            {
                _context.Clients.Add(_client);
            }

         
                _context.SaveChanges();
                DialogResult = true;
                Close();
            
           
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.png)|*.jpg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                if (fileInfo.Length > 2 * 1024 * 1024)
                {
                    MessageBox.Show("Размер фотографии не должен превышать 2 мегабайта.");
                    return;
                }

                // Создаем папку Images, если она не существует
                var imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                // Генерируем уникальное имя файла
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(openFileDialog.FileName);
                var destinationPath = Path.Combine(imagesFolder, fileName);

                // Копируем файл в папку Images
                File.Copy(openFileDialog.FileName, destinationPath, true);

                // Сохраняем путь к файлу в базе данных
                _client.PhotoPath = destinationPath;
                PhotoImage.Source = new BitmapImage(new Uri(destinationPath));
            }
        }

        private bool ValidateInput()
        {
            if (!LastNameTextBox.Text.All(c => char.IsLetter(c) || c == ' ' || c == '-') || LastNameTextBox.Text.Length > 50)
            {
                MessageBox.Show("Фамилия должна содержать только буквы, пробел и дефис, и не должна превышать 50 символов.");
                return false;
            }

            if (!FirstNameTextBox.Text.All(c => char.IsLetter(c) || c == ' ' || c == '-') || FirstNameTextBox.Text.Length > 50)
            {
                MessageBox.Show("Имя должно содержать только буквы, пробел и дефис, и не должно превышать 50 символов.");
                return false;
            }

            if (!PatronymicTextBox.Text.All(c => char.IsLetter(c) || c == ' ' || c == '-') || PatronymicTextBox.Text.Length > 50)
            {
                MessageBox.Show("Отчество должно содержать только буквы, пробел и дефис, и не должно превышать 50 символов.");
                return false;
            }

            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Неверный формат email.");
                return false;
            }

            if (!PhoneTextBox.Text.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == '(' || c == ')' || c == ' '))
            {
                MessageBox.Show("Телефон должен содержать только цифры, плюс, минус, открывающую и закрывающую круглые скобки, знак пробела.");
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _isAddEditWindowOpen = false;
        }
    }
}