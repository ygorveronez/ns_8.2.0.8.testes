namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum UnidadeNegocioBoticario
    {
        GrupoBoticario = 1,
        QuemDisseBerenice = 2,
    }

    public static class UnidadeNegocioBoticarioHelper
    {
        public static string ObterDescricao(this UnidadeNegocioBoticario tipo)
        {
            switch (tipo)
            {
                case UnidadeNegocioBoticario.GrupoBoticario: return "Grupo Boticário";
                case UnidadeNegocioBoticario.QuemDisseBerenice: return "Quem Disse Berenice";

                default: return string.Empty;
            }
        }

        public static string ObterSigla(this UnidadeNegocioBoticario tipo)
        {
            switch (tipo)
            {
                case UnidadeNegocioBoticario.GrupoBoticario: return "GB";
                case UnidadeNegocioBoticario.QuemDisseBerenice: return "QDB";

                default: return string.Empty;
            }
        }
    }
}
