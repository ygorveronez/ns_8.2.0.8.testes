namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum FormaPagamentoMotorista
    {
        Nenhum = 0,
        Carga = 1,
        Descarga = 2
    }

    public static class FormaPagamentoMotoristaHelper
    {
        public static string ObterDescricao(this FormaPagamentoMotorista tipoPagamento)
        {
            switch (tipoPagamento)
            {
                case FormaPagamentoMotorista.Carga: return "Carga";
                case FormaPagamentoMotorista.Descarga: return "Descarga";
                default: return string.Empty;
            }
        }
    }
}
