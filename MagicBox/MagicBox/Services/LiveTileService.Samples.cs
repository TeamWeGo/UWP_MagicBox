using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace MagicBox.Services
{
    internal partial class PrimaryTile /*LiveTileService*/
    {
        // More about Live Tiles Notifications at https://docs.microsoft.com/windows/uwp/controls-and-patterns/tiles-and-notifications-sending-a-local-tile-notification
        public string time { get; set; } = "8:15 AM, Saturday";
        public string musicName { get; set; } = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore.";
        public string branding { get; set; } = "name";
        public string appName { get; set; } = "MyList";
        public Uri uri { get; set; } = new Uri("http://m2.music.126.net/Nquqb_EIL1akMIyB1a7B6A==/1416170988286688.mp3");

        public PrimaryTile(string input1, string input2, Uri uri)
        {
            musicName = input1;
            time = input2;
            this.uri = uri;
        }




    }
}
