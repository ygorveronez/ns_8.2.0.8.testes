namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GrupoNC
    {
        NaoSelecionado = 0,
        Transporte = 1,
        NotaFiscal = 2,
        Fiscal = 3,
        Importacao = 4,
        Pedido = 5,
        Materiais = 6
    }

    public static class GrupoNCHelper
    {
        public static string ObterDescricao(this GrupoNC status)
        {
            switch (status)
            {
                case GrupoNC.Transporte: return "Transporte";
                case GrupoNC.NotaFiscal: return "NF-e";
                case GrupoNC.Fiscal: return "Fiscal";
                case GrupoNC.Importacao: return "Importação";
                case GrupoNC.Pedido: return "Pedido";
                case GrupoNC.Materiais: return "Materiais";
                default: return string.Empty;
            }
        }
    }
}
