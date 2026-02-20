using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public class Participante : ServicoBase
    {
        
        public Participante(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterDynamicParaParticipante(dynamic dynParticipante, Repositorio.UnitOfWork unitOfWork)
        {
            if (dynParticipante == null)
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade();

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa participante = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

            participante.ClienteExterior = (bool)dynParticipante.ParticipanteExterior;
            participante.RazaoSocial = (string)dynParticipante.RazaoSocial;

            participante.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
            participante.Endereco.Logradouro = (string)dynParticipante.Endereco;
            participante.Endereco.Numero = (string)dynParticipante.Numero;
            participante.Endereco.Bairro = (string)dynParticipante.Bairro;
            participante.Endereco.Complemento = (string)dynParticipante.Complemento;

            participante.Email = (string)dynParticipante.EmailGeral;
            participante.EnviarEmail = (bool)dynParticipante.EnviarXMLEmailGeral;
            participante.CodigoDocumento = (string)dynParticipante.CodigoDocumento;
            participante.ExigirNumeroControleCliente = ((string)dynParticipante.ExigirNumeroControleCliente).ToBool();
            participante.ExigirNumeroNumeroReferenciaCliente = ((string)dynParticipante.ExigirNumeroNumeroReferenciaCliente).ToBool();

            participante.AtualizarEnderecoPessoa = true;//(bool)dynParticipante.SalvarEndereco;

            if (participante.ClienteExterior)
            {
                participante.CPFCNPJ = (string)dynParticipante.PessoaExterior;
                participante.AtualizarEnderecoPessoa = false;
            }
            else
                participante.CPFCNPJ = (string)dynParticipante.CPFCNPJ;

            if (string.IsNullOrWhiteSpace(participante.CPFCNPJ))
                return null;

            participante.RGIE = (string)dynParticipante.IE;
            participante.NomeFantasia = (string)dynParticipante.NomeFantasia;
            participante.CodigoAtividade = (int)dynParticipante.Atividade;
            participante.Endereco.Telefone = (string)dynParticipante.TelefonePrincipal;
            participante.Endereco.CEP = (string)dynParticipante.CEP;
            participante.EmailContato = (string)dynParticipante.EmailContato;
            participante.EnviarEmailContato = (bool)dynParticipante.EnviarXMLEmailContato;
            participante.EmailContador = (string)dynParticipante.EmailContador;
            participante.EnviarEmialContador = (bool)dynParticipante.EnviarXMLEmailContador;
            participante.Endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo((int)dynParticipante.Localidade));

            return participante;
        }

        public void SalvarParticipante(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa participante, Dominio.ObjetosDeValor.Endereco endereco, Dominio.Enumeradores.TipoTomador tipoParticipante, bool permissaoTotal, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoesAlteracao, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (participante == null)
                return;

            Servicos.Cliente serCliente = new Cliente(StringConexao);

            string descricaoTipoParticipante = "";
            if (tipoParticipante == Dominio.Enumeradores.TipoTomador.Destinatario)
                descricaoTipoParticipante = "Destinatário";
            else if (tipoParticipante == Dominio.Enumeradores.TipoTomador.Remetente)
                descricaoTipoParticipante = "Remetente";
            else if (tipoParticipante == Dominio.Enumeradores.TipoTomador.Outros)
                descricaoTipoParticipante = "Tomador";
            else if (tipoParticipante == Dominio.Enumeradores.TipoTomador.Expedidor)
                descricaoTipoParticipante = "Expedidor";
            else if (tipoParticipante == Dominio.Enumeradores.TipoTomador.Recebedor)
                descricaoTipoParticipante = "Recebedor";

            Dominio.Entidades.Cliente cliente = null;

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Remetente) || permissaoTotal)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConverterPessoa = serCliente.ConverterObjetoValorPessoa(participante, descricaoTipoParticipante, unitOfWork, 0, false);
                if (retornoConverterPessoa.Status)
                    cliente = retornoConverterPessoa.cliente;
                else
                    mensagem += retornoConverterPessoa.Mensagem;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(participante.CPFCNPJ) && participante.CPFCNPJ != "0")
                {
                    cliente = AlterarCamposPermitidosCliente(permissoesAlteracao, participante, unitOfWork);
                    if (cliente == null)
                        mensagem += "O " + descricaoTipoParticipante + " não está cadastrado na base Multisoftware";
                }
            }

            if (cliente == null)
                return;

            if (cliente.Tipo != "E")
                cte.SetarParticipante(cliente, tipoParticipante, endereco);
            else
                cte.SetarParticipanteExportacao(serCliente.ObterClienteCTE(cliente, null), tipoParticipante, cliente.Pais, cliente.Atividade, cliente.Localidade, cliente);
        }

        public Dominio.Entidades.Cliente AlterarCamposPermitidosCliente(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoesAlteracao, Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa participante, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(participante.CPFCNPJ)));
            if (cliente != null)
            {
                if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarIEParticipanete))
                {
                    cliente.IE_RG = participante.RGIE;
                }
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Atualizar(cliente);
            }
            return cliente;
        }

        public void SalvarParticipantePreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa participante, Dominio.ObjetosDeValor.Endereco endereco, Dominio.Enumeradores.TipoTomador tipoParticipante, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (participante != null)
            {
                Servicos.Cliente serCliente = new Cliente(StringConexao);

                string descricaoTipoParticipante = "";
                if (tipoParticipante == Dominio.Enumeradores.TipoTomador.Destinatario)
                    descricaoTipoParticipante = "Destinatário";
                else if (tipoParticipante == Dominio.Enumeradores.TipoTomador.Remetente)
                    descricaoTipoParticipante = "Remetente";
                else if (tipoParticipante == Dominio.Enumeradores.TipoTomador.Outros)
                    descricaoTipoParticipante = "Tomador";
                else if (tipoParticipante == Dominio.Enumeradores.TipoTomador.Expedidor)
                    descricaoTipoParticipante = "Expedidor";
                else if (tipoParticipante == Dominio.Enumeradores.TipoTomador.Recebedor)
                    descricaoTipoParticipante = "Recebedor";

                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConverterPessoa = serCliente.ConverterObjetoValorPessoa(participante, descricaoTipoParticipante, unitOfWork, 0, false);
                if (retornoConverterPessoa.Status)
                {
                    Dominio.Entidades.Cliente cliente = retornoConverterPessoa.cliente;
                    if (cliente.Tipo != "E")
                    {
                        preCTe.SetarParticipante(cliente, tipoParticipante, null, null, endereco);
                    }
                    else
                    {
                        preCTe.SetarParticipanteExportacao(serCliente.ObterClienteCTE(cliente, null), cliente, tipoParticipante, cliente.Localidade.Pais);
                    }
                }
                else
                {
                    mensagem += retornoConverterPessoa.Mensagem;
                }

            }

        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterParticipanteParaParticipante(Dominio.Entidades.ParticipanteCTe participanteCTe, bool enviarCTeApenasParaTomador, bool ehTomador, Repositorio.UnitOfWork unitOfWork)
        {
            if (participanteCTe == null)
                return null;

            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade();

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa participante = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
            participante.CPFCNPJ = participanteCTe.CPF_CNPJ;
            participante.RGIE = participanteCTe.IE_RG;
            participante.RazaoSocial = participanteCTe.Nome;
            participante.NomeFantasia = participanteCTe.NomeFantasia;
            participante.CodigoIntegracao = participanteCTe.Cliente?.CodigoIntegracao ?? "";

            participante.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
            participante.Endereco.Telefone = participanteCTe.Telefone1;

            participante.CodigoAtividade = participanteCTe.Atividade?.Codigo ?? 0;
            participante.Endereco.CEP = participanteCTe.CEP;
            participante.Endereco.Logradouro = participanteCTe.Endereco;
            participante.Endereco.Numero = participanteCTe.Numero;
            participante.Endereco.Bairro = participanteCTe.Bairro;
            participante.Endereco.Complemento = participanteCTe.Complemento;
            participante.Endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(participanteCTe.Localidade, participanteCTe.Pais);

            participante.AtualizarEnderecoPessoa = (participanteCTe.Localidade?.Estado?.Sigla != "EX") && participanteCTe.SalvarEndereco;
            participante.ClienteExterior = participanteCTe.Exterior;

            if (!enviarCTeApenasParaTomador || ehTomador)
            {
                participante.Email = participanteCTe.Email;
                participante.EnviarEmail = participanteCTe.EmailStatus;
                participante.EmailContato = participanteCTe.EmailContato;
                participante.EnviarEmailContato = participanteCTe.EmailContatoStatus;
                participante.EmailContador = participanteCTe.EmailContador;
                participante.EnviarEmialContador = participanteCTe.EmailContadorStatus;
            }

            return participante;
        }

        #endregion
    }
}
