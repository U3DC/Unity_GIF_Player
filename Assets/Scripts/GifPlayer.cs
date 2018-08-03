using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace U3DC.UI.Extension.Common
{
    /// <summary>
    /// Gif动画支持
    /// 1.挂载此脚本到UGUI的Image上
    /// 2.使用SetGifPath(string path)传入GIF图片路径
    /// </summary>
    public class GifPlayer : MonoBehaviour
    {
        // 帧数   
        private const float Fps = 24;
        private UnityEngine.UI.Image _image;
        public List<Texture2D> _tex2DList = new List<Texture2D>();
        private float _time;

        private void Awake()
        {
            _image = GetComponent<UnityEngine.UI.Image>();
        }

        /// <summary>
        /// 设置gif图片的绝对路径
        /// </summary>
        /// <param name="path"></param>
        public void SetGifPath(string path)
        {
            UnityEngine.Resources.UnloadUnusedAssets();
            var image = System.Drawing.Image.FromFile(path);
            _tex2DList = Gif2Texture2D(image);
        }

        private int _framCount;
        /// <summary>
        /// Gif转Texture2D
        /// </summary>
        /// <param name="image"> System.Image</param>
        /// <returns>Texture2D集合</returns>
        private List<Texture2D> Gif2Texture2D(System.Drawing.Image image)
        {
            var tex = new List<Texture2D>();
            if (image == null) return tex;
            // 图片构成有两种形式： 1、多页(.gif)  2、多分辨率
            // 获取image对象的dimenson数，打印结果是1。
            Debug.Log("image对象的dimenson数:" + image.FrameDimensionsList.Length);
            // image.FrameDimensionsList[0]-->获取image对象第一个dimension的 Guid（全局唯一标识符）
            // 根据指定的GUID创建一个提供获取图像框架维度信息的实例
            var frameDimension = new FrameDimension(image.FrameDimensionsList[0]);
            // 获取指定维度的帧数
            _framCount = image.GetFrameCount(frameDimension);
            Debug.Log(_framCount);
            // 遍历图像帧
            for (var i = 0; i < _framCount; i++)
            {
                // 选择由维度和索引指定的帧（激活图像帧）; 
                image.SelectActiveFrame(frameDimension, i);
                // 创建指定大小的 Bitmap 的实例。
                var framBitmap = new Bitmap(image.Width, image.Height);
                // 从指定的Image 创建新的Graphics,并在指定的位置使用原始物理大小绘制指定的 Image,将当前激活帧的图形绘制到framBitmap上;
                // 简单点就是从 frameBitmap（里面什么都没画，是张白纸）创建一个Graphics，然后执行画画DrawImage
                using (var newGraphics = System.Drawing.Graphics.FromImage(framBitmap))
                {
                    newGraphics.DrawImage(image, Point.Empty);
                }
                // 创建一个指定大小的 Texture2D 的实例
                var frameTexture2D = new Texture2D(framBitmap.Width, framBitmap.Height, TextureFormat.ARGB32, true);
                // 执行Bitmap转Texture2D
                frameTexture2D.LoadImage(Bitmap2Byte(framBitmap));
                // 添加到列表中
                tex.Add(frameTexture2D);
            }
            return tex;
        }
        /// <summary>
        /// Bitmap转Byte
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        /// <returns>byte数组</returns>
        private byte[] Bitmap2Byte(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                // 将bitmap 以png格式保存到流中
                bitmap.Save(stream, ImageFormat.Png);
                // 创建一个字节数组，长度为流的长度
                var data = new byte[stream.Length];
                // 重置指针
                stream.Seek(0, SeekOrigin.Begin);
                // 从流读取字节块存入data中
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }


        private void Update()
        {
            if (_tex2DList.Count <= 0) return;
            if (_framCount < 2) return;
            _time += Time.deltaTime;
            var index = (int)(_time * Fps) % _tex2DList.Count;
            if (_image != null)
            {
                _image.sprite = Sprite.Create(_tex2DList[index], new Rect(0, 0, _tex2DList[index].width, _tex2DList[index].height), new Vector2(0.5f, 0.5f));
            }
        }
    }
}
