using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frete
{
    public class ProvisaoVolumetria : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>
    {
        public ProvisaoVolumetria(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ProvisaoVolumetria(UnitOfWork unitOfWork,CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public int ContarConsultaRelatorioProvisaoVolumetria(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaProvisaoVolumetria = new ConsultaProvisaoVolumetria().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaProvisaoVolumetria.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ProvisaoVolumetria> RelatorioProvisaoVolumetria(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaProvisaoVolumetria = new ConsultaProvisaoVolumetria().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaProvisaoVolumetria.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ProvisaoVolumetria)));

            return consultaProvisaoVolumetria.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ProvisaoVolumetria>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ProvisaoVolumetria>> RelatorioProvisaoVolumetriaAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioProvisaoVolumetria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaProvisaoVolumetria = new ConsultaProvisaoVolumetria().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaProvisaoVolumetria.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ProvisaoVolumetria)));

            return await consultaProvisaoVolumetria.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Fretes.ProvisaoVolumetria>();
        }
    }
}
