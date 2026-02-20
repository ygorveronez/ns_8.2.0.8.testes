using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/MotivoRejeicaoAlteracaoPedido")]
    public class MotivoRejeicaoAlteracaoPedidoController : BaseController
    {
		#region Construtores

		public MotivoRejeicaoAlteracaoPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido motivoRejeicaoAlteracaoPedido = new Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido();

                PreencherMotivoRejeicaoAlteracaoPedido(motivoRejeicaoAlteracaoPedido);

                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido repositorio = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido(unitOfWork);

                repositorio.Inserir(motivoRejeicaoAlteracaoPedido, Auditado);

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
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido repositorio = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido motivoRejeicaoAlteracaoPedido = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoRejeicaoAlteracaoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMotivoRejeicaoAlteracaoPedido(motivoRejeicaoAlteracaoPedido);

                unitOfWork.Start();

                repositorio.Atualizar(motivoRejeicaoAlteracaoPedido, Auditado);

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
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido repositorio = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido motivoRejeicaoAlteracaoPedido = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoRejeicaoAlteracaoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoRejeicaoAlteracaoPedido.Codigo,
                    motivoRejeicaoAlteracaoPedido.Descricao,
                    Status = motivoRejeicaoAlteracaoPedido.Ativo,
                    motivoRejeicaoAlteracaoPedido.Tipo,
                    motivoRejeicaoAlteracaoPedido.Observacao
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
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido repositorio = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido motivoRejeicaoAlteracaoPedido = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoRejeicaoAlteracaoPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(motivoRejeicaoAlteracaoPedido, Auditado);

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

        #endregion

        #region Métodos Privados

        private void PreencherMotivoRejeicaoAlteracaoPedido(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido motivoRejeicaoAlteracaoPedido)
        {
            var descricao = Request.GetStringParam("Descricao");
            var observacao = Request.GetStringParam("Observacao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ControllerException("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new ControllerException("Descrição não pode passar de 200 caracteres.");

            if (observacao.Length > 2000)
                throw new ControllerException("Observação não pode passar de 2000 caracteres.");

            motivoRejeicaoAlteracaoPedido.Ativo = Request.GetBoolParam("Status");
            motivoRejeicaoAlteracaoPedido.Descricao = descricao;
            motivoRejeicaoAlteracaoPedido.Observacao = observacao;
            motivoRejeicaoAlteracaoPedido.Tipo = Request.GetEnumParam<TipoMotivoRejeicaoAlteracaoPedido>("Tipo");
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaMotivoRejeicaoAlteracaoPedido ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaMotivoRejeicaoAlteracaoPedido()
            {
                Descricao = Request.GetStringParam("Descricao"),
                SituacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo),
                Tipo = Request.GetEnumParam("Tipo", TipoMotivoRejeicaoAlteracaoPedido.Todos)
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaMotivoRejeicaoAlteracaoPedido filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 15, Models.Grid.Align.left, true);

                if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido repositorio = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido> listaMotivoRejeicaoAlteracaoPedido = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido>();

                var listaMotivoRejeicaoAlteracaoPedidoRetornar = (
                    from motivo in listaMotivoRejeicaoAlteracaoPedido
                    select new
                    {
                        motivo.Codigo,
                        motivo.Descricao,
                        motivo.DescricaoAtivo,
                        Tipo = motivo.Tipo.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoRejeicaoAlteracaoPedidoRetornar);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
