using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class AuditoriaOrdemServicoServico
    {
        public int Codigo { get; set; }
        public int CodigoOrdemServico { get; set; }
        public int Numero { get; set; }
        public string Servico { get; set; }
        public string Observacao { get; set; }
        public int QuilometragemOrdemServico { get; set; }
        public int HorimetroOrdemServico { get; set; }
        public DateTime Data { get; set; }
    }
}
