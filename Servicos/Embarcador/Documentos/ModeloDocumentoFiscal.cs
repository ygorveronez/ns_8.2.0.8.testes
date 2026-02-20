namespace Servicos.Embarcador.Documentos
{
    public sealed class ModeloDocumentoFiscal
    {
        private static ModeloDocumentoFiscal _Instancia;

        public Dominio.Entidades.ModeloDocumentoFiscal ModeloNFe { get; private set; }
        public Dominio.Entidades.ModeloDocumentoFiscal ModeloOutras { get; private set; }

        private ModeloDocumentoFiscal()
        {

        }

        public static ModeloDocumentoFiscal GetInstance(Repositorio.UnitOfWork unitOfWork)
        {
            if (_Instancia == null)
            {
                _Instancia = new ModeloDocumentoFiscal();
                _Instancia.CarregarModelos(unitOfWork);
            }
            return _Instancia;
        }

        private void CarregarModelos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            ModeloNFe = repModeloDocumentoFiscal.BuscarPorModelo("55");
            ModeloOutras = repModeloDocumentoFiscal.BuscarPorModelo("99");
        }
    }
}
