using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/MotivoRecusaCancelamento")]
    public class MotivoRecusaCancelamentoController : BaseController
    {
		#region Construtores

		public MotivoRecusaCancelamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
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
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento repMotivoRecusaCancelamento = new Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento motivoRecusaCancelamento = repMotivoRecusaCancelamento.BuscarPorCodigo(codigo);

                if (motivoRecusaCancelamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    motivoRecusaCancelamento.Codigo,
                    motivoRecusaCancelamento.Descricao,
                    motivoRecusaCancelamento.Status,
                    motivoRecusaCancelamento.TipoMotivoRecusaCancelamento
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento repMotivoRecusaCancelamento = new Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento motivoRecusaCancelamento = new Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento();

                PreencheEntidade(motivoRecusaCancelamento, unitOfWork);

                repMotivoRecusaCancelamento.Inserir(motivoRecusaCancelamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento repMotivoRecusaCancelamento = new Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento motivoRecusaCancelamento = repMotivoRecusaCancelamento.BuscarPorCodigo(codigo, true);

                if (motivoRecusaCancelamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencheEntidade(motivoRecusaCancelamento, unitOfWork);

                repMotivoRecusaCancelamento.Atualizar(motivoRecusaCancelamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento repMotivoRecusaCancelamento = new Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento motivoRecusaCancelamento = repMotivoRecusaCancelamento.BuscarPorCodigo(codigo);

                if (motivoRecusaCancelamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repMotivoRecusaCancelamento.Deletar(motivoRecusaCancelamento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
                }
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
            grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
            if (Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo) == SituacaoAtivoPesquisa.Todos)
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 25, Models.Grid.Align.left, true);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento repMotivoRecusaCancelamento = new Repositorio.Embarcador.Chamados.MotivoRecusaCancelamento(unitOfWork);

            SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);

            TipoMotivoRecusaCancelamento tipoMotivoRecusaCancelamento = Request.GetEnumParam<TipoMotivoRecusaCancelamento>("TipoMotivoRecusaCancelamento");

            string descricao = Request.GetStringParam("Descricao");

            List<Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento> listaGrid = repMotivoRecusaCancelamento.Consultar(descricao, status, tipoMotivoRecusaCancelamento, TipoServicoMultisoftware, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMotivoRecusaCancelamento.ContarConsulta(descricao, status, tipoMotivoRecusaCancelamento, TipoServicoMultisoftware);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            DescricaoStatus = obj.DescricaoStatus,
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento motivoRecusaCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            motivoRecusaCancelamento.Descricao = Request.GetStringParam("Descricao");
            motivoRecusaCancelamento.TipoMotivoRecusaCancelamento = Request.GetEnumParam<TipoMotivoRecusaCancelamento>("TipoMotivoRecusaCancelamento");
            motivoRecusaCancelamento.Status = Request.GetBoolParam("Status");
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "DescricaoStatus") propOrdenar = "Status";
        }
        #endregion
    }
}
