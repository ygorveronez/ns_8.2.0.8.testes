using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaSolicitacaoAbastecimentoGas
    {
        public List<double> CodigosBasesSatelite { get; set; }
        public List<double> CodigosBasesSupridora { get; set; }
        public int CodigoUsuario { get; set; }
        public List<double> CodigosSupridoresPermitidos { get; set; }
        public DateTime? DataCriacaoInicial { get; set; }
        public DateTime? DataCriacaoFinal { get; set; }
        public DateTime? DataSolicitacaoInicial { get; set; }
        public DateTime? DataSolicitacaoFinal { get; set; }
        public DateTime? DataSolicitacao { get; set; }
        public bool PossuiVolumeRodoviario { get; set; }
        public bool PossuiDisponibilidadeDeTransferencia { get; set; }
        public bool AgruparPorDia { get; set; }
        public SituacaoAprovacaoSolicitacaoGas? Situacao { get; set; }
    }
}
