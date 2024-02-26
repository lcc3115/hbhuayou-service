using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;

namespace HYSignServices.ToolsDoc
{
    public class Image_Tools
    {
        //获取背景图片
        static Image imgBack = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "images\\back-img.jpg");
        //获取图片文件所在相对路径
        static string PATH = AppDomain.CurrentDomain.BaseDirectory + "images\\";

        public static Bitmap CreateImg(string phase_name, string count, string cross_id)
        {
            List<ImgInfo> imgList = RotateImageByPhase(phase_name, cross_id);
            Bitmap bmp = null;
            bmp = CombinImage(imgList, 0, -100);
            if (bmp != null)
            {
                bmp = Add_CountDown(bmp, count);
                return bmp;
                //bmp.Save("F:\\test.jpg", ImageFormat.Jpeg);
            }
            return null;
        }

        public static Bitmap CombinImage(List<ImgInfo> imgList, int xDeviation = 0, int yDeviation = 0)
        {
            Bitmap bmp = new Bitmap(imgBack.Width, imgBack.Height);
            Graphics g = Graphics.FromImage(bmp);
            for (int i = 0; i < imgList.Count; i++)
            {
                g.DrawImage(imgList[i].img, imgList[i].xy[0], imgList[i].xy[1], imgList[i].img.Width / 10, imgList[i].img.Height / 10);
            }
            return bmp;
        }

