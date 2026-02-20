using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoLegenda : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda>
    {
        #region Construtores

        public ConfiguracaoLegenda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda BuscarPorCodigoControle(CodigoControleLegenda codigoControle)
        {
            var consultaConfiguracaoLegenda = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda>()
                .Where(o => o.CodigoControle == codigoControle);

            return consultaConfiguracaoLegenda.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda> BuscarPorCodigosControle(List<CodigoControleLegenda> codigosControle)
        {
            var consultaConfiguracaoLegenda = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda>()
                .Where(o => codigosControle.Contains(o.CodigoControle));

            return consultaConfiguracaoLegenda.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda> BuscarPorGrupoCodigoControle(GrupoCodigoControleLegenda grupoCodigoControle)
        {
            List<CodigoControleLegenda> codigosControle = CodigoControleLegendaHelper.ObterCodigosControlePorGrupo(grupoCodigoControle);

            return BuscarPorCodigosControle(codigosControle);
        }

        #endregion
    }
}
