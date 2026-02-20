using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.OleDb;
using System.IO;
namespace SGT.WebAdmin.Controllers.Importacoes
{
    [AllowAnonymous]
    public class ImportacaoProsystController : BaseController
    {
		#region Construtores

		public ImportacaoProsystController(Conexao conexao) : base(conexao) { }

		#endregion

        //GET: ImportacaoProsyst
        public async Task<IActionResult> Index()
        {
            return View();
        }

        #region Métodos Globais

        [HttpPost]
        public async Task<IActionResult> BuscarDisntaciasDasAPIs()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Rota> rotas = repRota.buscarTodasRotas();
                Servicos.Embarcador.Logistica.MapRequestApi serMapApi = new Servicos.Embarcador.Logistica.MapRequestApi(unitOfWork);
                int numeroNaoEncontrou = 0;
                foreach (Dominio.Entidades.Embarcador.Logistica.Rota rota in rotas)
                {
                    try
                    {
                        rota.DistanciaKMAPI = serMapApi.BuscarDistanciaMapRequestApi(rota.Origem.Descricao + ", " + rota.Origem.Estado.Sigla, rota.Destino.Descricao + ", " + rota.Destino.Estado.Sigla);
                        repRota.Atualizar(rota);
                    }
                    catch (Exception ex)
                    {
                        numeroNaoEncontrou++;
                        Servicos.Log.TratarErro(ex);
                    }
                }

                ViewBag.Retorno = "Terminou Busca. Não Encontrou: " + numeroNaoEncontrou.ToString();
                return View();
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarDadosBancariosTerceiros()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int quantidadeImportada = 0;
                System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                DataSet ds = new DataSet();

                Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

