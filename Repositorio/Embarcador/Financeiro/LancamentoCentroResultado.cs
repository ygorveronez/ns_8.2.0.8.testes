using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class LancamentoCentroResultado : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado>
    {
        public LancamentoCentroResultado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado>();

            query = query.Where(o => o.DocumentoEntrada.Codigo == codigoDocumentoEntrada);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> BuscarPorTitulo(int codigoTitulo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado>();

            query = query.Where(o => o.Titulo.Codigo == codigoTitulo);

            return query.ToList();
        }
    }
}
