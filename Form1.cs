﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImageEditor
{

    public partial class Form1 : Form
    {

        public OpenFileDialog opf = new OpenFileDialog();
        public SaveFileDialog sf = new SaveFileDialog();
        public FolderBrowserDialog fbd = new FolderBrowserDialog();
        public Bitmap newBmp = new Bitmap(1, 1);
        Bitmap bmp = new Bitmap(1,1);
        Bitmap Originalbmp = new Bitmap(1, 1);
        public Form1()
        {
            InitializeComponent();
            opf.Filter = "Image Files(*.BMP; *.JPG; *.GIF)| *.BMP; *.JPG; *.GIF | All files(*.*) | *.*";
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            opf.FileOk += Opf_FileOk;
            opf.ShowDialog();
        }

        private void Opf_FileOk(object sender, CancelEventArgs e)
        {
            bmp = new Bitmap(opf.FileName);
            if (bmp.Palette.Entries.Length == 0)
            {
                MessageBox.Show("Error: Image file has no defined Pallete!");
                return;
            }
            Originalbmp = new Bitmap(opf.FileName); 
            pictureBox1.Image = bmp;
            pbOriginal.Image = bmp;
            newBmp = new Bitmap(bmp);
            

            for (int i = 0; i < bmp.Palette.Entries.Length; i++)
            {
                checkedListBox1.Items.Add(bmp.Palette.Entries[i], false);
                listView1.Items.Add("   ");
                listView1.Items[i].BackColor = bmp.Palette.Entries[i];
                
            }
            

            
        }

        private void btnWhite_Click(object sender, EventArgs e)
        {
            bmp = Originalbmp;
            newBmp = new Bitmap(bmp);
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    switchColorInImage((Color)checkedListBox1.Items[i], Color.White);
                }
                else
                { 
                    switchColorInImage((Color)checkedListBox1.Items[i], Color.Black);
                }
            }
        }

       

        private void btnBlack_Click(object sender, EventArgs e)
        {
            bmp = Originalbmp;
            newBmp = new Bitmap(bmp);
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    switchColorInImage((Color)checkedListBox1.Items[i], Color.Black);
                }
                else
                {
                    switchColorInImage((Color)checkedListBox1.Items[i], Color.White);
                }
            }
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Originalbmp;
            bmp = Originalbmp;
            newBmp = new Bitmap(bmp);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        private void cbAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemCheckState(i, cbAll.CheckState);
            }
        }

        private void switchColorInImage(Color original, Color newColor)
        {
            Graphics g2 = pictureBox1.CreateGraphics();
            Graphics g = Graphics.FromImage(newBmp);
            // Set the image attribute's color mappings
            ColorMap[] colorMap = new ColorMap[bmp.Palette.Entries.Length];

            for (int i = 0; i < bmp.Palette.Entries.Length; i++)
            {
                colorMap[i] = new ColorMap();
                colorMap[i].OldColor = bmp.Palette.Entries[i];
                if (colorMap[i].OldColor == original)
                {
                    colorMap[i].NewColor = newColor;
                }

            }

            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);
            // Draw using the color map
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            g.DrawImage(newBmp, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
            pictureBox1.Image = newBmp;




        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sf.ShowDialog();
            newBmp.Save(sf.FileName, ImageFormat.Bmp);
        }

        private void saveAllLayersSeperatlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fbd.ShowDialog();

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                pictureBox1.Image = Originalbmp;
                bmp = Originalbmp;
                newBmp = new Bitmap(bmp);
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

                for (int k = 0; k < checkedListBox1.Items.Count; k++)
                {
                    if (i!=k)
                    {
                        checkedListBox1.SetItemChecked(k, false);
                    }
                    else { checkedListBox1.SetItemChecked(k, true); }

                }
                for (int j = 0; j < checkedListBox1.Items.Count; j++)
                {
                    if (checkedListBox1.GetItemChecked(j))
                    {
                        switchColorInImage((Color)checkedListBox1.Items[j], Color.White);
                    }
                    else
                    {
                        switchColorInImage((Color)checkedListBox1.Items[j], Color.Black);
                    }
                    
                }
                newBmp.Save(fbd.SelectedPath + "\\out_white_" + i + ".bmp");
            }
          
      
        }
    }
}