using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.WebService.Devolucao
{
    public class Devolucao
    {
        #region Propriedades Privadas

        readonly private Repositorio.UnitOfWork _unitOfWork;
        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion Propriedades Privadas

        #region Construtores

        public Devolucao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Retorno<bool> AdicionarPendenciaFinanceira(Dominio.ObjetosDeValor.WebService.Devolucao.AdicionarPendenciaFinanceira pendenciaFinanceira)
        {
            try
            {
                Servicos.Log.TratarErro($"AdicionarPendenciaFinanceira: {Newtonsoft.Json.JsonConvert.SerializeObject(pendenciaFinanceira)}", "GestaoDevolucao");

                if (string.IsNullOrEmpty(pendenciaFinanceira.ChaveNFReferencia))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Chave NF Referência inválida ou não informada.");

                if (string.IsNullOrEmpty(pendenciaFinanceira.Motivo))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Motivo (MD) inválido ou não informado.");

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoImportacao repositorioGestaoDevolucaoImportacao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoImportacao(_unitOfWork);

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao gestaoDevolucaoImportacao = repositorioGestaoDevolucaoImportacao.BuscarPorNotaFiscalDevolucao(pendenciaFinanceira.ChaveNFReferencia);
                if (gestaoDevolucaoImportacao == null)
                    return Retorno<bool>.CriarRetornoSucesso(true, $"Gestão de Devolução não encontrada para a NFd {pendenciaFinanceira.ChaveNFReferencia}.");

                List<string> listaMotivoPendenciaFinanceira = new List<string>() { "180", "181", "131" };

                gestaoDevolucaoImportacao.Initialize();
                gestaoDevolucaoImportacao.Motivo = pendenciaFinanceira.Motivo;
                gestaoDevolucaoImportacao.ComPendenciaFinanceira = listaMotivoPendenciaFinanceira.Contains(gestaoDevolucaoImportacao.Motivo);

                repositorioGestaoDevolucaoImportacao.Atualizar(gestaoDevolucaoImportacao);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucaoImportacao, gestaoDevolucaoImportacao.GetChanges(), "Atualização via AdicionarPendenciaFinanceira", _unitOfWork);

                if (gestaoDevolucaoImportacao.GestaoDevolucao != null)
                {
                    Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(_unitOfWork);
                    Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = gestaoDevolucaoImportacao.GestaoDevolucao;
                    gestaoDevolucao.Initialize();
                    gestaoDevolucao.ComPendenciaFinanceira = gestaoDevolucaoImportacao.ComPendenciaFinanceira;
                    repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), "Atualização via AdicionarPendenciaFinanceira", _unitOfWork);
                }

                _unitOfWork.CommitChanges();
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "GestaoDevolucao");

                return Retorno<bool>.CriarRetornoDadosInvalidos("Ocorreu uma falha ao receber pendência financeira.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return Retorno<bool>.CriarRetornoSucesso(true, "Recebido com sucesso.");
        }

        public Retorno<bool> AtualizarOcorrenciaDevolucao(Dominio.ObjetosDeValor.WebService.Devolucao.AtualizarOcorrenciaDevolucao ocorrenciaDevolucao)
        {
            try
            {
                Servicos.Log.TratarErro($"AtualizarOcorrenciaDevolucao: {Newtonsoft.Json.JsonConvert.SerializeObject(ocorrenciaDevolucao)}", "GestaoDevolucao");

                if (string.IsNullOrEmpty(ocorrenciaDevolucao.NumeroOcorrencia))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Número da Ocorrência inválido ou não informado.");

                if (string.IsNullOrEmpty(ocorrenciaDevolucao.CodCliente))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Código do Cliente inválido ou não informado.");

                if (string.IsNullOrEmpty(ocorrenciaDevolucao.ChaveNFe))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Chave da Nota Fiscal inválida ou não informada.");

                if (string.IsNullOrEmpty(ocorrenciaDevolucao.ValorNFe))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Valor da Nota Fiscal inválida ou não informada.");

                if (ocorrenciaDevolucao.DataOcorrencia == DateTime.MinValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Data Ocorrência inválida ou não informada.");

                if (string.IsNullOrEmpty(ocorrenciaDevolucao.ChaveNFD))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Chave da Nota Fiscal Devolução inválida ou não informada.");

                if (string.IsNullOrEmpty(ocorrenciaDevolucao.ValorNFD))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Valor da Nota Fiscal Devolução inválida ou não informada.");

                // Criar objeto de integração

                _unitOfWork.Start();

                // Inserções e atualizações de objetos no banco de dados

                _unitOfWork.CommitChanges();
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "GestaoDevolucao");

                return Retorno<bool>.CriarRetornoDadosInvalidos("Ocorreu uma falha ao atualizar ocorrência devolução.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return Retorno<bool>.CriarRetornoSucesso(true, "Recebido com sucesso.");
        }

        public Retorno<bool> AtualizarLaudo(Dominio.ObjetosDeValor.WebService.Devolucao.AtualizarLaudo dadosLaudo)
        {
            try
            {
                Servicos.Log.TratarErro($"AtualizarLaudo: {Newtonsoft.Json.JsonConvert.SerializeObject(dadosLaudo)}", "GestaoDevolucao");

                // Criar objeto de integração

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo repositorioGestaoDevolucaoLaudo = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

                if (string.IsNullOrEmpty(dadosLaudo.CodFornecedor))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Necessário enviar código do fornecedor.");

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo laudoDevolucao = repositorioGestaoDevolucaoLaudo.BuscarPorCodigo(dadosLaudo.NumeroLaudo);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(dadosLaudo.CodFornecedor);

                if (laudoDevolucao == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Número do laudo inválido ou não encontrado.");

                if (empresa == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Código do fornecedor inválido ou não encontrado.");

                laudoDevolucao.Initialize();
                laudoDevolucao.DataCompensacao = dadosLaudo.DataCompensacao;
                laudoDevolucao.DataVencimento = dadosLaudo.DataVencimento;
                laudoDevolucao.Valor = dadosLaudo.Valor;
                laudoDevolucao.NumeroCompensacao = dadosLaudo.NumeroCompensacao;
                laudoDevolucao.Transportador = empresa;

                repositorioGestaoDevolucaoLaudo.Atualizar(laudoDevolucao);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, laudoDevolucao, laudoDevolucao.GetChanges(), "Atualização Laudo via WS", _unitOfWork);

                _unitOfWork.CommitChanges();
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "GestaoDevolucao");

                return Retorno<bool>.CriarRetornoDadosInvalidos("Ocorreu uma falha ao atualizar laudo.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return Retorno<bool>.CriarRetornoSucesso(true, "Recebido com sucesso.");
        }

        public Retorno<bool> AdicionarNotaDevolucao(Dominio.ObjetosDeValor.WebService.Devolucao.AdicionarNotaDevolucao notaDevolucao)
        {
            try
            {
                Servicos.Log.TratarErro($"AdicionarNotaDevolucao: {Newtonsoft.Json.JsonConvert.SerializeObject(notaDevolucao)}", "GestaoDevolucao");

                if (string.IsNullOrEmpty(notaDevolucao.ChaveNFeReferencia))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Chave NFe Referência inválida ou não informada.");

                if (string.IsNullOrEmpty(notaDevolucao.ChaveNFD))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Chave NFD inválida ou não informada.");

                if (string.IsNullOrEmpty(notaDevolucao.XmlNFD))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("XML da Nota Fiscal de Devolução inválido ou não informado.");

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoImportacao repositorioGestaoDevolucaoImportacao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoImportacao(_unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(_unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO repositorioGestaoDevolucaoNFDxNFO = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO(_unitOfWork);

                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(_unitOfWork, _auditado, _clienteMultisoftware);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorChave(notaDevolucao.ChaveNFeReferencia);

                double.TryParse(notaDevolucao.CpfCnpjCliente, out double cpfCnpjCliente);

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalDevolucao = null;
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe dadosNotaFiscalDevolucao = null;

                (xmlNotaFiscalDevolucao, dadosNotaFiscalDevolucao) = servicoGestaoDevolucao.ObterDadosNotaFiscalDevolucao(notaDevolucao.XmlNFD);

                IList<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoNotaFiscal> listaGestaoDevolucaoPorNota = repositorioGestaoDevolucao.BuscarPorChavesNotaOrigem(new List<string>() { notaDevolucao.ChaveNFeReferencia });
                if (listaGestaoDevolucaoPorNota != null && listaGestaoDevolucaoPorNota.Count > 0)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao> etapasLiberadas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao>() { Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao.DefinicaoTipoDevolucao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao.AprovacaoTipoDevolucao };
                    List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> listaGestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigos(listaGestaoDevolucaoPorNota.Select(n => n.CodigoGestaoDevolucao).ToList());
                    Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao repGestaoDevolucaoNotaFiscalDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao(_unitOfWork);

                    foreach (Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao in listaGestaoDevolucao)
                    {
                        if (!etapasLiberadas.Contains(gestaoDevolucao.EtapaAtual.Etapa)) continue;

                        if (!gestaoDevolucao.NotasFiscaisDevolucao.Any(nota => nota.XMLNotaFiscal.Chave == xmlNotaFiscalDevolucao.Chave))
                        {
                            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao gestaoDevolucaoNotaFiscalDevolucao = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao()
                            {
                                GestaoDevolucao = gestaoDevolucao,
                                XMLNotaFiscal = xmlNotaFiscalDevolucao
                            };

                            repGestaoDevolucaoNotaFiscalDevolucao.Inserir(gestaoDevolucaoNotaFiscalDevolucao);

                            gestaoDevolucao.Initialize();
                            gestaoDevolucao.NotasFiscaisDevolucao.Add(gestaoDevolucaoNotaFiscalDevolucao);
                            repositorioGestaoDevolucao.Atualizar(gestaoDevolucao);
                            Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), "Incluído Nota de Devolução via Webservice (AdicionarNotaDevolucao)", _unitOfWork);
                        }
                    }

                }
                else
                {
                    Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao gestaoDevolucaoImportacao = repositorioGestaoDevolucaoImportacao.BuscarPorNotaFiscalDevolucao(notaDevolucao.ChaveNFD);
                    if (gestaoDevolucaoImportacao != null)
                        return Retorno<bool>.CriarRetornoSucesso(true, "Registro ignorado pois já existe devolução para a Chave da Nota de Devolucao.");

                    gestaoDevolucaoImportacao = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao()
                    {
                        OrigemRecebimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemGestaoDevolucao.AdicionarNotaDevolucao,
                        Cliente = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjCliente) ?? repositorioCliente.BuscarPorCodigoIntegracao(notaDevolucao.CodCliente),
                        NotaFiscalDevolucao = null,
                        NotaFiscalOrigem = pedidoXMLNotaFiscal?.XMLNotaFiscal,
                        Carga = pedidoXMLNotaFiscal?.CargaPedido?.Carga,
                        Status = notaDevolucao.Status,
                        NumeroDocumento = notaDevolucao.DocVendas,
                        Fornecimento = notaDevolucao.Fornecimento,
                        DocumentoFaturamento = notaDevolucao.DocFaturamento,
                        DataDocumento = notaDevolucao.DataDocumento,
                        ChaveNFeReferencia = notaDevolucao.ChaveNFeReferencia,
                        ChaveNFD = notaDevolucao.ChaveNFD,
                        UsuarioModificouOrdem = notaDevolucao.UsuarioModificouOrdem,
                        XML_NFD = notaDevolucao.XmlNFD,
                        CFOP = notaDevolucao.CFOP,
                        EmDevolucao = false,
                    };
                    repositorioGestaoDevolucaoImportacao.Inserir(gestaoDevolucaoImportacao);
                }

                if (pedidoXMLNotaFiscal != null && xmlNotaFiscalDevolucao != null)
                {
                    repositorioGestaoDevolucaoNFDxNFO.Inserir(new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFDxNFO
                    {
                        NFD = xmlNotaFiscalDevolucao,
                        NFO = pedidoXMLNotaFiscal.XMLNotaFiscal
                    });
                }

                _unitOfWork.CommitChanges();
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "GestaoDevolucao");
                return Retorno<bool>.CriarRetornoDadosInvalidos("Ocorreu uma falha ao receber nota devolução: " + excecao.Message);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "GestaoDevolucao");

                return Retorno<bool>.CriarRetornoDadosInvalidos("Ocorreu uma falha ao receber nota devolução.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return Retorno<bool>.CriarRetornoSucesso(true, "Recebido com sucesso.");
        }

        public async Task<Retorno<bool>> FinalizarDevolucao(Dominio.ObjetosDeValor.WebService.Devolucao.FinalizarDevolucao devolucao)
        {
            try
            {
                Servicos.Log.TratarErro($"FinalizarDevolucao: {Newtonsoft.Json.JsonConvert.SerializeObject(devolucao)}", "GestaoDevolucao");

                if (string.IsNullOrWhiteSpace(devolucao.ChaveNFD))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Chave Nota Fiscal inválida ou não informada.");

                if (string.IsNullOrWhiteSpace(devolucao.CpfCnpjCliente))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("CPF / CNPJ do cliente inválido ou não informado.");

                if (string.IsNullOrWhiteSpace(devolucao.ChaveNFRefencia))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Chave Nota Fiscal Referência inválida ou não informada.");

                Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(_unitOfWork);
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = await repositorioGestaoDevolucao.BuscarPorChaveNotaDevolucaoAsync(devolucao.ChaveNFD);

                if (gestaoDevolucao == null)
                {
                    gestaoDevolucao = await repositorioGestaoDevolucao.BuscarPorChaveNotaOrigemAsync(devolucao.ChaveNFRefencia);

                    if (gestaoDevolucao == null)
                        return Retorno<bool>.CriarRetornoDadosInvalidos("A devolução não foi gerada para a nota informada.");

                }

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao repositorioGestaoDevolucaoNotaFiscalDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao(_unitOfWork);
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem repositorioGestaoDevolucaoNotaFiscalOrigem = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem(_unitOfWork);

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao gestaoDevolucaoNotaFiscalDevolucao = await repositorioGestaoDevolucaoNotaFiscalDevolucao.BuscarPorChaveNotaEDevolucaoAsync(devolucao.ChaveNFD, gestaoDevolucao.Codigo);
                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem gestaoDevolucaoGestaoDevolucaoNotaFiscalOrigem = await repositorioGestaoDevolucaoNotaFiscalOrigem.BuscarPorChaveNotaEDevolucaoAsync(devolucao.ChaveNFRefencia, gestaoDevolucao.Codigo);


                _unitOfWork.Start();

                if (gestaoDevolucaoNotaFiscalDevolucao != null)
                {
                    gestaoDevolucaoNotaFiscalDevolucao.ControleFinalizacaoDevolucao = true;
                    await repositorioGestaoDevolucaoNotaFiscalDevolucao.AtualizarAsync(gestaoDevolucaoNotaFiscalDevolucao);
                }


                if (gestaoDevolucaoGestaoDevolucaoNotaFiscalOrigem != null)
                {
                    gestaoDevolucaoGestaoDevolucaoNotaFiscalOrigem.ControleFinalizacaoDevolucao = true;
                    await repositorioGestaoDevolucaoNotaFiscalOrigem.AtualizarAsync(gestaoDevolucaoGestaoDevolucaoNotaFiscalOrigem);
                }

                if (await repositorioGestaoDevolucao.TodasNotasPalletFinalizadasAsync(gestaoDevolucao.Codigo))
                {

                    if (gestaoDevolucao.Laudo == null)
                        return Retorno<bool>.CriarRetornoDadosInvalidos("A devolução ainda não possui laudo gerado e aprovado, portanto, não possui quantidades para liberar o saldo de pallets.");

                    Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(_unitOfWork, _auditado);
                    servicoMovimentacaoPallet.InformarDevolucaoPallet(gestaoDevolucao);
                }

                if (await repositorioGestaoDevolucao.TodasNotasFinalizadasAsync(gestaoDevolucao.Codigo))
                {
                    gestaoDevolucao.DadosComplementares.ControleFinalizacaoDevolucao = true;
                    await repositorioGestaoDevolucao.AtualizarAsync(gestaoDevolucao);
                }

                _unitOfWork.CommitChanges();
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao, "GestaoDevolucao");

                if (string.IsNullOrEmpty(excecao.Message))
                    return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
                else
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Ocorreu uma falha ao finalizar devolução.");
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return Retorno<bool>.CriarRetornoSucesso(true, "Recebido com sucesso.");
        }

        #endregion Métodos Públicos

        #region Métodos Privados
        #endregion
    }
}
