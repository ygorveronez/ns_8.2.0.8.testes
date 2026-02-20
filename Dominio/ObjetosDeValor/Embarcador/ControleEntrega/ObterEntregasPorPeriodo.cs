using System;

namespace Dominio.ObjetosDeValor.Embarcador.ControleEntrega
{
    public class ObterEntregasPorPeriodo
    {
        public string destino { get; set; }
        public DateTime? data_inicio { get; set; }
        public DateTime? data_fim { get; set; }
    }
}
