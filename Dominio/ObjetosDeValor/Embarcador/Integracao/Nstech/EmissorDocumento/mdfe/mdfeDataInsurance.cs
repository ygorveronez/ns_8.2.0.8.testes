using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataInsurance
    {
        /// <summary>
        /// Tag infResp
        /// </summary>
        public mdfeDataInsuranceInfResp infResp { get; set; }

        /// <summary>
        /// Tag infSeg
        /// </summary>
        public mdfeDataInsuranceInfSeg infSeg { get; set; }

        /// <summary>
        /// Tag nApol
        /// </summary>
        public string policyNumber { get; set; }

        /// <summary>
        /// Tag nAver
        /// </summary>
        public List<string> endorsementNumber { get; set; }
    }

    public class mdfeDataInsuranceInfResp
    {
        /// <summary>
        /// Tag respSeg
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumRespSeg respSeg { get; set; }

        /// <summary>
        /// Tag infSeg
        /// </summary>
        public mdfeDataInsuranceInfRespRespSegPersonDocument respSegPersonDocument { get; set; }
    }

    public class mdfeDataInsuranceInfRespRespSegPersonDocument
    {
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
    }

    public class mdfeDataInsuranceInfSeg
    {
        /// <summary>
        /// Tag CNPJ
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag xSeg
        /// </summary>
        public string name { get; set; }
    }
}