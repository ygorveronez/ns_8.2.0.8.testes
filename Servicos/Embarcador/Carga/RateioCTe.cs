using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Carga
{
    public class RateioCTe : ServicoBase
    {
        
        public RateioCTe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region MétodosPublicos

        public void AtualizarRateiosPorCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = RecriarRateioEntreNotasPorCargaCTe(cargaCTe, unitOfWork, tipoServicoMultisoftware);
            bool incluirICMSnoFrete = cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            RecriarRateioEntrePedidos(cargaPedidos, cargaCTe.Carga, incluirICMSnoFrete, cargaCTe.CTe.CST, cargaCTe.CTe.PercentualICMSIncluirNoFrete, cargaCTe.CTe.PercentualReducaoBaseCalculoICMS, cargaCTe.CTe.AliquotaICMS, unitOfWork);
            AjustarValoresCarga(cargaCTe.Carga, unitOfWork);
        }

        public void AlterarDadosCarga(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultiSoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);
            List<Dominio.Entidades.Veiculo> veiculos = new List<Dominio.Entidades.Veiculo>();
            List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTe.CTe;
                carga.Empresa = cargaCTe.CTe.Empresa;
                foreach (Dominio.Entidades.VeiculoCTE veiculoCTe in cte.Veiculos)
                {
                    if (!veiculos.Contains(veiculoCTe.Veiculo))
                        veiculos.Add(veiculoCTe.Veiculo);
                }
                foreach (Dominio.Entidades.MotoristaCTE motoristaCTe in cte.Motoristas)
                {
                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorCPF(Utilidades.String.OnlyNumbers(motoristaCTe.CPFMotorista));
                    if (motorista != null)
                    {
                        if (!motoristas.Contains(motorista))
                            motoristas.Add(motorista);
                    }
                }
            }


            if (veiculos.Count > 0)
            {
                carga.VeiculosVinculados.Clear();
                bool encontrouTracao = false;
                List<Dominio.Entidades.Veiculo> reboques = new List<Dominio.Entidades.Veiculo>();
                foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
                {
                    if (veiculo.TipoVeiculo == "0" || !encontrouTracao)
                    {
                        encontrouTracao = true;
                        carga.Veiculo = veiculo;
                    }
                    else
                    {
                        reboques.Add(veiculo);
                    }
                }
                if (encontrouTracao)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in reboques)
                        carga.VeiculosVinculados.Add(reboque);
                }
                else
                {
                    Dominio.Entidades.Veiculo tracao = reboques.FirstOrDefault();
                    for (int i = 1; i < reboques.Count(); i++)
                        carga.VeiculosVinculados.Add(reboques[i]);
                }
                serCarga.AlterarSegmentoCarga(ref carga);
            }

            if (motoristas.Count > 0)
                servicoCargaMotorista.AtualizarMotoristas(carga, motoristas);

            if (tipoServicoMultiSoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                serCarga.AtualizarVeiculoEMotoristasPedidos(carga, auditado, unitOfWork);
        }

        public void AjustarFretePorCTes(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCTe = RecriarRateioEntreNotasPorCargaCTe(cargaCTe, unitOfWork, tipoServicoMultisoftware);

                if (cargaPedidosCTe == null || cargaPedidosCTe.Count == 0)
                    continue;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoCTe in cargaPedidosCTe)
                    if (!cargaPedidos.Any(o => o.Codigo == cargaPedidoCTe.Codigo))
                        cargaPedidos.Add(cargaPedidoCTe);
            }

            if (cargaPedidos == null || cargaPedidos.Count == 0)
                return;

            RecriarRateioEntrePedidos(cargaPedidos, cargaPedido.Carga, cargaCTes.Any(o => o.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim), cargaCTes.Select(o => o.CTe.CST).FirstOrDefault(), cargaCTes.Select(o => o.CTe.PercentualICMSIncluirNoFrete).FirstOrDefault(), cargaCTes.Select(o => o.CTe.PercentualReducaoBaseCalculoICMS).FirstOrDefault(), cargaCTes.Select(o => o.CTe.AliquotaICMS).FirstOrDefault(), unitOfWork);
            AjustarValoresCarga(cargaPedido.Carga, unitOfWork);
        }

        public void CriarDocumentosCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoDeTransporteAnteriorCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.CTe.DocumentoCTe serDocumentoCTe = new Embarcador.CTe.DocumentoCTe(unitOfWork);

            Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);

            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF;
            cargaPedido.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;

            Servicos.Embarcador.CTe.CTe serCTe = new Embarcador.CTe.CTe(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                List<Dominio.Entidades.DocumentosCTE> documentos = repDocumentosCTe.BuscarPorCTe(cargaCTe.CTe.Codigo);
                cargaCTe.CTe.XMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                foreach (Dominio.Entidades.DocumentosCTE documento in documentos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = serDocumentoCTe.CriarDocumentoParaXMLNotaFiscal(cargaCTe, documento, unitOfWork);

                    if (cargaPedido.Pedido.Remetente.CPF_CNPJ != xmlNotaFiscal.Emitente.CPF_CNPJ)
                    {
                        cargaPedido.Pedido.Remetente = xmlNotaFiscal.Emitente;
                        cargaPedido.Origem = xmlNotaFiscal.Emitente.Localidade;
                    }

                    if (cargaCTe.CTe.Expedidor != null)
                        cargaPedido.Origem = cargaCTe.CTe.Expedidor.Localidade;

                    if (cargaPedido.Pedido.Destinatario == null || cargaPedido.Pedido.Destinatario.CPF_CNPJ != xmlNotaFiscal.Destinatario.CPF_CNPJ)
                    {
                        cargaPedido.Pedido.Destinatario = xmlNotaFiscal.Destinatario;
                        cargaPedido.Destino = xmlNotaFiscal.Destinatario.Localidade;
                    }
                    if (cargaCTe.CTe.Recebedor != null && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                        cargaPedido.Destino = cargaCTe.CTe.Recebedor.Localidade;

                    repPedido.Atualizar(cargaPedido.Pedido);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe();
                    cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal = serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoDeCarga);
                    cargaPedidoXMLNotaFiscalCTe.CargaCTe = cargaCTe;
                    repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTe);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, "Adicionado via CT-e manual", unitOfWork);

                    cargaCTe.CTe.XMLNotaFiscais.Add(xmlNotaFiscal);

                }
                repCTe.Atualizar(cargaCTe.CTe);

                List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosAnterior = repDocumentoDeTransporteAnteriorCTe.BuscarPorCTe(cargaCTe.CTe.Codigo);
                foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe docAnterior in documentosAnterior)
                {
                    if (string.IsNullOrWhiteSpace(docAnterior.Chave))
                        continue;

                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterEntidadeCTeParaObjeto(cargaCTe.CTe, enviarCTeApenasParaTomador, unitOfWork);
                    cteIntegracao.Chave = docAnterior.Chave;
                    string numero = docAnterior.Chave.Substring(25, 9);
                    string serie = docAnterior.Chave.Substring(22, 3);
                    cteIntegracao.Numero = int.Parse(numero);
                    cteIntegracao.Serie = serie;
                    cteIntegracao.DataEmissao = docAnterior.DataEmissao.HasValue ? docAnterior.DataEmissao.Value : DateTime.Now;
                    cteIntegracao.Emitente = serEmpresa.ConverterObjetoEmpresa(docAnterior.Emissor);
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                    serCTeSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cteIntegracao, cargaPedido, tipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);
                }
            }
        }

        #endregion

        #region Métodos Privados

        private void RecriarRateioEntrePedidos(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool incluirICMSNoFrete, string cst, decimal percentualICMSIncluirNoFrete, decimal percentualReducaoBaseCalculoICMS, decimal aliquota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            RateioFrete svcRateioFrete = new RateioFrete(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesDeletar = repCargaPedidoComponentesFrete.BuscarPorCargaPedido(cargaPedido.Codigo, false, cargaPedido.ModeloDocumentoFiscal, false);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteDeletar in cargaPedidoComponentesDeletar)
                    repCargaPedidoComponentesFrete.Deletar(cargaPedidoComponenteDeletar);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscalPedido = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
                cargaPedido.ValorFrete = pedidosXMLNotaFiscalPedido.Sum(obj => obj.ValorFrete);
                cargaPedido.ValorICMS = pedidosXMLNotaFiscalPedido.Sum(obj => obj.ValorICMS);
                cargaPedido.BaseCalculoICMS = pedidosXMLNotaFiscalPedido.Sum(obj => obj.BaseCalculoICMS);
                cargaPedido.CST = cst;
                cargaPedido.IncluirICMSBaseCalculo = incluirICMSNoFrete;
                cargaPedido.PercentualAliquota = aliquota;
                cargaPedido.ICMSPagoPorST = cst == "60" ? true : false;
                cargaPedido.PercentualIncluirBaseCalculo = percentualICMSIncluirNoFrete;
                cargaPedido.PercentualReducaoBC = percentualReducaoBaseCalculoICMS;

                Servicos.Log.TratarErro($"3 - CargaPedido: {cargaPedido.Codigo} -> Aliquota: {cargaPedido.PercentualAliquota}", "ProcessarAliquota");

                List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesFreteDinamico = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorCargaPedidoAgrupado(cargaPedido.Codigo, false);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesFreteDinamicoSumarizados = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componente in componentesFreteDinamico)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico compoenteExiste = (from obj in componentesFreteDinamicoSumarizados where obj.ComponenteFrete.Codigo == componente.ComponenteFrete.Codigo select obj).FirstOrDefault();
                    if (compoenteExiste == null)
                        componentesFreteDinamicoSumarizados.Add(componente);
                    else
                        compoenteExiste.ValorComponente += componente.ValorComponente;
                }

                decimal valorTotalComonentes = 0;
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteDinamico in componentesFreteDinamicoSumarizados)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();
                    cargaPedidoComponenteFrete.TipoComponenteFrete = componenteDinamico.TipoComponenteFrete;
                    cargaPedidoComponenteFrete.ValorComponente = componenteDinamico.ValorComponente;
                    cargaPedidoComponenteFrete.ComponenteFrete = componenteDinamico.ComponenteFrete;
                    cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                    cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = componenteDinamico.IncluirBaseCalculoImposto;
                    cargaPedidoComponenteFrete.ComponenteFilialEmissora = componenteDinamico.ComponenteFilialEmissora;
                    cargaPedidoComponenteFrete.Percentual = componenteDinamico.Percentual;
                    cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = componenteDinamico.IncluirIntegralmenteContratoFreteTerceiro;
                    repCargaPedidoComponentesFrete.Inserir(cargaPedidoComponenteFrete);
                    valorTotalComonentes += cargaPedidoComponenteFrete.ValorComponente;
                }

                cargaPedido.ValorFreteAPagar = cargaPedido.ValorFrete + valorTotalComonentes;

                if (cargaPedido.IncluirICMSBaseCalculo && cargaPedido.CST != "60")
                    cargaPedido.ValorFreteAPagar += cargaPedido.ValorICMS;

                repCargaPedido.Atualizar(cargaPedido);

                svcRateioFrete.GerarComponenteICMS(cargaPedido, false, unitOfWork);
            }
        }

        private void AjustarValoresCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            RateioFrete svcRateioFrete = new RateioFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFreteExiste = repCargaComponentesFrete.BuscarPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponeneteExiste in cargaComponentesFreteExiste)
                repCargaComponentesFrete.Deletar(cargaComponeneteExiste);

            if (carga.CargaAgrupada)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in carga.CargasAgrupamento)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFreteAgrupamento = repCargaComponentesFrete.BuscarPorCarga(cargaOrigem.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in cargaComponentesFreteAgrupamento)
                        repCargaComponentesFrete.Deletar(componente);
                }
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            carga.ValorFrete = cargaPedidos.Sum(obj => obj.ValorFrete);
            carga.ValorICMS = cargaPedidos.Sum(obj => obj.ValorICMS);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesFreteDinamicoPedido = repCargaPedidoComponentesFrete.BuscarPorCargaAgrupado(carga.Codigo, false);
            decimal valorTotalComonentesCarga = 0;
            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteDinamico in componentesFreteDinamicoPedido)
            {
                bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(carga.TabelaFrete, componenteDinamico.ComponenteFrete);

                componenteDinamico.ComponenteFrete = repComponenteFrete.BuscarPorCodigo(componenteDinamico.ComponenteFrete.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaCargaComponentesFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();
                cargaCargaComponentesFrete.TipoComponenteFrete = componenteDinamico.TipoComponenteFrete;
                cargaCargaComponentesFrete.ValorComponente = componenteDinamico.ValorComponente;
                cargaCargaComponentesFrete.ComponenteFrete = componenteDinamico.ComponenteFrete;
                cargaCargaComponentesFrete.ComponenteFilialEmissora = componenteDinamico.ComponenteFilialEmissora;
                cargaCargaComponentesFrete.SomarComponenteFreteLiquido = componenteDinamico.ComponenteFrete.SomarComponenteFreteLiquido;
                cargaCargaComponentesFrete.DescontarComponenteFreteLiquido = destacarComponenteTabelaFrete ? carga.TabelaFrete.DescontarComponenteFreteLiquido : componenteDinamico.ComponenteFrete.DescontarComponenteFreteLiquido;
                cargaCargaComponentesFrete.Carga = carga;
                cargaCargaComponentesFrete.IncluirBaseCalculoICMS = componenteDinamico.IncluirBaseCalculoImposto;
                cargaCargaComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro = componenteDinamico.IncluirIntegralmenteContratoFreteTerceiro;
                cargaCargaComponentesFrete.Percentual = componenteDinamico.Percentual;
                
                repCargaComponentesFrete.Inserir(cargaCargaComponentesFrete);

                if (componenteDinamico.ComponenteFrete == null || !componenteDinamico.ComponenteFrete.ComponentePertenceComposicaoFreteValor)
                    valorTotalComonentesCarga += cargaCargaComponentesFrete.ValorComponente;
            }

            decimal valorFreteLiquido = (from obj in componentesFreteDinamicoPedido where obj.ComponenteFrete.SomarComponenteFreteLiquido select obj.ValorComponente).Sum();

            carga.ValorFreteLiquido = Math.Round(carga.ValorFrete + valorFreteLiquido, 2, MidpointRounding.AwayFromZero);
            carga.ValorFreteAPagar = carga.ValorFrete + valorTotalComonentesCarga + cargaPedidos.Where(o => o.IncluirICMSBaseCalculo && o.CST != "60").Sum(o => o.ValorICMS);
            carga.ValorFreteOperador = carga.ValorFrete + valorTotalComonentesCarga;
            carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;

            if (carga.CargaSVM)
            {
                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.ExcluirComposicoesFrete(carga, unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete> composicoesFrete = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete>();
                composicoesFrete.Add(Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor do Frete Gerado pelo SVM Próprio", " Valor Gerado = " + carga.ValorFreteOperador.ToString("n2"), carga.ValorFreteOperador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor gerado pelo SVM Próprio", 0, carga.ValorFreteOperador));
                Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicoesFrete, unitOfWork, null);
            }
            repCarga.Atualizar(carga);

            svcRateioFrete.GerarComponenteICMS(carga, cargaPedidos, false, false, unitOfWork);
            svcRateioFrete.GerarComponentePisCofins(carga, cargaPedidos, false, unitOfWork);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> RecriarRateioEntreNotasPorCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            RateioFormula serRateioFormula = new RateioFormula(unitOfWork);
            RateioNotaFiscal svcRateioNotaFiscal = new RateioNotaFiscal(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            //Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = serRateioFormula.ObterFormulaDeRateio(cargaCTe.Carga, unitOfWork);
            Dominio.Entidades.Embarcador.Rateio.RateioFormula formulaRateio = cargaCTe.Carga.Pedidos.First().FormulaRateio;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLsNotasFiscaisCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarTodosCargaPedidoXMLNotaFiscalCTePorCargaCTe(cargaCTe.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> cargaCTEComponentesFrete = repCargaCTeComponentesFrete.BuscarPorCargaCTe(cargaCTe.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();


            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTe in cargaPedidosXMLsNotasFiscaisCTes)
            {
                if (!pedidosXMLNotaFiscal.Contains(cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal))
                {
                    pedidosXMLNotaFiscal.Add(cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal);
                }
            }

            if (pedidosXMLNotaFiscal == null || pedidosXMLNotaFiscal.Count == 0 || (pedidosXMLNotaFiscal.Count > 500 && tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador))
                return null;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ultimoPedidoXmlNotaFiscal = pedidosXMLNotaFiscal.LastOrDefault();
            decimal totalFreteCargaCTeNF = 0;
            decimal totalICMSNF = 0;
            decimal totalBaseCalculoNF = 0;
            int numeroNotas = pedidosXMLNotaFiscal.Count();

            decimal pesoLiquidoTotalNF = pedidosXMLNotaFiscal.Sum(obj => obj.XMLNotaFiscal.PesoLiquido);
            int volumeTotalNF = pedidosXMLNotaFiscal.Sum(obj => obj.XMLNotaFiscal.Volumes);
            decimal pesoTotalNF = pedidosXMLNotaFiscal.Sum(obj => obj.XMLNotaFiscal.Peso);
            decimal valorTotalNF = pedidosXMLNotaFiscal.Sum(obj => obj.XMLNotaFiscal.Valor);
            decimal metrosCubicosNF = pedidosXMLNotaFiscal.Sum(obj => obj.XMLNotaFiscal.MetrosCubicos);

            decimal densidadeProdutos = ultimoPedidoXmlNotaFiscal?.CargaPedido?.Produtos?.Sum(o => o.Produto?.MetroCubito) ?? 0m;

            decimal pesoTotalParaCalculoFatorCubagem = serRateioFormula.ObterPesoTotalCubadoFatorCubagem(pedidosXMLNotaFiscal);

            if (cargaCTe?.Carga?.CargaSVM ?? false)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal = pedidosXMLNotaFiscal.FirstOrDefault();

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> xmlNotasFiscaisPedidosExistentes = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorXMLnotaFiscal(pedidoXmlNotaFiscal.Codigo, null, false);
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete xmlNotasFiscaisPedidoExistente in xmlNotasFiscaisPedidosExistentes)
                    repPedidoXMLNotaFiscalComponenteFrete.Deletar(xmlNotasFiscaisPedidoExistente);

                if (!cargaPedidos.Contains(pedidoXmlNotaFiscal.CargaPedido))
                    cargaPedidos.Add(pedidoXmlNotaFiscal.CargaPedido);

                pedidoXmlNotaFiscal.ValorFrete = cargaCTe.CTe.ValorFrete;
                totalFreteCargaCTeNF += pedidoXmlNotaFiscal.ValorFrete;

                pedidoXmlNotaFiscal.ValorICMS = cargaCTe.CTe.ValorICMS;
                totalICMSNF += pedidoXmlNotaFiscal.ValorICMS;

                pedidoXmlNotaFiscal.BaseCalculoICMS = cargaCTe.CTe.BaseCalculoICMS;
                totalBaseCalculoNF += pedidoXmlNotaFiscal.BaseCalculoICMS;

                pedidoXmlNotaFiscal.CFOP = cargaCTe.CTe.CFOP;
                pedidoXmlNotaFiscal.CST = cargaCTe.CTe.CST;
                pedidoXmlNotaFiscal.IncluirICMSBaseCalculo = cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
                pedidoXmlNotaFiscal.PercentualAliquota = cargaCTe.CTe.AliquotaICMS;
                pedidoXmlNotaFiscal.ICMSPagoPorST = cargaCTe.CTe.CST == "60" ? true : false;
                pedidoXmlNotaFiscal.PercentualIncluirBaseCalculo = cargaCTe.CTe.PercentualICMSIncluirNoFrete;
                pedidoXmlNotaFiscal.PercentualReducaoBC = cargaCTe.CTe.PercentualReducaoBaseCalculoICMS;
                pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = cargaCTe.CTe != null && cargaCTe.CTe.CST == "60" ? (cargaCTe.CTe.ValorAReceber + cargaCTe.CTe.ValorICMS == cargaCTe.CTe.ValorPrestacaoServico) : false;

                if (pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscal))
                {
                    pedidoXmlNotaFiscal.ValorFrete += cargaCTe.CTe.ValorFrete - totalFreteCargaCTeNF;
                    pedidoXmlNotaFiscal.ValorICMS += cargaCTe.CTe.ValorICMS - totalICMSNF;
                    pedidoXmlNotaFiscal.BaseCalculoICMS += cargaCTe.CTe.BaseCalculoICMS - totalBaseCalculoNF;
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFreteUnico in cargaCTEComponentesFrete)
                {
                    decimal valorRateioOriginal = 0;
                    decimal pesoParaCalculoFatorCubagem = 0;

                    if (formulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                        pesoParaCalculoFatorCubagem = serRateioFormula.ObterPesoCubadoFatorCubagem(pedidoXmlNotaFiscal.CargaPedido.FormulaRateio?.ParametroRateioFormula, pedidoXmlNotaFiscal.CargaPedido.TipoUsoFatorCubagemRateioFormula, pedidoXmlNotaFiscal.CargaPedido.FatorCubagemRateioFormula ?? 0, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos);

                    decimal valorComponente = serRateioFormula.AplicarFormulaRateio(formulaRateio, cargaCTeComponenteFreteUnico.ValorComponente, numeroNotas, 1, pesoTotalNF, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, cargaCTeComponenteFreteUnico.Percentual, cargaCTeComponenteFreteUnico.TipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosNF, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNF, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNF, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                    if (pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscal))
                    {
                        decimal valorTotalComponente = (from obj in cargaCTEComponentesFrete where obj.TipoComponenteFrete == cargaCTeComponenteFreteUnico.TipoComponenteFrete && (cargaCTeComponenteFreteUnico.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaCTeComponenteFreteUnico.ComponenteFrete)) select obj.ValorComponente).Sum();
                        decimal valorTotalCargaPedidoComponente = repPedidoXMLNotaFiscalComponenteFrete.BuscarTotalCargaPedidoPorCompomenteCargaCTe(pedidoXmlNotaFiscal.CargaPedido.Codigo, cargaCTe.Codigo, cargaCTeComponenteFreteUnico.TipoComponenteFrete, cargaCTeComponenteFreteUnico.ComponenteFrete, false) + valorComponente;
                        valorComponente += valorTotalComponente - valorTotalCargaPedidoComponente;
                    }

                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponente = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete()
                    {
                        ComponenteFrete = cargaCTeComponenteFreteUnico.ComponenteFrete,
                        PedidoXMLNotaFiscal = pedidoXmlNotaFiscal,
                        TipoComponenteFrete = cargaCTeComponenteFreteUnico.TipoComponenteFrete,
                        ValorComponente = valorComponente,
                        IncluirBaseCalculoICMS = cargaCTeComponenteFreteUnico.IncluirBaseCalculoICMS,
                        Percentual = cargaCTeComponenteFreteUnico.Percentual,
                        TipoValor = cargaCTeComponenteFreteUnico.TipoValor,
                        DescontarValorTotalAReceber = cargaCTeComponenteFreteUnico.DescontarValorTotalAReceber,
                        AcrescentaValorTotalAReceber = cargaCTeComponenteFreteUnico.AcrescentaValorTotalAReceber,
                        NaoSomarValorTotalAReceber = cargaCTeComponenteFreteUnico.NaoSomarValorTotalAReceber,
                        DescontarDoValorAReceberValorComponente = cargaCTeComponenteFreteUnico.DescontarDoValorAReceberValorComponente,
                        DescontarDoValorAReceberOICMSDoComponente = cargaCTeComponenteFreteUnico.DescontarDoValorAReceberOICMSDoComponente,
                        ValorICMSComponenteDestacado = cargaCTeComponenteFreteUnico.ValorICMSComponenteDestacado,
                        NaoSomarValorTotalPrestacao = cargaCTeComponenteFreteUnico.NaoSomarValorTotalPrestacao,
                        IncluirIntegralmenteContratoFreteTerceiro = cargaCTeComponenteFreteUnico.IncluirIntegralmenteContratoFreteTerceiro
                    };

                    repPedidoXMLNotaFiscalComponenteFrete.Inserir(pedidoXMLNotaFiscalComponente);
                }

                repPedidoXMLNotaFiscal.Atualizar(pedidoXmlNotaFiscal);

                svcRateioNotaFiscal.GerarComponenteICMS(pedidoXmlNotaFiscal, false, unitOfWork);
                svcRateioNotaFiscal.GerarComponentePisCofins(pedidoXmlNotaFiscal, unitOfWork);
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal in pedidosXMLNotaFiscal)
                {
                    decimal valorRateioOriginal = 0;
                    decimal pesoParaCalculoFatorCubagem = 0;

                    if (formulaRateio?.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                        pesoParaCalculoFatorCubagem = serRateioFormula.ObterPesoCubadoFatorCubagem(pedidoXmlNotaFiscal.CargaPedido.FormulaRateio?.ParametroRateioFormula, pedidoXmlNotaFiscal.CargaPedido.TipoUsoFatorCubagemRateioFormula, pedidoXmlNotaFiscal.CargaPedido.FatorCubagemRateioFormula ?? 0, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> xmlNotasFiscaisPedidosExistentes = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorXMLnotaFiscal(pedidoXmlNotaFiscal.Codigo, null, false);
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete xmlNotasFiscaisPedidoExistente in xmlNotasFiscaisPedidosExistentes)
                        repPedidoXMLNotaFiscalComponenteFrete.Deletar(xmlNotasFiscaisPedidoExistente);

                    if (!cargaPedidos.Contains(pedidoXmlNotaFiscal.CargaPedido))
                        cargaPedidos.Add(pedidoXmlNotaFiscal.CargaPedido);

                    pedidoXmlNotaFiscal.ValorFrete = serRateioFormula.AplicarFormulaRateio(formulaRateio, cargaCTe.CTe.ValorFrete, numeroNotas, 1, pesoTotalNF, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosNF, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNF, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNF, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                    totalFreteCargaCTeNF += pedidoXmlNotaFiscal.ValorFrete;

                    pedidoXmlNotaFiscal.ValorICMS = serRateioFormula.AplicarFormulaRateio(formulaRateio, cargaCTe.CTe.ValorICMS, numeroNotas, 1, pesoTotalNF, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosNF, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNF, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNF, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                    totalICMSNF += pedidoXmlNotaFiscal.ValorICMS;

                    pedidoXmlNotaFiscal.BaseCalculoICMS = serRateioFormula.AplicarFormulaRateio(formulaRateio, cargaCTe.CTe.BaseCalculoICMS, numeroNotas, 1, pesoTotalNF, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosNF, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNF, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNF, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                    totalBaseCalculoNF += pedidoXmlNotaFiscal.BaseCalculoICMS;

                    pedidoXmlNotaFiscal.CFOP = cargaCTe.CTe.CFOP;
                    pedidoXmlNotaFiscal.CST = cargaCTe.CTe.CST;
                    pedidoXmlNotaFiscal.IncluirICMSBaseCalculo = cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
                    pedidoXmlNotaFiscal.PercentualAliquota = cargaCTe.CTe.AliquotaICMS;
                    pedidoXmlNotaFiscal.ICMSPagoPorST = cargaCTe.CTe.CST == "60" ? true : false;
                    pedidoXmlNotaFiscal.PercentualIncluirBaseCalculo = cargaCTe.CTe.PercentualICMSIncluirNoFrete;
                    pedidoXmlNotaFiscal.PercentualReducaoBC = cargaCTe.CTe.PercentualReducaoBaseCalculoICMS;
                    pedidoXmlNotaFiscal.DescontarICMSDoValorAReceber = cargaCTe.CTe != null && cargaCTe.CTe.CST == "60" ? (cargaCTe.CTe.ValorAReceber + cargaCTe.CTe.ValorICMS == cargaCTe.CTe.ValorPrestacaoServico) : false;

                    if (pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscal))
                    {
                        pedidoXmlNotaFiscal.ValorFrete += cargaCTe.CTe.ValorFrete - totalFreteCargaCTeNF;
                        pedidoXmlNotaFiscal.ValorICMS += cargaCTe.CTe.ValorICMS - totalICMSNF;
                        pedidoXmlNotaFiscal.BaseCalculoICMS += cargaCTe.CTe.BaseCalculoICMS - totalBaseCalculoNF;
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFrete in cargaCTEComponentesFrete)
                    {
                        decimal valorComponente = serRateioFormula.AplicarFormulaRateio(formulaRateio, cargaCTeComponenteFrete.ValorComponente, numeroNotas, 1, pesoTotalNF, pedidoXmlNotaFiscal.XMLNotaFiscal.Peso, pedidoXmlNotaFiscal.XMLNotaFiscal.Valor, valorTotalNF, cargaCTeComponenteFrete.Percentual, cargaCTeComponenteFrete.TipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, pedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosNF, densidadeProdutos, false, pedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoLiquidoTotalNF, pedidoXmlNotaFiscal.XMLNotaFiscal.Volumes, volumeTotalNF, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                        if (pedidoXmlNotaFiscal.Equals(ultimoPedidoXmlNotaFiscal))
                        {
                            decimal valorTotalComponente = (from obj in cargaCTEComponentesFrete where obj.TipoComponenteFrete == cargaCTeComponenteFrete.TipoComponenteFrete && (cargaCTeComponenteFrete.ComponenteFrete == null || obj.ComponenteFrete.Equals(cargaCTeComponenteFrete.ComponenteFrete)) select obj.ValorComponente).Sum();
                            decimal valorTotalCargaPedidoComponente = repPedidoXMLNotaFiscalComponenteFrete.BuscarTotalCargaPedidoPorCompomenteCargaCTe(pedidoXmlNotaFiscal.CargaPedido.Codigo, cargaCTe.Codigo, cargaCTeComponenteFrete.TipoComponenteFrete, cargaCTeComponenteFrete.ComponenteFrete, false) + valorComponente;
                            valorComponente += valorTotalComponente - valorTotalCargaPedidoComponente;
                        }

                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponente = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete()
                        {
                            ComponenteFrete = cargaCTeComponenteFrete.ComponenteFrete,
                            PedidoXMLNotaFiscal = pedidoXmlNotaFiscal,
                            TipoComponenteFrete = cargaCTeComponenteFrete.TipoComponenteFrete,
                            ValorComponente = valorComponente,
                            IncluirBaseCalculoICMS = cargaCTeComponenteFrete.IncluirBaseCalculoICMS,
                            Percentual = cargaCTeComponenteFrete.Percentual,
                            TipoValor = cargaCTeComponenteFrete.TipoValor,
                            DescontarValorTotalAReceber = cargaCTeComponenteFrete.DescontarValorTotalAReceber,
                            AcrescentaValorTotalAReceber = cargaCTeComponenteFrete.AcrescentaValorTotalAReceber,
                            NaoSomarValorTotalAReceber = cargaCTeComponenteFrete.NaoSomarValorTotalAReceber,
                            DescontarDoValorAReceberValorComponente = cargaCTeComponenteFrete.DescontarDoValorAReceberValorComponente,
                            DescontarDoValorAReceberOICMSDoComponente = cargaCTeComponenteFrete.DescontarDoValorAReceberOICMSDoComponente,
                            ValorICMSComponenteDestacado = cargaCTeComponenteFrete.ValorICMSComponenteDestacado,
                            NaoSomarValorTotalPrestacao = cargaCTeComponenteFrete.NaoSomarValorTotalPrestacao,
                            IncluirIntegralmenteContratoFreteTerceiro = cargaCTeComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro
                        };

                        repPedidoXMLNotaFiscalComponenteFrete.Inserir(pedidoXMLNotaFiscalComponente);
                    }

                    repPedidoXMLNotaFiscal.Atualizar(pedidoXmlNotaFiscal);

                    svcRateioNotaFiscal.GerarComponenteICMS(pedidoXmlNotaFiscal, false, unitOfWork);
                    svcRateioNotaFiscal.GerarComponentePisCofins(pedidoXmlNotaFiscal, unitOfWork);
                }
            }

            return cargaPedidos;
        }

        #endregion
    }
}
