using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    public class MotivoAtrasoCarregamentoController : BaseController
    {
		#region Construtores

		public MotivoAtrasoCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento repositorioMotivoAtrasoCarregamento = new Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento motivoAtrasoCarregamento = new Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento ();

                PreencherMotivoAtrasoCarregamento(motivoAtrasoCarregamento);

                unitOfWork.Start();
                repositorioMotivoAtrasoCarregamento.Inserir(motivoAtrasoCarregamento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento repositorioMotivoAtrasoCarregamento = new Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento motivoAtrasoCarregamento = repositorioMotivoAtrasoCarregamento.BuscarPorCodigo(codigo, false);

                if (motivoAtrasoCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMotivoAtrasoCarregamento(motivoAtrasoCarregamento);

                unitOfWork.Start();
                repositorioMotivoAtrasoCarregamento.Atualizar(motivoAtrasoCarregamento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento repositorioMotivoAtrasoCarregamento = new Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento motivoAtrasoCarregamento = repositorioMotivoAtrasoCarregamento.BuscarPorCodigo(codigo, false);

                if (motivoAtrasoCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoAtrasoCarregamento.Codigo,
                    motivoAtrasoCarregamento.Descricao,
                    motivoAtrasoCarregamento.Status
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
                Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento repositorioMotivoAtrasoCarregamento = new Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento motivoAtrasoCarregamento = repositorioMotivoAtrasoCarregamento.BuscarPorCodigo(codigo, false);

                if (motivoAtrasoCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                repositorioMotivoAtrasoCarregamento.Deletar(motivoAtrasoCarregamento, Auditado);

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

        #region Metodos Privados
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento repositorioMotivoAtrasoCarregamento = new Repositorio.Embarcador.Logistica.MotivoAtrasoCarregamento(unitOfWork);

                string descricao = Request.GetStringParam("Descricao");
                var status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                var grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                var listaMotivoPunicao = repositorioMotivoAtrasoCarregamento.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                var totalRegistros = repositorioMotivoAtrasoCarregamento.ContarConsulta(descricao, status);

                var listaMotivoPunicaoRetornar = (
                    from motivo in listaMotivoPunicao
                    select new
                    {
                        motivo.Codigo,
                        motivo.Descricao,
                        motivo.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoPunicaoRetornar);
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

       private void PreencherMotivoAtrasoCarregamento(Dominio.Entidades.Embarcador.Logistica.MotivoAtrasoCarregamento motivoAtraso)
        {
            motivoAtraso.Descricao = Request.GetStringParam("Descricao");
            motivoAtraso.Status = Request.GetBoolParam("Status");
        }
        #endregion
    }
}
