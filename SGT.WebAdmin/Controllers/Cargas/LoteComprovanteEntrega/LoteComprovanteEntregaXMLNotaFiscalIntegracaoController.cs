using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.LoteComprovanteEntrega
{
    [CustomAuthorize(new string[] { "ObterTotais", "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class LoteComprovanteEntregaXMLNotaFiscalIntegracaoController : BaseController
    {
		#region Construtores

		public LoteComprovanteEntregaXMLNotaFiscalIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoLote;
                int.TryParse(Request.Params("Codigo"), out codigoLote);

                int codigoXmlComprovanteEntrega;
                int.TryParse(Request.Params("XmlNotaComprovanteEntrega"), out codigoXmlComprovanteEntrega);
                bool integracaoTransportador = Request.GetBoolParam("XmlNotaComprovanteEntrega");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);
                grid.AdicionarCabecalho("CTe", "CTe", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Parada", "Parada", 5, Models.Grid.Align.left, true); 
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Mensagem", "Retorno", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 8, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao(unidadeDeTrabalho, integracaoTransportador);

                int countLotes = repIntegracao.ContarConsulta(codigoLote, codigoXmlComprovanteEntrega, situacao, tipo);

                List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao> listaIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao>();

                if (countLotes > 0)
                {
                    if (propOrdena == "TipoIntegracao")
                        propOrdena = "TipoIntegracao.Tipo";
                    else if (propOrdena == "Tentativas")
                        propOrdena = "NumeroTentativas";
                    else if (propOrdena == "DataEnvio")
                        propOrdena = "DataIntegracao";
                    else if (propOrdena == "Situacao")
                        propOrdena = "SituacaoIntegracao";

                    listaIntegracao = repIntegracao.Consultar(codigoLote, codigoXmlComprovanteEntrega, situacao, tipo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                }

                grid.setarQuantidadeTotal(countLotes);

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.SituacaoIntegracao,
                                       Parada = obj.XMLNotaFiscalComprovanteEntrega.CargaEntrega.Codigo,
                                       CTe = obj.XMLNotaFiscalComprovanteEntrega.Cte.CargaCTe.CTe.Numero,
                                       obj.TipoIntegracao.Tipo,
                                       Situacao = obj.SituacaoIntegracao.ObterDescricao(),
                                       TipoIntegracao = obj.TipoIntegracao.Tipo.ObterDescricao() + (obj.IntegracaoColeta ? " (Coleta)" : string.Empty),
                                       Retorno = obj.ProblemaIntegracao,
                                       Protocolo = !string.IsNullOrWhiteSpace(obj.Protocolo) ? obj.Protocolo : "",
                                       Tentativas = obj.NumeroTentativas,
                                       DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                       DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo :
                                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                                       DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
                                   }).ToList()); ;

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


        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLote;
                int.TryParse(Request.Params("Codigo"), out codigoLote);
                bool integracaoTransportador = Request.GetBoolParam("IntegracaoTransportador");

                Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao(unidadeDeTrabalho, integracaoTransportador);

                int totalAguardandoIntegracao = repIntegracao.ContarPorLote(codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, false);
                int totalIntegrado = repIntegracao.ContarPorLote(codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, false);
                int totalProblemaIntegracao = repIntegracao.ContarPorLote(codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, false);
                int totalAguardandoRetorno = repIntegracao.ContarPorLote(codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno, false);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao(unidadeDeTrabalho, false);
                Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao loteIntegracao = repIntegracao.BuscarPorCodigo(codigo);


                if (loteIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");


                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes) && loteIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                loteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, loteIntegracao, null, "Reenviou Integração.", unidadeDeTrabalho);
                repIntegracao.Atualizar(loteIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodos()
        {

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoLote = 0;
                int.TryParse(Request.Params("Codigo"), out codigoLote);
                bool integracaoTransportador = Request.GetBoolParam("IntegracaoTransportador");

                Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao(unidadeDeTrabalho, false);


                List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao> integracoes = repIntegracao.BuscarPorCodigo(codigoLote, situacao, tipo);

                foreach (Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao integracao in integracoes)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    repIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou Integração.", unidadeDeTrabalho);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosIntegracaoComprovanteEntrega()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao(unidadeDeTrabalho, false);
                Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao loteIntegracao = repIntegracao.BuscarPorCodigo(codigo);


                if (loteIntegracao == null)
                    return new JsonpResult(true, false, "Integração não encontrada.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = loteIntegracao.ArquivosTransacao.FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Comprovante Entrega CTe " + arquivoIntegracao.Codigo + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download.");
            }
        }
    }
}
