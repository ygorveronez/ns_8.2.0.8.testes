using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class ResultadoAcertoViagem
    {
        public int CodigoTracao { get; set; }
        public string PlacaTracao { get; set; }
        public int CodigoModeloTracao { get; set; }
        public string ModeloTracao { get; set; }
        public string MarcaVeiculo { get; set; }
        public string CodigoSegmento { get; set; }
        public string Segmento { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public string GrupoPessoa { get; set; }
        public int CodigoModeloVeicular { get; set; }
        public string ModeloVeicular { get; set; }
        public int AnoTracao { get; set; }
        public string FrotaTracao { get; set; }
        public int NumeroAcertoViagem { get; set; }
        public int QuantidadeDias { get; set; }
        public DateTime DataFinal { get; set; }
        public int CodigoMotorista { get; set; }
        public string NomeMotorista { get; set; }
        public decimal FaturamentoBruto { get; set; }
        public decimal ResultadoLiquido { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal CombustivelTracao { get; set; }
        public decimal CombustivelTracaoKm { get; set; }
        public decimal CombustivelEquipamentos { get; set; }
        public decimal CombustivelEquipamentosKm { get; set; }
        public int KMTotal { get; set; }
        public decimal ValorKMSemICMS { get; set; }
        public decimal ValorKMComICMS { get; set; }
        public decimal TotalMesComICMS { get; set; }
        public decimal ParametroMedia { get; set; }
        public decimal Media { get; set; }
        public string Observacao { get; set; }
        public decimal DieselKM { get; set; }
        public decimal Ocorrencias { get; set; }

        public decimal LitrosTracao { get; set; }
        public decimal LitrosReboque { get; set; }
        public decimal MediaLitroCavalo { get; set; }
        public decimal MediaLitroReboque { get; set; }

        public decimal PedagioPago { get; set; }
        public decimal PedagioRecebido { get; set; }

        public string CodigoIntegracaoMotorista { get; set; }
    }
}
