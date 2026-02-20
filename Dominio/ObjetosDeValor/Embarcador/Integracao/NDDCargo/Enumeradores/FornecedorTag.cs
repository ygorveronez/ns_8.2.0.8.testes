using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores
{
    [Serializable]
    public enum FornecedorTag
    {
        [XmlEnum("1")]
        ConectCar = 1,

        [XmlEnum("2")]
        SemParar = 2,

        [XmlEnum("3")]
        Veloe = 3,
    }
}
