using System.Collections.Generic;
using System.ServiceModel;

namespace EmissaoCTe.Integracao.Transportador
{
    [ServiceContract]
    public interface IMDFe
    {
        [OperationContract]
        Retorno<List<int>> ObterProtocolos(string cnpj, string token, string dataInicial, string dataFinal, string serie);

        [OperationContract]
        Retorno<List<RetornoConsultaXML>> ObterXML(string cnpj, string token, List<int> protocolos);

        [OperationContract]
        Retorno<int> EnviarEventoMDFe(string chaveMDFe, string cnpj, string protocolo, Dominio.Enumeradores.TipoIntegracaoMDFe tipoEvento, string token);
    }
}
