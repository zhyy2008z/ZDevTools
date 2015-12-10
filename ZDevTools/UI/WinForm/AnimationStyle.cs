using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZDevTools.UI.WinForm
{
	public enum AnimationStyle
	{
		/// <summary>
		/// 缩小显示
		/// </summary>
		ZoomOut,
		/// <summary>
		/// 顺时针旋转
		/// </summary>
		ClockwiseRotation,
		/// <summary>
		/// 逆时针旋转
		/// </summary>
		AnticlockwiseRotation,
		/// <summary>
		/// 自定义图像
		/// </summary>
		CustomImage,
		/// <summary>
		/// 无动画效果
		/// </summary>
		None
	}
}
