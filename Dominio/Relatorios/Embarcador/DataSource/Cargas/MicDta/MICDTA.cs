namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.MicDta
{
    public class MICDTA
    {
        public string Remetente { get; set; }
        public string EnderecoRemetente { get; set; }

        public string Destinatario { get; set; }
        public string EnderecoDestinatario { get; set; }

        public string Tomador { get; set; }
        public string EnderecoTomador { get; set; }

        public string CnpjTransportador { get; set; }
        public string Transportador { get; set; }
        public string EnderecoTransportador { get; set; }

        public string Veiculo { get; set; }
        public string CNPJProprietarioVeiculo { get; set; }
        public string DadosProprietarioVeiculo { get; set; }
        public string MarcaVeiculo { get; set; }
        public string ChassiVeiculo { get; set; }
        public string AnoVeiculo { get; set; }
        public string CapacidadeTracaoVeiculo { get; set; }
        public string Reboque { get; set; }
        public string ReboqueCheck { get; set; }
        public string SemirreboqueCheck { get; set; }
        public string Numero { get; set; }
        public string NumeroConhecimento { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string SiglaEstrangeira { get; set; }
        public string AlfandegaDestino { get; set; }
        public string CodigoAlfandegaDestino { get; set; }
        public string CodigoAlfandega { get; set; }
        public string PaisOrigemMercadorias { get; set; }
        public string ValorFOT { get; set; }
        public string ValorFrete { get; set; }
        public string ValorSeguro { get; set; }
        public string TipoVolumes { get; set; }
        public string TipoVolumesCampo1 { get; set; }
        public string TipoVolumesCampo2 { get; set; }
        public string QuantidadeVolumes { get; set; }
        public string PesoBruto { get; set; }
        public string NumeroLacres { get; set; }
        public string DescricaoMercadorias { get; set; }
        public string DocumentosAnexos { get; set; }
        public string DataEmissao { get; set; }
        public string TransitoAduaneiroSim { get; set; }
        public string TransitoAduaneiroNao { get; set; }
        public string NomeCPFCNPJTransportador { get; set; }
        public string DtaRotaPrazoDeTransporte { get; set; }
    }
}
