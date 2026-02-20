using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class ServicoInspecaoFederal : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal>
    {
        public ServicoInspecaoFederal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSIF filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSIF filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        public bool VerificaCodigoSIFCodigoIntegracao(Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal servicoInspecaoFederal)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal>();
            consulta = from obj in consulta select obj;

            return consulta.Where(o => (o.CodigoSIF == servicoInspecaoFederal.CodigoSIF || o.CodigoIntegracao == servicoInspecaoFederal.CodigoIntegracao) && o.Codigo != servicoInspecaoFederal.Codigo).Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaSIF filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ServicoInspecaoFederal>();
            consulta = from obj in consulta select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoSIF))
                consulta = consulta.Where(o => o.CodigoSIF == filtrosPesquisa.CodigoSIF);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                bool situacaoAtivo = filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                consulta = consulta.Where(obj => obj.Ativo == situacaoAtivo);
            }

            return consulta;
        }

        #endregion
    }
}
