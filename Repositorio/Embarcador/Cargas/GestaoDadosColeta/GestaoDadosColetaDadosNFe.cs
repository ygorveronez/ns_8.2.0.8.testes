using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.GestaoDadosColeta
{
    public class GestaoDadosColetaDadosNFe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe>
    {
        #region Construtores

        public GestaoDadosColetaDadosNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe BuscarPorCodigo(int codigo)
        {
            var consultaGestaoDadosColetaNFe = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe>()
                .Where(dadosColetaNFe => dadosColetaNFe.Codigo == codigo);
            return consultaGestaoDadosColetaNFe.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe BuscarPorCodigoGestaoDadosColeta(int codigoGestaoDadosColeta)
        {
            var consultaGestaoDadosColetaNFe = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe>()
                .Where(dadosColetaNFe => dadosColetaNFe.GestaoDadosColeta.Codigo == codigoGestaoDadosColeta);

            return consultaGestaoDadosColetaNFe
                .Fetch(dadosColetaNFe => dadosColetaNFe.GestaoDadosColeta)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe>();
            var result = from obj in query where obj.GestaoDadosColeta.CargaEntrega.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        #endregion Métodos Públicos
    }
}
