namespace Servicos.Embarcador.Veiculo
{
    public class VeiculoHistorico
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public VeiculoHistorico(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public static void InserirHistoricoVeiculo(Dominio.Entidades.Veiculo veiculo, bool? situacaoAnterior, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MetodosAlteracaoVeiculo metodoAlteracao, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            if (situacaoAnterior != null && veiculo.Ativo != situacaoAnterior)
            {
                Repositorio.Embarcador.Veiculos.VeiculoHistorico repHistoricoVeiculo = new Repositorio.Embarcador.Veiculos.VeiculoHistorico(unitOfWork);

                repHistoricoVeiculo.Inserir(new Dominio.Entidades.Embarcador.Veiculos.VeiculoHistorico()
                {
                    Data = System.DateTime.Now,
                    Situacao = veiculo.Ativo,
                    Usuario = usuario,
                    Veiculo = veiculo,
                    MetodoAlteracao = metodoAlteracao
                });
            }
        }
        #endregion
    }
}
