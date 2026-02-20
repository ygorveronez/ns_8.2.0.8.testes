namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public sealed class NotaFiscalContabilizacao
    {
        public int CodigoEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int NumeroRecebimento { get; set; }
        public string CFOPEntrada { get; set; }
        public string DataContabilizacao { get; set; }
        public string DataRecebimentoFisico { get; set; }
        public string CodigoTransacaoRecebimento { get; set; }
        public string TransacaoRecebimento { get; set; }
        public bool ReversaoRecebimento { get; set; }
        public string ContaTransacao { get; set; }
        public string UC { get; set; }
        public string OrdemVenda { get; set; }
        public string DataOrdemVenda { get; set; }
        public string CodigoUnicoNF { get; set; }
        public string EstruturaVenda { get; set; }
        public string Especie { get; set; }
        public string ItemFrete { get; set; }
        public bool CalcularPisCofins { get; set; }
        public string ContaContabil { get; set; }
        public string Mercado { get; set; }
        public string Diretoria { get; set; }
        public string DescricaoUC { get; set; }
        public string Pedagio { get; set; }
        public string DescTransacao { get; set; }
    }
}
