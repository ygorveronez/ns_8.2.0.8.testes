namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum LocalidadeBiddingFlag
    {
        Cliente,
        Cidade,
        Estado,
        Rota,
        CEP,
        Pais,
        Regiao
    }

    public static class LocalidadeBiddingFlagHelper
    {
        public static string ObterDescricao(this LocalidadeBiddingFlag origemTipo)
        {
            switch (origemTipo)
            {
                case LocalidadeBiddingFlag.Cliente: return "Cliente";
                case LocalidadeBiddingFlag.Cidade: return "Cidade";
                case LocalidadeBiddingFlag.Estado: return "Estado";
                case LocalidadeBiddingFlag.Rota: return "Rota";
                case LocalidadeBiddingFlag.CEP: return "CEP";
                case LocalidadeBiddingFlag.Pais: return "País";
                case LocalidadeBiddingFlag.Regiao: return "Região";
                default: return string.Empty;
            }
        }
    }
}
