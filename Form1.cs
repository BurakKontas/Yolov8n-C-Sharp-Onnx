using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using OpenCvSharp;
using System;
using Size = OpenCvSharp.Size;
using OpenCvSharp.Dnn;
namespace Cat_or_Dog
{
    public partial class Form1 : Form
    {
        private readonly string MODEL_PATH;

        private readonly Dictionary<int, string> CLASSES = new Dictionary<int, string>
        {
            {0, "person"},
            {1, "bicycle"},
            {2, "car"},
            {3, "motorcycle"},
            {4, "airplane"},
            {5, "bus"},
            {6, "train"},
            {7, "truck"},
            {8, "boat"},
            {9, "traffic light"},
            {10, "fire hydrant"},
            {11, "stop sign"},
            {12, "parking meter"},
            {13, "bench"},
            {14, "bird"},
            {15, "cat"},
            {16, "dog"},
            {17, "horse"},
            {18, "sheep"},
            {19, "cow"},
            {20, "elephant"},
            {21, "bear"},
            {22, "zebra"},
            {23, "giraffe"},
            {24, "backpack"},
            {25, "umbrella"},
            {26, "handbag"},
            {27, "tie"},
            {28, "suitcase"},
            {29, "frisbee"},
            {30, "skis"},
            {31, "snowboard"},
            {32, "sports ball"},
            {33, "kite"},
            {34, "baseball bat"},
            {35, "baseball glove"},
            {36, "skateboard"},
            {37, "surfboard"},
            {38, "tennis racket"},
            {39, "bottle"},
            {40, "wine glass"},
            {41, "cup"},
            {42, "fork"},
            {43, "knife"},
            {44, "spoon"},
            {45, "bowl"},
            {46, "banana"},
            {47, "apple"},
            {48, "sandwich"},
            {49, "orange"},
            {50, "broccoli"},
            {51, "carrot"},
            {52, "hot dog"},
            {53, "pizza"},
            {54, "donut"},
            {55, "cake"},
            {56, "chair"},
            {57, "couch"},
            {58, "potted plant"},
            {59, "bed"},
            {60, "dining table"},
            {61, "toilet"},
            {62, "tv"},
            {63, "laptop"},
            {64, "mouse"},
            {65, "remote"},
            {66, "keyboard"},
            {67, "cell phone"},
            {68, "microwave"},
            {69, "oven"},
            {70, "toaster"},
            {71, "sink"},
            {72, "refrigerator"},
            {73, "book"},
            {74, "clock"},
            {75, "vase"},
            {76, "scissors"},
            {77, "teddy bear"},
            {78, "hair drier"},
            {79, "toothbrush"}
        };

        public Form1()
        {
            InitializeComponent();
            MODEL_PATH = @"C:\Users\abura\source\repos\Cat or Dog\Cat or Dog\Assets\yolov8n.onnx";
        }

        private void PredictAsync()
        {
            var onnx = new Onnx(MODEL_PATH, CLASSES);

            var result = onnx.Prediction((Bitmap)pictureBox1.Image);

            pictureBox1.Image = result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UploadImage(pictureBox1);
        }

        private void UploadImage(PictureBox pictureBox, bool resizeBox = false)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                var selectedFile = openFileDialog.FileName;
                pictureBox.Image = System.Drawing.Image.FromFile(selectedFile);
                if (resizeBox) pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Resim yüklenirken bir hata oluştu: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PredictAsync();
        }
    }
}