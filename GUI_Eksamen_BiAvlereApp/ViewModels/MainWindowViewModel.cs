using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GUI_Eksamen_BiAvlereApp.Classes;
using GUI_Eksamen_BiAvlereApp.Data;
using GUI_Eksamen_BiAvlereApp.Views;
using Microsoft.Win32;


namespace GUI_Eksamen_BiAvlereApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly string AppTitle = "BeeHives and varroa counts";
        private string filePath = "";
        private ObservableCollection<VarroaCount> _varroaCounts;

        public MainWindowViewModel()
        {
            _varroaCounts = new ObservableCollection<VarroaCount>
            {
                new VarroaCount("BeeHive one", DateTime.Now, 5, "Not alot of mites"),
                new VarroaCount("BeeHive two", DateTime.Now, 30, "Alot of mites this day!"),
            };


            CurrentVarroaCount = null;
        }

        #region Search textbox
        //---------------------------FILTER BEEHIVES-------------------------//

        private string _beehiveName = string.Empty;
        public string BeeHiveName
        {
            get { return _beehiveName; }
            set
            {
                SetProperty(ref _beehiveName, value);
                ICollectionView cv = CollectionViewSource.GetDefaultView(VarroaCounts);
                cv.Filter += CollectionViewSource_Filter;
            }
        }

        private bool CollectionViewSource_Filter(object vc)
        {
            VarroaCount varcount = vc as VarroaCount;
            if (!string.IsNullOrEmpty(BeeHiveName))
            {
                return varcount.Name.Contains(BeeHiveName);
            }
            return varcount.Name.Contains("");
        }
        #endregion

        #region Properties for varroaCounts





        public ObservableCollection<VarroaCount> VarroaCounts
        {
            get { return _varroaCounts; }
            set { SetProperty(ref _varroaCounts, value); }
        }

        VarroaCount currentVarroaCount = null;
        public VarroaCount CurrentVarroaCount
        {
            get { return currentVarroaCount; }
            set { SetProperty(ref currentVarroaCount, value); }
        }


        int currentIndex = -1;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set { SetProperty(ref currentIndex, value); }
        }

        #endregion

        #region DirtySetup and filename

        private string filename = "";
        public string Filename
        {
            get { return filename; }
            set
            {
                SetProperty(ref filename, value);
                RaisePropertyChanged("Title");
            }
        }
        public string Title
        {
            get
            {
                var s = "";
                if (Dirty)
                    s = "*";
                return Filename + s + "" + AppTitle;
            }
        }

        private bool dirty = false;
        public bool Dirty
        {
            get { return dirty; }
            set
            {
                SetProperty(ref dirty, value);
                RaisePropertyChanged("Title");
            }
        }


        #endregion

        #region File commands

        #region newFileCommand

        ICommand _newCommand;
        public ICommand AddNewVarroaCountCommand
        {
            get
            {
                return _newCommand ?? (_newCommand = new DelegateCommand(() =>
                {
                    var newVarroaCount = new VarroaCount();
                    var vm = new VarroaCountViewModel("Add new varroa count", newVarroaCount)
                    {
                    };
                    var dlg = new VarroaCountView
                    {
                        DataContext = vm
                    };
                    if (dlg.ShowDialog() == true)
                    {
                        VarroaCounts.Add(newVarroaCount);
                        CurrentVarroaCount = newVarroaCount;
                        Dirty = true;
                    }
                }));
            }
        }

        ICommand _NewFileCommand;
        public ICommand NewFileCommand
        {
            get { return _NewFileCommand ?? (_NewFileCommand = new DelegateCommand(NewFileCommand_Execute)); }
        }

        private void NewFileCommand_Execute()
        {
            MessageBoxResult res = MessageBox.Show("Any unsaved data will be lost. Are you sure you want to initiate a new file?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                VarroaCounts.Clear();
                Filename = "";
                Dirty = false;
            }
        }

        #endregion

        #region OpenFileCommand


        ICommand _OpenFileCommand;
        public ICommand OpenFileCommand
        {
            get { return _OpenFileCommand ?? (_OpenFileCommand = new DelegateCommand<string>(OpenFileCommand_Execute)); }
        }

        private void OpenFileCommand_Execute(string argFilename)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Varroacounts documents|*.bees|All Files|*.*",
                DefaultExt = "bees"
            };
            if (filePath == "")
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                dialog.InitialDirectory = Path.GetDirectoryName(filePath);

            if (dialog.ShowDialog(App.Current.MainWindow) == true)
            {
                filePath = dialog.FileName;
                Filename = Path.GetFileName(filePath);
                try
                {
                    Repository.ReadFile(filePath, out ObservableCollection<VarroaCount> tempVarroaCounts);
                    VarroaCounts = tempVarroaCounts;
                    Dirty = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to open file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region SaveFileAsCommand

        ICommand _SaveAsCommand;
        public ICommand SaveAsCommand
        {
            get { return _SaveAsCommand ?? (_SaveAsCommand = new DelegateCommand<string>(SaveAsCommand_Execute)); }
        }

        private void SaveAsCommand_Execute(string argFilename)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Varroacounts documents|*.bees|All Files|*.*",
                DefaultExt = "bees"
            };
            if (filePath == "")
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                dialog.InitialDirectory = Path.GetDirectoryName(filePath);

            if (dialog.ShowDialog(App.Current.MainWindow) == true)
            {
                filePath = dialog.FileName;
                Filename = Path.GetFileName(filePath);
                SaveFile();
            }
        }


        #endregion

        #region SaveFileCommand

        ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _SaveCommand ?? (_SaveCommand = new DelegateCommand(SaveFileCommand_Execute, SaveFileCommand_CanExecute)
                           .ObservesProperty(() => VarroaCounts.Count));
            }
        }

        private void SaveFileCommand_Execute()
        {
            SaveFile();
        }

        private bool SaveFileCommand_CanExecute()
        {
            return (filename != "") && (VarroaCounts.Count > 0);
        }

        private void SaveFile()
        {
            try
            {
                Repository.SaveFile(filePath, VarroaCounts);
                Dirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to save file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region DeleteFileCommand

        ICommand _deleteCommand;
        public ICommand DeleteVarroaCountCommand => _deleteCommand ?? (_deleteCommand =
                                                  new DelegateCommand(DeleteVarroaCount, DeleteVarroaCount_CanExecute)
                                                      .ObservesProperty(() => CurrentIndex));

        private void DeleteVarroaCount()
        {
            MessageBoxResult res = MessageBox.Show("Are you sure you want to delete varroacount " + CurrentVarroaCount.Name +
                                                   "?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                VarroaCounts.Remove(CurrentVarroaCount);
                Dirty = true;
            }
        }

        private bool DeleteVarroaCount_CanExecute()
        {
            if (VarroaCounts.Count > 0 && CurrentIndex >= 0)
                return true;
            else
                return false;
        }

        #endregion

        #region EditFileCommand
        ICommand _editCommand;
        public ICommand EditVarroaCountCommand
        {
            get
            {
                return _editCommand ?? (_editCommand = new DelegateCommand(() =>
                   {
                       var tempVarroaCount = CurrentVarroaCount.Clone();
                       var vm = new VarroaCountViewModel("Edit Varroa count", tempVarroaCount)
                       {
                       };
                       var dlg = new VarroaCountView()
                       {
                           DataContext = vm,
                           Owner = App.Current.MainWindow
                       };
                       if (dlg.ShowDialog() == true)
                       {
                           // Copy values back
                           CurrentVarroaCount.Name = tempVarroaCount.Name;
                           CurrentVarroaCount.Datetime = tempVarroaCount.Datetime;
                           CurrentVarroaCount.NumberOfMites = tempVarroaCount.NumberOfMites;
                           CurrentVarroaCount.Comment = tempVarroaCount.Comment;
                           Dirty = true;
                       }
                   },
                   () => {
                       return CurrentIndex >= 0;
                   }
               ).ObservesProperty(() => CurrentIndex));
            }
        }


        #endregion

        #region DeleteFileCommand

        
        ICommand _closeAppCommand;
        public ICommand CloseAppCommand
        {
            get
            {
                return _closeAppCommand ?? (_closeAppCommand = new DelegateCommand(() =>
                {
                    Application.Current.MainWindow.Close();
                }));
            }
        }

        ICommand _closingCommand;
        public ICommand ClosingCommand
        {
            get
            {
                return _closingCommand ?? (_closingCommand = new
                           DelegateCommand<CancelEventArgs>(ClosingCommand_Execute));
            }
        }

        private void ClosingCommand_Execute(CancelEventArgs arg)
        {
            if (Dirty)
                arg.Cancel = UserRegrets();
        }

        private bool UserRegrets()
        {
            var regret = false;
            MessageBoxResult res = MessageBox.Show("You have unsaved data. Are you sure you want to close the application without saving data first?",
                "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.No)
            {
                regret = true;
            }
            return regret;
        }
        #endregion

        #endregion

        #region ViewDrawingCommand

        ICommand _newDrawingCommand;
        public ICommand NewDrawingCommand
        {
            get
            {
                return _newDrawingCommand ?? (_newDrawingCommand = new DelegateCommand(() =>
                {
                    var dlg = new DrawView();
                    if (dlg.ShowDialog() == true)
                    {
                        Dirty = true;
                    }
                }));
            }
        }

        #endregion
    }
}
