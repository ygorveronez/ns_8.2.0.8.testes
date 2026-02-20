using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga
{
    public class ParametroSugestaoProgramacaoCarga
    {
        public int CodigoConfiguracaoProgramacaoCarga { get; set; }

        public DateTime DataHistoricoFinal { get; set; }

        public DateTime DataHistoricoInicial { get; set; }

        public DateTime DataProgramacaoFinal { get; set; }

        public DateTime DataProgramacaoInicial { get; set; }
    }
}
