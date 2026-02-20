using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class RotaFreteAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento>
    {
        #region Construtores

        public RotaFreteAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento> Consultar(int codigoRotaEmbarcador, int codigoModeloVeicular)
        {
            var consultaRotaFreteAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento>();

            if (codigoRotaEmbarcador > 0)
                consultaRotaFreteAbastecimento = consultaRotaFreteAbastecimento.Where(o => o.RotaFrete.Codigo == codigoRotaEmbarcador);

            if (codigoModeloVeicular > 0)
                consultaRotaFreteAbastecimento = consultaRotaFreteAbastecimento.Where(o => o.ModeloVeicular.Codigo == codigoModeloVeicular);

            return consultaRotaFreteAbastecimento;
        }

        
        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento> Consultar(int codigoRotaEmbarcador, int codigoModeloVeicular, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRotaFreteAbastecimento = Consultar(codigoRotaEmbarcador, codigoModeloVeicular);

            return ObterLista(consultaRotaFreteAbastecimento, parametrosConsulta);
        }

        public int ContarConsulta(int codigoRotaEmbarcador, int codigoModeloVeicular)
        {
            var consultaRotaFreteAbastecimento = Consultar(codigoRotaEmbarcador, codigoModeloVeicular);

            return consultaRotaFreteAbastecimento.Count();
        }
        public List<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento> BuscarPorRotaEModeloVeicular(int codigoRotaEmbarcador, int codigoModeloVeicular)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento>();

            if (codigoRotaEmbarcador > 0)
                query = query.Where(o => o.RotaFrete.Codigo == codigoRotaEmbarcador);

            if (codigoModeloVeicular > 0)
                query = query.Where(o => o.ModeloVeicular.Codigo == codigoModeloVeicular);
            else
                query = query.Where(o => o.ModeloVeicular == null);

            return query.ToList();

        }
        #endregion
    }
}
