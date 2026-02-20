using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class PedidoOcorrenciaColetaEntregaIntegracao
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly string _urlAcessoCliente;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga _configuracaoGeralCarga;

        #endregion

        #region Construtores

        public PedidoOcorrenciaColetaEntregaIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PedidoOcorrenciaColetaEntregaIntegracao(Repositorio.UnitOfWork unitOfWork, string urlAcessoCliente)
        {
            _unitOfWork = unitOfWork;
            _urlAcessoCliente = urlAcessoCliente;
        }

        #endregion

        #region Métodos Públicos

        public static void GerarIntegracoes(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!configuracao.PedidoOcorrenciaColetaEntregaIntegracaoNova)
                return;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracaoAutorizados = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.InteliPost);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Riachuelo);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Dansales);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Emillenium);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.VTEX);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cobasi);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Isis);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Havan);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalu);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Simonetti);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marisa);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Deca);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Neokohm);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Diageo);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Yandeh);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Obramax);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Runtec);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ConfirmaFacil);
            tipoIntegracaoAutorizados.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mondelez);

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracaoAtivos = new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();

            if (pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repOcorrenciaTipoIntegracao.BuscarIntegracaoPorTipoOcorrencia(pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.Codigo);
                if (tiposIntegracao.Count > 0)
                    tiposIntegracaoAtivos.AddRange(tiposIntegracao);
            }

            Dominio.Entidades.Cliente tomador = pedidoOcorrenciaColetaEntrega.Pedido.ObterTomador();

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = null;
            if ((pedidoOcorrenciaColetaEntrega.Pedido?.TipoOperacao?.UsarConfiguracaoEmissao ?? false) && pedidoOcorrenciaColetaEntrega.Pedido?.TipoOperacao?.TipoIntegracao != null)
            {
                if (pedidoOcorrenciaColetaEntrega.Pedido?.TipoOperacao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && pedidoOcorrenciaColetaEntrega.Pedido.TipoOperacao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                    tipoIntegracao = pedidoOcorrenciaColetaEntrega.Pedido?.TipoOperacao.TipoIntegracao;
            }

            if (tipoIntegracao == null && tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    if (tomador.TipoIntegracao != null && tomador.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && tomador.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        tipoIntegracao = tomador.TipoIntegracao;
                }
                else if (tomador.GrupoPessoas != null)
                {
                    if (tomador.GrupoPessoas.TipoIntegracao != null && tomador.GrupoPessoas.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && tomador.GrupoPessoas.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                        tipoIntegracao = tomador.GrupoPessoas.TipoIntegracao;

                    if (pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.TipoOcorrenciaControleEntrega && tomador.GrupoPessoas.UtilizaMultiEmbarcador.HasValue && tomador.GrupoPessoas.UtilizaMultiEmbarcador.Value && tomador.GrupoPessoas.HabilitarIntegracaoOcorrenciasMultiEmbarcador.HasValue && tomador.GrupoPessoas.HabilitarIntegracaoOcorrenciasMultiEmbarcador.Value)
                        tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador);
                }
            }

            if (tipoIntegracao != null && !tiposIntegracaoAtivos.Contains(tipoIntegracao))
                tiposIntegracaoAtivos.Add(tipoIntegracao);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoAtivo in tiposIntegracaoAtivos)
            {
                if (!tipoIntegracaoAutorizados.Contains(tipoIntegracaoAtivo.Tipo))
                    continue;

                if (tipoIntegracaoAtivo.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Diageo &&
                    !(pedidoOcorrenciaColetaEntrega.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoIntegracaoDiageo?.PossuiIntegracaoDiageo ?? false))
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoOcorrenciaColetaEntregaIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao()
                {
                    PedidoOcorrenciaColetaEntrega = pedidoOcorrenciaColetaEntrega,
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = "",
                    TipoIntegracao = tipoIntegracaoAtivo
                };
                repPedidoOcorrenciaColetaEntregaIntegracao.Inserir(pedidoOcorrenciaColetaEntregaIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
                };
                Servicos.Auditoria.Auditoria.Auditar(auditado, pedidoOcorrenciaColetaEntregaIntegracao, "Registro de integração.", unitOfWork);
            }

            Servicos.Embarcador.Integracao.IntegracaoEDI.AdicionarEDIParaIntegracao(pedidoOcorrenciaColetaEntrega, unitOfWork);
        }

        public void ProcessarIntegracoesPendentes(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, int codigoClienteURLAcesso)
        {
            if (!configuracao.PedidoOcorrenciaColetaEntregaIntegracaoNova)
                return;

            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(_unitOfWork).BuscarConfiguracaoPadrao();

            int intervaloTempoRejeitadas = 5;
            List<int> integracoesPendentes = repPedidoOcorrenciaColetaEntregaIntegracao.BuscarIntegracoesPorSituacao(intervaloTempoRejeitadas, 100, configuracaoCanhoto.EfetuarIntegracaoApenasCanhotosDigitalizados);
            int total = integracoesPendentes.Count();
            for (int i = 0; i < total; i++)
            {
                ProcessarIntegracaoPendente(integracoesPendentes[i], tipoServicoMultisoftware, unitOfWorkAdminMultisoftware, clienteMultisoftware, codigoClienteURLAcesso);

                _unitOfWork.FlushAndClear();
            }
        }

        public void ProcessarIntegracaoPendente(int codigoPedidoOcorrenciaColetaEntregaIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, int codigoClienteURLAcesso)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoOcorrenciaColetaEntregaIntegracao = repPedidoOcorrenciaColetaEntregaIntegracao.BuscarPorCodigo(codigoPedidoOcorrenciaColetaEntregaIntegracao);

            switch (pedidoOcorrenciaColetaEntregaIntegracao?.TipoIntegracao.Tipo ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.InteliPost:
                    ProcessarIntegracao_Intelipost(pedidoOcorrenciaColetaEntregaIntegracao, clienteMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Riachuelo:
                    ProcessarIntegracao_Riachuelo(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador:
                    ProcessarIntegracao_MultiEmbarcador(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email:
                    ProcessarIntegracao_Email(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP:
                    ProcessarIntegracao_FTP(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Dansales:
                    ProcessarIntegracao_Dansales(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Emillenium:
                    ProcessarIntegracao_Emillenium(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Cobasi:
                    ProcessarIntegracao_Cobasi(pedidoOcorrenciaColetaEntregaIntegracao, tipoServicoMultisoftware, unitOfWorkAdminMultisoftware, codigoClienteURLAcesso);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.VTEX:
                    ProcessarIntegracao_Vtex(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalu:
                    ProcessarIntegracao_Magalu(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Havan:
                    ProcessarIntegracao_Havan(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Simonetti:
                    ProcessarIntegracao_Simonetti(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marisa:
                    ProcessarIntegracao_Marisa(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Deca:
                    ProcessarIntegracao_Deca(pedidoOcorrenciaColetaEntregaIntegracao, tipoServicoMultisoftware, unitOfWorkAdminMultisoftware, codigoClienteURLAcesso);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal:
                    ProcessarIntegracao_ArcelorMittal(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Neokohm:
                    ProcessarIntegracao_Neokohm(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Diageo:
                    ProcessarIntegracao_Diageo(pedidoOcorrenciaColetaEntregaIntegracao, unitOfWorkAdminMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Obramax:
                    ProcessarIntegracao_Obramax(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Runtec:
                    ProcessarIntegracao_Runtec(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Electrolux:
                    ProcessarIntegracao_Electrolux(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ConfirmaFacil:
                    ProcessarIntegracao_ConfirmaFacil(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Mondelez:
                    ProcessarIntegracao_Mondelez(pedidoOcorrenciaColetaEntregaIntegracao);
                    break;

            }
        }

        public static MemoryStream ObterArquivoEDIOcoren(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.EDI.OCOREN svcOCOREN = new Servicos.Embarcador.Integracao.EDI.OCOREN();
            Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ocoren = svcOCOREN.ConverterParaOCOREN(notasFiscais, null, integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia, "", integracao.PedidoOcorrenciaColetaEntrega.DataOcorrencia, integracao.PedidoOcorrenciaColetaEntrega.DataOcorrencia, "", unitOfWork, null, integracao.LayoutEDI, empresa, integracao.PedidoOcorrenciaColetaEntrega.Pedido.Remetente);
            Servicos.GeracaoEDI svcEDI = new GeracaoEDI(unitOfWork, integracao.LayoutEDI, empresa);
            return svcEDI.GerarArquivoRecursivo(ocoren);
        }

        #endregion

        #region Métodos Privados

        private void ProcessarIntegracao_Intelipost(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega = integracao.PedidoOcorrenciaColetaEntrega;
                Dominio.Entidades.Empresa empresa = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Empresa;
                if (empresa == null)
                    empresa = integracao.PedidoOcorrenciaColetaEntrega.Pedido.Empresa;

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = pedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.ToList();
                if (notasFiscais.Count <= 0)
                    notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(pedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                if (notasFiscais.Count == 0)
                    throw new Exception("Não possui nota fiscal para integrar!");

                Servicos.Embarcador.Integracao.Intelipost.IntegracaoOcorrencia servicoIntegracaoOcorrencia = new Integracao.Intelipost.IntegracaoOcorrencia(_unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = servicoIntegracaoOcorrencia.EnviarOcorrencia(pedidoOcorrenciaColetaEntrega.Pedido, pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia, notaFiscal, empresa, pedidoOcorrenciaColetaEntrega.DataOcorrencia, integracao.PedidoOcorrenciaColetaEntrega.Carga, clienteMultisoftware);
                    SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
                }
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Riachuelo(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.ToList();
                if (notasFiscais.Count <= 0)
                    notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = Integracao.Riachuelo.IntegracaoRiachuelo.IntegrarNFesEntregues(notasFiscais, _unitOfWork);
                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Cobasi(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftware, int codigoClienteURLAcesso)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.ToList();
                if (notasFiscais.Count <= 0)
                    notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = Integracao.Cobasi.IntegracaoCobasi.IntegrarOcorrencia(integracao.PedidoOcorrenciaColetaEntrega, _unitOfWork, tipoServicoMultisoftware, unitOfWorkAdminMultisoftware, codigoClienteURLAcesso);
                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Deca(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftware, int codigoClienteURLAcesso)
        {
            try
            {
                Integracao.Deca.IntegracaoDeca servicoIntegracaoDexcoMadeira = new Servicos.Embarcador.Integracao.Deca.IntegracaoDeca(_unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = servicoIntegracaoDexcoMadeira.IntegrarOcorrencia(integracao.PedidoOcorrenciaColetaEntrega, tipoServicoMultisoftware, unitOfWorkAdminMultisoftware, codigoClienteURLAcesso);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_MultiEmbarcador(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

                Dominio.Entidades.Cliente tomador = integracao.PedidoOcorrenciaColetaEntrega.Pedido.ObterTomador();
                Dominio.Entidades.Empresa empresa = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Empresa;
                if (empresa == null)
                    empresa = integracao.PedidoOcorrenciaColetaEntrega.Pedido.Empresa;

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.ToList();

                if (notasFiscais.Count <= 0)
                    notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                string codigoTipoOcorrenciaIntegracao = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.CodigoIntegracao;

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = Integracao.MultiEmbarcador.Ocorrencia.AdicionarOcorrencia(tomador, empresa, notasFiscais, codigoTipoOcorrenciaIntegracao, integracao.PedidoOcorrenciaColetaEntrega.DataOcorrencia, _unitOfWork);
                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Email(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                if (integracao.LayoutEDI != null)
                {

                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

                    string emails = Integracao.Email.IntegracaoEmail.ObterEmails(integracao.PedidoOcorrenciaColetaEntrega.Pedido.TipoOperacao, integracao.PedidoOcorrenciaColetaEntrega.Pedido.ObterTomador(), integracao.LayoutEDI);
                    if (string.IsNullOrWhiteSpace(emails)) throw new Exception("E-mail para envio dos documentos é inválido.");

                    string numero = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NumeroPedidoEmbarcador;
                    string nomeArquivo = Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao.LayoutEDI, numero);

                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.ToList();
                    if (notasFiscais.Count <= 0)
                        notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                    string NumerosNotasFiscais = notasFiscais != null && notasFiscais.Count > 0 ? String.Join("", (from notas in notasFiscais select notas.Numero)) : "";

                    Dominio.Entidades.Empresa empresa = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Empresa;

                    if (empresa == null)
                        empresa = integracao.PedidoOcorrenciaColetaEntrega.Pedido.Empresa;

                    string mensagem;
                    bool resultado = false;

                    MemoryStream arquivoEDI = ObterArquivoEDIOcoren(integracao, empresa, notasFiscais, _unitOfWork);
                    if (arquivoEDI != null)
                    {

                        List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>();
                        anexos.Add(new System.Net.Mail.Attachment(arquivoEDI, nomeArquivo, "text/plain"));
                        try
                        {
                            Integracao.Email.IntegracaoEmail.EnviarEmail(integracao.LayoutEDI, numero, NumerosNotasFiscais, emails, anexos, _unitOfWork);
                            mensagem = "Envio realizado com sucesso para " + emails;
                            resultado = true;
                        }
                        catch (Exception e)
                        {
                            mensagem = e.Message;
                        }
                        SalvarResultadoIntegracao(integracao, resultado, mensagem, mensagem, "registro envio e-mail", "txt", null, null, null);
                    }
                    else
                    {
                        SalvarResultadoIntegracaoFalha(integracao, "Arquivo EDI vazio.");
                    }
                }
                else
                {
                    SalvarResultadoIntegracaoFalha(integracao, "Layout EDI não configurado/contemplado.");
                }
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_FTP(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                if (integracao.LayoutEDI != null)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();
                    string url = string.Empty, usuario = string.Empty, senha = string.Empty, diretorio = string.Empty, porta = string.Empty, certificado = string.Empty;
                    bool passivo, utilizarSFTP, ssl, criarComNomeTemporaraio;

                    Dominio.Entidades.Empresa empresa = integracao.PedidoOcorrenciaColetaEntrega.Carga?.Empresa;
                    Dominio.Entidades.Empresa empresaFilialEmissora = null;

                    if (empresa == null)
                        empresa = integracao.PedidoOcorrenciaColetaEntrega.Pedido.Empresa;

                    Integracao.FTP.IntegracaoFTP.ObterConfiguracoesConexaoFTP(integracao.PedidoOcorrenciaColetaEntrega.Pedido.ObterTomador(), empresa, integracao.PedidoOcorrenciaColetaEntrega.Pedido.TipoOperacao, integracao.LayoutEDI.Codigo, out url, out usuario, out senha, out diretorio, out porta, out passivo, out utilizarSFTP, out ssl, out criarComNomeTemporaraio, out certificado, _unitOfWork);

                    string numero = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NumeroPedidoEmbarcador;
                    string nomeArquivo = Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao.LayoutEDI, numero);

                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NotasFiscais.ToList();
                    if (notasFiscais.Count <= 0)
                        notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                    if (configuracaoGeralCarga.UtilizarEmpresaFilialEmissoraNoArquivoEDI && integracao.PedidoOcorrenciaColetaEntrega.Carga?.EmpresaFilialEmissora != null)
                        empresaFilialEmissora = integracao.PedidoOcorrenciaColetaEntrega.Carga.EmpresaFilialEmissora;

                    MemoryStream arquivoEDI = ObterArquivoEDIOcoren(integracao, empresaFilialEmissora ?? empresa, notasFiscais, _unitOfWork);
                    if (arquivoEDI != null)
                    {
                        string mensagem;
                        bool resultado = false;
                        try
                        {
                            resultado = Servicos.FTP.EnviarArquivo(arquivoEDI, nomeArquivo, url, porta, diretorio, usuario, senha, passivo, ssl, out mensagem, utilizarSFTP, criarComNomeTemporaraio, certificado);
                            if (resultado) mensagem = "Envio realizado com sucesso.";
                        }
                        catch (Exception e)
                        {
                            mensagem = e.Message;
                        }
                        SalvarResultadoIntegracao(integracao, resultado, mensagem, mensagem, "registro envio FTP", "txt", null, null, null);
                    }
                    else
                    {
                        SalvarResultadoIntegracaoFalha(integracao, "Arquivo EDI vazio.");
                    }
                }
                else
                {
                    SalvarResultadoIntegracaoFalha(integracao, "Layout EDI não configurado.");
                }
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Dansales(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = Integracao.Dansales.IntegracaoDansales.IntegrarNFes(notasFiscais, integracao, _unitOfWork);
                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Emillenium(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repositorioPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                if (notasFiscais == null || notasFiscais.Count == 0)
                    throw new Exception("Pedido " + integracao.PedidoOcorrenciaColetaEntrega.Pedido.NumeroPedidoEmbarcador + " sem notas.");

                Integracao.Emillenium.IntegracaoEmillenium servicoIntegracaoEmillenium = new Integracao.Emillenium.IntegracaoEmillenium(_unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, _unitOfWork.StringConexao);

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = servicoIntegracaoEmillenium.ConfirmarEntrega(notaFiscal);
                    SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
                }
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Vtex(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repositorioPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                if (notasFiscais == null || notasFiscais.Count == 0)
                    throw new Exception("Pedido " + integracao.PedidoOcorrenciaColetaEntrega.Pedido.NumeroPedidoEmbarcador + " sem notas.");

                Integracao.VTEX.IntegracaoVtex servicoIntegracaoVtex = new Integracao.VTEX.IntegracaoVtex(_unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = servicoIntegracaoVtex.AtualizarRastreamentoPedido(notaFiscal, integracao.PedidoOcorrenciaColetaEntrega.Pedido);
                    SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
                }
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Magalu(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Integracao.Magalu.IntegracaoMagalu servicoIntegracaoMagalu = new Integracao.Magalu.IntegracaoMagalu(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = servicoIntegracaoMagalu.EnviarOcorrencia(integracao.PedidoOcorrenciaColetaEntrega);
                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Havan(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Integracao.Havan.IntegracaoHavan servicoIntegracaoHavan = new Integracao.Havan.IntegracaoHavan(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = servicoIntegracaoHavan.EnviarOcorrencia(integracao.PedidoOcorrenciaColetaEntrega);
                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Simonetti(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Integracao.Simonetti.IntegracaoSimonetti(_unitOfWork).IntegrarOcorrenciasColetaEntrega(notasFiscais, integracao);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        public void ProcessarIntegracao_Marisa(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repPedidoXmlNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Integracao.IntegracaoMarisa(_unitOfWork, _urlAcessoCliente).IntegrarOcorrenciasColetaEntrega(notasFiscais, integracao);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_ArcelorMittal(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Integracao.ArcelorMittal.IntegracaoArcelorMittal servicoIntegracaoArcelorMittal = new Integracao.ArcelorMittal.IntegracaoArcelorMittal(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = servicoIntegracaoArcelorMittal.EnviarOcorrencia(integracao.PedidoOcorrenciaColetaEntrega);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Neokohm(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Servicos.Embarcador.Integracao.Neokohm.IntegracaoNeokohm serIntegracaoNeokohm = new Integracao.Neokohm.IntegracaoNeokohm(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = serIntegracaoNeokohm.IntegrarOcorrenciaPedido(integracao);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Diageo(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftrware)
        {
            try
            {
                Servicos.Embarcador.Integracao.Diageo.IntegracaoDiageo serIntegracaoDiageo = new Integracao.Diageo.IntegracaoDiageo(_unitOfWork, unitOfWorkAdminMultisoftrware);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = serIntegracaoDiageo.IntegrarOcorrenciaPedidoFTP(integracao.PedidoOcorrenciaColetaEntrega);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Obramax(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Servicos.Embarcador.Integracao.Obramax.IntegracaoObramax serIntegracaoObramax = new Integracao.Obramax.IntegracaoObramax(_unitOfWork, _urlAcessoCliente);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = serIntegracaoObramax.IntegrarOcorrenciaPedido(integracao);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Runtec(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Servicos.Embarcador.Integracao.Runtec.IntegracaoRuntec serIntegracaoRuntec = new Integracao.Runtec.IntegracaoRuntec(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = serIntegracaoRuntec.IntegrarOcorrenciaPedido(integracao, _unitOfWork);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }


        private void ProcessarIntegracao_Electrolux(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Integracao.Electrolux.IntegracaoElectroluxOCOREN(_unitOfWork, integracao).IntegrarOcorrencia();

                if (httpRequisicaoResposta.sucesso)
                    SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
                else
                    SalvarResultadoIntegracaoFalha(integracao, httpRequisicaoResposta.mensagem);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_ConfirmaFacil(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Servicos.Embarcador.Integracao.ConfirmaFacil.IntegracaoConfirmaFacil serIntegracaoConfirmaFacil = new Integracao.ConfirmaFacil.IntegracaoConfirmaFacil(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = serIntegracaoConfirmaFacil.IntegrarOcorrenciaPedido(integracao, _unitOfWork, _urlAcessoCliente);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private void ProcessarIntegracao_Mondelez(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            try
            {
                Servicos.Embarcador.Integracao.Mondelez.IntegracaoMondelez servicoIntegracaoMondelez = new Integracao.Mondelez.IntegracaoMondelez(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = servicoIntegracaoMondelez.IntegrarOcorrenciaPedido(integracao);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao, e.Message);
            }
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga ObterConfiguracaoGeralCarga()
        {
            if (_configuracaoGeralCarga == null)
                _configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoGeralCarga;
        }

        #endregion

        #region Métodos Privados - Salvar Resultado

        private void SalvarResultadoIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta)
        {
            SalvarResultadoIntegracao(integracao, httpRequisicaoResposta.sucesso, httpRequisicaoResposta.mensagem, httpRequisicaoResposta.conteudoRequisicao, Guid.NewGuid().ToString(), httpRequisicaoResposta.extensaoRequisicao, httpRequisicaoResposta.conteudoResposta, Guid.NewGuid().ToString(), httpRequisicaoResposta.extensaoResposta);
        }

        private void SalvarResultadoIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, bool sucesso, string mensagem, string conteudoRequisicao, string nomeArquivoRequisicao, string extensaoArquivoRequisicao, string conteudoResposta, string nomeArquivoResposta, string extensaoArquivoResposta)
        {
            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;
            integracao.ProblemaIntegracao = mensagem;
            integracao.SituacaoIntegracao = sucesso ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            if (!string.IsNullOrWhiteSpace(conteudoRequisicao) || !string.IsNullOrWhiteSpace(conteudoResposta))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = DateTime.Now;
                arquivoIntegracao.Mensagem = integracao.PedidoOcorrenciaColetaEntrega.Pedido.NumeroPedidoEmbarcador + " - " + mensagem;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                if (!string.IsNullOrWhiteSpace(conteudoRequisicao))
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(conteudoRequisicao, nomeArquivoRequisicao, extensaoArquivoRequisicao, _unitOfWork);

                if (!string.IsNullOrWhiteSpace(conteudoResposta))
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(conteudoResposta, nomeArquivoResposta, extensaoArquivoResposta, _unitOfWork);
                else
                {
                    conteudoResposta = "Integração sem retorno";
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(conteudoResposta, nomeArquivoResposta, extensaoArquivoResposta, _unitOfWork);
                }

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repOcorrenciaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
                repOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                integracao.ArquivosTransacao.Add(arquivoIntegracao);
            }

            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(_unitOfWork);
            repPedidoOcorrenciaColetaEntregaIntegracao.Atualizar(integracao);
        }

        private void SalvarResultadoIntegracaoFalha(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, string mensagem)
        {
            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;
            integracao.ProblemaIntegracao = mensagem;
            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(_unitOfWork);
            repPedidoOcorrenciaColetaEntregaIntegracao.Atualizar(integracao);
        }

        #endregion
    }
}
