using Dominio.ObjetosDeValor.Embarcador.Carga;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.MDFe
{
    public class MDFeAquaviario
    {
        public MDFe MDFe { get; set; }//
        public List<TerminalPorto> TerminaisDestino { get; set; }
        public List<TerminalPorto> TerminaisOrigem { get; set; }
        public Empresa TransportadoraEmitente { get; set; }
        public Embarcador.Carga.Viagem Viagem { get; set; }//
        public Porto PortoEmbarque { get; set; }//
        public Porto PortoDesembarque { get; set; }//
        public Dominio.ObjetosDeValor.Localidade Origem { get; set; }//
        public Dominio.ObjetosDeValor.Localidade Destino { get; set; }//
        public ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe TipoModalMDFe { get; set; }//
        public bool UsarDadosCTe { get; set; }//
        public bool UsarSeguroCTe { get; set; }//
    }
}
