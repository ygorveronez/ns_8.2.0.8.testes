namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEntidadeIntegracao
    {
        NaoInformado = 0,
        Carga = 1,
        CargaOcorrencia = 2,
        CTe = 3
    }

    public static class TipoEntidadeIntegracaoHelper
    {
        public static string ObterDescricao(this TipoEntidadeIntegracao tipo)
        {
            switch (tipo)
            {
                case TipoEntidadeIntegracao.Carga: return "Carga";
                case TipoEntidadeIntegracao.CargaOcorrencia: return "Ocorrência";
                case TipoEntidadeIntegracao.CTe: return "CTe";
                default: return string.Empty;
            }
        }
    }
}