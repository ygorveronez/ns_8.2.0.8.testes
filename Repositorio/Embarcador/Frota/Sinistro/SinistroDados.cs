using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frota
{
    public class SinistroDados : RepositorioBase<Dominio.Entidades.Embarcador.Frota.SinistroDados>
    {
        public SinistroDados(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public SinistroDados(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.SinistroDados BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroDados>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var consultaSinistro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroDados>();
            int? ultimoNumero = consultaSinistro.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Frota.SinistroDados> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaSinistro filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.SinistroDados> result = Consultar(filtrosPesquisa);

            result = result
                .Fetch(o => o.Veiculo)
                .Fetch(o => o.Motorista)
                .Fetch(o => o.Cidade).ThenFetch(o => o.Estado).ThenFetch(o => o.Pais);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaSinistro filtrosPesquisa)
        {
            return Consultar(filtrosPesquisa).Count();
        }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Frota.SinistroDados> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaSinistro filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroDados>();

            if (filtrosPesquisa.DataSinistroInicial != DateTime.MinValue)
                query = query.Where(o => o.DataSinistro.Date >= filtrosPesquisa.DataSinistroInicial);

            if (filtrosPesquisa.DataSinistroFinal != DateTime.MinValue)
                query = query.Where(o => o.DataSinistro.Date <= filtrosPesquisa.DataSinistroFinal);

            if (filtrosPesquisa.Numero > 0)
                query = query.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBoletimOcorrencia))
                query = query.Where(o => o.NumeroBoletimOcorrencia == filtrosPesquisa.NumeroBoletimOcorrencia);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.Etapa.HasValue)
                query = query.Where(o => o.Etapa == filtrosPesquisa.Etapa.Value);

            if (filtrosPesquisa.CodigoCidade > 0)
                query = query.Where(o => o.Cidade.Codigo == filtrosPesquisa.CodigoCidade);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo);

            if (filtrosPesquisa.CodigoVeiculoReboque > 0)
                query = query.Where(o => o.VeiculoReboque.Codigo == filtrosPesquisa.CodigoVeiculoReboque);

            if (filtrosPesquisa.CodigoMotorista > 0)
                query = query.Where(o => o.Motorista.Codigo == filtrosPesquisa.CodigoMotorista);

            if (filtrosPesquisa.CodigoTipoSinistro > 0)
                query = query.Where(o => o.TipoSinistro.Codigo == filtrosPesquisa.CodigoTipoSinistro);

            if(filtrosPesquisa.CodigoGravidadeSinistro > 0)
                query = query.Where(o => o.GravidadeSinistro.Codigo == filtrosPesquisa.CodigoGravidadeSinistro);

            return query;
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Sinistro> ConsultarRelatorioSinistro(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSinistro = new ConsultaSinistro().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaSinistro.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Sinistro)));

            return consultaSinistro.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.Frota.Sinistro>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Sinistro>> ConsultarRelatorioSinistroAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSinistro = new ConsultaSinistro().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaSinistro.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Sinistro)));

            return await consultaSinistro.SetTimeout(1200).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.Sinistro>();
        }

        public int ContarConsultaRelatorioSinistro(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaSinistro = new ConsultaSinistro().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaSinistro.SetTimeout(1200).UniqueResult<int>();
        }

        #endregion
    }
}
