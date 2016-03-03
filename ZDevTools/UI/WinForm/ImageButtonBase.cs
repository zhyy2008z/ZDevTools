using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace ZDevTools.UI.WinForm
{
    /// <summary>
    /// <para>自定义图像按钮类，用来创建拥有释放、悬浮、按下动作的按钮。</para>
    /// <para>设置ImageList、MouseHoverImageIndex、MouseDownImageIndex，分别用来提供用于各种动作的图像、鼠标悬浮于按钮上时图像在ImageList中的索引、鼠标按下时的索引。</para>
    /// </summary>
    [Description("自定义图像按钮基类，用来创建拥有释放、悬浮、按下动作的按钮。")]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    public abstract partial class ImageButtonBase : Control
    {
        #region consts
        //定义悬浮/按下时图片的缩放比例
        const float HoverWeight = 0.05f;
        const float DownWeight = 0.1f;
        const float HoverRotateAngle = 20f;
        const float DownRotateAngle = 40f;
        const float HoverOffsetRate = 0.1f;
        const float DownOffsetRate = 0.15f;
        #endregion

        #region Constructors
        /// <summary>
        /// 构造函数
        /// </summary>
        public ImageButtonBase()
        {
            InitializeComponent();

            this.SuspendLayout();

            this.SetStyle(ControlStyles.CacheText | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw | ControlStyles.UserMouse, true);

            this.BackColor = Color.Transparent;
            this.MouseHoverImageIndex = -1;
            this.MouseDownImageIndex = -1;
            this.TextAlign = ContentAlignment.MiddleCenter;

            this.ResumeLayout(false);
        }
        #endregion

        #region Properties

        AnimationStyle _animationStyle;
        /// <summary>
        /// 选择按钮的动画风格，默认是无动画样式
        /// </summary>
        [DefaultValue(AnimationStyle.None)]
        [Localizable(true)]
        [Description("选择按钮的动画风格，默认是无动画样式")]
        [Category("ImageButton")]
        public AnimationStyle AnimationStyle
        {
            get { return _animationStyle; }
            set
            {
                _animationStyle = value;
                switch (AnimationStyle)
                {
                    case AnimationStyle.ZoomOut:
                        this.BackgroundImageLayout = ImageLayout.None;
                        break;
                    case AnimationStyle.ClockwiseRotation:
                        this.BackgroundImageLayout = ImageLayout.None;
                        break;
                    case AnimationStyle.AnticlockwiseRotation:
                        this.BackgroundImageLayout = ImageLayout.None;
                        break;
                    case AnimationStyle.CustomImage:
                        this.BackgroundImageLayout = ImageLayout.None;
                        break;
                    case AnimationStyle.Offset:
                        this.BackgroundImageLayout = ImageLayout.Center;
                        break;
                    case AnimationStyle.None:
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 鼠标停留时使用的ImageList索引
        /// </summary>
        [DefaultValue(-1)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [TypeConverter(typeof(ImageIndexConverter))]
        [Category("ImageButton")]
        [Description("鼠标停留时使用的ImageList索引")]
        public int MouseHoverImageIndex { get; set; }

        /// <summary>
        /// 鼠标按下时使用的ImageList索引
        /// </summary>
        [DefaultValue(-1)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [TypeConverter(typeof(ImageIndexConverter))]
        [Category("ImageButton")]
        [Description("鼠标按下时使用的ImageList索引")]
        public int MouseDownImageIndex { get; set; }

        int originalImageIndex;
        /// <summary>
        /// 获取或设置原始图像在ImageList中的索引
        /// </summary>
        [DefaultValue(-1)]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [TypeConverter(typeof(ImageIndexConverter))]
        [Category("ImageButton")]
        [Description("获取或设置原始图像在ImageList中的索引")]
        public int OriginalImageIndex
        {
            get { return originalImageIndex; }
            set
            {
                this.originalImageIndex = value;
                this.UpdateOriginalImage();
            }
        }

        Image originalImage;
        /// <summary>
        /// 设置按钮原始背景图像
        /// </summary>
        [Description("设置按钮原始背景图像")]
        [DefaultValue("")]
        [Localizable(true)]
        [Category("ImageButton")]
        public Image OriginalImage
        {
            get
            {
                return originalImage;
            }
            set
            {
                this.BackgroundImage = value;
                this.originalImage = value;
                if (this.DesignMode && value != null)
                    this.Size = value.Size;
            }
        }

        /// <summary>
        /// 获取/设置按钮文本
        /// </summary>
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [SettingsBindable(true)]
        [Category("Appearance")]
        [Description("获取/设置按钮文本")]
        [Browsable(true)]
        [EditorBrowsable]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] //important!!
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.Invalidate();//重绘整个button
            }
        }

        ContentAlignment textAlign;
        /// <summary>
        /// 获取/设置文字对齐方案
        /// </summary>
        [Localizable(true)]
        [Category("Appearance")]
        [Description("获取/设置文字对齐方案")]
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlign { get { return textAlign; } set { textAlign = value; this.Invalidate(); } }
        #endregion

        #region Behaviors
        /// <summary>
        /// 引发ImageButton的Click事件
        /// </summary>
        public void PerformClick()
        {
            this.OnClick(new EventArgs());
        }
        #endregion

        #region infrastructures
        delegate void DrawBackgroundImage(Image originalImage, int originalWidth, int originalHeight, Graphics newBackgroundGraphics);

        void setImage(DrawBackgroundImage drawBackgroundImage)
        {
            var originalBitmap = this.OriginalImage;
            if (originalBitmap == null) return;
            var originalWidth = originalBitmap.Width;
            var originalHeight = originalBitmap.Height;

            Bitmap bitmap = new Bitmap(originalWidth, originalHeight);
            var g = Graphics.FromImage(bitmap);

            drawBackgroundImage(originalBitmap, originalWidth, originalHeight, g);

            this.BackgroundImage = bitmap;
        }

        //绘制Text属性到ImageButton
        private void drawText(PaintEventArgs e)
        {
            //绘制文字
            if (!string.IsNullOrEmpty(this.Text))
            {
                var g = e.Graphics;

                var clientWidth = this.ClientSize.Width;
                var clientHeight = this.ClientSize.Height;
                SizeF measureSize = g.MeasureString(this.Text, this.Font, clientWidth);
                float x = 0;
                float y = 0;
                float width = measureSize.Width;
                float height = measureSize.Height;

                switch (this.TextAlign)
                {
                    case ContentAlignment.BottomCenter:
                        x = (clientWidth - measureSize.Width) / 2;//center
                        y = (clientHeight - measureSize.Height);//bottom
                        if (y < 0) y = 0;
                        break;
                    case ContentAlignment.BottomLeft:
                        y = (clientHeight - measureSize.Height);//bottom
                        if (y < 0) y = 0;
                        break;
                    case ContentAlignment.BottomRight:
                        x = (clientWidth - measureSize.Width);//right
                        y = (clientHeight - measureSize.Height);//bottom
                        if (y < 0) y = 0;
                        break;
                    case ContentAlignment.MiddleCenter:
                        x = (clientWidth - measureSize.Width) / 2;//center
                        y = (clientHeight - measureSize.Height) / 2;//middle
                        if (y < 0) y = 0;
                        break;
                    case ContentAlignment.MiddleLeft:
                        y = (clientHeight - measureSize.Height) / 2;//middle
                        if (y < 0) y = 0;
                        break;
                    case ContentAlignment.MiddleRight:
                        x = (clientWidth - measureSize.Width);//right
                        y = (clientHeight - measureSize.Height) / 2;//middle
                        if (y < 0) y = 0;
                        break;
                    case ContentAlignment.TopCenter:
                        x = (clientWidth - measureSize.Width) / 2;//center
                        break;
                    case ContentAlignment.TopLeft:
                        break;
                    case ContentAlignment.TopRight:
                        x = (clientWidth - measureSize.Width);//right
                        break;
                    default:
                        break;
                }

                g.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new RectangleF(x, y, width, height));
            }
        }

        /// <summary>
        /// 更新原始图像
        /// </summary>
        protected void UpdateOriginalImage()
        {
            int imageCount = this.GetImageCount();
            if (imageCount > 0)
            {
                if (this.OriginalImageIndex > -1 && this.OriginalImageIndex < imageCount)
                    this.OriginalImage = this.GetImage(this.OriginalImageIndex);
                else
                    this.OriginalImage = this.GetImage(0);
            }
        }

        /// <summary>
        /// 获取图像
        /// </summary>
        /// <param name="index">图像索引号</param>
        /// <returns></returns>
        protected abstract Image GetImage(int index);

        /// <summary>
        /// 获取图像总数量
        /// </summary>
        /// <returns></returns>
        protected abstract int GetImageCount();

        #endregion

        #region loadHoverImage
        void loadHoverImage()
        {
            switch (AnimationStyle)
            {
                case AnimationStyle.ZoomOut:
                    setZoomOutHoverImage();
                    break;
                case AnimationStyle.ClockwiseRotation:
                    setRotationImage(HoverRotateAngle);
                    break;
                case AnimationStyle.AnticlockwiseRotation:
                    setRotationImage(-HoverRotateAngle);
                    break;
                case AnimationStyle.CustomImage:
                    if (MouseHoverImageIndex > -1 && this.MouseHoverImageIndex < this.GetImageCount())
                        this.BackgroundImage = this.GetImage(this.MouseHoverImageIndex);
                    break;
                case AnimationStyle.Offset:
                    if (OriginalImage != null)
                        this.Size = new Size((int)(OriginalImage.Width * (1 + HoverOffsetRate)), (int)(OriginalImage.Height * (1 + HoverOffsetRate)));
                    break;
                case AnimationStyle.None:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void setZoomOutHoverImage()
        {
            setImage(new DrawBackgroundImage((originalBitmap, originalWidth, originalHeight, g) =>
            {
                int offsetX = (int)(HoverWeight * originalWidth);
                int offsetY = (int)(HoverWeight * originalHeight);

                if (offsetX == 0)
                    offsetX = 1;

                if (offsetY == 0)
                    offsetY = 1;

                g.DrawImage(originalBitmap, new Rectangle(offsetX, offsetY, originalWidth - 2 * offsetX, originalHeight - 2 * offsetY));
            }));
        }

        void setRotationImage(float rotateAngle)
        {
            setImage(new DrawBackgroundImage((originalBitmap, originalWidth, originalHeight, g) =>
            {
                g.TranslateTransform(originalWidth / 2, originalHeight / 2);
                g.RotateTransform(rotateAngle);
                g.DrawImage(originalBitmap, -originalWidth / 2, -originalHeight / 2, originalWidth, originalHeight);
            }));
        }

        #endregion

        #region loadDownImage
        void loadDownImage()
        {
            switch (AnimationStyle)
            {
                case AnimationStyle.ZoomOut:
                    setZoomOutDownImage();
                    break;
                case AnimationStyle.ClockwiseRotation:
                    setRotationImage(DownRotateAngle);
                    break;
                case AnimationStyle.AnticlockwiseRotation:
                    setRotationImage(-DownRotateAngle);
                    break;
                case AnimationStyle.CustomImage:
                    if (this.MouseDownImageIndex > -1 && this.MouseDownImageIndex < this.GetImageCount())
                        this.BackgroundImage = this.GetImage(this.MouseDownImageIndex);
                    break;
                case AnimationStyle.Offset:
                    if (OriginalImage != null)
                        this.Size = new Size((int)(OriginalImage.Width * (1 + DownOffsetRate)), (int)(OriginalImage.Height * (1 + DownOffsetRate)));
                    break;
                case AnimationStyle.None:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void setZoomOutDownImage()
        {
            setImage(new DrawBackgroundImage((originalBitmap, originalWidth, originalHeight, g) =>
            {
                int offsetX = (int)(DownWeight * originalWidth);
                int offsetY = (int)(DownWeight * originalHeight);

                if (offsetX < 2)
                    offsetX = 2;

                if (offsetY < 2)
                    offsetY = 2;

                g.DrawImage(originalBitmap, new Rectangle(offsetX, offsetY, originalWidth - 2 * offsetX, originalHeight - 2 * offsetY));
            }));

        }
        #endregion

        #region eventHandlers

        /// <summary>
        /// 当绘制背景时发生
        /// </summary>
        /// <param name="e">绘制事件参数</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            this.drawText(e);
        }

        private void ImageButton_MouseLeave(object sender, EventArgs e)
        {
            switch (AnimationStyle)
            {
                case AnimationStyle.ZoomOut:
                    this.BackgroundImage = this.OriginalImage;
                    break;
                case AnimationStyle.ClockwiseRotation:
                    this.BackgroundImage = this.OriginalImage;
                    break;
                case AnimationStyle.AnticlockwiseRotation:
                    this.BackgroundImage = this.OriginalImage;
                    break;
                case AnimationStyle.CustomImage:
                    this.BackgroundImage = this.OriginalImage;
                    break;
                case AnimationStyle.Offset:
                    if (OriginalImage != null)
                        this.Size = this.OriginalImage.Size;
                    break;
                case AnimationStyle.None:
                    break;
                default:
                    break;
            }
        }

        private void ImageButton_MouseEnter(object sender, EventArgs e)
        {
            this.loadHoverImage();
        }

        private void ImageButton_MouseDown(object sender, MouseEventArgs e)
        {
            this.loadDownImage();
        }

        private void ImageButton_MouseUp(object sender, MouseEventArgs e)
        {
            this.loadHoverImage();
        }

        #endregion


    }
}
