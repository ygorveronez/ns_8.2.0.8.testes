namespace Dominio.ObjetosDeValor.EDI.DESPESACOMPLEMENTAR
{
    public class Despesa
    {
        public string Registro { get; set; }
        public string ChaveRedespacho { get; set; } 
        public string Evento { get; set; }
        public string GrupoCusto { get; set; }
        public string NumeroDespesa { get; set; }
        public decimal TipoCobranca { get; set; }
        public decimal ValorDespesa { get; set; }
    }
}
