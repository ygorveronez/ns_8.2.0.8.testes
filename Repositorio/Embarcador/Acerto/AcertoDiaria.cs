using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoDiaria : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria>
    {
        public AcertoDiaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria> BuscarPorAcerto(int codigoAcertoViagem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcertoViagem);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Acerto.AcertoDiaria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria> BuscarAcertoDiaria(int codigoAcerto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarAcertoDiaria(int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Justificativa> BuscarJustificativas(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo select obj;
            return result.Select(c => c.Justificativa).Distinct().ToList();
        }

        public decimal BuscarValorPorJustificativa(int codigoJustificativa, int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Justificativa.Codigo == codigoJustificativa select obj;
            return result.Sum(c => c.Valor);
        }
    }
}
