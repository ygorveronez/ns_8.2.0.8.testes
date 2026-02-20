using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoEnvioProgramado : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>
    {

        #region Construtores

        public IntegracaoEnvioProgramado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoEnvioProgramado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #endregion

        #region Métodos Públicos

        public bool ExistePorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Any();
        }

        public bool ExistePorCargaOcorrencia(int codigoCargaOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

            query = query.Where(obj => obj.CargaOcorrencia.Codigo == codigoCargaOcorrencia && obj.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Any();
        }

        public bool ExistePorCTe(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

            query = query.Where(obj => obj.CTe.Codigo == codigoCTe && obj.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Any();
        }

        public bool ExisteAutorizadoPorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == tipoIntegracao && o.SituacaoIntegracao == SituacaoIntegracao.Integrado);

            return query.Any();
        }

        public List<TipoEntidadeIntegracao> BuscarOrigensIntegracaoDisponiveis()
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

            IQueryable<TipoEntidadeIntegracao> result = from obj in query select obj.TipoEntidadeIntegracao;

            return result.Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado BuscarPorCodigoArquivo(int codigoArquivo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado BuscarPorCodigoCargaOuOcorrencia(string codigoCargaEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>()
                .Where(x => x.SituacaoIntegracao == SituacaoIntegracao.AgRetorno)
                .Where(x => x.Carga.CodigoCargaEmbarcador == codigoCargaEmbarcador || x.CargaOcorrencia.Carga.CodigoCargaEmbarcador == codigoCargaEmbarcador);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado BuscarPorCodigoCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>()
                .Where(x => x.SituacaoIntegracao == SituacaoIntegracao.AgRetorno && x.CTe.Codigo == codigoCTe);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> BuscarIntegracoesPendentes(TipoEntidadeIntegracao tipoEntidadeIntegracao, int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

            query = query.Where(obj =>
                ((obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && !obj.EnvioBloqueado && (obj.DataIntegracao <= DateTime.Now))
                || (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)))
                && obj.TipoEntidadeIntegracao == tipoEntidadeIntegracao
                && (obj.CargaOcorrencia == null || obj.CargaOcorrencia.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada)
            );

            if (tipoEntidadeIntegracao == TipoEntidadeIntegracao.Carga || tipoEntidadeIntegracao == TipoEntidadeIntegracao.CTe)
                query = query.Where(obj => !obj.Carga.GerandoIntegracoes && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            if (tipoEntidadeIntegracao == TipoEntidadeIntegracao.CTe)
                query = query.Fetch(x => x.Carga).ThenFetch(x => x.TabelaFrete);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(0)
                .Take(numeroRegistrosPorVez)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> BuscarIntegracoesPendentesPorTipoDocumento(TipoEntidadeIntegracao tipoEntidadeIntegracao, int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, Dominio.Enumeradores.TipoDocumento modeloDocumentoFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

            query = query.Where(obj =>
                ((obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && !obj.EnvioBloqueado && (obj.DataIntegracao <= DateTime.Now))
                || (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)))
                && obj.TipoEntidadeIntegracao == tipoEntidadeIntegracao);

            if (tipoEntidadeIntegracao == TipoEntidadeIntegracao.Carga || tipoEntidadeIntegracao == TipoEntidadeIntegracao.CTe)
                query = query.Where(obj => !obj.Carga.GerandoIntegracoes && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            if (tipoEntidadeIntegracao == TipoEntidadeIntegracao.CTe)
                query = query.Fetch(x => x.Carga).ThenFetch(x => x.TabelaFrete);

            if (modeloDocumentoFiscal == Dominio.Enumeradores.TipoDocumento.NFSe)
                query = query.Where(x => x.TipoDocumento == Dominio.Enumeradores.TipoDocumento.NFSe);
            else if (modeloDocumentoFiscal == Dominio.Enumeradores.TipoDocumento.Outros)
                query = query.Where(x => x.TipoDocumento == Dominio.Enumeradores.TipoDocumento.Outros);
            else
                query = query.Where(x => x.TipoDocumento != Dominio.Enumeradores.TipoDocumento.NFSe && x.TipoDocumento != Dominio.Enumeradores.TipoDocumento.Outros);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(0)
                .Take(numeroRegistrosPorVez)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> BuscarIntegracoesComFalha(DateTime dataInicial, DateTime dataFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

            query = query.Where(obj => obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.DataIntegracao >= dataInicial && obj.DataIntegracao <= dataFinal);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoEnvioProgramado filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> consulta = Consultar(filtroPesquisa);
            return ObterLista(consulta, parametroConsulta);
        }

        public int ContarConsultar(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoEnvioProgramado filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> consulta = Consultar(filtroPesquisa);
            return consulta.Count();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoEnvioProgramado filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

            consulta = consulta.Where(obj => 
                (((SituacaoCarga?)obj.Carga.SituacaoCarga ?? obj.CargaOcorrencia.Carga.SituacaoCarga) != SituacaoCarga.Cancelada)
                && (obj.CargaOcorrencia == null || obj.CargaOcorrencia.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada)
            );

            if (!string.IsNullOrEmpty(filtroPesquisa.NumeroCarga))
                consulta = consulta.Where(obj => obj.Carga.CodigoCargaEmbarcador == filtroPesquisa.NumeroCarga);

            if (filtroPesquisa.NumeroCTE > 0)
                consulta = consulta.Where(obj => obj.Carga.CargaCTes.Any(o => o.CTe.Numero == filtroPesquisa.NumeroCTE));

            if (filtroPesquisa.NumeroOcorrencia > 0)
                consulta = consulta.Where(obj => obj.CargaOcorrencia.NumeroOcorrencia == filtroPesquisa.NumeroOcorrencia);

            if (filtroPesquisa.SituacaoIntegracao.HasValue)
                consulta = consulta.Where(obj => obj.SituacaoIntegracao == filtroPesquisa.SituacaoIntegracao.Value);

            if (filtroPesquisa.TipoEntidadeIntegracao.HasValue)
                consulta = consulta.Where(obj => obj.TipoEntidadeIntegracao == filtroPesquisa.TipoEntidadeIntegracao.Value);

            if (filtroPesquisa.DataIntegracaoInicial != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.DataIntegracao >= filtroPesquisa.DataIntegracaoInicial);

            if (filtroPesquisa.DataIntegracaoFinal != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.DataIntegracao <= filtroPesquisa.DataIntegracaoFinal);

            if (filtroPesquisa.DataProgramadaInicial != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.DataEnvioProgramada >= filtroPesquisa.DataProgramadaInicial);

            if (filtroPesquisa.DataCriacaoCargaInicial != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.Carga.DataCriacaoCarga >= filtroPesquisa.DataCriacaoCargaInicial);

            if (filtroPesquisa.DataCriacaoCargaFinal != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.Carga.DataCriacaoCarga <= filtroPesquisa.DataCriacaoCargaFinal);

            if (filtroPesquisa.DataEmissaoCTEInicial != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.Carga.CargaCTes.Any(o => o.CTe.DataEmissao.Value >= filtroPesquisa.DataEmissaoCTEInicial));

            if (filtroPesquisa.DataEmissaoCTEFinal != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.Carga.CargaCTes.Any(o => o.CTe.DataEmissao.Value <= filtroPesquisa.DataEmissaoCTEFinal));

            return consulta;
        }

        #endregion Métodos Privados
    }
}
