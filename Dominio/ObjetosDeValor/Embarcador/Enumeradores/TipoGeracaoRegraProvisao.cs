namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGeracaoRegraProvisao
    {
        Indefinida = 0,
        TermoQuitacao = 1,
        PrazoExcedido = 2,
    }

    public static class TipoGeracaoRegraProvisaoHelper
    {
        public static string ObterDescricao(this TipoGeracaoRegraProvisao TipoGeracaoRegraProvisao)
        {
            switch (TipoGeracaoRegraProvisao)
            {
                case TipoGeracaoRegraProvisao.TermoQuitacao: return "Termo Quitação";
                case TipoGeracaoRegraProvisao.PrazoExcedido: return "Prazo Excedido";
                default: return string.Empty;
            }
        }
    }
}