using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class DadosNotaProcessar
    {
        public int CodigoIntegradora { get; set; }
        public Dominio.ObjetosDeValor.WebService.Carga.Protocolos Protocolo { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> NotasFiscais { get; set; }
    }
}
