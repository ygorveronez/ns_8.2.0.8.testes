using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSAD : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD>
    {
        public IntegracaoSAD(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD BuscarPorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD>();

            query = query.Where(o => o.CentroDescarregamento.Codigo == codigoCentroDescarregamento);
            
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD BuscarRegistroSemCentroDescarregamento()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD>();
            
            query = query.Where(o => o.CentroDescarregamento == null);

            return query.FirstOrDefault();
        }

        public List<(string URL, int CodigoCentroDescarregamento)> BuscarURLsCancelarAgendaPorCentrosDescarregamento(List<int> codigosCentrosDescarregamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAD>()
                .Where(obj => (codigosCentrosDescarregamento.Contains(obj.CentroDescarregamento.Codigo) || obj.CentroDescarregamento == null) && (obj.URLIntegracaoSADCancelarAgenda != null && obj.URLIntegracaoSADCancelarAgenda != " "));
            
            return query.Select(
                obj => ValueTuple.Create(
                    obj.URLIntegracaoSADCancelarAgenda,
                    obj.CentroDescarregamento != null ? obj.CentroDescarregamento.Codigo : 0
                )).ToList();
        }
    }
}
