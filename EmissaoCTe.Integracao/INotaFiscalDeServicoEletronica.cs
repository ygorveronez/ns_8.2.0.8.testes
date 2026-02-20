using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    [ServiceContract]
    public interface INotaFiscalDeServicoEletronica
    {
        [OperationContract]
        Retorno<int> IntegrarNFSe(Dominio.ObjetosDeValor.NFSe.NFSe nfse, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        Retorno<int> CancelarNFSe(string cnpjEmpresaAdministradora, int codigoNFSe, string justificativa, string token);

        [OperationContract]
        Retorno<int> ExcluirNFSe(string cnpjEmpresaAdministradora, int codigoNFSe, string token);
    }
}
