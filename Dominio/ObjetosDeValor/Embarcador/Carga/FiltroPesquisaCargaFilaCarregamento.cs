using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaFilaCarregamento
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? TipoServicoMultisoftware { get; set; }
    }
}
