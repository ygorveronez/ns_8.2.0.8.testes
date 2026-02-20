using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace WSValidacaoCommerce
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Validacao" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Validacao.svc or Validacao.svc.cs at the Solution Explorer and start debugging.
    public class Validacao : IValidacao
    {
        #region Métodos Públicos

        public bool ExisteCliente(string cnpjCpf)
        {
            //GravaConteudoNoArquivo("ExisteCliente " + cnpjCpf);
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexao.StringConexao))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT EMP_CNPJ, EMP_NOME, EMP_DATA, EMP_DATAULTIMOACESSO, EMP_DATA_BLOQUEIO FROM T_EMPRESAS WHERE EMP_CNPJ = @cnpjCpf", connection))
                        {
                            //GravaConteudoNoArquivo("CNPJ " + cnpjCpf);
                            cmd.Parameters.AddWithValue("@cnpjCpf", cnpjCpf);

                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                sda.SelectCommand = cmd;
                                using (DataTable dt = new DataTable())
                                {
                                    dt.TableName = "Empresas";
                                    sda.Fill(dt);

                                    if (dt.Rows.Count > 0)
                                        return true;
                                    else
                                        return false;
                                }
                            }
                        }
                    }
                    catch (SqlException se)
                    {
                        GravaConteudoNoArquivo(se.Message);
                        return false;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return false;
            }
        }

        public bool InsereDadosCliente(string cnpjCpf, string fantasia, DateTime data)
        {
            //GravaConteudoNoArquivo("InsereDadosCliente " + cnpjCpf);
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexao.StringConexao))
                {
                    try
                    {
                        string sql = @"INSERT INTO T_EMPRESAS 
                                        (EMP_CNPJ, EMP_NOME, EMP_DATA, EMP_DATA_BLOQUEIO, EMP_DATAULTIMOACESSO)
                                        VALUES 
                                        (@cnpjCpf, @fantasia, @data, @dataBloqueio, @dataUltimoAcesso)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@cnpjCpf", cnpjCpf);
                            command.Parameters.AddWithValue("@fantasia", fantasia);
                            command.Parameters.AddWithValue("@data", data.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("@dataBloqueio", data.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("@dataUltimoAcesso", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            connection.Open();
                            command.ExecuteNonQuery();
                            return true;
                        }
                    }
                    catch (SqlException se)
                    {
                        GravaConteudoNoArquivo(se.Message);
                        return true;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return false;
            }
        }

        public bool AtualizarDataVencimentoBloqueio(string cnpjCpf, DateTime dataVencimento, DateTime dataBloqueio)
        {
            //GravaConteudoNoArquivo("AtualizarDataVencimentoBloqueio " + cnpjCpf);
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexao.StringConexao))
                {
                    try
                    {
                        string sql = @"UPDATE T_EMPRESAS SET EMP_DATA = @dataVencimento, EMP_DATA_BLOQUEIO = @dataBloqueio WHERE EMP_CNPJ = @cnpjCpf";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@dataVencimento", dataVencimento.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("@dataBloqueio", dataBloqueio.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("@cnpjCpf", cnpjCpf);

                            connection.Open();
                            command.ExecuteNonQuery();
                            return true;
                        }
                    }
                    catch (SqlException se)
                    {
                        GravaConteudoNoArquivo(se.Message);
                        return false;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return false;
            }
        }

        public DateTime? RetornaDataVencimento(string cnpjCpf)
        {
            //GravaConteudoNoArquivo("RetornaDataVencimento " + cnpjCpf);

            try
            {
                if (cnpjCpf.Length >= 15)
                {
                    cnpjCpf = Criptografar(cnpjCpf, cnpjCpf.Length);
                    //GravaConteudoNoArquivo("RetornaDataVencimento crip " + cnpjCpf);
                }
            }
            catch
            {
                GravaConteudoNoArquivo("Não foi possível descriptografar cnpj");
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexao.StringConexao))
                {
                    try
                    {
                        //GravaConteudoNoArquivo("RetornaDataVencimento CNPJ " + cnpjCpf);
                        using (SqlCommand cmd = new SqlCommand("SELECT EMP_CNPJ, EMP_NOME, EMP_DATA, EMP_DATAULTIMOACESSO, EMP_DATA_BLOQUEIO FROM T_EMPRESAS WHERE EMP_CNPJ = @cnpjCpf", connection))
                        {
                            cmd.Parameters.AddWithValue("@cnpjCpf", cnpjCpf);

                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                sda.SelectCommand = cmd;
                                using (DataTable dt = new DataTable())
                                {
                                    dt.TableName = "Empresas";
                                    sda.Fill(dt);

                                    if (dt.Rows.Count > 0)
                                    {
                                        string sql = "UPDATE T_EMPRESAS SET EMP_DATAULTIMOACESSO = @dataAtual WHERE EMP_CNPJ = @cnpjCpfUpdate";

                                        using (SqlCommand updateCmd = new SqlCommand(sql, connection))
                                        {
                                            updateCmd.Parameters.AddWithValue("@dataAtual", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                            updateCmd.Parameters.AddWithValue("@cnpjCpfUpdate", cnpjCpf);

                                            connection.Open();
                                            updateCmd.ExecuteNonQuery();
                                        }

                                        //GravaConteudoNoArquivo("EMP_DATA DO CNPJ " + cnpjCpf + " DATA " + dt.Rows[0].ItemArray[2]);
                                        return Convert.ToDateTime(dt.Rows[0].ItemArray[2]);//EMP_DATA
                                    }
                                    else
                                        return null;
                                }
                            }
                        }
                    }
                    catch (SqlException se)
                    {
                        GravaConteudoNoArquivo(se.Message);
                        return null;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return null;
            }
        }

        public DateTime? RetornaDataBloqueio(string cnpjCpf)
        {
            //GravaConteudoNoArquivo("RetornaDataBloqueio " + cnpjCpf);
            try
            {
                if (cnpjCpf.Length >= 15)
                {
                    cnpjCpf = Criptografar(cnpjCpf, cnpjCpf.Length);
                    //GravaConteudoNoArquivo("RetornaDataVencimento crip " + cnpjCpf);
                }
            }
            catch
            {
                GravaConteudoNoArquivo("Não foi possível descriptografar cnpj");
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexao.StringConexao))
                {
                    try
                    {
                        //GravaConteudoNoArquivo("RetornaDataBloqueio CNPJ " + cnpjCpf);
                        using (SqlCommand cmd = new SqlCommand("SELECT EMP_CNPJ, EMP_NOME, EMP_DATA, EMP_DATAULTIMOACESSO, EMP_DATA_BLOQUEIO FROM T_EMPRESAS WHERE EMP_CNPJ = @cnpjCpf", connection))
                        {
                            cmd.Parameters.AddWithValue("@cnpjCpf", cnpjCpf);

                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                sda.SelectCommand = cmd;
                                using (DataTable dt = new DataTable())
                                {
                                    dt.TableName = "Empresas";
                                    sda.Fill(dt);

                                    if (dt.Rows.Count > 0)
                                    {
                                        string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        string sql = "UPDATE T_EMPRESAS SET EMP_DATAULTIMOACESSO = @dataAtual WHERE EMP_CNPJ = @cnpjCpfUpdate";

                                        using (SqlCommand updateCmd = new SqlCommand(sql, connection))
                                        {
                                            updateCmd.Parameters.AddWithValue("@dataAtual", dataAtual);
                                            updateCmd.Parameters.AddWithValue("@cnpjCpfUpdate", cnpjCpf);

                                            connection.Open();
                                            updateCmd.ExecuteNonQuery();
                                        }

                                        //GravaConteudoNoArquivo("EMP_DATA_BLOQUEIO DO CNPJ " + cnpjCpf + " DATA " + dt.Rows[0].ItemArray[4]);
                                        return Convert.ToDateTime(dt.Rows[0].ItemArray[4]);//EMP_DATA_BLOQUEIO
                                    }
                                    else
                                        return null;
                                }
                            }
                        }
                    }
                    catch (SqlException se)
                    {
                        GravaConteudoNoArquivo(se.Message);
                        return null;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return null;
            }
        }

        public DateTime? RetornaDataAtual()
        {
            //GravaConteudoNoArquivo("RetornaDataAtual");
            try
            {
                try
                {
                    return DateTime.Now;
                }
                catch (SqlException se)
                {
                    GravaConteudoNoArquivo(se.Message);
                    return null;
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return null;
            }
        }

        public bool BloquearCliente(string cnpjCpf)
        {
            //GravaConteudoNoArquivo("BloquearCliente " + cnpjCpf);
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexao.StringConexao))
                {
                    try
                    {
                        string data = DateTime.Now.Date.ToString("yyyy-MM-dd");
                        string sql = "UPDATE T_EMPRESAS SET EMP_DATA = @data, EMP_DATA_BLOQUEIO = @data WHERE EMP_CNPJ = @cnpjCpf";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@data", data);
                            command.Parameters.AddWithValue("@cnpjCpf", cnpjCpf);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }

                        return true;
                    }
                    catch (SqlException se)
                    {
                        GravaConteudoNoArquivo(se.Message);
                        return false;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return false;
            }
        }

        public bool DesbloquearCliente(string cnpjCpf)
        {
            //GravaConteudoNoArquivo("DesbloquearCliente " + cnpjCpf);
            try
            {
                using (SqlConnection connection = new SqlConnection(Conexao.StringConexao))
                {
                    try
                    {
                        string data = DateTime.Now.Date.AddYears(1).ToString("yyyy-MM-dd");
                        string sql = "UPDATE T_EMPRESAS SET EMP_DATA = @data, EMP_DATA_BLOQUEIO = @data WHERE EMP_CNPJ = @cnpjCpf"; 

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@data", data);
                            command.Parameters.AddWithValue("@cnpjCpf", cnpjCpf);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }

                        return true;
                    }
                    catch (SqlException se)
                    {
                        GravaConteudoNoArquivo(se.Message);
                        return false;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return false;
            }
        }

        public async System.Threading.Tasks.Task<bool> CadastrarClienteWooCommerceAsync(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa clienteIntegracao, string url, string key, string secret)
        {
            try
            {
                try
                {
                    await CadastrarCliente(clienteIntegracao, url, key, secret);

                    return true;
                }
                catch (Exception se)
                {
                    GravaConteudoNoArquivo(se.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return false;
            }
        }

        public async System.Threading.Tasks.Task<bool> CadastrarProdutoWooCommerceAsync(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao, string url, string key, string secret)
        {
            try
            {
                try
                {
                    await CadastrarProduto(produtoIntegracao, url, key, secret);

                    return true;
                }
                catch (Exception se)
                {
                    GravaConteudoNoArquivo(se.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return false;
            }
        }

        public async System.Threading.Tasks.Task<bool> ConfirmarPedidoWooCommerceAsync(string url, string key, string secret, string codigoPedido)
        {
            try
            {
                try
                {
                    return await ConfirmarPedido(url, key, secret, codigoPedido);
                }
                catch (Exception se)
                {
                    GravaConteudoNoArquivo(se.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return false;
            }
        }


        public async System.Threading.Tasks.Task<List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido>> BuscarPedidosWooCommerceAsync(string url, string key, string secret, bool atualizarParaEmProcessamento)
        {
            try
            {
                try
                {
                    List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido> pedidos = await BuscarPedidos(url, key, secret, atualizarParaEmProcessamento);

                    return pedidos;
                }
                catch (Exception se)
                {
                    GravaConteudoNoArquivo(se.Message);
                    return null;
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return null;
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido> BuscarPedidosTrayAsync(string url, string consumer_key, string consumer_secret, string code, bool atualizarParaEmProcessamento, string status, string statusProcessamento)
        {
            try
            {
                try
                {
                    List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido> pedidos = BuscarPedidosTray(url, consumer_key, consumer_secret, code, atualizarParaEmProcessamento, status, statusProcessamento);

                    return pedidos;
                }
                catch (Exception se)
                {
                    GravaConteudoNoArquivo(se.Message);
                    return null;
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return null;
            }
        }

        public async System.Threading.Tasks.Task<string> CadastrarProdutoTrayAsync(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao, string url, string consumer_key, string consumer_secret, string code)
        {
            try
            {
                try
                {
                    return await CadastrarProdutoTray(produtoIntegracao, url, consumer_key, consumer_secret, code);
                }
                catch (Exception se)
                {
                    GravaConteudoNoArquivo(se.Message);
                    return se.Message;
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return e.Message;
            }
        }

        public async System.Threading.Tasks.Task<bool> ConfirmarPedidoTrayAsync(string url, string consumer_key, string consumer_secret, string code, string codigoPedido, string statusProcessamento)
        {
            try
            {
                try
                {
                    return await ConfirmarPedidoTray(url, consumer_key, consumer_secret, code, codigoPedido, statusProcessamento);
                }
                catch (Exception se)
                {
                    GravaConteudoNoArquivo(se.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                GravaConteudoNoArquivo(e.Message);
                return false;
            }
        }
        #endregion

        #region Métodos Privados

        private static string Criptografar(string wStri, int total)
        {
            string retorno = "";
            string[] Simbolos = new string[5];

            Simbolos[1] = "ABCDEFGHIJLMNOPQRSTUVXZYWK ~!@#$%^&*()";
            Simbolos[2] = "ÂÀ©Øû×ƒçêùÿ5Üø£úñÑªº¿®¬¼ëèïÙýÄÅÉæÆôöò»Á";
            Simbolos[3] = "abcdefghijlmnopqrstuvxzywk1234567890";
            Simbolos[4] = "áâäàåíóÇüé¾¶§÷ÎÏ-+ÌÓß¸°¨·¹³²Õµþîì¡«½";

            for (int x = 0; x < total; x++)
            {
                //if pos(copy(wStri,x,1),Simbolos[1])>0 then
                if (Simbolos[1].IndexOf(wStri.Substring(x, 1)) > 0)
                {
                    //Result := Result+copy(Simbolos[2], pos(copy(wStri,x,1),Simbolos[1]),1)
                    retorno = retorno + Simbolos[2].Substring(Simbolos[1].IndexOf(wStri.Substring(x, 1)), 1);
                }
                else if (Simbolos[2].IndexOf(wStri.Substring(x, 1)) > 0) //if (wStri.Substring(x, 1).IndexOf(Simbolos[2]) > 0)
                    retorno = retorno + Simbolos[1].Substring(Simbolos[2].IndexOf(wStri.Substring(x, 1)), 1);
                else if (Simbolos[3].IndexOf(wStri.Substring(x, 1)) > 0)                //if (wStri.Substring(x, 1).IndexOf(Simbolos[3]) > 0)
                    retorno = retorno + Simbolos[4].Substring(Simbolos[3].IndexOf(wStri.Substring(x, 1)), 1);
                else if (Simbolos[4].IndexOf(wStri.Substring(x, 1)) > 0) //if (wStri.Substring(x, 1).IndexOf(Simbolos[4]) > 0)
                    retorno = retorno + Simbolos[3].Substring(Simbolos[4].IndexOf(wStri.Substring(x, 1)), 1);

            }

            if (!string.IsNullOrWhiteSpace(retorno) && retorno.Length >= 14)
                retorno = retorno.Substring(0, 14);

            return retorno;
        }

        private static void GravaConteudoNoArquivo(string mensagem, string prefixo = "")
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                string arquivo = (string.IsNullOrWhiteSpace(prefixo) ? "" : prefixo + "-") + dateTime.Day + "-" + dateTime.Month + "-" + dateTime.Year + ".txt";
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                string file = System.IO.Path.Combine(path, arquivo);

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                System.IO.StreamWriter strw = new System.IO.StreamWriter(file, true);
                try
                {
                    strw.WriteLine(DateTime.Now.ToLongTimeString());
                    strw.WriteLine(mensagem);
                    strw.WriteLine();
                }
                catch
                {
                }
                finally
                {
                    strw.Close();
                }
            }
            catch
            {
            }
        }

        private static async System.Threading.Tasks.Task CadastrarProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao, string url, string key, string secret)
        {
            //RestAPI rest = new RestAPI("https://amigliss.com.br/wp-json/wc/v3/", "ck_c3dfe35d08586655ad110bd4be2bc90edb6d2e44", "cs_5364930e2d9a9440bbd448a20df6086bf1a10afc");
            RestAPI rest = new RestAPI(url, key, secret);
            WCObject wc = new WCObject(rest);

            var TOTODSprodutos = await wc.Product.GetAll();

            int.TryParse(produtoIntegracao.CodigoProduto, out int codigoIntegracao);
            var produtos = await wc.Product.Get(codigoIntegracao);
            bool inserir = produtos == null;

            //Add new product
            Product p = new Product()
            {
                //name = produtoIntegracao.DescricaoProduto,                
                //price = produtoIntegracao.ValorUnitario,
                date_modified = DateTime.Now,
                id = codigoIntegracao,
                stock_quantity = (int)produtoIntegracao.Quantidade
                //sale_price = produtoIntegracao.ValorUnitario,
                //regular_price = produtoIntegracao.ValorUnitario
            };
            if (inserir)
                await wc.Product.Add(p);
            else
                await wc.Product.Update(codigoIntegracao, p);
        }

        private static async System.Threading.Tasks.Task CadastrarCliente(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa clienteIntegracao, string url, string key, string secret)
        {
            RestAPI rest = new RestAPI(url, key, secret);
            WCObject wc = new WCObject(rest);

            var clientes = await wc.Customer.GetAll();
            bool inserir = true;
            int.TryParse(clienteIntegracao.CodigoIntegracao, out int codigoIntegracao);
            if (clientes != null && clientes.Count > 0)
            {
                if (clientes.Any(c => c.id == codigoIntegracao))
                    inserir = false;
            }

            Customer p = new Customer()
            {
                date_modified = DateTime.Now,
                email = clienteIntegracao.Email,
                first_name = clienteIntegracao.RazaoSocial.Split(' ').FirstOrDefault(),
                id = codigoIntegracao,
                last_name = clienteIntegracao.RazaoSocial.Replace(clienteIntegracao.RazaoSocial.Split(' ').FirstOrDefault(), "").Trim(),
                is_paying_customer = true,
                username = clienteIntegracao.RazaoSocial.Split(' ').FirstOrDefault(),
                password = clienteIntegracao.RazaoSocial.Split(' ').FirstOrDefault()
            };

            p.billing = new CustomerBilling();
            p.billing.address_1 = clienteIntegracao.Endereco?.Logradouro ?? "";
            p.billing.city = clienteIntegracao.Endereco?.Cidade?.Descricao ?? "";
            p.billing.email = clienteIntegracao.Email;
            p.billing.first_name = clienteIntegracao.RazaoSocial.Split(' ').FirstOrDefault();
            p.billing.last_name = clienteIntegracao.RazaoSocial.Replace(clienteIntegracao.RazaoSocial.Split(' ').FirstOrDefault(), "").Trim();
            p.billing.phone = clienteIntegracao.Endereco?.Telefone ?? "";
            p.billing.postcode = clienteIntegracao.Endereco?.CEP ?? "";
            p.billing.state = clienteIntegracao.Endereco?.Cidade?.SiglaUF ?? "";

            if (inserir)
                await wc.Customer.Add(p);
            else
                await wc.Customer.Update((int)codigoIntegracao, p);
        }

        private static async System.Threading.Tasks.Task<bool> ConfirmarPedido(string url, string key, string secret, string codigoPedido)
        {
            RestAPI rest = new RestAPI(url, key, secret);
            WCObject wc = new WCObject(rest);
            int codigoPedidoInt = 0;
            int.TryParse(codigoPedido, out codigoPedidoInt);
            if (codigoPedidoInt == 0)
                return false;

            var pedido = await wc.Order.Get(codigoPedidoInt);
            if (pedido.id == codigoPedidoInt)
            {
                pedido.status = "completed";
                await wc.Order.Update(pedido.id.Value, pedido);
            }
            return true;
        }


        private static async System.Threading.Tasks.Task<List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido>> BuscarPedidos(string url, string key, string secret, bool atualizarParaEmProcessamento)
        {
            List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido> pedidosIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido>();

            RestAPI rest = new RestAPI(url, key, secret);
            WCObject wc = new WCObject(rest);

            Dictionary<string, string> parms = new Dictionary<string, string>() { { "status", "processing" } };

            var pedidos = await wc.Order.GetAll(parms);
            foreach (var pedido in pedidos)
            {
                var clienteWS = await wc.Customer.Get(pedido.customer_id.Value);

                Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Cliente cliente = new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Cliente()
                {
                    CodigoIntegracao = clienteWS.id.Value.ToString("D"),
                    Email = clienteWS.email,
                    RazaoSocial = clienteWS.first_name + " " + clienteWS.last_name,
                    Logradouro = clienteWS.billing.address_1,
                    Cidade = clienteWS.billing.city,
                    SiglaUF = clienteWS.billing.state,
                    Telefone = clienteWS.billing.phone,
                    CEP = clienteWS.billing.postcode,
                    NumeroEndereco = clienteWS.meta_data.Where(c => c.key == "billing_number").Select(c => c.value)?.FirstOrDefault()?.ToString() ?? "",
                    Bairro = clienteWS.meta_data.Where(c => c.key == "billing_neighborhood").Select(c => c.value).FirstOrDefault().ToString(),
                    CPFCNPJ = clienteWS.meta_data.Where(c => c.key == "billing_cpf").Select(c => c.value).FirstOrDefault().ToString()
                };

                List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Produto> produtos = new List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Produto>();
                foreach (var prod in pedido.line_items)
                {
                    Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Produto produto = new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Produto()
                    {
                        CodigoProduto = prod.product_id.Value.ToString("D"),//variation_id//prod.id.Value.ToString("D"),
                        CodigoVariacao = prod.variation_id.HasValue && prod.variation_id.Value > 0 ? prod.variation_id.Value.ToString("D") : "",
                        DescricaoProduto = prod.name,
                        Quantidade = prod.quantity.Value,
                        ValorUnitario = prod.subtotal.Value
                    };
                    produtos.Add(produto);
                }

                Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido ped = new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido()
                {
                    Cliente = cliente,
                    CodigoIntregacao = pedido.id.Value,
                    DataPedido = pedido.date_created.Value,
                    FormaPagamento = pedido.payment_method,
                    Observacao = string.Join(", ", pedido.shipping_lines.Select(t => t.method_title)),
                    Valor = pedido.total.Value,
                    Produtos = produtos
                };
                pedidosIntegracao.Add(ped);

                if (atualizarParaEmProcessamento)
                {
                    pedido.status = "completed";
                    await wc.Order.Update(pedido.id.Value, pedido);
                }
            }
            return pedidosIntegracao;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido> BuscarPedidosTray(string url, string consumer_key, string consumer_secret, string code, bool atualizarParaEmProcessamento, string status, string statusProcessamento)
        {
            List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido> pedidosIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido>();

            string URLAuth = "https://" + url + "/auth/";

            //Criando a chave de acesso
            NameValueCollection queryParameters = new NameValueCollection();

            queryParameters.Add("consumer_key", consumer_key);
            queryParameters.Add("consumer_secret", consumer_secret);
            queryParameters.Add("code", code);

            List<string> items = new List<string>();

            foreach (String name in queryParameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(queryParameters[name])));

            string postString = String.Join("&", items.ToArray());

            HttpWebRequest webRequest = WebRequest.Create(URLAuth) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(postString);
            requestWriter.Close();

            StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string responseData = responseReader.ReadToEnd();

            responseReader.Close();
            webRequest.GetResponse().Close();

            dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(responseData);

            string refresh_token = (string)objRetorno.refresh_token;

            //Atualizando a Chave de Acesso
            queryParameters.Clear();
            queryParameters.Add("refresh_token", refresh_token);

            items.Clear();

            foreach (String name in queryParameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(queryParameters[name])));

            string argsString = String.Join("&", items.ToArray());

            WebRequest request = WebRequest.Create(URLAuth + "?" + argsString);

            request.Credentials = CredentialCache.DefaultCredentials;

            WebResponse response = request.GetResponse();

            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            responseData = reader.ReadToEnd();

            //Console.WriteLine(responseData);
            reader.Close();
            response.Close();

            objRetorno = JsonConvert.DeserializeObject<dynamic>(responseData);
            string access_token = (string)objRetorno.access_token;

            URLAuth = "https://" + url + "/orders/";

            queryParameters = new NameValueCollection();

            queryParameters.Add("access_token", access_token);
            queryParameters.Add("sort", "id_desc");
            if (!string.IsNullOrWhiteSpace(status))
                queryParameters.Add("status", status);

            items.Clear();

            foreach (String name in queryParameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(queryParameters[name])));

            argsString = String.Join("&", items.ToArray());

            request = WebRequest.Create(URLAuth + "?" + argsString);

            request.Credentials = CredentialCache.DefaultCredentials;

            response = request.GetResponse();

            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            dataStream = response.GetResponseStream();

            reader = new StreamReader(dataStream);

            responseData = reader.ReadToEnd();
            objRetorno = JsonConvert.DeserializeObject<dynamic>(responseData);

            reader.Close();
            response.Close();

            if (objRetorno.Orders != null && objRetorno.Orders.Count > 0)
            {
                foreach (var order in objRetorno.Orders)
                {
                    string idPedido = (string)order.Order.id;

                    Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido ped = BuscarDadosPedido(url, access_token, idPedido);

                    pedidosIntegracao.Add(ped);

                    if (atualizarParaEmProcessamento)
                        AtualizarParaEmProcessamento(url, access_token, idPedido, statusProcessamento);
                }
            }

            return pedidosIntegracao;
        }

        private static Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido BuscarDadosPedido(string url, string access_token, string idPedido)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string URLAuth = "https://" + url + "/orders/" + idPedido + "/complete";

            NameValueCollection queryParameters = new NameValueCollection();

            queryParameters.Add("access_token", access_token);

            List<string> items = new List<string>();

            foreach (String name in queryParameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(queryParameters[name])));

            string argsString = String.Join("&", items.ToArray());

            WebRequest request = WebRequest.Create(URLAuth + "?" + argsString);

            request.Credentials = CredentialCache.DefaultCredentials;

            WebResponse response = request.GetResponse();

            Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            string responseData = reader.ReadToEnd();

            dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(responseData);

            Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Cliente cliente = new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Cliente()
            {
                CodigoIntegracao = (string)objRetorno.Order.Customer.id,
                Email = (string)objRetorno.Order.Customer.email,
                RazaoSocial = (string)objRetorno.Order.Customer.name,
                Logradouro = (string)objRetorno.Order.Customer.address,
                Cidade = (string)objRetorno.Order.Customer.city,
                SiglaUF = (string)objRetorno.Order.Customer.state,
                Telefone = (string)objRetorno.Order.Customer.phone,
                CEP = (string)objRetorno.Order.Customer.zip_code,
                Bairro = (string)objRetorno.Order.Customer.neighborhood,
                CPFCNPJ = string.IsNullOrWhiteSpace((string)objRetorno.Order.Customer.cpf) ? (string)objRetorno.Order.Customer.cnpj : (string)objRetorno.Order.Customer.cpf
            };

            List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Produto> produtos = new List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Produto>();
            foreach (var prod in objRetorno.Order.ProductsSold)
            {
                Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Produto produto = new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Produto()
                {
                    CodigoProduto = (string)prod.ProductsSold.id,
                    DescricaoProduto = (string)prod.ProductsSold.name,
                    Quantidade = (decimal)prod.ProductsSold.quantity,
                    ValorUnitario = (decimal)prod.ProductsSold.price//ValorUnitario = (decimal)prod.ProductsSold.original_price
                };
                produtos.Add(produto);
            }

            Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido ped = new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido()
            {
                Cliente = cliente,
                CodigoIntregacao = (int)objRetorno.Order.id,
                DataPedido = (DateTime)objRetorno.Order.date,
                Observacao = (string)objRetorno.Order.store_note,
                Valor = (decimal)objRetorno.Order.partial_total,
                Produtos = produtos
            };

            reader.Close();
            response.Close();

            return ped;
        }

        private static bool AtualizarParaEmProcessamento(string url, string access_token, string idPedido, string statusProcessamento)
        {
            string URLAuth = "https://" + url + "/orders/" + idPedido;

            NameValueCollection queryParameters = new NameValueCollection();

            queryParameters.Add("access_token", access_token);

            List<string> items = new List<string>();

            foreach (String name in queryParameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(queryParameters[name])));

            string argsString = String.Join("&", items.ToArray());

            string putString = "{";
            putString += "  \"Order\": {";
            putString += "      \"status\": \"" + statusProcessamento + "\"";
            putString += "  }";
            putString += "}";

            HttpWebRequest webRequest = WebRequest.Create(URLAuth + "?" + argsString) as HttpWebRequest;
            webRequest.Method = "PUT";
            webRequest.ContentType = "application/json";

            StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(putString);
            requestWriter.Close();

            StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string responseData = responseReader.ReadToEnd();

            responseReader.Close();
            webRequest.GetResponse().Close();

            return true;
        }

        private static async System.Threading.Tasks.Task<string> CadastrarProdutoTray(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao, string url, string consumer_key, string consumer_secret, string code)
        {
            string URLAuth = "https://" + url + "/auth/";
            string msgRetorno = "Produto atualizado com sucesso";
            //Criando a chave de acesso
            NameValueCollection queryParameters = new NameValueCollection();

            queryParameters.Add("consumer_key", consumer_key);
            queryParameters.Add("consumer_secret", consumer_secret);
            queryParameters.Add("code", code);

            List<string> items = new List<string>();

            foreach (String name in queryParameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(queryParameters[name])));

            string postString = String.Join("&", items.ToArray());

            HttpWebRequest webRequest = WebRequest.Create(URLAuth) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(postString);
            requestWriter.Close();

            StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string responseData = responseReader.ReadToEnd();

            responseReader.Close();
            webRequest.GetResponse().Close();

            dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(responseData);

            string refresh_token = (string)objRetorno.refresh_token;

            queryParameters.Clear();
            queryParameters.Add("refresh_token", refresh_token);

            items.Clear();

            foreach (String name in queryParameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(queryParameters[name])));

            string argsString = String.Join("&", items.ToArray());

            WebRequest request = WebRequest.Create(URLAuth + "?" + argsString);

            request.Credentials = CredentialCache.DefaultCredentials;

            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            responseData = reader.ReadToEnd();

            reader.Close();
            response.Close();

            objRetorno = JsonConvert.DeserializeObject<dynamic>(responseData);
            string access_token = (string)objRetorno.access_token;

            URLAuth = "https://" + url + "/products/" + produtoIntegracao.CodigoProduto;

            queryParameters = new NameValueCollection();
            queryParameters.Add("access_token", access_token);

            items.Clear();

            foreach (String name in queryParameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(queryParameters[name])));

            argsString = String.Join("&", items.ToArray());

            request = WebRequest.Create(URLAuth + "?" + argsString);

            request.Credentials = CredentialCache.DefaultCredentials;

            try
            {
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);

                responseData = reader.ReadToEnd();
                objRetorno = JsonConvert.DeserializeObject<dynamic>(responseData);

                reader.Close();
                response.Close();
            }
            catch
            {
                objRetorno = null;
            }

            if (objRetorno != null)
            {
                string putString = "{";
                putString += "    \"Product\": {";
                putString += "        \"ean\": \"" + produtoIntegracao.CodigocEAN + "\",";
                putString += "        \"name\": \"" + produtoIntegracao.DescricaoProduto + "\",";
                putString += "        \"ncm\": \"" + produtoIntegracao.CodigoNCM + "\",";
                putString += "        \"description\": \"" + produtoIntegracao.DescricaoProduto + "\",";
                putString += "        \"description_small\": \"" + produtoIntegracao.DescricaoProduto + "\",";
                putString += "        \"price\": " + produtoIntegracao.ValorUnitario + ",";
                putString += "        \"cost_price\": " + produtoIntegracao.ValorUnitario + ",";
                putString += "        \"promotional_price\": " + produtoIntegracao.ValorUnitario + ",";
                //putString += "        \"start_promotion\": \"2019-03-01\",";
                //putString += "        \"end_promotion\": \"2019-09-01\",";
                //putString += "        \"brand\": \"marca\",";
                //putString += "        \"model\": \"Modelo\",";
                //putString += "        \"weight\": 1000,";
                //putString += "        \"length\": 10,";
                //putString += "        \"width\": 10,";
                //putString += "        \"availability\": \"\",";
                //putString += "        \"height\": 10,";
                putString += "        \"stock\": " + produtoIntegracao.Quantidade + "";
                //putString += "        \"category_id\": \"2\",";
                //putString += "        \"available\": 1,";
                //putString += "        \"warranty\":\"\",";
                //putString += "        \"reference\": \"111\",";
                //putString += "        \"picture_source_1\": \"http://bancodeimagens/imagem1.jpg\",";
                //putString += "        \"picture_source_2\": \"http://bancodeimagens/imagem2.jpg\",";
                //putString += "        \"picture_source_3\": \"http://bancodeimagens/imagem3.jpg\",";
                //putString += "        \"picture_source_4\": \"http://bancodeimagens/imagem4.jpg\",";
                //putString += "        \"picture_source_5\": \"http://bancodeimagens/imagem5.jpg\",";
                //putString += "        \"picture_source_6\": \"http://bancodeimagens/imagem6.jpg\",";
                //putString += "        \"metatag\": [";
                //putString += "            {";
                //putString += "                \"type\": \"keywords\",";
                //putString += "                \"content\": \"Key1, Key2, Key3\",";
                //putString += "                \"local\": 1";
                //putString += "            }";
                //putString += "        ],";
                //putString += "        \"related_categories\": [],";
                //putString += "        \"release_date\": \"\"";
                putString += "    }";
                putString += "}";

                webRequest = WebRequest.Create(URLAuth + "?" + argsString) as HttpWebRequest;
                webRequest.Method = "PUT";
                webRequest.ContentType = "application/json";

                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                requestWriter.Write(putString);
                requestWriter.Close();

                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();

                responseReader.Close();
                webRequest.GetResponse().Close();
            }
            else
            {
                string putString = "{";
                putString += "\"Product\":  {";
                putString += "\"ean\":\"" + produtoIntegracao.CodigocEAN + "\",";
                putString += "\"name\":\"" + produtoIntegracao.DescricaoProduto + "\",";
                putString += "\"ncm\":\"" + produtoIntegracao.CodigoNCM + "\",";
                putString += "\"description\":\"" + produtoIntegracao.DescricaoProduto + "\",";
                putString += "\"description_small\":\"" + produtoIntegracao.DescricaoProduto + "\",";
                putString += "\"price\":" + produtoIntegracao.ValorUnitario + ",";
                putString += "\"cost_price\":" + produtoIntegracao.ValorUnitario + ",";
                putString += "\"promotional_price\":" + produtoIntegracao.ValorUnitario + ",";
                putString += "\"start_promotion\":\"2019-03-01\",";
                putString += "\"end_promotion\":\"2019-09-01\",";
                putString += "\"ipi_value\": 10,";
                putString += "\"brand\":\"Marca\",";
                putString += "\"model\":\"Modelo\",";
                putString += "\"weight\":10,";
                putString += "\"length\":10,";
                putString += "\"width\":10,";
                putString += "\"height\":10,";
                putString += "\"stock\":" + produtoIntegracao.Quantidade + ",";
                putString += "\"category_id\":\"2\",";
                putString += "\"available\":1,";
                putString += "\"availability\":\"Disponível em 1 dias\",";
                putString += "\"availability_days\":1,";
                putString += "\"reference\":\"" + produtoIntegracao.CodigoProduto + "\",";
                putString += "\"hot\": \"1\",";
                putString += "\"release\": \"1\",";
                putString += "\"additional_button\": \"0\",";
                putString += "\"related_categories\":[],";
                putString += "\"release_date\":\"\",";
                //putString += "\"picture_source_1\":\"http://bancodeimagens/imagem1.jpg\",";
                //putString += "\"picture_source_2\":\"http://bancodeimagens/imagem2.jpg\",";
                //putString += "\"picture_source_3\":\"http://bancodeimagens/imagem3.jpg\",";
                //putString += "\"picture_source_4\":\"http://bancodeimagens/imagem4.jpg\",";
                //putString += "\"picture_source_5\":\"http://bancodeimagens/imagem5.jpg\",";
                //putString += "\"picture_source_6\":\"http://bancodeimagens/imagem6.jpg\",";
                putString += "\"metatag\":[{\"type\":\"keywords\",";
                putString += "\"content\":\"Key1, Key2, Key3\",";
                putString += "\"local\":1}],";
                putString += "\"virtual_product\":\"0\"";
                putString += "}";
                putString += "}";

                URLAuth = "https://" + url + "/products?access_token=" + access_token;
                //webRequest = WebRequest.Create(URLAuth + "?" + argsString) as HttpWebRequest;
                webRequest = WebRequest.Create(URLAuth) as HttpWebRequest;
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";

                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                requestWriter.Write(putString);
                requestWriter.Close();

                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();

                responseReader.Close();
                webRequest.GetResponse().Close();

                msgRetorno = "Produto inserido com sucesso.";
            }

            return msgRetorno;
        }

        private static async System.Threading.Tasks.Task<bool> ConfirmarPedidoTray(string url, string consumer_key, string consumer_secret, string code, string codigoPedido, string statusProcessamento)
        {
            List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido> pedidosIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.PedidosVendas.Pedido>();

            string URLAuth = "https://" + url + "/auth/";

            NameValueCollection queryParameters = new NameValueCollection();

            queryParameters.Add("consumer_key", consumer_key);
            queryParameters.Add("consumer_secret", consumer_secret);
            queryParameters.Add("code", code);

            List<string> items = new List<string>();

            foreach (String name in queryParameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(queryParameters[name])));

            string postString = String.Join("&", items.ToArray());

            HttpWebRequest webRequest = WebRequest.Create(URLAuth) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(postString);
            requestWriter.Close();

            StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string responseData = responseReader.ReadToEnd();

            responseReader.Close();
            webRequest.GetResponse().Close();

            dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(responseData);

            string refresh_token = (string)objRetorno.refresh_token;

            //Atualizando a Chave de Acesso
            queryParameters.Clear();
            queryParameters.Add("refresh_token", refresh_token);

            items.Clear();

            foreach (String name in queryParameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(queryParameters[name])));

            string argsString = String.Join("&", items.ToArray());

            WebRequest request = WebRequest.Create(URLAuth + "?" + argsString);

            request.Credentials = CredentialCache.DefaultCredentials;

            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            responseData = reader.ReadToEnd();

            reader.Close();
            response.Close();

            objRetorno = JsonConvert.DeserializeObject<dynamic>(responseData);
            string access_token = (string)objRetorno.access_token;

            return AtualizarParaEmProcessamento(url, access_token, codigoPedido, statusProcessamento); ;
        }


        #endregion
    }
}
