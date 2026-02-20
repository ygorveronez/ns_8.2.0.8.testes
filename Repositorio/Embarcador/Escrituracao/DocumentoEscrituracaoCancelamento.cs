using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Escrituracao
{
    public class DocumentoEscrituracaoCancelamento : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>
    {
        #region Construtores

        public DocumentoEscrituracaoCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public DocumentoEscrituracaoCancelamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracaoCancelamento filtrosPesquisa)
        {
            string[] situacoes = new string[] { "C", "I" };

            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

            query = query.Where(obj => situacoes.Contains(obj.CTe.Status));

            if (filtrosPesquisa.CodigoLoteEscrituracaoCancelamento > 0)
                query = query.Where(obj => obj.LoteEscrituracaoCancelamento.Codigo == filtrosPesquisa.CodigoLoteEscrituracaoCancelamento);
            else
                query = query.Where(obj => obj.LoteEscrituracaoCancelamento == null && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumentoCancelamento.AgEscrituracao);

            if (filtrosPesquisa.CodigoFilial > 0)
                query = query.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTransportador > 0)
                query = query.Where(o => o.CTe.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoModeloDocumento > 0)
                query = query.Where(o => o.CTe.ModeloDocumentoFiscal.Codigo == filtrosPesquisa.CodigoModeloDocumento);

            if (filtrosPesquisa.CpfCnpjTomador > 0)
                query = query.Where(o => o.CTe.TomadorPagador.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador);

            if (filtrosPesquisa.DataInicio.HasValue)
                query = query.Where(obj =>  obj.CTe.DataCancelamento >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                query = query.Where(obj => obj.CTe.DataCancelamento < filtrosPesquisa.DataLimite.Value.AddDays(1).Date);

            if (filtrosPesquisa.SomentePagamentoLiberado)
                query = query.Where(o => !o.AguardandoAutorizacao);

            return query;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracaoCancelamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = ObterQueryConsulta(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                query = query.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

            if (parametrosConsulta.InicioRegistros > 0)
                query = query.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                query = query.Take(parametrosConsulta.LimiteRegistros);

            return query.Fetch(obj => obj.Carga)
                        .Fetch(obj => obj.CargaOcorrencia)
                        .Fetch(obj => obj.Filial)
                        .Fetch(obj => obj.FechamentoFrete)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.TomadorPagador)
                        .ThenFetch(obj => obj.Cliente)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Empresa)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Serie)
                        .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracaoCancelamento filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = ObterQueryConsulta(filtrosPesquisa);

            return query.Count();
        }

        public List<int> BuscarCodigosCTesPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CargaOcorrencia == null);

            return query.Select(obj => obj.CTe.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento BuscarPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

            query = query.Where(obj => obj.CTe.Codigo == codigoCTe);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento BuscarPorCTeECarga(int codigoCarga, int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

            query = query.Where(obj => obj.CTe.Codigo == codigoCTe && obj.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> BuscarPorLoteEscrituracaoCancelamento(int codigoLoteEscrituracaoCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

            query = query.Where(obj => obj.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento);

            return query.Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.CFOP)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Serie)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Empresa)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Remetente)
                        .ThenFetch(obj => obj.Cliente)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Remetente)
                        .ThenFetch(obj => obj.Cliente)
                        .ThenFetch(obj => obj.GrupoPessoas)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Remetente)
                        .ThenFetch(obj => obj.Atividade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Destinatario)
                        .ThenFetch(obj => obj.Cliente)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Destinatario)
                        .ThenFetch(obj => obj.Cliente)
                        .ThenFetch(obj => obj.GrupoPessoas)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.TomadorPagador)
                        .ThenFetch(obj => obj.Cliente)
                        .ThenFetch(obj => obj.Localidade)
                        .Fetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.TomadorPagador)
                        .ThenFetch(obj => obj.Cliente)
                        .ThenFetch(obj => obj.GrupoPessoas)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> ObterTipoOperacaoPadraoEscrituracao(int codigoLoteEscrituracaoCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento && o.Carga != null && o.Carga.TipoOperacao != null);

            return query.Select(o => o.Carga.TipoOperacao).Distinct().ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>> ObterTipoOperacaoPadraoEscrituracaoAsync(int codigoLoteEscrituracaoCancelamento, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento && o.Carga != null && o.Carga.TipoOperacao != null);

            return query.Select(o => o.Carga.TipoOperacao).Distinct().ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> ObterTipoOperacaoPadraoEscrituracaoOcorrencia(int codigoLoteEscrituracaoCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento && o.CargaOcorrencia != null && o.CargaOcorrencia.Carga.TipoOperacao != null && o.Carga == null );

            return query.Select(o => o.CargaOcorrencia.Carga.TipoOperacao).Distinct().ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>> ObterTipoOperacaoPadraoEscrituracaoOcorrenciaAsync(int codigoLoteEscrituracaoCancelamento, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento>();

            query = query.Where(o => o.LoteEscrituracaoCancelamento.Codigo == codigoLoteEscrituracaoCancelamento && o.CargaOcorrencia != null && o.CargaOcorrencia.Carga.TipoOperacao != null && o.Carga == null );

            return query.Select(o => o.CargaOcorrencia.Carga.TipoOperacao).Distinct().ToListAsync(cancellationToken);
        }

       

        #endregion
    }
}
