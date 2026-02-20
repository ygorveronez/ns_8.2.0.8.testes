namespace Servicos.WebServiceCarrefour.Conversores.Carga
{
    public sealed class ModeloVeicularCargaConversor
    {
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.ModeloVeicular Converter(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCargaConverter)
        {
            if (modeloVeicularCargaConverter == null)
                return null;

            Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.ModeloVeicular modeloVeicularCarga = new Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.ModeloVeicular();

            modeloVeicularCarga.CodigoIntegracao = modeloVeicularCargaConverter.CodigoIntegracao;
            modeloVeicularCarga.Descricao = modeloVeicularCargaConverter.Descricao;
            modeloVeicularCarga.TipoModeloVeicular = modeloVeicularCargaConverter.Tipo;

            return modeloVeicularCarga;
        }

        #endregion
    }
}
