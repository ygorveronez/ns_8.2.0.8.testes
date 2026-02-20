using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoCancelamento : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento>
    {
        public PedidoCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento>();

            query = query.Where(o => o.Pedido.Codigo == codigoPedido);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento> Consultar(int numeroPedido, string numeroPedidoEmbarcador, int codigoGrupoPessoas, double cpfCnpjRemetente, double cpfCnpjDestinatario, DateTime dataInicial, DateTime dataFinal, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento>();

            if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                query = query.Where(o => o.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador);

            if (numeroPedido > 0)
                query = query.Where(o => o.Pedido.Numero == numeroPedido);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.Pedido.GrupoPessoas.Codigo == codigoGrupoPessoas || o.Pedido.Remetente.GrupoPessoas.Codigo == codigoGrupoPessoas || o.Pedido.Destinatario.GrupoPessoas.Codigo == codigoGrupoPessoas);

            if (cpfCnpjRemetente > 0d)
                query = query.Where(o => o.Pedido.Remetente.CPF_CNPJ == cpfCnpjRemetente);

            if (cpfCnpjDestinatario > 0d)
                query = query.Where(o => o.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            if (cpfCnpjDestinatario > 0d)
                query = query.Where(o => o.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataCancelamento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataCancelamento < dataFinal.AddDays(1).Date);

            return query.Fetch(o => o.Pedido).ThenFetch(o => o.Usuario).OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int numeroPedido, string numeroPedidoEmbarcador, int codigoGrupoPessoas, double cpfCnpjRemetente, double cpfCnpjDestinatario, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento>();

            if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                query = query.Where(o => o.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador);

            if (numeroPedido > 0)
                query = query.Where(o => o.Pedido.Numero == numeroPedido);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.Pedido.GrupoPessoas.Codigo == codigoGrupoPessoas || o.Pedido.Remetente.GrupoPessoas.Codigo == codigoGrupoPessoas || o.Pedido.Destinatario.GrupoPessoas.Codigo == codigoGrupoPessoas);

            if (cpfCnpjRemetente > 0d)
                query = query.Where(o => o.Pedido.Remetente.CPF_CNPJ == cpfCnpjRemetente);

            if (cpfCnpjDestinatario > 0d)
                query = query.Where(o => o.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            if (cpfCnpjDestinatario > 0d)
                query = query.Where(o => o.Pedido.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataCancelamento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataCancelamento < dataFinal.AddDays(1).Date);

            return query.Count();
        }
    }
}
