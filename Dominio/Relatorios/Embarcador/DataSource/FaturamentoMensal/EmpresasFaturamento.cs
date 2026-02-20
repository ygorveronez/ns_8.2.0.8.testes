namespace Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal
{
    public class EmpresasFaturamento
    {
        public int CodigoEmpresa { get; set; }
        public double CNPJCliente { get; set; }
        public int CodigoFaturamentoMensalCliente { get; set; }
        public string Empresa { get; set; }
        public string DiaFaturamento { get; set; }
        public string ProximoVencimento { get; set; }
        public string UltimoVencimento { get; set; }
        public string ValorFaturamento { get; set; }
        public string CadastroFaturamento { get; set; }
        public string PlanoMensal { get; set; }
        public string QtdDocumento { get; set; }
        public string QtdNFe { get; set; }
        public string QtdNFSe { get; set; }
        public string QtdBoleto { get; set; }
        public string QtdTitulo { get; set; }
    }
}
