using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class MercadoLivreHandlingUnitDetailArquivo : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo>
    {
        public MercadoLivreHandlingUnitDetailArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> BuscarPorDetail(Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo>();

            query = query.Where(o => o.HandlingUnitDetail == detail);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> BuscarPorHandlingUnit(Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit handlingUnit)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo>();

            query = query.Where(o => o.HandlingUnitDetail.HandlingUnit == handlingUnit);

            return query.Fetch(o => o.HandlingUnitDetail)
                        .Fetch(o => o.PedidoCTeParaSubcontratacao)
                        .ToList();
        }

        public Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo BuscarPorDetailEKey(int codigoDetail, string key)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo>();

            query = query.Where(o => o.HandlingUnitDetail.Codigo == codigoDetail && o.Key == key);

            return query.ToList().FirstOrDefault();
        }
    }
}
