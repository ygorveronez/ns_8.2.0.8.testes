namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class ContratoVeiculo
    {
        public int Codigo { get; set; }
        public string Veiculo { get; set; }
        public int CodigoVeiculo { get; set; }
        public decimal ValorDiaria { get; set; }
        public decimal ValorQuinzena { get; set; }
        public int QuantidadeDias { get; set; }
        public decimal Total { get; set; }
        public int QuantidadeDocumentos { get; set; }
        public decimal ValorDocumentos { get; set; }
    }
}
