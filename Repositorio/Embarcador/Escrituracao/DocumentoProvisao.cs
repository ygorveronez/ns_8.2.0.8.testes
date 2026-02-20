using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Escrituracao
{
    public class DocumentoProvisao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>
    {
        #region Construtores

        public DocumentoProvisao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public DocumentoProvisao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao filtrosPesquisa)
        {
            var consultaDocumentoProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>()
                .Where(o => (o.CTe == null || o.CTe.Status != "C"));

            if (filtrosPesquisa.ConcatenarComDocumentosSemPrevisao)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(obj => (obj.Provisao == null && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao) || obj.Provisao.Codigo == filtrosPesquisa.CodigoProvisao);
            else
            {
                if (filtrosPesquisa.CodigoProvisao > 0)
                    consultaDocumentoProvisao = consultaDocumentoProvisao.Where(obj => obj.Provisao.Codigo == filtrosPesquisa.CodigoProvisao);

                if (filtrosPesquisa.SomenteSemProvisao)
                    consultaDocumentoProvisao = consultaDocumentoProvisao.Where(obj => obj.Provisao == null);
            }

            if (filtrosPesquisa.TipoLocalPrestacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.interMunicipal)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(obj => obj.CST != null);
            else if (filtrosPesquisa.TipoLocalPrestacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.intraMunicipal)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(obj => obj.CST == null);

            if (filtrosPesquisa.CodigoCancelamentoProvisao > 0)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(obj => obj.CancelamentoProvisao.Codigo == filtrosPesquisa.CodigoCancelamentoProvisao);

            if (filtrosPesquisa.CodigoCarga > 0)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.Carga.Codigo == filtrosPesquisa.CodigoCarga);

            if (filtrosPesquisa.CodigoOcorrencia > 0)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.CargaOcorrencia.Codigo == filtrosPesquisa.CodigoOcorrencia);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(obj => obj.DataEmissao >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(obj => obj.DataEmissao < filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.ListaCodigoTransportador?.Count > 0)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => filtrosPesquisa.ListaCodigoTransportador.Contains(o.Empresa.Codigo));

            if (filtrosPesquisa.CpfCnpjTomador > 0)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.Tomador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador);

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoRegraEscrituracao > 0)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.RegraEscrituracao.Codigo == filtrosPesquisa.CodigoRegraEscrituracao);

            if (filtrosPesquisa.CancelamentoProvisaoContraPartida)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.DisponivelParaCancelamentoContraPartida);
            else
                if (filtrosPesquisa.SituacaoProvisaoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Todos)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.Situacao == filtrosPesquisa.SituacaoProvisaoDocumento);

            if (filtrosPesquisa?.TipoEtapasDocumentoProvisao ?? false)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.CTe != null);
            else
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.CTe == null);

            if (filtrosPesquisa.NaoPermitirProvisionarSemCalculoFrete)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.Carga.SituacaoCarga != SituacaoCarga.CalculoFrete && o.Carga.SituacaoCarga != SituacaoCarga.Nova);

            return consultaDocumentoProvisao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Codigo == codigo
                        select obj;

            return resut.Fetch(obj => obj.Carga)
                .Fetch(obj => obj.CargaOcorrencia)
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.EmpresaPai)
                .ThenFetch(obj => obj.Configuracao).FirstOrDefault();
        }

        public bool ExisteDocumentoProvisionadoPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Carga.Codigo == codigo && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado
                        select obj;

            return resut.Any();
        }

        public bool ExisteProvisaoCanceladaPorPagamento(int pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Pagamento.Codigo == pagamento && obj.Cancelado
                        select obj;

            return resut.Any();
        }

        public bool ExisteProvisaoCanceladaPorPagamentos(List<int> pagamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where pagamentos.Contains(obj.Pagamento.Codigo) && obj.CancelamentoProvisao.CancelamentoProvisaoContraPartida
                        select obj;

            return resut.Any();
        }

        public bool ExisteDocumentoProvisionadoPorOcorrencia(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.CargaOcorrencia.Codigo == codigo && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado
                        select obj;

            return resut.Any();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoProvisaoSumarizada> BuscarCodigosPorProvisaoEmCancelamento(int codigoCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            query = query.Where(o => o.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao && o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmCancelamento && !o.MovimentoFinanceiroGerado);

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.CancelamentoProvisaoSumarizada()
            {
                Codigo = o.Codigo,
                Carga = o.Carga.Codigo,
                CargaOcorrencia = o.CargaOcorrencia.Codigo,
                SituacaoCarga = o.Carga.SituacaoCarga,
                SituacaoOcorrencia = o.CargaOcorrencia.SituacaoOcorrencia
            }).ToList();
        }

        public List<int> BuscarCodigosPorCancelamentoProvisao(int codigoCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            query = query.Where(o => o.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao);

            return query.Select(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarCodigosPorOcorrenciaNFSParaPagamento(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            query = query.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
            || o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado
            );

            if (ocorrencia > 0)
                query = query.Where(o => o.CargaOcorrencia.Codigo == ocorrencia);

            query.Where(o => o.XMLNotaFiscal == null && o.CTeTerceiro == null);

            return query.FirstOrDefault();


        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarCodigosPorProvisaoParaPagamento(int xmlNotaFiscal, int cteTerceiro, int pedidoCTeParaSubContratacao, int carga, int ocorrencia, bool permitirProvisoesEstornadas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            query = query.Where(o => permitirProvisoesEstornadas || o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao || o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado);

            if (carga > 0)
                query = query.Where(o => o.Carga.Codigo == carga && o.CargaOcorrencia == null);

            if (ocorrencia > 0)
                query = query.Where(o => o.CargaOcorrencia.Codigo == ocorrencia);

            if (xmlNotaFiscal > 0)
                query = query.Where(o => o.XMLNotaFiscal.Codigo == xmlNotaFiscal);

            if (cteTerceiro > 0)
                query = query.Where(o => o.CTeTerceiro.Codigo == cteTerceiro);

            if (pedidoCTeParaSubContratacao > 0)
                query = query.Where(o => o.PedidoCTeParaSubContratacao.Codigo == pedidoCTeParaSubContratacao || o.PedidoCTeParaSubContratacao == null);

            return query.FirstOrDefault();


            //return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoSumarizada()
            //{
            //    Codigo = o.Codigo,
            //    Empresa = o.Empresa.Codigo,
            //    TipoOperacao = o.TipoOperacao.Codigo,
            //    Tomador = o.Tomador.CPF_CNPJ,
            //    GrupoTomador = o.Tomador.GrupoPessoas.Codigo,
            //    XMLNotaFiscal = o.XMLNotaFiscal.Codigo,
            //    CTeTerceiro = o.CTeTerceiro.Codigo,
            //    ModeloDocumentoFiscal = o.ModeloDocumentoFiscal.Codigo,
            //    TipoDocumentoEmissao = o.ModeloDocumentoFiscal.TipoDocumentoEmissao,
            //    NumeroDocumento = o.NumeroDocumento,
            //    SerieDocumento = o.SerieDocumento,
            //    DataEmissao = o.DataEmissao,
            //    Situacao = o.Situacao,
            //    Carga = o.Carga.Codigo,
            //    Filial = o.Filial.Codigo,
            //    CargaOcorrencia = o.CargaOcorrencia.Codigo,
            //    Valor = o.ValorProvisao,
            //    ValorICMS = o.ValorICMS,
            //    PercentualAliquota = o.PercentualAliquota,
            //    CST = o.CST,
            //    BaseCalculoICMS = o.BaseCalculoICMS,
            //    ValorISS = o.ValorISS,
            //    BaseCalculoISS = o.BaseCalculoISS,
            //    ValorRetencaoISS = o.ValorRetencaoISS,
            //    PercentualAliquotaISS = o.PercentualAliquotaISS,
            //    ICMSInclusoBC = o.ICMSInclusoBC,
            //    ISSInclusoBC = o.ISSInclusoBC
            //}).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarCodigosPorProvisaoParaPagamentoPorCargas(List<int> cargas)
        {
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> result = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();

            int take = 1000;
            int start = 0;
            while (start < cargas?.Count)
            {
                //Códigos dos pedidos take...
                List<int> tmp = cargas.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
                var filter = from o in query
                             where (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao || o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado)
                             && (cargas.Contains(o.Carga.Codigo) && o.CargaOcorrencia == null)
                             select o;

                result.AddRange(filter.ToList());

                start += take;
            }

            return result;
        }

        //public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> qBuscarCodigosPorProvisaoParaPagamentoPorCargas(List<int> cargas)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
        //    query = query.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao || o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado);

        //    query = query.Where(o => cargas.Contains(o.Carga.Codigo) && o.CargaOcorrencia == null);

        //    return query.ToList();
        //}

        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoSumarizada> BuscarCodigosPorProvisaoEmFechamento(int codigoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            query = query.Where(o => o.Provisao.Codigo == codigoProvisao && o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmFechamento && !o.MovimentoFinanceiroGerado);

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoSumarizada()
            {
                Codigo = o.Codigo,
                Empresa = o.Empresa.Codigo,
                TipoOperacao = o.TipoOperacao.Codigo,
                Tomador = o.Tomador.CPF_CNPJ,
                RotaFrete = o.Carga.Rota.Codigo,
                Stage = o.Stage.Codigo,
                ImpostoValorAgregado = o.ImpostoValorAgregado.Codigo,
                CodigoImpostoValorAgregado = o.ImpostoValorAgregado.CodigoIVA,
                GrupoTomador = o.Tomador.GrupoPessoas.Codigo,
                Remetente = o.Remetente.CPF_CNPJ,
                GrupoRemetente = o.Remetente.GrupoPessoas.Codigo,
                Destinatario = o.Destinatario.CPF_CNPJ,
                Expedidor = o.Expedidor.CPF_CNPJ,
                Recebedor = o.Recebedor.CPF_CNPJ,
                GrupoDestinatario = o.Destinatario.GrupoPessoas.Codigo,
                Origem = o.Origem.Codigo,
                Destino = o.Destino.Codigo,
                PesoBruto = o.PesoBruto,
                XMLNotaFiscal = o.XMLNotaFiscal.Codigo,
                CTeTerceiro = o.CTeTerceiro.Codigo,
                PedidoCTeParaSubContratacao = o.PedidoCTeParaSubContratacao.Codigo,
                ModeloDocumentoFiscal = o.ModeloDocumentoFiscal.Codigo,
                TipoDocumentoEmissao = o.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                NumeroDocumento = o.NumeroDocumento,
                SerieDocumento = o.SerieDocumento,
                DataEmissao = o.DataEmissao,
                Carga = o.Carga.Codigo,
                Filial = o.Filial.Codigo,
                CargaOcorrencia = o.CargaOcorrencia.Codigo,
                TipoOcorrencia = o.CargaOcorrencia.TipoOcorrencia.Codigo,
                Valor = o.ValorProvisao,
                ValorICMS = o.ValorICMS,
                PercentualAliquota = o.PercentualAliquota,
                CST = o.CST,
                BaseCalculoICMS = o.BaseCalculoICMS,
                ValorISS = o.ValorISS,
                BaseCalculoISS = o.BaseCalculoISS,
                ValorRetencaoISS = o.ValorRetencaoISS,
                PercentualAliquotaISS = o.PercentualAliquotaISS,
                ICMSInclusoBC = o.ICMSInclusoBC,
                ISSInclusoBC = o.ISSInclusoBC,
                ValorAdValorem = (decimal?)o.ValorAdValorem != null ? o.ValorAdValorem : 0m,
                ValorDescarga = (decimal?)o.ValorDescarga != null ? o.ValorDescarga : 0m,
                ValorPedagio = (decimal?)o.ValorPedagio != null ? o.ValorPedagio : 0m,
                ValorGris = (decimal?)o.ValorGris != null ? o.ValorGris : 0m,
                ValorEntrega = (decimal?)o.ValorEntrega != null ? o.ValorEntrega : 0m,
                ValorPernoite = (decimal?)o.ValorPernoite != null ? o.ValorPernoite : 0m,
                ValorContratoFrete = (decimal?)o.ValorContratoFrete != null ? o.ValorContratoFrete : 0m,
                ValorFrete = (decimal?)o.ValorFrete != null ? o.ValorFrete : 0m,
                TipoValorFrete = (TipoValorFreteDocumentoProvisao?)o.TipoValorFrete != null ? o.TipoValorFrete : TipoValorFreteDocumentoProvisao.NaoDefinido,
                ValorDesconto = o.ValorDesconto,
                OutrasAliquotas = o.OutrasAliquotas,
                CSTIBSCBS = o.CSTIBSCBS,
                ClassificacaoTributariaIBSCBS = o.ClassificacaoTributariaIBSCBS,
                BaseCalculoIBSCBS = o.BaseCalculoIBSCBS,
                AliquotaIBSEstadual = o.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = o.PercentualReducaoIBSEstadual,
                ValorIBSEstadual = o.ValorIBSEstadual,
                AliquotaIBSMunicipal = o.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = o.PercentualReducaoIBSMunicipal,
                ValorIBSMunicipal = o.ValorIBSMunicipal,
                AliquotaCBS = o.AliquotaCBS,
                PercentualReducaoCBS = o.PercentualReducaoCBS,
                ValorCBS = o.ValorCBS
            }).ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarDocumentoCargaEmFechamento(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Carga.Codigo == carga && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmFechamento
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarDocumentoCargaAgrupamentoEmFechamento(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Carga.CargaAgrupamento.Codigo == carga && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmFechamento
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarDocumentoOcorrenciaEmFechamento(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.CargaOcorrencia.Codigo == ocorrencia && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmFechamento
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarDocumentoPorXMLNotaFiscalCargaEDocumentoFaturamento(int xmlNotaFiscal, int codigoCarga, int codigoDocumentoFaturamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal
                              && obj.Carga.Codigo == codigoCarga
                              && obj.DocumentoFaturamento != null && obj.DocumentoFaturamento.Codigo == codigoDocumentoFaturamento
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarPorXMLNotaFiscalECarga(int xmlNotaFiscal, int carga, bool considerarProvisaoComOcorrencia = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal && obj.Carga.Codigo == carga
                              && (considerarProvisaoComOcorrencia || obj.CargaOcorrencia == null)
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarPorCTeAnteriorECarga(int cteAnterior, int carga, int pedidoCTeParaSubContratacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.CTeTerceiro.Codigo == cteAnterior && obj.Carga.Codigo == carga && obj.CargaOcorrencia == null
                        && (obj.PedidoCTeParaSubContratacao.Codigo == pedidoCTeParaSubContratacao || obj.PedidoCTeParaSubContratacao == null)
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarPorNFSEOcorrencia(int lancalmentoNFe, int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.LancamentoNFSManual.Codigo == lancalmentoNFe && obj.CargaOcorrencia.Codigo == ocorrencia
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarPorCTeEFechamento(int cte, int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.CTe.Codigo == cte && obj.FechamentoFrete.Codigo == fechamento
                        select obj;

            return resut.FirstOrDefault();
        }

        public List<(int CodigoCarga, bool CargaTrechoDefinida)> BuscarDadosCargasPorDocumentosAguardandoProvisao(bool retornarCargasLogoAposEmissao, bool naoRetornarDocumentosProvisaoComVinculoOcorrencia, bool naoRetornarCargasAguardandoImportarCTeOuLancarNFS)
        {
            var consultaDocumentoProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var consultaCargaCancelamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            var consultaCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            List<SituacaoCarga> situacoesCargaNaoGerar = new List<SituacaoCarga>()
            {
                SituacaoCarga.Nova,
                SituacaoCarga.AgIntegracao,
                SituacaoCarga.CalculoFrete,
                SituacaoCarga.Cancelada,
                SituacaoCarga.Anulada
            };

            List<SituacaoCheckin> situacoesCheckinPendente = SituacaoCheckinHelper.ObterSituacoesPendentes();

            consultaCargaCTe = consultaCargaCTe.Where(cargaCTe => cargaCTe.CTe.Status != "C" && cargaCTe.CTe.Status != "I" && situacoesCheckinPendente.Contains(cargaCTe.SituacaoCheckin));

            consultaCargaCancelamento = consultaCargaCancelamento.Where(cancelamento =>
                   cancelamento.Situacao != SituacaoCancelamentoCarga.Anulada &&
                   cancelamento.Situacao != SituacaoCancelamentoCarga.Cancelada &&
                   cancelamento.Situacao != SituacaoCancelamentoCarga.SolicitacaoReprovada &&
                   cancelamento.Situacao != SituacaoCancelamentoCarga.Reprovada
               );

            consultaDocumentoProvisao = consultaDocumentoProvisao.Where(documento =>
                    documento.Situacao == SituacaoProvisaoDocumento.AgProvisao &&
                    documento.Carga.DataFinalizacaoEmissao != null &&
                    !documento.Carga.CargaEmitidaParcialmente &&
                    (documento.Carga.DadosSumarizados.CargaTrecho == null || (documento.Carga.TipoOperacao != null && documento.Carga.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn &&
                    !consultaCargaCTe.Any(cargaCTe => cargaCTe.Carga.Codigo == documento.Carga.Codigo))) &&
                    !consultaCargaCancelamento.Any(cancelamento => cancelamento.Carga.Codigo == documento.Carga.Codigo) &&
                    !situacoesCargaNaoGerar.Contains(documento.Carga.SituacaoCarga)
                );

            if (naoRetornarDocumentosProvisaoComVinculoOcorrencia)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(obj => obj.CargaOcorrencia == null);

            if (naoRetornarCargasAguardandoImportarCTeOuLancarNFS)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(obj => !obj.Carga.AgImportacaoCTe && !obj.Carga.AgNFSManual);

            if (!retornarCargasLogoAposEmissao)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Where(o => o.DataEmissao <= DateTime.Now.AddDays(-1));

            return consultaDocumentoProvisao
                .GroupBy(o => new { Codigo = o.Carga.Codigo, CargaTrecho = o.Carga.DadosSumarizados.CargaTrecho })
                .Select(g => new ValueTuple<int, bool>(g.Key.Codigo, g.Key.CargaTrecho != null))
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarPorXMLNotaFiscalEFechamento(int xmlNotaFiscal, int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal && obj.FechamentoFrete.Codigo == fechamento
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarPorXMLNotaFiscalEOcorrencia(int xmlNotaFiscal, int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal && obj.CargaOcorrencia.Codigo == ocorrencia
                        select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarPorCTeAnteriorEOcorrencia(int cteAnterior, int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.CTeTerceiro.Codigo == cteAnterior && obj.CargaOcorrencia.Codigo == ocorrencia
                        select obj;

            return resut.FirstOrDefault();
        }

        public decimal ValorTotalPorCancelamentoProvisao(int cancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.CancelamentoProvisao.Codigo == cancelamentoProvisao
                        select obj;
            return resut.Sum(obj => obj.ValorProvisao);
        }

        public decimal ValorTotalPorProvisao(int provisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Provisao.Codigo == provisao
                        select obj;
            return resut.Sum(obj => obj.ValorProvisao);
        }

        public int DisponibilizarProvisoesProvionarPorDocumentoFaturamento(int documentoFaturamento)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.Pagamento = null, docProvi.DocumentoFaturamento = null, docProvi.Situacao= :Situacao where docProvi.DocumentoFaturamento= :DocumentoFaturamento and docProvi.Provisao is null";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("DocumentoFaturamento", documentoFaturamento);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao);
            return query.ExecuteUpdate();
        }

        public int DisponibilizarProvisoesLiquidarPorDocumentoFaturamento(int documentoFaturamento)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.Pagamento = null, docProvi.DocumentoFaturamento = null, docProvi.Situacao= :Situacao where docProvi.DocumentoFaturamento= :DocumentoFaturamento and docProvi.Provisao is not null";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("DocumentoFaturamento", documentoFaturamento);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado);
            return query.ExecuteUpdate();
        }

        public void LimparDocumentosProvisaoPorPagamento(int pagamento)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.Pagamento = null, docProvi.DocumentoFaturamento = null where docProvi.Pagamento= :Pagamento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Pagamento", pagamento);
            query.ExecuteUpdate();
        }

        public void LiquidarDocumentosProvisaoPorPagamento(int pagamento, bool cancelarContraPartida)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.Situacao= :Situacao where docProvi.Pagamento= :Pagamento ";
            if (cancelarContraPartida)
                hql += " and docProvi.Situacao != :SituacaoContraPartida";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Pagamento", pagamento);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Liquidado);

            if (cancelarContraPartida)
                query.SetEnum("SituacaoContraPartida", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado);

            query.ExecuteUpdate();
        }

        public void DisponibilizarDocumentosParaCancelamentoContraPartida(int pagamento)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.Situacao= :Situacao where docProvi.Pagamento= :Pagamento and docProvi.Situacao= :SituacaoContraPartida";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Pagamento", pagamento);
            query.SetEnum("SituacaoContraPartida", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.DisponivelParaCancelamentoContraPartida);
            query.ExecuteUpdate();
        }

        public void ConfirmarProvisaoDocumentos(int provisao)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.Situacao= :Situacao where docProvi.Provisao= :Provisao ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Provisao", provisao);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado);
            query.ExecuteUpdate();
        }

        public void SetarDocumentosGerarMovimentoProvisionar(int provisao)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado, docProvi.Situacao =:Situacao where docProvi.Provisao= :Provisao ";

            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Provisao", provisao);
            query.SetInt32("Situacao", (int)SituacaoProvisaoDocumento.EmFechamento);
            query.SetBoolean("MovimentoFinanceiroGerado", false);
            query.ExecuteUpdate();
        }

        public void SetarDocumentosLiberadosProvisionar(int provisao)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.Provisao = null, docProvi.Situacao= :Situacao, docProvi.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado where docProvi.Provisao= :Provisao ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Provisao", provisao);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao);
            query.SetBoolean("MovimentoFinanceiroGerado", false);
            query.ExecuteUpdate();
        }

        public void SetarDocumentosRefazerProvisaoPorStage(int codigoProvisao, int codigoStage, int codigoImpostoValorAgregado)
        {
            this.SessionNHiBernate
                .CreateQuery("update DocumentoProvisao documento set documento.Situacao = :situacao, documento.MovimentoFinanceiroGerado = :movimentoFinanceiroGerado, documento.ImpostoValorAgregado = :codigoImpostoValorAgregado where documento.Provisao = :codigoProvisao and documento.Stage = :codigoStage")
                .SetInt32("codigoProvisao", codigoProvisao)
                .SetInt32("codigoStage", codigoStage)
                .SetInt32("codigoImpostoValorAgregado", codigoImpostoValorAgregado)
                .SetEnum("situacao", SituacaoProvisaoDocumento.EmFechamento)
                .SetBoolean("movimentoFinanceiroGerado", false)
                .ExecuteUpdate();
        }

        public void SetarImpostoValorAgregadoPorProvisaoEStage(int codigoProvisao, int codigoStage, int codigoImpostoValorAgregado)
        {
            this.SessionNHiBernate
                .CreateQuery("update DocumentoProvisao documento set documento.ImpostoValorAgregado = :codigoImpostoValorAgregado where documento.Provisao = :codigoProvisao and documento.Stage = :codigoStage")
                .SetInt32("codigoProvisao", codigoProvisao)
                .SetInt32("codigoStage", codigoStage)
                .SetParameter("codigoImpostoValorAgregado", (codigoImpostoValorAgregado > 0) ? (int?)codigoImpostoValorAgregado : null)
                .ExecuteUpdate();
        }

        public List<(int CodigoStage, int CodigoImpostoValorAgregado, TipoNotaFiscalIntegrada? TipoNotaFiscalIntegrada, int CodigoCargaPedido)> BuscarDadosImpostoValorAgregadoPorProvisaoComStage(int codigoProvisao)
        {
            var consultaDocumentoProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>()
                .Where(documento => documento.Provisao.Codigo == codigoProvisao && documento.Stage != null);

            var consultaPedidoNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            return consultaDocumentoProvisao
                .Select(documento => ValueTuple.Create(
                    documento.Stage.Codigo,
                    (documento.ImpostoValorAgregado != null) ? documento.ImpostoValorAgregado.Codigo : 0,
                    (documento.XMLNotaFiscal != null) ? documento.XMLNotaFiscal.TipoNotaFiscalIntegrada : null,
                    (documento.XMLNotaFiscal != null) ? consultaPedidoNotaFiscal.Where(x => x.XMLNotaFiscal.Codigo == documento.XMLNotaFiscal.Codigo && x.CargaPedido.Codigo != null).Select(x => x.CargaPedido.Codigo).FirstOrDefault() : 0
                 ))
                .ToList();
        }

        public void SetarDocumentoMovimentoGeradoCancelamento(int documento)
        {
            string hql = "update DocumentoProvisao docProvi set  docProvi.Situacao= :Situacao, docProvi.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado where docProvi.Codigo = :Codigo ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Codigo", documento);
            query.SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Cancelado);
            query.SetBoolean("MovimentoFinanceiroGerado", true);
            query.ExecuteUpdate();
        }

        public void SetarDocumentoMovimentoGeradoProvisionado(int documento)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado where docProvi.Codigo = :Codigo ";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Codigo", documento);
            query.SetBoolean("MovimentoFinanceiroGerado", true);
            query.ExecuteUpdate();
        }

        public int SetarDocumentosParaProvisao(int provisao, Dominio.ObjetosDeValor.Embarcador.Escrituracao.AdicionaProvisao parametros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento situacao, List<int> codigosNaoSelecionadas)
        {
            StringBuilder hql = new StringBuilder();

            hql.Append("update DocumentoProvisao docProvi ");
            hql.Append("   set docProvi.Situacao= :Situacao, ");
            hql.Append("       docProvi.Provisao= :Provisao ");
            hql.Append(" where docProvi.Situacao = :SituacaoAtual ");
            hql.Append("   and docProvi.Provisao is null ");
            hql.Append("   and docProvi.DataEmissao >= :DataInicio ", parametros.DataInicio.HasValue);
            hql.Append("   and docProvi.DataEmissao < :DataFim ", parametros.DataFim.HasValue);
            hql.Append("   and docProvi.Tomador = :Tomador ", parametros.Tomador > 0);
            hql.Append("   and docProvi.Filial = :Filial ", parametros.Filial > 0);
            hql.Append("   and docProvi.Empresa in (:Empresa) ", parametros.CodigosTransportadores?.Count > 0);
            hql.Append("   and docProvi.Carga = :Carga ", parametros.Carga > 0);
            hql.Append("   and docProvi.CargaOcorrencia = :CargaOcorrencia ", parametros.Ocorrencia > 0);
            hql.Append("   and docProvi.TipoOperacao = :TipoOperacao ", parametros.TipoOperacao > 0);
            hql.Append("   and docProvi.Codigo not in (:CodigosFora) ", codigosNaoSelecionadas.Count > 0);
            hql.Append("   and docProvi.CST is null ", parametros.TipoLocalPrestacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.intraMunicipal);
            hql.Append("   and docProvi.CST is not null ", parametros.TipoLocalPrestacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.interMunicipal);
            hql.Append("   and ( ");
            hql.Append("           docProvi.CTe is null or not exists ( ");
            hql.Append("               select 1 ");
            hql.Append("                 from ConhecimentoDeTransporteEletronico docProviCte ");
            hql.Append("                where docProviCte = docProvi.CTe ");
            hql.Append("                  and docProviCte.Status = 'C' ");
            hql.Append("           ) ");
            hql.Append("       ) ");

            if (parametros.DocumentosProvisoes != null)
                hql.Append("   and docProvi.Codigo in (:Documentos) ");

            var query = this.SessionNHiBernate.CreateQuery(hql.ToString());

            query.SetEnum("SituacaoAtual", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao);
            query.SetEnum("Situacao", situacao);
            query.SetInt32("Provisao", provisao);

            if (parametros.DataInicio.HasValue)
                query.SetDateTime("DataInicio", parametros.DataInicio.Value.Date);

            if (parametros.DataFim.HasValue)
                query.SetDateTime("DataFim", parametros.DataFim.Value.AddDays(1).Date);

            if (parametros.CodigosTransportadores?.Count > 0)
                query.SetParameterList("Empresa", parametros.CodigosTransportadores, NHibernate.NHibernateUtil.Int32);

            if (parametros.Tomador > 0)
                query.SetDouble("Tomador", parametros.Tomador);

            if (parametros.Filial > 0)
                query.SetInt32("Filial", parametros.Filial);

            if (parametros.Carga > 0)
                query.SetInt32("Carga", parametros.Carga);

            if (parametros.Ocorrencia > 0)
                query.SetInt32("CargaOcorrencia", parametros.Ocorrencia);

            if (parametros.TipoOperacao > 0)
                query.SetInt32("TipoOperacao", parametros.TipoOperacao);

            if (codigosNaoSelecionadas.Count > 0)
                query.SetParameterList("CodigosFora", codigosNaoSelecionadas);

            if (parametros.DocumentosProvisoes != null)
                query.SetParameterList("Documentos", parametros.DocumentosProvisoes.Select(X => X.Codigo), NHibernate.NHibernateUtil.Int32);

            return query.ExecuteUpdate();
        }

        public int SetarDocumentosParaCancelamentoProvisao(int cancelamentoProvisao, int carga, int cargaOcorrencia, DateTime dataInicio, DateTime dataFim, double tomador, int filial, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento situacao, List<int> codigosNaoSelecionadas, bool cancelamentoProvisaoContraPartida)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.Situacao= :Situacao, docProvi.MovimentoFinanceiroGerado= :MovimentoFinanceiroGerado, docProvi.CancelamentoProvisao= :CancelamentoProvisao where docProvi.Situacao = :SituacaoAtual ";

            if (dataInicio != DateTime.MinValue)
                hql += "and docProvi.DataEmissao >= :DataInicio ";
            if (dataFim != DateTime.MinValue)
                hql += "and docProvi.DataEmissao < :DataFim ";
            if (tomador > 0)
                hql += "and docProvi.Tomador = :Tomador ";
            if (filial > 0)
                hql += "and docProvi.Filial = :Filial ";
            if (empresa > 0)
                hql += "and docProvi.Empresa = :Empresa ";
            if (carga > 0)
                hql += "and docProvi.Carga = :Carga ";
            if (cargaOcorrencia > 0)
                hql += "and docProvi.CargaOcorrencia = :CargaOcorrencia ";
            if (codigosNaoSelecionadas.Count > 0)
                hql += "and docProvi.Codigo not in (:CodigosFora) ";

            var query = this.SessionNHiBernate.CreateQuery(hql);

            if (cancelamentoProvisaoContraPartida)
                query.SetEnum("SituacaoAtual", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.DisponivelParaCancelamentoContraPartida);
            else
                query.SetEnum("SituacaoAtual", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Provisionado);

            query.SetEnum("Situacao", situacao);
            query.SetBoolean("MovimentoFinanceiroGerado", false);
            query.SetInt32("CancelamentoProvisao", cancelamentoProvisao);
            if (dataInicio != DateTime.MinValue)
                query.SetDateTime("DataInicio", dataInicio.Date);
            if (dataFim != DateTime.MinValue)
                query.SetDateTime("DataFim", dataFim.AddDays(1).Date);
            if (empresa > 0)
                query.SetInt32("Empresa", empresa);
            if (tomador > 0)
                query.SetDouble("Tomador", tomador);
            if (filial > 0)
                query.SetInt32("Filial", filial);
            if (carga > 0)
                query.SetInt32("Carga", carga);
            if (cargaOcorrencia > 0)
                query.SetInt32("CargaOcorrencia", cargaOcorrencia);

            if (codigosNaoSelecionadas.Count > 0)
                query.SetParameterList("CodigosFora", codigosNaoSelecionadas);

            return query.ExecuteUpdate();
        }

        public void ExcluirTodosAgProvisaoPorCarga(int carga)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE DocumentoProvisao docProvi WHERE docProvi.Carga =:Carga and docProvi.Situacao = :Situacao and docProvi.CargaOcorrencia is null")
                             .SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao)
                             .SetInt32("Carga", carga)
                             .ExecuteUpdate();
        }

        public void ExcluirTodosAgProvisaoPorOcorrencia(int ocorrencia)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE DocumentoProvisao docProvi WHERE docProvi.CargaOcorrencia =:CargaOcorrencia and docProvi.Situacao = :Situacao ")
                             .SetEnum("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao)
                             .SetInt32("CargaOcorrencia", ocorrencia)
                             .ExecuteUpdate();
        }


        public void ExcluirPorStageECarga(int codigoStage, int codCarga)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE DocumentoProvisao docProvi WHERE docProvi.Carga =:Carga and docProvi.Stage = :Stage ")
                             .SetInt32("Carga", codCarga)
                             .SetInt32("Stage", codigoStage)
                             .ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorCargasEscriturados(List<int> codigoCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where codigoCargas.Contains(obj.Carga.Codigo) && obj.CTe.Status == "A" && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Liquidado
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorCargasAgProvisao(List<int> codigoCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where codigoCargas.Contains(obj.Carga.Codigo) && obj.CTe.Status == "A" && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorOcorrenciasEscriturados(List<int> codigosOcorrencias)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where codigosOcorrencias.Contains(obj.CargaOcorrencia.Codigo) && obj.CTe.Status == "A" && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Liquidado
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarTomadoresProvisao(int provisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Provisao.Codigo == provisao
                        select obj;

            return resut.Select(obj => obj.Tomador).Distinct().Fetch(obj => obj.GrupoPessoas).ToList();
        }


        public Task<List<Dominio.Entidades.Cliente>> BuscarTomadoresCancelamentoProvisaoAsync(int cancelamentoProvisao, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.CancelamentoProvisao.Codigo == cancelamentoProvisao
                        select obj;

            return resut.Select(obj => obj.Tomador).Distinct().Fetch(obj => obj.GrupoPessoas).ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorProvisao(int provisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Provisao.Codigo == provisao
                        select obj;

            return resut.Fetch(obj => obj.XMLNotaFiscal)
                        .Fetch(obj => obj.Carga)
                        .Fetch(obj => obj.Filial)
                        .Fetch(obj => obj.TipoOperacao)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorProvisoesComFetch(List<int> codigosProvisao, List<string> fetch)
        {
            var hql = new StringBuilder();
            hql.Append("select distinct doc from Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao doc ");

            for (int i = 0; i < fetch.Count; i++)
            {
                if (fetch[i] == "Canhoto")
                {
                    hql.Append("left join fetch doc.XMLNotaFiscal xnf left join fetch xnf.Canhoto ");
                    continue;
                }

                hql.Append($" left join fetch doc.{fetch[i]} as {fetch[i]} ");
            }

            hql.Append("where doc.Provisao.Codigo in (:codigosProvisao)");

            var query = this.SessionNHiBernate.CreateQuery(hql.ToString());
            query.SetParameterList("codigosProvisao", codigosProvisao);

            return query.List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorCancelamento(int codigoCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao
                        select obj;

            return resut
                .Fetch(obj => obj.Empresa)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.TipoOperacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> BuscarNotasPorProvisao(int provisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Provisao.Codigo == provisao
                        select obj.XMLNotaFiscal;

            return resut.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorCargaFilhoCancelada(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Carga.Codigo == codigoCarga && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Cancelado
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorCarga(int codigoCarga, bool naoRetornarDocumentosProvisaoComVinculoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao);

            if (naoRetornarDocumentosProvisaoComVinculoOcorrencia)
                query = query.Where(obj => obj.CargaOcorrencia == null);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorDocumentoFaturamentoEPagamento(int codigoDocumentoFaturamento, int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.DocumentoFaturamento.Codigo == codigoDocumentoFaturamento
                              && obj.Pagamento != null
                              && obj.Pagamento.Codigo == codigoPagamento
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorCargaENotas(int codigoCarga, List<int> notas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Carga.Codigo == codigoCarga && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
                        && notas.Contains(obj.XMLNotaFiscal.Codigo)
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorCargaENotasSemFiltro(int codigoCarga, List<int> notas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Carga.Codigo == codigoCarga
                        && notas.Contains(obj.XMLNotaFiscal.Codigo)
                        select obj;

            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao BuscarPorCargaENota(int codigoCarga, int nota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Carga.Codigo == codigoCarga && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Cancelado
                        && nota == obj.XMLNotaFiscal.Codigo
                        select obj;

            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorCargaECTe(int codigoCarga, int codigoCte)
        {
            var consultaNotaFiscalPorCte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(notaFiscalCte => notaFiscalCte.CargaCTe.Carga.Codigo == codigoCarga && notaFiscalCte.CargaCTe.CTe.Codigo == codigoCte);

            var consultaDocumentoProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>()
                .Where(documento => documento.Carga.Codigo == codigoCarga && consultaNotaFiscalPorCte.Any(notaFiscalCte => notaFiscalCte.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == documento.XMLNotaFiscal.Codigo));

            return consultaDocumentoProvisao.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarComNotaFiscalPorCarga(int codigoCarga)
        {
            var consultaDocumentoProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>()
                .Where(documento => documento.Carga.Codigo == codigoCarga && documento.XMLNotaFiscal != null);

            return consultaDocumentoProvisao.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarComNotaFiscalPorCargas(List<int> codigosCargas)
        {
            var consultaDocumentoProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>()
                .Where(documento => codigosCargas.Contains(documento.Carga.Codigo) && documento.XMLNotaFiscal != null);

            return consultaDocumentoProvisao.ToList();
        }

        public bool BuscarDocumenttosProvisaoGeradosNumIntervalo(DateTime? dataInicial, DateTime? dataFinal)
        {
            var consultaDocumentoProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>()
                .Where(documento => documento.Provisao.DataCriacao >= dataInicial && documento.Provisao.DataCriacao <= dataFinal && documento.Provisao.Situacao != SituacaoProvisao.Finalizado);
            var consultaNotas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>().Where(x => consultaDocumentoProvisao.Any(p => p.XMLNotaFiscal.Codigo == x.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo));
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>().Where(d => consultaNotas.Any(p => p.CargaCTe.CTe.Codigo == d.CTe.Codigo) && (d.NumeroMiro == string.Empty || d.NumeroMiro == null));
            return consultaDocumentoFaturamento.Any();

        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorProvisaoNaoPresenteNaLista(int provisao, List<int> documentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where obj.Provisao.Codigo == provisao && !documentos.Contains(obj.Codigo)
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorOcorrenciasAgProvisao(List<int> codigosOcorrencias)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where codigosOcorrencias.Contains(obj.CargaOcorrencia.Codigo) && obj.CTe.Status == "A" && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorCodigos(List<int> codigosDocumentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var resut = from obj in query
                        where codigosDocumentoProvisao.Contains(obj.Codigo) && obj.CTe.Status == "A"
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaDocumentoProvisao = Consultar(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                consultaDocumentoProvisao = consultaDocumentoProvisao.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

            if (parametrosConsulta.InicioRegistros > 0)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                consultaDocumentoProvisao = consultaDocumentoProvisao.Take(parametrosConsulta.LimiteRegistros);

            return consultaDocumentoProvisao
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.CargaOcorrencia)
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.Tomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.EmpresaPai)
                .ThenFetch(obj => obj.Configuracao)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.FechamentoFrete)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao filtrosPesquisa)
        {
            var consultaDocumentoProvisao = Consultar(filtrosPesquisa);

            return consultaDocumentoProvisao.Count();
        }

        public List<int> BuscarNumeroDocumentoInvalidoParaCancelamento(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga &&
                                       obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmFechamento);

            return query.Select(o => o.Provisao.Numero).Distinct().ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoAbertaParaEstorno> BuscarDadosProvisaoAbertaParaEstorno(int quantidadeDiasLimiteProvisaoAberto)
        {

            var sql = $@"select docFat.DFA_CODIGO CodigoDocumentoFaturamento, docProv.DPV_CODIGO CodigoDocumentoProvisao ,Prov.PRV_CODIGO CodigoProvisao
                        from T_CARGA_CTE CargaCTe 
                        inner join T_CARGA carga on carga.car_codigo = CargaCTe.car_codigo
                        join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                        join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                        join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                        join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                        join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                        join T_DOCUMENTO_FATURAMENTO docFat on docFat.CON_CODIGO = CargaCTe.CON_CODIGO 
                        join T_DOCUMENTO_PROVISAO docProv on docProv.NFX_CODIGO = PedidoXMLNotaFiscal.NFX_CODIGO and docProv.CAR_CODIGO = carga.CAR_CODIGO
                        join T_PROVISAO Prov on docProv.PRV_CODIGO = Prov.PRV_CODIGO
                        where _Stage.STA_DATA_FOLHA < '{DateTime.Now.AddDays(-quantidadeDiasLimiteProvisaoAberto).ToString("yyyy/MM/dd")}' and carga.car_situacao not in ({(int)SituacaoCancelamentoCarga.Anulada},{(int)SituacaoCancelamentoCarga.Cancelada},{(int)SituacaoCancelamentoCarga.SolicitacaoReprovada},{(int)SituacaoCancelamentoCarga.Reprovada})
                        and docProv.CPV_CODIGO is null and docProv.DPV_SITUACAO != 5 and docProv.DPV_SITUACAO != 6 and docFat.DFA_DATA_MIRO is null";

            //and (CargaCTe.PCO_SITUACAO_CHECKIN = {(int)SituacaoCheckin.Confirmado} or CargaCTe.PCO_SITUACAO_CHECKIN = {(int)SituacaoCheckin.RecusaReprovada} or CargaCTe.PCO_SITUACAO_CHECKIN = {(int)SituacaoCheckin.SemRegraAprovacao})

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoAbertaParaEstorno)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoAbertaParaEstorno>();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarPorStage(List<int> codigoStage)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            query = from obj in query where codigoStage.Contains(obj.Stage.Codigo) select obj;

            return query.ToList();
        }
        public IList<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoAbertaParaEstorno> BuscarDadosProvisaoAbertaParaEstornoDeTermoQuitacao(DateTime dataInicialTermo, DateTime dataFinalTermo, Dominio.Entidades.Empresa transportador)
        {


            var sql = $@"select docFat.DFA_CODIGO CodigoDocumentoFaturamento, docProv.DPV_CODIGO CodigoDocumentoProvisao ,Prov.PRV_CODIGO CodigoProvisao
                        from T_CARGA_CTE CargaCTe 
                        inner join T_CARGA carga on carga.car_codigo = CargaCTe.car_codigo
                        inner join T_EMPRESA Transportador on carga.EMP_CODIGO = Transportador.EMP_CODIGO
                        join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                        join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                        join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                        join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                        join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                        join T_DOCUMENTO_FATURAMENTO docFat on docFat.CON_CODIGO = CargaCTe.CON_CODIGO 
                        join T_DOCUMENTO_PROVISAO docProv on docProv.NFX_CODIGO = PedidoXMLNotaFiscal.NFX_CODIGO and docProv.CAR_CODIGO = carga.CAR_CODIGO
                        join T_PROVISAO Prov on docProv.PRV_CODIGO = Prov.PRV_CODIGO
                        where _Stage.STA_DATA_FOLHA > '{dataInicialTermo.ToString("yyyy/MM/dd")}' and _Stage.STA_DATA_FOLHA < '{dataFinalTermo.AddDays(1).ToString("yyyy/MM/dd")}' and carga.car_situacao not in ({(int)SituacaoCancelamentoCarga.Anulada},{(int)SituacaoCancelamentoCarga.Cancelada},{(int)SituacaoCancelamentoCarga.SolicitacaoReprovada},{(int)SituacaoCancelamentoCarga.Reprovada})
                        and docProv.CPV_CODIGO is null and docProv.DPV_SITUACAO != 5 and docProv.DPV_SITUACAO != 6 and docFat.DFA_DATA_MIRO is null and Transportador.EMP_CODIGO = {transportador.Codigo}";

            //and (CargaCTe.PCO_SITUACAO_CHECKIN = {(int)SituacaoCheckin.Confirmado} or CargaCTe.PCO_SITUACAO_CHECKIN = {(int)SituacaoCheckin.RecusaReprovada} or CargaCTe.PCO_SITUACAO_CHECKIN = {(int)SituacaoCheckin.SemRegraAprovacao})

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoAbertaParaEstorno)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoAbertaParaEstorno>();
        }

        public int SetarDocumentosCanceladosPorStage(int CodigoStage, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento situacao)
        {
            string hql = "update DocumentoProvisao docProvi set docProvi.Situacao= :Situacao where docProvi.Stage = :stage ";
            var query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetEnum("Situacao", situacao);
            if (CodigoStage > 0)
                query.SetInt32("stage", CodigoStage);

            return query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarDocumentoProvisaoPorDocumentoEscituracao(List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> documentoEscrituracaos)
        {
            int codigoCarga = documentoEscrituracaos.Select(x => x.Carga.Codigo).FirstOrDefault();

            List<int> xmlNotaFiscal = documentoEscrituracaos.SelectMany(x => x.CTe.XMLNotaFiscais).Select(x => x.Codigo).ToList();

            IQueryable<int> queryCargaPedidoXMLNotaFiscalCte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>()
                .Where(x => xmlNotaFiscal.Contains(x.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo))
                .Select(x => x.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)
                .Distinct();

            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>()
                .Where(x => x.Carga.Codigo == codigoCarga
                    && queryCargaPedidoXMLNotaFiscalCte.Contains(x.XMLNotaFiscal.Codigo)
                    && x.LancamentoNFSManual == null
                    && x.CargaOcorrencia == null);

            return query.Fetch(x => x.Provisao).ToList();
        }

        #endregion
    }
}
