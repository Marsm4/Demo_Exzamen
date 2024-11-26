using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Demo_Exzamen
{
    public partial class MainWindow : Window
    {
        private RPM_DEMO_PROBAEntities1 _context;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private string _searchText = "";
        private string _genderFilter = "Все";

        public MainWindow()
        {
            InitializeComponent();
            _context = new RPM_DEMO_PROBAEntities1();
            LoadData();
        }

        private void LoadData()
        {
            var query = _context.Clients.AsQueryable();

            // Фильтрация по полу
            if (_genderFilter != "Все")
            {
                var genderCode = _genderFilter == "Мужской" ? "1" : "2";
                query = query.Where(c => c.GenderCode == genderCode);
            }

            // Поиск
            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(c =>
                    c.FirstName.Contains(_searchText) ||
                    c.LastName.Contains(_searchText) ||
                    c.Patronymic.Contains(_searchText) ||
                    c.Email.Contains(_searchText) ||
                    c.Phone.Contains(_searchText));
            }

            // Сортировка
            query = query.OrderBy(c => c.LastName);

            // Пагинация
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / _pageSize);
            _currentPage = Math.Min(_currentPage, totalPages);

            // Проверка на отрицательное значение для Skip
            var skipCount = (_currentPage - 1) * _pageSize;
            if (skipCount < 0)
            {
                skipCount = 0;
            }

            var clients = query
                .Skip(skipCount)
                .Take(_pageSize)
                .ToList();

            dataGrid.ItemsSource = clients;
            PageInfo.Text = $"{(_currentPage - 1) * _pageSize + 1} - {Math.Min(_currentPage * _pageSize, totalItems)} из {totalItems}";
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            _currentPage++;
            LoadData();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadData();
            }
        }

        private void GenderFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _genderFilter = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();
            _currentPage = 1;
            LoadData();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = ((TextBox)sender).Text;
            _currentPage = 1;
            LoadData();
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = dataGrid.SelectedItem as Clients;
            if (selectedClient != null)
            {
                _context.Clients.Remove(selectedClient);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void BirthdayThisMonth_Click(object sender, RoutedEventArgs e)
        {
            var currentMonth = DateTime.Now.Month;
            var clients = _context.Clients
                .Where(c => c.Birthday.HasValue && c.Birthday.Value.Month == currentMonth)
                .OrderBy(c => c.LastName)
                .ToList();

            dataGrid.ItemsSource = clients;
        }

        private static bool _isAddEditWindowOpen = false;

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAddEditWindowOpen)
            {
                _isAddEditWindowOpen = true;
                var addEditWindow = new AddEditClientWindow(_context);
                addEditWindow.Closed += (s, args) => _isAddEditWindowOpen = false;
                if (addEditWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Окно редактирования уже открыто.");
            }
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = dataGrid.SelectedItem as Clients;
            if (selectedClient != null)
            {
                if (!_isAddEditWindowOpen)
                {
                    _isAddEditWindowOpen = true;
                    var addEditWindow = new AddEditClientWindow(_context, selectedClient);
                    addEditWindow.Closed += (s, args) => _isAddEditWindowOpen = false;
                    if (addEditWindow.ShowDialog() == true)
                    {
                        LoadData();
                    }
                }
                else
                {
                    MessageBox.Show("Окно редактирования уже открыто.");
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.");
            }
        }

        private void PageSizeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    string selectedText = selectedItem.Content.ToString();
                    if (selectedText == "Все")
                    {
                        _pageSize = int.MaxValue; // или количество всех записей в вашем источнике данных
                    }
                    else
                    {
                        _pageSize = int.Parse(selectedText);
                    }

                    // Обновляем количество отображаемых записей
                    _currentPage = 1;
                    LoadData();
                }
            }
        }

        private List<Clients> GetAllClients()
        {
            return _context.Clients.ToList();
        }

        private List<Clients> FilterAndSearchClients(List<Clients> clients)
        {
            var query = clients.AsQueryable();

            // Фильтрация по полу
            if (_genderFilter != "Все")
            {
                var genderCode = _genderFilter == "Мужской" ? "1" : "2";
                query = query.Where(c => c.GenderCode == genderCode);
            }

            // Поиск
            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(c =>
                    c.FirstName.Contains(_searchText) ||
                    c.LastName.Contains(_searchText) ||
                    c.Patronymic.Contains(_searchText) ||
                    c.Email.Contains(_searchText) ||
                    c.Phone.Contains(_searchText));
            }

            return query.ToList();
        }

        private int GetCurrentPage()
        {
            return _currentPage;
        }
    }
}