namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoModal
    {
        Todos = 0,
        Rodoviario = 1,
        Aereo = 2,
        Aquaviario = 3,
        Ferroviario = 4,
        Dutoviario = 5,
        Multimodal = 6
    }

    public static class TipoModalHelper
    {
        public static string ObterDescricao(this TipoModal tipo)
        {
            switch (tipo)
            {
                case TipoModal.Rodoviario: return "Rodoviário";
                case TipoModal.Aereo: return "Aéreo";
                case TipoModal.Aquaviario: return "Aquaviário";
                case TipoModal.Ferroviario: return "Ferroviário";
                case TipoModal.Dutoviario: return "Dutoviário";
                case TipoModal.Multimodal: return "Multimodal";
                default: return string.Empty;
            }
        }

        public static string ObtertODOSDescricao(this TipoModal tipo)
        {
            switch (tipo)
            {
                case TipoModal.Todos: return "Todos";
                case TipoModal.Rodoviario: return "Rodoviário";
                case TipoModal.Aereo: return "Aéreo";
                case TipoModal.Aquaviario: return "Aquaviário";
                case TipoModal.Ferroviario: return "Ferroviário";
                case TipoModal.Dutoviario: return "Dutoviário";
                case TipoModal.Multimodal: return "Multimodal";
                default: return string.Empty;
            }
        }
    }
}
