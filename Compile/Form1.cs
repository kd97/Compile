using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Compile;

namespace LEX
{
    public partial class Form1 : Form
    {
        Lex c=new Lex();
        //TreeNode topNode;
        Parse  j=new Parse();
        public Form1()
        {
            InitializeComponent();
            c = new Lex();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            int index = textBox1.GetFirstCharIndexOfCurrentLine();
            int line = textBox1.GetLineFromCharIndex(index) + 1;
            int col = textBox1.SelectionStart - index + 1;
            label3.Text = line + " 行，" + col + " 列";
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int index = textBox1.GetFirstCharIndexOfCurrentLine();
            int line = textBox1.GetLineFromCharIndex(index) + 1;
            int col = textBox1.SelectionStart - index + 1;
            label3.Text = line + " 行，" + col + " 列";
        }

        private void button1_Click(object sender, EventArgs e)
        {

            
            var topNode = new TreeNode();
            topNode.Text = "";
            topNode.Text = "程序";
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(topNode);
            String phrase = "";
            String error = "";
            String dfa = "";
            //词法分析
            c.lexexe(textBox1.Text, out phrase, out error,out dfa);
            textBox2.Text = phrase;  //词法分析结果     
            textBox4.Text = "词法分析错误:\r\n";
            textBox4.Text += error;  //出现的错误        
            textBox5.Text = dfa;
            String sen = "";
            List<Information> list = new List<Information>();
            Information first = new Information(0, "开始", "程序");

            list = j.parsexe(phrase, out sen, out error);
            list.Add(first);
            Bind(topNode, list, "程序");
            ArrayList z = new ArrayList();
            z = j.treelist;
            textBox7.Text = "树的先序结构" + "\r\n";
            foreach (String tmp in z)
            {

                textBox7.Text += tmp + "\r\n";
            }
            textBox3.Text = sen;



            textBox4.Text += "\r\n语法分析错误:\r\n";
            textBox4.Text += error;
            String table = "";
            this.dataGridView2.Rows.Clear();
            foreach (DictionaryEntry m in j.products)
            {
                String ks = "";
                ArrayList o = (ArrayList)m.Value;
                foreach (Product k in o)
                {
                    //int index = this.dataGridView2.Rows.Add();
                    table += k.getLeft() + "->";
                    ks = k.getLeft() + "->";
                    //this.dataGridView2.Rows[index].Cells[0].Value = k.getLeft();

                    foreach (String tmp in k.getRight())
                    {
                        table += tmp+" ";
                        ks += tmp + " ";
                        //ks += k.getRight() + "";
                    }
                    //this.dataGridView2.Rows[index].Cells[1].Value = ks;
                    int index = this.dataGridView2.Rows.Add();
                    this.dataGridView2.Rows[index].Cells[0].Value = ks;
                    table += "\t" +"when";
                    ks = "";
                    foreach (String tmp in k.getSelect())
                    {
                        table += "<" + tmp + ">";
                        ks += tmp + " ";
                    }
                    this.dataGridView2.Rows[index].Cells[1].Value = ks;
                    table += "\r\n";
                }



            }


            this.dataGridView1.Rows.Clear();
            Hashtable fuhaobiao = Meaning.fuhaoBiao;
            foreach (DictionaryEntry d in fuhaobiao)
            {
                FuHao fuhao = (FuHao)d.Value;
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells[0].Value = fuhao.name;
                this.dataGridView1.Rows[index].Cells[1].Value = fuhao.width;
                this.dataGridView1.Rows[index].Cells[2].Value = fuhao.offset;
                this.dataGridView1.Rows[index].Cells[3].Value = fuhao.type;
            }
            dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            textBox6.Text = table;





           

            string ac = "";
            string ak = "";
            foreach(DictionaryEntry m in j.nTerminals)
            {

                WordStruct o = (WordStruct)m.Value;
                ac =ak="";
                int index3 = this.dataGridView3.Rows.Add();
                int index4 = this.dataGridView4.Rows.Add();
                this.dataGridView3.Rows[index3].Cells[0].Value = o.getDecri();
                this.dataGridView4.Rows[index4].Cells[0].Value = o.getDecri();
                foreach(string s in o.getFirst())
                {
                    ac += s + " ";


                }
                foreach(string s in o.getFollow())
                {

                    ak += s + " ";
                }
                this.dataGridView3.Rows[index3].Cells[1].Value =ac;
                this.dataGridView4.Rows[index4].Cells[1].Value = ak;

            }





            textBox11.Text = Meaning.yuyi;
            textBox11.Text += (Meaning.line + 1) + "\tend\r\n";//输出end 终结语义分析
            
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
           
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //DialogResult result = openFileDialog1.ShowDialog();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            openFileDialog.Filter = "文本文件|*.txt";
            //openFileDialog.RestoreDirectory = true;
            //openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = File.OpenText(openFileDialog.FileName);
                while (sr.EndOfStream != true)
                    textBox1.Text += sr.ReadLine();
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }


        private ArrayList FindChildren(String name)
        {
            for (int i = 0; i < j.hopelast.father.Count; i++)
            {
                if (j.hopelast.father[i].Equals(name))
                {
                    j.hopelast.father[i] += "s" + i;

                    return (ArrayList)j.hopelast.children[i];
                }

            }
            return null;
        }

        private void Bind(TreeNode parNode, List<Information> list, String nodeName)
        {

            ArrayList childList = new ArrayList();
            childList = FindChildren(nodeName);
            
            if (childList != null)
            {
                foreach (String urlTypese in childList)
                {
                    var node = new TreeNode();
                    
                    node.Text = urlTypese;
                    parNode.Nodes.Add(node);
                    Bind(node, list, urlTypese);
                }
            }
        }

        private void tabPage10_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
