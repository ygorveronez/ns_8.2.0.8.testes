using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public class RotaFreteAbastecimento
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public RotaFreteAbastecimento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void GerarRequisicaoAbastecimento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if(carga != null)
            {
                Repositorio.Embarcador.Logistica.RotaFreteAbastecimento repRotaFreteAbastecimento = new Repositorio.Embarcador.Logistica.RotaFreteAbastecimento(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento> listaRotaFreteAbastecimento = new List<Dominio.Entidades.Embarcador.Logistica.RotaFreteAbastecimento>();

                if (carga.Rota?.Codigo > 0)
                {
                    if (carga.ModeloVeicularCarga?.Codigo > 0)
                        listaRotaFreteAbastecimento = repRotaFreteAbastecimento.BuscarPorRotaEModeloVeicular(carga.Rota.Codigo, carga.ModeloVeicularCarga.Codigo);

                    if (listaRotaFreteAbastecimento.Count == 0)
                        listaRotaFreteAbastecimento = repRotaFreteAbastecimento.BuscarPorRotaEModeloVeicular(carga.Rota.Codigo, 0);

                    if (listaRotaFreteAbastecimento.Count > 0)
                    {
                        foreach (var rotaFreteAbastecimento in listaRotaFreteAbastecimento)
                        {
                            if (rotaFreteAbastecimento.PreAbastecimentos != null && rotaFreteAbastecimento.PreAbastecimentos.Count > 0)
                            {
                                foreach (var preAbastecimento in rotaFreteAbastecimento.PreAbastecimentos)
                                {
                                    Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();

                                    abastecimento.Veiculo = carga.Veiculo;
                                    abastecimento.Motorista = carga.Motoristas?.FirstOrDefault();
                                    abastecimento.Produto = preAbastecimento.Produto;
                                    abastecimento.Posto = preAbastecimento.Posto;
                                    abastecimento.TipoAbastecimento = preAbastecimento.TipoAbastecimento;
                                    abastecimento.ValorUnitario = preAbastecimento.ValorUnitario;

                                    if (preAbastecimento.TanqueCheio)
                                        abastecimento.Litros = carga.Veiculo.CapacidadeMaximaTanque;
                                    else
                                        abastecimento.Litros = preAbastecimento.Litros;

                                    abastecimento.Requisicao = true;
                                    abastecimento.Situacao = "R";
                                    repAbastecimento.Inserir(abastecimento);
                                    
                                    Servicos.Embarcador.Abastecimento.Abastecimento.GerarRequisicaoAutomatica(unitOfWork, abastecimento);

                                }
                            }
                        }
                    }
                }
            }          
        }

        #endregion

        #region Métodos Privados

        

        #endregion
    }
}
