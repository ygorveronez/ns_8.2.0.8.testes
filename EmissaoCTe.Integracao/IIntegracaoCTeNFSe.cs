using System.Collections.Generic;
using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    [ServiceContract]
    public interface IIntegracaoCTeNFSe
    {
        [OperationContract]
        RetornoDocumento<int> IntegrarDocumento(Dominio.ObjetosDeValor.CTe.CTeNFSe documento, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        RetornoDocumento<int> IntegrarDocumentoAguardarConfirmacao(Dominio.ObjetosDeValor.CTe.CTeNFSe documento, string cnpjEmpresaAdministradora, string token);        

        [OperationContract]
        Retorno<RetornoCTeNFSe> BuscarPorCodigoDocumento(int codigo, string documento, string token, string codificarUTF8);

        [OperationContract]
        Retorno<RetornoCTeNFSe> BuscarPorChaveCTe(string chaveCTe, string token, string codificarUTF8);

        [OperationContract]
        Retorno<int> IntegrarNFSeProcessada(Dominio.ObjetosDeValor.NFSe.NFSeProcessada nfseProcessada, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        RetornoDocumento<int> ReenviarDocumento(int codigo, string documento, string token);

        [OperationContract]
        RetornoDocumento<int> ConfirmarEmissaoDocumento(int codigo, string documento, string token);

        [OperationContract]
        Retorno<List<RetornoProtocoloPorNFe>> ConsultarProtocoloPorNFe(string chaveNFe, string token);

        [OperationContract]
        Retorno<int> InformarTerminoEnvioDocumentos(Dominio.ObjetosDeValor.IntegrarCarga integrarCarga, string token);

        [OperationContract]
        Retorno<int> SolicitarCancelamentoDocumentos(Dominio.ObjetosDeValor.IntegrarCarga integrarCarga, string justificativa, string token);

        [OperationContract]
        Retorno<int> SolicitarQuitacaoCIOTCarga(Dominio.ObjetosDeValor.IntegrarCarga integrarCarga, string token);

        [OperationContract]
        Retorno<int> CancelarCTe(string cnpjEmpresaAdministradora, int codigo, string justificativa, string token);
    }
}
