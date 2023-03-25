using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;
using otdVision.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otdVision.Service
{
    public class VisionProcessor
    {
        public VisionProcessor()
        {
            Noises.Add(new Noise() { NoiseId = "椒盐噪声", NoiseDesc = "这是一种随机的噪声，类似于黑色和白色的盐和胡椒点。这种噪声通常由传感器故障或数据传输错误引起" });
            Noises.Add(new Noise() { NoiseId = "高斯噪声", NoiseDesc = "这是一种正态分布的噪声，通常由光线、传感器或数据传输引起。它通常是一种连续的噪声，可视为随机的颜色变化" });
            Noises.Add(new Noise() { NoiseId = "模拟信号噪声", NoiseDesc = "这种噪声通常由相机的电路和传感器引起，通常在低光条件下更明显" });


            WaveFilters.Add(new WaveFilter() { FilterId = "均值滤波器", FilterDesc = "这种滤波器对图像中的每个像素周围的像素取平均值。它对高斯噪声有一定的去除效果，但不适用于椒盐噪声" });
            WaveFilters.Add(new WaveFilter() { FilterId = "高斯滤波器", FilterDesc = "这种滤波器是一种线性平滑滤波器，它使用一组卷积核来将周围像素的值加权平均。它可以有效地去除高斯噪声，但不适用于椒盐噪声" });
            WaveFilters.Add(new WaveFilter() { FilterId = "中值滤波器", FilterDesc = "这种滤波器用周围像素的中值来代替当前像素的值。它对椒盐噪声有很好的去除效果，但对图像的平滑程度较差" });
            WaveFilters.Add(new WaveFilter() { FilterId = "双边滤波器", FilterDesc = "这种滤波器在处理图像时不仅考虑像素值之间的相似性，还考虑它们在空间上的距离。这种滤波器能够在保留边缘信息的同时去除噪声" });
            WaveFilters.Add(new WaveFilter() { FilterId = "自适应滤波器", FilterDesc = "这种滤波器采用像素周围的局部像素值的加权平均值，以提高噪声去除效果。它可以根据图像中的噪声强度自适应地调整滤波器的大小" });
            WaveFilters.Add(new WaveFilter() { FilterId = "小波滤波器", FilterDesc = "这种滤波器使用小波变换来对图像进行去噪处理。它可以同时去除高斯噪声和椒盐噪声" });

            EdgeDetections.Add(new EdgeDetection() { DetectionId = "Canny", DetectionDesc = "Canny边缘检测算法是一种广泛使用的基于梯度的边缘检测算法。该算法将图像转换为灰度图像，并通过应用高斯滤波器来减少噪声。接着，通过计算图像的梯度来检测边缘，并使用非极大值抑制来获得更细的边缘线条。最后，应用双阈值算法来确定哪些边缘是真实的，并使用滞后阈值去除不需要的边缘" });
            EdgeDetections.Add(new EdgeDetection() { DetectionId = "Sobel", DetectionDesc = "Sobel边缘检测算法是一种基于梯度的边缘检测算法，通过在x和y方向应用Sobel算子来计算图像的梯度。该算法可以在x和y方向分别检测水平和垂直边缘" });
            EdgeDetections.Add(new EdgeDetection() { DetectionId = "Scharr", DetectionDesc = "Scharr边缘检测算法是一种改进的Sobel算法，它使用3x3的卷积核来检测边缘" });
            EdgeDetections.Add(new EdgeDetection() { DetectionId = "Laplacian", DetectionDesc = "Laplacian边缘检测算法是一种基于二阶导数的边缘检测算法。该算法通过计算图像的拉普拉斯算子来检测边缘。它通常比其他算法更容易检测到图像中的弧形和圆形边缘" });
            EdgeDetections.Add(new EdgeDetection() { DetectionId = "Roberts", DetectionDesc = "Roberts边缘检测算法是一种基于微分的边缘检测算法，它使用2x2的卷积核来检测图像中的边缘。它通常对噪声比较敏感" });
            EdgeDetections.Add(new EdgeDetection() { DetectionId = "Prewitt", DetectionDesc = "Prewitt边缘检测算法是一种基于梯度的边缘检测算法，它通过在x和y方向应用Prewitt算子来计算图像的梯度" });

            FilterParamaters.Add(new FilterParamater()
            {
                FilterId = "均值滤波器",
                Paramaters = "3,3"
            });
            DetectionParamaters.Add(new DetectionParamater()
            {
                DetectionId = "Canny",
                Paramaters = "50,200",
            });
        }

        public List<Noise> Noises = new List<Noise>();

        public List<WaveFilter> WaveFilters = new List<WaveFilter>();

        public List<EdgeDetection> EdgeDetections = new List<EdgeDetection>();

        public List<FilterParamater> FilterParamaters = new List<FilterParamater>();

        public List<DetectionParamater> DetectionParamaters = new List<DetectionParamater>();

        public Mat MatProcess(ProcessingRule processingRule)
        {
            Mat img = Cv2.ImRead(processingRule.FilePath);

            //灰度处理
            if (processingRule.IsGray)
            {
                Cv2.CvtColor(img, img, ColorConversionCodes.BGR2GRAY);
            }
            if (processingRule.FilterName == "均值滤波器")
            {
                string cs = processingRule.FilterParamater;
                int x = 3;
                int y = 3;
                string[] ps = cs.Split(',');
                x = int.Parse(ps[0]);
                y = int.Parse(ps[1]);
                OpenCvSharp.Size size = new OpenCvSharp.Size(x, y);

                // 进行均值滤波处理
                Cv2.Blur(img, img, size);

            }
            if (processingRule.DetectionName == "Canny")
            {
                string cs = processingRule.DetectionParamater;
                int x = 50;
                int y = 200;
                string[] ps = cs.Split(',');
                x = int.Parse(ps[0]);
                y = int.Parse(ps[1]);
                Cv2.Canny(img, img, x, y);
            }

            return img;
        }

        public Mat MatProcessContours(ProcessingRule processingRule,out List<OutCircle> outCircles)
        {
            Mat org = Cv2.ImRead(processingRule.FilePath);
            Mat img = MatProcess(processingRule);
            //// 查找轮廓
            //Point[][] contours;
            //HierarchyIndex[] hierarchy;
            //Cv2.FindContours(img, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
            


            //// 绘制外接矩形
            //for (int i = 0; i < contours.Length; i++)
            //{
            //    Rect rect = Cv2.BoundingRect(contours[i]);
            //    Cv2.Rectangle(img, rect, Scalar.Red, 2);
            //}

            // 查找轮廓
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(img, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
            outCircles = new List<OutCircle>();
            // 绘制外接圆
            for (int i = 0; i < contours.Length; i++)
            {
                Point2f center;
                float radius;
                Cv2.MinEnclosingCircle(contours[i], out center, out radius);
                outCircles.Add(new OutCircle() { Center=center,Radius=radius});
                //Cv2.Circle(org, center.ToPoint(), (int)radius, Scalar.Yellow, 2);
            }
            outCircles.Sort((x, y) => x.Radius.CompareTo(y.Radius));
            Cv2.Circle(org, outCircles[0].Center.ToPoint(), (int)outCircles[0].Radius, Scalar.Yellow, 2);
            Cv2.Circle(org, outCircles[outCircles.Count-1].Center.ToPoint(), (int)outCircles[outCircles.Count-1].Radius, Scalar.Yellow, 2);
            return org;
        }

    }
}
