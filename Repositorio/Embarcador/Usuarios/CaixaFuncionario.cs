using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios
{
    public class CaixaFuncionario : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario>
    {
        public CaixaFuncionario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario BuscarPorCaixaAbertoFuncionario(int codigoFuncionario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario>();
            var result = from obj in query where obj.Usuario.Codigo == codigoFuncionario && obj.SituacaoCaixa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa.Aberto select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario> Consultar(int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa situacaoCaixa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario>();

            var result = from obj in query select obj;

            if (codigoUsuario > 0)
                result = result.Where(obj => obj.Usuario.Codigo == codigoUsuario);

            if ((int)situacaoCaixa > 0)
                result = result.Where(obj => obj.SituacaoCaixa == situacaoCaixa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCaixa situacaoCaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionario>();

            var result = from obj in query select obj;

            if (codigoUsuario > 0)
                result = result.Where(obj => obj.Usuario.Codigo == codigoUsuario);

            if ((int)situacaoCaixa > 0)
                result = result.Where(obj => obj.SituacaoCaixa == situacaoCaixa);

            return result.Count();
        }
    }
}
