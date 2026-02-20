using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteSimplifiedDataTotal
    {
        /// <summary>
        /// Total Service Provision Value(tag vTPrest)
        /// </summary>
        public decimal serviceAmount { get; set; }

        /// <summary>
        /// Amount to Receive(tag vRec)
        /// </summary>
        public decimal receivableServiceAmount { get; set; }
    }
}