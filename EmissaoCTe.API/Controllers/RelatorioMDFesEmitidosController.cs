using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;


namespace EmissaoCTe.API.Controllers
{
    public class RelatorioMDFesEmitidosController : ApiController
    {
        private List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosDocumentoMDFe> ListaCTes;
        private List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosDocumentoMDFe> ListaNFes;

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repCTeMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
                Repositorio.NotaFiscalEletronicaMDFe repNFeMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unitOfWork);
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);

                if (!Enum.TryParse<Dominio.Enumeradores.StatusMDFe>(Request.Params["Status"], out Dominio.Enumeradores.StatusMDFe status))
                    status = Dominio.Enumeradores.StatusMDFe.Todos;

                int.TryParse(Request.Params["NumeroInicial"], out int numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);

                string numeroUnidade = Request.Params["NumeroUnidade"];
                string numeroCarga = Request.Params["NumeroCarga"];
                string placaVeiculo = Request.Params["PlacaVeiculo"];
                string cpfMotorista = Request.Params["CPFMotorista"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string ufCarregamento = Request.Params["UFCarregamento"];
                string ufDescarregamento = Request.Params["UFDescarregamento"];
                string tipoRelatorio = Request.Params["TipoRelatorio"];
                string tipoArquivo = Request.Params["TipoArquivo"];
                string nomeUsuario = Request.Params["NomeUsuario"];

                bool.TryParse(Request.Params["Averbacao99999"], out bool averbacao99999);

                List<ReportParameter> parametros = new List<ReportParameter> {
                    new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial),
                    new ReportParameter("Periodo", string.Concat("De ", dataEmissaoInicial.ToString("dd/MM/yyyy"), " até ", dataEmissaoFinal.ToString("dd/MM/yyyy")))
                };

                if (tipoRelatorio == "RelatorioMDFesEmitidosAverbacaoSumarizado")
                {
                    List<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFesEmitidosAverbacoes> listaMDFes = repMDFe.RelatorioMDFesEmitidosAverbacao(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placaVeiculo, cpfMotorista, nomeMotorista, ufCarregamento, ufDescarregamento, status, 0, "", false, "", nomeUsuario, tipoRelatorio == "RelatorioMDFesEmitidosSeguradora", numeroCarga, numeroUnidade, averbacao99999);

                    List<ReportDataSource> dataSources = new List<ReportDataSource>();
                    dataSources.Add(new ReportDataSource("RelatorioMDFesEmitidosAverbacaoSumarizado", listaMDFes));

                    Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioMDFesEmitidosAverbacaoSumarizado.rdlc", tipoArquivo, parametros, dataSources);

                    return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioMDFesEmitidosAverbacaoSumarizado." + arquivo.FileNameExtension.ToLower());
                }
                else
                {
                    List<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFesEmitidos> listaMDFes = repMDFe.RelatorioMDFesEmitidos(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placaVeiculo, cpfMotorista, nomeMotorista, ufCarregamento, ufDescarregamento, status, 0, "", false, "", nomeUsuario, tipoRelatorio == "RelatorioMDFesEmitidosSeguradora", numeroCarga, numeroUnidade);

                    string[] relatoriosComCTesOuNFes = new string[] {
                    "RelatorioMDFesEmitidosCompleto",
                    "RelatorioMDFesEmitidosCompletoMotorista",
                    "RelatorioMDFesEmitidosCompletoCTes"
                    };

                    if (relatoriosComCTesOuNFes.Contains(tipoRelatorio))
                    {
                        int[] codigosMDFes = (from obj in listaMDFes select obj.CodigoMDFe).Distinct().ToArray();

                        this.ListaCTes = repCTeMDFe.BuscarDocumentosParaRelatorio(codigosMDFes);
                        this.ListaNFes = repNFeMDFe.BuscarDocumentosParaRelatorio(codigosMDFes);
                    }


                    if (tipoRelatorio == "RelatorioMDFesEmitidosCompletoCTes")
                    {
                        List<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFesEmitidos> listaMDFesComCTes = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFesEmitidos>();

                        foreach (var cte in this.ListaCTes)
                        {
                            var mdf = (from o in listaMDFes where o.CodigoMDFe == cte.CodigoMDFe select o).FirstOrDefault();
                            mdf.NumeroCTe = cte.Numero;
                            listaMDFesComCTes.Add(mdf);
                        }

                        listaMDFes = listaMDFesComCTes;
                    }

                    List<ReportDataSource> dataSources = new List<ReportDataSource>
                    {
                    new ReportDataSource("MDFesEmitidos", listaMDFes),
                    new ReportDataSource("ListaCTes", ListaCTes),
                    new ReportDataSource("ListaNFes", ListaNFes)
                    };

                    Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/" + tipoRelatorio + ".rdlc", tipoArquivo, parametros, dataSources, (object sender, SubreportProcessingEventArgs e) =>
                    {
                        if (e.ReportPath.Contains("RelatorioMDFesEmitidosCompleto_CTes"))
                            e.DataSources.Add(new ReportDataSource("CTes", (from obj in this.ListaCTes where obj.CodigoMDFe == int.Parse(e.Parameters["CodigoMDFe"].Values[0]) select obj)));
                        else if (e.ReportPath.Contains("RelatorioMDFesEmitidosCompleto_NFes"))
                            e.DataSources.Add(new ReportDataSource("NFes", (from obj in this.ListaNFes where obj.CodigoMDFe == int.Parse(e.Parameters["CodigoMDFe"].Values[0]) select obj)));
                    });

                    return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioMDFesEmitidos." + arquivo.FileNameExtension.ToLower());
                }


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadLoteXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int serie, numeroInicial, numeroFinal = 0;
                int.TryParse(Request.Params["Serie"], out serie);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                DateTime dataEmissaoInicial, dataEmissaoFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                Dominio.Enumeradores.StatusMDFe statusAux;
                Dominio.Enumeradores.StatusMDFe? status = null;
                if (Enum.TryParse(Request.Params["Status"], out statusAux))
                    status = statusAux;

                string ufCarregamento = Request.Params["UFCarregamento"];
                string ufDescarregamento = Request.Params["UFDescarregamento"];
                string placaVeiculo = Request.Params["Placa"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Request.Params["CPFMotorista"];
                string nomeUsuario = Request.Params["NomeUsuario"];
                string numeroUnidade = Request.Params["NumeroUnidade"];
                string numeroCarga = Request.Params["NumeroCarga"];

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                List<int> listaCodigosMdfes = repMDFe.ObterCodigosDownloadLote(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.TipoAmbiente, serie, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, string.Empty, cpfMotorista, nomeMotorista, this.Usuario.Series.Where(s => s.Tipo == Dominio.Enumeradores.TipoSerie.MDFe).Select(o => o.Codigo).ToArray(), nomeUsuario, numeroCarga, numeroUnidade);

                if (listaCodigosMdfes.Count > 500)
                    return Json<bool>(false, false, string.Concat("Quantidade de manifestos para geração de lote inválida (", listaCodigosMdfes.Count, "). É permitido o download de um lote de no máximo 500 manifestos de transporte."));

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
                return Arquivo(svcMDFe.ObterLoteDeXML(listaCodigosMdfes, this.EmpresaUsuario.Codigo, unitOfWork), "application/zip", "LoteXML.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o lote de MDF-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadLoteDAMDFE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                int.TryParse(Request.Params["Serie"], out int serie);
                int.TryParse(Request.Params["NumeroInicial"], out int numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);

                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoFinal);

                Dominio.Enumeradores.StatusMDFe? status = null;
                if (Enum.TryParse(Request.Params["Status"], out Dominio.Enumeradores.StatusMDFe statusAux))
                    status = statusAux;

                string ufCarregamento = Request.Params["UFCarregamento"];
                string ufDescarregamento = Request.Params["UFDescarregamento"];
                string placaVeiculo = Request.Params["Placa"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Request.Params["CPFMotorista"];
                string nomeUsuario = Request.Params["NomeUsuario"];
                string numeroUnidade = Request.Params["NumeroUnidade"];
                string numeroCarga = Request.Params["NumeroCarga"];

                int.TryParse(ConfigurationManager.AppSettings["DiasDownloadDacte"], out int diasDownloadDacte);
                if (diasDownloadDacte == 0) diasDownloadDacte = 120;

                if (dataEmissaoInicial.Date < DateTime.Now.AddDays(-diasDownloadDacte).Date)
                    return Json<bool>(false, false, "A data de emissão inicial não deve ser menor do que " + DateTime.Now.AddDays(-120).ToString("dd/MM/yyyy") + " para realizar o download do lote de DACTEs.");

                List<string> listaChavesMdfes = repMDFe.BuscarListaChaveMDFes(this.EmpresaUsuario.Codigo, 0, this.EmpresaUsuario.TipoAmbiente, serie, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, string.Empty, cpfMotorista, nomeMotorista, this.Usuario.Series.Where(s => s.Tipo == Dominio.Enumeradores.TipoSerie.MDFe).Select(o => o.Codigo).ToArray(), nomeUsuario, numeroCarga, numeroUnidade);
                if (listaChavesMdfes.Count <= 0)
                    return Json<bool>(false, false, "Nenhuma DAMDFE encontrada para o período selecionado.");

                int.TryParse(ConfigurationManager.AppSettings["QuantidadeDownloadLoteCTe"], out int quantidadeDownloadLoteCTe);

                if (quantidadeDownloadLoteCTe > 0 && listaChavesMdfes.Count > quantidadeDownloadLoteCTe)
                    return Json<bool>(false, false, "Somente é possível baixar " + quantidadeDownloadLoteCTe.ToString() + " DAMDFEs por vez, verifique filtro selecionado.");

                Servicos.MDFe svcCTe = new Servicos.MDFe(unitOfWork);
                return Arquivo(svcCTe.ObterLoteDeDAMDFE(listaChavesMdfes, this.EmpresaUsuario.Codigo, unitOfWork), "application/zip", "LoteDAMDFE.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o lote de CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int serie, numeroInicial, numeroFinal, inicioRegistros = 0;
                int.TryParse(Request.Params["Serie"], out serie);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int numeroCTe = 0;

                DateTime dataEmissaoInicial, dataEmissaoFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                Dominio.Enumeradores.StatusMDFe statusAux;
                Dominio.Enumeradores.StatusMDFe? status = null;
                if (Enum.TryParse(Request.Params["Status"], out statusAux))
                    status = statusAux;

                string ufCarregamento = Request.Params["UFCarregamento"];
                string ufDescarregamento = Request.Params["UFDescarregamento"];
                string placaVeiculo = Request.Params["Placa"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Request.Params["CPFMotorista"];
                string nomeUsuario = Request.Params["NomeUsuario"];
                string numeroCarga = Request.Params["NumeroCarga"];
                string numeroUnidade = Request.Params["NumeroUnidade"];
                
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unitOfWork);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.Consultar(this.EmpresaUsuario.Codigo, 0, this.EmpresaUsuario.TipoAmbiente, serie, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, string.Empty, cpfMotorista, nomeMotorista, this.Usuario.Series.Where(s => s.Tipo == Dominio.Enumeradores.TipoSerie.MDFe).Select(o => o.Codigo).ToArray(), numeroCTe, inicioRegistros, 50, nomeUsuario, numeroCarga, numeroUnidade);
                int countCTes = repMDFe.ContarConsulta(this.EmpresaUsuario.Codigo, 0, this.EmpresaUsuario.TipoAmbiente, serie, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, string.Empty, cpfMotorista, nomeMotorista, this.Usuario.Series.Where(s => s.Tipo == Dominio.Enumeradores.TipoSerie.MDFe).Select(o => o.Codigo).ToArray(), numeroCTe, nomeUsuario);

                List<Dominio.Entidades.VeiculoMDFe> veiculos = repVeiculoMDFe.BuscarPorMDFes((from obj in mdfes select obj.Codigo).ToArray());

                var retorno = from obj in mdfes
                              select new
                              {
                                  obj.Codigo,
                                  obj.Status,
                                  obj.Numero,
                                  Serie = obj.Serie.Numero,
                                  DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                  Placa = (from veic in veiculos where veic.MDFe.Codigo == obj.Codigo select veic.Placa).FirstOrDefault(),
                                  EstadoCarregamento = string.Concat(obj.EstadoCarregamento.Sigla, " - ", obj.EstadoCarregamento.Nome),
                                  EstadoDescarregamento = string.Concat(obj.EstadoDescarregamento.Sigla, " - ", obj.EstadoDescarregamento.Nome),
                                  obj.DescricaoStatus,
                                  MensagemSefaz = (obj.MensagemStatus == null ? (obj.MensagemRetornoSefaz != null ? System.Web.HttpUtility.HtmlEncode(obj.MensagemRetornoSefaz) : string.Empty) : obj.MensagemStatus.MensagemDoErro)
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Status", "Número|8", "Série|6", "Emissão|10", "Placa|8", "UF Carga|15", "UF Descarga|15", "Status|10", "Retorno Sefaz|18" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os MDF-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
