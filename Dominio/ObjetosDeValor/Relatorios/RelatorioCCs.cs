namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCCs
    {
        // CT-e
        public int AgrupamentoCTe { get; set; }
        public int NumeroCTe { get; set; }
        public int NumeroSerie { get; set; }
        public string DataEmissaoCTe { get; set; }

        // CC-e
        public int AgrupamentoCCe { get; set; }
        public int NumeroCCe { get; set; }
        public string Protocolo { get; set; }
        public string DataEmissaoCCe { get; set; }
        public string DataAutorizacaoCCe { get; set; }

        // Campos CC-e
        public string Campo { get; set; }
        public string Valor { get; set; }
    }
}
