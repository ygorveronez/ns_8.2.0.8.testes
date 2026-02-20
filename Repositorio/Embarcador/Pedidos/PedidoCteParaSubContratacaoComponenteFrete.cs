using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoCteParaSubContratacaoComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>
    {
        public PedidoCteParaSubContratacaoComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> BuscarPorPedidoCteParaSubcontratacao(int pedidoCTeSubcontratacao, bool apenasModeloCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
            var result = from obj in query where obj.PedidoCTeParaSubContratacao.Codigo == pedidoCTeSubcontratacao select obj;

            if (apenasModeloCTe)
                result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> BuscarPorCarga(int codigoCarga, bool apenasModeloCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
            var result = from obj in query where obj.PedidoCTeParaSubContratacao.CargaPedido.Carga.Codigo == codigoCarga select obj;

            if (apenasModeloCTe)
                result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete BuscarPorPedidoCteParaSubContratacaoETipo(int codigoPedidoCTeParaSubContratacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();

            var result = from obj in query where obj.PedidoCTeParaSubContratacao.Codigo == codigoPedidoCTeParaSubContratacao && obj.TipoComponenteFrete == tipoComponente select obj;

            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> BuscarPorPedidoCTeParaSubcontratacao(int pedidoCTeSubcontratacao, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
            var result = from obj in query where (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && obj.PedidoCTeParaSubContratacao.Codigo == pedidoCTeSubcontratacao select obj;

            if (modeloDocumentoFiscal == null || modeloDocumentoFiscal.Numero == "57")
                result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");
            else
                result = result.Where(obj => obj.ModeloDocumentoFiscal.Codigo == modeloDocumentoFiscal.Codigo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> BuscarPorPedidosCTeParaSubcontratacao(List<int> pedidosCTeSubcontratacao, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal)
        {
            if (pedidosCTeSubcontratacao.Count < 2000)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
                var result = from obj in query where (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && pedidosCTeSubcontratacao.Contains(obj.PedidoCTeParaSubContratacao.Codigo) select obj;

                if (modeloDocumentoFiscal == null || modeloDocumentoFiscal.Numero == "57")
                    result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");
                else
                    result = result.Where(obj => obj.ModeloDocumentoFiscal.Codigo == modeloDocumentoFiscal.Codigo);

                return result
                    .Fetch(obj => obj.ComponenteFrete)
                    .ToList();
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> listaRetornar = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
            List<int> listaOriginal = pedidosCTeSubcontratacao;
            int tamanhoLote = 2000;
            int indiceInicial = 0;

            while (indiceInicial < listaOriginal.Count)
            {
                List<int> lote = listaOriginal.GetRange(indiceInicial, Math.Min(tamanhoLote, listaOriginal.Count - indiceInicial));

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
                var result = from obj in query where (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && lote.Contains(obj.PedidoCTeParaSubContratacao.Codigo) select obj;

                if (modeloDocumentoFiscal == null || modeloDocumentoFiscal.Numero == "57")
                    result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");
                else
                    result = result.Where(obj => obj.ModeloDocumentoFiscal.Codigo == modeloDocumentoFiscal.Codigo);

                listaRetornar.AddRange(
                    result
                    .Fetch(obj => obj.ComponenteFrete)
                    .ToList()
                    );

                indiceInicial += tamanhoLote;
            }

            return listaRetornar;

        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> BuscarPorPedidoCTeParaSubcontratacao(List<int> pedidosCTeSubcontratacao, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
            var result = from obj in query where (obj.ComponenteFrete == null || !obj.ComponenteFrete.ComponentePertenceComposicaoFreteValor) && pedidosCTeSubcontratacao.Contains(obj.PedidoCTeParaSubContratacao.Codigo) select obj;

            if (modeloDocumentoFiscal == null || modeloDocumentoFiscal.Numero == "57")
                result = result.Where(obj => obj.ModeloDocumentoFiscal == null || obj.ModeloDocumentoFiscal.Numero == "57");
            else
                result = result.Where(obj => obj.ModeloDocumentoFiscal.Codigo == modeloDocumentoFiscal.Codigo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> BuscarTodosdoCargaPedido(int cargaPedido, bool componenteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
            var result = from obj in query where obj.PedidoCTeParaSubContratacao.CargaPedido.Codigo == cargaPedido && obj.ComponenteFilialEmissora == componenteFilialEmissora select obj;

            return result.ToList();
        }

        public decimal BuscarTotalCargaPedidoPorCompomente(int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
            var result = from obj in query where obj.PedidoCTeParaSubContratacao.CargaPedido.Codigo == cargaPedido && obj.TipoComponenteFrete == tipoComponente select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0;
        }

        public decimal BuscarTotalMoedaCargaPedidoPorComponente(int cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();

            query = query.Where(obj => obj.PedidoCTeParaSubContratacao.CargaPedido.Codigo == cargaPedido && obj.TipoComponenteFrete == tipoComponente);

            if (componente != null)
                query = query.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return query.Select(obj => (decimal?)obj.ValorComponente).Sum() ?? 0m;
        }

        public List<(decimal Valor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponente, int CodigoComponenteFrete)> BuscarValoresPorPedidoCTeParaSubContratacao(int codigoPedidoCTeParaSubContratacao)
        {
            var consultaComponenteFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>()
                .Where(componenteFrete => componenteFrete.PedidoCTeParaSubContratacao.Codigo == codigoPedidoCTeParaSubContratacao);

            return consultaComponenteFrete
                .Select(componenteFrete => ValueTuple.Create(componenteFrete.ValorComponente, componenteFrete.TipoComponenteFrete, (componenteFrete.ComponenteFrete == null) ? 0 : componenteFrete.ComponenteFrete.Codigo))
                .ToList();
        }

        public decimal BuscarValorTotalDescontoPorPedidoCTeParaSubContratacao(int codigoPedidoCTeParaSubContratacao)
        {
            string sqlQuery = @"SELECT SUM(PedidoCTeParaSubcontratacaoComponentesFrete.PCF_VALOR_COMPONENTE)  
                                  FROM T_PEDIDO_CTE_PARA_SUBCONTRATACAO_COMPONENTES_FRETE PedidoCTeParaSubcontratacaoComponentesFrete 
                                  JOIN T_COMPONENTE_FRETE ComponenteFrete on ComponenteFrete.CFR_CODIGO = PedidoCTeParaSubcontratacaoComponentesFrete.CFR_CODIGO 
                                 WHERE PedidoCTeParaSubcontratacaoComponentesFrete.PSC_CODIGO = :PSC_CODIGO 
                                   AND ComponenteFrete.CFR_DESCONTAR_COMPONENTE_FRETE_LIQUIDO = 1";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("PSC_CODIGO", codigoPedidoCTeParaSubContratacao);

            return query.UniqueResult<decimal>();
        }

        public void DeletarPorPedidoCTeParaSubcontratacao(int codigoPedidoCTeParaSubcontratacao)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao.CreateQuery("DELETE PedidoCteParaSubContratacaoComponenteFrete obj WHERE obj.PedidoCTeParaSubContratacao.Codigo = :codigoPedidoCTeParaSubcontratacao")
                                 .SetInt32("codigoPedidoCTeParaSubcontratacao", codigoPedidoCTeParaSubcontratacao)
                                 .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery("DELETE PedidoCteParaSubContratacaoComponenteFrete obj WHERE obj.PedidoCTeParaSubContratacao.Codigo = :codigoPedidoCTeParaSubcontratacao")
                                .SetInt32("codigoPedidoCTeParaSubcontratacao", codigoPedidoCTeParaSubcontratacao)
                                .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }
    }
}
