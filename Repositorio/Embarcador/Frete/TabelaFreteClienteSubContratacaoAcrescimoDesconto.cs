using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteClienteSubContratacaoAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto>
    {
        public TabelaFreteClienteSubContratacaoAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto> BuscarPorSubcontratacao(int codigoSubcontratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto>()
                .Where(o => o.TabelaFreteClienteSubContratacao.Codigo == codigoSubcontratacao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto> BuscarPorSubcontratacoes(List<int> codigosSubcontratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto>()
                .Where(o => codigosSubcontratacao.Contains(o.TabelaFreteClienteSubContratacao.Codigo));

            return query
                .Fetch(o => o.TabelaFreteClienteSubContratacao)
                .Fetch(o => o.Justificativa)
                .ToList();
        }
    }
}
