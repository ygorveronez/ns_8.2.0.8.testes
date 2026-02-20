using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class RevisaoSMDados
    {
        public int? Codigo { get; set; }
        public RevisaoSMDetalhamento Detalhamento { get; set; }
        public RevisaoSMDetalhamentoRota Rota { get; set; }
        public RevisaoSMDetalhamentoCheckList CheckList { get; set; }
        public List<RevisaoSMDetalhamentoLocalizadorAvulso> LocalizadorAvulso { get; set; }
        public RevisaoSMDetalhamentoEscolta EscoltaArmada { get; set; }
        public RevisaoSMDetalhamentoEscolta EscoltaVelada { get; set; }
    }
}
