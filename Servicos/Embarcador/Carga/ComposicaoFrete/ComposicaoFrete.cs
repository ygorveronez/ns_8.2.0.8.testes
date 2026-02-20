using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.ComposicaoFrete
{
    public class ComposicaoFrete
    {
        public static void SetarComposicaoFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao, bool composicaoFreteFilialEmissora, List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete> composicaoFretes, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            bool abriuTransacao = false;
            if (!unitOfWork.IsActiveTransaction())
            {
                unitOfWork.Start();
                abriuTransacao = true;
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao in composicaoFretes)
                SetarComposicaoFrete(carga, cargaPedidos, pedidoXMLNotaFiscais, pedidoCTesParaSubcontratacao, composicaoFreteFilialEmissora, composicao, unitOfWork, preCarga);

            if (abriuTransacao)
                unitOfWork.CommitChanges();
        }

        public static void SetarComposicaoFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao, bool composicaoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete cargaComposicao = new Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete();
            cargaComposicao.Carga = carga;
            cargaComposicao.ComposicaoFreteFilialEmissora = composicaoFreteFilialEmissora;
            cargaComposicao.Formula = composicao.Formula;
            cargaComposicao.ValoresFormula = composicao.ValoresFormula;
            cargaComposicao.DescricaoComponente = composicao.DescricaoComponente;
            cargaComposicao.PreCarga = preCarga;
            cargaComposicao.Valor = composicao.Valor;
            cargaComposicao.ComponenteFrete = composicao.CodigoComponente > 0 ? new Dominio.Entidades.Embarcador.Frete.ComponenteFrete { Codigo = composicao.CodigoComponente } : null;
            cargaComposicao.CargaPedidos = cargaPedidos;
            cargaComposicao.PedidoXMLNotasFiscais = pedidoXMLNotaFiscais;
            cargaComposicao.PedidoCTesParaSubContratacao = pedidoCTesParaSubcontratacao;
            cargaComposicao.ValorCalculado = composicao.ValorCalculado;
            cargaComposicao.TipoCampoValor = composicao.TipoValor;
            cargaComposicao.TipoParametro = composicao.TipoParametro;
            repCargaComposicaoFrete.Inserir(cargaComposicao);
        }

        public static void AtualizarComposicaoFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete cargaComposicao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao, bool composicaoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);

            cargaComposicao.Carga = carga;
            cargaComposicao.ComposicaoFreteFilialEmissora = composicaoFreteFilialEmissora;
            cargaComposicao.Formula = composicao.Formula;
            cargaComposicao.ValoresFormula = composicao.ValoresFormula;
            cargaComposicao.DescricaoComponente = composicao.DescricaoComponente;
            cargaComposicao.Valor = composicao.Valor;
            cargaComposicao.ComponenteFrete = new Dominio.Entidades.Embarcador.Frete.ComponenteFrete { Codigo = composicao.CodigoComponente };
            cargaComposicao.CargaPedidos = cargaPedidos;
            cargaComposicao.PedidoXMLNotasFiscais = pedidoXMLNotaFiscais;
            cargaComposicao.PedidoCTesParaSubContratacao = pedidoCTesParaSubcontratacao;
            cargaComposicao.ValorCalculado = composicao.ValorCalculado;
            cargaComposicao.TipoCampoValor = composicao.TipoValor;
            cargaComposicao.TipoParametro = composicao.TipoParametro;

            repCargaComposicaoFrete.Atualizar(cargaComposicao);
        }


        public static void AtualizarComposicaoFreteAlteradoManualmente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete cargaComposicao, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao, bool composicaoFreteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);
            cargaComposicao.Carga = carga;
            cargaComposicao.ComposicaoFreteFilialEmissora = composicaoFreteFilialEmissora;
            cargaComposicao.Formula = composicao.Formula;
            cargaComposicao.ValoresFormula = composicao.ValoresFormula;
            cargaComposicao.DescricaoComponente = composicao.DescricaoComponente;
            cargaComposicao.Valor = composicao.Valor;
            cargaComposicao.ComponenteFrete = new Dominio.Entidades.Embarcador.Frete.ComponenteFrete { Codigo = composicao.CodigoComponente };
            cargaComposicao.CargaPedidos = cargaPedidos;
            cargaComposicao.PedidoXMLNotasFiscais = pedidoXMLNotaFiscais;
            cargaComposicao.PedidoCTesParaSubContratacao = pedidoCTesParaSubcontratacao;
            cargaComposicao.ValorCalculado = composicao.ValorCalculado;
            cargaComposicao.TipoCampoValor = composicao.TipoValor;
            cargaComposicao.TipoParametro = composicao.TipoParametro;

            repCargaComposicaoFrete.Atualizar(cargaComposicao);
        }


        

        public static void ExcluirComposicoesFrete(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes = repCargaComposicaoFrete.BuscarPorPreCarga(preCarga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete cargaComposicao in cargaComposicaoFretes)
            {
                cargaComposicao.PedidoXMLNotasFiscais = null;
                cargaComposicao.PedidoCTesParaSubContratacao = null;

                repCargaComposicaoFrete.Deletar(cargaComposicao);
            }
        }

        public static void ExcluirComposicoesFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);
            repCargaComposicaoFrete.DeletarPorCarga(carga.Codigo, false);
        }

        public static void ExcluirComposicoesFreteValor(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);
            repCargaComposicaoFrete.DeletarPorCarga(carga.Codigo, true);
        }
    }
}
