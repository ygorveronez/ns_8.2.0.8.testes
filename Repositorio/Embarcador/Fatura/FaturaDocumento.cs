using Dominio.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>
    {
        public FaturaDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<int> BuscarNumeroFaturaPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Documento.CTe.Codigo == codigoCTe && o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            return query.Select(o => o.Fatura.Numero).Distinct().ToList();
        }

        public int? BuscarPrimeiroNumeroFaturaPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Documento.CTe.Codigo == codigoCTe && o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            return query.Select(o => (int?)o.Fatura.Numero).FirstOrDefault();
        }

        public List<int> BuscarNumeroFaturaPorCTe(int[] codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => codigosCTes.Contains(o.Documento.CTe.Codigo) && o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            return query.Select(o => o.Fatura.Numero).Distinct().ToList();
        }

        public List<int> BuscarNumeroFaturaPorCargaCTe(int codigoCarga, int? codigoFaturaDesconsiderar = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.Carga.Codigo == codigoCarga);

            queryFaturaDocumento = queryFaturaDocumento.Where(o => queryCargaCTe.Select(cargaCTe => cargaCTe.CTe.Codigo).Contains(o.Documento.CTe.Codigo) &&
                                            o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            if (codigoFaturaDesconsiderar.HasValue)
            {
                queryFaturaDocumento = queryFaturaDocumento.Where(o => o.Fatura.Codigo != codigoFaturaDesconsiderar.Value);
            }

            return queryFaturaDocumento.Select(o => o.Fatura.Numero).Distinct().ToList();
        }

        public List<int> BuscarNumeroFaturaPorDocumento(List<int> codigosDocumentos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && codigosDocumentos.Contains(o.Carga.Codigo));

            queryFaturaDocumento = queryFaturaDocumento.Where(o => queryCargaCTe.Select(cargaCTe => cargaCTe.CTe.Codigo).Contains(o.Documento.CTe.Codigo) &&
                                            o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            return queryFaturaDocumento.Select(o => o.Fatura.Numero).Distinct().ToList();
        }

        public int? BuscarPrimeiroNumeroFaturaPorCargaCTe(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.Carga.Codigo == codigoCarga);

            queryFaturaDocumento = queryFaturaDocumento.Where(o => queryCargaCTe.Any(cargaCTe => cargaCTe.CTe == o.Documento.CTe) &&
                                                                   o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            return queryFaturaDocumento.Select(o => (int?)o.Fatura.Numero).FirstOrDefault();
        }

        public List<int> BuscarNumeroFaturaPorOcorrencia(int codigoCargaOcorrencia, int? codigoFaturaDesconsiderar = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoCargaOcorrencia);

            queryFaturaDocumento = queryFaturaDocumento.Where(o => queryCargaCTe.Any(cargaCTe => cargaCTe.CTe == o.Documento.CTe) &&
                                                                   o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            if (codigoFaturaDesconsiderar.HasValue)
            {
                queryFaturaDocumento = queryFaturaDocumento.Where(o => o.Fatura.Codigo != codigoFaturaDesconsiderar.Value);
            }

            return queryFaturaDocumento.Select(o => o.Fatura.Numero).Distinct().ToList();
        }

        public int? BuscarPrimeiroNumeroFaturaPorOcorrencia(int codigoCargaOcorrencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoCargaOcorrencia);

            queryFaturaDocumento = queryFaturaDocumento.Where(o => queryCargaCTe.Any(cargaCTe => cargaCTe.CTe == o.Documento.CTe) &&
                                                                   o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            return queryFaturaDocumento.Select(o => (int?)o.Fatura.Numero).FirstOrDefault();
        }

        public List<int> BuscarNumeroFaturaPorCarga(int codigoCarga, int? codigoFaturaDesconsiderar = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Documento.Carga.Codigo == codigoCarga && o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            if (codigoFaturaDesconsiderar.HasValue)
            {
                query = query.Where(o => o.Fatura.Codigo != codigoFaturaDesconsiderar.Value);
            }

            return query.Select(o => o.Fatura.Numero).Distinct().ToList();
        }

        public bool ExisteNaFatura(int codigoFatura, int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.Codigo == codigoDocumento);

            return query.Any();
        }

        public decimal ObterValorTotalEmFatura(int codigoDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Documento.Codigo == codigoDocumento && o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            return query.Sum(o => (decimal?)o.ValorTotalACobrar) ?? 0m;
        }

        public bool ExisteNaFaturaPorPreFaturaNatura(int codigoFatura, int codigoPreFaturaNatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> queryDocumentoFaturamento = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            IQueryable<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura> queryItemPreFaturaNatura = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura>();

            queryItemPreFaturaNatura = queryItemPreFaturaNatura.Where(o => o.PreFatura.Codigo == codigoPreFaturaNatura);
            queryDocumentoFaturamento = queryDocumentoFaturamento.Where(o => queryItemPreFaturaNatura.Select(i => i.CargaCTe.CTe.Codigo).Contains(o.CTe.Codigo));

            queryFaturaDocumento = queryFaturaDocumento.Where(o => o.Fatura.Codigo == codigoFatura && queryDocumentoFaturamento.Select(d => d.Codigo).Contains(o.Documento.Codigo));

            return queryFaturaDocumento.Any();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaDocumento BuscarPorCodigo(int codigoFaturaDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Codigo == codigoFaturaDocumento);

            return query.Fetch(o => o.Documento)
                .ThenFetch(o => o.CTe)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> BuscarParaContabilizarPorFatura(int codigoFatura)
        {
            var consultaFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>()
                .Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.CTe != null);

            return consultaFaturaDocumento
                .Fetch(o => o.Documento)
                .ThenFetch(o => o.CTe)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaDocumento BuscarFaturaPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Documento.CTe.Codigo == codigoCTe && o.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> Consultar(int codigoFatura, int numero, int serie, int codigoEmpresa, int codigoCarga, string propOrdenar, string dirOrdena, int inicio, int limite, long codigoTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            if (numero > 0)
                query = query.Where(o => o.Documento.Numero == numero.ToString());

            if (serie > 0)
                query = query.Where(o => o.Documento.EmpresaSerie.Numero == serie);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Documento.Empresa.Codigo == codigoEmpresa);

            if (codigoTomador > 0)
                query = query.Where(o => o.Documento.Tomador.CPF_CNPJ == codigoTomador);

            if (codigoCarga > 0)
                query = query.Where(o => o.Documento.CargaPagamento.Codigo == codigoCarga);

            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoFatura, int numero, int serie, int codigoEmpresa, int codigoCarga, long codigoTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            if (numero > 0)
                query = query.Where(o => o.Documento.Numero == numero.ToString());

            if (serie > 0)
                query = query.Where(o => o.Documento.EmpresaSerie.Numero == serie);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Documento.Empresa.Codigo == codigoEmpresa);

            if (codigoCarga > 0)
                query = query.Where(o => o.Documento.CargaPagamento.Codigo == codigoCarga);

            if (codigoTomador > 0)
                query = query.Where(o => o.Documento.Tomador.CPF_CNPJ == codigoTomador);


            return query.Count();
        }

        public decimal BuscarTotalDaPrestacao(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Sum(o => (decimal?)o.Documento.CTe.ValorPrestacaoServico) ?? 0m;
        }

        public decimal BuscarTotalDaPrestacaoComCST(int codigoFatura, List<string> CSTs, bool comCSTs, bool modeloCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            if (modeloCTe)
            {
                query = query.Where(o => o.Documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == TipoDocumento.CTe);
                if (comCSTs)
                    query = query.Where(o => CSTs.Contains(o.Documento.CTe.CST));
                else
                    query = query.Where(o => !CSTs.Contains(o.Documento.CTe.CST));
            }
            else
            {
                query = query.Where(o => o.Documento.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe);
            }

            return query.Sum(o => (decimal?)o.Documento.CTe.ValorPrestacaoServico) ?? 0m;
        }

        public decimal BuscarTotalReceber(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Sum(o => (decimal?)o.Documento.CTe.ValorAReceber) ?? 0m;
        }

        public List<int> BuscarCodigosCTesCSTs(int codigoFatura, List<string> CSTs, bool comCSTs, bool modeloCTe)
        {
            var query = _BuscarCodigosCTesCSTs(codigoFatura, CSTs, comCSTs, modeloCTe);

            return query.ToList();
        }

        public bool PossuiDadosPorCodigosCTesCSTs(int codigoFatura, List<string> CSTs, bool comCSTs, bool modeloCTe)
        {
            var query = _BuscarCodigosCTesCSTs(codigoFatura, CSTs, comCSTs, modeloCTe);

            return query.Count() > 0;
        }

        public IQueryable<int> _BuscarCodigosCTesCSTs(int codigoFatura, List<string> CSTs, bool comCSTs, bool modeloCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            if (modeloCTe)
            {
                query = query.Where(o => o.Documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == TipoDocumento.CTe);
                if (comCSTs)
                    query = query.Where(o => CSTs.Contains(o.Documento.CTe.CST));
                else
                    query = query.Where(o => !CSTs.Contains(o.Documento.CTe.CST));
            }
            else
            {
                query = query.Where(o => o.Documento.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe);
            }

            return query.Select(o => o.Documento.CTe.Codigo).Distinct();
        }

        public decimal BuscarTotalReceberCSTs(int codigoFatura, List<string> CSTs, bool comCSTs, bool modeloCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            if (modeloCTe)
            {
                query = query.Where(o => o.Documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == TipoDocumento.CTe);
                if (comCSTs)
                    query = query.Where(o => CSTs.Contains(o.Documento.CTe.CST));
                else
                    query = query.Where(o => !CSTs.Contains(o.Documento.CTe.CST));
            }
            else
            {
                query = query.Where(o => o.Documento.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe);
            }

            return query.Sum(o => (decimal?)o.Documento.CTe.ValorAReceber) ?? 0m;
        }

        public decimal BuscarTotalBaseCalculoICMSComCSTs(int codigoFatura, List<string> CSTs, bool comCSTs, bool modeloCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();


            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            if (modeloCTe)
            {
                query = query.Where(o => o.Documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == TipoDocumento.CTe);
                if (comCSTs)
                    query = query.Where(o => CSTs.Contains(o.Documento.CTe.CST));
                else
                    query = query.Where(o => !CSTs.Contains(o.Documento.CTe.CST));

                return query.Sum(o => (decimal?)o.Documento.CTe.BaseCalculoICMS) ?? 0m;
            }
            else
            {
                query = query.Where(o => o.Documento.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe);

                return query.Sum(o => (decimal?)o.Documento.CTe.BaseCalculoISS) ?? 0m;
            }


        }

        public decimal BuscarTotalBaseCalculoICMS(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Sum(o => (decimal?)o.Documento.CTe.BaseCalculoICMS) ?? 0m;
        }

        public decimal BuscarTotalBaseCalculoISS(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Sum(o => (decimal?)o.Documento.CTe.BaseCalculoISS) ?? 0m;
        }

        public decimal BuscarTotalICMS(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.CTe.CST != "60");

            return query.Sum(o => (decimal?)o.Documento.CTe.ValorICMS) ?? 0m;
        }

        public decimal BuscarTotalICMSSST(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.CTe.CST == "60");

            return query.Sum(o => (decimal?)o.Documento.CTe.ValorICMS) ?? 0m;
        }

        public decimal BuscarTotalISS(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe);

            return query.Sum(o => (decimal?)o.Documento.CTe.ValorISS) ?? 0m;
        }

        public decimal BuscarTotalISSRetido(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe);

            return query.Sum(o => (decimal?)o.Documento.CTe.ValorISSRetido) ?? 0m;
        }

        public decimal ObterTotalDesconto(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            decimal descontosDosDocumentos = query.Sum(o => (decimal?)o.ValorDesconto) ?? 0m;
            decimal valorTotalDeDocumentosDebito = query.AsEnumerable()
                .Where(o => o.Documento.ModeloDocumentoFiscal != null
                    && o.Documento.ModeloDocumentoFiscal.DescontarValorDesseDocumentoFatura
                    && o.Documento.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito
                        == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Debito)
                .Sum(o => (decimal?)o.ValorTotalACobrar) ?? 0m;

            return descontosDosDocumentos + valorTotalDeDocumentosDebito * -1;
        }

        public decimal ObterTotalAcrescimo(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            decimal acrescimosDosDocumentos = query.Sum(o => (decimal?)o.ValorAcrescimo) ?? 0m;
            decimal valorTotalDeDocuentosCredito = query.AsEnumerable()
                .Where(o => o.Documento.ModeloDocumentoFiscal != null
                    && o.Documento.ModeloDocumentoFiscal.DescontarValorDesseDocumentoFatura
                    && o.Documento.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito
                        == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Credito)
                .Sum(o => (decimal?)o.ValorTotalACobrar) ?? 0m;



            return acrescimosDosDocumentos + valorTotalDeDocuentosCredito;
        }

        public decimal ObterTotalACobrarLiquido(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura).Fetch(o => o.Documento).ThenFetch(o => o.ModeloDocumentoFiscal);

            return query.Sum(o => (decimal?)o.ValorACobrar) ?? 0m;
        }

        public TipoAmbiente BuscarTipoAmbienteFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = query.Select(o => o.Documento).FirstOrDefault();

            return documentoFaturamento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga ? documentoFaturamento.Carga.CargaCTes.FirstOrDefault().CTe.TipoAmbiente : documentoFaturamento.CTe.TipoAmbiente;
        }

        public decimal ObterTotalACobrar(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Sum(o => (decimal?)o.ValorTotalACobrar) ?? 0m;
        }

        public int ContarPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Count();
        }

        public Dominio.Entidades.Empresa ObterPrimeiraEmpresaEmissora(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Where(o => o.Documento.Carga.Empresa != null).Select(o => o.Documento.Carga.Empresa).FirstOrDefault() ??
                   query.Where(o => o.Documento.CTe.Empresa != null).Select(o => o.Documento.CTe.Empresa).FirstOrDefault();
        }

        public Dominio.Entidades.Cliente ObterPrimeiroTomador(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Where(o => o.Documento.Tomador != null).Select(o => o.Documento.Tomador).FirstOrDefault();
        }

        public DateTime? BuscarPrimeiraDataEmissao(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Select(o => o.Documento.CTe.DataEmissao).FirstOrDefault() ?? query.Select(o => o.Documento.Carga.CargaCTes.Select(c => c.CTe.DataEmissao).FirstOrDefault()).FirstOrDefault();
        }

        public Dominio.Entidades.ModeloDocumentoFiscal BuscarPrimeiroModeloDocumento(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => query.Any(c => o.CTe == c.Documento.CTe));

            return queryCargaCTe.Select(o => o.CTe.ModeloDocumentoFiscal)?.FirstOrDefault() ?? null;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroCargaPedido(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => query.Any(c => o.CTe == c.Documento.CTe));

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryCargaCTe.Any(c => c.Carga == o.Carga));

            return queryCargaPedido.FirstOrDefault();
        }

        public bool TodasFaturasFinalizadasDaCarga(int codigoCarga)
        {
            var queryDoc = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            queryDoc = queryDoc.Where(c => c.CargaPagamento.Codigo == codigoCarga);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            query = query.Where(c => queryDoc.Any(p => p.Codigo == c.Documento.Codigo && c.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado && c.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado));

            return query.Any();
        }

        public bool TodasIntegracoesFaturasFinalizadasDaCarga(int codigoCarga)
        {
            var queryDoc = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            queryDoc = queryDoc.Where(c => c.CargaPagamento.Codigo == codigoCarga);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            query = query.Where(c => queryDoc.Any(p => p.Codigo == c.Documento.Codigo && c.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado && c.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado));

            var queryIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            queryIntegracao = queryIntegracao.Where(c => c.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado && query.Any(p => p.Fatura.Codigo == c.Fatura.Codigo));

            return queryIntegracao.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargas(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            var queryDoc = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            queryDoc = queryDoc.Where(o => query.Any(c => o.Codigo == c.Documento.Codigo));

            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            queryCarga = queryCarga.Where(o => queryDoc.Any(c => c.CargaPagamento.Codigo == o.Codigo));

            return queryCarga.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarPrimeiraCarga(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => query.Any(c => o.CTe == c.Documento.CTe));

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryCargaCTe.Any(c => c.Carga == o.Carga));

            return queryCargaPedido.FirstOrDefault()?.Carga ?? null;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido BuscarPrimeiroCargaPedidoPorCTe(int codigoCTe)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => o.CTe.Codigo == codigoCTe);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryCargaCTe.Any(c => c.Carga == o.Carga));

            return queryCargaPedido.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento BuscarPrimeiroDocumentoFaturamento(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>()
                .Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Select(o => o.Documento).FirstOrDefault();
        }

        public DateTime? BuscarPrimeiraDataPrevisaoChegadaNavioPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(o => o.Codigo == codigoCTe && o.Viagem != null);

            var querySchedule = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            querySchedule = querySchedule.Where(o => query.Any(d => d.Viagem == o.PedidoViagemNavio && d.TerminalDestino.Codigo == o.TerminalAtracacao.Codigo));

            return querySchedule.Select(o => o.DataPrevisaoChegadaNavio).FirstOrDefault();
        }

        public DateTime? BuscarPrimeiraDataPrevisaoChegadaNavio(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.CTe.Viagem != null);

            var querySchedule = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            querySchedule = querySchedule.Where(o => query.Any(d => d.Documento.CTe.Viagem == o.PedidoViagemNavio && d.Documento.CTe.TerminalDestino.Codigo == o.TerminalAtracacao.Codigo));

            return querySchedule.Select(o => o.DataPrevisaoChegadaNavio).FirstOrDefault();
            //return query.Select(o => o.Documento.CTe.Viagem.DataPrevisaoChegadaNavio).FirstOrDefault() ?? query.Select(o => o.Documento.Carga.CargaCTes.Select(c => c.Carga.PedidoViagemNavio.DataPrevisaoChegadaNavio).FirstOrDefault()).FirstOrDefault();
        }

        public DateTime? BuscarPrimeiraDataPrevisaoSaidaNavioPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(o => o.Codigo == codigoCTe && o.Viagem != null);

            var querySchedule = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            querySchedule = querySchedule.Where(o => query.Any(d => d.Viagem == o.PedidoViagemNavio && d.TerminalOrigem.Codigo == o.TerminalAtracacao.Codigo));

            return querySchedule.Select(o => o.DataPrevisaoSaidaNavio).FirstOrDefault();
            //return query.Select(o => o.Documento.CTe.Viagem.DataPrevisaoSaidaNavio).FirstOrDefault() ?? query.Select(o => o.Documento.Carga.CargaCTes.Select(c => c.Carga.PedidoViagemNavio.DataPrevisaoSaidaNavio).FirstOrDefault()).FirstOrDefault();
        }

        public DateTime? BuscarPrimeiraDataPrevisaoSaidaNavio(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.CTe.Viagem != null);

            var querySchedule = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule>();
            querySchedule = querySchedule.Where(o => query.Any(d => d.Documento.CTe.Viagem == o.PedidoViagemNavio && d.Documento.CTe.TerminalOrigem.Codigo == o.TerminalAtracacao.Codigo));

            return querySchedule.Select(o => o.DataPrevisaoSaidaNavio).FirstOrDefault();
            //return query.Select(o => o.Documento.CTe.Viagem.DataPrevisaoSaidaNavio).FirstOrDefault() ?? query.Select(o => o.Documento.Carga.CargaCTes.Select(c => c.Carga.PedidoViagemNavio.DataPrevisaoSaidaNavio).FirstOrDefault()).FirstOrDefault();
        }

        public DateTime? BuscarPrimeiraDataPrevisaoEncerramentoPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.Codigo == codigoCTe);

            return query.Select(o => o.DataPrevistaEntrega).FirstOrDefault();
        }

        public DateTime? BuscarPrimeiraDataPrevisaoEncerramento(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Select(o => o.Documento.CTe.DataPrevistaEntrega).FirstOrDefault() ?? query.Select(o => o.Documento.Carga.CargaCTes.Select(c => c.CTe.DataPrevistaEntrega).FirstOrDefault()).FirstOrDefault();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarConhecimentos(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Select(o => o.Documento.CTe).ToList();
        }

        public DateTime? BuscarPrimeiraDataPrevisaoInicioPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            query = query.Where(o => o.Codigo == codigoCTe);

            return query.Select(o => o.DataInicioPrestacaoServico).FirstOrDefault();
        }

        public DateTime? BuscarPrimeiraDataPrevisaoInicio(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Select(o => o.Documento.CTe.DataInicioPrestacaoServico).FirstOrDefault() ?? query.Select(o => o.Documento.Carga.CargaCTes.Select(c => c.CTe.DataInicioPrestacaoServico).FirstOrDefault()).FirstOrDefault();
        }

        public DateTime? BuscarDataEmissaoDocumento(int codigoFatura, string order)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.OrderBy("Documento.DataEmissao " + order).Select(o => o.Documento.DataEmissao).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> BuscarPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Fetch(o => o.Documento).ThenFetch(o => o.CTe)
                        .Fetch(o => o.Documento).ThenFetch(o => o.ModeloDocumentoFiscal)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> BuscarPorFatura(List<int> codigosFaturas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => codigosFaturas.Contains(o.Fatura.Codigo));

            return query.Fetch(o => o.Documento).ThenFetch(o => o.CTe)
                        .Fetch(o => o.Documento).ThenFetch(o => o.ModeloDocumentoFiscal)
                        .ToList();
        }

        public List<int> BuscarCodigosPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosNaoCanceladosPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && !o.Cancelado);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<string> BuscarChaveCTesAutorizadosPorCarga(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.Carga);

            return query.Select(o => o.Documento.Carga.CargaCTes.Where(c => c.CargaCTeComplementoInfo == null && c.CTe.Status == "A").Select(c => c.CTe.Chave)).SelectMany(o => o).ToList();
        }

        public List<string> BuscarChaveCTesAutorizadosPorCTe(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe && o.Documento.CTe.Status == "A");

            return query.Select(o => o.Documento.CTe.Chave).ToList();
        }

        public DateTime ObterMaiorDataEmissaoPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Max(o => o.Documento.DataEmissao);
        }

        public bool PossuiFaturamentoParcialDocumento(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.ValorACobrar != o.Documento.ValorDocumento);

            return query.Any();
        }

        public List<string> ObterDocumentosComCanhotoPendente(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe && o.Documento.CTe.XMLNotaFiscais.Any(xml => xml.Canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente));

            return query.Select(o => o.Documento.Numero).ToList();
        }

        public List<int> BuscarCodigosCTesPorFatura(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe);

            return query.Select(o => o.Documento.CTe.Codigo).Distinct().ToList();
        }

        public List<string> BuscarChavesCTesPorFatura(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe);

            return query.Select(o => o.Documento.CTe.Chave).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> BuscarPorDocumentoFaturamento(int codigoDocumentoFaturamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura situacaoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Documento.Codigo == codigoDocumentoFaturamento && o.Fatura.Situacao == situacaoFatura);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaDocumento BuscarPorDocumentoFaturamentoEFatura(int codigoDocumentoFaturamento, int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Documento.Codigo == codigoDocumentoFaturamento && o.Fatura.Codigo == codigoFatura);

            return query.FirstOrDefault();
        }

        public bool PossuiMoedaEstrangeira(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.Moeda.HasValue && o.Documento.Moeda.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? ObterMoedaFatura(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.Documento.Moeda.HasValue && o.Documento.Moeda.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real);

            return query.Select(o => o.Documento.Moeda).FirstOrDefault();
        }

        public decimal ObterTotalMoedaEstrangeira(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.Sum(o => (decimal?)o.Documento.ValorTotalMoeda) ?? 0m;
        }

        #region Metodos privados

        #endregion
    }
}
