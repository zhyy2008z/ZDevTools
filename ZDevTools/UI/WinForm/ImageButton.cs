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
    [Description("自定义图像按钮类（ImageList版本），用来创建拥有释放、悬浮、按下动作的按钮。")]
    [ToolboxBitmap(typeof(ImageButton))]
    [ToolboxItem(true)]
    public class ImageButton : ImageButtonBase
    {
        #region Properties

        ImageList imageList;
        /// <summary>
        /// 按钮的图像列表对象，用来为按钮的各种状态提供图像
        /// </summary>
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("ImageButton")]
        [Description("按钮的图像列表对象，用来为按钮的各种状态提供图像")]
        public ImageList ImageList
        {
            get { return this.imageList; }
            set
            {
                this.imageList = value;
                this.UpdateOriginalImage();
            }
        }

        #endregion

        protected override Image GetImage(int index)
        {
            return this.ImageList.Images[index];
        }

        protected override int GetImageCount()
        {
            if (this.ImageList == null)
                return 0;
            return this.ImageList.Images.Count;
        }
    }
}
