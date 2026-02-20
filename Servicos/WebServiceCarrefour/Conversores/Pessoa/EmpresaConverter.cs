using System;

namespace Servicos.WebServiceCarrefour.Conversores.Pessoa
{
    public sealed class EmpresaConverter
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public EmpresaConverter(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Certificado ObterCertificado(Dominio.Entidades.Empresa empresaConverter)
        {
            if (!empresaConverter.DataInicialCertificado.HasValue || !empresaConverter.DataFinalCertificado.HasValue || string.IsNullOrEmpty(empresaConverter.NomeCertificado))
                return null;

            Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Certificado certificado = new Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Certificado();

            certificado.DataInicio = empresaConverter.DataInicialCertificado.Value;
            certificado.DataFim = empresaConverter.DataFinalCertificado.Value;

            if (certificado.DataInicio <= DateTime.Now && certificado.DataFim >= DateTime.Now.Date)
                certificado.CertificadoAtivo = true;

            return certificado;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Empresa Converter(Dominio.Entidades.Cliente cliente)
        {
            if (cliente == null)
                return null;

            Localidade.EnderecoConverter servicoConverterEndereco = new Localidade.EnderecoConverter(_unitOfWork);
            Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Empresa empresaConvertida = new Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Empresa();

            empresaConvertida.CNPJ = cliente.CPF_CNPJ_SemFormato;
            empresaConvertida.IE = cliente.IE_RG;
            empresaConvertida.NomeFantasia = cliente.NomeFantasia;
            empresaConvertida.RazaoSocial = cliente.Nome;
            empresaConvertida.InscricaoMunicipal = cliente.InscricaoMunicipal;
            empresaConvertida.InscricaoST = "";
            empresaConvertida.Endereco = servicoConverterEndereco.Converter(cliente);

            return empresaConvertida;
        }

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Empresa Converter(Dominio.Entidades.Empresa empresaConverter)
        {
            if (empresaConverter == null)
                return null;

            Localidade.EnderecoConverter servicoConverterEndereco = new Localidade.EnderecoConverter(_unitOfWork);
            Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Empresa empresa = new Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.Empresa();

            empresa.CNPJ = empresaConverter.CNPJ;
            empresa.IE = empresaConverter.InscricaoEstadual;
            empresa.NomeFantasia = empresaConverter.NomeFantasia;
            empresa.RazaoSocial = empresaConverter.RazaoSocial;
            empresa.CodigoIntegracao = empresaConverter.CodigoIntegracao;
            empresa.RNTRC = empresaConverter.RegistroANTT;
            empresa.CodigoDocumento = empresaConverter.CodigoDocumento;
            empresa.SimplesNacional = empresaConverter.OptanteSimplesNacional;
            empresa.InscricaoMunicipal = empresaConverter.InscricaoMunicipal;
            empresa.InscricaoST = empresaConverter.Inscricao_ST;
            empresa.EmissaoDocumentosForaDoSistema = empresaConverter.EmissaoDocumentosForaDoSistema;
            empresa.LiberacaoParaPagamentoAutomatico = empresaConverter.LiberacaoParaPagamentoAutomatico;
            empresa.Certificado = ObterCertificado(empresaConverter);
            empresa.Endereco = servicoConverterEndereco.Converter(empresaConverter);

            return empresa;
        }

        #endregion
    }
}
