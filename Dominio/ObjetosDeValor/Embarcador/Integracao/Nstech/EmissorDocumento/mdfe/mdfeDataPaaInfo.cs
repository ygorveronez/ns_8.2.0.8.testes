using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataPaaInfo
    {
        /// <summary>
        /// Tag CNPJPAA
        /// </summary>
        public string document { get; set; }

        /// <summary>
        /// Tag PAASignature
        /// </summary>
        public mdfeDataPaaInfoPaaSignature paaSignature { get; set; }
    }

    public class mdfeDataPaaInfoPaaSignature
    {
        /// <summary>
        /// Tag SignatureValue
        /// </summary>
        public string signatureValue { get; set; }

        /// <summary>
        /// Tag RSAKeyValue
        /// </summary>
        public mdfeDataPaaInfoPaaSignatureRsaKeyValue rsaKeyValue { get; set; }
    }

    public class mdfeDataPaaInfoPaaSignatureRsaKeyValue
    {
        /// <summary>
        /// Tag Modulus
        /// </summary>
        public string modulus { get; set; }

        /// <summary>
        /// Tag Exponent
        /// </summary>
        public string exponent { get; set; }
    }
}