        private static List<ImgInfo> RotateImageByPhase(string phase_name, string cross_id)
        {
            List<ImgInfo> imgList = new List<ImgInfo>();
            /***********箭头图片所在位置***********/
            int[] left_up = {180,150 };
            int[] left_down = {180,480 };
            int[] right_up = {550,150 };
            int[] right_down = {550,480 };
            /***********箭头图片所在位置***********/
            ImgInfo imgInfo = new ImgInfo();
            switch (phase_name)
            {
                case "东向西左直":
                case "东向西":
                case "东放行":
                    imgInfo.img = Image.FromFile(PATH + "straightorleftorright.png");//straightorleftorright
                    imgInfo.img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    imgInfo.xy = right_up;
                    imgList.Add(imgInfo);
                    break;
                case "西向东左直":
                case "西向东":
                    imgInfo.img = Image.FromFile(PATH + "straightorleftorright.png");//straightorleftorright
                    imgInfo.img.RotateFlip(RotateFlipType.Rotate270FlipX);
                    imgInfo.xy = left_down;
                    imgList.Add(imgInfo);
                    break;
                case "北向南左直":
                case "北向南":
                    imgInfo.img = Image.FromFile(PATH + "straightorleftorright.png");//直行或左转或右转straightorleftorright
                    imgInfo.img.RotateFlip(RotateFlipType.Rotate180FlipX);
                    imgInfo.xy = left_up;
                    imgList.Add(imgInfo);
                    break;
                case "南向北左直":
                case "南向北":
                    imgInfo.img = Image.FromFile(PATH + "straightorleftorright.png");//直行或左转或右转straightorleftorright
                    imgInfo.xy = right_down;
                    imgList.Add(imgInfo);
                    break;
                case "南北左直":
                case "南北放行":
                case "北向南直、南向北左直":
                    for (int i = 0; i < 2; i++)
                    {
                        imgInfo = new ImgInfo();
                        imgInfo.img = Image.FromFile(PATH + "straightorleftorright.png");//直行或左转或右转straightorleftorright
                        if (i == 0)
                        {
                            imgInfo.img.RotateFlip(RotateFlipType.Rotate180FlipX);
                            imgInfo.xy = left_up;
                        }
                        else
                        {
                            imgInfo.xy = right_down;
                        }
                        imgList.Add(imgInfo);
                    }
                    break;
                case "东西放行":
                case "东西左直":
                    for (int i = 0; i < 2; i++)
                    {
                        imgInfo = new ImgInfo();
                        imgInfo.img = Image.FromFile(PATH + "straightorleftorright.png");//直行或左转或右转straightorleftorright
                        if (i == 0)
                        {
                            imgInfo.img.RotateFlip(RotateFlipType.Rotate90FlipX);
                            imgInfo.xy = right_up;
                        }
                        else
                        {
                            imgInfo.img.RotateFlip(RotateFlipType.Rotate270FlipX);
                            imgInfo.xy = left_down;
                        }
                        imgList.Add(imgInfo);
                    }
                    break;
                case "南北直":
                case "南北直行":
                    if (cross_id == "420100023165" || cross_id == "420100023112")
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            imgInfo = new ImgInfo();
                            if (i == 0)
                            {
                                imgInfo.img = Image.FromFile(PATH + "straight.png");//直行straight
                                imgInfo.img.RotateFlip(RotateFlipType.Rotate180FlipX);
                                imgInfo.xy = left_up;;
                            }
                            else
                            {
                                imgInfo.img = Image.FromFile(PATH + "leftorstraight.png");//直行和左转leftorstraight
                                imgInfo.xy = right_down;
                            }
                            imgList.Add(imgInfo);
                        }
                    }
                    if (cross_id == "420100023167")
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            imgInfo = new ImgInfo();
                            if (i == 0)
                            {
                                imgInfo.img = Image.FromFile(PATH + "rightorstraight.png");//直行和右转rightorstraight
                                imgInfo.img.RotateFlip(RotateFlipType.Rotate180FlipX);
                                imgInfo.xy = left_up;
                            }
                            else
                            {
                                imgInfo.img = Image.FromFile(PATH + "rightorstraight.png");//直行和右转rightorstraight
                                imgInfo.xy = right_down;
                            }
                            imgList.Add(imgInfo);
                        }
                    }
                    break;
                case "东向西左转":
                    imgInfo.img = Image.FromFile(PATH + "left.png");//左转left
                    imgInfo.xy = right_up;
                    imgList.Add(imgInfo);
                    break;
                case "南北左":
                    for (int i = 0; i < 2; i++)
                    {
                        imgInfo = new ImgInfo();
                        imgInfo.img = Image.FromFile(PATH + "left.png");//左转left
                        if (i == 0)
                        {
                            imgInfo.img.RotateFlip(RotateFlipType.Rotate90FlipX);
                            imgInfo.xy = left_up;
                        }
                        else
                        {
                            imgInfo.img.RotateFlip(RotateFlipType.Rotate270FlipX);
                            imgInfo.xy = right_down;
                        }
                        imgList.Add(imgInfo);
                    }
                    break;
                case "西放行":
                    for (int i = 0; i < 2; i++)
                    {
                        imgInfo = new ImgInfo();
                        if (i == 0)
                        {
                            imgInfo.img = Image.FromFile(PATH + "right.png");//右转right
                            imgInfo.img.RotateFlip(RotateFlipType.Rotate180FlipX);
                            imgInfo.xy = left_up;
                        }
                        else
                        {
                            imgInfo.img = Image.FromFile(PATH + "straightorleftorright.png");//直行或左转或右转straightorleftorright
                            imgInfo.img.RotateFlip(RotateFlipType.Rotate270FlipX);
                            imgInfo.xy = left_down;
                        }
                        imgList.Add(imgInfo);
                    }
                    break;

                case "南北直（下匝道右转）":
                    for (int i = 0; i < 2; i++)
                    {
                        imgInfo = new ImgInfo();
                        if (i == 0)
                        {
                            imgInfo.img = Image.FromFile(PATH + "rightorstraight.png");//直行和右转rightorstraight
                            imgInfo.img.RotateFlip(RotateFlipType.Rotate180FlipX);
                            imgInfo.xy = left_up;
                        }
                        else
                        {
                            imgInfo.img = Image.FromFile(PATH + "leftorstraight.png");//直行和左转leftorstraight
                            imgInfo.xy = left_down;
                        }
                        imgList.Add(imgInfo);
                    }
                    break;
                default:
                    break;
            }
            return imgList;
        }

        private static Bitmap Add_CountDown(Bitmap bmp,string count)
        {
            Graphics g = Graphics.FromImage(bmp);
            String str = count;
            Font font = new Font("宋体", 96, FontStyle.Bold);
            SolidBrush sbrush = new SolidBrush(Color.FromArgb(0, 206, 209));
            g.DrawString(str, font, sbrush, new PointF(450, 300));
            return bmp;
        }
    }

    public class ImgInfo
    {
        public Image img;
        public int[] xy = new int[2];
    }
}