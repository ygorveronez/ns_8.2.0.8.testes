namespace Dominio.ObjetosDeValor.Embarcador.Filial
{
    public sealed class GestaoArmazem
    {
        public int Codigo { get; set; }
        public string CodigoFilial { get; set; }
        public string DescricaoFilial { get; set; }
        public string CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public string CodigoArmazem { get; set; }
        public decimal EstoqueDisponivel { get; set; }
        public decimal EstoqueSessaoRoterizacao { get; set; }

        #region Propriedades com Regras

        public decimal EstoqueFinalPrevisto { get { return EstoqueDisponivel - EstoqueSessaoRoterizacao > 0 ? EstoqueDisponivel - EstoqueSessaoRoterizacao : 0 ; } }

        #endregion
    }
}
