using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioHistoricosPneus
    {
        public int CodigoPneu { get; set; }
        public int CodigoVeiculo { get; set; }
        public DateTime? Data { get; set; }
        public string Eixo { get; set; }
        public string Veiculo { get; set; }
        public int KMAtualVeiculo { get; set; }
        public int KMTroca { get; set; }
        public string TipoHistorico { get; set; }
        public string SeriePneu { get; set; }
        public string Observacao { get; set; }
        public string TipoVeiculo { get; set; }
        public int Ordem { get; set; }

        public string DescricaoTipoHistorico
        {
            get
            {
                switch (this.TipoHistorico)
                {
                    case "E": return "Entrada";
                    case "S": return "Saída";
                    default: return "";
                }
            }
        }
    }
}
