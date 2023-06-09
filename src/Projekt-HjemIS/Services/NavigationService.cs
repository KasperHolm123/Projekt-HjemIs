﻿using Projekt_HjemIS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Services
{
    public interface INavigationService
    {
        BaseViewModel CurrentView { get; }
        BaseViewModel BaseView { get; }

        void NavigateTo<T>() where T : BaseViewModel;
        void ChangeBaseView<T>() where T : BaseViewModel;
    }

    public class NavigationService : ObservableObject, INavigationService
    {
        private readonly Func<Type, BaseViewModel> _viewModelFactory;

        private BaseViewModel _baseView;
        public BaseViewModel BaseView
        {
            get => _baseView;
            private set
            {
                _baseView = value;
                OnPropertyChanged();
            }
        }

        private BaseViewModel _currentView;
        public BaseViewModel CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }


        public NavigationService(Func<Type, BaseViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
        {
            var viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            CurrentView = viewModel;
        }

        public void ChangeBaseView<TViewModel>() where TViewModel : BaseViewModel
        {
            var viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            BaseView = viewModel;
        }
    }
}
