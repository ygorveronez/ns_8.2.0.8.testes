using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public class VeiculosRastreadores
    {
        public string Placa { get; set; }
        public string TipoVeiculo { get; set; }
        public string Status { get; set; }
        public List<Rastreadores> Rastreadores { get; set; }
    }
}
