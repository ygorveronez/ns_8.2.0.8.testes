using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ItemNaoConformidadeFornecedor : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor>
    {
        #region Constructores
        public ItemNaoConformidadeFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor> BuscarPorItemNaoConformidade(int codigoItem)
        {
            var consultaItemNaoConformidadeTiposOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor>()
                .Where(x => x.ItemNaoConformidade.Codigo == codigoItem);

            return consultaItemNaoConformidadeTiposOperacao.ToList();
        }
        public List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor> BuscarPorItensNaoConformidadeAtivos()
        {
            var consultaItemNaoConformidadeTiposOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor>()
                .Where(item => item.ItemNaoConformidade.IrrelevanteParaNC == false);

            return consultaItemNaoConformidadeTiposOperacao
                .Select(item => new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor()
                {
                    Codigo = item.Codigo,
                    CodigoItemNaoConformidade = item.ItemNaoConformidade.Codigo,
                    Fornecedor = item.Fornecedor.CPF_CNPJ
                })
                .ToList();
        }
        #endregion

    }
}
