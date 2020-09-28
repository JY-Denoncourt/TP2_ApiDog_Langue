using ApiHelper;
using DogFetchApp.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DogFetchApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Variables ---------------------------------------------------------------------------
        private int choiceNb = 0;
        private int nbDog = 0;
        private int totChoice = 0;
        private string selectedBreed;
        private string selectedNb;
        private bool lessChoiceThanWanted;

        private ObservableCollection<string> breedsList = new ObservableCollection<string>();
        private ObservableCollection<string> nbDogWantedList;
        private ObservableCollection<string> dogImageList = new ObservableCollection<string>();
        private ObservableCollection<string> currentImages = new ObservableCollection<string>();

        private string language;
        #endregion

        #region IAsyncCommand------------------------------------------------------------------------
        public AsyncCommand<string> LoadDogsCommand { get; private set; }
        public AsyncCommand<string> NextLoadDogsCommand { get; private set; }
        public AsyncCommand<string> BackLoadDogsCommand { get; private set; }


        public DelegateCommand<string> ChangeLangageDelegate { get; private set; }
        #endregion

        #region Defenitions--------------------------------------------------------------------------
        public ObservableCollection<string> BreedsList
        {
            get => breedsList;
            set
            {
                breedsList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> NbDogWantedList
        {
            get => nbDogWantedList;
            set
            {
                nbDogWantedList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> DogImageList
        {
            get => dogImageList;
            set
            {
                dogImageList = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> CurrentImages
        {
            get => currentImages;
            set
            {
                currentImages = value;
                OnPropertyChanged();
            }
        }

        public string SelectedBreed
        {
            get => selectedBreed;
            set
            {
                selectedBreed = value;
                OnPropertyChanged();
                LoadDogsCommand.RaiseCanExecuteChanged();
            }
        }
        public string SelectedNb
        {
            get => selectedNb;
            set
            {
                selectedNb = value;
                OnPropertyChanged();
                LoadDogsCommand.RaiseCanExecuteChanged();
            }
        }
        public int ChoiceNb
        {
            get => choiceNb;
            set
            {
                choiceNb = value;
                OnPropertyChanged();
                NextLoadDogsCommand.RaiseCanExecuteChanged();
                BackLoadDogsCommand.RaiseCanExecuteChanged();
            }
        }
        public int NbDog
        {
            get => nbDog;
            set
            {
                nbDog = value;
                OnPropertyChanged();
            }
        }
        public bool LessChoiceThanWanted
        {
            get => lessChoiceThanWanted;
            set
            {
                lessChoiceThanWanted = value;
                OnPropertyChanged();
                NextLoadDogsCommand.RaiseCanExecuteChanged();
            }
        }


        public string Language {
            get => language;
            set
            {
                language = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructeur ------------------------------------------------------------------------
        public MainViewModel()
        {
            //Command-------------------------------
            LoadDogsCommand = new AsyncCommand<string>(LoadImages, CanExecuteLoadImage);
            NextLoadDogsCommand = new AsyncCommand<string>(NextLoadImages, CanExecuteNextLoadImage);
            BackLoadDogsCommand = new AsyncCommand<string>(BackLoadImages, CanExecuteBackLoadImage);

            ChangeLangageDelegate = new DelegateCommand<string>(ChangeLanguage);

            loadBreeds();
            initDogWanted();
        }

        #endregion

        #region Methodes ----------------------------------------------------------------------------

        #region (OK) Autres methodes utilitaires
        //(OK) Methode pour mettre des nombre dans le combobox nbImage
        private void initDogWanted()
        {
            NbDogWantedList = new ObservableCollection<string> { "1", "3", "5", "10" };
        }



        //(OK) --> OC des Breeds en requete sur le web DogApi
        private async void loadBreeds()
        {
            List<string> list = await DogApiProcessor.LoadBreedList();
            foreach(string item in list)
            {
                breedsList.Add(item);
            }
        }

        #endregion

        #region (OK) Aller chercher nb image de dog sur le web
        private async Task LoadImages(string T)
        {
            NbDog = Int32.Parse(SelectedNb);
            var dogImg = await DogApiProcessor.GetImageUrl(SelectedBreed, NbDog);


            CurrentImages.Clear();
            if (dogImg.message.Count < NbDog)       //Cas ou il y a moins Img que ce que user veut
            {
                nbDog = dogImg.message.Count;
                LessChoiceThanWanted = true;
            }
            else LessChoiceThanWanted = false;

            if(T != "Next")                        //Cas ou le user refait Fetch et non Next
            {
                DogImageList.Clear();
                ChoiceNb = 0;
                totChoice = 0;
            }

            for (int i = 0; i < NbDog; i++)
            {
                DogImageList.Add(dogImg.message[i]);
                CurrentImages.Add(dogImg.message[i]);
            }
            ChoiceNb++;
            totChoice++;
        }

        private bool CanExecuteLoadImage(string T)
        {
            bool CanFetch;
            if ((SelectedBreed != null) && (SelectedNb != null)) CanFetch = true;
            else CanFetch = false;

            return CanFetch;  
        }
        #endregion

        #region (OK) Sert a refaire un fetch des memes choix du fetch avant
        private async Task NextLoadImages(string T)
        {
            
            if (ChoiceNb == totChoice)        //Si revenu au bout DogImageList, donc demande nouvelle requete web
            {
                await LoadImages("Next");
                System.Diagnostics.Debug.WriteLine("\nNew requet web"+ totChoice);
            }
            else                              //Si pas encore revenu au bout DogImageList, donc reaffiche ce qui est dans list
            {
                CurrentImages.Clear();
                ChoiceNb++;
                for (int i = ((ChoiceNb - 1) * NbDog); i < (ChoiceNb * NbDog); i++)
                {
                    CurrentImages.Add(DogImageList[i]);
                }
                
            }

        }

        private bool CanExecuteNextLoadImage(string T)
        {
            bool CanNext;
            if ( (!LessChoiceThanWanted) && (ChoiceNb > 0)) CanNext = true;
            else CanNext = false;

            return CanNext;
        }
        #endregion

        #region (OK) Back sur les image loader
        private async Task BackLoadImages(string T)
        {
            CurrentImages.Clear();
            
            for (int i = ((ChoiceNb-2)*NbDog); i < ((ChoiceNb-1) * NbDog); i++)
            {
                CurrentImages.Add(DogImageList[i]);
            }

            ChoiceNb--;
        }

        private bool CanExecuteBackLoadImage(string T)
        {
            bool CanBack = true;
            if (ChoiceNb < 2) CanBack = false;

            return CanBack;
        }
        #endregion

        #region (--) Sert a faire changement de langue app et redemarrer
        private void ChangeLanguage(string param)
        {
            MessageBoxResult result = MessageBox.Show($"{ Properties.Resources.Msg_Restart}", "My App", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Properties.Settings.Default.Language = param;
                    Properties.Settings.Default.Save();


                   
                    
                    var filename = Application.ResourceAssembly.Location;
                    var newFile = Path.ChangeExtension(filename, ".exe");
                    Process.Start(newFile);
                    Application.Current.Shutdown();




                    //Restart();
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show($"{ Properties.Resources.Msg_RestartLater}");
                    break;
                case MessageBoxResult.Cancel:
                    MessageBox.Show($"{ Properties.Resources.Msg_Nevermind}");
                    break;
            }
        }


        void Restart()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
        #endregion

        #endregion

    }

}
