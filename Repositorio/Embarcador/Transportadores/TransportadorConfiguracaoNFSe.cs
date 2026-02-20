using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Transportadores
{
    public class TransportadorConfiguracaoNFSe : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>
    {
        public TransportadorConfiguracaoNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TransportadorConfiguracaoNFSe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarPorCodigoEmpresa(int CodigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();
            var resut = from obj in query where obj.Empresa.Codigo == CodigoEmpresa select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> BuscarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();
            var resut = from obj in query select obj;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> BuscarTodosAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();

            var result = from obj in query select obj;

            return result
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.SerieNFSe)
                .Fetch(obj => obj.ServicoNFSe)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.NaturezaNFSe)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        //utilizado para se ter uma previa do ISS sem saber o transportador que fará o transporte.
        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarPorLocalidadeEmpresaELocalidadeEmprestacao(int localidadeEmpresa, int localidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();
            var result = from obj in query where obj.Empresa.Localidade.Codigo == localidadeEmpresa select obj;

            if (localidade > 0)
                result = result.Where(obj => obj.LocalidadePrestacao.Codigo == localidade);
            else
                result = result.Where(obj => obj.LocalidadePrestacao == null);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarPorEmpresaELocalidade(int codigoEmpresa, int localidade, string ufTomador, int grupoTomador, int localidadeTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (localidade > 0)
                result = result.Where(obj => obj.LocalidadePrestacao.Codigo == localidade);
            else
                result = result.Where(obj => obj.LocalidadePrestacao == null);

            if (!string.IsNullOrWhiteSpace(ufTomador))
                result = result.Where(obj => obj.UFTomador.Sigla == ufTomador);

            if (grupoTomador > 0)
                result = result.Where(obj => obj.GrupoTomador.Codigo == grupoTomador);

            if (localidadeTomador > 0)
                result = result.Where(obj => obj.LocalidadeTomador.Codigo == localidadeTomador);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarPorLocalidadeEmpresa(int localidadeEmpresa, int localidade, string ufTomador, int grupoTomador, int localidadeTomador, int codigoTipoOperacao, double clienteTomador, int codigoTipoOcorrencia, IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> query)
        {
            var result = (from obj in query where obj.Empresa != null && obj.Empresa.Localidade != null && obj.Empresa.Localidade.Codigo == localidadeEmpresa select obj).ToList();

            if (localidade > 0)
                result = (from obj in result where obj.LocalidadePrestacao != null && obj.LocalidadePrestacao.Codigo == localidade select obj).ToList();
            else
                result = (from obj in result where obj.LocalidadePrestacao == null select obj).ToList();

            if (!string.IsNullOrWhiteSpace(ufTomador))
                result = (from obj in result where obj.UFTomador != null && obj.UFTomador.Sigla == ufTomador select obj).ToList();

            if (grupoTomador > 0)
                result = (from obj in result where obj.GrupoTomador != null && obj.GrupoTomador.Codigo == grupoTomador select obj).ToList();

            if (localidadeTomador > 0)
                result = (from obj in result where obj.LocalidadeTomador != null && obj.LocalidadeTomador.Codigo == localidadeTomador select obj).ToList();

            if (codigoTipoOperacao > 0)
                result = (from obj in result where obj.TipoOperacao != null && obj.TipoOperacao.Codigo == codigoTipoOperacao select obj).ToList();
            else
                result = (from obj in result where obj.TipoOperacao == null select obj).ToList();

            if (codigoTipoOcorrencia > 0)
                result = (from obj in result where obj.TipoOcorrencia != null && obj.TipoOcorrencia.Codigo == codigoTipoOcorrencia select obj).ToList();
            else
                result = (from obj in result where obj.TipoOcorrencia == null select obj).ToList();

            if (clienteTomador > 0)
                result = (from obj in result where obj.ClienteTomador != null && obj.ClienteTomador.CPF_CNPJ == clienteTomador select obj).ToList();
            else
                result = (from obj in result where obj.ClienteTomador == null select obj).ToList();

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarPorEmpresaELocalidade(int codigoEmpresa, int localidade, string ufTomador, int grupoTomador, int localidadeTomador, int codigoTipoOperacao, double clienteTomador, int codigoTipoOcorrencia, IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> query)
        {
            var result = (from obj in query where obj.Empresa != null && obj.Empresa.Codigo == codigoEmpresa select obj).ToList();

            if (localidade > 0)
                result = (from obj in result where obj.LocalidadePrestacao != null && obj.LocalidadePrestacao.Codigo == localidade select obj).ToList();
            else
                result = (from obj in result where obj.LocalidadePrestacao == null select obj).ToList();

            if (!string.IsNullOrWhiteSpace(ufTomador))
                result = (from obj in result where obj.UFTomador != null && obj.UFTomador.Sigla == ufTomador select obj).ToList();

            if (grupoTomador > 0)
                result = (from obj in result where obj.GrupoTomador != null && obj.GrupoTomador.Codigo == grupoTomador select obj).ToList();

            if (localidadeTomador > 0)
                result = (from obj in result where obj.LocalidadeTomador != null && obj.LocalidadeTomador.Codigo == localidadeTomador select obj).ToList();

            if (codigoTipoOperacao > 0)
                result = (from obj in result where obj.TipoOperacao != null && obj.TipoOperacao.Codigo == codigoTipoOperacao select obj).ToList();
            else
                result = (from obj in result where obj.TipoOperacao == null select obj).ToList();

            if (codigoTipoOcorrencia > 0)
                result = (from obj in result where obj.TipoOcorrencia != null && obj.TipoOcorrencia.Codigo == codigoTipoOcorrencia select obj).ToList();
            else
                result = (from obj in result where obj.TipoOcorrencia == null select obj).ToList();

            if (clienteTomador > 0)
                result = (from obj in result where obj.ClienteTomador != null && obj.ClienteTomador.CPF_CNPJ == clienteTomador select obj).ToList();
            else
                result = (from obj in result where obj.ClienteTomador == null select obj).ToList();

            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarPorEmpresaLocalidadeServico(int localidade, int codigoEmpresa, int codigoservico, IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> query)
        {
            var result = (from obj in query where obj.Empresa != null && obj.Empresa.Codigo == codigoEmpresa select obj).ToList();

            if (localidade > 0)
                result = (from obj in result where obj.LocalidadePrestacao != null && obj.LocalidadePrestacao.Codigo == localidade select obj).ToList();
            else
                result = (from obj in result where obj.LocalidadePrestacao == null select obj).ToList();

            if (codigoservico > 0)
                result = (from obj in result where obj.ServicoNFSe != null && obj.ServicoNFSe.Codigo == codigoservico select obj).ToList();


            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaConfiguracaoNFSe filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();

            var result = from obj in query where obj.Empresa.Codigo == filtrosPesquisa.Empresa && obj.SerieNFSe != null select obj;

            if ((filtrosPesquisa?.LocalidadePrestacao ?? 0) > 0)
                result = result.Where(obj => obj.LocalidadePrestacao.Codigo == filtrosPesquisa.LocalidadePrestacao);

            if ((filtrosPesquisa?.Servico ?? 0) > 0)
                result = result.Where(obj => obj.ServicoNFSe.Codigo == filtrosPesquisa.Servico);

            if ((filtrosPesquisa?.TipoOperacao ?? 0) > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.TipoOperacao);

            if (!string.IsNullOrEmpty(filtrosPesquisa?.NBS))
                result = result.Where(obj => obj.NBS == filtrosPesquisa.NBS);

            var consulta = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();

            if (inicioRegistros > 0)
            {
                consulta = consulta.Skip(inicioRegistros).ToList();
            }

            if (maximoRegistros > 0)
            {
                consulta = consulta.Take(maximoRegistros).ToList();
            }
            return consulta;
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaConfiguracaoNFSe filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();

            var result = from obj in query where obj.Empresa.Codigo == filtrosPesquisa.Empresa && obj.SerieNFSe != null select obj;

            if ((filtrosPesquisa?.LocalidadePrestacao ?? 0) > 0)
                result = result.Where(obj => obj.LocalidadePrestacao.Codigo == filtrosPesquisa.LocalidadePrestacao);

            if ((filtrosPesquisa?.Servico ?? 0) > 0)
                result = result.Where(obj => obj.ServicoNFSe.Codigo == filtrosPesquisa.Servico);

            if ((filtrosPesquisa?.TipoOperacao ?? 0) > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.TipoOperacao);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe BuscarParaAtualizarNaImportacao(
                                        int codigoEmpresa,
                                        int localidade,
                                         string ufTomador,
                                         int servicoNFSe,
                                         string serieRPS,
                                         int naturezaNFSe,
                                         int tipoOperacao,
                                         bool incluirISSBaseCalculo,
                                         int serieNFSe
                                        )
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();

            var result = (from obj in query where obj.Empresa != null && obj.Empresa.Codigo == codigoEmpresa select obj).ToList();

            if (localidade > 0)
                result = (from obj in result where obj.LocalidadePrestacao != null && obj.LocalidadePrestacao.Codigo == localidade select obj).ToList();
            else
                result = (from obj in result where obj.LocalidadePrestacao == null select obj).ToList();

            if (!string.IsNullOrWhiteSpace(ufTomador))
                result = (from obj in result where obj.UFTomador != null && obj.UFTomador.Sigla == ufTomador select obj).ToList();

            if (servicoNFSe > 0)
                result = (from obj in result where obj.ServicoNFSe != null && obj.ServicoNFSe.Codigo == servicoNFSe select obj).ToList();
            else
                result = (from obj in result where obj.ServicoNFSe == null select obj).ToList();

            if (!string.IsNullOrWhiteSpace(serieRPS))
                result = (from obj in result where obj.SerieRPS != null select obj).ToList();

            if (naturezaNFSe > 0)
                result = (from obj in result where obj.NaturezaNFSe != null && obj.NaturezaNFSe.Codigo == naturezaNFSe select obj).ToList();
            else
                result = (from obj in result where obj.NaturezaNFSe == null select obj).ToList();

            if (tipoOperacao > 0)
                result = (from obj in result where obj.TipoOperacao != null && obj.TipoOperacao.Codigo == tipoOperacao select obj).ToList();
            else
                result = (from obj in result where obj.TipoOperacao == null select obj).ToList();

            if (serieNFSe > 0)
                result = (from obj in result where obj.SerieNFSe != null && obj.SerieNFSe.Codigo == serieNFSe select obj).ToList();
            else
                result = (from obj in result where obj.SerieNFSe == null select obj).ToList();

            return result.FirstOrDefault();
        }

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ConfiguracoesNFSe> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaConfiguracoesNFSe().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Transportadores.ConfiguracoesNFSe)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ConfiguracoesNFSe>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ConfiguracoesNFSe>> ConsultarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaConfiguracoesNFSe().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Transportadores.ConfiguracoesNFSe)));

            return await consulta.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ConfiguracoesNFSe>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioConfiguracoesNFSe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaConfiguracoesNFSe().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public bool ExisteRealizarArredondamentoCalculoIss(int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>().Where(x => x.Empresa.Codigo == codigoEmpresa);
            return query.Select(x => (bool?)x.RealizarArredondamentoCalculoIss).FirstOrDefault() ?? false;
        }

        #endregion
    }
}