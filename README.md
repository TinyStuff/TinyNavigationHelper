# TinyNavigationHelper
TinyNavigationHelper is a library that is created for you that want to abstract the navigation without installing a bigger MVVM framework. 

Today there are only an implementation for Xamarin.Forms, but it created in a way that it will be possible to create implementatons for other platforms as well.

## How to install
For your Xamarin.Forms project install the package from NuGet:

```
Install-Package TinyNavigationHelper.Forms -pre
```

If you want to use navigation from a project that not has references to Xamarin.Forms (for example if you have your ViewModels in a separete project for use on other platforms), install the abstraction package for that project.

```
Install-Package TinyNavigationHelper.Abstraction -pre
```

## How to configure navigation for Xamarin.Forms

```cs
var navigationHelper = new NavigationHelper(this);
navigationHelper.RegisterView<MainView>("MainView");
```
If you want to use it with an IoC instance you need to register the specific instance in the IoC container. The example below is how to register in Autofac, but you can use the container you prefer.

```cs
var containerBuilder = new ContainerBuilder();
containerBuilder.RegisterInstance<INavigationHelper>(navigationHelper);

var container = containerBuilder.Build();
```
## How to use TinyNavigationHelper
There are two ways to use the navigation helper. To resolve the INavigationHelper interface via foreample constructor inbjection, or to access it via the Current property on the NavigationHelper class in the TinyNavigationHelper.Abstraction namespace.

```cs
var navigationHelper = NavigationHelper.Current;
```

### Navigate
To navigate to a page use the NavigateTo method and the key you registered for the page.

```cs
await navigationHelper.NavigateToAsync("MainView");
```

#### Navigate with parameter
The NavigateTo method can take a argument if you want to pass a data to the view you're navigating to.

```cs
await navigationHelper.NavigateToAsync("MainView", "Parameter");
```
The parameter will be sent to the constructor of the page.

```cs
public class MainView
{
     public MainView(object parameter)
     {
          var data = parameter as string;
     }
}
```
#### Navigate back
To navigate back, use the Back method.

```cs
await navigationHelper.BackAsync();
```

#### Forms specific extensions
On Xamarin Forms there are a set of extension methods that allows for navigation to an instantiated page. Note that this will only work on the project that references `TinyNavigationHelper.Forms`.

WARNING: This will only work in Xamarin Forms since it references `Xamarin.Forms`.

```cs
// Step 1 - Add a using to reference the extension method
using TinyNavigationHelper.Forms;

// Step 2 - Navigate
var page = new NewsView();
await NavigationHelper.Current.NavigateToAsync(page);
```

### Modal
To open a modal, use the OpenModal method.
```cs
await navigationHelper.OpenModalAsync("MainView");
```
You can send a parameter to a modal in the same way as with the NavigateTo method.

```cs
await navigationHelper.OpenModalAsync("MainView", "parameter");
```

If you want the modal to have it's own navigation stack you can pass pass true the withNavigation argument.

```cs
//without parameter
await navigationHelper.OpenModalAsync("MainView", true);

//with parameter
await navigationHelper.OpenModalAsync("MainView", "parameter", true);
```

### Set as root page
If you want to navigate to a page and reset the history in the navigation stack you can use the SetRootView method.

```cs
//without parameter and navigation stack
navigationHelper.SetRootView("MainView");

//with parameter, but without navigation stack
navigationHelper.SetRootView("MainView", "parameter");

//without parameter, but with navigation stack
navigationHelper.SetRootView("MainView", true);

//with parameter and navigation stack
navigationHelper.SetRootView("MainView", "parameter", true);
```

### Handle view keys
The recommendation is to not use the string directly in your code, but instead create a class with view constants.

```cs
public class ViewConstants
{
     public const string MainView = "MainView";
}
```

```cs
var navigationHelper = new NavigationHelper(this);
navigationHelper.RegisterView<MainView>(ViewConstants.MainView);
```

```cs
await navigationHelper.NavigateToAsync(ViewConstants.MainView);
```

