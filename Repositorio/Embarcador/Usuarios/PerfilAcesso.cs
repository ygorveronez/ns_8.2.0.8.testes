using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Usuarios
{
    public class PerfilAcesso : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso>
    {
        public PerfilAcesso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso>();

            // Filtros
            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                query = query.Where(o => o.Ativo == ativo);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            return query;
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, status);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> BuscarTodosPerfis()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso>();
            var result = from obj in query select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso> RelatorioPerfis(int perfil, bool? ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso>();
            var result = from obj in query select obj;

            if (perfil > 0)
                result = result.Where(o => o.Codigo == perfil);

            if (ativo.HasValue)
                result = result.Where(o => o.Ativo == ativo.Value);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }
    }
}
