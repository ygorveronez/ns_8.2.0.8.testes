using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Containers
{
    [CustomAuthorize("Containers/ConferenciaContainer")]
    public class ConferenciaContainerController : BaseController
    {
		#region Construtores

		public ConferenciaContainerController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Aprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoConferenciaContainer = Request.GetIntParam("Codigo");
                string observacao = Request.GetStringParam("Observacao");
                Servicos.Embarcador.Pedido.ConferenciaContainer servicoConferenciaContainer = new Servicos.Embarcador.Pedido.ConferenciaContainer(unitOfWork, Auditado);

                servicoConferenciaContainer.Aprovar(codigoConferenciaContainer, observacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a conferência do container.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoConferenciaContainer = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.ConferenciaContainer repositorioConferenciaContainer = new Repositorio.Embarcador.Pedidos.ConferenciaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer conferenciaContainer = repositorioConferenciaContainer.BuscarPorCodigo(codigoConferenciaContainer, auditavel: false);

                return new JsonpResult(new
                {
                    conferenciaContainer.Codigo
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter a conferência do container.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaConferenciaContainer ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaConferenciaContainer()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                Situacao = Request.GetNullableEnumParam<SituacaoConferenciaContainer>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Número da Carga", "CodigoCargaEmbarcador", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Número do Container", "NumeroContainer", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "SituacaoDescricao", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaConferenciaContainer filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pedidos.ConferenciaContainer repositorioConferenciaContainer = new Repositorio.Embarcador.Pedidos.ConferenciaContainer(unitOfWork);
                int totalRegistros = repositorioConferenciaContainer.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer> conferenciasContainer = (totalRegistros > 0) ? repositorioConferenciaContainer.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer>();

                var conferenciasContainerRetornar = (
                    from conferenciaContainer in conferenciasContainer
                    select new
                    {
                        conferenciaContainer.Codigo,
                        conferenciaContainer.Situacao,
                        conferenciaContainer.Carga.CodigoCargaEmbarcador,
                        NumeroContainer = conferenciaContainer.Container?.Numero ?? "",
                        SituacaoDescricao = conferenciaContainer.Situacao.ObterDescricao(),
                        conferenciaContainer.Observacao,
                        DT_RowColor = conferenciaContainer.Situacao.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(conferenciasContainerRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        #endregion Métodos Privados
    }
}
