using System.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class ConfiguracaoWebServiceIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao>
    {
        public ConfiguracaoWebServiceIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao tipoWebServiceIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao>();
        
            query = query.Where(o => o.Tipo == tipoWebServiceIntegracao);

            return query.FirstOrDefault();
        }
    }
}
