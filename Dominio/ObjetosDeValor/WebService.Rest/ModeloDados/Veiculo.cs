using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Veiculo
    {
        public string Placa { get; set; }

        public int CapacidadeKG { get; set; }

        public DateTime? DataValidadeGerenciadoraRisco { get; set; }

        public ModeloVeicularCarga ModeloVeicular { get; set; }

        public VeiculoRastreador Rastreador { get; set; }
    }
}
