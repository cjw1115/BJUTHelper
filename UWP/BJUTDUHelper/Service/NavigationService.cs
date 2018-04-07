using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace BJUTDUHelper.Service
{
    public class NavigationService
    {
        private static List<EventHandler<BackRequestedEventArgs>> Handlers { get; set; } = new List<EventHandler<BackRequestedEventArgs>>();
        private static Dictionary<EventHandler<BackRequestedEventArgs>, List<EventHandler<BackRequestedEventArgs>>> singleHandlers { get; set; } = new Dictionary<EventHandler<BackRequestedEventArgs>, List<EventHandler<BackRequestedEventArgs>>>();
        private static List<EventHandler<BackRequestedEventArgs>> HandlerIndexs { get; set; } = new List<EventHandler<BackRequestedEventArgs>>();
        private static EventHandler<BackRequestedEventArgs> lastSingleHandler = null;
        private static AppViewBackButtonVisibility orinalVisibility;
        //暂时全盘接管导航

        public static void RegSingleHandler(EventHandler<BackRequestedEventArgs> singleHandler)
        {

            var view = SystemNavigationManager.GetForCurrentView();
            
            if (singleHandlers.Count <= 0)
                lastSingleHandler = null;
            else
            {
                lastSingleHandler = singleHandlers.Last().Key;
            }

            if (lastSingleHandler == null)
            {
                var list = new List<EventHandler<BackRequestedEventArgs>>();
                list.AddRange(Handlers);
                singleHandlers.Add(singleHandler, list);

                foreach (var item in Handlers)
                {
                    view.BackRequested -= item;
                }
                view.BackRequested += singleHandler;

                HandlerIndexs.Add(singleHandler);

                orinalVisibility = SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility;
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                var list = new List<EventHandler<BackRequestedEventArgs>>();

                list.Add(lastSingleHandler);
                singleHandlers.Add(singleHandler, list);

                view.BackRequested -= lastSingleHandler;
                view.BackRequested += singleHandler;
                HandlerIndexs.Add(singleHandler);
            }
            
        }
        //移除导航处理handler
        public static void UnRegSingleHandler(EventHandler<BackRequestedEventArgs> singleHandler)
        {
            var view = SystemNavigationManager.GetForCurrentView();

            if (HandlerIndexs.Count <= 0)
            {
                lastSingleHandler = null;
            }
            else
            {
                lastSingleHandler = HandlerIndexs.Last();
            }

            if (lastSingleHandler == singleHandler)
            {
                view.BackRequested -= lastSingleHandler;
                var list = singleHandlers[lastSingleHandler];
                foreach (var item in list)
                {
                    view.BackRequested += item;
                }
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = orinalVisibility;
            }
            else
            {
                var index=HandlerIndexs.IndexOf(singleHandler);
                if (index == 0)
                {
                    var last = HandlerIndexs[1];
                    singleHandlers[last] = singleHandlers[singleHandler];
                    //view.BackRequested -= singleHandler;
                    //view.BackRequested += last;
                }
                else
                {
                    var front= HandlerIndexs[index -1];
                    var last = HandlerIndexs[index+1];
                    singleHandlers[last] = singleHandlers[front];
                    //view.BackRequested -= singleHandler;
                    //view.BackRequested += last;
                }
            }
            HandlerIndexs.Remove(singleHandler);
            singleHandlers.Remove(singleHandler);
        }

        //在处理程序委托链上增加一个处理程序
        public static void RegHandler(EventHandler<BackRequestedEventArgs> handler)
        {
            Handlers.Add(handler);
            var view = SystemNavigationManager.GetForCurrentView();
            view.BackRequested += handler;
        }
        //在处理程序委托链上减少一个处理程序
        public static void UnRegHandler(EventHandler<BackRequestedEventArgs> handler)
        {
            Handlers.Remove(handler);
            var view = SystemNavigationManager.GetForCurrentView();

            view.BackRequested -= handler;
        }

        
    }
}
