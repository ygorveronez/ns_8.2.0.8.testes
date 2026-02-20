using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using AdminMultisoftware.Dominio.Entidades.Modulos;

namespace AdminMultisoftware.Repositorio.Modulos
{
    public class Formulario : RepositorioBase<Dominio.Entidades.Modulos.Formulario>
    {
        public Formulario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Modulos.Formulario> BuscarFormularioPorModulo(int modulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Formulario>();
            var result = from obj in query where obj.Modulo.Codigo == modulo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Modulos.Formulario> Consultar(string descricao, bool usuarioEmbarcador, bool usuarioTMS, int modulo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consulta(descricao, usuarioEmbarcador, usuarioTMS, modulo);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        private IQueryable<Dominio.Entidades.Modulos.Formulario> Consulta(string descricao, bool usuarioEmbarcador, bool usuarioTMS, int modulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Formulario>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (modulo > 0)
                result = result.Where(obj => obj.Modulo.Codigo == modulo);

            if (usuarioEmbarcador)
                result = result.Where(obj => obj.TiposServicosMultisoftware.Contains(Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                                             || obj.TiposServicosMultisoftware.Contains(Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                                             || !obj.TiposServicosMultisoftware.Any());

            if (usuarioTMS)
                result = result.Where(obj => obj.TiposServicosMultisoftware.Contains(Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                             || !obj.TiposServicosMultisoftware.Any());

            result = result.Fetch(p => p.TiposServicosMultisoftware);

            return result;
        }

        public List<Dominio.Entidades.Modulos.Formulario> BuscarPorClieteETipoServico(Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool emHomologacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Formulario>();
            var result = from obj in query
                         where
                            obj.Ativo && obj.Modulo.Ativo
                         select obj;

            result = result.Where(obj => (obj.Modulo.TiposServicosMultisoftware.Contains(tipoServico) && (obj.TiposServicosMultisoftware.Contains(tipoServico) || (obj.TiposServicosMultisoftware.Count <= 0))) || (obj.Modulo.TiposServicosMultisoftware.Count <= 0 && (obj.TiposServicosMultisoftware.Contains(tipoServico) || (obj.TiposServicosMultisoftware.Count <= 0))));
            //result = result.Where(obj => obj.TiposServicosMultisoftware.Contains(tipoServico) || (obj.TiposServicosMultisoftware.Count <= 0));

            result = result.Where(obj => obj.Modulo.ClientesModulo.Any(cm => cm.Cliente.Codigo == cliente.Codigo && (cm.ModuloExclusivo || !cm.ModuloBloqueado)) || obj.Modulo.ClientesModulo.Count() <= 0 || obj.Modulo.ClientesModulo.All(cm => cm.Cliente.Codigo != cliente.Codigo && !cm.ModuloExclusivo));
            result = result.Where(obj => obj.ClientesFormulario.Any(cm => cm.Cliente.Codigo == cliente.Codigo && (cm.FormularioExclusivo || !cm.FormularioBloqueado)) || obj.ClientesFormulario.Count() <= 0 || obj.ClientesFormulario.All(cm => cm.Cliente.Codigo != cliente.Codigo && !cm.FormularioExclusivo));

            if (!emHomologacao)
                result = result.Where(obj => !obj.EmHomologacao && !obj.Modulo.EmHomologacao);

            return result.OrderBy(obj => obj.Modulo.Codigo).OrderBy(obj => obj.Sequencia)
                .Fetch(obj => obj.Modulo)
                .ToList();
        }

        public List<Dominio.Entidades.Modulos.Formulario> BuscarPorClienteETipoServicoEspecifico(Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool emHomologacao)
        {
            IQueryable<Dominio.Entidades.Modulos.Formulario> query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Formulario>();

            query = query.Where(o => o.Ativo && o.Modulo.Ativo && (o.Modulo.TiposServicosMultisoftware.Contains(tipoServico) || o.TiposServicosMultisoftware.Contains(tipoServico)));

            query = query.Where(obj => obj.Modulo.ClientesModulo.Any(cm => cm.Cliente.Codigo == cliente.Codigo && (cm.ModuloExclusivo || !cm.ModuloBloqueado)) || obj.Modulo.ClientesModulo.Count() <= 0 || obj.Modulo.ClientesModulo.All(cm => cm.Cliente.Codigo != cliente.Codigo && !cm.ModuloExclusivo));
            query = query.Where(obj => obj.ClientesFormulario.Any(cm => cm.Cliente.Codigo == cliente.Codigo && (cm.FormularioExclusivo || !cm.FormularioBloqueado)) || obj.ClientesFormulario.Count() <= 0 || obj.ClientesFormulario.All(cm => cm.Cliente.Codigo != cliente.Codigo && !cm.FormularioExclusivo));

            if (!emHomologacao)
                query = query.Where(obj => !obj.EmHomologacao && !obj.Modulo.EmHomologacao);

            return query.OrderBy(obj => obj.Modulo.Codigo)
                        .OrderBy(obj => obj.Sequencia)
                        .Fetch(obj => obj.Modulo)
                        .ToList();
        }

        public int ContarConsulta(string descricao, int modulo, bool usuarioEmbarcador, bool usuarioTMS)
        {
            var result = Consulta(descricao, usuarioEmbarcador, usuarioTMS, modulo);

            return result.Count();
        }

        public Dominio.Entidades.Modulos.Formulario BuscarPorCodigoFormulario(int codigoFormulario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Formulario>();
            var result = from obj in query where obj.CodigoFormulario == codigoFormulario select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Modulos.Formulario> BuscarPorCodigosFormularios(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Formulario>()
                .Where(obj => codigos.Contains(obj.CodigoFormulario));

            return query
                .ToList();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Formulario>();
            return query.Select(obj => obj.CodigoFormulario).Max() + 1;
        }

        public List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> BuscarPorTipoServico(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico)
        {
            var query = this.SessionNHiBernate.Query<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario>();

            var result = from obj in query where obj.TiposServicosMultisoftware.Contains(tipoServico) select obj;

            return result.ToList();
        }
    }
}
