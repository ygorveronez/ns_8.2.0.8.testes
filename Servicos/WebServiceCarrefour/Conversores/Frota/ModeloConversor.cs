namespace Servicos.WebServiceCarrefour.Conversores.Frota
{
    public sealed class ModeloConversor
    {
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Modelo Converter(Dominio.Entidades.ModeloVeiculo modeloConverter)
        {
            if (modeloConverter == null)
                return null;

            MarcaConverter servicoConverterMarca = new MarcaConverter();
            Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Modelo modelo = new Dominio.ObjetosDeValor.WebServiceCarrefour.Frota.Modelo();

            modelo.Ativo = modeloConverter.Status == "A" ? true : false;
            modelo.CodigoFIPE = modeloConverter.CodigoFIPE;
            modelo.CodigoIntegracao = modeloConverter.CodigoIntegracao;
            modelo.Descricao = modeloConverter.Descricao;
            modelo.Marca = servicoConverterMarca.Converter(modeloConverter.MarcaVeiculo);
            modelo.NumeroEixos = modeloConverter.NumeroEixo;
            modelo.PossuiArla32 = modeloConverter.PossuiArla32 == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim ? true : false;

            return modelo;
        }

        #endregion
    }
}
