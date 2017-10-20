using System;
using System.Collections.Generic;
using System.Text;
using TinyNavigationHelper.Abstraction;
using Xamarin.Forms;

namespace TinyNavigationHelper.Forms
{
    public class DefaultViewCreator : IViewCreator<Page>
    {
        public Page CreateView(Type type)
        {
            return (Page)Activator.CreateInstance(type);
        }

        public Page CreateView(Type type, object parameter)
        {
            return (Page)Activator.CreateInstance(type, parameter);
        }
    }
}
