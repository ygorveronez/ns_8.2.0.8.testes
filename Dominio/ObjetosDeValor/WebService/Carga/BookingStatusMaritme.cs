using System;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class BookingStatusMaritme
    {
        private string _statusCode;


        [DataMember(Order = 1, IsRequired = true)]
        public string StatusCode 
        { get => _statusCode;
          set => _statusCode = ValidateAndConvertStatusCode(value); }

        [DataMember(Order = 2, IsRequired = true)]
        public DateTime StatusDateTime { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string UTCTimeDifference { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public string StatusLocation { get; set; }

        [DataMember(Order = 5, IsRequired = true)]
        public string StatusRemarks { get; set; }

        private string ValidateAndConvertStatusCode(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("StatusCode não pode ser nulo ou vazio.");
            }
            string upperValue = value.ToUpperInvariant();

            return upperValue;
        }
    }
}
