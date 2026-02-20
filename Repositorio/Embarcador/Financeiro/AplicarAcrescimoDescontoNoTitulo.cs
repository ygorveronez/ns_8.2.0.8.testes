using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class AplicarAcrescimoDescontoNoTitulo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo>
    {
        public AplicarAcrescimoDescontoNoTitulo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo> BuscarPorCodigo(int codigo, int codigoTitulo, int codigoJustificativa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consulta = Consultar(codigo, codigoTitulo, codigoJustificativa);

            return ObterLista(consulta, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }
        public Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo>();
            query = from obj in query where obj.Codigo == codigo select obj;
            return query.FirstOrDefault();
        }

        public int ContarBuscar(int codigo, int codigoTitulo, int codigoJustificativa)
        {
            var consulta = Consultar(codigo, codigoTitulo, codigoJustificativa);

            return consulta.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo BuscarPorTitulo(int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo>();

            if (codigoTitulo > 0)
                query = query.Where(t => t.Titulo.Codigo == codigoTitulo);

            return query.FirstOrDefault();
        }



        #endregion
        #region Metodo Privados 
        public IQueryable<Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo> Consultar(int codigo, int codigoTitulo, int codigoJustificativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AplicarAcrescimoDescontoNoTitulo>();

            if (codigo > 0)
                query = query.Where(t => t.Codigo == codigo);

            if (codigoTitulo > 0)
                query = query.Where(o => o.Titulo.Codigo == codigoTitulo);

            if (codigoJustificativa > 0)
                query = query.Where(o => o.Justificativa.Codigo == codigoJustificativa);

            return query;
        }
        #endregion


    }
}
