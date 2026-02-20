using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoExportacaoContabil : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil>
    {
        #region Métodos Globais

        public DocumentoExportacaoContabil(UnitOfWork unitOfWork) : base(unitOfWork) { }

        //public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        //{
        //    IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil> query = ObterQueryConsulta(filtrosPesquisa);

        //    if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
        //        query = query.OrderBy(parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar);

        //    if (parametrosConsulta.InicioRegistros > 0 || parametrosConsulta.LimiteRegistros > 0)
        //        query = query.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);

        //    return query.Fetch(o => o.CTe).ThenFetch(o => o.Serie)
        //                .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
        //                .Fetch(o => o.MovimentoFinanceiro)
        //                .ToList();
        //}

        //public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao filtrosPesquisa)
        //{
        //    IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil> query = ObterQueryConsulta(filtrosPesquisa);

        //    return query.Count();
        //}

        //public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil> BuscarPorLoteContabilizacao(int codigoLoteContabilizacao)
        //{
        //    IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil>();

        //    query = query.Where(o => o.LoteContabilizacao.Codigo == codigoLoteContabilizacao);

        //    return query.Fetch(o => o.Empresa)
        //                .Fetch(o => o.Tomador)
        //                .ToList();
        //}

        //public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorLoteContabilizacao(int codigoLoteContabilizacao)
        //{
        //    IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil>();

        //    query = query.Where(o => o.LoteContabilizacao.Codigo == codigoLoteContabilizacao && o.CTe != null);

        //    return query.Select(o => o.CTe).Distinct().ToList();
        //}

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil> ConsultarNovo(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = ObterQueryConsulta(filtrosPesquisa, parametrosConsulta, false);

            query = query.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil)));

            return query.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ConsultaDocumentoExportacaoContabil>();
        }

        public int ContarConsultaNovo(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao filtrosPesquisa)
        {
            var query = ObterQueryConsulta(filtrosPesquisa, null, true);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public void SetarLoteContabilizacao(int codigoLoteContabilizacao, List<int> codigosDocumentos)
        {
            decimal count = codigosDocumentos.Count / 2000m;
            int execucoes = 0;

            if (UnitOfWork.IsActiveTransaction())
            {
                while (execucoes < count)
                {
                    List<int> codigosDocumentosAux = codigosDocumentos.Skip(execucoes * 2000).Take(2000).ToList();

                    UnitOfWork.Sessao.CreateQuery("UPDATE DocumentoExportacaoContabil documento SET documento.LoteContabilizacao = :codigoLoteContabilizacao WHERE documento.Codigo IN (:codigosDocumentos)").SetInt32("codigoLoteContabilizacao", codigoLoteContabilizacao).SetParameterList("codigosDocumentos", codigosDocumentosAux).ExecuteUpdate();

                    execucoes++;
                }
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    while (execucoes < count)
                    {
                        List<int> codigosDocumentosAux = codigosDocumentos.Skip(execucoes * 2000).Take(2000).ToList();

                        UnitOfWork.Sessao.CreateQuery("UPDATE DocumentoExportacaoContabil documento SET documento.LoteContabilizacao = :codigoLoteContabilizacao WHERE documento.Codigo IN (:codigosDocumentos)").SetInt32("codigoLoteContabilizacao", codigoLoteContabilizacao).SetParameterList("codigosDocumentos", codigosDocumentosAux).ExecuteUpdate();

                        execucoes++;
                    }

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        #endregion

        #region Metodos Privados

        private NHibernate.IQuery ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool count)
        {
            StringBuilder sql = new StringBuilder();

            if (filtrosPesquisa.CodigoLoteContabilizacao <= 0)
                sql.Append(@"DECLARE @DocumentosInvalidosExportacaoContabil TABLE(DEC_CODIGO INT);

                             WITH DocumentosAptos AS (SELECT documento.DEC_TIPO, 
                             ISNULL(documento.CON_CODIGO, 0) CON_CODIGO, 
                             ISNULL(documento.CFT_CODIGO, 0) CFT_CODIGO
                             FROM T_DOCUMENTO_EXPORTACAO_CONTABIL documento 
                             INNER JOIN T_DOCUMENTO_EXPORTACAO_CONTABIL_CONTA conta on documento.DEC_CODIGO = conta.DEC_CODIGO 
	                         WHERE LCO_CODIGO IS NULL
                             GROUP BY
                             documento.DEC_TIPO, 
                             documento.CON_CODIGO, 
                             documento.CFT_CODIGO
                             HAVING 
                             SUM(CASE WHEN conta.DCC_TIPO in (1,3,5) THEN documento.DEC_VALOR ELSE -documento.DEC_VALOR END) = 0) 
                         
                             INSERT INTO @DocumentosInvalidosExportacaoContabil
                             SELECT DISTINCT d.DEC_CODIGO FROM DocumentosAptos DocumentoApto 
                             INNER JOIN T_DOCUMENTO_EXPORTACAO_CONTABIL d ON d.DEC_TIPO = DocumentoApto.DEC_TIPO and ISNULL(d.CON_CODIGO, 0) = DocumentoApto.CON_CODIGO and ISNULL(d.CFT_CODIGO, 0) = DocumentoApto.CFT_CODIGO ");

            sql.Append("SELECT ");

            if (count)
                sql.Append("COUNT(DocumentoExportacaoContabil.DEC_CODIGO) ");
            else
                sql.Append(@"DocumentoExportacaoContabil.DEC_CODIGO Codigo,
                             DocumentoExportacaoContabil.DEC_TIPO TipoDocumentoExportacaoContabil,
                             Tomador.CLI_CGCCPF CodigoTomador,
                             Tomador.CLI_NOME + (CASE Tomador.CLI_FISJUR WHEN 'F' THEN ' (' + FORMAT(Tomador.CLI_CGCCPF, '000\.000\.000\-00') + ')' WHEN 'J' THEN ' (' + FORMAT(Tomador.CLI_CGCCPF, '00\.000\.000\/0000\-00') + ')' ELSE '' END) Tomador,
                             Empresa.EMP_CODIGO CodigoEmpresa,
                             Empresa.EMP_RAZAO + ' (' + EmpresaLocalidade.LOC_DESCRICAO + '-' + EmpresaLocalidade.UF_SIGLA + ')' Empresa,
                             DocumentoExportacaoContabil.DEC_NUMERO Documento,
                             ModeloDocumentoFiscal.MOD_CODIGO CodigoModeloDocumento,
                             ModeloDocumentoFiscal.MOD_ABREVIACAO ModeloDocumento,
                             CONVERT(NVARCHAR(10), MovimentoFinanceiro.MOV_DATA, 103) DataMovimento,
                             DocumentoExportacaoContabil.DEC_VALOR Valor ");

            sql.Append(@"FROM T_DOCUMENTO_EXPORTACAO_CONTABIL DocumentoExportacaoContabil
                         INNER JOIN T_MOVIMENTO_FINANCEIRO MovimentoFinanceiro ON MovimentoFinanceiro.MOV_CODIGO = DocumentoExportacaoContabil.MOV_CODIGO
                         LEFT JOIN T_CTE CTe ON CTe.CON_CODIGO = DocumentoExportacaoContabil.CON_CODIGO
                         LEFT JOIN T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = CTe.CON_SERIE 
                         LEFT JOIN T_MODDOCFISCAL ModeloDocumentoFiscal ON ModeloDocumentoFiscal.MOD_CODIGO = CTe.CON_MODELODOC
                         LEFT JOIN T_CLIENTE Tomador ON Tomador.CLI_CGCCPF = DocumentoExportacaoContabil.CLI_TOMADOR
                         LEFT JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = DocumentoExportacaoContabil.EMP_CODIGO
                         LEFT JOIN T_LOCALIDADES EmpresaLocalidade ON EmpresaLocalidade.LOC_CODIGO = Empresa.LOC_CODIGO WHERE 1 = 1 ");

            if (filtrosPesquisa.CodigoLoteContabilizacao > 0)
                sql.Append($"AND DocumentoExportacaoContabil.LCO_CODIGO = {filtrosPesquisa.CodigoLoteContabilizacao} ");
            else
            {
                sql.Append(@"AND DocumentoExportacaoContabil.LCO_CODIGO IS NULL AND DocumentoExportacaoContabil.DEC_CODIGO IN (SELECT DEC_CODIGO FROM @DocumentosInvalidosExportacaoContabil) ");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
                    sql.Append($"AND DocumentoExportacaoContabil.DEC_NUMERO = '{filtrosPesquisa.NumeroDocumento}' ");

                if (filtrosPesquisa.CodigoEmpresa > 0)
                    sql.Append($"AND DocumentoExportacaoContabil.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa} ");

                if (filtrosPesquisa.CodigoModeloDocumentoFiscal > 0)
                    sql.Append($"AND CTe.CON_MODELODOC = {filtrosPesquisa.CodigoModeloDocumentoFiscal} ");

                if (filtrosPesquisa.CpfCnpjTomador > 0d)
                    sql.Append($"AND DocumentoExportacaoContabil.CLI_TOMADOR = {filtrosPesquisa.CpfCnpjTomador:F0} ");

                if (filtrosPesquisa.DataInicio.HasValue)
                    sql.Append($"AND MovimentoFinanceiro.MOV_DATA >= '{filtrosPesquisa.DataInicio:yyyy-MM-dd}' ");

                if (filtrosPesquisa.DataLimite.HasValue)
                    sql.Append($"AND MovimentoFinanceiro.MOV_DATA < '{filtrosPesquisa.DataLimite.Value.AddDays(1):yyyy-MM-dd}' ");

                if (filtrosPesquisa.Tipo.HasValue)
                    sql.Append($"AND DocumentoExportacaoContabil.DEC_TIPO = {filtrosPesquisa.Tipo:D} ");

                if (filtrosPesquisa.CodigosSelecionados?.Count > 0 && filtrosPesquisa.SelecionarTodos.HasValue)
                {
                    if (filtrosPesquisa.SelecionarTodos == true)
                        sql.Append($"AND DocumentoExportacaoContabil.DEC_CODIGO NOT IN ({string.Join(", ", filtrosPesquisa.CodigosSelecionados)}) ");
                    else
                        sql.Append($"AND DocumentoExportacaoContabil.DEC_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosSelecionados)}) ");
                }
            }

            if (!count && parametrosConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                    sql.Append($"ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar} ");

                if (parametrosConsulta.InicioRegistros > 0 || parametrosConsulta.LimiteRegistros > 0)
                    sql.Append($"OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH NEXT {parametrosConsulta.LimiteRegistros} ROWS ONLY ");
            }

            var query = SessionNHiBernate.CreateSQLQuery(sql.ToString());

            return query;
        }

        //public IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil> ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDocumentoLoteContabilizacao filtrosPesquisa)
        //{
        //    IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil>();

        //    if (filtrosPesquisa.CodigoLoteContabilizacao > 0)
        //        query = query.Where(o => o.LoteContabilizacao.Codigo == filtrosPesquisa.CodigoLoteContabilizacao);
        //    else
        //    {
        //        query = query.Where(o => o.LoteContabilizacao == null);

        //        if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
        //            query = query.Where(o => o.Numero == filtrosPesquisa.NumeroDocumento);

        //        if (filtrosPesquisa.CodigoEmpresa > 0)
        //            query = query.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

        //        if (filtrosPesquisa.CodigoModeloDocumentoFiscal > 0)
        //            query = query.Where(o => o.CTe.ModeloDocumentoFiscal.Codigo == filtrosPesquisa.CodigoModeloDocumentoFiscal);

        //        if (filtrosPesquisa.CpfCnpjTomador > 0d)
        //            query = query.Where(o => o.Tomador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador);

        //        if (filtrosPesquisa.DataInicio.HasValue)
        //            query = query.Where(o => o.MovimentoFinanceiro.DataMovimento >= filtrosPesquisa.DataInicio);

        //        if (filtrosPesquisa.DataLimite.HasValue)
        //            query = query.Where(o => o.MovimentoFinanceiro.DataMovimento < filtrosPesquisa.DataLimite.Value.AddDays(1));

        //        if (filtrosPesquisa.Tipo.HasValue)
        //            query = query.Where(o => o.TipoMovimento == filtrosPesquisa.Tipo);

        //        if (filtrosPesquisa.SelecionarTodos.HasValue)
        //        {
        //            if (filtrosPesquisa.SelecionarTodos == true)
        //                query = query.Where(o => !filtrosPesquisa.CodigosSelecionados.Contains(o.Codigo));
        //            else
        //                query = query.Where(o => filtrosPesquisa.CodigosSelecionados.Contains(o.Codigo));
        //        }
        //    }

        //    return query;
        //}

        #endregion
    }
}
