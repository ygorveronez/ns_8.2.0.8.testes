namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoSeparacaoMercadoria
    {
        AguardandoSeparacaoMercadoria = 1,
        SeparacaoMercadoriaFinalizada = 2
    }

    public static class SituacaoSeparacaoMercadoriaHelper
    {
        public static string ObterDescricao(this SituacaoSeparacaoMercadoria situacao)
        {
            switch (situacao)
            {
                case SituacaoSeparacaoMercadoria.AguardandoSeparacaoMercadoria: return "Aguardando Separação de Mercadoria";
                case SituacaoSeparacaoMercadoria.SeparacaoMercadoriaFinalizada: return "Separação de Mercadoria Finalizada";
                default: return string.Empty;
            }
        }
    }
}
