using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Veiculo
{
    public sealed class Inutilizados
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        public Inutilizados(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _auditado = auditado;
        }

        public void InativarVeiculosSemUtilizacao(int quantidadesDiasParaDesativarVeiculos)
        {
            Repositorio.Veiculo repVeiculos = new Repositorio.Veiculo(_unitOfWork);

            DateTime dataAtual = DateTime.Today;
            DateTime dataLimiteSemCarga = dataAtual.AddDays(-quantidadesDiasParaDesativarVeiculos);

            string msgAuditoria = $"Inativado por não possuir nenhuma carga desde {dataLimiteSemCarga.ToDateString()}";
            List<Dominio.Entidades.Veiculo> veiculosSemUtilizacao = repVeiculos.BuscarVeiculosSemCargaDesdeDataLimite(dataLimiteSemCarga);

            for (int i = 0, s = veiculosSemUtilizacao.Count; i < s; i++)
            {
                Dominio.Entidades.Veiculo veiculo = veiculosSemUtilizacao[i];

                bool situacaoAnterior = veiculo.Ativo;
                veiculo.Ativo = false;

                Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculo, situacaoAnterior, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MetodosAlteracaoVeiculo.InativarVeiculosSemUtilizacao_Inutilizados, null, _unitOfWork);
                Servicos.Embarcador.Veiculo.Veiculo.AlterarSituacaoVeiculo(veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Indisponivel, _unitOfWork, _auditado, msgAuditoria);

                repVeiculos.Atualizar(veiculo);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, veiculo, msgAuditoria, _unitOfWork);
            }
        }
    }
}
