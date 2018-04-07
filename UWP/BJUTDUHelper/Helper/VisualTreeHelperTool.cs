using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BJUTDUHelper.Helper
{
    public class VisualTreeHelperTool
    {
        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            int count = Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
        public static T FindNamedVisualChild<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            Queue<DependencyObject> queue = new Queue<DependencyObject>();
            queue.Enqueue(obj);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                int count = Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(item);
                for (int i = 0; i < count; i++)
                {
                    var re=Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(item, i);
                    var child=re as FrameworkElement;
                    if (child != null && child.Name == name)
                    {
                        return child as T;
                    }
                    else
                    {
                        queue.Enqueue(child);
                    }
                }
            }


            return null;
        }

        public static VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null || string.IsNullOrWhiteSpace(name))
                return null;

            IList<VisualStateGroup> groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (var group in groups)
            {
                if (group.Name == name)
                    return group;
            }

            return null;
        }
    }
}
