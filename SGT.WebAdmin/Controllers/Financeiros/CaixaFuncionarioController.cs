using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/CaixaFuncionario")]
    public class CaixaFuncionarioController : BaseController
    {
		#region Construtores

		public CaixaFuncionarioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa situacaoCaixa;
                Enum.TryParse(Request.Params("SituacaoCaixa"), out situacaoCaixa);
                int codigoUsuario = this.Usuario.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "DataAbertura", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacaoCaixa", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Descricao")
                    propOrdenar = "PlanoConta.Descricao";

                Repositorio.Embarcador.Usuarios.CaixaFuncionario repCaixaFuncionario = new Repositorio.Embarcador.Usuarios.CaixaFuncionario(unitOfWork);
                List<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario> listaCaixaFuncionario = repCaixaFuncionario.Consultar(codigoUsuario, situacaoCaixa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCaixaFuncionario.ContarConsulta(codigoUsuario, situacaoCaixa));

                var lista = (from p in listaCaixaFuncionario
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 DataAbertura = p.DataAbertura.HasValue ? p.DataAbertura.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 p.DescricaoSituacaoCaixa
                             }).ToList();

                grid.AdicionaRows(lista);
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
        public async Task<IActionResult> PesquisaMovimentacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Usuarios.CaixaFuncionario repCaixaFuncionario = new Repositorio.Embarcador.Usuarios.CaixaFuncionario(unitOfWork);
                Repositorio.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro repCaixaFuncionarioMovimentoFinanceiro = new Repositorio.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));
                int codigoUsuario = this.Usuario.Codigo;
                Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario caixa = null;
                if (codigo > 0)
                    caixa = repCaixaFuncionario.BuscarPorCodigo(codigo);
                if (caixa == null || caixa.SituacaoCaixa != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Fechado)
                {
                    if (this.Usuario.PlanoConta != null && this.Usuario.SituacaoCaixa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Aberto)
                        caixa = this.Usuario.CaixaFuncionario;
                    else
                        caixa = repCaixaFuncionario.BuscarPorCaixaAbertoFuncionario(codigoUsuario);
                }
                int codigoPlanoConta = this.Usuario.PlanoConta?.Codigo ?? 0;
                DateTime dataConsulta = caixa != null && caixa.DataAbertura.HasValue ? caixa.DataAbertura.Value.Date : DateTime.Now.Date;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código", "Codigo", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data", "DataMovimento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Doc.", "Documento", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Entrada", "Entrada", 9, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Saída", "Saida", 9, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito> listaMovimentos = null;
                if (codigo > 0 && (caixa == null || caixa.SituacaoCaixa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Fechado))
                {
                    listaMovimentos = repCaixaFuncionarioMovimentoFinanceiro.Consultar(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    grid.setarQuantidadeTotal(repCaixaFuncionarioMovimentoFinanceiro.ContarConsulta(codigo));
                }
                else
                {
                    listaMovimentos = repMovimentoFinanceiro.Consultar(codigoPlanoConta, dataConsulta, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    grid.setarQuantidadeTotal(repMovimentoFinanceiro.ContarConsulta(codigoPlanoConta, dataConsulta));
                }

                var lista = (from p in listaMovimentos
                             select new
                             {
                                 p.MovimentoFinanceiro.Codigo,
                                 DataMovimento = p.DataMovimento.ToString("dd/MM/yyyy"),
                                 p.MovimentoFinanceiro.Documento,
                                 p.MovimentoFinanceiro.Observacao,
                                 Entrada = p.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito ? p.Valor : 0,
                                 Saida = p.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito ? p.Valor : 0
                             }).ToList();

                grid.AdicionaRows(lista);
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
        public async Task<IActionResult> PesquisaMovimentacoesCaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa repCaixaFuncionarioValorCaixa = new Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa(unitOfWork);

                int codigo = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoValorCaixa", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 9, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa> listaMovimentosCaixa = repCaixaFuncionarioValorCaixa.Consultar(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCaixaFuncionarioValorCaixa.ContarConsulta(codigo));

                var lista = (from p in listaMovimentosCaixa
                             select new
                             {
                                 CodigoValorCaixa = p.Codigo,
                                 p.Codigo,
                                 Data = p.DataLancamento.HasValue ? p.DataLancamento.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 p.Descricao,
                                 p.Valor
                             }).ToList();

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Usuarios.CaixaFuncionario repCaixaFuncionario = new Repositorio.Embarcador.Usuarios.CaixaFuncionario(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario caixa = repCaixaFuncionario.BuscarPorCodigo(codigo);
                Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa repCaixaFuncionarioValorCaixa = new Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

                if (caixa == null)
                    return new JsonpResult(false, "Caixa não localizado.");
                if (caixa.SituacaoCaixa != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Fechado)
                    return new JsonpResult(false, "Só é possível visualizar caixa fechado.");

                var dynCaixaFuncionario = new
                {
                    Codigo = caixa.Codigo,
                    SituacaoCaixa = caixa.SituacaoCaixa,
                    Operador = caixa.Usuario.Descricao,
                    PlanoConta = caixa.PlanoConta.BuscarDescricao,
                    Status = "Fechado",
                    DataAbertura = caixa.DataAbertura.Value.ToString("dd/MM/yyy"),
                    SaldoInicial = caixa.SaldoInicial.ToString("n2"),
                    Observacao = caixa.Observacao,
                    Entradas = caixa.TotalEntradas.ToString("n2"),
                    Saidas = caixa.TotalSaidas.ToString("n2"),
                    Saldo = (caixa.TotalEntradas - caixa.TotalSaidas).ToString("n2"),
                    SaldoCaixa = caixa.SaldoNoCaixa.ToString("n2"),
                    SaldoFinal = caixa.SaldoFinal.ToString("n2"),
                    ValoresNoCaixa = caixa.SaldoNoCaixa.ToString("n2")
                };

                return new JsonpResult(dynCaixaFuncionario);
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCaixaFuncionario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoUsuario = this.Usuario.Codigo;
                Repositorio.Embarcador.Usuarios.CaixaFuncionario repCaixaFuncionario = new Repositorio.Embarcador.Usuarios.CaixaFuncionario(unitOfWork);
                Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa repCaixaFuncionarioValorCaixa = new Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario caixa = null;
                if (this.Usuario.PlanoConta != null && this.Usuario.SituacaoCaixa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Aberto)
                    caixa = this.Usuario.CaixaFuncionario;
                else
                    caixa = repCaixaFuncionario.BuscarPorCaixaAbertoFuncionario(codigoUsuario);

                decimal saldoInicial = 0, saldoCaixa = 0, saldoEntradas = 0, saldoSaidas = 0, saldoFinal = 0;

                if (this.Usuario.PlanoConta != null)
                {
                    saldoInicial = repMovimentoFinanceiro.BuscarSaldoAnteriorConta(this.Usuario.PlanoConta.Codigo, (caixa != null && caixa.DataAbertura.HasValue ? caixa.DataAbertura.Value.Date : DateTime.Now.Date));
                    saldoEntradas = repMovimentoFinanceiro.BuscarSaldoEntradas(this.Usuario.PlanoConta.Codigo, (caixa != null && caixa.DataAbertura.HasValue ? caixa.DataAbertura.Value.Date : DateTime.Now.Date));
                    saldoSaidas = repMovimentoFinanceiro.BuscarSaldoSaidas(this.Usuario.PlanoConta.Codigo, (caixa != null && caixa.DataAbertura.HasValue ? caixa.DataAbertura.Value.Date : DateTime.Now.Date));
                    saldoFinal = saldoInicial + saldoEntradas - saldoSaidas;
                }
                if (caixa != null)
                    saldoCaixa = repCaixaFuncionarioValorCaixa.ValoresLancadoCaixa(caixa.Codigo);

                var dynCaixaFuncionario = new
                {
                    Codigo = caixa?.Codigo ?? 0,
                    SituacaoCaixa = caixa?.SituacaoCaixa ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Fechado,
                    Operador = this.Usuario.Descricao,
                    PlanoConta = this.Usuario.PlanoConta?.BuscarDescricao ?? string.Empty,
                    Status = caixa == null ? "Fechado" : caixa.DescricaoSituacaoCaixa,
                    DataAbertura = caixa != null ? caixa.DataAbertura.HasValue ? caixa.DataAbertura.Value.ToString("dd/MM/yyy") : DateTime.Now.Date.ToString("dd/MM/yyy") : DateTime.Now.Date.ToString("dd/MM/yyy"),
                    SaldoInicial = saldoInicial.ToString("n2"),
                    Observacao = caixa?.Observacao ?? string.Empty,
                    Entradas = saldoEntradas.ToString("n2"),
                    Saidas = saldoSaidas.ToString("n2"),
                    Saldo = (saldoEntradas - saldoSaidas).ToString("n2"),
                    SaldoCaixa = saldoCaixa.ToString("n2"),
                    SaldoFinal = saldoFinal.ToString("n2"),
                    ValoresNoCaixa = saldoCaixa.ToString("n2")
                };

                return new JsonpResult(dynCaixaFuncionario);
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

        public async Task<IActionResult> AbrirCaixaFuncionario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoUsuario = this.Usuario.Codigo;
                if (this.Usuario.PlanoConta == null)
                    return new JsonpResult(false, "Seu usuário não possui conta vinculada.");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.CaixaFuncionario repCaixaFuncionario = new Repositorio.Embarcador.Usuarios.CaixaFuncionario(unitOfWork);
                Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa repCaixaFuncionarioValorCaixa = new Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

                unitOfWork.Start();

                decimal saldoInicial = 0, saldoCaixa = 0, saldoEntradas = 0, saldoSaidas = 0, saldoFinal = 0;

                if (this.Usuario.PlanoConta != null)
                {
                    saldoInicial = repMovimentoFinanceiro.BuscarSaldoAnteriorConta(this.Usuario.PlanoConta.Codigo, DateTime.Now.Date);
                    saldoEntradas = repMovimentoFinanceiro.BuscarSaldoEntradas(this.Usuario.PlanoConta.Codigo, DateTime.Now.Date);
                    saldoSaidas = repMovimentoFinanceiro.BuscarSaldoSaidas(this.Usuario.PlanoConta.Codigo, DateTime.Now.Date);
                    saldoFinal = saldoInicial + saldoEntradas - saldoSaidas;
                }

                Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario caixa = new Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario()
                {
                    DataAbertura = DateTime.Now.Date,
                    Observacao = Request.GetStringParam("Observacao"),
                    PlanoConta = this.Usuario.PlanoConta,
                    SaldoFinal = saldoFinal,
                    SaldoInicial = saldoInicial,
                    SaldoNoCaixa = saldoCaixa,
                    SituacaoCaixa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Aberto,
                    TotalEntradas = saldoEntradas,
                    TotalSaidas = saldoSaidas,
                    Usuario = this.Usuario
                };

                repCaixaFuncionario.Inserir(caixa);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(this.Usuario.Codigo);
                usuario.CaixaFuncionario = caixa;
                usuario.SituacaoCaixa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Aberto;
                repUsuario.Atualizar(usuario);

                unitOfWork.CommitChanges();

                var dynCaixaFuncionario = new
                {
                    Codigo = caixa?.Codigo ?? 0,
                    SituacaoCaixa = caixa?.SituacaoCaixa ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Fechado,
                    Operador = this.Usuario.Descricao,
                    PlanoConta = this.Usuario.PlanoConta?.BuscarDescricao ?? string.Empty,
                    Status = caixa == null ? "Fechado" : caixa.DescricaoSituacaoCaixa,
                    DataAbertura = caixa != null ? caixa.DataAbertura.HasValue ? caixa.DataAbertura.Value.ToString("dd/MM/yyy") : DateTime.Now.Date.ToString("dd/MM/yyy") : DateTime.Now.Date.ToString("dd/MM/yyy"),
                    SaldoInicial = saldoInicial.ToString("n2"),
                    Observacao = caixa?.Observacao ?? string.Empty,
                    Entradas = saldoEntradas.ToString("n2"),
                    Saidas = saldoSaidas.ToString("n2"),
                    Saldo = (saldoEntradas - saldoSaidas).ToString("n2"),
                    SaldoCaixa = saldoCaixa.ToString("n2"),
                    SaldoFinal = saldoFinal.ToString("n2"),
                    ValoresNoCaixa = saldoCaixa.ToString("n2")
                };
                return new JsonpResult(dynCaixaFuncionario);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao abrir o caixa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> FecharCaixaFuncionario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoUsuario = this.Usuario.Codigo;
                if (this.Usuario.PlanoConta == null)
                    return new JsonpResult(false, "Seu usuário não possui conta vinculada.");
                if (this.Usuario.CaixaFuncionario == null)
                    return new JsonpResult(false, "Seu usuário não possui caixa em aberto.");
                if (this.Usuario.SituacaoCaixa != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Aberto)
                    return new JsonpResult(false, "Seu usuário não possui caixa em aberto.");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.CaixaFuncionario repCaixaFuncionario = new Repositorio.Embarcador.Usuarios.CaixaFuncionario(unitOfWork);
                Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa repCaixaFuncionarioValorCaixa = new Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa(unitOfWork);
                Repositorio.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro repCaixaFuncionarioMovimentoFinanceiro = new Repositorio.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario caixa = repCaixaFuncionario.BuscarPorCodigo(this.Usuario.CaixaFuncionario.Codigo);

                decimal saldoInicial = 0, saldoCaixa = 0, saldoEntradas = 0, saldoSaidas = 0, saldoFinal = 0;

                saldoCaixa = repCaixaFuncionarioValorCaixa.ValoresLancadoCaixa(caixa.Codigo);
                saldoInicial = repMovimentoFinanceiro.BuscarSaldoAnteriorConta(caixa.PlanoConta.Codigo, caixa.DataAbertura.Value.Date);
                saldoEntradas = repMovimentoFinanceiro.BuscarSaldoEntradas(caixa.PlanoConta.Codigo, caixa.DataAbertura.Value.Date);
                saldoSaidas = repMovimentoFinanceiro.BuscarSaldoSaidas(caixa.PlanoConta.Codigo, caixa.DataAbertura.Value.Date);
                saldoFinal = saldoInicial + saldoEntradas - saldoSaidas;

                List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito> listaMovimentos = repMovimentoFinanceiro.BuscarMovimentosFinanceiro(caixa.PlanoConta.Codigo, caixa.DataAbertura.Value.Date);
                foreach (var movimento in listaMovimentos)
                {
                    Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro mov = new Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioMovimentoFinanceiro();
                    mov.CaixaFuncionario = caixa;
                    mov.MovimentoFinanceiroDebitoCredito = movimento;
                    mov.TipoEntradaSaida = movimento.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Saida : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Entrada;

                    repCaixaFuncionarioMovimentoFinanceiro.Inserir(mov);
                }

                if (saldoFinal < 0)
                    return new JsonpResult(false, "Atenção! O saldo final não pode ficar negativo para realizar o fechamento do caixa.");

                //removido por solicitação da transben
                //if (saldoFinal != saldoCaixa)
                //    return new JsonpResult(false, "Atenção! O saldo final deve ser igual ao valor no caixa lançado.");

                caixa.SaldoFinal = saldoFinal;
                caixa.SaldoInicial = saldoInicial;
                caixa.SaldoNoCaixa = saldoCaixa;
                caixa.SituacaoCaixa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Fechado;
                caixa.TotalEntradas = saldoEntradas;
                caixa.TotalSaidas = saldoSaidas;

                repCaixaFuncionario.Atualizar(caixa);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(this.Usuario.Codigo);
                usuario.CaixaFuncionario = null;
                usuario.SituacaoCaixa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Fechado;
                repUsuario.Atualizar(usuario);

                unitOfWork.CommitChanges();

                var dynCaixaFuncionario = new
                {
                    Codigo = caixa.Codigo
                };
                return new JsonpResult(dynCaixaFuncionario, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fechar o caixa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> AdicionarValorCaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoUsuario = this.Usuario.Codigo;
                if (this.Usuario.PlanoConta == null)
                    return new JsonpResult(false, "Seu usuário não possui conta vinculada.");

                Repositorio.Embarcador.Usuarios.CaixaFuncionario repCaixaFuncionario = new Repositorio.Embarcador.Usuarios.CaixaFuncionario(unitOfWork);
                Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa repCaixaFuncionarioValorCaixa = new Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa(unitOfWork);

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario caixa = repCaixaFuncionario.BuscarPorCodigo(Request.GetIntParam("Codigo"));
                if (caixa == null)
                    return new JsonpResult(false, "Caixa não localizado.");
                int codigoValorCaixa = Request.GetIntParam("CodigoValorCaixa");

                Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa valor = null;
                if (codigoValorCaixa == 0)
                {
                    valor = new Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa()
                    {
                        CaixaFuncionario = caixa,
                        DataLancamento = Request.GetDateTimeParam("Data"),
                        Descricao = Request.GetStringParam("Descricao"),
                        Valor = Request.GetDecimalParam("Valor")
                    };
                }
                else
                {
                    valor = repCaixaFuncionarioValorCaixa.BuscarPorCodigo(codigoValorCaixa);
                    valor.DataLancamento = Request.GetDateTimeParam("Data");
                    valor.Descricao = Request.GetStringParam("Descricao");
                    valor.Valor = Request.GetDecimalParam("Valor");
                }

                if (caixa.DataAbertura.Value.Date > valor.DataLancamento.Value.Date)
                    return new JsonpResult(false, true, "Atenção! A data informada é menor que a data de abertura do seu caixa.");
                if (valor.DataLancamento.Value.Date > DateTime.Now.Date)
                    return new JsonpResult(false, true, "Atenção! A data informada não pode ser maior que a data atual.");

                if (codigoValorCaixa == 0)
                    repCaixaFuncionarioValorCaixa.Inserir(valor);
                else
                    repCaixaFuncionarioValorCaixa.Atualizar(valor);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adivionar o valor.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverValorCaixa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoUsuario = this.Usuario.Codigo;
                if (this.Usuario.PlanoConta == null)
                    return new JsonpResult(false, "Seu usuário não possui conta vinculada.");

                Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa repCaixaFuncionarioValorCaixa = new Repositorio.Embarcador.Usuarios.CaixaFuncionarioValorCaixa(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa caixa = repCaixaFuncionarioValorCaixa.BuscarPorCodigo(Request.GetIntParam("Codigo"));
                repCaixaFuncionarioValorCaixa.Deletar(caixa);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover o valor lançado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarMovimentoFinanceiro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoUsuario = this.Usuario.Codigo;
                if (this.Usuario.PlanoConta == null)
                    return new JsonpResult(false, true, "Seu usuário não possui conta vinculada.");

                Repositorio.Embarcador.Usuarios.CaixaFuncionario repCaixaFuncionario = new Repositorio.Embarcador.Usuarios.CaixaFuncionario(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario caixa = repCaixaFuncionario.BuscarPorCodigo(Request.GetIntParam("Codigo"));
                if (caixa == null)
                    return new JsonpResult(false, true, "Caixa não localizado.");

                int centroCusto, planoDebito, planoCredito, tipoMovimento = 0;
                int.TryParse(Request.Params("CentroResultado"), out centroCusto);
                int.TryParse(Request.Params("PlanoDebito"), out planoDebito);
                int.TryParse(Request.Params("PlanoCredito"), out planoCredito);
                int.TryParse(Request.Params("TipoMovimento"), out tipoMovimento);

                DateTime dataMovimento, dataGeracaoMovimento = DateTime.Now;
                DateTime.TryParse(Request.Params("DataMovimento"), out dataMovimento);

                DateTime dataBase;
                DateTime.TryParse(Request.Params("DataBase"), out dataBase);
                int codigoGrupoPessoa = 0;
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);
                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                decimal valorMovimento = 0;
                decimal.TryParse(Request.Params("ValorMovimento"), out valorMovimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipo;
                Enum.TryParse(Request.Params("TipoDocumento"), out tipo);

                if (Servicos.Embarcador.Financeiro.FechamentoDiario.VerificarSeExisteFechamento(0, dataMovimento, unitOfWork, tipo))
                    return new JsonpResult(false, true, "Já existe um fechamento diário igual ou posterior à data de " + dataMovimento.ToString("dd/MM/yyyy") + ", não sendo possível adicionar o movimento financeiro.");

                if (caixa.DataAbertura.Value.Date > dataMovimento.Date)
                    return new JsonpResult(false, true, "Atenção! A data informada é menor que a data de abertura do seu caixa.");
                if (dataMovimento.Date > DateTime.Now.Date)
                    return new JsonpResult(false, true, "Atenção! A data informada não pode ser maior que a data atual.");

                Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanceiro = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro();

                movimentoFinanceiro.DataGeracaoMovimento = dataGeracaoMovimento;
                movimentoFinanceiro.DataMovimento = dataMovimento;
                movimentoFinanceiro.Valor = valorMovimento;
                movimentoFinanceiro.TipoDocumentoMovimento = tipo;
                movimentoFinanceiro.Documento = Request.Params("NumeroDocumento");
                movimentoFinanceiro.Observacao = Request.Params("Observacao");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    movimentoFinanceiro.Empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);
                movimentoFinanceiro.TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Manual;
                movimentoFinanceiro.DataBase = dataBase;
                movimentoFinanceiro.GrupoPessoas = null;
                movimentoFinanceiro.Pessoa = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    movimentoFinanceiro.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                if (planoDebito > 0)
                    movimentoFinanceiro.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo(planoDebito);

                if (planoCredito > 0)
                    movimentoFinanceiro.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo(planoCredito);

                if (centroCusto > 0)
                    movimentoFinanceiro.CentroResultado = repCentroResultado.BuscarPorCodigo(centroCusto);

                if (tipoMovimento > 0)
                {
                    movimentoFinanceiro.TipoMovimento = repTipoMovimento.BuscarPorCodigo(tipoMovimento);

                    if (!string.IsNullOrWhiteSpace(movimentoFinanceiro.TipoMovimento.Observacao))
                        movimentoFinanceiro.Observacao = movimentoFinanceiro.TipoMovimento.Observacao + " - " + movimentoFinanceiro.Observacao;
                }

                if (planoDebito == planoCredito)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Atenção! Não é permitido cadastrar com a mesma conta em entrada e saída.");
                }

                if (movimentoFinanceiro.PlanoDeContaCredito.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Atenção! Não é permitido realizar movimentações para uma conta sintética");
                }

                if (movimentoFinanceiro.PlanoDeContaDebito.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Atenção! Não é permitido realizar movimentações para uma conta sintética");
                }

                if (movimentoFinanceiro.PlanoDeContaDebito.Codigo != this.Usuario.PlanoConta.Codigo && movimentoFinanceiro.PlanoDeContaCredito.Codigo != this.Usuario.PlanoConta.Codigo)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Atenção! Não foi selecionado nenhuma conta que está vinculado ao seu usuário.");
                }

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
                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao abrir o caixa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BaixarRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                byte[] pdf = ReportRequest.WithType(ReportType.CaixaFuncionario)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoCaixa", Request.Params("Codigo"))
                    .AddExtraData("Detalhado", Request.Params("Detalhado"))
                    .CallReport()
                    .GetContentFile();

                // Retorna o arquivo
                return Arquivo(pdf, "application/pdf", "CaixaFuncionario.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
