namespace Dominio.ObjetosDeValor.ImpostoMotorista
{
    public class ParametrosCalculo
    {
        public string CPF_CNPJ_Contratado { get; set; }
        public decimal ValorFreteBruto { get; set; }
        public decimal ValorFreteAcumulado { get; set; }
        public decimal ValorRetidoINSSAcumuladoContratado { get; set; }
        public decimal DeducaoImpostoRetidoFonteIRRF { get; set; }
        public int QuantidadeDependentes { get; set; }
        public decimal ValorRetidoSESTSENATAcumuladoContratado { get; set; }
        
        //public decimal BaseAcumuladaIRRF { get; set; }
        //public decimal BaseAcumuladaINSS { get; set; }
    }
}
