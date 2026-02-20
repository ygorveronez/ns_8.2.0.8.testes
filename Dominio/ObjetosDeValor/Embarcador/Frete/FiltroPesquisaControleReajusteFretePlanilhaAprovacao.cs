using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaControleReajusteFretePlanilhaAprovacao
    {
        public int CodigoEmpresa { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoUsuario { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataInicial { get; set; }

        public int Numero { get; set; }

        public SituacaoControleReajusteFretePlanilha? Situacao { get; set; }
    }
}
