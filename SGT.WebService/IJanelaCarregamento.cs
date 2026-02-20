using Dominio.ObjetosDeValor.WebService.Logistica.JanelaCarregamento;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IJanelaCarregamento" in both code and config file together.
    [ServiceContract]
    public interface IJanelaCarregamento
    {
        [OperationContract]
        Retorno<bool> InformarEtapaFluxoPatio(int? protocoloCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxo, string dataEtapa, decimal? peso, string numeroCarga, string codigoFilial, string dataLacre, string doca, string observacao);

        [OperationContract]
        Retorno<bool> RetornarEtapaFluxoPatio(int protocoloCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxo);

        [OperationContract]
        Retorno<bool> InformarEtapaFluxoPatioPorPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxo, string dataEtapa);

        [OperationContract]
        Retorno<bool> InformarEtapaFluxoPatioPorPlaca(Dominio.ObjetosDeValor.WebService.Carga.AvancoFluxoPatioPorPlaca corpo);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento> BuscarDisponibilidadeEntrega(Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.DisponibilidadeCarregamento disponibilidadeCarregamento);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ReservaCarregamento> AtualizarReserva(int protocoloReserva, Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.DisponibilidadeCarregamento disponibilidadeCarregamento);

        [OperationContract]
        Retorno<bool> ConfirmarReserva(int protocoloReserva);

        [OperationContract]
        Retorno<bool> CancelarReserva(int protocoloReserva);

        [OperationContract]
        Retorno<bool> ReceberPlaca(string placa);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarFluxoPatioPendenteIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.GestaoPatio.FluxoPatio> BuscarFluxoPatioPorProtocolo(int protocoloCarga);

        [OperationContract]
        Retorno<bool> ConfirmarFluxoPatioPendenteIntegracao(int protocoloCarga);

        [OperationContract]
        Retorno<bool> ControlarLiberacaoTransportadores(ControleLiberacaoTransportadores controleLiberacaoTransportadores);

        [OperationContract]
        Retorno<bool> RejeitarCarga(int protocoloIntegracaoCarga, string motivoRejeicaoCarga, string cnpjTransportador);

        [OperationContract]
        Retorno<bool> AceitarCarga(int protocoloIntegracaoCarga, string cnpjTransportador);
    }
}
