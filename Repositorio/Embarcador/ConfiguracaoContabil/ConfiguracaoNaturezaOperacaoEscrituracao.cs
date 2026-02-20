using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoNaturezaOperacaoEscrituracao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoEscrituracao>
    {
        public ConfiguracaoNaturezaOperacaoEscrituracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoEscrituracao> BuscarPorConfiguracaoNaturezaOperacao(int configuracaoNaturezaOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoEscrituracao>();
            var result = from obj in query where obj.ConfiguracaoNaturezaOperacao.Codigo == configuracaoNaturezaOperacao select obj;
            return result.ToList();
        }

    }
}
