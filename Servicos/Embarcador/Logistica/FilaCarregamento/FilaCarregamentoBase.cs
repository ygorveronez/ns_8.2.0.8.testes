using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public abstract class FilaCarregamentoBase
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga _configuracaoGeralCarga;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga _configuracaoPreCarga;
        protected readonly Repositorio.UnitOfWorkContainer _unitOfWorkContainer;
        protected readonly OrigemAlteracaoFilaCarregamento _origemAlteracao;

        #endregion

        #region Construtores

        public FilaCarregamentoBase(Repositorio.UnitOfWork unitOfWork, OrigemAlteracaoFilaCarregamento origemAlteracao)
        {
            _origemAlteracao = origemAlteracao;
            _unitOfWorkContainer = new Repositorio.UnitOfWorkContainer(unitOfWork);
        }

        public FilaCarregamentoBase(Repositorio.UnitOfWorkContainer unitOfWorkContainer, OrigemAlteracaoFilaCarregamento origemAlteracao)
        {
            _origemAlteracao = origemAlteracao;
            _unitOfWorkContainer = unitOfWorkContainer;
        }

        #endregion

        #region Métodos Privados

        private string ObterMensagemProblemaIntegracaoMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao)
        {
            if (!integracao.TipoIntegracao.Ativo)
                return string.Empty;

            switch (integracao.SituacaoIntegracao)
            {
                case SituacaoIntegracao.AgIntegracao:
                    return $"Aguardando integração{(string.IsNullOrWhiteSpace(integracao.TipoIntegracao?.Descricao) ? "" : $" {integracao.TipoIntegracao.Descricao}")}.";

                case SituacaoIntegracao.AgRetorno:
                    return $"Aguardando retorno da integração{(string.IsNullOrWhiteSpace(integracao.TipoIntegracao?.Descricao) ? "" : $" {integracao.TipoIntegracao.Descricao}")}.";

                case SituacaoIntegracao.ProblemaIntegracao:
                    return $"Você foi retirado da fila automaticamente devido a Bloqueio de motorista, entrar em contato com a sua transportadora.{(string.IsNullOrWhiteSpace(integracao.ProblemaIntegracao) ? "" : $" Motivo: {integracao.ProblemaIntegracao}")}";

                default:
                    return string.Empty;
            }
        }

        private string ObterMensagemProblemaIntegracaoVeiculo(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao)
        {
            if (!integracao.TipoIntegracao.Ativo)
                return string.Empty;

            switch (integracao.SituacaoIntegracao)
            {
                case SituacaoIntegracao.AgIntegracao:
                    return $"Aguardando integração{(string.IsNullOrWhiteSpace(integracao.TipoIntegracao?.Descricao) ? "" : $" {integracao.TipoIntegracao.Descricao}")}.";

                case SituacaoIntegracao.AgRetorno:
                    return $"Aguardando retorno da integração{(string.IsNullOrWhiteSpace(integracao.TipoIntegracao?.Descricao) ? "" : $" {integracao.TipoIntegracao.Descricao}")}.";

                case SituacaoIntegracao.ProblemaIntegracao:
                    if (!string.IsNullOrWhiteSpace(integracao.ProblemaIntegracao))
                    {
                        if (integracao.TipoIntegracao.Tipo == TipoIntegracao.BuonnyRNTRC)
                            return $"Você foi retirado da fila devido ao status da RNTRC, entrar em contato com a sua transportadora. Motivo: {integracao.ProblemaIntegracao}";
                        else if (integracao.TipoIntegracao.Tipo == TipoIntegracao.Buonny)
                            return $"Você foi retirado da fila automaticamente devido a Checklist – Status: {integracao.ProblemaIntegracao}";
                    }

                    return "Você foi retirado da fila automaticamente devido a Bloqueio de veículo, entrar em contato com a sua transportadora.";

                default:
                    return string.Empty;
            }
        }

        private void ValidarCadastroMotoristaAtivo(Dominio.Entidades.Usuario motorista)
        {
            if (motorista.Status == "I")
            {
                if (string.IsNullOrWhiteSpace(motorista.MotivoBloqueio))
                    throw new ServicoException("Cadastro do motorista está inativo, entre em contato com a transportadora.");
                else
                    throw new ServicoException($"Cadastro do motorista esta bloqueado, entre em contato com a transportadora. Motivo: {motorista.MotivoBloqueio}");
            }
        }

        private void ValidarPunicaoVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            DateTime dataHotaAtual = DateTime.Now;

            if (veiculo.DataSuspensaoInicio.HasValue && veiculo.DataSuspensaoFim.HasValue)
            {
                if ((dataHotaAtual >= veiculo.DataSuspensaoInicio.Value) && (dataHotaAtual <= veiculo.DataSuspensaoFim.Value))
                    throw new ServicoException($"Veículo {veiculo.Placa_Formatada} esta suspenso: {veiculo.MotivoBloqueio}");
            }

            Repositorio.Embarcador.Logistica.PunicaoVeiculo repositorioPunicaoVeiculo = new Repositorio.Embarcador.Logistica.PunicaoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo> punicoesVeiculo = repositorioPunicaoVeiculo.BuscarPorVeiculo(veiculo.Codigo);

            foreach (Dominio.Entidades.Embarcador.Logistica.PunicaoVeiculo punicao in punicoesVeiculo)
            {
                if (punicao.Ativo && (punicao.DataInicioPunicao <= dataHotaAtual) && (punicao.DataInicioPunicao.AddDays(punicao.DiasPunicao) >= dataHotaAtual))
                    throw new ServicoException($"{punicao.Motivo.Descricao}");
            }
        }

        private void ValidarRestricoesMotorista(Dominio.Entidades.Usuario motorista)
        {
            if (motorista.Empresa != null)
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

                if (configuracaoEmbarcador.ValidarDataLiberacaoSeguradora)
                {
                    if (!motorista.DataValidadeLiberacaoSeguradora.HasValue)
                        throw new ServicoException("Data da seguradora (GR) não informada, entrar em contato com a transportadora.");

                    if (motorista.DataValidadeLiberacaoSeguradora.Value < DateTime.Today)
                        throw new ServicoException("Data da seguradora (GR) está vencida, entrar em contato com a transportadora.");
                }
            }

            if (!motorista.DataVencimentoHabilitacao.HasValue)
                throw new ServicoException("Data da CNH não informada, entrar em contato com a transportadora.");

            if (motorista.DataVencimentoHabilitacao.Value < DateTime.Today)
                throw new ServicoException("Data da CNH está vencida, entrar em contato com a transportadora.");

            if (motorista.DataSuspensaoInicio.HasValue && motorista.DataSuspensaoFim.HasValue)
            {
                DateTime dataAtual = DateTime.Now;

                if ((dataAtual >= motorista.DataSuspensaoInicio) && (dataAtual <= motorista.DataSuspensaoFim))
                    throw new ServicoException($"Cadastro do motorista está suspenso, entrar em contato com a transportadora.");
            }
        }

        private void ValidarRestricoesVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            Veiculo.Veiculo.ValidarDataLiberacaoSeguradora(veiculo, configuracaoEmbarcador);
        }

        private void ValidarRetornoConsultaIntegracoesMotorista(Dominio.Entidades.Usuario motorista)
        {
            foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao in motorista.Integracoes)
            {
                if (integracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                    throw new ServicoException(ObterMensagemProblemaIntegracaoMotorista(integracao));
            }
        }

        private void ValidarRetornoConsultaIntegracoesVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao in veiculo.Integracoes)
            {
                if (integracao.SituacaoIntegracao != SituacaoIntegracao.Integrado && !veiculo.NaoValidarIntegracaoParaFilaCarregamento)
                    throw new ServicoException(ObterMensagemProblemaIntegracaoVeiculo(integracao));
            }
        }

        #endregion

        #region Métodos Protegidos

        protected bool IsUtilizaFilaCarregamentoMotorista()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWorkContainer.UnitOfWork);

            return repConfiguracaoTMS.BuscarConfiguracaoPadrao()?.UtilizarFilaCarregamento ?? false;
        }

        protected Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamentoPorVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            CentroCarregamento servicoCentroCarregamento = new CentroCarregamento(_unitOfWorkContainer.UnitOfWork);

            return servicoCentroCarregamento.ObterCentroCarregamentoPorVeiculo(veiculo) ?? throw new ServicoException("Centro de carregamento não encontrado.");
        }

        protected Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWorkContainer.UnitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        protected Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga ObterConfiguracaoGeralCarga()
        {
            if (_configuracaoGeralCarga == null)
                _configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWorkContainer.UnitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoGeralCarga;
        }

        protected Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga ObterConfiguracaoPreCarga()
        {
            if (_configuracaoPreCarga == null)
                _configuracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(_unitOfWorkContainer.UnitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoPreCarga;
        }

        protected Dominio.Entidades.Usuario ObterMotorista(int codigoMotorista)
        {
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWorkContainer.UnitOfWork);

            return repositorioMotorista.BuscarPorCodigo(codigoMotorista);
        }

        protected Dominio.Entidades.Embarcador.Logistica.AreaVeiculo ObterAreaVeiculo(int codigoAreaVeiculo)
        {
            Repositorio.Embarcador.Logistica.AreaVeiculo repositorioAreaVeiculo = new Repositorio.Embarcador.Logistica.AreaVeiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorioAreaVeiculo.BuscarPorCodigo(codigoAreaVeiculo);
        }

        protected Dominio.Entidades.Embarcador.Veiculos.Equipamento ObterEquipamento(int codigoEquipamento)
        {
            Repositorio.Embarcador.Veiculos.Equipamento repositorioEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWorkContainer.UnitOfWork);

            return repositorioEquipamento.BuscarPorCodigo(codigoEquipamento);
        }

        protected string ObterNumeroCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                return "";

            return new Carga.Carga(_unitOfWorkContainer.UnitOfWork).ObterNumeroCarga(carga, _unitOfWorkContainer.UnitOfWork);
        }

        protected Dominio.Entidades.Veiculo ObterVeiculoPorCodigo(int codigoVeiculo)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorioVeiculo.BuscarPorCodigo(codigoVeiculo) ?? throw new ServicoException("Veículo não encontrado.");
        }

        protected void ValidarConjuntoVeiculo(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo, bool naoValidarIntegracaoGR)
        {
            if (conjuntoVeiculo.Tracao != null)
                ValidarVeiculo(conjuntoVeiculo.Tracao, naoValidarIntegracaoGR);

            foreach (Dominio.Entidades.Veiculo reboque in conjuntoVeiculo.Reboques)
            {
                ValidarVeiculo(reboque, naoValidarIntegracaoGR);
            }
        }

        protected void ValidarConjuntoVeiculoExclusivoOutroCentroCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento)
        {
            if (conjuntoVeiculo.Tracao != null)
                ValidarVeiculoExclusivoOutroCentroCarregamento(conjuntoVeiculo.Tracao, centroCarregamento);

            foreach (Dominio.Entidades.Veiculo reboque in conjuntoVeiculo.Reboques)
            {
                ValidarVeiculoExclusivoOutroCentroCarregamento(reboque, centroCarregamento);
            }
        }

        protected void ValidarMotorista(Dominio.Entidades.Usuario motorista, bool naoValidarIntegracaoGR)
        {
            ValidarCadastroMotoristaAtivo(motorista);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();

            if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                return;

            ValidarRestricoesMotorista(motorista);

            if (!naoValidarIntegracaoGR)
                ValidarRetornoConsultaIntegracoesMotorista(motorista);
        }

        protected void ValidarVeiculo(Dominio.Entidades.Veiculo veiculo, bool naoValidarIntegracaoGR)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();

            if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                return;

            if (!naoValidarIntegracaoGR)
                ValidarRetornoConsultaIntegracoesVeiculo(veiculo);

            ValidarPunicaoVeiculo(veiculo);
            ValidarRestricoesVeiculo(veiculo);
        }

        protected void ValidarVeiculoExclusivoOutroCentroCarregamento(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento)
        {
            if (centroCarregamento == null)
                return;

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamentoExclusivoVeiculo = repositorioCentroCarregamento.BuscarPorVeiculo(centroCarregamento.Codigo, veiculo.Codigo);

            if (centroCarregamentoExclusivoVeiculo != null)
                throw new ServicoException($"O veículo com a placa {veiculo.Placa} pode entrar somente na fila do centro de carregamento {centroCarregamentoExclusivoVeiculo.Descricao}");
        }

        #endregion
    }
}
