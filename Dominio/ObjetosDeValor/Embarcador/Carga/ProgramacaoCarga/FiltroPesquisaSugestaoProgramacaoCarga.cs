using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga
{
    public sealed class FiltroPesquisaSugestaoProgramacaoCarga
    {
        public int CodigoConfiguracaoProgramacaoCarga { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<int> CodigosModeloVeicularCarga { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public DateTime? DataProgramacaoFinal { get; set; }

        public DateTime? DataProgramacaoInicial { get; set; }

        public List<Enumeradores.SituacaoSugestaoProgramacaoCarga> Situacao { get; set; }
    }
}
