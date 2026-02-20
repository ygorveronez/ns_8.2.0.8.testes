namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaEmissaoSVM
    {
        Nenhum = 0,
        PortoOrigem = 1,
        PortoTransbordo = 2,
        PortoDestino = 3
    }

    public static class FormaEmissaoSVMHelper
    {
        public static string ObterDescricao(this FormaEmissaoSVM forma)
        {
            switch (forma)
            {
                case FormaEmissaoSVM.Nenhum: return "Nenhum";
                case FormaEmissaoSVM.PortoOrigem: return "Porto de Origem";
                case FormaEmissaoSVM.PortoTransbordo: return "Porto de Transbordo";
                case FormaEmissaoSVM.PortoDestino: return "Porto de Destino";
                default: return string.Empty;
            }
        }
    }
}
