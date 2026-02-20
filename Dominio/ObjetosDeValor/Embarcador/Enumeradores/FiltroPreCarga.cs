namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FiltroPreCarga
    {
        Todos = 0,
        ComCarga = 1,
        ComDadosInformados = 2,
        EmDia = 3,
        EmAtraso = 4,
        ProblemaVincularCarga = 5
    }

    public static class FiltroPreCargaHelper
    {
        public static string ObterDescricao(this FiltroPreCarga filtroPreCarga)
        {
            switch (filtroPreCarga)
            {
                case FiltroPreCarga.ComCarga: return "Com Carga";
                case FiltroPreCarga.ComDadosInformados: return "Com Dados Informados";
                case FiltroPreCarga.EmDia: return "Em Dia";
                case FiltroPreCarga.EmAtraso: return "Em Atraso";
                case FiltroPreCarga.ProblemaVincularCarga: return "Problema ao Vincular Carga";
                default: return string.Empty;
            }
        }
    }
}
