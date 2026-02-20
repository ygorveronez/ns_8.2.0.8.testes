using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    public class FluxoPatioIntegracaoController : BaseController
    {
		#region Construtores

		public FluxoPatioIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos

        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioFluxoPatioIntegracao = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoPatioIntegracao filtroPequisa = ObterFiltrosPesquisa();
                Models.Grid.Grid grid = ObterGrid();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao> fluxoPatioIntegracoes = repositorioFluxoPatioIntegracao.Consultar(filtroPequisa, parametrosConsulta);
                int totalRegitrosIntegracao = repositorioFluxoPatioIntegracao.ContarConsultar(filtroPequisa);

                grid.setarQuantidadeTotal(totalRegitrosIntegracao);
                grid.AdicionaRows(FormatarRetorno(fluxoPatioIntegracoes));

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
        }
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repIntegracao = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.HistoricoNaoEncontrado);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.NaoHaRegistrosParaEsseHistorico);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Integração Fluxo de Pátio - " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
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

                Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repIntegracao = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo, false);
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
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioFluxoPatioIntegracao = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao fluxoPatioIntegracao = repositorioFluxoPatioIntegracao.BuscarPorCodigo(codigo, false);

                fluxoPatioIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoPatioIntegracao, null,"Reenvio de integração", unitOfWork);

                repositorioFluxoPatioIntegracao.Atualizar(fluxoPatioIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoReenviarArquivoParaIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoPatioIntegracao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoPatioIntegracao()
            {
                CodigoIntegradora = Request.GetIntParam("Integradora"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                Data = Request.GetNullableDateTimeParam("Data"),
                EtapaFluxo = Request.GetEnumParam<EtapaFluxoGestaoPatio>("EtapaPatio"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracao")
            };
        }

        private Models.Grid.Grid ObterGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Numero Carga", "NumeroCarga", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destinatario", "Destinatario", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Integradora", "Integradora", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação Integração", "SituacaoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Etapa Patio", "EtapaPatio", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Integração", "DataIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 10, Models.Grid.Align.left, true);
            return grid;

        }

        private dynamic FormatarRetorno(List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao> fluxoPatioIntegracoes)
        {
            return from obj in fluxoPatioIntegracoes
                   select new
                   {
                       obj.Codigo,
                       NumeroCarga = obj.Carga.CodigoCargaEmbarcador,
                       Remetente = obj.Carga?.Pedidos?.Where(p => p.Pedido?.Remetente != null)?.Select(pd => pd.Pedido.Remetente)?.FirstOrDefault()?.Descricao ?? string.Empty,
                       Destinatario = obj.Carga?.Pedidos?.Where(p => p.Pedido?.Destinatario != null)?.Select(pd => pd.Pedido.Destinatario)?.FirstOrDefault()?.Descricao ?? string.Empty,
                       Filial = obj.Carga?.Filial?.Descricao ?? string.Empty,
                       Integradora = obj.TipoIntegracao?.Descricao ?? string.Empty,
                       EtapaPatio = obj.EtapaFluxoGestaoPatio.ObterDescricao(),
                       SituacaoIntegracao = obj.SituacaoIntegracao.ObterDescricao(),
                       DataIntegracao = obj.DataIntegracao,
                       Retorno = obj.ProblemaIntegracao
                   };
        }

        #endregion
    }
}
