using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Constants;
using Social_Network.Core.Models;
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
        }

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

        public string PhotoTabBackground => IsPhotoTabSelected ? "#C8702A" : "#EDE5D8";
        public string NoteTabBackground => IsNoteTabSelected ? "#C8702A" : "#EDE5D8";
        public string PhotoTabTextColor => IsPhotoTabSelected ? "White" : "#7A5C3E";
        public string NoteTabTextColor => IsNoteTabSelected ? "White" : "#7A5C3E";

        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private string noteText = string.Empty;
        [ObservableProperty] private string? errorMessage;
        [ObservableProperty] private bool isUploading;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasImages))]
        private ObservableCollection<string> selectedImages = new();

        public bool HasImages => SelectedImages.Count > 0;

        // ----- Панели хэштегов / отметок -----
        [ObservableProperty] private bool showHashtagPanel;
        [ObservableProperty] private bool showMentionPanel;

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

        // ----- Хэштеги (динамический поиск существующих) -----
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
            var t = (tag ?? TagQuery).Trim().TrimStart('#').ToLowerInvariant();
            if (!string.IsNullOrEmpty(t) && SelectedTags.All(s => s != t))
                SelectedTags.Add(t);
            TagQuery = string.Empty;
            TagSuggestions.Clear();
            OnPropertyChanged(nameof(HasTagSuggestions));
        }

        [RelayCommand]
        private void RemoveTag(string tag) => SelectedTags.Remove(tag);

        // ----- Отметка людей (динамический поиск) -----
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

        [RelayCommand]
        private void AddMention(User user)
        {
            if (user == null) return;
            if (SelectedMentions.All(s => s.Id != user.Id))
                SelectedMentions.Add(user);
            MentionQuery = string.Empty;
            MentionSuggestions.Clear();
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

        [RelayCommand]
        private async Task PickImages()
        {
            try
            {
                var results = await FilePicker.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "Выберите фото",
                    FileTypes = FilePickerFileType.Images
                });
                if (results == null) return;

                IsUploading = true;
                ErrorMessage = null;
                foreach (var file in results)
                {
                    using var stream = await file.OpenReadAsync();
                    var url = await _upload.UploadAsync(stream, file.FileName);
                    if (!string.IsNullOrEmpty(url) && !SelectedImages.Contains(url))
                        SelectedImages.Add(url);
                }
                IsUploading = false;
                OnPropertyChanged(nameof(HasImages));
            }
            catch (Exception ex)
            {
                IsUploading = false;
                ErrorMessage = "Не удалось загрузить фото: " + ex.Message;
            }
        }

        [RelayCommand]
        private void RemoveImage(string path)
        {
            SelectedImages.Remove(path);
            OnPropertyChanged(nameof(HasImages));
        }

        [RelayCommand]
        private async Task Publish()
        {
            if (IsBusy) return;
            ErrorMessage = null;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var type = IsPhotoTabSelected ? PostType.Photo : PostType.Note;
            var content = IsPhotoTabSelected ? Description : NoteText;

            if (type == PostType.Photo && !HasImages)
            {
                ErrorMessage = "Выберите хотя бы одно фото";
                return;
            }
            if (type == PostType.Note && string.IsNullOrWhiteSpace(content))
            {
                ErrorMessage = "Введите текст заметки";
                return;
            }

            IsBusy = true;

            var tags = SelectedTags.ToList();
            if (!string.IsNullOrWhiteSpace(TagQuery))
                tags.AddRange(ParseTags(TagQuery));
            tags = tags.Distinct().ToList();

            var mentionIds = SelectedMentions.Select(u => u.Id).Distinct().ToList();
            var images = IsPhotoTabSelected ? SelectedImages.ToList() : null;

            var created = await _post.CreatePostAsync(userId, content, type, images, tags, mentionIds);
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
            SelectedImages.Clear();
            SelectedTags.Clear();
            SelectedMentions.Clear();
            TagSuggestions.Clear();
            MentionSuggestions.Clear();
            ShowHashtagPanel = false;
            ShowMentionPanel = false;
            IsPhotoTabSelected = true;
        }

        private static List<string> ParseTags(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new();
            return text.Split(new[] { ' ', ',', '#', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(t => t.Trim().ToLowerInvariant())
                       .Where(t => t.Length > 0)
                       .Distinct()
                       .ToList();
        }
    }
}
