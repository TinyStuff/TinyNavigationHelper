using System;
using System.Collections.Generic;
using System.Text;

namespace TinyNavigationHelper.Abstraction
{
    public class NavigationHelper
    {
        public static INavigationHelper Current { get; set; }
    }
}
