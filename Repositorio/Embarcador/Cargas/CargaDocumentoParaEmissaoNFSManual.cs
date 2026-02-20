using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaDocumentoParaEmissaoNFSManual : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>
    {
        #region Construtores

        public CargaDocumentoParaEmissaoNFSManual(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarSelecaoNFSManual(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o =>
                    filtrosPesquisa.SituacoesCargasPermitidas.Contains(o.Carga.SituacaoCarga) &&
                    (o.CargaOcorrencia == null || (o.CargaOcorrencia != null && filtrosPesquisa.SituacoesOcorrenciasPermitidas.Contains(o.CargaOcorrencia.SituacaoOcorrencia))) &&
                    (o.FechamentoFrete == null || o.FechamentoFrete.Situacao != SituacaoFechamentoFrete.Cancelado)
                );

            if (filtrosPesquisa.CodigoDestinatario > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.Destinatario.CPF_CNPJ == filtrosPesquisa.CodigoDestinatario);

            if (filtrosPesquisa.CodigoLancamentoNFSManual > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.LancamentoNFSManual.Codigo == filtrosPesquisa.CodigoLancamentoNFSManual);
            else
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.LancamentoNFSManual == null);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.CargaOrigem.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador);

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                if (!filtrosPesquisa.RetornarFiliaisTransportador)
                    consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.CargaOrigem.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);
                else
                    consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.CargaOrigem.Empresa.Codigo == filtrosPesquisa.CodigoTransportador || o.CargaOrigem.Empresa.Matriz.Any(mtz => mtz.Codigo == filtrosPesquisa.CodigoTransportador));
            }


            if (filtrosPesquisa.Codigosfilial?.Count > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => filtrosPesquisa.Codigosfilial.Contains(o.CargaOrigem.Filial.Codigo));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.CargaOrigem.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.CargaOrigem.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CpfCnpjTomador > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.Tomador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.DataEmissao >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.DataEmissao < filtrosPesquisa.DataFinal.Value.AddDays(1).Date);

            if (filtrosPesquisa.NumeroInicial > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.Numero <= filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.Moeda.HasValue && filtrosPesquisa.CodigoLancamentoNFSManual <= 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.Moeda == filtrosPesquisa.Moeda);

            if (filtrosPesquisa.Residuais)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.DocResidual == true);
            else
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.DocResidual != true);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.NumeroPedidoCliente == filtrosPesquisa.NumeroPedidoCliente);

            if (filtrosPesquisa.ComplementoOcorrencia == NFSManualTipoComplemento.ApenasDocumentosOriginais)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.CargaOcorrencia == null);
            else if (filtrosPesquisa.ComplementoOcorrencia == NFSManualTipoComplemento.ApenasComplementosOcorrencia)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.CargaOcorrencia != null);

            if (filtrosPesquisa.CodigosDocumentos?.Count > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => filtrosPesquisa.CodigosDocumentos.Contains(o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo));

            return consultaCargaDocumentoParaEmissaoNFSManual;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarSelecaoNFSManualExterna(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, int numeroInicial, int numeroFinal, DateTime? dataInicio, DateTime? dataFim)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o =>
                    o.LancamentoNFSManual == null &&
                    o.Tomador.CPF_CNPJ == lancamentoNFSManual.Tomador.CPF_CNPJ &&
                    o.Carga.Empresa.Codigo == lancamentoNFSManual.Transportador.Codigo &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    (o.CargaOcorrencia == null || (o.CargaOcorrencia != null && o.CargaOcorrencia.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada))
                );

            if (lancamentoNFSManual.Filial != null)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.Carga.Filial.Codigo == lancamentoNFSManual.Filial.Codigo);

            if (lancamentoNFSManual.FechamentoFrete != null)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.FechamentoFrete != null);
            else
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.FechamentoFrete == null);

            if (!lancamentoNFSManual.DadosNFS.Moeda.HasValue || lancamentoNFSManual.DadosNFS.Moeda == MoedaCotacaoBancoCentral.Real)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.Moeda == null || o.Moeda == MoedaCotacaoBancoCentral.Real);
            else
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.Moeda == lancamentoNFSManual.DadosNFS.Moeda);

            if (dataInicio.HasValue)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.DataEmissao >= dataInicio.Value.Date);

            if (dataFim.HasValue)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.DataEmissao <= dataFim.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (numeroInicial > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Where(o => o.Numero <= numeroFinal);

            return consultaCargaDocumentoParaEmissaoNFSManual;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> AplicarFetchCargaOrigem(IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query)
        {
            return query
                    .Fetch(obj => obj.CargaOrigem).ThenFetch(obj => obj.Filial)
                    .Fetch(obj => obj.CargaOrigem).ThenFetch(obj => obj.Empresa);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> AplicarFetchPedidoXMLNotaFiscal(IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query)
        {
            return query
                    .Fetch(obj => obj.PedidoXMLNotaFiscal).ThenFetch(obj => obj.XMLNotaFiscal).ThenFetch(obj => obj.Canhoto);

        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> AplicarFetchOutros(IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query)
        {
            return query
                    .Fetch(obj => obj.FechamentoFrete)
                    .Fetch(obj => obj.Tomador)
                    .Fetch(obj => obj.LocalidadePrestacao)
                    .Fetch(obj => obj.Destinatario)
                    .Fetch(obj => obj.CTe).ThenFetch(o => o.CentroResultadoFaturamento);
        }

        #endregion

        #region Métodos Públicos

        public int ContarNFSNaoEmitidosPorCarga(int codigoCarga)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.CTe == null && o.FechamentoFrete == null);

            return consultaCargaDocumentoParaEmissaoNFSManual.Count();
        }

        public int ContarNFSNaoEmitidosPorFechamentoFrete(int codigoFechamentoFrete)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.FechamentoFrete.Codigo == codigoFechamentoFrete && o.CTe == null);

            return consultaCargaDocumentoParaEmissaoNFSManual.Count();
        }

        public int ContarPorPedidoXMLNotasFiscais(List<int> pedidosXmlNotasFiscais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where pedidosXmlNotasFiscais.Contains(obj.PedidoXMLNotaFiscal.Codigo) && !obj.DocResidual select obj;
            return result.Count();
        }

        public int ContarPorPedidoXMLNotasFiscaisResiduais(List<int> pedidosXmlNotasFiscais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where pedidosXmlNotasFiscais.Contains(obj.PedidoXMLNotaFiscal.Codigo) && obj.DocResidual select obj;
            return result.Count();
        }

        public bool ExisteNFSDeOcorrenciaPorCTe(int cte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.CTe.Codigo == cte && obj.CargaOcorrencia != null select obj;
            return result.Any();
        }

        public int ContarPorCargaVinculadasNFSManual(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.LancamentoNFSManual != null && obj.FechamentoFrete == null select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasPorLancamento(int lancamento)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.LancamentoNFSManual.Codigo == lancamento && o.FechamentoFrete == null);

            return consultaCargaDocumentoParaEmissaoNFSManual
                .Select(o => o.Carga)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete> BuscarFechamentosFretePorLancamento(int lancamento)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.LancamentoNFSManual.Codigo == lancamento && o.FechamentoFrete != null);

            return consultaCargaDocumentoParaEmissaoNFSManual
                .Select(o => o.FechamentoFrete)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasPorListaLancamento(List<int> lancamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where lancamentos.Contains(obj.LancamentoNFSManual.Codigo) select obj.Carga;
            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarDocumentosNFSManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.DadosNFS.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.FechamentoFrete == null);

            return consultaCargaDocumentoParaEmissaoNFSManual.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarDocPorCargaPedido(int codigoCargaPedido)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return consultaCargaDocumentoParaEmissaoNFSManual.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorCargaENumeroNota(int codigoCarga, List<int> numerosNota)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.FechamentoFrete == null && numerosNota.Contains(o.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero));

            return consultaCargaDocumentoParaEmissaoNFSManual.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorCargaENotaFiscal(int codigoCarga, List<int> notasFiscais)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.FechamentoFrete == null && notasFiscais.Contains(o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo));

            return consultaCargaDocumentoParaEmissaoNFSManual.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual BuscarPorCargaCTeEChaveNF(int codigoCarga, string chaveNF)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.Carga.Codigo == codigoCarga && chaveNF == o.Chave && o.CargaOcorrencia != null); ;

            return consultaCargaDocumentoParaEmissaoNFSManual.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorCargasOrigem(List<int> codigosCarga)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => codigosCarga.Contains(o.CargaOrigem.Codigo) && o.FechamentoFrete == null);

            return consultaCargaDocumentoParaEmissaoNFSManual.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarPorFechamentoFrete(int codigoFechamentoFrete)
        {
            return ConsultarPorFechamentoFrete(codigoFechamentoFrete, parametrosConsulta: null);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarPorFechamentoFrete(int codigoFechamentoFrete, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.FechamentoFrete.Codigo == codigoFechamentoFrete);

            consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual.Fetch(o => o.LancamentoNFSManual);

            return ObterLista(consultaCargaDocumentoParaEmissaoNFSManual, parametrosConsulta);
        }

        public bool ExistePorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(obj => obj.Carga.Codigo == carga && obj.FechamentoFrete == null);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual BuscarPorPedidoXMLNotaFiscal(int codigoPedidoXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.PedidoXMLNotaFiscal.Codigo == codigoPedidoXMLNotaFiscal);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual BuscarPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido);

            return query.FirstOrDefault();
        }

        public decimal BuscarValorPrestacaoServicoJaRateadasPorLancamentoNFsManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && obj.RateouValorFrete && obj.DocumentosNFSe == null select obj;
            return result.Sum(obj => (decimal?)obj.ValorPrestacaoServico) ?? 0m;
        }

        public decimal BuscarValorRetencaoISSJaRateadasPorLancamentoNFsManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && obj.RateouValorFrete && obj.DocumentosNFSe == null select obj;
            return result.Sum(obj => (decimal?)obj.ValorRetencaoISS) ?? 0m;
        }

        public decimal BuscarBaseCalculoISSJaRateadasPorLancamentoNFsManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && obj.RateouValorFrete && obj.DocumentosNFSe == null select obj;
            return result.Sum(obj => (decimal?)obj.BaseCalculoISS) ?? 0m;
        }

        public decimal BuscarBaseCalculoIBSCBSJaRateadasPorLancamentoNFsManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && obj.RateouValorFrete && obj.DocumentosNFSe == null select obj;
            return result.Sum(obj => (decimal?)obj.BaseCalculoIBSCBS) ?? 0m;
        }

        public decimal BuscarValorCBSJaRateadasPorLancamentoNFsManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && obj.RateouValorFrete && obj.DocumentosNFSe == null select obj;
            return result.Sum(obj => (decimal?)obj.ValorCBS) ?? 0m;
        }

        public decimal BuscarValorIBSEstadualJaRateadasPorLancamentoNFsManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && obj.RateouValorFrete && obj.DocumentosNFSe == null select obj;
            return result.Sum(obj => (decimal?)obj.ValorIBSEstadual) ?? 0m;
        }

        public decimal BuscarValorIBSMunicipalJaRateadasPorLancamentoNFsManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && obj.RateouValorFrete && obj.DocumentosNFSe == null select obj;
            return result.Sum(obj => (decimal?)obj.ValorIBSMunicipal) ?? 0m;
        }

        public decimal BuscarValorISSServicoJaRateadasPorLancamentoNFsManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && obj.RateouValorFrete && obj.DocumentosNFSe == null select obj;
            return result.Sum(obj => (decimal?)obj.ValorISS) ?? 0m;
        }

        public List<int> BuscarNaoRateadasPorLancamentoNFsManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && !obj.RateouValorFrete && obj.DocumentosNFSe == null select obj.Codigo;
            return result.ToList();
        }

        public List<int> BuscarRateadasPorLancamentoNFsManual(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && obj.RateouValorFrete && obj.DocumentosNFSe == null select obj.Codigo;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarListaDocumentosPorNumeroPedidoDescontoFrete(string numeroPedidoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.NumeroPedidoCliente == numeroPedidoCliente select obj;
            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorLancamentoNFsManual(int codigo)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.LancamentoNFSManual.Codigo == codigo);

            return consultaCargaDocumentoParaEmissaoNFSManual
                .Fetch(obj => obj.PedidoCTeParaSubContratacao)
                .ThenFetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(o => o.PedidoCTeParaSubContratacao)
                .Fetch(o => o.PedidoXMLNotaFiscal)
                .ThenFetch(o => o.CargaPedido)
                .ThenFetch(o => o.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(o => o.PedidoXMLNotaFiscal)
                .ThenFetch(o => o.CargaPedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorLancamentosNFsManual(List<int> codigos)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => codigos.Contains(o.LancamentoNFSManual.Codigo));

            return consultaCargaDocumentoParaEmissaoNFSManual
                .Fetch(obj => obj.Carga)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorLancamentoNFsManualSemDocumentos(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.LancamentoNFSManual.Codigo == codigo && obj.DocumentosNFSe == null select obj;
            return result
                .Fetch(obj => obj.PedidoCTeParaSubContratacao)
                .ThenFetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.CargaPedido)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.PedidoXMLNotaFiscal)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public void SetarCTesPorLancamentoManual(int lancamento, int cte)
        {
            string hql = "update CargaDocumentoParaEmissaoNFSManual docNFS set docNFS.CTe = :CTe where docNFS.LancamentoNFSManual= :LancamentoNFSManual and docNFS.DocumentosNFSe is null ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("CTe", cte);
            query.SetInt32("LancamentoNFSManual", lancamento);
            query.ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual BuscarPorCodigoComFetch(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.Codigo == codigo);

            return query.Fetch(o => o.CTe).ThenFetch(o => o.CentroResultadoFaturamento).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarSelecaoNFSManual(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = ConsultarSelecaoNFSManual(filtrosPesquisa);

            var cargaDocumentoParaEmissaoNFSManualPrimeiroFetch = AplicarFetchCargaOrigem(consultaCargaDocumentoParaEmissaoNFSManual).ToList();
            var cargaDocumentoParaEmissaoNFSManualSegundoFetch = AplicarFetchPedidoXMLNotaFiscal(consultaCargaDocumentoParaEmissaoNFSManual).ToList();
            var cargaDocumentoParaEmissaoNFSManualTerceiroFetch = AplicarFetchOutros(consultaCargaDocumentoParaEmissaoNFSManual).ToList();

            foreach (var cd1 in cargaDocumentoParaEmissaoNFSManualPrimeiroFetch)
            {
                var cd2 = cargaDocumentoParaEmissaoNFSManualSegundoFetch.First(cd => cd.Codigo == cd1.Codigo);
                var cd3 = cargaDocumentoParaEmissaoNFSManualTerceiroFetch.First(cd => cd.Codigo == cd1.Codigo);

                //Fetch CD2
                cd1.PedidoXMLNotaFiscal = cd2.PedidoXMLNotaFiscal;

                //Fetch CD3
                cd1.FechamentoFrete = cd3.FechamentoFrete;
                cd1.Tomador = cd3.Tomador;
                cd1.LocalidadePrestacao = cd3.LocalidadePrestacao;
                cd1.Destinatario = cd3.Destinatario;

                if (cd1.CTe != null)
                    cd1.CTe.CentroResultadoFaturamento = cd3.CTe.CentroResultadoFaturamento;
            }


            return ObterLista(consultaCargaDocumentoParaEmissaoNFSManual, parametrosConsulta);
        }

        public int ContarConsultaSelecaoNFSManual(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = ConsultarSelecaoNFSManual(filtrosPesquisa);

            return consultaCargaDocumentoParaEmissaoNFSManual.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarSelecaoNFSManualExterna(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, int numeroInicial, int numeroFinal, DateTime? dataInicio, DateTime? dataFim, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = ConsultarSelecaoNFSManualExterna(lancamentoNFSManual, numeroInicial, numeroFinal, dataInicio, dataFim);

            consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.FechamentoFrete)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.LocalidadePrestacao)
                .Fetch(obj => obj.Destinatario);

            return ObterLista(consultaCargaDocumentoParaEmissaoNFSManual, parametrosConsulta);
        }

        public int ContarConsultaSelecaoNFSManualExterna(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, int numeroInicial, int numeroFinal, DateTime? dataInicio, DateTime? dataFim)
        {
            var result = ConsultarSelecaoNFSManualExterna(lancamentoNFSManual, numeroInicial, numeroFinal, dataInicio, dataFim);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarDocumentosDiferentesPertencentesAsNotas(List<int> notas, List<int> documentosSelecionados)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            var result = from obj in query
                         where
                         notas.Contains(obj.DocumentosNFSe.NFSe.Codigo) &&
                         !documentosSelecionados.Contains(obj.Codigo) &&
                         obj.LancamentoNFSManual == null
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarDocumentoParaEmissaoNFSManualPorFechamentoFrete(int codigoFechamentoFrete, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.FechamentoFrete.Codigo == codigoFechamentoFrete);

            consultaCargaDocumentoParaEmissaoNFSManual = consultaCargaDocumentoParaEmissaoNFSManual
                .Fetch(o => o.Remetente)
                .Fetch(o => o.Destinatario).ThenFetch(o => o.Localidade)
                .Fetch(o => o.CTe)
                .Fetch(o => o.ModeloDocumentoFiscal);

            return ObterLista(consultaCargaDocumentoParaEmissaoNFSManual, parametrosConsulta);
        }



        public int ContarConsultaDocumentoParaEmissaoNFSManualPorFechamentoFrete(int codigoFechamentoFrete)
        {
            var consultaCargaDocumentoParaEmissaoNFSManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.FechamentoFrete.Codigo == codigoFechamentoFrete);

            return consultaCargaDocumentoParaEmissaoNFSManual.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ObterConsultaDocumentoParaEmissaoNFSManual(int carga, int numeroDocumento, double cpfCnpjDestinatario, bool apenasSemOcorrencias, bool buscarPorCargaAgrupada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            var result = from obj in query where obj.FechamentoFrete == null && (obj.CargaOcorrencia == null || obj.CargaOcorrencia.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada) select obj;

            if (buscarPorCargaAgrupada)
                result = result.Where(obj => obj.Carga.Codigo == carga);
            else
                result = result.Where(obj => obj.CargaOrigem.Codigo == carga);

            if (numeroDocumento > 0)
                result = result.Where(o => o.Numero == numeroDocumento);

            if (apenasSemOcorrencias)
            {
                result = result.Where(o => o.CargaOcorrencia == null);
            }

            if (cpfCnpjDestinatario > 0)
            {
                result = result.Where(o => o.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarDocumentoParaEmissaoNFSManual(int carga, int numeroDocumento, double cpfCnpjDestinatario, bool apenasSemOcorrencias, bool buscarPorCargaAgrupada, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = ObterConsultaDocumentoParaEmissaoNFSManual(carga, numeroDocumento, cpfCnpjDestinatario, apenasSemOcorrencias, buscarPorCargaAgrupada);

            return query.Fetch(o => o.Remetente)
                         .Fetch(o => o.Destinatario).ThenFetch(o => o.Localidade)
                         .Fetch(o => o.CTe)
                         .Fetch(o => o.ModeloDocumentoFiscal)
                         .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarDocumentoParaEmissaoNFSManual(int carga, int numeroDocumento, double cpfCnpjDestinatario, bool apenasSemOcorrencias, bool buscarPorCargaAgrupada)
        {
            var query = ObterConsultaDocumentoParaEmissaoNFSManual(carga, numeroDocumento, cpfCnpjDestinatario, apenasSemOcorrencias, buscarPorCargaAgrupada);

            return query.Count();
        }

        public bool ExistemEmitidosPorOcorrencia(int codigoCargaOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.CargaOcorrencia.Codigo == codigoCargaOcorrencia && o.LancamentoNFSManual != null);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ConsultarPorOcorrencia(int codigoCargaOcorrencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.CargaOcorrencia.Codigo == codigoCargaOcorrencia);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                query = query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query.ToList();
        }

        public int ContarConsultaPorOcorrencia(int codigoCargaOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.CargaOcorrencia.Codigo == codigoCargaOcorrencia);

            return query.Count();
        }

        public int ContarConsultaPorLancamento(int codigoLancamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.LancamentoNFSManual.Codigo == codigoLancamento);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual BuscarPorOcorrenciaECargaCTe(int codigoCargaOcorrencia, int cargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.CargaOcorrencia.Codigo == codigoCargaOcorrencia && o.CargaCTe.Codigo == cargaCTe);

            return query.FirstOrDefault();
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.NFSManual.Documento> BuscarParaImpressaoRelacao(int codigoLancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);

            return query.OrderBy(o => o.Numero).Select(o => new Dominio.Relatorios.Embarcador.DataSource.NFSManual.Documento()
            {
                Carga = o.Carga.CodigoCargaEmbarcador,
                CPFCNPJDestinatario = o.Destinatario.CPF_CNPJ,
                DataEmissao = o.DataEmissao,
                Destinatario = o.Destinatario.Nome,
                Localidade = o.LocalidadePrestacao.Descricao + " - " + o.LocalidadePrestacao.Estado.Sigla,
                Numero = o.Numero,
                TipoPessoaDestinatario = o.Destinatario.Tipo,
                Valor = o.ValorFrete
            }).ToList();
        }

        public bool ExisteGeradoPorCargaCTe(int codigoCargaCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.LancamentoNFSManual != null && o.CargaCTe.Codigo == codigoCargaCTe);

            return query.Any();
        }

        public bool ExisteGeradoPorPedidoXMLNotaFiscal(int codigoCargaCTe)//int[] codigosPedidoXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> queryNotas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            queryNotas = queryNotas.Where(c => c.CargaCTe.Codigo == codigoCargaCTe);

            query = query.Where(o => o.LancamentoNFSManual != null && queryNotas.Any(c => c.PedidoXMLNotaFiscal.Codigo == o.PedidoXMLNotaFiscal.Codigo));

            //query = query.Where(o => o.LancamentoNFSManual != null && codigosPedidoXMLNotaFiscal.Contains(o.PedidoXMLNotaFiscal.Codigo));

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorCargaCTe(int codigoCargaCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            query = query.Where(o => o.CargaCTe.Codigo == codigoCargaCTe);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorPedidoXMLNotaFiscalPorCarga(int codigoCargaCTe)//int[] codigosPedidoXMLNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> queryNotas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();
            queryNotas = queryNotas.Where(c => c.CargaCTe.Codigo == codigoCargaCTe);

            query = query.Where(o => o.LancamentoNFSManual != null && queryNotas.Any(c => c.PedidoXMLNotaFiscal.Codigo == o.PedidoXMLNotaFiscal.Codigo));

            //query = query.Where(o => codigosPedidoXMLNotaFiscal.Contains(o.PedidoXMLNotaFiscal.Codigo));

            return query.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaDocumentoEmissaoNFSManual> ConsultarRelatorioDocumentoEmissaoNFSManual(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery consulta = new ConsultaCargaDocumentoEmissaoNFSManual().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaDocumentoEmissaoNFSManual)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaDocumentoEmissaoNFSManual>();
        }

        public int ContarConsultaRelatorioDocumentoEmissaoNFSManual(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery consulta = new ConsultaCargaDocumentoEmissaoNFSManual().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarPorNfseSemDocumentos(int quantidadeDias)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            var result = query.Where(o => o.LancamentoNFSManual == null && (o.DataEnvioUltimoAlertaNFsePendente == null || o.DataEnvioUltimoAlertaNFsePendente.Value.Date <= DateTime.Now.Date.AddDays(-quantidadeDias)));

            return result.ToList();
        }

        public bool ExisteDocumentoPendente(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaExistenciaDocumentoNFSManual filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>()
                .Where(o => o.LancamentoNFSManual == null
                    && (o.Carga != null && o.Carga.Codigo == filtroPesquisa.CodigoCarga)
                    && o.Chave == filtroPesquisa.Chave
                    && o.Numero == filtroPesquisa.Numero
                    && o.Serie == filtroPesquisa.Serie
                    && o.ValorFrete == filtroPesquisa.ValorFrete
                    && o.DataEmissao == filtroPesquisa.DataEmissao);

            if (filtroPesquisa.CodigoXMLNotaFiscal > 0)
                query = query.Where(o => o.PedidoXMLNotaFiscal != null && o.PedidoXMLNotaFiscal.XMLNotaFiscal != null && o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == filtroPesquisa.CodigoXMLNotaFiscal);
            else
                query = query.Where(o => o.PedidoXMLNotaFiscal == null);

            if (filtroPesquisa.CodigoCTe > 0)
                query = query.Where(o => o.CTe != null && o.CTe.Codigo == filtroPesquisa.CodigoCTe);
            else
                query = query.Where(o => o.CTe == null);

            if (filtroPesquisa.CodigoModeloDocumentoFiscal > 0)
                query = query.Where(o => o.ModeloDocumentoFiscal != null && o.ModeloDocumentoFiscal.Codigo == filtroPesquisa.CodigoModeloDocumentoFiscal);
            else
                query = query.Where(o => o.ModeloDocumentoFiscal == null);

            if (filtroPesquisa.CodigoPedidoCTeParaSubContratacao > 0)
                query = query.Where(o => o.PedidoCTeParaSubContratacao != null && o.PedidoCTeParaSubContratacao.Codigo == filtroPesquisa.CodigoPedidoCTeParaSubContratacao);
            else
                query = query.Where(o => o.PedidoCTeParaSubContratacao == null);

            if (filtroPesquisa.CodigoDocumentoNFSe > 0)
                query = query.Where(o => o.DocumentosNFSe != null && o.DocumentosNFSe.Codigo == filtroPesquisa.CodigoDocumentoNFSe);
            else
                query = query.Where(o => o.DocumentosNFSe == null);

            if (filtroPesquisa.CodigoCargaOcorrencia > 0)
                query = query.Where(o => o.CargaOcorrencia != null && o.CargaOcorrencia.Codigo == filtroPesquisa.CodigoCargaOcorrencia);
            else
                query = query.Where(o => o.CargaOcorrencia == null);

            return query.Any();
        }

        #endregion
    }
}
