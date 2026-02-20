using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoElectroluxConsultaLog : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog>
    {
        public IntegracaoElectroluxConsultaLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoElectrolux tipoIntegracao, bool possuiSubIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog>();

            query = query.Where(o => o.Tipo == tipoIntegracao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoElectrolux tipoIntegracao, bool possuiSubIntegracao, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog>();

            query = query.Where(o => o.Tipo == tipoIntegracao);

            return query.OrderByDescending(o => o.DataConsulta).Skip(inicio).Take(limite).ToList();
        }


    }
}
