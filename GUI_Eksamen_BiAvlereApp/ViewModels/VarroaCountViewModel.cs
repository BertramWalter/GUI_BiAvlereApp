using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GUI_Eksamen_BiAvlereApp.Classes;

namespace GUI_Eksamen_BiAvlereApp.ViewModels
{
    public class VarroaCountViewModel : BindableBase
    {
        public VarroaCountViewModel(string title, VarroaCount varroaCount)
        {
            Title = title;
            CurrentVarroaCount = varroaCount;
        }

        string title;

        public string Title
        {
            get { return title; }
            set
            {
                SetProperty(ref title, value);
            }
        }

        VarroaCount _curretVarroaCount;

        public VarroaCount CurrentVarroaCount
        {
            get { return _curretVarroaCount; }
            set { SetProperty(ref _curretVarroaCount, value); }
        }

        public bool IsValid
        {
            get
            {
                bool isValid = true;
                if (string.IsNullOrWhiteSpace(CurrentVarroaCount.Name))
                    isValid = false;
                return isValid;
            }
        }

        #region Commands

        ICommand _okBtnCommand;
        public ICommand OkBtnCommand
        {
            get
            {
                return _okBtnCommand ?? (_okBtnCommand = new DelegateCommand(
                               OkBtnCommand_Execute, OkBtnCommand_CanExecute)
                           .ObservesProperty(() => CurrentVarroaCount.Name));
            }
        }

        private void OkBtnCommand_Execute()
        {
            // Nothing needs to be done here
        }

        private bool OkBtnCommand_CanExecute()
        {
            return IsValid;
        }

        #endregion
    }
}
