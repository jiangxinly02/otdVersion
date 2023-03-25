using Newtonsoft.Json;
using OpenCvSharp;
using otdVision.Entity;
using otdVision.Service;

namespace otdVisionTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       
        VisionProcessor visionProcessor = new VisionProcessor();
        private void Form1_Load(object sender, EventArgs e)
        {

            
            if (!grayBox.Checked)
            {
                grayBox.Checked= true;
            }
            foreach (var wave in visionProcessor.WaveFilters)
            {
                cbxFilter.Items.Add(wave.FilterId);
            }
            foreach (var edge in visionProcessor.EdgeDetections)
            {
                cbxEdge.Items.Add(edge.DetectionId);
            }

        }

        private void btnNoise_Click(object sender, EventArgs e)
        {
            txtMsg.Text = JsonConvert.SerializeObject(visionProcessor.Noises, Formatting.Indented);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtMsg.Text = JsonConvert.SerializeObject(visionProcessor.WaveFilters, Formatting.Indented);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtMsg.Text = JsonConvert.SerializeObject(visionProcessor.EdgeDetections, Formatting.Indented);
        }

        private void btnLook_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ofd.Title = "请选择要导入的excel";
            ofd.Filter = "所有文件|*.jpg";
            ofd.ShowDialog();
            string path = ofd.FileName;

            ProcessingRule processingRule = new ProcessingRule()
            {
                FilePath = path,
                IsGray = grayBox.Checked,
                DetectionName = cbxEdge.Text,
                DetectionParamater = txtEdge.Text,
                FilterName = cbxFilter.Text,
                FilterParamater = txtFilter.Text
            };

            
            Mat img = visionProcessor.MatProcess(processingRule);
            
            

            
            Cv2.ImShow("Image", img);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();

        }

        private void cbxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var parameter in visionProcessor.FilterParamaters)
            {
                if(parameter.FilterId== "均值滤波器")
                {
                    txtFilter.Text = parameter.Paramaters;
                    txtFilter.Focus();
                    txtFilter.SelectAll();
                    return;
                }
            }
            
        }


        private void cbxEdge_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var parameter in visionProcessor.DetectionParamaters)
            {
                if (parameter.DetectionId == "Canny")
                {
                    txtEdge.Text = parameter.Paramaters;
                    txtEdge.Focus();
                    txtEdge.SelectAll();
                    return;
                }
            }
        }
        List<OutCircle> outCircles;
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbxEdge.Text))
            {
                MessageBox.Show("请选择边缘检测算法！");
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ofd.Title = "请选择要导入的excel";
            ofd.Filter = "所有文件|*.jpg";
            ofd.ShowDialog();
            string path = ofd.FileName;

            ProcessingRule processingRule = new ProcessingRule()
            {
                FilePath = path,
                IsGray = grayBox.Checked,
                DetectionName = cbxEdge.Text,
                DetectionParamater = txtEdge.Text,
                FilterName = cbxFilter.Text,
                FilterParamater = txtFilter.Text
            };

            
            Mat img = visionProcessor.MatProcessContours(processingRule,out outCircles);

            MessageBox.Show("已定位最大外接圆中心点("+ outCircles[outCircles.Count-1].Center.X.ToString()+","+
                outCircles[outCircles.Count - 1].Center.Y.ToString()+")和最小外接圆中心点("+ outCircles[0].Center.X.ToString() + "," +
                outCircles[0].Center.Y.ToString()+")");


            Cv2.ImShow("Image", img);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (outCircles is null)
            {
                MessageBox.Show("请先定位正确位置！");
                return;
            }
            if (string.IsNullOrEmpty(cbxEdge.Text))
            {
                MessageBox.Show("请选择边缘检测算法！");
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ofd.Title = "请选择要导入的excel";
            ofd.Filter = "所有文件|*.jpg";
            ofd.ShowDialog();
            string path = ofd.FileName;

            ProcessingRule processingRule = new ProcessingRule()
            {
                FilePath = path,
                IsGray = grayBox.Checked,
                DetectionName = cbxEdge.Text,
                DetectionParamater = txtEdge.Text,
                FilterName = cbxFilter.Text,
                FilterParamater = txtFilter.Text
            };

            List<OutCircle> newOutCircles;
            Mat img = visionProcessor.MatProcessContours(processingRule, out newOutCircles);
            Point2f point1 = new Point2f()
            {
                X = outCircles[0].Center.X - outCircles[outCircles.Count - 1].Center.X,
                Y = (outCircles[0].Center.Y - outCircles[outCircles.Count - 1].Center.Y)
            };

            Point2f point2 = new Point2f()
            {
                X = newOutCircles[0].Center.X - newOutCircles[newOutCircles.Count - 1].Center.X,
                Y = (newOutCircles[0].Center.Y - newOutCircles[newOutCircles.Count - 1].Center.Y)
            };
            double length1 = Math.Sqrt(Math.Pow(point1.X, 2) + Math.Pow(point1.Y, 2));
            double length2 = Math.Sqrt(Math.Pow(point2.X, 2) + Math.Pow(point2.Y, 2));
            double theta1 = Math.Acos(point1.X / Math.Sqrt(point1.X * point1.X + point1.Y * point1.Y));
            double angle1 = theta1 * 180 / Math.PI;

            double theta2 = Math.Acos(point2.X / Math.Sqrt(point2.X * point2.X + point2.Y * point2.Y));
            double angle2 = theta2 * 180 / Math.PI;


            
            MessageBox.Show("请向X轴移动[" + (outCircles[outCircles.Count-1].Center.X- newOutCircles[newOutCircles.Count-1].Center.X).ToString()+"]向Y轴移动["+
                (outCircles[outCircles.Count - 1].Center.Y - newOutCircles[newOutCircles.Count - 1].Center.Y).ToString()+
                "],然后顺时针旋转["+(angle2-angle1).ToString()+"]°,误差为["+(((length2-length1)/length1)*100).ToString()+ "%]");
        }
    }
}