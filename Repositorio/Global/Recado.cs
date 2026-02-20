using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class Recado : RepositorioBase<Dominio.Entidades.Recado>, Dominio.Interfaces.Repositorios.Recado
    {
        public Recado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Recado> Consultar(int codigoEmpresa, DateTime dataLancamento, string nomeUsuario, string titulo, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Recado>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataLancamento != DateTime.MinValue)
                result = result.Where(o => o.DataLancamento >= dataLancamento && o.DataLancamento <= dataLancamento.AddDays(1));

            if (!string.IsNullOrWhiteSpace(titulo))
                result = result.Where(o => o.Titulo.Contains(titulo));

            if (!string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Nome.Contains(nomeUsuario));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataLancamento, string nomeUsuario, string titulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Recado>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataLancamento != DateTime.MinValue)
                result = result.Where(o => o.DataLancamento >= dataLancamento && o.DataLancamento <= dataLancamento.AddDays(1));

            if (string.IsNullOrWhiteSpace(titulo))
                result = result.Where(o => o.Titulo.Contains(titulo));

            if (string.IsNullOrWhiteSpace(nomeUsuario))
                result = result.Where(o => o.Usuario.Nome.Contains(nomeUsuario));

            return result.Count();
        }

        public Dominio.Entidades.Recado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Recado>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }
    }
}
