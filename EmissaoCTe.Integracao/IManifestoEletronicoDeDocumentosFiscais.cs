using System.Collections.Generic;
using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    [ServiceContract]
    public interface IManifestoEletronicoDeDocumentosFiscais
    {
        [OperationContract]
        Retorno<int> IntegrarMDFe(int codigoEmpresaPai, int numeroCarga, int numeroUnidade, string dataHoraEncerramento, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> ListaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento);

        [OperationContract]
        Retorno<int> IntegrarMDFePorTxt(int codigoEmpresaPai, string nomeArquivo, string arquivoTexto);

        [OperationContract]
        Retorno<int> IntegrarMDFePorObjeto(Dominio.ObjetosDeValor.MDFe.MDFe mdfe, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        Retorno<int> IntegrarMDFePorChaveCTe(Dominio.ObjetosDeValor.MDFe.MDFe mdfe, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        Retorno<int> IntegrarMDFePorNFe(int codigoEmpresaPai, int numeroCarga, int numeroUnidade, string dataHoraEncerramento, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> ListaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento);

        [OperationContract]
        Retorno<int> IntegrarMDFePorCTes(int[] codigosCTes, string observacaoMDFe, string cnpjEmpresaEmitente, string cnpjEmpresaPai, int numeroUnidade, string token, Dominio.ObjetosDeValor.ValePedagioCompra ValePedagioCompra, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> ListaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento);

        [OperationContract]
        Retorno<int> IntegrarMDFePorCodigoCTes(int[] codigosCTes, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> Veiculos, List<Dominio.ObjetosDeValor.MotoristaMDFeIntegracao> Motoristas, List<Dominio.ObjetosDeValor.VeiculoMDFeIntegracao> Reboques, string observacaoFisco, string observacaoContribuinte, string token, int numeroUnidade, string numeroUnidadeString, List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> seguros, List<Dominio.ObjetosDeValor.ValePedagioIntegracao> valesPedagio, Dominio.ObjetosDeValor.MDFe.DadosMDFe dadosMDFe, List<Dominio.ObjetosDeValor.MDFe.NFeGlobalizada> nfesGlobalizadas, Dominio.ObjetosDeValor.MDFe.ProdutoPredominante produtoPredominante, List<Dominio.ObjetosDeValor.MDFe.CIOT> ListaCIOT, Dominio.ObjetosDeValor.MDFe.InformacoesPagamento informacoesPagamento);

        [OperationContract]
        Retorno<int> EncerrarMDFe(string cnpjEmpresaAdministradora, int codigoMDFe, string dataHoraEncerramento, int codigoIBGEMunicipioEncerramento, string token);

        [OperationContract]
        Retorno<bool> EncerrarMDFeExterno(Dominio.ObjetosDeValor.MDFe.EncerramentoMDFeExterno encerramentoMDFeExterno);

        [OperationContract]
        Retorno<int> CancelarMDFe(string cnpjEmpresaAdministradora, int codigoMDFe, string justificativa, string token);

        [OperationContract]
        Retorno<int> ReemitirMDFe(int codigoMDFe, string token);

        [OperationContract]
        Retorno<int> IntegrarMDFePorCTesEPlaca(int[] codigosCTes, string cnpjEmpresaEmitente, string cnpjEmpresaPai, int numeroUnidade, string placaTracao, string placaReboque, string token);

        [OperationContract]
        Retorno<List<int>> IntegrarMDFePorCTesDestinosDiferentes(int[] codigosCTes, string cnpjEmpresaEmitente, string cnpjEmpresaPai, int numeroUnidade, int numeroCarga, string observacaoMDFe, string token);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT> IntegrarCIOTPorCTes(Dominio.ObjetosDeValor.CIOT.CIOT ciot, string token);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.CIOT.RetornoCIOT> ConsultarCIOTPorProtocolo(int protocolo, string token);

        [OperationContract]
        Retorno<int> EnviarXMLMDFe(string xml, string cnpjEmpresaPai, string cnpjEmpresaEmitente, string token);
    }
}
