using System;

namespace Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito
{
    public class NotaDebito
    {
        public int Numero { get; set; }
        public DateTime DataEmissao { get; set; }

        public string NomeTomador { get; set; }
        public string EnderecoTomador { get; set; }
        public string BairroTomador { get; set; }
        public string CEPTomador { get; set; }
        public string CidadeTomador { get; set; }
        public string UFTomador { get; set; }
        public string CNPJTomador { get; set; }
        public string IETomador { get; set; }

        public string NomeTransportador { get; set; }
        public string EnderecoTransportador { get; set; }
        public string BairroTransportador { get; set; }
        public string CEPTransportador { get; set; }
        public string CidadeTransportador { get; set; }
        public string UFTransportador { get; set; }
        public string CNPJTransportador { get; set; }
        public string IETransportador { get; set; }
        public string IMTransportador { get; set; }

        public string Contato { get; set; }
        public string Telefone { get; set; }
        public string NotasFiscaisReferencia { get; set; }
        public string Referencia { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorICMS { get; set; }
        public string Observacao { get; set; }
        public string Banco { get; set; }
        public string NumeroBanco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string Placas { get; set; }
        public string NomeFavorecido { get; set; }
        public string CNPJFavorecido { get; set; }
    }
}
