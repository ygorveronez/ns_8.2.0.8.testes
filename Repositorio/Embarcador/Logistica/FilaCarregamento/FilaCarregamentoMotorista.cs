using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class FilaCarregamentoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista>
    {
        #region Construtores

        public FilaCarregamentoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista> ConsultarSemFilaCarregamentoVeiculo(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoMotorista filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                   .Where(o => o.ConjuntoMotorista.FilaCarregamentoMotorista != null);

            var consultaFilaCarregamentoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista>()
                .Where(o => !consultaFilaCarregamentoVeiculo.Where(v => v.ConjuntoMotorista.FilaCarregamentoMotorista.Codigo == o.Codigo).Any());
            
            if (filtrosPesquisa.RetornarMotoristaComReboqueAtrelado)
                consultaFilaCarregamentoMotorista = consultaFilaCarregamentoMotorista.Where(o => o.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel || o.Situacao == SituacaoFilaCarregamentoMotorista.ReboqueAtrelado);
            else
                consultaFilaCarregamentoMotorista = consultaFilaCarregamentoMotorista.Where(o => o.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel);

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaFilaCarregamentoMotorista = consultaFilaCarregamentoMotorista.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.CodigoGrupoModeloVeicularCarga > 0)
                consultaFilaCarregamentoMotorista = consultaFilaCarregamentoMotorista.Where(o => o.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular.Codigo == filtrosPesquisa.CodigoGrupoModeloVeicularCarga);

            if (filtrosPesquisa.CodigosModeloVeicularCarga?.Count > 0)
                consultaFilaCarregamentoMotorista = consultaFilaCarregamentoMotorista.Where(o => filtrosPesquisa.CodigosModeloVeicularCarga.Contains(o.ConjuntoVeiculo.ModeloVeicularCarga.Codigo));

            return consultaFilaCarregamentoMotorista;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista BuscarAtivaPorMotorista(int codigo)
        {
            List<SituacaoFilaCarregamentoMotorista> situacoesAtiva = SituacaoFilaCarregamentoMotoristaHelper.ObterSituacoesAtiva();

            var filaCarregamentoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista>()
                .Where(o => (o.Motorista.Codigo == codigo) && (situacoesAtiva.Contains(o.Situacao)))
                .FirstOrDefault();

            return filaCarregamentoMotorista;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista BuscarPorCodigo(int codigo)
        {
            var filaCarregamentoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return filaCarregamentoMotorista;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista> ConsultarSemFilaCarregamentoVeiculo(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoMotorista filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFilaCarregamentoMotorista = ConsultarSemFilaCarregamentoVeiculo(filtrosPesquisa);

            return ObterLista(consultaFilaCarregamentoMotorista, parametrosConsulta);
        }

        public int ContarConsultaSemFilaCarregamentoVeiculo(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoMotorista filtrosPesquisa)
        {
            var consultaFilaCarregamentoMotorista = ConsultarSemFilaCarregamentoVeiculo(filtrosPesquisa);

            return consultaFilaCarregamentoMotorista.Count();
        }

        #endregion
    }
}
