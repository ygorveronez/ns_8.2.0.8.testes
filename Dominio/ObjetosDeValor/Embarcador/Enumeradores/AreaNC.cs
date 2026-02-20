namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AreaNC
    {
        NaoSelecionado = 0,
        Transporte = 1,
        Planejamento = 2,
        Comex = 3, 
        Entradas = 4
    }
    public static class AreaNCHelper
    {
        public static string ObterDescricao(this AreaNC status)
        {
            switch (status)
            {
                case AreaNC.Transporte: return "Transporte";
                case AreaNC.Planejamento: return "Planejamento";
                case AreaNC.Comex: return "Comex";
                case AreaNC.Entradas: return "Entradas";
                default: return string.Empty;
            }
        }
    }
}
