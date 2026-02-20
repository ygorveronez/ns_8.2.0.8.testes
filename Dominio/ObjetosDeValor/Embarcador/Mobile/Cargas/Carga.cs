using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas
{
    public class Carga
    {
        public int CodigoIntegracao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.Multisoftware.ClienteMultisoftware ClienteMultisoftware { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCarga { get; set; }
        public string NumeroCargaEmbarcador { get; set; }
        public string DataCarga { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Filial { get; set; }
        public string TipoCarga { get; set; }
        public string Peso { get; set; }
        public string DataSaida { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto> Canhotos { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido> Pedidos { get; set; }
    }
}

