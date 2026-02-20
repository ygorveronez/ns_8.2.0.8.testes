using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.MICDTA
{
    public class Carga
    {
        public Consignatario consignatario { get; set; }
        public Remetente remetente { get; set; }
        public Destinatario destinatario { get; set; }

        public string codigoAduanaDestino { get; set; }
        public string nomeAduanaDestino { get; set; }
        public string paisOrigemMercadorias { get; set; }
        public string valorFOTMercadorias { get; set; }
        public string moedaValorFOT { get; set; }
        public string valorSeguro { get; set; }
        public string moedaValorSeguro { get; set; }
        public string codigoTiposVolumes { get; set; }
        public string nomeTiposVolumes { get; set; }
        public int qtdeVolumes { get; set; }
        public string pesoBruto { get; set; }
        public string descricaoMercadorias { get; set; }
        public string documentosAnexos { get; set; }
        public List<nfe> nfes { get; set; }
    }
}
