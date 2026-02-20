using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Documentos
{
    public class GestaoDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>
    {
        #region Construtores

        public GestaoDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public GestaoDocumento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public IList<int> ConsultarCodigos(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento filtrosPesquisa)
        {
            var parametros = new List<ParametroSQL>();

            filtrosPesquisa.RegistroComCarga = true;

            string sql = @"select CAST(GED_CODIGO AS INT) Codigo
                           from T_GESTAO_DOCUMENTO GestaoDocumento";

            sql += SetarJoinQueryGestaoDocumento();
            sql += SetarWhereQueryGestaoDocumento(filtrosPesquisa, ref parametros);

            var sqlDinamico = new SQLDinamico(sql, parametros);

            var consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);
            return consulta.SetTimeout(600).List<int>();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento> BuscarInconsistentesPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>()
                .Where(o => codigos.Contains(o.Codigo) &&
                       o.SituacaoGestaoDocumento == SituacaoGestaoDocumento.Inconsistente &&
                       (o.CargaCTe != null || o.CargaCTeComplementoInfo != null));

            return query
                .ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Documentos.GestaoDocumento> Consultar(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var sqlDinamico = QueryGestaoDocumento(filtrosPesquisa, false, parametroConsulta);

            var consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Documentos.GestaoDocumento)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Documentos.GestaoDocumento>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento filtrosPesquisa)
        {
            var sqlDinamico = QueryGestaoDocumento(filtrosPesquisa, true);

            var consulta = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public Dominio.Entidades.Embarcador.Documentos.GestaoDocumento BuscarPorCodigo(int codigo)
        {
            var consultaGestaoDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>()
                .Where(o => o.Codigo == codigo);

            return consultaGestaoDocumento
                .Fetch(obj => obj.CargaCTe).ThenFetch(obj => obj.PreCTe)
                .Fetch(obj => obj.CargaCTeComplementoInfo).ThenFetch(obj => obj.PreCTe)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.GestaoDocumento BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>()
                .Where(o => o.CTe.Codigo == codigoCTe && (o.CargaCTe == null || (o.CargaCTe.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.CargaCTe.Carga.SituacaoCarga != SituacaoCarga.Anulada)));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.GestaoDocumento BuscarPorCTeAnterior(string cteAnterior)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>();

            query = query.Where(o =>
            o.MotivoInconsistenciaGestaoDocumento == MotivoInconsistenciaGestaoDocumento.SemCarga
            && o.CTe.DocumentosTransporteAnterior.Any(obj => obj.Chave == cteAnterior));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento> BuscarPorCargasCTe(List<int> listaCodigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>()
                .Where(obj => listaCodigos.Contains(obj.CargaCTe.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.GestaoDocumento BuscarPorNotaFiscal(string chaveNF)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>();

            query = query.Where(o =>
            o.MotivoInconsistenciaGestaoDocumento == MotivoInconsistenciaGestaoDocumento.SemCarga
            && o.CTe.Documentos.Any(obj => obj.ChaveNFE == chaveNF));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.GestaoDocumento BuscarPorCTeComplementado(string cteComplementado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>();

            query = query.Where(o =>
            o.MotivoInconsistenciaGestaoDocumento == MotivoInconsistenciaGestaoDocumento.SemCarga
            && o.CTe.ChaveCTESubComp == cteComplementado);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.GestaoDocumento BuscarPorCodigoPedidoCliente(string codigoPedidoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>();
            query = query.Where(o => o.CargaCTe.PreCTe.CodigoPedidoCliente == codigoPedidoCliente);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento> BuscarSemRegraAprovacaoPorCodigos(List<int> codigosGestaoDocumento)
        {
            var consultaGestaoDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>()
                .Where(o => codigosGestaoDocumento.Contains(o.Codigo) && (o.SituacaoGestaoDocumento == SituacaoGestaoDocumento.Inconsistente || o.SituacaoGestaoDocumento == SituacaoGestaoDocumento.SemRegraAprovacao || o.SituacaoGestaoDocumento == SituacaoGestaoDocumento.EmTratativa));

            return consultaGestaoDocumento
                .Fetch(obj => obj.CargaCTe).ThenFetch(obj => obj.PreCTe)
                .Fetch(obj => obj.CargaCTeComplementoInfo).ThenFetch(obj => obj.PreCTe)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.GestaoDocumento BuscarPorCargaCTe(int codigoCargaCTe)
        {
            var consultaGestaoDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento>()
                .Where(o => o.CargaCTe.Codigo == codigoCargaCTe);

            return consultaGestaoDocumentos.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private SQLDinamico QueryGestaoDocumento(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = null)
        {
            var parametros = new List<ParametroSQL>();
            string sql;

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0) over ()) ";
            else
                sql = @"select 
                        GestaoDocumento.GED_CODIGO Codigo,
                        GestaoDocumento.CCT_CODIGO CodigoCargaCTe,
                        GestaoDocumento.CON_CODIGO CodigoCTe,
                        GestaoDocumento.GED_SITUACAO_GESTAO_DOCUMENTO SituacaoGestaoDocumento,
                        GestaoDocumento.GED_MOTIVO_INCONSISTENCIA_GESTAO_DOCUMENTO MotivoInconsistenciaGestaoDocumento,
                        GestaoDocumento.GED_VALOR_DESCONTO ValorDesconto,
                        GestaoDocumento.GED_NUMERO_NFE_RECEBIDA NFeRecebida,
                        GestaoDocumento.GED_DETALHES_INCONSISTENCIA DetalhesInconsistencia,
                        GestaoDocumento.GED_QUANTIDADE_IMPORTACOES_CTE QuantidadeImportacoesCTe,
                        GestaoDocumento.GED_DATA_IMPORTACAO_CTE DataImportacaoCTe,
                        PreCTe.PCO_CODIGO CodigoPreCTe,
                        CTe.CON_NUM Numero,
                        CTe.CON_VALOR_RECEBER Valor,
                        CTe.CON_CHAVECTE Chave,
                        CTe.CON_DATAHORAEMISSAO DataEmissao,
                        CTe.CON_VAL_ICMS ValorICMSRecebido,
                        CTE.CON_ALIQUOTA_IBS_ESTADUAL AliquotaIBSUF, 
                        CTE.CON_ALIQUOTA_IBS_MUNICIPAL AliquotaIBSMunicipal, 
                        CTE.CON_VALOR_IBS_ESTADUAL ValorIBSEstadual, 
                        CTE.CON_VALOR_IBS_MUNICIPAL ValorIBSMunicipal, 
                        CTE.CON_ALIQUOTA_CBS AliquotaCBS,
                        CTE.CON_VALOR_CBS ValorCBS,
                        PreCTe.PCO_VALOR_RECEBER ValorEsperado,
                        PreCTe.PCO_CODIGO_PEDIDO_CLIENTE NumeroPedidoCliente,
                        PreCTe.PCO_PESO_TOTAL_CARGA PesoCarga,
                        PreCTe.PCO_VALORES_FORMULA_CALCULO_FRETE_CARGA PesoCubado,
                        PreCTe.PCO_VAL_ICMS ValorICMSEsperado,
                        (ISNULL(PreCTe.PCO_VALOR_RECEBER, 0) - CTe.CON_VALOR_RECEBER) DiferencaValores,
                        EmpresaSerie.ESE_NUMERO Serie,
                        ISNULL(Carga.CAR_CODIGO_CARGA_EMBARCADOR, CargaOcorrencia.CAR_CODIGO_CARGA_EMBARCADOR) Carga,
                        CAST(Ocorrencia.COC_NUMERO_CONTRATO AS NVARCHAR(10)) Ocorrencia,
                        ISNULL(TipoOperacao.TOP_DESCRICAO, TipoOperacaoOcorrencia.TOP_DESCRICAO) TipoOperacao,
                        TomadorPagador.PCT_NOME Tomador,
                        (Empresa.EMP_RAZAO + ' (' + LocalidadeEmpresa.LOC_DESCRICAO + '-' + LocalidadeEmpresa.UF_SIGLA + ')' + ' - ' + Empresa.EMP_CNPJ) Transportador,
                        Remetente.PCT_NOME Remetente,
                        Destinatario.PCT_NOME Destinatario,
                        LocalidadeDestinatario.LOC_DESCRICAO CidadeDestino,
                        LocalidadeDestinatario.UF_SIGLA UFDestino,
                        isnull((
                            select sum(NF_VALOR_FRETE) 
                              from T_XML_NOTA_FISCAL xmlNotaFiscal
                              join T_CTE_XML_NOTAS_FISCAIS cteXmlNotaFiscal on cteXmlNotaFiscal.NFX_CODIGO = xmlNotaFiscal.NFX_CODIGO
                             where cteXmlNotaFiscal.CON_CODIGO = CTe.CON_CODIGO
                        ), 0) FreteNfXml,
                        isnull((
                            select sum(PNF_VALOR) 
                              from T_PRE_CTE_DOCS docs
                             where docs.PCO_CODIGO = PreCTe.PCO_CODIGO
                        ), 0) ValorNota,
                        substring((
                            select distinct ', ' + docs.PNF_CHAVENFE  
                              from T_PRE_CTE_DOCS docs
                             where docs.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')
                        ), 3, 4000) ChaveNota,
                        substring((
                            select distinct ', ' + Pedido.PED_NUMERO_PEDIDO_EMBARCADOR 
                              from T_CARGA_CTE CargaCTe 
                              join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                              join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                              join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                              join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                             where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')
                        ), 3, 200) NumeroPedido,
                        substring((
                            select distinct ', ' + (case when Pedido.PED_USAR_OUTRO_ENDERECO_DESTINO = 1 then PedidoOutroEndereco.COE_CEP else PedidoEndereco.PEN_CEP end)
                              from T_CARGA_CTE CargaCTe
                              join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO
                              join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO
                              join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                              join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                              join T_PEDIDO_ENDERECO PedidoEndereco on PedidoEndereco.PEN_CODIGO = Pedido.PEN_CODIGO_DESTINO
                              left join T_CLIENTE_OUTRO_ENDERECO PedidoOutroEndereco on PedidoOutroEndereco.COE_CODIGO = PedidoEndereco.COE_CODIGO
                             where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')
                        ), 3, 200) CEPDestino,
                        ISNULL((
							select MAX(RegrasAutorizacao.RAT_PRIORIDADE_APROVACAO) 
							 from T_REGRAS_AUTORIZACAO_DOCUMENTO RegrasAutorizacao
							 join T_AUTORIZACAO_ALCADA_DOCUMENTO AutorizacaoAlcadaDocumento on AutorizacaoAlcadaDocumento.RAD_CODIGO = RegrasAutorizacao.RAT_CODIGO
							where AutorizacaoAlcadaDocumento.GED_CODIGO = GestaoDocumento.GED_CODIGO AND AutorizacaoAlcadaDocumento.AAL_BLOQUEADA = 0
						),0) Prioridade, 
						ISNULL((
							select MAX(RegrasAutorizacao.RAT_Descricao) 
							 from T_REGRAS_AUTORIZACAO_DOCUMENTO RegrasAutorizacao
							 join T_AUTORIZACAO_ALCADA_DOCUMENTO AutorizacaoAlcadaDocumento on AutorizacaoAlcadaDocumento.RAD_CODIGO = RegrasAutorizacao.RAT_CODIGO
							where AutorizacaoAlcadaDocumento.GED_CODIGO = GestaoDocumento.GED_CODIGO AND AutorizacaoAlcadaDocumento.AAL_BLOQUEADA = 0
						),'') Regra,
                        ISNULL((
							select top 1 Aprovadores.FUN_NOME
							 from T_FUNCIONARIO Aprovadores
							 join T_AUTORIZACAO_ALCADA_DOCUMENTO AutorizacaoAlcadaDocumento on AutorizacaoAlcadaDocumento.FUN_CODIGO = Aprovadores.FUN_CODIGO
							where AutorizacaoAlcadaDocumento.GED_CODIGO = GestaoDocumento.GED_CODIGO and AutorizacaoAlcadaDocumento.AAL_SITUACAO = 1
                            order by AutorizacaoAlcadaDocumento.AAL_CODIGO desc
						),'') UltimoAprovador ";

            sql += " from T_GESTAO_DOCUMENTO GestaoDocumento";

            sql += SetarJoinQueryGestaoDocumento();
            sql += SetarWhereQueryGestaoDocumento(filtrosPesquisa, ref parametros);

            if (!somenteContarNumeroRegistros)
            {
                sql += $" order by {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}";

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametroConsulta.InicioRegistros} rows fetch next {parametroConsulta.LimiteRegistros} rows only;";
            }

            return new SQLDinamico(sql, parametros);
        }

        private string SetarJoinQueryGestaoDocumento()
        {
            return @"   join T_CTE CTe on CTe.CON_CODIGO = GestaoDocumento.CON_CODIGO
                        left outer join T_EMPRESA_SERIE EmpresaSerie on CTe.CON_SERIE = EmpresaSerie.ESE_CODIGO 
                        left outer join T_EMPRESA Empresa on CTe.EMP_CODIGO = Empresa.EMP_CODIGO 
                        left outer join T_LOCALIDADES LocalidadeEmpresa on LocalidadeEmpresa.LOC_CODIGO = Empresa.LOC_CODIGO
                        left outer join T_CTE_PARTICIPANTE TomadorPagador on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagador.PCT_CODIGO
                        left outer join T_CLIENTE TomadorPagadorCliente on TomadorPagador.CLI_CODIGO = TomadorPagadorCliente.CLI_CGCCPF 
                        left outer join T_CTE_PARTICIPANTE Remetente on CTe.CON_REMETENTE_CTE = Remetente.PCT_CODIGO 
                        left outer join T_CLIENTE RemetenteCliente on Remetente.CLI_CODIGO = RemetenteCliente.CLI_CGCCPF 
                        left outer join T_CTE_PARTICIPANTE Destinatario on CTe.CON_DESTINATARIO_CTE = Destinatario.PCT_CODIGO 
                        left outer join T_LOCALIDADES LocalidadeDestinatario on LocalidadeDestinatario.LOC_CODIGO = Destinatario.LOC_CODIGO
                        left outer join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = GestaoDocumento.CCT_CODIGO
                        left outer join T_CARGA_CTE_COMPLEMENTO_INFO CargaCTeComplementoInfo on CargaCTeComplementoInfo.CCC_CODIGO = GestaoDocumento.CCC_CODIGO
                        left outer join T_PRE_CTE PreCTe on PreCTe.PCO_CODIGO = isnull(CargaCTe.PCO_CODIGO, CargaCTeComplementoInfo.PCO_CODIGO)
                        left outer join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO 
                        left outer join T_TIPO_OPERACAO TipoOperacao on Carga.TOP_CODIGO = TipoOperacao.TOP_CODIGO 
                        left outer join T_FILIAL Filial on Carga.FIL_CODIGO = Filial.FIL_CODIGO 
                        left outer join T_CARGA_OCORRENCIA Ocorrencia on CargaCTeComplementoInfo.COC_CODIGO = Ocorrencia.COC_CODIGO
                        left outer join T_CARGA CargaOcorrencia on CargaOcorrencia.CAR_CODIGO = Ocorrencia.CAR_CODIGO 
                        left outer join T_TIPO_OPERACAO TipoOperacaoOcorrencia on TipoOperacaoOcorrencia.TOP_CODIGO = CargaOcorrencia.TOP_CODIGO 
                        left outer join T_FILIAL FilialOcorrencia on FilialOcorrencia.FIL_CODIGO = CargaOcorrencia.FIL_CODIGO";
        }

        private string SetarWhereQueryGestaoDocumento(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento filtrosPesquisa, ref List<ParametroSQL> parametros)
        {
            string where = " where 1 = 1 ";
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoEmpresa.Count > 0)
                where += $" and Empresa.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoEmpresa)})";

            if (filtrosPesquisa.CodigoCTe?.Count > 0)
                where += $" and CTe.CON_CODIGO in ({string.Join(",", filtrosPesquisa.CodigoCTe)})";

            if (filtrosPesquisa.Serie > 0)
                where += $" and EmpresaSerie.ESE_NUMERO = {filtrosPesquisa.Serie}";

            if (filtrosPesquisa.CodigoFilial > 0)
                where += $" and (Filial.FIL_CODIGO = {filtrosPesquisa.CodigoFilial} or FilialOcorrencia.FIL_CODIGO = {filtrosPesquisa.CodigoFilial})";

            if (filtrosPesquisa.NumeroNotaFiscal > 0)
                where += $" and CTe.CON_CODIGO IN (SELECT _notafiscal.CON_CODIGO FROM T_CTE_DOCS _notafiscal WHERE _notafiscal.NFC_NUMERO = '{filtrosPesquisa.NumeroNotaFiscal}') "; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.NumeroNotasFiscais?.Count > 0)
            {
                where += $" and CTe.CON_CODIGO IN (SELECT _notafiscal.CON_CODIGO FROM T_CTE_DOCS _notafiscal WHERE _notafiscal.NFC_NUMERO in(:NOTAFISCAL_NFC_NUMERO)) ";
                parametros.Add(new ParametroSQL("NOTAFISCAL_NFC_NUMERO", filtrosPesquisa.NumeroNotasFiscais));
            }

            if (filtrosPesquisa.Tomador?.Count > 0)
                where += $" and TomadorPagadorCliente.CLI_CGCCPF in ({string.Join(", ", filtrosPesquisa.Tomador)})";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
                where += $" and CTe.CON_CHAVECTE = '{filtrosPesquisa.Chave}'";

            if (filtrosPesquisa.Chaves?.Count > 0)
                where += $" and CTe.CON_CHAVECTE in('{string.Join("', '", filtrosPesquisa.Chaves)}')";

            if (filtrosPesquisa.SituacaoGestaoDocumento.Count > 0)
                where += $" and GestaoDocumento.GED_SITUACAO_GESTAO_DOCUMENTO IN ({string.Join(", ", filtrosPesquisa.SituacaoGestaoDocumento.Select(o => o.ToString("D")))})";

            if (filtrosPesquisa.MotivoInconsistenciaGestaoDocumento.Count > 0)
                where += $" and GestaoDocumento.GED_MOTIVO_INCONSISTENCIA_GESTAO_DOCUMENTO IN ({string.Join(", ", filtrosPesquisa.MotivoInconsistenciaGestaoDocumento.Select(o => o.ToString("D")))})";

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                where += $" and CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoInicial.ToString(pattern)}'";

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                where += $" and CTe.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataEmissaoFinal.AddDays(1).ToString(pattern)}'";

            if (filtrosPesquisa.CodigoCarga > 0)
                where += $" and (Carga.CAR_CODIGO = {filtrosPesquisa.CodigoCarga} or CargaOcorrencia.CAR_CODIGO = {filtrosPesquisa.CodigoCarga})";

            if (filtrosPesquisa.CodigoOcorrencia > 0)
                where += $" and Ocorrencia.COC_CODIGO = {filtrosPesquisa.CodigoOcorrencia}";

            if (filtrosPesquisa.Remetente > 0)
                where += $" and RemetenteCliente.CLI_CGCCPF = {filtrosPesquisa.Remetente}";

            if (filtrosPesquisa.CodigoTipoOperacao.Count > 0)
                where += $" and (TipoOperacao.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoTipoOperacao)}) or TipoOperacaoOcorrencia.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoTipoOperacao)}))";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
                where += $" and PreCTe.PCO_CODIGO_PEDIDO_CLIENTE = '{filtrosPesquisa.NumeroPedidoCliente}' ";

            if (filtrosPesquisa.NumeroPedidosClientes?.Count > 0)
                where += $" and PreCTe.PCO_CODIGO_PEDIDO_CLIENTE in ('{string.Join("', '", filtrosPesquisa.NumeroPedidosClientes)}') ";

            if (filtrosPesquisa.RegistroComCarga)
                where += $" and (CargaCTe.CCT_CODIGO is not null or CargaCTeComplementoInfo.CCC_CODIGO is not null)";

            if (filtrosPesquisa.CodigoUsuarioAprovador > 0)
                where += $" and exists (SELECT top(1) Aprovacao.AAL_CODIGO FROM T_AUTORIZACAO_ALCADA_DOCUMENTO Aprovacao WHERE Aprovacao.GED_CODIGO = GestaoDocumento.GED_CODIGO and Aprovacao.FUN_CODIGO = {filtrosPesquisa.CodigoUsuarioAprovador} AND Aprovacao.AAL_BLOQUEADA = 0 AND Aprovacao.AAL_SITUACAO <> 1) "; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.ChavesNFe?.Count > 0)
            {
                where += $" and PreCTe.PCO_CODIGO IN (SELECT docs.PCO_CODIGO FROM T_PRE_CTE_DOCS docs WHERE docs.PNF_CHAVENFE IN (:DOCS_PNF_CHAVENFE)) "; 
                parametros.Add(new ParametroSQL("DOCS_PNF_CHAVENFE", filtrosPesquisa.ChavesNFe));

            }

            return where;
        }

        #endregion
    }
}
