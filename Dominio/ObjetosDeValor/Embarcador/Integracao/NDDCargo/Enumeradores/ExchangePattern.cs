using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores
{
    [Serializable]
    public enum ExchangePattern
    {
        [XmlEnum("1")]
        RequisicaoSincrona = 1,

        [XmlEnum("7")]
        RequisicaoAssincrona = 7,

        [XmlEnum("8")]
        RespostaAssincrona = 8
    }
}
