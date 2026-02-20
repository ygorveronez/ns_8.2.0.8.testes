using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Senig;
using Dominio.ObjetosDeValor.Enumerador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public class Extratta
    {
        #region Métodos Globais

        public SituacaoRetornoCIOT IntegrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repositorioCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            
            bool sucesso = false;
            string mensagemErro = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao = ObterConfiguracaoExtratta(ciot.ConfiguracaoCIOT, unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repositorioCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPrimeiroPorCarga(cargaCIOT?.Carga?.Codigo ?? 0);

                #region Tratar Destinatario/Remetente Recebedor/Expedidor
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCIOT?.Carga?.Pedidos != null && cargaCIOT?.Carga?.Pedidos.Count > 0 ? cargaCIOT.Carga.Pedidos.FirstOrDefault() : null;

                Dominio.Entidades.Cliente destinatario = null, remetente = null;

                if ((cargaPedido?.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido?.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor) && cargaPedido?.Recebedor != null)
                    destinatario = cargaPedido.Recebedor;
                else
                    destinatario = pedido?.Destinatario;

                if ((cargaPedido?.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido?.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor) && cargaPedido?.Expedidor != null)
                    remetente = cargaPedido.Expedidor;
                else
                    remetente = pedido?.Remetente;

                #endregion

                if (pedido == null || remetente == null || destinatario == null)
                {
                    sucesso = false;
                    mensagemErro = "Não foi possível identificar cliente de origem/destino para realizar integração na Extratta.";
                }

                ciot.Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Extratta;

                if (ciot.Contratante == null)
                    ciot.Contratante = cargaCIOT.Carga.Empresa;

                if (ciot.Motorista == null)
                {
                    Dominio.Entidades.Usuario veiculoMotorista = null;
                    if (cargaCIOT.Carga.Veiculo != null)
                        veiculoMotorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(cargaCIOT.Carga.Veiculo.Codigo);

                    ciot.Motorista = cargaCIOT.Carga.Motoristas != null && cargaCIOT.Carga.Motoristas.Count > 0 ? cargaCIOT.Carga.Motoristas.FirstOrDefault() : veiculoMotorista ?? null;
                }

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);

                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, OperadoraCIOT.Extratta);


                bool validarANTT = cargaCIOT?.Carga?.TipoOperacao?.PossuiIntegracaoANTT ?? false;

                List<Dominio.Entidades.Cliente> proprietariosReboque = new List<Dominio.Entidades.Cliente>();
                foreach (var veiculoVinculado in ciot.VeiculosVinculados)
                {
                    if (veiculoVinculado.Proprietario != null && veiculoVinculado.Proprietario.CPF_CNPJ != ciot.Transportador.CPF_CNPJ && proprietariosReboque.Where(o => o.CPF_CNPJ == veiculoVinculado.Proprietario.CPF_CNPJ).FirstOrDefault() == null)
                        proprietariosReboque.Add(veiculoVinculado.Proprietario);
                }


                if (string.IsNullOrWhiteSpace(mensagemErro))
                {
                    if (IntegrarContratado(ciot, ciot.Transportador, modalidadeTerceiro, proprietariosReboque, ciot.Contratante, configuracao, unitOfWork, out mensagemErro) &&
                        IntegrarMotorista(ciot, ciot.Transportador, ciot.Motorista, modalidadeTerceiro, ciot.Contratante, configuracao, unitOfWork, out mensagemErro) &&
                        IntegrarCartao(ciot, ciot.Transportador, modalidadeTerceiro, ciot.Motorista, ciot.Contratante, configuracao, unitOfWork, out mensagemErro, tipoPagamentoCIOT) &&
                        IntegrarCadastroPix(ciot, ciot.Transportador, modalidadeTerceiro, ciot.Motorista, ciot.Contratante, configuracao, unitOfWork, out mensagemErro, tipoPagamentoCIOT, cargaCIOT) &&
                        IntegrarCliente(ciot, remetente, ciot.Motorista, ciot.Contratante, configuracao, unitOfWork, out mensagemErro) &&
                        IntegrarCliente(ciot, destinatario, ciot.Motorista, ciot.Contratante, configuracao, unitOfWork, out mensagemErro) &&
                        IntegrarVeiculo(ciot, ciot.Veiculo, ciot.VeiculosVinculados.ToList(), ciot.Transportador, ciot.Motorista, modalidadeTerceiro, ciot.Contratante, configuracao, unitOfWork, out mensagemErro) &&
                        ValidarRNTRCProprietario(ciot, ciot.Transportador, configuracao, validarANTT, unitOfWork, out mensagemErro) &&
                        IntegrarViagem(ciot, cargaCIOT, remetente, destinatario, ciot.Contratante, pedido, modalidadeTerceiro, configuracao, unitOfWork, out mensagemErro, tipoPagamentoCIOT))
                        sucesso = true;
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar a integração da Extratta.";
            }

            if (!sucesso)
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                ciot.Mensagem = mensagemErro;
            }

            if (ciot.Codigo > 0)
                repositorioCIOT.Atualizar(ciot);
            else
                repositorioCIOT.Inserir(ciot);

            return sucesso ? SituacaoRetornoCIOT.Autorizado : SituacaoRetornoCIOT.ProblemaIntegracao;
        }

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repositorioCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao = ObterConfiguracaoExtratta(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repositorioCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            if (configuracao.NaoRealizarQuitacaoViagemEncerramentoCIOT == true)
            {
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                cargaCIOT.CIOT.Mensagem = "O sistema não realizou a quitação da viagem devido a configuração de CIOT Extratta selecionada.";
                cargaCIOT.CIOT.DataEncerramento = DateTime.Now;
                mensagemErro = string.Empty;

                if (ciot.Codigo > 0)
                    repositorioCIOT.Atualizar(ciot);
                else
                    repositorioCIOT.Inserir(ciot);

                return true;
            }

            if (BaixarViagem(ciot, configuracao, unitOfWork, out mensagemErro))
            {
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                cargaCIOT.CIOT.Mensagem = "Quitação realizada com sucesso.";
                cargaCIOT.CIOT.DataEncerramento = DateTime.Now;

                if (ciot.Codigo > 0)
                    repositorioCIOT.Atualizar(ciot);
                else
                    repositorioCIOT.Inserir(ciot);

                return true;
            }

            return false;
        }

        public bool CancelarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repositorioCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao = ObterConfiguracaoExtratta(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repositorioCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            if (IntegrarCancelamentoViagem(ciot, configuracao, unitOfWork, out mensagemErro))
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                ciot.DataCancelamento = DateTime.Now;
                ciot.Mensagem = mensagemErro;

                if (ciot.Codigo > 0)
                    repositorioCIOT.Atualizar(ciot);
                else
                    repositorioCIOT.Inserir(ciot);

                return true;
            }

            return false;
        }

        public void EmitirPagamentoMotorista(int codigoPagamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoExtratta repIntegracaoExtratta = new Repositorio.Embarcador.Configuracoes.IntegracaoExtratta(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento = repPagamentoMotorista.BuscarPorCodigo(codigoPagamento);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(codigoPagamento);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta integracaoExtratta = repIntegracaoExtratta.Buscar();

            if (!(integracaoExtratta?.PossuiIntegracao ?? false))
                throw new ServicoException("Pagamento via Target não está configurado!");

            if (pagamento.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista) > 0)
            {
                
                IntegrarDespesa(unitOfWork, pagamento, pagamentoEnvio, configuracaoTMS, integracaoExtratta, tipoServicoMultisoftware, auditado, out string jsonEnvio, out string jsonRetorno, out string codigoRetorno);
                if (!string.IsNullOrEmpty(jsonEnvio) && !string.IsNullOrEmpty(jsonRetorno))
                {
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();
                    if (!string.IsNullOrWhiteSpace(codigoRetorno))
                        pagamentoRetorno.CodigoRetorno = codigoRetorno;

                    pagamentoRetorno.DescricaoRetorno = pagamentoEnvio.Retorno;
                    pagamentoRetorno.Data = DateTime.Now;
                    pagamentoRetorno.PagamentoMotoristaTMS = pagamento;
                    pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
                    pagamentoRetorno.ArquivoRetorno = jsonRetorno;

                    pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonEnvio, "json", unitOfWork);
                    pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork);
                    repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);
                }
            }
            else
            {
                SalvarPagamentoSemValor(ref pagamentoEnvio, ref pagamento);
            }
            repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
            repPagamentoMotorista.Atualizar(pagamento);            
        }

        public bool EstornarPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, out string mensagemErro, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTargetGeral repConfiguracaoIntegracaoTargetGeral = new Repositorio.Embarcador.Configuracoes.IntegracaoTargetGeral(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoExtratta repIntegracaoExtratta = new Repositorio.Embarcador.Configuracoes.IntegracaoExtratta(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta integracaoExtratta = repIntegracaoExtratta.Buscar();

            if (!(integracaoExtratta?.PossuiIntegracao ?? false))
                throw new ServicoException("Pagamento via Extratta não está configurado!");

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetornoIntegrado = repPagamentoMotoristaIntegracaoRetorno.BuscarPorCodigoPagamentoEComRetorno(pagamentoMotorista.Codigo);

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();

            bool sucesso = false;
            string codigoRetorno = string.Empty;
            string jsonPost = string.Empty;
            string jsonResult = string.Empty; 

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                mensagemErro = string.Empty;

                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.EstornarDespesaViagemIntegrar estornarDespesaViagemIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.EstornarDespesaViagemIntegrar();
                estornarDespesaViagemIntegrar.CNPJAplicacao = integracaoExtratta.CNPJAplicacao;
                estornarDespesaViagemIntegrar.Token = integracaoExtratta.Token;
                estornarDespesaViagemIntegrar.CNPJEmpresa = integracaoExtratta.CNPJEmpresa;
                estornarDespesaViagemIntegrar.NroControleIntegracao = pagamentoMotorista.Codigo.ToString("D");
                estornarDespesaViagemIntegrar.IdCargaAvulsa = pagamentoRetornoIntegrado?.CodigoRetorno ?? pagamentoMotorista.Codigo.ToString("D");

                string url = integracaoExtratta.URL + "CargaAvulsa/Estornar";

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("CNPJAplicacao", integracaoExtratta.CNPJAplicacao);
                client.DefaultRequestHeaders.Add("Token", integracaoExtratta.Token);
                client.DefaultRequestHeaders.Add("CNPJEmpresa", integracaoExtratta.CNPJEmpresa);

                jsonPost = JsonConvert.SerializeObject(estornarDespesaViagemIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                Servicos.Log.TratarErro(jsonPost, "Extratta");

                var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;

                sucesso = false;

                if (result.IsSuccessStatusCode)
                {
                    dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                    jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                    Servicos.Log.TratarErro(jsonResult, "Extratta");

                    if ((bool)retornoIntegracao.Sucesso)
                    {
                        mensagemErro = "Estorno do lançamento de despesa integrado com sucesso.";
                        sucesso = true;
                        codigoRetorno = (string)retornoIntegracao?.IdCargaAvulsa ?? "";
                    }
                    else
                    {
                        mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha integração do Estorno da Despesa na Extratta.";
                        sucesso = false;
                    }
                }
                else
                {
                    Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                    mensagemErro = result.Content.ReadAsStringAsync().Result;
                    sucesso = false;
                }
                //GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = ex.Message;
                sucesso = false;
            }

            pagamentoRetorno.CodigoRetorno = codigoRetorno;
            pagamentoRetorno.DescricaoRetorno = mensagemErro;
            pagamentoRetorno.Data = DateTime.Now;
            pagamentoRetorno.PagamentoMotoristaTMS = pagamentoMotorista;
            pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;

            pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonPost, "json", unitOfWork);
            pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResult, "json", unitOfWork);

            pagamentoRetorno.ArquivoRetorno = jsonResult;

            repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);

            return sucesso;
        }

        public bool AutorizarPagamentoParcelaCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela tipoAutorizacaoPagamentoCIOTParcela, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repositorioCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao = ObterConfiguracaoExtratta(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repositorioCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPrimeiroPorCarga(cargaCIOT?.Carga?.Codigo ?? 0);


            #region Tratar Destinatario/Remetente Recebedor/Expedidor
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCIOT?.Carga?.Pedidos != null && cargaCIOT?.Carga?.Pedidos.Count > 0 ? cargaCIOT.Carga.Pedidos.FirstOrDefault() : null;

            Dominio.Entidades.Cliente destinatario = null, remetente = null;

            if ((cargaPedido?.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido?.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor) && cargaPedido?.Recebedor != null)
                destinatario = cargaPedido.Recebedor;
            else
                destinatario = pedido?.Destinatario;

            if ((cargaPedido?.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || cargaPedido?.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor) && cargaPedido?.Expedidor != null)
                remetente = cargaPedido.Expedidor;
            else
                remetente = pedido?.Remetente;

            #endregion

            bool sucesso = false;
            mensagemErro = string.Empty;

            if (pedido == null || pedido?.Remetente == null || pedido?.Remetente == null)
            {
                sucesso = false;
                mensagemErro = "Não foi possível identificar cliente de origem/destino para realizar integração na Extratta.";
            }

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, OperadoraCIOT.Extratta);

            if ((tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Adiantamento && (cargaCIOT.ContratoFrete?.ValorAdiantamento ?? 0) > 0 && !string.IsNullOrWhiteSpace(ciot.ProtocoloAdiantamento)) ||
                (tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Abastecimento && (cargaCIOT.ContratoFrete?.ValorAbastecimento ?? 0) > 0 && !string.IsNullOrWhiteSpace(ciot.ProtocoloAbastecimento)) ||
                (tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Saldo && (cargaCIOT.ContratoFrete?.ValorSaldo ?? 0) > 0 && !string.IsNullOrWhiteSpace(ciot.ProtocoloSaldo)))
            {
                sucesso = IntegrarViagem(ciot, cargaCIOT, remetente, destinatario, ciot.Contratante, pedido, modalidadeTerceiro, configuracao, unitOfWork, tipoAutorizacaoPagamentoCIOTParcela, out mensagemErro, tipoPagamentoCIOT);
            }
            else
            {
                sucesso = false;
                mensagemErro = $"A parcela de {tipoAutorizacaoPagamentoCIOTParcela.ObterDescricao().ToLower()} não foi integrada com a Extrata.";
            }

            if (sucesso)
            {
                switch (tipoAutorizacaoPagamentoCIOTParcela)
                {
                    case TipoAutorizacaoPagamentoCIOTParcela.Adiantamento:
                        ciot.DataAutorizacaoPagamentoAdiantamento = DateTime.Now;
                        ciot.AdiantamentoPago = true;
                        break;
                    case TipoAutorizacaoPagamentoCIOTParcela.Abastecimento:
                        ciot.DataAutorizacaoPagamentoAbastecimento = DateTime.Now;
                        ciot.AbastecimentoPago = true;
                        break;
                    case TipoAutorizacaoPagamentoCIOTParcela.Saldo:
                        ciot.DataAutorizacaoPagamentoSaldo = DateTime.Now;
                        ciot.SaldoPago = true;
                        break;
                }
            }
            else
            {
                ciot.Mensagem = mensagemErro;
            }

            repositorioCIOT.Atualizar(ciot);

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private void IntegrarDespesa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta integracaoExtratta, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, out string jsonEnvio, out string jsonRetorno,out string codigoRetorno)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(pagamento.Motorista.Localidade?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(pagamento.Terceiro, unitOfWork);

            bool sucesso = false;
            codigoRetorno = string.Empty;
            string mensagemErro = string.Empty;

            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                long numeroCartao = !string.IsNullOrWhiteSpace(pagamento.Motorista.NumeroCartao.ObterSomenteNumeros()) ? pagamento.Motorista.NumeroCartao.ObterSomenteNumeros().ToLong() : modalidadeTerceiro.NumeroCartao.ObterSomenteNumeros().ToLong();
                string documento = pagamento.Motorista.CPF;

                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.MotoristaIntegrar motoristaIntegrar = PreencherMotoristaIntegrar(pagamento.Motorista, modalidadeTerceiro, integracaoExtratta.CNPJEmpresa, integracaoExtratta.CNPJAplicacao, integracaoExtratta.Token, localidade, null);
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CartaoIntegrar cartaoIntegrar = PreencherCartaoIntegrar(numeroCartao, documento, integracaoExtratta.CNPJEmpresa, integracaoExtratta.CNPJAplicacao, integracaoExtratta.Token, null);
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.DespesaViagemIntegrar despesaViagemIntegrar = PreencherDespesaViagem(pagamento, integracaoExtratta, configuracaoTMS);

                string urlMotoristaIntegrar = integracaoExtratta.URL + "Motorista/Integrar";
                string urlCartoesVincularCartao = integracaoExtratta.URL + "Cartoes/VincularCartao";
                string urlCargaAvulsaCarregar = integracaoExtratta.URL + "CargaAvulsa/Carregar";

                HttpClient clientIntegrarMotorista = ObterClientIntegracaoExtratta(integracaoExtratta, urlMotoristaIntegrar);
                HttpClient clientVincularCartao = ObterClientIntegracaoExtratta(integracaoExtratta, urlCartoesVincularCartao);
                HttpClient clientCarregarCargaAvulsa = ObterClientIntegracaoExtratta(integracaoExtratta, urlCargaAvulsaCarregar);

                jsonEnvio = JsonConvert.SerializeObject(motoristaIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                Servicos.Log.TratarErro(jsonEnvio, "Extratta");

                var contentMotoristaIntegrar = new StringContent(jsonEnvio.ToString(), Encoding.UTF8, "application/json");
                var resultMotoristaIntegrar = clientIntegrarMotorista.PostAsync(urlMotoristaIntegrar, contentMotoristaIntegrar).Result;

                if (resultMotoristaIntegrar.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoSucessoMensagemExtratta retornoIntegracaoMotorista = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoSucessoMensagemExtratta>(resultMotoristaIntegrar.Content.ReadAsStringAsync().Result);

                    jsonRetorno = JsonConvert.SerializeObject(retornoIntegracaoMotorista, Formatting.Indented);
                    Servicos.Log.TratarErro(jsonRetorno, "Extratta");

                    if (retornoIntegracaoMotorista.Sucesso)
                    {
                        jsonEnvio = JsonConvert.SerializeObject(cartaoIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                        Servicos.Log.TratarErro(jsonEnvio, "Extratta");

                        var contentVincularCartao = new StringContent(jsonEnvio.ToString(), Encoding.UTF8, "application/json");
                        var resultVincularCartao = clientVincularCartao.PostAsync(urlCartoesVincularCartao, contentVincularCartao).Result;

                        if (resultVincularCartao.IsSuccessStatusCode)
                        {
                            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoSucessoMensagemExtratta retornoIntegracaoVincularCartao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoSucessoMensagemExtratta>(resultVincularCartao.Content.ReadAsStringAsync().Result);

                            jsonRetorno = JsonConvert.SerializeObject(retornoIntegracaoVincularCartao, Formatting.Indented);
                            Servicos.Log.TratarErro(jsonRetorno, "Extratta");

                            if (retornoIntegracaoVincularCartao.Sucesso)
                            {
                                jsonEnvio = JsonConvert.SerializeObject(despesaViagemIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                                Servicos.Log.TratarErro(jsonEnvio, "Extratta");

                                var contentCargaAvulsaCarregar = new StringContent(jsonEnvio.ToString(), Encoding.UTF8, "application/json");
                                var resultCargaAvulsaCarregar = clientCarregarCargaAvulsa.PostAsync(urlCargaAvulsaCarregar, contentCargaAvulsaCarregar).Result;

                                if (resultCargaAvulsaCarregar.IsSuccessStatusCode)
                                {
                                    dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(resultCargaAvulsaCarregar.Content.ReadAsStringAsync().Result);

                                    jsonRetorno = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                                    Servicos.Log.TratarErro(jsonRetorno, "Extratta");

                                    if ((bool)retornoIntegracao.Sucesso)
                                    {
                                        mensagemErro = "Lançamento de despesa integrado com sucesso.";
                                        sucesso = true;
                                        codigoRetorno = (string)retornoIntegracao?.Objeto?.CargasAvulsas?[0].IdCargaAvulsa ?? "";
                                    }
                                    else
                                        throw new ServicoException(!string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha integração da Despesa na Extratta.");
                                }
                                else
                                {
                                    Servicos.Log.TratarErro(resultCargaAvulsaCarregar.Content.ReadAsStringAsync().Result, "Extratta");
                                    throw new ServicoException(resultCargaAvulsaCarregar.Content.ReadAsStringAsync().Result);
                                }
                            }
                            else
                                throw new ServicoException(!string.IsNullOrWhiteSpace(retornoIntegracaoVincularCartao.Mensagem) ? retornoIntegracaoVincularCartao.Mensagem : "Falha integração com Cartoes/VincularCartao da Extratta.");
                        }
                        else
                        {
                            Servicos.Log.TratarErro(resultVincularCartao.Content.ReadAsStringAsync().Result);
                            throw new ServicoException(resultVincularCartao.Content.ReadAsStringAsync().Result);
                        }
                    }
                    else
                        throw new ServicoException(!string.IsNullOrWhiteSpace(retornoIntegracaoMotorista.Mensagem) ? retornoIntegracaoMotorista.Mensagem : "Falha integração com Motorista/Integrar da Extratta.");
                }
                else
                {
                    Servicos.Log.TratarErro(resultMotoristaIntegrar.Content.ReadAsStringAsync().Result, "Extratta");
                    throw new ServicoException(resultMotoristaIntegrar.Content.ReadAsStringAsync().Result);
                }
            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;

                pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.FalhaIntegracao;
                pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = ex.Message;

                pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.FalhaIntegracao;
                pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }

            pagamentoEnvio.Retorno = mensagemErro;
            pagamentoEnvio.Data = DateTime.Now;
            pagamentoEnvio.ArquivoEnvio = jsonEnvio;
            pagamentoEnvio.TipoIntegracaoPagamentoMotorista = TipoIntegracaoPagamentoMotorista.Extratta;

            if (sucesso)
            {
                pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                pagamento.CodigoViagem = codigoRetorno.ToInt();
            }
        }

      

        private static HttpClient ObterClientIntegracaoExtratta(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta integracaoExtratta, string url)
        {
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", integracaoExtratta.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", integracaoExtratta.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", integracaoExtratta.CNPJEmpresa);

            return client;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.DespesaViagemIntegrar PreencherDespesaViagem(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta integracaoExtratta, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.DespesaViagemIntegrar despesaViagemIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.DespesaViagemIntegrar();
            despesaViagemIntegrar.CNPJAplicacao = integracaoExtratta.CNPJAplicacao;
            despesaViagemIntegrar.Token = integracaoExtratta.Token;
            despesaViagemIntegrar.CNPJEmpresa = integracaoExtratta.CNPJEmpresa;
            despesaViagemIntegrar.NroControleIntegracao = pagamento.Codigo.ToString("D");
            despesaViagemIntegrar.CPFUsuario = integracaoExtratta.Usuario;
            despesaViagemIntegrar.CargasAvulsas = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CargaAvulsa>();

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CargaAvulsa cargaAvulsa = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CargaAvulsa()
            {
                CPF = pagamento.Motorista.CPF,
                Documento = pagamento.Motorista.CPF,
                Nome = pagamento.Motorista.Nome,
                Observacao = pagamento.Observacao,
                Valor = Math.Round(pagamento.TotalPagamento(configuracaoTMS.NaoDescontarValorSaldoMotorista), 2),
            };
            despesaViagemIntegrar.CargasAvulsas.Add(cargaAvulsa);

            return despesaViagemIntegrar;
        }

        private bool IntegrarContratado(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, List<Dominio.Entidades.Cliente> proprietariosReboque, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (proprietario == null)
            {
                mensagemErro = "Integração de Proprietário - Proprietario não definido.";
                return true;
            }

            if (modalidade == null)
            {
                mensagemErro = "Integração de Proprietário - A modalidade do transportador não está configurada.";
                return false;
            }

            string url = configuracao.URLAPI + "Proprietario/Integrar";

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ProprietarioIntegrar proprietarioIntegrar = this.ObterContratado(ciot, proprietario, modalidade, contratante, configuracao, unitOfWork);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(proprietarioIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool?)retornoIntegracao.Sucesso ?? false)
                {
                    mensagemErro = "Proprietário integrado com sucesso.";
                    sucesso = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty((string)retornoIntegracao?.Result?.Mensagem))
                        mensagemErro = $"Falha integração proprietário/contratado: {(string)retornoIntegracao.Result.Mensagem}";
                    else
                        mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? $"Falha integração proprietário/contratado: {(string)retornoIntegracao.Mensagem}" : "Falha integração proprietário/contratado na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            #region Integrar Proprietários Reboque
            if (sucesso)
            {
                foreach (var proprietarioReboque in proprietariosReboque)
                {
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ProprietarioIntegrar proprietarioReboqueIntegrar = this.ObterContratado(ciot, proprietarioReboque, null, contratante, configuracao, unitOfWork);

                    client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
                    client.DefaultRequestHeaders.Add("Token", configuracao.Token);
                    client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

                    jsonPost = string.Empty;
                    jsonPost = JsonConvert.SerializeObject(proprietarioReboqueIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                    Servicos.Log.TratarErro(jsonPost, "Extratta");

                    content = null;
                    content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");

                    result = null;
                    result = client.PostAsync(url, content).Result;

                    sucesso = false;
                    jsonResult = string.Empty;

                    if (result.IsSuccessStatusCode)
                    {
                        dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                        jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                        Servicos.Log.TratarErro(jsonResult, "Extratta");

                        if ((bool?)retornoIntegracao.Sucesso ?? false)
                        {
                            mensagemErro = "Proprietário reboque integrado com sucesso.";
                            sucesso = true;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty((string)retornoIntegracao?.Result?.Mensagem))
                                mensagemErro = $"Falha integração proprietário reboque: {(string)retornoIntegracao.Result.Mensagem}";
                            else
                                mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? $"Falha integração proprietário reboque: {(string)retornoIntegracao.Mensagem}" : "Falha integração proprietário reboque na Extratta.";
                            sucesso = false;
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                        mensagemErro = result.Content.ReadAsStringAsync().Result;
                        sucesso = false;
                    }

                    GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);
                }
            }
            #endregion

            return sucesso;
        }

        private bool IntegrarMotorista(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (motorista == null)
            {
                mensagemErro = "Integração de Motorista - Nenhum motorista informado na carga/veiculo.";
                return false;
            }

            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(motorista.Localidade?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.MotoristaIntegrar motoristaIntegrar = PreencherMotoristaIntegrar(motorista, modalidade, contratante.CNPJ_SemFormato, configuracao.CNPJAplicacao, configuracao.Token, localidade, configuracao);

            string url = configuracao.URLAPI + "Motorista/Integrar";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(motoristaIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                {
                    mensagemErro = "Motorista integrado com sucesso.";
                    sucesso = true;
                }
                else
                {
                    mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha integração motorista na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            return sucesso;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.MotoristaIntegrar PreencherMotoristaIntegrar(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, string CNPJcontratante, string CNPJAplicacao, string token, Dominio.Entidades.Localidade localidade, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.MotoristaIntegrar motoristaIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.MotoristaIntegrar();

            motoristaIntegrar.CNPJAplicacao = CNPJAplicacao;
            motoristaIntegrar.Token = token;
            motoristaIntegrar.CNPJEmpresa = (configuracao?.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa ?? false) ? CNPJAplicacao : CNPJcontratante;
            motoristaIntegrar.Nome = motorista.Nome;
            motoristaIntegrar.RG = motorista.RG;
            motoristaIntegrar.RGOrgaoExpedidor = Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRGHelper.ObterDescricao(motorista.OrgaoEmissorRG ?? Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG.Nenhum);
            motoristaIntegrar.CPF = motorista.CPF;
            motoristaIntegrar.Sexo = motorista.Sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Feminino ? "F" : "M";
            motoristaIntegrar.CNH = !string.IsNullOrWhiteSpace(motorista.NumeroHabilitacao) ? motorista.NumeroHabilitacao.PadLeft(11, '0') : string.Empty;
            motoristaIntegrar.CNHCategoria = !string.IsNullOrWhiteSpace(motorista.Categoria) ? motorista.Categoria : "C";
            motoristaIntegrar.ValidadeCNH = motorista.DataVencimentoHabilitacao.HasValue ? motorista.DataVencimentoHabilitacao.Value.ToString("yyyy-MM-dd") : string.Empty;
            motoristaIntegrar.Celular = motorista.Celular;
            if (modalidade != null)
                motoristaIntegrar.TipoContrato = ObterTipoContrato(modalidade);
            else
                motoristaIntegrar.TipoContrato = 4;
            motoristaIntegrar.Email = motorista.Email;
            //motoristaIntegrar.Foto = ObterFotoMotorista(motorista, unitOfWork);
            motoristaIntegrar.CEP = motorista.CEP;
            motoristaIntegrar.Endereco = motorista.Endereco;
            motoristaIntegrar.Complemento = motorista.Complemento;
            motoristaIntegrar.Numero = motorista.NumeroEndereco;
            motoristaIntegrar.Bairro = motorista.Bairro;
            motoristaIntegrar.IBGECidade = localidade?.CodigoIBGE ?? 0;
            motoristaIntegrar.IBGEEstado = localidade?.Estado?.CodigoIBGE ?? 0;
            motoristaIntegrar.BACENPais = localidade?.Pais?.Codigo ?? 0;

            return motoristaIntegrar;
        }

        private bool IntegrarCartao(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (modalidade == null)
            {
                mensagemErro = "Integração de Cartão - A modalidade do transportador não está configurada.";
                return false;
            }

            if (motorista == null)
            {
                mensagemErro = "Integração de Cartão - Nenhum motorista informado na carga/veiculo.";
                return false;
            }

            if (tipoPagamentoCIOT != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
                return true;

            long numeroCartao;
            string documento;

            if (modalidade.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista)
            {
                numeroCartao = !string.IsNullOrWhiteSpace(motorista.NumeroCartao.ObterSomenteNumeros()) ? motorista.NumeroCartao.ObterSomenteNumeros().ToLong() : modalidade.NumeroCartao.ObterSomenteNumeros().ToLong();
                documento = motorista.CPF;
            }
            else
            {
                numeroCartao = modalidade.NumeroCartao.ObterSomenteNumeros().ToLong();
                documento = proprietario.CPF_CNPJ_SemFormato;
            }

            // Cartão não informado no cadastro
            if (numeroCartao == 0)
                return true;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CartaoIntegrar cartaoIntegrar = PreencherCartaoIntegrar(numeroCartao, documento, contratante.CNPJ_SemFormato, configuracao.CNPJAplicacao, configuracao.Token, configuracao);

            string url = configuracao.URLAPI + "Cartoes/VincularCartao";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(cartaoIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                {
                    mensagemErro = "Cartão integrado com sucesso.";
                    sucesso = true;
                }
                else
                {
                    mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha na integração do cartão na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            return sucesso;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CartaoIntegrar PreencherCartaoIntegrar(long numeroCartao, string documento, string CNPJcontratante, string CNPJAplicacao, string token, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CartaoIntegrar cartaoIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CartaoIntegrar();

            cartaoIntegrar.CNPJAplicacao = CNPJAplicacao;
            cartaoIntegrar.Token = token;
            cartaoIntegrar.CNPJEmpresa = (configuracao?.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa ?? false) ? CNPJAplicacao : CNPJcontratante;
            cartaoIntegrar.NumeroCartao = numeroCartao;
            cartaoIntegrar.Documento = documento;

            return cartaoIntegrar;
        }

        private bool IntegrarCliente(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (cliente == null)
            {
                mensagemErro = "Integração de Cliente - Cliente não informado.";
                return false;
            }

            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(cliente.Localidade?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ClienteIntegrar clienteIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ClienteIntegrar();
            clienteIntegrar.CNPJAplicacao = configuracao.CNPJAplicacao;
            clienteIntegrar.Token = configuracao.Token;
            clienteIntegrar.CNPJEmpresa = configuracao.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa ? configuracao.CNPJAplicacao : contratante.CNPJ_SemFormato;
            clienteIntegrar.IdCliente = null; //Será ignorado para não enviar a propriedade no JSON
            clienteIntegrar.BACENPais = localidade?.Pais?.Codigo ?? 0;
            clienteIntegrar.IBGEEstado = localidade?.Estado?.CodigoIBGE ?? 0;
            clienteIntegrar.IBGECidade = localidade?.CodigoIBGE ?? 0;
            clienteIntegrar.RazaoSocial = cliente.Nome;
            clienteIntegrar.NomeFantasia = !string.IsNullOrWhiteSpace(cliente.NomeFantasia) ? cliente.NomeFantasia : cliente.Nome;
            clienteIntegrar.TipoPessoa = cliente.Tipo == "F" ? 1 : 2;
            clienteIntegrar.CNPJCPF = cliente.CPF_CNPJ_SemFormato;
            clienteIntegrar.RG = cliente.Tipo == "F" ? cliente.IE_RG : string.Empty;
            clienteIntegrar.OrgaoExpedidorRG = cliente.Tipo == "F" ? Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRGHelper.ObterDescricao(motorista.OrgaoEmissorRG ?? Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG.Nenhum) : string.Empty;
            clienteIntegrar.IE = cliente.Tipo == "J" ? (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cliente.IE_RG)) ? Utilidades.String.OnlyNumbers(cliente.IE_RG).ToInt() : 0) : 0;
            clienteIntegrar.Celular = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cliente.Celular)) ? Utilidades.String.OnlyNumbers(cliente.Celular).ToInt() : 0;
            clienteIntegrar.Email = cliente.Email;
            clienteIntegrar.CEP = cliente.CEP;
            clienteIntegrar.Endereco = cliente.Endereco;
            clienteIntegrar.Complemento = cliente.Complemento;
            clienteIntegrar.Numero = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cliente.Numero)) ? Utilidades.String.OnlyNumbers(cliente.Numero).ToInt() : 0;
            clienteIntegrar.Bairro = cliente.Bairro;

            string url = configuracao.URLAPI + "Cliente/Integrar";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(clienteIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                {
                    mensagemErro = "Cliente integrado com sucesso.";
                    sucesso = true;
                }
                else
                {
                    mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha na integração de cliente na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            return sucesso;
        }

        private bool IntegrarFilial(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (cargaCIOT?.Carga.Empresa == null)
            {
                mensagemErro = "Integração de Filial - Filial não encontrada.";
                return false;
            }

            Dominio.Entidades.Empresa empresa = cargaCIOT?.Carga.Empresa;
            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(empresa.Localidade?.Codigo ?? 0);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.FilialIntegrar filialIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.FilialIntegrar();
            filialIntegrar.CNPJAplicacao = configuracao.CNPJAplicacao;
            filialIntegrar.Token = configuracao.Token;
            filialIntegrar.CNPJEmpresa = configuracao.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa ? configuracao.CNPJAplicacao : contratante.CNPJ_SemFormato;
            filialIntegrar.CNPJ = empresa.CNPJ_SemFormato;
            filialIntegrar.RazaoSocial = empresa.RazaoSocial;
            filialIntegrar.NomeFantasia = !string.IsNullOrWhiteSpace(empresa.NomeFantasia) ? empresa.NomeFantasia : empresa.RazaoSocial;
            filialIntegrar.Sigla = ""; // Não definido | É obrigatório
            filialIntegrar.CEP = empresa.CEP;
            filialIntegrar.Endereco = empresa.Endereco;
            filialIntegrar.Bairro = empresa.Bairro;
            filialIntegrar.Telefone = empresa.Telefone;
            filialIntegrar.Email = empresa.Email;
            filialIntegrar.CodigoBACENPais = localidade?.Pais?.Codigo ?? 0;
            filialIntegrar.codigoIbgeEstado = localidade?.Estado?.CodigoIBGE ?? 0;
            filialIntegrar.codigoIbgeCidade = localidade?.CodigoIBGE ?? 0;

            string url = configuracao.URLAPI + "Filial/Integrar";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(filialIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                {
                    mensagemErro = "Filial integrado com sucesso.";
                    sucesso = true;
                }
                else
                {
                    mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha na integração de filial na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            return sucesso;
        }

        private bool IntegrarVeiculo(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Veiculo> reboques, Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (veiculo == null)
            {
                mensagemErro = "Integração de Veiculos - Veículo não informado.";
                return false;
            }

            //if (reboques?.Count <= 0)
            //{
            //	mensagemErro = "Integração de Veiculos - Reboques do veículo não informado.";
            //	return false;
            //}

            // Tração
            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar veiculoIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar();
            veiculoIntegrar.CNPJAplicacao = configuracao.CNPJAplicacao;
            veiculoIntegrar.Token = configuracao.Token;
            veiculoIntegrar.CPFCNPJProprietario = proprietario.CPF_CNPJ_SemFormato;
            veiculoIntegrar.CPFMotorista = motorista.CPF;
            veiculoIntegrar.CNPJEmpresa = configuracao.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa ? configuracao.CNPJAplicacao : contratante.CNPJ_SemFormato;
            veiculoIntegrar.Placa = veiculo.Placa;
            veiculoIntegrar.Chassi = veiculo.Chassi;
            veiculoIntegrar.Renavam = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(veiculo.Renavam)) ? Utilidades.String.OnlyNumbers(veiculo.Renavam).ToInt() : 0;
            veiculoIntegrar.AnoFabricacao = veiculo.AnoFabricacao;
            veiculoIntegrar.AnoModelo = veiculo.AnoModelo;
            veiculoIntegrar.Marca = veiculo.Marca?.Descricao ?? string.Empty;
            veiculoIntegrar.Modelo = veiculo.Modelo?.Descricao ?? string.Empty;
            veiculoIntegrar.ComTracao = veiculo.TipoVeiculo == "0" ? true : false;
            veiculoIntegrar.TipoContrato = ObterTipoContrato(modalidade);
            veiculoIntegrar.QuantidadeEixos = veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0;
            veiculoIntegrar.IdTecnologia = null;
            veiculoIntegrar.IBGECidade = null;

            // Reboques
            List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar> reboquesIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar>();
            foreach (Dominio.Entidades.Veiculo reboque in reboques)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar reboqueIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar();
                reboqueIntegrar.CNPJAplicacao = configuracao.CNPJAplicacao;
                reboqueIntegrar.Token = configuracao.Token;
                reboqueIntegrar.CPFCNPJProprietario = reboque?.Proprietario?.CPF_CNPJ_SemFormato;
                reboqueIntegrar.CPFMotorista = motorista.CPF;
                reboqueIntegrar.CNPJEmpresa = configuracao.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa ? configuracao.CNPJAplicacao : contratante.CNPJ_SemFormato;
                reboqueIntegrar.Placa = reboque.Placa;
                reboqueIntegrar.Chassi = reboque.Chassi;
                reboqueIntegrar.Renavam = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(reboque.Renavam)) ? Utilidades.String.OnlyNumbers(reboque.Renavam).ToInt() : 0;
                reboqueIntegrar.AnoFabricacao = reboque.AnoFabricacao;
                reboqueIntegrar.AnoModelo = reboque.AnoModelo;
                reboqueIntegrar.Marca = reboque.Marca?.Descricao ?? string.Empty;
                reboqueIntegrar.Modelo = reboque.Modelo?.Descricao ?? string.Empty;
                reboqueIntegrar.ComTracao = reboque.TipoVeiculo == "0" ? true : false;
                reboqueIntegrar.TipoContrato = ObterTipoContrato(modalidade);
                reboqueIntegrar.QuantidadeEixos = reboque.ModeloVeicularCarga?.NumeroEixos ?? 0;

                reboquesIntegrar.Add(reboqueIntegrar);
            }

            string url = configuracao.URLAPI + "Veiculo/Integrar";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(veiculoIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoVeiculo retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoVeiculo>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if (retornoIntegracao.Sucesso)
                {
                    mensagemErro = "Veículo integrado com sucesso.";
                    sucesso = true;
                }
                else
                {
                    mensagemErro = !string.IsNullOrWhiteSpace(retornoIntegracao.Mensagem) ? retornoIntegracao.Mensagem : "Falha na integração de veículo na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            if (sucesso)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar reboque in reboquesIntegrar)
                {
                    if (sucesso)
                    {
                        client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
                        client.DefaultRequestHeaders.Add("Token", configuracao.Token);
                        client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

                        jsonPost = string.Empty;
                        jsonPost = JsonConvert.SerializeObject(reboque, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                        Servicos.Log.TratarErro(jsonPost, "Extratta");

                        content = null;
                        content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");

                        result = null;
                        result = client.PostAsync(url, content).Result;

                        sucesso = false;
                        jsonResult = string.Empty;

                        if (result.IsSuccessStatusCode)
                        {
                            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoVeiculo retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoVeiculo>(result.Content.ReadAsStringAsync().Result);

                            jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                            Servicos.Log.TratarErro(jsonResult, "Extratta");

                            if (retornoIntegracao.Sucesso)
                            {
                                mensagemErro = "Reboque integrado com sucesso.";
                                sucesso = true;
                            }
                            else
                            {
                                mensagemErro = !string.IsNullOrWhiteSpace(retornoIntegracao.Mensagem) ? retornoIntegracao.Mensagem : "Falha na integração de reboque na Extratta.";
                                sucesso = false;
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                            mensagemErro = result.Content.ReadAsStringAsync().Result;
                            sucesso = false;
                        }

                        GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);
                    }
                }
            }

            return sucesso;
        }

        private bool ValidarRNTRCProprietario(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, bool validarANTT, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (!validarANTT)
                return true;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ConsultaSituacaoANTT consultaSituacaoANTT = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ConsultaSituacaoANTT();
            consultaSituacaoANTT.CNPJAplicacao = configuracao.CNPJAplicacao;
            consultaSituacaoANTT.Token = configuracao.Token;
            consultaSituacaoANTT.CpfCnpj = proprietario.CPF_CNPJ_SemFormato;

            string url = configuracao.URLAPI + "Proprietario/ConsultarSituacaoANTT";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(consultaSituacaoANTT, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoSituacaoANTT retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoSituacaoANTT>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                {
                    if (retornoIntegracao.Objeto.RNTRCAtivo)
                    {
                        mensagemErro = "RNTRC do proprietario está ativo.";
                        sucesso = true;
                    }
                    else
                    {
                        mensagemErro = !string.IsNullOrWhiteSpace(retornoIntegracao.Mensagem) ? retornoIntegracao.Mensagem : "RNTRC do proprietario está inativo.";
                        sucesso = false;
                    }
                }
                else
                {
                    mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha ao validar RNTRC na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            return sucesso;
        }

        private bool IntegrarViagem(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Cliente clienteOrigem, Dominio.Entidades.Cliente clienteDestino, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            return IntegrarViagem(ciot, cargaCIOT, clienteOrigem, clienteDestino, contratante, pedido, modalidadeTerceiro, configuracao, unitOfWork, null, out mensagemErro, tipoPagamentoCIOT);
        }

        private bool IntegrarViagem(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Cliente clienteOrigem, Dominio.Entidades.Cliente clienteDestino, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela? tipoAutorizacaoPagamentoCIOTParcela, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            var repIntegracaoExtratta = new Repositorio.Embarcador.Configuracoes.IntegracaoExtratta(unitOfWork);
            var integracaoExtratta = repIntegracaoExtratta.Buscar();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var repositorioCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unitOfWork);

            mensagemErro = string.Empty;

            int ncm = 0;
            if (!string.IsNullOrWhiteSpace(cargaCIOT.Carga?.TipoDeCarga?.NCM.ObterSomenteNumeros()) && cargaCIOT.Carga?.TipoDeCarga?.NCM.ObterSomenteNumeros().Length > 3)
                ncm = cargaCIOT.Carga?.TipoDeCarga?.NCM.ObterSomenteNumeros().Substring(0, 4).ToInt() ?? 0;

            var integrarViagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.IntegrarViagem();

            integrarViagem.Token = configuracao.Token;
            integrarViagem.IdViagem = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(ciot.ProtocoloAutorizacao)) ? Utilidades.String.OnlyNumbers(ciot.ProtocoloAutorizacao).ToNullableInt() : null;
            integrarViagem.CNPJAplicacao = configuracao.CNPJAplicacao;
            integrarViagem.CNPJEmpresa = configuracao.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa ? configuracao.CNPJAplicacao : contratante.CNPJ_SemFormato;
            integrarViagem.CPFCNPJClienteOrigem = clienteOrigem.CPF_CNPJ_SemFormato;
            integrarViagem.CPFCNPJClienteDestino = clienteDestino.CPF_CNPJ_SemFormato;
            integrarViagem.CPFCNPJProprietario = ciot?.Transportador?.CPF_CNPJ_SemFormato ?? string.Empty;
            integrarViagem.CNPJFilial = cargaCIOT.Carga.Empresa.CNPJ_SemFormato;
            integrarViagem.CPFMotorista = ciot.Motorista.CPF;
            integrarViagem.Placa = ciot.Veiculo.Placa;
            integrarViagem.StatusViagem = 1;

            if (ciot.VeiculosVinculados?.Count > 0)
            {
                if (configuracao.EnviarCarretaViagemV2)
                {

                    integrarViagem.CarretasViagemV2 = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CarretaViagem>();
                    foreach (var veiculo in ciot.VeiculosVinculados)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CarretaViagem carretaviagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CarretaViagem();
                        carretaviagem.RNTRC = veiculo.RNTRC.ToString();
                        carretaviagem.Placa = veiculo.Placa;
                        if (veiculo.Proprietario != null)
                            carretaviagem.CPFCNPJProprietario = veiculo.Proprietario?.CPF_CNPJ.ToString();
                        else
                            carretaviagem.CPFCNPJProprietario = veiculo.EmpresaFilial?.CNPJ.ToString();
                        integrarViagem.CarretasViagemV2.Add(carretaviagem);
                    }
                }
                else
                    integrarViagem.Carretas = ciot.VeiculosVinculados.Select(o => o.Placa).ToList();
            }
            else
                integrarViagem.Carretas = new List<string>();

            integrarViagem.DocumentoCliente = cargaCIOT.ContratoFrete.NumeroContrato.ToString();
            integrarViagem.PesoSaida = cargaCIOT.PesoBruto;

            if (configuracao.EnviarQuantidadesMaioresQueZero)
            {
                if ((integrarViagem.PesoSaida ?? 0) <= 0)
                    integrarViagem.PesoSaida = 1;
            }

            integrarViagem.IRRPF = cargaCIOT.ContratoFrete.ValorIRRF;
            integrarViagem.INSS = cargaCIOT.ContratoFrete.ValorINSS;
            integrarViagem.SESTSENAT = cargaCIOT.ContratoFrete.ValorSEST + cargaCIOT.ContratoFrete.ValorSENAT;
            integrarViagem.DataEmissao = cargaCIOT.ContratoFrete.DataEmissaoContrato.ToString("yyyy-MM-dd HH:mm:ss");
            integrarViagem.HabilitarDeclaracaoCiot = true;
            integrarViagem.NaturezaCarga = ncm;
            integrarViagem.NumeroControle = (string.IsNullOrEmpty(configuracao.PrefixoCampoNumeroControle) ? "" : configuracao.PrefixoCampoNumeroControle) + (cargaCIOT.Carga?.Codigo ?? 0).ToString();
            integrarViagem.ForcarCiotNaoEquiparado = configuracao.ForcarCIOTNaoEquiparado;
            integrarViagem.IdCarga = null;

            if (configuracao.ConfiguracaoCIOT.UtilizarDataAtualComoInicioTerminoCIOT)
            {
                integrarViagem.DataColeta = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                integrarViagem.DataPrevisaoEntrega = DateTime.Now.AddDays(configuracao.ConfiguracaoCIOT.DiasTerminoCIOT ?? 1).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                integrarViagem.DataColeta = cargaCIOT.Carga.DataInicioViagem?.ToString("yyyy-MM-dd HH:mm:ss") ?? pedido.DataCarregamentoPedido?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
                integrarViagem.DataPrevisaoEntrega = cargaCIOT.Carga.Pedidos?.FirstOrDefault()?.Pedido?.DataAgendamento?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
            }

            integrarViagem.ViagemRegra = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemRegra>();
            
            var viagemRegra = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemRegra();

            viagemRegra.IdViagemRegra = null;
            viagemRegra.TaxaAntecipacao = 0;
            viagemRegra.ToleranciaPeso = 0;
            viagemRegra.TarifaTonelada = 0;
            viagemRegra.TipoQuebraMercadoria = 1;
            integrarViagem.ViagemRegra.Add(viagemRegra);

            integrarViagem.DadosPagamento = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.DadoPagamento();
            if (modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador && (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Deposito || tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Transferencia))
            {
                if (ciot.Transportador.Banco != null && !string.IsNullOrWhiteSpace(ciot.Transportador.NumeroConta) && !string.IsNullOrWhiteSpace(ciot.Transportador.Agencia))
                {
                    integrarViagem.DadosPagamento.FormaPagamento = ObterFormaPagamento(ciot.Transportador.TipoContaBanco ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Nenhum);
                    integrarViagem.DadosPagamento.CodigoBacen = string.Format("{0:0000}", ciot.Transportador.Banco.Numero);
                    integrarViagem.DadosPagamento.Agencia = ciot.Transportador.Agencia;
                    integrarViagem.DadosPagamento.Conta = ciot.Transportador.NumeroConta;
                }
            }
            else if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao)
            {
                integrarViagem.DadosPagamento.FormaPagamento = 1; // Cartão
            }

            integrarViagem.ViagemEventos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemEvento>();

            if (cargaCIOT.ContratoFrete?.ValorAdiantamento > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemEvento eventoAdiantamento = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemEvento()
                {
                    IdViagemEvento = !string.IsNullOrWhiteSpace(ciot.ProtocoloAdiantamento.ObterSomenteNumeros()) ? ciot.ProtocoloAdiantamento.ObterSomenteNumeros().ToNullableInt() : null,
                    TipoEvento = 0, // Adiantamento
                    ValorPagamento = cargaCIOT.ContratoFrete?.ValorAdiantamento ?? 0,
                    Status = (tipoAutorizacaoPagamentoCIOTParcela.HasValue && tipoAutorizacaoPagamentoCIOTParcela.Value == TipoAutorizacaoPagamentoCIOTParcela.Adiantamento) || ciot.AdiantamentoPago ? 2 : 0, // 2 - Baixado/Efetivado | 0 - Aberto/Pendente
                    HabilitarPagamentoCartao = tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao,
                    ViagemDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemDocumento>(),
                    ViagemOutrosAcrescimos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemOutroAcrescimo>(),
                    ViagemOutrosDescontos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemOutroDesconto>()
                };
                integrarViagem.ViagemEventos.Add(eventoAdiantamento);
            }

            if (cargaCIOT.ContratoFrete?.ValorAbastecimento > 0)
            {
                if (!tipoAutorizacaoPagamentoCIOTParcela.HasValue || tipoAutorizacaoPagamentoCIOTParcela != TipoAutorizacaoPagamentoCIOTParcela.Saldo)
                {
                    var eventoAbastecimento = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemEvento()
                    {
                        IdViagemEvento = !string.IsNullOrWhiteSpace(ciot.ProtocoloAbastecimento.ObterSomenteNumeros()) ? ciot.ProtocoloAbastecimento.ObterSomenteNumeros().ToNullableInt() : null,
                        TipoEvento = 5, // Abastecimento
                        ValorPagamento = cargaCIOT.ContratoFrete?.ValorAbastecimento ?? 0,
                        Status = (tipoAutorizacaoPagamentoCIOTParcela.HasValue && tipoAutorizacaoPagamentoCIOTParcela.Value == TipoAutorizacaoPagamentoCIOTParcela.Abastecimento) || ciot.AbastecimentoPago ? 2 : 0, // 2 - Baixado/Efetivado | 0 - Aberto/Pendente
                        HabilitarPagamentoCartao = tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao,
                        ViagemDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemDocumento>(),
                        ViagemOutrosAcrescimos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemOutroAcrescimo>(),
                        ViagemOutrosDescontos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemOutroDesconto>()
                    };

                    // Envia abastecimento para extratta se houver valor de abastecimento no contrato
                    if (integracaoExtratta.IntegrarAbastecimentoComTicketLog && cargaCIOT.ContratoFrete?.ValorAbastecimento > 0)
                    {
                        eventoAbastecimento.ValorPagamento = cargaCIOT.ContratoFrete.ValorAbastecimento;
                        eventoAbastecimento.Status = 2; // <-- Se integra com a ticketlog, ajusta para enviar sempre 2 (baixado)
                        eventoAbastecimento.Instrucao = "Abastecimento TicketLog";
                        eventoAbastecimento.HabilitarPagamentoCartao = false;
                        eventoAbastecimento.NumeroControle = (string.IsNullOrEmpty(configuracao.PrefixoCampoNumeroControle) ? "" : configuracao.PrefixoCampoNumeroControle) + (cargaCIOT.Carga?.Codigo ?? 0).ToString() + "-" + (eventoAbastecimento.TipoEvento).ToString();
                        eventoAbastecimento.DadosAbastecimento = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.DadosAbastecimento
                        {
                            CodigoCredito = "",
                            CodigoCliente = integracaoExtratta.CodigoClienteTicketLog,
                            CodigoProduto = integracaoExtratta.CodigoProdutoTicketLog,
                            numerocartao = cargaCIOT.Carga.Veiculo?.NumeroCartaoAbastecimento ?? "",
                            DataValidade = "",
                            DataLiberacao = ""
                        };
                    }

                    integrarViagem.ViagemEventos.Add(eventoAbastecimento);
                }
            }

            if (cargaCIOT.ContratoFrete?.SaldoAReceber > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemEvento eventoSaldo = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemEvento()
                {
                    IdViagemEvento = !string.IsNullOrWhiteSpace(ciot.ProtocoloSaldo.ObterSomenteNumeros()) ? ciot.ProtocoloSaldo.ObterSomenteNumeros().ToNullableInt() : null,
                    TipoEvento = 1, // Saldo
                    ValorPagamento = cargaCIOT.ContratoFrete?.SaldoAReceber ?? 0,
                    Status = (tipoAutorizacaoPagamentoCIOTParcela.HasValue && tipoAutorizacaoPagamentoCIOTParcela.Value == TipoAutorizacaoPagamentoCIOTParcela.Saldo) || ciot.SaldoPago ? 2 : 0, // 2 - Baixado/Efetivado | 0 - Aberto/Pendente
                    HabilitarPagamentoCartao = tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao,
                    ViagemDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemDocumento>(),
                    ViagemOutrosAcrescimos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemOutroAcrescimo>(),
                    ViagemOutrosDescontos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ViagemOutroDesconto>()
                };
                integrarViagem.ViagemEventos.Add(eventoSaldo);
            }

            integrarViagem.DocumentosFiscais = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.DocumentoFiscal>();

            foreach (Dominio.Entidades.Embarcador.Documentos.CIOTCTe ciotCTe in ciot.CTes)
            {
                var cte = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.DocumentoFiscal()
                {
                    IdViagemDocumentoFiscal = !string.IsNullOrWhiteSpace(ciotCTe.ProtocoloDocumentoFiscal.ObterSomenteNumeros()) ? ciotCTe.ProtocoloDocumentoFiscal.ObterSomenteNumeros().ToNullableInt() : null,
                    NumeroDocumento = ciotCTe.CargaCTe.CTe.Numero,
                    Serie = ciotCTe.CargaCTe.CTe.Serie?.Numero.ToString() ?? string.Empty,
                    TipoDocumento = 1 // CT-e
                };

                integrarViagem.DocumentosFiscais.Add(cte);
            }

            string url = configuracao.URLAPI + "Viagem/Integrar";

            var client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(integrarViagem, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                {
                    if (!string.IsNullOrWhiteSpace((string)retornoIntegracao.Objeto.IdViagem))
                        ciot.ProtocoloAutorizacao = (string)retornoIntegracao.Objeto.IdViagem;

                    bool gravouIdsDocumentosFiscais = false;
                    foreach (dynamic evento in retornoIntegracao.Objeto.Eventos)
                    {
                        int tipoEvento = (int)evento.TipoEventoViagem;
                        string idViagemEvento = (string)evento.IdViagemEvento;

                        if (!string.IsNullOrWhiteSpace(idViagemEvento))
                        {
                            if (tipoEvento == 0)
                                ciot.ProtocoloAdiantamento = idViagemEvento;
                            else if (tipoEvento == 1)
                                ciot.ProtocoloSaldo = idViagemEvento;
                            else if (tipoEvento == 5)
                                ciot.ProtocoloAbastecimento = idViagemEvento;
                        }

                        if (!gravouIdsDocumentosFiscais)
                        {
                            foreach (dynamic idDocumentoFiscal in evento.IdsViagemDocumento)
                            {
                                foreach (Dominio.Entidades.Embarcador.Documentos.CIOTCTe ciotCTe in ciot.CTes)
                                {
                                    if (ciotCTe.CargaCTe.CTe.Numero == (int)idDocumentoFiscal.NumeroDocumento)
                                    {
                                        ciotCTe.ProtocoloDocumentoFiscal = (string)idDocumentoFiscal.IdViagemDocumento;

                                        repositorioCIOTCTe.Atualizar(ciotCTe);
                                    }
                                }
                            }

                            gravouIdsDocumentosFiscais = true;
                        }
                    }

                    if (string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) && (retornoIntegracao.Objeto.CIOT != null && (bool)retornoIntegracao.Objeto.CIOT.Declarado == true && (int)retornoIntegracao.Objeto.CIOT.Resultado == 0))
                    {
                        ciot.Numero = (string)retornoIntegracao.Objeto.CIOT.Dados.Ciot;
                        ciot.CodigoVerificador = (string)retornoIntegracao.Objeto.CIOT.Dados.Verificador;

                        ciot.DataAbertura = DateTime.Now;
                        ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                        ciot.Mensagem = "CIOT integrado com sucesso.";
                        mensagemErro = "CIOT integrado com sucesso";
                        sucesso = true;
                    }
                    else
                    {
                        mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha ao integrar viagem na Extratta.";
                        sucesso = false;
                    }
                }
                else
                {
                    mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha ao integrar viagem na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            return sucesso;
        }

        private bool IntegrarCancelamentoViagem(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            // Se não possuir protocolo de autorização então ainda não foi integrado o CIOT. Desta forma não precisa cancelar o CIOT.
            if (string.IsNullOrWhiteSpace(ciot.ProtocoloAutorizacao.ObterSomenteNumeros()))
                return true;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CancelamentoViagem cancelamentoViagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CancelamentoViagem();
            cancelamentoViagem.CNPJAplicacao = configuracao.CNPJAplicacao;
            cancelamentoViagem.Token = configuracao.Token;
            cancelamentoViagem.DataEvento = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            cancelamentoViagem.CodigoViagem = !string.IsNullOrWhiteSpace(ciot.ProtocoloAutorizacao.ObterSomenteNumeros()) ? ciot.ProtocoloAutorizacao.ObterSomenteNumeros().ToInt() : 0;

            string url = configuracao.URLAPI + "Viagem/CancelarViagem";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(cancelamentoViagem, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                {
                    mensagemErro = "Integrado cancelamento do CIOT com sucesso.";
                    sucesso = true;
                }
                else
                {
                    mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha na integração de cancelamento do CIOT na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            return sucesso;
        }

        private bool IntegrarCadastroPix(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            mensagemErro = string.Empty;

            if (tipoPagamentoCIOT != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.PIX)
                return true;

            if (modalidade.TipoFavorecidoCIOT == null)
            {
                mensagemErro = $"Falha na integração do cadastro PIX (Tipo do Favorecido não selecionado)";
                return false;
            }

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CadastroPix cadastroPixIntegrar = PreencherCadastroPix(configuracao.CNPJAplicacao, configuracao.Token, configuracao.NomeUsuario, configuracao.DocumentoUsuario, modalidade, proprietario, motorista); ;

            string url = configuracao.URLAPI + "Pix/CadastrarChaveProprietario";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(cadastroPixIntegrar, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                {
                    mensagemErro = "Cadastro PIX integrado com sucesso.";
                    sucesso = true;
                }
                else
                {
                    mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha na integração do cadastro PIX na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            return sucesso;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CadastroPix PreencherCadastroPix(string CNPJAplicacao, string token, string nomeUsuarioAudit, string documentoUsuarioAudit, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Usuario motorista)
        {

            (string cpfCNPJProprietario, string chavePix, int tipoChavePix) = modalidade.TipoFavorecidoCIOT switch
            { 
                TipoFavorecidoCIOT.Transportador => ObterDadosTransportador(proprietario), 
                TipoFavorecidoCIOT.Motorista => ObterDadosMotorista(motorista),
                _ => throw new NotImplementedException("Tipo de favorecido não suportado!")
            };

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CadastroPix cadastroPix = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CadastroPix();
            cadastroPix.CNPJAplicacao = CNPJAplicacao ?? string.Empty;
            cadastroPix.Token = token ?? string.Empty;
            cadastroPix.CPFCNPJProprietario = cpfCNPJProprietario;
            cadastroPix.Chave = chavePix;
            cadastroPix.Tipo = tipoChavePix;
            cadastroPix.DocumentoUsuarioAudit = documentoUsuarioAudit ?? string.Empty;
            cadastroPix.NomeUsuarioAudit = nomeUsuarioAudit ?? string.Empty;

            return cadastroPix;
        }

        private bool BaixarViagem(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            if (string.IsNullOrWhiteSpace(ciot.ProtocoloAutorizacao.ObterSomenteNumeros()))
            {
                mensagemErro += "Não foi possível identificar o número da viagem.";
                return false;
            }

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.BaixarViagem baixarViagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.BaixarViagem();
            baixarViagem.CNPJAplicacao = configuracao.CNPJAplicacao;
            baixarViagem.Token = configuracao.Token;
            baixarViagem.DataEvento = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            baixarViagem.CodigoViagem = !string.IsNullOrWhiteSpace(ciot.ProtocoloAutorizacao.ObterSomenteNumeros()) ? ciot.ProtocoloAutorizacao.ObterSomenteNumeros().ToInt() : 0;

            string url = configuracao.URLAPI + "Viagem/BaixarViagem";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(baixarViagem, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            bool sucesso = false;
            string jsonResult = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                {
                    mensagemErro = "Baixa do CIOT realizada com sucesso.";
                    sucesso = true;
                }
                else
                {
                    mensagemErro = !string.IsNullOrWhiteSpace((string)retornoIntegracao.Mensagem) ? (string)retornoIntegracao.Mensagem : "Falha ao baixar CIOT na Extratta.";
                    sucesso = false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                sucesso = false;
            }

            GravarArquivoIntegracao(ciot, jsonPost, jsonResult, mensagemErro, unitOfWork);

            return sucesso;
        }

        private bool ConsultarTipoCavalo(Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ConsultaTipoCavalo consultaTipoCavalo = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ConsultaTipoCavalo();
            consultaTipoCavalo.CNPJAplicacao = configuracao.CNPJAplicacao;
            consultaTipoCavalo.Token = configuracao.Token;
            consultaTipoCavalo.DataBase = DateTime.Now.ToString("yyyy-MM-dd");
            consultaTipoCavalo.CNPJEmpresa = configuracao.CNPJAplicacao;

            string url = configuracao.URLAPI + "Veiculo/ConsultarTipoCavalo";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(consultaTipoCavalo, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoTipoCavalo retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoTipoCavalo>(result.Content.ReadAsStringAsync().Result);

                string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                    return true;
                else
                {
                    mensagemErro = "Falha na consulta de tipo de cavalo da Extratta.";
                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                return false;
            }
        }

        private bool ConsultarTipoCarreta(Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ConsultaTipoCarreta consultaTipoCarreta = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ConsultaTipoCarreta();
            consultaTipoCarreta.CNPJAplicacao = configuracao.CNPJAplicacao;
            consultaTipoCarreta.Token = configuracao.Token;
            consultaTipoCarreta.DataBase = DateTime.Now.ToString("yyyy-MM-dd");
            consultaTipoCarreta.CNPJEmpresa = configuracao.CNPJAplicacao;

            string url = configuracao.URLAPI + "Veiculo/ConsultarTipoCarreta";

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(Extratta));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("CNPJAplicacao", configuracao.CNPJAplicacao);
            client.DefaultRequestHeaders.Add("Token", configuracao.Token);
            client.DefaultRequestHeaders.Add("CNPJEmpresa", configuracao.CNPJAplicacao);

            string jsonPost = JsonConvert.SerializeObject(consultaTipoCarreta, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            Servicos.Log.TratarErro(jsonPost, "Extratta");

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoTipoCarreta retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoTipoCarreta>(result.Content.ReadAsStringAsync().Result);

                string jsonResult = JsonConvert.SerializeObject(retornoIntegracao, Formatting.Indented);
                Servicos.Log.TratarErro(jsonResult, "Extratta");

                if ((bool)retornoIntegracao.Sucesso)
                    return true;
                else
                {
                    mensagemErro = "Falha na consulta de tipo de carreta da Extratta.";
                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro(result.Content.ReadAsStringAsync().Result, "Extratta");

                mensagemErro = result.Content.ReadAsStringAsync().Result;
                return false;
            }
        }

        private int ObterTipoContrato(Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade)
        {
            return modalidade?.TipoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado ? 3 : 4; //Tipo do contrato: 1 = Frota; 2 = Cooperado; 3 = Agregado; 4 = Terceiro;
        }

        private Dominio.Entidades.Embarcador.CIOT.CIOTExtratta ObterConfiguracaoExtratta(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CIOT.CIOTExtratta repCIOTExtratta = new Repositorio.Embarcador.CIOT.CIOTExtratta(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao = repCIOTExtratta.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

            return configuracao;
        }

        private byte[] ObterFotoMotorista(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            if (!string.IsNullOrWhiteSpace(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" })))
                return new byte[0];

            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" }), $"{motorista.Codigo}.*").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return new byte[0];

            return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
        }

        private void GravarArquivoIntegracao(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, string jsonRequest, string jsonResponse, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repositorioCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repositorioCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            if (!string.IsNullOrWhiteSpace(jsonRequest))
                ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);

            if (!string.IsNullOrWhiteSpace(jsonResponse))
                ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            ciotIntegracaoArquivo.Mensagem = mensagem;

            repositorioCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            if (ciot.Codigo > 0)
                repositorioCIOT.Atualizar(ciot);
            else
                repositorioCIOT.Inserir(ciot);
        }

        private int ObterFormaPagamento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoContaBanco)
        {
            switch (tipoContaBanco)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente: return 2; // Conta Corrente
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Poupança: return 3; // Conta Poupança
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Salario: return 4; // Conta Pagamento
                default: return 5; // Outros
            }
        }

        private void SalvarPagamentoSemValor(ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            pagamentoEnvio.ArquivoEnvio = "";
            pagamentoEnvio.Data = DateTime.Now;
            pagamentoEnvio.TipoIntegracaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista.Extratta;
            pagamentoEnvio.Retorno = "Não enviado a operadora, saldo compensado";
            pagamentoEnvio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            pagamento.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ProprietarioIntegrar ObterContratado(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.Entidades.Empresa contratante, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ProprietarioIntegrar proprietarioIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ProprietarioIntegrar();
            Dominio.Entidades.Localidade localidade = repositorioLocalidade.BuscarPorCodigo(proprietario.Localidade?.Codigo ?? 0);

            if (modalidade == null)
                modalidade = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(proprietario, unitOfWork);

            proprietarioIntegrar.CNPJAplicacao = configuracao.CNPJAplicacao;
            proprietarioIntegrar.Token = configuracao.Token;
            proprietarioIntegrar.CNPJEmpresa = configuracao.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa ? configuracao.CNPJAplicacao : contratante.CNPJ_SemFormato;
            proprietarioIntegrar.CnpjCpf = proprietario.CPF_CNPJ_SemFormato;
            proprietarioIntegrar.RazaoSocial = proprietario.Nome;
            proprietarioIntegrar.NomeFantasia = !string.IsNullOrWhiteSpace(proprietario.NomeFantasia) ? proprietario.NomeFantasia : proprietario.Nome;
            proprietarioIntegrar.RG = proprietario.Tipo == "F" ? proprietario.IE_RG : "";
            proprietarioIntegrar.RGOrgaoExpedidor = proprietario.Tipo == "F" ? Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRGHelper.ObterDescricao(proprietario.OrgaoEmissorRG ?? Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG.Nenhum) : "";
            proprietarioIntegrar.RNTRC = !string.IsNullOrWhiteSpace(modalidade?.RNTRC) ? modalidade.RNTRC.ToInt() : 0;
            proprietarioIntegrar.IE = proprietario.Tipo == "J" ? proprietario.IE_RG.ToInt() : 0;
            proprietarioIntegrar.TipoContrato = ObterTipoContrato(modalidade);
            proprietarioIntegrar.DataNascimento = proprietario.DataNascimento.HasValue ? proprietario.DataNascimento.Value.ToString("yyyy-MM-dd") : string.Empty;

            if (configuracao.UtilizarTipoGeracaoCIOTPreenchimentoHabilitarContratoCiotAgregado)
            {
                proprietarioIntegrar.HabilitarContratoCiotAgregado = ciot.CIOTPorPeriodo;
            }
            else
            {
                proprietarioIntegrar.HabilitarContratoCiotAgregado = modalidade?.TipoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado ? true : false;
            }

            proprietarioIntegrar.Contatos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.Contato>();
            if (!string.IsNullOrWhiteSpace(proprietario.Email) || !string.IsNullOrWhiteSpace(proprietario.Telefone1) || !string.IsNullOrWhiteSpace(proprietario.Celular))
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.Contato contato = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.Contato();
                contato.Email = proprietario.Email ?? string.Empty;
                contato.Telefone = proprietario.Telefone1 ?? string.Empty;
                contato.Celular = !string.IsNullOrWhiteSpace(proprietario.Celular) ? proprietario.Celular.ToInt() : 0;

                proprietarioIntegrar.Contatos.Add(contato);
            }

            if (!string.IsNullOrWhiteSpace(proprietario.Telefone2))
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.Contato contato2 = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.Contato();
                contato2.Telefone = proprietario.Telefone2;

                proprietarioIntegrar.Contatos.Add(contato2);
            }

            proprietarioIntegrar.Enderecos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.Enderecos>();

            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.Enderecos endereco = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.Enderecos();
            endereco.CEP = proprietario.CEP;
            endereco.Endereco = proprietario.Endereco;
            endereco.Complemento = proprietario.Complemento;
            endereco.Numero = !string.IsNullOrWhiteSpace(proprietario.Numero) ? proprietario.Numero.ToInt() : 0;
            endereco.Bairro = proprietario.Bairro;
            endereco.IBGECidade = localidade?.CodigoIBGE ?? 0;
            endereco.IBGEEstado = localidade?.Estado?.CodigoIBGE ?? 0;
            endereco.BACENPais = localidade?.Pais?.Codigo ?? 0;
            endereco.NomeMae = string.Empty;
            endereco.NomePai = string.Empty;

            proprietarioIntegrar.Enderecos.Add(endereco);

            return proprietarioIntegrar;
        }

        private Dominio.Entidades.Auditoria.HistoricoObjeto ObterInformacoesAuditoria(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Auditoria.HistoricoObjeto repHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);

            List<Dominio.Entidades.Auditoria.HistoricoObjeto> historicoObjetoCIOT = repHistoricoObjeto.Consultar(0, cargaCIOT.CIOT.Codigo, "CIOT", null, null, 0, 0);
            List<Dominio.Entidades.Auditoria.HistoricoObjeto> historicoObjetoCarga = repHistoricoObjeto.Consultar(0, cargaCIOT.Carga.Codigo, "Carga", null, null, 0, 0);

            if (historicoObjetoCIOT.Any())
                return historicoObjetoCIOT.LastOrDefault();
            else
                return historicoObjetoCarga.LastOrDefault(historicoCarga => historicoCarga.DescricaoAcao.Contains("Autorizou Emissão dos documentos"));
        }

        private (string cpfCNPJProprietario, string chavePix, int tipoChavePix) ObterDadosTransportador(Dominio.Entidades.Cliente transportador)
        {
            string cpfCNPJProprietario = transportador.CPF_CNPJ.ToString() ?? "O CPF/CNPJ não foi encontrado!";
            string chavePix = transportador.ChavePix ?? "A chave pix não foi encontrada!";
            int tipoChavePix = (transportador.TipoChavePix == TipoChavePix.CPFCNPJ && transportador.ChavePix != null) ? RetornaTipoChavePixCpfOuCNPJ(transportador.ChavePix) : transportador.TipoChavePix != TipoChavePix.CPFCNPJ ? (int)transportador.TipoChavePix : 0;

            return (cpfCNPJProprietario, chavePix, tipoChavePix);
        }

        private (string cpfCNPJProprietario, string chavePix, int tipoChavePix) ObterDadosMotorista(Dominio.Entidades.Usuario motorista)
        {
            Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario dadosBancarios = motorista.DadosBancarios?.FirstOrDefault(dadosBancarios => !string.IsNullOrEmpty(dadosBancarios.ChavePix));
            if (dadosBancarios == null)
                return ("O CPF/CNPJ não foi encontrado!", "A chave pix não foi encontrada!", 0);

            string cpfCNPJProprietario = motorista.CPF ?? "O CPF/CNPJ não foi encontrado!";
            string chavePix = dadosBancarios.ChavePix ?? "A chave pix não foi encontrada!";
            int tipoChavePix = (dadosBancarios.TipoChavePix == TipoChavePix.CPFCNPJ && dadosBancarios.ChavePix != null) ? RetornaTipoChavePixCpfOuCNPJ(dadosBancarios.ChavePix) : dadosBancarios.TipoChavePix != TipoChavePix.CPFCNPJ ? (int)dadosBancarios.TipoChavePix : 0;

            return (cpfCNPJProprietario, chavePix, tipoChavePix);
        }

        public static int RetornaTipoChavePixCpfOuCNPJ(string cpfCnpj)
        {
            string cpfCnpjSemPontuacao = cpfCnpj.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);

            if (cpfCnpjSemPontuacao.Length > 11)
                return 1;
            else
                return 0;
        }

        #endregion
    }
}
