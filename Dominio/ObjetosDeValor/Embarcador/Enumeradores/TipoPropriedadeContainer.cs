namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPropriedadeContainer
    {
        Nenhum = 0,
        Proprio = 1,
        Soc = 2
    }

    public static class TipoPropriedadeContainerHelper
    {
        public static string ObterDescricao(this TipoPropriedadeContainer propriedadeContainer)
        {
            switch (propriedadeContainer)
            {
                case TipoPropriedadeContainer.Proprio: return "Próprio";
                case TipoPropriedadeContainer.Soc: return "SOC";
                default: return string.Empty;
            }
        }
    }
}