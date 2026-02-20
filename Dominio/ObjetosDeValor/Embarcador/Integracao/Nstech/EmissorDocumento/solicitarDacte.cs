using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class solicitarDacte
    {
        public string by { get; set; }
        public string externalId { get; set; }
        public cteOptionsDacteNotifications notifications { get; set; }
    }
}