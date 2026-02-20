using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using System;
using System.Linq;

namespace Servicos.WebService.Ocorrencia
{
    public class CargaOcorrencia : ServicoWebServiceBase
    {
        Repositorio.UnitOfWork _unitOfWork;
        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        TipoServicoMultisoftware _tipoServicoMultisoftware;
        AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        protected string _adminStringConexao;

        public CargaOcorrencia(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { _unitOfWork = unitOfWork; }
        public CargaOcorrencia(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
        }
        public CargaOcorrencia(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        public CargaOcorrencia() : base() { }

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<int> EnviarOcorrencia(Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti ocorrenciaIntegracao)
        {
            Servicos.Log.TratarErro($"EnviarOcorrencia: {(ocorrenciaIntegracao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(ocorrenciaIntegracao) : string.Empty)}", "Request");

            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(_unitOfWork);
            Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(_unitOfWork);
            Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(_unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(_unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOocrrenciaCTe = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);

            try
            {
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoOocrrenciaCTe.BuscarPorCodigoIntegracao(ocorrenciaIntegracao.TipoOcorrencia?.CodigoIntegracao ?? "");

                if (tipoOcorrencia == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoDadosInvalidos("O tipo de ocorrência com o código de integração '" + (ocorrenciaIntegracao.TipoOcorrencia?.CodigoIntegracao ?? "") + "' não foi encontrado.", 0);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(ocorrenciaIntegracao.ProtocoloCarga);

                if (ocorrenciaIntegracao.ProtocoloCarga == 0 || carga == null)
                    return Retorno<int>.CriarRetornoDadosInvalidos("Carga com protocolo: " + ocorrenciaIntegracao.ProtocoloCarga + " não foi encontrada.", 0);

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia
                {
                    Carga = carga,
                    DataAlteracao = DateTime.Now,
                    DataOcorrencia = ocorrenciaIntegracao.DataOcorrencia,
                    DataFinalizacaoEmissaoOcorrencia = ocorrenciaIntegracao.DataOcorrencia,
                    NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork),
                    Observacao = "Ocorrência gerada automaticamente pela integração.",
                    SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgInformacoes,
                    ValorOcorrencia = ocorrenciaIntegracao.Conhecimentos.Sum(x => x.ValorFrete.ValorTotalAReceber),
                    ValorOcorrenciaOriginal = ocorrenciaIntegracao.Conhecimentos.Sum(x => x.ValorFrete.ValorTotalAReceber),
                    ObservacaoCTe = !string.IsNullOrWhiteSpace(ocorrenciaIntegracao?.Observacao) ? ocorrenciaIntegracao?.Observacao : "Gerada pela integração",
                    CTeEmitidoNoEmbarcador = true,
                    TipoOcorrencia = tipoOcorrencia,
                    ValorOcorrenciaLiquida = ocorrenciaIntegracao.Conhecimentos.Sum(x => x.ValorFrete.ValorTotalAReceber),
                    IncluirICMSFrete = false,
                    PrioridadeAprovacaoAtualEtapaAprovacao = 0,
                    PrioridadeAprovacaoAtualEtapaEmissao = 0,
                    Inativa = false,
                    OcorrenciaRecebidaDeIntegracao = true
                };
                repOcorrencia.Inserir(cargaOcorrencia, _auditado);

                foreach (Dominio.ObjetosDeValor.WebService.CTe.CTe cte in ocorrenciaIntegracao.Conhecimentos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCte.BuscarPorChaveCTe(cte.ChaveCTeVinculado);

                    if (cargaCTe == null)
                    {
                        _unitOfWork.Rollback();
                        return Retorno<int>.CriarRetornoDadosInvalidos("CT -e original não encontrado: " + cte.ChaveCTeVinculado, 0);
                    }

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOcorrencia = repCTe.BuscarPorChave(cte.Chave);

                    if (cteOcorrencia == null)
                    {
                        if (string.IsNullOrWhiteSpace(cte.XMLAutorizacao))
                        {
                            _unitOfWork.Rollback();
                            return Retorno<int>.CriarRetornoDadosInvalidos($"O CT-e {cte.Numero} não possui um XML.", 0);
                        }

                        System.IO.MemoryStream memoryStream = Utilidades.String.ToStream(cte.XMLAutorizacao);

                        object retornoInserir = svcCTe.GerarCTeAnterior(memoryStream, carga.Empresa.Codigo, string.Empty, string.Empty, null, null, true, false, _tipoServicoMultisoftware, true);

                        if (retornoInserir.GetType() == typeof(string))
                        {
                            _unitOfWork.Rollback();
                            return Retorno<int>.CriarRetornoDadosInvalidos((string)retornoInserir, 0);
                        }

                        cteOcorrencia = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retornoInserir;
                    }

                    cargaOcorrencia = repOcorrencia.BuscarPorCodigo(cargaOcorrencia.Codigo);

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento
                    {
                        CargaCTe = cargaCTe,
                        CargaOcorrencia = cargaOcorrencia,
                        CTeImportado = cteOcorrencia
                    };
                    repCargaOcorrenciaDocumento.Inserir(cargaOcorrenciaDocumento);

                    if (cteOcorrencia.Codigo > 0)
                    {
                        cteOcorrencia = repCTe.BuscarPorCodigo(cteOcorrencia.Codigo);
                        cteOcorrencia.CTeSemCarga = false;
                        repCTe.Atualizar(cteOcorrencia);
                    }

                    Dominio.Entidades.OcorrenciaDeCTe ocorrenciaDeCTe = srvOcorrencia.GerarOcorrenciaCTe(cargaOcorrencia, cargaCTe, _unitOfWork);

                    cargaOcorrenciaDocumento.OcorrenciaDeCTe = ocorrenciaDeCTe;
                    cargaCTe.CTe.PossuiCTeComplementar = true;

                    repCTe.Atualizar(cargaCTe.CTe);
                    repCargaOcorrenciaDocumento.Atualizar(cargaOcorrenciaDocumento);

                    if (!srvOcorrencia.AjustarCTeImportado(out string erro, cteOcorrencia, cargaCTe.Carga, tipoOcorrencia?.ComponenteFrete, _unitOfWork))
                        throw new Exception(erro);
                }

                if (tipoOcorrencia != null && tipoOcorrencia.ComponenteFrete != null)
                {
                    cargaOcorrencia.TipoOcorrencia = tipoOcorrencia;
                    cargaOcorrencia.OrigemOcorrencia = tipoOcorrencia.OrigemOcorrencia;
                    cargaOcorrencia.ComponenteFrete = tipoOcorrencia.ComponenteFrete;
                    cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar;

                    if (tipoOcorrencia.ComponenteFrete.SomarComponenteFreteLiquido)
                        cargaOcorrencia.ValorOcorrenciaLiquida = cargaOcorrencia.ValorOcorrencia;
                    else
                        cargaOcorrencia.ValorOcorrenciaLiquida = 0;

                    repOcorrencia.Atualizar(cargaOcorrencia);

                    serCargaCTeComplementar.ImportarCTesComplementaresParaOcorrencia(cargaOcorrencia, _unitOfWork, _tipoServicoMultisoftware);

                    Servicos.Embarcador.Integracao.IntegracaoEDI.AdicionarEDIParaIntegracao(cargaOcorrencia, false, _tipoServicoMultisoftware, _unitOfWork);

                    serOcorrencia.ValidarEnviarEmissaoComplementosOcorrencia(cargaOcorrencia, _unitOfWork);
                }
                else
                {
                    _unitOfWork.Rollback();
                    return Retorno<int>.CriarRetornoDadosInvalidos("Tipo ocorrência não encontrada pelo codigo integração: " + (ocorrenciaIntegracao.TipoOcorrencia?.CodigoIntegracao ?? ""), 0);
                }

                _unitOfWork.CommitChanges();

                return Retorno<int>.CriarRetornoSucesso(cargaOcorrencia.Codigo);

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                ArmazenarLogIntegracao(ocorrenciaIntegracao, _unitOfWork);
                return Retorno<int>.CriarRetornoDadosInvalidos(ex.Message, 0);
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> EnviarCancelamentoOcorrencia(Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento ocorrenciaCancelamento)
        {
            Servicos.Log.TratarErro($"EnviarCancelamentoOcorrencia: {(ocorrenciaCancelamento != null ? Newtonsoft.Json.JsonConvert.SerializeObject(ocorrenciaCancelamento) : string.Empty)}", "Request");

            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaIntegracaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(ocorrenciaCancelamento.ProtocoloOcorrecia);
                if (ocorrencia == null || ocorrenciaCancelamento.ProtocoloOcorrecia == 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Ocorrência não localizada", false);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento OcorrenciaCancelamento = repOcorrenciaCancelamento.BuscarPorOcorrencia(ocorrencia.Codigo);

                _unitOfWork.Start();

                if (OcorrenciaCancelamento == null)
                {
                    OcorrenciaCancelamento = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento()
                    {
                        Ocorrencia = ocorrencia,
                        DataCancelamento = ocorrenciaCancelamento.DataCancelamento,
                        MotivoCancelamento = $"Cancelamento importado pela integração ({ocorrenciaCancelamento.MotivoCancelamento})",
                        Situacao = SituacaoCancelamentoOcorrencia.EmCancelamento,
                        Tipo = TipoCancelamentoOcorrencia.Cancelamento,
                        SituacaoOcorrenciaNoCancelamento = ocorrencia?.SituacaoOcorrenciaNoCancelamento ?? SituacaoOcorrencia.Finalizada
                    };

                    repOcorrenciaCancelamento.Inserir(OcorrenciaCancelamento, _auditado);

                    foreach (var cte in ocorrenciaCancelamento.Conhecimentos)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(cte.Chave);

                        if (conhecimento != null)// && (cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Cancelada || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Anulado || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.AnuladoGerencialmente || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Inutilizada))
                        {
                            DateTime dataCancelamento;
                            DateTime.TryParse(cte.DataCancelamento, out dataCancelamento);
                            conhecimento.Cancelado = "S";
                            conhecimento.Status = cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Cancelada ? "C" : cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Anulado || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.AnuladoGerencialmente ? "Z" : cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Inutilizada ? "I" : "C";
                            conhecimento.ProtocoloCancelamentoInutilizacao = !string.IsNullOrWhiteSpace(cte.ProtocoloCancelamentoInutilizacao) ? cte.ProtocoloCancelamentoInutilizacao : "CANCIMPORTADO";
                            conhecimento.Log += string.Concat(" / CT-e de cancelamento importado com sucesso em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                            conhecimento.DataRetornoSefaz = dataCancelamento > DateTime.MinValue ? dataCancelamento : DateTime.Now;
                            conhecimento.DataCancelamento = dataCancelamento > DateTime.MinValue ? dataCancelamento : DateTime.Now;
                            conhecimento.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(cte.MensagemRetornoSefaz);

                            repCTe.Atualizar(conhecimento);
                        }
                    }
                }

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                ArmazenarLogIntegracao(ocorrenciaCancelamento, _unitOfWork);
                return Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message, 0);
            }
        }


        #endregion
    }
}


