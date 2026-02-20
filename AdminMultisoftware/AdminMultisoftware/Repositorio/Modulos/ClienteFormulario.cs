using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Modulos
{
    public class ClienteFormulario : RepositorioBase<Dominio.Entidades.Modulos.ClienteFormulario>
    {
        public ClienteFormulario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Modulos.ClienteFormulario> BuscarTodosComDescricaoDiferenciadaPorCliente(int cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.ClienteFormulario>();
            var result = from obj in query where obj.Cliente.Codigo == cliente && obj.Descricao != null && obj.Descricao != "" select obj;
            return result.ToList();
        }

        public int ContarConsulta(int codigoCliente, bool usuarioEmbarcador, bool usuarioTMS, int codigoFormulario, string descricao, bool? exclusivo, bool? bloqueado)
        {
            var result = Consultar(codigoCliente, usuarioEmbarcador, usuarioTMS, codigoFormulario, descricao, exclusivo, bloqueado);

            return result.Count();
        }

        public List<Dominio.Entidades.Modulos.ClienteFormulario> Consultar(int codigoCliente, bool usuarioEmbarcador, bool usuarioTMS, int codigoFormulario, string descricao, bool? exclusivo, bool? bloqueado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(codigoCliente, usuarioEmbarcador, usuarioTMS, codigoFormulario, descricao, exclusivo, bloqueado);

            result = result
                .Fetch(o => o.Cliente)
                .Fetch(o => o.Formulario);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public Dominio.Entidades.Modulos.ClienteFormulario BuscarPorCodigoClienteFormulario(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.ClienteFormulario>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.ClienteFormulario>();
            return query.Select(obj => obj.Codigo).Max() + 1;
        }

        public bool VerificaFormularioExclusivo(int codigoFormulario, int codigoFormularioCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.ClienteFormulario>();

            var result = from obj in query where obj.FormularioExclusivo select obj;

            if (codigoFormularioCliente > 0)
                result = result.Where(obj => obj.Formulario.Codigo == codigoFormulario && obj.Codigo != codigoFormularioCliente);
            else
                result = result.Where(obj => obj.Formulario.Codigo == codigoFormulario);

            return result.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Modulos.ClienteFormulario> Consultar(int codigoCliente, bool usuarioEmbarcador, bool usuarioTMS, int codigoFormulario, string descricao, bool? exclusivo, bool? bloqueado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.ClienteFormulario>();

            var result = from obj in query select obj;

            if (codigoCliente > 0)
                result = result.Where(obj => obj.Cliente.Codigo.Equals(codigoCliente));

            if (codigoFormulario > 0)
                result = result.Where(obj => obj.Formulario.Codigo.Equals(codigoFormulario));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (bloqueado.HasValue)
                result = result.Where(obj => obj.FormularioBloqueado == bloqueado.Value);

            if (exclusivo.HasValue)
                result = result.Where(obj => obj.FormularioExclusivo == exclusivo.Value);

            if (usuarioEmbarcador)
                result = result.Where(obj => obj.Formulario.TiposServicosMultisoftware.Contains(Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                                             || obj.Formulario.TiposServicosMultisoftware.Contains(Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                                             || !obj.Formulario.TiposServicosMultisoftware.Any());

            if (usuarioTMS)
                result = result.Where(obj => obj.Formulario.TiposServicosMultisoftware.Contains(Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                             || !obj.Formulario.TiposServicosMultisoftware.Any());

            result = result.Fetch(p => p.Formulario);

            return result;
        }

        #endregion
    }
}
