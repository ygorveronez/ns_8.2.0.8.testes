namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFilaCarregamento
    {
        Reversa = 1,
        Vazio = 2
    }

    public static class TipoFilaCarregamentoHelper
    {
        public static string ObterDescricao(this TipoFilaCarregamento tipoFilaCarregamento)
        {
            switch (tipoFilaCarregamento)
            {
                case TipoFilaCarregamento.Reversa: return "Reversa";
                case TipoFilaCarregamento.Vazio: return "Vazio";
                default: return string.Empty;
            }
        }
    }
}
