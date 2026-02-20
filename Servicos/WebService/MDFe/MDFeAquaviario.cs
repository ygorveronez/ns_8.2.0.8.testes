using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.WebService;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.WebService.MDFe
{
    public class MDFeAquaviario : ServicoWebServiceBase
    {
        Repositorio.UnitOfWork _unitOfWork;
        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        TipoServicoMultisoftware _tipoServicoMultisoftware;
        AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        protected string _adminStringConexao;


        public MDFeAquaviario(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { _unitOfWork = unitOfWork; }
        public MDFeAquaviario(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
        }
        public MDFeAquaviario(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }
        

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> EnviarMDFeAquaviario(Dominio.ObjetosDeValor.WebService.MDFe.MDFeAquaviario mdfeAquaviario)
        {
            Servicos.Log.TratarErro($"EnviarMDFeAquaviario: {(mdfeAquaviario != null ? Newtonsoft.Json.JsonConvert.SerializeObject(mdfeAquaviario) : string.Empty)}", "Request");
            StringBuilder mensagemErro = new StringBuilder();

            try
            {
                StringBuilder stMensagem = new StringBuilder();

                Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork);

                if (repMDFe.ContemMDFePorChave(mdfeAquaviario.MDFe.Chave))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Já existe um MDF-e inserido na base com a chave informada " + mdfeAquaviario.MDFe.Chave, false);

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = new CargaMDFeManual();
                cargaMDFeManual.MDFeRecebidoDeIntegracao = true;
                cargaMDFeManual.MDFeImportado = true;
                cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmDigitacao;
                cargaMDFeManual.TipoModalMDFe = mdfeAquaviario.TipoModalMDFe;
                cargaMDFeManual.PortoOrigem = serPedidoWS.SalvarPorto(mdfeAquaviario.PortoEmbarque, ref stMensagem, _auditado);
                cargaMDFeManual.PortoDestino = serPedidoWS.SalvarPorto(mdfeAquaviario.PortoDesembarque, ref stMensagem, _auditado);
                cargaMDFeManual.Destino = repLocalidade.BuscarPorCodigoIBGE(mdfeAquaviario.Destino?.IBGE ?? 0);
                cargaMDFeManual.PedidoViagemNavio = serPedidoWS.SalvarViagem(mdfeAquaviario.Viagem, ref stMensagem, _auditado, false);
                cargaMDFeManual.Origem = repLocalidade.BuscarPorCodigoIBGE(mdfeAquaviario.Origem?.IBGE ?? 0);
                cargaMDFeManual.Empresa = repEmpresa.BuscarPorCNPJ(mdfeAquaviario.TransportadoraEmitente.CNPJ);
                cargaMDFeManual.UsarDadosCTe = mdfeAquaviario.UsarDadosCTe;
                cargaMDFeManual.UsarSeguroCTe = mdfeAquaviario.UsarSeguroCTe;
                cargaMDFeManual.Reboques = new List<Dominio.Entidades.Veiculo>();

                if (stMensagem.Length > 0)
                {
                    Servicos.Log.TratarErro($"Falha ao salvar o MDF-e Aquaviario: {stMensagem.ToString()}");
                    _unitOfWork.Rollback();
                    return Retorno<bool>.CriarRetornoDadosInvalidos(stMensagem.ToString(), false);
                }

                repCargaMDFeManual.Inserir(cargaMDFeManual);

                cargaMDFeManual.TerminalCarregamento = new List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
                if (mdfeAquaviario.TerminaisOrigem != null && mdfeAquaviario.TerminaisOrigem.Count > 0)
                {
                    foreach (var terminalCarregamento in mdfeAquaviario.TerminaisOrigem)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = serPedidoWS.SalvarTerminalPorto(terminalCarregamento, ref stMensagem, _auditado);

                        if (stMensagem.Length > 0)
                        {
                            Servicos.Log.TratarErro($"Falha ao salvar o MDF-e Aquaviario: {stMensagem.ToString()}");
                            _unitOfWork.Rollback();
                            return Retorno<bool>.CriarRetornoDadosInvalidos(stMensagem.ToString(), false);
                        }

                        cargaMDFeManual.TerminalCarregamento.Add(terminal);
                    }
                }

                cargaMDFeManual.TerminalDescarregamento = new List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
                if (mdfeAquaviario.TerminaisDestino != null && mdfeAquaviario.TerminaisDestino.Count > 0)
                {
                    foreach (var terminalDescarregamento in mdfeAquaviario.TerminaisDestino)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = serPedidoWS.SalvarTerminalPorto(terminalDescarregamento, ref stMensagem, _auditado);

                        if (stMensagem.Length > 0)
                        {
                            Servicos.Log.TratarErro($"Falha ao salvar o MDF-e Aquaviario: {stMensagem.ToString()}");
                            _unitOfWork.Rollback();
                            return Retorno<bool>.CriarRetornoDadosInvalidos(stMensagem.ToString(), false);
                        }

                        cargaMDFeManual.TerminalDescarregamento.Add(terminal);
                    }
                }

                cargaMDFeManual.CTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                if (mdfeAquaviario.MDFe != null && mdfeAquaviario.MDFe.ChavesDeCTe != null && mdfeAquaviario.MDFe.ChavesDeCTe.Count > 0)
                {
                    foreach (string chaveCTe in mdfeAquaviario.MDFe.ChavesDeCTe)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorChaveCTe(chaveCTe);
                        if (cargaCTe == null)
                        {
                            _unitOfWork.Rollback();
                            return Retorno<bool>.CriarRetornoDadosInvalidos("CT-e de chave " + chaveCTe + " não localizado na base.", false);
                        }
                        if (!cargaMDFeManual.CTes.Any(o => o.Codigo == cargaCTe.Codigo))
                            cargaMDFeManual.CTes.Add(cargaCTe);
                    }
                }

                repCargaMDFeManual.Atualizar(cargaMDFeManual);

                if ((cargaMDFeManual.CTes == null || cargaMDFeManual.CTes.Count <= 0) && (cargaMDFeManual.Cargas == null || cargaMDFeManual.Cargas.Count <= 0))
                {
                    _unitOfWork.Rollback();
                    return Retorno<bool>.CriarRetornoDadosInvalidos("É necessário adicionar ao menos um CT-e ou uma Carga para salvar o MDF-e manual.", false);
                }
                if (cargaMDFeManual.TerminalCarregamento == null || cargaMDFeManual.TerminalCarregamento.Count() == 0)
                {
                    _unitOfWork.Rollback();
                    return Retorno<bool>.CriarRetornoDadosInvalidos("É necessário adicionar ao menos um terminal de carregamento para salvar o MDF-e manual.", false);
                }
                if (cargaMDFeManual.PedidoViagemNavio == null)
                {
                    _unitOfWork.Rollback();
                    return Retorno<bool>.CriarRetornoDadosInvalidos("É necessário informar um Navio/Viagem/Direção para salvar o MDF-e manual.", false);
                }

                if (!GerarMDFe(out string erro, cargaMDFeManual.Codigo, mdfeAquaviario, _unitOfWork))
                {
                    _unitOfWork.Rollback();
                    return Retorno<bool>.CriarRetornoDadosInvalidos(erro, false);
                }

                serCargaDadosSumarizados.AtualizarDadosMDFeAquaviario(cargaMDFeManual.Codigo, _unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaMDFeManual, null, "Recebeu a MDF-e pela integração.", _unitOfWork);

                _unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();
                ArmazenarLogIntegracao(mdfeAquaviario, _unitOfWork);
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                ArmazenarLogIntegracao(mdfeAquaviario, _unitOfWork);
                return Retorno<bool>.CriarRetornoDadosInvalidos($"Ocorreu uma falha ao obter os dados das integrações. {mensagemErro.ToString()}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AtualizarSituacaoMDFeAquaviario(string chaveMDFe, Dominio.Enumeradores.StatusMDFe statusMDFe, string protocolo, DateTime dataEvento, string mensagemRetornoSefaz, string motivo)
        {
            StringBuilder mensagemErro = new StringBuilder();

            try
            {
                StringBuilder stMensagem = new StringBuilder();

                Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento repCargaMDFeManualCancelamento = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento(_unitOfWork);

                if (string.IsNullOrWhiteSpace(chaveMDFe))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Favor informe a chave do MDF-e ", false);

                if (string.IsNullOrWhiteSpace(protocolo))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Favor informe o protocolo ", false);

                if (dataEvento <= DateTime.MinValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Favor informe o a data do retorno", false);

                if (statusMDFe != Dominio.Enumeradores.StatusMDFe.Cancelado && statusMDFe != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Só é permitido alterar o status para Cancelado ou Encerrado", false);

                if (!repMDFe.ContemMDFePorChave(chaveMDFe))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi encontrado nenhum MDF-e com a chave informada " + chaveMDFe, false);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorChave(chaveMDFe);
                if (mdfe == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi encontrado nenhum MDF-e com a chave informada " + chaveMDFe, false);

                _unitOfWork.Start();

                mdfe.Status = statusMDFe;
                if (statusMDFe == Dominio.Enumeradores.StatusMDFe.Cancelado)
                {
                    mdfe.ProtocoloCancelamento = protocolo;
                    mdfe.DataCancelamento = dataEvento;
                    mdfe.DataIntegracao = dataEvento;
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.Cancelado;
                    mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(mensagemRetornoSefaz);
                    mdfe.Log += $" / Cancelamento importado via WS com sucesso em {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}.";
                    mdfe.JustificativaCancelamento = motivo;

                    Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento();
                    cargaMDFeManualCancelamento.CargaMDFeManual = repCargaMDFeManual.BuscarPorMDFe(mdfe.Codigo);
                    if (cargaMDFeManualCancelamento.CargaMDFeManual != null)
                    {
                        cargaMDFeManualCancelamento.DataCancelamento = DateTime.Now;
                        cargaMDFeManualCancelamento.MotivoCancelamento = motivo;
                        cargaMDFeManualCancelamento.Usuario = repUsuario.BuscarPrimeiro();
                        cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.EmCancelamento;
                        cargaMDFeManualCancelamento.RecebidoPorIntegracao = true;

                        repCargaMDFeManualCancelamento.Inserir(cargaMDFeManualCancelamento, _auditado);
                    }
                }
                else
                {
                    mdfe.ProtocoloEncerramento = protocolo;
                    mdfe.DataEncerramento = dataEvento;
                    mdfe.DataIntegracao = dataEvento;
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.Encerrado;
                    mdfe.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(mensagemRetornoSefaz);
                    mdfe.Log += $" / Encerramento importado com sucesso em {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}.";
                }

                repMDFe.Atualizar(mdfe);

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
                return Retorno<bool>.CriarRetornoDadosInvalidos($"Ocorreu uma falha ao obter os dados das integrações. {mensagemErro.ToString()}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }


        #endregion

        #region Métodos Privados

        private bool GerarMDFe(out string erro, int codigoMDFeManual, Dominio.ObjetosDeValor.WebService.MDFe.MDFeAquaviario mdfeAquaviario, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoMDFeManual);

            erro = "";

            try
            {
                Dominio.ObjetosDeValor.PortoMDFeIntegracao portoEmbarque = null;
                if (cargaMDFeManual.PortoOrigem != null)
                {
                    portoEmbarque = new Dominio.ObjetosDeValor.PortoMDFeIntegracao()
                    {
                        Codigo = cargaMDFeManual.PortoOrigem.Codigo,
                        Descricao = cargaMDFeManual.PortoOrigem.Descricao
                    };
                }

                Dominio.ObjetosDeValor.PortoMDFeIntegracao portoDesembarque = null;
                if (cargaMDFeManual.PortoDestino != null)
                {
                    portoDesembarque = new Dominio.ObjetosDeValor.PortoMDFeIntegracao()
                    {
                        Codigo = cargaMDFeManual.PortoDestino.Codigo,
                        Descricao = cargaMDFeManual.PortoDestino.Descricao
                    };
                }

                Dominio.ObjetosDeValor.PedidoViagemNavioMDFeIntegracao viagem = null;
                if (cargaMDFeManual.PedidoViagemNavio != null)
                {
                    viagem = new Dominio.ObjetosDeValor.PedidoViagemNavioMDFeIntegracao()
                    {
                        Codigo = cargaMDFeManual.PedidoViagemNavio.Codigo,
                        Descricao = cargaMDFeManual.PedidoViagemNavio.Descricao
                    };
                }

                List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisCarregamento = new List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao>();
                if (cargaMDFeManual.TerminalCarregamento != null && cargaMDFeManual.TerminalCarregamento.Count > 0)
                {
                    foreach (var terminal in cargaMDFeManual.TerminalCarregamento)
                    {
                        terminaisCarregamento.Add(new Dominio.ObjetosDeValor.TerminalMDFeIntegracao()
                        {
                            Codigo = terminal.Codigo,
                            Descricao = terminal.Descricao
                        }
                        );
                    }
                }

                List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao> terminaisDescarregamento = new List<Dominio.ObjetosDeValor.TerminalMDFeIntegracao>();
                if (cargaMDFeManual.TerminalDescarregamento != null && cargaMDFeManual.TerminalDescarregamento.Count > 0)
                {
                    foreach (var terminal in cargaMDFeManual.TerminalDescarregamento)
                    {
                        terminaisDescarregamento.Add(new Dominio.ObjetosDeValor.TerminalMDFeIntegracao()
                        {
                            Codigo = terminal.Codigo,
                            Descricao = terminal.Descricao
                        }
                        );
                    }
                }

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                if (cargaMDFeManual.CTes != null && cargaMDFeManual.CTes.Count > 0)
                    ctes = cargaMDFeManual.CTes.Select(c => c.CTe).ToList();

                //ctes nullos
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = svcMDFe.GerarMDFeImportadoPorCTes(mdfeAquaviario, cargaMDFeManual.Empresa, ctes, unidadeTrabalho, null, null, "", null, cargaMDFeManual.Origem, cargaMDFeManual.Destino, 0, "", null, null, null, null, true, null, null, portoEmbarque, portoDesembarque, viagem, terminaisCarregamento, terminaisDescarregamento, configuracaoTMS);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe();
                cargaMDFeManualMDFe.CargaMDFeManual = cargaMDFeManual;
                cargaMDFeManualMDFe.MDFe = mdfe;

                repCargaMDFeManualMDFe.Inserir(cargaMDFeManualMDFe);

                cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmEmissao;
                repCargaMDFeManual.Atualizar(cargaMDFeManual);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }

            return true;

        }

        #endregion
    }
}