using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    [ServiceContract]
    public interface IIntegracaoNFSe
    {
        [OperationContract]
        Retorno<RetornoNFSe> BuscarPorCodigoNFSe(int codigoNFSe, Dominio.Enumeradores.TipoIntegracaoNFSe tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token);
    }
}

