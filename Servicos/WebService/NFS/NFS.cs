using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using Dominio.ObjetosDeValor.WebService.CTe;
using Dominio.ObjetosDeValor.WebService.NFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Servicos.WebService.NFS
{
    public class NFS : ServicoBase
    {
        #region Propiedades Privadas

        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Constructores

        public NFS(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public NFS(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Metodos Publicos

        public List<Dominio.ObjetosDeValor.WebService.NFS.NFS> BuscarNFS(int codigoCarga, int codigoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, ref string mensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.ObjetosDeValor.WebService.NFS.NFS> NFSsIntegracao = new List<Dominio.ObjetosDeValor.WebService.NFS.NFS>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfigWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfigWebService.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarListaPorProtocoloCargaOrigemEProtocoloPedido(codigoCarga, codigoPedido);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedido.FirstOrDefault();

            List<int> codigosCargaPedidos = new List<int>();

            if (cargaPedido != null)
            {
                codigosCargaPedidos.Add(cargaPedido.Codigo);

                if (configuracaoWebService.RetornarDadosRedespachoTransbordoComInformacoesCargaOrigemConsultada)
                {
                    Repositorio.Embarcador.Cargas.Transbordo repositorioTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
                    Repositorio.Embarcador.Cargas.Redespacho repositorioRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(unitOfWork);

                    List<int> protocolosTransbordos = repositorioTransbordo.BuscarPorProtocoloIntegracaoCargaOrigem(cargaPedido.Carga.Protocolo);
                    List<int> protocolosRedespachos = repositorioRedespacho.BuscarPorProtocoloIntegracaoCargaOrigem(cargaPedido.Carga.Protocolo);
                    List<int> protocolosTransbordosRedespachos = protocolosTransbordos.Concat(protocolosRedespachos).ToList();

                    if (protocolosTransbordosRedespachos.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidoTransbordoRedespacho = repCargaPedido.BuscarListaPorProtocoloCargaOrigemEProtocoloPedido(protocolosTransbordosRedespachos, codigoPedido);

                        codigosCargaPedidos.AddRange(listaCargaPedidoTransbordoRedespacho.Select(cargaPedidoTransbordoRedespacho => cargaPedidoTransbordoRedespacho.Codigo));
                    }
                }
            }

            if (codigosCargaPedidos.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarNFSePorCargaPedido(cargaPedido.Codigo, cargaPedido.Carga.Codigo, inicioRegistro, fimRegistro, tipoServicoMultisoftware, configuracaoWebService);
                List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in cargaCTes select obj.CTe.Codigo).ToList());

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                    NFSsIntegracao.Add(ConverterObjetoCargaNFS(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, unitOfWork));
            }
            else
                Servicos.Log.TratarErro("ProtocoloIntegracaoCarga: " + codigoCarga + "; protocoloIntegracaoPedido: " + codigoPedido, "Protocolo Invalido BuscarNFSs");

            return NFSsIntegracao;
        }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarNFSCompleta(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> nfssIntegracao = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao repConfiguracaoCargaIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaIntegracao configuracaoCargaIntegracao = repConfiguracaoCargaIntegracao.BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarNFSePorCarga(carga.Codigo, inicioRegistro, fimRegistro);
            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in cargaCTes select obj.CTe.Codigo).ToList());

            Servicos.WebService.CTe.CTe svcWSCTe = new CTe.CTe(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                nfssIntegracao.Add(svcWSCTe.ConverterObjetoCargaCTe(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, unitOfWork, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, configuracaoTMS, configuracaoCargaIntegracao));

            return nfssIntegracao;
        }

        public int ContarNFSCompleta(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            return repCargaPedidoXMLNotaFiscalCTe.ContarNFSePorCarga(carga.Codigo);
        }

        public List<Dominio.ObjetosDeValor.WebService.NFS.NFS> BuscarNFS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.NFS.NFS> NFSsIntegracao = new List<Dominio.ObjetosDeValor.WebService.NFS.NFS>();
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarNFSePorCarga(carga.Codigo, inicioRegistro, fimRegistro);
            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in cargaCTes select obj.CTe.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                NFSsIntegracao.Add(ConverterObjetoCargaNFS(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, unitOfWork));

            return NFSsIntegracao;
        }

        public int ContarNFS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            return repCargaPedidoXMLNotaFiscalCTe.ContarNFSePorCarga(carga.Codigo);
        }

        public int ContarNFS(int codigoCarga, int codigoPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfigWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfigWebService.BuscarConfiguracaoPadrao();
            //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(codigoCarga, codigoPedido);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarListaPorProtocoloCargaOrigemEProtocoloPedido(codigoCarga, codigoPedido);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedido != null ? listaCargaPedido.FirstOrDefault() : null;

            if (cargaPedido != null)
                return repCargaPedidoXMLNotaFiscalCTe.ContarNFSePorCargaPedido(cargaPedido.Codigo, codigoCarga, tipoServicoMultisoftware, configuracaoWebService);
            else
                return 0;
        }

        public List<Dominio.ObjetosDeValor.WebService.NFS.NFS> BuscarNFSPeriodo(DateTime dataInicial, DateTime dataFinal, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicioRegistro, int fimRegistro, string codigoTipoOperacao, string situacao, ref string mensagem)
        {
            List<Dominio.ObjetosDeValor.WebService.NFS.NFS> NFSsIntegracao = new List<Dominio.ObjetosDeValor.WebService.NFS.NFS>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarNFSePorPeriodoEmpresa(dataInicial, dataFinal, empresa, inicioRegistro, fimRegistro, codigoTipoOperacao, situacao);
            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTes((from obj in cargaCTes select obj.CTe.Codigo).ToList());
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                NFSsIntegracao.Add(ConverterObjetoCargaNFS(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, unitOfWork));
            }

            return NFSsIntegracao;
        }

        public int ContarNFSPeriodo(DateTime dataInicial, DateTime dataFinal, int empresa, string codigoTipoOperacao, string situacao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            return repCargaPedidoXMLNotaFiscalCTe.ContarNFSePorPeriodoEmpresa(dataInicial, dataFinal, empresa, codigoTipoOperacao, situacao);
        }

        public Dominio.ObjetosDeValor.WebService.NFS.NFS ConverterObjetoCargaNFS(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaosCTes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, bool codificarUTF8, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.NFSeItem repNFSeItem = new Repositorio.NFSeItem(unitOfWork);

            Servicos.WebService.Empresa.Motorista serWSMotorista = new Empresa.Motorista(unitOfWork);
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Frota.Veiculo(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Dominio.ObjetosDeValor.WebService.NFS.NFS nfs = ConverterObjetoNFS(cargaCTe.CTe, cTeContaContabilContabilizacaosCTes, tipoDocumentoRetorno, codificarUTF8, unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCargaCTeEChaveNF(cargaCTe.Carga.Codigo, nfs.NFSe?.Documentos?.FirstOrDefault()?.ChaveNFe ?? string.Empty);
            nfs.NFSe.NumeroCarga = cargaCTe.Carga.CodigoCargaEmbarcador;
            nfs.NFSe.ProtocoloCarga = cargaCTe.Carga.Protocolo;
            nfs.Ocorrencia = ConverterObjetoOcorrencia(cargaCTe.CargaCTeComplementoInfo?.CargaOcorrencia ?? cargaDocumentoParaEmissaoNFSManual?.CargaOcorrencia);

            nfs.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
            foreach (Dominio.Entidades.Usuario motorista in cargaCTe.Carga.Motoristas)
                nfs.Motoristas.Add(serWSMotorista.ConverterObjetoMotorista(motorista));

            nfs.Veiculo = serWSVeiculo.ConverterObjetoConjuntoVeiculos(cargaCTe.Carga.Veiculo, cargaCTe.Carga.VeiculosVinculados.ToList(), unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCTe.Codigo);

            nfs.protocolos = new List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();

            nfs.CodigoServico = cargaCTe.LancamentoNFSManual?.CodigoServico;

            if (cargaCTe.LancamentoNFSManual == null)
            {
                List<Dominio.Entidades.NFSeItem> itens = repNFSeItem.BuscarPorCTe(cargaCTe.CTe.Codigo);
                Dominio.Entidades.NFSeItem item = itens.FirstOrDefault();

                nfs.NFSe.ServicoNFSe = new Dominio.ObjetosDeValor.WebService.NFS.ServicoNFSe
                {
                    Numero = item?.Servico?.Numero ?? "",
                    Descricao = item?.Servico?.Descricao ?? ""
                };
            }
            else
            {
                nfs.NFSe.ServicoNFSe = new Dominio.ObjetosDeValor.WebService.NFS.ServicoNFSe
                {
                    Numero = cargaCTe.LancamentoNFSManual?.DadosNFS?.ServicoNFSe?.Numero ?? "",
                    Descricao = cargaCTe.LancamentoNFSManual?.DadosNFS?.ServicoNFSe?.Descricao ?? ""
                };
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosCTe)
            {
                Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos();
                nfs.ProtocolosDePedidos.Add(cargaPedido.Pedido.Codigo);

                protocolo.protocoloIntegracaoCarga = cargaPedido.Carga.Protocolo;
                protocolo.protocoloIntegracaoPedido = cargaPedido.Pedido.Protocolo;
                nfs.protocolos.Add(protocolo);
            }

            return nfs;
        }

        public Dominio.ObjetosDeValor.WebService.NFS.NFS ConverterObjetoNFS(Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse, List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaosCTes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, bool codificarUTF8, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLNFSe repXMLNFSe = new Repositorio.XMLNFSe(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Empresa.Motorista serWSMotorista = new Empresa.Motorista(unitOfWork);
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Frota.Veiculo(unitOfWork);

            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = (from obj in cTeContaContabilContabilizacaosCTes where obj.Cte.Codigo == nfse.Codigo select obj).ToList();
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorCTe(nfse.Codigo);

            Dominio.ObjetosDeValor.WebService.NFS.NFS nfs = new Dominio.ObjetosDeValor.WebService.NFS.NFS
            {
                LocalidadePrestacaoServico = serLocalidade.ConverterObjetoLocalidade(nfse.LocalidadeInicioPrestacao),
                NFSe = new Dominio.ObjetosDeValor.WebService.NFS.NFSe
                {
                    DataEmissao = nfse.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                    DataCancelamento = nfse.DataCancelamento?.ToString("dd/MM/yyyy HH:mm:ss"),
                    MotivoCancelamento = nfse.ObservacaoCancelamento,
                    NaturezaNFSe = new Dominio.ObjetosDeValor.WebService.NFS.NaturezaNFSe
                    {
                        Codigo = nfse.NaturezaNFSe?.Numero ?? nfse.ModeloDocumentoFiscal?.Codigo ?? 0,
                        Descricao = nfse.NaturezaNFSe?.Descricao ?? nfse.ModeloDocumentoFiscal?.Descricao ?? ""
                    },
                    Numero = nfse.Numero,
                    //NumeroCarga = cargaCTe.Carga.CodigoCargaEmbarcador,
                    NumeroProtocolo = nfse.Codigo.ToString(),
                    NumeroRPS = nfse.RPS?.Numero ?? 0,
                    SerieRPS = nfse.RPS?.Serie ?? "",
                    CodigoVerificacao = nfse.Protocolo,
                    Serie = nfse.Serie != null ? nfse.Serie.Numero : 0,
                    XML = "",
                    PDF = "",
                    Documentos = new List<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe>(),
                    StatusNFSe = nfse.Status == "A" ? Dominio.Enumeradores.StatusNFSe.Autorizado : nfse.Status == "C" ? Dominio.Enumeradores.StatusNFSe.Cancelado : Dominio.Enumeradores.StatusNFSe.Pendente,
                },
                ValoresNFS = new Dominio.ObjetosDeValor.WebService.NFS.ValorNFS
                {
                    AliquotaISS = nfse.AliquotaISS,
                    BaseCalculoISS = nfse.BaseCalculoISS,
                    ISSRetido = nfse.ISSRetido,
                    ValorCOFINS = nfse.ValorCOFINS,
                    ValorCSLL = nfse.ValorCSLL,
                    ValorDeducoes = nfse.ValorDeducoes,
                    ValorDescontoCondicionado = nfse.ValorDescontoCondicionado,
                    ValorDescontoIncondicionado = nfse.ValorDescontoIncondicionado,
                    ValorINSS = nfse.ValorINSS,
                    ValorIR = nfse.ValorIR,
                    ValorISS = nfse.ValorISS,
                    ValorISSRetido = nfse.ValorISSRetido,
                    ValorOutrasRetencoes = nfse.ValorOutrasRetencoes,
                    ValorPIS = nfse.ValorPIS,
                    ValorServicos = nfse.ValorAReceber,
                    ValorServicosSemImpostoIncluso = nfse.IncluirISSNoFrete == OpcaoSimNao.Sim ? nfse.ValorAReceber - nfse.ValorISS : nfse.ValorAReceber
                },
                Protocolo = nfse.Codigo,
                ProtocolosDePedidos = new List<int>() { },
                TipoNotaFiscalServico = Dominio.Enumeradores.TipoNotaFiscalServico.Eletronica,
                Tomador = serWSPessoa.ConverterObjetoParticipamenteCTe(nfse.Tomador),
                TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(nfse.Empresa)
            };

            if (nfse.TipoCTE == TipoCTE.Complemento && nfs.NFSe != null && nfse.CargaCTeOcorrencias != null && nfse.CargaCTeOcorrencias.Count > 0)
            {
                nfs.NFSe.NumeroNFSeComplementada = nfse.CargaCTeOcorrencias.FirstOrDefault().CTeComplementado?.Numero ?? 0;
                nfs.NFSe.SerieNFSeComplementada = nfse.CargaCTeOcorrencias.FirstOrDefault().CTeComplementado?.Serie.Numero ?? 0;
            }

            nfs.ContasContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil>();
            foreach (Dominio.Entidades.CTeContaContabilContabilizacao cteContaContabilContabilizacao in cTeContaContabilContabilizacaos)
            {
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil contaContabil = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ContaContabil();
                contaContabil.CodigoIntegracao = cteContaContabilContabilizacao.PlanoConta.PlanoContabilidade;
                nfs.ContasContabeis.Add(contaContabil);
            }

            nfs.ItemServico = nfse.ItemServico;

            decimal valor = nfse.ValorAReceber;
            if (nfse.CentroResultado != null)
            {
                nfs.CentroResultado = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { CodigoIntegracao = nfse.CentroResultado.PlanoContabilidade };
                if (nfse.ValorMaximoCentroContabilizacao > 0)
                {
                    valor = nfse.ValorAReceber - nfse.ValorMaximoCentroContabilizacao;
                    if (valor < 0)
                    {
                        nfs.CentroResultado.Valor = nfse.ValorAReceber;
                        valor = 0;
                    }
                    else
                        nfs.CentroResultado.Valor = nfse.ValorMaximoCentroContabilizacao;
                }
            }

            if (nfse.CentroResultadoEscrituracao != null)
                nfs.CentroResultadoEscrituracao = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { Valor = valor, CodigoIntegracao = nfse.CentroResultadoEscrituracao.PlanoContabilidade };

            if (nfse.CentroResultadoDestinatario != null)
                nfs.CentroResultadoDestinatario = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.CentroResultado() { Valor = valor, CodigoIntegracao = nfse.CentroResultadoDestinatario.PlanoContabilidade };


            if (nfse.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento || (nfse.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && !configuracao.NaoRetornarNotasEmDocumentoComplementar))
            {
                List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(nfse.Codigo);
                //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(nfse.Codigo);

                foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentosCTe)
                {
                    Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe nota = new Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe();

                    if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                    {
                        nota.ChaveNFe = documentoCTe.ChaveNFE;
                        nota.Numero = documentoCTe.Numero;
                        nota.Serie = documentoCTe.Serie;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Numero)))
                            nota.Numero = documentoCTe.Numero;
                        if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(documentoCTe.Serie)))
                            nota.Serie = documentoCTe.Serie;
                    }

                    if (!string.IsNullOrWhiteSpace(nota.ChaveNFe) || !string.IsNullOrWhiteSpace(nota.Numero)) { }
                    nfs.NFSe.Documentos.Add(nota);
                }
            }

            if (tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos)
            {
                Dominio.Entidades.XMLCTe xmlCTe = repXMLCTe.BuscarPorCTe(nfse.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
                if (xmlCTe != null)
                    nfs.NFSe.XML = xmlCTe.XML;
            }

            if (tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos)
            {
                if (nfse.Status == "A" || nfse.Status == "C" || nfse.Status == "K")
                {
                    string nomeArquivo = nfse.Empresa.CNPJ + "_" + nfse.Numero.ToString() + "_" + nfse.Serie.Numero.ToString() + ".pdf";
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, nfse.Empresa.CNPJ, nomeArquivo);
                    byte[] danfse = null;
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        danfse = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                    else
                    {
                        Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                        danfse = svcNFSe.ObterDANFSECTe(nfse.Codigo, unitOfWork);
                    }

                    if (danfse != null)
                    {
                        if (codificarUTF8)
                            nfs.NFSe.PDF = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, danfse));
                        else
                            nfs.NFSe.PDF = Convert.ToBase64String(danfse);
                    }
                    else
                        nfs.NFSe.PDF = "";
                }
            }

            bool isUtilizarXCampoSomenteNoRedespacho = cargaCTe?.Carga?.TipoOperacao?.UtilizarXCampoSomenteNoRedespacho ?? false;
            bool isFlagXCampoValido = !isUtilizarXCampoSomenteNoRedespacho || (isUtilizarXCampoSomenteNoRedespacho && cargaCTe?.Carga?.Redespacho != null);
            if (!string.IsNullOrWhiteSpace(cargaCTe?.Carga?.TipoOperacao?.DocumentoXCampo) && !string.IsNullOrWhiteSpace(cargaCTe?.Carga?.TipoOperacao?.DocumentoXTexto) && isFlagXCampoValido)
            {
                nfs.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.CTe.Observacao>
                {
                    new Dominio.ObjetosDeValor.CTe.Observacao()
                    {
                        Identificador = cargaCTe.Carga.TipoOperacao.DocumentoXCampo,
                        Descricao = cargaCTe.Carga.TipoOperacao.DocumentoXTexto
                    }
                };
            }

            return nfs;
        }

        public Dominio.ObjetosDeValor.WebService.NFS.NFS ConverterObjetoCargaNFSComplementado(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.XMLNFSe repXMLNFSe = new Repositorio.XMLNFSe(unitOfWork);

            Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.Frete.ComponenteFrete serComponenteFrete = new Embarcador.Frete.ComponenteFrete(unitOfWork);
            Servicos.WebService.Empresa.Empresa serWSEmpresa = new Empresa.Empresa(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            Dominio.ObjetosDeValor.WebService.NFS.NFS nfs = new Dominio.ObjetosDeValor.WebService.NFS.NFS();

            nfs.LocalidadePrestacaoServico = serLocalidade.ConverterObjetoLocalidade(cargaCTeComplementoInfo.CTe.LocalidadeInicioPrestacao);
            nfs.NFSe = new Dominio.ObjetosDeValor.WebService.NFS.NFSe();
            nfs.NFSe.DataEmissao = cargaCTeComplementoInfo.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss");
            nfs.NFSe.MotivoCancelamento = cargaCTeComplementoInfo.CTe.ObservacaoCancelamento;
            nfs.NFSe.NaturezaNFSe = new Dominio.ObjetosDeValor.WebService.NFS.NaturezaNFSe();
            nfs.NFSe.NaturezaNFSe.Codigo = cargaCTeComplementoInfo.CTe.NaturezaNFSe != null ? cargaCTeComplementoInfo.CTe.NaturezaNFSe.Numero : 1;
            nfs.NFSe.NaturezaNFSe.Descricao = cargaCTeComplementoInfo.CTe.NaturezaNFSe != null ? cargaCTeComplementoInfo.CTe.NaturezaNFSe.Descricao : "PRESTAÇÃO DE SERVIÇO";
            nfs.NFSe.Numero = cargaCTeComplementoInfo.CTe.Numero;
            nfs.NFSe.NumeroProtocolo = cargaCTeComplementoInfo.Codigo.ToString();
            nfs.NFSe.NumeroRPS = cargaCTeComplementoInfo.CTe.RPS != null ? cargaCTeComplementoInfo.CTe.RPS.Numero : 0;
            nfs.NFSe.SerieRPS = cargaCTeComplementoInfo.CTe.RPS != null ? cargaCTeComplementoInfo.CTe.RPS.Serie : string.Empty;
            nfs.NFSe.CodigoVerificacao = !string.IsNullOrWhiteSpace(cargaCTeComplementoInfo.CTe.Protocolo) ? cargaCTeComplementoInfo.CTe.Protocolo : string.Empty;
            nfs.NFSe.Serie = cargaCTeComplementoInfo.CTe.Serie != null ? cargaCTeComplementoInfo.CTe.Serie.Numero : 0;

            if (cargaCTeComplementoInfo.CargaCTeComplementado?.CTe != null)
            {
                nfs.NFSe.NumeroNFSeComplementada = cargaCTeComplementoInfo.CargaCTeComplementado.CTe.Numero;
                nfs.NFSe.SerieNFSeComplementada = cargaCTeComplementoInfo.CargaCTeComplementado.CTe.Serie.Numero;
            }

            nfs.NFSe.XML = "";
            if (tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos)
            {
                Dominio.Entidades.XMLCTe xmlCTe = repXMLCTe.BuscarPorCTe(cargaCTeComplementoInfo.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
                if (xmlCTe != null)
                    nfs.NFSe.XML = xmlCTe.XML;
            }

            nfs.NFSe.PDF = "";
            if (tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.PDF || tipoDocumentoRetorno == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos)
            {
                if (cargaCTeComplementoInfo.CTe.Status == "A" || cargaCTeComplementoInfo.CTe.Status == "C" || cargaCTeComplementoInfo.CTe.Status == "K")
                {
                    string nomeArquivo = cargaCTeComplementoInfo.CTe.Empresa.CNPJ + "_" + cargaCTeComplementoInfo.CTe.Numero.ToString() + "_" + cargaCTeComplementoInfo.CTe.Serie.Numero.ToString() + ".pdf";
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, cargaCTeComplementoInfo.CTe.Empresa.CNPJ, nomeArquivo);
                    byte[] danfse = null;
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        danfse = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                    else
                    {
                        Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                        danfse = svcNFSe.ObterDANFSECTe(cargaCTeComplementoInfo.CTe.Codigo);
                    }

                    if (danfse != null)
                        nfs.NFSe.PDF = Convert.ToBase64String(danfse);
                    else
                        nfs.NFSe.PDF = "";
                }
            }
            nfs.Protocolo = cargaCTeComplementoInfo.CTe.Codigo;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCTeComplementoInfo.Codigo);
            nfs.ProtocolosDePedidos = new List<int>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosCTe)
            {
                nfs.ProtocolosDePedidos.Add(cargaPedido.Pedido.Protocolo/*cargaPedido.Pedido.Codigo*/);
            }
            nfs.TipoNotaFiscalServico = Dominio.Enumeradores.TipoNotaFiscalServico.Eletronica;
            nfs.Tomador = serWSPessoa.ConverterObjetoParticipamenteCTe(cargaCTeComplementoInfo.CTe.Tomador);
            nfs.TransportadoraEmitente = serWSEmpresa.ConverterObjetoEmpresa(cargaCTeComplementoInfo.CTe.Empresa);
            nfs.ValoresNFS = new Dominio.ObjetosDeValor.WebService.NFS.ValorNFS();
            nfs.ValoresNFS.AliquotaISS = cargaCTeComplementoInfo.CTe.AliquotaISS;
            nfs.ValoresNFS.BaseCalculoISS = cargaCTeComplementoInfo.CTe.BaseCalculoISS;
            nfs.ValoresNFS.ISSRetido = cargaCTeComplementoInfo.CTe.ISSRetido;
            nfs.ValoresNFS.ValorCOFINS = cargaCTeComplementoInfo.CTe.ValorCOFINS;
            nfs.ValoresNFS.ValorCSLL = cargaCTeComplementoInfo.CTe.ValorCSLL;
            nfs.ValoresNFS.ValorDeducoes = cargaCTeComplementoInfo.CTe.ValorDeducoes;
            nfs.ValoresNFS.ValorDescontoCondicionado = cargaCTeComplementoInfo.CTe.ValorDescontoCondicionado;
            nfs.ValoresNFS.ValorDescontoIncondicionado = cargaCTeComplementoInfo.CTe.ValorDescontoIncondicionado;
            nfs.ValoresNFS.ValorINSS = cargaCTeComplementoInfo.CTe.ValorINSS;
            nfs.ValoresNFS.ValorIR = cargaCTeComplementoInfo.CTe.ValorIR;
            nfs.ValoresNFS.ValorISS = cargaCTeComplementoInfo.CTe.ValorISS;
            nfs.ValoresNFS.ValorISSRetido = cargaCTeComplementoInfo.CTe.ValorISSRetido;
            nfs.ValoresNFS.ValorOutrasRetencoes = cargaCTeComplementoInfo.CTe.ValorOutrasRetencoes;
            nfs.ValoresNFS.ValorPIS = cargaCTeComplementoInfo.CTe.ValorPIS;
            nfs.ValoresNFS.ValorServicos = cargaCTeComplementoInfo.CTe.ValorAReceber;
            return nfs;
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico GerarNFSe(Dominio.ObjetosDeValor.WebService.NFS.NFS nfse, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorCFOP(5352, Dominio.Enumeradores.TipoCFOP.Saida);

            double cpfCnpjTomador = Utilidades.String.OnlyNumbers(nfse.Tomador.CPFCNPJ).ToDouble();

            if (cpfCnpjTomador <= 0D)
                throw new ServicoException($"Não foi possível converter o CPF/CNPJ do tomador ({nfse.Tomador.CPFCNPJ} - {nfse.Tomador.RazaoSocial})");

            Dominio.Entidades.Cliente tomador = ObterTomadorNFSe(nfse, unitOfWork);

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("39");
            if (modeloDocumentoFiscal == null)
                throw new ServicoException("Modelo de documento fiscal para NFS-e não encontrado.");

            if (empresa == null && nfse.TransportadoraEmitente != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(nfse.TransportadoraEmitente.CNPJ)))
                empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(nfse.TransportadoraEmitente.CNPJ));

            if (empresa == null)
                throw new ServicoException("Transportadora não encontrada.");

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = GerarNFSePorObjetoWebService(nfse, empresa, tomador, cfop, unitOfWork);

            return cte;
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico GerarNFSe(Dominio.ObjetosDeValor.WebService.CTe.CTe nfse, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorCFOP(5352, Dominio.Enumeradores.TipoCFOP.Saida);

            double cpfCnpjTomador = Utilidades.String.OnlyNumbers(nfse.Tomador.CPFCNPJ).ToDouble();

            if (cpfCnpjTomador <= 0D)
                throw new ServicoException($"Não foi possível converter o CPF/CNPJ do tomador ({nfse.Tomador.CPFCNPJ} - {nfse.Tomador.RazaoSocial})");

            Dominio.Entidades.Cliente tomador = ObterTomadorNFSe(nfse, unitOfWork);

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("39");
            if (modeloDocumentoFiscal == null)
                throw new ServicoException("Modelo de documento fiscal para NFS-e não encontrado.");

            if (empresa == null && nfse.TransportadoraEmitente != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(nfse.TransportadoraEmitente.CNPJ)))
                empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(nfse.TransportadoraEmitente.CNPJ));

            if (empresa == null)
                throw new ServicoException("Transportadora não encontrada.");

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = GerarNFSePorObjetoWebService(nfse, empresa, tomador, cfop, unitOfWork);

            return cte;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorCarga(RequestCtePorCarga dadosRequest, Dominio.Entidades.WebService.Integradora integradora)
        {
            if (dadosRequest.Limite > 50)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50.");

            if (dadosRequest.ProtocoloCarga == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de integração.");

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

            if (integradora.Empresa != null)
                carga = repCarga.BuscarPorCodigo(dadosRequest.ProtocoloCarga);
            else
                carga = repCarga.BuscarPorProtocolo(dadosRequest.ProtocoloCarga);

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS> objeto = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>();
            objeto.Itens = BuscarNFS(carga, dadosRequest.TipoDocumentoRetorno, dadosRequest.Inicio, dadosRequest.Limite, _unitOfWork);
            objeto.NumeroTotalDeRegistro = ContarNFS(carga, _unitOfWork);

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou NFS-es", _unitOfWork);

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoSucesso(objeto);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarNFSsCompletasPorCarga(RequestCtePorCarga dadosRequest, Dominio.Entidades.WebService.Integradora integradora)
        {
            if (dadosRequest.Limite > 50)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50.");

            if (dadosRequest.ProtocoloCarga == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de integração.");

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

            if (integradora.Empresa != null)
                carga = repCarga.BuscarPorCodigo(dadosRequest.ProtocoloCarga);
            else
                carga = repCarga.BuscarPorProtocolo(dadosRequest.ProtocoloCarga);

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe> objeto = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>();
            objeto.Itens = BuscarNFSCompleta(carga, dadosRequest.TipoDocumentoRetorno, dadosRequest.Inicio, dadosRequest.Limite, _unitOfWork);
            objeto.NumeroTotalDeRegistro = ContarNFSCompleta(carga, _unitOfWork);

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou NFS-es Completas", _unitOfWork);

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>>.CriarRetornoSucesso(objeto);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>> BuscarNFSesComplementaresAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite)
        {

            Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao);
            try
            {
                if (limite <= 50)
                {
                    Servicos.WebService.NFS.NFS serWSNFS = new Servicos.WebService.NFS.NFS(_unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

                    retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>();
                    retorno.Status = true;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargasCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarNFSsAguardandoIntegracao(inicio, limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repCargaCTeComplementoInfo.ContarNFSsAguardandoIntegracao();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar>();
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargasCTeComplementoInfo)
                    {
                        Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar nfsComplementar = new Dominio.ObjetosDeValor.WebService.NFS.NFSComplementar();
                        nfsComplementar.ProtocoloNFSComplementado = cargaCTeComplementoInfo.CargaCTeComplementado?.CTe?.Codigo ?? 0;
                        nfsComplementar.NFS = serWSNFS.ConverterObjetoCargaNFSComplementado(cargaCTeComplementoInfo, tipoDocumentoRetorno, unitOfWork);

                        if (cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia != null)
                        {
                            nfsComplementar.Ocorrencia = new Dominio.ObjetosDeValor.WebService.Ocorrencia.Ocorrencia
                            {
                                Protocolo = cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia.Codigo,
                                CodigoIntegracao = cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia.CodigoProceda,
                                Descricao = cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia.Descricao,
                                ProtocoloCarga = cargaCTeComplementoInfo.CargaOcorrencia.Carga.Protocolo,
                                NumeroCargaEmbarcador = cargaCTeComplementoInfo.CargaOcorrencia.Carga.CodigoCargaEmbarcador,
                                NumeroOcorrencia = cargaCTeComplementoInfo.CargaOcorrencia.NumeroOcorrencia
                            };

                            nfsComplementar.Ocorrencia.Pedidos = new List<Dominio.ObjetosDeValor.WebService.Ocorrencia.Pedido>();
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaCTeComplementoInfo.CargaOcorrencia.Carga.Pedidos)
                            {
                                Dominio.ObjetosDeValor.WebService.Ocorrencia.Pedido pedido = new Dominio.ObjetosDeValor.WebService.Ocorrencia.Pedido();
                                pedido.NumeroEmbarcador = cargaPedido.Pedido.NumeroPedidoEmbarcador;
                                pedido.Protocolo = cargaPedido.Pedido.Protocolo;
                                nfsComplementar.Ocorrencia.Pedidos.Add(pedido);
                            }
                        }

                        retorno.Objeto.Itens.Add(nfsComplementar);
                    }

                    Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou NFS-es complementares aguardando integração", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Por favor, informe os códigos de integração.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar NFSs complementares";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoNFSComplementar(int protocoloNFS)
        {

            Dominio.ObjetosDeValor.WebService.Retorno<bool> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao);

            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            retorno.Status = true;
            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorCTe(protocoloNFS);

                if (cargaCTeComplementoInfo != null)
                {
                    if (!cargaCTeComplementoInfo.ComplementoIntegradoEmbarcador)
                    {
                        cargaCTeComplementoInfo.ComplementoIntegradoEmbarcador = true;
                        repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);

                        Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCTeComplementoInfo, "Confirmou integração NFS-e complementar", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Objeto = true;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                        retorno.Mensagem = "A integração já foi confirmada anteriormente.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O protocólo do CT-e informado não existe na base da Multisoftware";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar confirmar a integração do NFS complementar.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorOcocorrencia(RequestNFSOcorrencia dadosRequest, Dominio.Entidades.WebService.Integradora integradora)
        {
            if (dadosRequest.Limite > 50)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 50.");

            if (dadosRequest.ProtocoloOcorrencia == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoDadosInvalidos("É obrigatório informar o protocolo de integração.");

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOocorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = null;

            cargaOcorrencia = repCargaOocorrencia.BuscarPorCodigo(dadosRequest.ProtocoloOcorrencia);

            if (cargaOcorrencia == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoDadosInvalidos("Registro da ocorrencia não encontrada.");


            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS> objeto = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>();
            objeto.Itens = BuscarNFS(cargaOcorrencia.Carga, dadosRequest.TipoDocumentoRetorno, dadosRequest.Inicio, dadosRequest.Limite, _unitOfWork);
            objeto.NumeroTotalDeRegistro = ContarNFS(cargaOcorrencia.Carga, _unitOfWork);

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou NFS-es", _unitOfWork);

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>.CriarRetornoSucesso(objeto);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>> ConsultarDocumentosParaEmissaoDeNotaManual(Dominio.Entidades.WebService.Integradora integradora)
        {

            if (integradora?.Empresa == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>>.CriarRetornoDadosInvalidos("Dados inválidos para esta integração.");

            Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual()
            {
                CodigoTransportador = integradora.Empresa.Codigo
            };

            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual> objeto = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>()
            {
                NumeroTotalDeRegistro = repCargaDocumentoParaEmissaoNFSManual.ContarConsultaSelecaoNFSManual(filtrosPesquisa),
                Itens = new List<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>()
            };

            if (objeto.NumeroTotalDeRegistro == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>>.CriarRetornoSucesso(objeto, "Não Foram Encontrados Documentos Pendentes");

            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissao = repCargaDocumentoParaEmissaoNFSManual.ConsultarSelecaoNFSManual(filtrosPesquisa, null);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documentoParaEmissao in documentosParaEmissao)
                objeto.Itens.Add(ConverterDocumentoPendenteNotaManual(documentoParaEmissao));

            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Consultou Documentos Para Emissão De Nota Manual", _unitOfWork);

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual>>.CriarRetornoSucesso(objeto);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<int> GerarNotaManual(GerarNotaManual gerarNotaManual)
        {
            try
            {
                if (gerarNotaManual?.Nfse == null)
                    throw new ControllerException("NFS-e é Obrigatória");

                if (gerarNotaManual?.ProtocolosDocumentos == null)
                    throw new ControllerException("Protocolos Documentos são Obrigatórios");

                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(_unitOfWork);
                Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);
                Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(_unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(_unitOfWork);

                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentos = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCodigos(gerarNotaManual.ProtocolosDocumentos);

                if (documentos.Count == 0)
                    throw new ControllerException("Nenhum documento selecionado.");

                if (documentos.Any(o => o.Carga.SituacaoCarga == SituacaoCarga.Cancelada || o.Carga.SituacaoCarga == SituacaoCarga.Anulada))
                    throw new ControllerException("Enquanto as notas estavam sendo selecionadas uma carga das notas selecionadas foi cancelada. Por favor, refaça o processo.");

                if (documentos.Where(o => o.CargaOcorrencia != null).Any(o => o.CargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.Cancelada || o.CargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.Anulada))
                    throw new ControllerException("Enquanto as notas estavam sendo selecionadas uma ocorrência das notas selecionadas foi cancelada. Por favor, refaça o processo.");

                if (documentos.Any(o => o.FechamentoFrete == null) && documentos.Any(o => o.FechamentoFrete != null))
                    throw new ControllerException("Não é possivel gerar NFS manual com documentos sem fechamento quando existem documentos com fechamento.");

                List<MoedaCotacaoBancoCentral> moedas = documentos.Select(o => o.Moeda ?? MoedaCotacaoBancoCentral.Real).Distinct().ToList();

                if (moedas.Count > 1)
                    throw new ControllerException($"Não é possível gerar uma NFS manual com mais de um tipo de moeda ({string.Join(", ", moedas.Select(o => o.ObterDescricao()))}).");

                bool primeiroDocumentoEhComplemento = documentos[0]?.CargaOcorrencia != null;
                bool temComplementosMisturadosComNormais = documentos.Any(o => (o.CargaOcorrencia != null) != primeiroDocumentoEhComplemento);

                if (temComplementosMisturadosComNormais)
                    throw new ControllerException($"Não é possível gerar uma NFS manual misturando documentos que são complementos de ocorrência com que não são.");

                _unitOfWork.Start();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(_unitOfWork);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                if (configuracaoCargaEmissaoDocumento?.NaoPermitirNFSComMultiplosCentrosResultado ?? false)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = documentos.Where(o => o.PedidoXMLNotaFiscal != null && o.PedidoCTeParaSubContratacao == null).Select(o => o.PedidoXMLNotaFiscal.CargaPedido).Distinct().ToList();
                    cargaPedidos.AddRange(documentos.Where(o => o.PedidoCTeParaSubContratacao != null && o.PedidoXMLNotaFiscal == null && o.CargaCTe == null).Select(o => o.PedidoCTeParaSubContratacao.CargaPedido).Distinct().ToList());
                    cargaPedidos.AddRange(documentos.Where(o => o.CargaCTe != null && o.PedidoXMLNotaFiscal == null && o.PedidoCTeParaSubContratacao == null).Select(o => o.CargaCTe.NotasFiscais.Select(p => p.PedidoXMLNotaFiscal.CargaPedido)).SelectMany(o => o).Distinct().ToList());

                    if (cargaPedidos.Select(o => o.Pedido.CentroResultado).Distinct().Count() > 1)
                        throw new ControllerException("Existe mais de um centro de resultado nos documentos selecionados, não sendo possível gerar a NFS.");
                }



                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual referencia = documentos[0];

                Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS = new Dominio.Entidades.Embarcador.NFS.DadosNFSManual()
                {
                    Numero = gerarNotaManual.Nfse.Numero,
                    ValorFrete = documentos.Sum(obj => obj.ValorFrete),
                    ValorReceber = documentos.Sum(obj => obj.ValorFrete),
                    DataEmissao = gerarNotaManual.Nfse.DataEmissao.ToDateTime(),
                    AliquotaISS = referencia.PercentualAliquotaISS,
                    ValorBaseCalculo = referencia.BaseCalculoISS,
                    ValorISS = referencia.ValorISS,
                    NumeroRPS = repDadosNFSManual.BuscarProximoNumeroRPS(),
                    TipoArredondamentoISS = TipoArredondamentoNFSManual.Normal,
                    ValorTotalMoeda = documentos.Sum(obj => obj.ValorTotalMoeda ?? 0m),
                    Moeda = moedas.FirstOrDefault(),
                };

                if (_tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                {
                    if (configuracaoEmbarcador.PadraoInclusaiISSDesmarcado)
                        dadosNFS.IncluirISSBC = false;
                    else
                        dadosNFS.IncluirISSBC = true;
                    if (documentos != null && documentos.Count > 0)
                    {
                        if (configuracaoEmbarcador.PossuiWMS)
                            dadosNFS.Observacoes = "Minutas " + string.Join(" ", (from o in documentos select o.Numero.ToString("D")).ToList());
                        else
                            dadosNFS.Observacoes = "DESCARGA DE MERCADORIA MIN.: " + string.Join(", ", (from o in documentos select o.Numero.ToString("D")).ToList());
                    }
                }

                Dominio.Entidades.Empresa transportador = referencia.Carga.Empresa;
                Dominio.Entidades.Embarcador.Filiais.Filial filial = referencia.Carga.Filial;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = referencia.Carga.TipoOperacao;
                Dominio.Entidades.Cliente tomador = referencia.Tomador;

                Dominio.Entidades.EmpresaSerie empresaSerie = repEmpresaSerie.BuscarPorEmpresaTipo(transportador.Codigo, Dominio.Enumeradores.TipoSerie.NFSe);
                if (empresaSerie != null)
                    dadosNFS.Serie = empresaSerie;

                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = new Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual()
                {
                    Transportador = transportador,
                    Situacao = SituacaoLancamentoNFSManual.DadosNota,
                    Tomador = tomador,
                    Filial = filial,
                    TipoOperacao = tipoOperacao,
                    FechamentoFrete = referencia.FechamentoFrete,
                    DadosNFS = dadosNFS,
                    LocalidadePrestacao = referencia.LocalidadePrestacao,
                    NFSResidual = referencia.DocResidual,
                    DataCriacao = DateTime.Now,
                    CargasMultiCTe = (from o in documentos where o.DocumentosNFSe != null select o).Count() > 0,
                };

                repDadosNFSManual.Inserir(dadosNFS);
                repLancamentoNFSManual.Inserir(nfsManual, _auditado);


                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, nfsManual.Tomador.Localidade.Codigo, nfsManual.Tomador.Localidade?.Estado?.Sigla ?? "", nfsManual.Tomador.GrupoPessoas?.Codigo ?? 0, nfsManual.Tomador.Localidade?.Codigo ?? 0);

                if (configuracaoNFSe == null)
                    configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, nfsManual.Tomador.Localidade.Codigo, nfsManual.Tomador.Localidade?.Estado?.Sigla ?? "", 0, 0);
                if (configuracaoNFSe == null)
                    configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, nfsManual.Tomador.Localidade.Codigo, "", nfsManual.Tomador.GrupoPessoas?.Codigo ?? 0, 0);
                if (configuracaoNFSe == null)
                    configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, nfsManual.Tomador.Localidade.Codigo, "", 0, nfsManual.Tomador.Localidade?.Codigo ?? 0);
                if (configuracaoNFSe == null)
                    configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, nfsManual.Tomador.Localidade.Codigo, "", 0, 0);
                if (configuracaoNFSe == null)
                    configuracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(nfsManual.Transportador.Codigo, 0, "", 0, 0);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documento in documentos)
                {
                    documento.LancamentoNFSManual = nfsManual;
                    repCargaDocumentoParaEmissaoNFSManual.Atualizar(documento);
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, documento, null, "Criou uma NFS-e manual com o documento.", _unitOfWork);
                }

                AdicionarDescontos(nfsManual, referencia, configuracaoEmbarcador, _unitOfWork);
                Servicos.Embarcador.NFSe.NFSManual.CalcularValores(dadosNFS, _unitOfWork);
                Servicos.Embarcador.NFSe.NFSManual.CalcularISS(dadosNFS);

                decimal baseCalculoIBSCBS = dadosNFS.ValorFrete;

                if (dadosNFS.IncluirISSBC)
                    baseCalculoIBSCBS += dadosNFS.ValorISS;

                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                {
                    BaseCalculo = baseCalculoIBSCBS,
                    ValoAbaterBaseCalculo = dadosNFS.ValorISS,
                    CodigoLocalidade = nfsManual.Tomador.Localidade.Codigo,
                    SiglaUF = nfsManual.Tomador.Localidade.Estado.Sigla,
                    CodigoTipoOperacao = 0,
                    Empresa = transportador
                });

                dadosNFS.NBS = configuracaoNFSe?.ServicoNFSe?.NBS ?? "";
                dadosNFS.IndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;
                dadosNFS.CSTIBSCBS = impostoIBSCBS.CST;
                dadosNFS.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributaria;
                dadosNFS.BaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
                dadosNFS.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
                dadosNFS.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
                dadosNFS.ValorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                dadosNFS.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
                dadosNFS.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
                dadosNFS.ValorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                dadosNFS.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
                dadosNFS.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
                dadosNFS.ValorCBS = impostoIBSCBS.ValorCBS;


                repDadosNFSManual.Atualizar(dadosNFS);

                // Integracao com SignalR
                svcNFSManual.InformarLancamentoNFSManualAtualizada(nfsManual.Codigo);

                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoSucesso(nfsManual.Codigo);
            }
            catch (ControllerException excecao)
            {
                _unitOfWork.Rollback();
                Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoExcecao(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao adicionar dados.");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<int> EnviarXMLNFSManual(int protocolo, string nfseBase64)
        {
            return Anexar(protocolo, nfseBase64, "XML");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<int> EnviarPDFNFSManual(int protocolo, string pdfBase64)
        {
            return Anexar(protocolo, pdfBase64, "DANFSE");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.PreNFSe>> BuscarPreNFSeAguardandoEmissao(DateTime dataInicioEmissao, DateTime dataFimEmissao, string numeroCarga)
        {
            Repositorio.PreConhecimentoDeTransporteEletronico repositorioPreNFSe = new Repositorio.PreConhecimentoDeTransporteEletronico(_unitOfWork);
            List<int> codigosIbgePermitidos = new List<int>() { 5300108 };
            List<Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.PreNFSe> listaPreNFSe = repositorioPreNFSe.BuscarPreNFSeAguardandoEmissao(dataInicioEmissao, dataFimEmissao, numeroCarga, codigosIbgePermitidos);

            return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.PreNFSe>>.CriarRetornoSucesso(listaPreNFSe);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> EnviarNFSeEmitidas(List<Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.NFSe> notasFiscaisServico)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(_unitOfWork);
            Servicos.Embarcador.Carga.PreCTe servicoCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(_unitOfWork);
            Servicos.PreCTe servicoPreCTe = new Servicos.PreCTe(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            foreach (Dominio.ObjetosDeValor.WebService.NFS.PreNFSe.NFSe nfse in notasFiscaisServico)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(nfse.CodigoIdentificacao))
                        throw new ServicoException($"O código de identificação deve ser informado.");

                    byte[] pdfDanfse = string.IsNullOrWhiteSpace(nfse.Pdf) ? null : Convert.FromBase64String(nfse.Pdf);
                    string xml = string.IsNullOrWhiteSpace(nfse.Xml) ? string.Empty : Encoding.UTF8.GetString(Convert.FromBase64String(nfse.Xml));

                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorCodigo(nfse.CodigoIdentificacao.ToInt());

                    if (cargaCTe == null)
                        throw new ServicoException($"Não foi possível localizar a pré NFS-e com o código de identificação {nfse.CodigoIdentificacao}.");

                    servicoPreCTe.GerarNFSPorPreCTe(cargaCTe.PreCTe, cargaCTe, nfse.Numero, nfse.Serie, xml, pdfDanfse, nfse.DataEmissao, nfse.AliquotaIss, nfse.BaseCalculoIss, nfse.ValorIss, nfse.PercentualRetencaoIss, nfse.ValorIssRetido, nfse.ValorPis, nfse.ValorCofins, nfse.ValorIr, nfse.ValorCsll, cargaCTe.PreCTe.ValorFrete, nfse.ValorPrestacaoServico, nfse.ValorReceber, nfse.NumeroRps, nfse.Observacao, configuracaoEmbarcador, _tipoServicoMultisoftware);
                    servicoCargaPreCTe.VerificarEnviouTodosDocumentos(_unitOfWork, cargaCTe.Carga, _tipoServicoMultisoftware, configuracaoEmbarcador);
                    servicoCargaCTe.EnviarEmailPreviaDocumentosCargaCte(cargaCTe.Carga.Codigo, cargaCTe);

                    _unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao);
                }
            }

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> BuscarNFSsPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite, string codigoTipoOperacao, string situacao, Dominio.Entidades.WebService.Integradora integradora)
        {

            Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            try
            {
                if (limite <= 50)
                {
                    DateTime _dataInicial = dataInicial.ToDateTime();
                    DateTime _dataFinal = dataFinal.ToDateTime();

                    if (_dataInicial != DateTime.MinValue && _dataFinal != DateTime.MinValue)
                    {
                        Servicos.WebService.NFS.NFS serWSNFS = new Servicos.WebService.NFS.NFS(_unitOfWork);
                        string mensagem = "";

                        retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.NFS.NFS>
                        {
                            Itens = serWSNFS.BuscarNFSPeriodo(_dataInicial, _dataFinal, integradora.Empresa?.Codigo ?? 0, tipoDocumentoRetorno, inicio, limite, codigoTipoOperacao, situacao, ref mensagem),
                            NumeroTotalDeRegistro = serWSNFS.ContarNFSPeriodo(_dataInicial, _dataFinal, integradora.Empresa?.Codigo ?? 0, codigoTipoOperacao, situacao)
                        };
                        retorno.Status = true;

                        if (!string.IsNullOrWhiteSpace(mensagem))
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = mensagem;
                        }
                        else
                            Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou NFS-es", _unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "É obrigatório informar a data inicial e a data final.";
                    }
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 50";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as Notas Fiscais de Serviço";
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> BuscarNFSAguardandoIntegracao(int inicio, int limite)
        {
            Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<int>>();

            try
            {
                if (limite <= 100)
                {
                    Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(_unitOfWork);

                    List<int> codigosCTes = repLancamentoNFSManual.BuscarNFSManualPendentesConfirmacao(inicio, limite);
                    retorno.Objeto = new Paginacao<int>();
                    retorno.Objeto.NumeroTotalDeRegistro = repLancamentoNFSManual.ContarNFSManualPendentesConfirmacao();
                    retorno.Objeto.Itens = new List<int>();
                    foreach (var codigo in codigosCTes)
                    {
                        retorno.Objeto.Itens.Add(codigo);
                    }
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou NFS Manual aguardando confirmação de integração", _unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as NFS Manuais";
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.NFS.NFS> BuscarNFSPorProtocolo(int protocoloNFS, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno tipoDocumentoRetorno)
        {
            Retorno<Dominio.ObjetosDeValor.WebService.NFS.NFS> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.NFS.NFS>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            try
            {
                if (protocoloNFS > 0)
                {
                    Servicos.WebService.NFS.NFS serWSNFS = new Servicos.WebService.NFS.NFS(_unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                    Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(_unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(protocoloNFS);
                    if (cargaCTe != null && (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS))
                    {
                        List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTe(cargaCTe.CTe.Codigo);
                        retorno.Objeto = serWSNFS.ConverterObjetoCargaNFS(cargaCTe, cTeContaContabilContabilizacaos, tipoDocumentoRetorno, configuracaoTMS.UtilizarCodificacaoUTF8ConversaoPDF, _unitOfWork);
                        retorno.Status = true;

                        Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou NFS-e por protocolo", _unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O protocolo informado não é de uma Nota de Serviço existente.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Por favor, informe o Protocolo da Nota de Serviço.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Nota de Serviço";
            }

            return retorno;
        }
        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Cliente ObterTomadorNFSe(Dominio.ObjetosDeValor.WebService.NFS.NFS nfse, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(Utilidades.String.OnlyNumbers(nfse.Tomador.CPFCNPJ).ToDouble());

            if (tomador != null)
                return tomador;

            Cliente svcCliente = new Cliente();
            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = svcCliente.ConverterObjetoValorPessoa(nfse.Tomador, "", unitOfWork);

            if (!retorno.Status)
                throw new ServicoException("Não foi possível converter o cliente da NFS-e: " + retorno.Mensagem);

            return retorno.cliente;
        }

        private Dominio.Entidades.Cliente ObterTomadorNFSe(Dominio.ObjetosDeValor.WebService.CTe.CTe nfse, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(Utilidades.String.OnlyNumbers(nfse.Tomador.CPFCNPJ).ToDouble());

            if (tomador != null)
                return tomador;

            Cliente svcCliente = new Cliente();
            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = svcCliente.ConverterObjetoValorPessoa(nfse.Tomador, "", unitOfWork);

            if (!retorno.Status)
                throw new ServicoException("Não foi possível converter o cliente da NFS-e: " + retorno.Mensagem);

            return retorno.cliente;
        }

        private Dominio.Entidades.ConhecimentoDeTransporteEletronico GerarNFSePorObjetoWebService(Dominio.ObjetosDeValor.WebService.NFS.NFS nfse, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente tomador, Dominio.Entidades.CFOP cfop, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.RPSNFSe repRPSNFSe = new Repositorio.RPSNFSe(unitOfWork);
            Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
            Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("39");

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = new Dominio.Entidades.ConhecimentoDeTransporteEletronico()
            {
                AliquotaISS = nfse.ValoresNFS.AliquotaISS,
                Empresa = empresa,
                Cancelado = "N",
                CFOP = cfop,
                NaturezaDaOperacao = cfop.NaturezaDaOperacao,
                DataEmissao = nfse.NFSe.DataEmissao.ToDateTime(),
                DataAutorizacao = nfse.NFSe.DataEmissao.ToDateTime(),
                ValorFrete = nfse.ValoresNFS.BaseCalculoISS,
                ValorAReceber = nfse.ValoresNFS.BaseCalculoISS - nfse.ValoresNFS.ValorISSRetido,
                ValorPrestacaoServico = nfse.ValoresNFS.BaseCalculoISS,
                BaseCalculoISS = nfse.ValoresNFS.BaseCalculoISS,
                ValorCOFINS = nfse.ValoresNFS.ValorCOFINS,
                ValorDeducoes = nfse.ValoresNFS.ValorDeducoes,
                ValorCSLL = nfse.ValoresNFS.ValorCSLL,
                ValorDescontoCondicionado = nfse.ValoresNFS.ValorDescontoCondicionado,
                ValorDescontoIncondicionado = nfse.ValoresNFS.ValorDescontoIncondicionado,
                ValorINSS = nfse.ValoresNFS.ValorINSS,
                ValorIR = nfse.ValoresNFS.ValorIR,
                ValorISS = nfse.ValoresNFS.ValorISS,
                ValorPIS = nfse.ValoresNFS.ValorPIS,
                ValorISSRetido = nfse.ValoresNFS.ValorISSRetido,
                ValorOutrasRetencoes = nfse.ValoresNFS.ValorOutrasRetencoes,
                ISSRetido = nfse.ValoresNFS.ISSRetido,
                LocalidadeEmissao = empresa.Localidade,
                LocalidadeInicioPrestacao = tomador.Localidade,
                LocalidadeTerminoPrestacao = tomador.Localidade,
                ModalTransporte = repModalTransporte.BuscarPorCodigo(1, false),
                Numero = nfse.NFSe.Numero,
                Protocolo = nfse.NFSe.CodigoVerificacao,
                MensagemRetornoSefaz = nfse.UltimoRetornoSEFAZ,
                Serie = svcCTe.ObterSerie(empresa, nfse.NFSe.Serie, Dominio.Enumeradores.TipoSerie.NFSe, unitOfWork),
                ModeloDocumentoFiscal = modeloDocumentoFiscal,
                ProdutoPredominante = "Diversos",
                TipoCTE = nfse.NFSe.NumeroNFSeComplementada != 0 ? Dominio.Enumeradores.TipoCTE.Complemento : Dominio.Enumeradores.TipoCTE.Normal,
                TipoAmbiente = empresa?.TipoAmbiente ?? TipoAmbiente.Producao,
                TipoEmissao = "1",
                TipoEnvio = 0,
                TipoImpressao = Dominio.Enumeradores.TipoImpressao.Retrato,
                TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario,
                TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago,
                TipoServico = Dominio.Enumeradores.TipoServico.Normal,
                TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente,
                Status = "A",
                TipoControle = repCTe.BuscarUltimoTipoControle() + 1
            };

            if (nfse.ValoresNFS.IBSCBS != null)
            {
                Dominio.ObjetosDeValor.CTe.IBSCBS impostoIBSCBS = nfse.ValoresNFS.IBSCBS;
                cte.NBS = impostoIBSCBS.NBS;
                cte.CodigoIndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;
                cte.CSTIBSCBS = impostoIBSCBS.CSTIBSCBS;
                cte.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributariaIBSCBS;
                cte.BaseCalculoIBSCBS = impostoIBSCBS.BaseCalculoIBSCBS;
                cte.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
                cte.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
                cte.ValorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                cte.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
                cte.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
                cte.ValorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                cte.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
                cte.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
                cte.ValorCBS = impostoIBSCBS.ValorCBS;
            }

            Dominio.Entidades.RPSNFSe rpsNFSe = new Dominio.Entidades.RPSNFSe()
            {
                Empresa = empresa,
                Numero = nfse.NFSe.NumeroRPS,
                Serie = nfse.NFSe.SerieRPS,
                Status = "A"
            };

            repRPSNFSe.Inserir(rpsNFSe);

            cte.RPS = rpsNFSe;
            cte.SetarParticipante(tomador, Dominio.Enumeradores.TipoTomador.Remetente);
            cte.SetarParticipante(tomador, Dominio.Enumeradores.TipoTomador.Destinatario);

            Servicos.CTe.SetarTomadorPagadorCTe(ref cte);

            repCTe.Inserir(cte);

            for (int i = 0; i < nfse.NFSe.Documentos.Count; i++)
            {
                Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe documentoNFSe = nfse.NFSe.Documentos[i];

                Dominio.Entidades.DocumentosCTE documentoCTe = new Dominio.Entidades.DocumentosCTE()
                {
                    CTE = cte,
                    ChaveNFE = documentoNFSe.ChaveNFe,
                    Numero = documentoNFSe.Numero,
                    Serie = documentoNFSe.Serie,
                    DataEmissao = cte.DataEmissao.Value,
                    ModeloDocumentoFiscal = !string.IsNullOrWhiteSpace(documentoNFSe.ChaveNFe) && documentoNFSe.ChaveNFe.Length == 44 ? repModeloDocumentoFiscal.BuscarPorModelo("55") : repModeloDocumentoFiscal.BuscarPorModelo("99")
                };

                if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                    documentoCTe.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(documentoCTe.ChaveNFE);

                repDocumentoCTe.Inserir(documentoCTe);
            }

            if (!string.IsNullOrWhiteSpace(nfse.NFSe.XML))
            {
                Dominio.Entidades.XMLCTe xmlCTe = new Dominio.Entidades.XMLCTe()
                {
                    CTe = cte,
                    Tipo = Dominio.Enumeradores.TipoXMLCTe.Autorizacao,
                    XML = nfse.NFSe.XML
                };

                repXMLCTe.Inserir(xmlCTe);
            }

            if (nfse.NFSe.NumeroNFSeComplementada != 0)
            {
                Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);

                Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                {
                    CTe = cte,
                    Chave = null,
                    Numero = nfse.NFSe.NumeroNFSeComplementada,
                    Serie = nfse.NFSe.SerieNFSeComplementada != 0 ? nfse.NFSe.SerieNFSeComplementada.ToString() : cte.Serie?.Numero.ToString(),
                    DataEmissao = null
                };

                repDocumentoOriginario.Inserir(documentoOriginario);
            }

            return cte;
        }

        private Dominio.Entidades.ConhecimentoDeTransporteEletronico GerarNFSePorObjetoWebService(Dominio.ObjetosDeValor.WebService.CTe.CTe nfse, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente tomador, Dominio.Entidades.CFOP cfop, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.RPSNFSe repRPSNFSe = new Repositorio.RPSNFSe(unitOfWork);
            Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
            Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unitOfWork);
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("39");

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = new Dominio.Entidades.ConhecimentoDeTransporteEletronico()
            {
                AliquotaISS = nfse.ValorFrete.ISS?.Aliquota ?? 0m,
                Empresa = empresa,
                Cancelado = "N",
                CFOP = cfop,
                NaturezaDaOperacao = cfop.NaturezaDaOperacao,
                DataEmissao = nfse.DataEmissao.ToDateTime(),
                DataAutorizacao = (string.IsNullOrEmpty(nfse.DataAutorizacao) ? nfse.DataEmissao : nfse.DataAutorizacao).ToDateTime(),
                ValorFrete = nfse.ValorFrete.FreteProprio,
                ValorAReceber = nfse.ValorFrete.ValorTotalAReceber,
                ValorPrestacaoServico = nfse.ValorFrete.ValorPrestacaoServico,
                BaseCalculoISS = nfse.ValorFrete.ISS?.ValorBaseCalculoISS ?? 0m,
                ValorISS = nfse.ValorFrete.ISS?.ValorISS ?? 0m,
                ValorISSRetido = nfse.ValorFrete.ISS?.ValorRetencaoISS ?? 0m,
                ISSRetido = nfse.ValorFrete.ISS?.PercentualRetencao > 0m,
                LocalidadeEmissao = empresa.Localidade,
                LocalidadeInicioPrestacao = tomador.Localidade,
                LocalidadeTerminoPrestacao = tomador.Localidade,
                ModalTransporte = repModalTransporte.BuscarPorCodigo(1, false),
                Numero = nfse.Numero,
                Protocolo = nfse.ProtocoloAutorizacao,
                MensagemRetornoSefaz = nfse.MensagemRetornoSefaz,
                Serie = svcCTe.ObterSerie(empresa, nfse.Serie, Dominio.Enumeradores.TipoSerie.NFSe, unitOfWork),
                ModeloDocumentoFiscal = modeloDocumentoFiscal,
                ProdutoPredominante = nfse.ProdutoPredominante,
                TipoCTE = nfse.TipoCTE,
                TipoAmbiente = empresa?.TipoAmbiente ?? TipoAmbiente.Producao,
                TipoEmissao = "1",
                TipoEnvio = 0,
                TipoImpressao = Dominio.Enumeradores.TipoImpressao.Retrato,
                TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario,
                TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago,
                TipoServico = Dominio.Enumeradores.TipoServico.Normal,
                TipoTomador = TipoTomador.Remetente,
                Status = "A",
                TipoControle = repCTe.BuscarUltimoTipoControle() + 1
            };

            Dominio.Entidades.RPSNFSe rpsNFSe = new Dominio.Entidades.RPSNFSe()
            {
                Empresa = empresa,
                Numero = nfse.NumeroRPS ?? 0,
                Serie = nfse.SerieRPS,
                Status = "A"
            };

            repRPSNFSe.Inserir(rpsNFSe);

            cte.RPS = rpsNFSe;
            cte.SetarParticipante(tomador, Dominio.Enumeradores.TipoTomador.Remetente);
            cte.SetarParticipante(tomador, Dominio.Enumeradores.TipoTomador.Destinatario);

            Servicos.CTe.SetarTomadorPagadorCTe(ref cte);

            repCTe.Inserir(cte);

            for (int i = 0; i < nfse.Documentos.Count; i++)
            {
                Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe documentoNFSe = nfse.Documentos[i];

                Dominio.Entidades.DocumentosCTE documentoCTe = new Dominio.Entidades.DocumentosCTE()
                {
                    CTE = cte,
                    ChaveNFE = documentoNFSe.ChaveNFe,
                    Numero = documentoNFSe.Numero,
                    Serie = documentoNFSe.Serie,
                    DataEmissao = cte.DataEmissao.Value,
                    ModeloDocumentoFiscal = !string.IsNullOrWhiteSpace(documentoNFSe.ChaveNFe) && documentoNFSe.ChaveNFe.Length == 44 ? repModeloDocumentoFiscal.BuscarPorModelo("55") : repModeloDocumentoFiscal.BuscarPorModelo("99")
                };

                if (!string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                    documentoCTe.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(documentoCTe.ChaveNFE);

                repDocumentoCTe.Inserir(documentoCTe);
            }

            for (int i = 0; i < nfse.ValorFrete.ComponentesAdicionais.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional = nfse.ValorFrete.ComponentesAdicionais[i];

                Dominio.Entidades.ComponentePrestacaoCTE componentePrestacaoCTE = new Dominio.Entidades.ComponentePrestacaoCTE()
                {
                    CTE = cte,
                    IncluiNaBaseDeCalculoDoICMS = componenteAdicional.IncluirBaseCalculoICMS,
                    IncluiNoTotalAReceber = componenteAdicional.IncluirTotalReceber,
                    NomeCTe = componenteAdicional.Componente.Descricao,
                    Valor = componenteAdicional.ValorComponente
                };

                repComponentePrestacaoCTe.Inserir(componentePrestacaoCTE);
            }

            if (!string.IsNullOrWhiteSpace(nfse.XMLAutorizacao))
            {
                Dominio.Entidades.XMLCTe xmlCTe = new Dominio.Entidades.XMLCTe()
                {
                    CTe = cte,
                    Tipo = Dominio.Enumeradores.TipoXMLCTe.Autorizacao,
                    XML = nfse.XMLAutorizacao
                };

                repXMLCTe.Inserir(xmlCTe);
            }

            return cte;
        }

        private Dominio.ObjetosDeValor.WebService.Ocorrencia.Ocorrencia ConverterObjetoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia)
        {
            if (cargaOcorrencia == null)
                return null;

            Dominio.ObjetosDeValor.WebService.Ocorrencia.Ocorrencia ocorrencia = new Dominio.ObjetosDeValor.WebService.Ocorrencia.Ocorrencia
            {
                Protocolo = cargaOcorrencia.TipoOcorrencia?.Codigo ?? 0,
                CodigoIntegracao = cargaOcorrencia.TipoOcorrencia?.CodigoProceda ?? string.Empty,
                Descricao = cargaOcorrencia.TipoOcorrencia?.Descricao ?? string.Empty,
                ProtocoloCarga = cargaOcorrencia.Carga?.Protocolo ?? 0,
                NumeroCargaEmbarcador = cargaOcorrencia.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                NumeroOcorrencia = cargaOcorrencia?.NumeroOcorrencia ?? 0,
            };

            return ocorrencia;
        }

        private Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual ConverterDocumentoPendenteNotaManual(Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documentoEmissaoNFSManual)
        {
            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(_unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(_unitOfWork);
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);

            Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual documentoPendente = new Dominio.ObjetosDeValor.WebService.NFS.DocumentoPendenteNotaManual();
            if (documentoEmissaoNFSManual.PedidoXMLNotaFiscal != null)
                documentoPendente.NFe = serNFe.ConverterXMLEmNota(documentoEmissaoNFSManual.PedidoXMLNotaFiscal.XMLNotaFiscal, "", _unitOfWork);
            if (documentoEmissaoNFSManual.CTe != null)
                documentoPendente.CTeAnterior = serCTe.ConverterObjetoCTe(documentoEmissaoNFSManual.CTe, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, _unitOfWork, false);
            documentoPendente.Tomador = serPessoa.ConverterObjetoPessoa(documentoEmissaoNFSManual.Tomador);
            documentoPendente.LocalidadePrestacao = serLocalidade.ConverterObjetoLocalidade(documentoEmissaoNFSManual.LocalidadePrestacao);
            documentoPendente.ValorFrete = documentoEmissaoNFSManual.ValorFrete;
            documentoPendente.ProtocoloDocumento = documentoEmissaoNFSManual.Codigo;

            return documentoPendente;
        }

        private Dominio.ObjetosDeValor.WebService.Retorno<int> Anexar(int codigo, string base64, string tipo)
        {
            try
            {
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(_unitOfWork);
                Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(_unitOfWork);

                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repLancamentoNFSManual.BuscarPorCodigo(codigo);

                if (nfsManual == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoExcecao("Não foi possível encontrar o registro.");

                if (string.IsNullOrWhiteSpace(base64))
                    return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoExcecao("Nenhum arquivo selecionado para envio.");

                if (nfsManual.Situacao != SituacaoLancamentoNFSManual.DadosNota)
                    return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoExcecao("Situação da nota não permite anexar arquivos.");

                if (tipo.Equals("XML"))
                {
                    byte[] byteArray = Convert.FromBase64String(base64);

                    string xmlString = Encoding.UTF8.GetString(byteArray);

                    bool isXml = IsXml(xmlString);

                    if (!isXml)
                        return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoExcecao("O Arquivo informado não é um xml.");

                    nfsManual.DadosNFS.XML = xmlString;
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, nfsManual, null, "Anexou o XML.", _unitOfWork);
                }
                else if (tipo.Equals("DANFSE"))
                {
                    nfsManual.DadosNFS.ImagemNFS = this.SalvarDANFSE(nfsManual.DadosNFS.Numero, nfsManual.DadosNFS.Serie.Numero, nfsManual.Transportador, Convert.FromBase64String(base64), _unitOfWork);
                    nfsManual.Situacao = SituacaoLancamentoNFSManual.AgAprovacao;

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, nfsManual, null, "Anexou a DANFSE.", _unitOfWork);
                }

                repDadosNFSManual.Atualizar(nfsManual.DadosNFS);

                // Integracao com SignalR
                svcNFSManual.InformarLancamentoNFSManualAtualizada(nfsManual.Codigo);

                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoSucesso(nfsManual.Codigo);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Dominio.ObjetosDeValor.WebService.Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }

        }
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoNFS(int protocoloNFS)
        {
            Dominio.ObjetosDeValor.WebService.Retorno<bool> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<bool>();

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(_unitOfWork);
                Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(protocoloNFS, true);
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCTe(protocoloNFS);

                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

                if (cte == null || lancamentoNFSManual == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Nenhuma NFS Manual encontrada.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    retorno.Objeto = false;
                    return retorno;
                }

                _unitOfWork.Start();

                cte.NFsManualIntegrada = true;
                lancamentoNFSManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Finalizada;
                lancamentoNFSManual = Servicos.Embarcador.Integracao.IntegracaoNFSManual.AtualizarNumeracaoNFSManualIntegracao(lancamentoNFSManual, _unitOfWork);

                Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfseManualCTeIntegracao = repNFSManualCTeIntegracao.BuscarPrimeiroPorLancamentoNFSManual(lancamentoNFSManual.Codigo);
                if (nfseManualCTeIntegracao != null && nfseManualCTeIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                {
                    nfseManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    nfseManualCTeIntegracao.DataIntegracao = DateTime.Now;
                    nfseManualCTeIntegracao.LancamentoNFSManual = lancamentoNFSManual;
                    nfseManualCTeIntegracao.NumeroTentativas += 1;
                    nfseManualCTeIntegracao.ProblemaIntegracao = "Sucesso";
                    nfseManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                    repNFSManualCTeIntegracao.Atualizar(nfseManualCTeIntegracao);
                }

                repLancamentoNFSManual.Atualizar(lancamentoNFSManual);
                repCTe.Atualizar(cte, _auditado);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, cte, "Confirmada a integração da NFS Manual", _unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, lancamentoNFSManual, "Confirmada a integração da NFS Manual", _unitOfWork);

                // Integracao com SignalR
                svcNFSManual.InformarLancamentoNFSManualAtualizada(lancamentoNFSManual.Codigo);

                _unitOfWork.CommitChanges();

                retorno.Objeto = true;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Objeto = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da NFS Manual.";
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        private bool IsXml(string input)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(input);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }

        private string SalvarDANFSE(int numero, int serie, Dominio.Entidades.Empresa empresa, byte[] pdfData, Repositorio.UnitOfWork unitOfWork)
        {
            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

            if (!string.IsNullOrWhiteSpace(caminhoRelatorios))
            {
                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, "NFSe", empresa.CNPJ, numero.ToString() + "_" + serie.ToString()) + ".pdf";

                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, pdfData);

                return caminhoPDF;
            }
            else
            {
                return "";
            }
        }

        private void AdicionarDescontos(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            if (!lancamentoNFSManual.NFSResidual)
                return;

            Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual filtrosPesquisa = ObterFiltroPesquisaCargaDescontoParaEmissaoNFSManual(lancamentoNFSManual, documento, configuracaoEmbarcador, unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto repositorioLancamentoNFSManualDesconto = new Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioLancamentoNFSManualDesconto.BuscarCargasParaLancamentoNFSManualDesconto(filtrosPesquisa);
            decimal ValorFreteBruto = lancamentoNFSManual.DadosNFS.ValorFrete;
            decimal valorDescontos = 0m;

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                decimal desconto = -(valorDescontos + carga.ValorFreteResidual);
                if (desconto <= lancamentoNFSManual.DadosNFS.ValorFrete)
                {
                    Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto lancamentoNFSManualDesconto = new Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto()
                    {
                        Carga = carga,
                        LancamentoNFSManual = lancamentoNFSManual
                    };

                    repositorioLancamentoNFSManualDesconto.Inserir(lancamentoNFSManualDesconto);
                    valorDescontos += carga.ValorFreteResidual;
                }
            }

            valorDescontos = -valorDescontos;

            lancamentoNFSManual.DadosNFS.ValorFrete -= valorDescontos;
            lancamentoNFSManual.DadosNFS.ValorDescontos = valorDescontos;

            if (lancamentoNFSManual.DadosNFS.ValorFrete <= 0)
                throw new ControllerException($"O valor total do frete (R$ {ValorFreteBruto.ToString("n2")}) deve ser superior ao valor dos descontos (R$ {valorDescontos.ToString("n2")})");
        }

        private Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual ObterFiltroPesquisaCargaDescontoParaEmissaoNFSManual(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual()
            {
                CodigoCargaEmbarcador = documento.Carga.CodigoCargaEmbarcador,
                CodigoFilial = lancamentoNFSManual.Filial.Codigo,
                CodigoTipoOperacao = lancamentoNFSManual.TipoOperacao.Codigo,
                CodigoTransportador = lancamentoNFSManual.Transportador.Codigo,
                CpfCnpjTomador = lancamentoNFSManual.Tomador.Codigo,
            };

            if (configuracaoEmbarcador.UtilizaMoedaEstrangeira)
                filtrosPesquisa.Moeda = lancamentoNFSManual.DadosNFS.Moeda;

            return filtrosPesquisa;
        }
        #endregion
    }
}
