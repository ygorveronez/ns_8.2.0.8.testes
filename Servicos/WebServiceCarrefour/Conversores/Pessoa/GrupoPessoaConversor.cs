namespace Servicos.WebServiceCarrefour.Conversores.Pessoa
{
    public sealed class GrupoPessoaConversor
    {
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.GrupoPessoa Converter(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaConverter)
        {
            if (grupoPessoaConverter == null)
                return null;

            Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.GrupoPessoa grupoPessoa = new Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas.GrupoPessoa();

            grupoPessoa.CodigoIntegracao = grupoPessoaConverter.CodigoIntegracao;
            grupoPessoa.Descricao = grupoPessoaConverter.Descricao;

            return grupoPessoa;
        }

        #endregion
    }
}
