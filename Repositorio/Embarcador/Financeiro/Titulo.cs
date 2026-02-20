using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using LinqKit;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;

namespace Repositorio.Embarcador.Financeiro
{
    public class Titulo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Titulo>
    {
        #region Construtores

        public Titulo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Titulo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorCodigoRecebidoIntegracao(int codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.CodigoRecebidoIntegracao == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }

        public bool ContemPorAcerto(int codigoAcerto)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.StatusTitulo != StatusTitulo.Cancelado);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorCTeModeloNovo(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.StatusTitulo != StatusTitulo.Cancelado && o.Documentos.Any(doc => doc.CTe.Codigo == codigoCTe));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorCodigo(int codigo, TipoTitulo tipoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.Codigo == codigo && obj.TipoTitulo == tipoTitulo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarTituloDocumentoPorCTe(int codigoCTe, bool consultarSemCancelado = true, int? codigoFatura = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
            if (codigoCTe > 0)
                query = query.Where(obj => obj.Titulo.TipoTitulo == TipoTitulo.Receber && obj.CTe.Codigo == codigoCTe);

            if (consultarSemCancelado)
                query = query.Where(obj => obj.Titulo.StatusTitulo != StatusTitulo.Cancelado);

            if (codigoFatura != null && codigoFatura.HasValue)
                query = query.Where(obj => obj.FaturaDocumento.Fatura.Codigo == codigoFatura);

            return query.Select(c => c.Titulo).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorCTe(int codigoCTe, bool consultarSemCancelado = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            if (codigoCTe > 0)
            {
                var queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
                var resultFatura = from obj in queryFatura where obj.ConhecimentoDeTransporteEletronico.Codigo == codigoCTe && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;

                if (consultarSemCancelado)
                    query = query.Where(obj => obj.StatusTitulo != StatusTitulo.Cancelado);

                query = query.Where(obj => resultFatura.Select(a => a.Fatura).Contains(obj.FaturaParcela.Fatura) || obj.Documentos.Any(o => o.CTe.Codigo == codigoCTe) || obj.ConhecimentoDeTransporteEletronico.Codigo == codigoCTe);
            }
            return query.Timeout(2000).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorCodigosComFetch(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.Fetch(o => o.Portador)
                        .Fetch(o => o.Pessoa).ThenFetch(o => o.ClientePortadorConta)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorCodigos(List<int> codigos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo tipoTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where codigos.Contains(obj.Codigo) && obj.TipoTitulo == tipoTitulo && obj.StatusTitulo == statusTitulo select obj;
            return result.ToList();
        }

        public List<double> BuscarCPFCNPJTomadorPorListaCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query.Select(o => o.Pessoa.CPF_CNPJ).Distinct().ToList();
        }

        public List<int> BuscarCodigoGrupoPessoasPorListaCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query.Select(o => o.GrupoPessoas.Codigo).Distinct().ToList();
        }

        public int QuantidadeTitulosReceber(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente && obj.Empresa.Codigo == codigoEmpresa && obj.DataEmissao.Value.Month == dataConsulta.Month && obj.DataEmissao.Value.Year == dataConsulta.Year && obj.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber && obj.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado select obj;
            return result.Count();
        }

        public int QuantidadeBoletosReceber(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.TipoAmbiente == tipoAmbiente && obj.NossoNumero != "" && obj.NossoNumero != null && obj.Empresa.Codigo == codigoEmpresa && obj.DataEmissao.Value.Month == dataConsulta.Month && obj.DataEmissao.Value.Year == dataConsulta.Year && obj.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber && obj.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorListaCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public bool ContemTitulosPagosNotaFiscal(int codigoNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigoNota && obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada select obj;
            return result.Count() > 0;
        }

        public bool ContemBoletosFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where (obj.FaturaParcela.Fatura.Codigo == codigoFatura || obj.FaturaCargaDocumento.Fatura.Codigo == codigoFatura) && obj.NossoNumero != "" && obj.NossoNumero != null select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> RetornarBoletosFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where (obj.FaturaParcela.Fatura.Codigo == codigoFatura || obj.FaturaCargaDocumento.Fatura.Codigo == codigoFatura) && obj.NossoNumero != "" && obj.NossoNumero != null select obj;
            return result.ToList();
        }

        public bool ContemTitulosPagosFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where (obj.FaturaParcela.Fatura.Codigo == codigoFatura || obj.FaturaCargaDocumento.Fatura.Codigo == codigoFatura) && obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> RetornarTitulosPagosFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where (obj.FaturaParcela.Fatura.Codigo == codigoFatura || obj.FaturaCargaDocumento.Fatura.Codigo == codigoFatura) && obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada select obj;
            return result.ToList();
        }

        public bool ContemTituloDuplicado(int codigoNota, DateTime dataVencimento, double cnpjPessoa, decimal valor, int codigo, int sequencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query
                         where obj.DataVencimento.Value.Date == dataVencimento.Date &&
                               obj.Pessoa.CPF_CNPJ == cnpjPessoa &&
                               obj.Valor == valor &&
                               obj.NotaFiscal != null &&
                               obj.NotaFiscal.Codigo == codigoNota &&
                               obj.Sequencia == sequencia
                         select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo != codigo);

            return result.Any();
        }

        public bool ContemTituloDuplicado(DateTime dataEmissao, DateTime dataVencimento, double cnpjPessoa, decimal valor, TipoTitulo tipoTitulo, int codigo, string numeroDocumentoTituloOriginal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query
                         where obj.DataEmissao.Value.Date == dataEmissao.Date &&
                               obj.DataVencimento.Value.Date == dataVencimento.Date &&
                               obj.Pessoa.CPF_CNPJ == cnpjPessoa &&
                               obj.Valor == valor &&
                               obj.TipoTitulo == tipoTitulo &&
                               obj.StatusTitulo != StatusTitulo.Cancelado && obj.StatusTitulo != StatusTitulo.Quitada
                         select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo != codigo);

            if (!string.IsNullOrWhiteSpace(numeroDocumentoTituloOriginal))
                result = result.Where(obj => obj.NumeroDocumentoTituloOriginal.Equals(numeroDocumentoTituloOriginal));

            return result.Count() > 0;
        }

        public bool ContemTituloNossoNumeroDuplicado(int codigo, string numeroBoleto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query
                         where obj.StatusTitulo != StatusTitulo.Cancelado
                         && obj.TipoTitulo == TipoTitulo.Pagar
                         select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo != codigo);

            if (!string.IsNullOrWhiteSpace(numeroBoleto))
                result = result.Where(obj => obj.NossoNumero.Equals(numeroBoleto));

            return result.Any();
        }

        public bool ContemTituloDuplicado(int codigo, double cnpjPessoa, TipoTitulo tipoTitulo, int sequencia, string tipoDocumentoTituloOriginal, string numeroDocumentoTituloOriginal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query
                         where obj.Pessoa.CPF_CNPJ == cnpjPessoa &&
                               obj.TipoTitulo == tipoTitulo &&
                               obj.Sequencia == sequencia &&
                               obj.StatusTitulo != StatusTitulo.Cancelado && obj.StatusTitulo != StatusTitulo.Quitada
                         select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo != codigo);

            if (!string.IsNullOrWhiteSpace(tipoDocumentoTituloOriginal))
                result = result.Where(obj => obj.TipoDocumentoTituloOriginal.Equals(tipoDocumentoTituloOriginal));

            if (!string.IsNullOrWhiteSpace(numeroDocumentoTituloOriginal))
                result = result.Where(obj => obj.NumeroDocumentoTituloOriginal.Equals(numeroDocumentoTituloOriginal));

            return result.Any();
        }

        public bool ContemTitulosPagosBaixaTitulo(int codigoBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.TituloBaixaNegociacao.TituloBaixa.Codigo == codigoBaixa && obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorBaixaTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.TituloBaixaNegociacao.TituloBaixa.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorPessoaTipoTitulo(double cnpjPessoa, string numeroDocumentoTituloOriginal, string tipoDocumentoTituloOriginal, TipoTitulo tipoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            query = query.Where(c => c.Pessoa.CPF_CNPJ == cnpjPessoa && c.NumeroDocumentoTituloOriginal == numeroDocumentoTituloOriginal && c.TipoDocumentoTituloOriginal == tipoDocumentoTituloOriginal && c.StatusTitulo == StatusTitulo.EmAberto && c.TipoTitulo == tipoTitulo);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorAbastecimento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.Abastecimento.Codigo == codigo && obj.StatusTitulo == StatusTitulo.EmAberto select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorBoletoRemessa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.BoletoRemessa.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro BuscarMovimentoBaixaTitulo(int codigo)
        {
            var queryBaixa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var resultBaixa = from obj in queryBaixa where obj.Titulo.Codigo == codigo select obj;

            if (resultBaixa.Count() > 0)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();
                var result = from obj in query where obj.Documento == resultBaixa.Select(o => o.TituloBaixa.Codigo).FirstOrDefault().ToString() && (obj.TipoDocumentoMovimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento || obj.TipoDocumentoMovimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento) select obj;
                return result.FirstOrDefault();
            }
            else
                return null;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTitulosArquivoPH(DateTime dataInicial, DateTime dataFinal, TipoMovimentoArquivoContabilQuestor tipoMovimento, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            query = query.Where(o => o.StatusTitulo == StatusTitulo.Quitada);

            if (dataInicial > DateTime.MinValue)
                query = query.Where(o => o.DataLiquidacao.Value.Date >= dataInicial.Date);
            if (dataFinal > DateTime.MinValue)
                query = query.Where(o => o.DataLiquidacao.Value.Date <= dataFinal.Date);
            if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasPagar)
                query = query.Where(o => o.TipoTitulo == TipoTitulo.Pagar);
            if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasReceber)
                query = query.Where(o => o.TipoTitulo == TipoTitulo.Receber);
            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);
            if (tipoAmbiente > 0)
                query = query.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorFatura(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.FaturaParcela.Fatura.Codigo == codigo || obj.FaturaCargaDocumento.Fatura.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorParcelaFatura(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.FaturaParcela.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<int> BuscarCodigosNaoCanceladosPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => obj.StatusTitulo != StatusTitulo.Cancelado && (obj.FaturaParcela.Fatura.Codigo == codigoFatura || obj.FaturaCargaDocumento.Fatura.Codigo == codigoFatura));

            return query.Select(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarTituloAReceberPorNossoNumero(string nossoNumero, string numeroBanco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.NossoNumero == nossoNumero && obj.StatusTitulo != StatusTitulo.Cancelado && obj.TipoTitulo == TipoTitulo.Receber && obj.BoletoConfiguracao.NumeroBanco == numeroBanco select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarTituloAPagarPorNossoNumero(string nossoNumero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.NossoNumero == nossoNumero && obj.StatusTitulo != StatusTitulo.Cancelado && obj.TipoTitulo == TipoTitulo.Pagar select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarTituloAPagarPorNossoNumeroIniciaCom(string nossoNumero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.NossoNumero.StartsWith(nossoNumero) && obj.StatusTitulo != StatusTitulo.Cancelado && obj.TipoTitulo == TipoTitulo.Pagar select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPrimeiroPorNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorDuplicataDocumentoEntrada(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.DuplicataDocumentoEntrada.Codigo == codigo && obj.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorGuiaDocumentoEntrada(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.DocumentoEntradaGuia.Codigo == codigo && obj.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarTodosPorDuplicataDocumentoEntrada(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.DuplicataDocumentoEntrada.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarTodosPorGuiaDocumentoEntrada(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.DocumentoEntradaGuia.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorContratoFrete(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.ContratoFrete.Codigo == codigo && obj.ContratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorContratoFrete(int codigoContratoFrete, bool adiantamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.ContratoFrete.Codigo == codigoContratoFrete && obj.Adiantado == adiantamento && obj.StatusTitulo == StatusTitulo.EmAberto select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorFornecedorNumeroEPeriodo(string numeroDocumento, double cpfCNPJFornecedor, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => obj.TipoTitulo == TipoTitulo.Pagar && obj.Pessoa.CPF_CNPJ == cpfCNPJFornecedor && obj.StatusTitulo != StatusTitulo.Cancelado && obj.StatusTitulo != StatusTitulo.Quitada);

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                query = query.Where(o => o.NumeroDocumentoTituloOriginal == numeroDocumento);

            if (dataEmissaoInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Value.Date >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Value.Date <= dataEmissaoFinal.Date);

            return query.ToList();
        }

        public DateTime? BuscarDataVencimentoContratoFrete(int codigo, int sequencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.ContratoFrete.Codigo == codigo && obj.Sequencia == sequencia && obj.StatusTitulo == StatusTitulo.EmAberto select obj;
            return result.Select(o => o.DataVencimento)?.FirstOrDefault() ?? null;
        }

        public DateTime? BuscarVencimentoMaisAntigoPorPessoaOuGrupoPessoas(int codigoGrupoPessoas, double cpfCnpjPessoa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => (o.GrupoPessoas.Codigo == codigoGrupoPessoas || o.Pessoa.CPF_CNPJ == cpfCnpjPessoa || (o.Pessoa.GrupoPessoas != null && o.Pessoa.GrupoPessoas.Codigo == codigoGrupoPessoas)) && o.StatusTitulo == StatusTitulo.EmAberto);

            return query.OrderBy("DataVencimento asc").Select(o => o.DataVencimento)?.FirstOrDefault() ?? null;
        }

        public DateTime? BuscarDataPagamentoContratoFrete(int codigo, int sequencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.ContratoFrete.Codigo == codigo && obj.Sequencia == sequencia && obj.StatusTitulo == StatusTitulo.Quitada select obj;
            return result.Select(o => o.DataLiquidacao)?.FirstOrDefault() ?? null;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTodosPorContratoFrete(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.ContratoFrete.Codigo == codigo select obj;
            return result.ToList();
        }

        public int ContarPorStatusEDocumentoEntrada(int codigoDocumentoEntrada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.DuplicataDocumentoEntrada.DocumentoEntrada.Codigo == codigoDocumentoEntrada && obj.StatusTitulo == status select obj.Codigo;
            return result.Count();
        }

        public int ContarGuiaPorStatusEDocumentoEntrada(int codigoDocumentoEntrada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.DocumentoEntradaGuia.DocumentoEntrada.Codigo == codigoDocumentoEntrada && obj.StatusTitulo == status select obj.Codigo;
            return result.Count();
        }

        public List<int> BuscarCodigosPorDataAlteracao(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.DataAlteracao > dataUltimoProcessamento && o.DataAlteracao <= dataProcessamentoAtual && o.TituloBaixaNegociacao == null && (o.Veiculos.Count > 0 || o.LancamentosCentroResultado.Count > 0));

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTodosTitulosComFiltro(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro filtrosPesquisa)
        {
            var query = ObterConsultaCadastroTituloFinanceiro(filtrosPesquisa);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = ObterConsultaCadastroTituloFinanceiro(filtrosPesquisa);

            query = query.OrderBy(propOrdenacao + " " + dirOrdenacao);

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query
                .Fetch(o => o.FaturaParcela).ThenFetch(o => o.Fatura)
                .Fetch(o => o.Pessoa)
                .Fetch(o => o.BoletoRemessa)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro filtrosPesquisa)
        {
            var query = ObterConsultaCadastroTituloFinanceiro(filtrosPesquisa);

            return query.Count();
        }

        public decimal ObterValorPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.Documentos.Any(d => d.Carga.Codigo == codigoCarga) && o.TipoTitulo == TipoTitulo.Receber && o.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado);

            return query.Sum(o => (decimal?)o.ValorOriginal) ?? 0m;
        }

        public decimal ObterValorPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.Documentos.Any(d => d.CTe.Codigo == codigoCTe) && o.TipoTitulo == TipoTitulo.Receber && o.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado);

            return query.Sum(o => (decimal?)o.ValorOriginal) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultarTituloGeracaoBoleto(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBoletoGeracao filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = ObterConsultaBoletoGeracao(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                query = query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query
                .Fetch(o => o.Pessoa)
                .Fetch(o => o.BoletoRemessa)
                .ToList();
        }

        public int ContarConsultarTituloGeracaoBoleto(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBoletoGeracao filtrosPesquisa)
        {
            var query = ObterConsultaBoletoGeracao(filtrosPesquisa);

            if (query.Count() > 0)
                return query.Count();
            else
                return 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultarTituloGeracaoBoleto(List<int> codigosTitulos, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto select obj;

            if (codigosTitulos.Count > 0)
                result = result.Where(obj => codigosTitulos.Contains(obj.Codigo));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarTituloGeracaoBoleto(List<int> codigosTitulos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto select obj;

            if (codigosTitulos.Count > 0)
                result = result.Where(obj => codigosTitulos.Contains(obj.Codigo));

            if (result.Count() > 0)
                return result.Count();
            else
                return 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultaTitulosPendentes(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.StatusTitulo != StatusTitulo.Cancelado select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            var queryAgrupado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var resultAgrupado = from obj in queryAgrupado where obj.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada select obj;
            result = result.Where(obj => !resultAgrupado.Where(a => a.Titulo != null).Any(a => a.Titulo.Codigo == obj.Codigo));

            result = result.Fetch(obj => obj.Pessoa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContaConsultaTitulosPendentes(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.StatusTitulo != StatusTitulo.Cancelado select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            var queryAgrupado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var resultAgrupado = from obj in queryAgrupado where obj.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada select obj;
            result = result.Where(obj => !resultAgrupado.Where(a => a.Titulo != null).Any(a => a.Titulo.Codigo == obj.Codigo));

            result = result.Fetch(obj => obj.Pessoa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultarTitulosPendentes(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = ObterConsultaTitulosPendentes(filtrosPesquisa);

            result = result.Fetch(obj => obj.Pessoa);

            return ObterLista(result, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar && o.DuplicataDocumentoEntrada.DocumentoEntrada.Codigo == codigoDocumentoEntrada && o.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto);

            return query.ToList();
        }

        public List<int> BuscarCodigosPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.TipoTitulo == TipoTitulo.Pagar && o.DuplicataDocumentoEntrada.DocumentoEntrada.Codigo == codigoDocumentoEntrada);

            return query.Select(o => o.Codigo).ToList();
        }

        public int ContarTitulosPendentes(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa)
        {
            var result = ObterConsultaTitulosPendentes(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ObterDocumentosSelecionadosTitulosAPagar(bool selecionarTodos, List<int> codigosTitulos, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa)
        {
            var query = ObterConsultaTitulosPendentes(filtrosPesquisa);

            if (selecionarTodos)
                query = query.Where(o => !codigosTitulos.Contains(o.Codigo));
            else
                query = query.Where(o => codigosTitulos.Contains(o.Codigo));

            return query.ToList();
        }

        public List<int> ObterCodigosTitulosAPagar(bool selecionarTodos, List<int> codigosTitulos, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa)
        {
            var query = ObterConsultaTitulosPendentes(filtrosPesquisa);

            if (selecionarTodos)
                query = query.Where(o => !codigosTitulos.Contains(o.Codigo));
            else
                query = query.Where(o => codigosTitulos.Contains(o.Codigo));

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultarTitulosAReceberPendentesParaBordero(double cnpjPessoa, int codigoGrupoPessoa, int numeroCTe, string numeroCarga, int codigoTitulo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = ObterConsultaTitulosAReceberPendentesParaBordero(cnpjPessoa, codigoGrupoPessoa, numeroCTe, numeroCarga, codigoTitulo);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsultaTitulosAReceberPendentesParaBordero(double cnpjPessoa, int codigoGrupoPessoa, int numeroCTe, string numeroCarga, int codigoTitulo)
        {
            var query = ObterConsultaTitulosAReceberPendentesParaBordero(cnpjPessoa, codigoGrupoPessoa, numeroCTe, numeroCarga, codigoTitulo);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultarTitulosAReceberPendentes(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = ObterConsultaTitulosAReceberPendentes(filtrosPesquisa);

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaTitulosAReceberPendentes(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = ObterConsultaTitulosAReceberPendentes(filtrosPesquisa);

            return query.Count();
        }

        public decimal ObterValorTotalPendenteTitulosAReceber(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = ObterConsultaTitulosAReceberPendentes(filtrosPesquisa);

            if (filtrosPesquisa.SelecionarTodos)
                query = query.Where(o => !filtrosPesquisa.CodigosTitulos.Contains(o.Codigo));
            else
                query = query.Where(o => filtrosPesquisa.CodigosTitulos.Contains(o.Codigo));

            return query.Sum(o => (decimal?)o.ValorOriginal) ?? 0m;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Financeiro.AgrupamentoValoresTitulo> ObterDetalhesTitulosReceberPendentes(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = ObterConsultaTitulosAReceberPendentes(filtroPesquisa);

            if (filtroPesquisa.SelecionarTodos)
                query = query.Where(o => !filtroPesquisa.CodigosTitulos.Contains(o.Codigo));
            else
                query = query.Where(o => filtroPesquisa.CodigosTitulos.Contains(o.Codigo));

            return query.GroupBy(o => new { o.MoedaCotacaoBancoCentral }).Select(o => new Dominio.ObjetosDeValor.Embarcador.Financeiro.AgrupamentoValoresTitulo { Moeda = o.Key.MoedaCotacaoBancoCentral, ValorOriginal = (decimal?)o.Sum(t => t.ValorOriginal) ?? 0m, ValorOriginalMoeda = (decimal?)o.Sum(t => t.ValorOriginalMoedaEstrangeira) ?? 0m }).ToList();
        }

        public List<int> ObterCodigosTitulosAReceber(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = ObterConsultaTitulosAReceberPendentes(filtroPesquisa);

            if (filtroPesquisa.SelecionarTodos)
                query = query.Where(o => !filtroPesquisa.CodigosTitulos.Contains(o.Codigo));
            else
                query = query.Where(o => filtroPesquisa.CodigosTitulos.Contains(o.Codigo));

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DetalheTituloGeracaoBaixa> ObterDetalhesTitulosAReceberGeracaoBaixa(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = ObterConsultaTitulosAReceberPendentes(filtroPesquisa);

            if (filtroPesquisa.SelecionarTodos)
                query = query.Where(o => !filtroPesquisa.CodigosTitulos.Contains(o.Codigo));
            else
                query = query.Where(o => filtroPesquisa.CodigosTitulos.Contains(o.Codigo));

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Financeiro.DetalheTituloGeracaoBaixa() { Codigo = o.Codigo, Moeda = o.MoedaCotacaoBancoCentral }).ToList();
        }

        public void RemoverTituloDoMovimentoFinanceiro(int codigo, UnitOfWork unitOfWork)
        {
            var query = this.SessionNHiBernate.CreateQuery("UPDATE MovimentoFinanceiro movimento SET movimento.Titulo = null WHERE movimento.Titulo.Codigo = :codigoTitulo").SetInt32("codigoTitulo", codigo);

            query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultaAlteracaoBoleto(int codigoAlteracao, double cnpjPessoa, int codigoEmpresa, DateTime vencimentoInicial, DateTime vencimentoFinal, DateTime emissaoInicial, DateTime emissaoFinal, int codigoBoletoConfiguracao, TipoAmbiente tipoAmbiente, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = ObterConsultaAlteracaoBoleto(codigoAlteracao, cnpjPessoa, codigoEmpresa, vencimentoInicial, vencimentoFinal, emissaoInicial, emissaoFinal, codigoBoletoConfiguracao, tipoAmbiente);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAlteracaoBoleto(int codigoAlteracao, double cnpjPessoa, int codigoEmpresa, DateTime vencimentoInicial, DateTime vencimentoFinal, DateTime emissaoInicial, DateTime emissaoFinal, int codigoBoletoConfiguracao, TipoAmbiente tipoAmbiente)
        {
            var query = ObterConsultaAlteracaoBoleto(codigoAlteracao, cnpjPessoa, codigoEmpresa, vencimentoInicial, vencimentoFinal, emissaoInicial, emissaoFinal, codigoBoletoConfiguracao, tipoAmbiente);

            return query.Count();
        }

        public decimal TituloAReceberEmAtraso(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataVencimentoInicial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProvisaoPesquisaTitulo provisaoPesquisaTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => obj.DataVencimento.Value.Date < dataVencimentoInicial.Date);
            query = query.Where(obj => obj.TipoTitulo == TipoTitulo.Receber);
            query = query.Where(obj => obj.StatusTitulo == StatusTitulo.EmAberto);

            if (codigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (tipoAmbiente > 0)
                query = query.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            if (provisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SemProvisao)
                query = query.Where(obj => obj.Provisao == false);
            else if (provisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SomenteProvisao)
                query = query.Where(obj => obj.Provisao == true);

            return query.ToList().Sum(o => o.ValorPendente);
        }

        public decimal TituloAPagarEmAtraso(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataVencimentoInicial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProvisaoPesquisaTitulo provisaoPesquisaTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => obj.DataVencimento.Value.Date < dataVencimentoInicial.Date);
            query = query.Where(obj => obj.TipoTitulo == TipoTitulo.Pagar);
            query = query.Where(obj => obj.StatusTitulo == StatusTitulo.EmAberto);

            if (codigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (tipoAmbiente > 0)
                query = query.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            if (provisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SemProvisao)
                query = query.Where(obj => obj.Provisao == false);
            else if (provisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SomenteProvisao)
                query = query.Where(obj => obj.Provisao == true);

            return query.ToList().Sum(o => o.ValorPendente);
        }

        public void LiberarPagamentosPorPagamento(int pagamento)
        {
            UnitOfWork.Sessao.CreateQuery("update Titulo obj set obj.Pagamento = null WHERE obj.Pagamento.Codigo = :Pagamento")
                             .SetInt32("Pagamento", pagamento)
                             .ExecuteUpdate();
        }

        public void LiberarTitulosPagamento(int pagamento)
        {
            string hql = "update Titulo titulo set titulo.LiberadoPagamento = :LiberadoPagamento where titulo.Pagamento = :Pagamento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetBoolean("LiberadoPagamento", true);
            query.SetInt32("Pagamento", pagamento);

            query.ExecuteUpdate();
        }

        public void QuitarTitulosPagamento(int pagamento, DateTime dataLiquidacao)
        {
            string hql = "update Titulo titulo set titulo.StatusTitulo = :StatusTitulo, titulo.DataLiquidacao = :DataLiquidacao where titulo.Pagamento = :Pagamento ";
            var query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("StatusTitulo", (int)StatusTitulo.Quitada);
            query.SetDateTime("DataLiquidacao", dataLiquidacao);
            query.SetInt32("Pagamento", pagamento);

            query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTituloPendentePorCTe(string cnpjEmissor, int numero, int serie)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber &&
                                     o.StatusTitulo == StatusTitulo.EmAberto &&
                                     !o.ModeloAntigo &&
                                     !o.Baixas.Any(b => b.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Cancelada) &&
                                     o.Documentos.Any(doc => doc.CTe.Empresa.CNPJ == cnpjEmissor && doc.CTe.Numero == numero && doc.CTe.Serie.Numero == serie));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTituloPendentePorNumeroDocumento(double cnpjPessoaCliente, string numeroDocumento, decimal valor, int parcela)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber &&
                                     o.StatusTitulo == StatusTitulo.EmAberto &&
                                     !o.Baixas.Any(b => b.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Cancelada) &&
                                     o.Pessoa.CPF_CNPJ == cnpjPessoaCliente && o.NumeroDocumentoTituloOriginal == numeroDocumento);

            if (valor > 0)
                query = query.Where(o => o.ValorPendente == valor);

            if (parcela > 0)
                query = query.Where(o => o.Sequencia == parcela);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTituloReceberPendentePorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber &&
                                     o.StatusTitulo == StatusTitulo.EmAberto &&
                                     !o.ModeloAntigo &&
                                     !o.Baixas.Any(b => b.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Cancelada) &&
                                     o.Codigo == codigo);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTituloPagarPendentePorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar &&
                                     o.StatusTitulo != StatusTitulo.Quitada &&
                                     o.StatusTitulo != StatusTitulo.Cancelado &&
                                     !o.ModeloAntigo &&
                                     o.Codigo == codigo);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTituloPendentePorFatura(int numeroFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber &&
                                     o.StatusTitulo == StatusTitulo.EmAberto &&
                                     !o.ModeloAntigo &&
                                     !o.Baixas.Any(b => b.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Cancelada) &&
                                     (o.FaturaParcela.Fatura.Numero == numeroFatura || o.FaturaDocumento.Fatura.Numero == numeroFatura));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTituloPendentePorTitulo(string razaoSocial, DateTime vencimento, decimal valor)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber &&
                                     o.StatusTitulo == StatusTitulo.EmAberto &&
                                     !o.ModeloAntigo &&
                                     !o.Baixas.Any(b => b.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Cancelada) &&
                                     o.Documentos.Any(doc => doc.CTe.Empresa.NomeCNPJ == razaoSocial && doc.CTe.Titulo.Valor == valor && doc.CTe.Titulo.DataVencimento == vencimento));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorFaturaDocumento(int codigoFaturaDocumento, StatusTitulo situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.FaturaDocumento.Codigo == codigoFaturaDocumento && o.StatusTitulo == situacao);

            return query.ToList();
        }

        public void AjustarTitulosCancelamentoCarga(int codigoFaturaDocumento, StatusTitulo situacao)
        {
            var query = this.SessionNHiBernate.CreateQuery("UPDATE Titulo titulo SET titulo.FaturaDocumento = null WHERE titulo.FaturaDocumento = :codigoFaturaDocumento AND titulo.StatusTitulo = :situacao");

            query.SetInt32("codigoFaturaDocumento", codigoFaturaDocumento);
            query.SetEnum("situacao", situacao);

            query.ExecuteUpdate();
        }

        public void InformarDataAutorizacao(List<int> codigos, DateTime dataAutorizacao)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE Titulo SET DataAutorizacao = :dataAutorizacao WHERE Codigo IN (:codigos)")
                .SetParameterList("codigos", codigos)
                .SetParameter("dataAutorizacao", dataAutorizacao)
                .ExecuteUpdate();
        }

        public void RemoverDataAutorizacao(List<int> codigos)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE Titulo SET DataAutorizacao = null WHERE Codigo IN (:codigos)")
                .SetParameterList("codigos", codigos)
                .ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultarAutorizacaoPagamento(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = ConsultarAutorizacaoPagamento(filtrosPesquisa);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.ToList();
        }

        public int ContarConsultaAutorizacaoPagamento(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento filtrosPesquisa)
        {
            var result = ConsultarAutorizacaoPagamento(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ObterDocumentosSelecionadosAutorizacaoPagamento(bool selecionarTodos, List<int> codigosTitulos, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento filtrosPesquisa)
        {
            var query = ConsultarAutorizacaoPagamento(filtrosPesquisa);

            if (selecionarTodos)
                query = query.Where(o => !codigosTitulos.Contains(o.Codigo));
            else
                query = query.Where(o => codigosTitulos.Contains(o.Codigo));

            return query.ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.BoletoPendenteEnvioEmail> BuscarBoletosPendenteEnvio()
        {
            string sql = @"SELECT TIT_CODIGO Codigo 
                            FROM T_TITULO 
                            WHERE BCF_CODIGO IS NOT NULL 
                            AND (TIT_BOLETO_ENVIADO_POR_EMAIL = 0 OR TIT_BOLETO_ENVIADO_POR_EMAIL IS NULL) 
                            AND ((TIT_STATUS_BOLETO = 2 AND TIT_BOLETO_GERADO_AUTOMATICAMENTE = 1) OR TIT_ENVIAR_DOCUMENTACAO_FATURAMENTO_CTE = 1)";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.BoletoPendenteEnvioEmail)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.BoletoPendenteEnvioEmail>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.CobrancaPendenteEnviarEmail> BuscarCobrancaPendenteEnvio()
        {
            string sql = @"select t.tit_codigo as Codigo
                             from( 
	                            select t.tit_codigo 
	                                 , t.tit_data_envio_email_cobranca
		                             , datediff(day,cast(t.tit_data_vencimento as date), cast(getdate() as date)) as dias
		                             , case 
		  	                             when coalesce(p.cli_nao_usar_configuracao_fatura_grupo, 0) = 1 and p.CLI_COBRANCA_HABILITAR_CONFIGURACAO_PERSONALIZADA = 1 then p.CLI_COBRANCA_QUNATIDADE_DIAS
		 	                             when (coalesce(p.cli_nao_usar_configuracao_fatura_grupo, 0) <> 1 or coalesce(p.CLI_COBRANCA_HABILITAR_CONFIGURACAO_PERSONALIZADA, 0) <> 1) and gp.GRP_COBRANCA_HABILITAR_CONFIGURACAO_PERSONALIZADA = 1 then gp.GRP_COBRANCA_QUNATIDADE_DIAS
		                                 else coalesce(c.EEC_COBRANCA_QUNATIDADE_DIAS, 0)
		                               end as dias_para_envio 
		                             , case 
		  	                             when coalesce(p.cli_nao_usar_configuracao_fatura_grupo, 0) = 1 and p.CLI_COBRANCA_HABILITAR_CONFIGURACAO_PERSONALIZADA = 1 and p.CLI_COBRANCA_NAO_ENVIAR_EMAIL = 1 then 0
		 	                             when (coalesce(p.cli_nao_usar_configuracao_fatura_grupo, 0) <> 1 or coalesce(p.CLI_COBRANCA_HABILITAR_CONFIGURACAO_PERSONALIZADA, 0) <> 1) and gp.GRP_COBRANCA_HABILITAR_CONFIGURACAO_PERSONALIZADA = 1 and gp.GRP_COBRANCA_NAO_ENVIAR_EMAIL = 1 then 0
		                                 else 1
		                               end as enviar 
	                              from t_titulo t
	                             inner join t_cliente p 
	                                on p.cli_cgccpf = t.cli_cgccpf
	                              left join t_grupo_pessoas gp
	                                on gp.grp_codigo = t.grp_codigo
	                             cross join t_configuracao_envio_email_cobranca c 
	                             where t.tit_tipo = 1
	                               and t.tit_status in (1,2,5)
                             ) as t
                             where t.TIT_DATA_ENVIO_EMAIL_COBRANCA is null
                               and t.enviar = 1 
                               and t.dias >= t.dias_para_envio
                               and t.dias >= 0";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.CobrancaPendenteEnviarEmail)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.CobrancaPendenteEnviarEmail>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.CobrancaPendenteEnviarEmail> BuscarAvisoVencimentoPendenteEnvio()
        {
            string sql = @"select t.tit_codigo as Codigo
                             from( 
	                            select t.tit_codigo 
	                                 , t.tit_data_envio_email_aviso_vencimento
		                             , datediff(day,cast(getdate() as date), cast(t.tit_data_vencimento as date)) as dias
		                             , case 
		  	                             when coalesce(p.cli_nao_usar_configuracao_fatura_grupo, 0) = 1 and p.cli_aviso_vencimeto_habilitar_configuracao_personalizada = 1  then coalesce(p.cli_aviso_vencimeto_enviar_diariamente, 0)
		 	                             when (coalesce(p.cli_nao_usar_configuracao_fatura_grupo, 0) <> 1 or coalesce(p.cli_aviso_vencimeto_habilitar_configuracao_personalizada, 0) <> 1) and gp.grp_aviso_vencimeto_habilitar_configuracao_personalizada = 1 then coalesce(gp.grp_aviso_vencimeto_enviar_diariamente, 0)
			                             else coalesce(c.eec_aviso_vencimeto_enviar_diariamente, 0)
		                               end as enviar_diariamente
		                             , case 
		  	                             when coalesce(p.cli_nao_usar_configuracao_fatura_grupo, 0) = 1 and p.cli_aviso_vencimeto_habilitar_configuracao_personalizada = 1 then p.cli_aviso_vencimeto_qunatidade_dias
		 	                             when (coalesce(p.cli_nao_usar_configuracao_fatura_grupo, 0) <> 1 or coalesce(p.cli_aviso_vencimeto_habilitar_configuracao_personalizada, 0) <> 1) and gp.grp_aviso_vencimeto_habilitar_configuracao_personalizada = 1 then gp.grp_aviso_vencimeto_qunatidade_dias
		                                 else coalesce(c.eec_aviso_vencimeto_qunatidade_dias, 0)
		                               end as dias_para_envio 
		                             , case 
		  	                             when coalesce(p.cli_nao_usar_configuracao_fatura_grupo, 0) = 1 and p.cli_aviso_vencimeto_habilitar_configuracao_personalizada = 1 and p.cli_aviso_vencimeto_nao_enviar_email = 1 then 0
		 	                             when (coalesce(p.cli_nao_usar_configuracao_fatura_grupo, 0) <> 1 or coalesce(p.cli_aviso_vencimeto_habilitar_configuracao_personalizada, 0) <> 1) and gp.grp_aviso_vencimeto_habilitar_configuracao_personalizada = 1 and gp.grp_aviso_vencimeto_nao_enviar_email = 1 then 0
		                                 else 1
		                               end as enviar 
	                              from t_titulo t
	                             inner join t_cliente p 
	                                on p.cli_cgccpf = t.cli_cgccpf
	                              left join t_grupo_pessoas gp
	                                on gp.grp_codigo = t.grp_codigo
	                             cross join t_configuracao_envio_email_cobranca c 
	                             where t.tit_tipo = 1
	                               and t.tit_status = 1
                             ) as t
                             where (t.tit_data_envio_email_aviso_vencimento is null or (t.enviar_diariamente = 1 and cast(getdate() as date) <> cast(t.tit_data_envio_email_aviso_vencimento as date)))
                               and t.enviar = 1 
                               and t.dias <= t.dias_para_envio
                               and t.dias >= 0";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.CobrancaPendenteEnviarEmail)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.CobrancaPendenteEnviarEmail>();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> TitulosAReceberEmAtraso(DateTime dataVencimento, int codigoGrupoPessoa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => obj.DataVencimento.Value.Date < dataVencimento.Date);
            query = query.Where(obj => obj.TipoTitulo == TipoTitulo.Receber && obj.StatusTitulo == StatusTitulo.EmAberto && obj.Pessoa != null && (obj.GrupoPessoas.Codigo == codigoGrupoPessoa || obj.Pessoa.GrupoPessoas.Codigo == codigoGrupoPessoa));

#if DEBUG
            query = query.Where(obj => obj.DataEmissao.Value.Date > DateTime.Now.Date.AddDays(-30));//Para não trazer mais de mil registros da base de teste
#endif

            return query
                .Fetch(o => o.Pessoa)
                .ToList();
        }

        public bool ExisteTituloQuitadoPorFatura(int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.StatusTitulo == StatusTitulo.Quitada && o.FaturaParcela.Fatura.Codigo == codigoFatura);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultarTitulosPagarPendenteIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, string codigoIntegracaoTipoMovimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => ((bool?)obj.Integrado ?? false) == false && obj.TipoTitulo == TipoTitulo.Pagar);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoTipoMovimento))
                query = query.Where(obj => obj.TipoMovimento.CodigoIntegracao == codigoIntegracaoTipoMovimento);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaTitulosPagarPendenteIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => ((bool?)obj.Integrado ?? false) == false && obj.TipoTitulo == TipoTitulo.Pagar);

            return query.Count();
        }

        public string ObterSequenciaComParcelaTitulo(int codigoTitulo)
        {
            string sql = @"SELECT CAST(Titulo.TIT_SEQUENCIA AS NVARCHAR(20)) +
				                    CASE
                                        WHEN Titulo.TBN_CODIGO IS NOT NULL THEN '/' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.TBN_CODIGO = Titulo.TBN_CODIGO)
                                        WHEN Titulo.TDD_CODIGO IS NOT NULL THEN '/' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.TDD_CODIGO = Titulo.TDD_CODIGO)
                                        WHEN Titulo.CFT_CODIGO IS NOT NULL THEN '/' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.CFT_CODIGO = Titulo.CFT_CODIGO)
                                        WHEN Titulo.NFI_CODIGO IS NOT NULL THEN '/' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.NFI_CODIGO = Titulo.NFI_CODIGO)
                                        WHEN Titulo.FAP_CODIGO IS NOT NULL THEN '/' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.FAP_CODIGO = Titulo.FAP_CODIGO) 
                                        WHEN Titulo.CMA_CODIGO IS NOT NULL THEN '/' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.CMA_CODIGO = Titulo.CMA_CODIGO) 
                                        WHEN ContratoFinanciamentoParcela.TIT_CODIGO IS NOT NULL THEN '/' + 
                                            (SELECT CAST(MAX(CFP.CFP_SEQUENCIA) AS NVARCHAR(20)) FROM T_CONTRATO_FINANCIAMENTO_PARCELA CFP 
                                            JOIN T_CONTRATO_FINANCIAMENTO CF ON CF.CFI_CODIGO = CFP.CFI_CODIGO
                                            WHERE CF.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO) 
                                        ELSE '/1'
                                    END
                        FROM T_TITULO Titulo
                        LEFT OUTER JOIN T_CONTRATO_FINANCIAMENTO_PARCELA ContratoFinanciamentoParcela on ContratoFinanciamentoParcela.TIT_CODIGO = Titulo.TIT_CODIGO
                        LEFT OUTER JOIN T_CONTRATO_FINANCIAMENTO ContratoFinanciamento on ContratoFinanciamento.CFI_CODIGO = ContratoFinanciamentoParcela.CFI_CODIGO
                        WHERE Titulo.TIT_CODIGO = " + codigoTitulo;

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.UniqueResult<string>();
        }

        public bool BuscarTitulosQuitadosPorSinistroParcela(int codigoSinistroParcela)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => o.StatusTitulo == StatusTitulo.Quitada && o.SinistroParcela.Codigo == codigoSinistroParcela);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTitulosPorSinistroParcela(List<int> codigosSinistroParcela)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(o => codigosSinistroParcela.Contains(o.SinistroParcela.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultarTitulosPorPessoa(double cnpjPessoa, int codigoEmpresaPai, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = ObterConsultaPorPessoa(cnpjPessoa, codigoEmpresaPai);

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query.OrderBy(propOrdena + " " + dirOrdena).ToList();
        }

        public int ContarConsultarTitulosPorPessoa(double cnpjPessoa, int codigoEmpresaPai)
        {
            var query = ObterConsultaPorPessoa(cnpjPessoa, codigoEmpresaPai);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo BuscarPorPortalClienteCodigo(string portalClienteCodigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.PortalClienteCodigo == portalClienteCodigo select obj;
            return result.FirstOrDefault();
        }

        public bool PortalClienteCodigoJaExistente(string portalClienteCodigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.PortalClienteCodigo == portalClienteCodigo select obj;
            return result.Any();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> ObterConsultaPorPessoa(double cnpjPessoa, int codigoEmpresaPai)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => obj.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber && obj.StatusTitulo != StatusTitulo.Cancelado);

            if (cnpjPessoa > 0d)
                query = query.Where(obj => obj.Pessoa.CPF_CNPJ == cnpjPessoa);

            if (codigoEmpresaPai > 0)
                query = query.Where(obj => (obj.Empresa.Codigo == codigoEmpresaPai || obj.Empresa.EmpresaPai.Codigo == codigoEmpresaPai));

            return query;
        }


        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultarAutorizacaoPagamento(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            var result = from obj in query where obj.TipoTitulo == TipoTitulo.Pagar && obj.StatusTitulo == StatusTitulo.EmAberto select obj;

            if (filtrosPesquisa.CodigoPagamentoEletronico > 0)
            {
                var queryPagamentoEletronico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
                queryPagamentoEletronico = queryPagamentoEletronico.Where(p => p.PagamentoEletronico.Codigo == filtrosPesquisa.CodigoPagamentoEletronico);

                result = result.Where(obj => queryPagamentoEletronico.Any(c => c.Titulo == obj));
            }

            if (filtrosPesquisa.Fornecedor > 0)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == filtrosPesquisa.Fornecedor);

            if (filtrosPesquisa.DataEmissaoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date >= filtrosPesquisa.DataEmissaoInicial.Date);

            if (filtrosPesquisa.DataEmissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date <= filtrosPesquisa.DataEmissaoFinal.Date);

            if (filtrosPesquisa.DataVencimentoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimento.Value.Date >= filtrosPesquisa.DataVencimentoInicial.Date);

            if (filtrosPesquisa.DataVencimentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimento.Value.Date <= filtrosPesquisa.DataVencimentoFinal.Date);

            if (filtrosPesquisa.NumeroTitulo > 0)
                result = result.Where(obj => obj.Codigo == filtrosPesquisa.NumeroTitulo);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
                result = result.Where(obj => obj.NumeroDocumentoTituloOriginal.Contains(filtrosPesquisa.NumeroDocumento));

            if (filtrosPesquisa.SituacaoAutorizacao != SituacaoAutorizacao.Todos)
            {
                if (filtrosPesquisa.SituacaoAutorizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacao.PendenteAutorizacao)
                    result = result.Where(obj => obj.DataAutorizacao == null);

                if (filtrosPesquisa.SituacaoAutorizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacao.SomenteAutorizados)
                    result = result.Where(obj => obj.DataAutorizacao != null);
            }

            if (filtrosPesquisa.TipoTituloNegociacao != TipoTituloNegociacao.Todos)
            {
                if (filtrosPesquisa.TipoTituloNegociacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTituloNegociacao.Negociacao)
                    result = result.Where(obj => obj.TituloBaixaNegociacao != null);
                if (filtrosPesquisa.TipoTituloNegociacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTituloNegociacao.Originais)
                    result = result.Where(obj => obj.TituloBaixaNegociacao == null);
            }

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
                result = result.Where(obj => obj.TipoMovimento.Codigo == filtrosPesquisa.CodigoTipoMovimento);

            if (filtrosPesquisa.CodigoCentroResultado > 0)
                result = result.Where(obj => obj.LancamentosCentroResultado.Any(b => b.CentroResultado.Codigo == filtrosPesquisa.CodigoCentroResultado));

            if (filtrosPesquisa.TiposDocumento?.Count > 0)
            {
                Expression<Func<Dominio.Entidades.Embarcador.Financeiro.Titulo, bool>> tiposDeDocumentos = PredicateBuilder.False<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

                IQueryable<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela> queryContratoFinanciamentoParcela = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();
                IQueryable<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela> resultContratoFinanciamentoParcela = from obj in queryContratoFinanciamentoParcela select obj;

                foreach (TipoDocumentoPesquisaTitulo TipoDocumento in filtrosPesquisa.TiposDocumento)
                {
                    switch (TipoDocumento)
                    {
                        case TipoDocumentoPesquisaTitulo.ContratoFrete:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.ContratoFrete != null);
                            break;
                        case TipoDocumentoPesquisaTitulo.DocumentoEntrada:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.DuplicataDocumentoEntrada != null);
                            break;
                        case TipoDocumentoPesquisaTitulo.Fatura:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.FaturaParcela != null || obj.FaturaCargaDocumento != null);
                            break;
                        case TipoDocumentoPesquisaTitulo.Negociacao:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.TituloBaixaNegociacao != null);
                            break;
                        case TipoDocumentoPesquisaTitulo.NotaFiscal:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.NotaFiscal != null);
                            break;
                        case TipoDocumentoPesquisaTitulo.ContratoFinanciamento:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => resultContratoFinanciamentoParcela.Any(a => a.Titulo == obj));
                            break;
                        case TipoDocumentoPesquisaTitulo.Outros:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.NotaFiscal == null && obj.TituloBaixaNegociacao == null && obj.FaturaParcela == null && obj.FaturaCargaDocumento == null && obj.DuplicataDocumentoEntrada == null && obj.ContratoFrete == null && !resultContratoFinanciamentoParcela.Any(a => a.Titulo == obj));
                            break;
                    }
                }

                result = result.Where(tiposDeDocumentos);
            }

            if (filtrosPesquisa.SituacaoBoletoTitulo != SituacaoBoletoTitulo.Todos)
            {
                if (filtrosPesquisa.SituacaoBoletoTitulo == SituacaoBoletoTitulo.ComBoleto)
                    result = result.Where(obj => obj.NossoNumero != "" && obj.NossoNumero != null);
                else if (filtrosPesquisa.SituacaoBoletoTitulo == SituacaoBoletoTitulo.SemBoleto)
                    result = result.Where(obj => obj.NossoNumero == "" || obj.NossoNumero == null);
            }

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                result = result.Where(obj => obj.Pessoa.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> ObterConsultaTitulosAReceberPendentes(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloReceberPendente filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => obj.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber && !obj.ModeloAntigo && obj.Documentos.Count > 0);

            if (filtroPesquisa.CNPJPessoa > 0)
                query = query.Where(obj => obj.Pessoa.CPF_CNPJ == filtroPesquisa.CNPJPessoa);

            if (filtroPesquisa.CodigoFatura > 0)
                query = query.Where(obj => obj.FaturaParcela.Fatura.Codigo == filtroPesquisa.CodigoFatura);

            if (filtroPesquisa.CodigoGrupoPessoa > 0)
                query = query.Where(obj => obj.GrupoPessoas.Codigo == filtroPesquisa.CodigoGrupoPessoa || obj.Pessoa.GrupoPessoas.Codigo == filtroPesquisa.CodigoGrupoPessoa);

            if (filtroPesquisa.CodigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == filtroPesquisa.CodigoEmpresa);

            if (filtroPesquisa.SomenteTitulosDeNegociacao == Dominio.Enumeradores.OpcaoSimNao.Sim)
                query = query.Where(obj => obj.TituloBaixaNegociacao != null);
            else if (filtroPesquisa.SomenteTitulosDeNegociacao == Dominio.Enumeradores.OpcaoSimNao.Nao)
                query = query.Where(obj => obj.TituloBaixaNegociacao == null);

            if (filtroPesquisa.CodigoConhecimento.Count() > 0)
                query = query.Where(o => o.Documentos.Any(d => d.CTe.Codigo > 0 && filtroPesquisa.CodigoConhecimento.Contains(d.CTe.Codigo)));

            if (filtroPesquisa.CodigoCarga > 0)
                query = query.Where(o => o.Documentos.Any(d => d.Carga.Codigo == filtroPesquisa.CodigoCarga) || o.Documentos.Any(d => d.CTe.CargaCTes.Any(c => c.Carga.Codigo == filtroPesquisa.CodigoCarga)));

            if (long.TryParse(filtroPesquisa.NumeroPedido, out long vNumeroPedido) && vNumeroPedido > 0L)
            {
                query = query.Where(o => o.Documentos.Any(doc => doc.Carga.Pedidos.Any(ped => ped.Pedido.NumeroPedidoEmbarcador.Contains(filtroPesquisa.NumeroPedido) ||
                                                                 doc.Carga.IntegracoesAvon.Any(avon => avon.NumeroMinuta == vNumeroPedido) ||
                                                                 doc.Carga.IntegracoesNatura.Any(natura => natura.DocumentoTransporte.Numero == vNumeroPedido) ||
                                                                 doc.CTe.CargaCTes.Any(cte => cte.Carga.Pedidos.Any(pe => pe.Pedido.NumeroPedidoEmbarcador.Contains(filtroPesquisa.NumeroPedido)) ||
                                                                                              cte.Carga.IntegracoesAvon.Any(av => av.NumeroMinuta == vNumeroPedido) ||
                                                                                              cte.Carga.IntegracoesNatura.Any(natu => natu.DocumentoTransporte.Numero == vNumeroPedido)))));
            }
            else if (!string.IsNullOrWhiteSpace(filtroPesquisa.NumeroPedido))
            {
                query = query.Where(o => o.Documentos.Any(doc => doc.Carga.Pedidos.Any(ped => ped.Pedido.NumeroPedidoEmbarcador.Contains(filtroPesquisa.NumeroPedido) ||
                                                                 doc.CTe.CargaCTes.Any(cte => cte.Carga.Pedidos.Any(pe => pe.Pedido.NumeroPedidoEmbarcador.Contains(filtroPesquisa.NumeroPedido))))));
            }

            if (int.TryParse(filtroPesquisa.NumeroOcorrenciaCliente, out int iNumeroOcorrencia) && iNumeroOcorrencia > 0)
            {
                query = query.Where(o => o.Documentos.Any(oco => oco.Carga.Ocorrencias.Any(x => x.NumeroOcorrencia == iNumeroOcorrencia || x.NumeroOcorrenciaCliente.Contains(filtroPesquisa.NumeroOcorrenciaCliente)) ||
                                                                 oco.CTe.CargaCTeOcorrencias.Any(z => z.CargaOcorrencia.NumeroOcorrencia == iNumeroOcorrencia || z.CargaOcorrencia.NumeroOcorrenciaCliente.Contains(filtroPesquisa.NumeroOcorrenciaCliente))));

            }
            else if (!string.IsNullOrWhiteSpace(filtroPesquisa.NumeroOcorrenciaCliente))
            {
                query = query.Where(o => o.Documentos.Any(oco => oco.Carga.Ocorrencias.Any(x => x.NumeroOcorrenciaCliente.Contains(filtroPesquisa.NumeroOcorrenciaCliente)) ||
                                                                 oco.CTe.CargaCTeOcorrencias.Any(z => z.CargaOcorrencia.NumeroOcorrenciaCliente.Contains(filtroPesquisa.NumeroOcorrenciaCliente))));
            }

            if (filtroPesquisa.NumeroDocumentoOriginario > 0)
                query = query.Where(o => o.Documentos.Any(docs => docs.CTe.DocumentosOriginarios.Any(doc => doc.Numero == filtroPesquisa.NumeroDocumentoOriginario)));

            if (filtroPesquisa.DataProgramacaoPagamentoInicial != DateTime.MinValue)
                query = query.Where(obj => obj.DataProgramacaoPagamento.Value.Date >= filtroPesquisa.DataProgramacaoPagamentoInicial.Date);

            if (filtroPesquisa.DataProgramacaoPagamentoFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataProgramacaoPagamento.Value.Date < filtroPesquisa.DataProgramacaoPagamentoFinal.Date);

            if (filtroPesquisa.DataVencimentoInicial != DateTime.MinValue)
                query = query.Where(obj => obj.DataVencimento.Value.Date >= filtroPesquisa.DataVencimentoInicial.Date);

            if (filtroPesquisa.DataVencimentoFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataVencimento.Value.Date < filtroPesquisa.DataVencimentoFinal.AddDays(1).Date);

            if (filtroPesquisa.DataEmissaoInicial != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date >= filtroPesquisa.DataEmissaoInicial.Date);

            if (filtroPesquisa.DataEmissaoFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date < filtroPesquisa.DataEmissaoFinal.AddDays(1).Date);

            if (filtroPesquisa.NumeroTitulo > 0)
                query = query.Where(o => o.Codigo == filtroPesquisa.NumeroTitulo);

            if (filtroPesquisa.Valor > 0)
                query = query.Where(o => o.Valor == filtroPesquisa.Valor);

            if (filtroPesquisa.CodigoBaixa > 0)
                query = query.Where(o => o.Baixas.Any(b => b.TituloBaixa.Codigo == filtroPesquisa.CodigoBaixa));
            else
                query = query.Where(obj => obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && !obj.Baixas.Any(b => b.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Cancelada));

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> ObterConsultaTitulosAReceberPendentesParaBordero(double cnpjPessoa, int codigoGrupoPessoa, int numeroCTe, string numeroCarga, int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => obj.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber && obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto && !obj.ModeloAntigo && obj.Documentos.Count > 0 && !obj.Borderos.Any(o => o.Bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.Cancelado));

            if (cnpjPessoa > 0d)
                query = query.Where(obj => obj.Pessoa.CPF_CNPJ == cnpjPessoa);

            if (codigoGrupoPessoa > 0)
                query = query.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupoPessoa || obj.Pessoa.GrupoPessoas.Codigo == codigoGrupoPessoa);

            if (numeroCTe > 0)
                query = query.Where(o => o.Documentos.Any(d => d.CTe.Numero == numeroCTe || d.Carga.CargaCTes.Any(cc => cc.CTe.Numero == numeroCTe)));

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.Documentos.Any(d => d.Carga.CodigoCargaEmbarcador == numeroCarga || d.CTe.CargaCTes.Any(cc => cc.Carga.CodigoCargaEmbarcador == numeroCarga)));

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> ObterConsultaCadastroTituloFinanceiro(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaTituloFinanceiro filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
                query = query.Where(o => o.TipoMovimento.Codigo == filtrosPesquisa.CodigoTipoMovimento);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NossoNumero))
                query = query.Where(obj => obj.NossoNumero.StartsWith(filtrosPesquisa.NossoNumero));

            if (filtrosPesquisa.DataProgramacaoPagamentoInicial != DateTime.MinValue)
                query = query.Where(obj => obj.DataProgramacaoPagamento.Value.Date >= filtrosPesquisa.DataProgramacaoPagamentoInicial.Date);

            if (filtrosPesquisa.DataProgramacaoPagamentoFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataProgramacaoPagamento.Value.Date <= filtrosPesquisa.DataProgramacaoPagamentoFinal.Date);

            if (filtrosPesquisa.DataInicialVencimento != DateTime.MinValue)
                query = query.Where(obj => obj.DataVencimento.Value.Date >= filtrosPesquisa.DataInicialVencimento.Date);

            if (filtrosPesquisa.DataFinalVencimento != DateTime.MinValue)
                query = query.Where(obj => obj.DataVencimento.Value.Date <= filtrosPesquisa.DataFinalVencimento.Date);

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date >= filtrosPesquisa.DataInicialEmissao.Date);

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date <= filtrosPesquisa.DataFinalEmissao.Date);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query = query.Where(obj => obj.Veiculos.Any(o => o.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigoCTe > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.Codigo == filtrosPesquisa.CodigoCTe) || obj.ConhecimentoDeTransporteEletronico.Codigo == filtrosPesquisa.CodigoCTe);

            if (filtrosPesquisa.CodigoCategoriaPessoa > 0)
                query = query.Where(obj => obj.Pessoa.Categoria.Codigo == filtrosPesquisa.CodigoCategoriaPessoa);

            if (filtrosPesquisa.ValorDe > 0)
                query = query.Where(obj => obj.ValorOriginal >= filtrosPesquisa.ValorDe);

            if (filtrosPesquisa.ValorAte > 0)
                query = query.Where(obj => obj.ValorOriginal <= filtrosPesquisa.ValorAte);


            long vNumeroPedido = 0;
            long.TryParse(filtrosPesquisa.NumeroPedido, out vNumeroPedido);
            if (vNumeroPedido > 0)
            {
                var queryAvon = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();
                var queryNatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                var resultAvon = from obj in queryAvon where obj.NumeroMinuta.Equals(vNumeroPedido) select obj;
                var resultNatura = from obj in queryNatura where obj.DocumentoTransporte.Numero.Equals(vNumeroPedido) select obj;
                var resultCargaPedido = from obj in queryCargaPedido where obj.Pedido.NumeroPedidoEmbarcador.Contains(filtrosPesquisa.NumeroPedido) select obj;

                query = query.Where(obj =>
                    obj.FaturaParcela.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultAvon.Select(b => b.Carga.Codigo).FirstOrDefault()) ||
                    obj.FaturaParcela.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultNatura.Select(b => b.Carga.Codigo).FirstOrDefault()) ||
                    obj.FaturaParcela.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultCargaPedido.Select(b => b.Carga.Codigo).FirstOrDefault()));
            }
            else if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
            {
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var resultCargaPedido = from obj in queryCargaPedido where obj.Pedido.NumeroPedidoEmbarcador.Contains(filtrosPesquisa.NumeroPedido) select obj;

                query = query.Where(obj => obj.FaturaParcela.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultCargaPedido.Select(b => b.Carga.Codigo).FirstOrDefault()));
            }

            if (filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue || filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue)
                    query = query.Where(o => o.DataBaseLiquidacao >= filtrosPesquisa.DataBaseLiquidacaoInicial.Date);

                if (filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue)
                    query = query.Where(o => o.DataBaseLiquidacao < filtrosPesquisa.DataBaseLiquidacaoFinal.AddDays(1).Date);
            }

            if (filtrosPesquisa.ValorPago > 0)
                query = query.Where(obj => obj.ValorPago >= filtrosPesquisa.ValorPago);

            if (filtrosPesquisa.ValorPagoAte > 0)
                query = query.Where(obj => obj.ValorPago <= filtrosPesquisa.ValorPagoAte);

            if (filtrosPesquisa.ValorMovimento > 0)
                query = query.Where(obj => obj.ValorOriginal.Equals(filtrosPesquisa.ValorMovimento));

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                query = query.Where(obj => obj.Pessoa.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoa);

            if (filtrosPesquisa.CodigoFatura > 0)
                query = query.Where(obj => obj.FaturaParcela.Fatura.Codigo == filtrosPesquisa.CodigoFatura);

            if (filtrosPesquisa.CodigoPessoa > 0)
                query = query.Where(obj => obj.Pessoa.CPF_CNPJ == filtrosPesquisa.CodigoPessoa);

            if (filtrosPesquisa.CodigoPortador > 0)
                query = query.Where(obj => obj.Portador.CPF_CNPJ == filtrosPesquisa.CodigoPortador);

            if (filtrosPesquisa.Adiantado >= 0)
            {
                if (filtrosPesquisa.Adiantado == 0)
                    query = query.Where(obj => obj.Adiantado == false);
                else
                    query = query.Where(obj => obj.Adiantado == true);
            }

            if (filtrosPesquisa.CodigoTitulo > 0)
                query = query.Where(obj => obj.Codigo == filtrosPesquisa.CodigoTitulo || obj.CodigoRecebidoIntegracao == filtrosPesquisa.CodigoTitulo);

            if (filtrosPesquisa.StatusTitulo != null && filtrosPesquisa.StatusTitulo.Count > 0)
                query = query.Where(obj => filtrosPesquisa.StatusTitulo.Contains(obj.StatusTitulo));

            if (filtrosPesquisa.TipoTitulo > 0)
                query = query.Where(obj => obj.TipoTitulo == filtrosPesquisa.TipoTitulo);

            if (filtrosPesquisa.TipoDocumento != Dominio.Enumeradores.TipoDocumento.Todos)
                query = query.Where(obj => obj.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao == filtrosPesquisa.TipoDocumento);

            if (filtrosPesquisa.TipoDeDocumento.HasValue)
            {
                if (filtrosPesquisa.TipoDeDocumento.Value == TipoDocumentoPesquisaTitulo.ContratoFrete)
                    query = query.Where(obj => obj.ContratoFrete != null);
                else if (filtrosPesquisa.TipoDeDocumento.Value == TipoDocumentoPesquisaTitulo.DocumentoEntrada)
                    query = query.Where(obj => obj.DuplicataDocumentoEntrada != null);
                else if (filtrosPesquisa.TipoDeDocumento.Value == TipoDocumentoPesquisaTitulo.Fatura)
                    query = query.Where(obj => obj.FaturaParcela != null || obj.FaturaCargaDocumento != null);
                else if (filtrosPesquisa.TipoDeDocumento.Value == TipoDocumentoPesquisaTitulo.Negociacao)
                    query = query.Where(obj => obj.TituloBaixaNegociacao != null);
                else if (filtrosPesquisa.TipoDeDocumento.Value == TipoDocumentoPesquisaTitulo.NotaFiscal)
                    query = query.Where(obj => obj.NotaFiscal != null);
                else if (filtrosPesquisa.TipoDeDocumento.Value == TipoDocumentoPesquisaTitulo.ContratoFinanciamento)
                {
                    var queryContratoFinanciamentoParcela = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();
                    var resultContratoFinanciamentoParcela = from obj in queryContratoFinanciamentoParcela select obj;
                    query = query.Where(obj => resultContratoFinanciamentoParcela.Any(a => a.Titulo == obj));
                }
                else if (filtrosPesquisa.TipoDeDocumento.Value == TipoDocumentoPesquisaTitulo.Outros)
                {
                    var queryContratoFinanciamentoParcela = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();
                    var resultContratoFinanciamentoParcela = from obj in queryContratoFinanciamentoParcela select obj;

                    query = query.Where(obj => obj.NotaFiscal == null && obj.TituloBaixaNegociacao == null && obj.FaturaParcela == null && obj.FaturaCargaDocumento == null && obj.DuplicataDocumentoEntrada == null && obj.ContratoFrete == null);
                    query = query.Where(obj => !resultContratoFinanciamentoParcela.Any(a => a.Titulo == obj));
                }
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DocumentoOriginal))
            {
                if ((filtrosPesquisa.DocumentoOriginal.Substring(0, 1) == "'") && (filtrosPesquisa.DocumentoOriginal.Substring((filtrosPesquisa.DocumentoOriginal.Length - 1), 1) == "'"))
                    query = query.Where(obj => obj.NumeroDocumentoTituloOriginal.Equals(filtrosPesquisa.DocumentoOriginal.Replace("'", "")));
                else if (filtrosPesquisa.DocumentoOriginal.Substring(0, 1) == "'")
                    query = query.Where(obj => obj.NumeroDocumentoTituloOriginal.StartsWith(filtrosPesquisa.DocumentoOriginal.Replace("'", "")));
                else if (filtrosPesquisa.DocumentoOriginal.Substring((filtrosPesquisa.DocumentoOriginal.Length - 1), 1) == "'")
                    query = query.Where(obj => obj.NumeroDocumentoTituloOriginal.EndsWith(filtrosPesquisa.DocumentoOriginal.Replace("'", "")));
                else
                    query = query.Where(obj => obj.NumeroDocumentoTituloOriginal.Contains(filtrosPesquisa.DocumentoOriginal));
            }

            if (int.TryParse(filtrosPesquisa.NumeroOcorrencia, out int iNumeroOcorrencia) && iNumeroOcorrencia > 0)
            {
                query = query.Where(o => o.Documentos.Any(d => d.CTe.CargaCTeOcorrencias.Any(cco => cco.CargaOcorrencia.NumeroOcorrencia == iNumeroOcorrencia || cco.CargaOcorrencia.NumeroOcorrenciaCliente.Contains(filtrosPesquisa.NumeroOcorrencia)) ||
                                                               d.Carga.Ocorrencias.Any(coc => coc.NumeroOcorrencia == iNumeroOcorrencia || coc.NumeroOcorrenciaCliente.Contains(filtrosPesquisa.NumeroOcorrencia))));
            }
            else if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOcorrencia))
            {
                query = query.Where(o => o.Documentos.Any(d => d.CTe.CargaCTeOcorrencias.Any(cco => cco.CargaOcorrencia.NumeroOcorrenciaCliente.Contains(filtrosPesquisa.NumeroOcorrencia)) ||
                                                               d.Carga.Ocorrencias.Any(coc => coc.NumeroOcorrenciaCliente.Contains(filtrosPesquisa.NumeroOcorrencia))));
            }

            if (filtrosPesquisa.NumeroDocumentoOriginario > 0)
                query = query.Where(o => o.Documentos.Any(d => d.CTe.DocumentosOriginarios.Any(dor => dor.Numero == filtrosPesquisa.NumeroDocumentoOriginario)));

            if (filtrosPesquisa.TipoAmbiente > 0)
                query = query.Where(obj => obj.TipoAmbiente == filtrosPesquisa.TipoAmbiente);

            if ((int)filtrosPesquisa.FormaTitulo > -1 && filtrosPesquisa.FormaTitulo != FormaTitulo.Todos)
                query = query.Where(obj => obj.FormaTitulo == filtrosPesquisa.FormaTitulo);

            if (!filtrosPesquisa.VisualizarTitulosPagamentoSalario)
                query = query.Where(obj => obj.FormaTitulo != FormaTitulo.PagamentoSalario);

            if (filtrosPesquisa.ProvisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SemProvisao)
                query = query.Where(obj => obj.Provisao == false);
            else if (filtrosPesquisa.ProvisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SomenteProvisao)
                query = query.Where(obj => obj.Provisao == true);

            if (filtrosPesquisa.CodigoRemessa > 0)
                query = query.Where(obj => obj.BoletoRemessa.Codigo == filtrosPesquisa.CodigoRemessa);

            if (filtrosPesquisa.TipoBoleto != TipoBoletoPesquisaTitulo.Todos)
            {
                if (filtrosPesquisa.TipoBoleto == TipoBoletoPesquisaTitulo.ComBoleto)
                    query = query.Where(obj => obj.BoletoStatusTitulo != BoletoStatusTitulo.Nenhum);
                else if (filtrosPesquisa.TipoBoleto == TipoBoletoPesquisaTitulo.SemBoleto)
                    query = query.Where(obj => obj.BoletoStatusTitulo == BoletoStatusTitulo.Nenhum);
                else if (filtrosPesquisa.TipoBoleto == TipoBoletoPesquisaTitulo.ComRemessa)
                    query = query.Where(obj => obj.BoletoRemessa != null);
                else if (filtrosPesquisa.TipoBoleto == TipoBoletoPesquisaTitulo.SemRemessa)
                    query = query.Where(obj => obj.BoletoStatusTitulo != BoletoStatusTitulo.Nenhum && obj.BoletoRemessa == null);
            }

            if (filtrosPesquisa.MoedaCotacaoBancoCentral.HasValue && filtrosPesquisa.MoedaCotacaoBancoCentral.Value != MoedaCotacaoBancoCentral.Todas)
                query = query.Where(obj => obj.MoedaCotacaoBancoCentral == filtrosPesquisa.MoedaCotacaoBancoCentral);

            if (!string.IsNullOrEmpty(filtrosPesquisa.RaizCnpjPessoa))
                query = query.Where(o => o.Pessoa.CPF_CNPJ >= Convert.ToDouble(filtrosPesquisa.RaizCnpjPessoa.PadRight(14, '0')) && o.Pessoa.CPF_CNPJ <= Convert.ToDouble(filtrosPesquisa.RaizCnpjPessoa.PadRight(14, '9')));

            //Emissão Multimodal
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.NumeroBooking == filtrosPesquisa.NumeroBooking) || obj.ConhecimentoDeTransporteEletronico.NumeroBooking == filtrosPesquisa.NumeroBooking);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.NumeroOS == filtrosPesquisa.NumeroOS) || obj.ConhecimentoDeTransporteEletronico.NumeroOS == filtrosPesquisa.NumeroOS);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.CargaCTes.Any(ot => ot.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga)) || obj.ConhecimentoDeTransporteEletronico.CargaCTes.Any(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga));

            if (filtrosPesquisa.NumeroNota > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.XMLNotaFiscais.Any(ot => ot.Numero == filtrosPesquisa.NumeroNota)) || obj.ConhecimentoDeTransporteEletronico.XMLNotaFiscais.Any(o => o.Numero == filtrosPesquisa.NumeroNota));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControleCliente))
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.XMLNotaFiscais.Any(ot => ot.NumeroControleCliente == filtrosPesquisa.NumeroControleCliente)) || obj.ConhecimentoDeTransporteEletronico.XMLNotaFiscais.Any(o => o.NumeroControleCliente == filtrosPesquisa.NumeroControleCliente));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.NumeroControle == filtrosPesquisa.NumeroControle) || obj.ConhecimentoDeTransporteEletronico.NumeroControle == filtrosPesquisa.NumeroControle);

            if (filtrosPesquisa.CodigoTerminalOrigem > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.TerminalOrigem.Codigo == filtrosPesquisa.CodigoTerminalOrigem) || obj.ConhecimentoDeTransporteEletronico.TerminalOrigem.Codigo == filtrosPesquisa.CodigoTerminalOrigem);

            if (filtrosPesquisa.CodigoTerminalDestino > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.TerminalDestino.Codigo == filtrosPesquisa.CodigoTerminalDestino) || obj.ConhecimentoDeTransporteEletronico.TerminalDestino.Codigo == filtrosPesquisa.CodigoTerminalDestino);

            if (filtrosPesquisa.CodigoViagem > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.Viagem.Codigo == filtrosPesquisa.CodigoViagem) || obj.ConhecimentoDeTransporteEletronico.Viagem.Codigo == filtrosPesquisa.CodigoViagem);

            if ((int)filtrosPesquisa.TipoProposta > 0 && filtrosPesquisa.TipoProposta != TipoPropostaMultimodal.Nenhum)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.CargaCTes.Any(ot => ot.Carga.CargaOrigemPedidos.Any(p => p.TipoPropostaMultimodal == filtrosPesquisa.TipoProposta))) || obj.ConhecimentoDeTransporteEletronico.CargaCTes.Any(o => o.Carga.CargaOrigemPedidos.Any(p => p.TipoPropostaMultimodal == filtrosPesquisa.TipoProposta)));

            if (filtrosPesquisa.TiposPropostasMultimodal != null && filtrosPesquisa.TiposPropostasMultimodal.Count > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.CargaCTes.Any(ot => ot.Carga.CargaOrigemPedidos.Any(p => filtrosPesquisa.TiposPropostasMultimodal.Contains(p.TipoPropostaMultimodal)))) || obj.ConhecimentoDeTransporteEletronico.CargaCTes.Any(o => o.Carga.CargaOrigemPedidos.Any(p => filtrosPesquisa.TiposPropostasMultimodal.Contains(p.TipoPropostaMultimodal))));

            if (filtrosPesquisa.StatusEmAberto.HasValue && filtrosPesquisa.StatusEmAberto.Value)
                query = query.Where(obj => obj.StatusTitulo == StatusTitulo.EmAberto);

            if (filtrosPesquisa.TipoAPagar.HasValue && filtrosPesquisa.TipoAPagar.Value)
                query = query.Where(obj => obj.TipoTitulo == TipoTitulo.Pagar);

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> ObterConsultaBoletoGeracao(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBoletoGeracao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            query = query.Where(obj => obj.StatusTitulo == StatusTitulo.EmAberto && obj.TipoTitulo == TipoTitulo.Receber);

            if (filtrosPesquisa.CnpjPessoa > 0)
                query = query.Where(obj => obj.Pessoa.CPF_CNPJ == filtrosPesquisa.CnpjPessoa);

            if (filtrosPesquisa.FormaTitulo != FormaTitulo.Todos)
                query = query.Where(obj => obj.FormaTitulo == filtrosPesquisa.FormaTitulo);

            if (filtrosPesquisa.CodigoRemessa > 0)
                query = query.Where(obj => obj.BoletoRemessa.Codigo == filtrosPesquisa.CodigoRemessa);

            if (filtrosPesquisa.SomentePendentes)
                query = query.Where(obj => obj.NossoNumero == null || obj.NossoNumero == "");

            if (filtrosPesquisa.SomenteSemRemessa)
                query = query.Where(obj => obj.BoletoRemessa == null && obj.NossoNumero != null && obj.NossoNumero != "");

            if (filtrosPesquisa.CodigosEmpresa.Count > 0)
                query = query.Where(obj => filtrosPesquisa.CodigosEmpresa.Contains(obj.Empresa.Codigo));

            if (filtrosPesquisa.DataVencimentoInicial > DateTime.MinValue && filtrosPesquisa.DataVencimentoFinal > DateTime.MinValue)
                query = query.Where(obj => obj.DataVencimento.Value.Date >= filtrosPesquisa.DataVencimentoInicial && obj.DataVencimento.Value.Date <= filtrosPesquisa.DataVencimentoFinal);
            else if (filtrosPesquisa.DataVencimentoInicial > DateTime.MinValue)
                query = query.Where(obj => obj.DataVencimento.Value.Date == filtrosPesquisa.DataVencimentoInicial);
            else if (filtrosPesquisa.DataVencimentoFinal > DateTime.MinValue)
                query = query.Where(obj => obj.DataVencimento.Value.Date == filtrosPesquisa.DataVencimentoFinal);

            if (filtrosPesquisa.DataEmissaoInicial > DateTime.MinValue && filtrosPesquisa.DataEmissaoFinal > DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date >= filtrosPesquisa.DataEmissaoInicial && obj.DataEmissao.Value.Date <= filtrosPesquisa.DataEmissaoFinal);
            else if (filtrosPesquisa.DataEmissaoInicial > DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date == filtrosPesquisa.DataEmissaoInicial);
            else if (filtrosPesquisa.DataEmissaoFinal > DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Value.Date == filtrosPesquisa.DataEmissaoFinal);

            if (filtrosPesquisa.TipoAmbiente > 0)
                query = query.Where(obj => obj.TipoAmbiente == filtrosPesquisa.TipoAmbiente);

            if (filtrosPesquisa.CodigoConhecimento > 0)
                query = query.Where(obj => obj.ConhecimentoDeTransporteEletronico.Codigo == filtrosPesquisa.CodigoConhecimento);

            //Emissão Multimodal
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.NumeroBooking == filtrosPesquisa.NumeroBooking) || obj.ConhecimentoDeTransporteEletronico.NumeroBooking == filtrosPesquisa.NumeroBooking);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.NumeroOS == filtrosPesquisa.NumeroOS) || obj.ConhecimentoDeTransporteEletronico.NumeroOS == filtrosPesquisa.NumeroOS);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.CargaCTes.Any(ot => ot.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga)) || obj.ConhecimentoDeTransporteEletronico.CargaCTes.Any(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga));

            if (filtrosPesquisa.NumeroNota > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.XMLNotaFiscais.Any(ot => ot.Numero == filtrosPesquisa.NumeroNota)) || obj.ConhecimentoDeTransporteEletronico.XMLNotaFiscais.Any(o => o.Numero == filtrosPesquisa.NumeroNota));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControleCliente))
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.XMLNotaFiscais.Any(ot => ot.NumeroControleCliente == filtrosPesquisa.NumeroControleCliente)) || obj.ConhecimentoDeTransporteEletronico.XMLNotaFiscais.Any(o => o.NumeroControleCliente == filtrosPesquisa.NumeroControleCliente));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.NumeroControle == filtrosPesquisa.NumeroControle) || obj.ConhecimentoDeTransporteEletronico.NumeroControle == filtrosPesquisa.NumeroControle);

            if (filtrosPesquisa.CodigoTerminalOrigem > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.TerminalOrigem.Codigo == filtrosPesquisa.CodigoTerminalOrigem) || obj.ConhecimentoDeTransporteEletronico.TerminalOrigem.Codigo == filtrosPesquisa.CodigoTerminalOrigem);

            if (filtrosPesquisa.CodigoTerminalDestino > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.TerminalDestino.Codigo == filtrosPesquisa.CodigoTerminalDestino) || obj.ConhecimentoDeTransporteEletronico.TerminalDestino.Codigo == filtrosPesquisa.CodigoTerminalDestino);

            if (filtrosPesquisa.CodigoViagem > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.Viagem.Codigo == filtrosPesquisa.CodigoViagem) || obj.ConhecimentoDeTransporteEletronico.Viagem.Codigo == filtrosPesquisa.CodigoViagem);

            if ((int)filtrosPesquisa.TipoProposta > 0 && filtrosPesquisa.TipoProposta != TipoPropostaMultimodal.Nenhum)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.CargaCTes.Any(ot => ot.Carga.CargaOrigemPedidos.Any(p => p.TipoPropostaMultimodal == filtrosPesquisa.TipoProposta))) || obj.ConhecimentoDeTransporteEletronico.CargaCTes.Any(o => o.Carga.CargaOrigemPedidos.Any(p => p.TipoPropostaMultimodal == filtrosPesquisa.TipoProposta)));

            if (filtrosPesquisa.TiposPropostasMultimodal != null && filtrosPesquisa.TiposPropostasMultimodal.Count > 0)
                query = query.Where(obj => obj.Documentos.Any(o => o.CTe.CargaCTes.Any(ot => ot.Carga.CargaOrigemPedidos.Any(p => filtrosPesquisa.TiposPropostasMultimodal.Contains(p.TipoPropostaMultimodal)))) || obj.ConhecimentoDeTransporteEletronico.CargaCTes.Any(o => o.Carga.CargaOrigemPedidos.Any(p => filtrosPesquisa.TiposPropostasMultimodal.Contains(p.TipoPropostaMultimodal))));

            if (filtrosPesquisa.CodigoFatura > 0)
                query = query.Where(obj => obj.FaturaParcela.Fatura.Codigo == filtrosPesquisa.CodigoFatura);

            if (filtrosPesquisa.CodigoOperadorFatura > 0)
                query = query.Where(obj => obj.Usuario.Codigo == filtrosPesquisa.CodigoOperadorFatura);

            if (filtrosPesquisa.CodigoConfiguracaoBoleto > 0)
                query = query.Where(obj => obj.BoletoConfiguracao.Codigo == filtrosPesquisa.CodigoConfiguracaoBoleto);

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> ObterConsultaAlteracaoBoleto(int codigoAlteracao, double cnpjPessoa, int codigoEmpresa, DateTime vencimentoInicial, DateTime vencimentoFinal, DateTime emissaoInicial, DateTime emissaoFinal, int codigoBoletoConfiguracao, TipoAmbiente tipoAmbiente)
        {
            if (codigoAlteracao > 0)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo>();
                var result = from obj in query where obj.BoletoAlteracao.Codigo == codigoAlteracao select obj;
                var resultTitulo = result.Select(obj => obj.Titulo);

                return resultTitulo;
            }
            else
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
                query = query.Where(obj =>
                                obj.BoletoConfiguracao != null &&
                                obj.BoletoRemessa != null &&
                                obj.NossoNumero != "" &&
                                obj.NossoNumero != null &&
                                obj.StatusTitulo == StatusTitulo.EmAberto &&
                                obj.BoletoStatusTitulo != BoletoStatusTitulo.Nenhum &&
                                obj.TipoTitulo == TipoTitulo.Receber
                                );

                if (cnpjPessoa > 0)
                    query = query.Where(obj => obj.Pessoa.CPF_CNPJ == cnpjPessoa);

                if (codigoBoletoConfiguracao > 0)
                    query = query.Where(obj => obj.BoletoConfiguracao.Codigo == codigoBoletoConfiguracao);

                if (codigoEmpresa > 0)
                    query = query.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

                if (vencimentoInicial > DateTime.MinValue)
                    query = query.Where(obj => obj.DataVencimento.Value.Date >= vencimentoInicial);

                if (vencimentoFinal > DateTime.MinValue)
                    query = query.Where(obj => obj.DataVencimento.Value.Date <= vencimentoFinal);

                if (emissaoInicial > DateTime.MinValue)
                    query = query.Where(obj => obj.DataEmissao.Value.Date >= emissaoInicial);

                if (emissaoFinal > DateTime.MinValue)
                    query = query.Where(obj => obj.DataEmissao.Value.Date <= emissaoFinal);

                if (tipoAmbiente != TipoAmbiente.Nenhum)
                    query = query.Where(obj => obj.TipoAmbiente == tipoAmbiente);

                return query;
            }
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Titulo> ObterConsultaTitulosPendentes(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaBaixaTituloPendente filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>().Where(titulo => titulo.Provisao == false);
            var result = from obj in query where obj.StatusTitulo != StatusTitulo.Cancelado && obj.TipoTitulo == filtrosPesquisa.TipoTitulo && !obj.DataCancelamento.HasValue select obj;

            if (filtrosPesquisa.TipoTitulo == TipoTitulo.Receber)
                result = result.Where(o => (o.ModeloAntigo || o.Documentos.Count == 0));

            if (filtrosPesquisa.CodigoPagamentoEletronico > 0)
            {
                var queryPagamentoEletronico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
                var resultPagamentoEletronico = from obj in queryPagamentoEletronico where obj.PagamentoEletronico.Codigo == filtrosPesquisa.CodigoPagamentoEletronico select obj;

                result = result.Where(obj => resultPagamentoEletronico.Select(a => a.Titulo).Contains(obj));
            }
            if (filtrosPesquisa.SituacaoBoletoTitulo != null && filtrosPesquisa.SituacaoBoletoTitulo.HasValue)
            {
                if (filtrosPesquisa.SituacaoBoletoTitulo.Value == SituacaoBoletoTitulo.ComBoleto)
                    result = result.Where(obj => obj.NossoNumero != "" && obj.NossoNumero != null);
                else if (filtrosPesquisa.SituacaoBoletoTitulo.Value == SituacaoBoletoTitulo.SemBoleto)
                    result = result.Where(obj => obj.NossoNumero == "" || obj.NossoNumero == null);
            }
            if (filtrosPesquisa.SituacaoPagamentoEletronico != null && filtrosPesquisa.SituacaoPagamentoEletronico.HasValue)
            {
                if (filtrosPesquisa.SituacaoPagamentoEletronico.Value == SituacaoPagamentoEletronico.Pendente)
                {
                    var queryPagamentoEletronico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
                    var resultPagamentoEletronico = from obj in queryPagamentoEletronico select obj;

                    result = result.Where(obj => !resultPagamentoEletronico.Select(a => a.Titulo).Contains(obj));
                }
                else if (filtrosPesquisa.SituacaoPagamentoEletronico.Value == SituacaoPagamentoEletronico.Gerado)
                {
                    var queryPagamentoEletronico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
                    var resultPagamentoEletronico = from obj in queryPagamentoEletronico select obj;

                    result = result.Where(obj => resultPagamentoEletronico.Select(a => a.Titulo).Contains(obj));
                }
            }

            if (filtrosPesquisa.NumeroTitulo > 0)
                result = result.Where(obj => obj.Codigo == filtrosPesquisa.NumeroTitulo);

            if (filtrosPesquisa.CnpjPessoa > 0)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == filtrosPesquisa.CnpjPessoa);

            if (filtrosPesquisa.CodigoFatura > 0)
                result = result.Where(obj => obj.FaturaParcela.Fatura.Codigo == filtrosPesquisa.CodigoFatura);

            if (filtrosPesquisa.DocumentoEntrada != null)
                result = result.Where(obj => obj.DuplicataDocumentoEntrada.DocumentoEntrada.Codigo == filtrosPesquisa.DocumentoEntrada.Codigo || obj.NumeroDocumentoTituloOriginal.Contains(filtrosPesquisa.DocumentoEntrada.Numero.ToString()));

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                result = result.Where(obj => obj.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoa || obj.Pessoa.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoa);

            if (filtrosPesquisa.TipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                if (filtrosPesquisa.CodigoEmpresa > 0)
                    result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.SomenteTitulosDeNegociacao == Dominio.Enumeradores.OpcaoSimNao.Sim)
                result = result.Where(obj => obj.TituloBaixaNegociacao != null);
            else if (filtrosPesquisa.SomenteTitulosDeNegociacao == Dominio.Enumeradores.OpcaoSimNao.Nao)
                result = result.Where(obj => obj.TituloBaixaNegociacao == null);

            if (filtrosPesquisa.CodigoConhecimento > 0)
                result = result.Where(obj => obj.ConhecimentoDeTransporteEletronico.Codigo == filtrosPesquisa.CodigoConhecimento);

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>();
                var resultCarga = from obj in queryCarga where obj.Carga.Codigo == filtrosPesquisa.CodigoCarga && obj.StatusFaturaCarga != StatusFaturaCarga.NaoFaturada select obj;

                result = result.Where(obj => resultCarga.Select(a => a.Fatura).Contains(obj.FaturaParcela.Fatura));
            }

            long vNumeroPedido = 0;
            long.TryParse(filtrosPesquisa.NumeroPedido, out vNumeroPedido);
            if (vNumeroPedido > 0)
            {
                var queryAvon = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();
                var queryNatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                var resultAvon = from obj in queryAvon where obj.NumeroMinuta.Equals(vNumeroPedido) select obj;
                var resultNatura = from obj in queryNatura where obj.DocumentoTransporte.Numero.Equals(vNumeroPedido) select obj;
                var resultCargaPedido = from obj in queryCargaPedido where obj.Pedido.NumeroPedidoEmbarcador.Contains(filtrosPesquisa.NumeroPedido) select obj;

                result = result.Where(obj =>
                    obj.FaturaParcela.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultAvon.Select(b => b.Carga.Codigo).FirstOrDefault()) ||
                    obj.FaturaParcela.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultNatura.Select(b => b.Carga.Codigo).FirstOrDefault()) ||
                    obj.FaturaParcela.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultCargaPedido.Select(b => b.Carga.Codigo).FirstOrDefault()));
            }
            else if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
            {
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var resultCargaPedido = from obj in queryCargaPedido where obj.Pedido.NumeroPedidoEmbarcador.Contains(filtrosPesquisa.NumeroPedido) select obj;

                result = result.Where(obj => obj.FaturaParcela.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultCargaPedido.Select(b => b.Carga.Codigo).FirstOrDefault()));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOcorrencia))
                result = result.Where(o => o.FaturaParcela.Fatura.Cargas.Any(car => car.Carga.Ocorrencias.Any(oco => oco.NumeroOcorrenciaCliente.Contains(filtrosPesquisa.NumeroOcorrencia))));

            if (filtrosPesquisa.DataEmissaoInicial > DateTime.MinValue && filtrosPesquisa.DataEmissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao >= filtrosPesquisa.DataEmissaoInicial.Date && obj.DataEmissao <= filtrosPesquisa.DataEmissaoFinal.AddDays(1));
            else if (filtrosPesquisa.DataEmissaoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao >= filtrosPesquisa.DataEmissaoInicial.Date);
            else if (filtrosPesquisa.DataEmissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao <= filtrosPesquisa.DataEmissaoFinal.AddDays(1));

            if (filtrosPesquisa.DataProgramacaoPagamentoInicial > DateTime.MinValue && filtrosPesquisa.DataProgramacaoPagamentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataProgramacaoPagamento >= filtrosPesquisa.DataProgramacaoPagamentoInicial.Date && obj.DataProgramacaoPagamento < filtrosPesquisa.DataProgramacaoPagamentoFinal.AddDays(1));
            else if (filtrosPesquisa.DataProgramacaoPagamentoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataProgramacaoPagamento >= filtrosPesquisa.DataProgramacaoPagamentoInicial.Date);
            else if (filtrosPesquisa.DataProgramacaoPagamentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataProgramacaoPagamento < filtrosPesquisa.DataProgramacaoPagamentoFinal.AddDays(1));

            if (filtrosPesquisa.DataVencimentoInicial > DateTime.MinValue && filtrosPesquisa.DataVencimentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimento >= filtrosPesquisa.DataVencimentoInicial.Date && obj.DataVencimento < filtrosPesquisa.DataVencimentoFinal.AddDays(1));
            else if (filtrosPesquisa.DataVencimentoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimento >= filtrosPesquisa.DataVencimentoInicial.Date);
            else if (filtrosPesquisa.DataVencimentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimento < filtrosPesquisa.DataVencimentoFinal.AddDays(1));

            if (filtrosPesquisa.DataAutorizacaoInicial > DateTime.MinValue && filtrosPesquisa.DataAutorizacaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataAutorizacao >= filtrosPesquisa.DataAutorizacaoInicial.Date && obj.DataAutorizacao < filtrosPesquisa.DataAutorizacaoFinal.AddDays(1));
            else if (filtrosPesquisa.DataAutorizacaoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataAutorizacao >= filtrosPesquisa.DataAutorizacaoInicial.Date);
            else if (filtrosPesquisa.DataAutorizacaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataAutorizacao <= filtrosPesquisa.DataAutorizacaoFinal.AddDays(1));

            if (filtrosPesquisa.ValorInicial > 0 && filtrosPesquisa.ValorFinal > 0)
                result = result.Where(obj => obj.ValorOriginal >= filtrosPesquisa.ValorInicial && obj.ValorOriginal <= filtrosPesquisa.ValorFinal);
            else if (filtrosPesquisa.ValorInicial > 0)
                result = result.Where(obj => obj.ValorOriginal >= filtrosPesquisa.ValorInicial);
            else if (filtrosPesquisa.ValorFinal > 0)
                result = result.Where(obj => obj.ValorOriginal <= filtrosPesquisa.ValorFinal);

            if (filtrosPesquisa.TipoAmbiente != TipoAmbiente.Nenhum)
                result = result.Where(obj => obj.TipoAmbiente == filtrosPesquisa.TipoAmbiente);

            if ((int)filtrosPesquisa.FormaTitulo > -1 && filtrosPesquisa.FormaTitulo != FormaTitulo.Todos)
                result = result.Where(obj => obj.FormaTitulo == filtrosPesquisa.FormaTitulo);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
            {
                if ((filtrosPesquisa.NumeroDocumento.Substring(0, 1) == "'") && (filtrosPesquisa.NumeroDocumento.Substring((filtrosPesquisa.NumeroDocumento.Length - 1), 1) == "'"))
                    result = result.Where(obj => obj.NumeroDocumentoTituloOriginal.Equals(filtrosPesquisa.NumeroDocumento.Replace("'", "")));
                else if (filtrosPesquisa.NumeroDocumento.Substring(0, 1) == "'")
                    result = result.Where(obj => obj.NumeroDocumentoTituloOriginal.StartsWith(filtrosPesquisa.NumeroDocumento.Replace("'", "")));
                else if (filtrosPesquisa.NumeroDocumento.Substring((filtrosPesquisa.NumeroDocumento.Length - 1), 1) == "'")
                    result = result.Where(obj => obj.NumeroDocumentoTituloOriginal.EndsWith(filtrosPesquisa.NumeroDocumento.Replace("'", "")));
                else
                    result = result.Where(obj => obj.NumeroDocumentoTituloOriginal.Contains(filtrosPesquisa.NumeroDocumento));
            }

            if (filtrosPesquisa.CodigoNaturezaOperacaoEntrada > 0)
                result = result.Where(obj => obj.DuplicataDocumentoEntrada.DocumentoEntrada.NaturezaOperacao.Codigo == filtrosPesquisa.CodigoNaturezaOperacaoEntrada);

            if (filtrosPesquisa.CodigoBaixa > 0)
                result = result.Where(o => o.Baixas.Any(b => b.TituloBaixa.Codigo == filtrosPesquisa.CodigoBaixa));
            else
            {
                result = result.Where(obj => obj.StatusTitulo == StatusTitulo.EmAberto);
                result = result.Where(o => !o.Baixas.Any(b => b.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada));
            }

            if (filtrosPesquisa.TiposDocumento?.Count > 0)
            {
                var tiposDeDocumentos = PredicateBuilder.False<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

                var queryContratoFinanciamentoParcela = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela>();
                var resultContratoFinanciamentoParcela = from obj in queryContratoFinanciamentoParcela select obj;

                foreach (TipoDocumentoPesquisaTitulo TipoDocumento in filtrosPesquisa.TiposDocumento)
                {
                    switch (TipoDocumento)
                    {
                        case TipoDocumentoPesquisaTitulo.ContratoFrete:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.ContratoFrete != null);
                            break;
                        case TipoDocumentoPesquisaTitulo.DocumentoEntrada:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.DuplicataDocumentoEntrada != null);
                            break;
                        case TipoDocumentoPesquisaTitulo.Fatura:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.FaturaParcela != null || obj.FaturaCargaDocumento != null);
                            break;
                        case TipoDocumentoPesquisaTitulo.Negociacao:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.TituloBaixaNegociacao != null);
                            break;
                        case TipoDocumentoPesquisaTitulo.NotaFiscal:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.NotaFiscal != null);
                            break;
                        case TipoDocumentoPesquisaTitulo.ContratoFinanciamento:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => resultContratoFinanciamentoParcela.Any(a => a.Titulo == obj));
                            break;
                        case TipoDocumentoPesquisaTitulo.Outros:
                            tiposDeDocumentos = tiposDeDocumentos.Or(obj => obj.NotaFiscal == null && obj.TituloBaixaNegociacao == null && obj.FaturaParcela == null && obj.FaturaCargaDocumento == null && obj.DuplicataDocumentoEntrada == null && obj.ContratoFrete == null && !resultContratoFinanciamentoParcela.Any(a => a.Titulo == obj));
                            break;
                    }
                }

                result = result.Where(tiposDeDocumentos);
            }

            if (filtrosPesquisa.CodigoBanco > 0)
                result = result.Where(obj => obj.Pessoa.Banco.Codigo == filtrosPesquisa.CodigoBanco);

            if (filtrosPesquisa.ValorTitulo > 0)
                result = result.Where(obj => obj.ValorOriginal == filtrosPesquisa.ValorTitulo);

            if (!string.IsNullOrEmpty(filtrosPesquisa.RaizCnpjPessoa))
                result = result.Where(o => o.Pessoa.CPF_CNPJ >= Convert.ToDouble(filtrosPesquisa.RaizCnpjPessoa.PadRight(14, '0')) && o.Pessoa.CPF_CNPJ <= Convert.ToDouble(filtrosPesquisa.RaizCnpjPessoa.PadRight(14, '9')));

            if (filtrosPesquisa.CnpjCpfPortador > 0)
                result = result.Where(obj => obj.Portador.CPF_CNPJ == filtrosPesquisa.CnpjCpfPortador);

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
                result = result.Where(obj => obj.TipoMovimento.Codigo == filtrosPesquisa.CodigoTipoMovimento);

            return result;
        }

        #endregion Métodos Privados

        #region Gráficos Faturamento Home

        public decimal ValorFaturamentoMes(int codigoEmpresa, DateTime dataMes, TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query
                         where obj.DataEmissao.Value.Month == dataMes.Month &&
                                obj.DataEmissao.Value.Year == dataMes.Year &&
                                obj.TipoTitulo == TipoTitulo.Receber &&
                                obj.StatusTitulo != StatusTitulo.Cancelado &&
                                !(obj.StatusTitulo == StatusTitulo.Quitada && obj.ValorPago == 0)
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            if (tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            if (result.Count() > 0)
                return result.Select(obj => obj.ValorOriginal).Sum();
            else
                return 0;
        }

        public decimal ValorReceitasMes(int codigoEmpresa, DateTime dataMes, TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query
                         where obj.DataVencimento.Value.Month == dataMes.Month &&
                                obj.DataVencimento.Value.Year == dataMes.Year &&
                                obj.TipoTitulo == TipoTitulo.Receber &&
                                obj.StatusTitulo != StatusTitulo.Cancelado
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            if (tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            if (result.Count() > 0)
                return result.Select(obj => obj.ValorOriginal).Sum();
            else
                return 0;
        }

        public decimal ValorRecebidoMes(int codigoEmpresa, DateTime dataMes, TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query
                         where obj.DataLiquidacao.Value.Month == dataMes.Month &&
                                obj.DataLiquidacao.Value.Year == dataMes.Year &&
                                obj.ValorPago > 0 &&
                                obj.TipoTitulo == TipoTitulo.Receber &&
                                obj.StatusTitulo != StatusTitulo.Cancelado
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            if (tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            if (result.Count() > 0)
                return result.Select(obj => obj.ValorPago).Sum();
            else
                return 0;
        }

        public decimal ValorDespesasMes(int codigoEmpresa, DateTime dataMes, TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query
                         where obj.DataVencimento.Value.Month == dataMes.Month &&
                                obj.DataVencimento.Value.Year == dataMes.Year &&
                                obj.TipoTitulo == TipoTitulo.Pagar &&
                                obj.StatusTitulo != StatusTitulo.Cancelado
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            if (tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            if (result.Count() > 0)
                return result.Select(obj => obj.ValorOriginal).Sum();
            else
                return 0;
        }

        public decimal ValorPagoMes(int codigoEmpresa, DateTime dataMes, TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query
                         where obj.DataLiquidacao.Value.Month == dataMes.Month &&
                                obj.DataLiquidacao.Value.Year == dataMes.Year &&
                                obj.ValorPago > 0 &&
                                obj.TipoTitulo == TipoTitulo.Pagar &&
                                obj.StatusTitulo != StatusTitulo.Cancelado
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            if (tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            if (result.Count() > 0)
                return result.Select(obj => obj.ValorPago).Sum();
            else
                return 0;
        }

        public decimal ValorLiquidadoMes(int codigoEmpresa, DateTime dataMes, TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query
                         where obj.DataVencimento.Value.Month == dataMes.Month &&
                                obj.DataVencimento.Value.Year == dataMes.Year &&
                                obj.ValorPago > 0 &&
                                obj.TipoTitulo == TipoTitulo.Receber &&
                                obj.StatusTitulo != StatusTitulo.Cancelado
                         select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            if (tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            if (result.Count() > 0)
                return result.Select(obj => obj.ValorPago).Sum();
            else
                return 0;
        }

        #endregion Gráficos Faturamento Home

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasReceber> RelatorioPosicaoContasReceber2(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, List<int> gruposPessoas, decimal valorCTeInicial, decimal valorCTeFinal, DateTime dataInicialQuitacao, DateTime dataFinalQuitacao, DateTime dataInicialMovimento, DateTime dataFinalMovimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusCTe, int codigoGrupoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, bool? cteVinculadoACarga, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioCTes(false, propriedades, gruposPessoas, valorCTeInicial, valorCTeFinal, dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento, statusCTe, codigoGrupoCTe, situacao, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, paginar, todosCNPJdaRaizEmbarcador, cteVinculadoACarga, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasReceber)));

            return query.SetTimeout(60000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasReceber>();
        }

        public int ContarPosicaoContasReceber2(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, List<int> gruposPessoas, decimal valorCTeInicial, decimal valorCTeFinal, DateTime dataInicialQuitacao, DateTime dataFinalQuitacao, DateTime dataInicialMovimento, DateTime dataFinalMovimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusCTe, int codigoGrupoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, bool? cteVinculadoACarga, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioCTes(true, propriedades, gruposPessoas, valorCTeInicial, valorCTeFinal, dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento, statusCTe, codigoGrupoCTe, situacao, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, paginar, todosCNPJdaRaizEmbarcador, cteVinculadoACarga, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            return query.SetTimeout(60000).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioCTes(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, List<int> gruposPessoas, decimal valorCTeInicial, decimal valorCTeFinal, DateTime dataInicialQuitacao, DateTime dataFinalQuitacao, DateTime dataInicialMovimento, DateTime dataFinalMovimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusCTe, int codigoGrupoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, bool paginar, bool todosCNPJdaRaizEmbarcador, bool? cteVinculadoACarga, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty,
                   having = string.Empty;

            //string having = " having 1=1 ";


            //if (exibirNotasFiscais)
            //{
            //    select += "CTe.CON_CODIGO Codigo, ";
            //    groupBy += "CTe.CON_CODIGO, ";
            //}

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaCTes(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaCTes(ref where, ref having, ref groupBy, ref joins, gruposPessoas, valorCTeInicial, valorCTeFinal, dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento, statusCTe, codigoGrupoCTe, situacao, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, cteVinculadoACarga);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaCTes(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            string retorno = (count ? "select distinct(count(0) over ())" : "select " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_CTE CTe " + joins +
                   " LEFT OUTER JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = CTe.CON_CODIGO " +
                   " LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO AND CA.CAR_SITUACAO <> 12 AND CA.CAR_SITUACAO <> 13 AND CA.CAR_SITUACAO <> 18 AND (CA.CAR_CARGA_TRANSBORDO = 0 OR CA.CAR_CARGA_TRANSBORDO IS NULL) " +
                   " where CTe.CON_STATUS = 'A' " + where +
                   (groupBy.Length > 0 ? " group by " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) + " having 1=1 " + having +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");

            return retorno;
        }

        private void SetarSelectRelatorioConsultaCTes(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "NumeroCTe":
                    if (!select.Contains("NumeroCTe"))
                    {
                        select += "CTe.CON_NUM NumeroCTe, ";
                        groupBy += "CTe.CON_NUM, ";
                    }
                    break;
                case "CodigoCTe":
                    if (!select.Contains("CodigoCTe"))
                    {
                        select += "CTe.CON_CODIGO CodigoCTe, ";
                        groupBy += "CTe.CON_CODIGO, ";
                    }
                    break;
                case "SerieCTe":
                    if (!select.Contains("SerieCTe"))
                    {
                        select += "SerieCTe.ESE_NUMERO SerieCTe, ";
                        groupBy += "SerieCTe.ESE_NUMERO, ";
                        joins += "left outer join T_EMPRESA_SERIE SerieCTe on CTe.CON_SERIE = SerieCTe.ESE_CODIGO ";
                    }
                    break;
                case "Filial":
                    if (!select.Contains("Filial"))
                    {
                        select += "LocalidadeEmitenteCTe.UF_SIGLA Filial, ";

                        joins += "inner join T_EMPRESA TransportadorCTe on CTe.EMP_CODIGO = TransportadorCTe.EMP_CODIGO ";

                        groupBy += "LocalidadeEmitenteCTe.UF_SIGLA, ";
                        joins += "inner join T_LOCALIDADES LocalidadeEmitenteCTe on TransportadorCTe.LOC_CODIGO = LocalidadeEmitenteCTe.LOC_CODIGO ";
                    }
                    break;
                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga"))
                    {
                        select += "Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ";
                        groupBy += "Carga.CAR_CODIGO_CARGA_EMBARCADOR, ";

                        if (!joins.Contains("CargaCTe"))
                            joins += "left outer join T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = CTe.CON_CODIGO ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ";
                    }
                    break;
                case "DataEmissaoCarga":
                    if (!select.Contains("DataEmissaoCarga"))
                    {
                        select += "Carga.CAR_DATA_CRIACAO DataEmissaoCarga, ";
                        groupBy += "Carga.CAR_DATA_CRIACAO, ";

                        if (!joins.Contains("CargaCTe"))
                            joins += "left outer join T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = CTe.CON_CODIGO ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ";
                    }
                    break;
                case "DataEmissaoCTe":
                    if (!select.Contains("DataEmissaoCTe"))
                    {
                        select += "CTe.CON_DATAHORAEMISSAO DataEmissaoCTe, ";
                        groupBy += "CTe.CON_DATAHORAEMISSAO, ";
                    }
                    break;
                case "DataAutorizacao":
                    if (!select.Contains("DataAutorizacao"))
                    {
                        select += "CTe.CON_RETORNOCTEDATA DataAutorizacao, ";
                        groupBy += "CTe.CON_RETORNOCTEDATA, ";
                    }
                    break;
                case "CNPJRemetente":
                    if (!select.Contains(" CNPJRemetente,"))
                    {
                        select += "RemetenteCTe.PCT_CPF_CNPJ CNPJRemetente, ";

                        if (!groupBy.Contains("RemetenteCTe.PCT_CPF_CNPJ"))
                            groupBy += "RemetenteCTe.PCT_CPF_CNPJ, ";

                        if (!joins.Contains("RemetenteCTe"))
                            joins += "left outer join T_CTE_PARTICIPANTE RemetenteCTe on CTe.CON_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO ";
                    }
                    break;
                case "Remetente":
                    if (!select.Contains(" Remetente,"))
                    {
                        select += "RemetenteCTe.PCT_NOME Remetente, ";

                        if (!groupBy.Contains("RemetenteCTe.PCT_NOME"))
                            groupBy += "RemetenteCTe.PCT_NOME, ";

                        if (!joins.Contains("RemetenteCTe"))
                            joins += "left outer join T_CTE_PARTICIPANTE RemetenteCTe on CTe.CON_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO ";
                    }
                    break;
                case "CNPJDestinatario":
                    if (!select.Contains(" CNPJDestinatario,"))
                    {
                        select += "DestinatarioCTe.PCT_CPF_CNPJ CNPJDestinatario, ";

                        if (!groupBy.Contains("DestinatarioCTe.PCT_CPF_CNPJ"))
                            groupBy += "DestinatarioCTe.PCT_CPF_CNPJ, ";

                        if (!joins.Contains("DestinatarioCTe"))
                            joins += "left outer join T_CTE_PARTICIPANTE DestinatarioCTe on CTe.CON_DESTINATARIO_CTE = DestinatarioCTe.PCT_CODIGO ";
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains(" Destinatario,"))
                    {
                        select += "DestinatarioCTe.PCT_NOME Destinatario, ";

                        if (!groupBy.Contains("DestinatarioCTe.PCT_NOME"))
                            groupBy += "DestinatarioCTe.PCT_NOME, ";

                        if (!joins.Contains("DestinatarioCTe"))
                            joins += "left outer join T_CTE_PARTICIPANTE DestinatarioCTe on CTe.CON_DESTINATARIO_CTE = DestinatarioCTe.PCT_CODIGO ";
                    }
                    break;
                case "CidadeTomador":
                    if (!select.Contains(" CidadeTomador"))
                    {
                        //select += "'' as CidadeTomador, ";
                        select += "CidadeTomadorPagadorCTe.LOC_DESCRICAO CidadeTomador, ";

                        if (!joins.Contains("TomadorPagadorCTe"))
                            joins += "left outer join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ";

                        if (!joins.Contains("CidadeTomadorPagadorCTe"))
                            joins += "left outer join T_LOCALIDADES CidadeTomadorPagadorCTe on CidadeTomadorPagadorCTe.LOC_CODIGO = TomadorPagadorCTe.LOC_CODIGO ";

                        //if (!groupBy.Contains("TomadorPagadorCTe.PCT_NOME"))
                        //    groupBy += "TomadorPagadorCTe.PCT_NOME, ";
                        if (!groupBy.Contains("CidadeTomadorPagadorCTe.LOC_DESCRICAO"))
                            groupBy += "CidadeTomadorPagadorCTe.LOC_DESCRICAO, ";
                    }
                    break;
                case "CodigoGrupo ":
                    if (!select.Contains(" CodigoGrupo "))
                    {
                        select += "0 as CodigoGrupo , ";
                    }
                    break;
                case "DescricaoGrupo":
                    if (!select.Contains(" DescricaoGrupo "))
                    {
                        select += "GrupoTomadorPagadorCTe.GRP_DESCRICAO DescricaoGrupo, ";

                        if (!groupBy.Contains("GrupoTomadorPagadorCTe.GRP_DESCRICAO"))
                            groupBy += "GrupoTomadorPagadorCTe.GRP_DESCRICAO, ";

                        if (!joins.Contains("TomadorPagadorCTe"))
                            joins += "left outer join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ";

                        if (!joins.Contains("ClienteTomadorPagadorCTe"))
                            joins += "left outer join T_CLIENTE ClienteTomadorPagadorCTe on TomadorPagadorCTe.CLI_CODIGO = ClienteTomadorPagadorCTe.CLI_CGCCPF ";

                        if (!joins.Contains("GrupoTomadorPagadorCTe"))
                            joins += "left outer join T_GRUPO_PESSOAS GrupoTomadorPagadorCTe on ClienteTomadorPagadorCTe.GRP_CODIGO = GrupoTomadorPagadorCTe.GRP_CODIGO ";
                    }
                    break;
                case "CNPJTomador":
                    if (!select.Contains(" CNPJTomador"))
                    {
                        select += "TomadorPagadorCTe.PCT_CPF_CNPJ CNPJTomador, ";

                        if (!joins.Contains("TomadorPagadorCTe"))
                            joins += "left outer join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ";

                        if (!groupBy.Contains("TomadorPagadorCTe.PCT_CPF_CNPJ"))
                            groupBy += "TomadorPagadorCTe.PCT_CPF_CNPJ, ";
                    }
                    break;
                case "Tomador":
                    if (!select.Contains(" Tomador"))
                    {
                        select += "TomadorPagadorCTe.PCT_NOME Tomador, ";

                        if (!joins.Contains("TomadorPagadorCTe"))
                            joins += "left outer join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ";

                        if (!groupBy.Contains("TomadorPagadorCTe.PCT_NOME"))
                            groupBy += "TomadorPagadorCTe.PCT_NOME, ";
                    }
                    break;
                case "Origem":
                    if (!select.Contains(" Origem,"))
                    {
                        select += " InicioPrestacaoCTe.LOC_DESCRICAO + '-' + InicioPrestacaoCTe.UF_SIGLA Origem, ";
                        groupBy += "InicioPrestacaoCTe.LOC_DESCRICAO, ";

                        if (!groupBy.Contains("InicioPrestacaoCTe.UF_SIGLA"))
                            groupBy += "InicioPrestacaoCTe.UF_SIGLA, ";

                        if (!joins.Contains(" InicioPrestacaoCTe "))
                            joins += "left outer join T_LOCALIDADES InicioPrestacaoCTe on CTe.CON_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO ";
                    }
                    break;
                case "UFOrigem":
                    if (!select.Contains("UFOrigem"))
                    {
                        select += " InicioPrestacaoCTe.UF_SIGLA UFOrigem, ";

                        if (!groupBy.Contains("InicioPrestacaoCTe.UF_SIGLA"))
                            groupBy += "InicioPrestacaoCTe.UF_SIGLA, ";

                        if (!joins.Contains(" InicioPrestacaoCTe "))
                            joins += "left outer join T_LOCALIDADES InicioPrestacaoCTe on CTe.CON_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO ";
                    }
                    break;
                case "Destino":
                    if (!select.Contains(" Destino,"))
                    {
                        select += " FimPrestacaoCTe.LOC_DESCRICAO + '-' + FimPrestacaoCTe.UF_SIGLA Destino, ";
                        groupBy += "FimPrestacaoCTe.LOC_DESCRICAO, ";

                        if (!groupBy.Contains("FimPrestacaoCTe.UF_SIGLA"))
                            groupBy += "FimPrestacaoCTe.UF_SIGLA, ";

                        if (!joins.Contains(" FimPrestacaoCTe "))
                            joins += "left outer join T_LOCALIDADES FimPrestacaoCTe on CTe.CON_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO ";
                    }
                    break;
                case "UFDestino":
                    if (!select.Contains("UFDestino"))
                    {
                        select += " FimPrestacaoCTe.UF_SIGLA UFDestino, ";

                        if (!groupBy.Contains("FimPrestacaoCTe.UF_SIGLA"))
                            groupBy += " FimPrestacaoCTe.UF_SIGLA, ";

                        if (!joins.Contains(" FimPrestacaoCTe "))
                            joins += "left outer join T_LOCALIDADES FimPrestacaoCTe on CTe.CON_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO ";
                    }
                    break;
                case "ValorReceber":
                    if (!select.Contains("ValorReceber"))
                    {
                        select += "CTe.CON_VALOR_RECEBER ValorReceber, ";

                        if (!groupBy.Contains("CTe.CON_VALOR_RECEBER"))
                            groupBy += "CTe.CON_VALOR_RECEBER, ";
                    }
                    break;
                case "Frotas":
                    if (!select.Contains(" Frotas "))
                    {
                        select += "substring((select ', ' + veiculo1.VEI_NUMERO_FROTA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Frotas, ";

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy += "CTe.CON_CODIGO, ";
                    }
                    break;
                case "Placas":
                    if (!select.Contains(" Placas "))
                    {
                        select += "substring((select ', ' + veiculo1.VEI_PLACA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Placas, ";

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy += "CTe.CON_CODIGO, ";
                    }
                    break;
                case "ProprioTerceiro":
                    if (!select.Contains(" ProprioTerceiro "))
                    {
                        select += "(select TOP(1) PVE_NOME from t_cte_veiculo_proprietario where PVE_CODIGO = Veiculo.PVE_CODIGO) ProprioTerceiro, ";

                        if (!joins.Contains(" Veiculo "))
                            joins += "left outer join T_CTE_VEICULO Veiculo on Veiculo.CON_CODIGO = CTe.CON_CODIGO ";

                        if (!groupBy.Contains("Veiculo.PVE_CODIGO"))
                            groupBy += "Veiculo.PVE_CODIGO, ";
                    }
                    break;
                case "Motoristas":
                    if (!select.Contains("Motoristas"))
                    {
                        select += "substring((select ', ' + motoristaCTe1.CMO_NOME_MOTORISTA from T_CTE_MOTORISTA motoristaCTe1 where motoristaCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Motoristas, ";

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy += "CTe.CON_CODIGO, ";
                    }
                    break;
                case "NumeroDTMinuta":
                    if (!select.Contains("NumeroDTMinuta"))
                    {
                        select += "COALESCE(DocumentoNatura.DTN_NUMERO, DocumentoAvon.MAV_NUMERO) NumeroDTMinuta, ";

                        joins += "left outer join T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL NotaFiscalNatura on NotaFiscalNatura.NDT_CODIGO = (select TOP 1 NDT_CODIGO FROM T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL where CON_CODIGO = CTe.CON_CODIGO) ";
                        joins += "left outer join T_NATURA_DOCUMENTO_TRANSPORTE DocumentoNatura on NotaFiscalNatura.DTN_CODIGO = DocumentoNatura.DTN_CODIGO ";
                        joins += "left outer join T_AVON_MANIFESTO_DOCUMENTO NotaFiscalAvon on NotaFiscalAvon.CON_CODIGO = CTe.CON_CODIGO ";
                        joins += "left outer join T_AVON_MANIFESTO DocumentoAvon on DocumentoAvon.MAV_CODIGO = NotaFiscalAvon.MAV_CODIGO ";

                        groupBy += "DocumentoAvon.MAV_NUMERO, DocumentoNatura.DTN_NUMERO, ";
                    }
                    break;
                case "Notas":
                    if (!select.Contains("Notas"))
                    {
                        select += "substring((select ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Notas, ";

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy += "CTe.CON_CODIGO, ";
                    }
                    break;
                case "CodigoFatura":
                    if (!select.Contains(" CodigoFatura,"))
                    {
                        select += "Fatura.FAT_CODIGO CodigoFatura, ";

                        if (!groupBy.Contains("Fatura.FAT_CODIGO"))
                            groupBy += "Fatura.FAT_CODIGO, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";
                    }
                    break;
                case "NumeroFatura":
                    if (!select.Contains(" NumeroFatura,"))
                    {
                        select += "(CASE WHEN Fatura.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR Fatura.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN Fatura.FAT_NUMERO ELSE Fatura.FAT_NUMERO_FATURA_INTEGRACAO END) NumeroFatura, ";

                        if (!groupBy.Contains("Fatura.FAT_NUMERO_FATURA_INTEGRACAO, "))
                            groupBy += "Fatura.FAT_NUMERO_FATURA_INTEGRACAO, ";

                        if (!groupBy.Contains("Fatura.FAT_NUMERO, "))
                            groupBy += "Fatura.FAT_NUMERO, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";
                    }
                    break;
                case "NumeroPreFatura":
                    if (!select.Contains(" NumeroPreFatura,"))
                    {
                        select += "Fatura.FAT_NUMERO_PRE_FATURA NumeroPreFatura, ";

                        if (!groupBy.Contains("Fatura.FAT_NUMERO_PRE_FATURA"))
                            groupBy += "Fatura.FAT_NUMERO_PRE_FATURA, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";
                    }
                    break;
                case "CodigoGrupoFatura":
                    if (!select.Contains(" CodigoGrupoFatura,"))
                    {
                        select += "GrupoFatura.GRP_CODIGO CodigoGrupoFatura, ";

                        if (!groupBy.Contains("GrupoFatura.GRP_CODIGO"))
                            groupBy += "GrupoFatura.GRP_CODIGO, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("GrupoFatura"))
                            joins += "left outer join T_GRUPO_PESSOAS GrupoFatura on Fatura.GRP_CODIGO = GrupoFatura.GRP_CODIGO ";
                    }
                    break;
                case "GrupoFatura":
                    if (!select.Contains(" GrupoFatura,"))
                    {
                        select += "GrupoFatura.GRP_DESCRICAO GrupoFatura, ";

                        if (!groupBy.Contains("GrupoFatura.GRP_DESCRICAO"))
                            groupBy += "GrupoFatura.GRP_DESCRICAO, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("GrupoFatura"))
                            joins += "left outer join T_GRUPO_PESSOAS GrupoFatura on Fatura.GRP_CODIGO = GrupoFatura.GRP_CODIGO ";
                    }
                    break;
                case "ClienteFatura":
                    if (!select.Contains(" ClienteFatura,"))
                    {
                        select += "ClienteFatura.CLI_NOME ClienteFatura, ";

                        if (!groupBy.Contains("ClienteFatura.CLI_NOME"))
                            groupBy += "ClienteFatura.CLI_NOME, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("ClienteFatura"))
                            joins += "left outer join T_CLIENTE ClienteFatura on Fatura.CLI_CGCCPF = ClienteFatura.CLI_CGCCPF ";
                    }
                    break;
                case "DataEmissaoFatura":
                    if (!select.Contains(" DataEmissaoFatura,"))
                    {
                        select += "Fatura.FAT_DATA_FATURA DataEmissaoFatura, ";

                        if (!groupBy.Contains("Fatura.FAT_DATA_FATURA"))
                            groupBy += "Fatura.FAT_DATA_FATURA, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";
                    }
                    break;
                case "ClienteTitulo":
                    if (!select.Contains(" ClienteTitulo,"))
                    {
                        select += "ClienteTitulo.CLI_NOME ClienteTitulo, ";

                        if (!groupBy.Contains("ClienteTitulo.CLI_NOME"))
                            groupBy += "ClienteTitulo.CLI_NOME, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("ParcelaFatura"))
                            joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                        if (!joins.Contains("Titulo"))
                            joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                        if (!joins.Contains("ClienteTitulo"))
                            joins += "left outer join T_CLIENTE ClienteTitulo on Titulo.CLI_CGCCPF = ClienteTitulo.CLI_CGCCPF ";
                    }
                    break;
                case "Modelo":
                    if (!select.Contains(" Modelo,"))
                    {
                        select += "ModeloDocumento.MOD_ABREVIACAO Modelo, ";

                        if (!groupBy.Contains("ModeloDocumento.MOD_ABREVIACAO"))
                            groupBy += "ModeloDocumento.MOD_ABREVIACAO, ";

                        if (!joins.Contains("ModeloDocumento"))
                            joins += "left outer join T_MODDOCFISCAL ModeloDocumento on CTe.CON_MODELODOC = ModeloDocumento.MOD_CODIGO ";
                    }
                    break;
                case "ComponentesFrete":
                    if (!select.Contains(" ComponentesFrete "))
                    {
                        select += "'' as ComponentesFrete , ";
                    }
                    break;
                case "DataBaseBaixa":
                    if (!select.Contains(" DataBaseBaixa,"))
                    {
                        select += "CASE WHEN MIN(TituloBaixa.TIB_DATA_BASE) = CAST('01/01/1800' AS DATE) THEN NULL ";
                        select += "ELSE MAX(TituloBaixa.TIB_DATA_BASE) END DataBaseBaixa, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("ParcelaFatura"))
                            joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                        if (!joins.Contains("Titulo"))
                            joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                        if (!joins.Contains("TituloBaixaAgrupada"))
                            joins += "left outer join T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupada on Titulo.TIT_CODIGO = TituloBaixaAgrupada.TIT_CODIGO ";

                        if (!joins.Contains("TituloBaixa "))
                            joins += "left outer join T_TITULO_BAIXA TituloBaixa on TituloBaixaAgrupada.TIB_CODIGO = TituloBaixa.TIB_CODIGO ";
                    }
                    break;
                case "DataMovimento":
                    if (!select.Contains(" DataMovimento,"))
                    {
                        select += "CASE WHEN MIN(TituloBaixa.TIB_DATA_BAIXA) = CAST('01/01/1800' AS DATE) THEN NULL ";
                        select += "ELSE MAX(TituloBaixa.TIB_DATA_BAIXA) END DataMovimento, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("ParcelaFatura"))
                            joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                        if (!joins.Contains("Titulo"))
                            joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                        if (!joins.Contains("TituloBaixaAgrupada"))
                            joins += "left outer join T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupada on Titulo.TIT_CODIGO = TituloBaixaAgrupada.TIT_CODIGO ";

                        if (!joins.Contains("TituloBaixa "))
                            joins += "left outer join T_TITULO_BAIXA TituloBaixa on TituloBaixaAgrupada.TIB_CODIGO = TituloBaixa.TIB_CODIGO ";
                    }
                    break;
                case "ValorTitulo":
                    if (!count && !select.Contains(" ValorTitulo,"))
                    {
                        select += "CASE WHEN MIN(Titulo.TIT_STATUS) = 3 THEN SUM(Titulo.TIT_VALOR_ORIGINAL) ";
                        select += " ELSE MIN(Titulo.TIT_VALOR_ORIGINAL) END ValorTitulo, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("ParcelaFatura"))
                            joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                        if (!joins.Contains("Titulo"))
                            joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";

                    }
                    break;
                case "ValorPendenteTitulo":
                    if (!count && !select.Contains(" ValorPendenteTitulo,"))
                    {
                        select += "CASE WHEN MIN(Titulo.TIT_STATUS) = 3 THEN SUM(Titulo.TIT_VALOR_PENDENTE) ";
                        select += " ELSE MIN(Titulo.TIT_VALOR_PENDENTE) END ValorPendenteTitulo, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("ParcelaFatura"))
                            joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                        if (!joins.Contains("Titulo"))
                            joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";

                    }
                    break;
                case "DataVencimentoTitulo":
                    if (!count && !select.Contains(" DataVencimentoTitulo,"))
                    {
                        select += "MAX(Titulo.TIT_DATA_VENCIMENTO) DataVencimentoTitulo, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("ParcelaFatura"))
                            joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                        if (!joins.Contains("Titulo"))
                            joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                    }
                    break;
                case "DataEmissaoTitulo":
                    if (!count && !select.Contains(" DataEmissaoTitulo,"))
                    {
                        select += "MAX(Titulo.TIT_DATA_EMISSAO) DataEmissaoTitulo, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("ParcelaFatura"))
                            joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                        if (!joins.Contains("Titulo"))
                            joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                    }
                    break;
                case "StatusTitulo":
                    if (!count && !select.Contains(" StatusTitulo,"))
                    {
                        select += "CASE WHEN MIN(Titulo.TIT_STATUS) = 1 THEN '1 - Aberto' WHEN MIN(Titulo.TIT_STATUS) = 3 THEN '2 - Quitado' ELSE '1 - Aberto' END StatusTitulo, ";

                        if (!joins.Contains("Fatura"))
                            joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                        if (!joins.Contains("ParcelaFatura"))
                            joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                        if (!joins.Contains("Titulo"))
                            joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                    }
                    break;
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaCTes(ref string where, ref string having, ref string groupBy, ref string joins, List<int> gruposPessoas, decimal valorCTeInicial, decimal valorCTeFinal, DateTime dataInicialQuitacao, DateTime dataFinalQuitacao, DateTime dataInicialMovimento, DateTime dataFinalMovimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusCTe, int codigoGrupoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, bool? cteVinculadoACarga)
        {
            if (dataInicialEmissao != DateTime.MinValue)
                where += " and CTe.CON_DATAHORAEMISSAO >= '" + dataInicialEmissao.ToString("yyyy-MM-dd") + "' ";

            if (dataFinalEmissao != DateTime.MinValue)
                where += " and CTe.CON_DATAHORAEMISSAO < '" + dataFinalEmissao.AddDays(1).ToString("yyyy-MM-dd") + "' ";

            if (valorCTeInicial > 0)
                where += " and CTe.CON_VALOR_RECEBER >= '" + valorCTeInicial.ToString().Replace(",", ".") + "' ";

            if (valorCTeFinal > 0)
                where += " and CTe.CON_VALOR_RECEBER <= '" + valorCTeFinal.ToString().Replace(",", ".") + "' ";

            if (cteVinculadoACarga.HasValue)
                where += " and CTe.CON_SEM_CARGA = " + (cteVinculadoACarga.Value ? "0" : "1") + " ";

            if (dataInicialQuitacao != DateTime.MinValue || dataFinalQuitacao > DateTime.MinValue)
            {
                if (!joins.Contains("Fatura"))
                    joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                if (!joins.Contains("ParcelaFatura"))
                    joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                if (!joins.Contains("Titulo"))
                    joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                if (!joins.Contains("TituloBaixaAgrupada"))
                    joins += "left outer join T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupada on Titulo.TIT_CODIGO = TituloBaixaAgrupada.TIT_CODIGO ";

                if (!joins.Contains("TituloBaixa "))
                    joins += "left outer join T_TITULO_BAIXA TituloBaixa on TituloBaixaAgrupada.TIB_CODIGO = TituloBaixa.TIB_CODIGO ";

                if (dataInicialQuitacao != DateTime.MinValue)
                    having += " and CASE MIN(TituloBaixa.TIB_DATA_BASE) = CAST('01/01/1800' AS DATE) THEN MIN(TituloBaixa.TIB_DATA_BASE) >= '" + dataInicialEmissao.ToString("yyyy-MM-dd") + "' ELSE MAX(TituloBaixa.TIB_DATA_BASE) >= '" + dataInicialEmissao.ToString("yyyy-MM-dd") + "' ";

                if (dataFinalQuitacao != DateTime.MinValue)
                    having += " and CASE MIN(TituloBaixa.TIB_DATA_BASE) = CAST('01/01/1800' AS DATE) THEN MIN(TituloBaixa.TIB_DATA_BASE) <= '" + dataFinalQuitacao.ToString("yyyy-MM-dd") + "' ELSE MAX(TituloBaixa.TIB_DATA_BASE) <= '" + dataFinalQuitacao.ToString("yyyy-MM-dd") + "' ";
            }

            if (dataInicialMovimento != DateTime.MinValue || dataFinalMovimento > DateTime.MinValue)
            {
                if (!joins.Contains("Fatura"))
                    joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                if (!joins.Contains("ParcelaFatura"))
                    joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                if (!joins.Contains("Titulo"))
                    joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                if (!joins.Contains("TituloBaixaAgrupada"))
                    joins += "left outer join T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupada on Titulo.TIT_CODIGO = TituloBaixaAgrupada.TIT_CODIGO ";

                if (!joins.Contains("TituloBaixa "))
                    joins += "left outer join T_TITULO_BAIXA TituloBaixa on TituloBaixaAgrupada.TIB_CODIGO = TituloBaixa.TIB_CODIGO ";

                if (dataInicialQuitacao != DateTime.MinValue)
                    having += " and CASE MIN(TituloBaixa.TIB_DATA_BAIXA) = CAST('01/01/1800' AS DATE) THEN MIN(TituloBaixa.TIB_DATA_BAIXA) >= '" + dataInicialEmissao.ToString("yyyy-MM-dd") + "' ELSE MAX(TituloBaixa.TIB_DATA_BAIXA) >= '" + dataInicialEmissao.ToString("yyyy-MM-dd") + "' ";

                if (dataFinalQuitacao != DateTime.MinValue)
                    having += " and CASE MIN(TituloBaixa.TIB_DATA_BAIXA) = CAST('01/01/1800' AS DATE) THEN MIN(TituloBaixa.TIB_DATA_BAIXA) <= '" + dataInicialEmissao.ToString("yyyy-MM-dd") + "' ELSE MAX(TituloBaixa.TIB_DATA_BAIXA) <= '" + dataInicialEmissao.ToString("yyyy-MM-dd") + "' ";
            }


            //if (dataInicialQuitacao != DateTime.MinValue)
            //    where += " and CTe.CON_DATAHORAEMISSAO < '" + dataFinalEmissao.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (statusCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada || statusCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
            {
                if (!joins.Contains("Fatura"))
                    joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                if (!joins.Contains("ParcelaFatura"))
                    joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                if (!joins.Contains("Titulo"))
                    joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                if (statusCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                    where += " and Titulo.TIT_STATUS = 3 ";
                else if (statusCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                    where += " and (Titulo.TIT_STATUS <> 3 or Titulo.TIT_STATUS is null) ";
            }

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber.Todos)
            {
                //select += "Fatura.FAT_NUMERO NumeroFatura, ";

                //if (!groupBy.Contains("Fatura.FAT_NUMERO"))
                //    groupBy += "Fatura.FAT_NUMERO, ";

                if (!joins.Contains("Fatura"))
                    joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber.CTeComFatura)
                    where += " and Fatura.FAT_NUMERO IS NOT NULL ";
                if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber.CTeSemFatura)
                    where += " and Fatura.FAT_NUMERO IS NULL ";

            }

            if (dataInicialVencimento != DateTime.MinValue || dataFinalVencimento > DateTime.MinValue)
            {
                if (!groupBy.Contains("Fatura.TIT_DATA_VENCIMENTO"))
                    groupBy += "Fatura.TIT_DATA_VENCIMENTO, ";

                if (!joins.Contains("Fatura"))
                    joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                if (!joins.Contains("ParcelaFatura"))
                    joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                if (!joins.Contains("Titulo"))
                    joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                if (dataInicialVencimento != DateTime.MinValue)
                    having += " and MAX(Titulo.TIT_DATA_VENCIMENTO) >= '" + dataInicialVencimento.ToString("yyyy-MM-dd") + "' ";

                if (dataFinalQuitacao != DateTime.MinValue)
                    having += " and MAX(Titulo.TIT_DATA_VENCIMENTO) <= '" + dataFinalVencimento.ToString("yyyy-MM-dd") + "' ";

                // having += " and CASE MIN(Titulo.TIT_DATA_VENCIMENTO) = CAST('01/01/1800' AS DATE) THEN MIN(Titulo.TIT_DATA_VENCIMENTO) <= '" + dataFinalVencimento.ToString("yyyy-MM-dd") + "' ELSE MAX(Titulo.TIT_DATA_VENCIMENTO) <= '" + dataFinalVencimento.ToString("yyyy-MM-dd") + "'";
            }

            if (status > 0)
            {
                if (!groupBy.Contains("Titulo.TIT_STATUS"))
                    groupBy += "Titulo.TIT_STATUS, ";

                if (!joins.Contains("Fatura"))
                    joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                if (!joins.Contains("ParcelaFatura"))
                    joins += "left outer join T_FATURA_PARCELA ParcelaFatura on Fatura.FAT_CODIGO = ParcelaFatura.FAT_CODIGO ";

                if (!joins.Contains("Titulo"))
                    joins += "left outer join T_TITULO Titulo on ParcelaFatura.FAP_CODIGO = Titulo.FAP_CODIGO ";

                where += " and Titulo.TIT_STATUS = " + (int)status;
            }

            if (cnpjPessoa > 0)
            {
                if (!joins.Contains("TomadorPagadorCTe"))
                    joins += "left outer join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ";

                if (!joins.Contains("ClienteTomadorPagadorCTe"))
                    joins += "left outer join T_CLIENTE ClienteTomadorPagadorCTe on TomadorPagadorCTe.CLI_CODIGO = ClienteTomadorPagadorCTe.CLI_CGCCPF ";

                where += " and ClienteTomadorPagadorCTe.CLI_CGCCPF = " + cnpjPessoa.ToString() + " ";
            }

            if (codigoCTe > 0)
            {
                where += " and CTe.CON_CODIGO = " + codigoCTe.ToString();
            }

            if (codigoFatura > 0)
            {

                if (!groupBy.Contains("Fatura.FAT_CODIGO"))
                    groupBy += "Fatura.FAT_CODIGO, ";

                if (!joins.Contains("Fatura"))
                    joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                where += " and Fatura.FAT_CODIGO = " + codigoFatura.ToString() + " ";
            }

            if (codigoGrupoPessoa > 0)
            {
                if (!groupBy.Contains("GrupoFatura.GRP_CODIGO"))
                    groupBy += "GrupoFatura.GRP_CODIGO, ";

                if (!joins.Contains("Fatura"))
                    joins += "left outer join T_FATURA Fatura on CTe.FAT_CODIGO = Fatura.FAT_CODIGO and Fatura.FAT_SITUACAO <> 3 ";

                if (!joins.Contains("GrupoFatura"))
                    joins += "left outer join T_GRUPO_PESSOAS GrupoFatura on Fatura.GRP_CODIGO = GrupoFatura.GRP_CODIGO ";

                where += " and GrupoFatura.GRP_CODIGO = " + codigoGrupoPessoa.ToString() + " ";
            }

            if (codigoGrupoCTe > 0)
            {
                where += " and ClienteTomadorPagadorCTe.GRP_CODIGO <> " + codigoGrupoCTe + " ";

                if (!joins.Contains("TomadorPagadorCTe"))
                    joins += "left outer join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ";

                if (!joins.Contains("ClienteTomadorPagadorCTe"))
                    joins += "left outer join T_CLIENTE ClienteTomadorPagadorCTe on TomadorPagadorCTe.CLI_CODIGO = ClienteTomadorPagadorCTe.CLI_CGCCPF ";
            }

            if (gruposPessoas != null && gruposPessoas.Count > 0)
            {
                IEnumerable<int> gruposPessoasValidos = gruposPessoas.Where(o => o > 0);

                if (gruposPessoasValidos.Count() > 0)
                    where += " and (ClienteTomadorPagadorCTe.GRP_CODIGO in (" + string.Join(",", gruposPessoasValidos) + ")";

                bool opcaoSemGrupo = gruposPessoas.Contains(-1);

                if (opcaoSemGrupo && gruposPessoasValidos.Count() > 0)
                    where += " or ";
                else if (opcaoSemGrupo)
                    where += " and (";

                if (opcaoSemGrupo)
                    where += " ClienteTomadorPagadorCTe.GRP_CODIGO is null";

                where += ")";

                if (!joins.Contains("TomadorPagadorCTe"))
                    joins += "left outer join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ";

                if (!joins.Contains("ClienteTomadorPagadorCTe"))
                    joins += "left outer join T_CLIENTE ClienteTomadorPagadorCTe on TomadorPagadorCTe.CLI_CODIGO = ClienteTomadorPagadorCTe.CLI_CGCCPF ";
            }

        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasReceber> RelatorioPosicaoContasReceber(List<int> gruposPessoas, decimal valorCTeInicial, decimal valorCTeFinal, DateTime dataInicialQuitacao, DateTime dataFinalQuitacao, DateTime dataInicialMovimento, DateTime dataFinalMovimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusCTe, int codigoGrupoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT T.Filial, T.NumeroCTe, T.CodigoCTe, T.SerieCTe, T.Tomador, T.CNPJTomador, T.CidadeTomador, T.CodigoGrupo, T.DescricaoGrupo, T.CNPJRemetente, T.Remetente,
		                            T.CNPJDestinatario, T.Destinatario, T.ValorReceber, T.ProprioTerceiro, T.Frotas, T.Placas, T.Motoristas, T.DataEmissaoCTe, T.Notas, T.Origem, T.UFOrigem,
		                            T.Destino, T.UFDestino, T.NumeroDTMinuta, T.NumeroCarga, T.DataEmissaoCarga, T.CodigoFatura, T.NumeroFatura, T.NumeroPreFatura, T.CodigoGrupoFatura,
		                            T.GrupoFatura, T.ClienteFatura, T.DataEmissaoFatura, T.ClienteTitulo, T.Modelo, T.ComponentesFrete, 
		                            CASE
		                            WHEN MIN(T.DataBaseBaixa) = CAST('01/01/1800' AS DATE) THEN NULL
		                            ELSE MAX(T.DataBaseBaixa)
		                            END DataBaseBaixa, 
		                            CASE
		                            WHEN MIN(T.DataMovimento) = CAST('01/01/1800' AS DATE) THEN NULL
		                            ELSE MAX(T.DataMovimento)
		                            END DataMovimento, 		  
		                            CASE 
		                            WHEN MIN(T.CodigoStatus) = 3 THEN SUM(T.ValorTitulo)
		                            ELSE MIN(T.ValorTitulo)
		                            END ValorTitulo, 
		                            MAX(T.ValorPendenteTitulo) ValorPendenteTitulo, 
		                            MAX(T.DataVencimentoTitulo) DataVencimentoTitulo,  
		                            MAX(T.DataEmissaoTitulo) DataEmissaoTitulo, 
		                            MIN(T.StatusTitulo) StatusTitulo, 
		                            MIN(T.CodigoStatus) CodigoStatus FROM (
		                            SELECT  LE.UF_SIGLA Filial, C.CON_NUM NumeroCTe,
                                            C.CON_CODIGO CodigoCTe,
                                            ES.ESE_NUMERO SerieCTe,
                                            CL.CLI_NOME Tomador,
                                            CL.CLI_CGCCPF CNPJTomador,
                                            LT.LOC_DESCRICAO CidadeTomador,
				                            G.GRP_CODIGO CodigoGrupo,
				                            G.GRP_DESCRICAO DescricaoGrupo,
				                            REM.PCT_CPF_CNPJ CNPJRemetente,
				                            REM.PCT_NOME Remetente,
				                            DEST.PCT_CPF_CNPJ CNPJDestinatario,
				                            DEST.PCT_NOME Destinatario,
                                            C.CON_VALOR_RECEBER ValorReceber,
                                            '' Frotas,
				                            '' ProprioTerceiro,
                                            '' Placas,
                                            '' Motoristas,
                                            '' Notas,
				                            '' ComponentesFrete,
                                            C.CON_DATAHORAEMISSAO DataEmissaoCTe,
                                            LI.LOC_DESCRICAO Origem,
				                            LI.UF_SIGLA UFOrigem,
                                            LF.LOC_DESCRICAO Destino,
				                            LF.UF_SIGLA UFDestino,
                                            COALESCE(CIA.CIA_NUMERO_MINUTA, INDT.IDT_NUMERO) NumeroDTMinuta,
                                            CA.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                                            CA.CAR_DATA_CRIACAO DataEmissaoCarga,
                                            F.FAT_CODIGO CodigoFatura,
                                            F.FAT_NUMERO NumeroFatura,
                                            F.FAT_NUMERO_PRE_FATURA NumeroPreFatura,
                                            GP.GRP_CODIGO CodigoGrupoFatura,
                                            GP.GRP_DESCRICAO GrupoFatura,
                                            CF.CLI_NOME ClienteFatura,
                                            F.FAT_DATA_FATURA DataEmissaoFatura,                
				                            T.TIT_CODIGO NumeroTitulo,
                                            T.TIT_VALOR_ORIGINAL ValorTitulo,
                                            T.TIT_VALOR_PENDENTE ValorPendenteTitulo,
                                            CT.CLI_NOME ClienteTitulo,
                                            T.TIT_DATA_VENCIMENTO DataVencimentoTitulo,
                                            T.TIT_DATA_EMISSAO DataEmissaoTitulo,				
                                            CASE 
	                                            WHEN T.TIT_STATUS = 1 THEN '1 - Aberto'
	                                            WHEN T.TIT_STATUS = 3 THEN '2 - Quitado'
                                                ELSE '1 - Aberto'
                                            END StatusTitulo,
                                            T.TIT_STATUS CodigoStatus,
				                            M.MOD_ABREVIACAO Modelo,
				                            ISNULL(TB.TIB_DATA_BASE, CAST('01/01/1800' AS DATE)) DataBaseBaixa,
				                            ISNULL(TB.TIB_DATA_BAIXA, CAST('01/01/1800' AS DATE)) DataMovimento
                                    FROM T_CTE C
                                    JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = C.CON_SERIE
                                    JOIN T_LOCALIDADES LI ON LI.LOC_CODIGO = C.CON_LOCINICIOPRESTACAO
                                    JOIN T_LOCALIDADES LF ON LF.LOC_CODIGO = C.CON_LOCTERMINOPRESTACAO
                                    JOIN T_EMPRESA E ON E.EMP_CODIGO = C.EMP_CODIGO
		                            JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO	
		                            JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = C.CON_MODELODOC			
                                    JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO
                                    JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO AND CA.CAR_SITUACAO <> 12 AND CA.CAR_SITUACAO <> 13 AND CA.CAR_SITUACAO <> 18 AND (CA.CAR_CARGA_TRANSBORDO = 0 OR CA.CAR_CARGA_TRANSBORDO IS NULL)
                                    LEFT OUTER JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO AND F.FAT_SITUACAO <> 3
		                            LEFT OUTER JOIN T_FATURA_PARCELA FP ON FP.FAT_CODIGO = F.FAT_CODIGO
		                            LEFT OUTER JOIN T_TITULO T ON T.FAP_CODIGO = FP.FAP_CODIGO
		                            LEFT OUTER JOIN T_CLIENTE CT ON CT.CLI_CGCCPF = T.CLI_CGCCPF
		                            LEFT OUTER JOIN T_TITULO_BAIXA_AGRUPADO TA ON TA.TIT_CODIGO = T.TIT_CODIGO
		                            LEFT OUTER JOIN T_TITULO_BAIXA TB ON TB.TIB_CODIGO = TA.TIB_CODIGO
                                    LEFT OUTER JOIN T_GRUPO_PESSOAS GP ON GP.GRP_CODIGO = F.GRP_CODIGO
                                    LEFT OUTER JOIN T_CLIENTE CF ON CF.CLI_CGCCPF = F.CLI_CGCCPF
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE DEST ON DEST.PCT_CODIGO = C.CON_DESTINATARIO_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE REM ON REM.PCT_CODIGO = C.CON_REMETENTE_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE EXPE ON EXPE.PCT_CODIGO = C.CON_EXPEDIDOR_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE OUTR ON OUTR.PCT_CODIGO = C.CON_TOMADOR_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE RECEB ON RECEB.PCT_CODIGO = C.CON_RECEBEDOR_CTE
                                    LEFT OUTER JOIN T_CARGA_INTEGRACAO_AVON CIA ON CIA.CAR_CODIGO = CA.CAR_CODIGO
                                    LEFT OUTER JOIN T_CARGA_INTEGRACAO_NATURA CIN ON CIN.CAR_CODIGO = CA.CAR_CODIGO
                                    LEFT OUTER JOIN T_INTEGRACAO_NATURA_DOCUMENTO_TRANSPORTE INDT ON INDT.IDT_CODIGO = CIN.IDT_CODIGO
		                            JOIN T_CLIENTE CL ON CL.CLI_CGCCPF = 
	                                                        CASE
		                                                        WHEN C.CON_TOMADOR = 0 THEN REM.CLI_CODIGO 
		                                                        WHEN C.CON_TOMADOR = 1 THEN EXPE.CLI_CODIGO
		                                                        WHEN C.CON_TOMADOR = 2 THEN RECEB.CLI_CODIGO
		                                                        WHEN C.CON_TOMADOR = 3 THEN DEST.CLI_CODIGO
		                                                        ELSE OUTR.CLI_CODIGO
	                                                        END                                    
                                    JOIN T_LOCALIDADES LT ON LT.LOC_CODIGO = CL.LOC_CODIGO
                                    LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = CL.GRP_CODIGO
                                    WHERE C.CON_CANCELADO = 'N' AND C.CON_STATUS = 'A')
                            AS T";

            query += @" GROUP BY T.Filial, T.NumeroCTe, T.CodigoCTe, T.SerieCTe, T.Tomador, T.CNPJTomador, T.CodigoGrupo, T.DescricaoGrupo, T.CNPJRemetente, T.Remetente,
                            T.CNPJDestinatario, T.Destinatario, T.ValorReceber, T.ProprioTerceiro, T.Placas, T.Motoristas, T.DataEmissaoCTe, T.Notas, T.Origem, T.UFOrigem,
                            T.Destino, T.UFDestino, T.NumeroDTMinuta, T.NumeroCarga, T.DataEmissaoCarga, T.CodigoFatura, T.NumeroFatura, T.NumeroPreFatura, T.CodigoGrupoFatura,
                            T.GrupoFatura, T.ClienteFatura, T.DataEmissaoFatura, T.ClienteTitulo, T.Modelo, T.ComponentesFrete, T.Frotas, T.CidadeTomador";

            query = @"SELECT TT.Filial, TT.NumeroCTe, TT.CodigoCTe, TT.SerieCTe, TT.Tomador, TT.CNPJTomador, TT.CidadeTomador, TT.CodigoGrupo, TT.DescricaoGrupo, TT.CNPJRemetente, TT.Remetente,
                             TT.CNPJDestinatario, TT.Destinatario, TT.ValorReceber, TT.ProprioTerceiro, TT.Frotas, TT.Placas, TT.Motoristas, TT.DataEmissaoCTe, TT.Notas, TT.Origem, TT.UFOrigem,
		                     TT.Destino, TT.UFDestino, TT.NumeroDTMinuta, TT.NumeroCarga, TT.DataEmissaoCarga, TT.CodigoFatura, TT.NumeroFatura, TT.NumeroPreFatura, TT.CodigoGrupoFatura,
		                     TT.GrupoFatura, TT.ClienteFatura, TT.DataEmissaoFatura, TT.ClienteTitulo, TT.Modelo, TT.ComponentesFrete, 
		                     TT.DataBaseBaixa, TT.DataMovimento, TT.ValorTitulo, TT.ValorPendenteTitulo, TT.DataVencimentoTitulo,
                             TT.DataEmissaoTitulo, TT.StatusTitulo, TT.CodigoStatus FROM (" + query + ") AS TT";

            query += " WHERE 1 = 1 ";

            if (codigoGrupoCTe > 0)
                query += " AND TT.CodigoGrupo = " + codigoGrupoCTe.ToString();

            if (gruposPessoas != null && gruposPessoas.Count > 0)
            {
                IEnumerable<int> gruposPessoasValidos = gruposPessoas.Where(o => o > 0);

                if (gruposPessoasValidos.Count() > 0)
                    query += " and (TT.CodigoGrupo in (" + string.Join(",", gruposPessoasValidos) + "))";
                else
                    query += " and (TT.CodigoGrupo IS NULL)";
            }

            if (status > 0)
                query += " AND TT.CodigoStatus = " + (int)status;

            if (codigoCTe > 0)
                query += " AND TT.CodigoCTe = " + codigoCTe.ToString();

            if (statusCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                query += " AND TT.CodigoStatus = " + (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
            else if (statusCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                query += " AND (TT.CodigoStatus <> " + (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada + " OR TT.CodigoStatus IS NULL) ";

            if (codigoFatura > 0)
                query += " AND TT.CodigoFatura = " + codigoFatura.ToString();

            if (codigoGrupoPessoa > 0)
                query += " AND TT.CodigoGrupoFatura = " + codigoGrupoPessoa.ToString();

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber.CTeComFatura)
                query += " AND TT.NumeroFatura IS NOT NULL";
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber.CTeSemFatura)
                query += " AND TT.NumeroFatura IS NULL";

            if (cnpjPessoa > 0)
                query += " AND TT.CNPJTomador = " + cnpjPessoa.ToString();

            if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND TT.DataEmissaoCTe >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' AND TT.DataEmissaoCTe <= '" + dataFinalEmissao.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                query += " AND TT.DataEmissaoCTe >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND TT.DataEmissaoCTe <= '" + dataFinalEmissao.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND TT.DataVencimentoTitulo >= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' AND TT.DataVencimentoTitulo <= '" + dataFinalVencimento.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento == DateTime.MinValue)
                query += " AND TT.DataVencimentoTitulo >= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialVencimento == DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND TT.DataVencimentoTitulo <= '" + dataFinalVencimento.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            if (valorCTeInicial > 0 && valorCTeFinal > 0)
                query += " AND TT.ValorReceber >= '" + valorCTeInicial.ToString().Replace(",", ".") + "' AND TT.ValorReceber <= '" + valorCTeFinal.ToString().Replace(",", ".") + "'";
            else if (valorCTeInicial > 0 && valorCTeFinal == 0)
                query += " AND TT.ValorReceber >= '" + valorCTeInicial.ToString().Replace(",", ".") + "' ";
            else if (valorCTeInicial == 0 && valorCTeFinal > 0)
                query += " AND TT.ValorReceber <= '" + valorCTeFinal.ToString().Replace(",", ".") + "' ";

            if (dataInicialMovimento > DateTime.MinValue && dataFinalMovimento > DateTime.MinValue)
                query += " AND TT.DataMovimento >= '" + dataInicialMovimento.ToString("MM/dd/yyyy") + "' AND TT.DataMovimento <= '" + dataFinalMovimento.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialMovimento > DateTime.MinValue && dataFinalMovimento == DateTime.MinValue)
                query += " AND TT.DataMovimento >= '" + dataInicialMovimento.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialMovimento == DateTime.MinValue && dataFinalMovimento > DateTime.MinValue)
                query += " AND TT.DataMovimento <= '" + dataFinalMovimento.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += " AND TT.DataBaseBaixa >= '" + dataInicialQuitacao.ToString("MM/dd/yyyy") + "' AND TT.DataBaseBaixa <= '" + dataFinalQuitacao.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao == DateTime.MinValue)
                query += " AND TT.DataBaseBaixa >= '" + dataInicialQuitacao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialQuitacao == DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += " AND TT.DataBaseBaixa <= '" + dataFinalQuitacao.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasReceber)));

            return nhQuery.SetTimeout(30000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasReceber>();
        }

        public int ContarPosicaoContasReceber(List<int> gruposPessoas, decimal valorCTeInicial, decimal valorCTeFinal, DateTime dataInicialQuitacao, DateTime dataFinalQuitacao, DateTime dataInicialMovimento, DateTime dataFinalMovimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusCTe, int codigoGrupoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento)
        {
            string query = @"SELECT T.Filial, T.NumeroCTe, T.CodigoCTe, T.SerieCTe, T.Tomador, T.CNPJTomador, T.CidadeTomador, T.CodigoGrupo, T.DescricaoGrupo, T.CNPJRemetente, T.Remetente,
		                            T.CNPJDestinatario, T.Destinatario, T.ValorReceber, T.ProprioTerceiro, T.Frotas, T.Placas, T.Motoristas, T.DataEmissaoCTe, T.Notas, T.Origem, T.UFOrigem,
		                            T.Destino, T.UFDestino, T.NumeroDTMinuta, T.NumeroCarga, T.DataEmissaoCarga, T.CodigoFatura, T.NumeroFatura, T.NumeroPreFatura, T.CodigoGrupoFatura,
		                            T.GrupoFatura, T.ClienteFatura, T.DataEmissaoFatura, T.ClienteTitulo, T.Modelo, T.ComponentesFrete, 
		                            CASE
		                            WHEN MIN(T.DataBaseBaixa) = CAST('01/01/1800' AS DATE) THEN NULL
		                            ELSE MAX(T.DataBaseBaixa)
		                            END DataBaseBaixa, 
		                            CASE
		                            WHEN MIN(T.DataMovimento) = CAST('01/01/1800' AS DATE) THEN NULL
		                            ELSE MAX(T.DataMovimento)
		                            END DataMovimento, 		  
		                            CASE 
		                            WHEN MIN(T.CodigoStatus) = 3 THEN SUM(T.ValorTitulo)
		                            ELSE MIN(T.ValorTitulo)
		                            END ValorTitulo, 
		                            MAX(T.ValorPendenteTitulo) ValorPendenteTitulo, 
		                            MAX(T.DataVencimentoTitulo) DataVencimentoTitulo,  
		                            MAX(T.DataEmissaoTitulo) DataEmissaoTitulo, 
		                            MIN(T.StatusTitulo) StatusTitulo, 
		                            MIN(T.CodigoStatus) CodigoStatus FROM (
		                            SELECT  LE.UF_SIGLA Filial, C.CON_NUM NumeroCTe,
                                            C.CON_CODIGO CodigoCTe,
                                            ES.ESE_NUMERO SerieCTe,
                                            CL.CLI_NOME Tomador,
                                            CL.CLI_CGCCPF CNPJTomador,
                                            LT.LOC_DESCRICAO CidadeTomador,
				                            G.GRP_CODIGO CodigoGrupo,
				                            G.GRP_DESCRICAO DescricaoGrupo,
				                            REM.PCT_CPF_CNPJ CNPJRemetente,
				                            REM.PCT_NOME Remetente,
				                            DEST.PCT_CPF_CNPJ CNPJDestinatario,
				                            DEST.PCT_NOME Destinatario,
                                            C.CON_VALOR_RECEBER ValorReceber,
                                            '' Frotas,
				                            '' ProprioTerceiro,
                                            '' Placas,
                                            '' Motoristas,
                                            '' Notas,
				                            '' ComponentesFrete,
                                            C.CON_DATAHORAEMISSAO DataEmissaoCTe,
                                            LI.LOC_DESCRICAO Origem,
				                            LI.UF_SIGLA UFOrigem,
                                            LF.LOC_DESCRICAO Destino,
				                            LF.UF_SIGLA UFDestino,
                                            COALESCE(CIA.CIA_NUMERO_MINUTA, INDT.IDT_NUMERO) NumeroDTMinuta,
                                            CA.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                                            CA.CAR_DATA_CRIACAO DataEmissaoCarga,
                                            F.FAT_CODIGO CodigoFatura,
                                            F.FAT_NUMERO NumeroFatura,
                                            F.FAT_NUMERO_PRE_FATURA NumeroPreFatura,
                                            GP.GRP_CODIGO CodigoGrupoFatura,
                                            GP.GRP_DESCRICAO GrupoFatura,
                                            CF.CLI_NOME ClienteFatura,
                                            F.FAT_DATA_FATURA DataEmissaoFatura,                
				                            T.TIT_CODIGO NumeroTitulo,
                                            T.TIT_VALOR_ORIGINAL ValorTitulo,
                                            T.TIT_VALOR_PENDENTE ValorPendenteTitulo,
                                            CT.CLI_NOME ClienteTitulo,
                                            T.TIT_DATA_VENCIMENTO DataVencimentoTitulo,
                                            T.TIT_DATA_EMISSAO DataEmissaoTitulo,				
                                            CASE 
	                                            WHEN T.TIT_STATUS = 1 THEN '1 - Aberto'
	                                            WHEN T.TIT_STATUS = 3 THEN '2 - Quitado'
                                            END StatusTitulo,
                                            T.TIT_STATUS CodigoStatus,
				                            M.MOD_ABREVIACAO Modelo,
				                            ISNULL(TB.TIB_DATA_BASE, CAST('01/01/1800' AS DATE)) DataBaseBaixa,
				                            ISNULL(TB.TIB_DATA_BAIXA, CAST('01/01/1800' AS DATE)) DataMovimento
                                    FROM T_CTE C
                                    JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = C.CON_SERIE
                                    JOIN T_LOCALIDADES LI ON LI.LOC_CODIGO = C.CON_LOCINICIOPRESTACAO
                                    JOIN T_LOCALIDADES LF ON LF.LOC_CODIGO = C.CON_LOCTERMINOPRESTACAO
                                    JOIN T_EMPRESA E ON E.EMP_CODIGO = C.EMP_CODIGO
		                            JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO	
		                            JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = C.CON_MODELODOC			
                                    JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO
                                    JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO AND CA.CAR_SITUACAO <> 12 AND CA.CAR_SITUACAO <> 13 AND CA.CAR_SITUACAO <> 18 AND (CA.CAR_CARGA_TRANSBORDO = 0 OR CA.CAR_CARGA_TRANSBORDO IS NULL)
                                    LEFT OUTER JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO AND F.FAT_SITUACAO <> 3
		                            LEFT OUTER JOIN T_FATURA_PARCELA FP ON FP.FAT_CODIGO = F.FAT_CODIGO
		                            LEFT OUTER JOIN T_TITULO T ON T.FAP_CODIGO = FP.FAP_CODIGO
		                            LEFT OUTER JOIN T_CLIENTE CT ON CT.CLI_CGCCPF = T.CLI_CGCCPF
		                            LEFT OUTER JOIN T_TITULO_BAIXA_AGRUPADO TA ON TA.TIT_CODIGO = T.TIT_CODIGO
		                            LEFT OUTER JOIN T_TITULO_BAIXA TB ON TB.TIB_CODIGO = TA.TIB_CODIGO
                                    LEFT OUTER JOIN T_GRUPO_PESSOAS GP ON GP.GRP_CODIGO = F.GRP_CODIGO
                                    LEFT OUTER JOIN T_CLIENTE CF ON CF.CLI_CGCCPF = F.CLI_CGCCPF
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE DEST ON DEST.PCT_CODIGO = C.CON_DESTINATARIO_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE REM ON REM.PCT_CODIGO = C.CON_REMETENTE_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE EXPE ON EXPE.PCT_CODIGO = C.CON_EXPEDIDOR_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE OUTR ON OUTR.PCT_CODIGO = C.CON_TOMADOR_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE RECEB ON RECEB.PCT_CODIGO = C.CON_RECEBEDOR_CTE
                                    LEFT OUTER JOIN T_CARGA_INTEGRACAO_AVON CIA ON CIA.CAR_CODIGO = CA.CAR_CODIGO
                                    LEFT OUTER JOIN T_CARGA_INTEGRACAO_NATURA CIN ON CIN.CAR_CODIGO = CA.CAR_CODIGO
                                    LEFT OUTER JOIN T_INTEGRACAO_NATURA_DOCUMENTO_TRANSPORTE INDT ON INDT.IDT_CODIGO = CIN.IDT_CODIGO
		                            JOIN T_CLIENTE CL ON CL.CLI_CGCCPF = 
	                                                        CASE
		                                                        WHEN C.CON_TOMADOR = 0 THEN REM.CLI_CODIGO 
		                                                        WHEN C.CON_TOMADOR = 1 THEN EXPE.CLI_CODIGO
		                                                        WHEN C.CON_TOMADOR = 2 THEN RECEB.CLI_CODIGO
		                                                        WHEN C.CON_TOMADOR = 3 THEN DEST.CLI_CODIGO
		                                                        ELSE OUTR.CLI_CODIGO
	                                                        END
                                    JOIN T_LOCALIDADES LT ON LT.LOC_CODIGO = CL.LOC_CODIGO
                                    LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = CL.GRP_CODIGO                                    
                                    WHERE C.CON_CANCELADO = 'N' AND C.CON_STATUS = 'A')
                            AS T";

            query += @" GROUP BY T.Filial, T.NumeroCTe, T.CodigoCTe, T.SerieCTe, T.Tomador, T.CNPJTomador, T.CodigoGrupo, T.DescricaoGrupo, T.CNPJRemetente, T.Remetente,
                            T.CNPJDestinatario, T.Destinatario, T.ValorReceber, T.ProprioTerceiro, T.Placas, T.Motoristas, T.DataEmissaoCTe, T.Notas, T.Origem, T.UFOrigem,
                            T.Destino, T.UFDestino, T.NumeroDTMinuta, T.NumeroCarga, T.DataEmissaoCarga, T.CodigoFatura, T.NumeroFatura, T.NumeroPreFatura, T.CodigoGrupoFatura,
                            T.GrupoFatura, T.ClienteFatura, T.DataEmissaoFatura, T.ClienteTitulo, T.Modelo, T.ComponentesFrete, T.Frotas, T.CidadeTomador";

            query = @"SELECT TT.Filial, TT.NumeroCTe, TT.CodigoCTe, TT.SerieCTe, TT.Tomador, TT.CNPJTomador, TT.CidadeTomador, TT.CodigoGrupo, TT.DescricaoGrupo, TT.CNPJRemetente, TT.Remetente,
                             TT.CNPJDestinatario, TT.Destinatario, TT.ValorReceber, TT.ProprioTerceiro, TT.Frotas, TT.Placas, TT.Motoristas, TT.DataEmissaoCTe, TT.Notas, TT.Origem, TT.UFOrigem,
		                     TT.Destino, TT.UFDestino, TT.NumeroDTMinuta, TT.NumeroCarga, TT.DataEmissaoCarga, TT.CodigoFatura, TT.NumeroFatura, TT.NumeroPreFatura, TT.CodigoGrupoFatura,
		                     TT.GrupoFatura, TT.ClienteFatura, TT.DataEmissaoFatura, TT.ClienteTitulo, TT.Modelo, TT.ComponentesFrete, 
		                     TT.DataBaseBaixa, TT.DataMovimento, TT.ValorTitulo, TT.ValorPendenteTitulo, TT.DataVencimentoTitulo,
                             TT.DataEmissaoTitulo, TT.StatusTitulo, TT.CodigoStatus FROM (" + query + ") AS TT";

            query += " WHERE 1 = 1 ";

            if (codigoGrupoCTe > 0)
                query += " AND TT.CodigoGrupo = " + codigoGrupoCTe.ToString();

            if (gruposPessoas != null && gruposPessoas.Count > 0)
            {
                IEnumerable<int> gruposPessoasValidos = gruposPessoas.Where(o => o > 0);

                if (gruposPessoasValidos.Count() > 0)
                    query += " and (TT.CodigoGrupo in (" + string.Join(",", gruposPessoasValidos) + "))";
                else
                    query += " and (TT.CodigoGrupo IS NULL)";
            }

            if (status > 0)
                query += " AND TT.CodigoStatus = " + (int)status;

            if (codigoCTe > 0)
                query += " AND TT.CodigoCTe = " + codigoCTe.ToString();

            if (statusCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                query += " AND TT.CodigoStatus = " + (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
            else if (statusCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                query += " AND (TT.CodigoStatus <> " + (int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada + " OR TT.CodigoStatus IS NULL) ";

            if (codigoFatura > 0)
                query += " AND TT.CodigoFatura = " + codigoFatura.ToString();

            if (codigoGrupoPessoa > 0)
                query += " AND TT.CodigoGrupoFatura = " + codigoGrupoPessoa.ToString();

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber.CTeComFatura)
                query += " AND TT.NumeroFatura IS NOT NULL";
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber.CTeSemFatura)
                query += " AND TT.NumeroFatura IS NULL";

            if (cnpjPessoa > 0)
                query += " AND TT.CNPJTomador = " + cnpjPessoa.ToString();

            if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND TT.DataEmissaoCTe >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' AND TT.DataEmissaoCTe <= '" + dataFinalEmissao.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                query += " AND TT.DataEmissaoCTe >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND TT.DataEmissaoCTe <= '" + dataFinalEmissao.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND TT.DataVencimentoTitulo >= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' AND TT.DataVencimentoTitulo <= '" + dataFinalVencimento.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento == DateTime.MinValue)
                query += " AND TT.DataVencimentoTitulo >= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialVencimento == DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND TT.DataVencimentoTitulo <= '" + dataFinalVencimento.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            if (valorCTeInicial > 0 && valorCTeFinal > 0)
                query += " AND TT.ValorReceber >= '" + valorCTeInicial.ToString().Replace(",", ".") + "' AND TT.ValorReceber <= '" + valorCTeFinal.ToString().Replace(",", ".") + "'";
            else if (valorCTeInicial > 0 && valorCTeFinal == 0)
                query += " AND TT.ValorReceber >= '" + valorCTeInicial.ToString().Replace(",", ".") + "' ";
            else if (valorCTeInicial == 0 && valorCTeFinal > 0)
                query += " AND TT.ValorReceber <= '" + valorCTeFinal.ToString().Replace(",", ".") + "' ";

            if (dataInicialMovimento > DateTime.MinValue && dataFinalMovimento > DateTime.MinValue)
                query += " AND TT.DataMovimento >= '" + dataInicialMovimento.ToString("MM/dd/yyyy") + "' AND TT.DataMovimento <= '" + dataFinalMovimento.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialMovimento > DateTime.MinValue && dataFinalMovimento == DateTime.MinValue)
                query += " AND TT.DataMovimento >= '" + dataInicialMovimento.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialMovimento == DateTime.MinValue && dataFinalMovimento > DateTime.MinValue)
                query += " AND TT.DataMovimento <= '" + dataFinalMovimento.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += " AND TT.DataBaseBaixa >= '" + dataInicialQuitacao.ToString("MM/dd/yyyy") + "' AND TT.DataBaseBaixa <= '" + dataFinalQuitacao.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao == DateTime.MinValue)
                query += " AND TT.DataBaseBaixa >= '" + dataInicialQuitacao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialQuitacao == DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += " AND TT.DataBaseBaixa <= '" + dataFinalQuitacao.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            query = @"SELECT COUNT(0) as CONTADOR
                FROM (" + query + ") AS TTT";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(30000).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoCTe> RelatorioDescontoAcrescimoCTe(int codigoGrupoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT C.CON_NUM NumeroCTe,
                                    ES.ESE_NUMERO SerieCTe,
                                    CASE
	                                    WHEN C.CON_TOMADOR = 0 THEN REM.PCT_NOME 
	                                    WHEN C.CON_TOMADOR = 1 THEN EXPE.PCT_NOME
	                                    WHEN C.CON_TOMADOR = 2 THEN RECEB.PCT_NOME
	                                    WHEN C.CON_TOMADOR = 3 THEN DEST.PCT_NOME
	                                    ELSE OUTR.PCT_NOME
                                    END Tomador,
                                    G.GRP_DESCRICAO Grupo,
                                    C.CON_DATAHORAEMISSAO DataEmissaoCTe,
                                    TB.TIB_DATA_BAIXA DataPagamentoTitulo,
                                    F.FAT_NUMERO NumeroFatura,
                                    SUBSTRING((SELECT DISTINCT ', ' + CAST(titulos.TIT_CODIGO AS NVARCHAR(20))
                                    FROM T_TITULO_BAIXA tituloBaixa
                                    inner join T_TITULO_BAIXA_AGRUPADO titulos ON titulos.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                    WHERE tituloBaixa.TIB_CODIGO = TB.TIB_CODIGO AND titulos.TIT_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000) NumeroTitulo,
                                    C.CON_VALOR_RECEBER ValorReceber,
                                    TD.TDC_VALOR_ACRESCIMO ValorAcrescimo,
                                    JA.JUS_DESCRICAO MotivoAcrescimo,
                                    TD.TDC_VALOR_DESCONTO ValorDesconto,
                                    JD.JUS_DESCRICAO MotivoDesconto,
                                    TD.TDC_VALOR_PAGO ValorPago,
                                     (SELECT TOP 1 
                                    CASE 
	                                    WHEN T.TIT_STATUS = 1 THEN 'Aberto'
	                                    WHEN T.TIT_STATUS = 3 THEN 'Quitado'
                                    END
                                    FROM T_TITULO T
                                    WHERE T.TIT_CODIGO IN (SELECT TIT_CODIGO FROM T_TITULO_BAIXA_AGRUPADO TA WHERE TA.TIB_CODIGO = TB.TIB_CODIGO)) StatusTitulo,
                                    G.GRP_CODIGO CodigoGrupo,
                                    (SELECT TOP 1 t.TIT_STATUS
                                    FROM T_TITULO T
                                    WHERE T.TIT_CODIGO IN (SELECT TIT_CODIGO FROM T_TITULO_BAIXA_AGRUPADO TA WHERE TA.TIB_CODIGO = TB.TIB_CODIGO)) CodigoStaus,
                                    C.CON_CODIGO CodigoCTe,
                                    F.FAT_CODIGO CodigoFatura,
                                    F.GRP_CODIGO CodigoGrupoFatura,
                                    CASE
	                                    WHEN C.CON_TOMADOR = 0 THEN REM.CLI_CODIGO 
	                                    WHEN C.CON_TOMADOR = 1 THEN EXPE.CLI_CODIGO
	                                    WHEN C.CON_TOMADOR = 2 THEN RECEB.CLI_CODIGO
	                                    WHEN C.CON_TOMADOR = 3 THEN DEST.CLI_CODIGO
	                                    ELSE OUTR.CLI_CODIGO
                                    END CNPJTomador
                                    FROM T_TITULO_BAIXA_DETALHE_CONHECIMENTO TD
                                    JOIN T_CTE C ON C.CON_CODIGO = TD.CON_CODIGO
                                    JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = C.CON_SERIE
                                    JOIN T_TITULO_BAIXA TB ON TB.TIB_CODIGO = TD.TIB_CODIGO
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE DEST ON DEST.PCT_CODIGO = C.CON_DESTINATARIO_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE REM ON REM.PCT_CODIGO = C.CON_REMETENTE_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE EXPE ON EXPE.PCT_CODIGO = C.CON_EXPEDIDOR_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE OUTR ON OUTR.PCT_CODIGO = C.CON_TOMADOR_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE RECEB ON RECEB.PCT_CODIGO = C.CON_RECEBEDOR_CTE
                                    LEFT OUTER JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO AND F.FAT_SITUACAO <> 3
                                    LEFT OUTER JOIN T_JUSTIFICATIVA JD ON JD.JUS_CODIGO = TD.JUS_CODIGO_DESCONTO
                                    LEFT OUTER JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = TD.JUS_CODIGO_ACRESCIMO
                                    LEFT OUTER JOIN T_CLIENTE CL ON CL.CLI_CGCCPF = 
	                                    CASE
		                                    WHEN C.CON_TOMADOR = 0 THEN REM.CLI_CODIGO 
		                                    WHEN C.CON_TOMADOR = 1 THEN EXPE.CLI_CODIGO
		                                    WHEN C.CON_TOMADOR = 2 THEN RECEB.CLI_CODIGO
		                                    WHEN C.CON_TOMADOR = 3 THEN DEST.CLI_CODIGO
		                                    ELSE OUTR.CLI_CODIGO
	                                    END
                                    LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = CL.GRP_CODIGO";

            query = @"SELECT T.NumeroCTe,
                            T.SerieCTe,
                            T.Tomador,
                            T.Grupo,
                            T.DataEmissaoCTe,
                            T.DataPagamentoTitulo,
                            T.NumeroFatura,
                            T.NumeroTitulo,
                            T.ValorReceber,
                            T.ValorAcrescimo,
                            T.MotivoAcrescimo,
                            T.ValorDesconto,
                            T.MotivoDesconto,
                            T.ValorPago,
                            T.StatusTitulo
                FROM (" + query + ") AS T ";
            query += " WHERE 1 = 1 ";

            if (codigoGrupoCTe > 0)
                query += " AND T.CodigoGrupo = " + codigoGrupoCTe.ToString();

            if (status > 0)
                query += " AND T.CodigoStaus = " + (int)status;

            if (codigoCTe > 0)
                query += " AND T.CodigoCTe = " + codigoCTe.ToString();

            if (codigoFatura > 0)
                query += " AND T.CodigoFatura = " + codigoFatura.ToString();

            if (codigoGrupoPessoa > 0)
                query += " AND T.CodigoGrupoFatura = " + codigoGrupoPessoa.ToString();

            if (cnpjPessoa > 0)
                query += " AND T.CNPJTomador = " + cnpjPessoa.ToString();

            if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND T.DataEmissaoCTe >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' AND T.DataEmissaoCTe <= '" + dataFinalEmissao.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                query += " AND T.DataEmissaoCTe >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND T.DataEmissaoCTe <= '" + dataFinalEmissao.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND T.DataPagamentoTitulo >= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' AND T.DataPagamentoTitulo <= '" + dataFinalVencimento.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento == DateTime.MinValue)
                query += " AND T.DataPagamentoTitulo >= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialVencimento == DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND T.DataPagamentoTitulo <= '" + dataFinalVencimento.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoCTe)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoCTe>();
        }

        public int ContarDescontoAcrescimoCTe(int codigoGrupoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento)
        {
            string query = @"SELECT C.CON_NUM NumeroCTe,
                                    ES.ESE_NUMERO SerieCTe,
                                    CASE
	                                    WHEN C.CON_TOMADOR = 0 THEN REM.PCT_NOME 
	                                    WHEN C.CON_TOMADOR = 1 THEN EXPE.PCT_NOME
	                                    WHEN C.CON_TOMADOR = 2 THEN RECEB.PCT_NOME
	                                    WHEN C.CON_TOMADOR = 3 THEN DEST.PCT_NOME
	                                    ELSE OUTR.PCT_NOME
                                    END Tomador,
                                    G.GRP_DESCRICAO Grupo,
                                    C.CON_DATAHORAEMISSAO DataEmissaoCTe,
                                    TB.TIB_DATA_BAIXA DataPagamentoTitulo,
                                    F.FAT_NUMERO NumeroFatura,
                                    SUBSTRING((SELECT DISTINCT ', ' + CAST(titulos.TIT_CODIGO AS NVARCHAR(20))
                                    FROM T_TITULO_BAIXA tituloBaixa
                                    inner join T_TITULO_BAIXA_AGRUPADO titulos ON titulos.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                    WHERE tituloBaixa.TIB_CODIGO = TB.TIB_CODIGO AND titulos.TIT_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000) NumeroTitulo,
                                    C.CON_VALOR_RECEBER ValorReceber,
                                    TD.TDC_VALOR_ACRESCIMO ValorAcrescimo,
                                    JA.JUS_DESCRICAO MotivoAcrescimo,
                                    TD.TDC_VALOR_DESCONTO ValorDesconto,
                                    JD.JUS_DESCRICAO MotivoDesconto,
                                    TD.TDC_VALOR_PAGO ValorPago,
                                     (SELECT TOP 1 
                                    CASE 
	                                    WHEN T.TIT_STATUS = 1 THEN 'Aberto'
	                                    WHEN T.TIT_STATUS = 3 THEN 'Quitado'
                                    END
                                    FROM T_TITULO T
                                    WHERE T.TIT_CODIGO IN (SELECT TIT_CODIGO FROM T_TITULO_BAIXA_AGRUPADO TA WHERE TA.TIB_CODIGO = TB.TIB_CODIGO)) StatusTitulo,
                                    G.GRP_CODIGO CodigoGrupo,
                                    (SELECT TOP 1 t.TIT_STATUS
                                    FROM T_TITULO T
                                    WHERE T.TIT_CODIGO IN (SELECT TIT_CODIGO FROM T_TITULO_BAIXA_AGRUPADO TA WHERE TA.TIB_CODIGO = TB.TIB_CODIGO)) CodigoStaus,
                                    C.CON_CODIGO CodigoCTe,
                                    F.FAT_CODIGO CodigoFatura,
                                    F.GRP_CODIGO CodigoGrupoFatura,
                                    CASE
	                                    WHEN C.CON_TOMADOR = 0 THEN REM.CLI_CODIGO 
	                                    WHEN C.CON_TOMADOR = 1 THEN EXPE.CLI_CODIGO
	                                    WHEN C.CON_TOMADOR = 2 THEN RECEB.CLI_CODIGO
	                                    WHEN C.CON_TOMADOR = 3 THEN DEST.CLI_CODIGO
	                                    ELSE OUTR.CLI_CODIGO
                                    END CNPJTomador
                                    FROM T_TITULO_BAIXA_DETALHE_CONHECIMENTO TD
                                    JOIN T_CTE C ON C.CON_CODIGO = TD.CON_CODIGO
                                    JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = C.CON_SERIE
                                    JOIN T_TITULO_BAIXA TB ON TB.TIB_CODIGO = TD.TIB_CODIGO
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE DEST ON DEST.PCT_CODIGO = C.CON_DESTINATARIO_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE REM ON REM.PCT_CODIGO = C.CON_REMETENTE_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE EXPE ON EXPE.PCT_CODIGO = C.CON_EXPEDIDOR_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE OUTR ON OUTR.PCT_CODIGO = C.CON_TOMADOR_CTE
                                    LEFT OUTER JOIN T_CTE_PARTICIPANTE RECEB ON RECEB.PCT_CODIGO = C.CON_RECEBEDOR_CTE
                                    LEFT OUTER JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO AND F.FAT_SITUACAO <> 3
                                    LEFT OUTER JOIN T_JUSTIFICATIVA JD ON JD.JUS_CODIGO = TD.JUS_CODIGO_DESCONTO
                                    LEFT OUTER JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = TD.JUS_CODIGO_ACRESCIMO
                                    LEFT OUTER JOIN T_CLIENTE CL ON CL.CLI_CGCCPF = 
	                                    CASE
		                                    WHEN C.CON_TOMADOR = 0 THEN REM.CLI_CODIGO 
		                                    WHEN C.CON_TOMADOR = 1 THEN EXPE.CLI_CODIGO
		                                    WHEN C.CON_TOMADOR = 2 THEN RECEB.CLI_CODIGO
		                                    WHEN C.CON_TOMADOR = 3 THEN DEST.CLI_CODIGO
		                                    ELSE OUTR.CLI_CODIGO
	                                    END
                                    LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = CL.GRP_CODIGO";

            query = @"SELECT COUNT(0) as CONTADOR
                FROM (" + query + ") AS T WHERE 1 = 1 ";

            if (codigoGrupoCTe > 0)
                query += " AND T.CodigoGrupo = " + codigoGrupoCTe.ToString();

            if (status > 0)
                query += " AND T.CodigoStaus = " + (int)status;

            if (codigoCTe > 0)
                query += " AND T.CodigoCTe = " + codigoCTe.ToString();

            if (codigoFatura > 0)
                query += " AND T.CodigoFatura = " + codigoFatura.ToString();

            if (codigoGrupoPessoa > 0)
                query += " AND T.CodigoGrupoFatura = " + codigoGrupoPessoa.ToString();

            if (cnpjPessoa > 0)
                query += " AND T.CNPJTomador = " + cnpjPessoa.ToString();

            if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND T.DataEmissaoCTe >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' AND T.DataEmissaoCTe <= '" + dataFinalEmissao.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                query += " AND T.DataEmissaoCTe >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND T.DataEmissaoCTe <= '" + dataFinalEmissao.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND T.DataPagamentoTitulo >= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' AND T.DataPagamentoTitulo <= '" + dataFinalVencimento.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento == DateTime.MinValue)
                query += " AND T.DataPagamentoTitulo >= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialVencimento == DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND T.DataPagamentoTitulo <= '" + dataFinalVencimento.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoFatura> RelatorioDescontoAcrescimoFatura(DateTime? dataInicialQuitacao, DateTime? dataFinalQuitacao, List<int> gruposPessoas, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime? dataInicialEmissao, DateTime? dataFinalEmissao, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT F.FAT_NUMERO NumeroFatura, 
                FAT_DATA_INICIAL DataInicial,
                FAT_DATA_FINAL DataFinal,

                CASE
					WHEN J.JUS_TIPO = 2 THEN
						(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
						JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
						WHERE FA.FAT_CODIGO = F.FAT_CODIGO)						
					ELSE 0
				END TotalAcrescimos,
				CASE
					WHEN J.JUS_TIPO = 1 THEN
						(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
						JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
						WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 						
					ELSE 0
				END TotalDescontos,

                FAT_TOTAL - (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) + 

				(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO)			
				TotalFatura,

                ISNULL(FAT_OBSERVACAO, '') + ' ' + ISNULL(FAT_OBSERVACAO_FATURA, '')  Observacao,
                G.GRP_DESCRICAO Grupo,
                C.CLI_NOME Pessoa,
                CASE
	                WHEN J.JUS_TIPO = 1 THEN 'Desconto'
	                ELSE 'Acréscimo'
                END Tipo,
                J.JUS_DESCRICAO Justificativa, 
                FAD.FAD_VALOR Valor, F.FAT_SITUACAO, F.FAT_CODIGO, F.GRP_CODIGO, F.CLI_CGCCPF,
                (SELECT TOP 1 T.TIT_DATA_LIQUIDACAO FROM T_TITULO T
				JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO AND FP.FAT_CODIGO = F.FAT_CODIGO ORDER BY T.TIT_CODIGO DESC) DataQuitacao

                FROM T_FATURA F
                LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = F.GRP_CODIGO
                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = F.CLI_CGCCPF
                JOIN T_FATURA_ACRESCIMO_DESCONTO FAD ON FAD.FAT_CODIGO = F.FAT_CODIGO
                JOIN T_JUSTIFICATIVA J ON J.JUS_CODIGO = FAD.JUS_CODIGO";

            query = "SELECT T.NumeroFatura, T.DataInicial, T.DataFinal, T.TotalAcrescimos, T.TotalDescontos, T.TotalFatura, T.Observacao, T.Grupo, T.Pessoa, T.Tipo, T.Justificativa, T.Valor, T.FAT_SITUACAO, T.FAT_CODIGO, T.GRP_CODIGO, T.CLI_CGCCPF, T.DataQuitacao FROM (" + query + ") AS T "; // SQL-INJECTION-SAFE

            query += " WHERE T.FAT_SITUACAO <> 3 ";

            if (codigoCTe > 0)
                query += " AND T.FAT_CODIGO IN (SELECT C.FAT_CODIGO FROM T_FATURA_CARGA_DOCUMENTO C WHERE C.FCD_STATUS_DOCUMENTO = 1 AND C.CON_CODIGO = " + codigoCTe.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoFatura > 0)
                query += " AND T.FAT_CODIGO = " + codigoFatura.ToString();

            if (codigoGrupoPessoa > 0)
                query += " AND T.GRP_CODIGO = " + codigoGrupoPessoa.ToString();

            if (gruposPessoas != null && gruposPessoas.Count > 0)
            {
                IEnumerable<int> gruposPessoasValidos = gruposPessoas.Where(o => o > 0);

                if (gruposPessoasValidos.Count() > 0)
                    query += " and (T.GRP_CODIGO in (" + string.Join(",", gruposPessoasValidos) + "))";
                else
                    query += " and (T.GRP_CODIGO IS NULL)";
            }

            if (cnpjPessoa > 0)
                query += " AND T.CLI_CGCCPF = " + cnpjPessoa.ToString();

            if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND T.DataInicial >= '" + dataInicialEmissao?.ToString("MM/dd/yyyy") + "' AND T.DataFinal <= '" + dataFinalEmissao?.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                query += " AND T.DataInicial >= '" + dataInicialEmissao?.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND T.DataFinal <= '" + dataFinalEmissao?.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += " AND T.DataQuitacao >= '" + dataInicialQuitacao?.ToString("MM/dd/yyyy") + "' AND T.DataQuitacao <= '" + dataFinalQuitacao?.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao == DateTime.MinValue)
                query += " AND T.DataQuitacao >= '" + dataInicialQuitacao?.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialQuitacao == DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += " AND T.DataQuitacao <= '" + dataFinalQuitacao?.ToString("MM/dd/yyyy 23:59:59") + "' ";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo; 
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoFatura)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoFatura>();
        }

        public int ContarDescontoAcrescimoFatura(DateTime? dataInicialQuitacao, DateTime? dataFinalQuitacao, List<int> gruposPessoas, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime? dataInicialEmissao, DateTime? dataFinalEmissao)
        {
            string query = @"SELECT F.FAT_NUMERO NumeroFatura, 
                FAT_DATA_INICIAL DataInicial,
                FAT_DATA_FINAL DataFinal,

                CASE
					WHEN J.JUS_TIPO = 2 THEN
						(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
						JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
						WHERE FA.FAT_CODIGO = F.FAT_CODIGO)						
					ELSE 0
				END TotalAcrescimos,
				CASE
					WHEN J.JUS_TIPO = 1 THEN
						(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
						JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
						WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 						
					ELSE 0
				END TotalDescontos,

                FAT_TOTAL - (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) + 

				(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO)			
				TotalFatura,

                ISNULL(FAT_OBSERVACAO, '') + ' ' + ISNULL(FAT_OBSERVACAO_FATURA, '')  Observacao,
                G.GRP_DESCRICAO Grupo,
                C.CLI_NOME Pessoa,
                CASE
	                WHEN J.JUS_TIPO = 1 THEN 'Desconto'
	                ELSE 'Acréscimo'
                END Tipo,
                J.JUS_DESCRICAO Justificativa, 
                FAD.FAD_VALOR Valor, F.FAT_SITUACAO, F.FAT_CODIGO, F.GRP_CODIGO, F.CLI_CGCCPF,
                (SELECT TOP 1 T.TIT_DATA_LIQUIDACAO FROM T_TITULO T
				JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO AND FP.FAT_CODIGO = F.FAT_CODIGO ORDER BY T.TIT_CODIGO DESC) DataQuitacao

                FROM T_FATURA F
                LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = F.GRP_CODIGO
                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = F.CLI_CGCCPF
                JOIN T_FATURA_ACRESCIMO_DESCONTO FAD ON FAD.FAT_CODIGO = F.FAT_CODIGO
                JOIN T_JUSTIFICATIVA J ON J.JUS_CODIGO = FAD.JUS_CODIGO";

            query = "SELECT T.NumeroFatura, T.DataInicial, T.DataFinal, T.TotalAcrescimos, T.TotalDescontos, T.TotalFatura, T.Observacao, T.Grupo, T.Pessoa, T.Tipo, T.Justificativa, T.Valor, T.FAT_SITUACAO, T.FAT_CODIGO, T.GRP_CODIGO, T.CLI_CGCCPF, T.DataQuitacao FROM (" + query + ") AS T "; // SQL-INJECTION-SAFE

            query += " WHERE T.FAT_SITUACAO <> 3 ";

            if (codigoCTe > 0)
                query += " AND T.FAT_CODIGO IN (SELECT C.FAT_CODIGO FROM T_FATURA_CARGA_DOCUMENTO C WHERE C.FCD_STATUS_DOCUMENTO = 1 AND C.CON_CODIGO = " + codigoCTe.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoFatura > 0)
                query += " AND T.FAT_CODIGO = " + codigoFatura.ToString();

            if (codigoGrupoPessoa > 0)
                query += " AND T.GRP_CODIGO = " + codigoGrupoPessoa.ToString();

            if (gruposPessoas != null && gruposPessoas.Count > 0)
            {
                IEnumerable<int> gruposPessoasValidos = gruposPessoas.Where(o => o > 0);

                if (gruposPessoasValidos.Count() > 0)
                    query += " and (T.GRP_CODIGO in (" + string.Join(",", gruposPessoasValidos) + "))";
                else
                    query += " and (T.GRP_CODIGO IS NULL)";
            }

            if (cnpjPessoa > 0)
                query += " AND T.CLI_CGCCPF = " + cnpjPessoa.ToString();

            if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND T.DataInicial >= '" + dataInicialEmissao?.ToString("MM/dd/yyyy") + "' AND T.DataFinal <= '" + dataFinalEmissao?.AddDays(1).ToString("MM/dd/yyyy") + "'";
            else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                query += " AND T.DataInicial >= '" + dataInicialEmissao?.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND T.DataFinal <= '" + dataFinalEmissao?.AddDays(1).ToString("MM/dd/yyyy") + "' ";

            if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += " AND T.DataQuitacao >= '" + dataInicialQuitacao?.ToString("MM/dd/yyyy") + "' AND T.DataQuitacao <= '" + dataFinalQuitacao?.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao == DateTime.MinValue)
                query += " AND T.DataQuitacao >= '" + dataInicialQuitacao?.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialQuitacao == DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += " AND T.DataQuitacao <= '" + dataFinalQuitacao?.ToString("MM/dd/yyyy 23:59:59") + "' ";

            query = @"SELECT COUNT(0) as CONTADOR
                FROM (" + query + ") AS TT WHERE 1 = 1 ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.PosicaoCTe> RelatorioPosicaoCTe(bool somenteDiaInformado, bool somenteAvon, string statusCTe, bool somenteCTesFaturados, DateTime dataPosicao, int codigoTransportadora, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string sqlFatura = "";
            if (somenteCTesFaturados)
                sqlFatura = " JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO AND F.FAT_DATA_FATURA < '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + "' ";
            else
                sqlFatura = " LEFT OUTER JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO ";

            string querySelect = "";
            if (somenteAvon)
            {
                querySelect = @"SELECT 1 Codigo,
                            1 NumeroCTe,
                            1 SerieCTe,
                            '1' NumeroCarga,
                            'Autorizado' StatusCTe,
                            NULL DataEmissao,
                            NULL DataAutorizacao,
                            NULL DataCancelamento,
                            NULL DataAnulacao,
                            NULL DataImportacao,
                            NULL DataVinculoCarga,
                            '' CPFCNPJRemetente,
                            '' Remetente,
                            '' CPFCNPJDestinatario,
                            'AVON' Destinatario,
                            '' CPFCNPJTomador,
                            'AVON' Tomador, 
                            'AVON' InicioPrestacao,
                            'UF' UFInicioPrestacao,
                            'AVON' FimPrestacao,
                            'UF' UFFimPrestacao,
                            'AVON' Transportador,
                            0.0 AliquotaICMS,
                            SUM(C.CON_VAL_ICMS) ValorICMS,
                            SUM(C.CON_VALOR_FRETE) ValorFrete,
                            SUM(C.CON_VALOR_RECEBER) ValorReceber,
                            SUM(C.CON_VALOR_TOTAL_MERC) ValorMercadoria,
                            'AVON' ChaveCTe,
                            NULL DataFatura ";
            }
            else
            {
                querySelect = @"SELECT C.CON_CODIGO Codigo,
                            C.CON_NUM NumeroCTe,
                            S.ESE_NUMERO SerieCTe,
                            CA.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                            CASE
	                            WHEN C.CON_STATUS = 'C' AND C.CON_DATA_CANCELAMENTO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 'Autorizado'
	                            WHEN C.CON_STATUS = 'Z' AND C.CON_DATA_ANULACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN 'Autorizado'
	                            WHEN C.CON_STATUS = 'A' AND C.CON_DATA_AUTORIZACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN 'Salvo'
	                            WHEN C.CON_STATUS = 'P' THEN 'Pendente'
	                            WHEN C.CON_STATUS = 'E' THEN 'Enviado'
	                            WHEN C.CON_STATUS = 'S' THEN 'Salvo'
	                            WHEN C.CON_STATUS = 'A' THEN 'Autorizado'
	                            WHEN C.CON_STATUS = 'C' THEN 'Cancelado'
	                            WHEN C.CON_STATUS = 'Z' THEN 'Anulado'
	                            WHEN C.CON_STATUS = 'R' THEN 'Rejeitado'
	                            WHEN C.CON_STATUS = 'I' THEN 'Inutilizado'
	                            WHEN C.CON_STATUS = 'S' THEN 'Em Digitação'
	                            WHEN C.CON_STATUS = 'D' THEN 'Denegado'
	                            ELSE C.CON_STATUS
                            END StatusCTe,
                            C.CON_DATAHORAEMISSAO DataEmissao,
                            CASE
	                            WHEN C.CON_DATA_AUTORIZACAO IS NOT NULL AND C.CON_DATA_AUTORIZACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN NULL
	                            ELSE C.CON_DATA_AUTORIZACAO
                            END DataAutorizacao,
                            CASE
	                            WHEN C.CON_DATA_CANCELAMENTO IS NOT NULL AND C.CON_DATA_CANCELAMENTO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN NULL
	                            ELSE C.CON_DATA_CANCELAMENTO
                            END DataCancelamento,
                            CASE
	                            WHEN C.CON_DATA_ANULACAO IS NOT NULL AND C.CON_DATA_ANULACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN NULL
	                            ELSE C.CON_DATA_ANULACAO
                            END DataAnulacao,
                            C.CON_DATA_INTEGRACAO DataImportacao,
                            CC.CCT_DATA_VINCULO_CARGA DataVinculoCarga,
                            REMETENTE.PCT_CPF_CNPJ CPFCNPJRemetente,
                            REMETENTE.PCT_NOME Remetente,
                            DESTINATARIO.PCT_CPF_CNPJ CPFCNPJDestinatario,
                            DESTINATARIO.PCT_NOME Destinatario,
                            TOMADOR.PCT_CPF_CNPJ CPFCNPJTomador,
                            TOMADOR.PCT_NOME Tomador, 
                            INICIO.LOC_DESCRICAO InicioPrestacao,
                            INICIO.UF_SIGLA UFInicioPrestacao,
                            FIM.LOC_DESCRICAO FimPrestacao,
                            FIM.UF_SIGLA UFFimPrestacao,
                            E.EMP_FANTASIA Transportador,
                            C.CON_ALIQ_ICMS AliquotaICMS,
                            C.CON_VAL_ICMS ValorICMS,
                            C.CON_VALOR_FRETE ValorFrete,
                            C.CON_VALOR_RECEBER ValorReceber,
                            C.CON_VALOR_TOTAL_MERC ValorMercadoria,
                            C.CON_CHAVECTE ChaveCTe,
                            F.FAT_DATA_FATURA DataFatura ";
            }

            string query = @" FROM T_CTE C
                            JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = C.CON_SERIE
                            LEFT OUTER JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO
                            LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
                            JOIN T_CTE_PARTICIPANTE TOMADOR ON TOMADOR.PCT_CODIGO = C.CON_TOMADOR_PAGADOR_CTE
                            JOIN T_CLIENTE CLI ON CLI.CLI_CGCCPF = TOMADOR.CLI_CODIGO 
                            JOIN T_CTE_PARTICIPANTE REMETENTE ON C.CON_REMETENTE_CTE = REMETENTE.PCT_CODIGO
                            JOIN T_CTE_PARTICIPANTE DESTINATARIO ON C.CON_DESTINATARIO_CTE = DESTINATARIO.PCT_CODIGO
                            JOIN T_LOCALIDADES INICIO ON INICIO.LOC_CODIGO = C.CON_LOCINICIOPRESTACAO
                            JOIN T_LOCALIDADES FIM ON FIM.LOC_CODIGO = C.CON_LOCTERMINOPRESTACAO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = C.EMP_CODIGO " + sqlFatura + @"
                            WHERE (CA.CAR_CARGA_TRANSBORDO IS NULL OR CA.CAR_CARGA_TRANSBORDO = 0) ";

            if (somenteAvon)
                query += "AND (CLI.GRP_CODIGO = 1025)";
            else
                query += "AND (CLI.GRP_CODIGO <> 1025 OR CLI.GRP_CODIGO IS NULL)";

            if (somenteCTesFaturados)
                query += " AND C.FAT_CODIGO IS NOT NULL ";
            else
                query += " AND ((C.FAT_CODIGO IS NULL OR F.FAT_DATA_FATURA >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + "')  OR (F.FAT_SITUACAO = 1 AND (not F.FAT_DATA_FATURA >= '" + dataPosicao.ToString("yyyy-MM-dd") + "')))";

            if (codigoTransportadora > 0)
                query += " AND C.EMP_CODIGO = " + codigoTransportadora.ToString();

            if (!somenteDiaInformado)
                query += " AND C.CON_DATA_AUTORIZACAO < '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + "' ";
            else
                query += " AND C.CON_DATA_AUTORIZACAO >= '" + dataPosicao.ToString("yyyy-MM-dd") + "' AND C.CON_DATA_AUTORIZACAO < '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + "' ";

            if (statusCTe != "0")
            {
                query += @" AND (CASE
                                    WHEN C.CON_STATUS = 'C' AND C.CON_DATA_CANCELAMENTO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 'A'
                                    WHEN C.CON_STATUS = 'Z' AND C.CON_DATA_ANULACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN 'A'
                                    WHEN C.CON_STATUS = 'A' AND C.CON_DATA_AUTORIZACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN 'S'
                                    ELSE C.CON_STATUS
                                END) = '" + statusCTe + "'";
            }
            else
            {
                query += @" AND (CASE
                                    WHEN C.CON_STATUS = 'C' AND C.CON_DATA_CANCELAMENTO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 'A'
                                    WHEN C.CON_STATUS = 'Z' AND C.CON_DATA_ANULACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN 'A'
                                    WHEN C.CON_STATUS = 'A' AND C.CON_DATA_AUTORIZACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN 'S'
                                    ELSE C.CON_STATUS
                                END) in ('A', 'C', 'Z')";
            }

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            query = querySelect + query;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.PosicaoCTe)));

            return nhQuery.SetTimeout(60000).List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.PosicaoCTe>();
        }

        public int ContarRelatorioPosicaoCTe(bool somenteDiaInformado, bool somenteAvon, string statusCTe, bool somenteCTesFaturados, DateTime dataPosicao, int codigoTransportadora)
        {
            string sqlFatura = "";
            if (somenteCTesFaturados)
                sqlFatura = " JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO AND F.FAT_DATA_FATURA < '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + "' ";
            else
                sqlFatura = " LEFT OUTER JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO ";

            string querySelect = "";
            if (somenteAvon)
            {
                querySelect = @"SELECT CASE
	                                        WHEN COUNT(0) > 0 THEN 1
	                                        ELSE 0
	                                   END CONTADOR ";
            }
            else
            {
                querySelect = "SELECT COUNT(0) as CONTADOR ";
            }

            string query = @" FROM T_CTE C
                            JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = C.CON_SERIE
                            LEFT OUTER JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO
                            LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
                            JOIN T_CTE_PARTICIPANTE TOMADOR ON TOMADOR.PCT_CODIGO = C.CON_TOMADOR_PAGADOR_CTE
                            JOIN T_CLIENTE CLI ON CLI.CLI_CGCCPF = TOMADOR.CLI_CODIGO
                            JOIN T_CTE_PARTICIPANTE REMETENTE ON C.CON_REMETENTE_CTE = REMETENTE.PCT_CODIGO
                            JOIN T_CTE_PARTICIPANTE DESTINATARIO ON C.CON_DESTINATARIO_CTE = DESTINATARIO.PCT_CODIGO
                            JOIN T_LOCALIDADES INICIO ON INICIO.LOC_CODIGO = C.CON_LOCINICIOPRESTACAO
                            JOIN T_LOCALIDADES FIM ON FIM.LOC_CODIGO = C.CON_LOCTERMINOPRESTACAO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = C.EMP_CODIGO " + sqlFatura + @"
                            WHERE (CA.CAR_CARGA_TRANSBORDO IS NULL OR CA.CAR_CARGA_TRANSBORDO = 0) ";
            if (somenteAvon)
                query += "AND (CLI.GRP_CODIGO = 1025)";
            else
                query += "AND (CLI.GRP_CODIGO <> 1025 OR CLI.GRP_CODIGO IS NULL)";

            if (codigoTransportadora > 0)
                query += " AND C.EMP_CODIGO = " + codigoTransportadora.ToString();

            if (somenteCTesFaturados)
                query += " AND C.FAT_CODIGO IS NOT NULL ";
            else
                query += " AND ((C.FAT_CODIGO IS NULL OR F.FAT_DATA_FATURA >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + "')  OR (F.FAT_SITUACAO = 1 AND (not F.FAT_DATA_FATURA >= '" + dataPosicao.ToString("yyyy-MM-dd") + "')))";

            if (statusCTe != "0")
            {
                query += @" AND (CASE
                                    WHEN C.CON_STATUS = 'C' AND C.CON_DATA_CANCELAMENTO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 'A'
                                    WHEN C.CON_STATUS = 'Z' AND C.CON_DATA_ANULACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN 'A'
                                    WHEN C.CON_STATUS = 'A' AND C.CON_DATA_AUTORIZACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN 'S'
                                    ELSE C.CON_STATUS
                                END) = '" + statusCTe + "'";
            }
            else
            {
                query += @" AND (CASE
                                    WHEN C.CON_STATUS = 'C' AND C.CON_DATA_CANCELAMENTO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 'A'
                                    WHEN C.CON_STATUS = 'Z' AND C.CON_DATA_ANULACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN 'A'
                                    WHEN C.CON_STATUS = 'A' AND C.CON_DATA_AUTORIZACAO >= '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"'  THEN 'S'
                                    ELSE C.CON_STATUS
                                END) in ('A', 'C', 'Z')";
            }

            if (!somenteDiaInformado)
                query += " AND C.CON_DATA_AUTORIZACAO < '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + "' ";
            else
                query += " AND C.CON_DATA_AUTORIZACAO >= '" + dataPosicao.ToString("yyyy-MM-dd") + "' AND C.CON_DATA_AUTORIZACAO < '" + dataPosicao.AddDays(1).ToString("yyyy-MM-dd") + "' ";

            query = querySelect + query;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(60000).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FaturamentoMensal> RelatorioFaturamentoMensal(int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataFaturamentoMensal tipoData, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string queryParameters = "", queryTipoData = "";
            if (codigoEmpresa > 0)
                queryParameters += " AND EMP_CODIGO = " + codigoEmpresa.ToString();

            if ((int)tipoData == 1)
                queryTipoData += "TIT_DATA_EMISSAO";
            else if ((int)tipoData == 2)
                queryTipoData += "TIT_DATA_VENCIMENTO";
            else
                queryTipoData += "TIT_DATA_LIQUIDACAO";

            if (dataInicial != DateTime.MinValue)
                queryParameters += " AND " + queryTipoData + " >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                queryParameters += " AND " + queryTipoData + " <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if (tipoAmbiente > 0)
                queryParameters += " AND TIT_AMBIENTE = " + (int)tipoAmbiente;

            string query = @"   SELECT Mes, Ano, ValorReceber, ValorPagar, (ValorReceber - ValorPagar) Total  FROM(

                                SELECT MesCodigo, 
                                CASE 
	                                WHEN MesCodigo = 1 THEN 'Janeiro'
	                                WHEN MesCodigo = 2 THEN 'Fevereiro'
	                                WHEN MesCodigo = 3 THEN 'Março'
	                                WHEN MesCodigo = 4 THEN 'Abril'
	                                WHEN MesCodigo = 5 THEN 'Maio'
	                                WHEN MesCodigo = 6 THEN 'Junho'
	                                WHEN MesCodigo = 7 THEN 'Julho'
	                                WHEN MesCodigo = 8 THEN 'Agosto'
	                                WHEN MesCodigo = 9 THEN 'Setembro'
	                                WHEN MesCodigo = 10 THEN 'Outubro'
	                                WHEN MesCodigo = 11 THEN 'Novembro'
	                                WHEN MesCodigo = 12 THEN 'Dezembro'
	                                ELSE 'Indefinido'
                                END Mes,
                                ANO, SUM(ValorReceber) ValorReceber, SUM(ValorPagar) ValorPagar FROM(

                                SELECT MONTH(" + queryTipoData + @") MesCodigo,
                                '' Mes, 
                                YEAR(" + queryTipoData + @") Ano, 
                                0 ValorReceber, 
                                ISNULL(SUM(TIT_VALOR_ORIGINAL),0) ValorPagar

                                FROM T_TITULO
                                WHERE TIT_TIPO = 2 AND TIT_STATUS <> 4 AND " + queryTipoData + @" IS NOT NULL " + queryParameters + @"
                                GROUP BY MONTH(" + queryTipoData + @"), YEAR(" + queryTipoData + @")

                                UNION

                                SELECT MONTH(" + queryTipoData + @") MesCodigo,
                                '' Mes, 
                                YEAR(" + queryTipoData + @") Ano, 
                                ISNULL(SUM(TIT_VALOR_ORIGINAL),0) ValorReceber, 
                                0 ValorPagar

                                FROM T_TITULO
                                WHERE TIT_TIPO = 1 AND TIT_STATUS <> 4 AND " + queryTipoData + @" IS NOT NULL " + queryParameters + @"
                                GROUP BY MONTH(" + queryTipoData + @"), YEAR(" + queryTipoData + @")) AS T
                                GROUP BY MesCodigo, Ano) AS TT ";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                if (propGrupo == "Mes")
                    propGrupo = "MesCodigo";
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (propOrdenacao == "Mes")
                propOrdenacao = "MesCodigo";

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by ANO " + dirOrdenacao + ", MesCodigo " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.FaturamentoMensal)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FaturamentoMensal>();
        }

        public int ContarRelatorioFaturamentoMensal(int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataFaturamentoMensal tipoData, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            string queryParameters = "", queryTipoData = "";
            if (codigoEmpresa > 0)
                queryParameters += " AND EMP_CODIGO = " + codigoEmpresa.ToString();

            if ((int)tipoData == 1)
                queryTipoData += "TIT_DATA_EMISSAO";
            else if ((int)tipoData == 2)
                queryTipoData += "TIT_DATA_VENCIMENTO";
            else
                queryTipoData += "TIT_DATA_LIQUIDACAO";

            if (dataInicial != DateTime.MinValue)
                queryParameters += " AND " + queryTipoData + " >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                queryParameters += " AND " + queryTipoData + " <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if (tipoAmbiente > 0)
                queryParameters += " AND TIT_AMBIENTE = " + (int)tipoAmbiente;

            string query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT COUNT(0) as CONTADOR FROM(

                                SELECT MONTH(" + queryTipoData + @") MesCodigo,
                                '' Mes, 
                                YEAR(" + queryTipoData + @") Ano, 
                                0 ValorReceber, 
                                ISNULL(SUM(TIT_VALOR_ORIGINAL),0) ValorPagar

                                FROM T_TITULO
                                WHERE TIT_TIPO = 2 AND TIT_STATUS <> 4 AND " + queryTipoData + @" IS NOT NULL " + queryParameters + @"
                                GROUP BY MONTH(" + queryTipoData + @"), YEAR(" + queryTipoData + @")

                                UNION

                                SELECT MONTH(" + queryTipoData + @") MesCodigo,
                                '' Mes, 
                                YEAR(" + queryTipoData + @") Ano, 
                                ISNULL(SUM(TIT_VALOR_ORIGINAL),0) ValorReceber, 
                                0 ValorPagar

                                FROM T_TITULO
                                WHERE TIT_TIPO = 1 AND TIT_STATUS <> 4 AND " + queryTipoData + @" IS NOT NULL " + queryParameters + @"
                                GROUP BY MONTH(" + queryTipoData + @"), YEAR(" + queryTipoData + @")) AS T
                                GROUP BY MesCodigo, Ano) AS TT ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PerfilCliente> RelatorioPerfilCliente(int codigoEmpresa, double cnpjPessoa, string estado, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string queryEmpresa = "", queryParameters = "";
            if (codigoEmpresa > 0)
                queryEmpresa += " AND T.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (cnpjPessoa > 0)
                queryParameters += " AND C.CLI_CGCCPF = '" + cnpjPessoa.ToString() + "'";

            if (!string.IsNullOrWhiteSpace(estado))
                queryParameters += " AND L.UF_SIGLA = '" + estado + "'";

            if (tipoAmbiente > 0)
                queryEmpresa += " AND T.TIT_AMBIENTE = " + (int)tipoAmbiente;

            string query = @"   SELECT C.CLI_CGCCPF CNPJPessoa,
                                C.CLI_NOME Pessoa,
                                C.CLI_FONE Telefone,
                                (SELECT AVG(TIT_VALOR_ORIGINAL) FROM T_TITULO T
                                WHERE TIT_STATUS <> 4 AND TIT_TIPO = 1 AND CLI_CGCCPF = C.CLI_CGCCPF " + queryEmpresa + @" ) MediaCompras,
                                MAX(TIT_VALOR_ORIGINAL) MaiorCompra,
                                (SELECT TOP 1 TIT_VALOR_ORIGINAL FROM T_TITULO T
                                WHERE TIT_STATUS <> 4 AND TIT_TIPO = 1 AND CLI_CGCCPF = C.CLI_CGCCPF " + queryEmpresa + @" ORDER BY TIT_CODIGO DESC) UltimaCompra,
                                (SELECT TOP 1 TIT_DATA_EMISSAO FROM T_TITULO T
                                WHERE TIT_STATUS <> 4 AND TIT_TIPO = 1 AND CLI_CGCCPF = C.CLI_CGCCPF " + queryEmpresa + @" ORDER BY TIT_CODIGO DESC) DataUltimaCompra,
                                (SELECT TOP 1 TIT_DATA_VENCIMENTO FROM T_TITULO T
                                WHERE TIT_STATUS <> 4 AND TIT_TIPO = 1 AND CLI_CGCCPF = C.CLI_CGCCPF " + queryEmpresa + @" ORDER BY TIT_CODIGO DESC) VencimentoUltimaCompra,
                                (SELECT TOP 1 TIT_DATA_VENCIMENTO FROM T_TITULO T
                                WHERE TIT_STATUS = 1 AND TIT_TIPO = 1 AND CLI_CGCCPF = C.CLI_CGCCPF " + queryEmpresa + @" ORDER BY TIT_DATA_VENCIMENTO ASC) ProximoVencimento,
                                (SELECT TOP 1 TIT_VALOR_PENDENTE FROM T_TITULO T
                                WHERE TIT_STATUS = 1 AND TIT_TIPO = 1 AND CLI_CGCCPF = C.CLI_CGCCPF " + queryEmpresa + @" ORDER BY TIT_DATA_VENCIMENTO ASC) ValorProximoVencimento,
                                (SELECT TOP 1 TIT_DATA_EMISSAO FROM T_TITULO T
                                WHERE TIT_STATUS <> 4 AND T.TIT_TIPO = 1 AND CLI_CGCCPF = C.CLI_CGCCPF " + queryEmpresa + @" ORDER BY TIT_CODIGO ASC) DataPrimeiraCompra,
                                SUM(T.TIT_VALOR_PAGO) TotalPago,
                                SUM(T.TIT_VALOR_ORIGINAL) TotalGeral,
                                SUM(T.TIT_VALOR_PENDENTE) TotalPendente,
                                (SELECT ISNULL(SUM(TIT_VALOR_PENDENTE),0) FROM T_TITULO T
                                WHERE TIT_STATUS <> 4 AND TIT_STATUS <> 3 AND TIT_TIPO = 1 AND CLI_CGCCPF = C.CLI_CGCCPF " + queryEmpresa + @" AND TIT_DATA_VENCIMENTO >= GETDATE()) TotalVencer,
                                (SELECT ISNULL(SUM(TIT_VALOR_PENDENTE),0) FROM T_TITULO T
                                WHERE TIT_STATUS <> 4 AND TIT_STATUS <> 3 AND TIT_TIPO = 1 AND CLI_CGCCPF = C.CLI_CGCCPF " + queryEmpresa + @" AND TIT_DATA_VENCIMENTO < GETDATE()) TotalVencido,
                                COUNT(TIT_CODIGO) QuantidadeTitulos
                                FROM T_TITULO T
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = T.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                WHERE T.TIT_STATUS <> 4 AND T.TIT_TIPO = 1 " + queryEmpresa + queryParameters + @"
                                GROUP BY C.CLI_CGCCPF, C.CLI_NOME, C.CLI_FONE ";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.PerfilCliente)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PerfilCliente>();
        }

        public int ContarRelatorioPerfilCliente(int codigoEmpresa, double cnpjPessoa, string estado, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            string queryEmpresa = "", queryParameters = "";
            if (codigoEmpresa > 0)
                queryEmpresa += " AND T.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (cnpjPessoa > 0)
                queryParameters += " AND C.CLI_CGCCPF = '" + cnpjPessoa.ToString() + "'";

            if (!string.IsNullOrWhiteSpace(estado))
                queryParameters += " AND L.UF_SIGLA = '" + estado + "'";

            if (tipoAmbiente > 0)
                queryEmpresa += " AND T.TIT_AMBIENTE = " + (int)tipoAmbiente;

            string query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT C.CLI_CGCCPF CNPJPessoa
                                FROM T_TITULO T
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = T.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                WHERE T.TIT_STATUS <> 4 AND T.TIT_TIPO = 1 " + queryEmpresa + queryParameters + @"
                                GROUP BY C.CLI_CGCCPF, C.CLI_NOME, C.CLI_FONE) AS T ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioFrancesinha> RelatorioFrancesinha(List<int> codigosTitulos)
        {
            string query = @"   SELECT
                                TIT_NOSSO_NUMERO NossoNumero,
                                TIT_CODIGO NumeroDocumento, 
                                CLI_NOME NomePessoa,
                                C.CLI_CGCCPF CNPJPessoa,
                                B.BCF_DESCRICAO_BANCO + ' / ' + B.BCF_NUMERO_AGENCIA + '-' + B.BCF_DIGITO_AGENCIA + ' / ' + B.BCF_NUMERO_CONTA BancoAgenciaConta,
                                TIT_DATA_EMISSAO DataEmissao, 
                                TIT_DATA_VENCIMENTO DataVencimento,
                                TIT_VALOR_ORIGINAL Valor,
                                BRE_NUMERO_SEQUENCIAL Remessa,

                                E.EMP_CNPJ CNPJEmpresa,
                                E.EMP_FANTASIA FantasiaEmpresa,
                                E.EMP_ENDERECO EnderecoEmpresa,
                                E.EMP_NUMERO NumeroEnderecoEmpresa,
                                E.EMP_CEP CEPEmpresa,
                                E.EMP_BAIRRO BairroEmpresa,
                                E.EMP_FONE FoneEmpresa,
                                LEMP.LOC_DESCRICAO CidadeEmpresa,
                                LEMP.UF_SIGLA EstadoEmpresa

                                FROM T_TITULO T                                
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = T.CLI_CGCCPF
                                JOIN T_BOLETO_CONFIGURACAO B ON B.BCF_CODIGO = T.BCF_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = B.EMP_CODIGO
                                JOIN T_LOCALIDADES LEMP ON LEMP.LOC_CODIGO = E.LOC_CODIGO
                                LEFT OUTER JOIN T_BOLETO_REMESSA R ON R.BRE_CODIGO = T.BRE_CODIGO
                                WHERE ";

            var codigos = "";
            if (codigosTitulos.Count > 0)
                for (int i = 0; i < codigosTitulos.Count; i++)
                {
                    if (String.IsNullOrWhiteSpace(codigos))
                        codigos = codigosTitulos[i].ToString();
                    else
                        codigos = codigos + ", " + codigosTitulos[i].ToString();
                }

            query += " T.TIT_CODIGO IN (" + codigos + ")";
            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioFrancesinha)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioFrancesinha>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixa> ConsultarRelatorioFluxoCaixa(DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, int codigoEmpresa, TipoAmbiente tipoAmbiente, ProvisaoPesquisaTitulo provisaoPesquisaTitulo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string ambienteTitulo = "";
            string empresaTitulo = "";
            string dataVencimentoTitulo = " AND T.TIT_DATA_VENCIMENTO >= '" + dataVencimentoInicial.ToString("MM/dd/yyyy") + "' AND T.TIT_DATA_VENCIMENTO <= '" + dataVencimentoFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            string provisaoPesquisa = "";

            if (provisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SemProvisao)
                provisaoPesquisa = "AND T.TIT_PROVISAO = 0";
            else if (provisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SomenteProvisao)
                provisaoPesquisa = "AND T.TIT_PROVISAO = 1";

            if (tipoAmbiente > 0)
                ambienteTitulo = " AND T.TIT_AMBIENTE  = " + Convert.ToString((int)tipoAmbiente);

            if (codigoEmpresa > 0)
                empresaTitulo = " AND T.EMP_CODIGO = " + codigoEmpresa;

            string sqlQuery = @"SELECT TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL + ' / ' + CAST(TIT_SEQUENCIA AS VARCHAR) Documento,
                                CLI_NOME Pessoa,
                                C.CLI_CGCCPF CNPJCPFPessoa,
                                TIT_DATA_VENCIMENTO DataVencimento,
                                CASE
	                                WHEN TIT_TIPO = 2 THEN TIT_VALOR_PENDENTE
	                                ELSE 0
                                END ValorPagar,
                                CASE
	                                WHEN TIT_TIPO = 1 THEN TIT_VALOR_PENDENTE
	                                ELSE 0
                                END ValorReceber
                                FROM T_TITULO T
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = T.CLI_CGCCPF
                                WHERE T.TIT_VALOR_PENDENTE > 0 " + empresaTitulo + ambienteTitulo + provisaoPesquisa + dataVencimentoTitulo;

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixa)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixa>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixaConta> ConsultarRelatorioFluxoCaixaConta(int codigoEmpresa, int codigoTipoPagamentoRecebimento, TipoAmbiente tipoAmbiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string ambienteMovimento = "";
            string empresaMovimento = "";
            string tipoPagamento = "";

            if (codigoTipoPagamentoRecebimento > 0)
                tipoPagamento = " AND T.TPR_CODIGO  = " + codigoTipoPagamentoRecebimento;

            if (tipoAmbiente > 0)
                ambienteMovimento = " AND M.MOV_AMBIENTE  = " + Convert.ToString((int)tipoAmbiente);

            if (codigoEmpresa > 0)
                empresaMovimento = " AND M.EMP_CODIGO = " + codigoEmpresa + " AND T.EMP_CODIGO = " + codigoEmpresa;

            string sqlQuery = @" SELECT TPR_DESCRICAO DescricaoPagamentoRecebimento, SUM(ValorDebito) - SUM(ValorCredito) Saldo, TPR_LIMITE_CONTA LimiteConta
                                FROM(
                                SELECT MDC_VALOR, MDC_TIPO, TPR_DESCRICAO, TPR_LIMITE_CONTA,
                                CASE
	                                WHEN MDC_TIPO = 1 THEN MDC_VALOR
	                                ELSE 0
                                END ValorDebito,
                                CASE
	                                WHEN MDC_TIPO = 2 THEN MDC_VALOR
	                                ELSE 0
                                END ValorCredito
                                FROM T_TIPO_PAGAMENTO_RECEBIMENTO T
                                LEFT OUTER JOIN T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MD ON T.PLA_CODIGO = MD.PLA_CODIGO
                                LEFT OUTER JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MD.MOV_CODIGO
                                WHERE TPR_ATIVO = 1 " + empresaMovimento + ambienteMovimento + tipoPagamento + @"
                                ) AS T
                                GROUP BY TPR_DESCRICAO, TPR_LIMITE_CONTA ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixaConta)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixaConta>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AutorizacaoPagamentoTitulo> RelatorioAutorizacaoPagamentoTitulo(int codigoTitulo, string usuario)
        {
            string query = @"   SELECT
                                TIT_CODIGO Codigo,
                                TIT_DATA_EMISSAO DataEmissao,
                                TIT_DATA_VENCIMENTO DataVencimento,
                                TIT_DATA_LIQUIDACAO DataLiquidacao,
                                TIT_SEQUENCIA Sequencia,
                                TIT_VALOR_ORIGINAL Valor,
                                TIT_VALOR_PAGO ValorPago,
                                TIT_DESCONTO Desconto,
                                TIT_ACRESCIMO Acrescimo,
                                TIT_OBSERVACAO Observacao,
                                TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL TipoDocumentoTituloOriginal,
                                TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumentoTituloOriginal,
                                TM.TIM_DESCRICAO TipoMovimento,

                                E.EMP_FANTASIA FantasiaEmpresa,
                                E.EMP_CNPJ CNPJEmpresa,                                
                                E.EMP_TIPO TipoEmpresa,
                                E.EMP_CODIGO_INTEGRACAO CodigoIntegracaoEmpresa,

                                C.CLI_NOME NomePessoa,
                                C.CLI_CGCCPF CNPJPessoa,
                                C.CLI_FISJUR TipoPessoa,
                                C.CLI_CODIGO_INTEGRACAO CodigoIntegracaoPessoa,

                                T.TIT_MOEDA_COTACAO_BANCO_CENTRAL Moeda, 
                                T.TIT_VALOR_MOEDA_COTACAO ValorMoeda, 
                                T.TIT_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorOriginalMoeda,
								'" + usuario + @"' Usuario,
								Portador.CLI_NOME NomePortador,
								Portador.CLI_CGCCPF CNPJPortador,
								Portador.CLI_FISJUR TipoPortador,
								C.CLI_ENDERECO RuaPessoa,
								C.CLI_NUMERO NumeroPessoa,
								C.CLI_BAIRRO BairroPessoa,
								C.CLI_COMPLEMENTO ComplementoPessoa,
								L.LOC_DESCRICAO CidadePessoa,
								L.UF_SIGLA UFPessoa,
								ISNULL(BancoPortador.BCO_DESCRICAO, Banco.BCO_DESCRICAO) Banco,
								ISNULL(Portador.CLI_BANCO_AGENCIA, C.CLI_BANCO_AGENCIA) Agencia,
								ISNULL(Portador.CLI_BANCO_DIGITO_AGENCIA, C.CLI_BANCO_DIGITO_AGENCIA) DigitoAgencia,
								ISNULL(Portador.CLI_BANCO_NUMERO_CONTA, C.CLI_BANCO_NUMERO_CONTA) NumeroConta
                                FROM T_TITULO T
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = T.CLI_CGCCPF
								JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                LEFT OUTER JOIN T_TIPO_MOVIMENTO TM ON TM.TIM_CODIGO = T.TIM_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = T.EMP_CODIGO
								LEFT OUTER JOIN T_CLIENTE Portador on Portador.CLI_CGCCPF = T.CLI_CGCCPF_PORTADOR
								LEFT OUTER JOIN T_BANCO Banco on Banco.BCO_CODIGO = C.BCO_CODIGO
								LEFT OUTER JOIN T_BANCO BancoPortador on BancoPortador.BCO_CODIGO = Portador.BCO_CODIGO
                                WHERE T.TIT_CODIGO =  " + codigoTitulo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.AutorizacaoPagamentoTitulo)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AutorizacaoPagamentoTitulo>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasPagar> RelatorioPosicaoContasPagar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPosicaoContasPagar filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string query = @" SELECT Titulo.TIT_CODIGO Codigo,
                                CategoriaPessoa.CTP_DESCRICAO CategoriaFornecedor,
                                Fornecedor.CLI_CONTA_FORNECEDOR_EBS ContaContabilFornecedor,
                                Fornecedor.CLI_FISJUR TipoFornecedor,
                                Empresa.EMP_RAZAO Filial,
                                Fornecedor.CLI_CGCCPF CPFCNPJFornecedor,
                                Fornecedor.CLI_NOME Fornecedor,
                                Titulo.TIT_TIPO Tipo,
                                Titulo.TIT_DATA_EMISSAO DataEmissao,
                                Titulo.TIT_DATA_VENCIMENTO DataVencimento,
                                CASE WHEN MIN(TituloBaixa.TIB_DATA_BASE) = CAST('01/01/1800' AS DATE) OR Titulo.TIT_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN NULL
                                ELSE MAX(TituloBaixa.TIB_DATA_BASE) END DataBaseBaixa,
                                CASE WHEN Titulo.TIT_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN NULL ELSE Titulo.TIT_DATA_LIQUIDACAO END DataPagamento,
                                Titulo.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL TipoDocumento,
                                Titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumento,
                                CAST(Titulo.TIT_SEQUENCIA AS NVARCHAR(20)) +
                                CASE
                                    WHEN Titulo.TBN_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.TBN_CODIGO = Titulo.TBN_CODIGO)
                                    WHEN Titulo.TDD_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.TDD_CODIGO = Titulo.TDD_CODIGO)
                                    WHEN Titulo.CFT_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.CFT_CODIGO = Titulo.CFT_CODIGO)
                                    WHEN Titulo.NFI_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.NFI_CODIGO = Titulo.NFI_CODIGO)
                                    WHEN Titulo.FAP_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.FAP_CODIGO = Titulo.FAP_CODIGO) 
                                    WHEN Titulo.CMA_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.CMA_CODIGO = Titulo.CMA_CODIGO) 
                                    WHEN ContratoFinanciamentoParcela.TIT_CODIGO IS NOT NULL THEN ' / ' + 
                                        (SELECT CAST(MAX(CFP.CFP_SEQUENCIA) AS NVARCHAR(20)) FROM T_CONTRATO_FINANCIAMENTO_PARCELA CFP 
                                        JOIN T_CONTRATO_FINANCIAMENTO CF ON CF.CFI_CODIGO = CFP.CFI_CODIGO
                                        WHERE CF.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO) 
                                    ELSE ''
                                END Parcela,
                                CASE WHEN Titulo.TIT_DATA_CANCELAMENTO <= '" + filtrosPesquisa.DataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 0.00 WHEN Titulo.TIT_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN Titulo.TIT_VALOR_ORIGINAL ELSE Titulo.TIT_VALOR_PENDENTE END ValorPendente,
                                Titulo.TIT_VALOR_ORIGINAL ValorTitulo,
                                CASE WHEN Titulo.TIT_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 0.00 ELSE Titulo.TIT_ACRESCIMO END ValorAcrescimo,
                                CASE WHEN Titulo.TIT_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 0.00 ELSE Titulo.TIT_DESCONTO END ValorDesconto,
                                CASE WHEN Titulo.TIT_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 0.00 ELSE (CASE WHEN ISNULL(Titulo.TIT_VALOR_PENDENTE, 0) > 0 THEN(ISNULL(Titulo.TIT_VALOR_PENDENTE, 0) + ISNULL(Titulo.TIT_ACRESCIMO, 0) - ISNULL(Titulo.TIT_DESCONTO, 0)) ELSE 0 END) END ValorSaldo,
                                CASE WHEN Titulo.TIT_DATA_LIQUIDACAO >= '" + filtrosPesquisa.DataPosicao.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 0.00 ELSE Titulo.TIT_VALOR_PAGO END ValorPago
                                FROM T_TITULO Titulo
                                JOIN T_CLIENTE Fornecedor ON Fornecedor.CLI_CGCCPF = Titulo.CLI_CGCCPF
                                LEFT OUTER JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Titulo.EMP_CODIGO
                                LEFT OUTER JOIN T_CONTRATO_FINANCIAMENTO_PARCELA ContratoFinanciamentoParcela on ContratoFinanciamentoParcela.TIT_CODIGO = Titulo.TIT_CODIGO
                                LEFT OUTER JOIN T_CONTRATO_FINANCIAMENTO ContratoFinanciamento on ContratoFinanciamento.CFI_CODIGO = ContratoFinanciamentoParcela.CFI_CODIGO
                                left outer join T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupada on Titulo.TIT_CODIGO = TituloBaixaAgrupada.TIT_CODIGO
                                left outer join T_TITULO_BAIXA TituloBaixa on TituloBaixaAgrupada.TIB_CODIGO = TituloBaixa.TIB_CODIGO
                                left join T_CATEGORIA_PESSOA CategoriaPessoa on CategoriaPessoa.CTP_CODIGO = Fornecedor.CTP_CODIGO
                                WHERE Titulo.TIT_TIPO = 2 ";

            //if (filtrosPesquisa.DataPosicao >= DateTime.MinValue)            
            //    query += " and Titulo.TIT_DATA_EMISSAO <= '" + filtrosPesquisa.DataPosicao.ToString("yyyy-MM-dd") + "' ";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += " and Titulo.TIT_DATA_EMISSAO >= '" + filtrosPesquisa.DataInicial.ToString("yyyy-MM-dd") + "' ";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += " and Titulo.TIT_DATA_EMISSAO < '" + filtrosPesquisa.DataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' ";

            if (filtrosPesquisa.DataPosicao != DateTime.MinValue)
                query += " and Titulo.TIT_DATA_EMISSAO <= '" + filtrosPesquisa.DataPosicao.AddDays(1).ToString("yyyy-MM-dd") + "' ";

            if (filtrosPesquisa.Situacao == StatusTitulo.Quitada)
                query += " and Titulo.TIT_STATUS = 3";
            if (filtrosPesquisa.Situacao == StatusTitulo.EmAberto)
                query += " and Titulo.TIT_STATUS = 1";
            else
                query += " and not (Titulo.TIT_STATUS = 3 and Titulo.TIT_VALOR_PAGO = 0) ";

            query += @" GROUP BY Titulo.TIT_CODIGO, Fornecedor.CLI_FISJUR, Empresa.EMP_RAZAO, Fornecedor.CLI_CGCCPF, Fornecedor.CLI_NOME,Titulo.TIT_TIPO, Titulo.TIT_DATA_EMISSAO, Titulo.TIT_DATA_VENCIMENTO,
                    Titulo.TIT_DATA_LIQUIDACAO, Titulo.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL, Titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL, Titulo.TBN_CODIGO, Titulo.TIT_SEQUENCIA, Titulo.TDD_CODIGO,
                    Titulo.CFT_CODIGO, Titulo.NFI_CODIGO, Titulo.FAP_CODIGO, Titulo.CMA_CODIGO, ContratoFinanciamentoParcela.TIT_CODIGO, ContratoFinanciamento.CFI_CODIGO,
                    Titulo.TIT_VALOR_PENDENTE, Titulo.TIT_VALOR_ORIGINAL, Titulo.TIT_ACRESCIMO, Titulo.TIT_DESCONTO, Titulo.TIT_VALOR_PAGO, Titulo.TIT_DATA_CANCELAMENTO , CategoriaPessoa.CTP_DESCRICAO, Fornecedor.CLI_CONTA_FORNECEDOR_EBS ";


            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
                query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasPagar)));

            return nhQuery.SetTimeout(60000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasPagar>();
        }

        public int ContarRelatorioPosicaoContasPagar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPosicaoContasPagar filtrosPesquisa)
        {
            string query = @" SELECT COUNT(0) as CONTADOR FROM T_TITULO Titulo
                            JOIN T_CLIENTE Fornecedor ON Fornecedor.CLI_CGCCPF = Titulo.CLI_CGCCPF
                            LEFT OUTER JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Titulo.EMP_CODIGO
                            LEFT OUTER JOIN T_CONTRATO_FINANCIAMENTO_PARCELA ContratoFinanciamentoParcela on ContratoFinanciamentoParcela.TIT_CODIGO = Titulo.TIT_CODIGO
                            LEFT OUTER JOIN T_CONTRATO_FINANCIAMENTO ContratoFinanciamento on ContratoFinanciamento.CFI_CODIGO = ContratoFinanciamentoParcela.CFI_CODIGO
                            left outer join T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupada on Titulo.TIT_CODIGO = TituloBaixaAgrupada.TIT_CODIGO
                            left outer join T_TITULO_BAIXA TituloBaixa on TituloBaixaAgrupada.TIB_CODIGO = TituloBaixa.TIB_CODIGO
                            WHERE Titulo.TIT_TIPO = 2 ";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += " and Titulo.TIT_DATA_EMISSAO >= '" + filtrosPesquisa.DataInicial.ToString("yyyy-MM-dd") + "' ";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += " and Titulo.TIT_DATA_EMISSAO < '" + filtrosPesquisa.DataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' ";

            if (filtrosPesquisa.Situacao == StatusTitulo.Quitada)
                query += " and Titulo.TIT_STATUS = 3";
            if (filtrosPesquisa.Situacao == StatusTitulo.EmAberto)
                query += " and Titulo.TIT_STATUS = 1";
            else
                query += " and not (Titulo.TIT_STATUS = 3 and Titulo.TIT_VALOR_PAGO = 0) ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(60000).UniqueResult<int>();
        }

        #endregion Relatórios

        #region Relatório de Títulos Sem Movimentação

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloSemMovimento> ConsultarRelatorioTitulosSemMovimentos(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo? tipo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo> status, double cnpjPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, Dominio.Enumeradores.TipoAmbiente? tipoAmbiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioTitulosSemMovimentos(false, propriedades, codigoEmpresa, tipo, status, cnpjPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, tipoAmbiente, tipoServicoMultisoftware, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloSemMovimento)));

            return query.SetTimeout(30000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloSemMovimento>();
        }

        public int ContarConsultaRelatorioTitulosSemMovimentos(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo? tipo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo> status, double cnpjPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, Dominio.Enumeradores.TipoAmbiente? tipoAmbiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioTitulosSemMovimentos(true, propriedades, codigoEmpresa, tipo, status, cnpjPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, tipoAmbiente, tipoServicoMultisoftware, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            return query.SetTimeout(30000).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioTitulosSemMovimentos(bool count, List<PropriedadeAgrupamento> propriedades, int codigoEmpresa, TipoTitulo? tipo, List<StatusTitulo> status, double cnpjPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, TipoAmbiente? tipoAmbiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                  groupBy = string.Empty,
                  joins = string.Empty,
                  where = string.Empty,
                  orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioTitulos(propriedades[i].Propriedade, ref select, ref groupBy, ref joins);

            SetarWhereRelatorioTitulosSemMovimentos(ref where, ref joins, codigoEmpresa, tipo, status, cnpjPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, tipoAmbiente, tipoServicoMultisoftware);

            if (!string.IsNullOrWhiteSpace(propAgrupa))
            {
                SetarSelectRelatorioTitulos(propAgrupa, ref select, ref groupBy, ref joins);

                orderBy = " order by " + propAgrupa + " " + dirAgrupa;
            }

            if (!string.IsNullOrWhiteSpace(propOrdena))
            {
                if (propOrdena != propAgrupa && select.Contains(" " + propOrdena + ","))
                    orderBy += (orderBy.Length <= 0 ? " order by " : ", ") + propOrdena + " " + dirOrdena;
            }

            return (count ? "select distinct(count(0) over ())" : "select " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_TITULO Titulo " + joins +
                   " where 1=1 " + where +
                   " group by " + (groupBy.Length > 0 ? groupBy.Substring(0, groupBy.Length - 2) : string.Empty) + (count ? string.Empty : (orderBy.Length > 0 ? orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");
        }

        private void SetarWhereRelatorioTitulosSemMovimentos(ref string where, ref string joins, int codigoEmpresa, TipoTitulo? tipo, List<StatusTitulo> status, double cnpjPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, TipoAmbiente? tipoAmbiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            where += " and Titulo.TIM_CODIGO IS NULL and Titulo.TBN_CODIGO IS NULL";
            if (codigoEmpresa > 0)
                where += " and (Titulo.EMP_CODIGO = " + codigoEmpresa.ToString() + " OR Titulo.EMP_CODIGO IS NULL)";

            if (tipo.HasValue)
                where += " and Titulo.TIT_TIPO = " + tipo.Value.ToString("D");

            if (status != null && status.Count > 0)
                where += " and Titulo.TIT_STATUS in (" + string.Join(", ", status.Select(o => o.ToString("D"))) + ")";

            if (cnpjPessoa > 0d)
                where += " and Titulo.CLI_CGCCPF = " + cnpjPessoa.ToString("g", cultura);

            if (dataInicialEmissao != DateTime.MinValue)
                where += " and Titulo.TIT_DATA_EMISSAO >= '" + dataInicialEmissao.ToString("yyyy-MM-dd") + "' ";

            if (dataFinalEmissao != DateTime.MinValue)
                where += " and Titulo.TIT_DATA_EMISSAO < '" + dataFinalEmissao.AddDays(1).ToString("yyyy-MM-dd") + "' ";

            if (dataInicialVencimento != DateTime.MinValue)
                where += " and Titulo.TIT_DATA_VENCIMENTO >= '" + dataInicialVencimento.ToString("yyyy-MM-dd") + "' ";

            if (dataFinalVencimento != DateTime.MinValue)
                where += " and Titulo.TIT_DATA_VENCIMENTO < '" + dataFinalVencimento.AddDays(1).ToString("yyyy-MM-dd") + "' ";

            if (tipoAmbiente.HasValue)
                where += " and Titulo.TIT_AMBIENTE = " + tipoAmbiente.Value.ToString("D");
        }

        #endregion Relatório de Títulos Sem Movimentação

        #region Relatório de Títulos

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.Titulo> ConsultarRelatorioTitulos(List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioTitulos(false, propriedades, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.Titulo)));

            return query.SetTimeout(30000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.Titulo>();
        }

        public int ContarConsultaRelatorioTitulos(List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioTitulos(true, propriedades, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(30000).UniqueResult<int>();
        }

        private SQLDinamico ObterSelectConsultaRelatorioTitulos(bool count, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                  groupBy = string.Empty,
                  joins = string.Empty,
                  where = string.Empty,
                  orderBy = string.Empty,
                  having = string.Empty;

            var parametros = new List<ParametroSQL>();

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioTitulos(propriedades[i].Propriedade, ref select, ref groupBy, ref joins);

            SetarWhereRelatorioTitulos(ref where, ref having, ref joins, ref parametros, filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(propAgrupa))
            {
                SetarSelectRelatorioTitulos(propAgrupa, ref select, ref groupBy, ref joins);

                orderBy = " order by " + propAgrupa + " " + dirAgrupa;
            }

            if (!string.IsNullOrWhiteSpace(propOrdena))
            {
                if (propOrdena != propAgrupa && select.Contains(" " + propOrdena + ","))
                    orderBy += (orderBy.Length <= 0 ? " order by " : ", ") + propOrdena + " " + dirOrdena;
            }

            return new SQLDinamico((count ? "select distinct(count(0) over ())" : "select " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_TITULO Titulo " + joins +
                   " where 1=1 " + where +
                   " group by " + (groupBy.Length > 0 ? groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (having.Length > 0 ? " having " + having : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;"), parametros);
        }

        private void SetarWhereRelatorioTitulos(ref string where, ref string having, ref string joins, ref List<ParametroSQL> parametros, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo filtrosPesquisa)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.NovoModeloFatura.HasValue)
                where += " and exists (select fatura.FAT_CODIGO from T_FATURA fatura inner join T_FATURA_PARCELA parcela on parcela.FAT_CODIGO = fatura.FAT_CODIGO where parcela.FAP_CODIGO = Titulo.FAP_CODIGO and (fatura.FAT_NOVO_MODELO = " + (filtrosPesquisa.NovoModeloFatura.Value ? "1" : "0 or fatura.FAT_NOVO_MODELO is null") + "))";

            if (filtrosPesquisa.CodigosTipoMovimento.Count > 0)
                where += $" and Titulo.TIM_CODIGO in({string.Join(", ", filtrosPesquisa.CodigosTipoMovimento)})";

            if (filtrosPesquisa.ValorInicial > 0m)
                where += " and Titulo.TIT_VALOR_ORIGINAL >= " + filtrosPesquisa.ValorInicial.ToString("g", cultura);

            if (filtrosPesquisa.ValorFinal > 0m)
                where += " and Titulo.TIT_VALOR_ORIGINAL <= " + filtrosPesquisa.ValorFinal.ToString("g", cultura);

            if (filtrosPesquisa.Adiantado == 0)
                where += " and (Titulo.TIT_ADIANTADO = 0 OR Titulo.TIT_ADIANTADO IS NULL)";
            else if (filtrosPesquisa.Adiantado == 1)
                where += " and Titulo.TIT_ADIANTADO = 1";

            if (filtrosPesquisa.Autorizados == OpcaoSimNao.Sim)
                where += " and Titulo.TIT_DATA_AUTORIZACAO IS NOT NULL";
            else if (filtrosPesquisa.Autorizados == OpcaoSimNao.Nao)
                where += " and Titulo.TIT_DATA_AUTORIZACAO IS NULL";

            if (filtrosPesquisa.TipoBoleto != TipoBoletoPesquisaTitulo.Todos)
            {
                if (filtrosPesquisa.TipoBoleto == TipoBoletoPesquisaTitulo.SemBoleto)
                    where += " and (Titulo.TIT_STATUS_BOLETO = 0 OR Titulo.TIT_STATUS_BOLETO IS NULL)";
                else if (filtrosPesquisa.TipoBoleto == TipoBoletoPesquisaTitulo.ComBoleto)
                    where += " and Titulo.TIT_STATUS_BOLETO >= 1";
                else if (filtrosPesquisa.TipoBoleto == TipoBoletoPesquisaTitulo.ComRemessa)
                    where += " and Titulo.BRE_CODIGO IS NOT NULL";
                else if (filtrosPesquisa.TipoBoleto == TipoBoletoPesquisaTitulo.SemRemessa)
                    where += " and Titulo.TIT_STATUS_BOLETO >= 1 and Titulo.BRE_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CnpjPortador > 0d)
                where += " and Titulo.CLI_CGCCPF_PORTADOR = " + filtrosPesquisa.CnpjPortador.ToString("g", cultura);

            if (filtrosPesquisa.CodigoCategoria > 0)
            {
                where += " and Categoria.CTP_CODIGO = " + filtrosPesquisa.CodigoCategoria;

                if (!joins.Contains(" Tomador "))
                    joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                if (!joins.Contains(" Categoria "))
                    joins += " left join T_CATEGORIA_PESSOA Categoria on Categoria.CTP_CODIGO = Tomador.CTP_CODIGO";
            }

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                if (filtrosPesquisa.CodigosEmpresa.Count > 0)
                    where += " and (Titulo.EMP_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosEmpresa) + ") OR Titulo.EMP_CODIGO IS NULL)";
            }
            else
            {
                if (filtrosPesquisa.CodigosEmpresa.Count > 0)
                {
                    if (!joins.Contains(" FaturaParcela "))
                        joins += " left join T_FATURA_PARCELA FaturaParcela on Titulo.FAP_CODIGO = FaturaParcela.FAP_CODIGO";

                    if (!joins.Contains(" Fatura "))
                        joins += " left join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaParcela.FAT_CODIGO";

                    where += " and (Fatura.EMP_TRANSPORTADOR in (" + string.Join(", ", filtrosPesquisa.CodigosEmpresa) + ") or (Titulo.EMP_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosEmpresa) + ") OR Titulo.EMP_CODIGO IS NULL))";
                }
            }

            if (filtrosPesquisa.Tipo.HasValue && filtrosPesquisa.Tipo.Value != TipoTitulo.Todos)
                where += " and Titulo.TIT_TIPO = " + filtrosPesquisa.Tipo.Value.ToString("D");

            if (filtrosPesquisa.Status != null && filtrosPesquisa.Status.Count > 0)
                where += " and Titulo.TIT_STATUS in (" + string.Join(", ", filtrosPesquisa.Status.Select(o => o.ToString("D"))) + ")";

            if (filtrosPesquisa.CodigoTitulo > 0)
                where += " and Titulo.TIT_CODIGO = " + filtrosPesquisa.CodigoTitulo.ToString();

            if (filtrosPesquisa.CodigoDocumentoEntrada > 0)
                where += " and exists (select duplicata.TDD_CODIGO from T_TMS_DOCUMENTO_ENTRADA_DUPLICATA duplicata where duplicata.TDD_CODIGO = Titulo.TDD_CODIGO and duplicata.TDE_CODIGO = " + filtrosPesquisa.CodigoDocumentoEntrada.ToString() + ")"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoFatura > 0)
                where += " and exists (select parcela.FAP_CODIGO from T_FATURA_PARCELA parcela where parcela.FAP_CODIGO = Titulo.FAP_CODIGO and parcela.FAT_CODIGO = " + filtrosPesquisa.CodigoFatura.ToString() + ")"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.GruposPessoas != null && filtrosPesquisa.GruposPessoas.Count > 0)
            {
                where += " and (Titulo.GRP_CODIGO in (" + string.Join(", ", filtrosPesquisa.GruposPessoas) + ") OR Tomador.GRP_CODIGO in (" + string.Join(", ", filtrosPesquisa.GruposPessoas) + "))";
                if (!joins.Contains(" Tomador "))
                    joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DocumentoOriginal))
                where += " and Titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL LIKE '%" + filtrosPesquisa.DocumentoOriginal + "%'";

            if (filtrosPesquisa.CodigoCTe > 0)
                where += " and exists (select documento.TDO_CODIGO from T_TITULO_DOCUMENTO documento where documento.TIT_CODIGO = Titulo.TIT_CODIGO and documento.CON_CODIGO = " + filtrosPesquisa.CodigoCTe.ToString() + ")"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CnpjPessoas?.Count > 0d)
                where += " and Titulo.CLI_CGCCPF in (" + string.Join(", ", filtrosPesquisa.CnpjPessoas) + ")";

            if (filtrosPesquisa.DataBaseInicial != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_BASE_LIQUIDACAO AS DATE) >= '" + filtrosPesquisa.DataBaseInicial.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataBaseFinal != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_BASE_LIQUIDACAO AS DATE) <= '" + filtrosPesquisa.DataBaseFinal.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicialEmissao.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinalEmissao.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataInicialVencimento != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataInicialVencimento.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalVencimento != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataFinalVencimento.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataInicialQuitacao != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_LIQUIDACAO AS DATE) >= '" + filtrosPesquisa.DataInicialQuitacao.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalQuitacao != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_LIQUIDACAO AS DATE) <= '" + filtrosPesquisa.DataFinalQuitacao.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataInicialCancelamento != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_CANCELAMENTO AS DATE) >= '" + filtrosPesquisa.DataInicialCancelamento.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalCancelamento != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_CANCELAMENTO AS DATE) <= '" + filtrosPesquisa.DataFinalCancelamento.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataInicialEmissaoDocumentoEntrada != DateTime.MinValue || filtrosPesquisa.DataFinalEmissaoDocumentoEntrada != DateTime.MinValue)
            {
                where += " and exists (select documentoEntrada.TDE_CODIGO from T_TMS_DOCUMENTO_ENTRADA documentoEntrada inner join T_TMS_DOCUMENTO_ENTRADA_DUPLICATA duplicata on documentoEntrada.TDE_CODIGO = duplicata.TDE_CODIGO where duplicata.TDD_CODIGO = Titulo.TDD_CODIGO";

                if (filtrosPesquisa.DataInicialEmissaoDocumentoEntrada != DateTime.MinValue)
                    where += " and CAST(documentoEntrada.TDE_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicialEmissaoDocumentoEntrada.ToString(pattern) + "' ";

                if (filtrosPesquisa.DataFinalEmissaoDocumentoEntrada != DateTime.MinValue)
                    where += " and CAST(documentoEntrada.TDE_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinalEmissaoDocumentoEntrada.ToString(pattern) + "' ";

                where += ")";
            }

            if (filtrosPesquisa.CodigoBordero > 0)
                where += " and Titulo.BOR_CODIGO = " + filtrosPesquisa.CodigoBordero.ToString();

            if (filtrosPesquisa.Moeda != MoedaCotacaoBancoCentral.Todas)
                where += " and Titulo.TIT_MOEDA_COTACAO_BANCO_CENTRAL = " + (int)filtrosPesquisa.Moeda;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente)) 
            {
                where += @" and exists (select TituloDocumento.TDO_CODIGO from T_TITULO_DOCUMENTO TituloDocumento
                                        inner join T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on (TituloDocumento.CON_CODIGO is not null and TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) or (TituloDocumento.CAR_CODIGO is not null and TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)
                                        inner join T_DOCUMENTO_FATURAMENTO_NUMERO_PEDIDO DocumentoFaturamentoPedido on DocumentoFaturamentoPedido.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO
                                        where TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO and DocumentoFaturamentoPedido.DFA_NUMERO_PEDIDO like :DOCUMENTOFATURAMENTOPEDIDO_DFA_NUMERO_PEDIDO) ";
                parametros.Add(new ParametroSQL("DOCUMENTOFATURAMENTOPEDIDO_DFA_NUMERO_PEDIDO", $"%{filtrosPesquisa.NumeroPedidoCliente}%"));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOcorrenciaCliente))
            {
                where += @" and exists (select TituloDocumento.TDO_CODIGO from T_TITULO_DOCUMENTO TituloDocumento
                                        inner join T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on (TituloDocumento.CON_CODIGO is not null and TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) or (TituloDocumento.CAR_CODIGO is not null and TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)
                                        inner join T_DOCUMENTO_FATURAMENTO_NUMERO_OCORRENCIA DocumentoFaturamentoOcorrencia on DocumentoFaturamentoOcorrencia.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO
                                        where TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO and DocumentoFaturamentoOcorrencia.DFA_NUMERO_PEDIDO_OCORRENCIA like :DOCUMENTOFATURAMENTOOCORRENCIA_DFA_NUMERO_PEDIDO_OCORRENCIA) ";
                parametros.Add(new ParametroSQL("DOCUMENTOFATURAMENTOOCORRENCIA_DFA_NUMERO_PEDIDO_OCORRENCIA", $"%{filtrosPesquisa.NumeroOcorrenciaCliente}%"));

            }

            if (filtrosPesquisa.NumeroOcorrencia > 0)
            {
                where += @" and exists (select TituloDocumento.TDO_CODIGO from T_TITULO_DOCUMENTO TituloDocumento
                                        inner join T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on (TituloDocumento.CON_CODIGO is not null and TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) or (TituloDocumento.CAR_CODIGO is not null and TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)
                                        where TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO and DocumentoFaturamento.DFA_NUMERO_OCORRENCIA = " + filtrosPesquisa.NumeroOcorrencia.ToString() + ") ";
            }

            if (filtrosPesquisa.NumeroDocumentoOriginario > 0)
            {
                where += @" and exists (select TituloDocumento.TDO_CODIGO from T_TITULO_DOCUMENTO TituloDocumento
                                        inner join T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on (TituloDocumento.CON_CODIGO is not null and TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) or (TituloDocumento.CAR_CODIGO is not null and TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)
                                        where TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO and DocumentoFaturamento.DFA_NUMERO_DOCUMENTO_ORIGINARIO = " + filtrosPesquisa.NumeroDocumentoOriginario.ToString() + ") ";
            }

            if (filtrosPesquisa.TipoAmbiente != TipoAmbiente.Nenhum)
                where += " and Titulo.TIT_AMBIENTE = " + filtrosPesquisa.TipoAmbiente.ToString("D");

            if (filtrosPesquisa.TipoDocumento.HasValue)
            {
                if (filtrosPesquisa.TipoDocumento.Value == TipoDocumentoPesquisaTitulo.ContratoFrete)
                    where += " and Titulo.CFT_CODIGO IS NOT NULL";
                else if (filtrosPesquisa.TipoDocumento.Value == TipoDocumentoPesquisaTitulo.DocumentoEntrada)
                    where += " and Titulo.TDD_CODIGO IS NOT NULL";
                else if (filtrosPesquisa.TipoDocumento.Value == TipoDocumentoPesquisaTitulo.Fatura)
                    where += " and (Titulo.FAP_CODIGO IS NOT NULL OR Titulo.FCD_CODIGO IS NOT NULL)";
                else if (filtrosPesquisa.TipoDocumento.Value == TipoDocumentoPesquisaTitulo.Negociacao)
                    where += " and Titulo.TBN_CODIGO IS NOT NULL";
                else if (filtrosPesquisa.TipoDocumento.Value == TipoDocumentoPesquisaTitulo.NotaFiscal)
                    where += " and Titulo.NFI_CODIGO IS NOT NULL";
                else if (filtrosPesquisa.TipoDocumento.Value == TipoDocumentoPesquisaTitulo.ContratoFinanciamento)
                    where += " and exists (select contratoFinanciamentoParcela.TIT_CODIGO from T_CONTRATO_FINANCIAMENTO_PARCELA contratoFinanciamentoParcela where contratoFinanciamentoParcela.TIT_CODIGO = Titulo.TIT_CODIGO)";
                else if (filtrosPesquisa.TipoDocumento.Value == TipoDocumentoPesquisaTitulo.Outros)
                    where += " and Titulo.NFI_CODIGO IS NULL and Titulo.NFI_CODIGO IS NULL and Titulo.TBN_CODIGO IS NULL and Titulo.FAP_CODIGO IS NULL and Titulo.FCD_CODIGO IS NULL and Titulo.TDD_CODIGO IS NULL and Titulo.CFT_CODIGO IS NULL";
            }

            if (filtrosPesquisa.FormaTitulo.Count > 0)
                where += $" and Titulo.TIT_FORMA in ({string.Join(", ", filtrosPesquisa.FormaTitulo.Select(o => (int)o))})";

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                where += new System.Text.StringBuilder()
                            .Append(" AND EXISTS ( ")
                            .Append("         SELECT 1 ")
                            .Append("           FROM T_TITULO_DOCUMENTO ")
                            .Append("           LEFT JOIN T_CARGA_CTE ")
                            .Append("             ON T_TITULO_DOCUMENTO.CON_CODIGO = T_CARGA_CTE.CON_CODIGO ")
                            .Append("           JOIN T_CARGA ")
                            .Append("             ON ( ")
                            .Append("                    (T_CARGA_CTE.CAR_CODIGO IS NOT NULL AND T_CARGA_CTE.CAR_CODIGO = T_CARGA.CAR_CODIGO) OR ")
                            .Append("                    (T_TITULO_DOCUMENTO.CAR_CODIGO IS NOT NULL AND T_TITULO_DOCUMENTO.CAR_CODIGO = T_CARGA.CAR_CODIGO) ")
                            .Append("                ) ")
                            .Append($"         WHERE T_TITULO_DOCUMENTO.TIT_CODIGO = Titulo.TIT_CODIGO ")
                            .Append($"           AND T_CARGA.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao} ")
                            .Append("     ) ")
                            .ToString();
            }

            if (filtrosPesquisa.ProvisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SemProvisao)
                where += " and (Titulo.TIT_PROVISAO = 0 OR Titulo.TIT_PROVISAO IS NULL)";
            else if (filtrosPesquisa.ProvisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SomenteProvisao)
                where += " and Titulo.TIT_PROVISAO = 1";

            if (filtrosPesquisa.TipoProposta.Count > 0)
                where += $@" and exists (select TituloDocumento.TIT_CODIGO from T_TITULO_DOCUMENTO TituloDocumento
                                        left join T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = TituloDocumento.CON_CODIGO
                                        left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO
                                        where TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO
                                        and CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoProposta.Select(o => o.GetHashCode()))})) ";

            if (filtrosPesquisa.CodigoTipoPagamentoRecebimento > 0)
            {
                where += @" and (Titulo.TIT_CODIGO in (SELECT D.TIT_CODIGO 
                            FROM T_TITULO_BAIXA_AGRUPADO D
                            JOIN T_TITULO_BAIXA B ON B.TIB_CODIGO = D.TIB_CODIGO
                            JOIN T_TIPO_PAGAMENTO_RECEBIMENTO P ON P.TPR_CODIGO = B.TPR_CODIGO
                            WHERE B.TIB_SITUACAO = 3 AND B.TIB_VALOR > 0 AND P.TPR_CODIGO = " + filtrosPesquisa.CodigoTipoPagamentoRecebimento + @")
                            OR Titulo.TIT_CODIGO in (SELECT D.TIT_CODIGO 
                            FROM T_TITULO_BAIXA_AGRUPADO D
                            JOIN T_TITULO_BAIXA B ON B.TIB_CODIGO = D.TIB_CODIGO
                            JOIN T_TITULO_BAIXA_TIPO_PAGAMENTO_RECEBIMENTO RR ON RR.TIB_CODIGO = B.TIB_CODIGO
                            JOIN T_TIPO_PAGAMENTO_RECEBIMENTO P ON P.TPR_CODIGO = RR.TPR_CODIGO
                            WHERE B.TIB_SITUACAO = 3 AND B.TIB_VALOR > 0 AND P.TPR_CODIGO = " + filtrosPesquisa.CodigoTipoPagamentoRecebimento + @"))";
            }

            if (filtrosPesquisa.CodigoPagamentoEletronico > 0)
            {
                where += @" and Titulo.TIT_CODIGO in (SELECT TT.TIT_CODIGO
	                            FROM T_PAGAMENTO_ELETRONICO_TITULO TT
                                JOIN T_PAGAMENTO_ELETRONICO P ON P.PAE_CODIGO = TT.PAE_CODIGO
                                WHERE P.PAE_CODIGO = " + filtrosPesquisa.CodigoPagamentoEletronico + ")";
            }

            if (filtrosPesquisa.CodigoRemessa > 0)
                where += " and Titulo.BRE_CODIGO = " + filtrosPesquisa.CodigoRemessa.ToString();

            if (filtrosPesquisa.CodigoCheque > 0)
            {
                where += @" and Titulo.TIT_CODIGO in (SELECT baixaAgrupada.TIT_CODIGO
	                            FROM T_TITULO_BAIXA_AGRUPADO baixaAgrupada
                                JOIN T_TITULO_BAIXA baixa ON baixa.TIB_CODIGO = baixaAgrupada.TIB_CODIGO
                                JOIN T_TITULO_BAIXA_CHEQUE baixaCheque ON baixaCheque.TIB_CODIGO = baixa.TIB_CODIGO
                                WHERE baixaCheque.CHQ_CODIGO = " + filtrosPesquisa.CodigoCheque + ")";
            }

            if (filtrosPesquisa.DataAutorizacaoInicial != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_AUTORIZACAO AS DATE) >= '" + filtrosPesquisa.DataAutorizacaoInicial.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataAutorizacaoFinal != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_AUTORIZACAO AS DATE) <= '" + filtrosPesquisa.DataAutorizacaoFinal.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataProgramacaoPagamentoInicial != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_PROGRAMACAO_PAGAMENTO AS DATE) >= '" + filtrosPesquisa.DataProgramacaoPagamentoInicial.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataProgramacaoPagamentoFinal != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_PROGRAMACAO_PAGAMENTO AS DATE) <= '" + filtrosPesquisa.DataProgramacaoPagamentoFinal.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataInicialEntradaDocumentoEntrada != DateTime.MinValue || filtrosPesquisa.DataFinalEntradaDocumentoEntrada != DateTime.MinValue)
            {
                where += " and exists (select documentoEntrada.TDE_CODIGO from T_TMS_DOCUMENTO_ENTRADA documentoEntrada inner join T_TMS_DOCUMENTO_ENTRADA_DUPLICATA duplicata on documentoEntrada.TDE_CODIGO = duplicata.TDE_CODIGO where duplicata.TDD_CODIGO = Titulo.TDD_CODIGO";

                if (filtrosPesquisa.DataInicialEntradaDocumentoEntrada != DateTime.MinValue)
                    where += " and CAST(documentoEntrada.TDE_DATA_ENTRADA AS DATE) >= '" + filtrosPesquisa.DataInicialEntradaDocumentoEntrada.ToString(pattern) + "' ";

                if (filtrosPesquisa.DataFinalEntradaDocumentoEntrada != DateTime.MinValue)
                    where += " and CAST(documentoEntrada.TDE_DATA_ENTRADA AS DATE) <= '" + filtrosPesquisa.DataFinalEntradaDocumentoEntrada.ToString(pattern) + "' ";

                where += ")";
            }

            if (filtrosPesquisa.ModelosDocumento.Count > 0)
            {
                where += "AND EXISTS (SELECT 1 FROM T_TITULO_DOCUMENTO TituloDocumento " +
                            "JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND " +
                            "TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO " +
                            "JOIN T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = DocumentoFaturamento.MOD_CODIGO " +
                            "WHERE TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO " +
                            "AND ModeloDocumentoFiscal.MOD_CODIGO in (" + string.Join(", ", filtrosPesquisa.ModelosDocumento) + "))";
            }

            if (filtrosPesquisa.CodigoPagamentoMotoristaTipo > 0)
            {
                where += $" and PagamentoMotorista.PMT_CODIGO = {filtrosPesquisa.CodigoPagamentoMotoristaTipo}";
                if (!joins.Contains(" PagamentoMotorista "))
                    joins += " left join T_PAGAMENTO_MOTORISTA_TMS PagamentoMotorista on PagamentoMotorista.TIT_CODIGO = Titulo.TIT_CODIGO";
            }

            if (filtrosPesquisa.DataInicialLancamento != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_LANCAMENTO AS DATE) >= '" + filtrosPesquisa.DataInicialLancamento.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinalLancamento != DateTime.MinValue)
                where += " and CAST(Titulo.TIT_DATA_LANCAMENTO AS DATE) <= '" + filtrosPesquisa.DataFinalLancamento.ToString(pattern) + "' ";

            if (!filtrosPesquisa.VisualizarTitulosPagamentoSalario)
                where += $" AND Titulo.TIT_FORMA <> {FormaTitulo.PagamentoSalario.ToString("d")}";

            if (filtrosPesquisa.CodigoComandoBanco > 0)
                where += $" AND exists (SELECT Retorno.TIT_CODIGO FROM T_BOLETO_RETORNO Retorno WHERE Retorno.TIT_CODIGO = titulo.TIT_CODIGO AND Retorno.BRC_CODIGO = {filtrosPesquisa.CodigoComandoBanco})"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where += $" AND exists (SELECT tituloVeiculo.TIT_CODIGO FROM T_TITULO_VEICULO tituloVeiculo WHERE tituloVeiculo.TIT_CODIGO = Titulo.TIT_CODIGO and tituloVeiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo})"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Renegociado != TituloRenegociado.Todos)
            {
                if (filtrosPesquisa.Renegociado == TituloRenegociado.Sim)
                {
                    where += $" AND Titulo.TIT_STATUS IN (1, 3, 4) AND Titulo.TIT_STATUS = 3";
                    having += $" SUM(Titulo.TIT_VALOR_PAGO) = 0 OR SUM(Titulo.TIT_VALOR_PAGO) < (SUM(Titulo.TIT_VALOR_ORIGINAL) - SUM(Titulo.TIT_DESCONTO) + SUM(Titulo.TIT_ACRESCIMO))";
                }
                else
                {
                    where += $" AND Titulo.TIT_STATUS IN (1, 3, 4)";
                    having += $" NOT (Titulo.TIT_STATUS = 3 AND (SUM(Titulo.TIT_VALOR_PAGO) = 0 OR SUM(Titulo.TIT_VALOR_PAGO) < (SUM(Titulo.TIT_VALOR_ORIGINAL) - SUM(Titulo.TIT_DESCONTO) + SUM(Titulo.TIT_ACRESCIMO))))";
                }
            }

            if (filtrosPesquisa.TiposContato.Count > 0)
            {
                where += "AND EXISTS (SELECT 1 FROM " +
                    "T_CONTATO_CLIENTE_DOCUMENTO ContatoClienteDocumento " +
                    "LEFT JOIN T_CONTATO_CLIENTE_TIPO_CONTATO ContatoClienteTipoContato ON ContatoClienteTipoContato.CCL_CODIGO = ContatoClienteDocumento.CCL_CODIGO " +
                    "WHERE ContatoClienteDocumento.TIT_CODIGO = Titulo.TIT_CODIGO AND ContatoClienteTipoContato.TCO_CODIGO IN(" + string.Join(",", filtrosPesquisa.TiposContato) + "))";
            }

            if (filtrosPesquisa.SituacoesContato.Count > 0)
            {
                where += "AND EXISTS (SELECT 1 FROM  " +
                    "T_CONTATO_CLIENTE_DOCUMENTO ContatoClienteDocumento " +
                    "LEFT JOIN T_CONTATO_CLIENTE ContatoCliente ON ContatoCliente.CCL_CODIGO = ContatoClienteDocumento.CCL_CODIGO " +
                    "WHERE ContatoClienteDocumento.TIT_CODIGO = Titulo.TIT_CODIGO AND ContatoCliente.SCO_CODIGO IN(" + string.Join(",", filtrosPesquisa.SituacoesContato) + "))";
            }
            
        }

        private void SetarSelectRelatorioTitulos(string propriedade, ref string select, ref string groupBy, ref string joins)
        {
            switch (propriedade)
            {
                case "Usuario":
                    if (!select.Contains(" Usuario,"))
                    {
                        select += "Usuario.FUN_NOME Usuario, ";

                        if (!groupBy.Contains("Usuario.FUN_NOME"))
                            groupBy += "Usuario.FUN_NOME, ";

                        if (!joins.Contains(" Usuario "))
                            joins += " left join T_FUNCIONARIO Usuario on Titulo.FUN_CODIGO = Usuario.FUN_CODIGO";
                    }
                    break;
                case "DataLancamento":
                    if (!select.Contains(" DataLancamento,"))
                    {
                        select += "Titulo.TIT_DATA_LANCAMENTO DataLancamento, ";

                        if (!groupBy.Contains("Titulo.TIT_DATA_LANCAMENTO"))
                            groupBy += "Titulo.TIT_DATA_LANCAMENTO, ";
                    }
                    break;
                case "TipoMovimento":
                    if (!select.Contains(" TipoMovimento,"))
                    {
                        select += "ISNULL(TipoMovimento.TIM_DESCRICAO, 'NÃO INFORMADO') TipoMovimento, ";
                        groupBy += "TipoMovimento.TIM_DESCRICAO, ";

                        if (!joins.Contains(" TipoMovimento "))
                            joins += " LEFT OUTER JOIN T_TIPO_MOVIMENTO TipoMovimento ON TipoMovimento.TIM_CODIGO = Titulo.TIM_CODIGO";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA
                                    FROM T_TITULO_VEICULO tituloVeiculo
                                    inner join T_VEICULO veiculo on Veiculo.VEI_CODIGO = tituloVeiculo.VEI_CODIGO
                                    WHERE tituloVeiculo.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000) Veiculo, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";

                    break;
                case "TipoPagamentoRecebimento":
                    if (!select.Contains(" TipoPagamentoRecebimento,"))
                    {
                        select += @"ISNULL((SELECT TOP(1) P.TPR_DESCRICAO
                            FROM T_TITULO_BAIXA_AGRUPADO D
                            JOIN T_TITULO_BAIXA B ON B.TIB_CODIGO = D.TIB_CODIGO
                            JOIN T_TIPO_PAGAMENTO_RECEBIMENTO P ON P.TPR_CODIGO = B.TPR_CODIGO
                            WHERE D.TIT_CODIGO = Titulo.TIT_CODIGO AND B.TIB_VALOR > 0 AND B.TIB_SITUACAO != 4), 
                            SUBSTRING((SELECT DISTINCT ', ' + P.TPR_DESCRICAO
                            FROM T_TITULO_BAIXA_AGRUPADO D
                            JOIN T_TITULO_BAIXA B ON B.TIB_CODIGO = D.TIB_CODIGO
                            JOIN T_TITULO_BAIXA_TIPO_PAGAMENTO_RECEBIMENTO RR ON RR.TIB_CODIGO = B.TIB_CODIGO
                            JOIN T_TIPO_PAGAMENTO_RECEBIMENTO P ON P.TPR_CODIGO = RR.TPR_CODIGO
                            WHERE D.TIT_CODIGO = Titulo.TIT_CODIGO AND B.TIB_VALOR > 0 AND B.TIB_SITUACAO != 4 FOR XML PATH('')), 3, 1000)) TipoPagamentoRecebimento, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "ValorCapitalContratoFinanciamento":
                    if (!select.Contains(" ValorCapitalContratoFinanciamento,"))
                    {
                        select += @"SUM(ContratoFinanciamentoParcela.CFP_VALOR) ValorCapitalContratoFinanciamento, ";

                        if (!joins.Contains(" ContratoFinanciamentoParcela "))
                            joins += " left join T_CONTRATO_FINANCIAMENTO_PARCELA ContratoFinanciamentoParcela on ContratoFinanciamentoParcela.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "ValorAcrescimoContratoFinanciamento":
                    if (!select.Contains(" ValorAcrescimoContratoFinanciamento,"))
                    {
                        select += @"SUM(ContratoFinanciamentoParcela.CFP_VALOR_ACRESCIMO) ValorAcrescimoContratoFinanciamento, ";

                        if (!joins.Contains(" ContratoFinanciamentoParcela "))
                            joins += " left join T_CONTRATO_FINANCIAMENTO_PARCELA ContratoFinanciamentoParcela on ContratoFinanciamentoParcela.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;

                case "AcrescimoCalculado":
                    if (!select.Contains(" AcrescimoCalculado,"))
                    {
                        select += @"(ISNULL(SUM(ContratoFinanciamentoParcela.CFP_VALOR_ACRESCIMO), 0) + ISNULL(SUM(Titulo.TIT_ACRESCIMO), 0)) AcrescimoCalculado, ";

                        if (!joins.Contains(" ContratoFinanciamentoParcela "))
                            joins += " left join T_CONTRATO_FINANCIAMENTO_PARCELA ContratoFinanciamentoParcela on ContratoFinanciamentoParcela.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select += "(CASE WHEN Titulo.TIT_CODIGO_RECEBIDO_INTEGRACAO IS NULL OR Titulo.TIT_CODIGO_RECEBIDO_INTEGRACAO = 0 THEN Titulo.TIT_CODIGO ELSE Titulo.TIT_CODIGO_RECEBIDO_INTEGRACAO END) Codigo, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                        if (!groupBy.Contains("Titulo.TIT_CODIGO_RECEBIDO_INTEGRACAO"))
                            groupBy += "Titulo.TIT_CODIGO_RECEBIDO_INTEGRACAO, ";
                    }
                    break;
                case "Fatura":
                    if (!select.Contains(" Fatura,"))
                    {
                        select += "(CASE WHEN Fatura.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR Fatura.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN Fatura.FAT_NUMERO ELSE Fatura.FAT_NUMERO_FATURA_INTEGRACAO END) Fatura, ";
                        groupBy += "Fatura.FAT_NUMERO, ";
                        groupBy += "Fatura.FAT_NUMERO_FATURA_INTEGRACAO, ";

                        if (!joins.Contains(" FaturaParcela "))
                            joins += " left join T_FATURA_PARCELA FaturaParcela on Titulo.FAP_CODIGO = FaturaParcela.FAP_CODIGO";

                        if (!joins.Contains(" Fatura "))
                            joins += " left join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaParcela.FAT_CODIGO";
                    }
                    break;

                case "ObservacaoFatura":
                    if (!select.Contains(" ObservacaoFatura,"))
                    {
                        select += "ISNULL(Fatura.FAT_OBSERVACAO, '') + ' ' + ISNULL(Fatura.FAT_OBSERVACAO_FATURA, '') ObservacaoFatura, ";
                        groupBy += "Fatura.FAT_OBSERVACAO, Fatura.FAT_OBSERVACAO_FATURA, ";

                        if (!joins.Contains(" FaturaParcela "))
                            joins += " left join T_FATURA_PARCELA FaturaParcela on Titulo.FAP_CODIGO = FaturaParcela.FAP_CODIGO";

                        if (!joins.Contains(" Fatura "))
                            joins += " left join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaParcela.FAT_CODIGO";
                    }
                    break;

                case "Bordero":
                    if (!select.Contains(" Bordero,"))
                    {
                        select += "Bordero.BOR_NUMERO Bordero, ";
                        groupBy += "Bordero.BOR_NUMERO, ";

                        if (!joins.Contains(" Bordero "))
                            joins += " left join T_BORDERO Bordero on Bordero.BOR_CODIGO = Titulo.BOR_CODIGO";
                    }
                    break;
                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select += "LocalidadeEmpresa.UF_SIGLA Filial, ";
                        groupBy += "LocalidadeEmpresa.UF_SIGLA, ";

                        if (!joins.Contains(" Empresa "))
                            joins += " left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Titulo.EMP_CODIGO";

                        if (!joins.Contains(" LocalidadeEmpresa "))
                            joins += " left join T_LOCALIDADES LocalidadeEmpresa on LocalidadeEmpresa.LOC_CODIGO = Empresa.LOC_CODIGO";
                    }
                    break;
                case "CNPJEmpresaFormatado":
                    if (!select.Contains(" CNPJEmpresa,"))
                    {
                        select += "ISNULL(Empresa.EMP_CNPJ, EmpresaTitulo.EMP_CNPJ) CNPJEmpresa, ";
                        groupBy += "Empresa.EMP_CNPJ, EmpresaTitulo.EMP_CNPJ, ";

                        if (!joins.Contains(" EmpresaTitulo "))
                            joins += " left join T_EMPRESA EmpresaTitulo on EmpresaTitulo.EMP_CODIGO = Titulo.EMP_CODIGO";

                        if (!joins.Contains(" FaturaParcela "))
                            joins += " left join T_FATURA_PARCELA FaturaParcela on Titulo.FAP_CODIGO = FaturaParcela.FAP_CODIGO";

                        if (!joins.Contains(" Fatura "))
                            joins += " left join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaParcela.FAT_CODIGO";

                        if (!joins.Contains(" Empresa "))
                            joins += " left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Fatura.EMP_TRANSPORTADOR";
                    }
                    break;
                case "Empresa":
                    if (!select.Contains(" Empresa,"))
                    {
                        select += "ISNULL(Empresa.EMP_RAZAO, EmpresaTitulo.EMP_RAZAO) Empresa, ";
                        groupBy += "Empresa.EMP_RAZAO, EmpresaTitulo.EMP_RAZAO, ";

                        if (!joins.Contains(" EmpresaTitulo "))
                            joins += " left join T_EMPRESA EmpresaTitulo on EmpresaTitulo.EMP_CODIGO = Titulo.EMP_CODIGO";

                        if (!joins.Contains(" FaturaParcela "))
                            joins += " left join T_FATURA_PARCELA FaturaParcela on Titulo.FAP_CODIGO = FaturaParcela.FAP_CODIGO";

                        if (!joins.Contains(" Fatura "))
                            joins += " left join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaParcela.FAT_CODIGO";

                        if (!joins.Contains(" Empresa "))
                            joins += " left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Fatura.EMP_TRANSPORTADOR";
                    }
                    break;
                case "CPFCNPJPessoaFormatado":
                    if (!select.Contains(" CNPJCPF,"))
                    {
                        select += "Tomador.CLI_CGCCPF CPFCNPJPessoa, Tomador.CLI_CGCCPF CNPJCPF, Tomador.CLI_FISJUR TipoPessoa, ";
                        groupBy += "Tomador.CLI_CGCCPF, Tomador.CLI_FISJUR, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                    }
                    break;
                case "Categoria":
                    if (!select.Contains(" Categoria,"))
                    {
                        select += "Categoria.CTP_DESCRICAO Categoria, ";
                        groupBy += "Categoria.CTP_DESCRICAO, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                        if (!joins.Contains(" Categoria "))
                            joins += " left join T_CATEGORIA_PESSOA Categoria on Categoria.CTP_CODIGO = Tomador.CTP_CODIGO";
                    }
                    break;
                case "NomePessoa":
                    if (!select.Contains(" NomePessoa,"))
                    {
                        select += "Tomador.CLI_NOME NomePessoa, ";
                        groupBy += "Tomador.CLI_NOME, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                    }
                    break;
                case "PortadorConta":
                    if (!select.Contains(" PortadorConta,"))
                    {
                        select += "PortadorConta.CLI_NOME PortadorConta, ";
                        groupBy += "PortadorConta.CLI_NOME, ";

                        if (!joins.Contains(" PortadorConta "))
                        {
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                            joins += " left join T_CLIENTE PortadorConta on PortadorConta.CLI_CGCCPF = Tomador.CLI_CGCCPF_PORTADOR_CONTA";
                        }
                    }
                    break;
                case "TipoTitulo":
                    if (!select.Contains(" TipoTitulo,"))
                    {
                        select += "CASE WHEN Titulo.TIT_TIPO = 1 THEN 'A Receber' ELSE 'A Pagar' END TipoTitulo, ";
                        groupBy += "Titulo.TIT_TIPO, ";
                    }
                    break;
                case "MoedaCotacaoBancoCentral":
                    if (!select.Contains(" MoedaCotacaoBancoCentral,"))
                    {
                        select += "CASE WHEN Titulo.TIT_MOEDA_COTACAO_BANCO_CENTRAL = 0 THEN 'Real' WHEN Titulo.TIT_MOEDA_COTACAO_BANCO_CENTRAL = 1 THEN 'Dólar (Venda)' WHEN Titulo.TIT_MOEDA_COTACAO_BANCO_CENTRAL = 10813 THEN 'Dolar (Compra)' WHEN Titulo.TIT_MOEDA_COTACAO_BANCO_CENTRAL = 3 THEN 'Peso Argentino' WHEN Titulo.TIT_MOEDA_COTACAO_BANCO_CENTRAL = 4 THEN 'Peso Uruguaio' WHEN Titulo.TIT_MOEDA_COTACAO_BANCO_CENTRAL = 5 THEN 'Peso Chileno' WHEN Titulo.TIT_MOEDA_COTACAO_BANCO_CENTRAL = 6 THEN 'Guarani' ELSE 'Real' END MoedaCotacaoBancoCentral, ";
                        groupBy += "Titulo.TIT_MOEDA_COTACAO_BANCO_CENTRAL, ";
                    }
                    break;
                case "StatusTitulo":
                    if (!select.Contains(" StatusTitulo,"))
                    {
                        select += "CASE WHEN Titulo.TIT_STATUS = 1 THEN 'Aberto' WHEN Titulo.TIT_STATUS = 4 THEN 'Cancelado' ELSE 'Quitado' END StatusTitulo, ";
                        groupBy += "Titulo.TIT_STATUS, ";
                    }
                    break;
                case "DataEmissaoFormatada":
                case "DataEmissao":
                    if (!select.Contains(" DataEmissao,"))
                    {
                        select += "Titulo.TIT_DATA_EMISSAO DataEmissao, ";
                        groupBy += "Titulo.TIT_DATA_EMISSAO, ";
                    }
                    break;
                case "DataVencimento":
                    if (!select.Contains(" DataVencimento,"))
                    {
                        select += "Titulo.TIT_DATA_VENCIMENTO DataVencimento, ";
                        groupBy += "Titulo.TIT_DATA_VENCIMENTO, ";
                    }
                    break;
                case "DataVencimentoFormatada":
                    if (!select.Contains(" DataVencimento,"))
                    {
                        select += "Titulo.TIT_DATA_VENCIMENTO DataVencimento, ";
                        groupBy += "Titulo.TIT_DATA_VENCIMENTO, ";
                    }
                    break;
                case "DataPagamento":
                    if (!select.Contains(" DataPagamento,"))
                    {
                        select += "CONVERT(NVARCHAR(20), Titulo.TIT_DATA_LIQUIDACAO, 103) DataPagamento, ";
                        groupBy += "Titulo.TIT_DATA_LIQUIDACAO, ";
                    }
                    break;
                case "DataAutorizacao":
                    if (!select.Contains(" DataAutorizacao,"))
                    {
                        select += "CONVERT(NVARCHAR(20), Titulo.TIT_DATA_AUTORIZACAO, 103) DataAutorizacao, ";
                        groupBy += "Titulo.TIT_DATA_AUTORIZACAO, ";
                    }
                    break;
                case "DataCancelamento":
                    if (!select.Contains(" DataCancelamento,"))
                    {
                        select += "CONVERT(NVARCHAR(20), Titulo.TIT_DATA_CANCELAMENTO, 103) DataCancelamento, ";
                        groupBy += "Titulo.TIT_DATA_CANCELAMENTO, ";
                    }
                    break;
                case "DataBaseCRT":
                    if (!select.Contains(" DataBaseCRT,"))
                    {
                        select += "CONVERT(NVARCHAR(20), Titulo.TIT_DATA_BASE_CRT, 103) DataBaseCRT, ";
                        groupBy += "Titulo.TIT_DATA_BASE_CRT, ";
                    }
                    break;
                case "ValorDesonto":
                    if (!select.Contains(" ValorDesonto,"))
                        select += "SUM(Titulo.TIT_DESCONTO) ValorDesonto, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "ValorAcrescimo":
                    if (!select.Contains(" ValorAcrescimo,"))
                        select += "SUM(Titulo.TIT_ACRESCIMO) ValorAcrescimo, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao,"))
                    {
                        select += "Titulo.TIT_OBSERVACAO Observacao, ";
                        groupBy += "Titulo.TIT_OBSERVACAO, ";
                    }
                    break;
                case "ValorPago":
                    if (!select.Contains(" ValorPago,"))
                        select += "SUM(Titulo.TIT_VALOR_PAGO) ValorPago, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "ValorMoedaCotacao":
                    if (!select.Contains(" ValorMoedaCotacao,"))
                        select += "SUM(Titulo.TIT_VALOR_MOEDA_COTACAO) ValorMoedaCotacao, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "ValorOriginalMoedaEstrangeira":
                    if (!select.Contains(" ValorOriginalMoedaEstrangeira,"))
                        select += "SUM(Titulo.TIT_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA) ValorOriginalMoedaEstrangeira, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "ValorTitulo":
                    if (!select.Contains(" ValorTitulo,"))
                        select += "SUM(Titulo.TIT_VALOR_ORIGINAL) ValorTitulo, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "ValorPendente":
                    if (!select.Contains(" ValorPendente,"))
                        select += "SUM(Titulo.TIT_VALOR_PENDENTE) ValorPendente, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "ValorSaldo":
                    if (!select.Contains(" ValorSaldo,"))
                        select += "SUM(CASE WHEN ISNULL(Titulo.TIT_VALOR_PENDENTE, 0) > 0 THEN(ISNULL(Titulo.TIT_VALOR_PENDENTE, 0) + ISNULL(Titulo.TIT_ACRESCIMO, 0) - ISNULL(Titulo.TIT_DESCONTO, 0)) ELSE 0 END) ValorSaldo, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "DescontoGeracao":
                    if (!select.Contains(" DescontoGeracao,"))
                        select += "SUM(Titulo.TIT_VALOR_DESCONTO) DescontoGeracao, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "AcrescimoGeracao":
                    if (!select.Contains(" AcrescimoGeracao,"))
                        select += "SUM(Titulo.TIT_VALOR_ACRESCIMO) AcrescimoGeracao, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "DescontoBaixa":
                    if (!select.Contains(" DescontoBaixa,"))
                        select += "SUM(Titulo.TIT_VALOR_DESCONTO_BAIXA) DescontoBaixa, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "AcrescimoBaixa":
                    if (!select.Contains(" AcrescimoBaixa,"))
                        select += "SUM(Titulo.TIT_VALOR_ACRESCIMO_BAIXA) AcrescimoBaixa, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;
                case "DataLancamentoNota":
                    if (!select.Contains(" DataLancamentoNota,"))
                    {
                        select += "CONVERT(NVARCHAR(20), DocumentoEntrada.TDE_DATA_ENTRADA, 103) DataLancamentoNota, ";
                        groupBy += "DocumentoEntrada.TDE_DATA_ENTRADA, ";

                        if (!joins.Contains(" DocumentoEntradaDuplicata "))
                            joins += " left join T_TMS_DOCUMENTO_ENTRADA_DUPLICATA DocumentoEntradaDuplicata on DocumentoEntradaDuplicata.TDD_CODIGO = Titulo.TDD_CODIGO";

                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " left join T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada on DocumentoEntrada.TDE_CODIGO = DocumentoEntradaDuplicata.TDE_CODIGO";
                    }
                    break;
                case "NaturezaOperacao":
                    if (!select.Contains(" NaturezaOperacao,"))
                    {
                        select += "NaturezaOperacaoDocumentoEntrada.NAT_DESCRICAO NaturezaOperacao, ";
                        groupBy += "NaturezaOperacaoDocumentoEntrada.NAT_DESCRICAO, ";

                        if (!joins.Contains(" DocumentoEntradaDuplicata "))
                            joins += " left join T_TMS_DOCUMENTO_ENTRADA_DUPLICATA DocumentoEntradaDuplicata on DocumentoEntradaDuplicata.TDD_CODIGO = Titulo.TDD_CODIGO";

                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " left join T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada on DocumentoEntrada.TDE_CODIGO = DocumentoEntradaDuplicata.TDE_CODIGO";

                        if (!joins.Contains(" NaturezaOperacaoDocumentoEntrada "))
                            joins += " left join T_NATUREZAOPERACAO NaturezaOperacaoDocumentoEntrada on DocumentoEntrada.NAT_CODIGO = NaturezaOperacaoDocumentoEntrada.NAT_CODIGO";
                    }
                    break;
                case "ModeloDocumento":
                    if (!select.Contains(" ModeloDocumento,"))
                    {
                        select += @"CASE
					                    WHEN Titulo.CFT_CODIGO IS NOT NULL THEN 'Contrato Frete' 
					                    WHEN Titulo.TBN_CODIGO IS NOT NULL AND Titulo.TIT_TIPO = 2 THEN (SELECT SUBSTRING((SELECT DISTINCT ', ' + CAST(MD.MOD_ABREVIACAO AS NVARCHAR(4000)) 
                                                                              FROM T_TITULO_BAIXA_NEGOCIACAO N 
                                                                              JOIN T_TITULO_BAIXA TB ON TB.TIB_CODIGO = N.TIB_CODIGO
                                                                              JOIN T_TITULO_BAIXA_AGRUPADO TA ON TA.TIB_CODIGO = TB.TIB_CODIGO
                                                                              JOIN T_TITULO TT ON TT.TIT_CODIGO = TA.TIT_CODIGO
                                                                              JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA ND ON ND.TDD_CODIGO = TT.TDD_CODIGO 
                                                                              JOIN T_TMS_DOCUMENTO_ENTRADA D ON D.TDE_CODIGO = ND.TDE_CODIGO 
                                                                              JOIN T_MODDOCFISCAL MD ON MD.MOD_CODIGO = D.MOD_CODIGO
                                                                             WHERE TB.TIB_SITUACAO != 4 AND N.TBN_CODIGO = Titulo.TBN_CODIGO FOR XML PATH('')), 3, 4000))
					                    WHEN ModeloDocumentoEntrada.MOD_ABREVIACAO IS NOT NULL THEN ModeloDocumentoEntrada.MOD_ABREVIACAO
										WHEN Titulo.NFI_CODIGO IS NOT NULL THEN 'Nota Fiscal'
										WHEN Titulo.FAP_CODIGO IS NOT NULL THEN 'Fatura'
										WHEN Titulo.FCD_CODIGO IS NOT NULL THEN 'Fatura'
					                    ELSE 'Outros'
				                    END ModeloDocumento, ";

                        if (!joins.Contains(" DocumentoEntradaDuplicata "))
                            joins += " left join T_TMS_DOCUMENTO_ENTRADA_DUPLICATA DocumentoEntradaDuplicata on DocumentoEntradaDuplicata.TDD_CODIGO = Titulo.TDD_CODIGO";

                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " left join T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada on DocumentoEntrada.TDE_CODIGO = DocumentoEntradaDuplicata.TDE_CODIGO";

                        if (!joins.Contains(" ModeloDocumentoEntrada "))
                            joins += " left join T_MODDOCFISCAL ModeloDocumentoEntrada on ModeloDocumentoEntrada.MOD_CODIGO = DocumentoEntrada.MOD_CODIGO";

                        if (!groupBy.Contains("Titulo.TIT_TIPO"))
                            groupBy += "Titulo.TIT_TIPO, ";

                        if (!groupBy.Contains("ModeloDocumentoEntrada.MOD_ABREVIACAO"))
                            groupBy += "ModeloDocumentoEntrada.MOD_ABREVIACAO, ";

                        if (!groupBy.Contains("Titulo.TBN_CODIGO"))
                            groupBy += "Titulo.TBN_CODIGO, ";

                        if (!groupBy.Contains("Titulo.TDD_CODIGO"))
                            groupBy += "Titulo.TDD_CODIGO, ";

                        if (!groupBy.Contains("Titulo.CFT_CODIGO"))
                            groupBy += "Titulo.CFT_CODIGO, ";

                        if (!groupBy.Contains("Titulo.NFI_CODIGO"))
                            groupBy += "Titulo.NFI_CODIGO, ";

                        if (!groupBy.Contains("Titulo.FAP_CODIGO"))
                            groupBy += "Titulo.FAP_CODIGO, ";

                        if (!groupBy.Contains("Titulo.FCD_CODIGO"))
                            groupBy += "Titulo.FCD_CODIGO, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;

                case "TipoDocumentoTituloOriginal":
                    if (!select.Contains(" TipoDocumentoTituloOriginal,"))
                    {
                        select += "ISNULL((SELECT SUBSTRING((SELECT DISTINCT ', ' + ModeloDocumentoFiscal.MOD_ABREVIACAO FROM T_TITULO_DOCUMENTO TituloDocumento JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO JOIN T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = DocumentoFaturamento.MOD_CODIGO WHERE TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 4000)), Titulo.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL) TipoDocumentoTituloOriginal, ";
                        groupBy += "Titulo.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "NumeroDocumentoTituloOriginal":
                    if (!select.Contains(" NumeroDocumentoTituloOriginal,"))
                    {
                        select += "Titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumentoTituloOriginal, ";
                        groupBy += "Titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL, ";
                    }
                    break;
                case "NumeroBoleto":
                    if (!select.Contains(" NumeroBoleto,"))
                    {
                        select += "Titulo.TIT_NOSSO_NUMERO NumeroBoleto, ";
                        groupBy += "Titulo.TIT_NOSSO_NUMERO, ";
                    }
                    break;
                case "DataBaseLiquidacao":
                    if (!select.Contains(" DataBaseLiquidacao,"))
                    {
                        select += "CONVERT(NVARCHAR(20), Titulo.TIT_DATA_BASE_LIQUIDACAO, 103) DataBaseLiquidacao, ";
                        groupBy += "Titulo.TIT_DATA_BASE_LIQUIDACAO, ";
                    }
                    break;
                case "DataProgramacaoPagamento":
                    if (!select.Contains(" DataProgramacaoPagamento,"))
                    {
                        select += "CONVERT(NVARCHAR(20), Titulo.TIT_DATA_PROGRAMACAO_PAGAMENTO, 103) DataProgramacaoPagamento, ";
                        groupBy += "Titulo.TIT_DATA_PROGRAMACAO_PAGAMENTO, ";
                    }
                    break;
                case "GrupoPessoa":
                    if (!select.Contains(" GrupoPessoa,"))
                    {
                        select += "GrupoPessoas.GRP_DESCRICAO GrupoPessoa, ";
                        groupBy += "GrupoPessoas.GRP_DESCRICAO, ";

                        if (!joins.Contains(" GrupoPessoas "))
                            joins += " left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = Titulo.GRP_CODIGO";
                    }
                    break;
                case "DataEmissaoDocumentos":
                    if (!select.Contains(" DataEmissaoDocumentos,"))
                    {
                        select += "(SELECT SUBSTRING((SELECT DISTINCT ', ' + CONVERT(NVARCHAR(20), DocumentoFaturamento.DFA_DATAHORAEMISSAO, 103) FROM T_TITULO_DOCUMENTO TituloDocumento JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on(TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) WHERE TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 4000)) DataEmissaoDocumentos, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "Conhecimentos":
                    if (!select.Contains(" Conhecimentos,"))
                    {
                        select += "(SELECT SUBSTRING((SELECT DISTINCT ', ' + CONVERT(NVARCHAR(20), DocumentoFaturamento.DFA_NUMERO, 103) FROM T_TITULO_DOCUMENTO TituloDocumento JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on(TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) WHERE TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 4000)) Conhecimentos, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "SeriesConhecimentos":
                    if (!select.Contains(" SeriesConhecimentos,"))
                    {
                        select += @"(SELECT SUBSTRING((SELECT DISTINCT ', ' + CONVERT(NVARCHAR(20), Serie.ESE_NUMERO, 103) 
                            FROM T_TITULO_DOCUMENTO TituloDocumento 
                            JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on (TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO)                             
                            JOIN T_EMPRESA_SERIE Serie on Serie.ESE_CODIGO = DocumentoFaturamento.ESE_CODIGO
                            WHERE TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 4000)) SeriesConhecimentos, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "Cargas":
                    if (!select.Contains(" Cargas,"))
                    {
                        select += @"(SELECT SUBSTRING((SELECT DISTINCT ', ' + CONVERT(NVARCHAR(20), ISNULL(CTeCarga.CAR_CODIGO_CARGA_EMBARCADOR, Carga.CAR_CODIGO_CARGA_EMBARCADOR), 103) 
                            FROM T_TITULO_DOCUMENTO TituloDocumento 
                            JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on (TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) 
                            LEFT OUTER JOIN T_CARGA Carga on Carga.CAR_CODIGO = TituloDocumento.CAR_CODIGO
                            LEFT OUTER JOIN T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = TituloDocumento.CON_CODIGO 
                            LEFT OUTER JOIN T_CARGA CTeCarga on CTeCarga.CAR_CODIGO = CargaCTe.CAR_CODIGO
                            WHERE TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 4000)) Cargas, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "EmpresaConhecimentos":
                    if (!select.Contains(" EmpresaConhecimentos,"))
                    {
                        select += "(SELECT SUBSTRING((SELECT DISTINCT ', ' + SUBSTRING(Empresa.EMP_CNPJ,1,2) + '.' + SUBSTRING(Empresa.EMP_CNPJ,3,3) + '.' + SUBSTRING(Empresa.EMP_CNPJ,6,3) + '/' + SUBSTRING(Empresa.EMP_CNPJ,9,4) + '-' + SUBSTRING(Empresa.EMP_CNPJ,13,2) + ' - ' + Empresa.EMP_RAZAO FROM T_TITULO_DOCUMENTO TituloDocumento JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on (TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) JOIN T_EMPRESA Empresa ON DocumentoFaturamento.EMP_CODIGO = Empresa.EMP_CODIGO WHERE TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 4000)) EmpresaConhecimentos, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "AgenciaCliente":
                    if (!select.Contains(" AgenciaCliente,"))
                    {
                        select += "Tomador.CLI_BANCO_AGENCIA AgenciaCliente, ";
                        groupBy += "Tomador.CLI_BANCO_AGENCIA, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                    }
                    break;
                case "DigitoAgenciaCliente":
                    if (!select.Contains(" DigitoAgenciaCliente,"))
                    {
                        select += "Tomador.CLI_BANCO_DIGITO_AGENCIA DigitoAgenciaCliente, ";
                        groupBy += "Tomador.CLI_BANCO_DIGITO_AGENCIA, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                    }
                    break;
                case "NumeroContaCliente":
                    if (!select.Contains(" NumeroContaCliente,"))
                    {
                        select += "Tomador.CLI_BANCO_NUMERO_CONTA NumeroContaCliente, ";
                        groupBy += "Tomador.CLI_BANCO_NUMERO_CONTA, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                    }
                    break;
                case "TipoContaCliente":
                    if (!select.Contains(" TipoContaCliente,"))
                    {
                        select += "CASE Tomador.CLI_BANCO_TIPO_CONTA WHEN 1 THEN 'Corrente' WHEN 2 THEN 'Poupança' ELSE '' END TipoContaCliente, ";
                        groupBy += "Tomador.CLI_BANCO_TIPO_CONTA, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                    }
                    break;
                case "BancoCliente":
                    if (!select.Contains(" BancoCliente,"))
                    {
                        select += "BancoTomador.BCO_DESCRICAO BancoCliente, ";
                        groupBy += "BancoTomador.BCO_DESCRICAO, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";

                        if (!joins.Contains(" BancoTomador "))
                            joins += " left join T_BANCO BancoTomador on BancoTomador.BCO_CODIGO = Tomador.BCO_CODIGO";
                    }
                    break;
                case "ContaFornecedorEBS":
                    if (!select.Contains(" ContaFornecedorEBS,"))
                    {
                        select += "Tomador.CLI_CONTA_FORNECEDOR_EBS ContaFornecedorEBS, ";
                        groupBy += "Tomador.CLI_CONTA_FORNECEDOR_EBS, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                    }
                    break;
                case "Adiantado":
                    if (!select.Contains(" Adiantado,"))
                    {
                        select += "CASE WHEN Titulo.TIT_ADIANTADO = 1 THEN 'Sim' ELSE 'Não' END Adiantado, ";
                        groupBy += "Titulo.TIT_ADIANTADO, ";
                    }
                    break;
                case "Portador":
                    if (!select.Contains(" Portador,"))
                    {
                        select += "Portador.CLI_NOME Portador, ";
                        groupBy += "Portador.CLI_NOME, ";

                        if (!joins.Contains(" Portador "))
                            joins += " left join T_CLIENTE Portador ON Portador.CLI_CGCCPF = Titulo.CLI_CGCCPF_PORTADOR";
                    }
                    break;
                case "NumeroControleCTe":
                    if (!select.Contains(" NumeroControleCTe,"))
                    {
                        select += "ISNULL(SUBSTRING((SELECT DISTINCT ', ' + CAST(ISNULL(Cte.CON_NUMERO_CONTROLE, '') AS NVARCHAR(500)) FROM T_CTE Cte JOIN T_TITULO_DOCUMENTO TituloDocumento ON TituloDocumento.CON_CODIGO = Cte.CON_CODIGO WHERE TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 1000), '') NumeroControleCTe, ";
                        groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "NumeroBookingCTe":
                    if (!select.Contains(" NumeroBookingCTe,"))
                    {
                        select += "Cte.CON_NUMERO_BOOKING NumeroBookingCTe, ";
                        groupBy += "Cte.CON_NUMERO_BOOKING, ";

                        if (!joins.Contains(" TituloDocumento "))
                            joins += " left join T_TITULO_DOCUMENTO TituloDocumento ON TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!joins.Contains(" CTe "))
                            joins += " left join T_CTE CTe ON CTe.CON_CODIGO = TituloDocumento.CON_CODIGO";
                    }
                    break;
                case "NavioViagemDirecao":
                    if (!select.Contains(" NavioViagemDirecao,"))
                    {
                        select += "ViagemNavio.PVN_DESCRICAO NavioViagemDirecao, ";
                        groupBy += "ViagemNavio.PVN_DESCRICAO, ";

                        if (!joins.Contains(" TituloDocumento "))
                            joins += " left join T_TITULO_DOCUMENTO TituloDocumento ON TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!joins.Contains(" CTe "))
                            joins += " left join T_CTE CTe ON CTe.CON_CODIGO = TituloDocumento.CON_CODIGO";

                        if (!joins.Contains(" ViagemNavio "))
                            joins += " left join T_PEDIDO_VIAGEM_NAVIO ViagemNavio ON ViagemNavio.PVN_CODIGO = Cte.CON_VIAGEM";
                    }
                    break;
                case "TipoDocumento":
                    if (!select.Contains(" TipoDocumento,"))
                    {
                        select += " CASE WHEN Titulo.TBN_CODIGO IS NOT NULL THEN 'Negociação' WHEN Titulo.TDD_CODIGO IS NOT NULL THEN 'Doc. Entrada' WHEN Titulo.CFT_CODIGO IS NOT NULL THEN 'Contrato Frete' WHEN Titulo.NFI_CODIGO IS NOT NULL THEN 'Nota Fiscal' WHEN Titulo.FAP_CODIGO IS NOT NULL THEN 'Fatura' WHEN Titulo.FCD_CODIGO IS NOT NULL THEN 'Fatura' ELSE 'Outros' END TipoDocumento, ";

                        if (!groupBy.Contains("Titulo.TBN_CODIGO"))
                            groupBy += "Titulo.TBN_CODIGO, ";

                        if (!groupBy.Contains("Titulo.TDD_CODIGO"))
                            groupBy += "Titulo.TDD_CODIGO, ";

                        if (!groupBy.Contains("Titulo.CFT_CODIGO"))
                            groupBy += "Titulo.CFT_CODIGO, ";

                        if (!groupBy.Contains("Titulo.NFI_CODIGO"))
                            groupBy += "Titulo.NFI_CODIGO, ";

                        if (!groupBy.Contains("Titulo.FAP_CODIGO"))
                            groupBy += "Titulo.FAP_CODIGO, ";

                        if (!groupBy.Contains("Titulo.FCD_CODIGO"))
                            groupBy += "Titulo.FCD_CODIGO, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "NumeroDocumento":
                    if (!select.Contains(" NumeroDocumento,"))
                    {
                        select += @"CASE WHEN Titulo.TBN_CODIGO IS NOT NULL THEN (SELECT SUBSTRING((SELECT DISTINCT ', ' + CAST(TA.TIT_CODIGO AS NVARCHAR(4000))
                                                                              FROM T_TITULO_BAIXA_NEGOCIACAO N
                                                                              JOIN T_TITULO_BAIXA TB ON TB.TIB_CODIGO = N.TIB_CODIGO
                                                                              JOIN T_TITULO_BAIXA_AGRUPADO TA ON TA.TIB_CODIGO = TB.TIB_CODIGO
                                                                             WHERE TB.TIB_SITUACAO != 4 AND N.TBN_CODIGO = Titulo.TBN_CODIGO FOR XML PATH('')), 3, 4000))
                                        WHEN Titulo.TDD_CODIGO IS NOT NULL THEN(SELECT CAST(D.TDE_NUMERO_LONG AS NVARCHAR(4000)) FROM T_TMS_DOCUMENTO_ENTRADA_DUPLICATA NN JOIN T_TMS_DOCUMENTO_ENTRADA D ON D.TDE_CODIGO = NN.TDE_CODIGO WHERE NN.TDD_CODIGO = Titulo.TDD_CODIGO)
                                        WHEN Titulo.CFT_CODIGO IS NOT NULL THEN(SELECT CAST(NN.CFT_NUMERO_CONTRATO AS NVARCHAR(4000)) FROM T_CONTRATO_FRETE_TERCEIRO NN WHERE NN.CFT_CODIGO = Titulo.CFT_CODIGO)
                                        WHEN Titulo.NFI_CODIGO IS NOT NULL THEN(SELECT CAST(NN.NFI_NUMERO AS NVARCHAR(4000)) FROM T_NOTA_FISCAL NN WHERE NN.NFI_CODIGO = Titulo.NFI_CODIGO)
                                        WHEN Titulo.FAP_CODIGO IS NOT NULL THEN(SELECT CAST(F.FAT_NUMERO AS NVARCHAR(4000)) FROM T_FATURA_PARCELA P JOIN T_FATURA F ON F.FAT_CODIGO = P.FAT_CODIGO WHERE P.FAP_CODIGO = Titulo.FAP_CODIGO)
                                        WHEN Titulo.FCD_CODIGO IS NOT NULL THEN(SELECT CAST(F.FAT_NUMERO AS NVARCHAR(4000)) FROM T_FATURA_CARGA_DOCUMENTO P JOIN T_FATURA F ON F.FAT_CODIGO = P.FAT_CODIGO WHERE P.FCD_CODIGO = Titulo.FCD_CODIGO)
                                        ELSE '0'
                                    END NumeroDocumento, ";

                        if (!groupBy.Contains("Titulo.TBN_CODIGO"))
                            groupBy += "Titulo.TBN_CODIGO, ";

                        if (!groupBy.Contains("Titulo.TDD_CODIGO"))
                            groupBy += "Titulo.TDD_CODIGO, ";

                        if (!groupBy.Contains("Titulo.CFT_CODIGO"))
                            groupBy += "Titulo.CFT_CODIGO, ";

                        if (!groupBy.Contains("Titulo.NFI_CODIGO"))
                            groupBy += "Titulo.NFI_CODIGO, ";

                        if (!groupBy.Contains("Titulo.FAP_CODIGO"))
                            groupBy += "Titulo.FAP_CODIGO, ";

                        if (!groupBy.Contains("Titulo.FCD_CODIGO"))
                            groupBy += "Titulo.FCD_CODIGO, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "Parcela":
                    if (!select.Contains(" Parcela,"))
                    {
                        select += @"CAST(Titulo.TIT_SEQUENCIA AS NVARCHAR(20)) +
				                    CASE
                                        WHEN Titulo.TBN_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.TBN_CODIGO = Titulo.TBN_CODIGO)
                                        WHEN Titulo.TDD_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.TDD_CODIGO = Titulo.TDD_CODIGO)
                                        WHEN Titulo.CFT_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.CFT_CODIGO = Titulo.CFT_CODIGO)
                                        WHEN Titulo.NFI_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.NFI_CODIGO = Titulo.NFI_CODIGO)
                                        WHEN Titulo.FAP_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.FAP_CODIGO = Titulo.FAP_CODIGO) 
                                        WHEN Titulo.CMA_CODIGO IS NOT NULL THEN ' / ' + (SELECT CAST(MAX(T.TIT_SEQUENCIA) AS NVARCHAR(20)) FROM T_TITULO T WHERE T.CMA_CODIGO = Titulo.CMA_CODIGO) 
                                        WHEN ContratoFinanciamentoParcela.TIT_CODIGO IS NOT NULL THEN ' / ' + 
                                            (SELECT CAST(MAX(CFP.CFP_SEQUENCIA) AS NVARCHAR(20)) FROM T_CONTRATO_FINANCIAMENTO_PARCELA CFP 
                                            JOIN T_CONTRATO_FINANCIAMENTO CF ON CF.CFI_CODIGO = CFP.CFI_CODIGO
                                            WHERE CF.CFI_CODIGO = ContratoFinanciamento.CFI_CODIGO) 
                                        ELSE ''
                                    END Parcela, ";

                        if (!joins.Contains(" ContratoFinanciamentoParcela "))
                            joins += " left join T_CONTRATO_FINANCIAMENTO_PARCELA ContratoFinanciamentoParcela on ContratoFinanciamentoParcela.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!joins.Contains(" ContratoFinanciamento "))
                            joins += " left join T_CONTRATO_FINANCIAMENTO ContratoFinanciamento on ContratoFinanciamento.CFI_CODIGO = ContratoFinanciamentoParcela.CFI_CODIGO";

                        if (!groupBy.Contains("Titulo.TIT_SEQUENCIA"))
                            groupBy += "Titulo.TIT_SEQUENCIA, ";

                        if (!groupBy.Contains("Titulo.TBN_CODIGO"))
                            groupBy += "Titulo.TBN_CODIGO, ";

                        if (!groupBy.Contains("Titulo.TDD_CODIGO"))
                            groupBy += "Titulo.TDD_CODIGO, ";

                        if (!groupBy.Contains("Titulo.TDD_CODIGO"))
                            groupBy += "Titulo.TDD_CODIGO, ";

                        if (!groupBy.Contains("Titulo.CMA_CODIGO"))
                            groupBy += "Titulo.CMA_CODIGO, ";

                        if (!groupBy.Contains("Titulo.CFT_CODIGO"))
                            groupBy += "Titulo.CFT_CODIGO, ";

                        if (!groupBy.Contains("Titulo.NFI_CODIGO"))
                            groupBy += "Titulo.NFI_CODIGO, ";

                        if (!groupBy.Contains("Titulo.FAP_CODIGO"))
                            groupBy += "Titulo.FAP_CODIGO, ";

                        if (!groupBy.Contains("Titulo.FCD_CODIGO"))
                            groupBy += "Titulo.FCD_CODIGO, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";

                        if (!groupBy.Contains("ContratoFinanciamento.CFI_CODIGO"))
                            groupBy += "ContratoFinanciamento.CFI_CODIGO, ";

                        if (!groupBy.Contains("ContratoFinanciamentoParcela.TIT_CODIGO"))
                            groupBy += "ContratoFinanciamentoParcela.TIT_CODIGO, ";
                    }
                    break;
                case "DescricaoFormaTitulo":
                    if (!select.Contains(" FormaTitulo,"))
                    {
                        select += "Titulo.TIT_FORMA FormaTitulo, ";
                        groupBy += "Titulo.TIT_FORMA, ";
                    }
                    break;
                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select += new System.Text.StringBuilder()
                            .Append("(SELECT SUBSTRING(( ")
                            .Append("           SELECT DISTINCT ', ' + CAST(T_TIPO_OPERACAO.TOP_DESCRICAO AS NVARCHAR(4000)) ")
                            .Append("			  FROM T_TITULO_DOCUMENTO ")
                            .Append("			  LEFT JOIN T_CARGA_CTE ON T_TITULO_DOCUMENTO.CON_CODIGO = T_CARGA_CTE.CON_CODIGO ")
                            .Append("			  JOIN T_CARGA ")
                            .Append("				ON ( ")
                            .Append("					   (T_CARGA_CTE.CAR_CODIGO IS NOT NULL AND T_CARGA_CTE.CAR_CODIGO = T_CARGA.CAR_CODIGO) OR ")
                            .Append("					   (T_TITULO_DOCUMENTO.CAR_CODIGO IS NOT NULL AND T_TITULO_DOCUMENTO.CAR_CODIGO = T_CARGA.CAR_CODIGO) ")
                            .Append("				   ) ")
                            .Append("			  JOIN T_TIPO_OPERACAO ON T_CARGA.TOP_CODIGO = T_TIPO_OPERACAO.TOP_CODIGO ")
                            .Append("            WHERE T_TITULO_DOCUMENTO.TIT_CODIGO = Titulo.TIT_CODIGO ")
                            .Append("			   FOR XML PATH('') ")
                            .Append("        ), 3, 4000) ")
                            .Append(") TipoOperacao, ")
                            .ToString();
                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "Provisao":
                    if (!select.Contains(" Provisao,"))
                    {
                        select += @"CASE 
                                        WHEN Titulo.TIT_PROVISAO = 1 THEN 'Sim'
                                        ELSE 'Não' 
                                    END Provisao, ";
                        groupBy += "Titulo.TIT_PROVISAO, ";
                    }
                    break;
                case "TitulosAgrupados"://Usado no relatório de títulos sem movimentação
                    if (!select.Contains(" TitulosAgrupados,"))
                    {
                        select += @"CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(T.TIT_CODIGO AS NVARCHAR(20)) FROM T_TITULO_BAIXA_NEGOCIACAO N
                            JOIN T_TITULO_BAIXA B ON B.TIB_CODIGO = N.TIB_CODIGO
                            JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                            JOIN T_TITULO T ON T.TBN_CODIGO = N.TBN_CODIGO
                            WHERE B.TIB_SITUACAO != 4 AND A.TIT_CODIGO = TITULO.TIT_CODIGO FOR XML PATH('')), 3, 2000) AS NVARCHAR(2000)) TitulosAgrupados, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "Remessa":
                    if (!select.Contains(" Remessa,"))
                    {
                        select += @"Isnull(CAST(BoletoRemessa.BRE_NUMERO_SEQUENCIAL AS NVARCHAR(10)) , 
                            SUBSTRING((SELECT DISTINCT ', ' + CAST(P.PAE_NUMERO AS NVARCHAR(20))
                                        FROM T_PAGAMENTO_ELETRONICO_TITULO TT
                                        JOIN T_PAGAMENTO_ELETRONICO P ON P.PAE_CODIGO = TT.PAE_CODIGO
                                        WHERE TT.TIT_CODIGO = Titulo.TIT_CODIGO ), 3, 1000)) Remessa, ";
                        groupBy += "BoletoRemessa.BRE_NUMERO_SEQUENCIAL, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";

                        if (!joins.Contains(" BoletoRemessa "))
                            joins += " left join T_BOLETO_REMESSA BoletoRemessa on BoletoRemessa.BRE_CODIGO = Titulo.BRE_CODIGO";
                    }
                    break;
                case "ArquivoRemessa":
                    if (!select.Contains(" ArquivoRemessa,"))
                    {
                        select += "BoletoRemessa.BRE_OBSERVACAO ArquivoRemessa, ";
                        groupBy += "BoletoRemessa.BRE_OBSERVACAO, ";

                        if (!joins.Contains(" BoletoRemessa "))
                            joins += " left join T_BOLETO_REMESSA BoletoRemessa on BoletoRemessa.BRE_CODIGO = Titulo.BRE_CODIGO";
                    }
                    break;
                case "CodigoRemessa":
                    if (!select.Contains(" CodigoRemessa,"))
                    {
                        select += "CAST(BoletoRemessa.BRE_CODIGO AS NVARCHAR(10)) CodigoRemessa, ";
                        groupBy += "BoletoRemessa.BRE_CODIGO, ";

                        if (!joins.Contains(" BoletoRemessa "))
                            joins += " left join T_BOLETO_REMESSA BoletoRemessa on BoletoRemessa.BRE_CODIGO = Titulo.BRE_CODIGO";
                    }
                    break;
                case "NumerosCheques":
                    if (!select.Contains(" NumerosCheques, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + cheque.CHQ_NUMERO_CHEQUE
                                        FROM T_CHEQUE cheque
                                        JOIN T_TITULO_BAIXA_CHEQUE baixaCheque ON baixaCheque.CHQ_CODIGO = cheque.CHQ_CODIGO
                                        JOIN T_TITULO_BAIXA baixa ON baixa.TIB_CODIGO = baixaCheque.TIB_CODIGO
                                        JOIN T_TITULO_BAIXA_AGRUPADO baixaAgrupada ON baixaAgrupada.TIB_CODIGO = baixa.TIB_CODIGO
                                        WHERE baixa.TIB_SITUACAO != 4 AND baixaAgrupada.TIT_CODIGO = Titulo.TIT_CODIGO FOR XML PATH('')), 3, 2000) NumerosCheques, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "ObservacaoInterna":
                    if (!select.Contains(" ObservacaoInterna, "))
                    {
                        select += "Titulo.TIT_OBSERVACAO_INTERNA as ObservacaoInterna, ";

                        if (!groupBy.Contains("Titulo.TIT_OBSERVACAO_INTERNA"))
                            groupBy += "Titulo.TIT_OBSERVACAO_INTERNA, ";
                    }
                    break;
                case "OrdemServico":
                    if (!select.Contains(" OrdemServico,"))
                    {
                        select += "OrdemServico.OSE_NUMERO OrdemServico, ";
                        groupBy += "OrdemServico.OSE_NUMERO, ";

                        if (!joins.Contains(" DocumentoEntradaDuplicata "))
                            joins += " left join T_TMS_DOCUMENTO_ENTRADA_DUPLICATA DocumentoEntradaDuplicata on DocumentoEntradaDuplicata.TDD_CODIGO = Titulo.TDD_CODIGO";

                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " left join T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada on DocumentoEntrada.TDE_CODIGO = DocumentoEntradaDuplicata.TDE_CODIGO";

                        if (!joins.Contains(" OrdemServico "))
                            joins += " left join T_FROTA_ORDEM_SERVICO OrdemServico on OrdemServico.OSE_CODIGO = DocumentoEntrada.OSE_CODIGO";
                    }
                    break;
                case "OrdemCompra":
                    if (!select.Contains(" OrdemCompra,"))
                    {
                        select += "OrdemCompra.ORC_NUMERO OrdemCompra, ";
                        groupBy += "OrdemCompra.ORC_NUMERO, ";

                        if (!joins.Contains(" DocumentoEntradaDuplicata "))
                            joins += " left join T_TMS_DOCUMENTO_ENTRADA_DUPLICATA DocumentoEntradaDuplicata on DocumentoEntradaDuplicata.TDD_CODIGO = Titulo.TDD_CODIGO";

                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " left join T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada on DocumentoEntrada.TDE_CODIGO = DocumentoEntradaDuplicata.TDE_CODIGO";

                        if (!joins.Contains(" OrdemCompra "))
                            joins += " left join T_ORDEM_COMPRA OrdemCompra on OrdemCompra.ORC_CODIGO = DocumentoEntrada.ORC_CODIGO";
                    }
                    break;

                case "VariacaoCambial":
                    if (!select.Contains(" VariacaoCambial, "))
                    {
                        select += "( SUM(Titulo.TIT_VALOR_ORIGINAL) - SUM(Titulo.TIT_VALOR_PAGO) ) VariacaoCambial, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;

                case "TipoChavePixFormatada":
                    if (!select.Contains(" TipoChavePix,"))
                    {
                        select += "Tomador.CLI_TIPO_CHAVE_PIX TipoChavePix, ";
                        groupBy += "Tomador.CLI_TIPO_CHAVE_PIX, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                    }
                    break;

                case "ChavePix":
                    if (!select.Contains(" ChavePix,"))
                    {
                        select += "Tomador.CLI_CHAVE_PIX ChavePix, ";
                        groupBy += "Tomador.CLI_CHAVE_PIX, ";

                        if (!joins.Contains(" Tomador "))
                            joins += " left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Titulo.CLI_CGCCPF";
                    }
                    break;

                case "AcrescimoNaBaixa":
                    if (!select.Contains(" AcrescimentoNaBaixa, "))
                        select += @"(select SUM(c.TBA_VALOR) from T_TITULO_BAIXA b
                                            join T_TITULO_BAIXA_AGRUPADO a on a.TIB_CODIGO = b.TIB_CODIGO
                                            join T_TITULO_BAIXA_ACRESCIMO c on c.TIB_CODIGO = b.TIB_CODIGO
                                            join T_JUSTIFICATIVA j on j.JUS_CODIGO = c.JUS_CODIGO and j.JUS_TIPO = 2
                                            where a.TIT_CODIGO = Titulo.TIT_CODIGO) AcrescimoNaBaixa, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;

                case "AcrescimoLancamentoTitulo":
                    if (!select.Contains(" AcrescimoLancamentoTitulo, "))
                        select += @"(select SUM(a.AAD_VALOR) from T_APLICAR_ACRESCIMO_DESCONTO_TITULO a
				                           left join T_JUSTIFICATIVA j on j.JUS_CODIGO = a.JUS_CODIGO 
                                           where a.TIT_CODIGO = Titulo.TIT_CODIGO
                                           and j.JUS_TIPO = 2) AcrescimoLancamentoTitulo, ";
                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";
                    break;

                case "DescontoLancamentoTitulo":
                    if (!select.Contains(" AcrescimoLancamentoTitulo, "))
                        select += @"(select SUM(a.AAD_VALOR) from T_APLICAR_ACRESCIMO_DESCONTO_TITULO a
				                           left join T_JUSTIFICATIVA j on j.JUS_CODIGO = a.JUS_CODIGO 
                                           where a.TIT_CODIGO = Titulo.TIT_CODIGO
                                           and j.JUS_TIPO = 1) DescontoLancamentoTitulo, ";

                    if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                        groupBy += "Titulo.TIT_CODIGO, ";

                    break;

                case "TipoProposta":
                    if (!select.Contains(" TipoProposta, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + case
                                                    WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 1 THEN 'Carga Fechada'
                                                    WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 2 THEN 'Carga Fracionada'
                                                    WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3 THEN 'Feeder'
                                                    WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 4 THEN 'VAS'
                                                    WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 5 THEN 'Embarque Certo - Feeder'
                                                    WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 6 THEN 'Embarque Certo - Cabotagem'
                                                    WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 7 THEN 'No Show - Cabotagem'
                                                    WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 8 THEN 'Faturamento - Contabilidade'
                                                    WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 9 THEN 'Demurrage - Cabotagem'
                                                    WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 10 THEN 'Detention - Cabotagem'
                                                else '' end
                                            from T_CARGA_PEDIDO cargaPedido
                                            inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO
                                            inner join T_TITULO_DOCUMENTO tituloDocumento on tituloDocumento.CON_CODIGO = CargaCTe.CON_CODIGO
                                        WHERE tituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO for xml path('')), 3, 1000) TipoProposta, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;

                case "NumeroProposta":
                    if (!select.Contains(" NumeroProposta, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + Pedido.PED_CODIGO_PROPOSTA
                                        from T_PEDIDO Pedido
                                        inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.PED_CODIGO = Pedido.PED_CODIGO
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO
                                        inner join T_TITULO_DOCUMENTO tituloDocumento on tituloDocumento.CON_CODIGO = CargaCTe.CON_CODIGO
                                    WHERE tituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO for xml path('')), 3, 1000) NumeroProposta, ";

                        if (!groupBy.Contains("Titulo.TIT_CODIGO"))
                            groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;

                case "Renegociado":
                    if (!select.Contains(" Renegociado, "))
                    {
                        select += @"CASE WHEN Titulo.TIT_STATUS = 3 AND (SUM(Titulo.TIT_VALOR_PAGO) = 0 OR SUM(Titulo.TIT_VALOR_PAGO) < (SUM(Titulo.TIT_VALOR_ORIGINAL) - SUM(Titulo.TIT_DESCONTO) + SUM(Titulo.TIT_ACRESCIMO))) THEN 'Sim' ELSE 'Não' END Renegociado, ";

                        if (!groupBy.Contains("Titulo.TIT_STATUS, "))
                            groupBy += "Titulo.TIT_STATUS, ";
                    }
                    break;

                case "AprovadorOrdemCompra":
                    if (!select.Contains(" AprovadorOrdemCompra, "))
                    {
                        select += "STRING_AGG(Usuario.FUN_NOME, ', ') AS AprovadorOrdemCompra, ";

                        if (!joins.Contains(" DocumentoEntradaDuplicata "))
                        {
                            joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA AS DocumentoEntradaDuplicata ";
                            joins += "        ON DocumentoEntradaDuplicata.TDD_CODIGO = Titulo.TDD_CODIGO";
                        }

                        if (!joins.Contains(" DocumentoEntrada "))
                        {
                            joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA AS DocumentoEntrada ";
                            joins += "        ON DocumentoEntrada.TDE_CODIGO = DocumentoEntradaDuplicata.TDE_CODIGO";
                        }

                        if (!joins.Contains(" OrdemCompra "))
                        {
                            joins += " LEFT JOIN T_ORDEM_COMPRA AS OrdemCompra ";
                            joins += "        ON OrdemCompra.ORC_CODIGO = DocumentoEntrada.ORC_CODIGO";
                        }

                        if (!joins.Contains(" AprovadorOrdemCompra "))
                        {
                            joins += "  LEFT JOIN T_AUTORIZACAO_ALCADA_ORDEM_COMPRA AS Aprovacao ";
                            joins += "         ON OrdemCompra.ORC_CODIGO = Aprovacao.ORC_CODIGO";
                            joins += "  LEFT JOIN T_FUNCIONARIO AS Usuario ";
                            joins += "         ON Aprovacao.FUN_CODIGO = Usuario.FUN_CODIGO";
                        }

                        if (!groupBy.Contains("OrdemCompra.ORC_NUMERO, "))
                            groupBy += "OrdemCompra.ORC_NUMERO, ";
                    }

                    break;
                case "TipoContato":
                    if (!select.Contains(" TipoContato, "))
                    {
                        select += "( SELECT STRING_AGG(TipoContato.TCO_DESCRICAO, ' , ') FROM T_CONTATO_CLIENTE_DOCUMENTO ContatoClienteDocumento LEFT JOIN T_CONTATO_CLIENTE_TIPO_CONTATO ContatoClienteTipoContato ON ContatoClienteTipoContato.CCL_CODIGO =ContatoClienteDocumento.CCL_CODIGO LEFT JOIN T_TIPO_CONTATO TipoContato ON TipoContato.TCO_CODIGO = ContatoClienteTipoContato.TCO_CODIGO WHERE ContatoClienteDocumento.TIT_CODIGO = Titulo.TIT_CODIGO) AS TipoContato, ";

                    }

                    break;
                case "SituacaoContato":
                    if (!select.Contains(" TipoContato, "))
                    {
                        select += " ( SELECT STRING_AGG(SituacaoContato.SCO_DESCRICAO, ' , ') FROM T_CONTATO_CLIENTE_DOCUMENTO ContatoClienteDocumento LEFT JOIN T_CONTATO_CLIENTE ContatoCliente ON ContatoCliente.CCL_CODIGO = ContatoClienteDocumento.CCL_CODIGO LEFT JOIN T_SITUACAO_CONTATO SituacaoContato ON SituacaoContato.SCO_CODIGO = ContatoCliente.SCO_CODIGO WHERE ContatoClienteDocumento.TIT_CODIGO = Titulo.TIT_CODIGO) AS SituacaoContato , ";
                    }

                    break;
                default:
                    break;
            }
        }

        #endregion Relatório de Títulos
    }
}
