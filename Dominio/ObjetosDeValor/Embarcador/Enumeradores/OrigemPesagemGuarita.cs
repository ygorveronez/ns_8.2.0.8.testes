namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemPesagemGuarita
    {
        Manual = 1,
        Integracao = 2
    }

    public static class OrigemPesagemGuaritaHelper
    {
        public static string ObterDescricao(this OrigemPesagemGuarita origemPesagemGuarita)
        {
            switch (origemPesagemGuarita)
            {
                case OrigemPesagemGuarita.Manual: return "Manual";
                case OrigemPesagemGuarita.Integracao: return "Integração";
                default: return string.Empty;
            }
        }
    }
}

