using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Cargas/GrupoModeloVeicular")]
    public class GrupoModeloVeicularController : BaseController
    {
		#region Construtores

		public GrupoModeloVeicularController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var descricao = Request.GetStringParam("Descricao");
                var status = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("Descricao").Nome(Localization.Resources.Gerais.Geral.Descricao).Tamanho(65).Align(Models.Grid.Align.left);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.Prop("DescricaoAtivo").Nome(Localization.Resources.Consultas.GrupoModeloVeicular.Status).Tamanho(20).Align(Models.Grid.Align.left); ;

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var repositorioGrupoModeloVeicular = new Repositorio.Embarcador.Cargas.GrupoModeloVeicular(unitOfWork);
                var listaGrupo = repositorioGrupoModeloVeicular.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                var totalRegistros = repositorioGrupoModeloVeicular.ContarConsulta(descricao, status);
                var listaRetornar = (
                    from grupo in listaGrupo
                    select new
                    {
                        grupo.Codigo,
                        grupo.Descricao,
                        grupo.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.GrupoModeloVeicular repGrupoModeloVeicular = new Repositorio.Embarcador.Cargas.GrupoModeloVeicular(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular grupoModeloVeicular = repGrupoModeloVeicular.BuscarPorCodigo(codigo);

                // Valida
                if (grupoModeloVeicular == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    grupoModeloVeicular.Codigo,
                    grupoModeloVeicular.Descricao,
                    grupoModeloVeicular.Ativo
                };

                // Retorna informacoes
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
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.GrupoModeloVeicular repGrupoModeloVeicular = new Repositorio.Embarcador.Cargas.GrupoModeloVeicular(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular grupoModeloVeicular = new Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular();

                // Preenche entidade com dados
                PreencheEntidade(ref grupoModeloVeicular, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(grupoModeloVeicular, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                unitOfWork.Start();
                repGrupoModeloVeicular.Inserir(grupoModeloVeicular, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
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
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.GrupoModeloVeicular repGrupoModeloVeicular = new Repositorio.Embarcador.Cargas.GrupoModeloVeicular(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular grupoModeloVeicular = repGrupoModeloVeicular.BuscarPorCodigo(codigo, true);

                // Valida
                if (grupoModeloVeicular == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref grupoModeloVeicular, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(grupoModeloVeicular, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                unitOfWork.Start();
                repGrupoModeloVeicular.Atualizar(grupoModeloVeicular, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
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
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.GrupoModeloVeicular repGrupoModeloVeicular = new Repositorio.Embarcador.Cargas.GrupoModeloVeicular(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular grupoModeloVeicular = repGrupoModeloVeicular.BuscarPorCodigo(codigo);

                // Valida
                if (grupoModeloVeicular == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                unitOfWork.Start();
                repGrupoModeloVeicular.Deletar(grupoModeloVeicular, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Relacional")
                return "Relacional.Codigo";

            return propriedadeOrdenar;
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular grupoModeloVeicular, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao") ?? string.Empty;
            bool.TryParse(Request.Params("Ativo"), out bool ativo);

            // Vincula dados
            grupoModeloVeicular.Descricao = descricao;
            grupoModeloVeicular.Ativo = ativo;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular grupoModeloVeicular, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            msgErro = "";

            if (string.IsNullOrEmpty(grupoModeloVeicular.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            return true;
        }

        #endregion
    }
}
