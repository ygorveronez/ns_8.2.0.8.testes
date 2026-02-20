namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ProdutoServico
    {
        Nenhum = 0,
        Produto = 1,
        Servico = 2
    }

    public static class ProdutoServicoHelper
    {
        public static string ObterDescricao(this ProdutoServico tipoCobranca)
        {
            switch (tipoCobranca)
            {
                case ProdutoServico.Nenhum: return "Não informado";
                case ProdutoServico.Produto: return "Produto";
                case ProdutoServico.Servico: return "Serviço";
                default: return "Produto";
            }
        }
    }
}
