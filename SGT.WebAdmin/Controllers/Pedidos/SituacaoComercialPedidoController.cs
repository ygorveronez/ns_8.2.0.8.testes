using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pedidos/SituacaoComercialPedido")]
    public class SituacaoComercialPedidoController : BaseController
    {
		#region Construtores

		public SituacaoComercialPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido situacaoComercialPedido = new Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido();

                PreencherSituacaoComercialPedido(situacaoComercialPedido);

                unitOfWork.Start();

                new Repositorio.Embarcador.Pedidos.SituacaoComercialPedido(unitOfWork).Inserir(situacaoComercialPedido);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.SituacaoComercialPedido repositorioSituacaoComercialPedido = new Repositorio.Embarcador.Pedidos.SituacaoComercialPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido situacaoComercialPedido = repositorioSituacaoComercialPedido.BuscarPorCodigo(codigo, false);

                if (situacaoComercialPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherSituacaoComercialPedido(situacaoComercialPedido);

                unitOfWork.Start();

                repositorioSituacaoComercialPedido.Atualizar(situacaoComercialPedido);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.SituacaoComercialPedido repositorioSituacaoComercialPedido = new Repositorio.Embarcador.Pedidos.SituacaoComercialPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido situacaoComercialPedido = repositorioSituacaoComercialPedido.BuscarPorCodigo(codigo);

                if (situacaoComercialPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    situacaoComercialPedido.Codigo,
                    situacaoComercialPedido.Descricao,
                    situacaoComercialPedido.CodigoIntegracao,
                    situacaoComercialPedido.Observacao,
                    situacaoComercialPedido.Cor,
                    situacaoComercialPedido.BloqueiaPedido
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.SituacaoComercialPedido repositorioSituacaoComercialPedido = new Repositorio.Embarcador.Pedidos.SituacaoComercialPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido situacaoComercialPedido = repositorioSituacaoComercialPedido.BuscarPorCodigo(codigo);

                if (situacaoComercialPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorioSituacaoComercialPedido.Deletar(situacaoComercialPedido);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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
                var grid = ObterGridPesquisa();

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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherSituacaoComercialPedido(Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido situacaoComercialPedido)
        {
            situacaoComercialPedido.Descricao = Request.GetNullableStringParam("Descricao") ?? throw new ControllerException(Localization.Resources.Consultas.CategoriaPessoa.DescricaoObrigatoria);
            situacaoComercialPedido.Observacao = Request.GetNullableStringParam("Observacao");
            situacaoComercialPedido.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            situacaoComercialPedido.Cor = Request.GetStringParam("Cor");
            situacaoComercialPedido.BloqueiaPedido = Request.GetBoolParam("BloqueiaPedido");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaSituacaoComercialPedido filtroPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Bloqueia Pedido", "BloqueiaPedido", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Cor", false);


                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pedidos.SituacaoComercialPedido repositorioSituacaoComercialPedido = new Repositorio.Embarcador.Pedidos.SituacaoComercialPedido(unitOfWork);
                int totalRegistros = repositorioSituacaoComercialPedido.ContarConsulta(filtroPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido> listaSituacaoComercialPedido = totalRegistros > 0 ? repositorioSituacaoComercialPedido.Consultar(filtroPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido>();

                var listaSituacaoComercialPedidoRetornar = (
                    from situacaoComercialPedido in listaSituacaoComercialPedido
                    select new
                    {
                        situacaoComercialPedido.Codigo,
                        situacaoComercialPedido.Descricao,
                        situacaoComercialPedido.Cor,
                        situacaoComercialPedido.CodigoIntegracao,
                        BloqueiaPedido = situacaoComercialPedido.BloqueiaPedido ? "Sim" : "Não"
                    }
                ).ToList();

                grid.AdicionaRows(listaSituacaoComercialPedidoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaSituacaoComercialPedido ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaSituacaoComercialPedido
            {
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Descricao = Request.GetStringParam("Descricao"),
                BloqueiaPedido = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao>("BloqueiaPedido", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Todos)
            };
        }

        #endregion
    }
}
