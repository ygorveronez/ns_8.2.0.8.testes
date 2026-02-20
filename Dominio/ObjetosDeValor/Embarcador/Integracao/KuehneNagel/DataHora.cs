using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class DataHora
    {
        #region Propriedades

        [XmlElement(ElementName = "Date")]
        public string Data { get; set; }

        [XmlElement(ElementName = "Time")]
        public string Hora { get; set; }

        [XmlElement(ElementName = "UTCTimeDifference")]
        public string TimeZone { get; set; }

        #endregion Propriedades

        #region Construtores

        public DataHora() { }

        public DataHora(DateTime dataHora)
        {
            Data = dataHora.ToString("yyyy-MM-dd");
            Hora = dataHora.ToString("HH:mm:ss");
        }

        #endregion Construtores

        #region Métodos Públicos

        public DateTime? ObterDataHora()
        {
            if (string.IsNullOrWhiteSpace(Data))
                return null;

            string dataConverter = $"{Data} {(string.IsNullOrWhiteSpace(Hora) ? "00:00:00" : Hora)} {(string.IsNullOrWhiteSpace(TimeZone) ? "-03:00" : TimeZone)}";

            return dataConverter.ToNullableDateTime(formato: "yyyy-MM-dd HH:mm:ss K");
        }

        #endregion
    }
}
