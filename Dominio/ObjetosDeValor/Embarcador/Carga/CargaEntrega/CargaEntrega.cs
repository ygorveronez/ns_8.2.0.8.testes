using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega
{
    class CargaEntrega
    {
    }
    public class CargaEntregaComValorNF
    {
        public double Remetente { get; set; }
        public double Tomador { get; set; }
        public double Recebedor { get; set; }
        public double Expedidor { get; set; }
        public DateTime PrevisaoEntrega { get; set; }
        public double ValorNf { get; set; }
    }
}
