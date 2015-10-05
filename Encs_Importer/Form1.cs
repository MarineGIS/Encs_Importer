/*
Copyright [2015] [DSU_ITC]

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Encs_Importer.ISO_IEC_8211;
using Encs_Importer.ENC_Builder;
using System.Xml.Serialization;
using Encs_Importer.Factory;

namespace Encs_Importer
{
    public partial class Form1 : Form
    {
        DataTable dataTable;
        string DirectoryRootPoint;
        string DirectoryPoint;
        public Form1()
        {
            InitializeComponent();

            DirectoryRootPoint = Application.StartupPath + @"\ENC_ROOT";
            DirectoryPoint = DirectoryRootPoint;
            dataTable = new DataTable("DataT");
            dataTable.Columns.Add("FileName", typeof(string));

            DataRow row = dataTable.NewRow();

           // row["FileName"] = Directory.GetDirectoryRoot(DirectoryPoint);
            row["FileName"] = DirectoryRootPoint;
            dataTable.Rows.Add(row);

            comboBox1.DataSource = dataTable;
            comboBox1.DisplayMember = "FileName";
            GetFile(DirectoryPoint);
          
        }
        private void GetFile(string path)
        {
            listView1.Items.Clear();
            string[] files = Directory.GetFiles(path);
            string[] directories = Directory.GetDirectories(path);
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                ListViewItem item = new ListViewItem(new string[] { 
                        info.Name,info.Length.ToString(),info.Extension,info.LastWriteTime.ToString()
                    });
                listView1.Items.Add(item);
            }

            foreach (string directoryName in directories)
            {
                DirectoryInfo info = new DirectoryInfo(directoryName);
                ListViewItem item = new ListViewItem(new string[] { 
                        info.Name,"","dir",info.LastWriteTime.ToString()
                    });
                listView1.Items.Add(item);
            }
         
        }
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
        
            int selectedIndex = comboBox1.SelectedIndex;
            DataRow row = ((DataRowView)comboBox1.SelectedItem).Row;
            DirectoryPoint = row["FileName"].ToString();
            Console.WriteLine(DirectoryPoint);

            GetFile(DirectoryPoint);
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            string root = DirectoryPoint;
            dataTable.Clear();
            string[] directory = Directory.GetDirectories(DirectoryPoint);
            DataRow rootRow = dataTable.NewRow();
            rootRow["FileName"] = root;
            dataTable.Rows.Add(rootRow);

            foreach (var item in directory)
            {
                DataRow row = dataTable.NewRow();
                row["FileName"] = item;
                dataTable.Rows.Add(row);
            }
            comboBox1.DataSource = dataTable;
            comboBox1.DisplayMember = "FileNames";
        }

        private void button2_Click(object sender, EventArgs e) 
        {
            if (DirectoryPoint == DirectoryRootPoint)
                return;
            DirectoryPoint = DirectoryPoint.Substring(0, DirectoryPoint.Length - 1);
            DirectoryPoint = DirectoryPoint.Substring(0, DirectoryPoint.LastIndexOf('\\')+1);
            comboBox1.Text = DirectoryPoint;
            GetFile(DirectoryPoint);
        } // 데이터 폴더 설정

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string targetName = listView1.SelectedItems[0].SubItems[0].Text;
            string targetType = listView1.SelectedItems[0].SubItems[2].Text;

            if (targetType == "dir")
            {
                DirectoryPoint = DirectoryPoint.Substring(0, DirectoryPoint.Length);
                DirectoryPoint = DirectoryPoint + "\\" + targetName +"\\";
                comboBox1.Text = DirectoryPoint;
                GetFile(DirectoryPoint);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                int index = listView1.FocusedItem.Index;
                string fileName = listView1.Items[index].SubItems[0].Text;
                string fn_extension = listView1.Items[index].SubItems[2].Text;
                if (!fn_extension.Equals(".000"))
                    return;
                string path = comboBox1.Text.ToString();
                string fullPath = path +"\\"+ fileName;

                Factory.BinaryReader reader = new Factory.BinaryReader(fullPath);
                //byte[] readData = GeneralFunction.GetBytes(System.Text.Encoding.ASCII.GetString(reader.Read()));
                byte[] readData = reader.Read();
                Builder encBuilder = new Builder(Builder.ENCODING_TYPE.ISO8211, Builder.DATA_TYPE.S101);

                encBuilder.Build(readData);
                //encBuilder.Generator_Json();

                //FeatureCatalogue.FeatureCatalogue fc = new FeatureCatalogue.FeatureCatalogue(@"C:\Users\PGH\Desktop\Encs_Importer\Encs_Importer\bin\Debug\Schema\S-101_FC_0.8.8.xml");
                //fc.Load_XML_FC();

                //encBuilder.BindingFeatureCatalogue(fc);

                encBuilder.Generator_XML(Application.StartupPath + @"\test.xml");
              
            }
        } // XML 형식 샘플 데이터 생성 

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
