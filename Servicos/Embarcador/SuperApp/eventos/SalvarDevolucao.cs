using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.SuperApp.Eventos
{
    public class SalvarDevolucao : IntegracaoSuperApp
    {
        private string _cpfMotorista;

        public SalvarDevolucao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, unitOfWorkAdmin, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkAdmin = unitOfWorkAdmin;
            _clienteMultisoftware = clienteMultisoftware;
        }

        public void ProcessarEvento(Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracaoSuperApp, bool devolucaoParcial, out RetornoIntegracaoSuperApp retornoIntegracaoSuperApp)
        {
            retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();
            try
            {
                Servicos.Log.TratarErro("Inicio Salvar Devolução", "IntegracaoSuperAPPOutrosTipos");

                string jsonRequisicao = integracaoSuperApp.ArquivoRequisicao != null ? obterJsonRequisicao(integracaoSuperApp.ArquivoRequisicao) : integracaoSuperApp.StringJsonRequest;

                if (string.IsNullOrEmpty(jsonRequisicao))
                    throw new ServicoException($"Arquivo de integração/Request não encontrado.");

                EventoSalvarDevolucao eventoSalvarDevolucao = Newtonsoft.Json.JsonConvert.DeserializeObject<EventoSalvarDevolucao>(jsonRequisicao);
                if (eventoSalvarDevolucao == null)
                    throw new ServicoException("Falha na conversão da requisição para objeto.");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repositorioMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(_unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado repositorioCargaEntregaProdutoChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado(_unitOfWork);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoTMS();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();

                ObterDadosChecklistFlow(eventoSalvarDevolucao.Data.Response);

                _cpfMotorista = eventoSalvarDevolucao.Data.Driver.Document.Value;
                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaPorCPF(_cpfMotorista);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = ObterAuditado(motorista);

                string IDTrizy = eventoSalvarDevolucao.Data.Travel._id; //Carga.
                int codigoCarga = eventoSalvarDevolucao.Data.Travel.ExternalID.ToInt(); //Codigo Carga.

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorIDIdentificacaoTrizy(IDTrizy) ?? repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ServicoException($"Carga não encontrada. ID: {IDTrizy}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);
                else if (carga.SituacaoCarga == SituacaoCarga.Cancelada)
                    throw new ServicoException($"Carga Cancelada. ID: {IDTrizy}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);

                string IDTrizyStoppingPointDocument = eventoSalvarDevolucao.Data.StoppingPointDocument?._id ?? string.Empty; //Canhoto.
                int codigoCanhoto = eventoSalvarDevolucao.Data.StoppingPointDocument?.ExternalId.ToInt() ?? 0; //Codigo Canhoto.

                if (string.IsNullOrEmpty(IDTrizyStoppingPointDocument) && codigoCanhoto <= 0)
                {
                    throw new ServicoException($"Faltou os dados do canhoto: {IDTrizyStoppingPointDocument}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);
                }

                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorIdTrizy(IDTrizyStoppingPointDocument) ?? repCanhoto.BuscarPorCodigo(codigoCanhoto) ?? throw new ServicoException($"Não foi localizado um canhoto compatível com a entrega. ID: {IDTrizyStoppingPointDocument}");

                string IDTrizyStoppingPoint = eventoSalvarDevolucao.Data.StoppingPoint?._id ?? string.Empty;
                string[] codigoExterno = eventoSalvarDevolucao.Data.StoppingPoint?.ExternalId?.Split(';');
                int codigoCargaEntrega = codigoExterno != null && codigoExterno.Length > 0 ? codigoExterno[0].ToInt() : 0;
                double identificacaoCliente = codigoExterno != null && codigoExterno.Length > 1 ? codigoExterno[1].ToDouble() : 0;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorIdTrizy(IDTrizyStoppingPoint) ?? repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega) ?? repositorioCargaEntrega.BuscarPorCargaECliente(carga.Codigo, identificacaoCliente) ?? throw new ServicoException($"Não foi localizada Coleta/Entrega. ID:{IDTrizy} | Código:{codigoCargaEntrega}");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = repositorioCargaEntregaNotaFiscal.BuscarPorCargaENumeroNotaFiscal(carga.Codigo, new List<int> { canhoto.XMLNotaFiscal.Numero });

                if (cargaEntregaNotaFiscal == null)
                {
                    throw new ServicoException($"Carga Entrega Nota Fiscal não encontrada: {cargaEntrega.Codigo} - {canhoto.XMLNotaFiscal.Numero}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);
                }

                Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivoDevolucaoEntrega = repositorioMotivoDevolucaoEntrega.BuscarPorCodigo(eventoSalvarDevolucao.Data.Reason.ExternalId.ToInt());

                if (motivoDevolucaoEntrega == null)
                {
                    throw new ServicoException($"Motivo de devolução de entrega não encontrado: {eventoSalvarDevolucao.Data.Reason.ExternalId.ToInt()}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado);
                }

                Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscalMobile = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal()
                {
                    Codigo = cargaEntregaNotaFiscal.Codigo,
                    DevolucaoParcial = devolucaoParcial,
                    DevolucaoTotal = !devolucaoParcial,
                    MotivoDevolucaoEntrega = motivoDevolucaoEntrega,
                    Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>(),
                    Numero = canhoto.XMLNotaFiscal.Numero
                };

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos = repositorioCargaEntregaProduto.BuscarPorCargaEntrega(cargaEntrega.Codigo);

                if (devolucaoParcial)
                {
                    foreach (StoppingPointDocumentItem stoppingPointItem in eventoSalvarDevolucao.Data.StoppingPointDocumentItems)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto produto = cargaEntregaProdutos.FirstOrDefault(cargaEntregaProduto => cargaEntregaProduto.Codigo.ToString() == stoppingPointItem.ExternalId);
                        if (produto != null)
                        {
                            decimal valorDevolucaoCalculado = 0;
                            decimal quantidadeDevolvido = (stoppingPointItem.ValueTotal?.value ?? 0) - (stoppingPointItem.ValueDelivered?.value ?? 0);
                            if (quantidadeDevolvido > 0 && configuracaoChamado.CalcularValorDasDevolucoes)
                                valorDevolucaoCalculado = quantidadeDevolvido * repositorioCargaEntregaPedido.ObterPrecoUnitarioPedidoProdutoPorCargaEntrega(produto.CargaEntrega.Codigo, produto.Produto.Codigo);
                            cargaEntregaNotaFiscalMobile.Produtos.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto()
                            {
                                Protocolo = produto.Codigo,
                                QuantidadeDevolucao = quantidadeDevolvido,
                                Lote = produto?.Lote ?? string.Empty,
                                DataCritica = produto.DataCritica,
                                ValorDevolucao = valorDevolucaoCalculado,
                            });
                        }
                    }
                }

                int ocorrenciaQuantidadeImagens = _dadosEvidencias.imagensEntrega.Count;
                string observacaoOcorrencia = _dadosEvidencias.observacoes.Count > 0 ? _dadosEvidencias.observacoes[0] : string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointRejeicaoColetaEntrega = null;
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = null;

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repositorioChamado.BuscarAbertosOuEmTratativaPorCargaEntrega(cargaEntrega.Codigo).Result;

                if (!chamados.Exists(chamado => chamado.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao && chamado.MotivoChamado.Codigo == (motivoDevolucaoEntrega.MotivoChamado?.Codigo ?? 0)))
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros
                    {
                        codigoCargaEntrega = cargaEntrega?.Codigo ?? 0,
                        codigoMotivo = motivoDevolucaoEntrega.TipoOcorrencia?.Codigo ?? 0,
                        data = DateTime.Now,
                        wayPoint = wayPointRejeicaoColetaEntrega,
                        usuario = null,
                        motivoRetificacao = 0,
                        tipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiMobile,
                        observacao = observacaoOcorrencia,
                        configuracao = configuracao,
                        devolucaoParcial = devolucaoParcial,
                        notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal>()
                        {
                            cargaEntregaNotaFiscalMobile
                        },
                        motivoFalhaGTA = 0,
                        apenasRegistrar = false,
                        dadosRecebedor = null,
                        permitirEntregarMaisTarde = false,
                        dataConfirmacaoChegada = null,
                        wayPointConfirmacaoChegada = null,
                        atendimentoRegistradoPeloMotorista = true,
                        OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.App,
                        dataInicioCarregamento = null,
                        quantidadeImagens = ocorrenciaQuantidadeImagens,
                        imagens = _dadosEvidencias.imagensEntrega,
                        clienteMultisoftware = _clienteMultisoftware,
                        valorChamado = 0
                    };

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(parametros, auditado, _unitOfWork, out chamado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiMobile);
                }
                else
                {
                    chamado = chamados[0];
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> cargaEntregaProdutoChamados = repositorioCargaEntregaProdutoChamado.BuscarPorChamado(chamado.Codigo);
                    dynamic notasNoAtendimento = servicoControleEntrega.ObterNotasFiscais(cargaEntrega, cargaEntregaProdutos, configuracao, configuracaoChamado, _unitOfWork, chamado, cargaEntregaProdutoChamados, false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiMobile);
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> notasFiscaisFormatadas = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal>();

                    foreach (var n in notasNoAtendimento)
                    {
                        dynamic dados = n.Dados;
                        dynamic produtos = n.Produtos;

                        Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivo = repositorioMotivoDevolucaoEntrega.BuscarPorCodigo(dados.MotivoDaDevolucao.Codigo);

                        Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal notaFiscalMobile = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal
                        {
                            Codigo = dados.Codigo,
                            DevolucaoParcial = dados.DevolucaoParcial,
                            DevolucaoTotal = !dados.DevolucaoParcial,
                            MotivoDevolucaoEntrega = motivo,
                            Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>(),
                            Numero = dados.Numero
                        };

                        foreach (var produto in produtos)
                        {
                            notaFiscalMobile.Produtos.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto
                            {
                                Protocolo = produto.Codigo,
                                QuantidadeDevolucao = Convert.ToDecimal(produto.QuantidadeDevolucao),
                                Lote = produto?.Lote ?? string.Empty,
                                DataCritica = string.IsNullOrEmpty(produto.DataCritica) ? null : DateTime.Parse(produto.DataCritica),
                                ValorDevolucao = Convert.ToDecimal(produto.ValorDevolucao)
                            });
                        }

                        notasFiscaisFormatadas.Add(notaFiscalMobile);
                    }

                    if (cargaEntrega.DevolucaoParcial == false && devolucaoParcial)
                    {
                        cargaEntrega.DevolucaoParcial = true;
                    }

                    notasFiscaisFormatadas.Add(cargaEntregaNotaFiscalMobile);

                    servicoControleEntrega.SalvarDevolucaoCargaEntrega(cargaEntrega, notasFiscaisFormatadas, null, motivoDevolucaoEntrega.MotivoChamado, chamado, configuracao, auditado, cargaEntrega.Situacao, devolucaoParcial, false, configuracaoChamado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiMobile, null, true);
                }

                if (chamado != null)
                {
                    string IdOcorrenciaTrizy = eventoSalvarDevolucao.Data._id;
                    if (!string.IsNullOrWhiteSpace(IdOcorrenciaTrizy))
                    {
                        chamado.IdOcorrenciaTrizy = IdOcorrenciaTrizy;
                        repositorioChamado.Atualizar(chamado);
                    }

                    Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, _unitOfWork);

                    AdicionarImagensChamado(chamado, _dadosEvidencias.imagensEntrega, _unitOfWork, auditado);

                    new Servicos.Embarcador.Chamado.Chamado(_unitOfWork).EnviarEmailChamadoAberto(chamado, _unitOfWork);
                }

                retornoIntegracaoSuperApp.Sucesso = true;
            }
            catch (ServicoException ex) when (ex.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroIgnorado)
            {
                retornoIntegracaoSuperApp.Sucesso = true;
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPOutrosTipos");
            }
            catch (ServicoException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPOutrosTipos");
            }
            catch (Exception ex)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar devolução";
                Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPOutrosTipos");
            }
        }
    }
}
