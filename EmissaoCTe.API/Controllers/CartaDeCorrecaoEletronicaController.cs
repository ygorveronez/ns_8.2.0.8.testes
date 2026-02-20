using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class CartaDeCorrecaoEletronicaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("cartadecorrecaoeletronica.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros, codigoCTe = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);

                List<Dominio.Entidades.CartaDeCorrecaoEletronica> listaCCes = repCCe.Consultar(codigoCTe, inicioRegistros, 50);
                int countCCes = repCCe.ContarConsulta(codigoCTe);

                var result = (from obj in listaCCes
                              select new
                              {
                                  obj.Codigo,
                                  obj.Status,
                                  obj.NumeroSequencialEvento,
                                  DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                  obj.DescricaoStatus,
                                  MensagemRetornoSefaz = obj.MensagemStatus != null ? string.Concat(obj.MensagemStatus.CodigoDoErro, " - ", obj.MensagemStatus.MensagemDoErro) : obj.MensagemRetornoSefaz
                              }).ToList();

                return Json(result, true, null, new string[] { "Codigo", "Status", "Número Evento|15", "Data Emissão|15", "Status|15", "Retorno Sefaz|45" }, countCCes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as Cartas de Correção Eletrônicas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarCampos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string descricaoCampo = Request.Params["Descricao"];

                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);

                List<Dominio.Entidades.CampoCCe> listaCamposCCe = repCampoCCe.ConsultarParaEmissao(descricaoCampo, inicioRegistros, 50);
                int countCamposCCe = repCampoCCe.ContarConsultaParaEmissao(descricaoCampo);

                var result = (from obj in listaCamposCCe select new { obj.Codigo, obj.Descricao }).ToList();

                return Json(result, true, null, new string[] { "Codigo", "Campo|90" }, countCamposCCe);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os campos da CC-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterInformacoesCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Servicos.Criptografia.Descriptografar(Request.Params["CodigoCTe"], "CT3##MULT1@#$S0FTW4R3"), out codigo);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (cte != null)
                {
                    var retorno = new
                    {
                        cte.Codigo,
                        cte.Numero,
                        Serie = cte.Serie.Numero,
                        cte.ValorFrete,
                        cte.Chave,
                        DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                        Remetente = cte.Remetente != null ? string.Concat(cte.Remetente.CPF_CNPJ_Formatado, " - ", cte.Remetente.NomeFantasia) : string.Empty,
                        Destinatario = cte.Destinatario != null ? string.Concat(cte.Destinatario.CPF_CNPJ_Formatado, " - ", cte.Destinatario.NomeFantasia) : string.Empty,
                    };

                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "CT-e não encontrado.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCCe, codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCCe"], out codigoCCe);
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                bool emitir = false;
                bool.TryParse(Request.Params["Emitir"], out emitir);

                DateTime dataEvento;
                DateTime.TryParseExact(Request.Params["DataEvento"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEvento);

                Dominio.Entidades.CartaDeCorrecaoEletronica cce = null;

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                if (codigoCCe > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração da Carta de Correção Eletrônica negada!");

                    cce = repCCe.BuscarPorCodigo(codigoCCe, codigoCTe);

                    if (cce.Status != Dominio.Enumeradores.StatusCCe.Rejeicao && cce.Status != Dominio.Enumeradores.StatusCCe.EmDigitacao)
                        return Json<bool>(false, false, "O status da Carta de Correção Eletrônica não permite a alteração da mesma.");

                    if (this.UsuarioAdministrativo != null)
                        cce.Log = string.Concat(cce.Log, " / Alterado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    else
                        cce.Log = string.Concat(cce.Log, " / Alterado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                    if (cce.Status == Dominio.Enumeradores.StatusCCe.Rejeicao && cce.MensagemRetornoSefaz.ToLower().Contains("duplicidade de evento"))
                        cce.NumeroSequencialEvento += 1;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de Carta de Correção Eletrônica negada!");

                    cce = new Dominio.Entidades.CartaDeCorrecaoEletronica();
                    cce.NumeroSequencialEvento = repCCe.BuscarUltimoNumeroSequencial(codigoCTe) + 1;

                    if (this.UsuarioAdministrativo != null)
                        cce.Log = string.Concat("Criado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    else
                        cce.Log = string.Concat("Criado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                }

                cce.CTe = repCTe.BuscarPorId(codigoCTe, this.EmpresaUsuario.Codigo);
                cce.DataEmissao = dataEvento;
                cce.Status = Dominio.Enumeradores.StatusCCe.EmDigitacao;
                cce.MensagemRetornoSefaz = string.Empty;

                JsonResult retorno = this.ValidarDadosCCe(cce, unidadeDeTrabalho);

                if (retorno == null)
                {
                    if (cce.Codigo > 0)
                        repCCe.Atualizar(cce);
                    else
                        repCCe.Inserir(cce);
                }
                else
                {
                    unidadeDeTrabalho.Rollback();
                    return retorno;
                }

                this.SalvarItensCCe(cce, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                if (emitir)
                {
                    Servicos.CCe svcCCe = new Servicos.CCe(unidadeDeTrabalho);

                    cce.Status = Dominio.Enumeradores.StatusCCe.Pendente;

                    if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTe.EmitirCCe(cce, this.EmpresaUsuario.Codigo, unidadeDeTrabalho))
                    {
                        if (cce.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(1, cce.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CCe, Conexao.StringConexao);
                    }
                    else
                        return Json((cce == null ? null : new { Codigo = cce.Codigo }), false, "A Carta de Correção Eletrônica foi salva, porém, ocorreram problemas ao enviar para o sefaz.");
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a Carta de Correção Eletrônica.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCCe, codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCCe"], out codigoCCe);
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);
                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigoCCe, codigoCTe);

                if (cce == null)
                    return Json<bool>(false, false, "Carta de Correção Eletrônica não encontrada.");

                Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unitOfWork);
                List<Dominio.Entidades.ItemCCe> itensCCe = repItemCCe.BuscarPorCCe(cce.Codigo);

                var retorno = new
                {
                    cce.Codigo,
                    DataEmissao = cce.DataEmissao.Value.ToString("dd/MM/yyyy"),
                    cce.Log,
                    cce.NumeroSequencialEvento,
                    cce.Protocolo,
                    cce.Status,
                    Itens = (from obj in itensCCe
                             select new Dominio.ObjetosDeValor.ItemCCe()
                             {
                                 Codigo = obj.Codigo,
                                 Excluir = false,
                                 Sequencial = obj.NumeroItemAlterado,
                                 Valor = obj.ValorAlterado,
                                 Campo = new Dominio.ObjetosDeValor.CampoCCe()
                                 {
                                     Codigo = obj.CampoAlterado.Codigo,
                                     Descricao = obj.CampoAlterado.Descricao,
                                     GrupoCampo = obj.CampoAlterado.GrupoCampo,
                                     IndicadorRepeticao = obj.CampoAlterado.IndicadorRepeticao,
                                     NomeCampo = obj.CampoAlterado.NomeCampo,
                                     QuantidadeCaracteres = obj.CampoAlterado.QuantidadeCaracteres,
                                     QuantidadeDecimais = obj.CampoAlterado.QuantidadeDecimais,
                                     QuantidadeInteiros = obj.CampoAlterado.QuantidadeInteiros,
                                     Status = obj.CampoAlterado.Status,
                                     TipoCampo = obj.CampoAlterado.TipoCampo
                                 }
                             }).ToList()
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da Carta de Correção Eletrônica.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe, codigoCCe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["CodigoCCe"], out codigoCCe);

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);
                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigoCCe, codigoCTe);

                if (cce != null)
                {
                    byte[] data = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTe.ObterXMLCCe(cce, unitOfWork);
                    if (data != null)
                    {
                        return Arquivo(data, "text/xml", string.Concat("CCe_", cce.NumeroSequencialEvento, "-CTe_", cce.NumeroCTe, ".xml"));
                    }
                }

                return Json<bool>(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadPDF()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCCe = 0;
                int.TryParse(Request.Params["CodigoCCe"], out codigoCCe);

                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);
                Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unidadeDeTrabalho);
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeDeTrabalho);

                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigoCCe);
                List<Dominio.Entidades.ItemCCe> itensCCe = repItemCCe.BuscarPorCCe(codigoCCe);

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("CCe", new Dominio.Entidades.CartaDeCorrecaoEletronica[] { cce }));
                dataSources.Add(new ReportDataSource("ItensCCe", itensCCe));


                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCCe.rdlc", "PDF", null, dataSources);
                string nomeArquivo = string.Concat("CCe_", cce.NumeroSequencialEvento, "-CTe_", cce.NumeroCTe, ".", arquivo.FileNameExtension);
                return Arquivo(arquivo.Arquivo, arquivo.MimeType, nomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do PDF.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeDeTrabalho);

                int serieCTe, numeroInicialCTe, numeroFinalCTe;
                int.TryParse(Request.Params["SerieCTe"], out serieCTe);
                int.TryParse(Request.Params["NumeroInicialCTe"], out numeroInicialCTe);
                int.TryParse(Request.Params["NumeroFinalCTe"], out numeroFinalCTe);

                string tipoArquivo = Request.Params["TipoArquivo"];

                Dominio.Enumeradores.StatusCCe statusAux;
                Dominio.Enumeradores.StatusCCe? status = null;
                if (Enum.TryParse(Request.Params["StatusCCe"], out statusAux))
                    status = statusAux;

                DateTime dataInicialCCe, dataFinalCCe, dataInicialCTe, dataFinalCTe;
                DateTime.TryParseExact(Request.Params["DataInicialCCe"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialCCe);
                DateTime.TryParseExact(Request.Params["DataFinalCCe"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalCCe);
                DateTime.TryParseExact(Request.Params["DataInicialCTe"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialCTe);
                DateTime.TryParseExact(Request.Params["DataFinalCTe"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalCTe);

                List<Dominio.ObjetosDeValor.Relatorios.RelatorioCCs> cces = repCCe.ConsultaRelatorioCCe(this.EmpresaUsuario.Codigo, status, serieCTe, numeroInicialCTe, numeroFinalCTe, dataInicialCCe, dataFinalCCe, dataInicialCTe, dataFinalCTe); //repCCe.InstanciaRelatorio();

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("PeriodoCCe", FormataPeriodoData(dataInicialCCe, dataFinalCCe, "Todos")));
                parametros.Add(new ReportParameter("PeriodoCTe", FormataPeriodoData(dataInicialCTe, dataFinalCTe, "Todos")));
                parametros.Add(new ReportParameter("NumerosCTe", FormataPeriodoNumero(numeroInicialCTe, numeroFinalCTe, "Todos")));
                parametros.Add(new ReportParameter("SerieCTe", FormataSerie(serieCTe, "Todos")));
                parametros.Add(new ReportParameter("StatusCCe", FormataStatus(status, "Todos")));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("CCes", cces));

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCCesEmitidas.rdlc", tipoArquivo, parametros, dataSources);
                string nomeArquivo = string.Concat("RelatorioCCes-", DateTime.Now.ToString("HH:mm"), ".", arquivo.FileNameExtension);
                return Arquivo(arquivo.Arquivo, arquivo.MimeType, nomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do PDF.");
            }
        }
        #endregion

        #region Métodos Privados

        private void SalvarItensCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<Dominio.ObjetosDeValor.ItemCCe> itensCCe = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ItemCCe>>(Request.Params["ItensCCe"]);

            if (itensCCe.Count() > 0)
            {
                Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unidadeDeTrabalho);
                Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.ItemCCe item in itensCCe)
                {
                    Dominio.Entidades.ItemCCe itemCCe = item.Codigo > 0 ? repItemCCe.BuscarPorCodigo(item.Codigo, cce.Codigo) : new Dominio.Entidades.ItemCCe();

                    if (!item.Excluir)
                    {
                        itemCCe.CCe = cce;
                        itemCCe.CampoAlterado = repCampoCCe.BuscarPorCodigo(item.Campo.Codigo);
                        itemCCe.NumeroItemAlterado = item.Sequencial;
                        itemCCe.ValorAlterado = item.Valor;

                        if (item.Codigo > 0)
                            repItemCCe.Atualizar(itemCCe);
                        else
                            repItemCCe.Inserir(itemCCe);
                    }
                    else if (item.Codigo > 0)
                    {
                        repItemCCe.Deletar(itemCCe);
                    }
                }
            }
        }

        private JsonResult ValidarDadosCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork)
        {
            if (cce.CTe == null)
                return Json<bool>(false, false, "CT-e não encontrado.");

            if (cce.CTe.Status != "A")
                return Json<bool>(false, false, "O CT-e deve estar autorizado para a emissão de uma Carta de Correção Eletrônica.");

            //if ((DateTime.Now - cce.CTe.DataRetornoSefaz.Value).TotalDays > 30)
            //    return Json<bool>(false, false, "O CT-e foi autorizado a mais de 30 dias, não podendo ser emitida uma Carta de Correção Eletrônica para o mesmo.");

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            if (repCTe.ContarPorChaveDoCTeOriginalETipo(this.EmpresaUsuario.Codigo, cce.CTe.Chave, Dominio.Enumeradores.TipoCTE.Anulacao) > 0)
                return Json<bool>(false, false, "O CT-e já possui um CT-e de Anulação vinculado a ele, não podendo ser emitida uma Carta de Correção Eletrônica para o mesmo.");

            if (repCTe.ContarPorChaveDoCTeOriginalETipo(this.EmpresaUsuario.Codigo, cce.CTe.Chave, Dominio.Enumeradores.TipoCTE.Substituto) > 0)
                return Json<bool>(false, false, "O CT-e já possui um CT-e de Anulação vinculado a ele, não podendo ser emitida uma Carta de Correção Eletrônica para o mesmo.");

            return null;
        }

        private string FormataPeriodoData(DateTime inicio, DateTime fim, string dft)
        {
            string _formato = "dd/MM/yyyy";
            string periodo = string.Empty;

            if (inicio != DateTime.MinValue)
                periodo += " De " + inicio.ToString(_formato);

            if (fim != DateTime.MinValue)
                periodo += " Até " + fim.ToString(_formato);

            periodo = periodo.Trim();
            return string.IsNullOrWhiteSpace(periodo) ? dft : periodo;
        }
        private string FormataPeriodoNumero(int inicio, int fim, string dft)
        {
            string periodo = string.Empty;

            if (inicio > 0)
                periodo += " De " + inicio.ToString();

            if (fim > 0)
                periodo += " Até " + fim.ToString();

            periodo = periodo.Trim();
            return string.IsNullOrWhiteSpace(periodo) ? dft : periodo;
        }

        private string FormataSerie(int serie, string dft)
        {
            string periodo = string.Empty;

            if (serie > 0)
                periodo += serie.ToString();

            periodo = periodo.Trim();
            return string.IsNullOrWhiteSpace(periodo) ? dft : periodo;
        }

        private string FormataStatus(Dominio.Enumeradores.StatusCCe? status, string dft)
        {
            string periodo = string.Empty;

            if (status != null)
            {
                Dominio.Entidades.CartaDeCorrecaoEletronica temp = new Dominio.Entidades.CartaDeCorrecaoEletronica()
                {
                    Status = status.Value
                };
                periodo += temp.DescricaoStatus;
            }

            periodo = periodo.Trim();
            return string.IsNullOrWhiteSpace(periodo) ? dft : periodo;
        }
        #endregion

    }
}
