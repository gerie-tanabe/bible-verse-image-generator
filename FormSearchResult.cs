using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Bible_Verses
{
    public partial class FormSearchResult : Form
    {
        string version;
        public FormSearchResult(string _version)
        {
            InitializeComponent();
            version = _version;
        }

        private void FormSearchResult_Load(object sender, EventArgs e)
        {
            
           
        }


        [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
        class MyRegEx : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(args[1]), Convert.ToString(args[0]));
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            Search();
        }


        private void Search()
        {
            SQLiteConnection cnn = new SQLiteConnection(version);
            cnn.Open();

            string[] searchWords = txtSearch.Text.Split(' ');
            string criteria = string.Empty;
            // SQLiteCommand cmd = new SQLiteCommand("Select distinct(chapter) from verse where book_id=@bookID order by chapter", cnn);

            //foreach (string sWord in searchWords)
            //{
            //    if (!string.IsNullOrEmpty(criteria))
            //        criteria += "  AND ";

            //    criteria += " text LIKE '%" + sWord + "%' ";


            //}

            foreach (string sWord in searchWords)
            {
                if (!string.IsNullOrEmpty(criteria))
                    criteria += "  AND ";

                criteria += " (replace(text,':',' ') LIKE '% " + sWord + " %' " +
                    " OR replace(text,'.',' ') LIKE '% " + sWord + " %' " +
                    " OR replace(text,',',' ') LIKE '% " + sWord + " %' " +
                    " OR replace(text,';',' ') LIKE '% " + sWord + " %' " +
                    " OR replace(text,'-',' ') LIKE '% " + sWord + " %' " +
                    " OR text LIKE '" + sWord + " %') ";


            }


            SQLiteCommand cmd = new SQLiteCommand("Select (name || ' ' || chapter || ':' || verse || char(10) || text ) as v from verse left join book on book_reference_id=book_id where " + criteria, cnn);
            //// cmd.Parameters.AddWithValue("@bookID", bookid);
            DataTable dt = new DataTable();
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(dt);
            dgvResults.DataSource = dt;
            toolStripStatusLabel1.Text = dt.Rows.Count.ToString() + " verse/s";
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Search();
            }
        }
    }
}
