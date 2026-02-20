namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class ConfiguracaoSubcontratacaoTabelaFreteTerceiro
    {
        public int Codigo { get; set; }
        public int CodigoTabelaFrete { get; set; }
        public double CPFCNPJTerceiro { get; set; }
        public string CPFCNPJTerceiroFormatado
        {
            get
            {
                if (TipoTerceiro == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoTerceiro == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJTerceiro) : string.Format(@"{0:000\.000\.000\-00}", CPFCNPJTerceiro);
            }
        }
        public string NomeTerceiro { get; set; }
        public string TipoTerceiro { get; set; }
        public decimal PercentualCobranca { get; set; }
        public decimal PercentualDesconto { get; set; }
    }
}
