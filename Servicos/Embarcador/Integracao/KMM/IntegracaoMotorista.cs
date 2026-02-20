using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.KMM;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas;

namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {
        #region Métodos Públicos

        public void IntegrarMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao motoristaIntegracao)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            motoristaIntegracao.DataIntegracao = DateTime.Now;
            motoristaIntegracao.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Pessoa objetoMotorista = ObterMotorista(motoristaIntegracao.Motorista, configuracaoIntegracaoKMM);

                Hashtable request = new Hashtable
                {
                    { "module", "M3012" },
                    { "operation", "manipulaPessoa" },
                    { "parameters", objetoMotorista }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                motoristaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                motoristaIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                motoristaIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(motoristaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioMotoristaIntegracao.Atualizar(motoristaIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Pessoa ObterMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Pessoa();

            retorno.Operation = "INSERT";
            retorno.CodPessoa = null;
            retorno.Tipo = motorista.Tipo == "J" ? "PJ" : "PF";
            retorno.InscricaoEstadual = "ISENTO";
            retorno.IdExterno = motorista.CPF.ToString().PadLeft(11, '0');
            retorno.CNAE = null;
            retorno.CodContabil = null;
            retorno.DataCadastro = motorista.DataCadastro?.ToString("yyyy-MM-dd");
            retorno.Observacoes = motorista.Observacao;
            retorno.Modalidades = this.ObterMotoristaModalidade(motorista);
            retorno.PessoaFisica = this.ObterMotoristaPessoaFisica(motorista);
            retorno.PessoaJuridica = this.ObterMotoristaPessoaJuridica(motorista);
            retorno.Documentos = this.ObterMotoristaDocumento(motorista);
            retorno.RNTRC = this.ObterMotoristaRNTRC(motorista);
            retorno.Enderecos = this.ObterMotoristaEnderecos(motorista);
            retorno.Telefones = this.ObterMotoristaTelefones(motorista);
            retorno.Emails = this.ObterMotoristaEmails(motorista);
            retorno.ContasBancarias = this.ObterMotoristaContasBancarias(motorista);

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ModalidadePessoa> ObterMotoristaModalidade(Dominio.Entidades.Usuario motorista)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ModalidadePessoa> retorno = new List<ModalidadePessoa>();

            var modalidade = new ModalidadePessoa();
            modalidade.Ativo = "true";
            modalidade.NumModalidade = 8;
            modalidade.Operation = "INSERT";
            retorno.Add(modalidade);

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.PessoaFisica ObterMotoristaPessoaFisica(Dominio.Entidades.Usuario motorista)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.PessoaFisica();

            retorno.Operation = "INSERT";
            retorno.CPF = motorista?.CPF.ToString().PadLeft(11, '0');
            retorno.Nome = motorista.Nome;
            retorno.NomeSocial = motorista.Nome;
            retorno.Sexo = motorista.Sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Masculino ? "M" : "F";
            retorno.DataNascimento = motorista.DataNascimento?.ToString("yyyy-MM-dd");
            retorno.EstadoCivil = this.ObterEstadoCivilKMM(motorista.EstadoCivil);
            retorno.RacaCor = this.ObterRacaCorKMM(motorista.CorRaca);
            //retorno.GrauInstrucao = ??;

            if (motorista.LocalidadeNascimento != null)
            {
                retorno.PaisNacionalidadeESocial = motorista.LocalidadeNascimento.Pais.Sigla.Substring(0, motorista.LocalidadeNascimento.Pais.Sigla.Length - 1).ToInt().ToString().PadLeft(3, '0');
                retorno.PaisNascimentoESocial = motorista.LocalidadeNascimento.Pais.Sigla.Substring(0, motorista.LocalidadeNascimento.Pais.Sigla.Length - 1).ToInt().ToString().PadLeft(3, '0');
                retorno.Naturalidade = new Naturalidade();
                retorno.Naturalidade.NatMunicipioId = null;
                retorno.Naturalidade.NaturalidadeDesc = motorista.LocalidadeNascimento.Descricao;
                retorno.Naturalidade.NaturalidadeUF = motorista.LocalidadeNascimento.Estado.Sigla;
                retorno.Naturalidade.NatMunicipioIBGE = motorista.LocalidadeNascimento.CodigoIBGE.ToString();
                retorno.Naturalidade.NatPaisId = motorista.LocalidadeNascimento.Pais.Sigla.ToInt();
            }

            Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato pai = motorista.Contatos?.Where(o => o.TipoParentesco == TipoParentesco.Pai)?.FirstOrDefault() ?? null;
            Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato mae = motorista.Contatos?.Where(o => o.TipoParentesco == TipoParentesco.Mae)?.FirstOrDefault() ?? null;

            retorno.CpfMae = mae?.CPF;
            retorno.NomeMae = mae?.Nome;
            retorno.CpfPai = pai?.CPF;
            retorno.NomePai = pai?.Nome;
            //retorno.PossuiDefFisica = ??;
            //retorno.PossuiDefVisual = ??;
            //retorno.PossuiDefAuditiva = ??;
            //retorno.PossuiDefMental = ??;
            //retorno.PossuiDefIntelectual = ??;
            //retorno.PossuiDefReabilitado = ??;
            //retorno.InfosDeficiencia = ??;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.PessoaJuridica ObterMotoristaPessoaJuridica(Dominio.Entidades.Usuario motorista)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.PessoaJuridica();

            retorno.Operation = "INSERT";
            retorno.CNPJ = motorista?.CPF.ToString().PadLeft(14, '0');
            retorno.NomeFantasia = motorista.Nome;
            retorno.RazaoSocial = motorista.Nome;
            retorno.InscricaoEstadual = "ISENTO";
            retorno.RazaoSocialResumida = motorista.Nome;
            retorno.AtividadeFiscal = 9;
            //retorno.Alvara = ??;
            //retorno.CertificadoOTM = ??;
            //retorno.RegimeTributario = ??;
            //retorno.Suframa = ??;
            //retorno.TipoLucro = ??;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Documento ObterMotoristaDocumento(Dominio.Entidades.Usuario motorista)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Documento retorno = null;

            if (motorista?.TipoPessoa == "F" || motorista.TipoPessoa == null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Documento();

                if (!string.IsNullOrEmpty(motorista.PIS))
                {
                    retorno.PisPasepNit = Regex.Replace(motorista.PIS, @"[^\d]", "");
                }
                else
                {
                    Repositorio.Cliente repGrupoPessoas = new Repositorio.Cliente(_unitOfWork);
                    Dominio.Entidades.Cliente pessoa = repGrupoPessoas.BuscarPorCPFCNPJ(double.Parse(motorista.CPF));

                    if (pessoa != null)
                    {
                        retorno.PisPasepNit = Regex.Replace(pessoa.PISPASEP, @"[^\d]", "");
                    }
                }                

                if (!string.IsNullOrEmpty(motorista.RG))
                {
                    var carteiraIdentidade = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.CarteiraIdentidade();
                    carteiraIdentidade.Numero = motorista.RG;
                    carteiraIdentidade.DataEmissao = motorista.DataEmissaoRG?.ToString("yyyy-MM-dd");
                    carteiraIdentidade.OrgaoEmissor = motorista.OrgaoEmissorRG?.ToString();
                    carteiraIdentidade.UF = motorista.EstadoRG?.Descricao;
                    retorno.CarteiraIdentidade = carteiraIdentidade;
                }

                if (!string.IsNullOrEmpty(motorista.TituloEleitoral))
                {
                    var tituloEleitor = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.TituloEleitor();
                    tituloEleitor.Numero = motorista.TituloEleitoral;
                    //tituloEleitor.DV = ??;
                    tituloEleitor.Secao = motorista.SecaoEleitoral;
                    tituloEleitor.Zona = motorista.ZonaEleitoral;
                    retorno.TituloEleitor = tituloEleitor;
                }

                if (!string.IsNullOrEmpty(motorista.NumeroHabilitacao))
                {
                    var cnh = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.CNH();
                    cnh.NumeroRegistro = motorista.NumeroRegistroHabilitacao;
                    cnh.Numero = motorista.NumeroHabilitacao;
                    cnh.Categoria = motorista.Categoria;
                    cnh.UF = motorista.UFEmissaoCNH?.Sigla;
                    cnh.OrgaoEmissor = motorista.UFEmissaoCNH?.Sigla;
                    cnh.DataEmissao = motorista.DataHabilitacao?.ToString("yyyy-MM-dd");
                    cnh.DataPrimeiraHabilitacao = motorista.DataPrimeiraHabilitacao?.ToString("yyyy-MM-dd");
                    cnh.DataValidade = motorista.DataVencimentoHabilitacao?.ToString("yyyy-MM-dd");
                    cnh.Renach = motorista.RenachHabilitacao;
                    retorno.CNH = cnh;
                }

                if (!string.IsNullOrEmpty(motorista.NumeroCTPS))
                {
                    var carteiraTrabalho = new CarteiraTrabalho();
                    carteiraTrabalho.Numero = motorista.NumeroCTPS;
                    carteiraTrabalho.Serie = motorista.SerieCTPS;
                    carteiraTrabalho.UF = motorista.EstadoCTPS?.Sigla;
                    retorno.CarteiraTrabalho = carteiraTrabalho;
                }

                retorno.CarteiraReservista = null;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RNTRC ObterMotoristaRNTRC(Dominio.Entidades.Usuario motorista)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RNTRC();

            //retorno.DataEmissao = ??;
            //retorno.DataVencimento = ??;
            //retorno.Numero = ??;
            retorno.TipoTransportador = "TAC";

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.EnderecoKMM> ObterMotoristaEnderecos(Dominio.Entidades.Usuario motorista)
        {
            var retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.EnderecoKMM>();

            var endereco = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.EnderecoKMM();
            endereco.Operation = "UPDATE";
            if (motorista.Localidade == null || string.IsNullOrEmpty(motorista.Localidade.CEP))
            {
                throw new Exception("Verifique se o motorista possui localidade vinculada e se a mesma possui um CEP.");
            }


            if (!string.IsNullOrEmpty(motorista.CEP))
            {
                endereco.IdExterno = motorista.CEP;
                endereco.CEP = motorista.CEP;
            }
            else
            {
                endereco.IdExterno = motorista.Localidade?.CEP;
                endereco.CEP = motorista.Localidade?.CEP;
            }

            //endereco.CodEndereco = ??;
            endereco.EnderecoPadrao = true;
            endereco.Municipio = motorista.Localidade?.DescricaoCidadeEstado;
            endereco.UF = motorista.Localidade?.Estado.Sigla;
            //endereco.MunicipioId = ??;
            endereco.MunicipioIBGE = motorista.Localidade?.CodigoIBGE.ToString();
            if (motorista.Localidade?.Pais != null)
            {
                endereco.CodigoBacen = motorista.Localidade.Pais.Sigla.ToInt();
            }
            endereco.Logradouro = motorista.Endereco;
            endereco.Numero = motorista.NumeroEndereco;
            endereco.Complemento = motorista.Complemento;
            endereco.Bairro = motorista.Bairro;
            endereco.Tipo = 1;
            endereco.AtividadeFiscal = 9; //Não Informado
            endereco.InscricaoEstadual = "ISENTO";
            retorno.Add(endereco);

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone> ObterMotoristaTelefones(Dominio.Entidades.Usuario motorista)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone> retorno = null;

            if (!string.IsNullOrEmpty(motorista.Telefone))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone>();

                var telefone1 = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Telefone();
                telefone1.Operation = "UPDATE";
                //telefone1.TelefoneId = ??;
                telefone1.TelefonePadrao = true;
                telefone1.Tipo = 1;

                var retObterTelefoneKMM = this.ObterTelefoneKMM(motorista?.Localidade?.Pais?.Sigla, motorista.Telefone);
                telefone1.IdExterno = retObterTelefoneKMM.ToString();
                telefone1.DDI = retObterTelefoneKMM.ddi;
                telefone1.DDD = retObterTelefoneKMM.ddd;
                telefone1.Prefixo = retObterTelefoneKMM.prefixo;
                telefone1.Numero = retObterTelefoneKMM.numero;
                //telefone1.Ramal = ??;
                //telefone1.Contato = ??;
                retorno.Add(telefone1);
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email> ObterMotoristaEmails(Dominio.Entidades.Usuario motorista)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email> retorno = null;

            if (!string.IsNullOrEmpty(motorista.Email))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email>();

                var email = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Email();
                email.Operation = "UPDATE";
                email.IdExterno = motorista.Email.Length > 200 ? motorista.Email.Substring(0, 200) : motorista.Email;
                //email.EmailId = ??;
                email.EmailPadrao = true;

                int arroba = motorista.Email.IndexOf('@');
                if (arroba > -1)
                {
                    email.Username = motorista.Email.Substring(0, arroba);
                    email.Provedor = motorista.Email.Substring(arroba + 1);
                }
                else
                {
                    email.Username = motorista.Email;
                    email.Provedor = motorista.Email;
                }

                email.Tipo = 1;
                email.Proprietario = motorista.Nome;
                //email.Observacao = ??;

                retorno.Add(email);
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContaBancaria> ObterMotoristaContasBancarias(Dominio.Entidades.Usuario motorista)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContaBancaria> retorno = null;

            if (!string.IsNullOrEmpty(motorista.NumeroConta) && motorista.Banco != null)
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContaBancaria>();

                var contaBancaria = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ContaBancaria();

                contaBancaria.Operation = "UPDATE";
                contaBancaria.IdExterno = motorista.Banco?.Codigo.ToString() + motorista.NumeroConta;
                //contaBancaria.ContaId = ??;
                contaBancaria.Padrao = true;
                contaBancaria.Tipo = 1;
                contaBancaria.Banco = motorista.Banco?.Codigo.ToString();
                contaBancaria.Agencia = motorista.Agencia;
                contaBancaria.ContaAgDv = motorista.DigitoAgencia;
                contaBancaria.Conta = motorista.NumeroConta;
                //contaBancaria.ContaDv = ??;
                contaBancaria.CodPessoaTitular = null;
                //contaBancaria.Operacao = ??;
                //contaBancaria.InstituicaoId = ??;
                //contaBancaria.InstituicaoConta = ??;
                retorno.Add(contaBancaria);
            }

            return retorno;
        }

        #endregion Métodos Privados
    }
}