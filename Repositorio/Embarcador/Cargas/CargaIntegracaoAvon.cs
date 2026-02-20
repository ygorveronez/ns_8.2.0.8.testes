using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoAvon : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>
    {
        public CargaIntegracaoAvon(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga orderby obj.Codigo descending select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon BuscarPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMinutaAvon situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Situacao == situacao select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon> Consultar(int codigoCarga, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.Count();
        }
    }
}
