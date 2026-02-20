using Dominio.Entidades.Embarcador.Financeiro;
using Dominio.ObjetosDeValor.CTe;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloBaixa : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>
    {
        public TituloBaixa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> BuscarPorPeriodo(DateTime dataInicial, DateTime dataFinal, SituacaoBaixaTitulo situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();

            query = query.Where(o => o.SituacaoBaixaTitulo == situacao && o.DataBaixa >= dataInicial.Date && o.DataBaixa.Value.Date < dataFinal.Date && o.Valor > 0m);

            return query.Timeout(300).ToList();
        }

        public List<int> ConsultarSeExisteTituloPendente(int codigoEmpresa, DateTime dataFechamento, SituacaoBaixaTitulo[] situacaoBaixaTitulo)
        {
            var queryTB = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();

            queryTB = queryTB.Where(o => o.DataBaixa.Value.Date < dataFechamento.Date && situacaoBaixaTitulo.Contains(o.SituacaoBaixaTitulo) && o.Titulo.Empresa.Codigo == codigoEmpresa);

            return queryTB.SelectMany(tb => tb.TitulosAgrupados.Select(tba => tba.Titulo.Codigo)).ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTitulosPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            return result.Select(obj => obj.Titulo).Timeout(3000).ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TipoMovimento BuscarTipoMovimentoPadrao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo && obj.Titulo != null && obj.Titulo.TipoMovimento != null select obj;
            return result.Select(obj => obj.Titulo.TipoMovimento).Timeout(3000).FirstOrDefault();
        }

        public bool ContemTitulosGeradosDeNegociacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo && obj.Titulo.TituloBaixaNegociacao != null select obj;
            return result.Any();
        }

        public bool ContemTituloEmBaixaGerada(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var result = from obj in query where obj.Titulo.Codigo == codigo && obj.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada select obj;
            return result.Any();
        }

        public bool ContemParcelaQuitada(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.TituloBaixaNegociacao.TituloBaixa.Codigo == codigo && obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada select obj;
            return result.Select(o => o.Codigo).Any();
        }

        public int CodigoFaturaBaixaAReceber(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigo && o.Titulo.FaturaParcela != null);

            List<int> codigosFaturas = query.Select(o => o.Titulo.FaturaParcela.Fatura.Codigo).Distinct().ToList();

            //var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            //int codigoFatura = 0;
            //if (result.Count() > 0)
            //{
            //    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> listaTitulos = result.ToList();
            //    for (int i = 0; i < listaTitulos.Count(); i++)
            //    {
            //        if (listaTitulos[i].Titulo.FaturaParcela != null && listaTitulos[i].Titulo.FaturaParcela.Fatura != null)
            //        {
            //            if (codigoFatura == 0)
            //                codigoFatura = listaTitulos[i].Titulo.FaturaParcela.Fatura.Codigo;
            //            else if (codigoFatura != listaTitulos[i].Titulo.FaturaParcela.Fatura.Codigo)
            //            {
            //                codigoFatura = 0;
            //                break;
            //            }

            //        }
            //    }
            //}

            if (codigosFaturas.Count > 1)
                return 0;
            else
                return codigosFaturas.FirstOrDefault();
        }

        public int NumeroFaturaBaixaAReceber(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigo && o.Titulo.FaturaParcela != null);

            List<int> codigosFaturas = query.Select(o => o.Titulo.FaturaParcela.Fatura.Numero).Distinct().ToList();

            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            //var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            //int numeroFatura = 0;
            //if (result.Count() > 0)
            //{
            //    List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> listaTitulos = result.ToList();
            //    for (int i = 0; i < listaTitulos.Count(); i++)
            //    {
            //        if (listaTitulos[i].Titulo.FaturaParcela != null && listaTitulos[i].Titulo.FaturaParcela.Fatura != null)
            //        {
            //            if (numeroFatura == 0)
            //                numeroFatura = listaTitulos[i].Titulo.FaturaParcela.Fatura.Numero;
            //            else if (numeroFatura != listaTitulos[i].Titulo.FaturaParcela.Fatura.Numero)
            //            {
            //                numeroFatura = 0;
            //                break;
            //            }

            //        }
            //    }
            //}

            if (codigosFaturas.Count > 1)
                return 0;
            else
                return codigosFaturas.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixa BuscarPorTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var result = from obj in query
                         where obj.Titulo.Codigo == codigo && obj.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada
                         select obj;
            return result.Select(obj => obj.TituloBaixa).FirstOrDefault();
        }

        public bool ExisteAtivaPorTitulo(int codigoTitulo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(o => o.Titulo.Codigo == codigoTitulo && o.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixa BuscarPorFatura(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var result = from obj in query where (obj.Titulo.FaturaParcela.Fatura.Codigo == codigo || obj.Titulo.FaturaCargaDocumento.Fatura.Codigo == codigo) && obj.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada select obj;
            return result.Select(obj => obj.TituloBaixa).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> RetornarrPorFatura(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var result = from obj in query where (obj.Titulo.FaturaParcela.Fatura.Codigo == codigo || obj.Titulo.FaturaCargaDocumento.Fatura.Codigo == codigo) && obj.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada select obj;
            return result.Select(obj => obj.TituloBaixa).ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixa BuscarPorBordero(int codigoBordero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();

            query = query.Where(o => o.Bordero.Codigo == codigoBordero);

            return query.FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo EtapaDoTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var result = from obj in query where obj.Titulo.Codigo == codigo select obj;
            if (result.Count() > 0)
                return result.Select(obj => obj.TituloBaixa).FirstOrDefault().SituacaoBaixaTitulo;
            else
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Iniciada;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> ConsultaBaixaReceber(DateTime dataBaseInicial, DateTime dataBaseFinal, int codigoConhecimento, int codigoEmpresa, int codigoTitulo, int numeroFatura, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo etapaBaixa, int codigoGrupoPessoa, double codigoCliente, int codigoOperador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaTituloBaixa = ConsultaBaixaReceber(dataBaseInicial, dataBaseFinal, codigoConhecimento, codigoEmpresa, codigoTitulo, numeroFatura, dataInicial, dataFinal, etapaBaixa, codigoGrupoPessoa, codigoCliente, codigoOperador, tipoServico, tipoAmbiente);

            return ObterLista(consultaTituloBaixa, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContaConsultaBaixaReceber(DateTime dataBaseInicial, DateTime dataBaseFinal, int codigoConhecimento, int codigoEmpresa, int codigoTitulo, int numeroFatura, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo etapaBaixa, int codigoGrupoPessoa, double codigoCliente, int codigoOperador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var consultaTituloBaixa = ConsultaBaixaReceber(dataBaseInicial, dataBaseFinal, codigoConhecimento, codigoEmpresa, codigoTitulo, numeroFatura, dataInicial, dataFinal, etapaBaixa, codigoGrupoPessoa, codigoCliente, codigoOperador, tipoServico, tipoAmbiente);

            return consultaTituloBaixa.Count();
        }

        public int ContarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();

            query = query.Where(o => o.Titulo.DuplicataDocumentoEntrada.DocumentoEntrada.Codigo == codigoDocumentoEntrada && o.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada);

            return query.Count();
        }

        public int ContarGuiaPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();

            query = query.Where(o => o.Titulo.DocumentoEntradaGuia.DocumentoEntrada.Codigo == codigoDocumentoEntrada && o.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> ConsultaBaixaPagar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoBaixa tipoBaixa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, decimal valorInicial, decimal valorFinal, int codigoEmpresa, int codigoTitulo, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo etapaBaixa, double codigoCliente, int codigoOperador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string numeroDocumentoOriginario, int codigoGrupoPessoa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaTituloBaixa = ConsultaBaixaPagar(tipoBaixa, dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal, valorInicial, valorFinal, codigoEmpresa, codigoTitulo, dataInicial, dataFinal, etapaBaixa, codigoCliente, codigoOperador, tipoServico, tipoAmbiente, numeroDocumentoOriginario, codigoGrupoPessoa);

            return ObterLista(consultaTituloBaixa, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContaConsultaBaixaPagar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoBaixa tipoBaixa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, decimal valorInicial, decimal valorFinal, int codigoEmpresa, int codigoTitulo, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo etapaBaixa, double codigoCliente, int codigoOperador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string numeroDocumentoOriginario, int codigoGrupoPessoa)
        {
            var consultaTituloBaixa = ConsultaBaixaPagar(tipoBaixa, dataEmissaoInicial, dataEmissaoFinal, dataVencimentoInicial, dataVencimentoFinal, valorInicial, valorFinal, codigoEmpresa, codigoTitulo, dataInicial, dataFinal, etapaBaixa, codigoCliente, codigoOperador, tipoServico, tipoAmbiente, numeroDocumentoOriginario, codigoGrupoPessoa);

            return consultaTituloBaixa.Count();
        }

        public dynamic ConsultaBaixaReceberNovo(int codigoCTe, int codigoCarga, int codigoEmpresa, int codigoTitulo, int numeroFatura, int codigoTipoPagamento, DateTime dataInicial, DateTime dataFinal, DateTime dataBaseInicial, DateTime dataBaseFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo? situacaoBaixaTitulo, int codigoGrupoPessoa, double codigoPessoa, int codigoOperador, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, int numeroDocOriginario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> query = ObterQueryConsultaBaixaTituloReceberNovo(codigoCTe, codigoCarga, codigoEmpresa, codigoTitulo, numeroFatura, codigoTipoPagamento, dataInicial, dataFinal, dataBaseInicial, dataBaseFinal, situacaoBaixaTitulo, codigoGrupoPessoa, codigoPessoa, codigoOperador, numeroDocOriginario);

            return query.Select(p => new
            {
                p.Codigo,
                CodigosTitulos = p.CodigosTitulos,
                NumeroFaturas = p.NumeroFaturas,
                GrupoPessoa = p.GrupoPessoas.Descricao ?? p.Pessoa.GrupoPessoas.Descricao ?? string.Empty,
                NumeroCargas = p.NumeroCargas,
                Situacao = p.SituacaoBaixaTitulo.ObterDescricao(),//p.DescricaoSituacaoBaixaTitulo,
                DataBase = p.DataBase.HasValue ? p.DataBase.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataBaixa = p.DataBaixa.HasValue ? p.DataBaixa.Value.ToString("dd/MM/yyyy") : string.Empty,
                ValorOriginal = p.ValorOriginal > 0 ? p.ValorOriginal.ToString("n2") : "0,00",
                Valor = p.Valor > 0 ? p.Valor.ToString("n2") : "0,00"
            }).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsultaBaixaReceberNovo(int codigoCTe, int codigoCarga, int codigoEmpresa, int codigoTitulo, int numeroFatura, int codigoTipoPagamento, DateTime dataInicial, DateTime dataFinal, DateTime dataBaseInicial, DateTime dataBaseFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo? situacaoBaixaTitulo, int codigoGrupoPessoa, double codigoPessoa, int codigoOperador, int numeroDocOriginario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> query = ObterQueryConsultaBaixaTituloReceberNovo(codigoCTe, codigoCarga, codigoEmpresa, codigoTitulo, numeroFatura, codigoTipoPagamento, dataInicial, dataFinal, dataBaseInicial, dataBaseFinal, situacaoBaixaTitulo, codigoGrupoPessoa, codigoPessoa, codigoOperador, numeroDocOriginario);

            return query.Count();
        }

        public decimal ObterValorTotalTitulosPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(obj => obj.TituloBaixa.Codigo == codigoTituloBaixa);

            return query.Sum(o => (decimal?)o.Titulo.ValorOriginal) ?? 0m;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento> ObterTitulosComDataCancelamentoSuperiorABaixa(int codigoTituloBaixa)
        {
            var sqlQuery = @"select Titulo.TIT_CODIGO Codigo, Titulo.TIT_DATA_CANCELAMENTO DataCancelamento from t_titulo_baixa TituloBaixa 
                             inner join t_titulo_baixa_agrupado TituloBaixaAgrupado on TituloBaixa.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO
                             inner join t_titulo_baixa_agrupado_documento TituloBaixaAgrupadoDocumento on TituloBaixaAgrupadoDocumento.TIA_CODIGO = TituloBaixaAgrupado.TIA_CODIGO
                             inner join t_titulo_documento TituloDocumentoTituloBaixaAgrupadoDocumento on TituloDocumentoTituloBaixaAgrupadoDocumento.TDO_CODIGO = TituloBaixaAgrupadoDocumento.TDO_CODIGO
                             inner join t_titulo_documento TituloDocumento on (TituloDocumentoTituloBaixaAgrupadoDocumento.CON_CODIGO is not null and TituloDocumentoTituloBaixaAgrupadoDocumento.CON_CODIGO = TituloDocumento.CON_CODIGO) or (TituloDocumentoTituloBaixaAgrupadoDocumento.CAR_CODIGO is not null and TituloDocumentoTituloBaixaAgrupadoDocumento.CAR_CODIGO = TituloDocumento.CAR_CODIGO)
                             inner join t_titulo Titulo on TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO
                             where TituloBaixa.TIB_CODIGO = " + codigoTituloBaixa.ToString() + " and (TituloBaixaAgrupadoDocumento.TBD_VALOR_PAGO - TituloBaixaAgrupadoDocumento.TBD_VALOR_DESCONTO + TituloBaixaAgrupadoDocumento.TBD_VALOR_ACRESCIMO) <> TituloBaixaAgrupadoDocumento.TBD_VALOR_TOTAL_A_PAGAR and Titulo.TIT_STATUS = 4 and Titulo.TIT_TIPO = 1 and Titulo.TIT_DATA_CANCELAMENTO > TituloBaixa.TIB_DATA_BAIXA";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento)));

            return query.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento>();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> BuscarPorSituacao(SituacaoBaixaTitulo situacao, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();

            query = query.Where(o => o.SituacaoBaixaTitulo == situacao && !o.ModeloAntigo);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public List<int> BuscarCodigosTitulosPendentesGeracao(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();

            query = query.Where(o => o.Codigo == codigoTituloBaixa);

            return query.Select(o => o.TitulosPendentesGeracao.Select(t => t.Codigo)).SelectMany(o => o).ToList();
        }

        public bool DocumentosContemPagamento(int codigoTituloBaixa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> queryDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            queryCTe = queryCTe.Where(o => o.Titulo.Baixas.Any(b => b.TituloBaixa.Codigo == codigoTituloBaixa) && o.CTe != null);
            queryCarga = queryCarga.Where(o => o.Titulo.Baixas.Any(b => b.TituloBaixa.Codigo == codigoTituloBaixa) && o.Carga != null);

            queryDocumentoFaturamento = queryDocumentoFaturamento.Where(o => o.TipoLiquidacao == TipoLiquidacao.PagamentoTransportador && o.Pagamento != null &&
                                                                            ((o.TipoDocumento == TipoDocumentoFaturamento.CTe && queryCTe.Any(c => c.CTe == o.CTe)) ||
                                                                             (o.TipoDocumento == TipoDocumentoFaturamento.Carga && queryCarga.Any(c => c.Carga == o.Carga))));

            return queryDocumentoFaturamento.Any();
        }

        public bool ContemTitulosComPagamentoEletronico(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;

            var queryPagamentoEletronico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
            result = result.Where(o => queryPagamentoEletronico.Where(a => a.Titulo.Codigo == o.Titulo.Codigo).Any());

            return result.Count() > 0;
        }

        public bool ContemTitulosNegociadosEmOutraBaixa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.TituloBaixaNegociacao.TituloBaixa.Codigo == codigo select obj;

            var queryPagamentoEletronico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();
            result = result.Where(o => queryPagamentoEletronico.Where(a => a.Titulo.Codigo == o.Codigo && a.TituloBaixa.SituacaoBaixaTitulo != SituacaoBaixaTitulo.Cancelada).Any());

            return result.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> ObterQueryConsultaBaixaTituloReceberNovo(int codigoCTe, int codigoCarga, int codigoEmpresa, int codigoTitulo, int numeroFatura, int codigoTipoPagamento, DateTime dataInicial, DateTime dataFinal, DateTime dataBaseInicial, DateTime dataBaseFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo? situacaoBaixaTitulo, int codigoGrupoPessoa, double codigoPessoa, int codigoOperador, int numeroDocOriginario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>();
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> subQueryTitulos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            query = query.Where(obj => obj.TipoBaixaTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber && !obj.ModeloAntigo);

            subQueryTitulos = subQueryTitulos.Where(o => o.Titulo.Documentos.Count > 0);

            if (codigoTitulo > 0 || numeroFatura > 0 || codigoEmpresa > 0 || codigoCTe > 0 || codigoCarga > 0 || numeroDocOriginario > 0)
            {
                if (codigoTitulo > 0)
                    subQueryTitulos = subQueryTitulos.Where(o => o.Titulo.Codigo == codigoTitulo);

                if (numeroFatura > 0)
                    subQueryTitulos = subQueryTitulos.Where(obj => obj.Titulo.FaturaParcela.Fatura.Numero == numeroFatura);

                if (codigoEmpresa > 0)
                    subQueryTitulos = subQueryTitulos.Where(obj => obj.Titulo.Empresa.Codigo == codigoEmpresa || obj.Titulo.Empresa == null);

                if (codigoCTe > 0)
                    subQueryTitulos = subQueryTitulos.Where(o => o.Titulo.Documentos.Any(d => d.CTe.Codigo == codigoCTe));

                if (codigoCarga > 0)
                    subQueryTitulos = subQueryTitulos.Where(o => o.Titulo.Documentos.Any(d => d.Carga.Codigo == codigoCarga));

                if (numeroDocOriginario > 0)
                    subQueryTitulos = subQueryTitulos.Where(o => o.Titulo.Documentos.Any(d => d.CTe.DocumentosOriginarios.Any(dor => dor.Numero == numeroDocOriginario)));

                query = query.Where(o => subQueryTitulos.Select(t => t.TituloBaixa.Codigo).Contains(o.Codigo));
            }
            //else
            //{
            //    query = query.Where(o => o.SituacaoBaixaTitulo == SituacaoBaixaTitulo.EmGeracao || subQueryTitulos.Select(t => t.TituloBaixa.Codigo).Contains(o.Codigo));
            //}

            if (situacaoBaixaTitulo.HasValue)
                query = query.Where(obj => obj.SituacaoBaixaTitulo == situacaoBaixaTitulo.Value);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(obj => obj.DataBaixa >= dataInicial);
            if (dataFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataBaixa.Value.Date <= dataFinal.Date);

            if (dataBaseInicial != DateTime.MinValue)
                query = query.Where(obj => obj.DataBase >= dataBaseInicial);
            if (dataBaseFinal != DateTime.MinValue)
                query = query.Where(obj => obj.DataBase.Value.Date <= dataBaseFinal.Date);

            if (codigoPessoa > 0)
                query = query.Where(obj => obj.Pessoa.CPF_CNPJ == codigoPessoa);

            if (codigoGrupoPessoa > 0)
                query = query.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupoPessoa);

            if (codigoOperador > 0)
                query = query.Where(obj => obj.Usuario.Codigo == codigoOperador);

            if (codigoTipoPagamento > 0)
                query = query.Where(o => o.TipoPagamentoRecebimento.Codigo == codigoTipoPagamento);

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> ConsultaBaixaPagar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoBaixa tipoBaixa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, decimal valorInicial, decimal valorFinal, int codigoEmpresa, int codigoTitulo, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo etapaBaixa, double codigoCliente, int codigoOperador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string numeroDocumentoOriginario, int codigoGrupoPessoa)
        {
            var consultaTituloBaixaAgrupado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>()
                .Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.TipoTitulo == TipoTitulo.Pagar);

            if (!string.IsNullOrWhiteSpace(numeroDocumentoOriginario))
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.NumeroDocumentoTituloOriginal.Contains(numeroDocumentoOriginario));

            if (codigoTitulo > 0)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.Codigo == codigoTitulo);

            if (dataEmissaoInicial > DateTime.MinValue && dataEmissaoFinal > DateTime.MinValue)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.DataEmissao >= dataEmissaoInicial && tituloBaixaAgrupado.Titulo.DataEmissao.Value.Date <= dataEmissaoFinal.Date);
            if (dataEmissaoInicial > DateTime.MinValue && dataEmissaoFinal == DateTime.MinValue)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.DataEmissao >= dataEmissaoInicial);
            if (dataEmissaoInicial == DateTime.MinValue && dataEmissaoFinal > DateTime.MinValue)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.DataEmissao.Value.Date <= dataEmissaoFinal.Date);

            if (dataVencimentoInicial > DateTime.MinValue && dataVencimentoFinal > DateTime.MinValue)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.DataVencimento >= dataVencimentoInicial && tituloBaixaAgrupado.Titulo.DataVencimento.Value.Date <= dataVencimentoFinal.Date);
            if (dataVencimentoInicial > DateTime.MinValue && dataVencimentoFinal == DateTime.MinValue)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.DataVencimento >= dataVencimentoInicial);
            if (dataVencimentoInicial == DateTime.MinValue && dataVencimentoFinal > DateTime.MinValue)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.DataVencimento.Value.Date <= dataVencimentoFinal.Date);

            if (valorInicial > 0 && valorFinal > 0)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.ValorOriginal >= valorInicial && tituloBaixaAgrupado.Titulo.ValorOriginal <= valorFinal);
            if (valorInicial > 0)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.ValorOriginal >= valorInicial);
            if (valorFinal > 0)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.ValorOriginal <= valorFinal);

            if ((tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe) && (codigoEmpresa > 0))
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.Empresa.Codigo == codigoEmpresa);

            if (tipoAmbiente > 0)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.TipoAmbiente == tipoAmbiente);

            var consultaTituloBaixa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>()
                .Where(tituloBaixa => consultaTituloBaixaAgrupado.Any(tituloBaixaAgrupado => tituloBaixaAgrupado.TituloBaixa.Codigo == tituloBaixa.Codigo));
                
            if(tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.ModeloAntigo == true);

            if (tipoBaixa != TipoBaixa.Todos)
            {
                var consultaTituloBaixaNegociacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaNegociacao>();

                if (tipoBaixa == TipoBaixa.ComNegociacao)
                    consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => consultaTituloBaixaNegociacao.Any(negociacao => negociacao.TituloBaixa.Codigo == tituloBaixa.Codigo));
                else if (tipoBaixa == TipoBaixa.SomenteBaixa)
                    consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => !consultaTituloBaixaNegociacao.Any(negociacao => negociacao.TituloBaixa.Codigo == tituloBaixa.Codigo));
            }

            if (etapaBaixa > 0)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.SituacaoBaixaTitulo == etapaBaixa);

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.DataBaixa >= dataInicial && tituloBaixa.DataBaixa.Value.Date <= dataFinal.Date);
            if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.DataBaixa >= dataInicial);
            if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.DataBaixa.Value.Date <= dataFinal.Date);

            if (codigoCliente > 0)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.Pessoa.CPF_CNPJ == codigoCliente);

            if (codigoOperador > 0)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.Usuario.Codigo == codigoOperador);

            if (codigoGrupoPessoa > 0)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.GrupoPessoas.Codigo == codigoGrupoPessoa);

            return consultaTituloBaixa;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> ConsultaBaixaReceber(DateTime dataBaseInicial, DateTime dataBaseFinal, int codigoConhecimento, int codigoEmpresa, int codigoTitulo, int numeroFatura, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo etapaBaixa, int codigoGrupoPessoa, double codigoCliente, int codigoOperador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var consultaTituloBaixaAgrupado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>()
                .Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.TipoTitulo == TipoTitulo.Receber);

            if (codigoTitulo > 0)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.Codigo == codigoTitulo);

            if (numeroFatura > 0)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.FaturaParcela.Fatura.Numero == numeroFatura);

            if ((tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe) && (codigoEmpresa > 0))
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.Empresa.Codigo == codigoEmpresa);

            if (codigoConhecimento > 0)
            {
                if (tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    var consultaFaturaCargaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>()
                        .Where(faturaDocumento => faturaDocumento.ConhecimentoDeTransporteEletronico.Codigo == codigoConhecimento && faturaDocumento.StatusDocumentoFatura == StatusDocumentoFatura.Normal);

                    consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado =>
                        consultaFaturaCargaDocumento.Any(faturaDocumento => faturaDocumento.Fatura.Codigo == tituloBaixaAgrupado.Titulo.FaturaParcela.Fatura.Codigo) ||
                        tituloBaixaAgrupado.Titulo.Documentos.Any(tituloDocumento => tituloDocumento.CTe.Codigo == codigoConhecimento) ||
                        tituloBaixaAgrupado.Titulo.ConhecimentoDeTransporteEletronico.Codigo == codigoConhecimento
                    );
                }
                else
                {
                    var consultaFaturaCargaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>()
                        .Where(faturaDocumento => faturaDocumento.ConhecimentoDeTransporteEletronico.Codigo == codigoConhecimento && faturaDocumento.StatusDocumentoFatura == StatusDocumentoFatura.Normal && faturaDocumento.Fatura.Situacao != SituacaoFatura.Cancelado);

                    consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => consultaFaturaCargaDocumento.Any(faturaDocumento => faturaDocumento.Fatura.Codigo == tituloBaixaAgrupado.Titulo.FaturaParcela.Fatura.Codigo));
                }
            }

            if (tipoAmbiente > 0)
                consultaTituloBaixaAgrupado = consultaTituloBaixaAgrupado.Where(tituloBaixaAgrupado => tituloBaixaAgrupado.Titulo.TipoAmbiente == tipoAmbiente);

            var consultaTituloBaixa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa>()
                .Where(tituloBaixa => consultaTituloBaixaAgrupado.Any(tituloBaixaAgrupado => tituloBaixaAgrupado.TituloBaixa.Codigo == tituloBaixa.Codigo));

            if (tipoServico != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.ModeloAntigo == true);

            if (etapaBaixa > 0)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.SituacaoBaixaTitulo == etapaBaixa);

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.DataBaixa >= dataInicial && tituloBaixa.DataBaixa.Value.Date <= dataFinal.Date);
            if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.DataBaixa >= dataInicial);
            if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.DataBaixa.Value.Date <= dataFinal.Date);

            if (dataBaseInicial > DateTime.MinValue && dataBaseFinal > DateTime.MinValue)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.DataBase >= dataBaseInicial && tituloBaixa.DataBase.Value.Date <= dataBaseFinal.Date);
            if (dataBaseInicial > DateTime.MinValue && dataBaseFinal == DateTime.MinValue)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.DataBase >= dataBaseInicial);
            if (dataBaseInicial == DateTime.MinValue && dataBaseFinal > DateTime.MinValue)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.DataBase.Value.Date <= dataBaseFinal.Date);

            if (codigoCliente > 0)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.Pessoa.CPF_CNPJ == codigoCliente);

            if (codigoGrupoPessoa > 0)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.GrupoPessoas.Codigo == codigoGrupoPessoa);

            if (codigoOperador > 0)
                consultaTituloBaixa = consultaTituloBaixa.Where(tituloBaixa => tituloBaixa.Usuario.Codigo == codigoOperador);

            return consultaTituloBaixa;
        }

        #endregion

        #region Relatório Baixa de Títulos

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.BaixaTitulo> ConsultarRelatorioBaixaTitulo(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int tipoPagamentoRecebimento, TipoTitulo tipo, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioBaixaTitulo(codigoEmpresa, dataInicial, dataFinal, tipoPagamentoRecebimento, tipo, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.BaixaTitulo)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.BaixaTitulo>();
        }

        public int ContarConsultaRelatorioBaixaTitulo(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int tipoPagamentoRecebimento, TipoTitulo tipo, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioBaixaTitulo(codigoEmpresa, dataInicial, dataFinal, tipoPagamentoRecebimento, tipo, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioBaixaTitulo(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int tipoPagamentoRecebimento, TipoTitulo tipo, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioBaixaTitulo(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioBaixaTitulo(ref where, ref groupBy, ref joins, codigoEmpresa, dataInicial, dataFinal, tipoPagamentoRecebimento, tipo);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioBaixaTitulo(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_TITULO Titulo ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioBaixaTitulo(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "Titulo.TIT_CODIGO Codigo, ";
                        groupBy += "Titulo.TIT_CODIGO, ";
                    }
                    break;
                case "DataBaixa":
                    if (!select.Contains(" DataBaixa, "))
                    {
                        if (!joins.Contains(" TituloBaixaAgrupado "))
                            joins += " JOIN T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado ON TituloBaixaAgrupado.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!joins.Contains(" TituloBaixa "))
                            joins += " JOIN T_TITULO_BAIXA TituloBaixa ON TituloBaixa.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO";

                        select += "TituloBaixa.TIB_DATA_BAIXA DataBaixa, ";
                        groupBy += "TituloBaixa.TIB_DATA_BAIXA, ";
                    }
                    break;
                case "CPFCNPJPessoaFormatado":
                    if (!select.Contains(" CNPJCPFPessoa, "))
                    {
                        if (!joins.Contains(" Pessoa "))
                            joins += " JOIN T_CLIENTE Pessoa ON Pessoa.CLI_CGCCPF = Titulo.CLI_CGCCPF";

                        select += "Titulo.CLI_CGCCPF CNPJCPFPessoa, Pessoa.CLI_FISJUR TipoPessoa, ";
                        groupBy += "Titulo.CLI_CGCCPF, Pessoa.CLI_FISJUR, ";
                    }
                    break;
                case "RazaoPessoa":
                    if (!select.Contains(" RazaoPessoa, "))
                    {
                        if (!joins.Contains(" Pessoa "))
                            joins += " JOIN T_CLIENTE Pessoa ON Pessoa.CLI_CGCCPF = Titulo.CLI_CGCCPF";

                        select += "Pessoa.CLI_NOME RazaoPessoa, ";
                        groupBy += "Pessoa.CLI_NOME, ";
                    }
                    break;
                case "NumeroDocumentoOriginal":
                    if (!select.Contains(" NumeroDocumentoOriginal, "))
                    {
                        select += "Titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumentoOriginal, ";
                        groupBy += "Titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL, ";
                    }
                    break;
                case "TipoDocumentoOriginal":
                    if (!select.Contains(" TipoDocumentoOriginal, "))
                    {
                        select += "Titulo.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL TipoDocumentoOriginal, ";
                        groupBy += "Titulo.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL, ";
                    }
                    break;
                case "ValorOriginal":
                    if (!select.Contains(" ValorOriginal, "))
                    {
                        select += "SUM(Titulo.TIT_VALOR_ORIGINAL) ValorOriginal, ";
                    }
                    break;
                case "ValorBaixa":
                    if (!select.Contains(" ValorBaixa, "))
                    {
                        if (!joins.Contains(" TituloBaixaAgrupado "))
                            joins += " JOIN T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado ON TituloBaixaAgrupado.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!joins.Contains(" TituloBaixa "))
                            joins += " JOIN T_TITULO_BAIXA TituloBaixa ON TituloBaixa.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO";

                        if (!joins.Contains(" Pagamentos "))
                            joins += " LEFT OUTER JOIN T_TITULO_BAIXA_TIPO_PAGAMENTO_RECEBIMENTO Pagamentos on Pagamentos.TIB_CODIGO = TituloBaixa.TIB_CODIGO";

                        select += @"CASE
                                        WHEN Pagamentos.TTP_VALOR > 0 AND Pagamentos.TTP_VALOR IS NOT NULL
	                                        THEN Pagamentos.TTP_VALOR
                                        WHEN ((select count(*) from T_TITULO_BAIXA_AGRUPADO tba where tba.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO) > 0)
				                            THEN SUM(TituloBaixa.TIB_VALOR) / (select count(*) from T_TITULO_BAIXA_AGRUPADO tba where tba.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO)
                                        ELSE SUM(TituloBaixa.TIB_VALOR)
                                    END ValorBaixa, ";

                        groupBy += "Pagamentos.TTP_VALOR, ";
                        groupBy += "TituloBaixaAgrupado.TIB_CODIGO, ";
                    }
                    break;
                case "ValorAcrescimoBaixa":
                    if (!select.Contains(" ValorAcrescimoBaixa, "))
                    {
                        if (!joins.Contains(" TituloBaixaAgrupado "))
                            joins += " JOIN T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado ON TituloBaixaAgrupado.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!joins.Contains(" TituloBaixa "))
                            joins += " JOIN T_TITULO_BAIXA TituloBaixa ON TituloBaixa.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO";

                        select += "SUM(ISNULL(TituloBaixa.TIB_VALOR_ACRESCIMO, 0)) ValorAcrescimoBaixa, ";
                    }
                    break;
                case "ValorDescontoBaixa":
                    if (!select.Contains(" ValorDescontoBaixa, "))
                    {
                        if (!joins.Contains(" TituloBaixaAgrupado "))
                            joins += " JOIN T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado ON TituloBaixaAgrupado.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!joins.Contains(" TituloBaixa "))
                            joins += " JOIN T_TITULO_BAIXA TituloBaixa ON TituloBaixa.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO";

                        select += "SUM(ISNULL(TituloBaixa.TIB_VALOR_DESCONTO, 0)) ValorDescontoBaixa, ";
                    }
                    break;
                case "PlanoCredito":
                    if (!select.Contains(" PlanoCredito, "))
                    {
                        if (!joins.Contains(" TipoMovimento "))
                            joins += " JOIN T_TIPO_MOVIMENTO TipoMovimento ON TipoMovimento.TIM_CODIGO = Titulo.TIM_CODIGO";

                        if (!joins.Contains(" PlanoCredito "))
                            joins += " JOIN T_PLANO_DE_CONTA PlanoCredito ON PlanoCredito.PLA_CODIGO = TipoMovimento.PLA_CODIGO_CREDITO";

                        select += "PlanoCredito.PLA_PLANO PlanoCredito, ";
                        groupBy += "PlanoCredito.PLA_PLANO, ";
                    }
                    break;
                case "DescricaoPlanoCredito":
                    if (!select.Contains(" DescricaoPlanoCredito, "))
                    {
                        if (!joins.Contains(" TipoMovimento "))
                            joins += " JOIN T_TIPO_MOVIMENTO TipoMovimento ON TipoMovimento.TIM_CODIGO = Titulo.TIM_CODIGO";

                        if (!joins.Contains(" PlanoCredito "))
                            joins += " JOIN T_PLANO_DE_CONTA PlanoCredito ON PlanoCredito.PLA_CODIGO = TipoMovimento.PLA_CODIGO_CREDITO";

                        select += "PlanoCredito.PLA_DESCRICAO DescricaoPlanoCredito, ";
                        groupBy += "PlanoCredito.PLA_DESCRICAO, ";
                    }
                    break;
                case "PlanoDebito":
                    if (!select.Contains(" PlanoDebito, "))
                    {
                        if (!joins.Contains(" TipoMovimento "))
                            joins += " JOIN T_TIPO_MOVIMENTO TipoMovimento ON TipoMovimento.TIM_CODIGO = Titulo.TIM_CODIGO";

                        if (!joins.Contains(" PlanoDebito "))
                            joins += " JOIN T_PLANO_DE_CONTA PlanoDebito ON PlanoDebito.PLA_CODIGO = TipoMovimento.PLA_CODIGO_DEBITO";

                        select += "PlanoDebito.PLA_PLANO PlanoDebito, ";
                        groupBy += "PlanoDebito.PLA_PLANO, ";
                    }
                    break;
                case "DescricaoPlanoDebito":
                    if (!select.Contains(" DescricaoPlanoDebito, "))
                    {
                        if (!joins.Contains(" TipoMovimento "))
                            joins += " JOIN T_TIPO_MOVIMENTO TipoMovimento ON TipoMovimento.TIM_CODIGO = Titulo.TIM_CODIGO";

                        if (!joins.Contains(" PlanoDebito "))
                            joins += " JOIN T_PLANO_DE_CONTA PlanoDebito ON PlanoDebito.PLA_CODIGO = TipoMovimento.PLA_CODIGO_DEBITO";

                        select += "PlanoDebito.PLA_DESCRICAO DescricaoPlanoDebito, ";
                        groupBy += "PlanoDebito.PLA_DESCRICAO, ";
                    }
                    break;
                case "PlanoConta":
                    if (!select.Contains(" PlanoConta, "))
                    {
                        if (!joins.Contains(" TituloBaixaAgrupado "))
                            joins += " JOIN T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado ON TituloBaixaAgrupado.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!joins.Contains(" TituloBaixa "))
                            joins += " JOIN T_TITULO_BAIXA TituloBaixa ON TituloBaixa.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO";

                        if (!joins.Contains(" PagamentoRecebimento "))
                            joins += " LEFT OUTER JOIN T_TIPO_PAGAMENTO_RECEBIMENTO PagamentoRecebimento ON PagamentoRecebimento.TPR_CODIGO = TituloBaixa.TPR_CODIGO";

                        if (!joins.Contains(" PlanoConta "))
                            joins += " LEFT OUTER JOIN T_PLANO_DE_CONTA PlanoConta ON PlanoConta.PLA_CODIGO = PagamentoRecebimento.PLA_CODIGO";

                        if (!joins.Contains(" Pagamentos "))
                            joins += " LEFT OUTER JOIN T_TITULO_BAIXA_TIPO_PAGAMENTO_RECEBIMENTO Pagamentos on Pagamentos.TIB_CODIGO = TituloBaixa.TIB_CODIGO";

                        if (!joins.Contains(" PRPagamentos "))
                            joins += " LEFT OUTER JOIN T_TIPO_PAGAMENTO_RECEBIMENTO PRPagamentos ON PRPagamentos.TPR_CODIGO = Pagamentos.TPR_CODIGO";

                        if (!joins.Contains(" CONTAPagamentos "))
                            joins += " LEFT OUTER JOIN T_PLANO_DE_CONTA CONTAPagamentos ON CONTAPagamentos.PLA_CODIGO = PRPagamentos.PLA_CODIGO";

                        select += "ISNULL(PlanoConta.PLA_PLANO, CONTAPagamentos.PLA_PLANO) PlanoConta, ";
                        groupBy += "PlanoConta.PLA_PLANO, CONTAPagamentos.PLA_PLANO, ";
                    }
                    break;
                case "DescricaoPlanoConta":
                    if (!select.Contains(" DescricaoPlanoConta, "))
                    {
                        if (!joins.Contains(" TituloBaixaAgrupado "))
                            joins += " JOIN T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado ON TituloBaixaAgrupado.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!joins.Contains(" TituloBaixa "))
                            joins += " JOIN T_TITULO_BAIXA TituloBaixa ON TituloBaixa.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO";

                        if (!joins.Contains(" PagamentoRecebimento "))
                            joins += " LEFT OUTER JOIN T_TIPO_PAGAMENTO_RECEBIMENTO PagamentoRecebimento ON PagamentoRecebimento.TPR_CODIGO = TituloBaixa.TPR_CODIGO";

                        if (!joins.Contains(" PlanoConta "))
                            joins += " LEFT OUTER JOIN T_PLANO_DE_CONTA PlanoConta ON PlanoConta.PLA_CODIGO = PagamentoRecebimento.PLA_CODIGO";

                        if (!joins.Contains(" Pagamentos "))
                            joins += " LEFT OUTER JOIN T_TITULO_BAIXA_TIPO_PAGAMENTO_RECEBIMENTO Pagamentos on Pagamentos.TIB_CODIGO = TituloBaixa.TIB_CODIGO";

                        if (!joins.Contains(" PRPagamentos "))
                            joins += " LEFT OUTER JOIN T_TIPO_PAGAMENTO_RECEBIMENTO PRPagamentos ON PRPagamentos.TPR_CODIGO = Pagamentos.TPR_CODIGO";

                        if (!joins.Contains(" CONTAPagamentos "))
                            joins += " LEFT OUTER JOIN T_PLANO_DE_CONTA CONTAPagamentos ON CONTAPagamentos.PLA_CODIGO = PRPagamentos.PLA_CODIGO";

                        select += "ISNULL(PlanoConta.PLA_DESCRICAO, CONTAPagamentos.PLA_DESCRICAO) DescricaoPlanoConta, ";
                        groupBy += "PlanoConta.PLA_DESCRICAO, CONTAPagamentos.PLA_DESCRICAO, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        if (!joins.Contains(" TituloBaixaAgrupado "))
                            joins += " JOIN T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado ON TituloBaixaAgrupado.TIT_CODIGO = Titulo.TIT_CODIGO";

                        if (!joins.Contains(" TituloBaixa "))
                            joins += " JOIN T_TITULO_BAIXA TituloBaixa ON TituloBaixa.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO";

                        select += "ISNULL(Titulo.TIT_OBSERVACAO, '') + ' ' + ' ' + ISNULL(TituloBaixa.TIB_OBSERVACAO, '') Observacao, ";
                        groupBy += "Titulo.TIT_OBSERVACAO, TituloBaixa.TIB_OBSERVACAO, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioBaixaTitulo(ref string where, ref string groupBy, ref string joins, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int tipoPagamentoRecebimento, TipoTitulo tipo)
        {
            string pattern = "yyyy-MM-dd";

            if (!joins.Contains(" TituloBaixaAgrupado "))
                joins += " JOIN T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado ON TituloBaixaAgrupado.TIT_CODIGO = Titulo.TIT_CODIGO";

            if (!joins.Contains(" TituloBaixa "))
                joins += " JOIN T_TITULO_BAIXA TituloBaixa ON TituloBaixa.TIB_CODIGO = TituloBaixaAgrupado.TIB_CODIGO";

            if (codigoEmpresa > 0)
                where += " AND Titulo.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (dataInicial != DateTime.MinValue)
                where += " AND CAST(TituloBaixa.TIB_DATA_BAIXA AS DATE) >= '" + dataInicial.ToString(pattern) + "' ";

            if (dataFinal != DateTime.MinValue)
                where += " AND CAST(TituloBaixa.TIB_DATA_BAIXA AS DATE) <= '" + dataFinal.ToString(pattern) + "'";

            if (tipo > 0)
                where += " AND Titulo.TIT_TIPO = " + tipo.ToString("D");

            if (tipoPagamentoRecebimento > 0)
                where += " AND (TituloBaixa.TPR_CODIGO = " + tipoPagamentoRecebimento + " OR Pagamentos.TPR_CODIGO = " + tipoPagamentoRecebimento + ")";

            where += " AND Titulo.TIT_STATUS = 3"; //Somente Quitadas
            where += " AND Titulo.TIT_VALOR_PAGO > 0 ";
            where += " AND TituloBaixa.TIB_VALOR > 0 ";
        }

        #endregion
    }
}
