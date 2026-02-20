using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataModalInformationAir
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enumModalMDFe modal { get; } = enumModalMDFe.air;
    }
}
