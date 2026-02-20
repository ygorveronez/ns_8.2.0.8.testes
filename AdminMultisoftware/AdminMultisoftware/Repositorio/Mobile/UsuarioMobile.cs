using System.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Mobile
{
    public class UsuarioMobile : RepositorioBase<Dominio.Entidades.Mobile.UsuarioMobile>
    {
        public UsuarioMobile(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Mobile.UsuarioMobile BuscarPorCFPESenha(string cpf, string senha)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.UsuarioMobile>();
            var result = from obj in query where obj.CPF == cpf && obj.Senha == senha && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Mobile.UsuarioMobile BuscarPorCFP(string cpf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.UsuarioMobile>();
            var result = from obj in query where obj.CPF == cpf && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Mobile.UsuarioMobile BuscarPorSessao(string sessao, System.DateTime dataSessao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.UsuarioMobile>();
            var result = from obj in query where obj.Sessao == sessao && obj.DataSessao >= dataSessao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Mobile.UsuarioMobile BuscarPorSessao(string sessao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.UsuarioMobile>();
            var result = from obj in query where obj.Sessao == sessao select obj;
            return result.FirstOrDefault();
        }
    }
}
