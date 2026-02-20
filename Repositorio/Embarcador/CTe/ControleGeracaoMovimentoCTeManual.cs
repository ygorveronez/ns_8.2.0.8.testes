using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CTe
{
    public class ControleGeracaoMovimentoCTeManual : RepositorioBase<Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual>
    {
        public ControleGeracaoMovimentoCTeManual(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual> BuscarParaGeracao(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual>();

            query = query.Where(o => o.CargaCTe.CTe.Status == o.SituacaoCTeGerar);

            return query.Skip(inicio).Take(limite).ToList();
        }

        public Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual BuscarPorCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual>();

            query = query.Where(o => o.CargaCTe.Codigo == codigoCargaCTe);

            return query.FirstOrDefault();
        }
    }
}
