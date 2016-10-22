using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Crossout.Model.Items;
using System.IO;

namespace Crossout.Images
{
    public class EmbedImageCreator
    {
        Item item;
        string itemNameString;
        string sellPriceString;
        string buyPriceString;
        string branding;
        string backgroundPath = Path.GetFullPath(@"Sources\background.png");
        string itemImagePath;

        public EmbedImageCreator(Item imageItem, string imageBranding = "CrossoutDB.com")
        {
            item = imageItem;
            itemNameString = item.Name;
            sellPriceString = "Sell Price: " + item.FormatSellPrice;
            buyPriceString = "Buy Price: " + item.FormatBuyPrice;
            branding = imageBranding;

            itemImagePath = Path.GetFullPath(@"img\items\" + item.Id + ".png");
        }

        PointF overlayLocation = new PointF(0f, 0f);
        PointF itemNameLocation = new PointF(80f, 1f);
        PointF sellPriceLocation = new PointF(80f, 20f);
        PointF buyPriceLocation = new PointF(80f, 35f);
        PointF brandingLocation = new PointF(200f, 45f);

        Brush RarityColor(int rarityNumber)
        {
            Brush b;
            switch (rarityNumber)
            {
                case 1:
                    b = Brushes.White;
                    break;
                case 2:
                    b = Brushes.Blue;
                    break;
                case 3:
                    b = Brushes.Purple;
                    break;
                case 4:
                    b = Brushes.Gold;
                    break;
                case 5:
                    b = Brushes.OrangeRed;
                    break;
                default:
                    b = Brushes.LightGray;
                    break;
            }
            
            return b;
        }

        public Image CreateEmbedImage()
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(backgroundPath);//load the image file
            Bitmap overlay = (Bitmap)Image.FromFile(itemImagePath);

            Graphics graphics = Graphics.FromImage(bitmap);
            Font arialFont = new Font("Arial", 8);
            Font arialBoldFont = new Font("Arial", 8, FontStyle.Bold);

            graphics.DrawString(itemNameString, arialBoldFont, RarityColor(item.RarityId), itemNameLocation);
            graphics.DrawString(sellPriceString, arialFont, Brushes.White, sellPriceLocation);
            graphics.DrawString(buyPriceString, arialFont, Brushes.White, buyPriceLocation);
            graphics.DrawString(branding, arialFont, Brushes.White, brandingLocation);

            graphics.DrawImage(overlay, overlayLocation);

            Image img = bitmap;

            return img;
        }
    }
}
