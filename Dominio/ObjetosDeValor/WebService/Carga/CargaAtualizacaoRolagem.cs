using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class CargaAtualizacaoRolagem
    {
        public int ProtocoloCarga { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroBooking { get; set; }
        public Embarcador.Carga.Viagem Viagem { get; set; }
        public Embarcador.Carga.Porto PortoOrigem { get; set; }
        public Embarcador.Carga.Porto PortoDestino { get; set; }
        public Embarcador.Carga.TerminalPorto TerminalPortoOrigem { get; set; }
        public Embarcador.Carga.TerminalPorto TerminalPortoDestino { get; set; }
        public List<Embarcador.Carga.Transbordo> Transbordo { get; set; }
        public Embarcador.Carga.Container Container { get; set; }
        public string Lacre1 { get; set; }
        public string Lacre2 { get; set; }
        public string Lacre3 { get; set; }
    }
}
