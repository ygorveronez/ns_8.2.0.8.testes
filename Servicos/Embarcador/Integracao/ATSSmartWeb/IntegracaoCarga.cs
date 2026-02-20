using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos;
using Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.ATSSmartWeb
{
    public partial class IntegracaoATSSmartWeb
    {
        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoATSSmartWeb?.URL))
                    throw new ServicoException("Não há URL configurada para a integração");

                if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoATSSmartWeb?.CNPJCompany) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoATSSmartWeb?.NomeCompany) || _configuracaoIntegracaoATSSmartWeb?.Localidade == null)
                    throw new ServicoException("Configuração da integração incompleta");

                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                if (this.controleNaColeta(cargaIntegracao.Carga))
                {
                    if (!this.IntegrarPontoControle(ref cargaIntegracao))
                        return;

                    if (!this.IntegrarPontoParada(ref cargaIntegracao))
                        return;

                    if (!this.IntegrarAtualizarDocumentos(ref cargaIntegracao))
                        return;

                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Integração concluída com sucesso";
                }
                else
                {
                    #region Validações

                    Dominio.Entidades.Veiculo veiculo = cargaIntegracao.Carga.Veiculo;

                    if (veiculo == null)
                        veiculo = cargaIntegracao.Carga.VeiculosVinculados.FirstOrDefault();

                    if (veiculo == null)
                        throw new ServicoException(@"Veículo não definido na carga");

                    Dominio.Entidades.Usuario motorista = cargaIntegracao.Carga.Motoristas != null && cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.FirstOrDefault() : null;

                    if (motorista == null)
                        motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                    if (motorista == null)
                        throw new ServicoException(@"Motorista não definido na carga");

                    #endregion

                    object request = this.obterViagem(cargaIntegracao.Carga, veiculo, motorista);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("GestaoSolicitacaoMonitoramentoIntegracao/Integrar", request);

                    if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    {
                        if (!this.IntegrarPontoParada(ref cargaIntegracao))
                            return;

                        if (!this.IntegrarAtualizarDocumentos(ref cargaIntegracao))
                            return;

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaIntegracao.ProblemaIntegracao = "Integração concluída com sucesso";
                    }
                    else
                    {
                        String message = retWS.ProblemaIntegracao;
                        if (message.Length > 300)
                        {
                            message = message.Substring(0, 300);
                        }
                        cargaIntegracao.ProblemaIntegracao = message;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    jsonRequisicao = retWS.jsonRequisicao;
                    jsonRetorno = retWS.jsonRetorno;
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaIntegracao.ProblemaIntegracao = message;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da viagem com a ATS Smart Web";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração da Viagem");

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoATSSmartWeb?.URL))
                    throw new ServicoException("Não há URL configurada para a integração");

                if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoATSSmartWeb?.CNPJCompany) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoATSSmartWeb?.NomeCompany) || _configuracaoIntegracaoATSSmartWeb?.Localidade == null)
                    throw new ServicoException("Configuração da integração incompleta");

                cargaDadosTransporteIntegracao.NumeroTentativas += 1;
                cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

                #region Validações

                Dominio.Entidades.Veiculo veiculo = cargaDadosTransporteIntegracao.Carga.Veiculo;

                if (veiculo == null)
                    veiculo = cargaDadosTransporteIntegracao.Carga.VeiculosVinculados.FirstOrDefault();

                if (veiculo == null)
                    throw new ServicoException(@"Veículo não definido na carga");

                Dominio.Entidades.Usuario motorista = cargaDadosTransporteIntegracao.Carga.Motoristas != null && cargaDadosTransporteIntegracao.Carga.Motoristas.Count > 0 ? cargaDadosTransporteIntegracao.Carga.Motoristas.FirstOrDefault() : null;

                if (motorista == null)
                    motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                if (motorista == null)
                    throw new ServicoException(@"Motorista não definido na carga");

                #endregion

                object request = this.obterViagem(cargaDadosTransporteIntegracao.Carga, veiculo, motorista, false, true);

                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("GestaoSolicitacaoMonitoramentoIntegracao/Integrar", request);

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    if (!this.IntegrarPontoParada(ref cargaDadosTransporteIntegracao, true))
                        return;

                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração concluída com sucesso";
                }
                else
                {
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaDadosTransporteIntegracao.ProblemaIntegracao = message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da viagem com a ATS Smart Web";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração da Viagem");

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        private bool controleNaColeta(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, TipoIntegracao.ATSSmartWeb);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            return (configuracao.QuandoIniciarMonitoramento == QuandoIniciarMonitoramento.AoInformarVeiculoNaCarga) && (cargaDadosTransporteIntegracao != null && cargaDadosTransporteIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado);
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envViagem obterViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Usuario motorista, bool agendamento = false, bool controleColeta = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envViagem retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envViagem();

            Dominio.Entidades.Veiculo primeiraCarreta = null;
            Dominio.Entidades.Veiculo segundaCarreta = null;

            primeiraCarreta = veiculo.VeiculosVinculados?.ElementAtOrDefault(0);
            segundaCarreta = veiculo.VeiculosVinculados?.ElementAtOrDefault(1);

            retorno.Cliente = this.obterCliente();
            retorno.Condutor = this.obterCondutorViagem(carga, motorista);
            retorno.VeiculoTracao = this.obterVeiculo(veiculo, carga);
            retorno.PrimeiraCarreta = this.obterVeiculo(primeiraCarreta, carga);
            retorno.SegundaCarreta = this.obterVeiculo(segundaCarreta, carga);
            retorno.PontosControle = this.obterPontosControle(carga, controleColeta);

            retorno.CodigoExterno = carga.Codigo.ToString();
            retorno.Tipo = agendamento ? TipoGestaoSolicitacaoMonitoramentoIntegracao.Agendamento : TipoGestaoSolicitacaoMonitoramentoIntegracao.Viagem;
            retorno.DataHoraPrevisaoInicioViagem = carga.DataInicioViagem ?? carga.DataInicioViagemReprogramada ?? carga.DataInicioViagemPrevista ?? carga.DataCarregamentoCarga ?? carga.DataCriacaoCarga;

            DateTime? dataUltimaEntrega = retorno.PontosControle.Max(o => o.DataHoraPrevisaoFim);
            DateTime? dataFimViagem = carga.DataFimViagemPrevista ?? dataUltimaEntrega;

            if (dataFimViagem < dataUltimaEntrega) 
                dataFimViagem = dataUltimaEntrega;

            retorno.DataHoraPrevisaoFimViagem = dataFimViagem;

            retorno.ValorTotalCarga = carga.DadosSumarizados?.ValorTotalProdutos ?? 0;
            retorno.VinculoCondutor = motorista.TipoMotorista == TipoMotorista.Proprio ? 1 : 3;
            retorno.VinculoVeiculoTracao = veiculo.Tipo == "P" ? 1 : 3;
            retorno.CodigoExternoOperacao = carga.TipoOperacao?.Descricao.ToString() ?? "";
            retorno.CodigoRota = null;
            return retorno;

        }
        public Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPessoaViagem obterCliente()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPessoaViagem retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPessoaViagem();

            retorno.Nome = _configuracaoIntegracaoATSSmartWeb?.NomeCompany ?? "";
            retorno.CPF_CNPJ = _configuracaoIntegracaoATSSmartWeb?.CNPJCompany ?? "";
            retorno.CodigoExterno = _configuracaoIntegracaoATSSmartWeb?.CNPJCompany.ObterSomenteNumeros().ToString() ?? "";
            retorno.Condutor = false;
            retorno.Cidade = _configuracaoIntegracaoATSSmartWeb?.Localidade?.Descricao ?? "";
            retorno.UF = obterCodigoDeUF(_configuracaoIntegracaoATSSmartWeb?.Localidade?.Estado?.Sigla ?? "");

            return retorno;
        }
        public Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envMotoristaViagem obterCondutorViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envMotoristaViagem retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envMotoristaViagem();

            retorno.Nome = motorista.Nome;
            retorno.CPF_CNPJ = motorista.CPF;
            retorno.Condutor = true;
            retorno.CodigoExterno = motorista.CPF.ToString();
            retorno.Cidade = motorista.Localidade?.Descricao ?? string.Empty;
            retorno.UF = obterCodigoDeUF(motorista.Localidade?.Estado?.Sigla ?? "");
            retorno.Complemento = this.obterComplemento(motorista);
            retorno.FisicaComplemento = this.obterFisicaComplemento(motorista);
            retorno.Endereco = this.obterEndereco(motorista);

            if (motorista.TipoMotorista == TipoMotorista.Terceiro)
                retorno.Empresa = this.obterEmpresa(motorista);
            else
                retorno.Empresa = this.obterEmpresa(motorista.Empresa ?? carga.Empresa);

            return retorno;
        }
        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem> obterPontosControle(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool controleColeta = false)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioTMS.BuscarPrimeiroRegistro();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem> dadosPontosControle = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repositorioCargaEntrega.BuscarPorCarga(carga.Codigo);

            if (controleColeta)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault();
                DateTime DataHoraPrevisaoFim = cargaEntregas.FirstOrDefault()?.DataPrevista.Value ?? DateTime.Now;

                if (cargaPedido != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem();

                    Dominio.Entidades.Cliente cliente = cargaPedido.Pedido?.Expedidor ?? cargaPedido.Pedido?.Remetente;
                    ponto.PontoControle = this.obterPontoControle(cliente);

                    int minutosPadrao = configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao;

                    ponto.DataHoraPrevisaoInicio = (carga.DataInicioViagem ?? carga.DataInicioViagemReprogramada ?? carga.DataInicioViagemPrevista ?? cargaPedido.Pedido.DataCarregamentoPedido);
                    ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio?.AddMinutes(minutosPadrao);
                    ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem;

                    dadosPontosControle.Add(ponto);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in carga.Pedidos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem();

                    Dominio.Entidades.Cliente cliente = pedido.Pedido?.Recebedor ?? pedido.Pedido?.Destinatario;
                    ponto.PontoControle = this.obterPontoControle(cliente);

                    int minutosPadrao = configuracaoTMS.TempoPadraoDeEntregaParaCalcularPrevisao;

                    ponto.DataHoraPrevisaoInicio = pedido.Pedido.PrevisaoEntrega;
                    ponto.DataHoraPrevisaoFim = pedido.Pedido.PrevisaoEntrega?.AddMinutes(minutosPadrao);

                    //Se for a primeira Coleta, manda como Origem.
                    if (dadosPontosControle.Count == 0)
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem;

                    //Se for a ultima entrega, manda como Destino.
                    else if (pedido == carga.Pedidos.ToList()[carga.Pedidos.Count() - 1])
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Destino;

                    //Entrega.
                    else 
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Entrega;

                    dadosPontosControle.Add(ponto);
                }
            }
            else
            {
                if (cargaEntregas.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault();
                    DateTime DataHoraPrevisaoFim = cargaEntregas.FirstOrDefault()?.DataPrevista.Value ?? DateTime.Now;

                    if (!cargaEntregas.Exists(ce => ce.Coleta))
                    {
                        if (cargaPedido == null)
                            throw new ServicoException("Carga sem Coleta e sem Pedido para integrar ponto de controle.");

                        Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem();

                        Dominio.Entidades.Cliente cliente = cargaPedido.Pedido?.Expedidor ?? cargaPedido.Pedido?.Remetente;
                        ponto.PontoControle = this.obterPontoControle(cliente);
                        int minutosPadrao = configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao;

                        ponto.DataHoraPrevisaoInicio = (carga.DataInicioViagem ?? carga.DataInicioViagemReprogramada ?? carga.DataInicioViagemPrevista ?? cargaPedido.Pedido.DataCarregamentoPedido);
                        ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio?.AddMinutes(minutosPadrao);
                        ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem;

                        dadosPontosControle.Add(ponto);
                    }


                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControleViagem();

                        Dominio.Entidades.Cliente cliente = cargaEntrega.Cliente;

                        ponto.PontoControle = this.obterPontoControle(cliente);

                        int minutosPadrao = 60;
                        if (cargaEntrega.Coleta && configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao > 0)
                            minutosPadrao = configuracaoTMS.TempoPadraoDeColetaParaCalcularPrevisao;
                        else if (!cargaEntrega.Coleta && configuracaoTMS.TempoPadraoDeEntregaParaCalcularPrevisao > 0)
                            minutosPadrao = configuracaoTMS.TempoPadraoDeEntregaParaCalcularPrevisao;

                        ponto.DataHoraPrevisaoInicio = cargaEntrega.DataPrevista;
                        ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio?.AddMinutes(minutosPadrao);

                        while (ponto.DataHoraPrevisaoInicio <= DataHoraPrevisaoFim)
                        {
                            ponto.DataHoraPrevisaoInicio = ponto.DataHoraPrevisaoInicio?.AddMinutes(minutosPadrao);
                            ponto.DataHoraPrevisaoFim = ponto.DataHoraPrevisaoInicio?.AddMinutes(minutosPadrao);
                        }
                        //Se for a primeira Coleta, manda como Origem.
                        if (dadosPontosControle.Count == 0)
                            ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Origem;

                        //Se for a ultima entrega, manda como Destino.
                        else if (cargaEntrega == cargaEntregas[cargaEntregas.Count - 1])
                            ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Destino;

                        //Coleta.
                        else if (cargaEntrega.Coleta)
                            ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Coleta;

                        //Entrega.
                        else if (!cargaEntrega.Coleta)
                            ponto.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoSolicitacaoMonitoramentoIntegracaoPontoControle.Entrega;

                        dadosPontosControle.Add(ponto);
                    }
                }
            }
            return dadosPontosControle;
        }
        public Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle obterPontoControle(Dominio.Entidades.Cliente pessoa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle();
            retorno.Pessoa = this.obterPessoa(pessoa);
            retorno.Nome = retorno.Pessoa.Nome;
            retorno.Latitude = pessoa.Localidade?.Latitude ?? 0;
            retorno.Longitude = pessoa.Localidade?.Longitude ?? 0;

            return retorno;
        }
    }
}
