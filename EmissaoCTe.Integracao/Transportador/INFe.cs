using System.IO;
using System.ServiceModel;

namespace EmissaoCTe.Integracao.Transportador
{
    [ServiceContract]
    public interface INFe
    {
        [OperationContract]
        Retorno<string> EnviarXMLNFeParaIntegracao(Stream arquivo);

        [OperationContract]
        Retorno<bool> DisponibilizarXMLParaEmissaoCTe(string token, string cnpjTransportador, string identificadorXML);
    }
}
