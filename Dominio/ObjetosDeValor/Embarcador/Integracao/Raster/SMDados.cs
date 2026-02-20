using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class SMDados
    {
        public int? Codigo { get; set; }
        public SMDadosEngate Engate { get; set; }
        public SMDetalhamento Detalhamento { get; set; }
        public SMDetalhamentoRota Rota { get; set; }
        public SMDetalhamentoCheckList CheckList { get; set; }
        public SMDetalhamentoLiberacaoEngate LiberacaoEngate { get; set; }
        public List<SMDetalhamentoLocalizadorAvulso> LocalizadorAvulso { get; set; }
        public SMDetalhamentoEscolta EscoltaArmada { get; set; }
        public SMDetalhamentoEscolta EscoltaVelada { get; set; }
    }
}
