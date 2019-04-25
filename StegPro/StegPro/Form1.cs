using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Drawing.Imaging;

namespace StegPro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public byte[] textfile;
        public byte[] tmpBytes;
        public int i, j = 0, k=0, l=0;
        public string textFileName;
        public string imageFileName;
        //HIDE
         
        private void hide_Click(object sender, EventArgs e)
        {
            //SELECT IMAGE TO HIDE TEXT 
            MessageBox.Show("Select image to hide text");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                
                pictureBox1.Image = new Bitmap(openFileDialog.FileName);
                textBox1.Text += "To hide the text:" + openFileDialog.FileName + Environment.NewLine;
            }

            //SELECT THE TEXT FILE TO HIDE IN IMAGE
            MessageBox.Show("Select text file to hide");
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                
                textBox1.Text += "Text file to be embedded in the image:" + openFileDialog.FileName + Environment.NewLine;
                string[] tmp = openFileDialog.FileName.Split('\\');
                textFileName = tmp[tmp.Length - 1];
                if(!string.IsNullOrEmpty(vigenerekey.Text) && checkKey(vigenerekey)==true)
                {

                    Vigenere.encrypt(openFileDialog.FileName, vigenerekey.Text);
                    textfile = File.ReadAllBytes(@"C:\ProgramData\stegpro_encrypted.txt");
                    File.Delete(@"C:\ProgramData\stegpro_encrypted.txt");   
                }
                else
                {
                    textfile = File.ReadAllBytes(openFileDialog.FileName);
                }
                
            }

            //GET IMAGE SIZE AND PIXELS

            Bitmap bmp = new Bitmap(pictureBox1.Image);
            int x = bmp.Width;
            int y = bmp.Height;

            byte[] r = new byte[x * y];
            byte[] g = new byte[x * y];
            byte[] b = new byte[x * y];
            k = 0;

            for (i = 0; i < y; i++)
            {
                for (j = 0; j < x; j++)
                {
                    r[k] = bmp.GetPixel(j, i).R;
                    g[k] = bmp.GetPixel(j, i).G;
                    b[k] = bmp.GetPixel(j, i).B;
                    k++;
                }
            }

            //CHANGE BITS BETWEEN TEXT FILE BITS AND IMAGE PIXELS LSB BITS
            BitArray textBits = new BitArray(textfile);
            i = 0; j = 0;
            changeBits(r, textBits);
            changeBits(g, textBits);
            changeBits(b, textBits);

            Bitmap newImage = (Bitmap)bmp.Clone();
            x = bmp.Width;
            y = bmp.Height;
            k = 0;
            Color rgb;
            for (i = 0; i < y; i++)
            {
                for (j = 0; j < x; j++)
                {
                    rgb = Color.FromArgb((int)r[k], (int)g[k], (int)b[k]);
                    newImage.SetPixel(j, i, rgb);
                    k++;
                }
            }
            textBox1.Text += "The text file is embedded in the image." + Environment.NewLine;

            //SAVE IMAGE TO CUSTOM FOLDER
            pictureBox2.Image = newImage;
            var savePath = new FolderBrowserDialog();
            MessageBox.Show("Select folder to save the new image");
            if (savePath.ShowDialog() == DialogResult.OK)
            {
                string folderName = savePath.SelectedPath;
                pictureBox2.Image.Save(folderName + "\\newImage.png", ImageFormat.Png);
            }
            textBox1.Text += "The new image is saved to "+savePath.SelectedPath+"\\newImage.png" + Environment.NewLine;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            results(textBox1);
        }

