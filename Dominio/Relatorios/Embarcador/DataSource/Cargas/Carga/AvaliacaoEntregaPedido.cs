using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public sealed class AvaliacaoEntregaPedido
    {
        public int Codigo { get; set; }

        public string NumeroCarga { get; set; }

        public string NumeroPedido { get; set; }

        public DateTime DataAvaliacao { get; set; }

        public string TransportadorFormatado
        {
            get
            {
                return $"{TransportadorCNPJ.ObterCnpjFormatado()} - {TransportadorRazao}";
            }
        }

        public string TransportadorCNPJ { get; set; }

        public string TransportadorRazao { get; set; }

        public string Placas { get; set; }

        public string NotasFiscais { get; set; }

        public string Feedback { get; set; }

        public string Motivo { get; set; }

        public string DestinatarioFormatado
        {
            get
            {
                return $"{DestinatarioCNPJ.ToString().ObterCpfOuCnpjFormatado(DestinatarioTipo)} - {DestinatarioNome}";
            }
        }

        public double DestinatarioCNPJ { get; set; }

        public string DestinatarioTipo { get; set; }

        public string DestinatarioNome { get; set; }
    }
}
