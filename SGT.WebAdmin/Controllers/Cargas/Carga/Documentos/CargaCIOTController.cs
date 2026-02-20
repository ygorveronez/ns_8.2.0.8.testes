using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Carga
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "Imprimir" }, "Cargas/Carga")]
    public class CargaCIOTController : BaseController
    {
		#region Construtores

		public CargaCIOTController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                (dynamic lista, totalRegistros) = await ExecutaPesquisaAsync(propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork, cancellationToken);

                // Seta valores na grid
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeDeTrabalho);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                int codigoCargaCIOTArquivo = 0;
                int.TryParse(Request.Params("CodigoCargaCIOTArquivo"), out codigoCargaCIOTArquivo);

                byte[] arquivo = null;
                string ciotNumero = string.Empty;
                if (codigoCargaCIOTArquivo != 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCodigoArquivo(codigoCargaCIOTArquivo);

                    if (cargaCIOT == null)
                        return new JsonpResult(true, false, "Histórico não encontrado.");

                    Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo arquivoIntegracao = cargaCIOT.ArquivosTransacao.Where(o => o.Codigo == codigoCargaCIOTArquivo).FirstOrDefault();

                    if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                        return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                    arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });
                    ciotNumero = cargaCIOT.CIOT.Numero;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigoArquivo(codigo);

                    if (ciot == null)
                        return new JsonpResult(true, false, "Histórico não encontrado.");

                    Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo arquivoIntegracao = ciot.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                    if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                        return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                    arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });
                    ciotNumero = ciot.Numero;
                }

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta Integração CIOT " + ciotNumero + ".zip");
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

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                int codigoCargaCIOT = 0;
                int.TryParse(Request.Params("CodigoCargaCIOT"), out codigoCargaCIOT);

                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeDeTrabalho);
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCargaCIOTArquivo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo> arquivosTransacaoCIOT = null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo> arquivosTransacaoCargaCIOT = null;
                if (codigoCargaCIOT != 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCodigo(codigoCargaCIOT);
                    if (cargaCIOT.CIOT.CIOTPorPeriodo && !cargaCIOT.CargaAberturaCIOT)
                        arquivosTransacaoCargaCIOT = cargaCIOT.ArquivosTransacao.ToList();
                    else
                        arquivosTransacaoCIOT = cargaCIOT.CIOT.ArquivosTransacao.ToList();
                }
                else
                {
                    Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigo);
                    arquivosTransacaoCIOT = ciot.ArquivosTransacao.ToList();
                }

                dynamic retorno = null;
                if (arquivosTransacaoCargaCIOT != null)
                {
                    grid.setarQuantidadeTotal(arquivosTransacaoCargaCIOT.Count());

                    retorno = (from obj in arquivosTransacaoCargaCIOT.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   Codigo = 0,
                                   CodigoCargaCIOTArquivo = obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();
                }
                else
                {
                    grid.setarQuantidadeTotal(arquivosTransacaoCIOT.Count());

                    retorno = (from obj in arquivosTransacaoCIOT.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                                       select new
                                       {
                                           obj.Codigo,
                                           CodigoCargaCIOTArquivo = 0,
                                           Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                           obj.DescricaoTipo,
                                           obj.Mensagem
                                       }).ToList();
                }

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
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 20, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargasCIOT = repCargaCIOT.BuscarPorCIOT(codigo);

                grid.setarQuantidadeTotal(cargasCIOT.Count());

                var retorno = (from obj in cargasCIOT.OrderByDescending(o => o.Codigo).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   obj.Carga.CodigoCargaEmbarcador,
                                   Situacao = obj.CIOT.Situacao == SituacaoCIOT.Aberto && obj.CIOT.CIOTPorPeriodo && !obj.CargaAdicionadaAoCIOT ? (obj.Situacao ?? SituacaoCIOT.AgIntegracao) : obj.CIOT.Situacao,
                                   DescricaoSituacao = obj.CIOT.Situacao == SituacaoCIOT.Aberto && obj.CIOT.CIOTPorPeriodo && !obj.CargaAdicionadaAoCIOT ? (!string.IsNullOrEmpty(obj.DescricaoSituacao) ? obj.DescricaoSituacao : "Ag. Integração") : obj.CIOT.DescricaoSituacao,
                                   DT_RowColor = CorFundoCiot(obj),
                                   DT_FontColor = CorFonteCIOT(obj),
                                   Mensagem = obj.CIOT.Situacao == SituacaoCIOT.Aberto && obj.CIOT.CIOTPorPeriodo && !obj.CargaAdicionadaAoCIOT ? obj.Mensagem : obj.CIOT.Mensagem
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
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTTarget repCIOTTarget = new Repositorio.Embarcador.CIOT.CIOTTarget(unitOfWork);

                // Parametros
                int carga = Request.GetIntParam("Carga");
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(carga);

                // Valida
                if (cargaCIOT == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.CIOT.CIOTTarget configuracao = cargaCIOT.CIOT.Operadora == OperadoraCIOT.Target ? repCIOTTarget.BuscarPorConfiguracaoCIOT(cargaCIOT.CIOT.ConfiguracaoCIOT.Codigo) : null;
                bool imprimirCiotTarget = configuracao != null && configuracao.UtilizarCiotTarget;

                byte[] arquivo = null;
                string mensagemErro = string.Empty;
                if (cargaCIOT.CIOT.Operadora == OperadoraCIOT.eFrete)
                {
                    Servicos.Embarcador.CIOT.EFrete serEfrete = new Servicos.Embarcador.CIOT.EFrete();
                    arquivo = serEfrete.ObterOperacaoTransportePdf(cargaCIOT, unitOfWork, out string erro);
                    if (arquivo == null)
                        return new JsonpResult(false, false, erro);
                }
                else if (imprimirCiotTarget)
                {
                    Servicos.Embarcador.CIOT.Target serTarget = new Servicos.Embarcador.CIOT.Target();
                    arquivo = serTarget.ImprimirCIOT(cargaCIOT.CIOT, unitOfWork, out string erro);
                    if (arquivo == null)
                        return new JsonpResult(false, false, erro);
                }
                else
                    arquivo = new Servicos.Embarcador.CIOT.CIOT().GerarContratoFrete(cargaCIOT.CIOT.Codigo, unitOfWork, out mensagemErro);

                if (arquivo == null)
                {
                    if (!string.IsNullOrWhiteSpace(mensagemErro))
                        return new JsonpResult(false, false, "Não foi possível gerar o PDF: " + mensagemErro);
                    else
                        return new JsonpResult(false, false, "Não foi possível gerar o PDF.");
                }

                return Arquivo(arquivo, "application/pdf", "CIOT - " + cargaCIOT.CIOT.Numero.ToString() + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o PDF.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                // Parâmetros
                int codigoCIOT;
                int.TryParse(Request.Params("Codigo"), out codigoCIOT);

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, true);
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(codigoCarga);

                // Valida informações
                if (ciot == null || carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (ciot.CIOTPorPeriodo && ciot.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                {
                    if (cargaCIOT.CargaAdicionadaAoCIOT)
                        return new JsonpResult(false, true, "A situação do CIOT (" + ciot.DescricaoSituacao + ") não permite essa ação.");
                }
                else if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia && ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao)
                    return new JsonpResult(false, true, "A situação do CIOT (" + ciot.DescricaoSituacao + ") não permite essa ação.");

                if (!configuracaoTMS.PermitirReenvioCIOTCargaEmitida && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
                    return new JsonpResult(false, true, "A situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite essa ação.");

                unitOfWork.Start();

                carga.PossuiPendencia = false;
                carga.ProblemaIntegracaoCIOT = false;
                carga.MotivoPendencia = "";

                if (ciot.CIOTPorPeriodo && ciot.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                {
                    cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                    repCargaCIOT.Atualizar(cargaCIOT);
                }
                else
                {
                    ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao;
                    repCIOT.Atualizar(ciot);
                }

                //Validação feita para cargas do GPA, que são encerradas pela Fluxo de Patio e precisa reenviar o CIOT Rejeitado                
                if (configuracaoTMS.PermitirReenvioCIOTCargaEmitida &&
                    (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte) ||
                    (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada) ||
                    (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao))
                {
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos;
                    carga.NaoGerarMDFe = true;
                }

                repCarga.Atualizar(carga, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ciot, null, $"Reenviou a integração do CIOT na carga {carga.CodigoCargaEmbarcador}.", unitOfWork);

                unitOfWork.CommitChanges();

                if (ciot.Operadora == OperadoraCIOT.RepomFrete)
                {
                    string mensagemErro;
                    Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete svcRepomFrete = new Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete();
                    svcRepomFrete.AjustarReenviarCIOT(ciot, cargaCIOT, TipoServicoMultisoftware, unitOfWork, out mensagemErro);

                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar reenviar a integração do CIOT.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarRegistroDeDesembarque()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();


                // Parâmetros
                int codigoCIOT;
                int.TryParse(Request.Params("Codigo"), out codigoCIOT);

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, true);

                // Valida informações
                if (ciot == null || carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto && ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)
                    return new JsonpResult(false, true, "A situação do CIOT (" + ciot.DescricaoSituacao + ") não permite essa ação.");

                if (ciot.Operadora != Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete)
                    return new JsonpResult(false, true, "Operação disponível apenas para a operadora eFrete.");

                Servicos.Embarcador.CIOT.EFrete serEfrete = new Servicos.Embarcador.CIOT.EFrete();
                if (serEfrete.GerarRegistroDeDesembarque(ciot, unitOfWork, out string erro))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ciot, null, $"Gerou registro de desembarque do CIOT na carga {carga.CodigoCargaEmbarcador}.", unitOfWork);
                else
                    return new JsonpResult(false, true, erro);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar gerar registro de desembarque do CIOT.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoCargaCIOT", false);
            grid.AdicionarCabecalho("Situacao", false);
            grid.AdicionarCabecalho("Número", "Numero", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Cód. Verificador", "CodigoVerificador", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Protocolo", "ProtocoloAutorizacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo CIOT", "TipoCIOT", 9, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Final da Viagem", "DataFinalViagem", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Para Fechamento", "DataParaFechamento", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Mensagem", "Mensagem", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("RegistradoPeloEmbarcador", false);
            grid.AdicionarCabecalho("StatusMDFe", false);

            return grid;
        }

        private async Task<(dynamic lista, int total)> ExecutaPesquisaAsync(string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaInformacoesBancarias repCargaInformacoesBancarias = new Repositorio.Embarcador.Cargas.CargaInformacoesBancarias(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork, cancellationToken);

            int carga = Request.GetIntParam("Carga");
            int codigoCiot = Request.GetIntParam("EntidadeCIOT");
            if (codigoCiot == 0)
                codigoCiot = Request.GetIntParam("EntidadeCIOTPreCte");

            List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> listaGrid = await repCargaCIOT.ConsultarAsync(carga, codigoCiot, propOrdenar, dirOrdena, inicio, limite);
            int totalRegistros = await repCargaCIOT.ContarConsultaAsync(carga, codigoCiot);

            bool registradoPeloEmbarcador = await repCargaInformacoesBancarias.RegistradoPeloEmbarcadorPorCargaAsync(carga);
            bool statusMDFe = await repCargaMDFe.ExisteStatusMDFePorCargaAsync(carga);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.CIOT.Codigo,
                            CodigoCargaCIOT = obj.Codigo,
                            obj.CIOT.Numero,
                            obj.CIOT.CodigoVerificador,
                            obj.CIOT.ProtocoloAutorizacao,
                            TipoCIOT = obj.CIOT.CIOTPorPeriodo ? "Por Período" : "Por Viagem",
                            Situacao = obj.CIOT.Situacao == SituacaoCIOT.Aberto && obj.CIOT.CIOTPorPeriodo && !obj.CargaAdicionadaAoCIOT ? (obj.Situacao ?? SituacaoCIOT.AgIntegracao) : obj.CIOT.Situacao,
                            DataFinalViagem = obj.CIOT.DataFinalViagem.ToString("dd/MM/yyyy"),
                            DataParaFechamento = obj.CIOT.DataParaFechamento?.ToString("dd/MM/yyyy") ?? string.Empty,
                            Transportador = obj.CIOT.Transportador.Nome,
                            DescricaoSituacao = obj.CIOT.Situacao == SituacaoCIOT.Aberto && obj.CIOT.CIOTPorPeriodo && !obj.CargaAdicionadaAoCIOT ? (!string.IsNullOrEmpty(obj.DescricaoSituacao) ? obj.DescricaoSituacao : "Ag. Integração") : obj.CIOT.DescricaoSituacao,
                            DT_RowColor = CorFundoCiot(obj),
                            DT_FontColor = CorFonteCIOT(obj),
                            Mensagem = obj.CIOT.Situacao == SituacaoCIOT.Aberto && obj.CIOT.CIOTPorPeriodo && !obj.CargaAdicionadaAoCIOT ? obj.Mensagem : obj.CIOT.Mensagem,
                            RegistradoPeloEmbarcador = registradoPeloEmbarcador,
                            StatusMDFe = statusMDFe
                        };

            return (lista.ToList(), totalRegistros);
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Transportador") propOrdenar = "Transportador.Nome";

            propOrdenar = "CIOT." + propOrdenar;
        }

        private string CorFonteCIOT(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia ||
                cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado)
                return "#FFF";
            else if (cargaCIOT.CIOT.Situacao == SituacaoCIOT.Aberto && cargaCIOT.CIOT.CIOTPorPeriodo && !cargaCIOT.CargaAdicionadaAoCIOT && cargaCIOT.Situacao == SituacaoCIOT.Pendencia)
                return "#FFF";

            return "";
        }

        private string CorFundoCiot(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            if (cargaCIOT.CIOT.Situacao == SituacaoCIOT.Aberto && cargaCIOT.CIOT.CIOTPorPeriodo && !cargaCIOT.CargaAdicionadaAoCIOT)
            {
                if ((cargaCIOT.Situacao ?? SituacaoCIOT.AgIntegracao) == SituacaoCIOT.AgIntegracao)
                    return "";
                else if (cargaCIOT.Situacao == SituacaoCIOT.Pendencia)
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;
            }
            else
            {
                if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado)
                    return "#777";
                else if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto ||
                        cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem ||
                        cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)
                    return cargaCIOT.CIOT.CIOTPorPeriodo ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
                else if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia)
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;
            }

            return "";
        }

        #endregion
    }
}
