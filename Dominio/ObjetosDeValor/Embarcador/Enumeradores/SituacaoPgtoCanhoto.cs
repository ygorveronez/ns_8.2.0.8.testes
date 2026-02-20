namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPgtoCanhoto
    {
        Todas = 0,
        Pendente = 1,
        Liberado = 2,
        Rejeitado = 3
    }

    public static class SituacaoPgtoCanhotoHelper
    {
        public static string ObterDescricao(this SituacaoPgtoCanhoto situacaoPgtoCanhoto)
        {
            switch (situacaoPgtoCanhoto)
            {
                case SituacaoPgtoCanhoto.Pendente: return "Pendente";
                case SituacaoPgtoCanhoto.Liberado: return "Liberado";
                case SituacaoPgtoCanhoto.Rejeitado: return "Rejeitado";
                default: return string.Empty;
            }
        }
    }
}
