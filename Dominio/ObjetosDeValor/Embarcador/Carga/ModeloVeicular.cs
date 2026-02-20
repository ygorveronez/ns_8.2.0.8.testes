using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class ModeloVeicular
    {
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga TipoModeloVeicular { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular> DivisaoCapacidade { get; set; }
        public decimal QuantidadeExtraExcedenteTolerado { get; set; }
        public string CodigoAgrupamentoCarga { get; set; }
        public int CodigoInterno { get; set; }
        public int Protocolo { get; set; }
        public decimal Capacidade { get; set; }
        public decimal ToleranciaExtra { get; set; }
    }
}
