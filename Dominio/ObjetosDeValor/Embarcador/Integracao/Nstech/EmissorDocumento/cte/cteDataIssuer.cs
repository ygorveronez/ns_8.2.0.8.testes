using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteDataIssuer
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
        /// State Registration(tag IE)
        /// </summary>
        public string stateRegistration { get; set; }

        /// <summary>
        /// <para>legal</para>
        /// <para>individual</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumIssuerType type { get; set; }

        /// <summary>
        /// <para>legal - Tag CNPJ</para>
        /// <para>individual - Tag CPF</para>
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag xFant
        /// </summary>
        public string tradeName { get; set; }

        /// <summary>
        /// <para>Tax Regime Code(tag CRT)</para>
        /// <para>simples_nacional</para>
        /// <para>lucro_presumido</para>
        /// <para>lucro_real</para>
        /// <para>micro_empreendedor_individual</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTaxRegime taxRegime { get; set; }

        /// <summary>
        /// State Registration of the Tax Substitute(tag IEST)
        /// </summary>
        public string stateRegistrationST { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public cteDataIssuerAddress address { get; set; }
    }

    public class cteDataIssuerAddress
    {
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
        public cteDataIssuerAddressCity city { get; set; }

        /// <summary>
        /// ZIP Code(tag CEP)
        /// </summary>
        public string zipCode { get; set; }

        /// <summary>
        /// Street
        /// </summary>
        public string street { get; set; }
    }

    public class cteDataIssuerAddressCity
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