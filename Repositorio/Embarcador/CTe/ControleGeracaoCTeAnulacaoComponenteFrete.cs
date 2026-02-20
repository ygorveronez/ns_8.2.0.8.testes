using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CTe
{
    public class ControleGeracaoCTeAnulacaoComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete>
    {
        public ControleGeracaoCTeAnulacaoComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete> BuscarPorControleGeracaoCTeAnulacao(int codigoControleGeracaoCTeAnulacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete>();

            query = query.Where(o => o.ControleGeracaoCTeAnulacao.Codigo == codigoControleGeracaoCTeAnulacao);

            return query.ToList();
        }
    }
}
