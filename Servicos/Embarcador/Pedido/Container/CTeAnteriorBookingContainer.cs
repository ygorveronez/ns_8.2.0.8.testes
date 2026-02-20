using Repositorio;
using System;
using System.Collections.Generic;


namespace Servicos.Embarcador.Pedido.Container
{
    public class CTeAnteriorBookingContainer : ServicoBase
    {
        public CTeAnteriorBookingContainer(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public static void ProcessarCTeAnteriorBookingContainer(Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new ConhecimentoDeTransporteEletronico(unidadeTrabalho);              
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Pedido.Container.CTeAnteriorBookingContainer> ctesPentendes = repCTe.BuscarCTeAnteriorBookingContainerPendenteVinculo();

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;

                for (int i = 0; i < ctesPentendes.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarCargaPedidoParaVincularCTesPorNumeroBooking(ctesPentendes[i].NumeroBooking, ctesPentendes[i].NumeroContainer, ctesPentendes[i].CodigoRemetente, ctesPentendes[i].CodigoTomador);
                    if (cargaPedido == null)
                        cargaPedido = repCargaPedido.BuscarCargaPedidoParaVincularCTesPorNumeroBooking(ctesPentendes[i].NumeroBooking, ctesPentendes[i].NumeroContainer);
                    if (cargaPedido == null)
                        cargaPedido = repCargaPedido.BuscarCargaPedidoParaVincularCTesPorNumeroBooking(ctesPentendes[i].NumeroBooking);
                    if (cargaPedido != null)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = repCTe.BuscarPorCodigo(ctesPentendes[i].Codigo);

                        if (cteConvertido != null)
                        {
                            unidadeTrabalho.Start();

                            if (cargaPedido != null && cargaPedido.Carga != null)                            
                                svcCTe.SalvarInformacoesMultiModal(cteConvertido, cargaPedido, cteConvertido.ValorAReceber, unidadeTrabalho);                                                            

                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                            {
                                CargaPedido = cargaPedido,
                                CTe = cteConvertido
                            };

                            cteConvertido.CTeSemCarga = false;

                            repCTe.Atualizar(cteConvertido);
                            repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);

                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, null, "Vinculado automaticamente CT-e emitido no embarcador. Chave: " + cteConvertido.Chave, unidadeTrabalho);
                            
                            serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unidadeTrabalho.StringConexao);

                            unidadeTrabalho.CommitChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }
    }
}
