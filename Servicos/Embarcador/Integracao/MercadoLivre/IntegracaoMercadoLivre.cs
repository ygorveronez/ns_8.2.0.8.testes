using AdminMultisoftware.Dominio.Enumeradores;
using Amazon.Runtime.Internal.Util;
using DocumentFormat.OpenXml.Presentation;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.MercadoLivre;
using Flurl.Http;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.MercadoLivre
{
    public class IntegracaoMercadoLivre
    {
        #region Propriedades Privadas

        private static string _caminhoArquivos;
        private static string _caminhoArquivosIntegracao;
        private static string _prefixoMSMQ;
        private static AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        private int _quantidadeThreadsExecutarRotaEFacility;

        #endregion

        #region Propriedades Publicas

        public bool NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes { get; set; } = false;

        #endregion

        #region Construtores

        public IntegracaoMercadoLivre(Repositorio.UnitOfWork unitOfWork)
        {
            _caminhoArquivos = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Integracoes", "MercadoLivre");
            _caminhoArquivosIntegracao = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;
        }

        public IntegracaoMercadoLivre(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, string caminhoArquivos, string caminhoArquivosIntegracao, string prefixoMSMQ, int quantidadeThreadsExecutarRotaEFacility)
        {
            _caminhoArquivos = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "Integracoes", "MercadoLivre");
            _caminhoArquivosIntegracao = caminhoArquivosIntegracao;
            _clienteMultisoftware = clienteMultisoftware;
            _prefixoMSMQ = prefixoMSMQ;
            _quantidadeThreadsExecutarRotaEFacility = quantidadeThreadsExecutarRotaEFacility;
        }

        #endregion

        #region Métodos Globais

        public void AdicionarHandlingUnitParaConsulta(List<string> ids, int rota, string facility, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, DateTime? dataConfirmarProcessamento = null, bool AvancarEtapaDocumentosParaEmissaoAutomaticamente = false)
        {
            if (carga == null || ((ids == null || ids.Count == 0) && (rota == 0 || facility == null)))
                return;

            Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit repMercadoLivreHandlingUnit = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit(unitOfWork);

            if (ids == null || ids.Count == 0)
            {
                Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit handlingUnit = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit()
                {
                    ID = null,
                    Rota = rota,
                    Facility = facility,
                    Situacao = dataConfirmarProcessamento != null ? MercadoLivreHandlingUnitSituacao.AgConfirmacao : MercadoLivreHandlingUnitSituacao.AgConsulta,
                    TipoIntegracaoMercadoLivre = ObterTipoIntegracaoMercadoLivre(carga, unitOfWork),
                    DataConfirmarProcessamento = dataConfirmarProcessamento,
                    AvancarEtapaDocumentosParaEmissaoAutomaticamente = AvancarEtapaDocumentosParaEmissaoAutomaticamente,
                    DataInclusao = DateTime.Now
                };

                repMercadoLivreHandlingUnit.Inserir(handlingUnit);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre()
                {
                    Carga = carga,
                    HandlingUnit = handlingUnit,
                };

                repCargaIntegracaoMercadoLivre.Inserir(cargaIntegracaoMercadoLivre);
            }
            else
            {
                foreach (string id in ids)
                {
                    Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit handlingUnit = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit()
                    {
                        ID = id,
                        Situacao = MercadoLivreHandlingUnitSituacao.AgConsulta,
                        TipoIntegracaoMercadoLivre = ObterTipoIntegracaoMercadoLivre(carga, unitOfWork),
                        DataInclusao = DateTime.Now
                    };

                    repMercadoLivreHandlingUnit.Inserir(handlingUnit);

                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre()
                    {
                        Carga = carga,
                        HandlingUnit = handlingUnit,
                    };

                    repCargaIntegracaoMercadoLivre.Inserir(cargaIntegracaoMercadoLivre);
                }
            }
        }

        public void ConsultarHandlingUnitsPendentes(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit repMercadoLivreHandlingUnit = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit(unitOfWork);

            object lockObterPedido = new object();
            List<int> codigosCargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarCodigosPorSituacao(MercadoLivreHandlingUnitSituacao.AgConsulta, 3);

            foreach (int codigoCargaIntegracaoMercadoLivre in codigosCargaIntegracaoMercadoLivre)
            {
                unitOfWork.FlushAndClear();

                ConsultarCargaIntegracaoMercadoLivrePendente(codigoCargaIntegracaoMercadoLivre, tipoServicoMultisoftware, lockObterPedido, auditado, unitOfWork, cancellationToken);
            }
        }

        public void ConsultarCargaIntegracaoMercadoLivrePendente(int codigoCargaIntegracaoMercadoLivre, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, object lockObterPedido, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit repMercadoLivreHandlingUnit = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorCodigo(codigoCargaIntegracaoMercadoLivre);
            Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit = cargaIntegracaoMercadoLivre.HandlingUnit;

            if (cargaIntegracaoMercadoLivre.Carga.SituacaoCarga != SituacaoCarga.AgNFe)
            {
                mercadoLivreHandlingUnit.Situacao = MercadoLivreHandlingUnitSituacao.Falha;
                if (mercadoLivreHandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                    mercadoLivreHandlingUnit.Mensagem = "A situação da carga não permite a consulta da Rota e Facility.";
                else
                    mercadoLivreHandlingUnit.Mensagem = "A situação da carga não permite a consulta do Handling Unit.";

                repMercadoLivreHandlingUnit.Atualizar(mercadoLivreHandlingUnit);

                InformarCargaAtualizada(cargaIntegracaoMercadoLivre, unitOfWork);

                return;
            }

            mercadoLivreHandlingUnit.DataInicioIntegracao = DateTime.Now;

            if (!ConsultarHandlingUnit(out string mensagemErro, out string token, ref mercadoLivreHandlingUnit, tipoServicoMultisoftware, unitOfWork, cancellationToken) ||
                !VincularHandlingUnitNaCargaPedido(out mensagemErro, cargaIntegracaoMercadoLivre, tipoServicoMultisoftware, lockObterPedido, unitOfWork, null) ||
                !DownloadVincularRotaEFacityNaCargaPedido(out mensagemErro, token, cargaIntegracaoMercadoLivre, tipoServicoMultisoftware, lockObterPedido, unitOfWork, null, cancellationToken))
            {
                mercadoLivreHandlingUnit.Situacao = MercadoLivreHandlingUnitSituacao.Falha;
                mercadoLivreHandlingUnit.Mensagem = mensagemErro.Left(500);
                mercadoLivreHandlingUnit.DataFimIntegracao = DateTime.Now;

                repMercadoLivreHandlingUnit.Atualizar(mercadoLivreHandlingUnit);

                InformarCargaAtualizada(cargaIntegracaoMercadoLivre, unitOfWork);

                return;
            }

            mercadoLivreHandlingUnit.DataFimIntegracao = DateTime.Now;
            mercadoLivreHandlingUnit.Situacao = MercadoLivreHandlingUnitSituacao.Sucesso;
            mercadoLivreHandlingUnit.Mensagem = "Consulta realizada com sucesso.";

            repMercadoLivreHandlingUnit.Atualizar(mercadoLivreHandlingUnit);

            InformarCargaAtualizada(cargaIntegracaoMercadoLivre, unitOfWork);

            VerificarAvancarCargaEtapa2(cargaIntegracaoMercadoLivre, mercadoLivreHandlingUnit, tipoServicoMultisoftware, auditado, unitOfWork);
        }

        public bool VincularHandlingUnitNaCargaPedido(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, object lockObterPedido, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            mensagemErro = string.Empty;

            if (cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                return true;

            #region Inicializando os repositorios

            Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional repCTeTerceiroDocumentoAdicional = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);

            #endregion

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail> details = repMercadoLivreHandlingUnitDetail.BuscarPorHandlingUnit(cargaIntegracaoMercadoLivre.HandlingUnit.Codigo);
            List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> arquivos = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorHandlingUnit(cargaIntegracaoMercadoLivre.HandlingUnit);

            #region Processar CT-e SuContratação

            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
            Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidade = null;
            Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFe = null;
            int qtdDocumentos = 0;

            /*
            pedidoCTeParaSubContratacao = arquivos.Where(o => o.PedidoCTeParaSubcontratacao != null).Select(o => o.PedidoCTeParaSubcontratacao).FirstOrDefault();

            if (pedidoCTeParaSubContratacao?.CTeTerceiro != null)
            {
                cteTerceiroQuantidade = repCTeTerceiroQuantidade.BuscarPorCTeTerceiroEUnidadeMedida(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo, Dominio.Enumeradores.UnidadeMedida.KG);
                cteTerceiroNFe = repCTeTerceiroNFe.BuscarPrimeiroPorCTeTerceiro(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo);
                //repCTeTerceiroDocumentoAdicional.RemoverPorCTeTerceiro(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo);

                //pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria = 0;
                //cteTerceiroQuantidade.Quantidade = 0;
                //cteTerceiroNFe.ValorTotal = 0;
            }
            */

            foreach (Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail in details
                .Where(o => o.TipoDocumento == TipoDocumentoMercadoLivreHandlingUnit.CTe && o.DetailRegistroPai == null))
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao? situacaoDocumento = detail.Situacao;
                if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Concluido || situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Desconsiderado)
                    continue;

                List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> arquivosDetail = arquivos.Where(o => o.HandlingUnitDetail.Codigo == detail.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo in arquivosDetail)
                {
                    Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detailDocumentoFilho = details.Where(o => o.DetailRegistroPai?.Codigo == detail.Codigo).FirstOrDefault();

                    if (arquivo == null || string.IsNullOrWhiteSpace(arquivo.NomeArquivo))
                        continue;

                    if (!VincularCTeSubcontratacao(
                            out mensagemErro
                            , cargaIntegracaoMercadoLivre
                            , detail
                            , detailDocumentoFilho
                            , arquivo
                            , ref pedidoCTeParaSubContratacao
                            , ref cteTerceiroNFe
                            , ref cteTerceiroQuantidade
                            , unitOfWork
                            , tipoServicoMultisoftware
                            , configuracaoTMS
                            , configuracaoGeralCarga
                            , lockObterPedido)
                        )
                        return false;

                    qtdDocumentos++;
                }

                if (qtdDocumentos >= 500)
                {
                    if (cteTerceiroQuantidade != null)
                        repCTeTerceiroQuantidade.Atualizar(cteTerceiroQuantidade);

                    if (cteTerceiroNFe != null)
                        repCTeTerceiroNFe.Atualizar(cteTerceiroNFe);

                    if (pedidoCTeParaSubContratacao?.CTeTerceiro != null)
                    {
                        if (cteTerceiroQuantidade != null)
                            pedidoCTeParaSubContratacao.CTeTerceiro.Peso = cteTerceiroQuantidade.Quantidade;

                        repCTeTerceiro.Atualizar(pedidoCTeParaSubContratacao.CTeTerceiro);
                    }

                    pedidoCTeParaSubContratacao = null;
                    cteTerceiroQuantidade = null;
                    cteTerceiroNFe = null;
                    qtdDocumentos = 0;
                }
            }

            if (qtdDocumentos > 0)
            {
                if (cteTerceiroQuantidade != null)
                    repCTeTerceiroQuantidade.Atualizar(cteTerceiroQuantidade);

                if (cteTerceiroNFe != null)
                    repCTeTerceiroNFe.Atualizar(cteTerceiroNFe);

                if (pedidoCTeParaSubContratacao?.CTeTerceiro != null)
                {
                    if (cteTerceiroQuantidade != null)
                        pedidoCTeParaSubContratacao.CTeTerceiro.Peso = cteTerceiroQuantidade.Quantidade;

                    repCTeTerceiro.Atualizar(pedidoCTeParaSubContratacao.CTeTerceiro);
                }
            }

            #endregion Processar CT-e SuContratação


            #region Processar Notas Fiscais

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNFe = null;

            foreach (Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail in details
                .Where(o => o.TipoDocumento == TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal && o.DetailRegistroPai == null))
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao? situacaoDocumento = detail.Situacao;
                if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Concluido || situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Desconsiderado)
                    continue;

                List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> arquivosDetail = arquivos.Where(o => o.HandlingUnitDetail.Codigo == detail.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo in arquivosDetail)
                {
                    Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detailDocumentoFilho = details.Where(o => o.DetailRegistroPai?.Codigo == detail.Codigo).FirstOrDefault();

                    if (arquivo == null || string.IsNullOrWhiteSpace(arquivo.NomeArquivo))
                        continue;

                    if (!VincularNFe(out mensagemErro, ref cargaPedidoNFe, cargaIntegracaoMercadoLivre, detail, arquivo, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, lockObterPedido))
                        return false;
                }
            }

            #endregion Processar Notas Fiscais

            #region Processar DCe

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDCe = null;

            foreach (Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail in details
                .Where(o => o.TipoDocumento == TipoDocumentoMercadoLivreHandlingUnit.DCe && o.DetailRegistroPai == null))
            {
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao? situacaoDocumento = detail.Situacao;
                    if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Concluido || situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Desconsiderado)
                        continue;

                    List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> arquivosDetail = arquivos.Where(o => o.HandlingUnitDetail.Codigo == detail.Codigo).ToList();

                    foreach (Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo in arquivosDetail)
                    {
                        Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detailDocumentoFilho = details.Where(o => o.DetailRegistroPai?.Codigo == detail.Codigo).FirstOrDefault();

                        if (arquivo == null || string.IsNullOrWhiteSpace(arquivo.NomeArquivo))
                            continue;

                        if (!VincularDCe(out mensagemErro, ref cargaPedidoDCe, cargaIntegracaoMercadoLivre, detail, arquivo, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, lockObterPedido))
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    Log.TratarErro(ex);
                    detail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                    detail.Mensagem = ex.Message;
                    repMercadoLivreHandlingUnitDetail.Atualizar(detail);
                    mensagemErro = ex.Message;
                    return false;
                }
            }

            #endregion Processar DCe

            mensagemErro = null;
            return true;
        }

        public bool ReprocessarHandLingUnitDaCargaPedido(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit repMercadoLivreHandlingUnit = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit(unitOfWork);

            if (!ValidarHandlingUnitDaCargaPedido(out mensagemErro, cargaIntegracaoMercadoLivre))
                return false;

            unitOfWork.Start();

            if (auditado != null)
            {
                string mensagemAuditoria = string.Empty;
                if (cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                    mensagemAuditoria = $"Reprocessou a rota {cargaIntegracaoMercadoLivre.HandlingUnit.Rota} e facility {cargaIntegracaoMercadoLivre.HandlingUnit.Facility} do Mercado Livre.";
                else
                    mensagemAuditoria = $"Reprocessou o handling unit {cargaIntegracaoMercadoLivre.HandlingUnit.ID} do Mercado Livre.";
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaIntegracaoMercadoLivre.Carga, mensagemAuditoria, unitOfWork);
            }

            cargaIntegracaoMercadoLivre.HandlingUnit.Situacao = MercadoLivreHandlingUnitSituacao.AgConsulta;
            cargaIntegracaoMercadoLivre.HandlingUnit.Mensagem = null;
            cargaIntegracaoMercadoLivre.HandlingUnit.DataFimIntegracao = null;
            repMercadoLivreHandlingUnit.Atualizar(cargaIntegracaoMercadoLivre.HandlingUnit);

            unitOfWork.CommitChanges();

            mensagemErro = null;
            return true;
        }

        public bool DesconsiderarDocumentoMercadoLivre(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandLingUnitDetail, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

            if (cargaIntegracaoMercadoLivre == null)
            {
                if (cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                    mensagemErro = "Rota e Facility não encontrado.";
                else
                    mensagemErro = "Handling Unit não encontrado.";
                return false;
            }

            if (mercadoLivreHandLingUnitDetail == null)
            {
                mensagemErro = "Documento não encontrado.";
                return false;
            }

            if (cargaIntegracaoMercadoLivre.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
            {
                mensagemErro = $"A situação da carga ({cargaIntegracaoMercadoLivre.Carga.SituacaoCarga.ObterDescricao()}) não permite a inclusão de documentos.";
                return false;
            }

            if (cargaIntegracaoMercadoLivre.HandlingUnit.Situacao != MercadoLivreHandlingUnitSituacao.Falha)
            {
                if (cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                    mensagemErro = $"Rota e Facility {cargaIntegracaoMercadoLivre.HandlingUnit.Situacao.ObterDescricao()}, situação não permitir alteração.";
                else
                    mensagemErro = $"Handling Unit {cargaIntegracaoMercadoLivre.HandlingUnit.Situacao.ObterDescricao()}, situação não permitir alteração.";
                return false;
            }

            if (cargaIntegracaoMercadoLivre.Carga.ProcessandoDocumentosFiscais)
            {
                mensagemErro = "A carga está processando os documentos fiscais, não sendo possível incluir documentos.";
                return false;
            }

            unitOfWork.Start();

            if (auditado != null)
            {
                string mensagemAuditoria = string.Empty;
                if (cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                    mensagemAuditoria = $"Documento de chave {mercadoLivreHandLingUnitDetail.ChaveAcesso} da rota {cargaIntegracaoMercadoLivre.HandlingUnit.Rota} e facility {cargaIntegracaoMercadoLivre.HandlingUnit.Facility} foi desconsiderado.";
                else
                    mensagemAuditoria = $"Documento de chave {mercadoLivreHandLingUnitDetail.ChaveAcesso} da handling unit {cargaIntegracaoMercadoLivre.HandlingUnit.ID} foi desconsiderado.";
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaIntegracaoMercadoLivre.Carga, mensagemAuditoria, unitOfWork);
            }

            mercadoLivreHandLingUnitDetail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Desconsiderado;
            repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandLingUnitDetail);

            unitOfWork.CommitChanges();

            mensagemErro = null;
            return true;
        }

        public bool RemoverHandlingUnitDaCargaPedido(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre configuracaoIntegracaoMercadoLivre = ObterConfiguracaoIntegracaoMercadoLivre(unitOfWork);

            if (!ValidarHandlingUnitDaCargaPedido(out mensagemErro, cargaIntegracaoMercadoLivre))
                return false;

            if (configuracaoIntegracaoMercadoLivre.LimparComposicaoCargaRetiradaRotaFacility)
            {
                if (!RemoverCTesSubContratacaoENotasFiscaisViculadosAoRotaFacility(out mensagemErro, unitOfWork, cargaIntegracaoMercadoLivre, repMercadoLivreHandlingUnitDetail, auditado))
                    return false;

                RemoverHandlingUnit(unitOfWork, cargaIntegracaoMercadoLivre, repCargaIntegracaoMercadoLivre, repMercadoLivreHandlingUnitDetail, auditado, false);
            }
            else
            {
                RemoverHandlingUnit(unitOfWork, cargaIntegracaoMercadoLivre, repCargaIntegracaoMercadoLivre, repMercadoLivreHandlingUnitDetail, auditado, true);
            }

            mensagemErro = null;
            return true;
        }

        public bool ConfirmarProcessamentoHandlingUnitDaCargaPedido(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit repMercadoLivreHandlingUnit = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit(unitOfWork);
            //Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre configuracaoIntegracaoMercadoLivre = ObterConfiguracaoIntegracaoMercadoLivre(unitOfWork);

            if (cargaIntegracaoMercadoLivre.HandlingUnit.Situacao != MercadoLivreHandlingUnitSituacao.AgConfirmacao)
            {
                mensagemErro = "Situção da integração não permite confirmação.";
                return false;
            }

            unitOfWork.Start();

            if (auditado != null)
            {
                string mensagemAuditoria = string.Empty;
                if (cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                    mensagemAuditoria = $"Confirmou processamento da rota {cargaIntegracaoMercadoLivre.HandlingUnit.Rota} e facility {cargaIntegracaoMercadoLivre.HandlingUnit.Facility} do Mercado Livre.";
                else
                    mensagemAuditoria = $"Confirmou processamento do handling unit {cargaIntegracaoMercadoLivre.HandlingUnit.ID} do Mercado Livre.";
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaIntegracaoMercadoLivre.Carga, mensagemAuditoria, unitOfWork);
            }

            cargaIntegracaoMercadoLivre.HandlingUnit.Situacao = MercadoLivreHandlingUnitSituacao.AgConsulta;
            cargaIntegracaoMercadoLivre.HandlingUnit.Mensagem = null;
            cargaIntegracaoMercadoLivre.HandlingUnit.DataFimIntegracao = null;
            repMercadoLivreHandlingUnit.Atualizar(cargaIntegracaoMercadoLivre.HandlingUnit);

            unitOfWork.CommitChanges();

            mensagemErro = null;
            return true;
        }

        private bool DownloadVincularRotaEFacityNaCargaPedido(out string mensagemErro, string token, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, object lockObterPedido, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, CancellationToken cancellationToken)
        {
            mensagemErro = string.Empty;
            string mensagemErroContinuarProximoDocumento = string.Empty;

            if (!(cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility))
                return true;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre repIntegracaoMercadoLivre = new Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre = repIntegracaoMercadoLivre.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();


            #region Processar CT-e SuContratação

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
            Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidade = null;
            Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFe = null;

            List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail> details = repMercadoLivreHandlingUnitDetail.BuscarPorHandlingUnit(cargaIntegracaoMercadoLivre.HandlingUnit.Codigo);

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);

            int qtdDocumentos = 0;
            bool continuarProximoDocumento = false;
            List<Task<(string mensagemErro, bool continuarProximoDocumento)>> lstTasks = new List<Task<(string mensagemErro, bool continuarProximoDocumento)>>();

            foreach (Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail in details
                .Where(o => o.TipoDocumento == TipoDocumentoMercadoLivreHandlingUnit.CTe &&
                            o.Situacao != MercadoLivreHandlingUnitDetailSituacao.Desconsiderado &&
                            o.Situacao != MercadoLivreHandlingUnitDetailSituacao.Concluido)
                .OrderBy(o => o.Codigo))
            {
                Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detailDocumentoFilho = details.Where(o => o.DetailRegistroPai?.Codigo == detail.Codigo).FirstOrDefault();

                if (pedidoCTeParaSubContratacao == null)
                {
                    try
                    {
                        //Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
                        Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoDetail = null;

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao? situacaoDocumento = detail.Situacao;
                        if (situacaoDocumento == null)
                        {
                            arquivoDetail = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetailEKey(detail.Codigo, detail.ShipmentID.ToString());
                            if (arquivoDetail == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                            else if (arquivoDetail.PedidoCTeParaSubcontratacao == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }

                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Download)
                        {
                            if (!DownloadCTeSubContratacao(out mensagemErro, out continuarProximoDocumento, out arquivoDetail, detail, integracaoMercadoLivre, token, unitOfWork))
                            {
                                /*
                                if (continuarProximoDocumento)
                                {
                                    mensagemErroContinuarProximoDocumento = mensagemErro;
                                    continue;
                                }
                                */

                                Task.WaitAll(lstTasks.ToArray());
                                return false;
                            }

                            situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }

                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento)
                        {
                            qtdDocumentos++;

                            if (!VincularCTeSubcontratacao(out mensagemErro, cargaIntegracaoMercadoLivre, detail, detailDocumentoFilho, arquivoDetail, ref pedidoCTeParaSubContratacao, ref cteTerceiroNFe, ref cteTerceiroQuantidade, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, lockObterPedido))
                            {
                                Task.WaitAll(lstTasks.ToArray());
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.TratarErro(ex);

                        mensagemErro = ex.Message;
                        return false;
                    }

                    continue;
                }

                qtdDocumentos++;

                Task<(string mensagemErro, bool continuarProximoDocumento)> newTask = new Task<(string mensagemErro, bool continuarProximoDocumento)>(() =>
                {
                    string mensagemErro = null;
                    bool continuarProximoDocumentoTask = false;
                    if (!DownloadVincularCTeSubcontratacao(out mensagemErro, out continuarProximoDocumentoTask, cargaIntegracaoMercadoLivre, detail, detailDocumentoFilho, integracaoMercadoLivre, pedidoCTeParaSubContratacao, cteTerceiroNFe, cteTerceiroQuantidade, token, configuracaoTMS, configuracaoGeralCarga, tipoServicoMultisoftware, unitOfWork.StringConexao))
                        return (mensagemErro, continuarProximoDocumentoTask);
                    else
                        return (null, continuarProximoDocumentoTask);
                });

                lstTasks.Add(newTask);
                newTask.Start();
                Thread.Sleep(100);

                // Se o número de tarefas simultâneas atingir o limite
                if (lstTasks.Count >= _quantidadeThreadsExecutarRotaEFacility)
                {
                    // Verifica se o cancelamento foi solicitado
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Task.WaitAll(lstTasks.ToArray());
                        mensagemErro = "Cancelamento solicitado. A tarefa será interrompida.";
                        return false;
                    }

                    // Espera a conclusão de qualquer tarefa para liberar espaço para uma nova
                    Task<(string mensagemErro, bool continuarProximoDocumento)> concluida = Task.WhenAny(lstTasks.ToArray()).Result;
                    var retorno = concluida.Result;

                    if (!string.IsNullOrEmpty(retorno.mensagemErro))
                    {
                        /*
                        if (retorno.continuarProximoDocumento)
                        {
                            mensagemErroContinuarProximoDocumento = mensagemErro;
                            continue;
                        }
                        */

                        Task.WaitAll(lstTasks.ToArray());
                        mensagemErro = retorno.mensagemErro;
                        return false;
                    }

                    // Remove a tarefa concluída da lista
                    lstTasks.Remove(concluida);
                }

                if (qtdDocumentos >= 500)
                {
                    Task.WaitAll(lstTasks.ToArray());

                    TotalizarCTeTerceiro(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo, cteTerceiroNFe.Codigo, cteTerceiroQuantidade.Codigo, unitOfWork);

                    foreach (Task<(string mensagemErro, bool continuarProximoDocumento)> task in lstTasks)
                    {
                        var retorno = task.Result;

                        if (!string.IsNullOrEmpty(retorno.mensagemErro))
                        {
                            /*
                            if (retorno.continuarProximoDocumento)
                            {
                                mensagemErroContinuarProximoDocumento = mensagemErro;
                                continue;
                            }
                            */

                            mensagemErro = retorno.mensagemErro;
                            return false;
                        }
                    }

                    qtdDocumentos = 0;
                    pedidoCTeParaSubContratacao = null;
                    cteTerceiroQuantidade = null;
                    cteTerceiroNFe = null;
                    lstTasks = new List<Task<(string mensagemErro, bool continuarProximoDocumento)>>();
                }
            }

            if (qtdDocumentos > 0)
            {
                System.Threading.Tasks.Task.WaitAll(lstTasks.ToArray());

                TotalizarCTeTerceiro(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo, cteTerceiroNFe.Codigo, cteTerceiroQuantidade.Codigo, unitOfWork);

                foreach (Task<(string mensagemErro, bool continuarProximoDocumento)> task in lstTasks)
                {
                    var retorno = task.Result;

                    if (!string.IsNullOrEmpty(retorno.mensagemErro))
                    {
                        /*
                        if (retorno.continuarProximoDocumento)
                        {
                            mensagemErroContinuarProximoDocumento = mensagemErro;
                            continue;
                        }
                        */

                        mensagemErro = retorno.mensagemErro;
                        return false;
                    }
                }

                qtdDocumentos = 0;
                pedidoCTeParaSubContratacao = null;
                cteTerceiroQuantidade = null;
                cteTerceiroNFe = null;
            }

            #endregion Processar CT-e SuContratação


            #region Processar Notas Fiscais

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNFe = null;

            qtdDocumentos = 0;
            lstTasks = new List<Task<(string mensagemErro, bool continuarProximoDocumento)>>();
            foreach (Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail in details
                .Where(o => o.TipoDocumento == TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal &&
                            o.Situacao != MercadoLivreHandlingUnitDetailSituacao.Desconsiderado &&
                            o.Situacao != MercadoLivreHandlingUnitDetailSituacao.Concluido &&
                            o.DetailRegistroPai == null)
                .OrderBy(o => o.Codigo))
            {
                if (cargaPedidoNFe == null)
                {
                    try
                    {
                        Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoDetail = null;

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao? situacaoDocumento = detail.Situacao;
                        if (situacaoDocumento == null)
                        {
                            arquivoDetail = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetailEKey(detail.Codigo, detail.ShipmentID.ToString());
                            if (arquivoDetail == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                            else if (arquivoDetail.PedidoXMLNotaFiscal == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }

                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Download)
                        {
                            if (!DownloadNFe(out mensagemErro, out continuarProximoDocumento, out arquivoDetail, detail, integracaoMercadoLivre, token, tipoServicoMultisoftware, unitOfWork))
                            {
                                /*
                                if (continuarProximoDocumento)
                                {
                                    mensagemErroContinuarProximoDocumento = mensagemErro;
                                    continue;
                                }
                                */

                                return false;
                            }

                            situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }

                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento)
                        {
                            qtdDocumentos++;

                            if (!VincularNFe(out mensagemErro, ref cargaPedidoNFe, cargaIntegracaoMercadoLivre, detail, arquivoDetail, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, lockObterPedido))
                                return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.TratarErro(ex);
                        detail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                        detail.Mensagem = ex.Message;
                        repMercadoLivreHandlingUnitDetail.Atualizar(detail);
                        mensagemErro = ex.Message;
                        return false;
                    }

                    continue;
                }

                qtdDocumentos++;

                Task<(string mensagemErro, bool continuarProximoDocumento)> newTask = new Task<(string mensagemErro, bool continuarProximoDocumento)>(() =>
                {
                    string mensagemErro = null;
                    bool continuarProximoDocumentoTask = false;
                    if (!DownloadVincularNFe(out mensagemErro, out continuarProximoDocumentoTask, cargaIntegracaoMercadoLivre, detail, integracaoMercadoLivre, cargaPedidoNFe.Codigo, token, configuracaoTMS, configuracaoGeralCarga, tipoServicoMultisoftware, unitOfWork.StringConexao))
                        return (mensagemErro, continuarProximoDocumentoTask);
                    else
                        return (null, continuarProximoDocumentoTask);
                });

                lstTasks.Add(newTask);
                newTask.Start();
                Thread.Sleep(100);

                // Se o número de tarefas simultâneas atingir o limite
                if (lstTasks.Count >= 5)
                {
                    // Verifica se o cancelamento foi solicitado
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Task.WaitAll(lstTasks.ToArray());
                        mensagemErro = "Cancelamento solicitado. A tarefa será interrompida.";
                        return false;
                    }

                    // Espera a conclusão de qualquer tarefa para liberar espaço para uma nova
                    Task<(string mensagemErro, bool continuarProximoDocumento)> concluida = Task.WhenAny(lstTasks.ToArray()).Result;
                    var retorno = concluida.Result;

                    if (!string.IsNullOrEmpty(retorno.mensagemErro))
                    {
                        /*
                        if (retorno.continuarProximoDocumento)
                        {
                            mensagemErroContinuarProximoDocumento = mensagemErro;
                            continue;
                        }
                        */

                        Task.WaitAll(lstTasks.ToArray());
                        mensagemErro = retorno.mensagemErro;
                        return false;
                    }

                    // Remove a tarefa concluída da lista
                    lstTasks.Remove(concluida);
                }
            }

            Task.WaitAll(lstTasks.ToArray());

            foreach (Task<(string mensagemErro, bool continuarProximoDocumento)> task in lstTasks)
            {
                var retorno = task.Result;

                if (!string.IsNullOrEmpty(retorno.mensagemErro))
                {
                    /*
                    if (retorno.continuarProximoDocumento)
                    {
                        mensagemErroContinuarProximoDocumento = mensagemErro;
                        continue;
                    }
                    */

                    mensagemErro = retorno.mensagemErro;
                    return false;
                }
            }

            #endregion


            #region Processar DCe

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDCe = null;

            qtdDocumentos = 0;
            lstTasks = new List<Task<(string mensagemErro, bool continuarProximoDocumento)>>();
            foreach (Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail in details
                .Where(o => o.TipoDocumento == TipoDocumentoMercadoLivreHandlingUnit.DCe &&
                            o.Situacao != MercadoLivreHandlingUnitDetailSituacao.Desconsiderado &&
                            o.Situacao != MercadoLivreHandlingUnitDetailSituacao.Concluido &&
                            o.DetailRegistroPai == null)
                .OrderBy(o => o.Codigo))
            {
                if (cargaPedidoDCe == null)
                {
                    try
                    {
                        Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoDetail = null;

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao? situacaoDocumento = detail.Situacao;
                        if (situacaoDocumento == null)
                        {
                            arquivoDetail = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetailEKey(detail.Codigo, detail.ShipmentID.ToString());
                            if (arquivoDetail == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                            else if (arquivoDetail.PedidoXMLNotaFiscal == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }

                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Download)
                        {
                            if (!DownloadDCe(out mensagemErro, out continuarProximoDocumento, out arquivoDetail, detail, integracaoMercadoLivre, token, tipoServicoMultisoftware, unitOfWork))
                            {
                                return false;
                            }

                            situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }

                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento)
                        {
                            qtdDocumentos++;

                            if (!VincularDCe(out mensagemErro, ref cargaPedidoDCe, cargaIntegracaoMercadoLivre, detail, arquivoDetail, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, lockObterPedido))
                                return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.TratarErro(ex);
                        detail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        detail.Mensagem = ex.Message;
                        repMercadoLivreHandlingUnitDetail.Atualizar(detail);
                        mensagemErro = ex.Message;
                        return false;
                    }

                    continue;
                }

                qtdDocumentos++;

                Task<(string mensagemErro, bool continuarProximoDocumento)> newTask = new Task<(string mensagemErro, bool continuarProximoDocumento)>(() =>
                {
                    string mensagemErro = null;
                    bool continuarProximoDocumentoTask = false;
                    if (!DownloadVincularDCe(out mensagemErro, out continuarProximoDocumentoTask, cargaIntegracaoMercadoLivre, detail, integracaoMercadoLivre, cargaPedidoDCe.Codigo, token, configuracaoTMS, configuracaoGeralCarga, tipoServicoMultisoftware, unitOfWork.StringConexao))
                        return (mensagemErro, continuarProximoDocumentoTask);
                    else
                        return (null, continuarProximoDocumentoTask);
                });

                lstTasks.Add(newTask);
                newTask.Start();
                Thread.Sleep(100);

                // Se o número de tarefas simultâneas atingir o limite
                if (lstTasks.Count >= 5)
                {
                    // Verifica se o cancelamento foi solicitado
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Task.WaitAll(lstTasks.ToArray());
                        mensagemErro = "Cancelamento solicitado. A tarefa será interrompida.";
                        return false;
                    }

                    // Espera a conclusão de qualquer tarefa para liberar espaço para uma nova
                    Task<(string mensagemErro, bool continuarProximoDocumento)> concluida = Task.WhenAny(lstTasks.ToArray()).Result;
                    var retorno = concluida.Result;

                    if (!string.IsNullOrEmpty(retorno.mensagemErro))
                    {
                        Task.WaitAll(lstTasks.ToArray());
                        mensagemErro = retorno.mensagemErro;
                        return false;
                    }

                    // Remove a tarefa concluída da lista
                    lstTasks.Remove(concluida);
                }
            }

            Task.WaitAll(lstTasks.ToArray());

            foreach (Task<(string mensagemErro, bool continuarProximoDocumento)> task in lstTasks)
            {
                var retorno = task.Result;

                if (!string.IsNullOrEmpty(retorno.mensagemErro))
                {
                    mensagemErro = retorno.mensagemErro;
                    return false;
                }
            }

            #endregion


            if (!string.IsNullOrEmpty(mensagemErroContinuarProximoDocumento))
            {
                mensagemErro = mensagemErroContinuarProximoDocumento;
                return false;
            }

            return true;
        }

        public bool EfetuarDownloadNFeComplementaresPorCarga(out string mensagemErro, int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;

            try
            {
                Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
                Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre repIntegracaoMercadoLivre = new Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre = repIntegracaoMercadoLivre.BuscarPrimeiroRegistro();

                if (integracaoMercadoLivre == null || string.IsNullOrWhiteSpace(integracaoMercadoLivre.URL) || string.IsNullOrWhiteSpace(integracaoMercadoLivre.SecretKey) || string.IsNullOrWhiteSpace(integracaoMercadoLivre.ID))
                {
                    mensagemErro = "A configuração da integração com o Mercado Livre não foi realizada.";
                    return false;
                }

                string token = ObterToken(integracaoMercadoLivre);

                if (string.IsNullOrEmpty(token))
                {
                    mensagemErro = "Não foi possível obter o token de integração do Mercado Livre.";
                    return false;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> listaCargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorCarga(codigoCarga);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre in listaCargaIntegracaoMercadoLivre)
                {
                    if (cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.Dispatch)
                    {
                        Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit = cargaIntegracaoMercadoLivre.HandlingUnit;
                        if (!ObterDispatch(out mensagemErro, ref mercadoLivreHandlingUnit, token, integracaoMercadoLivre, tipoServicoMultisoftware, unitOfWork, true))
                            return false;
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail> listaMercadoLivreHandlingUnitDetail = repMercadoLivreHandlingUnitDetail.BuscarNFePendenteDownloadPorHandlingUnit(cargaIntegracaoMercadoLivre.HandlingUnit.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail in listaMercadoLivreHandlingUnitDetail)
                        {
                            Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoDetail = null;

                            if (!DownloadNFe(out mensagemErro, out bool continuarProximoDocumento, out arquivoDetail, mercadoLivreHandlingUnitDetail, integracaoMercadoLivre, token, tipoServicoMultisoftware, unitOfWork))
                                return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                mensagemErro = e.Message;
                return false;
            }

            return true;
        }

        public void VerificarConsultaRotaEFacilityAutomatizada(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoOperacao?.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility && (carga.TipoOperacao?.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente ?? false))
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedidos?.FirstOrDefault()?.Pedido;

                int rota = pedido?.Rota ?? 0;
                string facility = pedido?.Facility;

                if (rota != 0 && !string.IsNullOrEmpty(facility))
                {
                    Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorRotaEFacilityECarga(rota, facility, carga.Codigo);

                    if (cargaIntegracaoMercadoLivre != null)
                        return;

                    int acrescimoDescrescimo = carga.TipoOperacao.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida == TipoTempoAcrescimoDecrescimoDataPrevisaoSaida.Acrescimo ? 1 : -1;
                    DateTime? dataConfirmarProcessamento = (pedido.DataPrevisaoSaida ?? System.DateTime.Now).AddTicks(carga.TipoOperacao.TempoAcrescimoDecrescimoDataPrevisaoSaidaTicks * acrescimoDescrescimo);

                    this.AdicionarHandlingUnitParaConsulta(null, rota, facility, carga, unitOfWork, dataConfirmarProcessamento, carga.TipoOperacao.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente);
                }
            }
        }

        public void IniciarAutomaticamenteRotaEFacility(int codigoCargaIntegracaoMercadoLivre, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit repMercadoLivreHandlingUnit = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorCodigo(codigoCargaIntegracaoMercadoLivre);
                Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit = cargaIntegracaoMercadoLivre.HandlingUnit;

                if (mercadoLivreHandlingUnit.Situacao == MercadoLivreHandlingUnitSituacao.AgConfirmacao)
                {
                    mercadoLivreHandlingUnit.Situacao = MercadoLivreHandlingUnitSituacao.AgConsulta;
                    mercadoLivreHandlingUnit.Mensagem = string.Empty;

                    repMercadoLivreHandlingUnit.Atualizar(mercadoLivreHandlingUnit);

                    string mensagemAuditoria = $"Confirmação automatica do processamento da rota {cargaIntegracaoMercadoLivre.HandlingUnit.Rota} e facility {cargaIntegracaoMercadoLivre.HandlingUnit.Facility} do Mercado Livre.";
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaIntegracaoMercadoLivre.Carga, mensagemAuditoria, unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion

        #region Métodos Privados

        private bool DownloadVincularCTeSubcontratacao(out string mensagemErro, out bool continuarProximoDocumento, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detailDocumentoFilho, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFe, Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidade, string token, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string StringConexao)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    try
                    {
                        mensagemErro = null;
                        continuarProximoDocumento = false;

                        Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
                        Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoDetail = null;

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao? situacaoDocumento = detail.Situacao;
                        if (situacaoDocumento == null)
                        {
                            arquivoDetail = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetailEKey(detail.Codigo, detail.ShipmentID.ToString());
                            if (arquivoDetail == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                            else if (arquivoDetail?.PedidoCTeParaSubcontratacao == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }

                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Download)
                        {
                            if (!DownloadCTeSubContratacao(out mensagemErro, out continuarProximoDocumento, out arquivoDetail, detail, integracaoMercadoLivre, token, unitOfWork))
                                return false;

                            situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }

                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento)
                        {
                            if (arquivoDetail == null)
                                arquivoDetail = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetailEKey(detail.Codigo, detail.ShipmentID.ToString());

                            if (!VincularCTeSubcontratacao(out mensagemErro, cargaIntegracaoMercadoLivre, detail, detailDocumentoFilho, arquivoDetail, ref pedidoCTeParaSubContratacao, ref cteTerceiroNFe, ref cteTerceiroQuantidade, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, null))
                                return false;
                        }
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);

                        mensagemErro = e.Message;
                        continuarProximoDocumento = false;
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);

                mensagemErro = e.Message;
                continuarProximoDocumento = false;
                return false;
            }
        }

        private void TotalizarCTeTerceiro(int codigoCTeTerceiro, int codigoCteTerceiroNFe, int codigoCTeTerceiroQuantidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional repCTeTerceiroDocumentoAdicional = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional(unitOfWork);

            var cteTerceiroNFe = repCTeTerceiroNFe.BuscarPorCodigo(codigoCteTerceiroNFe);
            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiroAtualizar = repCTeTerceiro.BuscarPorCodigo(codigoCTeTerceiro);
            cteTerceiroAtualizar.ValorTotalMercadoria = cteTerceiroAtualizar.ValorTotalMercadoriaOriginal + repCTeTerceiroDocumentoAdicional.BuscarTotalValorMercadoria(cteTerceiroAtualizar.Codigo);
            repCTeTerceiro.Atualizar(cteTerceiroAtualizar);

            Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFeAtualizar = repCTeTerceiroNFe.BuscarPorCodigo(cteTerceiroNFe.Codigo);
            if (cteTerceiroNFeAtualizar != null)
            {
                cteTerceiroNFeAtualizar.ValorTotal = cteTerceiroAtualizar.ValorTotalMercadoria;
                repCTeTerceiroNFe.Atualizar(cteTerceiroNFeAtualizar);
            }

            Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidadeAtualizar = repCTeTerceiroQuantidade.BuscarPorCodigo(codigoCTeTerceiroQuantidade);
            if (cteTerceiroQuantidadeAtualizar != null)
            {
                string medida = cteTerceiroQuantidadeAtualizar.TipoMedida;
                cteTerceiroQuantidadeAtualizar.Quantidade = cteTerceiroQuantidadeAtualizar.QuantidadeOriginal + repCTeTerceiroDocumentoAdicional.BuscarTotalQuantidade(cteTerceiroAtualizar.Codigo);
                repCTeTerceiroQuantidade.Atualizar(cteTerceiroQuantidadeAtualizar);
            }
        }

        private bool DownloadCTeSubContratacao(out string mensagemErro, out bool continuarProximoDocumento, out Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoDetail, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, string token, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;
            continuarProximoDocumento = false;
            arquivoDetail = null;

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

            var retTransmitir = transmitirWSDownloadCTe(mercadoLivreHandlingUnitDetail, integracaoMercadoLivre, token).Result;

            if (!retTransmitir.sucesso)
            {
                mercadoLivreHandlingUnitDetail.Mensagem = retTransmitir.mensagemErro.Left(500);
                repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                if (!string.IsNullOrEmpty(retTransmitir.response))
                {
                    Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
                    {
                        ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retTransmitir.requestUri, "json", unitOfWork, _caminhoArquivosIntegracao),
                        ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retTransmitir.response, "json", unitOfWork, _caminhoArquivosIntegracao),
                        Data = DateTime.Now,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                        Mensagem = mercadoLivreHandlingUnitDetail.HandlingUnit.Mensagem
                    };

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                }

                mensagemErro = retTransmitir.mensagemErro;
                continuarProximoDocumento = retTransmitir.continuarProximoDocumento;
                return false;
            }

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);

            unitOfWork.Start();

            try
            {
                arquivoDetail = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo()
                {
                    HandlingUnitDetail = mercadoLivreHandlingUnitDetail,
                    Key = mercadoLivreHandlingUnitDetail.ShipmentID.ToString(),
                    NomeArquivo = SalvarDocumento(retTransmitir.response),
                    TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.CTe
                };

                repMercadoLivreHandlingUnitDetailArquivo.Inserir(arquivoDetail);

                mercadoLivreHandlingUnitDetail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                mercadoLivreHandlingUnitDetail.Mensagem = null;
                repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            return true;
        }

        private async Task<(bool sucesso, string mensagemErro, string response, string requestUri, bool continuarProximoDocumento)> transmitirWSDownloadCTe(Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string uri = ObterURI(integracaoMercadoLivre);
            string requestUri = null;
            if (!mercadoLivreHandlingUnitDetail.RequestDocument.Contains("?"))
                requestUri = mercadoLivreHandlingUnitDetail.RequestDocument + $"?access_token={token}";
            else
                requestUri = mercadoLivreHandlingUnitDetail.RequestDocument + $"&access_token={token}";

            IFlurlResponse result = null;
            string response = string.Empty;
            string mensagemErro = string.Empty;
            bool continuarProximoDocumento = false;

            try
            {
                result = await requestUri.WithHeader("Content-Type", "application/json").GetAsync();
                response = await result.ResponseMessage.Content.ReadAsStringAsync();
            }
            catch (FlurlHttpException e)
            {
                Servicos.Log.TratarErro(e);

                if (e.StatusCode == 403 || e.StatusCode == 404)
                    continuarProximoDocumento = true;

                mensagemErro = $"Ocorreu um erro ao efetuar o download do CT-e - {e.Message}";
                return (false, mensagemErro, response, requestUri, continuarProximoDocumento);
            }
            catch (TaskCanceledException e)
            {
                Servicos.Log.TratarErro(e);

                mensagemErro = $"Ocorreu um erro ao efetuar o download do CT-e - {requestUri}";
                return (false, mensagemErro, response, requestUri, continuarProximoDocumento);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);

                mensagemErro = $"Ocorreu um erro ao efetuar o download do CT-e - {e.Message}";
                return (false, mensagemErro, response, requestUri, continuarProximoDocumento);
            }

            if (!result.ResponseMessage.IsSuccessStatusCode)
            {
                dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(response);

                if (!string.IsNullOrEmpty(objRetorno?.message))
                    mensagemErro = $"StatusCode: {result.StatusCode}, Message: {objRetorno?.message}, Url: {requestUri}";
                else
                    mensagemErro = $"StatusCode: {result.StatusCode}, Url: {requestUri}";

                return (false, mensagemErro, response, requestUri, continuarProximoDocumento);
            }

            return (true, mensagemErro, response, requestUri, continuarProximoDocumento);
        }

        private bool DownloadVincularNFe(out string mensagemErro, out bool continuarProximoDocumento, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, int CodigoCargaPedidoNFe, string token, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string StringConexao)
        {
            mensagemErro = null;
            continuarProximoDocumento = false;

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    try
                    {
                        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNFe = repCargaPedido.BuscarPorCodigo(CodigoCargaPedidoNFe);
                        Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoDetail = null;

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao? situacaoDocumento = detail.Situacao;
                        if (situacaoDocumento == null)
                        {
                            arquivoDetail = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetailEKey(detail.Codigo, detail.ShipmentID.ToString());
                            if (arquivoDetail == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                            else if (arquivoDetail.PedidoXMLNotaFiscal == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }

                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Download)
                        {
                            if (!DownloadNFe(out mensagemErro, out continuarProximoDocumento, out arquivoDetail, detail, integracaoMercadoLivre, token, tipoServicoMultisoftware, unitOfWork))
                                return false;

                            situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }
                        else
                            Thread.Sleep(200);


                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento)
                        {
                            if (!VincularNFe(out mensagemErro, ref cargaPedidoNFe, cargaIntegracaoMercadoLivre, detail, arquivoDetail, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, null))
                                return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.TratarErro(ex);

                        mensagemErro = ex.Message;
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                mensagemErro = ex.Message;
                return false;
            }
        }

        private bool DownloadNFe(out string mensagemErro, out bool continuarProximoDocumento, out Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoDetail, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, string token, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;
            continuarProximoDocumento = false;
            arquivoDetail = null;

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

            var retTransmitir = transmitirWSDownloadNFe(mercadoLivreHandlingUnitDetail, integracaoMercadoLivre, token).Result;

            if (!retTransmitir.sucesso)
            {
                mercadoLivreHandlingUnitDetail.Mensagem = retTransmitir.mensagemErro.Left(500);
                repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                if (!string.IsNullOrEmpty(retTransmitir.response))
                {
                    Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
                    {
                        ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retTransmitir.requestUri, "json", unitOfWork, _caminhoArquivosIntegracao),
                        ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retTransmitir.response, "json", unitOfWork, _caminhoArquivosIntegracao),
                        Data = DateTime.Now,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                        Mensagem = mercadoLivreHandlingUnitDetail.HandlingUnit.Mensagem
                    };

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                }

                mensagemErro = retTransmitir.mensagemErro;
                continuarProximoDocumento = retTransmitir.continuarProximoDocumento;
                return false;
            }

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            unitOfWork.Start();

            try
            {
                arquivoDetail = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo()
                {
                    HandlingUnitDetail = mercadoLivreHandlingUnitDetail,
                    Key = mercadoLivreHandlingUnitDetail.ShipmentID.ToString(),
                    NomeArquivo = SalvarDocumento(retTransmitir.response),
                    TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal
                };

                repMercadoLivreHandlingUnitDetailArquivo.Inserir(arquivoDetail);

                mercadoLivreHandlingUnitDetail.Situacao = mercadoLivreHandlingUnitDetail.DetailRegistroPai != null ? MercadoLivreHandlingUnitDetailSituacao.Concluido : MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                mercadoLivreHandlingUnitDetail.Mensagem = null;
                repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                if (mercadoLivreHandlingUnitDetail.DetailRegistroPai != null)
                {
                    Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                    byte[] data = ObterDocumento(arquivoDetail.NomeArquivo);
                    MemoryStream stream = new MemoryStream(data);

                    System.IO.StreamReader streamReader = new StreamReader(stream);

                    if (!serNFe.BuscarDadosNotaFiscal(out mensagemErro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, streamReader, unitOfWork, null, true, false, false, tipoServicoMultisoftware, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                    {
                        mercadoLivreHandlingUnitDetail.Mensagem = mensagemErro.Left(500);
                        repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                        return false;
                    }

                    xmlNotaFiscal.SemCarga = false;

                    if (xmlNotaFiscal.Codigo == 0)
                    {
                        xmlNotaFiscal.DataRecebimento = DateTime.Now;

                        repXmlNotaFiscal.Inserir(xmlNotaFiscal);
                    }
                    else
                        repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

                    mercadoLivreHandlingUnitDetail.XMLNotaFiscal = xmlNotaFiscal;
                    repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                    Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo detailArquivoRegistroPai = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetail(mercadoLivreHandlingUnitDetail.DetailRegistroPai).FirstOrDefault();

                    if (detailArquivoRegistroPai != null)
                    {
                        Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe repCTeTerceiroDocumentoAdicionalNFe = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe(unitOfWork);
                        Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe cteTerceiroDocumentoAdicionalNFe = repCTeTerceiroDocumentoAdicionalNFe.BuscarPorCTeTerceiroEChaveAcessoNFe(detailArquivoRegistroPai.PedidoCTeParaSubcontratacao.CTeTerceiro.Codigo, mercadoLivreHandlingUnitDetail.ChaveAcesso);

                        if (cteTerceiroDocumentoAdicionalNFe != null)
                        {
                            cteTerceiroDocumentoAdicionalNFe.XMLNotaFiscal = xmlNotaFiscal;
                            repCTeTerceiroDocumentoAdicionalNFe.Atualizar(cteTerceiroDocumentoAdicionalNFe);
                        }
                    }
                }

                unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            return true;
        }

        private bool DownloadVincularDCe(out string mensagemErro, out bool continuarProximoDocumento, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, int CodigoCargaPedidoNFe, string token, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string StringConexao)
        {
            mensagemErro = null;
            continuarProximoDocumento = false;

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    try
                    {
                        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNFe = repCargaPedido.BuscarPorCodigo(CodigoCargaPedidoNFe);
                        Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoDetail = null;

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao? situacaoDocumento = detail.Situacao;
                        if (situacaoDocumento == null)
                        {
                            arquivoDetail = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetailEKey(detail.Codigo, detail.ShipmentID.ToString());
                            if (arquivoDetail == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                            else if (arquivoDetail.PedidoXMLNotaFiscal == null)
                                situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }

                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Download)
                        {
                            if (!DownloadDCe(out mensagemErro, out continuarProximoDocumento, out arquivoDetail, detail, integracaoMercadoLivre, token, tipoServicoMultisoftware, unitOfWork))
                                return false;

                            situacaoDocumento = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                        }
                        else
                            Thread.Sleep(200);


                        if (situacaoDocumento == MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento)
                        {
                            if (!VincularDCe(out mensagemErro, ref cargaPedidoNFe, cargaIntegracaoMercadoLivre, detail, arquivoDetail, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, null))
                                return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.TratarErro(ex);

                        mensagemErro = ex.Message;
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                mensagemErro = ex.Message;
                return false;
            }
        }

        private bool DownloadDCe(out string mensagemErro, out bool continuarProximoDocumento, out Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoDetail, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, string token, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;
            continuarProximoDocumento = false;
            arquivoDetail = null;

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

            var retTransmitir = transmitirWSDownloadNFe(mercadoLivreHandlingUnitDetail, integracaoMercadoLivre, token).Result;

            if (!retTransmitir.sucesso)
            {
                mercadoLivreHandlingUnitDetail.Mensagem = retTransmitir.mensagemErro.Left(500);
                repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                if (!string.IsNullOrEmpty(retTransmitir.response))
                {
                    Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
                    {
                        ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retTransmitir.requestUri, "json", unitOfWork, _caminhoArquivosIntegracao),
                        ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retTransmitir.response, "json", unitOfWork, _caminhoArquivosIntegracao),
                        Data = DateTime.Now,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                        Mensagem = mercadoLivreHandlingUnitDetail.HandlingUnit.Mensagem
                    };

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                }

                mensagemErro = retTransmitir.mensagemErro;
                continuarProximoDocumento = retTransmitir.continuarProximoDocumento;
                return false;
            }

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            unitOfWork.Start();

            try
            {
                arquivoDetail = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo()
                {
                    HandlingUnitDetail = mercadoLivreHandlingUnitDetail,
                    Key = mercadoLivreHandlingUnitDetail.ShipmentID.ToString(),
                    NomeArquivo = SalvarDocumento(retTransmitir.response),
                    TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.DCe
                };

                repMercadoLivreHandlingUnitDetailArquivo.Inserir(arquivoDetail);

                mercadoLivreHandlingUnitDetail.Situacao = mercadoLivreHandlingUnitDetail.DetailRegistroPai != null ? MercadoLivreHandlingUnitDetailSituacao.Concluido : MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                mercadoLivreHandlingUnitDetail.Mensagem = null;
                repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                if (mercadoLivreHandlingUnitDetail.DetailRegistroPai != null)
                {
                    Servicos.Embarcador.DCe.DCe serDCe = new Servicos.Embarcador.DCe.DCe(unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                    byte[] data = ObterDocumento(arquivoDetail.NomeArquivo);
                    MemoryStream stream = new MemoryStream(data);

                    System.IO.StreamReader streamReader = new StreamReader(stream);

                    if (!serDCe.BuscarDadosDCe(out mensagemErro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, streamReader, unitOfWork, null, true, false, false, tipoServicoMultisoftware, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                    {
                        mercadoLivreHandlingUnitDetail.Mensagem = mensagemErro.Left(500);
                        repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                        return false;
                    }

                    xmlNotaFiscal.SemCarga = false;

                    if (xmlNotaFiscal.Codigo == 0)
                    {
                        xmlNotaFiscal.DataRecebimento = DateTime.Now;

                        repXmlNotaFiscal.Inserir(xmlNotaFiscal);
                    }
                    else
                        repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

                    mercadoLivreHandlingUnitDetail.XMLNotaFiscal = xmlNotaFiscal;
                    repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                    Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo detailArquivoRegistroPai = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetail(mercadoLivreHandlingUnitDetail.DetailRegistroPai).FirstOrDefault();

                    if (detailArquivoRegistroPai != null)
                    {
                        Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe repCTeTerceiroDocumentoAdicionalNFe = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe(unitOfWork);
                        Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe cteTerceiroDocumentoAdicionalNFe = repCTeTerceiroDocumentoAdicionalNFe.BuscarPorCTeTerceiroEChaveAcessoNFe(detailArquivoRegistroPai.PedidoCTeParaSubcontratacao.CTeTerceiro.Codigo, mercadoLivreHandlingUnitDetail.ChaveAcesso);

                        if (cteTerceiroDocumentoAdicionalNFe != null)
                        {
                            cteTerceiroDocumentoAdicionalNFe.XMLNotaFiscal = xmlNotaFiscal;
                            repCTeTerceiroDocumentoAdicionalNFe.Atualizar(cteTerceiroDocumentoAdicionalNFe);
                        }
                    }
                }

                unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            return true;
        }

        private async Task<(bool sucesso, string mensagemErro, string response, string requestUri, bool continuarProximoDocumento)> transmitirWSDownloadNFe(Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string uri = ObterURI(integracaoMercadoLivre);
            string requestUri = null;
            if (!mercadoLivreHandlingUnitDetail.RequestDocument.Contains("?"))
                requestUri = mercadoLivreHandlingUnitDetail.RequestDocument + $"?access_token={token}";
            else
                requestUri = mercadoLivreHandlingUnitDetail.RequestDocument + $"&access_token={token}";

            IFlurlResponse result = null;
            string response = string.Empty;
            string mensagemErro = string.Empty;
            bool continuarProximoDocumento = false;

            try
            {
                result = await requestUri.WithHeader("Content-Type", "application/json").GetAsync();
                response = await result.ResponseMessage.Content.ReadAsStringAsync();
            }
            catch (FlurlHttpException e)
            {
                Servicos.Log.TratarErro(e);

                if (e.StatusCode == 403 || e.StatusCode == 404)
                    continuarProximoDocumento = true;

                mensagemErro = $"Ocorreu um erro ao efetuar o download do NF-e - {e.Message}";
                return (false, mensagemErro, response, requestUri, continuarProximoDocumento);
            }
            catch (TaskCanceledException e)
            {
                Servicos.Log.TratarErro(e);

                mensagemErro = $"Ocorreu um erro ao efetuar o download da NF-e - {requestUri}";
                return (false, mensagemErro, response, requestUri, continuarProximoDocumento);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);

                mensagemErro = $"Ocorreu um erro ao efetuar o download da NF-e - {e.Message}";
                return (false, mensagemErro, response, requestUri, continuarProximoDocumento);
            }

            if (!result.ResponseMessage.IsSuccessStatusCode)
            {
                dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(response);

                if (!string.IsNullOrEmpty(objRetorno?.message))
                    mensagemErro = $"StatusCode: {result.StatusCode.ToString()}, Message: {objRetorno?.message}, Url: {requestUri}";
                else
                    mensagemErro = $"StatusCode: {result.StatusCode.ToString()}, Url: {requestUri}";

                return (false, mensagemErro, response, requestUri, continuarProximoDocumento);
            }

            return (true, mensagemErro, response, requestUri, continuarProximoDocumento);
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre ObterTipoIntegracaoMercadoLivre(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre tipoIntegracaoMercadoLivre = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre.HandlingUnit;

            if (carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false)
            {
                if (carga.TipoOperacao.TipoIntegracaoMercadoLivre.HasValue)
                    tipoIntegracaoMercadoLivre = carga.TipoOperacao.TipoIntegracaoMercadoLivre.Value;
            }
            else
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                if (tomador != null)
                {
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                    {
                        if (tomador.TipoIntegracaoMercadoLivre.HasValue)
                            tipoIntegracaoMercadoLivre = tomador.TipoIntegracaoMercadoLivre.Value;
                    }
                    else if (tomador.GrupoPessoas != null)
                    {
                        if (tomador.GrupoPessoas.TipoIntegracaoMercadoLivre.HasValue)
                            tipoIntegracaoMercadoLivre = tomador.GrupoPessoas.TipoIntegracaoMercadoLivre.Value;
                    }
                }
            }

            return tipoIntegracaoMercadoLivre;
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaPedido ObterCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMercadoLivreHandlingUnit tipoDocumento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (tipoDocumento == TipoDocumentoMercadoLivreHandlingUnit.CTe)
                {
                    if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SubContratada ||
                        cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Redespacho ||
                        cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.RedespachoIntermediario)
                        return cargaPedido;
                }
                else if (tipoDocumento == TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal)
                {
                    if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Normal && !repPedidoXMLNotaFiscal.ContemDocumentoLancadoTipo(cargaPedido.Codigo, TipoDocumento.Outros))
                        return cargaPedido;
                }
                else if (tipoDocumento == TipoDocumentoMercadoLivreHandlingUnit.DCe)
                {
                    if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Normal && repPedidoXMLNotaFiscal.ContemDocumentoLancadoTipo(cargaPedido.Codigo, TipoDocumento.Outros))
                        return cargaPedido;
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (!repPedidoCTeParaSubcontratacao.ExistePorCargaPedido(cargaPedido.Codigo) &&
                    !repPedidoXMLNotaFiscal.VerificarSeExistePorCargaPedido(cargaPedido.Codigo))
                    return cargaPedido;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoExistente = cargaPedidos.FirstOrDefault();

            if (!Servicos.Embarcador.Carga.CargaPedido.Duplicar(out string mensagem, out Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNovo, cargaPedidoExistente, cargaPedidoExistente.Pedido.NumeroPedidoEmbarcador, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga))
                throw new Exception(mensagem);

            return cargaPedidoNovo;
        }

        private bool VincularNFe(out string mensagemErro, ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, object lockObterPedido)
        {
            mensagemErro = string.Empty;

            if (arquivo == null)
            {
                Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
                arquivo = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetailEKey(detail.Codigo, detail.ShipmentID.ToString());
            }

            if (cargaPedido == null)
            {
                lock (lockObterPedido)
                {
                    cargaPedido = ObterCargaPedido(cargaIntegracaoMercadoLivre.Carga, TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga);

                    if (!VincularNFeLock(out mensagemErro, cargaPedido, arquivo, detail, tipoServicoMultisoftware, configuracaoTMS, unitOfWork))
                        return false;
                }
            }
            else
            {
                if (!VincularNFeLock(out mensagemErro, cargaPedido, arquivo, detail, tipoServicoMultisoftware, configuracaoTMS, unitOfWork))
                    return false;
            }

            return true;
        }

        private bool VincularNFeLock(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
           
            byte[] data = ObterDocumento(arquivo.NomeArquivo);
            MemoryStream stream = new MemoryStream(data);

            System.IO.StreamReader streamReader = new StreamReader(stream);

            if (!serNFe.BuscarDadosNotaFiscal(out mensagemErro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, streamReader, unitOfWork, null, true, false, false, tipoServicoMultisoftware, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
            {
                detail.Mensagem = mensagemErro.Left(500);
                detail.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                repMercadoLivreHandlingUnitDetail.Atualizar(detail);

                return false;
            }

            unitOfWork.Start();

            try
            {
                mensagemErro = serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, out bool msgAlertaObservacao, out bool notaFiscalEmOutraCarga);

                if (!string.IsNullOrWhiteSpace(mensagemErro) && !msgAlertaObservacao)
                {
                    unitOfWork.Rollback();

                    detail.Mensagem = mensagemErro.Left(500);
                    detail.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                    repMercadoLivreHandlingUnitDetail.Atualizar(detail);

                    return false;
                }

                xmlNotaFiscal.SemCarga = false;

                if (xmlNotaFiscal.Codigo == 0)
                {
                    xmlNotaFiscal.DataRecebimento = DateTime.Now;

                    repXmlNotaFiscal.Inserir(xmlNotaFiscal);
                }
                else
                    repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga, null);

                if (!configuracaoTMS.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga && pedidoXMLNotaFiscal != null && pedidoXMLNotaFiscal.CargaPedido.Codigo != cargaPedido.Codigo)
                {
                    mensagemErro = $"A NF-e {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero} já foi adicionada ao pedido {pedidoXMLNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador}.";

                    unitOfWork.Rollback();

                    detail.Mensagem = mensagemErro.Left(500);
                    detail.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                    repMercadoLivreHandlingUnitDetail.Atualizar(detail);

                    return false;
                }

                arquivo.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;

                repMercadoLivreHandlingUnitDetailArquivo.Atualizar(arquivo);

                detail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Concluido;
                detail.Mensagem = null;
                repMercadoLivreHandlingUnitDetail.Atualizar(detail);

                unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            return true;
        }

        private bool VincularCTeSubcontratacao(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detailDocumentoFilho, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo, ref Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, ref Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFe, ref Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidade, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, object lockObterPedido, bool atualizarValores = true)
        {
            mensagemErro = string.Empty;

            if (arquivo == null)
            {
                Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
                arquivo = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetailEKey(detail.Codigo, detail.ShipmentID.ToString());
            }

            Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);

            Servicos.Cliente servicoCliente = new Servicos.Cliente();

            byte[] data = ObterDocumento(arquivo.NomeArquivo);
            MemoryStream stream = new MemoryStream(data);

            object objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(stream);

            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTePorObjeto(objCTe);
            Dominio.Entidades.Cliente emitente = null;
            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoEmissor = servicoCliente.ConverterParaTransportadorTerceiro(cte.Emitente, "emitente do CT-e", unitOfWork, naoAtualizarDadosNaIntegracaoMercadoLivre: NaoAtualizarDadosPessoaEmImportacoesDeNotasFiscaisOuIntegracoes);

            if (retornoConversaoEmissor.Status)
                emitente = retornoConversaoEmissor.cliente;
            else
            {
                mensagemErro = retornoConversaoEmissor.Mensagem;
                return false;
            }

            if (pedidoCTeParaSubContratacao == null)
            {
                lock (lockObterPedido)
                {
                    if (!VincularCTeSubcontratacaoLock(out mensagemErro, cargaIntegracaoMercadoLivre, detail, detailDocumentoFilho, arquivo, cte, emitente, ref pedidoCTeParaSubContratacao, ref cteTerceiroNFe, ref cteTerceiroQuantidade, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, unitOfWork, atualizarValores))
                        return false;
                }
            }
            else
            {
                if (!VincularCTeSubcontratacaoLock(out mensagemErro, cargaIntegracaoMercadoLivre, detail, detailDocumentoFilho, arquivo, cte, emitente, ref pedidoCTeParaSubContratacao, ref cteTerceiroNFe, ref cteTerceiroQuantidade, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, unitOfWork, atualizarValores))
                    return false;
            }

            return true;
        }

        private bool VincularCTeSubcontratacaoLock(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detailDocumentoFilho, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, Dominio.Entidades.Cliente emitente, ref Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, ref Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe cteTerceiroNFe, ref Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cteTerceiroQuantidade, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Repositorio.UnitOfWork unitOfWork, bool atualizarValores)
        {
            unitOfWork.Start();

            try
            {
                Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional repCTeTerceiroDocumentoAdicional = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe repCTeTerceiroDocumentoAdicionalNFe = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe(unitOfWork);
                Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);
                Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);

                mensagemErro = string.Empty;
                bool inseriu = false;

                if (pedidoCTeParaSubContratacao == null)
                {
                    inseriu = true;

                    mensagemErro = serCargaCteParaSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cte, ObterCargaPedido(cargaIntegracaoMercadoLivre.Carga, TipoDocumentoMercadoLivreHandlingUnit.CTe, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga), tipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);

                    if (pedidoCTeParaSubContratacao?.CTeTerceiro != null)
                    {
                        repCTeTerceiroDocumentoAdicional.RemoverPorCTeTerceiro(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo);

                        cteTerceiroQuantidade = repCTeTerceiroQuantidade.BuscarPorCTeTerceiroEUnidadeMedida(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo, Dominio.Enumeradores.UnidadeMedida.KG);
                        cteTerceiroNFe = repCTeTerceiroNFe.BuscarPrimeiroPorCTeTerceiro(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo);
                    }
                }

                if (!string.IsNullOrEmpty(mensagemErro))
                {
                    unitOfWork.Rollback();

                    detail.Mensagem = mensagemErro.Left(500);
                    detail.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                    repMercadoLivreHandlingUnitDetail.Atualizar(detail);

                    return false;
                }

                arquivo.PedidoCTeParaSubcontratacao = pedidoCTeParaSubContratacao;

                repMercadoLivreHandlingUnitDetailArquivo.Atualizar(arquivo);

                #region Adicionar CT-e SubContratação Adicional

                Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional documentoAdicional = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional()
                {
                    Chave = cte.Chave,
                    CTeTerceiro = pedidoCTeParaSubContratacao.CTeTerceiro,
                    Numero = cte.Numero,
                    Emitente = emitente,
                    ValorTotalMercadoria = cte.InformacaoCarga?.ValorTotalCarga ?? 0m
                };

                repCTeTerceiroDocumentoAdicional.Inserir(documentoAdicional);

                if (detailDocumentoFilho != null)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe documentoAdicionalNFe = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe()
                    {
                        Chave = detailDocumentoFilho.ChaveAcesso,
                        CTeTerceiro = pedidoCTeParaSubContratacao.CTeTerceiro,
                        CTeTerceiroDocumentoAdicional = documentoAdicional,
                        Numero = detailDocumentoFilho.ShipmentID,
                        XMLNotaFiscal = detailDocumentoFilho.XMLNotaFiscal
                    };

                    repCTeTerceiroDocumentoAdicionalNFe.Inserir(documentoAdicionalNFe);
                }

                #endregion Adicionar CT-e SubContratação Adicional

                if (atualizarValores)
                {
                    if (!inseriu)
                    {
                        pedidoCTeParaSubContratacao.CTeTerceiro.ValorTotalMercadoria += cte.InformacaoCarga?.ValorTotalCarga ?? 0m;

                        if (cteTerceiroNFe != null)
                            cteTerceiroNFe.ValorTotal += cte.InformacaoCarga?.ValorTotalCarga ?? 0m;
                    }

                    if (cteTerceiroQuantidade != null)
                    {
                        string medida = cteTerceiroQuantidade.TipoMedida;

                        cteTerceiroQuantidade.Quantidade += cte.QuantidadesCarga?.Where(o => o.Medida == medida).Sum(o => o.Quantidade) ?? 0m;
                    }
                }

                detail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Concluido;
                detail.Mensagem = null;
                repMercadoLivreHandlingUnitDetail.Atualizar(detail);

                unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            return true;
        }

        private bool VincularDCe(out string mensagemErro, ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, object lockObterPedido)
        {
            mensagemErro = string.Empty;

            if (arquivo == null)
            {
                Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
                arquivo = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetailEKey(detail.Codigo, detail.ShipmentID.ToString());
            }

            if (cargaPedido == null)
            {
                lock (lockObterPedido)
                {
                    cargaPedido = ObterCargaPedido(cargaIntegracaoMercadoLivre.Carga, TipoDocumentoMercadoLivreHandlingUnit.DCe, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga);

                    cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Normal;

                    if (!VincularDCeLock(out mensagemErro, cargaPedido, arquivo, detail, tipoServicoMultisoftware, configuracaoTMS, unitOfWork))
                        return false;
                }
            }
            else
            {
                if (!VincularDCeLock(out mensagemErro, cargaPedido, arquivo, detail, tipoServicoMultisoftware, configuracaoTMS, unitOfWork))
                    return false;
            }

            return true;
        }

        private bool VincularDCeLock(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail detail, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.Embarcador.DCe.DCe serDCe = new Servicos.Embarcador.DCe.DCe(unitOfWork);

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            byte[] data = ObterDocumento(arquivo.NomeArquivo);
            MemoryStream stream = new MemoryStream(data);

            System.IO.StreamReader streamReader = new StreamReader(stream);

            if (!serDCe.BuscarDadosDCe(out mensagemErro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, streamReader, unitOfWork, null, true, false, false, tipoServicoMultisoftware, false, false, cargaPedido, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
            {
                detail.Mensagem = mensagemErro.Left(500);
                detail.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                repMercadoLivreHandlingUnitDetail.Atualizar(detail);

                return false;
            }

            unitOfWork.Start();

            try
            {
                xmlNotaFiscal.SemCarga = false;

                if (xmlNotaFiscal.Codigo == 0)
                {
                    xmlNotaFiscal.DataRecebimento = DateTime.Now;

                    repXmlNotaFiscal.Inserir(xmlNotaFiscal);
                }
                else
                    repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoDeCarga, null);

                if (!configuracaoTMS.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga && pedidoXMLNotaFiscal != null && pedidoXMLNotaFiscal.CargaPedido.Codigo != cargaPedido.Codigo)
                {
                    mensagemErro = $"A NF-e {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero} já foi adicionada ao pedido {pedidoXMLNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador}.";

                    unitOfWork.Rollback();

                    detail.Mensagem = mensagemErro.Left(500);
                    detail.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                    repMercadoLivreHandlingUnitDetail.Atualizar(detail);

                    return false;
                }

                arquivo.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;

                repMercadoLivreHandlingUnitDetailArquivo.Atualizar(arquivo);

                detail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Concluido;
                detail.Mensagem = null;
                repMercadoLivreHandlingUnitDetail.Atualizar(detail);

                unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }

            return true;
        }

        private void InformarCargaAtualizada(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Repositorio.UnitOfWork unitOfWork)
        {
            //Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);

            //if (!repCargaIntegracaoMercadoLivre.ExisteComSituacaoPorCarga(cargaIntegracaoMercadoLivre.CargaPedido.Carga.Codigo, MercadoLivreHandlingUnitSituacao.AgConsulta))
            //{
            //    MSMQ.MSMQ.SendPrivateMessage(new Dominio.MSMQ.Notification()
            //    {
            //        ClientMultisoftwareID = _clienteMultisoftware.Codigo,
            //        Content = cargaIntegracaoMercadoLivre.CargaPedido.Carga.Codigo,
            //        Hub = Dominio.SignalR.Hubs.IntegracaoMercadoLivre,
            //        MSMQQueue = Dominio.MSMQ.MSMQQueue.SGTWebAdmin,
            //        UsersID = new List<int>() { 0 },
            //        Service = Servicos.Embarcador.Hubs.IntegracaoMercadoLivre.GetHub(Servicos.Embarcador.Hubs.IntegracaoMercadoLivreHubs.InformarHUAtualizado)
            //    }, _prefixoMSMQ);
            //}
        }

        private void VerificarAvancarCargaEtapa2(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            if (mercadoLivreHandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility && mercadoLivreHandlingUnit.AvancarEtapaDocumentosParaEmissaoAutomaticamente)
            {
                try
                {
                    Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(cargaIntegracaoMercadoLivre.Carga.Codigo);
                    carga.AvancouCargaEtapaDocumentoLote = false;

                    List<PermissaoPersonalizada> permissoesPersonalizadas = new List<PermissaoPersonalizada>() { PermissaoPersonalizada.Carga_InformarDocumentosFiscais };
                    var resultado = servicoCarga.ConfirmarEnvioDosDocumentos(carga, false, false, tipoServicoMultisoftware, permissoesPersonalizadas, null, null, carga.Operador, unitOfWork);

                    repositorioCarga.Atualizar(carga);

                    unitOfWork.CommitChanges();

                    if (auditado != null)
                    {
                        string mensagemAuditoria = $"Avanço automatico da etapa de NF-e, rota {cargaIntegracaoMercadoLivre.HandlingUnit.Rota} e facility {cargaIntegracaoMercadoLivre.HandlingUnit.Facility} do Mercado Livre.";
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaIntegracaoMercadoLivre.Carga, mensagemAuditoria, unitOfWork);
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro($"Carga {cargaIntegracaoMercadoLivre.Carga.Codigo} ocorreu um problema ao avançar a etapa automaticamente.");
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private bool ConsultarHandlingUnit(out string mensagemErro, out string token, ref Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            mensagemErro = null;
            token = null;

            Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre repIntegracaoMercadoLivre = new Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre = repIntegracaoMercadoLivre.BuscarPrimeiroRegistro();

            if (integracaoMercadoLivre == null || string.IsNullOrWhiteSpace(integracaoMercadoLivre.URL) || string.IsNullOrWhiteSpace(integracaoMercadoLivre.SecretKey) || string.IsNullOrWhiteSpace(integracaoMercadoLivre.ID))
            {
                mensagemErro = "A configuração da integração com o Mercado Livre não foi realizada.";
                return false;
            }

            token = ObterToken(integracaoMercadoLivre);

            if (mercadoLivreHandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                return ObterRotaEFacility(out mensagemErro, mercadoLivreHandlingUnit, token, integracaoMercadoLivre, unitOfWork, cancellationToken);
            else if (mercadoLivreHandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.Dispatch)
                return ObterDispatch(out mensagemErro, ref mercadoLivreHandlingUnit, token, integracaoMercadoLivre, tipoServicoMultisoftware, unitOfWork, false);
            else
                return ObterHandlingUnit(out mensagemErro, ref mercadoLivreHandlingUnit, token, integracaoMercadoLivre, unitOfWork);
        }

        private string ObterURI(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre)
        {
            string url = integracaoMercadoLivre.URL;

            if (!url.EndsWith("/"))
                url += "/";

            return url;
        }

        private HttpClient ObterClient(string url)
        {
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMercadoLivre));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string uri = ObterURI(integracaoMercadoLivre);
            string requestUri = $"{uri}oauth/token?client_id={integracaoMercadoLivre.ID}&client_secret={integracaoMercadoLivre.SecretKey}&grant_type=client_credentials";

            IFlurlResponse result = requestUri.WithHeader("Content-Type", "application/json").PostAsync().Result;
            string jsonResponse = result.ResponseMessage.Content.ReadAsStringAsync().Result;

            if (!result.ResponseMessage.IsSuccessStatusCode)
                Servicos.Log.TratarErro("Retorno ObterToken: " + result.StatusCode.ToString(), "MercadoLivre");
            else
            {
                dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                return (string)objetoRetorno?.access_token ?? string.Empty;
            }

            return string.Empty;
        }

        private void AdicionarArquivosIntegracao(ref Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, string jsonRequest, string jsonResponse, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork, _caminhoArquivosIntegracao),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork, _caminhoArquivosIntegracao),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = mensagem
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (mercadoLivreHandlingUnit.ArquivosIntegracao == null)
                mercadoLivreHandlingUnit.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            mercadoLivreHandlingUnit.ArquivosIntegracao.Add(arquivoIntegracao);
        }

        private bool ObterHandlingUnit(out string mensagemErro, ref Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, string token, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit repMercadoLivreHandlingUnit = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit(unitOfWork);
            bool sucesso = true;
            mensagemErro = string.Empty;

            if (mercadoLivreHandlingUnit.DataFimProcessarFiscalData == null)
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    string uri = ObterURI(integracaoMercadoLivre);
                    string requestUri = $"{uri}handling_units/{mercadoLivreHandlingUnit.ID}?access_token={token}";

                    IFlurlResponse result = requestUri.WithHeader("Content-Type", "application/json").GetAsync().Result;

                    string jsonResponse = result.ResponseMessage.Content.ReadAsStringAsync().Result;

                    if (!result.ResponseMessage.IsSuccessStatusCode)
                    {
                        dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                        AdicionarArquivosIntegracao(ref mercadoLivreHandlingUnit, requestUri, jsonResponse, mercadoLivreHandlingUnit.Mensagem, unitOfWork);

                        mensagemErro = objetoRetorno?.message ?? result.StatusCode.ToString();

                        sucesso = false;
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit objetoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit>(jsonResponse);

                        mercadoLivreHandlingUnit.Channel = objetoRetorno.channel;

                        if (!DateTime.TryParseExact(objetoRetorno.date_shipped, "yyyy-MM-ddTHH:mm:ssZ", null, DateTimeStyles.None, out DateTime data))
                            data = DateTime.Now;

                        mercadoLivreHandlingUnit.Date = data;
                        mercadoLivreHandlingUnit.FacilityID = objetoRetorno.facility_id;

                        AdicionarArquivosIntegracao(ref mercadoLivreHandlingUnit, requestUri, jsonResponse, "Consulta realizada com sucesso.", unitOfWork);

                        if (mercadoLivreHandlingUnit.Codigo > 0)
                            repMercadoLivreHandlingUnit.Atualizar(mercadoLivreHandlingUnit);
                        else
                            repMercadoLivreHandlingUnit.Inserir(mercadoLivreHandlingUnit);

                        sucesso = SalvarHandlightUnitDetails(out mensagemErro, ref mercadoLivreHandlingUnit, objetoRetorno.details, token, integracaoMercadoLivre, unitOfWork);
                    }
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    mensagemErro = $"Ocorreu um erro ao obter os dados da hu - {e.Message}";
                    sucesso = false;
                }
            }

            if (sucesso)
            {
                mercadoLivreHandlingUnit.DataFimProcessarFiscalData = System.DateTime.Now;
                repMercadoLivreHandlingUnit.Atualizar(mercadoLivreHandlingUnit);
            }

            return sucesso;
        }

        private bool ObterDispatch(out string mensagemErro, ref Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, string token, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool salvarNFeCTe)
        {
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit repMercadoLivreHandlingUnit = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit(unitOfWork);

            if (mercadoLivreHandlingUnit.DataFimProcessarFiscalData == null)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string uri = ObterURI(integracaoMercadoLivre);
                string requestUri = $"{uri}dispatches/{mercadoLivreHandlingUnit.ID}/fiscal-info?access_token={token}";

                do
                {
                    try
                    {
                        IFlurlResponse result = requestUri.WithHeader("Content-Type", "application/json").GetAsync().Result;

                        string jsonResponse = result.ResponseMessage.Content.ReadAsStringAsync().Result;

                        if (!result.ResponseMessage.IsSuccessStatusCode)
                        {
                            dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                            AdicionarArquivosIntegracao(ref mercadoLivreHandlingUnit, requestUri, jsonResponse, mercadoLivreHandlingUnit.Mensagem, unitOfWork);

                            mensagemErro = (objRetorno?.message ?? result.StatusCode.ToString()) + $" - {requestUri}";

                            return false;
                        }

                        Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit objetoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit>(jsonResponse);

                        if (mercadoLivreHandlingUnit.Codigo > 0)
                            repMercadoLivreHandlingUnit.Atualizar(mercadoLivreHandlingUnit);
                        else
                            repMercadoLivreHandlingUnit.Inserir(mercadoLivreHandlingUnit);

                        if (SalvarDispatchFiscalData(out mensagemErro, ref mercadoLivreHandlingUnit, objetoRetorno.fiscal_data, token, integracaoMercadoLivre, tipoServicoMultisoftware, unitOfWork, salvarNFeCTe))
                        {
                            if (string.IsNullOrWhiteSpace(objetoRetorno._links.next))
                                requestUri = string.Empty;
                            else
                                requestUri = $"{objetoRetorno._links.@base}{objetoRetorno._links.context}/{objetoRetorno._links.next}&access_token={token}";
                        }
                        else
                            return false;

                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        mensagemErro = $"Ocorreu um erro ao obter os dados da hu - {e.Message}";
                        return false;
                    }
                }
                while (!string.IsNullOrWhiteSpace(requestUri));
            }

            mercadoLivreHandlingUnit.DataFimProcessarFiscalData = System.DateTime.Now;
            repMercadoLivreHandlingUnit.Atualizar(mercadoLivreHandlingUnit);

            mensagemErro = string.Empty;
            return true;
        }

        private bool ObterRotaEFacility(out string mensagemErro, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, string token, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            //https://api.mercadolibre.com/mlb/routes/12345678/facilities/mx001/fiscal-info?offset=0&limit=3&access_token=accessToken
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit repMercadoLivreHandlingUnit = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnit(unitOfWork);

            if (mercadoLivreHandlingUnit.DataFimProcessarFiscalData == null)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                mensagemErro = string.Empty;
                string uri = ObterURI(integracaoMercadoLivre);
                string requestUri = $"{uri}MLB/routes/{mercadoLivreHandlingUnit.Rota}/facilities/{mercadoLivreHandlingUnit.Facility}/fiscal-info?access_token={token}";
                bool achouDocumentos = false;

                do
                {
                    try
                    {
                        IFlurlResponse result = requestUri.WithHeader("Content-Type", "application/json").GetAsync().Result;

                        string jsonResponse = result.ResponseMessage.Content.ReadAsStringAsync().Result;

                        if (!result.ResponseMessage.IsSuccessStatusCode)
                        {
                            dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                            AdicionarArquivosIntegracao(ref mercadoLivreHandlingUnit, requestUri, jsonResponse, mercadoLivreHandlingUnit.Mensagem, unitOfWork);

                            mensagemErro = (objRetorno?.message ?? result.StatusCode.ToString()) + $" - {requestUri}";
                            return false;
                        }

                        Dominio.ObjetosDeValor.MercadoLivre.RoutesFacilities objetoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.MercadoLivre.RoutesFacilities>(jsonResponse);

                        if (mercadoLivreHandlingUnit.Codigo > 0)
                            repMercadoLivreHandlingUnit.Atualizar(mercadoLivreHandlingUnit);
                        else
                            repMercadoLivreHandlingUnit.Inserir(mercadoLivreHandlingUnit);

                        if (objetoRetorno?.items.Count() == 0 && !achouDocumentos)
                        {
                            mensagemErro = "Consulta efetuada com sucesso, mas não foi localizado nenhum registro para importar.";
                            return false;
                        }
                        else if (objetoRetorno?.items.Count() <= _quantidadeThreadsExecutarRotaEFacility)
                        {
                            achouDocumentos = true;

                            foreach (RoutesFacilitiesItems item in objetoRetorno?.items)
                            {
                                IFlurlResponse resultItem = $"{item.href}?access_token={token}".WithHeader("Content-Type", "application/json").GetAsync().Result;

                                string jsonResponseItem = resultItem.ResponseMessage.Content.ReadAsStringAsync().Result;

                                if (!resultItem.ResponseMessage.IsSuccessStatusCode)
                                {
                                    dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                    mensagemErro = (objRetorno?.message ?? resultItem.StatusCode.ToString()) + $" - {requestUri}";
                                    return false;
                                }

                                Dominio.ObjetosDeValor.MercadoLivre.RoutesShipments objetoRetornoItem = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.MercadoLivre.RoutesShipments>(jsonResponseItem);

                                if (!SalvarRoutesShipmentsFiscalData(out mensagemErro, mercadoLivreHandlingUnit, objetoRetornoItem.fiscal_data, token, integracaoMercadoLivre, unitOfWork, null))
                                    return false;
                            }
                        }
                        else if (objetoRetorno?.items.Count() > _quantidadeThreadsExecutarRotaEFacility)
                        {
                            achouDocumentos = true;
                            decimal decimalBlocos = Math.Ceiling(((decimal)objetoRetorno?.items.Count()) / _quantidadeThreadsExecutarRotaEFacility);
                            int blocos = (int)Math.Truncate(decimalBlocos);

                            List<Task<string>> lstTasks = new List<Task<string>>();

                            for (int i = 0; i < _quantidadeThreadsExecutarRotaEFacility; i++)
                            {
                                List<RoutesFacilitiesItems> itensTask = objetoRetorno.items.Skip(i * blocos).Take(blocos).ToList();

                                Task<string> newTask = new Task<string>(() =>
                                {
                                    string mensagemErro = null;

                                    if (!ObterRotaEFacilityItens(out mensagemErro, mercadoLivreHandlingUnit, integracaoMercadoLivre, itensTask, null, token, unitOfWork.StringConexao, cancellationToken))
                                        return mensagemErro;
                                    else
                                        return null;
                                });

                                lstTasks.Add(newTask);
                                newTask.Start();
                            }

                            Task.WaitAll(lstTasks.ToArray());

                            foreach (Task<string> task in lstTasks)
                            {
                                if (!string.IsNullOrEmpty((string)task.Result))
                                {
                                    mensagemErro = (string)task.Result;
                                    return false;
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(objetoRetorno.paging?.next))
                            requestUri = string.Empty;
                        else
                            requestUri = $"{objetoRetorno.paging.next}&access_token={token}";
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        mensagemErro = $"Ocorreu um erro ao obter os dados da rota e facility - {e.Message}";
                        return false;
                    }
                }
                while (!string.IsNullOrWhiteSpace(requestUri));
            }

            mercadoLivreHandlingUnit.DataFimProcessarFiscalData = System.DateTime.Now;
            repMercadoLivreHandlingUnit.Atualizar(mercadoLivreHandlingUnit);

            mensagemErro = string.Empty;
            return true;
        }

        private bool ObterRotaEFacilityItens(out string mensagemErro, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, List<RoutesFacilitiesItems> itens, HttpClient client, string token, string stringConexao, CancellationToken cancellationToken)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                mensagemErro = null;

                try
                {
                    foreach (RoutesFacilitiesItems item in itens)
                    {
                        // Verifica se o cancelamento foi solicitado
                        if (cancellationToken.IsCancellationRequested)
                        {
                            mensagemErro = "Cancelamento solicitado. A tarefa será interrompida.";
                            return false;
                        }

                        IFlurlResponse resultItem = $"{item.href}?access_token={token}".WithHeader("Content-Type", "application/json").GetAsync().Result;

                        string jsonResponseItem = resultItem.ResponseMessage.Content.ReadAsStringAsync().Result;

                        if (!resultItem.ResponseMessage.IsSuccessStatusCode)
                        {
                            dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponseItem);
                            mensagemErro = (objRetorno?.message ?? resultItem.StatusCode.ToString()) + $" - {item.href}";
                            return false;
                        }

                        Dominio.ObjetosDeValor.MercadoLivre.RoutesShipments objetoRetornoItem = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.MercadoLivre.RoutesShipments>(jsonResponseItem);

                        if (!SalvarRoutesShipmentsFiscalData(out mensagemErro, mercadoLivreHandlingUnit, objetoRetornoItem.fiscal_data, token, integracaoMercadoLivre, unitOfWork, client))
                            return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    mensagemErro = ex.Message;
                    return false;
                }
            }
        }

        private bool SalvarDispatchFiscalData(out string mensagemErro, ref Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, List<Dominio.ObjetosDeValor.MercadoLivre.HandlingUnitFiscalData> fiscalDatas, string token, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool salvarNFeCTe)
        {
            if (fiscalDatas == null)
            {
                mensagemErro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

            foreach (Dominio.ObjetosDeValor.MercadoLivre.HandlingUnitFiscalData fiscalData in fiscalDatas)
            {
                if (fiscalData == null)
                    continue;

                if (fiscalData.tax != null &&
                    fiscalData.tax.document != null &&
                    !string.IsNullOrWhiteSpace(fiscalData.tax.cte_key))
                {
                    Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail = null;

                    if (!SalvarHandlingUnitFiscalDataTax(out mensagemErro, mercadoLivreHandlingUnit, fiscalData.tax, token, out mercadoLivreHandlingUnitDetail, unitOfWork))
                        return false;

                    if (fiscalData.invoice != null && fiscalData.invoice.document != null && salvarNFeCTe)
                    {
                        if (!SalvarHandlingUnitFiscalDataInvoice(out mensagemErro, mercadoLivreHandlingUnit, fiscalData.invoice, token, mercadoLivreHandlingUnitDetail, tipoServicoMultisoftware, unitOfWork))
                            return false;
                    }
                }
                else if (fiscalData.invoice != null &&
                         fiscalData.invoice.document != null)
                {
                    if (!SalvarHandlingUnitFiscalDataInvoice(out mensagemErro, mercadoLivreHandlingUnit, fiscalData.invoice, token, null, tipoServicoMultisoftware, unitOfWork))
                        return false;
                }
            }

            mensagemErro = string.Empty;
            return true;
        }

        private bool SalvarHandlingUnitFiscalDataInvoice(out string mensagemErro, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, Dominio.ObjetosDeValor.MercadoLivre.HandlingUnitFiscalDataInvoice handlingUnitFiscalDataInvoice, string token, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetailRegistroPai, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);

            string requestUri = handlingUnitFiscalDataInvoice.document.href + $"&access_token={token}";

            Regex regex = new Regex("\\/invoices\\/(.*)\\/download");
            Match match = regex.Match(handlingUnitFiscalDataInvoice.document.href);
            long numeroInvoice = long.Parse(match.Groups[1].Value);

            bool encontrouDocumento = false;
            Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail = repMercadoLivreHandlingUnitDetail.BuscarPorHandlingUnitEShipmentID(mercadoLivreHandlingUnit.Codigo, numeroInvoice);
            List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> arquivos = null;
            if (mercadoLivreHandlingUnitDetail != null)
            {
                arquivos = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetail(mercadoLivreHandlingUnitDetail);
                if (arquivos.Count() > 0)
                {
                    if (ExisteDocumento(arquivos.FirstOrDefault().NomeArquivo))
                        encontrouDocumento = true;
                }
            }
            else
            {
                mercadoLivreHandlingUnitDetail = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail();
                arquivos = new List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo>();
            }

            if (!encontrouDocumento)
            {
                IFlurlResponse result = requestUri.WithHeader("Content-Type", "application/json").GetAsync().Result;

                string response = result.ResponseMessage.Content.ReadAsStringAsync().Result;

                if (!result.ResponseMessage.IsSuccessStatusCode)
                {
                    dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(response);

                    AdicionarArquivosIntegracao(ref mercadoLivreHandlingUnit, requestUri, response, mercadoLivreHandlingUnit.Mensagem, unitOfWork);

                    mensagemErro = (objRetorno?.message ?? result.StatusCode.ToString()) + $" - {requestUri}";

                    return false;
                }

                Regex regexchNFe = new Regex("<chNFe>(.*)</chNFe>");
                Match matchchNFe = regexchNFe.Match(response);
                string chaveAcesso = matchchNFe.Groups[1].Value.ToString();

                mercadoLivreHandlingUnitDetail.HandlingUnit = mercadoLivreHandlingUnit;
                mercadoLivreHandlingUnitDetail.ShipmentID = numeroInvoice;
                mercadoLivreHandlingUnitDetail.TrackingNumber = "";
                mercadoLivreHandlingUnitDetail.Status = "";
                mercadoLivreHandlingUnitDetail.TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal;
                mercadoLivreHandlingUnitDetail.Situacao = mercadoLivreHandlingUnitDetailRegistroPai != null ? MercadoLivreHandlingUnitDetailSituacao.Concluido : MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                mercadoLivreHandlingUnitDetail.RequestDocument = handlingUnitFiscalDataInvoice.document.href;
                mercadoLivreHandlingUnitDetail.ChaveAcesso = chaveAcesso;
                mercadoLivreHandlingUnitDetail.DetailRegistroPai = mercadoLivreHandlingUnitDetailRegistroPai;

                if (mercadoLivreHandlingUnitDetail.Codigo > 0)
                    repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);
                else
                    repMercadoLivreHandlingUnitDetail.Inserir(mercadoLivreHandlingUnitDetail);

                Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo = arquivos.Where(o => o.Key == numeroInvoice.ToString()).FirstOrDefault();

                if (arquivo == null)
                {
                    arquivo = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo()
                    {
                        HandlingUnitDetail = mercadoLivreHandlingUnitDetail,
                        Key = numeroInvoice.ToString(),
                        NomeArquivo = SalvarDocumento(response),
                        TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal
                    };

                    repMercadoLivreHandlingUnitDetailArquivo.Inserir(arquivo);
                }

                if (mercadoLivreHandlingUnitDetailRegistroPai != null)
                {
                    Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                    byte[] data = ObterDocumento(arquivo.NomeArquivo);
                    MemoryStream stream = new MemoryStream(data);

                    System.IO.StreamReader streamReader = new StreamReader(stream);

                    if (!serNFe.BuscarDadosNotaFiscal(out mensagemErro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, streamReader, unitOfWork, null, true, false, false, tipoServicoMultisoftware, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                    {
                        mercadoLivreHandlingUnitDetail.Mensagem = mensagemErro.Left(500);
                        repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                        return false;
                    }

                    xmlNotaFiscal.SemCarga = false;

                    if (xmlNotaFiscal.Codigo == 0)
                    {
                        xmlNotaFiscal.DataRecebimento = DateTime.Now;

                        repXmlNotaFiscal.Inserir(xmlNotaFiscal);
                    }
                    else
                        repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

                    Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivoRegistroPai = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetail(mercadoLivreHandlingUnitDetailRegistroPai).FirstOrDefault();
                    if (arquivoRegistroPai != null)
                    {
                        Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional repCTeTerceiroDocumentoAdicional = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicional(unitOfWork);
                        Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe repCTeTerceiroDocumentoAdicionalNFe = new Repositorio.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe(unitOfWork);
                        Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe cteTerceiroDocumentoAdicionalNFe = repCTeTerceiroDocumentoAdicionalNFe.BuscarPorCTeTerceiroEChaveAcessoNFe(arquivoRegistroPai.PedidoCTeParaSubcontratacao.CTeTerceiro.Codigo, mercadoLivreHandlingUnitDetailRegistroPai.ChaveAcesso);

                        if (cteTerceiroDocumentoAdicionalNFe == null)
                        {
                            Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional documentoAdicional = repCTeTerceiroDocumentoAdicional.BuscarPorCTeTerceiroEChaveAcesso(arquivoRegistroPai.PedidoCTeParaSubcontratacao.CTeTerceiro.Codigo, mercadoLivreHandlingUnitDetailRegistroPai.ChaveAcesso);

                            if (documentoAdicional != null)
                            {
                                cteTerceiroDocumentoAdicionalNFe = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe();
                                cteTerceiroDocumentoAdicionalNFe.Chave = xmlNotaFiscal.Chave;
                                cteTerceiroDocumentoAdicionalNFe.CTeTerceiro = arquivoRegistroPai.PedidoCTeParaSubcontratacao.CTeTerceiro;
                                cteTerceiroDocumentoAdicionalNFe.CTeTerceiroDocumentoAdicional = documentoAdicional;
                                cteTerceiroDocumentoAdicionalNFe.Numero = xmlNotaFiscal.Numero;
                                cteTerceiroDocumentoAdicionalNFe.XMLNotaFiscal = xmlNotaFiscal;
                                repCTeTerceiroDocumentoAdicionalNFe.Inserir(cteTerceiroDocumentoAdicionalNFe);
                            }
                        }
                    }
                }
            }

            mensagemErro = string.Empty;
            return true;
        }

        private bool SalvarHandlingUnitFiscalDataTax(out string mensagemErro, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, Dominio.ObjetosDeValor.MercadoLivre.HandlingUnitFiscalDataTax handlingUnitFiscalDataTax, string token, out Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
            mercadoLivreHandlingUnitDetail = null;

            string requestUri = handlingUnitFiscalDataTax.document.href + $"&access_token={token}";
            long numeroCTe = handlingUnitFiscalDataTax.cte_key.Substring(25, 9).ToLong();

            mercadoLivreHandlingUnitDetail = repMercadoLivreHandlingUnitDetail.BuscarPorHandlingUnitEShipmentID(mercadoLivreHandlingUnit.Codigo, numeroCTe);

            bool encontrouDocumento = false;
            List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> arquivos = null;
            if (mercadoLivreHandlingUnitDetail != null)
            {
                arquivos = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetail(mercadoLivreHandlingUnitDetail);
                if (arquivos.Count() > 0)
                {
                    if (ExisteDocumento(arquivos.FirstOrDefault().NomeArquivo))
                        encontrouDocumento = true;
                }
            }
            else
            {
                mercadoLivreHandlingUnitDetail = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail();
                arquivos = new List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo>();
            }

            if (!encontrouDocumento)
            {
                IFlurlResponse result = requestUri.WithHeader("Content-Type", "application/json").GetAsync().Result;

                string response = result.ResponseMessage.Content.ReadAsStringAsync().Result;

                if (!result.ResponseMessage.IsSuccessStatusCode)
                {
                    dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(response);

                    AdicionarArquivosIntegracao(ref mercadoLivreHandlingUnit, requestUri, response, mercadoLivreHandlingUnit.Mensagem, unitOfWork);

                    mensagemErro = objRetorno?.message ?? result.StatusCode.ToString();

                    return false;
                }

                mercadoLivreHandlingUnitDetail.HandlingUnit = mercadoLivreHandlingUnit;
                mercadoLivreHandlingUnitDetail.ShipmentID = numeroCTe;
                mercadoLivreHandlingUnitDetail.TrackingNumber = "";
                mercadoLivreHandlingUnitDetail.Status = "";
                mercadoLivreHandlingUnitDetail.TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.CTe;
                mercadoLivreHandlingUnitDetail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                mercadoLivreHandlingUnitDetail.RequestDocument = handlingUnitFiscalDataTax.document.href;
                mercadoLivreHandlingUnitDetail.ChaveAcesso = handlingUnitFiscalDataTax.cte_key;

                if (mercadoLivreHandlingUnitDetail.Codigo > 0)
                    repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);
                else
                    repMercadoLivreHandlingUnitDetail.Inserir(mercadoLivreHandlingUnitDetail);

                Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo = arquivos.Where(o => o.Key == handlingUnitFiscalDataTax.cte_key).FirstOrDefault();

                if (arquivo == null)
                {
                    arquivo = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo()
                    {
                        HandlingUnitDetail = mercadoLivreHandlingUnitDetail,
                        Key = handlingUnitFiscalDataTax.cte_key,
                        NomeArquivo = SalvarDocumento(response),
                        TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.CTe
                    };

                    repMercadoLivreHandlingUnitDetailArquivo.Inserir(arquivo);
                }
            }

            mensagemErro = string.Empty;
            return true;
        }

        private bool SalvarHandlightUnitDetails(out string mensagemErro, ref Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, List<Dominio.ObjetosDeValor.MercadoLivre.HandlingUnitDetails> details, string token, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, Repositorio.UnitOfWork unitOfWork)
        {
            if (details == null)
            {
                mensagemErro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

            foreach (Dominio.ObjetosDeValor.MercadoLivre.HandlingUnitDetails detail in details)
            {
                if (detail == null)
                    continue;

                long shipmentID = detail.shipment_id.ToLong();

                Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail = repMercadoLivreHandlingUnitDetail.BuscarPorHandlingUnitEShipmentID(mercadoLivreHandlingUnit.Codigo, shipmentID);

                if (mercadoLivreHandlingUnitDetail == null || mercadoLivreHandlingUnitDetail.ChaveAcesso == null)
                {
                    if (mercadoLivreHandlingUnitDetail == null)
                    {
                        mercadoLivreHandlingUnitDetail = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail();
                        mercadoLivreHandlingUnitDetail.HandlingUnit = mercadoLivreHandlingUnit;
                        mercadoLivreHandlingUnitDetail.ShipmentID = shipmentID;
                        mercadoLivreHandlingUnitDetail.Status = detail.status;
                        mercadoLivreHandlingUnitDetail.TrackingNumber = detail.tracking_number;
                        mercadoLivreHandlingUnitDetail.Weight = detail.weight.ToDecimal();

                        repMercadoLivreHandlingUnitDetail.Inserir(mercadoLivreHandlingUnitDetail);
                    }

                    if (mercadoLivreHandlingUnitDetail.Status != "FORWARD")
                    {
                        mensagemErro = string.Empty;
                        return true;
                    }

                    if (!ObterShipmentFiscalInfo(out mensagemErro, out dynamic shipmentFiscalInfo, token, mercadoLivreHandlingUnitDetail, integracaoMercadoLivre))
                        return false;

                    foreach (dynamic fiscalData in shipmentFiscalInfo.fiscal_data)
                    {
                        if (fiscalData.invoice == null)
                        {
                            mensagemErro = "fiscal_data está sem informações da nota (invoice)";
                            return false;
                        }

                        if (fiscalData.tax == null)
                        {
                            mensagemErro = "fiscal_data.tax está sem informações da nota (invoice)";
                            return false;
                        }

                        string taxType = ((string)fiscalData.tax.type)?.ToLower();
                        string modeloDocumento = ((string)fiscalData.invoice.key).Length == 44 ? ((string)fiscalData.invoice.key).Substring(20, 2) : "";

                        if (taxType == "icms" && !ObterCTeShipment(out mensagemErro, ref mercadoLivreHandlingUnit, ref mercadoLivreHandlingUnitDetail, (string)fiscalData.tax.document.href, token, integracaoMercadoLivre, unitOfWork))
                            return false;
                        else if (taxType == "no_tax" && !ObterNFeShipment(out mensagemErro, ref mercadoLivreHandlingUnit, ref mercadoLivreHandlingUnitDetail, fiscalData, token, integracaoMercadoLivre, unitOfWork, modeloDocumento))
                            return false;

                        // Salvar Nota Fiscal do CT-e
                        if (taxType == "no_tax")
                        {
                            if (modeloDocumento.Equals("99"))
                                mercadoLivreHandlingUnitDetail.TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.DCe;
                            else
                                mercadoLivreHandlingUnitDetail.TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal;

                            mercadoLivreHandlingUnitDetail.ChaveAcesso = fiscalData.invoice.key;
                            mercadoLivreHandlingUnitDetail.RequestDocument = fiscalData.invoice.document.href;
                            mercadoLivreHandlingUnitDetail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                            repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);
                        }
                        else if (taxType == "icms")
                        {
                            mercadoLivreHandlingUnitDetail.TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.CTe;
                            mercadoLivreHandlingUnitDetail.ChaveAcesso = fiscalData.tax.cte_key;
                            mercadoLivreHandlingUnitDetail.RequestDocument = fiscalData.tax.document.href;
                            mercadoLivreHandlingUnitDetail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Pend_Processamento;
                            repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);

                            string requestUri = (string)fiscalData.invoice.document.href;
                            long numeroInvoice = ((string)fiscalData.invoice.key).Substring(25, 9).ToLong();
                            string chaveAcesso = fiscalData.invoice.key;

                            Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetailNfeAdicional = repMercadoLivreHandlingUnitDetail.BuscarPorHandlingUnitEChaveAcesso(mercadoLivreHandlingUnit.Codigo, chaveAcesso);

                            if (mercadoLivreHandlingUnitDetailNfeAdicional == null)
                                mercadoLivreHandlingUnitDetailNfeAdicional = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail();

                            mercadoLivreHandlingUnitDetailNfeAdicional.HandlingUnit = mercadoLivreHandlingUnit;
                            mercadoLivreHandlingUnitDetailNfeAdicional.ShipmentID = numeroInvoice;
                            mercadoLivreHandlingUnitDetailNfeAdicional.ChaveAcesso = chaveAcesso;
                            mercadoLivreHandlingUnitDetailNfeAdicional.TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal;
                            mercadoLivreHandlingUnitDetailNfeAdicional.Situacao = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
                            mercadoLivreHandlingUnitDetailNfeAdicional.RequestDocument = requestUri;
                            mercadoLivreHandlingUnitDetailNfeAdicional.TrackingNumber = "";
                            mercadoLivreHandlingUnitDetailNfeAdicional.Status = "";
                            mercadoLivreHandlingUnitDetailNfeAdicional.DetailRegistroPai = mercadoLivreHandlingUnitDetail;

                            if (mercadoLivreHandlingUnitDetailNfeAdicional.Codigo > 0)
                                repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetailNfeAdicional);
                            else
                                repMercadoLivreHandlingUnitDetail.Inserir(mercadoLivreHandlingUnitDetailNfeAdicional);
                        }
                    }
                }
            }

            mensagemErro = string.Empty;
            return true;
        }

        private bool ObterShipmentFiscalInfo(out string mensagemErro, out dynamic objetoRetorno, string token, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string uri = ObterURI(integracaoMercadoLivre);
            string requestUri = $"{uri}shipments/{mercadoLivreHandlingUnitDetail.ShipmentID}/fiscal-info?access_token={token}&doctype=xml";

            HttpClient client = ObterClient(uri);

            HttpResponseMessage result = client.GetAsync(requestUri).Result;

            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            objetoRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            if (!result.IsSuccessStatusCode)
            {
                mensagemErro = (objetoRetorno?.message ?? result.StatusCode.ToString()) + $" - {requestUri}";
                return false;
            }
            else
            {
                if (objetoRetorno == null || objetoRetorno.fiscal_data == null || objetoRetorno.fiscal_data.Count == 0)
                {
                    mensagemErro = $"fiscal_data está sem informações da nota (invoice) - {requestUri}";
                    return false;
                }
                else
                {
                    mensagemErro = null;
                    return true;
                }
            }
        }

        private bool ObterCTeShipment(out string mensagemErro, ref Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, ref Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail, string url, string token, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
            
            List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> arquivos = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetail(mercadoLivreHandlingUnitDetail);
            bool encontrouDocumento = false;
            if (arquivos.Count() > 0)
            { 
                if (ExisteDocumento(arquivos.FirstOrDefault().NomeArquivo))
                    encontrouDocumento = true;
            }

            bool sucesso = true;
            mensagemErro = string.Empty;
            if (!encontrouDocumento)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string uri = ObterURI(integracaoMercadoLivre);
                string requestUri = $"{url}&access_token={token}";

                IFlurlResponse result = requestUri.WithHeader("Content-Type", "application/json").GetAsync().Result;

                string jsonResponse = result.ResponseMessage.Content.ReadAsStringAsync().Result;

                if (!result.ResponseMessage.IsSuccessStatusCode)
                {
                    dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    mensagemErro = (objetoRetorno?.message ?? result.StatusCode.ToString()) + $" - {requestUri}";
                    sucesso = false;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo()
                    {
                        HandlingUnitDetail = mercadoLivreHandlingUnitDetail,
                        NomeArquivo = SalvarDocumento(jsonResponse),
                        TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.CTe
                    };

                    repMercadoLivreHandlingUnitDetailArquivo.Inserir(arquivo);
                }
            }

            return sucesso;
        }

        private bool ObterNFeShipment(out string mensagemErro, ref Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, ref Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail, dynamic fiscalData, string token, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, Repositorio.UnitOfWork unitOfWork, string modeloDocumento = "")
        {
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repMercadoLivreHandlingUnitDetailArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> arquivos = repMercadoLivreHandlingUnitDetailArquivo.BuscarPorDetail(mercadoLivreHandlingUnitDetail);
            bool encontrouDocumento = false;
            if (arquivos.Count() > 0)
            {
                if (ExisteDocumento(arquivos.FirstOrDefault().NomeArquivo))
                    encontrouDocumento = true;
            }

            bool sucesso = true;
            mensagemErro = string.Empty;
            if (!encontrouDocumento)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string uri = ObterURI(integracaoMercadoLivre);
                string requestUri = $"{(string)fiscalData.invoice.document.href}&access_token={token}";

                IFlurlResponse result = requestUri.WithHeader("Content-Type", "application/json").GetAsync().Result;

                string jsonResponse = result.ResponseMessage.Content.ReadAsStringAsync().Result;

                if (!result.ResponseMessage.IsSuccessStatusCode)
                {
                    dynamic objetoRetorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    mensagemErro = (objetoRetorno?.message ?? result.StatusCode.ToString()) + $" - {requestUri}";
                    return false;
                }
                else
                {
                    TipoDocumentoMercadoLivreHandlingUnit tipoDocumentoArquivo = TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal;

                    if (modeloDocumento.Equals("99"))
                        tipoDocumentoArquivo = TipoDocumentoMercadoLivreHandlingUnit.DCe;

                    Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo arquivo = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo()
                    {
                        HandlingUnitDetail = mercadoLivreHandlingUnitDetail,
                        NomeArquivo = SalvarDocumento(jsonResponse),
                        Key = (string)fiscalData.invoice.key,
                        TipoDocumento = tipoDocumentoArquivo
                    };

                    repMercadoLivreHandlingUnitDetailArquivo.Inserir(arquivo);
                }
            }

            return sucesso;
        }

        private string SalvarDocumento(string documento)
        {
            string nomeArquivo = Guid.NewGuid().ToString().Replace("-", "");

            Utilidades.IO.FileStorageService.Storage.WriteAllText(Utilidades.IO.FileStorageService.Storage.Combine(_caminhoArquivos, nomeArquivo + ".xml"), documento);

            return nomeArquivo;
        }

        private byte[] ObterDocumento(string nomeArquivo)
        {
            return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(Utilidades.IO.FileStorageService.Storage.Combine(_caminhoArquivos, nomeArquivo + ".xml"));
        }

        private bool ExisteDocumento(string nomeArquivo)
        {
            return Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(_caminhoArquivos, nomeArquivo + ".xml"));
        }

        private bool SalvarRoutesShipmentsFiscalData(out string mensagemErro, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, List<Dominio.ObjetosDeValor.MercadoLivre.RoutesFacilitiesFiscalData> fiscalDatas, string token, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre integracaoMercadoLivre, Repositorio.UnitOfWork unitOfWork, HttpClient client)
        {
            if (fiscalDatas == null)
            {
                mensagemErro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

            foreach (Dominio.ObjetosDeValor.MercadoLivre.RoutesFacilitiesFiscalData fiscalData in fiscalDatas)
            {
                if (fiscalData == null)
                    continue;

                if (fiscalData.tax != null
                    && !string.IsNullOrWhiteSpace(fiscalData.tax.href)
                    && !string.IsNullOrWhiteSpace(fiscalData.tax.key)
                   )
                {
                    Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail = null;

                    if (!SalvarRoutesShipmentsFiscalDataTax(out mensagemErro, mercadoLivreHandlingUnit, fiscalData.tax, client, token, out mercadoLivreHandlingUnitDetail, unitOfWork))
                        return false;

                    if (fiscalData.invoice != null && !string.IsNullOrWhiteSpace(fiscalData.invoice.href))
                    {
                        if (!SalvarRoutesShipmentsFiscalDataInvoice(out mensagemErro, mercadoLivreHandlingUnit, fiscalData.invoice, client, token, mercadoLivreHandlingUnitDetail, unitOfWork))
                            return false;
                    }
                }
                else if (fiscalData.invoice != null && !string.IsNullOrWhiteSpace(fiscalData.invoice.href))
                {
                    if (!SalvarRoutesShipmentsFiscalDataInvoice(out mensagemErro, mercadoLivreHandlingUnit, fiscalData.invoice, client, token, null, unitOfWork))
                        return false;
                }
            }

            mensagemErro = string.Empty;
            return true;
        }

        private bool SalvarRoutesShipmentsFiscalDataInvoice(out string mensagemErro, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, Dominio.ObjetosDeValor.MercadoLivre.RoutesFacilitiesFiscalDataInvoice routesFacilitiesFiscalDataInvoice, HttpClient client, string token, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetailRegistroPai, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

            string requestUri = routesFacilitiesFiscalDataInvoice.href;
            long numeroInvoice = routesFacilitiesFiscalDataInvoice.key.Substring(25, 9).ToLong();
            string chaveAcesso = routesFacilitiesFiscalDataInvoice.key;
            string modeloDocumento = routesFacilitiesFiscalDataInvoice.key.Substring(20, 2);  

            Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail = repMercadoLivreHandlingUnitDetail.BuscarPorHandlingUnitEChaveAcesso(mercadoLivreHandlingUnit.Codigo, chaveAcesso);

            if (mercadoLivreHandlingUnitDetail == null)
                mercadoLivreHandlingUnitDetail = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail();

            mercadoLivreHandlingUnitDetail.HandlingUnit = mercadoLivreHandlingUnit;
            mercadoLivreHandlingUnitDetail.ShipmentID = numeroInvoice;
            mercadoLivreHandlingUnitDetail.ChaveAcesso = chaveAcesso;

            if(modeloDocumento.Equals("99"))
                mercadoLivreHandlingUnitDetail.TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.DCe;
            else
                mercadoLivreHandlingUnitDetail.TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal;

            mercadoLivreHandlingUnitDetail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
            mercadoLivreHandlingUnitDetail.RequestDocument = requestUri;
            mercadoLivreHandlingUnitDetail.TrackingNumber = "";
            mercadoLivreHandlingUnitDetail.Status = "";
            mercadoLivreHandlingUnitDetail.DetailRegistroPai = mercadoLivreHandlingUnitDetailRegistroPai;

            if (mercadoLivreHandlingUnitDetail.Codigo > 0)
                repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);
            else
                repMercadoLivreHandlingUnitDetail.Inserir(mercadoLivreHandlingUnitDetail);

            mensagemErro = string.Empty;
            return true;
        }

        private bool SalvarRoutesShipmentsFiscalDataTax(out string mensagemErro, Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnit mercadoLivreHandlingUnit, Dominio.ObjetosDeValor.MercadoLivre.RoutesFacilitiesFiscalDataTax routesFacilitiesFiscalDataTax, HttpClient client, string token, out Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandlingUnitDetail, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

            string requestUri = routesFacilitiesFiscalDataTax.href;
            long numeroCTe = routesFacilitiesFiscalDataTax.key.Substring(25, 9).ToLong();
            string chaveAcesso = routesFacilitiesFiscalDataTax.key;

            mercadoLivreHandlingUnitDetail = repMercadoLivreHandlingUnitDetail.BuscarPorHandlingUnitEChaveAcesso(mercadoLivreHandlingUnit.Codigo, chaveAcesso);

            if (mercadoLivreHandlingUnitDetail == null)
                mercadoLivreHandlingUnitDetail = new Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail();

            mercadoLivreHandlingUnitDetail.HandlingUnit = mercadoLivreHandlingUnit;
            mercadoLivreHandlingUnitDetail.ShipmentID = numeroCTe;
            mercadoLivreHandlingUnitDetail.ChaveAcesso = chaveAcesso;
            mercadoLivreHandlingUnitDetail.TipoDocumento = TipoDocumentoMercadoLivreHandlingUnit.CTe;
            mercadoLivreHandlingUnitDetail.Situacao = MercadoLivreHandlingUnitDetailSituacao.Pend_Download;
            mercadoLivreHandlingUnitDetail.RequestDocument = requestUri;
            mercadoLivreHandlingUnitDetail.TrackingNumber = "";
            mercadoLivreHandlingUnitDetail.Status = "";

            if (mercadoLivreHandlingUnitDetail.Codigo > 0)
                repMercadoLivreHandlingUnitDetail.Atualizar(mercadoLivreHandlingUnitDetail);
            else
                repMercadoLivreHandlingUnitDetail.Inserir(mercadoLivreHandlingUnitDetail);

            mensagemErro = string.Empty;
            return true;
        }

        private bool ValidarHandlingUnitDaCargaPedido(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre)
        {
           
            if (cargaIntegracaoMercadoLivre == null)
            {
                if (cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                    mensagemErro = "Rota e Facility não encontrado.";
                else
                    mensagemErro = "Handling Unit não encontrado.";
                return false;
            }

            if (cargaIntegracaoMercadoLivre.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
            {
                mensagemErro = $"A situação da carga ({cargaIntegracaoMercadoLivre.Carga.SituacaoCarga.ObterDescricao()}) não permite a inclusão de documentos.";
                return false;
            }

            if (cargaIntegracaoMercadoLivre.HandlingUnit.Situacao == MercadoLivreHandlingUnitSituacao.AgConsulta)
            {
                if (cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                    mensagemErro = $"Rota e Facility {cargaIntegracaoMercadoLivre.HandlingUnit.Situacao.ObterDescricao()}, situação não permitir remoção.";
                else
                    mensagemErro = $"Handling Unit {cargaIntegracaoMercadoLivre.HandlingUnit.Situacao.ObterDescricao()}, situação não permitir remoção.";
                return false;
            }

            if (cargaIntegracaoMercadoLivre.Carga.ProcessandoDocumentosFiscais)
            {
                mensagemErro = "A carga está processando os documentos fiscais, não sendo possível incluir documentos.";
                return false;
            }

            mensagemErro = string.Empty;
            return true;

        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre ObterConfiguracaoIntegracaoMercadoLivre(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre repConfiguracaoIntegracaoMercadoLivre = new Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre(unitOfWork);
            return repConfiguracaoIntegracaoMercadoLivre.BuscarPrimeiroRegistro();
        }

        private bool RemoverCTesSubContratacaoENotasFiscaisViculadosAoRotaFacility(out string mensagemErro, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            
            Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo repArquivo = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoCTe servicoDocumentoCTe = new Servicos.Embarcador.CTe.DocumentoCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetailArquivo> lstArquivo = repArquivo.BuscarPorHandlingUnit(cargaIntegracaoMercadoLivre.HandlingUnit);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> lstCTesSubContratacao = lstArquivo.Where(o => o.PedidoCTeParaSubcontratacao != null).Select(c => c.PedidoCTeParaSubcontratacao).Distinct<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>().ToList();

            if (lstCTesSubContratacao.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao arquivo in lstCTesSubContratacao)
                {
                    unitOfWork.Start();
                    repMercadoLivreHandlingUnitDetail.RemoverCTesVinculadosPorCodigo(arquivo.Codigo);
                    if (!servicoDocumentoCTe.DeletarCTesSubContratacaoViculadosAoRotaFacilityMercadoLivre(out mensagemErro, arquivo, unitOfWork))
                        return false;

                    unitOfWork.CommitChanges();

                }

                Servicos.Auditoria.Auditoria.Auditar(auditado, lstCTesSubContratacao.Select(p => p.CargaPedido).FirstOrDefault(), null, "Excluiu todos os CT-es para subcontratação.", unitOfWork);
            }

            List<int> lstCodigosPedidoXMLNotaFiscal = lstArquivo.Where(o => o.PedidoXMLNotaFiscal != null).Select(n => n.PedidoXMLNotaFiscal.Codigo).ToList();

            if (lstCodigosPedidoXMLNotaFiscal.Count > 0)
            {

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = lstArquivo.Where(o => o.PedidoXMLNotaFiscal != null).Select(n => n.PedidoXMLNotaFiscal.CargaPedido).FirstOrDefault();
                Servicos.Embarcador.Carga.DocumentoEmissao servicoDocumentoEmissao = new Servicos.Embarcador.Carga.DocumentoEmissao(unitOfWork);

                if (lstCodigosPedidoXMLNotaFiscal.Count < 100)
                {
                    unitOfWork.Start();
                    repMercadoLivreHandlingUnitDetail.RemoverNotasFiscaisVinculadas(lstCodigosPedidoXMLNotaFiscal);
                    servicoDocumentoEmissao.DeletarNotasFiscaisViculadasPorRotaFacility(cargaPedido, lstCodigosPedidoXMLNotaFiscal, auditado);
                    unitOfWork.CommitChanges();
                }
                else
                {

                    decimal decimalBlocos = Math.Ceiling(((decimal)lstCodigosPedidoXMLNotaFiscal.Count) / 100);
                    int blocos = (int)Math.Truncate(decimalBlocos);

                    for (int i = 0; i < blocos; i++)
                    {
                        unitOfWork.Start();
                        //Remove os vinculos com as Notas Fiscais
                        repMercadoLivreHandlingUnitDetail.RemoverNotasFiscaisVinculadas(lstCodigosPedidoXMLNotaFiscal.Skip(i * 100).Take(100).ToList());
                        servicoDocumentoEmissao.DeletarNotasFiscaisViculadasPorRotaFacility(cargaPedido, lstCodigosPedidoXMLNotaFiscal.Skip(i * 100).Take(100).ToList(), auditado);
                        unitOfWork.CommitChanges();
                    }
                }

            }

            mensagemErro = string.Empty;
            return true;

        }

        private void RemoverHandlingUnit(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre, Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre, Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool removerCTesVinculados)
        {
            unitOfWork.Start();

            if (auditado != null)
            {
                string mensagemAuditoria = string.Empty;
                if (cargaIntegracaoMercadoLivre.HandlingUnit.TipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                    mensagemAuditoria = $"Removeu a rota {cargaIntegracaoMercadoLivre.HandlingUnit.Rota} e facility {cargaIntegracaoMercadoLivre.HandlingUnit.Facility} do Mercado Livre.";
                else
                    mensagemAuditoria = $"Removeu o handling unit {cargaIntegracaoMercadoLivre.HandlingUnit.ID} do Mercado Livre.";
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaIntegracaoMercadoLivre.Carga, mensagemAuditoria, unitOfWork);
            }

            repCargaIntegracaoMercadoLivre.Deletar(cargaIntegracaoMercadoLivre);

            if (removerCTesVinculados)
                repMercadoLivreHandlingUnitDetail.RemoverCTesVinculados(cargaIntegracaoMercadoLivre.HandlingUnit.Codigo);
            unitOfWork.CommitChanges();
        }

        #endregion
    }
}
