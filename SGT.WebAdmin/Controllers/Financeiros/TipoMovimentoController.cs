using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/TipoMovimento")]
    public class TipoMovimentoController : BaseController
    {
		#region Construtores

		public TipoMovimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento finalidadeTipoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Todas;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTipoMovimento forma;
                Enum.TryParse(Request.Params("Ativo"), out ativo);
                Enum.TryParse(Request.Params("FormaTipoMovimento"), out forma);
                string descricao = Request.Params("Descricao");
                int centroCusto, planoDebito, planoCredito = 0;
                int.TryParse(Request.Params("CentroResultado"), out centroCusto);
                int.TryParse(Request.Params("PlanoDebito"), out planoDebito);
                int.TryParse(Request.Params("PlanoCredito"), out planoCredito);
                List<int> finalidades = new List<int>();
                string codigoIntegracao = Request.Params("CodigoIntegracao");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    //Enum.TryParse(Request.Params("FinalidadeTipoMovimento"), out finalidadeTipoMovimento);

                    int.TryParse(Request.Params("FinalidadeTipoMovimento"), out int finalidade);
                    if (finalidade > 0)
                    {
                        finalidades.Add((int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Todas);
                        finalidades.Add(finalidade);
                    }
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Cód.", "Codigo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoMovimento.ContaDeEntrada, "PlanoDebito", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoMovimento.ContaDeSaida, "PlanoCredito", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CodigoDebito", false);
                grid.AdicionarCabecalho("CodigoCredito", false);
                grid.AdicionarCabecalho("CentroResultado", false);
                grid.AdicionarCabecalho("CodigoResultado", false);
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 7, Models.Grid.Align.left, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "PlanoDebito")
                    propOrdenar = "PlanoDeContaDebito.Plano";
                if (propOrdenar == "PlanoCredito")
                    propOrdenar = "PlanoDeContaCredito.Plano";
                if (propOrdenar == "CentroResultado")
                    propOrdenar = "CentroResultado.Plano";

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento> listaTipoMovimento = repTipoMovimento.Consultar(codigoEmpresa, descricao, centroCusto, planoDebito, planoCredito, ativo, forma, finalidadeTipoMovimento, finalidades, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, codigoIntegracao);
                grid.setarQuantidadeTotal(repTipoMovimento.ContarConsulta(codigoEmpresa, descricao, centroCusto, planoDebito, planoCredito, ativo, forma, finalidadeTipoMovimento, finalidades, codigoIntegracao));
                var lista = (from p in listaTipoMovimento
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 PlanoDebito = p.PlanoDeContaDebito != null ? "(" + p.PlanoDeContaDebito.Plano + ") " + p.PlanoDeContaDebito.Descricao : string.Empty,
                                 PlanoCredito = p.PlanoDeContaCredito != null ? "(" + p.PlanoDeContaCredito.Plano + ") " + p.PlanoDeContaCredito.Descricao : string.Empty,
                                 CodigoDebito = p.PlanoDeContaDebito != null ? p.PlanoDeContaDebito.Codigo : 0,
                                 CodigoCredito = p.PlanoDeContaCredito != null ? p.PlanoDeContaCredito.Codigo : 0,
                                 CodigoResultado = p.CentrosResultados != null ? p.CentrosResultados.Count() == 1 ? p.CentrosResultados[0].CentroResultado != null ? p.CentrosResultados[0].CentroResultado.Codigo : 0 : 0 : 0,
                                 CentroResultado = p.CentrosResultados != null ? p.CentrosResultados.Count() == 1 ? p.CentrosResultados[0].CentroResultado != null ? p.CentrosResultados[0].CentroResultado.Descricao : string.Empty : string.Empty : string.Empty,
                                 p.DescricaoAtivo,
                                 p.CodigoIntegracao
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = new Dominio.Entidades.Embarcador.Financeiro.TipoMovimento();

                PreencherTipoMovimento(tipoMovimento, unitOfWork);

                repTipoMovimento.Inserir(tipoMovimento, Auditado);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                SalvarListaCentroResultado(tipoMovimento, unitOfWork, configuracaoFinanceiro);
                SalvarConfiguracoesExportacao(tipoMovimento, unitOfWork);
                SalvarTipoDespesa(tipoMovimento, unitOfWork, configuracaoFinanceiro);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = repTipoMovimento.BuscarPorCodigo(codigo, true);

                if (tipoMovimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherTipoMovimento(tipoMovimento, unitOfWork);

                repTipoMovimento.Atualizar(tipoMovimento, Auditado);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                SalvarListaCentroResultado(tipoMovimento, unitOfWork, configuracaoFinanceiro);
                SalvarConfiguracoesExportacao(tipoMovimento, unitOfWork);
                SalvarTipoDespesa(tipoMovimento, unitOfWork, configuracaoFinanceiro);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = repTipoMovimento.BuscarPorCodigo(codigo);

                if (tipoMovimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa> tiposDespesa = repTipoMovimentoTipoDespesa.BuscarPorTipoMovimento(codigo);

                var dynTipoMovimento = new
                {
                    tipoMovimento.Codigo,
                    tipoMovimento.Descricao,
                    tipoMovimento.Observacao,
                    tipoMovimento.CodigoFinalidadeTED,
                    PlanoDebito = new { Codigo = tipoMovimento.PlanoDeContaDebito != null ? tipoMovimento.PlanoDeContaDebito.Codigo : 0, Descricao = tipoMovimento.PlanoDeContaDebito != null ? "(" + tipoMovimento.PlanoDeContaDebito.Plano + ") " + tipoMovimento.PlanoDeContaDebito.Descricao : "" },
                    PlanoCredito = new { Codigo = tipoMovimento.PlanoDeContaCredito != null ? tipoMovimento.PlanoDeContaCredito.Codigo : 0, Descricao = tipoMovimento.PlanoDeContaCredito != null ? "(" + tipoMovimento.PlanoDeContaCredito.Plano + ") " + tipoMovimento.PlanoDeContaCredito.Descricao : "" },
                    PlanoDebitoSintetico = new { Codigo = tipoMovimento.PlanoDeContaDebito != null ? tipoMovimento.PlanoDeContaDebito.Codigo : 0, Descricao = tipoMovimento.PlanoDeContaDebito != null ? "(" + tipoMovimento.PlanoDeContaDebito.Plano + ") " + tipoMovimento.PlanoDeContaDebito.Descricao : "" },
                    PlanoCreditoSintetico = new { Codigo = tipoMovimento.PlanoDeContaCredito != null ? tipoMovimento.PlanoDeContaCredito.Codigo : 0, Descricao = tipoMovimento.PlanoDeContaCredito != null ? "(" + tipoMovimento.PlanoDeContaCredito.Plano + ") " + tipoMovimento.PlanoDeContaCredito.Descricao : "" },
                    CentroResultado = (from p in tipoMovimento.CentrosResultados
                                       select new
                                       {
                                           p.CentroResultado.Codigo,
                                           p.CentroResultado.Descricao
                                       }).ToList(),
                    tipoMovimento.Ativo,
                    tipoMovimento.FormaTipoMovimento,
                    tipoMovimento.FinalidadeTipoMovimento,
                    Finalidades = Array.ConvertAll(tipoMovimento.Finalidades.Split(';'), int.Parse),
                    tipoMovimento.Exportar,
                    tipoMovimento.NaoGerarRateioDeDespesaPorVeiculo,
                    ConfiguracoesExportacao = tipoMovimento.ContasExportacao.Select(o => new
                    {
                        o.Codigo,
                        o.ContaContabil,
                        PlanoConta = new { Descricao = o.PlanoConta?.BuscarDescricao ?? string.Empty, Codigo = o.PlanoConta?.Codigo ?? 0 },
                        CentroResultado = new { Descricao = o.CentroResultado?.Descricao ?? string.Empty, Codigo = o.CentroResultado?.Codigo ?? 0 },
                        o.CodigoCentroResultado,
                        o.Tipo
                    }).ToList(),
                    TiposDespesa = (from p in tiposDespesa
                                    select new
                                    {
                                        p.TipoDespesaFinanceira.Codigo,
                                        p.TipoDespesaFinanceira.Descricao
                                    }).ToList(),
                    Banco = tipoMovimento.Banco != null ? new { tipoMovimento.Banco.Codigo, tipoMovimento.Banco.Descricao } : null,
                    tipoMovimento.CodigoHistorico,
                    tipoMovimento.CodigoIntegracao
                };

                return new JsonpResult(dynTipoMovimento);
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = repTipoMovimento.BuscarPorCodigo(codigo);

                if (tipoMovimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado repTipoMovimentoCentroResultado = new Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado> listaCentroResultado = repTipoMovimentoCentroResultado.BuscarPorTipoMovimento(tipoMovimento.Codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa> listaTipoDespesa = repTipoMovimentoTipoDespesa.BuscarPorTipoMovimento(tipoMovimento.Codigo);

                foreach (Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado centroResultado in listaCentroResultado)
                    repTipoMovimentoCentroResultado.Deletar(centroResultado, Auditado);
                foreach (Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa tipoDespesa in listaTipoDespesa)
                    repTipoMovimentoTipoDespesa.Deletar(tipoDespesa, Auditado);

                repTipoMovimento.Deletar(tipoMovimento, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterFinalidades()
        {
            try
            {
                ArrayList retorno = new ArrayList();

                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Todas, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimentoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Todas) });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.TituloFinanceiro, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimentoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.TituloFinanceiro) });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Pedagio, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimentoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Pedagio) });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.NaturezaOperacao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimentoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.NaturezaOperacao) });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.MultiploTitulo, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimentoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.MultiploTitulo) });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.MovimentoFinanceiro, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimentoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.MovimentoFinanceiro) });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Justificativa, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimentoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Justificativa) });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.FaturamentoMensal, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimentoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.FaturamentoMensal) });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.DocumentoEntrada, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimentoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.DocumentoEntrada) });
                retorno.Insert(0, new { value = (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Abastecimento, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimentoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Abastecimento) });

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar as Finalidades.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado repTipoMovimentoCentroResultado = new Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = repTipoMovimento.BuscarPorCodigo(codigo);

                if (tipoMovimento == null)
                    return new JsonpResult(false, true, "Tipo de Movimento não encontrado.");

                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado> centrosResultados = repTipoMovimentoCentroResultado.BuscarPorTipoMovimento(codigo);
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = centrosResultados.Count == 1 ? centrosResultados.FirstOrDefault()?.CentroResultado : null;

                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesa = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(codigo);

                var retorno = new
                {
                    tipoMovimento.Codigo,
                    tipoMovimento.Descricao,
                    CodigoResultado = centroResultado?.Codigo ?? 0,
                    CentroResultado = centroResultado?.Descricao ?? string.Empty,
                    CodigoTipoDespesa = tipoDespesa?.Codigo ?? 0,
                    TipoDespesa = tipoDespesa?.Descricao ?? string.Empty
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes do tipo de movimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoTipoMovimento(unitOfWork);
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        string retorno = "";

                        Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = new Dominio.Entidades.Embarcador.Financeiro.TipoMovimento();
                        tipoMovimento.Ativo = true;
                        tipoMovimento.Finalidades = "0";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricao = (from obj in linha.Colunas where obj.NomeCampo == "Descricao" select obj).FirstOrDefault();
                        if (colDescricao != null)
                            tipoMovimento.Descricao = colDescricao.Valor;
                        else
                            throw new Exception("O campo descrição é obrigatório para adicionar um registro!");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPlanoDebito = (from obj in linha.Colunas where obj.NomeCampo == "PlanoDebito" select obj).FirstOrDefault();
                        if (colPlanoDebito != null)
                        {
                            string codigoPlanoDebito = colPlanoDebito.Valor;
                            tipoMovimento.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo(codigoPlanoDebito.ToInt());
                        }
                        else
                            throw new Exception("O campo conta de entrada é obrigatório para adicionar um registro!");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPlanoCredito = (from obj in linha.Colunas where obj.NomeCampo == "PlanoCredito" select obj).FirstOrDefault();
                        if (colPlanoCredito != null)
                        {
                            string codigoPlanoCredito = colPlanoCredito.Valor;
                            tipoMovimento.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo(codigoPlanoCredito.ToInt());
                        }
                        else
                            throw new Exception("O campo conta de saída é obrigatório para adicionar um registro!");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBanco = (from obj in linha.Colunas where obj.NomeCampo == "Banco" select obj).FirstOrDefault();
                        if (colBanco != null)
                        {
                            string numeroBanco = colBanco.Valor;
                            tipoMovimento.Banco = repBanco.BuscarPorNumero(numeroBanco.ToInt());
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoHistorico = (from obj in linha.Colunas where obj.NomeCampo == "CodigoHistorico" select obj).FirstOrDefault();
                        tipoMovimento.CodigoHistorico = "";
                        if (colCodigoHistorico != null)
                            tipoMovimento.CodigoHistorico = colCodigoHistorico.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracao" select obj).FirstOrDefault();
                        tipoMovimento.CodigoIntegracao = "";
                        if (colCodigoIntegracao != null)
                            tipoMovimento.CodigoIntegracao = colCodigoIntegracao.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                        tipoMovimento.Observacao = "";
                        if (colObservacao != null)
                            tipoMovimento.Observacao = colObservacao.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colExportar = (from obj in linha.Colunas where obj.NomeCampo == "Exportar" select obj).FirstOrDefault();
                        tipoMovimento.Exportar = false;
                        if (colExportar != null)
                            tipoMovimento.Exportar = colExportar.Valor != "1" ? false : true;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNaoGerarRateioDeDespesaPorVeiculo = (from obj in linha.Colunas where obj.NomeCampo == "NaoGerarRateioDeDespesaPorVeiculo" select obj).FirstOrDefault();
                        tipoMovimento.NaoGerarRateioDeDespesaPorVeiculo = false;
                        if (colNaoGerarRateioDeDespesaPorVeiculo != null)
                            tipoMovimento.NaoGerarRateioDeDespesaPorVeiculo = colNaoGerarRateioDeDespesaPorVeiculo.Valor != "1" ? false : true;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoFinalidadeTED = (from obj in linha.Colunas where obj.NomeCampo == "CodigoFinalidadeTED" select obj).FirstOrDefault();
                        tipoMovimento.CodigoFinalidadeTED = "";
                        if (colCodigoFinalidadeTED != null)
                            tipoMovimento.CodigoFinalidadeTED = colCodigoFinalidadeTED.Valor;


                        //Salvar Tipo Movimento
                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                        }
                        else
                        {
                            repTipoMovimento.Inserir(tipoMovimento, Auditado);
                            contador++;
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(ex2.Message, i));
                    }
                }

                unitOfWork.CommitChanges();

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoTipoMovimento(unitOfWork);

            return new JsonpResult(configuracoes.ToList());
        }

        #endregion

        #region Métodos Privados

        private void PreencherTipoMovimento(Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

            int.TryParse(Request.Params("PlanoDebito"), out int planoDebito);
            int.TryParse(Request.Params("PlanoCredito"), out int planoCredito);
            int.TryParse(Request.Params("PlanoDebitoSintetico"), out int planoDebitoSintetico);
            int.TryParse(Request.Params("PlanoCreditoSintetico"), out int planoCreditoSintetico);
            int codigoBanco = Request.GetIntParam("Banco");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento finalidadeTipoMovimento;
            Enum.TryParse(Request.Params("FinalidadeTipoMovimento"), out finalidadeTipoMovimento);

            List<int> finalidades = JsonConvert.DeserializeObject<List<int>>(Request.Params("Finalidades"));
            tipoMovimento.Finalidades = string.Join(";", finalidades);
            if (string.IsNullOrEmpty(tipoMovimento.Finalidades))
                tipoMovimento.Finalidades = "0";

            if (tipoMovimento.Codigo == 0)
                tipoMovimento.Empresa = Usuario.Empresa;

            tipoMovimento.FormaTipoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTipoMovimento.Automatica;
            tipoMovimento.Ativo = Request.GetBoolParam("Ativo");
            tipoMovimento.Descricao = Request.Params("Descricao");
            tipoMovimento.Observacao = Request.Params("Observacao");
            tipoMovimento.CodigoFinalidadeTED = Request.Params("CodigoFinalidadeTED");
            tipoMovimento.Exportar = Request.GetBoolParam("Exportar");
            tipoMovimento.NaoGerarRateioDeDespesaPorVeiculo = Request.GetBoolParam("NaoGerarRateioDeDespesaPorVeiculo");
            tipoMovimento.CodigoHistorico = Request.GetStringParam("CodigoHistorico");

            if (planoDebito > 0)
                tipoMovimento.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo(planoDebito);
            else
                tipoMovimento.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo(planoDebitoSintetico);

            if (planoCredito > 0)
                tipoMovimento.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo(planoCredito);
            else
                tipoMovimento.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo(planoCreditoSintetico);

            if (tipoMovimento.PlanoDeContaCredito.Codigo == tipoMovimento.PlanoDeContaDebito.Codigo)
                throw new ControllerException("Não é permitido cadastrar com as mesmas contas de entrada e saída.");

            tipoMovimento.FinalidadeTipoMovimento = finalidadeTipoMovimento;
            tipoMovimento.Banco = codigoBanco > 0 ? repBanco.BuscarPorCodigo(codigoBanco) : null;
            tipoMovimento.CodigoIntegracao = Request.Params("CodigoIntegracao");
        }

        private void SalvarListaCentroResultado(Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro)
        {
            Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado repTipoMovimentoCentroResultado = new Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado> listaCentroResultado = repTipoMovimentoCentroResultado.BuscarPorTipoMovimento(tipoMovimento.Codigo);
            for (int i = 0; i < listaCentroResultado.Count(); i++)
                repTipoMovimentoCentroResultado.Deletar(listaCentroResultado[i], Auditado);

            dynamic listaCentro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CentroResultado"));
            if (listaCentro != null)
            {
                foreach (var centroResultado in listaCentro)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado tipoMovimentoCentro = new Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado();
                    tipoMovimentoCentro.TipoMovimento = tipoMovimento;
                    tipoMovimentoCentro.CentroResultado = repCentroResultado.BuscarPorCodigo((int)centroResultado.Codigo);
                    repTipoMovimentoCentroResultado.Inserir(tipoMovimentoCentro, Auditado);
                }
            }

            if (configuracaoFinanceiro.AtivarControleDespesas)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado> centrosResultadoValidar = repTipoMovimentoCentroResultado.BuscarPorTipoMovimento(tipoMovimento.Codigo);
                if (centrosResultadoValidar.Count > 1)
                    throw new ControllerException("Só é permitido informar um Centro de Resultado para o controle de despesas.");
            }
        }

        private void SalvarConfiguracoesExportacao(Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.ConfiguracaoContaExportacao repConfiguracaoContaExportacao = new Repositorio.Embarcador.Financeiro.ConfiguracaoContaExportacao(unitOfWork);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            dynamic configuracoesExportacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaConfiguracoesExportacao"));

            if (tipoMovimento.ContasExportacao != null && tipoMovimento.ContasExportacao.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var configuracaoExportacao in configuracoesExportacao)
                {
                    int codigo = 0;

                    if (int.TryParse((string)configuracaoExportacao.Codigo, out codigo))
                        codigos.Add((int)configuracaoExportacao.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao> configuracoesDeletar = (from obj in tipoMovimento.ContasExportacao where !codigos.Contains(obj.Codigo) select obj).ToList();

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

                config.TipoMovimento = tipoMovimento;
                config.PlanoConta = codigoPlanoConta > 0 ? repPlanoConta.BuscarPorCodigo(codigoPlanoConta) : null;
                config.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito)configuracaoExportacao.Tipo;
                config.ContaContabil = (string)configuracaoExportacao.ContaContabil;
                config.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado, false) : null;
                config.CodigoCentroResultado = (string)configuracaoExportacao.CodigoCentroResultado;

                if (config.Codigo > 0)
                    repConfiguracaoContaExportacao.Atualizar(config);
                else
                    repConfiguracaoContaExportacao.Inserir(config);
            }
        }

        private void SalvarTipoDespesa(Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro)
        {
            Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesaFinanceira = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

            dynamic dynTiposDespesa = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposDespesa"));

            List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa> tiposDespesa = repTipoMovimentoTipoDespesa.BuscarPorTipoMovimento(tipoMovimento.Codigo);

            if (tiposDespesa.Count > 0)
            {
                List<long> codigos = new List<long>();

                foreach (dynamic tipoDespesa in dynTiposDespesa)
                {
                    int codigoTipoDespesa = ((string)tipoDespesa.Codigo).ToInt();
                    if (codigoTipoDespesa > 0)
                        codigos.Add(codigoTipoDespesa);
                }

                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa> deletar = (from obj in tiposDespesa where !codigos.Contains(obj.TipoDespesaFinanceira.Codigo) select obj).ToList();

                for (var i = 0; i < deletar.Count; i++)
                    repTipoMovimentoTipoDespesa.Deletar(deletar[i]);
            }

            foreach (dynamic tipoDespesa in dynTiposDespesa)
            {
                int codigoTipoDespesa = ((string)tipoDespesa.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa tipoOperacaoGrupoTomadorBloqueado = repTipoMovimentoTipoDespesa.BuscarPorTipoMovimentoETipoDespesa(tipoMovimento.Codigo, codigoTipoDespesa);

                if (tipoOperacaoGrupoTomadorBloqueado == null)
                {
                    tipoOperacaoGrupoTomadorBloqueado = new Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa();

                    tipoOperacaoGrupoTomadorBloqueado.TipoMovimento = tipoMovimento;
                    tipoOperacaoGrupoTomadorBloqueado.TipoDespesaFinanceira = repTipoDespesaFinanceira.BuscarPorCodigo(codigoTipoDespesa);

                    repTipoMovimentoTipoDespesa.Inserir(tipoOperacaoGrupoTomadorBloqueado);
                }
            }

            if (configuracaoFinanceiro.AtivarControleDespesas)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa> tiposDespesaValidar = repTipoMovimentoTipoDespesa.BuscarPorTipoMovimento(tipoMovimento.Codigo);
                if (tiposDespesaValidar.Count > 1)
                    throw new ControllerException("Só é permitido informar um Tipo de Despesa para o controle de despesas.");
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoTipoMovimento(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();
            int tamanho = 200;

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Financeiro.TipoMovimento.Descricao, Propriedade = "Descricao", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Financeiro.TipoMovimento.PlanoDebito, Propriedade = "PlanoDebito", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Financeiro.TipoMovimento.PlanoCredito, Propriedade = "PlanoCredito", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Financeiro.TipoMovimento.Banco, Propriedade = "Banco", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Financeiro.TipoMovimento.CodigoHistorico, Propriedade = "CodigoHistorico", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Financeiro.TipoMovimento.CodigoIntegracao, Propriedade = "CodigoIntegracao", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Financeiro.TipoMovimento.Observacao, Propriedade = "Observacao", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Financeiro.TipoMovimento.Exportar, Propriedade = "Exportar", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Financeiro.TipoMovimento.NaoGerarRateioDeDespesaPorVeiculo, Propriedade = "NaoGerarRateioDeDespesaPorVeiculo", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Financeiro.TipoMovimento.CodigoFinalidadeTED, Propriedade = "CodigoFinalidadeTED", Tamanho = tamanho, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        #endregion
    }
}
