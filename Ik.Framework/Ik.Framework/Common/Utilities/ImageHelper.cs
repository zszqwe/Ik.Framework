using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Utilities
{
    /// <summary>
    /// 图片处理帮助类
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 从文字生成图片默认生成jpeg格式图片（生成图片）
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="font">字体</param>
        /// <param name="rect">矩形</param>
        /// <param name="brush">文字颜色格式</param>
        /// <param name="backColor">矩形背景颜色</param>
        /// <returns></returns>
        public static Bitmap CreatePicByText(string text, Font font, Rectangle rect, Brush brush, Color backColor)
        {
            Graphics g;
            Bitmap bmp;
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            bmp = new Bitmap(rect.Width, rect.Height);
            g = Graphics.FromImage(bmp);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.FillRectangle(new SolidBrush(backColor), rect);
            g.DrawString(text, font, brush, rect, format);
            g.Dispose();

            #region  混淆正弦扭曲
            Random r = new Random(3);
            bmp = TwistImage(bmp, true, r.Next(3, 6), r.Next(2, 6));
            #endregion

            return bmp;
        }

        /// <summary>
        /// 根据文字和拼音生成汉字拼音合成图片(合成图片)
        /// </summary>
        /// <param name="spell">拼音数组</param><param name="text">文本数组</param>
        /// <returns/>
        public static Bitmap DefaultCreatePicByTextAndSpell(string[] text, string[] spell)
        {
            if (text.Length <= 0 && spell.Length <= 0)
            {
                throw new ArgumentNullException("text and spell is can not be null");
            }
            var rect = new Rectangle(0, 0, 100, 52);
            var spell_rect = new Rectangle(0, 0, 100, 30);
            var spell_font = new Font("微软雅黑", 18);
            var text_font = new Font("微软雅黑", 30);
            var brush = Brushes.SandyBrown;
            var color = Color.White;

            Bitmap bitmap = new Bitmap(rect.Width * text.Length, spell_rect.Height + rect.Height);
            Graphics graphics = Graphics.FromImage((Image)bitmap);
            int num = text.Length > spell.Length ? text.Length : spell.Length;
            for (int index = 0; index < num; ++index)
            {
                Bitmap picByText1 = ImageHelper.CreatePicByText(index >= text.Length ? " " : text[index], text_font, rect,
                    brush, color);
                Bitmap picByText2 = ImageHelper.CreatePicByText(index >= spell.Length ? " " : spell[index], spell_font, spell_rect,
                    brush, color);
                graphics.DrawImage((Image)picByText2, index * rect.Width, 0, rect, GraphicsUnit.Pixel);
                graphics.DrawImage((Image)picByText1, index * rect.Width, spell_rect.Height, rect, GraphicsUnit.Pixel);
            }
            graphics.Dispose();

            #region  混淆  噪音线  噪点
            //混淆逻辑
            Graphics g = Graphics.FromImage(bitmap);
            //生成随机生成器 
            Random random = new Random();

            //画图片的背景噪音线 
            for (int i = 0; i < 12; i++)
            {
                int x1 = random.Next(bitmap.Width);
                int x2 = random.Next(bitmap.Width);
                int y1 = random.Next(spell_rect.Height, bitmap.Height);
                int y2 = random.Next(spell_rect.Height, bitmap.Height);

                g.DrawLine(new Pen(Color.SandyBrown, 2), x1, y1, x2, y2);
            }

            //画图片的前景噪音点 
            for (int i = 0; i < 200; i++)
            {
                int x = random.Next(bitmap.Width);
                int y = random.Next(bitmap.Height);

                bitmap.SetPixel(x, y, Color.SandyBrown);
            }
            #endregion

            return bitmap;
        }


        //2PI
        private const double PI2 = 6.283185307179586476925286766559;

        /// <summary>
        /// 正弦曲线Wave扭曲图片（Edit By 51aspx.com）
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="dMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        private static Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            Graphics graph = Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();
            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);
                    Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            return destBmp;
        }

    }
}
