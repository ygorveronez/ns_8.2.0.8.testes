using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataIssuer
    {
        /// <summary>
        /// <para>legal</para>
        /// <para>individual</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssuerType type { get; set; }

        /// <summary>
        /// Tag CNPJ
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag IE
        /// </summary>
        public string stateRegistration { get; set; }

        /// <summary>
        /// Tag xNome
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Tag xFant
        /// </summary>
        public string tradeName { get; set; }

        /// <summary>
        /// Tag enderEmit
        /// </summary>
        public mdfeDataIssuerAddress address { get; set; }
    }

    public class mdfeDataIssuerAddress
    {
        /// <summary>
        /// Street
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
        /// City
        /// </summary>
        public mdfeDataIssuerAddressCity city { get; set; }

        /// <summary>
        /// ZIP Code(tag CEP)
        /// </summary>
        public string zipCode { get; set; }

        /// <summary>
        /// Tag fone
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// Tag email
        /// </summary>
        public string email { get; set; }
    }

    public class mdfeDataIssuerAddressCity
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
}