using Calculate_MauiDevExpress_1._0.Class;
using DevExpress.Maui.Editors;
using System.Data;
using System.Text.RegularExpressions;

namespace Calculate_MauiDevExpress_1._0
{
    public partial class MainPage : ContentPage
    {
        private Timer? timer;
        private decimal CourseCruptoActive;

        private volatile bool AnimationPlusAndMinusWhile = true;
        public MainPage()
        {
            InitializeComponent();
            _ = InitializeCrypto();
            _ = CourceUsdAndEur();
            timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

            GridPopup.WidthRequest = (DeviceDisplay.MainDisplayInfo.Width / 3) * 0.8;
            GridPopup.HeightRequest = (DeviceDisplay.MainDisplayInfo.Height / 3) * 0.4;


            if (Preferences.Default.ContainsKey("Theme"))
            {
                if (Preferences.Default.Get("Theme", "White") == "White")
                {
                    MainContent.BackgroundColor = Color.Parse("#fff");
                    BtnTheme.Source = "moontheme.svg";
                }
                else if (Preferences.Default.Get("Theme", "White") == "Dark")
                {
                    MainContent.BackgroundColor = Color.Parse("#1C274C");
                    BtnTheme.Source = "suntheme.svg";
                }
            }
        }
        private async Task CourceUsdAndEur()
        {
            CourseUsd.Text = $"$ {await Crypto.GetUsdToRubAsync("USD")}";
            CourseEur.Text = $"€ {await Crypto.GetUsdToRubAsync("EUR")}";
        }
        protected override bool OnBackButtonPressed()
        {
            if (SquareGrid.IsVisible)
            {
                SquareGrid.IsVisible = false;
                TransformGrid.IsVisible = true;
            }
            else if (DataGrid.IsVisible)
            {
                DataGrid.IsVisible = false;
                TransformGrid.IsVisible = true;
            }
            else if (LenghtGrid.IsVisible)
            {
                LenghtGrid.IsVisible = false;
                TransformGrid.IsVisible = true;
            }
            else if (WeightGrid.IsVisible)
            {
                WeightGrid.IsVisible = false;
                TransformGrid.IsVisible = true;
            }
            else if (NumberSGrid.IsVisible)
            {
                NumberSGrid.IsVisible = false;
                TransformGrid.IsVisible = true;
            }
            else if (SpeedGrid.IsVisible)
            {
                SpeedGrid.IsVisible = false;
                TransformGrid.IsVisible = true;
            }
            else if (TransformGrid.IsVisible || CryptoRate.IsVisible)
            {
                TransformGrid.IsVisible = false;
                CryptoRate.IsVisible = false;
                StartGrid.IsVisible = true;
            }

            return true;
        }

        private void TimerCallback(object state)
        {
            if (CryptoLayout2.Children.Count <= 2)
            {
                LoadCrypto.Rotation += 5;
            }
            else
            {
                timer?.Dispose();
            }

        }

