namespace Servicos.Embarcador.Carga
{
    public class CargaFechamento
    {

        private Repositorio.UnitOfWork _unitOfWork;
        private string _stringConexao;

        public CargaFechamento(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, stringConexao: null) { }

        public CargaFechamento(Repositorio.UnitOfWork unitOfWork, string stringConexao = null)
        {
            _unitOfWork = unitOfWork;
            _stringConexao = stringConexao;
        }


        public Dominio.Entidades.Embarcador.Cargas.CargaFechamento GerarCargaFechamento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaFechamento repCargaFechamento = new Repositorio.Embarcador.Cargas.CargaFechamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaFechamento fechamento = repCargaFechamento.BuscarPorCarga(carga.Codigo);

            if (fechamento == null)
                fechamento = new Dominio.Entidades.Embarcador.Cargas.CargaFechamento();

            fechamento.Carga = carga;
            fechamento.MotivoRejeicaoCalculoFrete = "";
            fechamento.SituacaoFechamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaFechamento.AgRateio;
            fechamento.ValorFrete = carga.ValorFrete;
            fechamento.ValorRecalculado = 0;

            if (fechamento.Codigo > 0)
                repCargaFechamento.Atualizar(fechamento);
            else
                repCargaFechamento.Inserir(fechamento);


            return fechamento;
        }







    }
}
