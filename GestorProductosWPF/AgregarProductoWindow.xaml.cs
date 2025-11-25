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

namespace GestorProductosWPF
{
    /// <summary>
    /// Lógica de interacción para AgregarProductoWindow.xaml
    /// </summary>
    public partial class AgregarProductoWindow : Window
    {
        public Producto Producto { get; set; }
        public AgregarProductoWindow()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            Producto = new Producto
            { 
                Id= int.Parse(txtID.Text),
                codigoBarras= txtCodigoBarras.Text,
                Nombre= txtNombre.Text,
                Categoria= txtCategoria.Text,
                Precio= decimal.Parse(txtPrecio.Text),
                Stock= int.Parse(txtStock.Text),

            };
            this.DialogResult = true;
            this.Close();
        }
    }
}
