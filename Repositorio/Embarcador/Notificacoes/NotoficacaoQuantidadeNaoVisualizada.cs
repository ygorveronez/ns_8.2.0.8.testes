using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Notificacoes
{
    public class NotoficacaoQuantidadeNaoVisualizada : RepositorioBase<Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada>
    {
        public NotoficacaoQuantidadeNaoVisualizada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada BuscarPorUsuario(int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.NotoficacaoQuantidadeNaoVisualizada>();
            var result = from obj in query where obj.Usuario.Codigo == usuario select obj;
            return result.FirstOrDefault();
        }
    }
}
