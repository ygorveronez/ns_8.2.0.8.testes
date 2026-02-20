using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System.Net;
using System.Text;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Documentos
{
    [CustomAuthorize(new string[] { "BuscarMDFeCarga", "DownloadDAMDFE", "BuscarDadosParaEncerramentoPorCodigo", "ConsultarCargaMDFe", "DownloadXMLAutorizacao" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaMDFeController : BaseController
    {
        #region Construtores

        public CargaMDFeController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ConsultarCargaMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

                int codigoCarga = int.Parse(Request.Params("Carga"));

                Dominio.Enumeradores.StatusMDFe statusMDFe = (Dominio.Enumeradores.StatusMDFe)int.Parse(Request.Params("Status"));


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoMDFE", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data da Emissão", "Emissao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("UF Carregamento", "UFCarga", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("UF Descarregamento", "UFDesgarga", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("MDF-e Manual", "MDFeManual", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("DataPrevisaoEncerramento", false);
                grid.AdicionarCabecalho("Importado", false);
                grid.AdicionarCabecalho("CodigoDoErro", false);
                grid.AdicionarCabecalho("HabilitarSincronizarDocumento", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                propOrdenacao = "MDFe." + propOrdenacao;

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repCargaMDFe.ConsultarMDFe(codigoCarga, statusMDFe, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(codigoCarga);

                grid.setarQuantidadeTotal(repCargaMDFe.ContarConsultaMDFe(codigoCarga, statusMDFe));
                var lista = (from obj in cargaMDFes
                             select new
                             {
                                 obj.Codigo,
                                 CodigoMDFE = obj.MDFe.Codigo,
                                 CodigoEmpresa = obj.MDFe.Empresa.Codigo,
                                 obj.MDFe.Numero,
                                 Serie = obj.MDFe.Serie.Numero,
                                 Emissao = obj.MDFe.DataEmissao.HasValue ? obj.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                 UFCarga = obj.MDFe.EstadoCarregamento.Nome,
                                 UFDesgarga = obj.MDFe.EstadoDescarregamento.Nome,
                                 DescricaoStatus = obj.MDFe.DescricaoStatus,
                                 Status = obj.MDFe.Status,
                                 MDFeManual = repCargaMDFeManual.ExistePorMDFe(obj.MDFe.Codigo) ? "Sim" : "Não",
                                 obj.MDFe.Importado,
                                 RetornoSefaz = (obj.MDFe.MensagemStatus == null ? (obj.MDFe.MensagemRetornoSefaz != null ? WebUtility.HtmlEncode(obj.MDFe.MensagemRetornoSefaz) : string.Empty) : obj.MDFe.MensagemStatus.MensagemDoErro),
                                 DT_RowColor = obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? "#dff0d8" : obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao ? "rgba(193, 101, 101, 1)" : obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ? "#777" : "",
                                 DT_FontColor = (obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao || obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado) ? "#FFFFFF" : "",
                                 DataPrevisaoEncerramento = obj.MDFe.DataPrevisaoEncerramento.HasValue ? obj.MDFe.DataPrevisaoEncerramento.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                 CodigoDoErro = obj.MDFe?.MensagemStatus?.CodigoDoErro ?? 0,
                                 HabilitarSincronizarDocumento = this.ObterHabilitarSincronizarDocumento(obj.MDFe, null),
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

        public async Task<IActionResult> TentarEmitirNovamente()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                bool mdfesEmitidos = true;
                string mensagemPendenciaDocumento = "";

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = null;

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                {
                    cargaCancelamento = repCargaCancelamento.BuscarPorCarga(carga.Codigo);
                    if (cargaCancelamento.SituacaoCargaNoCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
                    {
                        mensagemPendenciaDocumento = carga.MotivoPendencia;
                        mdfesEmitidos = false;
                    }
                }

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos && carga.PossuiPendencia)
                {
                    Servicos.Embarcador.Carga.Documentos serCargaDocumentos = new Servicos.Embarcador.Carga.Documentos(unidadeDeTrabalho);
                    mensagemPendenciaDocumento = carga.MotivoPendencia;
                    int numeroMDFe = repCargaMDFe.ContarPorCarga(carga.Codigo);
                    if (numeroMDFe <= 0)// se possui MDF-e e tem pendência quer dizer que ocorreu uma rejeição por isso não é necessário verificar novamente e sim mandar novamente para emissão o MDF-e
                    {
                        carga.AutorizouTodosCTes = true;
                        carga.problemaCTE = false;
                        carga.problemaNFS = false;
                        carga.PossuiPendencia = false;
                        carga.MotivoPendencia = "";
                        carga.problemaMDFe = false;
                        repCarga.Atualizar(carga);
                        mdfesEmitidos = true;
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Solicitou Novamente o Envio de MDF-es da Carga", unidadeDeTrabalho);

                var retorno = new
                {
                    MDFeEmitidos = mdfesEmitidos,
                    Mensagem = mensagemPendenciaDocumento,
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os MDFEs.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> InformarDadosMDFeManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.MDFE.ManifestoEletronicoDeDocumentosFiscaisManual repMDFeManual = new Repositorio.Embarcador.MDFE.ManifestoEletronicoDeDocumentosFiscaisManual(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Carga.Documentos serDocumentos = new Servicos.Embarcador.Carga.Documentos(unitOfWork);
                int CargaMDFe = int.Parse(Request.Params("CargaMDFe"));
                int Codigo = int.Parse(Request.Params("Codigo"));
                string Chave = Request.Params("Chave");
                string Observacao = Request.Params("Observacao");

                Dominio.Entidades.Embarcador.MDFe.ManifestoEletronicoDeDocumentosFiscaisManual mdfeManual = repMDFeManual.BuscarPorCodigo(Codigo, true);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorCodigo(CargaMDFe);
                if (cargaMDFe.MDFeManual.Codigo == mdfeManual.Codigo)
                {
                    if (!mdfeManual.MDFeInformado)
                    {

                        mdfeManual.Chave = Chave;
                        mdfeManual.ObservacaoOperador = Observacao;
                        mdfeManual.DataInformacaoManual = DateTime.Now;
                        mdfeManual.Usuario = this.Usuario;
                        mdfeManual.MDFeInformado = true;
                        repMDFeManual.Atualizar(mdfeManual, Auditado);

                        List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFe.BuscarPorCarga(cargaMDFe.Carga.Codigo);
                        if (!cargaMDFEs.Exists(obj => obj.MDFeManual.MDFeInformado != true))
                        {
                            cargaMDFe.Carga.AgImportacaoMDFe = false;
                            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaMDFe.Carga;
                            serDocumentos.FinalizarEmissoes(ref carga, configuracaoTMS, TipoServicoMultisoftware, unitOfWork, Auditado);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFe.Carga, null, "Informou MFD-e Manual (" + mdfeManual.Descricao + ")", unitOfWork);

                        unitOfWork.CommitChanges();

                        return new JsonpResult(true);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "O MDF-e já foi informado.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "O MDF-e na base não é o mesmo que MDF-e informado.");
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados para encerramento do MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EncerrarMDFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/EncerramentoCarga");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.EncerramentoCarga_EncerrarMDFe))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");


                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                int codigo = int.Parse(Request.Params("Codigo"));
                int codCarga = int.Parse(Request.Params("Carga"));
                int codMunicipio = int.Parse(Request.Params("Localidade"));

                DateTime dataEncerramento;
                DateTime.TryParseExact(Request.Params("DataEncerramento") + " " + Request.Params("HoraEncerramento"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);

                if (mdfe.Importado && (integracaoIntercab?.AtivarIntegracaoMDFeAquaviario ?? false))
                    return new JsonpResult(false, true, "Não é possível executar esta operação para um MDF-e importado.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Solicitou o Encerramento do MDF-e.", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Solicitou o Encerramento do MDF-e." + mdfe.Descricao, unidadeTrabalho);

                Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeTrabalho);
                string retorno = serCargaMDFe.EncerrarMDFe(codigo, codCarga, codMunicipio, dataEncerramento, WebServiceConsultaCTe, this.Usuario, TipoServicoMultisoftware, unidadeTrabalho, Auditado);

                if (String.IsNullOrEmpty(retorno))
                {
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, retorno);
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados para encerramento do MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> InformarDataPrevisaoEncerramento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                int codigo = int.Parse(Request.Params("Codigo"));
                int codCarga = int.Parse(Request.Params("Carga"));

                DateTime dataPrevisaoEncerramento;
                DateTime.TryParseExact(Request.Params("DataPrevisaoEncerramento"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataPrevisaoEncerramento);

                if (dataPrevisaoEncerramento == DateTime.MinValue)
                    return new JsonpResult(false, true, "Favor informe uma data de previsão de encerramento válida.");

                unidadeTrabalho.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);

                if (dataPrevisaoEncerramento < mdfe.DataEmissao)
                    return new JsonpResult(false, true, "A data de previsão de encerramento não pode ser menor que a data de emissão do MDF-e.");

                mdfe.DataPrevisaoEncerramento = dataPrevisaoEncerramento;

                repMDFe.Atualizar(mdfe);

                unidadeTrabalho.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Informou data de previsão de encerramento do MDF-e " + dataPrevisaoEncerramento.ToString("dd/MM/yyyy HH:mm"), unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Informou data de previsão de encerramento do MDF-e " + mdfe.Descricao + " " + dataPrevisaoEncerramento.ToString("dd/MM/yyyy HH:mm"), unidadeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha informar a data de previsão de encerramento do MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosParaEncerramentoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe dadosEncerramentoMDFe = serCargaMDFe.ObterDadosEncerramento(codigo, unitOfWork);

                var encerramentoMDF = new
                {
                    dadosEncerramentoMDFe.Codigo,
                    Estado = dadosEncerramentoMDFe.Estado.Nome.Trim() + " - " + dadosEncerramentoMDFe.Estado.Sigla,
                    DataEncerramento = dadosEncerramentoMDFe.DataEncerramento.ToString("dd/MM/yyyy"),
                    HoraEncerramento = dadosEncerramentoMDFe.DataEncerramento.ToString("HH:mm"),
                    Localidades = (from obj in dadosEncerramentoMDFe.Localidades
                                   select new
                                   {
                                       Codigo = obj.Codigo,
                                       Descricao = obj.Descricao
                                   }).ToList()
                };

                return new JsonpResult(encerramentoMDF);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados para encerramento do MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDAMDFE()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params("CodigoMDFe"), out codigoMDFe);
                int codigoEmpresa = 0;
                int.TryParse(Request.Params("CodigoEmpresa"), out codigoEmpresa);
                bool contingencia = false;
                bool.TryParse(Request.Params("Contingencia"), out contingencia);

                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeDeTrabalho);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);


                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado &&
                    !contingencia)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado para o download do DAMDFE.");

                if (contingencia && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao && mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmDigitacao && mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmitidoContingencia)
                    return new JsonpResult(false, false, "O MDF-e não pode ter sido autorizado para baixar o DAMDFE em contingência.");

                if (mdfe.Importado && (integracaoIntercab?.AtivarIntegracaoMDFeAquaviario ?? false))
                    return new JsonpResult(false, false, "Não é possível executar esta operação para um MDF-e importado.");


                string caminhoPDF = string.Empty;

                if (!contingencia)
                {
                    if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                    {
                        caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";
                    }
                }
                else
                {
                    Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                    if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).SolicitarEmissaoContingencia(mdfe.Codigo, unidadeDeTrabalho))
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmitidoContingencia;
                        repMDFe.Atualizar(mdfe);

                        string obsContingencia = this.Usuario != null ? string.Concat(this.Usuario.CPF, " - ", this.Usuario.Nome) : string.Concat(this.Usuario.CPF, " - ", this.Usuario.Nome);
                        svcMDFe.SalvarRetornoSefaz(mdfe, "O", 0, 0, string.Concat("MDFe emitido em contingência pelo usuário ", obsContingencia), unidadeDeTrabalho);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "MDF-e impresso em contingência.", unidadeDeTrabalho);

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCargaMDFe.BuscarCargaPorMDFe(mdfe.Codigo);
                        if (carga != null)
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"MDF-e {mdfe.Numero} impresso em contingência.", unidadeDeTrabalho);

                        caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Numero.ToString()) + "_CONTINGENCIA_.pdf";
                    }
                    else
                    {
                        return new JsonpResult(false, false, "Não foi possível solicitar a emissão em Contingência. Contate o suporte técnico.");
                    }
                }

                byte[] arquivo = null;

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);

                if (arquivo == null)
                {
                    //Adiciona nova requisição para gerar arquivo e realizar download via ReportApi
                    var resultReportApi = ReportRequest.WithType(ReportType.CargaMDFe)
                                              .WithExecutionType(ExecutionType.Async)
                                              .AddExtraData("codigoMDFe", mdfe.Codigo)
                                              .AddExtraData("contingencia", contingencia)
                                              .CallReport();
                    if (resultReportApi == null)
                        throw new Exception();

                    arquivo = resultReportApi.GetContentFile();
                }

                if (arquivo != null)
                {

                    if (!contingencia)
                        return Arquivo(arquivo, "application/pdf", string.Concat(mdfe.Chave, ".pdf"));
                    else
                        return Arquivo(arquivo, "application/pdf", string.Concat(mdfe.Numero, "_CONTINGENCIA_.pdf"));
                }
                else
                    return new JsonpResult(false, false, "Não foi possível gerar o DAMDFE, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do DAMDFE.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteDAMDFE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int.TryParse(Request.Params("CodigoEmpresa"), out int codigoEmpresa);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<string> chavesMdfes = repCargaMDFe.BuscarChavesMDFeAutorizadosPorCarga(codigoCarga);

                if (chavesMdfes.Count == 0)
                    return new JsonpResult(false, true, "Não há MDF-es disponíveis para esta carga.");
                if (chavesMdfes.Count > 1000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 1000 arquivos.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcMDFe.ObterLoteDeDAMDFE(chavesMdfes, codigoEmpresa, unidadeTrabalho);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_DAMDFE.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote do DAMDFE.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params("CodigoMDFe"), out codigoMDFe);
                int codigoEmpresa = 0;
                int.TryParse(Request.Params("CodigoEmpresa"), out codigoEmpresa);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar autorizado para o download do XML de autorização.");

                if (mdfe.Importado && (integracaoIntercab?.AtivarIntegracaoMDFeAquaviario ?? false))
                    return new JsonpResult(false, false, "Não é possível executar esta operação para um MDF-e importado.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXMLAutorizacao(mdfe, unitOfWork);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, ".xml"));
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML de autorização.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLEncerramento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = Request.GetIntParam("CodigoMDFe");
                int codigoEmpresa = Request.GetIntParam("CodigoEmpresa");

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, codigoEmpresa);

                if (mdfe == null)
                    return new JsonpResult(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, "O MDF-e deve estar encerrado para o download do XML de encerramento.");

                if (mdfe.Importado && (integracaoIntercab?.AtivarIntegracaoMDFeAquaviario ?? false))
                    return new JsonpResult(false, false, "Não é possível executar esta operação para um MDF-e importado.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXMLEncerramento(mdfe, unitOfWork);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, ".xml"));
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML de encerramento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteXML()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int.TryParse(Request.Params("CodigoEmpresa"), out int codigoEmpresa);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> codigosMdfes = repCargaMDFe.BuscarCodigosMDFePorAutorizadosCarga(codigoCarga);

                if (codigosMdfes.Count == 0)
                    return new JsonpResult(false, true, "Não há MDF-es disponíveis para esta carga.");
                if (codigosMdfes.Count > 1000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 1000 arquivos.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcMDFe.ObterLoteDeXML(codigosMdfes, codigoEmpresa, unidadeTrabalho);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_XML_MDFE.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de XML.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EmitirNovamente(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoMDFE = Request.GetIntParam("CodigoMDFE");
                int codigoEmpresa = Request.GetIntParam("CodigoEmpresa");

                if (codigoMDFE > 0)
                {
                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                    Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork, cancellationToken);
                    Repositorio.PercursoMDFe repPercursoMDFe = new Repositorio.PercursoMDFe(unitOfWork, cancellationToken);
                    Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens repCargaLocaisPrestacaoPassagens = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacaoPassagens(unitOfWork, cancellationToken);

                    Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
                    Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);
                    Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);

                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = await repMDFe.BuscarPorCodigoAsync(codigoMDFE, codigoEmpresa);

                    if (mdfe == null)
                        throw new ControllerException("O MDF-e informado não foi localizado.");

                    if (mdfe.Status != StatusMDFe.Rejeicao && mdfe.Status != StatusMDFe.Pendente && mdfe.Status == StatusMDFe.EmitidoContingencia)
                        throw new ControllerException("A atual situação do MDF-e (" + mdfe.DescricaoStatus + ") não permite sua emissão.");

                    Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = await repCargaMDFe.BuscarPorMDFeAsync(mdfe.Codigo);

                    if (cargaMDFe != null)
                    {
                        if (cargaMDFe.Carga.SituacaoCarga == SituacaoCarga.Anulada || cargaMDFe.Carga.SituacaoCarga == SituacaoCarga.Cancelada)
                            throw new ControllerException("Não é possível emitir o MDF-e na situação atual da carga.");

                        if (repositorioCargaCte.ExisteCTeCanceladoInutilizadoOuDenegado(cargaMDFe.Carga.Codigo))
                            throw new ControllerException("Não é possível emitir o MDF-e com CT-e cancelado, inutilizado ou denegado.");

                        List<Dominio.Entidades.PercursoMDFe> percursosMDFe = await repPercursoMDFe.BuscarPorMDFeAsync(mdfe.Codigo);
                        foreach (Dominio.Entidades.PercursoMDFe percursoMDFe in percursosMDFe)
                            await repPercursoMDFe.DeletarAsync(percursoMDFe);

                        if (cargaMDFe.CargaLocaisPrestacao != null)//todo: remover essa comparação, assim como remover a da emissão do MDF-e, cargas futuras sempre terão essa informação
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();
                            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> cargaLocaisPrestacaoPassagens = await repCargaLocaisPrestacaoPassagens.BuscarPorLocalPrestacaoAsync(cargaMDFe.CargaLocaisPrestacao.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens cargaLocalPrestacaoPassagem in cargaLocaisPrestacaoPassagens)
                                passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = cargaLocalPrestacaoPassagem.EstadoDePassagem.Sigla, Posicao = cargaLocalPrestacaoPassagem.Posicao });

                            svcMDFe.GerarPercursos(mdfe, unitOfWork, null, serCargaLocaisPrestacao.RetornaPassagensProntasParaMDFe(cargaMDFe.CargaLocaisPrestacao.LocalidadeInicioPrestacao.Estado, cargaMDFe.CargaLocaisPrestacao.LocalidadeTerminoPrestacao.Estado, cargaMDFe.CargaLocaisPrestacao.LocalidadeFronteira, passagens));
                        }
                        else
                        {
                            string estadoOrigem = mdfe.EstadoCarregamento.Sigla;
                            string estadoDestino = mdfe.EstadoDescarregamento.Sigla;

                            Servicos.Embarcador.Carga.CargaFronteira serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
                            Dominio.Entidades.Embarcador.Cargas.CargaFronteira fronteiraPrincipalCarga = serCargaFronteira.ObterFronteiraPrincipal(cargaMDFe.Carga);

                            if (estadoOrigem == "EX")
                                estadoOrigem = fronteiraPrincipalCarga?.Fronteira?.Localidade?.Estado?.Sigla;

                            if (estadoDestino == "EX")
                                estadoDestino = fronteiraPrincipalCarga?.Fronteira?.Localidade?.Estado?.Sigla;

                            Dominio.Entidades.PercursoEstado percursoEstado = await repPercursoEstado.BuscarAsync(mdfe.Empresa.EmpresaPai.Codigo, estadoOrigem, estadoDestino);

                            svcMDFe.GerarPercursos(mdfe, unitOfWork, percursoEstado);
                        }

                        if (cargaMDFe.Carga.PossuiPendencia)
                        {
                            cargaMDFe.Carga.PossuiPendencia = false;
                            cargaMDFe.Carga.problemaMDFe = false;
                            cargaMDFe.Carga.MotivoPendencia = "";
                            await repCarga.AtualizarAsync(cargaMDFe.Carga);
                        }

                        new Servicos.Embarcador.Carga.Documentos(unitOfWork).AtualizarInformacoesCIOT(cargaMDFe.Carga, unitOfWork);

                        new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unitOfWork).GerarInformacoesBancariasMDFe(cargaMDFe.Carga, mdfe);
                    }
                    else if (!Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente()?.ProcessarCTeMultiCTe ?? false)
                        throw new ControllerException("O MDF-e informado não pertence a uma carga.");

                    if (mdfe.DataEmissao < DateTime.Now.AddDays(-1))
                    {
                        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                        mdfe.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
                    }

                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.Pendente;

                    serCargaMDFe.AtualizarANTT(ref mdfe, TipoServicoMultisoftware, unitOfWork);

                    if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente()?.ProcessarCTeMultiCTe ?? false)
                        new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unitOfWork).AtualizarInformacoesPagamento(ref mdfe);

                    await repMDFe.AtualizarAsync(mdfe);

                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, mdfe, null, "Solicitou Emissão do MDF-e", unitOfWork, cancellationToken: cancellationToken);

                    if (cargaMDFe != null)
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaMDFe.Carga, null, "Solicitou Emissão do MDF-e " + mdfe.Descricao, unitOfWork, cancellationToken: cancellationToken);

                    await unitOfWork.CommitChangesAsync(cancellationToken);

                    bool sucesso = svcMDFe.Emitir(mdfe, unitOfWork);

                    if (!sucesso)
                        return new JsonpResult(false, true, "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + mdfe.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.");
                }

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o MDF-e.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SincronizarDocumentoEmProcessamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFE = Request.GetIntParam("CodigoMDFE");
                int codigoEmpresa = Request.GetIntParam("codigoEmpresa");

                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

                Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

                string retorno = await serCargaMDFe.SincronizarDocumentoEmProcessamentoAsync(codigoMDFE, codigoEmpresa, Auditado, TipoServicoMultisoftware, cancellationToken);

                if (!string.IsNullOrWhiteSpace(retorno.ToString()))
                    return new JsonpResult(false, true, retorno.ToString());

                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = await repCargaMDFe.BuscarPorMDFeAsync(codigoMDFE);

                if (cargaMDFe != null)
                    await RemoverPendenciaCargaAsync(cargaMDFe.Carga, unitOfWork, cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar sincronizar o MDF-e.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SincronizarLoteDocumentoEmProcessamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("carga");

                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga, false);

                if (carga == null)
                    throw new ServicoException("Carga não localizada!");

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> MDFes = await repCargaMDFe.BuscarMDFesPorCargaAsync(codigoCarga, cancellationToken);

                StringBuilder retorno = new StringBuilder();

                foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe in MDFes)
                {
                    if (ObterHabilitarSincronizarDocumento(MDFe, null))
                    {
                        try
                        {
                            string novoretorno = await serCargaMDFe.SincronizarDocumentoEmProcessamentoAsync(MDFe.Codigo, MDFe.Empresa.Codigo, Auditado, TipoServicoMultisoftware, cancellationToken);
                            if (!string.IsNullOrEmpty(novoretorno))
                            {
                                retorno.AppendLine(novoretorno);
                            }
                        }
                        catch (ServicoException ex)
                        {
                            retorno.AppendLine($"Documento {MDFe.Codigo} - ({ex.Message})");
                        }
                        catch (Exception)
                        {
                            retorno.AppendLine($"Documento {MDFe.Codigo} - (Ocorreu uma falha ao tentar sincronizar o Documento)");
                        }

                    }
                }

                if (!string.IsNullOrWhiteSpace(retorno.ToString()))
                    return new JsonpResult(false, true, retorno.ToString());


                await RemoverPendenciaCargaAsync(carga, unitOfWork, cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar sincronizar o MDF-e.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ReemitirMDFesRejeitados()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);
                Servicos.Embarcador.Carga.MDFe svcCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeTrabalho);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in carga.CargaMDFes)
                {
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = cargaMDFe.MDFe;
                    if (mdfe.Status == StatusMDFe.Rejeicao ||
                        mdfe.Status == StatusMDFe.Pendente ||
                        mdfe.Status == StatusMDFe.EmitidoContingencia)
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.Pendente;

                        if (mdfe.DataEmissao < DateTime.Now.AddDays(-1))
                        {
                            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                            mdfe.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Solicitou Emissão do MDF-e", unidadeTrabalho);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFe.Carga, null, "Solicitou Emissão do MDF-e " + mdfe.Descricao, unidadeTrabalho);

                        new Servicos.Embarcador.Carga.Documentos(unidadeTrabalho).AtualizarInformacoesCIOT(cargaMDFe.Carga, unidadeTrabalho);

                        new Servicos.Embarcador.MDFe.MDFeInformacoesPagamento(unidadeTrabalho).GerarInformacoesBancariasMDFe(cargaMDFe.Carga, mdfe);

                        serCargaMDFe.AtualizarANTT(ref mdfe, TipoServicoMultisoftware, unidadeTrabalho);

                        if (!svcMDFe.Emitir(mdfe, unidadeTrabalho))
                            return new JsonpResult(false, true, "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + mdfe.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.");

                    }
                }

                if (carga.PossuiPendencia)
                {
                    carga.PossuiPendencia = false;
                    carga.problemaMDFe = false;
                    carga.MotivoPendencia = "";
                    repCarga.Atualizar(carga);
                }
                svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar reemitir os MDF-es.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ImprimirRelacaoEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Impressao serImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                byte[] pdf;
                if (configuracaoTMS.RelatorioEntregaPorPedido)
                    pdf = serImpressao.ObterPdfRelacaoEntregaPorPedido(codigoCarga, Cliente.NomeFantasia);
                else
                    pdf = serImpressao.ObterPdfRelacaoEntrega(codigoCarga, Cliente.NomeFantasia);

                return Arquivo(pdf, "application/pdf", $"Relatorio de Relacao de Entrega {DateTime.Now.ToString("dd-MM-yyyy HH-mm")}.pdf");
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImprimirRelacaoEmbarque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Impressao serImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (!configuracaoTMS.ExibirClassificacaoNFe)
                    throw new Exception("A configuração não permite que este relatório seja gerado!");

                byte[] pdf;
                pdf = serImpressao.ObterPdfRelacaoEmbarque(codigoCarga);

                return Arquivo(pdf, "application/pdf", $"Relatorio de Relacao de Embarque {DateTime.Now.ToString("dd-MM-yyyy HH-mm")}.pdf");
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarCargaComMDFesRejeitados()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_FinalizarCargaMDFeRejeitado))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Log.LogCargaLiberadaMDFeComRejeicao repLogCargaLiberadaMDFeComRejeicao = new Repositorio.Embarcador.Cargas.Log.LogCargaLiberadaMDFeComRejeicao(unidadeTrabalho);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Log.LogCargaLiberadaMDFeComRejeicao logCargaLiberadaMDFeComRejeicao = new Dominio.Entidades.Embarcador.Cargas.Log.LogCargaLiberadaMDFeComRejeicao();
                logCargaLiberadaMDFeComRejeicao.Justificativa = "";
                logCargaLiberadaMDFeComRejeicao.Carga = carga;
                logCargaLiberadaMDFeComRejeicao.DataRegistroLog = DateTime.Now;
                logCargaLiberadaMDFeComRejeicao.Usuario = this.Usuario;
                repLogCargaLiberadaMDFeComRejeicao.Inserir(logCargaLiberadaMDFeComRejeicao);

                if (carga.PossuiPendencia)
                {
                    carga.NaoGerarMDFe = true;
                    carga.PossuiPendencia = false;
                    carga.problemaMDFe = false;
                    carga.MotivoPendencia = "";
                    repCarga.Atualizar(carga);
                }
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Liberou a Carga com MDF-es Rejeitados", unidadeTrabalho);
                unidadeTrabalho.CommitChanges();

                svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar liberar a carga com os MDF-es rejeitados.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        //todo:remover esse método está sendo usado na tela de encerramento, mudar para buscar paginado assim como está na carga.
        public async Task<IActionResult> BuscarMDFeCarga()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                bool mdfesEmitidos = true;
                string mensagemPendenciaDocumento = "";

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = null;

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                {
                    cargaCancelamento = repCargaCancelamento.BuscarPorCarga(carga.Codigo);
                    if (cargaCancelamento.SituacaoCargaNoCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
                    {
                        mensagemPendenciaDocumento = carga.MotivoPendencia;
                        mdfesEmitidos = false;
                    }
                }
                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos && carga.PossuiPendencia)
                {
                    Servicos.Embarcador.Carga.Documentos serCargaDocumentos = new Servicos.Embarcador.Carga.Documentos(unidadeDeTrabalho);
                    mensagemPendenciaDocumento = carga.MotivoPendencia;

                    if (!carga.PossuiPendencia)
                        repCarga.Atualizar(carga);
                    else
                    {
                        mdfesEmitidos = false;
                        if (string.IsNullOrWhiteSpace(carga.MotivoPendencia))
                            mensagemPendenciaDocumento = carga.MotivoPendencia;
                    }
                }

                if (carga.AgImportacaoMDFe)
                    mdfesEmitidos = false;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFe.BuscarPorCarga(carga.Codigo);


                dynamic retorno;

                retorno = new
                {
                    Interestadual = true,
                    MDFeEmitidos = mdfesEmitidos,
                    CargaSubContratada = cargaPedidos.All(obj => obj.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada) ? true : false,
                    MDFeRejeitados = cargaMDFEs.Exists(obj => obj.MDFe != null && (obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)),
                    MDFeEmProcessamento = cargaMDFEs.Exists(obj => obj.MDFe != null && (obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                        obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado &&
                        obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                        obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao)),
                    MensagemPendenciaDocumento = mensagemPendenciaDocumento,
                    MDFeEncerrados = !cargaMDFEs.Exists(obj => obj.MDFe != null && obj.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe && (obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado && obj.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao)),
                    MDFEs = (from p in cargaMDFEs
                             where p.MDFe != null
                             select new
                             {
                                 p.Codigo,
                                 CodigoMDFE = p.MDFe.Codigo,
                                 CodigoEmpresa = p.MDFe.Empresa.Codigo,
                                 p.MDFe.Numero,
                                 MDFeAutorizado = p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? true : false,
                                 Serie = p.MDFe.Serie.Numero,
                                 Emissao = p.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                 UFCarga = p.MDFe.EstadoCarregamento.Sigla + " - " + p.MDFe.EstadoCarregamento.Nome,
                                 UFDesgarga = p.MDFe.EstadoDescarregamento.Sigla + " - " + p.MDFe.EstadoDescarregamento.Nome,
                                 Status = p.MDFe.DescricaoStatus,
                                 RetornoSefaz = p.MDFe.MensagemStatus != null ? p.MDFe.MensagemStatus.MensagemDoErro : System.Web.HttpUtility.HtmlEncode(p.MDFe.MensagemRetornoSefaz)
                             }).ToList(),
                    MDFEsManual = (from p in cargaMDFEs
                                   where p.MDFeManual != null
                                   select new
                                   {
                                       p.Codigo,
                                       CodigoMDFeManual = p.MDFeManual.Codigo,
                                       CodigoEmpresa = p.Carga.Empresa.Codigo,
                                       Numero = p.MDFeManual.Numero > 0 ? p.MDFeManual.Numero.ToString() : "",
                                       Chave = !string.IsNullOrWhiteSpace(p.MDFeManual.Chave) ? p.MDFeManual.Chave : "",
                                       DataInformacaoManual = p.MDFeManual.DataInformacaoManual.ToString("dd/MM/yyyy HH:mm"),
                                       UFCarga = p.MDFeManual.EstadoCarregamento.Sigla + " - " + p.MDFeManual.EstadoCarregamento.Nome,
                                       UFDesgarga = p.MDFeManual.EstadoDescarregamento.Sigla + " - " + p.MDFeManual.EstadoDescarregamento.Nome,
                                       DT_RowColor = p.MDFeManual.MDFeInformado ? "#dff0d8" : "",
                                       p.MDFeManual.MDFeInformado,
                                       Observacao = p.MDFeManual.ObservacaoOperador != null ? p.MDFeManual.ObservacaoOperador : ""
                                   }).ToList()
                };


                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os MDFEs.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ValidarMDFeAquaviarioTelaCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFeAquaviario = Request.GetIntParam("CodigoMDFeAquaviarioParaCancelamento");

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unidadeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFeAquaviario);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe = repCargaMDFeManualMDFe.BuscarPorMDFe(mdfe?.Codigo ?? 0);

                if (cargaMDFeManualMDFe == null)
                    return new JsonpResult(false, true, "MDF-e não encontrado.");

                if (cargaMDFeManualMDFe?.CargaMDFeManual?.MDFeRecebidoDeIntegracao ?? false)
                    return new JsonpResult(false, true, "MDF-e foi recebido via integração.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, true, "O MDF-e deve estar autorizado para realizar o Cancelamento.");


                var retorno = new
                {
                    CargaMDFeManualMDFe = new { Codigo = cargaMDFeManualMDFe?.Codigo ?? 0, Descricao = cargaMDFeManualMDFe?.Descricao ?? string.Empty }
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados gerais para cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Averbação

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCargaMDFeAverbacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

                int carga = Request.GetIntParam("Carga");
                int codigoCancelamentoCarga = Request.GetIntParam("CancelamentoCarga");
                int numeroMDFe = Request.GetIntParam("NumeroMDFe");

                Dominio.Enumeradores.StatusAverbacaoMDFe? situacao = null;
                if (Enum.TryParse(Request.Params("SituacaoAverbacao"), out Dominio.Enumeradores.StatusAverbacaoMDFe situacaoAux))
                    situacao = situacaoAux;

                string apolice = Request.Params("Apolice");

                // Grid
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("Nº MDF-e", "NumeroMDFe", 5, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Nº Averbação", "Averbacao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Apólice", "Apolice", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Seguradora", "Seguradora", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Averbadora", "DescricaoSeguradoraAverbacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoStatus", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Retorno", "DataRetorno", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Retorno", "MensagemRetorno", 20, Models.Grid.Align.left, true);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "NumeroMDFe") propOrdenacao = "MDFe.Numero";
                else if (propOrdenacao == "Apolice") propOrdenacao = "ApoliceSeguroAverbacao.ApoliceSeguro.NumeroApolice";
                else if (propOrdenacao == "Seguradora") propOrdenacao = "ApoliceSeguroAverbacao.ApoliceSeguro.Seguradora.Nome";
                else if (propOrdenacao == "DescricaoSeguradoraAverbacao") propOrdenacao = "ApoliceSeguroAverbacao.ApoliceSeguro.SeguradoraAverbacao";
                else if (propOrdenacao == "DescricaoStatus") propOrdenacao = "Status";

                // Busca os valores
                List<Dominio.Entidades.AverbacaoMDFe> averbacoes = repAverbacaoMDFe.ConsultaPorCarga(carga, codigoCancelamentoCarga, numeroMDFe, apolice, situacao, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                int totalRegistros = repAverbacaoMDFe.ContarConsultaPorCarga(carga, codigoCancelamentoCarga, numeroMDFe, apolice, situacao);

                var lista = (from obj in averbacoes
                             select new
                             {
                                 obj.Codigo,
                                 NumeroMDFe = obj.MDFe.Numero,
                                 obj.Averbacao,
                                 Apolice = obj.ApoliceSeguroAverbacao.ApoliceSeguro.NumeroApolice,
                                 Seguradora = obj.ApoliceSeguroAverbacao.ApoliceSeguro.Seguradora?.Nome ?? string.Empty,
                                 DescricaoSeguradoraAverbacao = obj.ApoliceSeguroAverbacao.ApoliceSeguro.SeguradoraAverbacao.Descricao(),
                                 DescricaoStatus = obj.Status.Descricao(),
                                 obj.Status,
                                 DataRetorno = obj.DataRetorno.HasValue ? obj.DataRetorno.Value.ToString("dd/MM/yyy") : "",
                                 obj.MensagemRetorno,
                                 DT_RowColor = obj.Status.CorLinha(),
                                 DT_FontColor = obj.Status.CorFonte()
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> ReaverbarRejeitadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                // Parâmetros
                int codigoCarga = Request.GetIntParam("Carga");

                // Busca averbacao
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                // Valida 
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                // Atualiza situação da carga
                if (carga.PossuiPendencia)
                {
                    carga.PossuiPendencia = false;
                    carga.ProblemaAverbacaoMDFe = false;
                    carga.MotivoPendencia = "";

                    repCarga.Atualizar(carga);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Reenviou averbações rejeitadas.", unitOfWork);

                unitOfWork.CommitChanges();

                string stringConexao = _conexao.StringConexao;

                Task.Factory.StartNew(() => AsyncAverbacaoRejeitados(codigoCarga, stringConexao));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar averbar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EncerrarAvervacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);
                Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

                int codigoAverbacaoMDFe = Request.GetIntParam("Codigo");
                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.AverbacaoMDFe averbacao = repAverbacaoMDFe.BuscarPorCodigoECarga(codigoAverbacaoMDFe, codigoCarga);

                if (averbacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                if (averbacao.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, true, "O MDF-e dessa averbação não está encerrado");

                if (averbacao.Status != Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso)
                    return new JsonpResult(false, true, "A situação da averbação (" + averbacao.DescricaoStatus + ") não permite essa ação");

                unitOfWork.Start();

                averbacao.Status = StatusAverbacaoMDFe.AgEncerramento;

                repAverbacaoMDFe.Atualizar(averbacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao, null, "Solicitou o encerramento da averbação.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao.MDFe, null, "Solicitou o encerramento da averbação.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar cancelar a averbação do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarAvervacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);
                Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

                int codigoAverbacaoMDFe = Request.GetIntParam("Codigo");
                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.AverbacaoMDFe averbacao = repAverbacaoMDFe.BuscarPorCodigoECarga(codigoAverbacaoMDFe, codigoCarga);

                if (averbacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                if (averbacao.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado)
                    return new JsonpResult(false, true, "O MDF-e dessa averbação não está cancelado");

                if (averbacao.Status != Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso)
                    return new JsonpResult(false, true, "A situação da averbação (" + averbacao.DescricaoStatus + ") não permite essa ação");

                unitOfWork.Start();

                averbacao.Status = StatusAverbacaoMDFe.AgCancelamento;

                repAverbacaoMDFe.Atualizar(averbacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao, null, "Solicitou o cancelamento da averbação.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao.MDFe, null, "Solicitou o cancelamento da averbação.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar cancelar a averbação do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AverbarMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);
                Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

                int codigoAverbacaoMDFe = Request.GetIntParam("Codigo");
                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.AverbacaoMDFe averbacao = repAverbacaoMDFe.BuscarPorCodigoECarga(codigoAverbacaoMDFe, codigoCarga);

                if (averbacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                if (averbacao.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
                    return new JsonpResult(false, true, "O MDF-e dessa averbação não está autorizado");

                if (averbacao.Status != Dominio.Enumeradores.StatusAverbacaoMDFe.Pendente && averbacao.Status != Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao)
                    return new JsonpResult(false, true, "A situação da averbação (" + averbacao.DescricaoStatus + ") não permite essa ação");

                unitOfWork.Start();

                averbacao.Status = StatusAverbacaoMDFe.AgEmissao;

                repAverbacaoMDFe.Atualizar(averbacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao, null, "Emitiu a averbação.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao.MDFe, null, "Emitiu a averbação.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar averbar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracaoAverbacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.AverbacaoMDFe integracao = repAverbacaoMDFe.BuscarPorCodigo(codigo, false);

                grid.setarQuantidadeTotal(integracao.ArquivosIntegracao.Count());

                var retorno = (from obj in integracao.ArquivosIntegracao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoAverbacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unidadeDeTrabalho);

                Dominio.Entidades.AverbacaoMDFe integracao = repAverbacaoMDFe.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosIntegracao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta da Averbação " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> LiberarComProblemaAverbacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_LiberarAverbacaoRejeitada))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                // Valida informações
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                if (carga.PossuiPendencia)
                {
                    carga.LiberarComProblemaAverbacaoMDFe = true;
                    carga.PossuiPendencia = false;
                    carga.ProblemaAverbacaoMDFe = false;
                    carga.MotivoPendencia = "";

                    repCarga.Atualizar(carga);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Avançou Etapa com Averbação Rejeitada.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                svcHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao avançar etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void AsyncAverbacaoRejeitados(int codigoCarga, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
                Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

                List<Dominio.Entidades.AverbacaoMDFe> averbacoesRejeitadas = repAverbacaoMDFe.BuscarPorCargaTipoEStatus(codigoCarga, Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao, new Dominio.Enumeradores.StatusAverbacaoMDFe[] { Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao, Dominio.Enumeradores.StatusAverbacaoMDFe.Pendente });

                if (averbacoesRejeitadas.Count > 0)
                {
                    foreach (Dominio.Entidades.AverbacaoMDFe averbacaoMDFe in averbacoesRejeitadas)
                    {
                        if (averbacaoMDFe.Tipo != Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao || averbacaoMDFe.Status != Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao)
                            continue;

                        averbacaoMDFe.Status = Dominio.Enumeradores.StatusAverbacaoMDFe.AgEmissao;

                        repAverbacaoMDFe.Atualizar(averbacaoMDFe);
                    }

                    svcHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private bool ObterHabilitarSincronizarDocumento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento)
        {
            if (mdfe.Status == StatusMDFe.Enviado && mdfe.DataIntegracao != null && (System.DateTime.Now.AddMinutes(-30) > mdfe.DataIntegracao))
            {
                if (mdfe.SistemaEmissor == null || mdfe.SistemaEmissor == TipoEmissorDocumento.Integrador && mdfe.CodigoIntegradorAutorizacao != 0)
                    return true;
                else if (mdfe.SistemaEmissor == TipoEmissorDocumento.NSTech)
                    return true;
            }
            else if (mdfe.Status == StatusMDFe.EmCancelamento && cargaCancelamento?.DataCancelamento != null && (System.DateTime.Now.AddMinutes(-30) > cargaCancelamento?.DataCancelamento))
            {
                if (mdfe.SistemaEmissor == TipoEmissorDocumento.NSTech)
                    return true;
            }
            else if (mdfe.Status == StatusMDFe.EmEncerramento)
            {
                if (mdfe.SistemaEmissor == TipoEmissorDocumento.NSTech)
                    return true;
            }
            else if (mdfe.Status == StatusMDFe.EventoInclusaoMotoristaEnviado)
            {
                if (mdfe.SistemaEmissor == TipoEmissorDocumento.NSTech)
                    return true;
            }

            return false;
        }

        private async Task RemoverPendenciaCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            bool cargaPossuiMDFeNaoAutorizado = await repCargaMDFe.ExisteMDFeNaoAutorizado(carga.Codigo, cancellationToken);

            if (carga.PossuiPendencia && !cargaPossuiMDFeNaoAutorizado)
            {
                carga.PossuiPendencia = false;
                carga.problemaMDFe = false;
                carga.MotivoPendencia = "";
                await repCarga.AtualizarAsync(carga);
            }
        }

        #endregion
    }
}
