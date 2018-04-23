using PlaylistsNET.Content;
using PlaylistsNET.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Notifications;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace vMixListMaker
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Item> Playlist = new ObservableCollection<Item>();
        public MainViewModel MainViewModel = new MainViewModel();
        //public TimeSpan StartTime { get; set; } = new TimeSpan(17, 45, 00);
        private string[] AllowedMediaTypes = {
            "video/avi",
            "video/mp4",
            "video/x-matroska",
            "video/quicktime",
        };

        public MainPage()
        {
            InitializeComponent();
            DataContext = Playlist;
        }

        private void RefreshCollectionTimes()
        {
            int iterator = 0;

            foreach (Item item in Playlist)
            {
                if (iterator == 0)
                {
                    item.StartTime = DateTime.Today.Add(MainViewModel.StartingTime);
                }
                else
                {
                    item.StartTime = Playlist[iterator - 1].EndTime;
                }

                item.EndTime = item.StartTime.Add(item.FileDuration);

                iterator++;
            }
        }

        private void PlaylistView_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Agregar al playlist";
            e.DragUIOverride.IsContentVisible = false;
        }

        private async void PlaylistView_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var storageItems = await e.DataView.GetStorageItemsAsync();

                foreach (var storageItem in storageItems)
                {
                    var storageFile = storageItem as StorageFile;
                    System.Diagnostics.Debug.WriteLine(storageFile.ContentType);

                    if (AllowedMediaTypes.Contains(storageFile.ContentType, StringComparer.OrdinalIgnoreCase))
                    {
                        Windows.Storage.FileProperties.VideoProperties videoProperties = await storageFile.Properties.GetVideoPropertiesAsync();
                        TimeSpan duration = new RoundedTimeSpan(videoProperties.Duration.Ticks, 0).TimeSpan;

                        Item item = new Item()
                        {
                            Id = Guid.NewGuid(),
                            FileName = storageFile.DisplayName,
                            FilePath = storageFile.Path,
                            FileDuration = duration
                        };

                        Playlist.Add(item);
                        RefreshCollectionTimes();
                    }
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            while (PlaylistView.SelectedItems.Count > 0)
            {
                Playlist.Remove((Item)PlaylistView.SelectedItem);
                RefreshCollectionTimes();
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistView.SelectedItems.Count > 0)
            {
                foreach (Item item in PlaylistView.SelectedItems)
                {
                    int selectedIndex = PlaylistView.Items.IndexOf(item);

                    if (selectedIndex - 1 >= 0)
                    {
                        Playlist.Move(selectedIndex, selectedIndex - 1);
                        PlaylistView.SelectedIndex = selectedIndex - 1;
                        RefreshCollectionTimes();
                    }
                }
            }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistView.SelectedItems.Count > 0)
            {
                foreach (Item item in PlaylistView.SelectedItems)
                {
                    int selectedIndex = PlaylistView.Items.IndexOf(item);

                    if (selectedIndex + 1 < Playlist.Count)
                    {
                        Playlist.Move(selectedIndex, selectedIndex + 1);
                        PlaylistView.SelectedIndex = selectedIndex + 1;
                        RefreshCollectionTimes();
                    }
                }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            M3uContent playlistContent = new M3uContent();
            M3uPlaylist savePlaylist = new M3uPlaylist
            {
                IsExtended = false
            };

            foreach (Item item in Playlist)
            {
                savePlaylist.PlaylistEntries.Add(new M3uPlaylistEntry() {
                    Path = item.FilePath
                });
            }

            string textPlaylist = playlistContent.Create(savePlaylist);

            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "New Playlist"
            };
            savePicker.FileTypeChoices.Add("M3U Playlist", new List<string>() { ".m3u" });

            StorageFile playlistFile = await savePicker.PickSaveFileAsync();

            if (playlistFile != null)
            {
                CachedFileManager.DeferUpdates(playlistFile);
                await FileIO.WriteTextAsync(playlistFile, textPlaylist);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(playlistFile);

                if (status == FileUpdateStatus.Complete)
                {
                    ToastNotificationManager.CreateToastNotifier().Show(new ToastNotificationBuilder().Build(
                        "vMix List Maker",
                        "Playlist " + playlistFile.Name + " saved successfully!"
                    ));
                }
            }
        }

        private void TimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            RefreshCollectionTimes();
        }
    }
}
