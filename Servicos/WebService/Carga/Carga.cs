using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Pedido;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.Carga;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.WebService.Carga
{
    public class Carga : ServicoWebServiceBase
    {
        #region Propriedades Privadas

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly string _adminStringConexao;
        private readonly string _webServiceConsultaCTe;

        #endregion Propriedades Privadas

        #region Construtores

        public Carga(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Carga(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
        }

        public Carga(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao, string webServiceConsultaCTe = "", CancellationToken cancellationToken = default) : base(unitOfWork, tipoServicoMultisoftware, clienteAcesso, clienteMultisoftware, cancellationToken)
        {
            _auditado = auditado;
            _adminStringConexao = adminStringConexao;
            _webServiceConsultaCTe = webServiceConsultaCTe;
        }

        public Carga(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
            _adminStringConexao = adminStringConexao;
        }

        public Carga() : base() { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> EnviarCargaDocumentos(Dominio.ObjetosDeValor.WebService.Carga.CargaDocumentos cargaDocumentos)
        {
            Servicos.Log.TratarErro($"EnviarCargaDocumentos: {(cargaDocumentos != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cargaDocumentos) : string.Empty)}", "Request");

            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(_unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFinanceiraFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal repCargaIntegracaoEmbarcadorPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeSVMMultimodal repCTeSVMMultimodal = new Repositorio.Embarcador.CTe.CTeSVMMultimodal(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.ParticipanteCTe repParticipanteCTe = new Repositorio.ParticipanteCTe(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);


            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga(_unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork);
            Servicos.Embarcador.CTe.CTEsImportados svcCTesImportados = new Servicos.Embarcador.CTe.CTEsImportados(_unitOfWork);
            Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro svcFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(_unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados svcCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao svcCargaLocalPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(_unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(_unitOfWork);
            Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(_unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(cargaDocumentos.ProtocoloCarga);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
                string mensagemErro = "";
                if (carga != null)
                {
                    if (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe && carga.SituacaoCarga != SituacaoCarga.CalculoFrete)
                        return Retorno<bool>.CriarRetornoDadosInvalidos("Situação da carga atual não permite recebimento dos documentos", false);

                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);

                    cargaPedido.Carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                    cargaPedido.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                    cargaPedido.CTeEmitidoNoEmbarcador = true;
                    //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                    Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {cargaDocumentos.PesoBruto}. Carga.EnviarCargaDocumentos", "PesoCargaPedido");
                    cargaPedido.Peso = cargaDocumentos.PesoBruto;
                    cargaPedido.PesoLiquido = cargaDocumentos.PesoLiquido;

                    foreach (Dominio.ObjetosDeValor.WebService.CTe.CTe conhecimento in cargaDocumentos.Conhecimentos)
                    {
                        bool modeloDAT = !string.IsNullOrWhiteSpace(conhecimento.Modelo) ? conhecimento.Modelo != "57" : false;
                        if (string.IsNullOrWhiteSpace(conhecimento.XMLAutorizacao) && !modeloDAT)
                        {
                            _unitOfWork.Rollback();
                            return Retorno<bool>.CriarRetornoDadosInvalidos($"O CT-e {conhecimento.Numero} não possui um XML.", false);
                        }

                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(conhecimento.Chave);

                        if (modeloDAT && cte == null && string.IsNullOrWhiteSpace(conhecimento.XMLAutorizacao))
                        {
                            if (carga.Empresa == null)
                            {
                                _unitOfWork.Rollback();
                                return Retorno<bool>.CriarRetornoDadosInvalidos("Não é possível gerar um CT-e sem os dados da empresa.", false);
                            }

                            cte = svcCTe.GerarCTePorCTeIntegracao(conhecimento, carga.Empresa, _unitOfWork);
                        }

                        if (cte == null)
                        {
                            System.IO.MemoryStream memoryStream = Utilidades.String.ToStream(conhecimento.XMLAutorizacao);

                            object retornoInserir = null;
                            if (carga.Empresa == null)
                                retornoInserir = "Não é possível gerar um CT-e sem os dados da empresa.";
                            else
                                retornoInserir = svcCTe.GerarCTeAnterior(memoryStream, carga.Empresa.Codigo, string.Empty, string.Empty, _unitOfWork, null, true, false, _tipoServicoMultisoftware, true, null, cte?.NumeroControle ?? "");

                            if (retornoInserir.GetType() == typeof(string))
                            {
                                _unitOfWork.Rollback();
                                return Retorno<bool>.CriarRetornoDadosInvalidos((string)retornoInserir, false);
                            }

                            cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retornoInserir;
                        }

                        if (cte != null && cte.Destinatario == null && conhecimento != null && conhecimento.Destinatario != null && !string.IsNullOrWhiteSpace(conhecimento.Destinatario.CPFCNPJ))
                        {
                            Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(Utilidades.String.OnlyNumbers(conhecimento.Destinatario.CPFCNPJ).ToDouble());
                            if (destinatario != null)
                            {
                                cte.SetarParticipante(destinatario, Dominio.Enumeradores.TipoTomador.Destinatario, null, null);

                                if (cte.Destinatario != null && cte.Destinatario.Codigo == 0)
                                {
                                    repParticipanteCTe.Inserir(cte.Destinatario);
                                    repCTe.Atualizar(cte);
                                }
                                else
                                    repCTe.Atualizar(cte);
                            }
                        }
                        if (cte != null && cte.Remetente == null && conhecimento != null && conhecimento.Remetente != null && !string.IsNullOrWhiteSpace(conhecimento.Remetente.CPFCNPJ))
                        {
                            Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(Utilidades.String.OnlyNumbers(conhecimento.Remetente.CPFCNPJ).ToDouble());
                            if (remetente != null)
                            {
                                cte.SetarParticipante(remetente, Dominio.Enumeradores.TipoTomador.Remetente, null, null);

                                if (cte.Remetente != null && cte.Remetente.Codigo == 0)
                                {
                                    repParticipanteCTe.Inserir(cte.Remetente);
                                    repCTe.Atualizar(cte);
                                }
                                else
                                    repCTe.Atualizar(cte);
                            }
                        }

                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                        {
                            CargaPedido = cargaPedido,
                            CTe = cte
                        };

                        repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);

                        //atualizar dados do cte de multimodal
                        if (cargaPedido != null && cargaPedido.Carga != null)
                        {
                            svcCTe.SalvarInformacoesMultiModal(cte, cargaPedido, cte.ValorAReceber, _unitOfWork, conhecimento.NumeroControle);
                            repCTe.Atualizar(cte);
                        }

                        if (cte.Codigo > 0)
                        {
                            cte = repCTe.BuscarPorCodigo(cte.Codigo);
                            cte.CTeSemCarga = false;
                            repCTe.Atualizar(cte);
                        }

                        if (conhecimento.CTeTitulo != null)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura = repConfiguracaoFinanceiraFatura.BuscarPrimeiroRegistro();

                            servicoTitulo.GerarTituloPorDocumentoRecebidoIntegracao(cte, conhecimento.CTeTitulo, configuracaoFinanceiraFatura, _tipoServicoMultisoftware, _auditado);

                            carga.GerouTituloAutorizacao = true;
                            repCarga.Atualizar(carga);
                        }

                        if (conhecimento.ChavesCTeCTM != null && conhecimento.ChavesCTeCTM.Count > 0)
                        {
                            foreach (string chave in conhecimento.ChavesCTeCTM)
                            {
                                Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal vinculo = new Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal()
                                {
                                    CTeSVM = cte,
                                    CTeMultimodal = repCTe.BuscarPorChave(chave)
                                };
                                if (vinculo.CTeMultimodal != null)
                                    repCTeSVMMultimodal.Inserir(vinculo);
                            }
                        }
                        if (conhecimento.ChavesCTeSVM != null && conhecimento.ChavesCTeSVM.Count > 0)
                        {
                            foreach (string chave in conhecimento.ChavesCTeSVM)
                            {
                                Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal vinculo = new Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal()
                                {
                                    CTeSVM = repCTe.BuscarPorChave(chave),
                                    CTeMultimodal = cte,
                                    CargaMultimodal = carga
                                };
                                if (vinculo.CTeSVM != null)
                                    repCTeSVMMultimodal.Inserir(vinculo);
                            }
                        }
                    }

                    repPedido.Atualizar(cargaPedido.Pedido);
                    repCargaPedido.Atualizar(cargaPedido);
                    carga = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);

                    cargaPedido = repCargaPedido.BuscarPorCodigo(cargaPedido.Codigo);

                    mensagemErro = svcCTesImportados.CriarNotasFiscaisDaCarga(cargaPedido, _tipoServicoMultisoftware, _unitOfWork, true, null, configuracaoTMS, false);

                    if (!string.IsNullOrWhiteSpace(mensagemErro))
                    {
                        _unitOfWork.Rollback();
                        return Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro, false);
                    }

                    carga.ValorICMS = cargaPedido.ValorICMS;
                    carga.ValorFrete = cargaPedido.ValorFrete;
                    carga.ValorFreteAPagar = cargaPedido.ValorFreteAPagar;
                    carga.ValorFreteLiquido = carga.ValorFrete;
                    carga.ValorFreteEmbarcador = cargaPedido.ValorFreteAPagar;
                    carga.PossuiPendencia = false;

                    Servicos.Embarcador.Carga.Ocorrencia.RefazerComplementacaoValorFreteCarga(carga, _unitOfWork, "", _tipoServicoMultisoftware, false);
                    svcCargaLocalPrestacao.VerificarEAjustarLocaisPrestacao(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, _unitOfWork, _tipoServicoMultisoftware, configuracaoPedido, false);
                    Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, configuracaoTMS, _unitOfWork, _tipoServicoMultisoftware);
                    svcCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, configuracaoTMS, _unitOfWork, _tipoServicoMultisoftware);
                    svcFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, carga.TipoFreteEscolhido, _unitOfWork, false, _tipoServicoMultisoftware, "");

                    if (carga.CargaSVM)
                    {
                        carga.SituacaoCarga = SituacaoCarga.AgNFe;
                        carga.ProcessandoDocumentosFiscais = true;
                        carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                        carga.DataConfirmacaoDocumentosFiscais = DateTime.Now;
                        carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;

                        cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.VinculadoMultimodalProprio;
                        repCargaPedido.Atualizar(cargaPedido);
                    }
                    else
                    {
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                        carga.DataInicioEmissaoDocumentos = DateTime.Now;
                        carga.DataEnvioUltimaNFe = DateTime.Now;
                        carga.DataRecebimentoUltimaNFe = DateTime.Now;
                        carga.problemaAverbacaoCTe = false;
                        carga.CTesEmDigitacao = false;
                        carga.DataInicioGeracaoCTes = DateTime.Now;

                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                            Servicos.Log.TratarErro("Atualizou a situação para calculo frete 40 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                    }

                    repCarga.Atualizar(carga);

                    _unitOfWork.CommitChanges();

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
                else
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado nenhuma Carga", false);
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                ArmazenarLogIntegracao(cargaDocumentos, _unitOfWork);
                return Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message);
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> EnviarCancelamentoCarga(Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoCarga envioCancelamentoCarga)
        {
            Servicos.Log.TratarErro($"EnviarCancelamentoCarga: {(envioCancelamentoCarga != null ? Newtonsoft.Json.JsonConvert.SerializeObject(envioCancelamentoCarga) : string.Empty)}", "Request");

            Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(_unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(_unitOfWork);
            Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga repJustificativaCancelamentoCarga = new Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga(_unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(envioCancelamentoCarga.ProtocoloCarga);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarPrimeiroRegistro();

                if (carga != null)
                {
                    if (carga.SituacaoCarga == SituacaoCarga.Anulada || carga.SituacaoCarga == SituacaoCarga.Cancelada)
                        return Retorno<bool>.CriarRetornoDadosInvalidos("Carga já cancelada", false);

                    if (configuracaoFinanceiro.ValidarDataPrevisaoPagamentoEDataPagamentoNoCancelamentoDosCTes && servicoCarga.ValidaPagamentoCTes(carga, out List<int> numeroCtesComPagamento))
                        return Retorno<bool>.CriarRetornoDadosInvalidos($"Não é possível cancelar a carga, CT-e(s) {String.Join(", ", numeroCtesComPagamento)} possuem Data Previsão/Pagamento definida.", false);
                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamento
                    {
                        Carga = carga,
                        DataCancelamento = envioCancelamentoCarga.DataCancelamento,
                        MotivoCancelamento = envioCancelamentoCarga.MotivoCancelamento,
                        Tipo = envioCancelamentoCarga.Tipo,
                        Usuario = repUsuario.BuscarPorLogin(envioCancelamentoCarga.Usuario),
                        EnviouAverbacoesCTesParaCancelamento = envioCancelamentoCarga.EnviouAverbacoesCTesParaCancelamento,
                        SituacaoCargaNoCancelamento = carga.SituacaoCarga,
                        CancelarDocumentosEmitidosNoEmbarcador = envioCancelamentoCarga.CancelarDocumentosEmitidosNoEmbarcador,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento,
                        DuplicarCarga = false,//envioCancelamentoCarga.DuplicarCarga,
                        Justificativa = envioCancelamentoCarga.Justificativa != null ? repJustificativa.BuscarPorDescricao(envioCancelamentoCarga.Justificativa.Descricao) : null,
                        JustificativaCancelamentoCarga = envioCancelamentoCarga.JustificativaCancelamento != null ? repJustificativaCancelamentoCarga.BuscarPorDescricao(envioCancelamentoCarga.JustificativaCancelamento.Descricao) : null,
                        OperadorResponsavel = repUsuario.BuscarPorLogin(envioCancelamentoCarga.OperadorResponsavel)
                    };

                    if (cargaCancelamento.Usuario == null)
                        cargaCancelamento.Usuario = repUsuario.BuscarPrimeiro();

                    repCargaCancelamento.Inserir(cargaCancelamento);

                    Servicos.Embarcador.Integracao.IntegracaoCargaCancelamento.AdicionarIntegracoesCarga(cargaCancelamento, _unitOfWork, _tipoServicoMultisoftware);
                    cargaCancelamento.GerouIntegracao = true;
                    servicoCargaCancelamentoAprovacao.CriarAprovacao(cargaCancelamento, _tipoServicoMultisoftware);

                    repCargaCancelamento.Atualizar(cargaCancelamento);

                    string tipo = "o Cancelamento";
                    if (envioCancelamentoCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                        tipo = "a Anulação";

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCancelamento, null, $"Adicionou {tipo} da Carga via integração.", _unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCancelamento.Carga, null, $"Adicionou {tipo} da Carga via integração.", _unitOfWork);

                    if (envioCancelamentoCarga.CTes != null && envioCancelamentoCarga.CTes.Count > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoCTe cteCancelamento in envioCancelamentoCarga.CTes)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTE.BuscarPorChave(cteCancelamento.ChaveCTe);
                            //if (cte == null)
                            //{
                            //    _unitOfWork.Rollback();
                            //    return Retorno<bool>.CriarRetornoDadosInvalidos("CT-e não localizado na base " + cteCancelamento.ChaveCTe, false);
                            //}
                            if (cte != null)
                            {
                                cte.Initialize();

                                cte.Status = cteCancelamento.Status;
                                cte.ProtocoloCancelamentoInutilizacao = cteCancelamento.ProtocoloCancelamentoInutilizacao;
                                cte.MensagemRetornoSefaz = cteCancelamento.MensagemRetornoSefaz;
                                cte.ObservacaoCancelamento = cteCancelamento.ObservacaoCancelamento;
                                cte.Cancelado = cteCancelamento.Cancelado;
                                if (cteCancelamento.DataRetornoSefaz.HasValue)
                                    cte.DataRetornoSefaz = cteCancelamento.DataRetornoSefaz;
                                if (cteCancelamento.DataCancelamento.HasValue)
                                    cte.DataCancelamento = cteCancelamento.DataCancelamento;

                                repCTE.Atualizar(cte, _auditado);

                                Servicos.Auditoria.Auditoria.Auditar(_auditado, cte, null, $"Cancelado/inutilizado via integração.", _unitOfWork);
                            }
                        }
                    }

                    if (envioCancelamentoCarga.AverbacaoCTes != null && envioCancelamentoCarga.AverbacaoCTes.Count > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoAverbacaoCTe averbacaoCancelamento in envioCancelamentoCarga.AverbacaoCTes)
                        {
                            Dominio.Entidades.AverbacaoCTe averbacaoCTe = repAverbacaoCTe.BuscarPorChaveCTeEProtocolo(averbacaoCancelamento.ChaveCTe, averbacaoCancelamento.Protocolo);
                            if (averbacaoCTe != null)
                            {
                                averbacaoCTe.Initialize();
                                averbacaoCTe.CodigoRetorno = averbacaoCancelamento.CodigoRetorno;
                                averbacaoCTe.MensagemRetorno = averbacaoCancelamento.MensagemRetorno;
                                averbacaoCTe.CodigoIntegracao = averbacaoCancelamento.CodigoIntegracao;
                                averbacaoCTe.Status = averbacaoCancelamento.Status;
                                averbacaoCTe.Tipo = averbacaoCancelamento.Tipo;
                                if (averbacaoCancelamento.DataRetorno.HasValue)
                                    averbacaoCTe.DataRetorno = averbacaoCancelamento.DataRetorno;

                                repAverbacaoCTe.Atualizar(averbacaoCTe, _auditado);
                            }
                        }
                    }

                    _unitOfWork.CommitChanges();

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
                else
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado nenhuma Carga para realizar o cancelamento", false);
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                ArmazenarLogIntegracao(envioCancelamentoCarga, _unitOfWork);
                return Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message);
            }
        }

        public static void ValidarCamposIntegracaoCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, bool replicarCadastroVeiculo, bool ignorarTransportadorNaoCadastrado, bool buscarClientesCadastradosNaIntegracaoDaCarga, bool utilizarProdutosDiversosNaIntegracaoDaCarga, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out int codigoPersonalizado, out Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, out Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.WebService.Frota.Veiculo serVeiculo = new Servicos.WebService.Frota.Veiculo(unitOfWork);
            Servicos.WebService.Empresa.Empresa servicoEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            Dominio.Entidades.Empresa empresa = null;
            filial = null;
            tipoOperacao = null;
            codigoPersonalizado = 0;

            serPedidoWS.RemoverDadosEssencaisDoPedido(ref cargaIntegracao);

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.TipoOperacao?.CodigoIntegracao))
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracao.TipoOperacao.CodigoIntegracao);

                if (tipoOperacao == null)
                {
                    tipoOperacao = serPedidoWS.SalvarTipoOperacao(cargaIntegracao.TipoOperacao, ref stMensagem);
                    if (tipoOperacao == null)
                        stMensagem.Append("Não foi encontrado um tipo de operação para o código de integração " + cargaIntegracao.TipoOperacao.CodigoIntegracao + " na base da Multisoftware");
                }
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.Filial?.CodigoIntegracao))
            {
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao);

                if (filial == null)
                {
                    stMensagem.Append("Não foi encontrado uma filial para o código de integração " + cargaIntegracao.Filial.CodigoIntegracao + " na base da Multisoftware");
                }
            }

            if (cargaIntegracao.TransportadoraEmitente != null)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cargaIntegracao.TransportadoraEmitente.CNPJ));

                if (empresa == null && !string.IsNullOrWhiteSpace(cargaIntegracao.TransportadoraEmitente.CodigoIntegracao))
                    empresa = repEmpresa.BuscarPorCodigoIntegracao(cargaIntegracao.TransportadoraEmitente.CodigoIntegracao);

                if (empresa == null)
                {
                    if (!ignorarTransportadorNaoCadastrado)
                        stMensagem.Append("Não foi encontrado um transportador para o CNPJ " + cargaIntegracao.TransportadoraEmitente.CNPJ + " na base da Multisoftware");
                    else
                    {
                        string msgValidacaoEmpresaEmitente = servicoEmpresa.ValidarCamposEmpresaIntegracao(cargaIntegracao.TransportadoraEmitente);
                        if (!string.IsNullOrEmpty(msgValidacaoEmpresaEmitente))
                            stMensagem.Append("Dados da Transportadora Emitente estão inválidos: " + msgValidacaoEmpresaEmitente);
                    }
                }
                else
                {
                    if (empresa.Status == "I")
                        stMensagem.Append("A transportadora informada está inativa;");
                    else if ((!empresa.EmissaoDocumentosForaDoSistema && empresa.ModeloDocumentoFiscalCargaPropria == null && string.IsNullOrEmpty(empresa.NomeCertificado) && (tipoOperacao == null || !tipoOperacao.FretePorContadoCliente)))
                    {
                        stMensagem.Append("A transportadora informada não está habilitada para emitir CT-es;");
                    }

                    if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && filial != null && empresa.FiliaisEmbarcadorHabilitado != null && empresa.FiliaisEmbarcadorHabilitado.Count > 0)
                    {
                        int codigoFilial = filial.Codigo;
                        if (!empresa.FiliaisEmbarcadorHabilitado.Any(obj => obj.Codigo == codigoFilial))
                            stMensagem.Append("A empresa informada não está apto a transportar na filial " + filial.Descricao + ".");
                    }

                    if (empresa.EmpresaPai != null && empresa.EmpresaPai.TipoAmbiente != empresa.TipoAmbiente)
                        stMensagem.Append("A empresa informada não está apta a emitir em ambiente de " + empresa.EmpresaPai.DescricaoTipoAmbiente);
                }
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataCriacaoCarga))
            {
                DateTime data;
                if (!DateTime.TryParseExact(cargaIntegracao.DataCriacaoCarga, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                {
                    stMensagem.Append("A data final de criação da carga não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                }
                ;
            }

            decimal pesoMaximo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PesoMaximoIntegracaoCarga.Value;
            if (pesoMaximo > 0 && cargaIntegracao.PesoBruto >= pesoMaximo)
                stMensagem.Append("O peso enviado (" + cargaIntegracao.PesoBruto.ToString() + ") é inválido, verificar e reenviar integração com um peso válido; ");

            if (buscarClientesCadastradosNaIntegracaoDaCarga)
            {
                if (cargaIntegracao.Remetente != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.CPFCNPJ) && (cargaIntegracao.Remetente.Endereco == null || string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.Endereco.Logradouro)))
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cargaIntegracao.Remetente.CPFCNPJ)));
                    if (cliente == null)
                        stMensagem.Append("Cliente remetente (" + cargaIntegracao.Remetente.CPFCNPJ + ") não possui cadastro; ");
                    else
                    {
                        cargaIntegracao.Remetente.NomeFantasia = string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.NomeFantasia) ? cliente.NomeFantasia : cargaIntegracao.Remetente.NomeFantasia;
                        cargaIntegracao.Remetente.RazaoSocial = string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.RazaoSocial) ? cliente.Nome : cargaIntegracao.Remetente.RazaoSocial;
                        cargaIntegracao.Remetente.CodigoAtividade = cargaIntegracao.Remetente.CodigoAtividade == 0 ? cliente.Atividade?.Codigo ?? 0 : cargaIntegracao.Remetente.CodigoAtividade;
                        cargaIntegracao.Remetente.RGIE = string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.RGIE) ? cliente.IE_RG : cargaIntegracao.Remetente.RGIE;
                        cargaIntegracao.Remetente.TipoPessoa = cliente.Tipo == "J" ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica;
                        if (cargaIntegracao.Remetente.Endereco == null)
                            cargaIntegracao.Remetente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                        cargaIntegracao.Remetente.Endereco.Bairro = string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.Endereco.Bairro) ? cliente.Bairro : cargaIntegracao.Remetente.Endereco.Bairro;
                        cargaIntegracao.Remetente.Endereco.CEP = string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.Endereco.CEP) ? cliente.CEP : cargaIntegracao.Remetente.Endereco.CEP;
                        cargaIntegracao.Remetente.Endereco.Complemento = string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.Endereco.Complemento) ? cliente.Complemento : cargaIntegracao.Remetente.Endereco.Complemento;
                        cargaIntegracao.Remetente.Endereco.Logradouro = string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.Endereco.Logradouro) ? cliente.Endereco : cargaIntegracao.Remetente.Endereco.Logradouro;
                        cargaIntegracao.Remetente.Endereco.Numero = string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.Endereco.Numero) ? cliente.Numero : cargaIntegracao.Remetente.Endereco.Numero;
                        cargaIntegracao.Remetente.Endereco.Telefone = string.IsNullOrWhiteSpace(cargaIntegracao.Remetente.Endereco.Telefone) ? Utilidades.String.OnlyNumbers(cliente.Telefone1) : cargaIntegracao.Remetente.Endereco.Telefone;
                        if (cargaIntegracao.Remetente.Endereco.Cidade == null)
                            cargaIntegracao.Remetente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                        if (cargaIntegracao.Remetente.Endereco.Cidade.IBGE == 0)
                            cargaIntegracao.Remetente.Endereco.Cidade.IBGE = cliente.Localidade.CodigoIBGE;
                    }
                }

                if (cargaIntegracao.Destinatario != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.CPFCNPJ) && (cargaIntegracao.Destinatario.Endereco == null || string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.Endereco.Logradouro)))
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cargaIntegracao.Destinatario.CPFCNPJ)));
                    if (cliente == null)
                        stMensagem.Append("Cliente destinatario (" + cargaIntegracao.Destinatario.CPFCNPJ + ") não possui cadastro; ");
                    else
                    {
                        cargaIntegracao.Destinatario.NomeFantasia = string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.NomeFantasia) ? cliente.NomeFantasia : cargaIntegracao.Destinatario.NomeFantasia;
                        cargaIntegracao.Destinatario.RazaoSocial = string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.RazaoSocial) ? cliente.Nome : cargaIntegracao.Destinatario.RazaoSocial;
                        cargaIntegracao.Destinatario.CodigoAtividade = cargaIntegracao.Destinatario.CodigoAtividade == 0 ? cliente.Atividade?.Codigo ?? 0 : cargaIntegracao.Destinatario.CodigoAtividade;
                        cargaIntegracao.Destinatario.RGIE = string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.RGIE) ? cliente.IE_RG : cargaIntegracao.Destinatario.RGIE;
                        cargaIntegracao.Destinatario.TipoPessoa = cliente.Tipo == "J" ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica;
                        if (cargaIntegracao.Destinatario.Endereco == null)
                            cargaIntegracao.Destinatario.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                        cargaIntegracao.Destinatario.Endereco.Bairro = string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.Endereco.Bairro) ? cliente.Bairro : cargaIntegracao.Destinatario.Endereco.Bairro;
                        cargaIntegracao.Destinatario.Endereco.CEP = string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.Endereco.CEP) ? cliente.CEP : cargaIntegracao.Destinatario.Endereco.CEP;
                        cargaIntegracao.Destinatario.Endereco.Complemento = string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.Endereco.Complemento) ? cliente.Complemento : cargaIntegracao.Destinatario.Endereco.Complemento;
                        cargaIntegracao.Destinatario.Endereco.Logradouro = string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.Endereco.Logradouro) ? cliente.Endereco : cargaIntegracao.Destinatario.Endereco.Logradouro;
                        cargaIntegracao.Destinatario.Endereco.Numero = string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.Endereco.Numero) ? cliente.Numero : cargaIntegracao.Destinatario.Endereco.Numero;
                        cargaIntegracao.Destinatario.Endereco.Telefone = string.IsNullOrWhiteSpace(cargaIntegracao.Destinatario.Endereco.Telefone) ? Utilidades.String.OnlyNumbers(cliente.Telefone1) : cargaIntegracao.Destinatario.Endereco.Telefone;
                        if (cargaIntegracao.Destinatario.Endereco.Cidade == null)
                            cargaIntegracao.Destinatario.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                        if (cargaIntegracao.Destinatario.Endereco.Cidade.IBGE == 0)
                            cargaIntegracao.Destinatario.Endereco.Cidade.IBGE = cliente.Localidade.CodigoIBGE;
                    }
                }

                if (cargaIntegracao.Expedidor != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.CPFCNPJ) && (cargaIntegracao.Expedidor.Endereco == null || string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.Endereco.Logradouro)))
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cargaIntegracao.Expedidor.CPFCNPJ)));
                    if (cliente == null)
                        stMensagem.Append("Cliente expedidor (" + cargaIntegracao.Expedidor.CPFCNPJ + ") não possui cadastro; ");
                    else
                    {
                        cargaIntegracao.Expedidor.NomeFantasia = string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.NomeFantasia) ? cliente.NomeFantasia : cargaIntegracao.Expedidor.NomeFantasia;
                        cargaIntegracao.Expedidor.RazaoSocial = string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.RazaoSocial) ? cliente.Nome : cargaIntegracao.Expedidor.RazaoSocial;
                        cargaIntegracao.Expedidor.CodigoAtividade = cargaIntegracao.Expedidor.CodigoAtividade == 0 ? cliente.Atividade?.Codigo ?? 0 : cargaIntegracao.Expedidor.CodigoAtividade;
                        cargaIntegracao.Expedidor.RGIE = string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.RGIE) ? cliente.IE_RG : cargaIntegracao.Expedidor.RGIE;
                        cargaIntegracao.Expedidor.TipoPessoa = cliente.Tipo == "J" ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica;
                        if (cargaIntegracao.Expedidor.Endereco == null)
                            cargaIntegracao.Expedidor.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                        cargaIntegracao.Expedidor.Endereco.Bairro = string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.Endereco.Bairro) ? cliente.Bairro : cargaIntegracao.Expedidor.Endereco.Bairro;
                        cargaIntegracao.Expedidor.Endereco.CEP = string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.Endereco.CEP) ? cliente.CEP : cargaIntegracao.Expedidor.Endereco.CEP;
                        cargaIntegracao.Expedidor.Endereco.Complemento = string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.Endereco.Complemento) ? cliente.Complemento : cargaIntegracao.Expedidor.Endereco.Complemento;
                        cargaIntegracao.Expedidor.Endereco.Logradouro = string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.Endereco.Logradouro) ? cliente.Endereco : cargaIntegracao.Expedidor.Endereco.Logradouro;
                        cargaIntegracao.Expedidor.Endereco.Numero = string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.Endereco.Numero) ? cliente.Numero : cargaIntegracao.Expedidor.Endereco.Numero;
                        cargaIntegracao.Expedidor.Endereco.Telefone = string.IsNullOrWhiteSpace(cargaIntegracao.Expedidor.Endereco.Telefone) ? Utilidades.String.OnlyNumbers(cliente.Telefone1) : cargaIntegracao.Expedidor.Endereco.Telefone;
                        if (cargaIntegracao.Expedidor.Endereco.Cidade == null)
                            cargaIntegracao.Expedidor.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                        if (cargaIntegracao.Expedidor.Endereco.Cidade.IBGE == 0)
                            cargaIntegracao.Expedidor.Endereco.Cidade.IBGE = cliente.Localidade.CodigoIBGE;
                    }
                }

                if (cargaIntegracao.Recebedor != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.CPFCNPJ) && (cargaIntegracao.Recebedor.Endereco == null || string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.Endereco.Logradouro)))
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cargaIntegracao.Recebedor.CPFCNPJ)));
                    if (cliente == null)
                        stMensagem.Append("Cliente recebedor (" + cargaIntegracao.Recebedor.CPFCNPJ + ") não possui cadastro; ");
                    else
                    {
                        cargaIntegracao.Recebedor.NomeFantasia = string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.NomeFantasia) ? cliente.NomeFantasia : cargaIntegracao.Recebedor.NomeFantasia;
                        cargaIntegracao.Recebedor.RazaoSocial = string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.RazaoSocial) ? cliente.Nome : cargaIntegracao.Recebedor.RazaoSocial;
                        cargaIntegracao.Recebedor.CodigoAtividade = cargaIntegracao.Recebedor.CodigoAtividade == 0 ? cliente.Atividade?.Codigo ?? 0 : cargaIntegracao.Recebedor.CodigoAtividade;
                        cargaIntegracao.Recebedor.RGIE = string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.RGIE) ? cliente.IE_RG : cargaIntegracao.Recebedor.RGIE;
                        cargaIntegracao.Recebedor.TipoPessoa = cliente.Tipo == "J" ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica;
                        if (cargaIntegracao.Recebedor.Endereco == null)
                            cargaIntegracao.Recebedor.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                        cargaIntegracao.Recebedor.Endereco.Bairro = string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.Endereco.Bairro) ? cliente.Bairro : cargaIntegracao.Recebedor.Endereco.Bairro;
                        cargaIntegracao.Recebedor.Endereco.CEP = string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.Endereco.CEP) ? cliente.CEP : cargaIntegracao.Recebedor.Endereco.CEP;
                        cargaIntegracao.Recebedor.Endereco.Complemento = string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.Endereco.Complemento) ? cliente.Complemento : cargaIntegracao.Recebedor.Endereco.Complemento;
                        cargaIntegracao.Recebedor.Endereco.Logradouro = string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.Endereco.Logradouro) ? cliente.Endereco : cargaIntegracao.Recebedor.Endereco.Logradouro;
                        cargaIntegracao.Recebedor.Endereco.Numero = string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.Endereco.Numero) ? cliente.Numero : cargaIntegracao.Recebedor.Endereco.Numero;
                        cargaIntegracao.Recebedor.Endereco.Telefone = string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.Endereco.Telefone) ? Utilidades.String.OnlyNumbers(cliente.Telefone1) : cargaIntegracao.Recebedor.Endereco.Telefone;
                        if (cargaIntegracao.Recebedor.Endereco.Cidade == null)
                            cargaIntegracao.Recebedor.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                        if (cargaIntegracao.Recebedor.Endereco.Cidade.IBGE == 0)
                            cargaIntegracao.Recebedor.Endereco.Cidade.IBGE = cliente.Localidade.CodigoIBGE;
                    }
                }
            }

            if (utilizarProdutosDiversosNaIntegracaoDaCarga)
            {
                if (cargaIntegracao.Produtos == null)
                {
                    cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                    Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                    produto.CodigoGrupoProduto = "1";
                    produto.CodigoProduto = "1";
                    produto.DescricaoGrupoProduto = "Diversos";
                    produto.DescricaoProduto = "Diversos";
                    produto.Quantidade = 1;
                    produto.PesoUnitario = cargaIntegracao.PesoBruto;
                    if (cargaIntegracao.Produtos != null && produto != null)
                        cargaIntegracao.Produtos.Add(produto);
                }
            }

            if (cargaIntegracao.Veiculo != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Veiculo.Placa))
            {
                if (cargaIntegracao.Veiculo.Placa.Length == 9)
                    cargaIntegracao.Veiculo.Placa = cargaIntegracao.Veiculo.Placa.Substring(0, 7);

                if (cargaIntegracao.Veiculo.Placa.Length != 7 && configuracao.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Brasil)
                    stMensagem.Append($"A placa do veículo ({cargaIntegracao.Veiculo.Placa}) está incorreta.");
                else
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlacaVarrendoFiliais(empresa?.Codigo ?? 0, cargaIntegracao.Veiculo.Placa);

                    if (veiculo == null)
                    {
                        if (string.IsNullOrWhiteSpace(cargaIntegracao.Veiculo.Renavam) || string.IsNullOrWhiteSpace(cargaIntegracao.Veiculo.UF))
                        {
                            if (!replicarCadastroVeiculo || empresa == null)
                                stMensagem.Append("A placa informada (" + cargaIntegracao.Veiculo.Placa + ") não está cadastrada na base multisoftware ");
                            else
                            {
                                Dominio.Entidades.Veiculo veiculoOutroTransportador = repVeiculo.BuscarPorPlaca(cargaIntegracao.Veiculo.Placa);

                                if (veiculoOutroTransportador == null)
                                    stMensagem.Append("A placa informada (" + cargaIntegracao.Veiculo.Placa + ") não está cadastrada ");
                                else
                                {
                                    //Cadastrar veiculo com o transportador enviado na integração da carga
                                    Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoCadastro = serVeiculo.ConverterObjetoVeiculo(veiculoOutroTransportador, unitOfWork);
                                    string mensagemRetornoCadastro = string.Empty;
                                    veiculo = serVeiculo.SalvarVeiculo(veiculoCadastro, empresa, false, ref mensagemRetornoCadastro, unitOfWork, tipoServicoMultisoftware);

                                    if (veiculo == null)
                                        stMensagem.Append(mensagemRetornoCadastro);
                                }
                            }
                        }
                    }
                }

                if (cargaIntegracao.Veiculo.Reboques != null && cargaIntegracao.Veiculo.Reboques.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboqueIntegracao in cargaIntegracao.Veiculo.Reboques)
                    {
                        if (reboqueIntegracao.Placa.Length != 7)
                            stMensagem.Append($"A placa do reboque ({reboqueIntegracao.Placa}) está incorreta.");
                        else
                        {
                            Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorPlacaVarrendoFiliais(empresa?.Codigo ?? 0, reboqueIntegracao.Placa);

                            if (reboque == null)
                            {
                                if (string.IsNullOrWhiteSpace(reboqueIntegracao.Renavam) || string.IsNullOrWhiteSpace(reboqueIntegracao.UF))
                                {
                                    if (!replicarCadastroVeiculo || empresa == null)
                                        stMensagem.Append("A placa do reboque informada (" + reboqueIntegracao.Placa + ") não está cadastrada na base multisoftware ");
                                    else
                                    {
                                        Dominio.Entidades.Veiculo veiculoOutroTransportador = repVeiculo.BuscarPorPlaca(reboqueIntegracao.Placa);
                                        if (veiculoOutroTransportador == null)
                                            stMensagem.Append("A placa do reboque informada (" + reboqueIntegracao.Placa + ") não está cadastrada ");
                                        else
                                        {
                                            //Cadastrar veiculo com o transportador enviado na integração da carga
                                            Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoCadastro = serVeiculo.ConverterObjetoVeiculo(veiculoOutroTransportador, unitOfWork);
                                            string mensagemRetornoCadastro = string.Empty;
                                            reboque = serVeiculo.SalvarVeiculo(veiculoCadastro, empresa, false, ref mensagemRetornoCadastro, unitOfWork, tipoServicoMultisoftware);

                                            if (reboque == null)
                                                stMensagem.Append(mensagemRetornoCadastro);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (cargaIntegracao.PortoOrigem != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.Porto porto = null;
                if (!string.IsNullOrWhiteSpace(cargaIntegracao.PortoOrigem.Descricao))
                    porto = repPorto.BuscarPorDescricao(cargaIntegracao.PortoOrigem.Descricao);
                if (porto == null && !string.IsNullOrWhiteSpace(cargaIntegracao.PortoOrigem.CodigoIntegracao))
                    porto = repPorto.BuscarPorCodigoIntegracao(cargaIntegracao.PortoOrigem.CodigoIntegracao);
                if (porto == null && !string.IsNullOrWhiteSpace(cargaIntegracao.PortoOrigem.CodigoDocumento))
                    porto = repPorto.BuscarPorCodigoDocumento(cargaIntegracao.PortoOrigem.CodigoDocumento);

                if (porto == null && string.IsNullOrWhiteSpace(cargaIntegracao.PortoOrigem.CodigoDocumento) && string.IsNullOrWhiteSpace(cargaIntegracao.PortoOrigem.CodigoIntegracao))
                {
                    stMensagem.Append("Dados obrigatórios para o preenchimento do porto de origem não foram informados. ");
                }
            }

            if (cargaIntegracao.PortoDestino != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.Porto porto = null;
                if (!string.IsNullOrWhiteSpace(cargaIntegracao.PortoDestino.Descricao))
                    porto = repPorto.BuscarPorDescricao(cargaIntegracao.PortoDestino.Descricao);
                if (porto == null && !string.IsNullOrWhiteSpace(cargaIntegracao.PortoDestino.CodigoIntegracao))
                    porto = repPorto.BuscarPorCodigoIntegracao(cargaIntegracao.PortoDestino.CodigoIntegracao);
                if (porto == null && !string.IsNullOrWhiteSpace(cargaIntegracao.PortoDestino.CodigoDocumento))
                    porto = repPorto.BuscarPorCodigoDocumento(cargaIntegracao.PortoDestino.CodigoDocumento);

                if (porto == null && string.IsNullOrWhiteSpace(cargaIntegracao.PortoDestino.CodigoDocumento) && string.IsNullOrWhiteSpace(cargaIntegracao.PortoDestino.CodigoIntegracao))
                {
                    stMensagem.Append("Dados obrigatórios para o preenchimento do porto de destino não foram informados. ");
                }
            }

            if (cargaIntegracao.TerminalPortoOrigem != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = null;
                if (!string.IsNullOrWhiteSpace(cargaIntegracao.TerminalPortoOrigem.Descricao))
                    terminal = repTipoTerminalImportacao.BuscarPorDescricao(cargaIntegracao.TerminalPortoOrigem.Descricao);
                if (terminal == null && !string.IsNullOrWhiteSpace(cargaIntegracao.TerminalPortoOrigem.CodigoIntegracao))
                    terminal = repTipoTerminalImportacao.BuscarPorCodigoIntegracao(cargaIntegracao.TerminalPortoOrigem.CodigoIntegracao);
                if (terminal == null && !string.IsNullOrWhiteSpace(cargaIntegracao.TerminalPortoOrigem.CodigoDocumento))
                    terminal = repTipoTerminalImportacao.BuscarPorCodigoDocumentacao(cargaIntegracao.TerminalPortoOrigem.CodigoDocumento);

                if (terminal == null && string.IsNullOrWhiteSpace(cargaIntegracao.TerminalPortoOrigem.CodigoIntegracao) && string.IsNullOrWhiteSpace(cargaIntegracao.TerminalPortoOrigem.CodigoDocumento))
                {
                    stMensagem.Append("Dados obrigatórios para o preenchimento do terminal de origem não foram informados. ");
                }
            }

            if (cargaIntegracao.TerminalPortoDestino != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = null;
                if (!string.IsNullOrWhiteSpace(cargaIntegracao.TerminalPortoDestino.Descricao))
                    terminal = repTipoTerminalImportacao.BuscarPorDescricao(cargaIntegracao.TerminalPortoDestino.Descricao);
                if (terminal == null && !string.IsNullOrWhiteSpace(cargaIntegracao.TerminalPortoDestino.CodigoIntegracao))
                    terminal = repTipoTerminalImportacao.BuscarPorCodigoIntegracao(cargaIntegracao.TerminalPortoDestino.CodigoIntegracao);
                if (terminal == null && !string.IsNullOrWhiteSpace(cargaIntegracao.TerminalPortoDestino.CodigoDocumento))
                    terminal = repTipoTerminalImportacao.BuscarPorCodigoDocumentacao(cargaIntegracao.TerminalPortoDestino.CodigoDocumento);

                if (terminal == null && string.IsNullOrWhiteSpace(cargaIntegracao.TerminalPortoDestino.CodigoIntegracao) && string.IsNullOrWhiteSpace(cargaIntegracao.TerminalPortoDestino.CodigoDocumento))
                {
                    stMensagem.Append("Dados obrigatórios para o preenchimento do terminal de destino não foram informados. ");
                }
            }

            if (cargaIntegracao.Viagem != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = null;
                Dominio.Entidades.Embarcador.Pedidos.Navio navio = null;

                if (cargaIntegracao.Viagem.CodigoIntegracao > 0)
                    viagem = repPedidoViagemNavio.BuscarPorCodigoIntegracao(cargaIntegracao.Viagem.CodigoIntegracao.ToString("D"));
                if (viagem == null && !string.IsNullOrWhiteSpace(cargaIntegracao.Viagem.Descricao))
                    viagem = repPedidoViagemNavio.BuscarPorDescricao(cargaIntegracao.Viagem.Descricao);

                if (viagem == null && (string.IsNullOrWhiteSpace(cargaIntegracao.Viagem.Descricao) || cargaIntegracao.Viagem.Navio == null || cargaIntegracao.Viagem.NumeroViagem <= 0))
                {
                    stMensagem.Append("Dados obrigatórios para o preenchimento da viagem não foram informados. ");
                }

                if (cargaIntegracao.Viagem.Navio != null)
                {
                    if (!string.IsNullOrWhiteSpace(cargaIntegracao.Viagem.Navio.Descricao))
                        navio = repNavio.BuscarPorDescricao(cargaIntegracao.Viagem.Navio.Descricao);
                    if (navio == null && !string.IsNullOrWhiteSpace(cargaIntegracao.Viagem.Navio.CodigoIntegracao))
                        navio = repNavio.BuscarPorCodigoIntegracao(cargaIntegracao.Viagem.Navio.CodigoIntegracao);
                    if (navio == null && !string.IsNullOrWhiteSpace(cargaIntegracao.Viagem.Navio.CodigoIRIN))
                        navio = repNavio.BuscarTodosPorCodigoIRIN(cargaIntegracao.Viagem.Navio.CodigoIRIN);

                    if (navio == null && (string.IsNullOrWhiteSpace(cargaIntegracao.Viagem.Navio.Descricao) || string.IsNullOrWhiteSpace(cargaIntegracao.Viagem.Navio.CodigoIRIN) || string.IsNullOrWhiteSpace(cargaIntegracao.Viagem.Navio.CodigoEmbarcacao)))
                    {
                        stMensagem.Append("Dados obrigatórios para o preenchimento do navio informado na viagem não foram informados. ");
                    }
                }
            }

            if (cargaIntegracao.ViagemLongoCurso != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = null;
                Dominio.Entidades.Embarcador.Pedidos.Navio navio = null;

                viagem = repPedidoViagemNavio.BuscarPorCodigoIntegracao(cargaIntegracao.ViagemLongoCurso.CodigoIntegracao.ToString("D"));
                if (viagem == null && !string.IsNullOrWhiteSpace(cargaIntegracao.ViagemLongoCurso.Descricao))
                    viagem = repPedidoViagemNavio.BuscarPorDescricao(cargaIntegracao.ViagemLongoCurso.Descricao);

                if (viagem == null && (string.IsNullOrWhiteSpace(cargaIntegracao.ViagemLongoCurso.Descricao) || cargaIntegracao.ViagemLongoCurso.Navio == null || cargaIntegracao.ViagemLongoCurso.NumeroViagem <= 0))
                {
                    stMensagem.Append("Dados obrigatórios para o preenchimento da viagem de longo curso não foram informados. ");
                }

                if (cargaIntegracao.ViagemLongoCurso.Navio != null)
                {
                    if (!string.IsNullOrWhiteSpace(cargaIntegracao.ViagemLongoCurso.Navio.Descricao))
                        navio = repNavio.BuscarPorDescricao(cargaIntegracao.ViagemLongoCurso.Navio.Descricao);
                    if (navio == null && !string.IsNullOrWhiteSpace(cargaIntegracao.ViagemLongoCurso.Navio.CodigoIntegracao))
                        navio = repNavio.BuscarPorCodigoIntegracao(cargaIntegracao.ViagemLongoCurso.Navio.CodigoIntegracao);
                    if (navio == null && !string.IsNullOrWhiteSpace(cargaIntegracao.ViagemLongoCurso.Navio.CodigoIRIN))
                        navio = repNavio.BuscarTodosPorCodigoIRIN(cargaIntegracao.ViagemLongoCurso.Navio.CodigoIRIN);

                    if (navio == null && (string.IsNullOrWhiteSpace(cargaIntegracao.ViagemLongoCurso.Navio.Descricao) || string.IsNullOrWhiteSpace(cargaIntegracao.ViagemLongoCurso.Navio.CodigoIRIN) || string.IsNullOrWhiteSpace(cargaIntegracao.ViagemLongoCurso.Navio.CodigoEmbarcacao)))
                    {
                        stMensagem.Append("Dados obrigatórios para o preenchimento do navio informado na viagem de longo curso não foram informados. ");
                    }
                }
            }

            if (cargaIntegracao.Container != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.Container container = null;
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = null;

                container = repContainer.BuscarPorCodigoIntegracao(cargaIntegracao.Container.CodigoIntegracao.ToString("D"));
                if (container == null && !string.IsNullOrWhiteSpace(cargaIntegracao.Container.Numero))
                    container = repContainer.BuscarPorNumero(cargaIntegracao.Container.Numero);

                if (container == null && cargaIntegracao.Container.TipoContainer == null)
                {
                    stMensagem.Append("Dados obrigatórios para o preenchimento do container não foram informados. ");
                }

                if (cargaIntegracao.Container.TipoContainer != null)
                {
                    if (!string.IsNullOrWhiteSpace(cargaIntegracao.Container.TipoContainer.Descricao))
                        containerTipo = repContainerTipo.BuscarPorDescricao(cargaIntegracao.Container.TipoContainer.Descricao);
                    if (containerTipo == null && cargaIntegracao.Container.TipoContainer.CodigoIntegracao > 0 && !string.IsNullOrWhiteSpace(cargaIntegracao.Container.TipoContainer.CodigoIntegracao.ToString("D")))
                        containerTipo = repContainerTipo.BuscarPorCodigoIntegracao(cargaIntegracao.Container.TipoContainer.CodigoIntegracao.ToString("D"));

                    if (containerTipo == null && string.IsNullOrWhiteSpace(cargaIntegracao.Container.TipoContainer.Descricao))
                    {
                        stMensagem.Append("Dados obrigatórios para o preenchimento do tipo do container não foram informados. ");
                    }
                }
            }

            if (cargaIntegracao.Transbordo != null && cargaIntegracao.Transbordo.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo transbordo in cargaIntegracao.Transbordo)
                {
                    if (transbordo.Porto == null || transbordo.Terminal == null || transbordo.Viagem == null)
                    {
                        stMensagem.Append("Transbordo de sequencia " + transbordo.Sequencia.ToString("D") + " não foi informado os dados obrigatórios. ");
                    }
                    else
                    {
                        if (transbordo.Porto != null)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Porto porto = null;
                            if (!string.IsNullOrWhiteSpace(transbordo.Porto.Descricao))
                                porto = repPorto.BuscarPorDescricao(transbordo.Porto.Descricao);
                            if (porto == null && !string.IsNullOrWhiteSpace(transbordo.Porto.CodigoIntegracao))
                                porto = repPorto.BuscarPorCodigoIntegracao(transbordo.Porto.CodigoIntegracao);
                            if (porto == null && !string.IsNullOrWhiteSpace(transbordo.Porto.CodigoDocumento))
                                porto = repPorto.BuscarPorCodigoDocumento(transbordo.Porto.CodigoDocumento);

                            if (porto == null && string.IsNullOrWhiteSpace(transbordo.Porto.CodigoIATA))
                            {
                                stMensagem.Append("Dados obrigatórios para o preenchimento do porto do transbordo da sequencia " + transbordo.Sequencia.ToString("D") + " não foram informados. ");
                            }
                        }
                        if (transbordo.Terminal != null)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = null;
                            if (!string.IsNullOrWhiteSpace(transbordo.Terminal.Descricao))
                                terminal = repTipoTerminalImportacao.BuscarPorDescricao(transbordo.Terminal.Descricao);
                            if (terminal == null && !string.IsNullOrWhiteSpace(transbordo.Terminal.CodigoIntegracao))
                                terminal = repTipoTerminalImportacao.BuscarPorCodigoIntegracao(transbordo.Terminal.CodigoIntegracao);
                            if (terminal == null && !string.IsNullOrWhiteSpace(transbordo.Terminal.CodigoDocumento))
                                terminal = repTipoTerminalImportacao.BuscarPorCodigoDocumentacao(transbordo.Terminal.CodigoDocumento);

                            if (terminal == null && string.IsNullOrWhiteSpace(transbordo.Terminal.CodigoIntegracao) && string.IsNullOrWhiteSpace(transbordo.Terminal.CodigoDocumento))
                            {
                                stMensagem.Append("Dados obrigatórios para o preenchimento do terminal do transbordo da sequencia " + transbordo.Sequencia.ToString("D") + " não foram informados. ");
                            }
                        }

                        if (transbordo.Viagem != null)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = null;
                            Dominio.Entidades.Embarcador.Pedidos.Navio navio = null;

                            viagem = repPedidoViagemNavio.BuscarPorCodigoIntegracao(transbordo.Viagem.CodigoIntegracao.ToString("D"));
                            if (viagem == null && !string.IsNullOrWhiteSpace(transbordo.Viagem.Descricao))
                                viagem = repPedidoViagemNavio.BuscarPorDescricao(transbordo.Viagem.Descricao);

                            if (viagem == null && (string.IsNullOrWhiteSpace(transbordo.Viagem.Descricao) || transbordo.Viagem.Navio == null || transbordo.Viagem.NumeroViagem <= 0))
                            {
                                stMensagem.Append("Dados obrigatórios para o preenchimento da viagem do transbordo da sequencia " + transbordo.Sequencia.ToString("D") + " não foram informados. ");
                            }

                            if (transbordo.Viagem.Navio != null)
                            {
                                if (!string.IsNullOrWhiteSpace(transbordo.Viagem.Navio.Descricao))
                                    navio = repNavio.BuscarPorDescricao(transbordo.Viagem.Navio.Descricao);
                                if (navio == null && !string.IsNullOrWhiteSpace(transbordo.Viagem.Navio.CodigoIRIN))
                                    navio = repNavio.BuscarPorCodigoIntegracao(transbordo.Viagem.Navio.CodigoIntegracao);
                                if (navio == null && !string.IsNullOrWhiteSpace(transbordo.Viagem.Navio.CodigoIRIN))
                                    navio = repNavio.BuscarTodosPorCodigoIRIN(transbordo.Viagem.Navio.CodigoIRIN);

                                if (navio == null && (string.IsNullOrWhiteSpace(transbordo.Viagem.Navio.Descricao) || string.IsNullOrWhiteSpace(transbordo.Viagem.Navio.CodigoIRIN) || string.IsNullOrWhiteSpace(transbordo.Viagem.Navio.CodigoEmbarcacao)))
                                {
                                    stMensagem.Append("Dados obrigatórios para o preenchimento da viagem do transbordo da sequencia " + transbordo.Sequencia.ToString("D") + " não foram informados. ");
                                }
                            }
                        }

                    }
                }
            }

            if (configuracao.VerificarNFeEmOutraCargaNaIntegracao && cargaIntegracao.NotasFiscais != null && cargaIntegracao.NotasFiscais.Count > 0)
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in cargaIntegracao.NotasFiscais)
                {
                    if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorChaveAtiva(Utilidades.String.OnlyNumbers(notaFiscal.Chave));
                        if (pedidoXMLNotaFiscal != null)
                        {
                            if (configuracao.UtilizaEmissaoMultimodal)
                            {
                                if (repPedidoXMLNotaFiscal.BuscarPorChaveAtivaNoCTe(notaFiscal.Chave, 0, 0, 0))
                                    stMensagem.Append($"Já existe uma NF-e com esta chave ({notaFiscal.Chave}) vinculada a outro pedido na carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador}.");
                            }
                            else
                                stMensagem.Append($"Já existe uma NF-e com esta chave ({notaFiscal.Chave}) e número ({notaFiscal.Numero}) vinculada a outro pedido na carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador}.");
                        }
                    }
                }
            }

            if (configuracao.ValidarRemetenteDestinatarioUnicoIntegracaoCarga && cargaIntegracao.NotasFiscais != null && cargaIntegracao.NotasFiscais.Count > 0 && (cargaIntegracao.Remetente != null || cargaIntegracao.Destinatario != null))
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in cargaIntegracao.NotasFiscais)
                {
                    if (notaFiscal.Emitente != null && cargaIntegracao.Remetente != null && notaFiscal.Emitente.CPFCNPJ != cargaIntegracao.Remetente.CPFCNPJ)
                    {
                        stMensagem.Append($"A NF-e nº {notaFiscal.Numero} possui emitente ({notaFiscal.Emitente.CPFCNPJ}) diferente ao que informado no pedido ({cargaIntegracao.Remetente.CPFCNPJ}).");
                        break;
                    }

                    if (notaFiscal.Destinatario != null && cargaIntegracao.Destinatario != null && notaFiscal.Destinatario.CPFCNPJ != cargaIntegracao.Destinatario.CPFCNPJ)
                    {
                        stMensagem.Append($"A NF-e nº {notaFiscal.Numero} possui destinatário ({notaFiscal.Destinatario.CPFCNPJ}) diferente ao que informado no pedido ({cargaIntegracao.Destinatario.CPFCNPJ}).");
                        break;
                    }
                }
            }

            codigoPersonalizado = ObterCodigoRetornoPersonalizado(stMensagem.ToString(), configuracao);
        }

        private static int ObterCodigoRetornoPersonalizado(string mensagem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {

            int codigoRetornoPersonalizado = 0;

            //todo: regra fixa para o carrefour, depois podemos melhorar a configuração e busca caso mais clientes solicitem a configuração.
            if (configuracao.RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto)
            {
                if (mensagem.Contains("Não foi encontrado um transportador para o CNPJ ")
                    || mensagem.Contains("A empresa informada não está habilitada para emitir CT-es na base Multisoftware")
                    || mensagem.Contains("A empresa informada não está apto a transportar na filial")
                    || mensagem.Contains("A empresa informada não está apta a emitir em ambiente de")
                    )
                {
                    codigoRetornoPersonalizado = 350;
                }
            }

            return codigoRetornoPersonalizado;
        }

        public List<Dominio.ObjetosDeValor.WebService.Carga.Carga> BuscarCargasPorRecebedor(List<double> recebedores, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarCargasPedidoAgIntegracao(recebedores, 0, 500);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = (from obj in cargaPedidos select obj.Carga).Distinct().ToList();
            return ObterDetalhesCargasParaIntegracao(cargas, cargaPedidos, unitOfWork);
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>> BuscarCargasPorTransportador(Dominio.Entidades.Empresa empresa, bool? naoRetornarCargasComplementares)
        {
            if (empresa == null)
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>>.CriarRetornoDadosInvalidos("Dados inválidos para esta integração.");

            List<Dominio.Entidades.Empresa> empresas = new Servicos.Embarcador.Transportadores.Empresa(_unitOfWork).BuscarEmpresasPorRaizCnpj(empresa);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            List<int> listaEmpresas = empresas.Select(o => o.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarNaoIntegradasTerceiroComTransportador(listaEmpresas, naoRetornarCargasComplementares);
            if (cargas.Count < 400)
            {
                int totalRetornar = 400 - cargas.Count;
                cargas.AddRange(repCarga.BuscarNaoIntegradasComTransportador(listaEmpresas, naoRetornarCargasComplementares, totalRetornar));
            }

            return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Carga>>.CriarRetornoSucesso(ObterDetalhesCargasParaIntegracao(cargas, null, _unitOfWork));
        }

        public Dominio.ObjetosDeValor.WebService.Carga.Carga BuscarCargaPorTransportador(int protocoloCarga, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Empresa> empresas = new Servicos.Embarcador.Transportadores.Empresa(unitOfWork).BuscarEmpresasPorRaizCnpj(empresa);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarCargaPorTransportador(protocoloCarga, empresas.Select(o => o.Codigo).ToList());

            return ObterDetalhesCargaParaIntegracao(carga, null, unitOfWork);
        }

        public List<Dominio.ObjetosDeValor.WebService.CargaCancelamento.CargaCancelamento> BuscarCargasCanceladasPorTransportador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Empresa> empresas = new Servicos.Embarcador.Transportadores.Empresa(unitOfWork).BuscarEmpresasPorRaizCnpj(empresa);

            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> cancelamentos = repCargaCancelamento.BuscarNaoIntegradasComTransportador(empresas.Select(o => o.Codigo).ToList());

            return ObterDetalhesCancelamentosParaIntegracao(cancelamentos, unitOfWork);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao> BuscarDocumentacao(long protocolo, ref string mensagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Documentos.TrackingDocumentacao repTrackingDocumentacao = new Repositorio.Embarcador.Documentos.TrackingDocumentacao(unitOfWork);
            Repositorio.Embarcador.Documentos.TrackingDocumentacaoRegistro repDocumentacaoRegistro = new Repositorio.Embarcador.Documentos.TrackingDocumentacaoRegistro(unitOfWork);
            mensagem = "";

            Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao trackingDocumentacao = repTrackingDocumentacao.BuscarPorCodigo(protocolo);

            if (trackingDocumentacao == null)
            {
                mensagem = "Tracking de documentação não encontrada.";
                return null;
            }
            List<Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao> documentacoes = new List<Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao>();
            Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao documentacao = new Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao()
            {
                CargaIMO = trackingDocumentacao.TipoIMO == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO.ApenasIMO,
                CPFOperadorCarga = trackingDocumentacao.Usuario != null ? trackingDocumentacao.Usuario.CPF : "",
                DataGeracao = trackingDocumentacao.DataGeracao.Value.ToString("dd/MM/yyyy HH:mm"),
                NomeOperadorCarga = trackingDocumentacao.Usuario != null ? trackingDocumentacao.Usuario.Nome : "",
                PortoDestino = ConverterObjetoPorto(trackingDocumentacao.PortoDestino),
                PortoOrigem = ConverterObjetoPorto(trackingDocumentacao.PortoOrigem),
                Protocolo = protocolo,
                SituacaoTrackingDocumentacao = trackingDocumentacao.SituacaoTrackingDocumentacao,
                TipoIMO = trackingDocumentacao.TipoIMO,
                TipoTrackingDocumentacao = trackingDocumentacao.TipoTrackingDocumentacao,
                Viagem = ConverterObjetoViagem(trackingDocumentacao.PedidoViagemNavio),
                EmailOperadorCarga = trackingDocumentacao.Usuario != null ? trackingDocumentacao.Usuario.Email : "",
                EmailOperadorTrakingDocumentacao = trackingDocumentacao.Usuario.Email
            };
            documentacoes.Add(documentacao);

            trackingDocumentacao.IntegracaoPendente = false;
            repTrackingDocumentacao.Atualizar(trackingDocumentacao);
            Servicos.Auditoria.Auditoria.Auditar(auditado, trackingDocumentacao, null, "Realizou a integração via WS.", unitOfWork);

            //List<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistro> documentos = repDocumentacaoRegistro.BuscarPorTraking(protocolo);
            //List<Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao> documentacoes = new List<Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao>();

            //if (documentos != null && documentos.Count > 0)
            //{
            //    foreach (var doc in documentos)
            //    {
            //        Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao documentacao = new Dominio.ObjetosDeValor.Embarcador.Documentos.Documentacao()
            //        {
            //            CargaIMO = doc.CargaIMO,
            //            CPFOperadorCarga = doc.OperadorCarga != null ? doc.OperadorCarga.CPF : doc.TrackingDocumentacao.Usuario.CPF,
            //            DataGeracao = doc.DataGeracao.Value.ToString("dd/MM/yyyy HH:mm"),
            //            NomeOperadorCarga = doc.OperadorCarga != null ? doc.OperadorCarga.Nome : doc.TrackingDocumentacao.Usuario.Nome,
            //            PortoDestino = ConverterObjetoPorto(doc.PortoDestino),
            //            PortoOrigem = ConverterObjetoPorto(doc.PortoOrigem),
            //            Protocolo = protocolo,
            //            SituacaoTrackingDocumentacao = doc.TrackingDocumentacao.SituacaoTrackingDocumentacao,
            //            TipoIMO = doc.TrackingDocumentacao.TipoIMO,
            //            TipoTrackingDocumentacao = doc.TrackingDocumentacao.TipoTrackingDocumentacao,
            //            Viagem = ConverterObjetoViagem(doc.PedidoViagemNavio),
            //            EmailOperadorCarga = doc.OperadorCarga != null ? doc.OperadorCarga.Email : doc.TrackingDocumentacao.Usuario.Email,
            //            EmailOperadorTrakingDocumentacao = doc.TrackingDocumentacao.Usuario.Email
            //        };
            //        documentacoes.Add(documentacao);
            //        Servicos.Auditoria.Auditoria.Auditar(auditado, doc, null, "Realizou a integração via WS.", unitOfWork);


            //    }
            //    trackingDocumentacao.IntegracaoPendente = false;
            //    Servicos.Auditoria.Auditoria.Auditar(auditado, trackingDocumentacao, null, "Realizou a integração via WS.", unitOfWork);
            //}
            //else
            //    mensagem = "Nenhuma documentação encontrada";

            return documentacoes;
        }

        public List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> BuscarCarga(string numeroCarga, string numeroPedido, string codigoIntegracaoFilial, ref string mensagem)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            string mensagemFilial = "";
            if (!string.IsNullOrWhiteSpace(codigoIntegracaoFilial))
                mensagemFilial = " para a filial " + codigoIntegracaoFilial;

            if (!string.IsNullOrWhiteSpace(numeroCarga) && !string.IsNullOrWhiteSpace(numeroPedido))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedidoCodigoIntegracao(numeroCarga, numeroPedido, codigoIntegracaoFilial);
                if (cargaPedido == null)
                {

                    mensagem = "Não foi localizada uma carga para os codigos de carga (" + numeroCarga + ") e de pedido (" + numeroPedido + ") informados" + mensagemFilial + ", por favor verifique.";
                }
                else
                {
                    cargaPedidos.Add(cargaPedido);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(numeroCarga))
                {
                    cargaPedidos = repCargaPedido.BuscarPorCargaCodigoIntegracao(numeroCarga, codigoIntegracaoFilial);
                    if (cargaPedidos.Count == 0)
                        mensagem = "Não foi localizada uma carga para o código da carga (" + numeroCarga + ") informado" + mensagemFilial + ", por favor verifique.";
                }
                else if (!string.IsNullOrWhiteSpace(numeroPedido))
                {
                    cargaPedidos = repCargaPedido.BuscarCargaPedidoPorPedidoCodigoIntegracao(numeroPedido, codigoIntegracaoFilial);
                    if (cargaPedidos.Count == 0)
                        mensagem = "Não foi localizada uma carga para o codigo do pedido (" + numeroPedido + ") informado" + mensagemFilial + ", por favor verifique.";
                }
            }

            List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracao = BuscarCargasPedidos(cargaPedidos, configuracao, ref mensagem, unitOfWork);

            return cargasIntegracao;
        }

        public Dominio.ObjetosDeValor.WebService.Carga.Resumo.Carga BuscarResumoCarga(string numeroCarga, string numeroPedido, string codigoIntegracaoFilial, ref string mensagem)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            string mensagemFilial = "";
            if (!string.IsNullOrWhiteSpace(codigoIntegracaoFilial))
                mensagemFilial = " para a filial " + codigoIntegracaoFilial;

            if (!string.IsNullOrWhiteSpace(numeroCarga) && !string.IsNullOrWhiteSpace(numeroPedido))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedidoCodigoIntegracao(numeroCarga, numeroPedido, codigoIntegracaoFilial);
                if (cargaPedido == null)
                {

                    mensagem = mensagem = "Não foi localizada uma carga para os codigos de carga (" + numeroCarga + ") e de pedido (" + numeroPedido + ") informados" + mensagemFilial + ", por favor verifique.";
                }
                else
                {
                    cargaPedidos.Add(cargaPedido);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(numeroCarga))
                {
                    cargaPedidos = repCargaPedido.BuscarPorCargaCodigoIntegracao(numeroCarga, codigoIntegracaoFilial);
                    if (cargaPedidos.Count == 0)
                        mensagem = "Não foi localizada uma carga para o código da carga (" + numeroCarga + ") informado" + mensagemFilial + ", por favor verifique.";
                }
                else if (!string.IsNullOrWhiteSpace(numeroPedido))
                {
                    cargaPedidos = repCargaPedido.BuscarCargaPedidoPorNumeroPedidoEmbarcador(numeroPedido, codigoIntegracaoFilial);
                    if (cargaPedidos.Count == 0)
                        mensagem = "Não foi localizada uma carga para o codigo do pedido (" + numeroPedido + ") informado" + mensagemFilial + ", por favor verifique.";
                }
            }

            Dominio.ObjetosDeValor.WebService.Carga.Resumo.Carga cargaResumo = BuscarResumoCargasPedidos(cargaPedidos, ref mensagem, unitOfWork);

            return cargaResumo;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao>> BuscarSituacaoCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            try
            {
                if (protocolo == null)
                    throw new WebServiceException("É obrigatório informar o protocolo de integração");

                int codigoCarga = protocolo.protocoloIntegracaoCarga;
                int codigoPedido = protocolo.protocoloIntegracaoPedido;

                if (codigoCarga == 0 && codigoPedido == 0)
                    throw new WebServiceException("Por favor, informe os códigos de integração");

                if (codigoCarga > 0 && codigoPedido > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(codigoCarga, codigoPedido);

                    if (cargaPedido == null || cargaPedido.Count == 0)
                        throw new WebServiceException("Não foi localizada uma carga para os protocolos de carga (" + codigoCarga + ") e de pedido (" + codigoPedido + ") informados, por favor verifique.");

                    cargaPedidos.AddRange(cargaPedido);
                }
                else if (codigoCarga > 0)
                {
                    cargaPedidos = repCargaPedido.BuscarPorProtocoloCarga(codigoCarga);
                    if (cargaPedidos.Count == 0)
                        throw new WebServiceException("Não foi localizada uma carga para os protocolo da carga (" + codigoCarga + ") informado, por favor verifique.");
                }
                else if (codigoPedido > 0)
                {
                    cargaPedidos = repCargaPedido.BuscarPorProtocoloPedido(codigoPedido);
                    if (cargaPedidos.Count == 0)
                        throw new WebServiceException("Não foi localizada uma carga para os protocolo da carga (" + codigoCarga + ") informado, por favor verifique.");
                }

                List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao> cargaSituacao = BuscarSituacaoCargasPedidos(cargaPedidos, _unitOfWork);
                Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou situação da carga", _unitOfWork);

                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao>>.CriarRetornoSucesso(cargaSituacao);
            }
            catch (WebServiceException excecao)
            {
                AuditarRetornoDadosInvalidos(_unitOfWork, excecao.Message, _auditado);
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao>>.CriarRetornoExcecao("Ocorreu uma falha ao buscar a situação da carga");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido AtualizarCarga(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, ref bool mudouLocalidade, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (carga == null)
                return null;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoFronteira repositorioPedidoFronteira = new Repositorio.Embarcador.Pedidos.PedidoFronteira(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(carga.Codigo, pedido.Codigo);
            List<Dominio.Entidades.Cliente> listaFronteiras = repositorioPedidoFronteira.BuscarFronteirasPorPedido(pedido.Codigo);

            cargaPedido.Initialize();

            if (serCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware) || (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && repPedidoXMLNotaFiscal.ContarXMLPorCargaPedido(cargaPedido.Codigo) == 0) || carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga))
                    carga.CodigoCargaEmbarcador = cargaIntegracao.NumeroCarga;

                if (pedido.Filial != null)
                {
                    new Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, auditado, configuracao).TrocarFilial(carga, filialAntiga: carga.Filial, filialNova: pedido.Filial);

                    carga.Filial = pedido.Filial;
                }

                if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataCriacaoCarga))
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(cargaIntegracao.DataCriacaoCarga, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        stMensagem.Append("A data final de criação da carga não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                    }
                    ;
                    carga.DataCriacaoCarga = data;
                }

                // Adiciona fronteira na carga, se o pedido tiver uma
                Embarcador.Carga.CargaFronteira serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unitOfWork);

                if (pedido.Fronteira != null && !listaFronteiras.Contains(pedido.Fronteira))
                {
                    listaFronteiras.Add(pedido.Fronteira);
                }

                if (listaFronteiras.Count > 0)
                {
                    foreach (Dominio.Entidades.Cliente fronteira in listaFronteiras)
                    {
                        if (!serCargaFronteira.TemFronteira(carga))
                        {
                            repCargaFronteira.Inserir(new Dominio.Entidades.Embarcador.Cargas.CargaFronteira
                            {
                                Fronteira = fronteira,
                                Carga = carga,
                            });
                        }
                    }
                }

                if (configuracao.TrocarPreCargaPorCarga)
                    PreencherDatasCarga(ref carga, cargaIntegracao, ref stMensagem, configuracaoWebService);

                PreecherDadosCarga(ref carga, pedido, tipoOperacao, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, configuracao, configuracaoWebService, unitOfWork, auditado, null, string.Empty);

                repCarga.Atualizar(carga);

                if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroPreCarga))
                {
                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorNumeroPreCarga(cargaIntegracao.NumeroPreCarga, carga.Filial != null ? carga.Filial.Codigo : 0);
                    if (preCarga != null)
                    {
                        preCarga.Carga = carga;
                        repPreCarga.Atualizar(preCarga);
                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorPreCarga(preCarga.Codigo);
                        if (cargaJanelaCarregamento != null)
                        {
                            cargaJanelaCarregamento.Carga = carga;
                            repCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                        }
                    }
                    else
                        stMensagem.Append("Não foi encontrada uma pré carga com o número " + cargaIntegracao.NumeroPreCarga + " na base Multisoftware");
                }
            }
            else
            {
                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                {
                    stMensagem.Append("Não é possível atualizar a carga, pois, a mesma está na situação " + carga.DescricaoSituacaoCarga + " e já possui Notas Fiscais vinculadas a ela.");
                }
            }

            if (stMensagem.Length <= 0)
            {
                AtualizarCargaPedido(carga, ref cargaPedido, cargaIntegracao, ref stMensagem, ref mudouLocalidade, tipoServicoMultisoftware, configuracao, unitOfWork, auditado);
                return cargaPedido;
            }

            return null;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CriarCarga(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref int protocoloPedidoExistente, ref StringBuilder stMensagem, ref int protocoloCargaExistente, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool permiteAdicionarPedidoCargaFechada, bool buscarCargaPorTransportador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, bool validarTipoOperacaoMunicipal = false, bool cargaRecebidaDeintegracao = false, bool retornarCargaPedidoEncontrado = false)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (configuracaoTMS?.UtilizarSequenciaNumeracaoCargasViaIntegracao ?? false)
                cargaIntegracao.NumeroCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();



            if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga))
            {
                if (cargaIntegracao.NumeroCarga.Length <= 50)
                {
                    Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);

                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                    Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                    Repositorio.Embarcador.Logistica.CentroCarregamentoDoca repCentroCarregamentoDoca = new Repositorio.Embarcador.Logistica.CentroCarregamentoDoca(unitOfWork);
                    Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                    Repositorio.Usuario repOperador = new Repositorio.Usuario(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaFronteira repositorioCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unitOfWork);
                    Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                    Repositorio.Embarcador.Pedidos.PedidoFronteira repositorioPedidoFronteira = new Repositorio.Embarcador.Pedidos.PedidoFronteira(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioGeralCarga.BuscarPrimeiroRegistro();
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                    List<Dominio.Entidades.Cliente> listaFronteiras = repositorioPedidoFronteira.BuscarFronteirasPorPedido(pedido.Codigo);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                    bool trocarPreCargaPorCarga = configuracaoTMS?.TrocarPreCargaPorCarga ?? true;

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        carga = repCarga.BuscarPorCodigoEmbarcador(cargaIntegracao.NumeroCarga);
                    else
                    {
                        int filialCodigo = configuracaoEmbarcador.PermitirAdicionarPedidoOutraFilialCarga ? 0 : filial?.Codigo ?? 0;
                        bool filtrarSequencialCarga = !configuracaoWebService.NaoFiltrarSequencialCargaNoMetodoAdicionarCargaPedido;

                        if (!trocarPreCargaPorCarga)
                            carga = repCarga.BuscarPorCodigoCargaEmbarcador(cargaIntegracao.NumeroCarga, filialCodigo, cargaDePreCarga: null, filtrarSequencialCargaZero: false);
                        else if (cargaIntegracao.TipoOperacao != null && (!configuracaoTMS?.AgruparIntegracaoCargaComTipoOperacaoDiferente ?? false))
                        {
                            if (tipoOperacao != null && tipoOperacao.UtilizarExpedidorComoTransportador && !tipoOperacao.OperacaoDeRedespacho)// quando a operação é por expedição pode usar o mesmo codigo de carga e pedido desde que o transportador (expedidor) seja diferente.
                            {
                                string cnpjExpedidor = "";
                                if (cargaIntegracao.TransportadoraEmitente != null)
                                    cnpjExpedidor = cargaIntegracao.TransportadoraEmitente.CNPJ;
                                else if (cargaIntegracao.Expedidor != null)
                                    cnpjExpedidor = cargaIntegracao.Expedidor.CPFCNPJSemFormato;

                                carga = repCarga.buscarPorCodigoEmbarcadorExpedidor(cargaIntegracao.NumeroCarga, filialCodigo, cnpjExpedidor, cargaIntegracao.CargaDePreCarga);
                            }
                            else
                                carga = repCarga.BuscarPorCodigoCargaEmbarcador(cargaIntegracao.NumeroCarga, filialCodigo, "", cargaIntegracao.TipoOperacao.CodigoIntegracao, cargaIntegracao.CargaDePreCarga, filtrarSequencialCarga);
                        }
                        else if (!buscarCargaPorTransportador)
                            carga = repCarga.BuscarPorCodigoCargaEmbarcador(cargaIntegracao.NumeroCarga, filialCodigo, cargaIntegracao.CargaDePreCarga);
                        else
                            carga = repCarga.BuscarPorCodigoCargaEmbarcador(cargaIntegracao.NumeroCarga, filialCodigo, !string.IsNullOrWhiteSpace(cargaIntegracao.TransportadoraEmitente?.CNPJ) ? cargaIntegracao.TransportadoraEmitente.CNPJ : string.Empty, "", cargaIntegracao.CargaDePreCarga);
                    }

                    permiteAdicionarPedidoCargaFechada = carga?.TipoOperacao?.PermiteAdicionarPedidoCargaFechada ?? permiteAdicionarPedidoCargaFechada;

                    if (carga != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Veiculo?.Placa))
                    {
                        Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(cargaIntegracao.Veiculo.Placa);

                        //int numeroReboques = (carga.ModeloVeicularCarga ?? carga.VeiculosVinculados?.FirstOrDefault()?.ModeloVeicularCarga ?? veiculo.ModeloVeicularCarga)?.NumeroReboques ?? 0;
                        int numeroReboques = 0;
                        if (carga.ModeloVeicularCarga != null && cargaIntegracao.ModeloVeicular != null && (cargaIntegracao.ModeloVeicular.CodigoIntegracao?.Equals("0") ?? false))
                            carga.ModeloVeicularCarga = null;
                        else if (carga.ModeloVeicularCarga != null && carga.ModeloVeicularCarga.NumeroReboques.HasValue)
                            numeroReboques = carga.ModeloVeicularCarga.NumeroReboques.Value;
                        else if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count() > 0 && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga != null && carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga.NumeroReboques.HasValue)
                            numeroReboques = carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga.NumeroReboques.Value;
                        else if (veiculo != null && veiculo.ModeloVeicularCarga != null && veiculo.ModeloVeicularCarga.NumeroReboques.HasValue)
                            numeroReboques = veiculo.ModeloVeicularCarga.NumeroReboques.Value;

                        if (veiculo != null)
                        {
                            bool adicionarVeiculoComoReboque = (configuracaoWebService?.AdicionarVeiculoTipoReboqueComoReboqueAoAdicionarCarga ?? false) && veiculo.IsTipoVeiculoReboque() && numeroReboques > 0 && (carga.VeiculosVinculados == null || carga.VeiculosVinculados?.Count < numeroReboques);

                            if (adicionarVeiculoComoReboque)
                            {
                                if (carga.VeiculosVinculados == null)
                                    carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                                if (!carga.VeiculosVinculados.Contains(veiculo))
                                    carga.VeiculosVinculados.Add(veiculo);
                            }

                            if (cargaIntegracao.Veiculo != null)
                            {
                                if (cargaIntegracao.Veiculo.CapacidadeKG == 0)
                                    veiculo.CapacidadeKG = 0;
                                if (cargaIntegracao.Veiculo.Tara == 0)
                                    veiculo.Tara = 0;
                                if ((cargaIntegracao.Veiculo?.NumeroFrota ?? "0").Equals("0"))
                                    veiculo.NumeroFrota = "";
                            }

                            repVeiculo.Atualizar(veiculo);
                        }
                    }

                    if (carga == null)
                    {
                        //pedido = repPedido.BuscarPorCodigo(pedido.Codigo);
                        pedido.PedidoTotalmenteCarregado = true;
                        repPedido.Atualizar(pedido);
                        carga = new Dominio.Entidades.Embarcador.Cargas.Carga();
                        carga.CargaIntegradaEmbarcador = false;
                        carga.CodigoCargaEmbarcador = cargaIntegracao.NumeroCarga;
                        carga.PlacaDeAgrupamento = cargaIntegracao.CodigoAgrupamento;
                        carga.Filial = pedido.Filial;
                        carga.CargaSVMTerceiro = cargaIntegracao.PedidoDeSVMTerceiro;
                        carga.CargaDePreCarga = cargaIntegracao.CargaDePreCarga;
                        carga.NumeroDI = Utilidades.String.Left(cargaIntegracao.NumeroDI, 20);
                        //carga.ObservacaoTransportador = cargaIntegracao.ObservacaoTransportador; // Passado para o PreecherDadosCarga()
                        carga.CategoriaCargaEmbarcador = cargaIntegracao?.CategoriaCargaEmbarcador ?? string.Empty;
                        carga.SetPointVeiculo = cargaIntegracao?.SetPointVeiculo ?? string.Empty;
                        carga.CargaSVM = cargaIntegracao?.CargaSVMProprio ?? false;
                        carga.NaoComprarValePedagio = cargaIntegracao?.NaoComprarValePedagio ?? false;
                        carga.CategoriaOS = cargaIntegracao.CategoriaOS;
                        carga.NecessariaAverbacao = cargaIntegracao.NecessariaAverbacao == "Sim" ? true : false;
                        carga.DocumentoProvedor = cargaIntegracao.DocumentoProvedor;
                        carga.ValorTotalProvedor = cargaIntegracao.ValorTotalProvedor;
                        carga.LiberarPagamento = cargaIntegracao.LiberarPagamento == "Sim" ? true : false;
                        carga.TipoOS = cargaIntegracao.TipoOS;
                        carga.TipoOSConvertido = cargaIntegracao.TipoOSConvertido;
                        carga.DirecionamentoCustoExtra = cargaIntegracao.TipoDirecionamentoCustoExtra;
                        carga.TipoServicoXML = cargaIntegracao.TipoServicoXML;
                        carga.IndicLiberacaoOk = cargaIntegracao.IndicLiberacaoOk;
                        carga.StatusCustoExtra = StatusCustoExtra.EmAberto;
                        carga.CargaRecebidaDeIntegracao = cargaRecebidaDeintegracao;

                        Dominio.Entidades.Usuario operador = repOperador.BuscarPorCPF(cargaIntegracao?.OperadorCargaCPF);

                        if (operador != null)
                            carga.Operador = operador;

                        if (cargaIntegracao.Doca != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Doca.CodigoIntegracao) && carga.Filial != null)
                        {
                            int.TryParse(cargaIntegracao.Doca.CodigoIntegracao, out int codigoDocaInteiro);
                            if (codigoDocaInteiro > 0)
                            {
                                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca doca = repCentroCarregamentoDoca.BuscarPorFilialNumero(carga.Filial.Codigo, codigoDocaInteiro);
                                if (doca != null)
                                    carga.NumeroDoca = Utilidades.String.Left(doca.Descricao, 20);
                                else
                                    carga.NumeroDoca = Utilidades.String.Left(cargaIntegracao.Doca.CodigoIntegracao, 20);
                            }
                            else
                                carga.NumeroDoca = Utilidades.String.Left(cargaIntegracao.Doca.CodigoIntegracao, 20);
                        }
                        else if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizarDocaDoComplementoFilial.Value)
                        {
                            if (cargaIntegracao.Filial != null && cargaIntegracao.Filial.Endereco != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Filial.Endereco.Complemento))
                            {
                                int.TryParse(cargaIntegracao.Filial.Endereco.Complemento, out int codigoDocaInteiro);
                                if (codigoDocaInteiro > 0)
                                {
                                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca doca = repCentroCarregamentoDoca.BuscarPorFilialNumero(carga.Filial.Codigo, codigoDocaInteiro);
                                    if (doca != null)
                                        carga.NumeroDoca = Utilidades.String.Left(doca.Descricao, 20);
                                    else
                                        carga.NumeroDoca = Utilidades.String.Left(cargaIntegracao.Filial.Endereco.Complemento, 20);
                                }
                                else
                                    carga.NumeroDoca = Utilidades.String.Left(cargaIntegracao.Filial.Endereco.Complemento, 20);
                            }
                        }

                        PreecherDadosEmpresa(carga, cargaIntegracao, pedido, ref stMensagem, unitOfWork);

                        if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataCriacaoCarga))
                        {
                            DateTime data;
                            if (!DateTime.TryParseExact(cargaIntegracao.DataCriacaoCarga, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                            {
                                stMensagem.Append("A data final de criação da carga não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                            }
                            ;
                            carga.DataCriacaoCarga = data;
                        }

                        if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataPrevisaoEntrega))
                        {
                            if (!DateTime.TryParseExact(cargaIntegracao.DataPrevisaoEntrega, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisao))
                                stMensagem.Append("A data de previsão não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");

                            carga.DataPrevisaoTerminoCarga = dataPrevisao;
                        }

                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;

                        // Adiciona fronteira na carga, se o pedido tiver uma
                        if (pedido.Fronteira != null && !listaFronteiras.Contains(pedido.Fronteira))
                        {
                            listaFronteiras.Add(pedido.Fronteira);
                        }

                        if (listaFronteiras.Count > 0)
                        {
                            foreach (Dominio.Entidades.Cliente fronteira in listaFronteiras)
                            {
                                repositorioCargaFronteira.Inserir(new Dominio.Entidades.Embarcador.Cargas.CargaFronteira
                                {
                                    Fronteira = fronteira,
                                    Carga = carga,
                                });
                            }
                        }

                        PreencherDatasCarga(ref carga, cargaIntegracao, ref stMensagem, configuracaoWebService);
                        carga.CargaFechada = false;

                        if (auditado != null)
                        {
                            repCarga.Inserir(carga);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Adicionado. Protocolo " + carga.Codigo.ToString(), unitOfWork);
                        }
                        else
                            repCarga.Inserir(carga, auditado);

                        carga.Protocolo = carga.Codigo;

                        if (configuracaoTMS.SistemaIntegracaoPadraoCarga > 0)
                        {
                            Servicos.Embarcador.Integracao.IntegracaoCarga serIntegracaoCarga = new Embarcador.Integracao.IntegracaoCarga(unitOfWork);
                            serIntegracaoCarga.InformarIntegracaoCarga(carga, configuracaoTMS.SistemaIntegracaoPadraoCarga, unitOfWork);
                        }

                        PreecherDadosCarga(ref carga, pedido, tipoOperacao, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, configuracaoTMS, configuracaoWebService, unitOfWork, auditado, clienteAcesso, adminStringConexao);

                        if (carga.Empresa != null)
                        {
                            if ((!carga.Empresa.EmissaoDocumentosForaDoSistema && carga.Empresa.ModeloDocumentoFiscalCargaPropria == null && (carga.TipoOperacao == null || !carga.TipoOperacao.FretePorContadoCliente) && !(carga.Rota?.FinalizarViagemAutomaticamente ?? false)))
                            {
                                if (string.IsNullOrEmpty(carga.Empresa.NomeCertificado))
                                    stMensagem.Append("A empresa informada não está habilitada para emitir CT-es na base Multisoftware;");
                            }

                            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && carga.Filial != null && carga.Empresa.FiliaisEmbarcadorHabilitado != null && carga.Empresa.FiliaisEmbarcadorHabilitado.Count > 0)
                            {
                                if (!carga.Empresa.FiliaisEmbarcadorHabilitado.Any(obj => obj.Codigo == carga.Filial.Codigo))
                                    stMensagem.Append("A empresa informada não está apto a transportar na filial " + carga.Filial.Descricao + ".");
                            }

                            if (carga.Empresa.EmpresaPai != null && carga.Empresa.EmpresaPai.TipoAmbiente != carga.Empresa.TipoAmbiente)
                                stMensagem.Append("A empresa informada não está apta a emitir em ambiente de " + carga.Empresa.EmpresaPai.DescricaoTipoAmbiente);
                        }

                        Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = null;

                        if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroPreCarga) || (carga.TipoOperacao != null && carga.TipoOperacao.UsaJanelaCarregamentoPorEscala))
                        {
                            preCarga = repPreCarga.BuscarPorNumeroPreCarga(cargaIntegracao.NumeroPreCarga, carga.Filial != null ? carga.Filial.Codigo : 0);

                            if (preCarga == null && carga.TipoOperacao != null && carga.TipoOperacao.UsaJanelaCarregamentoPorEscala && carga.Empresa != null && carga.Veiculo != null)
                            {
                                DateTime dataEscala = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value : carga.DataCriacaoCarga;
                                preCarga = repPreCarga.BuscarPorEscala(carga.Veiculo.Codigo, carga.Empresa.Codigo, carga.Filial?.Codigo ?? 0, dataEscala.Date);
                            }
                        }
                        else
                            preCarga = pedido.PreCarga;

                        if (preCarga != null)
                        {
                            carga.PreCarga = preCarga;

                            if (carga.FaixaTemperatura == null)
                                carga.FaixaTemperatura = preCarga.FaixaTemperatura;

                            pedido.CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador;
                            repPedido.Atualizar(pedido);
                        }

                        if (protocoloPedidoExistente > 0)
                        {
                            if (carga.TipoOperacao == null || (!carga.TipoOperacao.OperacaoDeRedespacho && !carga.TipoOperacao.PermiteDividirPedidoEmCargasDiferentes))
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasPorPedido = configuracaoTMS.TrocarPreCargaPorCarga ? pedido.CargasPedido?.Where(x => x.SituacaoCarga != SituacaoCarga.Cancelada && x.SituacaoCarga != SituacaoCarga.Anulada).ToList() : repCarga.BuscarCargasPorPedido(pedido.Codigo);

                                if ((cargasPorPedido?.Count > 0) && cargasPorPedido.Where(obj => obj.TipoOperacao == null || (!obj.TipoOperacao.OperacaoDeRedespacho && !obj.TipoOperacao.PermiteDividirPedidoEmCargasDiferentes)).ToList().Count > 0)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste = cargasPorPedido.FirstOrDefault();

                                    if (cargaExiste.CodigoCargaEmbarcador == cargaIntegracao.NumeroPedidoEmbarcador && (cargaIntegracao.Filial == null || cargaExiste.Filial?.CodigoFilialEmbarcador == cargaIntegracao.Filial?.CodigoIntegracao))
                                    {
                                        protocoloCargaExistente = cargaExiste.Protocolo;
                                        stMensagem.Append("A carga e o pedido informados já foram integrados anteriormente (" + cargaExiste.CodigoCargaEmbarcador + ");");
                                    }
                                    else
                                    {
                                        if (!configuracaoTMS.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos)
                                            stMensagem.Append("O pedido " + cargaIntegracao.NumeroPedidoEmbarcador + " foi integrado a outra carga (" + cargaExiste.CodigoCargaEmbarcador + ") na base multisoftware.");
                                        else
                                            stMensagem.Append("Ped. " + cargaIntegracao.NumeroPedidoEmbarcador + " já está na DT " + cargaExiste.CodigoCargaEmbarcador + ".");
                                    }
                                }
                            }
                        }
                    }
                    else if (protocoloPedidoExistente > 0)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEProtocoloPedido(carga.Codigo, protocoloPedidoExistente);

                        if (cargaPedido != null)
                        {
                            protocoloCargaExistente = carga.Protocolo;

                            if (retornarCargaPedidoEncontrado)
                                return cargaPedido;

                            stMensagem.Append("A carga e o pedido informados já foram integrados anteriormente (" + carga.CodigoCargaEmbarcador + ");");
                        }
                        else if (carga.TipoOperacao == null || (!carga.TipoOperacao.OperacaoDeRedespacho && !carga.TipoOperacao.PermiteDividirPedidoEmCargasDiferentes))
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasPorPedido = configuracaoTMS.TrocarPreCargaPorCarga ? pedido.CargasPedido?.Where(x => x.SituacaoCarga != SituacaoCarga.Cancelada && x.SituacaoCarga != SituacaoCarga.Anulada).ToList() : repCarga.BuscarCargasPorPedido(pedido.Codigo);

                            if ((cargasPorPedido?.Count > 0) && cargasPorPedido.Where(obj => obj.TipoOperacao == null || (!obj.TipoOperacao.OperacaoDeRedespacho && !obj.TipoOperacao.PermiteDividirPedidoEmCargasDiferentes)).ToList().Count > 0)
                            {
                                if (!configuracaoTMS.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos)
                                    stMensagem.Append("O pedido " + cargaIntegracao.NumeroPedidoEmbarcador + " foi integrado a outra carga (" + pedido.CodigoCargaEmbarcador + ") na base multisoftware.");
                                else
                                    stMensagem.Append("Ped. " + cargaIntegracao.NumeroPedidoEmbarcador + " já está na DT " + pedido.CodigoCargaEmbarcador + ".");
                            }
                            else
                                stMensagem = new StringBuilder();
                        }
                        else
                            stMensagem = new StringBuilder();
                    }
                    else if (carga.CargaFechada && ((!permiteAdicionarPedidoCargaFechada && trocarPreCargaPorCarga) || !serCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware)))
                    {
                        protocoloCargaExistente = carga.Protocolo;
                        if (!string.IsNullOrEmpty(cargaIntegracao.NumeroControlePedido))
                        {
                            //verificar se numerocontrole existe em outros pedidos da mesma carga..
                            if (!repCargaPedido.ExistePorNumeroControleECarga(cargaIntegracao.NumeroControlePedido, carga.Codigo))
                                stMensagem.Append("A carga informada já foi fechada, por isso não é possível adicionar um novo pedido; o NumeroControle:" + cargaIntegracao.NumeroControlePedido + " não encontrado nos pedidos da carga");
                        }
                        else
                            stMensagem.Append("A carga informada já foi fechada, por isso não é possível adicionar um novo pedido;");
                    }

                    pedido.PedidoTotalmenteCarregado = true;
                    repPedido.Atualizar(pedido);

                    if (stMensagem.Length <= 0)
                    {
                        if (configuracaoTMS?.NaoSomarDistanciaPedidosIntegracao ?? false)
                            carga.Distancia = cargaIntegracao.Distancia;
                        else if (configuracaoGeralCarga?.ConsiderarApenasUmaVezKMParaPedidosComMesmoDestinoOrigemCarga ?? false)
                        {
                            if (!(carga.Pedidos.Any(p => p.Pedido.Destino.Codigo == pedido.Destino.Codigo && p.Pedido.Origem.Codigo == pedido.Origem.Codigo)))
                                carga.Distancia += cargaIntegracao.Distancia;
                        }
                        else
                            carga.Distancia += cargaIntegracao.Distancia;

                        carga.NumeroImpressora = cargaIntegracao.ImpressoraNumero;
                        carga.TipoRateioProdutos = cargaIntegracao.TipoRateioProdutos;
                        carga.CargaRecebidaDeIntegracao = cargaRecebidaDeintegracao;
                        carga.CargaCritica = cargaIntegracao.CargaCritica;
                        carga.Parqueada = cargaIntegracao.Parqueada;

                        carga.DataPrevisaoTerminoViagem = cargaIntegracao.PrevisaoTerminoViagem.ToNullableDateTime();
                        carga.DataPrevisaoStopTracking = cargaIntegracao.PrevisaoStopTracking.ToNullableDateTime();

                        if (!string.IsNullOrWhiteSpace(cargaIntegracao.IdentificacaoAdicional))
                        {
                            if (carga.CodigosAgrupados == null)
                                carga.CodigosAgrupados = new List<string>();

                            if (!carga.CodigosAgrupados.Contains(cargaIntegracao.IdentificacaoAdicional))
                                carga.CodigosAgrupados.Add(cargaIntegracao.IdentificacaoAdicional);
                        }

                        repCarga.Atualizar(carga);

                        Servicos.Log.TratarErro("Iniciou Carga Pedido " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = CriarCargaPedido(carga, pedido, cargaIntegracao, protocoloPedidoExistente, ref stMensagem, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, auditado, configuracaoGeralCarga, validarTipoOperacaoMunicipal);
                        if (stMensagem.Length == 0)
                            Servicos.Log.TratarErro("Finalizou Carga Pedido CP = " + (cargaPedido?.Codigo ?? 0) + "" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                        else
                            Servicos.Log.TratarErro("Erro  ao Finalizar Carga Pedido CP = " + (cargaPedido?.Codigo ?? 0) + "   " + stMensagem.ToString() + "   " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                        return cargaPedido;
                    }
                }
                else
                {
                    stMensagem.Append("O número da carga não deve possuir mais que 50 caracteres;");
                }
            }
            else
            {
                if (pedido != null && cargaIntegracao.NotasFiscais?.Count > 0)
                    Servicos.WebService.NFe.NotaFiscal.IntegrarNotaFiscal(pedido, cargaIntegracao.NotasFiscais, null, null, configuracaoTMS, tipoServicoMultisoftware, null, null, unitOfWork);
            }

            return null;
        }

        public void AtualizarCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, ref bool mudouLocalidade, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serRegraICMS = new Embarcador.Carga.ICMS(unitOfWork);

            decimal valorOrigem = cargaPedido.ValorFrete;
            decimal valorPagarOrigem = cargaPedido.ValorFreteAPagar;
            decimal valorISSOrigem = cargaPedido.ValorISS;
            decimal valorICMSOrigem = cargaPedido.ValorICMS;
            decimal valorRetencaoISSOrigem = cargaPedido.ValorRetencaoISS;
            decimal valorCBSOrigem = cargaPedido.ValorCBS;
            decimal valorIBSMunicipalOrigem = cargaPedido.ValorIBSMunicipal;
            decimal valorIBSEstadualOrigem = cargaPedido.ValorIBSEstadual;

            if (serCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware))
            {
                mudouLocalidade = PreencherCargaPedidoOrigemDestino(ref cargaPedido, cargaPedido.Pedido, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, unitOfWork);
                PreencherDistribuidores(ref cargaPedido, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, unitOfWork);
                cargaPedido.Ativo = true;

                cargaPedido.ValorFrete = 0;

                if (cargaPedido.Pedido.OrdemColetaProgramada > 0)
                {
                    cargaPedido.OrdemColeta = cargaPedido.Pedido.OrdemColetaProgramada;
                    carga.OrdemRoteirizacaoDefinida = true;
                }

                Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
                repCargaPedidoComponenteFrete.DeletarPorCargaPedidoETipoComponente(cargaPedido.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA);
                if (carga.TipoOperacao != null)
                {
                    if (carga.TipoOperacao.TipoCobrancaMultimodal != TipoCobrancaMultimodal.Nenhum)
                        cargaPedido.TipoCobrancaMultimodal = cargaIntegracao.TipoCobrancaMultimodal != 0 ? cargaIntegracao.TipoCobrancaMultimodal : carga.TipoOperacao.TipoCobrancaMultimodal;
                    if (cargaPedido != null && cargaPedido.Pedido != null && cargaPedido.Pedido.TipoDeCarga != null && (cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorto || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorto))
                        cargaPedido.ModalPropostaMultimodal = cargaPedido.Pedido.TipoDeCarga.ModalProposta;
                    else if (carga.TipoOperacao.ModalPropostaMultimodal != ModalPropostaMultimodal.Nenhum)
                        cargaPedido.ModalPropostaMultimodal = carga.TipoOperacao.ModalPropostaMultimodal;
                    if (carga.TipoOperacao.TipoServicoMultimodal != TipoServicoMultimodal.Nenhum)
                        cargaPedido.TipoServicoMultimodal = carga.TipoOperacao.TipoServicoMultimodal;
                    if (carga.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.Nenhum)
                        cargaPedido.TipoPropostaMultimodal = carga.TipoOperacao.TipoPropostaMultimodal;

                    if ((integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false))
                    {
                        if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Normal)
                            cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Normal;
                        else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.NormalESubContratada)
                            cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Normal;
                        else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Redespacho)
                            cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Redespacho;
                        else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.RedespachoIntermediario)
                            cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.RedespachoIntermediario;
                        else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SubContratada)
                            cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Subcontratacao;
                        else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SVMProprio)
                            cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.VinculadoMultimodalProprio;
                        else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SVMTerceiro)
                            cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.VinculadoMultimodalTerceiro;
                        else if (carga.TipoOperacao.TipoServicoMultimodal != TipoServicoMultimodal.Nenhum)
                            cargaPedido.TipoServicoMultimodal = carga.TipoOperacao.TipoServicoMultimodal;
                    }
                }


                if (cargaIntegracao.ValorFrete != null && cargaIntegracao.ValorFrete.FreteProprio > 0 && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    if (cargaIntegracao.ValorFrete.FreteProprio == 0 && cargaIntegracao.ValorFrete.ValorTotalAReceber > 0)//neste caso deve encontrar o valor do frete liquido
                    {
                        if (cargaPedido.PossuiCTe)
                        {
                            bool incluirBase = false;
                            decimal baseCalculo = cargaIntegracao.ValorFrete.ValorTotalAReceber;
                            decimal percentualIncluir = 100;
                            if (cargaIntegracao.ValorFrete.ICMS != null)
                            {
                                if (cargaIntegracao.ValorFrete.ICMS.CST == "60")
                                {
                                    cargaIntegracao.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.ValorTotalAReceber;
                                }
                                else
                                    cargaIntegracao.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.ValorTotalAReceber - cargaIntegracao.ValorFrete.ICMS.ValorICMS;
                            }
                            else
                            {
                                if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                                    cargaIntegracao.ValorFrete.FreteProprio = 0;
                                else
                                {
                                    Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serRegraICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, cargaPedido.Carga.Empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirBase, ref percentualIncluir, cargaPedido.BaseCalculoICMS, null, unitOfWork, tipoServicoMultisoftware, configuracao);
                                    if (regraICMS.CST == "60")
                                        cargaIntegracao.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.ValorTotalAReceber;
                                    else
                                        cargaIntegracao.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.ValorTotalAReceber - regraICMS.ValorICMS;
                                }

                            }

                        }
                        else if (cargaPedido.PossuiNFS || cargaPedido.PossuiNFSManual) // regra para Danone, não remover o ISS, rever isso pois o ISS tem uma regra diferente.
                            cargaIntegracao.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.ValorTotalAReceber;

                    }

                    cargaPedido.ValorFrete = cargaIntegracao.ValorFrete.FreteProprio;
                    cargaPedido.ValorFreteAPagar = cargaIntegracao.ValorFrete.ValorTotalAReceber;

                    decimal valorFreteLiquido = 0;

                    if (cargaIntegracao.ValorFrete.ICMS != null && cargaPedido.PossuiCTe)
                    {
                        cargaPedido.PercentualAliquota = cargaIntegracao.ValorFrete.ICMS.Aliquota;
                        cargaPedido.IncluirICMSBaseCalculo = cargaIntegracao.ValorFrete.ICMS.IncluirICMSBC;
                        cargaPedido.ImpostoInformadoPeloEmbarcador = true;
                        cargaPedido.ValorICMS = cargaIntegracao.ValorFrete.ICMS.ValorICMS;
                        cargaPedido.BaseCalculoICMS = cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS;
                        cargaPedido.ObservacaoRegraICMSCTe = cargaIntegracao.ValorFrete.ICMS.ObservacaoCTe;

                        Servicos.Log.TratarErro($"11 - CargaPedido: {cargaPedido.Codigo} -> Aliquota: {cargaPedido.PercentualAliquota}", "ProcessarAliquota");

                        bool incluirBC = cargaPedido.IncluirICMSBaseCalculo;
                        decimal percentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;

                        Dominio.Entidades.Empresa empresa = cargaPedido.Carga.Empresa;
                        Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serRegraICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirBC, ref percentualIncluirBaseCalculo, cargaPedido.BaseCalculoICMS, null, unitOfWork, tipoServicoMultisoftware, configuracao);

                        carga.ValorICMS += cargaPedido.ValorICMS - valorICMSOrigem;

                        if (cargaIntegracao.CFOP > 0)
                        {
                            cargaPedido.CFOP = repCFOP.BuscarPorNumero(cargaIntegracao.CFOP);
                            if (cargaPedido.CFOP == null)
                                stMensagem.Append("A CFOP informada (" + cargaIntegracao.CFOP + "), é inválida.");
                        }
                        else
                        {
                            cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);
                        }

                        if (!string.IsNullOrWhiteSpace(regraICMS.ObservacaoCTe) && !cargaPedido.ObservacaoRegraICMSCTe.Contains(regraICMS.ObservacaoCTe))
                            cargaPedido.ObservacaoRegraICMSCTe += regraICMS.ObservacaoCTe;



                        if (!string.IsNullOrWhiteSpace(cargaIntegracao.ValorFrete.ICMS.CST))
                        {
                            cargaPedido.CST = cargaIntegracao.ValorFrete.ICMS.CST;
                            cargaPedido.PercentualIncluirBaseCalculo = cargaIntegracao.ValorFrete.ICMS.PercentualInclusaoBC;
                            cargaPedido.PercentualReducaoBC = cargaIntegracao.ValorFrete.ICMS.PercentualReducaoBC;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                            {
                                cargaPedido.CST = regraICMS.CST;
                                cargaPedido.PercentualIncluirBaseCalculo = regraICMS.PercentualInclusaoBC;
                                cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                            }
                            else
                                stMensagem.Append("É obrigatório informar a CST");
                        }

                        if (cargaPedido.ValorFreteAPagar == 0 && cargaIntegracao.ValorFrete.ValorPrestacaoServico > 0)
                        {
                            if (regraICMS.DescontarICMSDoValorAReceber && cargaPedido.ValorICMS > 0)
                                cargaPedido.ValorFreteAPagar = cargaIntegracao.ValorFrete.ValorPrestacaoServico - cargaPedido.ValorICMS;
                            else
                                cargaPedido.ValorFreteAPagar = cargaIntegracao.ValorFrete.ValorPrestacaoServico;

                        }

                        if (regraICMS.SimplesNacional || cargaIntegracao.ValorFrete.ICMS.SimplesNacional)
                            cargaPedido.CST = "";
                    }
                    else
                    {
                        cargaPedido.PercentualIncluirBaseCalculo = 100;
                        cargaPedido.IncluirICMSBaseCalculo = true;
                        if (carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                        {
                            carga.CalculandoFrete = true;
                            repCarga.Atualizar(carga);
                        }
                    }

                    if (cargaIntegracao.ValorFrete.ISS != null && cargaPedido.PossuiNFS)
                    {
                        cargaPedido.PercentualAliquotaISS = cargaIntegracao.ValorFrete.ISS.Aliquota;
                        cargaPedido.IncluirISSBaseCalculo = cargaIntegracao.ValorFrete.ISS.IncluirISSBaseCalculo;
                        cargaPedido.PercentualRetencaoISS = cargaIntegracao.ValorFrete.ISS.PercentualRetencao;
                        cargaPedido.ValorISS = cargaIntegracao.ValorFrete.ISS.ValorISS;
                        cargaPedido.BaseCalculoISS = cargaIntegracao.ValorFrete.ISS.ValorBaseCalculoISS;
                        cargaPedido.ImpostoInformadoPeloEmbarcador = true;
                        carga.ValorISS += cargaPedido.ValorISS - valorISSOrigem;
                        carga.ValorRetencaoISS += cargaPedido.ValorRetencaoISS - valorRetencaoISSOrigem;
                    }

                    if (cargaIntegracao.ValorFrete.IBSCBS != null)
                    {
                        cargaPedido.CSTIBSCBS = cargaIntegracao.ValorFrete.IBSCBS.CST;
                        cargaPedido.NBS = cargaIntegracao.ValorFrete.IBSCBS.NBS;
                        cargaPedido.CodigoIndicadorOperacao = cargaIntegracao.ValorFrete.IBSCBS.CodigoIndicadorOperacao;
                        cargaPedido.ClassificacaoTributariaIBSCBS = cargaIntegracao.ValorFrete.IBSCBS.ClassificacaoTributaria;
                        cargaPedido.BaseCalculoIBSCBS = cargaIntegracao.ValorFrete.IBSCBS.BaseCalculo;
                        cargaPedido.AliquotaIBSEstadual = cargaIntegracao.ValorFrete.IBSCBS.AliquotaIBSEstadual;
                        cargaPedido.PercentualReducaoIBSEstadual = cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoIBSEstadual;
                        cargaPedido.ValorIBSEstadual = cargaIntegracao.ValorFrete.IBSCBS.ValorIBSEstadual;
                        cargaPedido.AliquotaIBSMunicipal = cargaIntegracao.ValorFrete.IBSCBS.AliquotaIBSMunicipal;
                        cargaPedido.PercentualReducaoIBSMunicipal = cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoIBSMunicipal;
                        cargaPedido.ValorIBSMunicipal = cargaIntegracao.ValorFrete.IBSCBS.ValorIBSMunicipal;
                        cargaPedido.AliquotaCBS = cargaIntegracao.ValorFrete.IBSCBS.AliquotaCBS;
                        cargaPedido.PercentualReducaoCBS = cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoCBS;
                        cargaPedido.ValorCBS = cargaIntegracao.ValorFrete.IBSCBS.ValorCBS;
                        carga.ValorCBS += cargaPedido.ValorCBS - valorCBSOrigem;
                        carga.ValorIBSMunicipal += cargaPedido.ValorIBSMunicipal - valorIBSMunicipalOrigem;
                        carga.ValorIBSEstadual += cargaPedido.ValorIBSEstadual - valorIBSEstadualOrigem;
                    }

                    if (cargaIntegracao.ValorFrete.ComponentesAdicionais != null)
                    {
                        Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                        repPedidoComponenteFrete.DeletarPorPedido(cargaPedido.Pedido.Codigo);
                        repCargaPedidoComponentesFrete.DeletarPorCargaPedido(cargaPedido.Codigo, false);

                        foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional in cargaIntegracao.ValorFrete.ComponentesAdicionais)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();
                            Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete pedidoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();

                            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.buscarPorCodigoEmbarcador(componenteAdicional.Componente.CodigoIntegracao);
                            if (componenteFrete != null)
                            {
                                cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                                cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                                cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = componenteAdicional.IncluirBaseCalculoICMS;
                                cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                                cargaPedidoComponenteFrete.DescontarValorTotalAReceber = componenteAdicional.DescontarValorTotalAReceber;
                                cargaPedidoComponenteFrete.TipoComponenteFrete = cargaPedidoComponenteFrete.ComponenteFrete.TipoComponenteFrete;

                                if (cargaPedidoComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                                {
                                    cargaPedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                                    cargaPedidoComponenteFrete.Percentual = componenteAdicional.ValorComponente;
                                    cargaPedidoComponenteFrete.ValorComponente = 0;
                                }
                                else
                                {
                                    cargaPedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                                    cargaPedidoComponenteFrete.ValorComponente = componenteAdicional.ValorComponente;
                                }
                                repCargaPedidoComponentesFrete.Inserir(cargaPedidoComponenteFrete);

                                bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(carga.TabelaFrete, cargaPedidoComponenteFrete.ComponenteFrete);
                                bool descontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? carga.TabelaFrete?.DescontarComponenteFreteLiquido : cargaPedidoComponenteFrete.ComponenteFrete.DescontarComponenteFreteLiquido) ?? false;

                                if (cargaPedidoComponenteFrete.ComponenteFrete.SomarComponenteFreteLiquido)
                                    valorFreteLiquido += componenteAdicional.ValorComponente;

                                if (descontarComponenteFreteLiquido)
                                    valorFreteLiquido += componenteAdicional.ValorComponente * -1;

                                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                {
                                    pedidoComponenteFrete.ComponenteFrete = componenteFrete;
                                    pedidoComponenteFrete.Pedido = cargaPedido.Pedido;
                                    pedidoComponenteFrete.IncluirBaseCalculoICMS = componenteAdicional.IncluirBaseCalculoICMS;
                                    pedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                                    pedidoComponenteFrete.DescontarValorTotalAReceber = componenteAdicional.DescontarValorTotalAReceber;
                                    pedidoComponenteFrete.TipoComponenteFrete = pedidoComponenteFrete.ComponenteFrete.TipoComponenteFrete;

                                    if (pedidoComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                                    {
                                        pedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                                        pedidoComponenteFrete.Percentual = componenteAdicional.ValorComponente;
                                        pedidoComponenteFrete.ValorComponente = 0;
                                    }
                                    else
                                    {
                                        pedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                                        pedidoComponenteFrete.ValorComponente = componenteAdicional.ValorComponente;
                                    }
                                    repPedidoComponenteFrete.Inserir(pedidoComponenteFrete);
                                }
                            }
                            else
                            {
                                stMensagem.Append("O código informado para o componente de frete (" + componenteAdicional.Componente.CodigoIntegracao + ") não existe na base da Multisoftware.");
                            }
                        }
                    }

                    if ((cargaIntegracao.ValorFrete.ICMS == null && cargaPedido.PossuiCTe) || (cargaIntegracao.ValorFrete.ISS == null && cargaPedido.PossuiNFS))
                    {
                        carga.ValorFrete += cargaPedido.ValorFrete - valorOrigem;
                        if (cargaPedido.ValorFreteAPagar > 0)
                            carga.ValorFreteAPagar += cargaPedido.ValorFreteAPagar - valorPagarOrigem;
                        else
                            carga.ValorFreteAPagar += cargaPedido.ValorFrete - valorOrigem;
                    }
                    else
                    {
                        carga.ValorFrete += cargaPedido.ValorFrete - valorOrigem;
                        carga.ValorFreteAPagar += cargaPedido.ValorFreteAPagar - valorPagarOrigem;

                        if (cargaPedido.PossuiCTe)
                            cargaPedido.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("57");
                        if (cargaPedido.PossuiNFS)
                            cargaPedido.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("39");
                        if (cargaPedido.PossuiNFSManual)
                            cargaPedido.ModeloDocumentoFiscal = null;

                        if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao && carga.TipoOperacao.ModeloDocumentoFiscal != null)
                            cargaPedido.ModeloDocumentoFiscal = carga.TipoOperacao.ModeloDocumentoFiscal;
                    }

                    carga.ValorFreteLiquido = Math.Round(carga.ValorFrete + valorFreteLiquido, 2, MidpointRounding.AwayFromZero);
                    carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                    repCarga.Atualizar(carga);
                }

                InformarDocumentosCargaPedido(carga, cargaPedido, cargaIntegracao, ref stMensagem, unitOfWork, tipoServicoMultisoftware, configuracao, auditado, true);

                if (configuracao.IncluirCargaCanceladaProcessarDT)
                {
                    List<int> codigosPedido = new List<int> { cargaPedido.Pedido.Codigo };

                    Dominio.Entidades.Embarcador.Pedidos.Stage stage = repPedidoStage.BuscarPorListaPedidos(codigosPedido).OrderByDescending(x => x.Codigo).FirstOrDefault();

                    if (stage != null)
                    {
                        cargaPedido.Expedidor = stage.Expedidor;
                        cargaPedido.Recebedor = stage.Recebedor;
                    }
                }

                repCargaPedido.Atualizar(cargaPedido);
            }
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CriarCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, int protocoloPedidoExistente, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, bool validarTipoOperacaoMunicipal = false)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serRegraICMS = new Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.NFe.NFe serNFe = new Embarcador.NFe.NFe(unitOfWork);

            Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);

            decimal valorCotacao = 0;
            if (cargaIntegracao != null && cargaIntegracao.Remetente != null)
                valorCotacao = cargaIntegracao.ValorTaxaFeeder;

            Servicos.Embarcador.Carga.CTe serCTe = new Embarcador.Carga.CTe(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = new Dominio.Entidades.Embarcador.Cargas.CargaPedido();
            cargaPedido.Carga = carga != null && carga.CargaAgrupamento != null ? carga.CargaAgrupamento : carga;
            cargaPedido.CargaOrigem = carga;
            cargaPedido.Pedido = pedido;
            cargaPedido.Ativo = true;
            cargaPedido.ValorFrete = 0;
            cargaPedido.ValorDescarga = cargaIntegracao.ValorDescarga;
            cargaPedido.ValorPedagio = cargaIntegracao.ValorPedagio;
            cargaPedido.PedidoPallet = cargaIntegracao.PedidoPallet;
            cargaPedido.CustoFrete = cargaIntegracao.CustoFrete;
            cargaPedido.TipoPaleteCliente = pedido.TipoPaleteCliente;

            if ((cargaPedido?.TipoPaleteCliente ?? TipoPaleteCliente.NaoDefinido) != TipoPaleteCliente.NaoDefinido)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> tiposPallet = repositorioTipoDetalhe.BuscarPorTipo(TipoTipoDetalhe.TipoPallet);
                Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhePalete = (from o in tiposPallet where o.TipoPaleteCliente == cargaPedido.TipoPaleteCliente select o).FirstOrDefault();
                if (tipoDetalhePalete != null)
                    cargaPedido.PesoPallet = (cargaPedido.Pallet * tipoDetalhePalete?.Valor ?? 0);
            }

            cargaPedido.TipoCobrancaMultimodal = cargaIntegracao.TipoCobrancaMultimodal != 0 ? cargaIntegracao.TipoCobrancaMultimodal : TipoCobrancaMultimodal.Nenhum;
            //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
            Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {pedido.PesoTotal}. Carga.CriarCargaPedido", "PesoCargaPedido");
            cargaPedido.Peso = pedido.PesoTotal;
            cargaPedido.PesoLiquido = pedido.PesoLiquidoTotal;

            cargaPedido.OrdemEntrega = cargaIntegracao.OrdemEntrega;
            cargaPedido.OrdemColeta = cargaIntegracao.OrdemColeta;

            if (cargaPedido.OrdemEntrega > 0 || cargaPedido.OrdemColeta > 0)
                carga.OrdemRoteirizacaoDefinida = true;

            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao && carga.TipoOperacao.ModeloDocumentoFiscal != null)
                cargaPedido.ModeloDocumentoFiscal = carga.TipoOperacao.ModeloDocumentoFiscal;

            repCargaPedido.Inserir(cargaPedido);

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                List<Dominio.Entidades.Cliente> clientesBloquearEmissaoDosDestinatarios = carga.TipoOperacao?.ClientesBloquearEmissaoDosDestinatario.ToList();
                serCargaPedido.BuscarConfiguracoesMultimodal(carga, unitOfWork, ref cargaPedido, integracaoIntercab, clientesBloquearEmissaoDosDestinatarios);
            }

            cargaPedido.TipoRateio = serCTe.BuscarTipoEmissaoDocumentosCTe(cargaPedido, tipoServicoMultisoftware, unitOfWork);

            if (cargaPedido.FormulaRateio == null && (carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false))
                cargaPedido.FormulaRateio = carga.TipoOperacao.RateioFormula;

            cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.Nova;
            PreencherCargaPedidoOrigemDestino(ref cargaPedido, pedido, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, unitOfWork);

            if (pedido.OrdemColetaProgramada > 0)
            {
                cargaPedido.OrdemColeta = pedido.OrdemColetaProgramada;
                carga.OrdemRoteirizacaoDefinida = true;
            }

            if (protocoloPedidoExistente > 0)//todo: rever a forma de buscar o pedido anterior quando for um redespacho
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedidoDiferenteCargaPedido(protocoloPedidoExistente, cargaPedido.Codigo);
                if (cargaPedidos.Count > 0)
                {
                    if (carga.TipoOperacao != null && carga.TipoOperacao.OperacaoDeRedespacho)
                    {
                        cargaPedido.CargaPedidoTrechoAnterior = (from obj in cargaPedidos where obj.Codigo > 0 && (obj.CargaPedidoProximoTrecho == null || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada) && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada select obj).FirstOrDefault();

                        if (cargaPedido.CargaPedidoTrechoAnterior != null)
                        {
                            cargaPedido.Redespacho = true;
                            if (serCarga.VerificarSeCargaEstaNaLogistica(cargaPedido.CargaPedidoTrechoAnterior.Carga, tipoServicoMultisoftware) ||
                                cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                                cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
                            {
                                carga.AguardandoEmissaoDocumentoAnterior = true;
                            }

                            if (cargaPedido.Expedidor == null)
                            {
                                cargaPedido.TipoEmissaoCTeParticipantes = cargaPedido.Recebedor == null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
                                cargaPedido.Expedidor = cargaPedido.CargaPedidoTrechoAnterior.Recebedor;
                                if (cargaPedido.Expedidor != null)
                                    cargaPedido.Origem = cargaPedido.Expedidor.Localidade;
                            }

                            if (cargaPedido.Expedidor == null && cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarExpedidorComoTransportador && cargaPedido.Carga.Empresa != null)
                            {
                                cargaPedido.TipoEmissaoCTeParticipantes = cargaPedido.Recebedor == null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
                                cargaPedido.Expedidor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(carga.Empresa.CNPJ_SemFormato)));
                                if (cargaPedido.Expedidor != null)
                                    cargaPedido.Origem = cargaPedido.Expedidor.Localidade;
                            }

                            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarExpedidorComoTransportador && cargaPedido.Carga.Empresa == null && cargaPedido.Expedidor != null)
                                cargaPedido.Carga.Empresa = repEmpresa.BuscarPorCNPJ(cargaPedido.Expedidor.CPF_CNPJ_SemFormato);
                        }
                        else
                        {
                            stMensagem.Append("Não existe uma carga feita anteriormente para esse pedido disponível para ser redespachada");
                            return null;
                        }
                    }
                    else
                    {
                        cargaPedido.CargaPedidoProximoTrecho = (from obj in cargaPedidos where obj.Codigo > 0 && obj.Carga.TipoOperacao != null && obj.Carga.TipoOperacao.OperacaoDeRedespacho && (obj.CargaPedidoTrechoAnterior == null || obj.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || obj.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada) && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada select obj).FirstOrDefault();
                        if (cargaPedido.CargaPedidoProximoTrecho != null)
                        {
                            if (cargaPedido.Recebedor == null)
                            {
                                cargaPedido.TipoEmissaoCTeParticipantes = cargaPedido.Expedidor == null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
                                cargaPedido.Recebedor = cargaPedido.CargaPedidoProximoTrecho.Recebedor;
                                if (cargaPedido.Recebedor != null)
                                    cargaPedido.Destino = cargaPedido.Recebedor.Localidade;
                            }
                        }
                    }
                }
                else
                {
                    if (carga.TipoOperacao != null && carga.TipoOperacao.OperacaoDeRedespacho)
                    {
                        cargaPedido.Redespacho = true;
                        carga.AguardandoEmissaoDocumentoAnterior = true;

                        if (cargaPedido.Expedidor == null && cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarExpedidorComoTransportador && cargaPedido.Carga.Empresa != null)
                        {
                            cargaPedido.TipoEmissaoCTeParticipantes = cargaPedido.Recebedor == null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
                            cargaPedido.Expedidor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(carga.Empresa.CNPJ_SemFormato)));
                            if (cargaPedido.Expedidor != null)
                                cargaPedido.Origem = cargaPedido.Expedidor.Localidade;
                        }

                        if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarExpedidorComoTransportador && cargaPedido.Carga.Empresa == null && cargaPedido.Expedidor != null)
                            cargaPedido.Carga.Empresa = repEmpresa.BuscarPorCNPJ(cargaPedido.Expedidor.CPF_CNPJ_SemFormato);
                    }
                }
            }

            serCargaPedido.VerificarFilialEmissaoCargaPedido(cargaPedido, configuracaoGeralCarga);

            repCargaPedido.Atualizar(cargaPedido);
            PreencherDistribuidores(ref cargaPedido, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, unitOfWork);

            Servicos.Embarcador.Carga.Carga.CalcularValorDescargaPorCargaPedido(cargaPedido, configuracao, unitOfWork);

            repCargaPedido.Atualizar(cargaPedido);

            carga.CargaDestinadaCTeComplementar = carga?.TipoOperacao?.OperacaoDestinadaCTeComplementar ?? false;
            if (carga.CargaDestinadaCTeComplementar)
            {
                string numeroOSMae = repPedido.BuscarNumeroOSMae(cargaPedido.Pedido?.Codigo ?? 0);
                if (!string.IsNullOrWhiteSpace(numeroOSMae))
                    serCarga.VincularMotoristaVeiculosOSMae(carga, cargaPedido.Pedido, numeroOSMae, unitOfWork, tipoServicoMultisoftware, configuracao);
            }

            if (carga.TipoOperacao != null && carga.TipoOperacao.FretePorContadoCliente)
            {
                carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente;
                repCarga.Atualizar(carga);
            }

            if (cargaPedido.Redespacho && cargaPedido.CargaPedidoTrechoAnterior != null)
            {
                cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoProximoTrecho = cargaPedido;
                repCargaPedido.Atualizar(cargaPedido.CargaPedidoTrechoAnterior);
                serNFe.VincularNotasDeRedespachoVinculadas(cargaPedido, cargaPedido.CargaPedidoTrechoAnterior, unitOfWork, tipoServicoMultisoftware, Auditado);
            }
            else if (cargaPedido.CargaPedidoProximoTrecho != null && cargaPedido.CargaPedidoProximoTrecho.Redespacho)
            {
                cargaPedido.CargaPedidoProximoTrecho.CargaPedidoTrechoAnterior = cargaPedido;
                repCargaPedido.Atualizar(cargaPedido.CargaPedidoProximoTrecho);
                serNFe.VincularNotasDeRedespachoVinculadas(cargaPedido, cargaPedido.CargaPedidoProximoTrecho, unitOfWork, tipoServicoMultisoftware, Auditado);
            }

            bool possuiCTe = false;
            bool possuiNFS = false;
            bool possuiNFSManual = false;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal = null;

            serCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, cargaPedido.Origem, cargaPedido.Destino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoFiscalIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual, validarTipoOperacaoMunicipal);

            cargaPedido.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;
            cargaPedido.PossuiCTe = possuiCTe;
            cargaPedido.PossuiNFS = possuiNFS;
            cargaPedido.PossuiNFSManual = possuiNFSManual;

            if (cargaPedido.PossuiNFSManual && cargaPedido.CargaPedidoFilialEmissora)
                cargaPedido.CargaPedidoFilialEmissora = false;

            cargaPedido.ModeloDocumentoFiscalIntramunicipal = modeloDocumentoFiscalIntramunicipal;

            cargaPedido.IncluirICMSBCFreteProprio = cargaIntegracao.ValorFrete?.ICMS?.IncluirICMSBCFreteProprio;

            if (cargaIntegracao.ValorFrete?.ICMS?.IncluirICMSBCFreteProprio == false)
            {
                cargaIntegracao.ValorFrete.ValorTotalAReceber = cargaIntegracao.ValorFrete.FreteProprio;
                cargaIntegracao.ValorFrete.FreteProprio = 0;
                cargaIntegracao.ValorFrete.ICMS = null;
            }
            if (cargaIntegracao.ValorFrete?.ICMS?.IncluirICMSBCFreteProprio == true)
                cargaIntegracao.ValorFrete.ICMS = null;

            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente)
            {
                if (cargaIntegracao.ValorFrete != null)
                {
                    if (cargaIntegracao.ValorFrete.FreteProprio == 0 && cargaIntegracao.ValorFrete.ValorTotalAReceber > 0)//neste caso deve encontrar o valor do frete liquido
                    {
                        if (cargaPedido.PossuiCTe)
                        {
                            bool incluirBase = false;
                            decimal baseCalculo = cargaIntegracao.ValorFrete.ValorTotalAReceber;
                            decimal percentualIncluir = 100;
                            if (cargaIntegracao.ValorFrete.ICMS != null)
                            {
                                if (cargaIntegracao.ValorFrete.ICMS.CST == "60")
                                {
                                    cargaIntegracao.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.ValorTotalAReceber;
                                    //cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS += cargaIntegracao.ValorFrete.ICMS.ValorICMS;
                                }
                                else
                                    cargaIntegracao.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.ValorTotalAReceber - cargaIntegracao.ValorFrete.ICMS.ValorICMS;
                            }
                            else
                            {
                                if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                                    cargaIntegracao.ValorFrete.FreteProprio = 0; //CargoX não envia os participantes, por isso o calculo do valor liquido é feito depois que recebe as notas.
                                else
                                {
                                    Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serRegraICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, cargaPedido.Carga.Empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirBase, ref percentualIncluir, baseCalculo, null, unitOfWork, tipoServicoMultisoftware, configuracao);
                                    if (regraICMS.CST == "60")
                                        cargaIntegracao.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.ValorTotalAReceber;
                                    else if (regraICMS.Aliquota == 0)
                                        cargaIntegracao.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.ValorTotalAReceber;
                                    else
                                        cargaIntegracao.ValorFrete.FreteProprio = serRegraICMS.ObterValorLiquido(cargaIntegracao.ValorFrete.ValorTotalAReceber, regraICMS.Aliquota);
                                }

                            }

                        }
                        else if (cargaPedido.PossuiNFS || cargaPedido.PossuiNFSManual) // regra para Danone, não remover o ISS, rever isso pois o ISS tem uma regra diferente.
                        {
                            if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                                cargaIntegracao.ValorFrete.FreteProprio = 0; //CargoX não envia os participantes, por isso o calculo do valor liquido é feito depois que recebe as notas.
                            else
                                cargaIntegracao.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.ValorTotalAReceber;
                        }
                    }

                    cargaPedido.ValorFrete = cargaIntegracao.ValorFrete.FreteProprio;
                    cargaPedido.ValorFreteAPagar = cargaIntegracao.ValorFrete.ValorTotalAReceber;
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cargaIntegracao.ValorFrete.FreteProprio > 0)
                    {
                        pedido.ValorFreteNegociado = valorCotacao > 0 ? (cargaIntegracao.ValorFrete.FreteProprio * valorCotacao) : cargaIntegracao.ValorFrete.FreteProprio;
                        if (valorCotacao > 0 && configuracao.UtilizaEmissaoMultimodal)
                            pedido.ObservacaoCTe += " Conversão de valor em dólar: " + valorCotacao.ToString("n2");
                        pedido.ImprimirObservacaoCTe = true;
                        repPedido.Atualizar(pedido);
                    }

                    if (cargaIntegracao.ValorFrete.ICMS != null && cargaPedido.PossuiCTe)
                    {
                        cargaPedido.PercentualAliquota = cargaIntegracao.ValorFrete.ICMS.Aliquota;
                        cargaPedido.IncluirICMSBaseCalculo = cargaIntegracao.ValorFrete.ICMS.IncluirICMSBC;
                        cargaPedido.ImpostoInformadoPeloEmbarcador = true;
                        cargaPedido.ValorICMS = cargaIntegracao.ValorFrete.ICMS.ValorICMS;
                        cargaPedido.BaseCalculoICMS = cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS;
                        cargaPedido.ObservacaoRegraICMSCTe = cargaIntegracao.ValorFrete.ICMS.ObservacaoCTe;

                        Servicos.Log.TratarErro($"12 - CargaPedido: {cargaPedido.Codigo} -> Aliquota: {cargaPedido.PercentualAliquota}", "ProcessarAliquota");

                        bool incluirBC = cargaPedido.IncluirICMSBaseCalculo;
                        decimal percentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;

                        Dominio.Entidades.Empresa empresa = cargaPedido.Carga.Empresa;
                        Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serRegraICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirBC, ref percentualIncluirBaseCalculo, cargaPedido.BaseCalculoICMS, null, unitOfWork, tipoServicoMultisoftware, configuracao);

                        //bool subcontratacao = false;
                        //if (cargaPedido.CargaPedidoFilialEmissora && cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                        //{
                        //    cargaPedido.ValorFreteAPagar = cargaPedido.ValorFrete;
                        //    subcontratacao = true;
                        //    cargaPedido.ValorICMS = regraICMS.ValorICMS;
                        //    cargaPedido.CST = regraICMS.CST;
                        //    cargaPedido.PercentualAliquota = regraICMS.Aliquota;
                        //    cargaPedido.IncluirICMSBaseCalculo = regraICMS.IncluirICMSBC;
                        //}

                        carga.ValorICMS += cargaPedido.ValorICMS;

                        if (cargaIntegracao.CFOP > 0)
                        {
                            cargaPedido.CFOP = repCFOP.BuscarPorNumero(cargaIntegracao.CFOP);
                            if (cargaPedido.CFOP == null)
                                stMensagem.Append("A CFOP informada (" + cargaIntegracao.CFOP + "), é inválida.");
                        }
                        else
                        {
                            cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);
                        }

                        if (!string.IsNullOrWhiteSpace(regraICMS.ObservacaoCTe))
                            cargaPedido.ObservacaoRegraICMSCTe += regraICMS.ObservacaoCTe;

                        //if (!subcontratacao)
                        //{
                        if (!string.IsNullOrWhiteSpace(cargaIntegracao.ValorFrete.ICMS.CST))
                        {
                            cargaPedido.CST = cargaIntegracao.ValorFrete.ICMS.CST;
                            cargaPedido.PercentualIncluirBaseCalculo = cargaIntegracao.ValorFrete.ICMS.PercentualInclusaoBC;
                            cargaPedido.PercentualReducaoBC = cargaIntegracao.ValorFrete.ICMS.PercentualReducaoBC;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                            {
                                cargaPedido.CST = regraICMS.CST;
                                cargaPedido.PercentualIncluirBaseCalculo = regraICMS.PercentualInclusaoBC;
                                cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                            }
                            else
                                stMensagem.Append("É obrigatório informar a CST");
                        }
                        //}

                        if (cargaPedido.ValorFreteAPagar == 0 && cargaIntegracao.ValorFrete.ValorPrestacaoServico > 0)
                        {
                            if (regraICMS.DescontarICMSDoValorAReceber && cargaPedido.ValorICMS > 0)
                                cargaPedido.ValorFreteAPagar = cargaIntegracao.ValorFrete.ValorPrestacaoServico - cargaPedido.ValorICMS;
                            else
                                cargaPedido.ValorFreteAPagar = cargaIntegracao.ValorFrete.ValorPrestacaoServico;

                        }

                        if (regraICMS.SimplesNacional || cargaIntegracao.ValorFrete.ICMS.SimplesNacional)
                            cargaPedido.CST = "";
                    }
                    else
                    {
                        cargaPedido.PercentualIncluirBaseCalculo = 100;
                        cargaPedido.IncluirICMSBaseCalculo = true;
                    }

                    if (cargaIntegracao.ValorFrete.ISS != null && cargaPedido.PossuiNFS)
                    {
                        cargaPedido.PercentualAliquotaISS = cargaIntegracao.ValorFrete.ISS.Aliquota;
                        cargaPedido.IncluirISSBaseCalculo = cargaIntegracao.ValorFrete.ISS.IncluirISSBaseCalculo;
                        cargaPedido.PercentualRetencaoISS = cargaIntegracao.ValorFrete.ISS.PercentualRetencao;
                        cargaPedido.ValorISS = cargaIntegracao.ValorFrete.ISS.ValorISS;
                        cargaPedido.BaseCalculoISS = cargaIntegracao.ValorFrete.ISS.ValorBaseCalculoISS;
                        cargaPedido.ImpostoInformadoPeloEmbarcador = true;
                        carga.ValorISS += cargaPedido.ValorISS;
                        carga.ValorRetencaoISS += cargaPedido.ValorRetencaoISS;
                    }

                    if (cargaIntegracao.ValorFrete.IBSCBS != null)
                    {
                        cargaPedido.CSTIBSCBS = cargaIntegracao.ValorFrete.IBSCBS.CST;
                        cargaPedido.CodigoIndicadorOperacao = cargaIntegracao.ValorFrete.IBSCBS.CodigoIndicadorOperacao;
                        cargaPedido.NBS = cargaIntegracao.ValorFrete.IBSCBS.NBS;
                        cargaPedido.ClassificacaoTributariaIBSCBS = cargaIntegracao.ValorFrete.IBSCBS.ClassificacaoTributaria;
                        cargaPedido.BaseCalculoIBSCBS = cargaIntegracao.ValorFrete.IBSCBS.BaseCalculo;
                        cargaPedido.AliquotaIBSEstadual = cargaIntegracao.ValorFrete.IBSCBS.AliquotaIBSEstadual;
                        cargaPedido.PercentualReducaoIBSEstadual = cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoIBSEstadual;
                        cargaPedido.ValorIBSEstadual = cargaIntegracao.ValorFrete.IBSCBS.ValorIBSEstadual;
                        cargaPedido.AliquotaIBSMunicipal = cargaIntegracao.ValorFrete.IBSCBS.AliquotaIBSMunicipal;
                        cargaPedido.PercentualReducaoIBSMunicipal = cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoIBSMunicipal;
                        cargaPedido.ValorIBSMunicipal = cargaIntegracao.ValorFrete.IBSCBS.ValorIBSMunicipal;
                        cargaPedido.AliquotaCBS = cargaIntegracao.ValorFrete.IBSCBS.AliquotaCBS;
                        cargaPedido.PercentualReducaoCBS = cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoCBS;
                        cargaPedido.ValorCBS = cargaIntegracao.ValorFrete.IBSCBS.ValorCBS;
                        carga.ValorCBS += cargaPedido.ValorCBS;
                        carga.ValorIBSMunicipal += cargaPedido.ValorIBSMunicipal;
                        carga.ValorIBSEstadual += cargaPedido.ValorIBSEstadual;
                    }

                    decimal valorFreteLiquido = 0;
                    CriarComponentesCargaPedido(cargaIntegracao.ValorFrete.ComponentesAdicionais, cargaPedido, pedido, false, valorCotacao, ref valorFreteLiquido, ref stMensagem, tipoServicoMultisoftware, unitOfWork);

                    if ((cargaIntegracao.ValorFrete.ICMS == null && cargaPedido.PossuiCTe) || (cargaIntegracao.ValorFrete.ISS == null && cargaPedido.PossuiNFS))
                    {
                        carga.ValorFrete += cargaPedido.ValorFrete;
                        if (cargaPedido.ValorFreteAPagar > 0)
                            carga.ValorFreteAPagar += cargaPedido.ValorFreteAPagar;
                        else
                            carga.ValorFreteAPagar += cargaPedido.ValorFrete;
                        //Comentado pois achamos que não é necessário calcular o imposto nesse momento, a thread de rateio de frete irá calcular esse imposto novamente.
                        //Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Embarcador.Carga.RateioFrete(unitOfWork);
                        //serRateioFrete.CalcularImpostos(ref carga, cargaPedido, cargaPedido.ValorFrete, false, tipoServicoMultisoftware, unitOfWork);
                    }
                    else
                    {
                        carga.ValorFrete += cargaPedido.ValorFrete;
                        carga.ValorFreteAPagar += cargaPedido.ValorFreteAPagar;

                        if (cargaPedido.PossuiCTe)
                            cargaPedido.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("57");
                        if (cargaPedido.PossuiNFS)
                            cargaPedido.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("39");
                        if (cargaPedido.PossuiNFSManual)
                            cargaPedido.ModeloDocumentoFiscal = null;

                        if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao && carga.TipoOperacao.ModeloDocumentoFiscal != null)
                            cargaPedido.ModeloDocumentoFiscal = carga.TipoOperacao.ModeloDocumentoFiscal;
                    }

                    //if (cargaPedido.ValorFreteAPagar > 0)
                    //    Servicos.Embarcador.Carga.FreteFilialEmissora.SetarFreteEmbarcadorFilialEmissora(ref carga, cargaPedido, tipoServicoMultisoftware, true, unitOfWork);

                    carga.ValorFreteLiquido = Math.Round(carga.ValorFrete + valorFreteLiquido, 2, MidpointRounding.AwayFromZero);
                    carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                    serCarga.InformarSituacaoCargaFreteValido(ref carga, tipoServicoMultisoftware, unitOfWork);
                    repCarga.Atualizar(carga);
                }
                else
                {
                    cargaPedido.PercentualIncluirBaseCalculo = 100;
                    cargaPedido.IncluirICMSBaseCalculo = true;
                }

                if (cargaIntegracao.ValorFreteFilialEmissora != null)
                {
                    if (cargaIntegracao.ValorFreteFilialEmissora.FreteProprio == 0 && cargaIntegracao.ValorFreteFilialEmissora.ValorTotalAReceber > 0)//neste caso deve encontrar o valor do frete liquido
                    {
                        bool incluirBase = false;
                        decimal baseCalculo = cargaIntegracao.ValorFreteFilialEmissora.ValorTotalAReceber;
                        decimal percentualIncluir = 100;
                        if (cargaIntegracao.ValorFreteFilialEmissora.ICMS != null)
                        {
                            if (cargaIntegracao.ValorFreteFilialEmissora.ICMS.CST == "60")
                            {
                                cargaIntegracao.ValorFreteFilialEmissora.FreteProprio = cargaIntegracao.ValorFreteFilialEmissora.ValorTotalAReceber;
                                //cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS += cargaIntegracao.ValorFrete.ICMS.ValorICMS;
                            }
                            else
                                cargaIntegracao.ValorFreteFilialEmissora.FreteProprio = cargaIntegracao.ValorFreteFilialEmissora.ValorTotalAReceber - cargaIntegracao.ValorFreteFilialEmissora.ICMS.ValorICMS;
                        }
                        else
                        {
                            if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                                cargaIntegracao.ValorFreteFilialEmissora.FreteProprio = 0; //CargoX não envia os participantes, por isso o calculo do valor liquido é feito depois que recebe as notas.
                            else
                            {
                                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serRegraICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, cargaPedido.Carga.EmpresaFilialEmissora, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirBase, ref percentualIncluir, cargaPedido.BaseCalculoICMS, null, unitOfWork, tipoServicoMultisoftware, configuracao);
                                if (regraICMS.CST == "60")
                                    cargaIntegracao.ValorFreteFilialEmissora.FreteProprio = cargaIntegracao.ValorFreteFilialEmissora.ValorTotalAReceber;
                                else
                                    cargaIntegracao.ValorFreteFilialEmissora.FreteProprio = cargaIntegracao.ValorFreteFilialEmissora.ValorTotalAReceber - regraICMS.ValorICMS;
                            }
                        }

                    }

                    cargaPedido.ValorFreteFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.FreteProprio;
                    cargaPedido.ValorFreteAPagarFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.ValorTotalAReceber;

                    if (cargaIntegracao.ValorFreteFilialEmissora.ICMS != null)
                    {
                        cargaPedido.PercentualAliquotaFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.ICMS.Aliquota;
                        cargaPedido.IncluirICMSBaseCalculoFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.ICMS.IncluirICMSBC;
                        cargaPedido.ImpostoInformadoPeloEmbarcador = true;
                        cargaPedido.ValorICMSFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.ICMS.ValorICMS;
                        cargaPedido.BaseCalculoICMSFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.ICMS.ValorBaseCalculoICMS;
                        cargaPedido.ObservacaoRegraICMSCTeFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.ICMS.ObservacaoCTe;

                        bool incluirBC = cargaPedido.IncluirICMSBaseCalculo;
                        decimal percentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo;

                        Dominio.Entidades.Empresa empresa = cargaPedido.Carga.EmpresaFilialEmissora;
                        Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serRegraICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, ref incluirBC, ref percentualIncluirBaseCalculo, cargaPedido.BaseCalculoICMS, null, unitOfWork, tipoServicoMultisoftware, configuracao);

                        carga.ValorICMSFilialEmissora += cargaPedido.ValorICMS;
                        cargaPedido.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);

                        if (!string.IsNullOrWhiteSpace(regraICMS.ObservacaoCTe))
                            cargaPedido.ObservacaoRegraICMSCTe += regraICMS.ObservacaoCTe;

                        //if (!subcontratacao)
                        //{
                        if (!string.IsNullOrWhiteSpace(cargaIntegracao.ValorFreteFilialEmissora.ICMS.CST))
                        {
                            cargaPedido.CST = cargaIntegracao.ValorFreteFilialEmissora.ICMS.CST;
                            cargaPedido.PercentualIncluirBaseCalculo = cargaIntegracao.ValorFreteFilialEmissora.ICMS.PercentualInclusaoBC;
                            cargaPedido.PercentualReducaoBC = cargaIntegracao.ValorFreteFilialEmissora.ICMS.PercentualReducaoBC;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(regraICMS.CST))
                            {
                                cargaPedido.CST = regraICMS.CST;
                                cargaPedido.PercentualIncluirBaseCalculo = regraICMS.PercentualInclusaoBC;
                                cargaPedido.PercentualReducaoBC = regraICMS.PercentualReducaoBC;
                            }
                            else
                                stMensagem.Append("É obrigatório informar a CST");
                        }
                        //}

                        if (cargaPedido.ValorFreteAPagarFilialEmissora == 0 && cargaIntegracao.ValorFreteFilialEmissora.ValorPrestacaoServico > 0)
                        {
                            if (regraICMS.DescontarICMSDoValorAReceber && cargaPedido.ValorICMSFilialEmissora > 0)
                                cargaPedido.ValorFreteAPagarFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.ValorPrestacaoServico - cargaPedido.ValorICMSFilialEmissora;
                            else
                                cargaPedido.ValorFreteAPagarFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.ValorPrestacaoServico;

                        }

                        if (regraICMS.SimplesNacional || cargaIntegracao.ValorFreteFilialEmissora.ICMS.SimplesNacional)
                            cargaPedido.CST = "";
                    }
                    else
                    {
                        cargaPedido.PercentualIncluirBaseCalculoFilialEmissora = 100;
                        cargaPedido.IncluirICMSBaseCalculoFilialEmissora = true;
                    }

                    if (cargaIntegracao.ValorFreteFilialEmissora.IBSCBS != null)
                    {
                        cargaPedido.CSTIBSCBSFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.CST;
                        cargaPedido.ClassificacaoTributariaIBSCBSFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.ClassificacaoTributaria;
                        cargaPedido.BaseCalculoIBSCBSFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.BaseCalculo;
                        cargaPedido.AliquotaIBSEstadualFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.AliquotaIBSEstadual;
                        cargaPedido.PercentualReducaoIBSEstadualFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.PercentualReducaoIBSEstadual;
                        cargaPedido.ValorIBSEstadualFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.ValorIBSEstadual;
                        cargaPedido.AliquotaIBSMunicipalFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.AliquotaIBSMunicipal;
                        cargaPedido.PercentualReducaoIBSMunicipalFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.PercentualReducaoIBSMunicipal;
                        cargaPedido.ValorIBSMunicipalFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.ValorIBSMunicipal;
                        cargaPedido.AliquotaCBSFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.AliquotaCBS;
                        cargaPedido.PercentualReducaoCBSFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.PercentualReducaoCBS;
                        cargaPedido.ValorCBSFilialEmissora = cargaIntegracao.ValorFreteFilialEmissora.IBSCBS.ValorCBS;
                        carga.ValorCBSFilialEmissora = +cargaPedido.ValorCBSFilialEmissora;
                        carga.ValorIBSMunicipalFilialEmissora += cargaPedido.ValorIBSMunicipalFilialEmissora;
                        carga.ValorIBSEstadualFilialEmissora += cargaPedido.ValorIBSEstadualFilialEmissora;
                    }

                    decimal valorFreteLiquido = 0;
                    CriarComponentesCargaPedido(cargaIntegracao.ValorFreteFilialEmissora.ComponentesAdicionais, cargaPedido, pedido, true, 0, ref valorFreteLiquido, ref stMensagem, tipoServicoMultisoftware, unitOfWork);

                    if ((cargaIntegracao.ValorFreteFilialEmissora.ICMS == null && cargaPedido.PossuiCTe) || (cargaIntegracao.ValorFreteFilialEmissora.ISS == null && cargaPedido.PossuiNFS))
                    {
                        carga.ValorFreteFilialEmissora += cargaPedido.ValorFreteFilialEmissora;
                        if (cargaPedido.ValorFreteAPagarFilialEmissora > 0)
                            carga.ValorFreteAPagarFilialEmissora += cargaPedido.ValorFreteAPagarFilialEmissora;
                        else
                            carga.ValorFreteAPagarFilialEmissora += cargaPedido.ValorFreteAPagarFilialEmissora;
                    }
                    else
                    {
                        carga.ValorFreteAPagarFilialEmissora += cargaPedido.ValorFreteAPagarFilialEmissora;
                        carga.ValorFreteAPagarFilialEmissora += cargaPedido.ValorFreteAPagarFilialEmissora;
                    }

                    carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                    repCarga.Atualizar(carga);
                }
            }

            InformarDocumentosCargaPedido(carga, cargaPedido, cargaIntegracao, ref stMensagem, unitOfWork, tipoServicoMultisoftware, configuracao, Auditado, false);

            if (cargaIntegracao.TipoOperacao != null && cargaIntegracao.TipoOperacao.TipoServicoMultimodal != TipoServicoMultimodal.Nenhum)
                cargaPedido.TipoServicoMultimodal = cargaIntegracao.TipoOperacao.TipoServicoMultimodal;
            if (cargaIntegracao.TipoOperacao != null && cargaIntegracao.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.Nenhum)
                cargaPedido.TipoPropostaMultimodal = cargaIntegracao.TipoOperacao.TipoPropostaMultimodal;
            if (cargaIntegracao.TipoOperacao != null && cargaIntegracao.TipoOperacao.TipoCobrancaMultimodal != TipoCobrancaMultimodal.Nenhum)
                cargaPedido.TipoCobrancaMultimodal = cargaIntegracao.TipoOperacao.TipoCobrancaMultimodal;
            if (cargaIntegracao.TipoOperacao != null && cargaIntegracao.TipoOperacao.ModalPropostaMultimodal != ModalPropostaMultimodal.Nenhum)
                cargaPedido.ModalPropostaMultimodal = cargaIntegracao.TipoOperacao.ModalPropostaMultimodal;

            return cargaPedido;

        }

        private void CriarComponentesCargaPedido(List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> componentesAdicionais, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool filialEmissora, decimal valorCotacao, ref decimal valorFreteLiquido, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (componentesAdicionais != null)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

                repPedidoComponenteFrete.DeletarPorPedido(pedido.Codigo);
                repCargaPedidoComponentesFrete.DeletarPorCargaPedido(cargaPedido.Codigo, false);

                foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional in componentesAdicionais)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete();
                    Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete pedidoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();

                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.buscarPorCodigoEmbarcador(componenteAdicional.Componente.CodigoIntegracao);
                    if (componenteFrete != null)
                    {
                        cargaPedidoComponenteFrete.ComponenteFrete = componenteFrete;
                        cargaPedidoComponenteFrete.CargaPedido = cargaPedido;
                        cargaPedidoComponenteFrete.IncluirBaseCalculoICMS = componenteAdicional.IncluirBaseCalculoICMS;
                        cargaPedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                        cargaPedidoComponenteFrete.DescontarValorTotalAReceber = componenteAdicional.DescontarValorTotalAReceber;
                        cargaPedidoComponenteFrete.TipoComponenteFrete = cargaPedidoComponenteFrete.ComponenteFrete.TipoComponenteFrete;
                        cargaPedidoComponenteFrete.ComponenteFilialEmissora = filialEmissora;

                        if (cargaPedidoComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                        {
                            cargaPedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                            cargaPedidoComponenteFrete.Percentual = componenteAdicional.ValorComponente;
                            cargaPedidoComponenteFrete.ValorComponente = 0;
                        }
                        else
                        {
                            cargaPedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                            cargaPedidoComponenteFrete.ValorComponente = valorCotacao > 0 ? (componenteAdicional.ValorComponente * valorCotacao) : componenteAdicional.ValorComponente;
                        }

                        repCargaPedidoComponentesFrete.Inserir(cargaPedidoComponenteFrete);

                        bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(cargaPedido.Carga?.TabelaFrete, cargaPedidoComponenteFrete.ComponenteFrete);
                        bool descontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? cargaPedido.Carga?.TabelaFrete?.DescontarComponenteFreteLiquido : cargaPedidoComponenteFrete.ComponenteFrete.DescontarComponenteFreteLiquido) ?? false;

                        if (cargaPedidoComponenteFrete.ComponenteFrete.SomarComponenteFreteLiquido)
                            valorFreteLiquido += valorCotacao > 0 ? (componenteAdicional.ValorComponente * valorCotacao) : componenteAdicional.ValorComponente;

                        if (descontarComponenteFreteLiquido)
                            valorFreteLiquido += valorCotacao > 0 ? ((componenteAdicional.ValorComponente * -1) * valorCotacao) : (componenteAdicional.ValorComponente * -1);

                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            pedidoComponenteFrete.ComponenteFrete = componenteFrete;
                            pedidoComponenteFrete.Pedido = pedido;
                            pedidoComponenteFrete.IncluirBaseCalculoICMS = componenteAdicional.IncluirBaseCalculoICMS;
                            pedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                            pedidoComponenteFrete.DescontarValorTotalAReceber = componenteAdicional.DescontarValorTotalAReceber;
                            pedidoComponenteFrete.TipoComponenteFrete = pedidoComponenteFrete.ComponenteFrete.TipoComponenteFrete;

                            if (pedidoComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                            {
                                pedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                                pedidoComponenteFrete.Percentual = componenteAdicional.ValorComponente;
                                pedidoComponenteFrete.ValorComponente = 0;
                            }
                            else
                            {
                                pedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo;
                                pedidoComponenteFrete.ValorComponente = valorCotacao > 0 ? (componenteAdicional.ValorComponente * valorCotacao) : componenteAdicional.ValorComponente;
                            }
                            repPedidoComponenteFrete.Inserir(pedidoComponenteFrete);
                        }
                    }
                    else
                    {
                        stMensagem.Append("O código informado para o componente de frete (" + componenteAdicional.Componente.CodigoIntegracao + ") não existe na base da Multisoftware.");
                    }
                }
            }
        }

        public void InformarDocumentosCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, bool atualizar)
        {
            Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Servicos.Embarcador.Carga.RateioNotaFiscal serRateioNotaFiscal = new Servicos.Embarcador.Carga.RateioNotaFiscal(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);

            if (cargaPedido != null && cargaIntegracao.CTes != null && cargaIntegracao.CTes.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte in cargaIntegracao.CTes)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                    string retorno = serCTeSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cte, cargaPedido, tipoServicoMultisoftware, ref pedidoCTeParaSubContratacao);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        stMensagem.Append(retorno);
                        break;
                    }
                }
                serCTeSubContratacao.CriarNotasFiscaisDaCargaPedido(cargaPedido, tipoServicoMultisoftware, configuracao, unitOfWork);
                cargaPedido.CienciaDoEnvioDaNotaInformado = true;

                if (cargaIntegracao.FecharCargaAutomaticamente)
                {
                    bool enviouTodas = true;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cp in cargaPedidos)
                    {
                        if (cp.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                        {
                            if (repPedidoXMLNotaFiscal.ContarPorCargaPedido(cp.Codigo) <= 0)
                                enviouTodas = false;
                        }
                        else
                        {
                            if (repPedidoCTeParaSubContratacao.ContarPorCargaPedido(cp.Codigo) <= 0)
                                enviouTodas = false;
                        }

                        if (enviouTodas)
                        {
                            cp.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;
                            repCargaPedido.Atualizar(cp);
                        }
                    }
                    if (enviouTodas)
                    {
                        carga.DataRecebimentoUltimaNFe = DateTime.Now;
                        carga.DataEnvioUltimaNFe = DateTime.Now;
                        carga.DataInicioEmissaoDocumentos = DateTime.Now;
                    }
                }
            }
            bool notaFiscalEmOutraCarga = false;
            if (cargaPedido != null && cargaIntegracao.NotasFiscais != null && cargaIntegracao.NotasFiscais.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(cargaIntegracao.NotasFiscais.FirstOrDefault().Chave) || cargaIntegracao.NotasFiscais.FirstOrDefault().Modelo == "99")
                {
                    decimal pesoNaNFs = 0;
                    int volumes = 0;

                    bool verificarValorPorNota = false;
                    if (cargaPedido.ValorFrete <= 0)
                        verificarValorPorNota = true;

                    foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in cargaIntegracao.NotasFiscais)
                    {
                        if (!verificarValorPorNota)
                            notaFiscal.ValorFrete = 0;

                        if (notaFiscal.Emitente == null)
                            notaFiscal.Emitente = cargaIntegracao.Remetente;

                        if (notaFiscal.Destinatario == null)
                            notaFiscal.Destinatario = cargaIntegracao.Destinatario;

                        if (notaFiscal.Destinatario == null && notaFiscal.Emitente != null)
                            notaFiscal.Destinatario = notaFiscal.Emitente;

                        if (notaFiscal.Expedidor == null)
                        {
                            if (((cargaPedido.Carga.TipoOperacao?.UtilizarExpedidorComoTransportador ?? false) && cargaPedido.Carga.Empresa != null))
                                notaFiscal.Recebedor = Servicos.Embarcador.Pessoa.Pessoa.Converter(cargaPedido.Carga.Empresa);
                        }

                        notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                        string mensagem = serCargaNotaFiscal.InformarDadosNotaCarga(notaFiscal, cargaPedido, tipoServicoMultisoftware, configuracao, Auditado, out bool alteradoTipoDeCarga, true, true, true);

                        pesoNaNFs += notaFiscal.PesoBruto;
                        volumes += (int)notaFiscal.VolumesTotal;

                        if (!string.IsNullOrWhiteSpace(mensagem))
                        {
                            stMensagem.Append(mensagem);
                            return;
                        }
                    }

                    //if (cargaPedido.ValorFreteAPagar > 0)
                    //    Servicos.Embarcador.Carga.FreteFilialEmissora.SetarFreteEmbarcadorFilialEmissora(ref carga, cargaPedido, tipoServicoMultisoftware, true, unitOfWork);

                    if (cargaPedido.CargaPedidoTrechoAnterior != null && cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoFilialEmissora &&
                                 (cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                                 || cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                                 || cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                                 || cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                                 || cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento))
                    {
                        Servicos.Embarcador.Carga.FilialEmissora serFilialEmissora = new Embarcador.Carga.FilialEmissora();
                        serFilialEmissora.GerarCTesAnterioresDaFilialEmissoraRedespacho(cargaPedido.CargaPedidoTrechoAnterior, cargaPedido, tipoServicoMultisoftware, configuracao, unitOfWork);
                    }

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && verificarValorPorNota)
                    {
                        if (repPedidoXMLNotaFiscal.BuscarValorTotalFretePorCargaPedido(cargaPedido.Codigo) > 0)
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
                            decimal valorICMSIncluso = 0;
                            Servicos.Log.TratarErro($"3 Notas para gerar Documento Provisão {string.Join(", ", pedidoXMLNotasFiscais?.Select(x => x.Codigo)?.ToList())}", "GeracaoDocumentosProvisao");
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                            {
                                decimal valorFrete = pedidoXMLNotaFiscal.XMLNotaFiscal.ValorFrete;
                                //if (cargaPedido.Expedidor != null && cargaPedido.CargaPedidoTrechoAnterior == null)
                                //    valorFrete = pedidoXMLNotaFiscal.XMLNotaFiscal.ValorFreteRedespacho;

                                pedidoXMLNotaFiscal.ValorFrete = valorFrete;
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalCompontesFrete = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
                                serRateioNotaFiscal.CalcularImpostos(cargaPedido, cargaPedido.Carga, false, pedidoXMLNotaFiscal, valorFrete, cargaPedido.IncluirICMSBaseCalculo, cargaPedido.PercentualIncluirBaseCalculo, pedidoXMLNotaFiscalCompontesFrete, tipoServicoMultisoftware, null, null, unitOfWork, configuracao);

                                Servicos.Embarcador.Carga.FreteFilialEmissora.SetarFreteEmbarcadorNotaFiscalFilialEmissora(ref cargaPedido, pedidoXMLNotaFiscal, tipoServicoMultisoftware, unitOfWork, configuracao);

                                repPedidoXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal);

                                if (valorFrete > 0)
                                {
                                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(pedidoXMLNotaFiscal, false, tipoServicoMultisoftware, unitOfWork);

                                    cargaPedido.ValorFrete += valorFrete;
                                    cargaPedido.ValorFreteAPagar += valorFrete;
                                    cargaPedido.ValorICMS += pedidoXMLNotaFiscal.ValorICMS;
                                    cargaPedido.ValorICMSIncluso += pedidoXMLNotaFiscal.ValorICMSIncluso;
                                    valorICMSIncluso += pedidoXMLNotaFiscal.ValorICMSIncluso;
                                    cargaPedido.ValorISS += pedidoXMLNotaFiscal.ValorISS;
                                    cargaPedido.BaseCalculoICMS += pedidoXMLNotaFiscal.BaseCalculoICMS;
                                    cargaPedido.BaseCalculoISS += pedidoXMLNotaFiscal.BaseCalculoISS;
                                    cargaPedido.PercentualAliquota = pedidoXMLNotaFiscal.PercentualAliquota;
                                    cargaPedido.PercentualAliquotaISS = pedidoXMLNotaFiscal.PercentualAliquotaISS;
                                    cargaPedido.PercentualReducaoBC = pedidoXMLNotaFiscal.PercentualReducaoBC;
                                    cargaPedido.PercentualRetencaoISS = pedidoXMLNotaFiscal.PercentualRetencaoISS;
                                    cargaPedido.DescontarICMSDoValorAReceber = pedidoXMLNotaFiscal.DescontarICMSDoValorAReceber;

                                    Servicos.Log.TratarErro($"13 - CargaPedido: {cargaPedido.Codigo} -> Aliquota: {cargaPedido.PercentualAliquota}", "ProcessarAliquota");

                                    cargaPedido.Carga.ValorFrete += valorFrete;
                                    cargaPedido.Carga.ValorFreteAPagar += valorFrete;
                                    cargaPedido.Carga.ValorICMS += pedidoXMLNotaFiscal.ValorICMS;
                                    cargaPedido.Carga.ValorISS += pedidoXMLNotaFiscal.ValorISS;
                                    cargaPedido.Carga.ValorRetencaoISS += pedidoXMLNotaFiscal.ValorRetencaoISS;
                                }
                            }

                            if (cargaPedido.Carga.ValorFrete > 0)
                            {
                                Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Embarcador.Carga.RateioFrete(unitOfWork);
                                serRateioFrete.GerarComponenteICMS(cargaPedido, false, unitOfWork);
                                serRateioFrete.GerarComponenteISS(cargaPedido, false, unitOfWork);

                                carga.ValorFreteLiquido = Math.Round(cargaPedido.Carga.ValorFrete, 2, MidpointRounding.AwayFromZero);
                                carga.ValorFrete = Math.Round(cargaPedido.Carga.ValorFrete, 2, MidpointRounding.AwayFromZero);
                                carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                                //serCarga.InformarSituacaoCargaFreteValido(ref carga, tipoServicoMultisoftware, unitOfWork);
                                repCarga.Atualizar(carga);

                                serRateioFrete.GerarComponenteICMS(carga, carga.ValorICMS, false, unitOfWork, valorICMSIncluso);
                                serRateioFrete.GerarComponenteISS(carga, carga.ValorISS, unitOfWork);
                                serRateioFrete.GerarComponentePisCofins(carga, carga.ValorPis + carga.ValorCofins, unitOfWork);

                                if (cargaPedido.CargaPedidoFilialEmissora)
                                {
                                    serRateioFrete.GerarComponenteICMS(cargaPedido, true, unitOfWork);

                                    carga.ValorFreteFilialEmissora = Math.Round(cargaPedido.Carga.ValorFreteFilialEmissora, 2, MidpointRounding.AwayFromZero);

                                    serRateioFrete.GerarComponenteICMS(carga, carga.ValorICMSFilialEmissora, true, unitOfWork, valorICMSIncluso);

                                }

                                if (cargaPedido.RegraTomador == null)
                                    cargaPedido.Pedido.UsarTipoPagamentoNF = true;

                                repPedido.Atualizar(cargaPedido.Pedido);
                            }
                        }
                    }
                    //else
                    //{
                    //    serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga,  false, tipoServicoMultisoftware, unitOfWork);
                    //    if (cargaPedido.CargaPedidoFilialEmissora)
                    //        serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(carga, true, tipoServicoMultisoftware, unitOfWork);
                    //    else
                    //    {
                    //        if (cargaPedido.CargaPedidoTrechoAnterior != null && cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoFilialEmissora && !cargaPedido.CargaPedidoTrechoAnterior.AgValorRedespacho)
                    //        {
                    //            serRateioNotaFiscal.RatearFreteCargaPedidoEntreNotas(cargaPedido.CargaPedidoTrechoAnterior.Carga, true, tipoServicoMultisoftware, unitOfWork);
                    //        }
                    //    }
                    //}
                    serCargaPedido.CriarUnidadesDeMedidaDaCargaPedido(cargaPedido, pesoNaNFs, volumes, unitOfWork);
                    cargaPedido.CienciaDoEnvioDaNotaInformado = true;
                    cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;
                }
                else
                {
                    bool encontrouTodas = true;
                    foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in cargaIntegracao.NotasFiscais)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial();
                        Embarcador.Pedido.NotaFiscal svcNotaFiscal = new Embarcador.Pedido.NotaFiscal(unitOfWork);

                        cargaPedidoXMLNotaFiscalParcial.CargaPedido = cargaPedido;
                        cargaPedidoXMLNotaFiscalParcial.NotaEnviadaIntegralmente = false;
                        cargaPedidoXMLNotaFiscalParcial.Numero = notaFiscal.Numero;
                        cargaPedidoXMLNotaFiscalParcial.Pedido = notaFiscal.NumeroDT;
                        repCargaPedidoXMLNotaFiscalParcial.Inserir(cargaPedidoXMLNotaFiscalParcial);

                        Servicos.Log.TratarErro($"5 Adicionando Pedidos Parciais {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")} Pedido: [{cargaPedido.Pedido.Codigo}]", "RequestLog");
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorNumeroOuNumeroPedidoSemCarga(notaFiscal.Numero, notaFiscal.NumeroDT, cargaPedido.Pedido.Remetente.CPF_CNPJ);

                        //if (xmlNotaFiscal != null && xmlNotaFiscal.CargaPedidoXMLNotaFiscalParcial == null)
                        if (xmlNotaFiscal != null)
                        {
                            cargaPedidoXMLNotaFiscalParcial.XMLNotaFiscal = xmlNotaFiscal;
                            xmlNotaFiscal.TipoNotaFiscalIntegrada = cargaPedidoXMLNotaFiscalParcial.TipoNotaFiscalIntegrada;
                            xmlNotaFiscal.MetrosCubicos = notaFiscal.MetroCubico;

                            repCargaPedidoXMLNotaFiscalParcial.Atualizar(cargaPedidoXMLNotaFiscalParcial);
                            repXMLNotaFiscal.Atualizar(xmlNotaFiscal);

                            bool msgAlertaObservacao = false;
                            string retorno = serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);
                            if (msgAlertaObservacao && !string.IsNullOrWhiteSpace(retorno))
                                retorno = "";

                            if (string.IsNullOrWhiteSpace(retorno))
                            {

                                svcNotaFiscal.PreencherDadosContabeisXMLNotaFiscal(xmlNotaFiscal, notaFiscal);
                                serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracao, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga, Auditado);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, null, "Adicionado via Integração de Carga", unitOfWork);
                            }
                            else
                            {
                                stMensagem.Append(retorno);
                                break;
                            }
                        }
                        else
                        {
                            encontrouTodas = false;
                        }
                    }
                    if (encontrouTodas)
                        cargaPedido.CienciaDoEnvioDaNotaInformado = true;
                }

                if (cargaIntegracao.FecharCargaAutomaticamente)
                {
                    Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);
                    serCarga.ValidarEnvioNotas(carga, unitOfWork);

                }


            }
            else if (!atualizar)
            {
                if ((carga.TipoOperacao?.DeslocamentoVazio ?? false) && cargaPedido.Pedido.Remetente != null && cargaPedido.Pedido.Destinatario != null && !repPedidoXMLNotaFiscal.VerificarSeExistePorCargaPedido(cargaPedido.Codigo))
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
                    {
                        CNPJTranposrtador = "",
                        DataEmissao = DateTime.Now,
                        Descricao = "Outros",
                        Destinatario = cargaPedido.Pedido.Destinatario,
                        Emitente = cargaPedido.Pedido.Remetente,
                        Modelo = "99",
                        nfAtiva = true,
                        Numero = 1,
                        PlacaVeiculoNotaFiscal = "",
                        Serie = "",
                        TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros,
                        TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida,
                        XML = "",
                        DataRecebimento = DateTime.Now,
                        Peso = 1m,
                        Valor = 1m
                    };

                    repXMLNotaFiscal.Inserir(xmlNotaFiscal);

                    serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracao, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, null, "Adicionado via Integração de Carga", unitOfWork);
                }
            }
        }

        public void AdicionarProdutosCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, bool usarPesoProduto)
        {
            AdicionarProdutosCarga(cargaPedido, cargaIntegracao.Produtos, ref stMensagem, unitOfWork, usarPesoProduto);
        }

        public void AdicionarProdutosCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosIntegracao, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, bool usarPesoProduto)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

            int contagemCargaPedidoProduto = repCargaPedidoProduto.ContarPorCargaPedido(cargaPedido.Codigo);
            if (contagemCargaPedidoProduto <= 0 || !cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete)
            {
                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaPedidoProduto = repPedidoProduto.BuscarPorPedido(cargaPedido.Pedido.Codigo);

                Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
                serCargaPedido.AdicionarProdutosCargaPedido(cargaPedido, listaPedidoProduto, usarPesoProduto, unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = null;
                if (cargaPedido.Carga.VeiculosVinculados?.Count > 0)
                    modeloVeicularCarga = cargaPedido.Carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;

                if (modeloVeicularCarga == null)
                    modeloVeicularCarga = cargaPedido.Carga.Veiculo?.ModeloVeicularCarga;

                if (produtosIntegracao?.Count > 0 && modeloVeicularCarga != null)
                    serCargaPedido.AdicionarDivisoesCapacidadeCargaPedido(cargaPedido, modeloVeicularCarga, produtosIntegracao, ref stMensagem, unitOfWork);
            }
        }

        public Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoCarga ConverterParaObjetoEnvioCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(_unitOfWork);

            Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoCarga envioCancelamentoCarga = new Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoCarga()
            {
                CancelarDocumentosEmitidosNoEmbarcador = cargaCargaIntegracao.CargaCancelamento.CancelarDocumentosEmitidosNoEmbarcador,
                DataCancelamento = cargaCargaIntegracao.CargaCancelamento.DataCancelamento,
                DuplicarCarga = cargaCargaIntegracao.CargaCancelamento.DuplicarCarga,
                EnviouAverbacoesCTesParaCancelamento = cargaCargaIntegracao.CargaCancelamento.EnviouAverbacoesCTesParaCancelamento,
                Justificativa = cargaCargaIntegracao.CargaCancelamento.Justificativa != null ? new Dominio.ObjetosDeValor.Embarcador.Financeiro.Justificativa() { Descricao = cargaCargaIntegracao.CargaCancelamento.Justificativa.Descricao } : null,
                JustificativaCancelamento = cargaCargaIntegracao.CargaCancelamento.JustificativaCancelamentoCarga != null ? new Dominio.ObjetosDeValor.WebService.CargaCancelamento.JustificativaCancelamento { Descricao = cargaCargaIntegracao.CargaCancelamento.JustificativaCancelamentoCarga.Descricao } : null,
                LiberarPedidosParaMontagemCarga = cargaCargaIntegracao.CargaCancelamento.LiberarPedidosParaMontagemCarga,
                MotivoCancelamento = cargaCargaIntegracao.CargaCancelamento.MotivoCancelamento,
                OperadorResponsavel = cargaCargaIntegracao.CargaCancelamento.OperadorResponsavel?.Login ?? "",
                ProtocoloCarga = cargaCargaIntegracao.CargaCancelamento.Carga.CargaProtocoloIntegrada,
                Situacao = cargaCargaIntegracao.CargaCancelamento.Situacao,
                SituacaoCargaNoCancelamento = cargaCargaIntegracao.CargaCancelamento.SituacaoCargaNoCancelamento,
                Tipo = cargaCargaIntegracao.CargaCancelamento.Tipo,
                Usuario = cargaCargaIntegracao.CargaCancelamento.Usuario?.Login,
                CTes = new List<Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoCTe>(),
                AverbacaoCTes = new List<Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoAverbacaoCTe>()
            };

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCTe.BuscarCTePorCarga(cargaCargaIntegracao.CargaCancelamento.Carga.Codigo);
            if (ctes != null && ctes.Count > 0)
            {
                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                {
                    envioCancelamentoCarga.CTes.Add(new Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoCTe()
                    {
                        Cancelado = cte.Cancelado,
                        ChaveCTe = cte.Chave,
                        DataAnulacao = cte.DataAnulacao,
                        DataCancelamento = cte.DataCancelamento,
                        DataRetornoSefaz = cte.DataRetornoSefaz,
                        MensagemRetornoSefaz = cte.MensagemRetornoSefaz,
                        ObservacaoCancelamento = cte.ObservacaoCancelamento,
                        ProtocoloCancelamentoInutilizacao = cte.ProtocoloCancelamentoInutilizacao,
                        Status = cte.Status
                    });
                }
            }

            List<Dominio.Entidades.AverbacaoCTe> averbacoes = repAverbacaoCTe.BuscarPorCarga(cargaCargaIntegracao.CargaCancelamento.Carga.Codigo);
            if (averbacoes != null && averbacoes.Count > 0)
            {
                foreach (Dominio.Entidades.AverbacaoCTe averbacao in averbacoes)
                {
                    envioCancelamentoCarga.AverbacaoCTes.Add(new Dominio.ObjetosDeValor.WebService.CargaCancelamento.EnvioCancelamentoAverbacaoCTe()
                    {
                        ChaveCTe = averbacao.CTe.Chave,
                        CodigoIntegracao = averbacao.CodigoIntegracao,
                        CodigoRetorno = averbacao.CodigoRetorno,
                        DataRetorno = averbacao.DataRetorno,
                        MensagemRetorno = averbacao.MensagemRetorno,
                        Protocolo = averbacao.Protocolo,
                        Status = averbacao.Status,
                        Tipo = averbacao.Tipo
                    });
                }
            }

            return envioCancelamentoCarga;
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.Porto ConverterObjetoPorto(Dominio.Entidades.Embarcador.Pedidos.Porto porto)
        {
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(_unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(_unitOfWork);

            if (porto != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.Porto dynPorto = new Dominio.ObjetosDeValor.Embarcador.Carga.Porto()
                {
                    Codigo = porto.Codigo,
                    CodigoIntegracao = porto.CodigoIntegracao,
                    CodigoDocumento = porto.CodigoDocumento,
                    CodigoIATA = porto.CodigoIATA,
                    Descricao = porto.Descricao,
                    Localidade = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
                    {
                        Cidade = serLocalidade.ConverterObjetoLocalidade(porto.Localidade)
                    },
                    Empresa = porto.Empresa != null ? serWSEmpresa.ConverterObjetoEmpresa(porto.Empresa) : null,
                    InativarCadastro = porto.Ativo ? false : true,
                    CodigoMercante = porto.CodigoMercante,
                    QuantidadeHorasFaturamentoAutomatico = porto.QuantidadeHorasFaturamentoAutomatico,
                    AtivarDespachanteComoConsignatario = porto.AtivarDespachanteComoConsignatario,
                    DiasAntesDoPodParaEnvioDaDocumentacao = porto?.DiasAntesDoPodParaEnvioDaDocumentacao,
                    RKST = porto?.RKST,
                    Atualizar = false
                };
                return dynPorto;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto ConverterObjetoTerminalPorto(Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(_unitOfWork);

            if (terminal != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto dynTerminalPorto = new Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto()
                {
                    Codigo = terminal.Codigo,
                    CodigoIntegracao = terminal.CodigoIntegracao,
                    CodigoTerminal = terminal.CodigoTerminal,
                    Descricao = terminal.Descricao,
                    Porto = ConverterObjetoPorto(terminal.Porto),
                    Terminal = serPessoa.ConverterObjetoPessoa(terminal.Terminal),
                    CodigoDocumento = terminal.CodigoDocumento,
                    InativarCadastro = !terminal.Ativo ? true : false,
                    Atualizar = false,
                    CodigoMercante = terminal.CodigoMercante,
                    CodigoObservacaoContribuinte = terminal.CodigoObservacaoContribuinte,
                    QuantidadeDiasEnvioDocumentacao = terminal.QuantidadeDiasEnvioDocumentacao
                };
                return dynTerminalPorto;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.Container ConverterObjetoContainer(Dominio.Entidades.Embarcador.Pedidos.Container container)
        {
            if (container != null)
            {
                int.TryParse(container.CodigoIntegracao, out int codigoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Carga.Container dynContainer = new Dominio.ObjetosDeValor.Embarcador.Carga.Container()
                {
                    Codigo = container.Codigo,
                    CodigoIntegracao = codigoIntegracao,
                    Descricao = container.Descricao,
                    Numero = container.Numero,
                    PesoLiquido = container.PesoLiquido,
                    Tara = (int)container.Tara,
                    TipoPropriedade = container.TipoPropriedade,
                    TipoContainer = ConverterObjetoTipoContainer(container.ContainerTipo),
                    InativarCadastro = !container.Status ? true : false,
                    DencidadeProduto = container.MetrosCubicos
                };
                return dynContainer;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer ConverterObjetoTipoContainer(Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer)
        {
            if (tipoContainer != null)
            {
                int.TryParse(tipoContainer.CodigoIntegracao, out int codigoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer dynTipoContainer = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer()
                {
                    Codigo = tipoContainer.Codigo,
                    CodigoIntegracao = codigoIntegracao,
                    CodigoDocumento = tipoContainer.CodigoDocumento,
                    Descricao = tipoContainer.Descricao,
                    Valor = tipoContainer.Valor,
                    InativarCadastro = tipoContainer.Status ? false : true,
                    Atualizar = false,
                    MetrosCubicos = tipoContainer.MetrosCubicos,
                    Tara = tipoContainer.Tara,
                    PesoLiquido = tipoContainer.PesoLiquido,
                    TEU = tipoContainer.TEU,
                    FFE = tipoContainer.FFE,
                    PesoMaximo = tipoContainer.PesoMaximo
                };
                return dynTipoContainer;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.EmpresaResponsavel ConverterObjetoEmpresaResponsavel(Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel empresaResponsavel)
        {
            if (empresaResponsavel != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.EmpresaResponsavel dynEmpresaResponsavel = new Dominio.ObjetosDeValor.Embarcador.Carga.EmpresaResponsavel()
                {
                    CodigoIntegracao = empresaResponsavel.CodigoIntegracao,
                    Descricao = empresaResponsavel.Descricao
                };
                return dynEmpresaResponsavel;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.CentroCusto ConverterObjetoCentroCustol(Dominio.Entidades.Embarcador.Pedidos.PedidoCentroCusto centroCusto)
        {
            if (centroCusto != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.CentroCusto dyncentroCusto = new Dominio.ObjetosDeValor.Embarcador.Carga.CentroCusto()
                {
                    CodigoIntegracao = centroCusto.CodigoIntegracao,
                    Descricao = centroCusto.Descricao
                };
                return dyncentroCusto;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.PropostaComercial ConverterObjetoPropostaComercial(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            int.TryParse(pedido.CodigoProposta, out int codigoProposta);
            if (codigoProposta > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.PropostaComercial dynPropostaComercial = new Dominio.ObjetosDeValor.Embarcador.Carga.PropostaComercial()
                {
                    CodigoIntegracao = codigoProposta,
                    Descricao = pedido.NumeroProposta
                };
                return dynPropostaComercial;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.Navio ConverterObjetoNavio(Dominio.Entidades.Embarcador.Pedidos.Navio navio)
        {
            if (navio != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.Navio dynNavio = new Dominio.ObjetosDeValor.Embarcador.Carga.Navio()
                {
                    Codigo = navio.Codigo,
                    CodigoIntegracao = navio.CodigoIntegracao,
                    CodigoDocumento = navio.CodigoDocumento,
                    CodigoEmbarcacao = navio.CodigoEmbarcacao,
                    CodigoIRIN = navio.Irin,
                    Descricao = navio.Descricao,
                    TipoEmbarcacao = navio.TipoEmbarcacao,
                    Atualizar = false,
                    CodigoIMO = navio.CodigoIMO,
                    InativarCadastro = !navio.Status ? true : false
                };
                return dynNavio;
            }
            else
            {
                return null;
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo> ConverterObjetoTransbordo(List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> transbordo)
        {
            if (transbordo != null && transbordo.Count > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo> bynTransbordo = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo>();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo transb in transbordo)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo dynTransb = new Dominio.ObjetosDeValor.Embarcador.Carga.Transbordo()
                    {
                        CodigoIntegracao = transb.Codigo,
                        Navio = ConverterObjetoNavio(transb.PedidoViagemNavio?.Navio ?? null),
                        Porto = ConverterObjetoPorto(transb.Porto),
                        Sequencia = transb.Sequencia,
                        Terminal = ConverterObjetoTerminalPorto(transb.Terminal),
                        Viagem = ConverterObjetoViagem(transb.PedidoViagemNavio)
                    };
                    bynTransbordo.Add(dynTransb);
                }
                return bynTransbordo;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.Viagem ConverterObjetoViagem(Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem)
        {
            if (viagem != null)
            {
                int.TryParse(viagem.CodigoIntegracao, out int codigoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Carga.Viagem dynViagem = new Dominio.ObjetosDeValor.Embarcador.Carga.Viagem()
                {
                    Codigo = viagem.Codigo,
                    CodigoIntegracao = codigoIntegracao,
                    Descricao = viagem.Descricao,
                    Direcao = viagem.DirecaoViagemMultimodal,
                    Navio = ConverterObjetoNavio(viagem.Navio),
                    NumeroViagem = viagem.NumeroViagem,
                    Atualizar = false,
                    InativarCadastro = viagem.Status ? false : true
                };
                dynViagem.Schedules = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Schedule>();
                if (viagem.Schedules != null && dynViagem.Schedules.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule schudule in viagem.Schedules)
                        dynViagem.Schedules.Add(ConverterObjetoSchedule(schudule));
                }
                return dynViagem;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.Schedule ConverterObjetoSchedule(Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule schedule)
        {
            if (schedule != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.Schedule dynSchedule = new Dominio.ObjetosDeValor.Embarcador.Carga.Schedule()
                {
                    PortoAtracacao = ConverterObjetoPorto(schedule.PortoAtracacao),
                    TerminalAtracacao = ConverterObjetoTerminalPorto(schedule.TerminalAtracacao),
                    DataDeadLine = schedule.DataDeadLine.HasValue ? schedule.DataDeadLine.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    DataPrevisaoChegadaNavio = schedule.DataPrevisaoChegadaNavio.HasValue ? schedule.DataPrevisaoChegadaNavio.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    DataPrevisaoSaidaNavio = schedule.DataPrevisaoSaidaNavio.HasValue ? schedule.DataPrevisaoSaidaNavio.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    ETAConfirmado = schedule.ETAConfirmado,
                    ETSConfirmado = schedule.ETSConfirmado
                };
                return dynSchedule;
            }
            else
            {
                return null;
            }
        }

        public void PreecherDadosPreCarga(Dominio.Entidades.Embarcador.Cargas.Carga preCarga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            int codigoVeiculoAnterior = preCarga.Veiculo?.Codigo ?? 0;

            PreecherDadosEmpresa(preCarga, cargaIntegracao, pedido, ref stMensagem, unitOfWork);
            PreecherDadosVeiculo(preCarga, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, configuracaoEmbarcador, unitOfWork, auditado);

            int codigoVeiculoPosterior = preCarga.Veiculo?.Codigo ?? 0;

            if (codigoVeiculoAnterior != codigoVeiculoPosterior && codigoVeiculoPosterior != 0)
                Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoriaPorCarga(preCarga, configuracaoEmbarcador, auditado, "Preenchidos dados da pré-carga", unitOfWork);

            PreecherDadosMotorista(preCarga, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, configuracaoEmbarcador, unitOfWork, auditado, clienteAcesso: null, adminStringConexao: string.Empty);
            PreecherDadosTipoOperacao(preCarga, cargaIntegracao, pedido, tipoOperacao, ref stMensagem, tipoServicoMultisoftware, configuracaoEmbarcador, unitOfWork, auditado);
        }

        public bool PreencherCargaPedidoOrigemDestino(ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

            bool mudouLocalidade = false;
            Servicos.Cliente servicoCliente = new Servicos.Cliente(StringConexao);
            Dominio.Entidades.Localidade origem = pedido.Origem;
            Dominio.Entidades.Localidade destino = pedido.Destino;

            if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor ?? false) && pedido.Tomador != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco endereco = repClienteOutroEndereco.BuscarPorPessoa(pedido.Recebedor.CPF_CNPJ)?.FirstOrDefault();

                if (endereco != null)
                    destino = endereco.Localidade;
            }

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();

            if (cargaIntegracao.Tomador != null && (!string.IsNullOrEmpty(cargaIntegracao.Tomador.CPFCNPJ) || !string.IsNullOrEmpty(cargaIntegracao.Tomador.CodigoIntegracao)))
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoTomador = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.Tomador, "Tomador", unitOfWork, 0, false);
                if (retornoTomador.Status == false)
                    stMensagem.Append(retornoTomador.Mensagem);
                else
                    cargaPedido.Tomador = retornoTomador.cliente;
            }
            else
                cargaPedido.Tomador = null;

            if (cargaIntegracao.Recebedor != null && (!string.IsNullOrEmpty(cargaIntegracao.Recebedor.CPFCNPJ) || !string.IsNullOrEmpty(cargaIntegracao.Recebedor.CodigoIntegracao)))
            {
                if (!string.IsNullOrWhiteSpace(cargaIntegracao.Recebedor.CPFCNPJ) || cargaIntegracao.Recebedor.ClienteExterior || !string.IsNullOrEmpty(cargaIntegracao.Recebedor.CodigoIntegracao))// regra criada para danone, quando importar o arquivo e o recebedo cnpj for vazio, vai aguardar o usuário informar manualmente.
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.Recebedor, "Recebedor", unitOfWork, 0, false);

                    if (retorno.Status == false)
                        stMensagem.Append(retorno.Mensagem);
                    else
                    {
                        cargaPedido.Recebedor = retorno.cliente;
                        if (!(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                            destino = cargaPedido.Recebedor.Localidade;
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                    }
                }
                else
                {
                    if (cargaIntegracao.Distribuicoes != null && cargaIntegracao.Distribuicoes.Count > 0)
                    {
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                        cargaPedido.AgInformarRecebedor = true;
                        cargaPedido.Destino = null;
                    }
                    else
                    {
                        stMensagem.Append("É obrigatório informar o CNPJ/CPF do recebedor.");
                    }
                }
            }
            else
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (pedido.Recebedor != null)
                    {
                        cargaPedido.Recebedor = pedido.Recebedor;
                        if (!(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                            destino = cargaPedido.Recebedor.Localidade;
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                    }
                    else
                        cargaPedido.Recebedor = null;
                }
                else
                    cargaPedido.Recebedor = null;
            }

            if ((cargaPedido.Carga.TipoOperacao?.UsarRecebedorComoPontoPartidaCarga ?? false) && cargaPedido.Recebedor != null)
            {
                cargaPedido.PontoPartida = cargaPedido.Recebedor;
                cargaPedido.PossuiColetaEquipamentoPontoPartida = true;
            }
            else
            {
                cargaPedido.PontoPartida = pedido.PontoPartida;
                cargaPedido.PossuiColetaEquipamentoPontoPartida = false;
            }

            if (cargaPedido.Recebedor == null && !cargaPedido.AgInformarRecebedor && cargaPedido.Pedido != null && cargaPedido.Pedido.Destinatario != null)
            {

                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoa(cargaPedido.Pedido.Destinatario.CPF_CNPJ);
                if (clienteDescarga != null && clienteDescarga.Distribuidor != null)
                {
                    double cnpj = 0;
                    double.TryParse(clienteDescarga.Distribuidor.CNPJ_SemFormato, out cnpj);
                    cargaPedido.Recebedor = repCliente.BuscarPorCPFCNPJ(cnpj);
                    if (cargaPedido.Recebedor != null)
                    {
                        destino = cargaPedido.Recebedor.Localidade;
                        cargaPedido.PendenteGerarCargaDistribuidor = true;
                        cargaPedido.Carga.PendenteGerarCargaDistribuidor = true;
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                        repCarga.Atualizar(cargaPedido.Carga);
                    }
                }
            }

            if (cargaIntegracao.Expedidor != null && (!string.IsNullOrEmpty(cargaIntegracao.Expedidor.CPFCNPJ) || !string.IsNullOrEmpty(cargaIntegracao.Expedidor.CodigoIntegracao)))
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = servicoCliente.ConverterObjetoValorPessoa(cargaIntegracao.Expedidor, "Expedidor", unitOfWork, 0, false);
                if (retorno.Status == false)
                    stMensagem.Append(retorno.Mensagem);
                else
                {
                    cargaPedido.Expedidor = retorno.cliente;
                    origem = cargaPedido.Expedidor.Localidade;
                    cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;

                    if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarExpedidorComoTransportador && cargaPedido.Carga.Empresa == null)
                        cargaPedido.Carga.Empresa = repEmpresa.BuscarPorCNPJ(cargaPedido.Expedidor.CPF_CNPJ_SemFormato);
                }
            }
            else
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (pedido.Expedidor != null)
                    {
                        cargaPedido.Expedidor = pedido.Expedidor;
                        origem = cargaPedido.Expedidor.Localidade;
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
                    }
                    else
                        cargaPedido.Expedidor = null;
                }
                else
                    cargaPedido.Expedidor = null;
            }

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarExpedidorComoTransportador && cargaPedido.Expedidor == null && cargaPedido.Carga.Empresa != null)
            {
                cargaPedido.Expedidor = repCliente.BuscarPorCPFCNPJ(double.Parse(cargaPedido.Carga.Empresa.CNPJ_SemFormato));

                if (cargaPedido.Expedidor == null)
                {
                    Servicos.Cliente serCliente = new Cliente(unitOfWork);
                    Servicos.WebService.Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoaExp = serPessoa.ConverterObjetoEmpresa(cargaPedido.Carga.Empresa);
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(pessoaExp, "Expedidor", unitOfWork, 0, false);
                    if (retorno.Status == true)
                        cargaPedido.Expedidor = retorno.cliente;
                }

                if (cargaPedido.Expedidor != null)
                {
                    cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
                    origem = cargaPedido.Expedidor.Localidade;
                }
            }

            if (cargaPedido.Expedidor == null && cargaPedido.Carga.Rota != null && cargaPedido.Carga.Rota.ExpedidorPedidosDiferenteOrigemRota != null)
            {
                if (cargaPedido.Carga.Rota.LocalidadesOrigem?.Count() == 1 && pedido.Origem?.Codigo != cargaPedido.Carga.Rota.LocalidadesOrigem.FirstOrDefault().Codigo)
                {
                    cargaPedido.Expedidor = cargaPedido.Carga.Rota.ExpedidorPedidosDiferenteOrigemRota;
                    cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
                    origem = cargaPedido.Carga.Rota.ExpedidorPedidosDiferenteOrigemRota.Localidade;
                }
            }

            if (cargaPedido.Expedidor == null && cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.Expedidor != null)
            {
                cargaPedido.Expedidor = cargaPedido.Carga.TipoOperacao.Expedidor;
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
                origem = cargaPedido.Expedidor.Localidade;
            }

            if (cargaPedido.Recebedor == null && cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.Recebedor != null)
            {
                cargaPedido.Recebedor = cargaPedido.Carga.TipoOperacao.Recebedor;
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                destino = cargaPedido.Recebedor.Localidade;
            }

            //#29721
            if (cargaPedido.Recebedor == null && cargaPedido.Carga.Rota != null && cargaPedido.Carga.Rota.Distribuidor != null && cargaPedido.Carga.Rota.GerarRedespachoAutomaticamente)
            {
                cargaPedido.Recebedor = cargaPedido.Carga.Rota.Distribuidor;
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                destino = cargaPedido.Recebedor.Localidade;
            }

            if (origem != null)
            {
                if (cargaPedido.Origem == null || (cargaPedido.Origem.Codigo != origem.Codigo))
                {
                    cargaPedido.Origem = origem;
                    mudouLocalidade = true;
                }
            }

            if (destino != null)
            {
                if ((cargaPedido.Destino == null || (cargaPedido.Destino.Codigo != destino.Codigo)) && !cargaPedido.AgInformarRecebedor)
                {
                    cargaPedido.Destino = destino;
                    mudouLocalidade = true;
                }
            }

            if (cargaPedido.Recebedor != null && cargaPedido.Expedidor != null)
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;

            cargaPedido.TipoTomador = cargaIntegracao.TipoTomador;
            if (cargaIntegracao.UtilizarTipoTomadorInformado || configuracaoWebService.SempreUtilizarTomadorEnviadoNoPedido)
            {
                cargaPedido.Pedido.UsarTipoTomadorPedido = true;
                repPedido.Atualizar(cargaPedido.Pedido);
            }

            if (!cargaPedido.Pedido.UsarTipoTomadorPedido)
            {
                Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = Servicos.Embarcador.Pedido.RegraTomador.BuscarRegraTomador(cargaPedido, tipoServicoMultisoftware, unitOfWork);

                if (regraTomador != null)
                {
                    cargaPedido.TipoTomador = regraTomador.TipoTomador;
                    cargaPedido.Tomador = regraTomador.Tomador;
                    cargaPedido.RegraTomador = regraTomador;
                    pedido.UsarTipoPagamentoNF = false;

                    if (regraTomador.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                    else if (regraTomador.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                    else
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

                    repPedido.Atualizar(pedido);
                }
                else
                    cargaPedido.RegraTomador = null;
            }
            else
            {
                cargaPedido.TipoTomador = cargaPedido.Pedido.TipoTomador;
                if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    cargaPedido.Tomador = cargaPedido.Pedido.Tomador;
                else
                    cargaPedido.Tomador = null;
            }

            return mudouLocalidade;
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga GerarCargaPorListaCargaIntegracao(List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> listaCargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Carga servicoCargaWS = new Carga(unitOfWork);
                Pedido servicoPedidoWS = new Pedido(unitOfWork);
                Servicos.WebService.Empresa.Empresa servicoEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;


                StringBuilder mensagemErro = new StringBuilder();

                Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracaoValidar = listaCargaIntegracao.FirstOrDefault();

                unitOfWork.Start();

                if (cargaIntegracaoValidar != null)
                {
                    if (cargaIntegracaoValidar.TransportadoraEmitente != null && !string.IsNullOrWhiteSpace(cargaIntegracaoValidar.TransportadoraEmitente.CNPJ))
                    {
                        Dominio.Entidades.Empresa transportadora = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cargaIntegracaoValidar.TransportadoraEmitente.CNPJ));
                        if (transportadora != null && transportadora.RecusarIntegracaoPODUnilever)
                        {
                            unitOfWork.Rollback();
                            stMensagem.Append($"Transportadora {transportadora.CNPJ} configurada para não aceitar integração de POD.");
                            return null;
                        }
                    }

                    string identificadorSegundario = string.Concat("00", cargaIntegracaoValidar.NumeroCarga);
                    carga = repCarga.BuscarPrimeiraCargaPorCodigoCargaEmbarcadorTodasSituacoes(cargaIntegracaoValidar.NumeroCarga);
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaSecundaria = repCarga.BuscarPrimeiraCargaPorCodigoCargaEmbarcadorTodasSituacoes(identificadorSegundario);

                    if (cargaSecundaria != null && cargaSecundaria.SituacaoCarga != SituacaoCarga.Cancelada)
                    {
                        string mensagemCancelamento = string.Empty;
                        if (!servicoCargaWS.SolicitarCancelamentoCarga(cargaSecundaria, ref mensagemCancelamento, tipoServicoMultisoftware, unitOfWork))
                        {
                            unitOfWork.Rollback();
                            stMensagem.Append(mensagemCancelamento);
                            return null;
                        }
                        else if (carga != null && carga.SituacaoCarga == SituacaoCarga.Cancelada)
                        {
                            servicoCargaWS.RemoverRegistrosDeCancelamentoCarga(carga, unitOfWork);
                            servicoCargaWS.AtivarCargaCancelada(carga, unitOfWork);
                        }
                    }
                    else if (carga != null && carga.SituacaoCarga != SituacaoCarga.Cancelada)
                    {
                        string mensagemCancelamento = string.Empty;
                        if (!servicoCargaWS.SolicitarCancelamentoCarga(carga, ref mensagemCancelamento, tipoServicoMultisoftware, unitOfWork))
                        {
                            unitOfWork.Rollback();
                            stMensagem.Append(mensagemCancelamento);
                            return null;
                        }
                    }
                }

                foreach (Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao in listaCargaIntegracao)
                {
                    int protocoloCargaExistente = 0;
                    int protocoloPedidoExistente = 0;
                    int codigoPersonalizado = 0;
                    ValidarCamposIntegracaoCarga(cargaIntegracao, configuracaoTMS.ReplicarCadastroVeiculoIntegracaoTransportadorDiferente, false, configuracaoTMS.BuscarClientesCadastradosNaIntegracaoDaCarga, configuracaoTMS.UtilizarProdutosDiversosNaIntegracaoDaCarga, ref mensagemErro, unitOfWork, configuracaoTMS, tipoServicoMultisoftware, out codigoPersonalizado, out tipoOperacao, out filial);

                    if (mensagemErro.Length > 0)
                    {
                        unitOfWork.Rollback();
                        stMensagem.Append($"Carga {cargaIntegracao.NumeroCarga} Pedido {cargaIntegracao.NumeroPedidoEmbarcador} (validação): {mensagemErro}");
                        return null;
                    }

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref mensagemErro, tipoServicoMultisoftware, ref protocoloPedidoExistente, ref protocoloCargaExistente, false, null, configuracaoTMS, null, string.Empty, ignorarPedidosInseridosManualmente: true, true);

                    if (mensagemErro.Length == 0 || protocoloPedidoExistente > 0)
                    {
                        cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemErro, ref protocoloCargaExistente, unitOfWork, tipoServicoMultisoftware, false, false, null, configuracaoTMS, null, string.Empty, filial, tipoOperacao, true);

                        if (cargaPedido != null)
                            servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);
                    }

                    if (mensagemErro.Length > 0)
                    {
                        unitOfWork.Rollback();
                        stMensagem.Append($"Carga {cargaIntegracao.NumeroCarga} Pedido {cargaIntegracao.NumeroPedidoEmbarcador}: {mensagemErro}");
                        return null;
                    }

                    carga = cargaPedido.Carga;
                }

                unitOfWork.CommitChanges();

                if (configuracaoTMS.FecharCargaPorThread)
                {
                    carga.FechandoCarga = true;
                    repCarga.Atualizar(carga);
                }
                else
                {
                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                    serCarga.FecharCarga(carga, unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware, false);
                    carga.CargaFechada = true;
                    cargaPedido.Carga.DataAtualizacaoCarga = DateTime.Now;
                    repCarga.Atualizar(carga);
                }

                return carga;
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex, "RequestLog");

                unitOfWork.Rollback();

                stMensagem.Append(ex.Message);

                return null;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unitOfWork.Rollback();

                stMensagem.Append("Falha genérica ao gerar carga.");

                return null;
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.ProdutoEmbarcador servicoProdutoEmbarcador = new Servicos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            _unitOfWork.Start();

            Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = null;
            if (!string.IsNullOrWhiteSpace(produto.CodigoGrupoProduto))
                grupoProduto = servicoProdutoEmbarcador.IntegrarGrupoProduto(produto.CodigoGrupoProduto, produto.DescricaoGrupoProduto);

            Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao = null;
            if (produto.LinhaSeparacao != null && !string.IsNullOrWhiteSpace(produto.LinhaSeparacao.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produto.LinhaSeparacao.Descricao))
                linhaSeparacao = servicoProdutoEmbarcador.IntegrarLinhaSeparacao(produto.LinhaSeparacao.CodigoIntegracao, produto.LinhaSeparacao.Descricao, produto.LinhaSeparacao.Roteiriza, produto.LinhaSeparacao?.Filial?.CodigoIntegracao ?? "", produto.LinhaSeparacao?.NivelPrioridade ?? 0, auditado);

            Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = null;
            if (produto.MarcaProduto != null && !string.IsNullOrWhiteSpace(produto.MarcaProduto.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produto.MarcaProduto.Descricao))
                marcaProduto = servicoProdutoEmbarcador.IntegrarMarca(produto.MarcaProduto.CodigoIntegracao, produto.MarcaProduto.Descricao, auditado);

            if (linhaSeparacao != null && linhaSeparacao.Ativo == false)
            {
                _unitOfWork.Rollback();
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("A linha de separação informada (" + linhaSeparacao.Descricao + ") está inatíva.");
            }
            else
            {
                Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = null;
                if (produto.TipoEmbalagem != null && !string.IsNullOrWhiteSpace(produto.TipoEmbalagem.CodigoIntegracao) && !string.IsNullOrWhiteSpace(produto.TipoEmbalagem.Descricao))
                    tipoEmbalagem = servicoProdutoEmbarcador.IntegrarTipoEmbalagem(produto.TipoEmbalagem.CodigoIntegracao, produto.TipoEmbalagem.Descricao);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador prod = repProdutoEmbarcador.buscarPorCodigoEmbarcador(produto.CodigoProduto);
                if (prod != null)
                    produtos.Add(prod);

                prod = servicoProdutoEmbarcador.IntegrarProduto(produtos, configuracao, produto.CodigoProduto, produto.DescricaoProduto, produto.PesoUnitario, grupoProduto, produto.MetroCubito, auditado, produto.CodigoDocumentacao, produto.Atualizar, produto.CodigoNCM, produto.QuantidadePorCaixa, produto.QuantidadeCaixaPorPallet, produto.Altura, produto.Largura, produto.Comprimento, linhaSeparacao, tipoEmbalagem, marcaProduto, produto.UnidadeMedida, produto.Observacao, produto.CodigocEAN, produto.CodigoEAN);

                this.SalvarFiliaisProduto(produto, prod);
                this.SalvarOrganizacoes(produto, prod);
                SalvarUnidadeDeConversao(produto, prod);
                ExecutarConversaoDePeso(produto, prod);

                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }
        }

        public bool SolicitarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref string mensagemCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                mensagemCancelamento = string.Empty;

                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                {
                    Carga = carga,
                    GerarIntegracoes = false,
                    MotivoCancelamento = "Cancelamento efetuado ao receber nova integração",
                    TipoServicoMultisoftware = tipoServicoMultisoftware
                };

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware);

                if (cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.Cancelada)
                    mensagemCancelamento = $"A carga {carga.CodigoCargaEmbarcador} não pode ser atualizada, é necessário cancelar manualmente para atualizar o registro.";

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in pedidos)
                {
                    pedido.Pedido.SituacaoPedido = SituacaoPedido.Cancelado;
                    repositorioPedido.Atualizar(pedido.Pedido);
                }

                unitOfWork.CommitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                throw;
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<int> SalvarDocumentoTransporte(Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte, Dominio.Entidades.WebService.Integradora integradora)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Servicos.Global.TempoDeExecucao servicoTempoDeExecucao = new Global.TempoDeExecucao();

            //Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, true, "Integração de Documento", "", Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransporte), string.Empty, "json", _unitOfWork, carga, SituacaoIntegracao.AgIntegracao);
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransporte);
            Servicos.Log.TratarErro(jsonString, "SalvarDT");

            string mensagemErro = string.Empty;

            try
            {
                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(_unitOfWork);

                string hashObjeto = GerarHashJson(jsonString);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte repositorioTipocumentoTransporte = new Repositorio.Embarcador.Cargas.TipoDocumentoTransporte.TipoDocumentoTransporte(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.Embarcador.Logistica.EventosDT repositorioEventosDT = new Repositorio.Embarcador.Logistica.EventosDT(_unitOfWork);
                Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE repCargaPedidoRecusaCTE = new Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE(_unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);

                Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever servicoIntegracaoUnilever = new Embarcador.Integracao.Unilever.IntegracaoUnilever(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                string codigoModeloVeicular = string.Empty;
                string numerCargaSemZero = Regex.Replace(documentoTransporte.NumeroCarga, @"^0+", "");
                List<int> protocolosPedidos = new List<int>();

                foreach (Dominio.ObjetosDeValor.WebService.Rest.Unilever.Pedido pedido in documentoTransporte.Pedido)
                {
                    if (string.IsNullOrEmpty(pedido.ProtocoloPedido) && pedido.Stage.Any(s => s.TipoPercurso != Vazio.PercursoRegreso))
                    {
                        mensagemErro = "Precisa informar o protocolo do pedido";
                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                        return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoDadosInvalidos(mensagemErro);
                    }

                    if (!string.IsNullOrEmpty(pedido?.Stage.Select(s => s?.ModeloVeicular?.CodigoIntegracao).FirstOrDefault()) && !string.IsNullOrWhiteSpace(codigoModeloVeicular))
                        codigoModeloVeicular = pedido?.Stage.Select(s => s?.ModeloVeicular?.CodigoIntegracao).FirstOrDefault() ?? string.Empty;

                    foreach (Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage stage in pedido.Stage)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga existeModelo = repModeloVeicularCarga.buscarPorCodigoIntegracao(stage?.ModeloVeicular?.CodigoIntegracao);

                        if (existeModelo == null && !string.IsNullOrEmpty(stage?.ModeloVeicular?.Descricao))
                        {
                            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga novoModeloVeicular = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga();

                            novoModeloVeicular.CodigoIntegracao = stage?.ModeloVeicular?.CodigoIntegracao ?? string.Empty;
                            novoModeloVeicular.Descricao = stage.ModeloVeicular.Descricao;
                            novoModeloVeicular.Ativo = true;
                            novoModeloVeicular.VeiculoPaletizado = false;
                            novoModeloVeicular.ModeloControlaCubagem = false;
                            novoModeloVeicular.ModeloTracaoReboquePadrao = false;
                            novoModeloVeicular.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral;

                            if (stage?.ModeloVeicular?.Capacidade > 0)
                            {
                                novoModeloVeicular.CapacidadePesoTransporte = stage.ModeloVeicular.Capacidade;
                                novoModeloVeicular.ToleranciaPesoExtra = 0;
                                novoModeloVeicular.ToleranciaPesoMenor = 0;
                            }
                            else
                            {
                                novoModeloVeicular.CapacidadePesoTransporte = 30000;
                                novoModeloVeicular.ToleranciaPesoExtra = 5000;
                                novoModeloVeicular.ToleranciaPesoMenor = 1;
                            }

                            if (stage?.ModeloVeicular?.ToleranciaExtra > 0)
                                novoModeloVeicular.ToleranciaPesoExtra = stage.ModeloVeicular.ToleranciaExtra;

                            repModeloVeicularCarga.Inserir(novoModeloVeicular);
                        }

                        if (existeModelo != null)
                        {
                            if (stage?.ModeloVeicular?.Capacidade > 0)
                            {
                                existeModelo.CapacidadePesoTransporte = stage.ModeloVeicular.Capacidade;
                                existeModelo.ToleranciaPesoExtra = 0;
                                existeModelo.ToleranciaPesoMenor = 0;
                            }

                            if (stage?.ModeloVeicular?.ToleranciaExtra > 0)
                                existeModelo.ToleranciaPesoExtra = stage.ModeloVeicular.ToleranciaExtra;

                            repModeloVeicularCarga.Atualizar(existeModelo);
                        }
                    }

                    int.TryParse(pedido.ProtocoloPedido, out int protocolo);
                    protocolosPedidos.Add(protocolo);
                }

                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(documentoTransporte.Filial?.CodigoIntegracao ?? string.Empty);
                if (filial == null)
                    filial = repFilial.buscarPorCodigoEmbarcador(documentoTransporte.Filial.Codigo.ToString());

                if (filial == null)
                {
                    mensagemErro = $"A filial ${(!string.IsNullOrEmpty(documentoTransporte.Filial.CodigoIntegracao) ? documentoTransporte.Filial.CodigoIntegracao : documentoTransporte.Filial.Codigo.ToString())} informada não esta cadastrada no MultiEmbarcador";
                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                    return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                }

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador(documentoTransporte.TipoCargaEmbarcador.CodigoIntegracao);
                if (tipoCarga == null)
                {
                    mensagemErro = $"O Tipo de carga {documentoTransporte.TipoCargaEmbarcador.CodigoIntegracao} informado não esta cadastrado no MultiEmbarcador.";
                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                    return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                }

                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigoIntegracao(documentoTransporte.TransportadoraEmitente?.CodigoIntegracao);
                if (transportador == null)
                {
                    mensagemErro = $"O transportador {documentoTransporte.TransportadoraEmitente?.CodigoIntegracao} informado não esta cadastrado no MultiEmbarcador.";
                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                    return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                }

                Dominio.Entidades.Embarcador.Cargas.Carga carga;

                if (configuracaoTMS.IncluirCargaCanceladaProcessarDT)
                    carga = repCarga.BuscarCargaPorCodigoEmbarcadorProcessarDocumentos(documentoTransporte.NumeroCarga);
                else
                    carga = repCarga.BuscarCargaPorCodigoEmbarcadorInclindoCanceladas(documentoTransporte.NumeroCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga cargaSegundaria = repCarga.BuscarCargaPorCodigoEmbarcadorInclindoCanceladas(numerCargaSemZero);
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cancelamentoCarga = repositorioCargaCancelamento.BuscarCancelamentoCargaPorDocumento(documentoTransporte.NumeroCarga);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(documentoTransporte?.TipoOperacao?.CodigoIntegracao ?? string.Empty);

                if (protocolosPedidos.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidosExistentes = repPedido.BuscarPorProtocolos(protocolosPedidos);
                    if (listaPedidosExistentes.Count != protocolosPedidos.Count)
                    {
                        List<int> protocolosPedidosInexistentes = new List<int>();
                        foreach (int protocolo in protocolosPedidos)
                            if (!listaPedidosExistentes.Any(p => p.Protocolo == protocolo))
                                protocolosPedidosInexistentes.Add(protocolo);

                        if (protocolosPedidosInexistentes.Count > 0)
                        {
                            mensagemErro = $"Protocolo(s) {string.Join(", ", protocolosPedidosInexistentes)} não existe no MultiEmbarcador.";
                            stopWatch.Stop();
                            servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga?.Protocolo ?? 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                            return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                        }
                    }
                    List<int> protocolosPedidosCancelados = new List<int>();
                    foreach (int protocolo in protocolosPedidos)
                        if (listaPedidosExistentes.Any(p => p.Protocolo == protocolo && p.SituacaoPedido == SituacaoPedido.Cancelado))
                            protocolosPedidosCancelados.Add(protocolo);

                    if (protocolosPedidosCancelados.Count > 0 && cargaSegundaria == null)
                    {
                        mensagemErro = $"Pedido protocolo {string.Join(", ", protocolosPedidosCancelados)} esta cancelado e não pode ser vinculado na carga";
                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga?.Protocolo ?? 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                        return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                    }

                    if (!(tipoOperacao?.OperacaoDeRedespacho ?? false))
                    {

                        List<int> codigosPedidoDT = repCargaPedido.BuscarProtocoloPedidoPorCarga(carga?.Codigo ?? 0); // buscar o codigo dos pedidos da carga enviada na DT.
                        List<int> codigosPedidoOutrasDTs = repCargaPedido.BuscarProtocoloPedidoPorProtocoloDeferenteDeCarga(protocolosPedidos, carga?.Codigo ?? 0); //Buscar codigo dos pedidos de carga deferente da DT.

                        List<int> diferentes = new List<int>();
                        foreach (int codigoOutraPedidoDT in codigosPedidoOutrasDTs)
                        {
                            if (codigosPedidoDT != null && codigosPedidoDT.Count > 0 && !codigosPedidoDT.Contains(codigoOutraPedidoDT))
                                diferentes.Add(codigoOutraPedidoDT);
                        }

                        if (diferentes.Count > 0)
                        {
                            if (configuracaoTMS.IncluirCargaCanceladaProcessarDT)
                            {
                                //ARCELOR. vamos remover os pedidos da carga para adicionar os mesmos na nova....
                                foreach (int codigoOutraPedidoDT in codigosPedidoOutrasDTs)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargapedido = repCargaPedido.BuscarPorPedidoECargaDiferente(carga.Codigo, codigoOutraPedidoDT);
                                    if (cargapedido != null)
                                    {
                                        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoVinculadoCarga(cargapedido, _unitOfWork, configuracaoTMS, _tipoServicoMultisoftware, _clienteMultisoftware, removerPedido: true);

                                        Servicos.Auditoria.Auditoria.Auditar(_auditado, cargapedido.Carga, null, "Excluiu pedido vinculado por integração ao receber o pedido em outra carga. Pedido Removido: " + cargapedido.Pedido.NumeroPedidoEmbarcador + " Carga: " + carga.CodigoCargaEmbarcador, _unitOfWork);
                                    }
                                }
                            }
                            else
                            {
                                mensagemErro = $"Pedido(s) protocolo(s) {string.Join(", ", diferentes)} estão ativos em outra carga no Multiembarcador";
                                stopWatch.Stop();
                                servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga?.Protocolo ?? 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                                return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                            }
                        }

                        if (carga == null && codigosPedidoOutrasDTs.Count > 0 && !configuracaoTMS.IncluirCargaCanceladaProcessarDT)
                        {
                            //CARGA NOVA, protocolos estao em outra DT E NAO É ARCELOR
                            mensagemErro = $"Pedido(s) protocolo(s) {string.Join(", ", codigosPedidoOutrasDTs)} estão ativos em outra carga no Multiembarcador";
                            stopWatch.Stop();
                            servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga?.Protocolo ?? 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                            return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                        }


                        List<int> pedidosRemovidos = new List<int>();
                        if (codigosPedidoDT.Count > 0)
                        {
                            foreach (int codigoPedidoDT in codigosPedidoDT)
                            {
                                if (!protocolosPedidos.Contains(codigoPedidoDT))
                                    pedidosRemovidos.Add(codigoPedidoDT);
                            }
                        }

                        List<int> removidos = new List<int>();
                        foreach (int codigoRemovido in pedidosRemovidos)
                        {
                            if (codigosPedidoOutrasDTs.Contains(codigoRemovido))
                                removidos.Add(codigoRemovido);
                        }

                        if (removidos.Count > 0)
                        {
                            mensagemErro = $"Pedido(s) protocolo(s) {string.Join(", ", removidos)} foram removidos porem estao ativos em outra carga no Multiembarcador";
                            stopWatch.Stop();
                            servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga?.Protocolo ?? 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                            return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                        }

                        //List<int> protocolosPedidosVinculados = new List<int>();
                        //List<int> cargas = new List<int>();
                        //foreach (int protocolo in protocolosPedidos)
                        //{
                        //    if (repCargaPedido.ExistePorPedidoPorProtocolo(protocolo, true, carga?.CodigoCargaEmbarcador ?? ""))
                        //        protocolosPedidosVinculados.Add(protocolo);
                        //}


                        //if (protocolosPedidosVinculados.Count > 0)
                        //    return Retorno<int>.CriarRetornoExcecao($"Pedido(s) protocolo(s) {string.Join(", ", protocolosPedidosVinculados)} estão ativos em outra carga no Multiembarcador");

                    }
                    else if (tipoOperacao?.OperacaoDeRedespacho ?? false)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedidos(protocolosPedidos);

                        foreach (int protocolo in protocolosPedidos)
                        {
                            if (!cargaPedidos.Any(obj => obj.Pedido.Codigo == protocolo))
                            {
                                //#63626 mesmo NÃO existindo carga pedido para o pedido, validar se o pedido tem a mesma localidade do expedidor da stage do pedido.

                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoSalvo = repPedido.BuscarPorProtocolo(protocolo);

                                //vamos deixar a mesma mensagem pois ja esta sendo tratada nos demais clientes
                                if (pedidoSalvo == null)
                                {
                                    mensagemErro = $"Não existe uma carga feita anteriormente para esse pedido disponível para ser redespachada";
                                    stopWatch.Stop();
                                    servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga?.Protocolo ?? 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                                    return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                                }

                                Dominio.ObjetosDeValor.WebService.Rest.Unilever.Stage Objetostage = documentoTransporte.Pedido.Where(obj => obj.ProtocoloPedido.ToInt() == protocolo).Select(obj => obj.Stage.FirstOrDefault()).FirstOrDefault();

                                Dominio.Entidades.Localidade localidadeExpedido = null;

                                if (localidadeExpedido == null && !string.IsNullOrWhiteSpace(Objetostage?.Expedidor?.CEP))
                                    localidadeExpedido = repositorioLocalidade.BuscarPorCEP(Objetostage?.Expedidor?.CEP);

                                if (localidadeExpedido == null && !string.IsNullOrWhiteSpace(Objetostage?.Expedidor.Cidade))
                                    localidadeExpedido = repositorioLocalidade.BuscarPorDescricao(Objetostage?.Expedidor.Cidade)?.FirstOrDefault();

                                if (localidadeExpedido == null && !string.IsNullOrWhiteSpace(Objetostage?.Expedidor.Cidade))
                                    localidadeExpedido = repositorioLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(Objetostage.Expedidor.Cidade), Objetostage.Expedidor.UF);

                                if (localidadeExpedido == null && !string.IsNullOrWhiteSpace(Objetostage?.Expedidor.Cidade))
                                    localidadeExpedido = repositorioLocalidade.BuscarPorDescricao(Utilidades.String.RemoveAllSpecialCharacters(Objetostage.Expedidor.Cidade))?.FirstOrDefault();

                                if (pedidoSalvo.Remetente?.Localidade?.Codigo != localidadeExpedido?.Codigo)
                                {
                                    mensagemErro = $"Não existe uma carga feita anteriormente para esse pedido disponível para ser redespachada";
                                    stopWatch.Stop();
                                    servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga?.Protocolo ?? 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                                    return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                                }
                            }
                        }
                    }
                }

                if (carga != null)
                {
                    if (!carga.CargaEmitidaParcialmente || carga.DataConfirmacaoCtes.HasValue)
                    {
                        List<SituacaoCarga> situacaoesPermitidasAtualizacao = new List<SituacaoCarga>() { SituacaoCarga.Nova, SituacaoCarga.AgNFe, SituacaoCarga.CalculoFrete, SituacaoCarga.AgTransportador, SituacaoCarga.Cancelada };

                        if (!situacaoesPermitidasAtualizacao.Contains(carga.SituacaoCarga))
                        {
                            if (!repCargaPedidoRecusaCTE.CargaPossuiRecusaCte(carga.Codigo))
                            {
                                if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn && carga.DataConfirmacaoCtes.HasValue)
                                {
                                    //feito temporariamente para correcao dos testes UNL aiaiaiia
                                    carga.CargaEmitidaParcialmente = false;
                                    repCarga.Atualizar(carga);
                                }

                                mensagemErro = $"A situação da carga {carga.CodigoCargaEmbarcador} não permite atualização.";
                                stopWatch.Stop();
                                servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                                return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                            }
                        }
                    }

                    Dominio.Entidades.Empresa transportadorNovo = null;

                    if (!string.IsNullOrWhiteSpace(documentoTransporte.TransportadoraEmitente?.CNPJ))
                        transportadorNovo = repEmpresa.BuscarEmpresaPorCNPJ(documentoTransporte.TransportadoraEmitente?.CNPJ);
                    else if (!string.IsNullOrWhiteSpace(documentoTransporte.TransportadoraEmitente?.CodigoIntegracao))
                        transportadorNovo = repEmpresa.BuscarPorCodigoIntegracao(documentoTransporte.TransportadoraEmitente?.CodigoIntegracao);

                    if ((carga?.CargaGeradaViaDocumentoTransporte ?? false) && !(transportadorNovo?.RecusarIntegracaoPODUnilever ?? false) && carga.CodigoCargaEmbarcador.StartsWith("0") && cargaSegundaria != null)
                    {
                        mensagemErro = "Esta carga não pode ser avançada pois transportador não esta habilitado para o projeto";
                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga?.Protocolo ?? 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                        return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                    }

                    if (carga.SituacaoCarga == SituacaoCarga.Cancelada && cargaSegundaria != null && !cargaSegundaria.CodigoCargaEmbarcador.StartsWith("0") && cargaSegundaria.SituacaoCarga != SituacaoCarga.Cancelada)
                    {
                        string mensagemRetorno = string.Empty;
                        bool sucessoNoCancelamento = SolicitarCancelamentoCarga(cargaSegundaria, ref mensagemRetorno, _tipoServicoMultisoftware, _unitOfWork);

                        if (!sucessoNoCancelamento)
                        {
                            stopWatch.Stop();
                            servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga?.Protocolo ?? 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemRetorno);

                            return Retorno<int>.CriarRetornoExcecao(mensagemRetorno);
                        }

                        RemoverRegistrosDeCancelamentoCarga(carga, _unitOfWork);
                        AtivarCargaCancelada(carga, _unitOfWork);
                    }


                    if (transportadorNovo != null)
                        repCanhoto.AtualizarEmpresaCarga(carga, repCargaPedido.BuscarCodigosPedidoPorCarga(carga.Codigo), transportadorNovo);

                    if ((carga.SituacaoCarga != SituacaoCarga.Cancelada) || (cancelamentoCarga?.Situacao == SituacaoCancelamentoCarga.Cancelada))
                    {
                        carga.ControleIntegracaoEmbarcador = documentoTransporte.ControleIntegracaoEmbarcador;
                        repCarga.Atualizar(carga);

                        Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno objetoIntegracao = repIntegradoraIntegracaoRetorno.BuscarUltimaPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);

                        if (objetoIntegracao != null && objetoIntegracao.HashJson == hashObjeto)
                        {
                            //Ultima integracao é igual, entao ja salva como processado
                            SalvarDocumento(documentoTransporte, objetoIntegracao.Carga);
                            Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, true, "Integração com mesmo HASH já processado", "", Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransporte), string.Empty, "json", _unitOfWork, objetoIntegracao.Carga, SituacaoIntegracao.Integrado, hashObjeto);
                            ReenviarCargaIntegracaoEvento(carga.Codigo);

                            stopWatch.Stop();
                            servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                            return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoSucesso(objetoIntegracao.Carga.Protocolo, $"Registro Processado com successo. Protocolo: {objetoIntegracao.Carga.Protocolo}");
                        }

                        SalvarDocumento(documentoTransporte, carga);
                        Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, true, "Integração de Documento", "", Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransporte), string.Empty, "json", _unitOfWork, carga, SituacaoIntegracao.AgIntegracao, hashObjeto);

                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                        return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoSucesso(carga.Protocolo, $"Registro Processado com successo. Protocolo: {carga.Protocolo}");
                    }
                }

                if (carga == null && (documentoTransporte.Pedido == null || documentoTransporte.Pedido.Count == 0))
                {
                    mensagemErro = "Documento de transporte sem pedidos.";
                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                    return Retorno<int>.CriarRetornoExcecao(mensagemErro);
                }

                if (transportador == null && !string.IsNullOrWhiteSpace(documentoTransporte.TransportadoraEmitente?.CNPJ))
                    transportador = repEmpresa.BuscarEmpresaPorCNPJ(documentoTransporte.TransportadoraEmitente?.CNPJ);
                else if (transportador == null && !string.IsNullOrWhiteSpace(documentoTransporte.TransportadoraEmitente?.CodigoIntegracao))
                    transportador = repEmpresa.BuscarPorCodigoIntegracao(documentoTransporte.TransportadoraEmitente?.CodigoIntegracao);

                if (cargaSegundaria != null && transportador != null && (transportador?.RecusarIntegracaoPODUnilever ?? false))
                {
                    string mensagemRetorno = string.Empty;
                    bool sucessoNoCancelamento = SolicitarCancelamentoCarga(cargaSegundaria, ref mensagemRetorno, _tipoServicoMultisoftware, _unitOfWork);

                    if (!sucessoNoCancelamento)
                    {
                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemRetorno);

                        return Retorno<int>.CriarRetornoExcecao(mensagemRetorno);
                    }
                }

                carga = new Dominio.Entidades.Embarcador.Cargas.Carga()
                {
                    CodigoCargaEmbarcador = documentoTransporte?.NumeroCarga,
                    DataCarregamentoCarga = documentoTransporte?.DataTerminoCarregamento.ToNullableDateTime(),
                    DataInicialPrevisaoCarregamento = documentoTransporte?.DataInicioCarregamento.ToNullableDateTime(),
                    DataFinalPrevisaoCarregamento = documentoTransporte?.DataTerminoCarregamento.ToNullableDateTime(),
                    DataInicioViagemPrevista = documentoTransporte?.DataPrevisaoInicioViagem.ToNullableDateTime(),
                    DataCriacaoCarga = documentoTransporte?.DataCriacaoCarga.ToNullableDateTime() ?? DateTime.MinValue,
                    Filial = filial,
                    Empresa = transportador,
                    TipoDeCarga = tipoCarga,
                    TipoOperacao = tipoOperacao,
                    ModeloVeicularCarga = !string.IsNullOrWhiteSpace(codigoModeloVeicular) ? repModeloVeicularCarga.buscarPorCodigoIntegracao(codigoModeloVeicular) : null,
                    SituacaoCarga = SituacaoCarga.Nova,
                    ExternalID1 = documentoTransporte.ExternalID1,
                    ExternalID2 = documentoTransporte.ExternalID2,
                    TipoDocumentoTransporte = repositorioTipocumentoTransporte.BuscarPorCodigoIntegracao(documentoTransporte?.TipoDT?.CodigoIntegracao ?? string.Empty),
                    CteGlobalizado = documentoTransporte.DocumentoGlobalizado,
                    ControleIntegracaoEmbarcador = documentoTransporte.ControleIntegracaoEmbarcador,
                    ProcessamentoEspecial = documentoTransporte?.ProcessamentoEspecial ?? string.Empty
                };

                servicoIntegracaoUnilever.ProcessamentoDadosTransporteCarga(carga, documentoTransporte, _tipoServicoMultisoftware, _auditado, _clienteURLAcesso, _adminStringConexao);

                if (carga.TipoOperacao != null)
                {
                    carga.ExigeNotaFiscalParaCalcularFrete = carga.TipoOperacao.ExigeNotaFiscalParaCalcularFrete;
                    carga.NaoExigeVeiculoParaEmissao = carga.TipoOperacao.NaoExigeVeiculoParaEmissao;

                    if (carga.TipoDeCarga == null && carga.TipoOperacao.TipoDeCargaPadraoOperacao != null)
                        carga.TipoDeCarga = carga.TipoOperacao.TipoDeCargaPadraoOperacao;
                }
                else
                    carga.ExigeNotaFiscalParaCalcularFrete = configuracaoTMS.ExigirNotaFiscalParaCalcularFreteCarga;

                if (documentoTransporte.EventosDT != null && documentoTransporte?.EventosDT.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.DocumentoTransporte.EventosDT eventosDT in documentoTransporte.EventosDT)
                    {
                        Dominio.Entidades.Embarcador.Logistica.EventosDT eventosDTEntidade = new Dominio.Entidades.Embarcador.Logistica.EventosDT()
                        {
                            Qualificador = eventosDT.Qualificador,
                            DataInicioPrevisto = eventosDT.DataInicioPrevisto,
                            DataFimPrevisto = eventosDT.DataFimPrevisto,
                            DataInicioReal = eventosDT.DataInicioReal,
                            DataFimReal = eventosDT.DataFimReal
                        };

                        repositorioEventosDT.Inserir(eventosDTEntidade);
                    }
                }

                if (carga.Empresa != null && carga.Empresa.TransportadorFerroviario)
                {
                    if (tipoOperacao != null && tipoOperacao?.ConfiguracaoTransportador?.TipoOperacaoModalFerroviario != null)
                    {
                        carga.TipoOperacao = tipoOperacao.ConfiguracaoTransportador.TipoOperacaoModalFerroviario;
                        carga.NaoExigeVeiculoParaEmissao = tipoOperacao.ConfiguracaoTransportador.TipoOperacaoModalFerroviario.NaoExigeVeiculoParaEmissao;
                        carga.ExigeNotaFiscalParaCalcularFrete = tipoOperacao.ConfiguracaoTransportador.TipoOperacaoModalFerroviario.ExigeNotaFiscalParaCalcularFrete;
                    }
                }

                repCarga.Inserir(carga);
                carga.Protocolo = carga.Codigo;
                repCarga.Atualizar(carga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidosCarga = repCargaPedido.BuscarPorCarga(carga.Codigo);

                if (carga.TipoOperacao != null)
                    new Servicos.Embarcador.Carga.CargaPallets(_unitOfWork, configuracaoTMS).RatearPaletesModeloVeicularCargaEntreOsPedidos(carga, listaCargaPedidosCarga);

                SalvarDocumento(documentoTransporte, carga);

                Dominio.ObjetosDeValor.WebService.Retorno<int> retorno = Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoSucesso(carga.Protocolo, $"Registro Processado com successo. Protocolo: {carga.Protocolo}");

                Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, true, "Integração de Documento de Transporte", "", Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransporte), Newtonsoft.Json.JsonConvert.SerializeObject(retorno), "json", _unitOfWork, carga, SituacaoIntegracao.AgIntegracao, hashObjeto);

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                Servicos.Log.TratarErro($"SalvarDocumentoTransporte - Protocolo Carga {carga?.Protocolo ?? 0} | Tempo total levado: {ts.ToString(@"mm\:ss\:fff")}", "TempoExecucao");

                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", carga?.Protocolo ?? 0, 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                return retorno;
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex.Message, "SalvarDT");

                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, ex.Message);

                return Retorno<int>.CriarRetornoExcecao(ex.Message);
            }
            catch (WebServiceException ex)
            {
                Servicos.Log.TratarErro(ex.Message, "SalvarDT");

                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, ex.Message);

                return Retorno<int>.CriarRetornoExcecao(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex.Message, "SalvarDT");

                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("SalvarDocumentoTransporte", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, ex.Message);

                return Retorno<int>.CriarRetornoExcecao(ex.Message);
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> BuscarTransportadorCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Servicos.WebService.Empresa.Empresa servicoEmpresa = new Empresa.Empresa(_unitOfWork);
            Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>();

            retorno.Status = true;
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            if (protocolo == null || protocolo.protocoloIntegracaoCarga == 0)
                return ObjetoCargaInvalido(retorno, "Informe o protocolo de integração da carga.");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);

            if (carga == null)
                return ObjetoCargaInvalido(retorno, "Carga não encontrada.");
            else if (carga.Empresa == null)
                return ObjetoCargaInvalido(retorno, "Carga sem transportador informado.");

            retorno.Objeto = servicoEmpresa.ConverterObjetoEmpresa(carga.Empresa);
            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou transportador da carga", _unitOfWork);

            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCargaService(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>();

            retorno.Status = true;
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            if (protocolo == null)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "É obrigatório informar o protocolo de integração";
                return retorno;
            }

            if (protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido == 0 && string.IsNullOrWhiteSpace(protocolo.IdentificadorRota))
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "Por favor, informe os códigos de integração";
                return retorno;
            }

            string mensagem = "";

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (protocolo.protocoloIntegracaoCarga > 0 && protocolo.protocoloIntegracaoPedido > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = repCargaPedido.BuscarListaPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if (cargaPedido == null || cargaPedido.Count == 0)
                {
                    mensagem = "Não foi localizada uma carga para os protocolos de carga (" + protocolo.protocoloIntegracaoCarga + ") e de pedido (" + protocolo.protocoloIntegracaoPedido + ") informados, por favor verifique.";
                }
                else
                {
                    cargaPedidos.AddRange(cargaPedido);
                }
            }
            else if (!string.IsNullOrWhiteSpace(protocolo.IdentificadorRota))
            {
                cargaPedidos = repCargaPedido.BuscarPorIdentificadorRota(protocolo.IdentificadorRota);

                if (cargaPedidos.Count == 0)
                    mensagem = "Não foi localizada uma carga para o identificador de rota (" + protocolo.IdentificadorRota + ") informado, por favor verifique.";
            }
            else
            {
                if (protocolo.protocoloIntegracaoCarga > 0)
                {
                    cargaPedidos = repCargaPedido.BuscarPorProtocoloCarga(protocolo.protocoloIntegracaoCarga, configuracao.RetornarCargasAgrupadasCarregamento);

                    if (cargaPedidos == null || cargaPedidos.Count == 0)
                        mensagem = "Não foi localizada uma carga para os protocolo da carga (" + protocolo.protocoloIntegracaoCarga + ") informado, por favor verifique.";
                }
                else if (protocolo.protocoloIntegracaoPedido > 0)
                {
                    cargaPedidos = repCargaPedido.BuscarPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);
                    if (cargaPedidos.Count == 0)
                        mensagem = "Não foi localizada uma carga para os protocolo de pedido (" + protocolo.protocoloIntegracaoPedido + ") informado, por favor verifique.";
                }
            }

            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();
            if (configuracaoWebService.QuandoGeradoPreCteRetornarInformacaoDeFreteCTeIntegrado)
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                bool possuiCTePendenteAutorizacao = repCargaCte.PossuiCTePendenteDeAutorizacao(cargaPedidos.FirstOrDefault()?.Carga?.Codigo ?? 0);
                if (possuiCTePendenteAutorizacao)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Os CT-es dessa carga não foram integrados ou estão pendentes de aprovação";
                    return retorno;
                }
            }

            retorno.Objeto = BuscarCargasPedidos(cargaPedidos, configuracao, ref mensagem, _unitOfWork);

            if (!string.IsNullOrWhiteSpace(mensagem))
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = mensagem;
            }
            else
                Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou carga", _unitOfWork);

            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCargaService(DateTime dataDe, DateTime dataAte, int codigoEmpresa)
        {
            if (dataDe > dataAte)
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>.CriarRetornoExcecao("Data de não pode ser maior que data até.");

            string mensagem = "";

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarListaPorDataCarregamento(dataDe, dataAte, codigoEmpresa);
            if (cargaPedidos == null || cargaPedidos.Count == 0)
                mensagem = "Não foi localizada cargas com as datas de carregamento informadas.";

            List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracao = BuscarCargasPedidos(cargaPedidos, configuracao, ref mensagem, _unitOfWork);

            if (!string.IsNullOrWhiteSpace(mensagem))
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>.CriarRetornoExcecao(mensagem);
            else
                Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou carga", _unitOfWork);

            return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>.CriarRetornoSucesso(cargasIntegracao);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarCargasPorPeriodo(Periodo periodo)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            ValidarPeriodos(periodo);
            string mensagem = "";

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarListaPorPeriodo(periodo);

            if (cargaPedidos == null || cargaPedidos.Count == 0)
                throw new WebServiceException("Não foi localizada cargas com as datas informadas.");

            List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasPedido = BuscarCargasPedidos(cargaPedidos, configuracao, ref mensagem, _unitOfWork);

            if (!string.IsNullOrWhiteSpace(mensagem))
                throw new WebServiceException("Dados inválidos para esta integração.");
            else
                Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou carga", _unitOfWork);

            return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>.CriarRetornoSucesso(cargasPedido);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarPedidoService(int protocolo)
        {
            Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>();

            retorno.Status = true;
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            if (protocolo == null)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "É obrigatório informar o protocolo de integração";
                return retorno;
            }

            if (protocolo == 0)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "Por favor, informe o protocolo do pedido";
                return retorno;
            }

            string mensagem = "";

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorProtocolo(protocolo);

            if (pedido == null)
                mensagem = "Não foi localizado um pedido para osprotocolo de pedido (" + protocolo + ") informado, por favor verifique.";


            retorno.Objeto = ConverterPedidioEmCargaIntegracao(pedido, configuracao, ref mensagem, _unitOfWork);

            if (!string.IsNullOrWhiteSpace(mensagem))
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = mensagem;
            }
            else
                Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou carga", _unitOfWork);

            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta ConverterCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido)
        {
            if (carga == null)
                return null;

            Servicos.WebService.Filial.Filial servicoFilial = new WebService.Filial.Filial(_unitOfWork);
            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(_unitOfWork);
            Servicos.WebService.Carga.Carga servicoCarga = new WebService.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);
            Servicos.Embarcador.Localidades.Endereco servicoEndereco = new Servicos.Embarcador.Localidades.Endereco(_unitOfWork);
            Servicos.WebService.Carga.TipoCarga servicoTipoCarga = new WebService.Carga.TipoCarga(_unitOfWork);
            Servicos.WebService.Carga.TipoOperacao servicoTipoOperacao = new WebService.Carga.TipoOperacao(_unitOfWork);
            Servicos.WebService.Carga.Pedido servicoPedido = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Servicos.WebService.Empresa.Empresa servicoEmpresa = new Empresa.Empresa(_unitOfWork);
            Servicos.Embarcador.NFe.NFe serNfe = new Servicos.Embarcador.NFe.NFe(_unitOfWork);

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta cargaIntegracaoCompleta = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta()
            {
                DataCriacaoCarga = carga?.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                FecharCargaAutomaticamente = carga?.FechandoCarga ?? false,
                Filial = servicoFilial.ConverterObjetoFilial(carga?.Filial ?? null),
                NumeroCarga = carga?.CodigoCargaEmbarcador ?? string.Empty,
                NumeroPreCarga = carga?.PreCarga?.NumeroPreCarga ?? string.Empty,
                CargaSVMProprio = carga?.CargaSVM ?? false
            };


            cargaIntegracaoCompleta.Pedidos = listaCargaPedido.Select(cargaPedido =>
                new Dominio.ObjetosDeValor.WebService.Carga.Pedido()
                {
                    Destinatario = svcPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Destinatario),
                    Expedidor = svcPessoa.ConverterObjetoPessoa(cargaPedido.Expedidor),
                    Recebedor = svcPessoa.ConverterObjetoPessoa(cargaPedido.Recebedor),
                    Remetente = svcPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Remetente),
                    Tomador = svcPessoa.ConverterObjetoPessoa(cargaPedido.Tomador),
                    NumeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    Protocolo = cargaPedido.Codigo,
                    TipoPagamento = cargaPedido.Pedido.TipoPagamento,
                    TipoTomador = cargaPedido.TipoTomador,
                    TipoRateio = cargaPedido.TipoRateio,
                    PossuiCTe = cargaPedido.PossuiCTe,
                    PossuiNFS = cargaPedido.PossuiNFS,
                    PossuiNFSManual = cargaPedido.PossuiNFSManual,
                    PalletAgrupamento = cargaPedido.Pedido.PalletAgrupamento,
                    CentroCusto = servicoCarga.ConverterObjetoCentroCustol(cargaPedido.Pedido?.PedidoCentroCusto ?? null),
                    CodigoOrdemServico = cargaPedido.Pedido?.CodigoOS.ToInt() ?? 0,
                    CargaRefrigeradaPrecisaEnergia = cargaPedido.Pedido?.NecessitaEnergiaContainerRefrigerado ?? false,
                    Container = servicoCarga.ConverterObjetoContainer(cargaPedido.Pedido?.Container ?? null),
                    ContainerADefinir = cargaPedido.Pedido?.ContainerADefinir ?? false,
                    CodigoBooking = Convert.ToInt32(cargaPedido.Pedido?.CodigoBooking ?? "0"),
                    ContemCargaPerigosa = cargaPedido.Pedido?.PossuiCargaPerigosa ?? false,
                    CubagemTotal = cargaPedido.Pedido?.CubagemTotal ?? 0,
                    ContemCargaRefrigerada = cargaPedido.Pedido?.ContemCargaRefrigerada ?? false,
                    DataFinalCarregamento = cargaPedido.Pedido?.DataPrevisaoSaida?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                    DataInicioCarregamento = cargaPedido.Pedido?.DataPrevisaoSaida?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                    DataPrevisaoEntrega = cargaPedido.Pedido?.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                    DataCarregamentoPedido = cargaPedido.Pedido?.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                    DataColeta = cargaPedido.Pedido?.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                    DescricaoCarrierNavioViagem = cargaPedido.Pedido?.DescricaoCarrierNavioViagem ?? string.Empty,
                    DescricaoTipoPropostaFeeder = cargaPedido.Pedido?.DescricaoTipoPropostaFeeder ?? string.Empty,
                    Destino = servicoEndereco.ConverterObjetoEnderecoPedido(cargaPedido.Pedido?.EnderecoDestino ?? null),
                    Embarcador = svcPessoa.ConverterObjetoPessoa(cargaPedido.Pedido?.Expedidor ?? null),
                    FormaAverbacaoCTE = cargaPedido.Pedido.FormaAverbacaoCTE,
                    NecessitaAverbacao = cargaPedido.Pedido.NecessitaAverbacaoAutomatica,
                    NumeroBL = cargaPedido.Pedido?.NumeroBL ?? string.Empty,
                    NumeroBooking = cargaPedido.Pedido?.NumeroBooking ?? string.Empty,
                    NumeroPedidoEmbarcador = cargaPedido.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                    Observacao = cargaPedido.Pedido?.Observacao ?? string.Empty,
                    ObservacaoLocalEntrega = string.Empty,
                    ObservacaoProposta = string.Empty,
                    Origem = servicoEndereco.ConverterObjetoEnderecoPedido(cargaPedido.Pedido?.EnderecoOrigem ?? null),
                    Ordem = cargaPedido.Pedido?.Ordem ?? string.Empty,
                    PercentualADValorem = cargaPedido.Pedido?.ValorAdValorem ?? 0m,
                    PesoBruto = cargaPedido?.Pedido.PesoTotal ?? 0m,
                    NumeroOrdemServico = cargaPedido.Pedido?.NumeroOS ?? string.Empty,
                    PortoDestino = servicoCarga.ConverterObjetoPorto(cargaPedido.Pedido?.PortoDestino ?? null),
                    PortoOrigem = servicoCarga.ConverterObjetoPorto(cargaPedido.Pedido?.Porto ?? null),
                    ProdutosPedido = ObterProdutoDaCargaPedido(cargaPedido),
                    ProvedorOS = svcPessoa.ConverterObjetoPessoa(cargaPedido.Pedido?.ProvedorOS ?? null),
                    QuantidadeConhecimentosTaxaDocumentacao = cargaPedido.Pedido.QuantidadeConhecimentosTaxaDocumentacao,
                    TipoDocumentoAverbacao = cargaPedido.Pedido.TipoDocumentoAverbacao,
                    QuantidadeContainerBooking = cargaPedido.Pedido?.QuantidadeContainerBooking ?? 0,
                    QuantidadeTipoContainerReserva = cargaPedido.Pedido?.QuantidadeTipoContainerReserva ?? 0,
                    QuantidadeVolumes = cargaPedido.Pedido?.QtVolumes ?? 0,
                    NumeroLacre1 = cargaPedido.Pedido?.LacreContainerUm ?? string.Empty,
                    NumeroLacre2 = cargaPedido.Pedido?.LacreContainerDois ?? string.Empty,
                    NumeroLacre3 = cargaPedido.Pedido?.LacreContainerTres ?? string.Empty,
                    TaraContainer = Convert.ToInt32(cargaPedido.Pedido?.Container?.Tara ?? 0m),
                    PropostaComercial = servicoCarga.ConverterObjetoPropostaComercial(cargaPedido?.Pedido ?? null),
                    RealizarCobrancaTaxaDocumentacao = cargaPedido.Pedido?.RealizarCobrancaTaxaDocumentacao ?? false,
                    Temperatura = cargaPedido.Pedido?.Temperatura ?? string.Empty,
                    TerminalPortoDestino = servicoCarga.ConverterObjetoTerminalPorto(cargaPedido.Pedido?.TerminalDestino ?? null),
                    TerminalPortoOrigem = servicoCarga.ConverterObjetoTerminalPorto(cargaPedido.Pedido?.TerminalOrigem ?? null),
                    TipoCargaEmbarcador = servicoTipoCarga.ConverterObjetoTipoCarga(integracaoIntercab.TipoDeCargaPadrao != null ? integracaoIntercab.TipoDeCargaPadrao : cargaPedido.Pedido?.TipoDeCarga ?? null),
                    TipoContainerReserva = servicoCarga.ConverterObjetoTipoContainer(cargaPedido.Pedido?.ContainerTipoReserva ?? null),
                    TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = integracaoIntercab?.CodigoTipoOperacao ?? string.Empty, ModalPropostaMultimodal = cargaPedido.ModalPropostaMultimodal, TipoCobrancaMultimodal = cargaPedido.TipoCobrancaMultimodal, TipoPropostaMultimodal = cargaPedido.TipoPropostaMultimodal, TipoServicoMultimodal = cargaPedido.TipoServicoMultimodal, Atualizar = false },
                    TipoPropostaFeeder = cargaPedido.Pedido.TipoPropostaFeeder,
                    Transbordo = servicoCarga.ConverterObjetoTransbordo(cargaPedido.Pedido?.Transbordos?.ToList() ?? null),
                    TransportadoraEmitentePedido = servicoEmpresa.ConverterObjetoEmpresa(cargaPedido.Pedido?.Empresa ?? null),
                    UsarOutroEnderecoDestino = cargaPedido.Pedido.UsarOutroEnderecoDestino,
                    UsarOutroEnderecoOrigem = cargaPedido.Pedido.UsarOutroEnderecoOrigem,
                    ValidarNumeroContainer = cargaPedido.Pedido.ValidarDigitoVerificadorContainer,
                    ValorDescarga = cargaPedido?.ValorDescarga ?? 0m,
                    ValorFrete = ConverterObjetoFreteValor(cargaPedido.Pedido ?? null),
                    ValorTaxaDocumento = cargaPedido?.Pedido?.ValorTaxaDocumento ?? 0m,
                    ValorTotalPaletes = cargaPedido?.Pedido?.ValorTotalPaletes ?? 0m,
                    Viagem = servicoCarga.ConverterObjetoViagem(cargaPedido.Pedido?.PedidoViagemNavio ?? null),
                    ViagemLongoCurso = servicoCarga.ConverterObjetoViagem(cargaPedido.Pedido?.PedidoViagemNavioLongoCurso ?? null),
                    TipoServicoCarga = cargaPedido?.Pedido.TipoServicoCarga
                }
            ).ToList();

            return cargaIntegracaoCompleta;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor ConverterObjetoFreteValor(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor()
            {
                FreteFilialEmissora = pedido?.ValorFreteFilialEmissora ?? 0m,
                FreteProprio = pedido?.ValorCustoFrete ?? 0m,
                ValorAReceberSemImpostoIncluso = pedido?.ValorFreteAReceber ?? 0m,
                ValorPrestacaoServico = pedido?.ValorFreteCotado ?? 0m,
                ValorTotalAReceber = pedido?.ValorTotalCarga ?? 0m,
                ComponentesAdicionais = ConverterObjetoPedidoComponente(pedido),

            };

        }

        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> ConverterObjetoPedidoComponente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(_unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete servicoComponenteFrete = new Embarcador.Frete.ComponenteFrete(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> listCargaPedidoComponente = repCargaPedidoComponenteFrete.BuscarPorPedido(pedido?.Codigo ?? 0);

            if (listCargaPedidoComponente == null)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> listaComponente = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete pedidoComponent in listCargaPedidoComponente)
                listaComponente.Add(servicoComponenteFrete.ConverterObjetoComponentePedido(pedidoComponent));

            return listaComponente;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CriarCargaPedidoPorDocumentoTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.Stage stage, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario> cargaPedidoXmlTemporario, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.CTe serCTe = new Embarcador.Carga.CTe(unitOfWork);
            Servicos.Embarcador.NFe.NFe serNFe = new Embarcador.NFe.NFe(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal servicoNota = new Embarcador.Pedido.NotaFiscal(unitOfWork);

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            if (!(carga?.TipoOperacao?.ConfiguracaoMontagemCarga?.ExibirPedidosMontagemIntegracao ?? false))
                pedido.PedidoTotalmenteCarregado = true;

            if (carga?.TipoOperacao?.ConfiguracaoCarga?.ConsiderarKMRecibidoDoEmbarcador ?? false)
                pedido.Distancia = stage.Distancia;

            if ((carga?.TipoOperacao?.ConfiguracaoMontagemCarga?.DisponibilizarPedidosMontagemDeterminadosTransportadores ?? false) && ((carga?.TipoOperacao?.ConfiguracaoMontagemCarga?.TransportadoresMontagemCarga?.Count() ?? 0) > 0))
            {
                if (carga.TipoOperacao.ConfiguracaoMontagemCarga.TransportadoresMontagemCarga.Any(obj => obj.Codigo == carga.Empresa.Codigo))
                    pedido.PedidoTotalmenteCarregado = false;
            }

            repPedido.Atualizar(pedido);

            if (configuracaoTMS.SistemaIntegracaoPadraoCarga > 0)
            {
                Servicos.Embarcador.Integracao.IntegracaoCarga serIntegracaoCarga = new Embarcador.Integracao.IntegracaoCarga(unitOfWork);
                serIntegracaoCarga.InformarIntegracaoCarga(carga, configuracaoTMS.SistemaIntegracaoPadraoCarga, unitOfWork);
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = new Dominio.Entidades.Embarcador.Cargas.CargaPedido();
            cargaPedido.Carga = carga != null && carga.CargaAgrupamento != null ? carga.CargaAgrupamento : carga;
            cargaPedido.CargaOrigem = carga;
            cargaPedido.Pedido = pedido;
            cargaPedido.Ativo = true;
            cargaPedido.ValorFrete = 0;

            //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
            Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {pedido.PesoTotal}. Carga.CriarCargaPedidoPorDocumentoTransporte", "PesoCargaPedido");
            cargaPedido.Peso = pedido.PesoTotal;
            cargaPedido.PesoLiquido = pedido.PesoLiquidoTotal;

            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao && carga.TipoOperacao.ModeloDocumentoFiscal != null)
                cargaPedido.ModeloDocumentoFiscal = carga.TipoOperacao.ModeloDocumentoFiscal;

            repCargaPedido.Inserir(cargaPedido);

            if (cargaPedidoXmlTemporario?.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario> cargaPedidoXmlTemporarioPedido = cargaPedidoXmlTemporario.Where(obj => obj.Pedido?.Protocolo == cargaPedido.Pedido.Protocolo).ToList();
                foreach (CargaPedidoXMLNotaFiscalTemporario temp in cargaPedidoXmlTemporarioPedido)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial existeNotaParcialComNumeroFatura = repCargaPedidoParcial.BuscarPorNumeroFatura(temp.NumeroFatura);
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = repXMLNotaFiscal.BuscarPorChave(temp.Chave ?? string.Empty);

                    if (existeNotaParcialComNumeroFatura != null)
                    {
                        existeNotaParcialComNumeroFatura.Chave = temp.Chave;
                        existeNotaParcialComNumeroFatura.Status = temp.Status;
                        existeNotaParcialComNumeroFatura.NumeroFatura = temp.NumeroFatura;
                        existeNotaParcialComNumeroFatura.TipoNotaFiscalIntegrada = temp.TipoNotaFiscalIntegrada;
                        existeNotaParcialComNumeroFatura.CargaPedido = cargaPedido;

                        if (notaFiscal != null)
                        {
                            existeNotaParcialComNumeroFatura.XMLNotaFiscal = notaFiscal;
                            servicoNota.InserirNotaCargaPedido(notaFiscal, cargaPedido, _tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoCarga, _auditado);
                        }

                        repCargaPedidoParcial.Atualizar(existeNotaParcialComNumeroFatura);

                        Servicos.Log.TratarErro($"Atualizou CargaPedidoParcial ao Criar Carga Pedido: - {(existeNotaParcialComNumeroFatura.Codigo)} | Chave: {existeNotaParcialComNumeroFatura.Chave} ", "VincularNotaFiscal");

                        continue;
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoParcial = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial()
                    {
                        Status = temp.Status,
                        CargaPedido = cargaPedido,
                        NumeroFatura = temp.NumeroFatura ?? string.Empty,
                        TipoNotaFiscalIntegrada = temp.TipoNotaFiscalIntegrada,
                        Chave = temp?.Chave ?? string.Empty,
                    };
                    repCargaPedidoParcial.Inserir(cargaPedidoParcial);

                    if (notaFiscal != null)
                        servicoNota.InserirNotaCargaPedido(notaFiscal, cargaPedido, _tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoCarga, _auditado);

                    repCargaPedidoParcial.Atualizar(cargaPedidoParcial);

                    Servicos.Log.TratarErro($"Inseriu CargaPedidoParcial ao criar Carga Pedido: - {(cargaPedidoParcial.Codigo)} | Chave: {cargaPedidoParcial.Chave} ", "VincularNotaFiscal");
                }
            }

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota in pedido.NotasFiscais)
                servicoNota.InserirNotaCargaPedido(nota, cargaPedido, tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoCarga);

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                List<Dominio.Entidades.Cliente> clientesBloquearEmissaoDosDestinatarios = carga.TipoOperacao?.ClientesBloquearEmissaoDosDestinatario.ToList();
                serCargaPedido.BuscarConfiguracoesMultimodal(carga, unitOfWork, ref cargaPedido, integracaoIntercab, clientesBloquearEmissaoDosDestinatarios);
            }

            cargaPedido.TipoRateio = serCTe.BuscarTipoEmissaoDocumentosCTe(cargaPedido, tipoServicoMultisoftware, unitOfWork);

            if (cargaPedido.FormulaRateio == null && (carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false))
                cargaPedido.FormulaRateio = carga.TipoOperacao.RateioFormula;

            cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.Nova;

            Dominio.Entidades.Localidade origem = pedido.Origem;
            Dominio.Entidades.Localidade destino = pedido.Destino;

            if (pedido.Tomador != null)
                cargaPedido.Tomador = pedido.Tomador;

            if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.NaoConsolida && stage != null)
            {
                cargaPedido.Recebedor = stage.Recebedor;
                cargaPedido.Expedidor = stage.Expedidor;

                if (cargaPedido.Recebedor != null && cargaPedido.Expedidor == null)
                    cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;

                if (cargaPedido.Recebedor == null && cargaPedido.Expedidor != null)
                    cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;

                if (cargaPedido.Recebedor != null && cargaPedido.Expedidor != null)
                    cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
            }

            cargaPedido.CanalEntrega = stage?.CanalEntrega ?? pedido.CanalEntrega;
            cargaPedido.CanalVenda = stage?.CanalVenda ?? pedido.CanalVenda;

            if (cargaPedido.Recebedor == null && pedido.Recebedor != null && cargaPedido.Carga.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.PreCheckIn)
            {
                cargaPedido.Recebedor = pedido.Recebedor;
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                if (!(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                    destino = cargaPedido.Recebedor.Localidade;

                if ((cargaPedido.Carga.TipoOperacao?.UsarRecebedorComoPontoPartidaCarga ?? false) && cargaPedido.Recebedor != null)
                {
                    cargaPedido.PontoPartida = cargaPedido.Recebedor;
                    cargaPedido.PossuiColetaEquipamentoPontoPartida = true;
                }
                else
                {
                    cargaPedido.PontoPartida = pedido.PontoPartida;
                    cargaPedido.PossuiColetaEquipamentoPontoPartida = false;
                }
            }

            if (cargaPedido.Recebedor == null && !cargaPedido.AgInformarRecebedor && cargaPedido.Pedido != null && cargaPedido.Pedido.Destinatario != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoa(cargaPedido.Pedido.Destinatario.CPF_CNPJ);
                if (clienteDescarga != null && clienteDescarga.Distribuidor != null)
                {
                    double cnpj = 0;
                    double.TryParse(clienteDescarga.Distribuidor.CNPJ_SemFormato, out cnpj);
                    cargaPedido.Recebedor = repCliente.BuscarPorCPFCNPJ(cnpj);
                    if (cargaPedido.Recebedor != null)
                    {
                        destino = cargaPedido.Recebedor.Localidade;
                        cargaPedido.PendenteGerarCargaDistribuidor = true;
                        cargaPedido.Carga.PendenteGerarCargaDistribuidor = true;
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                        repCarga.Atualizar(cargaPedido.Carga);
                    }
                }
            }

            if (cargaPedido.Expedidor == null && pedido.Expedidor != null)
            {
                cargaPedido.Expedidor = pedido.Expedidor;
                origem = cargaPedido.Expedidor.Localidade;
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;

                if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarExpedidorComoTransportador && cargaPedido.Carga.Empresa == null)
                    cargaPedido.Carga.Empresa = repEmpresa.BuscarPorCNPJ(cargaPedido.Expedidor.CPF_CNPJ_SemFormato);
            }

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarExpedidorComoTransportador && cargaPedido.Expedidor == null && cargaPedido.Carga.Empresa != null)
            {
                cargaPedido.Expedidor = repCliente.BuscarPorCPFCNPJ(double.Parse(cargaPedido.Carga.Empresa.CNPJ_SemFormato));

                if (cargaPedido.Expedidor == null)
                {
                    Servicos.Cliente serCliente = new Cliente(unitOfWork);
                    Servicos.WebService.Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoaExp = serPessoa.ConverterObjetoEmpresa(cargaPedido.Carga.Empresa);
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(pessoaExp, "Expedidor", unitOfWork, 0, false);
                    if (retorno.Status == true)
                        cargaPedido.Expedidor = retorno.cliente;
                }

                if (cargaPedido.Expedidor != null)
                {
                    cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
                    origem = cargaPedido.Expedidor.Localidade;
                }
            }

            if (cargaPedido.Expedidor == null && cargaPedido.Carga.Rota != null && cargaPedido.Carga.Rota.ExpedidorPedidosDiferenteOrigemRota != null)
            {
                if (cargaPedido.Carga.Rota.LocalidadesOrigem?.Count() == 1 && pedido.Origem?.Codigo != cargaPedido.Carga.Rota.LocalidadesOrigem.FirstOrDefault().Codigo)
                {
                    cargaPedido.Expedidor = cargaPedido.Carga.Rota.ExpedidorPedidosDiferenteOrigemRota;
                    cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
                    origem = cargaPedido.Carga.Rota.ExpedidorPedidosDiferenteOrigemRota.Localidade;
                }
            }

            if (cargaPedido.Expedidor == null && cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.Expedidor != null)
            {
                cargaPedido.Expedidor = cargaPedido.Carga.TipoOperacao.Expedidor;
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
                origem = cargaPedido.Expedidor.Localidade;
            }

            if (cargaPedido.Recebedor == null && cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.Recebedor != null)
            {
                cargaPedido.Recebedor = cargaPedido.Carga.TipoOperacao.Recebedor;
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                destino = cargaPedido.Recebedor.Localidade;
            }

            if (cargaPedido.Recebedor == null && cargaPedido.Carga.Rota != null && cargaPedido.Carga.Rota.Distribuidor != null && cargaPedido.Carga.Rota.GerarRedespachoAutomaticamente)
            {
                cargaPedido.Recebedor = cargaPedido.Carga.Rota.Distribuidor;
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
                destino = cargaPedido.Recebedor.Localidade;
            }

            if (origem != null)
            {
                if (cargaPedido.Origem == null || (cargaPedido.Origem.Codigo != origem.Codigo))
                {
                    cargaPedido.Origem = origem;
                }
            }

            if (destino != null)
            {
                if ((cargaPedido.Destino == null || (cargaPedido.Destino.Codigo != destino.Codigo)) && !cargaPedido.AgInformarRecebedor)
                {
                    cargaPedido.Destino = destino;
                }
            }

            if (cargaPedido.Recebedor != null && cargaPedido.Expedidor != null)
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;

            cargaPedido.TipoTomador = pedido.TipoTomador;
            if (pedido.UsarTipoTomadorPedido || configuracaoWebService.SempreUtilizarTomadorEnviadoNoPedido)
            {
                cargaPedido.Pedido.UsarTipoTomadorPedido = true;
                repPedido.Atualizar(cargaPedido.Pedido);
            }

            if (!cargaPedido.Pedido.UsarTipoTomadorPedido)
            {
                Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = Servicos.Embarcador.Pedido.RegraTomador.BuscarRegraTomador(cargaPedido, tipoServicoMultisoftware, unitOfWork);

                if (regraTomador != null)
                {
                    cargaPedido.TipoTomador = regraTomador.TipoTomador;
                    cargaPedido.Tomador = regraTomador.Tomador;
                    cargaPedido.RegraTomador = regraTomador;
                    pedido.UsarTipoPagamentoNF = false;

                    if (regraTomador.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                    else if (regraTomador.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                    else
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

                    repPedido.Atualizar(pedido);
                }
                else
                    cargaPedido.RegraTomador = null;
            }
            else
            {
                cargaPedido.TipoTomador = cargaPedido.Pedido.TipoTomador;
                if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    cargaPedido.Tomador = cargaPedido.Pedido.Tomador;
                else
                    cargaPedido.Tomador = null;
            }

            if (pedido.OrdemColetaProgramada > 0)
            {
                cargaPedido.OrdemColeta = pedido.OrdemColetaProgramada;
                carga.OrdemRoteirizacaoDefinida = true;
            }

            serCargaPedido.VerificarFilialEmissaoCargaPedido(cargaPedido, configuracaoGeralCarga);

            Servicos.Embarcador.Carga.Carga.CalcularValorDescargaPorCargaPedido(cargaPedido, configuracaoTMS, unitOfWork);

            repCargaPedido.Atualizar(cargaPedido);

            if (carga.TipoOperacao != null && carga.TipoOperacao.FretePorContadoCliente)
            {
                carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente;
                repCarga.Atualizar(carga);
            }

            if (cargaPedido.Redespacho && cargaPedido.CargaPedidoTrechoAnterior != null)
            {
                cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoProximoTrecho = cargaPedido;
                repCargaPedido.Atualizar(cargaPedido.CargaPedidoTrechoAnterior);
                serNFe.VincularNotasDeRedespachoVinculadas(cargaPedido, cargaPedido.CargaPedidoTrechoAnterior, unitOfWork, tipoServicoMultisoftware, auditado);
            }
            else if (cargaPedido.CargaPedidoProximoTrecho != null && cargaPedido.CargaPedidoProximoTrecho.Redespacho)
            {
                cargaPedido.CargaPedidoProximoTrecho.CargaPedidoTrechoAnterior = cargaPedido;
                repCargaPedido.Atualizar(cargaPedido.CargaPedidoProximoTrecho);
                serNFe.VincularNotasDeRedespachoVinculadas(cargaPedido, cargaPedido.CargaPedidoProximoTrecho, unitOfWork, tipoServicoMultisoftware, auditado);
            }

            bool possuiCTe = false;
            bool possuiNFS = false;
            bool possuiNFSManual = false;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal = null;

            serCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, cargaPedido.Origem, cargaPedido.Destino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoFiscalIntramunicipal, configuracaoTMS, out bool sempreDisponibilizarDocumentoNFSManual, true);

            cargaPedido.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;
            cargaPedido.PossuiCTe = possuiCTe;
            cargaPedido.PossuiNFS = possuiNFS;
            cargaPedido.PossuiNFSManual = possuiNFSManual;

            if (cargaPedido.PossuiNFSManual && cargaPedido.CargaPedidoFilialEmissora)
                cargaPedido.CargaPedidoFilialEmissora = false;

            cargaPedido.ModeloDocumentoFiscalIntramunicipal = modeloDocumentoFiscalIntramunicipal;

            if (!(carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && ValidarNotasPedidoEnviada(cargaPedido, unitOfWork))
                cargaPedido.SituacaoEmissao = SituacaoNF.NFEnviada;

            return cargaPedido;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            Retorno<bool> retorno = new Retorno<bool>();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            _unitOfWork.Start();

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            if (!configuracao.BuscarPorCargaPedidoCargasPendentesIntegracao)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);

                if (carga == null)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "A carga informada não existe no Multi Embarcador";
                    return retorno;
                }
                if (carga.CargaIntegradaEmbarcador)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                    retorno.Mensagem = "A carga já teve sua confirmação de integração realizada anteriormente";
                    return retorno;
                }

                carga.CargaIntegradaEmbarcador = true;
                repCarga.Atualizar(carga);
                retorno.Objeto = true;
                retorno.Status = true;
                retorno.Mensagem = "Confirmação da integração da carga feita com sucesso";

                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, "Confirmou integração da carga.", _unitOfWork);

                _unitOfWork.CommitChanges();
                return retorno;

            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPorProtocoloCargaEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

            if (cargaPedido == null)
            {
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "O pedido informado não existe no Multi Embarcador";
                return retorno;
            }

            if (cargaPedido.CargaPedidoIntegrada)
            {
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                retorno.Mensagem = "O pedido já teve sua confirmação de integração realizada anteriormente";
                return retorno;
            }

            cargaPedido.CargaPedidoIntegrada = true;
            repCargaPedido.Atualizar(cargaPedido);
            retorno.Objeto = true;
            retorno.Status = true;
            retorno.Mensagem = "Confirmação da integração da carga feita com sucesso";

            Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido.Carga, "Confirmou integração do pedido " + cargaPedido.Pedido.NumeroPedidoEmbarcador + ".", _unitOfWork);
            _unitOfWork.CommitChanges();
            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoDeEnvioProgramado(Dominio.ObjetosDeValor.WebService.ConfirmarIntegracaoDeEnvioProgramado confirmarIntegracaoDeEnvioProgramado)
        {
            Retorno<bool> retorno = new Retorno<bool>();

            Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);
            Servicos.Embarcador.Integracao.ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new Servicos.Embarcador.Integracao.ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            _unitOfWork.Start();

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado = repositorioIntegracaoEnvioProgramado.BuscarPorCodigoCTe(confirmarIntegracaoDeEnvioProgramado.ProtocoloCTe);

            if (integracaoEnvioProgramado == null)
            {
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = $"A carga {confirmarIntegracaoDeEnvioProgramado.DtCargoNumber ?? ""} informada, não existe no Multi Embarcador (The reported cargo {confirmarIntegracaoDeEnvioProgramado.DtCargoNumber ?? ""} does not exist in Multi Embarcador)";
                return retorno;
            }

            integracaoEnvioProgramado.ProblemaIntegracao = confirmarIntegracaoDeEnvioProgramado.Message;
            integracaoEnvioProgramado.StepIntegracao = confirmarIntegracaoDeEnvioProgramado.Step;

            if (confirmarIntegracaoDeEnvioProgramado.MessageCode == 200)
            {
                integracaoEnvioProgramado.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracaoEnvioProgramado.ProblemaIntegracao = "Integração Confirmada";

                retorno.Mensagem = confirmarIntegracaoDeEnvioProgramado.Message;

                Servicos.Auditoria.Auditoria.Auditar(_auditado, integracaoEnvioProgramado, "Confirmou integração do CT-e.", _unitOfWork);
            }
            else
            {

                integracaoEnvioProgramado.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                retorno.Mensagem = string.IsNullOrEmpty(confirmarIntegracaoDeEnvioProgramado.Message) ? $"Falha ao Integrar. MessageCode:{confirmarIntegracaoDeEnvioProgramado.MessageCode}" : confirmarIntegracaoDeEnvioProgramado.Message;

                Servicos.Auditoria.Auditoria.Auditar(_auditado, integracaoEnvioProgramado, "Informou erro na integração do CT-e.", _unitOfWork);
            }

            string jsonDadosRequisicao = confirmarIntegracaoDeEnvioProgramado.ToJson();
            string jsonDadosRetorno = retorno.Mensagem.ToJson();

            servicoArquivoTransacao.Adicionar(integracaoEnvioProgramado, jsonDadosRequisicao, jsonDadosRetorno, "json", retorno.Mensagem);
            repositorioIntegracaoEnvioProgramado.Atualizar(integracaoEnvioProgramado);

            _unitOfWork.CommitChanges();

            retorno.Objeto = true;
            retorno.Status = true;
            retorno.CodigoMensagem = 000;
            return retorno;
        }

        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>> BuscarCargasPendentesIntegracaoAsync(RequestCargasIntegracaoPendentes requestCargasIntegracaoPendentes, Dominio.Entidades.WebService.Integradora integradora)
        {
            Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>();

            retorno.Mensagem = "";
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            if (requestCargasIntegracaoPendentes.Limite > 1000)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "O limite não pode ser maior que 1000";
                return retorno;
            }

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = await repositorioConfiguracaoWebService.BuscarPrimeiroRegistroAsync();

            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();
            retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();

            DateTime dataFinalizacaoEmissao = configuracao.TempoHorasParaRetornoCTeAposFinalizacaoEmissao > 0 ? DateTime.Now.AddHours(-configuracao.TempoHorasParaRetornoCTeAposFinalizacaoEmissao) : DateTime.MinValue;

            List<int> codigosCargaPedidos;
            bool filtrarPorSituacao = !(configuracaoWebService?.RetornarCargasEmQualquerEtapaNoMetodoBuscarCargaPendenteIntegracao ?? false);

            if (configuracao.BuscarPorCargaPedidoCargasPendentesIntegracao)
            {
                retorno.Objeto.NumeroTotalDeRegistro = await repCargaPedido.ContarCargasPedidoAgIntegracaoEmbarcadorAsync(configuracao.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais, dataFinalizacaoEmissao, integradora?.Codigo ?? 0, integradora?.GrupoPessoas?.Codigo ?? 0, integradora?.Empresa?.Codigo ?? 0, integradora?.Clientes?.Select(c => c.CPF_CNPJ).ToList(), requestCargasIntegracaoPendentes.CodigoIntegracaoTipoOperacao, requestCargasIntegracaoPendentes.CodigoIntegracaoFilial, filtrarPorSituacao);

                if (retorno.Objeto.NumeroTotalDeRegistro == 0)
                {
                    retorno.Status = true;
                    retorno.Mensagem = "Nenhuma integração pendente encontrada.";

                    return retorno;
                }

                codigosCargaPedidos = await repCargaPedido.BuscarCargasPedidoAgIntegracaoEmbarcadorAsync(configuracao.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais, dataFinalizacaoEmissao, integradora?.Codigo ?? 0, integradora?.GrupoPessoas?.Codigo ?? 0, integradora?.Empresa?.Codigo ?? 0, integradora?.Clientes?.Select(c => c.CPF_CNPJ).ToList(), requestCargasIntegracaoPendentes.Inicio, requestCargasIntegracaoPendentes.Limite, requestCargasIntegracaoPendentes.CodigoIntegracaoTipoOperacao, requestCargasIntegracaoPendentes.CodigoIntegracaoFilial, filtrarPorSituacao);
            }
            else
            {
                retorno.Objeto.NumeroTotalDeRegistro = await repCargaPedido.ContarCargasAgIntegracaoEmbarcadorAsync(configuracao.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais, dataFinalizacaoEmissao, integradora?.Codigo ?? 0, integradora?.GrupoPessoas?.Codigo ?? 0, integradora?.Empresa?.Codigo ?? 0, requestCargasIntegracaoPendentes.CodigoIntegracaoTipoOperacao, requestCargasIntegracaoPendentes.CodigoIntegracaoFilial, filtrarPorSituacao);

                if (retorno.Objeto.NumeroTotalDeRegistro == 0)
                {
                    retorno.Status = true;
                    retorno.Mensagem = "Nenhuma integração pendente encontrada.";

                    return retorno;
                }

                codigosCargaPedidos = await repCargaPedido.BuscarCargasAgIntegracaoEmbarcadorAsync(configuracao.RetornarCargasPentendesIntegracaoSomenteParaIntegradoraNotasFiscais, dataFinalizacaoEmissao, integradora?.Codigo ?? 0, integradora?.GrupoPessoas?.Codigo ?? 0, integradora?.Empresa?.Codigo ?? 0, requestCargasIntegracaoPendentes.Inicio, requestCargasIntegracaoPendentes.Limite, requestCargasIntegracaoPendentes.CodigoIntegracaoTipoOperacao, requestCargasIntegracaoPendentes.CodigoIntegracaoFilial, filtrarPorSituacao);
            }

            List<(int ProtocoloCargaOrigem, int ProtocoloPedido)> protocolos = await repCargaPedido.BuscarProtocolosPorCodigosAsync(codigosCargaPedidos);

            for (int i = 0; i < protocolos.Count; i++)
            {
                Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos();
                protocolo.protocoloIntegracaoCarga = protocolos[i].ProtocoloCargaOrigem;
                protocolo.protocoloIntegracaoPedido = protocolos[i].ProtocoloPedido;
                retorno.Objeto.Itens.Add(protocolo);
            }

            retorno.Status = true;
            await Servicos.Auditoria.Auditoria.AuditarConsultaAsync(_auditado, "Buscou cargas pendentes de integração", _unitOfWork);

            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>> BuscarModelosVeicularesPendentesIntegracao(int quantidade)
        {
            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> repositorioIntegracaoModeloVeicular = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(_unitOfWork);
            Servicos.WebService.Carga.ModeloVeicularCarga servicoModeloVeicularCarga = new ModeloVeicularCarga(_unitOfWork);

            int totalRegistrosPentendeIntegracao = repositorioIntegracaoModeloVeicular.ContarRegistroPendenteIntegracao();

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular> retorno = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>(),
                NumeroTotalDeRegistro = totalRegistrosPentendeIntegracao
            };

            if (totalRegistrosPentendeIntegracao == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>>.CriarRetornoSucesso(retorno);

            IList<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> listaModelosVeicularesPendenteIntegracao = repositorioIntegracaoModeloVeicular.BuscarRegitrosPendenteIntegracao(quantidade);

            foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular in listaModelosVeicularesPendenteIntegracao)
                retorno.Itens.Add(new ModeloVeicular()
                {
                    Protocolo = modeloVeicular.Codigo,
                    Descricao = modeloVeicular.Descricao,
                    CodigoIntegracao = modeloVeicular.CodigoIntegracao,
                    TipoModeloVeicular = modeloVeicular.Tipo,
                    QuantidadeExtraExcedenteTolerado = modeloVeicular.ToleranciaPesoExtra,
                    DivisaoCapacidade = servicoModeloVeicularCarga.ConverterObjetoListaDivisaoCapacidade(modeloVeicular)
                });

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>>.CriarRetornoSucesso(retorno);

        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoModeloVeicular(List<int> protocolos)
        {
            if (protocolos == null || protocolos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Sem Protocolo de Modelo Veicular para Confirmar.");

            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> repositorioModeloVeicularCarga = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(_unitOfWork);

            IList<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> listaModelos = repositorioModeloVeicularCarga.BuscarRegitrosPendentesIntegracaoPeloProtocolos(protocolos);
            List<int> protocolosNaoProcessados = protocolos.Where(c => !listaModelos.Any(m => m.Codigo == c)).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular in listaModelos)
            {
                modeloVeicular.IntegradoERP = true;
                repositorioModeloVeicularCarga.Atualizar(modeloVeicular);
            }

            if (protocolosNaoProcessados != null && protocolosNaoProcessados.Count > 0)
                return Retorno<bool>.CriarRetornoSucesso(true, $"O(s) protocolo(s) {string.Join(",", protocolosNaoProcessados)} não foram confirmados porque não foi achados registros existentes.");

            return Retorno<bool>.CriarRetornoSucesso(true, "Todos os protocolos confirmados com sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarDadosTransporteCarga(Dominio.ObjetosDeValor.WebService.Carga.DadosTransporte dadosTransporte, Dominio.Entidades.WebService.Integradora integradora)
        {
            Servicos.Log.GravarDebug($"{(dadosTransporte != null ? Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte) : string.Empty)}", "InformarDadosTransporteCarga");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            Dominio.ObjetosDeValor.WebService.Retorno<bool> retornoBool = null;

            try
            {
                if (dadosTransporte == null)
                    throw new WebServiceException("É obrigatório informar os dados do container.");

                Servicos.WebService.Empresa.Motorista serMotorista = new Servicos.WebService.Empresa.Motorista(_unitOfWork);
                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

                if (dadosTransporte.ProtocoloCarga > 0)
                    carga = repCarga.BuscarPorProtocolo(dadosTransporte.ProtocoloCarga);
                else if (!string.IsNullOrWhiteSpace(dadosTransporte.NumeroCarga))
                    carga = repCarga.BuscarPorCodigoCargaEmbarcador(dadosTransporte.NumeroCarga, true);

                if (carga == null)
                    carga = repCarga.BuscarPorCodigoCargaEmbarcador(dadosTransporte.NumeroCarga, false);

                if (carga == null)
                    carga = repCarga.BuscarPorCodigoCargaEmbarcadorSemAgrupamento(dadosTransporte.NumeroCarga);

                if (carga == null & !string.IsNullOrEmpty(dadosTransporte.NumeroOrdem))
                    carga = repPedido.BuscarCargaPorNumeroOrdemPedido(dadosTransporte.NumeroOrdem);

                if (carga == null && string.IsNullOrWhiteSpace(dadosTransporte.NumeroCarga))
                {
                    throw new WebServiceException("Precisa Informar o número da carga");
                }

                if (carga == null)
                    throw new WebServiceException("Não foi localizada uma carga compatível com os dados informados na integração, por favor verifique.");

                _unitOfWork.Start();

                carga.Initialize();

                List<SituacaoCarga> situacaoesPermitidas = new List<SituacaoCarga>() {
                        SituacaoCarga.Nova,
                        SituacaoCarga.AgNFe,
                        SituacaoCarga.CalculoFrete,
                        SituacaoCarga.EmTransporte,
                        SituacaoCarga.CalculoFrete,
                        SituacaoCarga.AgNFe,
                        SituacaoCarga.PendeciaDocumentos
                        };

                if (!situacaoesPermitidas.Contains(carga.SituacaoCarga) && !(carga.TipoOperacao?.ConfiguracaoCarga?.ReceberAtualizacaoDeTransporteEmQualquerSituacaoCarga ?? false))
                    throw new WebServiceException("A atual situação da carga não permite atualização dos dados de transporte");

                if (carga.SituacaoCarga == SituacaoCarga.Cancelada || carga.SituacaoCarga == SituacaoCarga.Anulada)
                    throw new WebServiceException("A atual situação da carga (cancelada ou Anulada) não permite atualização dos dados de transporte");

                if (configuracaoWebService.AoSalvarDocumentoTransporteValidarSituacaoCarga)
                {
                    List<SituacaoCarga> situacoesPermitidaConfiguracao = new List<SituacaoCarga>()
                    {
                        SituacaoCarga.Nova,
                        SituacaoCarga.AgNFe,
                        SituacaoCarga.AgTransportador,
                    };
                    if (!situacoesPermitidaConfiguracao.Contains(carga.SituacaoCarga) && !(carga.TipoOperacao?.ConfiguracaoCarga?.ReceberAtualizacaoDeTransporteEmQualquerSituacaoCarga ?? false))
                        throw new WebServiceException("A atual situação da carga não permite atualização dos dados de transporte");
                }

                List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();
                Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte();

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckin tipoCheckin = TipoCheckinHelper.ObterTipoPorDescricao(dadosTransporte.TipoCheckin);

                bool descarregamentoOuContingenciaDescarregamento = (
                    tipoCheckin == TipoCheckin.Descarregamento ||
                    tipoCheckin == TipoCheckin.Contingenciadescarregamento
                    );

                bool dadosTransporteInformados = (
                       (carga.TipoDeCarga != null) &&
                       (carga.ModeloVeicularCarga != null) &&
                       (carga.Veiculo != null) &&
                       (!(carga.TipoOperacao?.ExigePlacaTracao ?? false) || (carga.VeiculosVinculados.Count == carga.ModeloVeicularCarga.NumeroReboques))
                   );

                if (descarregamentoOuContingenciaDescarregamento && dadosTransporteInformados)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repCargaEntrega.BuscarPorCarga(carga.Codigo);

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargasEntrega.Where(ce => !ce.Coleta).FirstOrDefault();

                    if (cargaEntrega == null)
                        throw new WebServiceException("Entrega não localizada para check-in de descarregamento");

                    cargaEntrega.DataEntradaRaio = DateTime.Now;

                    retornoBool = Retorno<bool>.CriarRetornoSucesso(true);
                    Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, true, $"Check-in do tipo descarregamento gerado em {cargaEntrega.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm")}", $"{carga.CodigoCargaEmbarcador}", Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Newtonsoft.Json.JsonConvert.SerializeObject(retornoBool), "json", _unitOfWork, carga, SituacaoIntegracao.Integrado);

                    _unitOfWork.CommitChanges();

                    return retornoBool;
                }

                if (configuracaoTMS.PermitirAtualizarModeloVeicularCargaDoVeiculoNoWebService && dadosTransporte?.Veiculo?.ModeloVeicular != null && dadosTransporte.Veiculo.ModeloVeicular.CodigoIntegracao != carga.ModeloVeicularCarga?.CodigoIntegracao)
                {
                    Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculcarCarga = repModeloVeicularCarga.buscarPorCodigoIntegracao(dadosTransporte.Veiculo.ModeloVeicular.CodigoIntegracao);

                    if (modeloVeiculcarCarga != null)
                    {
                        carga.ModeloVeicularCarga = modeloVeiculcarCarga;
                        repCarga.Atualizar(carga);
                    }
                    else
                        throw new WebServiceException($"Não foi localizado o Modelo Veicular: {dadosTransporte.Veiculo.ModeloVeicular.CodigoIntegracao}");
                }

                string retornoVeiculo = ProcessarVeiculosDadosTransporte(dadosTransporte, carga, dadosTrans, configuracaoWebService);
                if (!string.IsNullOrEmpty(retornoVeiculo))
                {
                    servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.CargaSemVeiculoInformado, "Carga sem veículo informado.");
                    throw new WebServiceException(retornoVeiculo);
                }
                else if (carga.Veiculo != null)
                    servicoMensagemAlerta.Confirmar(carga, TipoMensagemAlerta.CargaSemVeiculoInformado);

                dadosTrans.Carga = carga;
                dadosTrans.Carga.TipoCheckin = tipoCheckin;
                dadosTrans.Carga.NumeroEixosCheckin = dadosTransporte.EixosNocheckin;

                if (!string.IsNullOrWhiteSpace(dadosTransporte.DataAgendamento))
                {
                    DateTime? dataAgendamento = dadosTransporte.DataAgendamento.ToNullableDateTime();

                    if (!dataAgendamento.HasValue)
                        throw new WebServiceException("A Data de Agendamento não está em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                    else
                        dadosTrans.Carga.DataAgendamentoCarga = dataAgendamento;
                }
                if (!string.IsNullOrWhiteSpace(dadosTransporte.DataCarregamento))
                {
                    DateTime? dataCarregamento = dadosTransporte.DataCarregamento.ToNullableDateTime();

                    if (!dataCarregamento.HasValue)
                        throw new WebServiceException("A Data de Carregamento não está em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                    else
                        AtualizarDadosCarregamento(dadosTrans, dataCarregamento.Value, configuracaoTMS);
                }

                if (!string.IsNullOrWhiteSpace(dadosTransporte.DataPrevisaoInicioViagem))
                {
                    DateTime? dataPrevisaoInicioViagem = dadosTransporte.DataPrevisaoInicioViagem.ToNullableDateTime();

                    if (!dataPrevisaoInicioViagem.HasValue)
                        throw new WebServiceException("A Data Previsão Inicio Viagem não está em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                    else
                        dadosTrans.Carga.DataInicioViagemPrevista = dataPrevisaoInicioViagem;
                }

                if (!string.IsNullOrWhiteSpace(dadosTransporte.PrevisaoStopTracking))
                {
                    DateTime? dataPrevisaoStopTracking = dadosTransporte.PrevisaoStopTracking.ToNullableDateTime();

                    if (!dataPrevisaoStopTracking.HasValue)
                        throw new WebServiceException("A Data Previsão Stop Tracking não está em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                    else
                        dadosTrans.Carga.DataPrevisaoStopTracking = dataPrevisaoStopTracking;
                }

                if (!string.IsNullOrWhiteSpace(dadosTransporte.PrevisaoTerminoViagem))
                {
                    DateTime? dataPrevisaoTerminoViagem = dadosTransporte.PrevisaoTerminoViagem.ToNullableDateTime();

                    if (!dataPrevisaoTerminoViagem.HasValue)
                        throw new WebServiceException("A Data Previsão Termino Viagem não está em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                    else
                        dadosTrans.Carga.DataPrevisaoTerminoViagem = dataPrevisaoTerminoViagem;
                }

                dadosTrans.Carga.NumeroAgendamento = dadosTransporte.NumeroAgendamento;
                dadosTrans.CodigoEmpresa = carga.Empresa?.Codigo ?? (!string.IsNullOrWhiteSpace(dadosTransporte.Transportador?.CNPJ) ? repEmpresa.BuscarCodigoPorCNPJ(dadosTransporte.Transportador.CNPJ.ObterSomenteNumeros()) : 0);
                dadosTrans.CodigoTipoCarga = carga?.TipoDeCarga?.Codigo ?? 0;

                if (!string.IsNullOrWhiteSpace(dadosTransporte.DataDescarregamento))
                {
                    DateTime? dataDescarregamento = dadosTransporte.DataDescarregamento.ToNullableDateTime();

                    if (!dataDescarregamento.HasValue)
                        throw new WebServiceException("A Data de Descarregamento não está em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                    else
                        dadosTrans.Carga.DataDescarregamentoCarga = dataDescarregamento;
                }

                if (dadosTransporte.Motoristas?.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao in dadosTransporte.Motoristas)
                    {
                        Dominio.Entidades.Usuario motorista = null;

                        if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            motorista = repMotorista.BuscarMotoristaPorCPF(Utilidades.String.OnlyNumbers(motoristaIntegracao.CPF));
                        else if (carga.Empresa != null)
                            motorista = repMotorista.BuscarMotoristaPorCPF(carga.Empresa.Codigo, Utilidades.String.OnlyNumbers(motoristaIntegracao.CPF));

                        if (motorista == null)
                        {
                            string mensagem = "";
                            motorista = serMotorista.SalvarMotorista(motoristaIntegracao, _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : carga.Empresa, ref mensagem, _unitOfWork, _tipoServicoMultisoftware, _auditado, _clienteURLAcesso, _adminStringConexao);

                            if (!string.IsNullOrWhiteSpace(mensagem))
                            {
                                servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.CargaSemConfirmacaoMotorista, "Carga com irregularidade no vínculo com motorista.");
                                throw new WebServiceException(mensagem);
                            }
                        }

                        if (motorista != null)
                            motoristas.Add(motorista);
                    }
                }

                if (dadosTransporte.NumeroVeiculo > 0)
                {
                    Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
                    Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento existeStagePorNumeroVeiculo = repositorioStageAgrupamento.BuscarPorCargaENumeroVeiculo(carga.Codigo, dadosTransporte.NumeroVeiculo);

                    if (existeStagePorNumeroVeiculo != null)
                    {
                        existeStagePorNumeroVeiculo.Veiculo = carga.Veiculo;
                        existeStagePorNumeroVeiculo.Motorista = motoristas.FirstOrDefault();
                        repositorioStageAgrupamento.Atualizar(existeStagePorNumeroVeiculo);
                    }
                }

                if (dadosTransporte.Transportador != null)
                {
                    Dominio.Entidades.Empresa transportador = null;

                    if (!string.IsNullOrWhiteSpace(dadosTransporte.Transportador.CNPJ))
                    {
                        transportador = repEmpresa.BuscarEmpresaPorCNPJ(Utilidades.String.OnlyNumbers(dadosTransporte.Transportador.CNPJ));
                    }

                    if (transportador == null && !string.IsNullOrWhiteSpace(dadosTransporte.Transportador.CodigoIntegracao))
                        transportador = repEmpresa.BuscarPorCodigoIntegracao(Utilidades.String.OnlyNumbers(dadosTransporte.Transportador.CodigoIntegracao));

                    if (transportador == null && !string.IsNullOrWhiteSpace(dadosTransporte.Transportador.CNPJ))
                        throw new WebServiceException("Transportador não localizado, Cnpj inválido");

                    dadosTrans.CodigoEmpresa = transportador?.Codigo ?? 0;
                }

                foreach (Dominio.Entidades.Usuario motorista in motoristas)
                    dadosTrans.ListaCodigoMotorista.Add(motorista.Codigo);

                string mensagemErro = "";
                dynamic retorno = svcCarga.SalvarDadosTransporteCarga(dadosTrans, out mensagemErro, null, false, _tipoServicoMultisoftware, _webServiceConsultaCTe, _clienteMultisoftware, _auditado, _unitOfWork, false);
                if (!string.IsNullOrEmpty(mensagemErro))
                {
                    if (carga.TipoOperacao?.ConfiguracaoCarga?.HerdarDadosDeTransporteCargaPrimeiroTrecho ?? false)
                        servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.NaoPodeHerdarDadosTransporte, "Carga com falha ao herdar dados de transporte.");

                    throw new WebServiceException(mensagemErro);
                }

                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, carga.GetChanges(), "Salvou Dados do Transporte da Carga", _unitOfWork);

                _unitOfWork.CommitChanges();
            }
            catch (ServicoException ex)
            {
                _unitOfWork.Rollback();

                retornoBool = Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message);
                Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, ex.Message, "", Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Newtonsoft.Json.JsonConvert.SerializeObject(retornoBool), "json", _unitOfWork, carga, SituacaoIntegracao.Integrado);

                return retornoBool;
            }
            catch (WebServiceException ex)
            {
                _unitOfWork.Rollback();

                retornoBool = Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message);
                Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, ex.Message, "", Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Newtonsoft.Json.JsonConvert.SerializeObject(retornoBool), "json", _unitOfWork, carga, SituacaoIntegracao.Integrado);

                return retornoBool;
            }

            retornoBool = Retorno<bool>.CriarRetornoSucesso(true);
            Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, true, "Integração de Dados de Transporte e Carga", "", Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransporte), Newtonsoft.Json.JsonConvert.SerializeObject(retornoBool), "json", _unitOfWork, carga, SituacaoIntegracao.Integrado);

            return retornoBool;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> ObterCargasAguardandoEnvioDocumentos(RequestPaginacao requestPaginacao, Dominio.Entidades.WebService.Integradora integradora)
        {
            if (requestPaginacao.Limite == 0)
                requestPaginacao.Limite = 100;

            Dominio.ObjetosDeValor.WebService.Paginacao<int> retorno = new Dominio.ObjetosDeValor.WebService.Paginacao<int>()
            {
                Itens = new List<int>() { },
                NumeroTotalDeRegistro = 0
            };

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<int> cargasAgurardandoDocumento = repositorioCarga.BuscarCargasAguardandoEnviao(requestPaginacao.Inicio, requestPaginacao.Limite, integradora?.Empresa?.Codigo ?? 0);

            if (cargasAgurardandoDocumento.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>>.CriarRetornoSucesso(retorno);

            retorno.Itens = cargasAgurardandoDocumento;
            retorno.NumeroTotalDeRegistro = cargasAgurardandoDocumento.Count;

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>>.CriarRetornoSucesso(retorno);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarRecebimentoCargaAguardandoEnvioDocumentos(List<int> protocolosCargas)
        {
            if (protocolosCargas.Count == 0 || protocolosCargas == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Precisa informar os protocolos das cargas a confirmar");

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            foreach (int protocoloCarga in protocolosCargas)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga existeCarga = repositorioCarga.BuscarPorProtocolo(protocoloCarga);

                if (existeCarga == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi possivel encontrar carga pelo protocolo {protocoloCarga}");

                if (existeCarga.AgImportacaoCTe)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDuplicidadeRequisicao($"Carga com protocolo {protocoloCarga} já foi confirmada");

                existeCarga.AgImportacaoCTe = true;
                repositorioCarga.Atualizar(existeCarga);
            }

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Todas as cargas informadas foram comfirmadas com sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<PreDocumento>> RetornarPreDocumentosPorCarga(int protocoloCarga)
        {
            if (protocoloCarga == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<PreDocumento>>.CriarRetornoDadosInvalidos("Precisa Informar uma carga");

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCte = repositorioCargaCte.BuscarPorCargaSemCte(protocoloCarga);

            if (listaCargaCte.Count == 0 || listaCargaCte == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<PreDocumento>>.CriarRetornoDadosInvalidos("Não foi encontrado registros para a carga informada");

            List<PreDocumento> listaPreDocumentos = new List<PreDocumento>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in listaCargaCte)
                listaPreDocumentos.Add(ConverteObjetoPreDocumento(cargaCte));

            return Dominio.ObjetosDeValor.WebService.Retorno<List<PreDocumento>>.CriarRetornoSucesso(listaPreDocumentos, "Todas as cargas informadas foram comfirmadas com sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AjustarDatasPedido(Dominio.ObjetosDeValor.WebService.Carga.AjusteDatasPedido ajusteDatasPedido)
        {

            try
            {
                if ((ajusteDatasPedido == null) || (ajusteDatasPedido.ProtocoloPedido == 0))
                    throw new ServicoException("É obrigatório informar o protocolo do pedido.");

                DateTime dataValidade = DateTime.MinValue;
                DateTime dataPrevisaoEntrega = DateTime.MinValue;

                if (!string.IsNullOrWhiteSpace(ajusteDatasPedido.DataValidade))
                {
                    if (!DateTime.TryParseExact(ajusteDatasPedido.DataValidade, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataValidade))
                        throw new ServicoException("A data de validade não está no formato correto (dd/MM/yyyy HH:mm:ss).");
                }

                if (!string.IsNullOrWhiteSpace(ajusteDatasPedido.DataPrevisaoEntrega))
                {
                    if (!DateTime.TryParseExact(ajusteDatasPedido.DataPrevisaoEntrega, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataPrevisaoEntrega))
                        throw new ServicoException("A data de previsão de entrega não está no formato correto (dd/MM/yyyy HH:mm:ss).");
                }

                if ((dataValidade == DateTime.MinValue) && (dataPrevisaoEntrega == DateTime.MinValue))
                    throw new ServicoException("Necessário que ao menos uma data seja informada.");

                _unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(ajusteDatasPedido.ProtocoloPedido);

                if (pedido == null)
                    throw new WebServiceException($"Não foi possível encontrar nenhum pedido com o protocolo informado ({ajusteDatasPedido.ProtocoloPedido}).");

                pedido.Initialize();

                if (dataValidade != DateTime.MinValue)
                    pedido.DataValidade = dataValidade;

                if (dataPrevisaoEntrega != DateTime.MinValue)
                {
                    pedido.PrevisaoEntrega = dataPrevisaoEntrega;
                    pedido.DataInicioJanelaDescarga = dataPrevisaoEntrega;
                }

                repositorioPedido.Atualizar(pedido);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, pedido.GetChanges(), "Atualizou as datas do pedido via WS (AjustarDatasPedido).", _unitOfWork);

                _unitOfWork.CommitChanges();


                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "RequestLog");
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao atualizar as informações. {excecao.Message.ToString()}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AjustarNumeroOrdemPedido(Dominio.ObjetosDeValor.WebService.Carga.AjustarNumeroOrdemPedido ajusteNumeroOrdemPedido)
        {

            try
            {
                if ((ajusteNumeroOrdemPedido == null) || (ajusteNumeroOrdemPedido.ProtocoloPedido == 0))
                    throw new ServicoException("É obrigatório informar o protocolo do pedido.");

                _unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(ajusteNumeroOrdemPedido.ProtocoloPedido);

                if (pedido == null)
                    throw new WebServiceException($"Não foi possível encontrar nenhum pedido com o protocolo informado ({ajusteNumeroOrdemPedido.ProtocoloPedido}).");

                pedido.Initialize();
                pedido.NumeroOrdem = ajusteNumeroOrdemPedido.NumeroOrdem;

                repositorioPedido.Atualizar(pedido);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, pedido.GetChanges(), "Atualizou Numero Ordem do pedido via WS (AjustarNumeroOrdemPedido).", _unitOfWork);

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "RequestLog");
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao atualizar as informações. {excecao.Message.ToString()}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }


        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AlterarDataAgendamentoEntregaPorProtocoloPedido(Dominio.ObjetosDeValor.WebService.Carga.AjusteDataAgendamentoEntrega ajusteDataAgendamentoEntregaIntegracao)
        {
            Servicos.Log.TratarErro($"AlterarDataAgendamentoEntregaPorProtocoloPedido: {(ajusteDataAgendamentoEntregaIntegracao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(ajusteDataAgendamentoEntregaIntegracao) : string.Empty)}", "Request");
            StringBuilder mensagemErro = new StringBuilder();

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = repositorioCargaEntregaPedido.BuscarPorProtocoloPedido(ajusteDataAgendamentoEntregaIntegracao.ProtocoloPedido, false);

                if (cargaEntregaPedido == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi possível localizar Entregas ou Coletas para o Protocolo de Pedido: {ajusteDataAgendamentoEntregaIntegracao.ProtocoloPedido}");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregaPedido.CargaEntrega;
                //List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = cargaEntrega.Pedidos.Select(o => o.CargaPedido.Pedido).ToList();

                //if (listaPedidos?.Count() == 0)
                //    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi possível localizar os Pedidos de Entregas ou Coletas para a Carga: {cargaEntrega.Carga.CodigoCargaEmbarcador}");

                if (!string.IsNullOrWhiteSpace(ajusteDataAgendamentoEntregaIntegracao.DataAgendamento))
                {
                    DateTime? dataAgendamento = ajusteDataAgendamentoEntregaIntegracao.DataAgendamento.ToNullableDateTime();

                    if (!dataAgendamento.HasValue)
                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"A data de agendamento não está em um formato correto (dd/MM/yyyy HH:mm:ss).");

                    _unitOfWork.Start();

                    //foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in listaPedidos)
                    //{
                    //    pedido.DataAgendamento = dataAgendamento;
                    //    pedido.OrigemCriacaoDataAgendamentoPedido = OrigemCriacao.WebService;

                    //    repositorioPedido.Atualizar(pedido, _auditado);
                    //}

                    cargaEntrega.DataAgendamento = dataAgendamento;
                    cargaEntrega.OrigemCriacaoDataAgendamentoCargaEntrega = OrigemCriacao.WebService;

                    repositorioCargaEntrega.Atualizar(cargaEntrega, _auditado, null, $"Data de Agendamento alterada pelo WebService Método: AlterarDataAgendamentoEntregaPorProtocoloPedido");
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, _unitOfWork);
                    _unitOfWork.CommitChanges();

                }
                else
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos($"A data de agendamento não foi informada.");

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, $"Data de Agendamento da Carga: {cargaEntrega.Carga.CodigoCargaEmbarcador} alterado com sucesso!");
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro($"Carga: {ajusteDataAgendamentoEntregaIntegracao.ProtocoloPedido} Retornou essa mensagem: {excecao.Message}", "RequestLog");
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro($"Carga: {ajusteDataAgendamentoEntregaIntegracao.ProtocoloPedido} retornou exceção a seguir:", "RequestLog");
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao obter os dados das integrações. {mensagemErro.ToString()}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AlterarDadosAgendamentoPedido(AjusteDadosAgendamentoPedido ajusteDadosAgendamentoPedido)
        {
            Log.TratarErro($"AlterarDadosAgendamentoPedido: {(ajusteDadosAgendamentoPedido != null ? Newtonsoft.Json.JsonConvert.SerializeObject(ajusteDadosAgendamentoPedido) : string.Empty)}", "Request");

            try
            {
                if (string.IsNullOrWhiteSpace(ajusteDadosAgendamentoPedido.DataAgendamento))
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"A data de agendamento não foi informada.");

                DateTime? dataAgendamento = ajusteDadosAgendamentoPedido.DataAgendamento.ToNullableDateTime();
                if (!dataAgendamento.HasValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"A data de agendamento não está em um formato correto (dd/MM/yyyy HH:mm:ss).");

                if (!string.IsNullOrWhiteSpace(ajusteDadosAgendamentoPedido.SenhaAgendamentoCliente) && ajusteDadosAgendamentoPedido.SenhaAgendamentoCliente.Length > 150)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"A senha do agendamento no cliente não pode ter mais que 150 caracteres.");

                if (!string.IsNullOrWhiteSpace(ajusteDadosAgendamentoPedido.RestricaoEntrega) && ajusteDadosAgendamentoPedido.RestricaoEntrega.Length > 100)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"A restrição de entrega não pode ter mais que 100 caracteres.");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicionais = new(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = repositorioCargaEntregaPedido.BuscarPorProtocoloPedido(ajusteDadosAgendamentoPedido.ProtocoloPedido);

                if (cargaEntregaPedido == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi possível localizar Entregas ou Coletas para o Protocolo de Pedido: {ajusteDadosAgendamentoPedido.ProtocoloPedido}");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregaPedido.CargaEntrega;
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = cargaEntrega.Pedidos.Select(o => o.CargaPedido.Pedido).ToList();

                if (listaPedidos?.Count == 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Não foi possível localizar os Pedidos de Entregas ou Coletas para a Carga: {cargaEntrega.Carga.CodigoCargaEmbarcador}");

                if (listaPedidos.Exists(x => x.SituacaoPedido == SituacaoPedido.Cancelado))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Existe Pedido cancelado.");

                if (cargaEntrega.Carga.SituacaoCarga == SituacaoCarga.Cancelada)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("A Carga está cancelada.");

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> pedidosAdicionais = repositorioPedidoAdicionais.BuscarPorPedidos(listaPedidos.Select(x => x.Codigo).ToList());

                _unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in listaPedidos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = pedidosAdicionais.Find(x => x.Pedido.Codigo == pedido.Codigo);
                    if (pedidoAdicional == null)
                        return Retorno<bool>.CriarRetornoDadosInvalidos($"Pedido Adicional não encontrado para o Pedido: {pedido.Codigo}.");

                    if (!string.IsNullOrWhiteSpace(ajusteDadosAgendamentoPedido.SenhaAgendamentoCliente))
                        pedido.SenhaAgendamentoCliente = ajusteDadosAgendamentoPedido.SenhaAgendamentoCliente;

                    if (!string.IsNullOrWhiteSpace(ajusteDadosAgendamentoPedido.RestricaoEntrega))
                        pedidoAdicional.RestricaoEntrega = ajusteDadosAgendamentoPedido.RestricaoEntrega;

                    pedido.DataAgendamento = dataAgendamento;
                    pedido.OrigemCriacaoDataAgendamentoPedido = OrigemCriacao.WebService;
                    repositorioPedido.Atualizar(pedido, _auditado, null, "Data de Agendamento e Senha do Agendamento no Cliente alteradas pelo WebService Método: AlterarDadosAgendamentoPedido");

                    repositorioPedidoAdicionais.Atualizar(pedidoAdicional);
                }

                cargaEntrega.DataAgendamento = dataAgendamento;
                cargaEntrega.OrigemCriacaoDataAgendamentoCargaEntrega = OrigemCriacao.WebService;

                repositorioCargaEntrega.Atualizar(cargaEntrega, _auditado, null, "Data de Agendamento alterada pelo WebService Método: AlterarDadosAgendamentoPedido");
                Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, _unitOfWork);
                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true, $"Dados de Agendamento da Carga: {cargaEntrega.Carga.CodigoCargaEmbarcador} alterados com sucesso!");
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                Log.TratarErro($"Carga: {ajusteDadosAgendamentoPedido.ProtocoloPedido} Retornou essa mensagem: {excecao.Message}", "RequestLog");
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(excecao);
                Log.TratarErro($"Carga: {ajusteDadosAgendamentoPedido.ProtocoloPedido} retornou exceção a seguir:", "RequestLog");
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao obter os dados das integrações.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<StatusDTResponse> ObterStatusDT(StatusDTRequest statusDTRequest)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            StatusDTResponse statusDTResponse = new StatusDTResponse()
            {
                driverTicket = statusDTRequest.driverTicket,
                dtStatus = new List<StatusDT>()
            };

            foreach (string dtNumero in statusDTRequest.dtNumbers)
            {
                string dtNumeroFormatado = dtNumero;
                if (dtNumero[0] == 'D' && dtNumero[1] == 'T')
                    dtNumeroFormatado = dtNumero.Remove(0, 2).Insert(0, "00");

                (int codigoCarga, string globalStatus) existeCarga = repCarga.BuscarCodigoEGlobalStatusCargaPorCodigoCargaEmbarcador(dtNumeroFormatado);

                if (existeCarga.codigoCarga == 0)
                    return Retorno<StatusDTResponse>.CriarRetornoDadosInvalidos($"Nenhuma Carga encontrada com o Número de DT: {dtNumero}");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                decimal pesoLiquido = repCargaPedido.BuscarPesoLiquidoTotalPorCarga(existeCarga.codigoCarga);

                Dominio.ObjetosDeValor.WebService.Carga.StatusDT dtStatus = new Dominio.ObjetosDeValor.WebService.Carga.StatusDT()
                {
                    dtNumber = dtNumero,
                    dtPesoLiquido = pesoLiquido,
                    dtSAPFlag = existeCarga.globalStatus,
                    OcorrbLiveFlag = null
                };
                statusDTResponse.dtStatus.Add(dtStatus);
            }

            statusDTResponse.dateTimeRequest = DateTime.Now.ToDateTimeString();

            return Retorno<StatusDTResponse>.CriarRetornoSucesso(statusDTResponse);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.FilaHResponse> ObterDadosTotemFilaH(Dominio.ObjetosDeValor.WebService.Carga.FilaHRequest filahRequest)
        {
            Dominio.ObjetosDeValor.WebService.Carga.FilaHResponse response = new FilaHResponse();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            List<string> messageErro = new List<string>();
            response.plants = filahRequest.plants;
            response.requestDate = filahRequest.RequestDate;
            response.ticketDiver = filahRequest.DriverTicket;
            response.operations = new List<FilaHOperationsResponse>();

            foreach (FilaHOperations operation in filahRequest.operations)
                response.operations.Add(ObterOperationFilaH(operation, filahRequest, messageErro));

            response.checkinMessage = ObterChekinMessageFilaH(response.operations);
            response.checkinMessage.AddRange(messageErro);
            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.FilaHResponse>.CriarRetornoSucesso(response);

        }

        public Retorno<int> GerarCarregamentoRoteirizacao(Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao carregamentoRoteirizacao, Dominio.Entidades.WebService.Integradora integradora, bool wsRest = false)
        {
            return GerarCarregamentoRoteirizacaoAsync(carregamentoRoteirizacao, integradora, default, wsRest).GetAwaiter().GetResult();
        }

        public async Task<Retorno<int>> GerarCarregamentoRoteirizacaoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao carregamentoRoteirizacao, Dominio.Entidades.WebService.Integradora integradora, CancellationToken cancellationToken, bool wsRest = false)
        {
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, cancellationToken);

                Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(_unitOfWork);

                await _unitOfWork.StartAsync(cancellationToken);

                bool existeCargaIdentificadorRota = await repCarga.ExisteCargaIdentificadorRotaAsync(carregamentoRoteirizacao.IdentificadorDeRota);

                if (existeCargaIdentificadorRota)
                    throw new ServicoException($"Já existe carga para o identificador informado.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);

                CarregamentoRoteirizacaoDadosValidos dadosValidos = await ValidarDadosCarregamentoRoteirizacaoAsync(carregamentoRoteirizacao, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repConfiguracao.BuscarConfiguracaoPadraoAsync();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga()
                {
                    CodigoCargaEmbarcador = carregamentoRoteirizacao.NumeroCarregamento ?? string.Empty,
                    IdentificadorDeRota = carregamentoRoteirizacao.IdentificadorDeRota,
                    Empresa = dadosValidos.Transportador,
                    ExigeNotaFiscalParaCalcularFrete = dadosValidos.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? configuracaoEmbarcador.ExigirNotaFiscalParaCalcularFreteCarga,
                    Filial = dadosValidos.Filial,
                    ModeloVeicularCarga = dadosValidos.ModeloVeicularCarga,
                    MotivoPendencia = "",
                    ObservacaoCarregamentoRoteirizacao = carregamentoRoteirizacao.Observacao,
                    MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia,
                    NaoExigeVeiculoParaEmissao = dadosValidos.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false,
                    Rota = dadosValidos.RotaFrete,
                    SituacaoCarga = SituacaoCarga.Nova,
                    TipoDeCarga = dadosValidos.TipoCarga,
                    TipoOperacao = dadosValidos.TipoOperacao,
                    Veiculo = dadosValidos.Veiculo,
                    LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = false,
                    CargaGeradaPeloMetodoGerarCarregamento = true,
                    Distancia = carregamentoRoteirizacao.DistanciaEmKm,
                    TipoCarregamento = dadosValidos.TipoCarregamento,
                    DataCarregamentoCarga = carregamentoRoteirizacao.DataCarregamento
                };

                if (string.IsNullOrWhiteSpace(carga.CodigoCargaEmbarcador))
                {
                    if (configuracaoEmbarcador.NumeroCargaSequencialUnico)
                        carga.NumeroSequenciaCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork);
                    else
                        carga.NumeroSequenciaCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork, dadosValidos.Filial?.Codigo ?? 0);

                    carga.CodigoCargaEmbarcador = $"{dadosValidos.TipoOperacao?.ConfiguracaoCarga?.AdicionaPrefixoCodigoCarga ?? string.Empty}{carga.NumeroSequenciaCarga}".ToString();
                }

                if (dadosValidos.Veiculo?.VeiculosVinculados?.Count > 0)
                {
                    carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                    foreach (Dominio.Entidades.Veiculo veiculoVinculado in dadosValidos.Veiculo.VeiculosVinculados)
                        carga.VeiculosVinculados.Add(veiculoVinculado);
                }

                await repCarga.InserirAsync(carga);

                carga.Protocolo = carga.Codigo;

                if (carregamentoRoteirizacao.RotaFrete != null)
                {
                    Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork, cancellationToken);

                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = new Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete();

                    cargaRotaFrete.Carga = carga;
                    cargaRotaFrete.PolilinhaRota = carregamentoRoteirizacao.RotaFrete.Polilinha;
                    cargaRotaFrete.TempoDeViagemEmMinutos = carregamentoRoteirizacao.RotaFrete.TempoViagemMinutos;
                    cargaRotaFrete.TipoUltimoPontoRoteirizacao = carregamentoRoteirizacao.RotaFrete.TipoUltimoPontoRoteirizacao ?? TipoUltimoPontoRoteirizacao.PontoMaisDistante;

                    await repCargaRotaFrete.InserirAsync(cargaRotaFrete);

                    carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Concluido;
                    carga.CargaRotaFreteInformadaViaIntegracao = true;

                    await Servicos.Auditoria.Auditoria.AuditarAsync(_auditado, carga, "Carga roteirizada adicionada através do método Gerar Carregamento Roteirização", _unitOfWork, cancellationToken);
                }

                await repCarga.AtualizarAsync(carga);

                new Embarcador.Logistica.RestricaoRodagem(_unitOfWork).ValidaAtualizaZonaExclusaoRota(carga.Rota);
                new Embarcador.Carga.CargaMotorista(_unitOfWork).AdicionarMotorista(carga, dadosValidos.Motorista);

                string retorno = serCarga.CriarCargaPedidosPorPedidos(ref carga, dadosValidos.Pedidos, _tipoServicoMultisoftware, null, _unitOfWork, configuracaoEmbarcador, _auditado, carregamentoRoterizacaoValoresFrete: carregamentoRoteirizacao?.ComponenteFrete);

                if (!string.IsNullOrWhiteSpace(retorno))
                    throw new ServicoException(retorno);

                Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Embarcador.Carga.CargaDadosSumarizados(_unitOfWork, cancellationToken);
                carga.CargaFechada = true;

                await repCarga.AtualizarAsync(carga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repositorioCargaPedido.BuscarPorCargaAsync(carga.Codigo);

                await serCargaDadosSumarizados.AlterarDadosSumarizadosCargaAsync(carga, cargaPedidos, configuracaoEmbarcador, _unitOfWork, _tipoServicoMultisoftware);

                Embarcador.Carga.FecharCarga svcFecharCarga = new Embarcador.Carga.FecharCarga(_unitOfWork, cancellationToken);
                await svcFecharCarga.FecharCargaAsync(carga, _tipoServicoMultisoftware, _clienteMultisoftware, true, viaWSRest: wsRest);

                new Servicos.Embarcador.Carga.CargaRotaFrete(_unitOfWork).GerarRoteirizacaoManual(carga, _tipoServicoMultisoftware, gerarCarregamentoRoteirizacao: true);

                await _unitOfWork.CommitChangesAsync(cancellationToken);

                return Retorno<int>.CriarRetornoSucesso(carga.Protocolo);
            }
            catch (BaseException excecao)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                    return Retorno<int>.CriarRetornoDuplicidadeRequisicao(excecao.Message);

                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Retorno<int>.CriarRetornoExcecao($"Ocorreu uma falha ao gerar o carregamento roteirização.");
            }
            finally
            {
                await _unitOfWork.DisposeAsync();
            }
        }

        public async Task<Retorno<int>> AtualizarCarregamentoRoteirizacao(Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoRoteirizacao carregamentoRoteirizacao, CancellationToken cancellationToken, bool wsRest = false)
        {
            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
                Embarcador.Carga.Carga servicoCarga = new Embarcador.Carga.Carga(_unitOfWork, cancellationToken);

                if (carregamentoRoteirizacao.Protocolo <= 0)
                    throw new ServicoException("O campo protocolo é obrigatório.");

                if (string.IsNullOrWhiteSpace(carregamentoRoteirizacao.IdentificadorDeRota))
                    throw new ServicoException("O campo identificador de rota é obrigatório.");

                await _unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorProtocoloAsync(carregamentoRoteirizacao.Protocolo);
                CarregamentoRoteirizacaoDadosValidos dadosValidos = await ValidarDadosCarregamentoRoteirizacaoAsync(carregamentoRoteirizacao, cancellationToken, carga);

                bool estaNaLogistica = await servicoCarga.VerificarSeCargaEstaNaLogisticaAsync(carga, _tipoServicoMultisoftware);
                bool possuiNotas = (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && repPedidoXmlNotaFiscal.ContarXMLPorCargaPedido(carga.Pedidos.FirstOrDefault().Codigo) > 0);

                if (!estaNaLogistica || possuiNotas)
                    throw new ServicoException($"Não é possível atualizar a carga, pois, a mesma está na situação {carga.DescricaoSituacaoCarga} e já possui Notas Fiscais vinculadas a ela.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();

                carga.IdentificadorDeRota = carregamentoRoteirizacao.IdentificadorDeRota;
                carga.Empresa = dadosValidos.Transportador;
                carga.ExigeNotaFiscalParaCalcularFrete = dadosValidos.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? configuracaoEmbarcador.ExigirNotaFiscalParaCalcularFreteCarga;
                carga.Filial = dadosValidos.Filial;
                carga.ModeloVeicularCarga = dadosValidos.ModeloVeicularCarga;
                carga.MotivoPendencia = "";
                carga.ObservacaoCarregamentoRoteirizacao = carregamentoRoteirizacao.Observacao;
                carga.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                carga.NaoExigeVeiculoParaEmissao = dadosValidos.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false;
                carga.Rota = dadosValidos.RotaFrete;
                carga.SituacaoCarga = SituacaoCarga.Nova;
                carga.TipoDeCarga = dadosValidos.TipoCarga;
                carga.TipoOperacao = dadosValidos.TipoOperacao;
                carga.Veiculo = dadosValidos.Veiculo;
                carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = false;
                carga.CargaGeradaPeloMetodoGerarCarregamento = true;
                carga.Distancia = carregamentoRoteirizacao.DistanciaEmKm;
                carga.TipoCarregamento = dadosValidos.TipoCarregamento;
                carga.DataCarregamentoCarga = carregamentoRoteirizacao.DataCarregamento;

                if (carregamentoRoteirizacao.DataCarregamento != null)
                {
                    Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = await repositorioCargaJanelaCarregamento.BuscarPorCargaAsync(carga.Codigo);
                    if (cargaJanelaCarregamento != null)
                    {
                        cargaJanelaCarregamento.InicioCarregamento = carregamentoRoteirizacao.DataCarregamento ?? cargaJanelaCarregamento.InicioCarregamento;
                        await repositorioCargaJanelaCarregamento.AtualizarAsync(cargaJanelaCarregamento);
                    }
                }

                IEnumerable<int> protocolos = carregamentoRoteirizacao.Pedidos.Select(x => x.Protocolo);
                List<int> codigosPedidosParaRemover = carga.Pedidos.Where(x => !protocolos.Contains(x.Pedido.Protocolo)).Select(x => x.Pedido.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosParaRemover = repositorioCargaPedido.BuscarPorPedidos(codigosPedidosParaRemover);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();

                Servicos.Embarcador.Carga.CargaPedido.RemoverPedidosCarga(carga, cargaPedidosParaRemover, configuracaoEmbarcador, _tipoServicoMultisoftware, _unitOfWork, configuracaoGeralCarga);

                if (carregamentoRoteirizacao.RotaFrete != null)
                {
                    Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork, _cancellationToken);
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = await repCargaRotaFrete.BuscarPorCargaAsync(carga.Codigo);

                    bool novaCargaRotaFrete = cargaRotaFrete == null;

                    cargaRotaFrete ??= new Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete();

                    cargaRotaFrete.Carga = carga;
                    cargaRotaFrete.PolilinhaRota = carregamentoRoteirizacao.RotaFrete.Polilinha;
                    cargaRotaFrete.TempoDeViagemEmMinutos = carregamentoRoteirizacao.RotaFrete.TempoViagemMinutos;
                    cargaRotaFrete.TipoUltimoPontoRoteirizacao =
                        carregamentoRoteirizacao.RotaFrete.TipoUltimoPontoRoteirizacao
                        ?? TipoUltimoPontoRoteirizacao.PontoMaisDistante;

                    if (novaCargaRotaFrete)
                        await repCargaRotaFrete.InserirAsync(cargaRotaFrete);
                    else
                        await repCargaRotaFrete.AtualizarAsync(cargaRotaFrete);

                    carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Concluido;
                    carga.CargaRotaFreteInformadaViaIntegracao = true;

                    string acao = novaCargaRotaFrete
                        ? "InserirAsync Carga Rota Frete via integração AtualizarCarregamentoRoteirizacao"
                        : "AtualizarAsync Carga Rota Frete via integração AtualizarCarregamentoRoteirizacao";

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, acao, _unitOfWork);
                }

                new Embarcador.Logistica.RestricaoRodagem(_unitOfWork).ValidaAtualizaZonaExclusaoRota(carga.Rota);

                await repCarga.AtualizarAsync(carga);

                await Servicos.Auditoria.Auditoria.AuditarAsync(_auditado, carga, null, "Carga atualizada através do método AtualizarCarregamentoRoteirizacao.", _unitOfWork);

                servicoCarga.FecharCarga(carga, _unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, true, viaWSRest: wsRest);

                new Servicos.Embarcador.Carga.CargaRotaFrete(_unitOfWork).GerarRoteirizacaoManual(carga, _tipoServicoMultisoftware, gerarCarregamentoRoteirizacao: true);

                await _unitOfWork.CommitChangesAsync(cancellationToken);

                return Retorno<int>.CriarRetornoSucesso(carga.Protocolo);
            }
            catch (BaseException excecao)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                    return Retorno<int>.CriarRetornoDuplicidadeRequisicao(excecao.Message);

                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                await _unitOfWork.RollbackAsync(cancellationToken);

                return Retorno<int>.CriarRetornoExcecao($"Ocorreu uma falha ao atualizar o carregamento roteirização.");
            }
            finally
            {
                await _unitOfWork.DisposeAsync();
            }
        }

        public Retorno<int> GerarCarregamento(Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento, bool wsRest = false)
        {
            return GerarCarregamentoPadraoAsync(carregamento, wsRest).GetAwaiter().GetResult();
        }

        public async Task<Retorno<int>> GerarCarregamentoPadraoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento, bool wsRest = false, bool naoFecharConexao = false)
        {
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

                Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repConfiguracao.BuscarConfiguracaoPadraoAsync();
                Dominio.ObjetosDeValor.WebService.Carga.GerarAgrupamento.DadosValidados dadosValidados = await ValidarDadosParaGerarCarregamentoAsync(carregamento);

                await _unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga()
                {
                    CodigoCargaEmbarcador = carregamento.NumeroCarregamento ?? carregamento.NumeroCarga,
                    Empresa = dadosValidados.EmpresaIntegradora,
                    ExigeNotaFiscalParaCalcularFrete = dadosValidados.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? configuracaoEmbarcador.ExigirNotaFiscalParaCalcularFreteCarga,
                    Filial = dadosValidados.Filial,
                    ModeloVeicularCarga = dadosValidados.ModeloVeicularCarga,
                    MotivoPendencia = "",
                    ObservacaoTransportador = carregamento.Observacao,
                    MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia,
                    NaoExigeVeiculoParaEmissao = dadosValidados.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false,
                    Rota = dadosValidados.Pedidos.Select(x => x.RotaFrete).FirstOrDefault(),
                    SituacaoCarga = SituacaoCarga.Nova,
                    TipoDeCarga = dadosValidados.TipoCarga,
                    TipoOperacao = dadosValidados.TipoOperacao,
                    Veiculo = dadosValidados.Veiculo,
                    LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = false,
                    CargaGeradaPeloMetodoGerarCarregamento = true,
                    Distancia = carregamento.DistanciaEmKm
                };

                if (dadosValidados.Reboques.Count > 0)
                {
                    carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                    foreach (Dominio.Entidades.Veiculo veiculoVinculado in dadosValidados.Reboques)
                    {
                        carga.VeiculosVinculados.Add(veiculoVinculado);
                    }
                }

                await repCarga.InserirAsync(carga);
                carga.Protocolo = carga.Codigo;

                new Embarcador.Logistica.RestricaoRodagem(_unitOfWork).ValidaAtualizaZonaExclusaoRota(carga.Rota);
                new Embarcador.Carga.CargaMotorista(_unitOfWork).AdicionarMotorista(carga, dadosValidados.Motorista);

                Log.TratarErro("Entrou", "RequestLog");

                var retorno = serCarga.CriarCargaPedidosPorPedidos(ref carga, dadosValidados.Pedidos, _tipoServicoMultisoftware, null, _unitOfWork, configuracaoEmbarcador, _auditado);

                Log.TratarErro("Saiu", "RequestLog");

                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    await _unitOfWork.RollbackAsync();
                    return Retorno<int>.CriarRetornoDadosInvalidos(retorno);
                }

                if (!string.IsNullOrWhiteSpace(carregamento.DataCarregamento))
                    carga.DataCarregamentoCarga = carregamento.DataCarregamento.ToDateTime();

                Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
                carga.CargaFechada = true;

                await repCarga.AtualizarAsync(carga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);

                serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoEmbarcador, _unitOfWork, _tipoServicoMultisoftware);

                Embarcador.Carga.Carga svcCarga = new Embarcador.Carga.Carga(_unitOfWork);
                svcCarga.FecharCarga(carga, _unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, true, viaWSRest: wsRest);

                await Servicos.Auditoria.Auditoria.AuditarAsync(_auditado, carga, null, "Carga adicionada através do método GerarCarregamento.", _unitOfWork);

                await _unitOfWork.CommitChangesAsync();

                return Retorno<int>.CriarRetornoSucesso(carga.Protocolo);
            }
            catch (ServicoException excecao)
            {
                await _unitOfWork.RollbackAsync();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                    int protocoloCarga = await repCarga.BuscarProtocoloCargaPorNumeroCarregamentoEFilialProtocoloPedidosAsync(carregamento.NumeroCarregamento, carregamento.ProtocolosPedidos);

                    return Retorno<int>.CriarRetornoDuplicidadeRequisicao(excecao.Message, protocoloCarga);
                }

                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                await _unitOfWork.RollbackAsync();
                Log.TratarErro(excecao);
                return Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao gerar o carregamento.");
            }
            finally
            {
                await _unitOfWork.DisposeAsync();
            }
        }

        public async Task<Retorno<int>> GerarCarregamentoNovoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento, CancellationToken cancellationToken, bool wsRest = false, bool naoFecharConexao = false, bool fecharCarga = false)
        {

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, cancellationToken);

                Embarcador.Carga.Carga servicoCarga = new Embarcador.Carga.Carga(_unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();
                Dominio.ObjetosDeValor.WebService.Carga.GerarAgrupamento.DadosValidados dadosValidados = await ValidarDadosParaGerarCarregamentoAsync(carregamento, false, true);

                await _unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga()
                {
                    CodigoCargaEmbarcador = carregamento.NumeroCarregamento ?? carregamento.NumeroCarga,
                    Empresa = dadosValidados.EmpresaIntegradora,
                    ExigeNotaFiscalParaCalcularFrete = dadosValidados.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? configuracaoEmbarcador.ExigirNotaFiscalParaCalcularFreteCarga,
                    Filial = dadosValidados.Filial,
                    ModeloVeicularCarga = dadosValidados.ModeloVeicularCarga,
                    MotivoPendencia = "",
                    ObservacaoTransportador = carregamento.Observacao,
                    MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia,
                    NaoExigeVeiculoParaEmissao = dadosValidados.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false,
                    Rota = dadosValidados.Pedidos.Select(x => x.RotaFrete).FirstOrDefault(),
                    SituacaoCarga = SituacaoCarga.Nova,
                    TipoDeCarga = dadosValidados.TipoCarga,
                    TipoOperacao = dadosValidados.TipoOperacao,
                    Veiculo = dadosValidados.Veiculo,
                    LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = false,
                    CargaGeradaPeloMetodoGerarCarregamento = true,
                    Distancia = carregamento.DistanciaEmKm
                };

                if (dadosValidados.Reboques.Count > 0)
                {
                    carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                    foreach (Dominio.Entidades.Veiculo veiculoVinculado in dadosValidados.Reboques)
                    {
                        carga.VeiculosVinculados.Add(veiculoVinculado);
                    }
                }

                await repositorioCarga.InserirAsync(carga);
                carga.Protocolo = carga.Codigo;

                new Embarcador.Logistica.RestricaoRodagem(_unitOfWork).ValidaAtualizaZonaExclusaoRota(carga.Rota);
                new Embarcador.Carga.CargaMotorista(_unitOfWork).AdicionarMotorista(carga, dadosValidados.Motorista);

                await servicoCarga.CriarCargaPedidosPorPedidosAsync(carga, dadosValidados.Pedidos, _tipoServicoMultisoftware, null, _unitOfWork, configuracaoEmbarcador, _auditado);

                await repositorioCarga.AtualizarAsync(carga);

                if (!fecharCarga)
                {
                    await _unitOfWork.CommitChangesAsync();

                    return Retorno<int>.CriarRetornoSucesso(carga.Protocolo);
                }

                _unitOfWork.FlushAndClear();

                Embarcador.Carga.FecharCarga servicoFecharCarga = new Embarcador.Carga.FecharCarga(_unitOfWork);

                carga = await repositorioCarga.BuscarPorCodigoAsync(carga.Codigo);

                await servicoFecharCarga.FecharCargaAsync(carga, _tipoServicoMultisoftware, _clienteMultisoftware, true, viaWSRest: wsRest);
                carga.CargaFechada = true;

                await Servicos.Auditoria.Auditoria.AuditarAsync(_auditado, carga, null, "Carga adicionada através do método GerarCarregamento.", _unitOfWork);

                await repositorioCarga.AtualizarAsync(carga);

                await _unitOfWork.CommitChangesAsync();

                return Retorno<int>.CriarRetornoSucesso(carga.Protocolo);
            }
            catch (ServicoException excecao)
            {
                await _unitOfWork.RollbackAsync();
                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                await _unitOfWork.RollbackAsync();
                Log.TratarErro(excecao);
                return Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao gerar o carregamento.");
            }
            finally
            {
                if (!naoFecharConexao)
                    await _unitOfWork.DisposeAsync();
            }
        }

        public async Task<Dominio.ObjetosDeValor.WebService.Carga.GerarAgrupamento.DadosValidados> ValidarDadosParaGerarCarregamentoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento, bool validarProximoTrecho = false, bool usaObjeto = false)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Frota.Veiculo serVeiculo = new Frota.Veiculo(_unitOfWork);
            Empresa.Motorista serMotorista = new Empresa.Motorista(_unitOfWork);

            if (carregamento.ProtocolosPedidos?.Count == 0)
                throw new ServicoException("Prencha os protocolos dos pedidos.");

            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = string.IsNullOrWhiteSpace(carregamento.CodigoIntegracaoTipoCarga)
                ? null
                : await repTipoDeCarga.BuscarPorCodigoEmbarcadorAsync(carregamento.CodigoIntegracaoTipoCarga);

            if (!string.IsNullOrWhiteSpace(carregamento.CodigoIntegracaoTipoCarga) && tipoCarga == null)
                throw new ServicoException("Não foi localizado nenhum tipo de carga compatível com o código de integração informado.");

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = string.IsNullOrWhiteSpace(carregamento.CodigoIntegracaoTipoOperacao)
                ? null
                : repTipoOperacao.BuscarPorCodigoIntegracao(carregamento.CodigoIntegracaoTipoOperacao);

            if (!string.IsNullOrWhiteSpace(carregamento.CodigoIntegracaoTipoOperacao) && tipoOperacao == null)
                throw new ServicoException("Não foi localizado nenhum tipo de operação compatível com o código de integração informado.");

            Dominio.Entidades.Embarcador.Filiais.Filial filial = string.IsNullOrWhiteSpace(carregamento.Filial?.CodigoIntegracao ?? string.Empty)
                ? null
                : repFilial.buscarPorCodigoEmbarcador(carregamento.Filial.CodigoIntegracao);

            if (!string.IsNullOrWhiteSpace(carregamento.Filial?.CodigoIntegracao) && filial == null)
                throw new ServicoException("Não foi localizado nenhuma filial compatível com o código de integração informado.");

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = string.IsNullOrWhiteSpace(carregamento.CodigoModeloVeicular)
                ? null
                : repModeloVeicularCarga.buscarPorCodigoIntegracao(carregamento.CodigoModeloVeicular);

            if (!string.IsNullOrWhiteSpace(carregamento.CodigoModeloVeicular) && modeloVeicularCarga == null)
                throw new ServicoException("Não foi localizado nenhum modelo veicular compatível com o código de integração informado.");

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = null;
            Dominio.Entidades.Empresa empresa;

            int codigoFilial = 0;

            pedidos = await repPedido.BuscarPorCodigosAsync(carregamento.ProtocolosPedidos);
            if (pedidos.Count == 0)
                throw new ServicoException("Não foi localizado nenhum pedido compatível com os protocolos informados.");

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidos.FirstOrDefault();
            codigoFilial = pedido.Filial?.Codigo ?? 0;

            filial ??= pedido.Filial;
            empresa = pedido.Empresa;

            Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste = repCarga.BuscarPorCodigoCargaEmbarcador(carregamento.NumeroCarregamento, codigoFilial);

            if (cargaExiste != null)
                throw new ServicoException("Carregamento já gerado anteriormente.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);

            Dominio.Entidades.Empresa empresaIntegradora = null;

            if (!string.IsNullOrWhiteSpace(carregamento.TransportadoraEmitente?.CNPJ))
                empresaIntegradora = await repEmpresa.BuscarPorCNPJAsync(Utilidades.String.OnlyNumbers(carregamento.TransportadoraEmitente.CNPJ));

            empresaIntegradora ??= _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS ? null : empresa;

            Dominio.Entidades.Veiculo veiculo = null;
            List<Dominio.Entidades.Veiculo> reboques = new List<Dominio.Entidades.Veiculo>();

            if (!string.IsNullOrWhiteSpace(carregamento.Veiculo?.Placa))
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

                string mensagemVeiculo = string.Empty;

                if (empresaIntegradora != null)
                    veiculo = repVeiculo.BuscarPorPlacaIncluiInativos(empresaIntegradora.Codigo, carregamento.Veiculo.Placa);

                if (veiculo == null)
                {
                    veiculo = serVeiculo.SalvarVeiculo(carregamento.Veiculo, empresaIntegradora, false, ref mensagemVeiculo, _unitOfWork, _tipoServicoMultisoftware, _auditado);

                    if (!string.IsNullOrWhiteSpace(mensagemVeiculo))
                        throw new ServicoException(mensagemVeiculo);
                }
                else if (carregamento.Veiculo.Reboques?.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = await repositorioConfiguracaoWebService.BuscarConfiguracaoPadraoAsync();

                    reboques.AddRange(veiculo.VeiculosVinculados);

                    foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboque in carregamento.Veiculo.Reboques)
                    {
                        string mensagemReboque = string.Empty;

                        Dominio.Entidades.Veiculo reboqueSalvar = null;

                        if (empresaIntegradora != null)
                            reboqueSalvar = repVeiculo.BuscarPorPlacaIncluiInativos(empresaIntegradora.Codigo, reboque.Placa);

                        reboqueSalvar ??= serVeiculo.SalvarVeiculo(reboque, empresaIntegradora, false, ref mensagemReboque, _unitOfWork, _tipoServicoMultisoftware, _auditado);

                        if (!string.IsNullOrWhiteSpace(mensagemReboque))
                            throw new ServicoException(mensagemReboque);

                        if (reboqueSalvar != null)
                        {
                            if (!reboques.Contains(reboqueSalvar))
                                reboques.Add(reboqueSalvar);

                            if (!configuracaoWebService.NaoVincularReboqueNaTracaoAoAcionarMetodoGerarCarregamento && !veiculo.VeiculosVinculados.Contains(reboqueSalvar))
                                veiculo.VeiculosVinculados.Add(reboqueSalvar);
                        }
                    }
                }

                modeloVeicularCarga ??= veiculo.ModeloVeicularCarga;
            }

            Dominio.Entidades.Usuario motorista = null;
            if (!string.IsNullOrWhiteSpace(carregamento.Motorista?.CPF) || !string.IsNullOrWhiteSpace(carregamento.Motorista?.CodigoIntegracao))
            {
                string mensagem = "";
                motorista = serMotorista.SalvarMotorista(carregamento.Motorista, empresaIntegradora, ref mensagem, _unitOfWork, _tipoServicoMultisoftware, _auditado, null, _adminStringConexao);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    throw new ServicoException(mensagem);

                empresaIntegradora ??= motorista.Empresa;
            }

            if (validarProximoTrecho && carregamento.CarregamentosRedespacho != null && carregamento.CarregamentosRedespacho.Count > 0)
                await ValidarInformacoesCarregamentoRedespacho(carregamento.CarregamentosRedespacho);

            return new Dominio.ObjetosDeValor.WebService.Carga.GerarAgrupamento.DadosValidados
            {
                TipoCarga = tipoCarga,
                TipoOperacao = tipoOperacao,
                Filial = filial,
                ModeloVeicularCarga = modeloVeicularCarga,
                EmpresaIntegradora = empresaIntegradora,
                Veiculo = veiculo,
                Reboques = reboques,
                Motorista = motorista,
                Pedidos = pedidos,
            };
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga>> ObterDadosParadasPorDataCriacaoCarga(DadosObterCargaPorDataCriacao dadosObterCargaPorDataCriacao, int NumeroPagina)
        {
            try
            {

                if (((dadosObterCargaPorDataCriacao.DataCriacaoInicial == null || dadosObterCargaPorDataCriacao.DataCriacaoInicial == DateTime.MinValue) &&
                     (dadosObterCargaPorDataCriacao.DataCriacaoFinal == null || dadosObterCargaPorDataCriacao.DataCriacaoFinal == DateTime.MinValue))
                    &&
                    ((dadosObterCargaPorDataCriacao.DataEntregaInicial == null || dadosObterCargaPorDataCriacao.DataEntregaInicial == DateTime.MinValue) &&
                     (dadosObterCargaPorDataCriacao.DataEntregaFinal == null || dadosObterCargaPorDataCriacao.DataEntregaFinal == DateTime.MinValue)))
                    return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga>>.CriarRetornoExcecao($"É obrigatório informar uma data para consulta (Criação da Carga ou Data de Entrega).");

                Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga>> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga>>();

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas()
                {
                    DataInicial = dadosObterCargaPorDataCriacao.DataCriacaoInicial,
                    DataFinal = dadosObterCargaPorDataCriacao.DataCriacaoFinal,
                    DataEntregaInicial = dadosObterCargaPorDataCriacao.DataEntregaInicial,
                    DataEntregaFinal = dadosObterCargaPorDataCriacao.DataEntregaFinal,
                    CodigoIntegracaoCliente = dadosObterCargaPorDataCriacao.CodigoIntegracaoCliente ?? "",
                    NumeroCarga = dadosObterCargaPorDataCriacao.NumeroCarga ?? "",
                    NumeroCargas = new List<string>(),
                    CodigosTransportadores = new List<int>(),
                    CodigosVeiculos = new List<int>(),
                    CodigosMotoristas = new List<int>(),
                    CpfsCnpjsRemetentes = new List<double>(),
                    CpfsCnpjsDestinatarios = new List<double>(),
                    CodigoOrigem = 0,
                    CodigoDestino = 0,
                    CodigosFiliais = new List<int>(),
                    CodigosTipoOperacoes = new List<int>(),
                    CodigosTipoCargas = new List<int>(),
                    CodigosGrupoPessoas = new List<int>(),
                    CodigosRecebedores = new List<double>()
                };

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorio = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadeAgrupamento = ObterAgrupamentoRelatorioCargaParadas();

                int numeroPagina = (NumeroPagina <= 0) ? 1 : NumeroPagina;

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoAgrupar = "desc",
                    DirecaoOrdenar = "desc",
                    InicioRegistros = (numeroPagina - 1) * 300,
                    LimiteRegistros = 300,
                    PropriedadeAgrupar = "Filial"
                };
                int totalRegistros = repositorio.ContarConsultaRelatorioParadas(filtrosPesquisa, propriedadeAgrupamento);

                if (totalRegistros == 0)
                    return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga>>.CriarRetornoExcecao($"Não existem cargas com os parâmetros solicitados.");

                // TODO: ToList cast
                IList<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga> listaCargas = (totalRegistros > 0) ? repositorio.ConsultarRelatorioParadasWebService(filtrosPesquisa, propriedadeAgrupamento, parametrosConsulta) : new List<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga>();

                retorno.Objeto = listaCargas.ToList();
                retorno.Status = true;
                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                retorno.Mensagem = "Sucesso";
                retorno.CodigoMensagem = 200;

                return retorno;
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.ParadasCarga>>.CriarRetornoExcecao($"Ocorreu uma falha ao consultar.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }

        }

        public Dominio.ObjetosDeValor.WebService.Carga.RetornoWebHook ReceberPacotesWebHook(string jwt)
        {

            try
            {
                Repositorio.Embarcador.Cargas.PacoteWebHook repPacoteWebHook = new Repositorio.Embarcador.Cargas.PacoteWebHook(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.PacoteWebHook pacoteWebHook = new Dominio.Entidades.Embarcador.Cargas.PacoteWebHook()
                {
                    DataRecebimento = DateTime.Now,
                    LogKey = jwt,
                    SituacaoIntegracao = SituacaoIntegracao.AgIntegracao
                };
                repPacoteWebHook.Inserir(pacoteWebHook);

                return new Dominio.ObjetosDeValor.WebService.Carga.RetornoWebHook()
                {
                    message = "SUCCESS",
                    retcode = 0
                };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("ERRO PacoteWebHook.", "PacoteWebHook");
                Servicos.Log.TratarErro(ex, "PacoteWebHook");

                return new Dominio.ObjetosDeValor.WebService.Carga.RetornoWebHook()
                {
                    message = "SERVER_ERROR",
                    retcode = -100002
                };
            }
        }

        public Retorno<bool> FecharCarga(Dominio.ObjetosDeValor.WebService.Carga.ProtocoloIntegracao protocoloIntegracaoCarga)
        {
            Servicos.Embarcador.Integracao.IndicadorIntegracaoNFe servicoIndicadorIntegracaoNFe = new Servicos.Embarcador.Integracao.IndicadorIntegracaoNFe(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

            try
            {
                Servicos.Log.TratarErro("FecharCarga - protocoloIntegracaoCarga: " + protocoloIntegracaoCarga.protocoloIntegracaoCarga.ToString(), "RequestLog");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                carga = repositorioCarga.BuscarPorProtocolo(protocoloIntegracaoCarga.protocoloIntegracaoCarga);

                if (carga == null)
                    throw new WebServiceException("A carga informada não existe no Multi Embarcador");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                if (carga.SituacaoCarga == SituacaoCarga.Cancelada || carga.SituacaoCarga == SituacaoCarga.Anulada)
                    throw new WebServiceException("Não é possível fechar a carga porque ela foi cancelada.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroCanceladoOuAnulado);

                if (carga.CargaFechada && !carga.CargaDePreCargaEmFechamento)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaBase = carga;
                    if (carga.CargaAgrupamento != null)
                        cargaBase = carga.CargaAgrupamento;

                    if (configuracaoEmbarcador.UtilizarProtocoloDaPreCargaNaCarga && (cargaBase.SituacaoCarga == SituacaoCarga.AgNFe) && !cargaBase.ProcessandoDocumentosFiscais)
                    {
                        bool podeProcessar = true;
                        if (cargaBase.CargaAgrupada)
                        {
                            bool cargasPreCarga = repositorioCarga.BuscarCargasOriginaisSaoPrecarga(cargaBase.Codigo);
                            if (!cargasPreCarga)
                                cargaBase.CargaDePreCarga = false;
                            else
                                podeProcessar = false;
                        }

                        if (podeProcessar)
                        {

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaBase.Codigo);
                            servicoCarga.LiberarCargaSemNFe(cargaBase, cargaPedidos, configuracaoEmbarcador, _unitOfWork, _tipoServicoMultisoftware, _auditado);

                            cargaBase.ProcessandoDocumentosFiscais = true;
                            cargaBase.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;

                            Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(cargaBase, cargaPedidos, configuracaoEmbarcador, _unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                            repositorioCarga.Atualizar(cargaBase);
                            Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaBase, $"Solicitou as notas fiscais ao fechar a carga. Protocolo {cargaBase.Codigo}", _unitOfWork);
                        }

                        return Retorno<bool>.CriarRetornoSucesso(true);
                    }

                    throw new WebServiceException("O fechamento da carga já foi solicitado.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);
                }

                bool inconsistenciaPreCarga = false;

                if (carga.PreCarga != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
                    {
                        if (cargaPedido.Pedido.PreCarga == null)
                            inconsistenciaPreCarga = true;
                        else if (cargaPedido.Pedido.PreCarga.Codigo != carga.PreCarga.Codigo)
                            inconsistenciaPreCarga = true;
                        else
                        {
                            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoPreCarga in cargaPedido.Pedido.PreCarga.Pedidos)
                            {
                                if (!carga.Pedidos.Any(obj => obj.Pedido.Codigo == pedidoPreCarga.Codigo))
                                {
                                    inconsistenciaPreCarga = true;
                                    break;
                                }
                            }
                        }

                        if (inconsistenciaPreCarga)
                            break;
                    }
                }

                if (inconsistenciaPreCarga)
                    throw new WebServiceException($"Existem pedidos informados na carga que não pertencem a mesma pré carga ({carga.PreCarga.NumeroPreCarga}).");

                _unitOfWork.Start();

                if (configuracaoEmbarcador.FecharCargaPorThread)
                    carga.FechandoCarga = true;
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
                    {
                        PermitirHorarioCarregamentoInferiorAoAtual = carga.CargaDePreCargaEmFechamento
                    };

                    if (!carga.ExigeNotaFiscalParaCalcularFrete && !(carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                        bool todosPedidosEstaoAutorizados = repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo);

                        if (todosPedidosEstaoAutorizados)
                        {
                            carga.DataEnvioUltimaNFe = carga.DataEnvioUltimaNFe ?? DateTime.Now;
                            carga.DataInicioEmissaoDocumentos = carga.DataEnvioUltimaNFe;

                            bool cargaTemEmpresa = carga.Empresa != null;
                            bool cargaTemVeiculoEMotorista = (carga.Veiculo != null) && (carga.Motoristas?.Count > 0);
                            bool naoTemNenhumaPendenciaDeProdutos = !Servicos.Embarcador.Carga.Carga.IsCargaComPedidosSemProdutos(carga, _unitOfWork);
                            bool naoTemClienteSemLocalidade = !Servicos.Embarcador.Carga.Carga.IsCargaComClienteSemLocalidade(carga, _unitOfWork);
                            if (carga.SituacaoCarga == SituacaoCarga.AgTransportador
                                    && cargaTemEmpresa
                                    && (cargaTemVeiculoEMotorista || (carga.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false))
                                    && naoTemNenhumaPendenciaDeProdutos && naoTemClienteSemLocalidade)
                            {
                                carga.SituacaoCarga = SituacaoCarga.AgNFe;
                            }
                        }
                        else
                            servicoCarga.ValidarEnvioNotas(carga, _unitOfWork);
                    }

                    servicoCarga.FecharCarga(carga, _unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, recriarRotas: false, adicionarJanelaDescarregamento: carga.CargaAgrupamento == null, adicionarJanelaCarregamento: carga.CargaAgrupamento == null, validarDados: false, gerarAgendamentoColeta: true, propriedades: propriedades, viaWSRest: true);

                    if (carga.CargaAgrupamento == null)
                        carga.CargaFechada = true;

                    if (carga.CargaAgrupamento != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = carga.CargaAgrupamento;

                        if (!cargaAgrupada.ExigeNotaFiscalParaCalcularFrete && !(cargaAgrupada.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                        {
                            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                            bool todosPedidosEstaoAutorizados = repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(cargaAgrupada.Codigo);

                            if (todosPedidosEstaoAutorizados)
                            {
                                cargaAgrupada.DataEnvioUltimaNFe = cargaAgrupada.DataEnvioUltimaNFe ?? DateTime.Now;
                                cargaAgrupada.DataInicioEmissaoDocumentos = cargaAgrupada.DataEnvioUltimaNFe;

                                bool cargaTemEmpresa = carga.Empresa != null;
                                bool cargaTemVeiculoEMotorista = (carga.Veiculo != null) && (carga.Motoristas?.Count > 0);
                                bool naoTemNenhumaPendenciaDeProdutos = !Servicos.Embarcador.Carga.Carga.IsCargaComPedidosSemProdutos(cargaAgrupada, _unitOfWork);
                                bool naoTemClienteSemLocalidade = !Servicos.Embarcador.Carga.Carga.IsCargaComClienteSemLocalidade(carga, _unitOfWork);
                                if ((cargaAgrupada.SituacaoCarga == SituacaoCarga.AgTransportador)
                                    && cargaTemEmpresa
                                    && (cargaTemVeiculoEMotorista || (cargaAgrupada.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false))
                                    && naoTemNenhumaPendenciaDeProdutos && naoTemClienteSemLocalidade
                                )
                                {
                                    cargaAgrupada.SituacaoCarga = SituacaoCarga.AgNFe;
                                }

                                repositorioCarga.Atualizar(cargaAgrupada);
                            }
                        }
                    }

                    Servicos.Embarcador.Carga.CargaRotaFrete servicoCargaRotaFrete = new Servicos.Embarcador.Carga.CargaRotaFrete(_unitOfWork);
                    servicoCargaRotaFrete.GerarRoteirizacaoManual(carga, _tipoServicoMultisoftware);

                    Servicos.Log.TratarErro("24 - Fechou Carga (" + carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");
                }

                repositorioCarga.Atualizar(carga);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, Localization.Resources.Cargas.Carga.FechouACargaProtocolo, _unitOfWork);

                _unitOfWork.CommitChanges();
                Servicos.Log.TratarErro("FecharCarga - retorno: Sucesso", "RequestLog");

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro($"FecharCarga - retorno: {excecao.Message}", "RequestLog");
                servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(carga, excecao.Message);

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                {
                    AuditarRetornoDuplicidadeDaRequisicao(_unitOfWork, excecao.Message, _auditado, carga?.CodigoCargaEmbarcador ?? protocoloIntegracaoCarga.protocoloIntegracaoCarga.ToString());
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao(excecao.Message);
                }

                AuditarRetornoDadosInvalidos(_unitOfWork, excecao.Message, _auditado, carga?.CodigoCargaEmbarcador ?? protocoloIntegracaoCarga.protocoloIntegracaoCarga.ToString());
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro($"FecharCarga - retorno: {excecao.Message}", "RequestLog");
                servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(carga, excecao.Message);
                AuditarRetornoDadosInvalidos(_unitOfWork, excecao.Message, _auditado, carga?.CodigoCargaEmbarcador ?? protocoloIntegracaoCarga.protocoloIntegracaoCarga.ToString());
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();

                string mensagemErro = "Ocorreu uma falha ao solicitar o fechamento da carga";

                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro($"FecharCarga - retorno: {mensagemErro}", "RequestLog");
                servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(carga, mensagemErro);
                return Retorno<bool>.CriarRetornoExcecao(mensagemErro);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AtualizarDatasCarga(Dominio.ObjetosDeValor.WebService.Rest.AtualizarDatasCarga atualizarDatasCarga)
        {

            try
            {
                if (atualizarDatasCarga.ProtocoloCarga <= 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Número de Protocolo da Carga é obrigatório!");

                DateTime? dataInicioCarregamento = null;
                DateTime? dataTerminoCarregamento = null;
                DateTime? dataPrevisaoInicioViagem = null;
                DateTime? dataLoger = null;

                if (!string.IsNullOrWhiteSpace(atualizarDatasCarga.DataInicioCarregamento))
                {
                    dataInicioCarregamento = atualizarDatasCarga.DataInicioCarregamento.ToNullableDateTime();

                    if (!dataInicioCarregamento.HasValue)
                        return Retorno<bool>.CriarRetornoDadosInvalidos("DataInicioCarregamento não está em um formato correto (dd/MM/yyyy HH:mm:ss)");
                }

                if (!string.IsNullOrWhiteSpace(atualizarDatasCarga.DataTerminoCarregamento))
                {
                    dataTerminoCarregamento = atualizarDatasCarga.DataTerminoCarregamento.ToNullableDateTime();

                    if (!dataTerminoCarregamento.HasValue)
                        return Retorno<bool>.CriarRetornoDadosInvalidos("DataTerminoCarregamento não está em um formato correto (dd/MM/yyyy HH:mm:ss)");
                }

                if (!string.IsNullOrWhiteSpace(atualizarDatasCarga.DataPrevisaoInicioViagem))
                {
                    dataPrevisaoInicioViagem = atualizarDatasCarga.DataPrevisaoInicioViagem.ToNullableDateTime();

                    if (!dataPrevisaoInicioViagem.HasValue)
                        return Retorno<bool>.CriarRetornoDadosInvalidos("DataPrevisaoInicioViagem não está em um formato correto (dd/MM/yyyy HH:mm:ss)");
                }

                if (!string.IsNullOrWhiteSpace(atualizarDatasCarga.DataLoger))
                {
                    dataLoger = atualizarDatasCarga.DataLoger.ToNullableDateTime();

                    if (!dataLoger.HasValue)
                        return Retorno<bool>.CriarRetornoDadosInvalidos("DataLoger não está em um formato correto (dd/MM/yyyy HH:mm:ss)");
                }

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(atualizarDatasCarga.ProtocoloCarga);

                if (carga == null && !string.IsNullOrEmpty(atualizarDatasCarga.NumeroCarga))
                    carga = repCarga.BuscarPorCodigoCargaEmbarcador(atualizarDatasCarga.NumeroCarga);

                if (carga == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Carga " + atualizarDatasCarga.ProtocoloCarga + " não encontrada.");

                if (carga.DataInicioViagem.HasValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos($"Carga já teve início de viagem. Alteração das datas não é permitida.");

                bool dadosAtualizados = true;

                _unitOfWork.Start();

                carga.Initialize();

                if (dataInicioCarregamento.HasValue)
                    carga.DataCarregamentoCarga = dataInicioCarregamento;

                if (dataTerminoCarregamento.HasValue)
                    carga.DataFinalPrevisaoCarregamento = dataTerminoCarregamento;

                if (dataPrevisaoInicioViagem.HasValue)
                    carga.DataInicioViagemPrevista = dataPrevisaoInicioViagem;

                if (dataLoger.HasValue)
                {
                    carga.DataLoger = dataLoger;
                    carga.DataInicioViagemPrevista = dataLoger;
                }

                if (!string.IsNullOrEmpty(atualizarDatasCarga.StatusLoger))
                    carga.StatusLoger = atualizarDatasCarga.StatusLoger;

                repCarga.Atualizar(carga, _auditado);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(carga, configuracaoTMS, _unitOfWork, _tipoServicoMultisoftware);

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(dadosAtualizados, "Datas atualizadas com sucesso.");
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "RequestLog");
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDosDocumentosDaCarga(int protocoloIntegracaoCarga, string motivoDoCancelamento, string usuarioERPSolicitouCancelamento)
        {
            try
            {
                _unitOfWork.Start();
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(_unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloIntegracaoCarga);

                if (carga == null)
                    throw new WebServiceException("Protocolo da carga " + protocoloIntegracaoCarga + " não encontrado.");

                string mensagemErro = serCarga.ValidarSeCargaEstaAptaParaCancelamento(carga, motivoDoCancelamento, _unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento situacaoCancelamento = RetornoSolicitacaoCancelamento.CancelamentoRejeitado;

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    throw new WebServiceException(mensagemErro);

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                    throw new WebServiceException("Já foi solicitado o cancelamento da carga.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);

                bool podeCancelar = _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador
                        || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                        || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
                        || configuracaoTMS.PermitirCancelamentoTotalCargaViaWebService;

                if (!podeCancelar)
                    throw new WebServiceException("Não é possível solicitar o cancelamento, pois sua atual situação não permite o mesmo (" + carga.DescricaoSituacaoCarga + "), se necessário solicite que o cancelamento seja feito por um usuário autorizado na base Multisoftware.");

                if (configuracaoFinanceiro.ValidarDataPrevisaoPagamentoEDataPagamentoNoCancelamentoDosCTes && serCarga.ValidaPagamentoCTes(carga, out List<int> numeroCtesComPagamento))
                    throw new WebServiceException($"Não é possível cancelar a carga, CT-e(s) {String.Join(", ", numeroCtesComPagamento)} possuem Data Previsão/Pagamento definida.");

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                {
                    Carga = carga,
                    DefinirSituacaoEmCancelamento = true,
                    DuplicarCarga = true,
                    MotivoCancelamento = motivoDoCancelamento,
                    TipoServicoMultisoftware = _tipoServicoMultisoftware,
                    UsuarioERPSolicitouCancelamento = usuarioERPSolicitouCancelamento
                };

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, _unitOfWork);

                servicoCargaCancelamentoAprovacao.CriarAprovacao(cargaCancelamento, _tipoServicoMultisoftware);

                if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgAprovacaoSolicitacao)
                {
                    repositorioCargaCancelamento.Atualizar(cargaCancelamento);
                    situacaoCancelamento = RetornoSolicitacaoCancelamento.AguardandoAprovacao;
                }
                else
                {
                    Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware);

                    if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    {
                        string rejeicao = cargaCancelamento.MensagemRejeicaoCancelamento + " Codigo da Carga:" + carga.Codigo + " Codigo Carga Embarcador: " + carga.CodigoCargaEmbarcador;
                        Servicos.Log.TratarErro(rejeicao, "RequestLog");

                        throw new WebServiceException(rejeicao);
                    }

                    situacaoCancelamento = cargaCancelamento.Situacao == SituacaoCancelamentoCarga.Cancelada ? RetornoSolicitacaoCancelamento.Cancelada : RetornoSolicitacaoCancelamento.EmCancelamento;
                }

                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, "Solicitou o cancelamento da carga.", _unitOfWork);
                _unitOfWork.CommitChanges();

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoSucesso(situacaoCancelamento);
            }
            catch (WebServiceException excecao)
            {
                _unitOfWork.Rollback();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                {
                    AuditarRetornoDuplicidadeDaRequisicao(_unitOfWork, excecao.Message, _auditado, protocoloIntegracaoCarga.ToString());
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoDuplicidadeRequisicao(excecao.Message, RetornoSolicitacaoCancelamento.Cancelada);
                }

                AuditarRetornoDadosInvalidos(_unitOfWork, excecao.Message, _auditado, protocoloIntegracaoCarga.ToString());
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(excecao.Message, RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();

                AuditarRetornoDadosInvalidos(_unitOfWork, excecao.Message, _auditado, protocoloIntegracaoCarga.ToString());
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(excecao.Message, RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoExcecao("Ocorreu uma falha ao solicitar o cancelamento dos documentos da carga.", RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<bool> RemoverPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            Servicos.Log.TratarErro("RemoverPedido - Protocolo: " + (protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty), "Request");

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if (cargasPedidos == null || cargasPedidos.Count == 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possível encontrar nenhum pedido com os protocolos informados");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                _unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidos)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

                    if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (cargaPedido.Pedido.Container != null)
                            throw new WebServiceException("O pedido selecionado já possui container vinculado.");

                        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoVinculadoCarga(cargaPedido, _unitOfWork, configuracaoEmbarcador, _tipoServicoMultisoftware, _clienteMultisoftware, removerPedido: false);

                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCarga = repositorioCargaPedido.BuscarPedidosPorCarga(carga.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoCarga in pedidosCarga)
                        {
                            pedidoCarga.QuantidadeContainerBooking = pedidoCarga.QuantidadeContainerBooking - 1;

                            if (pedidoCarga.QuantidadeContainerBooking < 0)
                                pedidoCarga.QuantidadeContainerBooking = 0;

                            repositorioPedido.Atualizar(pedidoCarga);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, null, "Excluiu pedido vinculado por Integração.", _unitOfWork);
                    }
                    else
                    {
                        bool permitirRemoverTodos = !configuracaoGeralCarga.NaoPermitirRemoverUltimoPedidoCarga;

                        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(carga, cargaPedido, configuracaoEmbarcador, _tipoServicoMultisoftware, _unitOfWork, configuracaoGeralCarga, null, permitirRemoverTodos, false, configuracaoWebService.NaoRecalcularFreteAoAdicionarRemoverPedido);
                        Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, null, $"Removeu o pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} via integração", _unitOfWork);

                        if (permitirRemoverTodos && !repositorioCargaPedido.PossuiCargaPedidoPorCarga(carga.Codigo)) // Se era o último carga pedido, solicita o cancelamento da carga
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                            {
                                Carga = carga,
                                MotivoCancelamento = "Cancelamento por remoção do último pedido via integração",
                                TipoServicoMultisoftware = _tipoServicoMultisoftware,
                                Usuario = _auditado.Usuario
                            };

                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoEmbarcador, _unitOfWork);
                            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware);

                            if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                                throw new WebServiceException(cargaCancelamento.MensagemRejeicaoCancelamento);

                            Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCancelamento, null, "Adicionou o cancelamento da Carga ao remover o seu último pedido via integração.", _unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, null, "Adicionou o cancelamento da Carga ao remover o seu último pedido via integração.", _unitOfWork);
                        }
                        else
                        {
                            if (!(cargaPedido.Carga.TipoOperacao?.NaoIntegrarOpentech ?? false) && !(cargaPedido.Carga.Veiculo?.NaoIntegrarOpentech ?? false))
                                cargaPedido.Carga.AguardarIntegracaoEtapaTransportador = Servicos.WebService.NFe.NotaFiscal.AdicionarIntegracaoSM(cargaPedido.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, configuracaoEmbarcador.NaoAvancarEtapaComRejeicaoIntegracaoTransportadorRejeitada, cargaPedido.Carga.CargaEmitidaParcialmente, _unitOfWork);

                            carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;
                            if (carga.SituacaoCarga == SituacaoCarga.AgNFe && carga.ExigeNotaFiscalParaCalcularFrete)
                                carga.ProcessandoDocumentosFiscais = true;

                            repositorioCarga.Atualizar(carga);
                        }
                    }
                }

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao remover o pedido da carga");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AgruparCargasReceberDadosTransporte(Dominio.ObjetosDeValor.WebService.Rest.AgrupamentoCargasReceberDadosTransporte agrupamentoCargasReceberDadosTransporte)
        {
            Servicos.Log.TratarErro(Newtonsoft.Json.JsonConvert.SerializeObject(agrupamentoCargasReceberDadosTransporte), "AgruparCargasReceberDadosTransporte");

            try
            {
                if (agrupamentoCargasReceberDadosTransporte.operation.Count == 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Lista de operação é obrigatório");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
                Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(_unitOfWork);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

                string mensagemErro = "";
                foreach (Dominio.ObjetosDeValor.WebService.Rest.Operation operacao in agrupamentoCargasReceberDadosTransporte.operation)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte();

                    Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigoIntegracao(operacao.employer_code);
                    dadosTransporte.CodigoEmpresa = transportador?.Codigo ?? 0;

                    Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(dadosTransporte.CodigoEmpresa, operacao.vehicle_code);
                    dadosTransporte.CodigoTracao = veiculo?.Codigo ?? 0;

                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularInformado = repositorioModeloVeicularCarga.BuscarPorCodigo(veiculo?.ModeloVeicularCarga?.Codigo ?? 0);
                    dadosTransporte.CodigoModeloVeicular = modeloVeicularInformado?.Codigo ?? 0;

                    Dominio.Entidades.Usuario motorista = repositorioUsuario.BuscarMotoristaPorCPFECNPJEmpresa(operacao.driver.dni, transportador?.CNPJ, null);
                    dadosTransporte.CodigoMotorista = motorista?.Codigo ?? 0;

                    foreach (Dominio.ObjetosDeValor.WebService.Rest.Event evento in operacao.events)
                    {
                        _unitOfWork.Start();

                        switch (evento.code)
                        {
                            case "MS": //MasterShipment
                                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidosNumeroOrdem = repositorioPedido.BuscarPorNumeroOrdens(operacao.order_container);

                                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorPedidos(listaPedidosNumeroOrdem.Select(p => p.Codigo).ToList());

                                Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = new Servicos.Embarcador.Carga.CargaAgrupada(_unitOfWork).AgruparCargas(cargas, null, operacao.code, null, null, _tipoServicoMultisoftware, _clienteMultisoftware);
                                if (cargaAgrupada == null)
                                    throw new WebServiceException($"Falha ao agupar as Cargas: {string.Join(",", operacao.order_container)}");
                                try
                                {
                                    dadosTransporte.Carga = cargaAgrupada;
                                    dadosTransporte.Carga.Initialize();
                                    servicoCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, null, false, _tipoServicoMultisoftware, _webServiceConsultaCTe, _clienteMultisoftware, _auditado, _unitOfWork, false);
                                    Servicos.Auditoria.Auditoria.Auditar(_auditado, dadosTransporte.Carga, dadosTransporte.Carga.GetChanges(), "Salvou Dados do Transporte da Carga (AgruparCargasReceberDadosTransporte - Agrupamento)", _unitOfWork);
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao salvar dados transporte no agrupamento de cargas - continuando processamento: {ex.ToString()}", "CatchNoAction");
                                }
                                ; //Não deve ocasionar erro se não conseguiu salvar os dados transporte.

                                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaAgrupada, null, "Criada pelo agrupamento automatico das cargas " + string.Join(", ", (from obj in cargaAgrupada.CodigosAgrupados select obj).ToList()) + " (AgruparCargasReceberDadosTransporte)", _unitOfWork);
                                break;

                            case "CD": //Checkin do Veículo
                                dadosTransporte.Carga = repositorioCarga.BuscarCargaPorCodigoCargaEmbarcador(operacao.code);
                                dadosTransporte.Carga.Initialize();

                                servicoCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, null, false, _tipoServicoMultisoftware, _webServiceConsultaCTe, _clienteMultisoftware, _auditado, _unitOfWork, false);
                                if (!string.IsNullOrEmpty(mensagemErro))
                                    throw new WebServiceException(mensagemErro);

                                Servicos.Auditoria.Auditoria.Auditar(_auditado, dadosTransporte.Carga, dadosTransporte.Carga.GetChanges(), "Salvou Dados do Transporte da Carga (AgruparCargasReceberDadosTransporte)", _unitOfWork);
                                break;
                        }

                        _unitOfWork.CommitChanges();
                    }
                }

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "AgruparCargasReceberDadosTransporte");
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "AgruparCargasReceberDadosTransporte");
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao agrupar cargas.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<bool> DesbloquearCargaBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual(Dominio.ObjetosDeValor.WebService.Carga.ProtocoloIntegracao protocoloIntegracaoCarga)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            try
            {
                Servicos.Log.TratarErro("DesbloquearCarga - protocoloIntegracaoCarga: " + protocoloIntegracaoCarga.protocoloIntegracaoCarga.ToString(), "RequestLog");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                carga = repositorioCarga.BuscarPorProtocolo(protocoloIntegracaoCarga.protocoloIntegracaoCarga);


                if (carga == null)
                    throw new WebServiceException("A carga informada não existe no Multi Embarcador");

                Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);

                _unitOfWork.Start();

                servicoMensagemAlerta.Confirmar(carga, TipoMensagemAlerta.CargaAguardandoDesbloqueio);

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro($"DesbloquearCarga - retorno: {excecao.Message}", "RequestLog");

                AuditarRetornoDadosInvalidos(_unitOfWork, excecao.Message, _auditado, carga?.CodigoCargaEmbarcador ?? protocoloIntegracaoCarga.protocoloIntegracaoCarga.ToString());
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro($"DesbloquearCarga - retorno: {excecao.Message}", "RequestLog");
                AuditarRetornoDadosInvalidos(_unitOfWork, excecao.Message, _auditado, carga?.CodigoCargaEmbarcador ?? protocoloIntegracaoCarga.protocoloIntegracaoCarga.ToString());
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();

                string mensagemErro = "Ocorreu uma falha ao solicitar o desbloqueio da carga";

                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro($"FecharCarga - retorno: {mensagemErro}", "RequestLog");
                return Retorno<bool>.CriarRetornoExcecao(mensagemErro);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarDadosTransportador(Dominio.ObjetosDeValor.WebService.Carga.DadosTransportador dadosTransportador, Dominio.Entidades.WebService.Integradora integradora)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            List<Dominio.Entidades.Empresa> empresas = null;
            Dominio.Entidades.Empresa empresa = null;
            Dominio.ObjetosDeValor.WebService.Retorno<bool> retornoBool = null;
            try
            {
                if (dadosTransportador == null)
                    throw new WebServiceException("É obrigatório informar os dados do container.");

                if (dadosTransportador.ProtocoloCarga <= 0)
                    throw new WebServiceException("É obrigatório informar protocolo da carga.");

                if (string.IsNullOrEmpty(dadosTransportador.CodigoIntegracaoTransportador))
                    throw new WebServiceException("É obrigatório informar código integração transportador.");

                Servicos.WebService.Empresa.Motorista serMotorista = new Servicos.WebService.Empresa.Motorista(_unitOfWork);
                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

                if (dadosTransportador.ProtocoloCarga > 0)
                    carga = repCarga.BuscarPorProtocolo(dadosTransportador.ProtocoloCarga);
                if (carga == null)
                    throw new WebServiceException("Não foi localizada uma carga compatível com os dados informados na integração, por favor verifique.");


                empresas = repEmpresa.BuscarPorEmpresasCodigoIntegracao(dadosTransportador.CodigoIntegracaoTransportador);
                if (empresas == null || empresas.Count() == 0)
                    throw new WebServiceException($"Não encontrado traportador com código de integração {dadosTransportador.CodigoIntegracaoTransportador}");

                if (string.IsNullOrEmpty(dadosTransportador.CNPJtransportador))
                {
                    if (empresas.Count() > 1)
                        throw new WebServiceException($"Código integração {dadosTransportador.CodigoIntegracaoTransportador} associado mais do que uma empresa informe tabém um CNPJ especifico.");
                    empresa = empresas.FirstOrDefault();
                }
                else
                {
                    empresa = empresas.FirstOrDefault(e => e.CNPJ == dadosTransportador.CNPJtransportador);
                    if (empresa == null)
                        throw new WebServiceException($"CNPJ {dadosTransportador.CNPJtransportador} e código de integração {dadosTransportador.CodigoIntegracaoTransportador} não estão correlacionados.");
                }

                carga.Initialize();

                List<SituacaoCarga> situacaoesPermitidas = new List<SituacaoCarga>() {
                        SituacaoCarga.Nova,
                        SituacaoCarga.AgNFe,
                        SituacaoCarga.CalculoFrete,
                        SituacaoCarga.EmTransporte,
                        SituacaoCarga.PendeciaDocumentos
                        };

                if (!situacaoesPermitidas.Contains(carga.SituacaoCarga) && !(carga.TipoOperacao?.ConfiguracaoCarga?.ReceberAtualizacaoDeTransporteEmQualquerSituacaoCarga ?? false))
                    throw new WebServiceException("A atual situação da carga não permite atualização dos dados de transporte");

                _unitOfWork.Start();

                int registrosAfetados = 0;
                if (carga.Motoristas != null && carga.Motoristas.Count() > 0)
                {
                    List<int> codigoMotoristasCarga = carga.Motoristas.Select(x => x.Codigo).ToList();
                    List<int> codigoMotoristasEmpresaCarga = repMotorista.BuscarMotoristaPorEmpresaCodigosMotoristas(empresa.Codigo, codigoMotoristasCarga);
                    registrosAfetados = repMotorista.DeletarCargasMotoristas(carga.Codigo, codigoMotoristasCarga.Except(codigoMotoristasEmpresaCarga).ToList());
                }

                if (carga.Veiculo != null && repVeiculo.BuscarPorEmpresaCodigoVeiculo(empresa.Codigo, new List<int> { carga.Veiculo.Codigo }).Count() == 0)
                {
                    registrosAfetados++;
                    carga.Veiculo = null;
                }

                if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count() > 0)
                {
                    List<int> codigoVeiculosCarga = carga.VeiculosVinculados.Select(x => x.Codigo).ToList();
                    List<int> codigoVeiculosEmpresaCarga = repVeiculo.BuscarPorEmpresaCodigoVeiculo(empresa.Codigo, codigoVeiculosCarga);
                    registrosAfetados = +repVeiculo.DeletarVeiculosCarga(carga.Codigo, codigoVeiculosCarga.Except(codigoVeiculosEmpresaCarga).ToList());
                }

                carga.Empresa = empresa;
                if (registrosAfetados > 0)
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, carga.GetChanges(), "Salvou Dados do Transportador da Carga", _unitOfWork);

                repCarga.Atualizar(carga);
                _unitOfWork.CommitChanges();
            }
            catch (ServicoException ex)
            {
                retornoBool = Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message);
                Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, ex.Message, "", Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransportador), Newtonsoft.Json.JsonConvert.SerializeObject(retornoBool), "json", _unitOfWork, carga, SituacaoIntegracao.Integrado);

                return retornoBool;
            }
            catch (WebServiceException ex)
            {
                retornoBool = Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message);
                Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, false, ex.Message, "", Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransportador), Newtonsoft.Json.JsonConvert.SerializeObject(retornoBool), "json", _unitOfWork, carga, SituacaoIntegracao.Integrado);

                return retornoBool;
            }
            retornoBool = Retorno<bool>.CriarRetornoSucesso(true);
            Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.InformarIntegracao(integradora, true, "Integração de Dados de Transportador e Carga", "", Newtonsoft.Json.JsonConvert.SerializeObject(dadosTransportador), Newtonsoft.Json.JsonConvert.SerializeObject(retornoBool), "json", _unitOfWork, carga, SituacaoIntegracao.Integrado);

            return retornoBool;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento> ConsultarDetalhesCancelamentoDaCarga(int protocoloIntegracaoCarga)
        {
            Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento>();
            try
            {
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorProtocoloCarga(protocoloIntegracaoCarga);

                if (cargaCancelamento != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCancelamento.Carga.Codigo);

                    retorno.Status = true;
                    retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Carga.CargaCancelamento();
                    retorno.Objeto.DataCancelamento = cargaCancelamento.DataCancelamento != null ? cargaCancelamento.DataCancelamento.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
                    retorno.Objeto.MotivoRejeicaoCancelamento = cargaCancelamento.MensagemRejeicaoCancelamento;
                    retorno.Objeto.MotivoCancelamento = cargaCancelamento.MotivoCancelamento;
                    retorno.Objeto.UsuarioMultisoftware = cargaCancelamento.Usuario != null ? cargaCancelamento.Usuario.Nome : "";
                    retorno.Objeto.UsuarioERP = cargaCancelamento.UsuarioSolicitouCancelamento?.Nome ?? (cargaCancelamento.Usuario != null ? cargaCancelamento.Usuario.Nome : "");
                    retorno.Objeto.NumeroOS = cargaPedido?.Pedido.NumeroOS;
                    retorno.Objeto.CodigoIntegracaoOperador = cargaCancelamento.UsuarioSolicitouCancelamento?.CodigoIntegracao ?? (cargaCancelamento.Usuario != null ? cargaCancelamento.Usuario.CodigoIntegracao : "");
                    retorno.Objeto.DescricaoTipoOperacao = cargaCancelamento.Carga.TipoOperacao?.Descricao;
                    retorno.Objeto.StatusCustoExtra = cargaCancelamento.Carga.TipoOperacao?.ConfiguracaoCarga?.DirecionamentoCustoExtra.ObterDescricao() ?? TipoDirecionamentoCustoExtra.Nenhum.ObterDescricao();

                    switch (cargaCancelamento.Situacao)
                    {
                        case SituacaoCancelamentoCarga.RejeicaoCancelamento:
                            retorno.Objeto.SituacaoCancelamento = RetornoSolicitacaoCancelamento.CancelamentoRejeitado;
                            break;
                        case SituacaoCancelamentoCarga.Cancelada:
                            retorno.Objeto.SituacaoCancelamento = RetornoSolicitacaoCancelamento.Cancelada;
                            break;
                        case SituacaoCancelamentoCarga.Anulada:
                            retorno.Objeto.SituacaoCancelamento = RetornoSolicitacaoCancelamento.Anulada;
                            break;
                        default:
                            retorno.Objeto.SituacaoCancelamento = RetornoSolicitacaoCancelamento.EmCancelamento;
                            break;
                    }

                    //Retornar Em Cancelamento 1uando configurado para reenviar automaticamente o cancelamento rejeitado enquanto não tiver sido feito todas tentativas
                    if (configuracao != null && configuracao.TempoMinutosParaReenviarCancelamento > 0 && retorno.Objeto.SituacaoCancelamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.CancelamentoRejeitado)
                    {
                        int numeroTentativas = (60 / configuracao.TempoMinutosParaReenviarCancelamento) * 24;
                        if (repCargaCancelamento.VerificarCancelamentoPendenteReenvioAutomatico(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento, numeroTentativas, cargaCancelamento.Codigo))
                        {
                            retorno.Objeto.SituacaoCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.EmCancelamento;
                            retorno.Objeto.MotivoRejeicaoCancelamento = retorno.Objeto.MotivoRejeicaoCancelamento + " Reenvio em andamento.";
                        }
                    }

                    Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Consultou detalhes do cancelamento da carga", _unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "A carga informada não foi cancelada";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a carga";
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SetarCargaCritica(int protocoloCarga)
        {
            try
            {
                if (protocoloCarga == 0)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Necessário informar o protocolo de integração da carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(protocoloCarga);

                if (carga == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possível encontrar a carga com o protocolo informado.");

                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarUltimoPorCarga(carga.Codigo);

                _unitOfWork.Start();

                string mensagemRetorno = "";

                if (carga.CargaCritica ?? false)
                {
                    carga.CargaCritica = false;

                    if (monitoramento != null)
                        monitoramento.Critico = false;

                    mensagemRetorno = "Carga setada para não crítica";
                }
                else
                {
                    carga.CargaCritica = true;

                    if (monitoramento != null)
                        monitoramento.Critico = true;

                    mensagemRetorno = "Carga setada para crítica";
                }

                _unitOfWork.CommitChanges();


                return Retorno<bool>.CriarRetornoSucesso(true, mensagemRetorno);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "RequestLog");
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao atualizar as informações. {excecao.Message.ToString()}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>> BuscarCarregamentosPendentesIntegracao(RequestCarregamentosPendentesIntegracao requestCarregamentosPendentesIntegracao, Dominio.Entidades.WebService.Integradora integradora, bool requestPelaPedido = false)
        {
            Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento> carregamento = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>()
            };

            try
            {
                if (requestCarregamentosPendentesIntegracao.Limite > 100)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfigWS = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(_unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repositorioBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(_unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete repositorioSimulacaoFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = repConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configWS = repConfigWS.BuscarConfiguracaoPadrao();

                Servicos.WebService.Filial.Filial servicoFilial = new Servicos.WebService.Filial.Filial(_unitOfWork);

                int codigoEmpresa = integradora?.Empresa?.Codigo ?? 0;

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga
                    .BuscarCarregamentosPendentesIntegracao(
                        configuracao.RetornarCargasAgrupadasCarregamento,
                        configuracao.NaoRetornarCarregamentosSemData,
                        (configuracaoDadosTransporte?.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte ?? false),
                        configWS.RetornarCarregamentosSomenteCargasEmAgNF,
                        requestCarregamentosPendentesIntegracao.Inicio,
                        requestCarregamentosPendentesIntegracao.Limite,
                        codigoEmpresa,
                        requestCarregamentosPendentesIntegracao.CodigoFilial,
                        requestCarregamentosPendentesIntegracao.CodigoRegiao
                     );

                List<int> codigosCargas = cargas
                    .Where(x => x.Carregamento != null)
                    .Select(x => x.Carregamento.Codigo)
                    .ToList();

                List<BlocosPedidosCarregamento> blocosCarregamento = new List<BlocosPedidosCarregamento>();

                if (codigosCargas != null && codigosCargas.Count > 0)
                    blocosCarregamento = repositorioBlocoCarregamento.BuscarPorCarregamentosEOrdenarPorDescarregamento(codigosCargas) ?? new List<BlocosPedidosCarregamento>();

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete> simulacoesFrete = repositorioSimulacaoFrete.BuscarPorCarregamentos(codigosCargas);

                Servicos.WebService.Empresa.Empresa servicoEmpresa = new Servicos.WebService.Empresa.Empresa(_unitOfWork);
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = carga.CargaAgrupada && configuracao.RetornarCargasAgrupadasCarregamento ? repCargaPedido.BuscarPorCarga(carga.Codigo) : repCargaPedido.BuscarPorCargaOrigem(carga.Protocolo);
                    Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento objCarregamento = new Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento();

                    objCarregamento.NumeroCarregamento = carga.Carregamento?.NumeroCarregamento ?? "";
                    objCarregamento.ProtocoloIntegracaoCarga = carga.Protocolo;
                    objCarregamento.ProtocoloPreCarga = carga.Carregamento?.PreCarga?.Codigo.ToString() ?? "";
                    objCarregamento.CodigoModeloVeicular = carga.Carregamento?.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty;

                    if (requestPelaPedido)
                        objCarregamento.ProtocolosPedidos = (from obj in carga.Pedidos select obj.Pedido.Protocolo).ToList();
                    else
                    {
                        objCarregamento.CodigoIntegracaoFilial = carga.Filial?.CodigoFilialEmbarcador;
                        objCarregamento.Filial = servicoFilial.ConverterObjetoFilial(carga.Filial);
                        objCarregamento.ProtocoloCarregamento = carga.Carregamento?.Codigo ?? 0;
                        objCarregamento.CodigoIntegracaoTipoOperacao = carga.TipoOperacao?.CodigoIntegracao;

                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacaoFrete = carga.Carregamento != null
                            ? simulacoesFrete.FirstOrDefault(x => x.Carregamento != null && x.Carregamento.Codigo == carga.Carregamento.Codigo)
                            : null;

                        if (simulacaoFrete != null)
                            objCarregamento.SimulacaoFrete = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.BlocoSimulacaoFreteCarregamento { ValorFrete = simulacaoFrete?.ValorFrete ?? 0 };

                        List<string> integracoes = (from obj in cargaPedido select obj.Pedido.TipoDeCarga?.CodigoTipoCargaEmbarcador ?? "").ToList();

                        objCarregamento.CodigoIntegracaoTipoCarga = integracoes.Count > 0 ? integracoes.FirstOrDefault() : "";
                        objCarregamento.ProtocolosPedidos = (from obj in cargaPedido orderby obj.OrdemEntrega select obj.Pedido.Protocolo).ToList();
                        objCarregamento.BlocosPedidosCarregamento = blocosCarregamento?.Where(x => x.Carregamento == carga.Carregamento?.Codigo).ToList() ?? new List<BlocosPedidosCarregamento>();

                        decimal valorFrete = simulacaoFrete?.ValorFrete ?? 0;
                        decimal peso = carga.DadosSumarizados?.PesoTotal ?? 0;

                        objCarregamento.TONSimulado = (valorFrete != 0 && peso != 0) ? ((valorFrete / peso) * 1000).ToString("n2") : 0.ToString("n2");
                        objCarregamento.NumeroCarga = carga.CodigoCargaEmbarcador ?? "";

                        objCarregamento.TransportadoraEmitente = (carga.Carregamento?.Empresa == null ? new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() : servicoEmpresa.ConverterObjetoEmpresa(carga.Carregamento?.Empresa));
                        objCarregamento.CodigoTipoCarregamento = carga.TipoCarregamento?.Codigo;
                        objCarregamento.DescricaoTipoCarregamento = carga.TipoCarregamento?.Descricao ?? "";
                    }
                    carregamento.Itens.Add(objCarregamento);
                }

                carregamento.NumeroTotalDeRegistro = repCarga.ContarCarregamentosPendentesIntegracao(configuracao.RetornarCargasAgrupadasCarregamento, configuracao.NaoRetornarCarregamentosSemData, (configuracaoDadosTransporte?.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte ?? false), configWS.RetornarCarregamentosSomenteCargasEmAgNF, codigoEmpresa, requestCarregamentosPendentesIntegracao.CodigoFilial, requestCarregamentosPendentesIntegracao.CodigoRegiao);

                Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou carregamentos pendentes de integração", _unitOfWork);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>.CriarRetornoSucesso(carregamento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as cargas pendentes integração");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarIntegracaoCarregamento(int protocolo)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(_unitOfWork);
            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocolo);

                if (carga == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi encontrado uma carga para o protocolo informado");

                if (carga.Carregamento == null && string.IsNullOrWhiteSpace(carga.IdentificadorDeRota))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi encontrado um carregamento para o protocolo informado");

                if (carga.CarregamentoIntegradoERP)
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao("A confirmação da integração já foi realizada anteriormente.");

                carga.CarregamentoIntegradoERP = true;

                if (carga.Carregamento != null)
                    repCarregamento.Atualizar(carga.Carregamento);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, "Confirmou integração do carregamento.", _unitOfWork);

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteintegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> integracoesPendentesOuProblema = repositorioCargaDadosTransporteintegracao.BuscarPorCarga(carga.Codigo, SituacaoIntegracao.ProblemaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracao in integracoesPendentesOuProblema)
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracao.ProblemaIntegracao = "";
                    repositorioCargaDadosTransporteintegracao.Atualizar(integracao);
                }

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("A carga informada não existe no Multi Embarcador");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Retorno<bool> EncerrarCarga(int protocoloIntegracaoCarga, string ObservacaoEncerramento)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Mensagem = "";
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloIntegracaoCarga);

                _unitOfWork.Start();

                retorno.Mensagem = serCarga.SolicitarEncerramentoCarga(carga.Codigo, ObservacaoEncerramento, _clienteURLAcesso.WebServiceConsultaCTe, _tipoServicoMultisoftware, _unitOfWork, _auditado);

                if (string.IsNullOrWhiteSpace(retorno.Mensagem))
                {
                    retorno.Objeto = true;
                    retorno.Status = true;
                    _unitOfWork.CommitChanges();
                }
                else
                {
                    retorno.Objeto = false;
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    _unitOfWork.Rollback();
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao encerrar a carga";
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> IntegracaoEncerrarCarga(Dominio.ObjetosDeValor.WebService.Carga.DadosViagemATS dadosViagem)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Mensagem = "";
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

                if (dadosViagem == null)
                    throw new Exception(@"Requisição não possui dados da viagem");

                int.TryParse(dadosViagem.CodigoExterno, out int codigoCarga);

                if (codigoCarga <= 0)
                    throw new Exception($"Código da carga não foi preenchido corretamente: {dadosViagem.CodigoExterno}");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(codigoCarga);

                if (carga == null)
                    throw new Exception($"Carga não encontrada no sistema com o código {dadosViagem.CodigoExterno}");

                _unitOfWork.Start();

                retorno.Mensagem = serCarga.SolicitarEncerramentoCarga(carga.Codigo, "", _clienteURLAcesso.WebServiceConsultaCTe, _tipoServicoMultisoftware, _unitOfWork, _auditado);

                if (string.IsNullOrWhiteSpace(retorno.Mensagem))
                {
                    retorno.Objeto = true;
                    retorno.Status = true;
                    _unitOfWork.CommitChanges();
                }
                else
                {
                    retorno.Objeto = false;
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    _unitOfWork.Rollback();
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao encerrar a carga";
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SetarPedidoCritico(Dominio.ObjetosDeValor.Embarcador.Carga.PedidoCritico pedidoCritico)
        {
            try
            {
                if (pedidoCritico.protocoloCarga == 0)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Necessário informar o protocolo de integração da carga");

                if (pedidoCritico.protocoloPedido == 0)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Necessário informar o protocolo de integração do pedido");

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarCargaPedidoPorProtocolo(pedidoCritico.protocoloPedido, pedidoCritico.protocoloCarga);

                if (cargaPedido == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possível enncontrar um pedido ou carga com os protocolos informados");

                _unitOfWork.Start();

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                servicoCarga.SetarPedidoCritico(cargaPedido, _auditado, _unitOfWork);
                string mensagemRetorno = "";

                if (cargaPedido.Pedido.PedidoCritico ?? false)
                    mensagemRetorno = $"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} marcado como crítico.";
                else
                    mensagemRetorno = $"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} marcado como não crítico.";


                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true, mensagemRetorno);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "RequestLog");
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Ocorreu uma falha ao atualizar as informações. {excecao.Message.ToString()}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados
        private string GerarHashJson(string json)
        {
            string hash = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // formato hexadecimal
                }
                hash = builder.ToString();
            }

            return hash;
        }

        private static Retorno<T> ObjetoCargaInvalido<T>(Retorno<T> retorno, string mensagem)
        {
            retorno.Status = false;
            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
            retorno.Mensagem = mensagem;
            return retorno;
        }
        private PreDocumento ConverteObjetoPreDocumento(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte)
        {
            Servicos.PreCTe serPreCte = new Servicos.PreCTe(_unitOfWork);
            Servicos.WebService.Pessoas.Pessoa servicoPessoa = new WebService.Pessoas.Pessoa(_unitOfWork);
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Embarcador.Localidades.Localidade(_unitOfWork);

            Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = cargaCte.PreCTe;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCte.Carga;

            PreDocumento preDocumento = new PreDocumento();
            preDocumento.ModeloDocumento = preCte?.ModeloDocumentoFiscal?.TipoDocumentoEmissao.ObterDescricao() ?? "Sem Modelo";
            preDocumento.XML = serPreCte.BuscarXMLPreCte(preCte);
            preDocumento.Remetente = servicoPessoa.ConverterObjetoPessoa(preCte?.Remetente?.Cliente);
            preDocumento.Destinatario = servicoPessoa.ConverterObjetoPessoa(preCte?.Destinatario?.Cliente);
            preDocumento.Tomador = servicoPessoa.ConverterObjetoPessoa(preCte?.Tomador?.Cliente);
            preDocumento.Expedidor = servicoPessoa.ConverterObjetoPessoa(preCte?.Expedidor?.Cliente);
            preDocumento.Recebedor = servicoPessoa.ConverterObjetoPessoa(preCte?.Recebedor?.Cliente);
            preDocumento.Origem = servicoLocalidade.ConverterObjetoLocalidade(preCte?.Remetente?.Cliente != null ? preCte?.Remetente.Cliente?.Localidade : preCte.Expedidor?.Cliente?.Localidade);
            preDocumento.Destino = servicoLocalidade.ConverterObjetoLocalidade(preCte?.Recebedor?.Cliente != null ? preCte?.Recebedor.Cliente?.Localidade : preCte.Destinatario?.Cliente?.Localidade);
            preDocumento.ListaNotas = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe nota in cargaCte.NotasFiscais)
                preDocumento.ListaNotas.Add(Servicos.Embarcador.Carga.CargaIntegracaoEDI.ConverterXmlNotaFiscalEmNota(nota.PedidoXMLNotaFiscal, _unitOfWork));

            return preDocumento;
        }

        private void AtualizarDadosCarregamento(Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans, DateTime dataCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
            {
                BloquearJanelaCarregamentoExcedente = configuracaoTMS.BloquearGeracaoCargaComJanelaCarregamentoExcedente,
                PermitirHorarioCarregamentoComLimiteAtingido = true
            };

            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(_unitOfWork, configuracaoTMS, _auditado, configuracaoDisponibilidadeCarregamento);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(dadosTrans.Carga.Codigo);

            dadosTrans.Carga.DataCarregamentoCarga = dataCarregamento;

            if (cargaJanelaCarregamento == null)
                return;

            servicoDisponibilidadeCarregamento.AlterarHorarioCarregamento(cargaJanelaCarregamento, dataCarregamento, _tipoServicoMultisoftware);
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao> BuscarSituacaoCargasPedidos(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao> cargasIntegracao = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaSituacao();

                cargaIntegracao.ProtocoloCarga = cargaPedido.Carga.Codigo;
                cargaIntegracao.NumeroCarga = cargaPedido.Carga.CodigoCargaEmbarcador;
                cargaIntegracao.SituacaoCarga = cargaPedido.Carga.SituacaoCarga;
                cargaIntegracao.Mensagem = ObterMensagemProblemaCarga(cargaPedido.Carga, unitOfWork);

                cargasIntegracao.Add(cargaIntegracao);
            }

            return cargasIntegracao;
        }

        private string ObterMensagemProblemaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
            {
                if (carga.ProblemaIntegracaoValePedagio)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> integracoeslePedagio = repCargaIntegracaoValePedagio.BuscarPorCarga(carga.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio integracaoProblema = integracoeslePedagio.FirstOrDefault(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                    if (integracaoProblema != null)
                        return integracaoProblema.ProblemaIntegracao;
                    else
                        return "Não foi possível realizar a compra do vale pedágio.";
                }
                if (carga.problemaAverbacaoCTe)
                {
                    return "Não foi possível realizar a averbação do(s) CTe(s).";
                }
                if (carga.ProblemaAverbacaoMDFe)
                {
                    return "Não foi possível realizar a averbação do(s) MDFe(s).";
                }
                if (carga.problemaCTE)
                {
                    return "CTe(s) não autorizaram no Sefaz.";
                }
                if (carga.problemaNFS)
                {
                    return "NFSe(s) não autorizaram na Prefeitura.";
                }
                if (carga.problemaMDFe)
                {
                    return "MDFe(s) não autorizaram no Sefaz.";
                }
            }

            return string.Empty;
        }

        public List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> BuscarCargasPedidos(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaVeiculoContainer repCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);
            Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorioAprovacao = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);

            Servicos.WebService.Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Endereco serEndereco = new Embarcador.Localidades.Endereco(unitOfWork);
            Servicos.WebService.Filial.Filial serWSFilial = new Filial.Filial(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.WebService.Carga.ModeloVeicularCarga serModeloVeicularCarga = new ModeloVeicularCarga(unitOfWork);
            Servicos.WebService.Empresa.Motorista serWSMotorista = new Empresa.Motorista(unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido serProdutosPedido = new ProdutosPedido(unitOfWork);
            Servicos.WebService.Carga.TipoCarga serWSTipoCarga = new Servicos.WebService.Carga.TipoCarga(unitOfWork);
            Servicos.WebService.Carga.TipoOperacao serWSTipoOperacao = new Servicos.WebService.Carga.TipoOperacao(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Frota.Veiculo(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracao = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>();
            if (mensagem != null)
            {
                if (cargaPedidos == null || cargaPedidos.Count == 0)
                {
                    Servicos.Log.TratarErro("Pedidos não localizados.", "RequestLog");
                    mensagem = "Pedidos não localizados.";
                    return null;
                }
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                List<int> codigosCargas = (from obj in cargaPedidos select obj.Carga.Codigo).Distinct().ToList();
                if (codigosCargas.Count > 1)
                    cargas = repCarga.BuscarPorCodigos(codigosCargas);
                else
                    cargas.Add(repCarga.BuscarPorCodigoFetch(codigosCargas.FirstOrDefault()));

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaOrigens = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                List<int> codigosOrigens = (from obj in cargaPedidos select obj.CargaOrigem.Codigo).Distinct().ToList();
                if (codigosOrigens.Count > 1)
                    cargaOrigens = repCarga.BuscarPorCodigos(codigosOrigens);
                else
                {
                    if (codigosCargas.Count == codigosOrigens.Count && codigosCargas.FirstOrDefault() == codigosOrigens.FirstOrDefault())
                        cargaOrigens.Add(cargas.FirstOrDefault());
                    else
                        cargaOrigens.Add(repCarga.BuscarPorCodigoFetch(codigosOrigens.FirstOrDefault()));
                }

                if (cargas == null || cargas.Count == 0)
                {
                    Servicos.Log.TratarErro("Carga não localizada 1", "RequestLog");
                    mensagem = "Carga não localizada";
                    return null;
                }

                try
                {
                    codigosCargas = (from obj in cargas select obj.Codigo).ToList();
                }
                catch
                {
                    Servicos.Log.TratarErro("Carga não localizada 2", "RequestLog");
                    mensagem = "Carga não localizada";
                    return null;
                }

                if (codigosCargas == null || codigosCargas.Count == 0)
                {
                    Servicos.Log.TratarErro("Carga não localizada 3", "RequestLog");
                    mensagem = "Carga não localizada";
                    return null;
                }

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCargas(codigosCargas);
                List<(int CodigoCarga, CargaDadosSumarizados DadosSumarizados)> cargaCargasDadosSumarizados = repCargaDadosSumarizados.BuscarPorCargas(codigosCargas);
                List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga> listaAutorizacaoCargas = new List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>();

                if (cargas.Exists(obj => obj.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador))
                    listaAutorizacaoCargas = repositorioAprovacao.ConsultaPorCargas(codigosCargas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocosCargas = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento>();
                if (cargas.Exists(obj => obj.Carregamento != null))
                    blocosCargas = repBlocoCarregamento.BuscarPorCarregamentos((from obj in cargas where obj.Carregamento != null select obj.Carregamento.Codigo).Distinct().ToList());

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutosCargas = repCargaPedidoProduto.BuscarPorCargas(codigosCargas);
                List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> cargaVeiculoContainerCargas = repCargaVeiculoContainer.BuscarPorCargas(codigosCargas);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentesCarga = repCargaPedidoComponentesFrete.BuscarPorCargasSemComponenteFreteValor(codigosCargas, false);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargas);

                List<Dominio.Entidades.Embarcador.Acerto.AcertoCarga> acertoCargas = repAcertoCarga.BuscarPorCodigosCargaSituacao(codigosCargas, SituacaoAcertoViagem.Fechado);
                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos = repMonitoramento.BuscarPorCargas(codigosCargas);

                List<int> codigosCargaPedidos = (from obj in cargaPedidos select obj.Codigo).Distinct().ToList();
                List<int> codigosPedidos = (from obj in cargaPedidos select obj.Pedido.Codigo).Distinct().ToList();

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> pedidosAdicionais = repPedidoAdicional.BuscarPorPedidos(codigosPedidos);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repositorioCargaEntrega.BuscarPorCodigosCargaPedido(codigosCargaPedidos);

                List<(int CodigoPedido, int CodigoProduto, bool Desmembrado)> listaPedidosProdutosDesmembrados = new List<(int CodigoPedido, int CodigoProduto, bool Desmembrado)>();

                if (configuracaoWebService.PermiteReceberDataCriacaoPedidoERP)
                    listaPedidosProdutosDesmembrados = repositorioPedidoProduto.BuscarPedidosProdutosDesmembradosPorPedido(cargaPedidos.Select(x => x.Pedido.Codigo).ToList());

                Embarcador.Carga.CargaFronteira serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
                List<CargaFronteira> fronteiras = serCargaFronteira.ObterFronteirasPorCargas(cargas);

                bool retornarModeloVeiculo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().RetornarModeloVeiculo.Value; //Criado com urgência pois minerva solicitou urgência para retornar o modelo do veiculo

                List<int> codigosVeiculo = cargas.Where(o => o.Veiculo != null).Select(o => o.Veiculo.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> veiculosMotoristas = new List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();

                if (codigosVeiculo.Count > 0)
                    veiculosMotoristas = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(codigosVeiculo);

                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao> cargasConsultaValorPedagioIntegracao = repCargaConsultaValorPedagioIntegracao.ConsultaIntegracaoPorCargas(cargas.Select(o => o.Codigo).ToList(), null);

                string urlBase = string.Empty;

                if (!string.IsNullOrWhiteSpace(_adminStringConexao) && _unitOfWork != null && _tipoServicoMultisoftware != 0)
                {
                    AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_adminStringConexao);
                    urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(_clienteMultisoftware.Codigo, _tipoServicoMultisoftware, unitOfWorkAdmin, _adminStringConexao, _unitOfWork);
                    unitOfWorkAdmin.Dispose();
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = (from obj in cargas where obj.Codigo == cargaPedido.Carga.Codigo select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = (from obj in cargaOrigens where obj.Codigo == cargaPedido.CargaOrigem.Codigo select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == carga.Codigo select o).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados cargaDadosSumarizados = (from obj in cargaCargasDadosSumarizados where obj.CodigoCarga == cargaPedido.Carga.Codigo select obj.DadosSumarizados).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Acerto.AcertoCarga acertoCarga = acertoCargas.FirstOrDefault(acerto => acerto.Carga.Codigo == carga.Codigo);
                    Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = monitoramentos.FirstOrDefault(m => m.Carga.Codigo == carga.Codigo);

                    Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao()
                    {
                        Distancia = cargaDadosSumarizados?.Distancia ?? 0
                    };

                    if (configuracaoTMS.RetornarDataCarregamentoDaCargaNaConsulta)
                    {
                        if (carga.Filial?.Codigo == cargaOrigem.Filial?.Codigo)
                        {
                            cargaIntegracao.DataInicioCarregamento = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                            cargaIntegracao.DataFinalCarregamento = carga.DataPrevisaoTerminoCarga.HasValue ? carga.DataPrevisaoTerminoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                        }
                        else
                        {
                            cargaIntegracao.DataInicioCarregamento = cargaOrigem.DataCarregamentoCarga.HasValue ? cargaOrigem.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                            cargaIntegracao.DataFinalCarregamento = cargaOrigem.DataPrevisaoTerminoCarga.HasValue ? cargaOrigem.DataPrevisaoTerminoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : cargaOrigem.DataCarregamentoCarga.HasValue ? cargaOrigem.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                        }
                    }
                    else
                    {
                        if (cargaJanelaCarregamento != null)
                        {
                            cargaIntegracao.DataInicioCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm:ss");
                            cargaIntegracao.DataFinalCarregamento = cargaJanelaCarregamento.TerminoCarregamento.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                        else
                        {
                            cargaIntegracao.DataFinalCarregamento = carga.DataFinalPrevisaoCarregamento.HasValue ? carga.DataFinalPrevisaoCarregamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
                            cargaIntegracao.DataInicioCarregamento = carga.DataInicialPrevisaoCarregamento.HasValue ? carga.DataInicialPrevisaoCarregamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
                        }
                    }

                    cargaIntegracao.DataAgendamento = cargaPedido.Pedido.DataAgendamento?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                    cargaIntegracao.NumeroAcerto = acertoCarga?.AcertoViagem.Numero ?? 0;
                    cargaIntegracao.DataInicialAcerto = acertoCarga?.AcertoViagem?.DataInicial.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                    cargaIntegracao.DataFinalAcerto = acertoCarga?.AcertoViagem?.DataFinal.Value.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                    cargaIntegracao.DataCriacaoCarga = carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm:ss");
                    cargaIntegracao.DataPrevisaoEntrega = carga.DataPrevisaoTerminoCarga.HasValue ? carga.DataPrevisaoTerminoCarga.Value.ToString("dd/MM/yyyy") : "";
                    cargaIntegracao.HoraPrevisaoEntrega = carga.DataPrevisaoTerminoCarga.HasValue ? carga.DataPrevisaoTerminoCarga.Value.ToString("HH:mm") : "";
                    cargaIntegracao.Destinatario = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Destinatario);
                    cargaIntegracao.CargaDePreCarga = carga.CargaDePreCarga;
                    cargaIntegracao.TipoDirecionamentoCustoExtra = carga.TipoOperacao?.ConfiguracaoCarga?.DirecionamentoCustoExtra ?? TipoDirecionamentoCustoExtra.Nenhum;
                    cargaIntegracao.StatusCustoExtra = carga.StatusCustoExtra ?? StatusCustoExtra.EmAberto;

                    cargaIntegracao.TipoCarregamento = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCarregamento()
                    {
                        Descricao = carga?.TipoCarregamento?.Descricao ?? string.Empty,
                        CodigoIntegracao = carga?.TipoCarregamento?.CodigoIntegracao ?? string.Empty
                    };

                    cargaIntegracao.DataPrevisaoChegadaDestinatario = cargaPedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";

                    if (cargaPedido.Recebedor != null)
                        cargaIntegracao.Recebedor = serPessoa.ConverterObjetoPessoa(cargaPedido.Recebedor);

                    cargaIntegracao.UsarOutroEnderecoDestino = cargaPedido.Pedido.UsarOutroEnderecoDestino;

                    cargaIntegracao.OperadorCargaNome = carga.Operador?.Nome ?? "";
                    cargaIntegracao.OperadorCargaCPF = carga.Operador?.CPF_Formatado ?? "";
                    cargaIntegracao.OperadorCargaEmail = carga.Operador?.Email ?? "";

                    if (cargaPedido.Pedido != null && cargaPedido.Pedido.EnderecoRecebedor != null)
                        cargaIntegracao.Destino = serEndereco.ConverterObjetoEnderecoPedido(cargaPedido.Pedido.EnderecoRecebedor);
                    else if (cargaIntegracao.Recebedor != null && cargaIntegracao.Recebedor.Endereco != null)
                        cargaIntegracao.Destino = cargaIntegracao.Recebedor.Endereco;
                    else if (cargaIntegracao.UsarOutroEnderecoDestino && cargaPedido.Pedido != null && cargaPedido.Pedido.EnderecoDestino != null)
                        cargaIntegracao.Destino = serEndereco.ConverterObjetoEnderecoPedido(cargaPedido.Pedido.EnderecoDestino);
                    else if (cargaIntegracao.Destinatario != null && cargaIntegracao.Destinatario.Endereco != null)
                        cargaIntegracao.Destino = cargaIntegracao.Destinatario.Endereco;

                    cargaIntegracao.Expedidor = serPessoa.ConverterObjetoPessoa(cargaPedido.Expedidor);

                    cargaIntegracao.Remetente = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Remetente);
                    cargaIntegracao.UsarOutroEnderecoOrigem = cargaPedido.Pedido.UsarOutroEnderecoOrigem;
                    if (cargaPedido.Pedido != null && cargaPedido.Pedido.EnderecoExpedidor != null)
                        cargaIntegracao.Origem = serEndereco.ConverterObjetoEnderecoPedido(cargaPedido.Pedido.EnderecoExpedidor);
                    else if (cargaIntegracao.UsarOutroEnderecoOrigem)
                        cargaIntegracao.Origem = serEndereco.ConverterObjetoEnderecoPedido(cargaPedido.Pedido.EnderecoOrigem);
                    else if (cargaIntegracao.Expedidor != null)
                        cargaIntegracao.Origem = cargaIntegracao.Expedidor.Endereco;
                    else
                        cargaIntegracao.Origem = cargaIntegracao.Remetente.Endereco;

                    cargaIntegracao.Filial = serWSFilial.ConverterObjetoFilial(cargaOrigem.Filial);

                    CargaFronteira fronteiraPrincipalCarga = serCargaFronteira.ObterFronteiraPrincipal(carga, fronteiras);
                    cargaIntegracao.Fronteira = serLocalidade.ConverterObjetoFronteira(fronteiraPrincipalCarga?.Fronteira);

                    cargaIntegracao.ModeloVeicular = serModeloVeicularCarga.ConverterObjetoModeloVeicular(cargaOrigem.ModeloVeicularOrigem != null ? cargaOrigem.ModeloVeicularOrigem : carga.ModeloVeicularCarga);

                    cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

                    List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> motoristas = (from obj in cargaMotoristas where obj.Carga.Codigo == carga.Codigo select obj).ToList();

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista motorista in motoristas)
                        cargaIntegracao.Motoristas.Add(serWSMotorista.ConverterObjetoMotorista(motorista.Motorista));

                    if (carga.Carregamento != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocos = (from obj in blocosCargas where obj.Carregamento.Codigo == carga.Carregamento.Codigo && obj.Pedido.Codigo == cargaPedido.Pedido.Codigo select obj).ToList();

                        if (blocos.Count > 0)
                        {
                            cargaIntegracao.BlocosCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamento>();
                            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento bloco in blocos)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamento blocoCarregamento = new Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamento();
                                blocoCarregamento.Descricao = bloco.Bloco;
                                blocoCarregamento.CodigoIntegracao = bloco.Bloco;
                                cargaIntegracao.BlocosCarregamento.Add(blocoCarregamento);
                            }
                        }
                    }

                    cargaIntegracao.ProtocoloCarga = configuracaoTMS.RetornarCargasAgrupadasCarregamento && carga.CargaAgrupada ? cargaOrigem.Protocolo : carga.Protocolo;
                    cargaIntegracao.ProtocoloPedido = cargaPedido.Pedido.Protocolo;
                    cargaIntegracao.NumeroCarga = carga.CodigoCargaEmbarcador;
                    cargaIntegracao.NumeroPaletes = cargaPedido.Pedido.NumeroPaletes;
                    cargaIntegracao.NumeroPaletesFracionado = cargaPedido.Pedido.NumeroPaletesFracionado;
                    cargaIntegracao.NumeroPedidoEmbarcador = cargaPedido.Pedido.NumeroPedidoEmbarcador;
                    cargaIntegracao.TipoPedido = cargaPedido.Pedido.TipoPedido;
                    cargaIntegracao.PesoBruto = cargaPedido.Peso; //cargaPedido.Pedido.PesoTotal;
                    cargaIntegracao.PesoLiquido = cargaPedido.PesoLiquido; //cargaPedido.Pedido.PesoLiquidoTotal;
                    cargaIntegracao.CubagemTotal = cargaPedido.Pedido.CubagemTotal;
                    cargaIntegracao.PesoTotalPaletes = cargaPedido.Pedido.PesoTotalPaletes;
                    cargaIntegracao.CodigoIntegracaoRota = carga.Rota?.CodigoIntegracao ?? string.Empty;
                    cargaIntegracao.DescricaoRota = carga.Rota?.Descricao ?? string.Empty;
                    cargaIntegracao.IdentificadorRota = carga.IdentificadorDeRota;
                    cargaIntegracao.OrdemEntrega = cargaPedido.OrdemEntrega;
                    cargaIntegracao.OrdemColeta = cargaPedido.OrdemColeta;
                    cargaIntegracao.NumeroPager = carga.NumeroPager;
                    cargaIntegracao.ETA = cargaPedido.Pedido.DataETA.HasValue ? cargaPedido.Pedido.DataETA.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    cargaIntegracao.ValorTotalPedido = cargaPedido.Pedido.ValorTotalNotasFiscais;
                    cargaIntegracao.QuantidadeVolumes = cargaPedido.Pedido.QtVolumes;
                    cargaIntegracao.TipoPaleteCliente = cargaPedido.Pedido.TipoPaleteCliente;

                    if (cargaPedido.Pedido != null && cargaPedido.Pedido.TipoOperacao != null)
                        cargaIntegracao.TipoOperacaoPedido = serWSTipoOperacao.ConverterObjetoTipoOperacao(cargaPedido.Pedido.TipoOperacao);

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregas.FirstOrDefault(obj => obj.Pedidos.Any(o => o.CargaPedido.Codigo == cargaPedido.Codigo));

                    if (cargaEntrega != null)
                    {
                        cargaIntegracao.KMOrigemXDestino = cargaEntrega.Distancia > 0 ? (cargaEntrega.Distancia / 1000).ToString("n3") : "";
                    }

                    if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador)
                    {
                        cargaIntegracao.FreteNegociado = new Dominio.ObjetosDeValor.Embarcador.Carga.FreteNegociado();
                        cargaIntegracao.FreteNegociado.CPFOperador = carga.Operador?.CPF;
                        cargaIntegracao.FreteNegociado.NomeOperador = carga.Operador?.Nome;

                        List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga> listaAutorizacao = (from obj in listaAutorizacaoCargas where obj.OrigemAprovacao.Codigo == carga.Codigo select obj).ToList();
                        if (listaAutorizacao != null && listaAutorizacao.Count > 0)
                        {
                            cargaIntegracao.FreteNegociado.Aprovadores = new List<Dominio.ObjetosDeValor.Embarcador.Carga.FreteNegociadoAprovador>();
                            foreach (Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga aprovacao in listaAutorizacao)
                            {
                                if (aprovacao.Usuario != null)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Carga.FreteNegociadoAprovador aprovador = new Dominio.ObjetosDeValor.Embarcador.Carga.FreteNegociadoAprovador();
                                    aprovador.CPF = aprovacao.Usuario.CPF;
                                    aprovador.Nome = aprovacao.Usuario.Nome;
                                    cargaIntegracao.FreteNegociado.Aprovadores.Add(aprovador);
                                }
                            }
                        }
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = (from obj in cargaPedidoProdutosCargas where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();
                    cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();

                    if (configuracaoWebService.PermiteReceberDataCriacaoPedidoERP)
                        cargaIntegracao.PedidoAlterado = cargaPedido.Pedido.ItensAtualizados;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = serProdutosPedido.ConveterObjetoProduto(cargaPedidoProduto);

                        (int _, int _, bool Desmembrado) = listaPedidosProdutosDesmembrados
                            .FirstOrDefault(p =>
                                p.CodigoPedido == cargaPedido.Pedido.Codigo &&
                                p.CodigoProduto == cargaPedidoProduto.Produto.Codigo);

                        if (Desmembrado)
                            cargaIntegracao.Mesclar = Desmembrado;

                        produto.Mesclar = Desmembrado;

                        cargaIntegracao.Produtos.Add(produto);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga = carga.SituacaoCarga;
                    if (situacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || situacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(carga.Codigo);
                        if (cargaCancelamento != null)
                            situacaoCarga = cargaCancelamento.SituacaoCargaNoCancelamento;
                    }

                    if (situacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                        && situacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete
                        && (!carga.ExigeNotaFiscalParaCalcularFrete || situacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe))
                        cargaIntegracao.ValorFreteCalculado = true;

                    cargaIntegracao.Recebedor = serPessoa.ConverterObjetoPessoa(cargaPedido.Recebedor);
                    cargaIntegracao.TipoCargaEmbarcador = serWSTipoCarga.ConverterObjetoTipoCarga(carga.TipoDeCarga);
                    cargaIntegracao.TipoOperacao = serWSTipoOperacao.ConverterObjetoTipoOperacao(carga.TipoOperacao);
                    cargaIntegracao.TipoPagamento = cargaPedido.Pedido.TipoPagamento;
                    cargaIntegracao.TipoTomador = cargaPedido.TipoTomador;

                    if (cargaIntegracao.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                        cargaIntegracao.Tomador = cargaIntegracao.Remetente;
                    else if (cargaIntegracao.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                        cargaIntegracao.Tomador = cargaIntegracao.Destinatario;
                    else
                        cargaIntegracao.Tomador = serPessoa.ConverterObjetoPessoa(cargaPedido.Tomador);

                    cargaIntegracao.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(carga.Empresa);
                    cargaIntegracao.ValorTotalPaletes = cargaPedido.Pedido.ValorTotalPaletes;


                    if (cargaPedido.NumeroReboque == Dominio.ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque.SemReboque)
                    {
                        int codigoVeiculoReboque = carga.VeiculosVinculados.Count > 0 ? carga.VeiculosVinculados.ToList()[0].Codigo : 0;

                        Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer = (from obj in cargaVeiculoContainerCargas where obj.Carga.Codigo == carga.Codigo && obj.Veiculo.Codigo == codigoVeiculoReboque select obj).FirstOrDefault();

                        cargaIntegracao.Veiculo = serWSVeiculo.ConverterObjetoConjuntoVeiculos(carga.Veiculo, carga.VeiculosVinculados.ToList(), unitOfWork, 1, cargaVeiculoContainer, retornarModeloVeiculo, configuracaoTMS, veiculosMotoristas);
                    }
                    else
                    {
                        if ((cargaPedido.NumeroReboque == Dominio.ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque.ReboqueUm && carga.VeiculosVinculados.Count > 0) || (cargaPedido.NumeroReboque == Dominio.ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque.ReboqueDois && carga.VeiculosVinculados.Count == 1))
                        {
                            List<Dominio.Entidades.Veiculo> listaReboque = new List<Dominio.Entidades.Veiculo>();
                            listaReboque.Add(carga.VeiculosVinculados.ToList()[0]);

                            Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer = (from obj in cargaVeiculoContainerCargas where obj.Carga.Codigo == carga.Codigo && obj.Veiculo.Codigo == carga.VeiculosVinculados.ToList()[0].Codigo select obj).FirstOrDefault();

                            cargaIntegracao.Veiculo = serWSVeiculo.ConverterObjetoConjuntoVeiculos(carga.Veiculo, listaReboque, unitOfWork, 1, cargaVeiculoContainer, retornarModeloVeiculo, configuracaoTMS, veiculosMotoristas);
                        }
                        else if (cargaPedido.NumeroReboque == Dominio.ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque.ReboqueDois && carga.VeiculosVinculados.Count > 1)
                        {
                            List<Dominio.Entidades.Veiculo> listaReboque = new List<Dominio.Entidades.Veiculo>();
                            listaReboque.Add(carga.VeiculosVinculados.ToList()[1]);

                            Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer = (from obj in cargaVeiculoContainerCargas where obj.Carga.Codigo == carga.Codigo && obj.Veiculo.Codigo == carga.VeiculosVinculados.ToList()[1].Codigo select obj).FirstOrDefault();

                            cargaIntegracao.Veiculo = serWSVeiculo.ConverterObjetoConjuntoVeiculos(carga.Veiculo, listaReboque, unitOfWork, 2, cargaVeiculoContainer, retornarModeloVeiculo, configuracaoTMS, veiculosMotoristas);
                        }
                    }

                    if (cargaIntegracao.Veiculo != null)
                    {
                        if (cargaIntegracao.Veiculo.Reboques != null && cargaIntegracao.Veiculo.Reboques.Count > 0)
                            cargaIntegracao.VeiculoDaNota = cargaIntegracao.Veiculo.Reboques.LastOrDefault();
                        else
                            cargaIntegracao.VeiculoDaNota = cargaIntegracao.Veiculo;
                    }

                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                    bool possuiPreCTe = repCargaCte.PossuiPreCTe(cargaPedido?.Carga?.Codigo ?? 0);

                    if (!configuracaoWebService.QuandoGeradoPreCteRetornarInformacaoDeFreteCTeIntegrado || !possuiPreCTe)
                    {
                        PreencherDadosValorFretePadrao(cargaIntegracao, cargaPedido, carga, componentesCarga, unitOfWork);
                    }
                    else
                    {
                        PreencherDadosValorFreteComCte(cargaIntegracao, cargaPedido, carga, ref mensagem, unitOfWork);
                    }

                    //Dados do multimodal
                    cargaIntegracao.NumeroBooking = cargaPedido.Pedido.NumeroBooking;
                    cargaIntegracao.Container = ConverterObjetoContainer(cargaPedido.Pedido.Container);
                    cargaIntegracao.Viagem = ConverterObjetoViagem(cargaPedido.Pedido.PedidoViagemNavio);
                    cargaIntegracao.Embarcador = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Remetente);
                    cargaIntegracao.ViagemLongoCurso = ConverterObjetoViagem(cargaPedido.Pedido.PedidoViagemNavioLongoCurso);
                    cargaIntegracao.PortoOrigem = ConverterObjetoPorto(cargaPedido.Pedido.Porto);
                    cargaIntegracao.PortoDestino = ConverterObjetoPorto(cargaPedido.Pedido.PortoDestino);
                    cargaIntegracao.TerminalPortoOrigem = ConverterObjetoTerminalPorto(cargaPedido.Pedido.TerminalOrigem);
                    cargaIntegracao.TerminalPortoDestino = ConverterObjetoTerminalPorto(cargaPedido.Pedido.TerminalDestino);
                    cargaIntegracao.TipoContainerReserva = ConverterObjetoTipoContainer(cargaPedido.Pedido.ContainerTipoReserva);
                    cargaIntegracao.ContemCargaPerigosa = cargaPedido.Pedido.PossuiCargaPerigosa;
                    cargaIntegracao.ContemCargaRefrigerada = cargaPedido.Pedido.ContemCargaRefrigerada;
                    cargaIntegracao.TemperaturaObservacao = cargaPedido.Pedido.ObservacaoCTe;
                    cargaIntegracao.Temperatura = cargaPedido.Pedido.Temperatura;
                    cargaIntegracao.ValidarNumeroContainer = cargaPedido.Pedido.ValidarDigitoVerificadorContainer;
                    cargaIntegracao.PropostaComercial = ConverterObjetoPropostaComercial(cargaPedido.Pedido);

                    if (configuracaoTMS.UtilizaEmissaoMultimodal)
                        cargaIntegracao.Transbordo = ConverterObjetoTransbordo(cargaPedido.Pedido.PedidosTransbordo != null ? cargaPedido.Pedido.PedidosTransbordo.ToList() : null);

                    int.TryParse(cargaPedido.Pedido.CodigoOS, out int codigoOS);
                    cargaIntegracao.CodigoOrdemServico = codigoOS;
                    cargaIntegracao.NumeroOrdemServico = cargaPedido.Pedido.NumeroOS;
                    cargaIntegracao.Embarque = cargaPedido.Pedido.Embarque;
                    cargaIntegracao.MasterBL = cargaPedido.Pedido.MasterBL;
                    cargaIntegracao.NumeroDIEmbarque = cargaPedido.Pedido.NumeroDI;
                    cargaIntegracao.ProvedorOS = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.ProvedorOS);
                    cargaIntegracao.TipoServicoCarga = cargaPedido.Pedido.TipoServicoCarga;
                    int.TryParse(cargaPedido.Pedido.TaraContainer, out int taraContainer);
                    cargaIntegracao.TaraContainer = taraContainer;
                    if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.LacreContainerUm))
                        cargaIntegracao.NumeroLacre1 = cargaPedido.Pedido.LacreContainerUm;
                    if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.LacreContainerDois))
                        cargaIntegracao.NumeroLacre2 = cargaPedido.Pedido.LacreContainerDois;
                    if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.LacreContainerTres))
                        cargaIntegracao.NumeroLacre3 = cargaPedido.Pedido.LacreContainerTres;
                    int.TryParse(cargaPedido.Pedido.CodigoBooking, out int codigoBooking);
                    cargaIntegracao.CodigoBooking = codigoBooking;
                    cargaIntegracao.NumeroBL = cargaPedido.Pedido.NumeroBL;
                    cargaIntegracao.NecessitaAverbacao = cargaPedido.Pedido.NecessitaAverbacaoAutomatica;
                    cargaIntegracao.CargaRefrigeradaPrecisaEnergia = cargaPedido.Pedido.NecessitaEnergiaContainerRefrigerado;
                    cargaIntegracao.QuantidadeTipoContainerReserva = cargaPedido.Pedido.QuantidadeTipoContainerReserva;
                    cargaIntegracao.IMOClasse = cargaPedido.Pedido.IMOClasse;
                    cargaIntegracao.IMOSequencia = cargaPedido.Pedido.IMOSequencia;
                    cargaIntegracao.IMOUnidade = cargaPedido.Pedido.IMOUnidade;
                    cargaIntegracao.FormaAverbacaoCTE = cargaPedido.Pedido.FormaAverbacaoCTE;
                    cargaIntegracao.PercentualADValorem = cargaPedido.Pedido.ValorAdValorem;
                    cargaIntegracao.TipoDocumentoAverbacao = cargaPedido.Pedido.TipoDocumentoAverbacao;
                    cargaIntegracao.ObservacaoProposta = cargaPedido.Pedido.ObservacaoCTe;
                    cargaIntegracao.TipoPropostaFeeder = cargaPedido.Pedido.TipoPropostaFeeder;
                    cargaIntegracao.DescricaoTipoPropostaFeeder = cargaPedido.Pedido.DescricaoTipoPropostaFeeder;
                    cargaIntegracao.DescricaoCarrierNavioViagem = cargaPedido.Pedido.DescricaoCarrierNavioViagem;
                    cargaIntegracao.RealizarCobrancaTaxaDocumentacao = cargaPedido.Pedido.RealizarCobrancaTaxaDocumentacao;
                    cargaIntegracao.QuantidadeConhecimentosTaxaDocumentacao = cargaPedido.Pedido.QuantidadeConhecimentosTaxaDocumentacao;
                    cargaIntegracao.ValorTaxaDocumento = cargaPedido.Pedido.ValorTaxaDocumento;
                    cargaIntegracao.ContainerADefinir = cargaPedido.Pedido.ContainerADefinir;
                    cargaIntegracao.ValorCusteioSVM = cargaPedido.Pedido.ValorCusteioSVM;
                    cargaIntegracao.QuantidadeContainerBooking = cargaPedido.Pedido.QuantidadeContainerBooking;
                    cargaIntegracao.EmpresaResponsavel = ConverterObjetoEmpresaResponsavel(cargaPedido.Pedido.PedidoEmpresaResponsavel);
                    cargaIntegracao.CentroCusto = ConverterObjetoCentroCustol(cargaPedido.Pedido.PedidoCentroCusto);
                    cargaIntegracao.PedidoDeSVMTerceiro = cargaPedido.Pedido.PedidoDeSVMTerceiro;
                    cargaIntegracao.NumeroCE = cargaPedido.Pedido.NumeroCEFeeder;
                    cargaIntegracao.ValorTaxaFeeder = cargaPedido.Pedido.ValorTaxaFeeder;
                    cargaIntegracao.TipoCalculoCargaFracionada = cargaPedido.Pedido.TipoCalculoCargaFracionada;
                    cargaIntegracao.SituacaoCarga = carga.SituacaoCarga;
                    cargaIntegracao.DataInicioViagem = carga.DataInicioViagem.HasValue ? carga.DataInicioViagem.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    cargaIntegracao.ExecaoCab = null;
                    cargaIntegracao.ObsCarregamento = carga.Carregamento?.Observacao ?? "";
                    cargaIntegracao.Observacao = cargaPedido.Pedido.Observacao;
                    cargaIntegracao.ObservacaoInterna = cargaPedido.Pedido.ObservacaoInterna;
                    cargaIntegracao.CodigoPedidoCliente = cargaPedido.Pedido.CodigoPedidoCliente ?? string.Empty;

                    // cria a lista em cimae pega pela lista
                    Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = pedidosAdicionais.FirstOrDefault(ped => ped.Pedido.Codigo == (cargaPedido.Pedido?.Codigo ?? 0));

                    if (pedidoAdicional != null)
                        cargaIntegracao.ExecaoCab = pedidoAdicional.ExecaoCab;

                    cargaIntegracao.PossuiPendenciaRoteirizacao = (configuracaoTMS.ExigirRotaRoteirizadaNaCarga && carga.SituacaoRoteirizacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro);

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                    {
                        if (carga.CargaCancelamento != null && carga.CargaCancelamento.DataCancelamento.HasValue)
                            cargaIntegracao.DataCancelamentoCarga = carga.CargaCancelamento?.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss");
                        else
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(carga.Codigo);
                            if (cargaCancelamento != null && cargaCancelamento.DataCancelamento.HasValue)
                                cargaIntegracao.DataCancelamentoCarga = cargaCancelamento.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                    }
                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                    {
                        if (carga.CargaCancelamento != null && carga.CargaCancelamento.DataCancelamento.HasValue)
                            cargaIntegracao.DataAnulacaoCarga = carga.CargaCancelamento?.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss");
                        else
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(carga.Codigo);
                            if (cargaCancelamento != null && cargaCancelamento.DataCancelamento.HasValue)
                                cargaIntegracao.DataAnulacaoCarga = cargaCancelamento.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                    }

                    if (monitoramento != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoVeiculo = repPosicao.BuscarPrimeiroWaypointPorMonitoramentoVeiculo(monitoramento.Codigo, monitoramento.Veiculo.Codigo, monitoramento.DataInicio.HasValue ? monitoramento.DataInicio.Value : monitoramento.DataCriacao.Value);
                        if (posicaoVeiculo != null)
                        {
                            cargaIntegracao.PrimeiraPosicaoMonitoramento = new Dominio.ObjetosDeValor.Monitoramento.Posicao()
                            {
                                Data = posicaoVeiculo.DataVeiculo,
                                Latitude = posicaoVeiculo.Latitude,
                                Longitude = posicaoVeiculo.Longitude
                            };
                        }

                        cargaIntegracao.SituacaoMonitoramento = monitoramento.Status.ObterDescricao();
                    }
                    else if (configuracaoTMS.PossuiMonitoramento && (configMonitoramento?.GerarMonitoramentoAoFecharCarga ?? false))//mesmo nao tendo, deve criar como agendado apos roteirizar
                        cargaIntegracao.SituacaoMonitoramento = Localization.Resources.Enumeradores.MonitoramentoStatus.Agendado;

                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValorPedagioIntegracao = cargasConsultaValorPedagioIntegracao.FirstOrDefault(o => o.Carga.Codigo == carga.Codigo);

                    if (cargaConsultaValorPedagioIntegracao != null)
                    {
                        cargaIntegracao.ConsultaValePedagio = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultaValePedagio()
                        {
                            ValorConsultaValePedagio = cargaConsultaValorPedagioIntegracao.ValorValePedagio,
                            Integradora = cargaConsultaValorPedagioIntegracao.TipoIntegracao.Descricao,
                            DataIntegracaoConsulta = cargaConsultaValorPedagioIntegracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        };

                    }

                    if (carga.VeiculosCarregamento != null && carga.VeiculosCarregamento.Count > 0)
                    {
                        cargaIntegracao.PlacasCarregamento = new List<string>();
                        foreach (var item in carga.VeiculosCarregamento)
                        {
                            cargaIntegracao.PlacasCarregamento.Add(item.Placa);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(urlBase))
                    {
                        string linkRastreio = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(cargaPedido.Pedido.CodigoRastreamento, urlBase);
                        string linkRastreioVisualizacaoMapa = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentVisualizacaoMonitoramento(cargaPedido.Carga.Codigo, urlBase);

                        cargaIntegracao.RastreamentoPedido = linkRastreio;
                        cargaIntegracao.RastreamentoMonitoramento = linkRastreioVisualizacaoMapa;
                    }

                    cargasIntegracao.Add(cargaIntegracao);
                }
            }
            return cargasIntegracao;
        }

        private void PreencherDadosValorFretePadrao(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentesCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ICMS serICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            cargaIntegracao.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            decimal valorFreteProprio = 0;
            decimal valorFrete = 0;
            if (carga.TabelaFrete?.IncluirICMSValorFreteNaCarga ?? false)
            {
                decimal valorICMS = carga.ValorICMS;

                if (valorICMS != 0)
                    valorFreteProprio = (cargaPedido.ValorFrete + valorICMS);
            }

            if (carga.TabelaFrete?.DescontarComponenteFreteLiquido ?? false)
                valorFreteProprio += BuscarValorTotalComponentes(carga);

            if (carga.TabelaFrete?.NaoSomarValorTotalPrestacao ?? false)
                valorFrete = BuscarValorTotalDescontarComponentesTabelaFrete(carga, carga.TabelaFrete.ComponenteFreteDestacar);

            cargaIntegracao.ValorFrete.FreteProprio = (carga.TabelaFrete?.IncluirICMSValorFreteNaCarga ?? false) ? valorFreteProprio : cargaPedido.ValorFrete;
            cargaIntegracao.ValorFrete.ValorTotalAReceber = (carga.TabelaFrete?.NaoSomarValorTotalPrestacao ?? false) ? (cargaPedido.ValorTotalAReceberComICMSeISS - valorFrete) : cargaPedido.ValorTotalAReceberComICMSeISS;
            cargaIntegracao.ValorFrete.ValorPrestacaoServico = (carga.TabelaFrete?.NaoSomarValorTotalPrestacao ?? false) ? (cargaPedido.ValorPrestacaoServico - valorFrete) : cargaPedido.ValorPrestacaoServico;
            cargaIntegracao.ValorFrete.ValorCalculadoPelaTabela = carga.ValorFreteTabelaFrete;

            cargaIntegracao.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            cargaIntegracao.ValorFrete.ICMS.Aliquota = cargaPedido.PercentualAliquota;
            cargaIntegracao.CFOP = cargaPedido.CFOP != null ? cargaPedido.CFOP.CodigoCFOP : 0;
            cargaIntegracao.ValorFrete.ICMS.CST = cargaPedido.CST;
            cargaIntegracao.ValorFrete.ICMS.IncluirICMSBC = cargaPedido.IncluirICMSBaseCalculo;
            cargaIntegracao.ValorFrete.ICMS.PercentualInclusaoBC = cargaPedido.PercentualIncluirBaseCalculo;
            cargaIntegracao.ValorFrete.ICMS.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
            cargaIntegracao.ValorFrete.ICMS.SimplesNacional = carga.Empresa != null ? carga.Empresa.OptanteSimplesNacional : false;
            cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS = cargaPedido.BaseCalculoICMS;
            cargaIntegracao.ValorFrete.ICMS.ValorICMS = cargaPedido.ValorICMS;
            cargaIntegracao.ValorFrete.ICMS.ValorCreditoPresumido = cargaPedido.ValorCreditoPresumido;
            cargaIntegracao.ValorFrete.ICMS.ObservacaoCTe = serICMS.ObterObservacaoRegraICMS(cargaPedido.ObservacaoRegraICMSCTe, cargaIntegracao.ValorFrete.ICMS.Aliquota, 0, cargaIntegracao.ValorFrete.ValorTotalAReceber, cargaIntegracao.ValorFrete.ICMS.ValorICMS, cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS, carga.Empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, cargaIntegracao.ValorFrete.ICMS.PercentualInclusaoBC, cargaPedido.Pedido.ProdutoPredominante, cargaIntegracao.ValorFrete.ICMS.ValorCreditoPresumido);

            cargaIntegracao.ValorFrete.ISS = new Dominio.ObjetosDeValor.Embarcador.ISS.ISS();
            cargaIntegracao.ValorFrete.ISS.Aliquota = cargaPedido.PercentualAliquotaISS;
            cargaIntegracao.ValorFrete.ISS.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;
            cargaIntegracao.ValorFrete.ISS.PercentualRetencao = cargaPedido.PercentualRetencaoISS;
            cargaIntegracao.ValorFrete.ISS.ValorBaseCalculoISS = cargaPedido.BaseCalculoISS;
            cargaIntegracao.ValorFrete.ISS.ValorISS = cargaPedido.ValorISS;
            cargaIntegracao.ValorFrete.ISS.ValorRetencaoISS = cargaPedido.ValorRetencaoISS;

            cargaIntegracao.ValorFrete.IBSCBS = new Dominio.ObjetosDeValor.Embarcador.IBSCBS.IBSCBS();
            cargaIntegracao.ValorFrete.IBSCBS.CST = cargaPedido.CSTIBSCBS;
            cargaIntegracao.ValorFrete.IBSCBS.ClassificacaoTributaria = cargaPedido.ClassificacaoTributariaIBSCBS;
            cargaIntegracao.ValorFrete.IBSCBS.BaseCalculo = cargaPedido.BaseCalculoIBSCBS;
            cargaIntegracao.ValorFrete.IBSCBS.AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadual;
            cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadual;
            cargaIntegracao.ValorFrete.IBSCBS.ValorIBSEstadual = cargaPedido.ValorIBSEstadual;
            cargaIntegracao.ValorFrete.IBSCBS.AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipal;
            cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipal;
            cargaIntegracao.ValorFrete.IBSCBS.ValorIBSMunicipal = cargaPedido.ValorIBSMunicipal;
            cargaIntegracao.ValorFrete.IBSCBS.AliquotaCBS = cargaPedido.AliquotaCBS;
            cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoCBS = cargaPedido.PercentualReducaoCBS;
            cargaIntegracao.ValorFrete.IBSCBS.ValorCBS = cargaPedido.ValorCBS;

            if (cargaPedido.CSTIBSCBS != null)
            {
                decimal valorTotalDocumento = cargaIntegracao.ValorFrete.ValorPrestacaoServico;
                Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota impostoIBSCSB = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork).ObterOutrasAliquotasIBSCBS(cargaPedido.OutrasAliquotas?.Codigo ?? 0);
                if ((impostoIBSCSB?.SomarImpostosDocumento ?? false) || (cargaPedido.OutrasAliquotas?.CalcularImpostoDocumento ?? false))
                    valorTotalDocumento = valorTotalDocumento + cargaPedido.ValorIBSMunicipal + cargaPedido.ValorIBSEstadual + cargaPedido.ValorCBS;

                cargaIntegracao.ValorFrete.ValorTotalDocumentoFiscal = Math.Round(valorTotalDocumento, 2, MidpointRounding.AwayFromZero);
            }

            cargaIntegracao.ValorFrete.ValorTotalDocumentoFiscal = cargaIntegracao.ValorFrete.ValorPrestacaoServico + cargaPedido.ValorIBSMunicipal + cargaPedido.ValorIBSEstadual + cargaPedido.ValorCBS;

            cargaIntegracao.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentes = (from obj in componentesCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in componentes)
            {
                cargaIntegracao.ValorFrete.ComponentesAdicionais.Add(serComponenteFrete.ConverterObjetoComponentePedido(cargaPedidoComponenteFrete));
            }
        }

        private void PreencherDadosValorFreteComCte(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTE = new Repositorio.ComponentePrestacaoCTE(unitOfWork);

            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarCTesPorCargaPedido(cargaPedido.Codigo);
            cargaIntegracao.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            if (ctes is null || ctes.Count == 0)
            {
                var msg = $"Não existem CT-es para o pedido {cargaPedido?.Codigo} | Carga {carga?.CodigoCargaEmbarcador}";
                Servicos.Log.TratarErro(msg);
                mensagem = msg;
                return;
            }

            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesPrestacaoCtes = repComponentePrestacaoCTE.BuscarPorCTe(ctes.Select(c => c.Codigo).Distinct());
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteBase = ctes.FirstOrDefault();

            cargaIntegracao.ValorFrete.FreteProprio = 0;
            cargaIntegracao.ValorFrete.ValorTotalAReceber = ctes.Sum(cte => cte.ValorAReceber);
            cargaIntegracao.ValorFrete.ValorPrestacaoServico = ctes.Sum(cte => cte.ValorPrestacaoServico);
            cargaIntegracao.ValorFrete.ValorCalculadoPelaTabela = carga.ValorFreteTabelaFrete;

            cargaIntegracao.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
            cargaIntegracao.ValorFrete.ICMS.Aliquota = cteBase.AliquotaICMS;

            cargaIntegracao.ValorFrete.ICMS.CST = cteBase?.CSTICMS.ObterDescricao() ?? string.Empty;
            cargaIntegracao.ValorFrete.ICMS.IncluirICMSBC = cteBase.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;

            cargaIntegracao.ValorFrete.ICMS.PercentualInclusaoBC = cteBase.PercentualICMSIncluirNoFrete;
            cargaIntegracao.ValorFrete.ICMS.PercentualReducaoBC = cteBase.PercentualReducaoBaseCalculoICMS;
            cargaIntegracao.ValorFrete.ICMS.SimplesNacional = cteBase.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim;
            cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS = cteBase.BaseCalculoICMS;
            cargaIntegracao.ValorFrete.ICMS.ValorICMS = ctes.Sum(cte => cte.ValorICMS);
            cargaIntegracao.ValorFrete.ICMS.ValorCreditoPresumido = ctes.Sum(cte => cte.ValorPresumido);

            cargaIntegracao.ValorFrete.ICMS.ObservacaoCTe = cteBase.ObservacoesGerais;

            cargaIntegracao.ValorFrete.ISS = new Dominio.ObjetosDeValor.Embarcador.ISS.ISS();
            cargaIntegracao.ValorFrete.ISS.Aliquota = cteBase.AliquotaISS;
            cargaIntegracao.CFOP = cteBase.CFOP != null ? cteBase.CFOP.CodigoCFOP : 0;
            cargaIntegracao.ValorFrete.ISS.IncluirISSBaseCalculo = cteBase.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
            cargaIntegracao.ValorFrete.ISS.PercentualRetencao = cteBase.PercentualISSRetido;
            cargaIntegracao.ValorFrete.ISS.ValorBaseCalculoISS = cteBase.BaseCalculoISS;
            cargaIntegracao.ValorFrete.ISS.ValorISS = ctes.Sum(cte => cte.ValorISS);
            cargaIntegracao.ValorFrete.ISS.ValorRetencaoISS = ctes.Sum(cte => cte.ValorISSRetido);

            cargaIntegracao.ValorFrete.IBSCBS = new Dominio.ObjetosDeValor.Embarcador.IBSCBS.IBSCBS();
            cargaIntegracao.ValorFrete.IBSCBS.CST = cteBase.CSTIBSCBS;
            cargaIntegracao.ValorFrete.IBSCBS.ClassificacaoTributaria = cteBase.ClassificacaoTributariaIBSCBS;
            cargaIntegracao.ValorFrete.IBSCBS.BaseCalculo = cteBase.BaseCalculoIBSCBS;
            cargaIntegracao.ValorFrete.IBSCBS.AliquotaIBSEstadual = cteBase.AliquotaIBSEstadual;
            cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoIBSEstadual = cteBase.PercentualReducaoIBSEstadual;
            cargaIntegracao.ValorFrete.IBSCBS.ValorIBSEstadual = cteBase.ValorIBSEstadual;
            cargaIntegracao.ValorFrete.IBSCBS.AliquotaIBSMunicipal = cteBase.AliquotaIBSMunicipal;
            cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoIBSMunicipal = cteBase.PercentualReducaoIBSMunicipal;
            cargaIntegracao.ValorFrete.IBSCBS.ValorIBSMunicipal = cteBase.ValorIBSMunicipal;
            cargaIntegracao.ValorFrete.IBSCBS.AliquotaCBS = cteBase.AliquotaCBS;
            cargaIntegracao.ValorFrete.IBSCBS.PercentualReducaoCBS = cteBase.PercentualReducaoCBS;
            cargaIntegracao.ValorFrete.IBSCBS.ValorCBS = ctes.Sum(cte => cte.ValorCBS);

            if (cteBase.ValorTotalDocumentoFiscal > 0)
                cargaIntegracao.ValorFrete.ValorTotalDocumentoFiscal = cteBase.ValorTotalDocumentoFiscal;
            else if (cteBase.CSTIBSCBS != null)
            {
                decimal valorTotalDocumento = cteBase.ValorPrestacaoServico;
                Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota impostoIBSCSB = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork).ObterOutrasAliquotasIBSCBS(cteBase?.OutrasAliquotas?.Codigo ?? 0);
                if ((impostoIBSCSB?.SomarImpostosDocumento ?? false) || (cteBase?.OutrasAliquotas?.CalcularImpostoDocumento ?? false))
                    valorTotalDocumento = valorTotalDocumento + cteBase.ValorIBSMunicipal + cteBase.ValorIBSEstadual + cargaIntegracao.ValorFrete.IBSCBS.ValorCBS;

                cargaIntegracao.ValorFrete.ValorTotalDocumentoFiscal = Math.Round(valorTotalDocumento, 2, MidpointRounding.AwayFromZero);
            }

            cargaIntegracao.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();

            foreach (Dominio.Entidades.ComponentePrestacaoCTE componentePrestacaoCTE in componentesPrestacaoCtes.Where(c => c.Nome != "FRETE VALOR" && c.Nome != "IMPOSTOS"))
            {
                cargaIntegracao.ValorFrete.ComponentesAdicionais.Add(serComponenteFrete.ConverterObjetoComponentePrestacaoCTe(componentePrestacaoCTE));
            }
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> ConverterPedidioEmCargaIntegracao(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaVeiculoContainer repCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);
            Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorioAprovacao = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Servicos.WebService.Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Endereco serEndereco = new Embarcador.Localidades.Endereco(unitOfWork);
            Servicos.WebService.Filial.Filial serWSFilial = new Filial.Filial(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.WebService.Carga.ModeloVeicularCarga serModeloVeicularCarga = new ModeloVeicularCarga(unitOfWork);
            Servicos.WebService.Empresa.Motorista serWSMotorista = new Empresa.Motorista(unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido serProdutosPedido = new ProdutosPedido(unitOfWork);
            Servicos.WebService.Carga.TipoCarga serWSTipoCarga = new Servicos.WebService.Carga.TipoCarga(unitOfWork);
            Servicos.WebService.Carga.TipoOperacao serWSTipoOperacao = new Servicos.WebService.Carga.TipoOperacao(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Frota.Veiculo(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);

            List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracao = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>();
            if (mensagem != null)
            {
                Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao()
                {
                    DataCriacaoCarga = pedido.DataCriacao.HasValue ? pedido.DataCriacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                    Distancia = pedido.Distancia,
                    DataInicioCarregamento = pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                    DataFinalCarregamento = pedido.DataTerminoCarregamento.HasValue ? pedido.DataTerminoCarregamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                    DataPrevisaoEntrega = pedido.PrevisaoEntrega.HasValue ? pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                    Remetente = serPessoa.ConverterObjetoPessoa(pedido.Remetente),
                    Destinatario = serPessoa.ConverterObjetoPessoa(pedido.Destinatario),
                    Observacao = pedido.Observacao
                };

                if (pedido.Recebedor != null)
                    cargaIntegracao.Recebedor = serPessoa.ConverterObjetoPessoa(pedido.Recebedor);

                cargaIntegracao.UsarOutroEnderecoDestino = pedido.UsarOutroEnderecoDestino;
                if (pedido != null && pedido.EnderecoRecebedor != null)
                    cargaIntegracao.Destino = serEndereco.ConverterObjetoEnderecoPedido(pedido.EnderecoRecebedor);
                else if (cargaIntegracao.Recebedor != null && cargaIntegracao.Recebedor.Endereco != null)
                    cargaIntegracao.Destino = cargaIntegracao.Recebedor.Endereco;
                else if (cargaIntegracao.UsarOutroEnderecoDestino && pedido.EnderecoDestino != null)
                    cargaIntegracao.Destino = serEndereco.ConverterObjetoEnderecoPedido(pedido.EnderecoDestino);
                else if (cargaIntegracao.Destinatario != null && cargaIntegracao.Destinatario.Endereco != null)
                    cargaIntegracao.Destino = cargaIntegracao.Destinatario.Endereco;

                if (pedido.CanalEntrega != null)
                {
                    cargaIntegracao.CanalEntrega = new Dominio.ObjetosDeValor.Embarcador.Pedido.CanalEntrega();
                    cargaIntegracao.CanalEntrega.CodigoIntegracao = pedido.CanalEntrega.CodigoIntegracao;
                    cargaIntegracao.CanalEntrega.Descricao = pedido.CanalEntrega.Descricao;
                }

                cargaIntegracao.Expedidor = serPessoa.ConverterObjetoPessoa(pedido.Expedidor);
                cargaIntegracao.UsarOutroEnderecoOrigem = pedido.UsarOutroEnderecoOrigem;
                if (pedido.EnderecoExpedidor != null)
                    cargaIntegracao.Origem = serEndereco.ConverterObjetoEnderecoPedido(pedido.EnderecoExpedidor);
                else if (cargaIntegracao.UsarOutroEnderecoOrigem)
                    cargaIntegracao.Origem = serEndereco.ConverterObjetoEnderecoPedido(pedido.EnderecoOrigem);
                else if (cargaIntegracao.Expedidor != null)
                    cargaIntegracao.Origem = cargaIntegracao.Expedidor.Endereco;
                else
                    cargaIntegracao.Origem = cargaIntegracao.Remetente.Endereco;

                cargaIntegracao.Filial = serWSFilial.ConverterObjetoFilial(pedido.Filial);
                cargaIntegracao.ModeloVeicular = serModeloVeicularCarga.ConverterObjetoModeloVeicular(pedido.ModeloVeicularCarga);

                cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

                foreach (Dominio.Entidades.Usuario motorista in pedido.Motoristas)
                    cargaIntegracao.Motoristas.Add(serWSMotorista.ConverterObjetoMotorista(motorista));

                cargaIntegracao.ProtocoloCarga = 0;
                cargaIntegracao.ProtocoloPedido = pedido.Protocolo;
                cargaIntegracao.NumeroPaletes = pedido.NumeroPaletes;
                cargaIntegracao.NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador;
                cargaIntegracao.TipoPedido = pedido.TipoPedido;
                cargaIntegracao.PesoBruto = pedido.PesoTotal;
                cargaIntegracao.PesoLiquido = pedido.PesoLiquidoTotal;
                cargaIntegracao.CubagemTotal = pedido.CubagemTotal;
                cargaIntegracao.PesoTotalPaletes = pedido.PesoTotalPaletes;
                cargaIntegracao.CodigoIntegracaoRota = pedido.RotaFrete != null ? pedido.RotaFrete.CodigoIntegracao : string.Empty;
                cargaIntegracao.DescricaoRota = pedido.RotaFrete != null ? pedido.RotaFrete.Descricao : string.Empty;
                cargaIntegracao.ETA = pedido.DataETA.HasValue ? pedido.DataETA.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                cargaIntegracao.ValorTotalPedido = pedido.ValorTotalNotasFiscais;
                cargaIntegracao.QuantidadeVolumes = pedido.QtVolumes;

                cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtoPedido in pedido.Produtos)
                    cargaIntegracao.Produtos.Add(serProdutosPedido.ConveterObjetoProduto(produtoPedido));

                cargaIntegracao.TipoCargaEmbarcador = serWSTipoCarga.ConverterObjetoTipoCarga(pedido.TipoDeCarga);
                cargaIntegracao.TipoOperacao = serWSTipoOperacao.ConverterObjetoTipoOperacao(pedido.TipoOperacao);
                cargaIntegracao.TipoPagamento = pedido.TipoPagamento;
                cargaIntegracao.TipoTomador = pedido.TipoTomador;

                if (cargaIntegracao.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                    cargaIntegracao.Tomador = cargaIntegracao.Remetente;
                else if (cargaIntegracao.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                    cargaIntegracao.Tomador = cargaIntegracao.Destinatario;
                else
                    cargaIntegracao.Tomador = serPessoa.ConverterObjetoPessoa(pedido.Tomador);

                cargaIntegracao.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(pedido.Empresa);
                cargaIntegracao.ValorTotalPaletes = pedido.ValorTotalPaletes;

                bool retornarModeloVeiculo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().RetornarModeloVeiculo.Value; //Criado com urgência pois minerva solicitou urgência para retornar o modelo do veiculo

                if (pedido.VeiculoTracao != null)
                    cargaIntegracao.Veiculo = serWSVeiculo.ConverterObjetoConjuntoVeiculos(pedido.VeiculoTracao, pedido.Veiculos.ToList(), unitOfWork, 1, null, retornarModeloVeiculo);

                //cargaIntegracao.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();
                //cargaIntegracao.ValorFrete.FreteProprio = cargaPedido.ValorFrete;
                //cargaIntegracao.ValorFrete.ValorTotalAReceber = cargaPedido.ValorTotalAReceberComICMSeISS;
                //cargaIntegracao.ValorFrete.ValorPrestacaoServico = cargaPedido.ValorPrestacaoServico;

                //cargaIntegracao.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
                //cargaIntegracao.ValorFrete.ICMS.Aliquota = cargaPedido.PercentualAliquota;
                //cargaIntegracao.CFOP = cargaPedido.CFOP != null ? cargaPedido.CFOP.CodigoCFOP : 0;
                //cargaIntegracao.ValorFrete.ICMS.CST = cargaPedido.CST;
                //cargaIntegracao.ValorFrete.ICMS.IncluirICMSBC = cargaPedido.IncluirICMSBaseCalculo;

                //cargaIntegracao.ValorFrete.ICMS.PercentualInclusaoBC = cargaPedido.PercentualIncluirBaseCalculo;
                //cargaIntegracao.ValorFrete.ICMS.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
                //cargaIntegracao.ValorFrete.ICMS.SimplesNacional = carga.Empresa != null ? carga.Empresa.OptanteSimplesNacional : false;
                //cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS = cargaPedido.BaseCalculoICMS;
                //cargaIntegracao.ValorFrete.ICMS.ValorICMS = cargaPedido.ValorICMS;
                //cargaIntegracao.ValorFrete.ICMS.ValorCreditoPresumido = cargaPedido.ValorCreditoPresumido;

                //cargaIntegracao.ValorFrete.ICMS.ObservacaoCTe = serICMS.ObterObservacaoRegraICMS(cargaPedido.ObservacaoRegraICMSCTe, cargaIntegracao.ValorFrete.ICMS.Aliquota, 0, cargaIntegracao.ValorFrete.ValorTotalAReceber, cargaIntegracao.ValorFrete.ICMS.ValorICMS, cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS, carga.Empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, cargaIntegracao.ValorFrete.ICMS.PercentualInclusaoBC, cargaPedido.Pedido.ProdutoPredominante, cargaIntegracao.ValorFrete.ICMS.ValorCreditoPresumido);

                //cargaIntegracao.ValorFrete.ISS = new Dominio.ObjetosDeValor.Embarcador.ISS.ISS();
                //cargaIntegracao.ValorFrete.ISS.Aliquota = cargaPedido.PercentualAliquotaISS;
                //cargaIntegracao.ValorFrete.ISS.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;
                //cargaIntegracao.ValorFrete.ISS.PercentualRetencao = cargaPedido.PercentualRetencaoISS;
                //cargaIntegracao.ValorFrete.ISS.ValorBaseCalculoISS = cargaPedido.BaseCalculoISS;
                //cargaIntegracao.ValorFrete.ISS.ValorISS = cargaPedido.ValorISS;
                //cargaIntegracao.ValorFrete.ISS.ValorRetencaoISS = cargaPedido.ValorRetencaoISS;

                //cargaIntegracao.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
                //List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentes = (from obj in componentesCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();
                //foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in componentes)
                //{
                //    cargaIntegracao.ValorFrete.ComponentesAdicionais.Add(serComponenteFrete.ConverterObjetoComponentePedido(cargaPedidoComponenteFrete));
                //}

                //Dados do multimodal
                cargaIntegracao.NumeroBooking = pedido.NumeroBooking;
                cargaIntegracao.Container = ConverterObjetoContainer(pedido.Container);
                cargaIntegracao.Viagem = ConverterObjetoViagem(pedido.PedidoViagemNavio);
                cargaIntegracao.Embarcador = serPessoa.ConverterObjetoPessoa(pedido.Remetente);
                cargaIntegracao.ViagemLongoCurso = ConverterObjetoViagem(pedido.PedidoViagemNavioLongoCurso);
                cargaIntegracao.PortoOrigem = ConverterObjetoPorto(pedido.Porto);
                cargaIntegracao.PortoDestino = ConverterObjetoPorto(pedido.PortoDestino);
                cargaIntegracao.TerminalPortoOrigem = ConverterObjetoTerminalPorto(pedido.TerminalOrigem);
                cargaIntegracao.TerminalPortoDestino = ConverterObjetoTerminalPorto(pedido.TerminalDestino);
                cargaIntegracao.TipoContainerReserva = ConverterObjetoTipoContainer(pedido.ContainerTipoReserva);
                cargaIntegracao.ContemCargaPerigosa = pedido.PossuiCargaPerigosa;
                cargaIntegracao.ContemCargaRefrigerada = pedido.ContemCargaRefrigerada;
                cargaIntegracao.TemperaturaObservacao = pedido.ObservacaoCTe;
                cargaIntegracao.Temperatura = pedido.Temperatura;
                cargaIntegracao.ValidarNumeroContainer = pedido.ValidarDigitoVerificadorContainer;
                cargaIntegracao.PropostaComercial = ConverterObjetoPropostaComercial(pedido);

                if (configuracaoTMS.UtilizaEmissaoMultimodal)
                    cargaIntegracao.Transbordo = ConverterObjetoTransbordo(pedido.PedidosTransbordo != null ? pedido.PedidosTransbordo.ToList() : null);

                int.TryParse(pedido.CodigoOS, out int codigoOS);
                cargaIntegracao.CodigoOrdemServico = codigoOS;
                cargaIntegracao.NumeroOrdemServico = pedido.NumeroOS;
                cargaIntegracao.Embarque = pedido.Embarque;
                cargaIntegracao.MasterBL = pedido.MasterBL;
                cargaIntegracao.NumeroDIEmbarque = pedido.NumeroDI;
                cargaIntegracao.ProvedorOS = serPessoa.ConverterObjetoPessoa(pedido.ProvedorOS);
                cargaIntegracao.TipoServicoCarga = pedido.TipoServicoCarga;
                int.TryParse(pedido.TaraContainer, out int taraContainer);
                cargaIntegracao.TaraContainer = taraContainer;
                if (!string.IsNullOrWhiteSpace(pedido.LacreContainerUm))
                    cargaIntegracao.NumeroLacre1 = pedido.LacreContainerUm;
                if (!string.IsNullOrWhiteSpace(pedido.LacreContainerDois))
                    cargaIntegracao.NumeroLacre2 = pedido.LacreContainerDois;
                if (!string.IsNullOrWhiteSpace(pedido.LacreContainerTres))
                    cargaIntegracao.NumeroLacre3 = pedido.LacreContainerTres;
                int.TryParse(pedido.CodigoBooking, out int codigoBooking);
                cargaIntegracao.CodigoBooking = codigoBooking;
                cargaIntegracao.NumeroBL = pedido.NumeroBL;
                cargaIntegracao.NecessitaAverbacao = pedido.NecessitaAverbacaoAutomatica;
                cargaIntegracao.CargaRefrigeradaPrecisaEnergia = pedido.NecessitaEnergiaContainerRefrigerado;
                cargaIntegracao.QuantidadeTipoContainerReserva = pedido.QuantidadeTipoContainerReserva;
                cargaIntegracao.IMOClasse = pedido.IMOClasse;
                cargaIntegracao.IMOSequencia = pedido.IMOSequencia;
                cargaIntegracao.IMOUnidade = pedido.IMOUnidade;
                cargaIntegracao.FormaAverbacaoCTE = pedido.FormaAverbacaoCTE;
                cargaIntegracao.PercentualADValorem = pedido.ValorAdValorem;
                cargaIntegracao.TipoDocumentoAverbacao = pedido.TipoDocumentoAverbacao;
                cargaIntegracao.ObservacaoProposta = pedido.ObservacaoCTe;
                cargaIntegracao.TipoPropostaFeeder = pedido.TipoPropostaFeeder;
                cargaIntegracao.DescricaoTipoPropostaFeeder = pedido.DescricaoTipoPropostaFeeder;
                cargaIntegracao.DescricaoCarrierNavioViagem = pedido.DescricaoCarrierNavioViagem;
                cargaIntegracao.RealizarCobrancaTaxaDocumentacao = pedido.RealizarCobrancaTaxaDocumentacao;
                cargaIntegracao.QuantidadeConhecimentosTaxaDocumentacao = pedido.QuantidadeConhecimentosTaxaDocumentacao;
                cargaIntegracao.ValorTaxaDocumento = pedido.ValorTaxaDocumento;
                cargaIntegracao.ContainerADefinir = pedido.ContainerADefinir;
                cargaIntegracao.ValorCusteioSVM = pedido.ValorCusteioSVM;
                cargaIntegracao.QuantidadeContainerBooking = pedido.QuantidadeContainerBooking;
                cargaIntegracao.EmpresaResponsavel = ConverterObjetoEmpresaResponsavel(pedido.PedidoEmpresaResponsavel);
                cargaIntegracao.CentroCusto = ConverterObjetoCentroCustol(pedido.PedidoCentroCusto);
                cargaIntegracao.PedidoDeSVMTerceiro = pedido.PedidoDeSVMTerceiro;
                cargaIntegracao.NumeroCE = pedido.NumeroCEFeeder;
                cargaIntegracao.ValorTaxaFeeder = pedido.ValorTaxaFeeder;
                cargaIntegracao.TipoCalculoCargaFracionada = pedido.TipoCalculoCargaFracionada;
                cargaIntegracao.SituacaoCarga = SituacaoCarga.Todas;
                cargaIntegracao.ExecaoCab = null;
                cargaIntegracao.DataUltimaLiberacao = pedido.DataUltimaLiberacao.HasValue ? pedido.DataUltimaLiberacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;

                Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repPedidoAdicional.BuscarPorPedido(pedido.Codigo);
                if (pedidoAdicional != null)
                    cargaIntegracao.ExecaoCab = pedidoAdicional.ExecaoCab;

                cargasIntegracao.Add(cargaIntegracao);
            }
            return cargasIntegracao;
        }

        //private List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> BuscarCargasPedidos(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
        //    Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
        //    Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
        //    Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
        //    Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(unitOfWork);
        //    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
        //    Repositorio.Embarcador.Cargas.CargaVeiculoContainer repCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(unitOfWork);
        //    Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorioAprovacao = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(unitOfWork);

        //    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

        //    Servicos.WebService.Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(StringConexao);
        //    Servicos.Embarcador.Localidades.Endereco serEndereco = new Embarcador.Localidades.Endereco(StringConexao);
        //    Servicos.WebService.Filial.Filial serWSFilial = new Filial.Filial(StringConexao);
        //    Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(StringConexao);
        //    Servicos.WebService.Carga.ModeloVeicularCarga serModeloVeicularCarga = new ModeloVeicularCarga(StringConexao);
        //    Servicos.WebService.Empresa.Motorista serWSMotorista = new Empresa.Motorista(StringConexao);
        //    Servicos.WebService.Carga.ProdutosPedido serProdutosPedido = new ProdutosPedido(StringConexao);
        //    Servicos.WebService.Carga.TipoCarga serWSTipoCarga = new Servicos.WebService.Carga.TipoCarga(StringConexao);
        //    Servicos.WebService.Carga.TipoOperacao serWSTipoOperacao = new Servicos.WebService.Carga.TipoOperacao(StringConexao);
        //    Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(StringConexao);
        //    Servicos.WebService.Frota.Veiculo serWSVeiculo = new Frota.Veiculo(unitOfWork);
        //    Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(StringConexao);
        //    Servicos.Embarcador.Carga.ICMS serICMS = new Servicos.Embarcador.Carga.ICMS(StringConexao);

        //    List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracao = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>();
        //    if (mensagem != null)
        //    {
        //        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
        //        {
        //            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();

        //            if (configuracaoTMS.RetornarDataCarregamentoDaCargaNaConsulta)
        //            {
        //                if (cargaPedido.Carga.Filial?.Codigo == cargaPedido.CargaOrigem.Filial?.Codigo)
        //                {
        //                    cargaIntegracao.DataInicioCarregamento = cargaPedido.Carga.DataCarregamentoCarga.HasValue ? cargaPedido.Carga.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
        //                    cargaIntegracao.DataFinalCarregamento = cargaPedido.Carga.DataPrevisaoTerminoCarga.HasValue ? cargaPedido.Carga.DataPrevisaoTerminoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
        //                }
        //                else
        //                {
        //                    cargaIntegracao.DataInicioCarregamento = cargaPedido.CargaOrigem.DataCarregamentoCarga.HasValue ? cargaPedido.CargaOrigem.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
        //                    cargaIntegracao.DataFinalCarregamento = cargaPedido.CargaOrigem.DataPrevisaoTerminoCarga.HasValue ? cargaPedido.CargaOrigem.DataPrevisaoTerminoCarga.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
        //                }
        //            }
        //            else
        //            {
        //                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(cargaPedido.Carga.Codigo);
        //                if (cargaJanelaCarregamento != null)
        //                {
        //                    cargaIntegracao.DataInicioCarregamento = cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm:ss");
        //                    cargaIntegracao.DataFinalCarregamento = cargaJanelaCarregamento.TerminoCarregamento.ToString("dd/MM/yyyy HH:mm:ss");
        //                }
        //                else
        //                {
        //                    cargaIntegracao.DataFinalCarregamento = cargaPedido.Carga.DataFinalPrevisaoCarregamento.HasValue ? cargaPedido.Carga.DataFinalPrevisaoCarregamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
        //                    cargaIntegracao.DataInicioCarregamento = cargaPedido.Carga.DataInicialPrevisaoCarregamento.HasValue ? cargaPedido.Carga.DataInicialPrevisaoCarregamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
        //                }
        //            }
        //            cargaIntegracao.DataCriacaoCarga = cargaPedido.Carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm:ss");
        //            cargaIntegracao.DataPrevisaoEntrega = cargaPedido.Carga.DataPrevisaoTerminoCarga.HasValue ? cargaPedido.Carga.DataPrevisaoTerminoCarga.Value.ToString("dd/MM/yyyy") : "";
        //            cargaIntegracao.HoraPrevisaoEntrega = cargaPedido.Carga.DataPrevisaoTerminoCarga.HasValue ? cargaPedido.Carga.DataPrevisaoTerminoCarga.Value.ToString("HH:mm") : "";
        //            cargaIntegracao.Destinatario = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Destinatario);
        //            if (cargaPedido.Pedido.Recebedor != null)
        //                cargaIntegracao.Recebedor = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Recebedor);
        //            cargaIntegracao.UsarOutroEnderecoDestino = cargaPedido.Pedido.UsarOutroEnderecoDestino;

        //            cargaIntegracao.OperadorCargaNome = cargaPedido.Carga.Operador?.Nome ?? "";
        //            cargaIntegracao.OperadorCargaCPF = cargaPedido.Carga.Operador?.CPF_Formatado ?? "";
        //            cargaIntegracao.OperadorCargaEmail = cargaPedido.Carga.Operador?.Email ?? "";

        //            if (cargaPedido.Pedido != null && cargaPedido.Pedido.EnderecoRecebedor != null)
        //                cargaIntegracao.Destino = serEndereco.ConverterObjetoEnderecoPedido(cargaPedido.Pedido.EnderecoRecebedor);
        //            else if (cargaIntegracao.Recebedor != null && cargaIntegracao.Recebedor.Endereco != null)
        //                cargaIntegracao.Destino = cargaIntegracao.Recebedor.Endereco;
        //            else if (cargaIntegracao.UsarOutroEnderecoDestino && cargaPedido.Pedido != null && cargaPedido.Pedido.EnderecoDestino != null)
        //                cargaIntegracao.Destino = serEndereco.ConverterObjetoEnderecoPedido(cargaPedido.Pedido.EnderecoDestino);
        //            else if (cargaIntegracao.Destinatario != null && cargaIntegracao.Destinatario.Endereco != null)
        //                cargaIntegracao.Destino = cargaIntegracao.Destinatario.Endereco;

        //            cargaIntegracao.Expedidor = serPessoa.ConverterObjetoPessoa(cargaPedido.Expedidor);

        //            cargaIntegracao.Remetente = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Remetente);
        //            cargaIntegracao.UsarOutroEnderecoOrigem = cargaPedido.Pedido.UsarOutroEnderecoOrigem;
        //            if (cargaPedido.Pedido != null && cargaPedido.Pedido.EnderecoExpedidor != null)
        //                cargaIntegracao.Origem = serEndereco.ConverterObjetoEnderecoPedido(cargaPedido.Pedido.EnderecoExpedidor);
        //            else if (cargaIntegracao.UsarOutroEnderecoOrigem)
        //                cargaIntegracao.Origem = serEndereco.ConverterObjetoEnderecoPedido(cargaPedido.Pedido.EnderecoOrigem);
        //            else if (cargaIntegracao.Expedidor != null)
        //                cargaIntegracao.Origem = cargaIntegracao.Expedidor.Endereco;
        //            else
        //                cargaIntegracao.Origem = cargaIntegracao.Remetente.Endereco;

        //            cargaIntegracao.Filial = serWSFilial.ConverterObjetoFilial(cargaPedido.Pedido.Filial);
        //            cargaIntegracao.Fronteira = serLocalidade.ConverterObjetoFronteira(cargaPedido.Carga.Fronteira);

        //            cargaIntegracao.ModeloVeicular = serModeloVeicularCarga.ConverterObjetoModeloVeicular(cargaPedido.CargaOrigem.ModeloVeicularOrigem != null ? cargaPedido.CargaOrigem.ModeloVeicularOrigem : cargaPedido.Carga.ModeloVeicularCarga);

        //            cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

        //            foreach (Dominio.Entidades.Usuario motorista in cargaPedido.Carga.Motoristas)
        //            {
        //                cargaIntegracao.Motoristas.Add(serWSMotorista.ConverterObjetoMotorista(motorista));
        //            }



        //            if (cargaPedido.Carga.Carregamento != null)
        //            {
        //                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocos = repBlocoCarregamento.BuscarPorCarregamentoEPedido(cargaPedido.Carga.Carregamento.Codigo, cargaPedido.Pedido.Codigo);

        //                if (blocos.Count > 0)
        //                {
        //                    cargaIntegracao.BlocosCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamento>();
        //                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento bloco in blocos)
        //                    {
        //                        Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamento blocoCarregamento = new Dominio.ObjetosDeValor.Embarcador.Carga.BlocoCarregamento();
        //                        blocoCarregamento.Descricao = bloco.Bloco;
        //                        blocoCarregamento.CodigoIntegracao = bloco.Bloco;
        //                        cargaIntegracao.BlocosCarregamento.Add(blocoCarregamento);
        //                    }
        //                }
        //            }

        //            cargaIntegracao.ProtocoloCarga = configuracaoTMS.RetornarCargasAgrupadasCarregamento && cargaPedido.Carga.CargaAgrupada ? cargaPedido.CargaOrigem.Protocolo : cargaPedido.Carga.Protocolo;
        //            cargaIntegracao.ProtocoloPedido = cargaPedido.Pedido.Protocolo;
        //            cargaIntegracao.NumeroCarga = cargaPedido.Carga.CodigoCargaEmbarcador;
        //            cargaIntegracao.NumeroPaletes = cargaPedido.Pedido.NumeroPaletes;
        //            cargaIntegracao.NumeroPedidoEmbarcador = cargaPedido.Pedido.NumeroPedidoEmbarcador;
        //            cargaIntegracao.TipoPedido = cargaPedido.Pedido.TipoPedido;
        //            cargaIntegracao.PesoBruto = cargaPedido.Pedido.PesoTotal;
        //            cargaIntegracao.PesoLiquido = cargaPedido.Pedido.PesoLiquidoTotal;
        //            cargaIntegracao.CubagemTotal = cargaPedido.Pedido.CubagemTotal;
        //            cargaIntegracao.PesoTotalPaletes = cargaPedido.Pedido.PesoTotalPaletes;
        //            cargaIntegracao.CodigoIntegracaoRota = cargaPedido.Carga.Rota?.CodigoIntegracao ?? string.Empty;
        //            cargaIntegracao.DescricaoRota = cargaPedido.Carga.Rota?.Descricao ?? string.Empty;
        //            cargaIntegracao.OrdemEntrega = cargaPedido.OrdemEntrega;
        //            cargaIntegracao.OrdemColeta = cargaPedido.OrdemColeta;
        //            cargaIntegracao.NumeroPager = cargaPedido.Carga.NumeroPager;
        //            cargaIntegracao.ETA = cargaPedido.Pedido.DataETA.HasValue ? cargaPedido.Pedido.DataETA.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;

        //            if (cargaPedido.Carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador)
        //            {
        //                cargaIntegracao.FreteNegociado = new Dominio.ObjetosDeValor.Embarcador.Carga.FreteNegociado();
        //                cargaIntegracao.FreteNegociado.CPFOperador = cargaPedido.Carga.Operador?.CPF;
        //                cargaIntegracao.FreteNegociado.NomeOperador = cargaPedido.Carga.Operador?.Nome;

        //                List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga> listaAutorizacao = repositorioAprovacao.ConsultaPorCarga(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada);
        //                if (listaAutorizacao != null && listaAutorizacao.Count() >= 0)
        //                {
        //                    cargaIntegracao.FreteNegociado.Aprovadores = new List<Dominio.ObjetosDeValor.Embarcador.Carga.FreteNegociadoAprovador>();
        //                    foreach (Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga aprovacao in listaAutorizacao)
        //                    {
        //                        if (aprovacao.Usuario != null)
        //                        {
        //                            Dominio.ObjetosDeValor.Embarcador.Carga.FreteNegociadoAprovador aprovador = new Dominio.ObjetosDeValor.Embarcador.Carga.FreteNegociadoAprovador();
        //                            aprovador.CPF = aprovacao.Usuario.CPF;
        //                            aprovador.Nome = aprovacao.Usuario.Nome;
        //                            cargaIntegracao.FreteNegociado.Aprovadores.Add(aprovador);
        //                        }
        //                    }
        //                }
        //            }

        //            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
        //            cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
        //            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
        //            {
        //                cargaIntegracao.Produtos.Add(serProdutosPedido.ConveterObjetoProduto(cargaPedidoProduto));
        //            }

        //            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga = cargaPedido.Carga.SituacaoCarga;
        //            if (situacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || situacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
        //            {
        //                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(cargaPedido.Carga.Codigo);
        //                if (cargaCancelamento != null)
        //                    situacaoCarga = cargaCancelamento.SituacaoCargaNoCancelamento;
        //            }

        //            if (situacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
        //                && situacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete
        //                && (!cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete || situacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe))
        //                cargaIntegracao.ValorFreteCalculado = true;

        //            cargaIntegracao.Recebedor = serPessoa.ConverterObjetoPessoa(cargaPedido.Recebedor);
        //            cargaIntegracao.TipoCargaEmbarcador = serWSTipoCarga.ConverterObjetoTipoCarga(cargaPedido.Carga.TipoDeCarga);
        //            cargaIntegracao.TipoOperacao = serWSTipoOperacao.ConverterObjetoTipoOperacao(cargaPedido.Carga.TipoOperacao);
        //            cargaIntegracao.TipoPagamento = cargaPedido.Pedido.TipoPagamento;
        //            cargaIntegracao.TipoTomador = cargaPedido.TipoTomador;

        //            if (cargaIntegracao.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
        //                cargaIntegracao.Tomador = cargaIntegracao.Remetente;
        //            else if (cargaIntegracao.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
        //                cargaIntegracao.Tomador = cargaIntegracao.Destinatario;
        //            else
        //                cargaIntegracao.Tomador = serPessoa.ConverterObjetoPessoa(cargaPedido.Tomador);

        //            cargaIntegracao.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(cargaPedido.Carga.Empresa);
        //            cargaIntegracao.ValorTotalPaletes = cargaPedido.Pedido.ValorTotalPaletes;


        //            if (cargaPedido.NumeroReboque == Dominio.ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque.SemReboque)
        //            {
        //                int codigoVeiculoReboque = cargaPedido.Carga.VeiculosVinculados.Count > 0 ? cargaPedido.Carga.VeiculosVinculados.ToList()[0].Codigo : 0;

        //                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer = repCargaVeiculoContainer.BuscarPorCargaEVeiculo(cargaPedido.Carga.Codigo, codigoVeiculoReboque);

        //                cargaIntegracao.Veiculo = serWSVeiculo.ConverterObjetoConjuntoVeiculos(cargaPedido.Carga.Veiculo, cargaPedido.Carga.VeiculosVinculados.ToList(), 1, cargaVeiculoContainer);
        //            }
        //            else
        //            {
        //                if (cargaPedido.NumeroReboque == Dominio.ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque.ReboqueUm && cargaPedido.Carga.VeiculosVinculados.Count > 0)
        //                {
        //                    List<Dominio.Entidades.Veiculo> listaReboque = new List<Dominio.Entidades.Veiculo>();
        //                    listaReboque.Add(cargaPedido.Carga.VeiculosVinculados.ToList()[0]);

        //                    Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer = repCargaVeiculoContainer.BuscarPorCargaEVeiculo(cargaPedido.Carga.Codigo, cargaPedido.Carga.VeiculosVinculados.ToList()[0].Codigo);

        //                    cargaIntegracao.Veiculo = serWSVeiculo.ConverterObjetoConjuntoVeiculos(cargaPedido.Carga.Veiculo, listaReboque, 1, cargaVeiculoContainer);
        //                }
        //                else if (cargaPedido.NumeroReboque == Dominio.ObjetosDeValor.Embarcador.Enumeradores.NumeroReboque.ReboqueDois && cargaPedido.Carga.VeiculosVinculados.Count > 1)
        //                {
        //                    List<Dominio.Entidades.Veiculo> listaReboque = new List<Dominio.Entidades.Veiculo>();
        //                    listaReboque.Add(cargaPedido.Carga.VeiculosVinculados.ToList()[1]);

        //                    Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer = repCargaVeiculoContainer.BuscarPorCargaEVeiculo(cargaPedido.Carga.Codigo, cargaPedido.Carga.VeiculosVinculados.ToList()[1].Codigo);

        //                    cargaIntegracao.Veiculo = serWSVeiculo.ConverterObjetoConjuntoVeiculos(cargaPedido.Carga.Veiculo, listaReboque, 2, cargaVeiculoContainer);
        //                }
        //            }

        //            if (cargaIntegracao.Veiculo != null)
        //            {
        //                if (cargaIntegracao.Veiculo.Reboques != null && cargaIntegracao.Veiculo.Reboques.Count > 0)
        //                    cargaIntegracao.VeiculoDaNota = cargaIntegracao.Veiculo.Reboques.LastOrDefault();
        //                else
        //                    cargaIntegracao.VeiculoDaNota = cargaIntegracao.Veiculo;
        //            }

        //            cargaIntegracao.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();
        //            cargaIntegracao.ValorFrete.FreteProprio = cargaPedido.ValorFrete;
        //            cargaIntegracao.ValorFrete.ValorTotalAReceber = cargaPedido.ValorTotalAReceberComICMSeISS;
        //            cargaIntegracao.ValorFrete.ValorPrestacaoServico = cargaPedido.ValorPrestacaoServico;


        //            cargaIntegracao.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
        //            cargaIntegracao.ValorFrete.ICMS.Aliquota = cargaPedido.PercentualAliquota;
        //            cargaIntegracao.CFOP = cargaPedido.CFOP != null ? cargaPedido.CFOP.CodigoCFOP : 0;
        //            cargaIntegracao.ValorFrete.ICMS.CST = cargaPedido.CST;
        //            cargaIntegracao.ValorFrete.ICMS.IncluirICMSBC = cargaPedido.IncluirICMSBaseCalculo;

        //            cargaIntegracao.ValorFrete.ICMS.PercentualInclusaoBC = cargaPedido.PercentualIncluirBaseCalculo;
        //            cargaIntegracao.ValorFrete.ICMS.PercentualReducaoBC = cargaPedido.PercentualReducaoBC;
        //            cargaIntegracao.ValorFrete.ICMS.SimplesNacional = cargaPedido.Carga.Empresa != null ? cargaPedido.Carga.Empresa.OptanteSimplesNacional : false;
        //            cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS = cargaPedido.BaseCalculoICMS;
        //            cargaIntegracao.ValorFrete.ICMS.ValorICMS = cargaPedido.ValorICMS;

        //            cargaIntegracao.ValorFrete.ICMS.ObservacaoCTe = serICMS.ObterObservacaoRegraICMS(cargaPedido.ObservacaoRegraICMSCTe, cargaIntegracao.ValorFrete.ICMS.Aliquota, 0, cargaIntegracao.ValorFrete.ValorTotalAReceber, cargaIntegracao.ValorFrete.ICMS.ValorICMS, cargaIntegracao.ValorFrete.ICMS.ValorBaseCalculoICMS, cargaPedido.Carga.Empresa, cargaPedido.Pedido.Remetente, cargaPedido.Pedido.Destinatario, cargaPedido.ObterTomador(), cargaPedido.Origem, cargaPedido.Destino, cargaIntegracao.ValorFrete.ICMS.PercentualInclusaoBC, cargaPedido.Pedido.ProdutoPredominante);

        //            cargaIntegracao.ValorFrete.ISS = new Dominio.ObjetosDeValor.Embarcador.ISS.ISS();
        //            cargaIntegracao.ValorFrete.ISS.Aliquota = cargaPedido.PercentualAliquotaISS;
        //            cargaIntegracao.ValorFrete.ISS.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;
        //            cargaIntegracao.ValorFrete.ISS.PercentualRetencao = cargaPedido.PercentualRetencaoISS;
        //            cargaIntegracao.ValorFrete.ISS.ValorBaseCalculoISS = cargaPedido.BaseCalculoISS;
        //            cargaIntegracao.ValorFrete.ISS.ValorISS = cargaPedido.ValorISS;
        //            cargaIntegracao.ValorFrete.ISS.ValorRetencaoISS = cargaPedido.ValorRetencaoISS;

        //            cargaIntegracao.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
        //            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentes = repCargaPedidoComponentesFrete.BuscarPorCargaPedidoSemComponenteFreteValor(cargaPedido.Codigo, false, cargaPedido.ModeloDocumentoFiscal, false);
        //            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in componentes)
        //            {
        //                cargaIntegracao.ValorFrete.ComponentesAdicionais.Add(serComponenteFrete.ConverterObjetoComponentePedido(cargaPedidoComponenteFrete));
        //            }

        //            //Dados do multimodal
        //            cargaIntegracao.NumeroBooking = cargaPedido.Pedido.NumeroBooking;
        //            cargaIntegracao.Container = ConverterObjetoContainer(cargaPedido.Pedido.Container);
        //            cargaIntegracao.Viagem = ConverterObjetoViagem(cargaPedido.Pedido.PedidoViagemNavio);
        //            cargaIntegracao.Embarcador = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Remetente);
        //            cargaIntegracao.ViagemLongoCurso = ConverterObjetoViagem(cargaPedido.Pedido.PedidoViagemNavioLongoCurso);
        //            cargaIntegracao.PortoOrigem = ConverterObjetoPorto(cargaPedido.Pedido.Porto);
        //            cargaIntegracao.PortoDestino = ConverterObjetoPorto(cargaPedido.Pedido.PortoDestino);
        //            cargaIntegracao.TerminalPortoOrigem = ConverterObjetoTerminalPorto(cargaPedido.Pedido.TerminalOrigem);
        //            cargaIntegracao.TerminalPortoDestino = ConverterObjetoTerminalPorto(cargaPedido.Pedido.TerminalDestino);
        //            cargaIntegracao.TipoContainerReserva = ConverterObjetoTipoContainer(cargaPedido.Pedido.ContainerTipoReserva);
        //            cargaIntegracao.ContemCargaPerigosa = cargaPedido.Pedido.PossuiCargaPerigosa;
        //            cargaIntegracao.ContemCargaRefrigerada = cargaPedido.Pedido.ContemCargaRefrigerada;
        //            cargaIntegracao.TemperaturaObservacao = cargaPedido.Pedido.ObservacaoCTe;
        //            cargaIntegracao.Temperatura = cargaPedido.Pedido.Temperatura;
        //            cargaIntegracao.ValidarNumeroContainer = cargaPedido.Pedido.ValidarDigitoVerificadorContainer;
        //            cargaIntegracao.PropostaComercial = ConverterObjetoPropostaComercial(cargaPedido.Pedido);
        //            cargaIntegracao.Transbordo = ConverterObjetoTransbordo(cargaPedido.Pedido.PedidosTransbordo != null ? cargaPedido.Pedido.PedidosTransbordo.ToList() : null);
        //            int.TryParse(cargaPedido.Pedido.CodigoOS, out int codigoOS);
        //            cargaIntegracao.CodigoOrdemServico = codigoOS;
        //            cargaIntegracao.NumeroOrdemServico = cargaPedido.Pedido.NumeroOS;
        //            cargaIntegracao.Embarque = cargaPedido.Pedido.Embarque;
        //            cargaIntegracao.MasterBL = cargaPedido.Pedido.MasterBL;
        //            cargaIntegracao.NumeroDIEmbarque = cargaPedido.Pedido.NumeroDI;
        //            cargaIntegracao.ProvedorOS = serPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.ProvedorOS);
        //            int.TryParse(cargaPedido.Pedido.TaraContainer, out int taraContainer);
        //            cargaIntegracao.TaraContainer = taraContainer;
        //            if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.LacreContainerUm))
        //                cargaIntegracao.NumeroLacre1 = cargaPedido.Pedido.LacreContainerUm;
        //            if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.LacreContainerDois))
        //                cargaIntegracao.NumeroLacre2 = cargaPedido.Pedido.LacreContainerDois;
        //            if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.LacreContainerTres))
        //                cargaIntegracao.NumeroLacre3 = cargaPedido.Pedido.LacreContainerTres;
        //            int.TryParse(cargaPedido.Pedido.CodigoBooking, out int codigoBooking);
        //            cargaIntegracao.CodigoBooking = codigoBooking;
        //            cargaIntegracao.NumeroBL = cargaPedido.Pedido.NumeroBL;
        //            cargaIntegracao.NecessitaAverbacao = cargaPedido.Pedido.NecessitaAverbacaoAutomatica;
        //            cargaIntegracao.CargaRefrigeradaPrecisaEnergia = cargaPedido.Pedido.NecessitaEnergiaContainerRefrigerado;
        //            cargaIntegracao.QuantidadeTipoContainerReserva = cargaPedido.Pedido.QuantidadeTipoContainerReserva;
        //            cargaIntegracao.IMOClasse = cargaPedido.Pedido.IMOClasse;
        //            cargaIntegracao.IMOSequencia = cargaPedido.Pedido.IMOSequencia;
        //            cargaIntegracao.IMOUnidade = cargaPedido.Pedido.IMOUnidade;
        //            cargaIntegracao.FormaAverbacaoCTE = cargaPedido.Pedido.FormaAverbacaoCTE;
        //            cargaIntegracao.PercentualADValorem = cargaPedido.Pedido.ValorAdValorem;
        //            cargaIntegracao.TipoDocumentoAverbacao = cargaPedido.Pedido.TipoDocumentoAverbacao;
        //            cargaIntegracao.ObservacaoProposta = cargaPedido.Pedido.ObservacaoCTe;
        //            cargaIntegracao.TipoPropostaFeeder = cargaPedido.Pedido.TipoPropostaFeeder;
        //            cargaIntegracao.DescricaoTipoPropostaFeeder = cargaPedido.Pedido.DescricaoTipoPropostaFeeder;
        //            cargaIntegracao.DescricaoCarrierNavioViagem = cargaPedido.Pedido.DescricaoCarrierNavioViagem;
        //            cargaIntegracao.RealizarCobrancaTaxaDocumentacao = cargaPedido.Pedido.RealizarCobrancaTaxaDocumentacao;
        //            cargaIntegracao.QuantidadeConhecimentosTaxaDocumentacao = cargaPedido.Pedido.QuantidadeConhecimentosTaxaDocumentacao;
        //            cargaIntegracao.ValorTaxaDocumento = cargaPedido.Pedido.ValorTaxaDocumento;
        //            cargaIntegracao.ContainerADefinir = cargaPedido.Pedido.ContainerADefinir;
        //            cargaIntegracao.ValorCusteioSVM = cargaPedido.Pedido.ValorCusteioSVM;
        //            cargaIntegracao.QuantidadeContainerBooking = cargaPedido.Pedido.QuantidadeContainerBooking;
        //            cargaIntegracao.EmpresaResponsavel = ConverterObjetoEmpresaResponsavel(cargaPedido.Pedido.PedidoEmpresaResponsavel);
        //            cargaIntegracao.CentroCusto = ConverterObjetoCentroCustol(cargaPedido.Pedido.PedidoCentroCusto);
        //            cargaIntegracao.PedidoDeSVMTerceiro = cargaPedido.Pedido.PedidoDeSVMTerceiro;
        //            cargaIntegracao.NumeroCE = cargaPedido.Pedido.NumeroCEFeeder;
        //            cargaIntegracao.ValorTaxaFeeder = cargaPedido.Pedido.ValorTaxaFeeder;
        //            cargaIntegracao.TipoCalculoCargaFracionada = cargaPedido.Pedido.TipoCalculoCargaFracionada;
        //            cargaIntegracao.SituacaoCarga = cargaPedido.Carga.SituacaoCarga;
        //            cargaIntegracao.DataInicioViagem = cargaPedido.Carga.DataInicioViagem.HasValue ? cargaPedido.Carga.DataInicioViagem.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;

        //            cargaIntegracao.PossuiPendenciaRoteirizacao = (configuracaoTMS.ExigirRotaRoteirizadaNaCarga && cargaPedido.Carga.SituacaoRoteirizacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro);

        //            if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
        //            {
        //                if (cargaPedido.Carga.CargaCancelamento != null && cargaPedido.Carga.CargaCancelamento.DataCancelamento.HasValue)
        //                    cargaIntegracao.DataCancelamentoCarga = cargaPedido.Carga.CargaCancelamento?.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss");
        //                else
        //                {
        //                    Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(cargaPedido.Carga.Codigo);
        //                    if (cargaCancelamento != null && cargaCancelamento.DataCancelamento.HasValue)
        //                        cargaIntegracao.DataCancelamentoCarga = cargaCancelamento.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss");
        //                }
        //            }
        //            if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
        //            {
        //                if (cargaPedido.Carga.CargaCancelamento != null && cargaPedido.Carga.CargaCancelamento.DataCancelamento.HasValue)
        //                    cargaIntegracao.DataAnulacaoCarga = cargaPedido.Carga.CargaCancelamento?.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss");
        //                else
        //                {
        //                    Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(cargaPedido.Carga.Codigo);
        //                    if (cargaCancelamento != null && cargaCancelamento.DataCancelamento.HasValue)
        //                        cargaIntegracao.DataAnulacaoCarga = cargaCancelamento.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss");
        //                }
        //            }

        //            cargasIntegracao.Add(cargaIntegracao);
        //        }
        //    }
        //    return cargasIntegracao;
        //}

        private Dominio.ObjetosDeValor.WebService.Carga.Resumo.Carga BuscarResumoCargasPedidos(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.WebService.Carga.Resumo.Carga cargaResumo = null;

            if (string.IsNullOrWhiteSpace(mensagem))
            {
                cargaResumo = new Dominio.ObjetosDeValor.WebService.Carga.Resumo.Carga();

                cargaResumo.Protocolo = cargaPedidos.FirstOrDefault().CargaOrigem.Protocolo;
                cargaResumo.NumeroCarga = cargaPedidos.FirstOrDefault().CargaOrigem.CodigoCargaEmbarcador;
                cargaResumo.Situacao = cargaPedidos.FirstOrDefault().Carga.DescricaoSituacaoCarga;

                cargaResumo.Pedidos = new List<Dominio.ObjetosDeValor.WebService.Carga.Resumo.Pedido>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    Dominio.ObjetosDeValor.WebService.Carga.Resumo.Pedido pedidoResumo = new Dominio.ObjetosDeValor.WebService.Carga.Resumo.Pedido();

                    pedidoResumo.NumeroPedidoEmbarcador = cargaPedido.Pedido.NumeroPedidoEmbarcador;
                    pedidoResumo.Protocolo = cargaPedido.Pedido.Protocolo;
                    pedidoResumo.CTe = new List<Dominio.ObjetosDeValor.WebService.Carga.Resumo.Documento>();
                    pedidoResumo.NFSe = new List<Dominio.ObjetosDeValor.WebService.Carga.Resumo.Documento>();
                    if (cargaPedido.PossuiCTe)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCargaPedido(cargaPedido.Codigo);
                        if (cargaCTe != null)
                        {
                            string mensagemValidacao = cargaCTe.CTe?.Status == "F" ? "Contingência FSDA" : cargaCTe.CTe != null ? cargaCTe.CTe.MensagemStatus != null ? cargaCTe.CTe.MensagemStatus.CodigoDoErro.ToString() + " " + cargaCTe.CTe.MensagemStatus.MensagemDoErro : cargaCTe.CTe.MensagemRetornoSefaz : string.Empty;

                            Dominio.ObjetosDeValor.WebService.Carga.Resumo.Documento cte = new Dominio.ObjetosDeValor.WebService.Carga.Resumo.Documento();
                            cte.ChaveCTe = cargaCTe.CTe?.ChaveAcesso;
                            cte.Mensagem = mensagemValidacao;
                            cte.CodigoMensagem = cargaCTe.CTe?.Status == "F" ? "5" : string.IsNullOrWhiteSpace(mensagemValidacao) ? "0" : cargaCTe.CTe?.MensagemStatus != null ? cargaCTe.CTe.MensagemStatus.CodigoDoErro.ToString() : "0";
                            cte.Status = cargaCTe.CTe.DescricaoStatus;
                            pedidoResumo.CTe.Add(cte);
                        }
                    }
                    if (cargaPedido.PossuiNFSManual)
                    {
                        Dominio.ObjetosDeValor.WebService.Carga.Resumo.Documento nfs = new Dominio.ObjetosDeValor.WebService.Carga.Resumo.Documento();
                        nfs.Mensagem = "99 NFs Manual";
                        nfs.CodigoMensagem = "99";
                        nfs.Status = "Manual"; ;
                        pedidoResumo.NFSe.Add(nfs);
                    }
                    if (cargaPedido.PossuiNFS)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCargaPedido(cargaPedido.Codigo);
                        if (cargaCTe != null)
                        {
                            string mensagemValidacao = cargaCTe.CTe != null ? cargaCTe.CTe.MensagemStatus != null ? cargaCTe.CTe.MensagemStatus.CodigoDoErro.ToString() + " " + cargaCTe.CTe.MensagemStatus.MensagemDoErro : cargaCTe.CTe.MensagemRetornoSefaz : string.Empty;

                            Dominio.ObjetosDeValor.WebService.Carga.Resumo.Documento nfse = new Dominio.ObjetosDeValor.WebService.Carga.Resumo.Documento();
                            nfse.ChaveCTe = cargaCTe.CTe?.ChaveAcesso;
                            nfse.Mensagem = "NFSe " + mensagemValidacao;
                            nfse.CodigoMensagem = string.IsNullOrWhiteSpace(mensagemValidacao) ? "0" : cargaCTe.CTe?.MensagemStatus != null ? cargaCTe.CTe.MensagemStatus.CodigoDoErro.ToString() : "0";
                            nfse.Status = cargaCTe.CTe.DescricaoStatus;
                            pedidoResumo.NFSe.Add(nfse);
                        }
                    }
                    cargaResumo.Pedidos.Add(pedidoResumo);
                }
            }
            return cargaResumo;
        }

        private void PreecherDadosCarga(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repositorioConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLacre repCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repositorioConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();

            if (cargaIntegracao.ObservacaoLocalEntrega != null && cargaIntegracao.ObservacaoLocalEntrega.Length > 300)
            {
                stMensagem.Append("Não é permitido incluir uma observação de local de entrega com mais de 300 caracteres. ");
                return;
            }

            if (cargaIntegracao.TipoCargaEmbarcador != null)
            {
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                if (!string.IsNullOrWhiteSpace(cargaIntegracao.TipoCargaEmbarcador.Descricao))
                    carga.TipoDeCarga = repositorioTipoCarga.BuscarPorDescricao(cargaIntegracao.TipoCargaEmbarcador.Descricao);

                if (carga.TipoDeCarga == null)
                    carga.TipoDeCarga = repositorioTipoCarga.BuscarPorCodigoEmbarcador(cargaIntegracao.TipoCargaEmbarcador.CodigoIntegracao);
            }

            if (cargaIntegracao.Lacres != null && cargaIntegracao.Lacres.Count > 0)
            {
                List<string> numerosAtuais = repCargaLacre.BuscarNumerosPorCargaAsync(carga.Codigo, default).GetAwaiter().GetResult();

                foreach (string lacre in cargaIntegracao.Lacres)
                {
                    string novoNumero = Utilidades.String.Left(lacre, 60);

                    if (numerosAtuais.Contains(novoNumero))
                        continue;

                    Dominio.Entidades.Embarcador.Cargas.CargaLacre cargaLacre = new Dominio.Entidades.Embarcador.Cargas.CargaLacre();
                    cargaLacre.Numero = novoNumero;
                    cargaLacre.Carga = carga;
                    repCargaLacre.Inserir(cargaLacre);
                }
            }

            if (cargaIntegracao.FreteRota != null)
            {
                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                carga.Rota = repositorioRotaFrete.BuscarPorCodigoIntegracao(cargaIntegracao.FreteRota.Codigo);

                if (carga.Rota == null)
                    stMensagem.Append("Não foi encontrado uma rota com o código " + cargaIntegracao.FreteRota.Codigo + " na base da Multisoftware");

                //if(carga.Rota.SituacaoDaRoteirizacao == SituacaoRoteirizacao.)
            }

            if (carga.Rota == null && pedido.RotaFrete != null)
                carga.Rota = pedido.RotaFrete;

            new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).ValidaAtualizaZonaExclusaoRota(carga.Rota);

            if (cargaIntegracao.ModeloVeicular != null)
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                carga.ModeloVeicularCarga = repositorioModeloVeicular.buscarPorCodigoIntegracao(cargaIntegracao.ModeloVeicular.CodigoIntegracao);

                if (configuracaoEmbarcador.PermitirAtualizarModeloVeicularCargaDoVeiculoNoWebService && carga.ModeloVeicularCarga != null && !string.IsNullOrWhiteSpace(cargaIntegracao.ModeloVeicular.Descricao) && cargaIntegracao.ModeloVeicular.Descricao != carga.ModeloVeicularCarga.Descricao)
                {
                    carga.ModeloVeicularCarga.Initialize();
                    carga.ModeloVeicularCarga.Descricao = cargaIntegracao.ModeloVeicular.Descricao;

                    repositorioModeloVeicular.Atualizar(carga.ModeloVeicularCarga, auditado);
                }

                if (carga.ModeloVeicularCarga == null && !string.IsNullOrWhiteSpace(cargaIntegracao.ModeloVeicular.Descricao))
                    carga.ModeloVeicularCarga = repositorioModeloVeicular.BuscarPorDescricaoETipo(cargaIntegracao.ModeloVeicular.Descricao, TipoModeloVeicularCarga.Geral);

                if (carga.ModeloVeicularCarga == null && !string.IsNullOrWhiteSpace(cargaIntegracao.ModeloVeicular.Descricao))
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga novoModeloVeicular = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga();

                    novoModeloVeicular.CodigoIntegracao = cargaIntegracao.ModeloVeicular.CodigoIntegracao;
                    novoModeloVeicular.Descricao = cargaIntegracao.ModeloVeicular.Descricao;
                    novoModeloVeicular.Ativo = true;
                    novoModeloVeicular.CapacidadePesoTransporte = 30000;
                    novoModeloVeicular.ToleranciaPesoExtra = 5000;
                    novoModeloVeicular.ToleranciaPesoMenor = 1;
                    novoModeloVeicular.VeiculoPaletizado = false;
                    novoModeloVeicular.ModeloControlaCubagem = false;
                    novoModeloVeicular.ModeloTracaoReboquePadrao = false;
                    novoModeloVeicular.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral;
                    repositorioModeloVeicular.Inserir(novoModeloVeicular, auditado);

                    carga.ModeloVeicularCarga = novoModeloVeicular;
                }

                if (carga.ModeloVeicularCarga != null && carga.ModeloVeicularCarga.Tipo == TipoModeloVeicularCarga.Tracao)
                    carga.ModeloVeicularCarga = null;
            }

            PreecherDadosVeiculo(carga, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, configuracaoEmbarcador, unitOfWork, auditado);
            PreecherDadosMotorista(carga, cargaIntegracao, ref stMensagem, tipoServicoMultisoftware, configuracaoEmbarcador, unitOfWork, auditado, clienteAcesso, adminStringConexao);
            PreecherDadosTipoOperacao(carga, cargaIntegracao, pedido, pedido.ViaTransporte?.TipoOperacaoPadrao == null ? tipoOperacao : pedido.ViaTransporte?.TipoOperacaoPadrao, ref stMensagem, tipoServicoMultisoftware, configuracaoEmbarcador, unitOfWork, auditado);
            PreecherDadosEmpresa(carga, cargaIntegracao, pedido, ref stMensagem, unitOfWork);
            PreencherMotoristaGenerico(carga, unitOfWork, auditado);

            carga.ObservacaoLocalEntrega = cargaIntegracao.ObservacaoLocalEntrega;

            if (cargaIntegracao.FaixaTemperatura != null && (!string.IsNullOrEmpty(cargaIntegracao.FaixaTemperatura.CodigoIntegracao) || !string.IsNullOrEmpty(cargaIntegracao.FaixaTemperatura.Descricao)))
            {
                Repositorio.Embarcador.Cargas.FaixaTemperatura repositorioFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);

                if (!string.IsNullOrEmpty(cargaIntegracao.FaixaTemperatura.CodigoIntegracao))
                {
                    carga.FaixaTemperatura = repositorioFaixaTemperatura.BuscarPorCodigoIntegracao(cargaIntegracao.FaixaTemperatura.CodigoIntegracao);

                    if (carga.FaixaTemperatura == null)
                    {
                        stMensagem.Append("Não foi localizada uma faixa de temperatura cadastrada para com o código " + cargaIntegracao.FaixaTemperatura.CodigoIntegracao + " na base Multisoftware.");
                        return;
                    }
                }
                else
                {
                    int codigoTipoOperacao = carga?.TipoOperacao?.Codigo ?? 0;
                    if (codigoTipoOperacao == 0)
                        codigoTipoOperacao = pedido.PreCarga?.TipoOperacao?.Codigo ?? 0;

                    carga.IntegracaoTemperatura = cargaIntegracao.FaixaTemperatura.Descricao;
                    carga.FaixaTemperatura = repositorioFaixaTemperatura.BuscarPorDescricao(cargaIntegracao.FaixaTemperatura.Descricao, carga.Filial?.Codigo ?? 0, codigoTipoOperacao);

                    if (configuracaoWebService.PermitirUsarDescricaoFaixaTemperatura && carga.FaixaTemperatura == null)
                    {
                        Regex regex = new Regex(@"-?[0-9]\d*((\.|,)\d+)?");
                        List<decimal> temperaturas = new List<decimal>();

                        foreach (Match match in regex.Matches(cargaIntegracao.FaixaTemperatura.Descricao))
                            temperaturas.Add(match.Value.ToDecimal());

                        if (temperaturas.Count == 2)
                        {
                            Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemp = new Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura
                            {
                                Descricao = cargaIntegracao.FaixaTemperatura.Descricao,
                                FaixaInicial = temperaturas[0],
                                FaixaFinal = temperaturas[1]
                            };

                            repositorioFaixaTemperatura.Inserir(faixaTemp);

                            carga.FaixaTemperatura = faixaTemp;
                        }
                        else
                        {
                            stMensagem.Append("A faixa de temperatura informada é inválida");
                        }
                    }
                }
            }
            else
                carga.FaixaTemperatura = null;

            if (pedido.PedidoViagemNavio != null)
                carga.PedidoViagemNavio = pedido.PedidoViagemNavio;

            if (configuracaoEmbarcador.UtilizaEmissaoMultimodal)
            {
                carga.NaoGerarMDFe = true;
                carga.NaoExigeVeiculoParaEmissao = true;
            }

            if (pedido.Porto != null)
                carga.PortoOrigem = pedido.Porto;

            if (pedido.PortoDestino != null)
                carga.PortoDestino = pedido.PortoDestino;

            if (pedido.TerminalOrigem != null)
                carga.TerminalOrigem = pedido.TerminalOrigem;

            if (pedido.TerminalDestino != null)
                carga.TerminalDestino = pedido.TerminalDestino;

            if (pedido.ContainerTipoReserva != null)
                carga.TipoContainer = pedido.ContainerTipoReserva;

            if ((configuracaoContratoFreteTerceiro?.GerarCargaTerceiroApenasProvedorPedido ?? false) && pedido?.ProvedorOS != null)
                carga.ProvedorOS = pedido.ProvedorOS;

            if (pedido?.TipoOperacao != null)
                carga.CargaBloqueadaParaEdicaoIntegracao = pedido?.TipoOperacao?.CargaBloqueadaParaEdicaoIntegracao ?? false;

            if (cargaIntegracao.ContemCargaPerigosa)
                carga.CargaPerigosaIntegracaoLeilao = true;

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.ObservacaoTransportador))
                carga.ObservacaoTransportador = cargaIntegracao.ObservacaoTransportador;

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroOT))
                carga.NumeroOT = cargaIntegracao.NumeroOT;

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroTransferencia))
            {
                int limite = carga.GetPropertyLength(nameof(carga.NumeroTransferencia));

                if (cargaIntegracao.NumeroTransferencia.Length > limite)
                {
                    stMensagem.Append($"O número da transferência não pode conter mais que {limite} caracteres. ");
                    return;
                }

                carga.NumeroTransferencia = cargaIntegracao.NumeroTransferencia;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.Alocacao))
            {
                int limite = carga.GetPropertyLength(nameof(carga.Alocacao));

                if (cargaIntegracao.Alocacao.Length > limite)
                {
                    stMensagem.Append($"A alocação não pode conter mais que {limite} caracteres. ");
                    return;
                }

                carga.Alocacao = cargaIntegracao.Alocacao;
            }

            carga.ObservacaoParaFaturamento = pedido.ObservacaoParaFaturamento;
            carga.TipoServicoCarga = pedido.TipoServicoCarga;
        }

        private void PreecherDadosEmpresa(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaIntegracao.TransportadoraEmitente != null)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                carga.Empresa = repositorioEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cargaIntegracao.TransportadoraEmitente.CNPJ));

                if (carga.Empresa == null)
                    stMensagem.Append("Não foi encontrado um transportador para o CNPJ " + cargaIntegracao.TransportadoraEmitente.CNPJ + "na base da Multisoftware");
            }
            else if (pedido.Empresa != null)
                carga.Empresa = pedido.Empresa;
        }

        private void PreecherDadosMotorista(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao)
        {
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Dominio.Entidades.Usuario veiculoMotorista = null;
            if (carga.Veiculo != null)
                veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(carga.Veiculo.Codigo);

            if (cargaIntegracao.Motoristas != null)
            {
                //Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                Servicos.WebService.Empresa.Motorista servicoMotorista = new Empresa.Motorista(unitOfWork);
                List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao in cargaIntegracao.Motoristas)
                {
                    Dominio.Entidades.Usuario motorista = ObterMotoristaParaPreencherDados(motoristaIntegracao, carga, tipoServicoMultisoftware, unitOfWork);

                    if (motorista != null && motorista.Status == "I")
                    {
                        motorista.Status = "A";
                        repositorioUsuario.Atualizar(motorista);
                        if (auditado != null)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, motorista, $"Cadastro ativo por integração.", unitOfWork);
                    }

                    if (motorista == null)
                    {
                        string mensagem = "";
                        motorista = servicoMotorista.SalvarMotorista(motoristaIntegracao, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : carga.Empresa, ref mensagem, unitOfWork, tipoServicoMultisoftware, auditado, clienteAcesso, adminStringConexao);
                        stMensagem.Append(mensagem);
                    }

                    if (motorista == null)
                        continue;

                    if (configuracaoEmbarcador.AtualizarVinculoVeiculoMotoristaIntegracaoCarga && carga.Veiculo != null && veiculoMotorista?.Codigo != motorista.Codigo)
                    {
                        Log.TratarErro($"AtualizarVinculoVeiculoMotoristaIntegracaoCarga: De {veiculoMotorista?.CPF ?? "--"} para {motorista.CPF} em {carga.Veiculo.Placa}", "AdicionarCarga");
                        //Dominio.Entidades.Veiculo veiculoDoMotorista = repositorioVeiculo.BuscarPorMotorista(motorista.Codigo);

                        //if (veiculoDoMotorista != null)
                        //{
                        //    veiculoDoMotorista.Motorista = null;
                        //    repositorioVeiculo.Atualizar(veiculoDoMotorista);
                        //}

                        //carga.Veiculo.Motorista = motorista;
                        //repositorioVeiculo.Atualizar(carga.Veiculo);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, carga.Veiculo, $"Removido motorista principal.", unitOfWork);
                        repVeiculoMotorista.DeletarMotoristaPrincipal(carga.Veiculo.Codigo);

                        Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista VeiculoMotoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                        {
                            CPF = motorista.CPF,
                            Motorista = motorista,
                            Nome = motorista.Nome,
                            Veiculo = carga.Veiculo,
                            Principal = true
                        };

                        repVeiculoMotorista.Inserir(VeiculoMotoristaPrincipal);
                    }

                    if (configuracaoEmbarcador.AtualizarEnderecoMotoristaIntegracaoCarga && motoristaIntegracao.Endereco != null)
                    {
                        string mensagem = "";
                        servicoMotorista.SalvarMotorista(motoristaIntegracao, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : carga.Empresa, ref mensagem, unitOfWork, tipoServicoMultisoftware, auditado, clienteAcesso, adminStringConexao);
                        Servicos.Log.TratarErro(mensagem + " : " + Newtonsoft.Json.JsonConvert.SerializeObject(motoristaIntegracao), "AtualizarEnderecoMotoristaIntegracaoCarga");
                    }

                    motoristas.Add(motorista);
                }

                servicoCargaMotorista.AtualizarMotoristas(carga, motoristas);
            }
            else if (veiculoMotorista != null)
                servicoCargaMotorista.AtualizarMotorista(carga, veiculoMotorista);
        }

        private void PreecherDadosTipoOperacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (tipoOperacao != null)
            {
                carga.TipoOperacao = tipoOperacao;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    if (carga.TipoOperacao.UtilizarExpedidorComoTransportador && carga.Empresa == null && cargaIntegracao.Expedidor != null)//quando o expedidor será o transportador. Exemplos Armazéns que fazem a entrega.
                    {
                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                        carga.Empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cargaIntegracao.Expedidor.CPFCNPJ));
                    }

                    carga.ExigeNotaFiscalParaCalcularFrete = carga.TipoOperacao.ExigeNotaFiscalParaCalcularFrete;
                    carga.NaoExigeVeiculoParaEmissao = carga.TipoOperacao.NaoExigeVeiculoParaEmissao;

                    if (carga.TipoOperacao.TipoDeCargaPadraoOperacao != null)
                        carga.TipoDeCarga = carga.TipoOperacao.TipoDeCargaPadraoOperacao;
                }
                else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {// caso na integração venha um tipo de operação que não pertence ao grupo do remetente ou destinatário da carga o sistema ignora e deixa sem, futuramente criar uma configuração para em algumas integrações rejeite (conforme solicitação do cliente)

                    Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
                    serPedidoWS.AtualizarTipoOperacao(ref tipoOperacao, cargaIntegracao.TipoOperacao, ref stMensagem, auditado);

                    bool verificarPorGrupo = (carga.TipoOperacao.GrupoPessoas != null);
                    bool verificarPorPessoa = (carga.TipoOperacao.Pessoa != null);

                    if (verificarPorPessoa)
                    {
                        if ((pedido.Remetente == null || pedido.Remetente.CPF_CNPJ != carga.TipoOperacao.Pessoa.CPF_CNPJ) && (pedido.Destinatario == null || pedido.Destinatario.CPF_CNPJ != carga.TipoOperacao.Pessoa.CPF_CNPJ))
                            carga.TipoOperacao = null;
                    }
                    else if (verificarPorGrupo)
                    {
                        if ((pedido.Remetente.GrupoPessoas == null || pedido.Remetente.GrupoPessoas.Codigo != carga.TipoOperacao.GrupoPessoas.Codigo) && (pedido.Destinatario.GrupoPessoas == null || pedido.Destinatario.GrupoPessoas.Codigo != carga.TipoOperacao.GrupoPessoas.Codigo))
                            carga.TipoOperacao = null;
                    }
                }
            }

            if (carga.TipoOperacao == null)
                carga.ExigeNotaFiscalParaCalcularFrete = configuracaoEmbarcador.ExigirNotaFiscalParaCalcularFreteCarga;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                carga.ExigeNotaFiscalParaCalcularFrete = true;
        }

        private void PreecherDadosVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

            bool veiculoAlterado = false;

            if (cargaIntegracao.Veiculo != null)
            {
                Servicos.WebService.Frota.Veiculo servicoVeiculo = new Frota.Veiculo(unitOfWork);
                int codigoEmpresa = 0;

                if ((carga.Empresa != null) && (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    codigoEmpresa = carga.Empresa.Codigo;

                if (!string.IsNullOrWhiteSpace(cargaIntegracao.Veiculo.Placa) && !string.IsNullOrWhiteSpace(cargaIntegracao.Veiculo.Placa.Replace("-", "")))
                {
                    Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlacaVarrendoFiliais(codigoEmpresa, cargaIntegracao.Veiculo.Placa.Replace("-", ""));

                    string mensagemVeiculo = "";

                    if (veiculo != null)
                    {
                        if (configuracaoWebService?.AtualizarDadosVeiculoIntegracaoCarga ?? false)
                            servicoVeiculo.AtualizarVeiculo(cargaIntegracao.Veiculo, veiculo, ref mensagemVeiculo, unitOfWork, auditado);
                    }
                    else
                    {
                        veiculo = servicoVeiculo.SalvarVeiculo(cargaIntegracao.Veiculo, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : carga.Empresa, false, ref mensagemVeiculo, unitOfWork, tipoServicoMultisoftware, auditado);

                        veiculoAlterado = true;
                    }

                    stMensagem.Append(mensagemVeiculo);

                    int numeroReboques = (carga.ModeloVeicularCarga ?? carga.VeiculosVinculados?.FirstOrDefault()?.ModeloVeicularCarga ?? veiculo.ModeloVeicularCarga)?.NumeroReboques ?? 0;
                    bool adicionarVeiculoComoReboque = (configuracaoWebService?.AdicionarVeiculoTipoReboqueComoReboqueAoAdicionarCarga ?? false) && (veiculo?.IsTipoVeiculoReboque() ?? false) && (numeroReboques > 0) && ((carga.VeiculosVinculados == null) || (carga.VeiculosVinculados?.Count < numeroReboques));

                    if (!adicionarVeiculoComoReboque)
                        carga.Veiculo = veiculo;
                    else
                    {
                        if (carga.VeiculosVinculados == null)
                            carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                        if (!carga.VeiculosVinculados.Contains(veiculo))
                            carga.VeiculosVinculados.Add(veiculo);

                        return;
                    }
                }

                if (cargaIntegracao.Veiculo.Reboques != null && cargaIntegracao.Veiculo.Reboques.Count > 0)
                {
                    if (carga.VeiculosVinculados != null)
                        carga.VeiculosVinculados.Clear();

                    carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                    foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboqueIntegracao in cargaIntegracao.Veiculo.Reboques)
                    {
                        if (!string.IsNullOrWhiteSpace(reboqueIntegracao.Placa))
                        {
                            Dominio.Entidades.Veiculo reboque = repositorioVeiculo.BuscarPorPlacaVarrendoFiliais(codigoEmpresa, reboqueIntegracao.Placa.Replace("-", ""));

                            string mensagemVeiculo = "";

                            if (reboque != null)
                            {
                                if (configuracaoWebService?.AtualizarDadosVeiculoIntegracaoCarga ?? false)
                                    servicoVeiculo.AtualizarVeiculo(reboqueIntegracao, reboque, ref mensagemVeiculo, unitOfWork, auditado);

                                carga.VeiculosVinculados.Add(reboque);
                            }
                            else
                            {
                                reboque = servicoVeiculo.SalvarVeiculo(reboqueIntegracao, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : carga.Empresa, false, ref mensagemVeiculo, unitOfWork, tipoServicoMultisoftware, auditado);

                                if (string.IsNullOrWhiteSpace(mensagemVeiculo))
                                    carga.VeiculosVinculados.Add(reboque);

                                veiculoAlterado = true;
                            }

                            stMensagem.Append(mensagemVeiculo);
                        }
                    }
                }

                if (carga.VeiculosVinculados == null || carga.VeiculosVinculados.Count == 0)
                {
                    if (carga.VeiculosVinculados == null)
                        carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                    if (carga.Veiculo != null && carga.Veiculo.VeiculosVinculados != null)
                    {
                        if (carga.Veiculo.TipoVeiculo == "1")
                        {
                            if (carga.Veiculo.VeiculosTracao != null && carga.Veiculo.VeiculosTracao.Count > 0)
                            {
                                carga.Veiculo = carga.Veiculo.VeiculosTracao.FirstOrDefault();
                            }
                        }

                        foreach (Dominio.Entidades.Veiculo reboque in carga.Veiculo.VeiculosVinculados)
                        {
                            carga.VeiculosVinculados.Add(reboque);
                        }
                    }
                    else if (carga.Veiculo != null && carga.Veiculo.VeiculosTracao != null)
                    {
                        if (carga.Veiculo.TipoVeiculo == "1")
                        {
                            if (carga.Veiculo.VeiculosTracao.Count > 0)
                            {
                                carga.VeiculosVinculados.Add(carga.Veiculo);
                                carga.Veiculo = carga.Veiculo.VeiculosTracao.FirstOrDefault();
                            }
                        }
                    }
                }
            }

            if (configuracaoEmbarcador.AtualizarVinculoVeiculoMotoristaIntegracaoCarga && !veiculoAlterado && (carga.Veiculo != null) && (carga.VeiculosVinculados?.Count > 0))
            {
                bool alterarVeiculosVinculados = false;

                if (carga.Veiculo.VeiculosVinculados?.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in carga.Veiculo.VeiculosVinculados)
                    {
                        alterarVeiculosVinculados = !(from o in carga.VeiculosVinculados where o.Codigo == reboque.Codigo select o).Any();

                        if (alterarVeiculosVinculados)
                            break;
                    }
                }
                else
                    alterarVeiculosVinculados = true;

                if (alterarVeiculosVinculados)
                {
                    if (carga.Veiculo.VeiculosVinculados == null)
                        carga.Veiculo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                    else
                        carga.Veiculo.VeiculosVinculados.Clear();

                    foreach (Dominio.Entidades.Veiculo novoReboque in carga.VeiculosVinculados)
                        carga.Veiculo.VeiculosVinculados.Add(novoReboque);

                    repositorioVeiculo.Atualizar(carga.Veiculo);
                }
            }
        }

        private void PreencherMotoristaGenerico(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            servicoCarga.PreencherMotoristaGenericoCarga(carga, auditado, unitOfWork);
        }

        private void PreencherDistribuidores(ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.CargaPedidoDistribuicao repCargaPedidoDistribuicao = new Repositorio.Embarcador.Cargas.CargaPedidoDistribuicao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (cargaIntegracao.Distribuicoes != null && cargaIntegracao.Distribuicoes.Count > 0 && (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
            {
                cargaPedido.PendenteGerarCargaDistribuidor = true;
                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Distribuicao distribuicao in cargaIntegracao.Distribuicoes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoDistribuicao cargaPedidoDistribuicao = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDistribuicao();
                    cargaPedidoDistribuicao.CargaPedido = cargaPedido;

                    if (distribuicao.TipoOperacao != null && !string.IsNullOrWhiteSpace(distribuicao.TipoOperacao.CodigoIntegracao))
                    {
                        Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                        cargaPedidoDistribuicao.TipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(distribuicao.TipoOperacao.CodigoIntegracao);
                        if (cargaPedidoDistribuicao.TipoOperacao == null)
                            stMensagem.Append("Não existe um tipo de operação cadastrado com o código de integração " + distribuicao.TipoOperacao.CodigoIntegracao + " para gerar a viagem do distribuidor.");
                    }

                    if (distribuicao.TransportadorDistribuidor != null)
                    {
                        cargaPedidoDistribuicao.Empresa = repEmpresa.BuscarPorCNPJ(distribuicao.TransportadorDistribuidor.CNPJ);
                        if (cargaPedidoDistribuicao.Empresa == null)
                            stMensagem.Append("Não existe o transportador cadastrado com o CNPJ " + distribuicao.TransportadorDistribuidor.CNPJ + " para gerar a viagem do distribuidor.");
                    }

                    repCargaPedidoDistribuicao.Inserir(cargaPedidoDistribuicao);

                }
                cargaPedido.Carga.PendenteGerarCargaDistribuidor = true;
                repCarga.Atualizar(cargaPedido.Carga);
            }
        }

        private void PreencherDatasCarga(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService)
        {
            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataInicioCarregamento))
            {
                DateTime data;
                if (!DateTime.TryParseExact(cargaIntegracao.DataInicioCarregamento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                {
                    stMensagem.Append("A data inicial de previsão de carregamento não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                }
                ;
                carga.DataInicialPrevisaoCarregamento = data;
            }

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataFinalCarregamento))
            {
                DateTime data;
                if (!DateTime.TryParseExact(cargaIntegracao.DataFinalCarregamento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                {
                    stMensagem.Append("A data final de previsão de carregamento não esta em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                }
                ;
                carga.DataFinalPrevisaoCarregamento = data;
            }

            if (carga.DataInicialPrevisaoCarregamento != null)
                carga.DataCarregamentoCarga = carga.DataInicialPrevisaoCarregamento;
            else
                carga.DataCarregamentoCarga = null;

            if (configuracaoWebService.QuantidadeHorasPreencherDataCarregamentoCarga > 0 && carga.Codigo == 0)
                carga.DataCarregamentoCarga = DateTime.Now.AddHours(configuracaoWebService.QuantidadeHorasPreencherDataCarregamentoCarga);

            if (!string.IsNullOrWhiteSpace(cargaIntegracao.DataSeparacaoMercadoria))
            {
                DateTime data;
                if (!DateTime.TryParseExact(cargaIntegracao.DataSeparacaoMercadoria, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                {
                    stMensagem.Append("A data final de separação de mercadoria não está em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                }
                ;
                carga.DataSeparacaoMercadoria = data;
            }
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.Carga> ObterDetalhesCargasParaIntegracao(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCargas, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.Carga> cargasIntegracao = new List<Dominio.ObjetosDeValor.WebService.Carga.Carga>();

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                cargasIntegracao.Add(ObterDetalhesCargaParaIntegracao(carga, cargaPedidosCargas, unitOfWork));

            return cargasIntegracao;
        }

        private Dominio.ObjetosDeValor.WebService.Carga.Carga ObterDetalhesCargaParaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);

            Servicos.WebService.Frota.Veiculo svcWSVeiculo = new Frota.Veiculo(unitOfWork);
            Servicos.WebService.Empresa.Motorista svcWSMotorista = new Empresa.Motorista(unitOfWork);

            Servicos.WebService.Empresa.Empresa svcWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa svcPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados svcCargaDadosSumarizados = new Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = null;
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            if (cargasPedidos == null)
                cargaPedidos = carga.Pedidos.ToList();
            else
                cargaPedidos = cargasPedidos.Where(o => o.Carga.Codigo == carga.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscalsCarga = repPedidoXmlNotaFiscal.BuscarPorCarga(carga.Codigo);

            Dominio.ObjetosDeValor.WebService.Carga.Carga cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.Carga()
            {
                DataCriacaoCarga = carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm:ss"),
                DataPrevisaoEntrega = carga.DataPrevisaoTerminoCarga?.ToString("dd/MM/yyyy"),
                NumeroCarga = carga.CodigoCargaEmbarcador,
                Origem = svcCargaDadosSumarizados.ObterOrigem(carga),
                Destino = svcCargaDadosSumarizados.ObterDestino(carga),
                Protocolo = carga.Codigo,
                Veiculo = svcWSVeiculo.ConverterObjetoConjuntoVeiculos(carga.Veiculo, carga.VeiculosVinculados.ToList(), unitOfWork),
                Motoristas = svcWSMotorista.ConverterListaObjetoMotorista(carga.Motoristas),
                Transportador = svcWSEmpresa.ConverterObjetoEmpresa(carga.Empresa),
                Pedidos = new List<Dominio.ObjetosDeValor.WebService.Carga.Pedido>(),
                TipoCarga = carga.TipoDeCarga != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador()
                {
                    CodigoIntegracao = carga.TipoDeCarga.CodigoTipoCargaEmbarcador,
                    Descricao = carga.TipoDeCarga.Descricao
                } : null,
                CargaIntegracaoValePedagio = this.ObterCargaIntegracaoValePegadio(carga, unitOfWork),
                CargaValePedagio = this.ObterCargaValePegadio(carga, unitOfWork),
                RotaFrete = cargaRotaFrete != null ? new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFrete()
                {
                    Polilinha = cargaRotaFrete.PolilinhaRota,
                    TempoViagemMinutos = cargaRotaFrete.TempoDeViagemEmMinutos,
                    TipoUltimoPontoRoteirizacao = cargaRotaFrete.TipoUltimoPontoRoteirizacao
                } : null,
                SituacaoCargaEmbarcador = carga.SituacaoCarga
            };

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = (from obj in pedidoXMLNotaFiscalsCarga where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList();

                Dominio.ObjetosDeValor.WebService.Carga.Pedido pedidoIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.Pedido()
                {
                    Destinatario = svcPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Destinatario),
                    Expedidor = svcPessoa.ConverterObjetoPessoa(cargaPedido.Expedidor),
                    Recebedor = svcPessoa.ConverterObjetoPessoa(cargaPedido.Recebedor),
                    Remetente = svcPessoa.ConverterObjetoPessoa(cargaPedido.Pedido.Remetente),
                    Tomador = svcPessoa.ConverterObjetoPessoa(cargaPedido.Tomador),
                    NumeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    Protocolo = cargaPedido.Codigo,
                    TipoPagamento = cargaPedido.Pedido.TipoPagamento,
                    TipoTomador = cargaPedido.TipoTomador,
                    TipoRateio = cargaPedido.TipoRateio,
                    PossuiCTe = cargaPedido.PossuiCTe,
                    PossuiNFS = cargaPedido.PossuiNFS,
                    PossuiNFSManual = cargaPedido.PossuiNFSManual,
                    NotasFiscais = new List<Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal>()
                };

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xMLNotaFiscalProdutos = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto>();
                if (carga.TipoOperacao?.AtualizarProdutosPorXmlNotaFiscal ?? false)
                    xMLNotaFiscalProdutos = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(carga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                {
                    Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal notaFiscalIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal()
                    {
                        Chave = pedidoXMLNotaFiscal.XMLNotaFiscal.Chave,
                        DataEmissao = pedidoXMLNotaFiscal.XMLNotaFiscal.DataEmissao,
                        Destinatario = svcPessoa.ConverterObjetoPessoa(pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario),
                        Emitente = svcPessoa.ConverterObjetoPessoa(pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente),
                        Numero = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero,
                        Serie = pedidoXMLNotaFiscal.XMLNotaFiscal.Serie,
                        TipoOperacaoNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal.TipoOperacaoNotaFiscal,
                        Valor = pedidoXMLNotaFiscal.XMLNotaFiscal.Valor,
                        ValorFrete = pedidoXMLNotaFiscal.ValorFrete,
                        PossuiCTe = pedidoXMLNotaFiscal.PossuiCTe,
                        PossuiNFS = pedidoXMLNotaFiscal.PossuiNFS,
                        PossuiNFSManual = pedidoXMLNotaFiscal.PossuiNFSManual,
                        Peso = pedidoXMLNotaFiscal.XMLNotaFiscal.Peso,
                        Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>()
                    };

                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto xmlNotaFiscalProduto in xMLNotaFiscalProdutos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto
                        {
                            CodigoProduto = xmlNotaFiscalProduto.Produto.CodigoProdutoEmbarcador,
                            DescricaoProduto = xmlNotaFiscalProduto.Produto.Descricao,
                            CodigocEAN = xmlNotaFiscalProduto.Produto.CodigoCEAN,
                            UnidadeMedida = xmlNotaFiscalProduto.UnidadeMedida,
                            Quantidade = xmlNotaFiscalProduto.Quantidade,
                            ValorUnitario = xmlNotaFiscalProduto.ValorProduto
                        };

                        notaFiscalIntegracao.Produtos.Add(produto);
                    }

                    pedidoIntegracao.NotasFiscais.Add(notaFiscalIntegracao);
                }

                cargaIntegracao.Pedidos.Add(pedidoIntegracao);
            }

            return cargaIntegracao;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.CargaValePedagio> ObterCargaValePegadio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.CargaValePedagio> retorno = null;
            Servicos.WebService.Pessoas.Pessoa svcPessoa = new Pessoas.Pessoa(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> cargasValePedagio = repositorioCargaValePedagio.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaValePedagio cargaValePedagio in cargasValePedagio)
            {
                if (string.IsNullOrEmpty(cargaValePedagio.NumeroComprovante))
                    continue;

                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaValePedagio>();

                Dominio.ObjetosDeValor.WebService.Carga.CargaValePedagio valePedagio = new Dominio.ObjetosDeValor.WebService.Carga.CargaValePedagio();
                valePedagio.CodigoValePedagioEmbarcador = cargaValePedagio.Codigo;
                valePedagio.Fornecedor = svcPessoa.ConverterObjetoPessoa(cargaValePedagio.Fornecedor);
                valePedagio.Responsavel = svcPessoa.ConverterObjetoPessoa(cargaValePedagio.Responsavel);
                valePedagio.NumeroComprovante = cargaValePedagio.NumeroComprovante;
                valePedagio.CodigoAgendamentoPorto = cargaValePedagio.CodigoAgendamentoPorto;
                valePedagio.Valor = cargaValePedagio.Valor;
                valePedagio.CodigoIntegracaoValePedagioEmbarcador = cargaValePedagio?.CargaIntegracaoValePedagio?.Codigo;
                valePedagio.TipoCompra = cargaValePedagio.TipoCompra;
                valePedagio.QuantidadeEixos = cargaValePedagio.QuantidadeEixos;
                valePedagio.NaoIncluirMDFe = cargaValePedagio.NaoIncluirMDFe;
                retorno.Add(valePedagio);
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoValePedagio> ObterCargaIntegracaoValePegadio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoValePedagio> retorno = null;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargasIntegracaoValePedagio = repositorioCargaIntegracaoValePedagio.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio in cargasIntegracaoValePedagio)
            {
                if (cargaIntegracaoValePedagio != null && cargaIntegracaoValePedagio.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    if (retorno == null)
                        retorno = new List<CargaIntegracaoValePedagio>();

                    Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoValePedagio integracaoValePedagio = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoValePedagio();
                    integracaoValePedagio.CodigoIntegracaoValePedagioEmbarcador = cargaIntegracaoValePedagio.Codigo;
                    integracaoValePedagio.SituacaoValePedagio = cargaIntegracaoValePedagio.SituacaoValePedagio;
                    integracaoValePedagio.NumeroValePedagio = cargaIntegracaoValePedagio.NumeroValePedagio;
                    integracaoValePedagio.IdCompraValePedagio = cargaIntegracaoValePedagio.IdCompraValePedagio;
                    integracaoValePedagio.ValorValePedagio = cargaIntegracaoValePedagio.ValorValePedagio;
                    integracaoValePedagio.Observacao1 = cargaIntegracaoValePedagio.Observacao1;
                    integracaoValePedagio.Observacao2 = cargaIntegracaoValePedagio.Observacao2;
                    integracaoValePedagio.Observacao3 = cargaIntegracaoValePedagio.Observacao3;
                    integracaoValePedagio.Observacao4 = cargaIntegracaoValePedagio.Observacao4;
                    integracaoValePedagio.Observacao5 = cargaIntegracaoValePedagio.Observacao5;
                    integracaoValePedagio.Observacao6 = cargaIntegracaoValePedagio.Observacao6;
                    integracaoValePedagio.RotaTemporaria = cargaIntegracaoValePedagio.RotaTemporaria;
                    integracaoValePedagio.CodigoIntegracaoValePedagio = cargaIntegracaoValePedagio.CodigoIntegracaoValePedagio;
                    integracaoValePedagio.TipoRota = cargaIntegracaoValePedagio.TipoRota;
                    integracaoValePedagio.TipoCompra = cargaIntegracaoValePedagio.TipoCompra;
                    integracaoValePedagio.CompraComEixosSuspensos = cargaIntegracaoValePedagio.CompraComEixosSuspensos;
                    integracaoValePedagio.CodigoRoteiro = cargaIntegracaoValePedagio.CodigoRoteiro;
                    integracaoValePedagio.CodigoPercurso = cargaIntegracaoValePedagio.CodigoPercurso;
                    integracaoValePedagio.QuantidadeEixos = cargaIntegracaoValePedagio.QuantidadeEixos;
                    integracaoValePedagio.RecebidoPorIntegracao = cargaIntegracaoValePedagio.RecebidoPorIntegracao;
                    integracaoValePedagio.ValidaCompraRemoveuComponentes = cargaIntegracaoValePedagio.ValidaCompraRemoveuComponentes;
                    integracaoValePedagio.NomeTransportador = cargaIntegracaoValePedagio.NomeTransportador;
                    integracaoValePedagio.TipoPercursoVP = cargaIntegracaoValePedagio.TipoPercursoVP;
                    integracaoValePedagio.CnpjMeioPagamento = cargaIntegracaoValePedagio.CnpjMeioPagamento;
                    retorno.Add(integracaoValePedagio);
                }
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.WebService.CargaCancelamento.CargaCancelamento> ObterDetalhesCancelamentosParaIntegracao(List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> cancelamentos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            return (from obj in cancelamentos
                    select new Dominio.ObjetosDeValor.WebService.CargaCancelamento.CargaCancelamento()
                    {
                        DataCancelamento = obj.DataCancelamento,
                        MotivoCancelamento = obj.MotivoCancelamento,
                        ProtocoloCancelamento = obj.Codigo,
                        ProtocoloCarga = obj.Carga.Codigo,
                        PossuiDocumentoCancelado = repCargaCTe.ExisteCanceladoPorCarga(obj.Carga.Codigo),
                        TipoCancelamentoCargaDocumento = obj.TipoCancelamentoCargaDocumento
                    }).ToList();
        }

        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> AdicionarPedidoAsync(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, bool notificarAcompanhamento, CancellationToken cancellationToken, bool naoFecharConexao = false)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Servicos.Global.TempoDeExecucao servicoTempoDeExecucao = new Global.TempoDeExecucao();

            Servicos.Log.TratarErro($"AdicionarPedido: {(cargaIntegracao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cargaIntegracao) : string.Empty)}", "Request");
            StringBuilder mensagemErro = new StringBuilder();

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte configEmailDocTransporteRepository = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = await configEmailDocTransporteRepository.BuscarEmailEnviaDocumentoAtivoAsync();

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
                Servicos.Embarcador.Integracao.IndicadorIntegracaoNFe servicoIndicadorIntegracaoNFe = new Servicos.Embarcador.Integracao.IndicadorIntegracaoNFe(_unitOfWork, configuracaoTMS);

                Servicos.Embarcador.Pedido.RegraNumeroPedidoEmbarcador servicoRegraNumeroPedidoEmbarcador = new Servicos.Embarcador.Pedido.RegraNumeroPedidoEmbarcador(_unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;

                int codigoPersonalizado = 0;

                await servicoRegraNumeroPedidoEmbarcador.DefinirNumeroPedidoEmbarcadorComRegraAsync(cargaIntegracao);

                Servicos.Log.TratarErro("2 - Iniciou Validacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                bool ignorarTransportadorNaoCadastrado = false;

                Servicos.WebService.Carga.Carga.ValidarCamposIntegracaoCarga(cargaIntegracao, configuracaoTMS.ReplicarCadastroVeiculoIntegracaoTransportadorDiferente, ignorarTransportadorNaoCadastrado, configuracaoTMS.BuscarClientesCadastradosNaIntegracaoDaCarga, configuracaoTMS.UtilizarProdutosDiversosNaIntegracaoDaCarga, ref mensagemErro, _unitOfWork, configuracaoTMS, _tipoServicoMultisoftware, out codigoPersonalizado, out tipoOperacao, out filial);

                if (mensagemErro.Length > 0)
                {
                    Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} Retornou essa mensagem (validação): {mensagemErro}", "RequestLog");
                    Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                    if (codigoPersonalizado > 0)
                    {
                        retorno.CodigoMensagem = codigoPersonalizado;
                        AuditarRetornoDadosInvalidosCNPJTransportador(_unitOfWork, mensagemErro.ToString(), _auditado, cargaIntegracao.NumeroCarga);
                    }
                    else
                        AuditarRetornoDadosInvalidos(_unitOfWork, mensagemErro.ToString(), _auditado, cargaIntegracao.NumeroCarga);

                    servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracao, mensagemErro.ToString());

                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro.ToString());

                    return retorno;
                }

                await _unitOfWork.StartAsync();

                int protocoloCargaExistente = 0;
                int protocoloPedidoExistente = 0;
                string codigoRasterio = "";
                string linkRastreio = "";
                string linkRastreioMapaEntrega = "";

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga(_unitOfWork);
                Servicos.WebService.Carga.Pedido servicoPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);
                Servicos.WebService.Empresa.Empresa servicoEmpresa = new Servicos.WebService.Empresa.Empresa(_unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                Servicos.Embarcador.Veiculo.Veiculo servicoVeiculo = new Servicos.Embarcador.Veiculo.Veiculo(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

                Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa objTransportador = cargaIntegracao.TransportadoraEmitente;
                if (string.IsNullOrEmpty(servicoEmpresa.ValidarCamposEmpresaIntegracao(objTransportador)) && !(await servicoEmpresa.EmpresaIntegracaoExisteAsync(objTransportador)))
                {
                    Servicos.Log.TratarErro("3 - Adicionando Transportador " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                    try
                    {
                        servicoEmpresa.AdicionarOutAtualizarEmpresa(objTransportador, _unitOfWork, _auditado, _adminStringConexao);
                    }
                    catch (ServicoException ex)
                    {
                        mensagemErro.Append(ex.Message);
                    }
                }
                if (mensagemErro.Length > 0)
                {
                    Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} Retornou essa mensagem: {mensagemErro}", "RequestLog");
                    await _unitOfWork.RollbackAsync();

                    AuditarRetornoDadosInvalidos(_unitOfWork, mensagemErro.ToString(), _auditado, cargaIntegracao.NumeroCarga);
                    servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracao, mensagemErro.ToString());

                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro.ToString());

                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                }
                Servicos.Log.TratarErro("4 - Iniciou Criar Pedido " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref mensagemErro, _tipoServicoMultisoftware, ref protocoloPedidoExistente, ref protocoloCargaExistente, false, _auditado, configuracaoTMS, _clienteURLAcesso, _adminStringConexao, ignorarPedidosInseridosManualmente: true, true, notificarAcompanhamento, false);

                if (cargaIntegracao.InformarSeparacaoPedido != null)
                    new Servicos.Embarcador.Pedido.Pedido().InformarSeparacaoPedido(pedido, cargaIntegracao.InformarSeparacaoPedido, _unitOfWork);

                if (pedido != null)
                    Servicos.Log.TratarErro("5 - Pedido Criado " + pedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                if (mensagemErro.Length == 0 || (protocoloPedidoExistente > 0 && pedido != null))
                {
                    bool manterPreCargaComoCarga = (!configuracaoTMS.TrocarPreCargaPorCarga && !configuracaoTMS.UtilizarSequenciaNumeracaoCargasViaIntegracao && (protocoloPedidoExistente > 0) && pedido.PedidoDePreCarga && !string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga));
                    bool permitirFecharCargaAutomaticamente = true;

                    if (manterPreCargaComoCarga)
                    {
                        servicoPedidoWS.AtualizarParticipantesPedido(ref pedido, ref cargaIntegracao, ref mensagemErro, _tipoServicoMultisoftware, null, _auditado);

                        cargaPedido = await repositorioCargaPedido.BuscarCargaAtualPorPedidoAsync(pedido.Codigo);

                        if (cargaPedido != null)
                        {
                            bool pesoAlterado = (cargaPedido.Peso != cargaIntegracao.PesoBruto) || (cargaPedido.PesoLiquido != cargaIntegracao.PesoLiquido);
                            pedido.PesoTotal = cargaIntegracao.PesoBruto;
                            pedido.PesoLiquidoTotal = cargaIntegracao.PesoLiquido;
                            pedido.CustoFrete = cargaIntegracao.CustoFrete;
                            //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                            Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {cargaIntegracao.PesoBruto}. Carga.AdicionarPedido", "PesoCargaPedido");
                            cargaPedido.Peso = cargaIntegracao.PesoBruto;
                            cargaPedido.PesoLiquido = cargaIntegracao.PesoLiquido;
                            cargaPedido.CustoFrete = cargaIntegracao.CustoFrete;
                            await repositorioCargaPedido.AtualizarAsync(cargaPedido);
                            servicoCargaWS.PreencherCargaPedidoOrigemDestino(ref cargaPedido, pedido, cargaIntegracao, ref mensagemErro, _tipoServicoMultisoftware, _unitOfWork);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = await repositorioCargaPedido.BuscarPorCargaAsync(cargaPedido.Carga.Codigo);
                            if (!cargaPedido.Carga.CargaDePreCargaEmFechamento)
                            {
                                Dominio.Entidades.Embarcador.Cargas.Carga preCarga = cargaPedido.Carga;
                                preCarga.CargaDePreCargaEmFechamento = true;
                                preCarga.CodigoCargaEmbarcador = cargaIntegracao.NumeroCarga;
                                servicoCargaWS.PreecherDadosPreCarga(preCarga, pedido, tipoOperacao, cargaIntegracao, ref mensagemErro, _tipoServicoMultisoftware, configuracaoTMS, _unitOfWork, _auditado);

                                await servicoCargaDadosSumarizados.AlterarDadosSumarizadosCargaPadraoAsync(preCarga, listaCargaPedidos, configuracaoTMS, _unitOfWork, _tipoServicoMultisoftware);
                                await repositorioCarga.AtualizarAsync(preCarga);

                                if (configuracaoTMS.SistemaIntegracaoPadraoCarga > 0)
                                {
                                    Servicos.Embarcador.Integracao.IntegracaoCarga serIntegracaoCarga = new Servicos.Embarcador.Integracao.IntegracaoCarga(_unitOfWork);
                                    serIntegracaoCarga.InformarIntegracaoCarga(preCarga, configuracaoTMS.SistemaIntegracaoPadraoCarga, _unitOfWork);
                                }
                            }
                            else if (pesoAlterado)
                                servicoCargaDadosSumarizados.AtualizarPesos(cargaPedido.Carga, listaCargaPedidos, _unitOfWork, _tipoServicoMultisoftware);
                            if (cargaPedido.Carga.CargaAgrupada)
                            {
                                if (!cargaPedido.CargaOrigem.CargaDePreCargaEmFechamento)
                                {
                                    cargaPedido.CargaOrigem.CargaDePreCargaEmFechamento = true;
                                    await repositorioCarga.AtualizarAsync(cargaPedido.CargaOrigem);
                                }
                                permitirFecharCargaAutomaticamente = !(await repositorioCarga.ExistePreCargaComFechamentoNaoIniciadoPorCargaAgrupadaAsync(cargaPedido.Carga.Codigo));
                            }

                            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                            pedido.PedidoDePreCarga = false;
                            pedido.CodigoCargaEmbarcador = cargaPedido.Carga.CodigoCargaEmbarcador;
                            await repositorioPedido.AtualizarAsync(pedido);

                            if (pesoAlterado)
                            {
                                Dominio.Entidades.Cliente destinatario = cargaPedido.Recebedor ?? pedido.Destinatario;
                                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = await repositorioCargaJanelaDescarregamento.BuscarAtivaPorCargaEDestinatarioAsync(cargaPedido.Carga.Codigo, destinatario.CPF_CNPJ);
                                if (cargaJanelaDescarregamento != null)
                                {
                                    Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade servicoJanelaDescarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade(_unitOfWork, configuracaoTMS);
                                    servicoJanelaDescarregamentoDisponibilidade.AtualizarDefinicaoHorarioDescarregamento(cargaJanelaDescarregamento);
                                }
                            }
                        }
                    }
                    else if (configuracaoTMS?.UtilizarProtocoloDaPreCargaNaCarga ?? false)
                    {
                        cargaPedido = await repositorioCargaPedido.BuscarPorNumeroCargaEPedidoAsync(cargaIntegracao.NumeroCarga, pedido.Codigo);

                        if (cargaPedido != null)
                        {
                            cargaPedido.CargaOrigem.CargaDePreCarga = false;
                            await repositorioCarga.AtualizarAsync(cargaPedido.CargaOrigem);
                            servicoCargaWS.InformarDocumentosCargaPedido(cargaPedido.Carga, cargaPedido, cargaIntegracao, ref mensagemErro, _unitOfWork, _tipoServicoMultisoftware, configuracaoTMS, _auditado, atualizar: false);
                        }
                    }

                    if (cargaPedido == null)
                    {
                        Servicos.Log.TratarErro("6 - Iniciou Produtos " + pedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                        if (protocoloPedidoExistente == 0)
                            new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork).AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref mensagemErro, _unitOfWork, _auditado);
                        Servicos.Log.TratarErro("7 - Finalizou Produtos " + pedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                        if (cargaIntegracao.Transbordo != null && cargaIntegracao.Transbordo.Count > 0)
                            new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork).SalvarTransbordo(pedido, cargaIntegracao.Transbordo, ref mensagemErro, _unitOfWork, _unitOfWork.StringConexao, _auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                        Servicos.Log.TratarErro("8 - inicou Criar Carga " + pedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                        cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemErro, ref protocoloCargaExistente, _unitOfWork, _tipoServicoMultisoftware, false, false, _auditado, configuracaoTMS, _clienteURLAcesso, _adminStringConexao, filial, tipoOperacao);
                        if (cargaPedido != null)
                        {
                            Servicos.Log.TratarErro("9 - Criou Carga " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                            VincularCargaPreCarga(cargaPedido, configuracaoTMS, _unitOfWork);

                            Servicos.Log.TratarErro("10 - Adicionar Produtos " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                            servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, _unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

                            Servicos.Log.TratarErro("11 - Criou Produtos  " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                            CriarDependenciaProdutoSeNecessario(cargaIntegracao, cargaPedido, _unitOfWork);
                            CriarDependenciaPorCidadeNaoCadastrada(cargaIntegracao, cargaPedido, _unitOfWork);

                            if (cargaPedido.CargaOrigem.CargaAgrupamento != null && cargaPedido.CargaOrigem.CargaAgrupamento.Codigo == cargaPedido.Carga.Codigo)
                            {
                                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
                                Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupamento = cargaPedido.Carga;
                                serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaAgrupamento, configuracaoTMS, _unitOfWork, _tipoServicoMultisoftware);
                            }

                            if (!configuracaoTMS.TrocarPreCargaPorCarga)
                            {
                                Dominio.Entidades.Cliente destinatario = cargaPedido.Recebedor ?? pedido.Destinatario;
                                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = await repositorioCargaJanelaDescarregamento.BuscarAtivaPorCargaEDestinatarioAsync(cargaPedido.Carga.Codigo, destinatario.CPF_CNPJ);
                                if (cargaJanelaDescarregamento != null)
                                {
                                    Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade servicoJanelaDescarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade(_unitOfWork, configuracaoTMS);
                                    servicoJanelaDescarregamentoDisponibilidade.AtualizarDefinicaoHorarioDescarregamento(cargaJanelaDescarregamento);
                                }
                            }
                        }
                    }
                    if (cargaPedido != null)
                    {
                        servicoCarga.PreencherDadosAdicionaisUnilever(cargaPedido, cargaIntegracao, _unitOfWork);

                        if (!string.IsNullOrEmpty(cargaIntegracao.NumeroControlePedido) && await repositorioCargaPedido.ExistePorNumeroControleECargaAsync(cargaIntegracao.NumeroControlePedido, cargaPedido.Carga.Codigo))
                        {
                            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AdicionarEntrega(cargaPedido, cargaPedido.ClienteEntrega, _unitOfWork, configuracaoTMS);
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NotificarCargaEntregaAdicionada(cargaEntrega, _clienteMultisoftware, _unitOfWork);
                        }
                    }
                    if ((cargaPedido != null) && cargaIntegracao.FecharCargaAutomaticamente && (mensagemErro.Length == 0) && permitirFecharCargaAutomaticamente)
                    {
                        if (configuracaoTMS.AgruparCargaAutomaticamente)
                            cargaPedido.Carga.AgruparCargaAutomaticamente = true;
                        else if (!configuracaoTMS.FecharCargaPorThread)
                        {
                            if (cargaPedido.Carga.CargaFechada &&
                                (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgIntegracao ||
                                 cargaPedido.Carga.SituacaoCarga == SituacaoCarga.EmTransporte || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Encerrada ||
                                 cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Cancelada || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Anulada ||
                                 cargaPedido.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos))
                            {
                                Servicos.Log.TratarErro("12 - Carga já estava fechada. Carga " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                            }
                            else
                            {
                                Servicos.Log.TratarErro("12 - Fechar Carga " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                                Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
                                {
                                    PermitirHorarioCarregamentoInferiorAoAtual = manterPreCargaComoCarga
                                };
                                servicoCarga.FecharCarga(cargaPedido.Carga, _unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, recriarRotas: false, adicionarJanelaDescarregamento: true, adicionarJanelaCarregamento: true, validarDados: false, gerarAgendamentoColeta: true, propriedades: propriedades, notificarAcompanhamento);
                                Servicos.Log.TratarErro("13 - Fechou Carga " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                                if (cargaPedido.Carga.CargaAgrupamento == null)
                                    cargaPedido.Carga.CargaFechada = true;
                                await repositorioCarga.AtualizarAsync(cargaPedido.Carga);
                            }
                        }
                        else
                            cargaPedido.Carga.FechandoCarga = true;
                        await repositorioCarga.AtualizarAsync(cargaPedido.Carga);
                        Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido.Carga, "Solicitou o fechamento da carga. Protocolo " + cargaPedido.Carga.Codigo.ToString(), _unitOfWork);
                    }
                    if (cargaPedido != null)
                        new Servicos.Embarcador.Logistica.AgendamentoColeta(_unitOfWork).AtualizarDataEntregaPorCargaPedido(cargaPedido);

                    if (configuracaoTMS.PermitirAtualizarModeloVeicularCargaDoVeiculoNoWebService && !string.IsNullOrWhiteSpace(cargaIntegracao.ModeloVeicular?.CodigoIntegracao) && mensagemErro.Length == 0)
                    {
                        Dominio.Entidades.Veiculo veiculo = cargaPedido?.Carga?.Veiculo;
                        if (veiculo != null && cargaIntegracao.ModeloVeicular?.CodigoIntegracao != veiculo.ModeloVeicularCarga?.CodigoIntegracao)
                        {
                            servicoVeiculo.AtualizarModeloVeicularDoVeiculoCarga(veiculo, cargaIntegracao.ModeloVeicular, ref mensagemErro, _unitOfWork);
                            Servicos.Log.TratarErro("14 - Atualizou Vínculo do MVC do veículo " + veiculo.Codigo + " para = " + cargaIntegracao.ModeloVeicular.CodigoIntegracao + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                        }
                    }
                }
                if (mensagemErro.Length > 0)
                {
                    Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} Retornou essa mensagem: {mensagemErro.ToString()}", "RequestLog");
                    _unitOfWork.Rollback();
                    if ((protocoloCargaExistente > 0 && protocoloPedidoExistente > 0) || (protocoloPedidoExistente > 0 && string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga) || (protocoloPedidoExistente > 0 && pedido == null && configuracaoTMS.RetornosDuplicidadeWSSubstituirPorSucesso)))
                    {
                        Servicos.Log.TratarErro($"protocoloCargaExistente: {protocoloCargaExistente} protocoloPedidoExistente: {protocoloPedidoExistente}", "RequestLog");
                        bool retornarDuplicidade = true;
                        if (configuracaoTMS.RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto)
                        {
                            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);
                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = await repCargaCancelamento.BuscarPorProtocoloCargaAsync(protocoloCargaExistente);
                            if (cargaCancelamento != null)
                                retornarDuplicidade = false;
                        }

                        if (protocoloPedidoExistente > 0)
                            codigoRasterio = (await repPedido.BuscarPorCodigoAsync(protocoloPedidoExistente))?.CodigoRastreamento ?? "";
                        if (!string.IsNullOrWhiteSpace(codigoRasterio))
                        {
                            AdminMultisoftware.Repositorio.UnitOfWork UnitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_adminStringConexao);
                            string urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(_clienteMultisoftware.Codigo, _tipoServicoMultisoftware, UnitOfWorkAdmin, _adminStringConexao, _unitOfWork);
                            linkRastreio = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(codigoRasterio, urlBase);
                            linkRastreioMapaEntrega = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentVisualizacaoMonitoramento(cargaPedido?.Carga.Codigo ?? 0, urlBase);
                        }

                        if (retornarDuplicidade)
                        {
                            if (configuracaoTMS.RetornosDuplicidadeWSSubstituirPorSucesso)
                            {
                                if (cargaIntegracao.FecharCargaAutomaticamente && configuracaoTMS.FecharCargaPorThread && !configuracaoTMS.AgruparCargaAutomaticamente && protocoloCargaExistente > 0)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.Carga cargaFechamento = await repositorioCarga.BuscarPorProtocoloAsync(protocoloCargaExistente);
                                    if (cargaFechamento != null && !cargaFechamento.CargaFechada)
                                    {
                                        cargaFechamento.FechandoCarga = true;
                                        await repositorioCarga.AtualizarAsync(cargaFechamento);
                                    }
                                }

                                stopWatch.Stop();
                                servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", protocoloCargaExistente, protocoloPedidoExistente, "TempoExecucao", stopWatch.Elapsed, true, "");

                                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = protocoloCargaExistente, protocoloIntegracaoPedido = protocoloPedidoExistente, ParametroIdentificacaoCliente = cargaIntegracao.ParametroIdentificacaoCliente, RastreamentoPedido = linkRastreio, RastreamentoMonitoramento = linkRastreioMapaEntrega }, mensagemErro.ToString());
                            }
                            else
                            {
                                AuditarRetornoDuplicidadeDaRequisicao(_unitOfWork, mensagemErro.ToString(), _auditado, cargaIntegracao.NumeroCarga);
                                servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracao, mensagemErro.ToString());

                                stopWatch.Stop();
                                servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", protocoloCargaExistente, protocoloPedidoExistente, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro.ToString());

                                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDuplicidadeRequisicao(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = protocoloCargaExistente, protocoloIntegracaoPedido = protocoloPedidoExistente, ParametroIdentificacaoCliente = cargaIntegracao.ParametroIdentificacaoCliente, RastreamentoPedido = linkRastreio, RastreamentoMonitoramento = linkRastreioMapaEntrega });
                            }
                        }
                        else
                        {
                            AuditarRetornoDadosInvalidos(_unitOfWork, mensagemErro.ToString(), _auditado, cargaIntegracao.NumeroCarga);
                            servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracao, mensagemErro.ToString());

                            stopWatch.Stop();
                            servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", protocoloCargaExistente, protocoloPedidoExistente, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro.ToString());

                            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                        }
                    }
                    else
                    {
                        AuditarRetornoDadosInvalidos(_unitOfWork, mensagemErro.ToString(), _auditado, cargaIntegracao.NumeroCarga);
                        servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracao, mensagemErro.ToString());

                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", protocoloCargaExistente, protocoloPedidoExistente, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro.ToString());

                        return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                    }
                }

                servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaComSucesso(cargaPedido);
                Servicos.Log.TratarErro("15 - Iniciou Commit " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                await _unitOfWork.CommitChangesAsync();
                Servicos.Log.TratarErro("16 - Finalizou Commit " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                Servicos.Embarcador.Integracao.Email.IntegracaoEmail.EnviarEmailDestiantarioTransportadorRetira(pedido, cargaPedido, "Novo pedido criado", _unitOfWork);

                if (cargaPedido != null && cargaPedido.Carga != null)
                    Servicos.Log.TratarErro($"AdicionarCarga Retorno: Protocolo carga = {cargaPedido.Carga.Codigo}, protocolo pedido = {pedido.Codigo}", "RequestLog");
                else if (pedido != null)
                {
                    Servicos.Log.TratarErro($"AdicionarCarga Retorno: Protocolo pedido = {pedido.Codigo}");

                    if (pedido.Protocolo == 0)
                        pedido.Protocolo = pedido.Codigo;
                }

                Servicos.Log.TratarErro("17 - Retornou protocolos " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                if (pedido != null && !string.IsNullOrEmpty(pedido.CodigoRastreamento))
                    codigoRasterio = pedido.CodigoRastreamento;
                else if (protocoloPedidoExistente > 0)
                    codigoRasterio = (await repPedido.BuscarPorCodigoAsync(protocoloPedidoExistente))?.CodigoRastreamento ?? "";

                if (!string.IsNullOrWhiteSpace(codigoRasterio))
                {
                    AdminMultisoftware.Repositorio.UnitOfWork UnitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_adminStringConexao);
                    string urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(_clienteMultisoftware.Codigo, _tipoServicoMultisoftware, UnitOfWorkAdmin, _adminStringConexao, _unitOfWork);
                    linkRastreio = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(codigoRasterio, urlBase);
                    if (cargaPedido != null)
                        linkRastreioMapaEntrega = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentVisualizacaoMonitoramento(cargaPedido.Carga.Codigo, urlBase);
                }

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                Servicos.Log.TratarErro($"AdicionarCarga - ProtocoloCarga {cargaPedido?.Carga?.Protocolo ?? 0} | Protocolo Pedido: {pedido?.Protocolo ?? 0} | Tempo total levado: {ts.ToString(@"mm\:ss\:fff")}", "TempoExecucao");

                if (protocoloPedidoExistente == 0 || (cargaPedido != null && protocoloCargaExistente == 0) || configuracaoTMS.RetornosDuplicidadeWSSubstituirPorSucesso)
                {
                    if ((cargaPedido?.Carga?.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.EnviarEmailAoCLienteComLinkDeAgendamentoQuandoGerarCarga ?? false))
                    {
                        StringBuilder mensagem = new StringBuilder(Localization.Resources.Cargas.Carga.EmailAoClienteComLinkDeAgendamentoQuandoGerarACarga);
                        mensagem.Replace("X", cargaPedido.ClienteEntrega.NomeCNPJ);
                        mensagem.Replace("Y", cargaPedido.Carga.Codigo.ToString());
                        mensagem.Replace("W", "");
                        mensagem.AppendLine("</br>");

                        Servicos.Email serEmail = new Servicos.Email(_unitOfWork);

                        serEmail.EnviarEmail(email.Email, email.Email, email.Senha, null, null, null, Localization.Resources.Cargas.Carga.SituacaoDaCarga, mensagem.ToString(), email.DisplayEmail);
                    }

                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo : 0, pedido?.Protocolo ?? 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo /*cargaPedido.Carga.Codigo*/ : 0, protocoloIntegracaoPedido = pedido?.Protocolo /*pedido?.Codigo*/ ?? 0, RastreamentoPedido = linkRastreio, RastreamentoMonitoramento = linkRastreioMapaEntrega });
                }
                else if (pedido != null && cargaPedido == null)
                {
                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo : 0, pedido?.Protocolo ?? 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo /*cargaPedido.Carga.Codigo*/ : 0, protocoloIntegracaoPedido = pedido?.Protocolo /*pedido?.Codigo*/ ?? 0, RastreamentoPedido = linkRastreio, RastreamentoMonitoramento = linkRastreioMapaEntrega });
                }
                else
                {
                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo : 0, pedido?.Protocolo ?? 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro.ToString());

                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDuplicidadeRequisicao(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo /*cargaPedido.Carga.Codigo*/ : 0, protocoloIntegracaoPedido = pedido?.Protocolo /*pedido?.Codigo*/ ?? 0, RastreamentoPedido = linkRastreio, RastreamentoMonitoramento = linkRastreioMapaEntrega });
                }
            }
            catch (BaseException excecao)
            {
                await _unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} Retornou essa mensagem: {excecao.Message}", "RequestLog");
                if (notificarAcompanhamento)
                    ArmazenarLogIntegracao(cargaIntegracao, _unitOfWork);

                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", cargaIntegracao.ProtocoloCarga, cargaIntegracao.ProtocoloPedido, "TempoExecucao", stopWatch.Elapsed, false, excecao.Message);

                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                await _unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} retornou exceção a seguir:", "RequestLog");
                if (notificarAcompanhamento)
                    ArmazenarLogIntegracao(cargaIntegracao, _unitOfWork);

                string msgErro = $"Ocorreu uma falha ao obter os dados das integrações. {mensagemErro.ToString()}";
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("AdicionarPedido", cargaIntegracao.ProtocoloCarga, cargaIntegracao.ProtocoloPedido, "TempoExecucao", stopWatch.Elapsed, false, msgErro);

                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoExcecao(msgErro);
            }
            finally
            {
                if (!naoFecharConexao)
                    await _unitOfWork.DisposeAsync();
            }
        }

        private void VincularCargaPreCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            if ((cargaPedido.Carga.CargaPreCarga != null) || (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) || !configuracaoEmbarcador.TrocarPreCargaPorCarga)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaPreCarga = repositorioCarga.BuscarPreCargaPorNumeroCargaVincularPreCarga(cargaPedido.Carga.CodigoCargaEmbarcador);

            if (cargaPreCarga == null)
                cargaPreCarga = repositorioCargaPedido.BuscarPreCargaPorNumeroCargaVincularPreCarga(cargaPedido.Pedido.NumeroPedidoEmbarcador);

            if (cargaPreCarga == null)
                return;

            cargaPedido.Carga.CargaPreCarga = cargaPreCarga;
            cargaPedido.Carga.PercentualSeparacaoMercadoria = cargaPreCarga.PercentualSeparacaoMercadoria;
            cargaPedido.Carga.SeparacaoMercadoriaConfirmada = cargaPreCarga.SeparacaoMercadoriaConfirmada;
            cargaPedido.Carga.DataInicioSeparacaoMercadoria = cargaPreCarga.DataInicioSeparacaoMercadoria;
            cargaPedido.Carga.DataAtualizacaoSeparacaoMercadoria = cargaPreCarga.DataAtualizacaoSeparacaoMercadoria;

            Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> lacresCargaPreCarga = repositorioCargaLacre.BuscarPorCarga(cargaPreCarga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaLacre lacreCargaPreCarga in lacresCargaPreCarga)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaLacre lacreCarga = new Dominio.Entidades.Embarcador.Cargas.CargaLacre
                {
                    Carga = cargaPedido.Carga,
                    Numero = lacreCargaPreCarga.Numero,
                    TipoLacre = lacreCargaPreCarga.TipoLacre,
                    Cliente = lacreCargaPreCarga.Cliente
                };

                repositorioCargaLacre.Inserir(lacreCarga);
            }
        }

        /// <summary>
        /// Se não tiver nenhum produto no pedido, cria uma MensagemAlerta avisando do caso.
        /// Essa MensagemAlerta também impede que a carga siga seu fluxo antes que as dependências sejam resolvidas.
        /// </summary>
        private void CriarDependenciaProdutoSeNecessario(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaIntegracao.Produtos?.Count > 0)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repositorioConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repositorioConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();

            if (!(configuracaoCargaIntegracao?.AceitarPedidosComPendenciasDeProdutos ?? false))
                return;

            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MensagemAlertaCarga mensagemAlerta = servicoMensagemAlerta.Adicionar(cargaPedido.Carga, TipoMensagemAlerta.CargaSemProdutos, $"O pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} não possui nenhum produto informado. Adicione pelo menos um produto na tela de Pedido");
            Servicos.Log.TratarErro($"11.1 - Não tinha nenhum produto. Criou a MensagemAlerta ({mensagemAlerta.Codigo})", "AdicionarCarga");
        }

        /// <summary>
        /// Se algum dos clientes estiver cadastrado com a localidade "não cadastrda" adiciona a pendencia
        /// Essa MensagemAlerta também impede que a carga siga seu fluxo antes que as dependências sejam resolvidas.
        /// </summary>
        private void CriarDependenciaPorCidadeNaoCadastrada(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            string codigoLocalidadeNaoCadastrada = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().CodigoLocalidadeNaoCadastrada;
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            Dominio.Entidades.Localidade localidadeNaoCadastrada = !string.IsNullOrWhiteSpace(codigoLocalidadeNaoCadastrada) ? repositorioLocalidade.BuscarPorCodigo(codigoLocalidadeNaoCadastrada.ToInt()) : null;

            bool possuiLocalidadeNaoCadastrada = (
                localidadeNaoCadastrada != null && (
                    cargaPedido.Pedido.Destinatario?.Localidade.Codigo == localidadeNaoCadastrada.Codigo ||
                    cargaPedido.Pedido.Remetente?.Localidade.Codigo == localidadeNaoCadastrada.Codigo ||
                    cargaPedido.Pedido.Recebedor?.Localidade.Codigo == localidadeNaoCadastrada.Codigo ||
                    cargaPedido.Pedido.Expedidor?.Localidade.Codigo == localidadeNaoCadastrada.Codigo
                )
            );

            if (!possuiLocalidadeNaoCadastrada)
                return;

            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork);
            string descricaoParticipante = string.Empty;

            if (cargaPedido.Pedido.Destinatario?.Localidade.Codigo == localidadeNaoCadastrada.Codigo)
                descricaoParticipante = $" do destinatário {cargaPedido.Pedido.Destinatario.CPF_CNPJ_Formatado}";
            else if (cargaPedido.Pedido.Remetente?.Localidade.Codigo == localidadeNaoCadastrada.Codigo)
                descricaoParticipante = $" do remetente {cargaPedido.Pedido.Remetente.CPF_CNPJ_Formatado}";
            else if (cargaPedido.Pedido.Expedidor?.Localidade.Codigo == localidadeNaoCadastrada.Codigo)
                descricaoParticipante = $" do expedidor {cargaPedido.Pedido.Expedidor.CPF_CNPJ_Formatado}";
            else if (cargaPedido.Pedido.Recebedor?.Localidade.Codigo == localidadeNaoCadastrada.Codigo)
                descricaoParticipante = $" do recebedor {cargaPedido.Pedido.Recebedor.CPF_CNPJ_Formatado}";

            string mensagem = $"Localidade{descricaoParticipante} não encontrada, atualize o cadastro do cliente para seguir.";
            Dominio.Entidades.Embarcador.Cargas.MensagemAlertaCarga mensagemAlerta = servicoMensagemAlerta.Adicionar(cargaPedido.Carga, TipoMensagemAlerta.ClienteSemLocalidade, mensagem);
            Servicos.Log.TratarErro($"11.1 - {mensagem}. Criou a MensagemAlerta ({mensagemAlerta.Codigo})", "AdicionarCarga");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AtualizarCargaPadrao(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, string StringConexao, bool notificarMobile = true)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Servicos.Global.TempoDeExecucao servicoTempoDeExecucao = new Global.TempoDeExecucao();

            Servicos.Log.TratarErro($"AtualizarCarga - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | cargaIntegracao {(cargaIntegracao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cargaIntegracao) : string.Empty)}", "Request");

            StringBuilder mensagemErro = new StringBuilder();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaowebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaowebService.BuscarConfiguracaoPadrao();

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork, configuracaoTMS);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(_unitOfWork, configuracaoTMS);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(_unitOfWork, configuracaoTMS);
            Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(_unitOfWork, configuracaoTMS);
            Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade servicoJanelaDescarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade(_unitOfWork, configuracaoTMS);

            if (protocolo == null || protocolo.protocoloIntegracaoPedido == 0)
            {
                mensagemErro.AppendLine("É obrigatório informar o protocolo do pedido.");
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("AtualizarCarga", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro.ToString());
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro.ToString());
            }

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (cargaIntegracao.TipoOperacao != null) ? repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracao.TipoOperacao.CodigoIntegracao) : null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao ?? "");

            _unitOfWork.Start();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

            if (protocolo.protocoloIntegracaoCarga > 0)
                carga = repCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);
            else
            {
                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);

                    if (listaCargaPedido != null && listaCargaPedido.Count > 0)
                        carga = listaCargaPedido.FirstOrDefault().Carga;
                }
                else
                    carga = repCargaPedido.BuscarCargaPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);
            }

            if (carga != null)
            {
                new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(_unitOfWork).ValidarPermissaoCancelarCarga(carga);

                //temp para validar logs arcelor mittal
                if (configuracaoTMS.IncluirCargaCanceladaProcessarDT)
                    Servicos.Log.TratarErro($"AtualizarCarga - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | Sitacao Carga Entrada: " + carga.SituacaoCarga.ObterDescricao(), "RequestLog");

                DateTime? previsaoEntregaAntesDaAtualizacao = DateTime.MinValue;
                if (!configuracaoTMS.UtilizaAppTrizy)
                {
                    // Achar o pedido que vai ser
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAntesAtualizacao = ObterPedidoPorProtocolo(protocolo, _unitOfWork);
                    previsaoEntregaAntesDaAtualizacao = pedidoAntesAtualizacao?.PrevisaoEntrega;
                }

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.AtualizarPedido(protocolo, filial, tipoOperacao, cargaIntegracao, ref mensagemErro, _tipoServicoMultisoftware, _auditado, configuracaoTMS.EncerrarMDFeAutomaticamente, configuracaoTMS, configuracaoWebService);
                serProdutoPedidoWS.AtualizarProdutosPedido(pedido, carga, cargaIntegracao, ref mensagemErro, configuracaoTMS, _unitOfWork, _auditado);
                carga.DataAtualizacaoCarga = DateTime.Now; //#37394

                if (cargaIntegracao.InformarSeparacaoPedido != null)
                    new Servicos.Embarcador.Pedido.Pedido().InformarSeparacaoPedido(pedido, cargaIntegracao.InformarSeparacaoPedido, _unitOfWork);

                if (mensagemErro.Length == 0)
                {
                    bool mudouLocalidade = false;

                    //serProdutoPedidoWS.AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref mensagemErro, _unitOfWork, _auditado);//já está sendo chamado no método anterior AtualizarProdutosPedido
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = AtualizarCarga(pedido, carga, tipoOperacao, cargaIntegracao, ref mensagemErro, ref mudouLocalidade, _tipoServicoMultisoftware, configuracaoTMS, configuracaoWebService, _unitOfWork, _auditado);

                    if (cargaPedido != null)
                    {
                        AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, _unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

                        if (cargaIntegracao.Transbordo != null && cargaIntegracao.Transbordo.Count > 0)
                            new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork).SalvarTransbordo(pedido, cargaIntegracao.Transbordo, ref mensagemErro, _unitOfWork, _unitOfWork.StringConexao, _auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);

                        if (cargaIntegracao.FecharCargaAutomaticamente)
                        {
                            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
                            serCarga.FecharCarga(cargaPedido.Carga, _unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, mudouLocalidade, viaWSAtualizarCarga: true, ip: _auditado.IP);

                            if ((cargaPedido.Carga.CargaAgrupamento == null) && (!cargaPedido.Carga.CargaDePreCarga || !repCarga.ExisteCargaFechadaPorPreCarga(cargaPedido.Carga.Codigo)))
                                cargaPedido.Carga.CargaFechada = true;

                            Servicos.Log.TratarErro("23 - Fechou Carga (" + cargaPedido.Carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");

                            cargaPedido.Carga.DataAtualizacaoCarga = DateTime.Now; //#37394
                            repCarga.Atualizar(cargaPedido.Carga);

                        }//cargas para arcelor nao podem entrar em calculo de frete
                        else if (cargaPedido.Carga.CargaFechada && !configuracaoTMS.IncluirCargaCanceladaProcessarDT)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(cargaPedido.Carga.Codigo);
                            if (cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga == SituacaoCarga.Nova &&
                                ((cargaPedido.Carga.NaoExigeVeiculoParaEmissao && !configuracaoTMS.UtilizaEmissaoMultimodal) || (cargaPedido.Carga.ModeloVeicularCarga != null && carga.Veiculo != null && cargaMotoristas != null && cargaMotoristas.Count > 0)))
                            {
                                Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(_unitOfWork, _tipoServicoMultisoftware);
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaPedido.Carga.Codigo);
                                serFrete.DefinirEtapaEFreteCargas(carga, cargaPedidos, _unitOfWork);
                            }
                            else if (!carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga == SituacaoCarga.AgTransportador && carga.TipoFreteEscolhido == TipoFreteEscolhido.Tabela)
                                carga.CalculandoFrete = true;
                        }

                        if (carga.ExigeNotaFiscalParaCalcularFrete && (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.AgNFe || carga.SituacaoCarga == SituacaoCarga.CalculoFrete) && !carga.CargaRotaFreteInformadaViaIntegracao)
                            carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;
                    }

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(carga, configuracaoTMS, _unitOfWork, DateTime.MinValue, _tipoServicoMultisoftware);
                    new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(_unitOfWork).AtualizarDadosSumarizadosCarregamentoPorPedidoAtualizado(pedido);
                    new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork).AlterarDadosSumarizadosCarga(ref carga, configuracaoTMS, _unitOfWork, _tipoServicoMultisoftware);

                    if (!string.IsNullOrWhiteSpace(pedido.NumeroEXP) && pedido.IsChangedByPropertyName("Origem"))
                    {
                        servicoCargaJanelaCarregamento.DesagendarCarga(carga);
                        Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, $"Carga desagendada via integração ao alterar a origem", _unitOfWork);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(pedido.CodigoInLand))
                            servicoCargaJanelaCarregamentoTransportador.DisponibilizarParaTransportadoresPorCargaComPedidoInLand(carga, _tipoServicoMultisoftware);

                        if (!cargaIntegracao.FecharCargaAutomaticamente)
                        {
                            if (pedido.IsChangedByPropertyName("Destinatario") || (cargaPedido?.IsChangedByPropertyName("Recebedor") ?? false))
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);
                                servicoJanelaDescarregamento.Atualizar(carga, cargaJanelaCarregamento);
                            }
                            else if (cargaPedido?.IsChangedByPropertyName("Peso") ?? false)
                            {
                                Dominio.Entidades.Cliente destinatario = cargaPedido.Recebedor ?? pedido.Destinatario;
                                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarAtivaPorCargaEDestinatario(carga.Codigo, destinatario.CPF_CNPJ);

                                if (cargaJanelaDescarregamento != null)
                                    servicoJanelaDescarregamentoDisponibilidade.AtualizarDefinicaoHorarioDescarregamento(cargaJanelaDescarregamento);
                            }
                        }
                    }
                }

                if (mensagemErro.Length > 0)
                {
                    Servicos.Log.TratarErro($"AtualizarCarga - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | Retorno: " + mensagemErro.ToString(), "RequestLog");
                    _unitOfWork.Rollback();
                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("AtualizarCarga", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro.ToString());
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro.ToString());
                }

                //temp para validar logs arcelor mittal
                if (configuracaoTMS.IncluirCargaCanceladaProcessarDT)
                    Servicos.Log.TratarErro($"AtualizarCarga - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | Sitacao Carga Saída: " + carga.SituacaoCarga.ObterDescricao(), "RequestLog");

                _unitOfWork.CommitChanges();
                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, "Atualizou a carga.", _unitOfWork);

                // Manda uma push pro MTrack avisando o motorista
                if (notificarMobile && !configuracaoTMS.UtilizaAppTrizy)
                    NotificarAtualizacaoCargaAoMotorista(carga, pedido, previsaoEntregaAntesDaAtualizacao, _unitOfWork);

                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("AtualizarCarga", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, true, "");

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }

            if (protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido > 0)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.AtualizarPedido(protocolo, filial, tipoOperacao, cargaIntegracao, ref mensagemErro, _tipoServicoMultisoftware, _auditado, configuracaoTMS.EncerrarMDFeAutomaticamente, configuracaoTMS, configuracaoWebService);

                if (mensagemErro.Length > 0)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro($"AtualizarCarga - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | Retorno: " + mensagemErro.ToString(), "RequestLog");

                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("AtualizarCarga", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro.ToString());

                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro.ToString());
                }

                serProdutoPedidoWS.AtualizarProdutosPedido(pedido, null, cargaIntegracao, ref mensagemErro, configuracaoTMS, _unitOfWork, _auditado);

                if (cargaIntegracao.Transbordo != null && cargaIntegracao.Transbordo.Count > 0)
                    new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork).SalvarTransbordo(pedido, cargaIntegracao.Transbordo, ref mensagemErro, _unitOfWork, _unitOfWork.StringConexao, _auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);

                if (mensagemErro.Length > 0)
                {
                    Servicos.Log.TratarErro($"AtualizarPedido - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | Retorno: " + mensagemErro.ToString(), "RequestLog");
                    _unitOfWork.Rollback();

                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("AtualizarCarga", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro.ToString());

                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro.ToString());
                }

                new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(_unitOfWork).AtualizarDadosSumarizadosCarregamentoPorPedidoAtualizado(pedido);

                _unitOfWork.CommitChanges();

                Servicos.Log.TratarErro($"AtualizarCarga - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | Sucesso atualizar pedido.", "RequestLog");
                Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, "Atualizou o pedido por WebService.", _unitOfWork);


                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                Servicos.Log.TratarErro($"AtualizarCarga - Protocolo Carga {(protocolo?.protocoloIntegracaoCarga ?? 0)} | Tempo total levado: {ts.ToString(@"mm\:ss\:fff")}", "TempoExecucao");

                servicoTempoDeExecucao.SalvarLogExecucao("AtualizarCarga", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, true, "");

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }

            _unitOfWork.Rollback();

            string msgErro = string.Empty;

            if (protocolo.protocoloIntegracaoCarga > 0)
            {
                Servicos.Log.TratarErro($"AtualizarCarga - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | Não foi localizada nenhuma carga para o protocolo informado " + protocolo.protocoloIntegracaoCarga, "RequestLog");

                msgErro = $"Não foi localizada nenhuma carga para o protocolo informado ({protocolo.protocoloIntegracaoCarga}).";
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("AtualizarCarga", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, msgErro);

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(msgErro);
            }

            Servicos.Log.TratarErro($"AtualizarCarga - Protocolo {(protocolo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(protocolo) : string.Empty)} | Não foi localizada nenhuma carga para o pedido protocolo informado " + protocolo.protocoloIntegracaoPedido, "RequestLog");

            msgErro = $"Não foi localizada nenhuma carga para o pedido protocolo informado ({protocolo.protocoloIntegracaoPedido}).";

            stopWatch.Stop();
            servicoTempoDeExecucao.SalvarLogExecucao("AtualizarCarga", protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido, "TempoExecucao", stopWatch.Elapsed, false, msgErro);

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(msgErro);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDaCarga(int protocoloIntegracaoCarga, string motivoDoCancelamento, string usuarioERPSolicitouCancelamento, string controleIntegracaoEmbarcador, bool duplicar)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Servicos.Global.TempoDeExecucao servicoTempoDeExecucao = new Global.TempoDeExecucao();

            Servicos.Log.TratarErro("SolicitarCancelamentoDaCarga - protocoloIntegracaoCarga: " + protocoloIntegracaoCarga.ToString(), "RequestLog");

            Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<RetornoSolicitacaoCancelamento>();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);
            Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(_unitOfWork);

            Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloIntegracaoCarga);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga?.Codigo ?? 0);

            string error = string.Empty;

            if (carga == null)
            {
                error = "Protocolo da carga " + protocoloIntegracaoCarga + " não encontrado . ";
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("SolicitarCancelamentoDaCarga", protocoloIntegracaoCarga, 0, "TempoExecucao", stopWatch.Elapsed, false, error);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(error, Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }

            error = serCarga.ValidarSeCargaEstaAptaParaCancelamento(carga, motivoDoCancelamento, _unitOfWork);

            if (!string.IsNullOrWhiteSpace(error))
            {
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("SolicitarCancelamentoDaCarga", protocoloIntegracaoCarga, 0, "TempoExecucao", stopWatch.Elapsed, false, error);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(error, Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
            {
                error = "Já foi solicitado o cancelamento da carga. ";
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("SolicitarCancelamentoDaCarga", protocoloIntegracaoCarga, 0, "TempoExecucao", stopWatch.Elapsed, false, error);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(error, Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }

            if ((configuracaoWebService?.NaoPermitirSolicitarCancelamentoCargaViaIntegracaoViagemIniciada ?? false) && carga.DataInicioViagem.HasValue)
            {
                error = "Ambiente configurado para não permitir cancelar cargas já iniciadas via integração. ";
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("SolicitarCancelamentoDaCarga", protocoloIntegracaoCarga, 0, "TempoExecucao", stopWatch.Elapsed, false, error);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(error, Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }

            bool podeCancelar = false;

            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova ||
                carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                configuracaoTMS.PermitirCancelamentoTotalCargaViaWebService)
                podeCancelar = true;
            else
                error = "Não é possível solicitar o cancelamento, pois sua atual situação não permite o mesmo (" + carga.DescricaoSituacaoCarga + "), se necessário solicite que o cancelamento seja feito por um usuário autorizado na base Multisoftware.";

            if (!podeCancelar)
            {
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("SolicitarCancelamentoDaCarga", protocoloIntegracaoCarga, 0, "TempoExecucao", stopWatch.Elapsed, false, error);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(error, Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }

            if (configuracaoFinanceiro.ValidarDataPrevisaoPagamentoEDataPagamentoNoCancelamentoDosCTes && serCarga.ValidaPagamentoCTes(carga, out List<int> numeroCtesComPagamento))
            {
                error = ($"Não é possível cancelar a carga, CT-e(s) {String.Join(", ", numeroCtesComPagamento)} possuem Data Previsão/Pagamento definida.");

                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("SolicitarCancelamentoDaCarga", protocoloIntegracaoCarga, 0, "TempoExecucao", stopWatch.Elapsed, false, error);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(error, Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }

            Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga pagamentoProvedorCarga = repositorioPagamentoCarga.BuscarPorCodigoCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = pagamentoProvedorCarga?.PagamentoProvedor ?? null;
            if (pagamentoProvedor != null)
            {
                if (pagamentoProvedor.EtapaLiberacaoPagamentoProvedor == EtapaLiberacaoPagamentoProvedor.Aprovacao || pagamentoProvedor.EtapaLiberacaoPagamentoProvedor == EtapaLiberacaoPagamentoProvedor.Liberacao)
                {
                    error = "Não é possível solicitar o cancelamento da carga pois a mesma já foi liberada para pagamento";

                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("SolicitarCancelamentoDaCarga", protocoloIntegracaoCarga, 0, "TempoExecucao", stopWatch.Elapsed, false, error);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(error, Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
                }
                else
                {
                    pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor = SituacaoLiberacaoPagamentoProvedor.Rejeitada;
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, pagamentoProvedor, pagamentoProvedor.GetChanges(), $"Fluxo cancelado devido a solicitação no método SolicitarCancelamentoDaCarga.", _unitOfWork);
                }
            }

            _unitOfWork.Start();

            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
            {
                Carga = carga,
                DefinirSituacaoEmCancelamento = true,
                DuplicarCarga = duplicar,
                MotivoCancelamento = motivoDoCancelamento,
                TipoServicoMultisoftware = _tipoServicoMultisoftware,
                UsuarioERPSolicitouCancelamento = usuarioERPSolicitouCancelamento,
                ControleIntegracaoEmbarcador = controleIntegracaoEmbarcador,
                GerarIntegracoes = true
            };

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, _unitOfWork);

            servicoCargaCancelamentoAprovacao.CriarAprovacao(cargaCancelamento, _tipoServicoMultisoftware);

            if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgAprovacaoSolicitacao)
            {
                repositorioCargaCancelamento.Atualizar(cargaCancelamento);
                retorno.Objeto = RetornoSolicitacaoCancelamento.AguardandoAprovacao;
            }
            else
            {
                Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware, false);

                if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                {
                    retorno.Mensagem = cargaCancelamento.MensagemRejeicaoCancelamento + " Codigo da Carga:" + carga.Codigo + " Codigo Carga Embarcador: " + carga.CodigoCargaEmbarcador;
                    Servicos.Log.TratarErro(cargaCancelamento.MensagemRejeicaoCancelamento + " Codigo da Carga:" + carga.Codigo + " Codigo Carga Embarcador: " + carga.CodigoCargaEmbarcador, "RequestLog");
                    retorno.Objeto = RetornoSolicitacaoCancelamento.CancelamentoRejeitado;
                    retorno.Status = true;
                }
                else if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.Cancelada)
                {
                    retorno.Objeto = RetornoSolicitacaoCancelamento.Cancelada;
                }
                else
                    retorno.Objeto = RetornoSolicitacaoCancelamento.EmCancelamento;
            }

            Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, "Solicitou o cancelamento da carga.", _unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido.Pedido, "Solicitou o cancelamento do pedido.", _unitOfWork);

            _unitOfWork.CommitChanges();

            stopWatch.Stop();
            servicoTempoDeExecucao.SalvarLogExecucao("SolicitarCancelamentoDaCarga", protocoloIntegracaoCarga, 0, "TempoExecucao", stopWatch.Elapsed, true, "");

            return Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento>.CriarRetornoSucesso(retorno.Objeto);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<RetornoSolicitacaoCancelamento> SolicitarCancelamentoDoPedido(int protocoloIntegracaoPedido, string motivoDoCancelamento)
        {
            string erro = string.Empty;

            try
            {
                _unitOfWork.Start();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCancelamento repPedidoCancelamento = new Repositorio.Embarcador.Pedidos.PedidoCancelamento(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);


                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(protocoloIntegracaoPedido);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorPedido(pedido.Codigo);

                if (pedido == null)
                    throw new ServicoException("Protocolo do pedido inexistente.");

                if (carga != null)
                    new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(_unitOfWork).ValidarPermissaoCancelarCarga(carga);

                if (configuracao.TrocarPreCargaPorCarga)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

                    if (repositorioCargaPedido.ExistePorPedidoPorProtocolo(protocoloIntegracaoPedido, true))
                        throw new ServicoException("O pedido já está vinculado à uma carga, não sendo possível realizar a exclusão do mesmo.");

                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);

                    if (repositorioCarregamentoPedido.ExisteCarregamentoAtivoPorPedidoPorProtocolo(protocoloIntegracaoPedido))
                        throw new ServicoException("O pedido já está vinculado à um carregamento, não sendo possível realizar a exclusão do mesmo.");
                }
                else
                    Servicos.Embarcador.Pedido.Pedido.RemoverPedidoCancelado(pedido, _unitOfWork, _tipoServicoMultisoftware, _auditado, configuracao, configuracaoGeralCarga);

                Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento pedidoCancelamento = repPedidoCancelamento.BuscarPorPedido(pedido.Codigo);

                if (pedidoCancelamento != null)
                {
                    Servicos.Log.TratarErro(erro, "RequestLog");
                    throw new ServicoException($"Já existe um cancelamento registrado para este pedido em {pedidoCancelamento.DataCancelamento.ToString("dd/MM/yyyy HH:mm:ss")}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);
                }

                if (!Servicos.Embarcador.Pedido.Pedido.CancelarPedido(out erro, pedido, TipoPedidoCancelamento.Cancelamento, null, motivoDoCancelamento, _unitOfWork, _tipoServicoMultisoftware, _auditado, configuracao, _clienteMultisoftware))
                    throw new ServicoException(erro);

                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<RetornoSolicitacaoCancelamento>.CriarRetornoSucesso(RetornoSolicitacaoCancelamento.Cancelada);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                    return Dominio.ObjetosDeValor.WebService.Retorno<RetornoSolicitacaoCancelamento>.CriarRetornoDuplicidadeRequisicao(excecao.Message, RetornoSolicitacaoCancelamento.CancelamentoRejeitado);

                return Dominio.ObjetosDeValor.WebService.Retorno<RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(excecao.Message, RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.WebService.Retorno<RetornoSolicitacaoCancelamento>.CriarRetornoExcecao("Ocorreu uma falha ao solicitar o cancelamento do pedido", RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<int> AgruparCargas(List<int> ProtocoloCargas)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                foreach (int protocolo in ProtocoloCargas)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocolo);
                    if (carga == null)
                        return Retorno<int>.CriarRetornoDadosInvalidos("Não foi localizada uma carga compativel para o protocolo informado (" + protocolo + ")");

                    cargas.Add(carga);
                }

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = new Servicos.Embarcador.Carga.CargaAgrupada(_unitOfWork).AgruparCargas(null, cargas, _tipoServicoMultisoftware, _clienteMultisoftware);

                if (cargaAgrupada == null)
                {
                    _unitOfWork.Rollback();
                    return Retorno<int>.CriarRetornoDadosInvalidos("Não é possível criar o agrupamento para as cargas informadas.");
                }

                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaAgrupada, null, "Criada pelo agrupamento automatico das cargas " + string.Join(", ", (from obj in cargaAgrupada.CodigosAgrupados select obj).ToList()), _unitOfWork);
                _unitOfWork.CommitChanges();

                return Retorno<int>.CriarRetornoSucesso(cargaAgrupada.Codigo);
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                _unitOfWork.Rollback();
                return Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao agrupar as cargas.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<int> AgruparCargasPorCodigoCargaEmbarcador(AgruparCargaPorCodigoCarga agruparCargaPorCodigoCarga)
        {
            try
            {
                if (agruparCargaPorCodigoCarga.CodigosCargaEmbarcador == null || agruparCargaPorCodigoCarga.CodigosCargaEmbarcador.Count <= 1)
                    return Retorno<int>.CriarRetornoDadosInvalidos("Pelo menos duas cargas devem ser enviadas para agrupamento");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                foreach (string codigoCargaEmbarcador in agruparCargaPorCodigoCarga.CodigosCargaEmbarcador)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoCargaEmbarcador(codigoCargaEmbarcador);
                    if (carga == null)
                        return Retorno<int>.CriarRetornoDadosInvalidos("Não foi localizada uma carga compativel para o código informado (" + codigoCargaEmbarcador + ")");

                    cargas.Add(carga);
                }

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = new Servicos.Embarcador.Carga.CargaAgrupada(_unitOfWork).AgruparCargas(cargas, null, agruparCargaPorCodigoCarga.CodigoCargaAgrupada ?? "", null, null, _tipoServicoMultisoftware, _clienteMultisoftware);

                if (cargaAgrupada == null)
                {
                    _unitOfWork.Rollback();
                    return Retorno<int>.CriarRetornoDadosInvalidos("Não é possível criar o agrupamento para as cargas informadas.");
                }

                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaAgrupada, null, "Criada pelo agrupamento de cargas por Codigo Embarcador " + string.Join(", ", (from obj in cargaAgrupada.CodigosAgrupados select obj).ToList()), _unitOfWork);
                _unitOfWork.CommitChanges();

                return Retorno<int>.CriarRetornoSucesso(cargaAgrupada.Codigo);
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                _unitOfWork.Rollback();
                return Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao agrupar as cargas.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<int> AdicionarCargaCompleta(CargaIntegracaoCompleta cargaIntegracaoCompleta)
        {
            Servicos.Log.TratarErro($"CargaIntegracaoCompleta: {(cargaIntegracaoCompleta != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cargaIntegracaoCompleta) : string.Empty)}", "Request");

            Servicos.WebService.Carga.Pedido servicoPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            string existeCampoObrigratorioVazio = servicoCarga.ValidacaoInicialDosCamposObrigratorios(cargaIntegracaoCompleta, _unitOfWork);

            if (!string.IsNullOrEmpty(existeCampoObrigratorioVazio))
                return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoDadosInvalidos(existeCampoObrigratorioVazio, 0);

            StringBuilder mensagemErro = new StringBuilder();

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repsitorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

                int protocoloCargaExistente = 0;
                int protocoloPedidoExistente = 0;

                cargaIntegracaoCompleta.FecharCargaAutomaticamente = false;

                if (!string.IsNullOrWhiteSpace(cargaIntegracaoCompleta.NumeroCarga) && repositorioCarga.ContemCargaDuplicadaNumeracao(cargaIntegracaoCompleta.NumeroCarga, true))
                    return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoSucesso(repositorioCarga.CodigoCargaDuplicadaNumeracao(cargaIntegracaoCompleta.NumeroCarga));
                else if (!string.IsNullOrWhiteSpace(cargaIntegracaoCompleta.NumeroCarga) && repositorioCarga.ContemCargaDuplicadaNumeracao(cargaIntegracaoCompleta.NumeroCarga, false))
                    cargaIntegracaoCompleta.NumeroCarga = cargaIntegracaoCompleta.NumeroCarga + "-I";

                foreach (Dominio.ObjetosDeValor.WebService.Carga.Pedido pedidoIntegracao in cargaIntegracaoCompleta.Pedidos)
                {
                    if (pedidoIntegracao == null)
                        continue;

                    Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = Servicos.Embarcador.Carga.CargaIntegracao.ConverterCargaEmCargaIntegracao(pedidoIntegracao, cargaIntegracaoCompleta, _unitOfWork);

                    if (cargaIntegracao.TipoOperacao != null && !string.IsNullOrWhiteSpace(cargaIntegracao.TipoOperacao.CodigoIntegracao))
                        tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracao.TipoOperacao.CodigoIntegracao);

                    if (cargaIntegracao.Filial != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Filial.CodigoIntegracao))
                        filial = repsitorioFilial.BuscarPorCodigoIntegracao(cargaIntegracao.Filial.CodigoIntegracao);

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref mensagemErro, _tipoServicoMultisoftware, ref protocoloPedidoExistente, ref protocoloCargaExistente, false, _auditado, configuracaoTMS, _clienteURLAcesso, _adminStringConexao, ignorarPedidosInseridosManualmente: true, true);
                    if (mensagemErro.Length == 0)
                    {
                        new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork).AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref mensagemErro, _unitOfWork, _auditado);
                        if (cargaIntegracao.Transbordo != null && cargaIntegracao.Transbordo.Count > 0)
                            new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork).SalvarTransbordo(pedido, cargaIntegracao.Transbordo, ref mensagemErro, _unitOfWork, _unitOfWork.StringConexao, _auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);

                        cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemErro, ref protocoloCargaExistente, _unitOfWork, _tipoServicoMultisoftware, false, false, _auditado, configuracaoTMS, _clienteURLAcesso, _adminStringConexao, filial, tipoOperacao, false, true);
                        if (cargaPedido != null)
                        {
                            servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, _unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);
                            CriarDependenciaProdutoSeNecessario(cargaIntegracao, cargaPedido, _unitOfWork);
                            CriarDependenciaPorCidadeNaoCadastrada(cargaIntegracao, cargaPedido, _unitOfWork);
                        }
                    }
                    else
                    {
                        _unitOfWork.Rollback();
                        return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), 0);
                    }

                }

                if (cargaPedido == null || mensagemErro.Length > 0)
                {
                    _unitOfWork.Rollback();
                    return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), 0);
                }

                cargaIntegracaoCompleta.FecharCargaAutomaticamente = true;
                if (cargaPedido != null && cargaIntegracaoCompleta.FecharCargaAutomaticamente.Value && (mensagemErro.Length == 0))
                {
                    if (configuracaoTMS.AgruparCargaAutomaticamente)
                        cargaPedido.Carga.AgruparCargaAutomaticamente = true;
                    else if (!configuracaoTMS.FecharCargaPorThread)
                    {
                        if (cargaPedido.Carga.CargaFechada &&
                            (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgIntegracao ||
                             cargaPedido.Carga.SituacaoCarga == SituacaoCarga.EmTransporte || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Encerrada ||
                             cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Cancelada || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Anulada ||
                             cargaPedido.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos))
                        {
                            Servicos.Log.TratarErro("12 - Carga já estava fechada. Carga " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                        }
                        else
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
                            {
                                PermitirHorarioCarregamentoInferiorAoAtual = false
                            };
                            servicoCarga.FecharCarga(cargaPedido.Carga, _unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, recriarRotas: false, adicionarJanelaDescarregamento: true, adicionarJanelaCarregamento: true, validarDados: false, gerarAgendamentoColeta: true, propriedades: propriedades);
                            if (cargaPedido.Carga.CargaAgrupamento == null)
                                cargaPedido.Carga.CargaFechada = true;
                            repositorioCarga.Atualizar(cargaPedido.Carga);
                        }
                    }
                    else
                        cargaPedido.Carga.FechandoCarga = true;

                    cargaPedido.Carga.CargaRecebidaDeIntegracao = true;
                    repositorioCarga.Atualizar(cargaPedido.Carga);
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPedido.Carga, "Solicitou o fechamento da carga. Protocolo " + cargaPedido.Carga.Codigo.ToString(), _unitOfWork);
                }
                if (cargaPedido != null)
                    new Servicos.Embarcador.Logistica.AgendamentoColeta(_unitOfWork).AtualizarDataEntregaPorCargaPedido(cargaPedido);

                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoSucesso(cargaPedido.Carga.Codigo);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro($"Carga: {cargaIntegracaoCompleta.NumeroCarga} Retornou essa mensagem: {excecao.Message}", "RequestLog");
                ArmazenarLogIntegracao(cargaIntegracaoCompleta, _unitOfWork);
                return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro($"Carga: {cargaIntegracaoCompleta.NumeroCarga} retornou exceção a seguir:", "RequestLog");
                ArmazenarLogIntegracao(cargaIntegracaoCompleta, _unitOfWork);
                return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoExcecao($"Ocorreu uma falha ao obter os dados das integrações. {mensagemErro.ToString()}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarFrete(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            if (protocolo == null || protocolo.protocoloIntegracaoCarga == 0)
                throw new ServicoException("Codigo de integração da carga não informado");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Servicos.Global.TempoDeExecucao servicoTempoDeExecucao = new Global.TempoDeExecucao();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocolo.protocoloIntegracaoCarga);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorUnicaCarga(protocolo.protocoloIntegracaoCarga);

            string mensagemErro = string.Empty;

            if (carga == null)
            {
                mensagemErro = "Carga não encontrada";
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("ConfirmarFrete", 0, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);
            }

            if (servicoCarga.ValidacaoConfiguracaoCargaFrete(carga, _unitOfWork))
                if (servicoCarga.ProcessarIntegracaoCargaFretePendentes(carga, _unitOfWork))
                {
                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("ConfirmarFrete", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Integração de enviadas para processamento");
                }

            List<CargaFreteIntegracao> possuiIntegracao = repositorioCargaFreteIntegracao.ExisteIntegracoesCargaFreteParaEstaCarga(carga.Codigo);
            if ((carga.AguardandoIntegracaoFrete || carga.PendenciaIntegracaoFrete || (possuiIntegracao != null && possuiIntegracao.Count > 0)) && !((carga?.LiberadaComPendenciaIntegracaoFrete) ?? false))
            {
                mensagemErro = "Não pode ser avançada a etapa porque tem integrações pendentes ou com falhas.";
                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("ConfirmarFrete", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);
            }

            string mensagem = string.Empty;

            if (fluxoGestaoPatio != null)
                fluxoGestaoPatio.EmissaoCargaLiberada = false;

            if (fluxoGestaoPatio != null && fluxoGestaoPatio.DataInicioViagem.HasValue)
            {
                if (carga.SituacaoCarga != SituacaoCarga.CalculoFrete && (!carga.PossuiPendencia || configuracaoEmbarcador.IncluirCargaCanceladaProcessarDT))
                {
                    //mensagem = "Carga está numa situação que não é possivel avançar";
                    carga.LiberadaEmissaoERP = true;
                    repCarga.Atualizar(carga);

                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("ConfirmarFrete", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
                }
                else
                {
                    if (carga.CalculandoFrete && configuracaoEmbarcador.IncluirCargaCanceladaProcessarDT)
                    {
                        carga.LiberadaEmissaoERP = true;
                        repCarga.Atualizar(carga);

                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("ConfirmarFrete", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, fluxoGestaoPatio, null, "Confirmou via integração.", _unitOfWork);

                    if (!servicoCarga.LiberarEtapaEmisao(carga, _unitOfWork, _auditado, _tipoServicoMultisoftware, _clienteURLAcesso?.WebServiceConsultaCTe ?? "", out mensagem))
                    {
                        mensagemErro = "Carga não encontrada";
                        stopWatch.Stop();
                        servicoTempoDeExecucao.SalvarLogExecucao("ConfirmarFrete", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagemErro);

                        return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagem);
                    }
                }
            }
            else if (fluxoGestaoPatio != null)
            {
                fluxoGestaoPatio.EmissaoCargaLiberada = true;
                repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, fluxoGestaoPatio, null, "Confirmou via integração.", _unitOfWork);

                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("ConfirmarFrete", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }

            if (carga.SituacaoCarga != SituacaoCarga.CalculoFrete && (!carga.PossuiPendencia || configuracaoEmbarcador.IncluirCargaCanceladaProcessarDT))
            {
                //mensagem = "Carga está numa situação que não é possivel avançar";
                carga.LiberadaEmissaoERP = true;
                repCarga.Atualizar(carga);

                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("ConfirmarFrete", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }
            else
            {
                if (!servicoCarga.LiberarEtapaEmisao(carga, _unitOfWork, _auditado, _tipoServicoMultisoftware, _clienteURLAcesso?.WebServiceConsultaCTe ?? "", out mensagem))
                {
                    stopWatch.Stop();
                    servicoTempoDeExecucao.SalvarLogExecucao("ConfirmarFrete", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, false, mensagem);

                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagem);
                }

                stopWatch.Stop();
                servicoTempoDeExecucao.SalvarLogExecucao("ConfirmarFrete", carga.Protocolo, 0, "TempoExecucao", stopWatch.Elapsed, true, "");

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }

        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornarEtapaNota(int protocoloCarga)
        {
            try
            {
                if (protocoloCarga == 0)
                    throw new ServicoException("Protocolo da carga não é inválido.");

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

                dynamic retornarEtapaNota = servicoCarga.RetornarEtapa(protocoloCarga, _unitOfWork, _tipoServicoMultisoftware, _auditado);

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message);
            }
        }

        public Retorno<bool> AlterarNumeroCarga(AlterarNumeroCarga alterarNumeroCarga)
        {
            try
            {
                if (alterarNumeroCarga == null)
                    throw new ServicoException("Request inválido.");

                if (string.IsNullOrWhiteSpace(alterarNumeroCarga.numeroCarga))
                    throw new ServicoException("O número da carga deve ser informado.");

                if (alterarNumeroCarga.protocoloCarga <= 0)
                    throw new ServicoException("O protocolo da carga deve ser informado.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(alterarNumeroCarga.protocoloCarga);

                if (carga == null)
                    throw new ServicoException("Não foi encontrado uma carga para o protocolo informado");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();

                string numeroCargaAnterior = carga.CodigoCargaEmbarcador;
                string numeroCargaNovo = Utilidades.String.Left(alterarNumeroCarga.numeroCarga.Trim(), 50);

                if (carga.CargaDePreCarga || configuracaoEmbarcador.Pais == TipoPais.Exterior || configuracaoWebService.PermitirAlterarNumeroCargaQuandoForCarga)
                {
                    carga.CodigoCargaEmbarcador = numeroCargaNovo;

                    if (carga.Carregamento != null && configuracaoEmbarcador.Pais == TipoPais.Exterior)
                        carga.Carregamento.NumeroCarregamento = numeroCargaNovo;

                    repositorioCarga.Atualizar(carga);

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, string.Format(Localization.Resources.Cargas.Carga.AlterouNumeroDaCargaEmbarcador, numeroCargaAnterior, numeroCargaNovo), _unitOfWork);
                }
                else
                {
                    carga.CargaPossuiOutrosNumerosEmbarcador = true;
                    carga.CodigosAgrupados ??= new List<string>();

                    if (!carga.CodigosAgrupados.Contains(numeroCargaNovo))
                        carga.CodigosAgrupados.Add(numeroCargaNovo);

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, $"Adicionou o número da carga embarcador {numeroCargaNovo}.", _unitOfWork);
                }

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao alterar o número da carga");
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<int> ConsultarCargaPorNumeroCarregamento(string pedidoNumeroCarregamento)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if (string.IsNullOrWhiteSpace(pedidoNumeroCarregamento))
                return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoDadosInvalidos("Necessário informar o número de carregamento do pedido.");

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPedidoPorPedidoNumeroCarregamento(pedidoNumeroCarregamento);

            if (pedido == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoDadosInvalidos("Não foi possível encontrar nenhum pedido com número de carregamento informado.");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorPedido(pedido.Codigo);

            if (carga == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoDadosInvalidos("Não foi possível encontrar nenhuma carga para o número de carregamento informado.");

            return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoSucesso(carga.Protocolo);
        }

        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote>> EnviarPacoteAsync(Dominio.ObjetosDeValor.WebService.Rest.Pedidos.Pacote pacote)
        {
            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Cargas.Pacote repositorioPacote = new Repositorio.Embarcador.Cargas.Pacote(_unitOfWork, _cancellationToken);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, _cancellationToken);

                Servicos.Embarcador.Pacote.Pacote servicoPacote = new Servicos.Embarcador.Pacote.Pacote(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken);

                double cpfCnpjOrigem = Utilidades.Validate.ValidarCPFCNPJ(pacote.Origem) ? pacote.Origem.ObterSomenteNumeros().ToDouble() : 0d;
                double cpfCnpjDestino = Utilidades.Validate.ValidarCPFCNPJ(pacote.Destino) ? pacote.Destino.ObterSomenteNumeros().ToDouble() : 0d;
                double cpfCnpjContratante = Utilidades.Validate.ValidarCPFCNPJ(pacote.Contratante) ? pacote.Contratante.ObterSomenteNumeros().ToDouble() : 0d;

                Dominio.Entidades.Cliente origem = null;
                Dominio.Entidades.Cliente contratante = null;
                Dominio.Entidades.Cliente destino = null;

                if (cpfCnpjOrigem > 0)
                    origem = await repositorioCliente.BuscarPorCPFCNPJSemFetchAsync(cpfCnpjOrigem, _cancellationToken);
                else if (!string.IsNullOrWhiteSpace(pacote.Origem))
                    origem = await repositorioCliente.BuscarPorCodigoIntegracaoAsync(pacote.Origem ?? string.Empty, _cancellationToken);

                if (cpfCnpjDestino > 0)
                    destino = await repositorioCliente.BuscarPorCPFCNPJSemFetchAsync(cpfCnpjDestino, _cancellationToken);
                else if (!string.IsNullOrWhiteSpace(pacote.Destino))
                    destino = await repositorioCliente.BuscarPorCodigoIntegracaoAsync(pacote.Destino ?? string.Empty, _cancellationToken);

                if (cpfCnpjContratante > 0)
                    contratante = await repositorioCliente.BuscarPorCPFCNPJSemFetchAsync(cpfCnpjContratante, _cancellationToken);
                else if (!string.IsNullOrWhiteSpace(pacote.Contratante))
                    contratante = await repositorioCliente.BuscarPorCodigoIntegracaoAsync(pacote.Contratante ?? string.Empty, _cancellationToken);

                if (await Servicos.Embarcador.Carga.Pacote.PacoteIdentificador.GetInstance().ExistePacoteIgualAsync(_unitOfWork, pacote.Loggi_key, _cancellationToken))
                    throw new ServicoException("Pacote já foi registrado no sistema", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);

                await _unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Cargas.Pacote pacoteAdicionar = new Dominio.Entidades.Embarcador.Cargas.Pacote()
                {
                    Origem = origem ?? contratante,
                    Destino = destino,
                    Contratante = contratante,
                    CodigoIntegracaoOrigem = pacote.Origem,
                    CodigoIntegracaoDestino = pacote.Destino,
                    CodigoIntegracaoContratante = pacote.Contratante,
                    DataRecebimento = pacote.Data_confirmacao,
                    LogKey = pacote.Loggi_key,
                    Cubagem = pacote.Cubagem,
                    Peso = pacote.Peso
                };

                await repositorioPacote.InserirAsync(new List<Pacote>() { pacoteAdicionar }, "T_PACOTE");

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarCargaPedidoCompativelComPacoteAsync(origem?.CPF_CNPJ ?? contratante?.CPF_CNPJ ?? 0d, destino?.CPF_CNPJ ?? 0d, contratante?.CPF_CNPJ ?? 0d, _cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote()
                {
                    Pacote = pacoteAdicionar,
                    CargaPedido = cargaPedido
                };

                await repositorioCargaPedidoPacote.InserirAsync(new List<CargaPedidoPacote>() { cargaPedidoPacote }, "T_CARGA_PEDIDO_PACOTE");

                if (cargaPedidoPacote.CargaPedido != null)
                {
                    Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork, _cancellationToken);
                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiro = await repositorioCTeTerceiro.BuscarPorIdentificacaoPacoteAsync(pacote.Loggi_key, _cancellationToken);

                    cargaPedido.Pedido.Initialize();
                    cargaPedido.Carga.Initialize();

                    foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesTerceiro)
                    {
                        string retorno = await servicoPacote.VincularCTeCargaPedidoPacoteAsync(cargaPedidoPacote, cteTerceiro);

                        cteTerceiro.Initialize();

                        if (!string.IsNullOrWhiteSpace(retorno))
                            throw new ServicoException(retorno);
                    }

                    await servicoPacote.VerificarQuantidadePacotesCtesAvancaAutomaticoAsync(cargaPedidoPacote.CargaPedido.Carga, _auditado, _cancellationToken);

                    repositorioCTeTerceiro.AtualizarSomenteCamposAlterados(ctesTerceiro);
                    repositorioPedido.AtualizarSomenteCamposAlterados(new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { cargaPedido.Pedido });
                    repositorioCarga.AtualizarSomenteCamposAlterados(new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { cargaPedido.Carga });
                }

                await Servicos.Auditoria.Auditoria.AuditarAsync(_auditado, cargaPedidoPacote.Pacote, null, "Gerado por integração", _unitOfWork, cancellationToken: _cancellationToken);

                _unitOfWork.CommitChanges();

                Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote retornoPacote = new Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote()
                {
                    ProtocoloPedido = cargaPedidoPacote.CargaPedido?.Pedido.Protocolo ?? 0,
                    ProtocoloCarga = cargaPedidoPacote.CargaPedido?.Carga.Protocolo ?? 0
                };

                string mensagem = (cargaPedidoPacote.CargaPedido != null) ? "Pacote criado e vinculado a uma Carga/Pedido com sucesso" : "Pacote avulso criado com sucesso";

                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote>.CriarRetornoSucesso(retornoPacote, mensagem);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote>.CriarRetornoDuplicidadeRequisicao(excecao.Message);

                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();

                if ((excecao.InnerException != null) && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 80000)
                        return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote>.CriarRetornoDuplicidadeRequisicao(excecaoSql.Message);
                }

                Servicos.Log.TratarErro(excecao);
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Rest.Pedidos.RetornoPacote>.CriarRetornoExcecao("Ocorreu uma falha ao receber o pacote");
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> IntegrarAE(IntegracaoCargaAE cargaAE)
        {
            try
            {
                if (cargaAE.ProtocoloCarga == 0)
                    throw new WebServiceException("O protocolo da carga deve ser informado.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(cargaAE.ProtocoloCarga);

                if (carga == null)
                    throw new WebServiceException("Carga não encontrada. Favor verificar número do protocolo.");

                carga.SituacaoAE = (SituacaoAE)cargaAE.CodigoSituacaoAE;
                carga.DescricaoSituacaoAE = cargaAE.DescricaoSituacaoAE;
                carga.MotivoSituacaoAE = cargaAE.MotivoSituacaoAE;

                repositorioCarga.Atualizar(carga);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao integrar a AE da carga");
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AtualizarDadosPagamentoProvedor(DadosPagamentoProvedor dadosPagamento)
        {
            try
            {
                if (dadosPagamento.ProtocoloCarga == 0)
                    throw new WebServiceException("O protocolo da carga deve ser informado.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedor repositorioPagamento = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(_unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repositorioPagamentoCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(dadosPagamento.ProtocoloCarga);

                if (carga == null)
                    throw new WebServiceException("Carga não encontrada. Favor verificar número do protocolo.");

                List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> pagamentosProvedorCarga = repositorioPagamentoCarga.BuscarProvedoresPorCodigoCarga(carga.Codigo);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor pagamentoProvedor = repositorioPagamento.ObterPagamentoProvedor(pagamentosProvedorCarga);
                if (pagamentoProvedor != null)
                    pagamentoProvedor.Initialize();

                if (pagamentoProvedor != null && (pagamentoProvedor.EtapaLiberacaoPagamentoProvedor == EtapaLiberacaoPagamentoProvedor.Aprovacao || pagamentoProvedor.EtapaLiberacaoPagamentoProvedor == EtapaLiberacaoPagamentoProvedor.Liberacao))
                    throw new WebServiceException("Não é possível solicitar a alteração pois já foi liberado para pagamento.");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPrimeiroPorCarga(carga.Codigo);

                pedido.ValorTotalProvedor = dadosPagamento.ValorTotalProvedor;
                pedido.IndicLiberacaoOk = dadosPagamento.IndicLiberacaoOk;
                repositorioPedido.Atualizar(pedido);

                carga.ValorTotalProvedor = dadosPagamento.ValorTotalProvedor;
                carga.IndicLiberacaoOk = dadosPagamento.IndicLiberacaoOk;
                repositorioCarga.Atualizar(carga);

                if (pagamentoProvedor != null && pagamentoProvedor.EtapaLiberacaoPagamentoProvedor == EtapaLiberacaoPagamentoProvedor.DocumentoProvedor)
                {
                    if (dadosPagamento.IndicLiberacaoOk == false)
                    {
                        pagamentoProvedor.SituacaoLiberacaoPagamentoProvedor = SituacaoLiberacaoPagamentoProvedor.Rejeitada;

                        Servicos.Auditoria.Auditoria.Auditar(_auditado, pagamentoProvedor, pagamentoProvedor.GetChanges(), $"Fluxo rejeitado devido ao recebimento de uma atualização como False no método AtualizarDadosPagamentoProvedor.", _unitOfWork);
                    }
                    else if (dadosPagamento.IndicLiberacaoOk == true)
                    {
                        carga.LiberarPagamento = true;
                        repositorioCarga.Atualizar(carga);

                        Servicos.Auditoria.Auditoria.Auditar(_auditado, pagamentoProvedor, pagamentoProvedor.GetChanges(), $"Fluxo reiniciado devido ao recebimento de uma atualização como True no método AtualizarDadosPagamentoProvedor.", _unitOfWork);
                    }
                }

                if (pagamentoProvedor != null)
                    repositorioPagamento.Atualizar(pagamentoProvedor);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                Servicos.Log.TratarErro(excecao, "RequestLog");
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar os dados de pagamento do provedor.");
            }
        }

        public Retorno<bool> RetornoConfirmacaoColeta(Dominio.ObjetosDeValor.WebService.Carga.RetornoConfirmacaoColeta retornoConfirmacaoColeta)
        {
            if (string.IsNullOrEmpty(retornoConfirmacaoColeta.NumeroDaCarga))
                return Retorno<bool>.CriarRetornoDadosInvalidos("Número da Carga é obrigatório!");

            Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.FiltroPesquisaGestaoDadosColeta filtroPesquisaGestaoDadosColeta = new Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.FiltroPesquisaGestaoDadosColeta()
            {
                CodigoCargaEmbarcador = retornoConfirmacaoColeta.NumeroDaCarga
            };
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta repositorioGestaoDadosColeta = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta(_unitOfWork);
            // TODO: ToList não contem exist, cast
            List<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDadosColeta.GestaoDadosColeta> listaGestaoDadosColeta = repositorioGestaoDadosColeta.Consultar(filtroPesquisaGestaoDadosColeta, parametrosConsulta).ToList();

            if (listaGestaoDadosColeta == null || listaGestaoDadosColeta.Count == 0 || !listaGestaoDadosColeta.Exists(gestao => gestao.Tipo == TipoGestaoDadosColeta.DadosNfe))
                return Retorno<bool>.CriarRetornoDadosInvalidos("Dados de Gestão da Coleta não encontrada para carga!");

            Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta gestaoDadosColeta = repositorioGestaoDadosColeta.BuscarPorCodigo(listaGestaoDadosColeta.Find(gestao => gestao.Tipo == TipoGestaoDadosColeta.DadosNfe)?.Codigo ?? 0, false);
            if (gestaoDadosColeta == null)
                return Retorno<bool>.CriarRetornoDadosInvalidos("Dados de Gestão da Coleta não encontrada!");

            DateTime? dataRetornoConfirmacaoColeta = null;

            if (!string.IsNullOrWhiteSpace(retornoConfirmacaoColeta.DataConfirmacao) && !string.IsNullOrEmpty(retornoConfirmacaoColeta.HoraConfirmacao))
                dataRetornoConfirmacaoColeta = DateTime.ParseExact(retornoConfirmacaoColeta.DataConfirmacao + " " + retornoConfirmacaoColeta.HoraConfirmacao, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture);

            if (dataRetornoConfirmacaoColeta.HasValue)
                gestaoDadosColeta.DataRetornoConfirmacaoColeta = dataRetornoConfirmacaoColeta.Value;

            if (!string.IsNullOrEmpty(retornoConfirmacaoColeta.ErroConfirmacao))
                gestaoDadosColeta.ErroRetornoConfirmacaoColeta = retornoConfirmacaoColeta.ErroConfirmacao;

            if (!string.IsNullOrEmpty(retornoConfirmacaoColeta.IdExterno))
                gestaoDadosColeta.IdExternoRetornoConfirmacaoColeta = retornoConfirmacaoColeta.IdExterno;

            if (!string.IsNullOrEmpty(retornoConfirmacaoColeta.Operacao))
                gestaoDadosColeta.OperacaoRetornoConfirmacaoColeta = retornoConfirmacaoColeta.Operacao;

            repositorioGestaoDadosColeta.Atualizar(gestaoDadosColeta, _auditado);

            return Retorno<bool>.CriarRetornoSucesso(true, "Retorno de confirmação de coleta recebida com sucesso.");
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> ObterProdutoDaCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if (cargaPedido == null)
                return null;

            Servicos.WebService.Carga.ProdutosPedido servicoProduto = new ProdutosPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listCargaPedidoProduto = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido?.Codigo ?? 0);

            if (listCargaPedidoProduto == null || listCargaPedidoProduto.Count == 0)
                return new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();

            List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> listaProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>() { };

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in listCargaPedidoProduto)
                listaProdutos.Add(servicoProduto.ConveterObjetoProduto(cargaPedidoProduto));

            return listaProdutos;
        }

        /// <summary>
        /// Notifica o motorista no app MTrack da mudança em sua carga
        /// </summary>
        private void NotificarAtualizacaoCargaAoMotorista(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, DateTime? previsaoEntregaAntesDaAtualizacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga == null)
            {
                return;
            }

            Embarcador.Notificacao.NotificacaoMTrack serNotificacaoMTrack = new Servicos.Embarcador.Notificacao.NotificacaoMTrack(unitOfWork);

            if (pedido == null) // Se o pedido for null, não temos como saber qual foi a CargaEntrega atualizada
            {
                serNotificacaoMTrack.NotificarMudancaCarga(carga, carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.CargaAtualizada);
                return;
            }

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAtualizada = repCargaEntrega.BuscarPorPedido(pedido.Codigo, carga.Codigo, false);

            /* 
             * Se temos a CargaEntrega desse pedido e o valor de PrevisaoEntrega foi alterado, então
             * podemos avisar o motorista com precisão que a data de previsão da parada X foi alterada.
             * Caso contrário, notificamos o motorista apenas que a carga inteira mudou.
            */
            if (cargaEntregaAtualizada != null && !pedido.PrevisaoEntrega.Equals(previsaoEntregaAntesDaAtualizacao))
            {
                serNotificacaoMTrack.NotificarMudancaCargaEntrega(cargaEntregaAtualizada, carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.EntregaPrevisaoAtualizada, true, _clienteURLAcesso.Cliente.Codigo);
            }
            else
            {
                serNotificacaoMTrack.NotificarMudancaCarga(carga, carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.CargaAtualizada, true, _clienteURLAcesso.Cliente.Codigo);
            }
        }

        private static Dominio.Entidades.Embarcador.Pedidos.Pedido ObterPedidoPorProtocolo(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAntesDaAtualizacao = null;
            if (protocolo.protocoloIntegracaoPedido > 0)
            {
                pedidoAntesDaAtualizacao = repPedido.BuscarPorProtocolo(protocolo.protocoloIntegracaoPedido);
            }
            else if (protocolo.protocoloIntegracaoCarga > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(protocolo.protocoloIntegracaoCarga);
                if (cargaPedidos.Count == 1)
                    pedidoAntesDaAtualizacao = cargaPedidos.FirstOrDefault().Pedido;
            }

            return pedidoAntesDaAtualizacao;
        }

        private void SalvarDocumento(Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaDadosParaProcesamentoDt repCargaDadosParaProcesamentoDt = new Repositorio.Embarcador.Cargas.CargaDadosParaProcesamentoDt(_unitOfWork);
            Servicos.Log.TratarErro("Propiedade ControleIntegraca " + documentoTransporte?.ControleIntegracaoEmbarcador, "RequestLog");
            repCargaDadosParaProcesamentoDt.Inserir(new Dominio.Entidades.Embarcador.Cargas.CargaDadosParaProcessamentoDt()
            {
                Documento = Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransporte),
                Carga = carga
            });
        }


        private void SalvarFiliaisProduto(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto objProduto, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto)
        {
            if (produto == null)
                return;

            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repEmpresa = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcardorFilialSituacao repSituacoFilial = new Repositorio.Embarcador.Produtos.ProdutoEmbarcardorFilialSituacao(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFilial repEmbarcadorFilial = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFilial(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilialSituacoes> listaSituacoes = repSituacoFilial.BuscarTodos();
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial> filiais = produto.Filiais != null ? produto.Filiais.ToList() : new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial>();

            if (filiais?.Count > 0)
            {
                List<int> codigosFiliais = new List<int>();
                foreach (Dominio.ObjetosDeValor.WebService.Pedido.Filiais item in objProduto.Filiais)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial exiteFilianNoProduto = filiais.Where(f => f.FilialSituacao.SituacaoFilial == item.Situacao
                                                                                        && item.CodigoIntegracao == f.Filial.CodigoFilialEmbarcador
                                                                                        && item.NCM == f.NCM && item.UsoMaterial == f.UsoMaterial).FirstOrDefault();

                    if (exiteFilianNoProduto != null)
                        codigosFiliais.Add(exiteFilianNoProduto.Codigo);
                }
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial> listaFiliaisRemover = filiais.Where(f => !codigosFiliais.Contains(f.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial filialRemover in listaFiliaisRemover)
                {
                    filiais = filiais.Where(x => x.Codigo != filialRemover.Codigo).ToList();
                    repEmbarcadorFilial.Deletar(filialRemover);
                }
            }
            else
                produto.Filiais = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial>();

            if (objProduto?.Filiais?.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.WebService.Pedido.Filiais itemFilial in objProduto.Filiais)
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repEmpresa.buscarPorCodigoEmbarcador(itemFilial.CodigoIntegracao);

                    if (filial == null)
                        continue;

                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilialSituacoes exiteSituacaoCadastrada = listaSituacoes.Where(s => s.SituacaoFilial == itemFilial.Situacao).FirstOrDefault();
                    if (exiteSituacaoCadastrada == null)
                    {
                        exiteSituacaoCadastrada = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilialSituacoes()
                        {
                            SituacaoFilial = itemFilial.Situacao,
                            Descricao = itemFilial.Situacao.ObterDescricao()
                        };
                        repSituacoFilial.Inserir(exiteSituacaoCadastrada);
                    }
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial existefiLial = filiais.Where(f => f.FilialSituacao == exiteSituacaoCadastrada && f.ProdutoEmbarcador == produto && f.Filial == filial).FirstOrDefault();

                    if (existefiLial == null)
                        existefiLial = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFilial();

                    existefiLial.Ativo = true;
                    existefiLial.NCM = itemFilial.NCM;
                    existefiLial.Filial = filial;
                    existefiLial.UsoMaterial = itemFilial.UsoMaterial;
                    existefiLial.FilialSituacao = exiteSituacaoCadastrada;
                    existefiLial.ProdutoEmbarcador = produto;

                    if (existefiLial.Codigo > 0)
                        repEmbarcadorFilial.Atualizar(existefiLial);
                    else
                        repEmbarcadorFilial.Inserir(existefiLial);
                }
            }

        }

        private void SalvarOrganizacoes(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto objProduto, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao repOrganizacaoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Produtos.Organizacao repOrganizacao = new Repositorio.Embarcador.Produtos.Organizacao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao> listOrganizacoes = null;

            if (produto.Organizacoes != null)
                listOrganizacoes = produto.Organizacoes.ToList();
            else
                listOrganizacoes = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao>();

            if (listOrganizacoes.Count > 0)
            {
                List<int> codigos = new List<int>();
                foreach (Dominio.ObjetosDeValor.WebService.Pedido.Organizacao ItemOrganicazao in objProduto.Organizacao)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao existeOrganizacaoNoProduto = listOrganizacoes.Where(o => o.Organizacao.DescricaoHierarquia == ItemOrganicazao.DescricaoHierarquia
                                                                                                                                       && o.Organizacao.Canal == ItemOrganicazao.Canal && o.Organizacao.CodigoHierarquia == ItemOrganicazao.Hierarquia
                                                                                                                                       && o.Organizacao.Descricao == ItemOrganicazao.CodigoIntegracao && o.Organizacao.Setor == ItemOrganicazao.Setor
                                                                                                                                       && o.Organizacao.Nivel == ItemOrganicazao.Nivel).FirstOrDefault();

                    if (existeOrganizacaoNoProduto != null)
                        codigos.Add(existeOrganizacaoNoProduto.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao> listaParaRemover = listOrganizacoes.Where(o => !codigos.Contains(o.Codigo)).ToList();
                foreach (Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao item in listaParaRemover)
                    repOrganizacaoEmbarcador.Deletar(item);
            }

            if (objProduto.Organizacao == null)
                return;

            foreach (Dominio.ObjetosDeValor.WebService.Pedido.Organizacao ItemOrganicazao in objProduto.Organizacao)
            {
                Dominio.Entidades.Embarcador.Produtos.Organizacao organizacao = new Dominio.Entidades.Embarcador.Produtos.Organizacao();

                organizacao.Descricao = ItemOrganicazao.CodigoIntegracao;
                organizacao.Canal = ItemOrganicazao.Canal;
                organizacao.Setor = ItemOrganicazao.Setor;
                organizacao.Nivel = ItemOrganicazao.Nivel;
                organizacao.CodigoHierarquia = ItemOrganicazao.Hierarquia;
                organizacao.DescricaoHierarquia = ItemOrganicazao.DescricaoHierarquia;

                Dominio.Entidades.Embarcador.Produtos.Organizacao existeOrganizacao = repOrganizacao.OrganizacaoExistente(organizacao);

                if (existeOrganizacao == null)
                    repOrganizacao.Inserir(organizacao);
                else
                    organizacao = existeOrganizacao;

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao existeOrganizacaoNoProduto = listOrganizacoes.Where(o => o.Organizacao.DescricaoHierarquia == organizacao.DescricaoHierarquia
                                                                                                                                        && o.Organizacao.Canal == organizacao.Canal && o.Organizacao.CodigoHierarquia == organizacao.CodigoHierarquia
                                                                                                                                        && o.Organizacao.Descricao == organizacao.Descricao && o.Organizacao.Setor == organizacao.Setor
                                                                                                                                        && o.Organizacao.Nivel == organizacao.Nivel).FirstOrDefault();
                if (existeOrganizacaoNoProduto == null)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao organizacaoProduto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorOrganizacao()
                    {
                        Organizacao = organizacao,
                        ProdutoEmbarcador = produto
                    };
                    repOrganizacaoEmbarcador.Inserir(organizacaoProduto);
                }
            }

            repProdutoEmbarcador.Atualizar(produto);
        }

        private void SalvarUnidadeDeConversao(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto objProduto, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto)
        {
            if (objProduto.Conversao == null)
                return;

            Repositorio.Embarcador.Produtos.ConversaoDeUnidade repositorioTipoConversaoUnidade = new Repositorio.Embarcador.Produtos.ConversaoDeUnidade(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao repositorioTabelaConversao = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            if (produto.TabelaConversao != null && produto.TabelaConversao.Count > 0)
            {
                List<dynamic> conversoes = new List<dynamic>();

                foreach (Dominio.ObjetosDeValor.Embarcador.Produtos.Conversao conversao in objProduto.Conversao)
                    if (!string.IsNullOrEmpty(conversao.UnidadePara) && !string.IsNullOrEmpty(conversao.UnidadeDe) && conversao.QuantidadePara > 0 && conversao.QuantidadeDe > 0)
                        conversoes.Add(conversao);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao> listaConversaoRemover = produto.TabelaConversao.Where(c => !conversoes.Any(t => t.UnidadePara == c.TipoConversao.UnidadeDeMedida.Sigla
                                                                                                                                                && t.UnidadeDe == c.TipoConversao.Sigla && t.QuantidadePara == c.QuantidadePara
                                                                                                                                                && t.QuantidadeDe == c.QuantidadeDe)).ToList();

                foreach (Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao conversaoRemover in listaConversaoRemover)
                    produto.TabelaConversao.Remove(conversaoRemover);
            }
            else
                produto.TabelaConversao = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Produtos.Conversao conversao in objProduto.Conversao)
            {
                Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade existeTipoConversao = repositorioTipoConversaoUnidade.BuscarTipoConversaoPorSiglas(conversao.UnidadeDe, conversao.UnidadePara);

                if (existeTipoConversao == null)
                {
                    Repositorio.UnidadeDeMedida repositorioUnidadeMedida = new Repositorio.UnidadeDeMedida(_unitOfWork);

                    Dominio.Entidades.UnidadeDeMedida existeUnidadeMedida = repositorioUnidadeMedida.BuscarPorSigla(conversao.UnidadePara);
                    if (existeUnidadeMedida == null)
                    {
                        existeUnidadeMedida = new Dominio.Entidades.UnidadeDeMedida()
                        {
                            Descricao = conversao.UnidadePara,
                            Sigla = conversao.UnidadePara,
                            Status = "A",
                            CodigoDaUnidade = conversao.UnidadePara
                        };
                        repositorioUnidadeMedida.Inserir(existeUnidadeMedida);
                    }

                    existeTipoConversao = new Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade()
                    {
                        Descricao = conversao.UnidadeDe,
                        Sigla = conversao.UnidadeDe,
                        UnidadeDeMedida = existeUnidadeMedida
                    };

                    repositorioTipoConversaoUnidade.Inserir(existeTipoConversao);
                }

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao existeConversaoNoProduto = produto.TabelaConversao.Where(t => t.TipoConversao.Codigo == existeTipoConversao.Codigo && conversao.QuantidadePara == t.QuantidadePara
                                                                                                                                                && conversao.QuantidadeDe == t.QuantidadeDe).FirstOrDefault();

                if (existeConversaoNoProduto == null)
                    existeConversaoNoProduto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao();

                existeConversaoNoProduto.QuantidadeDe = conversao.QuantidadeDe;
                existeConversaoNoProduto.QuantidadePara = conversao.QuantidadePara;
                existeConversaoNoProduto.TipoConversao = existeTipoConversao;

                if (existeConversaoNoProduto.Codigo > 0)
                    repositorioTabelaConversao.Atualizar(existeConversaoNoProduto);
                else
                    repositorioTabelaConversao.Inserir(existeConversaoNoProduto);

                produto.TabelaConversao.Add(existeConversaoNoProduto);

            }

            repositorioProduto.Atualizar(produto);
        }

        public void ExecutarConversaoDePeso(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto objProduto, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto)
        {
            if (string.IsNullOrEmpty(objProduto.UnidadeMedidaPeso) || objProduto.UnidadeMedidaPeso == "KG" || objProduto.UnidadeMedidaPeso == "KGM")
                return;

            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            List<string> conversoesPermitidasForaTabela = new List<string>() { "GRM", "GR1", "TNE", "TON", "LBR" };

            if (conversoesPermitidasForaTabela.Contains(objProduto.UnidadeMedidaPeso))
                produto.PesoUnitario = ObterPesoConvertidoPorUnidadeDeMedida(objProduto.UnidadeMedidaPeso, produto.PesoUnitario);
            else
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorTabelaConversao existeConversaoNoProduto = produto.TabelaConversao.Where(c => c.TipoConversao.Sigla == objProduto.UnidadeMedidaPeso
                                                                                                                                            && c.TipoConversao.UnidadeDeMedida.Sigla == "KG").FirstOrDefault();
                if (existeConversaoNoProduto == null)
                    return;

                produto.PesoUnitario = produto.PesoUnitario * existeConversaoNoProduto.QuantidadePara;
            }

            repositorioProduto.Atualizar(produto);
        }

        private string ProcessarVeiculosDadosTransporte(Dominio.ObjetosDeValor.WebService.Carga.DadosTransporte dadosTransporte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService)
        {
            Servicos.WebService.Frota.Veiculo serVeiculo = new Servicos.WebService.Frota.Veiculo(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.Entidades.Veiculo existeVeiculo = null;
            if (!string.IsNullOrWhiteSpace(dadosTransporte?.Veiculo?.Placa))
            {
                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    existeVeiculo = repVeiculo.BuscarPorPlaca(dadosTransporte.Veiculo.Placa);
                else
                    existeVeiculo = repVeiculo.BuscarPorPlaca(carga.Empresa?.Codigo ?? 0, dadosTransporte.Veiculo.Placa);

                if (existeVeiculo == null && !(configuracaoWebService?.CadastrarVeiculoAoInformarDadosTransporteCarga ?? false))
                    return $"Veiculo por placa {dadosTransporte.Veiculo.Placa} não encontrado";
                else if (existeVeiculo == null && (configuracaoWebService?.CadastrarVeiculoAoInformarDadosTransporteCarga ?? false))
                {
                    //cadastrar veiculo
                    string mensagemCadastro = "";
                    existeVeiculo = serVeiculo.SalvarVeiculo(dadosTransporte.Veiculo, _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : carga.Empresa, false, ref mensagemCadastro, _unitOfWork, _tipoServicoMultisoftware);

                    if (!string.IsNullOrWhiteSpace(mensagemCadastro))
                        return mensagemCadastro;
                }

                if (existeVeiculo != null && (!(configuracaoWebService?.NaoValidarTipoDeVeiculoNoMetodoInformarDadosTransporteCarga ?? false) && existeVeiculo.TipoVeiculo != "0"))
                    return $"Veiculo informado não é de tipo tração";

                dadosTrans.CodigoTracao = existeVeiculo.Codigo;
                carga.Veiculo = existeVeiculo;
            }
            else if (!configuracaoWebService.PermitirRemoverVeiculoNoMetodoInformarDadosTransporteCarga)
                return "Precisa informar placa do veiculo";

            if (!string.IsNullOrWhiteSpace(dadosTransporte.Reboque1?.Placa) || !string.IsNullOrWhiteSpace(dadosTransporte.Reboque2?.Placa))
            {
                Dominio.Entidades.Veiculo reboque1 = null;
                if (!string.IsNullOrWhiteSpace(dadosTransporte.Reboque1?.Placa))
                {
                    if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        reboque1 = repVeiculo.BuscarPorPlaca(dadosTransporte.Reboque1.Placa);
                    else
                        reboque1 = repVeiculo.BuscarPorPlaca(carga.Empresa?.Codigo ?? 0, dadosTransporte.Reboque1.Placa);

                    string mensagemVeiculo = "";
                    if (reboque1 == null)
                        reboque1 = serVeiculo.SalvarVeiculo(dadosTransporte.Reboque1, _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : carga.Empresa, false, ref mensagemVeiculo, _unitOfWork, _tipoServicoMultisoftware);

                    if (!string.IsNullOrWhiteSpace(mensagemVeiculo))
                        return mensagemVeiculo;

                    dadosTrans.CodigoReboque = reboque1.Codigo;
                }

                Dominio.Entidades.Veiculo reboque2 = null;

                if (!string.IsNullOrWhiteSpace(dadosTransporte.Reboque2?.Placa))
                {
                    if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        reboque2 = repVeiculo.BuscarPorPlaca(dadosTransporte.Reboque2.Placa);
                    else
                        reboque2 = repVeiculo.BuscarPorPlaca(carga.Empresa?.Codigo ?? 0, dadosTransporte.Reboque2.Placa);

                    string mensagemVeiculo = "";
                    if (reboque2 == null)
                        reboque2 = serVeiculo.SalvarVeiculo(dadosTransporte.Reboque2, _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : carga.Empresa, false, ref mensagemVeiculo, _unitOfWork, _tipoServicoMultisoftware);

                    if (!string.IsNullOrWhiteSpace(mensagemVeiculo))
                        return mensagemVeiculo;

                    dadosTrans.CodigoSegundoReboque = reboque2.Codigo;
                }
            }

            if (dadosTransporte.Veiculo.Reboques == null || dadosTransporte.Veiculo.Reboques.Count == 0)
            {
                if (!(configuracaoWebService?.NaoValidarTipoDeVeiculoNoMetodoInformarDadosTransporteCarga ?? false) && carga.ModeloVeicularCarga?.NumeroReboques > 0)
                    return "Precisa informar pelo menos um Reboque";
                dadosTransporte.Veiculo.Reboques = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();
            }

            if (existeVeiculo != null && existeVeiculo.VeiculosVinculados == null)
                existeVeiculo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

            if (existeVeiculo?.VeiculosVinculados.Count > 0)
            {
                List<string> placasRevoques = new List<string>();

                foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboque in dadosTransporte.Veiculo.Reboques)
                    if (!string.IsNullOrEmpty(reboque.Placa))
                        placasRevoques.Add(reboque.Placa);

                List<Dominio.Entidades.Veiculo> reboquesRemover = existeVeiculo.VeiculosVinculados.Where(r => !placasRevoques.Contains(r.Placa)).ToList();

                foreach (Dominio.Entidades.Veiculo reboqueAremover in reboquesRemover)
                    existeVeiculo.VeiculosVinculados.Remove(reboqueAremover);
            }

            int numeroReboques = 1;

            foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboque in dadosTransporte.Veiculo.Reboques)
            {
                if ((carga?.ModeloVeicularCarga?.NumeroReboques ?? 0) == 0)
                    continue;

                Dominio.Entidades.Veiculo reboqueVeiculo = repVeiculo.BuscarPorPlaca(reboque.Placa);

                if (reboqueVeiculo == null)
                    return "Reboque não informado ou não encontrado!";

                if (reboqueVeiculo.TipoVeiculo != "1")
                    return $"A Placa {reboqueVeiculo.Placa} não é de tipo reboque.";

                bool existeVeiculoVinculado = existeVeiculo.VeiculosVinculados.Any(v => v.Codigo == reboqueVeiculo.Codigo);

                if (numeroReboques == 1)
                    dadosTrans.CodigoReboque = reboqueVeiculo.Codigo;
                if (numeroReboques == 2)
                    dadosTrans.CodigoSegundoReboque = reboqueVeiculo.Codigo;

                numeroReboques++;

                if (existeVeiculoVinculado)
                    continue;

                existeVeiculo.VeiculosVinculados.Add(reboqueVeiculo);
            }

            return string.Empty;
        }

        private Dominio.Entidades.Usuario ObterMotoristaParaPreencherDados(Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            string cpf = Utilidades.String.OnlyNumbers(motoristaIntegracao.CPF);
            if (string.IsNullOrWhiteSpace(cpf))
                return null;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return repositorioUsuario.BuscarMotoristaPorCPF(cpf);

            Dominio.Entidades.Empresa transportador = motoristaIntegracao.Transportador != null ? repositorioEmpresa.BuscarEmpresaPorCNPJ(Utilidades.String.OnlyNumbers(motoristaIntegracao.Transportador.CNPJ)) : carga.Empresa;
            if (transportador != null)
            {
                Dominio.Entidades.Empresa matriz = transportador.Matriz.FirstOrDefault();
                Dominio.Entidades.Usuario motorista = matriz != null ? repositorioUsuario.BuscarMotoristaPorCPFEEmpresa(cpf, matriz.Codigo) : null;
                if (motorista != null)
                    return motorista;

                return repositorioUsuario.BuscarMotoristaPorCPFEEmpresa(cpf, transportador.Codigo, string.Empty);
            }

            return repositorioUsuario.BuscarMotoristaPorCPF(cpf);
        }

        private decimal ObterPesoConvertidoPorUnidadeDeMedida(string unidadeMedida, decimal peso)
        {
            decimal pesoConvertido = 0;
            switch (unidadeMedida.ToUpper())
            {
                case "GRM":
                case "GR1":
                    pesoConvertido = peso / 1000;
                    break;
                case "TNE":
                case "TON":
                    pesoConvertido = peso * 1000;
                    break;
                case "LBR":
                    pesoConvertido = peso / 2.20462262m;
                    break;
            }

            return pesoConvertido;
        }

        public bool ValidarNotasPedidoEnviada(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNota = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (repNota.ContarXMLNotaFiscalPorCargaPedido(cargaPedido.Codigo) > 0)
                return true;

            if (cargaPedido.Pedido?.NotasFiscais?.Count > 0)
            {
                // ja tem notas no pedido, entao vamos vincular as notas para o CargaPedido;
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota in cargaPedido.Pedido.NotasFiscais)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal();
                    pedidoXMLNotaFiscal.CargaPedido = cargaPedido;
                    pedidoXMLNotaFiscal.XMLNotaFiscal = nota;
                    pedidoXMLNotaFiscal.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;

                    repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);
                }

                return true;
            }


            return false;

        }

        private void RemoverRegistrosDeCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao repositorioCargaCancelamentoSolicitacao = new Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao repositorioCargaCancelamentoIntegraca = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoIntegracoa = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao> existeRegistroCargaCancelamentoSolicitacao = repositorioCargaCancelamentoSolicitacao.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao> existeRegistroCargaCancelamentoIntegracao = repositorioCargaCancelamentoIntegraca.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> existeRegistroCargaCancelamentoCargaIntegracao = repositorioCargaCancelamentoIntegracoa.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento exsiteCargaCancelameto = repositorioCargaCancelamento.BuscarPorCarga(carga.Codigo);


            foreach (CargaCancelamentoSolicitacao cancelamentoSolicitacao in existeRegistroCargaCancelamentoSolicitacao)
                repositorioCargaCancelamentoSolicitacao.Deletar(cancelamentoSolicitacao);

            foreach (CargaCancelamentoIntegracao cancelamentoIntegracao in existeRegistroCargaCancelamentoIntegracao)
                repositorioCargaCancelamentoIntegraca.Deletar(cancelamentoIntegracao);

            foreach (CargaCancelamentoCargaIntegracao cancelamentoIntegracao in existeRegistroCargaCancelamentoCargaIntegracao)
                repositorioCargaCancelamentoIntegracoa.Deletar(cancelamentoIntegracao);

            repositorioCargaCancelamento.Deletar(exsiteCargaCancelameto);
        }

        private void AtivarCargaCancelada(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repostiorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repostirioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            carga.SituacaoCarga = SituacaoCarga.Nova;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repostirioCargaPedido.BuscarPorProtocoloCarga(carga.Protocolo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in cargaPedidos)
            {
                pedido.Pedido.SituacaoPedido = SituacaoPedido.Aberto;
                repostiorioPedido.Atualizar(pedido.Pedido);
            }
            repositorioCarga.Atualizar(carga);
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> AtualizarCarregamentoRoteirizacaoPedidos(CarregamentoRoteirizacao carregamentoRoteirizacao, Dominio.Entidades.Embarcador.Cargas.Carga carga = null, bool integracaoRouteasy = false)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(_unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(_unitOfWork);

            Cliente servicoCliente = new Cliente(_unitOfWork.StringConexao);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();

            List<int> protocolosPedidos = new List<int>();
            List<string> protocolosCanalEntrega = new List<string>();
            List<string> protocolosCanalVenda = new List<string>();

            if (carregamentoRoteirizacao == null)
                throw new ServicoException("Preencha os protocolos dos pedidos.");

            foreach (CarregamentoRoteirizacaoPedido carregamentoRoteirizacaoPedido in carregamentoRoteirizacao.Pedidos)
            {
                protocolosPedidos.Add(carregamentoRoteirizacaoPedido.Protocolo);

                if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacaoPedido.CodigoIntegracaoCanalEntrega))
                    protocolosCanalEntrega.Add(carregamentoRoteirizacaoPedido.CodigoIntegracaoCanalEntrega);

                if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacaoPedido.CodigoIntegracaoCanalVenda))
                    protocolosCanalVenda.Add(carregamentoRoteirizacaoPedido.CodigoIntegracaoCanalVenda);
            }

            if (protocolosPedidos.Count == 0)
                throw new ServicoException("Não foi informado nenhum pedido.");

            if (configuracaoMontagemCarga.TipoControleSaldoPedido == TipoControleSaldoPedido.CarregamentoUnico)
            {
                List<int> protocolosOutrasCargas = repositorioCargaPedido.BuscarProtocolosPedidosVinculados(protocolosPedidos, carregamentoRoteirizacao.Protocolo);

                if (protocolosOutrasCargas.Count > 0)
                    throw new ServicoException($"Pedido(s) protocolo(s) {string.Join(", ", protocolosOutrasCargas)} estão ativos em outra carga.");
            }

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorProtocolos(protocolosPedidos);
            List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> canaisEntrega = repositorioCanalEntrega.BuscarPorCodigosIntegracao(protocolosCanalEntrega);
            List<Dominio.Entidades.Embarcador.Pedidos.CanalVenda> canaisVenda = repositorioCanalVenda.BuscarPorCodigosIntegracao(protocolosCanalVenda);

            foreach (CarregamentoRoteirizacaoPedido carregamentoRoteirizacaoPedido in carregamentoRoteirizacao.Pedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidos.FirstOrDefault(pedido => pedido.Protocolo == carregamentoRoteirizacaoPedido.Protocolo);

                if (pedido == null)
                    throw new ServicoException($"Não foi localizado nenhum pedido compatível com o protocolo {carregamentoRoteirizacaoPedido.Protocolo}");

                if (pedido.SituacaoPedido == SituacaoPedido.Cancelado)
                    throw new ServicoException($"O pedido com protocolo {carregamentoRoteirizacaoPedido.Protocolo} está cancelado no Multiembarcador.");

                if (carregamentoRoteirizacaoPedido.DataPrevisaoEntrega.HasValue)
                {
                    pedido.DataPrevisaoChegadaDestinatario = carregamentoRoteirizacaoPedido.DataPrevisaoEntrega;
                    pedido.PrevisaoEntrega = carregamentoRoteirizacaoPedido.DataPrevisaoEntrega;
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, null, $"Adicionou {pedido.PrevisaoEntrega} a data de previsão de entrega.", _unitOfWork);
                }

                if (carregamentoRoteirizacaoPedido.DataPrevisaoSaida.HasValue)
                {
                    pedido.DataPrevisaoSaida = carregamentoRoteirizacaoPedido.DataPrevisaoSaida;
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, null, $"Adicionou {pedido.DataPrevisaoSaida} a data de previsão de saída.", _unitOfWork);
                }

                if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacaoPedido.CodigoIntegracaoCanalEntrega))
                {
                    Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = canaisEntrega.FirstOrDefault(canalEntrega => canalEntrega.CodigoIntegracao == carregamentoRoteirizacaoPedido.CodigoIntegracaoCanalEntrega);

                    if (canalEntrega == null)
                        throw new ServicoException($"Não foi localizado nenhum Canal de Entrega compatível com o código de integração informado ({carregamentoRoteirizacaoPedido.CodigoIntegracaoCanalEntrega}).");

                    pedido.CanalEntrega = canalEntrega;
                }

                if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacaoPedido.CodigoIntegracaoCanalVenda))
                {
                    Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda = canaisVenda.FirstOrDefault(canalVenda => canalVenda.CodigoIntegracao == carregamentoRoteirizacaoPedido.CodigoIntegracaoCanalVenda);

                    if (canalVenda == null)
                        throw new ServicoException($"Não foi localizado nenhum Canal de Venda compatível com o código de integração informado ({carregamentoRoteirizacaoPedido.CodigoIntegracaoCanalVenda}).");

                    pedido.CanalVenda = canalVenda;
                }

                if (carregamentoRoteirizacaoPedido.OrdemEntrega > 0)
                {
                    pedido.OrdemEntregaProgramada = carregamentoRoteirizacaoPedido.OrdemEntrega;

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarCargaPedidoPorPedido(pedido.Codigo);

                    if (cargaPedido != null)
                    {
                        cargaPedido.OrdemEntrega = carregamentoRoteirizacaoPedido.OrdemEntrega;
                        repositorioCargaPedido.Atualizar(cargaPedido);
                    }
                }

                if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacaoPedido.ObservacaoEntrega))
                    pedido.ObservacaoEntrega = carregamentoRoteirizacaoPedido.ObservacaoEntrega;

                if (carregamentoRoteirizacaoPedido.Expedidor != null && (!string.IsNullOrWhiteSpace(carregamentoRoteirizacaoPedido.Expedidor.CodigoIntegracao) || !string.IsNullOrWhiteSpace(carregamentoRoteirizacaoPedido.Expedidor.CPFCNPJ)))
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoExpedidor = servicoCliente.ConverterObjetoValorPessoa(carregamentoRoteirizacaoPedido.Expedidor, "Expedidor", _unitOfWork);

                    if (!retornoExpedidor.Status)
                        throw new ServicoException(retornoExpedidor.Mensagem);

                    pedido.Expedidor = retornoExpedidor.cliente;
                }

                if (carregamentoRoteirizacaoPedido.Recebedor != null && (!string.IsNullOrWhiteSpace(carregamentoRoteirizacaoPedido.Recebedor.CodigoIntegracao) || !string.IsNullOrWhiteSpace(carregamentoRoteirizacaoPedido.Recebedor.CPFCNPJ)))
                {
                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoRecebedor = servicoCliente.ConverterObjetoValorPessoa(carregamentoRoteirizacaoPedido.Recebedor, "Recebedor", _unitOfWork);

                    if (!retornoRecebedor.Status)
                        throw new ServicoException(retornoRecebedor.Mensagem);

                    pedido.Recebedor = retornoRecebedor.cliente;
                }

                repositorioPedido.Atualizar(pedido);

                if (carga != null)
                {
                    Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(_unitOfWork);
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoExistente = repositorioCargaPedido.BuscarPorCargaEPedido(carga.Codigo, pedido.Codigo);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao(); ;

                    if (cargaPedidoExistente == null)
                        servicoCargaPedido.AdicionarPedidoCarga(carga, pedido, configuracaoEmbarcador, _tipoServicoMultisoftware, integracaoRouteasy: integracaoRouteasy);
                    else
                        repPedido.Atualizar(pedido);
                }
            }

            return pedidos;
        }

        public async Task<CarregamentoRoteirizacaoDadosValidos> ValidarDadosCarregamentoRoteirizacaoAsync(CarregamentoRoteirizacao carregamentoRoteirizacao, CancellationToken cancellationToken, Dominio.Entidades.Embarcador.Cargas.Carga carga = null)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.TipoCarregamento repositorioTipoCarregamento = new Repositorio.Embarcador.Cargas.TipoCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, cancellationToken);

            Frota.Veiculo serVeiculo = new Frota.Veiculo(_unitOfWork);
            Empresa.Motorista serMotorista = new Empresa.Motorista(_unitOfWork);

            CarregamentoRoteirizacaoDadosValidos dadosValidos = new CarregamentoRoteirizacaoDadosValidos();
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(carregamentoRoteirizacao.Filial?.CodigoIntegracao ?? string.Empty);

            if (!string.IsNullOrEmpty(carregamentoRoteirizacao.Filial?.CodigoIntegracao ?? string.Empty) && filial == null)
                throw new ServicoException("Não foi localizado nenhuma filial compatível com o código de integração informado.");

            dadosValidos.Filial = filial;

            Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste = repCarga.BuscarPorCodigoCargaEmbarcador(carregamentoRoteirizacao.NumeroCarregamento, filial?.Codigo ?? 0);

            if (string.IsNullOrWhiteSpace(carregamentoRoteirizacao.IdentificadorDeRota))
                throw new ServicoException("O campo identificador de rota é obrigatório.");

            if (cargaExiste != null)
                throw new ServicoException("Carregamento já gerado anteriormente.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = AtualizarCarregamentoRoteirizacaoPedidos(carregamentoRoteirizacao, carga, integracaoRouteasy: true);

            Dominio.Entidades.Embarcador.Pedidos.Pedido primeiroPedido = pedidos.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacao.CodigoIntegracaoTipoCarga))
            {
                dadosValidos.TipoCarga = await repTipoDeCarga.BuscarPorCodigoEmbarcadorAsync(carregamentoRoteirizacao.CodigoIntegracaoTipoCarga);

                if (dadosValidos.TipoCarga == null)
                    throw new ServicoException("Não foi localizado nenhum tipo de carga compatível com o código de integração informado.");
            }

            if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacao.CodigoIntegracaoTipoOperacao))
            {
                dadosValidos.TipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(carregamentoRoteirizacao.CodigoIntegracaoTipoOperacao);

                if (dadosValidos.TipoOperacao == null)
                    throw new ServicoException("Não foi localizado nenhum tipo de operação compatível com o código de integração informado.");
            }

            if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacao.CodigoModeloVeicular))
            {
                dadosValidos.ModeloVeicularCarga = repModeloVeicularCarga.buscarPorCodigoIntegracao(carregamentoRoteirizacao.CodigoModeloVeicular);

                if (dadosValidos.ModeloVeicularCarga == null)
                    throw new ServicoException("Não foi localizado nenhum modelo veicular compatível com o código de integração informado.");
            }

            if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacao.CodigoIntegracaoTipoCarregamento))
            {
                dadosValidos.TipoCarregamento = repositorioTipoCarregamento.BuscarPorCodigoIntegracao(carregamentoRoteirizacao.CodigoIntegracaoTipoCarregamento);

                if (dadosValidos.TipoCarregamento == null)
                    throw new ServicoException("Não foi localizado nenhum tipo de carregamento compatível com o código de integração informado.");
            }

            if (carregamentoRoteirizacao.Transportador != null)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);

                if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacao.Transportador.CNPJ))
                {
                    string cnpjTratado = Utilidades.String.OnlyNumbers(carregamentoRoteirizacao.Transportador.CNPJ);

                    dadosValidos.Transportador = await repositorioEmpresa.BuscarPorCNPJAsync(cnpjTratado);
                }

                if ((dadosValidos.Transportador == null) && !string.IsNullOrWhiteSpace(carregamentoRoteirizacao.Transportador.CodigoIntegracao))
                    dadosValidos.Transportador = repositorioEmpresa.BuscarPorCodigoIntegracao(carregamentoRoteirizacao.Transportador.CodigoIntegracao);
            }

            dadosValidos.Transportador ??= dadosValidos.Motorista?.Empresa;
            dadosValidos.Transportador ??= primeiroPedido?.Empresa;

            if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacao.Veiculo?.Placa))
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

                string mensagemVeiculo = "";

                if (dadosValidos.Transportador != null)
                    dadosValidos.Veiculo = repVeiculo.BuscarPorPlacaIncluiInativos(dadosValidos.Transportador.Codigo, carregamentoRoteirizacao.Veiculo.Placa);

                if (dadosValidos.Veiculo == null)
                {
                    dadosValidos.Veiculo = serVeiculo.SalvarVeiculo(carregamentoRoteirizacao.Veiculo, dadosValidos.Transportador, false, ref mensagemVeiculo, _unitOfWork, _tipoServicoMultisoftware, _auditado);

                    if (!string.IsNullOrWhiteSpace(mensagemVeiculo))
                        throw new ServicoException(mensagemVeiculo);
                }

                if (dadosValidos.ModeloVeicularCarga == null)
                    dadosValidos.ModeloVeicularCarga = dadosValidos.Veiculo.ModeloVeicularCarga;
            }

            if (!string.IsNullOrWhiteSpace(carregamentoRoteirizacao.Motorista?.CPF))
            {
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);

                if (dadosValidos.Transportador != null)
                    dadosValidos.Motorista = repMotorista.BuscarMotoristaPorCPF(dadosValidos.Transportador.Codigo, Utilidades.String.OnlyNumbers(carregamentoRoteirizacao.Motorista.CPF));

                if (dadosValidos.Motorista == null)
                {
                    string mensagem = "";
                    dadosValidos.Motorista = serMotorista.SalvarMotorista(carregamentoRoteirizacao.Motorista, _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS ? null : dadosValidos.Transportador, ref mensagem, _unitOfWork, _tipoServicoMultisoftware, _auditado, null, _adminStringConexao);

                    if (!string.IsNullOrWhiteSpace(mensagem))
                        throw new ServicoException(mensagem);
                }
            }

            dadosValidos.Filial ??= primeiroPedido.Filial;
            dadosValidos.TipoCarga ??= primeiroPedido.TipoDeCarga;
            dadosValidos.TipoOperacao ??= primeiroPedido.TipoOperacao;
            dadosValidos.ModeloVeicularCarga ??= primeiroPedido.ModeloVeicularCarga;
            dadosValidos.Pedidos ??= pedidos;

            return dadosValidos;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Dados FilaH Unilever

        private Dominio.ObjetosDeValor.WebService.Carga.FilaHOperationsResponse ObterOperationFilaH(FilaHOperations operation, Dominio.ObjetosDeValor.WebService.Carga.FilaHRequest request, List<string> messageErro)
        {
            Dominio.ObjetosDeValor.WebService.Carga.FilaHOperationsResponse operationResponse = new Dominio.ObjetosDeValor.WebService.Carga.FilaHOperationsResponse();
            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXmlCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasRecebidas = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasEsperadas = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                List<string> numerosCargas = operation.documentsNumber.Select(x => x.documentNumber).ToList();
                List<string> numerosChaves = operation.documentsKey.Select(x => x.documentKey).ToList();

                if (numerosCargas.Any(x => x.Contains("DT")))//Definido que sera remplazado o dt por 00
                    numerosCargas = numerosCargas.Select(x => Regex.Replace(x, "[dtDT][tT]", "00", RegexOptions.IgnoreCase)).ToList();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorCodigosEmbarcadorOuNotasFiscais(numerosCargas, numerosChaves);


                List<(string codigoCargaEmbarcador, SituacaoCarga situacao)> cargasEncontradas = cargas.Count > 0 ? cargas.Select(x => ValueTuple.Create(x.CodigoCargaEmbarcador, x.SituacaoCarga)).ToList() : new List<(string, SituacaoCarga)>();
                List<string> codigoCargasInexistentes = numerosCargas.Count > 0 ? numerosCargas.Where(x => !cargasEncontradas.Any(o => o.codigoCargaEmbarcador == x)).ToList() : new List<string>();
                StringBuilder messagesErros = new StringBuilder();

                if (numerosCargas.Count > 0)
                {
                    foreach (string codigoCarga in codigoCargasInexistentes)
                        messagesErros.Append($"DT{codigoCarga.Substring(2)} - DT cancelada ou DT sem stage./");
                }

                if (cargas != null && cargas.Count > 0)
                    notasEsperadas = repXmlNotaFiscal.BuscarPorCargas(cargas.Select(obj => obj.Codigo).ToList());

                Dominio.Entidades.Embarcador.Cargas.Carga cargaBase = cargas.Count > 0 ? cargas.FirstOrDefault() : null; //Este regra foi definida pelo chefee

                List<(int codigo, string chaveNota, int codigoCarga, bool notaValida, int numeroNota, string nomeEmitente)> dadosNota = repXmlNotaFiscal.BuscarDadosPorChaves(numerosChaves);
                List<(int numeroCte, int codigoNota, bool notaCancelada, bool cteCancelado, string chave)> dadoCtes = repCargaPedidoXmlCTe.BuscaDadosCtePorNota(dadosNota.Select(x => x.codigo).ToList());
                bool tudoOk = true;

                foreach ((int codigo, string chaveNota, int codigoCarga, bool notaValida, int numeroNota, string nomeEmitente) dadoNota in dadosNota)
                {
                    if (dadoNota.notaValida)
                    {
                        tudoOk = false;
                        messageErro.Add($"A nota fiscal {dadoNota.numeroNota} é FOB, porém o fornecedor {dadoNota.nomeEmitente} está cadastrado na lista de exceção do MultiEmbarcador");
                    }

                    (int numeroCteAtual, int codigoNotaAtual, bool notaCanceladaAtual, bool cteCancelado, string chave) = dadoCtes.Where(x => x.codigoNota == dadoNota.codigo).FirstOrDefault();
                    bool exiteNotaEmOutroCte = dadoCtes.Where(x => x.codigoNota == dadoNota.codigo).Count() > 1;

                    if (notaCanceladaAtual)
                    {
                        messageErro.Add($"{dadoNota.chaveNota} / {numeroCteAtual} - Unitização não realizada, nota cancelada");
                        tudoOk = false;
                    }
                    else if (cteCancelado && !exiteNotaEmOutroCte)
                    {
                        messageErro.Add($"{dadoNota.chaveNota} / {numeroCteAtual} - Unitização não realizada, CTe cancelado");
                        tudoOk = false;
                    }
                    if (numeroCteAtual == 0)
                    {
                        messageErro.Add($"{dadoNota.chaveNota} / {numeroCteAtual} - Unitização não realizada, checkin de frete não realizado");
                        tudoOk = false;
                    }
                    else
                        messageErro.Add($"{dadoNota.chaveNota} - Unitização realizada com sucesso");
                }

                if (cargaBase != null)
                {
                    List<string> chavesNotasAtuaisNaCarga = repXmlNotaFiscal.BuscarPorChavesDasNotasNaCarga(cargaBase.Codigo);
                    List<string> chavesFaltantes = chavesNotasAtuaisNaCarga.Where(x => !numerosChaves.Contains(x)).ToList();
                    List<string> chavesAMais = numerosChaves.Where(x => !chavesNotasAtuaisNaCarga.Contains(x)).ToList();

                    foreach (string aMaisNota in chavesAMais)
                        messageErro.Add($"{aMaisNota} / {dadoCtes.Where(x => x.chave == aMaisNota)?.FirstOrDefault().numeroCte ?? 0}- Unitização não realizada, nota não consta na Carga");

                    foreach (string chaveFaltente in chavesFaltantes)
                        messageErro.Add($"{chaveFaltente} / {dadoCtes.Where(x => x.chave == chaveFaltente)?.FirstOrDefault().numeroCte ?? 0}- Unitização não realizada, nota faltante");

                    if (chavesAMais.Count > 0 || chavesFaltantes.Count > 0)
                        tudoOk = false;
                }

                List<(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota, string mensagem, bool validado, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)> listaNFes = ValidarNotasFiscaisFilaH(notasRecebidas, notasEsperadas, (cargas != null && cargas.Count > 0));
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCargas(cargas.Select(obj => obj.Codigo).ToList());
                cargaCTes.AddRange(repCargaCTe.BuscarPorNotasFiscais(listaNFes.Select(obj => obj.nota.Codigo).ToList()));
                cargaCTes = cargaCTes.Distinct().ToList();

                bool operacaoOcorrencia = operation.type == 98;

                operationResponse.optype = operation.type;
                operationResponse.plantName = operation.type == 1 ? ObterPlantNameFilaH(cargas) : null;
                operationResponse.freightDocuments = ObterFreightDocumentsFilaH(operacaoOcorrencia ? null : cargas, messagesErros, tudoOk);
                operationResponse.cteProductDocuments = ObterCTeProductDocumentsFilaH(listaNFes, cargaCTes, operation.type != 1, operacaoOcorrencia, operation.type);

                if (tudoOk && operation.type == 3)
                    messageErro.Add("Unitização realizada com sucesso, checkin de frete realizado com sucesso");

                return operationResponse;
            }
            catch (ServicoException exe)
            {
                List<string> messageRetornos = exe.Message.Split('/').ToList();

                return operationResponse;
            }
            catch (Exception)
            {
                return operationResponse;
            }
        }

        private List<string> ObterPlantNameFilaH(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas)
        {
            List<string> retorno = new List<string>();

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in carga.Pedidos)
                    if (!retorno.Contains(pedido.Pedido?.Filial?.CodigoFilialEmbarcador ?? string.Empty))
                        retorno.Add(pedido.Pedido.Filial.CodigoFilialEmbarcador);

            return retorno.Count > 0 ? retorno : null;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.FreightDocument> ObterFreightDocumentsFilaH(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, StringBuilder mensagems, bool tudoOk)
        {

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.Carga.FreightDocument> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.FreightDocument>();

            if (!string.IsNullOrEmpty(mensagems.ToString()))
            {
                List<string> messageRetornos = mensagems.ToString().Split('/').ToList();

                foreach (string retornoE in messageRetornos)
                    if (!string.IsNullOrEmpty(retornoE))
                    {
                        List<string> mensagemSplit = retornoE.Split('-').ToList();
                        string numeroCarga = mensagemSplit.Count > 0 ? mensagemSplit.ElementAtOrDefault(0) : string.Empty;
                        string erroCarga = mensagemSplit.Count > 1 ? mensagemSplit.ElementAtOrDefault(1) : string.Empty;

                        retorno.Add(new FreightDocument()
                        {
                            comments = erroCarga.Trim(),
                            partners = new List<Dominio.ObjetosDeValor.WebService.Carga.Partner>() {
                                           new Partner()
                                           {
                                               vendorCNPJ = string.Empty,
                                               customerCNPJ = string.Empty,
                                               partnerFunctionKUNNR = string.Empty,
                                               partnerFunctionLIFNR = string.Empty,
                                               partnerFunctionPARVW = string.Empty
                                           }
                                       },
                            poBalance = string.Empty,
                            carrierCNPJ = string.Empty,
                            carrierName = string.Empty,
                            vehicleTypes = new List<Dominio.ObjetosDeValor.WebService.Carga.VehicleType>() {
                                new VehicleType() {
                                    destCity = string.Empty,
                                    SAPVehicleTypeLoad = string.Empty
                                }
                            },
                            carrierNumber = string.Empty,
                            dangerousLoad = string.Empty,
                            documentNumber = numeroCarga.Trim(),
                            documentNumberStatus = string.Empty,
                            UnileverReceivingLocation = string.Empty
                        });
                    }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                bool sucesso = ValidarCargaFilaH(carga, out string mensagem);

                if (sucesso)
                    new Servicos.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork).Validar(carga);
                else
                    tudoOk = false;

                string codigoEmbarcador = ObterCodigoCargaEmbarcadorFormatadoFilaH(carga);
                retorno.Add(new FreightDocument()
                {
                    tNumber = codigoEmbarcador,
                    comments = mensagem,
                    partners = ObterPartnersFilaH(carga),
                    poBalance = "",
                    carrierCNPJ = carga.Empresa?.CNPJ ?? string.Empty,
                    carrierName = carga.Empresa?.RazaoSocial ?? string.Empty,
                    shipmentType = carga.TipoDocumentoTransporte?.CodigoIntegracao ?? string.Empty,
                    vehicleTypes = ObterVehicleTypesFilaH(carga),
                    carrierNumber = carga.Empresa != null ? carga.Empresa.CodigoIntegracao.ElementAtOrDefault(0) == 'T' ? carga.Empresa.CodigoIntegracao.Remove(0, 1) : string.Empty : string.Empty,
                    dangerousLoad = carga.CargaPerigosaIntegracaoLeilao ? "S" : "N",
                    documentNumber = codigoEmbarcador,
                    documentNumberStatus = "1", // NAO SEI
                    UnileverReceivingLocation = carga.Filial?.CodigoFilialEmbarcador ?? string.Empty
                });
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.Partner> ObterPartnersFilaH(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.Partner> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.Partner>();

            List<char> charValidar = new List<char>() { 'C', 'F' };
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
            {
                Dominio.Entidades.Cliente destinatario = cargaPedido.Pedido?.Destinatario;
                Dominio.Entidades.Cliente codigoIntegracao = cargaPedido.Pedido?.Destinatario;
                retorno.Add(new Partner()
                {
                    vendorCNPJ = "",
                    customerCNPJ = "",
                    partnerFunctionKUNNR = !string.IsNullOrWhiteSpace(destinatario?.CodigoIntegracao) && charValidar.Contains(destinatario.CodigoIntegracao.ElementAt(0)) ? destinatario.CodigoIntegracao.Remove(0, 1) : destinatario?.CodigoIntegracao ?? string.Empty,
                    partnerFunctionLIFNR = "",
                    partnerFunctionPARVW = "",
                });
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.Partner> ObterCarrierPartnersFilaH()
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.Partner> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.Partner>();

            retorno.Add(new Partner()
            {
                vendorCNPJ = "",
                customerCNPJ = "",
                partnerFunctionKUNNR = "",
                partnerFunctionLIFNR = "",
                partnerFunctionPARVW = "",
            });

            return retorno;
        }

        private bool ValidarCargaFilaH(Dominio.Entidades.Embarcador.Cargas.Carga carga, out string mensagem)
        {
            string retorno = string.Empty;

            if (carga == null)
            {
                mensagem = $"Unitização não realizada, DT{carga.CodigoCargaEmbarcador} inexistente";
                return false;
            }

            if (carga.SituacaoCarga == SituacaoCarga.Cancelada)
            {
                mensagem = $"Unitização não realizada, DT{carga.CodigoCargaEmbarcador.Substring(2)} está cancelada";
                return false;
            }

            if (!(carga?.CargaGeradaViaDocumentoTransporte ?? false))
            {
                mensagem = $"Unitização não realizada, a DT{carga.CodigoCargaEmbarcador.Substring(2)} foi criada diretamente no MultiEmbarcador";
                return false;
            }

            if ((carga.Veiculo == null && carga.VeiculosVinculados == null) || (carga.Motoristas == null || carga.Motoristas.Count == 0))
            {
                mensagem = $"Unitização não realizada, a DT{carga.CodigoCargaEmbarcador.Substring(2)} não possui pré-checkin de placas";
                return false;
            }

            mensagem = $"DT{carga.CodigoCargaEmbarcador.Substring(2)} - Check In autorizado com sucesso.";
            return true;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.VehicleType> ObterVehicleTypesFilaH(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.VehicleType> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.VehicleType>();
            if (carga.VeiculosVinculados == null)
                return retorno;

            foreach (Dominio.Entidades.Veiculo veiculo in carga.VeiculosVinculados)
            {
                retorno.Add(new VehicleType()
                {
                    destCity = veiculo.Empresa?.Localidade?.Descricao ?? string.Empty,
                    SAPVehicleTypeLoad = carga.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty
                });
            }
            if (carga.Veiculo != null)
            {
                retorno.Add(new VehicleType()
                {
                    destCity = carga.Veiculo?.Empresa?.Localidade?.Descricao ?? string.Empty,
                    SAPVehicleTypeLoad = carga.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty
                });
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.CteProductDocument> ObterCTeProductDocumentsFilaH(List<(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota, string mensagem, bool validado, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)> listaNFes, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, bool mostrarDados, bool operacaoOcorrencia, int codigoType)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.Carga.CteProductDocument> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.CteProductDocument>();
            if (!mostrarDados)
            {
                retorno.Add(new CteProductDocument()
                {
                    serie = null,
                    number = null,
                    comments = "",
                    netWeight = "",
                    issuerCNPJ = "",
                    issuerName = "",
                    grossWeight = "",
                    messageStatus = "",
                    operationType = "",
                    destinationCNPJ = "",
                    destinationName = "",
                    comexOperation_1 = "",
                    comexOperation_2 = "",
                    productDocuments = ObterProductDocumentsFilaH(null, mostrarDados, null, true, codigoType),
                    complianceProcess = "",
                    documentAccessKey = "",
                    unileverOperation = "",
                    documentAccessKeyStatus = null
                });
                return retorno;
            }

            foreach (CargaCTe cargaCTe in cargaCTes)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTe.CTe;
                if (cte == null)
                    continue;
                List<(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota, string mensagem, bool validado, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)> cteNFes = listaNFes.Where(obj => obj.cte?.Codigo == cargaCTe.CTe?.Codigo).ToList();

                if (cteNFes == null || cteNFes.Count == 0)
                    continue;

                bool cteValidado = operacaoOcorrencia || cteNFes.All(obj => obj.validado);

                retorno.Add(new CteProductDocument()
                {
                    serie = cte.Serie?.Numero ?? 0,
                    number = cte.Numero,
                    comments = cteValidado ? "Unitização realizada com sucesso" : "Unitização não realizada, notas pendentes",
                    documentAccessKey = cte.Chave,
                    documentAccessKeyStatus = cte.MensagemStatus?.CodigoDoErro ?? 0,
                    operationType = operacaoOcorrencia ? string.Empty : cte.CFOP?.NaturezaDaOperacao?.Descricao ?? string.Empty,
                    issuerCNPJ = operacaoOcorrencia ? string.Empty : cte.Empresa?.CNPJ ?? string.Empty,
                    issuerName = operacaoOcorrencia ? string.Empty : cte.Empresa?.RazaoSocial ?? string.Empty,
                    destinationCNPJ = operacaoOcorrencia ? string.Empty : cte.Destinatario?.CPF_CNPJ ?? string.Empty,
                    destinationName = operacaoOcorrencia ? string.Empty : cte.Destinatario?.Nome ?? string.Empty,
                    grossWeight = operacaoOcorrencia ? string.Empty : cte.Documentos != null ? Math.Round(cte.Documentos.Sum(obj => obj.Peso), 2).ToString() : string.Empty,
                    netWeight = "",
                    unileverOperation = ObterUnileverOperationFilaH(cte),
                    complianceProcess = ObterComplianceProcessFilaH(cte),
                    messageStatus = "",
                    comexOperation_1 = "N", //nao sei
                    comexOperation_2 = "",
                    productDocuments = ObterProductDocumentsFilaH(cteNFes, true, cargaCTe, operacaoOcorrencia, codigoType)
                }); ;
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.ProductDocument> ObterProductDocumentsFilaH(List<(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota, string mensagem, bool validado, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)> listaNFes, bool mostrarDados, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, bool operacaoOcorrencia, int codigoType)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repNotaProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.Carga.ProductDocument> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.ProductDocument>();
            if (!mostrarDados)
            {
                retorno.Add(new ProductDocument()
                {
                    serie = null,
                    number = null,
                    carriers = ObterCarriersFilaH(null, null),
                    comments = "",
                    products = ObterProductsFilaH(null),
                    issuerCPF = "",
                    netWeight = "",
                    issuerCNPJ = "",
                    issuerName = "",
                    carrierCNPJ = null,
                    carrierName = "",
                    grossWeight = "",
                    sapcodeDest = null,
                    freightPaidby = "",
                    messageStatus = "",
                    operationType = "",
                    sapcodeIssuer = null,
                    destinationCNPJ = "",
                    destinationName = "",
                    comexOperation_1 = "",
                    comexOperation_2 = null,
                    issuerVendorName = "",
                    complianceProcess = "",
                    documentAccessKey = "",
                    unileverOperation = "",
                    fiscalpointofCheck = "",
                    issuerCustomerName = "",
                    placeofdeliveryCNPJ = null,
                    placeofdeliveryName = "",
                    destinationVendorName = "",
                    intramunicipOperation = "",
                    destinationCustomerName = "",
                    documentAccessKeyStatus = null,
                    placeofdeliveryVendorName = "",
                    placeofdeliveryCustomerName = ""
                });
                return retorno;
            }
            if (operacaoOcorrencia)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repCargaOcorrencia.BuscarPorCTe(cargaCTe.Codigo);
                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
                {
                    retorno.Add(new ProductDocument()
                    {
                        serie = null,
                        number = ocorrencia.NumeroOcorrencia,
                        carriers = ObterCarriersFilaH(null, ocorrencia),
                        comments = "",
                        products = ObterProductsFilaH(null),
                        issuerCPF = "",
                        netWeight = "",
                        issuerCNPJ = "",
                        issuerName = "",
                        carrierCNPJ = null,
                        carrierName = "",
                        grossWeight = "",
                        sapcodeDest = null,
                        freightPaidby = "",
                        messageStatus = "",
                        operationType = "",
                        sapcodeIssuer = null,
                        destinationCNPJ = "",
                        destinationName = "",
                        comexOperation_1 = "",
                        comexOperation_2 = null,
                        issuerVendorName = "",
                        complianceProcess = "",
                        documentAccessKey = "",
                        unileverOperation = "",
                        fiscalpointofCheck = "",
                        issuerCustomerName = "",
                        placeofdeliveryCNPJ = null,
                        placeofdeliveryName = "",
                        destinationVendorName = "",
                        intramunicipOperation = "",
                        destinationCustomerName = "",
                        documentAccessKeyStatus = null,
                        placeofdeliveryVendorName = "",
                        placeofdeliveryCustomerName = ""
                    });
                }

                return retorno;
            }
            foreach ((Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota, string mensagem, bool validado, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte) dynNota in listaNFes)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = dynNota.nota;
                string mensagem = dynNota.mensagem;

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidos = repPedidoNota.BuscarPorXMLNotaFiscal(notaFiscal.Codigo);

                retorno.Add(new ProductDocument()
                {
                    comments = "",
                    documentAccessKey = notaFiscal.Chave,
                    documentAccessKeyStatus = 0, // nao achei
                    issuerName = operacaoOcorrencia ? string.Empty : notaFiscal.Emitente?.Nome ?? string.Empty,
                    issuerCNPJ = operacaoOcorrencia ? string.Empty : notaFiscal.Emitente?.CPF_CNPJ.ToString() ?? string.Empty,
                    destinationName = operacaoOcorrencia ? string.Empty : notaFiscal.Destinatario?.Nome ?? string.Empty,
                    destinationCNPJ = operacaoOcorrencia ? string.Empty : notaFiscal.Destinatario?.CPF_CNPJ.ToString() ?? string.Empty,
                    number = notaFiscal.Numero,
                    operationType = operacaoOcorrencia ? string.Empty : notaFiscal.NaturezaOP ?? string.Empty,
                    grossWeight = operacaoOcorrencia ? string.Empty : Math.Round(notaFiscal.Peso, 2).ToString(),
                    netWeight = operacaoOcorrencia ? string.Empty : Math.Round(notaFiscal.PesoLiquido, 2).ToString(),
                    products = operacaoOcorrencia ? new List<Product>() : ObterProductsFilaH(repNotaProduto.BuscarPorNotaFiscal(notaFiscal.Codigo)),
                    carriers = ObterCarriersFilaH(cargaCTe.Carga, null),
                    serie = operacaoOcorrencia ? 0 : notaFiscal.Serie.ToInt(),
                    issuerCPF = notaFiscal.Emitente?.Tipo == "F" ? notaFiscal.Emitente?.CPF_CNPJ.ToString() : string.Empty,
                    issuerVendorName = operacaoOcorrencia ? string.Empty : notaFiscal.Emitente?.Nome ?? string.Empty,
                    issuerCustomerName = operacaoOcorrencia ? string.Empty : notaFiscal.Emitente?.NomeFantasia ?? string.Empty,
                    destinationVendorName = operacaoOcorrencia ? string.Empty : notaFiscal.Destinatario?.Nome ?? string.Empty,
                    destinationCustomerName = operacaoOcorrencia ? string.Empty : notaFiscal.Destinatario?.NomeFantasia ?? string.Empty,
                    carrierCNPJ = operacaoOcorrencia ? 0 : Utilidades.String.OnlyNumbers(notaFiscal.Empresa?.CNPJ ?? string.Empty).ToDouble(),
                    carrierName = operacaoOcorrencia ? string.Empty : notaFiscal.Empresa?.RazaoSocial ?? string.Empty,
                    placeofdeliveryCNPJ = 0,
                    placeofdeliveryName = "",
                    placeofdeliveryVendorName = "",
                    placeofdeliveryCustomerName = "",
                    unileverOperation = ObterUnileverOperationFilaH(notaFiscal),
                    fiscalpointofCheck = "",
                    complianceProcess = ObterComplianceProcessFilaH(notaFiscal),
                    messageStatus = mensagem,
                    comexOperation_1 = "",
                    comexOperation_2 = 0,
                    freightPaidby = "", // FALTA
                    sapcodeIssuer = new List<string>() { "" },
                    sapcodeDest = codigoType == 3 ? new List<string>() { notaFiscal?.Destinatario?.CodigoIntegracao.ObterSomenteNumeros() } : operacaoOcorrencia ? null : pedidos.Select(obj => obj.CargaPedido.Pedido?.Filial?.CodigoFilialEmbarcador).Distinct().ToList()
                });
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.Carrier> ObterCarriersFilaH(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.Carrier> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.Carrier>();

            if (ocorrencia != null)
            {
                retorno.Add(new Carrier()
                {
                    tNumber = ocorrencia.NumeroOcorrencia.ToString(),
                    partners = ObterCarrierPartnersFilaH(),
                    locations = ObterCarriersLocationsFilaH(carga, ocorrencia),
                    carrierCNPJ = ocorrencia.Emitente?.CNPJ ?? string.Empty,
                    carrierName = ocorrencia.Emitente?.RazaoSocial ?? string.Empty,
                    occurNumber = string.Empty,
                    statusforDt = "",
                    shipmentType = string.Empty,
                    carrierNumber = string.IsNullOrWhiteSpace(ocorrencia.Emitente?.CodigoIntegracao) ? string.Empty : ocorrencia.Emitente.CodigoIntegracao.ElementAt(0) == 'T' ? ocorrencia.Emitente.CodigoIntegracao.Remove(0, 1) : ocorrencia.Emitente.CodigoIntegracao,
                    plateNumber01 = string.Empty,
                    plateNumber02 = string.Empty,
                    plateNumber03 = string.Empty,
                    plateNumber04 = string.Empty,
                    vehicleStateUnload = ""
                });
                return retorno;
            }

            if (carga == null)
                return retorno;

            string codigoEnviar = string.Empty;

            if (!string.IsNullOrWhiteSpace(carga.Empresa?.CodigoIntegracao) && carga.Empresa.CodigoIntegracao.ElementAt(0) == 'T')
                codigoEnviar = carga.Empresa.CodigoIntegracao.Remove(0, 1);
            else
                codigoEnviar = carga.Empresa?.CodigoIntegracao ?? string.Empty;

            retorno.Add(new Carrier()
            {
                tNumber = carga.CargaRecebidaDeIntegracao ? carga.CodigoCargaEmbarcador : string.Empty,
                partners = ObterCarrierPartnersFilaH(),
                locations = ObterCarriersLocationsFilaH(carga, null),
                carrierCNPJ = carga.Empresa?.CNPJ ?? string.Empty,
                occurNumber = carga.CargaRecebidaDeIntegracao ? string.Empty : carga.Ocorrencias != null ? string.Join(", ", carga.Ocorrencias.Select(obj => obj.NumeroOcorrencia).ToList()) : string.Empty,
                statusforDt = "",
                shipmentType = carga.TipoDocumentoTransporte?.Descricao ?? string.Empty,
                carrierNumber = codigoEnviar,
                plateNumber01 = carga.Veiculo?.Placa ?? string.Empty,
                plateNumber02 = (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0) ? carga.VeiculosVinculados?.ElementAt(0)?.Placa ?? string.Empty : string.Empty,
                plateNumber03 = (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 1) ? carga.VeiculosVinculados?.ElementAt(1)?.Placa ?? string.Empty : string.Empty,
                plateNumber04 = (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 2) ? carga.VeiculosVinculados?.ElementAt(2)?.Placa ?? string.Empty : string.Empty,
                vehicleStateUnload = ""
            });

            return retorno;
        }

        private string ObterUnileverOperationFilaH(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            return repFilial.VerificarPorCNPJ(cte?.Destinatario?.CPF_CNPJ) && repFilial.VerificarPorCNPJ(cte?.Remetente?.CPF_CNPJ) ? "Y" : "N";
        }

        private List<string> ObterChekinMessageFilaH(List<FilaHOperationsResponse> operations)
        {
            List<string> retorno = new List<string>();
            foreach (FilaHOperationsResponse operation in operations)
            {
                if (operation.freightDocuments != null && operation.freightDocuments.Count > 0)
                {
                    foreach (FreightDocument dt in operation.freightDocuments)
                    {
                        if (!string.IsNullOrWhiteSpace(dt.comments))
                            retorno.Add((dt.comments.Substring(0, 2) == "DT" && dt.comments.IndexOf('-') > 0 ? "" : dt.documentNumber + " - ") + dt.comments);
                    }
                }
                if (operation.cteProductDocuments != null && operation.cteProductDocuments.Count > 0)
                {
                    foreach (CteProductDocument cte in operation.cteProductDocuments)
                    {
                        if (!string.IsNullOrWhiteSpace(cte.comments))
                            retorno.Add(cte.comments);
                    }
                }
            }
            return retorno.Distinct().ToList();
        }

        private string ObterUnileverOperationFilaH(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            return repFilial.VerificarPorCNPJ(notaFiscal.Destinatario?.CPF_CNPJ.ToString()) && repFilial.VerificarPorCNPJ(notaFiscal.Emitente?.CPF_CNPJ.ToString()) ? "Y" : "N";
        }

        private string ObterComplianceProcessFilaH(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);
            return repNaoConformidade.ExistePorNotasFiscais(cte.Documentos.Select(obj => obj.ChaveNFE).ToList()) ? "Y" : "N";
        }

        private string ObterComplianceProcessFilaH(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal)
        {
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);
            return repNaoConformidade.ExistePorNotaFiscal(notaFiscal.Chave) ? "Y" : "N";
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.ProductInfo> ObterProductInfoFilaH(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto produto, bool mostrarDados)
        {
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            List<Dominio.ObjetosDeValor.WebService.Carga.ProductInfo> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.ProductInfo>();
            if (!mostrarDados)
            {
                retorno.Add(new ProductInfo()
                {
                    supplierproductUOM = "",
                    unileverproductUOM = "",
                    supplierproductQtty = 0,
                    unileverproductCode = "",
                    unileverproductQtty = 0,
                    unileverproductDescription = "",
                    unileverPlantforMaterialCode = ""
                });
                return retorno;
            }
            retorno.Add(new ProductInfo()
            {
                supplierproductUOM = produto.Produto?.Unidade?.Sigla ?? string.Empty,
                unileverproductUOM = produto.Produto?.Unidade?.Sigla ?? string.Empty,
                supplierproductQtty = 0,
                unileverproductCode = produto.Produto?.CodigoProdutoEmbarcador ?? string.Empty,
                unileverproductQtty = Math.Round(produto.Quantidade, 2),
                unileverproductDescription = produto.Produto?.Descricao ?? string.Empty,
                unileverPlantforMaterialCode = produto.Produto?.Filiais != null ? string.Join(", ", produto.Produto?.Filiais) : string.Empty
            });
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.Product> ObterProductsFilaH(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtosNota)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.Product> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.Product>();
            if (produtosNota == null || produtosNota.Count == 0)
            {
                retorno.Add(new Product()
                {
                    products = ObterProductInfoFilaH(null, false),
                    productCode = "",
                    productWeight = "",
                    productcQuantity = "",
                    productDescription = "",
                    productMeasureUnit = "",
                    productWeightMeasureUnit = ""
                });
                return retorno;
            }
            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto produto in produtosNota)
            {
                retorno.Add(new Product()
                {
                    products = ObterProductInfoFilaH(produto, true),
                    productCode = produto.cProd ?? string.Empty,
                    productWeight = Math.Round(produto.PesoProduto, 2).ToString(),
                    productcQuantity = Math.Round(produto.Quantidade, 2).ToString(),
                    productDescription = produto.ProdutoInterno?.DescricaoNotaFiscal ?? string.Empty,
                    productMeasureUnit = produto.UnidadeMedida ?? string.Empty,
                    productWeightMeasureUnit = produto.UnidadeMedida ?? string.Empty
                });
            }


            return retorno;
        }

        private string ObterCodigoCargaEmbarcadorFormatadoFilaH(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string retorno = string.Empty;
            if (!string.IsNullOrWhiteSpace(carga.CodigoCargaEmbarcador) && carga.CodigoCargaEmbarcador.ElementAt(0) == '0' && carga.CodigoCargaEmbarcador.ElementAt(1) == '0')
                retorno = string.Concat("DT", carga.CodigoCargaEmbarcador.Remove(0, 2));
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.Location> ObterCarriersLocationsFilaH(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.Location> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.Location>();
            retorno.Add(new Location()
            {
                OccurDest = "",
                OccurDestSAP = "",
                SapVehicleTypeUnload = carga.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty,
                ShipmentLegindicator = string.Empty, // VALIDAR
                UnileverLoadingLocation = "",
                UniveleverReceivingLocation = ""
            });

            return retorno;
        }

        private List<(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, string mensagem, bool validado, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)> ValidarNotasFiscaisFilaH(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasRecebidas, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasEsperadas, bool realizarValidacaoCarga)
        {
            //Nem sei pq existe este metodo rever depois
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(_unitOfWork);

            List<(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, string mensagem, bool validado, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)> retorno = new List<(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, string mensagem, bool validado, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)>();
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            notasFiscais.AddRange(notasRecebidas);
            notasFiscais.AddRange(notasEsperadas);
            notasFiscais = notasFiscais.Distinct().ToList();



            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota in notasFiscais)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChaveNFe(nota.Chave);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> naoConformidades = repNaoConformidade.BuscarPorNotaFiscal(nota.Codigo);
                if (!realizarValidacaoCarga)
                {
                    retorno.Add((nota, $"{nota.Chave} - Unitização realizada com sucesso.", true, cte));
                    continue;
                }

                if (cte == null)
                {
                    retorno.Add((nota, $"{nota.Chave} - Unitização não realizada, checkin de frete não realizado.", false, cte));
                    continue;
                }

                //if (cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Cancelada)
                //{
                //    retorno.Add((nota, $"{nota.Chave} / {cte.Numero} - Unitização não realizada, CTe cancelado.", false, cte));
                //    continue;
                //}

                //if (!notasEsperadas.Contains(nota))
                //{
                //    retorno.Add((nota, $"{nota.Chave} / {cte.Numero} - Unitização não realizada, nota não consta na Carga.", false, cte));
                //    continue;
                //}

                //if (!notasRecebidas.Contains(nota))
                //{
                //    retorno.Add((nota, $"{nota.Chave} / {cte.Numero} - Unitização não realizada, nota faltante.", false, cte));
                //    continue;
                //}
                if (naoConformidades != null && naoConformidades.Count > 0)
                {
                    retorno.Add((nota, string.Join(";", naoConformidades.Select(obj => obj.ItemNaoConformidade?.Descricao ?? obj.ItemNaoConformidade.Descricao).Distinct().ToList()), false, cte));
                    continue;
                }

                retorno.Add((nota, $"{nota.Chave} / {cte.Numero} - Unitização realizada com sucesso, checkin de frete realizado com sucesso.", true, cte));
                continue;
            }



            return retorno;
        }

        private List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> ObterAgrupamentoRelatorioCargaParadas()
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Filial", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Carga", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "TipoOperacao", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataCriacaoCargaFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Transportador", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Motoristas", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Placas", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "SituacaoEntregaDescricao", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "OrdemPrevista", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "OrdemExecutada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Aderencia", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "CPFCNPJClienteFormatado", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Cliente", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Endereco", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Bairro", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Cidade", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Estado", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "CEP", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataChegadaClienteFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataSaidaClienteFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "TempoPermanencia", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataEntregaFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataSaidaClienteFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "TempoPermanencia", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataEntregaFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "NotasFiscais", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "Pedidos", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "PesoBruto", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataHoraAvaliacaoFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "ResultadoAvaliacao", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "MotivoAvaliacao", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "ObservacaoAvaliacao", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "EntregaForaDoRaioFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "TipoParadaFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "ValorTotalNotas", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "KMPlanejado", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "KMRealizado", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataInicioViagemFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "EntregaForaDoRaioFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataConfirmacaoChegadaFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataInicioCarregamentoFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataInicioDescargaFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataTerminoDescargaFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataFimViagemFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "DataCarregamentoFormatada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "ConfirmacaoViaApp", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "EncerramentoManualViagemFormatado", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "CodigoIntegracaoCliente", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "ModeloVeicular", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "ProtocoloIntegracaoCarga", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "ProtocoloPedido", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "PrevisaoChegada", CodigoDinamico = 0},
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento {Propriedade = "PrevisaoChegadaRecalculadaETA", CodigoDinamico = 0},
            };

            return agrupamentos;
        }

        private void ValidarPeriodos(Periodo periodos)
        {
            if (periodos.DataCriacaoInicial == DateTime.MinValue && periodos.DataCriacaoFinal == DateTime.MinValue && periodos.DataCarregamentoInicial == DateTime.MinValue && periodos.DataCarregamentoFinal == DateTime.MinValue)
                throw new WebServiceException("Nenhum período informado!");

            if ((periodos.DataCriacaoInicial > periodos.DataCriacaoFinal) || periodos.DataCarregamentoInicial > periodos.DataCarregamentoFinal)
                throw new WebServiceException("Período inválido!");

            if (periodos.DataCriacaoInicial.DifferenceOfDaysBetween(periodos.DataCriacaoFinal) > 30 || periodos.DataCarregamentoInicial.DifferenceOfDaysBetween(periodos.DataCarregamentoFinal) > 30)
                throw new WebServiceException("Período não pode ser maior que 30 dias!");

        }

        private void ReenviarCargaIntegracaoEvento(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoEvento repositorioCargaIntegracaoEvento = new(_unitOfWork);
            List<CargaIntegracaoEvento> integracoes = repositorioCargaIntegracaoEvento.ListarPorCargaParaReenvio(codigoCarga);
            foreach (CargaIntegracaoEvento integracao in integracoes)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.EnvioSucesso = true;
                integracao.Mensagem = "Salvar DT processado com sucesso.";
                repositorioCargaIntegracaoEvento.Atualizar(integracao);
            }
        }

        private decimal BuscarValorTotalComponentes(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            decimal valorTotalComponentes = 0;

            if (carga.Componentes.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in carga.Componentes)
                {
                    if (componente.TipoComponenteFrete != TipoComponenteFrete.ISS && componente.TipoComponenteFrete != TipoComponenteFrete.ICMS)
                    {
                        valorTotalComponentes += componente.ValorComponente;
                    }
                }
            }

            return valorTotalComponentes;
        }

        private decimal BuscarValorTotalDescontarComponentesTabelaFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete)
        {
            decimal valorTotalComponentes = 0;

            if (carga.Componentes.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in carga.Componentes)
                {
                    if (componenteFrete == componente.ComponenteFrete)
                        valorTotalComponentes += componente.ValorComponente;
                }
            }

            return valorTotalComponentes;
        }

        private async Task ValidarInformacoesCarregamentoRedespacho(List<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento> carregamentosRedespacho)
        {
            foreach (var carregamentoRedespacho in carregamentosRedespacho)
            {
                if (string.IsNullOrWhiteSpace(carregamentoRedespacho.NumeroCarregamento))
                    throw new ServicoException("Prencha o número do carregamento de redespacho.");

                try
                {
                    await ValidarDadosParaGerarCarregamentoAsync(carregamentoRedespacho);
                }
                catch (BaseException ex)
                {
                    throw new ServicoException(ex.Message + $" Carregamento {carregamentoRedespacho.NumeroCarregamento}");
                }
            }
        }

        #endregion Métodos Privados - Dados FilaH
    }
}
