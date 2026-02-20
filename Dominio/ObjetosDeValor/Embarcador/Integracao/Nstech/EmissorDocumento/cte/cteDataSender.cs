using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataSender
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

        /// <summary>
        /// address
        /// </summary>
        public cteDataSenderAddress address { get; set; }

        /// <summary>
        /// <para>legal</para>
        /// <para>individual</para>
        /// <para>foreign</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumParticipantType type { get; set; }

        /// <summary>
        /// <para>legal - Tag CNPJ</para>
        /// <para>individual - Tag CPF</para>
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag xFant
        /// </summary>
        public string tradeName { get; set; }
    }

    public class cteDataSenderAddress
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

        /// <summary>
        /// city
        /// </summary>
        public cteDataSenderAddressCity city { get; set; }

        /// <summary>
        /// ZIP Code(tag CEP)
        /// </summary>
        public string zipCode { get; set; }

        /// <summary>
        /// country
        /// </summary>
        public cteDataSenderAddressCountry country { get; set; }
    }

    public class cteDataSenderAddressCity
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

    public class cteDataSenderAddressCountry
    {
        /// <summary>
        /// Country name(tag xPais)
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Country code(tag cPais)
        /// </summary>
        public string code { get; set; }
    }
}