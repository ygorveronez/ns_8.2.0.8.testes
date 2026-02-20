using System.Collections.Generic;
using System.Linq;
namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoMigrateRegimeTributario : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoMigrateRegimeTributario>
    {
        public IntegracaoMigrateRegimeTributario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMigrateRegimeTributario> Consultar(string descricao, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoMigrateRegimeTributario> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoMigrateRegimeTributario>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.ToList();
        }

        public int ContarConsulta(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoMigrateRegimeTributario> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoMigrateRegimeTributario>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));


            return query.Count();
        }


    }
}
