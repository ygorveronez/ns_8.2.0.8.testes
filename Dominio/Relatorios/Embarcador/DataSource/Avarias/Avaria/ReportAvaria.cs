namespace Dominio.Relatorios.Embarcador.DataSource.Avarias.Avaria
{
    public class ReportAvaria
    {
        public int Codigo { get; set; }
        public int Avaria { get; set; }
        public int Lote { get; set; }
        public string CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public int QuantidadeCaixas { get; set; }
        public int QuantidadeUnidades { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorAvaria { get; set; }
        public decimal ValorDescontoAvaria { get; set; }
        public string SituacaoAvaria { get; set; }
        public string DataSolicitacao { get; set; }
        public string DataCriacao { get; set; }
        public string DataAprovacao { get; set; }
        public string DataIntegracao { get; set; }
        public string Responsavel { get; set; }
        public string EtapaLote { get; set; }
        public string EtapaAvaria { get; set; }
        public string Filial { get; set; }
        public string Transportadora { get; set; }
        public string Criador { get; set; }
        public string DataEntrega { get; set; }
        public string CTe { get; set; }
        public string NotasFiscais { get; set; }
        public string Viagem { get; set; }
        public string TipoOperacao { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string MotivoAvaria { get; set; }
        public string Motorista { get; set; }
        public string RGMotorista { get; set; }
        public string Placa { get; set; }
        public string DataAvaria { get; set; }
        public string Expedidor { get; set; }
        public string Recebedor { get; set; }
        public string DataViagem { get; set; }
        public string DataCarga { get; set; }
    }
}
