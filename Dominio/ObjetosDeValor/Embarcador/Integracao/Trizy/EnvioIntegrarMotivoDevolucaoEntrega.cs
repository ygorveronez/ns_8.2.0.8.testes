using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class EnvioIntegrarMotivoDevolucaoEntrega
    {
        public string externalId { get; set; }
        public string source { get; set; }
        public TextoInternacionalizado label { get; set; }
        public string checklist { get; set; }
    }
}
