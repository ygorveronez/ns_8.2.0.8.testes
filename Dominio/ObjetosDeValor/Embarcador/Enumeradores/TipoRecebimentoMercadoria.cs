using System;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRecebimentoMercadoria
    {
        Mercadoria = 1,
        Volume = 2
    }


    public static class TipoRecebimentoMercadoriaHelper
    {
        public static string ObterDescricao(this TipoRecebimentoMercadoria tipo)
        {
            switch (tipo)
            {
                case TipoRecebimentoMercadoria.Mercadoria: return "Mercadoria";
                case TipoRecebimentoMercadoria.Volume: return "Volume";
                default: return String.Empty;
            }
        }
    }
}
