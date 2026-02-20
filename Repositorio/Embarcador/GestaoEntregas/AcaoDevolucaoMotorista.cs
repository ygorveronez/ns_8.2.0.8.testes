using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.GestaoEntregas
{
    public sealed class AcaoDevolucaoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista>
    {
        #region Construtores

        public AcaoDevolucaoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista> Consultar(string descricao, SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaAcaoDevolucaoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaAcaoDevolucaoMotorista = consultaAcaoDevolucaoMotorista.Where(o => o.Descricao.Contains(descricao));

            if (situacaoAtivo == SituacaoAtivoPesquisa.Ativo)
                consultaAcaoDevolucaoMotorista = consultaAcaoDevolucaoMotorista.Where(o => o.Ativo);
            else if (situacaoAtivo == SituacaoAtivoPesquisa.Inativo)
                consultaAcaoDevolucaoMotorista = consultaAcaoDevolucaoMotorista.Where(o => !o.Ativo);

            return consultaAcaoDevolucaoMotorista;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoEntregas.AcaoDevolucaoMotorista> Consultar(string descricao, SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAcaoDevolucaoMotorista = Consultar(descricao, situacaoAtivo);

            return ObterLista(consultaAcaoDevolucaoMotorista, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaAcaoDevolucaoMotorista = Consultar(descricao, situacaoAtivo);

            return consultaAcaoDevolucaoMotorista.Count();
        }

        #endregion
    }
}
