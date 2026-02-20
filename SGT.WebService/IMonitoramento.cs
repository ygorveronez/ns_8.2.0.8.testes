using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMonitoramento" in both code and config file together.
    [ServiceContract]
    public interface IMonitoramento
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga>> BuscarCargas(string dataInicial, int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> SolicitarReenvioEventosCarga(string numeroCarga);

        //[OperationContract]
        //Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.Carga>> BuscarHistoricoPosicoesPorCarga(string numeroCarga);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Monitoramento.HistoricoPosicoesCargaMonitoramento>> BuscarHistoricoPosicoesPorCarga(string numeroCarga);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.VeiculoDedicado> ConsultarVeiculoDedicado(string transportador, string placa);

    }
}
