using Dominio.Enumeradores;
using System;

namespace Servicos.Embarcador.Frete.ContratoFreteCliente
{
    public class ContratoFreteCliente
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public ContratoFreteCliente(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores       

        #region Métodos Públicos

        public void ConsultarSaldoEConsome(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool ignoraAlcada = false)
        {
            new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork).Confirmar(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMensagemAlerta.CargaSemSaldoContratoFreteCliente);
            new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork).Confirmar(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMensagemAlerta.ContratoFreteClienteFechado);
            bool fechado = false;

            if (!UtilizaContratoFreteCliente(carga, ignoraAlcada: ignoraAlcada))
                return;

            if (!MovimentaSaldo(carga, Dominio.ObjetosDeValor.Enumerador.TipoMovimentoContrato.Debito, ref fechado))
            {
                Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);

                if (!fechado && !servicoMensagemAlerta.IsMensagemSemConfirmacao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMensagemAlerta.CargaSemSaldoContratoFreteCliente))
                {
                    servicoMensagemAlerta.Adicionar(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMensagemAlerta.CargaSemSaldoContratoFreteCliente, true, "Necessário ajustar o frete manualmente.");
                    carga.SaldoInsuficienteContratoFreteCliente = true;
                }

                if (fechado && !servicoMensagemAlerta.IsMensagemSemConfirmacao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMensagemAlerta.ContratoFreteClienteFechado))
                    servicoMensagemAlerta.Adicionar(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMensagemAlerta.ContratoFreteClienteFechado, true, "Contrato de Frete do Cliente está fechado.");

                carga.TabelaFrete = null;
                carga.NaoAvancarAutomaticamenteEtapaFretePorPendencia = true;
                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;
            }
            else
                carga.SaldoInsuficienteContratoFreteCliente = false;
        }

        public bool ValidaNovoFreteComSaldo(Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal freteNovo, out string erro)
        {
            erro = string.Empty;

            if (!UtilizaContratoFreteCliente(carga))
                return true;

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.MovimentoContratoFreteCliente repositorioMovimentoContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.MovimentoContratoFreteCliente(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPrimeiroPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contrato = pedido.ContratoFreteCliente;


            if (contrato == null)
            {
                erro = "Contrato não encontrado";
                return false;
            }

            if (contrato.Fechado)
            {
                erro = "Este Contrato de Frete Cliente já está fechado.";
                return false;
            }

            decimal saldoUtilizadoContrato = repositorioMovimentoContratoFreteCliente.ConsultarSaldoUtilizadoPorCodigoContrato(contrato.Codigo);
            decimal saldoAtual = contrato.ValorContrato - saldoUtilizadoContrato;

            if ((saldoAtual - freteNovo) < 0)
            {
                erro = "O valor do frete deve ser inferior ao saldo pelo contrato frete cliente (" + contrato.Descricao + ").";
                return false;
            }

            return true;
        }

        public void ExtornaSaldo(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!UtilizaContratoFreteCliente(carga, false, true))
                return;

            bool fechado = false;

            MovimentaSaldo(carga, Dominio.ObjetosDeValor.Enumerador.TipoMovimentoContrato.Credito, ref fechado);
        }

        public void InverteTomador(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!UtilizaContratoFreteCliente(carga))
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            System.Collections.Generic.List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            string tipoTomadorOriginal = string.Empty;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
            {
                tipoTomadorOriginal = cargaPedido.TipoTomador.ObterDescricao();

                cargaPedido.TipoTomador = TipoTomador.Outros;
                cargaPedido.Tomador = cargaPedido.Pedido.Remetente;

                repositorioCargaPedido.Atualizar(cargaPedido);
            }

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();

            auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;

            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Trocou tipo de tomador de {tipoTomadorOriginal} para outros - Contrato de frete cliente ", _unitOfWork);
        }

        #endregion

        #region Métodos Privados

        public bool UtilizaContratoFreteCliente(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool consumindo = true, bool ignoraAlcada = false)
        {

            Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorioAprovacaoAlcadaCarga = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.MovimentoContratoFreteCliente repositorioMovimentoContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.MovimentoContratoFreteCliente(_unitOfWork);

            int contagemAprovacoes = repositorioAprovacaoAlcadaCarga.ContarAprovacoes(carga.Codigo);
            bool existePorCarga = repositorioMovimentoContratoFreteCliente.ExistePorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = carga.TipoOperacao;

            if (contagemAprovacoes > 0 && !ignoraAlcada)
                return false;

            if (tipoOperacao?.ConfiguracaoCalculoFrete == null)
                return false;

            if (existePorCarga)
            {
                return !consumindo;
            }
            else if (!consumindo)
            {
                return false;
            }

            return tipoOperacao.ConfiguracaoCalculoFrete.UtilizarContratoFreteCliente ?? false;
        }

        private bool MovimentaSaldo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Enumerador.TipoMovimentoContrato movimentoContrato, ref bool fechado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.MovimentoContratoFreteCliente repositorioMovimentoContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.MovimentoContratoFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repositorioContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPrimeiroPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contrato = pedido.ContratoFreteCliente;

            if (contrato == null)
                return false;

            if (movimentoContrato == Dominio.ObjetosDeValor.Enumerador.TipoMovimentoContrato.Credito && repositorioMovimentoContratoFreteCliente.ExistePorCarga(carga.Codigo, true))
                return false;

            if (contrato.Fechado)
            {
                fechado = true;
                return false;
            }

            decimal saldoUtilizadoContrato = repositorioMovimentoContratoFreteCliente.ConsultarSaldoUtilizadoPorCodigoContrato(contrato.Codigo);
            decimal saldoAtual = contrato.ValorContrato - saldoUtilizadoContrato;
            decimal saldoNovo;

            if (movimentoContrato == Dominio.ObjetosDeValor.Enumerador.TipoMovimentoContrato.Debito)
                saldoNovo = saldoAtual - carga.ValorFreteAPagar;
            else
                saldoNovo = saldoAtual + carga.ValorFreteAPagar;

            if (saldoNovo >= 0)
            {
                Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente movimentoContratoFreteCliente = new Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente()
                {
                    Carga = carga,
                    ContratoFreteCliente = contrato,
                    Data = DateTime.Now,
                    Valor = carga.ValorFreteAPagar,
                    TipoMovimentoContrato = movimentoContrato
                };

                repositorioMovimentoContratoFreteCliente.Inserir(movimentoContratoFreteCliente);
                contrato.SaldoAtual = saldoNovo;
                repositorioContratoFreteCliente.Atualizar(contrato);

                return true;
            }

            return false;
        }

        #endregion
    }
}
