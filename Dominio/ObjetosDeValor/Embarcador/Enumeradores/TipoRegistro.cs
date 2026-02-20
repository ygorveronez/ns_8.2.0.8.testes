namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRegistro
    {
        SemTipo = 0,
        Pagoviacreditoemconta = 1,
        PendentesemAberto = 2,
        Debitoscompensados = 3,
        PagoviaConfirming = 4,
        NotasCompensadasXAdiantamento = 5,
        Cockpit = 6,
        BaixaResultado = 7,
        Descontos = 8,
        TotaldeAdiantamento = 9
    }

    public static class TipoRegistroHelper
    {
        public static string ObterDescricao(this TipoRegistro tipo)
        {
            switch (tipo)
            {
                case TipoRegistro.SemTipo:return "Sem Tipo";
                case TipoRegistro.Pagoviacreditoemconta:return "Pago via crédito em conta";
                case TipoRegistro.PendentesemAberto: return "Pendentes em aberto";
                case TipoRegistro.Debitoscompensados: return "Débitos compensados";
                case TipoRegistro.PagoviaConfirming:return "Pago via confirming";
                case TipoRegistro.NotasCompensadasXAdiantamento: return "Notas compensadas x adiantamento";
                case TipoRegistro.Cockpit: return "Cockpit";
                case TipoRegistro.BaixaResultado: return "Baixa resultado";
                case TipoRegistro.Descontos: return "Descontos";
                case TipoRegistro.TotaldeAdiantamento: return "Total de adiantamentos";
                default: return string.Empty;
            }

        }

        public static string ObterSimilitudNome(this TipoRegistro tipo)
        {
            switch (tipo)
            {
                case TipoRegistro.SemTipo: return "SemTipo";
                case TipoRegistro.Pagoviacreditoemconta: return "PagoViaCreditoemconta";
                case TipoRegistro.PendentesemAberto: return "PendentesEmAberto";
                case TipoRegistro.Debitoscompensados: return "DebitosCompensados";
                case TipoRegistro.PagoviaConfirming: return "PagoViaConfirming";
                case TipoRegistro.NotasCompensadasXAdiantamento: return "NFExAdiantamento";
                case TipoRegistro.Cockpit: return "Cockpit";
                case TipoRegistro.BaixaResultado: return "Baixaresultado";
                case TipoRegistro.Descontos: return "Descontos";
                case TipoRegistro.TotaldeAdiantamento: return "TotalDeAdiantamento";
                default: return string.Empty;
            }

        }
    }
}
