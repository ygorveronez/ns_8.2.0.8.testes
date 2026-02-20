using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IIntegracaoNFe" in both code and config file together.
    [ServiceContract]
    public interface IIntegracaoNFe
    {
        [OperationContract]
        Retorno<List<RetornoImpressora>> ConsultarImpressora(int numeroUnidade, string status, string nomeImpressora, string token);

        [OperationContract]
        Retorno<List<RetornoNFe>> BuscarNFeImpressao(int codigoEmpresaPai, string numeroCarga, int numeroUnidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao, string token);

        [OperationContract]
        Retorno<object> AlterarSituacao(int codigoEmpresaPai, int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao, string pdfNFe, string token);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Impressao.Boleto>> BuscarBoletoImpressao(int codigoEmpresaPai, string numeroCarga, int numeroUnidade, int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao, string token);

        [OperationContract]
        Retorno<object> AlterarSituacaoBoleto(int codigoEmpresaPai, int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao, string pdfBoleto, string token);

        [OperationContract]
        Retorno<string> EnviarArquivoXMLNFeParaAverbacao(Stream arquivo);

        [OperationContract]
        Retorno<string> EnviarStringXMLNFeParaAverbacao(string xmlNFe, string token);
    }
}
