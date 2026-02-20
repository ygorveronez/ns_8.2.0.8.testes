using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoDadosTransporteMaritimo : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>
    {
        #region Construtores

        public PedidoDadosTransporteMaritimo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PedidoDadosTransporteMaritimo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo BuscarPorPedido(int codigoPedido)
        {
            var PedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>()
                .Where(o => o.Pedido.Codigo == codigoPedido && o.CodigoOriginal == 0 && !o.BookingTemporario && o.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo.Ativo);

            return PedidoDadosTransporteMaritimo.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> BuscarPorPedidoAsync(int codigoPedido)
        {
            var PedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>()
                .Where(o => o.Pedido.Codigo == codigoPedido && o.CodigoOriginal == 0 && !o.BookingTemporario && o.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo.Ativo);

            return PedidoDadosTransporteMaritimo.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo BuscarPorPedidoENumeroEXP(string NumeroPedido, string NumeroEXP)
        {
            var PedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>()
            .Where(o => o.Pedido.NumeroPedidoEmbarcador == NumeroPedido && o.CodigoOriginal == 0 && !o.BookingTemporario && o.NumeroEXP == NumeroEXP && o.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo.Ativo);

            return PedidoDadosTransporteMaritimo.FirstOrDefault();
        }

        public bool PossuiBookingTemporarioEmIntegracao(string NumeroPedido)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>().Any(o => o.Pedido.NumeroPedidoEmbarcador == NumeroPedido && o.BookingTemporario);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> BuscarPorPedidos(List<int> codigosPedido)
        {
            var PedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>()
                .Where(o => codigosPedido.Contains(o.Pedido.Codigo) && o.CodigoOriginal == 0 && !o.BookingTemporario && o.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo.Ativo);

            return PedidoDadosTransporteMaritimo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> BuscarTodosPorPedido(int codigoPedido)
        {
            var PedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>()
                .Where(o => o.Pedido.Codigo == codigoPedido && !o.BookingTemporario && o.CodigoOriginal == 0 && o.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo.Ativo);

            return PedidoDadosTransporteMaritimo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consulta(filtrosPesquisa);
            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimo filtrosPesquisa)
        {
            var query = Consulta(filtrosPesquisa);

            return query.Count();
        }

        public void InformarDataEstufagemDadosTransporteMaritimoSQL(DateTime dataCarregamento, int carga)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("update T_PEDIDO_DADOS_TRANSPORTE_MARITIMO ");
            sql.Append($" set CTM_PREVISAO_ESTUFAGEM = :DataCarregamento ");
            sql.Append("  from T_PEDIDO_DADOS_TRANSPORTE_MARITIMO pedidoDadosTransporte join T_PEDIDO pedido on pedido.PED_CODIGO = pedidoDadosTransporte.PED_CODIGO join T_CARGA_PEDIDO cargapedido ON cargapedido.PED_CODIGO = pedido.PED_CODIGO ");
            sql.Append($"  where pedidoDadosTransporte.CTM_STATUS = 0 AND cargaPedido.CAR_CODIGO = :Carga ");

            UnitOfWork.Sessao
                .CreateSQLQuery(sql.ToString())
                .SetParameter("Carga", carga)
                .SetParameter("DataCarregamento", dataCarregamento)
                .ExecuteUpdate();

        }

        public bool ExistePedidoDadosTransporteMaritimo()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> PedidoDadosTransporteMaritimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>();

            return PedidoDadosTransporteMaritimo.FirstOrDefault() != null;

        }
        #endregion Métodos Públicos


        #region Metodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> Consulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimo filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>();
            query = query.Where(obj => obj.CodigoOriginal == 0 && !obj.BookingTemporario);

            //if (filtrosPesquisa.Filial  > 0)
            //    query = query.Where(obj => obj.Filial.Codigo == filtrosPesquisa.Filial);

            if (filtrosPesquisa.DataInicial.HasValue && filtrosPesquisa.DataInicial.Value != DateTime.MinValue)
                query = query.Where(obj => obj.DataBooking >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataFim.HasValue && filtrosPesquisa.DataFim.Value != DateTime.MinValue)
                query = query.Where(obj => obj.DataBooking <= filtrosPesquisa.DataFim.Value);

            if (filtrosPesquisa.Destino > 0)
                query = query.Where(obj => obj.Pedido.Recebedor.Localidade.Codigo == filtrosPesquisa.Destino);

            if (filtrosPesquisa.Origem > 0)
                query = query.Where(obj => obj.Pedido.Remetente.Localidade.Codigo == filtrosPesquisa.Origem);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCargaEmbarcador))
                query = query.Where(obj => obj.Pedido.CargasPedido.Any(o => o.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCargaEmbarcador));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.numeroBooking))
                query = query.Where(obj => obj.Pedido.NumeroBooking == filtrosPesquisa.numeroBooking);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.numeroEXP))
                query = query.Where(obj => obj.Pedido.NumeroEXP == filtrosPesquisa.numeroEXP);

            if (filtrosPesquisa.status.HasValue)
                query = query.Where(obj => obj.Status == filtrosPesquisa.status.Value);

            return query;
        }


        #endregion

        #region Relatório

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoDadosTransporteMaritimo> ConsultarRelatorioPedidoDadosTransporteMaritimo(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoDadosTransporteMaritimo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPedidoDadosTransporteMaritimo = new ConsultaPedidoDadosTransporteMaritimo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPedidoDadosTransporteMaritimo.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoDadosTransporteMaritimo)));

            return consultaPedidoDadosTransporteMaritimo.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoDadosTransporteMaritimo>();
        }

        public int ContarConsultaRelatorioPedidoDadosTransporteMaritimo(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoDadosTransporteMaritimo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPedidoDadosTransporteMaritimo = new ConsultaPedidoDadosTransporteMaritimo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPedidoDadosTransporteMaritimo.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}