using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Social_Network.ViewModels
{
    public partial class CreatePostViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool isPhotoTabSelected = true;

        public bool IsNoteTabSelected => !IsPhotoTabSelected;

        // Иконка в шапке: стрелка для фото (переход к описанию), галочка для заметки
        public string HeaderActionIcon => IsPhotoTabSelected ? "→" : "✓";

        public string PhotoTabBackground => IsPhotoTabSelected ? "#C2692A" : "#EDE5D8";
        public string NoteTabBackground => IsNoteTabSelected ? "#C2692A" : "#EDE5D8";
        public string PhotoTabTextColor => IsPhotoTabSelected ? "White" : "#7A5C3E";
        public string NoteTabTextColor => IsNoteTabSelected ? "White" : "#7A5C3E";

        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private string noteText = string.Empty;
        [ObservableProperty] private string? selectedImagePath;
        [ObservableProperty] private ObservableCollection<string> galleryImages = new();

        [RelayCommand]
        private void SelectPhotoTab()
        {
            IsPhotoTabSelected = true;
            NotifyTabChanged();
        }

        [RelayCommand]
        private void SelectNoteTab()
        {
            IsPhotoTabSelected = false;
            NotifyTabChanged();
        }

        private void NotifyTabChanged()
        {
            OnPropertyChanged(nameof(IsNoteTabSelected));
            OnPropertyChanged(nameof(PhotoTabBackground));
            OnPropertyChanged(nameof(NoteTabBackground));
            OnPropertyChanged(nameof(PhotoTabTextColor));
            OnPropertyChanged(nameof(NoteTabTextColor));
            OnPropertyChanged(nameof(HeaderActionIcon));
        }

        [RelayCommand]
        private async Task PickImage()
        {
            var result = await MediaPicker.PickPhotoAsync();
            if (result != null)
                SelectedImagePath = result.FullPath;
        }

        [RelayCommand]
        private async Task Publish()
        {
            // Здесь вызов API
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand] private void AddHashtag() { }
        [RelayCommand] private void TagPeople() { }
        [RelayCommand] private void PickAlbum() { }
        [RelayCommand] private void HeaderAction() { }
        [RelayCommand] private void SelectGalleryImage(string path) => SelectedImagePath = path;
    }
}
