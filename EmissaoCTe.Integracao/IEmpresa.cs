using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    [ServiceContract]
    public interface IEmpresa
    {
        [OperationContract]
        Retorno<string> SalvarEmpresaEmissora(Dominio.ObjetosDeValor.Empresa empresa, string cnpjEmpresaPai, string token);

        [OperationContract]
        Retorno<RetornoValidarEmpresa> ValidarEmpresaEmissoraParaEmissao(string cnpjEmpresaEmissora, string cnpjEmpresaPai);

        [OperationContract]
        Retorno<object> SalvarEmpresaEmissoraIntegracao(Dominio.ObjetosDeValor.Empresa empresa, string cnpjEmpresaPai, string chaveAcesso);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Empresa> ObterDetalhesEmpresa(string cnpjEmpresaEmissora, string cnpjEmpresaPai, string token);

        [OperationContract]
        Retorno<string> EnviarVencimentoCertificado(string cnpj, string nome, string dataVencimento, string ambiente, string homologacao, string email, string telefone, Dominio.ObjetosDeValor.Empresa empresa);

        [OperationContract]
        Retorno<RetornoValidarEmpresa> BuscarSeguroTransportador(string cnpjTransportador, string cnpjEmpresaAdministradora, string token);
    }
}
