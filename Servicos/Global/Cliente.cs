using Dominio.ObjetosDeValor.Embarcador.Pessoas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos
{
    public class Cliente
    {
        public Cliente(string stringConexao) { }

        public Cliente(Repositorio.UnitOfWork unitOfWork) { }
        public Cliente() { }

        #region Métodos Públicos

        public Dominio.Entidades.ParticipanteCTe ConverterClienteParaParticipanteCTe(Dominio.Entidades.Cliente cliente)
        {
            Dominio.Entidades.ParticipanteCTe participanteCTe = new Dominio.Entidades.ParticipanteCTe();

            if (cliente != null)
            {
                participanteCTe.CPF_CNPJ = cliente.CPF_CNPJ_SemFormato;

                participanteCTe.Atividade = cliente.Atividade;
                participanteCTe.Email = cliente.Email;
                participanteCTe.EmailStatus = !string.IsNullOrWhiteSpace(cliente.Email) ? true : false;
                participanteCTe.EmailContador = cliente.EmailContador;
                participanteCTe.EmailContadorStatus = !string.IsNullOrWhiteSpace(cliente.EmailContador) ? true : false;
                participanteCTe.EmailContato = cliente.EmailContato;
                participanteCTe.EmailContatoStatus = !string.IsNullOrWhiteSpace(cliente.EmailContato) ? true : false;
                participanteCTe.IE_RG = cliente.IE_RG;
                participanteCTe.Nome = cliente.Nome;
                participanteCTe.NomeFantasia = cliente.NomeFantasia;
                participanteCTe.Cliente = cliente;
                participanteCTe.Bairro = cliente.Bairro;
                participanteCTe.CEP = cliente.CEP;
                participanteCTe.Complemento = cliente.Complemento;
                participanteCTe.Endereco = cliente.Endereco;
                participanteCTe.Localidade = cliente.Localidade;
                participanteCTe.Numero = cliente.Numero;
                participanteCTe.Telefone1 = cliente.Telefone1;
                participanteCTe.Telefone2 = cliente.Telefone2;
                participanteCTe.Cidade = cliente.Localidade.Descricao;
                participanteCTe.GrupoPessoas = cliente.GrupoPessoas;
                participanteCTe.CodigoIntegracao = cliente.CodigoIntegracao;

                if (cliente.Localidade.Pais != null)
                    participanteCTe.Pais = cliente.Localidade.Pais;

                participanteCTe.Cidade = cliente.Cidade;
                participanteCTe.Exterior = cliente.Tipo == "E" ? true : false;
                participanteCTe.Tipo = cliente.Tipo == "F" ? Dominio.Enumeradores.TipoPessoa.Fisica : Dominio.Enumeradores.TipoPessoa.Juridica;
            }
            else
            {
                participanteCTe = null;
            }


            return participanteCTe;
        }

        public Dominio.ObjetosDeValor.CTe.Cliente ObterClienteCTE(Dominio.Entidades.ParticipanteCTe participamenteCte)
        {
            Dominio.ObjetosDeValor.CTe.Cliente clienteCTE = new Dominio.ObjetosDeValor.CTe.Cliente();

            if (participamenteCte != null)
            {
                clienteCTE.CPFCNPJ = participamenteCte.CPF_CNPJ_SemFormato;
                clienteCTE.CPFCNPJFormatado = participamenteCte.CPF_CNPJ_Formatado;
                if (!string.IsNullOrWhiteSpace(participamenteCte.CPF_CNPJ))
                    clienteCTE.CGC = participamenteCte.CPF_CNPJ.ToString();

                clienteCTE.CodigoAtividade = participamenteCte.Atividade != null ? participamenteCte.Atividade.Codigo : 0;
                clienteCTE.Emails = participamenteCte.Email;
                clienteCTE.StatusEmails = !string.IsNullOrWhiteSpace(participamenteCte.Email) ? true : false;
                clienteCTE.EmailsContador = participamenteCte.EmailContador;
                clienteCTE.StatusEmailsContador = !string.IsNullOrWhiteSpace(participamenteCte.EmailContador) ? true : false;
                clienteCTE.EmailsContato = participamenteCte.EmailContato;
                clienteCTE.StatusEmailsContato = !string.IsNullOrWhiteSpace(participamenteCte.EmailContato) ? true : false;
                clienteCTE.RGIE = participamenteCte.IE_RG;
                clienteCTE.RazaoSocial = participamenteCte.Nome;
                clienteCTE.NomeFantasia = participamenteCte.NomeFantasia;

                clienteCTE.Bairro = participamenteCte.Bairro;
                clienteCTE.CEP = participamenteCte.CEP;
                clienteCTE.Complemento = participamenteCte.Complemento;
                clienteCTE.Endereco = participamenteCte.Endereco;
                clienteCTE.CodigoIBGECidade = participamenteCte.Localidade?.CodigoIBGE ?? 0;
                clienteCTE.Numero = participamenteCte.Numero;
                clienteCTE.Telefone1 = Utilidades.String.OnlyNumbers(participamenteCte.Telefone1);
                clienteCTE.Telefone2 = Utilidades.String.OnlyNumbers(participamenteCte.Telefone2);
                clienteCTE.Cidade = participamenteCte.Localidade?.Descricao;

                if (participamenteCte.Localidade?.Pais != null)
                    clienteCTE.CodigoPais = participamenteCte.Localidade.Pais.Sigla.ToString();

                clienteCTE.Cidade = participamenteCte.Cidade;
                clienteCTE.Exportacao = participamenteCte.Exterior;
                clienteCTE.CodigoLocalidadeEmbarcador = participamenteCte.Localidade?.CodigoLocalidadeEmbarcador;
            }
            else
            {
                clienteCTE = null;
            }


            return clienteCTE;
        }

        public Dominio.ObjetosDeValor.CTe.Cliente ConverterEmpresaParaCliente(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.CTe.Cliente clienteCTE = new Dominio.ObjetosDeValor.CTe.Cliente();

            if (empresa != null)
            {
                clienteCTE.CPFCNPJ = empresa.CNPJ;
                clienteCTE.CPFCNPJFormatado = empresa.CNPJ_Formatado;
                clienteCTE.CGC = empresa.CNPJ;

                clienteCTE.CodigoAtividade = 4;
                clienteCTE.Emails = empresa.Email;
                clienteCTE.StatusEmails = !string.IsNullOrWhiteSpace(empresa.Email) ? true : false;
                clienteCTE.EmailsContador = empresa.EmailContador;
                clienteCTE.StatusEmailsContador = !string.IsNullOrWhiteSpace(empresa.EmailContador) ? true : false;
                clienteCTE.EmailsContato = empresa.EmailAdministrativo;
                clienteCTE.StatusEmailsContato = !string.IsNullOrWhiteSpace(empresa.EmailAdministrativo) ? true : false;
                clienteCTE.RGIE = empresa.InscricaoEstadual;
                clienteCTE.RazaoSocial = empresa.RazaoSocial;
                clienteCTE.NomeFantasia = empresa.NomeFantasia;

                clienteCTE.Bairro = empresa.Bairro;
                clienteCTE.CEP = empresa.CEP;
                clienteCTE.Complemento = empresa.Complemento;
                clienteCTE.Endereco = empresa.Endereco;
                clienteCTE.CodigoIBGECidade = empresa.Localidade.CodigoIBGE;
                clienteCTE.Numero = empresa.Numero;
                clienteCTE.Telefone1 = Utilidades.String.OnlyNumbers(empresa.Telefone);
                clienteCTE.Telefone2 = Utilidades.String.OnlyNumbers(empresa.TelefoneContato);
                clienteCTE.Cidade = empresa.Localidade.Descricao;
                if (empresa.Localidade.Pais != null)
                    clienteCTE.CodigoPais = empresa.Localidade.Pais.Sigla.ToString();
                clienteCTE.Exportacao = false;
            }
            else
            {
                clienteCTE = null;
            }


            return clienteCTE;
        }

        public Dominio.ObjetosDeValor.CTe.Cliente ObterClienteCTE(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco)
        {
            if (cliente == null)
                return null;

            Dominio.ObjetosDeValor.CTe.Cliente clienteCTE = new Dominio.ObjetosDeValor.CTe.Cliente();

            clienteCTE.CPFCNPJ = cliente.CPF_CNPJ_SemFormato;
            clienteCTE.CPFCNPJFormatado = cliente.CPF_CNPJ_Formatado;
            clienteCTE.CGC = cliente.CPF_CNPJ.ToString();
            clienteCTE.CodigoAtividade = cliente.Atividade != null ? cliente.Atividade.Codigo : 0;
            clienteCTE.Emails = cliente.Email;
            clienteCTE.StatusEmails = cliente.EmailStatus == "A" ? true : false;
            clienteCTE.EmailsContador = cliente.EmailContador;
            clienteCTE.StatusEmailsContador = cliente.EmailContadorStatus == "A" ? true : false;
            clienteCTE.EmailsContato = cliente.EmailContato;
            clienteCTE.StatusEmailsContato = cliente.EmailContatoStatus == "A" ? true : false;
            clienteCTE.RGIE = cliente.IE_RG;
            clienteCTE.RazaoSocial = cliente.Nome;
            clienteCTE.NomeFantasia = cliente.NomeFantasia;
            clienteCTE.Codigo = cliente.CPF_CNPJ;
            clienteCTE.NaoAtualizarDadosCadastrais = cliente.NaoAtualizarDados;

            if (pedidoEndereco == null)// || cliente.Tipo == "E")
            {
                clienteCTE.Bairro = cliente.Bairro;
                clienteCTE.CEP = cliente.CEP;
                clienteCTE.Complemento = !string.IsNullOrWhiteSpace(cliente.Complemento) ? cliente.Complemento : "";
                clienteCTE.Endereco = cliente.Endereco;
                clienteCTE.CodigoIBGECidade = cliente.Localidade.CodigoIBGE;
                clienteCTE.Numero = cliente.Numero;
                clienteCTE.Telefone1 = Utilidades.String.OnlyNumbers(cliente.Telefone1);
                clienteCTE.Telefone2 = Utilidades.String.OnlyNumbers(cliente.Telefone2);
                clienteCTE.Cidade = cliente.Localidade.Descricao;
                clienteCTE.NaoAtualizarEndereco = true;
                if (cliente.Tipo == "E")
                {
                    if (cliente.Localidade.Pais != null)
                        clienteCTE.CodigoPais = cliente.Localidade.Pais.Sigla.ToString();
                    else if (cliente.Pais != null)
                        clienteCTE.CodigoPais = cliente.Pais.Sigla.ToString();

                    if (!string.IsNullOrWhiteSpace(cliente.Cidade))
                        clienteCTE.Cidade = cliente.Cidade;
                    clienteCTE.Exportacao = true;
                }
                else
                {
                    clienteCTE.CodigoPais = cliente.Localidade.Estado.Pais.Sigla.ToString();
                    clienteCTE.Exportacao = false;
                }

                if (!string.IsNullOrWhiteSpace(cliente.Localidade.CodigoLocalidadeEmbarcador))
                    clienteCTE.CodigoLocalidadeEmbarcador = cliente.Localidade.CodigoLocalidadeEmbarcador;
            }
            else
            {
                clienteCTE.CodigoEndereco = pedidoEndereco.ClienteOutroEndereco?.CodigoEmbarcador ?? "";
                clienteCTE.NaoAtualizarEndereco = cliente?.Localidade?.Estado?.Sigla == "EX" ? true : true;
                clienteCTE.Bairro = pedidoEndereco.Bairro;
                clienteCTE.CEP = pedidoEndereco.CEP;
                clienteCTE.Complemento = !string.IsNullOrWhiteSpace(pedidoEndereco.Complemento) ? pedidoEndereco.Complemento : "";
                clienteCTE.Endereco = pedidoEndereco.Endereco;
                clienteCTE.CodigoIBGECidade = pedidoEndereco.Localidade.CodigoIBGE;
                clienteCTE.Numero = pedidoEndereco.Numero;
                clienteCTE.Telefone1 = Utilidades.String.OnlyNumbers(pedidoEndereco.Telefone);
                clienteCTE.Cidade = pedidoEndereco.Localidade.Descricao;
                if (!string.IsNullOrWhiteSpace(pedidoEndereco.IE_RG))
                    clienteCTE.RGIE = pedidoEndereco.IE_RG;

                if (pedidoEndereco.Localidade.CodigoIBGE == 9999999 || cliente.Tipo == "E")
                {
                    clienteCTE.Exportacao = true;
                    if (pedidoEndereco.Localidade.Pais != null)
                        clienteCTE.CodigoPais = pedidoEndereco.Localidade.Pais.Sigla.ToString();
                    else if (cliente.Pais != null)
                        clienteCTE.CodigoPais = cliente.Pais.Sigla.ToString();
                }
                else
                {
                    clienteCTE.CodigoPais = pedidoEndereco.Localidade.Estado.Pais.Sigla.ToString();
                    clienteCTE.Exportacao = false;
                }

                if (!string.IsNullOrWhiteSpace(pedidoEndereco.Localidade.CodigoLocalidadeEmbarcador))
                    clienteCTE.CodigoLocalidadeEmbarcador = pedidoEndereco.Localidade.CodigoLocalidadeEmbarcador;
            }

            if (string.IsNullOrWhiteSpace(clienteCTE.Numero))
                clienteCTE.Numero = "S/N";

            if (!string.IsNullOrWhiteSpace(clienteCTE.Complemento) && clienteCTE.Complemento.Length > 60)
                clienteCTE.Complemento = clienteCTE.Complemento.Substring(0, 59);

            return clienteCTE;
        }

        public Dominio.ObjetosDeValor.RetornoVerificacaoCliente converterClienteEmbarcador(Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador, string tipoCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente veririfcacaoCliente = new Dominio.ObjetosDeValor.RetornoVerificacaoCliente();
            StringBuilder st = new StringBuilder();
            try
            {
                if (clienteEmbarcador != null)
                {
                    if (!string.IsNullOrWhiteSpace(clienteEmbarcador.CPFCNPJ))
                    {
                        double cpfCnpj = 0f;
                        clienteEmbarcador.CPFCNPJ = Utilidades.String.OnlyNumbers(clienteEmbarcador.CPFCNPJ);

                        clienteEmbarcador.CPFCNPJ = string.IsNullOrWhiteSpace(clienteEmbarcador.CPFCNPJ) ? "0" : clienteEmbarcador.CPFCNPJ;

                        if (double.TryParse(clienteEmbarcador.CPFCNPJ, out cpfCnpj) && ((clienteEmbarcador.CPFCNPJ.Length == 11 || clienteEmbarcador.CPFCNPJ.Length == 14 || configuracaoTMS.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior) || clienteEmbarcador.Exportacao))
                        {
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                            Repositorio.Embarcador.Pessoas.CategoriaPessoa repCategoriaPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
                            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                            bool inserir = false;

                            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                            if (clienteEmbarcador.Exportacao && cliente == null)
                            {
                                cliente = repCliente.BuscarPorNomeEndereco(clienteEmbarcador.RazaoSocial, clienteEmbarcador.Endereco);

                                if (cliente == null)
                                {
                                    Repositorio.Embarcador.Pessoas.PessoaExteriorOutraDescricao repPessoaExteriorOutraDescricao = new Repositorio.Embarcador.Pessoas.PessoaExteriorOutraDescricao(unitOfWork);

                                    cliente = repPessoaExteriorOutraDescricao.BuscarPessoaPorRazaoSocialEEndereco(clienteEmbarcador.RazaoSocial, clienteEmbarcador.Endereco);
                                }

                                if (cliente != null)
                                {
                                    clienteEmbarcador.CPFCNPJ = cliente.CPF_CNPJ.ToString();
                                    cpfCnpj = cliente.CPF_CNPJ;
                                }
                            }

                            Dominio.Entidades.Atividade atividade = repAtividade.BuscarPorCodigo(clienteEmbarcador.CodigoAtividade);

                            if (atividade == null)
                                st.Append(string.Concat("Código da Atividade " + clienteEmbarcador.CodigoAtividade + " informado não foi localizado. Favor verificar!"));

                            if (cliente == null)
                            {
                                cliente = new Dominio.Entidades.Cliente();
                                inserir = true;
                                cliente.Atividade = atividade;
                            }
                            else
                            {
                                if (cliente.NaoAtualizarDados || clienteEmbarcador.NaoAtualizarDadosCadastrais)
                                {
                                    veririfcacaoCliente.cliente = cliente;
                                    veririfcacaoCliente.Mensagem = string.Empty;
                                    veririfcacaoCliente.Status = true;
                                    return veririfcacaoCliente;
                                }

                                if (cliente.Atividade != atividade)
                                    cliente.Atividade = atividade;
                            }

                            if (string.IsNullOrWhiteSpace(cliente.CodigoIntegracao))
                            {
                                if (clienteEmbarcador.CodigoCliente != null && !string.IsNullOrWhiteSpace(clienteEmbarcador.CodigoCliente.Trim()))
                                    cliente.CodigoIntegracao = clienteEmbarcador.CodigoCliente;
                            }

                            if (!clienteEmbarcador.Exportacao)
                            {
                                cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(clienteEmbarcador.CodigoIBGECidade);
                                cliente.Tipo = (Utilidades.String.OnlyNumbers(clienteEmbarcador.CPFCNPJ).Length == 14 || clienteEmbarcador.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior) ? "J" : "F";
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(clienteEmbarcador.CodigoLocalidadeEmbarcador))
                                {
                                    Dominio.Entidades.Localidade localidadeExt = repLocalidade.buscarPorCodigoEmbarcador(clienteEmbarcador.CodigoLocalidadeEmbarcador);
                                    if (localidadeExt == null)
                                    {
                                        localidadeExt = new Dominio.Entidades.Localidade();
                                        localidadeExt.CEP = "";
                                        localidadeExt.Codigo = repLocalidade.BuscarPorMaiorCodigo();
                                        localidadeExt.Codigo++;
                                        localidadeExt.Descricao = clienteEmbarcador.Cidade;
                                        localidadeExt.Estado = new Dominio.Entidades.Estado() { Sigla = "EX" };
                                        localidadeExt.CodigoIBGE = 9999999;
                                        localidadeExt.CodigoLocalidadeEmbarcador = clienteEmbarcador.CodigoLocalidadeEmbarcador;
                                        if (!string.IsNullOrWhiteSpace(clienteEmbarcador.CodigoPais))
                                        {
                                            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                                            localidadeExt.Pais = repPais.BuscarPorCodigo(int.Parse(clienteEmbarcador.CodigoPais));
                                            if (localidadeExt.Pais == null)
                                            {
                                                st.Append(string.Concat("Código do país do ", tipoCliente, " (" + clienteEmbarcador.CodigoPais + ") não foi localizado no Multi Embarcador; "));
                                            }
                                        }
                                        repLocalidade.Inserir(localidadeExt);
                                    }
                                    cliente.Localidade = localidadeExt;
                                }
                                else
                                {
                                    cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(9999999);
                                }
                                cliente.Tipo = "E";
                                cliente.Cidade = clienteEmbarcador.Cidade;
                            }

                            cliente.Bairro = clienteEmbarcador.Bairro;
                            cliente.CEP = clienteEmbarcador.CEP;
                            cliente.Complemento = clienteEmbarcador.Complemento;
                            cliente.Endereco = clienteEmbarcador.Endereco;
                            cliente.Numero = clienteEmbarcador.Numero;
                            if (string.IsNullOrWhiteSpace(cliente.Numero) || cliente.Numero.Length < 2)
                                cliente.Numero = "S/N";


                            cliente.Telefone1 = clienteEmbarcador.Telefone1;
                            cliente.Telefone2 = clienteEmbarcador.Telefone2;

                            if (!string.IsNullOrWhiteSpace(clienteEmbarcador.Latitude))
                                cliente.Latitude = clienteEmbarcador.Latitude;

                            if (!string.IsNullOrWhiteSpace(clienteEmbarcador.Longitude))
                                cliente.Longitude = clienteEmbarcador.Longitude;

                            cliente.IndicadorIE = clienteEmbarcador.CodigoIndicadorIE;
                            cliente.CPF_CNPJ = cpfCnpj;
                            cliente.Email = !string.IsNullOrWhiteSpace(clienteEmbarcador.Emails) ? clienteEmbarcador.Emails : cliente.Email;
                            cliente.EmailContador = !string.IsNullOrWhiteSpace(clienteEmbarcador.EmailsContador) ? clienteEmbarcador.EmailsContador : cliente.EmailContador;
                            cliente.EmailContato = !string.IsNullOrWhiteSpace(clienteEmbarcador.EmailsContato) ? clienteEmbarcador.EmailsContato : cliente.EmailContato;
                            cliente.EmailContadorStatus = clienteEmbarcador.StatusEmailsContador ? "A" : "I";
                            cliente.EmailContatoStatus = clienteEmbarcador.StatusEmailsContato ? "A" : "I";
                            cliente.EmailStatus = clienteEmbarcador.StatusEmails ? "A" : cliente.EmailStatus == "A" ? "A" : "I";
                            cliente.IE_RG = string.IsNullOrWhiteSpace(clienteEmbarcador.RGIE) ? "ISENTO" : Utilidades.String.OnlyNumbers(clienteEmbarcador.RGIE);
                            cliente.Nome = clienteEmbarcador.RazaoSocial;
                            cliente.NomeFantasia = clienteEmbarcador.NomeFantasia;
                            cliente.Telefone2 = clienteEmbarcador.Telefone2;
                            cliente.Observacao = clienteEmbarcador.Observacao ?? string.Empty;
                            cliente.ContaFornecedorEBS = clienteEmbarcador.ContaContabil ?? string.Empty;
                            cliente.ExigeEtiquetagem = clienteEmbarcador.ExigeEtiquetagem;
                            cliente.ValorMinimoCarga = clienteEmbarcador.ValorMinimoCarga;
                            cliente.GerarPedidoBloqueado = clienteEmbarcador.GerarPedidoBloqueado;
                            cliente.ExcecaoCheckinFilaH = clienteEmbarcador.ExcecaoCheckinFilaH;
                            cliente.RaioEmMetros = clienteEmbarcador.RaioEmMetros;
                            cliente.NomeSocio = clienteEmbarcador.NomeSocio;
                            cliente.CPFSocio = clienteEmbarcador.CPFSocio;

                            cliente.EmailStatus = clienteEmbarcador.EmailStatus;
                            cliente.PISPASEP = clienteEmbarcador.PISPASEP;
                            cliente.DataNascimento = clienteEmbarcador.DataNascimento;

                            cliente.AtivarAcessoFornecedor = clienteEmbarcador.AtivarAcessoPortal;
                            cliente.VisualizarApenasParaPedidoDesteTomador = clienteEmbarcador.VisualizarApenasParaPedidoDesteTomador;

                            cliente.ExigeQueEntregasSejamAgendadas = clienteEmbarcador.ExigeQueSuasEntregasSejamAgendadas;

                            if (clienteEmbarcador.CodigoBanco > 0)
                            {
                                cliente.Banco = repBanco.BuscarPorNumero(clienteEmbarcador.CodigoBanco);
                                cliente.Agencia = clienteEmbarcador.Agencia;
                                cliente.NumeroConta = clienteEmbarcador.NumeroConta;
                                cliente.DigitoAgencia = clienteEmbarcador.Digito;
                                cliente.TipoContaBanco = clienteEmbarcador.TipoContaBanco;

                                if (cliente.Banco == null)
                                    st.Append("O banco informado não está cadastrado na base multisoftware; ");
                            }

                            if (clienteEmbarcador?.RaioEmMetros > 0)
                                cliente.TipoArea = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Raio;

                            if (clienteEmbarcador.CompartilharAcessoEntreGrupoPessoas.HasValue)
                                cliente.CompartilharAcessoEntreGrupoPessoas = clienteEmbarcador.CompartilharAcessoEntreGrupoPessoas.Value;

                            if (!string.IsNullOrWhiteSpace(clienteEmbarcador.CodigGrupoPessoa))
                            {
                                cliente.GrupoPessoas = repGrupoPessoas.BuscarPorCodigoIntegracao(clienteEmbarcador.CodigGrupoPessoa);
                                if (cliente.GrupoPessoas == null)
                                    st.Append(string.Concat("O grupo de pessoa informado para o ", tipoCliente, " não está cadastrado na base multisoftware; "));
                            }

                            if (!string.IsNullOrWhiteSpace(clienteEmbarcador.CodigoCategoria))
                            {
                                cliente.Categoria = repCategoriaPessoa.BuscarPorCodigoIntegracao(clienteEmbarcador.CodigoCategoria);
                                if (cliente.Categoria == null)
                                    st.Append(string.Concat("A categoria informada para o ", tipoCliente, " não está cadastrada na base multisoftware; "));
                            }

                            if (!clienteEmbarcador.Exportacao && clienteEmbarcador.Pais != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior)
                            {
                                if (cliente.CPF_CNPJ == 0f)
                                    st.Append(string.Concat("CPF/CNPJ do ", tipoCliente, " inválida; "));
                                else if (Utilidades.String.OnlyNumbers(clienteEmbarcador.CPFCNPJ).Length == 14)
                                {
                                    if (!Utilidades.Validate.ValidarCNPJ(clienteEmbarcador.CPFCNPJ) && configuracaoTMS.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Brasil)
                                    {
                                        string cpf = Utilidades.String.OnlyNumbers(clienteEmbarcador.CPFCNPJ).ToLong().ToString("d11");
                                        if (!Utilidades.Validate.ValidarCPF(cpf))
                                            st.Append(string.Concat("CPF/CNPJ do ", tipoCliente, " inválida; "));
                                        else
                                            cliente.Tipo = "F";
                                    }
                                }
                                else
                                {
                                    string cpf = Utilidades.String.OnlyNumbers(clienteEmbarcador.CPFCNPJ).ToLong().ToString("d11");
                                    if (!Utilidades.Validate.ValidarCPF(cpf) && configuracaoTMS.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Brasil)
                                        st.Append(string.Concat("CPF/CNPJ do ", tipoCliente, " inválida; "));
                                }
                            }

                            //if (cliente.CPF_CNPJ == 0f || (!(Utilidades.String.OnlyNumbers(clienteEmbarcador.CPFCNPJ).Length == 14 ? Utilidades.Validate.ValidarCNPJ(clienteEmbarcador.CPFCNPJ) : Utilidades.Validate.ValidarCPF(clienteEmbarcador.CPFCNPJ)) && !clienteEmbarcador.Exportacao))
                            //    st.Append(string.Concat("CPF/CNPJ do ", tipoCliente, " inválida; "));

                            if (cliente.Atividade == null)
                                st.Append(string.Concat("Atividade do ", tipoCliente, " inválida; "));

                            if (string.IsNullOrWhiteSpace(clienteEmbarcador.Bairro))
                            {
                                st.Append(string.Concat("Bairro do ", tipoCliente, " inválido; "));
                            }
                            else
                            {
                                if (clienteEmbarcador.Bairro.Trim().Length < 2)
                                {
                                    st.Append(string.Concat("Bairro do ", tipoCliente, " deve contér mais que 2 caracteres; "));
                                }
                            }

                            if (string.IsNullOrWhiteSpace(clienteEmbarcador.Endereco))
                            {
                                st.Append(string.Concat("Endereço do ", tipoCliente, " inválido; "));
                            }
                            else
                            {
                                if (clienteEmbarcador.Endereco.Length < 2)
                                {
                                    st.Append(string.Concat("Endereço do ", tipoCliente, " deve contér mais que 2 caracteres; "));
                                }
                            }

                            if (string.IsNullOrWhiteSpace(clienteEmbarcador.Numero))
                            {
                                st.Append(string.Concat("Número do endereço do ", tipoCliente, " inválido; "));
                            }

                            if (string.IsNullOrWhiteSpace(clienteEmbarcador.RazaoSocial))
                            {
                                st.Append(string.Concat("Nome/Razão Social do ", tipoCliente, " inválido; "));
                            }
                            else
                            {
                                if (clienteEmbarcador.RazaoSocial.Length < 2)
                                {
                                    st.Append(string.Concat("Nome/Razão Social do ", tipoCliente, " deve contér mais que 2 caracteres; "));
                                }

                            }

                            if (cliente.Localidade == null)
                                st.Append(string.Concat("Localidade do ", tipoCliente, " inválida; "));

                            if (!string.IsNullOrWhiteSpace(cliente.Email))
                            {
                                var emails = cliente.Email.Split(';');
                                foreach (string email in emails)
                                {
                                    if (!string.IsNullOrWhiteSpace(email))
                                    {
                                        if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                            st.Append(string.Concat("E-mail (", email, ") do ", tipoCliente, " inválido; "));
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(cliente.EmailContador))
                            {
                                var emails = cliente.EmailContador.Split(';');
                                foreach (string email in emails)
                                    if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                        st.Append(string.Concat("E-mail do contador (", email, ") do ", tipoCliente, " inválido; "));
                            }

                            if (!string.IsNullOrWhiteSpace(cliente.EmailContato))
                            {
                                var emails = cliente.EmailContato.Split(';');
                                foreach (string email in emails)
                                    if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                        st.Append(string.Concat("E-mail do contato (", email, ") do ", tipoCliente, " inválido; "));
                            }

                            if (st.Length > 0)
                            {
                                Servicos.Log.TratarErro("Cliente " + tipoCliente + ". Dados: " + Newtonsoft.Json.JsonConvert.SerializeObject(clienteEmbarcador));
                                Servicos.Log.TratarErro("Mensagem " + st.ToString());
                                veririfcacaoCliente.cliente = null;
                                veririfcacaoCliente.Status = false;
                                veririfcacaoCliente.Mensagem = st.ToString();
                            }
                            else
                            {
                                if (inserir)
                                {
                                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                                    {
                                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                                        if (grupoPessoas != null)
                                        {
                                            cliente.GrupoPessoas = grupoPessoas;
                                        }
                                    }
                                    cliente.Ativo = true;

                                    if (string.IsNullOrEmpty(cliente.Latitude) || string.IsNullOrEmpty(cliente.Longitude))
                                        cliente.GeoLocalizacaoStatus = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.NaoGerado;
                                    else
                                        cliente.GeoLocalizacaoStatus = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.Gerado;

                                    cliente.DataCadastro = DateTime.Now;
                                    repCliente.Inserir(cliente);
                                }
                                else
                                {
                                    if ((string.IsNullOrWhiteSpace(clienteEmbarcador.Latitude) || string.IsNullOrWhiteSpace(clienteEmbarcador.Longitude)) && cliente.GeoLocalizacaoStatus != Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.Gerado)
                                    {
                                        cliente.GeoLocalizacaoStatus = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.NaoGerado;
                                        cliente.Latitude = string.Empty;
                                        cliente.Longitude = string.Empty;
                                    }
                                    cliente.DataUltimaAtualizacao = DateTime.Now;
                                    cliente.Integrado = false;
                                    repCliente.Atualizar(cliente);
                                }

                                PreencherDadosCliente(cliente, unitOfWork, clienteEmbarcador.TipoCliente);
                                PreencherDadosFornecedor(cliente, unitOfWork, clienteEmbarcador.TipoFornecedor);
                                PreencherDadosTransportadorTerceiro(cliente, unitOfWork, clienteEmbarcador.TipoTransportador, clienteEmbarcador);
                                VincularClientePorEmpresa(cliente, clienteEmbarcador.CodigoEmpresa, unitOfWork);
                                PreencherContatosPessoa(cliente, clienteEmbarcador, unitOfWork);
                                PreencherEmailSecundarioPessoa(cliente, clienteEmbarcador, unitOfWork);
                                PreencherDadosDescarga(cliente, clienteEmbarcador, unitOfWork);

                                veririfcacaoCliente.cliente = cliente;
                                veririfcacaoCliente.Mensagem = "";
                                veririfcacaoCliente.Status = true;
                            }
                        }
                        else
                        {
                            st.Append(string.Concat("Falha ao obter CPF/CNPJ do ", tipoCliente, "; "));
                            veririfcacaoCliente.cliente = null;
                            veririfcacaoCliente.Status = false;
                            veririfcacaoCliente.Mensagem = st.ToString();
                        }
                    }
                    else
                    {
                        st.Append(string.Concat("CPF/CNPJ do ", tipoCliente, " não foi informado; "));
                        veririfcacaoCliente.cliente = null;
                        veririfcacaoCliente.Status = false;
                        veririfcacaoCliente.Mensagem = st.ToString();
                    }
                }
                else
                {
                    st.Append(string.Concat("Os dados do ", tipoCliente, " não foram informados (todos os dados do " + tipoCliente + " estão nulos);"));
                    veririfcacaoCliente.cliente = null;
                    veririfcacaoCliente.Status = false;
                    veririfcacaoCliente.Mensagem = st.ToString();
                }

                return veririfcacaoCliente;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Exceção Cliente " + tipoCliente + ". Dados: " + Newtonsoft.Json.JsonConvert.SerializeObject(clienteEmbarcador));
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        public Dominio.ObjetosDeValor.RetornoVerificacaoCliente ConverterObjetoValorPessoa(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, string tipoCliente, Repositorio.UnitOfWork unitOfWork, int empresa = 0, bool atualizarEmail = true, bool validarCPFPrimeiro = false, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null, bool usarOutroEnderecoCliente = false, bool integracaoViaWS = false, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = null, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null, bool metodoSalvarCliente = false)
        {
            if (cacheObjetoValorCTe == null)
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente veririfcacaoCliente = new Dominio.ObjetosDeValor.RetornoVerificacaoCliente();
            StringBuilder st = new StringBuilder();

            try
            {
                Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                if (pessoa == null)
                {
                    st.Append(string.Concat("Os dados do ", tipoCliente, " não foram informados (todos os dados do " + tipoCliente + " estão nulos);"));
                    veririfcacaoCliente.cliente = null;
                    veririfcacaoCliente.Status = false;
                    veririfcacaoCliente.Mensagem = st.ToString();

                    return veririfcacaoCliente;
                }

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa repConfiguracaoPessoa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPessoa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao(cacheObjetoValorCTe.ConfiguracaoTMS);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa configuracaoPessoa = repConfiguracaoPessoa.BuscarConfiguracaoPadrao();

                if ((tipoServicoMultisoftware.HasValue && tipoServicoMultisoftware.Value == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !string.IsNullOrEmpty(pessoa.CodigoIntegracao)
                    && (tipoCliente == "Expedidor" || tipoCliente == "Recebedor" || tipoCliente == "Remetente" || tipoCliente == "Destinatario" || tipoCliente == "Destinatário")) || (string.IsNullOrWhiteSpace(pessoa.CPFCNPJ)
                        && !string.IsNullOrWhiteSpace(pessoa.CodigoIntegracao)))
                {
                    Dominio.Entidades.Cliente clienteCodigoIntegracao = repCliente.BuscarPorCodigoIntegracao(pessoa.CodigoIntegracao, cacheObjetoValorCTe.lstCacheIndexClientes);
                    if (clienteCodigoIntegracao != null && pessoa.GrupoPessoa != null)
                        AtualizarGrupoPessoa(clienteCodigoIntegracao, pessoa.GrupoPessoa, auditado, unitOfWork, cacheObjetoValorCTe.lstGrupoPessoas);

                    if (clienteCodigoIntegracao != null)
                    {
                        veririfcacaoCliente.cliente = clienteCodigoIntegracao;
                        veririfcacaoCliente.Mensagem = "";
                        veririfcacaoCliente.Status = true;
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco = repClienteOutroEndereco.BuscarClientePorCodigoIntegracao(pessoa.CodigoIntegracao, cacheObjetoValorCTe.lstClienteOutroEndereco);
                        if (clienteOutroEndereco?.Cliente != null)
                        {
                            clienteCodigoIntegracao = clienteOutroEndereco.Cliente;
                            veririfcacaoCliente.cliente = clienteCodigoIntegracao;
                            veririfcacaoCliente.clienteOutroEndereco = clienteOutroEndereco;
                            veririfcacaoCliente.Mensagem = "";
                            veririfcacaoCliente.Status = true;
                            veririfcacaoCliente.UsarOutroEndereco = true;
                        }
                        else
                        {
                            double cpfCnpj = 0f;
                            pessoa.CPFCNPJ = Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ);
                            pessoa.CPFCNPJ = string.IsNullOrWhiteSpace(pessoa.CPFCNPJ) ? "0" : pessoa.CPFCNPJ;

                            if (double.TryParse(pessoa.CPFCNPJ, out cpfCnpj) && ((pessoa.CPFCNPJ.Length == 11 || pessoa.CPFCNPJ.Length == 14 || pessoa.CPFCNPJ.Length == 13 && repCliente.ExistePorCPFCNPJ(cpfCnpj) || pessoa.ClienteExterior) || configuracao.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior))
                            {
                                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = this.CadastrarCliente(ref st, cpfCnpj, tipoCliente, pessoa, unitOfWork, configuracao, configuracaoPessoa, empresa, atualizarEmail, validarCPFPrimeiro, auditado, usarOutroEnderecoCliente, tipoServicoMultisoftware, integracaoViaWS, null, cacheObjetoValorCTe, objetoValorPersistente, metodoSalvarCliente);

                                if (retorno.cliente != null)
                                    PreencherDadosObjetoDeValorIntegracao(retorno.cliente, pessoa, unitOfWork, cacheObjetoValorCTe);

                                return retorno;
                            }
                            else
                            {
                                st.Append(string.Concat("Falha ao obter CPF/CNPJ do ", tipoCliente, "; "));
                                veririfcacaoCliente.cliente = null;
                                veririfcacaoCliente.Status = false;
                                veririfcacaoCliente.Mensagem = st.ToString();
                            }
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(pessoa.CPFCNPJ) || pessoa.ClienteExterior || configuracao.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior)
                {
                    double cpfCnpj = 0f;
                    pessoa.CPFCNPJ = Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ);

                    pessoa.CPFCNPJ = string.IsNullOrWhiteSpace(pessoa.CPFCNPJ) ? "0" : pessoa.CPFCNPJ;

                    if (double.TryParse(pessoa.CPFCNPJ, out cpfCnpj) && ((pessoa.CPFCNPJ.Length == 11 || pessoa.CPFCNPJ.Length == 14 || pessoa.CPFCNPJ.Length == 13 && repCliente.ExistePorCPFCNPJ(cpfCnpj) || pessoa.ClienteExterior) || configuracao.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior))
                    {
                        Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = this.CadastrarCliente(ref st, cpfCnpj, tipoCliente, pessoa, unitOfWork, configuracao, configuracaoPessoa, empresa, atualizarEmail, validarCPFPrimeiro, auditado, usarOutroEnderecoCliente, tipoServicoMultisoftware, integracaoViaWS, unitOfWorkAdmin, cacheObjetoValorCTe, objetoValorPersistente, metodoSalvarCliente);

                        if (retorno.cliente != null)
                            PreencherDadosObjetoDeValorIntegracao(retorno.cliente, pessoa, unitOfWork, cacheObjetoValorCTe, objetoValorPersistente);

                        return retorno;
                    }
                    else
                    {
                        st.Append(string.Concat("Falha ao obter CPF/CNPJ do ", tipoCliente, "; "));
                        veririfcacaoCliente.cliente = null;
                        veririfcacaoCliente.Status = false;
                        veririfcacaoCliente.Mensagem = st.ToString();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(pessoa.CodigoIntegracao))
                    {
                        Dominio.Entidades.Cliente clienteCodigoIntegracao = repCliente.BuscarPorCodigoIntegracao(pessoa.CodigoIntegracao);
                        if (clienteCodigoIntegracao != null)
                        {
                            veririfcacaoCliente.cliente = clienteCodigoIntegracao;
                            veririfcacaoCliente.Mensagem = "";
                            veririfcacaoCliente.Status = true;
                        }
                        else
                        {
                            st.Append(string.Concat("Não foi encontrado nenhuma pessoa cadastrada com o código de integração ", pessoa.CodigoIntegracao, " na base multisoftware;"));
                            veririfcacaoCliente.cliente = null;
                            veririfcacaoCliente.Status = false;
                            veririfcacaoCliente.Mensagem = st.ToString();
                        }
                    }
                    else
                    {
                        st.Append(string.Concat("CPF/CNPJ do ", tipoCliente, " não foi informado; "));
                        veririfcacaoCliente.cliente = null;
                        veririfcacaoCliente.Status = false;
                        veririfcacaoCliente.Mensagem = st.ToString();
                    }

                }

                if (veririfcacaoCliente.cliente != null)
                    PreencherDadosObjetoDeValorIntegracao(veririfcacaoCliente.cliente, pessoa, unitOfWork, cacheObjetoValorCTe);

                return veririfcacaoCliente;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Exceção Cliente " + tipoCliente + ". Dados: " + Newtonsoft.Json.JsonConvert.SerializeObject(pessoa));
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        private void AtualizarGrupoPessoa(Dominio.Entidades.Cliente cliente, GrupoPessoa grupoPessoa, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> lstGrupoPessoas = null)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoCliente = repGrupoPessoa.BuscarPorCodigoIntegracao(grupoPessoa.CodigoIntegracao, lstGrupoPessoas);

            if (grupoCliente == null)
                return;

            if (cliente.GrupoPessoas == grupoCliente)
                return;

            cliente.GrupoPessoas = grupoCliente;
            repCliente.Atualizar(cliente, auditado);
        }


        public Dominio.ObjetosDeValor.RetornoVerificacaoCliente ConverterParaTransportadorTerceiro(Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresa, string tipoCliente, Repositorio.UnitOfWork unitOfWork, bool setarEmails = false, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe CacheObjetoValor = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null, bool naoAtualizarDadosNaIntegracaoMercadoLivre = false)
        {
            if (CacheObjetoValor == null)
                CacheObjetoValor = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente veririfcacaoCliente = new Dominio.ObjetosDeValor.RetornoVerificacaoCliente();
            StringBuilder st = new StringBuilder();
            try
            {
                if (empresa == null)
                {
                    st.Append(string.Concat("Os dados do ", tipoCliente, " não foram informados (todos os dados do " + tipoCliente + " estão nulos);"));
                    veririfcacaoCliente.cliente = null;
                    veririfcacaoCliente.Status = false;
                    veririfcacaoCliente.Mensagem = st.ToString();
                    return veririfcacaoCliente;
                }

                string cnpj = empresa.CNPJ?.Replace(".", "").Replace("/", "").Replace("-", "");
                if (string.IsNullOrWhiteSpace(cnpj))
                {
                    st.Append(string.Concat("CPF/CNPJ do ", tipoCliente, " não foi informado; "));
                    veririfcacaoCliente.cliente = null;
                    veririfcacaoCliente.Status = false;
                    veririfcacaoCliente.Mensagem = st.ToString();
                    return veririfcacaoCliente;
                }

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repositorioConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repositorioConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

                bool inserir = false;

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(cnpj), CacheObjetoValor);

                if (cliente == null)
                {
                    cliente = new Dominio.Entidades.Cliente();
                    cliente.DataCadastro = DateTime.Now;
                    inserir = true;
                }
                else if (cliente.NaoAtualizarDados || naoAtualizarDadosNaIntegracaoMercadoLivre)
                {
                    veririfcacaoCliente.cliente = cliente;
                    veririfcacaoCliente.Mensagem = "";
                    veririfcacaoCliente.Status = true;
                    return veririfcacaoCliente;
                }
                else
                {
                    if (auditado != null)
                        cliente.Initialize();
                }

                if (empresa.Endereco != null)
                {
                    if (empresa.Endereco.Cidade != null)
                    {
                        Dominio.Entidades.Localidade localidade = null;

                        if (empresa.Endereco.Cidade.IBGE > 0)
                            localidade = repLocalidade.BuscarPorCodigoIBGE(empresa.Endereco.Cidade.IBGE, CacheObjetoValor.lstLocalidades);
                        else
                        {
                            localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(empresa.Endereco.Cidade.Descricao), empresa.Endereco.Cidade.SiglaUF, CacheObjetoValor.lstLocalidades);
                        }

                        if (localidade != null)
                            cliente.Localidade = localidade;

                        if (cliente.Localidade != null)
                        {
                            cliente.Bairro = empresa.Endereco.Bairro;
                            cliente.CEP = empresa.Endereco.CEP;
                            cliente.Complemento = empresa.Endereco.Complemento;
                            cliente.Endereco = empresa.Endereco.Logradouro;
                            cliente.Numero = !string.IsNullOrEmpty(empresa.Endereco.Numero) ? empresa.Endereco.Numero : "S/N";
                            string telefone = empresa.Endereco.DDDTelefone + empresa.Endereco.Telefone;
                            cliente.Telefone1 = !string.IsNullOrWhiteSpace(telefone) ? telefone : !string.IsNullOrWhiteSpace(cliente.Telefone1) ? cliente.Telefone1 : "";
                            cliente.CPF_CNPJ = double.Parse(cnpj);
                            if (inserir)
                            {
                                cliente.Email = "";
                                cliente.EmailStatus = "I";
                                cliente.EmailContador = "";
                                cliente.EmailContato = "";
                                cliente.EmailContadorStatus = "I";
                                cliente.EmailContatoStatus = "I";
                            }

                            cliente.IE_RG = string.IsNullOrWhiteSpace(empresa.IE) ? "ISENTO" : Utilidades.String.OnlyNumbers(empresa.IE);
                            cliente.Nome = empresa.RazaoSocial;
                            cliente.NomeFantasia = empresa.NomeFantasia;
                            if (empresa.RegimeTributario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.NaoInformado)
                                cliente.RegimeTributario = empresa.RegimeTributario;
                            if (inserir)
                                if (cnpj.Length == 11)
                                {
                                    cliente.Tipo = "F";
                                    cliente.IE_RG = "ISENTO";
                                }
                                else
                                    cliente.Tipo = "J";
                        }
                        else
                        {
                            st.Append(string.Concat("Não foi possível encontrar a localidade do transportador " + empresa.RazaoSocial + ", por favor, cadastre o transportador manualmente e tente novamente."));
                        }
                    }
                    else
                    {
                        st.Append(string.Concat("É obrigatório informar a cidade do " + tipoCliente));
                    }
                }
                else
                {
                    st.Append(string.Concat("É obrigatório informar o endereço do " + tipoCliente));
                }

                if (inserir)
                    cliente.Atividade = repAtividade.BuscarPorCodigo(4);

                if (string.IsNullOrWhiteSpace(st.ToString()))
                {
                    if (inserir)
                    {
                        if (cliente.CPF_CNPJ == 0f || (cnpj.Length == 11 && !Utilidades.Validate.ValidarCPF(empresa.CNPJ)) || (cnpj.Length == 14 && !Utilidades.Validate.ValidarCNPJ(empresa.CNPJ)))
                            st.Append(string.Concat("CPF/CNPJ do ", tipoCliente, " inválida; "));

                        if (cliente.Atividade == null)
                            st.Append(string.Concat("Atividade do ", tipoCliente, " inválida; "));
                    }

                    if (string.IsNullOrWhiteSpace(cliente.Bairro))
                    {
                        st.Append(string.Concat("Bairro do ", tipoCliente, " inválido; "));
                    }
                    else
                    {
                        if (cliente.Bairro.Trim().Length < 2)
                        {
                            st.Append(string.Concat("Bairro do ", tipoCliente, " deve contér mais que 2 caracteres; "));
                        }
                    }


                    if (string.IsNullOrWhiteSpace(cliente.Endereco))
                    {
                        st.Append(string.Concat("Endereço do ", tipoCliente, " inválido; "));
                    }
                    else
                    {
                        if (cliente.Endereco.Length < 2)
                        {
                            st.Append(string.Concat("Endereço do ", tipoCliente, " deve contér mais que 2 caracteres; "));
                        }
                    }

                    if (string.IsNullOrWhiteSpace(cliente.Numero))
                    {
                        st.Append(string.Concat("Número do endereço do ", tipoCliente, " inválido; "));
                    }

                    if (string.IsNullOrWhiteSpace(cliente.Nome))
                    {
                        st.Append(string.Concat("Nome/Razão Social do ", tipoCliente, " inválido; "));
                    }
                    else
                    {
                        if (cliente.Nome.Length < 2)
                        {
                            st.Append(string.Concat("Nome/Razão Social do ", tipoCliente, " deve contér mais que 2 caracteres; "));
                        }

                    }

                    if (cliente.Localidade == null)
                        st.Append(string.Concat("Localidade do ", tipoCliente, " inválida; "));

                    if (!string.IsNullOrWhiteSpace(cliente.Email))
                    {
                        var emails = cliente.Email.Split(';');
                        foreach (string email in emails)
                        {
                            if (!string.IsNullOrEmpty(email))
                            {
                                if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                    st.Append(string.Concat("E-mail (", email, ") do ", tipoCliente, " inválido; "));
                            }
                        }

                    }

                    if (empresa.DadosBancarios != null)
                    {
                        if (empresa.DadosBancarios.Banco != null)
                        {
                            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(empresa.DadosBancarios.Banco.CodigoBanco)))
                            {
                                cliente.Banco = repBanco.BuscarPorNumero(int.Parse(Utilidades.String.OnlyNumbers(empresa.DadosBancarios.Banco.CodigoBanco)), CacheObjetoValor.lstBancos);
                                if (cliente.Banco != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(empresa.DadosBancarios.Agencia))
                                    {
                                        if (!string.IsNullOrWhiteSpace(empresa.DadosBancarios.NumeroConta))
                                        {
                                            cliente.NumeroConta = empresa.DadosBancarios.NumeroConta;
                                            cliente.Agencia = empresa.DadosBancarios.Agencia;
                                            cliente.DigitoAgencia = empresa.DadosBancarios.DigitoAgencia;
                                            cliente.TipoContaBanco = empresa.DadosBancarios.TipoContaBanco;
                                        }
                                        else
                                            st.Append("É obrigatório informar o número da conta se a conta bancária do terceiro for informada.");
                                    }
                                    else
                                        st.Append("É obrigatório informar o número da agência da conta se a conta bancária do terceiro for informada.");
                                }
                                else
                                    st.Append("Não foi encontrado nenhum banco para o código informado (" + empresa.DadosBancarios.Banco.CodigoBanco + ").");
                            }
                            else
                                st.Append("É obrigatório informar o código do banco se a conta bancária do terceiro for informada.");
                        }
                        else
                            st.Append("É obrigatório informar os dados do banco se a conta bancária do terceiro for informada.");
                    }

                    if (st.Length == 0)
                    {
                        if (inserir && cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                        {
                            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                            if (grupoPessoas != null)
                            {
                                cliente.GrupoPessoas = grupoPessoas;
                            }
                        }

                        cliente.Ativo = true;

                        if (objetoValorPersistente == null)
                        {
                            if (inserir)
                                repCliente.Inserir(cliente, auditado);
                            else if (!cliente.NaoAtualizarDados)
                                repCliente.Atualizar(cliente, auditado);
                        }
                        else
                        {
                            if (inserir)
                                objetoValorPersistente.Inserir(cliente);
                            else
                                objetoValorPersistente.Atualizar(cliente);
                        }


                        Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoa = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, cliente.CPF_CNPJ);

                        if (modalidadePessoa == null)
                        {
                            modalidadePessoa = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                            modalidadePessoa.Cliente = cliente;
                            modalidadePessoa.TipoModalidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro;

                            if (objetoValorPersistente == null)
                                repModalidadePessoas.Inserir(modalidadePessoa);
                            else
                                objetoValorPersistente.Inserir(modalidadePessoa);
                        }

                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportador = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoa.Codigo);
                        var inserirModalidadeTransportador = false;
                        if (modalidadeTransportador == null)
                        {
                            modalidadeTransportador = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas();
                            modalidadeTransportador.ModalidadePessoas = modalidadePessoa;
                            inserirModalidadeTransportador = true;
                        }

                        if (modalidadeTransportador != null)
                        {
                            modalidadeTransportador.ObservacaoCTe = "";

                            if (!string.IsNullOrWhiteSpace(empresa.RNTRC))
                                modalidadeTransportador.RNTRC = empresa.RNTRC.Length > 8 ? empresa.RNTRC.Substring(1, 8) : empresa.RNTRC.PadLeft(8, '0');

                            if (configuracaoVeiculo.SalvarTransportadorTerceiroComoGerarCIOT)
                                modalidadeTransportador.GerarCIOT = true;

                            if (empresa.TipoCIOT.HasValue)
                                modalidadeTransportador.TipoGeracaoCIOT = empresa.TipoCIOT.Value;

                            if (empresa.DadosBancarios?.PortadorConta?.TipoFavorecidoCIOT.HasValue ?? false)
                                modalidadeTransportador.TipoFavorecidoCIOT = empresa.DadosBancarios.PortadorConta.TipoFavorecidoCIOT;

                            if (inserirModalidadeTransportador)
                            {

                                if (objetoValorPersistente == null)
                                    repModalidadeTransportadoraPessoas.Inserir(modalidadeTransportador);
                                else
                                    objetoValorPersistente.Inserir(modalidadeTransportador);
                            }
                            else
                            {
                                if (objetoValorPersistente == null)
                                    repModalidadeTransportadoraPessoas.Atualizar(modalidadeTransportador);
                                else
                                    objetoValorPersistente.Atualizar(modalidadeTransportador);
                            }

                            if (empresa.DadosBancarios?.PortadorConta?.TipoPagamentoCIOT.HasValue ?? false)
                                Servicos.Cliente.SalvarTiposPagamentoCIOTPorOperadora(empresa.DadosBancarios.PortadorConta.TipoPagamentoCIOT, modalidadeTransportador, unitOfWork);

                        }
                    }
                }

                if (st.Length > 0)
                {
                    Servicos.Log.TratarErro("Cliente " + tipoCliente + ". Dados: " + Newtonsoft.Json.JsonConvert.SerializeObject(empresa));
                    Servicos.Log.TratarErro("Mensagem " + st.ToString());
                    veririfcacaoCliente.cliente = null;
                    veririfcacaoCliente.Status = false;
                    veririfcacaoCliente.Mensagem = st.ToString();
                }
                else
                {
                    veririfcacaoCliente.cliente = cliente;
                    veririfcacaoCliente.Mensagem = "";
                    veririfcacaoCliente.Status = true;
                }

                return veririfcacaoCliente;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Exceção Cliente " + tipoCliente + ". Dados: " + Newtonsoft.Json.JsonConvert.SerializeObject(empresa));
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        public bool SalvarLatitudeLongitude(Dominio.Entidades.Cliente cliente, string latitudeIntegracao, string longitudeIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (string.IsNullOrWhiteSpace(latitudeIntegracao) || string.IsNullOrWhiteSpace(longitudeIntegracao))
                return false;

            if (cliente == null)
                return false;

            string latitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(latitudeIntegracao);
            string longitude = Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(longitudeIntegracao);

            if (string.IsNullOrWhiteSpace(latitude) || string.IsNullOrWhiteSpace(longitude))
                return false;

            cliente.Latitude = latitude;
            cliente.Longitude = longitude;

            repCliente.Atualizar(cliente);

            return true;
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa SetarDadosPessoa(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, Repositorio.UnitOfWork unidadeTrabalho, bool validarCPFPrimeiro = false)
        {
            string[] splitEnderecorEmitente = pessoa.Endereco.Logradouro.Split(',');
            pessoa.Endereco.Logradouro = splitEnderecorEmitente[0].Trim();

            if (string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro) || pessoa.Endereco.Logradouro.Length <= 2)
                pessoa.Endereco.Logradouro = "NAO INFORMADO";

            if (splitEnderecorEmitente.Length > 1)
            {
                string[] splitNumero = splitEnderecorEmitente[1].Split('-');
                pessoa.Endereco.Numero = splitNumero[0].Trim().Replace("-", "");

                if (pessoa.Endereco.Numero == "0")
                    pessoa.Endereco.Numero = "S/N";
                if (splitNumero.Count() > 1)
                    pessoa.Endereco.Complemento = splitNumero[1].Trim();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(pessoa.Endereco.Numero))
                    pessoa.Endereco.Numero = "S/N";
            }

            AdminMultisoftware.Dominio.Entidades.Localidades.Endereco endereco = null;

            if (pessoa.Endereco.Bairro == null || pessoa.Endereco.Bairro.Length < 2 || string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro))
            {
                if (!string.IsNullOrWhiteSpace(pessoa.Endereco.CEP))
                {
                    AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                    endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(pessoa.Endereco.CEP)).ToString());
                    if (endereco != null)
                        pessoa.Endereco.Bairro = endereco.Bairro?.Descricao;

                    if (string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro))
                        pessoa.Endereco.Logradouro = endereco.Logradouro;
                }
            }

            if (pessoa.Endereco.Cidade.IBGE <= 0)
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorDescricaoEUF(pessoa.Endereco.Cidade.Descricao, pessoa.Endereco.Cidade.SiglaUF);

                if (localidade != null)
                {
                    pessoa.Endereco.Cidade.IBGE = localidade.CodigoIBGE;
                }
                else
                {
                    if (endereco == null)
                    {
                        AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                        endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(pessoa.Endereco.CEP)).ToString());
                    }

                    if (endereco != null)
                    {
                        int.TryParse(endereco.Localidade.CodigoIBGE, out int codigoIBGE);
                        pessoa.Endereco.Cidade.IBGE = codigoIBGE;
                    }
                }
            }

            pessoa.CPFCNPJ = Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ);
            pessoa.AtualizarEnderecoPessoa = false;

            if (pessoa.CPFCNPJ.Length >= 14)
            {
                pessoa.CPFCNPJ = pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 14);

                if (validarCPFPrimeiro)
                {
                    if (pessoa.CPFCNPJ.StartsWith("000") && Utilidades.Validate.ValidarCPF(pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 11)))
                    {
                        pessoa.CPFCNPJ = pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 11);

                        if (string.IsNullOrWhiteSpace(pessoa.RGIE))
                            pessoa.RGIE = "ISENTO";

                        pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;
                    }
                    else
                    {
                        pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                    }
                }
                else
                {
                    if (Utilidades.Validate.ValidarCNPJ(pessoa.CPFCNPJ))
                        pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                    else
                    {
                        pessoa.CPFCNPJ = pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 11);

                        if (string.IsNullOrWhiteSpace(pessoa.RGIE))
                            pessoa.RGIE = "ISENTO";

                        pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;
                    }
                }
            }
            else
                pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;

            return pessoa;
        }

        public string CadastrarCliente(ref Dominio.Entidades.Cliente cliente, Dominio.ObjetosDeValor.Cliente clienteObjeto, Repositorio.UnitOfWork unitOfWork)
        {
            if (cliente?.Codigo > 0)
                return "Error ao tentar cadastrar um novo cliente";

            double cnpjcliente = 0;
            double.TryParse(Utilidades.String.OnlyNumbers(clienteObjeto.CPFCNPJ), out cnpjcliente);

            bool tipoCpf = Utilidades.Validate.ValidarCPF(clienteObjeto.CPFCNPJ);
            bool tipoCNPJ = Utilidades.Validate.ValidarCNPJ(clienteObjeto.CPFCNPJ);

            if (!tipoCpf && !tipoCNPJ)
                return "Tipo CPF ou CNPJ formato invalido";

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);


            cliente = new Dominio.Entidades.Cliente();
            cliente.CodigoIntegracao = clienteObjeto.CodigoIntegracao;
            cliente.CPF_CNPJ = cnpjcliente;
            cliente.DataCadastro = DateTime.Now;
            cliente.AguardandoConferenciaInformacao = true;
            cliente.Integrado = false;
            cliente.DataUltimaAtualizacao = DateTime.Now;
            cliente.ValorTDE = 0;
            cliente.Tipo = tipoCNPJ ? "J" : "F";
            cliente.Nome = !string.IsNullOrEmpty(clienteObjeto.Descricao) ? clienteObjeto.Descricao : !string.IsNullOrEmpty(clienteObjeto.RazaoSocial) ? clienteObjeto.RazaoSocial : string.Empty;
            cliente.NomeFantasia = !string.IsNullOrEmpty(clienteObjeto.NomeFantasia) ? clienteObjeto.NomeFantasia : string.Empty;
            cliente.Ativo = true;

            if (clienteObjeto.SalvarEndereco)
            {
                if (clienteObjeto.Bairro != null && !clienteObjeto.Bairro.Equals(clienteObjeto?.Bairro))
                    cliente.Bairro = !string.IsNullOrWhiteSpace(clienteObjeto?.Bairro) && clienteObjeto?.Bairro.Length > 40 ? clienteObjeto?.Bairro.Substring(0, 40) : clienteObjeto?.Bairro;

                if (clienteObjeto.CEP != null && !clienteObjeto.CEP.Equals(clienteObjeto?.CEP) && string.IsNullOrWhiteSpace(clienteObjeto?.CEP))
                    cliente.CEP = clienteObjeto?.CEP ?? string.Empty;

                if (clienteObjeto.Complemento != null && !clienteObjeto.Complemento.Equals(clienteObjeto?.Complemento) && string.IsNullOrWhiteSpace(clienteObjeto?.Complemento))
                    cliente.Complemento = clienteObjeto?.Complemento ?? string.Empty;

                if (!string.IsNullOrEmpty(clienteObjeto.Endereco) && !clienteObjeto.Endereco.Equals(clienteObjeto?.Endereco))
                    cliente.Endereco = clienteObjeto?.Endereco ?? string.Empty;

                if (clienteObjeto.Localidade != null && (cliente.Localidade?.Codigo ?? 0) != clienteObjeto?.Localidade && clienteObjeto?.Localidade > 0)
                    cliente.Localidade = repositorioLocalidade.BuscarPorCodigo(clienteObjeto?.Localidade ?? 0);

                if (clienteObjeto.Numero != null && !clienteObjeto.Numero.Equals(clienteObjeto?.Numero) && string.IsNullOrWhiteSpace(clienteObjeto?.Numero))
                    cliente.Numero = clienteObjeto?.Numero ?? string.Empty;

                if (clienteObjeto.Telefone1 != null && !clienteObjeto.Telefone1.Equals(clienteObjeto?.Telefone1) && string.IsNullOrWhiteSpace(clienteObjeto?.Numero))
                    cliente.Telefone1 = clienteObjeto?.Telefone1 ?? string.Empty;
            }

            if (clienteObjeto.CodigoAtividade > 0)
                cliente.Atividade = repAtividade.BuscarPorCodigo(clienteObjeto.CodigoAtividade);
            else
                cliente.Atividade = Servicos.Atividade.ObterAtividade(0, cliente.Tipo, unitOfWork.StringConexao, 0, unitOfWork);

            if (string.IsNullOrEmpty(clienteObjeto.Endereco))
                return "Precisa Informar o endereço";
            if (string.IsNullOrWhiteSpace(clienteObjeto.Cidade))
                return "Precisa Informar a cidade";

            Dominio.Entidades.Localidade localidade = null;

            if (localidade == null && clienteObjeto.Endereco != null)
                localidade = repositorioLocalidade.BuscarPorCEP(clienteObjeto.CEP);

            if (localidade == null && clienteObjeto.Endereco != null && !string.IsNullOrWhiteSpace(clienteObjeto.Cidade))
                localidade = repositorioLocalidade.BuscarPorDescricao(clienteObjeto.Cidade)?.FirstOrDefault();

            if (localidade == null && clienteObjeto.Endereco != null && !string.IsNullOrWhiteSpace(clienteObjeto.Cidade))
                localidade = repositorioLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(clienteObjeto.Cidade), clienteObjeto.UF);

            if (localidade == null && clienteObjeto.Endereco != null && !string.IsNullOrWhiteSpace(clienteObjeto.Cidade))
                localidade = repositorioLocalidade.BuscarPorDescricao(Utilidades.String.RemoveAllSpecialCharacters(clienteObjeto.Cidade))?.FirstOrDefault();

            if (localidade == null)
                return "Localidade não encontrada para realizar cadastro, Informe CEP";

            cliente.Localidade = localidade ?? cliente.Localidade;
            repositorioCliente.Inserir(cliente);

            return "";
        }

        public List<Dominio.Entidades.Cliente> ObterFiliaisClientesRelacionadas(List<Dominio.Entidades.Cliente> clientes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            List<Dominio.Entidades.Cliente> clientesRetornar = new List<Dominio.Entidades.Cliente>();

            clientesRetornar.AddRange(clientes);

            foreach (Dominio.Entidades.Cliente cliente in clientes)
            {
                if (cliente.PossuiFilialCliente)
                {
                    clientesRetornar.AddRange(cliente.FilialCliente);
                    continue;
                }

                List<Dominio.Entidades.Cliente> clientesPai = repositorioCliente.ObterFiliaisClientesRelacionadas(cliente.CPF_CNPJ);

                clientesRetornar.AddRange(clientesPai);

                foreach (Dominio.Entidades.Cliente clientePai in clientesPai)
                    clientesRetornar.AddRange(clientePai.FilialCliente);
            }

            return clientesRetornar.Distinct().ToList();
        }

        public static void SalvarTiposPagamentoCIOTPorOperadora(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfigCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidade = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT> listaRetorno = new List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT>();
            List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT> listaRetornoDeletar = repModalidade.BuscarPorModalidadeTransportador(modalidade.Codigo);

            if (tipoPagamentoCIOT == null)
                return;

            List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> listaConfig = repConfigCIOT.BuscarAtivas();

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT> tiposPermitidosTruckPad = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT> {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Deposito,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Transferencia,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.PIX,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.BBC
            };

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT> tiposPermitidosPadrao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT> {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Deposito,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Transferencia,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao
            };

            if (listaRetornoDeletar != null)
            {
                foreach (var item in listaRetornoDeletar)
                {
                    repModalidade.Deletar(item);
                }
            }

            foreach (var config in listaConfig)
            {
                if (listaRetorno.Any(x => x.Operadora == config.OperadoraCIOT))
                    continue;

                if (config.OperadoraCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.TruckPad)
                {
                    if (tiposPermitidosTruckPad.Contains(tipoPagamentoCIOT.Value))
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT tipoPagamento = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT();
                        tipoPagamento.TipoPagamentoCIOT = tipoPagamentoCIOT.Value;
                        tipoPagamento.Operadora = config.OperadoraCIOT;
                        tipoPagamento.ModalidadeTransportadoraPessoas = modalidade;
                        listaRetorno.Add(tipoPagamento);
                        repModalidade.Inserir(tipoPagamento);
                    }
                }
                else
                {
                    if (tiposPermitidosPadrao.Contains(tipoPagamentoCIOT.Value))
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT tipoPagamento = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT();
                        tipoPagamento.TipoPagamentoCIOT = tipoPagamentoCIOT.Value;
                        tipoPagamento.Operadora = config.OperadoraCIOT;
                        tipoPagamento.ModalidadeTransportadoraPessoas = modalidade;
                        listaRetorno.Add(tipoPagamento);
                        repModalidade.Inserir(tipoPagamento);

                    }
                }
            }

            return;
        }
        #endregion

        #region Métodos Privados

        private void CriarUsuarioFornecedor(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorClienteFornecedor(pessoa.CPF_CNPJ, Dominio.Enumeradores.TipoAcesso.Fornecedor);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                usuario = repUsuario.BuscarPorCPF(pessoa.CPF_CNPJ_SemFormato);

            if (usuario != null)
                return;

            usuario = new Dominio.Entidades.Usuario
            {
                CPF = pessoa.CPF_CNPJ_SemFormato,
                ClienteFornecedor = pessoa,
                Tipo = "U",
                DataNascimento = DateTime.Today,
                DataAdmissao = DateTime.Today,
                Salario = 0,
                Login = pessoa.CPF_CNPJ_SemFormato,
                Senha = pessoa.CPF_CNPJ_SemFormato.Substring(0, 5),
                UsuarioAdministrador = true,
                Cliente = pessoa,
                Nome = pessoa.Nome,
                Telefone = pessoa.Telefone1,
                Localidade = pessoa.Localidade,
                Endereco = pessoa.Endereco,
                Complemento = pessoa.Complemento,
                Email = pessoa.Email,
                Status = "A",
                TipoAcesso = Dominio.Enumeradores.TipoAcesso.Fornecedor,
            };

            if (usuario.Setor == null)
                usuario.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };
            repUsuario.Inserir(usuario, auditado);
        }

        private Dominio.ObjetosDeValor.RetornoVerificacaoCliente CadastrarCliente(ref StringBuilder st, double cpfCnpj, string tipoCliente, Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPessoa configuracaoPessoa, int empresa = 0, bool atualizarEmail = true, bool validarCPFPrimeiro = false, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, bool inserirEnderecoNovo = false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null, bool integracaoViaWS = false, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = null, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValor = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null, bool metodoSalvarCliente = false)
        {
            if (cacheObjetoValor == null)
                cacheObjetoValor = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();
            else if (!cacheObjetoValor.Auditado && !integracaoViaWS)
                auditado = null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repCategoria = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.PessoaExteriorOutraDescricao repPessoaExteriorOutraDescricao = new Repositorio.Embarcador.Pessoas.PessoaExteriorOutraDescricao(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Repositorio.Embarcador.Localidades.MesoRegiao repMesoRegiao = new Repositorio.Embarcador.Localidades.MesoRegiao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao(cacheObjetoValor.configWebService);

            Dominio.Entidades.Embarcador.Rateio.RateioFormula rateioFormula = null;
            if (pessoa.ParametroRateioFormulaExclusivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.todos)
                rateioFormula = repRateioFormula.BuscarPorTipo(pessoa.ParametroRateioFormulaExclusivo, cacheObjetoValor.lstRateioFormula);

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente verificacaoCliente = new Dominio.ObjetosDeValor.RetornoVerificacaoCliente();

            bool inserir = false;
            bool inserirClienteOutroEndereco = false;

            Dominio.Entidades.Cliente cliente = null;
            long codigoParticipanteExterior = 0;

            if (pessoa.ClienteExterior)
            {
                long.TryParse(Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ), out codigoParticipanteExterior);
                if (codigoParticipanteExterior > 0 && Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ) != "99999999999999")
                    cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj, cacheObjetoValor);
                else
                {
                    if (!string.IsNullOrWhiteSpace(pessoa.CodigoIntegracao))
                        cliente = repCliente.BuscarPorCodigoIntegracao(pessoa.CodigoIntegracao, cacheObjetoValor.lstCacheIndexClientes);

                    string logradouro = (pessoa.Endereco != null && pessoa?.Endereco?.Logradouro != null) ? pessoa?.Endereco?.Logradouro : string.Empty;

                    if (cliente == null)
                        cliente = repCliente.BuscarPorRazaoExterior(pessoa.RazaoSocial, logradouro, cacheObjetoValor.lstCacheIndexClientes);

                    if (cliente == null)
                        cliente = repPessoaExteriorOutraDescricao.BuscarPessoaPorRazaoSocialEEndereco(pessoa.RazaoSocial, logradouro);

                    //if (cliente == null && !string.IsNullOrWhiteSpace(pessoa.CodigoDocumento))
                    //    cliente = repCliente.BuscarPorCodigoDocumentoETipo(pessoa.CodigoDocumento, "E");

                    if (cliente == null && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !integracaoViaWS)
                    {
                        verificacaoCliente.cliente = null;
                        verificacaoCliente.Status = false;
                        verificacaoCliente.Mensagem = $"Não foi encontrada uma pessoa do exterior com o nome {pessoa.RazaoSocial} e o endereço {logradouro}.";

                        return verificacaoCliente;
                    }
                }
            }
            else
                cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj, cacheObjetoValor);

            if ((configWebService?.UtilizarCodigosDeCadastroComoEnredecoSecundario ?? false) && cliente != null)
            {
                if (string.IsNullOrEmpty(pessoa.CodigoIntegracao))
                {
                    verificacaoCliente.cliente = cliente;
                    verificacaoCliente.Mensagem = string.Empty;
                    verificacaoCliente.Status = true;
                    return verificacaoCliente;
                }

                if (cliente.CodigoIntegracao != pessoa.CodigoIntegracao)
                {
                    //verifica se ja existe cliente outro endereco com cliente codigo integracao;
                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco = repClienteOutroEndereco.BuscarClientePorCodigoIntegracao(pessoa.CodigoIntegracao, cacheObjetoValor.lstClienteOutroEndereco);
                    if (clienteOutroEndereco?.Cliente != null && cliente.CPF_CNPJ == clienteOutroEndereco.Cliente.CPF_CNPJ)
                    {
                        //Adicionado por que estava sobrepondo os dados da unilever
                        if (!metodoSalvarCliente && repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever, cacheObjetoValor.lstTiposIntegracao))
                        {
                            verificacaoCliente.cliente = cliente;
                            verificacaoCliente.Mensagem = string.Empty;
                            verificacaoCliente.Status = true;
                            return verificacaoCliente;
                        }

                        cliente = clienteOutroEndereco.Cliente;
                        verificacaoCliente.clienteOutroEndereco = clienteOutroEndereco;
                    }
                    else
                    {
                        inserirClienteOutroEndereco = true;
                        inserirEnderecoNovo = true;
                    }
                }
            }

            if (cliente != null)
            {
                verificacaoCliente.CodigoDocumentoAnterior = cliente.CodigoDocumento;
                verificacaoCliente.NovoCodigoDocumento = pessoa.CodigoDocumento;

                if (cliente.NaoAtualizarDados)
                    pessoa.AtualizarEnderecoPessoa = false;
            }

            if (cliente == null)
            {
                cliente = new Dominio.Entidades.Cliente()
                {
                    DataCadastro = DateTime.Now,
                    CPF_CNPJ = cpfCnpj,
                    AguardandoConferenciaInformacao = true,
                    Integrado = false,
                    DataUltimaAtualizacao = DateTime.Now
                };

                inserir = true;

                if (pessoa.ClienteExterior)
                {
                    cliente.Tipo = "E";

                    if (codigoParticipanteExterior <= 0L || Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ) == "99999999999999")
                        cliente.CPF_CNPJ = repCliente.BuscarPorProximoExterior();
                }

                if (configWebService?.NaoSobrePorInformacoesViaIntegracao ?? false)
                    cliente.NaoAtualizarDados = true;

            }
            else
            {
                if (inserirEnderecoNovo)
                {
                    if ((pessoa.Endereco != null && pessoa.Endereco.Cidade != null && !string.IsNullOrEmpty(pessoa.Endereco.Bairro) && !string.IsNullOrEmpty(pessoa.Endereco.Numero) && !string.IsNullOrEmpty(pessoa.Endereco.Logradouro)) || inserirClienteOutroEndereco)
                    {
                        Dominio.Entidades.Localidade localidade = null;
                        if (pessoa?.Endereco?.Cidade?.IBGE > 0)
                            localidade = repLocalidade.BuscarPorCodigoIBGE(pessoa.Endereco.Cidade.IBGE, cacheObjetoValor.lstLocalidades);

                        if (localidade == null && pessoa.Endereco != null && pessoa.Endereco.Cidade != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.SiglaUF) && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.Descricao))
                            localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(pessoa.Endereco.Cidade.Descricao).ToUpper(), pessoa.Endereco.Cidade.SiglaUF.ToUpper(), cacheObjetoValor.lstLocalidades);

                        if (localidade == null && pessoa.Endereco != null)
                            localidade = repLocalidade.BuscarPorCEP(pessoa.Endereco.CEP, cacheObjetoValor.lstLocalidades);

                        if (localidade == null && pessoa.ClienteExterior)
                            localidade = repLocalidade.BuscarPorCodigoIBGE(9999999, cacheObjetoValor.lstLocalidades);

                        Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco outroEndereco = repClienteOutroEndereco.BuscarPorIE(pessoa.RGIE, pessoa.Endereco, cpfCnpj, cacheObjetoValor.lstClienteOutroEndereco);

                        if (localidade == null)
                        {
                            st.Append(string.Concat("Não foi possível localizar a cidade enviada; "));
                            verificacaoCliente.cliente = null;
                            verificacaoCliente.Status = false;
                            verificacaoCliente.Mensagem = st.ToString();

                            return verificacaoCliente;
                        }

                        if (outroEndereco == null || inserirClienteOutroEndereco || CompararEnderecos(outroEndereco, pessoa.Endereco, localidade))
                            outroEndereco = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco();

                        outroEndereco.Bairro = pessoa.Endereco.Bairro;
                        outroEndereco.CEP = pessoa.Endereco.CEP;
                        outroEndereco.Cliente = cliente;
                        outroEndereco.CodigoEmbarcador = pessoa.CodigoIntegracao;
                        outroEndereco.Complemento = pessoa.Endereco.Complemento;
                        outroEndereco.Endereco = pessoa.Endereco.Logradouro;
                        outroEndereco.EnderecoDigitado = true;
                        outroEndereco.IE_RG = pessoa.RGIE;
                        outroEndereco.Numero = pessoa.Endereco.Numero;
                        outroEndereco.Telefone = pessoa.Endereco.Telefone;
                        outroEndereco.TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Outros;
                        outroEndereco.TipoLogradouro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Outros;
                        outroEndereco.Localidade = localidade;
                        if (!string.IsNullOrWhiteSpace(pessoa.CodigoDocumento))
                            outroEndereco.CodigoDocumento = pessoa.CodigoDocumento;

                        if (outroEndereco.Codigo > 0)
                            if (objetoValorPersistente == null)
                            {
                                repClienteOutroEndereco.Atualizar(outroEndereco, auditado);
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cliente, null, "Atualizou endereço secundário " + outroEndereco.Descricao + ".", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                            }
                            else
                                objetoValorPersistente.Atualizar(outroEndereco);
                        else
                        {
                            if (objetoValorPersistente == null)
                            {
                                repClienteOutroEndereco.Inserir(outroEndereco, auditado);
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cliente, null, "Adicionou endereço secundário " + outroEndereco.Descricao + ".", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Insert);
                            }
                            else
                                objetoValorPersistente.Inserir(outroEndereco);
                        }

                        verificacaoCliente.UsarOutroEndereco = true;
                        verificacaoCliente.CodigoOutroEndereco = pessoa.CodigoIntegracao;
                        verificacaoCliente.clienteOutroEndereco = outroEndereco;
                    }

                    verificacaoCliente.cliente = cliente;
                    verificacaoCliente.Mensagem = string.Empty;
                    verificacaoCliente.Status = true;
                }
                else if (cliente.NaoAtualizarDados)
                {
                    verificacaoCliente.cliente = cliente;
                    verificacaoCliente.Mensagem = string.Empty;
                    verificacaoCliente.Status = true;
                    return verificacaoCliente;
                }

                if (auditado != null)
                    cliente.Initialize();
            }

            bool buscouGrupo = false;

            if (inserir || pessoa.AtualizarEnderecoPessoa)
            {
                if (pessoa.TipoPessoa == Dominio.Enumeradores.TipoPessoa.Fisica && pessoa.CPFCNPJ.Length <= 11 || pessoa.ValidarCPFPrimeiro)
                    validarCPFPrimeiro = true;

                if (!pessoa.ClienteExterior && cliente.Tipo != "E" && configuracao.Pais != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior)
                {
                    if (validarCPFPrimeiro)
                    {
                        if (Utilidades.Validate.ValidarCPF(pessoa.CPFCNPJ.Length > 11 ? pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 11, 11) : pessoa.CPFCNPJ))
                            cliente.Tipo = "F";
                        else if (Utilidades.Validate.ValidarCNPJ(pessoa.CPFCNPJ))
                            cliente.Tipo = "J";
                        else
                            cliente.Tipo = Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ).Length == 14 ? "J" : "F";
                    }
                    else
                    {
                        if (Utilidades.Validate.ValidarCNPJ(pessoa.CPFCNPJ))
                            cliente.Tipo = "J";
                        else if (Utilidades.Validate.ValidarCPF(pessoa.CPFCNPJ.Length > 11 ? pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 11, 11) : pessoa.CPFCNPJ))
                            cliente.Tipo = "F";
                        else
                            cliente.Tipo = Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ).Length == 14 ? "J" : "F";
                    }
                }

                else
                {
                    switch (pessoa.TipoPessoa)
                    {
                        case Dominio.Enumeradores.TipoPessoa.Fisica:
                            cliente.Tipo = "F";
                            break;
                        case Dominio.Enumeradores.TipoPessoa.Juridica:
                            cliente.Tipo = "J";
                            break;
                    }
                }

                if (!inserirEnderecoNovo || inserir)
                {
                    if (pessoa.Endereco != null)
                    {
                        if (pessoa.Endereco.Cidade != null || pessoa.ClienteExterior)
                        {
                            bool alterouCidade = false;
                            string cep = Utilidades.String.OnlyNumbers(pessoa.Endereco.CEP);

                            if (!pessoa.ClienteExterior)
                            {
                                Dominio.Entidades.Localidade localidade = null;

                                if (pessoa.Endereco.Cidade.IBGE != 0)
                                    localidade = repLocalidade.BuscarPorCodigoIBGE(pessoa.Endereco.Cidade.IBGE, cacheObjetoValor.lstLocalidades);

                                if (localidade == null && pessoa.Endereco.Cidade != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.SiglaUF) && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.Descricao))
                                    localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(pessoa.Endereco.Cidade.Descricao).ToUpper(), pessoa.Endereco.Cidade.SiglaUF.ToUpper(), cacheObjetoValor.lstLocalidades);

                                if (localidade == null && !string.IsNullOrWhiteSpace(cep))
                                    localidade = repLocalidade.BuscarPorCEP(cep, cacheObjetoValor.lstLocalidades);

                                if (localidade == null && unitOfWorkAdmin != null && !string.IsNullOrEmpty(cep))
                                {
                                    Servicos.Embarcador.Localidades.Localidade svcLocalidade = new Servicos.Embarcador.Localidades.Localidade(unitOfWork);
                                    dynamic endereco = svcLocalidade.BuscarEnderecoPorCEP(cep, unitOfWork, unitOfWorkAdmin);
                                    if (endereco != null)
                                        localidade = repLocalidade.BuscarPorCodigo(endereco.CodigoCidade, cacheObjetoValor.lstLocalidades);
                                }

                                if (cliente.Localidade != localidade)
                                    alterouCidade = true;

                                if (cliente.Localidade == null || !cliente.NaoAtualizarDados)
                                    cliente.Localidade = localidade ?? cliente.Localidade;

                                if (cliente.Pais == null && !string.IsNullOrEmpty(pessoa.Endereco.Cidade.Pais?.SiglaPais ?? ""))
                                    cliente.Pais = repPais.BuscarPorSiglaUF(pessoa.Endereco.Cidade.Pais.SiglaPais, cacheObjetoValor.lstPais);
                            }
                            else
                            {
                                bool cadastrarNovaCidade = false;
                                if (pessoa.Endereco.Cidade?.Codigo > 0)
                                {
                                    cliente.Localidade = repLocalidade.BuscarPorCodigo(pessoa.Endereco.Cidade.Codigo, cacheObjetoValor.lstLocalidades);
                                    if (cliente.Localidade == null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.CodigoIntegracao))
                                        cliente.Localidade = repLocalidade.buscarPorCodigoEmbarcador(pessoa.Endereco.Cidade.CodigoIntegracao, cacheObjetoValor.lstLocalidades);
                                    if (cliente.Localidade == null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.CodigoDocumento))
                                        cliente.Localidade = repLocalidade.buscarPorCodigoDocumento(pessoa.Endereco.Cidade.CodigoDocumento, cacheObjetoValor.lstLocalidades);
                                    if (cliente.Localidade == null)
                                    {
                                        cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(9999999, cacheObjetoValor.lstLocalidades);
                                        cadastrarNovaCidade = true;
                                    }

                                    cliente.Pais = repPais.BuscarPorCodigo(pessoa.Endereco.Cidade.Pais?.CodigoPais ?? 9999, cacheObjetoValor.lstPais);

                                    if (cliente.Pais == null && !string.IsNullOrEmpty(pessoa.Endereco.Cidade.Pais?.SiglaPais ?? ""))
                                        cliente.Pais = repPais.BuscarPorSiglaUF(pessoa.Endereco.Cidade.Pais.SiglaPais, cacheObjetoValor.lstPais);
                                }
                                if (!string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade?.CodigoIntegracao) || cadastrarNovaCidade)
                                {
                                    Dominio.Entidades.Localidade localidadeExt = null;
                                    if (!cadastrarNovaCidade)
                                        localidadeExt = repLocalidade.buscarPorCodigoEmbarcador(pessoa.Endereco.Cidade.CodigoIntegracao, cacheObjetoValor.lstLocalidades);
                                    if (localidadeExt == null)
                                        localidadeExt = repLocalidade.BuscarPorDescricaoEUF(pessoa.Endereco.Cidade.Descricao, "EX", null);

                                    if (localidadeExt == null)
                                    {
                                        localidadeExt = new Dominio.Entidades.Localidade();
                                        localidadeExt.CEP = "";
                                        if (objetoValorPersistente == null) //angelopendenciadeindice ****
                                            localidadeExt.Codigo = repLocalidade.BuscarPorMaiorCodigo();
                                        localidadeExt.Codigo++;
                                        localidadeExt.Descricao = pessoa.Endereco.Cidade.Descricao;
                                        localidadeExt.Estado = new Dominio.Entidades.Estado() { Sigla = "EX" };
                                        localidadeExt.CodigoIBGE = 9999999;
                                        localidadeExt.CodigoLocalidadeEmbarcador = pessoa.Endereco.Cidade.CodigoIntegracao;
                                        if (pessoa.Endereco.Cidade.Pais != null)
                                        {
                                            localidadeExt.Pais = repPais.BuscarPorCodigo(pessoa.Endereco.Cidade.Pais.CodigoPais, cacheObjetoValor.lstPais);
                                            if (localidadeExt.Pais == null && !string.IsNullOrEmpty(pessoa.Endereco.Cidade.Pais?.SiglaPais ?? ""))
                                                localidadeExt.Pais = repPais.BuscarPorSiglaUF(pessoa.Endereco.Cidade.Pais.SiglaPais, cacheObjetoValor.lstPais);

                                            if (localidadeExt.Pais == null)
                                            {
                                                st.Append(string.Concat("Código do país do ", tipoCliente, " (" + pessoa.Endereco.Cidade.Pais.CodigoPais + ") ou a Sigla (" + pessoa.Endereco.Cidade.Pais.SiglaPais + ") não foi localizado; "));
                                            }
                                        }
                                        if (objetoValorPersistente == null)
                                            repLocalidade.Inserir(localidadeExt);
                                        else
                                            objetoValorPersistente.Inserir(localidadeExt);
                                    }

                                    if (localidadeExt != null && (cliente.Localidade == null || !cliente.NaoAtualizarDados || cadastrarNovaCidade))
                                    {
                                        cliente.Localidade = localidadeExt;
                                        cliente.Pais = localidadeExt.Pais;
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade?.CodigoIntegracao))
                                        cliente.Localidade = repLocalidade.buscarPorCodigoEmbarcador(pessoa.Endereco.Cidade.CodigoIntegracao, cacheObjetoValor.lstLocalidades);
                                    if (cliente.Localidade == null)
                                        cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(9999999, cacheObjetoValor.lstLocalidades);
                                    cliente.Pais = repPais.BuscarPorCodigo(pessoa.Endereco.Cidade?.Pais?.CodigoPais ?? 9999);

                                    if (cliente.Pais == null && !string.IsNullOrEmpty(pessoa.Endereco.Cidade?.Pais?.SiglaPais ?? ""))
                                        cliente.Pais = repPais.BuscarPorSiglaUF(pessoa.Endereco.Cidade.Pais.SiglaPais, cacheObjetoValor.lstPais);
                                }

                                cliente.Tipo = "E";
                                cliente.Cidade = pessoa.Endereco.Cidade?.Descricao ?? "EXTERIOR";
                            }

                            if (pessoa.AtualizarEnderecoPessoa)
                            {
                                if (!inserirEnderecoNovo && !string.IsNullOrWhiteSpace(pessoa.CodigoDocumento))
                                    cliente.CodigoDocumento = pessoa.CodigoDocumento;
                                if (pessoa.Fornecedor)
                                    cliente.CodigoDocumentoFornecedor = pessoa.CodigoDocumentoFornecedor;
                                cliente.TipoEmissaoCTeDocumentosExclusivo = pessoa.TipoEmissaoCTeDocumentosExclusivo;
                                cliente.ExigirNumeroControleCliente = pessoa.ExigirNumeroControleCliente;
                                cliente.ExigirNumeroNumeroReferenciaCliente = pessoa.ExigirNumeroNumeroReferenciaCliente;
                                if (rateioFormula != null)
                                    cliente.RateioFormulaExclusivo = rateioFormula;
                            }

                            bool alterouCEP = cliente.CEP != cep;
                            cliente.Bairro = pessoa.Endereco.Bairro;
                            cliente.CEP = cep;
                            cliente.Complemento = pessoa.Endereco.Complemento;
                            cliente.Endereco = pessoa.Endereco.Logradouro;
                            cliente.Numero = !string.IsNullOrEmpty(pessoa.Endereco.Numero) ? pessoa.Endereco.Numero : "S/N";
                            cliente.Latitude = !string.IsNullOrEmpty(pessoa.Endereco.Latitude) ? pessoa.Endereco.Latitude : cliente.Latitude;
                            cliente.Longitude = !string.IsNullOrEmpty(pessoa.Endereco.Longitude) ? pessoa.Endereco.Longitude : cliente.Longitude;

                            if ((alterouCidade || alterouCEP) && (string.IsNullOrWhiteSpace(pessoa.Endereco.Latitude) || string.IsNullOrWhiteSpace(pessoa.Endereco.Longitude)))
                            {
                                cliente.Latitude = null;
                                cliente.Longitude = null;
                                cliente.GeoLocalizacaoStatus = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.NaoGerado;
                                cliente.GeoLocalizacaoRaioLocalidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoRaioLocalidade.NaoValidado;
                            }

                            cliente.DataUltimaAtualizacao = DateTime.Now;
                            cliente.Integrado = false;
                            cliente.Observacao = pessoa.Observacao;

                            if (pessoa.ExigeAgendamento != null)
                                cliente.ExigeQueEntregasSejamAgendadas = pessoa.ExigeAgendamento.Value;

                            if (configuracaoPessoa?.ExigeQueSuasEntregasSejamAgendadas ?? false)
                                cliente.ExigeQueEntregasSejamAgendadas = true;

                            if (!string.IsNullOrWhiteSpace(pessoa.CodigoIntegracao))
                                cliente.CodigoIntegracao = pessoa.CodigoIntegracao;

                            if (!string.IsNullOrWhiteSpace(pessoa.NumeroCUITRUIT))
                                cliente.NumeroCUITRUT = Utilidades.String.Left(pessoa.NumeroCUITRUIT, 150);

                            string telefone = pessoa.Endereco.DDDTelefone + pessoa.Endereco.Telefone;
                            telefone = telefone.Split(',')[0];
                            telefone = Utilidades.String.Left(telefone, 20);

                            if (!string.IsNullOrWhiteSpace(telefone) && telefone.Length < 4)
                                telefone = "";

                            cliente.Telefone1 = !string.IsNullOrWhiteSpace(telefone) ? telefone : !string.IsNullOrWhiteSpace(pessoa.Endereco?.Telefone ?? "") ? pessoa.Endereco.Telefone : cliente.Telefone1;

                            if (atualizarEmail || inserir)
                            {
                                cliente.EmailFatura = !string.IsNullOrWhiteSpace(pessoa.EmailFatura) ? pessoa.EmailFatura : !string.IsNullOrWhiteSpace(cliente.EmailFatura) ? cliente.EmailFatura : "";
                                cliente.Email = !string.IsNullOrWhiteSpace(pessoa.Email) ? pessoa.Email : !string.IsNullOrWhiteSpace(cliente.Email) ? cliente.Email : "";
                                cliente.EmailStatus = !string.IsNullOrWhiteSpace(cliente.Email) ? "A" : "I";
                                cliente.EmailContador = "";
                                cliente.EmailContato = "";
                                cliente.EmailContadorStatus = "I";
                                cliente.EmailContatoStatus = "I";
                            }

                            string rgIE = Utilidades.String.OnlyNumbers(pessoa.RGIE);

                            cliente.IE_RG = string.IsNullOrWhiteSpace(rgIE) ? "ISENTO" : rgIE;

                            if (cliente.IndicadorIE == null)
                            {
                                if (rgIE == "ISENTO" || string.IsNullOrWhiteSpace(rgIE))
                                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;
                                else
                                    cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;
                            }
                            else if (cliente.IE_RG == "ISENTO" && !inserirEnderecoNovo)
                                cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;

                            if (cliente.Tipo == "E" || pessoa.ClienteExterior)
                                cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte;

                            if (cliente.IE_RG != "ISENTO")
                                cliente.IndicadorIE = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS;

                            cliente.Nome = pessoa.RazaoSocial;
                            cliente.NomeFantasia = pessoa.NomeFantasia;
                            if (!string.IsNullOrWhiteSpace(pessoa.RKST))
                                cliente.RKST = pessoa.RKST?.Replace(" ", "");
                            if (!string.IsNullOrWhiteSpace(pessoa.MDGCode))
                                cliente.MDGCode = pessoa.MDGCode?.Replace(" ", "");
                            if (!string.IsNullOrWhiteSpace(pessoa.CMDID))
                                cliente.CMDID = pessoa.CMDID?.Replace(" ", "");

                            if (cliente.Tipo == "J" && string.IsNullOrWhiteSpace(pessoa.GrupoPessoa?.CodigoIntegracao))
                            {
                                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(cliente.CPF_CNPJ_SemFormato.Remove(8, 6), cacheObjetoValor.lstGrupoPessoas);
                                buscouGrupo = true;
                                if (grupoPessoas != null)
                                    cliente.GrupoPessoas = grupoPessoas;
                            }
                        }
                        else
                        {
                            st.Append(string.Concat("É obrigatório informar a cidade do " + tipoCliente));
                        }
                    }
                    else
                    {
                        st.Append(string.Concat("É obrigatório informar o endereço do " + tipoCliente));
                    }
                }
            }
            else if (pessoa.ClienteExterior)
            {
                Dominio.Entidades.Localidade localidadeExterior = null;
                if (!string.IsNullOrWhiteSpace(pessoa.Endereco?.Cidade?.CodigoIntegracao))
                    localidadeExterior = repLocalidade.buscarPorCodigoEmbarcador(pessoa.Endereco?.Cidade?.CodigoIntegracao, cacheObjetoValor.lstLocalidades);

                if (localidadeExterior != null)
                {
                    cliente.Pais = localidadeExterior.Pais;
                    cliente.Cidade = localidadeExterior.Descricao;
                    cliente.Localidade = localidadeExterior;
                }
                else
                {
                    cliente.Pais = repPais.BuscarPorCodigo(pessoa.Endereco?.Cidade?.Pais?.CodigoPais ?? 9999, cacheObjetoValor.lstPais);

                    if (cliente.Pais == null && !string.IsNullOrEmpty(pessoa.Endereco?.Cidade?.Pais?.SiglaPais ?? ""))
                        cliente.Pais = repPais.BuscarPorSiglaUF(pessoa.Endereco.Cidade.Pais.SiglaPais, cacheObjetoValor.lstPais);

                    cliente.Cidade = pessoa.Endereco?.Cidade?.Descricao ?? "EXTERIOR";
                }
            }

            if (!pessoa.Fornecedor && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && integracaoViaWS)
            {
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoasRemocao = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor, cliente.CPF_CNPJ);
                if (modalidadePessoasRemocao != null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoasRemocao = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoasRemocao.Codigo);

                    if (modalidadeFornecedorPessoasRemocao != null)
                        repModalidadeFornecedorPessoas.Deletar(modalidadeFornecedorPessoasRemocao);
                }
            }

            if (pessoa.MesoRegiao != null && !string.IsNullOrEmpty(pessoa.MesoRegiao?.CodigoIntegracao))
            {
                Dominio.Entidades.Embarcador.Localidades.MesoRegiao mesoRegiao = repMesoRegiao.BuscaPorCodigoIntegracao(pessoa.MesoRegiao?.CodigoIntegracao);

                if (mesoRegiao == null && !string.IsNullOrEmpty(pessoa.MesoRegiao?.Descricao))
                {
                    mesoRegiao = new Dominio.Entidades.Embarcador.Localidades.MesoRegiao()
                    {
                        CodigoIntegracao = pessoa.MesoRegiao?.CodigoIntegracao,
                        Descricao = pessoa.MesoRegiao?.Descricao,
                        Situacao = true
                    };
                    repMesoRegiao.Inserir(mesoRegiao);
                }
                cliente.MesoRegiao = mesoRegiao;
            }

            if (pessoa.Endereco?.Cidade?.Regiao != null && !string.IsNullOrEmpty(pessoa.Endereco?.Cidade?.Regiao?.CodigoIntegracao) && (configWebService?.SalvarRegiaoNoClienteParaPreencherRegiaoDestinoDosPedidos ?? false))
            {
                Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
                Dominio.Entidades.Embarcador.Localidades.Regiao existeRegiao = repRegiao.BuscarPorCodigoIntegracao(pessoa.Endereco.Cidade.Regiao.CodigoIntegracao, cacheObjetoValor.lstRegiao);
                if (existeRegiao == null)
                    existeRegiao = CadastroRegiaoInexistente(unitOfWork, pessoa, cacheObjetoValor);
                cliente.Regiao = existeRegiao;
            }

            if (inserir)
            {
                if (pessoa.CodigoAtividade > 0)
                    cliente.Atividade = repAtividade.BuscarPorCodigo(pessoa.CodigoAtividade, cacheObjetoValor.lstAtividade);
                else
                    cliente.Atividade = Servicos.Atividade.ObterAtividade(empresa, cliente.Tipo, unitOfWork.StringConexao, 0, unitOfWork, cacheObjetoValor.lstAtividade, cacheObjetoValor.lstEmpresa);
            }
            else if (pessoa.AtualizarEnderecoPessoa && pessoa.CodigoAtividade > 0)
                cliente.Atividade = repAtividade.BuscarPorCodigo(pessoa.CodigoAtividade, cacheObjetoValor.lstAtividade);

            if ((atualizarEmail && pessoa.AtualizarEnderecoPessoa) || inserir)
            {
                cliente.EmailFatura = !string.IsNullOrWhiteSpace(pessoa.EmailFatura) ? pessoa.EmailFatura : !string.IsNullOrWhiteSpace(cliente.EmailFatura) ? cliente.EmailFatura : "";
                cliente.Email = !string.IsNullOrWhiteSpace(pessoa.Email) ? pessoa.Email : !string.IsNullOrWhiteSpace(cliente.Email) ? cliente.Email : "";
                cliente.EmailStatus = !string.IsNullOrWhiteSpace(cliente.Email) ? "A" : "I";
            }

            if ((cliente.CPF_CNPJ == 0f && configuracao.Pais != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior) || (!pessoa.ClienteExterior && configuracao.Pais != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior && !(cliente.Tipo == "J" ? Utilidades.Validate.ValidarCNPJ(cliente.CPF_CNPJ_SemFormato) : Utilidades.Validate.ValidarCPF(cliente.CPF_CNPJ_SemFormato))))
                st.Append(string.Concat("CPF/CNPJ do ", tipoCliente, " inválida; "));

            if (cliente.Atividade == null)
                st.Append(string.Concat("Atividade do ", tipoCliente, " inválida; "));

            if (!string.IsNullOrWhiteSpace(pessoa.CodigoCategoria))
            {
                cliente.Categoria = repCategoria.BuscarPorCodigoIntegracao(pessoa.CodigoCategoria, cacheObjetoValor.lstCategoriaPessoa);

                if (cliente.Categoria == null)
                    st.Append(string.Concat("Categoria do cliente ", tipoCliente, " inválida; "));
            }

            if (!string.IsNullOrWhiteSpace(pessoa.CodigoCategoria))
            {
                cliente.Categoria = repCategoria.BuscarPorCodigoIntegracao(pessoa.CodigoCategoria, cacheObjetoValor.lstCategoriaPessoa);

                if (cliente.Categoria == null)
                    st.Append(string.Concat("Categoria do cliente ", tipoCliente, " inválida; "));
            }

            if (!string.IsNullOrWhiteSpace(pessoa.GrupoPessoa?.CodigoIntegracao))
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = repGrupoPessoas.BuscarPorCodigoIntegracao(pessoa.GrupoPessoa.CodigoIntegracao, cacheObjetoValor.lstGrupoPessoas);
                if (grupoPessoa == null)
                {
                    if (!configuracao.NaoValidarGrupoPessoaNaIntegracao)
                        st.Append(string.Concat("O grupo do cliente informado para o ", tipoCliente, " não existe na base Multisoftware; "));
                }
                else
                {
                    cliente.GrupoPessoas = grupoPessoa;
                }
            }

            if (pessoa.ValidarValorMinimoMercadoriaPorEntrega ?? false)
            {
                decimal valorMinimoMercadoriaPorEntrega = pessoa.ValorMinimoMercadoriaPorEntrega.ToDecimal();

                if (valorMinimoMercadoriaPorEntrega > 0)
                    st.Append(string.Concat("Valor mínimo da mercadoria por entrega do ", tipoCliente, " deve ser maior que 0; "));
                else
                {
                    cliente.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento = true;
                    cliente.ValorMinimoEntrega = valorMinimoMercadoriaPorEntrega;
                }
            }

            if (!inserirEnderecoNovo || inserir)
            {
                if (string.IsNullOrWhiteSpace(cliente.Bairro) || (cliente.Tipo == "E" && cliente.Bairro.Length < 3))
                {
                    cliente.Bairro = "S/B";
                }
                else
                {
                    if (cliente.Bairro.Trim().Length < 2)
                    {
                        cliente.Bairro = "Bairro " + cliente.Bairro.Trim();
                        st.Append(string.Concat("Bairro do ", tipoCliente, " deve conter mais que 1 caracter; "));
                    }
                    if (cliente.Bairro.Trim().Length > 60)
                        cliente.Bairro = cliente.Bairro.Substring(0, 60);
                }

                if (string.IsNullOrWhiteSpace(cliente.Endereco))
                {
                    st.Append(string.Concat("Endereço do ", tipoCliente, " inválido; "));
                }
                else
                {
                    if (cliente.Endereco.Length < 3)
                    {
                        cliente.Endereco = "Logradouro " + cliente.Endereco;
                    }
                    if (cliente.Endereco.Length > 80)
                        cliente.Endereco = cliente.Endereco.Substring(0, 80);
                }

                if (string.IsNullOrWhiteSpace(cliente.Numero))
                {
                    cliente.Numero = "S/N";
                }

                if (string.IsNullOrWhiteSpace(cliente.Nome))
                {
                    st.Append(string.Concat("Nome/Razão Social do ", tipoCliente, " inválido; "));
                }
                else
                {
                    if (cliente.Nome.Length < 3)
                    {
                        st.Append(string.Concat("Nome/Razão Social do ", tipoCliente, " deve conter mais que 2 caracteres; "));
                    }
                    if (cliente.Nome.Length > 80)
                        cliente.Nome = cliente.Nome.Substring(0, 80);
                }

                if (cliente.Localidade == null)
                {
                    string codigoLocalidadeNaoCadastrada = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().CodigoLocalidadeNaoCadastrada;
                    Dominio.Entidades.Localidade localidadeNaoCadastrada = !string.IsNullOrWhiteSpace(codigoLocalidadeNaoCadastrada) ? repLocalidade.BuscarPorCodigo(codigoLocalidadeNaoCadastrada.ToInt(), cacheObjetoValor.lstLocalidades) : null;

                    if (localidadeNaoCadastrada == null)
                    {
                        if (!string.IsNullOrWhiteSpace(pessoa?.Endereco?.Cidade?.Descricao) && !string.IsNullOrWhiteSpace(pessoa?.Endereco?.Cidade?.SiglaUF))
                            st.Append(string.Concat($"Localidade do {tipoCliente} inválida ({pessoa?.Endereco?.Cidade?.Descricao}/{pessoa?.Endereco?.Cidade?.SiglaUF}); "));
                        else
                            st.Append(string.Concat("Localidade do ", tipoCliente, " inválida; "));
                    }
                    else
                        cliente.Localidade = localidadeNaoCadastrada;
                }

                if (!inserirEnderecoNovo && pessoa.AtualizarEnderecoPessoa && !string.IsNullOrWhiteSpace(pessoa.CodigoDocumento))
                    cliente.CodigoDocumento = pessoa.CodigoDocumento;
                if (pessoa.Fornecedor)
                    cliente.CodigoDocumentoFornecedor = pessoa.CodigoDocumentoFornecedor;
                if (pessoa.InativarCliente)
                    cliente.Ativo = false;
                else
                    cliente.Ativo = true;
            }

            if (pessoa.AtualizarEnderecoPessoa && (!inserirEnderecoNovo || inserir))
            {
                if (!string.IsNullOrWhiteSpace(pessoa.CodigoDocumento))
                    cliente.CodigoDocumento = pessoa.CodigoDocumento;
                if (pessoa.Fornecedor)
                    cliente.CodigoDocumentoFornecedor = pessoa.CodigoDocumentoFornecedor;
                cliente.ExigirNumeroControleCliente = pessoa.ExigirNumeroControleCliente;
                cliente.ExigirNumeroNumeroReferenciaCliente = pessoa.ExigirNumeroNumeroReferenciaCliente;
                cliente.TipoEmissaoCTeDocumentosExclusivo = pessoa.TipoEmissaoCTeDocumentosExclusivo;
                if (rateioFormula != null)
                    cliente.RateioFormulaExclusivo = rateioFormula;

                if (pessoa.RegimeTributario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.NaoInformado)
                    cliente.RegimeTributario = pessoa.RegimeTributario;
            }

            if (!string.IsNullOrWhiteSpace(cliente.Email))
            {
                var emails = cliente.Email.Split(';');
                List<string> emailsValidos = new List<string>();

                foreach (string email in emails)
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        if (Utilidades.Validate.ValidarEmail(email.Trim()))
                            emailsValidos.Add(email.Trim());
                        //st.Append(string.Concat("E-mail (", email, ") do ", tipoCliente, " inválido; "));
                    }
                }
                cliente.Email = string.Join(";", emailsValidos);
            }

            if (!string.IsNullOrEmpty(cliente.CodigoAlternativo))
                cliente.CodigoAlternativo = pessoa.CodigoAlternativo;


            if (st.Length > 0)
            {
                if (!pessoa.AtualizarEnderecoPessoa && !inserir)
                {
                    st.Append(string.Concat("Como não foi solicitado a atualização do cadastro via integração, favor atualizar o cadastro na base multisoftware;"));
                }
                Servicos.Log.TratarErro("Cliente " + tipoCliente + ". Dados: " + Newtonsoft.Json.JsonConvert.SerializeObject(pessoa));
                Servicos.Log.TratarErro("Mensagem " + st.ToString());
                verificacaoCliente.cliente = null;
                verificacaoCliente.Status = false;
                verificacaoCliente.Mensagem = st.ToString();
            }
            else
            {
                if (inserir)
                {
                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null && !buscouGrupo)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6), cacheObjetoValor.lstGrupoPessoas);
                        if (grupoPessoas != null)
                        {
                            cliente.GrupoPessoas = grupoPessoas;
                        }
                    }

                    cliente.Ativo = true;

                    if (objetoValorPersistente == null)
                    {
                        if (auditado != null)
                            repCliente.Inserir(cliente, auditado);
                        else
                            repCliente.Inserir(cliente);
                    }
                    else
                        objetoValorPersistente.Inserir(cliente);


                    if (inserirEnderecoNovo)
                    {
                        if (pessoa.Endereco != null && pessoa.Endereco.Cidade != null && !string.IsNullOrEmpty(pessoa.Endereco.Bairro) && !string.IsNullOrEmpty(pessoa.Endereco.Numero) && !string.IsNullOrEmpty(pessoa.Endereco.Logradouro))
                        {
                            Dominio.Entidades.Localidade localidade = null;

                            if (pessoa.Endereco.Cidade.IBGE != 0)
                                localidade = repLocalidade.BuscarPorCodigoIBGE(pessoa.Endereco.Cidade.IBGE, cacheObjetoValor.lstLocalidades);

                            if (localidade == null && pessoa.Endereco != null && pessoa.Endereco.Cidade != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.SiglaUF) && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.Descricao))
                                localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(pessoa.Endereco.Cidade.Descricao).ToUpper(), pessoa.Endereco.Cidade.SiglaUF.ToUpper());

                            if (localidade == null && pessoa.Endereco != null)
                                localidade = repLocalidade.BuscarPorCEP(pessoa.Endereco.CEP, cacheObjetoValor.lstLocalidades);

                            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco outroEndereco = repClienteOutroEndereco.BuscarPorIE(pessoa.RGIE, pessoa.Endereco, cpfCnpj, cacheObjetoValor.lstClienteOutroEndereco);

                            if (outroEndereco == null || CompararEnderecos(outroEndereco, pessoa.Endereco, localidade))
                                outroEndereco = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco();

                            if (localidade == null)
                            {
                                st.Append(string.Concat("Não foi possível localizar a cidade enviada; "));
                                verificacaoCliente.cliente = null;
                                verificacaoCliente.Status = false;
                                verificacaoCliente.Mensagem = st.ToString();

                                return verificacaoCliente;
                            }

                            outroEndereco.Bairro = pessoa.Endereco.Bairro;
                            outroEndereco.CEP = pessoa.Endereco.CEP;
                            outroEndereco.Cliente = cliente;
                            outroEndereco.CodigoEmbarcador = pessoa.CodigoIntegracao;
                            outroEndereco.Complemento = pessoa.Endereco.Complemento;
                            outroEndereco.Endereco = pessoa.Endereco.Logradouro;
                            outroEndereco.EnderecoDigitado = true;
                            outroEndereco.IE_RG = pessoa.RGIE;
                            outroEndereco.Numero = pessoa.Endereco.Numero;
                            outroEndereco.Telefone = pessoa.Endereco.Telefone;
                            outroEndereco.TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Outros;
                            outroEndereco.TipoLogradouro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Outros;
                            outroEndereco.Localidade = localidade;
                            if (!string.IsNullOrWhiteSpace(pessoa.CodigoDocumento))
                                outroEndereco.CodigoDocumento = pessoa.CodigoDocumento;

                            if (outroEndereco.Codigo > 0)
                                if (objetoValorPersistente == null)
                                {
                                    repClienteOutroEndereco.Atualizar(outroEndereco, auditado);
                                    Servicos.Auditoria.Auditoria.Auditar(auditado, cliente, null, "Atualizou endereço secundário " + outroEndereco.Descricao + ".", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                                }
                                else
                                    objetoValorPersistente.Atualizar(outroEndereco);
                            else
                            {
                                if (objetoValorPersistente == null)
                                {
                                    repClienteOutroEndereco.Inserir(outroEndereco, auditado);
                                    Servicos.Auditoria.Auditoria.Auditar(auditado, cliente, null, "Adicionou endereço secundário " + outroEndereco.Descricao + ".", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Insert);
                                }
                                else
                                    objetoValorPersistente.Inserir(outroEndereco);
                            }

                            verificacaoCliente.UsarOutroEndereco = true;
                            verificacaoCliente.CodigoOutroEndereco = pessoa.CodigoIntegracao;
                            verificacaoCliente.clienteOutroEndereco = outroEndereco;
                        }
                    }
                }
                else
                {
                    if (objetoValorPersistente != null)
                    {
                        objetoValorPersistente.Atualizar(cliente);
                    }
                    else
                    {
                        if (cliente.IsChanged())
                        {
                            cliente.DataUltimaAtualizacao = DateTime.Now;
                            cliente.Integrado = false;
                            if (auditado != null)
                                repCliente.Atualizar(cliente, auditado);
                            else
                                repCliente.Atualizar(cliente);
                        }
                    }
                }

                if (pessoa.Cliente)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente, auditado, unitOfWork, cacheObjetoValor);
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas modaliadeClientePessoas = repModalidadeClientePessoas.BuscarPorModalidade(modalidadePessoas.Codigo, cacheObjetoValor.lstModalidadeClientePessoas);

                    if (modaliadeClientePessoas == null)
                    {
                        modaliadeClientePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas();
                        modaliadeClientePessoas.ModalidadePessoas = modalidadePessoas;
                        if (objetoValorPersistente == null)
                        {
                            if (auditado != null)
                            {
                                repModalidadeClientePessoas.Inserir(modaliadeClientePessoas, auditado);
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cliente, null, "Adicionou a Modalidade Cliente " + modaliadeClientePessoas.Descricao + ".", unitOfWork);
                            }
                            else
                                repModalidadeClientePessoas.Inserir(modaliadeClientePessoas);
                        }
                        else
                        {
                            objetoValorPersistente.Inserir(modaliadeClientePessoas);
                        }
                    }
                }

                if (pessoa.Fornecedor)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor, auditado, unitOfWork, cacheObjetoValor);
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoas.Codigo, cacheObjetoValor.lstModalidadeFornecedorPessoas);

                    if (modalidadeFornecedorPessoas == null)
                    {
                        modalidadeFornecedorPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas
                        {
                            ModalidadePessoas = modalidadePessoas,
                            NaoEObrigatorioInformarNfeNaColeta = pessoa.NaoEObrigatorioInformarNfeNaColeta ?? false,
                        };

                        if (objetoValorPersistente == null)
                        {
                            repModalidadeFornecedorPessoas.Inserir(modalidadeFornecedorPessoas, auditado);
                            if (auditado != null)
                                Auditoria.Auditoria.Auditar(auditado, cliente, null, "Adicionou a Modalidade fornecedor " + modalidadeFornecedorPessoas.Descricao + ".", unitOfWork);
                        }
                        else
                        {
                            objetoValorPersistente.Inserir(modalidadeFornecedorPessoas);
                        }
                    }

                    cliente.AtivarAcessoFornecedor = true;
                    cliente.DataUltimaAtualizacao = DateTime.Now;
                    cliente.Integrado = false;
                    if (objetoValorPersistente == null)
                        repCliente.Atualizar(cliente, auditado);
                    else
                        objetoValorPersistente.Atualizar(cliente);

                    CriarUsuarioFornecedor(cliente, unitOfWork, tipoServicoMultisoftware, auditado);
                }

                // Criando o ClienteDescarga
                if (!repClienteDescarga.PossuiClienteDescarga(cliente.CPF_CNPJ, cacheObjetoValor.lstCodigosClienteDescarga))
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = new Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga
                    {
                        Cliente = cliente,
                        NaoExigePreenchimentoDeChecklistEntrega = pessoa.NaoExigePreenchimentoDeChecklistEntrega ?? false,
                    };
                    if (objetoValorPersistente == null)
                    {
                        repClienteDescarga.Inserir(clienteDescarga);
                        if (auditado != null)
                            Auditoria.Auditoria.Auditar(auditado, cliente, null, "Adicionou o Cliente Descarga " + clienteDescarga.Descricao + ".", unitOfWork);
                    }
                    else
                    {
                        objetoValorPersistente.Inserir(clienteDescarga);
                    }
                }

                if (!string.IsNullOrWhiteSpace(pessoa.RNTRC) || !string.IsNullOrWhiteSpace(pessoa.NumeroCartaoCIOT) || pessoa.GerarCIOT != null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, auditado, unitOfWork, cacheObjetoValor);
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoas.Codigo, cacheObjetoValor.lstModalidadeTransportadoraPessoas);

                    bool inserirModalidade = false;
                    if (modalidadeTransportadoraPessoas == null)
                    {
                        modalidadeTransportadoraPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas();
                        inserirModalidade = true;
                    }
                    else
                        modalidadeTransportadoraPessoas.Initialize();

                    modalidadeTransportadoraPessoas.ModalidadePessoas = modalidadePessoas;

                    if (!string.IsNullOrWhiteSpace(pessoa.RNTRC))
                        modalidadeTransportadoraPessoas.RNTRC = pessoa.RNTRC.Length > 8 ? pessoa.RNTRC.Substring(1, 8) : pessoa.RNTRC.PadLeft(8, '0');

                    if (pessoa.GerarCIOT != null)
                        modalidadeTransportadoraPessoas.GerarCIOT = pessoa.GerarCIOT.Value;

                    if (pessoa.TipoFavorecidoCIOT != null)
                        modalidadeTransportadoraPessoas.TipoFavorecidoCIOT = pessoa.TipoFavorecidoCIOT.Value;

                    if (!string.IsNullOrWhiteSpace(pessoa.NumeroCartaoCIOT))
                        modalidadeTransportadoraPessoas.NumeroCartao = pessoa.NumeroCartaoCIOT.Left(50);

                    if (inserirModalidade)
                    {
                        if (objetoValorPersistente == null)
                        {
                            modalidadeTransportadoraPessoas.ReterImpostosContratoFrete = true;
                            if (auditado != null)
                            {
                                repModalidadeTransportadoraPessoas.Inserir(modalidadeTransportadoraPessoas, auditado);
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cliente, null, "Adicionou a Modalidade Transportador " + modalidadeTransportadoraPessoas.Descricao + ".", unitOfWork);
                            }
                            else
                                repModalidadeTransportadoraPessoas.Inserir(modalidadeTransportadoraPessoas);
                        }
                        else
                        {
                            objetoValorPersistente.Inserir(modalidadeTransportadoraPessoas);
                        }
                    }
                    else
                    {
                        repModalidadeTransportadoraPessoas.Atualizar(modalidadeTransportadoraPessoas);
                        var alteracoes = modalidadeTransportadoraPessoas.GetChanges();
                        if (alteracoes.Count > 0 && auditado != null)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cliente, alteracoes, "Alterou a Modalidade Transportador " + modalidadeTransportadoraPessoas.Descricao + ".", unitOfWork);
                    }

                    if (pessoa.TipoPagamentoCIOT != null)
                        Servicos.Cliente.SalvarTiposPagamentoCIOTPorOperadora(pessoa.TipoPagamentoCIOT.Value, modalidadeTransportadoraPessoas, unitOfWork);

                }

                verificacaoCliente.cliente = cliente;
                verificacaoCliente.Mensagem = "";
                verificacaoCliente.Status = true;
            }

            return verificacaoCliente;
        }

        /// <summary>
        /// Retorna se um endereço é suficientemente diferente de outro
        /// </summary>
        private bool CompararEnderecos(Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco endereco1, Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco2, Dominio.Entidades.Localidade endereco2Localidade)
        {
            if (endereco1 == null || endereco2 == null)
                return true;

            if (endereco1.Bairro != endereco2.Bairro)
                return true;

            if (endereco1.CEP != endereco2.CEP)
                return true;

            if (endereco1.Complemento != endereco2.Complemento)
                return true;

            if (endereco1.Endereco != endereco2.Logradouro)
                return true;

            if (endereco1.Numero != endereco2.Numero)
                return true;

            if (endereco1.Telefone != endereco2.Telefone)
                return true;

            if (endereco1.Localidade != endereco2Localidade)
                return true;

            return false;
        }

        private void ExcluirModalidadePessoa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade tipoModalidade, double cpf_cnpj, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(tipoModalidade, cpf_cnpj);

            if (modalidadePessoas == null)
                return;

            repModalidadePessoas.Deletar(modalidadePessoas);
        }

        private void PreencherDadosCliente(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork, bool possuiCliente)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas repModalidadeClientePessoas = new Repositorio.Embarcador.Pessoas.ModalidadeClientePessoas(unitOfWork);
            if (possuiCliente)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente, null, unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas modalidadeClientePessoas = repModalidadeClientePessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                if (modalidadeClientePessoas == null)
                    modalidadeClientePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas();

                modalidadeClientePessoas.ModalidadePessoas = modalidadePessoas;
                if (modalidadeClientePessoas.Codigo == 0)
                    repModalidadeClientePessoas.Inserir(modalidadeClientePessoas);
                else
                    repModalidadeClientePessoas.Atualizar(modalidadeClientePessoas);
            }
            else
            {
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Cliente, pessoa.CPF_CNPJ);
                if (modalidadePessoas != null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeClientePessoas modalidadeClientePessoas = repModalidadeClientePessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                    if (modalidadeClientePessoas != null)
                        repModalidadeClientePessoas.Deletar(modalidadeClientePessoas);

                    repModalidadePessoas.Deletar(modalidadePessoas);
                }
            }
        }

        private void PreencherDadosFornecedor(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork, bool possuiFornecedor)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
            if (possuiFornecedor)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor, null, unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                if (modalidadeFornecedorPessoas == null)
                    modalidadeFornecedorPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas();

                modalidadeFornecedorPessoas.ModalidadePessoas = modalidadePessoas;
                if (modalidadeFornecedorPessoas.Codigo == 0)
                    repModalidadeFornecedorPessoas.Inserir(modalidadeFornecedorPessoas);
                else
                    repModalidadeFornecedorPessoas.Atualizar(modalidadeFornecedorPessoas);
            }
            else
                this.ExcluirModalidadePessoa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor, pessoa.CPF_CNPJ, unitOfWork);
        }

        private void PreencherDadosTransportadorTerceiro(Dominio.Entidades.Cliente pessoa, Repositorio.UnitOfWork unitOfWork, bool possuiTransporteTerceiro, Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repositorioModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.TipoTerceiro repTipoTerceiro = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);

            if (possuiTransporteTerceiro)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = RetornarModalidadePessoa(pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, null, unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repositorioModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                if (modalidadeTransportadoraPessoas == null)
                    modalidadeTransportadoraPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas();

                modalidadeTransportadoraPessoas.ModalidadePessoas = modalidadePessoas;
                modalidadeTransportadoraPessoas.DataEmissaoRNTRC = clienteEmbarcador.DataEmissaoRNTRC;
                modalidadeTransportadoraPessoas.DataVencimentoRNTRC = clienteEmbarcador.DataVencimentoRNTRC;
                modalidadeTransportadoraPessoas.DiasVencimentoSaldoContratoFrete = clienteEmbarcador.DiasVencimentoSaldoContratoFrete;
                modalidadeTransportadoraPessoas.DiasVencimentoAdiantamentoContratoFrete = clienteEmbarcador.DiasVencimentoAdiantamentoContratoFrete;
                modalidadeTransportadoraPessoas.ReterImpostosContratoFrete = clienteEmbarcador.ReterImpostosContratoFrete;
                modalidadeTransportadoraPessoas.PercentualAdiantamentoFretesTerceiro = clienteEmbarcador.PercentualAdiantamentoFretesTerceiro;
                modalidadeTransportadoraPessoas.RNTRC = clienteEmbarcador.RNTRC;
                modalidadeTransportadoraPessoas.TipoTransportador = clienteEmbarcador.TipoTransportadorTerceiro ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                modalidadeTransportadoraPessoas.PercentualAbastecimentoFretesTerceiro = clienteEmbarcador.PercentualAbastecimentoFretesTerceiro;
                modalidadeTransportadoraPessoas.GerarCIOT = clienteEmbarcador.TerceiroGerarCIOT;
                modalidadeTransportadoraPessoas.TipoGeracaoCIOT = clienteEmbarcador.TerceiroTipoCIOT;
                modalidadeTransportadoraPessoas.TipoFavorecidoCIOT = clienteEmbarcador.TerceiroTipoFavorecidoCIOT;

                modalidadeTransportadoraPessoas.TipoQuitacaoCIOT = clienteEmbarcador.TerceiroTipoQuitacaoCIOT;
                modalidadeTransportadoraPessoas.TipoAdiantamentoCIOT = clienteEmbarcador.TerceiroTipoAdiantamentoCIOT;

                if (!string.IsNullOrEmpty(clienteEmbarcador.TipoTerceiro))
                {
                    var tipoTerceiro = repTipoTerceiro.BuscarPorDescricao(clienteEmbarcador.TipoTerceiro);
                    if (tipoTerceiro != null)
                        modalidadeTransportadoraPessoas.TipoTerceiro = tipoTerceiro;
                }

                if (modalidadeTransportadoraPessoas.Codigo == 0)
                    repositorioModalidadeTransportadoraPessoas.Inserir(modalidadeTransportadoraPessoas);
                else
                    repositorioModalidadeTransportadoraPessoas.Atualizar(modalidadeTransportadoraPessoas);

                SalvarTiposPagamentoCIOTPorOperadora(clienteEmbarcador.TerceiroTipoPagamentoCIOT, modalidadeTransportadoraPessoas, unitOfWork);

            }
            else
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repositorioModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                this.ExcluirModalidadePessoa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, pessoa.CPF_CNPJ, unitOfWork);
            }
        }

        private void PreencherDadosDescarga(Dominio.Entidades.Cliente pessoa, Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ClienteDescarga repositorioClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repositorioClienteDescarga.BuscarPorPessoa(pessoa.CPF_CNPJ);
            bool inserir = false;
            if (clienteDescarga == null)
            {
                clienteDescarga = new Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga();
                inserir = true;
            }
            else
                clienteDescarga.Initialize();

            clienteDescarga.Cliente = pessoa;
            if (!string.IsNullOrEmpty(clienteEmbarcador.CodigoTipoDeCarga))
                clienteDescarga.TipoDeCarga = repositorioTipoDeCarga.BuscarPorCodigoEmbarcador(clienteEmbarcador.CodigoTipoDeCarga);

            if (inserir)
                repositorioClienteDescarga.Inserir(clienteDescarga);
            else
                repositorioClienteDescarga.Atualizar(clienteDescarga);

        }

        private Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas RetornarModalidadePessoa(Dominio.Entidades.Cliente pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade tipoModalidade, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe CacheObjetoValor = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (CacheObjetoValor == null)
                CacheObjetoValor = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();

            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(tipoModalidade, pessoa.CPF_CNPJ, CacheObjetoValor.lstModalidadePessoas);
            if (modalidadePessoas == null)
            {
                modalidadePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                modalidadePessoas.Cliente = pessoa;
                modalidadePessoas.TipoModalidade = tipoModalidade;
                if (objetoValorPersistente == null)
                {
                    repModalidadePessoas.Inserir(modalidadePessoas);
                    if (auditado != null)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, pessoa, null, "Adicionou a Modalidade " + modalidadePessoas.DescricaoTipoModalidade + " a pessoa", unitOfWork);
                }
                else
                    objetoValorPersistente.Inserir(modalidadePessoas);

            }
            return modalidadePessoas;
        }

        private void VincularClientePorEmpresa(Dominio.Entidades.Cliente pessoa, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            if (codigoEmpresa == 0)//Apenas para MultiNFe
                return;

            Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.DadosCliente dadosCliente = repDadosCliente.Buscar(codigoEmpresa, pessoa.CPF_CNPJ);

            if (dadosCliente == null)
            {
                dadosCliente = new Dominio.Entidades.DadosCliente();
                dadosCliente.Cliente = pessoa;
                dadosCliente.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                repDadosCliente.Inserir(dadosCliente);
            }
        }
        private void PreencherContatosPessoa(Dominio.Entidades.Cliente pessoa, Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            if (!string.IsNullOrEmpty(clienteEmbarcador.Contato))
            {
                Repositorio.Embarcador.Contatos.TipoContato repTipoContato = new Repositorio.Embarcador.Contatos.TipoContato(unitOfWork);
                Dominio.Entidades.Embarcador.Contatos.TipoContato tipoContatoContato = repTipoContato.BuscarPorCodigo(clienteEmbarcador.ContatoTipo);
                Repositorio.Embarcador.Contatos.PessoaContato repPessoaContato = new Repositorio.Embarcador.Contatos.PessoaContato(unitOfWork);

                if (tipoContatoContato != null)
                {
                    List<Dominio.Entidades.Embarcador.Contatos.TipoContato> listaTipoContatoContato = new List<Dominio.Entidades.Embarcador.Contatos.TipoContato>();
                    listaTipoContatoContato.Add(tipoContatoContato);
                    Dominio.Entidades.Embarcador.Contatos.PessoaContato pessoaContato = new Dominio.Entidades.Embarcador.Contatos.PessoaContato();

                    pessoaContato.Contato = clienteEmbarcador.Contato;
                    pessoaContato.CPF = clienteEmbarcador.ContatoCPF;
                    pessoaContato.Cargo = clienteEmbarcador.ContatoCargo;
                    pessoaContato.Ativo = clienteEmbarcador.ContatoAtivo;
                    pessoaContato.Email = clienteEmbarcador.ContatoEmail;
                    pessoaContato.Telefone = clienteEmbarcador.ContatoTelefone;
                    pessoaContato.TiposContato = listaTipoContatoContato;
                    pessoaContato.Pessoa = pessoa;

                    repPessoaContato.Inserir(pessoaContato);
                }
            }
        }
        private void PreencherEmailSecundarioPessoa(Dominio.Entidades.Cliente pessoa, Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {

            if (!string.IsNullOrEmpty(clienteEmbarcador.EmailSecundario))
            {
                if (Utilidades.Validate.ValidarEmail(clienteEmbarcador.EmailSecundario.Trim()))
                {
                    Repositorio.Embarcador.Pessoas.ClienteOutroEmail repClienteOutroEmail = new Repositorio.Embarcador.Pessoas.ClienteOutroEmail(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail email = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail();

                    email.Email = clienteEmbarcador.EmailSecundario;
                    email.TipoEmail = clienteEmbarcador.TipoEmailSecundario ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Principal;
                    email.Cliente = pessoa;

                    repClienteOutroEmail.Inserir(email);
                }
            }

        }
        private Dominio.Entidades.Embarcador.Localidades.Regiao CadastroRegiaoInexistente(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValor = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValor == null)
                cacheObjetoValor = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();

            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
            Dominio.Entidades.Embarcador.Localidades.Regiao novaRegiao = new Dominio.Entidades.Embarcador.Localidades.Regiao()
            {
                CodigoIntegracao = pessoa?.Endereco.Cidade.Regiao.CodigoIntegracao
            };

            novaRegiao.Descricao = string.IsNullOrEmpty(pessoa?.Endereco?.Cidade?.Regiao?.Descricao ?? string.Empty)
                                                ? pessoa?.Endereco?.Cidade?.Regiao.CodigoIntegracao
                                                : pessoa?.Endereco?.Cidade?.Regiao?.Descricao;
            novaRegiao.Ativo = true;
            if (objetoValorPersistente == null)
                repRegiao.Inserir(novaRegiao);
            else
                objetoValorPersistente.Inserir(novaRegiao);

            return novaRegiao;
        }

        private void PreencherDadosObjetoDeValorIntegracao(Dominio.Entidades.Cliente pessoa, Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoaIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValorCTe = null, Dominio.ObjetosDeValor.Embarcador.CTe.ObjetoValorPersistente objetoValorPersistente = null)
        {
            if (cacheObjetoValorCTe == null)
                cacheObjetoValorCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            Repositorio.Embarcador.Pessoas.RestricaoEntrega repRestricaoEntrega = new Repositorio.Embarcador.Pessoas.RestricaoEntrega(unitOfWork);

            if (pessoa == null || pessoaIntegracao == null)
                return;

            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoaSemFetch(pessoa.CPF_CNPJ);
            Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega restricaoEntrega = null;

            if (!string.IsNullOrWhiteSpace(pessoaIntegracao?.RestricaoEntrega))
                restricaoEntrega = repRestricaoEntrega.BuscarPorCodigoIntegracao(pessoaIntegracao.RestricaoEntrega);

            if (clienteDescarga != null)
            {
                clienteDescarga.TempoAgendamento = pessoaIntegracao?.TempoAgendamento ?? clienteDescarga.TempoAgendamento;
                clienteDescarga.FormaAgendamento = pessoaIntegracao?.FormaAgendamento ?? clienteDescarga.FormaAgendamento;
                clienteDescarga.LinkParaAgendamento = pessoaIntegracao?.LinkAgendamento ?? clienteDescarga.LinkParaAgendamento;
                clienteDescarga.ExigeAgendamento = pessoaIntegracao?.ExigeAgendamentoDescarga ?? clienteDescarga.ExigeAgendamento;
                clienteDescarga.AgendamentoExigeNotaFiscal = pessoaIntegracao?.AgendamentoExigeNotaFiscal ?? clienteDescarga.AgendamentoExigeNotaFiscal;

                if (restricaoEntrega != null)
                    clienteDescarga.RestricoesDescarga = new List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega>() { restricaoEntrega };

                if (objetoValorPersistente != null)
                    objetoValorPersistente.Atualizar(clienteDescarga);
                else
                    repClienteDescarga.Atualizar(clienteDescarga);
            }

            pessoa.ExigeQueEntregasSejamAgendadas = pessoaIntegracao?.ExigeAgendamentoDescarga ?? clienteDescarga?.ExigeAgendamento ?? false;
            if (!string.IsNullOrWhiteSpace(pessoaIntegracao?.CodigoAlternativo ?? string.Empty))
                pessoa.CodigoAlternativo = pessoaIntegracao?.CodigoAlternativo ?? string.Empty;

            if (objetoValorPersistente != null)
                objetoValorPersistente.Atualizar(pessoa);
            else
                repCliente.Atualizar(pessoa);
        }

        #endregion
    }
}

