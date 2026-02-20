using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;


namespace AdminMultisoftware.Repositorio.Mobile
{
    public class UsuarioMobileCliente : RepositorioBase<Dominio.Entidades.Mobile.UsuarioMobileCliente>
    {
        public UsuarioMobileCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Mobile.UsuarioMobileCliente> BuscarPorUsuario(int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.UsuarioMobileCliente>();
            var result = from obj in query where obj.UsuarioMobile.Codigo == usuario select obj;
            return result
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.ClienteConfiguracao)
                .ToList();
        }

        public Dominio.Entidades.Mobile.UsuarioMobileCliente BuscarPorUsuarioECliente(int usuario, int cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.UsuarioMobileCliente>();
            var result = from obj in query where obj.UsuarioMobile.Codigo == usuario && obj.Cliente.Codigo == cliente select obj;
            return result
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.ClienteConfiguracao)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Mobile.UsuarioMobileCliente BuscarPorCliente(int cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.UsuarioMobileCliente>();
            var result = from obj in query where obj.Cliente.Codigo == cliente select obj;
            return result
                .Fetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.ClienteConfiguracao)
                .FirstOrDefault();
        }

        public bool ClienteExigeContraSenha(int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.UsuarioMobileCliente>();
            var result = from obj in query where obj.UsuarioMobile.Codigo == usuario select obj;

            result = result.Where(obj => obj.BaseHomologacao ? obj.Cliente.ClienteConfiguracaoHomologacao.ExigeContraSenha : obj.Cliente.ClienteConfiguracao.ExigeContraSenha);

            return result.Any();
        }
    }
}
