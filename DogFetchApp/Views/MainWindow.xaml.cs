using ApiHelper;
using ApiHelper.Models;
using DogFetchApp.Commands;
using DogFetchApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace DogFetchApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
    #region Variables ---------------------------------------------------------------------------
        
        MainViewModel currentViewmodel;

    #endregion

    # region Constructeur ------------------------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();
            ApiHelper.ApiHelper.InitializeClient();
            currentViewmodel = new MainViewModel();
            DataContext = currentViewmodel;
            
        }
        #endregion

        
    }
}
