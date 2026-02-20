using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Métodos Globais

        #endregion

        #region Métodos Privados

        private bool IntegrarContratoFrete(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, int? roteiroID)
        {// função equivalente DeclararOperacaoTransporte

            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
            mensagem = null;
            string jsonRequisicao = "";
            string jsonRetorno = "";
            bool sucesso = false;

            try
            {
                this.ObterToken(out mensagem);
                if (string.IsNullOrWhiteSpace(token))
                    return false;

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFrete ContratoFrete = this.ObterObjEmitirContratoFrete(cargaCIOT, roteiroID);

                string url = $"{this.urlWebService}mso-cargo-frete/api/Contrato";
                HttpClient requisicao = CriarRequisicao(url);
                jsonRequisicao = JsonConvert.SerializeObject(ContratoFrete, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicao.IsSuccessStatusCode)
                {
                    mensagem = ($"Falha ao enviar contrato frete Ambipar: {retornoRequisicao.StatusCode}");
                    cargaCIOT.CIOT.Mensagem = mensagem;
                    cargaCIOT.CIOT.Situacao = SituacaoCIOT.Pendencia;
                }
                else
                {
                    sucesso = true;
                    var contrato = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                    cargaCIOT.CIOT.DataAbertura = DateTime.Now;
                    cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                    cargaCIOT.CIOT.Mensagem = "CIOT integrado com sucesso.";
                    cargaCIOT.CIOT.Numero = contrato.ID;
                    //cargaCIOT.CIOT.Digito = transformer.GetDigitoCiot();
                    //cargaCIOT.CIOT.ProtocoloAutorizacao = transformer.GetProtocoloAutorizacaoCiot();
                }
            }
            catch (ServicoException ex)
            {
                mensagem = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagem = "Ocorreu uma falha ao realizar a integração CIOT contrato de frete da Ambipar";
            }

            cargaCIOT.CIOT.Mensagem = mensagem;
            GravarArquivoIntegracao(cargaCIOT.CIOT, jsonRequisicao, jsonRetorno, "json");

            return sucesso;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFrete ObterObjEmitirContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, int? roteiroID)
        {
            if (cargaCIOT.Carga == null)
                throw new ServicoException("Carga não encontrado para o CIOT.");

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCIOT.Carga.Codigo);

            if (cargaPedido == null)
                throw new ServicoException("Carga não possui pedidos para integração CIOT contrato de frete da Ambipar.");

            Dominio.Entidades.Cliente remetente = ObterRemetentePedido(cargaPedido);
            Dominio.Entidades.Cliente destinatario = ObterDestinatarioPedido(cargaPedido);


            Dominio.Entidades.Usuario motorista = cargaCIOT.Carga.Motoristas.FirstOrDefault();
            Dominio.Entidades.Veiculo semiTrailer = cargaCIOT.Carga.Veiculo;
            Dominio.Entidades.Veiculo truck = cargaCIOT.Carga.Veiculo;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe = cargaCIOT.Carga.CargaCTes.FirstOrDefault()?.CTe;
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFrete contratoFrete = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFrete();
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(cargaCIOT.CIOT.Transportador, _unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransformerAmbipar transformer = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransformerAmbipar(cargaCIOT, remetente, destinatario, CTe, modalidadeTerceiro);

            cargaCIOT.CIOT.DataAbertura = DateTime.Now;
            cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;

            if (cargaCIOT.Carga == null)
                throw new ServicoException("Carga não encontrada para integração CIOT contrato de frete da Ambipar.");

            if (remetente == null)
                throw new ServicoException("Remetente não pode ser vazio para integração CIOT contrato de frete da Ambipar..");

            if (destinatario == null)
                throw new ServicoException("Destinatario não pode ser vazio para integração CIOT contrato de frete da Ambipar..");

            if (CTe == null)
                throw new ServicoException("CTE não pode ser vazio para integração CIOT contrato de frete da Ambipar.");

            if (motorista == null)
                throw new ServicoException("Motorista deve ser informado para integração CIOT contrato de frete da Ambipar.");
            if (truck == null)
                throw new ServicoException("Veiculo deve ser informado para integração CIOT contrato de frete da Ambipar.");

            if (cargaCIOT.Carga.Terceiro == null)
                throw new ServicoException("Terceiro não pode ser nullo para integração CIOT contrato de frete da Ambipar.");

            if (cargaCIOT.Carga.ModeloVeicularCarga == null)
                throw new ServicoException("Modelo Veicular da carga nao pode ser vazio integração CIOT contrato de frete da Ambipar.");

            if (modalidadeTerceiro == null)
                throw new ServicoException("A carga não posui modalidade terceiro cadastrada CIOT contrato de frete da Ambipar.");

            // Cria um objeto Contrato contendo as informações necessárias para emitir o contrato
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFrete contrato = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFrete()
            {
                TipoEmissaoID = transformer.TipoEmissaoID(),
                TipoMidiaValePedagio = transformer.TipoMidiaValePedagio(),
                EmbarcadorFilialID = transformer.EmbarcadorFilialID(),
                TransportadorID = transformer.TransportadorID(),
                MotoristaID = transformer.MotoristaID(),
                CartaoID = transformer.CartaoID(),
                CartaoIDTransportador = transformer.CartaoIDTransportador(),
                VeiculoID = transformer.VeiculoID(),
                CarretaID = transformer.carretaID(),
                RoteiroID = roteiroID ?? 0,
                TipoOperacaoID = transformer.TipoOperacaoID(),
                TipoMercadoriaID = transformer.TipoMercadoriaID(),
                Valor = transformer.Valor(),
                ValorAdiantamento = transformer.ValorAdiantamento(),
                ValorAdiantamentoTransportadora = transformer.ValorAdiantamentoTransportadora(),
                ValorCarga = transformer.ValorCarga(),
                PesoCarga = transformer.PesoCarga(),
                DataPrevisaoEntrega = transformer.DataPrevisaoEntrega(),
                RoteiroIdaVolta = transformer.RoteiroIdaVolta(),
                EixoSuspensoIda = transformer.EixoSuspensoIda(),
                EixoSuspensoVolta = transformer.EixoSuspensoVolta(),
                DataPrevistaSaida = transformer.DataPrevistaSaida(),
                DataPrevistaPrestacaoContas = transformer.DataPrevistaPrestacaoContas(),
                CodigoViagem = transformer.CodigoViagem(),
                DataPrestacaoContas = transformer.DataPrestacaoContas(),
                DataQuitacao = transformer.DataQuitacao(),
                DataAgendamento = transformer.DataAgendamento(),
                DataPrevistaPagamento = transformer.DataPrevistaPagamento(),
                DataPagamento = transformer.DataPagamento(),
                CpfCnpjDestinatario = transformer.CpfCnpjDestinatario(),
                IgnorarPreConfiguracaoRoteiro = transformer.IgnorarPreConfiguracaoRoteiro(),
                FreteContaMotorista = transformer.FreteContaMotorista(),
                TipoChavePix = transformer.TipoChavePix(),
                TipoChavePixTransportador = transformer.TipoChavePixTransportador(),
                ChavePix = transformer.ChavePix(),
                ChavePixTransportador = transformer.ChavePixTransportador(),
                EmitirValePedagio = transformer.EmitirValePedagio(),
                ValePedagioCartao = transformer.ValePedagioCartao(),
                TipoCreditoVPO = transformer.TipoCreditoVPO(),
                FreteFracionado = transformer.FreteFracionado(),
                PercentualTransportador = transformer.PercentualTransportador(),
                PercentualMotorista = transformer.PercentualMotorista(),
                AprovarContrato = transformer.AprovarContrato(),
                ContratoDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoDocumento>
                {
                    new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoDocumento
                    {
                        TipoDocumentoID =  transformer.TipoDocumentoID(),
                        TipoDocumento = transformer.TipoDocumento(),
                        Numero = transformer.Numero(),
                        Serie = transformer.Serie(),
                    }
                },
                EixoSuspensoParadasIda = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoEixoSuspensoParadaIda>
                {
                    new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoEixoSuspensoParadaIda
                    {
                        Ordem = transformer.Ordem(),// 1,
                        EixosSuspenso = transformer.EixosSuspenso(),// 0
                    }
                },
                EixoSuspensoParadasVolta = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoEixoSuspensoParadaVolta>
                {
                    new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoEixoSuspensoParadaVolta
                    {
                        Ordem = transformer.Ordem(),// 1,
                        EixosSuspenso = transformer.EixosSuspenso(),// 0
                    }
                },
                ContratoRegraQuitacao = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoRegraQuitacao
                {
                    Tolerancia = transformer.Tolerancia(),
                    PercentualTolerancia = transformer.PercentualTolerancia(),
                    LimiteSuperior = transformer.LimiteSuperior(),
                    QuebraTolerancia = transformer.QuebraTolerancia(),
                    TipoPeso = transformer.TipoPeso(),// "C",
                    TipoCobrancaQuebra = transformer.TipoCobrancaQuebra(),// false,
                    TipoCobrancaAvaria = transformer.TipoCobrancaAvaria(),// false
                }
            };
            return contrato;
        }

        private Dominio.Entidades.Cliente ObterRemetentePedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if ((cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Expedidor != null)
                return cargaPedido.Expedidor;
            else
                return cargaPedido.Pedido.Remetente;
        }

        private Dominio.Entidades.Cliente ObterDestinatarioPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if ((cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null)
                return cargaPedido.Recebedor;
            else
                return cargaPedido.Pedido.Destinatario;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.AdvancePayment ObterAdiantamento(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            return new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.AdvancePayment()
            {
                date = cargaCIOT.ContratoFrete?.DataEmissaoContrato.ToString() ?? "",
                paymentType = "",
                value = cargaCIOT.ContratoFrete?.ValorAdiantamento ?? 0
            };
        }

        private int ObterNumeroEixos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            bool eixosSuspensos = carga.TipoOperacao?.TipoCarregamento == RetornoCargaTipo.Vazio;

            int numeroEixos = 0;
            if (carga.Veiculo.ModeloVeicularCarga != null)
            {
                numeroEixos = carga.Veiculo.ModeloVeicularCarga.NumeroEixos ?? 0;
                if (eixosSuspensos)
                    numeroEixos -= carga.Veiculo.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
            }

            if (carga.VeiculosVinculados != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                {
                    if (reboque.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                    {
                        numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;

                        if (eixosSuspensos)
                            numeroEixos -= reboque.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                    }
                }
            }

            return numeroEixos;
        }

        private decimal ObterPesoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            return repPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaCIOT.Carga.Codigo);
        }

        private void ObterNCMFrete(out string ncm, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            ncm = "0000";
            if (!string.IsNullOrWhiteSpace(cargaCIOT.Carga?.TipoDeCarga?.NCM.ObterSomenteNumeros()) && cargaCIOT.Carga?.TipoDeCarga?.NCM.ObterSomenteNumeros().Length > 3)
                ncm = cargaCIOT.Carga?.TipoDeCarga?.NCM.ObterSomenteNumeros().Substring(0, 4) ?? "0000";
        }

        private void ObterDatasInicioTerminoFrete(out DateTime dataInicioFrete, out DateTime dataTerminoFrete, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.CIOT.CIOTBBC configuracao)
        {
            dataInicioFrete = cargaCIOT.CIOT.DataAbertura ?? DateTime.Now;
            dataTerminoFrete = cargaCIOT.CIOT.DataFinalViagem;

            if (configuracao.ConfiguracaoCIOT.UtilizarDataAtualComoInicioTerminoCIOT)
            {
                dataInicioFrete = DateTime.Now;
                dataTerminoFrete = dataInicioFrete.AddDays(configuracao.ConfiguracaoCIOT.DiasTerminoCIOT ?? 1);
            }
        }

        private int ObterTipoPagamento(Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            //FormaPagamento - Indica o tipo de pagamento do valor do frete.
            //1 - IPEF
            //2 - Conta corrente
            //3 - Conta poupança
            //4 - Conta de pagamento
            //5 - Outros

            return 2;
        }

        #endregion
    }
}