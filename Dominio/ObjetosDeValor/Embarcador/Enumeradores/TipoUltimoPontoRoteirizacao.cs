namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoUltimoPontoRoteirizacao
    {
        Todos = 0,
        Retornando = 1,
        AteOrigem = 2,
        PontoMaisDistante = 3
    }

    public static class TipoUltimoPontoRoteirizacaoHelper
    {
        public static string ObterDescricao(this TipoUltimoPontoRoteirizacao tipo)
        {
            switch (tipo)
            {
                case TipoUltimoPontoRoteirizacao.Retornando: return "Retorno Vazio";
                case TipoUltimoPontoRoteirizacao.AteOrigem: return "Até a Origem";
                case TipoUltimoPontoRoteirizacao.PontoMaisDistante: return "Ponto mais Distante";
                default: return string.Empty;
            }
        }
    }
}
