namespace Servicos.WebServiceCarrefour.Conversores.Pessoa
{
    public sealed class ParticipanteCteConversor
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ParticipanteCteConversor(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Pessoa Converter(Dominio.Entidades.ParticipanteCTe participanteConverter)
        {
            if (participanteConverter == null)
                return null;

            Localidade.EnderecoConverter servicoConverterEndereco = new Localidade.EnderecoConverter(_unitOfWork);
            Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Pessoa participante = new Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Pessoa();

            participante.ClienteExterior = participanteConverter.Exterior;
            participante.TipoPessoa = participanteConverter.Tipo;
            participante.CodigoIntegracao = participanteConverter.CPF_CNPJ;
            participante.Email = participanteConverter.Email;
            participante.IM = participanteConverter.InscricaoMunicipal;
            participante.NomeFantasia = participanteConverter.NomeFantasia;
            participante.RazaoSocial = participanteConverter.Nome;
            participante.RGIE = participanteConverter.IE_RG;
            participante.CPFCNPJ = participanteConverter.CPF_CNPJ_Formatado;
            participante.CPFCNPJSemFormato = participanteConverter.CPF_CNPJ;
            participante.InscricaoSuframa = participanteConverter.InscricaoSuframa;
            participante.Endereco = servicoConverterEndereco.Converter(participanteConverter);

            if (participanteConverter.Atividade != null)
                participante.CodigoAtividade = participanteConverter.Atividade.Codigo;

            return participante;
        }

        #endregion
    }
}