        private async Task InitializeCrypto()
        {
            List<string> CryptoList = new List<string>() { "BTCUSDT", "ETHUSDT", "BNBUSDT", "TRXUSDT", "TONUSDT", "DOGEUSDT", "MATICUSDT", "LTCUSDT" };

            try
            {

                List<Frame> frames = new();
                foreach (string CryptoS in CryptoList)
                {
                    var result = await Crypto.GetCryptoPriceMarket(CryptoS.Replace("USDT", ""));
                    frames.Add(await Crypto.Card(CryptoS.Replace("USDT", " / USDT"), result.priceUSD.ToString().Replace(",", "."), result.percentChange24h.ToString().Replace(",", ".")));

                }
                double scrollX = ScrollCrypto.ScrollY;
                CryptoLayout2.Children.Clear();
                foreach (var frame in frames)
                {
                    CryptoLayout2.Children.Add(frame);
                }
                await ScrollCrypto.ScrollToAsync(0, scrollX, true);
            }
            catch
            {
                if (CryptoLayout2.Children.Count <= 2)
                    CryptoLayout2.Add(new Label { Text = "ошибка соединения", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.End, TextColor = Colors.Grey });
            }
            foreach (var frame in CryptoLayout2.Children)
            {
                if (frame is Frame frm)
                {
                    frm.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command((obj) => ClickFrameCrypto(obj as Frame)),
                        CommandParameter = frm
                    });
                    if (frm.Content is Microsoft.Maui.Controls.Grid child)
                    {
                        foreach (var lbl in child.Children)
                        {
                            if (lbl is Label lb)
                            {
                                lb.InputTransparent = true;
                            }
                            else if (lbl is Frame ff)
                            {
                                ff.InputTransparent = true;
                            }
                        }

                    }
                }
            }
        }
        private void ClickFrameCrypto(object obj)
        {
            if (obj is Frame frm)
            {
                PopupCrypto.IsOpen = true;
                NameCryptoConverter.Text = frm.AutomationId.Split("/")[0];
                CryptoConverterCoin.PlaceholderText = frm.AutomationId.Split("/")[0];
                CourseCruptoActive = Convert.ToDecimal(frm.AutomationId.Split("/")[1].Replace(".", ","));
            }
            
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.Text == "÷" || btn.Text == "×" || btn.Text == "—" || btn.Text == "+")
                {
                    if (TextOfTask.Text.Length >= 1)
                    {
                        if (TextOfTask.Text[TextOfTask.Text.Length - 1] == ' ')
                        {
                            TextOfTask.Text = TextOfTask.Text[..^3] + (btn.Text == "—" ? $" - " : $" {btn.Text} ");
                        }
                        else TextOfTask.Text += (btn.Text == "—" ? $" - " : $" {btn.Text} ");
                    }
                }
                else TextOfTask.Text += btn.Text;

            }

            ResultText.Text = $"{EvaluateExpression(TextOfTask.Text)}";

        }

        string EvaluateExpression(string expression)
        {
            expression = expression.Replace(",", ".").Replace("÷", "/").Replace("×", "*");

            DataTable dt = new DataTable();
            try
            {
                var result = dt.Compute(expression, "");
                return Convert.ToDouble(result).ToString();
            }
            catch
            {
                return ResultText.Text;
            }
        }

        private void Click_Del(object sender, EventArgs e)
        {
            if (TextOfTask.Text != "")
            {
                if (TextOfTask.Text[TextOfTask.Text.Length - 1].ToString() == " ")
                {
                    TextOfTask.Text = TextOfTask.Text[..^3];
                }
                else TextOfTask.Text = TextOfTask.Text[..^1];
            }
            ResultText.Text = $"{EvaluateExpression(TextOfTask.Text)}";

        }

        [Obsolete]
        private void Get_transf_Grid(object sender, EventArgs e)
        {
            TransformGrid.IsVisible = true;
            if (StartGrid.IsVisible)
            {
                //await StartGrid.TranslateTo(-StartGrid.Width, 0, 300, Easing.Linear);
                StartGrid.IsVisible = false;
                //StartGrid.TranslationX += StartGrid.Width;

            }
            else if (CryptoRate.IsVisible)
            {
                //await CryptoRate.TranslateTo(CryptoRate.Width, 0, 300, Easing.Linear);
                CryptoRate.IsVisible = false;
                //CryptoRate.TranslationX -= CryptoRate.Width;
            }
        }

        private void Get_Calculate_Grid(object sender, EventArgs e)
        {
            StartGrid.IsVisible = true;
            if (TransformGrid.IsVisible)
            {
                //await TransformGrid.TranslateTo(TransformGrid.Width, 0, 300, Easing.Linear);
                TransformGrid.IsVisible = false;
                //TransformGrid.TranslationX -= TransformGrid.Width;
            }
            else if (CryptoRate.IsVisible)
            {
                //await CryptoRate.TranslateTo(CryptoRate.Width, 0, 300, Easing.Linear);
                CryptoRate.IsVisible = false;
                //CryptoRate.TranslationX -= CryptoRate.Width;
            }
        }

        private void Get_Crypto_Grid(object sender, EventArgs e)
        {
            CryptoRate.IsVisible = true;
            if (StartGrid.IsVisible)
            {
                //await StartGrid.TranslateTo(-StartGrid.Width, 0, 300, Easing.Linear);
                StartGrid.IsVisible = false;
                //StartGrid.TranslationX += StartGrid.Width;

            }
            else if (TransformGrid.IsVisible)
            {
                //await TransformGrid.TranslateTo(-TransformGrid.Width, 0, 300, Easing.Linear);
                TransformGrid.IsVisible = false;
                //TransformGrid.TranslationX += TransformGrid.Width;
            }
        }

        private void Click_Image(object sender, EventArgs e)
        {
            TransformGrid.IsVisible = false;
            SquareGrid.IsVisible = true;
        }

        private void Click_Swap(object sender, TappedEventArgs e)
        {
            if (sender is Microsoft.Maui.Controls.Image img)
            {
                if (img.Rotation == 180) img.RotateTo(360, 400);
                else img.RotateTo(180, 400);
            }
            string s = ConvertOne.Text;
            ConvertOne.Text = ConvertTwo.Text;
            ConvertTwo.Text = s;
            TextResultSquare.Text = ConvertX.ConvertSquare(ConvertOne.Text, ConvertTwo.Text, TextCountSquare.Text);
        }

        private void Click_Back_square(object sender, EventArgs e)
        {
            SquareGrid.IsVisible = false;
            TransformGrid.IsVisible = true;
        }
        private void Click_DelAll(object sender, EventArgs e)
        {
            TextOfTask.Text = "";
            ResultText.Text = "";
        }

        private void Click_Number(object sender, EventArgs e)
        {
            if (TextCountSquare.Text.Length <= 12)
            {
                if (sender is Button btn)
                {
                    TextCountSquare.Text += btn.Text;
                }
            }
            TextResultSquare.Text = ConvertX.ConvertSquare(ConvertOne.Text, ConvertTwo.Text, TextCountSquare.Text);
        }
        private void Click_Del_Square(object sender, EventArgs e)
        {
            if (TextCountSquare.Text != "")
            {
                TextCountSquare.Text = TextCountSquare.Text[..^1];
            }
            TextResultSquare.Text = ConvertX.ConvertSquare(ConvertOne.Text, ConvertTwo.Text, TextCountSquare.Text);
        }
        private void Click_DelAll_Square(object sender, EventArgs e)
        {
            TextCountSquare.Text = "";
            TextResultSquare.Text = "";
        }

        private async void Click_Parameter(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertOne.Text = (match.Success ? match.Groups[1].Value : string.Empty).ToLower().Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertType, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertType.IsVisible = false;
            TextResultSquare.Text = ConvertX.ConvertSquare(ConvertOne.Text, ConvertTwo.Text, TextCountSquare.Text);
        }
        private async void Click_ParameterTwo(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertTwo.Text = (match.Success ? match.Groups[1].Value : string.Empty).ToLower().Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeTwo, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeTwo.IsVisible = false;
            TextResultSquare.Text = ConvertX.ConvertSquare(ConvertOne.Text, ConvertTwo.Text, TextCountSquare.Text);
        }

        private async void Click_Open_Type(object sender, EventArgs e)
        {
            if (FrameConvertType.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertType, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertType.IsVisible = false;
            }
            else if (!FrameConvertTypeTwo.IsVisible && !FrameConvertType.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertType, 400, true, 300, this);
                FrameConvertType.IsVisible = true;
            }
        }
        private async void Click_Open_TypeTwo(object sender, EventArgs e)
        {
            if (FrameConvertTypeTwo.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwo, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeTwo.IsVisible = false;
            }
            else if (!FrameConvertTypeTwo.IsVisible && !FrameConvertType.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwo, 400, true, 300, this);
                FrameConvertTypeTwo.IsVisible = true;
            }
        }
        private async void Click_Copy(object sender, EventArgs e)
        {
            if (TextOfTask.Text.Length > 0 && TextOfTask.Text != "...")
            {
                await Clipboard.SetTextAsync(ResultText.Text);
                FrameToCopy.IsVisible = true;
                await FrameToCopy.FadeTo(1, 200);
                await Task.Delay(500);
                await FrameToCopy.FadeTo(0, 200);
            }
        }

        //ДАННЫЕ

        //переход с главной на конмертацию данных
        private void Click_Image_Data(object sender, EventArgs e)
        {
            TransformGrid.IsVisible = false;
            DataGrid.IsVisible = true;
        }

        //кнопка назад
        private void Click_Back_Data(object sender, EventArgs e)
        {
            DataGrid.IsVisible = false;
            TransformGrid.IsVisible = true;
        }

        //открыть панель для выбора конвертации тип данных
        private async void Click_Open_Type_Data(object sender, EventArgs e)
        {
            if (FrameConvertTypeData.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeData, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeData.IsVisible = false;
            }
            else if (!FrameConvertTypeTwoData.IsVisible && !FrameConvertTypeData.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeData, 400, true, 300, this);
                FrameConvertTypeData.IsVisible = true;
            }
        }

        //кнопка поменять местами типы данных
        private void Click_Swap_Data(object sender, TappedEventArgs e)
        {
            if (sender is Microsoft.Maui.Controls.Image img)
            {
                if (img.Rotation == 180) img.RotateTo(360, 400);
                else img.RotateTo(180, 400);
            }
            string s = ConvertOneData.Text;
            ConvertOneData.Text = ConvertTwoData.Text;
            ConvertTwoData.Text = s;
            TextResultData.Text = ConvertX.ConvertData(ConvertOneData.Text.ToLower(), ConvertTwoData.Text.ToLower(), TextCountData.Text);
        }

        //вторая открыть панель для выбора конвертации тип данных
        private async void Click_Open_TypeTwo_Data(object sender, EventArgs e)
        {
            if (FrameConvertTypeTwoData.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwoData, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeTwoData.IsVisible = false;
            }
            else if (!FrameConvertTypeTwoData.IsVisible && !FrameConvertTypeData.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwoData, 400, true, 300, this);
                FrameConvertTypeTwoData.IsVisible = true;
            }
        }

        //обработка ввода цифр
        private void Click_Number_Data(object sender, EventArgs e)
        {
            if (TextCountData.Text.Length <= 12)
            {
                if (sender is Button btn)
                {
                    TextCountData.Text += btn.Text;
                }
            }
            TextResultData.Text = ConvertX.ConvertData(ConvertOneData.Text.ToLower(), ConvertTwoData.Text.ToLower(), TextCountData.Text);
        }

        //удалить последний символ строки
        private void Click_Del_Data(object sender, EventArgs e)
        {
            if (TextCountData.Text != "")
            {
                TextCountData.Text = TextCountData.Text[..^1];
            }
            TextResultData.Text = ConvertX.ConvertData(ConvertOneData.Text.ToLower(), ConvertTwoData.Text.ToLower(), TextCountData.Text);
        }

        //удалить все символы строк
        private void Click_DelAll_Data(object sender, EventArgs e)
        {
            TextCountData.Text = "";
            TextResultData.Text = "";
        }

        //выбор типа данных первый и второй
        private async void Click_Parameter_Data(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertOneData.Text = (match.Success ? match.Groups[1].Value : string.Empty).Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeData, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeData.IsVisible = false;
            TextResultData.Text = ConvertX.ConvertData(ConvertOneData.Text.ToLower(), ConvertTwoData.Text.ToLower(), TextCountData.Text);
        }
        private async void Click_ParameterTwo_Data(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertTwoData.Text = (match.Success ? match.Groups[1].Value : string.Empty).Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeTwoData, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeTwoData.IsVisible = false;
            TextResultData.Text = ConvertX.ConvertData(ConvertOneData.Text.ToLower(), ConvertTwoData.Text.ToLower(), TextCountData.Text);
        }

        //ДЛИНА

        //переход с главной на конмертацию данных
        private void Click_Image_Lenght(object sender, EventArgs e)
        {
            TransformGrid.IsVisible = false;
            LenghtGrid.IsVisible = true;
        }

        //кнопка назад
        private void Click_Back_Lenght(object sender, EventArgs e)
        {
            LenghtGrid.IsVisible = false;
            TransformGrid.IsVisible = true;
        }

        //открыть панель для выбора конвертации тип данных
        private async void Click_Open_Type_Lenght(object sender, EventArgs e)
        {
            if (FrameConvertTypeLenght.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeLenght, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeLenght.IsVisible = false;
            }
            else if (!FrameConvertTypeTwoLenght.IsVisible && !FrameConvertTypeLenght.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeLenght, 400, true, 300, this);
                FrameConvertTypeLenght.IsVisible = true;
            }
        }

        //кнопка поменять местами типы данных
        private void Click_Swap_Lenght(object sender, TappedEventArgs e)
        {
            if (sender is Microsoft.Maui.Controls.Image img)
            {
                if (img.Rotation == 180) img.RotateTo(360, 400);
                else img.RotateTo(180, 400);
            }
            string s = ConvertOneLenght.Text;
            ConvertOneLenght.Text = ConvertTwoLenght.Text;
            ConvertTwoLenght.Text = s;
            TextResultLenght.Text = ConvertX.ConvertLenght(ConvertOneLenght.Text.ToLower(), ConvertTwoLenght.Text.ToLower(), TextCountLenght.Text);
        }

        //вторая открыть панель для выбора конвертации тип данных
        private async void Click_Open_TypeTwo_Lenght(object sender, EventArgs e)
        {
            if (FrameConvertTypeTwoLenght.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwoLenght, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeTwoLenght.IsVisible = false;
            }
            else if (!FrameConvertTypeTwoLenght.IsVisible && !FrameConvertTypeLenght.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwoLenght, 400, true, 300, this);
                FrameConvertTypeTwoLenght.IsVisible = true;
            }
        }

        //обработка ввода цифр
        private void Click_Number_Lenght(object sender, EventArgs e)
        {
            if (TextCountLenght.Text.Length <= 12)
            {
                if (sender is Button btn)
                {
                    TextCountLenght.Text += btn.Text;
                }
            }
            TextResultLenght.Text = ConvertX.ConvertLenght(ConvertOneLenght.Text.ToLower(), ConvertTwoLenght.Text.ToLower(), TextCountLenght.Text);
        }

        //удалить последний символ строки
        private void Click_Del_Lenght(object sender, EventArgs e)
        {
            if (TextCountLenght.Text != "")
            {
                TextCountLenght.Text = TextCountLenght.Text[..^1];
            }
            TextResultLenght.Text = ConvertX.ConvertLenght(ConvertOneLenght.Text.ToLower(), ConvertTwoLenght.Text.ToLower(), TextCountLenght.Text);
        }

        //удалить все символы строк
        private void Click_DelAll_Lenght(object sender, EventArgs e)
        {
            TextCountLenght.Text = "";
            TextResultLenght.Text = "";
        }

        //выбор типа данных первый и второй
        private async void Click_Parameter_Lenght(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertOneLenght.Text = (match.Success ? match.Groups[1].Value : string.Empty).Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeLenght, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeLenght.IsVisible = false;
            TextResultLenght.Text = ConvertX.ConvertLenght(ConvertOneLenght.Text.ToLower(), ConvertTwoLenght.Text.ToLower(), TextCountLenght.Text);
        }
        private async void Click_ParameterTwo_Lenght(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertTwoLenght.Text = (match.Success ? match.Groups[1].Value : string.Empty).Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeTwoLenght, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeTwoLenght.IsVisible = false;
            TextResultLenght.Text = ConvertX.ConvertLenght(ConvertOneLenght.Text.ToLower(), ConvertTwoLenght.Text.ToLower(), TextCountLenght.Text);
        }

        //МАССА

        //переход с главной на конмертацию данных
        private void Click_Image_Weight(object sender, EventArgs e)
        {
            TransformGrid.IsVisible = false;
            WeightGrid.IsVisible = true;
        }

        //кнопка назад
        private void Click_Back_Weight(object sender, EventArgs e)
        {
            WeightGrid.IsVisible = false;
            TransformGrid.IsVisible = true;
        }

        //открыть панель для выбора конвертации тип данных
        private async void Click_Open_Type_Weight(object sender, EventArgs e)
        {
            if (FrameConvertTypeWeight.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeWeight, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeWeight.IsVisible = false;
            }
            else if (!FrameConvertTypeTwoWeight.IsVisible && !FrameConvertTypeWeight.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeWeight, 400, true, 300, this);
                FrameConvertTypeWeight.IsVisible = true;
            }
        }

        //кнопка поменять местами типы данных
        private void Click_Swap_Weight(object sender, TappedEventArgs e)
        {
            if (sender is Microsoft.Maui.Controls.Image img)
            {
                if (img.Rotation == 180) img.RotateTo(360, 400);
                else img.RotateTo(180, 400);
            }
            string s = ConvertOneWeight.Text;
            ConvertOneWeight.Text = ConvertTwoWeight.Text;
            ConvertTwoWeight.Text = s;
            TextResultWeight.Text = ConvertX.ConvertWeight(ConvertOneWeight.Text.ToLower(), ConvertTwoWeight.Text.ToLower(), TextCountWeight.Text);
        }

        //вторая открыть панель для выбора конвертации тип данных
        private async void Click_Open_TypeTwo_Weight(object sender, EventArgs e)
        {
            if (FrameConvertTypeTwoWeight.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwoWeight, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeTwoWeight.IsVisible = false;
            }
            else if (!FrameConvertTypeTwoWeight.IsVisible && !FrameConvertTypeWeight.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwoWeight, 400, true, 300, this);
                FrameConvertTypeTwoWeight.IsVisible = true;
            }
        }

        //обработка ввода цифр
        private void Click_Number_Weight(object sender, EventArgs e)
        {
            if (TextCountWeight.Text.Length <= 12)
            {
                if (sender is Button btn)
                {
                    TextCountWeight.Text += btn.Text;
                }
            }
            TextResultWeight.Text = ConvertX.ConvertWeight(ConvertOneWeight.Text.ToLower(), ConvertTwoWeight.Text.ToLower(), TextCountWeight.Text);
        }

        //удалить последний символ строки
        private void Click_Del_Weight(object sender, EventArgs e)
        {
            if (TextCountWeight.Text != "")
            {
                TextCountWeight.Text = TextCountWeight.Text[..^1];
            }
            TextResultWeight.Text = ConvertX.ConvertWeight(ConvertOneWeight.Text.ToLower(), ConvertTwoWeight.Text.ToLower(), TextCountWeight.Text);
        }

        //удалить все символы строк
        private void Click_DelAll_Weight(object sender, EventArgs e)
        {
            TextCountWeight.Text = "";
            TextResultWeight.Text = "";
        }

        //выбор типа данных первый и второй
        private async void Click_Parameter_Weight(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertOneWeight.Text = (match.Success ? match.Groups[1].Value : string.Empty).Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeWeight, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeWeight.IsVisible = false;
            TextResultWeight.Text = ConvertX.ConvertWeight(ConvertOneWeight.Text.ToLower(), ConvertTwoWeight.Text.ToLower(), TextCountWeight.Text);
        }
        private async void Click_ParameterTwo_Weight(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertTwoWeight.Text = (match.Success ? match.Groups[1].Value : string.Empty).Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeTwoWeight, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeTwoWeight.IsVisible = false;
            TextResultWeight.Text = ConvertX.ConvertWeight(ConvertOneWeight.Text.ToLower(), ConvertTwoWeight.Text.ToLower(), TextCountWeight.Text);
        }

        //СИСТЕМЫ СЧИСЛЕНИЯ

        //переход с главной на конмертацию данных
        private void Click_Image_NumberS(object sender, EventArgs e)
        {
            TransformGrid.IsVisible = false;
            NumberSGrid.IsVisible = true;
        }

        //кнопка назад
        private void Click_Back_NumberS(object sender, EventArgs e)
        {
            NumberSGrid.IsVisible = false;
            TransformGrid.IsVisible = true;
        }

        //открыть панель для выбора конвертации тип данных
        private async void Click_Open_Type_NumberS(object sender, EventArgs e)
        {
            if (FrameConvertTypeNumberS.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeNumberS, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeNumberS.IsVisible = false;
            }
            else if (!FrameConvertTypeTwoNumberS.IsVisible && !FrameConvertTypeNumberS.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeNumberS, 400, true, 300, this);
                FrameConvertTypeNumberS.IsVisible = true;
            }
        }

        //кнопка поменять местами типы данных
        private void Click_Swap_NumberS(object sender, TappedEventArgs e)
        {
            if (sender is Microsoft.Maui.Controls.Image img)
            {
                if (img.Rotation == 180) img.RotateTo(360, 400);
                else img.RotateTo(180, 400);
            }
            string s = ConvertOneNumberS.Text;
            ConvertOneNumberS.Text = ConvertTwoNumberS.Text;
            ConvertTwoNumberS.Text = s;
            TextResultNumberS.Text = ConvertX.ConvertNumberS(TextCountNumberS.Text, Convert.ToInt32(ConvertOneNumberS.Text), Convert.ToInt32(ConvertTwoNumberS.Text));
        }

        //вторая открыть панель для выбора конвертации тип данных
        private async void Click_Open_TypeTwo_NumberS(object sender, EventArgs e)
        {
            if (FrameConvertTypeTwoNumberS.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwoNumberS, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeTwoNumberS.IsVisible = false;
            }
            else if (!FrameConvertTypeTwoNumberS.IsVisible && !FrameConvertTypeNumberS.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwoNumberS, 400, true, 300, this);
                FrameConvertTypeTwoNumberS.IsVisible = true;
            }
        }

        //обработка ввода цифр
        private void Click_Number_NumberS(object sender, EventArgs e)
        {
            if (TextCountNumberS.Text.Length <= 12)
            {
                if (sender is Button btn)
                {
                    TextCountNumberS.Text += btn.Text;
                }
            }
            TextResultNumberS.Text = ConvertX.ConvertNumberS(TextCountNumberS.Text, Convert.ToInt32(ConvertOneNumberS.Text), Convert.ToInt32(ConvertTwoNumberS.Text));
        }

        //удалить последний символ строки
        private void Click_Del_NumberS(object sender, EventArgs e)
        {
            if (TextCountNumberS.Text != "")
            {
                TextCountNumberS.Text = TextCountNumberS.Text[..^1];
            }
            TextResultNumberS.Text = ConvertX.ConvertNumberS(TextCountNumberS.Text, Convert.ToInt32(ConvertOneNumberS.Text), Convert.ToInt32(ConvertTwoNumberS.Text));
        }

        //удалить все символы строк
        private void Click_DelAll_NumberS(object sender, EventArgs e)
        {
            TextCountNumberS.Text = "";
            TextResultNumberS.Text = "";
        }

        //выбор типа данных первый и второй
        private async void Click_Parameter_NumberS(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertOneNumberS.Text = (match.Success ? match.Groups[1].Value : string.Empty).Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeNumberS, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeNumberS.IsVisible = false;
            TextResultNumberS.Text = ConvertX.ConvertNumberS(TextCountNumberS.Text, Convert.ToInt32(ConvertOneNumberS.Text), Convert.ToInt32(ConvertTwoNumberS.Text));
        }
        private async void Click_ParameterTwo_NumberS(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertTwoNumberS.Text = (match.Success ? match.Groups[1].Value : string.Empty).Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeTwoNumberS, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeTwoNumberS.IsVisible = false;
            TextResultNumberS.Text = ConvertX.ConvertNumberS(TextCountNumberS.Text, Convert.ToInt32(ConvertOneNumberS.Text), Convert.ToInt32(ConvertTwoNumberS.Text));
        }

        //СИСТЕМЫ СЧИСЛЕНИЯ

        //переход с главной на конмертацию данных
        private void Click_Image_Speed(object sender, EventArgs e)
        {
            TransformGrid.IsVisible = false;
            SpeedGrid.IsVisible = true;
        }

        //кнопка назад
        private void Click_Back_Speed(object sender, EventArgs e)
        {
            SpeedGrid.IsVisible = false;
            TransformGrid.IsVisible = true;
        }

        //открыть панель для выбора конвертации тип данных
        private async void Click_Open_Type_Speed(object sender, EventArgs e)
        {
            if (FrameConvertTypeSpeed.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeSpeed, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeSpeed.IsVisible = false;
            }
            else if (!FrameConvertTypeTwoSpeed.IsVisible && !FrameConvertTypeSpeed.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeSpeed, 400, true, 300, this);
                FrameConvertTypeSpeed.IsVisible = true;
            }
        }

        //кнопка поменять местами типы данных
        private void Click_Swap_Speed(object sender, TappedEventArgs e)
        {
            if (sender is Microsoft.Maui.Controls.Image img)
            {
                if (img.Rotation == 180) img.RotateTo(360, 400);
                else img.RotateTo(180, 400);
            }
            string s = ConvertOneSpeed.Text;
            ConvertOneSpeed.Text = ConvertTwoSpeed.Text;
            ConvertTwoSpeed.Text = s;
            TextResultSpeed.Text = ConvertX.ConvertSpeed(ConvertOneSpeed.Text.ToLower(), ConvertTwoSpeed.Text.ToLower(), TextCountSpeed.Text);
        }

        //вторая открыть панель для выбора конвертации тип данных
        private async void Click_Open_TypeTwo_Speed(object sender, EventArgs e)
        {
            if (FrameConvertTypeTwoSpeed.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwoSpeed, 400, false, 300, this);
                await Task.Delay(300);
                FrameConvertTypeTwoSpeed.IsVisible = false;
            }
            else if (!FrameConvertTypeTwoSpeed.IsVisible && !FrameConvertTypeSpeed.IsVisible)
            {
                AnimationX.StartAnimation(FrameConvertTypeTwoSpeed, 400, true, 300, this);
                FrameConvertTypeTwoSpeed.IsVisible = true;
            }
        }

        //обработка ввода цифр
        private void Click_Number_Speed(object sender, EventArgs e)
        {
            if (TextCountSpeed.Text.Length <= 12)
            {
                if (sender is Button btn)
                {
                    TextCountSpeed.Text += btn.Text;
                }
            }
            TextResultSpeed.Text = ConvertX.ConvertSpeed(ConvertOneSpeed.Text.ToLower(), ConvertTwoSpeed.Text.ToLower(), TextCountSpeed.Text);
        }

        //удалить последний символ строки
        private void Click_Del_Speed(object sender, EventArgs e)
        {
            if (TextCountSpeed.Text != "")
            {
                TextCountSpeed.Text = TextCountSpeed.Text[..^1];
            }
            TextResultSpeed.Text = ConvertX.ConvertSpeed(ConvertOneSpeed.Text.ToLower(), ConvertTwoSpeed.Text.ToLower(), TextCountSpeed.Text);
        }

        //удалить все символы строк
        private void Click_DelAll_Speed(object sender, EventArgs e)
        {
            TextCountSpeed.Text = "";
            TextResultSpeed.Text = "";
        }

        //выбор типа данных первый и второй
        private async void Click_Parameter_Speed(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertOneSpeed.Text = (match.Success ? match.Groups[1].Value : string.Empty).Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeSpeed, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeSpeed.IsVisible = false;
            TextResultSpeed.Text = ConvertX.ConvertSpeed(ConvertOneSpeed.Text.ToLower(), ConvertTwoSpeed.Text.ToLower(), TextCountSpeed.Text);
        }
        private async void Click_ParameterTwo_Speed(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                Match match = Regex.Match(lbl.Text, @"\[([^]]*)\]");

                ConvertTwoSpeed.Text = (match.Success ? match.Groups[1].Value : string.Empty).Replace(" ", "");
            }
            AnimationX.StartAnimation(FrameConvertTypeTwoSpeed, 400, false, 300, this);
            await Task.Delay(300);
            FrameConvertTypeTwoSpeed.IsVisible = false;
            TextResultSpeed.Text = ConvertX.ConvertSpeed(ConvertOneSpeed.Text.ToLower(), ConvertTwoSpeed.Text.ToLower(), TextCountSpeed.Text);
        }



        [Obsolete]
        private async void EntrySearchCrypto_Completed(object sender, EventArgs e)
        {
            GridHideKayboard.IsVisible = false;
            RefreshCrypto.IsEnabled = false;
            await Task.Run(() =>
            {
                LoadCrypto.RotateTo(LoadCrypto.Rotation + 10000, 20000);
                AnimationPlusAndMinusWhile = true;
                Thread thread = new Thread(async () => { await AnimateRefreshCrypto(); });
                thread.IsBackground = true;
                thread.Start();

                Device.BeginInvokeOnMainThread(async () =>
                {
                    List<string> SearchCrypto = await Crypto.GetCryptoSearch(EntrySearchCrypto.Text);
                    if (SearchCrypto.Count > 0 && EntrySearchCrypto.Text.Length > 1)
                    {
                        CryptoLayout2.Children.Clear();
                        foreach (string key in SearchCrypto)
                        {
                            var crypto = await Crypto.GetCryptoPriceMarket(key.Replace("/", "").Replace("USDT", ""));
                            Frame f = await Crypto.Card(key.Replace("/", " / "), crypto.priceUSD.ToString().Replace(",", "."), crypto.percentChange24h.ToString());
                            CryptoLayout2.Add(f);
                        }
                    }
                    else
                    {
                        CryptoLayout2.Children.Clear();
                        CryptoLayout2.Add(new Label { Text = "Ничего не найдено", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.End, TextColor = Colors.Grey });
                    }
                    AnimationPlusAndMinusWhile = false;
                    RefreshCrypto.IsEnabled = true;
                });

            });

        }

        private async Task AnimateRefreshCrypto()
        {
            if (AnimationPlusAndMinusWhile)
            {
                await RefreshCrypto.RotateTo(RefreshCrypto.Rotation + 180, 300);
                Thread.Sleep(300);
                await AnimateRefreshCrypto();
            }

        }

        [Obsolete]
        private async void Click_Refresh_Crypto(object sender, EventArgs e)
        {
            await CourceUsdAndEur();
            RefreshCrypto.IsEnabled = false;
            EntrySearchCrypto.Text = "";
            await Task.Run(() =>
            {
                LoadCrypto.RotateTo(LoadCrypto.Rotation + 10000, 20000);
                AnimationPlusAndMinusWhile = true;
                Thread thread = new Thread(async () => { await AnimateRefreshCrypto(); });
                thread.IsBackground = true;
                thread.Start();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await InitializeCrypto();
                    AnimationPlusAndMinusWhile = false;
                    RefreshCrypto.IsEnabled = true;
                });

            });




        }

        private async void ClickTheme(object sender, EventArgs e)
        {
            if (sender is ImageButton btn)
            {
                if (Preferences.Default.Get("Theme", "White") == "White")
                {
                    await MainGrid.FadeTo(0, 300);
                    MainGrid.IsVisible = false;

                    MainContent.BackgroundColor = Color.Parse("#1C274C");
                    ThemeSet.SetDarkTheme(this);
                    btn.Source = "suntheme.svg";
                    Preferences.Default.Set("Theme", "Dark");

                    MainGrid.IsVisible = true;
                    await MainGrid.FadeTo(1, 400);
                }
                else if (Preferences.Default.Get("Theme", "White") == "Dark")
                {
                    await MainGrid.FadeTo(0, 300);
                    MainGrid.IsVisible = false;

                    MainContent.BackgroundColor = Color.Parse("#fff");
                    ThemeSet.SetWhiteTheme(this);
                    btn.Source = "moontheme.svg";
                    Preferences.Default.Set("Theme", "White");

                    MainGrid.IsVisible = true;
                    await MainGrid.FadeTo(1, 400);
                }
            }
            StartGrid.IsVisible = false; StartGrid.IsVisible = true;
        }

        private void Click_Hide_Keyboard(object sender, TappedEventArgs e)
        {
            EntrySearchCrypto.IsEnabled = false;
            EntrySearchCrypto.IsEnabled = true;
            GridHideKayboard.IsVisible = false;
        }

        private void Click_Entry_Input(object sender, TappedEventArgs e)
        {
            GridHideKayboard.IsVisible = true;
        }

        private void Click_Close_CryptoConverter(object sender, EventArgs e)
        {
            PopupCrypto.IsOpen = false;
        }

        private void CryptoConverter_Changes(object sender, EventArgs e)
        {
            if (sender is NumericEdit numEdit)
            {
                if (numEdit.AutomationId == "CryptoConverterCoin")
                {
                    CryptoConverterUsd.Value = numEdit.Value * CourseCruptoActive;
                    CryptoConverterRub.Value = Convert.ToDecimal(Convert.ToDouble(CryptoConverterUsd.Value) * Convert.ToDouble(CourseUsd.Text.Replace("$ ", "").Replace(".", ",")));
                }
                else if (numEdit.AutomationId == "CryptoConverterUsd")
                {
                    CryptoConverterCoin.Value = numEdit.Value / CourseCruptoActive;
                    CryptoConverterRub.Value = numEdit.Value * Convert.ToDecimal(CourseUsd.Text.Replace("$ ", "").Replace(".", ","));
                }
                else if (numEdit.AutomationId == "CryptoConverterRub")
                {
                    CryptoConverterUsd.Value = numEdit.Value / Convert.ToDecimal(CourseUsd.Text.Replace("$ ", "").Replace(".", ","));
                    CryptoConverterRub.Value = CryptoConverterUsd.Value / CourseCruptoActive;
                }

            }
        }
    }
}