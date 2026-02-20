using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ItemNaoConformidadeCFOP : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP>
    {
        #region Constructores
        public ItemNaoConformidadeCFOP(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP> BuscarPorItemNaoConformidade(int codigoItem)
        {
            var consultaItemNaoConformidadeTiposOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP>()
                .Where(x => x.ItemNaoConformidade.Codigo == codigoItem);

            return consultaItemNaoConformidadeTiposOperacao.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP> BuscarPorItensNaoConformidadeAtivos()
        {
            var consultaItemNaoConformidadeTiposOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP>()
                .Where(item => item.ItemNaoConformidade.IrrelevanteParaNC == false);

            return consultaItemNaoConformidadeTiposOperacao
                .Select(item => new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP()
                {
                    Codigo = item.Codigo,
                    CodigoItemNaoConformidade = item.ItemNaoConformidade.Codigo,
                    CFOP= item.CFOP.Codigo
                })
                .ToList();
        }

        #endregion

    }
}
