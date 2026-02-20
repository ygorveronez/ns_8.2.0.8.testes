namespace Servicos.WebServiceCarrefour.Conversores.Carga
{
    public sealed class MotoristaConverter
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public MotoristaConverter(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Motorista Converter(Dominio.Entidades.Usuario motoristaConverter)
        {
            if (motoristaConverter == null)
                return null;

            Pessoa.EmpresaConverter servicoConverterEmpresa = new Pessoa.EmpresaConverter(_unitOfWork);
            Localidade.EnderecoConverter servicoConverterEndereco = new Localidade.EnderecoConverter(_unitOfWork);

            Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Motorista motorista = new Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Motorista
            {
                Codigo = motoristaConverter.Codigo,
                CodigoIntegracao = motoristaConverter.CodigoIntegracao,
                CPF = motoristaConverter.CPF,
                DataAdmissao = motoristaConverter.DataAdmissao.HasValue ? motoristaConverter.DataAdmissao.Value.ToString("dd/MM/yyyy") : "",
                DataHabilitacao = motoristaConverter.DataHabilitacao.HasValue ? motoristaConverter.DataHabilitacao.Value.ToString("dd/MM/yyyy") : "",
                DataNascimento = motoristaConverter.DataNascimento.HasValue ? motoristaConverter.DataNascimento.Value.ToString("dd/MM/yyyy") : "",
                DataVencimentoHabilitacao = motoristaConverter.DataVencimentoHabilitacao.HasValue ? motoristaConverter.DataVencimentoHabilitacao.Value.ToString("dd/MM/yyyy") : "",
                Email = motoristaConverter.Email,
                Transportador = servicoConverterEmpresa.Converter(motoristaConverter.Empresa),
                Nome = motoristaConverter.Nome,
                NumeroHabilitacao = motoristaConverter.NumeroHabilitacao,
                RG = motoristaConverter.RG,
                Endereco = servicoConverterEndereco.Converter(motoristaConverter)
            };

            return motorista;
        }

        #endregion
    }
}
