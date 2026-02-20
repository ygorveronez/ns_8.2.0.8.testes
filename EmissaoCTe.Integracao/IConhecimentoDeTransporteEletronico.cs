using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    [ServiceContract]
    public interface IConhecimentoDeTransporteEletronico
    {
        [OperationContract]
        Retorno<int> AlterarCTe(int codigoCTe, Dominio.ObjetosDeValor.CTe.CTe cte, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        Retorno<int> IntegrarCTePorTxt(int codigoEmpresaPai, string nomeArquivo, string arquivoTexto);

        [OperationContract]
        Retorno<int> CancelarCTePorTxt(int codigoEmpresaPai, string nomeArquivo, string arquivoTexto);

        [OperationContract]
        Retorno<string> EnviarXMLNFeParaIntegracao(System.IO.Stream arquivo);

        [OperationContract]
        Retorno<int> IntegrarCTePorXMLNFe(string token, int codigoEmpresa, string nomeArquivo);

        [OperationContract]
        Retorno<int> IntegrarCTe(Dominio.ObjetosDeValor.CTe.CTe cte, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        Retorno<int> IntegrarCTeAguardarConfirmacao(Dominio.ObjetosDeValor.CTe.CTe cte, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        Retorno<int> CancelarCTe(string cnpjEmpresaAdministradora, string chaveCTe, string justificativa, string token);

        [OperationContract]
        Retorno<int> InutilizarCTe(string cnpjEmpresaAdministradora, string cnpjEmpresaEmitente, int numeroCTe, int serieCTe, string justificativa, string token);

        [OperationContract]
        Retorno<int> EnviarXMLCTe(string xml, string cnpjEmpresaPai, string cnpjEmpresaEmitente, string token);

        [OperationContract]
        Retorno<int> EnviarXMLCTeDeEnvio(string xmlEnvio, string chCTe, string nProt, string cStat, string tpAmb, string cnpjEmpresaPai, string cnpjEmpresaEmitente, string tipoVeiculo, string token);

        [OperationContract]
        Retorno<int> SolicitarReimpressaoCTe(int codigoCTe, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        Retorno<int> ValidarCadastroVeiculo(string placa, string cnpjTransportador, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        Retorno<RetornoImposto> CalcularImpostos(Dominio.ObjetosDeValor.CalculoImposto.Dados dados, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        Retorno<int> IntegrarCTeCTeComplementar(string chaveCTeComplementado, decimal valorComplemento, string observacao, string cnpjEmpresaAdministradora, string token);

        [OperationContract]
        Retorno<int> ConfirmarEmissaoCTe(int codigoCTe, string cnpjEmpresaAdministradora, string token);
    }
}
