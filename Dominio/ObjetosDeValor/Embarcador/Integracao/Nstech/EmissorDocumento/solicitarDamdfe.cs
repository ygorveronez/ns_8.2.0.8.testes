using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class solicitarDamdfe
    {
        public string by { get; set; }
        public string externalId { get; set; }
        public mdfeOptionsDamdfeNotifications notifications { get; set; }
    }
}