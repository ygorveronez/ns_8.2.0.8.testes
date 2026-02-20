using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escrituracao
{
    public class CancelamentoPagamento : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento>
    {

        public CancelamentoPagamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento BuscarSeExisteCancelamentoPagamentoEmFechamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.EmCancelamento || obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.PendenciaCancelamento select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento> BuscarCancelamentoPagamentoEmFechamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.EmCancelamento && obj.GerandoMovimentoFinanceiro select obj;

            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento> BuscarCancelamentoPagamentoAgIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.AgIntegracao select obj;

            return result.ToList();
        }

        public int ObterProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento>();

            var result = from obj in query select obj;

            int? retorno = result.Max(o => (int?)o.Numero);
            return retorno.HasValue ? (retorno.Value + 1) : 1;

        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento> BuscarPagamentosEmCancelamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.EmCancelamento && obj.GerandoMovimentoFinanceiro select obj;

            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento> _Consultar(DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int ocorrencia, int numero, int numeroDOC, int localidadePrestacao, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento? situacaoCancelamentoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento>();

            var result = from obj in query select obj;

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial.Value.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal.Value.Date <= dataFim);

            if (transportador > 0)
                result = result.Where(o => o.Empresa.Codigo == transportador);

            if (carga > 0)
                result = result.Where(o => o.DocumentosFaturamento.Any(doc => doc.CargaPagamento.Codigo == carga));

            if (numeroDOC > 0)
                result = result.Where(o => o.DocumentosFaturamento.Any(doc => doc.Numero == numeroDOC.ToString()));

            //if (ocorrencia > 0)
            //    result = result.Where(o => o.DocumentosFaturamento.Any(doc => doc.CargaOcorrenciaCancelamentoPagamento.Codigo == ocorrencia));

            if (filial > 0)
                result = result.Where(o => o.Filial.Codigo == filial);

            if (numero > 0)
                result = result.Where(o => o.Pagamentos.Any(pag => pag.Numero == numero));

            if (situacaoCancelamentoPagamento.HasValue)
                result = result.Where(o => o.Situacao == situacaoCancelamentoPagamento.Value);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento> Consultar(DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int ocorrencia, int numero, int numeroDOC, int localidadePrestacao, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento? situacaoCancelamentoPagamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {

            var result = _Consultar(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacaoCancelamentoPagamento);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, int transportador, int filial, int carga, int ocorrencia, int numero, int numeroDOC, int localidadePrestacao, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento? situacaoCancelamentoPagamento)
        {
            var result = _Consultar(dataInicio, dataFim, transportador, filial, carga, ocorrencia, numero, numeroDOC, localidadePrestacao, tomador, situacaoCancelamentoPagamento);

            return result.Count();
        }

    }
}
