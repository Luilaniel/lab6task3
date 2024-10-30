﻿using System.Text;
using Lab5;
using System.IO;
namespace Lab5
{
    public partial class fMain : Form
    {
        public fMain()
        {
            InitializeComponent();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            gvCameras.AutoGenerateColumns = false;

            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Brand";
            column.Name = "Brand";
            gvCameras.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Model";
            column.Name = "Model";
            gvCameras.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "CountryOfOrigin";
            column.Name = "Country";
            gvCameras.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "SensorType";
            column.Name = "Sensor type";
            gvCameras.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "SensorResolution";
            column.Name = "Sensor resolution";
            gvCameras.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "LensType";
            column.Name = "Lens type";
            column.Width = 180;
            gvCameras.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "VideoFormat";
            column.Name = "Video format";
            gvCameras.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "WeatherSealing";
            column.Name = "Weather sealing";
            gvCameras.Columns.Add(column);

            bindSrcCameras.Add(new Camera("Canon", "EOS R5", "Japan", "CMOS", 45, "Interchangeable lens", "8K DCI", true));
            EventArgs args = new EventArgs();
            OnResize(args);
        }

        private void fMain_Resize(object sender, EventArgs e)
        {
            int buttonSize = 9 * btnAdd.Width + 3 * tsSeparator1.Width + 30;
            btnExit.Margin = new System.Windows.Forms.Padding(Width - buttonSize, 0, 0, 0);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Camera camera = new Camera();

            fCamera ft = new fCamera(camera);
            if (ft.ShowDialog() == DialogResult.OK)
            {
                bindSrcCameras.Add(camera);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Camera camera = (Camera)bindSrcCameras.List[bindSrcCameras.Position];

            fCamera ft = new fCamera(camera);
            if (ft.ShowDialog() == DialogResult.OK)
            {
                bindSrcCameras.List[bindSrcCameras.Position] = camera;
            }
        }

        private void btnSaveAsText_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Save data in text format",
                InitialDirectory = Application.StartupPath
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                {
                    try
                    {
                        foreach (ICamera camera in bindSrcCameras.List)
                        {
                            sw.WriteLine($"{camera.GetFullName()}\t{((Camera)camera).CountryOfOrigin}\t{((Camera)camera).LensType}\t" +
                                         $"{((Camera)camera).SensorResolution}\t{((Camera)camera).SensorType}\t" +
                                         $"{((Camera)camera).VideoFormat}\t{((Camera)camera).WeatherSealing}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: \n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnOpenFromText_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Read data in text format",
                InitialDirectory = Application.StartupPath
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                bindSrcCameras.Clear();
                using (StreamReader sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8))
                {
                    string s;
                    try
                    {
                        while ((s = sr.ReadLine()) != null)
                        {
                            string[] split = s.Split('\t');
                            if (split.Length >= 8)
                            {
                                try
                                {
                                    Camera camera = new Camera(
                                        split[0],
                                        split[1],
                                        split[2],
                                        split[3],
                                        int.Parse(split[4]),
                                        split[5],
                                        split[6],
                                        bool.Parse(split[7])
                                    );
                                    bindSrcCameras.Add(camera);
                                }
                                catch (FormatException fe)
                                {
                                    MessageBox.Show($"Error in data format: {fe.Message}",
                                        "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Error: Insufficient data in line.",
                                    "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: \n{ex.Message}", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnOpenFromBinary_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Data files (*.cameras)|*.cameras|All files (*.*)|*.*",
                Title = "Read data in binary format",
                InitialDirectory = Application.StartupPath
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                bindSrcCameras.Clear();
                using (BinaryReader br = new BinaryReader(openFileDialog.OpenFile()))
                {
                    try
                    {
                        while (br.BaseStream.Position < br.BaseStream.Length)
                        {
                            Camera camera = new Camera();
                            for (int i = 1; i <= 8; i++)
                            {
                                switch (i)
                                {
                                    case 1:
                                        camera.Brand = br.ReadString();
                                        break;
                                    case 2:
                                        camera.Model = br.ReadString();
                                        break;
                                    case 3:
                                        camera.CountryOfOrigin = br.ReadString();
                                        break;
                                    case 4:
                                        camera.SensorType = br.ReadString();
                                        break;
                                    case 5:
                                        camera.SensorResolution = br.ReadInt32();
                                        break;
                                    case 6:
                                        camera.LensType = br.ReadString();
                                        break;
                                    case 7:
                                        camera.VideoFormat = br.ReadString();
                                        break;
                                    case 8:
                                        camera.WeatherSealing = br.ReadBoolean();
                                        break;
                                }
                            }
                            bindSrcCameras.Add(camera);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: \n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Закрити застосунок?", "Вихід з програми", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete the current record?", "Delete record",
               MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                bindSrcCameras.RemoveCurrent();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear the table?\n\nAll data will be lost", "Data cleaning",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                bindSrcCameras.Clear();
            }
        }
    }
}
