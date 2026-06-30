using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Constants;
using Social_Network.Core.Models;
using Social_Network.Helpers;
using Social_Network.Service;

namespace Social_Network.ViewModels
{
    public partial class CreatePostViewModel : BaseViewModel
    {
        private readonly IPostService _post;
        private readonly IUserService _user;
        private readonly IImageUploadService _upload;

        public CreatePostViewModel(IPostService post, IUserService user, IImageUploadService upload)
        {
            _post = post;
            _user = user;
            _upload = upload;

            // Сводка под описанием обновляется при изменении наборов
            SelectedTags.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(HasSelectedTags));
                OnPropertyChanged(nameof(HasTagsAndMentions));
            };
            SelectedMentions.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(HasSelectedMentions));
                OnPropertyChanged(nameof(HasTagsAndMentions));
            };
        }

        // Сводка выбранного (под описанием, видна после «Готово»)
        public bool HasSelectedTags => SelectedTags.Count > 0;
        public bool HasSelectedMentions => SelectedMentions.Count > 0;
        public bool HasTagsAndMentions => HasSelectedTags && HasSelectedMentions;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNoteTabSelected))]
        [NotifyPropertyChangedFor(nameof(HeaderTitle))]
        [NotifyPropertyChangedFor(nameof(PhotoTabBackground))]
        [NotifyPropertyChangedFor(nameof(NoteTabBackground))]
        [NotifyPropertyChangedFor(nameof(PhotoTabTextColor))]
        [NotifyPropertyChangedFor(nameof(NoteTabTextColor))]
        private bool isPhotoTabSelected = true;

        public bool IsNoteTabSelected => !IsPhotoTabSelected;
        public string HeaderTitle => IsPhotoTabSelected ? "Новая публикация" : "Новая заметка";

        // Выбранная — чуть темнее, невыбранная — как на скрине (тан)
        public string PhotoTabBackground => IsPhotoTabSelected ? "#8B4E1C" : "#D9C6A5";
        public string NoteTabBackground => IsNoteTabSelected ? "#8B4E1C" : "#D9C6A5";
        public string PhotoTabTextColor => IsPhotoTabSelected ? "White" : "#7A5C3E";
        public string NoteTabTextColor => IsNoteTabSelected ? "White" : "#7A5C3E";

        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private string noteText = string.Empty;
        [ObservableProperty] private string? errorMessage;
        [ObservableProperty] private bool isUploading;

        // Одно фото (заменяется при повторном выборе)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasImage))]
        private string? selectedImage;

        public bool HasImage => !string.IsNullOrEmpty(SelectedImage);

        // ----- Панели хэштегов / отметок -----
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HashtagButtonText))]
        private bool showHashtagPanel;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MentionButtonText))]
        private bool showMentionPanel;

        public string HashtagButtonText => ShowHashtagPanel ? "Готово" : "Добавить хэштег";
        public string MentionButtonText => ShowMentionPanel ? "Готово" : "Отметить";

        [RelayCommand]
        private void ToggleHashtagPanel()
        {
            ShowHashtagPanel = !ShowHashtagPanel;
            if (ShowHashtagPanel) ShowMentionPanel = false;
        }

        [RelayCommand]
        private void ToggleMentionPanel()
        {
            ShowMentionPanel = !ShowMentionPanel;
            if (ShowMentionPanel) ShowHashtagPanel = false;
        }

        // ----- Хэштеги -----
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasTagSuggestions))]
        private string tagQuery = string.Empty;

        public ObservableCollection<string> TagSuggestions { get; } = new();
        public ObservableCollection<string> SelectedTags { get; } = new();
        public bool HasTagSuggestions => TagSuggestions.Count > 0;

        partial void OnTagQueryChanged(string value) => _ = SearchTagsAsync();

        private async Task SearchTagsAsync()
        {
            var q = TagQuery?.Trim().TrimStart('#') ?? string.Empty;
            TagSuggestions.Clear();
            if (string.IsNullOrEmpty(q)) { OnPropertyChanged(nameof(HasTagSuggestions)); return; }

            var tags = await _post.SearchTagNamesAsync(q);
            foreach (var t in tags.Where(t => SelectedTags.All(s => s != t)))
                TagSuggestions.Add(t);
            OnPropertyChanged(nameof(HasTagSuggestions));
        }

        [RelayCommand]
        private void AddTag(string? tag)
        {
            var raw = tag ?? TagQuery;
            if (!ValidationHelper.IsValidTag(raw))
            {
                ErrorMessage = "Тег: без пробелов и решётки, например «лето» или «море»";
                return;
            }
            var t = raw.Trim().TrimStart('#').ToLowerInvariant();
            if (SelectedTags.All(s => s != t))
                SelectedTags.Add(t);
            TagQuery = string.Empty;
            TagSuggestions.Clear();
            ErrorMessage = null;
            OnPropertyChanged(nameof(HasTagSuggestions));
        }

        [RelayCommand]
        private void RemoveTag(string tag) => SelectedTags.Remove(tag);

        // ----- Отметка людей (только существующие) -----
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasMentionSuggestions))]
        private string mentionQuery = string.Empty;

        public ObservableCollection<User> MentionSuggestions { get; } = new();
        public ObservableCollection<User> SelectedMentions { get; } = new();
        public bool HasMentionSuggestions => MentionSuggestions.Count > 0;

        partial void OnMentionQueryChanged(string value) => _ = SearchMentionsAsync();

        private async Task SearchMentionsAsync()
        {
            var q = MentionQuery?.Trim().TrimStart('@') ?? string.Empty;
            MentionSuggestions.Clear();
            if (string.IsNullOrEmpty(q)) { OnPropertyChanged(nameof(HasMentionSuggestions)); return; }

            int myId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var users = await _user.SearchUsersAsync(q);
            foreach (var u in users.Where(u => u.Id != myId && SelectedMentions.All(s => s.Id != u.Id)))
                MentionSuggestions.Add(u);
            OnPropertyChanged(nameof(HasMentionSuggestions));
        }

        // Отметить можно только пользователя из списка (т.е. существующего)
        [RelayCommand]
        private void AddMention(User user)
        {
            if (user == null) return;
            if (SelectedMentions.All(s => s.Id != user.Id))
                SelectedMentions.Add(user);
            MentionQuery = string.Empty;
            MentionSuggestions.Clear();
            ErrorMessage = null;
            OnPropertyChanged(nameof(HasMentionSuggestions));
        }

        [RelayCommand]
        private void RemoveMention(User user)
        {
            if (user != null) SelectedMentions.Remove(user);
        }

        // ----- Вкладки и фото -----
        [RelayCommand]
        private void SelectPhotoTab() => IsPhotoTabSelected = true;

        [RelayCommand]
        private void SelectNoteTab() => IsPhotoTabSelected = false;

        // Выбрать одно фото (повторный выбор заменяет предыдущее)
        [RelayCommand]
        private async Task PickImages()
        {
            try
            {
                var file = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Выберите фото",
                    FileTypes = FilePickerFileType.Images
                });
                if (file == null) return;

                IsUploading = true;
                ErrorMessage = null;
                using var stream = await file.OpenReadAsync();
                var url = await _upload.UploadAsync(stream, file.FileName);
                IsUploading = false;

                if (!string.IsNullOrEmpty(url)) SelectedImage = url;
                else ErrorMessage = "Не удалось загрузить фото";
            }
            catch (Exception ex)
            {
                IsUploading = false;
                ErrorMessage = "Не удалось загрузить фото: " + ex.Message;
            }
        }

        [RelayCommand]
        private void RemoveImage() => SelectedImage = null;

        [RelayCommand]
        private async Task Publish()
        {
            if (IsBusy) return;
            if (IsUploading)
            {
                ErrorMessage = "Дождитесь загрузки фото";
                return;
            }
            ErrorMessage = null;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var type = IsPhotoTabSelected ? PostType.Photo : PostType.Note;
            var content = (IsPhotoTabSelected ? Description : NoteText)?.Trim() ?? string.Empty;

            if (type == PostType.Photo && !HasImage)
            {
                ErrorMessage = "Выберите фото";
                return;
            }
            if (type == PostType.Note && string.IsNullOrWhiteSpace(content) && SelectedTags.Count == 0)
            {
                ErrorMessage = "Введите текст заметки или добавьте теги";
                return;
            }

            IsBusy = true;

            var tags = SelectedTags.Distinct().ToList();
            var mentionIds = SelectedMentions.Select(u => u.Id).Distinct().ToList();
            var image = IsPhotoTabSelected ? SelectedImage : null;

            var created = await _post.CreatePostAsync(userId, content, type, image, tags, mentionIds);
            IsBusy = false;

            if (created == null)
            {
                ErrorMessage = "Не удалось опубликовать. Проверьте соединение с сервером.";
                return;
            }

            Reset();
            await Shell.Current.GoToAsync("//FeedPage");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            Reset();
            await Shell.Current.GoToAsync("//FeedPage");
        }

        private void Reset()
        {
            Description = string.Empty;
            NoteText = string.Empty;
            TagQuery = string.Empty;
            MentionQuery = string.Empty;
            SelectedImage = null;
            SelectedTags.Clear();
            SelectedMentions.Clear();
            TagSuggestions.Clear();
            MentionSuggestions.Clear();
            ShowHashtagPanel = false;
            ShowMentionPanel = false;
            ErrorMessage = null;
            IsPhotoTabSelected = true;
        }
    }
}
