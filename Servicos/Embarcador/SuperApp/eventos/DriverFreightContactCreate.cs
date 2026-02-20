using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Pessoas;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Servicos.Embarcador.SuperApp.Eventos
{
    public class DriverFreightContactCreate : IntegracaoSuperApp
    {
        #region Construtores

        public DriverFreightContactCreate(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware = null) : base(unitOfWork, unitOfWorkAdmin, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkAdmin = unitOfWorkAdmin;
        }

        #endregion

        #region Métodos Públicos

        public void ProcessarEvento(Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracaoSuperApp, out RetornoIntegracaoSuperApp retornoIntegracaoSuperApp)
        {
            retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();

            try
            {
                string jsonRequisicao = integracaoSuperApp.StringJsonRequest;

                if (string.IsNullOrEmpty(jsonRequisicao))
                    throw new ServicoException($"Arquivo de integração/Request não encontrado.");


                EventoSuperApp eventoDriverFreightContactCreate = Newtonsoft.Json.JsonConvert.DeserializeObject<EventoSuperApp>(jsonRequisicao);

                if (eventoDriverFreightContactCreate == null)
                    throw new ServicoException("Falha na conversão da requisição para objeto.");

                int carCodigo = eventoDriverFreightContactCreate.Data.Freight.ExternalId.ObterSomenteNumeros().ToInt();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(carCodigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos.DadosTransporteCarga dadosTransporteCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos.DadosTransporteCarga();

                dadosTransporteCarga.CodigoCarga = carCodigo;

                if (carga == null)
                    throw new ServicoException($"Carga não encontrada. Carga: {carCodigo}");

                Servicos.Log.TratarErro("Inicio Processar Cliente DriverFreightContactCreate", "IntegracaoSuperAPPOutrosTipos");
                ProcessarClienteEVeiculo(eventoDriverFreightContactCreate, carga, dadosTransporteCarga);

                Servicos.Log.TratarErro("Inicio Processar Motorisa DriverFreightContactCreate", "IntegracaoSuperAPPOutrosTipos");
                ProcessarMotorista(eventoDriverFreightContactCreate, carga, dadosTransporteCarga);

                Servicos.Embarcador.Cargas.CargaOfertaIntegracao servicoCargaOfertaIntegracao = new Servicos.Embarcador.Cargas.CargaOfertaIntegracao(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TrizyOfertarCarga);

                servicoCargaOfertaIntegracao.GerarIntegracaoOfertadeCarga(carga, tipoIntegracao, TipoIntegracaoOfertaCarga.Completar, CancellationToken.None).GetAwaiter().GetResult();

                SalvarDadosTransporteCarga(dadosTransporteCarga);

                retornoIntegracaoSuperApp.Sucesso = true;

                Servicos.Log.TratarErro("Fim Processar Cliente DriverFreightContactCreate", "IntegracaoSuperAPPOutrosTipos");
            }
            catch (ServicoException ex)
            {
                retornoIntegracaoSuperApp.Mensagem = ex.Message;
                Servicos.Log.TratarErro(ex);
            }
            catch (Exception ex)
            {
                retornoIntegracaoSuperApp.Mensagem = "Falha genérica ao processar " + TipoEventoApp.DriverFreightContactCreate.ObterDescricao();
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion

        #region Metodos Privados

        private void SalvarDadosTransporteCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos.DadosTransporteCarga dadosTransporteCarga)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(dadosTransporteCarga.CodigoCarga);

                try
                {
                    string mensagemErro = string.Empty;

                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte()
                    {
                        Carga = carga,
                        CodigoEmpresa = carga.Empresa?.Codigo ?? 0,
                        CodigoModeloVeicular = carga.ModeloVeicularCarga?.Codigo ?? 0,
                        CodigoReboque = dadosTransporteCarga.CodigoReboque1,
                        CodigoSegundoReboque = dadosTransporteCarga.CodigoReboque2,
                        CodigoTerceiroReboque = dadosTransporteCarga.CodigoReboque3,
                        CodigoTipoCarga = carga.TipoDeCarga?.Codigo ?? 0,
                        CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                        CodigoTracao = dadosTransporteCarga.CodigoTracao,
                        NumeroPager = carga.NumeroPager,
                        ObservacaoTransportador = carga.ObservacaoTransportador
                    };

                    dadosTransporte.ListaCodigoMotorista.Add(dadosTransporteCarga.CodigoMotorista);


                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                    {
                        OrigemAuditado = OrigemAuditado.Sistema,
                        TipoAuditado = TipoAuditado.Sistema
                    };

                    servicoCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, usuario: null, liberarComProblemaIntegracaoGrMotoristaVeiculo: false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, webServiceConsultaCTe: string.Empty, cliente: null, auditado, _unitOfWork);

                    if (!string.IsNullOrWhiteSpace(mensagemErro))
                        throw new ServicoException(mensagemErro);
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);
                }

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        private void ProcessarMotorista(EventoSuperApp eventoDriverFreightContactCreate, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos.DadosTransporteCarga dadosTransporteCarga)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            string cpf = eventoDriverFreightContactCreate.Data.Driver.Document.Value.ObterSomenteNumeros();
            Dominio.Entidades.Usuario motorista = repositorioUsuario.BuscarMotoristaPorCPF(carga.Empresa.Codigo, cpf);

            if (motorista != null)
            {
                dadosTransporteCarga.CodigoMotorista = motorista.Codigo;
                return;
            }


            motorista = new Dominio.Entidades.Usuario();
            motorista.Nome = eventoDriverFreightContactCreate.Data.Driver.FullName;
            motorista.Setor = new Dominio.Entidades.Setor { Codigo = 1 }; // Operacional
            motorista.TipoMotorista = TipoMotorista.Todos;
            motorista.Celular = eventoDriverFreightContactCreate.Data.Driver.CellPhone.ObterSomenteNumeros();
            motorista.Telefone = motorista.Celular;
            motorista.Empresa = carga.Empresa;
            motorista.Tipo = "M";
            motorista.Status = "A";
            motorista.CPF = cpf;

            repositorioUsuario.Inserir(motorista, auditado, descricaoAcao: "Adicionado via evento - DriverFreightContactCreate");

            dadosTransporteCarga.CodigoMotorista = motorista.Codigo;
        }

        private void ProcessarClienteEVeiculo(EventoSuperApp eventoDriverFreightContactCreate, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Integracao.TrizyEventos.DadosTransporteCarga dadosTransporteCarga)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            Repositorio.Embarcador.Pessoas.ModalidadePessoas repositorioModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repositorioModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(_unitOfWork);


            if (eventoDriverFreightContactCreate.Data?.VehicleComposition == null || eventoDriverFreightContactCreate.Data.VehicleComposition.Vehicles.Count == 0)
                throw new ServicoException($"Json Sem Veiculos.");

            int count = 0;

            List<Dominio.Entidades.Veiculo> reboques = new List<Dominio.Entidades.Veiculo>();
            Dominio.Entidades.Veiculo tracao = null;

            foreach (var vehicle in eventoDriverFreightContactCreate.Data.VehicleComposition.Vehicles)
            {
                count++;

                bool EhTracao = count == eventoDriverFreightContactCreate.Data.VehicleComposition.Vehicles.Count;

                if (string.IsNullOrEmpty(vehicle.NationalRegistryOfRoadCargoTransporter?.Number))
                    throw new ServicoException($"RNTRC inexistente");

                if (vehicle.NationalRegistryOfRoadCargoTransporter.Number.Length > 8)
                    throw new ServicoException($"RNTRC inválido: {vehicle.NationalRegistryOfRoadCargoTransporter.Number}, não pode ter mais de 8 Digitos");

                if (string.IsNullOrEmpty(vehicle.Owner?.Document?.Value))
                    throw new ServicoException($"CPF do Proprietário Inexistente.");

                if (string.IsNullOrEmpty(vehicle.Owner?.Name))
                    throw new ServicoException($"Nome do Proprietário Inexistente.");

                if (string.IsNullOrEmpty(vehicle.LicensePlate))
                    throw new ServicoException($"Placa inexistente.");

                if (string.IsNullOrEmpty(vehicle.NationalRegister))
                    throw new ServicoException($"Renavam inexistente.");

                long cpf = vehicle.Owner.Document.Value.ObterSomenteNumeros().ToLong();
                Dominio.Entidades.Cliente cliente = repositorioCliente.BuscarPorCPFCNPJ(cpf);

                if (cliente == null)
                {
                    cliente = ObterNovoCliente(vehicle, carga, cpf, auditado);
                    repositorioCliente.Inserir(cliente, auditado, descricaoAcao: "Adicionado via evento - DriverFreightContactCreate");

                    Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = ObterNovaModalidadePessoas(cliente);
                    repositorioModalidadePessoas.Inserir(modalidade);

                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadora = ObterNovaModalidadeTransportadoraPessoas(modalidade, vehicle);
                    repositorioModalidadeTransportadoraPessoas.Inserir(modalidadeTransportadora, auditado, descricaoAcao: "Adicionado via evento - DriverFreightContactCreate");
                }

                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlacaTodas(carga.Empresa.Codigo, vehicle.LicensePlate);

                if (veiculo == null)
                {
                    veiculo = ObterNovoVeiculo(vehicle, cliente, carga, EhTracao, auditado);

                    repositorioVeiculo.Inserir(veiculo, auditado, descricaoAcao: "Adicionado via evento - DriverFreightContactCreate");
                }
                else
                {
                    veiculo = ObterAtualizacaoVeiculo(veiculo, cliente, vehicle, EhTracao, auditado);

                    repositorioVeiculo.Atualizar(veiculo, auditado, descricaoAcao: "Atualizado via evento - DriverFreightContactCreate");
                }

                if (EhTracao)
                    tracao = veiculo;
                else
                    reboques.Add(veiculo);

            }

            if (tracao != null)
            {
                dadosTransporteCarga.CodigoTracao = tracao.Codigo;

                if (tracao.VeiculosVinculados == null)
                    tracao.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                else
                    tracao.VeiculosVinculados.Clear();

                int countReboque = 0;
                foreach (var item in reboques)
                {
                    countReboque++;

                    switch (countReboque)
                    {
                        case 1:
                            dadosTransporteCarga.CodigoReboque1 = item.Codigo;
                            break;
                        case 2:
                            dadosTransporteCarga.CodigoReboque2 = item.Codigo;
                            break;
                        case 3:
                            dadosTransporteCarga.CodigoReboque3 = item.Codigo;
                            break;
                        default:
                            break;
                    }

                    tracao.VeiculosVinculados.Add(item);
                }

                repositorioVeiculo.Atualizar(tracao, auditado, descricaoAcao: "Tração Adicionado via evento - DriverFreightContactCreate");
            }
        }

        private Dominio.Entidades.Veiculo ObterAtualizacaoVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Cliente cliente, Vehicle vehicle, bool ehTracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.MarcaVeiculo repositorioMarcaVeiculo = new Repositorio.MarcaVeiculo(_unitOfWork);
            Repositorio.ModeloVeiculo repositorioModeloVeiculo = new Repositorio.ModeloVeiculo(_unitOfWork);

            veiculo.TipoVeiculo = ehTracao ? "0" : "1";
            veiculo.Proprietario = cliente;

            veiculo.Cor = vehicle.Color;
            veiculo.AnoFabricacao = vehicle.ManufactureYear;
            veiculo.AnoModelo = vehicle.ModelYear;
            veiculo.Renavam = vehicle.NationalRegister;
            veiculo.Chassi = vehicle.VehicleIdentificationNumber;
            veiculo.RNTRC = vehicle.NationalRegistryOfRoadCargoTransporter.Number.ToInt();
            veiculo.Placa = vehicle.LicensePlate.ObterSomenteNumerosELetras().ToUpper();
            veiculo.Estado = new Dominio.Entidades.Estado { Sigla = vehicle.StateCode };

            if (!string.IsNullOrEmpty(vehicle.Manufacturer))
            {
                veiculo.Marca = repositorioMarcaVeiculo.BuscarPorDescricao(vehicle.Manufacturer, 0);

                if (veiculo.Marca == null)
                {
                    veiculo.Marca = new Dominio.Entidades.MarcaVeiculo
                    {
                        Descricao = vehicle.Manufacturer,
                        Status = "A",
                        TipoVeiculo = ehTracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque
                    };
                    repositorioMarcaVeiculo.Inserir(veiculo.Marca, auditado, descricaoAcao: "Adicionado via evento - DriverFreightContactCreate");
                }
            }

            if (!string.IsNullOrEmpty(vehicle.Model))
            {
                veiculo.Modelo = repositorioModeloVeiculo.BuscarPorDescricao(vehicle.Model, 0);

                if (veiculo.Modelo == null)
                {
                    veiculo.Modelo = new Dominio.Entidades.ModeloVeiculo
                    {
                        Descricao = vehicle.Model,
                        Status = "A",
                        MarcaVeiculo = veiculo.Marca,
                        NumeroEixo = vehicle.Axles != null ? vehicle.Axles.Count : 0,
                    };
                    repositorioModeloVeiculo.Inserir(veiculo.Modelo, auditado, descricaoAcao: "Adicionado via evento - DriverFreightContactCreate");
                }
            }

            return veiculo;
        }

        private Dominio.Entidades.Veiculo ObterNovoVeiculo(Vehicle vehicle, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool ehTracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.MarcaVeiculo repositorioMarcaVeiculo = new Repositorio.MarcaVeiculo(_unitOfWork);
            Repositorio.ModeloVeiculo repositorioModeloVeiculo = new Repositorio.ModeloVeiculo(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo();

            veiculo.TipoVeiculo = ehTracao ? "0" : "1";
            veiculo.Proprietario = cliente;
            veiculo.ModeloVeicularCarga = carga.ModeloVeicularCarga;
            veiculo.Empresa = carga.Empresa;
            veiculo.Ativo = true;

            veiculo.TipoProprietario = Dominio.Enumeradores.TipoProprietarioVeiculo.TACIndependente;
            veiculo.TipoRodado = "06";
            veiculo.Tipo = "T";
            veiculo.TipoCarroceria = "00";
            veiculo.Tara = 0;
            veiculo.CapacidadeKG = 0;

            veiculo.Cor = vehicle.Color;
            veiculo.AnoFabricacao = vehicle.ManufactureYear;
            veiculo.AnoModelo = vehicle.ModelYear;
            veiculo.Renavam = vehicle.NationalRegister;
            veiculo.Chassi = vehicle.VehicleIdentificationNumber;
            veiculo.RNTRC = vehicle.NationalRegistryOfRoadCargoTransporter.Number.ToInt();
            veiculo.Placa = vehicle.LicensePlate.ObterSomenteNumerosELetras().ToUpper();
            veiculo.Estado = new Dominio.Entidades.Estado { Sigla = vehicle.StateCode };

            if (!string.IsNullOrEmpty(vehicle.Manufacturer))
            {
                veiculo.Marca = repositorioMarcaVeiculo.BuscarPorDescricao(vehicle.Manufacturer, 0);

                if (veiculo.Marca == null)
                {
                    veiculo.Marca = new Dominio.Entidades.MarcaVeiculo
                    {
                        Descricao = vehicle.Manufacturer,
                        Status = "A",
                        TipoVeiculo = ehTracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque
                    };
                    repositorioMarcaVeiculo.Inserir(veiculo.Marca, auditado, descricaoAcao: "Adicionado via evento - DriverFreightContactCreate");
                }
            }

            if (!string.IsNullOrEmpty(vehicle.Model))
            {
                veiculo.Modelo = repositorioModeloVeiculo.BuscarPorDescricao(vehicle.Model, 0);

                if (veiculo.Modelo == null)
                {
                    veiculo.Modelo = new Dominio.Entidades.ModeloVeiculo
                    {
                        Descricao = vehicle.Model,
                        Status = "A",
                        MarcaVeiculo = veiculo.Marca,
                        NumeroEixo = vehicle.Axles != null ? vehicle.Axles.Count : 0,
                    };
                    repositorioModeloVeiculo.Inserir(veiculo.Modelo, auditado, descricaoAcao: "Adicionado via evento - DriverFreightContactCreate");
                }
            }

            return veiculo;

        }

        private Dominio.Entidades.Cliente ObterNovoCliente(Vehicle vehicle, Dominio.Entidades.Embarcador.Cargas.Carga carga, long _cpf, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);

            Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente
            {
                CPF_CNPJ = _cpf,
                Nome = vehicle.Owner.Name,
                Celular = vehicle.Owner?.telephone.ObterSomenteNumeros().ObterTelefoneFormatado(),
                DataCadastro = DateTime.Now,
                Tipo = vehicle.Owner.Document.Type == "CPF" ? "F" : "J",
                Atividade = new Dominio.Entidades.Atividade { Codigo = 4 }, //Prestadora d Serviço
                Ativo = true
            };

            if (vehicle.Owner.address != null)
            {
                cliente.Endereco = vehicle.Owner.address.street;
                cliente.Numero = vehicle.Owner.address.houseNumber;
                cliente.Bairro = vehicle.Owner.address.district;
                cliente.CEP = vehicle.Owner.address.postalCode;

                string cidade = vehicle.Owner.address.city.ObterSomenteNumerosELetrasComEspaco().ToUpper();
                string uf = vehicle.Owner.address.stateCode.ObterSomenteNumerosELetrasComEspaco().ToUpper();

                Dominio.Entidades.Localidade localidade = repositorioLocalidade.BuscarPorDescricaoEUF(cidade, uf);

                if (localidade == null)
                {
                    localidade = new Dominio.Entidades.Localidade
                    {
                        Descricao = cidade,
                        Estado = new Dominio.Entidades.Estado { Sigla = uf },
                        DataAtualizacao = DateTime.Now,
                        CodigoIBGE = 9999999 // Valor fictício para indicar localidade desconhecida
                    };
                    repositorioLocalidade.Inserir(localidade, auditado, descricaoAcao: "Adicionado localidade via evento - DriverFreightContactCreate");
                }

                cliente.Localidade = localidade;
            }
            else
            {
                cliente.Endereco = carga.Empresa.Endereco;
                cliente.Numero = carga.Empresa.Numero;
                cliente.Bairro = carga.Empresa.Bairro;
                cliente.CEP = carga.Empresa.CEP;
                cliente.Localidade = carga.Empresa.Localidade;
            }

            return cliente;
        }

        private ModalidadeTransportadoraPessoas ObterNovaModalidadeTransportadoraPessoas(ModalidadePessoas modalidade, Vehicle vehicle)
        {
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadora = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas();
            modalidadeTransportadora.RNTRC = vehicle.NationalRegistryOfRoadCargoTransporter.Number;
            modalidadeTransportadora.TipoTransportador = TipoProprietarioVeiculo.TACIndependente;
            modalidadeTransportadora.ModalidadePessoas = modalidade;

            return modalidadeTransportadora;
        }

        private ModalidadePessoas ObterNovaModalidadePessoas(Dominio.Entidades.Cliente cliente)
        {
            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
            modalidade.TipoModalidade = TipoModalidade.TransportadorTerceiro;
            modalidade.Cliente = cliente;

            return modalidade;
        }

        #endregion
    }
}