                if (customFile.Length > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {

                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        customFile.SaveAs(fileLocation);
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
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            int indiceLinha = 1;

                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                indiceLinha++;
                                try
                                {
                                    if (!string.IsNullOrEmpty(linha["CNPJ/CPF"].ToString()) && long.Parse(Utilidades.String.OnlyNumbers(linha["CNPJ/CPF"].ToString())) > 0)
                                    {
                                        double cnpj = double.Parse(Utilidades.String.OnlyNumbers(linha["CNPJ/CPF"].ToString()));//long.Parse().ToString("d14");

                                        Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpj);
                                        if (cliente != null)
                                        {
                                            if (!string.IsNullOrEmpty(linha["Banco "].ToString()))
                                            {
                                                string[] split = linha["Banco "].ToString().Split('-');
                                                if (int.Parse(split[0]) > 0)
                                                {
                                                    cliente.Banco = repBanco.BuscarPorNumero(int.Parse(split[0]));
                                                    if (cliente.Banco != null)
                                                    {
                                                        cliente.Agencia = linha[5].ToString();
                                                        cliente.NumeroConta = linha[6].ToString();
                                                        cliente.TipoContaBanco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente;
                                                        repCliente.Atualizar(cliente);
                                                        quantidadeImportada++;
                                                    }
                                                    else
                                                    {
                                                        sbErro.Append("Linha: ").Append(indiceLinha).Append("Codigo Banco não encontrado: (").Append(linha["Código do Banco"].ToString()).Append(") ").AppendLine("");
                                                    }
                                                }
                                                else
                                                {
                                                    sbErro.Append("Linha: ").Append(indiceLinha).Append("Codigo Banco não encontrado: (").Append(linha["Código do Banco"].ToString()).Append(") ").AppendLine("");
                                                }
                                            }
                                            else
                                            {
                                                sbErro.Append("Linha: ").Append(indiceLinha).Append("Codigo Banco não encontrado: (").Append(linha["Código do Banco"].ToString()).Append(") ").AppendLine("");
                                            }
                                        }
                                        else
                                        {
                                            sbErro.Append("Linha: ").Append(indiceLinha).Append("Cliente não localizado: (").Append(linha["CNPJ/CPF"].ToString()).Append(") ").AppendLine("");
                                        }
                                    }
                                    else
                                    {
                                        sbErro.Append("Linha: ").Append(indiceLinha).Append(" CNPJ INVALIDO: (").Append(linha["CNPJ/CPF"].ToString()).Append(") ").AppendLine("");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }

                    sbErro.Append("Terminou importação. Quantidade Importada = " + quantidadeImportada);
                    string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Transp.txt");
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                    ViewBag.Retorno = "Terminou";


                }
                return View();
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarTabelaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

                Servicos.Embarcador.Logistica.MapRequestApi serMapRequestApi = new Servicos.Embarcador.Logistica.MapRequestApi(unitOfWork);
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteRota repFreteRota = new Repositorio.Embarcador.Frete.TabelaFreteRota(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCarga repFreteTipoCarga = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga repFreteRotaTipoCargaModeloVeicular = new Repositorio.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga(unitOfWork);

                string colOrigem = "FCFM_COD_MUN_ORIG";
                string colDestino = "FCFM_COD_MUN_DEST";
                string colDescricaoDestino = "FCFM_DESC_DESTINO";
                string colTipoCarga = "FCFM_COD_TPFRETE";
                string colDistancia = "FCFM_DISTANCIA";
                string colModeloVeicular = "FCFV_COD_VEICULO";
                string colValorFrete = "FCFV_VLR_FRETE";
                string colValorPedagio = "FCFV_VLR_PEDAGIO";
                string colAtivo = "FCFM_SITUACAO";
                string codFreteEmbarcador = "FCFV_COD_FRETE";
                bool removerICMS = true;
                int codigoTabelaFretePadrao = 1;

                DataSet ds = new DataSet();

                Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

                if (customFile.Length > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {

                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        //string fileLocation = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + fileExtension;
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        customFile.SaveAs(fileLocation);
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
                            //excelConnectionString = "Provider=Microsoft.ACE.OLEDB.14.0;Data Source=" +
                            //fileLocation + ";Extended Properties=\"Excel 14.0;HDR=Yes;IMEX=2\"";

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
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            int indiceLinha = 1;
                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                indiceLinha++;
                                try
                                {
                                    Dominio.Entidades.Localidade origem = repLocalidade.BuscarPorCodigoIBGE(int.Parse(linha[colOrigem].ToString()));
                                    Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorCodigoIBGE(int.Parse(linha[colDestino].ToString()));
                                    if (origem != null && destino != null)
                                    {
                                        Dominio.Entidades.Embarcador.Logistica.Rota rota = repRota.BuscarRotaPorOrigemDestino(origem.Codigo, destino.Codigo);
                                        if (rota == null)
                                        {
                                            rota = new Dominio.Entidades.Embarcador.Logistica.Rota();
                                            rota.Ativo = true;
                                            rota.Destino = destino;
                                            rota.Origem = origem;
                                            rota.PossuiPedagio = false;
                                            try
                                            {
                                                rota.DistanciaKM = serMapRequestApi.BuscarDistanciaMapRequestApi(origem.Descricao + ", " + origem.Estado.Sigla, destino.Descricao + ", " + destino.Estado.Sigla);
                                                if (rota.DistanciaKM <= 0)
                                                {
                                                    decimal km = decimal.Parse(linha[colDistancia].ToString());
                                                    rota.DistanciaKM = (int)Math.Round(km, 0, MidpointRounding.AwayFromZero);
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                decimal km = decimal.Parse(linha[colDistancia].ToString());
                                                rota.DistanciaKM = (int)Math.Round(km, 0, MidpointRounding.AwayFromZero);
                                            }

                                            repRota.Inserir(rota);
                                        }

                                        string descricaoDestino = linha[colDescricaoDestino].ToString().TrimEnd();
                                        Dominio.Entidades.Embarcador.Frete.TabelaFreteRota tabelaFreteRota = repFreteRota.BuscarPorCodigoEmbarcador(linha[codFreteEmbarcador].ToString());

                                        if (tabelaFreteRota == null)
                                        {
                                            tabelaFreteRota = new Dominio.Entidades.Embarcador.Frete.TabelaFreteRota();
                                            tabelaFreteRota.Ativo = false;
                                            tabelaFreteRota.DescricaoDestinos = descricaoDestino;
                                            tabelaFreteRota.Origem = origem;
                                            tabelaFreteRota.Destino = destino;
                                            tabelaFreteRota.CodigoEmbarcador = linha[codFreteEmbarcador].ToString();
                                            tabelaFreteRota.TabelaFrete = new Dominio.Entidades.Embarcador.Frete.TabelaFrete() { Codigo = codigoTabelaFretePadrao };
                                            repFreteRota.Inserir(tabelaFreteRota);
                                        }
                                        else
                                        {
                                            if (descricaoDestino != tabelaFreteRota.DescricaoDestinos || tabelaFreteRota.Origem.Codigo != origem.Codigo || tabelaFreteRota.Destino.Codigo != destino.Codigo)
                                            {
                                                tabelaFreteRota.DescricaoDestinos = descricaoDestino;
                                                tabelaFreteRota.Origem = origem;
                                                tabelaFreteRota.Destino = destino;
                                                repFreteRota.Atualizar(tabelaFreteRota);
                                            }
                                        }

                                        Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoCarga.BuscarPorCodigoEmbarcador(linha[colTipoCarga].ToString().Trim());
                                        if (tipoCarga != null)
                                        {
                                            Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga tabelaFreteRotaTipoCarga = repFreteTipoCarga.BuscarPorTabelaTipoCarga(tabelaFreteRota.Codigo, tipoCarga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos);
                                            if (tabelaFreteRotaTipoCarga == null)
                                            {
                                                tabelaFreteRotaTipoCarga = new Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCarga();
                                                if (int.Parse(linha[colAtivo].ToString()) == 1)
                                                    tabelaFreteRotaTipoCarga.Ativo = true;
                                                else
                                                    tabelaFreteRotaTipoCarga.Ativo = false;
                                                tabelaFreteRotaTipoCarga.TabelaFreteRota = tabelaFreteRota;
                                                tabelaFreteRotaTipoCarga.TipoDeCarga = tipoCarga;
                                                repFreteTipoCarga.Inserir(tabelaFreteRotaTipoCarga);
                                            }
                                            if (tabelaFreteRotaTipoCarga.Ativo && !tabelaFreteRota.Ativo)
                                            {
                                                tabelaFreteRota.Ativo = true;
                                                repFreteRota.Atualizar(tabelaFreteRota);
                                            }
                                            if (!string.IsNullOrEmpty(linha[colModeloVeicular].ToString()))
                                            {
                                                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicular.buscarPorCodigoIntegracao(linha[colModeloVeicular].ToString().Trim());
                                                if (modeloVeicularCarga != null)
                                                {

                                                    Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga freteTCModeloVeicular = repFreteRotaTipoCargaModeloVeicular.BuscarPorTipoCargaModeloVeicular(tabelaFreteRotaTipoCarga.Codigo, modeloVeicularCarga.Codigo);
                                                    decimal valorFrete = decimal.Parse(linha[colValorFrete].ToString());
                                                    decimal valorPedagio = decimal.Parse(linha[colValorPedagio].ToString());



                                                    if (freteTCModeloVeicular == null)
                                                    {
                                                        freteTCModeloVeicular = new Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga();
                                                        freteTCModeloVeicular.ModeloVeicularCarga = modeloVeicularCarga;
                                                        freteTCModeloVeicular.TabelaFreteRotaTipoCarga = tabelaFreteRotaTipoCarga;
                                                        freteTCModeloVeicular.ValorPedagio = valorPedagio;
                                                        freteTCModeloVeicular.ValorFreteComICMS = valorFrete;
                                                        if (removerICMS)
                                                            freteTCModeloVeicular.ValorFrete = Math.Round(valorFreteSemICMS(tabelaFreteRota.Origem.Estado.Sigla, tabelaFreteRota.Destino.Estado.Sigla, valorFrete), 2, MidpointRounding.AwayFromZero);
                                                        else
                                                            freteTCModeloVeicular.ValorFrete = valorFrete;

                                                        repFreteRotaTipoCargaModeloVeicular.Inserir(freteTCModeloVeicular);
                                                    }
                                                    else
                                                    {
                                                        freteTCModeloVeicular.ValorPedagio = valorPedagio;
                                                        freteTCModeloVeicular.ValorFreteComICMS = valorFrete;
                                                        if (removerICMS)
                                                            freteTCModeloVeicular.ValorFrete = Math.Round(valorFreteSemICMS(tabelaFreteRota.Origem.Estado.Sigla, tabelaFreteRota.Destino.Estado.Sigla, valorFrete), 2, MidpointRounding.AwayFromZero);
                                                        else
                                                            freteTCModeloVeicular.ValorFrete = valorFrete;

                                                        repFreteRotaTipoCargaModeloVeicular.Atualizar(freteTCModeloVeicular);
                                                    }
                                                }
                                                else
                                                {
                                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Modelo Veícular: ").Append(linha[colModeloVeicular].ToString()).Append(". Não Encontrado ").AppendLine(""); ;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            sbErro.Append("Linha: ").Append(indiceLinha).Append(" Tipo de Carga: ").Append(linha[colTipoCarga].ToString()).Append(". Não Encontrada ").AppendLine(""); ;
                                        }
                                    }
                                    else
                                    {
                                        if (origem == null)
                                            sbErro.Append("Linha: ").Append(indiceLinha).Append(" Origem: ").Append(linha[colOrigem].ToString()).Append(". Não Encontrada ").AppendLine(""); ;
                                        if (destino == null)
                                            sbErro.Append("Linha: ").Append(indiceLinha).Append(" Origem: ").Append(linha[colDestino].ToString()).Append(". Não Encontrada ").AppendLine(""); ;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }

                    sbErro.Append("Terminou importação");
                    string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Frete.txt");
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());
                    ViewBag.Retorno = "Terminou Importação";
                }
                return View();
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                DataSet ds = new DataSet();

                Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

                if (customFile.Length > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {

                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        customFile.SaveAs(fileLocation);
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
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            int indiceLinha = 1;
                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                indiceLinha++;
                                try
                                {
                                    if (!string.IsNullOrEmpty(linha["TR_NUM_CGC"].ToString()) && long.Parse(linha["TR_NUM_CGC"].ToString()) > 0)
                                    {
                                        string cnpj = long.Parse(linha["TR_NUM_CGC"].ToString()).ToString("d14");

                                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);
                                        if (empresa == null)
                                        {
                                            empresa = new Dominio.Entidades.Empresa();
                                            empresa.Bairro = linha["TR_BAIRRO"].ToString().Trim();
                                            empresa.CEP = linha["TR_CEP"].ToString().TrimEnd();
                                            empresa.CNPJ = cnpj;
                                            empresa.Complemento = "";
                                            empresa.DataAtualizacao = DateTime.Now;
                                            if (linha["TR_E_MAIL"].ToString() == "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")
                                                empresa.Email = "";
                                            else
                                                empresa.Email = linha["TR_E_MAIL"].ToString().Trim();

                                            empresa.EmpresaPai = new Dominio.Entidades.Empresa() { Codigo = 1 };
                                            empresa.Endereco = linha["TR_ENDERECO"].ToString().Trim();
                                            if (empresa.Endereco.Split(',').Length == 2)
                                            {
                                                int result;
                                                if (int.TryParse(empresa.Endereco.Split(',')[1], out result))
                                                {
                                                    empresa.Endereco = empresa.Endereco.Split(',')[0];
                                                    empresa.Numero = result.ToString();
                                                }
                                            }
                                            else
                                            {
                                                empresa.Numero = "";
                                            }

                                            empresa.FusoHorario = "E. South America Standard Time";
                                            empresa.InscricaoEstadual = linha["TR_INSCR_ESTADUAL"].ToString().Trim();
                                            empresa.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(linha["TR_COD_MUNICIP"].ToString()));
                                            if (empresa.Localidade != null)
                                            {
                                                empresa.OptanteSimplesNacional = false;
                                                empresa.RazaoSocial = linha["TR_NOME_TRANSP"].ToString().Trim();
                                                empresa.NomeFantasia = empresa.RazaoSocial;
                                                if (int.Parse(linha["TR_COD_RNTRC"].ToString()) > 0)
                                                    empresa.RegistroANTT = linha["TR_COD_RNTRC"].ToString();
                                                else
                                                    empresa.RegistroANTT = "";

                                                if (int.Parse(linha["TR2_SITUACAO"].ToString()) == 0)
                                                    empresa.Status = "A";
                                                else
                                                    empresa.Status = "I";

                                                if (!string.IsNullOrWhiteSpace(empresa.Email))
                                                    empresa.StatusEmail = "A";
                                                else
                                                    empresa.StatusEmail = "I";

                                                empresa.StatusEmailAdministrativo = "I";
                                                empresa.StatusEmailContador = "I";
                                                empresa.StatusEmissao = "I";
                                                empresa.Telefone = "";
                                                if (!string.IsNullOrEmpty(linha["TR_FONE"].ToString().Trim()))
                                                {
                                                    string telefone = Utilidades.String.OnlyNumbers(linha["TR_FONE"].ToString());
                                                    empresa.Telefone = retornaTelefone(telefone);
                                                }
                                                empresa.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Homologacao;
                                                repEmpresa.Inserir(empresa);
                                            }
                                            else
                                            {
                                                sbErro.Append("Linha: ").Append(indiceLinha).Append(" Localidade Invalidada: (").Append(linha["TR_COD_MUNICIP"].ToString()).Append(") ").Append(linha["TR_NOME_TRANSP"].ToString()).Append(" NR Transp:").Append(linha["TR_NR_TRANSP"].ToString()).AppendLine("");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        sbErro.Append("Linha: ").Append(indiceLinha).Append(" CNPJ INVALIDO: (").Append(linha["TR_NUM_CGC"].ToString()).Append(") ").Append(linha["TR_NOME_TRANSP"].ToString()).Append(" NR Transp:").Append(linha["TR_NR_TRANSP"].ToString()).AppendLine("");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }

                    sbErro.Append("Terminou importação");
                    string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Transp.txt");
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                    ViewBag.Retorno = "Terminou";


                }
                return View();
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarPedidosSimonetti()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                DataSet ds = new DataSet();

                Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

                if (customFile.Length > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {

                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        customFile.SaveAs(fileLocation);
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
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            int indiceLinha = 1;
                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                indiceLinha++;
                                try
                                {
                                    string tipoPedido = linha["tipoPedido"].ToString();
                                    string numeroPedido = linha["codigoInterno"].ToString();
                                    string remetente = linha["remetente"].ToString();
                                    string remetente_cnpj = linha["remetente_cnpj"].ToString();
                                    string endereco_logradouro = linha["endereco_logradouro"].ToString();
                                    string endereco_bairro = linha["endereco_bairro"].ToString();
                                    string endereco_cidade = linha["endereco_cidade"].ToString();
                                    string endereco_cep = linha["endereco_cep"].ToString();
                                    string endereco_complemento = linha["endereco_complemento"].ToString();
                                    string data_pedido = linha["data_pedido"].ToString();
                                    string cliente = linha["cliente"].ToString();
                                    string cliente_cnpj = linha["cliente_cnpj"].ToString();
                                    string endereco_entrega = linha["endereco_entrega"].ToString();
                                    string bairro_entrega = linha["bairro_entrega"].ToString();
                                    string cidade_entrega = linha["cidade_entrega"].ToString();
                                    string cep_entrega = linha["cep_entrega"].ToString();
                                    string complemento_entrega = linha["complemento_entrega"].ToString();
                                    string PesoEntrega = linha["PesoEntrega"].ToString();
                                    string VolumeEntrega = linha["VolumeEntrega"].ToString();

                                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();

                                    pedido.CodigoPedidoCliente = numeroPedido;
                                    pedido.CubagemTotal = decimal.Parse(VolumeEntrega.Replace(".", ","));
                                    pedido.PesoTotal = decimal.Parse(PesoEntrega.Replace(".", ","));

                                    DateTime dataCarregamento;
                                    if (!DateTime.TryParseExact(data_pedido, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dataCarregamento))
                                    {
                                        dataCarregamento = DateTime.Now;
                                    };
                                    pedido.DataCarregamentoPedido = dataCarregamento;
                                    pedido.DataInicialColeta = dataCarregamento;
                                    pedido.DataFinalColeta = dataCarregamento;
                                    pedido.DataPrevisaoSaida = dataCarregamento;
                                    pedido.DataInicialViagemExecutada = dataCarregamento;
                                    pedido.DataFinalViagemExecutada = dataCarregamento;
                                    pedido.DataInicialViagemFaturada = dataCarregamento;
                                    pedido.DataFinalViagemFaturada = dataCarregamento;

                                    pedido.UltimaAtualizacao = DateTime.Now;

                                    Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(cliente_cnpj));
                                    if (destinatario == null)
                                    {
                                        destinatario = new Dominio.Entidades.Cliente();
                                        destinatario.CPF_CNPJ = double.Parse(cliente_cnpj);

                                        if (cliente_cnpj.Length <= 11)
                                        {
                                            destinatario.Tipo = "F";
                                            destinatario.Atividade = new Dominio.Entidades.Atividade() { Codigo = 7 };
                                        }
                                        else
                                        {
                                            destinatario.Tipo = "J";
                                            destinatario.Atividade = new Dominio.Entidades.Atividade() { Codigo = 3 };
                                        }

                                        destinatario.Bairro = bairro_entrega;
                                        destinatario.CEP = cep_entrega;

                                        List<Dominio.Entidades.Localidade> localidades = repLocalidade.BuscarPorDescricao(cidade_entrega);
                                        if (localidades.Count > 0)
                                        {
                                            Dominio.Entidades.Localidade localidade = (from obj in localidades where obj.Estado.Sigla == "ES" select obj).FirstOrDefault();
                                            if (localidade == null)
                                                localidade = localidades.FirstOrDefault();

                                            destinatario.Cidade = localidade.Descricao;
                                            destinatario.Localidade = localidade;

                                            destinatario.DataCadastro = DateTime.Now;

                                            destinatario.IE_RG = "ISENTO";
                                            destinatario.Nome = cliente;

                                            if (complemento_entrega.Length > 60)
                                                destinatario.Complemento = complemento_entrega.Substring(0, 60);
                                            else
                                                destinatario.Complemento = complemento_entrega;

                                            string[] enderecoSplit = endereco_entrega.Split(',');
                                            if (enderecoSplit.Length > 1)
                                            {
                                                destinatario.Endereco = enderecoSplit[0];
                                                destinatario.Numero = enderecoSplit[1];
                                            }
                                            else
                                            {
                                                destinatario.Endereco = enderecoSplit[0];
                                                destinatario.Numero = "S/N";
                                            }
                                            destinatario.NomeFantasia = cliente;
                                            destinatario.TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Comercial;
                                            destinatario.Ativo = true;
                                            repCliente.Inserir(destinatario);
                                            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                                            modalidade.Cliente = destinatario;
                                            modalidade.TipoModalidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente;
                                            repModalidadePessoas.Inserir(modalidade);

                                            Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas modalidadeClientePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas();

                                            modalidadeClientePessoas.ModalidadePessoas = modalidade;
                                            repModalidadeClientePessoas.Inserir(modalidadeClientePessoas);
                                        }
                                        else
                                        {
                                            sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append("Localidade destinatario não encontrada").AppendLine("");
                                        }
                                    }


                                    Dominio.Entidades.Cliente remetentePedido = repCliente.BuscarPorCPFCNPJ(double.Parse(remetente_cnpj));
                                    if (remetentePedido == null)
                                    {
                                        remetentePedido = new Dominio.Entidades.Cliente();
                                        remetentePedido.CPF_CNPJ = double.Parse(remetente_cnpj);

                                        if (remetente_cnpj.Length <= 11)
                                        {
                                            remetentePedido.Tipo = "F";
                                            remetentePedido.Atividade = new Dominio.Entidades.Atividade() { Codigo = 7 };
                                        }
                                        else
                                        {
                                            remetentePedido.Tipo = "J";
                                            remetentePedido.Atividade = new Dominio.Entidades.Atividade() { Codigo = 3 };
                                        }

                                        remetentePedido.Bairro = endereco_bairro;
                                        remetentePedido.CEP = endereco_cep;

                                        List<Dominio.Entidades.Localidade> localidades = repLocalidade.BuscarPorDescricao(endereco_cidade);
                                        if (localidades.Count > 0)
                                        {
                                            Dominio.Entidades.Localidade localidade = (from obj in localidades where obj.Estado.Sigla == "ES" select obj).FirstOrDefault();
                                            if (localidade == null)
                                                localidade = localidades.FirstOrDefault();

                                            remetentePedido.Cidade = localidade.Descricao;
                                            remetentePedido.Localidade = localidade;

                                            remetentePedido.DataCadastro = DateTime.Now;

                                            remetentePedido.IE_RG = "ISENTO";
                                            remetentePedido.Nome = remetente;

                                            if (endereco_complemento.Length > 60)
                                                remetentePedido.Complemento = endereco_complemento.Substring(0, 60);
                                            else
                                                remetentePedido.Complemento = endereco_complemento;

                                            string[] enderecoSplit = endereco_logradouro.Split(',');
                                            if (enderecoSplit.Length > 1)
                                            {
                                                remetentePedido.Endereco = enderecoSplit[0];
                                                remetentePedido.Numero = enderecoSplit[1];
                                            }
                                            else
                                            {
                                                remetentePedido.Endereco = enderecoSplit[0];
                                                remetentePedido.Numero = "S/N";
                                            }
                                            remetentePedido.NomeFantasia = remetente;
                                            remetentePedido.TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Comercial;
                                            remetentePedido.Ativo = true;
                                            repCliente.Inserir(remetentePedido);
                                            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                                            modalidade.Cliente = remetentePedido;
                                            modalidade.TipoModalidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente;
                                            repModalidadePessoas.Inserir(modalidade);

                                            Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas modalidadeClientePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas();

                                            modalidadeClientePessoas.ModalidadePessoas = modalidade;
                                            repModalidadeClientePessoas.Inserir(modalidadeClientePessoas);
                                        }
                                        else
                                        {
                                            sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append("Localidade destinatario não encontrada").AppendLine("");
                                        }
                                    }

                                    if (remetentePedido != null && destinatario != null)
                                    {
                                        pedido.Destinatario = destinatario;
                                        pedido.Destino = destinatario.Localidade;
                                        pedido.Filial = new Dominio.Entidades.Embarcador.Filiais.Filial() { Codigo = 30 };
                                        pedido.GerarAutomaticamenteCargaDoPedido = false;
                                        pedido.Remetente = remetentePedido;
                                        pedido.Origem = remetentePedido.Localidade;
                                        pedido.NumeroPedidoEmbarcador = numeroPedido;


                                        pedido.RotaFrete = repRotaFrete.BuscarPorLocalidade(pedido.Destino, true);
                                        if (pedido.RotaFrete == null)
                                            pedido.RotaFrete = repRotaFrete.BuscarPorEstado(pedido.Destino.Estado.Sigla, true);

                                        pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                                        pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaNormal;
                                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                                        pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                                        pedido.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa;
                                        pedido.Usuario = this.Usuario;
                                        pedido.Autor = this.Usuario;
                                        pedido.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
                                        repPedido.Inserir(pedido);

                                        pedido.Protocolo = pedido.Codigo;
                                        repPedido.Atualizar(pedido);

                                        Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                                        pedidoProduto.Pedido = pedido;
                                        pedidoProduto.PesoUnitario = pedido.PesoTotal;
                                        pedidoProduto.Produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador() { Codigo = 2265 };
                                        pedidoProduto.Quantidade = 1;
                                        pedidoProduto.ValorProduto = 1;
                                        repPedidoProduto.Inserir(pedidoProduto);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }

                    sbErro.Append("Terminou importação Produtos");
                    string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Transp.txt");
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                    ViewBag.Retorno = "Terminou";


                }
                return View();
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);


                DataSet ds = new DataSet();

                Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

                if (customFile.Length > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {

                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        customFile.SaveAs(fileLocation);
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
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            int indiceLinha = 1;
                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                indiceLinha++;
                                try
                                {
                                    if (!string.IsNullOrEmpty(linha["TR_NUM_CGC"].ToString()) && long.Parse(linha["TR_NUM_CGC"].ToString()) > 0)
                                    {
                                        string cnpj = long.Parse(linha["TR_NUM_CGC"].ToString()).ToString("d14");

                                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);
                                        if (empresa != null)
                                        {


                                            string placa = linha["MOT_PLACA_VEIC"].ToString().Replace("-", "");
                                            if (placa == "#" || placa.Length != 7)
                                            {
                                                placa = "";
                                            }
                                            if (!string.IsNullOrWhiteSpace(placa))
                                            {
                                                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, placa);
                                                if (veiculo == null)
                                                {
                                                    veiculo = new Dominio.Entidades.Veiculo();
                                                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.buscarPorCodigoIntegracao(linha["MOT_COD_TPVEICUL"].ToString());
                                                    veiculo.Empresa = empresa;
                                                    veiculo.ModeloVeicularCarga = modeloVeicular;
                                                    veiculo.Estado = new Dominio.Entidades.Estado() { Sigla = empresa.Localidade.Estado.Sigla };
                                                    veiculo.Placa = placa;
                                                    veiculo.Renavam = "";
                                                    veiculo.Ativo = true;
                                                    veiculo.Tipo = "P";
                                                    veiculo.TipoCarroceria = "00";
                                                    veiculo.TipoCombustivel = "D";
                                                    veiculo.TipoRodado = "00";
                                                    veiculo.TipoVeiculo = "0";
                                                    repVeiculo.Inserir(veiculo);
                                                }
                                            }

                                        }
                                        else
                                        {
                                            sbErro.Append("Linha: ").Append(indiceLinha).Append(" TRANSPORTADOR NÃO CADASTRADO: (").Append(linha["TR_NUM_CGC"].ToString()).Append(") ").Append(linha["TR_NOME_TRANSP"].ToString()).Append(" NR Transp:").Append(linha["TR_NR_TRANSP"].ToString()).AppendLine("");
                                        }
                                    }
                                    else
                                    {
                                        sbErro.Append("Linha: ").Append(indiceLinha).Append(" CNPJ TRANSP INVALIDO: (").Append(linha["TR_NUM_CGC"].ToString()).Append(") ").Append(linha["TR_NOME_TRANSP"].ToString()).Append(" NR Transp:").Append(linha["TR_NR_TRANSP"].ToString()).AppendLine("");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }

                    sbErro.Append("Terminou importação");
                    string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Veiculo.txt");
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                    ViewBag.Retorno = "Terminou";


                }
                return View();
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);


                DataSet ds = new DataSet();

                Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

                if (customFile.Length > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {

                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        customFile.SaveAs(fileLocation);
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
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            int indiceLinha = 1;
                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                indiceLinha++;
                                try
                                {
                                    if (!string.IsNullOrEmpty(linha["MT_DESC1_MATER"].ToString().Trim()))
                                    {
                                        if (!string.IsNullOrWhiteSpace(linha["MT_COD_MATER"].ToString().Trim()))
                                        {
                                            Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigoEmbarcador(linha["MT_CLASSE_TIPO"].ToString().Trim());
                                            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProduto.buscarPorCodigoEmbarcador(linha["MT_COD_MATER"].ToString().Trim());

                                            if (grupoProduto != null)
                                            {
                                                if (produto == null)
                                                {
                                                    produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador();
                                                    produto.Ativo = true;
                                                    produto.CodigoProdutoEmbarcador = linha["MT_COD_MATER"].ToString().Trim();
                                                    produto.Descricao = linha["MT_DESC1_MATER"].ToString().Trim();
                                                    produto.GrupoProduto = grupoProduto;
                                                    repProduto.Inserir(produto);
                                                }
                                                else
                                                {
                                                    if (produto.Descricao != linha["MT_DESC1_MATER"].ToString().Trim() || produto.GrupoProduto.Codigo != grupoProduto.Codigo)
                                                    {
                                                        produto.Descricao = linha["MT_DESC1_MATER"].ToString().Trim();
                                                        produto.GrupoProduto = grupoProduto;
                                                        repProduto.Atualizar(produto);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                sbErro.Append("Linha: ").Append(indiceLinha).Append(" Grupo de Produto não localizado ").AppendLine("");
                                            }
                                        }
                                        else
                                        {
                                            sbErro.Append("Linha: ").Append(indiceLinha).Append(" Código vazio ").AppendLine("");
                                        }
                                    }
                                    else
                                    {
                                        sbErro.Append("Linha: ").Append(indiceLinha).Append(" Descrição vazia. ").AppendLine("");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }

                    sbErro.Append("Terminou importação");
                    string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Produto.txt");
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                    ViewBag.Retorno = "Terminou";


                }
                return View();
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarGrupoProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);


                DataSet ds = new DataSet();

                Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

                if (customFile.Length > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {

                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        customFile.SaveAs(fileLocation);
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
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            int indiceLinha = 1;
                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                indiceLinha++;
                                try
                                {
                                    if (!string.IsNullOrEmpty(linha["CT_DESCRICAO"].ToString().Trim()))
                                    {
                                        if (!string.IsNullOrWhiteSpace(linha["CT_CLASSE_TIPO"].ToString().Trim()))
                                        {
                                            Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigoEmbarcador(linha["CT_CLASSE_TIPO"].ToString().Trim());

                                            if (grupoProduto == null)
                                            {
                                                grupoProduto = new Dominio.Entidades.Embarcador.Produtos.GrupoProduto();
                                                grupoProduto.Ativo = true;
                                                grupoProduto.CodigoGrupoProdutoEmbarcador = linha["CT_CLASSE_TIPO"].ToString().Trim();
                                                grupoProduto.Descricao = linha["CT_DESCRICAO"].ToString().Trim();
                                                repGrupoProduto.Inserir(grupoProduto);
                                            }
                                            else
                                            {
                                                if (grupoProduto.Descricao != linha["CT_DESCRICAO"].ToString().Trim())
                                                {
                                                    grupoProduto.Descricao = linha["CT_DESCRICAO"].ToString().Trim();
                                                    repGrupoProduto.Atualizar(grupoProduto);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            sbErro.Append("Linha: ").Append(indiceLinha).Append(" Código vazio ").AppendLine("");
                                        }
                                    }
                                    else
                                    {
                                        sbErro.Append("Linha: ").Append(indiceLinha).Append(" Descrição vazia. ").AppendLine("");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }

                    sbErro.Append("Terminou importação");
                    string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_GrupoProduto.txt");
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                    ViewBag.Retorno = "Terminou";


                }
                return View();
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarClientes()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeTrabalho);
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unidadeTrabalho);
                Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unidadeTrabalho);

                System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

                string colNome = "Cliente";
                string colEndereco = "Endereço";
                string colBairro = "Bairro";
                string colCidade = "Cidade";
                string colUF = "UF";
                string colCEP = "CEP";
                string colFone = "Fone";
                string colCPFCNPJ = "CNPJ";
                string colIE = "Inscr# Estadual";

                DataSet ds = new DataSet();

                Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

                if (customFile.Length > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        //string fileLocation = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao + DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + fileExtension;
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        customFile.SaveAs(fileLocation);
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
                            //excelConnectionString = "Provider=Microsoft.ACE.OLEDB.14.0;Data Source=" +
                            //fileLocation + ";Extended Properties=\"Excel 14.0;HDR=Yes;IMEX=2\"";

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
                                return null;

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            int indiceLinha = 1;
                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                indiceLinha++;

                                try
                                {
                                    if (indiceLinha % 30 == 0)
                                    {
                                        unidadeTrabalho.Dispose();
                                        unidadeTrabalho = null;
                                        repAtividade = null;
                                        repCliente = null;
                                        repLocalidade = null;
                                        repModalidadeClientePessoas = null;
                                        repModalidadePessoas = null;

                                        unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
                                        repAtividade = new Repositorio.Atividade(unidadeTrabalho);
                                        repCliente = new Repositorio.Cliente(unidadeTrabalho);
                                        repLocalidade = new Repositorio.Localidade(unidadeTrabalho);
                                        repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unidadeTrabalho);
                                        repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unidadeTrabalho);
                                    }

                                    double cpfCnpj;
                                    double.TryParse(Utilidades.String.OnlyNumbers(linha[colCPFCNPJ].ToString()), out cpfCnpj);

                                    if (cpfCnpj <= 0d)
                                    {
                                        sbErro.Append("Linha: ").Append(indiceLinha).Append(" CNPJ: ").Append(linha[colCPFCNPJ].ToString()).Append(". Não conseguiu formatar.").AppendLine();
                                        continue;
                                    }

                                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                                    if (cliente != null)
                                        continue;

                                    cliente = new Dominio.Entidades.Cliente();
                                    cliente.CPF_CNPJ = cpfCnpj;
                                    cliente.Tipo = "J";
                                    cliente.Atividade = repAtividade.BuscarPorCodigo(3);
                                    cliente.Bairro = linha[colBairro].ToString();

                                    string cep = Utilidades.String.OnlyNumbers(linha[colCEP].ToString());
                                    cliente.CEP = cep.Substring(0, 2) + "." + cep.Substring(2, 3) + "-" + cep.Substring(5, 3);

                                    cliente.DataCadastro = DateTime.Now;
                                    cliente.EnderecoDigitado = true;
                                    cliente.Localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(linha[colCidade].ToString()), linha[colUF].ToString());

                                    if (cliente.Localidade == null)
                                    {
                                        sbErro.Append("Linha: ").Append(indiceLinha).Append(" Localidade: ").Append(linha[colCidade].ToString()).Append("-").Append(linha[colUF].ToString()).Append(". Não Encontrada ").AppendLine();
                                        continue;
                                    }
                                    string endereco = linha[colEndereco].ToString();
                                    string[] enderecoSplit = endereco.Split(',');

                                    cliente.Endereco = enderecoSplit[0];

                                    if (enderecoSplit.Length > 0)
                                        cliente.Numero = enderecoSplit.LastOrDefault();

                                    if (string.IsNullOrWhiteSpace(cliente.Numero))
                                        cliente.Numero = "S/N";

                                    cliente.Nome = linha[colNome].ToString();
                                    cliente.NomeFantasia = cliente.Nome;
                                    cliente.Telefone1 = Utilidades.String.OnlyNumbers(linha[colFone].ToString());
                                    cliente.IE_RG = Utilidades.String.OnlyNumbers(linha[colIE].ToString());

                                    if (string.IsNullOrWhiteSpace(cliente.IE_RG))
                                        cliente.IE_RG = "ISENTO";

                                    cliente.TipoEmail = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Principal;
                                    cliente.TipoLogradouro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Rua;
                                    cliente.TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Comercial;
                                    cliente.Ativo = true;
                                    unidadeTrabalho.Start();


                                    repCliente.Inserir(cliente);

                                    Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                                    modalidadePessoas.Cliente = cliente;
                                    modalidadePessoas.TipoModalidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente;

                                    repModalidadePessoas.Inserir(modalidadePessoas);

                                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas modaliadeClientePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas();
                                    modaliadeClientePessoas.ModalidadePessoas = modalidadePessoas;

                                    repModalidadeClientePessoas.Inserir(modaliadeClientePessoas);

                                    unidadeTrabalho.CommitChanges();
                                }
                                catch (Exception ex)
                                {
                                    unidadeTrabalho.Rollback();
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine();
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }

                    sbErro.Append("Terminou importação");
                    string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Clientes.txt");
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());
                    ViewBag.Retorno = "Terminou Importação";

                }
                return View();
            }
            catch
            {
                throw;
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private decimal valorFreteSemICMS(string ufOrigem, string ufDestino, decimal valorFrete)
        {
            decimal valor = valorFrete;

            if (ufOrigem == "GO")
            {
                valor = valorFrete - (valorFrete * (decimal)0.12);
            }

            if (ufOrigem == "PR")
            {
                if (ufDestino == "MG" || ufDestino == "RS" || ufDestino == "RJ" || ufDestino == "SC" || ufDestino == "SP")
                {
                    valor = valorFrete - (valorFrete * (decimal)0.12);
                }
                else
                {
                    if (ufDestino != "PR")
                    {
                        valor = valorFrete - (valorFrete * (decimal)0.07);
                    }
                }
            }


            if (ufOrigem == "RS")
            {
                if (ufDestino == "MG" || ufDestino == "PR" || ufDestino == "RJ" || ufDestino == "SC" || ufDestino == "SP")
                {
                    valor = valorFrete - (valorFrete * (decimal)0.12);
                }
                else
                {
                    if (ufDestino != "RS")
                    {
                        valor = valorFrete - (valorFrete * (decimal)0.07);
                    }
                }
            }


            if (ufOrigem == "SC")
            {

                if (ufDestino == "MG" || ufDestino == "PR" || ufDestino == "RJ" || ufDestino == "RS" || ufDestino == "SP")
                {
                    valor = valorFrete - (valorFrete * (decimal)0.12);
                }
                else
                {
                    if (ufDestino == "SC")
                    {
                        valor = valorFrete - (valorFrete * (decimal)0.17);
                    }
                    else
                    {
                        valor = valorFrete - (valorFrete * (decimal)0.07);
                    }
                }

            }

            if (ufOrigem == "SP")
            {
                if (ufDestino == "MG" || ufDestino == "PR" || ufDestino == "RS" || ufDestino == "RJ" || ufDestino == "SC" || ufDestino == "SP")
                {
                    valor = valorFrete - (valorFrete * (decimal)0.12);
                }
                else
                {
                    //if (ufDestino != "SP")
                    //{
                    valor = valorFrete - (valorFrete * (decimal)0.07);
                    //}
                }
            }

            return valor;
        }

        private string retornaTelefone(string telefone)
        {
            string retorno = "";
            if (!string.IsNullOrEmpty(telefone))
            {
                telefone = long.Parse(telefone).ToString();
                if (telefone.Length <= 11 && telefone.Length >= 10)
                {
                    if (telefone.Length == 10)
                    {
                        int i = 0;
                        foreach (char c in telefone)
                        {
                            if (i == 0)
                                retorno += "(";
                            if (i == 2)
                                retorno += ") ";
                            if (i == 6)
                                retorno += "-";
                            retorno += c;
                            i++;
                        }
                    }
                    else
                    {
                        int i = 0;
                        foreach (char c in telefone)
                        {
                            if (i == 0)
                                retorno += "(";
                            if (i == 2)
                                retorno += ") ";
                            if (i == 7)
                                retorno += "-";
                            retorno += c;
                            i++;
                        }
                    }
                }

            }
            return retorno;
        }

        #endregion
    }
}
