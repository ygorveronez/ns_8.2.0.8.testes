using System;

namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPatio
{
    public class ComprovanteSaida
    {
        public DateTime DataAgenda { get; set; }
        
        public string Senha { get; set; }

        public string Fornecedor { get; set; }

        public string CodigoIntegracaoDestinatario { get; set; }

        public string NomeMotorista { get; set; }

        public string PlacaCavalo { get; set; }

        public string PlacaCarreta { get; set; }

        public string Telefone { get; set; }

        public DateTime DataChegada { get; set; }

		public string NumeroCarga { get; set; }
		public string DataSaida { get; set; }

        public string CPF { get; set; }

        public string RG { get; set; }

		public string Doca { get; set; }

		public string PrevisaoEntrega { get; set; }

		public string Transportador { get; set; }

		public string NomeCliente { get; set; }

		public byte[] QRCode { get; set; }

        public string NotasFiscais { get; set; }


    }
}
