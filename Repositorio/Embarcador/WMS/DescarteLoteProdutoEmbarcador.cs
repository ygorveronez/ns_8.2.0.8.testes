using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class DescarteLoteProdutoEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador>
    {

        public DescarteLoteProdutoEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> _Consultar(int produto, DateTime dataInicio, DateTime dataFim, int produtoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador>();

            var result = from obj in query select obj;

            // Filtros
            if(dataInicio != DateTime.MinValue)
                result = result.Where(o => o.Data.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.Data.Date <= dataFim);

            if (produtoEmbarcador > 0)
                result = result.Where(o => o.Lote.ProdutoEmbarcador.Codigo == produtoEmbarcador);

            if (situacao.HasValue)
                result = result.Where(o => o.Situacao == situacao.Value);

            if (produto > 0)
                result = result.Where(o => o.Produto.Codigo == produto);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador> Consultar(int produto, DateTime dataInicio, DateTime dataFim, int produtoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(produto, dataInicio, dataFim, produtoEmbarcador, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int produto, DateTime dataInicio, DateTime dataFim, int produtoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador? situacao)
        {
            var result = _Consultar(produto, dataInicio, dataFim, produtoEmbarcador, situacao);

            return result.Count();
        }
    }
}
