using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores
{
    [Serializable]
    public enum ProcessCode
    {
        [XmlEnum("1004")]
        GerarGUID = 1004,

        [XmlEnum("2006")]
        SolicitarImpressao = 2006,

        [XmlEnum("2018")]
        DownloadOperacao = 2018,

        [XmlEnum("2019")]
        OperacaoValePedagio = 2019,

        [XmlEnum("2021")]
        ConsultarOperacaoValePedagio = 2021,

        [XmlEnum("2022")]
        CancelarOperacaoValePedagio = 2022        
    }
}
