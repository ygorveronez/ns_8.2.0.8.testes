namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoInteracaoEntrega
    {
        Mobile = 1,
        Manual = 2,
        Hibrido = 3
    }


    public static class TipoInteracaoEntregaHelper
    {

        public static string ObterDescricao(this TipoInteracaoEntrega tipoInteracao)
        {
            switch (tipoInteracao)
            {
                case TipoInteracaoEntrega.Mobile: return "Mobile";
                case TipoInteracaoEntrega.Manual: return "Manual";
                case TipoInteracaoEntrega.Hibrido: return "Híbrido";
                default: return "";
            }
        }

    }
}

