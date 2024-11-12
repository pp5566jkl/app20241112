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
                openFileDialog1.Filter = "�Ϲ����(JPeg, Gif, Bmp, etc.)|.jpg;*jpeg;*.gif;*.bmp;*.tif;*.tiff;*.png|�Ҧ����(*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Bitmap MyBitmap = new Bitmap(openFileDialog1.FileName);
                    this.pictureBox1.Image = MyBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "�T�����");
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
                MessageBox.Show(ex.Message, "�T�����");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;

                // �w�q Prewitt �M Sobel �� Gx, Gy �B�n
                int[,] PrewittGx = { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
                int[,] PrewittGy = { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } };

                int[,] SobelGx = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                int[,] SobelGy = { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

                // �Ыؤ@�ӷs�� Bitmap �ΨӦs����t�˴����G
                Bitmap edgeBitmap = new Bitmap(Width, Height);

                // �ΨӫO�s�C�ӹ�������t�j��
                List<int> edgeValues = new List<int>();

                // ��ܨϥ� Prewitt ��t�˴� (�p�G�Q�� Sobel �i�H���� Gx, Gy �B�n)
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        // �p��C�ӹ������Ƕ���
                        int pixelValue = (int)((oldBitmap.GetPixel(x, y).R * 0.299 + oldBitmap.GetPixel(x, y).G * 0.587 + oldBitmap.GetPixel(x, y).B * 0.114));

                        // �p�� Gx �M Gy (��� Prewitt ��t�˴�)
                        int Gx = 0, Gy = 0;

                        // �p�� Gx �M Gy
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                int gray = (int)((oldBitmap.GetPixel(x + i, y + j).R * 0.299 + oldBitmap.GetPixel(x + i, y + j).G * 0.587 + oldBitmap.GetPixel(x + i, y + j).B * 0.114));
                                Gx += PrewittGx[i + 1, j + 1] * gray; // �ϥ� Prewitt �� Gx
                                Gy += PrewittGy[i + 1, j + 1] * gray; // �ϥ� Prewitt �� Gy
                            }
                        }

                        // �p����t�j�� (Magnitude)
                        double magnitude = Math.Sqrt(Gx * Gx + Gy * Gy);

                        // �N��t�j�צs�J�C���A�y��i�楿�W��
                        edgeValues.Add((int)magnitude);
                    }
                }

                // �p����t�j�ת��̤p�ȩM�̤j��
                int minEdgeValue = edgeValues.Min();
                int maxEdgeValue = edgeValues.Max();

                // ���W�ƨñN���G�M�g�� 0 �� 255 ���d��
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        // �p��C�ӹ������Ƕ���
                        int pixelValue = (int)((oldBitmap.GetPixel(x, y).R * 0.299 + oldBitmap.GetPixel(x, y).G * 0.587 + oldBitmap.GetPixel(x, y).B * 0.114));

                        // �p�� Gx �M Gy
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

                        // �p����t�j�� (Magnitude)
                        double magnitude = Math.Sqrt(Gx * Gx + Gy * Gy);

                        // ���W����t�j�ר� 0-255 �d��
                        int normEdge = (int)((magnitude - minEdgeValue) * 255.0 / (maxEdgeValue - minEdgeValue));

                        // �N���W�ƫ᪺��t�j�׳]���ӹ������Ƕ���
                        edgeBitmap.SetPixel(x, y, Color.FromArgb(normEdge, normEdge, normEdge));
                    }
                }

                // ��ܸg�L���W�ƫ᪺��t�˴����G
                this.pictureBox2.Image = edgeBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "�T�����");
            }
        }
    }
}