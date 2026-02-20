using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Modulos
{
    public class Modulo : RepositorioBase<Dominio.Entidades.Modulos.Modulo>
    {
        public Modulo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Modulos.Modulo> BuscarPorClieteETipoServico(Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool emHomologacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Modulo>();
            var result = from obj in query where (obj.TiposServicosMultisoftware.Contains(tipoServico) || obj.TiposServicosMultisoftware.Count <= 0) select obj;

            result = result.Where(obj => obj.ClientesModulo.Any(cm => cm.Cliente.Codigo == cliente.Codigo && (cm.ModuloExclusivo || !cm.ModuloBloqueado)) || obj.ClientesModulo.Count() <= 0 || obj.ClientesModulo.All(cm => cm.Cliente.Codigo != cliente.Codigo && !cm.ModuloExclusivo));
            result = result.Where(obj => obj.Ativo);

            if (!emHomologacao)
                result = result.Where(obj => !obj.EmHomologacao);

            return result.OrderBy(obj => obj.Sequencia).ToList();
        }

        public List<Dominio.Entidades.Modulos.Modulo> BuscarPorClienteETipoServicoEspecifico(Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool emHomologacao)
        {
            IQueryable<Dominio.Entidades.Modulos.Modulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Modulo>();

            query = query.Where(obj => obj.TiposServicosMultisoftware.Contains(tipoServico) && obj.Ativo);

            query = query.Where(obj => obj.ClientesModulo.Any(cm => cm.Cliente.Codigo == cliente.Codigo && (cm.ModuloExclusivo || !cm.ModuloBloqueado)) || obj.ClientesModulo.Count() <= 0 || obj.ClientesModulo.All(cm => cm.Cliente.Codigo != cliente.Codigo && !cm.ModuloExclusivo));

            if (!emHomologacao)
                query = query.Where(obj => !obj.EmHomologacao);

            return query.OrderBy(obj => obj.Sequencia).ToList();
        }

        public List<Dominio.Entidades.Modulos.Modulo> Consultar(string descricao, bool usuarioEmbarcador, bool usuarioTMS, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(descricao, usuarioEmbarcador, usuarioTMS);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, bool usuarioEmbarcador, bool usuarioTMS)
        {
            var result = Consultar(descricao, usuarioEmbarcador, usuarioTMS);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Modulos.Modulo> Consultar(string descricao, bool usuarioEmbarcador, bool usuarioTMS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Modulo>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

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

        public Dominio.Entidades.Modulos.Modulo BuscarPorCodigoModulo(int codigoModulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Modulo>();
            var result = from obj in query where obj.CodigoModulo == codigoModulo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.Modulo>();
            return query.Select(obj => obj.CodigoModulo).Max() + 1;
        }
    }
}
