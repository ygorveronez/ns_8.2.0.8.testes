using Avro.Util;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class AutorizacaoPagamentoContratoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete>
    {
        public AutorizacaoPagamentoContratoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete>();

            return (query.Max(o => (int?)o.Numero) ?? 0) + 1;
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete> Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, int numero, int carga, int empresa, DateTime dataCiotInicial, DateTime dataCiotFinal, double transportadorTerceiro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT? operadoraCIOT, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(usuario, dataInicial, dataFinal, numero, carga, empresa, dataCiotInicial, dataCiotFinal, transportadorTerceiro, operadoraCIOT);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.FetchMany(q => q.ContratoFrete).ToList();
        }

        public int ContarConsulta(int usuario, DateTime dataInicial, DateTime dataFinal, int numero, int carga, int empresa, DateTime dataCiotInicial, DateTime dataCiotFinal, double transportadorTerceiro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT? operadoraCIOT)
        {
            var result = _Consultar(usuario, dataInicial, dataFinal, numero, carga, empresa, dataCiotInicial, dataCiotFinal, transportadorTerceiro, operadoraCIOT);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> ConsultarPorContratoFrete(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete? situacao, int numero, int carga, int empresa, int codigoAutorizacaoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPagamentoAutorizacaoPagamento? tipoPagamento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT? operadoraCIOT, double transportadorTerceiro)
        {
            var result = _ConsultarPorContratoFrete(usuario, dataInicial, dataFinal, situacao, numero, carga, empresa, codigoAutorizacaoPagamento, tipoPagamento, operadoraCIOT, transportadorTerceiro);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorContratoFrete(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete? situacao, int numero, int carga, int empresa, int codigoAutorizacaoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPagamentoAutorizacaoPagamento? tipoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT? operadoraCIOT, double transportadorTerceiro)
        {
            var result = _ConsultarPorContratoFrete(usuario, dataInicial, dataFinal, situacao, numero, carga, empresa, codigoAutorizacaoPagamento, tipoPagamento, operadoraCIOT, transportadorTerceiro);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete> _Consultar(int usuario, DateTime dataInicial, DateTime dataFinal, int numero, int carga, int empresa, DateTime dataCiotInicial, DateTime dataCiotFinal, double transportadorTerceiro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT? operadoraCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete>();

            var result = from obj in query select obj;

            // Filtros
            if (numero > 0)
                result = result.Where(obj => obj.ContratoFrete.Any(o => o.NumeroContrato == numero));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataCriacao.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataCriacao.Date <= dataFinal);

            if (empresa > 0)
                result = result.Where(obj => obj.ContratoFrete.Any(o => o.TransportadorTerceiro.Codigo == empresa));

            if (carga > 0)
                result = result.Where(obj => obj.ContratoFrete.Any(o => o.Carga.Codigo == carga));

            if (dataCiotInicial != DateTime.MinValue )
                result = result.Where(q => q.ContratoFrete.Any(c => c.Carga.CargaCIOTs.Any(x => x.ContratoFrete.Codigo == c.Codigo && x.CIOT != null && x.CIOT.DataAbertura != null && x.CIOT.DataAbertura.Value.Date >= dataCiotInicial.Date)));

            if (dataCiotFinal != DateTime.MinValue)
                result = result.Where(q => q.ContratoFrete.Any(c => c.Carga.CargaCIOTs.Any(x => x.ContratoFrete.Codigo == c.Codigo && x.CIOT != null && x.CIOT.DataAbertura != null && x.CIOT.DataAbertura.Value.Date <= dataCiotFinal.Date)));

            if (transportadorTerceiro > 0)
                result = result.Where(obj => obj.ContratoFrete.Any(o => o.TransportadorTerceiro.CPF_CNPJ == transportadorTerceiro));
           
            if (operadoraCIOT.HasValue)
                result = result.Where(obj => obj.ContratoFrete.Any(o=> o.Carga.CargaCIOTs.Any(x => x.CIOT.Operadora == operadoraCIOT && x.ContratoFrete.Codigo == o.Codigo)));

            // Filtros da autorizacao
            if (usuario > 0)
                result = result.Where(obj => obj.Usuario.Codigo == usuario);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> _ConsultarPorContratoFrete(int usuario, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete? situacao, int numero, int carga, int empresa, int codigoAutorizacaoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPagamentoAutorizacaoPagamento? tipoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT? operadoraCIOT, double transportadorTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            // Filtros
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> result = null;

            if (codigoAutorizacaoPagamento > 0)
            {
                var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete>()
                    .Where(obj => obj.Codigo == codigoAutorizacaoPagamento);

                result = queryAutorizacao
                    .SelectMany(obj => obj.ContratoFrete);
            }
            else
            {
                result = from obj in query select obj;

                if (numero > 0)
                    result = result.Where(obj => obj.NumeroContrato == numero);

                if (dataInicial != DateTime.MinValue)
                    result = result.Where(obj => obj.DataEmissaoContrato.Date >= dataInicial);

                if (dataFinal != DateTime.MinValue)
                    result = result.Where(obj => obj.DataEmissaoContrato.Date <= dataFinal);

                if (situacao.HasValue)
                    result = result.Where(obj => obj.SituacaoContratoFrete == situacao.Value);

                if (empresa > 0)
                    result = result.Where(obj => obj.TransportadorTerceiro.Codigo == empresa);

                if (carga > 0)
                    result = result.Where(obj => obj.Carga.Codigo == carga);

                if (transportadorTerceiro > 0)
                    result = result.Where(obj => obj.TransportadorTerceiro.CPF_CNPJ == transportadorTerceiro);

                if (tipoPagamento.HasValue && tipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento)
                    result = result.Where(obj => obj.DataAutorizacaoPagamentoAdiantamento == null && obj.ValorAdiantamento > 0);

                if (tipoPagamento.HasValue && tipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoSaldo)
                    result = result.Where(obj => obj.DataAutorizacaoPagamentoSaldo == null && (obj.ValorFreteSubcontratacao - obj.ValorAdiantamento + obj.ValorTotalAcrescimoSaldo - obj.ValorTotalDescontoSaldo) > 0);

                if (operadoraCIOT.HasValue)
                    result = result.Where(obj => obj.Carga.CargaCIOTs.Any(x => x.CIOT.Operadora == operadoraCIOT));
                
                var listaSituacoes = new List<SituacaoContratoFrete>() { SituacaoContratoFrete.Aberto, SituacaoContratoFrete.Aprovado, SituacaoContratoFrete.Finalizada };
                result = result.Where(obj => listaSituacoes.Contains(obj.SituacaoContratoFrete) && obj.DataAutorizacaoPagamento == null);
            }

            return result;
        }

        #endregion
    }
}