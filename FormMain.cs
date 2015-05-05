using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data.SQLite;
using System.IO;

namespace Bible_Verses
{
    public partial class FormMain : Form
    {
        string version;
        Bitmap bm;
        Color titleColor = Color.Black;
        Color verseColor = Color.Black;
        Color bgColor = Color.White;
        Font titleFont = new Font("Arial", 16, FontStyle.Bold);
        Font verseFont = new Font("Arial", 16);
        StringFormat stringFormat = new StringFormat();
        Size sizex = new Size(512, 512);
        Rectangle titleRect;
        Image img = null;


        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        public void loadVersions()
        {
            string[] files = System.IO.Directory.GetFiles(Application.StartupPath + "\\Bibles\\");
            for(int i=0;i<files.Length;i++)
            {
                files[i]=Path.GetFileNameWithoutExtension(files[i]);
            }

            this.cmbVersion.Items.AddRange(files);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadVersions();
            //return;
            cmbVersion.SelectedIndex = 0;
            stringFormat.Alignment = StringAlignment.Center;      // Horizontal Alignment
            stringFormat.LineAlignment = StringAlignment.Center;
            loadBooks();
            cmbAlignment.SelectedIndex = 0;
            cmbTitleValign.SelectedIndex = 0;
            cmbVersion.SelectedIndex = 0;

        }

        private void button2_Click(object sender, EventArgs e)
        {

       }

        public void displayImage()
        {
            int bookValue, chapterValue, startValue, endValue;
            int.TryParse(cmbBooks.ComboBox.SelectedValue.ToString(), out bookValue);
            int.TryParse(this.cmbChapters.ComboBox.SelectedValue.ToString(), out chapterValue);
            int.TryParse(cmbStart.ComboBox.SelectedValue.ToString(), out startValue);
            int.TryParse(this.cmbEnd.ComboBox.SelectedValue.ToString(), out endValue);

            bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            //Image img = Image.FromFile("1.jpg");

            Graphics g = Graphics.FromImage(bm);
            g.Clear(bgColor);

            String drawString = getVerses(bookValue, chapterValue, startValue, endValue);
            String title;
            if (cmbEnd.Text != cmbStart.Text)
            {
                title = cmbBooks.Text + " " + cmbChapters.Text + ":" + cmbStart.Text + "-" + cmbEnd.Text;
            }
            else
            {
                title = cmbBooks.Text + " " + cmbChapters.Text + ":" + cmbStart.Text;
            }

            SolidBrush drawBrush = new SolidBrush(titleColor);
            SolidBrush verseBrush = new SolidBrush(verseColor);
            PointF drawPoint = new PointF(150.0F, 150.0F);
            Rectangle drawRect = new Rectangle(50, 100, bm.Width - 100, bm.Height - 150);
            Rectangle drawRect2 = new Rectangle(0, 0, bm.Width, bm.Height);

            //g.DrawRectangle(Pens.Black, titleRect);


            if (img!=null)
            {
                g.DrawImage(img, drawRect2);
            }
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawString(title, titleFont, drawBrush, titleRect, stringFormat);

            g.DrawString(drawString, verseFont, verseBrush, drawRect);
            pictureBox1.Image = bm;
        
        }

        public string getVerses(int book_id, int chapter, int versestart, int verseend)
        {
            SQLiteConnection cnn = new SQLiteConnection(version);
            cnn.Open();

            SQLiteCommand cmd = new SQLiteCommand("Select text from verse where book_id=@bookid and chapter=@chapter and verse between @verseStart and @verseEnd", cnn);
            cmd.Parameters.AddWithValue("@bookid", book_id);
            cmd.Parameters.AddWithValue("@chapter", chapter);
            cmd.Parameters.AddWithValue("@verseStart", versestart);
            cmd.Parameters.AddWithValue("@verseEnd", verseend);
            SQLiteDataReader r = cmd.ExecuteReader();


            string verses = string.Empty;

            while (r.Read())
            {
                verses += r["text"].ToString() + "\r\n\r\n";

            }

            return verses;
        }


        public void loadBooks()
        {
            SQLiteConnection cnn = new SQLiteConnection(version);
            try
            {
                cnn.Open();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }

            SQLiteCommand cmd = new SQLiteCommand("Select name,id from book order by id", cnn);
            DataTable dt = new DataTable();
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(dt);


            cmbBooks.ComboBox.ValueMember = "id";
            cmbBooks.ComboBox.DisplayMember = "name";
            cmbBooks.ComboBox.DataSource = dt;


        }

        public void loadChapters(int bookid)
        {
            SQLiteConnection cnn = new SQLiteConnection(version);
            cnn.Open();

            SQLiteCommand cmd = new SQLiteCommand("Select distinct(chapter) from verse where book_id=@bookID order by chapter", cnn);
            cmd.Parameters.AddWithValue("@bookID", bookid);
            DataTable dt = new DataTable();
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(dt);

            cmbChapters.ComboBox.ValueMember = "chapter";
            cmbChapters.ComboBox.DisplayMember = "chapter";
            cmbChapters.ComboBox.DataSource = dt;


        }

        public void loadVerses(int bookid, int chapter)
        {
            SQLiteConnection cnn = new SQLiteConnection(version);
            cnn.Open();

            SQLiteCommand cmd = new SQLiteCommand("Select distinct(verse) from verse where book_id=@bookID and chapter=@chapter order by verse", cnn);
            cmd.Parameters.AddWithValue("@bookID", bookid);
            cmd.Parameters.AddWithValue("@chapter", chapter);

            DataTable dt = new DataTable();

            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(dt);

            cmbStart.ComboBox.ValueMember = "verse";
            cmbStart.ComboBox.DisplayMember = "verse";
            cmbStart.ComboBox.DataSource = dt;

        }

