using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class CarregamentoPreCarga
    {
        public List<Motorista> Motoristas { get; set; }

        public string NumeroPreCarga { get; set; }

        public Pessoas.Empresa Transportador { get; set; }

        public Frota.Veiculo Veiculo { get; set; }
    }
}
