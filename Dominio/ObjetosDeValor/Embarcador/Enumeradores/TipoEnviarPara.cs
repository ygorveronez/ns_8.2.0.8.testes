namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEnviarPara
    {
        Transportador = 1,
        Fornecedor = 2,
        OperadorME = 3
    }

    public static class TipoEnviarParaHelper
    {
        public static string ObterDescricao(this TipoEnviarPara tipo)
        {
            switch (tipo)
            {
                case TipoEnviarPara.Transportador: return "Transportador";
                case TipoEnviarPara.Fornecedor: return "Fornecedor";
                case TipoEnviarPara.OperadorME: return "Operador ME";
                default: return string.Empty;
            }
        }
    }
}
