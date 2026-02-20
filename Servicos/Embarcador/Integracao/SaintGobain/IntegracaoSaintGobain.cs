using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Repositorio;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Servicos.Embarcador.Integracao.SaintGobain
{
    public sealed class IntegracaoSaintGobain : ServicoBase
    {

        #region Construtores

        public IntegracaoSaintGobain(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #endregion

        #region Métodos Privados

        private void BloquearCargaCancelamentoPeloPortal(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                return;

            Repositorio.Embarcador.Cargas.CargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> integracoesCarga = repositorioCargaIntegracao.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaIntegracao integracaoCarga in integracoesCarga)
            {
                integracaoCarga.BloquearCancelamentoCarga = true;
                repositorioCargaIntegracao.Atualizar(integracaoCarga);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.AndamentoPedido ConverterRetornoAndamentoPedido(Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Pedido retornoConsultaPedido)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.AndamentoPedido agendamentoPedido = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.AndamentoPedido();

            if (retornoConsultaPedido.dadosGeraisPedido == null)
                return new Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.AndamentoPedido();

            var detalhesPedido = retornoConsultaPedido.dadosGeraisPedido.FirstOrDefault();

            Dominio.Entidades.Embarcador.Filiais.Filial centro = !string.IsNullOrWhiteSpace(detalhesPedido.codCentro) ? repositorioFilial.buscarPorCodigoEmbarcador(detalhesPedido.codCentro) : null;

            agendamentoPedido.NumeroPedido = detalhesPedido.numOv;
            agendamentoPedido.CodigoCliente = detalhesPedido.codCliente;
            agendamentoPedido.NomeCliente = detalhesPedido.nomeCliente;
            agendamentoPedido.CNPJCliente = detalhesPedido.cnpjCliente;
            agendamentoPedido.CodigoCentro = detalhesPedido.codCentro;
            agendamentoPedido.DescricaoCentro = centro?.Descricao ?? string.Empty;
            agendamentoPedido.TipoCarregamento = detalhesPedido.tpCarregamento;
            agendamentoPedido.DescricaoTipoCarregamento = detalhesPedido.tpCarregamento == "01" ? "Cliente Retira" : "Entrega";

            agendamentoPedido.FluxoPedido = retornoConsultaPedido.fluxosStatusPedido;
            agendamentoPedido.ItensPendentes = retornoConsultaPedido.dadosQuantidadesPendentes;
            agendamentoPedido.LogsErros = retornoConsultaPedido.logsErros;

            agendamentoPedido.Remessas = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Remessa>();

            List<string> numerosRemessas = retornoConsultaPedido.fluxoRemessas != null && retornoConsultaPedido.fluxoRemessas.Count > 0 ? retornoConsultaPedido.fluxoRemessas.Select(o => o.numRemessa).Distinct().ToList() : new List<string>();
            if (numerosRemessas == null)
                numerosRemessas = new List<string>();

            foreach (var numeroRemessa in numerosRemessas)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Remessa remessa = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Remessa();

                remessa.FluxoRemessa = retornoConsultaPedido.fluxoRemessas.Where(o => o.numRemessa == numeroRemessa).ToList();
                remessa.DetalhesRemessa = retornoConsultaPedido.remessasVinculadasPedidoPesquisado.Count > 0 ? retornoConsultaPedido.remessasVinculadasPedidoPesquisado.Where(o => o.numRemessa == numeroRemessa).ToList() : new List<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RemessasVinculadasPedidoPesquisado>();

                remessa.NumeroRemessa = numeroRemessa;

                if (remessa.DetalhesRemessa.Count > 0)
                {
                    var detalheRemessas = remessa.DetalhesRemessa.FirstOrDefault();

                    remessa.NumeroNotaFiscal = detalheRemessas.nmNotaFiscal;
                    remessa.DataEmissao = detalheRemessas.dataRemessa;
                }

                agendamentoPedido.Remessas.Add(remessa);
            }

            return agendamentoPedido;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.Buscar();

            if (!(integracao?.PossuiIntegracaoSaintGobain ?? false))
                throw new ServicoException("Não foram configurados os dados de integração com a Saint-Gobain");

            Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao()
            {
                Senha = integracao.PasswordSaintGobain,
                Url = integracao.URLIntegracaoSaintGobain.ToLower(),
                Usuario = integracao.UserNameSaintGobain
            };

            //if (!configuracaoIntegracao.Url.EndsWith("/"))
            //    configuracaoIntegracao.Url += "/";

            //configuracaoIntegracao.Url += "WSImport.asmx";

            return configuracaoIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao ObterConfiguracaoIntegracaoConsultaPedido()
        {

            Repositorio.Embarcador.Configuracoes.IntegracaoSaintGobain repositorioIntegracaoSaintGobain = new Repositorio.Embarcador.Configuracoes.IntegracaoSaintGobain(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain integracaoSaintGobain = repositorioIntegracaoSaintGobain.Buscar();

            if (string.IsNullOrEmpty(integracaoSaintGobain?.UrlConsultaPedido) || string.IsNullOrEmpty(integracaoSaintGobain?.UrlConsultaUsuario))
                throw new ServicoException("Não foram configurados os dados de integração com a Saint-Gobain");

            Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao()
            {
                APIKey = integracaoSaintGobain.APIKey,
                ClientID = integracaoSaintGobain.ClientID,
                ClientSecret = integracaoSaintGobain.ClientSecret,
                UrlConsultaPedido = integracaoSaintGobain.UrlConsultaPedido,
                UrlValidaToken = integracaoSaintGobain.UrlValidaToken,
                UrlConsultaUsuario = integracaoSaintGobain.UrlConsultaUsuario,
            };

            //if (!configuracaoIntegracao.Url.EndsWith("/"))
            //    configuracaoIntegracao.Url += "/";

            //configuracaoIntegracao.Url += "WSImport.asmx";

            return configuracaoIntegracao;
        }

        private bool UtilizarConfiguracaoIntegracaoPIPO()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSaintGobain repositorioIntegracaoSaintGobain = new Repositorio.Embarcador.Configuracoes.IntegracaoSaintGobain(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain integracaoSaintGobain = repositorioIntegracaoSaintGobain.Buscar();

            return integracaoSaintGobain?.UtilizarEndPointPIPO ?? false;
        }

        private TimeSpan ObterTimeoutPadrao()
        {
            return new TimeSpan(hours: 0, minutes: 5, seconds: 0);
        }

        #region Servico Gateway
        private ServicoSaintGobain.ZSFH_TMS18_ALTERA_DT ObterDadosIntegracaoCarga(Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);

            ServicoSaintGobain.ZSFH_TMS18_ALTERA_DT dadosCarga = ObterDadosIntegracaoCarga(carga);

            dadosCarga.IT_ALTERA_DT[0].TDLNR = (repositorioEmpresa.BuscarPorCodigo(dadosTrans.CodigoEmpresa)?.CodigoIntegracao ?? "").PadLeft(10, '0');
            dadosCarga.IT_ALTERA_DT[0].EXTI1 = (repositorioVeiculo.BuscarPorCodigo(dadosTrans.CodigoTracao)?.Placa ?? "").Replace("-", "");
            dadosCarga.IT_ALTERA_DT[0].ZPARTNER_MOT = (repositorioUsuario.BuscarPorCodigo(dadosTrans.CodigoMotorista)?.CPF ?? "99999999999").PadLeft(11, '0');
            dadosCarga.IT_ALTERA_DT[0].NAME1_MOT = (repositorioUsuario.BuscarPorCodigo(dadosTrans.CodigoMotorista)?.Nome ?? "Atenção: Cadastrar Motorista no TMS");

            return dadosCarga;
        }

        private ServicoSaintGobain.ZSFH_TMS18_ALTERA_DT ObterDadosIntegracaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);
            DateTime? dataAgendamento = !(cargaJanelaCarregamento?.Excedente ?? true) ? cargaJanelaCarregamento.InicioCarregamento : carga.DataCarregamentoCarga;

            ServicoSaintGobain.ZSFH_SD_TMS_ALTERA_DT dadosAlteracaoCarga = new ServicoSaintGobain.ZSFH_SD_TMS_ALTERA_DT()
            {
                PROTOCOLOINTEGRACAOCARGA = carga.Protocolo.ToString(),
                TKNUM = carga.CodigoCargaEmbarcador,
                TDLNR = (carga.Empresa?.CodigoIntegracao ?? "").PadLeft(10, '0'),
                EXTI1 = (carga.Veiculo?.Placa ?? "").Replace("-", ""),
                ZPARTNER_MOT = (carga.Motoristas?.FirstOrDefault()?.CPF ?? "99999999999").PadLeft(11, '0'),
                NAME1_MOT = (carga.Motoristas?.FirstOrDefault()?.Nome ?? "Atenção: Cadastrar Motorista no TMS"),
                DATAAGENDAMENTO = dataAgendamento?.ToString("yyyy-MM-dd") ?? string.Empty,
                /**
                 * O campo Horaagendamento foi manualmente alterado direto do arquivo WSDL
                 * Caso o WSDL for atualizado, setar o type como char:6
                 */
                HORAAGENDAMENTO = dataAgendamento.HasValue ? dataAgendamento.Value.ToTimeString(true) : string.Empty
            };

            ServicoSaintGobain.ZSFH_TMS18_ALTERA_DT dadosCarga = new ServicoSaintGobain.ZSFH_TMS18_ALTERA_DT()
            {
                IT_ALTERA_DT = new ServicoSaintGobain.ZSFH_SD_TMS_ALTERA_DT[] { dadosAlteracaoCarga }
            };

            return dadosCarga;
        }

        private ServicoSaintGobain.ZSFH_TMS21_RECEBE_DADOS_NFSE ObterDadosIntegracaoLancamentoNFSManual(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioDocumentos = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto repositorioLancamentoNFSManualDesconto = new Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentos = repositorioDocumentos.BuscarPorLancamentoNFsManual(nfsManualCTeIntegracao.LancamentoNFSManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from o in documentos where o.PedidoXMLNotaFiscal != null && o.PedidoXMLNotaFiscal.CargaPedido != null select o.PedidoXMLNotaFiscal.CargaPedido).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDesconto = repositorioLancamentoNFSManualDesconto.BuscarCargasPedidoPorLancamentoNFSManual(nfsManualCTeIntegracao.LancamentoNFSManual.Codigo);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = nfsManualCTeIntegracao.LancamentoNFSManual.CTe;

            ServicoSaintGobain.ZSD_TMS_CABECALHO_NFSE cabecalhoNFS = new ServicoSaintGobain.ZSD_TMS_CABECALHO_NFSE()
            {
                C_CNPJ = cte.Empresa.CNPJ_SemFormato,
                C_CPF = "",
                C_IE = cte.Empresa.InscricaoEstadual,
                C_NOME = cte.Empresa.RazaoSocial.Left(60),
                C_STAT = "100",
                C_TP_NFS = nfsManualCTeIntegracao.LancamentoNFSManual.NFSResidual ? "1104" : "1601",
                C_UF = cte.Empresa.Localidade?.Estado?.Sigla ?? "",
                D_EMI = cte.DataEmissao?.ToString("yyyy-MM-dd") ?? "",
                D_SAI_ENT = cte.DataEmissao?.ToString("yyyy-MM-dd") ?? "",
                E_CNPJ = nfsManualCTeIntegracao.LancamentoNFSManual.Filial?.CNPJ ?? "",
                INF_AD_FISCO = cte.InformacaoAdicionalFisco.Left(250),
                NAT_OP = cte.NaturezaDaOperacao?.Descricao ?? "",
                N_NF = cte.Numero.ToString(),
                N_SERIE = cte.Serie?.Numero.ToString() ?? "",
                PES_FISIC = "",
                VER_PROC = "0000000",
                V_NF = Math.Round(cte.ValorPrestacaoServico, 2, MidpointRounding.AwayFromZero),
                V_PROD = Math.Round(cte.ValorAReceber, 2, MidpointRounding.AwayFromZero),
                V_RET_ISS = Math.Round(cte.ValorISSRetido, 2, MidpointRounding.AwayFromZero)
            };

            List<ServicoSaintGobain.ZSD_TMS_ITEM_NFSE> itensNFS = new List<ServicoSaintGobain.ZSD_TMS_ITEM_NFSE>();
            int numeroItem = 1;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosPorCargaPedido = (from o in documentos where o.PedidoXMLNotaFiscal?.CargaPedido?.Codigo == cargaPedido.Codigo select o).ToList();
                decimal valorFrete = documentosPorCargaPedido.Sum(o => o.ValorFrete);
                decimal valorRetencaoISS = documentosPorCargaPedido.Sum(o => o.ValorRetencaoISS);

                itensNFS.Add(new ServicoSaintGobain.ZSD_TMS_ITEM_NFSE()
                {
                    N_ITEM = numeroItem.ToString(),
                    N_PEDIDO = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    NUMERO_CARGA = cargaPedido.Carga.CodigoCargaEmbarcador,
                    PROT_INTEGRACAO_CARGA = cargaPedido.Carga.Protocolo.ToString(),
                    PROTOCOLO_INTEGRACAO_PEDIDO = cargaPedido.Pedido.Protocolo.ToString(),
                    V_NF_ITEM = Math.Round(valorFrete + valorRetencaoISS, 2, MidpointRounding.AwayFromZero),
                    V_RET_ISS_ITEM = Math.Round(valorRetencaoISS, 2, MidpointRounding.AwayFromZero)
                });

                numeroItem++;
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDesconto in cargaPedidosDesconto)
            {
                itensNFS.Add(new ServicoSaintGobain.ZSD_TMS_ITEM_NFSE()
                {
                    N_ITEM = numeroItem.ToString(),
                    N_PEDIDO = cargaPedidoDesconto.Pedido.NumeroPedidoEmbarcador,
                    NUMERO_CARGA = cargaPedidoDesconto.Carga.CodigoCargaEmbarcador,
                    PROT_INTEGRACAO_CARGA = cargaPedidoDesconto.Carga.Protocolo.ToString(),
                    PROTOCOLO_INTEGRACAO_PEDIDO = cargaPedidoDesconto.Pedido.Protocolo.ToString(),
                    V_NF_ITEM = Math.Round(cargaPedidoDesconto.Carga.ValorFreteResidual, 2, MidpointRounding.AwayFromZero),
                    V_RET_ISS_ITEM = 0m
                });

                numeroItem++;
            }

            ServicoSaintGobain.ZSFH_TMS21_RECEBE_DADOS_NFSE dadosLancamentoNFSManual = new ServicoSaintGobain.ZSFH_TMS21_RECEBE_DADOS_NFSE()
            {
                IS_CABECALHO = cabecalhoNFS,
                IT_ITEM = itensNFS.ToArray()
            };

            return dadosLancamentoNFSManual;
        }

        private ServicoSaintGobain.ZSFH_TMS19_BUSCA_FRETE ObterDadosIntegracaoValores(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCargaResumo(cargaIntegracao.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaValePedagio = repositorioCargaIntegracaoValePedagio.BuscarPorCarga(cargaIntegracao.Carga.Codigo, SituacaoIntegracao.Integrado);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio valePedagio = listaValePedagio.FirstOrDefault();
            List<ServicoSaintGobain.ZSD_TMS_FRETE> fretes = new List<ServicoSaintGobain.ZSD_TMS_FRETE>();

            List<(double CpfCnpjDestinatario, decimal ValorBaseFrete)> valoresBaseFretePorDestinatario = (
                from cargaPedido in cargaPedidos
                group cargaPedido by cargaPedido.Pedido.Destinatario.CPF_CNPJ
                into cargaPedidoPorDestinatario
                select ValueTuple.Create(cargaPedidoPorDestinatario.Key, cargaPedidoPorDestinatario.Sum(o => o.ValorBaseFrete))
            ).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                decimal valorBaseFrete = (from o in valoresBaseFretePorDestinatario where o.CpfCnpjDestinatario == cargaPedido.Pedido.Destinatario.CPF_CNPJ select o.ValorBaseFrete).FirstOrDefault();

                decimal valorMaiorMenorZDFD = cargaPedido.Carga.ObrigatorioInformarValorFreteOperador
                    ? Math.Round(cargaPedido.Carga.ValorFreteOperador - cargaPedido.Carga.ValorFreteTabelaFrete, 2, MidpointRounding.AwayFromZero)
                    : Math.Round(cargaPedido.Carga.ValorFreteResidual, 2, MidpointRounding.AwayFromZero);

                fretes.Add(new ServicoSaintGobain.ZSD_TMS_FRETE()
                {
                    FRETE_NF = Math.Round(cargaPedido.ValorFreteAPagar, 2, MidpointRounding.AwayFromZero),
                    ID_PEDAGIO = valePedagio?.NumeroValePedagio ?? "",
                    PEDAGIO_NF = Math.Round(valePedagio?.ValorValePedagio ?? 0m, 2, MidpointRounding.AwayFromZero),
                    PROTOCOLOINTEGRACAOCARGA = cargaPedido.Carga.Protocolo.ToString(),
                    PROTOCOLOINTEGRACAOPEDIDO = cargaPedido.Pedido.Protocolo.ToString(),
                    TPOPER = cargaPedido.Carga.TipoOperacao?.CodigoIntegracao ?? "",
                    VAL_MAIOR_MEN_ZDFD = valorMaiorMenorZDFD,
                    VAL_SOMA_S_ZDFD = Math.Round(valorBaseFrete, 2, MidpointRounding.AwayFromZero),
                    VAL_MAIOR_SOMA = Math.Round(cargaPedido.Carga.MaiorValorBaseFreteDosPedidos, 2, MidpointRounding.AwayFromZero)
                });
            }

            ServicoSaintGobain.ZSFH_TMS19_BUSCA_FRETE dadosValores = new ServicoSaintGobain.ZSFH_TMS19_BUSCA_FRETE()
            {
                IT_FRETE = fretes.ToArray()
            };

            return dadosValores;
        }

        private ServicoSaintGobain.ZSFH_TMS_SERVICOSClient ObterWebServiceClient(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            return ObterWebServiceClient(configuracaoIntegracao, timeout: ObterTimeoutPadrao());
        }

        private ServicoSaintGobain.ZSFH_TMS_SERVICOSClient ObterWebServiceClient(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao, TimeSpan timeout)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracaoIntegracao.Url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = timeout;
            binding.SendTimeout = timeout;

            if (configuracaoIntegracao.Url.StartsWith("https"))
            {
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
            }

            ServicoSaintGobain.ZSFH_TMS_SERVICOSClient wsSaintGobain = new ServicoSaintGobain.ZSFH_TMS_SERVICOSClient(binding, endpointAddress);

            wsSaintGobain.ClientCredentials.UserName.UserName = configuracaoIntegracao.Usuario;
            wsSaintGobain.ClientCredentials.UserName.Password = configuracaoIntegracao.Senha;

            return wsSaintGobain;
        }

        private ServicoSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO ObterDadosIntegracaoCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = carregamentoIntegracao.Carregamento;
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorCarregamento(carregamento.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas.FirstOrDefault();

            if (carga == null)
                return null;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Usuario motorista = repositorioCargaMotorista.BuscarPorCarga(carga.Codigo).FirstOrDefault()?.Motorista ?? null;
            List<ServicoSaintGobain.ZSD_TMS_AGENDAMENTO> objetoPedidos = new List<ServicoSaintGobain.ZSD_TMS_AGENDAMENTO>();
            string tipoAgendamento = carga.TipoOperacao != null ? Utilidades.String.Right("0" + carga.TipoOperacao.CodigoIntegracao, 2) : string.Empty; // Manter 0 a esquerda

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                ServicoSaintGobain.ZSD_TMS_AGENDAMENTO objetoPedido = new ServicoSaintGobain.ZSD_TMS_AGENDAMENTO()
                {
                    ACAO = carregamentoIntegracao.Status.ObterValorIntegracao(),
                    FILIAL = carga.Filial.CodigoFilialEmbarcador,
                    MODELO_VEICULAR = carga.ModeloVeicularCarga?.CodigoIntegracao,
                    CODIGO_MOTORISTA = motorista?.CodigoIntegracao,
                    NOME_MOTORISTA = motorista?.Nome.Left(35) ?? "Atenção: Cadastrar Motorista no TMS",
                    DOCUMENTO = motorista?.CPF ?? "99999999999",
                    NUMERO_CARGA = carga.CodigoCargaEmbarcador,
                    PROTOCOLO_INTEGRACAO_CARGA = carga.Protocolo.ToString(),
                    NUMERO_PEDIDO_EMBARCADOR = pedido.NumeroPedidoEmbarcador,
                    PROTOCOLO_INTEGRACAO_PEDIDO = pedido.Protocolo.ToString(),
                    TIPO_OPERACAO = carga.TipoOperacao?.CodigoIntegracao,
                    TRANSPORTADORA_EMITENTE = carregamento.Empresa?.CNPJ,
                    VEICULO = carga.Veiculo?.Placa,
                    DATA_AGENDAMENTO = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-dd") : string.Empty,
                    /**
                     * O campo HoraAgendamento foi manualmente alterado direto do arquivo WSDL
                     * Caso o WSDL for atualizado, setar o type como char6
                     */
                    HORA_AGENDAMENTO = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToTimeString(true) : string.Empty,
                    DOCA = "99",
                    TIPO_AGENDAMENTO = tipoAgendamento,
                    TIPO_RESERVA = "01",
                };

                List<ServicoSaintGobain.ZSD_TMS_ITEM_AGENDAMENTO> produtos = new List<ServicoSaintGobain.ZSD_TMS_ITEM_AGENDAMENTO>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaProdutos = repositorioCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

                for (int i = 0; i < cargaProdutos.Count; i++)
                {
                    produtos.Add(new ServicoSaintGobain.ZSD_TMS_ITEM_AGENDAMENTO()
                    {
                        N_ITEM = (i + 1).ToString(),
                        CODIGO_PRODUTO = cargaProdutos[i].Produto.CodigoProdutoEmbarcador,
                        QUANTIDADE = decimal.Round(cargaProdutos[i].Quantidade, 3, MidpointRounding.AwayFromZero)
                    });
                }

                objetoPedido.ITENS = produtos.ToArray();
                objetoPedidos.Add(objetoPedido);
            }

            ServicoSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO objetoRequisicao = new ServicoSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO()
            {
                IT_AGENDAMENTO = objetoPedidos.ToArray()
            };

            return objetoRequisicao;
        }

        private ServicoSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO ObterDadosIntegracaoCarregamentoObjetoTemporario(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = carregamentoIntegracao.Carregamento;
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorCarregamento(carregamento.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas.FirstOrDefault();
            List<ServicoSaintGobain.ZSD_TMS_AGENDAMENTO> objetoPedidos = new List<ServicoSaintGobain.ZSD_TMS_AGENDAMENTO>();
            string tipoAgendamento = dados.TipoOperacao != null ? Utilidades.String.Right("0" + dados.TipoOperacao.CodigoIntegracao, 2) : string.Empty; // Manter 0 a esquerda

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetiraPedido objPedido in dados.Pedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(objPedido.Codigo);

                ServicoSaintGobain.ZSD_TMS_AGENDAMENTO objetoPedido = new ServicoSaintGobain.ZSD_TMS_AGENDAMENTO()
                {
                    ACAO = carregamentoIntegracao.Status.ObterValorIntegracao(),
                    FILIAL = dados.Filial?.CodigoFilialEmbarcador,
                    MODELO_VEICULAR = dados.ModeloVeicularCarga?.CodigoIntegracao,
                    CODIGO_MOTORISTA = dados.Motorista?.CodigoIntegracao,
                    NOME_MOTORISTA = dados.Motorista?.Nome.Left(35) ?? "Atenção: Cadastrar Motorista no TMS",
                    DOCUMENTO = dados.Motorista?.CPF ?? "99999999999",
                    NUMERO_CARGA = carga.CodigoCargaEmbarcador,
                    PROTOCOLO_INTEGRACAO_CARGA = carga.Protocolo.ToString(),
                    TRANSPORTADORA_EMITENTE = dados?.Transportador?.CNPJ,
                    TIPO_OPERACAO = dados.TipoOperacao?.CodigoIntegracao,
                    VEICULO = dados.Veiculo?.Placa,
                    DATA_AGENDAMENTO = dados.DataCarregamentoCarga.ToString("yyyy-MM-dd"),
                    /**
                     * O campo HoraAgendamento foi manualmente alterado direto do arquivo WSDL
                     * Caso o WSDL for atualizado, setar o type como char6
                     */
                    HORA_AGENDAMENTO = dados.DataCarregamentoCarga.ToTimeString(true),
                    NUMERO_PEDIDO_EMBARCADOR = pedido.NumeroPedidoEmbarcador,
                    PROTOCOLO_INTEGRACAO_PEDIDO = pedido.Protocolo.ToString(),
                    DOCA = "99",
                    TIPO_AGENDAMENTO = tipoAgendamento,
                    TIPO_RESERVA = "01",
                };

                List<ServicoSaintGobain.ZSD_TMS_ITEM_AGENDAMENTO> produtos = new List<ServicoSaintGobain.ZSD_TMS_ITEM_AGENDAMENTO>();
                List<Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetiraPedidoProduto> cargaProdutos = objPedido.Produtos;

                for (int i = 0; i < cargaProdutos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = repositorioPedidoProduto.BuscarPorPedidoProduto(pedido.Codigo, cargaProdutos[i].Codigo);

                    produtos.Add(new ServicoSaintGobain.ZSD_TMS_ITEM_AGENDAMENTO()
                    {
                        N_ITEM = (i + 1).ToString(),
                        CODIGO_PRODUTO = pedidoProduto.Produto.CodigoProdutoEmbarcador,
                        QUANTIDADE = decimal.Round(cargaProdutos[i].Quantidade, 3, MidpointRounding.AwayFromZero)
                    });
                }

                objetoPedido.ITENS = produtos.ToArray();
                objetoPedidos.Add(objetoPedido);
            }

            ServicoSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO objetoRequisicao = new ServicoSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO()
            {
                IT_AGENDAMENTO = objetoPedidos.ToArray()
            };

            return objetoRequisicao;
        }

        private void IntegrarObjetoAgendamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, ServicoSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO dadosCarregamento, TimeSpan timeout)
        {
            InspectorBehavior inspector = new InspectorBehavior();
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicoSaintGobain.ZSFH_TMS_SERVICOSClient wsSaintGobain = ObterWebServiceClient(configuracaoIntegracao, timeout);

                wsSaintGobain.Endpoint.EndpointBehaviors.Add(inspector);
                ServicoSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTOResponse retornoRequisicao = wsSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO(dadosCarregamento);

                if (retornoRequisicao.ET_RETORNO.Length > 0)
                {
                    carregamentoIntegracao.ProblemaIntegracao = retornoRequisicao.ET_RETORNO[0].ZMESSAGE;
                    carregamentoIntegracao.SituacaoIntegracao = (retornoRequisicao.ET_RETORNO[0].ZTYPE_MESSAGE == "S") ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    carregamentoIntegracao.ProblemaIntegracao = "O WS Saint-Gobain não respondeu a solicitação.";
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(carregamentoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

                if (carregamentoIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    BloquearCargaCancelamentoPeloPortal(carregamentoIntegracao.Carregamento.CargasFrete.FirstOrDefault());
            }
            catch (ServicoException excecao)
            {
                carregamentoIntegracao.ProblemaIntegracao = excecao.Message;
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                carregamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a Saint-Gobain.";
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(carregamentoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }
        }

        #endregion

        #region Servico PI/PO
        private ServicoSaintGobainPIPO.ZSFH_TMS18_ALTERA_DTRequest ObterDadosIntegracaoCargaPIPO(Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);

            ServicoSaintGobainPIPO.ZSFH_TMS18_ALTERA_DTRequest dadosCarga = ObterDadosIntegracaoCargaPIPO(carga);

            dadosCarga.IT_ALTERA_DT[0].TDLNR = (repositorioEmpresa.BuscarPorCodigo(dadosTrans.CodigoEmpresa)?.CodigoIntegracao ?? "").PadLeft(10, '0');
            dadosCarga.IT_ALTERA_DT[0].EXTI1 = (repositorioVeiculo.BuscarPorCodigo(dadosTrans.CodigoTracao)?.Placa ?? "").Replace("-", "");
            dadosCarga.IT_ALTERA_DT[0].ZPARTNER_MOT = (repositorioUsuario.BuscarPorCodigo(dadosTrans.CodigoMotorista)?.CPF ?? "99999999999").PadLeft(11, '0');
            dadosCarga.IT_ALTERA_DT[0].NAME1_MOT = (repositorioUsuario.BuscarPorCodigo(dadosTrans.CodigoMotorista)?.Nome ?? "Atenção: Cadastrar Motorista no TMS");

            return dadosCarga;
        }

        private ServicoSaintGobainPIPO.ZSFH_TMS18_ALTERA_DTRequest ObterDadosIntegracaoCargaPIPO(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);
            DateTime? dataAgendamento = !(cargaJanelaCarregamento?.Excedente ?? true) ? cargaJanelaCarregamento.InicioCarregamento : (DateTime?)null;

            ServicoSaintGobainPIPO.ZSD_TMS_ALTERA_DT dadosAlteracaoCarga = new ServicoSaintGobainPIPO.ZSD_TMS_ALTERA_DT()
            {
                PROTOCOLOINTEGRACAOCARGA = carga.Protocolo.ToString(),
                TKNUM = carga.CodigoCargaEmbarcador,
                TDLNR = (carga.Empresa?.CodigoIntegracao ?? "").PadLeft(10, '0'),
                EXTI1 = (carga.Veiculo?.Placa ?? "").Replace("-", ""),
                ZPARTNER_MOT = (carga.Motoristas?.FirstOrDefault()?.CPF ?? "99999999999").PadLeft(11, '0'),
                NAME1_MOT = (carga.Motoristas?.FirstOrDefault()?.Nome ?? "Atenção: Cadastrar Motorista no TMS"),
                DATAAGENDAMENTO = dataAgendamento?.ToString("yyyy-MM-dd") ?? string.Empty,
                /**
                 * O campo Horaagendamento foi manualmente alterado direto do arquivo WSDL
                 * Caso o WSDL for atualizado, setar o type como char:6
                 */
                HORAAGENDAMENTO = dataAgendamento.HasValue ? dataAgendamento.Value.ToTimeString(true) : string.Empty
            };

            ServicoSaintGobainPIPO.ZSFH_TMS18_ALTERA_DTRequest dadosCarga = new ServicoSaintGobainPIPO.ZSFH_TMS18_ALTERA_DTRequest()
            {
                IT_ALTERA_DT = new ServicoSaintGobainPIPO.ZSD_TMS_ALTERA_DT[] { dadosAlteracaoCarga }
            };

            return dadosCarga;
        }

        private ServicoSaintGobainPIPO.ZSFH_TMS21_RECEBE_DADOS_NFSERequest ObterDadosIntegracaoLancamentoNFSManualPIPO(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioDocumentos = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto repositorioLancamentoNFSManualDesconto = new Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentos = repositorioDocumentos.BuscarPorLancamentoNFsManual(nfsManualCTeIntegracao.LancamentoNFSManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from o in documentos where o.PedidoXMLNotaFiscal != null && o.PedidoXMLNotaFiscal.CargaPedido != null select o.PedidoXMLNotaFiscal.CargaPedido).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDesconto = repositorioLancamentoNFSManualDesconto.BuscarCargasPedidoPorLancamentoNFSManual(nfsManualCTeIntegracao.LancamentoNFSManual.Codigo);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = nfsManualCTeIntegracao.LancamentoNFSManual.CTe;

            ServicoSaintGobainPIPO.ZSD_TMS_CABECALHO_NFSE cabecalhoNFS = new ServicoSaintGobainPIPO.ZSD_TMS_CABECALHO_NFSE()
            {
                C_CNPJ = cte.Empresa.CNPJ_SemFormato,
                C_CPF = "",
                C_IE = cte.Empresa.InscricaoEstadual,
                C_NOME = cte.Empresa.RazaoSocial.Left(60),
                C_STAT = "100",
                C_TP_NFS = nfsManualCTeIntegracao.LancamentoNFSManual.NFSResidual ? "1104" : "1601",
                C_UF = cte.Empresa.Localidade?.Estado?.Sigla ?? "",
                D_EMI = cte.DataEmissao?.ToString("yyyy-MM-dd") ?? "",
                D_SAI_ENT = cte.DataEmissao?.ToString("yyyy-MM-dd") ?? "",
                E_CNPJ = nfsManualCTeIntegracao.LancamentoNFSManual.Filial?.CNPJ ?? "",
                INF_AD_FISCO = cte.InformacaoAdicionalFisco.Left(250),
                NAT_OP = cte.NaturezaDaOperacao?.Descricao ?? "",
                N_NF = cte.Numero.ToString(),
                N_SERIE = cte.Serie?.Numero.ToString() ?? "",
                PES_FISIC = "",
                VER_PROC = "0000000",
                V_NF = Math.Round(cte.ValorPrestacaoServico, 2, MidpointRounding.AwayFromZero),
                V_PROD = Math.Round(cte.ValorAReceber, 2, MidpointRounding.AwayFromZero),
                V_RET_ISS = Math.Round(cte.ValorISSRetido, 2, MidpointRounding.AwayFromZero),
                V_NFSpecified = true,
                V_PRODSpecified = true,
                V_RET_ISSSpecified = true
            };

            List<ServicoSaintGobainPIPO.ZSD_TMS_ITEM_NFSE> itensNFS = new List<ServicoSaintGobainPIPO.ZSD_TMS_ITEM_NFSE>();
            int numeroItem = 1;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosPorCargaPedido = (from o in documentos where o.PedidoXMLNotaFiscal?.CargaPedido?.Codigo == cargaPedido.Codigo select o).ToList();
                decimal valorFrete = documentosPorCargaPedido.Sum(o => o.ValorFrete);
                decimal valorRetencaoISS = documentosPorCargaPedido.Sum(o => o.ValorRetencaoISS);

                itensNFS.Add(new ServicoSaintGobainPIPO.ZSD_TMS_ITEM_NFSE()
                {
                    N_ITEM = numeroItem.ToString(),
                    N_PEDIDO = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    NUMERO_CARGA = cargaPedido.Carga.CodigoCargaEmbarcador,
                    PROT_INTEGRACAO_CARGA = cargaPedido.Carga.Protocolo.ToString(),
                    PROTOCOLO_INTEGRACAO_PEDIDO = cargaPedido.Pedido.Protocolo.ToString(),
                    V_NF_ITEM = Math.Round(valorFrete + valorRetencaoISS, 2, MidpointRounding.AwayFromZero),
                    V_RET_ISS_ITEM = Math.Round(valorRetencaoISS, 2, MidpointRounding.AwayFromZero),
                    V_NF_ITEMSpecified = true,
                    V_RET_ISS_ITEMSpecified = true,
                });

                numeroItem++;
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDesconto in cargaPedidosDesconto)
            {
                itensNFS.Add(new ServicoSaintGobainPIPO.ZSD_TMS_ITEM_NFSE()
                {
                    N_ITEM = numeroItem.ToString(),
                    N_PEDIDO = cargaPedidoDesconto.Pedido.NumeroPedidoEmbarcador,
                    NUMERO_CARGA = cargaPedidoDesconto.Carga.CodigoCargaEmbarcador,
                    PROT_INTEGRACAO_CARGA = cargaPedidoDesconto.Carga.Protocolo.ToString(),
                    PROTOCOLO_INTEGRACAO_PEDIDO = cargaPedidoDesconto.Pedido.Protocolo.ToString(),
                    V_NF_ITEM = Math.Round(cargaPedidoDesconto.Carga.ValorFreteResidual, 2, MidpointRounding.AwayFromZero),
                    V_RET_ISS_ITEM = 0m,
                    V_NF_ITEMSpecified = true,
                    V_RET_ISS_ITEMSpecified = true
                });

                numeroItem++;
            }

            ServicoSaintGobainPIPO.ZSFH_TMS21_RECEBE_DADOS_NFSERequest dadosLancamentoNFSManual = new ServicoSaintGobainPIPO.ZSFH_TMS21_RECEBE_DADOS_NFSERequest()
            {
                IS_CABECALHO = cabecalhoNFS,
                IT_ITEM = itensNFS.ToArray()
            };

            return dadosLancamentoNFSManual;
        }

        private ServicoSaintGobainPIPO.ZSFH_TMS19_BUSCA_FRETERequest ObterDadosIntegracaoValoresPIPO(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCargaResumo(cargaIntegracao.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaValePedagio = repositorioCargaIntegracaoValePedagio.BuscarPorCarga(cargaIntegracao.Carga.Codigo, SituacaoIntegracao.Integrado);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio valePedagio = listaValePedagio.FirstOrDefault();
            List<ServicoSaintGobainPIPO.ZSD_TMS_FRETE> fretes = new List<ServicoSaintGobainPIPO.ZSD_TMS_FRETE>();

            List<(double CpfCnpjDestinatario, decimal ValorBaseFrete)> valoresBaseFretePorDestinatario = (
                from cargaPedido in cargaPedidos
                group cargaPedido by cargaPedido.Pedido.Destinatario.CPF_CNPJ
                into cargaPedidoPorDestinatario
                select ValueTuple.Create(cargaPedidoPorDestinatario.Key, cargaPedidoPorDestinatario.Sum(o => o.ValorBaseFrete))
            ).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                decimal valorBaseFrete = (from o in valoresBaseFretePorDestinatario where o.CpfCnpjDestinatario == cargaPedido.Pedido.Destinatario.CPF_CNPJ select o.ValorBaseFrete).FirstOrDefault();

                fretes.Add(new ServicoSaintGobainPIPO.ZSD_TMS_FRETE()
                {
                    FRETE_NF = Math.Round(cargaPedido.ValorFreteAPagar, 2, MidpointRounding.AwayFromZero),
                    ID_PEDAGIO = valePedagio?.NumeroValePedagio ?? "",
                    PEDAGIO_NF = Math.Round(valePedagio?.ValorValePedagio ?? 0m, 2, MidpointRounding.AwayFromZero),
                    PROTOCOLOINTEGRACAOCARGA = cargaPedido.Carga.Protocolo.ToString(),
                    PROTOCOLOINTEGRACAOPEDIDO = cargaPedido.Pedido.Protocolo.ToString(),
                    TPOPER = cargaPedido.Carga.TipoOperacao?.CodigoIntegracao ?? "",
                    VAL_MAIOR_MEN_ZDFD = Math.Round(cargaPedido.Carga.ValorFreteResidual, 2, MidpointRounding.AwayFromZero),
                    VAL_SOMA_S_ZDFD = Math.Round(valorBaseFrete, 2, MidpointRounding.AwayFromZero),
                    VAL_MAIOR_SOMA = Math.Round(cargaPedido.Carga.MaiorValorBaseFreteDosPedidos, 2, MidpointRounding.AwayFromZero),
                    FRETE_NFSpecified = true,
                    PEDAGIO_NFSpecified = true,
                    VAL_MAIOR_MEN_ZDFDSpecified = true,
                    VAL_MAIOR_SOMASpecified = true,
                    VAL_SOMA_S_ZDFDSpecified = true
                });
            }

            ServicoSaintGobainPIPO.ZSFH_TMS19_BUSCA_FRETERequest dadosValores = new ServicoSaintGobainPIPO.ZSFH_TMS19_BUSCA_FRETERequest()
            {
                IT_FRETE = fretes.ToArray()
            };

            return dadosValores;
        }

        private ServicoSaintGobainPIPO.SI_conjuntoFuncoesTMS_sync_OutClient ObterWebServiceClientPIPO(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            return ObterWebServiceClientPIPO(configuracaoIntegracao, timeout: ObterTimeoutPadrao());
        }

        private ServicoSaintGobainPIPO.SI_conjuntoFuncoesTMS_sync_OutClient ObterWebServiceClientPIPO(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao, TimeSpan timeout)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracaoIntegracao.Url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = timeout;
            binding.SendTimeout = timeout;

            if (configuracaoIntegracao.Url.StartsWith("https"))
            {
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
            }

            ServicoSaintGobainPIPO.SI_conjuntoFuncoesTMS_sync_OutClient wsSaintGobain = new ServicoSaintGobainPIPO.SI_conjuntoFuncoesTMS_sync_OutClient(binding, endpointAddress);

            wsSaintGobain.ClientCredentials.UserName.UserName = configuracaoIntegracao.Usuario;
            wsSaintGobain.ClientCredentials.UserName.Password = configuracaoIntegracao.Senha;

            return wsSaintGobain;
        }

        private ServicoSaintGobainPIPO.ZSFH_TMS27_RECEBE_DADOS_AGTORequest ObterDadosIntegracaoCarregamentoPIPO(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = carregamentoIntegracao.Carregamento;
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorCarregamento(carregamento.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas.FirstOrDefault();

            if (carga == null)
                return null;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Usuario motorista = repositorioCargaMotorista.BuscarPorCarga(carga.Codigo).FirstOrDefault()?.Motorista ?? null;
            List<ServicoSaintGobainPIPO.ZSD_TMS_AGENDAMENTO> objetoPedidos = new List<ServicoSaintGobainPIPO.ZSD_TMS_AGENDAMENTO>();
            string tipoAgendamento = carga.TipoOperacao != null ? Utilidades.String.Right("0" + carga.TipoOperacao.CodigoIntegracao, 2) : string.Empty; // Manter 0 a esquerda

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                ServicoSaintGobainPIPO.ZSD_TMS_AGENDAMENTO objetoPedido = new ServicoSaintGobainPIPO.ZSD_TMS_AGENDAMENTO()
                {
                    ACAO = carregamentoIntegracao.Status.ObterValorIntegracao(),
                    FILIAL = carga.Filial.CodigoFilialEmbarcador,
                    MODELO_VEICULAR = carga.ModeloVeicularCarga?.CodigoIntegracao,
                    CODIGO_MOTORISTA = motorista?.CodigoIntegracao,
                    NOME_MOTORISTA = motorista?.Nome.Left(35) ?? "Atenção: Cadastrar Motorista no TMS",
                    DOCUMENTO = motorista?.CPF ?? "99999999999",
                    NUMERO_CARGA = carga.CodigoCargaEmbarcador,
                    PROTOCOLO_INTEGRACAO_CARGA = carga.Protocolo.ToString(),
                    NUMERO_PEDIDO_EMBARCADOR = pedido.NumeroPedidoEmbarcador,
                    PROTOCOLO_INTEGRACAO_PEDIDO = pedido.Protocolo.ToString(),
                    TIPO_OPERACAO = carga.TipoOperacao?.CodigoIntegracao,
                    TRANSPORTADORA_EMITENTE = carregamento.Empresa?.CNPJ,
                    VEICULO = carga.Veiculo?.Placa,
                    DATA_AGENDAMENTO = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToString("yyyy-MM-dd") : string.Empty,
                    /**
                     * O campo HoraAgendamento foi manualmente alterado direto do arquivo WSDL
                     * Caso o WSDL for atualizado, setar o type como char6
                     */
                    HORA_AGENDAMENTO = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToTimeString(true) : string.Empty,
                    DOCA = "99",
                    TIPO_AGENDAMENTO = tipoAgendamento,
                    TIPO_RESERVA = "01",
                };

                List<ServicoSaintGobainPIPO.ZSD_TMS_ITEM_AGENDAMENTO> produtos = new List<ServicoSaintGobainPIPO.ZSD_TMS_ITEM_AGENDAMENTO>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaProdutos = repositorioCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

                for (int i = 0; i < cargaProdutos.Count; i++)
                {
                    produtos.Add(new ServicoSaintGobainPIPO.ZSD_TMS_ITEM_AGENDAMENTO()
                    {
                        N_ITEM = (i + 1).ToString(),
                        CODIGO_PRODUTO = cargaProdutos[i].Produto.CodigoProdutoEmbarcador,
                        QUANTIDADE = decimal.Round(cargaProdutos[i].Quantidade, 3, MidpointRounding.AwayFromZero),
                        QUANTIDADESpecified = true
                    });
                }

                objetoPedido.ITENS = produtos.ToArray();
                objetoPedidos.Add(objetoPedido);
            }

            ServicoSaintGobainPIPO.ZSFH_TMS27_RECEBE_DADOS_AGTORequest objetoRequisicao = new ServicoSaintGobainPIPO.ZSFH_TMS27_RECEBE_DADOS_AGTORequest()
            {
                IT_AGENDAMENTO = objetoPedidos.ToArray()
            };

            return objetoRequisicao;
        }

        private ServicoSaintGobainPIPO.ZSFH_TMS27_RECEBE_DADOS_AGTORequest ObterDadosIntegracaoCarregamentoObjetoTemporarioPIPO(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = carregamentoIntegracao.Carregamento;
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasPorCarregamento(carregamento.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas.FirstOrDefault();
            List<ServicoSaintGobainPIPO.ZSD_TMS_AGENDAMENTO> objetoPedidos = new List<ServicoSaintGobainPIPO.ZSD_TMS_AGENDAMENTO>();
            string tipoAgendamento = dados.TipoOperacao != null ? Utilidades.String.Right("0" + dados.TipoOperacao.CodigoIntegracao, 2) : string.Empty; // Manter 0 a esquerda

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetiraPedido objPedido in dados.Pedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(objPedido.Codigo);

                ServicoSaintGobainPIPO.ZSD_TMS_AGENDAMENTO objetoPedido = new ServicoSaintGobainPIPO.ZSD_TMS_AGENDAMENTO()
                {
                    ACAO = carregamentoIntegracao.Status.ObterValorIntegracao(),
                    FILIAL = dados.Filial?.CodigoFilialEmbarcador,
                    MODELO_VEICULAR = dados.ModeloVeicularCarga?.CodigoIntegracao,
                    CODIGO_MOTORISTA = dados.Motorista?.CodigoIntegracao,
                    NOME_MOTORISTA = dados.Motorista?.Nome.Left(35) ?? "Atenção: Cadastrar Motorista no TMS",
                    DOCUMENTO = dados.Motorista?.CPF ?? "99999999999",
                    NUMERO_CARGA = carga.CodigoCargaEmbarcador,
                    PROTOCOLO_INTEGRACAO_CARGA = carga.Protocolo.ToString(),
                    TRANSPORTADORA_EMITENTE = dados?.Transportador?.CNPJ,
                    TIPO_OPERACAO = dados.TipoOperacao?.CodigoIntegracao,
                    VEICULO = dados.Veiculo?.Placa,
                    DATA_AGENDAMENTO = dados.DataCarregamentoCarga.ToString("yyyy-MM-dd"),
                    /**
                     * O campo HoraAgendamento foi manualmente alterado direto do arquivo WSDL
                     * Caso o WSDL for atualizado, setar o type como char6
                     */
                    HORA_AGENDAMENTO = dados.DataCarregamentoCarga.ToTimeString(true),
                    NUMERO_PEDIDO_EMBARCADOR = pedido.NumeroPedidoEmbarcador,
                    PROTOCOLO_INTEGRACAO_PEDIDO = pedido.Protocolo.ToString(),
                    DOCA = "99",
                    TIPO_AGENDAMENTO = tipoAgendamento,
                    TIPO_RESERVA = "01",
                };

                List<ServicoSaintGobainPIPO.ZSD_TMS_ITEM_AGENDAMENTO> produtos = new List<ServicoSaintGobainPIPO.ZSD_TMS_ITEM_AGENDAMENTO>();
                List<Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetiraPedidoProduto> cargaProdutos = objPedido.Produtos;

                for (int i = 0; i < cargaProdutos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = repositorioPedidoProduto.BuscarPorPedidoProduto(pedido.Codigo, cargaProdutos[i].Codigo);

                    produtos.Add(new ServicoSaintGobainPIPO.ZSD_TMS_ITEM_AGENDAMENTO()
                    {
                        N_ITEM = (i + 1).ToString(),
                        CODIGO_PRODUTO = pedidoProduto.Produto.CodigoProdutoEmbarcador,
                        QUANTIDADE = decimal.Round(cargaProdutos[i].Quantidade, 3, MidpointRounding.AwayFromZero),
                        QUANTIDADESpecified = true
                    });
                }

                objetoPedido.ITENS = produtos.ToArray();
                objetoPedidos.Add(objetoPedido);
            }

            ServicoSaintGobainPIPO.ZSFH_TMS27_RECEBE_DADOS_AGTORequest objetoRequisicao = new ServicoSaintGobainPIPO.ZSFH_TMS27_RECEBE_DADOS_AGTORequest()
            {
                IT_AGENDAMENTO = objetoPedidos.ToArray()
            };

            return objetoRequisicao;
        }

        private void IntegrarObjetoAgendamentoPIPO(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, ServicoSaintGobainPIPO.ZSFH_TMS27_RECEBE_DADOS_AGTORequest dadosCarregamento, TimeSpan timeout)
        {
            InspectorBehavior inspector = new InspectorBehavior();
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicoSaintGobainPIPO.SI_conjuntoFuncoesTMS_sync_OutClient wsSaintGobain = ObterWebServiceClientPIPO(configuracaoIntegracao, timeout);

                wsSaintGobain.Endpoint.EndpointBehaviors.Add(inspector);
                ServicoSaintGobainPIPO.ZSD_TMS_RETORNO_WP27[] retornoRequisicao = wsSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO(dadosCarregamento.IT_AGENDAMENTO);

                if (retornoRequisicao.Length > 0)
                {
                    carregamentoIntegracao.ProblemaIntegracao = retornoRequisicao[0].ZMESSAGE;
                    carregamentoIntegracao.SituacaoIntegracao = (retornoRequisicao[0].ZTYPE_MESSAGE == "S") ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    carregamentoIntegracao.ProblemaIntegracao = "O WS Saint-Gobain não respondeu a solicitação.";
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(carregamentoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

                if (carregamentoIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    BloquearCargaCancelamentoPeloPortal(carregamentoIntegracao.Carregamento.CargasFrete.FirstOrDefault());
            }
            catch (ServicoException excecao)
            {
                carregamentoIntegracao.ProblemaIntegracao = excecao.Message;
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                carregamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a Saint-Gobain.";
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(carregamentoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }
        }

        #endregion

        private string ObterTokenConsultaPedido(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao)
        {
            string UrlToken = configuracaoIntegracao.UrlValidaToken;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient(UrlToken);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", configuracaoIntegracao.ClientID);
            request.AddParameter("client_secret", configuracaoIntegracao.ClientSecret);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", "openid");

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);
                return retorno.access_token;
            }

            return "";

        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain ObterConfiguracaoIntegracaoPadrao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSaintGobain repositorioSaintGobain = new Repositorio.Embarcador.Configuracoes.IntegracaoSaintGobain(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain configuracaoSaintGobain = repositorioSaintGobain.BuscarPrimeiroRegistro();

            if (configuracaoSaintGobain == null)
                throw new Exception("Não foi possível obter a configuração de integração com a Saint-Gobain.");


            if (string.IsNullOrEmpty(configuracaoSaintGobain.UsuariosSnowFlake) || string.IsNullOrEmpty(configuracaoSaintGobain.SenhaSnowFlake))
                throw new Exception("Não foi possível obter a configuração de integração com a Saint-Gobain. Usuário ou senha não informados.");
            return configuracaoSaintGobain;
        }

        private HttpClient ObterCliente(string usuario, string senha, string urlValidarToken, string apiKey)
        {

            string token = ObterTokenConsultaPedido(new ConfiguracaoIntegracao()
            {
                ClientID = usuario,
                ClientSecret = senha,
                UrlValidaToken = urlValidarToken
            });

            if (string.IsNullOrEmpty(token))
                throw new ServicoException("Não foi possivel obter o token de acesso!");

            HttpClient cliente = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSaintGobain));

            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            cliente.DefaultRequestHeaders.Add("APIKey", apiKey);

            return cliente;
        }

        private List<RequesteCargaSnowFlake> ObterObjetoIntegracaoCargaSnowFlake(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorCarga(cargaDadosTransporteIntegracao.Carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaDadosTransporteIntegracao.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

            List<RequesteCargaSnowFlake> retorno = new List<RequesteCargaSnowFlake>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidos)
                retorno.Add(new RequesteCargaSnowFlake()
                {
                    CCLI = cargaPedido?.Pedido?.Destinatario?.CodigoIntegracao ?? "",
                    CCTRO = cargaPedido?.CentroResultado?.CodigoCompanhia ?? "",
                    CFORNC = carga?.Empresa?.CodigoIntegracao ?? "",
                    CINCOTM = "",
                    CMOD_VEIC = carga?.ModeloVeicularCarga?.CodigoIntegracao ?? "",
                    CTPO_CARGA = carga?.TipoDeCarga?.CodigoTipoCargaEmbarcador ?? "",
                    CTPO_OPER = carga?.TipoOperacao?.CodigoIntegracao ?? "",
                    CUSUAR_SAP = "",
                    DAGNDA_CARGA = carga?.DataAgendamentoCarga.HasValue ?? false ? carga.DataAgendamentoCarga.Value.ToString("yyyy-MM-dd") : "",
                    DCHEGD_MTRTA = cargaPedido.DataChegada.HasValue ? cargaPedido.DataChegada.Value.ToString("yyyy-MM-dd") : "",
                    DCRIAC_AGNDA = carga.DataCriacaoCarga.ToString("yyyy-MM-dd"),
                    DCRIAC_DOCTO_TRNSP = "",
                    DENTRD_VEIC = "",//data Entrada veiculo
                    DFATMT = cargaJanelaCarregamento.DataTerminoCotacao.HasValue ? cargaJanelaCarregamento.DataTerminoCotacao.Value.ToString("yyyy-MM-dd") : "",
                    DFIM_CARRG = carga.DataFinalPrevisaoCarregamento.HasValue ? carga.DataFinalPrevisaoCarregamento.Value.ToString("yyyy-MM-dd") : "",
                    DINIC_CARRG = carga.DataInicialPrevisaoCarregamento.HasValue ? carga.DataInicialPrevisaoCarregamento.Value.ToString("yyyy-MM-dd") : "",
                    DRQUIS_CARRG = carga.DataInicialPrevisaoCarregamento.HasValue ? carga.DataInicialPrevisaoCarregamento.Value.ToString("yyyy-MM-dd") : "",
                    DSAIDA_VEIC = "",//data saida veiculo
                    DULT_AGNDA = "",//Desconhecido,
                    FNCRIO_AJDNT = "",
                    HAGNDA_CARGA = carga?.DataAgendamentoCarga.HasValue ?? false ? carga.DataAgendamentoCarga.Value.ToString("HH:mm:ss") : "",
                    HCHEGD_MTRTA = cargaPedido.DataChegada.HasValue ? cargaPedido.DataChegada.Value.ToString("HH:mm:ss") : "",
                    HCRIAC_AGNDA = carga.DataCriacaoCarga.ToString("HH:mm:ss"),
                    HCRIAC_DOCTO_TRNSP = "",//Sem conhecimento
                    HENTRD_VEIC = "",//Sem conhecimento
                    HFATMT = cargaJanelaCarregamento.DataTerminoCotacao.HasValue ? cargaJanelaCarregamento.DataTerminoCotacao.Value.ToString("HH:mm:ss") : "",
                    HFIM_CARRG = carga.DataInicialPrevisaoCarregamento.HasValue ? carga.DataInicialPrevisaoCarregamento.Value.ToString("HH:mm:ss") : "",
                    HINIC_CARRG = carga.DataInicialPrevisaoCarregamento.HasValue ? carga.DataInicialPrevisaoCarregamento.Value.ToString("HH:mm:ss") : "",
                    HRQUIS_CARRG = carga.DataInicialPrevisaoCarregamento.HasValue ? carga.DataInicialPrevisaoCarregamento.Value.ToString("HH:mm:ss") : "",
                    HSAIDA_VEIC = carga.DataInicioViagem.HasValue ? carga.DataInicioViagem.Value.ToString("HH:mm:ss") : "",
                    HULT_AGNDA = "",//Sem conhecimento
                    NCAPC_VEIC = 0,//Sem conhecimento
                    NENTRG = carga?.DadosSumarizados?.NumeroEntregas ?? 0,
                    NID_MTRTA = carga.Motoristas.FirstOrDefault()?.CPF.ToLong() ?? 0,
                    NKM_RDADO = 0,//Sem conhecimento
                    NNOTA_FSCAL_ELETR = cargaPedido?.NotasFiscais?.FirstOrDefault()?.XMLNotaFiscal?.Numero ?? 0,
                    NORD_CARGA = int.Parse(carga.CodigoCargaEmbarcador),
                    NPDIDO = cargaPedido?.Pedido?.Numero ?? 0,
                    NQTD_NO_SHOW = 0,//Sem conhecimento
                    NREMSS = 0,//Sem conhecimento
                    NTEMPO_RETOR_FORNC = 0,//Sem Mapeamento
                    QMEDD_BSICO = 0,//Sem conhecimento
                    QPECAS = cargaPedido?.QtVolumes ?? 0,
                    QPESO_BRUTO = cargaPedido.Peso,
                    QRAGND = cargaJanelaCarregamento.QuantidadeAlteracoesManuaisHorarioCarregamento,
                    RBAIRO = cargaPedido?.Pedido?.Destinatario?.Bairro ?? "",
                    RCIDDE = cargaPedido?.Pedido?.Destinatario?.Localidade?.Descricao ?? "",
                    RCLI = cargaPedido?.Pedido?.Destinatario?.Descricao ?? "",
                    REST = cargaPedido?.Destino?.CodigoURF ?? "",
                    RFORNC = carga?.Empresa?.CodigoIntegracao ?? "",
                    RID_VEIC = carga?.PlacasVeiculos ?? "",
                    RNOME_MTRTA = carga?.NomeMotoristas ?? "",
                    VFRETE_CARGA = cargaPedido?.ValorFrete ?? 0,
                    VFRETE_KM = 0,//Sem conhecimento
                    VFRETE_TON = 0,//Sem conhecimento
                    VPDIDO_NOTA_FSCAL = cargaPedido?.Pedido?.ValorTotalNotasFiscais ?? 0
                });

            return retorno;

        }

        private List<RequestAgendamento> ObterObjetoIntegracaoAgendamentoSnowFlake(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);


            List<RequestAgendamento> retorno = new List<RequestAgendamento>();

            retorno.Add(new RequestAgendamento
            {
                CTPO_OPER = carga?.TipoOperacao?.CodigoIntegracao ?? "",
                DAGNDA = cargaJanelaCarregamento?.InicioCarregamento.ToString("yyyy-MM-dd") ?? "",
                HAGNDA = cargaJanelaCarregamento?.InicioCarregamento.ToString("HH:mm:ss") ?? "",
                NGRADE_CTRO = 0//;
            });
            return retorno;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            
            if (!cargaDadosTransporteIntegracao.Carga.CarregamentoIntegradoERP)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Carregamento ERP ainda não foi integrado";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.NumeroTentativas++;
                cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

                repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

                return;
            }

            if (UtilizarConfiguracaoIntegracaoPIPO())
                IntegrarCargaPIPO(cargaDadosTransporteIntegracao, null, tipoServicoMultisoftware);
            else
                IntegrarCargaGateway(cargaDadosTransporteIntegracao, null, tipoServicoMultisoftware);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (UtilizarConfiguracaoIntegracaoPIPO())
                IntegrarCargaPIPO(cargaDadosTransporteIntegracao, dadosTrans, tipoServicoMultisoftware);
            else
                IntegrarCargaGateway(cargaDadosTransporteIntegracao, dadosTrans, tipoServicoMultisoftware);
        }

        public void IntegrarLancamentoNFSManual(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao)
        {
            if (UtilizarConfiguracaoIntegracaoPIPO())
                IntegrarLancamentoNFSManualPIPO(nfsManualCTeIntegracao);
            else
                IntegrarLancamentoNFSManualGateway(nfsManualCTeIntegracao);
        }

        public void IntegrarValores(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            if (UtilizarConfiguracaoIntegracaoPIPO())
                IntegrarValoresPIPO(cargaIntegracao);
            else
                IntegrarValoresGateway(cargaIntegracao);
        }

        public void IntegrarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            if (UtilizarConfiguracaoIntegracaoPIPO())
                IntegrarCarregamentoPIPO(carregamentoIntegracao);
            else
                IntegrarCarregamentoGateway(carregamentoIntegracao);
        }

        public void IntegrarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados)
        {
            if (UtilizarConfiguracaoIntegracaoPIPO())
                IntegrarCarregamentoPIPO(carregamentoIntegracao, dados);
            else
                IntegrarCarregamentoGateway(carregamentoIntegracao, dados);
        }

        #region Servico Gateway

        public void IntegrarCargaGateway(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositoriTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Servicos.Embarcador.Pedido.OcorrenciaPedido servicoOcorrenciaPedido = new Pedido.OcorrenciaPedido(_unitOfWork);


            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
            cargaDadosTransporteIntegracao.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                ServicoSaintGobain.ZSFH_TMS18_ALTERA_DT dadosCarga = dadosTrans != null ? ObterDadosIntegracaoCarga(dadosTrans, cargaDadosTransporteIntegracao.Carga) : ObterDadosIntegracaoCarga(cargaDadosTransporteIntegracao.Carga);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                InspectorBehavior inspector = new InspectorBehavior();
                ServicoSaintGobain.ZSFH_TMS_SERVICOSClient wsSaintGobain = ObterWebServiceClient(configuracaoIntegracao);

                wsSaintGobain.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoSaintGobain.ZSFH_TMS18_ALTERA_DTResponse retornoRequisicao = wsSaintGobain.ZSFH_TMS18_ALTERA_DT(dadosCarga);
                xmlRequisicao = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                if (retornoRequisicao.ET_RETORNO.Length > 0)
                {
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = retornoRequisicao.ET_RETORNO[0].ZMESSAGE;
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = (retornoRequisicao.ET_RETORNO[0].ZTYPE_MESSAGE == "S") ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "O WS Saint-Gobain não respondeu a solicitação.";
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, xmlRequisicao, xmlRetorno, "xml");

                if (cargaDadosTransporteIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    BloquearCargaCancelamentoPeloPortal(cargaDadosTransporteIntegracao.Carga);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaDadosTransporteIntegracao.Carga.Pedidos)
                        servicoOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoEmSeparacao, cargaPedido.Pedido, configuracaoEmbarcador, null);

                    var existeTipoIntegracao = repositoriTipoIntegracao.BuscarPorTipo(TipoIntegracao.SaintGobainCarga);

                    if (existeTipoIntegracao != null)
                        Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaDadosTransporteIntegracao(cargaDadosTransporteIntegracao.Carga, existeTipoIntegracao, _unitOfWork, false, false);
                }
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a Saint-Gobain.";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarLancamentoNFSManualGateway(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repositorioNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            nfsManualCTeIntegracao.DataIntegracao = DateTime.Now;
            nfsManualCTeIntegracao.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                ServicoSaintGobain.ZSFH_TMS21_RECEBE_DADOS_NFSE dadosLancamentoNFSManual = ObterDadosIntegracaoLancamentoNFSManual(nfsManualCTeIntegracao);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                InspectorBehavior inspector = new InspectorBehavior();
                ServicoSaintGobain.ZSFH_TMS_SERVICOSClient wsSaintGobain = ObterWebServiceClient(configuracaoIntegracao);

                wsSaintGobain.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoSaintGobain.ZSFH_TMS21_RECEBE_DADOS_NFSEResponse retornoRequisicao = wsSaintGobain.ZSFH_TMS21_RECEBE_DADOS_NFSE(dadosLancamentoNFSManual);
                xmlRequisicao = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                if (retornoRequisicao.ET_RETORNO.Length > 0)
                {
                    nfsManualCTeIntegracao.ProblemaIntegracao = retornoRequisicao.ET_RETORNO[0].ZMESSAGE;
                    nfsManualCTeIntegracao.SituacaoIntegracao = (retornoRequisicao.ET_RETORNO[0].ZTYPE_MESSAGE == "S") ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    nfsManualCTeIntegracao.ProblemaIntegracao = "O WS Saint-Gobain não respondeu a solicitação.";
                    nfsManualCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(nfsManualCTeIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }
            catch (ServicoException excecao)
            {
                nfsManualCTeIntegracao.ProblemaIntegracao = excecao.Message;
                nfsManualCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                nfsManualCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a Saint-Gobain.";
                nfsManualCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(nfsManualCTeIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositorioNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);
        }

        public void IntegrarValoresGateway(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                ServicoSaintGobain.ZSFH_TMS19_BUSCA_FRETE dadosValores = ObterDadosIntegracaoValores(cargaIntegracao);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                InspectorBehavior inspector = new InspectorBehavior();
                ServicoSaintGobain.ZSFH_TMS_SERVICOSClient wsSaintGobain = ObterWebServiceClient(configuracaoIntegracao);

                wsSaintGobain.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoSaintGobain.ZSFH_TMS19_BUSCA_FRETEResponse retornoRequisicao = wsSaintGobain.ZSFH_TMS19_BUSCA_FRETE(dadosValores);
                xmlRequisicao = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                if (retornoRequisicao.ET_RETORNO.Length > 0)
                {
                    string mensagemProblemaIntegracao = "";

                    foreach (ServicoSaintGobain.ZSD_TMS_RETORNO_WP19 retorno in retornoRequisicao.ET_RETORNO)
                    {
                        if (retorno.ZTYPE_MESSAGE != "S")
                            mensagemProblemaIntegracao += retorno.ZMESSAGE;
                    }

                    if (string.IsNullOrWhiteSpace(mensagemProblemaIntegracao))
                    {
                        cargaIntegracao.ProblemaIntegracao = "Registro gravado";
                        cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    }
                    else
                    {
                        cargaIntegracao.ProblemaIntegracao = mensagemProblemaIntegracao.Left(300);
                        cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
                else
                {
                    cargaIntegracao.ProblemaIntegracao = "O WS Saint-Gobain não respondeu a solicitação.";
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracao, xmlRequisicao, xmlRetorno, "xml");

                if (cargaIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    BloquearCargaCancelamentoPeloPortal(cargaIntegracao.Carga);
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a Saint-Gobain.";
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(cargaIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositorioCargaCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarCarregamentoGateway(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);

            carregamentoIntegracao.DataIntegracao = DateTime.Now;
            carregamentoIntegracao.NumeroTentativas++;

            ServicoSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO dadosCarregamento = ObterDadosIntegracaoCarregamento(carregamentoIntegracao);

            if (dadosCarregamento != null)
                IntegrarObjetoAgendamento(carregamentoIntegracao, dadosCarregamento, timeout: ObterTimeoutPadrao());
            else
            {
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = $"Informações de carga não encontrada para o carregamento {carregamentoIntegracao?.Carregamento?.NumeroCarregamento ?? String.Empty} para gerar integração.";
                carregamentoIntegracao.NumeroTentativas = 98;
            }

            repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        public void IntegrarCarregamentoGateway(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);

            carregamentoIntegracao.DataIntegracao = DateTime.Now;
            carregamentoIntegracao.NumeroTentativas++;

            ServicoSaintGobain.ZSFH_TMS27_RECEBE_DADOS_AGTO dadosCarregamento = ObterDadosIntegracaoCarregamentoObjetoTemporario(carregamentoIntegracao, dados);
            IntegrarObjetoAgendamento(carregamentoIntegracao, dadosCarregamento, timeout: new TimeSpan(hours: 0, minutes: 0, seconds: 45));

            repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        #endregion

        #region Servico PI/PO
        public void IntegrarCargaPIPO(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTrans, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositoriTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Servicos.Embarcador.Pedido.OcorrenciaPedido servicoOcorrenciaPedido = new Pedido.OcorrenciaPedido(_unitOfWork);


            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
            cargaDadosTransporteIntegracao.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                ServicoSaintGobainPIPO.ZSFH_TMS18_ALTERA_DTRequest dadosCarga = dadosTrans != null ? ObterDadosIntegracaoCargaPIPO(dadosTrans, cargaDadosTransporteIntegracao.Carga) : ObterDadosIntegracaoCargaPIPO(cargaDadosTransporteIntegracao.Carga);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                InspectorBehavior inspector = new InspectorBehavior();
                ServicoSaintGobainPIPO.SI_conjuntoFuncoesTMS_sync_OutClient wsSaintGobain = ObterWebServiceClientPIPO(configuracaoIntegracao);

                wsSaintGobain.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoSaintGobainPIPO.ZSD_TMS_RETORNO_WP18[] retornoRequisicao = wsSaintGobain.ZSFH_TMS18_ALTERA_DT(dadosCarga.IT_ALTERA_DT);
                xmlRequisicao = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                if (retornoRequisicao.Length > 0)
                {
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = retornoRequisicao[0].ZMESSAGE;
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = (retornoRequisicao[0].ZTYPE_MESSAGE == "S") ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "O WS Saint-Gobain não respondeu a solicitação.";
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, xmlRequisicao, xmlRetorno, "xml");

                if (cargaDadosTransporteIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    BloquearCargaCancelamentoPeloPortal(cargaDadosTransporteIntegracao.Carga);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaDadosTransporteIntegracao.Carga.Pedidos)
                        servicoOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoEmSeparacao, cargaPedido.Pedido, configuracaoEmbarcador, null);

                    var existeTipoIntegracao = repositoriTipoIntegracao.BuscarPorTipo(TipoIntegracao.SaintGobainCarga);

                    if (existeTipoIntegracao != null)
                        Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaDadosTransporteIntegracao(cargaDadosTransporteIntegracao.Carga, existeTipoIntegracao, _unitOfWork, false, false);
                }
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a Saint-Gobain.";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarLancamentoNFSManualPIPO(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repositorioNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            nfsManualCTeIntegracao.DataIntegracao = DateTime.Now;
            nfsManualCTeIntegracao.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                ServicoSaintGobainPIPO.ZSFH_TMS21_RECEBE_DADOS_NFSERequest dadosLancamentoNFSManual = ObterDadosIntegracaoLancamentoNFSManualPIPO(nfsManualCTeIntegracao);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                InspectorBehavior inspector = new InspectorBehavior();
                ServicoSaintGobainPIPO.SI_conjuntoFuncoesTMS_sync_OutClient wsSaintGobain = ObterWebServiceClientPIPO(configuracaoIntegracao);

                wsSaintGobain.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoSaintGobainPIPO.ZSD_TMS_RETORNO_WP21[] retornoRequisicao = wsSaintGobain.ZSFH_TMS21_RECEBE_DADOS_NFSE(dadosLancamentoNFSManual.IS_CABECALHO, dadosLancamentoNFSManual.IT_ITEM);
                xmlRequisicao = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                if (retornoRequisicao.Length > 0)
                {
                    nfsManualCTeIntegracao.ProblemaIntegracao = retornoRequisicao[0].ZMESSAGE;
                    nfsManualCTeIntegracao.SituacaoIntegracao = (retornoRequisicao[0].ZTYPE_MESSAGE == "S") ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    nfsManualCTeIntegracao.ProblemaIntegracao = "O WS Saint-Gobain não respondeu a solicitação.";
                    nfsManualCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(nfsManualCTeIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }
            catch (ServicoException excecao)
            {
                nfsManualCTeIntegracao.ProblemaIntegracao = excecao.Message;
                nfsManualCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                nfsManualCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a Saint-Gobain.";
                nfsManualCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(nfsManualCTeIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositorioNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);
        }

        public void IntegrarValoresPIPO(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracao();
                ServicoSaintGobainPIPO.ZSFH_TMS19_BUSCA_FRETERequest dadosValores = ObterDadosIntegracaoValoresPIPO(cargaIntegracao);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                InspectorBehavior inspector = new InspectorBehavior();
                ServicoSaintGobainPIPO.SI_conjuntoFuncoesTMS_sync_OutClient wsSaintGobain = ObterWebServiceClientPIPO(configuracaoIntegracao);

                wsSaintGobain.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoSaintGobainPIPO.ZSD_TMS_RETORNO_WP19[] retornoRequisicao = wsSaintGobain.ZSFH_TMS19_BUSCA_FRETE(dadosValores.IT_FRETE);
                xmlRequisicao = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                if (retornoRequisicao.Length > 0)
                {
                    string mensagemProblemaIntegracao = "";

                    foreach (ServicoSaintGobainPIPO.ZSD_TMS_RETORNO_WP19 retorno in retornoRequisicao)
                    {
                        if (retorno.ZTYPE_MESSAGE != "S")
                            mensagemProblemaIntegracao += retorno.ZMESSAGE;
                    }

                    if (string.IsNullOrWhiteSpace(mensagemProblemaIntegracao))
                    {
                        cargaIntegracao.ProblemaIntegracao = "Registro gravado";
                        cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    }
                    else
                    {
                        cargaIntegracao.ProblemaIntegracao = mensagemProblemaIntegracao.Left(300);
                        cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
                else
                {
                    cargaIntegracao.ProblemaIntegracao = "O WS Saint-Gobain não respondeu a solicitação.";
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracao, xmlRequisicao, xmlRetorno, "xml");

                if (cargaIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    BloquearCargaCancelamentoPeloPortal(cargaIntegracao.Carga);
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a Saint-Gobain.";
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(cargaIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositorioCargaCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarCarregamentoPIPO(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);

            carregamentoIntegracao.DataIntegracao = DateTime.Now;
            carregamentoIntegracao.NumeroTentativas++;

            ServicoSaintGobainPIPO.ZSFH_TMS27_RECEBE_DADOS_AGTORequest dadosCarregamento = ObterDadosIntegracaoCarregamentoPIPO(carregamentoIntegracao);

            if (dadosCarregamento != null)
                IntegrarObjetoAgendamentoPIPO(carregamentoIntegracao, dadosCarregamento, timeout: ObterTimeoutPadrao());
            else
            {
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = $"Informações de carga não encontrada para o carregamento {carregamentoIntegracao?.Carregamento?.NumeroCarregamento ?? String.Empty} para gerar integração.";
                carregamentoIntegracao.NumeroTentativas = 98;
            }

            repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        public void IntegrarCarregamentoPIPO(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Carga.DadosPortalRetira dados)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);

            carregamentoIntegracao.DataIntegracao = DateTime.Now;
            carregamentoIntegracao.NumeroTentativas++;

            ServicoSaintGobainPIPO.ZSFH_TMS27_RECEBE_DADOS_AGTORequest dadosCarregamento = ObterDadosIntegracaoCarregamentoObjetoTemporarioPIPO(carregamentoIntegracao, dados);
            IntegrarObjetoAgendamentoPIPO(carregamentoIntegracao, dadosCarregamento, timeout: new TimeSpan(hours: 0, minutes: 0, seconds: 45));

            repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        #endregion

        public void ReenviarIntegrarCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.Tipo != TipoCargaJanelaCarregamento.Carregamento)
                return;

            if (cargaJanelaCarregamento.Carga?.Carregamento == null)
                return;


            List<TipoIntegracao> tiposPermitidosReenviar = new List<TipoIntegracao>() { TipoIntegracao.SaintGobainAgendamento, TipoIntegracao.SaintGobain };

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            foreach (var tipo in tiposPermitidosReenviar)
            {

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoParaReenviar = repositorioTipoIntegracao.BuscarPorTipo(tipo);
                if (tipoIntegracaoParaReenviar == null)
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracao = repositorioCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(cargaJanelaCarregamento.Carga.Codigo, tipoIntegracaoParaReenviar.Codigo, integracaoColeta: false);

                if (integracao != null)
                {
                    integracao.ProblemaIntegracao = string.Empty;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                    repositorioCargaDadosTransporteIntegracao.Atualizar(integracao);
                }

                if (integracao == null && tipo == TipoIntegracao.SaintGobainAgendamento)
                    Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaDadosTransporteIntegracao(cargaJanelaCarregamento.Carga, tipoIntegracaoParaReenviar, _unitOfWork, false, false);
            }

            Servicos.Embarcador.Integracao.IntegracaoCarregamento servicoIntegracaoCarregamento = new Servicos.Embarcador.Integracao.IntegracaoCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao integracaoCarregamento = servicoIntegracaoCarregamento.ObterIntegracaoCarregamento(cargaJanelaCarregamento.Carga.Carregamento.Codigo, TipoIntegracao.SaintGobain);

            if (integracaoCarregamento == null)
                return;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);

            if ((integracaoCarregamento.SituacaoIntegracao == SituacaoIntegracao.Integrado) && (integracaoCarregamento.Status == StatusCarregamentoIntegracao.Inserir))
                integracaoCarregamento.Status = StatusCarregamentoIntegracao.Atualizar;

            integracaoCarregamento.ProblemaIntegracao = string.Empty;
            integracaoCarregamento.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
        }

        public Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.AndamentoPedido ConsultarAndamentoPedido(string numOv, string CodEmpresa, string CodUserSap, string tipoUsuario = "")
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoConsultaPedido();


            string urlConsultaPedido = configuracaoIntegracao.UrlConsultaPedido; //"https://api.saint-gobain.com/sgdsi/br/tms/portal-monit/ppc/dev/v1/portalMonitoramento/consultaPedido";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient(urlConsultaPedido);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("APIKey", configuracaoIntegracao.APIKey);
            request.AddHeader("Authorization", "Bearer " + ObterTokenConsultaPedido(configuracaoIntegracao));

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.ParametrosConsultaPedido parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.ParametrosConsultaPedido();
            parametrosConsulta.codEmpresa = string.IsNullOrEmpty(CodEmpresa) ? "BM40" : CodEmpresa;
            parametrosConsulta.numOv = numOv;
            parametrosConsulta.codUserSap = CodUserSap;
            parametrosConsulta.tpUserSap = tipoUsuario;

            request.AddJsonBody(parametrosConsulta);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Pedido retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Pedido>(response.Content);

                return ConverterRetornoAndamentoPedido(retorno);
            }

            return null;
        }

        public Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.Usuario ConsultarUsuarioAndamentoPedido(string codUserSap)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.ConfiguracaoIntegracao configuracaoIntegracao = ObterConfiguracaoIntegracaoConsultaPedido();

            string UrlConsultaUsuario = configuracaoIntegracao.UrlConsultaUsuario + "/" + codUserSap;  //"https://api.saint-gobain.com/sgdsi/br/tms/portal-monit/ppc/dev/v1/portalMonitoramento/validaUser/" + codUserSap;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient(UrlConsultaUsuario);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("APIKey", configuracaoIntegracao.APIKey);
            request.AddHeader("Authorization", "Bearer " + ObterTokenConsultaPedido(configuracaoIntegracao));

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoUsuario retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoUsuario>(response.Content);
                if (retorno != null && retorno.ITEM.Count > 0)
                    return retorno.ITEM.FirstOrDefault();
            }

            return null;
        }

        public void IntegrarTabelaFreteCliente(Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao integracaoFrete)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo>(_unitOfWork);

            integracaoFrete.DataIntegracao = DateTime.Now;
            integracaoFrete.NumeroTentativas++;
            string jsonrequest = "";
            string jsonresponse = "";
            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain configuracaoSantGobain = ObterConfiguracaoIntegracaoPadrao();

                if (string.IsNullOrEmpty(configuracaoSantGobain.UrlIntegracaoFreteSnowFlake))
                    throw new Exception("Não foi possível obter a configuração de integração com a Saint-Gobain. Url TabelaFrete Cliente não informado.");


                jsonrequest = JsonConvert.SerializeObject(new
                {
                    row = new List<RequestIntegracaoFRETE>(){
                    new RequestIntegracaoFRETE
                    {
                        NCONHC_TRNSP = 0,//numero cte
                        PFRETE_CIF = 0,
                        PFRETE_FOB = 0,
                        PFRETE_VLR_PRODT = 0,//porcentagen do frete no valor do produto 
                        VFRETE_RESDU = 0 // valor residual (cursto da ociosidade)
                    },
                }
                });

                HttpClient cliente = ObterCliente(configuracaoSantGobain.UsuariosSnowFlake, configuracaoSantGobain.SenhaSnowFlake, configuracaoSantGobain.UrlValidaToken, configuracaoSantGobain.ApiKeySnowFlake);
                StringContent content = new StringContent(jsonrequest, Encoding.UTF8, "application/json");
                var result = cliente.PostAsync(configuracaoSantGobain.UrlIntegracaoFreteSnowFlake, content).Result;

                jsonresponse = result.Content.ReadAsStringAsync().Result;

                if (!result.IsSuccessStatusCode)
                    throw new ServicoException("Erro ao tentar integrar com Saint-Gobain");

                integracaoFrete.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracaoFrete.ProblemaIntegracao = "Integrado Com Sucesso";
                servicoArquivoTransacao.Adicionar(integracaoFrete, jsonrequest, jsonresponse, "json");
            }
            catch (ServicoException exe)
            {
                integracaoFrete.ProblemaIntegracao = exe.Message;
                integracaoFrete.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                integracaoFrete.ProblemaIntegracao = "Ocorreu uma falha ao integrar com a Saint-Gobain.";
                integracaoFrete.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(integracaoFrete, jsonrequest, jsonresponse, "json");
            }

            repositorioTabelaFreteClienteIntegracao.Atualizar(integracaoFrete);
        }

        public void IntegrarCargaSnowFlake(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain configuracaoSantGobain = ObterConfiguracaoIntegracaoPadrao();

                if (string.IsNullOrEmpty(configuracaoSantGobain.UrlIntegracaoCargaSnowFlake))
                    throw new ServicoException("URL de integração Carga SnowFlake não configurada");

                jsonRequest = JsonConvert.SerializeObject(new
                {
                    row = ObterObjetoIntegracaoCargaSnowFlake(cargaDadosTransporteIntegracao)
                });


                HttpClient cliente = ObterCliente(configuracaoSantGobain.UsuariosSnowFlake, configuracaoSantGobain.SenhaSnowFlake, configuracaoSantGobain.UrlValidaToken, configuracaoSantGobain.ApiKeySnowFlake);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var result = cliente.PostAsync(configuracaoSantGobain.UrlIntegracaoCargaSnowFlake, content).Result;

                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (!result.IsSuccessStatusCode)
                    throw new ServicoException("Erro ao tentar integrar com Saint-Gobain");

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integrado Com Sucesso";
                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");

            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCargaDadosTransporte.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarAgendamentoSnowFlake(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain configuracaoSantGobain = ObterConfiguracaoIntegracaoPadrao();

                if (string.IsNullOrEmpty(configuracaoSantGobain.UrlIntegracaoAgendamentoSnowFlake))
                    throw new ServicoException("URL de integração Carga SnowFlake não configurada");

                jsonRequest = JsonConvert.SerializeObject(new
                {
                    row = ObterObjetoIntegracaoAgendamentoSnowFlake(cargaDadosTransporteIntegracao.Carga)
                });


                HttpClient cliente = ObterCliente(configuracaoSantGobain.UsuariosSnowFlake, configuracaoSantGobain.SenhaSnowFlake, configuracaoSantGobain.UrlValidaToken, configuracaoSantGobain.ApiKeySnowFlake);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var result = cliente.PostAsync(configuracaoSantGobain.UrlIntegracaoCargaSnowFlake, content).Result;

                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (!result.IsSuccessStatusCode)
                    throw new ServicoException("Erro ao tentar integrar com Saint-Gobain");

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integrado Com Sucesso";
                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");

            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCargaDadosTransporte.Atualizar(cargaDadosTransporteIntegracao);
        }

        #endregion
    }
}
