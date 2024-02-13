namespace Calculate_MauiDevExpress_1._0
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            if (!Preferences.Default.ContainsKey("Theme"))
            {
                Preferences.Default.Set("Theme", "White");
            }
        }
        protected override void OnStart()
        {
            if (Preferences.Default.ContainsKey("Theme"))
            {
                if (Preferences.Default.Get("Theme", "White") == "Dark")
                {
                    ThemeSet.SetDarkTheme(MainPage);
                }
            }
        }
    }
}