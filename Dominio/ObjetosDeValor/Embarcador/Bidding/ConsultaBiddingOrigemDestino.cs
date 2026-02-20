using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class ConsultaBiddingOrigemDestino
    {
        public List<Dominio.Entidades.Localidade> Localidades { get; set; }
        public List<Dominio.Entidades.Estado> Estados { get; set; }
        public List<Dominio.Entidades.Embarcador.Localidades.Regiao> Regioes { get; set; }
        public List<Dominio.Entidades.Cliente> Clientes { get; set; }
        public List<Dominio.Entidades.RotaFrete> RotasFrete { get; set; }

        public ConsultaBiddingOrigemDestino()
        {
            Localidades = new List<Dominio.Entidades.Localidade>();
            Estados = new List<Dominio.Entidades.Estado>();
            Regioes = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();
            Clientes = new List<Dominio.Entidades.Cliente>();
            RotasFrete = new List<Dominio.Entidades.RotaFrete>();
        }
    }
}
