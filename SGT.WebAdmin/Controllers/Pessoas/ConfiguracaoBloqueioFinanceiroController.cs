using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/ConfiguracaoBloqueioFinanceiro")]
    public class ConfiguracaoBloqueioFinanceiroController : BaseController
    {
		#region Construtores

		public ConfiguracaoBloqueioFinanceiroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Habilitado", "HabilitarRegra", 5, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Qtd. Dias Atraso Pagamento", "QuantidadeDiasAtrasoPagamento", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Qtd. Dias Novo Bloqueio", "QuantidadeDiasNovoBloqueio", 10, Models.Grid.Align.right);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaConfiguracaoBloqueioFinanceiro filtrosPesqusia = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro repConfiguracao = new Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro(unitOfWork);

                int totalRegistros = repConfiguracao.ContarConsulta(filtrosPesqusia, parametrosConsulta);

                List<Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro> configuracoes = totalRegistros > 0 ? repConfiguracao.Consultar(filtrosPesqusia, parametrosConsulta) : null;

                var registros = (from config in configuracoes
                                 select new
                                 {
                                     Codigo = config.Codigo.ToString() ?? "",
                                     Descricao = config.Descricao ?? "",
                                     HabilitarRegra = config.HabilitarRegra ? "Sim" : "Não",
                                     GrupoPessoa = config.GrupoPessoas?.Descricao ?? "",
                                     QuantidadeDiasAtrasoPagamento = config.QuantidadeDiasAtrasoPagamento.ToString() ?? "",
                                     QuantidadeDiasNovoBloqueio = config.QuantidadeDiasNovoBloqueio.ToString() ?? ""
                                 }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(registros);

                return new JsonpResult(grid);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar");
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

                Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro repConfiguracao = new Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro configuracao = new Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro();

                PreencherConfiguracao(configuracao, unitOfWork);

                repConfiguracao.Inserir(configuracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

                Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro repConfiguracao = new Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro configuracao = repConfiguracao.BuscarPorCodigo(codigo, true);

                if (configuracao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar a configuração.");

                PreencherConfiguracao(configuracao, unitOfWork);

                repConfiguracao.Atualizar(configuracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro repConfiguracao = new Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro configuracao = repConfiguracao.BuscarPorCodigo(codigo, false);

                var dynConfiguracao = new
                {
                    configuracao.Codigo,
                    configuracao.Descricao,
                    configuracao.HabilitarRegra,
                    configuracao.QuantidadeDiasAtrasoPagamento,
                    configuracao.QuantidadeDiasNovoBloqueio,
                    GrupoPessoa = new { Codigo = configuracao.GrupoPessoas?.Codigo ?? 0, Descricao = configuracao.GrupoPessoas?.Descricao ?? string.Empty }
                };

                return new JsonpResult(dynConfiguracao);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro repConfiguracao = new Repositorio.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro configuracao = repConfiguracao.BuscarPorCodigo(codigo, true);

                if (configuracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repConfiguracao.Deletar(configuracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaConfiguracaoBloqueioFinanceiro ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaConfiguracaoBloqueioFinanceiro filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaConfiguracaoBloqueioFinanceiro()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
            };

            return filtrosPesquisa;
        }

        private void PreencherConfiguracao(Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoBloqueioFinanceiro configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.GetStringParam("Descricao");
            bool habilitarRegra = Request.GetBoolParam("HabilitarRegra");
            int quantidadeDiasAtrasoPagamento = Request.GetIntParam("QuantidadeDiasAtrasoPagamento");
            int quantidadeDiasNovoBloqueio = Request.GetIntParam("QuantidadeDiasNovoBloqueio");
            int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoa");

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = codigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa) : null;

            configuracao.Descricao = descricao;
            configuracao.HabilitarRegra = habilitarRegra;
            configuracao.QuantidadeDiasAtrasoPagamento = quantidadeDiasAtrasoPagamento;
            configuracao.QuantidadeDiasNovoBloqueio = quantidadeDiasNovoBloqueio;
            configuracao.GrupoPessoas = grupoPessoas;
        }

        #endregion
    }
}
