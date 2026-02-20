using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest
{
    public class Veiculo
    {
        public string Placa { get; set; }
        public string Renavam { get; set; }
        public string UF { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo TipoVeiculo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado TipoRodado { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular ModeloVeicular { get; set; }

        public List<Veiculo> Reboques { get; set; }
    }
}
