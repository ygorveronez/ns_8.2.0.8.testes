using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class OcorrenciaPatioAdicionar
    {
        public Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        public string Descricao { get; set; }

        public ICollection<Entidades.Veiculo> Reboques { get; set; }

        public Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo Tipo { get; set; }

        public TipoLancamento TipoLancamento { get; set; }

        public Entidades.Veiculo Tracao { get; set; }
    }
}
