using System;
using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;

namespace Servicos.Embarcador.Transportadores
{
    public sealed class ImportacaoMotorista
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Empresa _empresa;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly AdminMultisoftware.Repositorio.UnitOfWork _unitOfWorkAdmin;

        #endregion

        #region Construtores

        public ImportacaoMotorista(Dictionary<string, dynamic> dados, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador,
            Dominio.Entidades.Empresa empresa, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            _dados = dados;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
            _empresa = empresa;
            _unitOfWorkAdmin = unitOfWorkAdmin;
            _configuracaoEmbarcador = configuracaoEmbarcador;

            if (!_unitOfWorkAdmin.IsActiveTransaction())
            {
                _unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(unitOfWorkAdmin.StringConexao);
                _unitOfWorkAdmin.Start();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterNome()
        {
            var nomeRetornar = string.Empty;
            _dados.TryGetValue("Nome", out dynamic nome);

            if (nome != null)
                nomeRetornar = ((string)nome).Trim();

            if (string.IsNullOrWhiteSpace(nomeRetornar))
                throw new ImportacaoException("Nome do motorista não preenchido");

            return nomeRetornar;
        }

        private string ObterNumeroRegistroHabilitacao()
        {
            var nomeRetornar = string.Empty;

            if (_dados.TryGetValue("NumeroRegistroHabilitacao", out var nome))
                nomeRetornar = ((string)nome).Trim();

            return nomeRetornar;
        }

        private DateTime? ObterDataValidadeSeguradora()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataValidadeLiberacaoSeguradora", out var dataValidadeSeguradora))
                data = ((string)dataValidadeSeguradora).ToNullableDateTime();

            return data;
        }

        private string ObterCNH()
        {
            var CNHRetornar = string.Empty;

            if (_dados.TryGetValue("CNH", out var cnh))
                CNHRetornar = (string)cnh;

            return string.IsNullOrWhiteSpace(CNHRetornar) ? string.Empty : CNHRetornar.Trim();
        }

        private DateTime? ObterValidadeCNH()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("ValidadeCNH", out var dataValidadeCNH))
                data = ((string)dataValidadeCNH).ToNullableDateTime();

