namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever
{
    public class RequestTruckAssignment
    {
        /// <summary>
        /// Alphanumeric identifier that uniquely identifies the shipper in carrier's dispatch system
        /// </summary>
        public string shipper { get; set; }

        /// <summary>
        /// Bill of Lading of the load provided by shipper
        /// </summary>
        public string billOfLading { get; set; }

        /// <summary>
        /// tractor identifier
        /// </summary>
        public string tractorNumber { get; set; }

        /// <summary>
        /// trailer identifier
        /// </summary>
        public string trailerNumber { get; set; }

        /// <summary>
        /// driver phone number
        /// </summary>
        public string driverPhone { get; set; }

        /// <summary>
        /// SCAC of the carrier which was tendered the load
        /// </summary>
        public string scac { get; set; }

        /// <summary>
        /// 4 character rail equipment initials
        /// </summary>
        public string railEquipmentInitials { get; set; }

        /// <summary>
        /// 6 digit rail equipment number
        /// </summary>
        public string railEquipmentNumber { get; set; }

        /// <summary>
        /// boolean indicator for brokered load or not
        /// </summary>
        public string isBrokeredLoad { get; set; }

        /// <summary>
        /// leg number
        /// </summary>
        public string leg { get; set; }

        /// <summary>
        /// SCAC of the FourKites-onboarded partner carrier hauling the load (please consult your account manager before using this parameter)
        /// </summary>
        public string operatingCarrierScac { get; set; }
    }
}
