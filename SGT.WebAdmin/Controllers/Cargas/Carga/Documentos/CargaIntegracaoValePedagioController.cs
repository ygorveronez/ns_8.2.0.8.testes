using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.IO;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Documentos
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ImpressaoValePedagio" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaIntegracaoValePedagioController : BaseController
    {
		#region Construtores

		public CargaIntegracaoValePedagioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);


                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio integracao = repCargaValePedagio.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta Integração Vale Pedágio " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio integracao = repCargaValePedagio.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
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
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> LiberarComProblemaValePedagio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeitacaoValePedagio))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissão para executar essa ação.");

                // Valida informações
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                if (carga.PossuiPendencia)
                {
                    carga.LiberadoComProblemaValePedagio = true;
                    carga.PossuiPendencia = false;
                    carga.MotivoPendencia = "";

                    Servicos.Embarcador.Carga.PreCTe.VerificarSeLiberaCargaSemIntegrarCTes(carga);

                    repCarga.Atualizar(carga);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Avançou Etapa com Vale Pedágios Rejeitados.", unitOfWork);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCargaValePedagio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaValePedagio repCargaPedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);

                int carga = Request.GetIntParam("Carga");
                int codigoCancelamentoCarga = Request.GetIntParam("CancelamentoCarga");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;

                if (!string.IsNullOrEmpty(Request.Params("SituacaoIntegracaoValePedagio")))
                {
                    if (Enum.TryParse(Request.Params("SituacaoIntegracaoValePedagio"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                        situacao = situacaoAux;
                }

                if (!string.IsNullOrEmpty(Request.Params("SituacaoIntegracaoValePedagioPreCte")))
                {
                    if (Enum.TryParse(Request.Params("SituacaoIntegracaoValePedagioPreCte"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                        situacao = situacaoAux;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);
                grid.AdicionarCabecalho("TipoIntegradora", false);
                grid.AdicionarCabecalho("SituacaoValePedagio", false);
                grid.AdicionarCabecalho("Nº Vale Pedagio", "NumeroValePedagio", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nº Recibo Vale Pedagio", "NumeroReciboValePedagio", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Integradora", "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Percurso", "TipoPercursoVP", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo da Compra", "TipoCompra", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação VP", "DescricaoSituacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor VP", "ValorValePedagio", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação Integração", "DescricaoSituacaoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Retorno", "DataIntegracao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Retorno", "ProblemaIntegracao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("RecebidoPorIntegracao", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "DescricaoSituacaoIntegracao") propOrdenacao = "SituacaoIntegracao";
                else if (propOrdenacao == "TipoIntegradora") propOrdenacao = "TipoIntegracao.Tipo";
                else if (propOrdenacao == "TipoIntegracao") propOrdenacao = "TipoIntegracao.Descricao";
                else if (propOrdenacao == "DescricaoSituacao") propOrdenacao = "SituacaoValePedagio";

                bool ctesSubContratacaoFilialEmissora = false;
                bool ctesSemSubContratacaoFilialEmissora = false;
                bool retornarDocumentoOperacaoContainer = false;

                bool.TryParse(Request.Params("CTesSubContratacaoFilialEmissora"), out ctesSubContratacaoFilialEmissora);
                bool.TryParse(Request.Params("CTesSemSubContratacaoFilialEmissora"), out ctesSemSubContratacaoFilialEmissora);
                bool.TryParse(Request.Params("RetornarDocumentoOperacaoContainer"), out retornarDocumentoOperacaoContainer);

                int totalRegistros = !ctesSemSubContratacaoFilialEmissora ? repCargaValePedagio.ContarConsulta(carga, codigoCancelamentoCarga, situacao, null, retornarDocumentoOperacaoContainer) : 0;

                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> averbacoesCarga = !ctesSemSubContratacaoFilialEmissora ? repCargaValePedagio.Consultar(carga, codigoCancelamentoCarga, situacao, null, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite, retornarDocumentoOperacaoContainer) : new List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> cargaValePedagios = repCargaPedagio.BuscarPorCarga(carga);

                if (averbacoesCarga.Any(x => x.RotaFrete != null))
                {
                    grid.AdicionarCabecalho("Rota", "RotaFrete", 15, Models.Grid.Align.center, true);
                }

                List<dynamic> lista = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio obj in averbacoesCarga)
                {
                    string numeroReciboVP = cargaValePedagios
                        .Where(o => o.CargaIntegracaoValePedagio != null && o.CargaIntegracaoValePedagio.Codigo == obj.Codigo).Select(o => o.NumeroComprovante).FirstOrDefault() ?? "";

                    lista.Add(new
                    {
                        obj.Codigo,
                        obj.NumeroValePedagio,
                        NumeroReciboValePedagio = numeroReciboVP,
                        ValorValePedagio = obj.ValorValePedagio.ToString("n2"),
                        RotaFrete = obj.RotaFrete?.Descricao != null ? obj.RotaFrete.Descricao : null,
                        obj.SituacaoIntegracao,
                        obj.DescricaoSituacaoIntegracao,
                        obj.NumeroTentativas,
                        ProblemaIntegracao = string.IsNullOrWhiteSpace(obj.Observacao1) ? obj.ProblemaIntegracao : obj.Observacao1,
                        TipoCompra = obj.TipoCompraDescricao,
                        TipoIntegradora = obj.TipoIntegracao.Tipo,
                        TipoPercursoVP = obj.TipoPercursoVP?.ObterDescricao() ?? "",
                        DataIntegracao = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                        TipoIntegracao = obj.TipoIntegracao.Descricao,
                        DescricaoSituacao = obj.SituacaoValePedagio.ObterDescricao(),
                        obj.SituacaoValePedagio,
                        obj.RecebidoPorIntegracao,
                        DT_RowColor = CorFundoValePedagio(obj),
                        DT_FontColor = CorFonteValePedagio(obj)
                    });
                }

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

        public async Task<IActionResult> CancelarValePedagio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repCargaValePedagio.BuscarPorCodigo(codigo);

                if (cargaIntegracaoValePedagio == null)
                    return new JsonpResult(true, false, "Vale Pedagio Não encontrado.");

                if (cargaIntegracaoValePedagio.PedagioIntegradoEmbarcador)
                    return new JsonpResult(true, false, "Pedágio integrado do embarcador não pode ser cancelado.");

                cargaIntegracaoValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.EmCancelamento;
                repCargaValePedagio.Atualizar(cargaIntegracaoValePedagio);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar o vale pedágio para cancelamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ImpressaoValePedagio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeDeTrabalho);
                Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repCargaValePedagio.BuscarPorCodigo(codigo);
                
                byte[] arquivo = null;
                string retorno = servicoValePedagio.ObterArquivoValePedagio(cargaIntegracaoValePedagio, ref arquivo, TipoServicoMultisoftware);

                if (!string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(false, true, retorno);

                if (arquivo == null)
                    return new JsonpResult(false, true, "Não foi possível gerar o Vale Pedágio");

                return Arquivo(arquivo, "application/pdf", cargaIntegracaoValePedagio.TipoIntegracao.DescricaoTipo + " - " + cargaIntegracaoValePedagio.NumeroValePedagio + ".pdf");
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do vale pedagio.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> BuscarURLValePedagio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repCargaValePedagio.BuscarPorCodigo(codigo);

                if (string.IsNullOrWhiteSpace(cargaIntegracaoValePedagio.Observacao6))
                    return new JsonpResult(false, "Ocorreu para vale pedagio não disponível.");

                return new JsonpResult(true, true, cargaIntegracaoValePedagio.Observacao6);
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao carregar URL do vale pedagio.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ImpressaoRecibo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);


                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repCargaValePedagio.BuscarPorCodigo(codigo);

                if (cargaIntegracaoValePedagio == null)
                    return new JsonpResult(true, false, "Vale Pedagio Não encontrado.");

                if (cargaIntegracaoValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar)
                {
                    return new JsonpResult(true, false, "Integradora sem parar não disponibiliza documento para impressão via integração.");
                    //Servicos.Embarcador.Integracao.SemParar.ValePedagio serValePedagioSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
                    //Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencia = serValePedagioSemParar.Autenticar(cargaIntegracaoValePedagio, unidadeDeTrabalho, false);
                    //if (credencia.Autenticado)
                    //{
                    //    byte[] arquivo = serValePedagioSemParar.ObterImpressaoValePedagio(credencia, cargaIntegracaoValePedagio, unidadeDeTrabalho);
                    //    //byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                    //    return Arquivo(arquivo, "application/pdf", cargaIntegracaoValePedagio.NumeroValePedagio + ".pdf");
                    //    //return Arquivo(arquivo, "application/zip", "Arquivos Consulta Integração Vale Pedágio " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
                    //}
                    //else
                    //{
                    //    return new JsonpResult(true, false, credencia.Retorno);
                    //}
                }
                else
                {
                    return new JsonpResult(true, false, "Não existe impressão para o tipo de vale pedagio utilizado.");
                }
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

        public async Task<IActionResult> CancelarValePedagioNaCarga()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repCargaValePedagio.BuscarPorCodigo(codigo);

                if (cargaIntegracaoValePedagio == null)
                    return new JsonpResult(true, false, "Vale Pedagio não encontrado.");

                if (cargaIntegracaoValePedagio.PedagioIntegradoEmbarcador)
                    return new JsonpResult(true, false, "Pedágio integrado do embarcador não pode ser cancelado.");

                cargaIntegracaoValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.EmCancelamento;
                repCargaValePedagio.Atualizar(cargaIntegracaoValePedagio);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaIntegracaoValePedagio.Carga, null, "Solicitou cancelamento individual do vale pedágio.", unidadeDeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao solicitar cancelamento do vale pedágio.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracaoRejeitadas()
        {
            string stringConexao = _conexao.StringConexao;
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(unitOfWork);

                // Parâmetros
                int codigoValePedagio;
                int.TryParse(Request.Params("Codigo"), out codigoValePedagio);

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                // Busca averbacao
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorCodigo(codigoValePedagio);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(cargaValePedagio.Carga, TipoServicoMultisoftware);

                // Valida informações
                if (cargaValePedagio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                bool gerarPedagioValorZeradoForaDoMesVigente = integracaoSemParar != null && integracaoSemParar.ComprarSomenteNoMesVigente && !cargaValePedagio.Carga.DataCriacaoCarga.IsDateSameMonth(DateTime.Now) &&
                    !(cargaValePedagio.Carga.DataCriacaoCarga.IsLastDayOfMonth() && cargaValePedagio.Carga.DataCriacaoCarga.Hour >= 20 && DateTime.Now.IsFirstDayOfMonth());

                if (gerarPedagioValorZeradoForaDoMesVigente)
                    return new JsonpResult(false, true, "Configurado para não comprar em mês diferente da criação da carga. Caso deseje reenviar a solicitação fora do mês vigente, é necessário retirar a configuração existente.");

                // Atualiza situação da carga
                if (cargaValePedagio.Carga.PossuiPendencia || cargaValePedagio.Carga.AgImportacaoCTe)
                {
                    cargaValePedagio.Carga.PossuiPendencia = false;
                    cargaValePedagio.Carga.ProblemaIntegracaoValePedagio = false;
                    cargaValePedagio.Carga.IntegrandoValePedagio = true;
                    cargaValePedagio.Carga.MotivoPendencia = "";

                    repCarga.Atualizar(cargaValePedagio.Carga);

                    if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                    else
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaValePedagio, null, "Reenviou integração rejeitada.", unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaValePedagio.Carga, null, "Reenviou integração rejeitada.", unitOfWork);
                    repCargaValePedagio.Atualizar(cargaValePedagio);
                }
                unitOfWork.CommitChanges();
                svcHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar reenviar a integração do vale pedágio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string CorFonteValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            if (cargaValePedagio.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao || cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada)
                return "#FFF";

            return "";
        }

        private string CorFundoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada)
                return "#777";

            if (cargaValePedagio.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (cargaValePedagio.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;
            if (cargaValePedagio.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo;

            return "";
        }

        private string ObterCaminhoPorTipoIntegracaoValePedagio(Repositorio.UnitOfWork unitOfWork, TipoIntegracao integracao)
        {
            string diretorioArquivos = string.Empty;

            if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos == null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoArquivo.BuscarPrimeiroRegistro();
                diretorioArquivos = configuracaoArquivo.CaminhoArquivosIntegracao;
            }
            else
                diretorioArquivos = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Integracao");

            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(diretorioArquivos, TipoIntegracaoValePedagioHelper.ObterPastaPorTipoIntegracao(integracao));

            return caminhoArquivo;
        }

        #endregion
    }
}
