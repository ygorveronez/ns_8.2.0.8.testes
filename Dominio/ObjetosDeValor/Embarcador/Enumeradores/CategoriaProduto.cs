namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CategoriaProduto
    {
        Todos = -1,
        MercadoriaRevenda = 0,
        MateriaPrima = 1,
        Embalagem = 2,
        ProdutoEmProcesso = 3,
        ProdutoAcabado = 4,
        Subproduto = 5,
        ProdutoIntermediario = 6,
        MaterialUsoConsumo = 7,
        AtivoImobilizado = 8,
        Servicos = 9,
        OutrosInsumos = 10,
        Outras = 99
    }

    public static class CategoriaProdutoHelper
    {
        public static string ObterDescricao(this CategoriaProduto categoria)
        {
            switch (categoria)
            {
                case CategoriaProduto.AtivoImobilizado: return "Ativo Imobilizado";
                case CategoriaProduto.Embalagem: return "Embalagem";
                case CategoriaProduto.MaterialUsoConsumo: return "Material de Uso e Consumo";
                case CategoriaProduto.MateriaPrima: return "Matéria Prima";
                case CategoriaProduto.MercadoriaRevenda: return "Mercadoria para Revenda";
                case CategoriaProduto.Outras: return "Outros";
                case CategoriaProduto.OutrosInsumos: return "Outros Insumos";
                case CategoriaProduto.ProdutoAcabado: return "Produto Acabado";
                case CategoriaProduto.ProdutoEmProcesso: return "Produto em Processo";
                case CategoriaProduto.ProdutoIntermediario: return "Produto Intermediário";
                case CategoriaProduto.Servicos: return "Serviços";
                case CategoriaProduto.Subproduto: return "Subproduto";
                default: return string.Empty;
            }
        }

        public static string ObterSigla(this CategoriaProduto categoria)
        {
            switch (categoria)
            {
                case CategoriaProduto.AtivoImobilizado: return "08";
                case CategoriaProduto.Embalagem: return "99";
                case CategoriaProduto.MaterialUsoConsumo: return "07";
                case CategoriaProduto.MateriaPrima: return "01";
                case CategoriaProduto.MercadoriaRevenda: return "00";
                case CategoriaProduto.Outras: return "99";
                case CategoriaProduto.OutrosInsumos: return "10";
                case CategoriaProduto.ProdutoAcabado: return "04";
                case CategoriaProduto.ProdutoEmProcesso: return "03";
                case CategoriaProduto.ProdutoIntermediario: return "06";
                case CategoriaProduto.Servicos: return "09";
                case CategoriaProduto.Subproduto: return "05";
                default: return string.Empty;
            }
        }
    }
}