        public void loadVerses2(int bookid, int chapter, int startVerse)
        {
            SQLiteConnection cnn = new SQLiteConnection(version);
            cnn.Open();

            SQLiteCommand cmd = new SQLiteCommand("Select distinct(verse) from verse where book_id=@bookID and chapter=@chapter and verse >= @startVerse order by verse", cnn);
            cmd.Parameters.AddWithValue("@bookID", bookid);
            cmd.Parameters.AddWithValue("@chapter", chapter);
            cmd.Parameters.AddWithValue("@startVerse", startVerse);

            DataTable dt = new DataTable();

            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(dt);

            this.cmbEnd.ComboBox.ValueMember = "verse";
            this.cmbEnd.ComboBox.DisplayMember = "verse";
            cmbEnd.ComboBox.DataSource = dt;


        }


        private void cmbBooks_SelectedIndexChanged(object sender, EventArgs e)
        {
            int value;
            int.TryParse(cmbBooks.ComboBox.SelectedValue.ToString(), out value);
            loadChapters(value);
        }

        private void cmbChapters_SelectedIndexChanged(object sender, EventArgs e)
        {

            int value, value2;
            int.TryParse(cmbBooks.ComboBox.SelectedValue.ToString(), out value);
            int.TryParse(this.cmbChapters.ComboBox.SelectedValue.ToString(), out value2);
            loadVerses(value, value2);
        }

        private void cmbStart_SelectedIndexChanged(object sender, EventArgs e)
        {

            int value, value2, value3;
            int.TryParse(cmbBooks.ComboBox.SelectedValue.ToString(), out value);
            int.TryParse(this.cmbChapters.ComboBox.SelectedValue.ToString(), out value2);
            int.TryParse(this.cmbStart.ComboBox.SelectedValue.ToString(), out value3);
            loadVerses2(value, value2, value3);
            //displayImage();
        }

        private void btnSavePicture_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            String title;
            if (cmbEnd.Text != cmbStart.Text)
            {
                title = cmbBooks.Text + " " + cmbChapters.Text + "-" + cmbStart.Text + "-" + cmbEnd.Text;
            }
            else
            {
                title = cmbBooks.Text + " " + cmbChapters.Text + "-" + cmbStart.Text;
            }

            saveDialog.Filter = "Png Image (*.png)|*.png|Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|Jpg Image (*.jpg)|*.jpg";
            saveDialog.FileName = title + ".png";

            //DialogResult result= saveDialog.ShowDialog();
            // string filename = saveDialog.FileName;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveDialog.FileName);
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                titleColor = colorDialog.Color;
                btnColor.BackColor = colorDialog.Color;
                displayImage();
            }
        }

        private void btnBGColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                bgColor = colorDialog.Color;
                btnBGColor.BackColor = colorDialog.Color;
                displayImage();
            }
        }

        private void btnVerseColor_Click(object sender, EventArgs e)
        {

            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                verseColor = colorDialog.Color;
                btnVerseColor.BackColor = colorDialog.Color;
                displayImage();
            }
        }

        private void cmbEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayImage();
        }

        private void btnTitleFont_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                titleFont = fontDialog.Font;

                //Font tmpFont = new Font(fontDialog.Font.Name, 8);
                //btnTitleFont.Font = tmpFont;
               
                displayImage(); 
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                verseFont = fontDialog.Font;

                //Font tmpFont = new Font(fontDialog.Font.Name, 8);
                //btnVerseFont.Font = fontDialog.Font;

                displayImage();
            }
        }

        private void btnBackgroundImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog imgDialog = new OpenFileDialog();
            imgDialog.Filter="Images |*.jpeg;*.png;*.jpg;*.gif";
            if (imgDialog.ShowDialog() == DialogResult.OK)
            {
               
                img = Image.FromFile(imgDialog.FileName);
                displayImage();
            }
        }

        private void comboAlignment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAlignment.Text == "Left")
            {
                stringFormat.Alignment = StringAlignment.Near;      
               // stringFormat.LineAlignment = StringAlignment.Near;
            }
            else if(cmbAlignment.Text == "Right")
            {
                
                stringFormat.Alignment = StringAlignment.Far;      
               // stringFormat.LineAlignment = StringAlignment.Far;
            }
            else
            {
                stringFormat.Alignment = StringAlignment.Center;      
               // stringFormat.LineAlignment = StringAlignment.Center;
            }
            displayImage();
        }

        private void cmbTitleValign_SelectedIndexChanged(object sender, EventArgs e)
        {
           // MessageBox.Show(this.pictureBox1.Width.ToString());
            if (cmbTitleValign.Text == "Top")
            {
                titleRect = new Rectangle(50, 50, this.pictureBox1.Width-100,50);
            }
            else
            {
                titleRect = new Rectangle(50, 450, this.pictureBox1.Width - 100, 50);
            }
            displayImage();
        }

        private void cmbVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string book = this.cmbVersion.Text;
            version = "Data Source = " + Application.StartupPath + "\\Bibles\\" + book + ".sqlite";
            loadBooks();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FormSearchResult formSearchResult = new FormSearchResult(version);
            formSearchResult.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To God be the glory.");
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void cmbBooks_Click(object sender, EventArgs e)
        {

        }
        
   }
}

