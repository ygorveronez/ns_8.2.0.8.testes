using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.GestaoEntregas
{
    public class FluxoGestaoEntrega : RepositorioBase<Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega>
    {
        public FluxoGestaoEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega> _Consultar(SituacaoEtapaFluxoGestaoEntrega? situacao, EtapaFluxoGestaoPatio etapaFluxoGestaoPatio, string numeroCarga, DateTime dataInicial, DateTime dataFinal, int filial, double destinatario, string placa, string pedido, int numeroPedido, int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                result = result.Where(o => o.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(o => o.Veiculo.Placa == placa);

            if (!string.IsNullOrWhiteSpace(pedido))
                result = result.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.NumeroPedidoEmbarcador == pedido));

            if (numeroPedido > 0)
                result = result.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.Numero == numeroPedido));

            if (etapaFluxoGestaoPatio != EtapaFluxoGestaoPatio.Todas)
                result = result.Where(o => o.Etapas.Any(e => e.Ordem == o.IndexEtapa && e.Etapa == etapaFluxoGestaoPatio));

            if (situacao.HasValue)
                result = result.Where(o => o.Situacao == situacao);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataCriacao.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataCriacao.Date <= dataFinal);

            if (filial > 0)
                result = result.Where(o => o.Carga.Filial.Codigo == filial);

            if (destinatario > 0)
                result = result.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.Destinatario.CPF_CNPJ == destinatario));

            if (codigoTransportador > 0)
                result = result.Where(o => o.Carga.Empresa.Codigo == codigoTransportador);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega> Consultar(SituacaoEtapaFluxoGestaoEntrega? situacao, EtapaFluxoGestaoPatio etapaFluxoGestaoPatio, string numeroCarga, DateTime dataInicial, DateTime dataFinal, int filial, double destinatario, string placa, string pedido, int numeroPedido, int codigoTransportador, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(situacao, etapaFluxoGestaoPatio, numeroCarga, dataInicial, dataFinal, filial, destinatario, placa, pedido, numeroPedido, codigoTransportador);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .ToList();
        }

        public int ContarConsulta(SituacaoEtapaFluxoGestaoEntrega? situacao, EtapaFluxoGestaoPatio etapaFluxoGestaoPatio, string numeroCarga, DateTime dataInicial, DateTime dataFinal, int filial, double destinatario, string placa, string pedido, int numeroPedido, int codigoTransportador)
        {
            var result = _Consultar(situacao, etapaFluxoGestaoPatio, numeroCarga, dataInicial, dataFinal, filial, destinatario, placa, pedido, numeroPedido, codigoTransportador);

            return result.Count();
        }
    }
}
