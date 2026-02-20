using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Veiculo
{
    public sealed class VeiculoImportar
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Empresa _empresa;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracao;

        #endregion

        #region Construtores

        public VeiculoImportar(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, Dictionary<string, dynamic> dados, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            _dados = dados;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
            _empresa = empresa;
            _configuracao = configuracao;
        }

        #endregion

        #region Métodos Privados

        private int ObterAnoFabricacao()
        {
            if (_dados.TryGetValue("AnoFabricacao", out var anoFabricacao))
                return ((string)anoFabricacao).ToInt();

            return 0;
        }

        private int ObterAnoModelo()
        {
            if (_dados.TryGetValue("AnoModelo", out var anoModelo))
                return ((string)anoModelo).ToInt();

            return 0;
        }

        private int ObterCapacidadeKilogramas()
        {
            int capacidadeValor = 0;

            if (_dados.TryGetValue("CapacidadeKilogramas", out var capacidadeKilogramas))
                capacidadeValor = ((string)capacidadeKilogramas).ToInt();

            if (capacidadeValor <= 0)
                throw new ImportacaoException("Favor informar capacidade(KG) superior a 0, campo obrigatório!");

            return capacidadeValor;
        }

        private int ObterCapacidadeMetrosCubicos()
        {
            if (_dados.TryGetValue("CapacidadeMetrosCubicos", out var capacidadeMetrosCubicos))
                return ((string)capacidadeMetrosCubicos).ToInt();

            return 0;
        }

        private string ObterChassi()
        {
            var chassiRetornar = string.Empty;

            if (_dados.TryGetValue("Chassi", out var chassi))
                chassiRetornar = (string)chassi;

            return string.IsNullOrWhiteSpace(chassiRetornar) ? string.Empty : chassiRetornar.Trim();
        }

        private Dominio.Entidades.Estado ObterEstado(Dominio.Entidades.Empresa transportador)
        {
            if (_configuracao.Pais != TipoPais.Brasil)
            {
                if (transportador == null)
                    throw new ImportacaoException("O veículo não possui transportador associado.");

                return transportador.Localidade.Estado;
            }

            var ufBuscar = string.Empty;

            if (_dados.TryGetValue("Uf", out var uf))
                ufBuscar = ((string)uf);

            if (string.IsNullOrWhiteSpace(ufBuscar))
                throw new ImportacaoException("UF não informada");

            Repositorio.Estado repositorio = new Repositorio.Estado(_unitOfWork);
            Dominio.Entidades.Estado estado = repositorio.BuscarPorSigla(ufBuscar.Trim().ToUpper());

            if (estado == null)
                throw new ImportacaoException("Estado não encontrado");

            return estado;
        }

        private int ObterKilometragemAtual()
        {
            if (_dados.TryGetValue("KilometragemAtual", out var kilometragemAtual))
                return ((string)kilometragemAtual).ToInt();

            return 0;
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterModeloVeicularCarga()
        {
            var codigoIntegracaoBuscar = string.Empty;

            if (_dados.TryGetValue("ModeloVeicular", out var codigoIntegracao))
                codigoIntegracaoBuscar = (string)codigoIntegracao;

            if (string.IsNullOrWhiteSpace(codigoIntegracaoBuscar))
                throw new ImportacaoException("Modelo Veicular não informado");

            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorio = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorio.buscarPorCodigoIntegracao(codigoIntegracaoBuscar.Trim());

            if (modeloVeicularCarga == null)
                modeloVeicularCarga = repositorio.buscarPorDescricao(codigoIntegracaoBuscar.Trim());

            if (modeloVeicularCarga == null)
                throw new ImportacaoException("Modelo Veicular não encontrado");

            return modeloVeicularCarga;
        }

        private Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista ObterMotorista(Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Repositorio.Usuario repositorio = new Repositorio.Usuario(_unitOfWork);

            string cpfMotoristaBuscar = string.Empty;
            if (_dados.TryGetValue("CpfMotorista", out var cpfMotorista))
                cpfMotoristaBuscar = (string)cpfMotorista;

            if (string.IsNullOrWhiteSpace(cpfMotoristaBuscar))
            {
                if (veiculo.Codigo > 0)
                    repVeiculoMotorista.DeletarMotoristaPrincipal(veiculo.Codigo);
                return null;
            }

            cpfMotoristaBuscar = Utilidades.String.OnlyNumbers(cpfMotoristaBuscar).PadLeft(11, '0');

            Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = veiculo.Codigo > 0 ? repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo) : null;
            if (veiculoMotorista != null && veiculoMotorista.Motorista.CPF == cpfMotoristaBuscar)
                return null;

            Dominio.Entidades.Usuario motorista = repositorio.BuscarMotoristaPorCPF(cpfMotoristaBuscar);
            if (motorista != null)
            {
                if (motorista.Empresa?.Codigo != veiculo.Empresa?.Codigo && !motorista.Empresas.Contains(veiculo.Empresa))
                    throw new ImportacaoException("O motorista informado não pertence a mesma empresa do veículo.");

                return PreencherVeiculoMotorista(veiculoMotorista, veiculo, motorista);
            }

            var nomeMotoristaVeiculo = string.Empty;
            if (_dados.TryGetValue("NomeMotorista", out var nomeMotorista))
                nomeMotoristaVeiculo = (string)nomeMotorista;

            if (!string.IsNullOrWhiteSpace(nomeMotoristaVeiculo))
                nomeMotoristaVeiculo = nomeMotoristaVeiculo.Trim();

            if (!string.IsNullOrWhiteSpace(nomeMotoristaVeiculo))
            {
                motorista = new Dominio.Entidades.Usuario();
                motorista.CPF = cpfMotoristaBuscar;
                motorista.Nome = nomeMotoristaVeiculo;
                motorista.Localidade = veiculo.Empresa?.Localidade;
                motorista.Empresa = veiculo.Empresa;
                motorista.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };
                motorista.Tipo = "M";
                motorista.Status = "A";
                motorista.TipoPessoa = "F";
                motorista.CentroResultado = veiculo.CentroResultado;
                repositorio.Inserir(motorista);

                return PreencherVeiculoMotorista(veiculoMotorista, veiculo, motorista);
            }

            return null;
        }

        private Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista PreencherVeiculoMotorista(Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

            if (veiculoMotorista == null)
            {
                veiculoMotorista = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                {
                    CPF = motorista.CPF,
                    Nome = motorista.Nome,
                    Principal = true,
                    Veiculo = veiculo,
                    Motorista = motorista,
                };

                if (veiculo.Codigo == 0)
                    return veiculoMotorista;
                else
                    repVeiculoMotorista.Inserir(veiculoMotorista);
            }
            else
            {
                veiculoMotorista.CPF = motorista.CPF;
                veiculoMotorista.Nome = motorista.Nome;
                veiculoMotorista.Motorista = motorista;
                repVeiculoMotorista.Atualizar(veiculoMotorista);
            }

            return null;
        }

        private string ObterObservacao()
        {
            var observacaoRetornar = string.Empty;

            if (_dados.TryGetValue("Observacao", out var observacao))
                observacaoRetornar = (string)observacao;

            return string.IsNullOrWhiteSpace(observacaoRetornar) ? string.Empty : observacaoRetornar.Trim();
        }

        private Dominio.Entidades.Cliente ObterProprietario()
        {
            var cnpjCpfProprietarioBuscar = 0d;

            if (_dados.TryGetValue("CnpjCpfProprietario", out var cnpjCpfProprietario))
                cnpjCpfProprietarioBuscar = ((string)cnpjCpfProprietario).ToDouble();

            if (cnpjCpfProprietarioBuscar > 0d)
            {
                Repositorio.Cliente repositorio = new Repositorio.Cliente(_unitOfWork);
                Dominio.Entidades.Cliente proprietario = repositorio.BuscarPorCPFCNPJ(cnpjCpfProprietarioBuscar);

                if (proprietario != null)
                    return proprietario;
            }

            return null;
        }

        private string ObterRenavam()
        {
            var renavamRetornar = string.Empty;

            if (_dados.TryGetValue("Renavam", out var renavam))
                renavamRetornar = (string)renavam;

            if (_configuracao.Pais == TipoPais.Brasil)
            {
                if (string.IsNullOrWhiteSpace(renavamRetornar))
                    throw new ImportacaoException("Renavam não informado");

                renavamRetornar = renavamRetornar.Trim();

                if (_configuracao.ValidarRENAVAMVeiculo && !Utilidades.Validate.ValidarRENAVAM(renavamRetornar))
                    throw new ImportacaoException($"O Renavam informado ({renavamRetornar}) é inválido");

                if (!_configuracao.ValidarRENAVAMVeiculo && renavamRetornar.Length != 11)
                    throw new ImportacaoException("Renavam deve possuir 11 dígitos");
            }

            return renavamRetornar;
        }

        private int ObterRntrcProprietario()
        {
            if (_dados.TryGetValue("RntrcProprietario", out var rntrcProprietario))
                return ((string)rntrcProprietario).ToInt();

            return 0;
        }

        private int ObterTara()
        {
            int taraValor = 0;
            if (_dados.TryGetValue("Tara", out var tara))
                taraValor = Convert.ToInt32(tara.ToString().Trim());

            if (taraValor <= 0)
                throw new ImportacaoException("Favor informar tara superior a 0, campo obrigatório!");

            return taraValor;
        }

        private string ObterTipo()
        {
            var tipoRetornar = string.Empty;

            if (_dados.TryGetValue("Tipo", out var tipo))
                tipoRetornar = !string.IsNullOrWhiteSpace(tipo)
                               ? tipo.ToString().Substring(0, 1)
                               : "";

            if (string.IsNullOrWhiteSpace(tipoRetornar))
                return "P";

            return tipoRetornar.Trim();
        }

        private string ObterTipoCarroceria()
        {
            var tipoCarroceriaRetornar = string.Empty;
            char complementoEnumerador = '0';
            var ajudaRetorno = "Tipo de carroceria inválida, favor utilizar numerador! Exemplo: 00 (Não aplicado), 01 (Aberta), 02 (Fechada/Bau), 03 (Granel), 04 (Porta Container), 05 (Sider)";

            if (_dados.TryGetValue("TipoCarroceria", out var tipoCarroceria))
                tipoCarroceriaRetornar = tipoCarroceria.ToString().Trim();

            if (!tipoCarroceriaRetornar.IsSomenteNumeros())
                throw new ImportacaoException(ajudaRetorno);

            if (!ValidarTipoCarroceria(tipoCarroceriaRetornar.ObterSomenteNumeros()))
                throw new ImportacaoException(ajudaRetorno);

            return tipoCarroceriaRetornar.Trim().PadLeft(2, complementoEnumerador);
        }

        private bool ValidarTipoCarroceria(string eNumerador)
        {
            switch (eNumerador.ToInt())
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    return true;
                default:
                    return false;
            }
        }

        private Dominio.Enumeradores.TipoProprietarioVeiculo ObterTipoProprietario()
        {
            if (_dados.TryGetValue("TipoProprietario", out var tipoProprietario))
                return ((string)tipoProprietario).ToEnum(Dominio.Enumeradores.TipoProprietarioVeiculo.Outros);

            return Dominio.Enumeradores.TipoProprietarioVeiculo.Outros;
        }

        private string ObterTipoRodado()
        {
            var tipoRodadoRetornar = string.Empty;
            char complementoEnumerador = '0';
            var ajudaRetorno = "Tipo rodado inválido, favor utilizar numerador! Exemplo: 00 (Não aplicado), 01 (Truck), 02 (Toco), 03 (Cavalo), 04 (Van), 05 (Utilitário), 06 (Outros)";

            if (_dados.TryGetValue("TipoRodado", out var tipoRodado))
                tipoRodadoRetornar = tipoRodado.ToString().Trim();

            if (!tipoRodadoRetornar.IsSomenteNumeros())
                throw new ImportacaoException(ajudaRetorno);

            if (!ValidarTipoRodado(tipoRodadoRetornar.ObterSomenteNumeros()))
                throw new ImportacaoException(ajudaRetorno);

            return tipoRodadoRetornar.Trim().PadLeft(2, complementoEnumerador);
        }

        private bool ValidarTipoRodado(string eNumerador)
        {
            switch (eNumerador.ToInt())
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    return true;

                default:
                    return false;
            }
        }

        private string ObterTipoVeiculo()
        {
            var tipoVeiculoRetornar = string.Empty;
            var ajudaRetorno = "Tipo de veículo inválido! Favor utilizar numerador. Ex: 0 -Tração, 1 -Reboque";

            if (_dados.TryGetValue("TipoVeiculo", out var tipoVeiculo))
                tipoVeiculoRetornar = (string)tipoVeiculo;

            if (!tipoVeiculoRetornar.IsSomenteNumeros())
                throw new ImportacaoException(ajudaRetorno);

            if (tipoVeiculoRetornar.Length != 1)
                throw new ImportacaoException(ajudaRetorno);

            if (!ValidarTipoVeiculo(tipoVeiculoRetornar.ObterSomenteNumeros()))
                throw new ImportacaoException(ajudaRetorno);

            return tipoVeiculoRetornar.Trim();
        }

        private bool ValidarTipoVeiculo(string eNumerador)
        {
            switch (eNumerador.ToInt())
            {
                case 0:
                case 1:
                    return true;
                default:
                    return false;
            }
        }

        private Dominio.Entidades.Empresa ObterTransportadora()
        {
            var cnpjTransportadoraBuscar = string.Empty;

            if (_dados.TryGetValue("CnpjTransportadora", out var cnpjTransportadora))
                cnpjTransportadoraBuscar = (string)cnpjTransportadora;

            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                return this._empresa;

            if (string.IsNullOrWhiteSpace(cnpjTransportadoraBuscar))
            {
                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    throw new ImportacaoException("CNPJ da transportadora não informado");

                return null;
            }

            Repositorio.Empresa repositorio = new Repositorio.Empresa(_unitOfWork);
            Dominio.Entidades.Empresa tranportadora = repositorio.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjTransportadoraBuscar));

            if (tranportadora == null)
                throw new ImportacaoException("Transportadora não encontrada");

            return tranportadora;
        }

        private Dominio.Entidades.Veiculo ObterVeiculo(Dominio.Entidades.Empresa transportador)
        {
            var placaBuscar = string.Empty;

            if (_dados.TryGetValue("Placa", out var placa))
                placaBuscar = (string)placa;

            if (string.IsNullOrWhiteSpace(placaBuscar))
                throw new ImportacaoException("Placa não informada");

            placaBuscar = placaBuscar.Replace("-", "").Trim().ToUpper();

            Repositorio.Veiculo repositorio = new Repositorio.Veiculo(_unitOfWork);
            Dominio.Entidades.Veiculo veiculo = _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? repositorio.BuscarPorPlaca(placaBuscar) : repositorio.BuscarPorPlaca(transportador.Codigo, placaBuscar);

            if (veiculo == null)
            {
                Servicos.Veiculo serVeiculo = new Servicos.Veiculo(_unitOfWork);

                if (!serVeiculo.ValidarPlaca(placaBuscar, _configuracao))//Só valida quantidade de dígitos
                    throw new ImportacaoException($"A placa informada é inválida");

                if (_configuracao.Pais == TipoPais.Brasil && _configuracao.ValidarPlacaVeiculo && !Utilidades.Validate.ValidarPlaca(placaBuscar))//Valida padrão antigo e MERCOSUL
                    throw new ImportacaoException($"A placa informada ({placaBuscar}) é inválida");

                veiculo = new Dominio.Entidades.Veiculo();
                veiculo.Placa = placaBuscar;
            }
            else
                veiculo.Initialize();

            return veiculo;
        }

        private Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador ObterTecnologiaRastreador()
        {
            string tecnologiaBuscar = string.Empty;
            if (_dados.TryGetValue("TecnologiaRastreador", out var tecnologia))
                tecnologiaBuscar = (string)tecnologia;

            Repositorio.Embarcador.Veiculos.TecnologiaRastreador repTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(_unitOfWork);
            return (!string.IsNullOrEmpty(tecnologiaBuscar)) ? repTecnologiaRastreador.ConsultarPorDescricao(tecnologiaBuscar) : null;
        }

        private Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador ObterTipoComunicacaoRastreador()
        {
            string tipoComunicacaoBuscar = string.Empty;
            if (_dados.TryGetValue("TipoComunicacaoRastreador", out var tipoComunicacao))
                tipoComunicacaoBuscar = (string)tipoComunicacao;

            Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador repTipoComunicacaoRastreador = new Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador(_unitOfWork);
            return (!string.IsNullOrEmpty(tipoComunicacaoBuscar)) ? repTipoComunicacaoRastreador.ConsultarPorDescricao(tipoComunicacaoBuscar) : null;
        }

        private string ObterNumeroEquipamentoRastreador()
        {
            string tipoComunicacaoBuscar = string.Empty;
            if (_dados.TryGetValue("NumeroEquipamentoRastreador", out var numero))
                tipoComunicacaoBuscar = (string)numero;

            return tipoComunicacaoBuscar;
        }

        private bool ObterPossuiRastreador(Dominio.Entidades.Veiculo veiculo)
        {
            bool possuiRastreador = veiculo.TecnologiaRastreador != null && veiculo.TipoComunicacaoRastreador != null && !string.IsNullOrEmpty(veiculo.NumeroEquipamentoRastreador);
            if (_configuracao.ObrigatorioCadastrarRastreadorNosVeiculos && !possuiRastreador)
                throw new ImportacaoException("Deve ser informado os dados de rastreamento do veículo.");

            return possuiRastreador;
        }

        private Dominio.Entidades.Embarcador.Veiculos.CorVeiculo ObterCor()
        {
            var corBuscar = string.Empty;

            if (_dados.TryGetValue("Cor", out var cor))
                corBuscar = (string)cor;

            Repositorio.Embarcador.Veiculos.CorVeiculo repCorVeiculo = new Repositorio.Embarcador.Veiculos.CorVeiculo(_unitOfWork);
            return (!string.IsNullOrEmpty(corBuscar)) ? repCorVeiculo.ConsultarPorDescricao(corBuscar) : null;
        }

        private string ObterTipoCombustivel()
        {
            var tipoCombustivelRetornar = string.Empty;
            string[] tiposExistentes = { "G", "D", "E", "I", "N", "S", "O" };

            if (_dados.TryGetValue("TipoCombustivel", out var tipoCombustivel))
                tipoCombustivelRetornar = (string)tipoCombustivel;

            if (string.IsNullOrWhiteSpace(tipoCombustivelRetornar))
                return "O";
            else
                tipoCombustivelRetornar = tipoCombustivelRetornar.Trim();

            if (tipoCombustivelRetornar.Length != 1 || !tiposExistentes.Contains(tipoCombustivelRetornar))
                throw new ImportacaoException("Tipo Combustível não está de acordo com o que o sistema espera (G, D, E, I, N, S, O).");

            return tipoCombustivelRetornar;
        }

        private Dominio.Entidades.ModeloVeiculo ObterModeloVeiculo()
        {
            var codigoIntegracaoBuscar = string.Empty;

            if (_dados.TryGetValue("ModeloVeiculo", out var codigoIntegracao))
                codigoIntegracaoBuscar = (string)codigoIntegracao;

            Dominio.Entidades.ModeloVeiculo modeloVeiculo = null;
            if (!string.IsNullOrWhiteSpace(codigoIntegracaoBuscar))
            {
                Repositorio.ModeloVeiculo repositorio = new Repositorio.ModeloVeiculo(_unitOfWork);
                modeloVeiculo = repositorio.BuscarPorCodigoIntegracao(codigoIntegracaoBuscar.Trim(), _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? _empresa.Codigo : 0);

                if (modeloVeiculo == null)
                    modeloVeiculo = repositorio.BuscarPorDescricao(codigoIntegracaoBuscar.Trim(), _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? _empresa.Codigo : 0);
            }

            return modeloVeiculo;
        }

        private Dominio.Entidades.MarcaVeiculo ObterMarcaVeiculo()
        {
            var codigoIntegracaoBuscar = string.Empty;

            if (_dados.TryGetValue("MarcaVeiculo", out var codigoIntegracao))
                codigoIntegracaoBuscar = (string)codigoIntegracao;

            Dominio.Entidades.MarcaVeiculo marcaVeiculo = null;
            if (!string.IsNullOrWhiteSpace(codigoIntegracaoBuscar))
            {
                Repositorio.MarcaVeiculo repositorio = new Repositorio.MarcaVeiculo(_unitOfWork);
                marcaVeiculo = repositorio.BuscarPorCodigoIntegracao(codigoIntegracaoBuscar.Trim(), _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? _empresa.Codigo : 0);

                if (marcaVeiculo == null)
                    marcaVeiculo = repositorio.BuscarPorDescricao(codigoIntegracaoBuscar.Trim(), _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? _empresa.Codigo : 0);
            }

            return marcaVeiculo;
        }

        private bool ObterAtivo()
        {
            bool ativoRetornar = true;

            string ativoConversao = string.Empty;
            if (_dados.TryGetValue("Ativo", out var ativo))
                ativoConversao = (string)ativo;

            if (string.IsNullOrWhiteSpace(ativoConversao))
                return ativoRetornar;

            if (ativoConversao.ToLower().Equals("inativo"))
                return false;

            return ativoRetornar;
        }

        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado ObterCentroResultado()
        {
            var codigoIntegracaoBuscar = string.Empty;

            if (_dados.TryGetValue("CentroResultado", out var codigoIntegracao))
                codigoIntegracaoBuscar = (string)codigoIntegracao;

            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = null;
            if (!string.IsNullOrWhiteSpace(codigoIntegracaoBuscar))
            {
                Repositorio.Embarcador.Financeiro.CentroResultado repositorio = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
                centroResultado = repositorio.BuscarPorCodigo(codigoIntegracaoBuscar.Trim().ToInt());

                if (centroResultado == null)
                    centroResultado = repositorio.BuscarPorDescricao(codigoIntegracaoBuscar.Trim());
            }

            return centroResultado;
        }

        private string ObterCIOT()
        {
            var ciotRetornar = string.Empty;

            if (_dados.TryGetValue("CIOT", out var ciot))
                ciotRetornar = (string)ciot;

            return !string.IsNullOrWhiteSpace(ciotRetornar) ? ciotRetornar.Trim() : string.Empty;
        }

        private int ObterCapacidadeTanque()
        {

            if (_dados.TryGetValue("CapacidadeTanque", out var capicadeTanque))
                return ((string)capicadeTanque).ToInt();

            return 0;
        }

        private int ObterCapacidadeTanqueMax()
        {

            if (_dados.TryGetValue("CapacidadeTanqueMax", out var capicadeTanqueMax))
                return ((string)capicadeTanqueMax).ToInt();

            return 0;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFrota ObterTipoFrota()
        {
            if (_dados.TryGetValue("TipoFrota", out var tipoFrota))
                return ((string)tipoFrota).ToEnum(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFrota.NaoDefinido);

            return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFrota.NaoDefinido;
        }

        private string ObterCodigoIntegracao()
        {
            var codIntegracao = string.Empty;

            if (_dados.TryGetValue("CodigoIntegracao", out var codInt))
                codIntegracao = (string)codInt;

            return !string.IsNullOrWhiteSpace(codIntegracao) ? codIntegracao.Trim() : string.Empty;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Veiculo ObterVeiculoImportar()
        {
            Dominio.Entidades.Empresa transportador = ObterTransportadora();
            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(transportador);

            veiculo.TipoFrota = ObterTipoFrota();
            veiculo.AnoFabricacao = ObterAnoFabricacao();
            veiculo.AnoModelo = ObterAnoModelo();
            veiculo.Ativo = ObterAtivo();
            veiculo.PendenteIntegracaoEmbarcador = true;
            veiculo.CapacidadeKG = ObterCapacidadeKilogramas();
            veiculo.CapacidadeM3 = ObterCapacidadeMetrosCubicos();
            veiculo.Chassi = ObterChassi();
            veiculo.Empresa = transportador;
            veiculo.Estado = ObterEstado(veiculo.Empresa);
            veiculo.KilometragemAtual = ObterKilometragemAtual();
            veiculo.ModeloVeicularCarga = ObterModeloVeicularCarga();
            veiculo.Observacao = ObterObservacao();
            veiculo.Proprietario = ObterProprietario();
            veiculo.Renavam = ObterRenavam();
            veiculo.RNTRC = ObterRntrcProprietario();
            veiculo.Tara = ObterTara();
            veiculo.Tipo = ObterTipo();
            veiculo.TipoCarroceria = ObterTipoCarroceria();
            veiculo.TipoProprietario = ObterTipoProprietario();
            veiculo.TipoRodado = ObterTipoRodado();
            veiculo.TipoVeiculo = ObterTipoVeiculo();
            veiculo.TecnologiaRastreador = ObterTecnologiaRastreador();
            veiculo.TipoComunicacaoRastreador = ObterTipoComunicacaoRastreador();
            veiculo.NumeroEquipamentoRastreador = ObterNumeroEquipamentoRastreador();
            veiculo.PossuiRastreador = ObterPossuiRastreador(veiculo);
            veiculo.CorVeiculo = ObterCor();
            veiculo.TipoCombustivel = ObterTipoCombustivel();
            veiculo.Modelo = ObterModeloVeiculo();
            veiculo.Marca = ObterMarcaVeiculo();
            veiculo.CentroResultado = ObterCentroResultado();
            veiculo.CIOT = ObterCIOT();
            veiculo.CapacidadeTanque = ObterCapacidadeTanque();
            veiculo.CapacidadeMaximaTanque = ObterCapacidadeTanqueMax();
            veiculo.CodigoIntegracao = ObterCodigoIntegracao();

            Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = ObterMotorista(veiculo);
            if (veiculoMotorista != null)
            {
                if (veiculo.Codigo.Equals(0) || veiculo.Motoristas == null)
                    veiculo.Motoristas = new List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();

                veiculo.Motoristas.Add(veiculoMotorista);
            }

            if (veiculo.Tipo == "T" && (veiculo.RNTRC == 0))
                throw new ServicoException(Localization.Resources.Veiculos.Veiculo.InformarTerceiroObrigatarioInformarRNTRC);

            return veiculo;
        }

        #endregion
    }
}
