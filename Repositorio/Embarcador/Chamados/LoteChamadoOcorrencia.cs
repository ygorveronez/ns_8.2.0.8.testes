using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class LoteChamadoOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia>
    {
        #region Construtores

        public LoteChamadoOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public int ObterProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia>();

            var result = from obj in query select obj;

            int? retorno = result.Max(o => (int?)o.NumeroLote);
            return retorno.HasValue ? (retorno.Value + 1) : 1;

        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Chamado.LoteChamadoOcorrencia> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaLoteChamadoOcorrencia filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsultaLoteChamadoOcorrencia(filtrosPesquisa, false, parametrosConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Chamado.LoteChamadoOcorrencia)));

            return consulta.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Chamado.LoteChamadoOcorrencia>();
        }

        public int Contar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaLoteChamadoOcorrencia filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsultaLoteChamadoOcorrencia(filtrosPesquisa, true, null));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Chamado.AtendimentoPendente> ConsultarAtendimentosPendentes(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAtendimentosPendentes filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsultaAtendimentosPendentes(filtrosPesquisa, false, parametrosConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Chamado.AtendimentoPendente)));

            return consulta.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Chamado.AtendimentoPendente>();
        }

        public int ContarAtendimentosPendentes(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAtendimentosPendentes filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsultaAtendimentosPendentes(filtrosPesquisa, true, null));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }


        #endregion

        #region Métodos Privados

        private string ObterConsultaLoteChamadoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaLoteChamadoOcorrencia filtrosPesquisa, bool somenteContar, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            string sql = "";
            if (somenteContar)
                sql = @"select count(0) ";
            else
                sql = ObterSelectLoteChamadoOcorrencia(sql);

            sql += "from T_LOTE_CHAMADO_OCORRENCIA LoteChamadoOcorrencia ";


            sql += ObterWhereLoteChamadoOcorrencia(filtrosPesquisa);

            if (parametrosConsulta != null)
            {
                sql += $"order by {parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "asc" : "desc")} ";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }

        private string ObterWhereLoteChamadoOcorrencia(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaLoteChamadoOcorrencia filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";
            string where = @"where 1=1 ";

            if (filtrosPesquisa.NumeroLote > 0)
                where += $"and LoteChamadoOcorrencia.LCO_NUMERO_LOTE = {filtrosPesquisa.NumeroLote} ";

            if (filtrosPesquisa.DataCriacaoInicial != DateTime.MinValue)
                where += $"and LoteChamadoOcorrencia.LCO_DATA_CRIACAO >= '{filtrosPesquisa.DataCriacaoInicial.ToString(pattern)}' ";

            if (filtrosPesquisa.DataCriacaoFinal != DateTime.MinValue)
                where += $"and LoteChamadoOcorrencia.LCO_DATA_CRIACAO <= '{filtrosPesquisa.DataCriacaoFinal.AddDays(1).ToString(pattern)}' ";

            if (filtrosPesquisa.Situacao.Count > 0)
                where += $"and LoteChamadoOcorrencia.LCO_SITUACAO in ({string.Join(", ", filtrosPesquisa.Situacao.Select(x => (int)x).ToList())}) ";

            if (filtrosPesquisa.CodigoTransportador > 0)
                where += $@"and {filtrosPesquisa.CodigoTransportador} in ((select _empresa.EMP_CODIGO
							from T_EMPRESA _empresa
                            left join  T_CHAMADOS _chamado on _chamado.EMP_CODIGO = _empresa.EMP_CODIGO
                            where _chamado.LCO_CODIGO = LoteChamadoOcorrencia.LCO_CODIGO)) ";

            return where;
        }

        private string ObterSelectLoteChamadoOcorrencia(string sql)
        {
            string pattern = "yyyy-MM-dd";

            sql += @"select 
                        LoteChamadoOcorrencia.LCO_CODIGO Codigo,
                        LoteChamadoOcorrencia.LCO_NUMERO_LOTE NumeroLote,
                        LoteChamadoOcorrencia.LCO_DATA_CRIACAO DataCriacao,
                        LoteChamadoOcorrencia.LCO_SITUACAO Situacao, 
                        (SUBSTRING((select DISTINCT ', ' + CONCAT(_empresa.EMP_CODIGO_INTEGRACAO, ' - ', _empresa.EMP_RAZAO) 
							from T_CHAMADOS _chamado
                            left join T_EMPRESA _empresa on _chamado.EMP_CODIGO = _empresa.EMP_CODIGO
                            where _chamado.LCO_CODIGO = LoteChamadoOcorrencia.LCO_CODIGO FOR XML PATH('')), 3, 4000)) Transportadores ";

            return sql;
        }


        private string ObterConsultaAtendimentosPendentes(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAtendimentosPendentes filtrosPesquisa, bool somenteContar, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            string sql = "";
            if (somenteContar)
                sql = @"select count(0) ";
            else
                sql = ObterSelectAtendimentosPendentes(sql, filtrosPesquisa);

            sql += @"from T_CHAMADOS Chamado
                            left join T_CARGA Carga on Carga.CAR_CODIGO = Chamado.CAR_CODIGO
                            left join T_MOTIVO_CHAMADA MotivoChamado on MotivoChamado.MCH_CODIGO = Chamado.MCH_CODIGO
                            left join T_EMPRESA Empresa on Chamado.EMP_CODIGO = Empresa.EMP_CODIGO
                            left join T_CLIENTE Cliente on Chamado.CLI_CGCCPF = Cliente.CLI_CGCCPF
							left join T_GRUPO_MOTIVO_CHAMADO GrupoMotivoChamado on GrupoMotivoChamado.GMC_CODIGO = Chamado.GMC_CODIGO
                            left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO
                            left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ";

            sql += ObterWhereAtendimentosPendentes(filtrosPesquisa);

            if (parametrosConsulta != null)
            {
                if (filtrosPesquisa.SituacaoLote == SituacaoLoteChamadoOcorrencia.EmEdicao && filtrosPesquisa.CodigoLote > 0)
                    sql += $"order by CodigoOrdenacao asc ";
                else
                    sql += $"order by {parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "asc" : "desc")} ";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $"offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }

        private string ObterSelectAtendimentosPendentes(string sql, Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAtendimentosPendentes filtrosPesquisa)
        {
            sql += $@"select Chamado.CHA_CODIGO Codigo,
                        CASE 
                            WHEN Chamado.LCO_CODIGO = {filtrosPesquisa.CodigoLote} THEN 0 
                            ELSE 1                    
                        END CodigoOrdenacao,
                        Chamado.CHA_NUMERO Numero,
                        Chamado.CHA_DATA_CRICAO DataCriacao,
                        Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                        CONCAT_WS(' - ',GrupoMotivoChamado.GMC_CODIGO_INTEGRACAO, GrupoMotivoChamado.GMC_DESCRICAO) GrupoMotivoAtendimento,
                        CONCAT_WS(' - ',MotivoChamado.MCH_CODIGO_INTEGRACAO, MotivoChamado.MCH_DESCRICAO) MotivoChamado,
                        CONCAT_WS(' - ',Empresa.EMP_CODIGO_INTEGRACAO, Empresa.EMP_RAZAO) Transportador,
                        CONCAT_WS(' - ',Cliente.CLI_CODIGO_INTEGRACAO, Cliente.CLI_NOME) Cliente,
                        Veiculo.VEI_PLACA Veiculo,
                        (SUBSTRING((select DISTINCT ', ' + CAST(_notaFiscal.NF_NUMERO as varchar(100)) from T_XML_NOTA_FISCAL _notaFiscal
                            join T_CHAMADO_XML_NOTA_FISCAL _chamadoNotaFiscal on _chamadoNotaFiscal.NFX_CODIGO = _notaFiscal.NFX_CODIGO
                            where _chamadoNotaFiscal.CHA_CODIGO = Chamado.CHA_CODIGO FOR XML PATH('')), 3, 4000)) NotasFiscais,
                        Filial.FIL_DESCRICAO Filial ";

            return sql;
        }

        private string ObterWhereAtendimentosPendentes(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAtendimentosPendentes filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";
            string where = @"where 1=1 ";

            where += $"and ((Chamado.CHA_SITUACAO = 11 and (Chamado.LCO_CODIGO is null or Chamado.LCO_CODIGO = 0)) or Chamado.LCO_CODIGO = {filtrosPesquisa.CodigoLote}) ";

            if (filtrosPesquisa.NumeroInicial > 0)
                where += $"and Chamado.CHA_NUMERO >= {filtrosPesquisa.NumeroInicial} ";

            if (filtrosPesquisa.NumeroFinal > 0)
                where += $"and Chamado.CHA_NUMERO <= {filtrosPesquisa.NumeroFinal} ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                where += $"and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ";

            if (filtrosPesquisa.DataCriacaoInicial != DateTime.MinValue)
                where += $"and Chamado.CHA_DATA_CRICAO >= '{filtrosPesquisa.DataCriacaoInicial.ToString(pattern)}' ";

            if (filtrosPesquisa.DataCriacaoFinal != DateTime.MinValue)
                where += $"and Chamado.CHA_DATA_CRICAO <= '{filtrosPesquisa.DataCriacaoFinal.AddDays(1).ToString(pattern)}' ";

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where += $"and Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} ";

            if (filtrosPesquisa.CodigoMotivoChamado > 0)
                where += $"and Chamado.MCH_CODIGO = {filtrosPesquisa.CodigoMotivoChamado} ";

            if (filtrosPesquisa.CodigoGrupoMotivoAtendimento > 0)
                where += $"and Chamado.GMC_CODIGO = {filtrosPesquisa.CodigoGrupoMotivoAtendimento} ";

            if (filtrosPesquisa.CodigoTransportador > 0)
                where += $"and Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ";

            if (filtrosPesquisa.CodigoCliente > 0)
                where += $"and Cliente.CLI_CGCCPF = {filtrosPesquisa.CodigoCliente} ";

            if (filtrosPesquisa.CodigoFilial > 0)
                where += $"and Filial.FIL_CODIGO = {filtrosPesquisa.CodigoFilial} ";

            if (filtrosPesquisa.CodigoLote > 0 && filtrosPesquisa.SituacaoLote != SituacaoLoteChamadoOcorrencia.EmEdicao)
                where += $"and Chamado.LCO_CODIGO = {filtrosPesquisa.CodigoLote} ";

            if (filtrosPesquisa.CodigoLote == 0)
                where += "and (Chamado.LCO_CODIGO is null or Chamado.LCO_CODIGO = 0) ";

            return where;
        }



        #endregion
    }
}
