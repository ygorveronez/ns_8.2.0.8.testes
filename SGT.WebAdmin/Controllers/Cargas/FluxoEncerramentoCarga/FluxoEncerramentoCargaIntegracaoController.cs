using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.FluxoEncerramentoCarga
{
    public class FluxoEncerramentoCargaIntegracaoController : BaseController
    {
		#region Construtores

		public FluxoEncerramentoCargaIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Tipo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número de Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data de Envio", "DataEnvio", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "ProblemaIntegracao", 45, Models.Grid.Align.left, true);

                List<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao> listaDadosCancelamento = repIntegracao.Consultar(codigo, situacao, "Codigo", grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repIntegracao.ContarBusca(codigo, situacao));

                var dynRetorno = (from obj in listaDadosCancelamento
                                  select new
                                  {
                                      obj.Codigo,
                                      TipoIntegracao = obj.TipoIntegracao?.Descricao ?? "",
                                      SituacaoIntegracao = obj.DescricaoSituacaoIntegracao,
                                      Tentativas = obj.NumeroTentativas.ToString(),
                                      DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                      obj.ProblemaIntegracao,
                                      DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ? "#ADD8E6" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? "#DFF0D8" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? "#C16565" :
                                                    obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? "#F7F7BA" :
                                                    "#FFFFFF",
                                      DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? "#FFFFFF" : "#666666"
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar integrações");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaCancelamento = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repIntegracao.ContarPorCargaCancelamento(codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repIntegracao.ContarPorCargaCancelamento(codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repIntegracao.ContarPorCargaCancelamento(codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repIntegracao.ContarPorCargaCancelamento(codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);

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
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultarTotalizadoresIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao integracaoDadosCancelamento = repIntegracao.BuscarPorCodigo(codigo, false);

                if (integracaoDadosCancelamento == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.IntegracaoNaoEncontrada);

                unitOfWork.Start();

                integracaoDadosCancelamento.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracaoDadosCancelamento, null, Localization.Resources.Cargas.CancelamentoCarga.ReenviouIntegracao, unitOfWork);

                repIntegracao.Atualizar(integracaoDadosCancelamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu falha ao reenviar integração");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao> integracoesPendentes = repIntegracao.BuscarIntegracoesPorCargaEncerramento(codigo);

                unitOfWork.Start();

                foreach (var integracao in integracoesPendentes)
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                    repIntegracao.Atualizar(integracao);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu um erro ao reenviar as integrações");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Data, "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Tipo, "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo, false);

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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Pedidos.Pedido.NaoHaRegistrosArquivosSalvosParaEsteHistoricoConsulta);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", $"Arquivo Integracao {integracao.CargaRegistroEncerramento.Carga.CodigoCargaEmbarcador}.zip" );
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoRealizarDownloadIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
