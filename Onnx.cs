using Cat_or_Dog.Aspects;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using Size = OpenCvSharp.Size;

namespace Cat_or_Dog
{
    public class Onnx
    {
        private readonly Dictionary<int, string> CLASSES;
        private readonly Scalar[] COLORS;
        private readonly Net NET;
        private readonly float CONFIDENCE_THRESHOLD = 0.5f;
        private readonly float NMS_THRESHOLD = 0.4f;
        private readonly int COLUMN_COUNT;

        public Onnx(string modelPath, Dictionary<int, string> classes)
        {
            NET = CvDnn.ReadNetFromOnnx(modelPath)!;
            if (NET == null) throw new ArgumentNullException(nameof(NET));

            CLASSES = classes;
            COLORS = CreateColors(classes.Count);
            COLUMN_COUNT = classes.Count + 4; // 4 is bbox
        }

        //[TimeFixing(350)]
        public Bitmap Prediction(Bitmap bitmap)
        {
            Mat image = bitmap.ToMat();
            int width = image.Width;
            int height = image.Height;

            int length = Math.Max(height, width);
            var scale = length / 640f;

            Mat squareImage = new Mat(length, length, MatType.CV_8UC3, new Scalar(0, 0, 0));

            int xOffset = (length - width) / 2;
            int yOffset = (length - height) / 2;

            image.CopyTo(new Mat(squareImage, new Rect(xOffset, yOffset, width, height)));

            var blob = CvDnn.BlobFromImage(squareImage, 1 / 255f, new Size(640, 640), swapRB: true);
            NET.SetInput(blob);

            Mat outputs = NET.Forward();

            Mat output = outputs.Reshape(1, COLUMN_COUNT, (int)outputs.Total() / COLUMN_COUNT);
            int iterCount = (int)outputs.Total() / COLUMN_COUNT;
            float[,] datas = new float[iterCount, COLUMN_COUNT];

            for (int i = 0; i < iterCount; i++)
            {
                for (int j = 0; j < COLUMN_COUNT; j++)
                {
                    datas[i, j] = output.Get<float>(j, i);
                }
            }

            var boxes = new List<Rect>();
            var scores = new List<float>();
            var classIds = new List<int>();

            for (int i = 0; i < iterCount; i++)
            {
                float[] row = GetRow(datas, i);
                float[] classScores = row.Skip(4).ToArray();
                float maxScore = classScores.Max();
                int maxClassIndex = Array.IndexOf(classScores, maxScore);
                if (maxScore >= CONFIDENCE_THRESHOLD)
                {
                    float[] box = row.Take(4).ToArray();
                    float centerX = box[0];
                    float centerY = box[1];
                    float w = box[2];
                    float h = box[3];

                    // Convert to original image scale
                    Rect bbox = new Rect(
                        (int)((centerX - 0.5 * w) * scale - xOffset),
                        (int)((centerY - 0.5 * h) * scale - yOffset),
                        (int)(w * scale),
                        (int)(h * scale)
                    );

                    // Clip bounding box to image dimensions
                    bbox.X = Math.Max(0, bbox.X);
                    bbox.Y = Math.Max(0, bbox.Y);
                    bbox.Width = Math.Min(width - bbox.X, bbox.Width);
                    bbox.Height = Math.Min(height - bbox.Y, bbox.Height);

                    boxes.Add(bbox);
                    scores.Add(maxScore);
                    classIds.Add(maxClassIndex);
                }
            }

            CvDnn.NMSBoxes(boxes, scores, CONFIDENCE_THRESHOLD, NMS_THRESHOLD, out var indices);

            foreach (var i in indices)
            {
                var index = i;
                Cv2.Rectangle(image, boxes[index], COLORS[classIds[index]], 2);
                var label = $"{CLASSES[classIds[index]]} ({scores[index]:0.00})";
                Cv2.PutText(image, label, new OpenCvSharp.Point(boxes[index].X, boxes[index].Y - 10), HersheyFonts.HersheySimplex, 0.5, COLORS[classIds[index]], 2);
            }

            return image.ToBitmap();
        }


        private Scalar[] CreateColors(int rows)
        {
            Random random = new Random();
            var colors = new Scalar[rows];

            for (int i = 0; i < rows; i++)
            {
                colors[i] = new Scalar(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
            }

            return colors;
        }

        private float[] GetRow(float[,] array, int row)
        {
            int cols = array.GetLength(1);
            float[] rowArray = new float[cols];
            for (int i = 0; i < cols; i++)
            {
                rowArray[i] = array[row, i];
            }
            return rowArray;
        }

        private float[,] ToMatArray(Mat data, int rows, int cols)
        {
            var matArray = new float[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matArray[i, j] = data.At<float>(i, j);
                }
            }
            return matArray;
        }
    }
}
