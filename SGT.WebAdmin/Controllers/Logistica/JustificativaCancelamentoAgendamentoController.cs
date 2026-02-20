using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/JustificativaCancelamentoAgendamento")]
    public class JustificativaCancelamentoAgendamentoController : BaseController
    {
		#region Construtores

		public JustificativaCancelamentoAgendamentoController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 50, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJustificativaCancelamentoAgendamento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.JustificativaCancelamentoAgendamento repositorioJustificativaCancelamentoAgendamento = new Repositorio.Embarcador.Logistica.JustificativaCancelamentoAgendamento(unitOfWork);
                int totalRegistro = repositorioJustificativaCancelamentoAgendamento.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento> JustificativasCancelamentoAgendamento = totalRegistro > 0 ? repositorioJustificativaCancelamentoAgendamento.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento>();

                var JustificativasCancelamentoAgendamentoRetornar = (
                    from JustificativaCancelamentoAgendamento in JustificativasCancelamentoAgendamento
                    select new
                    {
                        JustificativaCancelamentoAgendamento.Codigo,
                        JustificativaCancelamentoAgendamento.Descricao,
                        Situacao = JustificativaCancelamentoAgendamento.Ativa ? SituacaoAtivaPesquisa.Ativa.ObterDescricao() : SituacaoAtivaPesquisa.Inativa.ObterDescricao(),
                        JustificativaCancelamentoAgendamento.Observacao
                    }
                ).ToList();

                grid.AdicionaRows(JustificativasCancelamentoAgendamentoRetornar);
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
                Repositorio.Embarcador.Logistica.JustificativaCancelamentoAgendamento repositorioJustificativaCancelamentoAgendamento = new Repositorio.Embarcador.Logistica.JustificativaCancelamentoAgendamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento JustificativaCancelamentoAgendamento = repositorioJustificativaCancelamentoAgendamento.BuscarPorCodigo(codigo);

                if (JustificativaCancelamentoAgendamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    JustificativaCancelamentoAgendamento.Codigo,
                    JustificativaCancelamentoAgendamento.Descricao,
                    Situacao = JustificativaCancelamentoAgendamento.Ativa,
                    JustificativaCancelamentoAgendamento.Observacao,
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

                Repositorio.Embarcador.Logistica.JustificativaCancelamentoAgendamento repositorioJustificativaCancelamentoAgendamento = new Repositorio.Embarcador.Logistica.JustificativaCancelamentoAgendamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento JustificativaCancelamentoAgendamento = new Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento();

                PreencherJustificativaCancelamentoAgendamento(JustificativaCancelamentoAgendamento, unitOfWork);

                if (repositorioJustificativaCancelamentoAgendamento.ExisteDuplicidade(JustificativaCancelamentoAgendamento))
                    throw new ControllerException("Já existe uma Justificativa cadastrada com os mesmos dados");

                repositorioJustificativaCancelamentoAgendamento.Inserir(JustificativaCancelamentoAgendamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.JustificativaCancelamentoAgendamento repositorioJustificativaCancelamentoAgendamento = new Repositorio.Embarcador.Logistica.JustificativaCancelamentoAgendamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento JustificativaCancelamentoAgendamento = repositorioJustificativaCancelamentoAgendamento.BuscarPorCodigo(codigo);

                if (JustificativaCancelamentoAgendamento == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherJustificativaCancelamentoAgendamento(JustificativaCancelamentoAgendamento, unitOfWork);

                if (repositorioJustificativaCancelamentoAgendamento.ExisteDuplicidade(JustificativaCancelamentoAgendamento))
                    throw new ControllerException("Já existe uma Justificativa cadastrada com os mesmos dados");

                unitOfWork.Start();

                repositorioJustificativaCancelamentoAgendamento.Atualizar(JustificativaCancelamentoAgendamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
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
                Repositorio.Embarcador.Logistica.JustificativaCancelamentoAgendamento repositorioJustificativaCancelamentoAgendamento = new Repositorio.Embarcador.Logistica.JustificativaCancelamentoAgendamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento JustificativaCancelamentoAgendamento = repositorioJustificativaCancelamentoAgendamento.BuscarPorCodigo(codigo);

                if (JustificativaCancelamentoAgendamento == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioJustificativaCancelamentoAgendamento.Deletar(JustificativaCancelamentoAgendamento, Auditado);

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

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJustificativaCancelamentoAgendamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJustificativaCancelamentoAgendamento()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<SituacaoAtivaPesquisa>("Situacao")
            };
        }

        private void PreencherJustificativaCancelamentoAgendamento(Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento JustificativaCancelamentoAgendamento, Repositorio.UnitOfWork unitOfWork)
        {
            JustificativaCancelamentoAgendamento.Descricao = Request.GetStringParam("Descricao");
            JustificativaCancelamentoAgendamento.Ativa = Request.GetBoolParam("Situacao");
            JustificativaCancelamentoAgendamento.Observacao = Request.GetStringParam("Observacao");
        }

        #endregion
    }
}
