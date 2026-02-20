using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FreteNegociado
    {
        public string CPFOperador { get; set; }
        public string NomeOperador { get; set; }
        public List<FreteNegociadoAprovador> Aprovadores { get; set; }
    }
}
