namespace Dominio.ObjetosDeValor.Enumerador
{
    public enum TipoMovimentoContrato
    {
        Debito = 0,
        Credito = 1,
    }
    public static class TipoMovimentoContratoHelper
    {
        public static string ObterDescricao(this TipoMovimentoContrato tipoMovimentoContrato)
        {
            string retorno = "";
            switch (tipoMovimentoContrato)
            {
                case TipoMovimentoContrato.Debito:
                    retorno = "Débito";
                    break;
                case TipoMovimentoContrato.Credito:
                    retorno = "Crédito";
                    break;
            }
            return retorno;
        }
    }
}