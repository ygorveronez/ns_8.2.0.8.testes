using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MDFePassagens
    {
        public Dominio.Entidades.Localidade Origem { get; set; }
        public Dominio.Entidades.Localidade Destino { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao CargaLocaisPrestacao { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> Passagem { get; set; }
    }
}
