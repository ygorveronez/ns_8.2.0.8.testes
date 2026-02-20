using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCarregamentoAprovacao
    {
        public List<int> CodigosFilial { get; set; }

        public List<int> CodigosModeloVeicularCarga { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public int CodigoUsuario { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public string NumeroCarregamento { get; set; }

        public Enumeradores.SituacaoCarregamentoSolicitacao? SituacaoCarregamentoSolicitacao { get; set; }
    }
}
