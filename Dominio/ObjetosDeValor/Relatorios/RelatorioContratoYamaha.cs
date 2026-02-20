namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioContratoYamaha
    {
        public int Codigo { get; set; }
        public int NumeroCte { get; set; }
        public int SerieCTe { get; set; }
        public string ChaveCTe { get; set; }
        public string DataEmissaoCTe { get; set; }
        public string CNPJTransportadora { get; set; }
        public string NomeTransportadora { get; set; }
        public string CRTTransportadora { get; set; }
        public string TipoCTe { get; set; }
        public string CFOP { get; set; }
        public string CNPJRemetente { get; set; }
        public string NomeRemetente { get; set; }
        public string MunicipioRemetente { get; set; }
        public string CNPJDestinatario { get; set; }
        public string NomeDestinatario { get; set; }
        public string MunicipioDestinatario { get; set; }
        public string CNPJExpedidor { get; set; }
        public string NomeExpedidor { get; set; }
        public string MunicipioExpedidor { get; set; }
        public string CNPJRecebedor { get; set; }
        public string NomeRecebedor { get; set; }
        public string MunicipioRecebedor { get; set; }
        public string CNPJTomador { get; set; }
        public string NomeTomador { get; set; }
        public string MunicipioTomador { get; set; }
        public string MunicipioOrigem { get; set; }
        public string MunicipioDestino { get; set; }
        public string CST { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal BaseICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public int NumeroCTeAnterior { get; set; }
        public int SerieCTeAnterior { get; set; }
        public string ChaveCTeAnterior { get; set; }
        public string CodigoTransporte { get; set; }
        public string DataEmissaoContrato { get; set; }
        public string PercursoContrato { get; set; }
        public decimal ValorFreteContrato { get; set; }
    }
}
