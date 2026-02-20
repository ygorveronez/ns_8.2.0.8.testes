using Dominio.ObjetosDeValor.Embarcador.Integracao.Gadle;
using Dominio.ObjetosDeValor.WebService.Carga.BookingAVRO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataTaker
    {
        /// <summary>
        /// <para>sender</para>
        /// <para>shipper</para>
        /// <para>receiver</para>
        /// <para>recipient</para>
        /// <para>others</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTakerType type { get; set; }

        public cteDataTakerTaker taker { get; set; }
    }

    public class cteDataTakerTaker
    {
        /// <summary>
        /// Business name or name(tag xNome)
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Tag fone
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// Tag email
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// State Registration(tag IE)
        /// </summary>
        public string stateRegistration { get; set; }

        public cteDataTakerTakerAddress address { get; set; }

        /// <summary>
        /// <para>legal</para>
        /// <para>individual</para>
        /// <para>foreign</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumParticipantType type { get; set; }

        /// <summary>
        /// <para>Tag CNPJ</para>
        /// <para>Tag CPF</para>
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag xFant
        /// </summary>
        public string tradeName { get; set; }
    }

    public class cteDataTakerTakerAddress
    {
        /// <summary>
        /// Street address(tag xLgr)
        /// </summary>
        public string street { get; set; }

        /// <summary>
        /// Address number(tag nro)
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// Address complement(tag xCpl)
        /// </summary>
        public string complement { get; set; }

        /// <summary>
        /// Neighborhood of the address(tag xBairro)
        /// </summary>
        public string neighborhood { get; set; }

        public cteDataTakerTakerAddressCity city { get; set; }

        /// <summary>
        /// ZIP Code(tag CEP)
        /// </summary>
        public string zipCode { get; set; }

        public cteDataTakerTakerAddressCountry country { get; set; }
    }

    public class cteDataTakerTakerAddressCity
    {
        /// <summary>
        /// Municipality name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Municipality code
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// State (UF) of the municipality
        /// </summary>
        public string state { get; set; }
    }

    public class cteDataTakerTakerAddressCountry
    {
        /// <summary>
        /// Country name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Country code
        /// </summary>
        public string code { get; set; }
    }
}