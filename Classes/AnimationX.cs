using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate_MauiDevExpress_1._0.Class
{
    public static class AnimationX
    {
        public static void StartAnimation(View view, int Count, bool UpDown, uint duration, Page page)
        {
            var animation = new Animation();
            if (UpDown)
            {
                animation.Add(0, 1, new Animation(v => view.TranslationY = v, Count, 0));
            }
            else
            {
                animation.Add(0, 1, new Animation(v => view.TranslationY = v, 0, Count));
            }

            animation.Commit(page, "SimpleAnimation", 20, duration);
        }
    }
}
