using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TodoList
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<TaskItem> tasks;
        public ObservableCollection<TaskItem> Tasks
        {
            get { return tasks; }
            set
            {
                tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        private bool showIncompleteTasks;
        public bool ShowIncompleteTasks
        {
            get { return showIncompleteTasks; }
            set
            {
                showIncompleteTasks = value;
                OnPropertyChanged(nameof(ShowIncompleteTasks));
                UpdateTaskList();
            }
        }

        private string selectedFilter;
        public string SelectedFilter
        {
            get { return selectedFilter; }
            set
            {
                selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
                ApplyFilter();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Tasks = new ObservableCollection<TaskItem>();
            SelectedFilter = "All"; // Set the default filter to "All"
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text;
            string description = txtDescription.Text;
            DateTime dueDate = dpDueDate.SelectedDate.HasValue ? dpDueDate.SelectedDate.Value : DateTime.Now;

            if (dueDate.Date < DateTime.Today)
            {
                MessageBox.Show("Please select a future date.", "Invalid Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(title))
            {
                TaskItem task = new TaskItem { Title = title, Description = description, DueDate = dueDate, Completed = false };
                Tasks.Add(task);
                txtTitle.Text = "Title";
                txtDescription.Text = "Description";
                dpDueDate.SelectedDate = null;
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (lvTasks.SelectedItem != null)
            {
                Tasks.Remove((TaskItem)lvTasks.SelectedItem);
                UpdateTaskList();
            }
        }

        private void MarkAsComplete_Click(object sender, RoutedEventArgs e)
        {
            if (lvTasks.SelectedItem != null)
            {
                TaskItem task = (TaskItem)lvTasks.SelectedItem;
                task.Completed = true;
                UpdateTaskList();
            }
        }

        private void chkShowIncomplete_Checked(object sender, RoutedEventArgs e)
        {
            ShowIncompleteTasks = true;
        }

        private void chkShowIncomplete_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowIncompleteTasks = false;
        }

        private void toggleFilter_Checked(object sender, RoutedEventArgs e)
        {
            cmbFilter.IsEnabled = true;
            ApplyFilter();
        }

        private void toggleFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbFilter.IsEnabled = false;
            cmbFilter.SelectedIndex = 0; // Reset the combo box selection to "All"
            ApplyFilter();
        }

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFilter = (cmbFilter.SelectedItem as ComboBoxItem)?.Tag.ToString();
        }

        private void ApplyFilter()
        {
            if (toggleFilter?.IsChecked == true && cmbFilter?.SelectedItem != null)
            {
                string filter = (cmbFilter.SelectedItem as ComboBoxItem)?.Tag.ToString();

                ICollectionView view = CollectionViewSource.GetDefaultView(Tasks);
                view.Filter = null;

                if (filter == "Completed")
                {
                    view.Filter = item => ((TaskItem)item).Completed;
                }
                else if (filter == "Incomplete")
                {
                    view.Filter = item => !((TaskItem)item).Completed;
                }
            }
            else
            {
                UpdateTaskList();
            }
        }

        private void UpdateTaskList()
        {
            if (Tasks != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(Tasks);
                view.Refresh();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Title" || textBox.Text == "Description")
            {
                textBox.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = textBox.Name == "txtTitle" ? "Title" : "Description";
            }
        }

        private void TaskTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (lvTasks.SelectedItem != null)
            {
                TaskItem task = (TaskItem)lvTasks.SelectedItem;
                if (textBox.Name == "txtTitle")
                    task.Title = textBox.Text;
                else if (textBox.Name == "txtDescription")
                    task.Description = textBox.Text;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TaskItem : INotifyPropertyChanged
    {
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private DateTime dueDate;
        public DateTime DueDate
        {
            get { return dueDate; }
            set
            {
                dueDate = value;
                OnPropertyChanged(nameof(DueDate));
            }
        }

        private bool completed;
        public bool Completed
        {
            get { return completed; }
            set
            {
                completed = value;
                OnPropertyChanged(nameof(Completed));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
