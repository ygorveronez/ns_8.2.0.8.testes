using System;

namespace Dominio.Relatorios.Embarcador.DataSource.SAC
{
    public class Documento
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string TipoFrete { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataEmbarque { get; set; }
        public string MDFes { get; set; }
        public string Cargas { get; set; }
        public DateTime DataDigitacao { get; set; }
        public string Placas { get; set; }
        public string Frotas { get; set; }
        public DateTime DataPrevisaoEntrega { get; set; }
        public string TiposOperacoes { get; set; }

        public string Remetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string NumeroRemetente { get; set; }
        public string ComplementoRemetente { get; set; }
        public string BairroRemetente { get; set; }
        public string CidadeRemetente { get; set; }
        public string UFRemetente { get; set; }
        public string CNPJRemetente { get; set; }
        public string IERemetente { get; set; }

        public string Destinatario { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string NumeroDestinatario { get; set; }
        public string ComplementoDestinatario { get; set; }
        public string BairroDestinatario { get; set; }
        public string CidadeDestinatario { get; set; }
        public string UFDestinatario { get; set; }
        public string CNPJDestinatario { get; set; }
        public string IEDestinatario { get; set; }

        public decimal ValorFrete { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal ValorMercadoria { get; set; }

        public string Observacao { get; set; }

        public bool ContemComponentes { get; set; }
        public bool ContemDocumentos { get; set; }
        public bool ContemNotas { get; set; }
        public bool ContemFaturas { get; set; }
        public bool ContemOcorrencias { get; set; }

    }
}
