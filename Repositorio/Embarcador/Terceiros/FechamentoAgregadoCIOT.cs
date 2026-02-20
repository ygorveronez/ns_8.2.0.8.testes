using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class FechamentoAgregadoCIOT : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>
    {
        public FechamentoAgregadoCIOT(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT> BuscarPorCodigoFechamentoAgregado(int codigoFechamentoAgregado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>();
            var resut = from obj in query where obj.FechamentoAgregado.Codigo == codigoFechamentoAgregado select obj;
            return resut.Fetch(obj => obj.CIOT).ToList();
        }

        public List<int> BuscarCodigoCIOTPorCodigoFechamentoAgregado(int codigoFechamentoAgregado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>();
            var resut = from obj in query where obj.FechamentoAgregado.Codigo == codigoFechamentoAgregado select obj;
            return query.Select(o => o.CIOT.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarCIOTPorCodigoFechamentoAgregado(int codigoFechamentoAgregado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>();
            var queryCIOT = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            var result = query.Where(o => o.Codigo == codigoFechamentoAgregado).FirstOrDefault();

            var resultCiot = from obj in queryCIOT where obj.Codigo == result.CIOT.Codigo select obj;
            return resultCiot.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT BuscarPorCIOT(int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>();
            var resut = from obj in query where obj.CIOT.Codigo == codigoCIOT select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT> Consultar(int codigoFechamentoAgregado, int codigoCIOT, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoFechamentoAgregado, codigoCIOT);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Fetch(o => o.CIOT).ToList();
        }

        public int ContarConsulta(int codigoFechamentoAgregado, int codigoCIOT = 0)
        {
            var result = _Consultar(codigoFechamentoAgregado, codigoCIOT);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT> _Consultar(int codigoFechamentoAgregado, int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoFechamentoAgregado > 0)
                result = result.Where(obj => obj.FechamentoAgregado.Codigo == codigoFechamentoAgregado);

            if (codigoCIOT > 0)
                result = result.Where(obj => obj.CIOT.Codigo == codigoCIOT);

            return result;
        }

        #endregion
    }
}