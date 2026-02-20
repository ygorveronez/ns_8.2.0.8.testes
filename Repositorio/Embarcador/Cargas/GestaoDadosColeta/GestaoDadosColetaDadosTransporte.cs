using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.GestaoDadosColeta
{
    public class GestaoDadosColetaDadosTransporte: RepositorioBase<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte>
    {
        #region Construtores

        public GestaoDadosColetaDadosTransporte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte BuscarPorCodigo(int codigo)
        {
            var consultaGestaoDadosColetaTransporte = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte>()
                .Where(gestaoDadosColetaTransporte => gestaoDadosColetaTransporte.Codigo == codigo);

            return consultaGestaoDadosColetaTransporte.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte BuscarPorCodigoGestaoDadosColeta(int codigoGestaoDadosColeta)
        {
            var consultaGestaoDadosColetaTransporte = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte>()
                .Where(dadosColetaNFe => dadosColetaNFe.GestaoDadosColeta.Codigo == codigoGestaoDadosColeta);

            return consultaGestaoDadosColetaTransporte
                .Fetch(dadosColetaNFe => dadosColetaNFe.GestaoDadosColeta)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte>();
            var result = from obj in query where obj.GestaoDadosColeta.CargaEntrega.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }


        #endregion Métodos Públicos
    }
}
