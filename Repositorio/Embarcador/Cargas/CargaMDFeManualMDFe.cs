using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManualMDFe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>
    {
        public CargaMDFeManualMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe> BuscarPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe BuscarPorCargaMDFeManualENumeroMDFe(int codigoCargaMDFeManual, int numeroMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);
            query = query.Where(o => o.MDFe.Numero == numeroMDFe);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe> BuscarAutorizadosPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual && (o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado));

            return query.ToList();
        }

        public int ContarAutorizadosPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual && (o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado));

            return query.Count();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe> ConsultarMDFe(int cargaMDFeManual, Dominio.Enumeradores.StatusMDFe statusMDFe, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();

            var result = from obj in query where obj.CargaMDFeManual.Codigo == cargaMDFeManual && obj.MDFe != null select obj;

            if (statusMDFe != Dominio.Enumeradores.StatusMDFe.Todos)
                result = result.Where(obj => obj.MDFe.Status == statusMDFe);

            return result.Fetch(o => o.MDFe).ThenFetch(o => o.EstadoCarregamento)
                         .Fetch(o => o.MDFe).ThenFetch(o => o.EstadoDescarregamento)
                           .Fetch(o => o.MDFe).ThenFetch(o => o.Serie)
                         .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaMDFe(int cargaMDFeManual, Dominio.Enumeradores.StatusMDFe statusMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();

            var result = from obj in query where obj.CargaMDFeManual.Codigo == cargaMDFeManual && obj.MDFe != null select obj;

            if (statusMDFe != Dominio.Enumeradores.StatusMDFe.Todos)
                result = result.Where(obj => obj.MDFe.Status == statusMDFe);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe BuscarPrimeiroPorCargaMDFeManual(int cargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == cargaMDFeManual);

            return query.FirstOrDefault();
        }

    }
}
