using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas
{
    public class AprovacaoAlcadaSolicitacaoGas : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas,
        Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.RegraAprovacaoSolicitacaoGas,
        Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas
    >
    {
        #region Construtores

        public AprovacaoAlcadaSolicitacaoGas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAprovacaoSolicitacaoAbastecimentoGas filtrosPesquisa)
        {
            var consultaSolicitacaoGas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();
            var consultaAcadaSolicitacaoGas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas>()
                .Where(o => !o.Bloqueada);
            
            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAcadaSolicitacaoGas = consultaAcadaSolicitacaoGas.Where(obj => obj.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.CodigoBase > 0)
                consultaAcadaSolicitacaoGas = consultaAcadaSolicitacaoGas.Where(obj => obj.OrigemAprovacao.ClienteBase.CPF_CNPJ == filtrosPesquisa.CodigoBase);
            
            if (filtrosPesquisa.Situacao.HasValue)
                consultaAcadaSolicitacaoGas = consultaAcadaSolicitacaoGas.Where(obj => obj.OrigemAprovacao.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.DataSolicitacaoInicial.HasValue)
                consultaAcadaSolicitacaoGas = consultaAcadaSolicitacaoGas.Where(obj => obj.OrigemAprovacao.DataMedicao.Date >= filtrosPesquisa.DataSolicitacaoInicial.Value.Date);
            
            if (filtrosPesquisa.DataSolicitacaoFinal.HasValue)
                consultaAcadaSolicitacaoGas = consultaAcadaSolicitacaoGas.Where(obj => obj.OrigemAprovacao.DataMedicao.Date <= filtrosPesquisa.DataSolicitacaoFinal.Value.Date);

            return consultaSolicitacaoGas.Where(o => consultaAcadaSolicitacaoGas.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas> BuscarPorOrigem(int codigoOrigem)
        {
            var consultaAcadaSolicitacaoGas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas>()
                .Where(obj => obj.OrigemAprovacao.Codigo == codigoOrigem);
            
            return consultaAcadaSolicitacaoGas.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAprovacaoSolicitacaoAbastecimentoGas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSolicitacaoGas = Consultar(filtrosPesquisa);

            return ObterLista(consultaSolicitacaoGas, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAprovacaoSolicitacaoAbastecimentoGas filtrosPesquisa)
        {
            var consultaSolicitacaoGas = Consultar(filtrosPesquisa);

            return consultaSolicitacaoGas.Count();
        }

        #endregion
    }
}
