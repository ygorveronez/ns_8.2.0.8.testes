using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaFilaCarregamentoVeiculoVinculoExterno
    {
        public Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        public ICollection<Entidades.Veiculo> Reboques { get; set; }

        public Enumeradores.SituacaoFilaCarregamentoVeiculo Situacao { get; set; }

        public Entidades.Veiculo Tracao { get; set; }
    }
}
