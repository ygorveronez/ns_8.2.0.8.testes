using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga
{
    [CustomAuthorize("Cargas/MotivoAvaliacao")]
    public class MotivoAvaliacaoController : BaseController
    {
		#region Construtores

		public MotivoAvaliacaoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.MotivoAvaliacao repositorioMotivoAvaliacao = new Repositorio.Embarcador.Cargas.MotivoAvaliacao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao motivoAvaliacao = new Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao();

                PreencherEntidade(motivoAvaliacao, unitOfWork);

                unitOfWork.Start();

                repositorioMotivoAvaliacao.Inserir(motivoAvaliacao, Auditado);

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

                Repositorio.Embarcador.Cargas.MotivoAvaliacao repositorioMotivoAvaliacao = new Repositorio.Embarcador.Cargas.MotivoAvaliacao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao motivoAvaliacao = repositorioMotivoAvaliacao.BuscarPorCodigo(codigo, true);

                if (motivoAvaliacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(motivoAvaliacao, unitOfWork);

                unitOfWork.Start();

                repositorioMotivoAvaliacao.Atualizar(motivoAvaliacao, Auditado);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.MotivoAvaliacao repositorioMotivoAvaliacao = new Repositorio.Embarcador.Cargas.MotivoAvaliacao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao motivoAvaliacao = repositorioMotivoAvaliacao.BuscarPorCodigo(codigo, true);

                if (motivoAvaliacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoAvaliacao.Codigo,
                    motivoAvaliacao.Descricao,
                    motivoAvaliacao.GerarAtendimentoAutomaticoQuandoAvalicaoForUmaEstrela,
                    Motivo = motivoAvaliacao.MotivoChamado != null ? new { motivoAvaliacao.MotivoChamado.Codigo, motivoAvaliacao.MotivoChamado.Descricao } : null,
                    Situacao = motivoAvaliacao.Ativo,
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

                Repositorio.Embarcador.Cargas.MotivoAvaliacao repositorioMotivoAvaliacao = new Repositorio.Embarcador.Cargas.MotivoAvaliacao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao motivoAvaliacao = repositorioMotivoAvaliacao.BuscarPorCodigo(codigo, true);

                if (motivoAvaliacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorioMotivoAvaliacao.Deletar(motivoAvaliacao, Auditado);

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

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao motivoAvaliacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

            string descricao = Request.Params("Descricao");
            bool ativo = Request.GetBoolParam("Situacao");
            bool gerarAtendimentoAutomaticoQuandoAvalicaoForUmaEstrela = Request.GetBoolParam("GerarAtendimentoAutomaticoQuandoAvalicaoForUmaEstrela");
            int codigoMotivo = Request.GetIntParam("Motivo");

            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repositorioMotivoChamado.BuscarPorCodigo(codigoMotivo);

            motivoAvaliacao.Ativo = ativo;
            motivoAvaliacao.Descricao = descricao;
            motivoAvaliacao.MotivoChamado = motivoChamado;
            motivoAvaliacao.GerarAtendimentoAutomaticoQuandoAvalicaoForUmaEstrela = gerarAtendimentoAutomaticoQuandoAvalicaoForUmaEstrela;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Cargas.MotivoAvaliacao repositorioMotivoAvaliacao = new Repositorio.Embarcador.Cargas.MotivoAvaliacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> listaMotivoAvaliacao = repositorioMotivoAvaliacao.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repositorioMotivoAvaliacao.ContarConsulta(descricao, status);

                var retorno = (from motivo in listaMotivoAvaliacao
                               select new
                               {
                                   motivo.Codigo,
                                   motivo.Descricao,
                                   motivo.DescricaoAtivo
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
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
