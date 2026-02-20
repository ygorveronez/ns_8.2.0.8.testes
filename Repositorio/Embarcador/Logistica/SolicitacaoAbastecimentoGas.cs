using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class SolicitacaoAbastecimentoGas : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>
    {
        public SolicitacaoAbastecimentoGas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas BuscarPorDataMedicaoFilialBaseProduto(DateTime dataMedicao, double codigoSupridorBase, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>()
                .Where(obj => obj.DataMedicao == dataMedicao)
                .Where(obj => obj.ClienteBase.CPF_CNPJ == codigoSupridorBase)
                .Where(obj => obj.Produto.Codigo == codigoProduto);
            
            return query
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas BuscarSolicitacaoComDisponibilidadeTransferenciaPorDataDeMedicao(DateTime dataMedicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>()
                .Where(obj => (obj.DisponibilidadeTransferenciaProximoDia + obj.AdicionalDisponibilidadeTransferenciaProximoDia) > 0 && obj.DataMedicao.Date == dataMedicao.Date);

            return query
                .OrderByDescending(x => x.Codigo)
                .FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas> BuscarSolicitacoesComDisponibilidadeTransferenciaPorDataDeMedicaoECliente(DateTime dataMedicao, double cpfCnpjCliente = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>()
                .Where(obj => (obj.DisponibilidadeTransferenciaProximoDia + obj.AdicionalDisponibilidadeTransferenciaProximoDia) > 0 && obj.DataMedicao.Date == dataMedicao.Date);

            if (cpfCnpjCliente > 0)
                query = query.Where(obj => obj.ClienteBase.CPF_CNPJ == cpfCnpjCliente);

            return query
                .ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = _Consultar(filtrosPesquisa, parametrosConsulta);

            query = query.Fetch(obj => obj.ClienteBase);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = _Consultar(filtrosPesquisa, parametrosConsulta);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>()
                .Where(obj => obj.Codigo == codigo);

            return query
                .Fetch(obj => obj.ClienteBase)
                .Fetch(obj => obj.Justificativa)
                .FirstOrDefault();
        }

        #endregion

        #region Relatório

        #region Métodos Públicos

        public int ContarConsultaRelatorioSolicitacaoGas(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioConsolidacaoGas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            var consulta = new Consulta.ConsultaSolicitacaoAbastecimentoGas().ObterSqlContarPesquisa(filtrosPesquisa, propriedadesAgrupamento).CriarSQLQuery(this.SessionNHiBernate);
            
            return consulta.SetTimeout(600).UniqueResult<int>();
        }
        
        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.ConsolidacaoGas> ConsultaRelatorioSolicitacaoGas(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioConsolidacaoGas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new Consulta.ConsultaSolicitacaoAbastecimentoGas().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedadesAgrupamento).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.ConsolidacaoGas)));
            
            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ConsolidacaoGas>();
        }

        #endregion

        #region Métodos Privados
        
        #endregion

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas> _Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();
            
            if (filtrosPesquisa.DataSolicitacao.HasValue)
                query = query.Where(obj => obj.DataMedicao.Date == filtrosPesquisa.DataSolicitacao.Value.Date);

            if (filtrosPesquisa.DataSolicitacaoInicial.HasValue)
                query = query.Where(obj => obj.DataMedicao.Date >= filtrosPesquisa.DataSolicitacaoInicial.Value.Date);

            if (filtrosPesquisa.DataSolicitacaoFinal.HasValue)
                query = query.Where(obj => obj.DataMedicao.Date <= filtrosPesquisa.DataSolicitacaoFinal.Value.Date);

            if (filtrosPesquisa.DataCriacaoInicial.HasValue)
                query = query.Where(obj => obj.DataCriacao.Date >= filtrosPesquisa.DataCriacaoInicial.Value.Date);

            if (filtrosPesquisa.DataCriacaoFinal.HasValue)
                query = query.Where(obj => obj.DataCriacao.Date <= filtrosPesquisa.DataCriacaoFinal.Value.Date);

            if (filtrosPesquisa.CodigoUsuario > 0)
                query = query.Where(obj => obj.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.CodigosBasesSupridora?.Count > 0)
            {
                List<double> codigosSupridoresPermitidos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas>()
                    .Where(obj => filtrosPesquisa.CodigosBasesSupridora.Contains(obj.SuprimentoDeGas.SupridorPadrao.CPF_CNPJ))
                    .Select(obj => obj.Cliente.CPF_CNPJ)
                    .ToList();
                
                query = query.Where(obj => codigosSupridoresPermitidos.Contains(obj.ClienteBase.CPF_CNPJ));
            }

            if (filtrosPesquisa.CodigosSupridoresPermitidos?.Count > 0)
                query = query.Where(obj => filtrosPesquisa.CodigosSupridoresPermitidos.Contains(obj.ClienteBase.CPF_CNPJ));
            
            if (filtrosPesquisa.PossuiVolumeRodoviario)
                query = query.Where(obj => (obj.VolumeRodoviarioCarregamentoProximoDia + obj.AdicionalVolumeRodoviarioCarregamentoProximoDia) > 0);

            if (filtrosPesquisa.PossuiDisponibilidadeDeTransferencia)
                query = query.Where(obj => (obj.DisponibilidadeTransferenciaProximoDia + obj.AdicionalDisponibilidadeTransferenciaProximoDia) > 0);

            if (filtrosPesquisa.CodigosBasesSatelite?.Count > 0)
                query = query.Where(obj => filtrosPesquisa.CodigosBasesSatelite.Contains(obj.ClienteBase.CPF_CNPJ));
            
            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(obj => obj.Situacao == filtrosPesquisa.Situacao.Value);
            
            if (filtrosPesquisa.AgruparPorDia)
            {
                var queryDois = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();

                if (filtrosPesquisa.PossuiVolumeRodoviario)
                    queryDois = queryDois.Where(obj => (obj.VolumeRodoviarioCarregamentoProximoDia + obj.AdicionalVolumeRodoviarioCarregamentoProximoDia) > 0);

                if (filtrosPesquisa.PossuiDisponibilidadeDeTransferencia)
                    queryDois = queryDois.Where(obj => (obj.DisponibilidadeTransferenciaProximoDia + obj.AdicionalDisponibilidadeTransferenciaProximoDia) > 0);

                if (filtrosPesquisa.DataSolicitacao.HasValue)
                    queryDois = queryDois.Where(obj => obj.DataMedicao.Date == filtrosPesquisa.DataSolicitacao.Value.Date);

                if (filtrosPesquisa.DataSolicitacaoInicial.HasValue)
                    queryDois = queryDois.Where(obj => obj.DataMedicao.Date >= filtrosPesquisa.DataSolicitacaoInicial.Value.Date);

                if (filtrosPesquisa.DataSolicitacaoFinal.HasValue)
                    queryDois = queryDois.Where(obj => obj.DataMedicao.Date <= filtrosPesquisa.DataSolicitacaoFinal.Value.Date);

                if (filtrosPesquisa.DataCriacaoInicial.HasValue)
                    queryDois = queryDois.Where(obj => obj.DataCriacao.Date >= filtrosPesquisa.DataCriacaoInicial.Value.Date);

                queryDois = queryDois
                    .AsEnumerable()
                    .GroupBy(obj => new { obj.DataMedicao, obj.Produto, obj.ClienteBase })
                    .SelectMany(g => g.Where(x => x.Codigo == g.Max(y => y.Codigo))).AsQueryable();

                List<int> ultimasSolicitacoesDiarias = queryDois.Select(x => x.Codigo).ToList();

                query = query.Where(obj => ultimasSolicitacoesDiarias.Contains(obj.Codigo));
            }

            return query;
        }

        #endregion
    }
}
