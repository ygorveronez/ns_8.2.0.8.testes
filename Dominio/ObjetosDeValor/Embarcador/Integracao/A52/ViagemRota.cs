using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.A52
{
    public class ViagemRota
    {
        public string cd_tipo_rota { get; set; }
        public string cd_identificador { get; set; }
        public string polilinha { get; set; }
        public List<List<string>> pontos { get; set; }
    }
}
