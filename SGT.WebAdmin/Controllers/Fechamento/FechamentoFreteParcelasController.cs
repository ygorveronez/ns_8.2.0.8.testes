using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fechamento
{
    [CustomAuthorize("Fechamento/FechamentoFrete")]
    public class FechamentoFreteParcelasController : BaseController
    {
		#region Construtores

		public FechamentoFreteParcelasController(Conexao conexao) : base(conexao) { }

		#endregion


        public async Task<IActionResult> PesquisaParcelaFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFechamentoFrete = 0;
                int.TryParse(Request.Params("Codigo"), out codigoFechamentoFrete);

                Repositorio.Embarcador.Fechamento.FechamentoFreteParcela repFechamentoFreteParcela = new Repositorio.Embarcador.Fechamento.FechamentoFreteParcela(unitOfWork);
                Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete = repFechamentoFrete.BuscarPorCodigo(codigoFechamentoFrete);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoFechamentoFrete", false);
                grid.AdicionarCabecalho("Parcela", "Parcela", 8, Models.Grid.Align.center, true);


                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Desconto", "Desconto", 10, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 20, Models.Grid.Align.center, false);


                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Parcela")
                    propOrdenar = "Sequencia";

                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela> listaFechamentoFreteParcela = repFechamentoFreteParcela.BuscarPorFechamento(codigoFechamentoFrete, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFechamentoFreteParcela.ContarBuscarPorFechamento(codigoFechamentoFrete));
                var dynRetorno = (from obj in listaFechamentoFreteParcela
                                  select new
                                  {
                                      obj.Codigo,
                                      CodigoFechamentoFrete = obj.Fechamento.Codigo,
                                      Parcela = obj.Sequencia.ToString("n0"),
                                      Valor = obj.Valor.ToString("n2"),
                                      Desconto = obj.Desconto.ToString("n2"),
                                      DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                      obj.DescricaoSituacao
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as parcelas do Fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarParcelas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                //Repositorio.Embarcador.Fechamento.FechamentoFreteParcela repFechamentoFreteParcela = new Repositorio.Embarcador.Fechamento.FechamentoFreteParcela(unitOfWork);

                //int codigoFechamentoFrete, quantidadeParcelas, intervaloDeDias = 0;
                //int.TryParse(Request.Params("Codigo"), out codigoFechamentoFrete);
                //int.TryParse(Request.Params("QuantidadeParcelas"), out quantidadeParcelas);
                //int.TryParse(Request.Params("IntervaloDeDias"), out intervaloDeDias);

                //string observacaoFechamentoFrete = Request.Params("ObservacaoFechamentoFrete");
                //bool imprimirObservacaoFechamentoFrete = false;
                //bool.TryParse(Request.Params("ImprimirObservacaoFechamentoFrete"), out imprimirObservacaoFechamentoFrete);

                ////Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                ////int banco = 0;
                ////int.TryParse(Request.Params("Banco"), out banco);

                ////string agencia = Request.Params("Agencia");
                ////string digito = Request.Params("Digito");
                ////string numeroConta = Request.Params("NumeroConta");

                ////Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta;
                ////Enum.TryParse(Request.Params("TipoConta"), out tipoConta);

                //DateTime dataPrimeiroVencimento = Request.GetDateTimeParam("DataPrimeiroVencimento");

                //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento tipoArredondamento;
                //Enum.TryParse(Request.Params("TipoArredondamento"), out tipoArredondamento);

                //Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete = repFechamentoFrete.BuscarPorCodigo(codigoFechamentoFrete, true);

                //if (fechamentoFrete.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete.Fechado || fechamentoFrete.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete.Cancelado)
                //    return new JsonpResult(false, true, "A situação atual da fechamentoFrete não permite a geração das parcelas.");

                //if (dataPrimeiroVencimento == DateTime.MinValue)
                //    return new JsonpResult(false, true, "É preciso informar a data de vencimento.");

                //List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela> listaParcelas = repFechamentoFreteParcela.BuscarPorFechamento(codigoFechamentoFrete);

                //unitOfWork.Start();

                ////fechamentoFrete.TipoArredondamentoParcelas = tipoArredondamento;
                ////fechamentoFrete.ObservacaoFechamentoFrete = observacaoFechamentoFrete;
                ////fechamentoFrete.ImprimeObservacaoFechamentoFrete = imprimirObservacaoFechamentoFrete;

                ////if (banco > 0)
                ////    fechamentoFrete.Banco = repBanco.BuscarPorCodigo(banco);
                ////else
                ////    fechamentoFrete.Banco = null;

                ////fechamentoFrete.Agencia = agencia;
                ////fechamentoFrete.DigitoAgencia = digito;
                ////fechamentoFrete.NumeroConta = numeroConta;
                ////fechamentoFrete.TipoContaBanco = tipoConta;

                ////repFechamentoFrete.Atualizar(fechamentoFrete, Auditado);

                //for (int i = 0; i < listaParcelas.Count; i++)
                //    repFechamentoFreteParcela.Deletar(listaParcelas[i], Auditado);

                //decimal valorTotal = Math.Round(fechamentoFrete.ValorPagar, 2);

                //if (fechamentoFrete.Contrato.FranquiaContratoMensal > fechamentoFrete.ValorPagar)
                //    valorTotal = Math.Round(fechamentoFrete.Contrato.FranquiaContratoMensal, 2);

                //valorTotal = Math.Round(valorTotal + fechamentoFrete.TotalAcrescimos - fechamentoFrete.TotalDescontos, 2);

                //decimal valorParcela = Math.Round(valorTotal / quantidadeParcelas, 2);
                //decimal valorDiferenca = valorTotal - Math.Round(valorParcela * quantidadeParcelas, 2);
                //DateTime dataUltimaParcela = dataPrimeiroVencimento;

                //for (int i = 0; i < quantidadeParcelas; i++)
                //{
                //    Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela parcela = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela();
                //    parcela.DataEmissao = fechamentoFrete.DataFechamento;

                //    if (i == 0)
                //        parcela.DataVencimento = dataPrimeiroVencimento;
                //    else
                //        parcela.DataVencimento = dataUltimaParcela.AddDays(intervaloDeDias);

                //    dataUltimaParcela = parcela.DataVencimento;
                //    parcela.Desconto = 0;
                //    parcela.Fechamento = fechamentoFrete;
                //    parcela.Sequencia = i + 1;
                //    parcela.SituacaoParcela = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto;

                //    if (i == 0 && tipoArredondamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Primeira)
                //        parcela.Valor = valorParcela + valorDiferenca;
                //    else if ((i + 1) == quantidadeParcelas && tipoArredondamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento.Ultima)
                //        parcela.Valor = valorParcela + valorDiferenca;
                //    else
                //        parcela.Valor = valorParcela;

                //    repFechamentoFreteParcela.Inserir(parcela, Auditado);
                //}

                //repFechamentoFrete.Atualizar(fechamentoFrete);

                //Servicos.Auditoria.Auditoria.Auditar(Auditado, fechamentoFrete, null, "Gerou " + quantidadeParcelas.ToString() + " parcelas para a fechamentoFrete.", unitOfWork);

                //unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha gerar as parcelas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
