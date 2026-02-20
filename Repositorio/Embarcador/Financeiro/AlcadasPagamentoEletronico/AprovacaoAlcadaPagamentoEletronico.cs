using System;
using System.Collections.Generic;
using System.Linq;


namespace Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico
{
    public sealed class AprovacaoAlcadaPagamentoEletronico : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico,
        Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico,
        Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico
    >
    {
        #region Construtores

        public AprovacaoAlcadaPagamentoEletronico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoEletronicoAprovacao filtrosPesquisa)
        {
            var consultaOrdemServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico>();
            var consultaAlcadaOrdemServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.Numero > 0)
                consultaOrdemServico = consultaOrdemServico.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                consultaOrdemServico = consultaOrdemServico.Where(o => o.DataPagamento.Value.Date >= filtrosPesquisa.DataInicio.Date);

            if (filtrosPesquisa.DataLimite != DateTime.MinValue)
                consultaOrdemServico = consultaOrdemServico.Where(o => o.DataPagamento.Value.Date <= filtrosPesquisa.DataLimite.Date);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaOrdemServico = consultaOrdemServico.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoBoletoConfiguracao > 0)
                consultaOrdemServico = consultaOrdemServico.Where(o => o.BoletoConfiguracao.Codigo == filtrosPesquisa.CodigoBoletoConfiguracao);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaOrdemServico = consultaAlcadaOrdemServico.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao)
            {
                consultaAlcadaOrdemServico = consultaAlcadaOrdemServico.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);
                consultaOrdemServico = consultaOrdemServico.Where(o => o.SituacaoAutorizacaoPagamentoEletronico == 
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao);
            } 
            else if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPagamentoEletronico.Todos)
            {
                consultaOrdemServico = consultaOrdemServico.Where(o => o.SituacaoAutorizacaoPagamentoEletronico == filtrosPesquisa.Situacao);
            }

            return consultaOrdemServico.Where(o => consultaAlcadaOrdemServico.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoEletronicoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOrdemServico = Consultar(filtrosPesquisa);

            return ObterLista(consultaOrdemServico, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoEletronicoAprovacao filtrosPesquisa)
        {
            var consultaOrdemServico = Consultar(filtrosPesquisa);

            return consultaOrdemServico.Count();
        }

        #endregion
    }
}
