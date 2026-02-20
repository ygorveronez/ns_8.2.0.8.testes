using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 30000)]

    public class RH : LongRunningProcessBase<RH>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarGeracaoComissao(_stringConexao, _stringConexaoAdmin, _clienteMultisoftware.Codigo, _tipoServicoMultisoftware, unitOfWork);
            VerificarIntegracaoSituacaoColaborador(_stringConexao, _stringConexaoAdmin, _clienteMultisoftware.Codigo, _tipoServicoMultisoftware, unitOfWork);
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;
        }

        private void VerificarIntegracaoSituacaoColaborador(string stringConexao, string adminStringConexao, int clienteCodigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao repColaboradorSituacaoLancamentoIntegracao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao> lancamentosPendentes = repColaboradorSituacaoLancamentoIntegracao.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
            if (lancamentosPendentes.Count > 0)
            {
                try
                {
                    foreach (Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao lancamentoPendente in lancamentosPendentes)
                    {
                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorMotorista(lancamentoPendente.ColaboradorLancamento.Colaborador.Codigo);
                        new Servicos.Embarcador.Integracao.A52.IntegracaoA52(unidadeDeTrabalho).IntegrarSituacaoColaborador(lancamentoPendente, veiculo);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private void VerificarGeracaoComissao(string stringConexao, string adminStringConexao, int clienteCodigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionario> comissoesPendentes = repComissaoFuncionario.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.EmGeracao);
            if (comissoesPendentes.Count > 0)
            {
                AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);
                try
                {
                    AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(adminUnitOfWork);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(clienteCodigo);
                    Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unidadeDeTrabalho);

                    foreach (Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario in comissoesPendentes)
                    {
                        serComissaoFuncionario.GerarComissaoMotoristas(comissaoFuncionario.Motorista != null ? comissaoFuncionario.Motorista.Codigo : 0, comissaoFuncionario.CargoMotorista != null ? comissaoFuncionario.CargoMotorista.Codigo : 0, comissaoFuncionario.Codigo, cliente, tipoServicoMultisoftware, adminStringConexao);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    adminUnitOfWork.Dispose();
                }
            }
        }
    }
}
