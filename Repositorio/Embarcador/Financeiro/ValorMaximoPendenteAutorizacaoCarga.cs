using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class ValorMaximoPendenteAutorizacaoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga>
    {
        public ValorMaximoPendenteAutorizacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga> BuscarNaoExtornadasPorCarga(int codigoCarga)
        {
            IQueryable< Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValorMaximoPendenteAutorizacaoCarga.AutorizacaoExtornada);

            return query.ToList();
        }
    }
}
