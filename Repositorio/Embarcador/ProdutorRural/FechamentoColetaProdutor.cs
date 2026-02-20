using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.ProdutorRural
{
    public class FechamentoColetaProdutor : RepositorioBase<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor>
    {
        public FechamentoColetaProdutor(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor> BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor situacaoFechamentoColetaProdutor, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor>();
            query = query.Where(o => o.Situacao == situacaoFechamentoColetaProdutor);

            if (limite > 0)
                query = query.Take(limite);

            return query.Fetch(obj => obj.Carga).ToList();
        }

        public int ObterProximoFechamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor>();

            var result = from obj in query select obj;

            int? retorno = result.Max(o => (int?)o.Numero);
            return retorno.HasValue ? (retorno.Value + 1) : 1;

        }

        private IQueryable<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor> _Consultar(int numero, DateTime dataInicio, DateTime dataFim, int transportador, int filial, string preCarga, int pedido, int origem, double remetente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor situacaoFechamentoColetaProdutor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor>();

            var result = from obj in query select obj;

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial.Value.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal.Value.Date <= dataFim);

            if (transportador > 0)
                result = result.Where(o => o.Empresa.Codigo == transportador);

            if (!string.IsNullOrWhiteSpace(preCarga))
                result = result.Where(o => o.PedidosFechamento.Any(doc => doc.PedidoColetaProdutor.Pedido.PreCarga.NumeroPreCarga == preCarga));

            if (pedido > 0)
                result = result.Where(o => o.PedidosFechamento.Any(doc => doc.PedidoColetaProdutor.Pedido.Codigo == pedido));

            if (filial > 0)
                result = result.Where(o => o.Filial.Codigo == filial);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (situacaoFechamentoColetaProdutor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor.Todos)
                result = result.Where(o => o.Situacao == situacaoFechamentoColetaProdutor);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor> Consultar(int numero, DateTime dataInicio, DateTime dataFim, int transportador, int filial, string preCarga, int pedido, int origem, double remetente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor situacaoFechamentoColetaProdutor, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {

            var result = _Consultar(numero, dataInicio, dataFim, transportador, filial, preCarga, pedido, origem, remetente, situacaoFechamentoColetaProdutor);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int numero, DateTime dataInicio, DateTime dataFim, int transportador, int filial, string preCarga, int pedido, int origem, double remetente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor situacaoFechamentoColetaProdutor)
        {
            var result = _Consultar(numero, dataInicio, dataFim, transportador, filial, preCarga, pedido, origem, remetente, situacaoFechamentoColetaProdutor);

            return result.Count();
        }
    }
}
