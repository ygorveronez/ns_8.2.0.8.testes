using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SGT.BackgroundWorkers
{
    public class IntegrarCargaUnilever : LongRunningProcessBase<IntegrarCargaUnilever>
    {
        #region Métodos Privados

        private void VerificarIntegracoesAguardando(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Servicos.Log.TratarErro("Inicio Buscando Integrações Aguardando", "IntegracaoCargaUnilever");

                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);
                var listaIntegracoesAguardando = repIntegradoraIntegracaoRetorno.BuscarIntegracoesAguardando(30);

                Servicos.Log.TratarErro("Integrações Aguardando: " + listaIntegracoesAguardando.Count + "", "IntegracaoCargaUnilever");
                
                if (listaIntegracoesAguardando != null && listaIntegracoesAguardando.Count > 0)
                {
                    foreach (var codigoIntegracao in listaIntegracoesAguardando)
                        ProcessarIntegracao(unitOfWork, codigoIntegracao, tipoServicoMultisoftware);
                }

                Servicos.Log.TratarErro("Fim Integrações Aguardando", "IntegracaoCargaUnilever");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private static void ProcessarIntegracao(Repositorio.UnitOfWork unitOfWork, long codigoPedidoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);
                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integracao = repIntegradoraIntegracaoRetorno.BuscarPorCodigo(codigoPedidoIntegracao);

                if (integracao != null)
                {
                    if (integracao.ArquivoRequisicao == null)
                    {
                        integracao.Mensagem = "Arquivo de requisição não encontrado";
                        integracao.Situacao = SituacaoIntegracao.ProblemaIntegracao;
                        repIntegradoraIntegracaoRetorno.Atualizar(integracao);
                    }
                    else
                    {
                        string request = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(integracao.ArquivoRequisicao);

                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                        Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

                        string mensagemGeracaoCarga = string.Empty;
                        string numeroCarga = string.Empty;

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = GerarCarga(integracao.Codigo, integracao.Integradora.Codigo, request, ref numeroCarga, ref mensagemGeracaoCarga, unitOfWork, integracao.Integradora, tipoServicoMultisoftware);
                        string identificador = numeroCarga;

                        if (carga == null)
                        {
                            var resposta = new Retorno<int>() { Mensagem = mensagemGeracaoCarga, Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                            Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.AtualizarIntegracao(integracao, false, resposta.Mensagem, identificador, Utilidades.XML.Serializar(resposta, true), "xml", unitOfWork, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                        }
                        else if (!string.IsNullOrWhiteSpace(mensagemGeracaoCarga))
                        {
                            var resposta = new Retorno<int>() { Mensagem = mensagemGeracaoCarga, Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                            Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.AtualizarIntegracao(integracao, false, resposta.Mensagem, identificador, Utilidades.XML.Serializar(resposta, true), "xml", unitOfWork, carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                        }
                        else
                        {
                            var response = new Retorno<int>() { Mensagem = "", Status = true, Objeto = carga.Protocolo, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };

                            Servicos.Embarcador.Integracao.IntegradoraIntegracaoRetorno.AtualizarIntegracao(integracao, true, string.Empty, identificador, Utilidades.XML.Serializar(response, true), "xml", unitOfWork, carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                        }
                    }
                }
                unitOfWork.FlushAndClear();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();
            }
        }

        private static Dominio.Entidades.Embarcador.Cargas.Carga GerarCarga(long codigoIntegracao, int codigoIntegradora, string xmlCarga, ref string numeroCarga, ref string mensagemRetorno, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.WebService.Integradora integradora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                mensagemRetorno = string.Empty;
                numeroCarga = string.Empty;

                List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> listaCargaIntegracao = ConverterObjetoCargaIntegracao(xmlCarga, ref numeroCarga);

                if (listaCargaIntegracao == null || listaCargaIntegracao.Count <= 0)
                {
                    mensagemRetorno = "Não foi possível converter XML em carga, verifique o arquivo enviado e tente novamente.";
                    return null;
                }

                if (!ValidarIntegracaoAnterior(codigoIntegracao, codigoIntegradora, Newtonsoft.Json.JsonConvert.SerializeObject(listaCargaIntegracao), numeroCarga, unitOfWork))
                    throw new Exception("Integração sem alteração em relação a integração anterior.");

                //numeroCarga = listaCargaIntegracao.FirstOrDefault().NumeroCarga;

                StringBuilder mensagemErro = new StringBuilder();
                Servicos.WebService.Carga.Carga svcCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
                carga = svcCarga.GerarCargaPorListaCargaIntegracao(listaCargaIntegracao, ref mensagemErro, tipoServicoMultisoftware, null, unitOfWork);

                if (carga == null)
                {
                    mensagemRetorno = mensagemErro.ToString();
                    return null;
                }

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;
                auditado.Integradora = integradora;


                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Carga criada via integração", unitOfWork);

                return carga;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemRetorno = ex.Message;

                return null;
            }
        }

        private static List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> ConverterObjetoCargaIntegracao(string xmlCarga, ref string numeroCarga)
        {
            Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.Zp10swtDetTransDocIntResponse cargaUnilever = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.Zp10swtDetTransDocIntResponse));
            using (StringReader reader = new StringReader(xmlCarga))
            {
                cargaUnilever = (Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.Zp10swtDetTransDocIntResponse)serializer.Deserialize(reader);
            }

            List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> listaCargaIntegracao = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>();

            if (cargaUnilever.ExOutputTab.Item.Stage.ItemStage.Count <= 0)
                throw new Exception("Não encontrado nenhum Stage no XML, verifique o arquivo e envie novamente.");

            numeroCarga = cargaUnilever.ExOutputTab.Item.Tknum;

            string codigoTipoDeOperacao = cargaUnilever.ExOutputTab.Item.Shtyp;

            List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.ItemStage> listaStage = cargaUnilever.ExOutputTab.Item.Stage.ItemStage;

            for (int i = 0; i < listaStage.Count; i++)
            {
                List<string> numeroPedidos = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].Vbeln.ItemVbeln; //Pedidos
                List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.ItemNfe> notas = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].Nfe.ItemNfe;

                for (int j = 0; j < numeroPedidos.Count; j++)
                {
                    Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();
                    cargaIntegracao.NumeroCarga = cargaUnilever.ExOutputTab.Item.Tknum;
                    cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao
                    {
                        CodigoIntegracao = codigoTipoDeOperacao
                    };
                    cargaIntegracao.TipoCargaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador
                    {
                        CodigoIntegracao = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].LoadType
                    };
                    cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular
                    {
                        CodigoIntegracao = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].EquipType
                    };
                    cargaIntegracao.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa
                    {
                        CNPJ = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].Stcd1
                    };
                    cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                    Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                    produto.CodigoProduto = "1";
                    produto.DescricaoProduto = "DIVERSOS";
                    produto.CodigoGrupoProduto = "1";
                    produto.DescricaoGrupoProduto = "DIVERSOS";
                    cargaIntegracao.Produtos.Add(produto);

                    cargaIntegracao.NumeroPedidoEmbarcador = numeroPedidos[j];
                    List<Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.ItemNfe> notasPedidos = notas.Where(o => o.Vbeln == cargaIntegracao.NumeroPedidoEmbarcador).ToList();
                    if (notasPedidos.Count > 0)
                    {
                        cargaIntegracao.Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa
                        {
                            CPFCNPJ = notasPedidos[0].Stcd1Is,
                            CodigoAtividade = 3,
                            Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco
                            {
                                Bairro = notasPedidos[0].City2Is,
                                CEP = Utilidades.String.OnlyNumbers(notasPedidos[0].PostCode1Is),
                                Cidade = new Dominio.ObjetosDeValor.Localidade
                                {
                                    IBGE = notasPedidos[0].ZdestCitcodeIs
                                },
                                Logradouro = notasPedidos[0].StreetIs,
                                Numero = notasPedidos[0].HouseNum1Is,
                                Telefone = Utilidades.String.OnlyNumbers(notasPedidos[0].TelNumberIs)
                            },
                            RGIE = notasPedidos[0].Stcd3Is,
                            NomeFantasia = notasPedidos[0].Name1Is,
                            RazaoSocial = notasPedidos[0].Name1Is,
                            AtualizarEnderecoPessoa = true
                        };

                        cargaIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                        cargaIntegracao.PesoBruto = 0;
                        for (int n = 0; n < notasPedidos.Count; n++)
                        {
                            cargaIntegracao.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa
                            {
                                CPFCNPJ = notasPedidos[n].Stcd1,
                                CodigoIntegracao = cargaUnilever.ExOutputTab.Item.Stage.ItemStage[i].DestCustomer,
                                CodigoAtividade = 3,
                                Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco
                                {
                                    Bairro = notasPedidos[n].City2,
                                    CEP = Utilidades.String.OnlyNumbers(notasPedidos[n].PostCode1),
                                    Cidade = new Dominio.ObjetosDeValor.Localidade
                                    {
                                        IBGE = notasPedidos[n].ZdestCitcode
                                    },
                                    Logradouro = notasPedidos[n].Street,
                                    Numero = notasPedidos[n].HouseNum1,
                                    Telefone = Utilidades.String.OnlyNumbers(notasPedidos[n].TelNumber)
                                },
                                RGIE = notasPedidos[n].Stcd3,
                                NomeFantasia = notasPedidos[n].Name1,
                                RazaoSocial = notasPedidos[n].Name1,
                                AtualizarEnderecoPessoa = true
                            };

                            cargaIntegracao.PesoBruto += notasPedidos[n].Brgew;


                            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
                            notaFiscal.Serie = notasPedidos[n].Series;
                            notaFiscal.Chave = notasPedidos[n].Zfield;
                            notaFiscal.Numero = notasPedidos[n].Nfenum;
                            notaFiscal.Valor = notasPedidos[n].NfeTotal;
                            notaFiscal.PesoBruto = notasPedidos[n].Brgew;
                            notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;

                            DateTime.TryParseExact(notasPedidos[n].Docdat, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissao);
                            if (dataEmissao.Date > DateTime.MinValue)
                                notaFiscal.DataEmissao = dataEmissao.ToString("dd/MM/yyyy");

                            cargaIntegracao.NotasFiscais.Add(notaFiscal);
                        }
                    }
                    else
                    {
                        throw new Exception("Pedido " + cargaIntegracao.NumeroPedidoEmbarcador + " sem notas");
                    }

                    if (cargaIntegracao.Remetente != null)
                    {
                        cargaIntegracao.Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial
                        {
                            CodigoIntegracao = cargaIntegracao.Remetente.CPFCNPJ
                        };
                    }

                    listaCargaIntegracao.Add(cargaIntegracao);
                }

                if (codigoTipoDeOperacao == "ZA02")
                    break;
            }

            return listaCargaIntegracao;
        }

        private static bool ValidarIntegracaoAnterior(long codigoIntegracao, int codigoIntegradora, string jsonCargaAtual, string numeroIdentificador, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numeroIdentificador))
                    return true;

                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);

                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integradoraIntegracaoRetorno = repIntegradoraIntegracaoRetorno.BuscarUltimaPorIdentificador(numeroIdentificador, codigoIntegradora, codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);

                if (integradoraIntegracaoRetorno != null)
                {
                    string requestAnterior = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(integradoraIntegracaoRetorno.ArquivoRequisicao);
                    string numeroCargaAnterior = string.Empty;

                    string jsonCargaAnterior = Newtonsoft.Json.JsonConvert.SerializeObject(ConverterObjetoCargaIntegracao(requestAnterior, ref numeroCargaAnterior));

                    return jsonCargaAtual != jsonCargaAnterior;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return true;
            }
        }

        private void VerficarAgrupamentoStagesPendenteProcessamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                //List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stagesAgrupadas = repStageAgrupamento.BuscarPorCargaDt(6182);

                List<int> stagesAgrupadasProcessar = repStageAgrupamento.BuscarCodigosStageAgrupadaAProcessar(10);
                if (stagesAgrupadasProcessar != null && stagesAgrupadasProcessar.Count > 0)
                {
                    foreach (var codAgrupamento in stagesAgrupadasProcessar)
                        ProcessarStagesAgrupadasPendentes(codAgrupamento, tipoServicoMultisoftware, configuracaoTMS, unitOfWork);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerficarAgrupamentoStagesPendenteProcessamentoPorPrecheKin(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento AgrupamentosAguardandoGerarCargaPreChekin = repStageAgrupamento.BuscarPrimeiroStageAgrupadaAProcessarPorPreCheking();

                if (AgrupamentosAguardandoGerarCargaPreChekin != null)
                {
                    int cargaDt = AgrupamentosAguardandoGerarCargaPreChekin.CargaDT.Codigo;

                    List<int> stagesAgrupadasProcessar = repStageAgrupamento.BuscarCodigosStageAgrupadaAProcessarPorPreChekingPorCargaDT(cargaDt);
                    bool existeCargaJaGeradas = repStageAgrupamento.ExisteAgrupamentoDaCargaDTJaGerado(cargaDt);
                    ProcessarStagesAgrupadasPendentesPreChekin(stagesAgrupadasProcessar, cargaDt, existeCargaJaGeradas, tipoServicoMultisoftware, configuracaoTMS, unitOfWork);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }


        private static void ProcessarStagesAgrupadasPendentes(int codigoAgrupamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
            Servicos.Embarcador.Carga.Frete servicoFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork);

            Servicos.Embarcador.Carga.Carga servCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento = repStageAgrupamento.BuscarPorCodigoFetch(codigoAgrupamento);

            try
            {
                if (agrupamento != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                    auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;

                    List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesdoAgrupamento = repStage.BuscarporAgrupamento(agrupamento.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.Carga CargaDT = repCarga.BuscarPorCodigo(agrupamento.CargaDT.Codigo);

                    if (stagesdoAgrupamento == null || stagesdoAgrupamento.Count <= 0)
                    {
                        agrupamento.RetornoProcessamento = "Agrupamento sem stages";
                        agrupamento.Processado = false;
                        repStageAgrupamento.Atualizar(agrupamento);

                        unitOfWork.CommitChanges();
                        unitOfWork.FlushAndClear();
                        return;
                    }

                    Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = svcCarga.GerarCargaPorStageAgrupadaDT(CargaDT, agrupamento, stagesdoAgrupamento, configuracaoTMS, tipoServicoMultisoftware, unitOfWork, auditado);

                    if (carga != null)
                    {
                        carga = repCarga.BuscarPorCodigoFetch(carga.Codigo);

                        agrupamento.CargaGerada = carga;
                        agrupamento.Processado = false;
                        agrupamento.RetornoProcessamento = "";
                        repStageAgrupamento.Atualizar(agrupamento);

                        if (!CargaDT.StagesGeradas && !repStageAgrupamento.ExisteAgrupamentoDaCargaDTAGerar(CargaDT.Codigo))
                            CargaDT.StagesGeradas = true;

                        if (CargaDT.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn && CargaDT.SituacaoCarga == SituacaoCarga.AgTransportador)
                            CargaDT.SituacaoCarga = SituacaoCarga.AgNFe;
                        else if (CargaDT.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                        {
                            CargaDT.TipoFreteEscolhido = TipoFreteEscolhido.Embarcador;
                            CargaDT.CalculandoFrete = false;
                        }

                        repCarga.Atualizar(CargaDT);

                        bool dadosTransporteInformados = (
                            (carga.TipoDeCarga != null) &&
                            (carga.ModeloVeicularCarga != null) &&
                            (carga.Veiculo != null) &&
                            (!(carga.TipoOperacao?.ExigePlacaTracao ?? false) || ((carga.VeiculosVinculados?.Count ?? 0) == carga.ModeloVeicularCarga.NumeroReboques))
                        );

                        if (dadosTransporteInformados)
                        {
                            try
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte()
                                {
                                    Carga = carga,
                                    CodigoEmpresa = carga.Empresa?.Codigo ?? 0,
                                    CodigoModeloVeicular = carga.ModeloVeicularCarga?.Codigo ?? 0,
                                    CodigoReboque = (carga.VeiculosVinculados?.Count > 0) ? carga.VeiculosVinculados.ElementAt(0).Codigo : 0,
                                    CodigoSegundoReboque = (carga.VeiculosVinculados?.Count > 1) ? carga.VeiculosVinculados.ElementAt(1).Codigo : 0,
                                    CodigoTipoCarga = carga.TipoDeCarga?.Codigo ?? 0,
                                    CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                                    CodigoTracao = carga.Veiculo?.Codigo ?? 0,
                                    ObservacaoTransportador = carga.ObservacaoTransportador
                                };

                                if (agrupamento.Motorista != null)
                                    dadosTransporte.ListaCodigoMotorista.Add(agrupamento.Motorista.Codigo);

                                string mensagemErro = string.Empty;
                                svcCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, usuario: null, liberarComProblemaIntegracaoGrMotoristaVeiculo: false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, webServiceConsultaCTe: string.Empty, cliente: null, auditado, unitOfWork);
                            }
                            catch (ServicoException ex) { Servicos.Log.TratarErro(ex); }

                            if (CargaDT.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                                //essa carga ate entao nao serve pra nada. apenas para registro do valor de frete, vamos colocar ela pra frente pra nao atrapalhar (o frete e CTes ficam vinculados a mae).
                                carga.SituacaoCarga = SituacaoCarga.AgIntegracao;
                                carga.CalculandoFrete = false;
                                carga.PossuiPendencia = false;
                                carga.MotivoPendencia = "";
                                carga.ValorFrete = agrupamento.ValorFreteTotal;
                                carga.ValorFreteAPagar = agrupamento.ValorFreteTotal;
                                carga.TipoFreteEscolhido = TipoFreteEscolhido.Embarcador;

                                servicoFrete.CriarComponentesFreteCargaPorStageAgrupamento(agrupamento, carga);

                                //servCarga.CriarValoresFreteCargaFilhoConsolidacaoAutorizacaoEmissao(CargaDT, carga, unitOfWork);
                                servicoRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoTMS, false, unitOfWork, tipoServicoMultisoftware);

                                repCarga.Atualizar(carga);
                            }
                        }
                    }
                }

                repStage.AtualizarStageProcessadoPorAgrupamento(agrupamento.Codigo, false);

                unitOfWork.CommitChanges();

                unitOfWork.FlushAndClear();
            }
            catch (ServicoException se)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(se);

                agrupamento.RetornoProcessamento = se.Message;
                agrupamento.Processado = false;
                repStageAgrupamento.Atualizar(agrupamento);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();

                agrupamento.RetornoProcessamento = ex.Message;
                agrupamento.Processado = false;
                repStageAgrupamento.Atualizar(agrupamento);
            }
        }

        private static void ProcessarStagesAgrupadasPendentesPreChekin(List<int> codigosAgrupamentos, int codCargaDT, bool validarCargasJaGeradas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentos = repStageAgrupamento.BuscarPorCodigosFetch(codigosAgrupamentos);
            List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesdosAgrupamentos = repStage.BuscarporCodigosAgrupamentos(codigosAgrupamentos);

            if (agrupamentos?.Count() > 0)
            {
                List<int> AgrupamentoColetas = new List<int>();
                List<int> AgrupamentoTransferencia = new List<int>();
                List<int> AgrupamentoEntregas = new List<int>();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargapedidosColetas = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTransferencias = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEntregas = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCarga = repPedido.BuscarPorCarga(codCargaDT);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaDT = repCarga.BuscarPorCodigo(codCargaDT);

                foreach (var obj in agrupamentos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = repStage.BuscarporAgrupamento(obj.Codigo);
                    List<Vazio> tipoPercusoStages = stages.Select(s => s.TipoPercurso).ToList();

                    if (tipoPercusoStages.Any(s => s == Vazio.PercursoPreliminar))
                    {
                        AgrupamentoColetas.Add(obj.Codigo);
                        continue;
                    }

                    if (tipoPercusoStages.Any(s => s == Vazio.PercursoPrincipal))
                    {
                        AgrupamentoTransferencia.Add(obj.Codigo);
                        continue;
                    }

                    if (tipoPercusoStages.Any(s => s == Vazio.PercursoSubSeQuente))
                    {
                        AgrupamentoEntregas.Add(obj.Codigo);
                        continue;
                    }

                    AgrupamentoColetas.Add(obj.Codigo);
                }

                try
                {
                    unitOfWork.Start();

                    //iniciamos gerando as cargas de coletas. (cargas de coletas nao possuem cargas de trecho anterior)
                    foreach (var codagrupamento in AgrupamentoColetas)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento = repStageAgrupamento.BuscarPorCodigoFetch(codagrupamento);

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargapedidosGerados = GerarCargaPorAgrupamentoPreCheckin(codCargaDT, agrupamento, tipoServicoMultisoftware, configuracaoTMS, unitOfWork);

                        if (cargapedidosGerados.Count > 0)
                            cargapedidosColetas.AddRange(cargapedidosGerados);
                    }

                    //Agora gerando as cargas de Transferencia. (Cargas de Coletas devem ser refenciadas no carga pedido das cargas de transferencia como trecho anterior)
                    foreach (var codagrupamento in AgrupamentoTransferencia)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento = repStageAgrupamento.BuscarPorCodigoFetch(codagrupamento);

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargapedidosGerados = GerarCargaPorAgrupamentoPreCheckin(codCargaDT, agrupamento, tipoServicoMultisoftware, configuracaoTMS, unitOfWork);

                        if (cargapedidosGerados.Count > 0)
                            cargaPedidosTransferencias.AddRange(cargapedidosGerados);
                    }

                    //Por fim as cargas de Entrega. (Cargas de transferencia devem ser refenciadas no carga pedido das cargas de entregas como trecho anterior)
                    foreach (var codagrupamento in AgrupamentoEntregas)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento = repStageAgrupamento.BuscarPorCodigoFetch(codagrupamento);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargapedidosGerados = GerarCargaPorAgrupamentoPreCheckin(codCargaDT, agrupamento, tipoServicoMultisoftware, configuracaoTMS, unitOfWork);

                        if (cargapedidosGerados.Count > 0)
                            cargaPedidosEntregas.AddRange(cargapedidosGerados);
                    }

                    //os transportes podem ser gerados a qualquer momento, o usuario pode gerar todos de uma vez, ou pode apos gerar todos, gerar apenas mais um ou remover, precisamos garatir a correlação de todos os trechos
                    if (validarCargasJaGeradas)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentosBackup = repStageAgrupamento.BuscarPorCargaDt(codCargaDT);
                        List<int> codigoCargasColeta = new List<int>();
                        List<int> codigoCargasTransferencia = new List<int>();
                        List<int> codigoCargasEntrega = new List<int>();

                        foreach (var obj in agrupamentosBackup)
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = repStage.BuscarporAgrupamento(obj.Codigo);
                            List<Vazio> tipoPercusoStages = stages.Select(s => s.TipoPercurso).ToList();

                            if (tipoPercusoStages.Any(s => s == Vazio.PercursoPreliminar))
                            {
                                if (obj.CargaGerada != null)
                                {
                                    codigoCargasColeta.Add(obj.CargaGerada.Codigo);
                                    continue;
                                }
                            }

                            if (tipoPercusoStages.Any(s => s == Vazio.PercursoPrincipal))
                            {
                                if (obj.CargaGerada != null)
                                {
                                    codigoCargasTransferencia.Add(obj.CargaGerada.Codigo);
                                    continue;
                                }
                            }

                            if (tipoPercusoStages.Any(s => s == Vazio.PercursoSubSeQuente))
                            {
                                if (obj.CargaGerada != null)
                                {
                                    codigoCargasEntrega.Add(obj.CargaGerada.Codigo);
                                    continue;
                                }
                            }
                        }

                        cargapedidosColetas.AddRange(repCargaPedido.BuscarPorCargas(codigoCargasColeta));
                        cargaPedidosTransferencias.AddRange(repCargaPedido.BuscarPorCargas(codigoCargasTransferencia));
                        cargaPedidosEntregas.AddRange(repCargaPedido.BuscarPorCargas(codigoCargasEntrega));

                        cargapedidosColetas = cargapedidosColetas.Distinct().ToList();
                        cargaPedidosTransferencias = cargaPedidosTransferencias.Distinct().ToList();
                        cargaPedidosEntregas = cargaPedidosEntregas.Distinct().ToList();
                    }

                    VincularTrechosCargasdePrecheckin(pedidosCarga, cargapedidosColetas, cargaPedidosTransferencias, cargaPedidosEntregas, unitOfWork);

                    unitOfWork.CommitChanges();

                    unitOfWork.Flush();

                    //apos commitar vamos avancar as cargas geradas passando apenas os codigos pois o commit ja foi dado;
                    InformarDadosTransporteCargasPrEChekinGeradaEGerarIntegracoes(codCargaDT, codigosAgrupamentos, unitOfWork);

                }
                catch (ServicoException se)
                {
                    Servicos.Log.TratarErro(se);

                    unitOfWork.Rollback();

                    registrarProblemaGerarCargasAgrupamentoPreChekin(codigosAgrupamentos, "Problemas ao gerar transportes " + se.Message, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);

                    unitOfWork.Rollback();

                    registrarProblemaGerarCargasAgrupamentoPreChekin(codigosAgrupamentos, "Problemas ao gerar transportes", unitOfWork);
                }
            }
        }

        private static void registrarProblemaGerarCargasAgrupamentoPreChekin(List<int> codigosAgrupamentos, string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> agrupamentosProblemas = repStageAgrupamento.BuscarPorCodigosFetch(codigosAgrupamentos);

            foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento in agrupamentosProblemas)
            {
                agrupamento.RetornoProcessamento = msgErro;
                agrupamento.Processado = false;

                repStageAgrupamento.Atualizar(agrupamento);
            }
        }

        private static void VincularTrechosCargasdePrecheckin(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDT, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargapedidosColetas, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidosTransferencias, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidosEntregas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosDT)
            {
                //percorrendo os pedidos da DT vamos encontrar os trechos, o pedido deve estar sempre em apenas uma carga de coleta, ou uma carga de transf ou entrega

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoColeta = listaCargapedidosColetas.Where(x => x.Pedido.Codigo == pedido.Codigo).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTransf = listaCargaPedidosTransferencias.Where(x => x.Pedido.Codigo == pedido.Codigo).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoEntrega = listaCargaPedidosEntregas.Where(x => x.Pedido.Codigo == pedido.Codigo).FirstOrDefault();

                //aqui tratamos as coletas.
                if (cargaPedidoColeta != null)
                {
                    if (cargaPedidoTransf != null)
                    {
                        cargaPedidoTransf.CargaPedidoTrechoAnterior = cargaPedidoColeta;
                        cargaPedidoColeta.CargaPedidoProximoTrecho = cargaPedidoTransf;
                        repCargaPedido.Atualizar(cargaPedidoColeta);
                        repCargaPedido.Atualizar(cargaPedidoTransf);
                    }
                    else if (cargaPedidoEntrega != null)//caso nao tenha transf a entrega é automaticamente o proximo
                    {
                        cargaPedidoColeta.CargaPedidoProximoTrecho = cargaPedidoEntrega;
                        cargaPedidoEntrega.CargaPedidoTrechoAnterior = cargaPedidoColeta;

                        repCargaPedido.Atualizar(cargaPedidoColeta);
                        repCargaPedido.Atualizar(cargaPedidoEntrega);
                    }
                }

                //aqui tratamos as transferencias com entregas
                if (cargaPedidoTransf != null && cargaPedidoEntrega != null)
                {
                    cargaPedidoTransf.CargaPedidoProximoTrecho = cargaPedidoEntrega;
                    cargaPedidoEntrega.CargaPedidoTrechoAnterior = cargaPedidoTransf;

                    repCargaPedido.Atualizar(cargaPedidoTransf);
                    repCargaPedido.Atualizar(cargaPedidoEntrega);
                }
            }
        }

        private static void InformarDadosTransporteCargasPrEChekinGeradaEGerarIntegracoes(int codCargaDT, List<int> codigosAgrupamentos, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;

            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Cargas.Carga cargaDTvalidarIntegracao = repCarga.BuscarPorCodigoFetch(codCargaDT);

            //iniciamos gerando as cargas de coletas. (cargas de coletas nao possuem cargas de trecho anterior)
            foreach (var codagrupamento in codigosAgrupamentos)
            {
                Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento = repStageAgrupamento.BuscarPorCodigoFetch(codagrupamento);

                if (agrupamento.CargaGerada != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaGerada = repCarga.BuscarPorCodigoFetch(agrupamento.CargaGerada.Codigo);

                    InformarDadosTransporteCargaFilhoGerada(cargaGerada, agrupamento, svcCarga, auditado, unitOfWork);
                }

                unitOfWork.Flush();
            }

            if (cargaDTvalidarIntegracao != null && cargaDTvalidarIntegracao.TipoOperacao != null && cargaDTvalidarIntegracao.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn && (cargaDTvalidarIntegracao?.TipoOperacao?.ConfiguracaoCalculoFrete?.ExecutarPreCalculoFrete ?? false))
                Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaDadosTransporteIntegracao(cargaDTvalidarIntegracao, new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).BuscarPorTipo(TipoIntegracao.Unilever), unitOfWork, false, false);

            unitOfWork.CommitChanges();

            unitOfWork.FlushAndClear();

        }

        private static List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> GerarCargaPorAgrupamentoPreCheckin(int CodigocargaDT, Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;

            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.RecusaCargaCTe servicoRecusaCte = new Servicos.Embarcador.Carga.RecusaCargaCTe(unitOfWork, tipoServicoMultisoftware, configuracaoTMS, auditado);

            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE repCargaPedidoRecusa = new Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repositorioPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            Dominio.Entidades.Embarcador.Cargas.Carga cargaDT = repCarga.BuscarPorCodigoFetch(CodigocargaDT);

            List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesdoAgrupamento = repStage.BuscarporAgrupamento(agrupamento.Codigo);

            if (stagesdoAgrupamento == null || stagesdoAgrupamento.Count <= 0)
            {
                agrupamento.RetornoProcessamento = "Agrupamento sem stages";
                agrupamento.Processado = false;
                repStageAgrupamento.Atualizar(agrupamento);

                return cargaPedidos;
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> pedidosStage = repPedidoStage.BuscarPedidoStages(stagesdoAgrupamento.Select(x => x.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = pedidosStage.Select(obj => obj.Pedido).Distinct().ToList();

            bool cargaGeradaPorCteRecusa = false;
            Dominio.Entidades.Embarcador.Cargas.Carga cargaColeta = null;

            if (repCargaPedidoRecusa.ExisteRecusaPorPedido(pedidos.FirstOrDefault()?.Codigo ?? 0))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoRecusaCTE cargaPedidoRecusa = repCargaPedidoRecusa.BuscarRecusaPorPedido(pedidos.FirstOrDefault()?.Codigo ?? 0);

                //Recebemos novamente o pedido antes removido por recusa de CTE.. a carga ja esta gerada precisa gerar 
                cargaColeta = cargaPedidoRecusa.CargaRecusaGerada;
                //cargaColeta.CargaPossuiOutrosNumerosEmbarcador = true;
                cargaColeta.CodigoCargaEmbarcador = cargaDT.CodigoCargaEmbarcador;

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoExiste = repCargaPedido.BuscarPorCargaEPedido(cargaColeta.Codigo, cargaPedidoRecusa.Pedido.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoExisteDT = repCargaPedido.BuscarPorCargaEPedido(cargaDT.Codigo, cargaPedidoRecusa.Pedido.Codigo);
                servicoRecusaCte.CriarCargaCtePedidoRecusa(cargaPedidoRecusa.Pedido.Protocolo, cargaPedidoExiste, cargaPedidoExisteDT, cargaColeta, cargaDT);

                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = repDocumentoProvisao.BuscarPorCargaFilhoCancelada(cargaColeta.Codigo);
                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao in documentosProvisao)
                {
                    documentoProvisao.Situacao = SituacaoProvisaoDocumento.AgProvisao;
                    documentoProvisao.Carga = cargaDT;
                    repDocumentoProvisao.Atualizar(documentoProvisao);
                }

                //cargapeidos da carga mae devem ficar como emitidos.
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoMae = repCargaPedido.BuscarPorCargaEPedido(cargaDT.Codigo, cargaPedidoRecusa.Pedido.Codigo);
                if (cargaPedidoMae != null)
                {
                    cargaPedidoMae.CTesEmitidos = true;
                    repCargaPedido.Atualizar(cargaPedidoMae);
                }

                //integrar SD22 trava de edição
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao();
                tipoIntegracao = TipoIntegracao.Unilever;

                Dominio.Entidades.Embarcador.Pedidos.Stage stagePorPedido = repositorioPedidoStage.BuscarRelevantesCustoPorPedidoECargaPai(cargaPedidoExiste.Pedido.Codigo, cargaDT.Codigo);
                new Servicos.Embarcador.Carga.Carga(unitOfWork).CriarRegistroIntegracaoCargaFrete(cargaColeta, unitOfWork, tipoIntegracao, stagePorPedido, repositorioNotaFiscal.BuscarChavesNotasPorStage(stagePorPedido?.Codigo ?? 0));

                cargaGeradaPorCteRecusa = true;
            }
            else
                cargaColeta = svcCarga.GerarCargaPorStageAgrupadaDT(cargaDT, agrupamento, stagesdoAgrupamento, configuracaoTMS, tipoServicoMultisoftware, unitOfWork, auditado);

            if (cargaColeta != null)
            {
                cargaColeta = repCarga.BuscarPorCodigoFetch(cargaColeta.Codigo);

                agrupamento.CargaGerada = cargaColeta;
                agrupamento.Processado = false;
                agrupamento.RetornoProcessamento = "";
                repStageAgrupamento.Atualizar(agrupamento);

                if (!cargaDT.StagesGeradas && !repStageAgrupamento.ExisteAgrupamentoDaCargaDTAGerar(cargaDT.Codigo))
                    cargaDT.StagesGeradas = true;

                if (cargaDT.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn && cargaDT.SituacaoCarga == SituacaoCarga.AgTransportador)
                {
                    cargaDT.SituacaoCarga = SituacaoCarga.AgNFe;
                    cargaDT.DataEnvioUltimaNFe = null;
                }

                if (cargaGeradaPorCteRecusa)//coleta ja efetuada.
                {
                    cargaColeta.CargaFechada = true;
                    cargaColeta.SituacaoCarga = SituacaoCarga.AgIntegracao;
                    cargaColeta.AguardandoIntegracaoFrete = true;
                }
                else if (cargaColeta.SituacaoCarga == SituacaoCarga.Nova || cargaColeta.SituacaoCarga == SituacaoCarga.CalculoFrete)
                    cargaColeta.DataEnvioUltimaNFe = null;

                repCarga.Atualizar(cargaDT);
                repCarga.Atualizar(cargaColeta);

                string placas = " Placas: " + string.Join(", ", cargaColeta.VeiculosVinculados?.Select(o => o.Placa)) + " Tração: " + cargaColeta.Veiculo?.Placa ?? "";
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaColeta, null, "Carga gerada ao Confirmar transportes" + placas, unitOfWork);

                unitOfWork.Flush();

                cargaPedidos = repCargaPedido.BuscarPorCarga(cargaColeta.Codigo);
            }

            repStage.AtualizarStageProcessadoPorAgrupamento(agrupamento.Codigo, false);

            return cargaPedidos;
        }

        private static void InformarDadosTransporteCargaFilhoGerada(Dominio.Entidades.Embarcador.Cargas.Carga cargaGerada, Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento, Servicos.Embarcador.Carga.Carga svcCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            bool dadosTransporteInformados = (
                                (cargaGerada.TipoDeCarga != null) &&
                                (cargaGerada.ModeloVeicularCarga != null) &&
                                (cargaGerada.Veiculo != null) &&
                                (!(cargaGerada.TipoOperacao?.ExigePlacaTracao ?? false) || ((cargaGerada.VeiculosVinculados?.Count ?? 0) == cargaGerada.ModeloVeicularCarga.NumeroReboques))
                            );

            if (dadosTransporteInformados)
            {
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte()
                    {
                        Carga = cargaGerada,
                        CodigoEmpresa = cargaGerada.Empresa?.Codigo ?? 0,
                        CodigoModeloVeicular = cargaGerada.ModeloVeicularCarga?.Codigo ?? 0,
                        CodigoReboque = (cargaGerada.VeiculosVinculados?.Count > 0) ? cargaGerada.VeiculosVinculados.ElementAt(0).Codigo : 0,
                        CodigoSegundoReboque = (cargaGerada.VeiculosVinculados?.Count > 1) ? cargaGerada.VeiculosVinculados.ElementAt(1).Codigo : 0,
                        CodigoTipoCarga = cargaGerada.TipoDeCarga?.Codigo ?? 0,
                        CodigoTipoOperacao = cargaGerada.TipoOperacao?.Codigo ?? 0,
                        CodigoTracao = cargaGerada.Veiculo?.Codigo ?? 0,
                        ObservacaoTransportador = cargaGerada.ObservacaoTransportador
                    };

                    if (agrupamento.Motorista != null)
                        dadosTransporte.ListaCodigoMotorista.Add(agrupamento.Motorista.Codigo);

                    string mensagemErro = string.Empty;

                    svcCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, usuario: null, liberarComProblemaIntegracaoGrMotoristaVeiculo: false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, webServiceConsultaCTe: string.Empty, cliente: null, auditado, unitOfWork, true);
                }
                catch (ServicoException ex) { Servicos.Log.TratarErro(ex); } //enterra a excessao pois isso nao deve influenciar na geração da carga

            }
        }

        private static void VerificarLoteEscrituracaoMiroIntegrar(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoMiroIntegracao repositorioLoteEscrituracaoMiro = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoMiroIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao> integracaoesPendentes = repositorioLoteEscrituracaoMiro.BuscarIntegracoesPendentes(quantidadeRegistro: 15);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao integracaoPendente in integracaoesPendentes)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Escrituracao:
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarLoteEscrituracaoRetorno(integracaoPendente);
                        break;
                }
            }

        }

        private void EnviarEmailNaoConformidadeTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.Start();
                Servicos.Embarcador.NotaFiscal.NaoConformidade svcNaoConformidade = new Servicos.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);

                svcNaoConformidade.VerificarNaoConformidadesPendentesDeEnvio();

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        #endregion Métodos Privados

        #region Métodos Sobrescritos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro servicoTermoQuitacao = new Servicos.Embarcador.Financeiro.TermoQuitacaoFinanceiro(unitOfWork);
            Servicos.Embarcador.Financeiro.AvisoPeriodico servicoAvisoPeriodico = new Servicos.Embarcador.Financeiro.AvisoPeriodico(unitOfWork);

            VerificarIntegracoesAguardando(unitOfWork, _tipoServicoMultisoftware);
            VerificarLoteEscrituracaoMiroIntegrar(unitOfWork);
            VerficarAgrupamentoStagesPendenteProcessamento(unitOfWork, _tipoServicoMultisoftware);
            VerficarAgrupamentoStagesPendenteProcessamentoPorPrecheKin(unitOfWork, _tipoServicoMultisoftware);
            EnviarEmailNaoConformidadeTransportador(unitOfWork);
            new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unitOfWork).ProcessarIntegracoesPendentesDestinados();
            new Servicos.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork).ProcessarArquivoMovimentacaoContaPagar();
            new Servicos.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork).ProcessarMovimentoAguardando();
            servicoTermoQuitacao.VerificarTermosAGerar();
            servicoTermoQuitacao.EnviarEmailsPendenciaAprovacaoTransportadores(_clienteMultisoftware, unitOfWorkAdmin);
            servicoAvisoPeriodico.VerificarAvisosPeriodicosAGerar(_clienteMultisoftware, unitOfWorkAdmin);
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
        }

        #endregion Métodos Sobrescritos
    }
}