using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Justificativa
{
    public class EncerramentoManualViagemController : BaseController
    {
		#region Construtores

		public EncerramentoManualViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Justificativas/EncerramentoManualViagem")]
        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Justificativas.EncerramentoManualViagem repositorioMoedaEncerramentoManualViagem = new Repositorio.Embarcador.Justificativas.EncerramentoManualViagem(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Justificativas.FiltroPesquisaEncerramentoManualViagem filtrosPesquisa = ObterFiltrosPesquisaMoeda();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição Moeda", "Descricao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Observação", "Observacao", 40, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioMoedaEncerramentoManualViagem.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem> listaMoeda = totalRegistros > 0 ? repositorioMoedaEncerramentoManualViagem.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem>();

                grid.AdicionaRows((
                    from o in listaMoeda
                    select new
                    {
                        o.Codigo,
                        o.Descricao,
                        o.Observacao,
                        Situacao = o.Situacao.ObterDescricaoAtivo()
                    }).ToList()
                );

                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Justificativas.EncerramentoManualViagem repositorioMoedaEncerramentoManualViagem = new Repositorio.Embarcador.Justificativas.EncerramentoManualViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem encerramentoManualViagem = repositorioMoedaEncerramentoManualViagem.BuscarPorCodigo(codigo);

                if (encerramentoManualViagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(encerramentoManualViagem, unitOfWork);

                repositorioMoedaEncerramentoManualViagem.Atualizar(encerramentoManualViagem, Auditado);
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

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Justificativas.EncerramentoManualViagem repositorioMoedaEncerramentoManualViagem = new Repositorio.Embarcador.Justificativas.EncerramentoManualViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem encerramentoManualViagem = new Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem();

                PreencherEntidade(encerramentoManualViagem, unitOfWork);

                repositorioMoedaEncerramentoManualViagem.Inserir(encerramentoManualViagem, Auditado);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Justificativas.EncerramentoManualViagem repositorioMoedaEncerramentoManualViagem = new Repositorio.Embarcador.Justificativas.EncerramentoManualViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem encerramentoManualViagem = repositorioMoedaEncerramentoManualViagem.BuscarPorCodigo(codigo);

                if (encerramentoManualViagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioMoedaEncerramentoManualViagem.Deletar(encerramentoManualViagem, Auditado);

                unitOfWork.CommitChanges();

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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Justificativas.EncerramentoManualViagem repositorioMoedaEncerramentoManualViagem = new Repositorio.Embarcador.Justificativas.EncerramentoManualViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem encerramentoManualViagem = repositorioMoedaEncerramentoManualViagem.BuscarPorCodigo(codigo);

                if (encerramentoManualViagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                dynamic dynEncerramentoViagemManual = new
                {
                    encerramentoManualViagem.Codigo,
                    encerramentoManualViagem.Descricao,
                    encerramentoManualViagem.Observacao,
                    Situacao = encerramentoManualViagem.Situacao.ObterDescricaoAtivo()
                };

                return new JsonpResult(dynEncerramentoViagemManual);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metódos Privados

        private Dominio.ObjetosDeValor.Embarcador.Justificativas.FiltroPesquisaEncerramentoManualViagem ObterFiltrosPesquisaMoeda()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Justificativas.FiltroPesquisaEncerramentoManualViagem()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetBoolParam("Situacao"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem encerramentoManualViagem, Repositorio.UnitOfWork unitOfWork)
        {
            encerramentoManualViagem.Descricao = Request.GetStringParam("Descricao");
            encerramentoManualViagem.Situacao = Request.GetBoolParam("Situacao");
            encerramentoManualViagem.Observacao = Request.GetStringParam("Observacao");
        }

        #endregion
    }
}
