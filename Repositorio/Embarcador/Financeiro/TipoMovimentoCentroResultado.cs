using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TipoMovimentoCentroResultado : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado>
    {
        public TipoMovimentoCentroResultado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado> BuscarPorTipoMovimento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado>();
            var result = from obj in query where obj.TipoMovimento.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado BuscarCentroResultado(int codigoTipoMovimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado>();

            var result = from obj in query where obj.TipoMovimento.Codigo == codigoTipoMovimento select obj;

            return result.FirstOrDefault()?.CentroResultado ?? null;
        }
    }
}
