namespace Servicos.WebServiceCarrefour.Conversores.Frota
{
    public sealed class MarcaConverter
    {
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Marca Converter(Dominio.Entidades.MarcaVeiculo marcaConverter)
        {
            if (marcaConverter == null)
                return null;

            Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Marca marca = new Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Marca();

            marca.Ativo = marcaConverter.Status == "A";
            marca.CodigoIntegracao = marcaConverter.CodigoIntegracao;
            marca.Descricao = marcaConverter.Descricao;
            marca.TipoVeiculo = marcaConverter.TipoVeiculo.HasValue ? marcaConverter.TipoVeiculo.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao;

            return marca;
        }

        #endregion
    }
}
