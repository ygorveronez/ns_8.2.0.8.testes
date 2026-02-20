using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class SimuladorFreteCarregamento
    {
        public int CodigoSimuladorFrete { get; set; }
        public int CodigoPedido { get; set; }
        public DateTime? DataCarregamento { get; set; }
        public int CodigoCarregamento { get; set; }
        public string NumeroCarregamento { get; set; }
        public bool ExigeIsca { get; set; }
    }
}
