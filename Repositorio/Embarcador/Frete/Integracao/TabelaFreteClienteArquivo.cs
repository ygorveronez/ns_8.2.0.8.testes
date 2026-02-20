using System.Linq;

namespace Repositorio.Embarcador.Frete.Integracao
{
    public class TabelaFreteClienteArquivo : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo>
    {
        #region Constructores
        public TabelaFreteClienteArquivo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Metodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo> Consultar(int codigo, string codigoInternoLBL)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo>();

            if(codigo > 0)
                query = query.Where(t => t.Codigo == codigo);

            if(!string.IsNullOrWhiteSpace(codigoInternoLBL))
                query = query.Where(t => t.CodigoInternoLBC.Contains(codigoInternoLBL));

            return query;
        }

        #endregion

        #region Metodos Publicos 

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo BuscarPorCodigo(int codigo)
        {
            var consulta = Consultar(codigo, codigoInternoLBL: "" );
            return consulta.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo BuscarPorCodigoExternoLBC(string codigoExternoLBC)
        {
            var consulta = Consultar(codigo: 0, codigoExternoLBC);
            return consulta.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo BuscarPorCodigoECodigoExternoLBC(int codigo, string codigoExternoLBC)
        {
            var consulta = Consultar(codigo, codigoExternoLBC);
            return consulta.FirstOrDefault();
        }

        #endregion
    }
}
