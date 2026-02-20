using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GerenciamentoIrregularidades
{
    [CustomAuthorize("GerenciamentoIrregularidades/Irregularidade")]
    public class IrregularidadeController : BaseController
    {
		#region Construtores

		public IrregularidadeController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho("Gatilho", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Sequência", "Sequencia", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Portfólio Modulo Controle", "PortfolioModuloControle", 40, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Seguir para aprovação da Transp. Primeiro", "SeguirAprovacaoTranspPrimeiro", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaIrregularidade filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade repositorioIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);
                int totalRegistro = repositorioIrregularidade.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade> irregularidades = (totalRegistro > 0) ? repositorioIrregularidade.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade>();

                var irregularidadesRetornar = (
                    from irregularidade in irregularidades
                    select new
                    {
                        irregularidade.Codigo,
                        Gatilho = irregularidade.GatilhoIrregularidade,
                        irregularidade.Descricao,
                        irregularidade.Sequencia,
                        irregularidade.CodigoIntegracao,
                        PortfolioModuloControle = irregularidade.PortfolioModuloControle.Descricao,
                        SeguirAprovacaoTranspPrimeiro = irregularidade.SeguirAprovacaoTranspPrimeiro.ObterDescricao(),
                        Situacao = irregularidade.Ativa ? SituacaoAtivaPesquisa.Ativa.ObterDescricao() : SituacaoAtivaPesquisa.Inativa.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(irregularidadesRetornar);
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
                Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade repositorioIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade Irregularidade = repositorioIrregularidade.BuscarPorCodigo(codigo);

                if (Irregularidade == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    Irregularidade.Codigo,
                    Irregularidade.Descricao,
                    Irregularidade.Sequencia,
                    Irregularidade.CodigoIntegracao,
                    PortfolioModuloControle = new { Irregularidade.PortfolioModuloControle.Codigo, Irregularidade.PortfolioModuloControle.Descricao },
                    Irregularidade.SeguirAprovacaoTranspPrimeiro,
                    Situacao = Irregularidade.Ativa,
                    Tipo = Irregularidade.TipoIrregularidade,
                    Irregularidade.ValorTolerancia,
                    Irregularidade.PercentualTolerancia,
                    Gatilho = Irregularidade.GatilhoIrregularidade
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

                Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade repositorioIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade Irregularidade = new Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade();

                PreencherIrregularidade(Irregularidade, unitOfWork);

                if (repositorioIrregularidade.ExisteDuplicidade(Irregularidade))
                    throw new ControllerException("Já existe uma Irregularidade cadastrada com os mesmos dados");

                if (repositorioIrregularidade.ExisteRegistroComMesmoGatilho(Irregularidade))
                    throw new ControllerException("Já existe uma irregularidade cadastrada com esse gatilho!");

                repositorioIrregularidade.Inserir(Irregularidade, Auditado);

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
                Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade repositorioIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade Irregularidade = repositorioIrregularidade.BuscarPorCodigo(codigo);

                if (Irregularidade == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherIrregularidade(Irregularidade, unitOfWork);

                if (repositorioIrregularidade.ExisteDuplicidade(Irregularidade))
                    throw new ControllerException("Já existe uma Irregularidade cadastrada com os mesmos dados");

                if (repositorioIrregularidade.ExisteRegistroComMesmoGatilho(Irregularidade))
                    throw new ControllerException("Já existe uma irregularidade cadastrada com esse gatilho!");

                unitOfWork.Start();

                repositorioIrregularidade.Atualizar(Irregularidade, Auditado);

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
                Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade repositorioIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade Irregularidade = repositorioIrregularidade.BuscarPorCodigo(codigo);

                if (Irregularidade == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioIrregularidade.Deletar(Irregularidade, Auditado);

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

        private Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaIrregularidade ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaIrregularidade()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Sequencia = Request.GetIntParam("Sequencia"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                CodigoPortfolioModuloControle = Request.GetIntParam("PortfolioModuloControle"),
                SeguirAprovacaoTranspPrimeiro = Request.GetNullableBoolParam("SeguirAprovacaoTranspPrimeiro"),
                Situacao = Request.GetEnumParam<SituacaoAtivaPesquisa>("Situacao")
            };
        }

        private void PreencherIrregularidade(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade Irregularidade, Repositorio.UnitOfWork unitOfWork)
        {
            var portRep = new Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle(unitOfWork);
            var Portfolio = portRep.BuscarPorCodigo(Request.GetIntParam("PortfolioModuloControle"));
            if (Portfolio == null)
                throw new ControllerException("É necessário vincular a Irregularidade a um Portfólio");
            Irregularidade.Descricao = Request.GetStringParam("Descricao");
            Irregularidade.Sequencia = Request.GetIntParam("Sequencia");
            Irregularidade.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            Irregularidade.PortfolioModuloControle = Portfolio;
            Irregularidade.SeguirAprovacaoTranspPrimeiro = Request.GetBoolParam("SeguirAprovacaoTranspPrimeiro");
            Irregularidade.Ativa = Request.GetBoolParam("Situacao");
            Irregularidade.TipoIrregularidade = Request.GetEnumParam<TipoIrregularidade>("Tipo");
            Irregularidade.GatilhoIrregularidade = Request.GetEnumParam<GatilhoIrregularidade>("Gatilho");
            Irregularidade.ValorTolerancia = Request.GetDecimalParam("ValorTolerancia");
            Irregularidade.PercentualTolerancia = Request.GetIntParam("PercentualTolerancia");
        }

        #endregion
    }
}
