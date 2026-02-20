using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ComprovanteCarga
{
    [CustomAuthorize("Cargas/TipoComprovante")]
    public class TipoComprovanteController : BaseController
    {
		#region Construtores

		public TipoComprovanteController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaTipoComprovante filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante repositorioTipoComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante(unitOfWork);
                int totalRegistro = repositorioTipoComprovante.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante> motivosIrregularidades =
                    (totalRegistro > 0) ?
                    repositorioTipoComprovante.Consultar(filtrosPesquisa, parametrosConsulta) :
                    new List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante>();

                var tiposComprovantesRetornar = (
                    from motivoIrregularidade in motivosIrregularidades
                    select new
                    {
                        Codigo = motivoIrregularidade.Codigo,
                        Descricao = motivoIrregularidade.Descricao,
                        Observacao = motivoIrregularidade.Observacao,
                        Situacao = motivoIrregularidade.Ativa ? SituacaoAtivoPesquisa.Ativo.ObterDescricao() : SituacaoAtivoPesquisa.Inativo.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(tiposComprovantesRetornar);
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
                Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante repositorioTipoComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante TipoComprovante = repositorioTipoComprovante.BuscarPorCodigo(codigo);

                if (TipoComprovante == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    TipoComprovante.Codigo,
                    TipoComprovante.Descricao,
                    TipoComprovante.Observacao,
                    Situacao = TipoComprovante.Ativa
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

                Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante repositorioTipoComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante TipoComprovante = new Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante();

                PreencherTipoComprovante(TipoComprovante);

                repositorioTipoComprovante.Inserir(TipoComprovante, Auditado);

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
                Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante repositorioTipoComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante TipoComprovante = repositorioTipoComprovante.BuscarPorCodigo(codigo);

                if (TipoComprovante == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherTipoComprovante(TipoComprovante);

                repositorioTipoComprovante.Atualizar(TipoComprovante, Auditado);

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
                Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante repositorioTipoComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante TipoComprovante = repositorioTipoComprovante.BuscarPorCodigo(codigo);

                if (TipoComprovante == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioTipoComprovante.Deletar(TipoComprovante, Auditado);

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

        private string ObterPropriedadeOrdenar(string prop)
        {
            return prop;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaTipoComprovante ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaTipoComprovante()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao")
            };
        }

        private void PreencherTipoComprovante(Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante TipoComprovante)
        {
            TipoComprovante.Descricao = Request.GetStringParam("Descricao");
            TipoComprovante.Ativa = Request.GetBoolParam("Situacao");
            TipoComprovante.Observacao = Request.GetStringParam("Observacao");
        }

        #endregion
    }
}
