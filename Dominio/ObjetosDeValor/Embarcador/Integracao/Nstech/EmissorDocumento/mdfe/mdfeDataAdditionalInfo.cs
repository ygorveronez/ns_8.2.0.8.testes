using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataAdditionalInfo
    {
        /// <summary>
        /// Tag infAdFisco
        /// </summary>
        public string fiscalInfo { get; set; }

        /// <summary>
        /// Tag infCpl
        /// </summary>
        public string cplInfo { get; set; }
    }
}