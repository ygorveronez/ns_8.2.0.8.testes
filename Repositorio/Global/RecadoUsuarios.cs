using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class RecadoUsuarios : RepositorioBase<Dominio.Entidades.RecadoUsuarios>, Dominio.Interfaces.Repositorios.RecadoUsuarios
    {
        public RecadoUsuarios(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.RecadoUsuarios> BuscarPorRecado(int codigoRecado, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RecadoUsuarios>();

            query = query.Where(o => o.Recado.Codigo == codigoRecado);

            return query.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarPorRecado(int codigoRecado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RecadoUsuarios>();

            query = query.Where(o => o.Recado.Codigo == codigoRecado);

            return query.Count();
        }

        public List<Dominio.Entidades.RecadoUsuarios> ConsultarPendentesUsuario(int codigoUsuarioLeitura, string status, string titulo, string mensagem, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RecadoUsuarios>();
            var result = from obj in query where obj.Recado.Ativo && obj.Usuario.Codigo == codigoUsuarioLeitura select obj;

            if (status == "P")
                result = result.Where(o => o.DataLeitura == null);
            else if (status == "L")
                result = result.Where(o => o.DataLeitura != null);

            if (!string.IsNullOrWhiteSpace(titulo))
                result = result.Where(o => o.Recado.Titulo.Contains(titulo));

            if (!string.IsNullOrWhiteSpace(mensagem))
                result = result.Where(o => o.Recado.Descricao.Contains(mensagem));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarPendentesUsuario(int codigoUsuarioLeitura, string status, string titulo, string mensagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RecadoUsuarios>();
            var result = from obj in query where obj.Recado.Ativo && obj.Usuario.Codigo == codigoUsuarioLeitura select obj;

            if (status == "P")
                result = result.Where(o => (o.DataLeitura == null));
            else if (status == "L")
                result = result.Where(o => o.DataLeitura != null);

            if (!string.IsNullOrWhiteSpace(titulo))
                result = result.Where(o => o.Recado.Titulo.Contains(titulo));

            if (!string.IsNullOrWhiteSpace(mensagem))
                result = result.Where(o => o.Recado.Descricao.Contains(mensagem));

            return result.Count();
        }

        public Dominio.Entidades.RecadoUsuarios BuscarPorRecadoEUsuarioLeitura(int codigoRecado, int codigoUsuarioLeitura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RecadoUsuarios>();

            query = query.Where(o => o.Recado.Codigo == codigoRecado && o.Usuario.Codigo == codigoUsuarioLeitura);

            return query.FirstOrDefault();
        }

    }
}
