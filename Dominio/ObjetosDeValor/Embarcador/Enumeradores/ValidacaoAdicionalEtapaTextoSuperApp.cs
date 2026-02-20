
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ValidacaoAdicionalEtapaTextoSuperApp
    {
        Cpf = 1,
        Cnpj = 2,
        CpfCnpj = 3,
        
    }

    public static class ValidacaoAdicionalEtapaTextoSuperAppHelper
    {
        public static string ObterDescricao(this ValidacaoAdicionalEtapaTextoSuperApp tipoValidacao)
        {
            switch (tipoValidacao)
            {
                case ValidacaoAdicionalEtapaTextoSuperApp.Cpf: return "CPF";
                case ValidacaoAdicionalEtapaTextoSuperApp.Cnpj: return "CNPJ";
                case ValidacaoAdicionalEtapaTextoSuperApp.CpfCnpj: return "CPF ou CNPJ";
                default: return string.Empty;
            }
        }
        public static string ObterSuperAppType(this ValidacaoAdicionalEtapaTextoSuperApp tipoValidacao)
        {
            switch (tipoValidacao)
            {
                case ValidacaoAdicionalEtapaTextoSuperApp.Cpf: return "CPF";
                case ValidacaoAdicionalEtapaTextoSuperApp.Cnpj: return "CNPJ";
                case ValidacaoAdicionalEtapaTextoSuperApp.CpfCnpj: return "CPF_CNPJ";
                default: return string.Empty;
            }
        }
    }
}