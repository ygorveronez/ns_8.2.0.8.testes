using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiraBaixaTituloReceberMoeda : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda>
    {
        public ConfiguracaoFinanceiraBaixaTituloReceberMoeda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda BuscarPorMoeda(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda>();

            query = query.Where(o => o.Moeda == moeda);

            return query.Fetch(o => o.JustificativaAcrescimo)
                        .Fetch(o => o.JustificativaDesconto)
                        .FirstOrDefault();
        }
    }
}
