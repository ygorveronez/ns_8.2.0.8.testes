using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class DadosTransporteMaritimo
    {
        [DataMember]
        public string CodigoIdentificacaoCarga { get; set; }

        [DataMember]
        public string DescricaoIdentificacaoCarga { get; set; }

        [DataMember]
        public string CodigoNCM { get; set; }

        [DataMember]
        public string MetragemCarga { get; set; }

        [DataMember]
        public string Incoterm { get; set; }

        [DataMember]
        public string Transbordo { get; set; }

        [DataMember]
        public string MensagemTransbordo { get; set; }

        [DataMember]
        public string NumeroBL { get; set; }

        [DataMember]
        public string NumeroViagem { get; set; }

        [DataMember]
        public string DataBooking { get; set; }

        [DataMember]
        public string DataETASegundaOrigem { get; set; }

        [DataMember]
        public string DataETADestino { get; set; }

        [DataMember]
        public string DataETASegundoDestino { get; set; }

        [DataMember]
        public string DataETADestinoFinal { get; set; }

        [DataMember]
        public string DataRetiradaContainerDestino { get; set; }

        [DataMember]
        public string DataETSTransbordo { get; set; }

        [DataMember]
        public string DataETATransbordo { get; set; }

        [DataMember]
        public string DataDepositoContainer { get; set; }

        [DataMember]
        public string CodigoRota { get; set; }

        [DataMember]
        public string NumeroViagemTransbordo { get; set; }

        [DataMember]
        public string NomeNavioTransbordo { get; set; }

        [DataMember]
        public string CodigoPortoCarregamentoTransbordo { get; set; }

        [DataMember]
        public string DescricaoPortoCarregamentoTransbordo { get; set; }

        [DataMember]
        public string CodigoPortoDestinoTransbordo { get; set; }

        [DataMember]
        public string DescricaoPortoDestinoTransbordo { get; set; }

        [DataMember]
        public string TipoEnvio { get; set; }

        [DataMember]
        public string Halal { get; set; }

        [DataMember]
        public string CodigoViagem { get; set; }

        [DataMember]
        public string SegundaDataDeadLineCarga { get; set; }

        [DataMember]
        public ObjetosDeValor.Embarcador.Enumeradores.FretePrepaid FretePrepaid { get; set; }

        [DataMember]
        public string SegundaDataDeadLineDraf { get; set; }

        [DataMember]
        public string CodigoContratoFOB { get; set; }

        

    }
}
