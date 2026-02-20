using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class EnvioTipoOcorrencia
    {
        public string externalId { get; set; }
        public List<string> sources { get; set; }
        public TextoInternacionalizado label { get; set; }
        public string? checklist { get; set; }
    }
}
