using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Lab6FileManager
{
    public class FileSystemItem
    {
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public bool IsDirectory { get; set; }
        public string SizeText { get; set; } = string.Empty;
        public string ModifiedDate { get; set; } = string.Empty;
        public string Icon => IsDirectory ? "📁" : "📄";
    }

    public partial class MainWindow : Window
    {
        private ObservableCollection<FileSystemItem> _items;
        private string _currentPath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            _items = new ObservableCollection<FileSystemItem>();
            LstFiles.ItemsSource = _items;
            
            LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }

        private void LoadDirectory(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                ObservableCollection<FileSystemItem> newItems = new ObservableCollection<FileSystemItem>();

                foreach (DirectoryInfo d in dirInfo.GetDirectories())
                {
                    if (d.Name.StartsWith(".") || 
                        d.Attributes.HasFlag(FileAttributes.Hidden) || 
                        d.Attributes.HasFlag(FileAttributes.System))
                    {
                        continue;
                    }

                    newItems.Add(new FileSystemItem
                    {
                        Name = d.Name,
                        FullPath = d.FullName,
                        IsDirectory = true,
                        SizeText = "<DIR>",
                        ModifiedDate = d.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                    });
                }

                foreach (FileInfo f in dirInfo.GetFiles())
                {
                    if (f.Name.StartsWith(".") || 
                        f.Attributes.HasFlag(FileAttributes.Hidden) || 
                        f.Attributes.HasFlag(FileAttributes.System))
                    {
                        continue;
                    }

                    newItems.Add(new FileSystemItem
                    {
                        Name = f.Name,
                        FullPath = f.FullName,
                        IsDirectory = false,
                        SizeText = $"{f.Length / 1024} KB",
                        ModifiedDate = f.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                    });
                }
                _items = newItems;
                LstFiles.ItemsSource = _items;
                _currentPath = dirInfo.FullName;
                TxtPath.Text = _currentPath;
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentPath)) return;
            
            DirectoryInfo? parent = Directory.GetParent(_currentPath);
            if (parent != null)
            {
                LoadDirectory(parent.FullName);
            }
        }

        private void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            LoadDirectory(TxtPath.Text);
        }

        private void TxtPath_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadDirectory(TxtPath.Text);
            }
        }

        private void LstFiles_DoubleTapped(object sender, TappedEventArgs e)
        {
            if (LstFiles.SelectedItem is FileSystemItem selectedItem && selectedItem.IsDirectory)
            {
                LoadDirectory(selectedItem.FullPath);
            }
        }
    }
}