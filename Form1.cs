using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.IO;
using System.Threading;

namespace ExemploBase
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_Conectar_Click(object sender, EventArgs e)
        {
            #region Criar base de dados
            // Esta é a string que define o caminho da base de dados;
            string bancoDados = Application.StartupPath + @"\db\banco.sdf";

            // Esta é a string que define o parametro de conexão com a base de dados;
            string strConection = @"DataSource = " + bancoDados + "; Password = '1234'";

            // Este é o objeto que recebe a string de conexão;
            SqlCeEngine db = new SqlCeEngine(strConection);

            // Caso não exista a base de dados, ira ser criada;
            if (!File.Exists(bancoDados))
            {
                // Aqui cria a base de dados caso não exista;
                db.CreateDatabase();
                lb_Status.ForeColor = Color.Green;
                lb_Status.Text = "Conectado.";
            }
            // Aqui liberamos os recursos e fechamos a conexão com db;
            db.Dispose();

            /* Este é o objeto de conexão com a base de dados. obs.: ele deve receber a string de conexão
             como parametros*/
            SqlCeConnection conexao = new SqlCeConnection(strConection);

            // Aqui abrimos um try/catch antes de abrir a conexão;
            try
            {
                // Aqui abrimos a Conexão;
                conexao.Open();

                // Exibimos uma MSG informando que a conexão foi criada;
                lb_Resultado.ForeColor = Color.Green;
                Thread.Sleep(500);
                lb_Resultado.Text = "Conexão Criada Com Sucesso - Base de dados Conectada";

            }
            catch (Exception ex)
            {
                // Aqui informamos um erro caso a conexão falhe;
                lb_Resultado.ForeColor = Color.Red;
                lb_Resultado.Text = "Erro de conexão " + ex.Message;
            }
            finally
            {
                // Aqui fechamos a conexão;
                conexao.Close();
            }
            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region strings de conexão
            string bancoDados = Application.StartupPath + @"\db\banco.sdf";
            string strConection = @"DataSource = " + bancoDados + "; Password = '1234'";
            SqlCeConnection conexao = new SqlCeConnection(strConection);
            #endregion

            #region Tabela
            try
            {
                conexao.Open();
                // Aqui Criamos um objeto para criar o comando SQL e criar uma tabela;
                SqlCeCommand comando = new SqlCeCommand();  // Exemplo 1;

                // Aqui ligamos os comandos exemplos a conexão com o banco;
                comando.Connection = conexao;

                // Aqui Criamos a string SQL para criar tabelas;
                comando.CommandText = "CREATE TABLE usuario ( id INT NOT NULL PRIMARY KEY, nome NVARCHAR(30), email NVARCHAR(50))";

                // Aqui Executamos o Comando;
                comando.ExecuteNonQuery(); // Exemplo 1;

                // Aqui Exibimos um MSG Box indicando que a tabela foi criada;
                lb_Resultado.ForeColor = Color.Green;
                lb_Resultado.Text = "Tabela Criada";
                comando.Dispose();
            }
            catch (Exception ex)
            {
                // Aqui exibimos uma MSG Box caso de algum erro;
                lb_Resultado.ForeColor = Color.Red;
                lb_Resultado.Text = "Erro ao Criar Tabela";
            }
            finally
            {
                // Aqui fechamos a conexão;
                conexao.Close();
            }
            #endregion
        }

        private void btn_Inserir_Click(object sender, EventArgs e)
        {
            #region strings de conexão
            string bancoDados = Application.StartupPath + @"\db\banco.sdf";
            string strConection = @"DataSource = " + bancoDados + "; Password = '1234'";
            SqlCeConnection conexao = new SqlCeConnection(strConection);
            #endregion

            #region Inserir
            try
            {
                conexao.Open();
                // Aqui Criamos um objeto para criar o comando SQL e criar uma tabela;
                SqlCeCommand comando = new SqlCeCommand();  // Exemplo 1;

                // Aqui ligamos os comandos exemplos a conexão com o banco;
                comando.Connection = conexao;

                #region strings
                // Aqui são os campos de onde virão os valores para inserir o registro;
                string nome = txt_Nome.Text;
                string email = txt_Email.Text;
                if (nome == "" && email == "")
                {
                    lb_Resultado.ForeColor = Color.Red;
                    lb_Resultado.Text = "Campo Nome e ou Email não podem ser Vazios";
                    lb_Error.ForeColor = Color.Red;
                    lb_Error.Text = "*";
                    txt_Nome.Focus();
                    return;
                }
                int id = new Random(DateTime.Now.Millisecond).Next(0, 1000);
                #endregion

                // Aqui Criamos a string SQL para criar tabelas;
                comando.CommandText = "INSERT INTO usuario VALUES(" + id + ",'" + nome + "','" + email + "')";

                // Aqui Executamos o Comando;
                comando.ExecuteNonQuery(); // Exemplo 1;

                txt_Nome.Clear();
                txt_Email.Clear();
                txt_id.Clear();
                lb_Error.Text = "";

                // Aqui Exibimos um MSG Box indicando que a tabela foi criada;
                lb_Resultado.ForeColor = Color.Green;
                lb_Resultado.Text = $"Registro Inserido com Sucesso!";
                comando.Dispose();
            }
            catch (Exception ex)
            {
                // Aqui exibimos uma MSG Box caso de algum erro;
                lb_Resultado.ForeColor = Color.Red;
                lb_Resultado.Text = "Erro ao tentar Inserir Registro na base de Dados" + ex.Message;
            }
            finally
            {
                // Aqui fechamos a conexão;
                conexao.Close();
            }
            #endregion
        }

        private void btn_Buscar_Click(object sender, EventArgs e)
        {
            // Aqui limpamos a lista antes de exibirmos as informaçoes;
            lista.Rows.Clear();

            #region strings de conexão
            string bancoDados = Application.StartupPath + @"\db\banco.sdf";
            string strConection = @"DataSource = " + bancoDados + "; Password = '1234'";
            SqlCeConnection conexao = new SqlCeConnection(strConection);
            #endregion

            #region Buscar
            try
            {
                string query = "SELECT * FROM usuario";
                string nome = txt_Nome.Text;
                if (nome != "")
                {
                    query = "SELECT * FROM usuario WHERE nome LIKE '%" + nome + "%'";
                }

                // Aqui criamos um objeto datatable que serve pra preencher os dados na tabela;
                DataTable dados = new DataTable();

                // Aqui Criamos o Adptador que trata os dados na tabela e preenche as informaçoes;
                SqlCeDataAdapter adaptador = new SqlCeDataAdapter(query, strConection);

                conexao.Open();

                // Aqui o Adaptador preenche a tabela no DataTable;
                adaptador.Fill(dados);

                foreach (DataRow linha in dados.Rows)
                {
                    lista.Rows.Add(linha.ItemArray);
                }

                int total = int.Parse(lista.Rows.Count.ToString());
                lb_Resultado.ForeColor = Color.Green;
                lb_Resultado.Text = $"Busca concluida. O Arquivo contem um total de {total} registros na base de dados";

            }
            catch (Exception ex)
            {
                lista.Rows.Clear();
                lb_Resultado.ForeColor = Color.Red;
                lb_Resultado.Text = "Erro ao buscar registros ou um registro expecifico" + ex.Message;
            }
            finally
            {
                conexao.Close();
            }
            #endregion
        }

        private void lista_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        private void btn_Editar_Click(object sender, EventArgs e)
        {
            #region strings de conexão
            string bancoDados = Application.StartupPath + @"\db\banco.sdf";
            string strConection = @"DataSource = " + bancoDados + "; Password = '1234'";
            SqlCeConnection conexao = new SqlCeConnection(strConection);
            #endregion

            #region Editar
            try
            {
                conexao.Open();
                SqlCeCommand comando = new SqlCeCommand();
                comando.Connection = conexao;
                int id = (int)lista.SelectedRows[0].Cells[0].Value;
                string nome = txt_Nome.Text;
                string email = txt_Email.Text;
                string query = "UPDATE usuario SET nome = '" + nome + "', email = '" + email + "' WHERE id LIKE '" + id + "'";
                comando.CommandText = query;
                comando.ExecuteNonQuery();
                txt_Nome.Clear();
                txt_Email.Clear();
                txt_id.Clear();
                int total = int.Parse(lista.SelectedRows.Count.ToString());
                lb_Resultado.ForeColor = Color.Green;
                lb_Resultado.Text = $"Concluido! Total de {total} registros modifocados na base de dados.";
            }
            catch (Exception ex)
            {
                lb_Resultado.ForeColor = Color.Red;
                lb_Resultado.Text = "Erro ao tentar editar Registro " + ex.Message;
            }
            finally
            {
                conexao.Close();
            }
            #endregion
        }

        private void lista_Click(object sender, EventArgs e)
        {
            txt_id.Text = lista.SelectedRows[0].Cells[0].Value.ToString();
            txt_Nome.Text = lista.SelectedRows[0].Cells[1].Value.ToString();
            txt_Email.Text = lista.SelectedRows[0].Cells[2].Value.ToString();
        }

        private void btn_Excluir_Click(object sender, EventArgs e)
        {
            #region strings de conexão
            string bancoDados = Application.StartupPath + @"\db\banco.sdf";
            string strConection = @"DataSource = " + bancoDados + "; Password = '1234'";
            SqlCeConnection conexao = new SqlCeConnection(strConection);
            #endregion

            #region Excluir
            try
            {
                conexao.Open();
                SqlCeCommand comando = new SqlCeCommand();
                comando.Connection = conexao;

                int id = (int)lista.SelectedRows[0].Cells[0].Value;

                comando.CommandText = "DELETE FROM usuario WHERE id = '" + id + "'";
                comando.ExecuteNonQuery();

                txt_Nome.Clear();
                txt_Email.Clear();
                txt_id.Clear();

                int total = int.Parse(lista.SelectedRows.Count.ToString());
                lb_Resultado.ForeColor = Color.Green;
                lb_Resultado.Text = $"Completo! Total de {total} Registro Excluido da base de dados.";
            }
            catch (Exception ex)
            {
                lb_Resultado.ForeColor = Color.Red;
                lb_Resultado.Text = "Erro ao tentar excluir Registro " + ex.Message;
            }
            finally
            {
                conexao.Close();
            }
            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region strings de conexão
            string bancoDados = Application.StartupPath + @"\db\banco.sdf";
            string strConection = @"DataSource = " + bancoDados + "; Password = '1234'";
            SqlCeConnection conexao = new SqlCeConnection(strConection);
            #endregion
            if (File.Exists(bancoDados))
            {
                lb_Status.ForeColor = Color.Green;
                lb_Status.Text = "Conectado.";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txt_Nome.Clear();
            txt_Email.Clear();
            txt_id.Clear();
        }
    }
}
