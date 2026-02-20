using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class FechamentoAgregado : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado>
    {
        public FechamentoAgregado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado BuscarPorCIOT(int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado>();
            var resut = from obj in query where obj.CIOT.Codigo == codigoCIOT select obj;
            return resut.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado>();

            return (query.Max(o => (int?)o.Numero) ?? 0) + 1;
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado> Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, int codigoCIOT, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(usuario, dataInicial, dataFinal, codigoCIOT);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int usuario, DateTime dataInicial, DateTime dataFinal, int codigoCIOT)
        {
            var result = _Consultar(usuario, dataInicial, dataFinal, codigoCIOT);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado> _Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoCIOT > 0)
                result = result.Where(obj => obj.CIOT.Codigo == codigoCIOT);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataCriacao.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataCriacao.Date <= dataFinal);

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.Usuario.Codigo == usuario);

            return result;
        }

        #endregion
    }
}