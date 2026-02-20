using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3600000)]

    public class FaturamentoAutomaticoCTe : LongRunningProcessBase<FaturamentoAutomaticoCTe>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarCtesPendenteFaturamento(unitOfWork, _codigoEmpresa, _stringConexao, _tipoServicoMultisoftware, _webServiceConsultaCTe, _auditado);
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;
        }

        #endregion Métodos Protegidos

        #region Métodos Privados

        private void VerificarCtesPendenteFaturamento(Repositorio.UnitOfWork unitOfWork, int idEmpresa, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            //Rever essa consulta, que não tem limite de quantidade de ctes pra retornar, deixa a base lenta se tiver muitos
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> lstCteFaturar = repConhecimentoDeTransporteEletronico.BustarCTesParaFaturamento();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteFaturar in lstCteFaturar)
            {
                string msgErro = string.Empty;

                if (!Servicos.Embarcador.Fatura.FaturamentoLote.InserirFaturaPorCTe(unitOfWork, tipoServicoMultisoftware, stringConexao, cteFaturar, null, null, null, idEmpresa, null, ref msgErro))
                    throw new Exception(msgErro);

            }
        }

        #endregion Métodos Privados
    }
}