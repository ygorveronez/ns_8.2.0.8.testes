using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public sealed class FichaClienteLancamento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento>
    {
        #region Construtores

        public FichaClienteLancamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> Consultar(int codigoFichaCliente)
        {
            var consultaFichaClienteLancamentos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento>();

            if (codigoFichaCliente > 0)
                consultaFichaClienteLancamentos = consultaFichaClienteLancamentos.Where(o => o.FichaCliente.Codigo == codigoFichaCliente);

            return consultaFichaClienteLancamentos.AsQueryable();
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento BuscarPorCodigo(int codigo)
        {
            var consultaFichaClienteLancamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento>()
                .Where(o => o.Codigo == codigo);

            return consultaFichaClienteLancamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> BuscarPorFichaCliente(int codigoFichaCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento>();

            query = from obj in query where obj.FichaCliente.Codigo == codigoFichaCliente select obj;

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.FichaClienteLancamento> Consultar(int codigoFichaCliente, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaFichaClienteLancamento = Consultar(codigoFichaCliente);

            return ObterLista(consultaFichaClienteLancamento, parametroConsulta);
        }

        public int ContarConsulta(int codigoFichaCliente)
        {
            var consultaFichaClienteLancamento = Consultar(codigoFichaCliente);

            return consultaFichaClienteLancamento.Count();
        }

        #endregion Métodos Públicos
    }
}
