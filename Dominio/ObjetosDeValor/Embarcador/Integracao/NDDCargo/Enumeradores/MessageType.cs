using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores
{
    [Serializable]
    public enum MessageType
    {
        [XmlEnum("100")]
        Insert = 100
    }
}
