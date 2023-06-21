using SharpDX.MediaFoundation;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Wallpaper_Engine
{
    public class Thumbnail
    {
        /*private string filepath;

        public Thumbnail(string filepath)
        {
            this.filepath = filepath;
            MediaManager.Startup();
        }*/

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", CallingConvention = CallingConvention.StdCall)]
        private static extern void RtlMoveMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);

        public static BitmapSource Create(string filepath)
        {
            MediaManager.Startup();
            Stopwatch stopwatch = Stopwatch.StartNew();
            SourceReader reader = null;

            try
            {
                using MediaAttributes attr = new MediaAttributes(1);
                using MediaType newMediaType = new MediaType();

                //SourceReaderに動画のパスを設定
                attr.Set(SourceReaderAttributeKeys.EnableVideoProcessing.Guid, true);
                reader = new SourceReader(filepath, attr);

                //出力メディアタイプをRGB32bitに設定
                newMediaType.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
                newMediaType.Set(MediaTypeAttributeKeys.Subtype, VideoFormatGuids.Rgb32);
                reader.SetCurrentMediaType(SourceReaderIndex.FirstVideoStream, newMediaType);

                //元のメディアタイプから動画情報を取得する
                // duration:ビデオの総フレーム数
                // frameSize:フレーム画像サイズ（上位32bit:幅 下位32bit:高さ）
                // stride:フレーム画像一ライン辺りのバイト数
                MediaType mediaType = reader.GetCurrentMediaType(SourceReaderIndex.FirstVideoStream);
                long duration = reader.GetPresentationAttribute(SourceReaderIndex.MediaSource, PresentationDescriptionAttributeKeys.Duration);
                long frameSize = mediaType.Get(MediaTypeAttributeKeys.FrameSize);
                int stride = mediaType.Get(MediaTypeAttributeKeys.DefaultStride);
                Rectangle rect = new Rectangle()
                {
                    Width = (int)(frameSize >> 32),
                    Height = (int)(frameSize & 0xffffffff)
                };

                //取得する動画の位置を設定
                double mulPositionOfPercent = Math.Min(Math.Max(50, 0), 100.0) / 100.0;
                reader.SetCurrentPosition((long)(duration * mulPositionOfPercent));

                //動画から1フレーム取得し、Bitmapオブジェクトを作成してメモリコピー
                using Sample sample = reader.ReadSample(SourceReaderIndex.FirstVideoStream, SourceReaderControlFlags.None, out int actualStreamIndex, out SourceReaderFlags readerFlags, out long timeStampRef);
                using MediaBuffer buf = sample.ConvertToContiguousBuffer();
                IntPtr pBuffer = buf.Lock(out int maxLength, out int currentLength);
                Bitmap bmp = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                var bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                RtlMoveMemory(bmpData.Scan0, pBuffer, stride * rect.Height);
                bmp.UnlockBits(bmpData);
                buf.Unlock();

                //bitmap->bitmapsource
                using var ms = new MemoryStream();
                // MemoryStreamに書き出す
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                // MemoryStreamをシーク
                ms.Seek(0, SeekOrigin.Begin);
                // MemoryStreamからBitmapFrameを作成
                // (BitmapFrameはBitmapSourceを継承しているのでそのまま渡せばOK)
                BitmapSource bitmapSource =
                    BitmapFrame.Create(
                        ms,
                        BitmapCreateOptions.None,
                        BitmapCacheOption.OnLoad
                    );
                return bitmapSource;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }

                stopwatch.Stop();
                //textBox1.AppendText($"process time {stopwatch.ElapsedMilliseconds} msec ({moviePath})\r\n");
                MediaManager.Shutdown();
            }
        }
    }
}
