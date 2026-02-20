using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/GeracaoMovimentoLoteCentroResultado")]
    public class GeracaoMovimentoLoteCentroResultadoController : BaseController
    {
		#region Construtores

		public GeracaoMovimentoLoteCentroResultadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> GerarMovimentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/MovimentoFinanceiro");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Criar))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                dynamic centrosResultado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CentrosResultado"));

                if (centrosResultado == null || centrosResultado.Count == 0)
                    return new JsonpResult(false, true, "É necessário selecionar ao menos um centro de resultados para a geração dos movimentos.");

                int planoDebito = Request.GetIntParam("PlanoDebito");
                int planoCredito = Request.GetIntParam("PlanoCredito");
                int tipoMovimento = Request.GetIntParam("TipoMovimento");
                int grupoPessoas = Request.GetIntParam("GrupoPessoa");

                double pessoa = Request.GetDoubleParam("Pessoa");

                decimal valorTotalMovimento = Request.GetDecimalParam("ValorMovimento");

                DateTime dataGeracaoMovimento = DateTime.Now;
                DateTime dataMovimento = Request.GetDateTimeParam("DataMovimento");
                DateTime dataBase = Request.GetDateTimeParam("DataBase");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento>("TipoDocumento");

                string numeroDocumento = Request.Params("NumeroDocumento");
                string observacao = Request.Params("Observacao");

                if (Servicos.Embarcador.Financeiro.FechamentoDiario.VerificarSeExisteFechamento(0, dataMovimento, unitOfWork, tipo))
                    return new JsonpResult(false, true, "Já existe um fechamento diário igual ou posterior à data de " + dataMovimento.ToString("dd/MM/yyyy") + ", não sendo possível adicionar os movimentos financeiros.");

                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                decimal percentualTotalRateio = 0m, valorTotalRateado = 0m;

                int countCentrosResultado = centrosResultado.Count;

                unitOfWork.Start();

                for (int i = 0; i < countCentrosResultado; i++)
                {
                    dynamic centroResultado = centrosResultado[i];

                    decimal percentual = (decimal)centroResultado.Percentual;
                    int codigoCentroResultado = (int)centroResultado.CodigoCentroResultado;

                    percentualTotalRateio += percentual;

                    decimal valorMovimento = 0m;

                    if ((i + 1) == countCentrosResultado)
                        valorMovimento = valorTotalMovimento - valorTotalRateado;
                    else
                        valorMovimento = Math.Round(Math.Floor(valorTotalMovimento * percentual) / 100, 2, MidpointRounding.AwayFromZero);

                    valorTotalRateado += valorMovimento;

                    Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanceiro = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro
                    {
                        DataGeracaoMovimento = dataGeracaoMovimento,
                        DataMovimento = dataMovimento,
                        Valor = valorMovimento,
                        TipoDocumentoMovimento = tipo,
                        Documento = numeroDocumento,
                        Empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo),
                        TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Manual,
                        DataBase = dataBase,
                        GrupoPessoas = grupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(grupoPessoas) : null,
                        Pessoa = pessoa > 0d ? repPessoa.BuscarPorCPFCNPJ(pessoa) : null
                    };

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        movimentoFinanceiro.TipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                    if (planoDebito > 0)
                        movimentoFinanceiro.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo(planoDebito);

                    if (planoCredito > 0)
                        movimentoFinanceiro.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo(planoCredito);

                    if (codigoCentroResultado > 0)
                        movimentoFinanceiro.CentroResultado = repCentroResultado.BuscarPorCodigo(codigoCentroResultado);

                    if (tipoMovimento > 0)
                    {
                        movimentoFinanceiro.TipoMovimento = repTipoMovimento.BuscarPorCodigo(tipoMovimento);

                        if (!string.IsNullOrWhiteSpace(movimentoFinanceiro.TipoMovimento.Observacao))
                            movimentoFinanceiro.Observacao = movimentoFinanceiro.TipoMovimento.Observacao;
                    }

                    if (!string.IsNullOrWhiteSpace(movimentoFinanceiro.Observacao) && !string.IsNullOrWhiteSpace(observacao))
                        movimentoFinanceiro.Observacao += " - ";

                    movimentoFinanceiro.Observacao += observacao;

                    if (planoDebito == planoCredito)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Não é permitido cadastrar com a mesma conta em entrada e saída.");
                    }

                    if (movimentoFinanceiro.PlanoDeContaCredito.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico ||
                        movimentoFinanceiro.PlanoDeContaDebito.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Sintetico)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Não é permitido realizar movimentações para uma conta sintética.");
                    }

                    repMovimentoFinanceiro.Inserir(movimentoFinanceiro, Auditado);

                    Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoFinanceiroDebito = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito
                    {
                        DataGeracaoMovimento = dataGeracaoMovimento,
                        DataMovimento = dataMovimento,
                        DebitoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito,
                        MovimentoFinanceiro = movimentoFinanceiro,
                        PlanoDeConta = repPlanoConta.BuscarPorCodigo(planoDebito),
                        Valor = valorMovimento
                    };

                    repMovimentoFinanceiroDebitoCredito.Inserir(movimentoFinanceiroDebito, Auditado);

                    Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoFinanceiroCredito = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito
                    {
                        DataGeracaoMovimento = dataGeracaoMovimento,
                        DataMovimento = dataMovimento,
                        DebitoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito,
                        MovimentoFinanceiro = movimentoFinanceiro,
                        PlanoDeConta = repPlanoConta.BuscarPorCodigo(planoCredito),
                        Valor = valorMovimento
                    };

                    repMovimentoFinanceiroDebitoCredito.Inserir(movimentoFinanceiroCredito, Auditado);
                }

                if (valorTotalRateado != valorTotalMovimento)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Ocorreu uma falha ao ratear o valor do movimento entre os centros de resultado.");
                }

                if (percentualTotalRateio != 100m)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "O percentual rateado entre os centros de resultado deve ser de 100%. O percentual informado é de " + percentualTotalRateio.ToString("n2") + "%.");
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar as movimentações financeiras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
