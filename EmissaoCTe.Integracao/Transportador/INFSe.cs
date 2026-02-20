using System.Collections.Generic;
using System.ServiceModel;

namespace EmissaoCTe.Integracao.Transportador
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "INFSe" in both code and config file together.
    [ServiceContract]
    public interface INFSe
    {
        [OperationContract]
        Retorno<List<int>> ObterProtocolos(string cnpj, string token, string dataInicial, string dataFinal);

        [OperationContract]
        Retorno<List<RetornoNFSe>> ObterXML(string cnpj, string token, List<int> protocolos);
    }
}
