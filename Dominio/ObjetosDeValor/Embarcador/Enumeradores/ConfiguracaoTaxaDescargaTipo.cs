namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ConfiguracaoTaxaDescargaTipo
    {
        AjudantesPorQuantidade = 1,
    }
    public static class ConfiguracaoTaxaDescargaTipoHelper
    {
        public static string ObterDescricao(this ConfiguracaoTaxaDescargaTipo tipo)
        {
            switch (tipo)
            {
                case ConfiguracaoTaxaDescargaTipo.AjudantesPorQuantidade: return "Ajudantes por quantidade";
                default: return string.Empty;
            }
        }
    }
}
