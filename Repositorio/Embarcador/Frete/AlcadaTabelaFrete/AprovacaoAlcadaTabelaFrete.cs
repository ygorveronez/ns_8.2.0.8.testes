using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frete.AlcadasTabelaFrete
{
    public sealed class AprovacaoAlcadaTabelaFrete : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete,
        Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete,
        Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao
    >
    {
        #region Construtores

        public AprovacaoAlcadaTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteAprovacao filtrosPesquisa)
        {
            var consultaTabelaFreteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>();
            var consultaAlcadaTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoTabelaFrete > 0)
                consultaTabelaFreteAlteracao = consultaTabelaFreteAlteracao.Where(o => o.TabelaFrete.Codigo == filtrosPesquisa.CodigoTabelaFrete);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaTabelaFreteAlteracao = consultaTabelaFreteAlteracao.Where(o => o.TabelaFrete.TiposOperacao.Any(to => to.Codigo == filtrosPesquisa.CodigoTipoOperacao));

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaAlcadaTabelaFrete = consultaAlcadaTabelaFrete.Where(o => o.DataCriacao >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaAlcadaTabelaFrete = consultaAlcadaTabelaFrete.Where(o => o.DataCriacao <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaTabelaFrete = consultaAlcadaTabelaFrete.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.TipoAprovadorRegra.HasValue)
                consultaAlcadaTabelaFrete = consultaAlcadaTabelaFrete.Where(o => o.TipoAprovadorRegra == filtrosPesquisa.TipoAprovadorRegra.Value);

            if (filtrosPesquisa.SituacaoAlteracao.HasValue)
            {
                consultaTabelaFreteAlteracao = consultaTabelaFreteAlteracao.Where(o => o.SituacaoAlteracao == filtrosPesquisa.SituacaoAlteracao.Value);

                if (filtrosPesquisa.SituacaoAlteracao.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoTabelaFrete.AguardandoAprovacao)
                    consultaAlcadaTabelaFrete = consultaAlcadaTabelaFrete.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);
            }

            consultaTabelaFreteAlteracao = consultaTabelaFreteAlteracao.Where(o => consultaAlcadaTabelaFrete.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaTabelaFreteAlteracao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTabelaFreteAlteracao = Consultar(filtrosPesquisa);

            return ObterLista(consultaTabelaFreteAlteracao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteAprovacao filtrosPesquisa)
        {
            var consultaTabelaFreteAlteracao = Consultar(filtrosPesquisa);

            return consultaTabelaFreteAlteracao.Count();
        }

        public void AdicionarUsuarioComoNovoAprovadorEmPendentes(int codigoUsuario, int codigoEmpresa)
        {
            this.SessionNHiBernate.CreateSQLQuery($@"insert into T_AUTORIZACAO_ALCADA_TABELA_FRETE_ALTERACAO (TFA_CODIGO, AAL_BLOQUEADA, FUN_CODIGO, RAT_CODIGO, AAL_SITUACAO, AAL_DATA_CRIACAO, AAL_NUMERO_APROVADORES, AAL_TIPO_APROVADOR_REGRA)

                                                                output inserted.TFA_CODIGO

                                                                select distinct TabelaAlteracao.TFA_CODIGO OrigemAprovacao,
                                                                       Aprovacao.AAL_BLOQUEADA Bloqueada,
                                                                       {codigoUsuario} Usuario,
                                                                       Aprovacao.RAT_CODIGO RegraAutorizacao,
                                                                       {(int)SituacaoAlcadaRegra.Pendente} Situacao,
                                                                       getdate() DataCriacao,
                                                                       Aprovacao.AAL_NUMERO_APROVADORES NumeroAprovadores,
                                                                       Aprovacao.AAL_TIPO_APROVADOR_REGRA TipoAprovadorRegra

                                                                from T_TABELA_FRETE_ALTERACAO TabelaAlteracao
                                                                  join T_AUTORIZACAO_ALCADA_TABELA_FRETE_ALTERACAO Aprovacao on Aprovacao.TFA_CODIGO = TabelaAlteracao.TFA_CODIGO

                                                                where TabelaAlteracao.TFA_SITUACAO = {(int)SituacaoAlteracaoTabelaFrete.AguardandoAprovacao}
                                                                   and Aprovacao.AAL_TIPO_APROVADOR_REGRA = {(int)TipoAprovadorRegra.Transportador}
                                                                   and exists (
                                                                           select top(1) ValorAlteracao.TCA_CODIGO
                                                                             from T_TABELA_FRETE_CLIENTE_ALTERACAO ValorAlteracao
                                                                             join T_TABELA_FRETE_CLIENTE Valor on Valor.TFC_CODIGO = ValorAlteracao.TFC_CODIGO
                                                                            where ValorAlteracao.TFA_CODIGO = TabelaAlteracao.TFA_CODIGO
                                                                              and Valor.EMP_CODIGO = {codigoEmpresa}
                                                                                );
                                                                    ").ExecuteUpdate();
        }

        #endregion
    }
}
