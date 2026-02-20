using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoNaturezaOperacaoContabilizacao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao>
    {
        public ConfiguracaoNaturezaOperacaoContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao> BuscarPorConfiguracaoNaturezaOperacao(int configuracaoNaturezaOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao>();
            var result = from obj in query where obj.ConfiguracaoNaturezaOperacao.Codigo == configuracaoNaturezaOperacao select obj;
            return result.ToList();
        }

    }
}
