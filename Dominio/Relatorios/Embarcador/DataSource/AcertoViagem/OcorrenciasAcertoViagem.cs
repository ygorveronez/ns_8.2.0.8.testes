using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class OcorrenciasAcertoViagem
    {
        public int CodigoAcerto { get; set; }
        public int Codigo { get; set; }
        public int NumeroOcorrencia { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string Veiculo { get; set; }
        public string TipoOcorrencia { get; set; }
        public string Motorista { get; set; }
        public string Componente { get; set; }
        public decimal FreteLiquido { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal Valor { get; set; }
        public string DescricaoSituacao { get; set; }
        public decimal ValorComissao { get; set; }
    }
}
