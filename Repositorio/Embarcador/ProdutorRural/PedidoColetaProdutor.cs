using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.ProdutorRural
{
    public class PedidoColetaProdutor : RepositorioBase<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor>
    {
        public PedidoColetaProdutor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> BuscarPreCargaParaProcessamento(bool apenasPendentes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor>();
            var result = from obj in query
                         where
                            obj.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor.AgFechamento
                            && (
                                obj.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario
                                || obj.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente
                            )
                         select obj.Pedido.PreCarga;

            if (apenasPendentes)
                result = result.Where(o => o.CalculandoFrete || o.PendenciaCalculoFrete);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor BuscarPorPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor>();

            query = query.Where(o => o.Pedido.Codigo == codigoPedido);

            return query.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor> _Consultar(DateTime dataInicio, DateTime dataFim, int transportador, int filial, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor situacaoSituacaoPedidoColetaProdutor, Dominio.Enumeradores.TipoTomador tipoTomador, bool somenteFretePendente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor>();

            var result = from obj in query where obj.Pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado select obj;

            result = result.Where(obj => obj.Pedido.TipoTomador == tipoTomador);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Pedido.DataInicialColeta >= dataInicio.Date);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Pedido.DataInicialColeta < dataFim.AddDays(1).Date);

            if (transportador > 0)
                result = result.Where(o => o.Pedido.Empresa.Codigo == transportador);

            if (tomador > 0)
            {
                result = result.Where(o => (o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.Pedido.Destinatario.CPF_CNPJ == tomador)
                || (o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.Pedido.Remetente.CPF_CNPJ == tomador)
                || (o.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.Pedido.Tomador.CPF_CNPJ == tomador));
            }

            if (somenteFretePendente)
                result = result.Where(o => o.Pedido.PreCarga.CalculandoFrete || o.Pedido.PreCarga.PendenciaCalculoFrete);

            if (filial > 0)
                result = result.Where(o => o.Pedido.Filial.Codigo == filial);

            if (situacaoSituacaoPedidoColetaProdutor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor.Todas)
                result = result.Where(o => o.Situacao == situacaoSituacaoPedidoColetaProdutor);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor> Consultar(DateTime dataInicio, DateTime dataFim, int transportador, int filial, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor situacaoSituacaoPedidoColetaProdutor, Dominio.Enumeradores.TipoTomador tipoTomador, bool somenteFretePendente, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {

            var result = _Consultar(dataInicio, dataFim, transportador, filial, tomador, situacaoSituacaoPedidoColetaProdutor, tipoTomador, somenteFretePendente);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.PreCarga)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Remetente)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Origem)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.Destino)
                .Fetch(obj => obj.Pedido)
                .ThenFetch(obj => obj.EnderecoOrigem)
                .ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, int transportador, int filial, double tomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor situacaoSituacaoPedidoColetaProdutor, Dominio.Enumeradores.TipoTomador tipoTomador, bool somenteFretePendente)
        {
            var result = _Consultar(dataInicio, dataFim, transportador, filial, tomador, situacaoSituacaoPedidoColetaProdutor, tipoTomador, somenteFretePendente);

            return result.Count();
        }
    }
}
