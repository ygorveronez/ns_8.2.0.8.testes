using System;

namespace Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito
{
    public class NotaDebitoV2
    {
        public bool ModeloDebito { get; set; }

        public DateTime Data { get; set; }

        public string Numero { get; set; }

        public string Remetente { get; set; }

        public string EnderecoRemetente { get; set; }

        public string Destinatario { get; set; }

        public string EnderecoDestinatario { get; set; }

        public string Fatura { get; set; }

        public string Encomenda { get; set; }

        public string Vendedor { get; set; }

        public string Termos { get; set; }

        public string AprovadoPor { get; set; }

        public string FaturaInterna { get; set; }

        public string Serie { get; set; }

        public string ObservacaoAprovador { get; set; }

        public bool PossuiProdutos { get; set; }
    }
}
