using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IConsultaCNPJ" in both code and config file together.
    [ServiceContract]
    public interface IConsultaCNPJ
    {
        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica> SolicitarRequisicaoFazendaPessoaJuridica();

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica> ConsultarPessoaJuridicaFazenda(Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica requisicaoReceita, string CNPJ, string Captcha);

        [OperationContract]
        Retorno<string> ConsultarInscricaoSintegra(string CNPJ);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica> ConsultarCadastroCentralizado(string CNPJ);
    }
}
