namespace Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.CRT
{
    public class CRT
    {
        public string Remetente { get; set; }

        public string EnderecoRemetente { get; set; }

        public string Destinatario { get; set; }

        public string EnderecoDestinatario { get; set; }

        public string Tomador { get; set; }

        public string EnderecoTomador { get; set; }

        public string Recebedor { get; set; }

        public string EnderecoRecebedor { get; set; }

        public string Emitente { get; set; }

        public string EmitenteCNPJ { get; set; }

        public string EnderecoEmitente { get; set; }

        public string Numero { get; set; }

        public string SiglaEstrangeira { get; set; }

        public string Origem { get; set; }

        public string OrigemDataEmissao { get; set; }

        public string DestinoDataEntrega { get; set; }

        public string Observacao { get; set; }

        public decimal PesoBruto { get; set; }

        public decimal PesoLiquido { get; set; }

        public decimal Volumes { get; set; }

        public decimal ValorAReceber { get; set; }

        public decimal ValorFrete { get; set; }

        public string ValorMercadoriasPorExtenso { get; set; }

        public string DocumentosCTe { get; set; }

        public string DadosComponentes { get; set; }

        public decimal ComponenteFrete { get; set; }

        public decimal ComponenteSeguro { get; set; }

        public decimal ComponenteOutros { get; set; }

        public decimal TotalComponentes { get; set; }

        public bool FreteAoRemetente { get; set; }

        public string Aduanas { get; set; }

        public string DataEmissaoDocumento { get; set; }

        public string ObservacaoCTe { get; set; }
    }
}
