using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class AgendamentoColetaPedido : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>
    {
        #region Construtores

        public AgendamentoColetaPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> BuscarPorAgendamentoColeta(int codigoAgendamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            var result = from obj in query where obj.AgendamentoColeta.Codigo == codigoAgendamento select obj;

            return result
                .Fetch(o => o.Pedido)
                .ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.Pedido)
                .ThenFetch(o => o.Destinatario)
                .Fetch(o => o.Pedido)
                .ThenFetch(o => o.TipoOperacao)
                .Fetch(o => o.Pedido)
                .ThenFetch(o => o.Remetente)
                .Fetch(o => o.Pedido)
                .ThenFetch(o => o.ProdutoPrincipal)
                .ThenFetch(o => o.GrupoProduto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> BuscarPorCodigos(List<int> codigosAgendamentoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            var result = from obj in query where codigosAgendamentoPedido.Contains(obj.Codigo) select obj;

            return result
                .Fetch(o => o.Pedido)
                .Fetch(o => o.AgendamentoColeta)
                .ThenFetch(o => o.TipoCarga)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> BuscarPedidosMesmoAgendamentoPorCodigo(List<int> codigosAgendamentoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>()
                .Where(p => p.Pedidos.Any(pd => codigosAgendamentoPedido.Contains(pd.Codigo)));

            return query
                .SelectMany(o => o.Pedidos)
                .Fetch(o => o.Pedido)
                .Fetch(o => o.AgendamentoColeta)
                .ThenFetch(o => o.TipoCarga)
                .ToList();
        }

        public DateTime? BuscarMenorDataPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            query = query.Where(obj => obj.AgendamentoColeta.Carga.Codigo == codigoCarga);

            return query
                .Select(obj => obj.Pedido.DataValidade)
                .Min();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            var result = from obj in query where obj.AgendamentoColeta.Carga.Codigo == codigoCarga select obj;

            return result
                .Fetch(o => o.Pedido)
                .ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.Pedido)
                .ThenFetch(o => o.Destinatario)
                .Fetch(o => o.Pedido)
                .ThenFetch(o => o.TipoOperacao)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido BuscarPrimeiroPorCargaFetchDestinatarioECarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            query = from obj in query where obj.AgendamentoColeta.Carga.Codigo == codigoCarga select obj;

            return query
                .Fetch(o => o.Pedido)
                .ThenFetch(o => o.Destinatario)
                .Fetch(o => o.AgendamentoColeta)
                .ThenFetch(o => o.Carga)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> BuscarPedidosPorAgendamentoColeta(int codigoAgendamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            var result = from obj in query where obj.AgendamentoColeta.Codigo == codigoAgendamento select obj;

            return result
                .Fetch(o => o.Pedido)
                .ToList();
        }

        public List<int> BuscarCodigosTipoCargaDosPedidosPorAgendamentoColeta(int codigoAgendamento)
        {
            var consultaAgendamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>()
                .Where(o => o.AgendamentoColeta.Codigo == codigoAgendamento && o.Pedido.TipoDeCarga != null);

            return consultaAgendamentoPedido
                .Select(o => o.Pedido.TipoDeCarga.Codigo)
                .Distinct()
                .ToList();
        }

        public int BuscarSKUPorCargaEDestinatarioRecebedor(int codigoCarga, double cpfCnpjDestino)
        {
            var consultaAgendamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>()
                .Where(o => o.AgendamentoColeta.Carga.Codigo == codigoCarga && o.AgendamentoColeta.Destinatario.CPF_CNPJ == cpfCnpjDestino || o.AgendamentoColeta.Recebedor.CPF_CNPJ == cpfCnpjDestino)
                .Select(o => o.SKU).ToList();

            return consultaAgendamentoPedido.Sum();
        }

        public List<(int Carga, int Sku)> BuscarSKUPorCargas(List<int> codigosCarga, double cpfCnpjDestino)
        {
            IQueryable<(int Carga, int Sku)> query = SessionNHiBernate.Query<(int Carga, int Sku)>();

            var consultaAgendamentoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>()
               .Where(o => o.AgendamentoColeta.Recebedor.CPF_CNPJ == cpfCnpjDestino || o.AgendamentoColeta.Destinatario.CPF_CNPJ == cpfCnpjDestino);

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosCarga.Count / quantidadeRegistrosConsultarPorVez;

            List<(int Carga, int Sku)> listaRetorno = new List<(int Carga, int Sku)>();

            for (int i = 0; i <= quantidadeConsultas; i++)
            {
                var result = consultaAgendamentoPedido
                    .Where(o => codigosCarga.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.AgendamentoColeta.Carga.Codigo))
                    .Select(c => new ValueTuple<int, int>(c.AgendamentoColeta.Carga.Codigo, c.SKU))
                    .ToList();

                listaRetorno.AddRange(result);
            }

            return listaRetorno
                .GroupBy(obj => obj.Carga)
                .Select(obj => ValueTuple.Create(obj.Key, obj.Sum(x => x.Sku)))
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido BuscarPorPedido(string numeroPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            var result = from obj in query
                         where obj.Pedido.NumeroPedidoEmbarcador == numeroPedido &&
       obj.AgendamentoColeta.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Cancelado &&
       obj.AgendamentoColeta.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.CanceladoEmbarcador
                         select obj;

            return result
                .Fetch(o => o.Pedido)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> BuscarPorCodigosAgendamentoColeta(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigos.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaRetornar = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                agendamentoColetaRetornar.AddRange(query.Where(o => codigos.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.AgendamentoColeta.Codigo)).Fetch(o => o.Pedido).ToList());

            return agendamentoColetaRetornar;

        }

        public Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido BuscarPorCodigoArquivo(int codigoArquivoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivoIntegracao) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> BuscarPorCargas(List<int> codigosCargas, int limiteRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            var result = from obj in query where codigosCargas.Contains(obj.AgendamentoColeta.Carga.Codigo) select obj;

            return result.Take(limiteRegistros).ToList();
        }
        #endregion

        #region Métodos Privados

        #endregion
    }
}
