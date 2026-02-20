namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaOcorrenciaSumarizado
    {
        public int CodigoVeiculo { get; set; }

        public string Veiculo { get; set; }

        public string ModeloVeicular { get; set; }

        public int QuantidadeCargas { get; set; }

        public int QuantidadeDias { get; set; }
        
        public decimal ValorNotas { get; set; }
        
        public int QuantidadeDocumentos { get; set; }
    }
}
