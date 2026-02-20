using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class AgendamentoEntregaPedidoConsultaAuditoria : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria>
    {
        #region Construtores

        public AgendamentoEntregaPedidoConsultaAuditoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public List<int> BuscarCodigosPedidosJaAdicionadosPorPedidos(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria>()
                .Where(obj => codigosPedidos.Contains(obj.Pedido.Codigo));

            return query
                .Select(obj => obj.Pedido.Codigo)
                .ToList();
        }

        public int AdicionarAuditorias(List<int> codigosPedidos, int codigoEmpresa)
        {
            List<string> valores = new List<string>();

            for (int i = 0; i < codigosPedidos.Count; i++)
                valores.Add($" ({codigoEmpresa}, {codigosPedidos[i]}, '{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}')");

            string sql = $"INSERT INTO T_AGENDAMENTO_ENTREGA_PEDIDO_CONSULTA_AUDITORIA (EMP_CODIGO, PED_CODIGO, APA_DATA) VALUES {string.Join(", ", valores)}"; // SQL-INJECTION-SAFE

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria> BuscarPorPedidos(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria>()
              .Where(obj => codigosPedidos.Contains(obj.Pedido.Codigo));

            return query
                .Fetch(obj => obj.Pedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria> ConsultaPorPedido(int codigoPedido, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria>()
              .Where(obj => codigoPedido == obj.Pedido.Codigo);

            query = query
                .Fetch(obj => obj.Pedido)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Pais);
            
            return ObterLista(query, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria> ConsultaPorPedidos(List<int> codigosPedidos, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria>()
              .Where(obj => codigosPedidos.Contains(obj.Pedido.Codigo));

            query = query
                .Fetch(obj => obj.Pedido)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Pais);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsultaPorPedido(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria>()
              .Where(obj => codigosPedidos.Contains(obj.Pedido.Codigo));

            return query.Count();
        }
    }
}
