using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace FavoritaClickShop.Mysql
{
    internal class Cbodega
    {
        public void mostrarBodegas(DataGridView tablaBodega)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();
                string query = @"SELECT 
                            bodega.bo_id AS 'Bodega Id', 
                            producto.pro_nom AS 'Producto', 
                            bodega.bo_fecing AS 'Fecha de Ingreso', 
                            producto.pro_cad AS 'Caducidad', 
                            producto.pro_can AS 'Cantidad', 
                            producto.pro_cos AS 'Costo', 
                            producto.pro_pre AS 'Precio' 
                         FROM bodega 
                         INNER JOIN producto ON bodega.bo_id = producto.bo_id";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conexion);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                tablaBodega.DataSource = dt;
            }

            catch (Exception ex)
            {
                MessageBox.Show("No se encontraron los datos de la base de datos, error: " + ex.ToString());
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
            tablaBodega.CellFormatting += new DataGridViewCellFormattingEventHandler(tablaBodega_CellFormatting);

        }
        public void cargarNombresProveedores(ComboBox comboBoxProveedores)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = "SELECT prove_nom FROM proveedor";
                MySqlCommand command = new MySqlCommand(query, conexion);

                // Limpiar el ComboBox antes de cargar datos nuevos
                comboBoxProveedores.Items.Clear();

                // Abrir la conexión y ejecutar el comando
                // conexion.Open();
                MySqlDataReader reader = command.ExecuteReader();
                // comboBoxProveedores.Items.Add("No requiere");
                // Iterar a través de los resultados y agregar nombres al ComboBox
                while (reader.Read())
                {
                    string nombreProveedor = reader.GetString("prove_nom");
                    comboBoxProveedores.Items.Add(nombreProveedor);
                }

                // Seleccionar el primer elemento por defecto si hay elementos cargados
                if (comboBoxProveedores.Items.Count > 0)
                {
                    comboBoxProveedores.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar nombres de proveedores: " + ex.Message);
            }
            finally
            {
                if (conexion != null && conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }
            }
        }
        public void guardarBodega(TextBox boid, TextBox nom, DateTimePicker fechaIngreso, DateTimePicker fechaCaducidad, NumericUpDown can, NumericUpDown costo, NumericUpDown precio, ComboBox comboBoxProveedores)
        {
            MySqlConnection conexion = null;
            MySqlTransaction transaction = null;

            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();
                transaction = conexion.BeginTransaction();

                // Verificar si ya existe el bo_id en la tabla bodega
                string queryVerificarBodega = "SELECT COUNT(*) FROM bodega WHERE bo_id = @boId";
                MySqlCommand commandVerificarBodega = new MySqlCommand(queryVerificarBodega, conexion, transaction);
                commandVerificarBodega.Parameters.AddWithValue("@boId", boid.Text);
                int countBodega = Convert.ToInt32(commandVerificarBodega.ExecuteScalar());

                if (countBodega > 0)
                {
                    MessageBox.Show("Ya existe un registro con el número de bodega.");
                    return; // Salir del método sin guardar
                }

                // Obtener el prove_id del proveedor seleccionado
                string queryProveedor = "SELECT prove_id FROM proveedor WHERE prove_nom = @proveNom";
                MySqlCommand commandProveedor = new MySqlCommand(queryProveedor, conexion, transaction);
                commandProveedor.Parameters.AddWithValue("@proveNom", comboBoxProveedores.SelectedItem.ToString());
                object proveIdResult = commandProveedor.ExecuteScalar();
                if (proveIdResult == DBNull.Value)
                {
                    MessageBox.Show("Proveedor no encontrado.");
                    return;
                }
                int proveId = Convert.ToInt32(proveIdResult);

                // Insertar en tabla bodega
                string queryBodega = "INSERT INTO bodega(bo_id, bo_fecing, prove_id) VALUES (@boId, @boFecing, @proveId)";
                MySqlCommand myCommandBodega = new MySqlCommand(queryBodega, conexion, transaction);
                myCommandBodega.Parameters.AddWithValue("@boId", boid.Text);
                myCommandBodega.Parameters.AddWithValue("@boFecing", fechaIngreso.Value);
                myCommandBodega.Parameters.AddWithValue("@proveId", proveId);
                myCommandBodega.ExecuteNonQuery(); // Ejecutar la consulta de inserción en la tabla bodega

                // Obtener el último pro_cod insertado en la tabla producto
                string queryUltimoProCod = "SELECT MAX(pro_cod) FROM producto";
                MySqlCommand commandUltimoProCod = new MySqlCommand(queryUltimoProCod, conexion, transaction);
                object result = commandUltimoProCod.ExecuteScalar();
                int ultimoProCod = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                // Calcular el precio antes de la inserción
                CalcularPrecio(costo, precio);

                // Insertar en tabla producto con el siguiente pro_cod autoincrementado
                string queryProducto = "INSERT INTO producto(pro_cod, pro_nom, pro_cad, pro_can, pro_cos, pro_pre, bo_id, prove_id) VALUES (@proCod, @proNom, @proCad, @proCan, @proCos, @proPre, @boId, @proveId)";
                MySqlCommand myCommandProducto = new MySqlCommand(queryProducto, conexion, transaction);
                myCommandProducto.Parameters.AddWithValue("@proCod", ultimoProCod + 1); // Incrementar el último pro_cod
                myCommandProducto.Parameters.AddWithValue("@proNom", nom.Text);
                myCommandProducto.Parameters.AddWithValue("@proCad", fechaCaducidad.Value);
                myCommandProducto.Parameters.AddWithValue("@proCan", can.Value);
                myCommandProducto.Parameters.AddWithValue("@proCos", costo.Value);
                myCommandProducto.Parameters.AddWithValue("@proPre", precio.Value);
                myCommandProducto.Parameters.AddWithValue("@boId", boid.Text);
                myCommandProducto.Parameters.AddWithValue("@proveId", proveId);
                myCommandProducto.ExecuteNonQuery(); // Ejecutar la consulta de inserción en la tabla producto

                // Confirmar la transacción
                transaction.Commit();

                // Mostrar mensaje de éxito
                MessageBox.Show("Se guardó el registro correctamente");

                // Actualizar el DataGridView después de insertar el nuevo registro
                // CargarDatosBodega(); // Método para cargar nuevamente los datos en el DataGridView
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se revierte la transacción
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                MessageBox.Show("No se pudo guardar el registro: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        private void CalcularPrecio(NumericUpDown costo, NumericUpDown precio)
        {
            decimal costoProducto = costo.Value;
            decimal precioProducto = costoProducto * 1.1m; // Calcular el precio con el 10% de incremento
            precio.Value = precioProducto; // Actualizar el valor del precio
        }
        public void modificarBodega(TextBox boid, TextBox nom, DateTimePicker fechaIngreso, DateTimePicker fechaCaducidad, NumericUpDown can, NumericUpDown costo, NumericUpDown precio, ComboBox comboBoxProveedores)
        {
            MySqlConnection conexion = null;
            MySqlTransaction transaction = null;

            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();
                transaction = conexion.BeginTransaction();

                // Verificar si ya existe el bo_id en la tabla bodega
                string queryVerificarBodega = "SELECT COUNT(*) FROM bodega WHERE bo_id = @boId";
                MySqlCommand commandVerificarBodega = new MySqlCommand(queryVerificarBodega, conexion, transaction);
                commandVerificarBodega.Parameters.AddWithValue("@boId", boid.Text);
                int countBodega = Convert.ToInt32(commandVerificarBodega.ExecuteScalar());

                if (countBodega == 0)
                {
                    MessageBox.Show("No existe un registro con el número de bodega.");
                    return; // Salir del método sin modificar
                }

                // Obtener el prove_id del proveedor seleccionado
                string queryProveedor = "SELECT prove_id FROM proveedor WHERE prove_nom = @proveNom";
                MySqlCommand commandProveedor = new MySqlCommand(queryProveedor, conexion, transaction);
                commandProveedor.Parameters.AddWithValue("@proveNom", comboBoxProveedores.SelectedItem.ToString());
                object proveIdResult = commandProveedor.ExecuteScalar();
                if (proveIdResult == DBNull.Value)
                {
                    MessageBox.Show("Proveedor no encontrado.");
                    return;
                }
                int proveId = Convert.ToInt32(proveIdResult);

                // Obtener pro_cod del producto asociado a la bodega
                string queryProCod = "SELECT pro_cod FROM producto WHERE bo_id = @boId";
                MySqlCommand commandProCod = new MySqlCommand(queryProCod, conexion, transaction);
                commandProCod.Parameters.AddWithValue("@boId", boid.Text);
                object proCodResult = commandProCod.ExecuteScalar();
                if (proCodResult == DBNull.Value)
                {
                    MessageBox.Show("No se encontró un producto asociado a esta bodega.");
                    return;
                }
                int proCod = Convert.ToInt32(proCodResult);

                // Actualizar tabla bodega
                string queryActualizarBodega = "UPDATE bodega SET bo_fecing = @boFecing, prove_id = @proveId WHERE bo_id = @boId";
                MySqlCommand myCommandActualizarBodega = new MySqlCommand(queryActualizarBodega, conexion, transaction);
                myCommandActualizarBodega.Parameters.AddWithValue("@boId", boid.Text);
                myCommandActualizarBodega.Parameters.AddWithValue("@boFecing", fechaIngreso.Value);
                myCommandActualizarBodega.Parameters.AddWithValue("@proveId", proveId);
                myCommandActualizarBodega.ExecuteNonQuery(); // Ejecutar la consulta de actualización en la tabla bodega

                // Actualizar tabla producto
                string queryActualizarProducto = "UPDATE producto SET pro_nom = @proNom, pro_cad = @proCad, pro_can = @proCan, pro_cos = @proCos, pro_pre = @proPre WHERE pro_cod = @proCod";
                MySqlCommand myCommandActualizarProducto = new MySqlCommand(queryActualizarProducto, conexion, transaction);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCod", proCod);
                myCommandActualizarProducto.Parameters.AddWithValue("@proNom", nom.Text);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCad", fechaCaducidad.Value);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCan", can.Value);
                myCommandActualizarProducto.Parameters.AddWithValue("@proCos", costo.Value);
                myCommandActualizarProducto.Parameters.AddWithValue("@proPre", precio.Value); // Actualizar el precio del producto
                myCommandActualizarProducto.ExecuteNonQuery(); // Ejecutar la consulta de actualización en la tabla producto

                // Confirmar la transacción
                transaction.Commit();

                // Mostrar mensaje de éxito
                MessageBox.Show("Se modificó el registro correctamente");

                // Actualizar el DataGridView después de modificar el registro
                // CargarDatosBodega(); // Método para cargar nuevamente los datos en el DataGridView
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se revierte la transacción
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                MessageBox.Show("No se pudo modificar el registro: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public DataRow buscarBodega(int id)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                string query = @"SELECT bodega.bo_id, producto.pro_nom ,bodega.bo_fecing , producto.pro_cad, producto.pro_can, producto.pro_cos, producto.pro_pre 
                         FROM bodega 
                         INNER JOIN producto ON bodega.bo_id = producto.bo_id 
                         WHERE bodega.bo_id = @id";
                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@id", id);

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]; // Devuelve la primera fila encontrada
                }
                else
                {
                    return null; // No se encontraron resultados
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar bodega: " + ex.Message);
                return null;
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public void buscarBodegaPorId(DataGridView tablaBodega, TextBox id)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Query para buscar bodegas por ID
                string query = @"SELECT bodega.bo_id as 'Bodega Id', producto.pro_nom as 'Producto', bodega.bo_fecing as 'Fecha de Ingreso', 
                                producto.pro_cad as 'Fecha de Caducidad', producto.pro_can as 'Cantidad', 
                                producto.pro_cos as 'Costo', producto.pro_pre as 'Precio' 
                         FROM bodega 
                         INNER JOIN producto ON bodega.bo_id = producto.bo_id 
                         WHERE bodega.bo_id = @id";

                MySqlCommand command = new MySqlCommand(query, conexion);
                command.Parameters.AddWithValue("@id", id.Text.Trim());

                // Adaptador para llenar los datos en un DataTable
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Limpiar el DataGridView antes de mostrar los resultados
                tablaBodega.DataSource = null;
                tablaBodega.Rows.Clear();

                // Asignar el DataTable con los resultados al DataGridView
                tablaBodega.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar bodega: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }

        public void eliminarBodega(TextBox cod, DataGridView tablaBodega)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Eliminar productos relacionados con la bodega
                string queryProductos = "DELETE FROM producto WHERE bo_id = @boId";
                MySqlCommand commandProductos = new MySqlCommand(queryProductos, conexion);
                commandProductos.Parameters.AddWithValue("@boId", cod.Text);
                commandProductos.ExecuteNonQuery();

                // Eliminar la bodega de la tabla bodega
                string queryBodega = "DELETE FROM bodega WHERE bo_id = @boId";
                MySqlCommand commandBodega = new MySqlCommand(queryBodega, conexion);
                commandBodega.Parameters.AddWithValue("@boId", cod.Text);

                int rowsAffected = commandBodega.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Se eliminó el registro de la bodega.");
                }
                else
                {
                    MessageBox.Show("No se encontró ninguna bodega con ese ID.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el registro de la bodega y/o los productos relacionados. Error: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
        public void exportarExcel(DataGridView tabla)
        {
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                // Crear una instancia de Excel y abrir una nueva aplicación
                excelApp = new Excel.Application();
                excelApp.Visible = true; // Mostrar Excel

                // Crear un nuevo libro de Excel
                workbook = excelApp.Workbooks.Add();
                worksheet = (Excel.Worksheet)workbook.Sheets[1]; // Obtener la primera hoja de trabajo
                worksheet.Name = "Datos de Bodegas"; // Nombre de la hoja de trabajo

                // Encabezados de las columnas
                for (int i = 0; i < tabla.ColumnCount; i++)
                {
                    worksheet.Cells[1, i + 1] = tabla.Columns[i].HeaderText;
                }

                // Datos de las filas
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    for (int j = 0; j < tabla.Columns.Count; j++)
                    {
                        // Manejo seguro de valores nulos
                        if (tabla.Rows[i].Cells[j].Value != null)
                        {
                            worksheet.Cells[i + 2, j + 1] = tabla.Rows[i].Cells[j].Value.ToString();
                        }
                        else
                        {
                            worksheet.Cells[i + 2, j + 1] = string.Empty; // Opcional: Puedes usar otro valor por defecto si es null
                        }
                    }
                }

                // Diseño de tabla sencillo
                Excel.Range headerRange = worksheet.Range["A1", worksheet.Cells[1, tabla.ColumnCount]];
                headerRange.Font.Bold = true;
                headerRange.Interior.Color = Excel.XlRgbColor.rgbLightBlue;

                Excel.Range tableRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[tabla.Rows.Count + 1, tabla.Columns.Count]];
                tableRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                tableRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                // Ajustar el ancho de las columnas automáticamente
                worksheet.Columns.AutoFit();

                // Guardar el archivo Excel
                workbook.SaveAs("DatosBodegas.xlsx");

                // Mostrar un mensaje opcional
                // MessageBox.Show("Se exportaron los datos a Excel correctamente. Puedes encontrar el archivo en la carpeta del proyecto.");
            }
            catch (Exception ex)
            {
                // MessageBox.Show("Error al exportar a Excel: " + ex.Message);
            }
            finally
            {
                // Liberar recursos de Excel
                if (worksheet != null)
                {
                    Marshal.ReleaseComObject(worksheet);
                }
                if (workbook != null)
                {
                    // No cerrar el libro, solo liberar recursos
                    Marshal.ReleaseComObject(workbook);
                }

                // No cerrar la aplicación Excel aquí
                // No se invoca excelApp.Quit() ni se libera el objeto excelApp
            }
        }
        public void seleccionarBodega(DataGridView tablaBodega, TextBox textBoxBodegaId)
        {
            int columnIndex = tablaBodega.CurrentCell.ColumnIndex;
            try
            {
                if (columnIndex == 0)
                {
                    textBoxBodegaId.Text = tablaBodega.CurrentRow.Cells[0].Value.ToString();
                }
                // Asume que la columna del ID de bodega es la primera columna (índice 0)

                else if (columnIndex == 1)
                {
                    textBoxBodegaId.Text = tablaBodega.CurrentRow.Cells[1].Value.ToString();
                }

                else
                {
                    // Puedes manejar el caso cuando no es ninguna de las columnas deseadas
                    MessageBox.Show("No se seleccionó una columna válida.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se logró seleccionar, error: " + ex.ToString());
            }
        }
        private void tablaBodega_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv.Columns[e.ColumnIndex].Name == "Cantidad")
            {
                if (e.Value != null && e.Value.ToString() == "0")
                {
                    e.CellStyle.BackColor = Color.Red;
                }
            }
        }
        public void buscarBodegaPorFiltros(DataGridView tablaBodega, TextBox textBoxFiltro, ComboBox comboBoxFiltro)
        {
            MySqlConnection conexion = null;
            try
            {
                Conexion objetoConexion = new Conexion();
                conexion = objetoConexion.establecerConexion();

                // Construir la consulta base
                string query = @"SELECT b.bo_id as 'Bodega Id', p.pro_nom as 'Producto', b.bo_fecing as 'Fecha de Ingreso', 
                         p.pro_cad as 'Fecha de Caducidad', p.pro_can as 'Cantidad', 
                         p.pro_cos as 'Costo', p.pro_pre as 'Precio' 
                         FROM bodega b 
                         INNER JOIN producto p ON b.bo_id = p.bo_id 
                         WHERE 1=1";

                // Determinar el filtro basado en la selección del ComboBox
                if (comboBoxFiltro.SelectedItem != null)
                {
                    string filtro = textBoxFiltro.Text.Trim();
                    if (comboBoxFiltro.SelectedItem.ToString() == "Nombre del producto" && !string.IsNullOrWhiteSpace(filtro))
                    {
                        query += " AND p.pro_nom LIKE @filtro";
                    }
                    else if (comboBoxFiltro.SelectedItem.ToString() == "Bodega ID" && !string.IsNullOrWhiteSpace(filtro))
                    {
                        query += " AND b.bo_id = @filtro";
                    }
                }

                MySqlCommand command = new MySqlCommand(query, conexion);

                // Asignar valores a los parámetros
                if (!string.IsNullOrWhiteSpace(textBoxFiltro.Text))
                {
                    string filtro = textBoxFiltro.Text.Trim();
                    if (comboBoxFiltro.SelectedItem.ToString() == "Nombre del producto")
                    {
                        command.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                    }
                    else if (comboBoxFiltro.SelectedItem.ToString() == "Bodega ID")
                    {
                        command.Parameters.AddWithValue("@filtro", filtro);
                    }
                }

                // Adaptador para llenar los datos en un DataTable
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Limpiar el DataGridView antes de mostrar los resultados
                tablaBodega.DataSource = null;
                tablaBodega.Rows.Clear();

                // Asignar el DataTable con los resultados al DataGridView
                tablaBodega.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar bodega: " + ex.Message);
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Close();
                }
            }
        }
    }
}
