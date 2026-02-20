using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaFilaCarregamentoVeiculoReversa
    {
        public int CodigoCentroCarregamento { get; set; }

        public int CodigoGrupoModeloVeicularCarga { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public List<Enumeradores.SituacaoFilaCarregamentoVeiculoReversa> Situacoes { get; set; }
    }
}
