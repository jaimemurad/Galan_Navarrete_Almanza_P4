using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestorProductosWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GestorProductos gestor = new GestorProductos();
        public MainWindow()
        {
            InitializeComponent();
            CargarDatosIniciales();

            dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();

            comboTipoBusqueda.Items.Add("ID");
            comboTipoBusqueda.Items.Add("Nombre");
            comboTipoBusqueda.SelectedIndex = 0;

            comboCriterioOrden.Items.Add("ID");
            comboCriterioOrden.Items.Add("Precio");
            comboCriterioOrden.Items.Add("Nombre");
            comboCriterioOrden.SelectedIndex = 0;

        }

        private void CargarDatosIniciales()
        {
            gestor.AgregarProducto(new Producto
            {
                Id = 3,
                codigoBarras = "789456",
                Nombre = "Teclado",
                Categoria = "Electronica",
                Precio = 300,
                Stock = 200,
            }
            );
            gestor.AgregarProducto(new Producto
            {
                Id = 15,
                codigoBarras = "123456",
                Nombre = "USB",
                Categoria = "Electronica",
                Precio = 120,
                Stock = 10,
            }
            );
            
            gestor.AgregarProducto(new Producto
            {
                Id = 9,
                codigoBarras = "369258",
                Nombre = "Sudadera",
                Categoria = "Ropa",
                Precio = 600,
                Stock = 15,
            }
            );
        }

        private void MostrarResultado(Producto producto, int iteraciones)
        {
            txtResultadoBusqueda.Text = producto?.ToString() ?? "Producto no encontrado";
            progressIteraciones.Value = iteraciones * 10;
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var ventanaAgregar = new AgregarProductoWindow();
            if (ventanaAgregar.ShowDialog() == true)
            {
                Producto nuevoProducto = ventanaAgregar.Producto; //Asigna el producto creado en la ventana
                try
                {
                    gestor.AgregarProducto(nuevoProducto);
                    dataGridProductos.ItemsSource = null;
                    dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnOrdenar_Click(object sender, RoutedEventArgs e)
        {
            List<Producto> productos = new List<Producto>(gestor.ObtenerListaProductos());
            string criterio = comboCriterioOrden.SelectedItem.ToString();

            switch (criterio)
            {
                case "ID":
                    OrdenadorSimplificado.QuickSortPorId(productos);
                    break;
                case "Precio":
                    OrdenadorSimplificado.QuickSortPorPrecio(productos);
                    break;
                case "Nombre":
                    productos = OrdenadorSimplificado.MergeSortPorNombre(productos);
                    break;
            }
            listViewOrdenados.ItemsSource = productos;
            DibujarGraficoBarras(productos);
        }

        private void DibujarGraficoBarras(List<Producto> productos)
        {
            // 1. Limpia el Canvas antes de dibujar
            canvasGrafico.Children.Clear();

            // Si no hay productos, no dibujes nada
            if (productos == null || productos.Count == 0)
            {
                // Opcional: Agregar un mensaje de "Sin datos"
                return;
            }

            // --- CÁLCULO DE ESCALA ---

            // Encuentra el valor máximo y CONVIÉRTELO EXPLÍCITAMENTE A DOUBLE
            double maxValor = (double)productos.Max(p => p.Precio);

            // Si maxValor es cero, evita la división por cero
            double escala = (maxValor > 0) ? canvasGrafico.ActualHeight / maxValor : 0;

            // Definimos el ancho de cada barra y el espacio entre ellas
            double anchoBarra = 40;
            double espacioHorizontal = 25; // Espacio entre barras
            double anchoTotalElemento = anchoBarra + espacioHorizontal;

            // Posición Y de la base del gráfico (Borde inferior del Canvas)
            double baseY = canvasGrafico.ActualHeight;

            // --- DIBUJO DE BARRAS Y ETIQUETAS ---

            for (int i = 0; i < productos.Count; i++)
            {
                // CONVERTIMOS productos[i].Precio A double antes de multiplicar
                double altura = (double)productos[i].Precio * escala;

                // Calcular la posición X de inicio para este elemento (barra + etiqueta)
                double startX = i * anchoTotalElemento + espacioHorizontal;

                // 1. Dibuja la Barra (Rectangle)
                Rectangle barra = new Rectangle
                {
                    Width = anchoBarra,
                    Height = altura,
                    Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x7A, 0xCC)),
                    RadiusX = 3,
                    RadiusY = 3
                };

                // POSICIONAMIENTO CLAVE: Usamos Canvas.SetLeft/Bottom
                Canvas.SetLeft(barra, startX);
                Canvas.SetBottom(barra, 1);

                // 2. Dibuja la Etiqueta (TextBlock)
                TextBlock etiqueta = new TextBlock
                {
                    Text = (productos[i].Nombre.Length > 10) ? productos[i].Nombre.Substring(0, 10) + "..." : productos[i].Nombre,
                    Width = anchoTotalElemento,
                    TextAlignment = TextAlignment.Center,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x44))
                };

                // POSICIONAMIENTO CLAVE: Usamos Canvas.SetLeft/Bottom
                Canvas.SetLeft(etiqueta, startX - (espacioHorizontal / 2));
                Canvas.SetBottom(etiqueta, -20);

                // 3. Agrega los elementos al Canvas
                canvasGrafico.Children.Add(barra);
                canvasGrafico.Children.Add(etiqueta);
            }

            // 4. (Opcional) Dibuja la línea del Eje X para estética
            Line ejeX = new Line
            {
                X1 = 0,
                Y1 = baseY,
                X2 = canvasGrafico.ActualWidth,
                Y2 = baseY,
                Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0xCC, 0xCC)),
                StrokeThickness = 1
            };
            canvasGrafico.Children.Add(ejeX);
        }


        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridProductos.SelectedItem is Producto productoSeleccionado)
            {
                bool eliminado = gestor.EliminarProducto(productoSeleccionado.codigoBarras);
                if (eliminado)
                {
                    dataGridProductos.ItemsSource = null;
                    dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();

                    MessageBox.Show("Producto eliminado correctamente.", "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Selecciona un producto a eliminar", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnBuscar_Click_1(object sender, RoutedEventArgs e)
        {
            string criterio = comboTipoBusqueda.SelectedItem.ToString();
            string valor = txtBusqueda.Text;

            //La lista ordenada
            List<Producto> productosParaBusqueda = new List<Producto>(gestor.ObtenerListaProductos());
            OrdenadorSimplificado.QuickSortPorId(productosParaBusqueda); //Ordenar por Id para busqueda binaria

            switch (criterio)
            {
                case "ID":
                    if (int.TryParse(valor, out int id))
                    {
                        var (producto, iteraciones) = BuscadorSimplificado.BusquedaBinaria(productosParaBusqueda, id);
                        MostrarResultado(producto, iteraciones);
                    }
                    break;
                case "Nombre":
                    var (productoNombre, iteracionesNombre) = BuscadorSimplificado.BusquedaSecuencialNombre(productosParaBusqueda, valor);
                    MostrarResultado(productoNombre, iteracionesNombre);
                    break;

            }
        }
    }
}