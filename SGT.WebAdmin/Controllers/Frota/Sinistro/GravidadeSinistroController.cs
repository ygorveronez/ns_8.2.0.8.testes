using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.Sinistro
{
    [CustomAuthorize("Frota/GravidadeSinistro")]
    public class GravidadeSinistroController : BaseController
    {
        #region Construtores

        public GravidadeSinistroController(Conexao conexao) : base(conexao) { }

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

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGravidadeSinistro filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frota.GravidadeSinistro repGravidadeSinistro = new Repositorio.Embarcador.Frota.GravidadeSinistro(unitOfWork);
                int totalRegistro = repGravidadeSinistro.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro> GravidadeSinistros = (totalRegistro > 0) ? repGravidadeSinistro.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro>();

                var gravidadeSinistrosRetornar = (
                    from gravidadeSinistro in GravidadeSinistros
                    select new
                    {
                        gravidadeSinistro.Codigo,
                        gravidadeSinistro.Descricao,
                        Status = gravidadeSinistro.Status.ObterDescricaoAtivo()
                    }
                ).ToList();

                grid.AdicionaRows(gravidadeSinistrosRetornar);
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
                Repositorio.Embarcador.Frota.GravidadeSinistro repGravidadeSinistro = new Repositorio.Embarcador.Frota.GravidadeSinistro(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro gravidadeSinistro = repGravidadeSinistro.BuscarPorCodigo(codigo);

                if (gravidadeSinistro == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    gravidadeSinistro.Codigo,
                    gravidadeSinistro.Descricao,
                    gravidadeSinistro.Status
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

                Repositorio.Embarcador.Frota.GravidadeSinistro repGravidadeSinistro = new Repositorio.Embarcador.Frota.GravidadeSinistro(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro gravidadeSinistro = new Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro();

                PreencherGravidadeSinistro(gravidadeSinistro, unitOfWork);
                ValidarGravidadeSinistro(gravidadeSinistro, unitOfWork);

                repGravidadeSinistro.Inserir(gravidadeSinistro, Auditado);

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
                Repositorio.Embarcador.Frota.GravidadeSinistro repGravidadeSinistro = new Repositorio.Embarcador.Frota.GravidadeSinistro(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro gravidadeSinistro = repGravidadeSinistro.BuscarPorCodigo(codigo);

                if (gravidadeSinistro == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherGravidadeSinistro(gravidadeSinistro, unitOfWork);
                ValidarGravidadeSinistro(gravidadeSinistro, unitOfWork);

                repGravidadeSinistro.Atualizar(gravidadeSinistro, Auditado);

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
                Repositorio.Embarcador.Frota.GravidadeSinistro repGravidadeSinistro = new Repositorio.Embarcador.Frota.GravidadeSinistro(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro gravidadeSinistro = repGravidadeSinistro.BuscarPorCodigo(codigo);

                if (gravidadeSinistro == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repGravidadeSinistro.Deletar(gravidadeSinistro, Auditado);

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

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGravidadeSinistro ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGravidadeSinistro()
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

        private void PreencherGravidadeSinistro(Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro gravidadeSinistro, Repositorio.UnitOfWork unitOfWork)
        {
            gravidadeSinistro.Descricao = Request.GetStringParam("Descricao");
            gravidadeSinistro.Status = Request.GetBoolParam("Status");

        }

        private void ValidarGravidadeSinistro(Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro gravidadeSinistro, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrEmpty(gravidadeSinistro.Descricao))
                throw new ControllerException("A Descrição deve ser informada");
        }

        #endregion

    }
}