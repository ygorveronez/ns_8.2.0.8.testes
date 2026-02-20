using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public sealed class RegraParcelamentoOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia>
    {
        #region Construtores

        public RegraParcelamentoOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia> Consultar(string descricao, SituacaoAtivoPesquisa status)
        {
            var consultaRegraParcelamentoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaRegraParcelamentoOcorrencia = consultaRegraParcelamentoOcorrencia.Where(o => o.Descricao.Contains(descricao));

            if (status == SituacaoAtivoPesquisa.Ativo)
                consultaRegraParcelamentoOcorrencia = consultaRegraParcelamentoOcorrencia.Where(o => o.Ativo == true);
            else if (status == SituacaoAtivoPesquisa.Inativo)
                consultaRegraParcelamentoOcorrencia = consultaRegraParcelamentoOcorrencia.Where(o => o.Ativo == false);

            return consultaRegraParcelamentoOcorrencia;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia BuscarPrimeiraAtiva()
        {
            var consultaRegraParcelamentoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia>()
                .Where(o => o.Ativo == true);

            return consultaRegraParcelamentoOcorrencia.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegraParcelamentoOcorrencia> Consultar(string descricao, SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRegraParcelamentoOcorrencia = Consultar(descricao, status);

            return ObterLista(consultaRegraParcelamentoOcorrencia, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa status)
        {
            var consultaRegraParcelamentoOcorrencia = Consultar(descricao, status);

            return consultaRegraParcelamentoOcorrencia.Count();
        }

        #endregion
    }
}
