using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoDadosTransportes : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>
    {
        public CargaIntegracaoDadosTransportes(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaIntegracaoDadosTransportes(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao BuscarPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;
            return resut.FirstOrDefault();
        }

        #endregion Métodos Públicos

        #region Relatório Assíncrono
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracaoDadosTransportes>> ConsultarRelatorioCargaIntegracaoDadosTransportesAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaCargaIntegracaoDadosTransportes().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracaoDadosTransportes)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracaoDadosTransportes>();
        }
        #endregion

        #region Relatório Integrações

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracaoDadosTransportes> ConsultarRelatorioCargaIntegracaoDadosTransportes(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaCargaIntegracaoDadosTransportes().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracaoDadosTransportes)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracaoDadosTransportes>();
        }

        public int ContarConsultaRelatorioCargaIntegracaoDadosTransportes(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaCargaIntegracaoDadosTransportes().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion Relatório Integrações
    }
}
