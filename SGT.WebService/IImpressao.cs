using System.IO;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IImpressao" in both code and config file together.
    [ServiceContract]
    public interface IImpressao
    {
        [OperationContract]
        Retorno<string> EnviarArquivoXMLNFe(Stream arquivo);

        [OperationContract]
        Retorno<bool> SolicitarImpressaoNFe(string tokenXML, int? protocoloCarga, int? protocoloPedido);

        [OperationContract]
        Retorno<bool> SolicitarImpressaoBoleto(Dominio.ObjetosDeValor.WebService.Impressao.Boleto boleto, int? protocoloCarga, int? protocoloPedido);
    }
}
