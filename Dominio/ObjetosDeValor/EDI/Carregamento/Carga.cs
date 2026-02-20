using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.Carregamento
{
    public class Carga
    {
        public string Identificador { get; set; }
        public string CNPJEmbarcador { get; set; }
        public string CNPJTransportador { get; set; }
        public string EANEmbarcador { get; set; }
        public string EANTransportador { get; set; }

        public string CodigoParceiroComercial { get; set; }
        public string TripName { get; set; }
        public int NumeroCarregamento { get; set; }
        public string CarrierCode { get; set; }
        public string ModeloTransporte { get; set; }
        public string SequenciaChegada { get; set; }
        public string TipoEquipamento { get; set; }
        public string NivelServico { get; set; }
        public string CodigoTransportadorRedespacho { get; set; }
        public string LocalEmbarque { get; set; }
        public string SiteEmbarque { get; set; }
        public string PaisEmbarque { get; set; }
        public string CodigoTipoEquipamento { get; set; }
        public string ModeloCarregamento { get; set; }

        public List<Pedido> Pedidos { get; set; }

        public Rodape Rodape { get; set; }


    }
}
