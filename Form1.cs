using System.Windows.Forms;

namespace app20241112
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "圖像文件(JPeg, Gif, Bmp, etc.)|.jpg;*jpeg;*.gif;*.bmp;*.tif;*.tiff;*.png|所有文件(*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Bitmap MyBitmap = new Bitmap(openFileDialog1.FileName);
                    this.pictureBox1.Image = MyBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息顯示");
            }

            try
            {
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        pixel = oldBitmap.GetPixel(x, y);
                        int r, g, b, Result = 0;
                        r = pixel.R;
                        g = pixel.G;
                        b = pixel.B;
                        Result = (299 * r + 587 * g + 114 * b) / 1000;
                        newBitmap.SetPixel(x, y, Color.FromArgb(Result, Result, Result));
                    }
                this.pictureBox1.Image = newBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息顯示");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;

                // 定義 Prewitt 和 Sobel 的 Gx, Gy 遮罩
                int[,] PrewittGx = { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
                int[,] PrewittGy = { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } };

                int[,] SobelGx = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                int[,] SobelGy = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

                // 創建一個新的 Bitmap 用來存放邊緣檢測結果
                Bitmap edgeBitmap = new Bitmap(Width, Height);

                // 用來保存每個像素的邊緣強度
                List<int> edgeValues = new List<int>();

                // 選擇使用 Prewitt 邊緣檢測 (如果想用 Sobel 可以替換 Gx, Gy 遮罩)
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        // 計算每個像素的灰階值
                        int pixelValue = (int)((oldBitmap.GetPixel(x, y).R * 0.299 + oldBitmap.GetPixel(x, y).G * 0.587 + oldBitmap.GetPixel(x, y).B * 0.114));

                        // 計算 Gx 和 Gy (選擇 Prewitt 邊緣檢測)
                        int Gx = 0, Gy = 0;

                        // 計算 Gx 和 Gy
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                int gray = (int)((oldBitmap.GetPixel(x + i, y + j).R * 0.299 + oldBitmap.GetPixel(x + i, y + j).G * 0.587 + oldBitmap.GetPixel(x + i, y + j).B * 0.114));
                                Gx += PrewittGx[i + 1, j + 1] * gray; // 使用 Prewitt 的 Gx
                                Gy += PrewittGy[i + 1, j + 1] * gray; // 使用 Prewitt 的 Gy
                            }
                        }

                        // 計算邊緣強度 (Magnitude)
                        double magnitude = Math.Sqrt(Gx * Gx + Gy * Gy);

                        // 將邊緣強度存入列表中，稍後進行正規化
                        edgeValues.Add((int)magnitude);
                    }
                }

                // 計算邊緣強度的最小值和最大值
                int minEdgeValue = edgeValues.Min();
                int maxEdgeValue = edgeValues.Max();

                // 正規化並將結果映射到 0 到 255 的範圍
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        // 計算每個像素的灰階值
                        int pixelValue = (int)((oldBitmap.GetPixel(x, y).R * 0.299 + oldBitmap.GetPixel(x, y).G * 0.587 + oldBitmap.GetPixel(x, y).B * 0.114));

                        // 計算 Gx 和 Gy
                        int Gx = 0, Gy = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                int gray = (int)((oldBitmap.GetPixel(x + i, y + j).R * 0.299 + oldBitmap.GetPixel(x + i, y + j).G * 0.587 + oldBitmap.GetPixel(x + i, y + j).B * 0.114));
                                Gx += PrewittGx[i + 1, j + 1] * gray;
                                Gy += PrewittGy[i + 1, j + 1] * gray;
                            }
                        }

                        // 計算邊緣強度 (Magnitude)
                        double magnitude = Math.Sqrt(Gx * Gx + Gy * Gy);

                        // 正規化邊緣強度到 0-255 範圍
                        int normEdge = (int)((magnitude - minEdgeValue) * 255.0 / (maxEdgeValue - minEdgeValue));

                        // 將正規化後的邊緣強度設為該像素的灰階值
                        edgeBitmap.SetPixel(x, y, Color.FromArgb(normEdge, normEdge, normEdge));
                    }
                }

                // 顯示經過正規化後的邊緣檢測結果
                this.pictureBox2.Image = edgeBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息顯示");
            }
        }
    }
}