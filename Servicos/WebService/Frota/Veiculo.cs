using Dominio.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Servicos.WebService.Frota
{
    public class Veiculo : ServicoBase
    {        
        public Veiculo(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public void AtualizarVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao, Dominio.Entidades.Veiculo veiculo, ref string mensagem, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularcarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            veiculo.Initialize();

            if (veiculoIntegracao.PossuiTagValePedagio.HasValue)
                veiculo.PossuiTagValePedagio = veiculoIntegracao.PossuiTagValePedagio.Value;

            if (veiculoIntegracao.TipoRodado != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.NaoAplicavel)
            {
                switch (veiculoIntegracao.TipoRodado)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.Outros:
                        veiculo.TipoRodado = "06";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.CavaloMecanico:
                        veiculo.TipoRodado = "03";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.Toco:
                        veiculo.TipoRodado = "02";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.Truck:
                        veiculo.TipoRodado = "01";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.Utilitario:
                        veiculo.TipoRodado = "05";
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.VAN:
                        veiculo.TipoRodado = "04";
                        break;
                }
            }

            if (veiculoIntegracao.TipoCarroceria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria.NaoAplicavel)
                veiculo.TipoCarroceria = string.Format("{0:00}", (int)veiculoIntegracao.TipoCarroceria);

            if (!string.IsNullOrWhiteSpace(veiculoIntegracao.ModeloVeicular?.CodigoIntegracao))
                veiculo.ModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork).buscarPorCodigoIntegracao(veiculoIntegracao.ModeloVeicular.CodigoIntegracao);

            if (!string.IsNullOrWhiteSpace(veiculoIntegracao.NumeroFrota))
                veiculo.NumeroFrota = veiculoIntegracao.NumeroFrota;

            if (!string.IsNullOrWhiteSpace(veiculoIntegracao.NumeroChassi))
                veiculo.Chassi = veiculoIntegracao.NumeroChassi;

            if (!string.IsNullOrWhiteSpace(veiculoIntegracao.UF))
            {
                var estado = repEstado.BuscarPorSigla(veiculoIntegracao.UF);
                if (estado != null)
                    veiculo.Estado = estado;
            }

            if (!string.IsNullOrWhiteSpace(veiculoIntegracao.DataValidadeGR))
            {
                DateTime dataValidadeGR;

                if (!DateTime.TryParseExact(veiculoIntegracao.DataValidadeGR, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataValidadeGR))
                    mensagem += $"A Data Validade GR do veículo ({veiculoIntegracao.Placa}) não está em um formato correto (dd/MM/yyyy). ";
                else
                    veiculo.DataValidadeGerenciadoraRisco = dataValidadeGR;
            }

            veiculo.ValorContainerAverbacao = veiculoIntegracao.ValorContainerAverbacao;

            if (!string.IsNullOrWhiteSpace(veiculoIntegracao.Renavam))
                veiculo.Renavam = veiculoIntegracao.Renavam;

            //Não é possível atualizar devido a ser um enumerador não nulo
            //if (veiculoIntegracao.TipoVeiculo.HasValue)
            //    veiculo.TipoVeiculo = veiculoIntegracao.TipoVeiculo;

            if (veiculoIntegracao.ModeloVeicular != null && !string.IsNullOrWhiteSpace(veiculoIntegracao.ModeloVeicular.CodigoIntegracao))
            {
                var modeloVeicular = repModeloVeicularcarga.buscarPorCodigoIntegracao(veiculoIntegracao.ModeloVeicular.CodigoIntegracao);
                if (modeloVeicular != null)
                    veiculo.ModeloVeicularCarga = modeloVeicular;
            }

            if (veiculoIntegracao.Tara > 0)
                veiculo.Tara = veiculoIntegracao.Tara;

            repositorioVeiculo.Atualizar(veiculo, auditado);
        }

        public Dominio.Entidades.Veiculo SalvarVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao, Dominio.Entidades.Empresa empresaIntegradora, bool cadastrarVeiculoTerceiroParaEmpresa, ref string mensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, bool ignorarVeiculoTerceiro = false, string cpfMotorista = "", bool consultarSeReboqueExisteEmVezDeCadastrar = false)
        {
            if (veiculoIntegracao == null)
                return null;

            Servicos.WebService.Empresa.Motorista serWSMotorista = new Servicos.WebService.Empresa.Motorista(unitOfWork);
            Servicos.WebService.Frota.Modelo serWSModelo = new Servicos.WebService.Frota.Modelo(unitOfWork);
            Servicos.Cliente servicoCliente = new Servicos.Cliente(StringConexao);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repositorioConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);

            Dominio.Entidades.Empresa empresa = null;
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repositorioConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

            if (empresaIntegradora != null)
                empresa = repEmpresa.BuscarPorCodigo(empresaIntegradora.Codigo);
            else if (veiculoIntegracao.Transportador != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(veiculoIntegracao.Transportador.CNPJ)))
            {
                empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(veiculoIntegracao.Transportador.CNPJ));
                if (empresa == null)
                    mensagem += "CNPJ da transportadora (" + veiculoIntegracao.Transportador.CNPJ + ") não possui cadastro.";
            }

            Servicos.Cliente serCliente = new Cliente(StringConexao);

            string placa = !string.IsNullOrWhiteSpace(veiculoIntegracao.Placa) ? veiculoIntegracao.Placa.Replace("-", "") : "";

            bool situacaoAnterior = false;

            Servicos.Veiculo serVeiculo = new Servicos.Veiculo(unitOfWork);

            if (!serVeiculo.ValidarPlaca(placa, unitOfWork))
                mensagem += "A placa informada (" + placa + ") é inválida, por favor informe uma placa válida. ";

            Dominio.Entidades.Veiculo veiculo = null;
            if (empresa != null)
                veiculo = repVeiculo.BuscarPorPlacaIncluiInativos(empresa.Codigo, placa);
            else
                veiculo = repVeiculo.BuscarPorPlacaIncluiInativos(placa);

            bool inserir = false;
            if (veiculo == null)
            {
                inserir = true;
                veiculo = new Dominio.Entidades.Veiculo();
                Servicos.Log.TratarErro("Inserindo veiculo por placa " + placa);
            }
            else
            {
                veiculo.Initialize();
                situacaoAnterior = veiculo.Ativo;
            }

            veiculo.DataAtualizacao = DateTime.Now;

            string mensagemInconsistente = "";
            if (string.IsNullOrEmpty(veiculoIntegracao.UF))
                mensagemInconsistente += "É obrigatório informar a UF do Veículo (" + placa + "). ";
            else
            {
                if (veiculoIntegracao.UF.Length != 2)
                    mensagemInconsistente += "A UF do Veículo (" + placa + ") deve conter dois caracteres, por exemplo, SC, SP, RJ. ";
            }

            if (string.IsNullOrWhiteSpace(veiculoIntegracao.Renavam))
            {
                mensagemInconsistente += "É obrigatório informar o Renavam do veículo (" + placa + "). ";
            }
            else
            {
                if (veiculoIntegracao.Renavam.Length < 9)
                    mensagemInconsistente += "O Renavam do veículo  (" + placa + ") deve conter ao mínimo 9 caracteres. ";

                if (veiculoIntegracao.Renavam.Length > 11)
                    mensagemInconsistente += "O Renavam do veículo  (" + placa + ") deve conter ao máximo 11 caracteres. ";

                if ((configuracao != null) && (configuracao.ValidarRENAVAMVeiculo) && (!Utilidades.Validate.ValidarRENAVAM(veiculoIntegracao.Renavam)))
                    mensagemInconsistente += "O RENAVAM do veículo (" + placa + ") é inválido";
            }

            if (veiculoIntegracao.CapacidadeM3 > 999)
                mensagemInconsistente += "A capacidade cúbica do veículo não pode ser maior que 999. ";

            if (!string.IsNullOrWhiteSpace(veiculoIntegracao.DataSuspensaoInicio))
            {
                DateTime data;
                if (!DateTime.TryParseExact(veiculoIntegracao.DataSuspensaoInicio, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out data))
                {
                    mensagemInconsistente += "A data de inicio da suspensão não está em um formato correto (dd/MM/yyyy HH:mm). ";
                }
                veiculo.DataSuspensaoInicio = data;
                if (veiculo.DataSuspensaoInicio.Value.Year < 1900)
                {
                    veiculo.DataSuspensaoInicio = null;
                    mensagemInconsistente += "O ano de inicio da suspensão não pode ser menor que 1900. ";
                }
            }
            else veiculo.DataSuspensaoInicio = null;

            if (!string.IsNullOrWhiteSpace(veiculoIntegracao.DataSuspensaoFim))
            {
                DateTime data;
                if (!DateTime.TryParseExact(veiculoIntegracao.DataSuspensaoFim, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out data))
                {
                    mensagemInconsistente += "A data fim da suspensão não está em um formato correto (dd/MM/yyyy HH:mm). ";
                }
                veiculo.DataSuspensaoFim = data;
                if (veiculo.DataSuspensaoFim.Value.Year < 1900)
                {
                    veiculo.DataSuspensaoFim = null;
                    mensagemInconsistente += "O ano de fim da suspensão não pode ser menor que 1900. ";
                }
            }
            else veiculo.DataSuspensaoFim = null;

            if (veiculo.DataSuspensaoFim != null && veiculo.DataSuspensaoInicio != null && veiculo.DataSuspensaoFim < veiculo.DataSuspensaoInicio)
                mensagemInconsistente += "Data final de suspensão não pode ser menor que a inicial. ";

            if (!string.IsNullOrWhiteSpace(veiculoIntegracao.DataValidadeGR))
            {
                DateTime dataValidadeGR;
                if (!DateTime.TryParseExact(veiculoIntegracao.DataValidadeGR, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataValidadeGR))
                {
                    mensagemInconsistente += $"A Data Validade GR do veículo ({placa}) não está em um formato correto (dd/MM/yyyy). ";
                }

                veiculo.DataValidadeGerenciadoraRisco = dataValidadeGR;
            }
            else
                veiculo.DataValidadeGerenciadoraRisco = null;

            if (!string.IsNullOrWhiteSpace(mensagemInconsistente))
            {
                if (inserir)
                    mensagem += "A placa " + placa + " não está cadastrada para a transportadora " + (empresa?.Descricao ?? "") + " e foram detectas as seguintes inconsistências ao tentar adicionar o veículo: " + mensagemInconsistente;
                else
                    mensagem += mensagemInconsistente;
            }

            veiculo.Estado = repEstado.BuscarPorSigla(veiculoIntegracao.UF);
            if (veiculo.Estado == null && empresa != null && empresa.Localidade.Estado != null)
                veiculo.Estado = empresa.Localidade.Estado;
            veiculo.Placa = placa;
            veiculo.Renavam = veiculoIntegracao.Renavam;
            veiculo.ValorContainerAverbacao = veiculoIntegracao.ValorContainerAverbacao;
            veiculo.Ativo = veiculoIntegracao.Ativo;
            veiculo.XCampo = veiculoIntegracao.XCampo;
            veiculo.XTexto = veiculoIntegracao.XTexto;
            veiculo.TipoCarroceria = string.Format("{0:00}", (int)veiculoIntegracao.TipoCarroceria);
            veiculo.TipoCombustivel = veiculoIntegracao.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao ? "D" : "O";
            veiculo.TipoRodado = string.Format("{0:00}", (int)veiculoIntegracao.TipoRodado);
            veiculo.TipoVeiculo = veiculoIntegracao.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao ? "0" : "1";
            veiculo.CapacidadeKG = veiculoIntegracao.CapacidadeKG;
            veiculo.Tara = veiculoIntegracao.Tara;
            veiculo.CapacidadeM3 = veiculoIntegracao.CapacidadeM3;
            veiculo.AnoFabricacao = veiculoIntegracao.AnoFabricacao;
            veiculo.AnoModelo = veiculoIntegracao.AnoModelo;
            veiculo.NumeroFrota = veiculoIntegracao.NumeroFrota;
            veiculo.MotivoBloqueio = veiculoIntegracao.MotivoBloqueio;
            if (veiculoIntegracao.PossuiTagValePedagio.HasValue)
                veiculo.PossuiTagValePedagio = veiculoIntegracao.PossuiTagValePedagio.Value;
            if (veiculoIntegracao.TipoTagValePedagio.HasValue)
                veiculo.ModoCompraValePedagioTarget = veiculoIntegracao.TipoTagValePedagio;
            veiculo.NumeroCartaoValePedagio = veiculoIntegracao.NumeroCartaoValePedagio;
            if (veiculoIntegracao.PossuiRastreador.HasValue)
                veiculo.PossuiRastreador = veiculoIntegracao.PossuiRastreador.Value;
            if (veiculoIntegracao.TipoFrota.HasValue)
                veiculo.TipoFrota = veiculoIntegracao.TipoFrota;

            if (!string.IsNullOrWhiteSpace(veiculoIntegracao.NumeroChassi))
            {
                if (veiculoIntegracao.NumeroChassi.Length == 17)
                    veiculo.Chassi = veiculoIntegracao.NumeroChassi;
                else
                    mensagem += "O chassi do veículo (" + placa + ") deve possuir 17 caracteres. ";
            }
            if (veiculo.CapacidadeKG < 0)
                mensagem += "A capacidade KG do veículo (" + placa + ") está inválida: " + veiculo.CapacidadeKG.ToString("D");
            if (veiculo.CapacidadeM3 < 0)
                mensagem += "A capacidade M3 do veículo (" + placa + ") está inválida: " + veiculo.CapacidadeM3.ToString("D");

            veiculo.NumeroMotor = veiculoIntegracao.NumeroMotor;

            if (empresa != null)
            {
                if (empresa.Matriz != null && empresa.Matriz.Count > 0)
                    veiculo.Empresa = empresa.Matriz.FirstOrDefault();
                else
                    veiculo.Empresa = empresa;
            }

            if (string.IsNullOrWhiteSpace(mensagem))
            {

                if (!string.IsNullOrWhiteSpace(veiculoIntegracao.DataAquisicao))
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(veiculoIntegracao.DataAquisicao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        mensagem += "A data de aquisição do veículo  (" + placa + ") não está em um formato correto (dd/MM/yyyy). ";
                    }
                    veiculo.DataCompra = data;

                    if (veiculo.DataCompra.Value.Year < 1900)
                    {
                        veiculo.DataCompra = null;
                        mensagem += "O ano de compra do veículo não pode ser menor que 1900. ";
                    }
                }

                if (veiculoIntegracao.GrupoPessoaSegmento != null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = repGrupoPessoas.BuscarPorCodigoIntegracao(veiculoIntegracao.GrupoPessoaSegmento.CodigoIntegracao);
                    if (grupoPessoa != null)
                    {
                        veiculo.GrupoPessoas = grupoPessoa;
                    }
                    else
                    {
                        mensagem += "Não foi possível encontrar o segmento informado para o veículo (" + placa + ") na base multisoftware, por getileza verifique se o grupo existe na Multisoftware e está vinculado ao Grupo informado para o veículo. ";
                    }
                }
                else
                    veiculo.GrupoPessoas = null;

                if (cadastrarVeiculoTerceiroParaEmpresa && veiculoIntegracao.Proprietario != null && veiculoIntegracao.Proprietario.TransportadorTerceiro != null && !string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ))
                {
                    veiculo.Tipo = "P";

                    veiculo.Proprietario = null;
                    veiculo.FornecedorValePedagio = null;
                    veiculo.ResponsavelValePedagio = null;
                    veiculo.NumeroCompraValePedagio = string.Empty;
                }
                else if ((veiculoIntegracao.TipoPropriedadeVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Terceiros && veiculoIntegracao.Proprietario != null && veiculoIntegracao.Proprietario.TransportadorTerceiro != null) ||
                          veiculoIntegracao.Proprietario != null && veiculoIntegracao.Proprietario.TransportadorTerceiro != null && !string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ) && empresa != null && veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ != empresa.CNPJ)
                {
                    veiculo.Tipo = "T";
                    if (veiculoIntegracao.Proprietario != null && veiculoIntegracao.Proprietario.TransportadorTerceiro != null)
                    {
                        if (!string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.TransportadorTerceiro.RNTRC))
                        {
                            //if (veiculoIntegracao.Proprietario.TransportadorTerceiro.RNTRC.Length == 8)
                            //{
                            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacaoCliente = serCliente.ConverterParaTransportadorTerceiro(veiculoIntegracao.Proprietario.TransportadorTerceiro, "Transportador Terceiro", unitOfWork, false, auditado, tipoServicoMultisoftware);
                            if (retornoVerificacaoCliente.Status)
                            {
                                veiculo.Proprietario = retornoVerificacaoCliente.cliente;
                                veiculo.TipoProprietario = veiculoIntegracao.Proprietario.TipoTACVeiculo;
                                veiculo.RNTRC = int.Parse(veiculoIntegracao.Proprietario.TransportadorTerceiro.RNTRC);

                                if (!string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.CIOT))
                                {
                                    if (veiculoIntegracao.Proprietario.CIOT.Length == 12)
                                        veiculo.CIOT = veiculoIntegracao.Proprietario.CIOT;
                                    else
                                        mensagem += "O CIOT do terceiro " + veiculoIntegracao.Proprietario.TransportadorTerceiro.RazaoSocial + " deve possuir 12 caracteres. ";
                                }
                                else
                                    veiculo.CIOT = "";

                                veiculo.ValorValePedagio = veiculoIntegracao.Proprietario.ValorValePedagio;

                                if (!string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.NumeroCompraValePedagio))
                                {
                                    if (veiculoIntegracao.Proprietario.NumeroCompraValePedagio.Length < 20)
                                        veiculo.NumeroCompraValePedagio = veiculoIntegracao.Proprietario.NumeroCompraValePedagio;
                                    else
                                        mensagem += "O número de compra do vale pedágio do terceiro " + veiculoIntegracao.Proprietario.TransportadorTerceiro.RazaoSocial + " não deve possuir mais que 20 caracteres. ";
                                }
                                else
                                    veiculo.NumeroCompraValePedagio = "";

                                if (veiculoIntegracao.Proprietario.FornecedorValePedagio != null)
                                {
                                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = servicoCliente.ConverterObjetoValorPessoa(veiculoIntegracao.Proprietario.FornecedorValePedagio, "Fornecedor do Vale Pedágio", unitOfWork, 0, false, false, auditado);
                                    if (retornoConversao.Status == false)
                                        mensagem += retornoConversao.Mensagem;
                                    else
                                        veiculo.FornecedorValePedagio = retornoConversao.cliente;
                                }
                                else
                                    veiculo.FornecedorValePedagio = null;

                                if (veiculoIntegracao.Proprietario.ResponsavelValePedagio != null)
                                {
                                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = servicoCliente.ConverterObjetoValorPessoa(veiculoIntegracao.Proprietario.ResponsavelValePedagio, "Responsável do Vale Pedágio", unitOfWork, 0, false, false, auditado);
                                    if (retornoConversao.Status == false)
                                        mensagem += retornoConversao.Mensagem;
                                    else
                                        veiculo.ResponsavelValePedagio = retornoConversao.cliente;
                                }
                                else
                                    veiculo.ResponsavelValePedagio = null;

                            }
                            else
                            {
                                mensagem += retornoVerificacaoCliente.Mensagem;
                            }
                            //}
                            //else
                            //{
                            //    mensagem += "O RNTRC do terceiro " + veiculoIntegracao.Proprietario.TransportadorTerceiro.RazaoSocial + " deve possuir 8 caracteres. ";
                            //}
                        }
                        else
                        {
                            mensagem += "É obrigatório informar o RNTRC do terceiro " + veiculoIntegracao.Proprietario.TransportadorTerceiro.RazaoSocial + ". ";
                        }
                    }
                    else
                    {
                        mensagem += "É obrigatório informar os dados do Transportador Terceiro quando um veículo é de terceiros (veículo placa: " + placa + "). ";
                    }
                }
                else if (ignorarVeiculoTerceiro && !string.IsNullOrWhiteSpace(cpfMotorista))
                {
                    veiculo.Tipo = "T";
                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCPF(cpfMotorista);
                    if (motorista != null && motorista.Localidade != null)
                    {
                        double.TryParse(motorista.CPF, out double cpfMotoristaDouble);
                        Dominio.Entidades.Cliente proprietario = repCliente.BuscarPorCPFCNPJ(cpfMotoristaDouble);
                        if (proprietario == null)
                        {
                            proprietario = Servicos.Embarcador.Pessoa.Pessoa.ConverterFuncionario(motorista, unitOfWork);
                            repCliente.Inserir(proprietario);
                            veiculo.Proprietario = proprietario;
                        }
                    }
                }
                else
                {
                    veiculo.Tipo = "P";

                    veiculo.Proprietario = null;
                    veiculo.FornecedorValePedagio = null;
                    veiculo.ResponsavelValePedagio = null;
                    veiculo.NumeroCompraValePedagio = string.Empty;
                }

                if (veiculoIntegracao.ModeloVeicular != null)
                {
                    veiculo.ModeloVeicularCarga = repModeloVeicularCarga.buscarPorCodigoIntegracao(veiculoIntegracao.ModeloVeicular.CodigoIntegracao);
                    if (veiculo.ModeloVeicularCarga != null)
                    {
                        bool permite = false;
                        if (veiculo.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral)
                            permite = true;
                        else if (veiculoIntegracao.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao && veiculo.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao)
                            permite = true;
                        else if (veiculoIntegracao.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque && veiculo.ModeloVeicularCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Reboque)
                            permite = true;

                        if (!permite)
                        {
                            string descricaoTipo = veiculoIntegracao.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque ? "Reboque" : "Tração";
                            mensagem += "Informe um modelo veícular compatível com o tipo do veículo. (" + descricaoTipo + ") para o veículo  (" + placa + "). ";
                        }
                    }
                }
                else if (configuracaoVeiculo.ObrigatorioInformarModeloVeicularCargaNoWebService)
                {
                    if (veiculoIntegracao.ModeloVeicular == null)
                        mensagem += "É obrigatório informar o Modelo Veicular da carga do veiculo (" + placa + ")";

                    if (veiculo.ModeloVeicularCarga == null)
                        mensagem += "O código do modelo veicular de carga do veículo informado (" + veiculoIntegracao.ModeloVeicular.CodigoIntegracao + ") Não foi encontrado na base Multisoftware. ";
                }

                if (veiculoIntegracao.Modelo != null)
                {
                    if (veiculoIntegracao.Modelo.Marca != null)
                    {
                        veiculo.Modelo = serWSModelo.SalvarModeloVeiculo(veiculoIntegracao.Modelo, unitOfWork, auditado);
                        veiculo.Marca = veiculo.Modelo.MarcaVeiculo;
                    }
                    else
                    {
                        mensagem += "É obrigatório informar a marca do veículo (" + placa + ") se informar o modelo para o mesmo. ";
                    }
                }
            }

            if (veiculo.VeiculosVinculados != null)
                veiculo.VeiculosVinculados.Clear();

            if (string.IsNullOrWhiteSpace(mensagem) && veiculoIntegracao.Reboques != null)
            {
                veiculo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboqueIntegracao in veiculoIntegracao.Reboques)
                {
                    string mensagemReboque = string.Empty;

                    Dominio.Entidades.Veiculo reboqueSalvar = null;

                    if (empresaIntegradora != null && consultarSeReboqueExisteEmVezDeCadastrar)
                        reboqueSalvar = repVeiculo.BuscarPorPlacaIncluiInativos(empresaIntegradora.Codigo, reboqueIntegracao.Placa);

                    reboqueSalvar ??= this.SalvarVeiculo(reboqueIntegracao, empresa, cadastrarVeiculoTerceiroParaEmpresa, ref mensagemReboque, unitOfWork, tipoServicoMultisoftware, auditado, false, string.Empty, true);

                    if (reboqueSalvar != null)
                        veiculo.VeiculosVinculados.Add(reboqueSalvar);

                    if (!string.IsNullOrWhiteSpace(mensagemReboque))
                        mensagem = mensagemReboque;
                }
            }

            if (string.IsNullOrWhiteSpace(mensagem))
            {
                if (inserir)
                    repVeiculo.Inserir(veiculo, auditado);
                else
                {
                    Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculo, situacaoAnterior, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MetodosAlteracaoVeiculo.SalvarVeiculo_VeiculoWebService, null, unitOfWork);
                    repVeiculo.Atualizar(veiculo);
                    Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(auditado, veiculo, veiculo.GetChanges(), "Atualizou o veículo.", unitOfWork);
                }

                if (veiculoIntegracao.CentroCarregamentos != null && veiculoIntegracao.CentroCarregamentos.Count > 0)
                {
                    foreach (var centroCarregamento in veiculoIntegracao.CentroCarregamentos)
                    {
                        Dominio.Entidades.Embarcador.Logistica.CentroCarregamento existeCentroCarregamento = repCentroCarregamento.BuscarPorCodigoIntegracao(centroCarregamento.CodigoIntegracao);
                        if (existeCentroCarregamento == null)
                            continue;

                        if (existeCentroCarregamento.Veiculos.Any(o => o.Placa == veiculo.Placa))
                            continue;

                        existeCentroCarregamento.Veiculos.Add(veiculo);
                        repCentroCarregamento.Atualizar(existeCentroCarregamento);
                    }
                }

                if (veiculoIntegracao.Motoristas != null && veiculoIntegracao.Motoristas.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao in veiculoIntegracao.Motoristas)
                    {
                        if (motoristaIntegracao.tipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio)
                        {
                            if (veiculo.Tipo == "P")
                                motoristaIntegracao.tipoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio;
                            else
                                motoristaIntegracao.tipoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro;
                        }

                        Dominio.Entidades.Usuario motorista = serWSMotorista.SalvarMotorista(motoristaIntegracao, empresa, ref mensagem, unitOfWork, tipoServicoMultisoftware, auditado); //todo: rever, pois atualmente vincula apenas um motorista ao veículo
                        if (motorista != null)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(auditado, veiculo, $"Removido motorista principal.", unitOfWork);
                            repVeiculoMotorista.DeletarMotoristaPrincipal(veiculo.Codigo);
                            Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista VeiculoMotoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                            {
                                CPF = motorista.CPF,
                                Motorista = motorista,
                                Nome = motorista.Nome,
                                Veiculo = veiculo,
                                Principal = true
                            };
                            repVeiculoMotorista.Inserir(VeiculoMotoristaPrincipal);
                        }
                    }
                }
                else
                {
                    bool limparMotorista = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LimparMotoristaIntegracaoVeiculo.Value;
                    if (limparMotorista)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(auditado, veiculo, $"Removido motorista principal.", unitOfWork);
                        repVeiculoMotorista.DeletarMotoristaPrincipal(veiculo.Codigo);
                    }
                }

                Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculo);

                return veiculo;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo ConverterObjetoConjuntoVeiculos(Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Veiculo> reboques, Repositorio.UnitOfWork unitOfWork)
        {
            bool retornarModeloVeiculo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().RetornarModeloVeiculo.Value; //Criado com urgência pois minerva solicitou urgência para retornar o modelo do veiculo

            return ConverterObjetoConjuntoVeiculos(veiculo, reboques, unitOfWork, ordemReboque: 0, cargaVeiculoContainer: null, retornarModeloVeiculo);
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo ConverterObjetoConjuntoVeiculos(Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Veiculo> reboques, Repositorio.UnitOfWork unitOfWork, int ordemReboque, Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer, bool retornarModeloVeiculo, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null, List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> veiculosMotoristas = null)
        {
            if (veiculo == null)
                return null;

            //bool retornarModeloVeiculo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().RetornarModeloVeiculo.Value; //Criado com urgência pois minerva solicitou urgência para retornar o modelo do veiculo

            Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao = ConverterObjetoVeiculo(veiculo, unitOfWork, modeloVeicularCarga: (!retornarModeloVeiculo && (reboques?.Count ?? 0) > 0) ? reboques[0].ModeloVeicularCarga : null, configuracaoTMS: configuracaoTMS, veiculosMotoristas: veiculosMotoristas);
            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo, Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> repositorioLicencaVeiculoAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo, Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer>(unitOfWork);

            if (reboques != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in reboques)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo DynReboque = ConverterObjetoVeiculo(reboque, unitOfWork, ordemReboque, configuracaoTMS: configuracaoTMS, veiculosMotoristas: veiculosMotoristas);
                    DynReboque.Reboques = null;
                    DynReboque.DataRetiradaCtrn = cargaVeiculoContainer != null && cargaVeiculoContainer.DataRetiradaCtrn.HasValue ? cargaVeiculoContainer.DataRetiradaCtrn.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
                    DynReboque.NumeroContainer = cargaVeiculoContainer != null ? cargaVeiculoContainer.NumeroContainer : string.Empty;
                    DynReboque.TaraContainer = cargaVeiculoContainer != null ? cargaVeiculoContainer.TaraContainer : 0;
                    DynReboque.MaxGross = cargaVeiculoContainer != null ? cargaVeiculoContainer.MaxGross : 0;

                    if (cargaVeiculoContainer != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo> anexos = repositorioLicencaVeiculoAnexo.BuscarPorEntidade(cargaVeiculoContainer.Codigo);

                        if (anexos.Count > 0)
                        {
                            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", typeof(Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo).Name });
                            DynReboque.Anexos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Anexo>();

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo anexoContainer in anexos)
                            {
                                string extensao = $".{anexoContainer.ExtensaoArquivo}";
                                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexoContainer.GuidArquivo + extensao);

                                byte[] bufferAnexo = null;
                                if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                                {
                                    bufferAnexo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

                                    Dominio.ObjetosDeValor.Embarcador.Carga.Anexo anexo = new Dominio.ObjetosDeValor.Embarcador.Carga.Anexo();
                                    anexo.Nome = anexoContainer.NomeArquivo;
                                    anexo.Extensao = extensao;
                                    anexo.Arquivo = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, bufferAnexo));

                                    DynReboque.Anexos.Add(anexo);
                                }
                                else
                                    Servicos.Log.TratarErro("Anexo Veiculo Container não localizado: " + fileLocation);


                            }
                        }
                    }

                    veiculoIntegracao.Reboques.Add(DynReboque);
                }
            }

            return veiculoIntegracao;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo ConverterObjetoVeiculo(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork, int ordemReboque = 0, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null, List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> veiculosMotoristas = null)
        {
            if (veiculo == null)
                return null;

            Servicos.WebService.Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Frota.Modelo serModelo = new Modelo(unitOfWork);
            Servicos.WebService.Carga.ModeloVeicularCarga serConverterObjetoModeloVeicular = new Carga.ModeloVeicularCarga(unitOfWork);
            Servicos.WebService.Empresa.Motorista serMotorista = new Empresa.Motorista(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Empresa.Empresa(unitOfWork);

            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            if (configuracaoTMS == null)
                configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();
            veiculoIntegracao.Reboques = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();

            veiculoIntegracao.Protocolo = veiculo.Codigo;
            veiculoIntegracao.OrdemReboque = ordemReboque;
            veiculoIntegracao.AnoFabricacao = veiculo.AnoFabricacao;
            veiculoIntegracao.AnoModelo = veiculo.AnoModelo;
            veiculoIntegracao.Transportador = serEmpresa.ConverterObjetoEmpresa(veiculo.Empresa);
            veiculoIntegracao.Ativo = veiculo.Ativo;
            veiculoIntegracao.CapacidadeKG = veiculo.CapacidadeKG;
            veiculoIntegracao.CapacidadeM3 = veiculo.CapacidadeM3;
            veiculoIntegracao.Codigo = veiculo.Codigo;
            veiculoIntegracao.DataAquisicao = veiculo.DataCompra.HasValue ? veiculo.DataCompra.Value.ToString("dd/MM/yyyy") : "";
            veiculoIntegracao.GrupoPessoaSegmento = serPessoa.ConverterObjetoGrupoPessoa(veiculo.GrupoPessoas);
            veiculoIntegracao.Modelo = serModelo.ConverterObjetoModelo(veiculo.Modelo);
            veiculoIntegracao.ModeloVeicular = serConverterObjetoModeloVeicular.ConverterObjetoModeloVeicular(modeloVeicularCarga ?? veiculo.ModeloVeicularCarga);

            veiculoIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
            veiculoIntegracao.Equipamentos = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento>();

            Dominio.Entidades.Usuario veiculoMotorista = veiculosMotoristas != null ? veiculosMotoristas.FirstOrDefault(vm => vm.Veiculo.Codigo == veiculo.Codigo)?.Motorista : repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

            if (veiculoMotorista != null)
                veiculoIntegracao.Motoristas.Add(serMotorista.ConverterObjetoMotorista(veiculoMotorista));

            if (veiculo.Equipamentos.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento in veiculo.Equipamentos)
                    veiculoIntegracao.Equipamentos.Add(ConverterObjetoEquipamento(equipamento, unitOfWork));
            }

            veiculoIntegracao.NumeroChassi = veiculo.Chassi;
            veiculoIntegracao.NumeroFrota = veiculo.NumeroFrota;
            veiculoIntegracao.NumeroMotor = veiculo.NumeroMotor;
            veiculoIntegracao.Placa = veiculo.Placa;

            veiculoIntegracao.TipoPropriedadeVeiculo = veiculo.Tipo == "P" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Proprio : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Terceiros;
            if (veiculoIntegracao.TipoPropriedadeVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Terceiros)
            {
                veiculoIntegracao.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();
                veiculoIntegracao.Proprietario.TipoTACVeiculo = veiculo.TipoProprietario;
                if (veiculo.Proprietario != null)
                {
                    veiculoIntegracao.Proprietario.TransportadorTerceiro = serEmpresa.ConverterObjetoEmpresa(veiculo.Proprietario);
                    if (veiculoIntegracao.Proprietario.TransportadorTerceiro != null)
                    {
                        veiculoIntegracao.Proprietario.TransportadorTerceiro.RNTRC = veiculo.RNTRC > 0 ? veiculo.RNTRC.ToString() : "";
                        veiculoIntegracao.Proprietario.CIOT = veiculo.CIOT;
                    }
                }
                else
                    veiculoIntegracao.TipoPropriedadeVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Proprio;
            }

            veiculoIntegracao.Renavam = veiculo.Renavam;
            veiculoIntegracao.RNTC = veiculo.RNTRC.ToString();
            veiculoIntegracao.Tara = veiculo.Tara;
            veiculoIntegracao.UF = veiculo.Estado.Sigla;

            if (!string.IsNullOrWhiteSpace(veiculo.TipoCarroceria))
                veiculoIntegracao.TipoCarroceria = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria)int.Parse(veiculo.TipoCarroceria);
            else
                veiculoIntegracao.TipoCarroceria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria.NaoAplicavel;

            if (!string.IsNullOrWhiteSpace(veiculo.TipoRodado))
                veiculoIntegracao.TipoRodado = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado)int.Parse(veiculo.TipoRodado);
            else
                veiculoIntegracao.TipoRodado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.NaoAplicavel;

            veiculoIntegracao.TipoVeiculo = veiculo.TipoVeiculo == "0" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque;
            veiculoIntegracao.UF = veiculo.Estado.Sigla;
            veiculoIntegracao.Cor = veiculo?.CorVeiculo?.Descricao;

            veiculoIntegracao.PossuiRastreador = veiculo.PossuiRastreador;
            veiculoIntegracao.NumeroEquipamentoRastreador = veiculo.NumeroEquipamentoRastreador;
            veiculoIntegracao.TecnologiaRastreador = ConverterObjetoTecnologiaRastreador(veiculo.TecnologiaRastreador);
            veiculoIntegracao.TipoComunicacaoRastreador = ConverterObjetoTipoComunicacaoRastreador(veiculo.TipoComunicacaoRastreador);
            veiculoIntegracao.CentroResultado = ConverterObjetoCentroResultado(veiculo.CentroResultado);
            veiculoIntegracao.SegmentoVeiculo = ConverterObjetoSegmentoVeiculo(veiculo.SegmentoVeiculo);
            veiculoIntegracao.KilometragemAtual = veiculo.KilometragemAtual;
            veiculoIntegracao.CapacidadeMaximaTanque = veiculo.CapacidadeMaximaTanque;
            veiculoIntegracao.DescricaoModeloVeiculo = veiculo.Modelo?.Descricao ?? string.Empty;
            veiculoIntegracao.DescricaoMarcaVeiculo = veiculo.Marca?.Descricao ?? string.Empty;

            return veiculoIntegracao;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.TabelaPosto ConverterObjetoTabelaPosto(Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores tabelaPosto, Repositorio.UnitOfWork unitOfWork)
        {
            if (tabelaPosto == null)
                return null;

            Servicos.WebService.Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frota.TabelaPosto tab = new Dominio.ObjetosDeValor.Embarcador.Frota.TabelaPosto()
            {
                Codigo = tabelaPosto.Codigo,
                CodigoIntegracao = tabelaPosto.CodigoIntegracao,
                DataFinal = tabelaPosto.DataFinal.HasValue && tabelaPosto.DataFinal.Value > DateTime.MinValue ? tabelaPosto.DataFinal.Value.ToString("dd/MM/yyyy") : "",
                DataInicial = tabelaPosto.DataInicial.HasValue && tabelaPosto.DataInicial.Value > DateTime.MinValue ? tabelaPosto.DataInicial.Value.ToString("dd/MM/yyyy") : "",
                PercentualDesconto = tabelaPosto.PercentualDesconto,
                UnidadeMedida = tabelaPosto.DescricaoUnidadeDeMedida,
                ValorAte = tabelaPosto.ValorAte.HasValue ? tabelaPosto.ValorAte.Value : 0m,
                ValorDe = tabelaPosto.ValorFixo,
                Posto = serPessoa.ConverterObjetoPessoa(tabelaPosto.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente),
                Produto = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Produto()
                {
                    Codigo = tabelaPosto.Produto?.Codigo ?? 0,
                    CodigoIntegracao = tabelaPosto.Produto?.CodigoProduto ?? "",
                    Descricao = tabelaPosto.Produto?.Descricao ?? "",
                    NCM = tabelaPosto.Produto?.CodigoNCM ?? ""
                }
            };

            return tab;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento ConverterObjetoEquipamento(Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento, Repositorio.UnitOfWork unitOfWork)
        {
            if (equipamento == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento equipamentoRetorno = new Dominio.ObjetosDeValor.Embarcador.Frota.Equipamento()
            {
                Codigo = equipamento.Codigo,
                Descricao = equipamento.Descricao,
                Numero = equipamento.Numero,
                Chassi = equipamento.Chassi,
                Ativo = equipamento.Ativo,
                Hodometro = equipamento.Hodometro,
                Horimetro = equipamento.Horimetro,
                DataAquisicao = equipamento.DataAquisicao.HasValue ? equipamento.DataAquisicao.Value.ToString("dd/MM/yyyy") : "",
                AnoFabricacao = equipamento.AnoFabricacao,
                AnoModelo = equipamento.AnoModelo,
                Modelo = equipamento.ModeloEquipamento?.Descricao ?? "",
                Marca = equipamento.MarcaEquipamento?.Descricao ?? "",
                Segmento = equipamento.SegmentoVeiculo?.Descricao ?? "",
                CentroResultado = ConverterObjetoCentroResultado(equipamento.CentroResultado),
                Cor = equipamento.Cor,
                Renavan = equipamento.Renavam,
                CapacidadeTanque = equipamento.CapacidadeTanque
            };

            return equipamentoRetorno;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frota.TecnologiaRastreador ConverterObjetoTecnologiaRastreador(Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador)
        {
            if (tecnologiaRastreador == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Frota.TecnologiaRastreador()
            {
                Descricao = tecnologiaRastreador.Descricao,
                CodigoIntegracao = tecnologiaRastreador.CodigoIntegracao
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.TipoComunicacaoRastreador ConverterObjetoTipoComunicacaoRastreador(Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador tipoComunicacaoRastreador)
        {
            if (tipoComunicacaoRastreador == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Frota.TipoComunicacaoRastreador()
            {
                Descricao = tipoComunicacaoRastreador.Descricao,
                CodigoIntegracao = tipoComunicacaoRastreador.CodigoIntegracao
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado ConverterObjetoCentroResultado(Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado)
        {
            if (centroResultado == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado()
            {
                Descricao = centroResultado.Descricao,
                Codigo = centroResultado.Codigo
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.SegmentoVeiculo ConverterObjetoSegmentoVeiculo(Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmentoVeiculo)
        {
            if (segmentoVeiculo == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Frota.SegmentoVeiculo()
            {
                Descricao = segmentoVeiculo.Descricao,
                Codigo = segmentoVeiculo.Codigo
            };
        }
        #endregion
    }
}
