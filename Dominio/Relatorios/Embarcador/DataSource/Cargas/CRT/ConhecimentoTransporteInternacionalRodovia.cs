namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT
{
    public class ConhecimentoTransporteInternacionalRodovia
    {
        public string NomeRemetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string NomeDestinatario { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string NomeConsignatario { get; set; }
        public string EnderecoConsignatario { get; set; }
        public string Produtos { get; set; }
        public string LocalidadeEmissao { get; set; }
        public string Numero { get; set; }
        public string NomeTransportador { get; set; }
        public string EnderecoTransportador { get; set; }
        public string LocalidadePosseTransportador { get; set; }
        public string DataPosseTransportador { get; set; }
        public string LocalidadeDestino { get; set; }
        public string PrazoEntrega { get; set; }
        public string PesoBruto { get; set; }
        public string PesoLiquido { get; set; }
        public string Volume { get; set; }
        public string Valor { get; set; }
        public string Moeda { get; set; }
        public string ValorFrete { get; set; }
        public string ValorFreteDestino { get; set; }
        public string MoedaDestino { get; set; }
        public string ValorFreteOrigem { get; set; }
        public string MoedaOrigem { get; set; }
        public string ValorSeguroDestino { get; set; }
        public string ValorSeguroOrigem { get; set; }
        public string ValorTotal { get; set; }
        public string ValorTotalOrigem { get; set; }
        public string ValorTotalDestino { get; set; }
        public string ValorMercadoria { get; set; }
        public string FormalidadesAlfandega { get; set; }
        public string DeclaracoesObservacoes { get; set; }
        public string Incoterm { get; set; }
        public string NotificacaoCRT { get; set; }
        public string EnderecoNotificacaoCRT { get; set; }
        public string ValorOutrosOrigem { get; set; }
        public string ValorOutrosDestino { get; set; }
        public string DataEmissao { get; set; }
        public string NomeCPFCNPJTransportador { get; set; }
        public string DocumentosAnexos { get; set; }
    }
}
