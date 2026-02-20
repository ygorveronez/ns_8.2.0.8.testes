using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using CoreWCF;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Empresa(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IEmpresa
    {
        #region Métodos Globais     
        
        public Retorno<int> SalvarOrdemCompra(Dominio.ObjetosDeValor.Embarcador.Financeiro.OrdemCompra ordemCompra)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.WebService.Financeiro.DocumentoEntrada serWSDocumentoEntrada = new Servicos.WebService.Financeiro.DocumentoEntrada(unitOfWork);
            try
            {
                Servicos.Log.TratarErro("SalvarOrdemCompra: " + Newtonsoft.Json.JsonConvert.SerializeObject(ordemCompra));

                string mensagem = "";

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem = serWSDocumentoEntrada.SalvarOrdemCompra(ordemCompra, integradora?.Empresa, ref mensagem, unitOfWork, TipoServicoMultisoftware, Auditado);

                if (ordem == null)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro("SalvarOrdemCompra: " + mensagem);
                    return new Retorno<int>() { Mensagem = mensagem, Objeto = 0, Status = false };
                }
                else if (ordem != null)
                {
                    unitOfWork.CommitChanges();

                    Servicos.Log.TratarErro("SalvarOrdemCompra: Sucesso: " + ordem.Codigo.ToString());

                    return new Retorno<int>() { Mensagem = "", Objeto = ordem.Codigo, Status = true };
                }
                else
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro("SalvarOrdemCompra: " + mensagem);
                    return new Retorno<int>() { Mensagem = mensagem, Objeto = 0, Status = false };
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarOrdemCompra: " + ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> InformarMacroVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.Macro macroIntegracao, Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            try
            {
                unitOfWork.Start();

                if (macroIntegracao != null)
                {
                    Dominio.Entidades.Veiculo veiculo = null;
                    if (integradora.Empresa != null)
                        veiculo = repVeiculo.BuscarPorPlacaIncluiInativos(integradora.Empresa.Codigo, veiculoIntegracao.Placa);
                    else
                        veiculo = repVeiculo.BuscarPorPlacaIncluiInativos(veiculoIntegracao.Placa);

                    if (veiculo != null)
                    {
                        Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegraca = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

                        Repositorio.Embarcador.Veiculos.MacroVeiculo repMacroVeiculo = new Repositorio.Embarcador.Veiculos.MacroVeiculo(unitOfWork);
                        Repositorio.Embarcador.Veiculos.Macro repMacro = new Repositorio.Embarcador.Veiculos.Macro(unitOfWork);
                        Dominio.Entidades.Embarcador.Veiculos.Macro macro = repMacro.BuscarPorCodigoIntegracao(macroIntegracao.CodigoIntegracao);


                        if (macro != null)
                        {


                            DateTime data;
                            if (!string.IsNullOrWhiteSpace(macroIntegracao.DataMacro))
                            {
                                if (!DateTime.TryParseExact(macroIntegracao.DataMacro, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                                {
                                    unitOfWork.Rollback();
                                    return new Retorno<bool>() { Mensagem = "A data da macro não esta em um formato correto (dd/MM/yyyy HH:mm:ss);", Objeto = false, Status = false };
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new Retorno<bool>() { Mensagem = "É obrigatório informar a data da Macro.", Objeto = false, Status = false };
                            }


                            //todo: tratar tipos de macros aqui, hoje está fixo com angellira.
                            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegraca.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira);

                            if (tipoIntegracao == null)
                            {
                                unitOfWork.Rollback();
                                return new Retorno<bool>() { Mensagem = "Não existe nenhum tipo de integração compativel com o envio de macros habilitado.", Objeto = false, Status = false };
                            }

                            //if (string.IsNullOrWhiteSpace(macroIntegracao.CodigoIntegracaoViagem))
                            //{
                            //    unitOfWork.Rollback();
                            //    return new Retorno<bool>() { Mensagem = "É obrigatório informar o código de integração da carga.", Objeto = false, Status = false };
                            //}

                            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = null; // repCargaCargaIntegracao.BuscarPorProtocoloETipo(macroIntegracao.CodigoIntegracaoViagem, tipoIntegracao.Codigo);
                            if (!string.IsNullOrWhiteSpace(macroIntegracao.CodigoIntegracaoViagem))
                            {
                                cargaCargaIntegracao = repCargaCargaIntegracao.BuscarPorProtocoloETipo(macroIntegracao.CodigoIntegracaoViagem, tipoIntegracao.Codigo);
                                if (cargaCargaIntegracao == null)
                                {
                                    unitOfWork.Rollback();
                                    return new Retorno<bool>() { Mensagem = "Não foi localizada nenhuma carga compativel com o codigo de integração " + macroIntegracao.CodigoIntegracaoViagem + ".", Objeto = false, Status = false };
                                }
                            }

                            Dominio.Entidades.Embarcador.Veiculos.MacroVeiculo macroVeiculo = new Dominio.Entidades.Embarcador.Veiculos.MacroVeiculo
                            {
                                DataMacro = data,
                                DataRecebimento = DateTime.Now,
                                Veiculo = veiculo,
                                Macro = macro,
                                Carga = cargaCargaIntegracao?.Carga,
                                Latitude = macroIntegracao.Latitude,
                                Longitude = macroIntegracao.Longitude
                            };

                            repMacroVeiculo.Inserir(macroVeiculo);

                            Servicos.Embarcador.Veiculo.Macro.ProcessarMacroRecebida(macroVeiculo, TipoServicoMultisoftware, unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, macroVeiculo, "Informou macro do veículo", unitOfWork);

                            unitOfWork.CommitChanges();

                            return new Retorno<bool>() { Mensagem = "", Objeto = true, Status = true };
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new Retorno<bool>() { Mensagem = "Não foi localizada uma macro com o código de integração " + macroIntegracao.CodigoIntegracao + ".", Objeto = false, Status = false };
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new Retorno<bool>() { Mensagem = "Não foi possível encontrar o veículo " + veiculoIntegracao.Placa + ".", Objeto = false, Status = false };
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new Retorno<bool>() { Mensagem = "É obrigatório informar a macro.", Status = false };
                }

            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new Retorno<bool>() { Mensagem = excecao.Message, Status = false };
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração da macro.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SalvarVeiculo(veiculoIntegracao, integradora));
            });
        }

        public Retorno<bool> SalvarSituacaoColaborador(Dominio.ObjetosDeValor.Embarcador.Carga.SituacaoColaboradorIntegracaoWS situacaoColaboradorIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.Embarcador.Pessoa.ColaboradorSituacaoLancamento(unitOfWork, Auditado, Conexao.createInstance(_serviceProvider).AdminStringConexao).SalvarSituacaoColaborador(situacaoColaboradorIntegracao));
            });
        }

        public Retorno<string> SalvarVeiculoProtocolo(Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.IntegracaoVeiculo repIntegracaoVeiculo = new Repositorio.IntegracaoVeiculo(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Servicos.WebService.Frota.Veiculo(unitOfWork);
            try
            {
                Servicos.Log.TratarErro("SalvarVeiculoProtocolo: " + Newtonsoft.Json.JsonConvert.SerializeObject(veiculoIntegracao));

                string mensagem = "";

                Dominio.Entidades.Veiculo veiculo = null;
                Dominio.Entidades.Veiculo veiculoTerceiro = null;

                unitOfWork.Start();

                veiculo = serWSVeiculo.SalvarVeiculo(veiculoIntegracao, integradora.Empresa, false, ref mensagem, unitOfWork, TipoServicoMultisoftware, Auditado);

                if (veiculo == null)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro("SalvarVeiculoProtocolo: " + mensagem);
                    return new Retorno<string>() { Mensagem = mensagem, Objeto = "", Status = false };
                }

                if (configuracaoTMS != null && configuracaoTMS.CadastrarVeiculoTerceiroParaEmpresa && veiculoIntegracao.Proprietario.TransportadorTerceiro != null && !string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ) && Utilidades.Validate.ValidarCNPJ(veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ))
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ);
                    if (empresa == null)
                    {
                        unitOfWork.CommitChanges();
                        return new Retorno<string>() { Mensagem = "Transportador/proprietário do veículo informado (" + veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ + ") não possui cadastro.", Objeto = "", Status = false };
                    }

                    veiculoTerceiro = serWSVeiculo.SalvarVeiculo(veiculoIntegracao, empresa, true, ref mensagem, unitOfWork, TipoServicoMultisoftware, Auditado);
                    if (!string.IsNullOrWhiteSpace(mensagem))
                        Servicos.Log.TratarErro("SalvarVeiculoProtocolo Terceiro: " + mensagem);

                    if (veiculoTerceiro != null && veiculo != null)
                    {
                        unitOfWork.CommitChanges();

                        Dominio.Entidades.IntegracaoVeiculo integracaoVeiculo = new Dominio.Entidades.IntegracaoVeiculo();
                        integracaoVeiculo.Veiculo = veiculoTerceiro != null ? veiculoTerceiro : veiculo;
                        integracaoVeiculo.Arquivo = Newtonsoft.Json.JsonConvert.SerializeObject(veiculoIntegracao);
                        repIntegracaoVeiculo.Inserir(integracaoVeiculo);

                        Servicos.Log.TratarErro("SalvarVeiculoProtocolo: Sucesso: " + integracaoVeiculo.Codigo.ToString());
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, "Salvou protocolo do veículo", unitOfWork);

                        return new Retorno<string>() { Mensagem = "", Objeto = integracaoVeiculo.Codigo.ToString(), Status = true };
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro("SalvarVeiculoProtocolo: " + mensagem);
                        return new Retorno<string>() { Mensagem = mensagem, Objeto = "", Status = false };
                    }
                }
                else
                {
                    unitOfWork.CommitChanges();

                    Dominio.Entidades.IntegracaoVeiculo integracaoVeiculo = new Dominio.Entidades.IntegracaoVeiculo();
                    integracaoVeiculo.Veiculo = veiculo;
                    integracaoVeiculo.Arquivo = Newtonsoft.Json.JsonConvert.SerializeObject(veiculoIntegracao);
                    repIntegracaoVeiculo.Inserir(integracaoVeiculo);

                    Servicos.Log.TratarErro("SalvarVeiculoProtocolo: Sucesso: " + integracaoVeiculo.Codigo.ToString());
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, "Salvou protocolo do veículo", unitOfWork);

                    return new Retorno<string>() { Mensagem = "", Objeto = integracaoVeiculo.Codigo.ToString(), Status = true };
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarVeiculoProtocolo: " + ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarMotorista(Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SalvarMotorista(motoristaIntegracao, integradora));
            });
        }

        public Retorno<string> SalvarMotoristaProtocolo(Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.IntegracaoMotorista repIntegracaoMotorista = new Repositorio.IntegracaoMotorista(unitOfWork);

            Servicos.WebService.Empresa.Motorista serWSMotorista = new Servicos.WebService.Empresa.Motorista(unitOfWork);

            try
            {
                Servicos.Log.TratarErro("SalvarMotoristaProtocolo: " + Newtonsoft.Json.JsonConvert.SerializeObject(motoristaIntegracao));

                unitOfWork.Start();
                string mensagem = "";

                if (motoristaIntegracao.tipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos)
                    motoristaIntegracao.tipoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio;

                Dominio.Entidades.Usuario motorista = serWSMotorista.SalvarMotorista(motoristaIntegracao, integradora.Empresa, ref mensagem, unitOfWork, TipoServicoMultisoftware, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao);

                if (motorista != null)
                {
                    unitOfWork.CommitChanges();

                    Dominio.Entidades.IntegracaoMotorista integracaoMotorista = new Dominio.Entidades.IntegracaoMotorista();
                    integracaoMotorista.Usuario = motorista;
                    integracaoMotorista.Arquivo = Newtonsoft.Json.JsonConvert.SerializeObject(motoristaIntegracao);
                    repIntegracaoMotorista.Inserir(integracaoMotorista);

                    Servicos.Log.TratarErro("SalvarMotoristaProtocolo: Sucesso: " + integracaoMotorista.Codigo.ToString());
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, motorista, "Salvou protocolo do motorista", unitOfWork);

                    return new Retorno<string>() { Mensagem = "", Objeto = integracaoMotorista.Codigo.ToString(), Status = true };
                }
                else
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro("SalvarMotoristaProtocolo: " + mensagem);
                    return new Retorno<string>() { Mensagem = mensagem, Objeto = "", Status = false };
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarMotoristaProtocolo: " + ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SalvarVeiculoValePedagioCIOT(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresa, Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculo, Dominio.ObjetosDeValor.MDFe.ValePedagio valePedagio, Dominio.ObjetosDeValor.MDFe.CIOT ciot)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Servicos.WebService.Frota.Veiculo(unitOfWork);
            try
            {
                Servicos.Log.TratarErro("SalvarVeiculoValePedagioCIOT Empresa: " + Newtonsoft.Json.JsonConvert.SerializeObject(empresa));
                Servicos.Log.TratarErro("SalvarVeiculoValePedagioCIOT Veiculo: " + Newtonsoft.Json.JsonConvert.SerializeObject(veiculo));
                Servicos.Log.TratarErro("SalvarVeiculoValePedagioCIOT ValePedagio: " + Newtonsoft.Json.JsonConvert.SerializeObject(valePedagio));
                Servicos.Log.TratarErro("SalvarVeiculoValePedagioCIOT CIOT: " + Newtonsoft.Json.JsonConvert.SerializeObject(ciot));

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                if (empresa == null)
                    return new Retorno<bool>() { Mensagem = "Obrigatório envio de uma empresa.", Status = false };
                if (string.IsNullOrWhiteSpace(empresa.CNPJ))
                    return new Retorno<bool>() { Mensagem = "Obrigatório envio do CNPJ da empresa.", Status = false };
                if (veiculo == null)
                    return new Retorno<bool>() { Mensagem = "Obrigatório envio de um veículo.", Status = false };
                if (string.IsNullOrWhiteSpace(veiculo.Placa))
                    return new Retorno<bool>() { Mensagem = "Obrigatório envio de uma placa.", Status = false };

                Dominio.Entidades.Empresa empresaCadastro = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(empresa.CNPJ));
                if (empresaCadastro == null)
                    return new Retorno<bool>() { Mensagem = "Empresa (" + empresa.CNPJ + ") não localizada.", Status = false };

                string mensagem = "";

                Dominio.Entidades.Veiculo veiculoCadastro = serWSVeiculo.SalvarVeiculo(veiculo, empresaCadastro, false, ref mensagem, unitOfWork, TipoServicoMultisoftware, Auditado);

                if (ciot != null && !string.IsNullOrWhiteSpace(ciot.Numero))
                {
                    if (!string.IsNullOrWhiteSpace(ciot.CNPJCPFResponsavel))
                    {
                        Dominio.Entidades.Cliente responsavelCIOT = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(ciot.CNPJCPFResponsavel)));
                        if (responsavelCIOT == null)
                            return new Retorno<bool>() { Mensagem = "Respnsável vale pedágio (" + ciot.CNPJCPFResponsavel + ") não cadastrado no Embarcador.", Status = false };
                        veiculoCadastro.ResponsavelCIOT = responsavelCIOT;
                    }
                    else
                        veiculoCadastro.ResponsavelCIOT = null;

                    veiculoCadastro.CIOT = ciot.Numero;
                }
                else
                {
                    veiculoCadastro.ResponsavelCIOT = null;
                    veiculoCadastro.CIOT = string.Empty;
                }

                if (valePedagio != null && !string.IsNullOrWhiteSpace(valePedagio.NumeroComprovante))
                {
                    Dominio.Entidades.Cliente fornecedorValePedagio = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(valePedagio.CNPJFornecedor)));
                    if (fornecedorValePedagio == null)
                        return new Retorno<bool>() { Mensagem = "Fornecedor vale pedágio (" + valePedagio.CNPJFornecedor + ") não cadastrado no Embarcador.", Status = false };

                    Dominio.Entidades.Cliente responsavelValePedagio = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(valePedagio.CNPJResponsavel)));
                    if (responsavelValePedagio == null)
                        return new Retorno<bool>() { Mensagem = "Respnsável vale pedágio (" + valePedagio.CNPJResponsavel + ") não cadastrado no Embarcador.", Status = false };

                    veiculoCadastro.FornecedorValePedagio = fornecedorValePedagio;
                    veiculoCadastro.ResponsavelValePedagio = responsavelValePedagio;
                    veiculoCadastro.NumeroCompraValePedagio = valePedagio.NumeroComprovante;
                    veiculoCadastro.ValorValePedagio = valePedagio.ValorValePedagio;
                }
                else
                {
                    veiculoCadastro.FornecedorValePedagio = null;
                    veiculoCadastro.ResponsavelValePedagio = null;
                    veiculoCadastro.NumeroCompraValePedagio = string.Empty;
                    veiculoCadastro.ValorValePedagio = 0;
                }

                repVeiculo.Atualizar(veiculoCadastro);

                if (veiculoCadastro != null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoCadastro, "Salvou veículo do vale pedágio do CIOT", unitOfWork);

                    return new Retorno<bool>() { Mensagem = "", Objeto = true, Status = true };
                }
                else
                {
                    return new Retorno<bool>() { Mensagem = mensagem, Objeto = false, Status = false };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> BuscarTransportadores(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>>();
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>();
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

                    List<Dominio.Entidades.Empresa> empresas = repEmpresa.Consultar(true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, "Codigo", "desc", (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repEmpresa.ContarConsulta(true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                    retorno.Objeto.Itens = serEmpresa.RetornarEmpresas(empresas);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou transportadores", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os transportadores";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento>> BuscarEquipamentosPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Frota.Veiculo serWSVeiculo = new Servicos.WebService.Frota.Veiculo(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento>();
                    Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> equipamentos = repEquipamento.ConsultarEquipamentosPendentesIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento in equipamentos)
                        retorno.Objeto.Itens.Add(serWSVeiculo.ConverterObjetoEquipamento(equipamento, unitOfWork));

                    retorno.Objeto.NumeroTotalDeRegistro = repEquipamento.ContarConsultarEquipamentosPendentesIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou equipamentos", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os equipamentos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.TabelaPosto>> BuscarTabelaPostoPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.TabelaPosto>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.TabelaPosto>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Frota.Veiculo serWSVeiculo = new Servicos.WebService.Frota.Veiculo(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.TabelaPosto>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Frota.TabelaPosto>();
                    Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores> tabelas = repPostoCombustivelTabelaValores.ConsultarEquipamentosPendentesIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores tabela in tabelas)
                        retorno.Objeto.Itens.Add(serWSVeiculo.ConverterObjetoTabelaPosto(tabela, unitOfWork));

                    retorno.Objeto.NumeroTotalDeRegistro = repPostoCombustivelTabelaValores.ContarConsultarEquipamentosPendentesIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou equipamentos", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar a tabela de posto";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>> BuscarVeiculosPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Frota.Veiculo serWSVeiculo = new Servicos.WebService.Frota.Veiculo(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
                    parametroConsulta.InicioRegistros = (int)inicio;
                    parametroConsulta.LimiteRegistros = (int)limite;

                    Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo();
                    filtrosPesquisa.CodigoEmpresa = integradora.Empresa?.Codigo ?? 0;
                    filtrosPesquisa.PendenteIntegracaoEmbarcador = true;

                    List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.ConsultarEmbarcador(filtrosPesquisa, parametroConsulta);

                    foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
                        retorno.Objeto.Itens.Add(serWSVeiculo.ConverterObjetoVeiculo(veiculo, unitOfWork));

                    retorno.Objeto.NumeroTotalDeRegistro = repVeiculo.ContarConsultaEmbarcador(filtrosPesquisa);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou transportadores", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os veículos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>> BuscarMotoristasPendentesIntegracao(int? inicio, int? limite)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>>();
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
                    Servicos.WebService.Empresa.Motorista serWSMotorista = new Servicos.WebService.Empresa.Motorista(unitOfWork);

                    Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

                    var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotorista()
                    {
                        SituacaoColaborador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Todos,
                        Empresa = null,
                        Nome = "",
                        CpfCnpj = "",
                        Tipo = "",
                        Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo,
                        PlacaVeiculo = "",
                        SomentePendenteDeVinculo = false,
                        PendenteIntegracaoEmbarcador = true,
                        CodigoCargo = 0,
                        ProprietarioTerceiro = 0d,
                        NumeroMatricula = "",
                        CnpjRemetenteLocalCarregamentoAutorizado = 0d,
                        NaoBloqueado = false,
                    };

                    List<Dominio.Entidades.Usuario> motoristas = repMotorista.ConsultarEmbarcador(filtrosPesquisa, "Codigo", "desc", (int)inicio, (int)limite);
                    foreach (Dominio.Entidades.Usuario motorista in motoristas)
                        retorno.Objeto.Itens.Add(serWSMotorista.ConverterObjetoMotorista(motorista));

                    retorno.Objeto.NumeroTotalDeRegistro = repMotorista.ContarConsultaEmbarcador(filtrosPesquisa);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou transportadores", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os motoristas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> BuscarTransportadoresPendentesIntegracao(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (limite > 100)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 100.");

                List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> listaEmpresas = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>();

                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

                int totalRegistros = repositorioEmpresa.ContarConsultaTransportadoresPendentesIntegracao();

                if (totalRegistros > 0)
                {
                    Servicos.WebService.Empresa.Empresa serWSEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

                    List<Dominio.Entidades.Empresa> transportadores = repositorioEmpresa.ConsultarTransportadoresPendentesIntegracao("Codigo", "desc", (int)inicio, (int)limite);
                    foreach (Dominio.Entidades.Empresa transportador in transportadores)
                        listaEmpresas.Add(serWSEmpresa.ConverterObjetoEmpresa(transportador));
                }

                Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> retornoEmpresa = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>()
                {
                    Itens = listaEmpresas,
                    NumeroTotalDeRegistro = totalRegistros
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou transportadores", unitOfWork);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>>.CriarRetornoSucesso(retornoEmpresa);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os transportadores.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> BuscarTransportadoresPendentesIntegracaoERP(int? quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>>.CreateFrom(new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarTransportadoresPendentesIntegracao(quantidade ?? 0));
                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>>()
                {
                    DataRetorno = retorno.DataRetorno,
                    CodigoMensagem = retorno.CodigoMensagem,
                    Mensagem = retorno.Mensagem,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>.CreateFrom(retorno.Objeto),
                    Status = retorno.Status
                };
            });
        }

        public Retorno<bool> ConfirmarIntegracaoEquipamento(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(protocolo);

                if (equipamento != null)
                {
                    equipamento.Integrado = false;
                    repEquipamento.Atualizar(equipamento);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado um equipamento para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração do equipamento.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoTabelaPosto(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores tabela = repPostoCombustivelTabelaValores.BuscarPorCodigo(protocolo);

                if (tabela != null)
                {
                    tabela.Integrado = false;
                    repPostoCombustivelTabelaValores.Atualizar(tabela);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado uma tabela de posto para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da tabela do posto.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoVeiculo(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(protocolo);

                if (veiculo != null)
                {
                    veiculo.PendenteIntegracaoEmbarcador = false;
                    repVeiculo.Atualizar(veiculo);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado um veiculo para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração do veiculo.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoTransportador(string cnpj)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                ValidarToken();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCNPJ(cnpj);

                if (transportador == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um transportador para o protocolo informado.");

                if (transportador.Integrado)
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao("Transportador já foi integrado.");

                transportador.Integrado = true;
                repEmpresa.Atualizar(transportador);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração do transportador.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarIntegracaoMotorista(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(protocolo);

                if (motorista != null)
                {
                    motorista.PendenteIntegracaoEmbarcador = false;
                    repMotorista.Atualizar(motorista);
                    retorno.Status = true;
                    retorno.Objeto = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado um motorista para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração do motorista.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> ConsultarTransportador(string cnpj)
        {
            ValidarToken();
            Retorno<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>();
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpj));

                if (empresa != null)
                {
                    if (empresa.Status == "A")
                    {
                        Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);
                        retorno.Objeto = serEmpresa.ConverterObjetoEmpresa(empresa);
                        retorno.Status = true;

                        Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Consultou transportadores", unitOfWork);
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Mensagem = "O transportador com o CNPJ " + cnpj + " está inativado na base Multisoftware";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "Não existe nenhuma transportadora cadastrada com o CNPJ " + cnpj + " na base Multisoftware";
                }



            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consulta o transportador";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> SalvarTransportador(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa transportador)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SalvarTransportador(transportador));
            });
        }

        public Retorno<bool> InformarDadosBancariosTransportador(string cnpjTransportador, string codigoBanco, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoContaBanco, string numeroConta, string agencia, string digitoAgencia)
        {
            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjTransportador));

                if (empresa != null)
                {
                    int.TryParse(codigoBanco, out int numeroBanco);
                    Dominio.Entidades.Banco banco = repBanco.BuscarPorNumero(numeroBanco);
                    if (banco != null)
                    {
                        empresa.Banco = banco;
                        empresa.TipoContaBanco = tipoContaBanco;
                        empresa.NumeroConta = numeroConta;
                        empresa.Agencia = agencia;
                        empresa.DigitoAgencia = digitoAgencia;

                        repEmpresa.Atualizar(empresa);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, "Atualizado dados bancários via WebService", unitOfWork);

                        Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cnpjTransportador)));
                        if (cliente != null)
                        {
                            cliente.Banco = banco;
                            cliente.TipoContaBanco = tipoContaBanco;
                            cliente.NumeroConta = numeroConta;
                            cliente.Agencia = agencia;
                            cliente.DigitoAgencia = digitoAgencia;
                            cliente.DataUltimaAtualizacao = DateTime.Now;
                            cliente.Integrado = false;
                            repCliente.Atualizar(cliente);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, "Atualizado dados bancários via WebService", unitOfWork);
                        }

                        retorno.Objeto = true;
                        retorno.Status = true;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Mensagem = "Banco " + codigoBanco + " não localizado.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "Transportador " + cnpjTransportador + " não localizado.";
                }


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao salvar transportador";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> SalvarFeriado(Dominio.ObjetosDeValor.Embarcador.Configuracoes.Feriado feriado)
        {
            Servicos.Log.TratarErro("SalvarFeriado: " + Newtonsoft.Json.JsonConvert.SerializeObject(feriado));

            ValidarToken();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Feriado repFeriado = new Repositorio.Embarcador.Configuracoes.Feriado(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                string mensagemValidacao = ValidarCamposFeriado(feriado);

                if (string.IsNullOrWhiteSpace(mensagemValidacao))
                {
                    Dominio.Entidades.Localidade localidade = feriado.Localidade.IBGE > 0 ? repLocalidade.BuscarPorCodigoIBGE(feriado.Localidade.IBGE) : repLocalidade.BuscarPorDescricaoEUF(feriado.Localidade.Descricao, feriado.Localidade.SiglaUF);
                    Dominio.Entidades.Embarcador.Configuracoes.Feriado feriadoCadastro = repFeriado.BuscarPorCodigoIntegracao(feriado.CodigoIntegracao);

                    if (feriadoCadastro != null)
                        feriadoCadastro.Initialize();
                    else
                        feriadoCadastro = new Dominio.Entidades.Embarcador.Configuracoes.Feriado();

                    feriadoCadastro.Descricao = feriado.Descricao;
                    feriadoCadastro.Ativo = feriado.Ativo.HasValue ? feriado.Ativo.Value : true;

                    if (feriado.Ano > 0)
                        feriadoCadastro.Ano = feriado.Ano;
                    else
                        feriadoCadastro.Ano = null;

                    feriadoCadastro.Dia = feriado.Dia;
                    feriadoCadastro.Mes = feriado.Mes;
                    feriadoCadastro.Tipo = feriado.Tipo.Value;
                    feriadoCadastro.CodigoIntegracao = feriado.CodigoIntegracao;
                    feriadoCadastro.Localidade = null;
                    feriadoCadastro.Estado = null;

                    if (feriado.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado.Municipal)
                        feriadoCadastro.Localidade = localidade;
                    else if (feriado.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado.Estadual)
                        feriadoCadastro.Estado = repEstado.BuscarPorSigla(feriado.SiglaUF);

                    if (feriadoCadastro.Codigo > 0)
                        repFeriado.Atualizar(feriadoCadastro, Auditado);
                    else
                        repFeriado.Inserir(feriadoCadastro, Auditado);

                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = mensagemValidacao;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao salvar o feriado.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        private string ValidarCamposFeriado(Dominio.ObjetosDeValor.Embarcador.Configuracoes.Feriado feriado)
        {
            if (feriado == null)
                return "Nennhum dado do feriado recebido.";

            if (string.IsNullOrWhiteSpace(feriado.Descricao))
                return "O campo descrição é obrigatório.";

            if (string.IsNullOrWhiteSpace(feriado.CodigoIntegracao))
                return "O campo código integração é obrigatório.";

            if (feriado.Dia <= 0)
                return "O campo dia é obrigatório.";

            if (feriado.Mes <= 0)
                return "O campo mês é obrigatório.";

            if (!feriado.Tipo.HasValue)
                return "O campo tipo do feríado é obrigatório.";

            return string.Empty;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.ConsultaAbastecimento>> BuscarAbastecimentosPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.ConsultaAbastecimento>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.ConsultaAbastecimento>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Frota.Abastecimento serWSAbastecimento = new Servicos.WebService.Frota.Abastecimento(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.ConsultaAbastecimento>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Frota.ConsultaAbastecimento>();
                    Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

                    List<Dominio.Entidades.Abastecimento> abastecimentos = repAbastecimento.ConsultarPentendeIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Abastecimento abastecimento in abastecimentos)
                        retorno.Objeto.Itens.Add(serWSAbastecimento.ConverterObjetoAbastecimento(abastecimento, unitOfWork));

                    retorno.Objeto.NumeroTotalDeRegistro = repAbastecimento.ContarConsultarPentendeIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou abastecimentos", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os abastecimentos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoAbastecimento(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(protocolo);

                if (abastecimento != null)
                {
                    abastecimento.Integrado = true;
                    repAbastecimento.Atualizar(abastecimento);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado um abastecimento para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração do abastecimento.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TipoMovimento>> BuscarTipoMovimento(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TipoMovimento>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TipoMovimento>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Financeiro.TipoMovimento serWSTipoMovimento = new Servicos.WebService.Financeiro.TipoMovimento(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TipoMovimento>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.TipoMovimento>();
                    Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento> tiposMovimentos = repTipoMovimento.ConsultarIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento in tiposMovimentos)
                        retorno.Objeto.Itens.Add(serWSTipoMovimento.ConverterObjetoTipoMovimento(tipoMovimento, unitOfWork));

                    retorno.Objeto.NumeroTotalDeRegistro = repTipoMovimento.ContarConsultarIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou tipos de movimento", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os tipos de movimento";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Navio>> BuscarNaviosPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Navio>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Navio>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Navio>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Navio>();
                    Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Pedidos.Navio> navios = repNavio.ConsultarPentendeIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.Navio navio in navios)
                        retorno.Objeto.Itens.Add(serWSCarga.ConverterObjetoNavio(navio));

                    retorno.Objeto.NumeroTotalDeRegistro = repNavio.ContarConsultarPentendeIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Navios", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os Navios";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoNavio(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Navio navio = repNavio.BuscarPorCodigo(protocolo);

                if (navio != null)
                {
                    navio.Integrado = true;
                    repNavio.Atualizar(navio);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado um Navio para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração do Navio.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Viagem>> BuscarViagemPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Viagem>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Viagem>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Viagem>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Viagem>();
                    Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio> viagens = repViagem.ConsultarPentendeIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem in viagens)
                        retorno.Objeto.Itens.Add(serWSCarga.ConverterObjetoViagem(viagem));

                    retorno.Objeto.NumeroTotalDeRegistro = repViagem.ContarConsultarPentendeIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Viagens", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os Viagens";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoViagem(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = repViagem.BuscarPorCodigo(protocolo);

                if (viagem != null)
                {
                    viagem.Integrado = true;
                    repViagem.Atualizar(viagem);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado uma Viagem para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da Viagem.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Porto>> BuscarPortoPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Porto>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Porto>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.Porto>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Porto>();
                    Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Pedidos.Porto> portos = repPorto.ConsultarPentendeIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.Porto porto in portos)
                        retorno.Objeto.Itens.Add(serWSCarga.ConverterObjetoPorto(porto));

                    retorno.Objeto.NumeroTotalDeRegistro = repPorto.ContarConsultarPentendeIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Portos", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os Portos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoPorto(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Porto porto = repPorto.BuscarPorCodigo(protocolo);

                if (porto != null)
                {
                    porto.Integrado = true;
                    repPorto.Atualizar(porto);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado uma Porto para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da Porto.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer>> BuscarTipoContainerIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoContainer>();
                    Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo> tipos = repContainerTipo.ConsultarPentendeIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipo in tipos)
                        retorno.Objeto.Itens.Add(serWSCarga.ConverterObjetoTipoContainer(tipo));

                    retorno.Objeto.NumeroTotalDeRegistro = repContainerTipo.ContarConsultarPentendeIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Tipos Containeres", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os Tipos Containeres";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoTipoContainer(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipo = repContainerTipo.BuscarPorCodigo(protocolo);

                if (tipo != null)
                {
                    tipo.Integrado = true;
                    repContainerTipo.Atualizar(tipo);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado uma Tipo de Container para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da Tipo de Container.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto>> BuscarTerminalPortoPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Carga.TerminalPorto>();
                    Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> terminais = repTipoTerminalImportacao.ConsultarPentendeIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal in terminais)
                        retorno.Objeto.Itens.Add(serWSCarga.ConverterObjetoTerminalPorto(terminal));

                    retorno.Objeto.NumeroTotalDeRegistro = repTipoTerminalImportacao.ContarConsultarPentendeIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Terminais Portuarios", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os Terminais Portuarios";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoTerminalPorto(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = repTipoTerminalImportacao.BuscarPorCodigo(protocolo);

                if (terminal != null)
                {
                    terminal.Integrado = true;
                    repTipoTerminalImportacao.Atualizar(terminal);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado um Terminal para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração do Terminal.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>> BuscarProdutoPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>>();
            retorno.Mensagem = "";
            try
            {
                if (limite <= 100)
                {
                    Servicos.WebService.Carga.ProdutosPedido serProdutosPedido = new Servicos.WebService.Carga.ProdutosPedido(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                    Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = repProdutoEmbarcador.ConsultarPentendeIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto in produtos)
                        retorno.Objeto.Itens.Add(serProdutosPedido.ConveterObjetoProduto(produto));

                    retorno.Objeto.NumeroTotalDeRegistro = repProdutoEmbarcador.ContarConsultarPentendeIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Produtos", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os Produtos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoProduto(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(protocolo);

                if (produtoEmbarcador != null)
                {
                    produtoEmbarcador.Integrado = true;
                    repProdutoEmbarcador.Atualizar(produtoEmbarcador);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado um Produto para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração do Produto.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.OrdemServico>> BuscarOrdemServicoFinalizacaPendenteIntegracao(int? inicio, int? limite)
        {
            var unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            
            var retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.OrdemServico>>();
            
            retorno.Mensagem = "";

            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite <= 100)
                {
                    var serWSOrdemServico = new Servicos.WebService.Frota.OrdemServico(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.OrdemServico>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Frota.OrdemServico>();

                    Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);

                    var ordensServicos = repOrdemServicoFrota.ConsultarFinalizadasPentendeIntegracao("Codigo", "desc", (int)inicio, (int)limite);

                    foreach (var ordemServico in ordensServicos)
                        retorno.Objeto.Itens.Add(serWSOrdemServico.ConverterObjetoOrdemServico(ordemServico, unitOfWork));

                    retorno.Objeto.NumeroTotalDeRegistro = repOrdemServicoFrota.ContarConsultarFinalizadasPentendeIntegracao();
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou ordens de serviços finalizadas e pendentes de integração", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar as ordens de serviços finalizadas e pendentes de integração";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoOrdemServicoFinalizada(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>() { Status = true };
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repOrdemServicoFrota.BuscarPorCodigo(protocolo);

                if (ordemServico != null)
                {
                    ordemServico.Integrado = true;
                    repOrdemServicoFrota.Atualizar(ordemServico);
                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi localizado uma ordem de serviço para o protocolo informado.";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da ordem de serviço.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoTransportadorIntegracaoERP(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoTransportador(protocolos));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>> BuscarVeiculosPendentesIntegracaoERP(int? quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>>.CreateFrom(new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarVeiculosPendentesIntegracao(quantidade ?? 0));
                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>.CreateFrom(retorno.Objeto)
                } ;
            });
        }
        
        public Retorno<bool> ConfirmarIntegracaoVeiculoERP(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoVeiculo(protocolos));
            });
        }

        public Retorno<bool> InformarDisponibilidadeFrota(Dominio.ObjetosDeValor.WebService.Logistica.Veiculo.FilaCarregamentoVeiculo FilaCarregamentoVeiculo)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();
                Servicos.Log.TratarErro("InformarDisponibilidadeFrota: " + Newtonsoft.Json.JsonConvert.SerializeObject(FilaCarregamentoVeiculo));

                StringBuilder stMensagem = new StringBuilder();

                Servicos.WebService.Empresa.Empresa servicoEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao);

                int existente = servicoEmpresa.ValidarFilaCarregamentoVeiculo(FilaCarregamentoVeiculo, stMensagem);

                if (stMensagem.Length > 0)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro("InformarDisponibilidadeFrota: " + stMensagem.ToString());
                    return new Retorno<bool>() { Mensagem = stMensagem.ToString(), Objeto = false, Status = false };
                }

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo retorno = servicoEmpresa.InformarDisponibilidadeFrota(FilaCarregamentoVeiculo, stMensagem, existente);

                if (stMensagem.Length > 0 || retorno == null)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro("InformarDisponibilidadeFrota: " + stMensagem.ToString());
                    return new Retorno<bool>() { Mensagem = stMensagem.ToString(), Objeto = false, Status = false };
                }

                unitOfWork.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, retorno, "Informou disponibilidade da frota", unitOfWork);
                Servicos.Log.TratarErro($"InformarDisponibilidadeFrota: {(existente > 0 ? "Atualização - " : "")}Sucesso: " + retorno.Codigo.ToString("D"));

                return new Retorno<bool>() { Mensagem = existente > 0 ? "Atualizado" : "Informado", Objeto = true, Status = true };
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("InformarDisponibilidadeFrota: " + ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceEmpresa;
        }

        #endregion
    }
}
