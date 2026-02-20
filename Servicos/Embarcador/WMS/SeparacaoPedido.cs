using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.WMS
{
    public class SeparacaoPedido
    {
        public static void VerificarPedidoIntegracoesPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao repSeparacaoPedidoIntegracao = new Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao(unitOfWork);
            Repositorio.Embarcador.WMS.SeparacaoPedido repSeparacaoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao> separacaoPedidoIntegracoes = repSeparacaoPedidoIntegracao.BuscarIntegracaoPendente(3, 5, "Codigo", "asc", 20, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao.Individual);
            List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedido> separacaoPedidos = new List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedido>();
            for (int i = 0; i < separacaoPedidoIntegracoes.Count; i++)
            {
                Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao integracaoSeparacao = separacaoPedidoIntegracoes[i];
                if (integracaoSeparacao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ortec)
                {
                    new Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec(unitOfWork).EnviarPedidoSeparacaoOrtec(integracaoSeparacao, tipoServicoMultisoftware);
                }
                else
                {
                    integracaoSeparacao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    integracaoSeparacao.ProblemaIntegracao = "Tipo de integração não implementada";
                    integracaoSeparacao.NumeroTentativas++;
                }

                repSeparacaoPedidoIntegracao.Inserir(integracaoSeparacao);

                if (!separacaoPedidos.Contains(integracaoSeparacao.SeparacaoPedido))
                    separacaoPedidos.Add(integracaoSeparacao.SeparacaoPedido);
            }

            foreach (Dominio.Entidades.Embarcador.WMS.SeparacaoPedido separacaoPedido in separacaoPedidos)
            {
                List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao> separacaoPedidoIntegracaos = repSeparacaoPedidoIntegracao.BuscarPorSeparacaoPedido(separacaoPedido.Codigo);

                if (separacaoPedidoIntegracaos.Any(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao))
                    separacaoPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoPedido.IntegracaoRejeitada;
                else if (separacaoPedidoIntegracaos.Any(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao))
                    separacaoPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoPedido.AguardandoIntegracao;
                else
                    separacaoPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoPedido.Finalizada;

                repSeparacaoPedido.Atualizar(separacaoPedido);
            }
        }

        public static void GerarIntegracoes(Dominio.Entidades.Embarcador.WMS.SeparacaoPedido separacaoPedido, List<Dominio.ObjetosDeValor.Embarcador.WMS.NotasSelecionadasParaIntegracao> codigosNotasSelecionadasParaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.WMS.SeparacaoPedidoPedido repSeparacaoPedidoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedidoPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ortec);

            if (tipo != null)
            {
                if (separacaoPedido.LocalEntrega != null)
                    criarIntegracao(tipo, separacaoPedido, null, null, unitOfWork);
                else
                {
                    List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido> separacaoPedidoPedidos = repSeparacaoPedidoPedido.BuscarPorSeparacaoPedido(separacaoPedido.Codigo);
                    foreach (Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido separacaoPedidoPedido in separacaoPedidoPedidos)
                    {
                        if (separacaoPedido?.SelecionarNotasParaIntegracao ?? false && codigosNotasSelecionadasParaIntegracao.Count > 0)
                        {
                            List<int> codigosPedidoNotasFiscais = new List<int>();
                            foreach (Dominio.ObjetosDeValor.Embarcador.WMS.NotasSelecionadasParaIntegracao nota in codigosNotasSelecionadasParaIntegracao)
                                if (nota.CodigoPedido == separacaoPedidoPedido.Pedido.Codigo)
                                    codigosPedidoNotasFiscais.Add(nota.CodigoNF);

                            if (codigosPedidoNotasFiscais.Count == 0)
                            {
                                repSeparacaoPedidoPedido.Deletar(separacaoPedidoPedido);
                                continue;
                            }

                            if (separacaoPedido.SelecionarNotasParaIntegracao && codigosPedidoNotasFiscais.Count > 0)
                            {
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscaisSelecionadas = repPedidoXMLNotaFiscal.BuscarPorCodigos(codigosPedidoNotasFiscais);
                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal xmlNotaFiscal in notasFiscaisSelecionadas)
                                    criarIntegracao(tipo, separacaoPedido, xmlNotaFiscal.XMLNotaFiscal, separacaoPedidoPedido, unitOfWork);
                            }
                        }
                        else
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repPedidoXMLNotaFiscal.BuscarNotasFiscaisPorPedido(separacaoPedidoPedido.Pedido.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in notasFiscais)
                                criarIntegracao(tipo, separacaoPedido, xmlNotaFiscal, separacaoPedidoPedido, unitOfWork);
                        }
                    }
                }
            }
            else
            {
                separacaoPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoPedido.Finalizada;
            }
        }

        public static void GerarOcorrenciaReentregaSeparacao(Dominio.Entidades.Embarcador.WMS.SeparacaoPedido separacaoPedido, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.WMS.SeparacaoPedidoPedido repSeparacaoPedidoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedidoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (configuracaoTMS.TipoDeOcorrenciaReentrega != null)
            {
                Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente = Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido> separacaoPedidoPedidos = repSeparacaoPedidoPedido.BuscarPorSeparacaoPedido(separacaoPedido.Codigo);
                foreach (Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido separacaoPedidoPedido in separacaoPedidoPedidos)
                {
                    Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarPedidoOcorrenciaColetaEntrega(separacaoPedidoPedido.Pedido.ObterTomador(), separacaoPedidoPedido.Pedido, null, configuracaoTMS.TipoDeOcorrenciaReentrega, configuracaoPortalCliente, DateTime.Now, "", "", 0, configuracaoTMS, clienteMultisoftware, unitOfWork);
                }
            }
        }

        public static void AtualizarSituacaoNotasFiscais(Dominio.Entidades.Embarcador.WMS.SeparacaoPedido separacaoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal situacaoNota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.SeparacaoPedidoPedido repSeparacaoPedidoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedidoPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido> separacaoPedidoPedidos = repSeparacaoPedidoPedido.BuscarPorSeparacaoPedido(separacaoPedido.Codigo);
            foreach (Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido separacaoPedidoPedido in separacaoPedidoPedidos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repPedidoXMLNotaFiscal.BuscarNotasFiscaisPorPedido(separacaoPedidoPedido.Pedido.Codigo);
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in notasFiscais)
                {
                    xmlNotaFiscal.SituacaoEntregaNotaFiscal = situacaoNota;
                    repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                }
            }

        }


        private static void criarIntegracao(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo, Dominio.Entidades.Embarcador.WMS.SeparacaoPedido separacaoPedido, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido separacaoPedidoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao repSeparacaoPedidoIntegracao = new Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao separacaoPedidoIntegracao = repSeparacaoPedidoIntegracao.BuscarPorNotaESeparacaoPedido(xMLNotaFiscal.Codigo, separacaoPedido.Codigo);

            if (separacaoPedidoIntegracao == null)
            {
                separacaoPedidoIntegracao = new Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao();
                separacaoPedidoIntegracao.TipoIntegracao = tipo;
                separacaoPedidoIntegracao.DataIntegracao = DateTime.Now;
                separacaoPedidoIntegracao.ProblemaIntegracao = "";
                separacaoPedidoIntegracao.SeparacaoPedido = separacaoPedido;
                separacaoPedidoIntegracao.XMLNotaFiscal = xMLNotaFiscal;
                separacaoPedidoIntegracao.SeparacaoPedidoPedido = separacaoPedidoPedido;
                separacaoPedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                repSeparacaoPedidoIntegracao.Inserir(separacaoPedidoIntegracao);
            }
        }


    }
}
