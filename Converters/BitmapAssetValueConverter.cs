//using Avalonia.Data.Converters;
//using Avalonia.Platform;
//using Avalonia;
//using System.Globalization;
//using System.Reflection;
//using System;
//using Avalonia.Media.Imaging;
 


//public class BitmapAssetValueConverter : IValueConverter
//{
//    public static BitmapAssetValueConverter Instance = new BitmapAssetValueConverter();

//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        if (value == null)
//            return null;

//        if (value is string rawUri && targetType.IsAssignableFrom(typeof(Bitmap)))
//        {
//            Uri uri;

//            // Allow for assembly overrides
//            if (rawUri.StartsWith("avares://"))
//            {
//                uri = new Uri(rawUri);
//            }
//            else
//            {
//                string assemblyName = Assembly.GetEntryAssembly().GetName().Name;
//                uri = new Uri($"avares://{assemblyName}{rawUri}");
//            }

//            // Use AvaloniaLocator without 'Current'
//            var assets = AvaloniaLocator.Instance.GetService<IAssetLoader>();  // This is the corrected line

//            var asset = assets.Open(uri);

//            return new Bitmap(asset);
//        }

//        throw new NotSupportedException();
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        throw new NotSupportedException();
//    }
//}
