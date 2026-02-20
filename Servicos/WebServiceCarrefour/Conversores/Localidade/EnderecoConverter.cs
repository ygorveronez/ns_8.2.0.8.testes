namespace Servicos.WebServiceCarrefour.Conversores.Localidade
{
    public sealed class EnderecoConverter
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public EnderecoConverter(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco Converter(Dominio.Entidades.Cliente cliente)
        {
            if (cliente == null)
                return null;

            LocalidadeConversor servicoConverterLocalidade = new LocalidadeConversor(_unitOfWork);
            Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco endereco = new Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco();

            endereco.Bairro = cliente.Bairro;
            endereco.CEP = cliente.CEP;
            endereco.Logradouro = cliente.Endereco;
            endereco.Cidade = servicoConverterLocalidade.Converter(cliente.Localidade);
            endereco.Complemento = cliente.Complemento;
            endereco.Telefone = cliente.Telefone1;

            return endereco;
        }

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco Converter(Dominio.Entidades.Empresa empresa)
        {
            if (empresa == null)
                return null;

            LocalidadeConversor servicoConverterLocalidade = new LocalidadeConversor(_unitOfWork);
            Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco endereco = new Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco();

            endereco.Bairro = empresa.Bairro;
            endereco.CEP = empresa.CEP;
            endereco.Logradouro = empresa.Endereco;
            endereco.Cidade = servicoConverterLocalidade.Converter(empresa.Localidade);
            endereco.Complemento = empresa.Complemento;
            endereco.Telefone = empresa.Telefone;

            return endereco;
        }

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco Converter(Dominio.Entidades.ParticipanteCTe participante)
        {
            if (participante == null)
                return null;

            LocalidadeConversor servicoConverterLocalidade = new LocalidadeConversor(_unitOfWork);
            Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco endereco = new Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco();

            endereco.Bairro = participante.Bairro;
            endereco.CEP = participante.CEP;
            endereco.CEPSemFormato = participante.CEP_SemFormato;
            endereco.Cidade = servicoConverterLocalidade.Converter(participante.Localidade);
            endereco.Complemento = participante.Complemento;
            endereco.Telefone = participante.Telefone1;
            endereco.InscricaoEstadual = participante.IE_RG;
            endereco.Logradouro = participante.Endereco;
            endereco.Numero = participante.Numero;

            return endereco;
        }

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco Converter(Dominio.Entidades.Usuario usuario)
        {
            if (usuario == null)
                return null;

            LocalidadeConversor servicoConverterLocalidade = new LocalidadeConversor(_unitOfWork);
            Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco endereco = new Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Endereco();

            endereco.Bairro = usuario.Bairro;
            endereco.CEP = usuario.CEP;
            endereco.Cidade = servicoConverterLocalidade.Converter(usuario.Localidade);
            endereco.Complemento = usuario.Complemento;
            endereco.Logradouro = usuario.Endereco;
            endereco.Numero = usuario.NumeroEndereco;
            endereco.Telefone = usuario.Telefone;

            return endereco;
        }

        #endregion
    }
}
