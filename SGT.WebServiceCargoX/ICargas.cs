using System.ServiceModel;

namespace SGT.WebServiceCargoX
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ICargas
    {
        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarCarga(Dominio.ObjetosDeValor.WebService.CargoX.CargaIntegracao cargaIntegracao);

        [OperationContract]
        Retorno<bool> EncerrarCarga(int protocoloIntegracaoCarga, string ObservacaoEncerramento);

        [OperationContract]
        Retorno<int> CadastrarApoliceSeguro(Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro apoliceSeguro);

        [OperationContract]
        Retorno<int> AtualizarApoliceSeguro(Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro apoliceSeguro, int codigo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro>> BuscarApolicesSeguro(int inicio, int limite);

    }



}
