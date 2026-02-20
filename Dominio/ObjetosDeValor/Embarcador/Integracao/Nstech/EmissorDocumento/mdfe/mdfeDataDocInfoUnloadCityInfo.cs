using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataDocInfoUnloadCityInfo
    {
        /// <summary>
        /// Tag cMunDescarga
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Tag xMunDescarga
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Tag infCte
        /// </summary>
        public List<mdfeDataDocInfoUnloadCityInfoInfCte> infCte { get; set; }

        /// <summary>
        /// Tag infNFe
        /// </summary>
        public List<mdfeDataDocInfoUnloadCityInfoInfNfe> infNfe { get; set; }

        /// <summary>
        /// Tag infMDFeTransp
        /// </summary>
        public List<mdfeDataDocInfoUnloadCityInfoInfMdfe> infMdfe { get; set; }
    }
}
