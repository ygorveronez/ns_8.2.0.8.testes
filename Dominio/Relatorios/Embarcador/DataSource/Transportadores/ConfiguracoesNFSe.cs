namespace Dominio.Relatorios.Embarcador.DataSource.Transportadores
{
    public class ConfiguracoesNFSe
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string LocalidadePrestacaoServico { get; set; }
        public string UFLocalidadePrestacaoServico { get; set; }
        public int Serie { get; set; }
        public string SerieRPS { get; set; }
        public string Servico { get; set; }
        public string CodigoServico { get; set; }
        public string NumeroServico { get; set; }
        public string Natureza { get; set; }
        public decimal Aliquota { get; set; }
        public decimal Retencao { get; set; }
        public string TipoOperacao { get; set; }
        private bool IncluirValorISSBaseCalculo { get; set; }

        #endregion

        #region Propriedades com regras

        public string IncluirValorISSBaseCalculoFormatado
        {
            get
            {
                return System.BoolExtensions.ObterDescricao(this.IncluirValorISSBaseCalculo);
            }
        }

        #endregion
    }


}
