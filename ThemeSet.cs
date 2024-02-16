using DevExpress.Maui.Controls;
using DevExpress.Maui.Editors;
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
                        if (label.TextColor.ToRgbaHex() == Color.Parse("#030303").ToRgbaHex())
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

                foreach (var autocomedit in MainPage.GetVisualTreeDescendants().OfType<AutoCompleteEdit>())
                {
                    autocomedit.PlaceholderColor = Color.Parse("#5c5c5c");
                    autocomedit.TextColor = Color.Parse("#f2f2f2");
                    autocomedit.DropDownBackgroundColor = Color.Parse("#1C274C");
                    autocomedit.DropDownItemTextColor = Color.Parse("#f5f5f5");
                    autocomedit.IconColor = Color.Parse("#7a7a7a");
                }
                foreach (var popup in MainPage.GetVisualTreeDescendants().OfType<DXPopup>())
                {
                    foreach(var grid in popup.GetVisualTreeDescendants().OfType<Grid>())
                    {
                        grid.BackgroundColor = Color.Parse("#152147");
                        foreach (var element in grid.Children)
                        {
                            if (element is NumericEdit NEdit)
                            {
                                NEdit.TextColor = Color.Parse("#f2f2f2");
                            }
                        }
                    }
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

                foreach (var autocomedit in MainPage.GetVisualTreeDescendants().OfType<AutoCompleteEdit>())
                {
                    autocomedit.PlaceholderColor = Colors.Black;
                    autocomedit.TextColor = Colors.Black;
                    autocomedit.DropDownBackgroundColor = Color.Parse("#f5f5f5");
                    autocomedit.DropDownItemTextColor = Colors.Black;
                    autocomedit.IconColor = Colors.Gray;
                }
                foreach (var popup in MainPage.GetVisualTreeDescendants().OfType<DXPopup>())
                {
                    foreach (var grid in popup.GetVisualTreeDescendants().OfType<Grid>())
                    {
                        grid.BackgroundColor = Color.Parse("#f5f5f5");
                        foreach (var element in grid.Children)
                        {
                            if (element is NumericEdit NEdit)
                            {
                                NEdit.TextColor = Colors.Black;
                            }
                        }
                    }
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
