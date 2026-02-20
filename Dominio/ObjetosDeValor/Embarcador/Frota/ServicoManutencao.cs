using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class ServicoManutencao
    {
        public Servico Servico { get; set; }
        public DateTime? DataUltimaManutencao { get; set; }
        public int? KMUltimaManutencao { get; set; }
        public int TempoEstimado { get; set; }
        public decimal CustoMedio { get; set; }
        public decimal CustoEstimado { get; set; }
        public string Observacao { get; set; }
    }
}
