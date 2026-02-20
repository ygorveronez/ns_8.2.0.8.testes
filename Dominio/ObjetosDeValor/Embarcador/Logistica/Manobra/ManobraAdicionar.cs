using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class ManobraAdicionar
    {
        public Entidades.Embarcador.Logistica.ManobraAcao Acao { get; set; }

        public Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        public Entidades.Embarcador.Logistica.AreaVeiculoPosicao LocalDestino { get; set; }

        public bool LocalDestinoObrigatorio { get; set; }

        public Entidades.Embarcador.GestaoPatio.OcorrenciaPatio OcorrenciaPatio { get; set; }

        public ICollection<Entidades.Veiculo> Reboques { get; set; }

        public Entidades.Veiculo Tracao { get; set; }
    }
}
