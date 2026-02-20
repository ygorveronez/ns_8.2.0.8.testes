using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class PerfilPermissao : RepositorioBase<Dominio.Entidades.PerfilPermissao>, Dominio.Interfaces.Repositorios.PerfilPermissao
    {
        public PerfilPermissao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PerfilPermissao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PerfilPermissao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PerfilPermissao> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PerfilPermissao>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.PerfilPermissao> Consulta(int codigoEmpresa, string descricao, int inicioRegistros, int maximoRegistros, bool? ativo = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PerfilPermissao>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (ativo != null)
                result = result.Where(o => o.Ativo == ativo);

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.PerfilPermissao> ConsultaListaEmpresa(List<int> codigosEmpresa, string descricao, int inicioRegistros, int maximoRegistros, bool? ativo = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PerfilPermissao>();
            var result = from obj in query select obj;

            if (codigosEmpresa != null && codigosEmpresa.Count > 0)
                result = result.Where(o => codigosEmpresa.Contains(o.Empresa.Codigo));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (ativo != null)
                result = result.Where(o => o.Ativo == ativo);

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultr(int codigoEmpresa, string descricao, bool? ativo = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PerfilPermissao>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (ativo != null)
                result = result.Where(o => o.Ativo == ativo);

            return result.Count();
        }

    }
} 
