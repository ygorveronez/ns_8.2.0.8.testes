using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.Sinistro
{
    [CustomAuthorize("Frota/TipoSinistro")]
    public class TipoSinistroController : BaseController
    {
		#region Construtores

		public TipoSinistroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "Status", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoSinistro filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frota.TipoSinistro repTipoSinistro = new Repositorio.Embarcador.Frota.TipoSinistro(unitOfWork);
                int totalRegistro = repTipoSinistro.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro> tiposSinistros = (totalRegistro > 0) ? repTipoSinistro.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro>();

                var tiposSinistrosRetornar = (
                    from tipoSinistro in tiposSinistros
                    select new
                    {
                        tipoSinistro.Codigo,
                        tipoSinistro.Descricao,
                        Status = tipoSinistro.Status.ObterDescricaoAtivo(),
                    }
                ).ToList();

                grid.AdicionaRows(tiposSinistrosRetornar);
                grid.setarQuantidadeTotal(totalRegistro);

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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.TipoSinistro repTipoSinistro = new Repositorio.Embarcador.Frota.TipoSinistro(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro tipoSinistro = repTipoSinistro.BuscarPorCodigo(codigo);

                if (tipoSinistro == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    tipoSinistro.Codigo,
                    tipoSinistro.Descricao,
                    tipoSinistro.Status
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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

                Repositorio.Embarcador.Frota.TipoSinistro repTipoSinistro = new Repositorio.Embarcador.Frota.TipoSinistro(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro tipoSinistro = new Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro();

                PreencherTipoSinistro(tipoSinistro, unitOfWork);
                ValidarTipoSinistro(tipoSinistro, unitOfWork);

                repTipoSinistro.Inserir(tipoSinistro, Auditado);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.TipoSinistro repTipoSinistro = new Repositorio.Embarcador.Frota.TipoSinistro(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro tipoSinistro = repTipoSinistro.BuscarPorCodigo(codigo);

                if (tipoSinistro == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherTipoSinistro(tipoSinistro, unitOfWork);
                ValidarTipoSinistro(tipoSinistro, unitOfWork);

                repTipoSinistro.Atualizar(tipoSinistro, Auditado);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.TipoSinistro repTipoSinistro = new Repositorio.Embarcador.Frota.TipoSinistro(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro tipoSinistro = repTipoSinistro.BuscarPorCodigo(codigo);

                if (tipoSinistro == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repTipoSinistro.Deletar(tipoSinistro, Auditado);

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

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoSinistro ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoSinistro()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Status = Request.GetEnumParam<SituacaoAtivoPesquisa>("Status")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricatoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void PreencherTipoSinistro(Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro tipoSinistro, Repositorio.UnitOfWork unitOfWork)
        {
            tipoSinistro.Descricao = Request.GetStringParam("Descricao");
            tipoSinistro.Status = Request.GetBoolParam("Status");

        }

        private void ValidarTipoSinistro(Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro tipoSinistro, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrEmpty(tipoSinistro.Descricao))
                throw new ControllerException("A Descrição deve ser informada");
        }

        #endregion
    }

}
