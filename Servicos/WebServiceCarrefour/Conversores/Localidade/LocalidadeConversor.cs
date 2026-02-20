namespace Servicos.WebServiceCarrefour.Conversores.Localidade
{
    public sealed class LocalidadeConversor
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public LocalidadeConversor(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Pais ObterPais(Dominio.Entidades.Localidade localidade, Dominio.Entidades.Pais pais)
        {
            Dominio.Entidades.Pais paisConverter = localidade.Pais ?? pais;

            if (paisConverter == null)
                return null;

            return new Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Pais()
            {
                CodigoPais = paisConverter.Codigo,
                NomePais = paisConverter.Nome,
                SiglaPais = paisConverter.Abreviacao
            };
        }

        private Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Regiao ObterRegiao(Dominio.Entidades.Localidade localidade)
        {
            if (localidade.Regiao == null)
                return null;

            Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Regiao regiao = new Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Regiao();

            regiao.CodigoIntegracao = localidade.Regiao.CodigoIntegracao;
            regiao.Descricao = localidade.Regiao.Descricao;

            if (localidade.Regiao.LocalidadePolo != null)
            {
                regiao.CodigoIntegracaoLocalidadePolo = localidade.Regiao.LocalidadePolo.CodigoLocalidadeEmbarcador;
                regiao.IBGELocalidadePolo = localidade.Regiao.LocalidadePolo.CodigoIBGE;
            }

            return regiao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Localidade Converter(Dominio.Entidades.Localidade localidade)
        {
            return Converter(localidade, pais: null);
        }

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Localidade Converter(Dominio.Entidades.Localidade localidade, Dominio.Entidades.Pais pais)
        {
            if (localidade == null)
                return null;

            Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Localidade cidade = new Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade.Localidade
            {
                Codigo = localidade.Codigo,
                CodigoIntegracao = localidade.CodigoLocalidadeEmbarcador,
                Descricao = localidade.Descricao,
                IBGE = localidade.CodigoIBGE,
                SiglaUF = localidade.Estado.Sigla,
                CodigoDocumento = localidade.CodigoDocumento
            };

            cidade.Pais = ObterPais(localidade, pais);
            cidade.Regiao = ObterRegiao(localidade);

            return cidade;
        }

        #endregion
    }
}
