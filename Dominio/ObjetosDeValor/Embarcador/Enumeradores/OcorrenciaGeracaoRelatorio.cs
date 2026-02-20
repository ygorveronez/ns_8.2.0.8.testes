namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OcorrenciaGeracaoRelatorio
    {
        Diario = 1,
        Semanal = 2,
        Mensal = 3
    }

    public static class OcorrenciaGeracaoRelatorioHelper
    {
        public static string ObterDescricao(this OcorrenciaGeracaoRelatorio ocorrenciaGeracaoRelatorio)
        {
            switch (ocorrenciaGeracaoRelatorio)
            {
                case OcorrenciaGeracaoRelatorio.Diario: return "Diário";
                case OcorrenciaGeracaoRelatorio.Semanal: return "Semanal";
                case OcorrenciaGeracaoRelatorio.Mensal: return "Mensal";
                default: return string.Empty;
            }
        }
    }
}
