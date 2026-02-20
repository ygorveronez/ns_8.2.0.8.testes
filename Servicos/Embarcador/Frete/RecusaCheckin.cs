using System.Collections.Generic;

namespace Servicos.Embarcador.Frete
{
    public class RecusaCheckin
    {
        #region Atributos

        protected readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public RecusaCheckin(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void RecusarCheckin(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            if (cargaCTe.Carga.TipoOperacao?.TipoConsolidacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao.PreCheckIn && cargaCTe.Carga.DadosSumarizados.CargaTrecho == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaTrechoSumarizada.Agrupadora)
                AtualizarRecusaCargaCtesConsolidado(cargaCTe);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> ListacargaPedidoCTE = repCargaPedido.BuscarPorCargaECte(cargaCTe.Carga.Codigo, cargaCTe.CTe?.Codigo ?? 0);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in ListacargaPedidoCTE)
                CriarVinculoRecusaChekinCargaPedido(cargaPedido, cargaCTe.CTe, cargaCTe.Carga);

        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void CriarVinculoRecusaChekinCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE repCargaPedidoRecusa = new Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if (!repCargaPedidoRecusa.ExisteRecusaPorPedidoECarga(cargaPedido.Pedido.Codigo, cargaOrigem.Codigo))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE cargaPedidoRecusa = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE();
                cargaPedidoRecusa.Pedido = cargaPedido.Pedido;
                cargaPedidoRecusa.CTe = cte;
                cargaPedidoRecusa.CargaRecusaOrigem = cargaOrigem;
                cargaPedidoRecusa.CargaRecusaGerada = repCarga.BuscarCargaColetaPreCheckinPorCte(cte.Codigo);

                repCargaPedidoRecusa.Inserir(cargaPedidoRecusa);
            }
        }

        public void AtualizarRecusaCargaCtesConsolidado(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE repositorioCargaPedidoRecusaCte = new Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtesConsolidado = repCargaCte.BuscarTodosPorCTe(cargaCTe.CTe?.Codigo ?? 0);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteconsolidado in cargaCtesConsolidado)
            {
                //aqui vamos identificar as cargas de transferencia e cargas de entregas e remover o vinculo com o CTE
                if (!(cargaCteconsolidado.Carga.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? false) && cargaCteconsolidado.Carga.TipoOperacao?.TipoConsolidacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoConsolidacao.PreCheckIn && cargaCteconsolidado.Carga.DadosSumarizados.CargaTrecho == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaTrechoSumarizada.SubCarga)
                {
                    repositorioCargaPedidoRecusaCte.DeletarCargaCTERecusado(cargaCteconsolidado.Codigo);
                    continue;
                }

                if (cargaCteconsolidado.Codigo != cargaCTe.Codigo)
                {
                    cargaCteconsolidado.SituacaoCheckin = cargaCTe.SituacaoCheckin;
                    repCargaCte.Atualizar(cargaCteconsolidado);
                }

            }
        }


        public void ReplicarCteRecusadoCargaCtesConsolidado(Dominio.Entidades.Embarcador.Cargas.Carga cargaDT)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> listaCargaPedidoXMLNotaFiscalCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarPorCargaNaoReplicados(cargaDT.Codigo);

            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentoStageCarga = repStageAgrupamento.BuscarPorCargaDt(cargaDT.Codigo);
            foreach (var agrupamento in agrupamentoStageCarga)
            {
                if (agrupamento.CargaGerada == null)
                    continue;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargapedidoXmlNotaFiscalCte in listaCargaPedidoXMLNotaFiscalCTe)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoCargaFilho = repCargaPedido.BuscarPorCargaEPedido(agrupamento.CargaGerada.Codigo, cargapedidoXmlNotaFiscalCte.PedidoXMLNotaFiscal?.CargaPedido?.Pedido?.Codigo ?? 0);

                    if (cargaPedidoCargaFilho != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteExiste = repCargaCte.BuscarPorCargaECTe(agrupamento.CargaGerada.Codigo, cargapedidoXmlNotaFiscalCte.CargaCTe?.CTe?.Codigo ?? 0);
                        if (cargaCteExiste == null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteClonado = cargapedidoXmlNotaFiscalCte.CargaCTe.Clonar();
                            Utilidades.Object.DefinirListasGenericasComoNulas(cargaCteClonado);

                            cargaCteClonado.Carga = agrupamento.CargaGerada;
                            cargaCteClonado.CargaOrigem = agrupamento.CargaGerada;
                            cargaCteClonado.CTe = cargapedidoXmlNotaFiscalCte.CargaCTe.CTe;
                            repCargaCte.Inserir(cargaCteClonado);

                            cargapedidoXmlNotaFiscalCte.CargaCTe.ReplicadoCargaFilho = true;
                            repCargaCte.Atualizar(cargapedidoXmlNotaFiscalCte.CargaCTe);

                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> ListaPedidoXMlNotaFiscalFilho = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedidoCargaFilho.Codigo);

                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoxmlNotaFiscal in ListaPedidoXMlNotaFiscalFilho)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalTransbordo = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                                cargaPedidoXMLNotaFiscalTransbordo.PedidoXMLNotaFiscal = pedidoxmlNotaFiscal;
                                cargaPedidoXMLNotaFiscalTransbordo.CargaCTe = cargaCteClonado;
                                repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalTransbordo);
                            }
                        }

                        repCarga.Atualizar(agrupamento.CargaGerada);
                    }
                }
            }
        }

        #endregion Métodos Privados

    }
}
