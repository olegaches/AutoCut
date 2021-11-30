﻿using netDxf;
using netDxf.Header;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace ProjectZavod.ViewModels
{
    public class MainWindowVM: INotifyPropertyChanged
    {
        public string OrdersPath;
        public string ResultsPath;
        public static string DoorModelsPath = @"..\..\templates\Doors";
        public static string KeyHoleModelsPath = @"..\..\templates\KeyHoles";
        public static string[] DoorModels { get; set; }
        public static string[] KeyHoleModels { get; set; }


        private string doorType;
        public string DoorType
        {
            get { return doorType; }
            set
            {
                var temp = string.Concat(DoorModelsPath, "\\", value);
                if (temp != doorType)
                {
                    doorType = temp;
                    OnPropertyChanged("DoorType");
                }
            }
        }

        private string keyType1;
        public string KeyType1
        {
            get { return keyType1; }
            set
            {
                var temp = string.Concat(KeyHoleModelsPath, "\\", value);
                if (temp != keyType1)
                {
                    keyType1 = temp;
                    OnPropertyChanged("KeyType1");
                }
            }
        }

        private string keyType2;
        public string KeyType2
        {
            get { return keyType2; }
            set
            {
                var temp = string.Concat(KeyHoleModelsPath, "\\", value);
                if (temp != keyType2)
                {
                    keyType2 = temp;
                    OnPropertyChanged("KeyType2");
                }
            }
        }

        public class CommandHandler : ICommand
        {
            private Action _action;
            private Func<bool> _canExecute;
            /// <summary>
            /// Creates instance of the command handler
            /// </summary>
            /// <param name="action">Action to be executed by the command</param>
            /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
            public CommandHandler(Action action, Func<bool> canExecute)
            {
                _action = action;
                _canExecute = canExecute;
            }

            /// <summary>
            /// Wires CanExecuteChanged event 
            /// </summary>
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Forcess checking if execute is allowed
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public bool CanExecute(object parameter)
            {
                return _canExecute.Invoke();
            }

            public void Execute(object parameter)
            {
                _action();
            }
        }

        public string[] GetFilesFromDirectory(string directory) 
        {
            return Directory.GetFiles(directory);
        }

        public MainWindowVM()
        {
            DoorModels = GetFoldersFromDirectory(DoorModelsPath).Select(x => x.Split('\\').Last()).ToArray();
            KeyHoleModels = GetFoldersFromDirectory(KeyHoleModelsPath).Select(x => x.Split('\\').Last()).ToArray();
        }

        private ICommand _startProgrammButton;
        public ICommand StartProgrammButton
        {
            get
            {
                return _startProgrammButton ?? (_startProgrammButton = new CommandHandler(() => MakeCut(), () => CanExecute));
            }
        }

        private ICommand _browseOrdersFolderButton;
        public ICommand BrowseOrdersFolderButton
        {
            get
            {
                return _browseOrdersFolderButton ?? (_browseOrdersFolderButton = new CommandHandler(() => BrowseFolder(ref OrdersPath), () => CanExecute));
            }
        }

        private ICommand _browseResultsFolderButton;
        public ICommand BrowseResultsFolderButton
        {
            get
            {
                return _browseResultsFolderButton ?? (_browseResultsFolderButton = new CommandHandler(() => BrowseFolder(ref ResultsPath), () => CanExecute));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _width;
        public string Width
        {
            get { return _width; }
            set
            {
                if (value != _width)
                {
                    if (!new Regex("[^0-9.-]+").IsMatch(value))
                    {
                        _width = value;
                        OnPropertyChanged("Width");
                    }
                    else
                    {
                        InputData_Error();
                    }
                }
            }
        }

        private string _height;
        public string Height
        {
            get { return _height; }
            set
            {
                if (value != _height)
                {
                    if(!new Regex("[^0-9.]").IsMatch(value))
                    {
                        _height = value;
                        OnPropertyChanged("Height");
                    }
                    else
                    {
                        InputData_Error();
                    }
                }
            }
        }

        private void InputData_Error()
        {
            MessageBox.Show("вводить только положительные числа или числа с точкой");
        }

        public bool CanExecute
        {
            get
            {
                // check if executing is allowed, i.e., validate, check if a process is running, etc. 
                return true;
            }
        }

        public void MakeCut()
        {
            if (OrdersPath == null || ResultsPath == null)
            {
                throw new Exception("не указан путь к папке");
            }

            var orderFiles = GetFilesFromDirectory(OrdersPath);
            for (int i=0; i< orderFiles.Length; i++)
            {
                string file = string.Format($"{ResultsPath}\\createdFile{i}.dxf");
                DxfDocument ourFile = DxfDocument.Load(orderFiles[i]);
                if (ourFile == null)
                {
                    throw new Exception(orderFiles[i] + " File is not loaded, incorrect format");
                }

                double width = 880;
                double height = 2050;
                ChangeSize(ourFile, width,height);
                ourFile.Save(file);

                DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out _);
                if (dxfVersion < DxfVersion.AutoCad2000)
                    throw new Exception("you are using an old AutoCad Version");
            }
        }

        private void ChangeSize(DxfDocument ourFile, double width, double height)
        {
            double newWidth = width - 860;
            double newHeigh = height - 2050; // переменная не используется? 
            foreach (var x in ourFile.Lines)
            {
                if (x.Color.B == 255)
                    x.EndPoint = x.EndPoint + new Vector3(newWidth, 0, 0);
                if (x.Color.B == 155)
                {
                    x.StartPoint = x.StartPoint + new Vector3(newWidth, 0, 0);
                    x.EndPoint = x.EndPoint + new Vector3(newWidth, 0, 0);
                }
            }
            foreach (var x in ourFile.Circles)
            {
                if (x.Color.B == 255)
                    x.Center = x.Center + new Vector3(newWidth, 0, 0);
            }
        }

        public void BrowseFolder(ref string path)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
            {
                path = folderBrowser.SelectedPath;
            }
        }

        public string[] GetFoldersFromDirectory(string directory)
        {
            return Directory.GetDirectories(directory);
        }
    }
}
