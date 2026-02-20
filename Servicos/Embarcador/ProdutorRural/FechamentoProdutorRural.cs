using System.Collections.Generic;

namespace Servicos.Embarcador.ProdutorRural
{
    public static class FechamentoProdutorRural
    {
        public static void VerificarFechamentosCargaEmEmissao(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            unitOfWork.FlushAndClear();
            //unitOfWork.Dispose();
            //unitOfWork = null;
            //unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor repFechamentoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.FechamentoColetaProdutor(unitOfWork);

            List<Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor> fechamentoColetaProdutors = repFechamentoColetaProdutor.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor.AgEmissaoCarga, 10);

            for (int i = 0; i < fechamentoColetaProdutors.Count; i++)
            {
                Dominio.Entidades.Embarcador.ProdutorRural.FechamentoColetaProdutor fechamentoColetaProdutor = fechamentoColetaProdutors[i];
                if (fechamentoColetaProdutor.Carga != null &&
                    (fechamentoColetaProdutor.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                    || fechamentoColetaProdutor.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                    || fechamentoColetaProdutor.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                    || fechamentoColetaProdutor.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                    || fechamentoColetaProdutor.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento
                    ))
                {
                    unitOfWork.Start();

                    fechamentoColetaProdutor.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoColetaProdutor.Finalizado;

                    repFechamentoColetaProdutor.Atualizar(fechamentoColetaProdutor);

                    unitOfWork.CommitChanges();
                }
            }
        }

        public static void GerarPedidoColetaProdutor(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor repPedidoColetaProdutor = new Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor(unitOfWork);

            Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor pedidoColetaProdutor = repPedidoColetaProdutor.BuscarPorPedido(pedido.Codigo);

            if (pedidoColetaProdutor == null)
            {
                pedidoColetaProdutor = new Dominio.Entidades.Embarcador.ProdutorRural.PedidoColetaProdutor()
                {
                    Pedido = pedido,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedidoColetaProdutor.AgFechamento
                };

                repPedidoColetaProdutor.Inserir(pedidoColetaProdutor);
            }
        }
    }
}
