namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoLocal
    {
        AreaDeRisco = 1,
        Pernoite = 2,
        MicroRegiaoRoteirizacao = 3,
        PontoDeApoio = 4,
        Balanca = 5,
        ZonaExclusaoRota = 6,
        RaioProximidade = 7
    }

    public static class TipoLocalHelper
    {
        public static string ObterDescricao(this TipoLocal tipoLocal)
        {
            switch (tipoLocal)
            {
                case TipoLocal.AreaDeRisco: return "Área de risco";
                case TipoLocal.Pernoite: return "Pernoite";
                case TipoLocal.MicroRegiaoRoteirizacao: return "Micro região roteirização";
                case TipoLocal.PontoDeApoio: return "Ponto de apoio";
                case TipoLocal.Balanca: return "Balança";
                case TipoLocal.ZonaExclusaoRota: return "Zona de exclusão de rota";
                case TipoLocal.RaioProximidade: return "Raio de proximidade";
                default: return string.Empty;
            }
        }
    }
}