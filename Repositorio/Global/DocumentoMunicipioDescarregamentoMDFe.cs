using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class DocumentoMunicipioDescarregamentoMDFe : RepositorioBase<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>, Dominio.Interfaces.Repositorios.DocumentoMunicipioDescarregamentoMDFe
    {
        public DocumentoMunicipioDescarregamentoMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public DocumentoMunicipioDescarregamentoMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe BuscarPorCodigo(int codigo, int codigoMunicipioDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();
            var result = from obj in query where obj.Codigo == codigo && obj.MunicipioDescarregamento.Codigo == codigoMunicipioDescarregamento select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarCodigosDeCTesPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe && obj.CTe != null select obj.CTe.Codigo;

            return result.ToList();
        }

        public List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe select obj;

            return result.Fetch(o => o.CTe).ToList();
        }

        public Task<List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>> BuscarPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe select obj;

            return result.Fetch(o => o.CTe).ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe select obj.CTe;

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>> BuscarCTesPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe select obj.CTe;

            return result.ToListAsync(cancellationToken);
        }

        public List<int> BuscarCodigosCTesPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe select obj.CTe.Codigo;

            return result.ToList();
        }


        public Task<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPrimeiroCTePorMDFeAsync(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(obj => obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe && obj.CTe != null);

            return query.Select(o => o.CTe).FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe BuscarPorCTe(int codigoMDFe, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe && obj.CTe.Codigo == codigoCTe select obj;

            return result.FirstOrDefault();
        }

        public List<int> BuscarCodigoDeMDFesPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj.MunicipioDescarregamento.MDFe.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais BuscarMDFesPorCTeEStatus(int codigoCTe, Dominio.Enumeradores.StatusMDFe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.MDFe.Status == status && obj.CTe.Codigo == codigoCTe select obj.MunicipioDescarregamento.MDFe;

            return result.FirstOrDefault();
        }

        public bool VerificaCTePendenteMDFe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.MunicipioDescarregamento.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado select obj.MunicipioDescarregamento.MDFe.Codigo;

            return result.ToList().Count == 0;
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosDocumentoMDFe> BuscarDocumentosParaRelatorio(int[] codigosMDFes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query
                         where codigosMDFes.Contains(obj.MunicipioDescarregamento.MDFe.Codigo)
                         select new Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosDocumentoMDFe()
                         {
                             CodigoMDFe = obj.MunicipioDescarregamento.MDFe.Codigo,
                             Chave = obj.CTe.Chave,
                             Numero = obj.CTe.Numero,
                             Serie = obj.CTe.Serie.Numero,
                             ValorFrete = obj.CTe.ValorFrete,
                             MunicipioDescarregamento = obj.MunicipioDescarregamento.Municipio.Descricao,
                             UFDescarregamento = obj.MunicipioDescarregamento.Municipio.Estado.Sigla,
                             NotasDoCTe = obj.CTe.NumeroNotas
                         };

            return result.ToList();
        }

        public List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> BuscarPorMunicipio(int codigoMunicipio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();
            var result = from obj in query where obj.MunicipioDescarregamento.Codigo == codigoMunicipio select obj;
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>> BuscarPorMunicipioAsync(int codigoMunicipio, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();
            var result = from obj in query where obj.MunicipioDescarregamento.Codigo == codigoMunicipio select obj;

            return result.ToListAsync(cancellationToken);
        }

        public List<Dominio.ObjetosDeValor.Relatorios.DocumentoMDFe> BuscarDocumentosParaDAMDFE(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query
                         where obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe
                         select new Dominio.ObjetosDeValor.Relatorios.DocumentoMDFe()
                         {
                             Chave = obj.CTe.Chave,
                             Tipo = "CT-e",
                             CNPJEmitente = obj.CTe.Empresa.CNPJ
                         };

            return result.ToList();
        }

        public List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> ConsultarCargaConhecimento(int codigoConhecimento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();
            var result = from obj in query select obj;

            if (codigoConhecimento > 0)
                result = result.Where(o => o.CTe.Codigo == codigoConhecimento);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultarCargaConhecimento(int codigoConhecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();
            var result = from obj in query select obj;

            if (codigoConhecimento > 0)
                result = result.Where(o => o.CTe.Codigo == codigoConhecimento);

            return result.Count();
        }

        public void DeletarPorMunicipio(int codigoMunicipioDescarregamento)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE DocumentoMunicipioDescarregamentoMDFe obj WHERE obj.MunicipioDescarregamento.Codigo = :codigoMunicipioDescarregamento")
                                     .SetInt32("codigoMunicipioDescarregamento", codigoMunicipioDescarregamento)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE DocumentoMunicipioDescarregamentoMDFe obj WHERE obj.MunicipioDescarregamento.Codigo = :codigoMunicipioDescarregamento")
                                    .SetInt32("codigoMunicipioDescarregamento", codigoMunicipioDescarregamento)
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

        public List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> BuscarPorMDFe(List<int> codigoMDFe)
        {
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(o => codigoMDFe.Contains(o.MunicipioDescarregamento.MDFe.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas BuscarGrupoPessoasPorMDFe(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(o => o.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe);

            return query.Select(o => o.CTe.TomadorPagador.GrupoPessoas).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> BuscarGrupoPessoasPorMDFeAsync(int codigoMDFe, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            query = query.Where(o => o.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe);

            return query.Select(o => o.CTe.TomadorPagador.GrupoPessoas).FirstOrDefaultAsync(cancellationToken);
        }

        public IList<string> BuscarCNPJsAutorizadosDFe(int codigoMDFe)
        {
            string sql = $@"SELECT DISTINCT CNPJ FROM
                           (
                           SELECT
                           mdfeMunicipioDescarregamento.MDF_CODIGO MDFe,
                           CASE WHEN autorizadoDownloadDFeRemetente.CLI_FISJUR = 'J' THEN FORMAT(autorizadoDownloadDFeRemetente.CLI_CGCCPF, '00000000000000') WHEN autorizadoDownloadDFeRemetente.CLI_FISJUR = 'F' THEN FORMAT(autorizadoDownloadDFeRemetente.CLI_CGCCPF, '00000000000') ELSE null END CNPJAutorizadoRemetente,
                           CASE WHEN autorizadoDownloadDFeExpedidor.CLI_FISJUR = 'J' THEN FORMAT(autorizadoDownloadDFeExpedidor.CLI_CGCCPF, '00000000000000') WHEN autorizadoDownloadDFeExpedidor.CLI_FISJUR = 'F' THEN FORMAT(autorizadoDownloadDFeExpedidor.CLI_CGCCPF, '00000000000') ELSE null END CNPJAutorizadoExpedidor,
                           CASE WHEN autorizadoDownloadDFeRecebedor.CLI_FISJUR = 'J' THEN FORMAT(autorizadoDownloadDFeRecebedor.CLI_CGCCPF, '00000000000000') WHEN autorizadoDownloadDFeRecebedor.CLI_FISJUR = 'F' THEN FORMAT(autorizadoDownloadDFeRecebedor.CLI_CGCCPF, '00000000000') ELSE null END CNPJAutorizadoRecebedor,
                           CASE WHEN autorizadoDownloadDFeDestinatario.CLI_FISJUR = 'J' THEN FORMAT(autorizadoDownloadDFeDestinatario.CLI_CGCCPF, '00000000000000') WHEN autorizadoDownloadDFeDestinatario.CLI_FISJUR = 'F' THEN FORMAT(autorizadoDownloadDFeDestinatario.CLI_CGCCPF, '00000000000') ELSE null END CNPJAutorizadoDestinatario,
                           CASE WHEN autorizadoDownloadDFeTomador.CLI_FISJUR = 'J' THEN FORMAT(autorizadoDownloadDFeTomador.CLI_CGCCPF, '00000000000000') WHEN autorizadoDownloadDFeTomador.CLI_FISJUR = 'F' THEN FORMAT(autorizadoDownloadDFeTomador.CLI_CGCCPF, '00000000000') ELSE null END CNPJAutorizadoTomador
                           FROM T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC mdfeMunicipioDescarregamentoDoc
                           INNER JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO mdfeMunicipioDescarregamento ON mdfeMunicipioDescarregamento.MDD_CODIGO = mdfeMunicipioDescarregamentoDoc.MDD_CODIGO
                           INNER JOIN T_CTE cte ON mdfeMunicipioDescarregamentoDoc.CON_CODIGO = cte.CON_CODIGO

                           LEFT JOIN T_CTE_PARTICIPANTE remetenteCTe ON remetenteCTe.PCT_CODIGO = cte.CON_REMETENTE_CTE
                           LEFT JOIN T_GRUPO_PESSOAS_AUTORIZADO_DOWNLOAD_DFE grupoPessoasAutorizadoDownloadDFeRemetente ON grupoPessoasAutorizadoDownloadDFeRemetente.GRP_CODIGO = remetenteCTe.GRP_CODIGO
                           LEFT JOIN T_CLIENTE autorizadoDownloadDFeRemetente ON autorizadoDownloadDFeRemetente.CLI_CGCCPF = grupoPessoasAutorizadoDownloadDFeRemetente.CLI_CGCCPF

                           LEFT JOIN T_CTE_PARTICIPANTE expedidorCTe ON expedidorCTe.PCT_CODIGO = cte.CON_EXPEDIDOR_CTE
                           LEFT JOIN T_GRUPO_PESSOAS_AUTORIZADO_DOWNLOAD_DFE grupoPessoasAutorizadoDownloadDFeExpedidor ON grupoPessoasAutorizadoDownloadDFeExpedidor.GRP_CODIGO = expedidorCTe.GRP_CODIGO
                           LEFT JOIN T_CLIENTE autorizadoDownloadDFeExpedidor ON autorizadoDownloadDFeExpedidor.CLI_CGCCPF = grupoPessoasAutorizadoDownloadDFeExpedidor.CLI_CGCCPF

                           LEFT JOIN T_CTE_PARTICIPANTE recebedorCTe ON recebedorCTe.PCT_CODIGO = cte.CON_RECEBEDOR_CTE
                           LEFT JOIN T_GRUPO_PESSOAS_AUTORIZADO_DOWNLOAD_DFE grupoPessoasAutorizadoDownloadDFeRecebedor ON grupoPessoasAutorizadoDownloadDFeRecebedor.GRP_CODIGO = recebedorCTe.GRP_CODIGO
                           LEFT JOIN T_CLIENTE autorizadoDownloadDFeRecebedor ON autorizadoDownloadDFeRecebedor.CLI_CGCCPF = grupoPessoasAutorizadoDownloadDFeRecebedor.CLI_CGCCPF

                           LEFT JOIN T_CTE_PARTICIPANTE destinatarioCTe ON destinatarioCTe.PCT_CODIGO = cte.CON_DESTINATARIO_CTE
                           LEFT JOIN T_GRUPO_PESSOAS_AUTORIZADO_DOWNLOAD_DFE grupoPessoasAutorizadoDownloadDFeDestinatario ON grupoPessoasAutorizadoDownloadDFeDestinatario.GRP_CODIGO = destinatarioCTe.GRP_CODIGO
                           LEFT JOIN T_CLIENTE autorizadoDownloadDFeDestinatario ON autorizadoDownloadDFeDestinatario.CLI_CGCCPF = grupoPessoasAutorizadoDownloadDFeDestinatario.CLI_CGCCPF

                           LEFT JOIN T_CTE_PARTICIPANTE tomadorCTe ON tomadorCTe.PCT_CODIGO = cte.CON_TOMADOR_CTE
                           LEFT JOIN T_GRUPO_PESSOAS_AUTORIZADO_DOWNLOAD_DFE grupoPessoasAutorizadoDownloadDFeTomador ON grupoPessoasAutorizadoDownloadDFeTomador.GRP_CODIGO = tomadorCTe.GRP_CODIGO
                           LEFT JOIN T_CLIENTE autorizadoDownloadDFeTomador ON autorizadoDownloadDFeTomador.CLI_CGCCPF = grupoPessoasAutorizadoDownloadDFeTomador.CLI_CGCCPF

                           WHERE mdfeMunicipioDescarregamento.MDF_CODIGO = {codigoMDFe}

                           ) p
                           UNPIVOT (
                           CNPJ FOR CNPJ1 IN 
                           (CNPJAutorizadoRemetente, CNPJAutorizadoExpedidor, CNPJAutorizadoRecebedor, CNPJAutorizadoDestinatario, CNPJAutorizadoTomador)
                           ) AS unpvt 
                           WHERE LEN(CNPJ) > 0";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            return query.List<string>();
        }

        public List<string> BuscarChavesDeCTesPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe>();

            var result = from obj in query where obj.MunicipioDescarregamento.MDFe.Codigo == codigoMDFe select obj.CTe.Chave;

            return result.ToList();
        }
    }
}
