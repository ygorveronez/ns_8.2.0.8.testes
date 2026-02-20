namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class DACCe
    {
        public string NomeEmitente { get; set; }
        public string EnderecoEmitente { get; set; }
        public string ChaveNFe { get; set; }
        public string IEEmitente { get; set; }
        public string CPFCNPJEmitente { get; set; }
        public string Modelo { get; set; }
        public string Serie { get; set; }
        public string NumeroNFe { get; set; }
        public string MesEmissao { get; set; }
        public string Folha { get; set; }

        public string NomeRemetente { get; set; }
        public string CPFCNPJRementente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string BairroRemetente { get; set; }
        public string CEPRemetente { get; set; }
        public string MunicipioRemetente { get; set; }
        public string UFRemetente { get; set; }
        public string FoneRemetente { get; set; }
        public string IERemetente { get; set; }

        public string Seq { get; set; }
        public string Status { get; set; }
        public string DataRegistro { get; set; }
        public string NumeroProtocolo { get; set; }
        public string Correcao { get; set; }
    }
}
