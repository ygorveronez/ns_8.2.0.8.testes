using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/MovimentoFinanceiro", "PagamentosMotoristas/PagamentoMotoristaTMS")]
    public class MovimentoFinanceiroController : BaseController
    {
		#region Construtores

		public MovimentoFinanceiroController(Conexao conexao) : base(conexao) { }

		#endregion

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
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/MovimentoFinanceiro");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Criar))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int planoDebito, planoCredito, tipoMovimento = 0;
                int.TryParse(Request.Params("PlanoDebito"), out planoDebito);
                int.TryParse(Request.Params("PlanoCredito"), out planoCredito);
                int.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);

                DateTime dataMovimento, dataGeracaoMovimento = DateTime.Now;
                DateTime.TryParse(Request.Params("DataMovimento"), out dataMovimento);
                DateTime.TryParse(Request.Params("DataBase"), out DateTime dataBase);

                decimal.TryParse(Request.Params("ValorMovimento"), out decimal valorMovimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipo;
                Enum.TryParse(Request.Params("TipoDocumento"), out tipo);

                if (Servicos.Embarcador.Financeiro.FechamentoDiario.VerificarSeExisteFechamento(0, dataMovimento, unitOfWork, tipo))
                    return new JsonpResult(false, true, "Já existe um fechamento diário igual ou posterior à data de " + dataMovimento.ToString("dd/MM/yyyy") + ", não sendo possível adicionar o movimento financeiro.");

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanceiro = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro();

                PreencherMovimentoFinanceiro(movimentoFinanceiro, unitOfWork);

                movimentoFinanceiro.DataGeracaoMovimento = dataGeracaoMovimento;
                movimentoFinanceiro.DataMovimento = dataMovimento;
                movimentoFinanceiro.Valor = valorMovimento;
                movimentoFinanceiro.TipoDocumentoMovimento = tipo;
                movimentoFinanceiro.DataBase = dataBase;

                if (planoDebito > 0)
                    movimentoFinanceiro.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo(planoDebito);

                if (planoCredito > 0)
                    movimentoFinanceiro.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo(planoCredito);

                if (tipoMovimento > 0)
                {
                    movimentoFinanceiro.TipoMovimento = repTipoMovimento.BuscarPorCodigo(tipoMovimento);

                    if (!string.IsNullOrWhiteSpace(movimentoFinanceiro.TipoMovimento.Observacao))
                        movimentoFinanceiro.Observacao = movimentoFinanceiro.TipoMovimento.Observacao + " - " + movimentoFinanceiro.Observacao;
                }

                if (planoDebito == planoCredito)
                    throw new ControllerException("Atenção! Não é permitido cadastrar com a mesma conta em entrada e saída.");

                if (movimentoFinanceiro.PlanoDeContaCredito.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                    throw new ControllerException("Atenção! Não é permitido realizar movimentações para uma conta sintética");

                if (movimentoFinanceiro.PlanoDeContaDebito.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                    throw new ControllerException("Atenção! Não é permitido realizar movimentações para uma conta sintética");

                repMovimentoFinanceiro.Inserir(movimentoFinanceiro, Auditado);

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoFinanceiroDebito = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito();
                movimentoFinanceiroDebito.DataGeracaoMovimento = dataGeracaoMovimento;
                movimentoFinanceiroDebito.DataMovimento = dataMovimento;
                movimentoFinanceiroDebito.DebitoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito;
                movimentoFinanceiroDebito.MovimentoFinanceiro = movimentoFinanceiro;
                movimentoFinanceiroDebito.PlanoDeConta = repPlanoConta.BuscarPorCodigo(planoDebito);
                movimentoFinanceiroDebito.Valor = valorMovimento;

                repMovimentoFinanceiroDebitoCredito.Inserir(movimentoFinanceiroDebito, Auditado);

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoFinanceiroCredito = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito();
                movimentoFinanceiroCredito.DataGeracaoMovimento = dataGeracaoMovimento;
                movimentoFinanceiroCredito.DataMovimento = dataMovimento;
                movimentoFinanceiroCredito.DebitoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito;
                movimentoFinanceiroCredito.MovimentoFinanceiro = movimentoFinanceiro;
                movimentoFinanceiroCredito.PlanoDeConta = repPlanoConta.BuscarPorCodigo(planoCredito);
                movimentoFinanceiroCredito.Valor = valorMovimento;

                repMovimentoFinanceiroDebitoCredito.Inserir(movimentoFinanceiroCredito, Auditado);

                unitOfWork.CommitChanges();

                var dynTipoMovimento = new
                {
                    movimentoFinanceiro.Codigo,
                    TipoDocumento = movimentoFinanceiro.Observacao,
                    NumeroDocumento = movimentoFinanceiro.Documento,
                    Valor = movimentoFinanceiro.Valor.ToString("n2"),
                    NaoGerarRateioDeDespesaPorVeiculo = movimentoFinanceiro.TipoMovimento?.NaoGerarRateioDeDespesaPorVeiculo ?? false,
                    Pessoa = new
                    {
                        Codigo = movimentoFinanceiro.Pessoa?.Codigo ?? 0,
                        Descricao = movimentoFinanceiro.Pessoa?.Descricao ?? string.Empty,
                    }
                };
                return new JsonpResult(dynTipoMovimento);
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/MovimentoFinanceiro");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Alterar))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo, planoDebito, planoCredito, tipoMovimento = 0;
                int.TryParse(Request.Params("PlanoDebito"), out planoDebito);
                int.TryParse(Request.Params("PlanoCredito"), out planoCredito);
                int.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);
                int.TryParse(Request.Params("Codigo"), out codigo);

                DateTime.TryParse(Request.Params("DataBase"), out DateTime dataBase);
                DateTime.TryParse(Request.Params("DataMovimento"), out DateTime dataMovimento);

                decimal.TryParse(Request.Params("ValorMovimento"), out decimal valorMovimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipo;
                Enum.TryParse(Request.Params("TipoDocumento"), out tipo);

                if (Servicos.Embarcador.Financeiro.FechamentoDiario.VerificarSeExisteFechamento(0, dataMovimento, unitOfWork, tipo))
                    return new JsonpResult(false, true, "Já existe um fechamento diário igual ou posterior à data de " + dataMovimento.ToString("dd/MM/yyyy") + ", não sendo possível atualizar o movimento financeiro.");

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanceiro = repMovimentoFinanceiro.BuscarPorCodigo(codigo, true);

                PreencherMovimentoFinanceiro(movimentoFinanceiro, unitOfWork);

                movimentoFinanceiro.DataMovimento = dataMovimento;
                movimentoFinanceiro.Valor = valorMovimento;
                movimentoFinanceiro.TipoDocumentoMovimento = tipo;
                movimentoFinanceiro.DataBase = dataBase;

                if (planoDebito > 0)
                    movimentoFinanceiro.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo(planoDebito);

                if (planoCredito > 0)
                    movimentoFinanceiro.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo(planoCredito);

                if (tipoMovimento > 0)
                    movimentoFinanceiro.TipoMovimento = repTipoMovimento.BuscarPorCodigo(tipoMovimento);

                if (planoDebito == planoCredito)
                    throw new ControllerException("Atenção! Não é permitido cadastrar com a mesma conta em entrada e saída.");

                if (movimentoFinanceiro.PlanoDeContaCredito.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                    throw new ControllerException("Atenção! Não é permitido realizar movimentações para uma conta sintética");

                if (movimentoFinanceiro.PlanoDeContaDebito.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                    throw new ControllerException("Atenção! Não é permitido realizar movimentações para uma conta sintética");

                if (movimentoFinanceiro.TipoGeracaoMovimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Automatica)
                    throw new ControllerException("Atenção! Não é permitido modificar movimentos gerados automaticamente");

                repMovimentoFinanceiro.Atualizar(movimentoFinanceiro, Auditado);

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoFinanceiroDebito = repMovimentoFinanceiroDebitoCredito.BuscarPorMovimentoFinanceiro(movimentoFinanceiro.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito);
                movimentoFinanceiroDebito.Initialize();
                movimentoFinanceiroDebito.DataMovimento = dataMovimento;
                movimentoFinanceiroDebito.MovimentoFinanceiro = movimentoFinanceiro;
                movimentoFinanceiroDebito.PlanoDeConta = repPlanoConta.BuscarPorCodigo(planoDebito);
                movimentoFinanceiroDebito.Valor = valorMovimento;

                if (movimentoFinanceiroDebito.MovimentoConcolidado)
                    throw new ControllerException("Não é possível alterar este movimento pois já se encontra conciliado.");

                repMovimentoFinanceiroDebitoCredito.Atualizar(movimentoFinanceiroDebito, Auditado);

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoFinanceiroCredito = repMovimentoFinanceiroDebitoCredito.BuscarPorMovimentoFinanceiro(movimentoFinanceiro.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito);
                movimentoFinanceiroCredito.Initialize();
                movimentoFinanceiroCredito.DataMovimento = dataMovimento;
                movimentoFinanceiroCredito.MovimentoFinanceiro = movimentoFinanceiro;
                movimentoFinanceiroCredito.PlanoDeConta = repPlanoConta.BuscarPorCodigo(planoCredito);
                movimentoFinanceiroCredito.Valor = valorMovimento;

                if (movimentoFinanceiroCredito.MovimentoConcolidado)
                    throw new ControllerException("Não é possível alterar este movimento pois já se encontra conciliado.");

                repMovimentoFinanceiroDebitoCredito.Atualizar(movimentoFinanceiroCredito, Auditado);

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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanceiro = repMovimentoFinanceiro.BuscarPorCodigo(codigo);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                if (movimentoFinanceiro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                string obsAcertoMotorista = "";
                if (movimentoFinanceiro.TipoDocumentoMovimento == TipoDocumentoMovimento.Acerto && movimentoFinanceiro.Documento.All(char.IsDigit))
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarPorDocumento(movimentoFinanceiro.Documento);
                    if (acertoViagem != null && acertoViagem?.Motorista != null)
                        obsAcertoMotorista = !movimentoFinanceiro.Observacao.Contains(" - Motorista: ") ? ((acertoViagem != null ? " - Motorista: " : string.Empty) + acertoViagem?.Motorista?.Descricao) : string.Empty;
                }
                    
                var dynMovimentoFinanceiro = new
                {
                    movimentoFinanceiro.Codigo,
                    DataMovimento = movimentoFinanceiro.DataMovimento.ToString("dd/MM/yyyy"),
                    ValorMovimento = movimentoFinanceiro.Valor.ToString("n2"),
                    TipoDocumento = movimentoFinanceiro.TipoDocumentoMovimento,
                    NumeroDocumento = movimentoFinanceiro.Documento,
                    Observacao = movimentoFinanceiro.Observacao + obsAcertoMotorista,
                    PlanoDebito = new { Codigo = movimentoFinanceiro.PlanoDeContaDebito != null ? movimentoFinanceiro.PlanoDeContaDebito.Codigo : 0, Descricao = movimentoFinanceiro.PlanoDeContaDebito != null ? "(" + movimentoFinanceiro.PlanoDeContaDebito.Plano + ") " + movimentoFinanceiro.PlanoDeContaDebito.Descricao : "" },
                    PlanoCredito = new { Codigo = movimentoFinanceiro.PlanoDeContaCredito != null ? movimentoFinanceiro.PlanoDeContaCredito.Codigo : 0, Descricao = movimentoFinanceiro.PlanoDeContaCredito != null ? "(" + movimentoFinanceiro.PlanoDeContaCredito.Plano + ") " + movimentoFinanceiro.PlanoDeContaCredito.Descricao : "" },
                    CentroResultado = new { Codigo = movimentoFinanceiro.CentroResultado != null ? movimentoFinanceiro.CentroResultado.Codigo : 0, Descricao = movimentoFinanceiro.CentroResultado != null ? "(" + movimentoFinanceiro.CentroResultado.Plano + ") " + movimentoFinanceiro.CentroResultado.Descricao : "" },
                    TipoMovimento = new { Codigo = movimentoFinanceiro.TipoMovimento != null ? movimentoFinanceiro.TipoMovimento.Codigo : 0, Descricao = movimentoFinanceiro.TipoMovimento != null ? movimentoFinanceiro.TipoMovimento.Descricao : "" },
                    TipoGeracao = movimentoFinanceiro.TipoGeracaoMovimento,
                    DataBase = movimentoFinanceiro.DataBase.ToString("dd/MM/yyyy"),
                    GrupoPessoa = new { Codigo = movimentoFinanceiro.GrupoPessoas != null ? movimentoFinanceiro.GrupoPessoas.Codigo : 0, Descricao = movimentoFinanceiro.GrupoPessoas != null ? movimentoFinanceiro.GrupoPessoas.Descricao : "" },
                    Pessoa = new { Codigo = movimentoFinanceiro.Pessoa != null ? movimentoFinanceiro.Pessoa.CPF_CNPJ : 0, Descricao = movimentoFinanceiro.Pessoa != null ? movimentoFinanceiro.Pessoa.Nome : "" },
                    movimentoFinanceiro.MoedaCotacaoBancoCentral,
                    DataBaseCRT = movimentoFinanceiro.DataBaseCRT?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    ValorMoedaCotacao = movimentoFinanceiro.ValorMoedaCotacao.ToString("n10"),
                    ValorOriginalMoedaEstrangeira = movimentoFinanceiro.ValorOriginalMoedaEstrangeira.ToString("n2"),
                    TipoDespesaFinanceira = new { Codigo = movimentoFinanceiro.TipoDespesaFinanceira?.Codigo ?? 0, Descricao = movimentoFinanceiro.TipoDespesaFinanceira?.Descricao ?? string.Empty },
                };

                return new JsonpResult(dynMovimentoFinanceiro);
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/MovimentoFinanceiro");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Excluir))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                //Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);
                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento repRateioDespesaVeiculoLancamento = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanceiro = repMovimentoFinanceiro.BuscarPorCodigo(codigo);

                if (movimentoFinanceiro.TipoGeracaoMovimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Automatica)
                    return new JsonpResult(false, true, "Não é permitido excluir movimentos gerados automaticamente.");

                if (Servicos.Embarcador.Financeiro.FechamentoDiario.VerificarSeExisteFechamento(0, movimentoFinanceiro.DataMovimento, unitOfWork, movimentoFinanceiro.TipoDocumentoMovimento))
                    return new JsonpResult(false, true, "Já existe um fechamento diário igual ou posterior à data de " + movimentoFinanceiro.DataMovimento.ToString("dd/MM/yyyy") + ", não sendo possível excluir o movimento financeiro.");

                //Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoDebito = repMovimentoFinanceiroDebitoCredito.BuscarPorMovimentoFinanceiro(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito);
                //Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoCredito = repMovimentoFinanceiroDebitoCredito.BuscarPorMovimentoFinanceiro(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo = repRateioDespesaVeiculo.BuscarPorMovimentoFinanceiro(codigo);

                if (rateioDespesaVeiculo != null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, rateioDespesaVeiculo, null, "Excluído e revertido o rateio de despesa do veículo a partir do movimento financeiro", unitOfWork);

                    rateioDespesaVeiculo.Veiculos = null;
                    rateioDespesaVeiculo.SegmentosVeiculos = null;
                    rateioDespesaVeiculo.CentroResultados = null;

                    repRateioDespesaVeiculoLancamento.DeletarPorRateioDespesaVeiculo(rateioDespesaVeiculo.Codigo);
                    repRateioDespesaVeiculo.Deletar(rateioDespesaVeiculo);
                }

                //repMovimentoFinanceiroDebitoCredito.Deletar(movimentoDebito, Auditado);
                //repMovimentoFinanceiroDebitoCredito.Deletar(movimentoCredito, Auditado);
                repMovimentoFinanceiro.Deletar(movimentoFinanceiro, Auditado);

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

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRecibo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                bool movimentoManual = false;
                bool tituloReceber = false;
                bool tituloPagar = false;
                bool pagamentoMotorista = false;
                bool carga = false;
                bool.TryParse(Request.Params("PagamentoMotorista"), out pagamentoMotorista);
                bool.TryParse(Request.Params("TituloPagar"), out tituloPagar);
                bool.TryParse(Request.Params("TituloReceber"), out tituloReceber);
                bool.TryParse(Request.Params("MovimentoManual"), out movimentoManual);
                bool.TryParse(Request.Params("Carga"), out carga);

                int codigo = int.Parse(Request.Params("Codigo"));
                if (codigo == 0)
                    return new JsonpResult(false, false, "Favor selecione um registro para gerar o seu relatório.");

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R107_Recibo, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R107_Recibo, TipoServicoMultisoftware, "Relatorio de Recibo", "MovimentoFinanceiro", "ReciboFinanceiro.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;

                List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro> dadosRecibo = null;
                if (pagamentoMotorista || carga)
                {
                    var listaPrimeiraVia = repMovimentoFinanceiro.RelatorioReciboPagamentoMotorista(codigo, this.Usuario.Empresa?.Codigo ?? 0, configuracaoFinanceiro?.UtilizarEmpresaFilialImpressaoReciboPagamentoMotorista ?? false);
                    foreach (var primeiraVia in listaPrimeiraVia)
                    {
                        if (pagamentoMotorista)
                        {
                            primeiraVia.ObservacaoUsuario = primeiraVia.Observacao;
                            primeiraVia.Observacao = null;
                        }

                        if (dadosRecibo == null)
                            dadosRecibo = new List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro>();

                        dadosRecibo.Add(primeiraVia);
                    }

                    dadosRecibo.Add(new Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro()
                    {
                        Via = 2,
                        Acrescimo = dadosRecibo[0].Acrescimo,
                        CNPJEmpresa = dadosRecibo[0].CNPJEmpresa,
                        CNPJPessoa = dadosRecibo[0].CNPJPessoa,
                        Data = dadosRecibo[0].Data,
                        Desconto = dadosRecibo[0].Desconto,
                        Documento = dadosRecibo[0].Documento,
                        NomeEmpresa = dadosRecibo[0].NomeEmpresa,
                        Observacao = dadosRecibo[0].Observacao,
                        Parcela = dadosRecibo[0].Parcela,
                        Pessoa = dadosRecibo[0].Pessoa,
                        TipoDocumento = dadosRecibo[0].TipoDocumento,
                        ValorPago = dadosRecibo[0].ValorPago,
                        ValorTotal = dadosRecibo[0].ValorTotal,
                        NumeroCartao = dadosRecibo[0].NumeroCartao,
                        ObservacaoUsuario = dadosRecibo[0].ObservacaoUsuario
                    }
                    );
                }
                else if (movimentoManual) // TODO: ToList Cast
                    dadosRecibo = repMovimentoFinanceiro.RelatorioReciboMovimento(codigo, this.Usuario.Empresa?.Codigo ?? 0).ToList();
                else
                    dadosRecibo = repMovimentoFinanceiro.RelatorioReciboBaixa(codigo, this.Usuario.Empresa?.Codigo ?? 0).ToList();

                if (dadosRecibo.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioRecibo(pagamentoMotorista, movimentoManual, tituloReceber, tituloPagar, carga, codigo, nomeCliente, stringConexao, relatorioControleGeracao, dadosRecibo));
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, false, "Nenhum registro para regar o relatório.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidaMovimentoFinanceiroDuplicado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                DateTime dataMovimento = Request.GetDateTimeParam("DataMovimento");
                decimal valorMovimento = Request.GetDecimalParam("ValorMovimento");
                string numeroDocumento = Request.GetStringParam("NumeroDocumento");

                System.Threading.Thread.Sleep(500);//Para aguardar fechar a confirmação de Plano Orçamentário

                if (ConfiguracaoEmbarcador.SolicitarConfirmacaoMovimentoFinanceiroDuplicado && repMovimentoFinanceiro.ContemMovimentoFinanceiroMesmaDataValorNumeroDocumento(dataMovimento, valorMovimento, numeroDocumento, codigo))
                    return new JsonpResult(new { MovimentoDuplicado = true });

                return new JsonpResult(new { MovimentoDuplicado = false });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao validar movimento financeiro duplicado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarObservacao()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanciamento = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanciamento = repMovimentoFinanciamento.BuscarPorCodigo(codigo, true);

                if (movimentoFinanciamento == null)
                    return new JsonpResult(false, true, "Movimento Financeiro não encontrado.");

                unitOfWork.Start();

                movimentoFinanciamento.Observacao = observacao;
                repMovimentoFinanciamento.Atualizar(movimentoFinanciamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult("Observação alterada com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a observação do movimento financeiro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherMovimentoFinanceiro(Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanceiro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesaFinanceira = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

            int codigoCentroResultado = Request.GetIntParam("CentroResultado");
            int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoa");
            int codigoTipoDespesa = Request.GetIntParam("TipoDespesaFinanceira");

            double cnpjPessoa = Request.GetDoubleParam("Pessoa");

            movimentoFinanceiro.Documento = Request.GetStringParam("NumeroDocumento");
            movimentoFinanceiro.Observacao = Request.GetStringParam("Observacao");

            movimentoFinanceiro.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
            movimentoFinanceiro.GrupoPessoas = codigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa) : null;
            movimentoFinanceiro.Pessoa = cnpjPessoa > 0 ? repPessoa.BuscarPorCPFCNPJ(cnpjPessoa) : null;
            movimentoFinanceiro.TipoDespesaFinanceira = codigoTipoDespesa > 0 ? repTipoDespesaFinanceira.BuscarPorCodigo(codigoTipoDespesa) : null;

            if (movimentoFinanceiro.Codigo == 0)
            {
                movimentoFinanceiro.TipoGeracaoMovimento = TipoGeracaoMovimento.Manual;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    movimentoFinanceiro.Empresa = this.Usuario.Empresa;
                    movimentoFinanceiro.TipoAmbiente = this.Usuario.Empresa?.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Producao;
                }
            }

            movimentoFinanceiro.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
            movimentoFinanceiro.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
            movimentoFinanceiro.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
            movimentoFinanceiro.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentoFinanceiro filtrosPesquisa = ObterFiltrosPesquisa();
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> listaMovimentoFinanceiro = repMovimentoFinanceiro.Consultar(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMovimentoFinanceiro.ContarConsulta(filtrosPesquisa);

            var lista = (from p in listaMovimentoFinanceiro
                         select new
                         {
                             p.Codigo,
                             DataMovimento = p.DataMovimento.ToString("dd/MM/yyyy"),
                             Valor = p.Valor.ToString("n2"),
                             p.Documento,
                             p.DescricaoTipoDocumentoMovimento,
                             PlanoDebito = p.PlanoDeContaDebito != null ? "(" + p.PlanoDeContaDebito.Plano + ") " + p.PlanoDeContaDebito.Descricao : string.Empty,
                             PlanoCredito = p.PlanoDeContaCredito != null ? "(" + p.PlanoDeContaCredito.Plano + ") " + p.PlanoDeContaCredito.Descricao : string.Empty,
                             Favorecido = p.Pessoa != null && p.GrupoPessoas != null ? p.Pessoa.Nome + " - " + p.GrupoPessoas.Descricao : p.Pessoa != null ? p.Pessoa.Nome : p.GrupoPessoas != null ? p.GrupoPessoas.Descricao : string.Empty,
                             p.Observacao,
                             MoedaCotacaoBancoCentral = p.MoedaCotacaoBancoCentral?.ObterDescricao() ?? string.Empty,
                             ValorOriginalMoedaEstrangeira = p.ValorOriginalMoedaEstrangeira.ToString("n2"),
                             DT_RowColor = p.MovimentosFinanceirosDebitoCredito.Any(M => M.MovimentoConcolidado) ? CorGrid.Verde : CorGrid.Branco
                         }).ToList();

            return lista.ToList();
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Código", "Codigo", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Data", "DataMovimento", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, true);
            if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
            {
                grid.AdicionarCabecalho("Valor Original Moeda", "ValorOriginalMoedaEstrangeira", 6, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Moeda", "MoedaCotacaoBancoCentral", 6, Models.Grid.Align.left, false);
            }
            grid.AdicionarCabecalho("Nº Documento", "Documento", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo Doc.", "DescricaoTipoDocumentoMovimento", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Conta de Entrada", "PlanoDebito", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Conta de Saída", "PlanoCredito", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Favorecido", "Favorecido", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 10, Models.Grid.Align.left, true);

            return grid;
        }

        private void GerarRelatorioRecibo(bool pagamentoMotorista, bool movimentoFinanceiro, bool baixaReceber, bool baixaPagar, bool carga, int codigo, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro> dadosRecibo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

            try
            {
                ReportRequest.WithType(ReportType.ReciboFinanceiro)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("PagamentoMotorista", pagamentoMotorista)
                    .AddExtraData("MovimentoFinanceiro", movimentoFinanceiro)
                    .AddExtraData("Infracao", false)
                    .AddExtraData("BaixaPagar", baixaPagar)
                    .AddExtraData("Carga", carga)
                    .AddExtraData("NomeEmpresa", nomeEmpresa)
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("DadosRecibo", dadosRecibo.ToJson())
                    .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                    .CallReport();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentoFinanceiro ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentoFinanceiro()
            {
                DataMovimentoInicial = Request.GetDateTimeParam("DataMovimentoInicial"),
                DataMovimentoFinal = Request.GetDateTimeParam("DataMovimentoFinal"),
                DataBase = Request.GetDateTimeParam("DataBase"),
                ValorMovimento = Request.GetDecimalParam("ValorMovimento"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                Observacao = Request.GetStringParam("Observacao"),
                CodigoMovimento = Request.GetIntParam("Codigo"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                CodigoPlanoDebito = Request.GetIntParam("PlanoDebito"),
                CodigoPlanoCredito = Request.GetIntParam("PlanoCredito"),
                CodigoTipoMovimento = Request.GetIntParam("TipoMovimento"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                CnpjPessoa = Request.GetDoubleParam("Pessoa"),
                TipoDocumento = Request.GetEnumParam<TipoDocumentoMovimento>("TipoDocumento"),
                SituacaoMovimento = Request.GetNullableEnumParam<TipoConsolidacaoMovimentoFinanceiro>("SituacaoMovimento"),
                MoedaCotacaoBancoCentral = Request.GetEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral"),
                TipoAmbiente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.TipoAmbiente : Dominio.Enumeradores.TipoAmbiente.Nenhum,
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                VisualizarTitulosPagamentoSalario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? Usuario.PermiteVisualizarTitulosPagamentoSalario : true
            };
        }

        #endregion
    }
}
