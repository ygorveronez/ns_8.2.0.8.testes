using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaAprovacaoSolicitacaoAbastecimentoGas
    {
        public double CodigoBase { get; set; }
        public int CodigoUsuario { get; set; }
        public DateTime? DataSolicitacaoInicial { get; set; }
        public DateTime? DataSolicitacaoFinal { get; set; }
        public SituacaoAprovacaoSolicitacaoGas? Situacao { get; set; }
    }
}
