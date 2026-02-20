using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
namespace SGT.WebAdmin.Controllers.Creditos
{
    [CustomAuthorize("Creditos/CreditoDisponivel")]
    public class CreditoExtraController : BaseController
    {
		#region Construtores

		public CreditoExtraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Creditor", "Creditor", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "ValorCreditoExtra", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Liberação", "DataLiberacao", 14, Models.Grid.Align.center, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "Creditor")
                    propOrdena += ".Nome";

                int codCreditoDisponivel = int.Parse(Request.Params("CreditoDisponivel"));

                Repositorio.Embarcador.Creditos.CreditoExtra repCreditoExtra = new Repositorio.Embarcador.Creditos.CreditoExtra(unitOfWork);
                List<Dominio.Entidades.Embarcador.Creditos.CreditoExtra> listaCreditoExtra = repCreditoExtra.Consultar(codCreditoDisponivel, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCreditoExtra.ContarConsulta(codCreditoDisponivel));
                var lista = (from p in listaCreditoExtra
                            select new
                            {
                                p.Codigo,
                                Creditor = p.Creditor.Nome,
                                ValorCreditoExtra = p.ValorCreditoExtra.ToString("n2"),
                                DataLiberacao = p.DataLiberacao.ToString("dd/MM/yyyy")
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
        
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Creditos.CreditoExtra repCreditoExtra = new Repositorio.Embarcador.Creditos.CreditoExtra(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);
                Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);

                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);

                List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> hierarquia = repHierarquiaSolicitacaoCredito.BuscarPorRecebedor(this.Usuario.Codigo);
                Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDisponivel = repCreditoDisponivel.BuscarPorCodigo(int.Parse(Request.Params("CreditoDisponivel")), true);

