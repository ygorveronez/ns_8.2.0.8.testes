using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Veiculo
{
    public sealed class VeiculoGR
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Contrutores

        public VeiculoGR(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR Validar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Veiculo veiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoBrasilRisk = ValidarBrasilRisk(carga, veiculo);
            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoAdagio = ValidarAdagio(carga, veiculo);

            return retornoBrasilRisk ?? retornoAdagio;
        }

        public Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR ValidarAdagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Veiculo veiculo)
        {
            if ((veiculo == null) || !(carga.TipoOperacao?.ValidarMotoristaVeiculoAdagioAoConfirmarTransportador ?? false))
                return null;

            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retorno = new Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR();
            bool falhaIntegracaoGrVeiculo = false;

            retorno.Mensagem = Servicos.Embarcador.Veiculo.Veiculo.ConsultarVeiculoAdagio(veiculo, ref falhaIntegracaoGrVeiculo, _unitOfWork);
            retorno.Sucesso = !falhaIntegracaoGrVeiculo;
            retorno.TipoIntegracao = TipoIntegracao.Adagio;

            if (!retorno.Sucesso)
                retorno.Mensagem = $"{retorno.TipoIntegracao.Value.ObterDescricao()} Veículo {veiculo.Placa}: {retorno.Mensagem}";

            return retorno;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR ValidarBrasilRisk(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Veiculo veiculo)
        {
            if ((veiculo == null) || !(carga.TipoOperacao?.ValidarMotoristaVeiculoBrasilRiskAoConfirmarTransportador ?? false))
                return null;

            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retorno = new Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR();
            bool falhaIntegracaoGrVeiculo = false;

            retorno.Mensagem = Servicos.Embarcador.Veiculo.Veiculo.ConsultarVeiculoBrasilRisk(veiculo, ref falhaIntegracaoGrVeiculo, carga.Codigo, _unitOfWork);
            retorno.Sucesso = !falhaIntegracaoGrVeiculo;
            retorno.TipoIntegracao = TipoIntegracao.BrasilRisk;

            return retorno;
        }

        #endregion
    }
}
