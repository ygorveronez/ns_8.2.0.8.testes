namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEmissao
    {
        Todos = 0,
        EmissaoCarga = 1,
        EmissaoManual = 2,
        EmissaoAgrupado = 3
    }

    public static class EnumTipoEmissaoeHelper
    {
        public static string ObterDescricao(this TipoEmissao tipo)
        {
            switch (tipo)
            {
                case TipoEmissao.Todos: return "Todos";
                case TipoEmissao.EmissaoCarga: return "Emissão Carga";
                case TipoEmissao.EmissaoManual: return "Emissão Manual";
                case TipoEmissao.EmissaoAgrupado: return "Emissão Agrupado";
                default: return string.Empty;
            }
        }
    }
}
