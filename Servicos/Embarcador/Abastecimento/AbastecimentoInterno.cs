using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Servicos.Embarcador.Abastecimento
{
    public class AbastecimentoInterno : ServicoBase
    {
        public AbastecimentoInterno(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos
        public static void ProcessarTransferenciaTanque(Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia localArmazenamentoProdutoTransferencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.LocalArmazenamentoHistorico repLocalArmazenamentoHistorico = new Repositorio.Embarcador.Produtos.LocalArmazenamentoHistorico(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia repLocalArmazenamentoProdutoTransferencia = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

            try
            {

                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoOrigem = repLocalArmazenamentoProduto.BuscarPorCodigo(localArmazenamentoProdutoTransferencia.LocalArmazenamentoProduto.Codigo);
                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamentoDestino = repLocalArmazenamentoProduto.BuscarPorCodigo(localArmazenamentoProdutoTransferencia.LocalArmazenamentoProdutoDestino.Codigo);

                if (localArmazenamentoDestino == null)
                    throw new ServicoException($"Tanque destino não existe");

                if (localArmazenamentoOrigem == null)
                    throw new ServicoException($"Tanque origem não existe");

                if(!(localArmazenamentoDestino.ControleAbastecimentoDisponivel ?? false))
                    throw new ServicoException($"Tanque destino não controle abastecimento disponível");

                if (!(localArmazenamentoOrigem.ControleAbastecimentoDisponivel ?? false))
                    throw new ServicoException($"Tanque origem não controle abastecimento disponível");
            

                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico localArmazenamentoHistoricoOrigem = new Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico();
                decimal saldoDoTanqueOrigem = localArmazenamentoOrigem?.SaldoDoTanque ?? 0;
                decimal saldoDoTanqueDestino = localArmazenamentoDestino?.SaldoDoTanque ?? 0;

                localArmazenamentoHistoricoOrigem.LocalArmazenamentoProduto = localArmazenamentoOrigem;
                localArmazenamentoHistoricoOrigem.TipoMovimentacao = TipoMovimentacaoAbastecimento.Saida;
                localArmazenamentoHistoricoOrigem.SaldoAnterior = localArmazenamentoOrigem.SaldoDoTanque;
                localArmazenamentoHistoricoOrigem.SaldoAtual = saldoDoTanqueOrigem - localArmazenamentoProdutoTransferencia.QuantidadeTransferida;
                localArmazenamentoHistoricoOrigem.Data = DateTime.Now;
                repLocalArmazenamentoHistorico.Inserir(localArmazenamentoHistoricoOrigem);

                localArmazenamentoOrigem.SaldoDoTanque = saldoDoTanqueOrigem - localArmazenamentoProdutoTransferencia.QuantidadeTransferida;
                repLocalArmazenamentoProduto.Atualizar(localArmazenamentoOrigem);


                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico localArmazenamentoHistoricoAnterior = repLocalArmazenamentoHistorico.BuscarUltimoHistoricoPorLocalArmazenamento(localArmazenamentoDestino.Codigo);
                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico localArmazenamentoHistoricoDestino = new Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico();
                localArmazenamentoHistoricoDestino.LocalArmazenamentoProduto = localArmazenamentoDestino;
                localArmazenamentoHistoricoDestino.TipoMovimentacao = TipoMovimentacaoAbastecimento.Entrada;
                localArmazenamentoHistoricoDestino.SaldoAnterior = localArmazenamentoHistoricoAnterior != null ? localArmazenamentoHistoricoAnterior.SaldoAtual : 0 ;
                localArmazenamentoHistoricoDestino.SaldoAtual = saldoDoTanqueDestino + localArmazenamentoProdutoTransferencia.QuantidadeTransferida;
                localArmazenamentoHistoricoDestino.Data = DateTime.Now;
                repLocalArmazenamentoHistorico.Inserir(localArmazenamentoHistoricoDestino);

                localArmazenamentoDestino.SaldoDoTanque = saldoDoTanqueDestino + localArmazenamentoProdutoTransferencia.QuantidadeTransferida;
                repLocalArmazenamentoProduto.Atualizar(localArmazenamentoDestino);

                localArmazenamentoProdutoTransferencia.Situacao = SituacaoLocalArmazanamentoProdutoTransferencia.Transferido;
                localArmazenamentoProdutoTransferencia.DescricaoTransferencia = "Transferido com sucesso";
                repLocalArmazenamentoProdutoTransferencia.Atualizar(localArmazenamentoProdutoTransferencia);
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex, "AbastecimentoInterno");

                localArmazenamentoProdutoTransferencia.Situacao = SituacaoLocalArmazanamentoProdutoTransferencia.ProblemaTransferencia;
                localArmazenamentoProdutoTransferencia.DescricaoTransferencia = ex.Message;
                repLocalArmazenamentoProdutoTransferencia.Atualizar(localArmazenamentoProdutoTransferencia);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "AbastecimentoInterno");
            
                localArmazenamentoProdutoTransferencia.Situacao = SituacaoLocalArmazanamentoProdutoTransferencia.ProblemaTransferencia;
                localArmazenamentoProdutoTransferencia.DescricaoTransferencia = ex.Message;
                repLocalArmazenamentoProdutoTransferencia.Atualizar(localArmazenamentoProdutoTransferencia);
            }
        }

        public static void GerarHistoricoMovimentoAbastecimento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoAbastecimento tipoMovimento, Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida movimentoSaida, Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque movimentoEntrada)
        {
            Repositorio.Embarcador.Produtos.LocalArmazenamentoHistorico repLocalArmazenamentoHistorico = new Repositorio.Embarcador.Produtos.LocalArmazenamentoHistorico(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

            if (localArmazenamento == null)
                return;

            if (!(localArmazenamento.ControleAbastecimentoDisponivel ?? false))
                return;

            if (movimentoEntrada == null && movimentoSaida == null)
                return;

            decimal saldoAnterior = repLocalArmazenamentoHistorico.BuscarUltimoHistoricoPorLocalArmazenamento(localArmazenamento.Codigo)?.SaldoAtual ?? 0;
            decimal saldoAtual = saldoAnterior;

            if (tipoMovimento == TipoMovimentacaoAbastecimento.AtualizaEntrada || tipoMovimento == TipoMovimentacaoAbastecimento.AtualizaSaida || tipoMovimento == TipoMovimentacaoAbastecimento.CancelaEntrada || tipoMovimento == TipoMovimentacaoAbastecimento.CancelaSaida)
            {
                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico localArmazenamentoAnterior = null;


                if ((tipoMovimento == TipoMovimentacaoAbastecimento.AtualizaEntrada || tipoMovimento == TipoMovimentacaoAbastecimento.CancelaEntrada) && movimentoEntrada != null)
                {
                    localArmazenamentoAnterior = repLocalArmazenamentoHistorico.BuscarPorMovimentoEntrada(movimentoEntrada.Codigo);

                    if (localArmazenamentoAnterior == null)
                        throw new ServicoException($"Não encontrado histórico de movimento de saldo");

                    saldoAnterior = localArmazenamentoAnterior.SaldoAtual;
                    saldoAtual = localArmazenamentoAnterior.SaldoAtual - localArmazenamentoAnterior.QuantidadeMovimento;
                }
                else if ((tipoMovimento == TipoMovimentacaoAbastecimento.AtualizaSaida || tipoMovimento == TipoMovimentacaoAbastecimento.CancelaSaida) && movimentoSaida != null)
                {
                    localArmazenamentoAnterior = repLocalArmazenamentoHistorico.BuscarPorMovimentoSaida(movimentoSaida.Codigo);

                    if (localArmazenamentoAnterior == null)
                        throw new ServicoException($"Não encontrado histórico de movimento de saldo");

                    saldoAnterior = localArmazenamentoAnterior.SaldoAtual;
                    saldoAtual = localArmazenamentoAnterior.SaldoAtual + localArmazenamentoAnterior.QuantidadeMovimento;
                }
               
                if(saldoAtual < 0)
                    throw new ServicoException($"Saldo do tanque não pode ser negativo");

                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico localArmazenamentoEstorno = new Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico();

                localArmazenamentoEstorno.MovimentacaoAbastecimentoSaida = movimentoSaida;
                localArmazenamentoEstorno.MovimentoEntradaTanque = movimentoEntrada;
                localArmazenamentoEstorno.LocalArmazenamentoProduto = localArmazenamento;
                localArmazenamentoEstorno.TipoMovimentacao = tipoMovimento;
                localArmazenamentoEstorno.SaldoAnterior = saldoAnterior;
                localArmazenamentoEstorno.SaldoAtual = saldoAtual;
                localArmazenamentoEstorno.Data = DateTime.Now;

                repLocalArmazenamentoHistorico.Inserir(localArmazenamentoEstorno);

                localArmazenamento.SaldoDoTanque = saldoAtual;
                repLocalArmazenamentoProduto.Atualizar(localArmazenamento);

                saldoAnterior = saldoAtual;
            }


            if (tipoMovimento == TipoMovimentacaoAbastecimento.AtualizaEntrada)
                tipoMovimento = TipoMovimentacaoAbastecimento.Entrada;
            else if (tipoMovimento == TipoMovimentacaoAbastecimento.AtualizaSaida)
                tipoMovimento = TipoMovimentacaoAbastecimento.Saida;
            else if (tipoMovimento == TipoMovimentacaoAbastecimento.CancelaSaida || tipoMovimento == TipoMovimentacaoAbastecimento.CancelaEntrada)
                return;

            if (tipoMovimento == TipoMovimentacaoAbastecimento.Entrada && movimentoEntrada != null)
            {
                saldoAtual += movimentoEntrada.QuantidadeLitros;
            }
            else if(tipoMovimento == TipoMovimentacaoAbastecimento.Saida && movimentoSaida != null)
            {
                saldoAtual -= movimentoSaida.QuantidadeLitros;
            }

            if (saldoAtual < 0)
                throw new ServicoException($"Saldo do tanque não pode ser negativo");

            Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico localArmazenamentoHistorico = new Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico();

            localArmazenamentoHistorico.MovimentacaoAbastecimentoSaida = movimentoSaida;
            localArmazenamentoHistorico.MovimentoEntradaTanque = movimentoEntrada;
            localArmazenamentoHistorico.LocalArmazenamentoProduto = localArmazenamento;
            localArmazenamentoHistorico.TipoMovimentacao = tipoMovimento;
            localArmazenamentoHistorico.SaldoAnterior = saldoAnterior;
            localArmazenamentoHistorico.SaldoAtual = saldoAtual;
            localArmazenamentoHistorico.Data = DateTime.Now;

            repLocalArmazenamentoHistorico.Inserir(localArmazenamentoHistorico);

            localArmazenamento.SaldoDoTanque = saldoAtual;
            repLocalArmazenamentoProduto.Atualizar(localArmazenamento);
        }

        #endregion

        public static bool ValidaEntidadeMovimentacaoAbastecimentoSaida(Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida movimentacaoSaida, out string erro)
        {
            erro = string.Empty;

            Dominio.Entidades.Veiculo veiculo = movimentacaoSaida.Veiculo;
            if (veiculo != null)
            {
                if (veiculo.Modelo != null && (veiculo.Modelo.PossuiArla32 == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao && movimentacaoSaida.QuantidadeArla32 > 0))
                {
                    erro = "O modelo do veículo selecionado não permite o lançamento de ARLA 32.";
                    return false;
                }

            }

            if (movimentacaoSaida.Data > DateTime.Now)
            {
                erro = "Não é possível lançar um abastecimento com data futura.";
                return false;
            }

            if (veiculo == null)
            {
                erro = "Favor informe um veículo ou um equipamento para o lançamento do abastecimento.";
                return false;
            }

            return string.IsNullOrEmpty(erro);
        }
    }
}
