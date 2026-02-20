using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Canhoto
{
    public class RetornoCanhotoEntrega
    {
        public string numeroCarga { get; set; }
        public string numeroOcorrencia { get; set; }
        public List<RetornoCanhotoEntregaStages> stage { get; set; }
    }
}
