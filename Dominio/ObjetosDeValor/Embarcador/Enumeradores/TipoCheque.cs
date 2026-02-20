namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCheque
    {
        Caucao = 1,
        Pago = 2,
        Recebido = 3,
        Repassado = 4,
        Emitido = 5
    }

    public static class TipoChequeHelper
    {
        public static string ObterDescricao(this TipoCheque tipo)
        {
            switch (tipo)
            {
                case TipoCheque.Caucao: return "Caução";
                case TipoCheque.Emitido: return "Emitido";
                case TipoCheque.Pago: return "Pago";
                case TipoCheque.Recebido: return "Recebido";
                case TipoCheque.Repassado: return "Repassado";
                default: return string.Empty;
            }
        }
    }
}
