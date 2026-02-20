using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas
{
    public class ParametrosOfertasConsulta
    {
        public ParametrosOfertas ParametrosOfertas { get; set; }
        public List<Enumeradores.TipoIntegracao> TiposIntegracao { get; set; }
        public List<ParametrosOfertasDadosOfertaConsulta> ParametrosOfertasDadosOfertas { get; set; }
        public List<RelacionamentoParametrosOfertasCodigosDescricao> Empresas { get; set; }
        public List<RelacionamentoParametrosOfertasCodigosDescricao> Filiais { get; set; }
        public List<RelacionamentoParametrosOfertasCodigosDescricao> Funcionarios { get; set; }
        public List<RelacionamentoParametrosOfertasCodigosDescricao> TiposCarga { get; set; }
        public List<RelacionamentoParametrosOfertasCodigosDescricao> TiposOperacao { get; set; }
    }
}
