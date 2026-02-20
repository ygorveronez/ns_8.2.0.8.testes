namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioMensalidade
    {
        public int CodigoEmpresa { get; set; }
        public string CNPJ { get; set; }
        public string Nome { get; set; }
        public int QuantidadeCancelados { get; set; }
        public int QuantidadeAutorizados { get; set; }
        public int QuantidadeMDFeCancelados { get; set; }
        public int QuantidadeMDFeAutorizados { get; set; }
        public string DescricaoPlano { get; set; }
        public decimal ValorMensalidade { get; set; }
        public decimal ValorDespesasAdicionais { get; set; }
        public decimal ValorTotal
        {
            get
            {
                return this.ValorMensalidade + this.ValorDespesasAdicionais;
            }
        }
    }
}
