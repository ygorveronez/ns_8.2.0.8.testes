using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IIntegracaoNFSe" in both code and config file together.
    [ServiceContract]
    public interface IIntegracaoNFSe
    {
        [OperationContract]
        Retorno<bool> IntegrarNFSeAutorizado(Dominio.ObjetosDeValor.WebService.NFSe.NFSeOracle nfseOracle);
    }
}
