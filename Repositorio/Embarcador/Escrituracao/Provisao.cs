using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Escrituracao
{
    public class Provisao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.Provisao>
    {
        #region Construtores

        public Provisao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.Provisao> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaProvisao filtrosPesquisa)
        {
            var consultaProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Provisao>();

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaProvisao = consultaProvisao.Where(obj => obj.DataInicial.Value >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaProvisao = consultaProvisao.Where(obj => obj.DataFinal.Value <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaProvisao = consultaProvisao.Where(o => o.Transportadores.Any(t => t.Codigo == filtrosPesquisa.CodigoTransportador));

            if (filtrosPesquisa.CodigoCarga > 0)
                consultaProvisao = consultaProvisao.Where(o => o.DocumentosProvisao.Any(doc => doc.Carga.Codigo == filtrosPesquisa.CodigoCarga));

            if (filtrosPesquisa.NumeroDocumento > 0)
                consultaProvisao = consultaProvisao.Where(o => o.DocumentosProvisao.Any(doc => doc.NumeroDocumento == filtrosPesquisa.NumeroDocumento));

            if (filtrosPesquisa.CodigoOcorrencia > 0)
                consultaProvisao = consultaProvisao.Where(o => o.DocumentosProvisao.Any(doc => doc.CargaOcorrencia.Codigo == filtrosPesquisa.CodigoOcorrencia));

            if (filtrosPesquisa.CpfCnpjTomador > 0)
                consultaProvisao = consultaProvisao.Where(o => o.DocumentosProvisao.Any(doc => doc.Tomador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaProvisao = consultaProvisao.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.Numero > 0)
                consultaProvisao = consultaProvisao.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.SituacaoProvisao != SituacaoProvisao.Todos)
                consultaProvisao = consultaProvisao.Where(o => o.Situacao == filtrosPesquisa.SituacaoProvisao);

            if (filtrosPesquisa.TipoProvisao != TipoProvisao.Nenhum)
                consultaProvisao = consultaProvisao.Where(o => o.TipoProvisao == filtrosPesquisa.TipoProvisao);

            return consultaProvisao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escrituracao.Provisao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Provisao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.Provisao BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Provisao>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> BuscarTodosPorCargas(List<int> codigosCarga, List<string> fetch = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Provisao>()
                .Where(obj => codigosCarga.Contains(obj.Carga.Codigo));

            query = query.FetchMany(provisao => provisao.DocumentosProvisao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.Provisao BuscarSeExisteProvisaoEmFechamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Provisao>();
            var result = from obj in query where obj.Situacao == SituacaoProvisao.EmFechamento || obj.Situacao == SituacaoProvisao.PendenciaFechamento select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.Provisao BuscarNaoFinalizadaPorCarga(int codigoCarga)
        {
            var consultaProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Provisao>()
                .Where(provisao =>
                    provisao.Carga.Codigo == codigoCarga &&
                    provisao.Situacao != SituacaoProvisao.Cancelado
                );

            return consultaProvisao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> BuscarProvisaoEmFechamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Provisao>();
            var result = from obj in query where obj.Situacao == SituacaoProvisao.EmFechamento && obj.GerandoMovimentoFinanceiroProvisao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> BuscarProvisoesEmFechamentoPendentes(int registrosPorVez, int minutosCadaTentaivas)
        {
            var subQueryDocumentoContabil = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Provisao>()
                .Where(obj => obj.Situacao == SituacaoProvisao.EmFechamento && !obj.GerandoMovimentoFinanceiroProvisao && (obj.DataUltimoIntentoFechamento == null || obj.DataUltimoIntentoFechamento <= DateTime.Now.AddMinutes(-minutosCadaTentaivas)));

            return query.Take(registrosPorVez).ToList();
        }

        public int ObterProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Provisao>();

            var result = from obj in query select obj;

            int? retorno = result.Max(o => (int?)o.Numero);
            return retorno.HasValue ? (retorno.Value + 1) : 1;

        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaProvisao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaProvisao = Consultar(filtrosPesquisa);

            consultaProvisao = consultaProvisao.Fetch(obj => obj.Tomador);

            return ObterLista(consultaProvisao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaProvisao filtrosPesquisa)
        {
            var consultaProvisao = Consultar(filtrosPesquisa);

            return consultaProvisao.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> ObterProvisoesAgIntegracao(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaProvisao filtrosPesquisa, bool selecionarTodos, List<int> codigosOcorrencias)
        {
            var consultaProvisao = Consultar(filtrosPesquisa);

            if (selecionarTodos)
                consultaProvisao = consultaProvisao.Where(o => !codigosOcorrencias.Contains(o.Codigo));
            else
                consultaProvisao = consultaProvisao.Where(o => codigosOcorrencias.Contains(o.Codigo));

            return consultaProvisao.ToList();
        }

        #endregion
    }
}
