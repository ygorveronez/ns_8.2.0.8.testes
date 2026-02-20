using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using Dominio.ObjetosDeValor.Embarcador.Frete;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete;

namespace Repositorio.Embarcador.Frete
{
    public sealed class ApuracaoBonificacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao>
    {
        #region Construtores

        public ApuracaoBonificacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaApuracaoBonificacao filtrosPesquisa)
        {
            var consultaApuracaoBonificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao>();

            if (filtrosPesquisa.Ano > 0)
                consultaApuracaoBonificacao = consultaApuracaoBonificacao.Where(o => o.Ano == filtrosPesquisa.Ano);

            if (filtrosPesquisa.Mes.HasValue)
                consultaApuracaoBonificacao = consultaApuracaoBonificacao.Where(o => o.Mes == filtrosPesquisa.Mes);

            if (filtrosPesquisa.Numero > 0)
                consultaApuracaoBonificacao = consultaApuracaoBonificacao.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaApuracaoBonificacao = consultaApuracaoBonificacao.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            return consultaApuracaoBonificacao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao> BuscarPorAguardandoFinalizacao(int limiteRegistros)
        {
            var consultaApuracaoBonificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao>()
                .Where(o => o.Situacao == SituacaoApuracaoBonificacao.AguardandoGeracaoOcorrencia);

            return consultaApuracaoBonificacao.Take(limiteRegistros).ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao BuscarApuracaoDuplicada(int ano, Mes mes, List<int> codigodRegraApuracao)
        {
            var apuracoesBonificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao>()
                .Where(o =>
                    o.Ano == ano &&
                    o.Mes == mes &&
                    o.Situacao != SituacaoApuracaoBonificacao.Cancelado
                )
                .ToList();

            var consultaApuracaoBonificacao = apuracoesBonificacao
                .Where(o => codigodRegraApuracao.SequenceEqual(o.RegrasApuracao.Select(x => x.Codigo).ToList()));

            return consultaApuracaoBonificacao.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var consultaApuracaoBonificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao>();
            int? ultimoNumero = consultaApuracaoBonificacao.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public decimal BuscarTotalAcrescimoOuDesconto(int codigo, TipoAjusteValor tipoAjusteValor)
        {
            var consultaApuracaoBonificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento>();
            consultaApuracaoBonificacao = consultaApuracaoBonificacao.Where(o => o.ApuracaoBonificacao.Codigo == codigo);

            consultaApuracaoBonificacao = consultaApuracaoBonificacao.Where(o => o.RegraApuracao.Tipo == tipoAjusteValor);

            return consultaApuracaoBonificacao.Count() > 0 ? consultaApuracaoBonificacao.Sum(o => o.ValorOcorrencia) : 0;
        }

        public IList<ConsultaApuracaoBonificacao> BuscarFechamentoValorFrete(Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao apuracaoBonificacao)
        {
            int ultimoDiaDoMes = DateTime.DaysInMonth(apuracaoBonificacao.Ano, (int)apuracaoBonificacao.Mes);

            DateTime dataInicial = ObterDataApuracaoBonificao(1, (int)apuracaoBonificacao.Mes, apuracaoBonificacao.Ano);
            DateTime dataFinal = ObterDataApuracaoBonificao(ultimoDiaDoMes, (int)apuracaoBonificacao.Mes ,apuracaoBonificacao.Ano);

            string query = $@"SELECT
                                  Carga.EMP_CODIGO CodigoTransportador,
                                  Carga.CAR_CODIGO CodigoCarga,
                                  SUM(Carga.CAR_VALOR_FRETE) Valor
                                FROM 
                                  T_CARGA as Carga 
                                  left join T_EMPRESA Transportador on Carga.EMP_CODIGO = Transportador.EMP_CODIGO
                                WHERE 
                                  (
                                    (
                                      Carga.CAR_CARGA_FECHADA = 1 
                                      and Carga.CAR_CARGA_AGRUPADA = 0
                                    ) 
                                    or (
                                      Carga.CAR_CARGA_FECHADA = 0 
                                      and Carga.CAR_CODIGO_AGRUPAMENTO is not null
                                    )
                                  ) 
                                  and Carga.CAR_CARGA_DE_PRE_CARGA = 0 
                                  and CAST(Carga.CAR_DATA_CRIACAO AS DATE) >= '{dataInicial.ToString("yyyy-MM-dd")}' 
                                  and CAST(Carga.CAR_DATA_CRIACAO AS DATE) <  '{dataFinal.ToString("yyyy-MM-dd")}'
                                  and Carga.CAR_SITUACAO IN (9, 11, 15) 
                                  and Carga.CAR_VALOR_FRETE > 0
                                GROUP BY 
                                  Carga.EMP_CODIGO,
                                  Carga.CAR_CODIGO
                                ORDER BY
                                  Carga.EMP_CODIGO;";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ConsultaApuracaoBonificacao)));

            return nhQuery.List<ConsultaApuracaoBonificacao>();
        }

        public IList<ConsultaApuracaoBonificacao> BuscarFechamentoValorOcorrencia(Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao apuracaoBonificacao)
        {
            int ultimoDiaDoMes = DateTime.DaysInMonth(apuracaoBonificacao.Ano, (int)apuracaoBonificacao.Mes);

            DateTime dataInicial = ObterDataApuracaoBonificao(1, (int)apuracaoBonificacao.Mes, apuracaoBonificacao.Ano);
            DateTime dataFinal = ObterDataApuracaoBonificao(ultimoDiaDoMes, (int)apuracaoBonificacao.Mes, apuracaoBonificacao.Ano);

            string query = $@"SELECT 
                                  Carga.CAR_CODIGO CodigoCarga,
                                  Carga.EMP_CODIGO CodigoTransportador,
                                  SUM(Ocorrencia.COC_VALOR_OCORRENCIA) Valor
                                FROM 
                                  T_CARGA_OCORRENCIA as Ocorrencia 
                                  left join T_CARGA Carga on Carga.CAR_CODIGO = Ocorrencia.CAR_CODIGO 
                                  left join T_OCORRENCIA TipoOcorrencia on TipoOcorrencia.OCO_CODIGO = Ocorrencia.OCO_CODIGO 
                                  left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO 
                                WHERE 
                                  Ocorrencia.COC_INATIVA = 0 
                                  AND CAST( Ocorrencia.COC_DATA_OCORRENCIA AS DATE ) >= '{dataInicial.ToString("yyyy-MM-dd")}' 
                                  AND CAST( Ocorrencia.COC_DATA_OCORRENCIA AS DATE ) <= '{dataFinal.ToString("yyyy-MM-dd")}' 
                                  AND Ocorrencia.COC_SITUACAO_OCORRENCIA in (3) 
                                  AND Carga.CAR_CODIGO is not null 
                                GROUP BY 
                                  Carga.CAR_CODIGO,
                                  Carga.EMP_CODIGO
                                ORDER BY
                                  Carga.EMP_CODIGO;";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ConsultaApuracaoBonificacao)));

            return nhQuery.List<ConsultaApuracaoBonificacao>();
        }

        public DateTime ObterDataApuracaoBonificao(int dia, int mes, int ano)
        {
            return new DateTime(ano, mes, dia);
        }

        public List<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaApuracaoBonificacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaApuracaoBonificacao = Consultar(filtrosPesquisa);

            return ObterLista(consultaApuracaoBonificacao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaApuracaoBonificacao filtrosPesquisa)
        {
            var consultaApuracaoBonificacao = Consultar(filtrosPesquisa);

            return consultaApuracaoBonificacao.Count();
        }

        #endregion
    }
}