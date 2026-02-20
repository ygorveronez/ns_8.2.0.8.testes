namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRankingBidding
    {
        GridRankingOfertas = 1,
        GridRankingCherryPickingOferas = 2,       
    }

    public static class TipoRankingBiddingHelper
    {
        public static string ObterDescricao(this TipoRankingBidding tipo)
        {
            switch (tipo)
            {
                case TipoRankingBidding.GridRankingOfertas: return "GridRankingOferas";
                case TipoRankingBidding.GridRankingCherryPickingOferas: return "GridRankingCherryPickingOferas";                
                default: return string.Empty;
            }
        }
    }
}