using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frotas
{
    public class PlanejamentoFrotaDia : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia>
    {
        public PlanejamentoFrotaDia(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PlanejamentoFrotaDia(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }



        public List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia> BuscarPorData(int codigoFilial, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia>();

            query = query.Where(x => x.Filial.Codigo == codigoFilial);

            if (dataInicial > DateTime.MinValue)
                query = query.Where(x => x.Data >= dataInicial.Date);
            
            if (dataFinal > DateTime.MinValue)
                query = query.Where(x => x.Data < dataFinal.Date.AddDays(1));
            
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia> BuscarPorData(int codigoFilial, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia>();

            query = query.Where(x => x.Filial.Codigo == codigoFilial);

            if (data > DateTime.MinValue)
                query = query.Where(x => x.Data == data.Date);

            return query.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PlanejamentoFrotaDia> ConsultarRelatorioPlanejamentoFrotaDia(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPlanejamentoFrotaDia = new Frota.ConsultaFrotaDia().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPlanejamentoFrotaDia.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.PlanejamentoFrotaDia)));

            return consultaPlanejamentoFrotaDia.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.PlanejamentoFrotaDia>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PlanejamentoFrotaDia>> ConsultarRelatorioPlanejamentoFrotaDiaAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPlanejamentoFrotaDia = new Frota.ConsultaFrotaDia().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPlanejamentoFrotaDia.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.PlanejamentoFrotaDia)));

            return await consultaPlanejamentoFrotaDia.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.PlanejamentoFrotaDia>();
        }

        public int ContarConsultaRelatorioPlanejamentoFrotaDia(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPlanejamentoFrotaDia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPlanejamentoFrotaDia = new Frota.ConsultaFrotaDia().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPlanejamentoFrotaDia.SetTimeout(600).UniqueResult<int>();
        }

    }
}