            return data;
        }

        private string ObterTelefone()
        {
            var telefoneRetornar = string.Empty;

            if (_dados.TryGetValue("Telefone", out var telefone))
                telefoneRetornar = (string)telefone;

            return string.IsNullOrWhiteSpace(telefoneRetornar) ? string.Empty : Utilidades.String.OnlyNumbers(telefoneRetornar).Trim();
        }

        private DateTime? ObterDataNascimento()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataNascimento", out var dataNascimento))
                data = ((string)dataNascimento).ToNullableDateTime();

            return data;
        }

        private string ObterPISPASEP()
        {
            var pispasepRetornar = string.Empty;

            if (_dados.TryGetValue("PISPASEP", out var pispasep))
            {
                pispasepRetornar = Utilidades.String.OnlyNumbers(((string)pispasep) ?? string.Empty);
                if (!Utilidades.Validate.ValidarPISPASEP(pispasepRetornar))
                    throw new ImportacaoException("PIS/PASEP informado é inválido");
            }

            return string.IsNullOrWhiteSpace(pispasepRetornar) ? string.Empty : pispasepRetornar.Trim();
        }

        private Dominio.Entidades.Empresa ObterTransportadora()
        {
            var cnpjTransportadoraBuscar = string.Empty;

            if (_dados.TryGetValue("CnpjTransportadora", out var cnpjTransportadora))
                cnpjTransportadoraBuscar = (string)cnpjTransportadora;

            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                return _empresa;

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

        private string ObterRG()
        {
            var rgRetornar = string.Empty;

            if (_dados.TryGetValue("RG", out var rg))
                rgRetornar = ((string)rg).Trim();

            return rgRetornar;
        }

        private DateTime? ObterDataEmissaoRG()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataEmissaoRG", out var dataEmissaoRG))
                data = ((string)dataEmissaoRG).ToNullableDateTime();

            return data;
        }

        private OrgaoEmissorRG? ObterEmissorRG()
        {
            OrgaoEmissorRG? orgaoEmissorRGRetornar = null;

            if (_dados.TryGetValue("EmissorRG", out var rg))
                orgaoEmissorRGRetornar = ((string)rg).ToNullableEnum<OrgaoEmissorRG>();

            return orgaoEmissorRGRetornar;
        }

        private Dominio.Entidades.Estado ObterEstadoRG()
        {
            var estadoRGBuscar = string.Empty;

            if (_dados.TryGetValue("EstadoRG", out var estadoRG))
                estadoRGBuscar = (string)estadoRG;

            if (string.IsNullOrWhiteSpace(estadoRGBuscar))
                return null;

            var repositorio = new Repositorio.Estado(_unitOfWork);
            var estado = repositorio.BuscarPorSigla(estadoRGBuscar);

            if (estado == null)
                throw new ImportacaoException("Estado do RG não encontrado");

            return estado;
        }

        private string ObterCategoriaHabilitacao()
        {
            var categoriaRetornar = string.Empty;

            if (_dados.TryGetValue("CategoriaHabilitacao", out var categoria))
                categoriaRetornar = ((string)categoria).Trim();

            return categoriaRetornar;
        }

        private DateTime? ObterDataHabilitacao()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataEmissaoCNH", out var dataEmissaoCNH))
                data = ((string)dataEmissaoCNH).ToNullableDateTime();

            return data;
        }

        private DateTime? ObterDataPrimeiraHabilitacao()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataPrimeiraHabilitacao", out var dataPrimeiraHabilitacao))
                data = ((string)dataPrimeiraHabilitacao).ToNullableDateTime();

            return data;
        }

        private string ObterCelular()
        {
            var celularRetornar = string.Empty;

            if (_dados.TryGetValue("Celular", out var celular))
                celularRetornar = (string)celular;

            return string.IsNullOrWhiteSpace(celularRetornar) ? string.Empty : Utilidades.String.OnlyNumbers(celularRetornar).Trim();
        }

        private string ObterCEP()
        {
            var cepRetornar = string.Empty;

            if (_dados.TryGetValue("CEP", out var cep))
                cepRetornar = ((string)cep).Trim();

            if (!string.IsNullOrWhiteSpace(cepRetornar))
                if (cepRetornar.Length != 8)
                    throw new ImportacaoException("CEP é inválido, o mesmo deve possuir 8 dígitos");

            return cepRetornar;
        }

        private DateTime? ObterDataAdmissao()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataAdmissao", out var dataAdmissao))
                data = ((string)dataAdmissao).ToNullableDateTime();

            return data;
        }

        private string ObterNumeroCTPS()
        {
            var numeroCTPSRetornar = string.Empty;

            if (_dados.TryGetValue("NumeroCTPS", out var numeroCTPS))
                numeroCTPSRetornar = ((string)numeroCTPS).Trim();

            return numeroCTPSRetornar;
        }

        private string ObterSerieCTPS()
        {
            var serieCTPSRetornar = string.Empty;

            if (_dados.TryGetValue("SerieCTPS", out var serieCTPS))
                serieCTPSRetornar = ((string)serieCTPS).Trim();

            return serieCTPSRetornar;
        }

        private string ObterCargo()
        {
            var cargoRetornar = string.Empty;

            if (_dados.TryGetValue("Cargo", out var cargo))
                cargoRetornar = ((string)cargo).Trim();

            return cargoRetornar;
        }

        private string ObterCBO()
        {
            var cboRetornar = string.Empty;

            if (_dados.TryGetValue("CBO", out var cbo))
                cboRetornar = ((string)cbo).Trim();

            return cboRetornar;
        }

        private string ObterNumeroMatricula()
        {
            var numeroMatriculaRetornar = string.Empty;

            if (_dados.TryGetValue("NumeroMatricula", out var numeroMatricula))
                numeroMatriculaRetornar = ((string)numeroMatricula).Trim();

            return numeroMatriculaRetornar;
        }

        private string ObterNumeroEndereco()
        {
            var numeroEnderecoRetornar = string.Empty;

            if (_dados.TryGetValue("NumeroEndereco", out var numeroEndereco))
                numeroEnderecoRetornar = (string)numeroEndereco;

            return !string.IsNullOrWhiteSpace(numeroEnderecoRetornar) ? numeroEnderecoRetornar.Trim() : string.Empty;
        }

        private string ObterCidade()
        {
            var cidadeRetornar = string.Empty;

            if (_dados.TryGetValue("Cidade", out var numeroEndereco))
                cidadeRetornar = (string)numeroEndereco;

            return !string.IsNullOrWhiteSpace(cidadeRetornar) ? cidadeRetornar.Trim() : string.Empty;
        }

        private string ObterUF()
        {
            var UFRetornar = string.Empty;

            if (_dados.TryGetValue("UF", out var numeroEndereco))
                UFRetornar = (string)numeroEndereco;

            return !string.IsNullOrWhiteSpace(UFRetornar) ? UFRetornar.Trim() : string.Empty;
        }

        private string ObterComplemento()
        {
            var complementoRetornar = string.Empty;

            if (_dados.TryGetValue("Complemento", out var complemento))
                complementoRetornar = (string)complemento;

            return !string.IsNullOrWhiteSpace(complementoRetornar) ? complementoRetornar.Trim() : string.Empty;
        }

        private string ObterEndereco()
        {
            var enderecoRetornar = string.Empty;

            if (_dados.TryGetValue("Endereco", out var endereco))
                enderecoRetornar = (string)endereco;

            return !string.IsNullOrWhiteSpace(enderecoRetornar) ? enderecoRetornar.Trim() : string.Empty;
        }

        private string ObterBairro()
        {
            var bairroRetornar = string.Empty;

            if (_dados.TryGetValue("Bairro", out var bairro))
                bairroRetornar = (string)bairro;

            return !string.IsNullOrWhiteSpace(bairroRetornar) ? bairroRetornar.Trim() : string.Empty;
        }

        private string ObterNumeroCartao()
        {
            var numeroCartaoRetornar = string.Empty;

            if (_dados.TryGetValue("NumeroCartao", out var numerocartao))
                numeroCartaoRetornar = (string)numerocartao;

            return !string.IsNullOrWhiteSpace(numeroCartaoRetornar) ? numeroCartaoRetornar.Trim() : string.Empty;
        }

        private string ObterAgencia()
        {
            var agenciaRetornar = string.Empty;

            if (_dados.TryGetValue("Agencia", out var agencia))
                agenciaRetornar = (string)agencia.Trim();

            return !string.IsNullOrWhiteSpace(agenciaRetornar) ? agenciaRetornar.Trim() : string.Empty;
        }

        private Dominio.Entidades.Banco ObterBanco()
        {
            int bancoRetornar = 0;

            if (_dados.TryGetValue("Banco", out var bancoConverter))
                bancoRetornar = ((string)bancoConverter).ToInt();

            if (bancoRetornar == 0)
                return null;

            Repositorio.Banco repositorio = new Repositorio.Banco(_unitOfWork);
            Dominio.Entidades.Banco banco = repositorio.BuscarPorNumero(bancoRetornar);

            if (banco == null)
                throw new ImportacaoException("Banco não encontrado");

            return banco;
        }

        private Dominio.Entidades.Localidade ObterIBGE()
        {
            int ibgeRetornar = 0;

            if (_dados.TryGetValue("Localidade", out var ibgeConverter))
                ibgeRetornar = ((string)ibgeConverter).ToInt();

            if (ibgeRetornar == 0)
                return null;

            Repositorio.Localidade repositorio = new Repositorio.Localidade(_unitOfWork);
            Dominio.Entidades.Localidade localidade = repositorio.BuscarPorCodigoIBGE(ibgeRetornar);

            if (localidade == null)
                throw new ImportacaoException("Localidade não encontrada!");

            return localidade;
        }

        private Dominio.Entidades.Localidade ObterLocalidade()
        {
            int ibgeRetornar = 0;



            if (_dados.TryGetValue("Localidade", out var ibgeConverter))
                ibgeRetornar = ((string)ibgeConverter).ToInt();

            if (ibgeRetornar == 0)
                return null;

            Repositorio.Localidade repositorio = new Repositorio.Localidade(_unitOfWork);
            Dominio.Entidades.Localidade localidade = repositorio.BuscarPorCodigoIBGE(ibgeRetornar);

            if (localidade == null)
                throw new ImportacaoException("Localidade não encontrada!");

            return localidade;
        }

        private Dominio.Entidades.Cliente ObterPessoaAgregado()
        {
            double clienteRetornar = 0;

            if (_dados.TryGetValue("PessoaAgregado", out var pessoaConverter))
                clienteRetornar = ((string)pessoaConverter).ObterSomenteNumeros().ToDouble();

            if (clienteRetornar == 0)
                return null;

            Repositorio.Cliente repositorio = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente cliente = repositorio.BuscarPorCPFCNPJ(clienteRetornar);

            return cliente;
        }

        private string ObterNumeroConta()
        {
            var numeroContaRetornar = string.Empty;

            if (_dados.TryGetValue("NumeroConta", out var numeroconta))
                numeroContaRetornar = (string)numeroconta;

            return !string.IsNullOrWhiteSpace(numeroContaRetornar) ? numeroContaRetornar.Trim() : string.Empty;
        }

        private TipoMotorista ObterTipoMotorista()
        {
            string tipoMotoristaRetornar = string.Empty;

            if (_dados.TryGetValue("TipoMotorista", out var formaTituloConverte))
                tipoMotoristaRetornar = (string)formaTituloConverte;
            tipoMotoristaRetornar = !string.IsNullOrWhiteSpace(tipoMotoristaRetornar) ? tipoMotoristaRetornar.Trim() : string.Empty;

            return TipoMotoristaHelper.ObterTipoMotorista(tipoMotoristaRetornar);
        }

        private Dominio.Entidades.Usuario ObterMotorista()
        {
            var CPFBuscar = string.Empty;

            if (_dados.TryGetValue("CPF", out var cpf))
                CPFBuscar = (string)cpf;

            if (_configuracaoEmbarcador.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Brasil)
            {
                if (string.IsNullOrWhiteSpace(CPFBuscar))
                    throw new ImportacaoException("CPF não informado");

                if (!Utilidades.Validate.ValidarCPF(CPFBuscar))
                    throw new ImportacaoException("CPF é inválido");
            }

            int codigoEmpresa = 0;
            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = _empresa.Codigo;
            else
            {
                Dominio.Entidades.Empresa empresa = ObterTransportadora();
                codigoEmpresa = empresa?.Codigo ?? 0;
            }


            CPFBuscar = Utilidades.String.OnlyNumbers(CPFBuscar);

            Repositorio.Usuario repositorio = new Repositorio.Usuario(_unitOfWork);
            Dominio.Entidades.Usuario motorista = repositorio.BuscarMotoristaPorCPFEEmpresa(CPFBuscar.Trim(), codigoEmpresa);

            if (motorista == null)
            {
                return new Dominio.Entidades.Usuario()
                {
                    CPF = CPFBuscar
                };
            }

            motorista.Initialize();

            return motorista;
        }

        private void PreencherEnderecoPorCEP(Dominio.Entidades.Usuario motorista)
        {
            if (!string.IsNullOrWhiteSpace(motorista.CEP))
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
                Servicos.Embarcador.Localidades.Localidade svcLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);
                dynamic endereco = svcLocalidade.BuscarEnderecoPorCEP(motorista.CEP, _unitOfWork, _unitOfWorkAdmin);

                if (endereco != null)
                {
                    motorista.Endereco = endereco.Logradouro;
                    motorista.Bairro = endereco.Bairro;
                    motorista.Localidade = endereco.CodigoCidade > 0 ? repLocalidade.BuscarPorCodigo(endereco.CodigoCidade) : null;
                    motorista.TipoLogradouro = endereco.EnumTipoLogradouro;
                    motorista.Latitude = endereco.Latitude;
                    motorista.Longitude = endereco.Longitude;
                }
            }
        }

        private Dominio.Entidades.Localidade ObterLocalidadePorCidadeUF(string nomeLocalidade, string UFLocalidade)
        {
            if (string.IsNullOrWhiteSpace(nomeLocalidade) || string.IsNullOrWhiteSpace(UFLocalidade))
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCidadeUF(nomeLocalidade, UFLocalidade);

            if (localidade == null)
                throw new ImportacaoException("Localidade não encontrada!");

            return localidade;
        }


        private Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario ObterDadoBancario(Dominio.Entidades.Usuario motorista)
        {
            Dominio.Entidades.Banco banco = ObterBanco();
            string agencia = ObterAgencia();
            string numeroConta = ObterNumeroConta();

            if (banco == null || string.IsNullOrWhiteSpace(agencia) || string.IsNullOrWhiteSpace(numeroConta))
                return null;

            Repositorio.Embarcador.Transportadores.MotoristaDadoBancario repDadoBancario = new Repositorio.Embarcador.Transportadores.MotoristaDadoBancario(_unitOfWork);
            Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario dadoBancario = motorista.Codigo > 0 ? repDadoBancario.BuscarPorBancoAgenciaConta(banco.Codigo, agencia, numeroConta, motorista.Codigo) : null;

            if (dadoBancario != null)
                return null;

            dadoBancario = new Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario
            {
                Banco = banco,
                Agencia = agencia,
                NumeroConta = numeroConta,
                TipoContaBanco = TipoContaBanco.Corrente,
                Motorista = motorista,
                DigitoAgencia = string.Empty,
                ObservacaoConta = string.Empty
            };

            if (motorista.Codigo > 0)
            {
                repDadoBancario.Inserir(dadoBancario);
                return null;
            }

            return dadoBancario;
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

        public Dominio.Entidades.Usuario ObterMotoristaImportar()
        {
            Dominio.Entidades.Usuario motorista = ObterMotorista();

            motorista.NumeroRegistroHabilitacao = ObterNumeroRegistroHabilitacao();
            motorista.Nome = ObterNome();
            motorista.DataValidadeLiberacaoSeguradora = ObterDataValidadeSeguradora();
            motorista.NumeroHabilitacao = ObterCNH();
            motorista.DataVencimentoHabilitacao = ObterValidadeCNH();
            motorista.Telefone = ObterTelefone();
            motorista.DataNascimento = ObterDataNascimento();
            motorista.PIS = ObterPISPASEP();
            motorista.Empresa = ObterTransportadora();

            motorista.RG = ObterRG();
            motorista.DataEmissaoRG = ObterDataEmissaoRG();
            motorista.OrgaoEmissorRG = ObterEmissorRG();
            motorista.EstadoRG = ObterEstadoRG();
            motorista.Categoria = ObterCategoriaHabilitacao();
            motorista.DataHabilitacao = ObterDataHabilitacao();
            motorista.DataPrimeiraHabilitacao = ObterDataPrimeiraHabilitacao();
            motorista.Celular = ObterCelular();
            motorista.CEP = ObterCEP();
            motorista.DataAdmissao = ObterDataAdmissao();
            motorista.NumeroCTPS = ObterNumeroCTPS();
            motorista.SerieCTPS = ObterSerieCTPS();
            motorista.Cargo = ObterCargo();
            motorista.CBO = ObterCBO();
            motorista.NumeroMatricula = ObterNumeroMatricula();

            PreencherEnderecoPorCEP(motorista);
            motorista.NumeroEndereco = ObterNumeroEndereco();
            motorista.Complemento = ObterComplemento();
           
            motorista.PendenteIntegracaoEmbarcador = true;
            motorista.Status = "A";
            motorista.Tipo = "M";

            motorista.Endereco = ObterEndereco();
            motorista.Bairro = ObterBairro();
            motorista.Localidade = ObterIBGE();
            motorista.Localidade = ObterLocalidadePorCidadeUF(ObterCidade(), ObterUF());
            motorista.NumeroCartao = ObterNumeroCartao();
            motorista.TipoMotorista = ObterTipoMotorista();

            motorista.ClienteTerceiro = ObterPessoaAgregado();

            if (motorista.Setor == null)
                motorista.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };

            Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario dadoBancario = ObterDadoBancario(motorista);
            if (dadoBancario != null)
            {
                if (motorista.Codigo.Equals(0) || motorista.DadosBancarios == null)
                    motorista.DadosBancarios = new List<Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario>();

                motorista.DadosBancarios.Add(dadoBancario);
            }

            motorista.CodigoIntegracao = ObterCodigoIntegracao();

            return motorista;
        }

        #endregion
    }
}
