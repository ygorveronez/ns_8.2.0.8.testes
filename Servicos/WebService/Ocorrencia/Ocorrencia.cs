using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.ObjetosDeValor.Embarcador.Ocorrencia;
using Dominio.ObjetosDeValor.WebService;
using Newtonsoft.Json;
using Servicos.Embarcador.Integracao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.WebService.Ocorrencia
{
    public class Ocorrencia : ServicoBase
    {
        #region Propriedades Privadas

        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Constructores
       
        public Ocorrencia(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Ocorrencia(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Métodos Publicos

        public List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti> BuscarOcorrenciasPorTransportador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Empresa> empresas = new Servicos.Embarcador.Transportadores.Empresa(unitOfWork).BuscarEmpresasPorRaizCnpj(empresa);

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> cargasOcorrencia = repCargaOcorrencia.BuscarNaoIntegradasPorTransportador(empresas.Select(o => o.Codigo).ToList());

            return ObterDetalhesOcorrenciasIntegracao(cargasOcorrencia, unitOfWork);
        }

        public List<Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento> BuscarOcorrenciasCanceladasPorTransportador(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Empresa> empresas = new Servicos.Embarcador.Transportadores.Empresa(unitOfWork).BuscarEmpresasPorRaizCnpj(empresa);

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento> cancelamentos = repOcorrenciaCancelamento.BuscarNaoIntegradasComTransportador(empresas.Select(o => o.Codigo).ToList());

            return ObterDetalhesCancelamentosParaIntegracao(cancelamentos, unitOfWork);
        }

        public Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti BuscarOcorrenciaPorProtocoloETransportador(int protocoloOcorrencia, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Empresa> empresas = new Servicos.Embarcador.Transportadores.Empresa(unitOfWork).BuscarEmpresasPorRaizCnpj(empresa);

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarOcorrenciaPorProtocoloETransportador(protocoloOcorrencia, empresas.Select(o => o.Codigo).ToList());

            return ObterDetalhesOcorrenciaParaIntegracao(cargaOcorrencia, unitOfWork);
        }

        public List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega> BuscarOcorrenciasEntrega(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> listaOcorrenciasEntrega = protocolo.protocoloIntegracaoPedido > 0 ? repPedidoOcorrenciaColetaEntrega.BuscarPorPedido(protocolo.protocoloIntegracaoPedido) : repPedidoOcorrenciaColetaEntrega.BuscarPorCarga(protocolo.protocoloIntegracaoCarga);

            return ObterDetalhesOcorrenciasEntrega(listaOcorrenciasEntrega, unitOfWork);
        }

        public List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega> BuscarOcorrenciasEntregaPorNumeroPedidoEmbarcador(string numeroPedidoEmbarcador, DateTime dataInicial, DateTime dataFinal, Repositorio.UnitOfWork unitOfWork, int inicioRegistros = -1, int limiteRegistros = -1)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repositorioPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> listaOcorrenciasEntrega = repositorioPedidoOcorrenciaColetaEntrega.BuscarPorNumeroPedidoEmbarcador(numeroPedidoEmbarcador, dataInicial, dataFinal, inicioRegistros, limiteRegistros);

            return ObterDetalhesOcorrenciasEntrega(listaOcorrenciasEntrega, unitOfWork);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoOcorrencia(List<int> protocolos)
        {

            if (protocolos == null && protocolos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Precisa informar os protocolos que serar comfirmados");

            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> repositorioOcorrencia = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>(_unitOfWork);

            IList<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrencias = repositorioOcorrencia.BuscarRegitrosPendentesIntegracaoPeloProtocolos(protocolos);
            List<int> protocolosNaoProcessado = protocolos.Where(c => !listaOcorrencias.Any(m => m.Codigo == c)).ToList();

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in listaOcorrencias)
            {
                ocorrencia.IntegradoERP = true;
                repositorioOcorrencia.Atualizar(ocorrencia);
            }

            if (protocolosNaoProcessado == null || protocolosNaoProcessado.Count > 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, $"Para os Protocolo(s) {string.Join(",", protocolosNaoProcessado)} Não foram encontrados registros ou ja foram comfirmados.");

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Todos os protocolo integrados com sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarStatusCargaOcorrenciaCTe(RetornoStatusCargaOcorrenciaCTeResult retornoCargaOcorrenciaCTe)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao = repositorioOcorrenciaCTeIntegracao
                .BuscarPorCodigo(retornoCargaOcorrenciaCTe.ProtocoloIntegracao);

            if (ocorrenciaCTeIntegracao == null)
                return Retorno<bool>.CriarRetornoExcecao("Protocolo de Integração não encontrado");

            if (ocorrenciaCTeIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                return Retorno<bool>.CriarRetornoExcecao("A situação atual da integração da occorrência de CTe não permite integração");

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = ocorrenciaCTeIntegracao.CargaOcorrencia;

            if (retornoCargaOcorrenciaCTe.ProcessadoSucesso)
            {
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                bool existemOutrasIntegracoesPendentes = repositorioOcorrenciaCTeIntegracao
                    .BuscarPorOcorrencia(cargaOcorrencia.Codigo)
                    .Where(ocorrencia => ocorrencia != ocorrenciaCTeIntegracao)
                    .Any(ocorrencia => ocorrencia.SituacaoIntegracao != SituacaoIntegracao.Integrado);

                if (!existemOutrasIntegracoesPendentes)
                    cargaOcorrencia.SituacaoOcorrencia = SituacaoOcorrencia.Finalizada;
            }
            else
            {
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaOcorrencia.SituacaoOcorrencia = SituacaoOcorrencia.FalhaIntegracao;
            }

            ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;
            ocorrenciaCTeIntegracao.ProblemaIntegracao = retornoCargaOcorrenciaCTe.MensagemRetorno;

            Retorno<bool> retorno = Retorno<bool>.CriarRetornoSucesso(true);
            servicoArquivo.Adicionar(ocorrenciaCTeIntegracao, JsonConvert.SerializeObject(retornoCargaOcorrenciaCTe), JsonConvert.SerializeObject(retorno), "json");

            repositorioCargaOcorrencia.Atualizar(cargaOcorrencia);
            repositorioOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);

            return retorno;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>> BuscarOcorrenciasPendentesIntegracao(int quantidade)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();

            int totalOcorrenciasPendenteIntegracao = repositorioCargaOcorrencia.ContarRegistroPendenteIntegracao(configuracaoWebService.RetornarApenasOcorrenciasFinalizadasMetodoBuscarOcorrenciasPendentesIntegracao);

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia> retornoPaginacao = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>(),
                NumeroTotalDeRegistro = totalOcorrenciasPendenteIntegracao
            };

            if (totalOcorrenciasPendenteIntegracao == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>>.CriarRetornoSucesso(retornoPaginacao);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrenciasPendentes = repositorioCargaOcorrencia.BuscarRegistrosPendentesIntegracao(quantidade, configuracaoWebService.RetornarApenasOcorrenciasFinalizadasMetodoBuscarOcorrenciasPendentesIntegracao);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencias in ocorrenciasPendentes)
                retornoPaginacao.Itens.Add(ConverterObjetoOcorrencia(ocorrencias));

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>>.CriarRetornoSucesso(retornoPaginacao);

        }

        public Retorno<int> AdicionarOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia ocorrencia)
        {
            try
            {
                if ((ocorrencia.TipoOcorrencia == null) || string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracao))
                    throw new WebServiceException("É obrigatório informar o tipo da Ocorrência.");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repositorioConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService confiWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.CargaOcorrencia.Ocorrencia serOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repositorioConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorUsuarioMultisoftware();

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = null;

                Dominio.Entidades.Empresa empresa = _auditado.Integradora.Empresa;

                if (empresa == null && ocorrencia.Empresa != null)
                {
                    string cnpjEmpresa = Utilidades.String.OnlyNumbers(ocorrencia.Empresa.CNPJ);
                    empresa = repEmpresa.BuscarPorCNPJ(cnpjEmpresa);
                }

                if (empresa != null) //&& repEmpresaIntelipostIntegracao.VerificarEmpresaPossuiIntegracao(empresa.Codigo)
                {
                    Repositorio.Global.EmpresaIntelipostTipoOcorrencia repEmpresaIntelipostTipoOcorrencia = new Repositorio.Global.EmpresaIntelipostTipoOcorrencia(_unitOfWork);
                    Dominio.Entidades.EmpresaIntelipostTipoOcorrencia empresaIntelipostTipoOcorrencia = repEmpresaIntelipostTipoOcorrencia.BuscarPorEmpresaECodigoIntegracao(empresa.Codigo, ocorrencia.TipoOcorrencia.CodigoIntegracao);
                    tipoDeOcorrencia = empresaIntelipostTipoOcorrencia?.TipoOcorrencia;
                }

                if (tipoDeOcorrencia == null)
                    tipoDeOcorrencia = repOcorrenciaDeCTe.BuscarPorCodigoIntegracao(ocorrencia.TipoOcorrencia.CodigoIntegracao);

                if (tipoDeOcorrencia == null)
                    throw new WebServiceException("É obrigatório informar o tipo da Ocorrência.");

                if (tipoDeOcorrencia.OrigemOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga)
                    throw new WebServiceException("O tipo da ocorrência informada não é uma ocorrência por carga e não pode ser criado via WebService.");

                if (ocorrencia.NumerosNotasFiscais == null)
                    ocorrencia.NumerosNotasFiscais = new List<int>();

                if (ocorrencia.NumeroNotaFiscal > 0)
                    ocorrencia.NumerosNotasFiscais.Add(ocorrencia.NumeroNotaFiscal);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = (ocorrencia.ProtocoloIntegracaoCarga > 0) ? repCarga.BuscarPorProtocolo(ocorrencia.ProtocoloIntegracaoCarga) : null;
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscaisCarga = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

                if (carga == null && empresa != null)
                {
                    double cnpjRemetente = 0;
                    double.TryParse(Utilidades.String.OnlyNumbers(ocorrencia.Remetente?.CPFCNPJ ?? string.Empty), out cnpjRemetente);

                    double cnpjDestinatario = 0;
                    double.TryParse(Utilidades.String.OnlyNumbers(ocorrencia.Destinatario?.CPFCNPJ ?? string.Empty), out cnpjDestinatario);

                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorNumeroNotaEmitenteTransportadorTodasSituacoes(ocorrencia.NumerosNotasFiscais, empresa.Codigo, cnpjRemetente, cnpjDestinatario);

                    if (pedidoXMLNotaFiscal != null)
                    {
                        if (pedidoXMLNotaFiscal.CargaPedido?.Pedido?.SituacaoPedido == SituacaoPedido.Cancelado)
                            throw new WebServiceException("Não é possível adicionar uma ocorrência para um pedido cancelado.");

                        if (pedidoXMLNotaFiscal.CargaPedido?.Carga?.SituacaoCarga is SituacaoCarga.Cancelada or SituacaoCarga.Anulada)
                            throw new WebServiceException("Não é possível adicionar uma ocorrência para uma carga cancelada ou anulada.");

                        carga = pedidoXMLNotaFiscal.CargaPedido.Carga;
                        pedidoXMLNotaFiscaisCarga.Add(pedidoXMLNotaFiscal);
                    }
                }

                if (carga == null)
                {
                    if (ocorrencia.ProtocoloIntegracaoCarga > 0)
                        throw new WebServiceException($"Não foi localizada uma carga para o protocolo {ocorrencia.ProtocoloIntegracaoCarga} na base Multisoftware.");

                    throw new WebServiceException("Não foi localizada uma carga na base Multisoftware.");
                }

                if (confiWebService?.NaoPermitirConfirmarEntregaSituacaoCargaEmAndamento ?? false)
                {
                    List<SituacaoCarga> listaSituacaoCargaEmAberto = SituacaoCargaHelper.ObterSituacoesCargaPermiteAtualizar();
                    if (listaSituacaoCargaEmAberto.Contains(carga.SituacaoCarga))
                    {
                        Servicos.Log.TratarErro($"AdicionarOcorrencia - Carga pendente de avanço da etapa de frete para confirmação da entrega");
                        throw new WebServiceException("Carga pendente de avanço da etapa de frete para confirmação da entrega");
                    }
                }

                Dominio.Entidades.Cliente remetente = null;
                Dominio.Entidades.Cliente destinatario = null;

                if (ocorrencia.Remetente != null)
                {
                    double cnpj = 0;
                    double.TryParse(Utilidades.String.OnlyNumbers(ocorrencia.Remetente.CPFCNPJ), out cnpj);
                    remetente = repCliente.BuscarPorCPFCNPJ(cnpj);

                    if (remetente == null)
                        throw new WebServiceException($"Não foi localizado um remetente cadastradado na base Multisoftware para o CNPJ {ocorrencia.Remetente.CPFCNPJ}.");
                }

                if (ocorrencia.Destinatario != null)
                {
                    double cnpj = 0;
                    double.TryParse(Utilidades.String.OnlyNumbers(ocorrencia.Destinatario.CPFCNPJ), out cnpj);
                    destinatario = repCliente.BuscarPorCPFCNPJ(cnpj);

                    if (destinatario == null)
                        throw new WebServiceException($"Não foi localizado um destinatário cadastradado na base Multisoftware para o CNPJ {ocorrencia.Remetente.CPFCNPJ}.");
                }

                DateTime data;

                if (!string.IsNullOrWhiteSpace(ocorrencia.DataOcorrencia))
                {
                    if (!DateTime.TryParseExact(ocorrencia.DataOcorrencia, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                        throw new WebServiceException("A data da ocorrência não está em um formato correto (dd/MM/yyyy HH:mm:ss); ");
                }
                else
                    data = DateTime.Now;

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (configuracaoEmbarcador.PedidoOcorrenciaColetaEntregaIntegracaoNova)
                {
                    _unitOfWork.Start();

                    Dominio.ObjetosDeValor.Embarcador.Ocorrencia.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntregaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.PedidoOcorrenciaColetaEntrega()
                    {
                        GrupoOcorrencia = ocorrencia.GrupoOcorrencia,
                        Natureza = ocorrencia.Natureza,
                        NotafiscalDevolucao = ocorrencia.NotaFiscalDevolucao,
                        Razao = ocorrencia.Razao,
                        SolicitacaoCliente = ocorrencia.SolicitacaoCliente
                    };

                    serOcorrencia.GerarPedidoOcorrenciaNotas(pedidoXMLNotaFiscaisCarga, tipoDeOcorrencia, carga, data, ocorrencia.Observacao, ocorrencia.Latitude, ocorrencia.Longitude, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.WebService, "", 0, _tipoServicoMultisoftware, configuracaoEmbarcador, usuario, _clienteMultisoftware, _unitOfWork, _auditado, pedidoOcorrenciaColetaEntregaIntegracao);

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();

                    if (configuracaoWebService.SempreSeguirConfiguracaoOcorrenciaQuandoAdicionadaPeloMetodoAdicionarOcorrencia)
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = repositorioCargaEntregaNotaFiscal.BuscarPorCargaENumeroNotaFiscal(carga.Codigo, ocorrencia.NumerosNotasFiscais);

                        Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointRejeicaoColetaEntrega = null;

                        if (!string.IsNullOrEmpty(ocorrencia.Latitude) && !string.IsNullOrEmpty(ocorrencia.Longitude))
                            wayPointRejeicaoColetaEntrega = new WayPoint(ocorrencia.Latitude, ocorrencia.Longitude);

                        Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros
                        {
                            codigoCargaEntrega = cargaEntregaNotaFiscal.CargaEntrega?.Codigo ?? 0,
                            codigoMotivo = tipoDeOcorrencia.Codigo,
                            data = data,
                            wayPoint = wayPointRejeicaoColetaEntrega,
                            usuario = usuario,
                            motivoRetificacao = 0,
                            tipoServicoMultisoftware = _tipoServicoMultisoftware,
                            observacao = "",
                            configuracao = configuracaoEmbarcador,
                            devolucaoParcial = false,
                            notasFiscais = null,
                            motivoFalhaGTA = 0,
                            apenasRegistrar = false,
                            dadosRecebedor = null,
                            permitirEntregarMaisTarde = false,
                            dataConfirmacaoChegada = null,
                            wayPointConfirmacaoChegada = null,
                            atendimentoRegistradoPeloMotorista = false,
                            OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.WebService,
                            dataInicioCarregamento = carga.DataCarregamentoCarga,
                            quantidadeImagens = 0,
                            imagens = null,
                            clienteMultisoftware = null,
                            valorChamado = ocorrencia.ValorOcorrencia
                        };

                        Auditado auditado = null;
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(parametros, auditado, _unitOfWork, out Dominio.Entidades.Embarcador.Chamados.Chamado chamado, _tipoServicoMultisoftware);

                    }

                    _unitOfWork.CommitChanges();

                    return Retorno<int>.CriarRetornoSucesso(0);
                }
                else
                {
                    List<string> numerosNotasFiscais = ocorrencia.NumerosNotasFiscais != null && ocorrencia.NumerosNotasFiscais.Any() ? ocorrencia.NumerosNotasFiscais.Select(n => n.ToString()).ToList() : new List<string>();

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = null;

                    if (ocorrencia.NumerosNotasFiscais != null && ocorrencia.NumerosNotasFiscais.Any())
                        cargaOcorrenciaDocumento = repCargaOcorrenciaDocumento.BuscarPorCargaENotaFiscal(carga.Codigo, ocorrencia.NumerosNotasFiscais);
                    else
                        cargaOcorrenciaDocumento = repCargaOcorrenciaDocumento.BuscarPorCargaRemetenteDestinatario(carga.Codigo, remetente?.CPF_CNPJ ?? 0, destinatario?.CPF_CNPJ ?? 0, tipoDeOcorrencia.Codigo, 0);

                    if (cargaOcorrenciaDocumento != null && !(configuracaoOcorrencia?.PermiteInformarMaisDeUmaOcorrenciaPorNFe ?? false))
                        throw new WebServiceException("Já existe uma ocorrência gerada e em aberto para os dados envidos na integração.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);

                    bool retornarPreCtes = (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos && carga.AgImportacaoCTe);
                    bool todosCTesDaCargaSelecionada = tipoDeOcorrencia?.TodosCTesSelecionados ?? false;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = todosCTesDaCargaSelecionada ? repCargaCte.BuscarPorCargaSemComplementares(carga.Codigo) : repCargaCte.BuscarPorCargaENotaFiscal(carga.Codigo, false, true, remetente?.CPF_CNPJ ?? 0, destinatario?.CPF_CNPJ ?? 0, retornarPreCtes, numerosNotasFiscais, ocorrencia.SerieNotaFiscal);

                    Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSManual = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCargaENumeroNota(carga.Codigo, ocorrencia.NumerosNotasFiscais);

                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = serOcorrencia.GerarOcorrenciaNotasFiscais(tipoDeOcorrencia, carga, cargaDocumentosParaEmissaoNFSManual, cargaCTEs, data, ocorrencia.Observacao, ocorrencia.ValorOcorrencia, ocorrencia.Latitude, ocorrencia.Longitude, ocorrencia.KM, _tipoServicoMultisoftware, configuracaoEmbarcador, usuario, _clienteMultisoftware, _unitOfWork).Result;

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaOcorrencia, "Adicionou ocorrência", _unitOfWork);

                    _unitOfWork.CommitChanges();

                    return Retorno<int>.CriarRetornoSucesso(cargaOcorrencia.Codigo);
                }
            }
            catch (BaseException excecao)
            {
                _unitOfWork.Rollback();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                    return Retorno<int>.CriarRetornoDuplicidadeRequisicao(excecao.Message);

                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<int>.CriarRetornoExcecao(excecao.Message.Contains("SQL:") ? "Ocorreu uma falha ao adicionar a Ocorrência." : excecao.Message);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao>> BuscarAtendimentosPendentesIntegracao()
        {
            /*lembrar de usar o construtor pra funcionar as propriedades privadas, após isso pode remover está anotação.*/
            /*lembrar de usar o construtor pra funcionar as propriedades privadas, após isso pode remover está anotação.*/
            /*lembrar de usar o construtor pra funcionar as propriedades privadas, após isso pode remover está anotação.*/
            /*lembrar de usar o construtor pra funcionar as propriedades privadas, após isso pode remover está anotação.*/
            /*lembrar de usar o construtor pra funcionar as propriedades privadas, após isso pode remover está anotação.*/
            /*lembrar de usar o construtor pra funcionar as propriedades privadas, após isso pode remover está anotação.*/
            /*lembrar de usar o construtor pra funcionar as propriedades privadas, após isso pode remover está anotação.*/
            /*lembrar de usar o construtor pra funcionar as propriedades privadas, após isso pode remover está anotação.*/
            /*lembrar de usar o construtor pra funcionar as propriedades privadas, após isso pode remover está anotação.*/
            /*lembrar de usar o construtor pra funcionar as propriedades privadas, após isso pode remover está anotação.*/

            if (_tipoServicoMultisoftware != TipoServicoMultisoftware.MultiEmbarcador)
                return null;

            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repositorioChamado.BuscarAtendimentosPendenteDeIntegracao();
            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao> atendimentosPendentesIntegracoes = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao>();

            if (chamados?.Count > 0)
            {
                List<(int CodigoCarga, int NumeroNota)> listaCodigosCargasNumeroNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarNumeroNotasFiscaisECodigosCargasPorCarga(chamados.Select(o => o.Carga?.Codigo ?? 0).ToList());

                atendimentosPendentesIntegracoes.Itens = ConverterObjetoAtendimentosPendentesIntegracoes(chamados, listaCodigosCargasNumeroNotasFiscais);
            }
            else
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao>>.CriarRetornoExcecao("Não foi localizado nenhum Chamado de Ocorrência pendente!");


            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao>>.CriarRetornoSucesso(atendimentosPendentesIntegracoes);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoAtendimento(int codigoProtocolo)
        {
            if (codigoProtocolo == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Precisa informar o Número do Protocolo para que seja possível validar!");

            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);

            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCodigo(codigoProtocolo);

            if (chamado == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Chamado Ocorrência não encontrado para este protocolo!");

            _unitOfWork.Start();

            chamado.AguardandoIntegracao = false;

            repositorioChamado.Atualizar(chamado);

            _unitOfWork.Dispose();

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Protocolo teve seu status de integração alterado com sucesso!");
        }

        public Retorno<bool> SolicitarCancelamentoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.SolicitarCancelamentoOcorrencia ocorrenciaEmCancelamento)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            int protocoloOcorrencia = ocorrenciaEmCancelamento.ProtocoloOcorrencia;
            string motivoCancelamento = ocorrenciaEmCancelamento.MotivoCancelamento;

            try
            {
                Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

                Servicos.Embarcador.Carga.Ocorrencia serCargaOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia();

                if (string.IsNullOrWhiteSpace(motivoCancelamento))
                {
                    retorno.Mensagem = "Obrigatório informar motivo com 20 ou mais caracteres.";
                    retorno.Objeto = false;
                    retorno.Status = true;
                }

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(protocoloOcorrencia);
                if (cargaOcorrencia != null)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamentoExistente = repOcorrenciaCancelamento.BuscarPorOcorrencia(protocoloOcorrencia);
                    if (ocorrenciaCancelamentoExistente != null)
                    {
                        retorno.Mensagem = "Já existe uma solicitação de cancelamento para esta ocorrência.";
                        retorno.Objeto = false;
                        retorno.Status = true;
                    }
                    else
                    {
                        if (!serCargaOcorrencia.VerificarSeOcorrenciaPermiteCancelamento(out string mensagem, cargaOcorrencia, _unitOfWork, _tipoServicoMultisoftware))
                        {
                            retorno.Mensagem = mensagem;
                            retorno.Objeto = false;
                            retorno.Status = true;
                        }
                        else
                        {
                            bool permiteCancelarCTes = true;
                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTesComplementoInfo in cargaCTesComplementaresInfo)
                            {
                                if (cargaCTesComplementoInfo.CTe != null && cargaCTesComplementoInfo.CTe.CargaCTes.Any(obj => obj.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe))
                                {
                                    if (cargaCTesComplementoInfo.CTe.Status == "A" && cargaCTesComplementoInfo.CTe.DataRetornoSefaz < DateTime.Now.AddDays(-7) && cargaCTesComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "57")
                                        permiteCancelarCTes = false;
                                }
                            }

                            if (!permiteCancelarCTes)
                            {
                                retorno.Mensagem = "Não é possível cancelar CT-e(s) que foram emitidos a mais de 7 dias."; ;
                                retorno.Objeto = false;
                                retorno.Status = true;
                            }
                            else
                            {
                                _unitOfWork.Start();

                                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento
                                {
                                    Ocorrencia = cargaOcorrencia,
                                    DataCancelamento = DateTime.Now,
                                    MotivoCancelamento = motivoCancelamento,
                                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Cancelamento,
                                    //Usuario = Usuario,
                                    SituacaoOcorrenciaNoCancelamento = cargaOcorrencia.SituacaoOcorrencia,
                                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.EmCancelamento,
                                    EnviouCTesParaCancelamento = false
                                };

                                Auditado auditado = null;
                                repOcorrenciaCancelamento.Inserir(ocorrenciaCancelamento, auditado);

                                _unitOfWork.CommitChanges();

                                retorno.Mensagem = "Cancelamento da ocorrência solicitada com sucesso.";
                                retorno.Objeto = true;
                                retorno.Status = true;

                                Servicos.Auditoria.Auditoria.AuditarConsulta(auditado, "Solicitou cancelamento da ocorrência protocolo " + protocoloOcorrencia.ToString(), _unitOfWork);
                            }
                        }
                    }
                }
                else
                {
                    retorno.Mensagem = "Ocorrência protocolo " + protocoloOcorrencia.ToString() + " não encontrado.";
                    retorno.Objeto = false;
                    retorno.Status = true;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                _unitOfWork.Rollback();
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao solicitar cancelamento da ocorrência";
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia> BuscarSituacaoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.BuscarSituacaoOcorrencia ocorrenciaSituacao)
        {
            Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            int protocoloOcorrencia = ocorrenciaSituacao.ProtocoloOcorrencia;

            try
            {
                Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

                Servicos.Embarcador.Carga.Ocorrencia serCargaOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia();
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(protocoloOcorrencia);
                if (cargaOcorrencia != null)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamentoExistente = repOcorrenciaCancelamento.BuscarPorOcorrencia(protocoloOcorrencia);


                    retorno.Objeto = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia()
                    {
                        Data = cargaOcorrencia.DataOcorrencia.ToString("dd/MM/yyyy"),
                        Observacao = cargaOcorrencia.Observacao,
                        Ocorrencia = cargaOcorrencia.NumeroOcorrencia.ToString(),
                        Situacao = ocorrenciaCancelamentoExistente != null ? ocorrenciaCancelamentoExistente.DescricaoSituacao : cargaOcorrencia.DescricaoSituacao
                    };

                    retorno.Mensagem = "Ocorrência protocolo retornada com encontrado.";
                    retorno.Status = true;
                }
                else
                {
                    retorno.Mensagem = "Ocorrência protocolo " + protocoloOcorrencia.ToString() + " não encontrado.";
                    retorno.Status = true;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar da cancelamento da ocorrência";
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return retorno;
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti> ObterDetalhesOcorrenciasIntegracao(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> cargasOcorrencias, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti> ocorrenciaIntegracao = new List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti>();

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in cargasOcorrencias)
                ocorrenciaIntegracao.Add(ObterDetalhesOcorrenciaParaIntegracaoTransportador(ocorrencia));

            return ocorrenciaIntegracao;
        }

        private Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti ObterDetalhesOcorrenciaParaIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti ocorrenciaIntegracao = new Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti()
            {
                //Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = ocorrencia.Carga..FirstOrDefault().Destinatario.CPF_CNPJ_SemFormato },
                Empresa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = ocorrencia.Carga.Empresa.CNPJ_SemFormato },
                NumeroCargaEmbarcador = ocorrencia.Carga.CodigoCargaEmbarcador,
                Observacao = ocorrencia.Observacao,
                Protocolo = ocorrencia.Codigo,
                Descricao = ocorrencia.Descricao,
                ProtocoloCarga = ocorrencia.Carga.Codigo,
                TipoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia() { CodigoIntegracao = ocorrencia.CodigoTipoOcorrenciaParaIntegracao },
                ValorOcorrencia = ocorrencia.ValorOcorrencia,
                DataOcorrencia = ocorrencia.DataOcorrencia,
            };

            return ocorrenciaIntegracao;

        }

        private Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti ObterDetalhesOcorrenciaParaIntegracaoTransportador(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti ocorrenciaIntegracao = new Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti()
            {
                Empresa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = ocorrencia.Carga.Empresa.CNPJ_SemFormato },
                NumeroCargaEmbarcador = ocorrencia.Carga.CodigoCargaEmbarcador,
                Observacao = ocorrencia.Observacao,
                Protocolo = ocorrencia.Codigo,
                Descricao = ocorrencia.Descricao,
                ProtocoloCarga = ocorrencia.Carga.Codigo,
                TipoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia() { CodigoIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracao },
                ValorOcorrencia = ocorrencia.ValorOcorrencia,
                DataOcorrencia = ocorrencia.DataOcorrencia,
            };

            return ocorrenciaIntegracao;
        }

        private List<Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento> ObterDetalhesCancelamentosParaIntegracao(List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento> cancelamentos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            return (from obj in cancelamentos
                    select new Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento()
                    {
                        DataCancelamento = obj.DataCancelamento,
                        MotivoCancelamento = obj.MotivoCancelamento,
                        ProtocoloCancelamento = obj.Codigo,
                        ProtocoloOcorrecia = obj.Ocorrencia.Codigo,
                        PossuiDocumentoCancelado = repCargaCTe.ExisteCanceladoPorCarga(obj.Ocorrencia.Carga.Codigo)
                    }).ToList();
        }

        private List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega> ObterDetalhesOcorrenciasEntrega(List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> listaOcorrenciasEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega> OcorrenciaIntegracao = new List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>();
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<int> codigosCarga = (from obj in listaOcorrenciasEntrega where obj.Carga != null select obj.Carga.Codigo).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotas = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            if (codigosCarga.Count > 0)
                pedidosXMLNotas = repPedidoXMLNotaFiscal.ConsultarNotasVinculadasPorListaCargas(codigosCarga);

            foreach (var ocorrencia in listaOcorrenciasEntrega)
                OcorrenciaIntegracao.Add(ObterDetalhesOcorrenciaEntregaParaIntegracao(ocorrencia, pedidosXMLNotas, unitOfWork));

            return OcorrenciaIntegracao;
        }

        private Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega ObterDetalhesOcorrenciaEntregaParaIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrencia, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotas, Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega ocorrenciaIntegracao = new Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega()
            {
                ProtocoloPedido = ocorrencia.Pedido?.Protocolo ?? 0,
                NumeroPedido = ocorrencia.Pedido?.NumeroPedidoEmbarcador,
                CodigoOcorrencia = ocorrencia.TipoDeOcorrencia?.CodigoIntegracao,
                DescricaoOcorrencia = ocorrencia.TipoDeOcorrencia?.Descricao,
                Observacao = ocorrencia.Observacao,
                Cliente = ocorrencia.Alvo?.Descricao,
                DataOcorrencia = ocorrencia.DataOcorrencia.ToString("dd/MM/yyyy HH:mm:ss"),
                IdPedidoVTEX = ocorrencia.Pedido?.NumeroPedidoEmbarcador,
                IdPedidoEMillennium = ocorrencia.Pedido?.CodigoPedidoCliente,
                CodigoVolume = ocorrencia.Pacote,
                QuantidadeVolumes = ocorrencia.Volumes,
                Transportadora = ocorrencia.Carga?.Empresa.Descricao,
                OcorrenciaVisivelAoCliente = !(ocorrencia.TipoDeOcorrencia?.NaoIndicarAoCliente ?? false),
                OcorrenciaFinalizadora = ocorrencia.TipoDeOcorrencia?.Tipo == "F"
            };

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotasPedido = (from obj in pedidosXMLNotas where obj.CargaPedido.Pedido.Codigo == ocorrencia.Pedido.Codigo && obj.CargaPedido.Carga.Codigo == ocorrencia.Carga?.Codigo select obj).ToList();

            if (pedidosXMLNotasPedido.Count > 0)
            {
                ocorrenciaIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.WebService.Ocorrencia.NotaFiscal>();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidosXMLNotasPedido)
                {
                    var notaFiscal = new Dominio.ObjetosDeValor.WebService.Ocorrencia.NotaFiscal();
                    notaFiscal.ChaveNFe = pedidoXMLNotaFiscal.XMLNotaFiscal.Chave;
                    notaFiscal.NumeroNFe = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString();
                    ocorrenciaIntegracao.NotasFiscais.Add(notaFiscal);
                }
            }
            else if (ocorrencia.Pedido.NotasFiscais != null && ocorrencia.Pedido.NotasFiscais.Count > 0)
            {
                ocorrenciaIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.WebService.Ocorrencia.NotaFiscal>();
                foreach (var nota in ocorrencia.Pedido.NotasFiscais)
                {
                    var notaFiscal = new Dominio.ObjetosDeValor.WebService.Ocorrencia.NotaFiscal();
                    notaFiscal.ChaveNFe = nota.Chave;
                    notaFiscal.NumeroNFe = nota.Numero.ToString();
                    ocorrenciaIntegracao.NotasFiscais.Add(notaFiscal);
                }
            }

            return ocorrenciaIntegracao;

        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia ConverterObjetoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            Servicos.WebService.Pessoas.Pessoa servicoPessoa = new Pessoas.Pessoa(_unitOfWork);
            Servicos.WebService.Empresa.Empresa servicoEmpresa = new Servicos.WebService.Empresa.Empresa(_unitOfWork);
            Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(_unitOfWork);
            Servicos.Embarcador.Carga.CargaDocumentoParaEmissaoNFSManual serCargaDocumentoParaEmissaoNFSManual = new Servicos.Embarcador.Carga.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repositorioCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> listaCTes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaDocumentoParaEmissaoNFSManual> listaDocsParaEmissaoNFS = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaDocumentoParaEmissaoNFSManual>();

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repositorioCargaOcorrenciaDocumento.BuscarPorOcorrencia(ocorrencia.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> docsParaEmissaoNFS = cargaOcorrenciaDocumentos.Select(o => o.CargaDocumentoParaEmissaoNFSManualComplementado).Where(o => o != null).ToList();

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCte = cargaOcorrenciaDocumentos.Select(o => o.CargaCTe?.CTe).Where(cte => cte != null).ToList();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in listaCte)
            {
                listaCTes.Add(serCte.ConverterEntidadeCTeParaObjeto(cte, enviarCTeApenasParaTomador, _unitOfWork));
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual nfs in docsParaEmissaoNFS)
            {
                listaDocsParaEmissaoNFS.Add(serCargaDocumentoParaEmissaoNFSManual.ConverterEntidadeCargaDocumentoParaEmissaoNFSManualParaObjeto(nfs, _unitOfWork));
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(ocorrencia?.Carga ?? null);

            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia()
            {
                Protocolo = ocorrencia.Codigo,
                DataOcorrencia = ocorrencia.DataOcorrencia.ToString("dd/MM/yyyy"),
                TipoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia() { CodigoIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracao },
                Destinatario = servicoPessoa.ConverterObjetoPessoa(ocorrencia?.Carga?.Pedidos.Where(cp => cp.Pedido.Destinatario != null).Select(cp => cp.Pedido.Destinatario).FirstOrDefault()),
                Remetente = servicoPessoa.ConverterObjetoPessoa(ocorrencia?.Carga?.Pedidos.Where(cp => cp.Pedido.Remetente != null).Select(cp => cp.Pedido.Remetente).FirstOrDefault()),
                Empresa = servicoEmpresa.ConverterObjetoEmpresa(ocorrencia?.Carga?.Empresa),
                Observacao = ocorrencia.Observacao,
                ValorOcorrencia = ocorrencia.ValorOcorrencia,
                ProtocoloIntegracaoCarga = ocorrencia.Carga.Protocolo,
                ProtocoloIntegracaoPedido = string.Join(", ", (from obj in cargaPedidos where obj?.Pedido != null select obj.Pedido.Protocolo).ToList()),
                NumeroOcorrencia = ocorrencia?.NumeroOcorrencia ?? 0,
                Latitude = ocorrencia.Latitude,
                Longitude = ocorrencia.Longitude,
                SituacaoOcorrencia = ocorrencia.SituacaoOcorrencia,
                CTe = listaCTes,
                DocsParaEmissaoNFS = listaDocsParaEmissaoNFS
            };
        }

        private List<Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao> ConverterObjetoAtendimentosPendentesIntegracoes(List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados, List<(int CodigoCarga, int NumeroNota)> listaCodigosCargasNumeroNotasFiscais)
        {
            List<Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao> atendimentosPendentesIntegracoes = new List<Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao>();

            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
            {
                List<string> numerosNotasFiscais = listaCodigosCargasNumeroNotasFiscais.Where(o => o.CodigoCarga == (chamado.Carga?.Codigo ?? 0)).Select(o => o.NumeroNota.ToString()).ToList();

                atendimentosPendentesIntegracoes.Add(ConverterObjetoAtendimentosPendentesIntegracao(chamado, numerosNotasFiscais));
            }

            return atendimentosPendentesIntegracoes;
        }

        private Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao ConverterObjetoAtendimentosPendentesIntegracao(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, List<string> numerosNotasFiscais)
        {
            Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao atendimentosPendentesIntegracao = new Dominio.ObjetosDeValor.WebService.Ocorrencia.AtendimentosPendentesIntegracao()
            {
                Protocolo = chamado.Codigo.ToString(),
                NumeroAtendimento = chamado.Numero.ToString(),
                DataCriacao = chamado.DataCriacao.ToDateString(),
                FilialCarga = chamado.Carga?.Filial?.Descricao ?? string.Empty,
                TipoOperacaoCarga = chamado.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                TipoCargaCarga = chamado.Carga?.TipoDeCarga?.Descricao ?? string.Empty,
                ListaDadosNota = string.Join(", ", numerosNotasFiscais),
                MotivoAtendimento = chamado?.MotivoChamado?.Descricao ?? string.Empty,
                Observacao = chamado.Observacao,
                Quantidade = chamado.Quantidade.ToString()
            };

            return atendimentosPendentesIntegracao;
        }

        #endregion
    }
}
