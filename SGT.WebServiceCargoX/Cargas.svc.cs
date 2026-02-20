using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SGT.WebServiceCargoX
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Cargas : WebServiceBase, ICargas
    {
        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarCarga(Dominio.ObjetosDeValor.WebService.CargoX.CargaIntegracao cargaIntegracao)
        {
            Servicos.Log.TratarErro($"AdicionarCarga: {(cargaIntegracao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cargaIntegracao) : string.Empty)}");

            ValidarToken();

            StringBuilder mensagemErro = new StringBuilder();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Integracao.IndicadorIntegracaoNFe servicoIndicadorIntegracaoNFe = new Servicos.Embarcador.Integracao.IndicadorIntegracaoNFe(unitOfWork, configuracaoTMS);

                Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracaoMulti = ConverterIntegracaoCarga(cargaIntegracao, ref mensagemErro, unitOfWork, configuracaoTMS);

                if (mensagemErro.Length > 0)
                {
                    Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroDoEmbarcador} Retornou essa mensagem (validação): {mensagemErro.ToString()}");

                    Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });

                    return retorno;
                }

                if (cargaIntegracaoMulti == null)
                {
                    Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroDoEmbarcador} Não foi possível converter integração.");

                    Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos("Dados inválidos.", new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });

                    return retorno;
                }

                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracaoMulti.Filial?.CodigoIntegracao ?? "");
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (cargaIntegracaoMulti.TipoOperacao != null) ? repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracaoMulti.TipoOperacao.CodigoIntegracao) : null;


                if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                    unitOfWork.Start(System.Data.IsolationLevel.Serializable);
                else
                    unitOfWork.Start();

                int codigoCargaExistente = 0;
                int protocoloPedidoExistente = 0;
                Servicos.WebService.Carga.Pedido servicoPedido = new Servicos.WebService.Carga.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedido.CriarPedido(cargaIntegracaoMulti, filial, tipoOperacao, ref mensagemErro, TipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, false, Auditado, configuracaoTMS, ClienteAcesso, Conexao.AdminStringConexao);

                if (mensagemErro.Length == 0 || protocoloPedidoExistente > 0)
                {
                    if (protocoloPedidoExistente == 0)
                        new Servicos.WebService.Carga.ProdutosPedido(Conexao.StringConexao).AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracaoMulti, ref mensagemErro, unitOfWork, Auditado);

                    if (cargaIntegracaoMulti.Transbordo != null && cargaIntegracaoMulti.Transbordo.Count > 0)
                        new Servicos.WebService.Carga.ProdutosPedido(Conexao.StringConexao).SalvarTransbordo(pedido, cargaIntegracaoMulti.Transbordo, ref mensagemErro, unitOfWork, unitOfWork.StringConexao, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);

                    Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga(Conexao.StringConexao);
                    cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracaoMulti, ref protocoloPedidoExistente, ref mensagemErro, ref codigoCargaExistente, unitOfWork, TipoServicoMultisoftware, false, false, Auditado, configuracaoTMS, ClienteAcesso, Conexao.AdminStringConexao, filial, tipoOperacao);

                    if (cargaPedido != null)
                    {
                        servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracaoMulti, ref mensagemErro, unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

                        if (cargaIntegracao.NotasFiscais != null && cargaIntegracao.NotasFiscais.Count > 0)
                        {
                            foreach (var notaFiscal in cargaIntegracao.NotasFiscais)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial();
                                cargaPedidoXMLNotaFiscalParcial.CargaPedido = cargaPedido;
                                cargaPedidoXMLNotaFiscalParcial.NotaEnviadaIntegralmente = false;
                                cargaPedidoXMLNotaFiscalParcial.VincularNotaFiscalPorProcesso = true;
                                cargaPedidoXMLNotaFiscalParcial.Chave = Utilidades.String.OnlyNumbers(notaFiscal.IDNFe);
                                cargaPedidoXMLNotaFiscalParcial.Numero = Utilidades.Chave.ObterNumero(cargaPedidoXMLNotaFiscalParcial.Chave);
                                repCargaPedidoXMLNotaFiscalParcial.Inserir(cargaPedidoXMLNotaFiscalParcial);
                            }
                        }

                        if (cargaIntegracaoMulti.FecharCargaAutomaticamente)
                        {
                            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                            if (configuracaoTMS.AgruparCargaAutomaticamente)
                                cargaPedido.Carga.AgruparCargaAutomaticamente = true;
                            else if (!configuracaoTMS.FecharCargaPorThread)
                            {
                                new Servicos.Embarcador.Carga.Carga(Conexao.StringConexao).FecharCarga(cargaPedido.Carga, unitOfWork, TipoServicoMultisoftware, Cliente);

                                cargaPedido.Carga.CargaFechada = true;
                                Servicos.Log.TratarErro("22 - Fechou Carga (" + cargaPedido.Carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");


                                new Repositorio.Embarcador.Cargas.Carga(unitOfWork).Atualizar(cargaPedido.Carga);
                            }
                            else
                                cargaPedido.Carga.FechandoCarga = true;

                            repCarga.Atualizar(cargaPedido.Carga);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, "Solicitou o fechamento da carga. Protocolo " + cargaPedido.Carga.Codigo.ToString(), unitOfWork);
                        }
                    }
                }

                if (mensagemErro.Length > 0)
                {
                    Servicos.Log.TratarErro($"Carga: {cargaIntegracaoMulti.NumeroCarga} Retornou essa mensagem: {mensagemErro.ToString()}");
                    unitOfWork.Rollback();

                    if ((codigoCargaExistente > 0) || (protocoloPedidoExistente > 0 && string.IsNullOrWhiteSpace(cargaIntegracaoMulti.NumeroCarga)))
                    {
                        Servicos.Log.TratarErro($"codigoCargaExistente: {codigoCargaExistente} protocoloPedidoExistente: {protocoloPedidoExistente}");
                        bool retornarDuplicidade = true;
                        if (configuracaoTMS.RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto)
                        {
                            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(codigoCargaExistente);
                            if (cargaCancelamento != null)
                                retornarDuplicidade = false;
                        }

                        if (retornarDuplicidade)
                        {
                            if (configuracaoTMS.RetornosDuplicidadeWSSubstituirPorSucesso)
                            {
                                if (cargaIntegracaoMulti.FecharCargaAutomaticamente && configuracaoTMS.FecharCargaPorThread && !configuracaoTMS.AgruparCargaAutomaticamente && codigoCargaExistente > 0)
                                {
                                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                                    Dominio.Entidades.Embarcador.Cargas.Carga cargaFechamento = repCarga.BuscarPorCodigo(codigoCargaExistente);
                                    if (cargaFechamento != null && !cargaFechamento.CargaFechada)
                                    {
                                        cargaFechamento.FechandoCarga = true;
                                        repCarga.Atualizar(cargaFechamento);
                                    }
                                }

                                if (protocoloPedidoExistente == 0)
                                    protocoloPedidoExistente = repCargaPedido.BuscarPorCarga(codigoCargaExistente)?.FirstOrDefault().Pedido.Codigo ?? 0;

                                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = codigoCargaExistente, protocoloIntegracaoPedido = protocoloPedidoExistente, ParametroIdentificacaoCliente = cargaIntegracaoMulti.ParametroIdentificacaoCliente });
                            }
                            else
                            {
                                AuditarRetornoDuplicidadeDaRequisicao(unitOfWork, mensagemErro.ToString(), cargaIntegracaoMulti.NumeroCarga);
                                servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracaoMulti, mensagemErro.ToString());
                                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDuplicidadeRequisicao(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = codigoCargaExistente, protocoloIntegracaoPedido = protocoloPedidoExistente, ParametroIdentificacaoCliente = cargaIntegracaoMulti.ParametroIdentificacaoCliente });
                            }
                        }
                        else
                        {
                            AuditarRetornoDadosInvalidos(unitOfWork, mensagemErro.ToString(), cargaIntegracaoMulti.NumeroCarga);
                            servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracaoMulti, mensagemErro.ToString());
                            return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                        }
                    }
                    else
                    {
                        AuditarRetornoDadosInvalidos(unitOfWork, mensagemErro.ToString(), cargaIntegracaoMulti.NumeroCarga);
                        servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracaoMulti, mensagemErro.ToString());
                        return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                    }
                }

                servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaComSucesso(cargaPedido);

                unitOfWork.CommitChanges();

                //Servicos.Embarcador.Carga.Frete.AlertarRotaNaoCadastradaPorPedido(pedido, unitOfWork, TipoServicoMultisoftware);
                //Servicos.Embarcador.Carga.Frete.AlertarFaltaTabelaFretePorPedido(pedido, unitOfWork, TipoServicoMultisoftware);

                if (cargaPedido != null && cargaPedido.Carga != null)
                    Servicos.Log.TratarErro($"AdicionarCarga Retorno: Protocolo carga = {cargaPedido.Carga.Codigo}, protocolo pedido = {pedido.Codigo}");
                else if (pedido != null)
                    Servicos.Log.TratarErro($"AdicionarCarga Retorno: Protocolo pedido = {pedido.Codigo}");

                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo /*cargaPedido.Carga.Codigo*/ : 0, protocoloIntegracaoPedido = pedido?.Protocolo /*pedido?.Codigo*/ ?? 0 });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroDoEmbarcador} retornou exceção a seguir:");

                try
                {
                    string objetoJson = Newtonsoft.Json.JsonConvert.SerializeObject(cargaIntegracao);

                    ArmazenarLogParametros(objetoJson);
                }
                catch (Exception excecaoLog)
                {
                    Servicos.Log.TratarErro($"Falha ao serializar pedido para log: " + excecaoLog);
                }

                return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoExcecao($"Ocorreu uma falha ao obter os dados das integrações. {mensagemErro.ToString()}");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> EncerrarCarga(int protocoloIntegracaoCarga, string ObservacaoEncerramento)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(Conexao.StringConexao);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloIntegracaoCarga);

                if (carga == null)
                {
                    Servicos.Log.TratarErro($"EncerrarCarga, carga protocolo: {protocoloIntegracaoCarga} não localizada.");
                    retorno.Objeto = false;
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = $"Carga protocolo: {protocoloIntegracaoCarga} não localizada.";
                }
                else
                {
                    unitOfWork.Start();

                    retorno.Mensagem = serCarga.SolicitarEncerramentoCarga(carga.Codigo, ObservacaoEncerramento, ClienteAcesso.WebServiceConsultaCTe, TipoServicoMultisoftware, unitOfWork, Auditado);

                    if (string.IsNullOrWhiteSpace(retorno.Mensagem))
                    {
                        retorno.Objeto = true;
                        retorno.Status = true;
                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        retorno.Objeto = false;
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        unitOfWork.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao encerrar a carga";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }


        public Retorno<int> CadastrarApoliceSeguro(Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro apoliceSeguro)
        {
            Servicos.Log.TratarErro("CadastrarApoliceSeguro " + Newtonsoft.Json.JsonConvert.SerializeObject(apoliceSeguro));
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Retorno<int> retorno = new Retorno<int>() { Status = true };
            try
            {
                Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
                Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);
                Repositorio.Embarcador.Seguros.AverbacaoBradesco repAverbacaoBradesco = new Repositorio.Embarcador.Seguros.AverbacaoBradesco(unitOfWork);
                Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro repAverbacaoPortoSeguro = new Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro(unitOfWork);
                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                if (apoliceSeguro == null)
                    return new Retorno<int>() { Mensagem = "Objeto apolice de seguro não recebido.", Status = false };

                if (string.IsNullOrWhiteSpace(apoliceSeguro.NumeroApolice))
                    return new Retorno<int>() { Mensagem = "Número da apolice de seguro é obrigatório.", Status = false };

                if (apoliceSeguro.Seguradora == null)
                    return new Retorno<int>() { Mensagem = "Seguradora é obrigatório.", Status = false };

                DateTime dataInicio = DateTime.Now;
                if (!DateTime.TryParseExact(apoliceSeguro.DataInicioVigencia, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio))
                    return new Retorno<int>() { Mensagem = "Data inicio vigência é obrigatório no formato dd/MM/yyyy.", Status = false };

                DateTime dataFim = DateTime.Now;
                if (!DateTime.TryParseExact(apoliceSeguro.DataFimVigencia, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim))
                    return new Retorno<int>() { Mensagem = "Data inicio vigência é obrigatório no formato dd/MM/yyyy.", Status = false };

                Dominio.Entidades.Embarcador.Seguros.Seguradora seguradora = null;
                if (!Utilidades.Validate.ValidarCNPJ(apoliceSeguro.Seguradora.CPFCNPJ))
                    return new Retorno<int>() { Mensagem = "CNPJ da Seguradora é inválido.", Status = false };
                else
                {
                    seguradora = repSeguradora.BuscarPorCNPJ(double.Parse(apoliceSeguro.Seguradora.CPFCNPJ));
                    if (seguradora == null)
                    {
                        Dominio.Entidades.Cliente clienteSeguradora = repCliente.BuscarPorCPFCNPJ(double.Parse(apoliceSeguro.Seguradora.CPFCNPJ));
                        if (clienteSeguradora == null)
                        {
                            if (string.IsNullOrWhiteSpace(apoliceSeguro.Seguradora.RazaoSocial))
                                return new Retorno<int>() { Mensagem = "Seguradora não possui cadastro, favor enviar todos os dados na integração ou realizar o cadastro pelo portal.", Status = false };

                            Servicos.Cliente srvCliente = new Servicos.Cliente();
                            var retornoVerificacaoCliente = srvCliente.ConverterObjetoValorPessoa(apoliceSeguro.Seguradora, "Seguradora", unitOfWork);
                            if ((retornoVerificacaoCliente?.Status ?? false) && retornoVerificacaoCliente?.cliente != null)
                                clienteSeguradora = retornoVerificacaoCliente?.cliente;

                            if (clienteSeguradora == null)
                                return new Retorno<int>() { Mensagem = "Não foi possível cadastrar seguradora como cliente.", Status = false };

                            seguradora = new Dominio.Entidades.Embarcador.Seguros.Seguradora();
                            seguradora.ClienteSeguradora = clienteSeguradora;
                            seguradora.Ativo = true;
                            seguradora.Nome = clienteSeguradora.Nome;

                            repSeguradora.Inserir(seguradora, Auditado);
                        }
                    }
                }

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguroCadastro = repApoliceSeguro.BuscarPorApoliceSeguradoraVigencia(apoliceSeguro.NumeroApolice, seguradora?.Codigo ?? 0, dataInicio, dataFim);
                if (apoliceSeguroCadastro != null)
                    return new Retorno<int>() { Mensagem = "Já existe apólice cadastrada com mesmo Número, seguradora e Vigência.", Status = false };

                unitOfWork.Start();

                apoliceSeguroCadastro = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro();
                apoliceSeguroCadastro.Seguradora = seguradora;
                apoliceSeguroCadastro.NumeroApolice = apoliceSeguro.NumeroApolice;
                apoliceSeguroCadastro.NumeroAverbacao = apoliceSeguro.NumeroAverbacao;
                apoliceSeguroCadastro.InicioVigencia = dataInicio;
                apoliceSeguroCadastro.FimVigencia = dataFim;
                apoliceSeguroCadastro.Responsavel = apoliceSeguro.Responsavel;
                apoliceSeguroCadastro.SeguradoraAverbacao = apoliceSeguro.Averbadora;
                apoliceSeguroCadastro.ValorLimiteApolice = apoliceSeguro.ValorLimiteApolice;
                apoliceSeguroCadastro.Observacao = apoliceSeguro.Observacao;

                repApoliceSeguro.Inserir(apoliceSeguroCadastro, Auditado);

                if (apoliceSeguro.Averbacao != null)
                {
                    if (apoliceSeguroCadastro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM)
                    {
                        Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoATM();
                        averbacao.ApoliceSeguro = apoliceSeguroCadastro;
                        averbacao.CodigoATM = apoliceSeguro.Averbacao.Codigo;
                        averbacao.Usuario = apoliceSeguro.Averbacao.Usuario;
                        averbacao.Senha = apoliceSeguro.Averbacao.Senha;
                        repAverbacaoATM.Inserir(averbacao);
                    }
                    else if (apoliceSeguroCadastro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Bradesco)
                    {
                        Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco();
                        averbacao.ApoliceSeguro = apoliceSeguroCadastro;
                        averbacao.Token = apoliceSeguro.Averbacao.Token;
                        averbacao.WSDLQuorum = apoliceSeguro.Averbacao.WSDL;
                        repAverbacaoBradesco.Inserir(averbacao);
                    }
                    else if (apoliceSeguroCadastro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.PortoSeguro)
                    {
                        Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro();
                        averbacao.ApoliceSeguro = apoliceSeguroCadastro;
                        averbacao.Usuario = apoliceSeguro.Averbacao.Usuario;
                        averbacao.Senha = apoliceSeguro.Averbacao.Senha;
                        repAverbacaoPortoSeguro.Inserir(averbacao);
                    }
                }

                unitOfWork.CommitChanges();

                if (apoliceSeguroCadastro != null)
                {
                    retorno.Objeto = apoliceSeguroCadastro.Codigo;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi cadastrar apolice de seguro.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao cadastrar apolice de seguro.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<int> AtualizarApoliceSeguro(Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro apoliceSeguro, int codigo)
        {
            Servicos.Log.TratarErro("CadastrarApoliceSeguro " + Newtonsoft.Json.JsonConvert.SerializeObject(apoliceSeguro));
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Retorno<int> retorno = new Retorno<int>() { Status = true };
            try
            {
                Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
                Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);
                Repositorio.Embarcador.Seguros.AverbacaoBradesco repAverbacaoBradesco = new Repositorio.Embarcador.Seguros.AverbacaoBradesco(unitOfWork);
                Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro repAverbacaoPortoSeguro = new Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro(unitOfWork);
                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                if (codigo == 0)
                    return new Retorno<int>() { Mensagem = "Obrigatório enviar o código da apólice para atualização.", Status = false };

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguroCadastro = repApoliceSeguro.BuscarPorCodigo(codigo);
                if (apoliceSeguroCadastro == null)
                    return new Retorno<int>() { Mensagem = "Apólice de seguro código " + codigo.ToString() + " não localizada.", Status = false };

                if (apoliceSeguro == null)
                    return new Retorno<int>() { Mensagem = "Objeto apolice de seguro não recebido.", Status = false };

                if (string.IsNullOrWhiteSpace(apoliceSeguro.NumeroApolice))
                    return new Retorno<int>() { Mensagem = "Número da apolice de seguro é obrigatório.", Status = false };

                DateTime dataInicio = DateTime.Now;
                if (!DateTime.TryParseExact(apoliceSeguro.DataInicioVigencia, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio))
                    return new Retorno<int>() { Mensagem = "Data inicio vigência é obrigatório no formato dd/MM/yyyy.", Status = false };

                DateTime dataFim = DateTime.Now;
                if (!DateTime.TryParseExact(apoliceSeguro.DataFimVigencia, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim))
                    return new Retorno<int>() { Mensagem = "Data inicio vigência é obrigatório no formato dd/MM/yyyy.", Status = false };

                Dominio.Entidades.Embarcador.Seguros.Seguradora seguradora = null;
                if (apoliceSeguro.Seguradora != null)
                {
                    if (!Utilidades.Validate.ValidarCNPJ(apoliceSeguro.Seguradora.CPFCNPJ))
                        return new Retorno<int>() { Mensagem = "CNPJ da Seguradora é inválido.", Status = false };
                    else
                    {
                        seguradora = repSeguradora.BuscarPorCNPJ(double.Parse(apoliceSeguro.Seguradora.CPFCNPJ));
                        if (seguradora == null)
                        {
                            Dominio.Entidades.Cliente clienteSeguradora = repCliente.BuscarPorCPFCNPJ(double.Parse(apoliceSeguro.Seguradora.CPFCNPJ));
                            if (clienteSeguradora == null)
                            {
                                if (string.IsNullOrWhiteSpace(apoliceSeguro.Seguradora.RazaoSocial))
                                    return new Retorno<int>() { Mensagem = "Seguradora não possui cadastro, favor enviar todos os dados na integração ou realizar o cadastro pelo portal.", Status = false };

                                Servicos.Cliente srvCliente = new Servicos.Cliente();
                                var retornoVerificacaoCliente = srvCliente.ConverterObjetoValorPessoa(apoliceSeguro.Seguradora, "Seguradora", unitOfWork);
                                if ((retornoVerificacaoCliente?.Status ?? false) && retornoVerificacaoCliente?.cliente != null)
                                    clienteSeguradora = retornoVerificacaoCliente?.cliente;

                                if (clienteSeguradora == null)
                                    return new Retorno<int>() { Mensagem = "Não foi possível cadastrar seguradora como cliente.", Status = false };

                                seguradora = new Dominio.Entidades.Embarcador.Seguros.Seguradora();
                                seguradora.ClienteSeguradora = clienteSeguradora;
                                seguradora.Ativo = true;
                                seguradora.Nome = clienteSeguradora.Nome;

                                repSeguradora.Inserir(seguradora, Auditado);
                            }
                        }
                    }
                }

                unitOfWork.Start();

                if (seguradora != null)
                    apoliceSeguroCadastro.Seguradora = seguradora;
                apoliceSeguroCadastro.NumeroApolice = apoliceSeguro.NumeroApolice;
                apoliceSeguroCadastro.NumeroAverbacao = apoliceSeguro.NumeroAverbacao;
                apoliceSeguroCadastro.InicioVigencia = dataInicio;
                apoliceSeguroCadastro.FimVigencia = dataFim;
                apoliceSeguroCadastro.Responsavel = apoliceSeguro.Responsavel;
                apoliceSeguroCadastro.SeguradoraAverbacao = apoliceSeguro.Averbadora;
                apoliceSeguroCadastro.ValorLimiteApolice = apoliceSeguro.ValorLimiteApolice;
                apoliceSeguroCadastro.Observacao = apoliceSeguro.Observacao;

                repApoliceSeguro.Atualizar(apoliceSeguroCadastro); //Auditado

                if (apoliceSeguro.Averbacao != null)
                {
                    if (apoliceSeguroCadastro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM)
                    {
                        bool cadastrarAverbacao = false;
                        Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacao = repAverbacaoATM.BuscarPorApolice(apoliceSeguroCadastro.Codigo);
                        if (averbacao == null)
                        {
                            cadastrarAverbacao = true;
                            averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoATM();
                        }
                        averbacao.ApoliceSeguro = apoliceSeguroCadastro;
                        averbacao.CodigoATM = apoliceSeguro.Averbacao.Codigo;
                        averbacao.Usuario = apoliceSeguro.Averbacao.Usuario;
                        averbacao.Senha = apoliceSeguro.Averbacao.Senha;
                        if (cadastrarAverbacao)
                            repAverbacaoATM.Inserir(averbacao);
                        else
                            repAverbacaoATM.Atualizar(averbacao);
                    }
                    else if (apoliceSeguroCadastro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Bradesco)
                    {
                        bool cadastrarAverbacao = false;
                        Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco averbacao = repAverbacaoBradesco.BuscarPorApolice(apoliceSeguroCadastro.Codigo);
                        if (averbacao == null)
                        {
                            cadastrarAverbacao = true;
                            averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco();
                        }
                        averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco();
                        averbacao.ApoliceSeguro = apoliceSeguroCadastro;
                        averbacao.Token = apoliceSeguro.Averbacao.Token;
                        averbacao.WSDLQuorum = apoliceSeguro.Averbacao.WSDL;
                        if (cadastrarAverbacao)
                            repAverbacaoBradesco.Inserir(averbacao);
                        else
                            repAverbacaoBradesco.Atualizar(averbacao);
                    }
                    else if (apoliceSeguroCadastro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.PortoSeguro)
                    {
                        bool cadastrarAverbacao = false;
                        Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro averbacao = repAverbacaoPortoSeguro.BuscarPorApolice(apoliceSeguroCadastro.Codigo);
                        if (averbacao == null)
                        {
                            cadastrarAverbacao = true;
                            averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro();
                        }
                        averbacao = new Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro();
                        averbacao.ApoliceSeguro = apoliceSeguroCadastro;
                        averbacao.Usuario = apoliceSeguro.Averbacao.Usuario;
                        averbacao.Senha = apoliceSeguro.Averbacao.Senha;
                        repAverbacaoPortoSeguro.Inserir(averbacao);

                        if (cadastrarAverbacao)
                            repAverbacaoPortoSeguro.Inserir(averbacao);
                        else
                            repAverbacaoPortoSeguro.Atualizar(averbacao);
                    }
                }

                unitOfWork.CommitChanges();

                if (apoliceSeguroCadastro != null)
                {
                    retorno.Objeto = apoliceSeguroCadastro.Codigo;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi atualizar apolice de seguro.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao cadastrar apolice de seguro.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro>> BuscarApolicesSeguro(int inicio, int limite)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro>>();
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro>();

                    Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
                    int total = repApoliceSeguro.ContarTodas();
                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> listaApolices = repApoliceSeguro.ConsultarTodas(inicio, limite);

                    Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork.StringConexao);

                    var apolices = from o in listaApolices
                                   select new Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro()
                                   {
                                       Codigo = o.Codigo,
                                       Seguradora = serPessoa.ConverterObjetoPessoa(o.Seguradora.ClienteSeguradora),
                                       NumeroApolice = o.NumeroApolice,
                                       NumeroAverbacao = o.NumeroAverbacao,
                                       DataInicioVigencia = o.InicioVigencia.ToString("dd/MM/yyyy"),
                                       DataFimVigencia = o.FimVigencia.ToString("dd/MM/yyyy"),
                                       ValorLimiteApolice = o.ValorLimiteApolice,
                                       Responsavel = o.Responsavel,
                                       Averbadora = o.SeguradoraAverbacao,
                                       Observacao = o.Observacao
                                   };

                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro>();
                    retorno.Objeto.Itens.AddRange(apolices);
                    retorno.Objeto.NumeroTotalDeRegistro = total;

                    retorno.Status = true;
                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Apolices de seguro", unitOfWork);
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar apolices";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }


        #region Métodos Privados

        private Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao ConverterIntegracaoCarga(Dominio.ObjetosDeValor.WebService.CargoX.CargaIntegracao cargaIntegracao, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaRetorno = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();

            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repTipoDeCarga.BuscarPrimeira();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorPrimeiro();

            cargaRetorno.NumeroCarga = cargaIntegracao.NumeroDoFrete;
            cargaRetorno.NumeroPedidoEmbarcador = !string.IsNullOrWhiteSpace(cargaIntegracao.NumeroDoEmbarcador) ? cargaIntegracao.NumeroDoEmbarcador : cargaIntegracao.NumeroDoFrete;
            cargaRetorno.ObservacaoCTe = cargaIntegracao.Observacao;
            cargaRetorno.FecharCargaAutomaticamente = true;
            //cargaRetorno.TipoRateio = cargaIntegracao.TipoRateio;
            //cargaRetorno.TipoAverbacao = cargaIntegracao.TipoAverbacao;

            if (tipoDeCarga != null)
            {
                cargaRetorno.TipoCargaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador();
                cargaRetorno.TipoCargaEmbarcador.CodigoIntegracao = tipoDeCarga.CodigoTipoCargaEmbarcador;
            }

            if (tipoOperacao != null)
            {
                cargaRetorno.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao();
                cargaRetorno.TipoOperacao.CodigoIntegracao = tipoOperacao.CodigoIntegracao;
            }

            Dominio.Entidades.Empresa empresa = null;
            if (!string.IsNullOrWhiteSpace(cargaIntegracao.CNPJTransportadoraEmitente))
            {
                empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cargaIntegracao.CNPJTransportadoraEmitente));

                if (empresa == null)
                {
                    stMensagem.Append("Não foi encontrado um transportador para o CNPJ " + cargaIntegracao.CNPJTransportadoraEmitente + " na base da Multisoftware");
                }
                else
                {
                    if (empresa.EmpresaPai != null && empresa.EmpresaPai.TipoAmbiente != empresa.TipoAmbiente)
                        stMensagem.Append("A empresa informada não está apta a emitir em ambiente de " + empresa.EmpresaPai.DescricaoTipoAmbiente);
                }

                if (stMensagem.Length > 0)
                    return null;

                cargaRetorno.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                cargaRetorno.TransportadoraEmitente.CNPJ = Utilidades.String.OnlyNumbers(cargaIntegracao.CNPJTransportadoraEmitente);
            }

            else
            {
                empresa = repEmpresa.BuscarPrincipalEmissoraTMS();
                if (empresa != null)
                {
                    cargaRetorno.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                    cargaRetorno.TransportadoraEmitente.CNPJ = empresa.CNPJ;
                }
            }


            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa tomador = null;
            if (cargaIntegracao.Tomador != null)
            {
                string cnpjTomador = Utilidades.String.OnlyNumbers(cargaIntegracao.Tomador.CPFCNPJ);
                string inscricaoEstadual = Utilidades.String.OnlyNumbers(cargaIntegracao.Tomador.InscricaoEstadual);
                string cep = Utilidades.String.OnlyNumbers(cargaIntegracao.Tomador.CEP);
                if (string.IsNullOrWhiteSpace(cnpjTomador))
                    stMensagem.Append("Obrigatório o envio do CNPJ/CPF do Tomador.");
                else
                {
                    if (cargaIntegracao.Tomador.TipoPessoa == Dominio.Enumeradores.TipoPessoa.Fisica && !Utilidades.Validate.ValidarCPF(cnpjTomador))
                        stMensagem.Append("CPF do Tomador não é válido.");
                    else if (cargaIntegracao.Tomador.TipoPessoa == Dominio.Enumeradores.TipoPessoa.Juridica && !Utilidades.Validate.ValidarCNPJ(cnpjTomador))
                        stMensagem.Append("CPF do Tomador não é válido.");
                }

                if (cargaIntegracao.Tomador.CodigoAtividade <= 0 || cargaIntegracao.Tomador.CodigoAtividade > 7)
                    stMensagem.Append("Atividade do Tomador não é válida.");

                if (string.IsNullOrWhiteSpace(cargaIntegracao.Tomador.Bairro) || cargaIntegracao.Tomador.Bairro.Length <= 2)
                    stMensagem.Append("Bairro do Tomador não é válida.");

                if (string.IsNullOrWhiteSpace(cargaIntegracao.Tomador.Rua) || cargaIntegracao.Tomador.Rua.Length <= 2)
                    stMensagem.Append("Rua do Tomador não é válida.");

                if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8)
                    stMensagem.Append("CEP do Tomador não é válida.");

                if (cargaIntegracao.Tomador.IBGE == 0)
                    stMensagem.Append("IBGE do Tomador não é válida.");

                if (stMensagem.Length > 0)
                    return null;

                tomador = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                tomador.CPFCNPJ = cnpjTomador;
                tomador.CodigoAtividade = cargaIntegracao.Tomador.CodigoAtividade;
                tomador.NomeFantasia = cargaIntegracao.Tomador.NomeFantasia;
                tomador.RazaoSocial = cargaIntegracao.Tomador.RazaoSocial;
                tomador.RGIE = !string.IsNullOrWhiteSpace(inscricaoEstadual) ? inscricaoEstadual : "ISENTO";
                tomador.RNTRC = Utilidades.String.OnlyNumbers(cargaIntegracao.Tomador.RNTRC);
                tomador.TipoPessoa = cargaIntegracao.Tomador.TipoPessoa;
                tomador.Email = cargaIntegracao.Tomador.Email;
                tomador.AtualizarEnderecoPessoa = true;
                tomador.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                tomador.Endereco.Bairro = cargaIntegracao.Tomador.Bairro;
                tomador.Endereco.CEP = cep;
                tomador.Endereco.Complemento = cargaIntegracao.Tomador.Complemento;
                tomador.Endereco.Numero = !string.IsNullOrWhiteSpace(cargaIntegracao.Tomador.Numero) ? cargaIntegracao.Tomador.Numero : "S/N";
                tomador.Endereco.Logradouro = cargaIntegracao.Tomador.Rua;
                tomador.Endereco.Telefone = Utilidades.String.OnlyNumbers(cargaIntegracao.Tomador.Telefone);
                tomador.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                tomador.Endereco.Cidade.IBGE = cargaIntegracao.Tomador.IBGE;

                cargaRetorno.Tomador = tomador;
                cargaRetorno.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;

                cargaRetorno.Remetente = tomador;
                cargaRetorno.Destinatario = tomador;
            }
            else
                stMensagem.Append("Dados do tomador são obrigatórios.");

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa subcontratado = null;
            if (cargaIntegracao.Subcontratado != null)
            {
                string cnpjSubcontratado = Utilidades.String.OnlyNumbers(cargaIntegracao.Subcontratado.CPFCNPJ);
                string inscricaoEstadual = Utilidades.String.OnlyNumbers(cargaIntegracao.Subcontratado.InscricaoEstadual);
                string cep = Utilidades.String.OnlyNumbers(cargaIntegracao.Subcontratado.CEP);
                if (string.IsNullOrWhiteSpace(cnpjSubcontratado))
                    stMensagem.Append("Obrigatório o envio do CNPJ/CPF do Subcontratado.");
                else
                {
                    if (cargaIntegracao.Subcontratado.TipoPessoa == Dominio.Enumeradores.TipoPessoa.Fisica && !Utilidades.Validate.ValidarCPF(cnpjSubcontratado))
                        stMensagem.Append("CPF do subcontratado não é válido.");
                    else if (cargaIntegracao.Subcontratado.TipoPessoa == Dominio.Enumeradores.TipoPessoa.Juridica && !Utilidades.Validate.ValidarCNPJ(cnpjSubcontratado))
                        stMensagem.Append("CPF do subcontratado não é válido.");
                }

                if (cargaIntegracao.Subcontratado.CodigoAtividade <= 0 || cargaIntegracao.Subcontratado.CodigoAtividade > 7)
                    stMensagem.Append("Atividade do subcontratado não é válida.");

                if (string.IsNullOrWhiteSpace(cargaIntegracao.Subcontratado.Bairro) || cargaIntegracao.Subcontratado.Bairro.Length <= 2)
                    stMensagem.Append("Bairro do subcontratado não é válida.");

                if (string.IsNullOrWhiteSpace(cargaIntegracao.Subcontratado.Rua) || cargaIntegracao.Subcontratado.Rua.Length <= 2)
                    stMensagem.Append("Rua do subcontratado não é válida.");

                if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8)
                    stMensagem.Append("CEP do subcontratado não é válida.");

                if (cargaIntegracao.Subcontratado.IBGE == 0)
                    stMensagem.Append("IBGE do subcontratado não é válida.");

                if (stMensagem.Length > 0)
                    return null;

                subcontratado = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                subcontratado.CPFCNPJ = cnpjSubcontratado;
                subcontratado.CodigoAtividade = cargaIntegracao.Subcontratado.CodigoAtividade;
                subcontratado.NomeFantasia = cargaIntegracao.Subcontratado.NomeFantasia;
                subcontratado.RazaoSocial = cargaIntegracao.Subcontratado.RazaoSocial;
                subcontratado.RGIE = !string.IsNullOrWhiteSpace(inscricaoEstadual) ? inscricaoEstadual : "ISENTO";
                subcontratado.RNTRC = Utilidades.String.OnlyNumbers(cargaIntegracao.Subcontratado.RNTRC);
                subcontratado.TipoPessoa = cargaIntegracao.Subcontratado.TipoPessoa;
                subcontratado.Email = cargaIntegracao.Subcontratado.Email;
                subcontratado.AtualizarEnderecoPessoa = true;
                subcontratado.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                subcontratado.Endereco.Bairro = cargaIntegracao.Subcontratado.Bairro;
                subcontratado.Endereco.CEP = cep;
                subcontratado.Endereco.Complemento = cargaIntegracao.Subcontratado.Complemento;
                subcontratado.Endereco.Numero = !string.IsNullOrWhiteSpace(cargaIntegracao.Subcontratado.Numero) ? cargaIntegracao.Subcontratado.Numero : "S/N";
                subcontratado.Endereco.Logradouro = cargaIntegracao.Subcontratado.Rua;
                subcontratado.Endereco.Telefone = Utilidades.String.OnlyNumbers(cargaIntegracao.Subcontratado.Telefone);
                subcontratado.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                subcontratado.Endereco.Cidade.IBGE = cargaIntegracao.Subcontratado.IBGE;
            }

            if (configuracao.UtilizarProdutosDiversosNaIntegracaoDaCarga)
            {
                cargaRetorno.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                produto.CodigoGrupoProduto = "1";
                produto.CodigoProduto = "1";
                produto.DescricaoGrupoProduto = "Diversos";
                produto.DescricaoProduto = "Diversos";
                produto.Quantidade = 1;
                produto.PesoUnitario = 0;
                cargaRetorno.Produtos.Add(produto);
            }

            if (configuracao.ModeloVeicularCargaPadraoImportacaoPedido != null)
            {
                cargaRetorno.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular();
                cargaRetorno.ModeloVeicular.CodigoIntegracao = configuracao.ModeloVeicularCargaPadraoImportacaoPedido.CodigoIntegracao;
            }

            if (cargaIntegracao.Veiculos != null && cargaIntegracao.Veiculos.Count > 0)
            {
                var listaVeiculos = cargaIntegracao.Veiculos.OrderBy(o => o.TipoVeiculo);

                foreach (var veiculoIntegracao in listaVeiculos)
                {
                    if (veiculoIntegracao.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao && cargaRetorno.Veiculo == null)
                    {
                        if (string.IsNullOrWhiteSpace(veiculoIntegracao.Placa))
                            stMensagem.Append("Obrigatório informar a placa do veículo.");

                        if (string.IsNullOrWhiteSpace(veiculoIntegracao.Renavam))
                            stMensagem.Append("Obrigatório informar o Renavam do veículo.");

                        if (string.IsNullOrWhiteSpace(veiculoIntegracao.UF))
                            stMensagem.Append("Obrigatório informar o estado (UF) do veículo.");

                        if (veiculoIntegracao.Tara <= 0)
                            stMensagem.Append("Obrigatório informar a tara do veículo.");

                        if (stMensagem.Length > 0)
                            return null;

                        cargaRetorno.Veiculo = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();
                        cargaRetorno.Veiculo.Placa = veiculoIntegracao.Placa;
                        cargaRetorno.Veiculo.CapacidadeKG = veiculoIntegracao.CapacidadeKG;
                        cargaRetorno.Veiculo.CapacidadeM3 = veiculoIntegracao.CapacidadeM3;
                        cargaRetorno.Veiculo.Renavam = veiculoIntegracao.Renavam;
                        cargaRetorno.Veiculo.RNTC = veiculoIntegracao.RNTRC;
                        cargaRetorno.Veiculo.Tara = veiculoIntegracao.Tara;
                        cargaRetorno.Veiculo.TipoCarroceria = veiculoIntegracao.TipoCarroceria;
                        cargaRetorno.Veiculo.TipoRodado = veiculoIntegracao.TipoRodado;
                        cargaRetorno.Veiculo.TipoVeiculo = veiculoIntegracao.TipoVeiculo;
                        cargaRetorno.Veiculo.UF = veiculoIntegracao.UF;
                        cargaRetorno.Veiculo.TipoPropriedadeVeiculo = TipoPropriedadeVeiculo.Proprio;

                        if (veiculoIntegracao.Proprietario != null)
                        {
                            string cnpjProprietario = Utilidades.String.OnlyNumbers(veiculoIntegracao.Proprietario.CPFCNPJ);
                            string inscricaoEstadual = Utilidades.String.OnlyNumbers(veiculoIntegracao.Proprietario.InscricaoEstadual);
                            string cep = Utilidades.String.OnlyNumbers(veiculoIntegracao.Proprietario.CEP);
                            if (string.IsNullOrWhiteSpace(cnpjProprietario))
                                stMensagem.Append("Obrigatório o envio do CNPJ/CPF do proprietario.");
                            else
                            {
                                if (veiculoIntegracao.Proprietario.TipoPessoa == Dominio.Enumeradores.TipoPessoa.Fisica && !Utilidades.Validate.ValidarCPF(cnpjProprietario))
                                    stMensagem.Append("CPF do proprietario não é válido.");
                                else if (veiculoIntegracao.Proprietario.TipoPessoa == Dominio.Enumeradores.TipoPessoa.Juridica && !Utilidades.Validate.ValidarCNPJ(cnpjProprietario))
                                    stMensagem.Append("CPF do proprietario não é válido.");
                            }

                            if (veiculoIntegracao.Proprietario.CodigoAtividade <= 0 || veiculoIntegracao.Proprietario.CodigoAtividade > 7)
                                stMensagem.Append("Atividade do proprietario não é válida.");

                            if (string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.Bairro) || veiculoIntegracao.Proprietario.Bairro.Length <= 2)
                                stMensagem.Append("Bairro do proprietario não é válida.");

                            if (string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.Rua) || veiculoIntegracao.Proprietario.Rua.Length <= 2)
                                stMensagem.Append("Rua do proprietario não é válida.");

                            if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8)
                                stMensagem.Append("CEP do proprietario não é válida.");

                            if (veiculoIntegracao.Proprietario.IBGE == 0)
                                stMensagem.Append("IBGE do proprietario não é válida.");

                            if (stMensagem.Length > 0)
                                return null;

                            cargaRetorno.Veiculo.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();

                            cargaRetorno.Veiculo.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.CNPJ = cnpjProprietario;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Atividade = veiculoIntegracao.Proprietario.CodigoAtividade;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.NomeFantasia = veiculoIntegracao.Proprietario.NomeFantasia;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.RazaoSocial = veiculoIntegracao.Proprietario.RazaoSocial;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.IE = !string.IsNullOrWhiteSpace(inscricaoEstadual) ? inscricaoEstadual : "ISENTO";
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.RNTRC = Utilidades.String.OnlyNumbers(veiculoIntegracao.Proprietario.RNTRC);
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Emails = veiculoIntegracao.Proprietario.Email;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Bairro = veiculoIntegracao.Proprietario.Bairro;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.CEP = cep;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Complemento = veiculoIntegracao.Proprietario.Complemento;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Numero = !string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.Numero) ? veiculoIntegracao.Proprietario.Numero : "S/N";
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Logradouro = veiculoIntegracao.Proprietario.Rua;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Telefone = Utilidades.String.OnlyNumbers(veiculoIntegracao.Proprietario.Telefone);
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Cidade.IBGE = veiculoIntegracao.Proprietario.IBGE;

                            cargaRetorno.Veiculo.TipoPropriedadeVeiculo = TipoPropriedadeVeiculo.Terceiros;
                        }
                        else if (subcontratado != null)
                        {
                            cargaRetorno.Veiculo.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();

                            cargaRetorno.Veiculo.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.CNPJ = subcontratado.CPFCNPJ;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Atividade = subcontratado.CodigoAtividade;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.NomeFantasia = subcontratado.NomeFantasia;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.RazaoSocial = subcontratado.RazaoSocial;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.IE = subcontratado.RGIE;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.RNTRC = subcontratado.RNTRC;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Emails = subcontratado.Email;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Bairro = subcontratado.Endereco?.Bairro;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.CEP = subcontratado.Endereco?.CEP;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Complemento = subcontratado.Endereco?.Complemento;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Numero = subcontratado.Endereco?.Numero;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Logradouro = subcontratado.Endereco?.Logradouro;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Telefone = subcontratado.Endereco?.Telefone;
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                            cargaRetorno.Veiculo.Proprietario.TransportadorTerceiro.Endereco.Cidade.IBGE = subcontratado.Endereco?.Cidade?.IBGE ?? 0;

                            cargaRetorno.Veiculo.TipoPropriedadeVeiculo = TipoPropriedadeVeiculo.Terceiros;
                        }
                    }

                    if (veiculoIntegracao.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque)
                    {
                        if (cargaRetorno.Veiculo.Reboques == null)
                            cargaRetorno.Veiculo.Reboques = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();

                        var reboque = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();

                        if (string.IsNullOrWhiteSpace(veiculoIntegracao.Placa))
                            stMensagem.Append("Obrigatório informar a placa do veículo.");

                        if (string.IsNullOrWhiteSpace(veiculoIntegracao.Renavam))
                            stMensagem.Append("Obrigatório informar o Renavam do veículo.");

                        if (string.IsNullOrWhiteSpace(veiculoIntegracao.UF))
                            stMensagem.Append("Obrigatório informar o estado (UF) do veículo.");

                        if (veiculoIntegracao.Tara <= 0)
                            stMensagem.Append("Obrigatório informar a tara do veículo.");

                        if (stMensagem.Length > 0)
                            return null;

                        reboque.Placa = veiculoIntegracao.Placa;
                        reboque.CapacidadeKG = veiculoIntegracao.CapacidadeKG;
                        reboque.CapacidadeM3 = veiculoIntegracao.CapacidadeM3;
                        reboque.Renavam = veiculoIntegracao.Renavam;
                        reboque.RNTC = veiculoIntegracao.RNTRC;
                        reboque.Tara = veiculoIntegracao.Tara;
                        reboque.TipoCarroceria = veiculoIntegracao.TipoCarroceria;
                        reboque.TipoRodado = veiculoIntegracao.TipoRodado;
                        reboque.TipoVeiculo = veiculoIntegracao.TipoVeiculo;
                        reboque.UF = veiculoIntegracao.UF;

                        if (veiculoIntegracao.Proprietario != null)
                        {
                            string cnpjProprietario = Utilidades.String.OnlyNumbers(veiculoIntegracao.Proprietario.CPFCNPJ);
                            string inscricaoEstadual = Utilidades.String.OnlyNumbers(veiculoIntegracao.Proprietario.InscricaoEstadual);
                            string cep = Utilidades.String.OnlyNumbers(veiculoIntegracao.Proprietario.CEP);
                            if (string.IsNullOrWhiteSpace(cnpjProprietario))
                                stMensagem.Append("Obrigatório o envio do CNPJ/CPF do proprietario.");
                            else
                            {
                                if (veiculoIntegracao.Proprietario.TipoPessoa == Dominio.Enumeradores.TipoPessoa.Fisica && !Utilidades.Validate.ValidarCPF(cnpjProprietario))
                                    stMensagem.Append("CPF do proprietario não é válido.");
                                else if (veiculoIntegracao.Proprietario.TipoPessoa == Dominio.Enumeradores.TipoPessoa.Juridica && !Utilidades.Validate.ValidarCNPJ(cnpjProprietario))
                                    stMensagem.Append("CPF do proprietario não é válido.");
                            }

                            if (veiculoIntegracao.Proprietario.CodigoAtividade <= 0 || veiculoIntegracao.Proprietario.CodigoAtividade > 7)
                                stMensagem.Append("Atividade do proprietario não é válida.");

                            if (string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.Bairro) || veiculoIntegracao.Proprietario.Bairro.Length <= 2)
                                stMensagem.Append("Bairro do proprietario não é válida.");

                            if (string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.Rua) || veiculoIntegracao.Proprietario.Rua.Length <= 2)
                                stMensagem.Append("Rua do proprietario não é válida.");

                            if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8)
                                stMensagem.Append("CEP do proprietario não é válida.");

                            if (veiculoIntegracao.Proprietario.IBGE == 0)
                                stMensagem.Append("IBGE do proprietario não é válida.");

                            if (stMensagem.Length > 0)
                                return null;

                            reboque.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();

                            reboque.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();
                            reboque.Proprietario.TransportadorTerceiro = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                            reboque.Proprietario.TransportadorTerceiro.CNPJ = cnpjProprietario;
                            reboque.Proprietario.TransportadorTerceiro.Atividade = veiculoIntegracao.Proprietario.CodigoAtividade;
                            reboque.Proprietario.TransportadorTerceiro.NomeFantasia = veiculoIntegracao.Proprietario.NomeFantasia;
                            reboque.Proprietario.TransportadorTerceiro.RazaoSocial = veiculoIntegracao.Proprietario.RazaoSocial;
                            reboque.Proprietario.TransportadorTerceiro.IE = !string.IsNullOrWhiteSpace(inscricaoEstadual) ? inscricaoEstadual : "ISENTO";
                            reboque.Proprietario.TransportadorTerceiro.RNTRC = Utilidades.String.OnlyNumbers(veiculoIntegracao.Proprietario.RNTRC);
                            reboque.Proprietario.TransportadorTerceiro.Emails = veiculoIntegracao.Proprietario.Email;
                            reboque.Proprietario.TransportadorTerceiro.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Bairro = veiculoIntegracao.Proprietario.Bairro;
                            reboque.Proprietario.TransportadorTerceiro.Endereco.CEP = cep;
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Complemento = veiculoIntegracao.Proprietario.Complemento;
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Numero = !string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.Numero) ? veiculoIntegracao.Proprietario.Numero : "S/N";
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Logradouro = veiculoIntegracao.Proprietario.Rua;
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Telefone = Utilidades.String.OnlyNumbers(veiculoIntegracao.Proprietario.Telefone);
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Cidade.IBGE = veiculoIntegracao.Proprietario.IBGE;

                            reboque.TipoPropriedadeVeiculo = TipoPropriedadeVeiculo.Terceiros;
                        }
                        else if (subcontratado != null)
                        {
                            reboque.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();

                            reboque.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();
                            reboque.Proprietario.TransportadorTerceiro = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                            reboque.Proprietario.TransportadorTerceiro.CNPJ = subcontratado.CPFCNPJ;
                            reboque.Proprietario.TransportadorTerceiro.Atividade = subcontratado.CodigoAtividade;
                            reboque.Proprietario.TransportadorTerceiro.NomeFantasia = subcontratado.NomeFantasia;
                            reboque.Proprietario.TransportadorTerceiro.RazaoSocial = subcontratado.RazaoSocial;
                            reboque.Proprietario.TransportadorTerceiro.IE = subcontratado.RGIE;
                            reboque.Proprietario.TransportadorTerceiro.RNTRC = subcontratado.RNTRC;
                            reboque.Proprietario.TransportadorTerceiro.Emails = subcontratado.Email;
                            reboque.Proprietario.TransportadorTerceiro.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Bairro = subcontratado.Endereco?.Bairro;
                            reboque.Proprietario.TransportadorTerceiro.Endereco.CEP = subcontratado.Endereco?.CEP;
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Complemento = subcontratado.Endereco?.Complemento;
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Numero = subcontratado.Endereco?.Numero;
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Logradouro = subcontratado.Endereco?.Logradouro;
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Telefone = subcontratado.Endereco?.Telefone;
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                            reboque.Proprietario.TransportadorTerceiro.Endereco.Cidade.IBGE = subcontratado.Endereco?.Cidade?.IBGE ?? 0;

                            reboque.TipoPropriedadeVeiculo = TipoPropriedadeVeiculo.Terceiros;
                        }

                        cargaRetorno.Veiculo.Reboques.Add(reboque);
                    }

                }

            }

            if (cargaIntegracao.Motoristas != null && cargaIntegracao.Motoristas.Count > 0)
            {
                cargaRetorno.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

                foreach (var motoristaIntegracao in cargaIntegracao.Motoristas)
                {
                    var cpfMotorista = Utilidades.String.OnlyNumbers(motoristaIntegracao.CPF);
                    if (string.IsNullOrWhiteSpace(cpfMotorista) || !Utilidades.Validate.ValidarCPF(cpfMotorista))
                        stMensagem.Append("CPF do motorista inválido.");

                    if (string.IsNullOrWhiteSpace(motoristaIntegracao.Nome))
                        stMensagem.Append("Obrigatório informar o Nome do motorista.");

                    if (stMensagem.Length > 0)
                        return null;

                    var motorista = new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista();
                    motorista.CPF = motoristaIntegracao.CPF;
                    motorista.Nome = motoristaIntegracao.Nome;
                    cargaRetorno.Motoristas.Add(motorista);
                }
            }

            if (cargaIntegracao.ValorFrete != null)
            {
                cargaRetorno.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

                if (cargaIntegracao.ValorFrete.ValorPrestacaoServico > 0)
                    cargaRetorno.ValorFrete.ValorTotalAReceber = cargaIntegracao.ValorFrete.ValorPrestacaoServico;
                else
                    cargaRetorno.ValorFrete.FreteProprio = cargaIntegracao.ValorFrete.Frete;

                if (cargaIntegracao.ValorFrete.ComponentesAdicionais != null)
                {
                    cargaRetorno.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();

                    foreach (var componenteIntegracao in cargaIntegracao.ValorFrete.ComponentesAdicionais)
                    {
                        var componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                        componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                        componente.Componente.CodigoIntegracao = componenteIntegracao.Componente?.CodigoIntegracao;
                        componente.Componente.Descricao = componenteIntegracao.Componente?.Descricao;
                        componente.ValorComponente = componenteIntegracao.ValorComponente;
                        componente.IncluirBaseCalculoICMS = componenteIntegracao.IncluirBaseCalculoICMS;
                        componente.IncluirTotalReceber = true;

                        cargaRetorno.ValorFrete.ComponentesAdicionais.Add(componente);

                        if (componenteIntegracao.Componente != null && !string.IsNullOrWhiteSpace(componenteIntegracao.Componente.Descricao) && !string.IsNullOrWhiteSpace(componenteIntegracao.Componente.CodigoIntegracao))
                        {
                            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigoIntegracao(componenteIntegracao.Componente?.CodigoIntegracao);
                            if (componenteFrete == null)
                            {
                                componenteFrete = new Dominio.Entidades.Embarcador.Frete.ComponenteFrete();
                                componenteFrete.Descricao = componenteIntegracao.Componente.Descricao;
                                componenteFrete.CodigoIntegracao = componenteIntegracao.Componente.CodigoIntegracao;
                                componenteFrete.TipoComponenteFrete = TipoComponenteFrete.OUTROS;
                                componenteFrete.Ativo = true;
                                repComponenteFrete.Inserir(componenteFrete);
                            }
                        }
                    }
                }
            }

            if (cargaRetorno.ValorFrete == null || (cargaRetorno.ValorFrete.ValorTotalAReceber == 0 && cargaRetorno.ValorFrete.FreteProprio == 0))
                Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoDiversas(cargaRetorno.NumeroCarga, "error", 9999, "Sem valor de frete na integração da carga.", cargaRetorno.NumeroCarga, unitOfWork);

            if (stMensagem.Length > 0)
                return null;

            return cargaRetorno;
        }


        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas;
        }

        #endregion

    }
}
