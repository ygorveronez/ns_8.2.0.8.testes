using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteClienteIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao>
    {
        #region Construtores

        public TabelaFreteClienteIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao> Consultar(int codigoTabelaFreteCliente, SituacaoIntegracao? situacao)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao>()
                .Where(o => o.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente);

            if (situacao.HasValue)
                consultaIntegracao = consultaIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao> Consultar(int codigoTabelaFreteCliente, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIntegracao = Consultar(codigoTabelaFreteCliente, situacao);

            return ObterLista(consultaIntegracao, parametrosConsulta);
        }

        public int ContarConsulta(int codigoTabelaFreteCliente, SituacaoIntegracao? situacao)
        {
            var consultaIntegracao = Consultar(codigoTabelaFreteCliente, situacao);

            return consultaIntegracao.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao> BuscarIntegracoesComProblema()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao>()
                .Where(o => o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao);
            return consultaIntegracao.ToList();
        }
        
        public int ContarBuscarArquivosPorIntegracao(int codigo)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao>()
                .Where(o => o.Codigo == codigo);

            var consultaIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo>()
                .Where(o => consultaIntegracao.Any(p => p.ArquivosTransacao.Contains(o)))
                .OrderByDescending(o => o.Data);

            return consultaIntegracaoArquivo.Count();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo BuscarIntegracaoPorCodigo(int codigo)
        {
            var consultaIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo>()
                .Where(o => o.Codigo == codigo);

            return consultaIntegracaoArquivo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao>()
                .Where(obj =>
                    (
                        obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                        (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                    ) &&
                    obj.TipoIntegracao.Ativo
                );

            return consultaIntegracoes.OrderBy(o => o.Codigo).Take(8).ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao BuscarAguardandoRetornoPorTipoIntegracao(int codigoTabelaFreteCliente, int codigoTipoIntegracao)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao>()
                .Where(obj =>
                    obj.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente &&
                    obj.TipoIntegracao.Codigo == codigoTipoIntegracao &&
                    obj.SituacaoIntegracao == SituacaoIntegracao.AgRetorno
                );

            return consultaIntegracoes.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao BuscarPendentePorTipoIntegracao(int codigoTabelaFreteCliente, int codigoTipoIntegracao)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao>()
                .Where(obj =>
                    obj.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente &&
                    obj.TipoIntegracao.Codigo == codigoTipoIntegracao &&
                    (obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao || obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                );

            return consultaIntegracoes.FirstOrDefault();
        }

        public int ContarPorTabelaFreteClienteESituacao(int codigoTabelaFreteCliente, SituacaoIntegracao situacao)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao>()
                .Where(obj => obj.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente && obj.SituacaoIntegracao == situacao);

            return consultaIntegracoes.Count();
        }

        public void InserirOuAtualizarIntegracoesPorAjusteTabelaFrete(int codigoAjusteTabelaFrete, int codigoTipoIntegracao)
        {
            UnitOfWork.Sessao
                .CreateSQLQuery(
                    $@"insert into T_TABELA_FRETE_CLIENTE_INTEGRACAO (TFC_CODIGO, TPI_CODIGO, INT_DATA_INTEGRACAO, INT_PROBLEMA_INTEGRACAO, INT_SITUACAO_INTEGRACAO, INT_NUMERO_TENTATIVAS, INT_SISTEMA_INTEGRACAO)
                       select TabelaFreteClienteAjuste.TFC_TABELA_ORIGINARIA, {codigoTipoIntegracao}, getdate(), '', {(int)SituacaoIntegracao.AgIntegracao}, 0, {(int)SistemaIntegracao.NaoInformado}
                         from T_TABELA_FRETE_CLIENTE TabelaFreteClienteAjuste
                        where TabelaFreteClienteAjuste.TFA_CODIGO = {codigoAjusteTabelaFrete}
                          and not exists (
                                  select 1
                                    from T_TABELA_FRETE_CLIENTE_INTEGRACAO TabelaFreteClienteIntegracao
                                   where TabelaFreteClienteIntegracao.TFC_CODIGO = TabelaFreteClienteAjuste.TFC_TABELA_ORIGINARIA
                                     and TabelaFreteClienteIntegracao.TPI_CODIGO = {codigoTipoIntegracao}
                                     and TabelaFreteClienteIntegracao.INT_SITUACAO_INTEGRACAO in ({(int)SituacaoIntegracao.AgIntegracao}, {(int)SituacaoIntegracao.ProblemaIntegracao})
                              );"
                )
                .SetTimeout(120)
                .ExecuteUpdate();

            UnitOfWork.Sessao
                .CreateSQLQuery(
                    $@"update T_TABELA_FRETE_CLIENTE_INTEGRACAO
                          set INT_DATA_INTEGRACAO = getdate(), INT_NUMERO_TENTATIVAS = 0, INT_PROBLEMA_INTEGRACAO = '', INT_SITUACAO_INTEGRACAO = {(int)SituacaoIntegracao.AgIntegracao}
                        where TPI_CODIGO = {codigoTipoIntegracao}
                          and INT_SITUACAO_INTEGRACAO in ({(int)SituacaoIntegracao.AgIntegracao}, {(int)SituacaoIntegracao.ProblemaIntegracao})
                          and TFC_CODIGO in (
                                  select TabelaFreteClienteAjuste.TFC_TABELA_ORIGINARIA
                                    from T_TABELA_FRETE_CLIENTE TabelaFreteClienteAjuste
                                   where TabelaFreteClienteAjuste.TFA_CODIGO = {codigoAjusteTabelaFrete}
                              );"
                )
                .SetTimeout(120)
                .ExecuteUpdate();
        }

        public void InserirOuAtualizarIntegracoesPorTabelaFreteAlteracao(int codigoTabelaFreteAlteracao, int codigoTipoIntegracao)
        {
            UnitOfWork.Sessao
                .CreateSQLQuery(
                    $@"insert into T_TABELA_FRETE_CLIENTE_INTEGRACAO (TFC_CODIGO, TPI_CODIGO, INT_DATA_INTEGRACAO, INT_PROBLEMA_INTEGRACAO, INT_SITUACAO_INTEGRACAO, INT_NUMERO_TENTATIVAS, INT_SISTEMA_INTEGRACAO)
                       select TabelaFreteClienteAlteracao.TFC_CODIGO, {codigoTipoIntegracao}, getdate(), '', {(int)SituacaoIntegracao.AgIntegracao}, 0, {(int)SistemaIntegracao.NaoInformado}
                         from T_TABELA_FRETE_CLIENTE_ALTERACAO TabelaFreteClienteAlteracao
                        where TabelaFreteClienteAlteracao.TFA_CODIGO = {codigoTabelaFreteAlteracao}
                          and not exists (
                                  select 1
                                    from T_TABELA_FRETE_CLIENTE_INTEGRACAO TabelaFreteClienteIntegracao
                                   where TabelaFreteClienteIntegracao.TFC_CODIGO = TabelaFreteClienteAlteracao.TFC_CODIGO
                                     and TabelaFreteClienteIntegracao.TPI_CODIGO = {codigoTipoIntegracao}
                                     and TabelaFreteClienteIntegracao.INT_SITUACAO_INTEGRACAO in ({(int)SituacaoIntegracao.AgIntegracao}, {(int)SituacaoIntegracao.ProblemaIntegracao})
                              );"
                )
                .SetTimeout(120)
                .ExecuteUpdate();

            UnitOfWork.Sessao
                .CreateSQLQuery(
                    $@"update T_TABELA_FRETE_CLIENTE_INTEGRACAO
                          set INT_DATA_INTEGRACAO = getdate(), INT_NUMERO_TENTATIVAS = 0, INT_PROBLEMA_INTEGRACAO = '', INT_SITUACAO_INTEGRACAO = {(int)SituacaoIntegracao.AgIntegracao}
                        where TPI_CODIGO = {codigoTipoIntegracao}
                          and INT_SITUACAO_INTEGRACAO in ({(int)SituacaoIntegracao.AgIntegracao}, {(int)SituacaoIntegracao.ProblemaIntegracao})
                          and TFC_CODIGO in (
                                  select TabelaFreteClienteAlteracao.TFC_CODIGO
                                    from T_TABELA_FRETE_CLIENTE_ALTERACAO TabelaFreteClienteAlteracao
                                   where TabelaFreteClienteAlteracao.TFA_CODIGO = {codigoTabelaFreteAlteracao}
                              );"
                )
                .SetTimeout(120)
                .ExecuteUpdate();
        }

        #endregion
    }
}