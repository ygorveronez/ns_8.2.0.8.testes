using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.OleDb;
using SGTAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Importacao;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize("Frotas/ImportacaoDePedagio", "Frotas/Pedagio")]
    public class ImportacaoDePedagioController : BaseController
    {
		#region Construtores

		public ImportacaoDePedagioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarPedagios()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                //AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(adminUnitOfWork);
                //AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(this);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);

                bool possuiPedagioInconsistente = false;

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                DataSet ds = new DataSet();
                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }

                        file.SaveAs(fileLocation);

                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                        try
                        {
                            excelConnection.Open();
                            DataTable dt = new DataTable();

                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            if (dt == null)
                            {
                                return null;
                            }

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                            string query = "";
                            if (excelSheets[0] == "'PASSAGENS ESTACIONAMENTO 1$'")
                                query = string.Format("Select * from [{0}]", excelSheets[1]);
                            else
                                query = string.Format("Select * from [{0}]", excelSheets[0]);

                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            bool processarPegadiosAFaturar = false;
                            if ((ds.Tables[0]).Columns[0].ToString() == "Relatório de Itens a Faturar")
                                processarPegadiosAFaturar = true;

                            if (!processarPegadiosAFaturar)
                            {
                                if ((ds.Tables[0]).Columns[0].ToString() != "PLACA" || (ds.Tables[0]).Columns[1].ToString() != "TAG" || (ds.Tables[0]).Columns[2].ToString() != "PREFIXO")
                                    return new JsonpResult(false, "O arquivo selecionado não está de acordo com o layout do Sem Parar");

                                serNotificacao.InfomarPercentualProcessamento(null, 0, "Frotas/Pedagio", 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pedagio, TipoServicoMultisoftware, unitOfWork);

                                int indiceLinha = 1;
                                int totalLinhas = ds.Tables[0].Rows.Count;
                                foreach (DataRow linha in ds.Tables[0].Rows)
                                {
                                    indiceLinha++;
                                    try
                                    {
                                        unitOfWork.Start();
                                        string placa = linha["PLACA"].ToString();
                                        string tag = linha["TAG"].ToString();
                                        string prefixo = linha["PREFIXO"].ToString();
                                        string marca = linha["MARCA"].ToString();
                                        string rodovia = linha["RODOVIA"].ToString();
                                        string praca = linha["PRAÇA"].ToString();

                                        int tipoMovimentoParam = 0;
                                        int categoria = 0;
                                        decimal valor = 0;
                                        DateTime data;

                                        DateTime.TryParseExact(linha["DATA"].ToString() + " " + linha["HORA"].ToString(), "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data);
                                        int.TryParse(linha["CATEG"].ToString(), out categoria);
                                        decimal.TryParse(linha["VALOR"].ToString(), out valor);
                                        int.TryParse(linha["MOV FINANCEIRO"].ToString(), out tipoMovimentoParam);

                                        Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorDados(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito, placa, data, rodovia, praca);
                                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(placa);
                                        Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = repTipoMovimento.BuscarPorCodigo(tipoMovimentoParam);
                                        if (pedagio == null)
                                        {
                                            pedagio = new Dominio.Entidades.Embarcador.Pedagio.Pedagio();
                                            pedagio.Categoria = categoria;
                                            pedagio.Data = data;
                                            pedagio.ImportadoDeSemParar = true;
                                            pedagio.MarcaVeiculo = marca;
                                            pedagio.Praca = praca;
                                            pedagio.Prefixo = prefixo;
                                            pedagio.Rodovia = rodovia;
                                            pedagio.Tag = tag;
                                            pedagio.Valor = valor;
                                            pedagio.DataImportacao = DateTime.Now.Date;
                                            pedagio.DataAlteracao = DateTime.Now;
                                            pedagio.TipoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito;

                                            // Caso o veiculo nao esteja cadastrado ou seja de terceiros, o pedagio fica com Inconsistencia
                                            if (veiculo != null)
                                            {
                                                pedagio.Veiculo = veiculo;
                                                if (veiculo.Tipo.ToUpper() == "P")
                                                    pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado;
                                                else
                                                {
                                                    pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Inconsistente;
                                                    pedagio.TipoMovimento = tipoMovimento;
                                                }
                                            }
                                            else
                                            {
                                                pedagio.PlacaVeiculoNaoCadastrado = placa;
                                                pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Inconsistente;
                                            }

                                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                                                pedagio.Empresa = this.Usuario.Empresa;

                                            pedagio.Motorista = veiculo?.Motoristas?.FirstOrDefault().Motorista ?? null;

                                            repPedagio.Inserir(pedagio, Auditado);

                                            unitOfWork.CommitChanges();
                                        }

                                        if (pedagio.SituacaoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Inconsistente)
                                            possuiPedagioInconsistente = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        unitOfWork.Rollback();
                                        serNotificacao.GerarNotificacao(this.Usuario, 0, "Frotas/Pedagio", string.Format(Localization.Resources.Frotas.ImportacaoPedagio.ImportacaoPedagio.OcorreuFalhaImportarPedagio, indiceLinha, ex.Message.ToString()), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pedagio, TipoServicoMultisoftware, unitOfWork);
                                        return new JsonpResult(false, "Linha: " + indiceLinha + " Erro: " + ex.Message.ToString());
                                    }

                                    if (indiceLinha % 25 == 0)
                                    {
                                        unitOfWork.Dispose();
                                        unitOfWork = null;
                                        unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                                        repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                                        repVeiculo = new Repositorio.Veiculo(unitOfWork);
                                    }

                                    int processados = (int)(100 * indiceLinha) / totalLinhas;
                                    serNotificacao.InfomarPercentualProcessamento(null, 0, "Frotas/Pedagio", (decimal)processados, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pedagio, TipoServicoMultisoftware, unitOfWork);
                                }
                            }
                            else
                            {
                                string msgRetorno = string.Empty;
                                if (!ProcessarPlanilhaPedagiosAFaturar(ref possuiPedagioInconsistente, ref repVeiculo, ref repPedagio, ref ds, ref unitOfWork, ref serNotificacao, ref msgRetorno))
                                {
                                    if (!string.IsNullOrWhiteSpace(msgRetorno))
                                        return new JsonpResult(false, msgRetorno);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            return new JsonpResult(false, "Erro ao acessar os dados: " + ex.Message);
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }
                    else
                    {
                        return new JsonpResult(false, "Arquivo selecionado não está de acordo com o layout do Sem Parar!");
                    }

                    //unitOfWork.CommitChanges();
                    string inconsistencia = string.Empty;
                    if (possuiPedagioInconsistente)
                        inconsistencia = ", porém existem inconsistências";

                    serNotificacao.GerarNotificacao(this.Usuario, 0, "Frotas/Pedagio", string.Format(Localization.Resources.Frotas.ImportacaoPedagio.ImportacaoPedagio.ImportacaoPedagioConcluida, inconsistencia), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pedagio, TipoServicoMultisoftware, unitOfWork);
                    return new JsonpResult(new { Sucesso = !possuiPedagioInconsistente, Mensagem = string.Format(Localization.Resources.Frotas.ImportacaoPedagio.ImportacaoPedagio.ImportacaoPedagioConcluida, inconsistencia) });
                }
                else
                {
                    return new JsonpResult(false, "Arquivo não encontrado, por favor verifique!");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPedagios();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> pedagios = new List<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, pedagios, ((dados) =>
                {
                    Servicos.Embarcador.Frota.PedagioImportacao servicoPedagioImportacao = new Servicos.Embarcador.Frota.PedagioImportacao(unitOfWork, TipoServicoMultisoftware, Empresa, dados);

                    return servicoPedagioImportacao.ObterPedagioImportar();
                }));

                if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");

                int totalRegistrosImportados = 0;
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                bool permiteInserir = (bool)parametro.Inserir;
                bool permiteAtualizar = (bool)parametro.Atualizar;
                Repositorio.Embarcador.Pedagio.Pedagio repositorioPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio in pedagios)
                {
                    if ((pedagio.Codigo > 0) && permiteAtualizar)
                    {
                        repositorioPedagio.Atualizar(pedagio, Auditado);
                        totalRegistrosImportados++;
                    }
                    else if ((pedagio.Codigo == 0) && permiteInserir)
                    {
                        repositorioPedagio.Inserir(pedagio, Auditado);
                        totalRegistrosImportados++;
                    }
                }

                unitOfWork.CommitChanges();

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoPedagios()
        {
            List<ConfiguracaoImportacao> configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "Placa", Propriedade = "Placa", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = "Valor", Propriedade = "Valor", Tamanho = tamanho, Obrigatorio = true, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Data Passagem", Propriedade = "Data", Tamanho = tamanho, Obrigatorio = true, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = "Hora Passagem", Propriedade = "Hora", Tamanho = tamanho, Obrigatorio = true, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Tipo", Propriedade = "Tipo", Tamanho = tamanho, Obrigatorio = true, CampoInformacao = true, Regras = new List<string> { "required" } });

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Praça", Propriedade = "Praca", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = "Rodovia", Propriedade = "Rodovia", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = "Observação", Propriedade = "Observacao", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 9, Descricao = "Movimento Financeiro", Propriedade = "MovimentoFinanceiro", Tamanho = tamanho, CampoInformacao = true });

            return configuracoes;
        }

        private bool ProcessarPlanilhaPedagiosAFaturar(ref bool possuiPedagioInconsistente, ref Repositorio.Veiculo repVeiculo, ref Repositorio.Embarcador.Pedagio.Pedagio repPedagio, ref DataSet ds, ref Repositorio.UnitOfWork unitOfWork, ref Servicos.Embarcador.Notificacao.Notificacao serNotificacao, ref string msgRetorno)
        {
            msgRetorno = string.Empty;
            possuiPedagioInconsistente = false;

            try
            {
                serNotificacao.InfomarPercentualProcessamento(null, 0, "Frotas/Pedagio", 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pedagio, TipoServicoMultisoftware, unitOfWork);

                int indiceLinha = 1;
                int totalLinhas = ds.Tables[0].Rows.Count;
                foreach (DataRow linha in ds.Tables[0].Rows)
                {
                    indiceLinha++;
                    if (indiceLinha > 3)
                    {
                        try
                        {
                            unitOfWork.Start();
                            string placa = linha[2].ToString();

                            if (string.IsNullOrWhiteSpace(placa))
                                continue;

                            string tag = linha[8].ToString();
                            string prefixo = linha[7].ToString();
                            string marca = linha[3].ToString();
                            string rodovia = linha[4].ToString();
                            string praca = linha[9].ToString();//Coluna Embarcador


                            int categoria = 0;
                            decimal valor = 0;
                            DateTime data;

                            DateTime.TryParseExact(linha[0].ToString() + linha[1].ToString(), "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data);
                            if (data == DateTime.MinValue)
                                DateTime.TryParseExact(linha[0].ToString() + " " + linha[1].ToString(), "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data);
                            int.TryParse(linha[8].ToString(), out categoria);
                            decimal.TryParse(linha[6].ToString(), out valor);

                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio;

                            if (prefixo == "CR")
                                tipoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Credito;
                            else
                                tipoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito;


                            Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorDados(tipoPedagio, placa, data, rodovia, praca);
                            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(placa);
                            if (pedagio == null)
                            {
                                pedagio = new Dominio.Entidades.Embarcador.Pedagio.Pedagio();
                                pedagio.Categoria = categoria;
                                pedagio.Data = data;
                                pedagio.ImportadoDeSemParar = true;
                                pedagio.MarcaVeiculo = marca;
                                pedagio.Praca = praca;
                                pedagio.Prefixo = prefixo;
                                pedagio.Rodovia = rodovia;
                                pedagio.Tag = tag;
                                pedagio.Valor = tipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Credito ? (valor * -1) : valor;
                                pedagio.DataImportacao = DateTime.Now.Date;
                                pedagio.DataAlteracao = DateTime.Now;
                                pedagio.TipoPedagio = tipoPedagio;
                                pedagio.Observacao = praca;

                                // Caso o veiculo nao esteja cadastrado ou seja de terceiros, o pedagio fica com Inconsistencia
                                if (veiculo != null)
                                {
                                    pedagio.Veiculo = veiculo;
                                    if (veiculo.Tipo.ToUpper() == "P")
                                        pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado;
                                    else
                                        pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Inconsistente;
                                }
                                else
                                {
                                    pedagio.PlacaVeiculoNaoCadastrado = placa;
                                    pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Inconsistente;
                                }

                                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                                    pedagio.Empresa = this.Usuario.Empresa;

                                repPedagio.Inserir(pedagio, Auditado);

                                unitOfWork.CommitChanges();
                            }

                            if (pedagio.SituacaoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Inconsistente)
                                possuiPedagioInconsistente = true;

                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            serNotificacao.GerarNotificacao(this.Usuario, 0, "Frotas/Pedagio", string.Format(Localization.Resources.Frotas.ImportacaoPedagio.ImportacaoPedagio.OcorreuFalhaImportarPedagio, indiceLinha, ex.Message.ToString()), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pedagio, TipoServicoMultisoftware, unitOfWork);
                            msgRetorno = "Linha: " + indiceLinha + " Erro: " + ex.Message.ToString();
                            return false;
                        }
                    }
                    if (indiceLinha % 25 == 0)
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                        unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                        repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);
                        repVeiculo = new Repositorio.Veiculo(unitOfWork);
                    }

                    int processados = (int)(100 * indiceLinha) / totalLinhas;
                    serNotificacao.InfomarPercentualProcessamento(null, 0, "Frotas/Pedagio", (decimal)processados, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pedagio, TipoServicoMultisoftware, unitOfWork);
                }
            }
            catch (Exception ex)
            {
                msgRetorno = ex.Message;
                return false;
            }

            return true;
        }

        #endregion
    }
}
