namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MercadoLivreHandlingUnitSituacao
    {
        Sucesso = 0,
        Falha = 1,
        AgConsulta = 2,
        AgConfirmacao = 3
    }

    public static class MercadoLivreHandlingUnitSituacaoHelper
    {
        public static string ObterDescricao(this MercadoLivreHandlingUnitSituacao situacao)
        {
            switch (situacao)
            {
                case MercadoLivreHandlingUnitSituacao.Sucesso: return "Sucesso";
                case MercadoLivreHandlingUnitSituacao.Falha: return "Falha";
                case MercadoLivreHandlingUnitSituacao.AgConsulta: return "Ag. Consulta";
                case MercadoLivreHandlingUnitSituacao.AgConfirmacao: return "Ag. Confirmação";
                default: return string.Empty;
            }
        }
    }
}
