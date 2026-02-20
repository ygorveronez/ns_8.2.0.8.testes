using System.ServiceModel;

namespace EmissaoCTe.Integracao.Transportador
{
    [ServiceContract]
    public interface IOcorrenciaCTe
    {
        [OperationContract]
        Retorno<RetornoOcorrenciaCTe> ConsultarOcorrencia(string cnpjTransportador, string cnpjEmbarcador, string token, string chaveNFe, int numeroCTe);

        [OperationContract]
        Retorno<RetornoOcorrenciaCTe> ConsultarOcorrenciaChaveCTe(string chaveCTe, string cnpjTransportador, string token);


        [OperationContract]
        Retorno<int> IntegrarOcorrenciaCTe(string cnpjTransportador, string chaveCTe, string codigoOcorrencia, string dataOcorrencia, string observacao, string token);
    }
}
