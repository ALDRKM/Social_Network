namespace Social_Network.Controls
{
    public partial class AvatarView : ContentView
    {
        public static readonly BindableProperty SizeProperty =
            BindableProperty.Create(nameof(Size), typeof(double), typeof(AvatarView), 48d,
                propertyChanged: OnSizeChanged);

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(string), typeof(AvatarView), null,
                propertyChanged: OnSourceChanged);

        public static readonly BindableProperty IsSystemProperty =
            BindableProperty.Create(nameof(IsSystem), typeof(bool), typeof(AvatarView), false,
                propertyChanged: OnIsSystemChanged);

        public AvatarView()
        {
            InitializeComponent();
            ApplySize();
            ApplyAppearance();
        }

        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        // Url аватарки; если пусто — показываем силуэт-заглушку
        public string? Source
        {
            get => (string?)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        // Системный чат: иконка компьютера на фоне цвета меню навигации
        public bool IsSystem
        {
            get => (bool)GetValue(IsSystemProperty);
            set => SetValue(IsSystemProperty, value);
        }

        private static void OnSizeChanged(BindableObject bindable, object oldValue, object newValue)
            => ((AvatarView)bindable).ApplySize();

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
            => ((AvatarView)bindable).ApplyAppearance();

        private static void OnIsSystemChanged(BindableObject bindable, object oldValue, object newValue)
            => ((AvatarView)bindable).ApplyAppearance();

        private void ApplySize()
        {
            Holder.WidthRequest = Size;
            Holder.HeightRequest = Size;
            WidthRequest = Size;
            HeightRequest = Size;
            Clip.Center = new Point(Size / 2, Size / 2);
            Clip.RadiusX = Size / 2;
            Clip.RadiusY = Size / 2;
        }

        private void ApplyAppearance()
        {
            if (IsSystem)
            {
                BackgroundCircle.BackgroundColor = (Color)Application.Current!.Resources["AppNavAccent"];
                Avatar.IsVisible = false;
                Avatar.Source = null;
                Silhouette.IsVisible = false;
                ComputerIcon.IsVisible = true;
                return;
            }

            BackgroundCircle.BackgroundColor = (Color)Application.Current!.Resources["AppBrown"];
            ComputerIcon.IsVisible = false;

            var url = Source;
            if (string.IsNullOrWhiteSpace(url))
            {
                Avatar.IsVisible = false;
                Avatar.Source = null;
                Silhouette.IsVisible = true;
            }
            else
            {
                Avatar.Source = Helpers.MediaHelper.Resolve(url);
                Avatar.IsVisible = true;
                Silhouette.IsVisible = false;
            }
        }
    }
}
