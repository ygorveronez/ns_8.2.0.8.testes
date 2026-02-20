namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoIntegracaoPagamentoMotorista
    {
        SemIntegracao = 1,
        PamCard = 2,
        PagBem = 3,
        PamCardCorporativo = 4,
        Email = 5,
        Target = 6,
        Extratta = 7,
        RepomFrete = 8,
        KMM = 9
    }

    public static class TipoIntegracaoPagamentoMotoristaHelper
    {
        public static string ObterDescricao(this TipoIntegracaoPagamentoMotorista tipoIntegracao)
        {
            switch (tipoIntegracao)
            {
                case TipoIntegracaoPagamentoMotorista.SemIntegracao: return "Sem Integração";
                case TipoIntegracaoPagamentoMotorista.PamCard: return "Pamcard";
                case TipoIntegracaoPagamentoMotorista.PagBem: return "Pagbem";
                case TipoIntegracaoPagamentoMotorista.PamCardCorporativo: return "Pamcard Corporativo";
                case TipoIntegracaoPagamentoMotorista.Email: return "E-mail";
                case TipoIntegracaoPagamentoMotorista.Target: return "Target";
                case TipoIntegracaoPagamentoMotorista.Extratta: return "Extratta";
                case TipoIntegracaoPagamentoMotorista.RepomFrete: return "Repom Frete";
                case TipoIntegracaoPagamentoMotorista.KMM: return "KMM";
                default: return string.Empty;
            }
        }

        public static bool PossuiIntegracao(this TipoIntegracaoPagamentoMotorista tipoIntegracao)
        {
            switch (tipoIntegracao)
            {
                case TipoIntegracaoPagamentoMotorista.PamCard:
                case TipoIntegracaoPagamentoMotorista.PagBem:
                case TipoIntegracaoPagamentoMotorista.PamCardCorporativo:
                case TipoIntegracaoPagamentoMotorista.Email:
                case TipoIntegracaoPagamentoMotorista.Target:
                case TipoIntegracaoPagamentoMotorista.Extratta:
                case TipoIntegracaoPagamentoMotorista.RepomFrete:
                    return true;
                default: return false;
            }
        }
    }
}
