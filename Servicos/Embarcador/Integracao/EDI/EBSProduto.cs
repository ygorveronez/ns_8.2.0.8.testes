using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class EBSProduto
    {
        public Dominio.ObjetosDeValor.EDI.EBS.ListaProduto ConverterProdutosEBS(List<Dominio.Entidades.Produto> produtos, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Dominio.ObjetosDeValor.EDI.EBS.ListaProduto produtosEBS = new Dominio.ObjetosDeValor.EDI.EBS.ListaProduto();
            List<Dominio.ObjetosDeValor.EDI.EBS.Produto> listaProduto = new List<Dominio.ObjetosDeValor.EDI.EBS.Produto>();

            for (int i = 0; i < produtos.Count; i++)
            {
                Dominio.Entidades.Produto produto = produtos[i];
                if (produto != null)
                {
                    Dominio.ObjetosDeValor.EDI.EBS.Produto prod = new Dominio.ObjetosDeValor.EDI.EBS.Produto();

                    prod.Codigo = produto.Codigo.ToString("0000000000");
                    prod.Descricao = produto.Descricao;
                    prod.NCM = !string.IsNullOrWhiteSpace(produto.CodigoNCM) ? produto.CodigoNCM : string.Empty;
                    prod.Unidade1 = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla(produto.UnidadeDeMedida);
                    prod.Peso = produto.PesoBruto;
                    prod.Identificacao = !string.IsNullOrEmpty(produto.CodigoProduto) ? produto.CodigoProduto : string.Empty;
                    prod.TipoProduto = produto.CodigoCategoriaProduto;
                    prod.CSTEntradaICMS = "";
                    prod.CSTEntradaIPI = produto.DescricaoCSTIPICompra;
                    prod.CSTSaidaICMS = "";
                    prod.CSTSaidaIPI = produto.DescricaoCSTIPIVenda;
                    prod.Unidade2 = "";
                    prod.AliquotaICMS = "00,00";
                    prod.AliquotaIPI = string.Format("{0:00.00}", produto.AliquotaIPIVenda);
                    if (!string.IsNullOrWhiteSpace(prod.CSTEntradaICMS) && prod.CSTEntradaICMS.Length == 3)
                        prod.CSOSN = "S";//N
                    else
                        prod.CSOSN = "N";
                    prod.Identificacao2 = "";
                    prod.GTIN = "";
                    prod.CSTEntradaPIS = "";
                    prod.CSTEntradaCOFINS = "";
                    prod.CSTSaidaPIS = "";
                    prod.CSTSaidaCOFINS = "";
                    prod.CodigoPISCOFINS = "";
                    prod.AliquotaPIS = 0;
                    prod.AliquotaCOFINS = 0;
                    prod.CodigoANP = "";
                    prod.Descricao2 = "";
                    prod.CodigoCEST = !string.IsNullOrWhiteSpace(produto.CodigoCEST) ? produto.CodigoCEST : string.Empty;
                    prod.Brancos = "";
                    prod.TipoRegistro = "2";
                    prod.Sequencia = (i + 1).ToString("000000");

                    listaProduto.Add(prod);
                }
            }
            produtosEBS.Produtos = listaProduto;
            return produtosEBS;
        }
    }
}
