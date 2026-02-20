using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Modulos
{
    public class ClienteModulo : RepositorioBase<Dominio.Entidades.Modulos.ClienteModulo>
    {
        public ClienteModulo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Método Públicos

        public List<Dominio.Entidades.Modulos.ClienteModulo> BuscarTodosComDescricaoDiferenciadaPorCliente(int cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.ClienteModulo>();
            var result = from obj in query where obj.Cliente.Codigo == cliente && obj.Descricao != null && obj.Descricao != "" select obj;
            return result.ToList();
        }

        public int ContarConsulta(int codigoCliente, bool usuarioEmbarcador, bool usuarioTMS, int codigoModulo, string descricao, bool? exclusivo, bool? bloqueado)
        {
            var result = Consultar(codigoCliente, usuarioEmbarcador, usuarioTMS, codigoModulo, descricao, exclusivo, bloqueado);

            return result.Count();
        }

        public List<Dominio.Entidades.Modulos.ClienteModulo> Consultar(int codigoCliente, bool usuarioEmbarcador, bool usuarioTMS, int codigoModulo, string descricao, bool? exclusivo, bool? bloqueado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(codigoCliente, usuarioEmbarcador, usuarioTMS, codigoModulo, descricao, exclusivo, bloqueado);

            result = result
                .Fetch(o => o.Cliente)
                .Fetch(o => o.Modulo);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public Dominio.Entidades.Modulos.ClienteModulo BuscarPorCodigoClienteFormulario(int codigoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.ClienteModulo>();
            var result = from obj in query where obj.Codigo == codigoPessoa select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.ClienteModulo>();
            return query.Select(obj => obj.Codigo).Max() + 1;
        }

        public bool VerificaModuloExclusivo(int codigoModulo, int codigoModuloCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.ClienteModulo>();

            var result = from obj in query where obj.ModuloExclusivo select obj;

            if (codigoModuloCliente > 0)
                result = result.Where(obj => obj.Modulo.Codigo == codigoModulo && obj.Codigo != codigoModuloCliente);
            else
                result = result.Where(obj => obj.Modulo.Codigo == codigoModulo);

            return result.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Modulos.ClienteModulo> Consultar(int codigoCliente, bool usuarioEmbarcador, bool usuarioTMS, int codigoModulo, string descricao, bool? exclusivo, bool? bloqueado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.ClienteModulo>();

            var result = from obj in query select obj;

            if (codigoCliente > 0)
                result = result.Where(obj => obj.Cliente.Codigo.Equals(codigoCliente));

            if (codigoModulo > 0)
                result = result.Where(obj => obj.Modulo.Codigo.Equals(codigoModulo));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (bloqueado.HasValue)
                result = result.Where(obj => obj.ModuloBloqueado == bloqueado.Value);

            if (exclusivo.HasValue)
                result = result.Where(obj => obj.ModuloExclusivo == exclusivo.Value);

            if (usuarioEmbarcador)
                result = result.Where(obj => obj.Modulo.TiposServicosMultisoftware.Contains(Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                                             || obj.Modulo.TiposServicosMultisoftware.Contains(Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                                             || !obj.Modulo.TiposServicosMultisoftware.Any());
            if (usuarioTMS)
                result = result.Where(obj => obj.Modulo.TiposServicosMultisoftware.Contains(Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                             || !obj.Modulo.TiposServicosMultisoftware.Any());

            result = result.Fetch(p => p.Modulo);

            return result;
        }

        #endregion
    }
}
