using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class PedidoVinculado
    {
        #region Métodos Públicos

        public static void AjustarCargaEncaixada(Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete serCargaComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo repPedagioEstadoBaseCalculo = new Repositorio.Embarcador.ICMS.PedagioEstadoBaseCalculo(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargaCTeParaSubContratacaoNotasFiscais = repCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorCarga(cargaOrigem.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaOrigem.Codigo);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork).BuscarConfiguracaoPadrao();

            if (cargaOrigem.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && cargaOrigem.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga retornoRotas = serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(cargaOrigem, cargaPedidos, unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
                Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(cargaOrigem, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware);

                serRateioFrete.RatearFreteEntrePedidos(cargaOrigem, cargaPedidos, configuracao, cargaOrigem.ValorFrete, false, tipoServicoMultisoftware, unitOfWork);

                serCargaDadosSumarizados.AtualizarTiposDocumentos(cargaOrigem, cargaPedidos, unitOfWork);


                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretesDiretos = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
                List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> pedagioEstadosBaseCalculo = new List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo>();
                List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> tabelaAliquotas = new List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
                List<Dominio.Entidades.Cliente> tomadoresFilialEmissora = new List<Dominio.Entidades.Cliente>();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosFilialEmissoraComponentesFreteCargaImpostos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete>();
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteICMS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteISS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFretePisCONFIS = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> cargaCTesParaSubContratacao = repPedidoCTeParaSubcontratacao.BuscarPorCarga(cargaOrigem.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaETipo(cargaOrigem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS, null, false);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaETipo(cargaOrigem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PISCONFIS, null, false);


                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosFreteCargaImpostos = repCargaPedidoComponenteFrete.BuscarPorCargaComponentesImpostos(cargaOrigem.Codigo, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos = repCargaPedidoContaContabilContabilizacao.BuscarPorCarga(cargaOrigem.Codigo);
                if (cargaOrigem.EmpresaFilialEmissora != null)
                {
                    serRateioFrete.ZerarValoresDaCarga(cargaOrigem, true, unitOfWork);
                    cargaPedidosComponentesFreteCarga = repCargaPedidoComponenteFrete.BuscarPorCarga(cargaOrigem.Codigo, true);
                    pedagioEstadosBaseCalculo = repPedagioEstadoBaseCalculo.BuscarPorEstados((from obj in cargaPedidos select obj.Origem.Estado.Sigla).Distinct().ToList());
                    cargaPedidoProdutos = serCargaICMS.ObterProdutosCargaContidosEmRegras(cargaOrigem, unitOfWork);
                    tomadoresFilialEmissora = Servicos.Embarcador.Carga.FilialEmissora.ObterTomadoresFilialEmissora((from obj in cargaPedidos where obj.CargaPedidoFilialEmissora select obj.CargaOrigem.EmpresaFilialEmissora.CNPJ_SemFormato).Distinct().ToList(), unitOfWork);
                    cargaPedidosFilialEmissoraComponentesFreteCargaImpostos = repCargaPedidoComponenteFrete.BuscarPorCargaComponentesImpostos(cargaOrigem.Codigo, true);
                }
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = Servicos.Embarcador.Carga.Carga.ObterCargasOrigem(cargaOrigem, unitOfWork);

                bool possuiTrechoAnterior = false;

                bool abriuTransacao = false;
                if (!unitOfWork.IsActiveTransaction())
                {
                    unitOfWork.Start();
                    abriuTransacao = true;
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cp in cargaPedidos)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigemAgrupamento = (from obj in cargasOrigem where obj.Codigo == cp.CargaOrigem.Codigo select obj).FirstOrDefault();
                    if (cp.ValorFreteAPagar > 0)
                        Servicos.Embarcador.Carga.FreteFilialEmissora.SetarFreteEmbarcadorFilialEmissora(ref cargaOrigem, cargaOrigemAgrupamento, cp, tipoServicoMultisoftware, true, unitOfWork, configuracao, cargaPedidosComponentesFreteCarga, pedagioEstadosBaseCalculo, cargaPedidoProdutos, tabelaAliquotas, tomadoresFilialEmissora, cargaPedidosFilialEmissoraComponentesFreteCargaImpostos, componenteFreteICMS, out possuiTrechoAnterior, configuracaoGeralCarga);

                    if (cp.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro
                        || cp.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada
                        || cp.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada
                        || cp.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho
                        || cp.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio
                        || cp.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario)
                        AplicarRateioCTesSubContratacaoCargaPedido(cargaOrigem, cp, cargaPedidoContaContabilContabilizacaos, unitOfWork, tipoServicoMultisoftware, componenteFreteICMS, componenteFretePisCONFIS, cargaCTeParaSubContratacaoNotasFiscais, cargaCTesParaSubContratacao, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);

                    serRateioFrete.GerarComponenteICMS(cp, false, componenteFreteICMS, cargaPedidosFreteCargaImpostos, unitOfWork);
                    serRateioFrete.GerarComponenteISS(cp, componenteFreteISS, cargaPedidosFreteCargaImpostos, false, unitOfWork);
                    serRateioFrete.GerarComponentePisCofins(cp, componenteFretePisCONFIS, cargaPedidosFreteCargaImpostos, unitOfWork);
                }

                if (abriuTransacao)
                    unitOfWork.CommitChanges();

                serRateioFrete.GerarComponenteICMS(cargaOrigem, cargaPedidos, false, false, unitOfWork);
                serRateioFrete.GerarComponenteISS(cargaOrigem, cargaPedidos, unitOfWork);
                serRateioFrete.GerarComponentePisCofins(cargaOrigem, cargaPedidos, false, unitOfWork);

                if (possuiTrechoAnterior)
                    Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref cargaOrigem, false, tipoServicoMultisoftware, unitOfWork, configuracao);

                if (cargaOrigem.EmpresaFilialEmissora != null)
                {
                    serCargaComponetesFrete.AdicionarComponentesCargaAgrupada(cargaOrigem, true, cargaPedidosComponentesFreteCarga, unitOfWork);
                    serRateioFrete.AcrescentarValoresDaCargaAgrupada(cargaOrigem, true, cargaPedidos, unitOfWork);
                }

                //if (cargaOrigem.EmpresaFilialEmissora != null)
                //    serRateioFrete.RatearValorDoFrenteEntrePedidos(cargaOrigem, cargaPedidos, configuracao, true, unitOfWork, tipoServicoMultisoftware);

                repCarga.Atualizar(cargaOrigem);
                serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaOrigem, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware);
            }
        }

        public static void CriarCargaDeEncaixe(Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem, Dominio.Entidades.Embarcador.Cargas.Carga cargaEncaixar, double cpfCnpjExpedidor, double cpfCnpjRecebedor, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Carga servicoCarga = new Carga(unitOfWork);

            if (!servicoCarga.VerificarSeCargaEstaNaLogistica(cargaOrigem, tipoServicoMultisoftware))
                throw new ServicoException($"Não é possível encaixar pedidos na atual situação da carga ({cargaOrigem.DescricaoSituacaoCarga}).");


            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCTe.BuscarPorCarga(cargaEncaixar.Codigo, true);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                EncaixarCargaPedido(cargaCTe, cargaOrigem, cpfCnpjExpedidor, cpfCnpjRecebedor, unitOfWork, tipoServicoMultisoftware, configuracao, configuracaoGeralCarga);

            AjustarCargaEncaixada(cargaOrigem, configuracao, unitOfWork, tipoServicoMultisoftware);
        }

        public static void CriarPedidoDeEncaixe(Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            if (cargaOrigem == null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<int> codigosCargaPedidoEncaixar = repositorioCargaPedido.BuscarCodigosPorCargaEncaixar(cargaOrigem.CodigoCargaEmbarcador);

            if (codigosCargaPedidoEncaixar.Count == 0)
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiraCargaPedido = repositorioCargaPedido.BuscarPrimeiraPorCarga(cargaOrigem.Codigo);
            double cpfCnpjExpedidor = primeiraCargaPedido?.Expedidor?.CPF_CNPJ ?? 0d;
            double cpfCnpjRecebedor = 0d;

            CriarPedidoDeEncaixe(cargaOrigem, codigosCargaPedidoEncaixar, cpfCnpjExpedidor, cpfCnpjRecebedor, unitOfWork, tipoServicoMultisoftware, configuracao, configuracaoGeralCarga);
            AjustarCargaEncaixada(cargaOrigem, configuracao, unitOfWork, tipoServicoMultisoftware);
        }

        public static void CriarPedidoDeEncaixe(Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem, List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentosCargaEncaixar, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga in preAgrupamentosCargaEncaixar)
                CriarPedidoDeEncaixe(cargaOrigem, new List<int>() { preAgrupamentoCarga.CargaPedidoEncaixe.Codigo }, preAgrupamentoCarga.CnpjExpedidor.ToDouble(), preAgrupamentoCarga.CnpjRecebedor.ToDouble(), unitOfWork, tipoServicoMultisoftware, configuracao, configuracaoGeralCarga);

            if (preAgrupamentosCargaEncaixar.Count > 0)
                AjustarCargaEncaixada(cargaOrigem, configuracao, unitOfWork, tipoServicoMultisoftware);
        }

        public static void ValidarPermitePedidoDeEncaixe(List<int> codigosCargaPedidoEncaixar, double cpfCnpjExpedidor, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Carga servicoCarga = new Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCargaCTesPorCargaPedidoDaCarga(codigosCargaPedidoEncaixar, "", "", 0, 0);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                PermiteEncaixarCargaPedido(cargaCTe, cpfCnpjExpedidor, unitOfWork, tipoServicoMultisoftware, configuracao);
        }

        public static void CriarPedidoDeEncaixe(Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem, List<int> codigosCargaPedidoEncaixar, double cpfCnpjExpedidor, double cpfCnpjRecebedor, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Carga servicoCarga = new Carga(unitOfWork);

            if (!servicoCarga.VerificarSeCargaEstaNaLogistica(cargaOrigem, tipoServicoMultisoftware))
                throw new ServicoException($"Não é possível encaixar pedidos na atual situação da carga ({cargaOrigem.DescricaoSituacaoCarga}).");

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCargaCTesPorCargaPedidoDaCarga(codigosCargaPedidoEncaixar, "", "", 0, 0);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                EncaixarCargaPedido(cargaCTe, cargaOrigem, cpfCnpjExpedidor, cpfCnpjRecebedor, unitOfWork, tipoServicoMultisoftware, configuracao, configuracaoGeralCarga);
        }

        public static void VerificarSeExistePraAgrupamentoPedidoEncaixe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.Empresa?.PermiteEmitirSubcontratacao ?? false)
            {
                Integracao.Ortec.IntegracaoOrtec servicoIntegracaoOrtec = new Integracao.Ortec.IntegracaoOrtec(unitOfWork);

                if (servicoIntegracaoOrtec.IsPossuiIntegracaoOrtec())
                    servicoIntegracaoOrtec.VincularCargaAosAgrupamentosPorPedidoEncaixe(carga);
            }
        }

        #endregion

        #region Métodos Privados

        private static void AplicarRateioCTesSubContratacaoCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContaContabilContabilizacaos, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteICMS, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componentePisCofins, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> cargapedidoCTeParaSubContratacaoNotasFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> cargaCTesParaSubContratacao, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesICMSXMLNotaFiscalExistenteCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> componentesPisConfisXMLNotaFiscalExistenteCarga)
        {

            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Servicos.Embarcador.Carga.RateioCTeParaSubcontratacao serRateioCTeParaSubcontratacao = new Servicos.Embarcador.Carga.RateioCTeParaSubcontratacao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentes = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFrete = repCargaPedidoComponentesFrete.BuscarPorCargaPedido(cargaPedido.Codigo, false, null, false);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in cargaPedidoComponentesFrete)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFreteDinamico = cargaPedidoComponenteFrete.ConvertarParaComponenteDinamico();
                componentes.Add(componenteFreteDinamico);
            }

            bool cteAnteriorFilialEmissora = false;
            serRateioCTeParaSubcontratacao.RatearValorCTeSucontratacaoDoPedido(cargaOrigem, cargaPedido, cargaPedido.ValorFrete, cargaPedido.ValorTotalMoeda ?? 0m, componentes, cargaPedido.FormulaRateio, cteAnteriorFilialEmissora, cargaPedidoContaContabilContabilizacaos, tipoServicoMultisoftware, unitOfWork, componenteICMS, componentePisCofins, cargapedidoCTeParaSubContratacaoNotasFiscais, cargaCTesParaSubContratacao, componentesICMSXMLNotaFiscalExistenteCarga, componentesPisConfisXMLNotaFiscalExistenteCarga);
        }

        private static void EncaixarCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.Cargas.Carga carga, double cpfCnpjExpedidor, double cpfCnpjRecebedor, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCTe.Codigo);
            bool emitirPorPedidoAgrupado = false;

            if (cargaPedidosCTe.Count > 1)
                emitirPorPedidoAgrupado = true;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosCTe)
            {
                if (!repCargaPedido.BuscarSePedidoJaFoiEncaixadoNaCarga(cargaPedido.Pedido.NumeroPedidoEmbarcador, carga.Codigo))
                    EncaixarPedido(cargaPedido, carga, cpfCnpjExpedidor, cpfCnpjRecebedor, unitOfWork, tipoServicoMultisoftware, configuracao, emitirPorPedidoAgrupado, configuracaoGeralCarga);
            }
        }

        private static void PermiteEncaixarCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, double cpfCnpjExpedidor, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCTe.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoValidar in cargaPedidosCTe)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedidoValidar.Pedido;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarSePedidoJaFoiEncaixadoPorExpedidor(pedido.NumeroPedidoEmbarcador, pedido.Filial?.Codigo ?? 0, cpfCnpjExpedidor);

                if (cargaPedido != null && !cargaPedido.Pedido.ReentregaSolicitada)
                    throw new ServicoException($"O pedido {pedido.NumeroPedidoEmbarcador} já foi encaixado na viagem {cargaPedido.Carga.CodigoCargaEmbarcador} com expedição deste mesmo local.");

            }
        }

        //private static Dominio.Entidades.Embarcador.Cargas.CargaPedido EncaixarPedido(int taraContainer, int codigoContainer, string lacreContainerUm, string lacreContainerDois, string lacreContainerTres, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoEncaixe, string numeroPedido, Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem, double expedidor, double recebedor, ref string retorno, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        //{
        //    Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
        //    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
        //    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
        //    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
        //    Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

        //    Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);

        //    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedidoEncaixe.Pedido.Clonar();

        //    cargaPedidoEncaixe.Pedido.ControleNumeracao = cargaPedidoEncaixe.Pedido.Codigo;
        //    repPedido.Atualizar(cargaPedidoEncaixe.Pedido);

        //    pedido.NumeroPedidoEmbarcador = numeroPedido;
        //    if (expedidor > 0)
        //        pedido.Expedidor = repCliente.BuscarPorCPFCNPJ(expedidor);

        //    bool recebedorPedidoOrigem = false;

        //    if (recebedor > 0)
        //        pedido.Recebedor = repCliente.BuscarPorCPFCNPJ(recebedor);
        //    else
        //    {
        //        //pedido.Recebedor = null;
        //        recebedorPedidoOrigem = true;
        //    }


        //    Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
        //    Utilidades.Object.SetarPropriedadesObjetoDuplicar(pedido);
        //    pedido.CodigoCargaEmbarcador = cargaOrigem.CodigoCargaEmbarcador;
        //    pedido.Filial = cargaOrigem.Filial;
        //    pedido.AdicionadaManualmente = false;
        //    if (codigoContainer > 0)
        //        pedido.Container = repContainer.BuscarPorCodigo(codigoContainer);
        //    if (taraContainer > 0)
        //        pedido.TaraContainer = Utilidades.String.OnlyNumbers(taraContainer.ToString("n0"));
        //    pedido.LacreContainerDois = lacreContainerDois;
        //    pedido.LacreContainerTres = lacreContainerTres;
        //    pedido.LacreContainerUm = lacreContainerUm;

        //    repPedido.Inserir(pedido);


        //    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarSePedidoJaFoiEncaixadoPorExpedidor(pedido.NumeroPedidoEmbarcador, pedido.Filial?.Codigo ?? 0, expedidor);
        //    if (cargaPedido == null)
        //    {

        //        cargaPedido = Servicos.Embarcador.Carga.CargaPedido.CriarCargaPedido(cargaOrigem, pedido, cargaPedidoEncaixe, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware, configuracao);

        //        if (pedido.Expedidor != null && (pedido.Recebedor != null && !recebedorPedidoOrigem))
        //        {
        //            if (cargaPedido.Carga.GrupoPessoaPrincipal != null && cargaPedido.Carga.GrupoPessoaPrincipal.EmitirSempreComoRedespacho)
        //                cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
        //            else
        //                cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
        //        }
        //        else if (pedido.Expedidor != null || pedido.Recebedor != null)
        //            cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
        //        else if (repCargaPedido.VerificarSePedidoTeveRedespachoIntermediario(pedido.NumeroPedidoEmbarcador, expedidor))
        //            cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
        //        else
        //            cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

        //        if (configuracao.UtilizaEmissaoMultimodal && cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Subcontratacao)
        //            cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

        //        bool incluirICMS = true;
        //        decimal percentualIncluir = 100;
        //        Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = regraICMS = serCargaICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, cargaPedido.Carga.Empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirICMS, ref percentualIncluir, 0, null, unitOfWork, tipoServicoMultisoftware, configuracao);
        //        cargaPedido.IncluirICMSBaseCalculo = incluirICMS;
        //        cargaPedido.PercentualIncluirBaseCalculo = percentualIncluir;
        //        cargaPedido.PercentualAliquota = regraICMS.Aliquota;
        //        cargaPedido.CST = regraICMS.CST;
        //        cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);
        //        cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
        //        cargaPedido.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
        //        cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
        //        cargaPedido.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;

        //        VincularCTesCargaEnxaixe(cargaPedido, cargaPedidoEncaixe, unitOfWork, tipoServicoMultisoftware);

        //        //if (cargaOrigem.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && cargaOrigem.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
        //        serCTeSubContratacao.CriarNotasFiscaisDaCargaPedido(cargaPedido, tipoServicoMultisoftware, unitOfWork);

        //        return cargaPedido;

        //    }
        //    else
        //    {
        //        retorno = "O pedido  " + pedido.NumeroPedidoEmbarcador + ", já foi encaixado na viagem " + cargaPedido.Carga.CodigoCargaEmbarcador + " com expedição deste mesmo local.";
        //        return null;
        //    }
        //}

        private static void EncaixarPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoEncaixe, Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem, double cpfCnpjExpedidor, double cpfCnpjRecebedor, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool emitirPorPedidoAgrupado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedidoEncaixe.Pedido;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarSePedidoJaFoiEncaixadoPorExpedidor(pedido.NumeroPedidoEmbarcador, cargaOrigem.Filial?.Codigo ?? 0, cpfCnpjExpedidor);

            if (cargaPedido != null && !pedido.ReentregaSolicitada)
                throw new ServicoException($"O pedido {pedido.NumeroPedidoEmbarcador} já foi encaixado na viagem {cargaPedido.Carga.CodigoCargaEmbarcador} com expedição deste mesmo local.");

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);

            Dominio.Entidades.Cliente recebedorOrigem = pedido.Recebedor;
            Dominio.Entidades.Cliente expedidorOrigem = pedido.Expedidor;

            pedido.NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador;
            if (cpfCnpjExpedidor > 0)
                pedido.Expedidor = repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidor);

            bool recebedorPedidoOrigem = false;

            if (cpfCnpjRecebedor > 0)
                pedido.Recebedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor);
            else
                recebedorPedidoOrigem = true;

            if (pedido.ReentregaSolicitada)
                pedido.ReentregaSolicitada = false;

            repPedido.Atualizar(pedido);

            cargaPedido = Servicos.Embarcador.Carga.CargaPedido.CriarCargaPedido(cargaOrigem, pedido, cargaPedidoEncaixe, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware, configuracao, true, configuracaoGeralCarga);

            if (pedido.Expedidor != null && (pedido.Recebedor != null && !recebedorPedidoOrigem))
            {
                if (cargaPedido.Carga.GrupoPessoaPrincipal != null && cargaPedido.Carga.GrupoPessoaPrincipal.EmitirSempreComoRedespacho)
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                else
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
            }
            else if (pedido.Expedidor != null || pedido.Recebedor != null)
                cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
            else if (repCargaPedido.VerificarSePedidoTeveRedespachoIntermediario(pedido.NumeroPedidoEmbarcador, cpfCnpjExpedidor))
                cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
            else
                cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

            if ((configuracao.UtilizaEmissaoMultimodal && cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Subcontratacao) || (cargaPedido.Carga.TipoOperacao?.SempreEmitirSubcontratacao ?? false))
                cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

            if (emitirPorPedidoAgrupado)
                cargaPedido.TipoRateio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado;

            bool incluirICMS = true;
            decimal percentualIncluir = 100;

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serCargaICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, cargaPedido.Carga.Empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirICMS, ref percentualIncluir, 0, null, unitOfWork, tipoServicoMultisoftware, configuracao);

            cargaPedido.IncluirICMSBaseCalculo = incluirICMS;
            cargaPedido.PercentualIncluirBaseCalculo = percentualIncluir;
            cargaPedido.PercentualAliquota = regraICMS.Aliquota;
            cargaPedido.CST = regraICMS.CST;
            cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);
            cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
            cargaPedido.DescontarICMSDoValorAReceber = regraICMS.DescontarICMSDoValorAReceber;
            cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao = regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao;
            cargaPedido.NaoImprimirImpostosDACTE = regraICMS.NaoImprimirImpostosDACTE;
            cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte = regraICMS.NaoEnviarImpostoICMSNaEmissaoCte;
            cargaPedido.SetarRegraICMS(regraICMS.CodigoRegra);

            Servicos.Log.TratarErro($"2 - CargaPedido: {cargaPedido.Codigo} -> Aliquota: {cargaPedido.PercentualAliquota}", "ProcessarAliquota");

            VincularCTesCargaEnxaixe(cargaPedido, cargaPedidoEncaixe, unitOfWork, tipoServicoMultisoftware, configuracao);
                        
            pedido.Recebedor = recebedorOrigem;
            pedido.Expedidor = expedidorOrigem;
            repPedido.Atualizar(pedido);
        }

        private static void VincularCTesCargaEnxaixe(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoEncaixe, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);
            Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorCargaPedidoDaCarga(cargaPedidoEncaixe.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedidoEncaixe.Codigo);

            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            decimal peso = 0;
            for (int i = 0; i < ctes.Count; i++)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];
                if (repPedidoCTeParaSubContratacao.ContarPorCargaPedidoEChave(cargaPedido.Codigo, cte.Chave) <= 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe dynCTe = serCTe.ConverterEntidadeCTeParaObjeto(cte, enviarCTeApenasParaTomador, unitOfWork);
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                    serCTeSubContratacao.InformarDadosCTeNaCarga(unitOfWork, dynCTe, cargaPedido, tipoServicoMultisoftware, ref pedidoCTeParaSubContratacao, true);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalCTe in cte.XMLNotaFiscais)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = pedidoXMLNotaFiscals.Find(obj => obj.XMLNotaFiscal.Codigo == xmlNotaFiscalCTe.Codigo);
                        if (pedidoXMLNotaFiscal != null)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal;
                            peso += xmlNotaFiscal.Peso;
                            serCTeSubContratacao.CriarPedidoCTeParaSubContratacaoNotaFiscal(cargaPedido, xmlNotaFiscal, serCTeSubContratacao.ConverterPedidoCTeParaSubContratacao(pedidoCTeParaSubContratacao), true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao, unitOfWork, tipoServicoMultisoftware, configuracao, new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> { pedidoXMLNotaFiscal });
                        }
                    }
                }
            }

            if (!configuracao.AtualizarProdutosCarregamentoPorNota)
            {
                //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {peso}. PedidoVinculado.VincularCTesCargaEnxaixe", "PesoCargaPedido");
                cargaPedido.Peso = peso;
            }

            cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;
            repCargaPedido.Atualizar(cargaPedido);

            //cargaPedido.Pedido.PesoTotal = peso;
            //repPedido.Atualizar(cargaPedido.Pedido);
        }

        #endregion
    }
}
