﻿using Bachup.Helpers;
using Bachup.Model;
using Bachup.Model.BachupItems;
using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Bachup.ViewModel
{
    class AddBachupItemViewModel : BaseViewModel
    {
        public AddBachupItemViewModel(BachupGroup bachupGroup)
        {
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(Cancel);
            AddSourceFileCommand = new RelayCommand(AddSourceFile);
            AddSourceFolderCommand = new RelayCommand(AddSourceFolder);


            _bachupGroup = bachupGroup;
            ShowMessage = false;
        }

        private readonly BachupGroup _bachupGroup;

        // Properties (Bindings)

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        private string _source;
        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                NotifyPropertyChanged();
            }
        }

        private bool _showMessage;
        public bool ShowMessage
        {
            get
            {
                return _showMessage;
            }
            set
            {
                _showMessage = value;
                NotifyPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        private bool _zipBachupItem;
        public bool ZipBachupItem
        {
            get { return _zipBachupItem; }
            set
            {
                _zipBachupItem = value;
                NotifyPropertyChanged();
            }
        }

        private bool _useFileNameForBachup;
        public bool UseFileNameForBachup
        {
            get
            {
                return _useFileNameForBachup;
            }
            set
            {
                if (_useFileNameForBachup != value)
                {
                    _useFileNameForBachup = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _useGroupDestinations;
        public bool UseGroupDestinations
        {
            get { return _useGroupDestinations; }
            set
            {
                if (_useGroupDestinations != value)
                {
                    _useGroupDestinations = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Relay Commands
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand AddSourceFileCommand { get; private set; }
        public RelayCommand AddSourceFolderCommand { get; private set; }

        #region Events

        private void Add(object o)
        {

            if (CheckRequirements())
            {
                BachupType bachupType = GetBachupItemType();
                if (bachupType == BachupType.NotSupported)
                {
                    Message = "Unsupported Bachup Type";
                    ShowMessage = true;
                }


                switch (bachupType)
                {
                    case BachupType.GDB:
                        _bachupGroup.AddBachupItem(new BI_Geodatabase(Name, Source, _bachupGroup.ID)
                        {
                            ZipBachup = _zipBachupItem,
                            UseFileNameForBachup = _useFileNameForBachup,
                            UseGroupDestinations = _useGroupDestinations
                        });       
                        break;
                    case BachupType.TXT:
                        _bachupGroup.AddBachupItem(new BI_Text(Name, Source, _bachupGroup.ID)
                        {
                            ZipBachup = _zipBachupItem,
                            UseFileNameForBachup = _useFileNameForBachup,
                            UseGroupDestinations = _useGroupDestinations
                        });
                        break;
                    case BachupType.LAS:
                        _bachupGroup.AddBachupItem(new BI_LAS(Name, Source, _bachupGroup.ID)
                            {
                            ZipBachup = _zipBachupItem,
                            UseFileNameForBachup = _useFileNameForBachup,
                            UseGroupDestinations = _useGroupDestinations
                        });
                        break;
                    case BachupType.SHP:
                        _bachupGroup.AddBachupItem(new BI_Shapefile(Name, Source, _bachupGroup.ID)
                        {
                            ZipBachup = _zipBachupItem,
                            UseFileNameForBachup = _useFileNameForBachup,
                            UseGroupDestinations = _useGroupDestinations
                        });
                        break;
                    case BachupType.DIR:
                        _bachupGroup.AddBachupItem(new BI_DIR(Name, Source, _bachupGroup.ID)
                        {
                            ZipBachup = _zipBachupItem,
                            UseFileNameForBachup = _useFileNameForBachup,
                            UseGroupDestinations = _useGroupDestinations
                        });
                        break;
                    case BachupType.NotSupported:
                        return;                       
                }
                DialogHost.CloseDialogCommand.Execute(null, null);
            }               
        }

        private void Cancel(object o)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void AddSourceFile(object o)
        {
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "All Bachup Items|*.txt;*.las;*.shp|LAS|*.las|TXT|*.txt|SHP|*.shp";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    Source = openFileDialog.FileName;
                }
            }
        }

        private void AddSourceFolder(object o)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            if (Directory.Exists(Source))
                dialog.DefaultDirectory = Source;

            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Source = dialog.FileName;
            }
        }

        #endregion

        #region Methods

        private bool CheckRequirements()
        {
            if (String.IsNullOrEmpty(Name))
            {
                Message = "Enter A Name";
                ShowMessage = true;
                return false;
            }

            if (_bachupGroup.DoesItemExist(Name))
            {
                Message = "Item With That Name Exists";
                ShowMessage = true;
                return false;
            }

            if (!Directory.Exists(Source) ^ File.Exists(Source))
            {
                Message = "Source Path Does Not Exist";
                ShowMessage = true;
                return false;
            }

            if (String.IsNullOrEmpty(Source))
            {
                Message = "Enter A Source Path";
                ShowMessage = true;
                return false;
            }

            ShowMessage = false;
            return true;
        }

        private BachupType GetBachupItemType()
        {
            FileAttributes attr = File.GetAttributes(Source);
            string extension = Path.GetExtension(Source).Replace(".", "");

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory && extension != "gdb")
                return BachupType.DIR;
            
            switch (extension.ToLower())
            {
                case "gdb":
                    return BachupType.GDB;
                case "txt":
                    return BachupType.TXT;
                case "las":
                    return BachupType.LAS;
                case "shp":
                    return BachupType.SHP;
                default:
                    return BachupType.NotSupported;
            }
        }

        #endregion
    }
}
