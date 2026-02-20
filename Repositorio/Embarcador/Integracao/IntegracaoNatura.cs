using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoNatura : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura>
    {
        public IntegracaoNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura> Consultar(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura>();

            var result = from obj in query where obj.SubIntegracoes.Any() select obj;

            return result.OrderByDescending(o => o.DataConsulta).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura>();

            var result = from obj in query where obj.SubIntegracoes.Any() select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura tipoIntegracao, bool possuiSubIntegracao, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura>();

            query = query.Where(o => o.Tipo == tipoIntegracao);

            if (possuiSubIntegracao)
                query = query.Where(o => o.SubIntegracoes.Any());

            return query.OrderByDescending(o => o.DataConsulta).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura tipoIntegracao, bool possuiSubIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura>();

            query = query.Where(o => o.Tipo == tipoIntegracao);

            if (possuiSubIntegracao)
                query = query.Where(o => o.SubIntegracoes.Any());

            return query.Count();
        }
    }
}
