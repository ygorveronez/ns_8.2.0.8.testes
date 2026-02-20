namespace Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto
{
    public class ComprovanteEntrega
    {
        #region Propriedades

        #region Cabecalho

        public string RazaoSocialRemetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public byte[] CodigoDeBarras { get; set; }

        #endregion

        #region Dados Transportadora

        public string RazaoSocialTransportador { get; set; }
        public string FretePorConta { get; set; }
        public string CNPJTransportador { get; set; }
        public string EnderecoTransportador { get; set; }
        public string UFTransportador { get; set; }
        public string CidadeTransportador { get; set; }
        public string PlacaVeiculo { get; set; }
        public string ANTT { get; set; }

        #endregion

        #region Dados da NF-e
        public string RazaoSocialDestinatario { get; set; }
        public string IEDestinatario { get; set; }
        public string CNPJDestinatario { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string UFDestinatario { get; set; }
        public string CidadeDestinatario { get; set; }
        public string Centro { get; set; }
        public string ValorTotalNF { get; set; }
        public string ValorParaSeguro { get; set; }
        public string ValorDesconto { get; set; }
        public string ValorCobranca { get; set; }
        public string ValorOutros { get; set; }
        public string DocTransportes { get; set; }
        public string Fatura { get; set; }
        public string Remessa { get; set; }
        public string DocExternoTMS { get; set; }
        public string Quantidade { get; set; }
        public string EspecieCX { get; set; }
        public string PesoBruto { get; set; }
        public string DataEmissao { get; set; }
        public string NumeroNF { get; set; }
        public string ChaveAcessoNF { get; set; }
        public string SerieNF { get; set; }
        #endregion

        #endregion
    }
}