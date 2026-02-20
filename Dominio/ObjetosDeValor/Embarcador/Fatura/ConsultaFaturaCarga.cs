namespace Dominio.ObjetosDeValor.Embarcador.Fatura
{
    public class ConsultaFaturaCarga
    {
        public int Codigo { get; set; }
        public int CodigoFatura { get; set; }
        public int CodigoTitulo { get; set; }

        public decimal Valor { get; set; }

        public string NumerosFiscais { get; set; }
        public string NumeroFatura { get; set; }
        public string NumeroBoletos { get; set; }
        public string NumeroTitulos { get; set; }
        public string Vencimento { get; set; }
        public string Emissao { get; set; }
        public string NumeroControle { get; set; }
        public string Tomador { get; set; }
        public string Situacao { get; set; }
        public string TipoCTe { get; set; }
    }
}
