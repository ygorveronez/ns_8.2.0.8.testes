using Repositorio;
using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public class ProdutoPerigoso : ServicoBase
    {

        public ProdutoPerigoso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso> ConverterProdutoPerigosoCTeParaProdutoPerigoso(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ProdutoPerigosoCTE repProdutoPerigosoCTe = new Repositorio.ProdutoPerigosoCTE(unitOfWork);
            List<Dominio.Entidades.ProdutoPerigosoCTE> produtosPerigososCTe = repProdutoPerigosoCTe.BuscarPorCTe(cte.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso> produtosPerigosos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso>();

            foreach (Dominio.Entidades.ProdutoPerigosoCTE produtoPerigosoCTe in produtosPerigososCTe)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso produtoPerigoso = new Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso();
                produtoPerigoso.ClasseRisco = produtoPerigosoCTe.ClasseRisco;
                produtoPerigoso.NumeroONU = produtoPerigosoCTe.NumeroONU;
                produtoPerigoso.NomeApropriado = produtoPerigosoCTe.NomeApropriado;
                produtoPerigoso.Grupo = produtoPerigosoCTe.Grupo;
                produtoPerigoso.Quantidade = produtoPerigosoCTe.Quantidade;
                produtoPerigoso.PontoFulgor = produtoPerigosoCTe.PontoFulgor;
                produtoPerigoso.Volumes = produtoPerigosoCTe.Volumes;

                produtosPerigosos.Add(produtoPerigoso);
            }

            return produtosPerigosos;
        }


        public List<Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso> ConverterDynamicParaProdutoPerigoso(dynamic dynProdutosPerigosos, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso> produtosPerigosos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso>();

            foreach (var dynProdutoPerigoso in dynProdutosPerigosos)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso produtoPerigoso = new Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso();
                produtoPerigoso.ClasseRisco = (string)dynProdutoPerigoso.ClasseRisco;
                int numeroONU;
                int.TryParse(dynProdutoPerigoso.NumeroONU.ToString(), out numeroONU);
                produtoPerigoso.NumeroONU = numeroONU;
                produtoPerigoso.NomeApropriado = (string)dynProdutoPerigoso.Nome;
                produtoPerigoso.Grupo = (string)dynProdutoPerigoso.GrupoEmbalagem;
                produtoPerigoso.Quantidade = (string)dynProdutoPerigoso.Quantidade;
                produtoPerigoso.PontoFulgor = (string)dynProdutoPerigoso.PontoFulgor;
                produtoPerigoso.Volumes = (string)dynProdutoPerigoso.QuantidadeTipoVolume;

                produtoPerigoso.QuantidadeTotal = Utilidades.Decimal.Converter((string)dynProdutoPerigoso.QuantidadeTotal);
                if (dynProdutoPerigoso.UnidadeDeMedida != null && !string.IsNullOrWhiteSpace((string)dynProdutoPerigoso.UnidadeDeMedida) && (int)dynProdutoPerigoso.UnidadeDeMedida > 0)
                    produtoPerigoso.UnidadeDeMedida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaCTeAereo)dynProdutoPerigoso.UnidadeDeMedida;
                else
                    produtoPerigoso.UnidadeDeMedida = null;

                produtosPerigosos.Add(produtoPerigoso);
            }

            return produtosPerigosos;
        }

        public void SalvarProdutosPerigosos(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso> produtosPerigosos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ProdutoPerigosoCTE repProduto = new Repositorio.ProdutoPerigosoCTE(unitOfWork);
            if (cte.Codigo > 0)
                repProduto.DeletarPorCTe(cte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso prod in produtosPerigosos)
            {
                Dominio.Entidades.ProdutoPerigosoCTE produto = new Dominio.Entidades.ProdutoPerigosoCTE();
                produto.ClasseRisco = prod.ClasseRisco;
                produto.CTE = cte;
                produto.Grupo = prod.Grupo;
                produto.NomeApropriado = prod.NomeApropriado;
                produto.NumeroONU = prod.NumeroONU;
                produto.PontoFulgor = prod.PontoFulgor;
                produto.Quantidade = prod.Quantidade;
                produto.Volumes = prod.Volumes;
                produto.QuantidadeTotal = prod.QuantidadeTotal;
                produto.UnidadeDeMedida = prod.UnidadeDeMedida;

                repProduto.Inserir(produto);
            }
        }


        public void SalvarProdutosPerigososPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, List<Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso> produtosPerigosos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ProdutoPerigosoPreCTE repProduto = new Repositorio.ProdutoPerigosoPreCTE(unitOfWork);
            if (preCte.Codigo > 0)
                repProduto.DeletarPorPreCTe(preCte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.ProdutoPerigoso prod in produtosPerigosos)
            {
                Dominio.Entidades.ProdutoPerigosoPreCTE produto = new Dominio.Entidades.ProdutoPerigosoPreCTE();
                produto.ClasseRisco = prod.ClasseRisco;
                produto.PreCTE = preCte;
                produto.Grupo = prod.Grupo;
                produto.NomeApropriado = prod.NomeApropriado;
                produto.NumeroONU = prod.NumeroONU;
                produto.PontoFulgor = prod.PontoFulgor;
                produto.Quantidade = prod.Quantidade;
                produto.Volumes = prod.Volumes;
                repProduto.Inserir(produto);
            }
        }

    }
}
