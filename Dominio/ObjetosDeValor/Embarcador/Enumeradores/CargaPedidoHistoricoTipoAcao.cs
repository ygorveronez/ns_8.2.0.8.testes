namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CargaPedidoHistoricoTipoAcao
    {
        Exclusao = 1
    }


    public static class CargaPedidoHistoricoTipoAcaoHelper
    {
        public static string ObterDescricao(this CargaPedidoHistoricoTipoAcao o)
        {
            switch (o)
            {
                case CargaPedidoHistoricoTipoAcao.Exclusao: return "Exclusão";
                default: return string.Empty;
            }
        }
    }
}