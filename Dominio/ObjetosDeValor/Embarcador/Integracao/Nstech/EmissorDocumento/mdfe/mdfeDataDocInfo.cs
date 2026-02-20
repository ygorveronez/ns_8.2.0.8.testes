using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataDocInfo
    {
        /// <summary>
        /// Tag infMunDescarga
        /// </summary>
        public List<mdfeDataDocInfoUnloadCityInfo> unloadCityInfo { get; set; }
    }
}