                if (creditoDisponivel != null)
                {

                    Dominio.Entidades.Embarcador.Creditos.CreditoExtra creditoExtra = new Dominio.Entidades.Embarcador.Creditos.CreditoExtra();
                    creditoExtra.Creditor = this.Usuario;
                    creditoExtra.ValorCreditoExtra = decimal.Parse(Request.Params("ValorCreditoExtra"));
                    creditoExtra.CreditoDisponivel = creditoDisponivel;
                    creditoExtra.Ativo = true;
                    repCreditoExtra.Inserir(creditoExtra, Auditado);

                    if (hierarquia.Count > 0)
                    {
                        if (repHierarquiaSolicitacaoCredito.BuscarPorCreditorRecebedor(creditoExtra.Creditor.Codigo, creditoDisponivel.Recebedor.Codigo) != null)
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado>>((string)Request.Params("CreditosUtilizados"));

                            if (creditosUtilizados != null)
                            {
                                string retorno = serCreditoMovimentacao.UtilizarCreditos(creditosUtilizados, creditoExtra, unitOfWork);
                                if (!string.IsNullOrWhiteSpace(retorno))
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, retorno);
                                }
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "Você não ter permissão de liberar crédito para este operador.");
                        }
                    }

                    creditoDisponivel.ValorSaldo += creditoExtra.ValorCreditoExtra;
                    creditoDisponivel.ValorCreditoExtra += creditoExtra.ValorCreditoExtra;
                    repCreditoDisponivel.Atualizar(creditoDisponivel, Auditado);


                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Crédito não encontrado para liberar um crédito Extra.");
                }

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

        //public async Task<IActionResult> Atualizar()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
        //    try
        //    {
        //        unitOfWork.Start();

        //        Repositorio.Embarcador.Creditos.CreditoExtra repCreditoExtra = new Repositorio.Embarcador.Creditos.CreditoExtra(unitOfWork);
        //        Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
        //        Repositorio.Embarcador.Creditos.CreditoExtraUtilizado repCreditoExtraUtilizado = new Repositorio.Embarcador.Creditos.CreditoExtraUtilizado(unitOfWork);
        //        Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);

        //        Dominio.Entidades.Embarcador.Creditos.CreditoExtra creditoExtra = repCreditoExtra.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));


        //        decimal diferencaValor = decimal.Parse(Request.Params("ValorCredito")) - creditoExtra.ValorCredito;
        //        decimal saldoRestante = (creditoExtra.ValorSaldo + creditoExtra.ValorComprometido) + diferencaValor;
        //        creditoExtra.Recebedor = new Dominio.Entidades.Usuario() { Codigo = int.Parse(Request.Params("Recebedor")) };

        //        if (saldoRestante >= 0)
        //        {
        //            creditoExtra.Creditor = this.Usuario;
        //            creditoExtra.ValorCredito = decimal.Parse(Request.Params("ValorCredito"));

        //            DateTime dataInicio;
        //            DateTime.TryParseExact(Request.Params("DataInicioCredito"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
        //            if (dataInicio != DateTime.MinValue)
        //                creditoExtra.DataInicioCredito = dataInicio;

        //            DateTime dataFim;
        //            DateTime.TryParseExact(Request.Params("DataFimCredito"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);
        //            if (dataFim != DateTime.MinValue)
        //                creditoExtra.DataFimCredito = dataFim;


        //            if (repHierarquiaSolicitacaoCredito.BuscarPorCreditorRecebedor(creditoExtra.Creditor.Codigo, creditoExtra.Recebedor.Codigo) != null)
        //            {
        //                Dominio.Entidades.Embarcador.Creditos.CreditoExtra creditoExtraExiste = repCreditoExtra.BuscarRecebedorPossuiCreditoAtivo(creditoExtra.Creditor.Codigo, creditoExtra.Recebedor.Codigo, creditoExtra.DataInicioCredito, creditoExtra.DataFimCredito);

        //                if (creditoExtraExiste == null || (creditoExtraExiste.Codigo == creditoExtra.Codigo))
        //                {
        //                    List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado> creditosUtilizados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Creditos.CreditoUtilizado>>((string)Request.Params("CreditosUtilizados"));

        //                    if (creditosUtilizados != null)
        //                    {
        //                        List<Dominio.Entidades.Embarcador.Creditos.CreditoExtraUtilizado> creditosUtilizadosDestino = repCreditoExtraUtilizado.BuscarPorCreditoExtraDestino(creditoExtra.Codigo);
        //                        serCreditoMovimentacao.ExtornarCreditos(creditosUtilizadosDestino, unitOfWork);

        //                        string retorno = serCreditoMovimentacao.UtilizarCreditos(creditosUtilizados, creditoExtra, unitOfWork);
        //                        if (!string.IsNullOrWhiteSpace(retorno))
        //                        {
        //                            unitOfWork.Rollback();
        //                            return new JsonpResult(false, true, retorno);
        //                        }
        //                    }
        //                    creditoExtra.ValorSaldo += diferencaValor;
        //                    repCreditoExtra.Atualizar(creditoExtra);
        //                    unitOfWork.CommitChanges();
        //                    return new JsonpResult(true);
        //                }
        //                else
        //                {
        //                    unitOfWork.Rollback();
        //                    return new JsonpResult(false, true, "Já existe um crédito liberado para esse operador no periodo informado");
        //                }
        //            }
        //            else
        //            {
        //                unitOfWork.Rollback();
        //                return new JsonpResult(false, true, "Você não ter permissão de liberar crédito para este operador.");
        //            }
        //        }
        //        else
        //        {
        //            unitOfWork.Rollback();
        //            decimal valorMinimo = decimal.Parse(Request.Params("ValorCredito")) + (-saldoRestante);
        //            return new JsonpResult(false, true, "O valor de crédito não pode ser menor que " + valorMinimo.ToString("n2") + ", já que este valor já foi utilizado ou está comprometido pelo operador.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        unitOfWork.Rollback();
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
        //    }
        //}

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Creditos.CreditoExtra repCreditoExtra = new Repositorio.Embarcador.Creditos.CreditoExtra(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
                Dominio.Entidades.Embarcador.Creditos.CreditoExtra creditoExtra = repCreditoExtra.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizados = repCreditoDisponivelUtilizado.BuscarPorCreditoExtra(creditoExtra.Codigo);

                var dynCreditoExtra = new
                {
                    creditoExtra.Codigo,
                    CreditoDisponivel = new { creditoExtra.CreditoDisponivel.Codigo, Descricao = "" },
                    Creditor = new { Codigo = creditoExtra.Creditor.Codigo, Descricao = creditoExtra.Creditor.Nome },
                    creditoExtra.ValorCreditoExtra,
                    CreditosUtilizados = from obj in creditosUtilizados
                                         select new
                                         {
                                             obj.Codigo,
                                             ValorUtilizado = obj.ValorUtilizado.ToString("n2"),
                                             Creditor = new { obj.CreditoDisponivelOrigem.Creditor.Codigo, Descricao = obj.CreditoDisponivelOrigem.Creditor.Nome }
                                         }
                };
                return new JsonpResult(dynCreditoExtra);
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
                
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Creditos.CreditoExtra repCreditoExtra = new Repositorio.Embarcador.Creditos.CreditoExtra(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);

                Dominio.Entidades.Embarcador.Creditos.CreditoExtra creditoExtra = repCreditoExtra.BuscarPorCodigo(codigo);
                Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);
                
                if (creditoExtra.CreditoDisponivel.ValorSaldo >= creditoExtra.ValorCreditoExtra)
                {
                    List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> creditosUtilizados = repCreditoDisponivelUtilizado.BuscarPorCreditoExtra(creditoExtra.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado creditoDisponivelUtilizado in creditosUtilizados)
                    {
                        creditoExtra.CreditoDisponivel.ValorSaldo -= creditoExtra.ValorCreditoExtra;
                        creditoExtra.CreditoDisponivel.ValorCreditoExtra -= creditoExtra.ValorCreditoExtra;
                        creditoExtra.Ativo = false;
                        repCreditoDisponivel.Atualizar(creditoExtra.CreditoDisponivel);
                        repCreditoExtra.Atualizar(creditoExtra);
                    }
                    repCreditoExtra.Deletar(creditoExtra, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "O credito que você disponibilizou já foi utilizado por isso não é possível remove-lo, se necessário atualize o valor de credito reduzindo até " + (creditoExtra.CreditoDisponivel.ValorSaldo - creditoExtra.ValorCreditoExtra).ToString("n2") + " (valor já utilizado).");
                }
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
    }
}
