using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public class Pagbem
    {
        #region Métodos Globais 

        public void IntegrarANTTCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            string mensagemErro = string.Empty;
            string jsonEnvio = string.Empty;
            string jsonRetorno = string.Empty;

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
            cargaDadosTransporteIntegracao.NumeroTentativas++;

            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = repConfiguracaoCIOT.BuscarPrimeiroPorOperadora(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem);

            if (configuracaoCIOT == null)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Sem configuração do CIOT.";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }

            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = ObterConfiguracaoPagbem(configuracaoCIOT, unitOfWork);
            string token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagemErro;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }

            if (cargaDadosTransporteIntegracao.Carga.Veiculo == null || cargaDadosTransporteIntegracao.Carga.Veiculo.Proprietario == null)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "A carga não é de terceiro.";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(cargaDadosTransporteIntegracao.Carga.Veiculo.Proprietario, unitOfWork);
            string rntrc = !string.IsNullOrEmpty(modalidadeTerceiro?.RNTRC) ? modalidadeTerceiro.RNTRC : cargaDadosTransporteIntegracao.Carga.Veiculo != null && !string.IsNullOrWhiteSpace(cargaDadosTransporteIntegracao.Carga.Veiculo.RNTRC.ToString("n0")) ? Utilidades.String.OnlyNumbers(cargaDadosTransporteIntegracao.Carga.Veiculo.RNTRC.ToString("n0")) : "";
            if (string.IsNullOrWhiteSpace(rntrc))
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "ANTT não configurada no terceiro.";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }

            if (!this.ValidarRNTRCContratado(cargaDadosTransporteIntegracao.Carga.Veiculo.Proprietario, cargaDadosTransporteIntegracao.Carga.Veiculo, cargaDadosTransporteIntegracao.Carga.Empresa, configuracao, rntrc, token, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno, true))
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagemErro;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                cargaDadosTransporteIntegracao.Carga.MensagemProblemaIntegracaoGrMotoristaVeiculo = "ANTT:" + mensagemErro;
                cargaDadosTransporteIntegracao.Carga.ProblemaIntegracaoGrMotoristaVeiculo = true;
                cargaDadosTransporteIntegracao.Carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo = false;
                repCarga.Atualizar(cargaDadosTransporteIntegracao.Carga);
            }
            else
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Consulta realizada com sucesso.";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.Protocolo = "";

                if (repCargaPedido.PossuiIntegracaoPedido(cargaDadosTransporteIntegracao.Carga.Codigo))
                {
                    AdicionarIntegracaoCarga(cargaDadosTransporteIntegracao.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy, unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = repCargaIntegracao.BuscarPorCargaETipo(cargaDadosTransporteIntegracao.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy);
                    if (cargaIntegracao != null)
                        AdicionarCargaDadosTransporteParaIntegracao(cargaDadosTransporteIntegracao.Carga, cargaIntegracao.TipoIntegracao, unitOfWork);
                }
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                Mensagem = cargaDadosTransporteIntegracao.ProblemaIntegracao,
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonEnvio, "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Data = cargaDadosTransporteIntegracao.DataIntegracao,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public SituacaoRetornoCIOT IntegrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = ObterConfiguracaoPagbem(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            string mensagemErro = string.Empty;

            ciot.Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem;

            if (ciot.Contratante == null)
                ciot.Contratante = cargaCIOT.Carga.Empresa;

            if (ciot.Motorista == null)
            {
                Dominio.Entidades.Usuario veiculoMotorista = null;
                if (cargaCIOT.Carga.Veiculo != null)
                    veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(cargaCIOT.Carga.Veiculo.Codigo);

                ciot.Motorista = cargaCIOT.Carga.Motoristas != null && cargaCIOT.Carga.Motoristas.Count > 0 ? cargaCIOT.Carga.Motoristas.FirstOrDefault() : veiculoMotorista ?? null;
            }


            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);

            string token = ObterToken(configuracao, out mensagemErro);
            string rntrc = !string.IsNullOrEmpty(modalidadeTerceiro?.RNTRC) ? modalidadeTerceiro.RNTRC : cargaCIOT.Carga.Veiculo != null && !string.IsNullOrWhiteSpace(cargaCIOT.Carga.Veiculo.RNTRC.ToString("n0")) ? Utilidades.String.OnlyNumbers(cargaCIOT.Carga.Veiculo.RNTRC.ToString("n0")) : "";
            bool validarANTT = cargaCIOT?.Carga?.TipoOperacao?.PossuiIntegracaoANTT ?? false;

            bool sucesso = false;
            if (string.IsNullOrWhiteSpace(mensagemErro))
            {
                if (IntegrarContratado(ciot.Transportador, modalidadeTerceiro, ciot.Contratante, configuracao, token, unitOfWork, out mensagemErro) &&
                    IntegrarMotorista(ciot.Transportador, ciot.Motorista, ciot.Contratante, configuracao, token, unitOfWork, out mensagemErro) &&
                    IntegrarCartaoPagbem(ciot.Transportador, modalidadeTerceiro, ciot.Motorista, ciot.Contratante, configuracao, token, unitOfWork, out mensagemErro) &&
                    ValidarRNTRCContratado(ciot.Transportador, cargaCIOT?.Carga.Veiculo ?? ciot.Veiculo, ciot.Contratante, configuracao, rntrc, token, unitOfWork, out mensagemErro, out string jsonEnvio, out string jsonRetorno, validarANTT) &&
                    IntegrarContratoFrete(cargaCIOT, modalidadeTerceiro, string.Empty, string.Empty, configuracao, token, unitOfWork, out mensagemErro))
                    sucesso = true;
            }

            if (!sucesso)
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                ciot.Mensagem = mensagemErro;
            }
            else
            {
                if (configuracao.LiberarViagemManualmente)
                    ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem;
            }

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);

            return sucesso ? SituacaoRetornoCIOT.Autorizado : SituacaoRetornoCIOT.ProblemaIntegracao;
        }

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = ObterConfiguracaoPagbem(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            string token = ObterToken(configuracao, out mensagemErro);

            bool sucesso = false;
            if (string.IsNullOrWhiteSpace(mensagemErro))
            {
                if (IntegrarQuitacaoCIOT(ciot, ciot.Contratante, cargaCIOT, configuracao, token, unitOfWork, out mensagemErro))
                    sucesso = true;
            }

            if (!sucesso)
            {
                ciot.Mensagem = mensagemErro;
            }
            else
            {
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                cargaCIOT.CIOT.Mensagem = "Quitação realizada com sucesso.";
                cargaCIOT.CIOT.DataEncerramento = DateTime.Now;
            }

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);

            return sucesso;
        }

        public bool IntegrarMovimentoFinanceiro(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = ObterConfiguracaoPagbem(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(cargaCIOT.CIOT.Transportador, unitOfWork);

            string token = ObterToken(configuracao, out mensagemErro);

            bool sucesso = false;

            if (string.IsNullOrWhiteSpace(mensagemErro))
            {
                if (IntegrarAjusteFinanceiroCIOT(cargaCIOT.CIOT, modalidadeTerceiro, justificativa, valorMovimento, configuracao, token, out mensagemErro))
                    sucesso = true;
            }

            return sucesso;
        }

        public bool CancelarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = ObterConfiguracaoPagbem(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            string token = ObterToken(configuracao, out mensagemErro);

            if (IntegrarCancelamentoCIOT(ciot, ciot.Contratante, cargaCIOT, configuracao, token, unitOfWork, out mensagemErro))
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                ciot.DataCancelamento = DateTime.Now;
                ciot.Mensagem = mensagemErro;

                repCIOT.Atualizar(ciot);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void CancelarValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = repConfiguracaoCIOT.BuscarPrimeiroPorOperadora(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem);

            if (configuracaoCIOT == null)
                return;

            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = ObterConfiguracaoPagbem(configuracaoCIOT, unitOfWork);
            string mensagemErro = string.Empty;

            string token = ObterToken(configuracao, out mensagemErro);

            if (IntegrarCancelamentoValePedagio(cargaValePedagio, cargaValePedagio.Carga.Empresa, configuracao, token, unitOfWork, out mensagemErro))
            {
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada;
            }
            else
            {
                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada;
            }

            cargaValePedagio.ProblemaIntegracao = mensagemErro;

            cargaValePedagio.DataIntegracao = DateTime.Now;
            repCargaValePedagio.Atualizar(cargaValePedagio);
        }

        public void ConciliarCIOTs(DateTime dataConciliacao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = repConfiguracaoCIOT.BuscarPrimeiroPorOperadora(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem);

            if (configuracaoCIOT == null)
                return;

            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = ObterConfiguracaoPagbem(configuracaoCIOT, unitOfWork);

            if (configuracao == null)
                return;

            string token = ObterToken(configuracao, out string mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                Servicos.Log.TratarErro("Falha ao obter token Pagbem para conciliação dos CIOTs: " + mensagemErro);

            IntegrarConciliacaoFinanceira(dataConciliacao, configuracao, token, unitOfWork, tipoServicoMultisoftware);
        }

        public bool LiberarViagem(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = ObterConfiguracaoPagbem(ciot.ConfiguracaoCIOT, unitOfWork);

            if (IntegrarLiberacaoViagem(ciot, ciot.Contratante, configuracao, unitOfWork, out mensagemErro))
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                ciot.DataLiberacaoViagem = DateTime.Now;
                ciot.Mensagem = mensagemErro;

                repCIOT.Atualizar(ciot, auditado);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RealizarPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.CIOT.CIOTPagbem repCIOTPamcard = new Repositorio.Embarcador.CIOT.CIOTPagbem(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = repCIOTPamcard.BuscarPorConfiguracaoCIOT();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(pagamentoMotorista.Codigo);


            if (IntegrarCarga(pagamentoMotorista, pagamentoMotorista.Carga?.Empresa, configuracao, unitOfWork, out mensagemErro, out int codigoViagem, out string jsonEnvio, out string jsonRetorno))
            {
                pagamentoEnvio.ArquivoEnvio = jsonEnvio;
                pagamentoEnvio.Data = DateTime.Now;
                pagamentoEnvio.TipoIntegracaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista.PagBem;

                if (!string.IsNullOrEmpty(jsonEnvio) && !string.IsNullOrEmpty(jsonRetorno))
                {
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();
                    pagamentoRetorno.CodigoRetorno = codigoViagem.ToString();
                    pagamentoRetorno.DescricaoRetorno = "Sucesso";
                    pagamentoRetorno.Data = DateTime.Now;
                    pagamentoRetorno.PagamentoMotoristaTMS = pagamentoMotorista;
                    pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
                    pagamentoRetorno.ArquivoRetorno = jsonRetorno;
                    pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonEnvio, "json", unitOfWork);
                    pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork);
                    repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);
                }

                pagamentoEnvio.Retorno = "Sucesso";

                if (codigoViagem.ToString() != "0")
                {
                    pagamentoEnvio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
                else
                {
                    pagamentoEnvio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    pagamentoMotorista.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FalhaIntegracao;
                }

                pagamentoMotorista.CodigoViagem = codigoViagem;
                repPagamentoMotorista.Atualizar(pagamentoMotorista);

                return true;
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonEnvio) && !string.IsNullOrEmpty(jsonRetorno))
                {
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();
                    pagamentoRetorno.CodigoRetorno = "";
                    pagamentoRetorno.DescricaoRetorno = "Falha na integração";
                    pagamentoRetorno.Data = DateTime.Now;
                    pagamentoRetorno.PagamentoMotoristaTMS = pagamentoMotorista;
                    pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
                    pagamentoRetorno.ArquivoRetorno = jsonRetorno;

                    pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonEnvio, "json", unitOfWork);
                    pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork);

                    repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);
                }

                pagamentoEnvio.ArquivoEnvio = jsonEnvio;
                pagamentoEnvio.Retorno = mensagemErro;
                pagamentoEnvio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pagamentoEnvio.Data = DateTime.Now;

                pagamentoMotorista.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FalhaIntegracao;
                pagamentoMotorista.CodigoViagem = 0;

                repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
                repPagamentoMotorista.Atualizar(pagamentoMotorista);

                return false;
            }
        }

        public bool EstornarPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.CIOT.CIOTPagbem repCIOTPamcard = new Repositorio.Embarcador.CIOT.CIOTPagbem(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = repCIOTPamcard.BuscarPorConfiguracaoCIOT();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(pagamentoMotorista.Codigo);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();

            if (IntegrarEstornarCarga(pagamentoMotorista, pagamentoMotorista.Carga?.Empresa, configuracao, unitOfWork, out mensagemErro, out string jsonEnvio, out string jsonRetorno))
            {
                pagamentoRetorno.CodigoRetorno = "0";
                pagamentoRetorno.DescricaoRetorno = "Estornado com Sucesso";
                pagamentoRetorno.Data = DateTime.Now;
                pagamentoRetorno.PagamentoMotoristaTMS = pagamentoMotorista;
                pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
                pagamentoRetorno.ArquivoRetorno = jsonRetorno;

                pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonEnvio, "json", unitOfWork);
                pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork);


                repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);

                return true;
            }
            else
            {
                pagamentoRetorno.CodigoRetorno = "0";
                pagamentoRetorno.DescricaoRetorno = "Falha no Estorno, " + mensagemErro;
                pagamentoRetorno.Data = DateTime.Now;
                pagamentoRetorno.PagamentoMotoristaTMS = pagamentoMotorista;
                pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
                pagamentoRetorno.ArquivoRetorno = jsonRetorno;

                pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonEnvio, "json", unitOfWork);
                pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork);

                repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);

                return false;
            }
        }

        public void GerarCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            try
            {
                Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem = servicoValePedagio.ObterIntegracaoPagbem(carga.TipoOperacao, carga.Filial, carga.GrupoPessoaPrincipal, carga.FreteDeTerceiro, tipoServicoMultisoftware);
                Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = repConfiguracaoCIOT.BuscarPrimeiroPorOperadora(OperadoraCIOT.Pagbem);
                Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = ObterConfiguracaoPagbem(configuracaoCIOT, unitOfWork);

                if (integracaoPagbem == null)
                {
                    cargaValePedagio.ProblemaIntegracao = "Não possui configuração para Pagbem.";
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string mensagemRetorno = string.Empty;

                mensagemRetorno = string.Empty;
                int codigoRoteiro = 0;
                string codigoRotaEmbarcador = string.Empty;
                string retornoJson = "";
                string envioJson = "";
                string token = "";
                bool consultaRoteiro = false;

                Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault() ?? null;
                if (motorista != null && !IntegrarMotorista(carga.Terceiro, motorista, carga.Empresa, configuracao, token, unitOfWork, out mensagemRetorno))
                {
                    cargaValePedagio.ProblemaIntegracao = mensagemRetorno;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                if (carga.Rota != null && !string.IsNullOrWhiteSpace(carga.Rota.CodigoIntegracaoValePedagio))
                    int.TryParse(carga.Rota.CodigoIntegracaoValePedagio, out codigoRoteiro);

                if (integracaoPagbem.ConsultarVeiculoSemParar && !carga.Veiculo.PossuiTagValePedagio)
                {
                    if (ConsultarSituacaoVeiculoSemParar(carga.Veiculo, integracaoPagbem, ref mensagemRetorno, ref retornoJson, ref envioJson, unitOfWork))
                    {
                        if (ConsultarRotaAtendidaSemParar(codigoRoteiro, integracaoPagbem, ref mensagemRetorno, ref retornoJson, ref envioJson, unitOfWork))
                        {
                            carga.Veiculo.PossuiTagValePedagio = true;
                            repVeiculo.Atualizar(carga.Veiculo);
                        }
                    }
                }

                if (motorista != null && !carga.Veiculo.PossuiTagValePedagio && !IntegrarCartaoPagbem(carga.Terceiro, null, motorista, carga.Empresa, configuracao, token, unitOfWork, out mensagemRetorno))
                {
                    cargaValePedagio.ProblemaIntegracao = mensagemRetorno;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                if (codigoRoteiro <= 0)
                {
                    cargaValePedagio.ProblemaIntegracao = "Não existe rota localizada na Pagbem " + codigoRotaEmbarcador;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                decimal valorValePedagio = 0;
                int eixosTotal = 0;
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    eixosTotal = carga.Veiculo.ModeloVeicularCarga?.NumeroEixos.Value ?? 0;
                    if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                    {
                        foreach (var veinculoVinculado in carga.VeiculosVinculados)
                            eixosTotal += veinculoVinculado.ModeloVeicularCarga?.NumeroEixos.Value ?? 0;
                    }
                }
                consultaRoteiro = BuscarValorPedagioRota(carga, integracaoPagbem, codigoRoteiro, ref mensagemRetorno, ref retornoJson, ref envioJson, ref valorValePedagio, unitOfWork, eixosTotal);
                SalvarJsonIntegracao(ref cargaValePedagio, retornoJson, envioJson, "Consulta valor pedágio " + mensagemRetorno, unitOfWork);
                if (!consultaRoteiro)
                {
                    cargaValePedagio.ProblemaIntegracao = mensagemRetorno;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }
                if (valorValePedagio <= 0)
                {
                    cargaValePedagio.ProblemaIntegracao = "Não foi retornado o valor do pedágio pela Pagbem " + codigoRotaEmbarcador;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                int idViagem = 0;
                string numeroComprovante = string.Empty;
                bool comprarValePedagio = IntegrarCompraValePedagio(carga, integracaoPagbem, codigoRoteiro, valorValePedagio, ref mensagemRetorno, ref retornoJson, ref envioJson, ref idViagem, ref numeroComprovante, unitOfWork, tipoServicoMultisoftware);
                SalvarJsonIntegracao(ref cargaValePedagio, retornoJson, envioJson, "Compra vale pedágio " + mensagemRetorno, unitOfWork);

                if (!comprarValePedagio)
                {
                    carga.PossuiPendencia = true;
                    carga.ProblemaIntegracaoValePedagio = true;
                    carga.IntegrandoValePedagio = false;
                    carga.MotivoPendencia = mensagemRetorno;
                    cargaValePedagio.ProblemaIntegracao = mensagemRetorno;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);
                    repCarga.Atualizar(carga);
                }
                else
                {
                    cargaValePedagio.NumeroValePedagio = "";
                    cargaValePedagio.IdCompraValePedagio = idViagem.ToString();
                    cargaValePedagio.NumeroValePedagio = !string.IsNullOrEmpty(numeroComprovante) ? numeroComprovante : idViagem.ToString();
                    cargaValePedagio.ValorValePedagio = valorValePedagio;
                    cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                    cargaValePedagio.ProblemaIntegracao = string.Empty;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;

                    repCargaValePedagio.Atualizar(cargaValePedagio);
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaValePedagio.ProblemaIntegracao = "Falha no serviço da Pagbem";
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repCargaValePedagio.Atualizar(cargaValePedagio);

                return;
            }
        }

        public decimal RetornarValorPedagio(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem, int codigoRoteiro, int qtdEixos, Repositorio.UnitOfWork unitOfWork, out string mensagemRetorno)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemRetorno = string.Empty;
            decimal valorPedagio = 0;
            bool retorno = false;

            string token = ObterToken(integracaoPagbem, out mensagemRetorno);

            if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                return 0m;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ConsultaValorPedagioRota consultaValorPedagioRota = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ConsultaValorPedagioRota()
            {
                IdRota = codigoRoteiro,
                QtdEixos = qtdEixos,
                tipoEixo = "simples"
            };

            string url = integracaoPagbem.URLPagbem + "api/rotas/" + codigoRoteiro + "/calcular/eixos/" + qtdEixos;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("CNPJContratante", integracaoPagbem.CNPJEmpresaContratante);

            string jsonPost = JsonConvert.SerializeObject(consultaValorPedagioRota, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var result = client.GetAsync(url).Result;
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = null;
            if (result.IsSuccessStatusCode)
            {
                retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Pagbem");
                if (retornoIntegracao.isSucesso)
                {
                    valorPedagio = retornoIntegracao.resultado.pedagioTotal;

                    retorno = true;
                }
                else
                {
                    mensagemRetorno = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração da rota na Pagbem.";
                    retorno = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");
                mensagemRetorno = result.Content.ReadAsStringAsync().Result;

                retorno = false;
            }

            if (retorno)
            {
                return valorPedagio;
            }
            else
            {
                return valorPedagio;
            }
        }

        public byte[] GerarImpressaoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            return ReportRequest.WithType(ReportType.ValePedagioPagBem)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCargaValePedagio", cargaValePedagio.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        #endregion

        #region Métodos Privados

        private void IntegrarConciliacaoFinanceira(DateTime dataConciliacao, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, string token, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);

            Servicos.Embarcador.Terceiros.ContratoFrete svcContratoFrete = new Terceiros.ContratoFrete(unitOfWork);

            List<Dominio.Entidades.Empresa> empresas = repCIOT.BuscarEmpresasComCIOTAberto();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            foreach (Dominio.Entidades.Empresa empresa in empresas)
            {
                string url = configuracao.URLPagbem + $"api/conciliacoes/financeira/data/{dataConciliacao:yyyy-MM-dd}/contratante/{empresa.CNPJ}";

                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                //client.DefaultRequestHeaders.Add("CNPJContratante", empresa.CNPJ);

                HttpResponseMessage result = client.SendAsync(request).Result;

                string jsonResult = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoConciliacaoFinanceira retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoConciliacaoFinanceira>(jsonResult);

                    if (retornoIntegracao.isSucesso)
                    {
                        Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
                        {
                            ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao("", "json", unitOfWork),
                            ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResult, "json", unitOfWork),
                            Data = DateTime.Now,
                            Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                            Mensagem = "Conciliação financeira obtida com sucesso."
                        };

                        repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                        foreach (Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ResultadoConciliacaoFinanceira detalhesConciliacao in retornoIntegracao.resultado)
                        {
                            if (detalhesConciliacao.tipoEvento != "Quitacao" && detalhesConciliacao.tipoEvento != "Adiantamento")
                                continue;

                            if (detalhesConciliacao.idViagem <= 0)
                                continue;

                            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorProtocoloAutorizacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem, detalhesConciliacao.idViagem.ToString());

                            if (ciot == null)
                                continue;

                            if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto && ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)
                                continue;

                            unitOfWork.Start();

                            if (detalhesConciliacao.tipoEvento == "Quitacao")
                            {
                                bool encerrarCIOT = ciot.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;

                                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                                ciot.Mensagem += " Quitação realizada com sucesso.";
                                ciot.DataEncerramento = detalhesConciliacao.data;

                                ciot.DataChegada = detalhesConciliacao?.detalhesQuitacao?.dataChegada ?? null;
                                ciot.PesoChegada = detalhesConciliacao?.detalhesQuitacao?.pesoChegada ?? 0m;
                                ciot.ValorAvaria = detalhesConciliacao?.detalhesQuitacao?.valorAvaria ?? 0m;
                                ciot.ValorDiferencaFrete = detalhesConciliacao?.detalhesQuitacao?.valorDiferencaFrete ?? 0m;
                                ciot.ValorQuebra = detalhesConciliacao?.detalhesQuitacao?.valorQuebra ?? 0m;

                                if (ciot.ValorQuebra < 0)
                                    ciot.ValorQuebra = ciot.ValorQuebra * -1;
                                if (ciot.ValorDiferencaFrete < 0)
                                    ciot.ValorDiferencaFrete = ciot.ValorDiferencaFrete * -1;

                                decimal desconto = (ciot?.ValorQuebra ?? 0m) + (ciot?.ValorDiferencaFrete ?? 0m);
                                if (desconto <= 0)
                                    desconto = desconto * -1;

                                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                                repCIOT.Atualizar(ciot);

                                Servicos.Auditoria.Auditoria.Auditar(auditado, ciot, "Encerramento realizado via conciliação financeira com a operadora.", unitOfWork);

                                svcContratoFrete.AplicarDescontoPorCIOT(desconto, ciot, unitOfWork);
                                if (encerrarCIOT)
                                    svcContratoFrete.EncerrarContratosPorCIOT(ciot, tipoServicoMultisoftware, unitOfWork, ciot.DataEncerramento.HasValue ? ciot.DataEncerramento.Value : DateTime.Now, false);
                            }
                            else if (detalhesConciliacao.tipoEvento == "Adiantamento")
                            {
                                ciot.DataSaqueAdiantamento = detalhesConciliacao.data;
                                ciot.ValorSaqueAdiantamento += detalhesConciliacao.valorEvento;

                                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                                repCIOT.Atualizar(ciot);

                                Servicos.Auditoria.Auditoria.Auditar(auditado, ciot, "Conciliação do valor do adiantamento.", unitOfWork);
                            }

                            unitOfWork.CommitChanges();
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro(jsonResult, "Pagbem");
                    }
                }
                else
                {
                    Servicos.Log.TratarErro(jsonResult, "Pagbem");
                }
            }
        }

        private string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem ciotPagbem, out string erro)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                erro = string.Empty;

                string postBody = "grant_type=password&password=" + ciotPagbem.SenhaPagbem + "&username=" + ciotPagbem.UsuarioPagbem;


                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                var data = client.UploadString(ciotPagbem.URLPagbem + "token", postBody);

                var responseString = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoToken>((string)data);

                if (string.IsNullOrWhiteSpace(responseString.access_token))
                {

                    Servicos.Log.TratarErro(postBody, "Pagbem");
                    Servicos.Log.TratarErro(JsonConvert.SerializeObject(responseString, Formatting.Indented), "Pagbem");

                    erro = "Pagbem não retornou Token";
                }

                return responseString.access_token;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Pagbem");

                erro = "Não foi possível obter token Pagbem";
                return string.Empty;
            }
        }

        private string ObterToken(Dominio.Entidades.Embarcador.CIOT.CIOTPagbem ciotPagbem, out string erro)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                erro = string.Empty;

                string postBody = "grant_type=password&password=" + ciotPagbem.SenhaPagbem + "&username=" + ciotPagbem.UsuarioPagbem;


                WebClient client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                var data = client.UploadString(ciotPagbem.URLPagbem + "token", postBody);

                var responseString = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoToken>((string)data);

                if (string.IsNullOrWhiteSpace(responseString.access_token))
                {

                    Servicos.Log.TratarErro(postBody, "Pagbem");
                    Servicos.Log.TratarErro(JsonConvert.SerializeObject(responseString, Formatting.Indented), "Pagbem");

                    erro = "Pagbem não retornou Token";
                }

                return responseString.access_token;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Pagbem");

                erro = "Não foi possível obter token Pagbem";
                return string.Empty;
            }
        }

        private Dominio.Entidades.Embarcador.CIOT.CIOTPagbem ObterConfiguracaoPagbem(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.CIOT.CIOTPagbem repCIOTPamcard = new Repositorio.Embarcador.CIOT.CIOTPagbem(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao = repCIOTPamcard.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

            return configuracao;
        }

        private bool ValidarRNTRCContratado(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, string RNTRC, string token, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out string jsonEnvio, out string jsonRetorno, bool validarANTT)
        {
            mensagemErro = string.Empty;
            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;

            if (!validarANTT)
                return true;

            if (veiculo == null || string.IsNullOrWhiteSpace(RNTRC))
                return true;

            if (string.IsNullOrWhiteSpace(token))
                token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratadoRNTRC contratado = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratadoRNTRC();
            contratado.CNPJCPFContratado = proprietario.CPF_CNPJ_SemFormato;
            contratado.RNTRCContratado = RNTRC;
            contratado.veiculos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.VeiculosRNTRC>();
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.VeiculosRNTRC veiculoRNTRC = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.VeiculosRNTRC();
            veiculoRNTRC.placa = veiculo.Placa;
            contratado.veiculos.Add(veiculoRNTRC);

            string url = configuracao.URLPagbem + "api/antt/situacao";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", contratante.CNPJ);

            string jsonPost = JsonConvert.SerializeObject(contratado, Formatting.Indented);
            jsonEnvio = jsonPost;
            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PutAsync(url, content).Result;
            jsonRetorno = (string)result.Content.ReadAsStringAsync().Result;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                if (retornoIntegracao.isSucesso)
                {
                    if (retornoIntegracao.resultado.isRNTRCAtivo)
                        return true;
                    else
                    {
                        mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha validar RNTRC na Pagbem.";
                        return false;
                    }
                }
                else
                {
                    string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                    Servicos.Log.TratarErro(jsonResult, "Pagbem");

                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha validar RNTRC na Pagbem.";
                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                return false;
            }
        }

        private bool IntegrarContratado(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, string token, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = null;

            if (proprietario == null)
                return true;

            if (modalidade == null)
            {
                mensagemErro = "A modalidade do transportador não está configurada.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(token))
                token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(proprietario.Localidade?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Contratado contratado = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Contratado();
            contratado.CNPJCPF = proprietario.CPF_CNPJ_SemFormato;
            contratado.nome = proprietario.Nome;
            contratado.IE = proprietario.IE_RG;
            contratado.nomeMae = "";
            contratado.dataNascimento = proprietario.DataNascimento.HasValue ? proprietario.DataNascimento.Value.ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z" : string.Empty;
            contratado.endereco = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Endereco();
            contratado.endereco.logradouro = proprietario.Endereco;
            contratado.endereco.numero = proprietario.Numero;
            contratado.endereco.complemento = proprietario.Complemento;
            contratado.endereco.bairro = proprietario.Bairro;
            contratado.endereco.CEP = !string.IsNullOrWhiteSpace(proprietario.CEP) ? Utilidades.String.OnlyNumbers(proprietario.CEP.PadLeft(8, '0')) : "0";
            contratado.endereco.codigoIBGE = localidade?.CodigoIBGE ?? 0;
            contratado.endereco.municipio = localidade?.Descricao ?? string.Empty;
            contratado.endereco.UF = localidade?.Estado.Sigla ?? string.Empty;
            contratado.endereco.KM = "";

            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(proprietario.Telefone1)))
            {
                string telefone = Utilidades.String.OnlyNumbers(proprietario.Telefone1);
                if (telefone.Length >= 10)
                {
                    contratado.telefone1 = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Telefone();
                    contratado.telefone1.DDD = int.Parse(Utilidades.String.Left(telefone, 2));
                    contratado.telefone1.numero = int.Parse(telefone.Substring(2, telefone.Length - 2));
                }
            }
            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(proprietario.Telefone2)))
            {
                string telefone = Utilidades.String.OnlyNumbers(proprietario.Telefone2);
                if (telefone.Length >= 10)
                {
                    contratado.telefone2 = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Telefone();
                    contratado.telefone2.DDD = int.Parse(Utilidades.String.Left(telefone, 2));
                    contratado.telefone2.numero = int.Parse(telefone.Substring(2, telefone.Length - 2));
                }
            }

            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(proprietario.Telefone2)))
            {
                string telefone = Utilidades.String.OnlyNumbers(proprietario.Telefone2);
                if (telefone.Length >= 10)
                {
                    contratado.telefoneCelular = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Telefone();
                    contratado.telefoneCelular.DDD = int.Parse(Utilidades.String.Left(telefone, 2));
                    contratado.telefoneCelular.numero = int.Parse(telefone.Substring(2, telefone.Length - 2));
                }
            }

            contratado.email = proprietario.Email;
            contratado.RNTRC = modalidade.RNTRC;

            if (proprietario.Tipo == "J")
                contratado.PIS = null;
            else
                contratado.PIS = contratado.PIS;

            string url = configuracao.URLPagbem + "api/contratados/" + proprietario.CPF_CNPJ_SemFormato;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", contratante.CNPJ);

            string jsonPost = JsonConvert.SerializeObject(contratado, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PutAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                if (retornoIntegracao.isSucesso)
                    return true;
                else
                {
                    string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                    Servicos.Log.TratarErro(jsonResult, "Pagbem");

                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração contratado na Pagbem.";
                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                return false;
            }
        }

        private bool IntegrarMotorista(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, string token, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (string.IsNullOrWhiteSpace(token))
                token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            if (motorista == null)
            {
                mensagemErro = "Nenhum motorista informado na carga/veiculo.";
                return false;
            }

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(motorista.Localidade?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Motorista motoristaPagbem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Motorista();
            motoristaPagbem.CPF = motorista.CPF;
            motoristaPagbem.nome = motorista.Nome;
            //motoristaPagbem.RNTRC = 
            //motoristaPagbem.nomeMae =
            motoristaPagbem.dataNascimento = motorista.DataNascimento.HasValue ? motorista.DataNascimento.Value.ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z" : string.Empty;
            motoristaPagbem.email = motorista.Email;
            motoristaPagbem.PIS = motorista.PIS;

            motoristaPagbem.endereco = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Endereco();
            motoristaPagbem.endereco.logradouro = motorista.Endereco;
            motoristaPagbem.endereco.numero = motorista.NumeroEndereco;
            motoristaPagbem.endereco.complemento = motorista.Complemento;
            motoristaPagbem.endereco.bairro = motorista.Bairro;
            motoristaPagbem.endereco.CEP = !string.IsNullOrWhiteSpace(motorista.CEP) ? Utilidades.String.OnlyNumbers(motorista.CEP.PadLeft(8, '0')) : "0";
            motoristaPagbem.endereco.codigoIBGE = localidade?.CodigoIBGE ?? 0;
            motoristaPagbem.endereco.municipio = localidade?.Descricao ?? string.Empty;
            motoristaPagbem.endereco.UF = localidade?.Estado.Sigla ?? string.Empty;
            //motoristaPagbem.endereco.KM = 

            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(motorista.Telefone)))
            {
                string telefoneCompleto = Utilidades.String.OnlyNumbers(motorista.Telefone);
                if (telefoneCompleto.Length >= 10)
                {
                    string ddd = string.Empty;
                    string telefone = string.Empty;

                    if (telefoneCompleto.StartsWith("0"))
                    {
                        ddd = telefoneCompleto.Substring(1, 2);
                        telefone = telefoneCompleto.Substring(3, telefoneCompleto.Length - 3);
                    }
                    else
                    {
                        ddd = telefoneCompleto.Substring(0, 2);
                        telefone = telefoneCompleto.Substring(2, telefoneCompleto.Length - 2);
                    }

                    motoristaPagbem.telefone1 = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Telefone();
                    motoristaPagbem.telefone1.DDD = int.Parse(ddd);
                    motoristaPagbem.telefone1.numero = int.Parse(telefone);
                }
            }
            //if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(motorista.Telefone)))
            //{
            //    motoristaPagbem.telefone2 = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Telefone();
            //    motoristaPagbem.telefone2.DDD = int.Parse(Utilidades.String.OnlyNumbers(motorista.Telefone?.Split(' ')[0]));
            //    motoristaPagbem.telefone2.numero = int.Parse(Utilidades.String.OnlyNumbers(motorista.Telefone?.Split(' ')[1]));
            //}
            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(motorista.Celular)))
            {
                string telefoneCompleto = Utilidades.String.OnlyNumbers(motorista.Celular);
                if (telefoneCompleto.Length >= 10)
                {
                    string ddd = string.Empty;
                    string telefone = string.Empty;

                    if (telefoneCompleto.StartsWith("0"))
                    {
                        ddd = telefoneCompleto.Substring(1, 2);
                        telefone = telefoneCompleto.Substring(3, telefoneCompleto.Length - 3);
                    }
                    else
                    {
                        ddd = telefoneCompleto.Substring(0, 2);
                        telefone = telefoneCompleto.Substring(2, telefoneCompleto.Length - 2);
                    }

                    motoristaPagbem.telefoneCelular = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Telefone();
                    motoristaPagbem.telefoneCelular.DDD = int.Parse(ddd);
                    motoristaPagbem.telefoneCelular.numero = int.Parse(telefone);
                }
            }

            motoristaPagbem.RG = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RG();
            motoristaPagbem.RG.UF = motorista.EstadoRG?.Sigla ?? localidade?.Estado.Sigla ?? string.Empty;
            motoristaPagbem.RG.numero = motorista.RG;

            motoristaPagbem.CNH = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.CNH();
            motoristaPagbem.CNH.categoria = !string.IsNullOrWhiteSpace(motorista.Categoria) ? motorista.Categoria : "C";
            motoristaPagbem.CNH.numero = !string.IsNullOrWhiteSpace(motorista.NumeroHabilitacao) ? motorista.NumeroHabilitacao.PadLeft(11, '0') : string.Empty;
            motoristaPagbem.CNH.validade = motorista.DataVencimentoHabilitacao.HasValue ? motorista.DataVencimentoHabilitacao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z" : string.Empty;

            string url = configuracao.URLPagbem + "api/motoristas/" + motoristaPagbem.CPF;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", contratante.CNPJ);

            string jsonPost = JsonConvert.SerializeObject(motoristaPagbem, Formatting.Indented);
            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PutAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                if (retornoIntegracao.isSucesso)
                    return true;
                else
                {
                    string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                    Servicos.Log.TratarErro(jsonResult, "Pagbem");

                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração motorista na Pagbem.";
                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                return false;
            }
        }

        private bool IntegrarCartaoPagbem(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, string token, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (string.IsNullOrWhiteSpace(token))
                token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.CartaoPagbem cartaoPagbem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.CartaoPagbem();
            cartaoPagbem.numeroCartaoPagBem = modalidade == null ? motorista.NumeroCartao : modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? motorista.NumeroCartao : modalidade.NumeroCartao;
            cartaoPagbem.CNPJCPFPortador = modalidade == null ? motorista.CPF : modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? motorista.CPF : proprietario.CPF_CNPJ_SemFormato;

            //Responsável deve se informado apenas para portadores CNPJ
            if ((modalidade == null || modalidade.TipoFavorecidoCIOT != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista) && proprietario.Tipo == "J" && !configuracao.NaoIntegrarResponsavelCartaoPagbem)
            {
                cartaoPagbem.responsavelCartao = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ResponsavelCartao();
                cartaoPagbem.responsavelCartao.CPF = motorista.CPF;
                cartaoPagbem.responsavelCartao.nome = motorista.Nome;
                cartaoPagbem.responsavelCartao.telefone = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Telefone();
                if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(motorista.Telefone)))
                {
                    string telefoneMotorista = Utilidades.String.OnlyNumbers(motorista.Telefone);
                    if (telefoneMotorista.Length >= 10)
                    {
                        cartaoPagbem.responsavelCartao.telefone.DDD = int.Parse(Utilidades.String.Left(telefoneMotorista, 2));
                        cartaoPagbem.responsavelCartao.telefone.numero = int.Parse(telefoneMotorista.Substring(2, telefoneMotorista.Length - 2));
                    }
                }
            }

            string url = configuracao.URLPagbem + "api/cartoes";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", contratante.CNPJ);

            string jsonPost = JsonConvert.SerializeObject(cartaoPagbem, Formatting.Indented);
            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PutAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                if (retornoIntegracao.isSucesso)
                    return true;
                else
                {
                    string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                    Servicos.Log.TratarErro(jsonResult, "Pagbem");

                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração cartão motorista na Pagbem.";

                    if (mensagemErro.Contains("Cartão PagBem já associado") || mensagemErro.Contains("Cartão já vinculado"))
                        return true;

                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                return false;
            }
        }

        private bool IntegrarContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, string codigoRoteiro, string codigoPercurso, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, string token, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool retorno = false;
            mensagemErro = string.Empty;

            if (string.IsNullOrWhiteSpace(token))
                token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFrete contratoFrete = ObterContratoFrete(cargaCIOT, modalidadeTerceiro, codigoRoteiro, codigoPercurso, unitOfWork, configuracao);

            string url = configuracao.URLPagbem + "api/viagens";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.Timeout = TimeSpan.FromMinutes(5);

            string cnpjContratante = string.Empty;
            if (configuracao.UtilizarCnpjContratanteIntegracao)
                cnpjContratante = configuracao.CNPJEmpresaContratante;
            else
                cnpjContratante = cargaCIOT.CIOT.Contratante.CNPJ;

            client.DefaultRequestHeaders.Add("CNPJContratante", cnpjContratante);

            string jsonPost = JsonConvert.SerializeObject(contratoFrete, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });

            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonPost, "json", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            var result = client.PostAsync(url, content).Result;
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = null;
            if (result.IsSuccessStatusCode)
            {
                retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Pagbem");
                if (retornoIntegracao.isSucesso)
                {
                    ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResult, "json", unitOfWork);

                    retorno = true;
                }
                else
                {
                    ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResult, "json", unitOfWork);

                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração cartão motorista na Pagbem.";
                    retorno = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(mensagemErro, "json", unitOfWork);

                retorno = false;
            }

            if (retorno)
                ciotIntegracaoArquivo.Mensagem = "Envio realizado com sucesso.";
            else
                ciotIntegracaoArquivo.Mensagem = "Falha no envio.";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            if (retorno)
            {
                cargaCIOT.CIOT.Numero = retornoIntegracao.resultado.CIOT.Substring(0, 12);
                cargaCIOT.CIOT.CodigoVerificador = retornoIntegracao.resultado.CIOT.Substring(13, 4);
                cargaCIOT.CIOT.DataAbertura = DateTime.Now;
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                cargaCIOT.CIOT.ProtocoloAutorizacao = retornoIntegracao.resultado.idViagem.ToString();
                cargaCIOT.CIOT.Mensagem = "CIOT processado com sucesso.";

                //decimal.TryParse(retornoSucesso.DadosViagem.ValorPedagio, System.Globalization.NumberStyles.None, cultura, out decimal valorPedagio);
                //cargaCIOT.ContratoFrete.ValorPedagio = valorPedagio;

                //repContratoFrete.Atualizar(cargaCIOT.ContratoFrete);
                repCIOT.Atualizar(cargaCIOT.CIOT);

                return true;
            }
            else
            {
                if (mensagemErro.Contains("NumeroViagemCliente duplicado"))
                {
                    var retornoConsulta = this.ConsultarPorViagemCliente(cargaCIOT.CIOT, cargaCIOT.Carga.CodigoCargaEmbarcador, cnpjContratante, configuracao, unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ResultadoConsulta dadosViagem = retornoConsulta?.resultado?.FirstOrDefault();

                    if (retornoConsulta.isSucesso && dadosViagem != null && !string.IsNullOrWhiteSpace(dadosViagem.CIOT))
                    {
                        cargaCIOT.CIOT.Numero = dadosViagem.CIOT.Substring(0, 12);
                        cargaCIOT.CIOT.CodigoVerificador = dadosViagem.CIOT.Substring(13, 4);
                        cargaCIOT.CIOT.DataAbertura = DateTime.Now;
                        cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                        cargaCIOT.CIOT.ProtocoloAutorizacao = dadosViagem.idViagem.ToString();
                        cargaCIOT.CIOT.Mensagem = "CIOT processado com sucesso.";

                        repCIOT.Atualizar(cargaCIOT.CIOT);

                        return true;
                    }
                    else
                    {
                        cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                        cargaCIOT.CIOT.Mensagem = mensagemErro;

                        repCIOT.Atualizar(cargaCIOT.CIOT);
                        return false;
                    }
                }

                return false;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFrete ObterContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, string codigoRoteiro, string codigoPercurso, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCIOT.Carga.Pedidos.FirstOrDefault();

            Dominio.Entidades.Cliente destinatario = null;

            if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor)
                destinatario = cargaPedido.Recebedor;
            else
                destinatario = cargaPedido.Pedido.Destinatario;

            if (cargaCIOT.CIOT.Veiculo == null)
            {
                cargaCIOT.CIOT.Veiculo = cargaCIOT.Carga.Veiculo;
                cargaCIOT.CIOT.VeiculosVinculados = cargaCIOT.Carga.VeiculosVinculados.ToList();
            }

            Dominio.Entidades.Veiculo carreta = cargaCIOT.CIOT.VeiculosVinculados.FirstOrDefault();

            DateTime dataSaida = DateTime.Now.AddHours(1);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componentePedagio = repCargaComponentesFrete.BuscarPorCargaETipo(cargaCIOT.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, null, false);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCIOT.Carga.CargaCTes.FirstOrDefault()?.CTe;
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDocumentosViagem> documentosViagem = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDocumentosViagem>();
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteOrigem origem = null;
            decimal pesoSaidaKg = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo);
            decimal valorMecadoria = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(cargaCIOT.Carga.Codigo);
            string freteTipoPeso = !string.IsNullOrWhiteSpace(configuracao.FreteTipoPeso) ? configuracao.FreteTipoPeso : "Saida";
            if (cargaCIOT.Carga != null && cargaCIOT.Carga.TipoOperacao != null && !string.IsNullOrWhiteSpace(cargaCIOT.Carga.TipoOperacao.FreteTipoPesoPagBem))
                freteTipoPeso = cargaCIOT.Carga.TipoOperacao.FreteTipoPesoPagBem;

            if (cte != null)
            {
                origem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteOrigem()
                {
                    filialContratante = cargaCIOT.CIOT.Contratante?.CodigoIntegracao ?? string.Empty,
                    remetente = cte.Remetente.Nome,
                    CNPJRemetente = cte.Remetente.CPF_CNPJ,
                    endereco = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteOrigemEndereco()
                    {
                        logradouro = cte.Remetente.Endereco,
                        numero = cte.Remetente.Numero,
                        complemento = cte.Remetente.Complemento,
                        bairro = cte.Remetente.Bairro,
                        CEP = !string.IsNullOrWhiteSpace(cte.Remetente.CEP) ? Utilidades.String.OnlyNumbers(cte.Remetente.CEP.PadLeft(8, '0')) : "0",
                        codigoIBGE = cte.Remetente.Localidade?.CodigoIBGE ?? 0,
                        KM = "",
                    }
                };

                if (configuracao.TipoFilialContratante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilialContratantePagbem.GrupoPessoas)
                    origem.filialContratante = cte.Remetente.GrupoPessoas?.CodigoIntegracao ?? string.Empty;
            }
            else
            {
                origem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteOrigem()
                {
                    filialContratante = cargaCIOT.CIOT.Contratante?.CodigoIntegracao ?? string.Empty,
                    remetente = cargaPedido.Pedido.Remetente.Nome,
                    CNPJRemetente = cargaPedido.Pedido.Remetente.CPF_CNPJ_SemFormato,
                    endereco = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteOrigemEndereco()
                    {
                        logradouro = cargaPedido.Pedido.Remetente.Endereco,
                        numero = cargaPedido.Pedido.Remetente.Numero,
                        complemento = cargaPedido.Pedido.Remetente.Complemento,
                        bairro = cargaPedido.Pedido.Remetente.Bairro,
                        CEP = !string.IsNullOrWhiteSpace(cargaPedido.Pedido.Remetente.CEP) ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Remetente.CEP.PadLeft(8, '0')) : "0",
                        codigoIBGE = cargaPedido.Pedido.Remetente.Localidade?.CodigoIBGE ?? 0,
                        KM = "",
                    }
                };

                if (configuracao.TipoFilialContratante == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilialContratantePagbem.GrupoPessoas)
                    origem.filialContratante = cte.Remetente.GrupoPessoas?.CodigoIntegracao ?? string.Empty;
            }

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFrete contratoFrete = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFrete()
            {
                isViagemLiberada = !configuracao.LiberarViagemManualmente,
                contratado = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteContratado()
                {
                    CNPJCPF = cargaCIOT.CIOT.Transportador.CPF_CNPJ_SemFormato,
                    RNTRC = modalidadeTerceiro.RNTRC,
                    numeroCartaoPagBem = !string.IsNullOrWhiteSpace(modalidadeTerceiro.NumeroCartao) ? modalidadeTerceiro.NumeroCartao : null
                },
                motorista = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteMotorista()
                {
                    CPF = cargaCIOT.CIOT.Motorista.CPF,
                    CNH = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteMotoristaCNH()
                    {
                        categoria = cargaCIOT.CIOT.Motorista.Categoria,
                        numero = !string.IsNullOrWhiteSpace(cargaCIOT.CIOT.Motorista.NumeroHabilitacao) ? cargaCIOT.CIOT.Motorista.NumeroHabilitacao.PadLeft(11, '0') : string.Empty,
                        validade = cargaCIOT.CIOT.Motorista.DataVencimentoHabilitacao.HasValue ? cargaCIOT.CIOT.Motorista.DataVencimentoHabilitacao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z" : string.Empty
                    },
                    numeroCartaoPagBem = !string.IsNullOrWhiteSpace(cargaCIOT.CIOT.Motorista.NumeroCartao) ? cargaCIOT.CIOT.Motorista.NumeroCartao : null
                },
                origem = origem,
                destino = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDestino()
                {
                    destinatario = destinatario.Nome,
                    CNPJDestinatario = destinatario.CPF_CNPJ_SemFormato,
                    endereco = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteOrigemEndereco()
                    {
                        logradouro = destinatario.Endereco,
                        numero = destinatario.Numero,
                        complemento = destinatario.Complemento,
                        bairro = destinatario.Bairro,
                        CEP = !string.IsNullOrWhiteSpace(destinatario.CEP) ? Utilidades.String.OnlyNumbers(destinatario.CEP.PadLeft(8, '0')) : "0",
                        codigoIBGE = destinatario.Localidade?.CodigoIBGE ?? 0,
                        KM = "",
                    }
                },
                periodoViagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFretePeriodoViagem()
                {
                    inicio = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z",
                    termino = DateTime.Now.AddDays(15).ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z",
                },
                dadosFrete = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDadosFrete
                {
                    INSS = cargaCIOT.ContratoFrete.ValorINSS,
                    IR = cargaCIOT.ContratoFrete.ValorIRRF,
                    sestSenat = cargaCIOT.ContratoFrete.ValorSEST + cargaCIOT.ContratoFrete.ValorSENAT,
                    seguro = 0,
                    outrosDebitos = cargaCIOT.ContratoFrete.Descontos,
                    valorUnidadeFrete = configuracaoTMS.SolicitarValorFretePorTonelada ? (pesoSaidaKg > 0 ? (cargaCIOT.ContratoFrete.ValorFreteSubcontratacao / pesoSaidaKg) : 0) : (pesoSaidaKg > 0 ? (cargaCIOT.ContratoFrete.ValorLiquidoSemAdiantamento / pesoSaidaKg) : 0),
                    valorKgMercadoria = pesoSaidaKg > 0 ? (valorMecadoria / pesoSaidaKg) : 0,
                    valorTotalFrete = configuracaoTMS.SolicitarValorFretePorTonelada ? cargaCIOT.ContratoFrete.ValorFreteSubcontratacao : cargaCIOT.ContratoFrete.ValorLiquidoSemAdiantamento,
                    valorPesoSaida = pesoSaidaKg,
                    valorPedagio = componentePedagio != null ? componentePedagio.ValorComponente : 0,
                    distanciaPercorrida = cargaCIOT.Carga.Distancia > 0 ? (int)cargaCIOT.Carga.Distancia : (int)cargaCIOT.Carga.DadosSumarizados.Distancia,
                    tarifa = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDadosFreteTarifa()
                    {
                        quantidadeTarifasBancarias = 0,
                        valorTarifasBancarias = 0
                    },
                    tipoCarga = "Fracionada",
                    codigoTipoCargaANTT = !string.IsNullOrWhiteSpace(cargaCIOT.CIOT.Veiculo.ModeloVeicularCarga?.CodigoTipoCargaANTT ?? "") ? cargaCIOT.CIOT.Veiculo.ModeloVeicularCarga?.CodigoTipoCargaANTT : "5"
                },
                dadosCarga = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDadosCarga()
                {
                    tipoViagem = "Padrao",
                    NCM = "0001",
                    pesoTotalSaidaKg = pesoSaidaKg
                },
                veiculos = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteVeiculos()
                {
                    cavalo = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteVeiculosCavalo()
                    {
                        altoDesempenho = false,
                        placa = cargaCIOT.CIOT.Veiculo.Placa,
                        RNTRC = modalidadeTerceiro.RNTRC,
                        eixos = cargaCIOT.CIOT.Veiculo.ModeloVeicularCarga?.NumeroEixos.Value ?? 0
                    },
                    carretas = cargaCIOT.CIOT.VeiculosVinculados.Select(o => new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteVeiculosCarretas
                    {
                        placa = o.Placa,
                        RNTRC = modalidadeTerceiro.RNTRC,
                        eixos = o.ModeloVeicularCarga?.NumeroEixos.Value ?? 0
                    }).ToArray(),
                },
                pagamentoFrete = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFretePagamentoFrete()
                {
                    freteQuitacaoRegras = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFretePagamentoFreteFreteQuitacaoRegras()
                    {
                        tipoTolerancia = !string.IsNullOrWhiteSpace(configuracao.TipoTolerancia) ? configuracao.TipoTolerancia : "kilograma",
                        freteTipoPeso = freteTipoPeso,
                        freteLimiteSuperior = 0,
                        quebraTipoCobranca = !string.IsNullOrWhiteSpace(configuracao.QuebraTipoCobranca) ? configuracao.QuebraTipoCobranca : "NaoCobra",
                        quebraTolerancia = configuracao.QuebraTolerancia > 0 ? configuracao.QuebraTolerancia : 0,
                        avariaTipoCobranca = "NaoCobra"
                    },
                    informacoesObrigatoriasQuitacao = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFretePagamentoFreteInformacoesObrigatoriasQuitacao()
                    {
                        dataEntrega = cargaCIOT.Carga?.TipoOperacao?.DataEntregaPagbem ?? false,
                        peso = cargaCIOT.Carga?.TipoOperacao?.PesoPagbem ?? false,
                        ticketBalanca = cargaCIOT.Carga?.TipoOperacao?.TicketBalancaPagbem ?? false,
                        avaria = cargaCIOT.Carga?.TipoOperacao?.AvariaPagbem ?? false,
                        canhotoNFe = cargaCIOT.Carga?.TipoOperacao?.CanhotoNFePagbem ?? false,
                        comprovantePedagio = cargaCIOT.Carga?.TipoOperacao?.ComprovantePedagioPagbem ?? false,
                        DACTE = cargaCIOT.Carga.CargaCTes.Any(o => o.CTe.ModeloDocumentoFiscal != null && o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe) ? cargaCIOT.Carga?.TipoOperacao?.DACTEPagbem ?? false : false,
                        contratoTransporte = cargaCIOT.Carga?.TipoOperacao?.ContratoTransportePagbem ?? false
                    },
                    participacao = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFretePagamentoFreteParticipacao()
                    {
                        motorista = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFretePagamentoFreteParticipacaoPortentagem()
                        {
                            porcentagemAdiantamento = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? 100 : 0,
                            porcentagemQuitacao = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? 100 : 0
                        },
                        contratado = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFretePagamentoFreteParticipacaoPortentagem()
                        {
                            porcentagemAdiantamento = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? 0 : 100,
                            porcentagemQuitacao = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? 0 : 100
                        }
                    },
                    localAdiantamento = modalidadeTerceiro.TipoQuitacaoCIOT.HasValue ? modalidadeTerceiro.TipoQuitacaoCIOT.Value.ObterEnumeradorPagbem() : "QualquerLugar",
                    localQuitacao = modalidadeTerceiro.TipoAdiantamentoCIOT.HasValue ? modalidadeTerceiro.TipoAdiantamentoCIOT.Value.ObterEnumeradorPagbem() : "QualquerLugar",
                    meioPagamentoFrete = "PagBem",
                    destinacaoComercial = false
                },
                numeroViagemCliente = cargaCIOT.Carga.CodigoCargaEmbarcador,
                operador = cargaCIOT.ContratoFrete.Usuario != null ? cargaCIOT.ContratoFrete.Usuario.Login : "automatico",
                documentosViagem = cargaCIOT.Carga.CargaCTes.Select(o => new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDocumentosViagem
                {
                    tipoDocumentoViagem = o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros ? "CRT" : o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ? "NFS" : "CTe",
                    chaveAcessoDocumentoViagem = o.CTe.Chave,
                    documentoViagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDocumentosViagemDocumentoViagem()
                    {
                        serie = o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ? o.CTe.RPS?.Serie ?? "0" : o.CTe.Serie.Numero.ToString(),
                        numero = o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ? ((configuracao.IntegrarNumeroRPSNFSE ? o.CTe.RPS?.Numero.ToString() : o.CTe.Numero.ToString()) ?? "0") : o.CTe.Numero.ToString(),
                    },
                    carga = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDocumentosViagemCarga()
                    {
                        unidadeMedida = "kg",
                        quantidade = cargaCIOT.Carga.DadosSumarizados.PesoTotal,
                        pesoSaidaKg = cargaCIOT.Carga.DadosSumarizados.PesoTotal
                    },
                    NFes = (from nota in o.NotasFiscais
                            select new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDocumentosViagemNFes
                            {
                                serie = !string.IsNullOrWhiteSpace(nota.PedidoXMLNotaFiscal.XMLNotaFiscal.SerieOuSerieDaChave) ? nota.PedidoXMLNotaFiscal.XMLNotaFiscal.SerieOuSerieDaChave : "0",
                                numero = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString()
                            }).ToArray(),
                    valorDocumentoViagem = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(o.Carga.Codigo)
                }).ToArray()
            };

            //Caso não existir o subcontratado na viagem, não enviar a TAG. (Conforme orientação da Target pode-se enviar o mesmo do Contratado
            if (cargaCIOT.CIOT.Veiculo.Proprietario != null && cargaCIOT.CIOT.Veiculo.Tipo == "T" && contratoFrete.contratado.CNPJCPF != cargaCIOT.CIOT.Veiculo.Proprietario.CPF_CNPJ_SemFormato)
            {
                contratoFrete.subcontratado = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Subcontratado();
                contratoFrete.subcontratado.CNPJCPF = cargaCIOT.CIOT.Veiculo.Proprietario.CPF_CNPJ_SemFormato;
                contratoFrete.subcontratado.RNTRC = string.Format("{0:00000000}", cargaCIOT.CIOT.Veiculo.RNTRC);
            }

            decimal valorAdiantamento = cargaCIOT.ContratoFrete.ValorAdiantamento;
            DateTime dataVencimentoAdiantamento = DateTime.Now.Date.AddDays(cargaCIOT.ContratoFrete.DiasVencimentoAdiantamento);

            if (valorAdiantamento > 0m)
            {
                contratoFrete.pagamentoFrete.adiantamento = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFretePagamentoFreteAdiantamento>()
                {
                    new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFretePagamentoFreteAdiantamento()
                    {
                        data = dataVencimentoAdiantamento.ToString("yyyy-MM-dd"),
                        tipoParcela = "Valor",
                        unidades = valorAdiantamento
                    }
                };
            }

            //Somente enviar se existir a contratação do frete retorno.
            //contratoFrete.retorno = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Retorno();
            //contratoFrete.retorno.CEPRetorno = "";
            //contratoFrete.retorno.distanciaRetorno = 0;

            string produto = "Frete";
            contratoFrete.produtos = new List<string>();
            contratoFrete.produtos.Add(produto);

            return contratoFrete;
        }

        private bool IntegrarRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem, ref string codigoRotaEmbarcador, ref int codigoRoteiro, ref string mensagemRetorno, ref string retornoJson, ref string envioJson, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool retorno = false;
            envioJson = "";
            retornoJson = "";

            string token = ObterToken(integracaoPagbem, out mensagemRetorno);

            if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                return false;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ConsultaRota consultaRota = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ConsultaRota()
            {
                nomeRota = carga.Rota.CodigoIntegracao
            };

            codigoRotaEmbarcador = carga.Rota.CodigoIntegracao;
            codigoRoteiro = 0;

            string url = integracaoPagbem.URLPagbem + "api/rotas";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("CNPJContratante", integracaoPagbem.CNPJEmpresaContratante);

            string jsonPost = JsonConvert.SerializeObject(consultaRota, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Pagbem");
            envioJson = jsonPost;

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");

            var result = client.PostAsync(url, content).Result;
            retornoJson = result.Content.ReadAsStringAsync().Result;
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = null;
            if (result.IsSuccessStatusCode)
            {
                retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Pagbem");
                if (retornoIntegracao.isSucesso)
                {
                    codigoRoteiro = retornoIntegracao.resultado.idRota;

                    retorno = true;
                }
                else
                {
                    mensagemRetorno = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração da rota na Pagbem.";
                    retorno = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");
                mensagemRetorno = result.Content.ReadAsStringAsync().Result;

                retorno = false;
            }

            if (retorno)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool BuscarValorPedagioRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem, int codigoRoteiro, ref string mensagemRetorno, ref string retornoJson, ref string envioJson, ref decimal valorPedagio, Repositorio.UnitOfWork unitOfWork, int qtdEixos)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            valorPedagio = 0;
            bool retorno = false;
            envioJson = "";
            retornoJson = "";

            string token = ObterToken(integracaoPagbem, out mensagemRetorno);

            if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                return false;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ConsultaValorPedagioRota consultaValorPedagioRota = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ConsultaValorPedagioRota()
            {
                IdRota = codigoRoteiro,
                QtdEixos = qtdEixos > 0 ? qtdEixos : carga.ModeloVeicularCarga.NumeroEixos.Value,
                tipoEixo = "simples"
            };

            string url = integracaoPagbem.URLPagbem + "api/rotas/" + codigoRoteiro + "/calcular/eixos/" + (qtdEixos > 0 ? qtdEixos : carga.ModeloVeicularCarga.NumeroEixos.Value);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("CNPJContratante", integracaoPagbem.CNPJEmpresaContratante);

            string jsonPost = JsonConvert.SerializeObject(consultaValorPedagioRota, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Pagbem");
            envioJson = jsonPost;

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");

            var result = client.GetAsync(url).Result;
            retornoJson = result.Content.ReadAsStringAsync().Result;
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = null;
            if (result.IsSuccessStatusCode)
            {
                retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Pagbem");
                if (retornoIntegracao.isSucesso)
                {
                    valorPedagio = retornoIntegracao.resultado.pedagioTotal;

                    retorno = true;
                }
                else
                {
                    mensagemRetorno = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração da rota na Pagbem.";
                    retorno = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");
                mensagemRetorno = result.Content.ReadAsStringAsync().Result;

                retorno = false;
            }

            if (retorno)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ConsultarSituacaoVeiculoSemParar(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem, ref string mensagemRetorno, ref string retornoJson, ref string envioJson, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool retorno = false;
            envioJson = "";
            retornoJson = "";

            string token = ObterToken(integracaoPagbem, out mensagemRetorno);

            if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                return false;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            string url = integracaoPagbem.URLPagbem + "/api/veiculos/" + veiculo.Placa + "/tag/situacao";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("CNPJContratante", integracaoPagbem.CNPJEmpresaContratante);

            string jsonPost = url;
            Servicos.Log.TratarErro(jsonPost, "Pagbem");
            envioJson = jsonPost;

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");

            var result = client.GetAsync(url).Result;
            retornoJson = result.Content.ReadAsStringAsync().Result;
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = null;
            if (result.IsSuccessStatusCode)
            {
                retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Pagbem");
                if (retornoIntegracao.isSucesso)
                {
                    retorno = retornoIntegracao.resultado.TAGs != null && retornoIntegracao.resultado.TAGs.Count > 0 && retornoIntegracao.resultado.TAGs.FirstOrDefault() != null && retornoIntegracao.resultado.TAGs.FirstOrDefault().isAptoPedagio.HasValue ? retornoIntegracao.resultado.TAGs.FirstOrDefault().isAptoPedagio.Value : false;
                }
                else
                {
                    mensagemRetorno = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha na consulta do veículo na SemParar.";
                    retorno = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");
                mensagemRetorno = result.Content.ReadAsStringAsync().Result;

                retorno = false;
            }

            if (retorno)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ConsultarRotaAtendidaSemParar(int codigoRoteiro, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem, ref string mensagemRetorno, ref string retornoJson, ref string envioJson, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool retorno = false;
            envioJson = "";
            retornoJson = "";

            string token = ObterToken(integracaoPagbem, out mensagemRetorno);

            if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                return false;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            string url = integracaoPagbem.URLPagbem + "/api/rotas/" + Utilidades.String.OnlyNumbers(codigoRoteiro.ToString("n0"));

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("CNPJContratante", integracaoPagbem.CNPJEmpresaContratante);

            string jsonPost = url;
            Servicos.Log.TratarErro(jsonPost, "Pagbem");
            envioJson = jsonPost;

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");

            var result = client.GetAsync(url).Result;
            retornoJson = result.Content.ReadAsStringAsync().Result;
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = null;
            if (result.IsSuccessStatusCode)
            {
                retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Pagbem");
                if (retornoIntegracao.isSucesso)
                {
                    retorno = retornoIntegracao.resultado.utilizavelCom != null && retornoIntegracao.resultado.utilizavelCom.TAG_SemParar.HasValue ? retornoIntegracao.resultado.utilizavelCom.TAG_SemParar.Value : false;
                }
                else
                {
                    mensagemRetorno = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha na consulta do veículo na SemParar.";
                    retorno = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");
                mensagemRetorno = result.Content.ReadAsStringAsync().Result;

                retorno = false;
            }

            if (retorno)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IntegrarCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem, int codigoRoteiro, decimal valorValePedagio, ref string mensagemErro, ref string retornoJson, ref string envioJson, ref int idViagem, ref string numeroComprovante, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            idViagem = 0;
            bool retorno = false;
            mensagemErro = string.Empty;

            string token = ObterToken(integracaoPagbem, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(carga.Terceiro, unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.CompraValePedagio compraValePedagio = ObterCompraValePedagio(carga, integracaoPagbem, modalidadeTerceiro, unitOfWork, valorValePedagio, codigoRoteiro, tipoServicoMultisoftware);

            string url = integracaoPagbem.URLPagbem + "api/viagens/pedagio";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("CNPJContratante", integracaoPagbem.CNPJEmpresaContratante);

            string jsonPost = JsonConvert.SerializeObject(compraValePedagio, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Pagbem");
            envioJson = jsonPost;
            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");

            var result = client.PostAsync(url, content).Result;
            retornoJson = result.Content.ReadAsStringAsync().Result;
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = null;
            if (result.IsSuccessStatusCode)
            {
                retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Pagbem");
                if (retornoIntegracao.isSucesso)
                {
                    retorno = true;
                    idViagem = retornoIntegracao.resultado.idViagem;
                    numeroComprovante = retornoIntegracao.resultado.dadosReciboPedagio?.idVpo;
                }
                else
                {
                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração cartão motorista na Pagbem.";
                    retorno = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");
                mensagemErro = result.Content.ReadAsStringAsync().Result;
                retorno = false;
            }

            if (retorno)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.CompraValePedagio ObterCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Repositorio.UnitOfWork unitOfWork, decimal valorPedagio, int codigoRoteiro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault();

            Dominio.Entidades.Cliente destinatario = null;
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteOrigem origem = null;
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = repPedidoXMLNotaFiscal.BuscarPedidoXMLNotaFiscalCarga(carga.Codigo);

            if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor)
                destinatario = cargaPedido.Recebedor;
            else
                destinatario = cargaPedido.Pedido.Destinatario;

            origem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteOrigem()
            {
                filialContratante = carga.Empresa?.CodigoIntegracao ?? string.Empty,
                remetente = cargaPedido.Pedido.Remetente.Nome,
                CNPJRemetente = cargaPedido.Pedido.Remetente.CPF_CNPJ_SemFormato,
                endereco = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteOrigemEndereco()
                {
                    logradouro = cargaPedido.Pedido.Remetente.Endereco,
                    numero = cargaPedido.Pedido.Remetente.Numero,
                    complemento = cargaPedido.Pedido.Remetente.Complemento,
                    bairro = cargaPedido.Pedido.Remetente.Bairro,
                    CEP = !string.IsNullOrWhiteSpace(cargaPedido.Pedido.Remetente.CEP) ? Utilidades.String.OnlyNumbers(cargaPedido.Pedido.Remetente.CEP.PadLeft(8, '0')) : "0",
                    codigoIBGE = cargaPedido.Pedido.Remetente.Localidade?.CodigoIBGE ?? 0,
                    KM = "",
                }
            };
            Dominio.Entidades.Veiculo carreta = carga.VeiculosVinculados?.FirstOrDefault() ?? null;
            Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault() ?? null;
            int eixosTotal = 0;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                eixosTotal = carga.Veiculo.ModeloVeicularCarga?.NumeroEixos.Value ?? 0;
                if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                {
                    foreach (var veinculoVinculado in carga.VeiculosVinculados)
                        eixosTotal += veinculoVinculado.ModeloVeicularCarga?.NumeroEixos.Value ?? 0;
                }
            }

            //string numeroCartaoPagBem = modalidadeTerceiro == null || modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? motorista.NumeroCartao : modalidadeTerceiro.NumeroCartao;
            string numeroCartaoPagBem = motorista.NumeroCartao;
            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.CompraValePedagio compraValePedagio = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.CompraValePedagio()
            {
                isViagemLiberada = !integracaoPagbem.LiberarViagemManualmente,
                numeroViagemCliente = carga.CodigoCargaEmbarcador,
                operador = carga.Operador != null ? carga.Operador.Login : "automatico",
                numeroCartaoPagbem = carga.Veiculo.PossuiTagValePedagio ? null : numeroCartaoPagBem,
                motorista = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteMotorista()
                {
                    CPF = motorista.CPF,
                    CNH = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteMotoristaCNH()
                    {
                        categoria = motorista.Categoria,
                        numero = !string.IsNullOrWhiteSpace(motorista.NumeroHabilitacao) ? motorista.NumeroHabilitacao.PadLeft(11, '0') : string.Empty,
                        validade = motorista.DataVencimentoHabilitacao.HasValue ? motorista.DataVencimentoHabilitacao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z" : string.Empty
                    },
                    numeroCartaoPagBem = carga.Veiculo.PossuiTagValePedagio ? null : numeroCartaoPagBem
                },
                origem = origem,
                destino = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDestino()
                {
                    destinatario = destinatario.Nome,
                    CNPJDestinatario = destinatario.CPF_CNPJ_SemFormato,
                    endereco = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteOrigemEndereco()
                    {
                        logradouro = destinatario.Endereco,
                        numero = destinatario.Numero,
                        complemento = destinatario.Complemento,
                        bairro = destinatario.Bairro,
                        CEP = !string.IsNullOrWhiteSpace(destinatario.CEP) ? Utilidades.String.OnlyNumbers(destinatario.CEP.PadLeft(8, '0')) : "0",
                        codigoIBGE = destinatario.Localidade?.CodigoIBGE ?? 0,
                        KM = "",
                    }
                },
                periodoViagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFretePeriodoViagem()
                {
                    inicio = carga.Pedidos.FirstOrDefault().Pedido.DataPrevisaoSaida.HasValue && carga.Pedidos.FirstOrDefault().Pedido.DataPrevisaoSaida.Value >= DateTime.Now ? carga.Pedidos.FirstOrDefault().Pedido.DataPrevisaoSaida.Value.ToString("yyyy-MM-ddTHH:mm:ss") : DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z",
                    termino = carga.Pedidos.FirstOrDefault().Pedido.PrevisaoEntrega.HasValue && carga.Pedidos.FirstOrDefault().Pedido.PrevisaoEntrega.Value >= DateTime.Now ? carga.Pedidos.FirstOrDefault().Pedido.PrevisaoEntrega.Value.ToString("yyyy-MM-ddTHH:mm:ss") : DateTime.Now.AddDays(15).ToString("yyyy-MM-ddTHH:mm:ss") + ".000Z",
                },
                rota = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteRota()
                {
                    idRota = codigoRoteiro,
                    meioPagamentoPedagio = carga.Veiculo.PossuiTagValePedagio ? "TAG_SemParar" : "Cartao",
                    qtdEixos = eixosTotal > 0 ? eixosTotal : carga.ModeloVeicularCarga?.NumeroEixos.Value ?? 2,
                    TipoEixo = (eixosTotal > 0 ? eixosTotal : carga.ModeloVeicularCarga?.NumeroEixos.Value ?? 2) == 2 ? "simples" : "duplo",
                    //valorPedagioAvulso = valorPedagio
                },
                veiculos = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteVeiculos()
                {
                    cavalo = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteVeiculosCavalo()
                    {
                        altoDesempenho = false,
                        placa = carga.Veiculo.Placa,
                        RNTRC = modalidadeTerceiro.RNTRC,
                        eixos = carga.Veiculo.ModeloVeicularCarga?.NumeroEixos.Value ?? 0
                    },
                    carretas = carga.VeiculosVinculados.Select(o => new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteVeiculosCarretas
                    {
                        placa = o.Placa,
                        RNTRC = modalidadeTerceiro.RNTRC,
                        eixos = o.ModeloVeicularCarga?.NumeroEixos.Value ?? 0
                    }).ToArray(),
                    numeroViagemCliente = carga.CodigoCargaEmbarcador,
                    operador = carga.Operador != null ? carga.Operador.Login : "automatico",
                },
                documentosViagem = carga.CargaCTes.Select(o => new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDocumentosViagem
                {
                    tipoDocumentoViagem = o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ? "NFS" : "CTe",
                    chaveAcessoDocumentoViagem = o.CTe.Chave,
                    documentoViagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDocumentosViagemDocumentoViagem()
                    {
                        serie = o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ? o.CTe.RPS?.Serie ?? "0" : o.CTe.Serie.Numero.ToString(),
                        numero = o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ? ((integracaoPagbem.IntegrarNumeroRPSNFSE ? o.CTe.RPS?.Numero.ToString() : o.CTe.Numero.ToString()) ?? "0") : o.CTe.Numero.ToString(),
                    },
                    carga = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDocumentosViagemCarga()
                    {
                        unidadeMedida = "kg",
                        quantidade = carga.DadosSumarizados.PesoTotal,
                        pesoSaidaKg = carga.DadosSumarizados.PesoTotal
                    },
                    NFes = (from nota in o.NotasFiscais
                            select new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ContratoFreteDocumentosViagemNFes
                            {
                                serie = !string.IsNullOrWhiteSpace(nota.PedidoXMLNotaFiscal.XMLNotaFiscal.SerieOuSerieDaChave) ? nota.PedidoXMLNotaFiscal.XMLNotaFiscal.SerieOuSerieDaChave : "0",
                                numero = nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString()
                            }).ToArray(),
                    valorDocumentoViagem = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(o.Carga.Codigo)
                }).ToArray()
            };

            return compraValePedagio;
        }

        private bool IntegrarQuitacaoCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, string token, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (string.IsNullOrWhiteSpace(token))
                token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Logistica.InformacaoDescarga repInformacaoDescarga = new Repositorio.Embarcador.Logistica.InformacaoDescarga(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Quitacao quitacao = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Quitacao();
            decimal peso = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga?.Codigo ?? 0);
            decimal pesoDescarga = repInformacaoDescarga.BuscarPesoPorCarga(cargaCIOT.Carga?.Codigo ?? 0);

            quitacao.pesoTotalChegadaKg = pesoDescarga > 0 ? pesoDescarga : peso > 0 ? peso : 1;
            quitacao.valorTotalAvarias = 0;
            quitacao.documentosEntregues = true;
            quitacao.observacoes = string.Empty;

            string url = configuracao.URLPagbem + "api/viagens/" + ciot.ProtocoloAutorizacao + "/quitacao";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", contratante.CNPJ);

            string jsonPost = JsonConvert.SerializeObject(quitacao, Formatting.Indented);
            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PutAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                if (retornoIntegracao.isSucesso)
                    return true;
                else
                {
                    string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                    Servicos.Log.TratarErro(jsonResult, "Pagbem");

                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração quitacao CIOT.";

                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                return false;
            }
        }

        private bool IntegrarAjusteFinanceiroCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, string token, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (string.IsNullOrWhiteSpace(token))
                token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.DebitoCredito debitoCredito = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.DebitoCredito()
            {
                motivo = "Outros",
                observacao = Utilidades.String.Left(justificativa.Descricao, 100),
                recebedorAjuste = modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador ? "Contratado" : "Motorista",
                valor = justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo ? valorMovimento : -valorMovimento
            };

            string url = configuracao.URLPagbem + "api/viagens/" + ciot.ProtocoloAutorizacao + "/ajustefinanceiro";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", ciot.Contratante.CNPJ);

            string jsonPost = JsonConvert.SerializeObject(debitoCredito, Formatting.Indented);

            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var content = new StringContent(jsonPost, Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                if (retornoIntegracao.isSucesso)
                    return true;
                else
                {
                    string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);

                    Servicos.Log.TratarErro(jsonResult, "Pagbem");

                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha na integração do débito/crédito do CIOT.";

                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                return false;
            }
        }

        private bool IntegrarCancelamentoCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, string token, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (string.IsNullOrWhiteSpace(token))
                token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Cancelamento cancelamento = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Cancelamento()
            {
                motivo = "ErroLancamento"
            };

            string url = configuracao.URLPagbem + "api/viagens/" + ciot.ProtocoloAutorizacao;

            string jsonPost = JsonConvert.SerializeObject(cancelamento, Formatting.Indented);

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Content = content,
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url)
            };

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", contratante.CNPJ);

            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var result = client.SendAsync(request).Result;

            string jsonResult = result.Content.ReadAsStringAsync().Result;

            bool sucesso = false;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(jsonResult);
                if (retornoIntegracao.isSucesso)
                {
                    mensagemErro = "Cancelamento realizado com sucesso.";
                    sucesso = true;
                }
                else
                {
                    if (retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 && retornoIntegracao.erros.Any(o => o.mensagem == "Viagem já cancelada"))
                    {
                        mensagemErro = "Cancelamento realizado com sucesso.";
                        sucesso = true;
                    }
                    else
                    {
                        mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração cancelamento CIOT.";
                        sucesso = false;
                    }                    
                }
            }
            else
            {
                mensagemErro = jsonResult;
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonPost, "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResult, "json", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = mensagemErro
            };

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(ciot);

            return sucesso;
        }

        private bool IntegrarCancelamentoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, string token, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (string.IsNullOrWhiteSpace(token))
                token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Cancelamento cancelamento = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.Cancelamento()
            {
                motivo = "ErroLancamento"
            };

            string url = configuracao.URLPagbem + "api/viagens/" + cargaValePedagio.IdCompraValePedagio;

            string jsonPost = JsonConvert.SerializeObject(cancelamento, Formatting.Indented);

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Content = content,
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url)
            };

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", contratante.CNPJ);

            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var result = client.SendAsync(request).Result;

            string jsonResult = result.Content.ReadAsStringAsync().Result;

            bool sucesso = false;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(jsonResult);
                if (retornoIntegracao.isSucesso)
                {
                    mensagemErro = "Cancelamento realizado com sucesso.";
                    sucesso = true;
                }
                else
                {
                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração cancelamento CIOT.";
                    sucesso = false;
                }
            }
            else
            {
                mensagemErro = jsonResult;
                sucesso = false;
            }

            return sucesso;
        }

        private bool IntegrarLiberacaoViagem(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            string token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            string url = $"{configuracao.URLPagbem}api/viagens/{ciot.ProtocoloAutorizacao}/liberar";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url)
            };

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", contratante.CNPJ);

            HttpResponseMessage result = client.SendAsync(request).Result;

            string jsonResult = result.Content.ReadAsStringAsync().Result;

            bool sucesso = false;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(jsonResult);
                if (retornoIntegracao.isSucesso)
                {
                    mensagemErro = "Liberação da viagem realizada com sucesso.";
                    sucesso = true;
                }
                else
                {
                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração liberar viagem CIOT.";
                    sucesso = false;
                }
            }
            else
            {
                mensagemErro = jsonResult;
                sucesso = false;
            }

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao("", "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResult, "json", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = mensagemErro
            };

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(ciot);

            return sucesso;
        }

        private bool IntegrarEstornarCarga(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out string jsonEnvio, out string jsonRetorno)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;
            mensagemErro = string.Empty;
            string token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            if (contratante == null && string.IsNullOrWhiteSpace(configuracao.CNPJEmpresaContratante))
            {
                mensagemErro = "Sem contratante informado.";
                return false;
            }

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            string url = configuracao.URLPagbem + "api/corporativo/" + (pagamentoMotorista.Motorista.NumeroCartao) + "/carga/" + (pagamentoMotorista.CodigoViagem);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", contratante.CNPJ);

            var result = client.DeleteAsync(url).Result;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                jsonRetorno = (string)result.Content.ReadAsStringAsync().Result;
                if (retornoIntegracao.isSucesso)
                    return true;
                else
                {
                    string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                    Servicos.Log.TratarErro(jsonResult, "Pagbem");

                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha integração quitacao CIOT.";

                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");
                jsonRetorno = (string)result.Content.ReadAsStringAsync().Result;
                mensagemErro = result.Content.ReadAsStringAsync().Result;
                return false;
            }
        }

        private bool IntegrarCarga(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out int codigoViagem, out string jsonEnvio, out string jsonRetorno)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;
            mensagemErro = string.Empty;
            codigoViagem = 0;

            string token = ObterToken(configuracao, out mensagemErro);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                return false;

            if (contratante == null && string.IsNullOrWhiteSpace(configuracao.CNPJEmpresaContratante))
            {
                mensagemErro = "Sem contratante informado.";
                return false;
            }

            Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ExecutarCarga executarCarga = new Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.ExecutarCarga()
            {
                CPFFuncionario = pagamentoMotorista.Motorista.CPF,
                numeroCartaoPagBem = pagamentoMotorista.Motorista.NumeroCartao,
                valorCarregamento = pagamentoMotorista.Valor
            };

            string url = configuracao.URLPagbem + "api/corporativo";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (configuracao.UtilizarCnpjContratanteIntegracao)
                client.DefaultRequestHeaders.Add("CNPJContratante", configuracao.CNPJEmpresaContratante);
            else
                client.DefaultRequestHeaders.Add("CNPJContratante", contratante.CNPJ);

            string jsonPost = JsonConvert.SerializeObject(executarCarga, Formatting.Indented);
            jsonEnvio = jsonPost;

            Servicos.Log.TratarErro(jsonPost, "Pagbem");

            var content = new StringContent(jsonPost, Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;


            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoIntegracao>(result.Content.ReadAsStringAsync().Result);
                jsonRetorno = (string)result.Content.ReadAsStringAsync().Result;
                if (retornoIntegracao.isSucesso)
                {
                    codigoViagem = Convert.ToInt32(retornoIntegracao.resultado?.idCargaCartao ?? 0m);
                    return true;
                }
                else
                {
                    string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);

                    Servicos.Log.TratarErro(jsonResult, "Pagbem");

                    mensagemErro = retornoIntegracao.erros != null && retornoIntegracao.erros.Count > 0 ? string.Join(", ", retornoIntegracao.erros.Select(o => o.codigo + " - " + o.mensagem)) : "Falha na integração da carga.";

                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Pagbem");
                jsonRetorno = (string)result.Content.ReadAsStringAsync().Result;
                mensagemErro = result.Content.ReadAsStringAsync().Result;
                return false;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoConsulta ConsultarPorViagemCliente(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, string numeroViagemCliente, string contratante, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string mensagemErro = string.Empty;

                string token = ObterToken(configuracao, out mensagemErro);

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    throw new Exception(mensagemErro);

                string url = $"{configuracao.URLPagbem}api/viagens/cliente/{contratante}?numeroViagemCliente={numeroViagemCliente}";

                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Pagbem));

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                client.DefaultRequestHeaders.Add("CNPJContratante", contratante);

                HttpResponseMessage result = client.SendAsync(request).Result;

                string jsonResult = result.Content.ReadAsStringAsync().Result;

                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao("", "json", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResult, "json", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                    Mensagem = mensagemErro
                };

                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                repCIOT.Atualizar(ciot);

                return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem.RetornoConsulta>(jsonResult);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return null;
            }
        }


        private void SalvarJsonIntegracao(ref Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, string retornoJson, string envioJson, string mensagemRetorno, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
            cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(envioJson, "json", unidadeDeTrabalho);
            cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoJson, "json", unidadeDeTrabalho);
            cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
            cargaValePedagioIntegracaoArquivo.Mensagem = Utilidades.String.Left(mensagemRetorno, 400);
            cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
            cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);
        }

        private void AdicionarIntegracaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao enumTipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repConfiguracaoIntegracaoGrupoPessoas = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(enumTipoIntegracao);

            if (tipoIntegracao == null)
                return;

            bool adicionarIntegracao = false;

            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repConfiguracaoIntegracaoGrupoPessoas.BuscarPorTipoIntegracao(enumTipoIntegracao);

            if (integracaoGrupoPessoas.Any(o => o.Habilitado))
            {
                if (carga.GrupoPessoaPrincipal != null && integracaoGrupoPessoas.Where(o => o.Habilitado).Any(o => o.GrupoPessoas.Codigo == carga.GrupoPessoaPrincipal.Codigo))
                    adicionarIntegracao = true;
            }
            else if (integracaoGrupoPessoas.Any(o => !o.Habilitado))
            {
                if (carga.GrupoPessoaPrincipal == null || !integracaoGrupoPessoas.Where(o => !o.Habilitado).Any(o => o.GrupoPessoas.Codigo == carga.GrupoPessoaPrincipal.Codigo))
                    adicionarIntegracao = true;
            }
            else
            {
                adicionarIntegracao = true;
            }

            List<TipoIntegracao> integracoesPorTipoOperacaoETipoCarga = servicoCarga.ObterTipoIntegracoesPorTipoOperacaoETipoCarga(carga, unitOfWork);
            if (integracoesPorTipoOperacaoETipoCarga.Count > 0)
                adicionarIntegracao = integracoesPorTipoOperacaoETipoCarga.Contains(enumTipoIntegracao);

            if (adicionarIntegracao && !repCargaIntegracao.ExistePorTipoIntegracao(carga.Codigo, tipoIntegracao.Codigo))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaIntegracao()
                {
                    Carga = carga,
                    TipoIntegracao = tipoIntegracao
                };

                repCargaIntegracao.Inserir(cargaIntegracao);
            }
        }

        private void AdicionarCargaDadosTransporteParaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, tipoIntegracao.Codigo, false);

            if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab && carga.CargaRecebidaDeIntegracao == true)
                return;

            if (cargaDadosTransporteIntegracao == null)
            {
                cargaDadosTransporteIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao()
                {
                    Carga = carga,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = "",
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                    TipoIntegracao = tipoIntegracao,
                };

                repCargaDadosTransporteIntegracao.Inserir(cargaDadosTransporteIntegracao);
            }
            else
            {
                if (tipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain)
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                }
            }
        }

        #endregion
    }
}
