using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;


namespace AdminMultisoftware.Repositorio.Pessoas
{
    public class Usuario : RepositorioBase<AdminMultisoftware.Dominio.Entidades.Pessoas.Usuario>
    {
        public Usuario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Pessoas.Usuario> Consultar(string nome, string login, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.Usuario>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(login))
                result = result.Where(obj => obj.Nome.Contains(login));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string nome, string login)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.Usuario>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(login))
                result = result.Where(obj => obj.Nome.Contains(login));

            return result.Count();
        }

        public Dominio.Entidades.Pessoas.Usuario BuscarPorLoginESenha(string login, string senha)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.Usuario>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Login.Equals(login));
          
            result = result.Where(obj => obj.Senha.Equals(senha));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Pessoas.Usuario BuscarPorCodigo(int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.Usuario>();

            var result = from obj in query where obj.Codigo == codigoUsuario select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Pessoas.Usuario BuscarPrimeiro()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pessoas.Usuario>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }

    }
}
