using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Transportadores
{
    public sealed class MotoristaMobile
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly string _urlBaseOrigemRequisicao;

        #endregion

        #region Construtores

        public MotoristaMobile(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, urlBaseOrigemRequisicao: string.Empty) { }

        public MotoristaMobile(Repositorio.UnitOfWork unitOfWork, string urlBaseOrigemRequisicao)
        {
            _unitOfWork = unitOfWork;
            _urlBaseOrigemRequisicao = urlBaseOrigemRequisicao;
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Logistica.ManobraTracao> DesvincularTracaoMotorista(Dominio.Entidades.Usuario motorista, Logistica.ManobraMobile servicoManobra, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            List<Dominio.Entidades.Veiculo> veiculos = repositorioVeiculo.BuscarVeiculosPorMotorista(motorista.Codigo);
            List<Dominio.Entidades.Embarcador.Logistica.ManobraTracao> listaManobraTracaoAlteradas = new List<Dominio.Entidades.Embarcador.Logistica.ManobraTracao>();

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                //veiculo.Motorista = null;
                //veiculo.CPFMotorista = string.Empty;
                //veiculo.NomeMotorista = string.Empty;
                //repositorioVeiculo.Atualizar(veiculo);

                Servicos.Auditoria.Auditoria.Auditar(auditado, veiculo, $"Removido motorista principal.", _unitOfWork);
                repVeiculoMotorista.DeletarMotoristaPrincipal(veiculo.Codigo);
                Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracao = servicoManobra.DesvincularManobraTracao(motorista, veiculo);

                if (manobraTracao != null)
                    listaManobraTracaoAlteradas.Add(manobraTracao);
            }

            return listaManobraTracaoAlteradas;
        }

        private void ValidarMotoristaNaFilaCarregamento(Dominio.Entidades.Usuario motorista)
        {
            Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Logistica.FilaCarregamentoVeiculo(_unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorMotoristaNaFilaSemValidacaoNulo(motorista.Codigo);

            if (filaCarregamentoVeiculo != null)
                throw new ServicoException($"O motorista está na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}.");
        }

        private void ValidarVeiculoNaFilaCarregamentoOutroMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Veiculo veiculo)
        {
            Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Logistica.FilaCarregamentoVeiculo(_unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculo(veiculo.Codigo, motorista.Codigo);

            if (filaCarregamentoVeiculo != null)
                throw new ServicoException($"O veiculo {veiculo.Placa_Formatada} está na fila {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()} com o motorista {filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Descricao}.");
        }

        #endregion

        #region Métodos Privados de Consulta

        private Dominio.Entidades.Veiculo ObterTracaoPorMotorista(Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorMotorista(motorista.Codigo) ?? throw new ServicoException("Veículo do motorista não encontrada");

            if (!veiculo.IsTipoVeiculoTracao())
                throw new ServicoException($"Veículo {veiculo.Placa_Formatada} vinculado ao motorista não é uma tração");

            if ((veiculo.ModeloVeicularCarga?.NumeroReboques ?? 0) < 1)
                throw new ServicoException($"O medelo veícular do veículo {veiculo.Placa_Formatada} vinculado ao motorista não possui reboques");

            return veiculo;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Veiculo AlterarReboque(Dominio.Entidades.Usuario motorista, string placa)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Dominio.Entidades.Veiculo reboque = repositorioVeiculo.BuscarPorPlacaVarrendoFiliais(motorista.Empresa.Codigo, placa) ?? throw new ServicoException("Veículo não encontrado");

            if (!reboque.IsTipoVeiculoReboque())
                throw new ServicoException("Veículo informado não é um reboque");

            ValidarMotoristaNaFilaCarregamento(motorista);
            ValidarVeiculoNaFilaCarregamentoOutroMotorista(motorista, reboque);

            Dominio.Entidades.Veiculo tracao = ObterTracaoPorMotorista(motorista);

            if ((tracao.Empresa != null) && (reboque.Empresa != null) && (tracao.Empresa.Codigo != reboque.Empresa.Codigo))
                throw new ServicoException("Transportador do Veículo informado e da tração são diferentes");

            try
            {
                _unitOfWork.Start();

                if (reboque.VeiculosTracao?.Count > 0)
                {
                    Dominio.Entidades.Veiculo veiculoTracaoReboque = reboque.VeiculosTracao.FirstOrDefault();

                    if (veiculoTracaoReboque.Codigo != tracao.Codigo)
                    {
                        veiculoTracaoReboque.VeiculosVinculados.Clear();
                        repositorioVeiculo.Atualizar(veiculoTracaoReboque);
                    }
                }

                reboque.Empresa = tracao.Empresa;

                tracao.VeiculosVinculados?.Clear();
                tracao.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>() { reboque };

                repositorioVeiculo.Atualizar(reboque);
                repositorioVeiculo.Atualizar(tracao);

                _unitOfWork.CommitChanges();

                return tracao;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.Veiculo AlterarTracao(Dominio.Entidades.Usuario motorista, string placa, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlacaVarrendoFiliais(motorista.Empresa.Codigo, placa) ?? throw new ServicoException("Veículo não encontrado");

            if (!veiculo.IsTipoVeiculoTracao())
                throw new ServicoException("Veículo informado não é uma tração");

            ValidarMotoristaNaFilaCarregamento(motorista);
            ValidarVeiculoNaFilaCarregamentoOutroMotorista(motorista, veiculo);

            Logistica.ManobraMobile servicoManobra = new Logistica.ManobraMobile(_unitOfWork, _urlBaseOrigemRequisicao);

            try
            {
                _unitOfWork.Start();
                List<Dominio.Entidades.Embarcador.Logistica.ManobraTracao> listaManobraTracaoDesvinculada = DesvincularTracaoMotorista(motorista, servicoManobra, auditado);

                //veiculo.Motorista = motorista;
                //veiculo.CPFMotorista = motorista.CPF;
                //veiculo.NomeMotorista = motorista.Nome;

                //repositorioVeiculo.Atualizar(veiculo);

                Servicos.Auditoria.Auditoria.Auditar(auditado, veiculo, $"Removido motorista principal.", _unitOfWork);
                repVeiculoMotorista.DeletarMotoristaPrincipal(veiculo.Codigo);

                Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracaoVinculada = servicoManobra.VincularManobraTracao(motorista, veiculo);

                _unitOfWork.CommitChanges();

                foreach (Dominio.Entidades.Embarcador.Logistica.ManobraTracao manobraTracaoDesvinculada in listaManobraTracaoDesvinculada)
                    servicoManobra.NotificarManobraTracaoAlterada(manobraTracaoDesvinculada.Codigo);

                if (manobraTracaoVinculada != null)
                    servicoManobra.NotificarManobraTracaoAlterada(manobraTracaoVinculada.Codigo);

                return veiculo;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void FinalizarJornada(Dominio.Entidades.Usuario motorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            MotoristaJornada servicoMotoristaJornada = new MotoristaJornada(_unitOfWork);

            servicoMotoristaJornada.FinalizarJornada(motorista, tipoServicoMultisoftware);
        }

        public void IniciarJornada(Dominio.Entidades.Usuario motorista)
        {
            MotoristaJornada servicoMotoristaJornada = new MotoristaJornada(_unitOfWork);

            servicoMotoristaJornada.IniciarJornada(motorista);
        }

        #endregion
    }
}
