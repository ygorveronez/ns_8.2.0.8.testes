using System;
using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;

namespace Servicos.Embarcador.Integracao.Emillenium
{
    /// <summary>
    /// Classe que cria as cargas na integração com a E-Milleninum. WIP.
    /// </summary>
    class CriadorCargaIntegracaoEmillenium
    {

        Repositorio.UnitOfWork unitOfWork;
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware;

        public CriadorCargaIntegracaoEmillenium(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            this.unitOfWork = unitOfWork;
            this.tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        public void CriarOuAdicionarNaCarga(string numeroCarga, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos, out Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            var repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            var repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            
            var configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();
            carga = repCarga.BuscarPorCodigoCargaEmbarcador(numeroCarga, 0);

            if (carga == null)
            {
                try
                {
                    carga = CriarCargaParaPedido(listaPedidos, configuracaoTMS);
                    carga.CodigoCargaEmbarcador = numeroCarga;
                    repCarga.Atualizar(carga);

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                    auditado.Texto = "";
                    auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
                    auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "gerou a carga automaticamente via integração e-millenium", unitOfWork);
                }
                catch (Exception e)
                {
                    throw new ServicoException($"Erro ao criar a carga: {e.Message}");
                }
            }
            else
            {
                throw new ServicoException($"Carga {numeroCarga} já existente.");
            }
            //else
            //{
            //    try
            //    {
            //        Servicos.Embarcador.Carga.CargaPedido.AdicionarPedidoCarga(carga, pedido, NumeroReboque.SemReboque, TipoCarregamentoPedido.Normal, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);
            //    } catch (Exception e)
            //    {
            //        throw new ServicoException($"Erro ao adicionar pedido na carga: {e.Message}");
            //    }
            //}
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga CriarCargaParaPedido(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Pedido.Pedido.CriarCarga(out var carga, listaPedidos, unitOfWork, tipoServicoMultisoftware, null, configuracaoTMS, forcarGeracaoCarga: true);
            return carga;
        }

    }
}
