using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class BoletoAlteracaoTitulo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo>
    {
        public BoletoAlteracaoTitulo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo BuscarPorTituloEAlteracao(int codigoTitulo, int codigoAlteracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo>();
            var result = from obj in query where obj.Titulo.Codigo == codigoTitulo && obj.BoletoAlteracao.Codigo == codigoAlteracao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo> BuscarPorAlteracao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo>();
            var result = from obj in query where obj.BoletoAlteracao.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo> ConsultarPorCodigoBoletoAlteracao(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo>();
            var result = from obj in query where obj.BoletoAlteracao.Codigo == codigo select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarPorCodigoBoletoAlteracao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo>();
            var result = from obj in query where obj.BoletoAlteracao.Codigo == codigo select obj;

            return result.Count();
        }
    }
}