using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral>
    {
        public TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral> BuscarPorSubcontratacaoGeral(int codigoSubcontratacaoGeral)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral>()
                .Where(o => o.Codigo == codigoSubcontratacaoGeral);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral> BuscarPorSubcontratacoesGerais(List<int> codigosSubcontratacaoGeral)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral>()
                .Where(o => codigosSubcontratacaoGeral.Contains(o.Codigo));

            return query
                .Fetch(o => o)
                .Fetch(o => o.Justificativa)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral> BuscarPorTabelaFrete(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral>()
                .Where(o => o.TabelaFreteCliente.Codigo == codigoTabelaFrete);

            return query.ToList();
        }
    }
}