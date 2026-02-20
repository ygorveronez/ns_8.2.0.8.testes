using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoPropostaOcorrencia
    {
        Nenhum = 0,
        CargaFechada = 1,
        CargaFracionada = 2,
        Feeder = 3,
        VAS = 4
    }

    public static class TipoPropostaOcorrenciaHelper
    {
        public static string ObterDescricao(this TipoPropostaOcorrencia tipo)
        {
            switch (tipo)
            {
                case TipoPropostaOcorrencia.Nenhum: return "Nenhum";
                case TipoPropostaOcorrencia.CargaFechada: return "93 - Carga Fechada";
                case TipoPropostaOcorrencia.CargaFracionada: return "94 - Carga Fracionada";
                case TipoPropostaOcorrencia.Feeder: return "95 - Feeder";
                case TipoPropostaOcorrencia.VAS: return "96 - VAS";
                default: return string.Empty;
            }
        }
    }
}
