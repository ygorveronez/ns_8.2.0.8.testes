using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/TipoPagamentoRecebimento")]
    public class TipoPagamentoRecebimentoController : BaseController
    {
		#region Construtores

		public TipoPagamentoRecebimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;
                Enum.TryParse(Request.Params("Ativo"), out ativo);
                string descricao = Request.Params("Descricao");
                int codigoPlanoConta;
                int.TryParse(Request.Params("PlanoConta"), out codigoPlanoConta);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoPagamentoRecebido.ContaGerencial, "PlanoConta", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoPagamentoRecebido.Status, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento> listaTipoPagamentoRecebimento = repTipoPagamentoRecebimento.Consultar(codigoEmpresa, descricao, codigoPlanoConta, ativo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoPagamentoRecebimento.ContarConsulta(codigoEmpresa, descricao, codigoPlanoConta, ativo));
                var lista = (from p in listaTipoPagamentoRecebimento
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 PlanoConta = p.PlanoConta != null ? "(" + p.PlanoConta.Plano + ") " + p.PlanoConta.Descricao : string.Empty,
                                 p.DescricaoAtivo
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamentoRecebimento = new Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento();

                PreencherTipoPagamentoRecebimento(tipoPagamentoRecebimento, unitOfWork);

                repTipoPagamentoRecebimento.Inserir(tipoPagamentoRecebimento, Auditado);

                SalvarConfiguracoesExportacao(tipoPagamentoRecebimento, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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

                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigo, true);

                PreencherTipoPagamentoRecebimento(tipoPagamentoRecebimento, unitOfWork);

                repTipoPagamentoRecebimento.Atualizar(tipoPagamentoRecebimento, Auditado);

                SalvarConfiguracoesExportacao(tipoPagamentoRecebimento, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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

                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigo);

                var dynTipoPagamentoRecebimento = new
                {
                    tipoPagamentoRecebimento.Codigo,
                    tipoPagamentoRecebimento.Descricao,
                    tipoPagamentoRecebimento.CodigoIntegracao,
                    tipoPagamentoRecebimento.Observacao,
                    PlanoConta = new { Codigo = tipoPagamentoRecebimento.PlanoConta != null ? tipoPagamentoRecebimento.PlanoConta.Codigo : 0, Descricao = tipoPagamentoRecebimento.PlanoConta != null ? "(" + tipoPagamentoRecebimento.PlanoConta.Plano + ") " + tipoPagamentoRecebimento.PlanoConta.Descricao : "" },
                    tipoPagamentoRecebimento.Ativo,
                    LimiteConta = tipoPagamentoRecebimento.LimiteConta.ToString("n2"),
                    tipoPagamentoRecebimento.Exportar,
                    ConfiguracoesExportacao = tipoPagamentoRecebimento.ContasExportacao.Select(o => new
                    {
                        o.Codigo,
                        o.ContaContabil,
                        PlanoConta = new { Descricao = o.PlanoConta?.BuscarDescricao ?? string.Empty, Codigo = o.PlanoConta?.Codigo ?? 0 },
                        CentroResultado = new { Descricao = o.CentroResultado?.Descricao ?? string.Empty, Codigo = o.CentroResultado?.Codigo ?? 0 },
                        o.CodigoCentroResultado,
                        o.Tipo,
                        Reversao = o.Reversao ?? false
                    }).ToList(),
                    tipoPagamentoRecebimento.ObrigaChequeBaixaTitulo
                };
                return new JsonpResult(dynTipoPagamentoRecebimento);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigo);

                repTipoPagamentoRecebimento.Deletar(tipoPagamentoRecebimento, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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

        private void PreencherTipoPagamentoRecebimento(Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamentoRecebimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);

            int codigoPlanoConta = Request.GetIntParam("PlanoConta");

            tipoPagamentoRecebimento.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tipoPagamentoRecebimento.Descricao = Request.GetStringParam("Descricao");
            tipoPagamentoRecebimento.Observacao = Request.GetStringParam("Observacao");
            tipoPagamentoRecebimento.LimiteConta = Request.GetDecimalParam("LimiteConta");
            tipoPagamentoRecebimento.Exportar = Request.GetBoolParam("Exportar");
            tipoPagamentoRecebimento.Ativo = Request.GetBoolParam("Ativo");
            tipoPagamentoRecebimento.ObrigaChequeBaixaTitulo = Request.GetBoolParam("ObrigaChequeBaixaTitulo");

            tipoPagamentoRecebimento.PlanoConta = codigoPlanoConta > 0 ? repPlanoConta.BuscarPorCodigo(codigoPlanoConta) : null;
            if (tipoPagamentoRecebimento.Codigo == 0)
                tipoPagamentoRecebimento.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;
        }

        private void SalvarConfiguracoesExportacao(Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamentoRecebimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.ConfiguracaoContaExportacao repConfiguracaoContaExportacao = new Repositorio.Embarcador.Financeiro.ConfiguracaoContaExportacao(unitOfWork);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            dynamic configuracoesExportacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaConfiguracoesExportacao"));

            if (tipoPagamentoRecebimento.ContasExportacao != null && tipoPagamentoRecebimento.ContasExportacao.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var configuracaoExportacao in configuracoesExportacao)
                {
                    int codigo = 0;

                    if (int.TryParse((string)configuracaoExportacao.Codigo, out codigo))
                        codigos.Add((int)configuracaoExportacao.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao> configuracoesDeletar = (from obj in tipoPagamentoRecebimento.ContasExportacao where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < configuracoesDeletar.Count; i++)
                    repConfiguracaoContaExportacao.Deletar(configuracoesDeletar[i]);
            }

            foreach (var configuracaoExportacao in configuracoesExportacao)
            {
                Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao config = null;

                int codigo = 0;

                if (configuracaoExportacao.Codigo != null && int.TryParse((string)configuracaoExportacao.Codigo, out codigo))
                    config = repConfiguracaoContaExportacao.BuscarPorCodigo(codigo, false);

                if (config == null)
                    config = new Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao();

                int codigoPlanoConta = (int)configuracaoExportacao.PlanoConta.Codigo;
                int codigoCentroResultado = (int)configuracaoExportacao.CentroResultado.Codigo;

                config.TipoPagamentoRecebimento = tipoPagamentoRecebimento;
                config.PlanoConta = codigoPlanoConta > 0 ? repPlanoConta.BuscarPorCodigo(codigoPlanoConta) : null;
                config.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito)configuracaoExportacao.Tipo;
                config.ContaContabil = (string)configuracaoExportacao.ContaContabil;
                config.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado, false) : null;
                config.CodigoCentroResultado = (string)configuracaoExportacao.CodigoCentroResultado;
                config.Reversao = (bool)configuracaoExportacao.Reversao;

                if (config.Codigo > 0)
                    repConfiguracaoContaExportacao.Atualizar(config);
                else
                    repConfiguracaoContaExportacao.Inserir(config);
            }
        }

        #endregion
    }
}
