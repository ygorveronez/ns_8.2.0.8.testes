namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAquisicaoPneu
    {
        Carcaca = 1,
        PneuEmUso = 2,
        PneuNovoReposicao = 3,
        PneuNovoVeiculoNovo = 4,
        PneuRecapado = 5,
        PneuUsado = 6
    }

    public static class TipoAquisicaoPneuHelper
    {
        public static string ObterDescricao(this TipoAquisicaoPneu tipoAquisicaoPneu)
        {
            switch (tipoAquisicaoPneu)
            {
                case TipoAquisicaoPneu.Carcaca: return "Carcaças";
                case TipoAquisicaoPneu.PneuEmUso: return "Pneus em Uso";
                case TipoAquisicaoPneu.PneuNovoReposicao: return "Pneus Novos - Reposição";
                case TipoAquisicaoPneu.PneuNovoVeiculoNovo: return "Pneus Novos - Veículos Novos";
                case TipoAquisicaoPneu.PneuRecapado: return "Pneus Recapados";
                case TipoAquisicaoPneu.PneuUsado: return "Pneus Usados";
                default: return string.Empty;
            }
        }
    }
}
