using DSSGenogram.Models;
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
using System.Windows.Shapes;

namespace DSSGenogram.Views
{
    /// <summary>
    /// Interaction logic for GetVertexData.xaml
    /// </summary>
    public partial class GetVertexData : Window
    {
        public DataVertex DataVertex { get; set; }

        public GetVertexData()
        {
            InitializeComponent();

            btnInput.Click += BtnInput_Click;

            DataVertex = new DataVertex();
            SetDataVertex();
        }

        private void BtnInput_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SetDataVertex()
        {
            int gender;
            if (rdoMale.IsChecked == true)
                gender = 0;
            else
                gender = 1;

            DataVertex.Gender = gender.ToString();
            DataVertex.Name = txtNameInput.Text;
            DataVertex.Profession = txtProfInput.Text;
        }
    }
}
