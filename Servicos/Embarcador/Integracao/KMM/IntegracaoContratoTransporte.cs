using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {
        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            bool sucessoIntegracao = IntegrarCargaKMM(cargaIntegracao);

            if (sucessoIntegracao)
            {
                IntegrarContratoFrete(cargaIntegracao);
            }
        }

        public bool IntegrarCargaKMM(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            bool sucessoIntegracao = false;

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                Hashtable parameters = ConverterCarga(cargaIntegracao.Carga);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "insCarga" },
                    { "parameters", parameters }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                cargaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                cargaIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                var retornoWS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadraoV2>(retWS.jsonRetorno);
                if (retornoWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                {
                    sucessoIntegracao = false;
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    sucessoIntegracao = true;
                }

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaIntegracao.ProblemaIntegracao = message;
                sucessoIntegracao = false;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);

            return sucessoIntegracao;
        }

        public void IntegrarContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repositorioContratoFrete.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

            // valida a existência do contrato
            if (contratoFrete == null)
            {
                return;
            }

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                var objetoIntegracao = ObterContratoTransporte(cargaIntegracao.Carga, contratoFrete);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "importarCT" },
                    { "parameters", objetoIntegracao }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                cargaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                cargaIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                var retornoWS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadraoV2>(retWS.jsonRetorno);
                if (retornoWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    this.PrencherCodigoGetCodLancamento(retWS, contratoFrete);

                    if (contratoFrete.ValoresAdicionais != null && contratoFrete.ValoresAdicionais.Count > 0)
                    {
                        foreach (var valor in contratoFrete.ValoresAdicionais)
                        {
                            string mensagem = "";
                            if (!this.IntegrarContratoFreteAcrescimosDescontos(cargaIntegracao, contratoFrete, valor, out mensagem))
                            {
                                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            }
                        }
                        if (cargaIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                            cargaIntegracao.ProblemaIntegracao += "| Problema ao integrar item(ns) do contrato de frete ";
                    }
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarCargaEncerramento(Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao encerramentoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repositorioEncerramentoIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            if (!(configuracaoIntegracaoKMM?.PossuiIntegracao ?? false))
            {
                encerramentoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                encerramentoCarga.ProblemaIntegracao = "Não possui configuração para KMMM";

                repositorioEncerramentoIntegracao.Atualizar(encerramentoCarga);

                return;
            }

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                for (int i = 0; i < encerramentoCarga.CargaRegistroEncerramento.Carga.CargaCTes.Count(); i++)
                {
                    try
                    {
                        encerramentoCarga.DataIntegracao = DateTime.Now;
                        encerramentoCarga.NumeroTentativas++;

                        string chave = encerramentoCarga.CargaRegistroEncerramento.Carga.CargaCTes.ElementAt(i).CTe.Chave.ToString();
                        Hashtable body = new Hashtable
                        {
                            {  "chave_cte", chave }
                        };

                        Hashtable request = new Hashtable
                        {
                            { "module", "M1076" },
                            { "operation", "encerrarRomaneioCTe" },
                            { "parameters", body }
                        };

                        var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                        encerramentoCarga.SituacaoIntegracao = retWS.SituacaoIntegracao;
                        encerramentoCarga.ProblemaIntegracao = retWS.ProblemaIntegracao;
                        jsonRequisicao = retWS.jsonRequisicao;
                        jsonRetorno = retWS.jsonRetorno;
                    }
                    catch (Exception excecao)
                    {
                        Log.TratarErro(excecao);

                        encerramentoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        encerramentoCarga.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da KMM";
                    }

                    servicoArquivoTransacao.Adicionar(encerramentoCarga, jsonRequisicao, jsonRetorno, "json");

                    repositorioEncerramentoIntegracao.Atualizar(encerramentoCarga);
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                encerramentoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                encerramentoCarga.ProblemaIntegracao = message;

                repositorioEncerramentoIntegracao.Atualizar(encerramentoCarga);
            }
        }

        public bool IntegrarCIOTAcrescimosDescontos(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, out string mensagemRetorno)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            Repositorio.Embarcador.Terceiros.ContratoFrete repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo();
            Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            mensagemRetorno = string.Empty;

            try
            {
                string codigoExternoLancamentoID = repIntegracaoCodigoExterno.BuscarPorContratoFreteETipo(integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.Codigo, TipoCodigoExternoIntegracao.ContratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM)?.CodigoExterno ?? null;

                Hashtable objetoIntegracao = new Hashtable
                {
                    { "cnpj_filial", integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.Carga.Filial.CNPJ },
                    { "tipo_pagamento", "1" },
                    { "serie", null },
                    { "num_formulario", integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.Codigo },
                    { "item_codigo", integracaoContrato.ContratoFreteAcrescimoDesconto.Justificativa.Codigo },
                    { "valor", integracaoContrato.ContratoFreteAcrescimoDesconto.Valor },
                    { "data", integracaoContrato.ContratoFreteAcrescimoDesconto.Data.ToString("dd/MM/yyyy HH:mm:ss") },
                    { "observacao", integracaoContrato.ContratoFreteAcrescimoDesconto.Observacao },
                    { "lancto_id", codigoExternoLancamentoID }

                };

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "importarCTItem" },
                    { "parameters", objetoIntegracao }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);


                integracaoContrato.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
                mensagemRetorno = jsonRetorno.ToString();
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                mensagemRetorno = excecao.Message;

                integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork);
                integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork);
                integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                integracaoArquivo.Mensagem = mensagemRetorno;

                return false;
            }

            integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork);
            integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork);
            integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            integracaoArquivo.Mensagem = mensagemRetorno;

            repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);

            return true;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.AcrescimoDesconto IntegrarContratoFreteAcrescimosDescontos(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao integracaoContratoValores)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.AcrescimoDesconto retorno = null;

            Repositorio.Embarcador.Terceiros.ContratoFreteValoresIntegracao repositorioPagamentoContrato = new Repositorio.Embarcador.Terceiros.ContratoFreteValoresIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {


                integracaoContratoValores.DataIntegracao = DateTime.Now;
                integracaoContratoValores.NumeroTentativas++;

                Hashtable objetoIntegracao = new Hashtable
                {
                    { "placa", integracaoContratoValores.ContratoFrete.Carga.Veiculo.Placa },
                    { "cnpj_cpf", integracaoContratoValores.ContratoFrete.Carga.Terceiro.Tipo == "J" ? integracaoContratoValores.ContratoFrete.Carga.Terceiro.CPF_CNPJ.ToString().PadLeft(14,'0') : integracaoContratoValores.ContratoFrete.Carga.Terceiro.CPF_CNPJ.ToString().PadLeft(11,'0') },
                    { "data_emissao", (integracaoContratoValores.ContratoFrete.Carga?.DataCarregamentoCarga ?? integracaoContratoValores.ContratoFrete.Carga.DataCriacaoCarga) .ToString("dd/MM/yyyy") },
                    { "acrescimo_desconto", "0" },
                };

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "consultarTaxa" },
                    { "parameters", objetoIntegracao }
                };


                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                integracaoContratoValores.SituacaoIntegracao = retWS.SituacaoIntegracao;
                integracaoContratoValores.ProblemaIntegracao = retWS.ProblemaIntegracao;

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.AcrescimoDesconto>(jsonRetorno);

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracaoContratoValores.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                integracaoContratoValores.ProblemaIntegracao = message;
            }
            servicoArquivoTransacao.Adicionar(integracaoContratoValores, jsonRequisicao, jsonRetorno, "json");

            repositorioPagamentoContrato.Atualizar(integracaoContratoValores);

            return retorno;
        }

        public bool? AdicionarContratoFreteValoresAcrescimoDescontoIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteValoresIntegracao repContratoValoresIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteValoresIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao contratoIntegracao = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao
            {
                TipoIntegracao = tipo,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                ContratoFrete = contratoFrete
            };

            repContratoValoresIntegracao.Inserir(contratoIntegracao);

            var retornoContratos = new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(_unitOfWork).IntegrarContratoFreteAcrescimosDescontos(contratoIntegracao);

            if (retornoContratos != null && retornoContratos.Detalhe != null)
            {
                return false;
            }

            if (retornoContratos != null && retornoContratos.Resultado != null)
            {
                string mensagem = "";
                if (!this.AdicionarAcrescimoDescontoViaIntegracao(carga, contratoFrete, retornoContratos.Resultado, auditado, tipoServicoMultisoftware, contratoIntegracao, ref mensagem))
                {
                    contratoIntegracao.ProblemaIntegracao = mensagem;
                    contratoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    repContratoValoresIntegracao.Atualizar(contratoIntegracao);
                    return false;
                }

                return true;
            }

            return false;
        }
        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContratoTransporte ObterContratoTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            string cavalo = carga.Veiculo?.Placa.ToString();
            string carreta;
            if (carga.Veiculo?.TipoVeiculo == "1")
            {
                carreta = cavalo;
            }
            else if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
            {
                carreta = carga.VeiculosVinculados?.ElementAt(0)?.Placa.ToString();
            }
            else
            {
                carreta = "";
            }

            var cargaPedido = carga.Pedidos?.ElementAt(0);

            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaValePedagio = repositorioCargaIntegracaoValePedagio.BuscarPorCarga(carga.Codigo, SituacaoIntegracao.Integrado);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = listaValePedagio.FirstOrDefault();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContratoTransporte()
            {
                CNPJFilial = carga.Empresa?.CNPJ,
                TipoPagamento = this.ObterCodigoTipoPagamentoKMM(contratoFrete.ConfiguracaoCIOT),
                Serie = null,
                NumFormulario = contratoFrete.NumeroContrato.ToString(),
                NumeroCRTSistemaExterno = contratoFrete.Codigo.ToString(),
                DataEmissao = contratoFrete.DataEmissaoContrato.ToString("yyyy-MM-dd HH:mm:ss"),
                RemetenteCnpjCpf = LPadCpfCnpj(cargaPedido.Pedido.Remetente?.Tipo.ToString(), cargaPedido.Pedido.Remetente?.CPF_CNPJ.ToString()),
                DestinatarioCnpjCpf = LPadCpfCnpj(cargaPedido.Pedido.Destinatario?.Tipo.ToString(), cargaPedido.Pedido.Destinatario?.CPF_CNPJ.ToString()),
                Peso = cargaPedido.Pedido?.PesoTotal.ToString(),
                Volume = cargaPedido.Pedido?.QtVolumes.ToString(),
                M3 = cargaPedido.Pedido?.CubagemTotal.ToString(),
                ValorUnitario = Servicos.Embarcador.Terceiros.ContratoFrete.RetornarValorFreteSubcontratacaoSemAcrescimoDesconto(contratoFrete).ToString(),
                // no KMM o valor bruto está invertido com o líquido
                ValorFreteBruto = contratoFrete.ValorBrutoComAcrescimoDescontoSaldo.ToString(),
                ValorFreteLiquido = contratoFrete.ValorBruto.ToString(),
                MotoristaCPF = carga.Motoristas?.FirstOrDefault().CPF,
                PlacaControle = cavalo,
                PlacaReferencia = carreta,
                ValorItemAdiantamento = contratoFrete.ValorAdiantamento.ToString(),
                ValorPedagio = contratoFrete.ValorPedagio.ToString(),
                CentroCustoGerencial = "1",
                Observacao = contratoFrete.Observacao,
                TotalAcrescimos = contratoFrete.ValorTotalAcrescimoAdiantamento.ToString(),
                TotalDescontos = contratoFrete.ValorTotalDescontoSaldo.ToString(),
                NumeroDocumento = contratoFrete.NumeroControle.ToString(),
                //na kmm, cliente é o tomador
                NomeCliente = cargaPedido.Pedido?.Tomador?.Nome,
                CNPJCliente = LPadCpfCnpj(cargaPedido.Pedido.Tomador?.Tipo.ToString(), cargaPedido.Pedido.Tomador?.CPF_CNPJ.ToString()),
                CentroCustoOriginal = null,
                GerarCPG = 1,
                CodigoCargaEmbarcador = servicoCarga.ObterNumeroCarga(carga, configuracaoEmbarcador),
                VencimentoAdiantamento = contratoFrete.DataEmissaoContrato.AddDays(contratoFrete.DiasVencimentoAdiantamento).ToString("yyyy-MM-dd HH:mm:ss"),
                VencimentoSaldo = Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(contratoFrete, contratoFrete.DataEmissaoContrato).ToString("yyyy-MM-dd HH:mm:ss"),
                CentroCusto = cargaPedido.Pedido?.CentroDeCustoViagem?.CodigoIntegracao,
                TipoCompraValePedagio = cargaIntegracaoValePedagio != null ? cargaIntegracaoValePedagio.TipoCompra.ObterDescricao() : null,
                BaseInss = contratoFrete.BaseCalculoINSS.ToString(),
                ValorInss = contratoFrete.ValorINSS.ToString(),
                BaseIrrf = contratoFrete.BaseCalculoIRRF.ToString(),
                ValorIrrf = contratoFrete.ValorIRRF.ToString(),
                BaseSenat = contratoFrete.BaseCalculoSENAT.ToString(),
                ValorSenat = contratoFrete.ValorSENAT.ToString(),
                BaseSest = contratoFrete.BaseCalculoSEST.ToString(),
                ValorSest = contratoFrete.ValorSEST.ToString(),
                ValorAbastecimento = contratoFrete.ValorAbastecimento.ToString(),
                RepomContractCode = contratoFrete.ConfiguracaoCIOT?.OperadoraCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.RepomFrete ? carga.CargaCIOTs?.Where(q => q.ContratoFrete.Codigo == contratoFrete.Codigo)?.FirstOrDefault()?.CIOT?.ProtocoloAutorizacao ?? null : null,
            };
        }

        private Hashtable ConverterCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            List<String> composicao = new List<String>();
            string cavalo = carga.Veiculo?.Placa.ToString();
            if (!String.IsNullOrEmpty(cavalo))
            {
                composicao.Add(cavalo);
            }

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
            {
                for (int i = 0; i < carga.VeiculosVinculados.Count; i++)
                {
                    composicao.Add(carga.VeiculosVinculados?.ElementAt(i).Placa.ToString());
                }
            }

            List<String> motoristas = new List<String>();
            if (carga.Motoristas != null && carga.Motoristas.Count > 0)
            {
                for (int i = 0; i < carga.Motoristas.Count; i++)
                {
                    motoristas.Add(carga.Motoristas.ElementAt(i).CPF.ToString());
                }
            }

            var cargaPedido = carga.Pedidos?.ElementAt(0);

            Hashtable cargaHash = new Hashtable();
            cargaHash.Add("codigo_carga_embarcador", servicoCarga.ObterNumeroCarga(carga, configuracaoEmbarcador));//carga.CodigoCargaEmbarcador?.ToString());
            cargaHash.Add("cnpj_filial", carga.Empresa?.CNPJ.ToString());
            cargaHash.Add("data_inicio_viagem", carga.DataInicioViagem?.ToString("yyyy-MM-dd HH:mm:ss"));
            cargaHash.Add("data_inicio_viagem_prevista", carga.DataInicioViagemPrevista?.ToString("yyyy-MM-dd HH:mm:ss"));
            cargaHash.Add("data_inicio_viagem_reprogramada", carga.DataInicioViagemReprogramada?.ToString("yyyy-MM-dd HH:mm:ss"));
            cargaHash.Add("data_coleta", cargaPedido.Pedido?.DataCarregamentoPedido?.ToString("yyyy-MM-dd HH:mm:ss"));
            cargaHash.Add("centro_custo", cargaPedido.Pedido?.CentroDeCustoViagem?.CodigoIntegracao);
            cargaHash.Add("composicao", String.Join(",", composicao));

            // Remetente
            cargaHash.Add("remetente_cnpj_cpf", LPadCpfCnpj(cargaPedido.Pedido.Remetente?.Tipo.ToString(), cargaPedido.Pedido.Remetente?.CPF_CNPJ.ToString()));
            if (cargaPedido.Pedido.Remetente?.Localidade != null)
            {
                cargaHash.Add("remetente_cep", cargaPedido.Pedido.Remetente.CEP.ToString());
                cargaHash.Add("remetente_endereco", cargaPedido.Pedido?.Remetente.Endereco);
                cargaHash.Add("remetente_cidade", cargaPedido.Pedido?.Remetente.Localidade?.Descricao);
                cargaHash.Add("remetente_bairro", cargaPedido.Pedido?.Remetente.Bairro);
                cargaHash.Add("remetente_uf", cargaPedido.Pedido?.Remetente.Localidade?.Estado.Sigla.ToString());
                cargaHash.Add("remetente_pais", cargaPedido.Pedido?.Remetente.Localidade?.Pais?.Nome.ToString());
                cargaHash.Add("remetente_ibge", cargaPedido.Pedido?.Remetente.Localidade?.CodigoIBGE.ToString());
            }

            // Destinatario
            cargaHash.Add("destinatario_cnpj_cpf", LPadCpfCnpj(cargaPedido.Pedido.Destinatario?.Tipo.ToString(), cargaPedido.Pedido.Destinatario?.CPF_CNPJ.ToString()));
            if (cargaPedido.Pedido.Destinatario?.Localidade != null)
            {
                cargaHash.Add("destinatario_cep", cargaPedido.Pedido.Destinatario.CEP.ToString());
                cargaHash.Add("destinatario_endereco", cargaPedido.Pedido?.Destinatario.Endereco);
                cargaHash.Add("destinatario_cidade", cargaPedido.Pedido?.Destinatario.Localidade?.Descricao);
                cargaHash.Add("destinatario_bairro", cargaPedido.Pedido?.Destinatario.Bairro);
                cargaHash.Add("destinatario_uf", cargaPedido.Pedido?.Destinatario.Localidade?.Estado.Sigla.ToString());
                cargaHash.Add("destinatario_pais", cargaPedido.Pedido?.Destinatario.Localidade?.Pais?.Nome.ToString());
                cargaHash.Add("destinatario_ibge", cargaPedido.Pedido?.Destinatario.Localidade?.CodigoIBGE.ToString());
            }

            // Tomador
            cargaHash.Add("tomador_cnpj_cpf", LPadCpfCnpj(cargaPedido.Pedido.Tomador?.Tipo.ToString(), cargaPedido.Pedido.Tomador?.CPF_CNPJ.ToString()));
            if (cargaPedido.Pedido.Tomador?.Localidade != null)
            {
                cargaHash.Add("tomador_cep", cargaPedido.Pedido.Tomador.CEP.ToString());
                cargaHash.Add("tomador_endereco", cargaPedido.Pedido?.Tomador.Endereco);
                cargaHash.Add("tomador_cidade", cargaPedido.Pedido?.Tomador.Localidade?.Descricao);
                cargaHash.Add("tomador_bairro", cargaPedido.Pedido?.Tomador.Bairro);
                cargaHash.Add("tomador_uf", cargaPedido.Pedido?.Tomador.Localidade?.Estado.Sigla.ToString());
                cargaHash.Add("tomador_pais", cargaPedido.Pedido?.Tomador.Localidade?.Pais?.Nome.ToString());
                cargaHash.Add("tomador_ibge", cargaPedido.Pedido?.Tomador.Localidade?.CodigoIBGE.ToString());
            }

            // Recebedor
            cargaHash.Add("recebedor_cnpj_cpf", LPadCpfCnpj(cargaPedido.Pedido.Recebedor?.Tipo.ToString(), cargaPedido.Pedido.Recebedor?.CPF_CNPJ.ToString()));
            if (cargaPedido.Pedido.Recebedor?.Localidade != null)
            {
                cargaHash.Add("recebedor_cep", cargaPedido.Pedido.Recebedor.CEP.ToString());
                cargaHash.Add("recebedor_endereco", cargaPedido.Pedido?.Recebedor.Endereco);
                cargaHash.Add("recebedor_cidade", cargaPedido.Pedido?.Recebedor.Localidade?.Descricao);
                cargaHash.Add("recebedor_bairro", cargaPedido.Pedido?.Recebedor.Bairro);
                cargaHash.Add("recebedor_uf", cargaPedido.Pedido?.Recebedor.Localidade?.Estado.Sigla.ToString());
                cargaHash.Add("recebedor_pais", cargaPedido.Pedido?.Recebedor.Localidade?.Pais?.Nome.ToString());
                cargaHash.Add("recebedor_ibge", cargaPedido.Pedido?.Recebedor.Localidade?.CodigoIBGE.ToString());
            }

            // Recebedor Coleta
            cargaHash.Add("recebedor_coleta_cnpj_cpf", LPadCpfCnpj(cargaPedido.Pedido.RecebedorColeta?.Tipo.ToString(), cargaPedido.Pedido.RecebedorColeta?.CPF_CNPJ.ToString()));
            if (cargaPedido.Pedido.RecebedorColeta?.Localidade != null)
            {
                cargaHash.Add("recebedor_coleta_cep", cargaPedido.Pedido.RecebedorColeta.CEP.ToString());
                cargaHash.Add("recebedor_coleta_endereco", cargaPedido.Pedido?.RecebedorColeta.Endereco);
                cargaHash.Add("recebedor_coleta_cidade", cargaPedido.Pedido?.RecebedorColeta.Localidade?.Descricao);
                cargaHash.Add("recebedor_coleta_bairro", cargaPedido.Pedido?.RecebedorColeta.Bairro);
                cargaHash.Add("recebedor_coleta_uf", cargaPedido.Pedido?.RecebedorColeta.Localidade?.Estado.Sigla.ToString());
                cargaHash.Add("recebedor_coleta_pais", cargaPedido.Pedido?.RecebedorColeta.Localidade?.Pais?.Nome.ToString());
                cargaHash.Add("recebedor_coleta_ibge", cargaPedido.Pedido?.RecebedorColeta.Localidade?.CodigoIBGE.ToString());
            }

            // Expedidor
            cargaHash.Add("expedidor_cnpj_cpf", LPadCpfCnpj(cargaPedido.Pedido.Expedidor?.Tipo.ToString(), cargaPedido.Pedido.Expedidor?.CPF_CNPJ.ToString()));
            if (cargaPedido.Pedido.Expedidor?.Localidade != null)
            {
                cargaHash.Add("expedidor_cep", cargaPedido.Pedido.Expedidor.CEP.ToString());
                cargaHash.Add("expedidor_endereco", cargaPedido.Pedido?.Expedidor.Endereco);
                cargaHash.Add("expedidor_cidade", cargaPedido.Pedido?.Expedidor.Localidade?.Descricao);
                cargaHash.Add("expedidor_bairro", cargaPedido.Pedido?.Expedidor.Bairro);
                cargaHash.Add("expedidor_uf", cargaPedido.Pedido?.Expedidor.Localidade?.Estado.Sigla.ToString());
                cargaHash.Add("expedidor_pais", cargaPedido.Pedido?.Expedidor.Localidade?.Pais?.Nome.ToString());
                cargaHash.Add("expedidor_ibge", cargaPedido.Pedido?.Expedidor.Localidade?.CodigoIBGE.ToString());
            }


            cargaHash.Add("tipo_carga", carga.TipoDeCarga?.Descricao);
            cargaHash.Add("rg_motoristas", carga.RGMotoristas);
            cargaHash.Add("cpf_motoristas", String.Join(",", motoristas));

            if (!carga.CargaTransbordo)
            {
                // documentos
                List<String> documentos = new List<String>();
                if (carga.CargaCTes != null && carga.CargaCTes.Count > 0)
                {
                    for (int i = 0; i < carga.CargaCTes.Count; i++)
                    {
                        if (carga.CargaCTes.ElementAt(i).CTe.ModeloDocumentoFiscal != null && carga.CargaCTes.ElementAt(i).CargaCTeComplementoInfo == null)
                        {
                            if (carga.CargaCTes.ElementAt(i).CTe.ModeloDocumentoFiscal.Abreviacao.Equals("CT-e"))
                            {
                                documentos.Add(carga.CargaCTes.ElementAt(i).CTe.Numero + "," + carga.CargaCTes.ElementAt(i).CTe.Serie?.Numero + "," + carga.CargaCTes.ElementAt(i).CTe.ModeloDocumentoFiscal.Abreviacao);
                            }
                            if (carga.CargaCTes.ElementAt(i).CTe.ModeloDocumentoFiscal.Abreviacao.Equals("NFS-e"))
                            {
                                documentos.Add(carga.CargaCTes.ElementAt(i).CTe.Numero + "," + carga.CargaCTes.ElementAt(i).CTe.Serie?.Numero + "," + carga.CargaCTes.ElementAt(i).CTe.ModeloDocumentoFiscal.Abreviacao);

                            }
                            if (carga.CargaCTes.ElementAt(i).CTe.ModeloDocumentoFiscal.Abreviacao.Equals("ND")
                                || carga.CargaCTes.ElementAt(i).CTe.ModeloDocumentoFiscal.Abreviacao.Equals("CV")
                                || carga.CargaCTes.ElementAt(i).CTe.ModeloDocumentoFiscal.Abreviacao.Equals("DV")
                                || carga.CargaCTes.ElementAt(i).CTe.ModeloDocumentoFiscal.Abreviacao.Equals("OST"))
                            {
                                documentos.Add(carga.CargaCTes.ElementAt(i).CTe.Numero + "," + carga.CargaCTes.ElementAt(i).CTe.Serie?.Numero + "," + carga.CargaCTes.ElementAt(i).CTe.ModeloDocumentoFiscal.Abreviacao);
                            }
                        }
                    }

                    cargaHash.Add("data_emissao", carga.CargaCTes.ElementAt(0).CTe.DataEmissao?.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                if (documentos.Count > 0)
                {
                    cargaHash.Add("documentos", String.Join(";", documentos));
                }

            }

            // Centro de Resultado
            if (cargaPedido.Pedido?.CentroResultado == null)
            {
                throw new Exception("O pedido da carga não possui um centro de resultado definido para ser possível realizar a integração.");
            }
            cargaHash.Add("centro_resultado", cargaPedido.Pedido?.CentroResultado?.Descricao.ToString() + "," + cargaPedido.Pedido?.CentroResultado?.Plano.ToString());

            // Origem do Pedido
            if (cargaPedido?.Origem != null)
            {
                cargaHash.Add("municipio_origem", cargaPedido.Origem.Descricao.ToString());
                cargaHash.Add("uf_origem", cargaPedido.Origem.Estado.Sigla.ToString());
            }

            if (cargaPedido.Pedido?.EnderecoOrigem != null)
            {
                cargaHash.Add("endereco_origem", cargaPedido.Pedido?.EnderecoOrigem.Descricao);
                cargaHash.Add("cidade_origem", cargaPedido.Pedido?.EnderecoOrigem.Localidade?.Descricao);
                cargaHash.Add("bairro_origem", cargaPedido.Pedido?.EnderecoOrigem.Bairro);
                cargaHash.Add("cep_origem", cargaPedido.Pedido?.EnderecoOrigem.CEP);
                cargaHash.Add("pais_origem", cargaPedido.Pedido?.EnderecoOrigem.Localidade?.Pais?.Nome);
                cargaHash.Add("ibge_origem", cargaPedido.Pedido?.EnderecoOrigem.Localidade?.CodigoIBGE.ToString());
            }
            else if (cargaPedido?.Pedido?.Remetente != null)
            {
                cargaHash.Add("endereco_origem", cargaPedido.Pedido?.Remetente.Endereco);
                cargaHash.Add("cidade_origem", cargaPedido.Pedido?.Remetente.Localidade?.Descricao);
                cargaHash.Add("bairro_origem", cargaPedido.Pedido?.Remetente.Bairro);
                cargaHash.Add("cep_origem", cargaPedido.Pedido?.Remetente.CEP);
                cargaHash.Add("pais_origem", cargaPedido.Pedido?.Remetente.Localidade?.Pais?.Nome);
                cargaHash.Add("ibge_origem", cargaPedido.Pedido?.Remetente.Localidade?.CodigoIBGE.ToString());
            }

            // Destino Pedido
            if (cargaPedido?.Destino != null)
            {
                cargaHash.Add("municipio_destino", cargaPedido?.Destino.Descricao.ToString());
                cargaHash.Add("uf_destino", cargaPedido?.Destino.Estado.Sigla.ToString());
            }

            if (cargaPedido.Pedido?.EnderecoDestino != null)
            {
                cargaHash.Add("endereco_destino", cargaPedido.Pedido?.EnderecoDestino.Descricao);
                cargaHash.Add("cidade_destino", cargaPedido.Pedido?.EnderecoDestino.Localidade?.Descricao);
                cargaHash.Add("bairro_destino", cargaPedido.Pedido?.EnderecoDestino.Bairro);
                cargaHash.Add("cep_destino", cargaPedido.Pedido?.EnderecoDestino.CEP);
                cargaHash.Add("pais_destino", cargaPedido.Pedido?.EnderecoDestino.Localidade?.Pais?.Nome);
                cargaHash.Add("ibge_destino", cargaPedido.Pedido?.EnderecoDestino.Localidade?.CodigoIBGE.ToString());
            }
            else if (cargaPedido?.Pedido?.Destinatario != null)
            {
                cargaHash.Add("endereco_destino", cargaPedido.Pedido?.Destinatario?.Endereco);
                cargaHash.Add("cidade_destino", cargaPedido.Pedido?.Destinatario.Localidade?.Descricao);
                cargaHash.Add("bairro_destino", cargaPedido.Pedido?.Destinatario.Bairro);
                cargaHash.Add("cep_destino", cargaPedido.Pedido?.Destinatario.CEP);
                cargaHash.Add("pais_destino", cargaPedido.Pedido?.Destinatario.Localidade?.Pais?.Nome);
                cargaHash.Add("ibge_destino", cargaPedido.Pedido?.Destinatario.Localidade?.CodigoIBGE.ToString());
            }

            // Peso Total
            if (cargaPedido.Pedido?.PesoTotal != null)
            {
                cargaHash.Add("peso_total", cargaPedido.Pedido?.PesoTotal.ToString());
            }

            cargaHash.Add("carga_protocolo", carga.Protocolo);
            cargaHash.Add("tipo_emissao_cte_participantes", cargaPedido.TipoEmissaoCTeParticipantes.ObterDescricao().ToString());

            if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
            {
                if (cargaPedido.Recebedor != null)
                {
                    cargaHash.Add("pedido_recebedor_cnpj_cpf", LPadCpfCnpj(cargaPedido.Recebedor?.Tipo.ToString(), cargaPedido.Recebedor?.CPF_CNPJ.ToString()));
                    if (cargaPedido.Recebedor?.Localidade != null)
                    {
                        cargaHash.Add("pedido_recebedor_cep", cargaPedido.Recebedor.CEP.ToString());
                        cargaHash.Add("pedido_recebedor_endereco", cargaPedido.Recebedor.Endereco);
                        cargaHash.Add("pedido_recebedor_cidade", cargaPedido.Recebedor.Localidade?.Descricao);
                        cargaHash.Add("pedido_recebedor_bairro", cargaPedido.Recebedor.Bairro);
                        cargaHash.Add("pedido_recebedor_uf", cargaPedido.Recebedor.Localidade?.Estado.Sigla.ToString());
                        cargaHash.Add("pedido_recebedor_pais", cargaPedido.Recebedor.Localidade?.Pais?.Nome.ToString());
                        cargaHash.Add("pedido_recebedor_ibge", cargaPedido.Recebedor.Localidade?.CodigoIBGE.ToString());
                    }
                }
                if (cargaPedido.Expedidor != null)
                {
                    cargaHash.Add("pedido_expedidor_cnpj_cpf", LPadCpfCnpj(cargaPedido.Expedidor?.Tipo.ToString(), cargaPedido.Expedidor?.CPF_CNPJ.ToString()));
                    if (cargaPedido.Expedidor?.Localidade != null)
                    {
                        cargaHash.Add("pedido_expedidor_cep", cargaPedido.Expedidor.CEP.ToString());
                        cargaHash.Add("pedido_expedidor_endereco", cargaPedido.Expedidor.Endereco);
                        cargaHash.Add("pedido_expedidor_cidade", cargaPedido.Expedidor.Localidade?.Descricao);
                        cargaHash.Add("pedido_expedidor_bairro", cargaPedido.Expedidor.Bairro);
                        cargaHash.Add("pedido_expedidor_uf", cargaPedido.Expedidor.Localidade?.Estado.Sigla.ToString());
                        cargaHash.Add("pedido_expedidor_pais", cargaPedido.Expedidor.Localidade?.Pais?.Nome.ToString());
                        cargaHash.Add("pedido_expedidor_ibge", cargaPedido.Expedidor.Localidade?.CodigoIBGE.ToString());
                    }
                }
            }

            Dominio.Entidades.Cliente pedidoDestinatario = cargaPedido.ObterDestinatario();
            if (pedidoDestinatario != null)
            {
                cargaHash.Add("pedido_destinatario_cnpj_cpf", LPadCpfCnpj(pedidoDestinatario.Tipo.ToString(), pedidoDestinatario.CPF_CNPJ.ToString()));
                if (pedidoDestinatario.Localidade != null)
                {
                    cargaHash.Add("pedido_destinatario_cep", pedidoDestinatario.CEP.ToString());
                    cargaHash.Add("pedido_destinatario_endereco", pedidoDestinatario.Endereco);
                    cargaHash.Add("pedido_destinatario_cidade", pedidoDestinatario.Localidade?.Descricao);
                    cargaHash.Add("pedido_destinatario_bairro", pedidoDestinatario.Bairro);
                    cargaHash.Add("pedido_destinatario_uf", pedidoDestinatario.Localidade?.Estado.Sigla.ToString());
                    cargaHash.Add("pedido_destinatario_pais", pedidoDestinatario.Localidade?.Pais?.Nome.ToString());
                    cargaHash.Add("pedido_destinatario_ibge", pedidoDestinatario.Localidade?.CodigoIBGE.ToString());
                }
            }

            Dominio.Entidades.Cliente pedidoTomador = cargaPedido.ObterTomador();
            if (pedidoTomador != null)
            {
                cargaHash.Add("pedido_tomador_cnpj_cpf", LPadCpfCnpj(pedidoTomador.Tipo.ToString(), pedidoTomador.CPF_CNPJ.ToString()));
                if (pedidoTomador.Localidade != null)
                {
                    cargaHash.Add("pedido_tomador_cep", pedidoTomador.CEP.ToString());
                    cargaHash.Add("pedido_tomador_endereco", pedidoTomador.Endereco);
                    cargaHash.Add("pedido_tomador_cidade", pedidoTomador.Localidade?.Descricao);
                    cargaHash.Add("pedido_tomador_bairro", pedidoTomador.Bairro);
                    cargaHash.Add("pedido_tomador_uf", pedidoTomador.Localidade?.Estado.Sigla.ToString());
                    cargaHash.Add("pedido_tomador_pais", pedidoTomador.Localidade?.Pais?.Nome.ToString());
                    cargaHash.Add("pedido_tomador_ibge", pedidoTomador.Localidade?.CodigoIBGE.ToString());
                }
            }

            return cargaHash;
        }

        private bool AdicionarAcrescimoDescontoViaIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Resultado integracaoAcrescimoDesconto, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValoresIntegracao contratoIntegracao, ref string mensagem)
        {
            bool sucesso = true;
            Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serCargaFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(_unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(_unitOfWork, tipoServicoMultisoftware);

            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteValoresIntegracao repContratoFreteValoresIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteValoresIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);

            int codigoTaxaTerceiro = 0;
            decimal valor = integracaoAcrescimoDesconto.Valor;
            string observacao = "";

            Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repJustificativa.BuscarPorCodigoIntegracao(integracaoAcrescimoDesconto.Lancamento);

            if (justificativa == null)
            {
                mensagem = $"A justificativa não foi localizada com o código integração {integracaoAcrescimoDesconto.Lancamento ?? ""}";
                return false;
            }

            if (cargaCIOT != null && cargaCIOT.CIOT != null &&
                (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto ||
                 cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem ||
                 cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado) &&
                (contratoFrete.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte ||
                 contratoFrete.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada))
            {
                string mensagemErro = string.Empty;



                if (!justificativa.GerarMovimentoAutomatico)
                {
                    mensagem = "A justificativa não possui a movimentação financeira configurada, não sendo possível adicioná-la.";
                    return false;
                }

                if (justificativa.AplicacaoValorContratoFrete != AplicacaoValorJustificativaContratoFrete.NoTotal)
                {
                    mensagem = "A justificativa não está configurada para aplicação no total do contrato, não sendo possível adicioná-la.";
                    return false;
                }

                if (Servicos.Embarcador.CIOT.CIOT.IntegrarMovimentoFinanceiro(out mensagemErro, cargaCIOT, justificativa, valor, tipoServicoMultisoftware, _unitOfWork))
                {
                    Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor()
                    {
                        ContratoFrete = contratoFrete,
                        Justificativa = justificativa,
                        Valor = valor,
                        TipoJustificativa = justificativa.TipoJustificativa,
                        AplicacaoValor = justificativa.AplicacaoValorContratoFrete.HasValue ? justificativa.AplicacaoValorContratoFrete.Value : AplicacaoValorJustificativaContratoFrete.NoAdiantamento,
                        TipoMovimentoUso = justificativa.TipoMovimentoUsoJustificativa,
                        TipoMovimentoReversao = justificativa.TipoMovimentoReversaoUsoJustificativa,
                        Observacao = observacao
                    };

                    repContratoFreteValor.Inserir(contratoFreteValor, auditado);

                    if (contratoFreteValor.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                        contratoFrete.ValorFreteSubcontratacao = contratoFrete.ValorFreteSubcontratacao + valor;
                    else
                        contratoFrete.ValorFreteSubcontratacao = contratoFrete.ValorFreteSubcontratacao - valor;

                    Servicos.Auditoria.Auditoria.Auditar(auditado, contratoFrete.Carga, null, "Informou o valor " + valor.ToString("n2") + " de acréscimo/desconto ao contrato do terceiro.", _unitOfWork);

                }
                else
                {
                    mensagem = mensagemErro;
                    return false;
                }
            }

            if (contratoFrete.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
            {
                mensagem = "Não é possível alterar o contrato na situação atual da carga (" + contratoFrete.Carga.DescricaoSituacaoCarga + ").";
                return false;
            }

            if (contratoFrete.SituacaoContratoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto)
            {
                mensagem = "Não é possível adicionar um acréscimo/desconto na situação atual do contrato de frete.";
                return false;
            }

            contratoFrete.Initialize();

            _unitOfWork.Start();

            if (!servicoContratoFrete.AdicionarValorAoContrato(out string erro, ref contratoFrete, valor, justificativa.Codigo, observacao, codigoTaxaTerceiro, auditado))
            {
                _unitOfWork.Rollback();
                mensagem = erro;
                return false;
            }

            Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, _unitOfWork, tipoServicoMultisoftware);

            repContratoFrete.Atualizar(contratoFrete, auditado);

            _unitOfWork.CommitChanges();

            serCargaFreteSubcontratacaoTerceiro.ObterValorSubContratacao(contratoFrete);


            contratoIntegracao.ProblemaIntegracao = mensagem;
            contratoIntegracao.SituacaoIntegracao = sucesso ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;

            repContratoFreteValoresIntegracao.Atualizar(contratoIntegracao);

            return sucesso;
        }

        private int ObterCodigoTipoPagamentoKMM(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT)
        {
            /*
            Valores existentes no KMM: 
            1   Empresa
            2   REPOM
            3   Contrato Grupo
            4   PAMCARD
            6   PAGBEM
            7   TARGET
            8   VALECARD
            9   REPOM_FRETE
            10  WEX
            11  NDD
            12  EXTRATTA
            13  TIPBANK
            15  eFrete
            */

            int tipoPagamento = 1;
            if (configuracaoCIOT != null)
            {
                if (configuracaoCIOT.OperadoraCIOT == OperadoraCIOT.Repom)
                    tipoPagamento = 2;
                else if (configuracaoCIOT.OperadoraCIOT == OperadoraCIOT.Pamcard)
                    tipoPagamento = 4;
                else if (configuracaoCIOT.OperadoraCIOT == OperadoraCIOT.Pagbem)
                    tipoPagamento = 6;
                else if (configuracaoCIOT.OperadoraCIOT == OperadoraCIOT.Target)
                    tipoPagamento = 7;
                else if (configuracaoCIOT.OperadoraCIOT == OperadoraCIOT.RepomFrete)
                    tipoPagamento = 9;
                else if (configuracaoCIOT.OperadoraCIOT == OperadoraCIOT.Extratta)
                    tipoPagamento = 12;
                else if (configuracaoCIOT.OperadoraCIOT == OperadoraCIOT.eFrete)
                    tipoPagamento = 15;
            }

            return tipoPagamento;
        }

        private int ObterCodigoTipoPagamentoKMM(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista)
        {
            /*
            Valores existentes no KMM: 
            1   Empresa
            2   REPOM
            3   Contrato Grupo
            4   PAMCARD
            6   PAGBEM
            7   TARGET
            8   VALECARD
            9   REPOM_FRETE
            10  WEX
            11  NDD
            12  EXTRATTA
            13  TIPBANK
            15  eFrete
            */

            int tipoPagamento = 1;

            if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard)
                tipoPagamento = 4;
            else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target)
                tipoPagamento = 7;
            else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.RepomFrete)
                tipoPagamento = 9;
            else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta)
                tipoPagamento = 12;

            return tipoPagamento;
        }

        private bool IntegrarContratoFreteAcrescimosDescontos(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor contratoFreteValor, out string mensagemRetorno)
        {
            bool sucessoIntegracao = false;

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            Repositorio.Embarcador.Terceiros.ContratoFrete repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo();
            Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";
            mensagemRetorno = string.Empty;

            try
            {
                string codigoExternoLancamentoID = repIntegracaoCodigoExterno.BuscarPorContratoFreteETipo(contratoFrete.Codigo, TipoCodigoExternoIntegracao.ContratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM)?.CodigoExterno ?? null;

                Hashtable objetoIntegracao = new Hashtable
                {
                    { "cnpj_filial", contratoFrete.Carga.Empresa?.CNPJ ?? "" },
                    { "tipo_pagamento", this.ObterCodigoTipoPagamentoKMM(contratoFrete.ConfiguracaoCIOT) },
                    { "serie", null },
                    { "num_formulario", contratoFrete.NumeroContrato.ToString() },
                    { "item_codigo", contratoFreteValor.Justificativa.CodigoIntegracao },
                    { "valor", contratoFreteValor.Valor },
                    { "data", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
                    { "observacao", contratoFreteValor.Observacao },
                    { "lancto_id", codigoExternoLancamentoID }

                };

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "importarCTItem" },
                    { "parameters", objetoIntegracao }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
                mensagemRetorno = jsonRetorno.ToString();

                var retornoWS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadrao>(retWS.jsonRetorno);
                if (retornoWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                {
                    sucessoIntegracao = false;
                }
                else
                {
                    sucessoIntegracao = true;
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
                mensagemRetorno = message;

                sucessoIntegracao = false;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");


            return sucessoIntegracao;
        }
        private string PrencherCodigoGetCodLancamento(Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete)
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
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorContratoFreteETipo(contratoFrete.Codigo, TipoCodigoExternoIntegracao.ContratoFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM);
                        if (integracaoCodigoExterno == null)
                        {
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();
                            integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.ContratoFrete;
                            integracaoCodigoExterno.ContratoFrete = contratoFrete;
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
        #endregion
    }
}