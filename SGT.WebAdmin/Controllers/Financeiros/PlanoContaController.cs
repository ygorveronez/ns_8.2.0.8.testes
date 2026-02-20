using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AdminMultisoftware.Repositorio;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using DocumentFormat.OpenXml.Spreadsheet;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig;
using Dominio.ObjetosDeValor.Embarcador.Mobile.Request;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/PlanoConta")]
    public class PlanoContaController : BaseController
    {
		#region Construtores

		public PlanoContaController(Conexao conexao) : base(conexao) { }

		#endregion


        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.PlanoConta.Plano, "Plano", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Todas, "DescricaoAnaliticoSintetico", 9, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.PlanoConta.ReceitaDespesa, "DescricaoReceitaDespesa", 9, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.PlanoConta.GrupoDeResultado, "DescricaoGrupoDeResultado", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 7, Models.Grid.Align.center, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);

            int planoContaCredito = Request.GetIntParam("PlanoContaCredito");
            int planoContaDebito = Request.GetIntParam("PlanoContaDebito");

            int codigo = 0;
            int.TryParse(Request.Params("Codigo"), out codigo);
            string descricao = Request.Params("Descricao");
            string codigoIntegracao = Request.Params("CodigoIntegracao");
            if (codigo == 0 && !string.IsNullOrWhiteSpace(descricao))
            {
                int.TryParse(Request.Params("Descricao"), out codigo);
                if (codigo > 0)
                    descricao = "";
            }

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            string plano = Request.Params("Plano");
            string comGrupoResultado = Request.Params("ComGrupoResultado");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico tipo;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ReceitaDespesa receitaDespesa;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado grupoDeResultado;
            Enum.TryParse(Request.Params("Tipo"), out tipo);
            Enum.TryParse(Request.Params("Ativo"), out ativo);
            Enum.TryParse(Request.Params("ReceitaDespesa"), out receitaDespesa);
            Enum.TryParse(Request.Params("GrupoDeResultado"), out grupoDeResultado);

            List<Dominio.Entidades.Embarcador.Financeiro.PlanoConta> listaPlanoDeConta = repPlanoConta.Consultar(codigoIntegracao, receitaDespesa, codigo, codigoEmpresa, descricao, plano, ativo, tipo, grupoDeResultado, comGrupoResultado, planoContaDebito, planoContaCredito, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repPlanoConta.ContarConsulta(codigoIntegracao, receitaDespesa, codigo, codigoEmpresa, descricao, plano, ativo, tipo, grupoDeResultado, comGrupoResultado, planoContaDebito, planoContaCredito);
            var lista = (from p in listaPlanoDeConta
                         select new
                         {
                             p.Codigo,
                             Descricao = p.BuscarDescricao,
                             p.Plano,
                             p.DescricaoAnaliticoSintetico,
                             p.DescricaoReceitaDespesa,
                             p.DescricaoGrupoDeResultado,
                             p.DescricaoAtivo
                         }).ToList();

            return lista.ToList();
        }

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                // Retorna Dados
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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

                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoDeConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoDeConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta();
                planoDeConta.Ativo = bool.Parse(Request.Params("Ativo"));
                planoDeConta.Descricao = Request.Params("Descricao");
                planoDeConta.Plano = Request.Params("Plano");
                planoDeConta.PlanoContabilidade = Request.Params("PlanoContabilidade");
                planoDeConta.AnaliticoSintetico = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico)int.Parse(Request.Params("AnaliticoSintetico"));
                planoDeConta.ReceitaDespesa = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ReceitaDespesa)int.Parse(Request.Params("ReceitaDespesa"));
                planoDeConta.GrupoDeResultado = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado)int.Parse(Request.Params("GrupoDeResultado"));
                planoDeConta.Empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);
                planoDeConta.SaldoInicialConciliacaoBancaria = Request.GetDecimalParam("SaldoInicialConciliacaoBancaria");
                planoDeConta.SaldoFinalConciliacaoBancaria = Request.GetDecimalParam("SaldoFinalConciliacaoBancaria");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                if (repPlanoDeConta.ContemPlanoConta(int.Parse(Request.Params("Codigo")), Request.Params("Plano"), codigoEmpresa))
                    return new JsonpResult(false, "Esta numeração de plano de contas já está sendo utilizada.");

                repPlanoDeConta.Inserir(planoDeConta, Auditado);
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
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoDeConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoDeConta = repPlanoDeConta.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                planoDeConta.Ativo = bool.Parse(Request.Params("Ativo"));
                planoDeConta.Descricao = Request.Params("Descricao");
                planoDeConta.Plano = Request.Params("Plano");
                planoDeConta.PlanoContabilidade = Request.Params("PlanoContabilidade");
                planoDeConta.AnaliticoSintetico = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico)int.Parse(Request.Params("AnaliticoSintetico"));
                planoDeConta.ReceitaDespesa = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ReceitaDespesa)int.Parse(Request.Params("ReceitaDespesa"));
                planoDeConta.GrupoDeResultado = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado)int.Parse(Request.Params("GrupoDeResultado"));
                planoDeConta.Empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa?.Codigo ?? 0);
                planoDeConta.SaldoInicialConciliacaoBancaria = Request.GetDecimalParam("SaldoInicialConciliacaoBancaria");
                planoDeConta.SaldoFinalConciliacaoBancaria = Request.GetDecimalParam("SaldoFinalConciliacaoBancaria");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                if (repPlanoDeConta.ContemPlanoConta(int.Parse(Request.Params("Codigo")), Request.Params("Plano"), codigoEmpresa))
                    return new JsonpResult(false, "Esta numeração de plano de contas já está sendo utilizada.");

                repPlanoDeConta.Atualizar(planoDeConta, Auditado);
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoDeConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoDeConta = repPlanoDeConta.BuscarPorCodigo(codigo);
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoPai = repPlanoDeConta.BuscarPlanoPai(planoDeConta.Plano, codigoEmpresa);
                var dynPlanoConta = new
                {
                    planoDeConta.Codigo,
                    planoDeConta.Descricao,
                    Plano = new { Codigo = planoDeConta.Plano != null ? planoDeConta.Plano : "", Descricao = planoDeConta.Plano != null ? planoDeConta.Plano : "" },
                    planoDeConta.PlanoContabilidade,
                    planoDeConta.Ativo,
                    planoDeConta.AnaliticoSintetico,
                    PlanoContaSintetico = planoPai != null ? "(" + planoPai.Plano + ") - " + planoPai.Descricao : string.Empty,
                    planoDeConta.ReceitaDespesa,
                    planoDeConta.GrupoDeResultado,
                    SaldoInicialConciliacaoBancaria = planoDeConta.SaldoInicialConciliacaoBancaria.ToString("n2"),
                    SaldoFinalConciliacaoBancaria = planoDeConta.SaldoFinalConciliacaoBancaria.ToString("n2")
                };
                return new JsonpResult(dynPlanoConta);
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
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoDeConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoDeConta = repPlanoDeConta.BuscarPorCodigo(codigo);
                repPlanoDeConta.Deletar(planoDeConta, Auditado);
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

        #region Importação 
        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanoConta(unitOfWork);
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

                        Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPlano = (from obj in linha.Colunas where obj.NomeCampo == "Plano" select obj).FirstOrDefault();
                        planoConta.Plano = "";
                        if (colPlano != null)
                            planoConta.Plano = colPlano.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPlanoContabilidade = (from obj in linha.Colunas where obj.NomeCampo == "PlanoContabilidade" select obj).FirstOrDefault();
                        planoConta.PlanoContabilidade = null;
                        if (colPlanoContabilidade != null)
                            planoConta.PlanoContabilidade = colPlanoContabilidade.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricao = (from obj in linha.Colunas where obj.NomeCampo == "Descricao" select obj).FirstOrDefault();
                        planoConta.Descricao = null;
                        if (colDescricao != null)
                            planoConta.Descricao = colDescricao.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAnaliticoSintetico = (from obj in linha.Colunas where obj.NomeCampo == "AnaliticoSintetico" select obj).FirstOrDefault();
                        planoConta.AnaliticoSintetico = 0;
                        if (colAnaliticoSintetico != null)
                        {
                            if (colAnaliticoSintetico.Valor == "1" || colAnaliticoSintetico.Valor.ToLower() == "analitico" || colAnaliticoSintetico.Valor.ToLower() == "analítico")
                                planoConta.AnaliticoSintetico = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Analitico;
                            if (colAnaliticoSintetico.Valor == "2" || colAnaliticoSintetico.Valor.ToLower() == "sintetico" || colAnaliticoSintetico.Valor.ToLower() == "sintético")
                                planoConta.AnaliticoSintetico = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colReceitaDespesa = (from obj in linha.Colunas where obj.NomeCampo == "ReceitaDespesa" select obj).FirstOrDefault();
                        planoConta.ReceitaDespesa = 0;
                        if (colReceitaDespesa != null)
                        { 
                            if (colReceitaDespesa.Valor == "1" || colReceitaDespesa.Valor.ToLower() == "receita")
                                planoConta.ReceitaDespesa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ReceitaDespesa.Receita;
                            if (colReceitaDespesa.Valor == "2" || colReceitaDespesa.Valor.ToLower() == "despesa")
                                planoConta.ReceitaDespesa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ReceitaDespesa.Despesa;
                            if (colReceitaDespesa.Valor == "3" || colReceitaDespesa.Valor.ToLower() == "outros")
                                planoConta.ReceitaDespesa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ReceitaDespesa.Outros;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrupoDeResultado = (from obj in linha.Colunas where obj.NomeCampo == "GrupoDeResultado" select obj).FirstOrDefault();
                        planoConta.GrupoDeResultado = 0;
                        if (colGrupoDeResultado != null)
                        {
                            if (colReceitaDespesa.Valor == "0" || colReceitaDespesa.Valor.ToLower() == "nenhum")
                                planoConta.GrupoDeResultado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado.Nenhum;

                            if (colReceitaDespesa.Valor == "1" || colReceitaDespesa.Valor.ToLower() == "receitaoperacionalbruta")
                                planoConta.GrupoDeResultado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado.ReceitaOperacionalBruta;

                            if (colReceitaDespesa.Valor == "2" || colReceitaDespesa.Valor.ToLower() == "deducaoreceitabruta")
                                planoConta.GrupoDeResultado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado.DeducaoReceitaBruta;

                            if (colReceitaDespesa.Valor == "3" || colReceitaDespesa.Valor.ToLower() == "custovenda")
                                planoConta.GrupoDeResultado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado.CustoVenda;

                            if (colReceitaDespesa.Valor == "4" || colReceitaDespesa.Valor.ToLower() == "despesaoperacional")
                                planoConta.GrupoDeResultado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado.DespesaOperacional;

                            if (colReceitaDespesa.Valor == "5" || colReceitaDespesa.Valor.ToLower() == "resultadofinanceiro")
                                planoConta.GrupoDeResultado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado.ResultadoFinanceiro;

                            if (colReceitaDespesa.Valor == "6" || colReceitaDespesa.Valor.ToLower() == "resultadonaooperacional")
                                planoConta.GrupoDeResultado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado.ResultadoNaoOperacional;

                            if (colReceitaDespesa.Valor == "7" || colReceitaDespesa.Valor.ToLower() == "irpjcsll")
                                planoConta.GrupoDeResultado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado.IrpjCsll;

                            if (colReceitaDespesa.Valor == "8" || colReceitaDespesa.Valor.ToLower() == "investimento")
                                planoConta.GrupoDeResultado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado.Investimento;
                            
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSaldoInicialConciliacaoBancaria = (from obj in linha.Colunas where obj.NomeCampo == "SaldoInicialConciliacaoBancaria" select obj).FirstOrDefault();
                        planoConta.SaldoInicialConciliacaoBancaria = 0;
                        if (colSaldoInicialConciliacaoBancaria != null)
                            planoConta.SaldoInicialConciliacaoBancaria = decimal.Parse(colSaldoInicialConciliacaoBancaria.Valor);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSaldoFinalConciliacaoBancaria = (from obj in linha.Colunas where obj.NomeCampo == "SaldoFinalConciliacaoBancaria" select obj).FirstOrDefault();
                        planoConta.SaldoFinalConciliacaoBancaria = 0;
                        if (colSaldoFinalConciliacaoBancaria != null)
                            planoConta.SaldoFinalConciliacaoBancaria = decimal.Parse(colSaldoFinalConciliacaoBancaria.Valor);

                        planoConta.Ativo = true;
                        int codigoEmpresa = 0;
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                            codigoEmpresa = this.Usuario.Empresa.Codigo;

                        if (repPlanoConta.ContemPlanoConta(planoConta.Codigo, planoConta.Plano, codigoEmpresa))
                        {
                            retorno = "Esta numeração de plano de contas já está sendo utilizada.";
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                            unitOfWork.Rollback();
                        }
                        else
                        {
                            contador++;
                            repPlanoConta.Inserir(planoConta, Auditado);
                            
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, planoConta, null, "Inseriu por importação.", unitOfWork);

                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(Localization.Resources.Pessoas.Pessoa.OcorreuUmaFalhaAoProcessarLinha, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao); ;
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

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanoConta(unitOfWork);

            return new JsonpResult(configuracoes.ToList());
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPlanoConta(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Consultas.PlanoConta.Plano, Propriedade = "Plano", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Consultas.PlanoConta.PlanoContabilidade, Propriedade = "PlanoContabilidade", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Consultas.PlanoConta.Descricao, Propriedade = "Descricao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Consultas.PlanoConta.AnaliticoSintetico, Propriedade = "AnaliticoSintetico", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Consultas.PlanoConta.ReceitaDespesa, Propriedade = "ReceitaDespesa", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Consultas.PlanoConta.GrupoDeResultado, Propriedade = "GrupoDeResultado", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Consultas.PlanoConta.SaldoInicialConciliacaoBancaria, Propriedade = "SaldoInicialConciliacaoBancaria", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Consultas.PlanoConta.SaldoFinalConciliacaoBancaria, Propriedade = "SaldoFinalConciliacaoBancaria", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, processou = true };
            return retorno;
        }
        #endregion

        public async Task<IActionResult> ProximaNumeracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                string plano = Request.Params("Plano");

                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                int tamanhoPlano = 0;
                if (plano.Length == 0)
                    tamanhoPlano = 1;
                else if (plano.Length == 1)
                    tamanhoPlano = 3;
                else
                    tamanhoPlano = plano.Length + 3;

                Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoPai = repPlanoConta.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.PlanoConta> listaPlanoContas = repPlanoConta.BuscarProximoPlanoAnalitico(plano, tamanhoPlano, codigoEmpresa);

                string proximoPlano = plano;
                if (listaPlanoContas.Count > 0 && !string.IsNullOrWhiteSpace(listaPlanoContas[0].Plano))
                    proximoPlano = listaPlanoContas[0].Plano;
                else
                {
                    if (proximoPlano.Length == 1)
                        proximoPlano = proximoPlano + ".0";
                    else
                        proximoPlano = proximoPlano + ".00";
                }

                if (proximoPlano.Length == 1)
                    proximoPlano = Convert.ToString(Convert.ToInt32(proximoPlano) + 1);
                else if (proximoPlano.Length == 3)
                    proximoPlano = proximoPlano.Substring(0, 2) + Convert.ToString(Convert.ToInt32(proximoPlano.Substring(2, 1)) + 1);
                else
                    proximoPlano = proximoPlano.Substring(0, (proximoPlano.Length - 2)) + Convert.ToString(Convert.ToInt32(proximoPlano.Substring((proximoPlano.Length - 2), 2)) + 1).PadLeft(2, '0');

                var dynRetorno = new
                {
                    Plano = proximoPlano,
                    PlanoContaSintetico = planoPai != null ? "(" + planoPai.Plano + ") - " + planoPai.Descricao : string.Empty
                };
                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a próxima numeração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