//EXTRACT
        private void extract_Click(object sender, EventArgs e)
        {
            //SELECT IMAGE TO EXTRACT THE TEXT 
            MessageBox.Show("Select image to extract the text");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image = new Bitmap(openFileDialog.FileName);
                results(textBox1);
                textBox1.Text += "The text will be extract from this image:"+ openFileDialog.FileName + Environment.NewLine;
            }

            //GET IMAGE PIXELS
            Bitmap bmp = new Bitmap(pictureBox2.Image);
            int x = bmp.Width;
            int y = bmp.Height;

            byte[] r = new byte[x * y];
            byte[] g = new byte[x * y];
            byte[] b = new byte[x * y];
            k = 0;

            for (i = 0; i < y; i++)
            {
                for (j = 0; j < x; j++)
                {
                    r[k] = bmp.GetPixel(j, i).R;
                    g[k] = bmp.GetPixel(j, i).G;
                    b[k] = bmp.GetPixel(j, i).B;
                    k++;
                }
            }

            //EXTRACT ALL BITS
            BitArray extractBits = new BitArray(8);
            k = 0;
            l = 0;
            int length = (r.Length + g.Length + b.Length) / 8;
            tmpBytes = new byte[length];

            extractAllBits(r, extractBits);
            extractAllBits(g, extractBits);
            extractAllBits(b, extractBits);


            //BIT ARRAY TO BYTE ARRAY
            byte[] extractBytes = new byte[l];
            for (int i = 0; i < extractBytes.Length; i++)
            {
                extractBytes[i] = tmpBytes[i];
            }
            
            //VIGENERE DECRYPTION
            if(!string.IsNullOrEmpty(vigenerekey.Text) && checkKey(vigenerekey)==true)
            {
                Vigenere.decrypt(extractBytes, vigenerekey.Text);
            }

            //SAVE TEXT FILE TO CUSTOM FOLDER
            var savePath = new FolderBrowserDialog();
            MessageBox.Show("Select folder to save the extracted text file");
            if (savePath.ShowDialog() == DialogResult.OK)
            {
                string folderName = savePath.SelectedPath;
                File.WriteAllBytes(folderName + "\\extracted_text.txt", extractBytes);
            }
            textBox1.Text += "The extracted file saved to " + savePath.SelectedPath + "\\extracted_text.txt" + Environment.NewLine;
        }

        public void changeBits(byte[] rgb, BitArray textBits)
        {
            for (i = 0; i < rgb.Length; i++)
            {
                if (j < textBits.Length)
                {

                    if (rgb[i] % 2 == 0 && textBits[j] == true)
                    {
                        rgb[i]++;
                    }
                    else
                     if (rgb[i] % 2 == 1 && textBits[j] == false)
                    {
                        rgb[i]--;
                    }

                    j++;
                }
                else
                if (j >= textBits.Length)
                {

                    if (rgb[i] % 2 == 1)
                    {
                        rgb[i]--;
                    }

                }


            }
        }
        public void extractAllBits(byte[] rgb, BitArray extractBits)
        {
            for (int i = 0; i < rgb.Length; i++)
            {

                if (k == 8)
                {

                    extractBits.CopyTo(tmpBytes, l);
                    if (tmpBytes[l] == 0)//IF(INDEX==NULL)
                    {
                        break;
                    }
                    k = 0;
                    l++;
                }
                BitArray textBitsR = new BitArray(new byte[] { rgb[i] });
                extractBits[k] = textBitsR[0];

                k++;
            }
        }
        public void results(TextBox txt)
        {
            txt.Text = "STEGPRO - Image Steganography Tool" + Environment.NewLine;
            txt.Text += "Ramin KARIMKHANI - Twitter: @ramin_karimhani" + Environment.NewLine + Environment.NewLine;
        }
        public bool checkKey(TextBox txt)
        {
            bool b=true;
            if(!string.IsNullOrEmpty(txt.Text))
            {
                foreach(char c in txt.Text)
                {
                    if((c>=97 && c<=122) || (c >= 65 && c <= 90))
                    {
                        continue;
                    }
                    else
                    {
                        b = false;
                        MessageBox.Show("The key must consist of only English characters.");
                        textBox1.Text += "Warning: The key was not entered properly. The data will be embedded/extracted without the key being used." + Environment.NewLine;
                        break;
                    }
                }
            }
            return b;
        }



    


    }
}
