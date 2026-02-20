namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoFrota
    {
        NaoDefinido = 0,
        Fixo = 1,
        Spot = 2,
        Projeto = 3
    }

    public static class TipoFrotaHelper
    {
        public static string ObterDescricao(this TipoFrota tipo)
        {
            switch (tipo)
            {
                case TipoFrota.Fixo: return "Fixo";
                case TipoFrota.NaoDefinido: return "Não Definido";
                case TipoFrota.Spot: return "Spot";
                case TipoFrota.Projeto: return "Projeto";
                default: return string.Empty;
            }
        }
    }
}
