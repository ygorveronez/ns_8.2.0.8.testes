using AdminMultisoftware.Dominio.Enumeradores;
using Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Pacote
{
    public class Pacote : ServicoBase
    {

        public Pacote(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancellationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancellationToken)
        {
        }

        public async Task<string> VincularCTeCargaPedidoPacoteAsync(Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote, Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubContratacao = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = null, bool executarValidacaoCTe = false, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null, CancellationToken cancellationToken = default)
        {
            Servicos.Embarcador.Carga.CTeSubContratacao servicoCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(_unitOfWork);

            Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidoPacote.CargaPedido;

            if (cargaPedido == null)
                return string.Empty;

            if (executarValidacaoCTe)
            {
                string retorno = servicoCTeSubContratacao.ValidarRegrasCTeParaSubContratacao(cteTerceiro, cargaPedido, _unitOfWork, _tipoServicoMultisoftware);

                if (!string.IsNullOrEmpty(retorno))
                    return retorno;
            }

            if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
            {
                if (cargaPedido.Expedidor != null && cargaPedido.Recebedor != null)
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
                else if (cargaPedido.Expedidor != null || cargaPedido.Recebedor != null)
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;

                if (objetoValorPersistente != null)
                    objetoValorPersistente.Atualizar(cargaPedido);

            }

            cteTerceiro.Ativo = true;
            servicoCTeSubContratacao.InserirCTeSubContratacaoCargaPedido(cteTerceiro, cargaPedido, _tipoServicoMultisoftware, _unitOfWork, pedidoCTesParaSubContratacao, configuracao, cacheObjetoValorCTe, objetoValorPersistente);
                      
            return string.Empty;
        }

        public async Task VerificarQuantidadePacotesCtesAvancaAutomaticoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, CancellationToken cancellationToken = default)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repositorioPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, cancellationToken);

            if ((carga.TipoOperacao?.ConfiguracaoCarga?.PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes ?? 0) > 0 && carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && !carga.ProcessandoDocumentosFiscais)
            {
                bool podeAvancarAutomaticamente = true;

                List<Dominio.ObjetosDeValor.Embarcador.Carga.QuantidadePacotesCargaPorCargaPedido> quantidadePacotesCargaPorCargaPedidos = await repositorioCargaPedidoPacote.BuscarQuantidadePacoteCargaPorCargaPedidoAsync(carga.Codigo, cancellationToken);

                foreach (var item in quantidadePacotesCargaPorCargaPedidos)
                    if (item.PERCENTUAL <= carga.TipoOperacao?.ConfiguracaoCarga?.PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes)
                        podeAvancarAutomaticamente = false;

                if (podeAvancarAutomaticamente)
                {
                    carga.CalcularFreteLote = quantidadePacotesCargaPorCargaPedidos.Sum(x => x.QTD_CTES_ANTERIORES) > 6000 ? Dominio.Enumeradores.LoteCalculoFrete.Integracao : Dominio.Enumeradores.LoteCalculoFrete.Padrao;
                    carga.ProcessandoDocumentosFiscais = true;
                    await repositorioCarga.AtualizarAsync(carga);

                    await Servicos.Auditoria.Auditoria.AuditarAsync(auditado, carga, null, "Avançado automaticamente pois a tolerância cadastrada foi superada.", _unitOfWork, cancellationToken: cancellationToken);
                }
            }
        }
    }
}
