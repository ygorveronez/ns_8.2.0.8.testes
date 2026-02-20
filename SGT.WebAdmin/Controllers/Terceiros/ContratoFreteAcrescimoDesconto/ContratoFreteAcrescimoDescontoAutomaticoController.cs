using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros
{
    [CustomAuthorize("Terceiros/ContratoFreteAcrescimoDescontoAutomatico")]
    public class ContratoFreteAcrescimoDescontoAutomaticoController : BaseController
    {
		#region Construtores

		public ContratoFreteAcrescimoDescontoAutomaticoController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo de Valor", "TipoValor", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo de Cálculo", "TipoCalculo", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Observações", "Observacoes", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAutomatico filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico repositorioContratoFreteAcrescimoDescontoAutomatico = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico(unitOfWork);
                int totalRegistro = repositorioContratoFreteAcrescimoDescontoAutomatico.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico> acrescimosDescontosAutomaticos = (totalRegistro > 0) ? repositorioContratoFreteAcrescimoDescontoAutomatico.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico>();

                var acrescimosDescontosAutomaticosRetornar = (
                    from acrescimoDescontoAutomatico in acrescimosDescontosAutomaticos
                    select new
                    {
                        acrescimoDescontoAutomatico.Codigo,
                        acrescimoDescontoAutomatico.Descricao,
                        Justificativa = acrescimoDescontoAutomatico.Justificativa?.Descricao ?? string.Empty,
                        acrescimoDescontoAutomatico.Valor,
                        TipoValor = acrescimoDescontoAutomatico.TipoValor.ObterDescricao(),
                        TipoCalculo = acrescimoDescontoAutomatico.TipoCalculo?.ObterDescricao() ?? string.Empty,
                        Observacoes = acrescimoDescontoAutomatico.Observacoes ?? string.Empty
                    }
                ).ToList();

                grid.AdicionaRows(acrescimosDescontosAutomaticosRetornar);
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
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico repositorioContratoFreteAcrescimoDescontoAutomatico = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico ContratoFreteADA = repositorioContratoFreteAcrescimoDescontoAutomatico.BuscarPorCodigo(codigo);

                if (ContratoFreteADA == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    ContratoFreteADA.Codigo,
                    ContratoFreteADA.Descricao,
                    Justificativa = new { ContratoFreteADA.Justificativa?.Codigo, ContratoFreteADA.Justificativa?.Descricao } ?? null,
                    ContratoFreteADA.Valor,
                    ContratoFreteADA.TipoValor,
                    TipoCalculo = ContratoFreteADA.TipoCalculo ?? null,
                    Observacoes = ContratoFreteADA.Observacoes ??  string.Empty
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

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico repositorioContratoFreteAcrescimoDescontoAutomatico = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico ContratoFreteADA = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico();

                PreencherContratoFreteAcrescimoDescontoAutomatico(ContratoFreteADA, unitOfWork);

                repositorioContratoFreteAcrescimoDescontoAutomatico.Inserir(ContratoFreteADA, Auditado);

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
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico repositorioContratoFreteAcrescimoDescontoAutomatico = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico ContratoFreteADA = repositorioContratoFreteAcrescimoDescontoAutomatico.BuscarPorCodigo(codigo);

                if (ContratoFreteADA == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherContratoFreteAcrescimoDescontoAutomatico(ContratoFreteADA, unitOfWork);

                repositorioContratoFreteAcrescimoDescontoAutomatico.Atualizar(ContratoFreteADA, Auditado);

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
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico repositorioContratoFreteAcrescimoDescontoAutomatico = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico ContratoFreteADA = repositorioContratoFreteAcrescimoDescontoAutomatico.BuscarPorCodigo(codigo);

                if (ContratoFreteADA == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioContratoFreteAcrescimoDescontoAutomatico.Deletar(ContratoFreteADA, Auditado);

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

        private Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAutomatico ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAutomatico()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Justificativa = Request.GetIntParam("Justificativa"),
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricatoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void PreencherContratoFreteAcrescimoDescontoAutomatico(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico ContratoFreteADA, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.Justificativa repositorioJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
            int codigoJustificativa = Request.GetIntParam("Justificativa");

            ContratoFreteADA.Codigo = Request.GetIntParam("Codigo");
            ContratoFreteADA.Descricao = Request.GetStringParam("Descricao");
            ContratoFreteADA.Justificativa = (codigoJustificativa > 0) ? repositorioJustificativa.BuscarPorCodigo(codigoJustificativa) : null;
            ContratoFreteADA.Valor = Request.GetDecimalParam("Valor");
            ContratoFreteADA.TipoValor = Request.GetEnumParam<TipoValorContratoFreteADA>("TipoValor");
            ContratoFreteADA.TipoCalculo = Request.GetEnumParam<TipoCalculoContratoFreteADA>("TipoCalculo");
            ContratoFreteADA.Observacoes = Request.GetStringParam("Observacoes");
        }

        #endregion
    }
}
