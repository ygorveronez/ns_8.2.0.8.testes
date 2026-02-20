using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ItemNaoConformidadeFilial : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFilial>
    {
        #region Constructores
        public ItemNaoConformidadeFilial(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFilial> BuscarPorItemNaoConformidade(int codigoItem)
        {
            var consultaItemNaoConformidadeTiposOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFilial>()
                .Where(x => x.ItemNaoConformidade.Codigo == codigoItem);

            return consultaItemNaoConformidadeTiposOperacao.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeFilial> BuscarPorItensNaoConformidadeAtivos()
        {
            var consultaItemNaoConformidadeTiposOperacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFilial>()
                .Where(item => item.ItemNaoConformidade.IrrelevanteParaNC == false);

            return consultaItemNaoConformidadeTiposOperacao
                .Select(item => new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidadeFilial()
                {
                    Codigo = item.Codigo,
                    CodigoItemNaoConformidade = item.ItemNaoConformidade.Codigo,
                    Filial = item.Filial.Codigo
                })
                .ToList();
        }

        #endregion

    }
}
