using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Filiais
{
    public class SequenciaGestaoPatio : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>
    {
        #region Construtores

        public SequenciaGestaoPatio(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public SequenciaGestaoPatio(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Privados

        public IQueryable<Dominio.ObjetosDeValor.Embarcador.Filial.SequenciaGestaoPatio> Consultar(int codigoFilial, int codigoTipoOperacao)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => o.TipoOperacao != null);

            if (codigoFilial > 0)
                consultaSequenciaGestaoPatio = consultaSequenciaGestaoPatio.Where(o => o.Filial.Codigo == codigoFilial);

            if (codigoTipoOperacao > 0)
                consultaSequenciaGestaoPatio = consultaSequenciaGestaoPatio.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return consultaSequenciaGestaoPatio
                .GroupBy(o => new
                {
                    CodigoFilial = o.Filial.Codigo,
                    CodigoTipoOperacao = o.TipoOperacao.Codigo,
                    Filial = o.Filial.Descricao,
                    TipoOperacao = o.TipoOperacao.Descricao
                })
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Filial.SequenciaGestaoPatio()
                {
                    CodigoFilial = o.Key.CodigoFilial,
                    CodigoTipoOperacao = o.Key.CodigoTipoOperacao,
                    Filial = o.Key.Filial,
                    TipoOperacao = o.Key.TipoOperacao
                });
        }

        private string ObterSqlConsultaSequenciaGestaoPatioGuarita(List<int> codigosGuarita)
        {
            string sql = @$"SELECT guarita.CJC_CODIGO as CodigoGuarita,
                                   SequenciaGestaoPatio.SGP_GUARITA_ENTRADA_EXIBIR_HORARIO_EXATO as GuaritaEntradaExibirHorarioExato,
                                   SequenciaGestaoPatio.SGP_GUARITA_ENTRADA_INFORMAR_CHEGADA_VEICULO as ChegadaVeiculo,
                                   SequenciaGestaoPatio.SGP_GUARITA_ENTRADA_PERMITE_DENEGAR_CHEGADA as GuaritaEntradaPermiteDenegarChegada,
                                   SequenciaGestaoPatio.SGP_GUARITA_ENTRADA_PERMITE_INFORMAR_DADOS_DEVOLUCAO as GuaritaEntradaPermiteInformarDadosDevolucao
                              FROM T_CARGA_JANELA_CARREGAMENTO_GUARITA guarita
                              JOIN T_FLUXO_GESTAO_PATIO fluxo ON guarita.FGP_CODIGO = fluxo.FGP_CODIGO
                         LEFT JOIN T_CARGA carga ON fluxo.CAR_CODIGO = carga.CAR_CODIGO
                         LEFT JOIN T_PRE_CARGA preCarga ON preCarga.PCA_CODIGO = fluxo.PCA_CODIGO
                         LEFT JOIN T_TIPO_OPERACAO TipoOperacaoCarga ON carga.TOP_CODIGO = TipoOperacaoCarga.TOP_CODIGO
                         LEFT JOIN T_TIPO_OPERACAO TipoOperacaoPreCarga ON carga.TOP_CODIGO = TipoOperacaoPreCarga.TOP_CODIGO
                              JOIN T_SEQUENCIA_GESTAO_PATIO SequenciaGestaoPatio ON fluxo.FGE_TIPO = SequenciaGestaoPatio.SGP_TIPO AND fluxo.FIL_CODIGO = SequenciaGestaoPatio.FIL_CODIGO
                               AND SequenciaGestaoPatio.TOP_CODIGO = (CASE 
			                                                          WHEN fluxo.CAR_CODIGO IS NOT NULL THEN TipoOperacaoCarga.TOP_CODIGO
			                                                          WHEN fluxo.PCA_CODIGO IS NOT NULL THEN TipoOperacaoPreCarga.TOP_CODIGO
		                                                              END)
                             WHERE guarita.CJC_CODIGO in ({string.Join(",", codigosGuarita)})"; // SQL-INJECTION-SAFE
            return sql;
        }
        #endregion Métodos Privados

        #region Métodos Públicos

        public List<int> BuscarCodigosFiliaisPorTipo(TipoFluxoGestaoPatio tipo)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => o.Tipo == tipo);

            return consultaSequenciaGestaoPatio.Select(o => o.Filial.Codigo).Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio BuscarPorFilialETipoSemTipoOperacao(int codigoFilial, TipoFluxoGestaoPatio tipo)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => o.Filial.Codigo == codigoFilial && o.Tipo == tipo && o.TipoOperacao == null);

            return consultaSequenciaGestaoPatio.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio BuscarPorFilialTipoETipoOperacao(int codigoFilial, int codigoTipoOperacao, TipoFluxoGestaoPatio tipo)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => o.Filial.Codigo == codigoFilial && o.Tipo == tipo && o.TipoOperacao.Codigo == codigoTipoOperacao);

            return consultaSequenciaGestaoPatio.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> BuscarTodosPorFilial(int codigoFilial)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => o.Filial.Codigo == codigoFilial);

            return consultaSequenciaGestaoPatio.ToList();
        }

        public bool ExibirImprimirTicketBalanca(int codigoFilial)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => o.Filial.Codigo == codigoFilial);

            return consultaSequenciaGestaoPatio.Any(o => o.ImprimirTicketBalanca);
        }

        public List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> BuscarTodosPorFilialETipo(int codigoFilial, TipoFluxoGestaoPatio tipo)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => o.Filial.Codigo == codigoFilial && o.Tipo == tipo);

            return consultaSequenciaGestaoPatio.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> BuscarTodosPorFiliais(List<int> codigosFilial)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => codigosFilial.Contains(o.Filial.Codigo));

            return consultaSequenciaGestaoPatio.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> BuscarTodosPorFiliaisETipo(List<int> codigosFilial, TipoFluxoGestaoPatio? tipo)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => codigosFilial.Contains(o.Filial.Codigo));

            if (tipo.HasValue)
                consultaSequenciaGestaoPatio = consultaSequenciaGestaoPatio.Where(o => o.Tipo == tipo.Value);

            return consultaSequenciaGestaoPatio.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Filial.SequenciaGestaoPatio> Consultar(int codigoFilial, int codigoTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSequenciaGestaoPatio = Consultar(codigoFilial, codigoTipoOperacao);

            return ObterLista(consultaSequenciaGestaoPatio, parametrosConsulta);
        }

        public int ContarConsulta(int codigoFilial, int codigoTipoOperacao)
        {
            var consultaSequenciaGestaoPatio = Consultar(codigoFilial, codigoTipoOperacao);

            return consultaSequenciaGestaoPatio.Count();
        }

        public bool ExistePorFilialETipo(int codigoFilial, TipoFluxoGestaoPatio tipo)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => o.Filial.Codigo == codigoFilial && o.Tipo == tipo);

            return consultaSequenciaGestaoPatio.Count() > 0;
        }

        public bool ExistePorFilialETipoOperacao(int codigoFilial, int codigoTipoOperacao)
        {
            var consultaSequenciaGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>()
                .Where(o => o.Filial.Codigo == codigoFilial && o.TipoOperacao.Codigo == codigoTipoOperacao);

            return consultaSequenciaGestaoPatio.Count() > 0;
        }

        public Task<IList<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SequenciaGestaoPatioGuarita>> ObterSequenciaGestaoPatioGuaritaAsync(List<int> codigosGuarita)
        {
            string sql = ObterSqlConsultaSequenciaGestaoPatioGuarita(codigosGuarita);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SequenciaGestaoPatioGuarita)));

            return consulta.SetTimeout(600).ListAsync<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.SequenciaGestaoPatioGuarita>(CancellationToken);
        }

        #endregion Métodos Públicos
    }
}
