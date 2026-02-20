using System.Collections.Generic;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class Higienizacao
    {
        #region Atributos Protegidos

        protected readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Higienizacao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void AtualizarVeiculoParaHigienizado(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo == null)
                return;
            
            veiculo.Higienizado = true;

            new Repositorio.Veiculo(_unitOfWork).Atualizar(veiculo);
        }

        public void AtualizarVeiculoParaNaoHigienizado(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo == null)
                return;

            veiculo.Higienizado = false;

            new Repositorio.Veiculo(_unitOfWork).Atualizar(veiculo);
        }

        public void AtualizarVeiculosParaHigienizado(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            AtualizarVeiculosParaHigienizado(cargaBase.Veiculo, cargaBase.VeiculosVinculados);
        }

        public void AtualizarVeiculosParaHigienizado(Dominio.Entidades.Veiculo veiculo, ICollection<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            AtualizarVeiculoParaHigienizado(veiculo);

            if (veiculosVinculados?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in veiculosVinculados)
                    AtualizarVeiculoParaHigienizado(reboque);
            }
        }

        public void AtualizarVeiculosParaNaoHigienizado(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            AtualizarVeiculosParaNaoHigienizado(cargaBase.Veiculo, cargaBase.VeiculosVinculados);
        }

        public void AtualizarVeiculosParaNaoHigienizado(Dominio.Entidades.Veiculo veiculo, ICollection<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            AtualizarVeiculoParaNaoHigienizado(veiculo);

            if (veiculosVinculados?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in veiculosVinculados)
                    AtualizarVeiculoParaNaoHigienizado(reboque);
            }
        }

        public bool IsVeiculosHigienizados(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase)
        {
            if (((cargaBase.ModeloVeicularCarga?.NumeroReboques ?? 0) == 0) && !(cargaBase.Veiculo?.Higienizado ?? true))
                return false;

            if (cargaBase.VeiculosVinculados?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo veiculo in cargaBase.VeiculosVinculados)
                {
                    if (!veiculo.Higienizado)
                        return false;
                }
            }

            return true;
        }

        public bool IsVeiculosHigienizados(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo)
        {
            if ((conjuntoVeiculo.ModeloVeicularCarga.NumeroReboques == 0) && !(conjuntoVeiculo.Tracao?.Higienizado ?? true))
                return false;

            if (conjuntoVeiculo.Reboques?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboques in conjuntoVeiculo.Reboques)
                {
                    if (!reboques.Higienizado)
                        return false;
                }
            }

            return true;
        }

        public bool IsVeiculosHigienizados(Dominio.Entidades.Veiculo veiculo, ICollection<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            if (veiculosVinculados?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo veiculoVinculado in veiculosVinculados)
                {
                    if (!veiculoVinculado.Higienizado)
                        return false;
                }
            }
            else if (!(veiculo?.Higienizado ?? true))
                return false;

            return true;
        }

        #endregion
    }
}
