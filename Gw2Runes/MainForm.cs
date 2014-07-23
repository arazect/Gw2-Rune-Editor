﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AndaForceUtils.Math;

namespace Gw2Runes
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region Constants

        private const int BackgroundWidth = 350;
        private const int BackgroundHeight = 600;

        #endregion

        #region Resource variables

        private Image _background;
        private Image _rune;

        private readonly PrivateFontCollection _privateFontCollection = new PrivateFontCollection();
        private Font _captionFont;
        private Font _textFont;
        private SolidBrush _captionBrush = new SolidBrush(Color.FromArgb(220, 158, 37));
        private SolidBrush _textBrush = new SolidBrush(Color.FromArgb(156, 156, 156));

        private String _runeTextString;

        #endregion

        #region Events

        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowVersionInfo();
            LoadResources();
            LoadDefaulValues();
        }

        private void pBox_Paint(object sender, PaintEventArgs e)
        {
            DrawPreview(e.Graphics);
        }

        private void tbText_TextChanged(object sender, EventArgs e)
        {
            _runeTextString = tbRuneText.Text;

            pBox.Refresh();
        }

        #endregion

        #region Button Events

        private void btnCaptionColor_Click(object sender, EventArgs e)
        {
            ChangeBrushColorViaDialog(ref _captionBrush);
        }

        private void btnTextColor_Click(object sender, EventArgs e)
        {
            ChangeBrushColorViaDialog(ref _textBrush);
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            saveFileDialog.FileName = tbCaption.Text;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var pBoxImage = new Bitmap(BackgroundWidth, BackgroundHeight);
                var workingGraphics = Graphics.FromImage(pBoxImage);

                // Height of entered text + 90px rune icon and caption + 60px for empty space under
                var resultHeight =
                    (int) MathHelper.Clamp(
                        workingGraphics.MeasureString(_runeTextString, _textFont).Height + 90 + 60,
                        0,
                        BackgroundHeight);

                DrawPreview(Graphics.FromImage(pBoxImage));

                var imageResult = new Bitmap(BackgroundWidth, resultHeight);
                workingGraphics = Graphics.FromImage(imageResult);
                workingGraphics.DrawImage(pBoxImage, 0, 0);
                workingGraphics.DrawRectangle(new Pen(Color.Black, 2), 0, 0, imageResult.Width - 1,
                    imageResult.Height - 1);

                imageResult.Save(
                    saveFileDialog.FileName,
                    new[] {ImageFormat.Jpeg, ImageFormat.Png}[saveFileDialog.FilterIndex]);
            }
        }

        #endregion

        #region Universal Events

        private void RefreshPictureBox(object sender, EventArgs e)
        {
            pBox.Refresh();
        }

        #endregion

        #region Private Methods

        private void ShowVersionInfo()
        {
            Text = String.Format(
                "{0}, Version: {1}.{2}",
                Assembly.GetExecutingAssembly().GetName().Name,
                Assembly.GetExecutingAssembly().GetName().Version.Major, Assembly.GetExecutingAssembly().GetName().Version.Minor);
        }

        private void LoadDefaulValues()
        {
            _runeTextString = tbRuneText.Text;
        }

        private void LoadResources()
        {
            _background = Properties.Resources.background;
            _rune = Properties.Resources.rune;

            LoadFontFromResources();
//            _privateFontCollection.AddFontFile(@"Resources\Fonts\Fritz_Quadrata_Cyrillic_Regular.ttf");

            _captionFont = new Font(_privateFontCollection.Families.First().Name, 18.0f, GraphicsUnit.Pixel);
            _textFont = new Font(_privateFontCollection.Families.First().Name, 16.0f, GraphicsUnit.Pixel);

//            _background = Image.FromFile(@"Resources\images\Background\background.png");
//            _rune = Image.FromFile(@"Resources\Images\Runes\rune.jpg");
        }

        private void LoadFontFromResources()
        {
            const string resourceFontName = "Gw2Runes.Resources.Fonts.Fritz_Quadrata_Cyrillic_Regular.ttf";
            Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFontName);

            IntPtr data = Marshal.AllocCoTaskMem((int) fontStream.Length);
            var fontdata = new byte[fontStream.Length];
            fontStream.Read(fontdata, 0, (int) fontStream.Length);

            Marshal.Copy(fontdata, 0, data, (int) fontStream.Length);

            _privateFontCollection.AddMemoryFont(data, (int) fontStream.Length);

            fontStream.Close();
            Marshal.FreeCoTaskMem(data);
        }

        private void DrawPreview(Graphics e)
        {
            if (_background == null) return;
            if (_rune == null) return;

            e.SmoothingMode = SmoothingMode.AntiAlias;
            e.DrawImage(_background, new Point(0, 0));
            e.DrawImage(_rune, new Point(12, 28));
            e.DrawString(
                String.Format("Superior Rune of {0}", tbCaption.Text),
                _captionFont,
                _captionBrush,
                new PointF(60, 44));
            e.DrawString(
                _runeTextString,
                _textFont,
                _textBrush,
                new PointF(18, 88));
        }

        #endregion

        #region Extracted Methods

        private void ChangeBrushColorViaDialog(ref SolidBrush brush)
        {
            using (var colorDialog = new ColorDialog {AllowFullOpen = true, Color = _captionBrush.Color})
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    brush = new SolidBrush(colorDialog.Color);
                }
            }

            pBox.Refresh();
        }

        #endregion
    }
}