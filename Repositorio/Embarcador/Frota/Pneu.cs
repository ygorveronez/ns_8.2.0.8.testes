using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frota
{
    public class Pneu : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Pneu>
    {
        #region Construtores

        public Pneu(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Pneu(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.Pneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneu filtrosPesquisa)
        {
            var consultaPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Pneu>();

            if (filtrosPesquisa.Codigo > 0)
                consultaPneu = consultaPneu.Where(o => o.Codigo == filtrosPesquisa.Codigo);

            if (filtrosPesquisa.CodigoAlmoxarifado > 0)
                consultaPneu = consultaPneu.Where(o => o.Almoxarifado.Codigo == filtrosPesquisa.CodigoAlmoxarifado);

            if (filtrosPesquisa.CodigoBandaRodagem > 0)
                consultaPneu = consultaPneu.Where(o => o.BandaRodagem.Codigo == filtrosPesquisa.CodigoBandaRodagem);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaPneu = consultaPneu.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoModelo > 0)
                consultaPneu = consultaPneu.Where(o => o.Modelo.Codigo == filtrosPesquisa.CodigoModelo);

            if (filtrosPesquisa.NumeroNota > 0)
                consultaPneu = consultaPneu.Where(o => o.DocumentoEntradaItem.DocumentoEntrada.Numero == filtrosPesquisa.NumeroNota);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DTO))
                consultaPneu = consultaPneu.Where(o => o.DTO.Contains(filtrosPesquisa.DTO));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroFogo))
                consultaPneu = consultaPneu.Where(o => o.NumeroFogo.Contains(filtrosPesquisa.NumeroFogo));

            if (filtrosPesquisa.DataEntradaInicio.HasValue)
                consultaPneu = consultaPneu.Where(o => o.DataEntrada >= filtrosPesquisa.DataEntradaInicio.Value.Date);

            if (filtrosPesquisa.DataEntradaLimite.HasValue)
                consultaPneu = consultaPneu.Where(o => o.DataEntrada <= filtrosPesquisa.DataEntradaLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaPneu = consultaPneu.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.TipoAquisicao.HasValue)
                consultaPneu = consultaPneu.Where(o => o.TipoAquisicao == filtrosPesquisa.TipoAquisicao.Value);

            if (filtrosPesquisa.SituacaoCadastroPneu.HasValue && filtrosPesquisa.SituacaoCadastroPneu.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroPneu.Incompleto)
                consultaPneu = consultaPneu.Where(o => o.Almoxarifado == null || o.BandaRodagem == null || o.Modelo == null || o.NumeroFogo == "" || o.NumeroFogo == null || o.DTO == "" || o.DTO == null);
            else if (filtrosPesquisa.SituacaoCadastroPneu.HasValue && filtrosPesquisa.SituacaoCadastroPneu.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroPneu.Completo)
                consultaPneu = consultaPneu.Where(o => o.Almoxarifado != null && o.BandaRodagem != null && o.Modelo != null && o.NumeroFogo != "" && o.NumeroFogo != null && o.DTO != "" && o.DTO != null);

            if (filtrosPesquisa.VidaAtual.HasValue)
                consultaPneu = consultaPneu.Where(o => o.VidaAtual == filtrosPesquisa.VidaAtual.Value);

            return consultaPneu;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.Pneu BuscarPorCodigo(int codigo)
        {
            var pneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Pneu>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return pneu;
        }

        public bool ContemPneuMesmoNumeroFogo(string numeroFogo, int codigo)
        {
            var infracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Pneu>()
                .Where(o => o.Codigo != codigo && o.NumeroFogo == numeroFogo);

            return infracao.Count() > 0;
        }

        public bool ContemPneuCadastradoPelaNotaEntrada(int codigoItemDocumentoEntrada)
        {
            var pneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Pneu>()
                .Where(o => o.DocumentoEntradaItem.Codigo == codigoItemDocumentoEntrada)
                .FirstOrDefault();

            return pneu != null;
        }

        public List<Dominio.Entidades.Embarcador.Frota.Pneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneu filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPneu = Consultar(filtrosPesquisa);

            return ObterLista(consultaPneu, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneu filtrosPesquisa)
        {
            var consultaPneu = Consultar(filtrosPesquisa);

            return consultaPneu.Count();
        }

        public int BuscarUltimoNumeroFogoLock(int codEmpresa) {

            int retorno = 0;

            string query = @"SELECT MAX(CAST(pnu_numero_fogo AS int)) ultimoNumeroFogo from t_tms_pneu WHERE pnu_numero_fogo is not null and pnu_numero_fogo not like '%[^0-9.]%'";

            var result = this.SessionNHiBernate.CreateSQLQuery(query).SetTimeout(120).UniqueResult();

            if (result == null)
                return 0;

            int.TryParse(result.ToString(), out retorno);

            return retorno;
        }

        public int BuscarUltimoNumeroFogo(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Pneu>();

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            int? ultimoNumero = query.OrderByDescending(o => o.Codigo).FirstOrDefault().NumeroFogo.ToNullableInt();

            return ultimoNumero.HasValue ? (ultimoNumero.Value) : 0;
        }

        public Dominio.Entidades.Embarcador.Frota.Pneu BuscarPorNumeroFogo(string numeroFogo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Pneu>();

            var result = from obj in query
                         where
                            obj.NumeroFogo == numeroFogo
                         select obj;

            return result.FirstOrDefault();
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuCustoEstoque> ConsultarRelatorioPneuCustoEstoque(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneuCustoEstoque filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPneuHistorico = new ConsultaPneuCustoEstoque().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPneuHistorico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.PneuCustoEstoque)));

            return consultaPneuHistorico.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuCustoEstoque>();
        }


        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuCustoEstoque>> ConsultarRelatorioPneuCustoEstoqueAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneuCustoEstoque filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPneuHistorico = new ConsultaPneuCustoEstoque().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPneuHistorico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.PneuCustoEstoque)));

            return await consultaPneuHistorico.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuCustoEstoque>();
        }

        public int ContarConsultaRelatorioPneuCustoEstoque(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneuCustoEstoque filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPneuHistorico = new ConsultaPneuCustoEstoque().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPneuHistorico.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Pneu> ConsultarRelatorioPneu(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneu filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPneu = new ConsultaPneu().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPneu.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Pneu)));

            return consultaPneu.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.Pneu>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Pneu>> ConsultarRelatorioPneuAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneu filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPneu = new ConsultaPneu().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPneu.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Pneu)));

            return await consultaPneu.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.Pneu>();
        }

        public int ContarConsultaRelatorioPneu(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneu filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPneuHistorico = new ConsultaPneu().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPneuHistorico.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuPorVeiculo> ConsultarRelatorioPneuPorVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneuPorVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaQuantidade = new ConsultaPneuPorVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaQuantidade.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.PneuPorVeiculo)));

            return consultaQuantidade.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuPorVeiculo>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuPorVeiculo>> ConsultarRelatorioPneuPorVeiculoAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneuPorVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaQuantidade = new ConsultaPneuPorVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaQuantidade.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.PneuPorVeiculo)));

            return await consultaQuantidade.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.PneuPorVeiculo>();
        }

        public int ContarConsultaRelatorioPneuPorVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneuPorVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaQuantidade = new ConsultaPneuPorVeiculo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaQuantidade.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
