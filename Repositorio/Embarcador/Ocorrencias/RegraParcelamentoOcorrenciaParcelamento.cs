using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public sealed class RegraParcelamentoOcorrenciaParcelamento : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento>
    {
        #region Construtores

        public RegraParcelamentoOcorrenciaParcelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento> BuscarPorRegraParcelamentoOcorrencia(int codigoRegraParcelamentoOcorrencia)
        {
            var consultaRegraParcelamentoOcorrenciaParcelamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrenciaParcelamento>()
                .Where(o => o.RegraParcelamentoOcorrencia.Codigo == codigoRegraParcelamentoOcorrencia);

            return consultaRegraParcelamentoOcorrenciaParcelamento.ToList();
        }

        public void DeletarPorRegraParcelamentoOcorrencia(int codigoRegraParcelamentoOcorrencia)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete RegraParcelamentoOcorrenciaParcelamento parcelamento where parcelamento.RegraParcelamentoOcorrencia.Codigo = :codigo ")
                    .SetInt32("codigo", codigoRegraParcelamentoOcorrencia)
                    .ExecuteUpdate();

                return;
            }

            try
            {
                UnitOfWork.Start();

                UnitOfWork.Sessao
                    .CreateQuery($"delete RegraParcelamentoOcorrenciaParcelamento parcelamento where parcelamento.RegraParcelamentoOcorrencia.Codigo = :codigo ")
                    .SetInt32("codigo", codigoRegraParcelamentoOcorrencia)
                    .ExecuteUpdate();

                UnitOfWork.CommitChanges();
            }
            catch
            {
                UnitOfWork.Rollback();
                throw;
            }
        }

        #endregion
    }
}
