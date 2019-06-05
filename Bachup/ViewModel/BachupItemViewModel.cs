﻿using Bachup.Helpers;
using Bachup.Model;
using Bachup.View;
using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Bachup.ViewModel
{
    class BachupItemViewModel : BaseViewModel
    {
        public BachupItemViewModel(BachupItem item)
        {
            // Connect Commands On Creations
            AddDestinationCommand = new RelayCommand(AddDestination);
            DeleteDestinationCommand = new RelayCommand(DeleteDestination);
            ShowSourceCommand = new RelayCommand(ShowSource);
            DeleteBachupItemCommand = new RelayCommand(DeleteBachup);
            EditBachupItemCommand = new RelayCommand(EditBachupItem);
            RunBachupCommand = new RelayCommand(RunBachup);
            ShowDestinationCommand = new RelayCommand(ShowDestination);
            RepairSourceCommand = new RelayCommand(RepairSource);
            SaveDataCommand = new RelayCommand(SaveData);


            BachupItem = item;
            EnableDeleteButton = false;

            ValidateSource();
            ShowLastBachup();
            BachupItem.GetSize();
        }

        private BachupItem _bachupItem;
        public BachupItem BachupItem
        {
            get { return _bachupItem; }
            set
            {
                _bachupItem = value;
                NotifyPropertyChanged();
            }
        }


        public string LastBachup
        {
            get
            {
                return String.Format("Last Bachup: {0}", BachupItem.LastBachup);
            }
        }

        public string SizeInMB
        {
            get
            {
                return String.Format("Size: {0} mb", Math.Round(BachupItem.SizeInMB, 2));
            }
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get { return _isSaving; }
            set
            {
                if (_isSaving != value)
                {
                    _isSaving = value;
                    NotifyPropertyChanged();
                }               
            }
        }

        private double _saveProgress;
        public double SaveProgress
        {
            get { return _saveProgress; }
            set
            {
                if (_saveProgress != value)
                {
                    _saveProgress = value;
                    NotifyPropertyChanged();
                }              
            }
        }

        private bool _isSaveComplete;
        public bool IsSaveComplete
        {
            get { return _isSaveComplete; }
            set
            {
                _isSaveComplete = value;
                NotifyPropertyChanged();
            }
        }

        private string _selectedDestination;
        public String SelectedDestination
        {
            get
            {
                return _selectedDestination;
            }
            set
            {
                _selectedDestination = value;
                if (value != null)
                {
                    EnableDeleteButton = true;
                }
                NotifyPropertyChanged();
            }
        }

        private bool _enableDeleteButton;
        public bool EnableDeleteButton
        {
            get
            {
                return _enableDeleteButton;
            }
            set
            {
                _enableDeleteButton = value;
                NotifyPropertyChanged();
            }
        }

        private bool _enableRepairSourceButton;
        public bool EnableRepairSourceButton
        {
            get { return _enableRepairSourceButton; }
            set
            {
                _enableRepairSourceButton = value;
                NotifyPropertyChanged();
            }
        }

        private bool _enableShowSourceButton;
        public bool EnableShowSourceButton
        {
            get { return _enableShowSourceButton; }
            set
            {
                _enableShowSourceButton = value;
                NotifyPropertyChanged();
            }
        }

        private bool _lastBachupVisible;
        public bool LastBachupVisible
        {
            get
            {
                return _lastBachupVisible;
            }
            set
            {
                _lastBachupVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool Sunday
        {
            get
            {
                return BachupItem.AutoWeekdays[Weekdays.Sunday];
            }
            set
            {
                if (BachupItem.AutoWeekdays[Weekdays.Sunday] != value)
                {
                    BachupItem.AutoWeekdays[Weekdays.Sunday] = value;
                    NotifyPropertyChanged();
                }
                
            }
        }

        public bool Monday
        {
            get
            {
                return BachupItem.AutoWeekdays[Weekdays.Monday];
            }
            set
            {
                if (BachupItem.AutoWeekdays[Weekdays.Monday] != value)
                {
                    BachupItem.AutoWeekdays[Weekdays.Monday] = value;
                    NotifyPropertyChanged();
                }

            }
        }

        public bool Tuesday
        {
            get
            {
                return BachupItem.AutoWeekdays[Weekdays.Tuesday];
            }
            set
            {
                if (BachupItem.AutoWeekdays[Weekdays.Tuesday] != value)
                {
                    BachupItem.AutoWeekdays[Weekdays.Tuesday] = value;
                    NotifyPropertyChanged();
                }

            }
        }

        public bool Wednesday
        {
            get
            {
                return BachupItem.AutoWeekdays[Weekdays.Wednesday];
            }
            set
            {
                if (BachupItem.AutoWeekdays[Weekdays.Wednesday] != value)
                {
                    BachupItem.AutoWeekdays[Weekdays.Wednesday] = value;
                    NotifyPropertyChanged();
                }

            }
        }

        public bool Thursday
        {
            get
            {
                return BachupItem.AutoWeekdays[Weekdays.Thursday];
            }
            set
            {
                if (BachupItem.AutoWeekdays[Weekdays.Thursday] != value)
                {
                    BachupItem.AutoWeekdays[Weekdays.Thursday] = value;
                    NotifyPropertyChanged();
                }

            }
        }

        public bool Friday
        {
            get
            {
                return BachupItem.AutoWeekdays[Weekdays.Friday];
            }
            set
            {
                if (BachupItem.AutoWeekdays[Weekdays.Friday] != value)
                {
                    BachupItem.AutoWeekdays[Weekdays.Friday] = value;
                    NotifyPropertyChanged();
                }

            }
        }

        public bool Saturday
        {
            get
            {
                return BachupItem.AutoWeekdays[Weekdays.Saturday];
            }
            set
            {
                if (BachupItem.AutoWeekdays[Weekdays.Saturday] != value)
                { 
                    BachupItem.AutoWeekdays[Weekdays.Saturday] = value;
                    NotifyPropertyChanged();
                }

            }
        }

        // Relay Commands
        public RelayCommand AddDestinationCommand { get; private set; }
        public RelayCommand DeleteDestinationCommand { get; private set; }
        public RelayCommand ShowSourceCommand { get; private set; }
        public RelayCommand DeleteBachupItemCommand { get; private set; }
        public RelayCommand EditBachupItemCommand { get; private set; }
        public RelayCommand RunBachupCommand { get; private set; }
        public RelayCommand ShowDestinationCommand { get; private set; }
        public RelayCommand RepairSourceCommand { get; private set; }
        public RelayCommand SaveDataCommand { get; private set; }

        #region Events

        private async void DeleteDestination(object parameter)
        {
            string message = String.Format("Delete Destination?");
            string submessage = String.Format("{0}", SelectedDestination);

            var view = new ConfirmChoiceView
            {
                DataContext = new ConfirmChoiceViewModel(message, submessage)
            };

            var choice = await DialogHost.Show(view, "RootDialog");

            if ((bool)choice)
            {
                _bachupItem.DeleteDestination(_selectedDestination);
                MainViewModel.SaveData();
                EnableDeleteButton = false;
            }
        }

        private async void AddDestination(object parameter)
        {
            var view = new AddDestinationView
            {
                DataContext = new AddDestinationViewModel(_bachupItem)
            };

            await DialogHost.Show(view, "RootDialog");
        }

        private void ShowSource(object parameter)
        {
            if (_bachupItem.SourceType == BachupItemSourceTypes.Folder)
            {
                if (Directory.Exists(_bachupItem.Source))
                {
                    Process.Start("explorer.exe", _bachupItem.Source);
                }
            }
            else
            {
                if (File.Exists(_bachupItem.Source))
                {
                    Process.Start("explorer.exe", Path.GetDirectoryName(_bachupItem.Source));
                }
            }
                      
        }

        private async void DeleteBachup(object parameter)
        {
            string message = String.Format("Delete {0}", _bachupItem.Name);
            string submessage = String.Format("This is not reversable. You will lose this bachup item and its history. Source files will remain in place.");

            var view = new ConfirmChoiceView
            {
                DataContext = new ConfirmChoiceViewModel(message, submessage)
            };

            var choice = await DialogHost.Show(view, "RootDialog");

            if ((bool)choice == true)
            {
                BachupGroup bg = Bachup.ViewModel.MainViewModel.Bachup.Where(o => o.ID.Equals(_bachupItem.BachupGroupID)).Single();
                bg.RemoveBachupItem(_bachupItem);
                MainViewModel.SaveData();
            }
        }

        private async void EditBachupItem(object parameter)
        {
            var view = new EditBachupItemView
            {
                DataContext = new EditBachupItemViewModel(_bachupItem)
            };

            await DialogHost.Show(view, "RootDialog");
        }

        private void RunBachup(object parameter)
        {
            _bachupItem.RunBachup();
            ShowLastBachup();
            NotifyPropertyChanged("LastBachup");
            ValidateSource();             
        }

        private void ShowDestination(object parameter)
        {
            if (Directory.Exists(_selectedDestination))
            {
                Process.Start("explorer.exe", _selectedDestination);
            }
        }

        private void RepairSource(object parameter)
        {
            BachupItem.RepairSource();
            MainViewModel.SaveData();
            BachupItem.GetSize();
            NotifyPropertyChanged("SizeInMB");
            ValidateSource();
        }

        private void SaveData(object parameter)
        {
            MainViewModel.SaveData();
        }

        #endregion

        #region Methods

        private void ValidateSource()
        {
            EnableRepairSourceButton = !BachupItem.CheckSourceExistence();
            EnableShowSourceButton = BachupItem.CheckSourceExistence();
        }

        private void ShowLastBachup()
        {
            bool result = BachupItem.LastBachup.Year > 2000;
            LastBachupVisible = result;
        }

        #endregion


    }
}
