using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/ConhecimentoEletronico", "CTe/ConsultaCTe")]
    public class ConhecimentoEletronicoController : BaseController
    {
		#region Construtores

		public ConhecimentoEletronicoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico rptCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int numeroInicial, numeroFinal = 0;
                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);

                string cnpjcpfRemetente = Request.Params("Remetente");
                string cnpjcpfDestinatario = Request.Params("Destinatario");
                if (cnpjcpfRemetente == "0")
                    cnpjcpfRemetente = "";
                if (cnpjcpfDestinatario == "0")
                    cnpjcpfDestinatario = "";

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                string status = Request.Params("Status");
                string chaveCTe = Request.Params("ChaveCTe");
                Dominio.Enumeradores.TipoCTE finalidade;
                Enum.TryParse(Request.Params("Finalidade"), out finalidade);

                int empresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Nº CT-e", "Numero", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Modal", "DescricaoModal", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoCTE", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "LocalidadeInicioPrestacao", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "LocalidadeTerminoPrestacao", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno SEFAZ", "MensagemRetornoSefaz", 20, Models.Grid.Align.left, false);

                string ordenacao = "";
                ordenacao = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = rptCTe.Consultar(empresa, dataInicial, dataFinal, numeroInicial, numeroFinal, finalidade, status, cnpjcpfRemetente, cnpjcpfDestinatario, this.Usuario.Empresa.TipoAmbiente, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(rptCTe.ContarConsulta(empresa, dataInicial, dataFinal, numeroInicial, numeroFinal, finalidade, status, cnpjcpfRemetente, cnpjcpfDestinatario, this.Usuario.Empresa.TipoAmbiente));
                var lista = (from p in listaCTe
                             select new
                             {
                                 p.Codigo,
                                 CodigoCTE = p.Codigo,
                                 SituacaoCTe = p.Status,
                                 Status = p.DescricaoStatus,
                                 CodigoEmpresa = p.Empresa.Codigo,
                                 p.Numero,
                                 Serie = p.Serie.Numero,
                                 p.DataEmissao,
                                 p.DescricaoTipoCTE,
                                 Remetente = p.Remetente?.Nome,
                                 LocalidadeInicioPrestacao = p.LocalidadeInicioPrestacao.Descricao + " / " + p.LocalidadeInicioPrestacao.Estado.Sigla,
                                 Destinatario = p.Destinatario?.Nome,
                                 LocalidadeTerminoPrestacao = p.LocalidadeTerminoPrestacao.Descricao + " / " + p.LocalidadeTerminoPrestacao.Estado.Sigla,
                                 p.ValorFrete,
                                 p.DescricaoStatus,
                                 p.MensagemRetornoSefaz,
                                 DescricaoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalHelper.ObterDescricao(p.TipoModal)
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EmitirCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);;
                Servicos.Embarcador.Carga.ComponetesFrete serComponentesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);

                dynamic dynCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTe"));

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);


                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe>() {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.total
                };

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterDynamicParaCTe(dynCTe, unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;

                if (cteIntegracao.Codigo == 0)
                    cte = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();
                else
                    cte = repCTe.BuscarPorCodigo(cteIntegracao.Codigo);

                serCTe.SalvarDadosCTe(ref cte, cteIntegracao, cte.SituacaoCTeSefaz, permissoes, this.Usuario, unitOfWork, false, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Solicitou emissão do CT-e", unitOfWork);
                unitOfWork.CommitChanges();

                string retorno = EmitirCTe(cte.Codigo, unitOfWork);

                return new JsonpResult(true);

            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);;
                Servicos.Embarcador.Carga.ComponetesFrete serComponentesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);

                dynamic dynCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTe"));

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);


                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe>() {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.total
                    };

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterDynamicParaCTe(dynCTe, unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;

                if (cteIntegracao.Codigo == 0)
                    cte = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();
                else
                    cte = repCTe.BuscarPorCodigo(cteIntegracao.Codigo);

                serCTe.SalvarDadosCTe(ref cte, cteIntegracao, cte.SituacaoCTeSefaz, permissoes, this.Usuario, unitOfWork, false, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Salvou CT-e", unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar salvar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EmitirNovamente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (codigoCTe > 0)
                {
                    string retorno = EmitirCTe(codigoCTe, unitOfWork);

                    if (!string.IsNullOrWhiteSpace(retorno))
                        return new JsonpResult(false, true, retorno);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Solicitou emissão do CT-e", unitOfWork);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, "O Documento informado não foi localizado");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o Documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe, codigoEmpresa = 0;
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);
                int.TryParse(Request.Params("CodigoEmpresa"), out codigoEmpresa);

                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoEmpresa, codigoCTe);

                    if (cte != null && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                        byte[] data = svcCTe.ObterXMLCancelamento(cte, unitOfWork);

                        if (data != null)
                        {
                            return Arquivo(data, "text/xml", string.Concat(cte.Chave, ".xml"));
                        }
                    }
                    else
                    {
                        return new JsonpResult(false, false, "XML é possível baixar o xml de um arquivo que não é um CT-e.");
                    }
                }

                return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmailParaTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                if (cte != null)
                {
                    if (!(cte.Status.Equals("E") || cte.Status.Equals("R")))
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                        if (!svcCTe.EnviarEmail(cte.Codigo, this.Usuario.Empresa.Codigo, unitOfWork))
                            return new JsonpResult(false, false, "Não foi possível enviar o e-mail. Atualize a página e tente novamente!");

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Enviou e-mail para todos", unitOfWork);
                        return new JsonpResult(true, "Sucesso.");
                    }
                    else
                    {
                        return new JsonpResult(false, false, "O status do CT-e é inválido para o envio do e-mail.");
                    }
                }
                else
                {
                    return new JsonpResult(false, false, "CT-e não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao solicitar o envio do e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterInformacoesXMLNFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();
                if (files.Count <= 0)
                    return new JsonpResult(false, true, "Quantidade de arquivos inválida para a importação.");

                List<object> retorno = new List<object>();

                for (var i = 0; i < files.Count; i++)
                {
                    try
                    {
                        Servicos.DTO.CustomFile file = files[i];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();

                        if (extensao == ".xml")
                        {
                            Servicos.NFe svcNFe = new Servicos.NFe(unidadeTrabalho);
                            object documento = svcNFe.ObterDocumentoPorXML(file.InputStream, this.Empresa.Codigo, null, unidadeTrabalho);

                            if (documento == null)
                                retorno.Add(new { Indice = i, Sucesso = false, Mensagem = "Verifique se o XML é de uma NF-e autorizada." });
                            else
                                retorno.Add(new { Indice = i, Sucesso = true, Documento = documento });
                        }
                        else
                        {
                            retorno.Add(new { Indice = i, Sucesso = false, Mensagem = "A extensão " + extensao + " é inválida. Somente a extensão XML é aceita." });
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        retorno.Add(new { Indice = i, Sucesso = false, Mensagem = "Não foi possível ler o XML. Verifique se o XML da NF-e é válido e está formatado corretamente." });
                    }
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as informações do XML da(s) NF-e(s).");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> VisualizarPreDACTE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R088_PreDACTE, TipoServicoMultisoftware);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R088_PreDACTE, TipoServicoMultisoftware, "Pré-DACTE", "CTe", "PreDACTE.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                int codigo = int.Parse(Request.Params("CodigoCTe"));

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTE> dadosCTe = repCTe.RelatorioPreDACTE(codigo);
                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTECarga> dadosCarga = repCTe.RelatorioPreDACTECarga(codigo);
                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTEComponente> dadosComponente = repCTe.RelatorioPreDACTEComponente(codigo);
                if (dadosCTe.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioPreDACTE(codigo, stringConexao, relatorioControleGeracao, dadosCTe, dadosCarga, dadosComponente));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de CTe para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        #region Métodos Privados

        private string EmitirCTe(int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte != null)
            {
                if (cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao)
                {

                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                    cte.Status = "P";
                    repCTe.Atualizar(cte);

                    Servicos.Embarcador.Carga.CTe serCargaCTE = new Servicos.Embarcador.Carga.CTe(unitOfWork);
                    bool sucesso = svcCTe.Emitir(cte.Codigo, cte.Empresa.Codigo);
                    if (!sucesso)
                    {
                        mensagem = "O CT-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                    }
                }
                else
                {
                    mensagem = "A atual situação do CT-e (" + cte.DescricaoStatus + ") não permite sua emissão.";
                }
            }
            else
            {
                mensagem = "O CT-e informado não foi localizado";
            }

            return mensagem;
        }

        private void GerarRelatorioPreDACTE(int codigoCTe, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTE> dadosCTe, IList<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTECarga> dadosCarga, IList<Dominio.Relatorios.Embarcador.DataSource.CTe.RelatorioPreDACTEComponente> dadosComponente)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                var result = ReportRequest.WithType(ReportType.PreDacte)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("CodigoCte", codigoCTe)
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("DadosCTe", dadosCTe.ToJson())
                    .AddExtraData("DadosCarga", dadosCarga.ToJson())
                    .AddExtraData("DadosComponente", dadosComponente.ToJson())
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
