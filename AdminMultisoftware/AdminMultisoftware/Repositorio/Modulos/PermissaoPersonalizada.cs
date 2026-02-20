using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Modulos
{
    public class PermissaoPersonalizada : RepositorioBase<Dominio.Entidades.Modulos.PermissaoPersonalizada>
    {
        public PermissaoPersonalizada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Modulos.PermissaoPersonalizada> BuscarPermissaoPorFormularario(int formulario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.PermissaoPersonalizada>();
            var result = from obj in query where obj.Formulario.Codigo == formulario select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Modulos.PermissaoPersonalizada> Consultar(string descricao, int formulario, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.PermissaoPersonalizada>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (formulario > 0)
                result = result.Where(obj => obj.Formulario.Codigo == formulario);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public List<Dominio.Entidades.Modulos.PermissaoPersonalizada> BuscarPorClieteETipoServico(Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool emHomologacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.PermissaoPersonalizada>();
            var result = from obj in query
                         where
            obj.Ativo && obj.Formulario.Ativo && obj.Formulario.Modulo.Ativo
                         select obj;

            result = result.Where(obj => (obj.Formulario.Modulo.TiposServicosMultisoftware.Contains(tipoServico) && (obj.Formulario.TiposServicosMultisoftware.Contains(tipoServico) || (obj.Formulario.TiposServicosMultisoftware.Count <= 0))) || (obj.Formulario.Modulo.TiposServicosMultisoftware.Count <= 0 && (obj.Formulario.TiposServicosMultisoftware.Contains(tipoServico) || (obj.Formulario.TiposServicosMultisoftware.Count <= 0))));

            result = result.Where(obj => obj.Formulario.Modulo.ClientesModulo.Any(cm => cm.Cliente.Codigo == cliente.Codigo && (cm.ModuloExclusivo || !cm.ModuloBloqueado)) || obj.Formulario.Modulo.ClientesModulo.Count() <= 0 || obj.Formulario.Modulo.ClientesModulo.All(cm => cm.Cliente.Codigo != cliente.Codigo && !cm.ModuloExclusivo));
            result = result.Where(obj => obj.Formulario.ClientesFormulario.Any(cm => cm.Cliente.Codigo == cliente.Codigo && (cm.FormularioExclusivo || !cm.FormularioBloqueado)) || obj.Formulario.ClientesFormulario.Count() <= 0 || obj.Formulario.ClientesFormulario.All(cm => cm.Cliente.Codigo != cliente.Codigo && !cm.FormularioExclusivo));

            if (!emHomologacao)
                result = result.Where(obj => !obj.Formulario.EmHomologacao && !obj.Formulario.Modulo.EmHomologacao);

            return result.ToList();
        }


        public int ContarConsulta(string descricao, int formulario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.PermissaoPersonalizada>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (formulario > 0)
                result = result.Where(obj => obj.Formulario.Codigo == formulario);

            return result.Count();
        }

        public Dominio.Entidades.Modulos.PermissaoPersonalizada BuscarPorPermissaoECodigoFormulario(Dominio.Enumeradores.PermissaoPersonalizada permissaoPersonalizada, int formulario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Modulos.PermissaoPersonalizada>();
            var result = from obj in query where obj.CodigoPermissao == permissaoPersonalizada && obj.Formulario.Codigo == formulario select obj;
            return result.FirstOrDefault();
        }

    }
}
