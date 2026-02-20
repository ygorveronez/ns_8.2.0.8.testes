using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta
{
    public class DespesaViagemIntegrar
    {
        public string CNPJAplicacao { get; set; }
        public string Token { get; set; }
        public string CNPJEmpresa { get; set; }
        public string NroControleIntegracao { get; set; }
        public string CPFUsuario { get; set; }
        public List<CargaAvulsa> CargasAvulsas { get; set; }
    }
}
