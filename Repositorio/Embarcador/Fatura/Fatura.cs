using AdminMultisoftware.Repositorio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.DataSource.Fatura;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Fatura
{
    public class Fatura : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.Fatura>
    {
        public Fatura(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Fatura(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Fatura.Fatura BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Fatura.Fatura BuscarPorNumero(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = from obj in query where obj.Numero == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Fatura> BuscarPorCodigo(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Fatura> BuscarTodasPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> resultFatura = from obj in queryFatura where obj.Documento.CTe.Codigo == codigoCTe select obj;

            return resultFatura.Select(o => o.Fatura).ToList();
        }

        public Dominio.Entidades.Embarcador.Fatura.Fatura BuscarPorProtocoloIntegracao(int protocolo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> resultFatura = from obj in queryFatura where obj.ProtocoloIntegracao == protocolo select obj;

            return resultFatura.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Fatura.Fatura BuscarPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> resultFatura = from obj in queryFatura where obj.Documento.CTe.Codigo == codigoCTe && !obj.Cancelado select obj;

            return resultFatura.Select(o => o.Fatura).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Fatura.Fatura BuscarCanceladoPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> resultFatura = from obj in queryFatura where obj.Documento.CTe.Codigo == codigoCTe && obj.Cancelado select obj;

            return resultFatura.Select(o => o.Fatura).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Fatura> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();
            query = query.Where(obj => obj.Situacao == SituacaoFatura.Fechado && (obj.Cargas.Any(o => o.Carga.Codigo == codigoCarga) || obj.Documentos.Any(o => o.Documento.CTe.CargaCTes.Any(c => c.Carga.Codigo == codigoCarga))));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Fatura> BuscarPorEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura etapa, int codigoFuncionario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = from obj in query where obj.Etapa == etapa && obj.Usuario.Codigo == codigoFuncionario select obj;
            return result.ToList();
        }

        public List<int> BuscarCodigosPorEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura etapa, bool geradaPorLote = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = from obj in query where obj.Etapa == etapa select obj;
            if (geradaPorLote)
                result = result.Where(o => o.GeradoPorFaturaLote == true);
            return result.Select(obj => obj.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorEtapaSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura etapa, bool geradaPorLote = false, bool comValorTotal = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = from obj in query where obj.Etapa == etapa && obj.Situacao == situacao select obj;
            if (geradaPorLote)
                result = result.Where(o => o.GeradoPorFaturaLote == true);
            if (comValorTotal)
                result = result.Where(o => o.Total > 0);
            return result.Select(obj => obj.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Fatura> ConsultarFaturaConhecimento(int codigoConhecimento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> result = from obj in query where obj.Fatura.Situacao != SituacaoFatura.Cancelado select obj;

            if (codigoConhecimento > 0)
                result = result.Where(o => o.Documento.CTe.Codigo == codigoConhecimento);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Select(obj => obj.Fatura).ToList();
        }

        public int ContarConsultarFaturaConhecimento(int codigoConhecimento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();

            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> result = from obj in query where obj.Fatura.Situacao != SituacaoFatura.Cancelado select obj;

            if (codigoConhecimento > 0)
                result = result.Where(o => o.Documento.CTe.Codigo == codigoConhecimento);

            return result.Select(o => o.Fatura).Count();
        }

        public int UltimoNumeracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            return query.Max(o => (int?)o.Numero) ?? 0;
        }

        public int UltimoNumeracao(int ano)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => o.DataFatura.Year == ano);

            return query.Max(o => (int?)o.Numero) ?? 0;
        }

        public int BuscarProximoControleNumeracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            return (query.Max(o => o.ControleNumeracao) ?? 0) + 1;
        }

        public int BuscarProximoControleNumeracao(int ano)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => o.DataFatura.Year == ano);

            return (query.Max(o => o.ControleNumeracao) ?? 0) + 1;
        }

        public Dominio.Entidades.Embarcador.Fatura.Fatura BuscarProximosDados()
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = from obj in query select obj;

            result = result.OrderBy("Numero descending");

            return result.FirstOrDefault();
        }

        public int ExisteFaturaFechadaPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(c => c.Carga.Codigo == codigoCarga);

            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> queryDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            queryDocumentoFaturamento = queryDocumentoFaturamento.Where(c => queryCargaCTe.Any(a => a.CTe == c.CTe));

            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => o.Situacao == SituacaoFatura.Fechado && (o.Carga.Codigo == codigoCarga || (queryDocumentoFaturamento.Any(f => o.Documentos.Any(d => d.Documento == f)))));

            return query.Select(f => f.Codigo).FirstOrDefault();
        }

        public bool ExisteOutrasCargasVinculadasNaFatura(int codigoCarga, int codigoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(c => c.Carga.Codigo == codigoCarga);

            IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> queryDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            queryDocumentoFaturamento = queryDocumentoFaturamento.Where(c => queryCargaCTe.Any(a => a.CTe == c.CTe));

            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => o.Codigo == codigoFatura && !(queryDocumentoFaturamento.Any(f => o.Documentos.Any(d => d.Documento == f))));

            return query.Any();
        }
        public bool ExisteFaturaPorViagem(int codigoViagem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => o.PedidoViagemNavio.Codigo == codigoViagem);

            return query.Any();
        }

        public bool ExistePorPreFaturaEGrupoPessoas(int codigoGrupoPessoas, long numeroPreFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas && o.NumeroPreFatura == numeroPreFatura && o.Situacao != SituacaoFatura.Cancelado);

            return query.Any();
        }

        public List<int> ConsultarSeExisteFaturaPendente(int codigoEmpresa, DateTime dataFechamento, SituacaoFatura[] situacaoFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => o.DataFatura < dataFechamento.AddDays(1).Date && situacaoFatura.Contains(o.Situacao));

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Empresa.Codigo == codigoEmpresa);

            return query.Select(o => o.Numero).ToList();
        }

        public List<int> ConsultaCodigos(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura filtroPesquisaFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = ObterConsultaFatura(filtroPesquisaFatura);

            return result.Select(c => c.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Fatura> Consulta(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura filtroPesquisaFatura, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)

        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = ObterConsultaFatura(filtroPesquisaFatura);
            if (parametrosConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && !string.IsNullOrWhiteSpace(parametrosConsulta.DirecaoOrdenar))
                    result = result.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

                if (parametrosConsulta.InicioRegistros > 0 || parametrosConsulta.LimiteRegistros > 0)
                    result = result.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);
            }
            return result.Fetch(o => o.Cliente)
                         .ThenFetch(o => o.GrupoPessoas)
                         .Fetch(o => o.GrupoPessoas)
                         .Fetch(o => o.TipoOperacao).ToList();
        }

        public int ContaConsulta(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura filtroPesquisaFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = ObterConsultaFatura(filtroPesquisaFatura);

            return result.Count();
        }

        public IList<RelatorioPadraoFatura> BuscarRelatorioPadrao(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Configuracoes.ConfiguracaoGeral(UnitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            Configuracoes.ConfiguracaoFatura repositorioConfiguracaoFatura = new Configuracoes.ConfiguracaoFatura(UnitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repositorioConfiguracaoFatura.BuscarConfiguracaoPadrao();

            NHibernate.ISQLQuery query = this.SessionNHiBernate
                .CreateSQLQuery(ObterSelectRelatorioConsultaFaturaPadrao(fatura, configuracaoGeral.HabilitarFuncionalidadesProjetoGollum, configuracaoFatura.HabilitarLayoutFaturaNFSManual));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(RelatorioPadraoFatura)));

            return query.SetTimeout(6000).List<RelatorioPadraoFatura>();
        }

        private string ObterSelectRelatorioConsultaFaturaPadrao(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, bool habilitarFuncionalidadesProjetoGollum = false, bool habilitarLayoutFaturaNFSManual = false)
        {
            const string queryNovoModeloGollum = @"
                SELECT DISTINCT
                    FaturaDocumento.FAT_CODIGO AS Codigo, 
                    (CASE DocumentoFaturamento.DFA_TIPO_DOCUMENTO 
                        WHEN 1 THEN CTe.CON_NUM 
                        WHEN 2 THEN Carga.CAR_CODIGO_CARGA_EMBARCADOR 
                        ELSE 0
                    END) AS NumeroCTe,
                    (CASE ModeloDocumento.MOD_NUM 
                        WHEN '57' THEN SerieCTe.ESE_NUMERO
                        ELSE 0
                    END) AS SerieCTE,
                    (CASE ModeloDocumento.MOD_NUM 
                        WHEN '57' THEN CTe.CON_CHAVECTE
                        ELSE ''
                    END) AS ChaveCTe,
                    (CASE DocumentoFaturamento.DFA_TIPO_DOCUMENTO 
                        WHEN 1 THEN CTe.CON_DATAHORAEMISSAO 
                        WHEN 2 THEN Carga.CAR_DATA_FINALIZACAO_EMISSAO 
                        ELSE 0
                    END) AS DataEmissao,
                    FaturaDocumento.FDO_VALOR_COBRAR AS Total,
                    FaturaDocumento.FDO_VALOR_COBRAR AS Total,
                    CTe.CON_VAL_ICMS AS ICMS,
                    (CASE CTe.CON_CST WHEN '60' 
                          THEN CTe.CON_VALOR_RECEBER 
                          ELSE CTe.CON_VALOR_RECEBER - CTe.CON_VAL_ICMS 
                     END) AS ValorSemICMSBase,


                    SUBSTRING(
                        (SELECT DISTINCT ', ' + CAST(docs.NFC_NUMERO AS NVARCHAR(20))
                         FROM T_CTE_DOCS docs
                         JOIN T_CTE C ON docs.CON_CODIGO = Cte.CON_CODIGO
                         WHERE Cte.CON_CODIGO = C.CON_CODIGO
                         FOR XML PATH('')), 3, 1000) AS Notas,

                    (SELECT SUM(P.ICA_QTD)
                     FROM T_CTE_INF_CARGA P 
                     WHERE P.ICA_UN = '01' 
                       AND P.CON_CODIGO = Cte.CON_CODIGO) AS Peso,

                    ISNULL(
                        STUFF(
                            (SELECT DISTINCT ', ' + V2.PVN_DESCRICAO
                             FROM T_PEDIDO_VIAGEM_NAVIO V2
                             JOIN T_PEDIDO          P2  ON P2.PVN_CODIGO   = V2.PVN_CODIGO
                             JOIN T_CARGA_PEDIDO    CP2 ON CP2.PED_CODIGO  = P2.PED_CODIGO
                             JOIN T_CARGA_CTE       CC2 ON CC2.CAR_CODIGO  = CP2.CAR_CODIGO
                             WHERE CC2.CON_CODIGO = Cte.CON_CODIGO
                             FOR XML PATH('')), 1, 2, ''), ''
                    ) AS PedidoNavioDirecao,

                    CASE WHEN CargaDoCTe.CAR_CARGA_TAKE_OR_PAY = 1 
                         THEN '' 
                         ELSE ISNULL(CTe.CON_NUMERO_CONTROLE, '') 
                              + ' / ' + CAST(CTe.CON_NUM AS VARCHAR(20)) 
                    END AS NumeroControleCTe,

                    ISNULL(TerminalOrigem.TTI_CODIGO_TERMINAL,'') 
                        + ' / ' 
                        + ISNULL(TerminalDestino.TTI_CODIGO_TERMINAL,'')  AS Trecho,

                    CASE Fatura.FAT_NAO_UTILIZAR_MOEDA_ESTRANGEIRA 
                         WHEN 1 THEN 0 
                         ELSE ISNULL(DocumentoFaturamento.DFA_VALOR_TOTAL_MOEDA,0) 
                    END AS ValorTotalMoeda,

                    CASE Fatura.FAT_NAO_UTILIZAR_MOEDA_ESTRANGEIRA 
                         WHEN 1 THEN 0 
                         ELSE ISNULL(DocumentoFaturamento.DFA_MOEDA,0) 
                    END AS Moeda,

                    CTe.CON_BC_ICMS            AS BaseCalculoICMS,
                    CTe.CON_VALOR_TOTAL_MERC   AS ValorMercadoria,
                    DestinatarioCTe.PCT_NOME   AS Destinatario,
                    ISNULL(TPC.TOP_TIPO_PROPOSTA_MULTIMODAL,0) AS TipoProposta,

                    SUBSTRING(
                        (SELECT DISTINCT ', ' + CAST(ProdE.PRO_CODIGO_PRODUTO_EMBARCADOR AS NVARCHAR(20))
                         FROM T_CTE_PRODUTO CTeProd
                         LEFT JOIN T_PRODUTO_EMBARCADOR ProdE  ON CTeProd.PRO_CODIGO = ProdE.PRO_CODIGO
                         WHERE CTeProd.CON_CODIGO = Cte.CON_CODIGO
                         FOR XML PATH('')), 3, 1000) AS Produtos,

                    SUBSTRING(
                        (SELECT ', ' + CAST(CTeProd.CTP_QUANTIDADE AS NVARCHAR(20))
                         FROM T_CTE_PRODUTO CTeProd
                         WHERE CTeProd.CON_CODIGO = Cte.CON_CODIGO
                         FOR XML PATH('')), 3, 1000) AS Quantidades,

                    SUBSTRING(
                        (SELECT DISTINCT ', ' + CAST(DocOrig.CDO_NUMERO AS NVARCHAR(20))
                         FROM T_CTE_DOCUMENTO_ORIGINARIO DocOrig
                         WHERE Cte.CON_CODIGO = DocOrig.CON_CODIGO
                         FOR XML PATH('')), 3, 1000) AS CteOriginario,

                    CAST(
                        CASE WHEN Fatura.FAT_NAO_UTILIZAR_MOEDA_ESTRANGEIRA = 0 
                                  AND Tomador.CLI_FISJUR = 'E' 
                             THEN 1 ELSE 0 END 
                    AS BIT) AS TipoTomadorExterior,

                    CTe.CON_VALOR_COTACAO_MOEDA AS ValorCotacaoMoedaCTe
                FROM T_FATURA_DOCUMENTO FaturaDocumento
                JOIN T_FATURA                   Fatura   ON FaturaDocumento.FAT_CODIGO = Fatura.FAT_CODIGO
                LEFT JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento 
                                                   ON DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO
                LEFT JOIN T_CTE                   Cte      ON Cte.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                LEFT JOIN T_MODDOCFISCAL          ModeloDocumento ON ModeloDocumento.MOD_CODIGO = Cte.CON_MODELODOC
                LEFT JOIN T_EMPRESA_SERIE         SerieCTe ON Cte.CON_SERIE = SerieCTe.ESE_CODIGO
                LEFT JOIN T_CARGA                 Carga    ON Carga.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO
                LEFT JOIN T_TIPO_TERMINAL_IMPORTACAO TerminalOrigem  ON TerminalOrigem.TTI_CODIGO = Cte.CON_TERMINAL_ORIGEM
                LEFT JOIN T_TIPO_TERMINAL_IMPORTACAO TerminalDestino ON TerminalDestino.TTI_CODIGO = Cte.CON_TERMINAL_DESTINO
                LEFT JOIN T_CTE_PARTICIPANTE      DestinatarioCTe ON DestinatarioCTe.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE
                LEFT JOIN T_CARGA_CTE             CargaCTe ON CargaCTe.CON_CODIGO = Cte.CON_CODIGO
                LEFT JOIN T_CARGA                 CargaDoCTe ON CargaDoCTe.CAR_CODIGO = CargaCTe.CAR_CODIGO
                LEFT JOIN T_TIPO_OPERACAO         TPC       ON TPC.TOP_CODIGO = CargaDoCTe.TOP_CODIGO
                LEFT JOIN T_CLIENTE               Tomador   ON Tomador.CLI_CGCCPF = Fatura.CLI_CGCCPF_TOMADOR
                LEFT JOIN T_CARGA_PEDIDO          CargaPedido ON CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO
                LEFT JOIN T_PEDIDO                Pedido       ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                LEFT JOIN T_PEDIDO_VIAGEM_NAVIO   Viagem       ON Viagem.PVN_CODIGO = Pedido.PVN_CODIGO 
                WHERE FaturaDocumento.FAT_CODIGO = {0}
                ORDER BY SerieCTe, NumeroCTe;
                ";

            const string queryModeloAntigoGollum = @"
                SELECT DISTINCT
                    FaturaDocumento.FAT_CODIGO Codigo,
                    (CASE DocumentoFaturamento.DFA_TIPO_DOCUMENTO 
                        WHEN 1 THEN CTe.CON_NUM 
                        WHEN 2 THEN Carga.CAR_CODIGO_CARGA_EMBARCADOR 
                        ELSE 0 
                    END) NumeroCTe,
                    CASE 
                        WHEN ModeloDocumento.MOD_NUM = '57' THEN SerieCTe.ESE_NUMERO 
                        ELSE 0 
                    END SerieCTE,
                    (CASE DocumentoFaturamento.DFA_TIPO_DOCUMENTO 
                        WHEN 1 THEN CTe.CON_DATAHORAEMISSAO 
                        WHEN 2 THEN Carga.CAR_DATA_FINALIZACAO_EMISSAO 
                        ELSE NULL 
                    END) DataEmissao,
                    (CASE CTe.CON_CST 
                        WHEN '60' THEN CTe.CON_VALOR_RECEBER 
                        ELSE CTe.CON_VALOR_RECEBER - CTe.CON_VAL_ICMS 
                    END) ValorSemICMS,
                    CTe.CON_VAL_ICMS ICMS,
                    FaturaDocumento.FDO_VALOR_COBRAR Total,
                    SUBSTRING((SELECT DISTINCT ', ' + CAST(docs.NFC_NUMERO AS NVARCHAR(20))
                        FROM T_CTE_DOCS docs
                        JOIN T_CTE cteSub ON docs.CON_CODIGO = cteSub.CON_CODIGO
                        WHERE cteSub.CON_CODIGO = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) as Notas,
                    (SELECT SUM(P.ICA_QTD) FROM T_CTE_INF_CARGA P WHERE P.ICA_UN = '01' AND P.CON_CODIGO = CTe.CON_CODIGO) Peso,
                    ISNULL(Viagem.PVN_DESCRICAO, '') PedidoNavioDirecao,
                    CASE WHEN CargaDoCTe.CAR_CARGA_TAKE_OR_PAY = 1 
                        THEN '' 
                        ELSE ISNULL(CTe.CON_NUMERO_CONTROLE, '') + ' / ' + CAST(CTe.CON_NUM AS VARCHAR(20)) 
                    END NumeroControleCTe,
                    ISNULL(TerminalOrigem.TTI_CODIGO_TERMINAL, '') + ' / ' + ISNULL(TerminalDestino.TTI_CODIGO_TERMINAL, '') Trecho,
                    ISNULL(DocumentoFaturamento.DFA_VALOR_TOTAL_MOEDA, 0) ValorTotalMoeda,
                    ISNULL(DocumentoFaturamento.DFA_MOEDA, 0) Moeda,
                    0 TipoProposta,
                    DocOrigi.CDO_NUMERO CteOriginario
                FROM T_FATURA_DOCUMENTO FaturaDocumento
                INNER JOIN T_FATURA Fatura ON FaturaDocumento.FAT_CODIGO = Fatura.FAT_CODIGO
                LEFT JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento ON DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO
                LEFT JOIN T_CTE CTe ON CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                LEFT JOIN T_MODDOCFISCAL ModeloDocumento ON ModeloDocumento.MOD_CODIGO = CTe.CON_MODELODOC
                LEFT JOIN T_EMPRESA_SERIE SerieCTe ON CTe.CON_SERIE = SerieCTe.ESE_CODIGO
                LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO
                LEFT JOIN T_TIPO_TERMINAL_IMPORTACAO TerminalOrigem ON TerminalOrigem.TTI_CODIGO = CTe.CON_TERMINAL_ORIGEM
                LEFT JOIN T_TIPO_TERMINAL_IMPORTACAO TerminalDestino ON TerminalDestino.TTI_CODIGO = CTe.CON_TERMINAL_DESTINO
                LEFT JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                LEFT JOIN T_CARGA CargaDoCTe ON CargaDoCTe.CAR_CODIGO = CargaCTe.CAR_CODIGO
                LEFT JOIN T_CTE_DOCUMENTO_ORIGINARIO DocOrigi ON DocOrigi.CON_CODIGO = CTe.CON_CODIGO
                LEFT JOIN T_PEDIDO_VIAGEM_NAVIO Viagem ON Viagem.PVN_CODIGO = CTe.CON_VIAGEM
                WHERE FaturaDocumento.FAT_CODIGO = {0}
                ORDER BY SerieCTe, NumeroCTe";

            const string queryNovoModelo = @"
                SELECT DISTINCT
                    FaturaDocumento.FAT_CODIGO AS Codigo, 
                    (CASE DocumentoFaturamento.DFA_TIPO_DOCUMENTO 
                        WHEN 1 THEN CTe.CON_NUM 
                        WHEN 2 THEN Carga.CAR_CODIGO_CARGA_EMBARCADOR 
                        ELSE 0
                    END) AS NumeroCTe,
                    (CASE ModeloDocumento.MOD_NUM 
                        WHEN '57' THEN SerieCTe.ESE_NUMERO
                        ELSE 0
                    END) AS SerieCTE,
                    (CASE ModeloDocumento.MOD_NUM 
                        WHEN '57' THEN Cte.CON_CHAVECTE
                        ELSE ''
                    END) AS ChaveCTe,
                    (CASE DocumentoFaturamento.DFA_TIPO_DOCUMENTO 
                        WHEN 1 THEN CTe.CON_DATAHORAEMISSAO 
                        WHEN 2 THEN Carga.CAR_DATA_FINALIZACAO_EMISSAO 
                        ELSE 0
                    END) AS DataEmissao,
                    FaturaDocumento.FDO_VALOR_COBRAR AS Total,
                    FaturaDocumento.FDO_VALOR_COBRAR AS Total,
                    CTe.CON_VAL_ICMS AS ICMS,
                    (CASE CTe.CON_CST WHEN '60' 
                          THEN CTe.CON_VALOR_RECEBER 
                          ELSE CTe.CON_VALOR_RECEBER - CTe.CON_VAL_ICMS 
                    END) AS ValorSemICMSBase,

                    SUBSTRING(
                        (SELECT DISTINCT ', ' + CAST(docs.NFC_NUMERO AS NVARCHAR(20))
                         FROM T_CTE_DOCS docs
                         JOIN T_CTE C ON docs.CON_CODIGO = Cte.CON_CODIGO
                         WHERE Cte.CON_CODIGO = C.CON_CODIGO
                         FOR XML PATH('')), 3, 1000) AS Notas,

                    (SELECT SUM(P.ICA_QTD) 
                     FROM T_CTE_INF_CARGA P 
                     WHERE P.ICA_UN = '01' 
                       AND P.CON_CODIGO = Cte.CON_CODIGO) AS Peso,

                    ISNULL(
                        STUFF(
                            (SELECT DISTINCT ', ' + V2.PVN_DESCRICAO
                             FROM T_FATURA_DOCUMENTO FD2
                             JOIN T_DOCUMENTO_FATURAMENTO DF2 ON DF2.DFA_CODIGO = FD2.DFA_CODIGO
                             JOIN T_CTE                   CT2  ON CT2.CON_CODIGO = DF2.CON_CODIGO
                             JOIN T_PEDIDO_VIAGEM_NAVIO   V2   ON V2.PVN_CODIGO = CT2.CON_VIAGEM
                             WHERE FD2.FAT_CODIGO = FaturaDocumento.FAT_CODIGO
                             FOR XML PATH('')), 1, 2, ''), ''
                    ) AS PedidoNavioDirecao,

                    CASE WHEN CargaDoCTe.CAR_CARGA_TAKE_OR_PAY = 1 
                         THEN '' 
                         ELSE ISNULL(CTe.CON_NUMERO_CONTROLE, '') 
                              + ' / ' + CAST(CTe.CON_NUM AS VARCHAR(20)) 
                    END AS NumeroControleCTe,

                    ISNULL(TerminalOrigem.TTI_CODIGO_TERMINAL,'') 
                        + ' / ' 
                        + ISNULL(TerminalDestino.TTI_CODIGO_TERMINAL,'') AS Trecho,

                    CASE Fatura.FAT_NAO_UTILIZAR_MOEDA_ESTRANGEIRA 
                         WHEN 1 THEN 0 
                         ELSE ISNULL(DocumentoFaturamento.DFA_VALOR_TOTAL_MOEDA,0) 
                    END AS ValorTotalMoeda,

                    CASE Fatura.FAT_NAO_UTILIZAR_MOEDA_ESTRANGEIRA 
                         WHEN 1 THEN 0 
                         ELSE ISNULL(DocumentoFaturamento.DFA_MOEDA,0) 
                    END AS Moeda,

                    CTe.CON_BC_ICMS            AS BaseCalculoICMS,
                    CTe.CON_VALOR_TOTAL_MERC   AS ValorMercadoria,
                    DestinatarioCTe.PCT_NOME   AS Destinatario,
                    ISNULL(TPC.TOP_TIPO_PROPOSTA_MULTIMODAL,0) AS TipoProposta,

                    SUBSTRING(
                        (SELECT DISTINCT ', ' + CAST(ProdE.PRO_CODIGO_PRODUTO_EMBARCADOR AS NVARCHAR(20))
                         FROM T_CTE_PRODUTO CTeProd
                         LEFT JOIN T_PRODUTO_EMBARCADOR ProdE 
                                ON CTeProd.PRO_CODIGO = ProdE.PRO_CODIGO
                         WHERE CTeProd.CON_CODIGO = Cte.CON_CODIGO
                         FOR XML PATH('')), 3, 1000) AS Produtos,

                    SUBSTRING(
                        (SELECT ', ' + CAST(CTeProd.CTP_QUANTIDADE AS NVARCHAR(20))
                         FROM T_CTE_PRODUTO CTeProd
                         WHERE CTeProd.CON_CODIGO = Cte.CON_CODIGO
                         FOR XML PATH('')), 3, 1000) AS Quantidades,

                    SUBSTRING(
                        (SELECT DISTINCT ', ' + CAST(DocOrig.CDO_NUMERO AS NVARCHAR(20))
                         FROM T_CTE_DOCUMENTO_ORIGINARIO DocOrig
                         WHERE Cte.CON_CODIGO = DocOrig.CON_CODIGO
                         FOR XML PATH('')), 3, 1000) AS CteOriginario,

                    CAST(
                        CASE WHEN Fatura.FAT_NAO_UTILIZAR_MOEDA_ESTRANGEIRA = 0 
                                  AND Tomador.CLI_FISJUR = 'E' 
                             THEN 1 ELSE 0 END 
                    AS BIT) AS TipoTomadorExterior,

                    CTe.CON_VALOR_COTACAO_MOEDA AS ValorCotacaoMoedaCTe
                FROM T_FATURA_DOCUMENTO        FaturaDocumento
                JOIN T_FATURA                  Fatura   ON FaturaDocumento.FAT_CODIGO = Fatura.FAT_CODIGO
                LEFT JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento 
                           ON DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO
                LEFT JOIN T_CTE                CTe      ON CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                LEFT JOIN T_MODDOCFISCAL       ModeloDocumento ON ModeloDocumento.MOD_CODIGO = CTe.CON_MODELODOC
                LEFT JOIN T_EMPRESA_SERIE      SerieCTe ON CTe.CON_SERIE = SerieCTe.ESE_CODIGO
                LEFT JOIN T_CARGA              Carga    ON Carga.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO
                LEFT JOIN T_PEDIDO_VIAGEM_NAVIO Viagem  ON Viagem.PVN_CODIGO = CTe.CON_VIAGEM
                LEFT JOIN T_TIPO_TERMINAL_IMPORTACAO TerminalOrigem  
                           ON TerminalOrigem.TTI_CODIGO = CTe.CON_TERMINAL_ORIGEM
                LEFT JOIN T_TIPO_TERMINAL_IMPORTACAO TerminalDestino 
                           ON TerminalDestino.TTI_CODIGO = CTe.CON_TERMINAL_DESTINO
                LEFT JOIN T_CTE_PARTICIPANTE   DestinatarioCTe 
                           ON DestinatarioCTe.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE
                LEFT JOIN T_CARGA_CTE          CargaCTe ON CargaCTe.CON_CODIGO = Cte.CON_CODIGO
                LEFT JOIN T_CARGA              CargaDoCTe ON CargaDoCTe.CAR_CODIGO = CargaCTe.CAR_CODIGO
                LEFT JOIN T_TIPO_OPERACAO      TPC       ON TPC.TOP_CODIGO = CargaDoCTe.TOP_CODIGO
                LEFT JOIN T_CLIENTE            Tomador   ON Tomador.CLI_CGCCPF = Fatura.CLI_CGCCPF_TOMADOR
                WHERE FaturaDocumento.FAT_CODIGO = {0}
                ORDER BY SerieCTe, NumeroCTe";

            const string queryModeloAntigo = @"
                SELECT F.FAT_CODIGO Codigo, C.CON_NUM NumeroCTe, 
                    CASE 
                        WHEN M.MOD_NUM = '57' THEN S.ESE_NUMERO 
                        ELSE 0 
                    END SerieCTE, 
                    C.CON_DATAHORAEMISSAO DataEmissao, 
                    C.CON_VALOR_RECEBER - C.CON_VAL_ICMS ValorSemICMS, 
                    C.CON_VAL_ICMS ICMS, C.CON_VALOR_RECEBER Total, 
                    SUBSTRING((SELECT DISTINCT ', ' + CAST(docs.NFC_NUMERO AS NVARCHAR(20))
                        FROM T_CTE_DOCS docs
                        JOIN T_CTE cte ON docs.CON_CODIGO = cte.CON_CODIGO
                        WHERE cte.CON_CODIGO = C.CON_CODIGO FOR XML PATH('')), 3, 1000) as Notas, 
                    (SELECT SUM(P.ICA_QTD) FROM T_CTE_INF_CARGA P WHERE P.ICA_UN = '01' AND P.CON_CODIGO = F.CON_CODIGO) Peso,
                    ISNULL(Viagem.PVN_DESCRICAO, '') PedidoNavioDirecao,
                    CASE WHEN CargaDoCTe.CAR_CARGA_TAKE_OR_PAY = 1 THEN '' ELSE ISNULL(C.CON_NUMERO_CONTROLE, '') + ' / ' +CAST(C.CON_NUM AS VARCHAR(20)) END NumeroControleCTe,
                    ISNULL(TerminalOrigem.TTI_CODIGO_TERMINAL, '') + ' / ' + ISNULL(TerminalDestino.TTI_CODIGO_TERMINAL, '') Trecho,
                    ISNULL(C.CON_VALOR_TOTAL_MOEDA, 0) ValorTotalMoeda,
                    ISNULL(C.CON_MOEDA, 0) Moeda, 0 TipoProposta,
                    CteOriginario = DocOriginario.CDO_NUMERO
                FROM T_FATURA_CARGA_DOCUMENTO F 
                JOIN T_CTE C ON C.CON_CODIGO = F.CON_CODIGO 
                JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = C.CON_SERIE 
                JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = CON_MODELODOC 
                LEFT JOIN T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = C.CON_VIAGEM
                LEFT JOIN T_TIPO_TERMINAL_IMPORTACAO TerminalOrigem ON TerminalOrigem.TTI_CODIGO = C.CON_TERMINAL_ORIGEM
                LEFT JOIN T_TIPO_TERMINAL_IMPORTACAO TerminalDestino ON TerminalDestino.TTI_CODIGO = C.CON_TERMINAL_DESTINO
                LEFT JOIN T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = C.CON_CODIGO
                LEFT JOIN T_CARGA CargaDoCTe on CargaDoCTe.CAR_CODIGO = CargaCTe.CAR_CODIGO 
                LEFT JOIN T_CTE_DOCUMENTO_ORIGINARIO DocOriginario on DocOriginario.CON_CODIGO = C.CON_CODIGO
                WHERE F.FCD_STATUS_DOCUMENTO = 1 AND F.FAT_CODIGO = {0}
                ORDER BY C.CON_SERIE, C.CON_NUM";

            return (habilitarFuncionalidadesProjetoGollum || habilitarLayoutFaturaNFSManual, fatura.NovoModelo) switch
            {
                (true, true) => string.Format(queryNovoModeloGollum, fatura.Codigo),
                (true, false) => string.Format(queryModeloAntigoGollum, fatura.Codigo),
                (false, true) => string.Format(queryNovoModelo, fatura.Codigo),
                (false, false) => string.Format(queryModeloAntigo, fatura.Codigo)
            };
        }

        public IList<RelatorioPadraoFaturaDados> BuscarRelatorioPadraoDados(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura? tipoImpressaoFatura = null)
        {
            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioConsultaFaturaPadraoDados(fatura, tipoImpressaoFatura));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(RelatorioPadraoFaturaDados)));

            return query.SetTimeout(6000).List<RelatorioPadraoFaturaDados>();
        }

        private string ObterSelectRelatorioConsultaFaturaPadraoDados(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura? tipoImpressaoFatura = null)
        {
            string select;

            if (fatura.NovoModelo)
            {
                string sqlNumeroTitulos = @" CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(T.TIT_CODIGO AS NVARCHAR(20))
							FROM T_TITULO T
							WHERE T.FAP_CODIGO = P.FAP_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(1000)) NumeroTitulos, ";
                string join = "";
                string sql = "";
                if (fatura.FaturaPropostaFaturamento)
                    sqlNumeroTitulos = "";

                if (tipoImpressaoFatura.HasValue && tipoImpressaoFatura.Value == TipoImpressaoFatura.Multimodal)
                {
                    join = @" LEFT OUTER JOIN T_FATURA_PARCELA Parcela on Parcela.FAT_CODIGO = F.FAT_CODIGO
						   LEFT OUTER JOIN T_TITULO Titulo on Titulo.FAP_CODIGO = Parcela.FAP_CODIGO
						   LEFT OUTER JOIN T_BOLETO_CONFIGURACAO Boleto on Boleto.BCF_CODIGO = Titulo.BCF_CODIGO ";
                    sql = @", ISNULL(Boleto.BCF_DIAS_PROTEST_TITULO, '') Prostesto, ISNULL(Boleto.BCF_JUROS_AM_TITULO, 0) Juros, ISNULL(Boleto.BCF_PERCENTUAL_MULTA_TITULO, 0) Multa";
                    if (fatura.FaturaPropostaFaturamento)
                        sql = @", '' Prostesto, 0.0 Juros, 0.0 Multa";
                }

                select = @"SELECT DISTINCT F.FAT_CODIGO Codigo,  
                           F.FAT_NUMERO Numero, 
                           F.FAT_NUMERO_FATURA_ORIGINAL NumeroOriginal,
                           F.FAT_TOTAL Valor, 
                           F.FAT_TOTAL ValorLiquido,  
                           P.FAP_DATA_VENCIMENTO DataVencimento, 
                           P.FAP_SEQUENCIA Sequencia, 
                           P.FAP_VALOR ValorTitulo, 
                           CASE WHEN F.FAT_MOEDA_COTACAO_BANCO_CENTRAL IS NULL OR F.FAT_MOEDA_COTACAO_BANCO_CENTRAL = 0 THEN F.FAT_DESCONTO ELSE (select ISNULL(SUM(Documento.FDO_VALOR_DESCONTO_MOEDA), 0) from T_FATURA_DOCUMENTO Documento where Documento.FAT_CODIGO = F.FAT_CODIGO) END ValorDesconto,
                           JD.JUS_DESCRICAO MotivoDesconto, 
                           CASE WHEN F.FAT_MOEDA_COTACAO_BANCO_CENTRAL IS NULL OR F.FAT_MOEDA_COTACAO_BANCO_CENTRAL = 0 THEN F.FAT_ACRESCIMO ELSE (select ISNULL(SUM(Documento.FDO_VALOR_ACRESCIMO_MOEDA), 0) from T_FATURA_DOCUMENTO Documento where Documento.FAT_CODIGO = F.FAT_CODIGO) END ValorAcrescimo,
                           JA.JUS_DESCRICAO MotivoAcrescimo,
                           C.CLI_NUMERO_CUIT_RUIT NumeroCUITRUIT,
                           CASE 
                                WHEN ISNULL(TP.TOP_TIPO_PROPOSTA_MULTIMODAL, ISNULL(TPC.TOP_TIPO_PROPOSTA_MULTIMODAL, 0)) = 8 
                                THEN 
                                    C.CLI_NOME + ' ' + ISNULL(G.GRP_CLASSIFICACAO_EMPRESA, '')
                                ELSE 
			                        C.CLI_NOME
	                        END NomePessoa, 
                           C.CLI_ENDERECO EnderecoPessoa, 
                           C.CLI_BAIRRO BairroPessoa,
                           C.CLI_CEP CEPPessoa,
                           C.CLI_EMAIL EmailPessoa,
                           C.CLI_NUMERO NumeroEnderecoPessoa,
                           L.LOC_DESCRICAO CidadePessoa, 
                           L.UF_SIGLA EstadoPessoa, 
                           C.CLI_CGCCPF CNPJPessoa,  
                           C.CLI_FISJUR TipoPessoa,
                           ISNULL((SELECT TOP(1) CPT.PCT_IERG FROM T_FATURA_DOCUMENTO FF
						   JOIN T_DOCUMENTO_FATURAMENTO DF ON DF.DFA_CODIGO = FF.DFA_CODIGO
						   JOIN T_CTE CC ON CC.CON_CODIGO = DF.CON_CODIGO
						   JOIN T_CTE_PARTICIPANTE CPT ON CPT.PCT_CODIGO = CC.CON_TOMADOR_PAGADOR_CTE
						   WHERE FF.FAT_CODIGO = F.FAT_CODIGO), C.CLI_IERG) IEPessoa,
                           G.GRP_DESCRICAO NomeGrupo, 
                           G.GRP_CNPJ CNPJRaizGrupo, 
                           F.FAT_OBSERVACAO Observacao, 
                           F.FAT_DATA_INICIAL DataInicial, 
                           F.FAT_DATA_FINAL DataFinal, 
                           ISNULL(F.FAT_OBSERVACAO_FATURA, '') ObservacaoFatura, 
                           F.FAT_IMPRIME_OBSERVACAO ImprimirObservacaoFatura, 
                           E.EMP_RAZAO RazaoEmpresa, 
                           E.EMP_FANTASIA FantasiaEmpresa,
                           E.EMP_ENDERECO EnderecoEmpresa, 
                           E.EMP_BAIRRO BairroEmpresa, 
                           E.EMP_NUMERO NumeroEnderecoEmpresa, 
                           E.EMP_CEP CEPEmpresa, 
                           E.EMP_CNPJ CNPJEmpresa, 
                           E.EMP_INSCRICAO IEEmpresa,
                           E.EMP_FONE FoneEmpresa, 
                           LE.LOC_DESCRICAO CidadeEmpresa, 
                           LE.UF_SIGLA EstadoEmpresa, 
                           E.EMP_COMPLEMENTO ComplementoEmpresa,
                           B.BCO_DESCRICAO NomeBanco, 
                           B.BCO_NUMERO NumeroBanco, 
                           F.FAT_BANCO_AGENCIA AgenciaBanco, 
                           F.FAT_BANCO_DIGITO_AGENCIA DigitoAgenciaBanco, 
                           F.FAT_BANCO_NUMERO_CONTA NumeroContaBanco, 
                           F.FAT_BANCO_TIPO_CONTA TipoContaBanco, 
                           ISNULL(CF.CLI_OBSERVACAO_FATURA, '') ObservacaoFaturaCliente, 
                           ISNULL(G.GRP_OBSERVACAO_FATURA, '') ObservacaoFaturaGrupo, F.FAT_NUMERO_PRE_FATURA NumeroPreFatura,
                           P.FAP_DATA_EMISSAO DataEmissao,
                           " + sqlNumeroTitulos + @"
                           (SELECT TOP 1 CONVERT(bit, CASE CEM_NAO_EXIBIR_TITULOS_NA_FATURA WHEN 1 THEN 0 ELSE 1 END) FROM T_CONFIGURACAO_EMBARCADOR) ExibirTitulos,
                           ISNULL(F.FAT_MOEDA_COTACAO_BANCO_CENTRAL, 0) MoedaCotacaoBancoCentral, 
                           F.FAT_DATA_BASE_CRT DataBaseCRT, 
                           F.FAT_VALOR_MOEDA_COTACAO ValorMoedaCotacao, 
                           P.FAP_VALOR_TOTAL_MOEDA ParcelaMoedaEstrangeira,
                           ISNULL(TP.TOP_TIPO_PROPOSTA_MULTIMODAL, ISNULL(TPC.TOP_TIPO_PROPOSTA_MULTIMODAL, 0)) TipoProposta,
                           (SELECT CASE WHEN PED.PED_PO_NUMBER_CONTABILIDADE IS NOT NULL OR PED.PED_PO_NUMBER_CONTABILIDADE IS NOT NULL
				                        THEN 'Feeder service ' +  ISNULL(PVN.PVN_DESCRICAO, '') + ' ' + ISNULL(PED.PED_PO_NUMBER_CONTABILIDADE, '') + ' ' + convert(varchar, P.FAP_DATA_VENCIMENTO, 103)
			                       ELSE convert(varchar, P.FAP_DATA_VENCIMENTO, 103)
			                       END) DescricaoServico,
                           (F.FAT_TOTAL_MOEDA_ESTRANGEIRA - (select ISNULL(SUM(Documento.FDO_VALOR_DESCONTO_MOEDA), 0) from T_FATURA_DOCUMENTO Documento where Documento.FAT_CODIGO = F.FAT_CODIGO) + (select ISNULL(SUM(Documento.FDO_VALOR_ACRESCIMO_MOEDA), 0) from T_FATURA_DOCUMENTO Documento where Documento.FAT_CODIGO = F.FAT_CODIGO)) TotalMoedaEstrangeira,
                           ConfiguracaoGeral.CEM_TIPO_IMPRESSAO_FATURA TipoImpressaoFatura" + sql + @"

                           FROM T_FATURA F
                           LEFT OUTER JOIN T_JUSTIFICATIVA JD ON JD.JUS_CODIGO = F.JUS_CODIGO_DESCONTO
                           LEFT OUTER JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = F.JUS_CODIGO_ACRESCIMO
                           LEFT OUTER JOIN T_CLIENTE CF ON CF.CLI_CGCCPF = F.CLI_CGCCPF
                           LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = F.CLI_CGCCPF_TOMADOR
                           LEFT OUTER JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                           LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.CLI_CGCCPF_TOMADOR = C.CLI_CGCCPF    
                           LEFT OUTER JOIN T_FATURA_PARCELA P ON P.FAT_CODIGO = F.FAT_CODIGO
                           LEFT OUTER JOIN T_FUNCIONARIO FU ON FU.FUN_CODIGO = F.FUN_CODIGO
                           LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = F.EMP_CODIGO
                           LEFT OUTER JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                           LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = F.CAR_CODIGO
                           LEFT OUTER JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = CA.CAR_CODIGO
                           LEFT OUTER JOIN T_PEDIDO PED ON PED.PED_CODIGO = CP.PED_CODIGO
                           LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO PVN ON PVN.PVN_CODIGO = CA.PVN_CODIGO
                           LEFT OUTER JOIN T_TIPO_OPERACAO TPC ON TPC.TOP_CODIGO = CA.TOP_CODIGO
                           LEFT OUTER JOIN T_TIPO_OPERACAO TP ON TP.TOP_CODIGO = F.TOP_CODIGO
                           LEFT OUTER JOIN T_BANCO B ON B.BCO_CODIGO = F.BCO_CODIGO " + join + @"
                                         , T_CONFIGURACAO_EMBARCADOR ConfiguracaoGeral
                           WHERE F.FAT_CODIGO = " + fatura.Codigo;
            }
            else
            {
                select = @"  SELECT F.FAT_CODIGO Codigo,   
                            F.FAT_NUMERO Numero,  
                            F.FAT_TOTAL Valor,  
                            F.FAT_TOTAL -  
                           (SELECT ISNULL(SUM(AD.FAD_VALOR), 0)  
                            FROM T_FATURA_ACRESCIMO_DESCONTO AD  
                            JOIN T_JUSTIFICATIVA J ON J.JUS_CODIGO = AD.JUS_CODIGO  
                            WHERE J.JUS_TIPO = 1 AND AD.FAT_CODIGO = F.FAT_CODIGO) +   
                           (SELECT ISNULL(SUM(AD.FAD_VALOR), 0)  
                            FROM T_FATURA_ACRESCIMO_DESCONTO AD  
                            JOIN T_JUSTIFICATIVA J ON J.JUS_CODIGO = AD.JUS_CODIGO  
                            WHERE J.JUS_TIPO = 2 AND AD.FAT_CODIGO = F.FAT_CODIGO)  ValorLiquido,   
                            P.FAP_DATA_VENCIMENTO DataVencimento,  
                            P.FAP_SEQUENCIA Sequencia,  
                            P.FAP_VALOR ValorTitulo,  
                            (SELECT ISNULL(SUM(AD.FAD_VALOR), 0)  
                            FROM T_FATURA_ACRESCIMO_DESCONTO AD  
                            JOIN T_JUSTIFICATIVA J ON J.JUS_CODIGO = AD.JUS_CODIGO  
                            WHERE J.JUS_TIPO = 1 AND AD.FAT_CODIGO = F.FAT_CODIGO) ValorDesconto,   
                            JD.JUS_DESCRICAO MotivoDesconto,  
                            (SELECT ISNULL(SUM(AD.FAD_VALOR), 0)  
                            FROM T_FATURA_ACRESCIMO_DESCONTO AD  
                            JOIN T_JUSTIFICATIVA J ON J.JUS_CODIGO = AD.JUS_CODIGO  
                            WHERE J.JUS_TIPO = 2 AND AD.FAT_CODIGO = F.FAT_CODIGO) ValorAcrescimo,   
                            JA.JUS_DESCRICAO MotivoAcrescimo,  
                            C.CLI_NUMERO_CUIT_RUIT NumeroCUITRUIT,
                            CASE 
                                WHEN ISNULL(TP.TOP_TIPO_PROPOSTA_MULTIMODAL, ISNULL(TPC.TOP_TIPO_PROPOSTA_MULTIMODAL, 0)) = 8 
                                THEN 
                                    C.CLI_NOME + ' ' + ISNULL(G.GRP_CLASSIFICACAO_EMPRESA, '')
                                ELSE 
			                        C.CLI_NOME
	                        END NomePessoa,  
                            C.CLI_ENDERECO EnderecoPessoa,  
                            C.CLI_BAIRRO BairroPessoa,  
                            C.CLI_CEP CEPPessoa,  
                            C.CLI_NUMERO NumeroEnderecoPessoa,  
                            L.LOC_DESCRICAO CidadePessoa,  
                            L.UF_SIGLA EstadoPessoa,  
                            C.CLI_CGCCPF CNPJPessoa,
                            C.CLI_FISJUR TipoPessoa,
                            ISNULL((SELECT TOP(1) CPT.PCT_IERG FROM T_FATURA_DOCUMENTO FF
						   JOIN T_DOCUMENTO_FATURAMENTO DF ON DF.DFA_CODIGO = FF.DFA_CODIGO
						   JOIN T_CTE CC ON CC.CON_CODIGO = DF.CON_CODIGO
						   JOIN T_CTE_PARTICIPANTE CPT ON CPT.PCT_CODIGO = CC.CON_TOMADOR_PAGADOR_CTE
						   WHERE FF.FAT_CODIGO = F.FAT_CODIGO), C.CLI_IERG) IEPessoa,  
                            G.GRP_DESCRICAO NomeGrupo,  
                            G.GRP_CNPJ CNPJRaizGrupo,  
                            F.FAT_OBSERVACAO Observacao,  
                            F.FAT_DATA_INICIAL DataInicial,  
                            F.FAT_DATA_FINAL DataFinal,  
                            ISNULL(F.FAT_OBSERVACAO_FATURA, '') ObservacaoFatura,  
                            F.FAT_IMPRIME_OBSERVACAO ImprimirObservacaoFatura,  
                            E.EMP_FANTASIA RazaoEmpresa,  
                            E.EMP_FANTASIA FantasiaEmpresa,  
                            E.EMP_ENDERECO EnderecoEmpresa,  
                            E.EMP_BAIRRO BairroEmpresa,  
                            E.EMP_NUMERO NumeroEnderecoEmpresa,  
                            E.EMP_CEP CEPEmpresa,  
                            E.EMP_CNPJ CNPJEmpresa,  
                            E.EMP_INSCRICAO IEEmpresa,  
                            E.EMP_FONE FoneEmpresa,  
                            LE.LOC_DESCRICAO CidadeEmpresa,  
                            LE.UF_SIGLA EstadoEmpresa,  
                            E.EMP_COMPLEMENTO ComplementoEmpresa,  
                            B.BCO_DESCRICAO NomeBanco,  
                            B.BCO_NUMERO NumeroBanco,  
                            F.FAT_BANCO_AGENCIA AgenciaBanco,  
                            F.FAT_BANCO_DIGITO_AGENCIA DigitoAgenciaBanco,  
                            F.FAT_BANCO_NUMERO_CONTA NumeroContaBanco,  
                            F.FAT_BANCO_TIPO_CONTA TipoContaBanco,  
                            ISNULL(CF.CLI_OBSERVACAO_FATURA, '') ObservacaoFaturaCliente,  
                            ISNULL(G.GRP_OBSERVACAO_FATURA, '') ObservacaoFaturaGrupo, F.FAT_NUMERO_PRE_FATURA NumeroPreFatura,  
                            F.FAT_DATA_FATURA DataEmissao,  
                            CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(T.TIT_CODIGO AS NVARCHAR(20))  
                            FROM T_TITULO T  
                            WHERE T.FAP_CODIGO = P.FAP_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(1000)) NumeroTitulos,
                            ISNULL(F.FAT_MOEDA_COTACAO_BANCO_CENTRAL, 0) MoedaCotacaoBancoCentral, 
                            F.FAT_DATA_BASE_CRT DataBaseCRT, 
                            F.FAT_VALOR_MOEDA_COTACAO ValorMoedaCotacao, 
                            P.FAP_VALOR_TOTAL_MOEDA ParcelaMoedaEstrangeira,
                            F.FAT_TOTAL_MOEDA_ESTRANGEIRA TotalMoedaEstrangeira,
                            ISNULL(TP.TOP_TIPO_PROPOSTA_MULTIMODAL, ISNULL(TPC.TOP_TIPO_PROPOSTA_MULTIMODAL, 0)) TipoProposta,
                            ConfiguracaoGeral.CEM_TIPO_IMPRESSAO_FATURA TipoImpressaoFatura

                            FROM T_FATURA F  
                            LEFT OUTER JOIN T_JUSTIFICATIVA JD ON JD.JUS_CODIGO = F.JUS_CODIGO_DESCONTO  
                            LEFT OUTER JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = F.JUS_CODIGO_ACRESCIMO  
                            LEFT OUTER JOIN T_CLIENTE CF ON CF.CLI_CGCCPF = F.CLI_CGCCPF   
                            LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = F.CLI_CGCCPF_TOMADOR  
                            LEFT OUTER JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO  
                            LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = F.GRP_CODIGO  
                            LEFT OUTER JOIN T_FATURA_PARCELA P ON P.FAT_CODIGO = F.FAT_CODIGO  
                            LEFT OUTER JOIN T_FUNCIONARIO FU ON FU.FUN_CODIGO = F.FUN_CODIGO  
                            LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = F.EMP_CODIGO  
                            LEFT OUTER JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO  
                            LEFT OUTER JOIN T_BANCO B ON B.BCO_CODIGO = F.BCO_CODIGO
                            LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = F.CAR_CODIGO
                            LEFT OUTER JOIN T_TIPO_OPERACAO TPC ON TPC.TOP_CODIGO = CA.TOP_CODIGO
                            LEFT OUTER JOIN T_TIPO_OPERACAO TP ON TP.TOP_CODIGO = F.TOP_CODIGO
                                          , T_CONFIGURACAO_EMBARCADOR ConfiguracaoGeral 
                            WHERE F.FAT_CODIGO  = " + fatura.Codigo;
            }

            return select;
        }

        public IList<RelatorioPorCte> BuscarRelatorioPorCte(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioConsultaFaturaPorCte(fatura));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(RelatorioPorCte)));

            return query.SetTimeout(6000).List<RelatorioPorCte>();
        }

        private string ObterSelectRelatorioConsultaFaturaPorCte(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            return
                $@"select Fatura.FAT_CODIGO Codigo,
                          Fatura.FAT_NUMERO Numero,
                          Fatura.FAT_BANCO_AGENCIA AgenciaBanco,
                          Fatura.FAT_BANCO_DIGITO_AGENCIA DigitoAgenciaBanco,
                          Fatura.FAT_BANCO_NUMERO_CONTA NumeroContaBanco,
                          Fatura.FAT_BANCO_TIPO_CONTA TipoContaBanco,
                          Fatura.FAT_NUMERO_PRE_FATURA NumeroPreFatura,
                          isnull(Fatura.FAT_OBSERVACAO_FATURA, '') ObservacaoFatura,
                          Fatura.FAT_IMPRIME_OBSERVACAO ImprimirObservacaoFatura,
                          Cte.CON_NUM NumeroCte,
                          Cte.CON_VAL_ICMS ICMS,
                          (case Cte.CON_CST when '60' then Cte.CON_VALOR_RECEBER else Cte.CON_VALOR_RECEBER - Cte.CON_VAL_ICMS end) as ValorSemICMS,
                          (case ModeloDocumento.MOD_NUM when '57' then SerieCte.ESE_NUMERO else 0 end) SerieCte,
                          Parcela.FAP_DATA_VENCIMENTO DataVencimento,
                          Parcela.FAP_DATA_EMISSAO DataEmissao,
                          Empresa.EMP_RAZAO RazaoEmpresa,
                          Empresa.EMP_ENDERECO EnderecoEmpresa,
                          Empresa.EMP_BAIRRO BairroEmpresa,
                          Empresa.EMP_NUMERO NumeroEnderecoEmpresa,
                          Empresa.EMP_CEP CEPEmpresa,
                          Empresa.EMP_CNPJ CNPJEmpresa,
                          Empresa.EMP_INSCRICAO IEEmpresa,
                          Empresa.EMP_FONE FoneEmpresa,
                          Empresa.EMP_COMPLEMENTO ComplementoEmpresa,
                          LocalidadeEmpresa.LOC_DESCRICAO CidadeEmpresa,
                          LocalidadeEmpresa.UF_SIGLA EstadoEmpresa,
                          Cliente.CLI_NOME NomePessoa,
                          Cliente.CLI_ENDERECO EnderecoPessoa,
                          Cliente.CLI_BAIRRO BairroPessoa,
                          Cliente.CLI_CEP CEPPessoa,
                          Cliente.CLI_NUMERO NumeroEnderecoPessoa,
                          Cliente.CLI_CGCCPF CNPJPessoa,
                          Cliente.CLI_IERG IEPessoa,
                          LocalidadeCliente.LOC_DESCRICAO CidadePessoa,
                          LocalidadeCliente.UF_SIGLA EstadoPessoa,
                          Banco.BCO_DESCRICAO NomeBanco,
                          Banco.BCO_NUMERO NumeroBanco,
                          FaturaDocumento.FDO_VALOR_ACRESCIMO ValorAcrescimo,
                          FaturaDocumento.FDO_VALOR_DESCONTO ValorDesconto,
                          FaturaDocumento.FDO_VALOR_TOTAL_COBRAR ValorLiquido,
                          FaturaDocumento.FDO_VALOR_COBRAR ValorTitulo,
                          (
                              select sum(CteInfCarga.ICA_QTD)
                                from T_CTE_INF_CARGA CteInfCarga
                               where CteInfCarga.ICA_UN = '01'
                                 and CteInfCarga.CON_CODIGO = Cte.CON_CODIGO
                          ) Peso,
						  substring((
                              select distinct ', ' + cast(Docs.NFC_NUMERO as nvarchar(20))
                                from T_CTE_DOCS Docs
                               where Docs.CON_CODIGO = Cte.CON_CODIGO
                                for xml path('')
                          ), 3, 1000) as Notas
                     from T_FATURA Fatura
                     join T_FATURA_DOCUMENTO FaturaDocumento on FaturaDocumento.FAT_CODIGO = Fatura.FAT_CODIGO
                     join T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO
                     join T_CTE Cte on Cte.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                     left join T_MODDOCFISCAL ModeloDocumento on ModeloDocumento.MOD_CODIGO = Cte.CON_MODELODOC
                     left join T_EMPRESA_SERIE SerieCte on SerieCte.ESE_CODIGO = Cte.CON_SERIE
                     left join T_FATURA_PARCELA Parcela on Parcela.FAT_CODIGO = Fatura.FAT_CODIGO
                     left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Cte.EMP_CODIGO
                     left join T_LOCALIDADES LocalidadeEmpresa on LocalidadeEmpresa.LOC_CODIGO = Empresa.LOC_CODIGO
                     left join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = Fatura.CLI_CGCCPF_TOMADOR
                     left join T_LOCALIDADES LocalidadeCliente on LocalidadeCliente.LOC_CODIGO = Cliente.LOC_CODIGO
                     LEFT JOIN T_BANCO Banco ON Banco.BCO_CODIGO = Fatura.BCO_CODIGO
                    WHERE Fatura.FAT_CODIGO = {fatura.Codigo}";
        }

        public void LimparCargasCtePorFatura(int codigoFatura)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FaturaIntegracao obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                     .SetInt32("codigoFatura", codigoFatura)
                                     .ExecuteUpdate();

                    UnitOfWork.Sessao.CreateQuery("DELETE FaturaParcela obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                     .SetInt32("codigoFatura", codigoFatura)
                                     .ExecuteUpdate();

                    UnitOfWork.Sessao.CreateQuery("DELETE FaturaAcrescimoDesconto obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                     .SetInt32("codigoFatura", codigoFatura)
                                     .ExecuteUpdate();

                    UnitOfWork.Sessao.CreateQuery("UPDATE ConhecimentoDeTransporteEletronico obj SET obj.Fatura = NULL WHERE obj.Fatura.Codigo = :codigoFatura")
                                     .SetInt32("codigoFatura", codigoFatura)
                                     .ExecuteUpdate();

                    UnitOfWork.Sessao.CreateQuery("DELETE FaturaCargaDocumento obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                     .SetInt32("codigoFatura", codigoFatura)
                                     .ExecuteUpdate();

                    UnitOfWork.Sessao.CreateQuery("DELETE FaturaCarga obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                     .SetInt32("codigoFatura", codigoFatura)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FaturaIntegracao obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                    .SetInt32("codigoFatura", codigoFatura)
                                    .ExecuteUpdate();

                        UnitOfWork.Sessao.CreateQuery("DELETE FaturaParcela obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                            .SetInt32("codigoFatura", codigoFatura)
                                            .ExecuteUpdate();

                        UnitOfWork.Sessao.CreateQuery("DELETE FaturaAcrescimoDesconto obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                            .SetInt32("codigoFatura", codigoFatura)
                                            .ExecuteUpdate();

                        UnitOfWork.Sessao.CreateQuery("UPDATE ConhecimentoDeTransporteEletronico obj SET obj.Fatura = NULL WHERE obj.Fatura.Codigo = :codigoFatura")
                                            .SetInt32("codigoFatura", codigoFatura)
                                            .ExecuteUpdate();

                        UnitOfWork.Sessao.CreateQuery("DELETE FaturaCargaDocumento obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                            .SetInt32("codigoFatura", codigoFatura)
                                            .ExecuteUpdate();

                        UnitOfWork.Sessao.CreateQuery("DELETE FaturaCarga obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                            .SetInt32("codigoFatura", codigoFatura)
                                            .ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fatura.Faturamento> RelatorioFaturamento(DateTime dataBaseInicial, DateTime dataBaseFinal, DateTime dataInicialEmissaoFatura, DateTime dataFinalEmissaoFatura, List<int> gruposPessoas, DateTime dataInicialEmissaoCTe, DateTime dataFinalEmissaoCTe, DateTime dataInicialQuitacao, DateTime dataFinalQuitacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura situacaoFatura, DateTime dataInicialVencimento, DateTime dataFinalVencimento, decimal valorInicial, decimal valorFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT F.FAT_NUMERO NumeroFatura,
                F.FAT_DATA_FATURA DataEmissao,
                ISNULL(T.TIT_DATA_VENCIMENTO, FP.FAP_DATA_VENCIMENTO) DataVencimento,

                (SELECT TOP 1 B.TIB_DATA_BASE FROM T_TITULO_BAIXA B
				JOIN T_TITULO_BAIXA_AGRUPADO BA ON BA.TIB_CODIGO = B.TIB_CODIGO
				WHERE BA.TIT_CODIGO = T.TIT_CODIGO ORDER BY B.TIB_CODIGO DESC) DataQuitacao,

                ISNULL(G.GRP_DESCRICAO, GC.GRP_DESCRICAO) Grupo,
                C.CLI_NOME Pessoa,
                F.FAT_TOTAL ValorFatura,
                (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 

                AS TotalAcrescimos,

                (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 
                AS TotalDescontos,

                F.FAT_TOTAL 				
				- 
				(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 
                + 
                (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) AS TotalFatura,

				CASE
                    WHEN F.FAT_SITUACAO <> 3 AND T.TIT_CODIGO IS NULL THEN  F.FAT_TOTAL 				
				- 
				(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 
                + 
                (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO)

                ELSE 0
				
				END SaldoAberto,

                CASE
                    WHEN F.FAT_SITUACAO = 3 THEN 'Cancelado'
	                WHEN ISNULL(T.TIT_STATUS, 1) = 1 THEN 'Em Aberto'
	                WHEN ISNULL(T.TIT_STATUS, 1) = 3 THEN 'Quitado'
	                ELSE 'Em Aberto'
                END StatusFinanceiro,
                T.TIT_CODIGO CodigoTitulo,
				ISNULL(T.TIT_SEQUENCIA, FP.FAP_SEQUENCIA) Sequencia,
				CASE
                    WHEN F.FAT_SITUACAO <> 3 AND T.TIT_CODIGO IS NULL THEN  F.FAT_TOTAL 				
				- 
				(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 
                + 
                (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO)

                ELSE 0 
				END ValorPendente,
				ISNULL(T.TIT_VALOR_PAGO, 0) ValorPago,
                F.FAT_CODIGO CodigoFatura,
                (SELECT TOP 1 M.MOV_DATA_BASE 
                   FROM T_MOVIMENTO_FINANCEIRO M WHERE (MOV_TIPO = 6 OR MOV_TIPO = 5)
					AND M.MOV_DOCUMENTO = (SELECT TOP 1 CAST(A.TIB_CODIGO AS VARCHAR(20)) 
									        FROM T_TITULO_BAIXA_AGRUPADO A
									        JOIN T_TITULO TT ON TT.TIT_CODIGO = A.TIT_CODIGO
									        WHERE A.TIT_CODIGO = T.TIT_CODIGO AND TT.TIT_STATUS = 3)) DataBaseQuitacao, 
                (SELECT SUBSTRING((SELECT DISTINCT ', ' + CAST(CTE.CON_NUM AS NVARCHAR(4000)) 
                                        FROM T_FATURA_CARGA_DOCUMENTO DOCS
							            JOIN T_CTE CTE ON CTE.CON_CODIGO = DOCS.CON_CODIGO
						            WHERE DOCS.FCD_STATUS_DOCUMENTO = 1 AND DOCS.FAT_CODIGO = FP.FAT_CODIGO FOR XML PATH('')), 3, 4000)) Conhecimentos,
                (SELECT SUBSTRING((SELECT DISTINCT ', ' + CAST(CARG.CAR_CODIGO_CARGA_EMBARCADOR AS NVARCHAR(4000))                                         
							            FROM T_CARGA CARG
										JOIN T_FATURA_CARGA FATCAR ON FATCAR.CAR_CODIGO = CARG.CAR_CODIGO 
						            WHERE FATCAR.FAC_STATUS <> 3 AND FATCAR.FAT_CODIGO = F.FAT_CODIGO FOR XML PATH('')), 3, 4000)) Cargas
                FROM T_FATURA F
                LEFT OUTER JOIN T_FATURA_PARCELA FP ON FP.FAT_CODIGO = F.FAT_CODIGO
                LEFT OUTER JOIN T_TITULO T ON T.FAP_CODIGO = FP.FAP_CODIGO
                LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = F.GRP_CODIGO
                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = F.CLI_CGCCPF
                LEFT OUTER JOIN T_GRUPO_PESSOAS GC ON GC.GRP_CODIGO = C.GRP_CODIGO";

            if ((int)situacaoFatura == -1)
                query += " WHERE F.FAT_SITUACAO <> 3 ";
            else if (situacaoFatura == SituacaoFatura.Cancelado)
                query += " WHERE F.FAT_SITUACAO = 3 ";
            else if (situacaoFatura == SituacaoFatura.EmAntamento)
                query += " WHERE (F.FAT_SITUACAO = 1)";// OR ISNULL(T.TIT_STATUS, 1) = 1)";
            else if (situacaoFatura == SituacaoFatura.Fechado)
                query += " WHERE (F.FAT_SITUACAO = 2)";// OR ISNULL(T.TIT_STATUS, 1) = 3 OR ISNULL(T.TIT_STATUS, 1) = 1)";
            else if (situacaoFatura == SituacaoFatura.Liquidado)
                query += " WHERE (F.FAT_SITUACAO = 4 OR ISNULL(T.TIT_STATUS, 1) = 3)";
            else
                query += " WHERE 1 = 1 ";

            if (codigoCTe > 0)
                query += " AND F.FAT_CODIGO IN (SELECT C.FAT_CODIGO FROM T_FATURA_CARGA_DOCUMENTO C WHERE C.FCD_STATUS_DOCUMENTO = 1 AND C.CON_CODIGO = " + codigoCTe.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoFatura > 0)
                query += " AND F.FAT_CODIGO = " + codigoFatura.ToString();

            if (codigoGrupoPessoa > 0)
                query += " AND (F.GRP_CODIGO = " + codigoGrupoPessoa.ToString() + " OR C.GRP_CODIGO = " + codigoGrupoPessoa.ToString() + ")";

            if (gruposPessoas != null && gruposPessoas.Count > 0)
            {
                IEnumerable<int> gruposPessoasValidos = gruposPessoas.Where(o => o > 0);

                if (gruposPessoasValidos.Count() > 0)
                    query += " and (F.GRP_CODIGO in (" + string.Join(",", gruposPessoasValidos) + ") OR C.GRP_CODIGO in (" + string.Join(",", gruposPessoasValidos) + "))";
                else
                    query += " and (F.GRP_CODIGO IS NULL AND C.GRP_CODIGO IS NULL)";
            }

            if (cnpjPessoa > 0)
                query += " AND F.CLI_CGCCPF = " + cnpjPessoa.ToString();

            if (dataBaseInicial > DateTime.MinValue && dataBaseFinal > DateTime.MinValue)
                query += @" AND T.TIT_CODIGO IN (SELECT A.TIT_CODIGO FROM T_TITULO_BAIXA_AGRUPADO A
                    JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_DOCUMENTO = CAST(A.TIB_CODIGO AS VARCHAR(20)) AND (M.MOV_TIPO = 6 OR M.MOV_TIPO = 5)
                    WHERE M.MOV_DATA_BASE >= '" + dataBaseInicial.ToString("MM/dd/yyyy") + @"' AND M.MOV_DATA_BASE <= '" + dataBaseFinal.ToString("MM/dd/yyyy 23:59:59") + "')";
            else if (dataBaseInicial > DateTime.MinValue && dataBaseFinal == DateTime.MinValue)
                query += @" AND T.TIT_CODIGO IN (SELECT A.TIT_CODIGO FROM T_TITULO_BAIXA_AGRUPADO A
                    JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_DOCUMENTO = CAST(A.TIB_CODIGO AS VARCHAR(20)) AND (M.MOV_TIPO = 6 OR M.MOV_TIPO = 5)
                    WHERE M.MOV_DATA_BASE >= '" + dataBaseInicial.ToString("MM/dd/yyyy") + @"')";
            else if (dataBaseInicial == DateTime.MinValue && dataBaseFinal > DateTime.MinValue)
                query += @" AND T.TIT_CODIGO IN (SELECT A.TIT_CODIGO FROM T_TITULO_BAIXA_AGRUPADO A
                    JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_DOCUMENTO = CAST(A.TIB_CODIGO AS VARCHAR(20)) AND (M.MOV_TIPO = 6 OR M.MOV_TIPO = 5)
                    WHERE M.MOV_DATA_BASE <= '" + dataBaseFinal.ToString("MM/dd/yyyy 23:59:59") + "')";

            if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND F.FAT_DATA_INICIAL >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' AND F.FAT_DATA_FINAL <= '" + dataFinalEmissao.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                query += " AND F.FAT_DATA_INICIAL >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND F.FAT_DATA_FINAL <= '" + dataFinalEmissao.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (dataInicialEmissaoFatura > DateTime.MinValue && dataFinalEmissaoFatura > DateTime.MinValue)
                query += " AND F.FAT_DATA_FATURA >= '" + dataInicialEmissaoFatura.ToString("MM/dd/yyyy") + "' AND F.FAT_DATA_FATURA <= '" + dataFinalEmissaoFatura.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialEmissaoFatura > DateTime.MinValue && dataFinalEmissaoFatura == DateTime.MinValue)
                query += " AND F.FAT_DATA_FATURA >= '" + dataInicialEmissaoFatura.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialEmissaoFatura == DateTime.MinValue && dataFinalEmissaoFatura > DateTime.MinValue)
                query += " AND F.FAT_DATA_FATURA <= '" + dataFinalEmissaoFatura.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND ISNULL(T.TIT_DATA_VENCIMENTO, FP.FAP_DATA_VENCIMENTO)>= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' AND ISNULL(T.TIT_DATA_VENCIMENTO, FP.FAP_DATA_VENCIMENTO) <= '" + dataFinalVencimento.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento == DateTime.MinValue)
                query += " AND ISNULL(T.TIT_DATA_VENCIMENTO, FP.FAP_DATA_VENCIMENTO) >= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialVencimento == DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND ISNULL(T.TIT_DATA_VENCIMENTO, FP.FAP_DATA_VENCIMENTO) <= '" + dataFinalVencimento.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += @" AND  ISNULL((SELECT TOP 1 B.TIB_DATA_BASE FROM T_TITULO_BAIXA B
				    JOIN T_TITULO_BAIXA_AGRUPADO BA ON BA.TIB_CODIGO = B.TIB_CODIGO
				    WHERE BA.TIT_CODIGO = T.TIT_CODIGO ORDER BY B.TIB_CODIGO DESC), T.TIT_DATA_LIQUIDACAO) >= '" + dataInicialQuitacao.ToString("MM/dd/yyyy") + @"' AND ISNULL((SELECT TOP 1 B.TIB_DATA_BASE FROM T_TITULO_BAIXA B
				    JOIN T_TITULO_BAIXA_AGRUPADO BA ON BA.TIB_CODIGO = B.TIB_CODIGO
				    WHERE BA.TIT_CODIGO = T.TIT_CODIGO ORDER BY B.TIB_CODIGO DESC), T.TIT_DATA_LIQUIDACAO) <= '" + dataFinalQuitacao.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao == DateTime.MinValue)
                query += @" AND ISNULL((SELECT TOP 1 B.TIB_DATA_BASE FROM T_TITULO_BAIXA B
				    JOIN T_TITULO_BAIXA_AGRUPADO BA ON BA.TIB_CODIGO = B.TIB_CODIGO
				    WHERE BA.TIT_CODIGO = T.TIT_CODIGO ORDER BY B.TIB_CODIGO DESC), T.TIT_DATA_LIQUIDACAO)  >= '" + dataInicialQuitacao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialQuitacao == DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += @" AND ISNULL((SELECT TOP 1 B.TIB_DATA_BASE FROM T_TITULO_BAIXA B
				    JOIN T_TITULO_BAIXA_AGRUPADO BA ON BA.TIB_CODIGO = B.TIB_CODIGO
				    WHERE BA.TIT_CODIGO = T.TIT_CODIGO ORDER BY B.TIB_CODIGO DESC), T.TIT_DATA_LIQUIDACAO)  <= '" + dataFinalQuitacao.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (dataInicialEmissaoCTe > DateTime.MinValue && dataFinalEmissaoCTe > DateTime.MinValue)
                query += " AND F.FAT_CODIGO IN (SELECT C.FAT_CODIGO FROM T_FATURA_CARGA_DOCUMENTO C JOIN T_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO WHERE C.FCD_STATUS_DOCUMENTO = 1 AND CC.CON_DATAHORAEMISSAO >= '" + dataInicialEmissaoCTe.ToString("MM/dd/yyyy") + "' AND CC.CON_DATAHORAEMISSAO <= '" + dataFinalEmissaoCTe.ToString("MM/dd/yyyy 23:59:59") + "') "; // SQL-INJECTION-SAFE
            else if (dataInicialEmissaoCTe > DateTime.MinValue && dataFinalEmissaoCTe == DateTime.MinValue)
                query += " AND F.FAT_CODIGO IN (SELECT C.FAT_CODIGO FROM T_FATURA_CARGA_DOCUMENTO C JOIN T_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO WHERE C.FCD_STATUS_DOCUMENTO = 1 AND CC.CON_DATAHORAEMISSAO >= '" + dataInicialEmissaoCTe.ToString("MM/dd/yyyy") + "') "; // SQL-INJECTION-SAFE
            else if (dataInicialEmissaoCTe == DateTime.MinValue && dataFinalEmissaoCTe > DateTime.MinValue)
                query += " AND F.FAT_CODIGO IN (SELECT C.FAT_CODIGO FROM T_FATURA_CARGA_DOCUMENTO C JOIN T_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO WHERE C.FCD_STATUS_DOCUMENTO = 1 AND CC.CON_DATAHORAEMISSAO <= '" + dataFinalEmissaoCTe.ToString("MM/dd/yyyy 23:59:59") + "') "; // SQL-INJECTION-SAFE

            if (valorInicial > 0 && valorFinal > 0)
                query += " AND F.FAT_TOTAL >= '" + valorInicial.ToString().Replace(",", ".") + "' AND F.FAT_TOTAL <= '" + valorFinal.ToString().Replace(",", ".") + "'";
            else if (valorInicial > 0 && valorFinal == 0)
                query += " AND F.FAT_TOTAL >= '" + valorInicial.ToString().Replace(",", ".") + "' ";
            else if (valorInicial == 0 && valorFinal > 0)
                query += " AND F.FAT_TOTAL <= '" + valorFinal.ToString().Replace(",", ".") + "' ";

            if ((int)status > 0)
            {
                if (status == StatusTitulo.EmAberto)
                    query += " AND ISNULL(T.TIT_STATUS, 1) = 1";
                else if (status == StatusTitulo.Quitada)
                    query += " AND ISNULL(T.TIT_STATUS, 1) = 3";
            }

            bool agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fatura.Faturamento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Fatura.Faturamento>();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Fatura> BuscarPorSituacao(SituacaoFatura situacao, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => o.Situacao == situacao);

            return query.OrderBy(propOrdenar + " " + dirOrdenar).Skip(inicio).Take(limite).ToList();
        }

        public List<int> BuscarCodigosPorSituacao(SituacaoFatura situacao, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => o.Situacao == situacao);

            return query.Select(o => o.Codigo).Skip(inicio).Take(limite).ToList();
        }

        public List<int> BuscarCodigosPorSituacao(SituacaoFatura situacao, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => o.Situacao == situacao);

            return query.Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public int ContarFaturamento(DateTime dataBaseInicial, DateTime dataBaseFinal, DateTime dataInicialEmissaoFatura, DateTime dataFinalEmissaoFatura, List<int> gruposPessoas, DateTime dataInicialEmissaoCTe, DateTime dataFinalEmissaoCTe, DateTime dataInicialQuitacao, DateTime dataFinalQuitacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura situacaoFatura, DateTime dataInicialVencimento, DateTime dataFinalVencimento, decimal valorInicial, decimal valorFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao)
        {
            string query = @"SELECT F.FAT_NUMERO NumeroFatura,
                F.FAT_DATA_FATURA DataEmissao,
                ISNULL(T.TIT_DATA_VENCIMENTO, FP.FAP_DATA_VENCIMENTO) DataVencimento,

                (SELECT TOP 1 B.TIB_DATA_BASE FROM T_TITULO_BAIXA B
				JOIN T_TITULO_BAIXA_AGRUPADO BA ON BA.TIB_CODIGO = B.TIB_CODIGO
				WHERE BA.TIT_CODIGO = T.TIT_CODIGO ORDER BY B.TIB_CODIGO DESC) DataQuitacao,

                ISNULL(G.GRP_DESCRICAO, GC.GRP_DESCRICAO) Grupo,
                C.CLI_NOME Pessoa,
                F.FAT_TOTAL ValorFatura,
                (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 

                AS TotalAcrescimos,

                (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 
                AS TotalDescontos,

                F.FAT_TOTAL 				
				- 
				(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 
                + 
                (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) AS TotalFatura,

				CASE
                    WHEN F.FAT_SITUACAO <> 3 AND T.TIT_CODIGO IS NULL THEN  F.FAT_TOTAL 				
				- 
				(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 
                + 
                (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO)

                ELSE 0
				
				END SaldoAberto,

                CASE
                    WHEN F.FAT_SITUACAO = 3 THEN 'Cancelado'
	                WHEN ISNULL(T.TIT_STATUS, 1) = 1 THEN 'Em Aberto'
	                WHEN ISNULL(T.TIT_STATUS, 1) = 3 THEN 'Quitado'
	                ELSE 'Em Aberto'
                END StatusFinanceiro,
                T.TIT_CODIGO CodigoTitulo,
				ISNULL(T.TIT_SEQUENCIA, FP.FAP_SEQUENCIA) Sequencia,
				CASE
                    WHEN F.FAT_SITUACAO <> 3 AND T.TIT_CODIGO IS NULL THEN  F.FAT_TOTAL 				
				- 
				(SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 1
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO) 
                + 
                (SELECT ISNULL(SUM(FA.FAD_VALOR), 0) FROM T_FATURA_ACRESCIMO_DESCONTO FA 
                JOIN T_JUSTIFICATIVA JA ON JA.JUS_CODIGO = FA.JUS_CODIGO AND JA.JUS_TIPO = 2
                WHERE FA.FAT_CODIGO = F.FAT_CODIGO)

                ELSE 0 
				END ValorPendente,
				ISNULL(T.TIT_VALOR_PAGO, 0) ValorPago,
                F.FAT_CODIGO CodigoFatura,
                (SELECT TOP 1 M.MOV_DATA_BASE 
                   FROM T_MOVIMENTO_FINANCEIRO M WHERE (MOV_TIPO = 6 OR MOV_TIPO = 5)
					AND M.MOV_DOCUMENTO = (SELECT TOP 1 CAST(A.TIB_CODIGO AS VARCHAR(20)) 
									        FROM T_TITULO_BAIXA_AGRUPADO A
									        JOIN T_TITULO TT ON TT.TIT_CODIGO = A.TIT_CODIGO
									        WHERE A.TIT_CODIGO = T.TIT_CODIGO AND TT.TIT_STATUS = 3)) DataBaseQuitacao, 
                (SELECT SUBSTRING((SELECT DISTINCT ', ' + CAST(CTE.CON_NUM AS NVARCHAR(4000)) 
                                        FROM T_FATURA_CARGA_DOCUMENTO DOCS
							            JOIN T_CTE CTE ON CTE.CON_CODIGO = DOCS.CON_CODIGO
						            WHERE DOCS.FCD_STATUS_DOCUMENTO = 1 AND DOCS.FAT_CODIGO = FP.FAT_CODIGO FOR XML PATH('')), 3, 4000)) Conhecimentos,
                (SELECT SUBSTRING((SELECT DISTINCT ', ' + CAST(CARG.CAR_CODIGO_CARGA_EMBARCADOR AS NVARCHAR(4000))                                         
							            FROM T_CARGA CARG
										JOIN T_FATURA_CARGA FATCAR ON FATCAR.CAR_CODIGO = CARG.CAR_CODIGO 
						            WHERE FATCAR.FAC_STATUS <> 3 AND FATCAR.FAT_CODIGO = F.FAT_CODIGO FOR XML PATH('')), 3, 4000)) Cargas
                FROM T_FATURA F
                LEFT OUTER JOIN T_FATURA_PARCELA FP ON FP.FAT_CODIGO = F.FAT_CODIGO
                LEFT OUTER JOIN T_TITULO T ON T.FAP_CODIGO = FP.FAP_CODIGO
                LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = F.GRP_CODIGO
                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = F.CLI_CGCCPF
                LEFT OUTER JOIN T_GRUPO_PESSOAS GC ON GC.GRP_CODIGO = C.GRP_CODIGO";

            if ((int)situacaoFatura == -1)
                query += " WHERE F.FAT_SITUACAO <> 3 ";
            else if (situacaoFatura == SituacaoFatura.Cancelado)
                query += " WHERE F.FAT_SITUACAO = 3 ";
            else if (situacaoFatura == SituacaoFatura.EmAntamento)
                query += " WHERE (F.FAT_SITUACAO = 1)";// OR ISNULL(T.TIT_STATUS, 1) = 1)";
            else if (situacaoFatura == SituacaoFatura.Fechado)
                query += " WHERE (F.FAT_SITUACAO = 2)";// OR ISNULL(T.TIT_STATUS, 1) = 3 OR ISNULL(T.TIT_STATUS, 1) = 1)";
            else if (situacaoFatura == SituacaoFatura.Liquidado)
                query += " WHERE (F.FAT_SITUACAO = 4 OR ISNULL(T.TIT_STATUS, 1) = 3)";
            else
                query += " WHERE 1 = 1 ";

            if (codigoCTe > 0)
                query += " AND F.FAT_CODIGO IN (SELECT C.FAT_CODIGO FROM T_FATURA_CARGA_DOCUMENTO C WHERE C.FCD_STATUS_DOCUMENTO = 1 AND C.CON_CODIGO = " + codigoCTe.ToString() + ")"; // SQL-INJECTION-SAFE

            if (codigoFatura > 0)
                query += " AND F.FAT_CODIGO = " + codigoFatura.ToString();

            if (codigoGrupoPessoa > 0)
                query += " AND (F.GRP_CODIGO = " + codigoGrupoPessoa.ToString() + " OR C.GRP_CODIGO = " + codigoGrupoPessoa.ToString() + ")";

            if (gruposPessoas != null && gruposPessoas.Count > 0)
            {
                IEnumerable<int> gruposPessoasValidos = gruposPessoas.Where(o => o > 0);

                if (gruposPessoasValidos.Count() > 0)
                    query += " and (F.GRP_CODIGO in (" + string.Join(",", gruposPessoasValidos) + ") OR C.GRP_CODIGO in (" + string.Join(",", gruposPessoasValidos) + "))";
                else
                    query += " and (F.GRP_CODIGO IS NULL AND C.GRP_CODIGO IS NULL)";
            }

            if (cnpjPessoa > 0)
                query += " AND F.CLI_CGCCPF = " + cnpjPessoa.ToString();

            if (dataBaseInicial > DateTime.MinValue && dataBaseFinal > DateTime.MinValue)
                query += @" AND T.TIT_CODIGO IN (SELECT A.TIT_CODIGO FROM T_TITULO_BAIXA_AGRUPADO A
                    JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_DOCUMENTO = CAST(A.TIB_CODIGO AS VARCHAR(20)) AND (M.MOV_TIPO = 6 OR M.MOV_TIPO = 5)
                    WHERE M.MOV_DATA_BASE >= '" + dataBaseInicial.ToString("MM/dd/yyyy") + @"' AND M.MOV_DATA_BASE <= '" + dataBaseFinal.ToString("MM/dd/yyyy 23:59:59") + "')";
            else if (dataBaseInicial > DateTime.MinValue && dataBaseFinal == DateTime.MinValue)
                query += @" AND T.TIT_CODIGO IN (SELECT A.TIT_CODIGO FROM T_TITULO_BAIXA_AGRUPADO A
                    JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_DOCUMENTO = CAST(A.TIB_CODIGO AS VARCHAR(20)) AND (M.MOV_TIPO = 6 OR M.MOV_TIPO = 5)
                    WHERE M.MOV_DATA_BASE >= '" + dataBaseInicial.ToString("MM/dd/yyyy") + @"')";
            else if (dataBaseInicial == DateTime.MinValue && dataBaseFinal > DateTime.MinValue)
                query += @" AND T.TIT_CODIGO IN (SELECT A.TIT_CODIGO FROM T_TITULO_BAIXA_AGRUPADO A
                    JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_DOCUMENTO = CAST(A.TIB_CODIGO AS VARCHAR(20)) AND (M.MOV_TIPO = 6 OR M.MOV_TIPO = 5)
                    WHERE M.MOV_DATA_BASE <= '" + dataBaseFinal.ToString("MM/dd/yyyy 23:59:59") + "')";

            if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND F.FAT_DATA_INICIAL >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' AND F.FAT_DATA_FINAL <= '" + dataFinalEmissao.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                query += " AND F.FAT_DATA_INICIAL >= '" + dataInicialEmissao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                query += " AND F.FAT_DATA_FINAL <= '" + dataFinalEmissao.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (dataInicialEmissaoFatura > DateTime.MinValue && dataFinalEmissaoFatura > DateTime.MinValue)
                query += " AND F.FAT_DATA_FATURA >= '" + dataInicialEmissaoFatura.ToString("MM/dd/yyyy") + "' AND F.FAT_DATA_FATURA <= '" + dataFinalEmissaoFatura.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialEmissaoFatura > DateTime.MinValue && dataFinalEmissaoFatura == DateTime.MinValue)
                query += " AND F.FAT_DATA_FATURA >= '" + dataInicialEmissaoFatura.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialEmissaoFatura == DateTime.MinValue && dataFinalEmissaoFatura > DateTime.MinValue)
                query += " AND F.FAT_DATA_FATURA <= '" + dataFinalEmissaoFatura.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND ISNULL(T.TIT_DATA_VENCIMENTO, FP.FAP_DATA_VENCIMENTO)>= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' AND ISNULL(T.TIT_DATA_VENCIMENTO, FP.FAP_DATA_VENCIMENTO) <= '" + dataFinalVencimento.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento == DateTime.MinValue)
                query += " AND ISNULL(T.TIT_DATA_VENCIMENTO, FP.FAP_DATA_VENCIMENTO) >= '" + dataInicialVencimento.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialVencimento == DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                query += " AND ISNULL(T.TIT_DATA_VENCIMENTO, FP.FAP_DATA_VENCIMENTO) <= '" + dataFinalVencimento.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += @" AND  ISNULL((SELECT TOP 1 B.TIB_DATA_BASE FROM T_TITULO_BAIXA B
				    JOIN T_TITULO_BAIXA_AGRUPADO BA ON BA.TIB_CODIGO = B.TIB_CODIGO
				    WHERE BA.TIT_CODIGO = T.TIT_CODIGO ORDER BY B.TIB_CODIGO DESC), T.TIT_DATA_LIQUIDACAO) >= '" + dataInicialQuitacao.ToString("MM/dd/yyyy") + @"' AND ISNULL((SELECT TOP 1 B.TIB_DATA_BASE FROM T_TITULO_BAIXA B
				    JOIN T_TITULO_BAIXA_AGRUPADO BA ON BA.TIB_CODIGO = B.TIB_CODIGO
				    WHERE BA.TIT_CODIGO = T.TIT_CODIGO ORDER BY B.TIB_CODIGO DESC), T.TIT_DATA_LIQUIDACAO) <= '" + dataFinalQuitacao.ToString("MM/dd/yyyy 23:59:59") + "'";
            else if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao == DateTime.MinValue)
                query += @" AND ISNULL((SELECT TOP 1 B.TIB_DATA_BASE FROM T_TITULO_BAIXA B
				    JOIN T_TITULO_BAIXA_AGRUPADO BA ON BA.TIB_CODIGO = B.TIB_CODIGO
				    WHERE BA.TIT_CODIGO = T.TIT_CODIGO ORDER BY B.TIB_CODIGO DESC), T.TIT_DATA_LIQUIDACAO)  >= '" + dataInicialQuitacao.ToString("MM/dd/yyyy") + "' ";
            else if (dataInicialQuitacao == DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                query += @" AND ISNULL((SELECT TOP 1 B.TIB_DATA_BASE FROM T_TITULO_BAIXA B
				    JOIN T_TITULO_BAIXA_AGRUPADO BA ON BA.TIB_CODIGO = B.TIB_CODIGO
				    WHERE BA.TIT_CODIGO = T.TIT_CODIGO ORDER BY B.TIB_CODIGO DESC), T.TIT_DATA_LIQUIDACAO)  <= '" + dataFinalQuitacao.ToString("MM/dd/yyyy 23:59:59") + "' ";

            if (dataInicialEmissaoCTe > DateTime.MinValue && dataFinalEmissaoCTe > DateTime.MinValue)
                query += " AND F.FAT_CODIGO IN (SELECT C.FAT_CODIGO FROM T_FATURA_CARGA_DOCUMENTO C JOIN T_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO WHERE C.FCD_STATUS_DOCUMENTO = 1 AND CC.CON_DATAHORAEMISSAO >= '" + dataInicialEmissaoCTe.ToString("MM/dd/yyyy") + "' AND CC.CON_DATAHORAEMISSAO <= '" + dataFinalEmissaoCTe.ToString("MM/dd/yyyy 23:59:59") + "') "; // SQL-INJECTION-SAFE
            else if (dataInicialEmissaoCTe > DateTime.MinValue && dataFinalEmissaoCTe == DateTime.MinValue)
                query += " AND F.FAT_CODIGO IN (SELECT C.FAT_CODIGO FROM T_FATURA_CARGA_DOCUMENTO C JOIN T_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO WHERE C.FCD_STATUS_DOCUMENTO = 1 AND CC.CON_DATAHORAEMISSAO >= '" + dataInicialEmissaoCTe.ToString("MM/dd/yyyy") + "') "; // SQL-INJECTION-SAFE
            else if (dataInicialEmissaoCTe == DateTime.MinValue && dataFinalEmissaoCTe > DateTime.MinValue)
                query += " AND F.FAT_CODIGO IN (SELECT C.FAT_CODIGO FROM T_FATURA_CARGA_DOCUMENTO C JOIN T_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO WHERE C.FCD_STATUS_DOCUMENTO = 1 AND CC.CON_DATAHORAEMISSAO <= '" + dataFinalEmissaoCTe.ToString("MM/dd/yyyy 23:59:59") + "') "; // SQL-INJECTION-SAFE

            if (valorInicial > 0 && valorFinal > 0)
                query += " AND F.FAT_TOTAL >= '" + valorInicial.ToString().Replace(",", ".") + "' AND F.FAT_TOTAL <= '" + valorFinal.ToString().Replace(",", ".") + "'";
            else if (valorInicial > 0 && valorFinal == 0)
                query += " AND F.FAT_TOTAL >= '" + valorInicial.ToString().Replace(",", ".") + "' ";
            else if (valorInicial == 0 && valorFinal > 0)
                query += " AND F.FAT_TOTAL <= '" + valorFinal.ToString().Replace(",", ".") + "' ";

            if ((int)status > 0)
            {
                if (status == StatusTitulo.EmAberto)
                    query += " AND ISNULL(T.TIT_STATUS, 1) = 1";
                else if (status == StatusTitulo.Quitada)
                    query += " AND ISNULL(T.TIT_STATUS, 1) = 3";
            }

            query = "SELECT COUNT(0) as CONTADOR FROM (" +
                query + ") AS TT";

            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento> ObterTitulosComDataCancelamentoSuperiorAFaturaPorCTe(int codigo)
        {
            string sqlQuery = @"select Titulo.TIT_CODIGO Codigo, Titulo.TIT_DATA_CANCELAMENTO DataCancelamento from t_fatura Fatura
                             inner join t_fatura_documento FaturaDocumento on Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO
                             inner join t_documento_faturamento DocumentoFaturamento on DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO
                             inner join t_titulo_documento TituloDocumento on (DocumentoFaturamento.DFA_TIPO_DOCUMENTO = 1 and TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO)
                             inner join t_titulo Titulo on Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO
                             where Fatura.fat_codigo = " + codigo.ToString() + " and Titulo.TIT_STATUS = 4 and Fatura.FAT_DATA_FATURA < Titulo.TIT_DATA_CANCELAMENTO";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento> ObterTitulosComDataCancelamentoSuperiorAFaturaPorCarga(int codigo)
        {
            string sqlQuery = @"select Titulo.TIT_CODIGO Codigo, Titulo.TIT_DATA_CANCELAMENTO DataCancelamento from t_fatura Fatura
                             inner join t_fatura_documento FaturaDocumento on Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO
                             inner join t_documento_faturamento DocumentoFaturamento on DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO
                             inner join t_titulo_documento TituloDocumento on (DocumentoFaturamento.DFA_TIPO_DOCUMENTO = 2 and TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)
                             inner join t_titulo Titulo on Titulo.TIT_CODIGO = TituloDocumento.TIT_CODIGO
                             where Fatura.fat_codigo = " + codigo.ToString() + " and Titulo.TIT_STATUS = 4 and Fatura.FAT_DATA_FATURA < Titulo.TIT_DATA_CANCELAMENTO";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento>();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Fatura> BuscarFaturasPorNumeroCTeSituacoes(int codigoCte)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura> situacoesPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura> {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Liquidado,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmFechamento,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.SemRegraAprovacao,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.AguardandoAprovacao,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.ProblemaIntegracao
                };

            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            query = query.Where(o => situacoesPermitidas.Contains(o.Situacao) &&
                                     (o.FaturaCargaDocumentos.Any(f => f.ConhecimentoDeTransporteEletronico.Codigo == codigoCte)
                                                                   || o.Documentos.Any(d => d.Documento.CTe.Codigo == codigoCte)));

            return query.Select(o => o).ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> ObterConsultaFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura filtroPesquisaFatura)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();

            if (filtroPesquisaFatura.NumeroCTe > 0)
                result = result.Where(o => o.FaturaCargaDocumentos.Any(f => f.StatusDocumentoFatura == StatusDocumentoFatura.Normal && f.ConhecimentoDeTransporteEletronico.Numero == filtroPesquisaFatura.NumeroCTe) || o.Documentos.Any(d => d.Documento.CTe.Numero == filtroPesquisaFatura.NumeroCTe));

            if (filtroPesquisaFatura.NumeroNota > 0)
                result = result.Where(o => o.FaturaCargaDocumentos.Any(f => f.StatusDocumentoFatura == StatusDocumentoFatura.Normal && f.ConhecimentoDeTransporteEletronico.Documentos.Any(d => d.Numero == filtroPesquisaFatura.NumeroNota.ToString("D"))) || o.Documentos.Any(d => d.Documento.CTe.Documentos.Any(dd => dd.Numero == filtroPesquisaFatura.NumeroNota.ToString("D"))));

            if (!string.IsNullOrWhiteSpace(filtroPesquisaFatura.NumeroControle))
                result = result.Where(o => o.FaturaCargaDocumentos.Any(f => f.StatusDocumentoFatura == StatusDocumentoFatura.Normal && f.ConhecimentoDeTransporteEletronico.NumeroControle == filtroPesquisaFatura.NumeroControle) || o.Documentos.Any(d => d.Documento.CTe.NumeroControle == filtroPesquisaFatura.NumeroControle));

            if (!string.IsNullOrWhiteSpace(filtroPesquisaFatura.NumeroOS))
                result = result.Where(o => o.FaturaCargaDocumentos.Any(f => f.StatusDocumentoFatura == StatusDocumentoFatura.Normal && f.ConhecimentoDeTransporteEletronico.NumeroOS == filtroPesquisaFatura.NumeroOS) || o.Documentos.Any(d => d.Documento.CTe.NumeroOS == filtroPesquisaFatura.NumeroOS));

            if (!string.IsNullOrWhiteSpace(filtroPesquisaFatura.NumeroBooking))
                result = result.Where(o => o.FaturaCargaDocumentos.Any(f => f.StatusDocumentoFatura == StatusDocumentoFatura.Normal && f.ConhecimentoDeTransporteEletronico.NumeroBooking == filtroPesquisaFatura.NumeroBooking) || o.Documentos.Any(d => d.Documento.CTe.NumeroBooking == filtroPesquisaFatura.NumeroBooking));

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> queryPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> queryFaturaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();

            if (filtroPesquisaFatura.TipoPropostaMultimodal != null && filtroPesquisaFatura.TipoPropostaMultimodal.Count > 0)
            {
                queryCargaPedido = queryCargaPedido.Where(obj => filtroPesquisaFatura.TipoPropostaMultimodal.Contains(obj.TipoPropostaMultimodal));

                queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

                result = result.Where(obj => queryCargaCTe.Any(o => obj.Documentos.Any(d => d.Documento.CTe == o.CTe)));
            }

            if (filtroPesquisaFatura.TiposPropostasMultimodal != null && filtroPesquisaFatura.TiposPropostasMultimodal.Count > 0)
            {
                queryCargaPedido = queryCargaPedido.Where(obj => filtroPesquisaFatura.TiposPropostasMultimodal.Contains(obj.TipoPropostaMultimodal));

                queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

                result = result.Where(obj => queryCargaCTe.Any(o => obj.Documentos.Any(d => d.Documento.CTe == o.CTe)));
            }
            if (!string.IsNullOrWhiteSpace(filtroPesquisaFatura.NumeroControleCliente))
            {
                queryPedidoXMLNotaFiscal = queryPedidoXMLNotaFiscal.Where(obj => obj.XMLNotaFiscal.NumeroControleCliente == filtroPesquisaFatura.NumeroControleCliente);

                queryCargaPedido = queryCargaPedido.Where(obj => queryPedidoXMLNotaFiscal.Any(o => o.CargaPedido == obj));

                queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

                result = result.Where(obj => queryCargaCTe.Any(o => obj.Documentos.Any(d => d.Documento.CTe == o.CTe)));
            }
            if (!string.IsNullOrWhiteSpace(filtroPesquisaFatura.NumeroReferenciaEDI))
            {
                queryPedidoXMLNotaFiscal = queryPedidoXMLNotaFiscal.Where(obj => obj.XMLNotaFiscal.NumeroReferenciaEDI == filtroPesquisaFatura.NumeroReferenciaEDI);

                queryCargaPedido = queryCargaPedido.Where(obj => queryPedidoXMLNotaFiscal.Any(o => o.CargaPedido == obj));

                queryCargaCTe = queryCargaCTe.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

                result = result.Where(obj => queryCargaCTe.Any(o => obj.Documentos.Any(d => d.Documento.CTe == o.CTe)));
            }

            if (filtroPesquisaFatura.DataVencimentoInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Parcelas.Any(p => p.DataVencimento.Date >= filtroPesquisaFatura.DataVencimentoInicial));

            if (filtroPesquisaFatura.DataVencimentoFinal != DateTime.MinValue)
                result = result.Where(obj => obj.Parcelas.Any(p => p.DataVencimento.Date <= filtroPesquisaFatura.DataVencimentoFinal));

            if (filtroPesquisaFatura.DataEmissaoInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Parcelas.Any(p => p.DataEmissao.Date >= filtroPesquisaFatura.DataEmissaoInicial));

            if (filtroPesquisaFatura.DataEmissaoFinal != DateTime.MinValue)
                result = result.Where(obj => obj.Parcelas.Any(p => p.DataEmissao.Date <= filtroPesquisaFatura.DataEmissaoFinal));

            if (filtroPesquisaFatura.TerminalDestino > 0)
                result = result.Where(obj => obj.TerminalDestino.Codigo == filtroPesquisaFatura.TerminalDestino);

            if (filtroPesquisaFatura.TerminalOrigem > 0)
                result = result.Where(obj => obj.TerminalOrigem.Codigo == filtroPesquisaFatura.TerminalOrigem);

            if (filtroPesquisaFatura.PedidoViagemNavio > 0)
                result = result.Where(obj => obj.PedidoViagemNavio.Codigo == filtroPesquisaFatura.PedidoViagemNavio);

            if (filtroPesquisaFatura.Origem > 0)
                result = result.Where(obj => obj.Origem.Codigo == filtroPesquisaFatura.Origem);

            if (filtroPesquisaFatura.Destino > 0)
                result = result.Where(obj => obj.Destino.Codigo == filtroPesquisaFatura.Destino);

            if (filtroPesquisaFatura.TipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtroPesquisaFatura.TipoOperacao);

            if (filtroPesquisaFatura.Tomador > 0)
                result = result.Where(obj => obj.ClienteTomadorFatura.CPF_CNPJ == filtroPesquisaFatura.Tomador);

            if (filtroPesquisaFatura.Empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtroPesquisaFatura.Empresa);

            if (filtroPesquisaFatura.NumeroFatura > 0)
                result = result.Where(obj => obj.Numero == filtroPesquisaFatura.NumeroFatura || obj.NumeroFaturaIntegracao == filtroPesquisaFatura.NumeroFatura);

            if (filtroPesquisaFatura.CentroDeResultado > 0)
                result = result.Where(obj => obj.CentroResultado.Codigo == filtroPesquisaFatura.CentroDeResultado);

            if (!string.IsNullOrWhiteSpace(filtroPesquisaFatura.NumeroCarga))
                result = result.Where(obj => obj.Cargas.Any(o => o.Carga.CodigoCargaEmbarcador == filtroPesquisaFatura.NumeroCarga) || obj.Documentos.Any(o => o.Documento.CTe.CargaCTes.Any(c => c.Carga.CodigoCargaEmbarcador == filtroPesquisaFatura.NumeroCarga)));

            if (filtroPesquisaFatura.Operador > 0)
                result = result.Where(obj => obj.Usuario.Codigo == filtroPesquisaFatura.Operador);

            if (filtroPesquisaFatura.DataFatura > DateTime.MinValue)
                result = result.Where(obj => obj.DataFatura.Date == filtroPesquisaFatura.DataFatura.Date);

            if ((int)filtroPesquisaFatura.Etapa > 0)
                result = result.Where(obj => obj.Etapa == filtroPesquisaFatura.Etapa);

            if (filtroPesquisaFatura.Situacao.HasValue)
            {
                if (filtroPesquisaFatura.Situacao.Value == SituacaoFatura.ProblemaIntegracao)
                {
                    IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> queryProblemaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
                    queryProblemaIntegracao = queryProblemaIntegracao.Where(c => c.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                    result = result.Where(obj => queryProblemaIntegracao.Any(c => c.Fatura.Codigo == obj.Codigo));
                }
                else
                    result = result.Where(obj => obj.Situacao == filtroPesquisaFatura.Situacao);
            }

            if (filtroPesquisaFatura.Pessoa > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == filtroPesquisaFatura.Pessoa);

            if (filtroPesquisaFatura.GrupoPessoa > 0)
                result = result.Where(obj => obj.GrupoPessoas.Codigo == filtroPesquisaFatura.GrupoPessoa || obj.Cliente.GrupoPessoas.Codigo == filtroPesquisaFatura.GrupoPessoa);

            if (filtroPesquisaFatura.NumeroPreFatura > 0)
                result = result.Where(obj => obj.NumeroPreFatura == filtroPesquisaFatura.NumeroPreFatura);

            if (filtroPesquisaFatura.FaturadoAR.HasValue)
            {
                IQueryable<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente> queryAcordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente>();
                queryAcordo = queryAcordo.Where(c => c.Status == true);
                if (filtroPesquisaFatura.FaturadoAR.Value == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                {
                    queryAcordo = queryAcordo.Where(c => c.FaturamentoPermissaoExclusivaCabotagem == true || c.FaturamentoPermissaoExclusivaCustoExtra == true || c.FaturamentoPermissaoExclusivaLongoCurso == true);
                    result = result.Where(obj => queryAcordo.Any(c => c.Pessoa.CPF_CNPJ == obj.Cliente.CPF_CNPJ || c.GrupoPessoas.Codigo == obj.GrupoPessoas.Codigo || c.GrupoPessoas.Codigo == obj.Cliente.GrupoPessoas.Codigo));
                }
                else
                {
                    queryAcordo = queryAcordo.Where(c => c.FaturamentoPermissaoExclusivaCabotagem == false && c.FaturamentoPermissaoExclusivaCustoExtra == false && c.FaturamentoPermissaoExclusivaLongoCurso == false);
                    result = result.Where(obj => queryAcordo.Any(c => c.Pessoa.CPF_CNPJ == obj.Cliente.CPF_CNPJ || c.GrupoPessoas.Codigo == obj.GrupoPessoas.Codigo || c.GrupoPessoas.Codigo == obj.Cliente.GrupoPessoas.Codigo));
                }
            }

            if (filtroPesquisaFatura.Container > 0)
                result = result.Where(o => o.Documentos.Any(f => f.Documento.CTe.Containers.Any(c => c.Container.Codigo == filtroPesquisaFatura.Container)));

            if (filtroPesquisaFatura.DataFaturaInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataFatura >= filtroPesquisaFatura.DataFaturaInicial);

            if (filtroPesquisaFatura.DataFaturaFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataFatura <= filtroPesquisaFatura.DataFaturaFinal);

            return result;
        }

        #endregion
    }

}
