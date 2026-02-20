using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.ObjetosDeValor.Embarcador.Veiculos;
using Dominio.ObjetosDeValor.WebService.Logistica.Veiculo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.WebService.Empresa
{
    public class Empresa : ServicoBase
    {
        #region Variaveis Privadas

        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Constructores

        public Empresa(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }        

        public Empresa(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Metodos Públicos
        public List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> RetornarEmpresas(ICollection<Dominio.Entidades.Empresa> empresas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> empresasIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>();
            foreach (Dominio.Entidades.Empresa empresa in empresas)
            {
                empresasIntegracao.Add(ConverterObjetoEmpresa(empresa));
            }

            return empresasIntegracao;
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa ConverterObjetoEmpresa(Dominio.Entidades.Empresa empresa)
        {
            if (empresa != null)
            {
                Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade();
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                empresaIntegracao.CNPJ = empresa.CNPJ;
                empresaIntegracao.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                empresaIntegracao.Endereco.Bairro = empresa.Bairro;
                empresaIntegracao.Endereco.CEP = empresa.CEP;
                empresaIntegracao.Endereco.Logradouro = empresa.Endereco;
                empresaIntegracao.Endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(empresa.Localidade);
                empresaIntegracao.Endereco.Complemento = empresa.Complemento;
                empresaIntegracao.Endereco.Telefone = empresa.Telefone;
                empresaIntegracao.Endereco.Numero = empresa.Numero;
                empresaIntegracao.Emails = empresa.Email;
                empresaIntegracao.IE = empresa.InscricaoEstadual;
                empresaIntegracao.NomeFantasia = empresa.NomeFantasia;
                empresaIntegracao.RazaoSocial = empresa.RazaoSocial;
                empresaIntegracao.CodigoIntegracao = empresa.CodigoIntegracao;
                empresaIntegracao.RNTRC = empresa.RegistroANTT;
                empresaIntegracao.CodigoDocumento = empresa.CodigoDocumento;
                empresaIntegracao.SimplesNacional = empresa.OptanteSimplesNacional;
                empresaIntegracao.InscricaoMunicipal = empresa.InscricaoMunicipal;
                empresaIntegracao.InscricaoST = empresa.Inscricao_ST;
                empresaIntegracao.EmissaoDocumentosForaDoSistema = empresa.EmissaoDocumentosForaDoSistema;
                empresaIntegracao.LiberacaoParaPagamentoAutomatico = empresa.LiberacaoParaPagamentoAutomatico;
                empresaIntegracao.Protocolo = empresa.Codigo;

                if (empresa.DataInicialCertificado.HasValue && empresa.DataFinalCertificado.HasValue && !string.IsNullOrEmpty(empresa.NomeCertificado))
                {
                    empresaIntegracao.Certificado = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Certificado();
                    empresaIntegracao.Certificado.DataInicio = empresa.DataInicialCertificado.Value;
                    empresaIntegracao.Certificado.DataFim = empresa.DataFinalCertificado.Value;
                    if (empresaIntegracao.Certificado.DataInicio <= DateTime.Now && empresaIntegracao.Certificado.DataFim >= DateTime.Now.Date)
                        empresaIntegracao.Certificado.CertificadoAtivo = true;
                }

                return empresaIntegracao;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa ConverterObjetoEmpresa(Dominio.Entidades.Cliente cliente)
        {
            if (cliente != null)
            {
                Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade();
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                empresaIntegracao.CNPJ = cliente.CPF_CNPJ_SemFormato;
                empresaIntegracao.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                empresaIntegracao.Endereco.Bairro = cliente.Bairro;
                empresaIntegracao.Endereco.CEP = cliente.CEP;
                empresaIntegracao.Endereco.Logradouro = cliente.Endereco;
                empresaIntegracao.Endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(cliente.Localidade);
                empresaIntegracao.Endereco.Complemento = cliente.Complemento;
                empresaIntegracao.Endereco.Telefone = cliente.Telefone1;
                empresaIntegracao.IE = cliente.IE_RG;
                empresaIntegracao.NomeFantasia = cliente.NomeFantasia;
                empresaIntegracao.RazaoSocial = cliente.Nome;
                empresaIntegracao.InscricaoMunicipal = cliente.InscricaoMunicipal;
                empresaIntegracao.InscricaoST = "";
                return empresaIntegracao;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa ConverterObjetoEmpresa(Dominio.Entidades.ParticipanteCTe participanteCTe)
        {
            if (participanteCTe != null)
            {
                Servicos.Embarcador.Localidades.Localidade serLocalidade = new Embarcador.Localidades.Localidade();
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                empresaIntegracao.CNPJ = participanteCTe.CPF_CNPJ_SemFormato;
                empresaIntegracao.Atividade = participanteCTe.Atividade?.Codigo ?? 3;
                empresaIntegracao.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                empresaIntegracao.Endereco.Bairro = participanteCTe.Bairro;
                empresaIntegracao.Endereco.CEP = participanteCTe.CEP;
                empresaIntegracao.Endereco.Logradouro = participanteCTe.Endereco;
                empresaIntegracao.Endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(participanteCTe.Localidade);
                empresaIntegracao.Endereco.Complemento = participanteCTe.Complemento;
                empresaIntegracao.Endereco.Telefone = participanteCTe.Telefone1;
                empresaIntegracao.Endereco.Numero = participanteCTe.Numero;
                empresaIntegracao.IE = participanteCTe.IE_RG;
                empresaIntegracao.NomeFantasia = participanteCTe.NomeFantasia;
                empresaIntegracao.RazaoSocial = participanteCTe.Nome;
                empresaIntegracao.InscricaoMunicipal = participanteCTe.InscricaoMunicipal;
                empresaIntegracao.InscricaoST = "";
                empresaIntegracao.Emails = participanteCTe.Email;
                return empresaIntegracao;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa ConverterObjetoEmpresa(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa)
        {
            if (pessoa != null)
            {                
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                empresaIntegracao.CNPJ = pessoa.CPFCNPJ;
                empresaIntegracao.Endereco = pessoa.Endereco;
                empresaIntegracao.IE = pessoa.RGIE;
                empresaIntegracao.NomeFantasia = pessoa.NomeFantasia;
                empresaIntegracao.RazaoSocial = pessoa.RazaoSocial;
                empresaIntegracao.InscricaoMunicipal = pessoa.IM;
                empresaIntegracao.InscricaoST = "";
                empresaIntegracao.Emails = pessoa.Email;
                return empresaIntegracao;
            }
            else
            {
                return null;
            }
        }

        public string ValidarCamposEmpresaIntegracao(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa transportador)
        {
            if (transportador == null)
                return "Nennhum dado do transportador recebido.";

            if (string.IsNullOrWhiteSpace(transportador.CNPJ) || !Utilidades.Validate.ValidarCNPJ(Utilidades.String.OnlyNumbers(transportador.CNPJ)))
                return "CNPJ " + transportador.CNPJ + " não é valido.";

            if (string.IsNullOrWhiteSpace(transportador.RazaoSocial) || transportador.RazaoSocial.Length <= 2)
                return "Razão social não é valida, deve ter mais que 2 caracteres.";

            if (string.IsNullOrWhiteSpace(transportador.NomeFantasia) || transportador.NomeFantasia.Length <= 2)
            {
                transportador.NomeFantasia = transportador.RazaoSocial;
                if (string.IsNullOrWhiteSpace(transportador.NomeFantasia) || transportador.NomeFantasia.Length <= 2)
                    return "Nome Fantasia não é valida, deve ter mais que 2 caracteres.";
            }

            if (transportador.Endereco == null)
                return "Endereço do transportador é obrigatório.";

            if (string.IsNullOrWhiteSpace(transportador.Endereco.Logradouro) || transportador.Endereco.Logradouro.Length <= 2)
                return "Logradouro/rua não é valida, deve ter mais que 2 caracteres.";

            if (string.IsNullOrWhiteSpace(transportador.Endereco.CEP) || Utilidades.String.OnlyNumbers(transportador.Endereco.CEP).Length != 8)
                return "CEP não é valido.";

            if (string.IsNullOrWhiteSpace(transportador.Endereco.Bairro) || transportador.Endereco.Bairro.Length < 2)
                return "Bairro não é valida, deve ter mais que 2 caracteres.";

            if (transportador.Endereco.Cidade == null)
                return "Cidade do transportador é obrigatório.";

            if ((transportador.Endereco.Cidade.IBGE == 0) && (string.IsNullOrWhiteSpace(transportador.Endereco.Cidade.Descricao) || string.IsNullOrWhiteSpace(transportador.Endereco.Cidade.SiglaUF)))
                return "Obrigatório enviar IBGE ou Nome da Cidade e Sigla doe estado.";

            return string.Empty;
        }

        public Task<bool> EmpresaIntegracaoExisteAsync(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa transportador)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            return repEmpresa.ExistePorCNPJAsync(Utilidades.String.OnlyNumbers(transportador.CNPJ));
        }

        public void AdicionarOutAtualizarEmpresa(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa transportador, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string adminStringConexao)
        {

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);

            AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(unitOfWorkAdmin);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Pais repositorioPais = new Repositorio.Pais(unitOfWork);

            Dominio.Entidades.Localidade localidade = transportador.Endereco.Cidade.IBGE > 0 ? repLocalidade.BuscarPorCodigoIBGE(transportador.Endereco.Cidade.IBGE) : repLocalidade.BuscarPorDescricaoEUF(transportador.Endereco.Cidade.Descricao, transportador.Endereco.Cidade.SiglaUF);

            if (localidade == null)
            {
                //buscar por cep no admin.
                AdminMultisoftware.Dominio.Entidades.Localidades.Endereco Endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(transportador.Endereco.CEP)).ToString());
                string CodigoIBGE = Endereco?.Localidade?.CodigoIBGE ?? "";
                if (!string.IsNullOrEmpty(CodigoIBGE))
                {
                    localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(Utilidades.String.OnlyNumbers(CodigoIBGE)));
                }
            }

            if (localidade == null)
            {
                unitOfWorkAdmin.Dispose();
                throw new ServicoException(transportador.Endereco.Cidade.IBGE > 0 ? $"Não localizada cidade com o IBGE {transportador.Endereco.Cidade.IBGE}" : $"Não localizada cidade {transportador.Endereco.Cidade.Descricao} / {transportador.Endereco.Cidade.SiglaUF}");
            }

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(transportador.CNPJ));

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            if (empresa != null)
            {
                bool atualizarTipoEmissao = !repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);

                if (!string.IsNullOrWhiteSpace(transportador.IE))
                    empresa.InscricaoEstadual = Utilidades.String.OnlyNumbers(transportador.IE);
                if (!string.IsNullOrWhiteSpace(transportador.NomeFantasia))
                    empresa.NomeFantasia = transportador.NomeFantasia;
                if (!string.IsNullOrWhiteSpace(transportador.RazaoSocial))
                    empresa.RazaoSocial = transportador.RazaoSocial;
                if (!string.IsNullOrWhiteSpace(transportador.RNTRC))
                    empresa.RegistroANTT = Utilidades.String.OnlyNumbers(transportador.RNTRC);
                if (!string.IsNullOrWhiteSpace(transportador.Emails))
                    empresa.Email = transportador.Emails;
                if (!string.IsNullOrWhiteSpace(transportador.Endereco.Logradouro))
                    empresa.Endereco = transportador.Endereco.Logradouro;
                if (!string.IsNullOrWhiteSpace(transportador.Endereco.Numero))
                    empresa.Numero = transportador.Endereco.Numero;
                if (!string.IsNullOrWhiteSpace(transportador.Endereco.Complemento))
                    empresa.Complemento = transportador.Endereco.Complemento;
                if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(transportador.Endereco.CEP)))
                    empresa.CEP = Utilidades.String.OnlyNumbers(transportador.Endereco.CEP);
                if (!string.IsNullOrWhiteSpace(transportador.Endereco.Bairro))
                    empresa.Bairro = transportador.Endereco.Bairro;
                if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(transportador.Endereco.Telefone)))
                    empresa.Telefone = Utilidades.String.OnlyNumbers(transportador.Endereco.Telefone);
                if (!string.IsNullOrWhiteSpace(transportador.CodigoIntegracao))
                    empresa.CodigoIntegracao = transportador.CodigoIntegracao;
                if (!string.IsNullOrWhiteSpace(transportador?.Endereco?.Cidade?.Pais?.SiglaPais ?? string.Empty))
                    empresa.Pais = repositorioPais.BuscarPorSiglaUF(transportador.Endereco.Cidade.Pais.SiglaPais);
                if (!string.IsNullOrWhiteSpace(transportador.InscricaoMunicipal))
                    empresa.InscricaoMunicipal = Utilidades.String.OnlyNumbers(transportador.InscricaoMunicipal);
                if (!string.IsNullOrWhiteSpace(transportador.IMO))
                    empresa.IMO = transportador.IMO;
                if (transportador.DataValidadeIMO.HasValue)
                    empresa.DataValidadeIMO = transportador.DataValidadeIMO;

                if (transportador.Ativo.HasValue)
                {
                    if (transportador.Ativo.Value)
                        empresa.Status = "A";
                    else
                        empresa.Status = "I";
                }

                if (atualizarTipoEmissao)
                    empresa.EmissaoDocumentosForaDoSistema = transportador.EmissaoDocumentosForaDoSistema;

                if (string.IsNullOrWhiteSpace(empresa.FusoHorario))
                    empresa.FusoHorario = empresa.EmpresaPai?.FusoHorario ?? string.Empty;

                empresa.Localidade = localidade;
                repEmpresa.Atualizar(empresa);
                Servicos.Auditoria.Auditoria.Auditar(auditado, empresa, "Atualizado via WebService", unitOfWork);
            }
            else
            {
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();
                List<Dominio.Entidades.EmpresaSerie> empSeriesPai = repSerie.BuscarTodosPorEmpresa(empresaPai.Codigo);

                empresa = new Dominio.Entidades.Empresa
                {
                    CNPJ = Utilidades.String.OnlyNumbers(transportador.CNPJ),
                    InscricaoEstadual = Utilidades.String.OnlyNumbers(transportador.IE),
                    NomeFantasia = transportador.NomeFantasia,
                    RazaoSocial = transportador.RazaoSocial,
                    RegistroANTT = Utilidades.String.OnlyNumbers(transportador.RNTRC),
                    Email = transportador.Emails,
                    Endereco = transportador.Endereco.Logradouro,
                    Numero = string.IsNullOrWhiteSpace(transportador.Endereco.Numero) ? "S/N" : transportador.Endereco.Numero,
                    Complemento = transportador.Endereco.Complemento,
                    CEP = Utilidades.String.OnlyNumbers(transportador.Endereco.CEP),
                    Bairro = transportador.Endereco.Bairro,
                    Telefone = Utilidades.String.OnlyNumbers(transportador.Endereco.Telefone),
                    Localidade = localidade,
                    TipoAmbiente = repEmpresa.BuscarEmpresaPai()?.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Homologacao,
                    Status = transportador.Ativo.HasValue ? transportador.Ativo.Value ? "A" : "I" : "A",
                    EmpresaPai = empresaPai,
                    CodigoIntegracao = transportador.CodigoIntegracao,
                    Pais = (transportador?.Endereco?.Cidade?.Pais?.SiglaPais ?? string.Empty) != "" ? repositorioPais.BuscarPorSiglaUF(transportador.Endereco.Cidade.Pais.SiglaPais) : null,
                    EmissaoDocumentosForaDoSistema = transportador.EmissaoDocumentosForaDoSistema,
                    FusoHorario = empresaPai?.FusoHorario ?? string.Empty,
                    EmpresaMobile = empresaPai?.EmpresaMobile ?? false,
                    ModeloDocumentoFiscalCargaPropria = empresaPai?.ModeloDocumentoFiscalCargaPropria ?? null,
                    InscricaoMunicipal = Utilidades.String.OnlyNumbers(transportador.InscricaoMunicipal),
                    IMO = transportador.IMO ?? string.Empty,
                    DataValidadeIMO = transportador.DataValidadeIMO,
                };

                repEmpresa.Inserir(empresa);

                SalvarSeries(empresa, empSeriesPai, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(auditado, empresa, "Inserida via WebService", unitOfWork);
                unitOfWorkAdmin.Dispose();
            }
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarTransportador(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa transportador)
        {

            string mensagemValidacao = ValidarCamposEmpresaIntegracao(transportador);

            if (!string.IsNullOrWhiteSpace(mensagemValidacao))
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagemValidacao);

            AdicionarOutAtualizarEmpresa(transportador, _unitOfWork, _auditado, _adminStringConexao);
            _unitOfWork.CommitChanges();

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);

        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarMotorista(Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao, Dominio.Entidades.WebService.Integradora integradora)
        {
            Servicos.WebService.Empresa.Motorista serWSMotorista = new Servicos.WebService.Empresa.Motorista(_unitOfWork);

            _unitOfWork.Start();
            string mensagem = "";

            if (motoristaIntegracao.tipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos)
                motoristaIntegracao.tipoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio;

            Dominio.Entidades.Usuario motorista = serWSMotorista.SalvarMotorista(motoristaIntegracao, integradora.Empresa, ref mensagem, _unitOfWork, _tipoServicoMultisoftware, _auditado, _clienteAcesso, _adminStringConexao);

            if (motorista != null)
            {
                Servicos.Auditoria.Auditoria.Auditar(_auditado, motorista, "Salvou motorista", _unitOfWork);

                _unitOfWork.CommitChanges();

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }
            else
            {
                _unitOfWork.Rollback();
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagem);
            }

        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao, Dominio.Entidades.WebService.Integradora integradora)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Servicos.WebService.Frota.Veiculo(_unitOfWork);

            string mensagem = "";

            _unitOfWork.Start();

            Dominio.Entidades.Veiculo veiculo = serWSVeiculo.SalvarVeiculo(veiculoIntegracao, integradora.Empresa, false, ref mensagem, _unitOfWork, _tipoServicoMultisoftware, _auditado);

            if (veiculo == null)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarVeiculo: " + mensagem);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagem);
            }

            if (configuracaoTMS != null && configuracaoTMS.CadastrarVeiculoTerceiroParaEmpresa && veiculoIntegracao.Proprietario?.TransportadorTerceiro != null && !string.IsNullOrWhiteSpace(veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ) && Utilidades.Validate.ValidarCNPJ(veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ))
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ);
                if (empresa == null)
                {
                    _unitOfWork.CommitChanges();
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Transportador/proprietário do veículo informado (" + veiculoIntegracao.Proprietario.TransportadorTerceiro.CNPJ + ") não possui cadastro.");
                }

                Dominio.Entidades.Veiculo veiculoTerceiro = serWSVeiculo.SalvarVeiculo(veiculoIntegracao, empresa, true, ref mensagem, _unitOfWork, _tipoServicoMultisoftware, _auditado);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    Servicos.Log.TratarErro("SalvarVeiculo Terceiro: " + mensagem);
                mensagem = "";
            }

            if (veiculo == null)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarVeiculo: " + mensagem);
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(mensagem);
            }

            Servicos.Auditoria.Auditoria.Auditar(_auditado, veiculo, "Salvou Veiculo via Web Service", _unitOfWork);
            _unitOfWork.CommitChanges();
            Servicos.Log.TratarErro("SalvarVeiculo: Sucesso: " + veiculo.Codigo.ToString());

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>> BuscarTransportadoresPendentesIntegracao(int quatidade)
        {
            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Empresa> repositorioIntegracaoEmpresa = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Empresa>(_unitOfWork);

            int totalRegistrosPentendeIntegracao = repositorioIntegracaoEmpresa.ContarRegistroPendenteIntegracao();

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa> retorno = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>(),
                NumeroTotalDeRegistro = totalRegistrosPentendeIntegracao
            };

            if (totalRegistrosPentendeIntegracao == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>>.CriarRetornoSucesso(retorno);

            IList<Dominio.Entidades.Empresa> listaTranspostadoresPendenteIntegracao = repositorioIntegracaoEmpresa.BuscarRegitrosPendenteIntegracao(quatidade);

            foreach (Dominio.Entidades.Empresa transportador in listaTranspostadoresPendenteIntegracao)
                retorno.Itens.Add(ConverterObjetoEmpresa(transportador));

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa>>.CriarRetornoSucesso(retorno);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoTransportador(List<int> protocolos)
        {
            if (protocolos == null && protocolos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Precisa informar os protocolos que serar comfirmados");

            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Empresa> repositorioEmpresa = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Empresa>(_unitOfWork);
            IList<Dominio.Entidades.Empresa> listaTransportadoresParaConfirmar = repositorioEmpresa.BuscarRegitrosPendentesIntegracaoPeloProtocolos(protocolos);
            List<int> protocolosNaoProcessado = protocolos.Where(c => !listaTransportadoresParaConfirmar.Any(m => m.Codigo == c)).ToList();

            foreach (Dominio.Entidades.Empresa transportador in listaTransportadoresParaConfirmar)
            {
                transportador.IntegradoERP = true;
                repositorioEmpresa.Atualizar(transportador);
            }

            if (protocolosNaoProcessado == null || protocolosNaoProcessado.Count > 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, $"Para os Protocolo(s) {string.Join(",", protocolosNaoProcessado)} Não foram encontrados registros ou ja foram comfirmados.");

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Todos os protocolo integrados com sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>> BuscarVeiculosPendentesIntegracao(int quatidade)
        {
            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Veiculo> repositorioIntegracaoEmpresa = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Veiculo>(_unitOfWork);

            int totalRegistrosPentendeIntegracao = repositorioIntegracaoEmpresa.ContarRegistroPendenteIntegracao();

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo> retorno = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>(),
                NumeroTotalDeRegistro = totalRegistrosPentendeIntegracao
            };

            if (totalRegistrosPentendeIntegracao == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>>.CriarRetornoSucesso(retorno);

            IList<Dominio.Entidades.Veiculo> listaTranspostadoresPendenteIntegracao = repositorioIntegracaoEmpresa.BuscarRegitrosPendenteIntegracao(quatidade);
            Servicos.WebService.Frota.Veiculo servicoFrota = new Frota.Veiculo(_unitOfWork);

            foreach (Dominio.Entidades.Veiculo veiculo in listaTranspostadoresPendenteIntegracao)
                retorno.Itens.Add(servicoFrota.ConverterObjetoVeiculo(veiculo, _unitOfWork));

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>>.CriarRetornoSucesso(retorno);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoVeiculo(List<int> protocolos)
        {
            if (protocolos == null && protocolos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Precisa informar os protocolos que serar comfirmados");

            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Veiculo> repositorioVeiculo = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Veiculo>(_unitOfWork);
            IList<Dominio.Entidades.Veiculo> listaVeiculosParaConfirmar = repositorioVeiculo.BuscarRegitrosPendentesIntegracaoPeloProtocolos(protocolos);
            List<int> protocolosNaoProcessado = protocolos.Where(c => !listaVeiculosParaConfirmar.Any(m => m.Codigo == c)).ToList();

            foreach (Dominio.Entidades.Veiculo veiculo in listaVeiculosParaConfirmar)
            {
                veiculo.IntegradoERP = true;
                repositorioVeiculo.Atualizar(veiculo);
            }

            if (protocolosNaoProcessado == null || protocolosNaoProcessado.Count > 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, $"Para os Protocolo(s) {string.Join(",", protocolosNaoProcessado)} Não foram encontrados registros ou ja foram comfirmados.");

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Todos os protocolo integrados com sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Veiculos.RetornoVeiculoRastreadores> ObterRastreadoresVeiculosPorData(Dominio.ObjetosDeValor.Embarcador.Veiculos.DataConsultaModificacao dataModificacao)
        {
            Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Veiculos.RetornoVeiculoRastreadores> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.Embarcador.Veiculos.RetornoVeiculoRastreadores>();

            if (dataModificacao == null || dataModificacao.DataModificacao == DateTime.MinValue || dataModificacao.DataModificacaoFinal == DateTime.MinValue)
            {
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Status = false;
                retorno.Mensagem = "Data não informada ou inválida";
                return retorno;
            }

            retorno.Objeto = ObterVeiculosRastreadores(dataModificacao);
            retorno.Status = true;

            return retorno;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo InformarDisponibilidadeFrota(FilaCarregamentoVeiculo filaCarregamento, StringBuilder stMensagem, int existente)
        {
            try
            {
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWork);

                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWork, _auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaRetorno;

                FilaCarregamentoVeiculoAdicionar filaCarregamentoVeiculoAdicionar = ConverterWebServiceParaEmbarcador(filaCarregamento, stMensagem);
                if (stMensagem.Length > 0)
                    return null;

                if (existente > 0)
                {
                    filaRetorno = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(existente);
                    AtualizarExistente(filaCarregamentoVeiculoAdicionar, filaRetorno, servicoFilaCarregamentoVeiculo);

                    repositorioFilaCarregamentoVeiculo.Atualizar(filaRetorno);
                }
                else
                    filaRetorno = servicoFilaCarregamentoVeiculo.Adicionar(filaCarregamentoVeiculoAdicionar, _tipoServicoMultisoftware);

                return filaRetorno;
            }
            catch (ServicoException ex)
            {
                stMensagem.Append(ex.ToString());
            }
            catch (BaseException)
            {
                _unitOfWork.Rollback();
            }
            catch (System.Exception excecao)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(excecao);
            }

            return null;
        }

        public int ValidarFilaCarregamentoVeiculo(FilaCarregamentoVeiculo filaCarregamento, StringBuilder stMensagem)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorioTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWork);

            bool utilizarProgramacaoCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro()?.UtilizarProgramacaoCarga ?? false;
            int codigoFilaExistente = 0;

            if (string.IsNullOrEmpty(filaCarregamento.PlacaVeiculo))
                stMensagem.Append("É necessário informar a placa de um Veículo");
            else
            {
                var veiculo = repositorioVeiculo.BuscarPlaca(filaCarregamento.PlacaVeiculo);
                if (veiculo == null)
                    stMensagem.Append("Não foi encontrado um Veículo com a placa " + filaCarregamento.PlacaVeiculo + " na base da Multisoftware");
                else
                    codigoFilaExistente = repositorioFilaCarregamentoVeiculo.BuscarAtivaPorVeiculo(veiculo.Codigo)?.Codigo ?? 0;
            }

            bool atualizar = codigoFilaExistente > 0;

            if (string.IsNullOrEmpty(filaCarregamento.CodigoIntegracaoCentroCarregamento) && !atualizar)
                stMensagem.Append("É necessário informar o código de integração de um Centro de Carregamento");
            else if (!string.IsNullOrEmpty(filaCarregamento.CodigoIntegracaoCentroCarregamento))
            {
                var cg = repositorioCentroCarregamento.BuscarPorCodigoIntegracao(filaCarregamento.CodigoIntegracaoCentroCarregamento);
                if (cg == null)
                    stMensagem.Append("Não foi encontrado um Centro de Carregamento para o código de integração " + filaCarregamento.CodigoIntegracaoCentroCarregamento + " na base da Multisoftware");
            }

            if (string.IsNullOrEmpty(filaCarregamento.CPFMotorista) && utilizarProgramacaoCarga && !atualizar)
                stMensagem.Append("É necessário informar o CPF de um motorista");
            else if (utilizarProgramacaoCarga && !string.IsNullOrEmpty(filaCarregamento.CPFMotorista))
            {
                var motorista = repositorioUsuario.BuscarPorCPF(filaCarregamento.CPFMotorista);
                if (motorista == null)
                    stMensagem.Append("Não foi encontrado um Motorista com o CPF " + filaCarregamento.CPFMotorista + " na base da Multisoftware");
            }

            if (!string.IsNullOrEmpty(filaCarregamento.TipoRetorno))
            {
                int qtd = repositorioTipoRetornoCarga.BuscarPorDescricaoEAtivo(filaCarregamento.TipoRetorno).Count;
                if (qtd == 0)
                    stMensagem.Append("Não foi encontrado um Tipo de Retorno de Carga ativo com a descrição " + filaCarregamento.TipoRetorno + " na base da Multisoftware");
            }

            if ((utilizarProgramacaoCarga || _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS) && !string.IsNullOrWhiteSpace(filaCarregamento.PrevisaoChegada))
            {
                if (!DateTime.TryParseExact(filaCarregamento.PrevisaoChegada, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    stMensagem.Append("A Data de Previsão de Chegada não está no formato correto (dd/MM/yyyy HH:mm:ss)");
                else if (_tipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(_unitOfWork).BuscarPrimeiroRegistro();

                    if (configuracaoPreCarga.DiasParaTransportadorAdicionarFilaCarregamentoVeiculo > 0)
                    {
                        DateTime dataLimiteAdicionarFilaCarregamentoVeiculo = DateTime.Now.AddDays(configuracaoPreCarga.DiasParaTransportadorAdicionarFilaCarregamentoVeiculo);

                        if (data > dataLimiteAdicionarFilaCarregamentoVeiculo)
                            stMensagem.Append($"A data de previsão de chegada informada excede o limite configurado ({dataLimiteAdicionarFilaCarregamentoVeiculo.ToDateTimeString()})");
                    }
                }
            }
            else if ((utilizarProgramacaoCarga || _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS) && !atualizar)
                stMensagem.Append("É necessário informar uma Data de Previsão de Chegada");

            if ((filaCarregamento.CidadesDestino?.Count > 0 && (filaCarregamento.SiglasEstadoDestino?.Count > 0 || filaCarregamento.CodigosIntegracaoRegiaoDestino?.Count > 0)) ||
                (filaCarregamento.SiglasEstadoDestino?.Count > 0 && filaCarregamento.CodigosIntegracaoRegiaoDestino?.Count > 0))
                stMensagem.Append("Só é possível adicionar um conjunto de destinos. Escolha um conjunto entre cidades, estados e regiões.");

            return codigoFilaExistente;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.RetornoVeiculoRastreadores ObterVeiculosRastreadores(Dominio.ObjetosDeValor.Embarcador.Veiculos.DataConsultaModificacao dataModificacao)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarVeiculosPorPeriodoAtualizacao(dataModificacao.DataModificacao, dataModificacao.DataModificacaoFinal);

            Dominio.ObjetosDeValor.Embarcador.Veiculos.RetornoVeiculoRastreadores retornoVeiculoRastreadores = new RetornoVeiculoRastreadores();
            retornoVeiculoRastreadores.Veiculos = new List<VeiculosRastreadores>();

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                retornoVeiculoRastreadores.Veiculos.Add(new Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculosRastreadores
                {
                    Placa = veiculo.Placa?.ObterPlacaFormatada() ?? string.Empty,
                    TipoVeiculo = veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    Status = "A",
                    Rastreadores = ObterRastreadores(veiculo, dataModificacao)
                });
            }

            return retornoVeiculoRastreadores;
        }

        private List<Rastreadores> ObterRastreadores(Dominio.Entidades.Veiculo veiculo, Dominio.ObjetosDeValor.Embarcador.Veiculos.DataConsultaModificacao dataModificacao)
        {
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Posicao posicao = repPosicao.BuscarUltimaPosicaoVeiculo(veiculo.Codigo, dataModificacao.DataModificacao, dataModificacao.DataModificacaoFinal);

            List<Rastreadores> rastreadores = new List<Rastreadores>();

            if (posicao != null)
            {
                rastreadores.Add(
                new Rastreadores()
                {
                    IdDispositivo = posicao.IDEquipamento,
                    UltimaPosicao = posicao.Data.ToString("yyyy-MM-ddTHH:mm"),
                    UltimaPosicaoString = posicao.Data.ToString(),
                    Status = posicao.Ignicao == 1 ? "A" : "I"
                });
            }

            return rastreadores;
        }

        private void SalvarSeries(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.EmpresaSerie> empPaiSeries, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

            if (empPaiSeries.Count > 0)
            {
                foreach (var serie in empPaiSeries)
                {
                    Dominio.Entidades.EmpresaSerie empSerie = repSerie.BuscarPorSerie(empresa.Codigo, (int)serie.Numero, (Dominio.Enumeradores.TipoSerie)serie.Tipo);

                    if (empSerie == null)
                    {
                        empSerie = new Dominio.Entidades.EmpresaSerie();
                        empSerie.Empresa = empresa;
                        empSerie.Numero = (int)serie.Numero;
                        empSerie.Tipo = (Dominio.Enumeradores.TipoSerie)serie.Tipo;
                    }

                    empSerie.ProximoNumeroDocumento = (int)serie.ProximoNumeroDocumento;
                    empSerie.Status = (string)serie.Status;
                    empSerie.NaoGerarCargaAutomaticamente = (bool)serie.NaoGerarCargaAutomaticamente;

                    if (empSerie.Codigo > 0)
                        repSerie.Atualizar(empSerie);
                    else
                        repSerie.Inserir(empSerie);
                }
            }
            else
                SalvarSeriePadrao(empresa, unidadeDeTrabalho);
        }

        private void SalvarSeriePadrao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeTrabalho);
            if (empresa.EmpresaPai.Configuracao != null)
            {
                Dominio.Entidades.EmpresaSerie empSerie = new Dominio.Entidades.EmpresaSerie();
                empSerie.Empresa = empresa;
                empSerie.Numero = 1;
                empSerie.Tipo = Dominio.Enumeradores.TipoSerie.NFe;
                empSerie.Status = "A";
                repEmpresaSerie.Inserir(empSerie);

                empSerie = new Dominio.Entidades.EmpresaSerie();
                empSerie.Empresa = empresa;
                empSerie.Numero = 1;
                empSerie.Tipo = Dominio.Enumeradores.TipoSerie.NFSe;
                empSerie.Status = "A";
                repEmpresaSerie.Inserir(empSerie);

                empSerie = new Dominio.Entidades.EmpresaSerie();
                empSerie.Empresa = empresa;
                empSerie.Numero = 1;
                empSerie.Tipo = Dominio.Enumeradores.TipoSerie.CTe;
                empSerie.Status = "A";
                repEmpresaSerie.Inserir(empSerie);

                empSerie = new Dominio.Entidades.EmpresaSerie();
                empSerie.Empresa = empresa;
                empSerie.Numero = 1;
                empSerie.Tipo = Dominio.Enumeradores.TipoSerie.MDFe;
                empSerie.Status = "A";
                repEmpresaSerie.Inserir(empSerie);
            }
            else
            {
                Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
                serie.Empresa = empresa;
                serie.Numero = 1;
                serie.Status = "A";
                serie.Tipo = Dominio.Enumeradores.TipoSerie.CTe;
                repEmpresaSerie.Inserir(serie);
            }
        }

        private void AtualizarExistente(FilaCarregamentoVeiculoAdicionar filaCarregamento, Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaExistente, Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo)
        {
            if (filaCarregamento.DataProgramada != null && filaCarregamento.DataProgramada.Value != filaExistente.DataProgramada)
                filaExistente.DataProgramada = filaCarregamento.DataProgramada;

            if (filaCarregamento.CodigoMotorista > 0 && filaExistente.ConjuntoMotorista?.Motorista?.Codigo != filaCarregamento.CodigoMotorista)
                servicoFilaCarregamentoVeiculo.InformarMotorista(filaExistente.Codigo, filaCarregamento.CodigoMotorista, _tipoServicoMultisoftware, atualizarMotorista: true);

            if (filaCarregamento.CodigoCentroCarregamento > 0 && filaExistente.CentroCarregamento?.Codigo != filaCarregamento.CodigoCentroCarregamento)
                servicoFilaCarregamentoVeiculo.AlterarCentroCarregamento(filaExistente.Codigo, filaCarregamento.CodigoCentroCarregamento, _tipoServicoMultisoftware);

            if (filaCarregamento.CodigosDestino?.Count > 0 || filaCarregamento.SiglasEstadoDestino?.Count > 0 || filaCarregamento.CodigosRegiaoDestino?.Count > 0)
            {
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoDestino repCidades = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoDestino(_unitOfWork);
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino repEstados = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino(_unitOfWork);
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino repRegioes = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino(_unitOfWork);

                // Cidades
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino> cidadesExistentes = repCidades.BuscarPorFilaCarregamentoVeiculo(filaExistente.Codigo);

                List<int> removerCidades = cidadesExistentes.Select(o => o.Localidade.Codigo).ToList();
                List<int> adicionarCidades = filaCarregamento.CodigosDestino.Except(removerCidades).ToList();
                removerCidades = removerCidades.Except(filaCarregamento.CodigosDestino).ToList();

                foreach (var destino in cidadesExistentes.Where(o => removerCidades.Contains(o.Localidade.Codigo)))
                    repCidades.Deletar(destino);

                foreach (int codigoDestino in adicionarCidades)
                    repCidades.Inserir(new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino()
                    {
                        FilaCarregamentoVeiculo = filaExistente,
                        Localidade = new Dominio.Entidades.Localidade() { Codigo = codigoDestino }
                    });

                // Estados
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino> estadosExistentes = repEstados.BuscarPorFilaCarregamentoVeiculo(filaExistente.Codigo);

                List<string> removerEstados = estadosExistentes.Select(o => o.Estado.Sigla).ToList();
                List<string> adicionarEstados = filaCarregamento.SiglasEstadoDestino.Except(removerEstados).ToList();
                removerEstados = removerEstados.Except(filaCarregamento.SiglasEstadoDestino).ToList();

                foreach (var destino in estadosExistentes.Where(o => removerEstados.Contains(o.Estado.Sigla)))
                    repEstados.Deletar(destino);

                foreach (string codigoDestino in adicionarEstados)
                    repEstados.Inserir(new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino()
                    {
                        FilaCarregamentoVeiculo = filaExistente,
                        Estado = new Dominio.Entidades.Estado() { Sigla = codigoDestino }
                    });

                // Regiões
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino> regioesExistentes = repRegioes.BuscarPorFilaCarregamentoVeiculo(filaExistente.Codigo);

                List<int> removerRegioes = regioesExistentes.Select(o => o.Regiao.Codigo).ToList();
                List<int> adicionarRegioes = filaCarregamento.CodigosRegiaoDestino.Except(removerRegioes).ToList();
                removerRegioes = removerRegioes.Except(filaCarregamento.CodigosRegiaoDestino).ToList();

                foreach (var destino in regioesExistentes.Where(o => removerRegioes.Contains(o.Regiao.Codigo)))
                    repRegioes.Deletar(destino);

                foreach (int codigoDestino in adicionarRegioes)
                    repRegioes.Inserir(new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino()
                    {
                        FilaCarregamentoVeiculo = filaExistente,
                        Regiao = new Dominio.Entidades.Embarcador.Localidades.Regiao() { Codigo = codigoDestino }
                    });
            }

            if (filaCarregamento.CodigosTipoCarga?.Count > 0)
            {
                // Tipos de Carga
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga repositorioFilaCarregamentoVeiculoTipoCarga = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga> tiposExistentes = repositorioFilaCarregamentoVeiculoTipoCarga.BuscarPorFilaCarregamentoVeiculo(filaExistente.Codigo);

                List<int> removerTipos = tiposExistentes.Select(o => o.TipoCarga.Codigo).ToList();
                List<int> adicionarTipos = filaCarregamento.CodigosTipoCarga.Except(removerTipos).ToList();
                removerTipos = removerTipos.Except(filaCarregamento.CodigosTipoCarga).ToList();

                foreach (var tipo in tiposExistentes.Where(o => removerTipos.Contains(o.TipoCarga.Codigo)))
                    repositorioFilaCarregamentoVeiculoTipoCarga.Deletar(tipo);

                foreach (int codigoTipoCarga in adicionarTipos)
                    repositorioFilaCarregamentoVeiculoTipoCarga.Inserir(new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga()
                    {
                        FilaCarregamentoVeiculo = filaExistente,
                        TipoCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = codigoTipoCarga }
                    });
            }

        }

        private FilaCarregamentoVeiculoAdicionar ConverterWebServiceParaEmbarcador(FilaCarregamentoVeiculo filaCarregamento, StringBuilder stMensagem)
        {
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorioTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);

            bool utilizarProgramacaoCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro()?.UtilizarProgramacaoCarga ?? false;

            var centro = repositorioCentroCarregamento.BuscarPorCodigoIntegracao(filaCarregamento.CodigoIntegracaoCentroCarregamento, somenteAtivos: true);
            var motorista = utilizarProgramacaoCarga ? repositorioUsuario.BuscarPorCPF(filaCarregamento.CPFMotorista) : null;
            int tipoRetorno = repositorioTipoRetornoCarga.BuscarPorDescricaoEAtivo(filaCarregamento.TipoRetorno).FirstOrDefault()?.Codigo ?? 0;

            FilaCarregamentoVeiculoAdicionar filaCarregamentoVeiculoAdicionar = new FilaCarregamentoVeiculoAdicionar()
            {
                CodigoCentroCarregamento = centro?.Codigo ?? 0,
                CodigoFilial = centro?.Filial?.Codigo ?? 0,
                CodigoMotorista = motorista?.Codigo ?? 0,
                CodigoTipoRetornoCarga = tipoRetorno,
                CodigoVeiculo = repositorioVeiculo.BuscarPlaca(filaCarregamento.PlacaVeiculo)?.Codigo ?? 0,
                CodigosDestino = new List<int>(),
                SiglasEstadoDestino = new List<string>(),
                CodigosRegiaoDestino = new List<int>(),
                CodigosTipoCarga = new List<int>(),
            };

            if (utilizarProgramacaoCarga || _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                filaCarregamentoVeiculoAdicionar.DataProgramada = DateTime.ParseExact(filaCarregamento.PrevisaoChegada, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None);

            if (filaCarregamento.CidadesDestino?.Count > 0)
            {
                foreach (var cidade in filaCarregamento.CidadesDestino)
                {
                    var loc = repositorioLocalidade.BuscarPorCodigoIBGE(cidade.CodigoIBGE) ?? repositorioLocalidade.BuscarPorDescricao(cidade.Descricao).FirstOrDefault();
                    if (loc != null)
                        filaCarregamentoVeiculoAdicionar.CodigosDestino.Add(loc.Codigo);
                    else
                    {
                        string erro = cidade.CodigoIBGE > 0 ? $"Código IBGE {cidade.CodigoIBGE}" : $"Descrição {cidade.Descricao}";
                        stMensagem.Append($"Não foi encontrada uma cidade com {erro} na base da Multisoftware");
                    }
                }
            }

            if (filaCarregamento.SiglasEstadoDestino?.Count > 0)
            {
                foreach (var sigla in filaCarregamento.SiglasEstadoDestino)
                {
                    var estado = repositorioEstado.BuscarPorSigla(sigla);
                    if (estado != null)
                        filaCarregamentoVeiculoAdicionar.SiglasEstadoDestino.Add(estado.Sigla);
                    else
                        stMensagem.Append($"Não foi encontrado um estado com a sigla {sigla} na base da Multisoftware");
                }
            }

            if (filaCarregamento.CodigosIntegracaoRegiaoDestino?.Count > 0)
            {
                foreach (var regiao in filaCarregamento.CodigosIntegracaoRegiaoDestino)
                {
                    var reg = repositorioRegiao.BuscarPorCodigoIntegracao(regiao);

                    if (reg != null)
                        filaCarregamentoVeiculoAdicionar.CodigosRegiaoDestino.Add(reg.Codigo);
                    else
                        stMensagem.Append($"Não foi encontrada uma região com código de integração {regiao} na base da Multisoftware");
                }
            }

            if (filaCarregamento.CodigosIntegracaoTipoCarga?.Count > 0)
            {
                foreach (var tipoCarga in filaCarregamento.CodigosIntegracaoTipoCarga)
                {
                    var tipo = repositorioTipoDeCarga.BuscarPorCodigoEmbarcador(tipoCarga);

                    if (tipo != null)
                        filaCarregamentoVeiculoAdicionar.CodigosTipoCarga.Add(tipo.Codigo);
                    else
                        stMensagem.Append($"Não foi encontrado um tipo de carga com código de integração {tipoCarga} na base da Multisoftware");
                }
            }

            return filaCarregamentoVeiculoAdicionar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemAlteracaoFilaCarregamento ObterOrigemAlteracaoFilaCarregamento()
        {
            return Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(_tipoServicoMultisoftware);
        }

        #endregion
    }
}
