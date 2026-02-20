using System.Linq;


namespace Repositorio.Embarcador.DTe
{
    public class DocumentoTransporteEletronico : RepositorioBase<Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico>
    {
        public DocumentoTransporteEletronico(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarUltimoNumero(int codigoEmpresa, int serie, Dominio.Enumeradores.TipoAmbiente ambiente)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Serie.Codigo", serie));
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("TipoAmbiente", ambiente));



            criteria.SetProjection(NHibernate.Criterion.Projections.Max("Numero"));
            return criteria.UniqueResult<int>();
        }

    }
}
