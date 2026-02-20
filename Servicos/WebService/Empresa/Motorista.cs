using Dominio.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.WebService.Empresa
{
    public class Motorista : ServicoBase
    {        
        public Motorista() : base() { }
        public Motorista(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Usuario SalvarMotorista(Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao, Dominio.Entidades.Empresa empresaIntegradora, ref string mensagem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = null, string adminStringConexao = "")
        {
            Servicos.Log.TratarErro($"Inicio SALVAR MOTORISTA");
            Repositorio.Localidade repLocalidades = new Repositorio.Localidade(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Servicos.Cliente serCliente = new Cliente(StringConexao);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Empresa empresa = null;

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                string cnpjTransportador = motoristaIntegracao.Transportador?.CNPJ ?? string.Empty;
                string codigoIntegracaoTransportador = motoristaIntegracao.Transportador?.CodigoIntegracao ?? string.Empty;

                if (motoristaIntegracao.Transportador != null && !string.IsNullOrWhiteSpace(cnpjTransportador) && Utilidades.Validate.ValidarCNPJ(cnpjTransportador)) //motoristaIntegracao.Transportador.CNPJ.Length > 11
                {
                    empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(motoristaIntegracao.Transportador.CNPJ));
                    if (empresa == null)
                        mensagem += "Transportador " + motoristaIntegracao.Transportador.CNPJ + " não encontrado. ";
                }
                else if (empresaIntegradora != null && empresa == null)
                    empresa = repEmpresa.BuscarPorCodigo(empresaIntegradora.Codigo);
                else if (motoristaIntegracao.Transportador != null && !string.IsNullOrWhiteSpace(codigoIntegracaoTransportador))
                {
                    empresa = repEmpresa.BuscarPorCodigoIntegracao(codigoIntegracaoTransportador);
                    if (empresa == null)
                        mensagem += "Transportador " + codigoIntegracaoTransportador + " não encontrado. ";
                }
            }


            Dominio.Entidades.Usuario motoristaUnico = null;
            string cpfMotorista = string.Empty;
            if (string.IsNullOrWhiteSpace(motoristaIntegracao.CPF) || motoristaIntegracao.CPF.Trim().Length > 11)
            {
                // manter legado primeira hierarquia sera pelo cpf
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.CodigoIntegracao))
                {
                    motoristaUnico = repUsuario.BuscarPorCodigoIntegracao(motoristaIntegracao.CodigoIntegracao);
                    if (motoristaUnico == null)
                    {
                        mensagem += "Código de integração do motorista inválido.";
                        return null;
                    }
                    else
                        cpfMotorista = motoristaUnico.CPF;
                }
                else
                {
                    mensagem += "Informe um CPF válido para cadastrar um novo motorista. ";
                    return null;
                }
            }
            else
            {
                cpfMotorista = long.Parse(Utilidades.String.OnlyNumbers(motoristaIntegracao.CPF)).ToString("d11");
                if (empresa != null && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    motoristaUnico = repUsuario.BuscarMotoristaPorCPF(empresa.Codigo, Utilidades.String.OnlyNumbers(cpfMotorista));
                else
                    motoristaUnico = repUsuario.BuscarMotoristaPorCPF(Utilidades.String.OnlyNumbers(cpfMotorista), string.Empty);
            }


            bool inserir = false;
            if (motoristaUnico == null)
            {
                motoristaUnico = new Dominio.Entidades.Usuario();
                motoristaUnico.DataCadastro = DateTime.Now;
                inserir = true;

                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.Nome))
                    motoristaUnico.Nome = motoristaIntegracao.Nome;
                else
                    mensagem += "É obrigatório informar o nome do motorista para cadastrar o mesmo. ";
            }

            List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();

            if ((configuracaoWebService?.AtualizarTodosCadastrosMotoristasMesmoCodigoIntegracao ?? false) && !inserir)
                motoristas.AddRange(repUsuario.BuscarTodosPorCodigoIntegracao(motoristaIntegracao.CodigoIntegracao));
            else
                motoristas.Add(motoristaUnico);

            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                if (!inserir)
                    motorista.Initialize();

                if (motorista.Status == "I")
                    motorista.Status = "A";

                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.Nome))
                    motorista.Nome = motoristaIntegracao.Nome;

                motorista.CPF = cpfMotorista;

                motorista.CodigoIntegracao = motoristaIntegracao.CodigoIntegracao;
                if (motoristaIntegracao.Endereco != null)
                {
                    if (motoristaIntegracao.Endereco.Cidade != null)
                        motorista.Localidade = repLocalidades.BuscarPorCodigoIBGE(motoristaIntegracao.Endereco.Cidade.IBGE);

                    motorista.Endereco = motoristaIntegracao.Endereco.Logradouro;
                    motorista.Bairro = motoristaIntegracao.Endereco.Bairro;
                    motorista.CEP = motoristaIntegracao.Endereco.CEP;
                    motorista.Complemento = motoristaIntegracao.Endereco.Complemento;
                    motorista.NumeroEndereco = motoristaIntegracao.Endereco.Numero;
                    motorista.Telefone = Utilidades.String.OnlyNumbers(motoristaIntegracao.Endereco.DDDTelefone) + Utilidades.String.OnlyNumbers(motoristaIntegracao.Endereco.Telefone);
                    motorista.Celular = Utilidades.String.OnlyNumbers(motoristaIntegracao.Endereco.DDDTelefone) + Utilidades.String.OnlyNumbers(motoristaIntegracao.Endereco.Telefone2);

                    if (!string.IsNullOrWhiteSpace(motorista.Telefone))
                        motorista.Telefone = motorista.Telefone_Formatado;

                    if (!string.IsNullOrWhiteSpace(motorista.Celular))
                        motorista.Celular = motorista.Celular_Formatado;
                }
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.DataEmissaoRG))
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(motoristaIntegracao.DataEmissaoRG, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        mensagem += "A data de emissão do RG não está em um formato correto (dd/MM/yyyy). ";
                    };
                    motorista.DataEmissaoRG = data;
                    if (motorista.DataEmissaoRG.Value.Year < 1900)
                    {
                        motorista.DataEmissaoRG = null;
                        mensagem += "O ano de emissão do RG não pode ser menor que 1900. ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.DataVencimentoHabilitacao))
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(motoristaIntegracao.DataVencimentoHabilitacao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        mensagem += "A data de vencimento da habilitação do motorista não está em um formato correto (dd/MM/yyyy). ";
                    };
                    motorista.DataVencimentoHabilitacao = data;
                    if (motorista.DataVencimentoHabilitacao.Value.Year < 1900)
                    {
                        motorista.DataVencimentoHabilitacao = null;
                        mensagem += "O ano de vencimento da habilitação do motorista não pode ser menor que 1900. ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.DataHabilitacao))
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(motoristaIntegracao.DataHabilitacao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        mensagem += "A data da habilitação do motorista não está em um formato correto (dd/MM/yyyy). ";
                    };
                    motorista.DataHabilitacao = data;
                    if (motorista.DataHabilitacao.Value.Year < 1900)
                    {
                        motorista.DataHabilitacao = null;
                        mensagem += "O ano de habilitação do motorista não pode ser menor que 1900. ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.DataAdmissao))
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(motoristaIntegracao.DataAdmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        mensagem += "A data de Admissão do motorista não está em um formato correto (dd/MM/yyyy). ";
                    };
                    motorista.DataAdmissao = data;
                    if (motorista.DataAdmissao.Value.Year < 1900)
                    {
                        motorista.DataAdmissao = null;
                        mensagem += "O ano de admissao do motorista não pode ser menor que 1900. ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.DataNascimento))
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(motoristaIntegracao.DataNascimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        mensagem += "A data de nascimento do motorista não está em um formato correto (dd/MM/yyyy). ";
                    };
                    motorista.DataNascimento = data;
                    if (motorista.DataNascimento.Value.Year < 1900)
                    {
                        motorista.DataNascimento = null;
                        mensagem += "O ano de nascimento motorista não pode ser menor que 1900. ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.DataValidadeGR))
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(motoristaIntegracao.DataValidadeGR, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        mensagem += "A data de validade da GR não está em um formato correto (dd/MM/yyyy). ";
                    };
                    motorista.DataValidadeLiberacaoSeguradora = data;
                    if (motorista.DataValidadeLiberacaoSeguradora.Value.Year < 1900)
                    {
                        motorista.DataValidadeLiberacaoSeguradora = null;
                        mensagem += "O ano de validade da GR não pode ser menor que 1900. ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.DataSuspensaoInicio))
                {
                    DateTime data = DateTime.MinValue;
                    if (!DateTime.TryParseExact(motoristaIntegracao.DataSuspensaoInicio, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        mensagem += "A data de inicio da suspensão não está em um formato correto (dd/MM/yyyy HH:mm). ";
                    };
                    //data = DateTime.Parse(motoristaIntegracao.DataSuspensaoInicio);
                    motorista.DataSuspensaoInicio = data;
                    if (motorista.DataSuspensaoInicio.Value.Year < 1900)
                    {
                        motorista.DataSuspensaoInicio = null;
                        mensagem += "O ano de inicio da suspensão não pode ser menor que 1900. ";
                    }
                }
                else motorista.DataSuspensaoInicio = null;
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.DataSuspensaoFim))
                {
                    DateTime data = DateTime.MinValue;
                    if (!DateTime.TryParseExact(motoristaIntegracao.DataSuspensaoFim, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        mensagem += "A data fim da suspensão não está em um formato correto (dd/MM/yyyy HH:mm). ";
                    };
                    //data = DateTime.Parse(motoristaIntegracao.DataSuspensaoFim);
                    motorista.DataSuspensaoFim = data;
                    if (motorista.DataSuspensaoFim.Value.Year < 1900)
                    {
                        motorista.DataSuspensaoFim = null;
                        mensagem += "O ano de fim da suspensão não pode ser menor que 1900. ";
                    }
                }
                else motorista.DataSuspensaoFim = null;

                if (motorista.DataSuspensaoFim != null && motorista.DataSuspensaoInicio != null && motorista.DataSuspensaoFim < motorista.DataSuspensaoInicio)
                    mensagem += "Data final de suspensão não pode ser menor que a inicial. ";

                motorista.Email = motoristaIntegracao.Email;
                motorista.EnderecoDigitado = true;
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.NumeroHabilitacao))
                    motorista.NumeroHabilitacao = motoristaIntegracao.NumeroHabilitacao;
                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.CategoriaCNH))
                {
                    string categoriaCNH = Utilidades.String.OnlyNumbersAndChars(motoristaIntegracao.CategoriaCNH);
                    if (!string.IsNullOrWhiteSpace(categoriaCNH) && (categoriaCNH.Length == 2 || categoriaCNH.Length == 1))
                        motorista.Categoria = motoristaIntegracao.CategoriaCNH;
                    else
                        mensagem += "Categoria da CNH do motorista deve ter 1 ou 2 caracteres ";
                }

                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.RG) && motoristaIntegracao.RG.Length > 20)
                    mensagem += "O RG do motorista não deve conter mais que 20 caracteres. ";

                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.Nome) && motoristaIntegracao.Nome.Length > 80)
                    mensagem += "O nome do motorista não deve conter mais que 80 caracteres. ";

                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.RG))
                    motorista.RG = motoristaIntegracao.RG;

                if (motoristaIntegracao.OrgaoEmissorRG.HasValue)
                    motorista.OrgaoEmissorRG = motoristaIntegracao.OrgaoEmissorRG;

                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.EstadoRG))
                {
                    Dominio.Entidades.Estado estadoRG = repEstado.BuscarPorSigla(motoristaIntegracao.EstadoRG);
                    if (estadoRG == null)
                        mensagem += "Sigla do estado do RG do motorista inválido. ";
                    else
                        motorista.EstadoRG = estadoRG;
                }

                motorista.Tipo = "M";

                if (motorista.Setor == null)
                    motorista.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };

                if (motoristaIntegracao.Ativo.HasValue)
                    motorista.Status = motoristaIntegracao.Ativo.Value ? "A" : "I";
                else if (inserir)
                    motorista.Status = "A";

                motorista.MotivoBloqueio = motoristaIntegracao.MotivoBloqueio;

                if (motoristaIntegracao.tipoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos)
                    motorista.TipoMotorista = motoristaIntegracao.tipoMotorista;

                if (motoristaIntegracao.DadosBancarios != null)
                {
                    Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

                    motorista.Agencia = motoristaIntegracao.DadosBancarios.Agencia;
                    motorista.DigitoAgencia = motoristaIntegracao.DadosBancarios.DigitoAgencia;
                    motorista.NumeroConta = motoristaIntegracao.DadosBancarios.NumeroConta;
                    motorista.TipoContaBanco = motoristaIntegracao.DadosBancarios.TipoContaBanco;

                    if (motoristaIntegracao.DadosBancarios.Banco != null)
                    {
                        int codigoBanco;
                        int.TryParse(motoristaIntegracao.DadosBancarios.Banco.CodigoBanco, out codigoBanco);

                        motorista.Banco = repBanco.BuscarPorNumero(codigoBanco);
                    }

                    if (motorista.Banco == null)
                        mensagem += "O banco informado (dados bancários do motorista) (" + motoristaIntegracao.DadosBancarios.Banco?.CodigoBanco + " - " + motoristaIntegracao.DadosBancarios.Banco?.NomeBanco + ") não foi encontrado na base da Multisoftware. ";

                    if (string.IsNullOrWhiteSpace(motorista.Agencia))
                        mensagem += "A agência informada (dados bancários do motorista) é inválida. ";

                    if (string.IsNullOrWhiteSpace(motorista.NumeroConta))
                        mensagem += "O número da conta informada (dados bancários do motorista) é inválida. ";
                }

                motorista.NumeroCartao = motoristaIntegracao.NumeroCartao;

                if (!string.IsNullOrWhiteSpace(motorista.NumeroCartao) && motorista.NumeroCartao.Length > 16)
                    mensagem += "O número do cartão do motorista não pode ter mais do que 16 caracteres.";

                if (motoristaIntegracao.Escolaridade != null)
                    motorista.Escolaridade = motoristaIntegracao.Escolaridade;

                if (motoristaIntegracao.EstadoCivil != null)
                    motorista.EstadoCivil = motoristaIntegracao.EstadoCivil;

                if (empresa != null && !(configuracaoWebService?.AtualizarTodosCadastrosMotoristasMesmoCodigoIntegracao ?? false))
                {
                    if (empresa.Matriz != null && empresa.Matriz.Count > 0)
                    {
                        motorista.Empresa = empresa.Matriz.FirstOrDefault();

                        if (motorista.Localidade == null)
                            motorista.Localidade = empresa.Matriz.FirstOrDefault().Localidade;
                    }
                    else
                    {
                        motorista.Empresa = empresa;
                        if (motorista.Localidade == null)
                            motorista.Localidade = empresa.Localidade;
                    }
                }

                if (motoristaIntegracao.Transportador != null && !string.IsNullOrWhiteSpace(motoristaIntegracao.Transportador.CNPJ) && motoristaIntegracao.Transportador.CNPJ.Length <= 11)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Dominio.Entidades.Cliente clienteTerceiro = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(motoristaIntegracao.Transportador.CNPJ)));
                    if (clienteTerceiro != null)
                        motorista.ClienteTerceiro = clienteTerceiro;
                    else
                    {
                        Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoVerificacaoCliente = serCliente.ConverterParaTransportadorTerceiro(motoristaIntegracao.Transportador, "Transportador Terceiro", unitOfWork, false, auditado);
                        if (retornoVerificacaoCliente.Status)
                        {
                            motorista.ClienteTerceiro = retornoVerificacaoCliente.cliente;
                        }
                        else
                        {
                            mensagem += retornoVerificacaoCliente.Mensagem;
                            return null;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(motoristaIntegracao.DataDemissao))
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(motoristaIntegracao.DataDemissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        mensagem += "A data de Demissão do motorista não está em um formato correto (dd/MM/yyyy). ";
                    };
                    motorista.DataDemissao = data;
                }

                if (motoristaIntegracao.SituacaoColaborador != null)
                    motorista.SituacaoColaborador = motoristaIntegracao.SituacaoColaborador;

                if (!string.IsNullOrWhiteSpace(mensagem))
                    return null;

                Servicos.Log.TratarErro($"Inicio Cadastro Motoristas {motorista.CPF} ");
                if (clienteAcesso != null && !string.IsNullOrWhiteSpace(adminStringConexao))
                {
                    Servicos.Log.TratarErro($"Processando Motorista.... {motorista.CPF} ");
                    if (motoristaIntegracao.UtilizaMultiMobile.HasValue)
                    {
                        if (motoristaIntegracao.UtilizaMultiMobile.Value)
                            ConfigurarUsuarioMobile(ref motoristaUnico, clienteAcesso, adminStringConexao);
                        else
                        {
                            Servicos.Usuario servicoUsuario = new Servicos.Usuario(unitOfWork);
                            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);
                            servicoUsuario.RemoveUsuarioMobile(ref motoristaUnico, clienteAcesso, unitOfWorkAdmin);
                            motorista.CodigoMobile = 0;
                            unitOfWorkAdmin.Dispose();
                        }
                    }
                    else if (configuracaoTMS.CadastrarMotoristaMobileAutomaticamente)
                        ConfigurarUsuarioMobile(ref motoristaUnico, clienteAcesso, adminStringConexao);
                }
                Servicos.Log.TratarErro($"Fin Cadastro Motoristas {motorista.CPF} ");

                if (inserir)
                    repUsuario.Inserir(motorista, auditado);
                else
                {
                    repUsuario.Atualizar(motorista);
                    Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(auditado, motorista, motorista.GetChanges(), "Atualizou o motorista.", unitOfWork);
                }

                Servicos.Embarcador.Transportadores.Motorista.AtualizarIntegracoes(motorista, unitOfWork);
            }

            return motoristaUnico;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> ConverterListaObjetoMotorista(ICollection<Dominio.Entidades.Usuario> motoristas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> listaMotorista = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                listaMotorista.Add(ConverterObjetoMotorista(motorista));
            }

            return listaMotorista;
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.Motorista ConverterObjetoMotorista(Dominio.Entidades.Usuario motorista)
        {
            if (motorista == null)
                return null;

            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista
            {
                Codigo = motorista.Codigo,
                CodigoIntegracao = motorista.CodigoIntegracao,
                CPF = motorista.CPF,
                CategoriaCNH = motorista.Categoria,
                DataAdmissao = motorista.DataAdmissao.HasValue ? motorista.DataAdmissao.Value.ToString("dd/MM/yyyy") : "",
                DataHabilitacao = motorista.DataHabilitacao.HasValue ? motorista.DataHabilitacao.Value.ToString("dd/MM/yyyy") : "",
                DataNascimento = motorista.DataNascimento.HasValue ? motorista.DataNascimento.Value.ToString("dd/MM/yyyy") : "",
                DataVencimentoHabilitacao = motorista.DataVencimentoHabilitacao.HasValue ? motorista.DataVencimentoHabilitacao.Value.ToString("dd/MM/yyyy") : "",
                Email = motorista.Email,
                Transportador = serEmpresa.ConverterObjetoEmpresa(motorista.Empresa),
                Nome = motorista.Nome,
                NumeroHabilitacao = motorista.NumeroHabilitacao,
                RG = motorista.RG,
                EstadoRG = motorista.EstadoRG?.Sigla ?? string.Empty,
                Endereco = ConverterObjetoEnderecoMotorista(motorista),
                NumeroCartao = motorista.NumeroCartao,
                NumeroPISPASEP = motorista.PIS,
                ListaDadosBancarios = ConverterObjetoListaDadosBancariosMotorista(motorista.DadosBancarios),
                ListaContatos = ConverterObjetoListaContatosMotorista(motorista.Contatos)
            };

            return motoristaIntegracao;
        }

        #endregion

        #region Métodos Privados

        public string ConfigurarUsuarioMobile(ref Dominio.Entidades.Usuario motorista, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);
            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCFP(motorista.CPF);
                bool inserir = false;
                if (usuarioMobile == null)
                {
                    inserir = true;
                    usuarioMobile = new AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile();
                    usuarioMobile.CPF = motorista.CPF;
                    usuarioMobile.Celular = "";
                    usuarioMobile.Nome = motorista.Nome;
                    usuarioMobile.Sessao = "";
                }

                usuarioMobile.DataSessao = DateTime.Now;
                usuarioMobile.Senha = "";
                usuarioMobile.Ativo = true;

                Servicos.Log.TratarErro($"Inserindo Motorista.... {(inserir ? "Novo" : "Atualizando")}");

                if (inserir)
                    repUsuarioMobile.Inserir(usuarioMobile);
                else
                    repUsuarioMobile.Atualizar(usuarioMobile);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = null;
                if (usuarioMobile.Clientes != null)
                    usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == clienteAcesso.Cliente.Codigo select obj).FirstOrDefault();

                inserir = false;
                if (usuarioMobileCliente == null)
                {
                    Servicos.Log.TratarErro($"Inserindo Motorista mobile....");
                    usuarioMobileCliente = new AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente();
                    usuarioMobileCliente.Cliente = clienteAcesso.Cliente;
                    usuarioMobileCliente.UsuarioMobile = usuarioMobile;
                    usuarioMobileCliente.BaseHomologacao = clienteAcesso.URLHomologacao;
                    inserir = true;
                }
                else
                {
                    Servicos.Log.TratarErro($"Não Inseriu Motorista mobile....");
                    if (usuarioMobileCliente.BaseHomologacao == false && clienteAcesso.URLHomologacao)
                        return "Esse motorista já está apto a utilizar o aplicativo em produção, não sendo possível configurar o mesmo em homologação";

                    usuarioMobileCliente.BaseHomologacao = clienteAcesso.URLHomologacao;
                }

                if (inserir)
                    repUsuarioMobileCliente.Inserir(usuarioMobileCliente);
                else
                    repUsuarioMobileCliente.Atualizar(usuarioMobileCliente);

                motorista.CodigoMobile = usuarioMobile.Codigo;
                return "";
            }
            catch (Exception ex)
            {
                unitOfWorkAdmin.Dispose();
                throw;
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco ConverterObjetoEnderecoMotorista(Dominio.Entidades.Usuario motorista)
        {
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Servicos.Embarcador.Localidades.Localidade();

            return new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
            {
                Bairro = motorista.Bairro,
                CEP = motorista.CEP,
                Cidade = serLocalidade.ConverterObjetoLocalidade(motorista.Localidade),
                Complemento = motorista.Complemento,
                Logradouro = motorista.Endereco,
                Numero = motorista.NumeroEndereco,
                Telefone = motorista.Telefone
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.MotoristaContato> ConverterObjetoListaContatosMotorista(IList<Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato> listaContatos)
        {
            if (listaContatos == null || listaContatos.Count == 0)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Carga.MotoristaContato> contatos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MotoristaContato>();

            foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato contato in listaContatos)
                contatos.Add(ConverterObjetoContatosMotorista(contato));

            return contatos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosBancarios> ConverterObjetoListaDadosBancariosMotorista(IList<Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario> listaDadosBancarios)
        {
            if (listaDadosBancarios == null || listaDadosBancarios.Count == 0)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosBancarios> dadosBancarios = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosBancarios>();

            foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario dadoBancario in listaDadosBancarios)
                dadosBancarios.Add(ConverterObjetoDadosBancariosMotorista(dadoBancario));

            return dadosBancarios;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.MotoristaContato ConverterObjetoContatosMotorista(Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato contato)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.MotoristaContato()
            {
                Nome = contato.Nome,
                CPF = contato.CPF,
                Email = contato.Email,
                Telefone = contato.Telefone,
                DataNascimento = contato.DataNascimento?.ToDateString() ?? string.Empty,
                TipoParentesco = contato.TipoParentesco
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosBancarios ConverterObjetoDadosBancariosMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario dadoBancario)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosBancarios()
            {
                Banco = ConverterObjetoBanco(dadoBancario.Banco),
                Agencia = dadoBancario.Agencia,
                DigitoAgencia = dadoBancario.DigitoAgencia,
                NumeroConta = dadoBancario.NumeroConta,
                TipoContaBanco = dadoBancario.TipoContaBanco
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.Banco ConverterObjetoBanco(Dominio.Entidades.Banco banco)
        {
            if (banco == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.Banco()
            {
                NomeBanco = banco.Descricao,
                CodigoBanco = banco.Numero.ToString()
            };
        }

        #endregion
    }
}
