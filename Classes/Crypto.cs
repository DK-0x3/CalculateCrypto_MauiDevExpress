using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Calculate_MauiDevExpress_1._0.Class
{
    public static class Crypto
    {
        public static void Crop2(ref string str, int disired)
        {
            str = str.Substring(0, disired + 1 > str.Length ? str.Length : disired + 1);

            if (str.Contains('.'))
            {
                string[] strs = str.Split('.');

                if (strs[1].Length < 2)
                {
                    if (strs[1].Length <= 0)
                    {
                        str = strs[0];
                    }
                    else
                    {
                        str = (Math.Round(Convert.ToDouble(str.Replace(".", ",")))).ToString();
                    }
                }
            }
        }

        public static void Separation(ref string str)
        {
            if (str.Contains('.'))
            {
                string[] parts = str.Split('.');

                // Добавляем пробелы через каждые три символа в части до точки
                str = string.Format("{0:#,0}.{1}", long.Parse(parts[0]), parts[1]);
            }
            else
            {
                str = string.Format("{0:#,0}", long.Parse(str));
            }
        }

        public static async Task<string> GetUsdToRub(double usd)
        {
            string rub = (usd * await GetUsdToRubAsync("USD")).ToString();
            rub = rub.Replace(",", ".");
            Crop2(ref rub, 7);
            Separation(ref rub);
            return rub;
        }

        //карточка с данными о курсе крипто
        public async static Task<Frame> Card(string NameCrypto, string PriceCryptoUsd, string precent)
        {
            // Создаем элементы Grid и Label
            Grid grid = new Grid();
            grid.ColumnSpacing = 0;
            grid.RowSpacing = 0;

            Label nameLabel = new Label
            {
                Text = NameCrypto,
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Start
            };

            Grid grid2 = new Grid();
            Grid.SetRow(grid2, 1);
            Grid.SetColumnSpan(grid2, 2);

            grid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Crop2(ref PriceCryptoUsd, 6);
            Separation(ref PriceCryptoUsd);
            Label priceLabel = new Label
            {
                Text = $"{PriceCryptoUsd} $",
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Colors.White
            };

            string rub = (Convert.ToDouble(PriceCryptoUsd.Replace(" ", "").Replace(".", ",")) * await GetUsdToRubAsync("USD")).ToString();
            rub = rub.Replace(",", ".");
            Crop2(ref rub, 7);
            Separation(ref rub);
            Label equivalentLabel = new Label
            {
                Text = $"≈ {rub} ₽",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.Parse("#DBDBDB")
            };
            char plusOrMinus = precent[0] == '-' ? '-' : '+';
            precent = precent.Replace(".", ",").Replace("-", "");
            precent = Convert.ToDouble(precent).ToString();
            Crop2(ref precent, 3);

            Label percentLabel = new Label
            {
                Text = $"{plusOrMinus}{precent}%",
                FontSize = 20,
                TextColor = Color.Parse("#14FF00")
            };

            if (plusOrMinus == '-')
            {
                percentLabel.TextColor = Colors.Red;
            }

            Frame frame = new Frame
            {
                HeightRequest = 150,
                Margin = new Thickness(15),
                BackgroundColor = Color.Parse("#1C274C"),
                Content = grid,
                AutomationId = $"{NameCrypto.Split(" / ")[0]}/{PriceCryptoUsd}"
            };

            Frame frame2 = new Frame
            {
                BackgroundColor = Colors.Transparent,
                Opacity = 0.9,
                CornerRadius = 30,
                Padding = 0,
                Content = priceLabel
            };

            // Определения строк и столбцов
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Star) });

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Добавляем элементы в Grid
            grid.Add(nameLabel, 0, 0);
            Grid.SetColumnSpan(nameLabel, 2);

            grid2.Add(frame2, 0, 0);
            grid2.Add(equivalentLabel, 1, 0);
            grid.Add(grid2, 0, 1);
            Grid.SetColumnSpan(grid2, 2);
            grid.Add(percentLabel, 1, 0);

            return frame;
        }

        //парсер курса крипто с API ByBit V2
        public static async Task<(double priceUSD, double percentChange24h, double volumeUSD24h)> GetCryptoPriceMarket(string coinSymbol)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", "88c25f44-04fa-4860-8ed0-45feb4d3b43b");
                    string url = $"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={coinSymbol}";

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    JsonDocument jsonDocument = JsonDocument.Parse(responseBody);

                    double priceUSD = jsonDocument.RootElement
                        .GetProperty("data")
                        .GetProperty(coinSymbol)
                        .GetProperty("quote")
                        .GetProperty("USD")
                        .GetProperty("price")
                        .GetDouble();

                    double percentChange24h = jsonDocument.RootElement
                        .GetProperty("data")
                        .GetProperty(coinSymbol)
                        .GetProperty("quote")
                        .GetProperty("USD")
                        .GetProperty("percent_change_24h")
                        .GetDouble();

                    double volumeUSD24h = jsonDocument.RootElement
                        .GetProperty("data")
                        .GetProperty(coinSymbol)
                        .GetProperty("quote")
                        .GetProperty("USD")
                        .GetProperty("volume_24h")
                        .GetDouble();

                    return (priceUSD, percentChange24h, volumeUSD24h);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return (0, 0, 0); // Return default values in case of error
            }
        }

        public static async Task<double> GetUsdToRubAsync(string currency)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"https://api.exchangerate-api.com/v4/latest/{currency.ToUpper()}");
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            using (JsonDocument document = JsonDocument.Parse(responseBody))
            {
                // Получаем корневой элемент
                JsonElement root = document.RootElement;

                root.TryGetProperty("rates", out JsonElement resultElement);
                JsonElement firstResult = resultElement;

                firstResult.TryGetProperty("RUB", out JsonElement symbolx);

                return Convert.ToDouble(symbolx.GetDouble());
            }
        }

        public static async Task<List<string>> GetCryptoSearch(string SearchSymbol)
        {
            // Создаем HttpClient
            using var client = new HttpClient();

            // Загружаем JSON с веб-страницы
            var response = await client.GetAsync("https://api.bybit.com/v2/public/symbols");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            // Парсим JSON
            using (JsonDocument document = JsonDocument.Parse(responseBody))
            {
                // Получаем корневой элемент
                JsonElement root = document.RootElement;

                // Проверяем, что корневой элемент - объект
                if (root.ValueKind == JsonValueKind.Object)
                {
                    // Получаем массив symbols
                    JsonElement symbolsArray = root.GetProperty("result");
                    List<string> result = [];
                    // Итерируем по элементам массива
                    foreach (JsonElement symbolname in symbolsArray.EnumerateArray())
                    {
                        // Получаем значение base_currency
                        string? baseCurrency = symbolname.GetProperty("base_currency").GetString();
                        string? quoteCurrency = symbolname.GetProperty("quote_currency").GetString();

#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                        if (baseCurrency != null && baseCurrency.Contains(SearchSymbol) && quoteCurrency.Contains("USDT"))
                        {
                            result.Add($"{baseCurrency}/{quoteCurrency}");
                        }
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                    }
                    return result;
                }
                else
                {
                    return ["null"];
                }
            }
        }
    }
}
