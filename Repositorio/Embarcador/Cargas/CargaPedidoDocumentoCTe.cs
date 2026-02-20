using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Threading;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoDocumentoCTe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>
    {
        public CargaPedidoDocumentoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public int BuscarProximaOrdem(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            if (result.Count() > 0)
                return result.Max(obj => obj.Ordem) + 1;
            else
                return 1;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.Fetch(o => o.CTe).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe BuscarPorOrdemECargaPedido(int ordem, int codigoCargaPedido, int codigoDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            var result = from obj in query where obj.Ordem == ordem && obj.CargaPedido.Codigo == codigoCargaPedido && obj.Codigo != codigoDiferente select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe BuscarPorCTeECargaPedido(int codigoCTe, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            return result.FirstOrDefault();
        }        

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> BuscarCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            return result.OrderBy("Ordem").ToList();
        }
        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> BuscarPorCTeAsync(int codigoCTe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            return result.FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> Consultar(int cargaPedido, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Codigo == cargaPedido);

            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite)
                        .Fetch(o => o.CTe).ToList();
        }

        public int ContarConsulta(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Codigo == cargaPedido);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> BuscarPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Codigo == cargaPedido);

            return query.Fetch(o => o.CTe).ThenFetch(o => o.CentroResultadoFaturamento).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> BuscarPorCargaPedidoParaProcessamento(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido == cargaPedido);

            return query.Fetch(o => o.CTe).ThenFetch(o => o.Empresa)
                        .Fetch(o => o.CTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                        .Fetch(o => o.CTe).ThenFetch(o => o.TomadorPagador).ThenFetch(o => o.Cliente)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                        .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                        .Fetch(o => o.CTe).ThenFetch(o => o.XMLNotaFiscais)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> BuscarPorCargaPedidoParaVinculoCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido == cargaPedido);

            return query.Fetch(o => o.CTe).ThenFetch(o => o.Documentos)
                        .ToList();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> BuscarPorCargaPedidoSemFech(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Codigo == cargaPedido);

            return query.ToList();
        }

        public List<int> BuscarCodigosCTesPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Codigo == cargaPedido);

            return query.Select(o => o.CTe.Codigo).Distinct().ToList();
        }

        public bool ExistePorCargaPedidoECTe(int codigoCargaPedido, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.CTe.Codigo == codigoCTe);

            return query.Any();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExistePorCargaPedido(int cargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Codigo == cargaPedido);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExistePorCTe(IEnumerable<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => codigosCTes.Contains(o.CTe.Codigo) &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> RetornarCTesExistePorCTe(IEnumerable<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => codigosCTes.Contains(o.CTe.Codigo) &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            return query.Select(o => o.CTe).ToList();
        }

        public bool ExistePorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExistePorCTeECargaPedidoDiff(int codigoCTe, int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe &&
                                     o.CargaPedido.Codigo != codigoCargaPedido &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe BuscarPorCTeECargaPedidoDiff(List<int> codigosCTes, int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosCTes.Count / quantidadeRegistrosConsultarPorVez;

            for (int i = 0; i <= quantidadeConsultas; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = query.Where(o => o.CargaPedido.Codigo != codigoCargaPedido &&
                                                                                                                       o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                                                                                                       o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                                                                                                       codigosCTes.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CTe.Codigo)).FirstOrDefault();

                if (cargaPedidoDocumentoCTe != null)
                    return cargaPedidoDocumentoCTe;
            }

            return null;
        }

        public bool ExistePorCTeECargaPedido(int codigoCTe, int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe &&
                                     o.CargaPedido.Codigo == codigoCargaPedido &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> BuscarPorCTeECargaPedido(List<int> codigosCTes, int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosCTes.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaPedidoDocumentosCTesRetornar = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                cargaPedidoDocumentosCTesRetornar.AddRange(query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && codigosCTes.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CTe.Codigo)).ToList());

            return cargaPedidoDocumentosCTesRetornar;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public decimal ObterValorTotalMercadoria(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Codigo == cargaPedido);

            return query.Sum(o => (decimal?)o.CTe.ValorTotalMercadoria) ?? 0m;
        }

        public bool ExisteCTeNaoAutorizadoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga && o.CTe.Status != "A");

            return query.Any();
        }

        #endregion

        #region Métodos Públicos - Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume> RelatorioRelacaoSeparacaoVolume(int codigoCarga, string numeroNS, string cnpjRemetente, string numeroNota, string serie)
        {
            string query = @"SELECT DISTINCT E.EMP_RAZAO Empresa, E.EMP_CNPJ CNPJEmpresa, C.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, V.VEI_PLACA PlacaVeiculo,
				 CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(M.FUN_NOME AS NVARCHAR(2000))
	                FROM T_CARGA_MOTORISTA CM
					JOIN T_FUNCIONARIO M ON CM.CAR_MOTORISTA = M.FUN_CODIGO
	                WHERE CM.CAR_CODIGO = C.CAR_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Motoristas,
				CPD.CDC_ORDEM Sequencia,
				CAST(ISNULL(XM.NF_NUMERO, DOCS.NFC_NUMERO) AS VARCHAR(30)) NumeroNota,
				PDEST.PCT_NOME Destinatario,
				ISNULL(XM.NF_VOLUMES, DOCS.NFC_VOLUME) Volumes,
				CAST(ISNULL(XM.NF_NUMERO_ROMANEIO, DOCS.NFC_NUMERO) AS VARCHAR(30))  NumeroPedido,
				CAST(ISNULL(XM.NF_NUMERO_SOLICITACAO, DOCS.NFC_NUMERO) AS VARCHAR(30))  NumeroRemessa,
				PREST.PCT_NOME Remetente,	
				CASE
					WHEN XM.NF_NUMERO IS NULL THEN (SELECT TOP 1 R.ROT_DESCRICAO_ROTA_SEM_PARAR 
												FROM T_ROTA_CEP C
												JOIN T_ROTA R ON R.ROT_CODIGO = C.ROT_CODIGO
												WHERE CAST(REPLACE(REPLACE(PDEST.PCT_CEP, '-', ''), '.', '') AS INT) BETWEEN CAST(C.ROC_CEP_INICIAL AS INT) AND CAST(C.ROC_CEP_FINAL AS INT))
					ELSE CAST(ISNULL(XM.NF_SUB_ROTA, XM.NF_ROTA) AS VARCHAR(30))  
				END Rota,
                ISNULL(XM.NF_VALOR, DOCS.NFC_VALOR) ValorMercadorias,
                C.CAR_DATA_CRIACAO DataCarga,
                CTE.CON_CODIGO CodigoCTe
				from T_CARGA_PEDIDO_DOCUMENTO_CTE CPD
                JOIN T_CARGA_PEDIDO CP ON CP.CPE_CODIGO = CPD.CPE_CODIGO
				JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO
                JOIN T_EMPRESA E ON E.EMP_CODIGO = C.EMP_CODIGO
                JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO                
				LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = C.CAR_VEICULO				
				JOIN T_CTE CTE ON CTE.CON_CODIGO = CPD.CON_CODIGO
				JOIN T_CTE_PARTICIPANTE PDEST ON PDEST.PCT_CODIGO = CTE.CON_DESTINATARIO_CTE
				JOIN T_CTE_PARTICIPANTE PREST ON PREST.PCT_CODIGO = CTE.CON_REMETENTE_CTE
				JOIN T_CTE_DOCS DOCS ON DOCS.CON_CODIGO = CTE.CON_CODIGO
				LEFT OUTER JOIN T_XML_NOTA_FISCAL XM ON XM.NF_CHAVE = DOCS.NFC_CHAVENFE	and XM.NF_CHAVE <> '' and XM.NF_CHAVE is not null			
                WHERE 1 = 1 AND (XM.NF_VOLUMES > 0 OR DOCS.NFC_VOLUME > 0)  ";

            if (codigoCarga > 0)
                query += " AND C.CAR_CODIGO = " + codigoCarga.ToString();

            if (!string.IsNullOrEmpty(numeroNS))
                query += " AND XM.NF_NUMERO_SOLICITACAO = '" + numeroNS + "'";
            if (!string.IsNullOrEmpty(cnpjRemetente))
                query += " AND right(replicate('0',14) + convert(VARCHAR,PREST.PCT_CPF_CNPJ),14) = '" + cnpjRemetente + "'";
            if (!string.IsNullOrEmpty(numeroNota))
                query += " AND CAST(ISNULL(XM.NF_NUMERO, DOCS.NFC_NUMERO) AS VARCHAR(30)) = '" + numeroNota + "'";
            //if (!string.IsNullOrEmpty(serie))
            //    query += " AND CAST(ISNULL(XM.NF_SERIE, DOCS.NFC_SERIE) AS VARCHAR(30)) = '" + serie + "'";

            query += " ORDER BY CPD.CDC_ORDEM";



            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega> RelatorioRelacaoEntrega(int codigoCargaPedido)
        {
            string query = @"select distinct E.EMP_RAZAO NomeEmpresa,
                E.EMP_ENDERECO EnderecoEmpresa,
                E.EMP_BAIRRO BairroEmpresa,
                E.EMP_CEP CEPEmpresa,
                E.EMP_CNPJ CNPJEmpresa,
                E.EMP_INSCRICAO IEEmpresa,
                LE.LOC_DESCRICAO CidadeEmpresa,
                LE.UF_SIGLA EstadoEmpresa,
                C.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                C.CAR_CODIGO CodigoCarga,
                V.VEI_PLACA PlacaVeiculo,
                V.VEI_NUMERO_FROTA NumeroFrotaVeiculo,
                V.VEI_CAP_KG CapacidadeVeiculo,
                ISNULL((SELECT COUNT(1) FROM T_CARGA_VEICULOS_VINCULADOS CV WHERE CV.CAR_CODIGO = C.CAR_CODIGO), 0) ContemReboque,
                ISNULL((SELECT COUNT(1) FROM T_CARGA_MOTORISTA CV WHERE CV.CAR_CODIGO = C.CAR_CODIGO), 0) ContemMotoristas,
                ISNULL((SELECT COUNT(1) FROM T_CTE CTE JOIN T_CARGA_PEDIDO_DOCUMENTO_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO CCP ON CCP.CPE_CODIGO = CC.CPE_CODIGO WHERE CCP.CAR_CODIGO = C.CAR_CODIGO), 0) QtdCTe,
                ISNULL((SELECT COUNT(1) FROM T_CTE CTE JOIN T_CTE_DOCS D ON D.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO_DOCUMENTO_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO CCP ON CCP.CPE_CODIGO = CC.CPE_CODIGO WHERE CCP.CAR_CODIGO = C.CAR_CODIGO), 0) QtdNotas,
                --ISNULL((SELECT SUM(PESO.ICA_QTD) FROM T_CTE CTE JOIN T_CTE_DOCS D ON D.CON_CODIGO = CTE.CON_CODIGO JOIN T_CTE_INF_CARGA PESO ON PESO.CON_CODIGO = CTE.CON_CODIGO AND PESO.ICA_UN = '03' JOIN T_CARGA_PEDIDO_DOCUMENTO_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO CCP ON CCP.CPE_CODIGO = CC.CPE_CODIGO WHERE CCP.CAR_CODIGO = C.CAR_CODIGO), 0) QtdVolumes, 
                ISNULL((SELECT SUM(COS_QUANTIDADE) FROM T_CARGA_CONTROLE_EXPEDICAO E JOIN T_CONFERENCIA_SEPARACAO S ON S.CCX_CODIGO = E.CCX_CODIGO AND E.CAR_CODIGO = C.CAR_CODIGO), 0) QtdVolumes, 
                ISNULL((SELECT SUM(PESO.ICA_QTD) FROM T_CTE CTE JOIN T_CTE_DOCS D ON D.CON_CODIGO = CTE.CON_CODIGO JOIN T_CTE_INF_CARGA PESO ON PESO.CON_CODIGO = CTE.CON_CODIGO AND PESO.ICA_UN = '01' JOIN T_CARGA_PEDIDO_DOCUMENTO_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO CCP ON CCP.CPE_CODIGO = CC.CPE_CODIGO WHERE CCP.CAR_CODIGO = C.CAR_CODIGO), 0) QtdPeso,
                ISNULL((SELECT SUM(D.NFC_VALOR) FROM T_CTE CTE JOIN T_CTE_DOCS D ON D.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO_DOCUMENTO_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO CCP ON CCP.CPE_CODIGO = CC.CPE_CODIGO WHERE CCP.CAR_CODIGO = C.CAR_CODIGO), 0) ValorNotas,
                ISNULL((SELECT SUM(CTE.CON_VALOR_FRETE) FROM T_CTE CTE JOIN T_CARGA_PEDIDO_DOCUMENTO_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO CCP ON CCP.CPE_CODIGO = CC.CPE_CODIGO WHERE CCP.CAR_CODIGO = C.CAR_CODIGO), 0) ValorFreteSemICMS,
                ISNULL(PROP.CLI_CGCCPF, 0) CNPJProprietario, PROP.CLI_NOME NomeProprietario, E.EMP_REGISTROANTT ANTTEmpresa, '' CIOT, '' Rota, ISNULL(C.CAR_DATA_FINALIZACAO_EMISSAO, GETDATE()) DataFinalizacaoEmissao,
                CO.CLI_NOME NomeRemetente,
                CO.CLI_ENDERECO EnderecoRemetente,
                CO.CLI_BAIRRO BairroRemetente,
                CO.CLI_CEP CEPRemetente,
                CO.CLI_CGCCPF CNPJRemetente,
                CO.CLI_IERG IERemetente,
                LC.LOC_DESCRICAO CidadeRemetente,
                LC.UF_SIGLA EstadoRemetente
                from T_CARGA_PEDIDO_DOCUMENTO_CTE CPD
                JOIN T_CARGA_PEDIDO CP ON CP.CPE_CODIGO = CPD.CPE_CODIGO
                JOIN T_PEDIDO P ON P.PED_CODIGO = CP.PED_CODIGO
				LEFT OUTER JOIN T_CLIENTE CO ON CO.CLI_CGCCPF = P.CLI_CODIGO_REMETENTE
				LEFT OUTER JOIN T_LOCALIDADES LC ON LC.LOC_CODIGO = CO.LOC_CODIGO
                JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO
                JOIN T_EMPRESA E ON E.EMP_CODIGO = C.EMP_CODIGO
                JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                JOIN T_VEICULO V ON V.VEI_CODIGO = C.CAR_VEICULO
                LEFT OUTER JOIN T_CLIENTE PROP ON PROP.CLI_CGCCPF = V.VEI_PROPRIETARIO
                WHERE 1 = 1";
            query += " AND CPD.CPE_CODIGO = " + codigoCargaPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega> RelatorioRelacaoEntregaCarga(int codigoCarga)
        {
            string query = @"select distinct E.EMP_RAZAO NomeEmpresa,
                E.EMP_ENDERECO EnderecoEmpresa,
                E.EMP_BAIRRO BairroEmpresa,
                E.EMP_CEP CEPEmpresa,
                E.EMP_CNPJ CNPJEmpresa,
                E.EMP_INSCRICAO IEEmpresa,
                LE.LOC_DESCRICAO CidadeEmpresa,
                LE.UF_SIGLA EstadoEmpresa,
                C.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                C.CAR_CODIGO CodigoCarga,
                V.VEI_PLACA PlacaVeiculo,
                V.VEI_NUMERO_FROTA NumeroFrotaVeiculo,
                V.VEI_CAP_KG CapacidadeVeiculo,
                ISNULL((SELECT COUNT(1) FROM T_CARGA_VEICULOS_VINCULADOS CV WHERE CV.CAR_CODIGO = C.CAR_CODIGO), 0) ContemReboque,
                ISNULL((SELECT COUNT(1) FROM T_CARGA_MOTORISTA CV WHERE CV.CAR_CODIGO = C.CAR_CODIGO), 0) ContemMotoristas,
                ISNULL((SELECT COUNT(1) FROM T_CTE CTE JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO CCP ON CCP.CAR_CODIGO = CC.CAR_CODIGO WHERE CCP.CAR_CODIGO = C.CAR_CODIGO), 0) QtdCTe,
                ISNULL((SELECT COUNT(1) FROM T_CTE CTE JOIN T_CTE_DOCS D ON D.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO CCP ON CCP.CAR_CODIGO = CC.CAR_CODIGO WHERE CCP.CAR_CODIGO = C.CAR_CODIGO), 0) QtdNotas,                
                ISNULL((SELECT SUM(PESO.ICA_QTD) FROM T_CTE CTE JOIN T_CTE_INF_CARGA PESO ON PESO.CON_CODIGO = CTE.CON_CODIGO AND PESO.ICA_UN = '03' JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO WHERE CC.CAR_CODIGO = C.CAR_CODIGO), 0) QtdVolumes, 
                ISNULL((SELECT SUM(PESO.ICA_QTD) FROM T_CTE CTE JOIN T_CTE_INF_CARGA PESO ON PESO.CON_CODIGO = CTE.CON_CODIGO AND PESO.ICA_UN = '01' JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO WHERE CC.CAR_CODIGO = C.CAR_CODIGO), 0) QtdPeso,
                ISNULL((SELECT SUM(D.NFC_VALOR) FROM T_CTE CTE JOIN T_CTE_DOCS D ON D.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO CCP ON CCP.CAR_CODIGO = CC.CAR_CODIGO WHERE CCP.CAR_CODIGO = C.CAR_CODIGO), 0) ValorNotas,
                ISNULL((SELECT SUM(CTE.CON_VALOR_FRETE) FROM T_CTE CTE JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = CTE.CON_CODIGO JOIN T_CARGA_PEDIDO CCP ON CCP.CAR_CODIGO = CC.CAR_CODIGO WHERE CCP.CAR_CODIGO = C.CAR_CODIGO), 0) ValorFreteSemICMS,
                ISNULL(PROP.CLI_CGCCPF, 0) CNPJProprietario, PROP.CLI_NOME NomeProprietario, E.EMP_REGISTROANTT ANTTEmpresa, '' CIOT, '' Rota, ISNULL(C.CAR_DATA_FINALIZACAO_EMISSAO, GETDATE()) DataFinalizacaoEmissao,
                CO.CLI_NOME NomeRemetente,
                CO.CLI_ENDERECO EnderecoRemetente,
                CO.CLI_BAIRRO BairroRemetente,
                CO.CLI_CEP CEPRemetente,
                CO.CLI_CGCCPF CNPJRemetente,
                CO.CLI_IERG IERemetente,
                LC.LOC_DESCRICAO CidadeRemetente,
                LC.UF_SIGLA EstadoRemetente
                from T_CARGA_CTE CPD
                JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = CPD.CAR_CODIGO
                JOIN T_PEDIDO P ON P.PED_CODIGO = CP.PED_CODIGO
				LEFT OUTER JOIN T_CLIENTE CO ON CO.CLI_CGCCPF = P.CLI_CODIGO_REMETENTE
				LEFT OUTER JOIN T_LOCALIDADES LC ON LC.LOC_CODIGO = CO.LOC_CODIGO
                JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO
                JOIN T_EMPRESA E ON E.EMP_CODIGO = C.EMP_CODIGO
                JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                JOIN T_VEICULO V ON V.VEI_CODIGO = C.CAR_VEICULO
                LEFT OUTER JOIN T_CLIENTE PROP ON PROP.CLI_CGCCPF = V.VEI_PROPRIETARIO
                WHERE 1 = 1";
            query += " AND CPD.CAR_CODIGO = " + codigoCarga.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaCargaPedido> RelatorioRelacaoEntregaCargaPedido(int codigoCarga)
        {
            string query = $@"select distinct E.EMP_RAZAO NomeEmpresa,
                E.EMP_ENDERECO EnderecoEmpresa,
                E.EMP_BAIRRO BairroEmpresa,
                E.EMP_CEP CEPEmpresa,
                E.EMP_CNPJ CNPJEmpresa,
                E.EMP_INSCRICAO IEEmpresa,
                LE.LOC_DESCRICAO CidadeEmpresa,
                LE.UF_SIGLA EstadoEmpresa,
                C.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                C.CAR_CODIGO CodigoCarga,
                V.VEI_PLACA PlacaVeiculo,
                V.VEI_NUMERO_FROTA NumeroFrotaVeiculo,
                V.VEI_CAP_KG CapacidadeVeiculo,
                (SELECT SUM(NF_VALOR_TOTAL_PRODUTOS) FROM T_XML_NOTA_FISCAL XML 
                    JOIN T_PEDIDO_XML_NOTA_FISCAL PX ON PX.NFX_CODIGO = XML.NFX_CODIGO 
                    WHERE PX.CPE_CODIGO = CP.CPE_CODIGO) ValorNotas, 
                C.CAR_VALOR_FRETE ValorFreteSemICMS,
                ISNULL(PROP.CLI_CGCCPF, 0) CNPJProprietario, PROP.CLI_NOME NomeProprietario, E.EMP_REGISTROANTT ANTTEmpresa, '' Rota, 
                C.CAR_DATA_FINALIZACAO_EMISSAO DataFinalizacaoEmissao,
                CO.CLI_NOME NomeRemetente,
                CO.CLI_ENDERECO EnderecoRemetente,
                CO.CLI_BAIRRO BairroRemetente,
                CO.CLI_CEP CEPRemetente,
                CO.CLI_CGCCPF CNPJRemetente,
                CO.CLI_IERG IERemetente,
                LC.LOC_DESCRICAO CidadeRemetente,
                LC.UF_SIGLA EstadoRemetente,

                --Dados dos Documentos

                Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroCTe, 
                ISNULL(RECEB.CLI_NOME, DEST.CLI_NOME) NomeDestinatario, 
                Pedido.PED_PESO_TOTAL_CARGA QtdPeso,                 
                (
			                SELECT CAST(SUM(NF_VOLUMES) AS DECIMAL(18,2))
					          FROM T_XML_NOTA_FISCAL _xmlNotaFiscal
					          JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoXmlNotaFiscal 
					            ON _xmlNotaFiscal.NFX_CODIGO = _pedidoXmlNotaFiscal.NFX_CODIGO
					          JOIN T_CARGA_PEDIDO _cargaPedido
					            ON _cargaPedido.CPE_CODIGO = _pedidoXmlNotaFiscal.CPE_CODIGO
					          JOIN T_CARGA _carga
					            ON _carga.CAR_CODIGO = _cargaPedido.CAR_CODIGO
					          JOIN T_PEDIDO _pedido
					            ON _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
					          WHERE _xmlNotaFiscal.NF_ATIVA = 1
					            AND _pedido.CLI_CODIGO = DEST.CLI_CGCCPF
                                AND _cargaPedido.CPE_CODIGO = CP.CPE_CODIGO
                                
		        ) QtdVolume, 

                SUBSTRING((SELECT DISTINCT ', ' + CAST(xml.NF_NUMERO AS NVARCHAR(2000))
	                FROM T_XML_NOTA_FISCAL xml 
                    JOIN T_PEDIDO_XML_NOTA_FISCAL pedidoXML ON pedidoXML.NFX_CODIGO = xml.NFX_CODIGO
	                WHERE pedidoXML.CPE_CODIGO = CP.CPE_CODIGO AND xml.NF_ATIVA = 1 AND xml.NF_CLASSIFICACAO_NFE = 1 FOR XML PATH('')), 3, 2000) NotasRevenda,
                SUBSTRING((SELECT DISTINCT ', ' + CAST(xml.NF_NUMERO AS NVARCHAR(2000))
	                FROM T_XML_NOTA_FISCAL xml 
                    JOIN T_PEDIDO_XML_NOTA_FISCAL pedidoXML ON pedidoXML.NFX_CODIGO = xml.NFX_CODIGO
	                WHERE pedidoXML.CPE_CODIGO = CP.CPE_CODIGO AND xml.NF_ATIVA = 1 AND xml.NF_CLASSIFICACAO_NFE = 2 FOR XML PATH('')), 3, 2000) NotasNaoRevenda,
                SUBSTRING((SELECT DISTINCT ', ' + CAST(xml.NF_NUMERO AS NVARCHAR(2000))
	                FROM T_XML_NOTA_FISCAL xml 
                    JOIN T_PEDIDO_XML_NOTA_FISCAL pedidoXML ON pedidoXML.NFX_CODIGO = xml.NFX_CODIGO
	                WHERE pedidoXML.CPE_CODIGO = CP.CPE_CODIGO AND xml.NF_ATIVA = 1 AND xml.NF_CLASSIFICACAO_NFE = 3 FOR XML PATH('')), 3, 2000) NotasNFEletronicos,

                (SELECT COUNT(1) FROM T_XML_NOTA_FISCAL xml 
                    JOIN T_PEDIDO_XML_NOTA_FISCAL pedidoXML ON pedidoXML.NFX_CODIGO = xml.NFX_CODIGO
	                WHERE pedidoXML.CPE_CODIGO = CP.CPE_CODIGO AND xml.NF_ATIVA = 1 AND xml.NF_CLASSIFICACAO_NFE = 1) QtdNotasRevenda,
                (SELECT COUNT(1) FROM T_XML_NOTA_FISCAL xml 
                    JOIN T_PEDIDO_XML_NOTA_FISCAL pedidoXML ON pedidoXML.NFX_CODIGO = xml.NFX_CODIGO
	                WHERE pedidoXML.CPE_CODIGO = CP.CPE_CODIGO AND xml.NF_ATIVA = 1 AND xml.NF_CLASSIFICACAO_NFE = 2) QtdNotasNaoRevenda,
                (SELECT COUNT(1) FROM T_XML_NOTA_FISCAL xml 
                    JOIN T_PEDIDO_XML_NOTA_FISCAL pedidoXML ON pedidoXML.NFX_CODIGO = xml.NFX_CODIGO
	                WHERE pedidoXML.CPE_CODIGO = CP.CPE_CODIGO AND xml.NF_ATIVA = 1 AND xml.NF_CLASSIFICACAO_NFE = 3) QtdNotasNFEletronicos,

                ISNULL(RECEB.CLI_ENDERECO, DEST.CLI_ENDERECO) EnderecoDestinatario, 
                ISNULL(RECEB.CLI_BAIRRO, DEST.CLI_BAIRRO) BairroDestinatario, 
                ISNULL(RECEB.CLI_NUMERO, DEST.CLI_NUMERO) NumeroDestinatario, 
                ISNULL(RECEB.CLI_COMPLEMENTO, DEST.CLI_COMPLEMENTO) ComplementoDestinatario, 
                CASE WHEN RECEB.CLI_CEP IS NULL OR RECEB.CLI_CEP = '0' THEN DEST.CLI_CEP ELSE RECEB.CLI_CEP END CEPDestinatario, 
                ISNULL(LRE.LOC_DESCRICAO, LD.LOC_DESCRICAO) CidadeDestinatario, 
                ISNULL(LRE.UF_SIGLA, LD.UF_SIGLA) EstadoDestinatario, 
                ISNULL(RECEB.CLI_FONE, DEST.CLI_FONE) FoneDestinatario, 
				Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS ValorMercadoria,
				Pedido.PED_VALOR_FRETE_RECEBER ValorFrete,

                SUBSTRING((SELECT DISTINCT ', ' +  (motorista.FUN_NOME + ' (' + motorista.FUN_CPF + ')')
                    from T_FUNCIONARIO motorista
                    JOIN T_CARGA_MOTORISTA cargaMotorista ON cargaMotorista.CAR_MOTORISTA = motorista.FUN_CODIGO
                    WHERE cargaMotorista.CAR_CODIGO = C.CAR_CODIGO FOR XML PATH('')), 3, 2000) Motoristas,

                SUBSTRING((SELECT DISTINCT ', ' + carga.CAR_CODIGO_CARGA_EMBARCADOR
	                FROM T_CARGA carga 
                    LEFT OUTER JOIN T_CARGA cargaAgrupamento on carga.CAR_CODIGO_AGRUPAMENTO = cargaAgrupamento.CAR_CODIGO
                    LEFT OUTER JOIN T_FILIAL filial on filial.FIL_CODIGO = carga.FIL_CODIGO_ORIGEM
	                WHERE cargaAgrupamento.CAR_CODIGO = {codigoCarga} FOR XML PATH('')), 3, 2000) CargasAgrupadas,

                SUBSTRING((SELECT DISTINCT ', ' + Pedido.PED_NUMERO_PEDIDO_EMBARCADOR
	                FROM T_CARGA_PEDIDO CP 
                    LEFT OUTER JOIN T_CARGA C on C.CAR_CODIGO = CP.CAR_CODIGO
                    LEFT OUTER JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CP.PED_CODIGO
	                WHERE C.CAR_CODIGO = {codigoCarga} FOR XML PATH('')), 3, 2000) NumeroPedido

                from T_CARGA_PEDIDO CP                
                JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CP.PED_CODIGO
				LEFT OUTER JOIN T_CLIENTE CO ON CO.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
				LEFT OUTER JOIN T_LOCALIDADES LC ON LC.LOC_CODIGO = CO.LOC_CODIGO
                JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO				
                JOIN T_EMPRESA E ON E.EMP_CODIGO = C.EMP_CODIGO
                JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                JOIN T_VEICULO V ON V.VEI_CODIGO = C.CAR_VEICULO
				JOIN T_CARGA_DADOS_SUMARIZADOS CDS ON CDS.CDS_CODIGO = C.CDS_CODIGO
                LEFT OUTER JOIN T_CLIENTE PROP ON PROP.CLI_CGCCPF = V.VEI_PROPRIETARIO
                LEFT OUTER JOIN T_CLIENTE DEST ON DEST.CLI_CGCCPF = Pedido.CLI_CODIGO
                LEFT OUTER JOIN T_LOCALIDADES LD ON LD.LOC_CODIGO = DEST.LOC_CODIGO
                LEFT OUTER JOIN T_CLIENTE RECEB ON RECEB.CLI_CGCCPF = Pedido.CLI_CODIGO_RECEBEDOR
                LEFT OUTER JOIN T_LOCALIDADES LRE ON LRE.LOC_CODIGO = RECEB.LOC_CODIGO
                WHERE CP.CAR_CODIGO = " + codigoCarga.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaCargaPedido)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaCargaPedido>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento> RelatorioRelacaoEntregaDocumento(int codigoCargaPedido)
        {
            string query = @"select distinct C.CAR_CODIGO CodigoCarga, 
                C.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, 
                CAST(CTE.CON_NUM AS VARCHAR(20)) NumeroCTe, 
                CTE.CON_CODIGO CodigoCTe,
                REM.PCT_NOME NomeRemetente, 
                ISNULL(RECEB.CLI_NOME, DEST.PCT_NOME) NomeDestinatario, 
                ISNULL((SELECT SUM(PESO.ICA_QTD) FROM T_CTE_INF_CARGA PESO WHERE PESO.CON_CODIGO = CTE.CON_CODIGO AND PESO.ICA_UN = '01'), 0) QtdPeso, 
                --ISNULL((SELECT SUM(PESO.ICA_QTD) FROM T_CTE_INF_CARGA PESO WHERE PESO.CON_CODIGO = CTE.CON_CODIGO AND PESO.ICA_UN = '03'), 0) QtdVolume, 
                ISNULL((SELECT SUM(COS_QUANTIDADE) FROM T_CARGA_CONTROLE_EXPEDICAO E
				    JOIN T_CONFERENCIA_SEPARACAO S ON S.CCX_CODIGO = E.CCX_CODIGO AND E.CAR_CODIGO = C.CAR_CODIGO
				    WHERE S.COS_CODIGO_BARRAS LIKE 
				    CASE
					    WHEN X.NF_NUMERO_SOLICITACAO IS NULL OR X.NF_NUMERO_SOLICITACAO = '' THEN REPLICATE('0', 14 - LEN(REM.PCT_CPF_CNPJ)) + RTrim(REM.PCT_CPF_CNPJ) + REPLICATE('0', 9 - LEN(X.NF_NUMERO)) + RTrim(X.NF_NUMERO) + '%'
					    ELSE X.NF_NUMERO_SOLICITACAO + '%'
				    END), 0) QtdVolume,
                CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(D.NFC_NUMERO AS NVARCHAR(2000))
	                FROM T_CTE_DOCS D
	                WHERE D.CON_CODIGO = CTE.CON_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Notas,
                ISNULL(RECEB.CLI_ENDERECO, DEST.PCT_ENDERECO) EnderecoDestinatario, 
                ISNULL(RECEB.CLI_BAIRRO, DEST.PCT_BAIRRO) BairroDestinatario, 
                ISNULL(RECEB.CLI_NUMERO, DEST.PCT_NUMERO) NumeroDestinatario, 
                ISNULL(RECEB.CLI_COMPLEMENTO, DEST.PCT_COMPLEMENTO) ComplementoDestinatario, 
                CASE WHEN RECEB.CLI_CEP IS NULL OR RECEB.CLI_CEP = '0' THEN DEST.PCT_CEP ELSE RECEB.CLI_CEP END CEPDestinatario, 
                ISNULL(LRE.LOC_DESCRICAO, LD.LOC_DESCRICAO) CidadeDestinatario, 
                ISNULL(LRE.UF_SIGLA, LD.UF_SIGLA) EstadoDestinatario, 
                ISNULL(RECEB.CLI_FONE, DEST.PCT_FONE) FoneDestinatario, CPD.CDC_ORDEM Ordem, 
				CASE
					WHEN RECEB.CLI_CGCCPF IS NOT NULL THEN 'C.F. - *** RECEBEDOR ***'
					WHEN X.NF_GRAU_RISCO = 1 THEN 'G1 – ENTREGA NORMAL'
					WHEN X.NF_GRAU_RISCO = 2 THEN 'G3 – CONFERÊNCIA OBRIGATORIA'
					WHEN X.NF_GRAU_RISCO = 3 THEN 'G3 – CONFERÊNCIA OBRIGATORIA'
					ELSE ''
				END Instrucao, 
				CASE
					WHEN X.NF_SUB_ROTA IS NULL OR X.NF_SUB_ROTA = '' THEN X.NF_ROTA
					ELSE X.NF_SUB_ROTA
				END Setor,
                (SELECT COUNT(1)
	                FROM T_CTE_DOCS D
	                WHERE D.CON_CODIGO = CTE.CON_CODIGO) QtdNF,
				CTE.CON_VALOR_TOTAL_MERC ValorMercadoria,
				CTE.CON_VALOR_RECEBER ValorFrete,
				'' EmpresaFilial	
                from T_CARGA_PEDIDO_DOCUMENTO_CTE CPD
                JOIN T_CARGA_PEDIDO CP ON CP.CPE_CODIGO = CPD.CPE_CODIGO
                JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO
                JOIN T_CTE CTE ON CTE.CON_CODIGO = CPD.CON_CODIGO
                JOIN T_CTE_PARTICIPANTE REM ON REM.PCT_CODIGO = CTE.CON_REMETENTE_CTE
                JOIN T_CTE_PARTICIPANTE DEST ON DEST.PCT_CODIGO = CTE.CON_DESTINATARIO_CTE
                JOIN T_LOCALIDADES LR ON LR.LOC_CODIGO = REM.LOC_CODIGO
                JOIN T_LOCALIDADES LD ON LD.LOC_CODIGO = DEST.LOC_CODIGO
                JOIN T_VEICULO V ON V.VEI_CODIGO = C.CAR_VEICULO
                LEFT OUTER JOIN T_CARGA_VEICULOS_VINCULADOS CVV ON CVV.CAR_CODIGO = C.CAR_CODIGO
                LEFT OUTER JOIN T_VEICULO VR ON VR.VEI_CODIGO = CVV.VEI_CODIGO
                LEFT OUTER JOIN T_CLIENTE PROP ON PROP.CLI_CGCCPF = V.VEI_PROPRIETARIO
				JOIN T_CTE_XML_NOTAS_FISCAIS XN ON XN.CON_CODIGO = CTE.CON_CODIGO
				JOIN T_XML_NOTA_FISCAL X ON X.NFX_CODIGO = XN.NFX_CODIGO
				LEFT OUTER JOIN T_CLIENTE RECEB ON RECEB.CLI_CGCCPF = X.CLI_CODIGO_RECEBEDOR
				LEFT OUTER JOIN T_LOCALIDADES LRE ON LRE.LOC_CODIGO = RECEB.LOC_CODIGO
                WHERE 1 = 1";

            query += " AND CPD.CPE_CODIGO = " + codigoCargaPedido.ToString();
            query += " ORDER BY CPD.CDC_ORDEM ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento> RelatorioRelacaoEntregaDocumentoCarga(int codigoCarga)
        {
            string query = @"select distinct C.CAR_CODIGO CodigoCarga, 
                C.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, 
                CAST(CTE.CON_NUM AS VARCHAR(20)) NumeroCTe, 
                REM.PCT_NOME NomeRemetente, 
                ISNULL(RECEB.CLI_NOME, DEST.PCT_NOME) NomeDestinatario, 
                ISNULL((SELECT SUM(PESO.ICA_QTD) FROM T_CTE_INF_CARGA PESO WHERE PESO.CON_CODIGO = CTE.CON_CODIGO AND PESO.ICA_UN = '01'), 0) QtdPeso,                 
				ISNULL((SELECT SUM(PESO.ICA_QTD) FROM T_CTE_INF_CARGA PESO WHERE PESO.CON_CODIGO = CTE.CON_CODIGO AND PESO.ICA_UN = '03'), 0) QtdVolume,                
                CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(D.NFC_NUMERO AS NVARCHAR(2000))
	                FROM T_CTE_DOCS D
	                WHERE D.CON_CODIGO = CTE.CON_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Notas,				 
                ISNULL(RECEB.CLI_ENDERECO, DEST.PCT_ENDERECO) EnderecoDestinatario, 
                ISNULL(RECEB.CLI_BAIRRO, DEST.PCT_BAIRRO) BairroDestinatario, 
                ISNULL(RECEB.CLI_NUMERO, DEST.PCT_NUMERO) NumeroDestinatario, 
                ISNULL(RECEB.CLI_COMPLEMENTO, DEST.PCT_COMPLEMENTO) ComplementoDestinatario, 
                CASE WHEN RECEB.CLI_CEP IS NULL OR RECEB.CLI_CEP = '0' THEN DEST.PCT_CEP ELSE RECEB.CLI_CEP END CEPDestinatario, 
                ISNULL(LRE.LOC_DESCRICAO, LD.LOC_DESCRICAO) CidadeDestinatario, 
                ISNULL(LRE.UF_SIGLA, LD.UF_SIGLA) EstadoDestinatario, 
                ISNULL(RECEB.CLI_FONE, DEST.PCT_FONE) FoneDestinatario, 
				0 Ordem, 
				CASE
					WHEN RECEB.CLI_CGCCPF IS NOT NULL THEN 'C.F. - *** RECEBEDOR ***'
					WHEN X.NF_GRAU_RISCO = 1 THEN 'G1 – ENTREGA NORMAL'
					WHEN X.NF_GRAU_RISCO = 2 THEN 'G3 – CONFERÊNCIA OBRIGATORIA'
					WHEN X.NF_GRAU_RISCO = 3 THEN 'G3 – CONFERÊNCIA OBRIGATORIA'
					ELSE ''
				END Instrucao, 
				CASE
					WHEN X.NF_SUB_ROTA IS NULL OR X.NF_SUB_ROTA = '' THEN X.NF_ROTA
					ELSE X.NF_SUB_ROTA
				END Setor,
				(SELECT COUNT(1)
	                FROM T_CTE_DOCS D
	                WHERE D.CON_CODIGO = CTE.CON_CODIGO) QtdNF,
				CTE.CON_VALOR_TOTAL_MERC ValorMercadoria,
				CTE.CON_VALOR_RECEBER ValorFrete,
				'' EmpresaFilial		
                from T_CARGA_CTE CPD
                JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = CPD.CAR_CODIGO
                JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO
                JOIN T_CTE CTE ON CTE.CON_CODIGO = CPD.CON_CODIGO
                JOIN T_CTE_PARTICIPANTE REM ON REM.PCT_CODIGO = CTE.CON_REMETENTE_CTE
                JOIN T_CTE_PARTICIPANTE DEST ON DEST.PCT_CODIGO = CTE.CON_DESTINATARIO_CTE
                JOIN T_LOCALIDADES LR ON LR.LOC_CODIGO = REM.LOC_CODIGO
                JOIN T_LOCALIDADES LD ON LD.LOC_CODIGO = DEST.LOC_CODIGO
                JOIN T_VEICULO V ON V.VEI_CODIGO = C.CAR_VEICULO
                LEFT OUTER JOIN T_CARGA_VEICULOS_VINCULADOS CVV ON CVV.CAR_CODIGO = C.CAR_CODIGO
                LEFT OUTER JOIN T_VEICULO VR ON VR.VEI_CODIGO = CVV.VEI_CODIGO
                LEFT OUTER JOIN T_CLIENTE PROP ON PROP.CLI_CGCCPF = V.VEI_PROPRIETARIO
				LEFT OUTER JOIN T_CTE_XML_NOTAS_FISCAIS XN ON XN.CON_CODIGO = CTE.CON_CODIGO
				LEFT OUTER JOIN T_XML_NOTA_FISCAL X ON X.NFX_CODIGO = XN.NFX_CODIGO
				LEFT OUTER JOIN T_CLIENTE RECEB ON RECEB.CLI_CGCCPF = X.CLI_CODIGO_RECEBEDOR
				LEFT OUTER JOIN T_LOCALIDADES LRE ON LRE.LOC_CODIGO = RECEB.LOC_CODIGO
                WHERE 1 = 1";

            query += " AND CP.CAR_CODIGO = " + codigoCarga.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento> RelatorioRelacaoEntregaDocumentoPedidoCarga(int codigoCarga)
        {
            string query = @"select distinct C.CAR_CODIGO CodigoCarga, 
                C.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, 
                Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroCTe, 
                REM.CLI_NOME NomeRemetente, 
                ISNULL(RECEB.CLI_NOME, DEST.CLI_NOME) NomeDestinatario, 
                Pedido.PED_PESO_TOTAL_CARGA QtdPeso,                 
				CAST(Pedido.PED_QUANTIDADE_VOLUMES AS DECIMAL(18, 2)) QtdVolume,                
                CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(xml.NF_NUMERO AS NVARCHAR(2000))
	                FROM T_XML_NOTA_FISCAL xml 
                    JOIN T_PEDIDO_XML_NOTA_FISCAL pedidoXML ON pedidoXML.NFX_CODIGO = xml.NFX_CODIGO
	                WHERE pedidoXML.CPE_CODIGO = CP.CPE_CODIGO AND xml.NF_ATIVA = 1 FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Notas,				 
                ISNULL(RECEB.CLI_ENDERECO, DEST.CLI_ENDERECO) EnderecoDestinatario, 
                ISNULL(RECEB.CLI_BAIRRO, DEST.CLI_BAIRRO) BairroDestinatario, 
                ISNULL(RECEB.CLI_NUMERO, DEST.CLI_NUMERO) NumeroDestinatario, 
                ISNULL(RECEB.CLI_COMPLEMENTO, DEST.CLI_COMPLEMENTO) ComplementoDestinatario, 
                CASE WHEN RECEB.CLI_CEP IS NULL OR RECEB.CLI_CEP = '0' THEN DEST.CLI_CEP ELSE RECEB.CLI_CEP END CEPDestinatario, 
                ISNULL(LRE.LOC_DESCRICAO, LD.LOC_DESCRICAO) CidadeDestinatario, 
                ISNULL(LRE.UF_SIGLA, LD.UF_SIGLA) EstadoDestinatario, 
                ISNULL(RECEB.CLI_FONE, DEST.CLI_FONE) FoneDestinatario, 
				0 Ordem, 
				'' Instrucao, 
				'' Setor,
				(SELECT COUNT(1)
	                FROM T_XML_NOTA_FISCAL xml 
                    JOIN T_PEDIDO_XML_NOTA_FISCAL pedidoXML ON pedidoXML.NFX_CODIGO = xml.NFX_CODIGO
	                WHERE pedidoXML.CPE_CODIGO = CP.CPE_CODIGO AND xml.NF_ATIVA = 1) QtdNF,
				Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS ValorMercadoria,
				Pedido.PED_VALOR_FRETE_RECEBER ValorFrete,
				'' EmpresaFilial
                from T_CARGA_PEDIDO CP
                JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO
                JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CP.PED_CODIGO
                JOIN T_CLIENTE REM ON REM.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
                JOIN T_CLIENTE DEST ON DEST.CLI_CGCCPF = Pedido.CLI_CODIGO
                JOIN T_LOCALIDADES LR ON LR.LOC_CODIGO = REM.LOC_CODIGO
                JOIN T_LOCALIDADES LD ON LD.LOC_CODIGO = DEST.LOC_CODIGO
                JOIN T_VEICULO V ON V.VEI_CODIGO = C.CAR_VEICULO
                LEFT OUTER JOIN T_CARGA_VEICULOS_VINCULADOS CVV ON CVV.CAR_CODIGO = C.CAR_CODIGO
                LEFT OUTER JOIN T_VEICULO VR ON VR.VEI_CODIGO = CVV.VEI_CODIGO
                LEFT OUTER JOIN T_CLIENTE PROP ON PROP.CLI_CGCCPF = V.VEI_PROPRIETARIO
				LEFT OUTER JOIN T_CLIENTE RECEB ON RECEB.CLI_CGCCPF = Pedido.CLI_CODIGO_RECEBEDOR
				LEFT OUTER JOIN T_LOCALIDADES LRE ON LRE.LOC_CODIGO = RECEB.LOC_CODIGO
                WHERE CP.CAR_CODIGO = " + codigoCarga.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista> RelatorioRelacaoEntregaMotorista(int codigoCargaPedido)
        {
            string query = @"select distinct M.FUN_NOME Nome, M.FUN_CPF CPF, C.CAR_CODIGO CodigoCarga
                from T_CARGA_PEDIDO_DOCUMENTO_CTE CPD
                JOIN T_CARGA_PEDIDO CP ON CP.CPE_CODIGO = CPD.CPE_CODIGO
                JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO
                JOIN T_CARGA_MOTORISTA CM ON CM.CAR_CODIGO = C.CAR_CODIGO
                JOIN T_FUNCIONARIO M ON M.FUN_CODIGO = CM.CAR_MOTORISTA
                WHERE 1 = 1";
            query += " AND CPD.CPE_CODIGO = " + codigoCargaPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista> RelatorioRelacaoEntregaMotoristaCarga(int codigoCarga)
        {
            string query = @"select distinct M.FUN_NOME Nome, M.FUN_CPF CPF, C.CAR_CODIGO CodigoCarga
                from T_CARGA_CTE CPD
                JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = CPD.CAR_CODIGO
                JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO
                JOIN T_CARGA_MOTORISTA CM ON CM.CAR_CODIGO = C.CAR_CODIGO
                JOIN T_FUNCIONARIO M ON M.FUN_CODIGO = CM.CAR_MOTORISTA
                WHERE 1 = 1";
            query += " AND CPD.CAR_CODIGO = " + codigoCarga.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque> RelatorioRelacaoEntregaReboque(int codigoCargaPedido)
        {
            string query = @"select distinct VR.VEI_PLACA Placa, VR.VEI_NUMERO_FROTA NumeroFrota, C.CAR_CODIGO CodigoCarga
                from T_CARGA_PEDIDO_DOCUMENTO_CTE CPD
                JOIN T_CARGA_PEDIDO CP ON CP.CPE_CODIGO = CPD.CPE_CODIGO
                JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO
                JOIN T_CARGA_VEICULOS_VINCULADOS CVV ON CVV.CAR_CODIGO = C.CAR_CODIGO
                JOIN T_VEICULO VR ON VR.VEI_CODIGO = CVV.VEI_CODIGO
                WHERE 1 = 1";
            query += " AND CPD.CPE_CODIGO = " + codigoCargaPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque> RelatorioRelacaoEntregaReboqueCarga(int codigoCarga)
        {
            string query = @"select distinct VR.VEI_PLACA Placa, VR.VEI_NUMERO_FROTA NumeroFrota, C.CAR_CODIGO CodigoCarga
                from T_CARGA_CTE CPD
                JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = CPD.CAR_CODIGO
                JOIN T_CARGA C ON C.CAR_CODIGO = CP.CAR_CODIGO
                JOIN T_CARGA_VEICULOS_VINCULADOS CVV ON CVV.CAR_CODIGO = C.CAR_CODIGO
                JOIN T_VEICULO VR ON VR.VEI_CODIGO = CVV.VEI_CODIGO
                WHERE 1 = 1";
            query += " AND CPD.CAR_CODIGO = " + codigoCarga.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque>();
        }

        #endregion
    }
}
