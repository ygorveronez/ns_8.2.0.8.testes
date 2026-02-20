using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.KMM
{
    partial class IntegracaoKMM
    {
        public void IntegrarPagamentoMotorista(int codigoPagamento, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            TipoIntegracaoPagamentoMotorista[] tipos = new TipoIntegracaoPagamentoMotorista[]
            {
                TipoIntegracaoPagamentoMotorista.PamCard,
                TipoIntegracaoPagamentoMotorista.PagBem,
                TipoIntegracaoPagamentoMotorista.PamCardCorporativo,
                TipoIntegracaoPagamentoMotorista.Email,
                TipoIntegracaoPagamentoMotorista.Target,
                TipoIntegracaoPagamentoMotorista.Extratta,
                TipoIntegracaoPagamentoMotorista.RepomFrete,
            };

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento = repPagamentoMotorista.BuscarPorCodigo(codigoPagamento);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamentoETipoIntegracao(codigoPagamento, TipoIntegracaoPagamentoMotorista.KMM);
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> listaIntegracao = repPagamentoMotoristaIntegracaoEnvio.BuscarIntegracoesPendenteDeEnvio(codigoPagamento, tipos);

            if (listaIntegracao.Where(x => x.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao).FirstOrDefault() != null)
            {
                pagamentoEnvio.Retorno = "Pendência de integração com a operadora.";
                pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);

                return;
            }
            else if (listaIntegracao.Count() > 0) 
            {
                // Aguardando integração com a operadora.
                return;
            }

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();
            pagamentoRetorno.Data = DateTime.Now;
            pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
            pagamentoRetorno.PagamentoMotoristaTMS = pagamento;


            try
            {
                pagamentoEnvio.Data = DateTime.Now;
                pagamentoEnvio.NumeroTentativas += 1;

                var objetoIntegracao = ObterPagamentoMotorista(pagamento);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "importarCT" },
                    { "parameters", objetoIntegracao }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                {
                    pagamentoEnvio.Retorno = retWS.ProblemaIntegracao;
                    pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pagamentoRetorno.DescricaoRetorno = retWS.ProblemaIntegracao;
                }
                else
                {
                    this.PrencherCodigoGetCodPagamentoMotorista(retWS, pagamento);

                    pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                }

                pagamentoRetorno.ArquivoRetorno = retWS.jsonRetorno;
                pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retWS.jsonRequisicao, "json", _unitOfWork);
                pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retWS.jsonRetorno, "json", _unitOfWork);

                if (pagamentoEnvio.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    this.IntegrarLiberacaoPagamentoMotorista(pagamento, ref pagamentoEnvio, ref pagamentoRetorno);
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                
                pagamentoEnvio.Retorno = message;
                pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoRetorno.DescricaoRetorno = message;
            }

            repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoEnvio);
            repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);
        }
        public void IntegrarLiberacaoPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista,ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno)
        {
            Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao repositorioPagamentoContrato = new Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            try
            {
                TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo?.TipoIntegracaoPagamentoMotorista ?? TipoIntegracaoPagamentoMotorista.SemIntegracao;

                TipoPagamentoMotorista tipoPagamento = pagamentoMotorista.PagamentoMotoristaTipo?.TipoPagamentoMotorista ?? TipoPagamentoMotorista.Adiantamento;

                Hashtable parameters = new Hashtable {
                    { "num_formulario", pagamentoMotorista.Numero.ToString()},
                    { "tipo_pagamento", this.ObterCodigoTipoPagamentoKMM(tipoIntegracaoPagamentoMotorista)},
                    { "tipo_saldo_adto", "1"},
                    { "data_emissao", pagamentoMotorista.Data.ToString("yyyy-MM-dd HH:mm:ss")},
                    { "cnpj_unidade_negocio", pagamentoMotorista.Carga.Empresa?.CNPJ},
                    { "valor", pagamentoMotorista.Valor},
                };

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "incluirLiberacao" },
                    { "parameters", parameters }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                pagamentoEnvio.SituacaoIntegracao = retWS.SituacaoIntegracao;
                pagamentoEnvio.Retorno = retWS.ProblemaIntegracao;

                pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retWS.jsonRequisicao, "json", _unitOfWork);
                pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retWS.jsonRetorno, "json", _unitOfWork);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                pagamentoEnvio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                String message = "Problema ao autorizar pagamento: " + excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                pagamentoEnvio.Retorno = message;
                pagamentoRetorno.DescricaoRetorno = message;
            }

        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContratoTransporte ObterPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            string cavalo = pagamento.Carga.Veiculo?.Placa.ToString();
            string carreta;
            if (pagamento.Carga.Veiculo?.TipoVeiculo == "1")
            {
                carreta = cavalo;
            }
            else if (pagamento.Carga.VeiculosVinculados != null && pagamento.Carga.VeiculosVinculados.Count > 0)
            {
                carreta = pagamento.Carga.VeiculosVinculados?.ElementAt(0)?.Placa.ToString();
            }
            else
            {
                carreta = "";
            }

            TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamento.PagamentoMotoristaTipo?.TipoIntegracaoPagamentoMotorista ?? TipoIntegracaoPagamentoMotorista.SemIntegracao;
            var cargaPedido = pagamento.Carga.Pedidos?.ElementAt(0);

            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = new Dominio.Entidades.Embarcador.Terceiros.ContratoFrete();

            if (pagamento.Carga.Codigo > 0)
                contratoFrete = repContratoFrete.BuscarPorCarga(pagamento.Carga.Codigo);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContratoTransporte()
            {
                CNPJFilial = pagamento.Carga.Empresa?.CNPJ,
                TipoPagamento = this.ObterCodigoTipoPagamentoKMM(tipoIntegracaoPagamentoMotorista),
                Serie = "P",
                NumFormulario = pagamento.Numero.ToString(),
                NumeroCRTSistemaExterno = pagamento.Codigo.ToString(),
                DataEmissao = pagamento.Data.ToString("yyyy-MM-dd HH:mm:ss"),
                RemetenteCnpjCpf = LPadCpfCnpj(cargaPedido.Pedido.Remetente?.Tipo.ToString(), cargaPedido.Pedido.Remetente?.CPF_CNPJ.ToString()),
                DestinatarioCnpjCpf = LPadCpfCnpj(cargaPedido.Pedido.Destinatario?.Tipo.ToString(), cargaPedido.Pedido.Destinatario?.CPF_CNPJ.ToString()),
                Peso = cargaPedido.Pedido?.PesoTotal.ToString(),
                Volume = cargaPedido.Pedido?.QtVolumes.ToString(),
                M3 = cargaPedido.Pedido?.CubagemTotal.ToString(),
                ValorUnitario = pagamento.Valor.ToString(),
                // no KMM o valor bruto está invertido com o líquido
                ValorFreteBruto = pagamento.Valor.ToString(),
                ValorFreteLiquido = pagamento.Valor.ToString(),
                MotoristaCPF = pagamento.Motorista.CPF,
                PlacaControle = cavalo,
                PlacaReferencia = carreta,
                ValorItemAdiantamento = "0",
                ValorPedagio = "0",
                CentroCustoGerencial = "1",
                Observacao = pagamento.Observacao,
                TotalAcrescimos = "0",
                TotalDescontos = "0",
                NumeroDocumento = "P" + pagamento.Numero.ToString(),
                //na kmm, cliente é o tomador
                NomeCliente = cargaPedido.Pedido?.Tomador?.Nome,
                CNPJCliente = LPadCpfCnpj(cargaPedido.Pedido.Tomador?.Tipo.ToString(), cargaPedido.Pedido.Tomador?.CPF_CNPJ.ToString()),
                CentroCustoOriginal = null,
                GerarCPG = 1,
                CodigoCargaEmbarcador = servicoCarga.ObterNumeroCarga(pagamento.Carga, configuracaoEmbarcador),
                VencimentoAdiantamento = null,
                VencimentoSaldo = pagamento.DataPagamento.ToString("yyyy-MM-dd HH:mm:ss"),
                CentroCusto = cargaPedido.Pedido?.CentroDeCustoViagem?.CodigoIntegracao,
                TipoPagamentoMotorista = pagamento?.PagamentoMotoristaTipo?.Descricao,
                BaseInss = pagamento.BaseCalculoINSS.ToString(),
                ValorInss = pagamento.ValorINSS.ToString(),
                BaseIrrf = pagamento.BaseCalculoIRRF.ToString(),
                ValorIrrf = pagamento.ValorIRRF.ToString(),
                BaseSenat = pagamento.BaseCalculoSENAT.ToString(),
                ValorSenat = pagamento.ValorSENAT.ToString(),
                BaseSest = pagamento.BaseCalculoSEST.ToString(),
                ValorSest = pagamento.ValorSEST.ToString(),
                RepomContractCode = contratoFrete?.ConfiguracaoCIOT?.OperadoraCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.RepomFrete ? (pagamento.CodigoViagem > 0 ? pagamento.CodigoViagem.ToString() : null) : null
            };
        }

        private string PrencherCodigoGetCodPagamentoMotorista(Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !string.IsNullOrEmpty(retWS.jsonRetorno))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadraoV2 retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadraoV2>(retWS.jsonRetorno);
                var campos = retorno?.Result;
                if (campos != null)
                {
                    if ((campos.LancamentoID ?? 0) > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorPagamentoMotoristaTMSETipo(pagamentoMotorista.Codigo, TipoCodigoExternoIntegracao.PagamentoMotoristaTMS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM);
                        if (integracaoCodigoExterno == null)
                        {
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();
                            integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.PagamentoMotoristaTMS;
                            integracaoCodigoExterno.PagamentoMotoristaTMS = pagamentoMotorista;
                            integracaoCodigoExterno.CodigoExterno = campos.LancamentoID.ToString();
                            integracaoCodigoExterno.TipoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM;
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);

                            return integracaoCodigoExterno.CodigoExterno;
                        }
                    }
                }
            }

            return "";
        }
    }
}