using System.Collections.Generic;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ExecucaoComandos
    {
        #region Propriedades

        private UnitOfWork _unitOfWork;

        #endregion

        #region Métodos Públicos

        public ExecucaoComandos(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AlterarStatusCTes(List<int> codigosCTes, string status)
        {
            _unitOfWork.Sessao.CreateSQLQuery(@"UPDATE T_CTE SET CON_STATUS = :status WHERE CON_CODIGO in (:codigosCTes)")
                .SetParameterList("codigosCTes", codigosCTes)
                .SetParameter("status", status)
                .ExecuteUpdate();
        }

        public void AutorizarFaturamentosCTe(List<int> codigosCTes)
        {
            _unitOfWork.Sessao.CreateSQLQuery(@"UPDATE T_DOCUMENTO_FATURAMENTO SET DFA_SITUACAO = 1 WHERE CON_CODIGO IN (:codigosCTes)")
                .SetParameterList("codigosCTes", codigosCTes)
                .ExecuteUpdate();
        }

        public void AlterarSituacaoCargas(List<int> codigosCargas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacao)
        {
            _unitOfWork.Sessao.CreateSQLQuery(@"UPDATE T_CARGA SET CAR_SITUACAO = :situacao WHERE CAR_CODIGO in (:codigosCargas)")
                .SetParameterList("codigosCargas", codigosCargas)
                .SetParameter("situacao", situacao)
                .ExecuteUpdate();
        }

        public void ExecutarScript(string script)
        {
            _unitOfWork.Sessao.CreateSQLQuery(script)                
                .ExecuteUpdate();
        }

        #endregion
    }
}
