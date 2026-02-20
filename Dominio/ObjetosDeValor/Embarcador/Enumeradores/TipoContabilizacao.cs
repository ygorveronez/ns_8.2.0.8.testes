namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoContabilizacao
    {
        Todos = 0,
        Credito = 1,
        Debito = 2,
        NotaDebito = 3,
        ContasAPagar = 4,
        ContasAReceber = 5
    }

    public static class TipoContabilizacaoHelper
    {
        public static string ObterDescricao(this TipoContabilizacao situacao)
        {
            switch (situacao)
            {
                case TipoContabilizacao.Credito: return "Crédito";
                case TipoContabilizacao.Debito: return "Débito";
                case TipoContabilizacao.NotaDebito: return "Nota de Débito";
                case TipoContabilizacao.ContasAPagar: return "Contas a Pagar";
                case TipoContabilizacao.ContasAReceber: return "Contas a Pagar";
                default: return string.Empty;
            }
        }
    }
}