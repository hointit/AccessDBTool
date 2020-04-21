using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;
using System.Dynamic;
using System.Collections;
using Microsoft.Win32;

namespace AccessDBTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _path;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RunAll_Click(object sender, RoutedEventArgs e)
        {
            RunQuery(txtQuery.Text);
        }
        private void RunSelection_Click(object sender, RoutedEventArgs e)
        {
            RunQuery(txtQuery.SelectedText);
        }


        private void RunQuery(string query)
        {
            try
            {
                var watch = new System.Diagnostics.Stopwatch();

                watch.Start();


                
                OleDbConnection connection = new OleDbConnection();
                connection.ConnectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={_path};Persist Security Info=False;";

                connection.Open();
                OleDbCommand command = new OleDbCommand();

                command.Connection = connection;
                command.CommandText = query;

                OleDbDataReader reader = command.ExecuteReader();
                var data = new List<ExpandoObject>();
                HashSet<string> Columns = new HashSet<string>();

                while (reader.Read())
                {

                    data.Add(ReaderToExpandoObject(reader, Columns));
                };

                dg1.Columns.Clear();

                foreach (string colName in Columns)
                {
                    dg1.Columns.Add(new DataGridTextColumn
                    {
                        Header = colName,
                        Binding = new Binding(colName),
                    });
                }

                connection.Close();

                dg1.ItemsSource = data;

                watch.Stop();
                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

                string status = $"Get {data.Count.ToString()} items in {watch.ElapsedMilliseconds} ms";
                this.tblCounter.Text = status;
            }
            catch (Exception ex)
            {
                this.tblCounter.Text = ex.Message;
                MessageBox.Show(ex.Message);
            }
        }

        public ExpandoObject ReaderToExpandoObject(OleDbDataReader reader, HashSet<string> columns)
        {
            var row = new ExpandoObject();
            var dicRow = row as IDictionary<string, object>;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var iValue = reader.GetValue(i);
                var iName = reader.GetName(i);
                dicRow[iName] = iValue.ToString();
                if (columns.Add(reader.GetName(i)))
                {
                    Console.WriteLine(iName + "\t" + iValue.GetType());
                }
            }
            return row;
        }


        private void ChosseDatabase_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _path = txtPath.Text = openFileDialog.FileName;
            }
        }
    }
}
