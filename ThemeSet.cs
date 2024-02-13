using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate_MauiDevExpress_1._0
{
    public static class ThemeSet
    {
        public static void SetDarkTheme(Page MainPage)
        {
            if (MainPage != null)
            {
                var labels = MainPage.GetVisualTreeDescendants().OfType<Label>();

                foreach (var label in labels)
                {
                    if (IsInScrollView(label).AutomationId != "ScrollCrypto")
                    {
                        if (label.TextColor == Colors.Black)
                        {
                            label.TextColor = Color.Parse("#f5f5f5");
                        }
                        if (label.TextColor.ToRgbaHex() == Color.Parse("#262626").ToRgbaHex())
                        {
                            label.TextColor = Color.Parse("#f2f2f2");
                        }
                    }
                }
                var BtnImgs = MainPage.GetVisualTreeDescendants().OfType<ImageButton>();

                foreach (var btn in BtnImgs)
                {
                    if (btn.Source.ToString().Replace("File: ", "") != "suntheme.svg" && btn.Source.ToString().Replace("File: ", "") != "moontheme.svg")
                    {
                        btn.Source = $"dark_{btn.Source.ToString().Replace("File: ", "")}";
                        btn.BackgroundColor = Colors.Transparent;
                    }
                }

                var Imgs = MainPage.GetVisualTreeDescendants().OfType<Image>();

                foreach (var img in Imgs)
                {
                    img.Source = $"dark_{img.Source.ToString().Replace("File: ", "")}";

                }

                var Btns = MainPage.GetVisualTreeDescendants().OfType<Button>();

                foreach (var btn in Btns)
                {
                    btn.BackgroundColor = Colors.Transparent;
                    btn.TextColor = Color.Parse("#f5f5f5");
                }

                var Frames = MainPage.GetVisualTreeDescendants().OfType<Frame>();

                foreach (var frm in Frames)
                {

                    if (IsInScrollView(frm).AutomationId != "ScrollCrypto")
                    {
                        frm.BackgroundColor = Color.Parse("#03113d");
                        foreach (var lbl in frm.GetVisualTreeDescendants().OfType<Label>())
                        {
                            lbl.TextColor = Color.Parse("#f2f2f2");
                        }
                    }


                }

                foreach (var entry in MainPage.GetVisualTreeDescendants().OfType<Entry>())
                {
                    entry.TextColor = Color.Parse("#f5f5f5");
                }
            }
        }
        public static void SetWhiteTheme(Page MainPage)
        {
            if (MainPage != null)
            {
                var labels = MainPage.GetVisualTreeDescendants().OfType<Label>();

                foreach (var label in labels)
                {
                    if (IsInScrollView(label).AutomationId != "ScrollCrypto")
                    {
                        if (label.TextColor.ToRgbaHex() == Color.Parse("#f5f5f5").ToRgbaHex())
                        {
                            label.TextColor = Colors.Black;
                        }
                        else if (label.TextColor.ToRgbaHex() == Color.Parse("#f2f2f2").ToRgbaHex())
                        {
                            label.TextColor = Color.Parse("#262626");
                        }
                    }

                }
                var BtnImgs = MainPage.GetVisualTreeDescendants().OfType<ImageButton>();

                foreach (var btn in BtnImgs)
                {
                    btn.Source = $"{btn.Source.ToString().Replace("File: ", "").Replace("dark_", "")}";
                    btn.BackgroundColor = Colors.Transparent;
                }

                var Imgs = MainPage.GetVisualTreeDescendants().OfType<Image>();

                foreach (var img in Imgs)
                {
                    img.Source = $"{img.Source.ToString().Replace("File: ", "").Replace("dark_", "")}";
                }

                var Btns = MainPage.GetVisualTreeDescendants().OfType<Button>();

                foreach (var btn in Btns)
                {
                    btn.BackgroundColor = Colors.Transparent;
                    btn.TextColor = Color.Parse("#1C274C");
                }

                var Frames = MainPage.GetVisualTreeDescendants().OfType<Frame>();

                foreach (var frm in Frames)
                {

                    if (frm.BackgroundColor.ToRgbaHex() != Color.Parse("#1C274C").ToRgbaHex() && frm.BackgroundColor.ToRgbaHex() != Colors.Transparent.ToRgbaHex())
                    {

                        frm.BackgroundColor = Color.Parse("#f2f2f2");
                        foreach (var lbl in frm.GetVisualTreeDescendants().OfType<Label>())
                        {
                            lbl.TextColor = Color.Parse("#1C274C");
                        }

                    }
                }

                foreach (var entry in MainPage.GetVisualTreeDescendants().OfType<Entry>())
                {
                    entry.TextColor = Color.Parse("#1C274C");
                }
            }
        }
        private static ScrollView IsInScrollView(Element element)
        {
            Element parent = element.Parent;

            while (parent != null)
            {
                if (parent is ScrollView scrol)
                {
                    return scrol;
                }

                parent = parent.Parent;
            }
            ScrollView scr = new ScrollView();
            return scr; // Элемент не находится внутри ScrollView
        }

    }
}
