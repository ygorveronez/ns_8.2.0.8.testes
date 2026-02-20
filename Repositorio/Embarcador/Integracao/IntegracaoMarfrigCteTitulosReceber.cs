using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoMarfrigCteTitulosReceber : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber>
    {
        public IntegracaoMarfrigCteTitulosReceber(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber BuscarPorCodigoCTe(int codigoCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber>();

            var result = from obj in query where obj.CTe.Codigo == codigoCte select obj;

            return result.Fetch(obj => obj.ArquivosTransacao).Fetch(obj => obj.CTe).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public bool ExisteIntegracaoPorCTE(int codigoCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber>();
            var result = from obj in query where obj.CTe.Codigo == codigoCte select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber> BuscarAguardandoIntegracao(DateTime? dataCorteConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber>();
            query = query.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (o.DataQuitacaoCadastro.HasValue && o.DataQuitacaoCadastro.Value >= DateTime.Now.AddDays(-5)));

            if (dataCorteConsulta.HasValue && dataCorteConsulta != DateTime.MinValue)
                query = query.Where(o => o.DataCadastro >= dataCorteConsulta);

            return query.Fetch(obj => obj.CTe).ThenFetch(obj => obj.Titulo).ToList();
        }
    }
}
