using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Servicos.WebService.Pessoas
{
    public class Pessoa : ServicoBase
    {
        #region Propiedades Protegidas

        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Construstores
        
        public Pessoa(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Pessoa() : base() { }
        public Pessoa(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Pessoas.RamoAtividade> RetornarRamosAtividade(List<Dominio.Entidades.Atividade> atividades)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pessoas.RamoAtividade> ramosAtividade = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.RamoAtividade>();
            foreach (Dominio.Entidades.Atividade atividade in atividades)
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.RamoAtividade ramoAtividade = new Dominio.ObjetosDeValor.Embarcador.Pessoas.RamoAtividade();
                ramoAtividade.Codigo = atividade.Codigo;
                ramoAtividade.Descricao = atividade.Descricao;
                ramosAtividade.Add(ramoAtividade);
            }

            return ramosAtividade;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Veiculos.Motorista> RetornarMotoristas(List<Dominio.Entidades.Usuario> motoristas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Veiculos.Motorista> motoras = new List<Dominio.ObjetosDeValor.Embarcador.Veiculos.Motorista>();
            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                Dominio.ObjetosDeValor.Embarcador.Veiculos.Motorista mot = new Dominio.ObjetosDeValor.Embarcador.Veiculos.Motorista();
                mot.CPF = motorista.CPF;
                mot.Nome = motorista.Nome;
                motoras.Add(mot);
            }

            return motoras;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo> RetornarVeiculos(List<Dominio.Entidades.Veiculo> veiculos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo> veics = new List<Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo>();
            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo veic = new Dominio.ObjetosDeValor.Embarcador.Veiculos.Veiculo();
                veic.Placa = veiculo.Placa;
                veic.NumeroFrota = veiculo.NumeroFrota;
                veics.Add(veic);
            }

            return veics;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa> RetornarGrupoPessoas(List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            //todo: refazer regra dos reboques, fazer por operador

            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModelosVeicularesCargas = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeiculares = repModelosVeicularesCargas.BuscarTodosReboques();

            List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa> dynGrupoPessoa = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>();
            Servicos.WebService.Carga.TipoOperacao serWSTipoOperacao = new Servicos.WebService.Carga.TipoOperacao(unitOfWork);
            Servicos.WebService.Carga.ModeloVeicularCarga serWSModeloVeicularCarga = new Carga.ModeloVeicularCarga(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo in grupoPessoas)
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa dynGrupo = ConverterObjetoGrupoPessoa(grupo);

                if (grupo.ModelosReboque != null && grupo.ModelosReboque.Count > 0)
                    dynGrupo.ModelosVeiculares = serWSModeloVeicularCarga.RetornarModelosVeiculares(grupo.ModelosReboque.ToList());
                else
                    dynGrupo.ModelosVeiculares = serWSModeloVeicularCarga.RetornarModelosVeiculares(modelosVeiculares);

                dynGrupo.TiposOperacaoes = serWSTipoOperacao.RetornarTiposDeOperacao(repTipoOperacao.BuscarPorGrupoPessoas(grupo));
                dynGrupoPessoa.Add(dynGrupo);
            }

            return dynGrupoPessoa;
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa ConverterObjetoGrupoPessoa(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa)
        {
            if (grupoPessoa != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa dynGrupo = new Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa();
                dynGrupo.Codigo = grupoPessoa.Codigo;
                dynGrupo.CodigoIntegracao = grupoPessoa.CodigoIntegracao;
                dynGrupo.Descricao = grupoPessoa.Descricao;
                return dynGrupo;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterObjetoPessoaCodIntegracao(Dominio.Entidades.Cliente cliente, string numeroCarga = null)
        {
            if (cliente == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

            pessoa.ClienteExterior = cliente.Tipo == "E" ? true : false;

            if (pessoa.ClienteExterior)
                pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
            else
                pessoa.TipoPessoa = cliente.Tipo == "J" ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica;

            pessoa.Protocolo = cliente.Codigo;
            pessoa.Codigo = cliente.CodigoIntegracao;
            pessoa.CodigoAtividade = cliente.Atividade.Codigo;
            pessoa.CodigoIntegracao = cliente.CodigoIntegracao;
            pessoa.Email = cliente.Email;
            pessoa.Endereco = ConverterObjetoEnderecoPessoa(cliente, numeroCarga);
            pessoa.IM = cliente.InscricaoMunicipal;
            pessoa.CodigoDocumento = cliente.CodigoDocumento;
            pessoa.CodigoDocumentoFornecedor = cliente.CodigoDocumentoFornecedor;
            pessoa.ExigirNumeroControleCliente = cliente.ExigirNumeroControleCliente;
            pessoa.ExigirNumeroNumeroReferenciaCliente = cliente.ExigirNumeroNumeroReferenciaCliente;
            pessoa.NomeFantasia = cliente.NomeFantasia;
            pessoa.RazaoSocial = cliente.Nome;
            pessoa.RGIE = cliente.IE_RG;
            pessoa.CPFCNPJ = cliente.CPF_CNPJ_Formatado;
            pessoa.CPFCNPJSemFormato = cliente.CPF_CNPJ_SemFormato;
            pessoa.NaoEnviarParaDocsys = cliente.GrupoPessoas?.NaoEnviarParaDocsys ?? false;
            pessoa.RegimeTributario = cliente.RegimeTributario ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.NaoInformado;
            pessoa.IndicadorIE = cliente.IndicadorIE;
            pessoa.Observacao = cliente.Observacao;
            pessoa.DadosBancarios = ConverterObjetoDadosBancariosPessoa(cliente);
            pessoa.CPFCNPJSemFormato = cliente.CPF_CNPJ_SemFormato;
            if (!cliente.Ativo)
                pessoa.InativarCliente = true;

            return pessoa;
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterObjetoPessoa(Dominio.Entidades.Cliente cliente)
        {
            if (cliente == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

            pessoa.ClienteExterior = cliente.Tipo == "E" ? true : false;

            if (pessoa.ClienteExterior)
                pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
            else
                pessoa.TipoPessoa = cliente.Tipo == "J" ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica;

            pessoa.Protocolo = cliente.Codigo;
            pessoa.Codigo = cliente.CodigoIntegracao;
            pessoa.CodigoAtividade = cliente.Atividade.Codigo;
            pessoa.CodigoIntegracao = !string.IsNullOrWhiteSpace(cliente.CodigoIntegracao) ? cliente.CodigoIntegracao : cliente.CPF_CNPJ.ToString();
            pessoa.Email = cliente.Email;
            pessoa.Endereco = ConverterObjetoEnderecoPessoa(cliente);
            pessoa.IM = cliente.InscricaoMunicipal;
            pessoa.CodigoDocumento = cliente.CodigoDocumento;
            pessoa.CodigoDocumentoFornecedor = cliente.CodigoDocumentoFornecedor;
            pessoa.ExigirNumeroControleCliente = cliente.ExigirNumeroControleCliente;
            pessoa.ExigirNumeroNumeroReferenciaCliente = cliente.ExigirNumeroNumeroReferenciaCliente;
            pessoa.NomeFantasia = cliente.Tipo != "F" ? cliente.NomeFantasia : "";
            pessoa.RazaoSocial = cliente.Nome;
            pessoa.RGIE = cliente.IE_RG;
            pessoa.CPFCNPJ = cliente.CPF_CNPJ_Formatado;
            pessoa.CPFCNPJSemFormato = cliente.CPF_CNPJ_SemFormato;
            pessoa.NaoEnviarParaDocsys = cliente.GrupoPessoas?.NaoEnviarParaDocsys ?? false;
            pessoa.RegimeTributario = cliente.RegimeTributario ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.NaoInformado;

            if (cliente.IndicadorIE != null && Enum.IsDefined(typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE), cliente.IndicadorIE))
                pessoa.IndicadorIE = cliente.IndicadorIE;

            pessoa.Observacao = cliente.Observacao;
            pessoa.DadosBancarios = ConverterObjetoDadosBancariosPessoa(cliente);
            pessoa.CPFCNPJSemFormato = cliente.CPF_CNPJ_SemFormato;
            pessoa.CodigoAlternativo = cliente.CodigoAlternativo;
            pessoa.GrupoPessoa = ConverterObjetoGrupoPessoa(cliente.GrupoPessoas);

            if (!cliente.Ativo)
                pessoa.InativarCliente = true;

            return pessoa;
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterObjetoPessoa(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco)
        {
            if (cliente == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

            pessoa.ClienteExterior = cliente.Tipo == "E" ? true : false;

            if (pessoa.ClienteExterior)
                pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
            else
                pessoa.TipoPessoa = cliente.Tipo == "J" ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica;

            pessoa.Codigo = cliente.CodigoIntegracao;
            pessoa.CodigoAtividade = cliente.Atividade.Codigo;
            pessoa.CodigoIntegracao = !string.IsNullOrWhiteSpace(cliente.CodigoIntegracao) ? cliente.CodigoIntegracao : cliente.CPF_CNPJ.ToString();
            pessoa.Email = cliente.Email;
            if (pedidoEndereco != null)
                pessoa.Endereco = ConverterObjetoEnderecoPessoa(pedidoEndereco);
            else
                pessoa.Endereco = ConverterObjetoEnderecoPessoa(cliente);

            pessoa.IM = cliente.InscricaoMunicipal;
            pessoa.CodigoDocumento = cliente.CodigoDocumento;
            pessoa.CodigoDocumentoFornecedor = cliente.CodigoDocumentoFornecedor;
            pessoa.ExigirNumeroControleCliente = cliente.ExigirNumeroControleCliente;
            pessoa.ExigirNumeroNumeroReferenciaCliente = cliente.ExigirNumeroNumeroReferenciaCliente;
            pessoa.NomeFantasia = cliente.NomeFantasia;
            pessoa.RazaoSocial = cliente.Nome;
            pessoa.RGIE = cliente.IE_RG;
            pessoa.CPFCNPJ = cliente.CPF_CNPJ_Formatado;
            pessoa.CPFCNPJSemFormato = cliente.CPF_CNPJ_SemFormato;
            pessoa.NaoEnviarParaDocsys = cliente.GrupoPessoas?.NaoEnviarParaDocsys ?? false;
            pessoa.RegimeTributario = cliente.RegimeTributario ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.NaoInformado;
            pessoa.IndicadorIE = cliente.IndicadorIE;
            pessoa.Observacao = cliente.Observacao;
            pessoa.DadosBancarios = ConverterObjetoDadosBancariosPessoa(cliente);

            return pessoa;
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterObjetoEmpresa(Dominio.Entidades.Empresa empresa)
        {
            if (empresa != null)
            {
                Servicos.Embarcador.Localidades.Localidade serLocalidade = new Servicos.Embarcador.Localidades.Localidade();
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                pessoa.ClienteExterior = empresa.Tipo == "E" ? true : false;

                if (pessoa.ClienteExterior)
                    pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                else
                {
                    pessoa.TipoPessoa = empresa.Tipo == "J" ? Dominio.Enumeradores.TipoPessoa.Juridica : Dominio.Enumeradores.TipoPessoa.Fisica;
                }

                pessoa.CodigoAtividade = 4;
                pessoa.CodigoIntegracao = empresa.CNPJ_SemFormato.ToString();
                pessoa.Email = empresa.Email;
                pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                pessoa.Endereco.Bairro = empresa.Bairro;
                pessoa.Endereco.CEP = empresa.CEP;
                pessoa.Endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(empresa.Localidade);
                pessoa.Endereco.Complemento = empresa.Complemento;
                pessoa.Endereco.Telefone = empresa.Telefone;
                pessoa.Endereco.InscricaoEstadual = empresa.InscricaoEstadual;
                pessoa.Endereco.Logradouro = empresa.Endereco;
                pessoa.Endereco.Numero = empresa.Numero;
                pessoa.IM = empresa.InscricaoMunicipal;
                pessoa.NomeFantasia = empresa.NomeFantasia;
                pessoa.RazaoSocial = empresa.RazaoSocial;
                pessoa.RGIE = empresa.InscricaoEstadual;
                pessoa.CPFCNPJ = empresa.CNPJ;
                pessoa.CPFCNPJSemFormato = empresa.CNPJ;
                pessoa.CodigoDocumento = empresa.CodigoDocumento;

                return pessoa;
            }
            else
            {
                return null;
            }

        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterObjetoParticipamenteCTe(Dominio.Entidades.ParticipanteCTe participante)
        {
            if (participante != null)
            {
                Servicos.Embarcador.Localidades.Localidade serLocalidade = new Servicos.Embarcador.Localidades.Localidade();
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

                string codigoDocumento = "";
                if (participante.Cliente != null && participante.Cliente.Enderecos != null && participante.Cliente.Enderecos.Count > 0)
                    codigoDocumento = participante.Cliente.Enderecos.Where(o => o.IE_RG == participante.IE_RG)?.Select(o => o.CodigoDocumento)?.FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(codigoDocumento))
                    codigoDocumento = participante.Cliente?.CodigoDocumento ?? "";

                pessoa.ClienteExterior = participante.Exterior;
                pessoa.TipoPessoa = participante.Tipo;

                if (participante.Atividade != null)
                    pessoa.CodigoAtividade = participante.Atividade.Codigo;

                if (!string.IsNullOrWhiteSpace(participante.Cliente?.CodigoIntegracao))
                    pessoa.CodigoIntegracao = participante.Cliente.CodigoIntegracao;
                else
                    pessoa.CodigoIntegracao = participante.CPF_CNPJ;

                pessoa.Email = participante.Email;
                pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                pessoa.Endereco.Bairro = participante.Bairro;
                pessoa.Endereco.CEP = participante.CEP;
                pessoa.Endereco.CEPSemFormato = participante.CEP_SemFormato;
                pessoa.Endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(participante.Localidade);
                pessoa.Endereco.Complemento = participante.Complemento;
                pessoa.Endereco.Telefone = participante.Telefone1;
                pessoa.Endereco.InscricaoEstadual = participante.IE_RG;
                pessoa.Endereco.Logradouro = participante.Endereco;
                pessoa.Endereco.Numero = participante.Numero;
                pessoa.IM = participante.InscricaoMunicipal;
                pessoa.NomeFantasia = participante.NomeFantasia;
                pessoa.RazaoSocial = participante.Nome;
                pessoa.RGIE = participante.IE_RG;
                pessoa.CPFCNPJ = participante.CPF_CNPJ_Formatado;
                pessoa.CPFCNPJSemFormato = participante.CPF_CNPJ;
                pessoa.InscricaoSuframa = participante.InscricaoSuframa;
                pessoa.CodigoDocumento = codigoDocumento;
                pessoa.NaoEnviarParaDocsys = participante.GrupoPessoas?.NaoEnviarParaDocsys ?? false;
                if (!string.IsNullOrWhiteSpace(participante.Cliente?.CodigoAlternativo ?? string.Empty))
                    pessoa.CodigoAlternativo = participante.Cliente?.CodigoAlternativo ?? string.Empty;

                return pessoa;
            }
            else
            {
                return null;
            }

        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterObjetoParticipamenteNFSe(Dominio.Entidades.ParticipanteNFSe participante)
        {
            if (participante != null)
            {
                Servicos.Embarcador.Localidades.Localidade serLocalidade = new Servicos.Embarcador.Localidades.Localidade();
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                pessoa.ClienteExterior = participante.Exterior;
                pessoa.TipoPessoa = participante.Tipo;
                pessoa.CodigoAtividade = participante.Atividade.Codigo;
                pessoa.CodigoIntegracao = participante.CPF_CNPJ.ToString();
                pessoa.Email = participante.Email;
                pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                pessoa.Endereco.Bairro = participante.Bairro;
                pessoa.Endereco.CEP = participante.CEP;
                pessoa.Endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(participante.Localidade);
                pessoa.Endereco.Complemento = participante.Complemento;
                pessoa.Endereco.Telefone = participante.Telefone1;
                pessoa.Endereco.InscricaoEstadual = participante.IE_RG;
                pessoa.Endereco.Logradouro = participante.Endereco;
                pessoa.Endereco.Numero = participante.Numero;
                pessoa.IM = participante.InscricaoMunicipal;
                pessoa.NomeFantasia = participante.NomeFantasia;
                pessoa.RazaoSocial = participante.Nome;
                pessoa.RGIE = participante.IE_RG;
                pessoa.CPFCNPJ = participante.CPF_CNPJ_Formatado;
                pessoa.CPFCNPJSemFormato = participante.CPF_CNPJ;

                return pessoa;
            }
            else
            {
                return null;
            }

        }

        public List<anl.documentation.ctetransport.cteShipper> ConverterObjetoParticipantecteShipper(Dominio.Entidades.ParticipanteCTe participante, Dominio.Enumeradores.TipoPessoaCTe? tipoPessoaCTE)
        {
            List<anl.documentation.ctetransport.cteShipper> retornoPessoa = new List<anl.documentation.ctetransport.cteShipper>();
            anl.documentation.ctetransport.cteShipper pessoa = new anl.documentation.ctetransport.cteShipper();

            if (participante != null)
            {
                pessoa.customerCode = !string.IsNullOrWhiteSpace(participante.CodigoIntegracao) ? participante.CodigoIntegracao : participante.CPF_CNPJ_SemFormato;
                pessoa.legalName = participante.Nome;
                pessoa.customerTax = participante.CPF_CNPJ;
                pessoa.customerIE = participante.IE_RG;
                pessoa.streetNumber = participante.Numero;
                pessoa.streetName = participante.Localidade.Descricao;
                pessoa.poBoxNumber = participante.Complemento;
                pessoa.unitNumber = "";
                pessoa.district = participante.Bairro;
                pessoa.cityCode = participante.Localidade.CodigoCidade;
                pessoa.countryName = participante.Localidade.Pais.Descricao;
                pessoa.iso2CountryCode = participante.Localidade.Pais.Sigla;
                pessoa.subdivisionCode = participante.Localidade.Estado.Sigla;
                pessoa.subdivisionTypeCode = participante.Localidade.Estado.CodigoEstado;
                pessoa.subdivisionName = participante.Localidade.Estado.Nome;
                pessoa.postalCode = participante.CEP;
                retornoPessoa.Add(pessoa);

                return retornoPessoa;
            }
            return retornoPessoa;
        }

        public List<anl.documentation.ctetransport.cteRecipient> ConverterObjetoParticipantecteRecipient(Dominio.Entidades.ParticipanteCTe participante, Dominio.Enumeradores.TipoPessoaCTe? tipoPessoaCTE)
        {
            List<anl.documentation.ctetransport.cteRecipient> retornoPessoa = new List<anl.documentation.ctetransport.cteRecipient>();
            anl.documentation.ctetransport.cteRecipient pessoa = new anl.documentation.ctetransport.cteRecipient();

            if (participante != null)
            {
                pessoa.customerCode = !string.IsNullOrWhiteSpace(participante.CodigoIntegracao) ? participante.CodigoIntegracao : participante.CPF_CNPJ_SemFormato;
                pessoa.legalName = participante.Nome;
                pessoa.customerTax = participante.CPF_CNPJ;
                pessoa.customerIE = participante.IE_RG;
                pessoa.streetNumber = participante.Numero;
                pessoa.streetName = participante.Localidade.Descricao;
                pessoa.poBoxNumber = participante.Complemento;
                pessoa.unitNumber = "";
                pessoa.district = participante.Bairro;
                pessoa.cityCode = participante.Localidade.CodigoCidade;
                pessoa.countryName = participante.Localidade.Pais.Descricao;
                pessoa.iso2CountryCode = participante.Localidade.Pais.Sigla;
                pessoa.subdivisionCode = participante.Localidade.Estado.Sigla;
                pessoa.subdivisionTypeCode = participante.Localidade.Estado.CodigoEstado;
                pessoa.subdivisionName = participante.Localidade.Estado.Nome;
                pessoa.postalCode = participante.CEP;
                retornoPessoa.Add(pessoa);

                return retornoPessoa;
            }
            return retornoPessoa;
        }

        public List<anl.documentation.ctetransport.cteSender> ConverterObjetoParticipantecteSender(Dominio.Entidades.ParticipanteCTe participante, Dominio.Enumeradores.TipoPessoaCTe? tipoPessoaCTE)
        {
            List<anl.documentation.ctetransport.cteSender> retornoPessoa = new List<anl.documentation.ctetransport.cteSender>();
            anl.documentation.ctetransport.cteSender pessoa = new anl.documentation.ctetransport.cteSender();

            if (participante != null)
            {
                pessoa.customerCode = !string.IsNullOrWhiteSpace(participante.CodigoIntegracao) ? participante.CodigoIntegracao : participante.CPF_CNPJ_SemFormato;
                pessoa.legalName = participante.Nome;
                pessoa.customerTax = participante.CPF_CNPJ;
                pessoa.customerIE = participante.IE_RG;
                pessoa.streetNumber = participante.Numero;
                pessoa.streetName = participante.Localidade.Descricao;
                pessoa.poBoxNumber = participante.Complemento;
                pessoa.unitNumber = "";
                pessoa.district = participante.Bairro;
                pessoa.cityCode = participante.Localidade.CodigoCidade;
                pessoa.countryName = participante.Localidade.Pais.Descricao;
                pessoa.iso2CountryCode = participante.Localidade.Pais.Sigla;
                pessoa.subdivisionCode = participante.Localidade.Estado.Sigla;
                pessoa.subdivisionTypeCode = participante.Localidade.Estado.CodigoEstado;
                pessoa.subdivisionName = participante.Localidade.Estado.Nome;
                pessoa.postalCode = participante.CEP;
                retornoPessoa.Add(pessoa);

                return retornoPessoa;
            }
            return retornoPessoa;
        }

        public List<anl.documentation.ctetransport.cteReceiver> ConverterObjetoParticipantecteReceiver(Dominio.Entidades.ParticipanteCTe participante, Dominio.Enumeradores.TipoPessoaCTe? tipoPessoaCTE)
        {
            List<anl.documentation.ctetransport.cteReceiver> retornoPessoa = new List<anl.documentation.ctetransport.cteReceiver>();
            anl.documentation.ctetransport.cteReceiver pessoa = new anl.documentation.ctetransport.cteReceiver();

            if (participante != null)
            {
                pessoa.customerCode = !string.IsNullOrWhiteSpace(participante.CodigoIntegracao) ? participante.CodigoIntegracao : participante.CPF_CNPJ_SemFormato;
                pessoa.legalName = participante.Nome;
                pessoa.customerTax = participante.CPF_CNPJ;
                pessoa.customerIE = participante.IE_RG;
                pessoa.streetNumber = participante.Numero;
                pessoa.streetName = participante.Localidade.Descricao;
                pessoa.poBoxNumber = participante.Complemento;
                pessoa.unitNumber = "";
                pessoa.district = participante.Bairro;
                pessoa.cityCode = participante.Localidade.CodigoCidade;
                pessoa.countryName = participante.Localidade.Pais.Descricao;
                pessoa.iso2CountryCode = participante.Localidade.Pais.Sigla;
                pessoa.subdivisionCode = participante.Localidade.Estado.Sigla;
                pessoa.subdivisionTypeCode = participante.Localidade.Estado.CodigoEstado;
                pessoa.subdivisionName = participante.Localidade.Estado.Nome;
                pessoa.postalCode = participante.CEP;
                retornoPessoa.Add(pessoa);

                return retornoPessoa;
            }
            return retornoPessoa;
        }

        public List<anl.documentation.ctetransport.cteConsignee> ConverterObjetoParticipantecteConsignee(Dominio.Entidades.ParticipanteCTe participante, Dominio.Enumeradores.TipoPessoaCTe? tipoPessoaCTE)
        {
            List<anl.documentation.ctetransport.cteConsignee> retornoPessoa = new List<anl.documentation.ctetransport.cteConsignee>();
            anl.documentation.ctetransport.cteConsignee pessoa = new anl.documentation.ctetransport.cteConsignee();

            if (participante != null)
            {
                pessoa.customerCode = !string.IsNullOrWhiteSpace(participante.CodigoIntegracao) ? participante.CodigoIntegracao : participante.CPF_CNPJ_SemFormato;
                pessoa.legalName = participante.Nome;
                pessoa.customerTax = participante.CPF_CNPJ;
                pessoa.customerIE = participante.IE_RG;
                pessoa.streetNumber = participante.Numero;
                pessoa.streetName = participante.Localidade.Descricao;
                pessoa.poBoxNumber = participante.Complemento;
                pessoa.unitNumber = "";
                pessoa.district = participante.Bairro;
                pessoa.cityCode = participante.Localidade.CodigoCidade;
                pessoa.countryName = participante.Localidade.Pais.Descricao;
                pessoa.iso2CountryCode = participante.Localidade.Pais.Sigla;
                pessoa.subdivisionCode = participante.Localidade.Estado.Sigla;
                pessoa.subdivisionTypeCode = participante.Localidade.Estado.CodigoEstado;
                pessoa.subdivisionName = participante.Localidade.Estado.Nome;
                pessoa.postalCode = participante.CEP;
                retornoPessoa.Add(pessoa);

                return retornoPessoa;
            }
            return retornoPessoa;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarCliente(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa clienteIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Log.TratarErro($"SalvarCliente: {(clienteIntegracao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(clienteIntegracao) : string.Empty)}", "Request");
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Cliente servicoCliente = new Cliente(_unitOfWork.StringConexao);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = servicoCliente.ConverterObjetoValorPessoa(clienteIntegracao, "Cliente", _unitOfWork, 0, clienteIntegracao?.AtualizarEnderecoPessoa ?? false, false, auditado, tipoServicoMultisoftware, clienteIntegracao.AdicionarComoOutroEndereco, true, null, null, null, true);

            if (retorno.Status == false)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(retorno?.Mensagem ?? "");

            if (retorno.cliente == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(retorno?.Mensagem ?? "");

            Auditoria.Auditoria.Auditar(auditado, retorno.cliente, "Salvou cliente", _unitOfWork);
            _unitOfWork.CommitChanges();

            if (configuracao.UtilizaEmissaoMultimodal && retorno.CodigoDocumentoAnterior != retorno.NovoCodigoDocumento && !string.IsNullOrWhiteSpace(retorno.NovoCodigoDocumento))
            {
                Servicos.Log.TratarErro($"Atualização de localidade e CAR_CARGA_INTEGRADA_EMBARCADOR para false pelo webservice Pessoa", "AtualizacaoCargaIntegradaEmbarcador");

                bool atualizarTodos = string.IsNullOrWhiteSpace(retorno.CodigoDocumentoAnterior);
                DbConnection connection = _unitOfWork.GetConnection();
                DbTransaction transaction = connection.BeginTransaction();
                Servicos.Embarcador.Pessoa.Pessoa.VerificarCargasEmitidasAnteriormente(clienteIntegracao.CPFCNPJ, atualizarTodos, connection, transaction);
                transaction.Commit();
            }

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> BuscarClientesPendentesIntegracao(int quantidade)
        {
            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Cliente> repositorioCliente = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Cliente>(_unitOfWork);
            int totalRegistrosPendentes = repositorioCliente.ContarRegistroPendenteIntegracao();

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa> retorno = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>(),
                NumeroTotalDeRegistro = totalRegistrosPendentes
            };

            if (totalRegistrosPendentes == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>>.CriarRetornoSucesso(retorno);

            IList<Dominio.Entidades.Cliente> clientesPendenteIntegracao = repositorioCliente.BuscarRegitrosPendenteIntegracao(quantidade);

            foreach (Dominio.Entidades.Cliente clientePendente in clientesPendenteIntegracao)
                retorno.Itens.Add(ConverterObjetoPessoa(clientePendente));

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>>.CriarRetornoSucesso(retorno);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoPessoa(List<long> protocolos)
        {
            if (protocolos == null && protocolos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Precisa informar os protocolos que serar comfirmados");

            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Cliente> repositorioCliente = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Cliente>(_unitOfWork);
            IList<Dominio.Entidades.Cliente> listaClienteParaConfirmar = repositorioCliente.BuscarRegitrosPendentesIntegracaoPeloProtocolos(protocolos);
            List<long> protocolosNaoProcessado = protocolos.Where(c => !listaClienteParaConfirmar.Any(m => m.Codigo == c)).ToList();

            foreach (Dominio.Entidades.Cliente cliente in listaClienteParaConfirmar)
            {
                cliente.IntegradoERP = true;
                repositorioCliente.Atualizar(cliente);
            }

            if (protocolosNaoProcessado == null || protocolosNaoProcessado.Count > 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, $"Para os Protocolo(s) {string.Join(",", protocolosNaoProcessado)} Não foram encontrados registros ou ja foram comfirmados.");

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Todos os protocolo confirmadosw com sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarClienteComplementar(Dominio.ObjetosDeValor.Embarcador.Pessoas.PessoaComplementar clienteComplementarIntegracao)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteComplementar repClienteComplementar = new Repositorio.Embarcador.Pessoas.ClienteComplementar(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Cliente cliente = null;

            string cpfcnpj = Utilidades.String.OnlyNumbers(clienteComplementarIntegracao.CPFCNPJ);

            if (!string.IsNullOrEmpty(cpfcnpj))
            {
                if (clienteComplementarIntegracao.ClienteExterior)
                    cliente = repCliente.BuscarPorCPFCNPJ(cpfcnpj.ToDouble());

                if (Utilidades.Validate.ValidarCPFCNPJ(cpfcnpj))
                    cliente = repCliente.BuscarPorCPFCNPJ(cpfcnpj.ToDouble());
            }
            else if (!string.IsNullOrEmpty(clienteComplementarIntegracao.CodigoIntegracao))
                cliente = repCliente.BuscarPorCodigoIntegracao(clienteComplementarIntegracao.CodigoIntegracao);

            if (cliente == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Nenhum Cliente Encontrado!");

            Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar clienteComplementar = repClienteComplementar.BuscarPorCliente(cliente.CPF_CNPJ);

            if (clienteComplementar == null)
            {
                clienteComplementar = new Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar();
                clienteComplementar.Cliente = cliente;
            }

            clienteComplementar.CodigoMatriz = clienteComplementarIntegracao.CodigoMatriz;
            clienteComplementar.Matriz = clienteComplementarIntegracao.Matriz;
            clienteComplementar.EquipeVendas = clienteComplementarIntegracao.EquipeVendas;
            clienteComplementar.EscritorioVendas = clienteComplementarIntegracao.EscritorioVendas;
            clienteComplementar.CanalDistribuicao = clienteComplementarIntegracao.CanalDistribuicao;
            clienteComplementar.Abordagem = clienteComplementarIntegracao.Abordagem;
            clienteComplementar.ClienteCD = clienteComplementarIntegracao.ClienteCD?.ToUpper() == "SIM";
            clienteComplementar.SegundaRemessa = clienteComplementarIntegracao.SegundaRemessa?.ToUpper() == "SIM";
            clienteComplementar.ExclusividadeEntrega = clienteComplementarIntegracao.ExclusividadeEntrega?.ToUpper() == "SIM";
            clienteComplementar.Paletizacao = clienteComplementarIntegracao.Paletizacao;
            clienteComplementar.ClienteStrechado = clienteComplementarIntegracao.ClienteStrechado?.ToUpper() == "SIM";
            clienteComplementar.Agendamento = clienteComplementarIntegracao.Agendamento;
            clienteComplementar.ClienteComMulta = clienteComplementarIntegracao.ClienteComMulta?.ToUpper() == "SIM";
            clienteComplementar.EquipeVendasFP = clienteComplementarIntegracao.EquipeVendasFP;
            clienteComplementar.EscritorioVendasFP = clienteComplementarIntegracao.EscritorioVendasFP;
            clienteComplementar.MatrizReferencia = clienteComplementarIntegracao.MatrizReferencia;
            clienteComplementar.DescricaoTipoVeiculo = clienteComplementarIntegracao.DescricaoTipoVeiculo;
            clienteComplementar.ParticionamentoVeiculo = clienteComplementarIntegracao.ParticionamentoVeiculo;
            clienteComplementar.DescricaoParticionamentoVeiculo = clienteComplementarIntegracao.DescricaoParticionamentoVeiculo;
            clienteComplementar.PagamentoDescarga = clienteComplementarIntegracao.PagamentoDescarga;
            clienteComplementar.DescricaoPagamentoDescarga = clienteComplementarIntegracao.DescricaoPagamentoDescarga;
            clienteComplementar.AlturaRecebimento = clienteComplementarIntegracao.AlturaRecebimento;
            clienteComplementar.DescricaoAlturaRecebimento = clienteComplementarIntegracao.DescricaoAlturaRecebimento;
            clienteComplementar.RestricaoCarregamento = clienteComplementarIntegracao.RestricaoCarregamento;
            clienteComplementar.DescricaoRestricaoCarregamento = clienteComplementarIntegracao.DescricaoRestricaoCarregamento;
            clienteComplementar.ComposicaoPalete = clienteComplementarIntegracao.ComposicaoPalete;
            clienteComplementar.DescricaoComposicaoPalete = clienteComplementarIntegracao.DescricaoComposicaoPalete;
            clienteComplementar.SegundaFeira = clienteComplementarIntegracao.SegundaFeira;
            clienteComplementar.TercaFeira = clienteComplementarIntegracao.TercaFeira;
            clienteComplementar.QuartaFeira = clienteComplementarIntegracao.QuartaFeira;
            clienteComplementar.QuintaFeira = clienteComplementarIntegracao.QuintaFeira;
            clienteComplementar.SextaFeira = clienteComplementarIntegracao.SextaFeira;
            clienteComplementar.Sabado = clienteComplementarIntegracao.Sabado;
            clienteComplementar.Domingo = clienteComplementarIntegracao.Domingo;
            clienteComplementar.CapacidadeRecebimento = clienteComplementarIntegracao.CapacidadeRecebimento;
            clienteComplementar.CustoDescarga = clienteComplementarIntegracao.CustoDescarga;
            clienteComplementar.TipoCusto = clienteComplementarIntegracao.TipoCusto;
            clienteComplementar.Ajudantes = clienteComplementarIntegracao.Ajudantes;
            clienteComplementar.HoraRecebimentoCliente = clienteComplementarIntegracao.HoraRecebimentoCliente;
            clienteComplementar.RegraPallet = clienteComplementarIntegracao.RegraPallet.ToString().ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegraPallet>();

            if (clienteComplementar.Codigo > 0)
                repClienteComplementar.Atualizar(clienteComplementar);
            else
                repClienteComplementar.Inserir(clienteComplementar);

            if (!string.IsNullOrEmpty(clienteComplementar.EquipeVendas))
            {
                Dominio.Entidades.Usuario usuarioVendas = new Dominio.Entidades.Usuario();
                usuarioVendas = repositorioUsuario.BuscarPorNome(clienteComplementar.EquipeVendas);
                if (usuarioVendas != null)
                {
                    usuarioVendas.ClientesSetor.Add(cliente);
                    repositorioUsuario.Atualizar(usuarioVendas);
                }
            }

            if (!string.IsNullOrEmpty(clienteComplementar.EquipeVendasFP))
            {
                Dominio.Entidades.Usuario usuarioEquipeVendasFP = new Dominio.Entidades.Usuario();
                usuarioEquipeVendasFP = repositorioUsuario.BuscarPorNome(clienteComplementar.EquipeVendasFP);
                if (usuarioEquipeVendasFP != null)
                {
                    usuarioEquipeVendasFP.ClientesSetor.Add(cliente);
                    repositorioUsuario.Atualizar(usuarioEquipeVendasFP);
                    Auditoria.Auditoria.Auditar(_auditado, usuarioEquipeVendasFP, "Adicionou um cliente complementar ao usuário", _unitOfWork);
                }
            }

            if (!string.IsNullOrEmpty(clienteComplementar.EscritorioVendas))
            {
                Dominio.Entidades.Usuario usuarioEscritorioVendas = new Dominio.Entidades.Usuario();
                usuarioEscritorioVendas = repositorioUsuario.BuscarPorNome(clienteComplementar.EscritorioVendas);
                if (usuarioEscritorioVendas != null)
                {
                    usuarioEscritorioVendas.ClientesSetor.Add(cliente);
                    repositorioUsuario.Atualizar(usuarioEscritorioVendas);
                }
            }

            cliente.RegraPallet = clienteComplementar.RegraPallet;

            repCliente.Atualizar(cliente);

            new Servicos.Embarcador.GestaoPallet.RegraPalletHistorico(_unitOfWork).SalvarNovaRegraPeriodo(cliente);

            Auditoria.Auditoria.Auditar(_auditado, cliente, "Salvou dados complementares do cliente", _unitOfWork);
            _unitOfWork.CommitChanges();

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Dados salvos com Sucesso!!!");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> BuscarPessoas(int inicio, int limite, bool consultarApenasAtualizados)
        {
            Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> retorno = new Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>();
                    Servicos.WebService.Pessoas.Pessoa serWSPessoa = new Servicos.WebService.Pessoas.Pessoa(_unitOfWork);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                    IList<Dominio.Entidades.Cliente> pessoas = null;

                    if (consultarApenasAtualizados)
                    {
                        pessoas = repCliente.BuscarPorDataAtualizacao(DateTime.Now.AddDays(-1), "CPF_CNPJ", "desc", inicio, limite);
                        retorno.Objeto.NumeroTotalDeRegistro = repCliente.ContarBuscarPorDataAtualizacao(DateTime.Now.AddDays(-1));
                    }
                    else
                    {
                        pessoas = repCliente.BuscarTodos(inicio, limite, "CPF_CNPJ", false);
                        retorno.Objeto.NumeroTotalDeRegistro = repCliente.ContarTodos();
                    }
                    List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa> listaRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>();

                    foreach (Dominio.Entidades.Cliente pessoa in pessoas)
                        listaRetorno.Add(serWSPessoa.ConverterObjetoPessoa(pessoa));

                    retorno.Objeto.Itens = listaRetorno;
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(_auditado, "Buscou pessoas", _unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar as Pessoas";
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return retorno;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco ConverterObjetoEnderecoPessoa(Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco)
        {
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Servicos.Embarcador.Localidades.Localidade();

            return new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
            {
                Bairro = pedidoEndereco.Bairro,
                CEP = pedidoEndereco.CEP,
                Cidade = serLocalidade.ConverterObjetoLocalidade(pedidoEndereco.Localidade),
                Complemento = pedidoEndereco.Complemento,
                Telefone = pedidoEndereco.Telefone,
                InscricaoEstadual = pedidoEndereco.IE_RG,
                Logradouro = pedidoEndereco.Endereco,
                Numero = pedidoEndereco.Numero
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco ConverterObjetoEnderecoPessoa(Dominio.Entidades.Cliente cliente, string numeroCarga = null)
        {
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Servicos.Embarcador.Localidades.Localidade();

            string enderecoConcatenado = $"{cliente.Endereco?.Trim() ?? ""}, {cliente.Numero?.Trim() ?? ""} - {cliente.Complemento?.Trim() ?? ""}";

            return new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
            {
                Bairro = cliente.Bairro,
                CEP = cliente.CEP,
                Cidade = serLocalidade.ConverterObjetoLocalidade(cliente.Localidade),
                Complemento = cliente.Complemento,
                Telefone = cliente.Telefone1,
                InscricaoEstadual = cliente.IE_RG,
                Logradouro = cliente.Endereco,
                Numero = cliente.Numero,
                Latitude = cliente.Latitude,
                Longitude = cliente.Longitude,
                EnderecoConcatenado = enderecoConcatenado,
                DadosComplementares = ConverterObjetoEnderecoDadoComplementarPessoa(cliente, enderecoConcatenado, numeroCarga)//NOTFIS
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Localidade.EnderecoDadosComplementares ConverterObjetoEnderecoDadoComplementarPessoa(Dominio.Entidades.Cliente cliente, string enderecoConcatenado, string numeroCarga)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Localidade.EnderecoDadosComplementares()
            {
                Bairro = cliente.Bairro,
                Complemento = cliente.Complemento,
                Endereco = enderecoConcatenado,
                NumeroCarga = numeroCarga
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosBancarios ConverterObjetoDadosBancariosPessoa(Dominio.Entidades.Cliente cliente)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.DadosBancarios()
            {
                Banco = ConverterObjetoBanco(cliente.Banco),
                PortadorConta = ConverterObjetoPessoa(cliente.ClientePortadorConta),
                Agencia = cliente.Agencia,
                DigitoAgencia = cliente.DigitoAgencia,
                NumeroConta = cliente.NumeroConta,
                TipoContaBanco = cliente.TipoContaBanco ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente
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