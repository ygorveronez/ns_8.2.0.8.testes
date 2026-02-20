using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ConfiguracaoCTeEmitidoEmbarcador
    {

        public List<ConfiguracaoCTeEmitidoEmbarcadorComponente> Componentes { get; set; }
        
        public string DescricaoComponenteFreteLiquido { get; set; }

        public bool ValorFreteLiquidoDeveSerValorAReceber { get; set; }
        
        public bool ValorFreteLiquidoDeveSerValorAReceberSemICMS { get; set; }
    }
}
