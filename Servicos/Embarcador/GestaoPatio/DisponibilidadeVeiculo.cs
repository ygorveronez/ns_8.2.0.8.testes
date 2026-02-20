using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public class DisponibilidadeVeiculo
    {
        public static void SetaVeiculoDisponivel(int codigoVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.DisponibilidadeVeiculo repDisponibilidadeVeiculo = new Repositorio.Embarcador.GestaoPatio.DisponibilidadeVeiculo(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo repContratoFreteTransportadorVeiculo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo(unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo disponibilidadeVeiculo = repDisponibilidadeVeiculo.BuscarPorVeiculo(codigoVeiculo);

            if (disponibilidadeVeiculo != null)
            {
                disponibilidadeVeiculo.Disponivel = null;
                //disponibilidadeVeiculo.EmViagem = null;

                repDisponibilidadeVeiculo.Atualizar(disponibilidadeVeiculo);
            }
        }

        public static void GeraControleDisponibilidadeVeiculo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.DisponibilidadeVeiculo repDisponibilidadeVeiculo = new Repositorio.Embarcador.GestaoPatio.DisponibilidadeVeiculo(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo repContratoFreteTransportadorVeiculo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo(unitOfWork);

            if (fluxoGestaoPatio != null && fluxoGestaoPatio.Veiculo != null && repContratoFreteTransportadorVeiculo.VeiculoPossuiContratoValido(fluxoGestaoPatio.Veiculo.Codigo, DateTime.Now))
            {
                Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo disponibilidadeVeiculo = repDisponibilidadeVeiculo.BuscarPorVeiculo(fluxoGestaoPatio.Veiculo.Codigo);
                if (disponibilidadeVeiculo == null)
                {
                    disponibilidadeVeiculo = new Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo()
                    {
                        Veiculo = fluxoGestaoPatio.Veiculo,
                        PrevisaoDisponibilidade = fluxoGestaoPatio.DataFimViagemPrevista
                    };
                }

                disponibilidadeVeiculo.Disponivel = null;
                //disponibilidadeVeiculo.EmViagem = DateTime.Now;

                if (disponibilidadeVeiculo.Codigo == 0)
                    repDisponibilidadeVeiculo.Inserir(disponibilidadeVeiculo);
                else
                    repDisponibilidadeVeiculo.Atualizar(disponibilidadeVeiculo);
            }
        }

        public static void FimViagemDisponibilidadeVeiculo(int veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.DisponibilidadeVeiculo repDisponibilidadeVeiculo = new Repositorio.Embarcador.GestaoPatio.DisponibilidadeVeiculo(unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo disponibilidadeVeiculo = repDisponibilidadeVeiculo.BuscarPorVeiculo(veiculo);

            if (disponibilidadeVeiculo != null)
            {
                disponibilidadeVeiculo.Disponivel = DateTime.Now;
                repDisponibilidadeVeiculo.Atualizar(disponibilidadeVeiculo);
            }
        }
    }
}
