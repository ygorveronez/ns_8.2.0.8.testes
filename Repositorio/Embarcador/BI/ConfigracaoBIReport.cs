using System;
using System.Linq;

namespace Repositorio.Embarcador.BI
{
    public class ConfigracaoBIReport : RepositorioBase<Dominio.Entidades.Embarcador.BI.ConfiguracaoBIReport>
    {
        public ConfigracaoBIReport(UnitOfWork unitOfWork) : base(unitOfWork)
        {

        }


        public Dominio.Entidades.Embarcador.BI.ConfiguracaoBIReport BuscarReport(int codigoFormulario)
        {
            IQueryable<Dominio.Entidades.Embarcador.BI.ConfiguracaoBIReport> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.BI.ConfiguracaoBIReport>();

            query = query.Where(obj => obj.CodigoFormulario == codigoFormulario);

            return query.FirstOrDefault();
        }

        public int SetarDadosAcessoFormulario(int codigoFormulario, string token, Guid tokenId, DateTime? expiration)
        {
            NHibernate.IQuery query = this.SessionNHiBernate.CreateQuery("UPDATE ConfiguracaoBIReport SET Token = :token, TokenId = :tokenId, TokenExpiration = :tokenExpiration WHERE CodigoFormulario = :codigoFormulario");

            query.SetString("token", token);
            query.SetString("tokenId", tokenId.ToString());
            query.SetParameter("tokenExpiration", expiration);
            query.SetInt32("codigoFormulario", codigoFormulario);

            return query.ExecuteUpdate();
        }
        public int SetarTokenAutentication(string token)
        {
            NHibernate.IQuery query = this.SessionNHiBernate.CreateQuery("UPDATE ConfiguracaoBIReport SET TokenAutentication = :token ");

            query.SetString("token", token);

            return query.ExecuteUpdate();
        }

        public int SetarDadosAcessoFormulario(int codigoFormulario, string token, string tokenId, DateTime? expiration)
        {
            var query = this.SessionNHiBernate.CreateQuery("UPDATE ConfiguracaoBIReport SET Token = :token, TokenId = :tokenId, TokenExpiration = :tokenExpiration WHERE CodigoFormulario = :codigoFormulario");

            query.SetString("token", token);
            query.SetString("tokenId", tokenId.ToString());
            query.SetParameter("tokenExpiration", expiration);
            query.SetInt32("codigoFormulario", codigoFormulario);

            return query.ExecuteUpdate();
        }

    }
}
