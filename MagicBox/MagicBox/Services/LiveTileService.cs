using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MagicBox.Activation;
using MagicBox.Helpers;

using Windows.ApplicationModel.Activation;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace MagicBox.Services
{
    internal partial class LiveTileService /*: ActivationHandler<LaunchActivatedEventArgs>*/
    {

        static public void SetBadgeCountOnTile(int count)
        {
            // Update the badge on the real tile
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

            XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
            badgeElement.SetAttribute("value", count.ToString());

            BadgeNotification badge = new BadgeNotification(badgeXml);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
        }

        public static Windows.Data.Xml.Dom.XmlDocument CreateTiles(PrimaryTile primaryTile)
        {
            XDocument xDoc = new XDocument(
                new XElement("tile", new XAttribute("version", 3),
                    new XElement("visual",
                        // Small Tile
                        new XElement("binding", new XAttribute("branding", primaryTile.branding), new XAttribute("displayName", primaryTile.appName), new XAttribute("template", "TileSmall"),
                        new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", primaryTile.uri)),
                        new XElement("group",
                            new XElement("subgroup",
                                new XElement("text", primaryTile.musicName, new XAttribute("hint-style", "caption")),
                                new XElement("text", primaryTile.time, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))

                                )
                            )
                        ),

                        // Medium Tile
                        new XElement("binding", new XAttribute("branding", primaryTile.branding), new XAttribute("displayName", primaryTile.appName), new XAttribute("template", "TileMedium"),
                        new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", primaryTile.uri)),
                        new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile.musicName, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile.time, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        ),

                        // Wide Tile
                        new XElement("binding", new XAttribute("branding", primaryTile.branding), new XAttribute("displayName", primaryTile.appName), new XAttribute("template", "TileWide"),
                        new XElement("image", new XAttribute("placement", "background"), new XAttribute("src", primaryTile.uri)),
                        new XElement("group",
                            new XElement("subgroup",
                                new XElement("text", primaryTile.musicName, new XAttribute("hint-style", "caption")),
                                new XElement("text", primaryTile.time, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                //new XElement("text", primaryTile.message2, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        )

                    )
                )
            );

            Windows.Data.Xml.Dom.XmlDocument xmlDoc = new Windows.Data.Xml.Dom.XmlDocument();
            xmlDoc.LoadXml(xDoc.ToString());
            return xmlDoc;
        }
    }
}
