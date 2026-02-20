using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Microsoft.AspNetCore.Http;
using CoreWCF;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Pessoas(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IPessoas
    {

        #region Métodos Globais

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> BuscarPessoas(int? inicio, int? limite, bool? consultarApenasAtualizados)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            return new Servicos.WebService.Pessoas.Pessoa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarPessoas(inicio ?? 0, limite ?? 0, consultarApenasAtualizados ?? false);
        }

        public Retorno<bool> ConfirmarIntegracaoPessoa(string cnpj)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                ValidarToken();

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                List<Dominio.Entidades.Cliente> clientes = repCliente.BuscarClientesPorCodigoIntegracao(cnpj);
                if (clientes != null && clientes.Count > 0)
                {
                    foreach (Dominio.Entidades.Cliente cli in clientes)
                    {
                        if (cli.Tipo == "E")
                        {
                            cli.Initialize();
                            cli.Integrado = true;
                            repCliente.Atualizar(cli, Auditado);
                        }
                    }
                    return Retorno<bool>.CriarRetornoSucesso(true);
                }

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpj.ToDouble());
                if (cliente != null)
                {
                    if (cliente.Tipo != "E")
                    {
                        cliente.Initialize();
                        cliente.Integrado = true;
                        repCliente.Atualizar(cliente, Auditado);
                    }
                }

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do cliente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> BuscarTransportadoresTerceiro(int? inicio, int? limite, bool? consultarApenasAtualizados)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;
            consultarApenasAtualizados ??= false;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>();
                    Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    List<Dominio.Entidades.Cliente> pessoas = null;

                    pessoas = repCliente.BuscarTransportadorTerceiroPorDataAtualizacao((bool)consultarApenasAtualizados ? DateTime.Now.AddDays(-1) : DateTime.MinValue, "CPF_CNPJ", "desc", (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repCliente.ContarTransportadorTerceiroBuscarPorDataAtualizacao((bool)consultarApenasAtualizados ? DateTime.Now.AddDays(-1) : DateTime.MinValue);

                    List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> listaRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>();

                    foreach (Dominio.Entidades.Cliente pessoa in pessoas)
                        listaRetorno.Add(serEmpresa.ConverterObjetoEmpresa(pessoa));

                    retorno.Objeto.Itens = listaRetorno;
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou transportadores terceiros", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os transportadores terceiros";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoTransportadoresTerceiro(string cnpj)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                ValidarToken();

                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repTransportador = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas terceiro = repTransportador.BuscarPorPessoa(cnpj.ToDouble());

                if (terceiro == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um transportador terceiro pelo CNPJ repassado.");

                terceiro.Initialize();
                terceiro.Integrado = true;
                repTransportador.Atualizar(terceiro, Auditado);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do transportador terceiro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Container>> BuscarContainer(int? inicio, int? limite, bool? consultarApenasAtualizados)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;
            consultarApenasAtualizados ??= false;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Container>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Container>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Container>();

                    Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
                    Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.Container> containeres = null;

                    containeres = repContainer.BuscarContainerDataAtualizacao((bool)consultarApenasAtualizados ? DateTime.Now.AddDays(-1) : DateTime.MinValue, "Codigo", "desc", (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repContainer.ContarContainerBuscarPorDataAtualizacao((bool)consultarApenasAtualizados ? DateTime.Now.AddDays(-1) : DateTime.MinValue);

                    List<Dominio.ObjetosDeValor.Embarcador.Carga.Container> listaRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Container>();

                    foreach (Dominio.Entidades.Embarcador.Pedidos.Container container in containeres)
                        listaRetorno.Add(serWSCarga.ConverterObjetoContainer(container));

                    retorno.Objeto.Itens = listaRetorno;
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou container", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os containeres";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoContainer(string codigoIntegracao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                ValidarToken();

                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Container container = repContainer.BuscarPorCodigo(codigoIntegracao.ToInt());

                if (container == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um container pelo código de integração.");

                container.Initialize();
                container.Integrado = true;
                repContainer.Atualizar(container, Auditado);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do container.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>> BuscarGrupoPessoas(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>();
                    Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.ConsultarTodas("Codigo", "desc", (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repGrupoPessoas.ContarConsultaTodas();
                    retorno.Objeto.Itens = serWSPessoa.RetornarGrupoPessoas(grupoPessoas, unitOfWork);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou grupo de pessoas", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os Grupos de Pessoas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.RamoAtividade>> BuscarRamosDeAtividade(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.RamoAtividade>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.RamoAtividade>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.RamoAtividade>();
                    Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                    List<Dominio.Entidades.Atividade> atividades = repAtividade.ConsultarTodas("Codigo", "desc", (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repAtividade.ContarConsultaTodas();
                    retorno.Objeto.Itens = serWSPessoa.RetornarRamosAtividade(atividades);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou ramos de atividade", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as Atividades";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Veiculos.Motorista>> BuscarMotoristas(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Veiculos.Motorista>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Veiculos.Motorista>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Veiculos.Motorista>();
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                    Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                    List<Dominio.Entidades.Usuario> motoristas = repUsuario.BuscarTodosMotoristasAtivos((int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repUsuario.ContarTodosMotoristasAtivos();
                    retorno.Objeto.Itens = serWSPessoa.RetornarMotoristas(motoristas);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou os motoristas", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os motoristas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo>> BuscarVeiculos(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo>();
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                    Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                    List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarTodosVeiculosAtivos((int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repVeiculo.ContarTodosVeiculosAtivos();
                    retorno.Objeto.Itens = serWSPessoa.RetornarVeiculos(veiculos);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou os Veiculos", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os Veiculos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> SalvarCliente(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa clienteIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Pessoas.Pessoa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SalvarCliente(clienteIntegracao, Auditado, TipoServicoMultisoftware));
            });
        }

        public Retorno<bool> SalvarClienteComplementar(Dominio.ObjetosDeValor.Embarcador.Pessoas.PessoaComplementar clienteComplementarIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Pessoas.Pessoa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SalvarClienteComplementar(clienteComplementarIntegracao));
            });
        }

        public Retorno<bool> SalvarContainer(Dominio.ObjetosDeValor.Embarcador.Carga.Container containerIntegracao)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Servicos.Log.TratarErro("SalvarContainer: " + Newtonsoft.Json.JsonConvert.SerializeObject(containerIntegracao));

                StringBuilder stMensagem = new StringBuilder();
                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Pedidos.Container retorno = new Servicos.WebService.Carga.Pedido(unitOfWork).SalvarContainer(containerIntegracao, ref stMensagem, Auditado);
                if (stMensagem.Length > 0)
                {
                    Servicos.Log.TratarErro("SalvarContainer: " + stMensagem.ToString());
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = stMensagem.ToString(), Objeto = false, Status = false };
                }
                else if (retorno == null)
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi possível salvar o container, verifique os campos obrigatorios.", Objeto = false, Status = false };
                }
                else if (retorno != null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, retorno, "Salvou Container", unitOfWork);

                    Servicos.Log.TratarErro("SalvarContainer: Sucesso: " + retorno.Codigo.ToString("D"));
                    unitOfWork.CommitChanges();
                    return new Retorno<bool>() { Mensagem = "", Objeto = true, Status = true };
                }
                else
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi possível salvar o container, verifique os campos obrigatorios.", Objeto = false, Status = false };
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarContainer: " + ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarProdutoEmbarcador(Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Servicos.ProdutoEmbarcador servicoProdutoEmbarcador = new Servicos.ProdutoEmbarcador(unitOfWork);
            try
            {
                Servicos.Log.TratarErro("SalvarProdutoEmbarcador: " + Newtonsoft.Json.JsonConvert.SerializeObject(produtoIntegracao));

                StringBuilder stMensagem = new StringBuilder();

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador retorno = servicoProdutoEmbarcador.IntegrarProduto(produtoIntegracao.CodigoProduto, produtoIntegracao.CodigocEAN, 0, produtoIntegracao.DescricaoProduto, produtoIntegracao.PesoUnitario, null, produtoIntegracao.MetroCubito, Auditado, produtoIntegracao.CodigoDocumentacao, produtoIntegracao.InativarCadastro, produtoIntegracao.Atualizar, produtoIntegracao.CodigoNCM, TipoServicoMultisoftware, produtoIntegracao.QuantidadeCaixa, produtoIntegracao.SiglaUnidade, produtoIntegracao.TemperaturaTransporte, produtoIntegracao.PesoLiquidoUnitario, produtoIntegracao.QtdPalet, produtoIntegracao.AlturaCM, produtoIntegracao.LarguraCM, produtoIntegracao.ComprimentoCM, produtoIntegracao.Observacao, produtoIntegracao.QuantidadeCaixaPorPallet);
                if (stMensagem.Length > 0)
                {
                    Servicos.Log.TratarErro("SalvarProdutoEmbarcador: " + stMensagem.ToString());
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = stMensagem.ToString(), Objeto = false, Status = false };
                }
                else if (retorno == null)
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi possível salvar o produto, verifique os campos obrigatorios.", Objeto = false, Status = false };
                }
                else if (retorno != null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, retorno, "Salvou Produto", unitOfWork);

                    Servicos.Log.TratarErro("SalvarProdutoEmbarcador: Sucesso: " + retorno.Codigo.ToString("D"));
                    unitOfWork.CommitChanges();
                    return new Retorno<bool>() { Mensagem = "", Objeto = true, Status = true };
                }
                else
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi possível salvar o produto, verifique os campos obrigatorios.", Objeto = false, Status = false };
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarProdutoEmbarcador: " + ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarNavio(Dominio.ObjetosDeValor.Embarcador.Carga.Navio navioIntegracao)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Servicos.Log.TratarErro("SalvarNavio: " + Newtonsoft.Json.JsonConvert.SerializeObject(navioIntegracao));

                StringBuilder stMensagem = new StringBuilder();

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Pedidos.Navio retorno = new Servicos.WebService.Carga.Pedido(unitOfWork).SalvarNavio(navioIntegracao, ref stMensagem, Auditado, true);
                if (stMensagem.Length > 0)
                {
                    Servicos.Log.TratarErro("SalvarNavio: " + stMensagem.ToString());
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = stMensagem.ToString(), Objeto = false, Status = false };
                }
                else if (retorno == null)
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi possível salvar o navio, verifique os campos obrigatorios.", Objeto = false, Status = false };
                }
                else if (retorno != null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, retorno, "Salvou Navio", unitOfWork);

                    Servicos.Log.TratarErro("SalvarNavio: Sucesso: " + retorno.Codigo.ToString("D"));
                    unitOfWork.CommitChanges();
                    return new Retorno<bool>() { Mensagem = "", Objeto = true, Status = true };
                }
                else
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi possível salvar o navio, verifique os campos obrigatorios.", Objeto = false, Status = false };
                }
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarNavio: " + ex);
                return new Retorno<bool>() { Mensagem = ex.Message, Status = false };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarNavio: " + ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarViagem(Dominio.ObjetosDeValor.Embarcador.Carga.Viagem viagemIntegracao)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Servicos.Log.TratarErro("SalvarViagem: " + Newtonsoft.Json.JsonConvert.SerializeObject(viagemIntegracao));

                StringBuilder stMensagem = new StringBuilder();
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio retorno = new Servicos.WebService.Carga.Pedido(unitOfWork).SalvarViagem(viagemIntegracao, ref stMensagem, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);
                if (stMensagem.Length > 0)
                {
                    Servicos.Log.TratarErro("SalvarViagem: " + stMensagem.ToString());
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = stMensagem.ToString(), Objeto = false, Status = false };
                }
                else if (retorno == null)
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi possível salvar a viagem, verifique os campos obrigatorios.", Objeto = false, Status = false };
                }
                else if (retorno != null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, retorno, "Salvou Viagem", unitOfWork);

                    Servicos.Log.TratarErro("SalvarViagem: Sucesso: " + retorno.Codigo.ToString("D"));

                    unitOfWork.CommitChanges();
                    return new Retorno<bool>() { Mensagem = "", Objeto = true, Status = true };
                }
                else
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi possível salvar a viagem, verifique os campos obrigatorios.", Objeto = false, Status = false };
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarViagem: " + ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarLocalidade(Dominio.ObjetosDeValor.Localidade localidade)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Servicos.Log.TratarErro("SalvarLocalidade: " + Newtonsoft.Json.JsonConvert.SerializeObject(localidade));

                if (localidade == null)
                {
                    return new Retorno<bool>() { Mensagem = "Formato do objeto localidade inválido, favor verifique os campos.", Status = false };
                }

                unitOfWork.Start();

                StringBuilder stMensagem = new StringBuilder();

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Localidade cidade = null;
                if (localidade.IBGE > 0)
                    cidade = repLocalidade.BuscarPorCodigoIBGE(localidade.IBGE);
                if (cidade == null)
                    cidade = repLocalidade.buscarPorCodigoEmbarcador(localidade.CodigoIntegracao);
                if (cidade == null)
                    cidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(localidade.Descricao), localidade.SiglaUF);

                int codigoLocalidade = 0;
                bool inserindo = false;
                if (cidade == null)
                {
                    cidade = new Dominio.Entidades.Localidade();
                    codigoLocalidade = repLocalidade.BuscarPorMaiorCodigo() + 1;
                    inserindo = true;
                }
                else
                {
                    codigoLocalidade = cidade.Codigo;
                    cidade.Initialize();
                }

                cidade.Descricao = localidade.Descricao;
                string codigoAnterior = cidade.CodigoDocumento;
                cidade.CodigoDocumento = localidade.CodigoDocumento;
                cidade.CodigoIBGE = localidade.IBGE;
                if (!string.IsNullOrWhiteSpace(localidade.RKST))
                    cidade.RKST = localidade?.RKST.Replace(" ","");
                string novoCodigo = cidade.CodigoDocumento;

                cidade.CodigoLocalidadeEmbarcador = localidade.CodigoIntegracao;
                if (localidade.Pais != null && !string.IsNullOrWhiteSpace(localidade.Pais.NomePais))
                    cidade.Pais = repPais.BuscarPorNome(localidade.Pais.NomePais);
                if (cidade.Pais == null && localidade.Pais != null && localidade.Pais.CodigoPais > 0)
                    cidade.Pais = repPais.BuscarPorCodigo(localidade.Pais.CodigoPais);
                if (cidade.Pais == null && localidade.Pais != null && !string.IsNullOrWhiteSpace(localidade.Pais.SiglaPais))
                    cidade.Pais = repPais.BuscarPorSiglaUF(localidade.Pais.SiglaPais);

                if (cidade.Codigo == 0)
                {
                    cidade.Codigo = codigoLocalidade;
                    cidade.Estado = repEstado.BuscarPorSigla(localidade.SiglaUF);
                }

                if (inserindo)
                    repLocalidade.Inserir(cidade, Auditado);
                else
                    repLocalidade.Atualizar(cidade, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cidade, "Salvou Municipio", unitOfWork);

                Servicos.Log.TratarErro("SalvarLocalidade: Sucesso: " + cidade.Codigo.ToString("D"));

                unitOfWork.CommitChanges();

                if (configuracaoTMS.UtilizaEmissaoMultimodal && !inserindo && codigoAnterior != novoCodigo && !string.IsNullOrWhiteSpace(novoCodigo))
                {
                    Servicos.Log.TratarErro($"Atualização de localidade e CAR_CARGA_INTEGRADA_EMBARCADOR para false pelo Pessoas.svc", "AtualizacaoCargaIntegradaEmbarcador");

                    bool atualizarTodos = string.IsNullOrWhiteSpace(codigoAnterior);
                    DbConnection connection = unitOfWork.GetConnection();
                    DbTransaction transaction = connection.BeginTransaction();
                    Servicos.Embarcador.Localidades.Localidade.VerificarCargasEmitidasAnteriormente(cidade.Codigo, atualizarTodos, connection, transaction);
                    transaction.Commit();
                }

                return new Retorno<bool>() { Mensagem = "", Objeto = true, Status = true };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarLocalidade: " + ex);
                return new Retorno<bool>() { Mensagem = "Favor verifique os campos obrigatórios como IBGE e UF.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarConfiguracaoFatura(Dominio.ObjetosDeValor.Embarcador.Carga.ConfiguracaoFatura configuracaoFatura)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Servicos.Log.TratarErro("SalvarConfiguracaoFatura: " + Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoFatura));

                StringBuilder stMensagem = new StringBuilder();

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = null;
                Dominio.Entidades.Cliente pessoa = null;
                if (!string.IsNullOrWhiteSpace(configuracaoFatura.CodigoIntegracaoGrupoPessoa))
                    grupoPessoas = repGrupoPessoas.BuscarPorCodigoIntegracao(configuracaoFatura.CodigoIntegracaoGrupoPessoa);
                else
                    if (!string.IsNullOrWhiteSpace(configuracaoFatura.CodigoIntegracaoPessoa))
                        pessoa = repCliente.BuscarPorCodigoIntegracao(configuracaoFatura.CodigoIntegracaoPessoa);

                if (pessoa == null && grupoPessoas == null)
                    return new Retorno<bool>() { Mensagem = "Não foi encontrado nenhuma Pessoa ou Grupo de Pessoa. Favor verifique os códigos de integração.", Objeto = false, Status = false };

                unitOfWork.Start();
                if (pessoa != null)
                    SalvarFaturamentoPessoa(configuracaoFatura, pessoa, unitOfWork, Auditado);
                else if (grupoPessoas != null)
                    SalvarFaturamentoGrupoPessoa(configuracaoFatura, grupoPessoas, unitOfWork, Auditado);
                else
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi encontrado nenhuma Pessoa ou Grupo de Pessoa. Favor verifique os códigos de integração.", Objeto = false, Status = false };
                }

                unitOfWork.CommitChanges();

                Servicos.Log.TratarErro("SalvarConfiguracaoFatura: Sucesso: " + (!string.IsNullOrWhiteSpace(configuracaoFatura.CodigoIntegracaoGrupoPessoa) ? configuracaoFatura.CodigoIntegracaoGrupoPessoa : configuracaoFatura.CodigoIntegracaoPessoa));

                return new Retorno<bool>() { Mensagem = "", Objeto = true, Status = true };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarConfiguracaoFatura: " + ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarAbastecimento(Dominio.ObjetosDeValor.Embarcador.Carga.Abastecimento abastecimento)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            string dataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            try
            {
                Servicos.Log.TratarErro("SalvarAbastecimento: " + Newtonsoft.Json.JsonConvert.SerializeObject(abastecimento));

                StringBuilder stMensagem = new StringBuilder();

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (configuracaoAbastecimento == null || configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto == null)
                {
                    return new Retorno<bool>() { Mensagem = "A empresa não possui configuração para movimentação contábil de abastecimento, favor entre em contato para realizar a configuração.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                }

                //if (string.IsNullOrWhiteSpace(abastecimento.PlacaVeiculo))
                //    return new Retorno<bool>() { Mensagem = "A placa do veículo é obrigatória.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                if (string.IsNullOrWhiteSpace(abastecimento.CNPJPosto))
                    return new Retorno<bool>() { Mensagem = "O CNPJ do Posto é obrigatório.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                if (string.IsNullOrWhiteSpace(abastecimento.CodigoProduto))
                    return new Retorno<bool>() { Mensagem = "O Código do Produto é obrigatório.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                //if (abastecimento.KM <= 0)
                //    return new Retorno<bool>() { Mensagem = "O KM do veículo é obrigatório.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                if (abastecimento.ValorUnitario <= 0)
                    return new Retorno<bool>() { Mensagem = "O Valor Unitário é obrigatório.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                if (abastecimento.Litros <= 0)
                    return new Retorno<bool>() { Mensagem = "A quantidade de Litros é obrigatório.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                if (string.IsNullOrWhiteSpace(abastecimento.Data))
                    return new Retorno<bool>() { Mensagem = "A Data do abastecimento é obrigatório.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };

                DateTime.TryParseExact(abastecimento.Data, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataAbastecimento);

                if (dataAbastecimento == DateTime.MinValue)
                    return new Retorno<bool>() { Mensagem = "A Data do abastecimento está no formato incorreto (dd/MM/yyyy HH:mm:ss).", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };

                string placaVeiculo = abastecimento.PlacaVeiculo.Replace("-", "");
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(placaVeiculo);

                double cnpjPosto = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(abastecimento.CNPJPosto), out cnpjPosto);
                Dominio.Entidades.Cliente posto = null;
                if (cnpjPosto > 0)
                    posto = repCliente.BuscarPorCPFCNPJ(cnpjPosto);

                Dominio.Entidades.Produto produto = repProduto.BuscarPorPostoTabelaDeValor(cnpjPosto, abastecimento.CodigoProduto);

                //if (veiculo == null)
                //{
                //    Servicos.Log.TratarErro("Veículo: " + placaVeiculo + " não cadastrado..");
                //    return new Retorno<bool>() { Mensagem = "Veículo " + placaVeiculo + " não cadastrado.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                //}

                bool jaExisteAbastecimento = repAbastecimento.VerificarSeJaExisteAbastecimentoImportacaoWS(abastecimento.NumeroDocumento, abastecimento.CodigoProduto, posto?.CPF_CNPJ ?? 0, veiculo?.Codigo ?? 0, dataAbastecimento);
                if (jaExisteAbastecimento)
                {
                    Servicos.Log.TratarErro("AbastecimentoDuplicado: Falha: " + abastecimento.NumeroDocumento);
                    return new Retorno<bool>() { Mensagem = "Abastecimento " + abastecimento.NumeroDocumento + " já existe. (duplicado)", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao };
                }

                if (produto != null && repAbastecimento.AbastecimentoDuplicado(dataAbastecimento, abastecimento.NumeroDocumento, posto?.CPF_CNPJ ?? 0d, produto.Codigo, abastecimento.Litros, abastecimento.ValorUnitario))
                {
                    Servicos.Log.TratarErro("AbastecimentoDuplicado: Falha: " + abastecimento.NumeroDocumento);
                    return new Retorno<bool>() { Mensagem = "Abastecimento " + abastecimento.NumeroDocumento + " já existe. (duplicado)", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao };
                }

                Dominio.Entidades.Abastecimento abs = new Dominio.Entidades.Abastecimento();

                int km = abastecimento.KM;
                decimal quantidade = abastecimento.Litros, valorUnitario = abastecimento.ValorUnitario;
                string cpfMotorista = "";
                if (!string.IsNullOrWhiteSpace(abastecimento.CPFMotorista))
                    cpfMotorista = Utilidades.String.OnlyNumbers(abastecimento.CPFMotorista);

                unitOfWork.Start();

                abs.Data = dataAbastecimento;
                abs.Motorista = !string.IsNullOrWhiteSpace(cpfMotorista) ? repUsuario.BuscarMotoristaPorCPF(cpfMotorista) : null;
                abs.Kilometragem = km;
                abs.Litros = quantidade;
                abs.NomePosto = posto?.Nome ?? "";
                abs.Pago = false;
                abs.Situacao = "A";
                abs.DataAlteracao = DateTime.Now;
                abs.Status = "A";
                abs.ValorUnitario = valorUnitario;
                abs.Veiculo = veiculo;
                abs.Posto = posto;
                abs.Produto = produto;
                abs.TipoMovimento = configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto;
                abs.Documento = abastecimento.NumeroDocumento;
                abs.TipoRecebimentoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoAbastecimento.WSPosto;

                Dominio.Entidades.Usuario veiculoMotorista = null;
                if (veiculo != null)
                    veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                if (abs.Motorista == null && abs.Veiculo != null && veiculoMotorista != null)
                    abs.Motorista = veiculoMotorista;

                Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abs, abs.Veiculo, abs.Equipamento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                if (abs.Produto != null)
                {
                    if (abs.Produto.CodigoNCM.StartsWith("310210"))
                        tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;
                    else
                        tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                }

                abs.TipoAbastecimento = tipoAbastecimento;
                Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abs, unitOfWork, veiculo, null, configuracaoTMS);

                if (abs.Situacao == "I" && abs.MotivoInconsistencia.Contains("duplicado"))
                {
                    Servicos.Log.TratarErro("AbastecimentoDuplicado: Falha: " + abastecimento.NumeroDocumento);
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Abastecimento " + abastecimento.NumeroDocumento + " já existe. (duplicado para o mesmo veículo)", Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao };
                }
                else if (abs.Posto != null && abs.Produto != null)
                {
                    repAbastecimento.Inserir(abs);
                }
                else if (abs.Posto == null && abs.Veiculo != null)
                {
                    Servicos.Log.TratarErro("Posto de CNPJ: " + abastecimento.CNPJPosto + " não cadastrado");
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "O posto (" + abastecimento.CNPJPosto + ") não estão cadastrados, favor entre em contato para realizar a configuração.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                }
                else if (abs.Produto == null && abs.Veiculo != null && abs.Posto != null)
                {
                    Servicos.Log.TratarErro("Posto: " + abastecimento.CNPJPosto + " Código de Integração: " + abastecimento.CodigoProduto + " não cadastrado.");
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "O posto (" + abastecimento.CNPJPosto + ") com o Produto (" + abastecimento.CodigoProduto + ") não estão cadastrados, favor entre em contato para realizar a configuração.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                }
                else
                {
                    Servicos.Log.TratarErro("Abastecimento sem Veículo: " + abastecimento.PlacaVeiculo + " sem Posto " + abastecimento.CNPJPosto + " e sem Produto.");
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "O veículo (" + abastecimento.PlacaVeiculo + "), o posto (" + abastecimento.CNPJPosto + ") e o Produto (" + abastecimento.CodigoProduto + ") não estão cadastrados, favor entre em contato para realizar a configuração.", Objeto = false, Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos };
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, abs, "Salvou Abastecimento por Integração", unitOfWork);
                Servicos.Log.TratarErro("SalvarAbastecimento: Sucesso: " + abs.Codigo.ToString("D"));
                unitOfWork.CommitChanges();

                return new Retorno<bool>() { Mensagem = "", Objeto = true, Status = true, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso };

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarAbastecimento: " + ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false, DataRetorno = dataRetorno, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarGrupoPessoa(Dominio.ObjetosDeValor.Embarcador.Carga.GrupoPessoa grupoPessoaIntegracao)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Servicos.Log.TratarErro("SalvarGrupoPessoa: " + Newtonsoft.Json.JsonConvert.SerializeObject(grupoPessoaIntegracao));

                StringBuilder stMensagem = new StringBuilder();

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas retorno = new Servicos.WebService.Carga.Pedido(unitOfWork).SalvarGrupoPessoa(grupoPessoaIntegracao, ref stMensagem, Auditado);
                if (stMensagem.Length > 0)
                {
                    Servicos.Log.TratarErro("SalvarGrupoPessoa: " + stMensagem.ToString());
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = stMensagem.ToString(), Objeto = false, Status = false };
                }
                else if (retorno == null)
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi possível salvar o grupo de pessoa, verifique os campos obrigatorios.", Objeto = false, Status = false };
                }
                else if (retorno != null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, retorno, "Salvou Grupo de Pessoa", unitOfWork);

                    Servicos.Log.TratarErro("SalvarGrupoPessoa: Sucesso: " + retorno.Codigo.ToString("D"));
                    unitOfWork.CommitChanges();
                    return new Retorno<bool>() { Mensagem = "", Objeto = true, Status = true };
                }
                else
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "Não foi possível salvar o grupo de pessoa, verifique os campos obrigatorios.", Objeto = false, Status = false };
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarGrupoPessoa: " + ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<int> BuscarProtocoloMotorista(string ddd, string telefone)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Servicos.Log.TratarErro("BuscarProtocoloMotorista: DDD" + ddd + " Telefone " + telefone);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                string telefoneSemFormado = Utilidades.String.OnlyNumbers(ddd) + Utilidades.String.OnlyNumbers(telefone);
                string telefoneFormatado = telefoneSemFormado.ObterTelefoneFormatado();

                Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorCelular(telefoneSemFormado, telefoneFormatado);

                if (motorista == null)
                {
                    return new Retorno<int>() { Mensagem = "Não foi encontrado nenhum motorista ativo com o DDD e telefone celular informado.", Objeto = 0, Status = false };
                }
                else
                {
                    return new Retorno<int>() { Mensagem = "Motorista localizado", Objeto = motorista.Codigo, Status = true };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("BuscarProtocoloMotorista: " + ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha genérica ao realizar consulta.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Protocolo>> BuscarCargaMotorista(int protocoloMotorista)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Protocolo>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Protocolo>>();
            retorno.Mensagem = "";

            try
            {
                Servicos.Log.TratarErro("BuscarCargaMotorista: " + protocoloMotorista);

                retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Protocolo>();
                retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Protocolo>();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarCargasEmTransportePorMotorista(protocoloMotorista);

                if (cargas != null && cargas.Count > 0)
                {

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Protocolo objCarregamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Protocolo();

                        objCarregamento.protocoloTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;
                        objCarregamento.tipoOperacao = carga.TipoOperacao?.Descricao ?? "";
                        objCarregamento.codigoIntegracaoTipoOperacao = carga.TipoOperacao?.CodigoIntegracao ?? "";
                        objCarregamento.protocoloIntegracaoCarga = carga.Protocolo;
                        objCarregamento.Veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Veiculo>();

                        if (carga.Veiculo != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Veiculo veiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Veiculo()
                            {
                                MarcaVeiculo = carga.Veiculo.Marca?.Descricao ?? "",
                                ModeloVeicular = carga.Veiculo.Modelo?.Descricao ?? "",
                                ModeloVeiculo = carga.Veiculo.ModeloVeicularCarga?.Descricao ?? "",
                                NumeroFrota = carga.Veiculo.NumeroFrota,
                                Placa = carga.Veiculo.Placa,
                                TipoVeiculo = carga.Veiculo.DescricaoTipoVeiculo
                            };
                            objCarregamento.Veiculos.Add(veiculo);
                        }
                        if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                        {
                            foreach (Dominio.Entidades.Veiculo vinculado in carga.VeiculosVinculados)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Veiculo veiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.Veiculo()
                                {
                                    MarcaVeiculo = vinculado.Marca?.Descricao ?? "",
                                    ModeloVeicular = vinculado.Modelo?.Descricao ?? "",
                                    ModeloVeiculo = vinculado.ModeloVeicularCarga?.Descricao ?? "",
                                    NumeroFrota = vinculado.NumeroFrota,
                                    Placa = vinculado.Placa,
                                    TipoVeiculo = vinculado.DescricaoTipoVeiculo
                                };
                                objCarregamento.Veiculos.Add(veiculo);
                            }
                        }

                        retorno.Objeto.Itens.Add(objCarregamento);
                    }

                    retorno.Objeto.NumeroTotalDeRegistro = cargas.Count;
                    retorno.Status = true;
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.RegistroIndisponivel;
                    retorno.Status = false;
                    retorno.Mensagem = "Não foi localizada nenhuma carga em transporte para o motorista.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("BuscarCargaMotorista: " + ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as cargas associadas ao motorista";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> SalvarUsuario(Dominio.ObjetosDeValor.WebService.Usuario.UsuarioIntegracao usuario)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Servicos.Log.TratarErro("SalvarUsuario: " + Newtonsoft.Json.JsonConvert.SerializeObject(usuario));

                StringBuilder stMensagem = new StringBuilder();

                AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(Conexao.createInstance(_serviceProvider).ObterHost);
                AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = clienteURLAcesso?.Cliente;

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.PerfilPermissao repPerfilMultiCTe = new Repositorio.PerfilPermissao(unitOfWork);
                Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilEmbarcador = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfilAcesso = new Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso();

                if (usuario == null)
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("SalvarUsuario: Usuario invalido.", "Usuarios");
                    return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos, Mensagem = "Usuario invalido.", Objeto = false, Status = false };
                }

                if (usuario.RedefinirSenha)
                {
                    if (string.IsNullOrEmpty(usuario.CodigoIntegracao))
                    {
                        unitOfWorkAdmin.Dispose();
                        return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos, Mensagem = "Para Redefinir senha, codigo integração é obrigatório.", Objeto = false, Status = false };
                    }

                    Dominio.Entidades.Usuario usuarioRedefinir = repUsuario.BuscarPorCodigoIntegracao(usuario.CodigoIntegracao);
                    if (usuarioRedefinir == null)
                    {
                        unitOfWorkAdmin.Dispose();
                        return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.RegistroIndisponivel, Mensagem = "Usuário com codigo Integração " + usuario.CodigoIntegracao + " não encontrado. ", Objeto = false, Status = false };
                    }

                    usuarioRedefinir.DataUltimaAlteracaoSenhaObrigatoria = null;
                    usuarioRedefinir.AlterarSenhaAcesso = true;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, usuarioRedefinir, null, "Redefiniu a senha.", unitOfWork);

                    repUsuario.Atualizar(usuarioRedefinir);
                    unitOfWork.CommitChanges();
                    unitOfWorkAdmin.Dispose();

                    return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso, Mensagem = "Redefinir senha ativado com sucesso.", Objeto = true, Status = false };
                }

                if (string.IsNullOrWhiteSpace(usuario.Sistema))
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("SalvarUsuario: Sistema(" + usuario.Sistema + ") invalido.", "Usuarios");
                    return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos, Mensagem = "Sistema (" + usuario.Sistema + ") invalido.", Objeto = false, Status = false };
                }

                perfilAcesso = repPerfilEmbarcador.BuscarPorCodigoIntegracao(usuario.CodigoIntegracaoPerfilAcesso);
                if (configuracao.ExigePerfilUsuario)
                {
                    if (string.IsNullOrWhiteSpace(usuario.CodigoIntegracaoPerfilAcesso))
                    {
                        unitOfWorkAdmin.Dispose();
                        Servicos.Log.TratarErro("SalvarUsuario: Perfil com codigo Integracao invalido.", "Usuarios");
                        return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos, Mensagem = "Perfil com codigo Integracao invalido.", Objeto = false, Status = false };
                    }

                    if (perfilAcesso == null)
                    {
                        unitOfWorkAdmin.Dispose();
                        Servicos.Log.TratarErro("SalvarUsuario: Perfil com codigo Integracao " + usuario.CodigoIntegracaoPerfilAcesso + " não encontrado.", "Usuarios");
                        return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos, Mensagem = "Perfil com codigo Integracao invalido.", Objeto = false, Status = false };
                    }
                }

                if (string.IsNullOrWhiteSpace(usuario.Login))
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("SalvarUsuario: Login (" + usuario.Login + ") invalido.", "Usuarios");
                    return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos, Mensagem = "Login (" + usuario.Login + ") invalido.", Objeto = false, Status = false };
                }

                if (string.IsNullOrWhiteSpace(usuario.CPF_CNPJ) || !Utilidades.Validate.ValidarCPF(Utilidades.String.OnlyNumbers(usuario.CPF_CNPJ)))
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("SalvarUsuario: CPF (" + usuario.CPF_CNPJ + ") invalido.", "Usuarios");
                    return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos, Mensagem = "CPF (" + usuario.CPF_CNPJ + ") invalido.", Objeto = false, Status = false };
                }

                Dominio.Entidades.Usuario usuarioLogin = repUsuario.BuscarPorLoginETipo(usuario.Login, Dominio.Enumeradores.TipoAcesso.Embarcador);
                if (usuarioLogin != null && usuarioLogin.CPF != Utilidades.String.OnlyNumbers(usuario.CPF_CNPJ))
                {
                    unitOfWorkAdmin.Dispose();
                    Servicos.Log.TratarErro("SalvarUsuario: Login está vinculado a outro CPF.", "Usuarios");
                    return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos, Mensagem = "Login está vinculado a outro CPF.", Objeto = false, Status = false };
                }

                Servicos.WebService.Usuarios.Usuario serWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);

                try
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Usuario usuarioSistema = serWSUsuario.SalvarEAtualizarUsuarioIntegracao(usuario, perfilAcesso, Auditado, cliente, unitOfWork, unitOfWorkAdmin);

                    if (usuarioSistema != null)
                    {
                        unitOfWork.CommitChanges();
                        unitOfWorkAdmin.Dispose();

                        return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso, Mensagem = "Usuario criado com sucesso", Objeto = true, Status = true, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        unitOfWorkAdmin.Dispose();

                        Servicos.Log.TratarErro("SalvarUsuario: Não foi possível salvar usuario", "Usuarios");
                        return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica, Mensagem = "Não foi possível salvar usuario", Objeto = false, Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro("SalvarUsuario: " + ex, "Usuarios");
                    return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica, Mensagem = "Ocorreu uma falha generica ao realizar a integracao.", Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };
                }
                finally
                {
                    unitOfWork.Dispose();
                    unitOfWorkAdmin.Dispose();
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarUsuario: " + ex, "Usuarios");
                return new Retorno<bool>() { CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica, Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Usuario.UsuarioRetorno>> ObterUsuarios(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Usuario.UsuarioRetorno>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Usuario.UsuarioRetorno>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                IList<Dominio.Entidades.Usuario> usuarios = null;

                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.Usuario.UsuarioRetorno>();
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                    usuarios = repUsuario.BuscarTodosUsuarios(Dominio.Enumeradores.TipoAcesso.Embarcador, (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repUsuario.ContarTodosUsuarios(Dominio.Enumeradores.TipoAcesso.Embarcador);

                    List<Dominio.ObjetosDeValor.WebService.Usuario.UsuarioRetorno> listaRetorno = new List<Dominio.ObjetosDeValor.WebService.Usuario.UsuarioRetorno>();

                    foreach (Dominio.Entidades.Usuario user in usuarios)
                    {
                        Dominio.ObjetosDeValor.WebService.Usuario.UsuarioRetorno userRetorno = new Dominio.ObjetosDeValor.WebService.Usuario.UsuarioRetorno()
                        {
                            CodigoIntegracao = user.CodigoIntegracao,
                            CPF_CNPJ = user.CPF_CNPJ_Formatado,
                            Email = user.Email,
                            Login = user.Login,
                            Nome = user.Nome,
                            Situacao = user.DescricaoStatus
                        };

                        listaRetorno.Add(userRetorno);
                    }

                    retorno.Objeto.Itens = listaRetorno;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Usuarios", unitOfWork);

                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }

            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Usuarios");
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os Usuarios";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Usuario.DetalheUsuario> ObterDadosUsuario(string codigoIntegracao)
        {
            ValidarToken();


            Retorno<Dominio.ObjetosDeValor.WebService.Usuario.DetalheUsuario> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Usuario.DetalheUsuario>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.WebService.Usuarios.Usuario serWSUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);

            try
            {
                Dominio.Entidades.Usuario usuario = null;
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                usuario = repUsuario.BuscarPorCodigoIntegracao(codigoIntegracao);

                if (usuario != null)
                {
                    retorno.Objeto = serWSUsuario.converterObjetoDetalhesUsuario(usuario, unitOfWork);
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Usuarios", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.RegistroIndisponivel;
                    retorno.Mensagem = "Não foi encontrado usuário com codigoIntegracao: " + codigoIntegracao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Usuarios");
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao ObterDadosUsuario";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> BuscarClientesPendentesIntegracao(int? quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>>.CreateFrom(new Servicos.WebService.Pessoas.Pessoa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarClientesPendentesIntegracao(quantidade ?? 0));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> ConfirmarIntegracaoPessoaERP(List<long> listaProtocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Pessoas.Pessoa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoPessoa(listaProtocolos));
            });
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePessoas;
        }

        #endregion

        #region Métodos Privados

        private void SalvarFaturamentoPessoa(Dominio.ObjetosDeValor.Embarcador.Carga.ConfiguracaoFatura configuracaoFatura, Dominio.Entidades.Cliente entidade, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordo = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEmail repClienteOutroEmail = new Repositorio.Embarcador.Pessoas.ClienteOutroEmail(unitOfWork);

            entidade.Initialize();
            entidade.DiasSemanaFatura = configuracaoFatura.DiasSemanaFatura;
            entidade.DiasMesFatura = configuracaoFatura.DiasMesFatura;
            if (configuracaoFatura.TipoAgrupamentoFatura.HasValue)
                entidade.TipoAgrupamentoFatura = configuracaoFatura.TipoAgrupamentoFatura.Value;
            if (configuracaoFatura.TipoEnvioFatura.HasValue)
                entidade.TipoEnvioFatura = configuracaoFatura.TipoEnvioFatura.Value;
            if (configuracaoFatura.TipoPrazoFaturamento.HasValue)
                entidade.TipoPrazoFaturamento = configuracaoFatura.TipoPrazoFaturamento.Value;
            entidade.DiasDePrazoFatura = configuracaoFatura.DiasDePrazoFatura;
            entidade.GerarFaturamentoAVista = configuracaoFatura.GerarFaturamentoAVista;
            entidade.GerarTituloAutomaticamente = configuracaoFatura.GerarTituloAutomaticamente;
            entidade.GerarTituloPorDocumentoFiscal = configuracaoFatura.GerarTituloPorDocumentoFiscal;
            entidade.PermiteFinalDeSemana = configuracaoFatura.PermiteFinalDeSemana;
            entidade.ExigeCanhotoFisico = configuracaoFatura.ExigeCanhotoFisico;
            entidade.ArmazenaCanhotoFisicoCTe = configuracaoFatura.ArmazenaCanhotoFisicoCTe;
            entidade.SomenteOcorrenciasFinalizadoras = configuracaoFatura.AgregadoSomenteOcorrenciasFinalizadoras;
            entidade.FaturarSomenteOcorrenciasFinalizadoras = configuracaoFatura.FaturarSomenteOcorrenciasFinalizadoras;
            entidade.GerarBoletoAutomaticamente = configuracaoFatura.GerarBoletoAutomaticamente;
            entidade.EnviarArquivosDescompactados = configuracaoFatura.EnviarArquivosDescompactados;
            entidade.Banco = configuracaoFatura.NumeroBanco > 0 ? repBanco.BuscarPorNumero(configuracaoFatura.NumeroBanco) : null;
            entidade.Agencia = configuracaoFatura.NumeroAgencia;
            entidade.DigitoAgencia = configuracaoFatura.DigitoBanco;
            entidade.NumeroConta = configuracaoFatura.NumeroConta;
            entidade.TipoContaBanco = configuracaoFatura.TipoContaBanco;

            double cnpjTomadorFatura = 0;
            if (!string.IsNullOrWhiteSpace(configuracaoFatura.CNPJTomadorFatura))
                double.TryParse(Utilidades.String.OnlyNumbers(configuracaoFatura.CNPJTomadorFatura), out cnpjTomadorFatura);

            entidade.ClienteTomadorFatura = cnpjTomadorFatura > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjTomadorFatura) : null;
            entidade.ObservacaoFatura = configuracaoFatura.ObservacaoFatura;
            if (configuracaoFatura.AtualizarAssuntoEmailFatura)
                entidade.AssuntoEmailFatura = configuracaoFatura.AssuntoEmailFatura;
            if (configuracaoFatura.AtualizarCorpoEmailFatura)
                entidade.CorpoEmailFatura = configuracaoFatura.CorpoEmailFatura;
            entidade.ValorMaximoEmissaoPendentePagamento = configuracaoFatura.ValorMaximoEmissaoPendentePagamento;

            if (configuracaoFatura.AtualizarEmailFatura && !string.IsNullOrWhiteSpace(configuracaoFatura.EmailFatura))
            {
                string[] listaEmails = configuracaoFatura.EmailFatura.Split(';');
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail> emailsCliente = repClienteOutroEmail.BuscarPorCNPJCPFCliente(entidade.CPF_CNPJ);

                if (listaEmails.Length > 0)
                {
                    foreach (string email in listaEmails)
                    {
                        if (!repClienteOutroEmail.ContemEmailCliente(entidade.CPF_CNPJ, email))
                        {
                            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail emailPessoa = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail
                            {
                                Email = email,
                                EmailStatus = "A",
                                TipoEmail = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Administrativo,
                                Cliente = entidade
                            };
                            repClienteOutroEmail.Inserir(emailPessoa);
                        }
                    }

                }
            }
            entidade.DataUltimaAtualizacao = DateTime.Now;
            entidade.Integrado = false;
            repCliente.Atualizar(entidade);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, entidade, null, "Salvou as configurações de faturamento pelo acordo de faturamento", unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoCliente = repAcordo.BuscarAcordoCliente(entidade.CPF_CNPJ, 0);
            if (acordoCliente == null)
                acordoCliente = new Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente();
            else
                acordoCliente.Initialize();

            acordoCliente.Status = configuracaoFatura.InativarCadastro ? false : true;
            acordoCliente.Pessoa = entidade;
            acordoCliente.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa;
            acordoCliente.CabotagemGerarFaturamentoAVista = configuracaoFatura.CabotagemGerarFaturamentoAVista;
            acordoCliente.CabotagemDiasDePrazoFatura = configuracaoFatura.CabotagemDiasDePrazoFatura;
            acordoCliente.CabotagemTipoPrazoFaturamento = configuracaoFatura.CabotagemTipoPrazoFaturamento;
            acordoCliente.CabotagemDiasSemanaFatura = configuracaoFatura.CabotagemDiasSemanaFatura;
            acordoCliente.CabotagemDiasMesFatura = configuracaoFatura.CabotagemDiasMesFatura;
            if (configuracaoFatura.AtualizarCabotagemEmail)
                acordoCliente.CabotagemEmail = configuracaoFatura.CabotagemEmail;

            acordoCliente.LongoCursoGerarFaturamentoAVista = configuracaoFatura.LongoCursoGerarFaturamentoAVista;
            acordoCliente.LongoCursoDiasDePrazoFatura = configuracaoFatura.LongoCursoDiasDePrazoFatura;
            acordoCliente.LongoCursoTipoPrazoFaturamento = configuracaoFatura.LongoCursoTipoPrazoFaturamento;
            acordoCliente.LongoCursoDiasSemanaFatura = configuracaoFatura.LongoCursoDiasSemanaFatura;
            acordoCliente.LongoCursoDiasMesFatura = configuracaoFatura.LongoCursoDiasMesFatura;
            if (configuracaoFatura.AtualizarLongoCursoEmail)
                acordoCliente.LongoCursoEmail = configuracaoFatura.LongoCursoEmail;

            acordoCliente.CustoExtraGerarFaturamentoAVista = configuracaoFatura.CustoExtraGerarFaturamentoAVista;
            acordoCliente.CustoExtraDiasDePrazoFatura = configuracaoFatura.CustoExtraDiasDePrazoFatura;
            acordoCliente.CustoExtraTipoPrazoFaturamento = configuracaoFatura.CustoExtraTipoPrazoFaturamento;
            acordoCliente.CustoExtraDiasSemanaFatura = configuracaoFatura.CustoExtraDiasSemanaFatura;
            acordoCliente.CustoExtraDiasMesFatura = configuracaoFatura.CustoExtraDiasMesFatura;
            if (configuracaoFatura.AtualizarCustoExtraEmail)
                acordoCliente.CustoExtraEmail = configuracaoFatura.CustoExtraEmail;

            if (acordoCliente.Codigo > 0)
                repAcordo.Atualizar(acordoCliente);
            else
                repAcordo.Inserir(acordoCliente);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoCliente, null, "Salvou acordo de faturamento", unitOfWork);
        }

        private void SalvarFaturamentoGrupoPessoa(Dominio.ObjetosDeValor.Embarcador.Carga.ConfiguracaoFatura configuracaoFatura, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas entidade, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordo = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            entidade.Initialize();
            entidade.DiasSemanaFatura = configuracaoFatura.DiasSemanaFatura;
            entidade.DiasMesFatura = configuracaoFatura.DiasMesFatura;
            if (configuracaoFatura.TipoAgrupamentoFatura.HasValue)
                entidade.TipoAgrupamentoFatura = configuracaoFatura.TipoAgrupamentoFatura.Value;
            if (configuracaoFatura.TipoEnvioFatura.HasValue)
                entidade.TipoEnvioFatura = configuracaoFatura.TipoEnvioFatura.Value;
            if (configuracaoFatura.TipoPrazoFaturamento.HasValue)
                entidade.TipoPrazoFaturamento = configuracaoFatura.TipoPrazoFaturamento.Value;
            entidade.DiasDePrazoFatura = configuracaoFatura.DiasDePrazoFatura;
            entidade.GerarFaturamentoAVista = configuracaoFatura.GerarFaturamentoAVista;
            entidade.GerarTituloAutomaticamente = configuracaoFatura.GerarTituloAutomaticamente;
            entidade.GerarTituloPorDocumentoFiscal = configuracaoFatura.GerarTituloPorDocumentoFiscal;
            entidade.PermiteFinalDeSemana = configuracaoFatura.PermiteFinalDeSemana;
            entidade.ExigeCanhotoFisico = configuracaoFatura.ExigeCanhotoFisico;
            entidade.ArmazenaCanhotoFisicoCTe = configuracaoFatura.ArmazenaCanhotoFisicoCTe;
            entidade.SomenteOcorrenciasFinalizadoras = configuracaoFatura.AgregadoSomenteOcorrenciasFinalizadoras;
            entidade.FaturarSomenteOcorrenciasFinalizadoras = configuracaoFatura.FaturarSomenteOcorrenciasFinalizadoras;
            entidade.GerarBoletoAutomaticamente = configuracaoFatura.GerarBoletoAutomaticamente;
            entidade.EnviarArquivosDescompactados = configuracaoFatura.EnviarArquivosDescompactados;
            entidade.Banco = configuracaoFatura.NumeroBanco > 0 ? repBanco.BuscarPorNumero(configuracaoFatura.NumeroBanco) : null;
            entidade.Agencia = configuracaoFatura.NumeroAgencia;
            entidade.DigitoAgencia = configuracaoFatura.DigitoBanco;
            entidade.NumeroConta = configuracaoFatura.NumeroConta;
            entidade.TipoContaBanco = configuracaoFatura.TipoContaBanco;

            double cnpjTomadorFatura = 0;
            if (!string.IsNullOrWhiteSpace(configuracaoFatura.CNPJTomadorFatura))
                double.TryParse(Utilidades.String.OnlyNumbers(configuracaoFatura.CNPJTomadorFatura), out cnpjTomadorFatura);

            entidade.ClienteTomadorFatura = cnpjTomadorFatura > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjTomadorFatura) : null;
            entidade.ObservacaoFatura = configuracaoFatura.ObservacaoFatura;
            if (configuracaoFatura.AtualizarAssuntoEmailFatura)
                entidade.AssuntoEmailFatura = configuracaoFatura.AssuntoEmailFatura;
            if (configuracaoFatura.AtualizarCorpoEmailFatura)
                entidade.CorpoEmailFatura = configuracaoFatura.CorpoEmailFatura;
            if (configuracaoFatura.AtualizarEmailFatura)
                entidade.Email = configuracaoFatura.EmailFatura;
            entidade.ValorMaximoEmissaoPendentePagamento = configuracaoFatura.ValorMaximoEmissaoPendentePagamento;

            repGrupoPessoas.Atualizar(entidade);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, entidade, null, "Salvou as configurações de faturamento pelo acordo de faturamento", unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoCliente = repAcordo.BuscarAcordoCliente(0, entidade.Codigo);
            if (acordoCliente == null)
                acordoCliente = new Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente();
            else
                acordoCliente.Initialize();

            acordoCliente.GrupoPessoas = entidade;
            acordoCliente.Status = configuracaoFatura.InativarCadastro ? false : true;
            acordoCliente.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa;
            acordoCliente.CabotagemGerarFaturamentoAVista = configuracaoFatura.CabotagemGerarFaturamentoAVista;
            acordoCliente.CabotagemDiasDePrazoFatura = configuracaoFatura.CabotagemDiasDePrazoFatura;
            acordoCliente.CabotagemTipoPrazoFaturamento = configuracaoFatura.CabotagemTipoPrazoFaturamento;
            acordoCliente.CabotagemDiasSemanaFatura = configuracaoFatura.CabotagemDiasSemanaFatura;
            acordoCliente.CabotagemDiasMesFatura = configuracaoFatura.CabotagemDiasMesFatura;
            if (configuracaoFatura.AtualizarCabotagemEmail)
                acordoCliente.CabotagemEmail = configuracaoFatura.CabotagemEmail;

            acordoCliente.LongoCursoGerarFaturamentoAVista = configuracaoFatura.LongoCursoGerarFaturamentoAVista;
            acordoCliente.LongoCursoDiasDePrazoFatura = configuracaoFatura.LongoCursoDiasDePrazoFatura;
            acordoCliente.LongoCursoTipoPrazoFaturamento = configuracaoFatura.LongoCursoTipoPrazoFaturamento;
            acordoCliente.LongoCursoDiasSemanaFatura = configuracaoFatura.LongoCursoDiasSemanaFatura;
            acordoCliente.LongoCursoDiasMesFatura = configuracaoFatura.LongoCursoDiasMesFatura;
            if (configuracaoFatura.AtualizarLongoCursoEmail)
                acordoCliente.LongoCursoEmail = configuracaoFatura.LongoCursoEmail;

            acordoCliente.CustoExtraGerarFaturamentoAVista = configuracaoFatura.CustoExtraGerarFaturamentoAVista;
            acordoCliente.CustoExtraDiasDePrazoFatura = configuracaoFatura.CustoExtraDiasDePrazoFatura;
            acordoCliente.CustoExtraTipoPrazoFaturamento = configuracaoFatura.CustoExtraTipoPrazoFaturamento;
            acordoCliente.CustoExtraDiasSemanaFatura = configuracaoFatura.CustoExtraDiasSemanaFatura;
            acordoCliente.CustoExtraDiasMesFatura = configuracaoFatura.CustoExtraDiasMesFatura;
            if (configuracaoFatura.AtualizarCustoExtraEmail)
                acordoCliente.CustoExtraEmail = configuracaoFatura.CustoExtraEmail;

            if (acordoCliente.Codigo > 0)
                repAcordo.Atualizar(acordoCliente);
            else
                repAcordo.Inserir(acordoCliente);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, acordoCliente, null, "Salvou acordo de faturamento", unitOfWork);
        }

        #endregion
    }
}
