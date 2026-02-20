namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoSugestaoProgramacaoCarga
    {
        Gerada = 1,
        Publicada = 2,
        Cancelada = 3
    }

    public static class SituacaoSugestaoProgramacaoCargahelper
    {
        public static string ObterCorFonte(this SituacaoSugestaoProgramacaoCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoSugestaoProgramacaoCarga.Cancelada: return "#ffffff";
                default: return "#212529";
            }
        }

        public static string ObterCorLinha(this SituacaoSugestaoProgramacaoCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoSugestaoProgramacaoCarga.Cancelada: return "#ff6666";
                case SituacaoSugestaoProgramacaoCarga.Gerada: return "#91a8ee";
                case SituacaoSugestaoProgramacaoCarga.Publicada: return "#80ff80";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoSugestaoProgramacaoCarga situacao)
        {
            switch (situacao)
            {
                case SituacaoSugestaoProgramacaoCarga.Cancelada: return "Cancelada";
                case SituacaoSugestaoProgramacaoCarga.Gerada: return "Gerada";
                case SituacaoSugestaoProgramacaoCarga.Publicada: return "Publicada";
                default: return string.Empty;
            }
        }
    }
}
