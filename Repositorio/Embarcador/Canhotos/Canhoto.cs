using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using LinqKit;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Util;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Canhotos
{
    public class Canhoto : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.Canhoto>
    {
        #region Construtores

        public Canhoto(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Canhoto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public int BuscarProximaPosicaoPacote(int localArmazenamento, int pacote)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = from obj in query where obj.LocalArmazenamentoCanhoto.Codigo == localArmazenamento && obj.PacoteArmazenado == pacote select obj;

            int? retorno = result.Max(o => (int?)o.PosicaoNoPacote);

            return retorno.HasValue ? (retorno.Value + 1) : 1;
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorNumeroSerieEmitenteNFe(int numero, string serie, double emissor)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.XMLNotaFiscal.Numero == numero
            && obj.XMLNotaFiscal.Serie == serie
            && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == emissor
            && obj.XMLNotaFiscal.nfAtiva
            && obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe);
            return result
                    .Fetch(obj => obj.Emitente)
                    .ThenFetch(obj => obj.GrupoPessoas)
                    .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorNumeroSerieEmitenteNFeETransportador(int numero, string serie, double emissor, string transportador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.XMLNotaFiscal.Numero == numero
            && obj.XMLNotaFiscal.Serie == serie
            && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == emissor
            && obj.Empresa.CNPJ == transportador
            && obj.XMLNotaFiscal.nfAtiva
            && obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe);
            return result
                    .Fetch(obj => obj.Emitente)
                    .ThenFetch(obj => obj.GrupoPessoas)
                    .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorNumeroSerieEmitente(int numero, string serie, double emitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.Numero == numero && obj.Serie == serie);

            if (emitente > 0)
                result = result.Where(obj => obj.Emitente.CPF_CNPJ == emitente);

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorNumeroSerieEmitenteAsync(int numero, string serie, double emitente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj => obj.Numero == numero && obj.Serie == serie);

            if (emitente > 0)
                query = query.Where(obj => obj.Emitente.CPF_CNPJ == emitente);

            return query
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.XMLNotaFiscal)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorChave(string chaveNFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.XMLNotaFiscal.Chave == chaveNFe && obj.XMLNotaFiscal.nfAtiva && obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe);
            return result
                    .Fetch(obj => obj.Emitente)
                    .ThenFetch(obj => obj.GrupoPessoas)
                    .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorChaveAsync(string chaveNFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.XMLNotaFiscal.Chave == chaveNFe && obj.XMLNotaFiscal.nfAtiva && obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe);

            return result
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.XMLNotaFiscal)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorChaves(List<string> chavesNFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => chavesNFe.Contains(obj.XMLNotaFiscal.Chave) && obj.XMLNotaFiscal.nfAtiva && obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorNumeroCanhoto(int numero)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.Numero == numero);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorChaveENFAtiva(string chaveNFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.XMLNotaFiscal.Chave == chaveNFe && obj.XMLNotaFiscal.nfAtiva);
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(bool apenasLiberadoPgto, bool retornarCanhotosViaIntegracaoEmQualquerSituacao, int inicio, int limite, int empresaFilialEmissora, Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = QueryCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(apenasLiberadoPgto, retornarCanhotosViaIntegracaoEmQualquerSituacao, empresaFilialEmissora, configuracaoQualidadeEntrega);

            return query
                .Fetch(obj => obj.XMLNotaFiscal)
                .Fetch(obj => obj.Emitente)
                .Skip(inicio)
                .Take(limite)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(bool apenasLiberadoPgto, bool retornarCanhotosViaIntegracaoEmQualquerSituacao, int inicio, int limite, int empresaFilialEmissora, Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega, bool filtrarNFesNaoEntregues)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = QueryCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(apenasLiberadoPgto, retornarCanhotosViaIntegracaoEmQualquerSituacao, empresaFilialEmissora, configuracaoQualidadeEntrega);

            if (filtrarNFesNaoEntregues)
                query = query.Where(obj => obj.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Entregue);

            return query.
                Fetch(obj => obj.XMLNotaFiscal).
                Fetch(obj => obj.Emitente).
                Fetch(obj => obj.ChaveCTe).
                Skip(inicio).Take(limite).ToList();
        }

        public int ContarCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(bool apenasLiberadoPgto, bool retornarCanhotosViaIntegracaoEmQualquerSituacao, int empresaFilialEmissora, Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = QueryCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(apenasLiberadoPgto, retornarCanhotosViaIntegracaoEmQualquerSituacao, empresaFilialEmissora, configuracaoQualidadeEntrega);
            return query.Count();
        }

        public int ContarCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(bool apenasLiberadoPgto, bool retornarCanhotosViaIntegracaoEmQualquerSituacao, int empresaFilialEmissora, Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega, bool filtrarNFesNaoEntregues)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = QueryCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(apenasLiberadoPgto, retornarCanhotosViaIntegracaoEmQualquerSituacao, empresaFilialEmissora, configuracaoQualidadeEntrega);

            if (filtrarNFesNaoEntregues)
                query = query.Where(obj => obj.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Entregue);

            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> QueryCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(bool apenasLiberadoPgto, bool retornarCanhotosViaIntegracaoEmQualquerSituacao, int empresaFilialEmissora, Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj => !obj.DigitalizacaoIntegrada);

            query = query.Where(canhoto => canhoto.TipoCanhoto == TipoCanhoto.Avulso ? canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Any(pedidoXmlNotaFiscal => pedidoXmlNotaFiscal.XMLNotaFiscal != null) : canhoto.XMLNotaFiscal != null);

            if (configuracaoQualidadeEntrega?.VerificarDataConfirmacaoIntervaloRaio ?? false)
                query = query.Where(obj => obj.DisponivelParaConsulta);

            if (!retornarCanhotosViaIntegracaoEmQualquerSituacao)
                query = query.Where(obj => obj.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado);

            if (apenasLiberadoPgto)
            {
                query = query.Where(obj => obj.SituacaoPgtoCanhoto == SituacaoPgtoCanhoto.Liberado);

                //Seleção dos CTes sem canhotos pendente pagamento
                IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                queryCTes = queryCTes.Where(o => !o.XMLNotaFiscais.Any(x => x.Canhoto.SituacaoPgtoCanhoto != SituacaoPgtoCanhoto.Liberado));

                //Seleção das notas dos CTes 
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> queryNotas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                queryNotas = queryNotas.Where(obj => queryCTes.Any(c => c.XMLNotaFiscais.Any(o => o.Codigo == obj.Codigo)));

                //Seleção das canhotos das notas 
                query = query.Where(obj => (obj.Carga.TipoOperacao.RetornarCanhotoQuandoTodasNotasDoCTeEstiveremConformadasPagamento == false) ||
                                           (obj.Carga.TipoOperacao.RetornarCanhotoQuandoTodasNotasDoCTeEstiveremConformadasPagamento == true &&
                                            queryNotas.Any(n => n.Codigo == obj.XMLNotaFiscal.Codigo)));

            }

            if (empresaFilialEmissora > 0)
                query = query.Where(obj => obj.EmpresaFilialEmissora.Codigo == empresaFilialEmissora);

            return query;
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotosDigitalizadoseAgAprovacao(bool apenasLiberadoPgto, int inicio, int limite, int empresaFilialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = BaseCanhotosDigitalizadoseAgAprovacao(apenasLiberadoPgto, empresaFilialEmissora);
            return query.Fetch(obj => obj.XMLNotaFiscal)
                .Fetch(obj => obj.Emitente)
                .Skip(inicio)
                .Take(limite)
                .ToList();
        }

        public int ContarCanhotosDigitalizadoseAgAprovacao(bool apenasLiberadoPgto, int empresaFilialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = BaseCanhotosDigitalizadoseAgAprovacao(apenasLiberadoPgto, empresaFilialEmissora);
            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj => obj.CargaPedido.Codigo == codigoCargaPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = from o in query where codigos.Contains(o.Codigo) select o;

            return result
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>> BuscarPorCodigosAsync(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(canhoto => codigos.Contains(canhoto.Codigo));

            return query
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorCodigosESituacaoDiferenteDe(List<int> codigos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto situacaoDigitalizacaoCanhotoDiferente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = from o in query where codigos.Contains(o.Codigo) && o.SituacaoDigitalizacaoCanhoto != situacaoDigitalizacaoCanhotoDiferente select o;

            return result
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .ToList();
        }

        public int ContarCanhotosPendentesDigitalizacaoPorCTe(int cte)
        {
            string sqlQuery = @"select count(canhoto.NFX_CODIGO) from T_CTE_XML_NOTAS_FISCAIS cteNF inner join T_CANHOTO_NOTA_FISCAL canhoto ON canhoto.NFX_CODIGO = cteNF.NFX_CODIGO where cteNF.con_codigo = " + cte + " and canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO <> 3 and canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO <> 5 and canhoto.CNF_SITUACAO_CANHOTO <> 2 and canhoto.CNF_SITUACAO_CANHOTO <> 8"; // SQL-INJECTION-SAFE

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query.UniqueResult<int>();
        }
        public Task<string> BuscarSeExisteChamadoAbertoParaOCanhotoAsync(int codigoCanhoto)
        {
            var sqlQuery = @"

                            select (
                                    SELECT 
                                        STRING_AGG(ChamadoMotivoFormatado, ', ') AS ChamadosEMotivos
                                    FROM (
                                        SELECT DISTINCT
                                            CAST(Chamado.CHA_NUMERO AS NVARCHAR(MAX)) + ' - ' + Motivo.MCH_DESCRICAO AS ChamadoMotivoFormatado
                                        FROM T_CHAMADOS Chamado
                                        INNER JOIN T_MOTIVO_CHAMADA Motivo ON Motivo.MCH_CODIGO = Chamado.MCH_CODIGO
                                        WHERE 
                                            Chamado.CHA_SITUACAO NOT IN (2, 5)
                                            AND Motivo.MCH_TIPO_MOTIVO_ATENDIMENTO = 0
                                            AND (
                                                (
                                                    EXISTS (
                                                        SELECT 1
                                                        FROM T_CARGA_PEDIDO CP
                                                        INNER JOIN T_PEDIDO_XML_NOTA_FISCAL PNF ON CP.CPE_CODIGO = PNF.CPE_CODIGO
                                                        INNER JOIN T_XML_NOTA_FISCAL NFX ON PNF.NFX_CODIGO = NFX.NFX_CODIGO
                                                        WHERE CP.CAR_CODIGO = Chamado.CAR_CODIGO
                                                            AND NFX.NFX_CODIGO = CanhotoNotaFiscal.NFX_CODIGO
                                                    )
                                                    AND Chamado.CEN_CODIGO IS NULL
                                                )
                                                OR EXISTS (
                                                    SELECT 1
                                                    FROM T_CARGA_ENTREGA_NOTA_FISCAL CENF
                                                    INNER JOIN T_PEDIDO_XML_NOTA_FISCAL PNF ON CENF.PNF_CODIGO = PNF.PNF_CODIGO
                                                    INNER JOIN T_XML_NOTA_FISCAL NFX ON PNF.NFX_CODIGO = NFX.NFX_CODIGO
                                                    WHERE CENF.CEN_CODIGO = Chamado.CEN_CODIGO
                                                        AND NFX.NFX_CODIGO = CanhotoNotaFiscal.NFX_CODIGO
                                                )
                                            )
                                    ) ChamadosFormatados ) as Chamado
                            from T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal
                            where CNF_CODIGO = " + codigoCanhoto;

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query.UniqueResultAsync<string>(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarCanhotoPorNumeroECarga(int numero, int carga, bool canhotoAvulso)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.Numero == numero && obj.Carga.Codigo == carga);

            if (canhotoAvulso)
                query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso.Ativo);
            else
            {
                query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.NFe && obj.XMLNotaFiscal.nfAtiva);
            }

            return query
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarCanhotoPorNumerosECarga(List<int> numero, int carga, bool canhotoAvulso)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => numero.Contains(obj.Numero) && obj.Carga.Codigo == carga);

            if (canhotoAvulso)
                query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso.Ativo);
            else
            {
                query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.NFe && obj.XMLNotaFiscal.nfAtiva);
            }

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarCanhotoNFePorNumero(int numero, double emitente, int empresa, bool canhotoAvulso, int grupoCliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.Numero == numero);

            if (canhotoAvulso)
                query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso.Ativo);
            else
            {
                if (emitente > 0)
                {
                    query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.NFe
                  && ((obj.XMLNotaFiscal.Emitente.CPF_CNPJ == emitente && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida)
                  || (obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == emitente && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada))
                  && obj.XMLNotaFiscal.nfAtiva);
                }

                if (grupoCliente > 0)
                {
                    query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.NFe
                && ((obj.XMLNotaFiscal.Emitente.GrupoPessoas.Codigo == grupoCliente && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida)
                || (obj.XMLNotaFiscal.Destinatario.GrupoPessoas.Codigo == grupoCliente && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada))
                && obj.XMLNotaFiscal.nfAtiva);
                }

                if (empresa > 0)
                    query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.NFe && obj.Empresa.Codigo == empresa && obj.XMLNotaFiscal.nfAtiva);

            }

            return query
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarMaloteCanhotoPorCodigo(int codigoMalote)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = from obj in query where obj.Malote.Codigo == codigoMalote select obj;

            return result.Distinct().ToList();
        }

        public void RemoverMaloteCanhotos(int malote)
        {
            string hql = "UPDATE Canhoto canhoto SET canhoto.Malote = null WHERE canhoto.Malote = :Malote";
            IQuery query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Malote", malote);
            query.ExecuteUpdate();
        }

        public void AtualizarDataIntegracaoEntrega(int codigoCanhoto, DateTime dataIntegracaoEntrega)
        {
            string hql = "UPDATE Canhoto canhoto SET canhoto.DataIntegracaoEntrega = :DataIntegracaoEntrega WHERE canhoto.Codigo = :Codigo";
            IQuery query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Codigo", codigoCanhoto);
            query.SetDateTime("DataIntegracaoEntrega", dataIntegracaoEntrega);
            query.ExecuteUpdate();
        }

        public void SetarTransportadorCanhotos(int carga, int empresa)
        {
            string hql = "UPDATE Canhoto canhoto SET canhoto.Empresa = :Empresa WHERE canhoto.Carga = :Carga";
            IQuery query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Empresa", empresa);
            query.SetInt32("Carga", carga);
            query.ExecuteUpdate();
        }

        public void SetarTransportadorCanhotosPorCargaAgrupadaOuCargaNova(int codigoCargaAgrupada, int codigoEmpresa, int? codigoCargaNova = null)
        {
            string sql = "update canhoto set canhoto.car_codigo = " + codigoCargaAgrupada; // SQL-INJECTION-SAFE

            if (codigoEmpresa > 0)
                sql += ", canhoto.emp_codigo = " + codigoEmpresa;
            else
                sql += ", canhoto.emp_codigo = null ";

            sql += " from T_CANHOTO_NOTA_FISCAL canhoto inner join t_carga carga on carga.car_codigo = canhoto.CAR_CODIGO where carga.CAR_CODIGO_AGRUPAMENTO = " + codigoCargaAgrupada; // SQL-INJECTION-SAFE

            if (codigoCargaNova != null)
                sql += $"or carga.CAR_CODIGO = {codigoCargaNova}";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.ExecuteUpdate();
        }

        public void SetarTransportadorCanhotosPorCargaVinculada(int carga, int empresa)
        {
            string sql = "update canhoto set canhoto.car_codigo = " + carga; // SQL-INJECTION-SAFE

            if (empresa > 0)
                sql += ", canhoto.emp_codigo = " + empresa;
            else
                sql += ", canhoto.emp_codigo = null ";

            sql += " from T_CANHOTO_NOTA_FISCAL canhoto inner join t_carga carga on carga.car_codigo = canhoto.CAR_CODIGO where carga.CAR_CODIGO_VINCULADA = " + carga + ";"; // SQL-INJECTION-SAFE

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.ExecuteUpdate();
        }

        public void RemoverCanhotosMalote(int malote)
        {
            string hql = "UPDATE Canhoto canhoto SET canhoto.Malote = null WHERE canhoto.Malote.Codigo = :Malote";
            IQuery query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Malote", malote);
            query.ExecuteUpdate();
        }

        public void SetarCanhotoDigitalizado(int canhoto)
        {
            string hql = @"UPDATE Canhoto canhoto 
                            SET 
                                canhoto.DataUltimaModificacao = :DataUltimaModificacao,
                                canhoto.MotivoRejeicaoDigitalizacao = :MotivoRejeicaoDigitalizacao, 
                                canhoto.SituacaoDigitalizacaoCanhoto = :SituacaoDigitalizacaoCanhoto
                            WHERE 
                                canhoto.Codigo = :Canhoto";

            IQuery query = this.SessionNHiBernate.CreateQuery(hql);

            query.SetInt32("Canhoto", canhoto);
            query.SetDateTime("DataUltimaModificacao", DateTime.Now);
            query.SetString("MotivoRejeicaoDigitalizacao", "");
            query.SetEnum("SituacaoDigitalizacaoCanhoto", SituacaoDigitalizacaoCanhoto.Digitalizado);

            query.ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarCanhotoNFePorNumeros(List<int> numeros, double emitente, int empresa, bool canhotoAvulso, int grupoCliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => numeros.Contains(obj.Numero));

            if (canhotoAvulso)
                query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso.Ativo);
            else
            {
                if (emitente > 0)
                {
                    query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.NFe
                  && ((obj.XMLNotaFiscal.Emitente.CPF_CNPJ == emitente && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida)
                  || (obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == emitente && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada))
                  && obj.XMLNotaFiscal.nfAtiva);
                }

                if (grupoCliente > 0)
                {
                    query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.NFe
             && ((obj.XMLNotaFiscal.Emitente.GrupoPessoas.Codigo == grupoCliente && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida)
             || (obj.XMLNotaFiscal.Destinatario.GrupoPessoas.Codigo == grupoCliente && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada))
             && obj.XMLNotaFiscal.nfAtiva);
                }

                if (empresa > 0)
                    query = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.NFe && obj.Empresa.Codigo == empresa && obj.XMLNotaFiscal.nfAtiva);

            }

            return query
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCTe(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorChaveCTeSubcontratacao(string chaveCTeSubcontratacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.CTeSubcontratacao.ChaveAcesso == chaveCTeSubcontratacao && obj.CTeSubcontratacao.Ativo && obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao);

            return query
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorChaveCTe(string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.CargaCTe.CTe.Chave == chaveCTe && obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe);

            return query
                  .Fetch(obj => obj.Emitente)
                  .ThenFetch(obj => obj.GrupoPessoas)
                  .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorCargaECTe(int codigoCarga, int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.CargaCTe.CTe.Codigo == codigoCTe && obj.CargaCTe.Carga.Codigo == codigoCarga && obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorCargaENF(int codigoCarga, int codigoNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.XMLNotaFiscal.Codigo == codigoNotaFiscal);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorCTeCargaCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.CargaCTe.CTe.Codigo == codigoCTe && obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorQRCodeAvulso(string code)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.CanhotoAvulso.QRCode == code && obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso);
            return result
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.Codigo == codigo);
            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .Fetch(obj => obj.Emitente).ThenFetch(obj => obj.GrupoPessoas)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorCodigoAsync(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.Codigo == codigo);
            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .Fetch(obj => obj.Emitente).ThenFetch(obj => obj.GrupoPessoas)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarCanhotoXMLNotaFiscal(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.Codigo == codigo);
            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorIdTrizy(string idTrizy)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.IdTrizy == idTrizy);
            return result
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> consultaCanhoto = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);

            return consultaCanhoto
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.Emitente).ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Destinatario).ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Destinatario).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.XMLNotaFiscal)
                .Fetch(obj => obj.CanhotoAvulso)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>> BuscarPorCargaAsync(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> consultaCanhoto = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);

            return consultaCanhoto.ToListAsync(CancellationToken);
        }

        public async Task<List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>> BuscarPorCargasAsync(List<int> codigosCarga)
        {
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> resultado = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            int take = 1000;
            int start = 0;

            while (start < codigosCarga.Count)
            {
                List<int> tmp = codigosCarga.Skip(start).Take(take).ToList();

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> parcial = await SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                    .Where(obj => tmp.Contains(obj.Carga.Codigo))
                    .Fetch(obj => obj.Filial)
                    .Fetch(obj => obj.Emitente).ThenFetch(obj => obj.Localidade)
                    .Fetch(obj => obj.Destinatario).ThenFetch(obj => obj.Localidade)
                    .Fetch(obj => obj.Destinatario).ThenFetch(obj => obj.Empresa)
                    .Fetch(obj => obj.XMLNotaFiscal)
                    .Fetch(obj => obj.CanhotoAvulso)
                    .ToListAsync(CancellationToken);

                resultado.AddRange(parcial);

                start += take;
            }

            return resultado;
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorCargaOrigem(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> consultaCanhoto = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj => obj.CargaPedido.CargaOrigem.Codigo == codigoCarga);

            return consultaCanhoto
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.Emitente).ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Destinatario).ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Destinatario).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.XMLNotaFiscal)
                .Fetch(obj => obj.CanhotoAvulso)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPendetesLiberacaoFisicaPorMalote(int malote)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.Malote.Codigo == malote && obj.Emitente.GrupoPessoas.ExigeCanhotoFisico == true);
            return result
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotoCTePorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.TipoCanhoto == TipoCanhoto.CTe && obj.Carga.Codigo == carga);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPendentes(DateTime dataInicial)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = from obj in query
                                                                               where
                                                                               obj.SituacaoCanhoto == SituacaoCanhoto.Pendente
                                                                               && obj.Empresa != null
                                                                               && obj.Filial != null
                                                                               && obj.DataEnvioCanhoto.Date >= dataInicial
                                                                               select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ObterCanhotosModificadosPorUltimaConsulta(int motorista, DateTime dataUltimaConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.Carga.Motoristas.Any(mot => mot.Codigo == motorista) && obj.DataUltimaModificacao > dataUltimaConsulta);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorNF(int nota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.XMLNotaFiscal.Codigo == nota);
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.CargaPedido)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorNFAsync(int nota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            INhFetchRequest<Dominio.Entidades.Embarcador.Canhotos.Canhoto, Dominio.Entidades.Embarcador.Cargas.CargaPedido> result = query.Where(obj => obj.XMLNotaFiscal.Codigo == nota).Fetch(obj => obj.Carga)
                .Fetch(obj => obj.CargaPedido);

            return result.FirstOrDefaultAsync();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>> BuscarPorNotasAsync(List<int> codigosNotas)
        {
            if (codigosNotas == null || codigosNotas.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            int take = 2000;
            int start = 0;
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> resultado = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            while (start < codigosNotas.Count)
            {
                List<int> tmp = codigosNotas.Skip(start).Take(take).ToList();

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> parcial = await SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                    .Where(obj => tmp.Contains(obj.XMLNotaFiscal.Codigo))
                    .Fetch(obj => obj.Destinatario)
                    .Fetch(obj => obj.Emitente)
                    .ToListAsync();

                resultado.AddRange(parcial);

                start += take;
            }

            return resultado;
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotosPorNF(int nota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.XMLNotaFiscal.Codigo == nota);
            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.CargaPedido)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotosPorNFs(List<int> notas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => notas.Contains(obj.XMLNotaFiscal.Codigo));
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotosPorNFCanhotoAvulso(int codigoNotaFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.CanhotoAvulso != null && obj.CanhotoAvulso.PedidosXMLNotasFiscais.Any(pedidoXMLNotaFiscal => pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == codigoNotaFiscal));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarPorNFsComImagem(List<int> notas, int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => notas.Contains(obj.XMLNotaFiscal.Codigo) && obj.Carga.Codigo == carga && obj.GuidNomeArquivo != "" && obj.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado);
            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>> BuscarPorNotasDosCTesECanhotoAsync(List<int> codigosCTe, int canhotoCodigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj =>
                    obj.Codigo != canhotoCodigo &&
                    obj.SituacaoCanhoto != SituacaoCanhoto.Justificado &&
                    obj.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado &&
                    obj.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Cancelada &&
                    obj.XMLNotaFiscal.CTEs.Any(o => codigosCTe.Contains(o.Codigo))
                );

            return query.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>> BuscarPorNotasDosCTesAsync(List<int> codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj => obj.XMLNotaFiscal.CTEs.Any(o => codigoCTe.Contains(o.Codigo)));

            return query.ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.CTeSubcontratacao.Codigo == codigoCTe);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.Canhoto BuscarPorCargaCTe(int codigoCargaCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.CargaCTe.Codigo == codigoCargaCTe);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> Consultar(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = MontarQuery(filtro);

            return query.Fetch(obj => obj.Emitente).ThenFetch(obj => obj.GrupoPessoas).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ConsultarCanhotosNFeEntreguesAguardandoIntegracao(int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe && (obj.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado || obj.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente) && !obj.XMLNotaFiscal.RetornoNotaIntegrada && obj.XMLNotaFiscal.nfAtiva);

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ConsultarCanhotosDigitalizadosPendentesDesbloqueioTitulo(int maximoRegistros, int minutosReprocessamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado
                && obj.SituacaoGeracaoCancelamentoAutomatico == SituacaoGeracaoCancelamentoAutomatico.PendenteGerarDesbloqueioTitulo
                && (obj.DataGeracaoCancelamentoAutomatico == null || obj.DataGeracaoCancelamentoAutomatico.Value < DateTime.Now.AddMinutes(-minutosReprocessamento)));

            return result.OrderBy(obj => obj.DataGeracaoCancelamentoAutomatico.HasValue ? 1 : 0)
                .ThenBy(obj => obj.DataGeracaoCancelamentoAutomatico)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarCanhotosNFeEntreguesAguardandoIntegracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe && (obj.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado || obj.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente) && !obj.XMLNotaFiscal.RetornoNotaIntegrada && obj.XMLNotaFiscal.nfAtiva);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ConsultarLocalArmazenamento(int localArmazenamento, List<int> codigosFiliais, List<double> codigosRecebedores, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.LocalArmazenamentoCanhoto.Codigo == localArmazenamento);

            if (codigosFiliais.Any(o => o == -1))
                result = result.Where(o => codigosFiliais.Contains(o.Carga.Filial.Codigo) || o.Carga.Pedidos.Any(pedido => codigosRecebedores.Contains(pedido.Recebedor.CPF_CNPJ)));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.TipoDeCarga)
                .Fetch(obj => obj.Emitente)
                .Fetch(obj => obj.Empresa)
                .Fetch(obj => obj.CanhotoAvulso)
                .Fetch(obj => obj.CTeSubcontratacao)
                .Take(maximoRegistros).ToList();
        }

        public int ContarLocalArmazenamento(int localArmazenamento, List<int> codigosFiliais, List<double> codigosRecebedores)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.LocalArmazenamentoCanhoto.Codigo == localArmazenamento);

            if (codigosFiliais.Any(o => o == -1))
                result = result.Where(o => codigosFiliais.Contains(o.Carga.Filial.Codigo) || o.Carga.Pedidos.Any(pedido => codigosRecebedores.Contains(pedido.Recebedor.CPF_CNPJ)));


            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ConsultarCanhotosMalote(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = MontarQuery(filtro);

            return query.ToList();
        }

        public dynamic ConsultarDynamicMalotes(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtro, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = MontarQuery(filtro);

            return query
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros).Take(maximoRegistros).Select(canhoto => new
                {
                    canhoto.Codigo,
                    CodigoFilial = canhoto.Filial.Codigo,
                    canhoto.Numero,
                    canhoto.Serie,
                    DescricaoTipoCanhoto = canhoto.TipoCanhoto.ObterDescricao(),
                    DataEmissao = canhoto.DataEmissao.ToString("dd/MM/yyyy"),
                    NumeroCarga = canhoto.Carga.CodigoCargaEmbarcador ?? "",
                    Filial = canhoto.Filial.Descricao,
                    Valor = canhoto.Valor.ToString("n2"),
                    Destinatario = canhoto.Destinatario.Nome,
                    Motorista = canhoto.Carga.NomeMotoristas ?? "",
                    Empresa = canhoto.Empresa.RazaoSocial ?? "Não informado",
                    TipoCarga = canhoto.Carga.TipoDeCarga.Descricao ?? "",
                    DescricaoSituacao = canhoto.SituacaoCanhoto.ObterDescricao(),
                    DescricaoDigitalizacao = canhoto.SituacaoDigitalizacaoCanhoto.ObterDescricao(),
                    DataEnvioCanhoto = canhoto.SituacaoCanhoto != SituacaoCanhoto.Pendente ? canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy") : "",
                    DataNotaFiscal = ((DateTime?)canhoto.XMLNotaFiscal.DataEmissao).HasValue ? ((DateTime?)canhoto.XMLNotaFiscal.DataEmissao).Value.ToString("dd/MM/yyyy") : "",
                }).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ConsultarArquivosDownloadEmMassa(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = MontarQuery(filtro);
            query = query.Where(o => o.GuidNomeArquivo != "" && o.NomeArquivo != "");
            return query.WithOptions(o => { o.SetTimeout(10000); }).Select(canhoto => new Dominio.Entidades.Embarcador.Canhotos.Canhoto
            {
                GuidNomeArquivo = canhoto.GuidNomeArquivo,
                NomeArquivo = canhoto.NomeArquivo,
                TipoCanhoto = canhoto.TipoCanhoto,
                Numero = canhoto.Numero,
                Serie = canhoto.Serie,
                Emitente = new Dominio.Entidades.Cliente
                {
                    CPF_CNPJ = canhoto.Emitente.CPF_CNPJ,
                    Tipo = canhoto.Emitente.Tipo
                },

            }).ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos> ConsultarDynamic(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtro, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = MontarQuery(filtro);

            if (!filtro.Malote.HasValue || filtro.Malote.Value == 0)
                query = query.Where(o => o.Carga == null || (o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada));

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                query = query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query.WithOptions(o => { o.SetTimeout(3000); }).Select(canhoto => new Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos
            {
                CodigoLocalArmazenamento = (int?)canhoto.LocalArmazenamentoCanhoto.Codigo ?? 0,
                DescricaoLocalArmazenamento = canhoto.LocalArmazenamentoCanhoto.Descricao ?? string.Empty,
                Codigo = canhoto.Codigo,
                Chave = canhoto.TipoCanhoto == TipoCanhoto.NFe ? canhoto.XMLNotaFiscal.Chave : canhoto.TipoCanhoto == TipoCanhoto.CTeSubcontratacao ? canhoto.CTeSubcontratacao.ChaveAcesso : "",
                Numero = canhoto.Numero,
                Serie = canhoto.Serie,
                CodigoFilial = (int?)canhoto.Filial.Codigo ?? 0,
                Filial = canhoto.Filial.Descricao ?? string.Empty,
                SituacaoCanhoto = canhoto.SituacaoCanhoto,
                SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto,
                NumeroCarga = canhoto.Carga.CodigoCargaEmbarcador ?? "",
                TipoCarga = canhoto.Carga.TipoDeCarga.Descricao ?? "",
                Motorista = canhoto.Carga.NomeMotoristas ?? "",
                CPFCNPJEmitente = canhoto.Emitente.CPF_CNPJ,
                NomeEmitente = canhoto.Emitente.Nome,
                TipoEmitente = canhoto.Emitente.Tipo,
                Destinatario = canhoto.Destinatario.Nome ?? "",
                CNPJDestinatario = canhoto.Destinatario.CPF_CNPJ,
                TipoDestinatario = canhoto.Destinatario.Tipo,
                Empresa = canhoto.Empresa.RazaoSocial ?? "Não informado",
                NomeArquivo = canhoto.NomeArquivo,
                PacoteArmazenado = canhoto.PacoteArmazenado,
                PosicaoNoPacote = canhoto.PosicaoNoPacote,
                GuidNomeArquivo = canhoto.GuidNomeArquivo,
                Valor = int.Parse(Math.Round(canhoto.Valor, 0).ToString()),
                Observacao = canhoto.Observacao,
                NumeroCTe = canhoto.TipoCanhoto == TipoCanhoto.CTe ? canhoto.Numero.ToString() : canhoto.NumeroCTe,
                NumeroDocumentoOriginario = canhoto.NumeroDocumentoOriginario,
                SituacaoPgtoCanhoto = canhoto.SituacaoPgtoCanhoto,
                Veiculo = canhoto.Carga.PlacasVeiculos ?? "",
                SituacaoNotaFiscal = canhoto.XMLNotaFiscal.UltimaSituacaoEntregaDevolucao ?? canhoto.XMLNotaFiscal.SituacaoEntregaNotaFiscal,
                NumeroProtocolo = canhoto.NumeroProtocolo,
                Origem = this.RetornarOrigemDestino(canhoto, 1),
                Destino = this.RetornarOrigemDestino(canhoto, 2),
                CentroResultadoCarga = canhoto.Carga.TipoOperacao != null ? canhoto.Carga.TipoOperacao.Descricao : ""

            }).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = MontarQuery(filtro);

            if (!filtro.Malote.HasValue || filtro.Malote.Value == 0 && !(filtro.CodigosCargaEmbarcador?.Count > 0))
                result = result.Where(o => o.Carga == null || (o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada));

            return result.Count();
        }

        public int ContarCanhotosNaoEnviadosComEnvioImagemAceitoPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> result = query.Where(obj => obj.Carga.Codigo == carga && obj.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado);

            return result.Count();
        }

        public int ContarCanhotosNaoEnviadosPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.Carga.Codigo == carga &&
                                            obj.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente &&
                                            obj.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado &&
                                            ((obj.TipoCanhoto == TipoCanhoto.CTeSubcontratacao && obj.CTeSubcontratacao.Ativo) ||
                                             (obj.TipoCanhoto == TipoCanhoto.NFe && obj.XMLNotaFiscal.nfAtiva) ||
                                             (obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso.Ativo) ||
                                             (obj.TipoCanhoto == TipoCanhoto.CTe && obj.CargaCTe.CTe.Status == "A")));

            return query.Count();
        }

        public int ContarCanhotosPedentesPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.Carga.Codigo == carga
                                            &&
                                            obj.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente
                                            &&
                                            obj.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado
                                            &&
                                            obj.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado
                                            &&
                                            obj.XMLNotaFiscal.SituacaoEntregaNotaFiscal != SituacaoNotaFiscal.Devolvida
                                            &&
                                            obj.XMLNotaFiscal.SituacaoEntregaNotaFiscal != SituacaoNotaFiscal.AgReentrega
                                            &&
                                            ((obj.TipoCanhoto == TipoCanhoto.CTeSubcontratacao && obj.CTeSubcontratacao.Ativo) ||
                                             (obj.TipoCanhoto == TipoCanhoto.NFe && obj.XMLNotaFiscal.nfAtiva) ||
                                             (obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso.Ativo) ||
                                             (obj.TipoCanhoto == TipoCanhoto.CTe && obj.CargaCTe.CTe.Status == "A")));

            return query.Count();
        }

        public bool ValidarSeExisteCanhotoPendenteEAguardandoAprovacaoPorCTe(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe item in carga.CargaCTes)
            {
                query = query.Where(
                    o => o.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado
                          && (o.XMLNotaFiscal.CTEs.Any(c => c.Codigo == item.Codigo))
                            && (o.XMLNotaFiscal.SituacaoEntregaNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.Devolvida)
                          );

            }

            if (query.Count() > 0)
            {
                return false;
            }

            return true;

        }
        public bool VerificarSeExisteCanhotoNotaFiscalPendentePorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente && o.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado && o.XMLNotaFiscal.CTEs.Any(c => c.Codigo == codigoCTe));

            return query.Select(o => o.Codigo).Any();
        }

        public Task<bool> VerificarSeExisteCanhotoNotaFiscalPendentePorCTeAsync(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(o => o.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente &&
                            o.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado &&
                            o.XMLNotaFiscal.CTEs.Any(c => c.Codigo == codigoCTe));

            return query.Select(o => o.Codigo).AnyAsync(CancellationToken);
        }

        public bool VerificarSeExisteCanhotoCTePendentePorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente && o.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado && o.CargaCTe.CTe.Codigo == codigoCTe);

            return query.Select(o => o.Codigo).Any();
        }

        public Task<bool> VerificarSeExisteCanhotoCTePendentePorCTeAsync(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(o => o.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.RecebidoFisicamente &&
                            o.SituacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado &&
                            o.CargaCTe.CTe.Codigo == codigoCTe);

            return query.Select(o => o.Codigo).AnyAsync(CancellationToken);
        }

        public Task<bool> VerificarSeExisteCanhotoNotaFiscalPendenteDigitalizacaoPorCargaAsync(int codigoCarga, int codigoCanhoto)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj =>
                obj.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado &&
                obj.Carga.Codigo == codigoCarga &&
                obj.Codigo != codigoCanhoto);

            return query.AnyAsync(CancellationToken);
        }

        public bool VerificarSeExisteCanhotoNotaFiscalPendenteDigitalizacaoPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado && o.XMLNotaFiscal.CTEs.Any(c => c.Codigo == codigoCTe));

            return query.Select(o => o.Codigo).Any();
        }

        public Task<bool> VerificarSeExisteCanhotoNotaFiscalPendenteDigitalizacaoPorCTeAsync(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(o => o.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado &&
                            o.XMLNotaFiscal.CTEs.Any(c => c.Codigo == codigoCTe));

            return query.Select(o => o.Codigo).AnyAsync(CancellationToken);
        }

        public bool VerificarSeExisteCanhotoCTePendenteDigitalizacaoPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado && o.CargaCTe.CTe.Codigo == codigoCTe);

            return query.Select(o => o.Codigo).Any();
        }

        public Task<bool> VerificarSeExisteCanhotoCTePendenteDigitalizacaoPorCTeAsync(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(o => o.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado &&
                            o.CargaCTe.CTe.Codigo == codigoCTe);

            return query.Select(o => o.Codigo).AnyAsync(CancellationToken);
        }

        public bool VerificarSeExisteCanhotoPendenteDigitalizacaoPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado && (o.XMLNotaFiscal.CTEs.Any(c => c.Codigo == codigoCTe) || o.CargaCTe.CTe.Codigo == codigoCTe));

            return query.Select(o => o.Codigo).Any();
        }

        public bool VerificarSeExisteCanhotoNaoEntreguePeloMotoristaPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente && (o.XMLNotaFiscal.CTEs.Any(c => c.Codigo == codigoCTe) || o.CargaCTe.CTe.Codigo == codigoCTe));

            return query.Select(o => o.Codigo).Any();
        }

        public bool VerificarSeExisteCanhotoNotaFiscalNaoEntreguePeloMotoristaPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente && o.XMLNotaFiscal.CTEs.Any(c => c.Codigo == codigoCTe));

            return query.Select(o => o.Codigo).Any();
        }

        public bool VerificarSeExisteCanhotoCTeNaoEntreguePeloMotoristaPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente && o.CargaCTe.CTe.Codigo == codigoCTe);

            return query.Select(o => o.Codigo).Any();
        }

        public DateTime ObterUltimaDataEnvioCanhotoPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query2 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.XMLNotaFiscal.CTEs.Any(c => c.Codigo == codigoCTe));
            query2 = query2.Where(o => o.CargaCTe.CTe.Codigo == codigoCTe);

            DateTime data1 = query.OrderByDescending(o => o.DataEnvioCanhoto).Select(o => o.DataEnvioCanhoto).FirstOrDefault();
            DateTime data2 = query2.OrderByDescending(o => o.DataEnvioCanhoto).Select(o => o.DataEnvioCanhoto).FirstOrDefault();

            return data1 > data2 ? data1 : data2;
        }

        public DateTime ObterUltimaDataDigitalizacaoCanhotoPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query2 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado && o.DataDigitalizacao != null && o.XMLNotaFiscal.CTEs.Any(c => c.Codigo == codigoCTe));
            query2 = query2.Where(o => o.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado && o.DataDigitalizacao != null && o.CargaCTe.CTe.Codigo == codigoCTe);

            DateTime data1 = query.OrderByDescending(o => o.DataDigitalizacao.Value).Select(o => o.DataDigitalizacao.Value).FirstOrDefault();
            DateTime data2 = query2.OrderByDescending(o => o.DataDigitalizacao.Value).Select(o => o.DataDigitalizacao.Value).FirstOrDefault();

            return data1 > data2 ? data1 : data2;
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ConsultarParaVinculo(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhotoParaVinculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> consultaCanhoto = ConsultarParaVinculo(filtrosPesquisa);

            consultaCanhoto = consultaCanhoto
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.TipoDeCarga)
                .Fetch(obj => obj.Empresa);

            return ObterLista(consultaCanhoto, parametrosConsulta);
        }

        public int ContarConsultaParaVinculo(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhotoParaVinculo filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> consultaCanhoto = ConsultarParaVinculo(filtrosPesquisa);

            return consultaCanhoto.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.SituacoesCanhotosCarga> ConsultarSituacaoCanhotoCarga(int codigoCarga)
        {
            string sql = @"
                SELECT ISNULL(SUBSTRING((SELECT DISTINCT ', ' +
                    CASE
                    WHEN CNF_SITUACAO_CANHOTO = 0 THEN 'Todas'
                    WHEN CNF_SITUACAO_CANHOTO = 1 THEN 'Pendente'
                    WHEN CNF_SITUACAO_CANHOTO = 2 THEN 'Justificado'
                    WHEN CNF_SITUACAO_CANHOTO = 3 THEN 'Recebido Fisicamente'
                    WHEN CNF_SITUACAO_CANHOTO = 4 THEN 'Extraviado'
                    WHEN CNF_SITUACAO_CANHOTO = 5 THEN 'Entregue ao Motorista'
                    WHEN CNF_SITUACAO_CANHOTO = 6 THEN 'Enviado ao Cliente'
                    WHEN CNF_SITUACAO_CANHOTO = 7 THEN 'Recebido pelo Cliente'
                    WHEN CNF_SITUACAO_CANHOTO = 8 THEN 'Cancelado'
                    ELSE 'Nenhum'
                    END  FOR XML PATH('')), 3, 1000), '') Situacao
                    FROM T_CANHOTO_NOTA_FISCAL WHERE CAR_CODIGO = " + codigoCarga.ToString("D");

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Canhoto.SituacoesCanhotosCarga)));
            return query.SetTimeout(300).List<Dominio.ObjetosDeValor.Embarcador.Canhoto.SituacoesCanhotosCarga>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto> ConsultaCanhotosNotificacao(DateTime dataInicial, DateTime dataFinal)
        {
            string sql = @"
                SELECT Empresa.EMP_CODIGO Transportador,
                	   Filial.FIL_CODIGO Filial,
                	   Empresa.EMP_EMAIL_ENVIO_CANHOTO EmailTransportador,
                	   Filial.FIL_EMAIL EmailFilial,
                	   Filial.FIL_CNPJ Estabelecimento,
                	   Cliente.CLI_NOME Cliente,
                	   Canhoto.CNF_NUMERO Numero,
                       Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO SituacaoDigitalizacao,
                       Canhoto.CNF_TIPO_CANHOTO TipoCanhoto,
                       TipoCarga.TCG_DESCRICAO TipoCarga,
                       Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                       Empresa.EMP_RAZAO DescricaoTransportador,
                	   Canhoto.CNF_DATA_EMISSAO DataEmissao,
                       Canhoto.CNF_SERIE Serie,
                       Cliente.CLI_NOME Destinatario
                  FROM T_CANHOTO_NOTA_FISCAL Canhoto
                  JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Canhoto.EMP_CODIGO
                  JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Canhoto.FIL_CODIGO
                  JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Canhoto.CAR_CODIGO
                  LEFT JOIN T_TIPO_DE_CARGA TipoCarga ON TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO
                  LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Canhoto.CLI_CODIGO_DESTINATARIO
                 WHERE Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO <> 3 
                   AND Canhoto.CNF_SITUACAO_CANHOTO = 1
                   AND Canhoto.CNF_DATA_ENVIO_CANHOTO >= '" + dataInicial.ToString("yyyy-MM-dd") + @"'
                   AND Canhoto.CNF_DATA_ENVIO_CANHOTO < '" + dataFinal.ToString("yyyy-MM-dd") + @"'
                   AND Empresa.EMP_STATUS = 'A'
                   AND Carga.CAR_SITUACAO <> 13 
                   AND Carga.CAR_SITUACAO <> 18
                 GROUP BY Empresa.EMP_CODIGO,
                	   Filial.FIL_CODIGO,
                	   Empresa.EMP_EMAIL,
                       Empresa.EMP_EMAIL_ENVIO_CANHOTO,
                	   Filial.FIL_EMAIL,
                	   Filial.FIL_CNPJ,
                	   Cliente.CLI_NOME,
                	   Canhoto.CNF_NUMERO,
                	   Canhoto.CNF_DATA_EMISSAO,
                       Canhoto.CNF_SERIE,
                       Empresa.EMP_RAZAO,
                       Carga.CAR_CODIGO_CARGA_EMBARCADOR,
                       Canhoto.CNF_TIPO_CANHOTO,
                       TipoCarga.TCG_DESCRICAO,
                       Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO,
                       Cliente.CLI_NOME
                 ORDER BY Empresa.EMP_CODIGO, Filial.FIL_CODIGO";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto)));
            return query.SetTimeout(300).List<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto> ConsultaCanhotosNotificacaoPorTipoOperacao(DateTime dataInicial, DateTime dataFinal)
        {
            string sql = @"
                SELECT Empresa.EMP_CODIGO Transportador,
                	   Filial.FIL_CODIGO Filial,
                	   Empresa.EMP_EMAIL_ENVIO_CANHOTO EmailTransportador,
                	   Filial.FIL_EMAIL EmailFilial,
                	   Filial.FIL_CNPJ Estabelecimento,
                	   Cliente.CLI_NOME Cliente,
                	   Canhoto.CNF_NUMERO Numero,
                       Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO SituacaoDigitalizacao,
                       Canhoto.CNF_TIPO_CANHOTO TipoCanhoto,
                       TipoCarga.TCG_DESCRICAO TipoCarga,
                       Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                       Empresa.EMP_RAZAO DescricaoTransportador,
                	   Canhoto.CNF_DATA_EMISSAO DataEmissao,
                       Canhoto.CNF_SERIE Serie,
                       TipoOperacao.TOP_CODIGO CodigoTipoOperacao,
                       Cliente.CLI_NOME Destinatario
                  FROM T_CANHOTO_NOTA_FISCAL Canhoto
                  JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Canhoto.EMP_CODIGO
                  JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Canhoto.FIL_CODIGO
                  JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Canhoto.CAR_CODIGO
                  JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                  JOIN T_CONFIGURACAO_TIPO_OPERACAO_CANHOTO TipoOperacaoConfiguracaoCanhoto ON TipoOperacaoConfiguracaoCanhoto.CNH_CODIGO = TipoOperacao.CNH_CODIGO
                  LEFT JOIN T_TIPO_DE_CARGA TipoCarga ON TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO
                  LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Canhoto.CLI_CODIGO_DESTINATARIO
                 WHERE Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO <> 3 
                   AND Canhoto.CNF_SITUACAO_CANHOTO = 1
                   AND Canhoto.CNF_DATA_ENVIO_CANHOTO >= '" + dataInicial.ToString("yyyy-MM-dd") + @"'
                   AND Canhoto.CNF_DATA_ENVIO_CANHOTO < '" + dataFinal.ToString("yyyy-MM-dd") + @"'
                   AND Empresa.EMP_STATUS = 'A'
                   AND Carga.CAR_SITUACAO <> 13 
                   AND Carga.CAR_SITUACAO <> 18
                   AND TipoOperacaoConfiguracaoCanhoto.CNH_NOTIFICAR_CANHOTOS_PENDENTES = 1
                 GROUP BY Empresa.EMP_CODIGO,
                	   Filial.FIL_CODIGO,
                	   Empresa.EMP_EMAIL,
                       Empresa.EMP_EMAIL_ENVIO_CANHOTO,
                	   Filial.FIL_EMAIL,
                	   Filial.FIL_CNPJ,
                	   Cliente.CLI_NOME,
                	   Canhoto.CNF_NUMERO,
                	   Canhoto.CNF_DATA_EMISSAO,
                       Canhoto.CNF_SERIE,
                       Empresa.EMP_RAZAO,
                       Carga.CAR_CODIGO_CARGA_EMBARCADOR,
                       Canhoto.CNF_TIPO_CANHOTO,
                       TipoCarga.TCG_DESCRICAO,
                       Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO,
                       TipoOperacao.TOP_CODIGO,
                       Cliente.CLI_NOME
                 ORDER BY Empresa.EMP_CODIGO, Filial.FIL_CODIGO";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto)));
            return query.SetTimeout(300).List<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto>();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto> ConsultaCanhotosRejeitadosPorTipoOperacao()
        {

            string sql = $@"
                                             SELECT Empresa.EMP_CODIGO Transportador,
                	   Filial.FIL_CODIGO Filial,
                	   Empresa.EMP_EMAIL EmailTransportador,
                	   Empresa.EMP_EMAIL_ENVIO_CANHOTO EmailEnvioCanhotoTransportador,
                	   Filial.FIL_EMAIL EmailFilial,
                	   Filial.FIL_CNPJ Estabelecimento,
                	   Cliente.CLI_NOME Cliente,
                	   Canhoto.CNF_NUMERO Numero,
                       Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO SituacaoDigitalizacao,
                       Canhoto.CNF_TIPO_CANHOTO TipoCanhoto,
                       TipoCarga.TCG_DESCRICAO TipoCarga,
                       Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                       Empresa.EMP_RAZAO DescricaoTransportador,
                	   Canhoto.CNF_DATA_ULTIMA_MODIFICACAO DataEmissao,
                       Canhoto.CNF_SERIE Serie,
                       TipoOperacao.TOP_CODIGO CodigoTipoOperacao,
                       Canhoto.CNF_SITUACAO_CANHOTO SituacaoCanhoto
                  FROM T_CANHOTO_NOTA_FISCAL Canhoto
                  JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Canhoto.EMP_CODIGO
                  JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Canhoto.FIL_CODIGO
                  JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Canhoto.CAR_CODIGO
                  JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                  JOIN T_CONFIGURACAO_TIPO_OPERACAO_CANHOTO TipoOperacaoConfiguracaoCanhoto ON TipoOperacaoConfiguracaoCanhoto.CNH_CODIGO = TipoOperacao.CNH_CODIGO
                  LEFT JOIN T_TIPO_DE_CARGA TipoCarga ON TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO
                  LEFT JOIN T_CLIENTE Cliente ON Cliente.CLI_CGCCPF = Canhoto.CLI_CODIGO_DESTINATARIO
                 WHERE Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO = 4 
                  AND Canhoto.CNF_DATA_ULTIMA_MODIFICACAO >= '{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}'
                   AND Canhoto.CNF_DATA_ULTIMA_MODIFICACAO <= '{DateTime.Now.ToString("yyyy-MM-dd")}'
				    AND Canhoto.CNF_SITUACAO_CANHOTO = 1
                   AND Carga.CAR_SITUACAO <> 13 
                   AND Carga.CAR_SITUACAO <> 18
                   AND TipoOperacaoConfiguracaoCanhoto.CNH_NOTIFICAR_CANHOTOS_REJEITADOS = 1
                 GROUP BY Empresa.EMP_CODIGO,
                	   Filial.FIL_CODIGO,
                	   Empresa.EMP_EMAIL,
					    Empresa.EMP_EMAIL_ENVIO_CANHOTO,
                	   Filial.FIL_EMAIL,
                	   Filial.FIL_CNPJ,
                	   Cliente.CLI_NOME,
                	   Canhoto.CNF_NUMERO,
                	   Canhoto.CNF_DATA_ULTIMA_MODIFICACAO,
                       Canhoto.CNF_SERIE,
                       Canhoto.CNF_SITUACAO_CANHOTO,
                       Empresa.EMP_RAZAO,
                       Carga.CAR_CODIGO_CARGA_EMBARCADOR,
                       Canhoto.CNF_TIPO_CANHOTO,
                       TipoCarga.TCG_DESCRICAO,
                       Canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO,
                       TipoOperacao.TOP_CODIGO
                 ORDER BY Empresa.EMP_CODIGO, Filial.FIL_CODIGO";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto)));
            return query.SetTimeout(300).List<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto>();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotosComDigitalizacaoRejeitada()
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => obj.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotosComDigitalizacaoRejeitadaEAguardandoRecebimentoFisico()
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(obj => (obj.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada || obj.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BuscarCanhotosComDigitalizacaoRejeitadaEAguardandoRecebimentoFisicoERecebimentoFisicoPendente()
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.Carga == null || (o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada))
                .Where(obj => (obj.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada || obj.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao || obj.SituacaoCanhoto == SituacaoCanhoto.Pendente));

            return query.Distinct().ToList();
        }

        public int ContarNaoPendentesPoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> consultaCanhoto = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(obj => obj.Carga.Codigo == codigoCarga && (obj.SituacaoCanhoto != SituacaoCanhoto.Pendente || obj.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao));

            return consultaCanhoto.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos> ConsultarCanhotoSQLDynamic(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool contarConsulta, Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega)
        {
            var filtrosConsultaCanhoto = ObterFiltrosConsultaCanhoto(filtrosPesquisa, configuracaoQualidadeEntrega);

            string sql = GetSQLConsultaCanhoto(filtrosPesquisa) + GetSQLJoinsConsultaCanhoto(filtrosPesquisa) + filtrosConsultaCanhoto.WhereClause;

            if (parametrosConsulta != null)
            {
                sql += $" order by {(parametrosConsulta.PropriedadeOrdenar.Contains("Descricao") ? parametrosConsulta.PropriedadeOrdenar.Replace("Descricao", "") : parametrosConsulta.PropriedadeOrdenar)} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "asc" : "desc")} ";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }


            ISQLQuery consulta = new SQLDinamico(sql, filtrosConsultaCanhoto.Parametros).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Canhoto.ConsultaCanhotos>();
        }

        public int ContarConsultaCanhotoSQLDynamic(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega)
        {
            string sql = "SELECT COUNT (1) FROM T_CANHOTO_NOTA_FISCAL as CanhotoNotaFiscal";

            var filtrosConsultaCanhoto = ObterFiltrosConsultaCanhoto(filtrosPesquisa, configuracaoQualidadeEntrega);

            sql += GetSQLJoinsConsultaCanhoto(filtrosPesquisa) + filtrosConsultaCanhoto.WhereClause;

            ISQLQuery consulta = new SQLDinamico(sql, filtrosConsultaCanhoto.Parametros).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.DataConfirmacaoCanhoto> ConsultaDataConfirmacaoCanhotos(List<int> codigosCanhotos)
        {
            string sql = @"select
	                           CargaEntrega.CEN_CODIGO CodigoCargaEntrega,
                               CanhotoNotaFiscal.CNF_CODIGO CodigoCanhoto,
	                           CargaEntrega.CEN_DATA_ENTREGA DataConfirmacao
                           from 
	                           T_CARGA Carga
	                           join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO
	                           join T_CANHOTO_NOTA_FISCAL CanhotoNotaFiscal on CanhotoNotaFiscal.CAR_CODIGO = Carga.CAR_CODIGO
                           where
	                           CanhotoNotaFiscal.CNF_CODIGO in (:codigosCanhotos)
                               and CargaEntrega.CEN_DATA_ENTREGA is not null
	                           and CargaEntrega.CEN_COLETA = 0";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameterList("codigosCanhotos", codigosCanhotos);

            return query
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Canhoto.DataConfirmacaoCanhoto)))
                .List<Dominio.ObjetosDeValor.Embarcador.Canhoto.DataConfirmacaoCanhoto>();
        }

        public void AtualizarEmpresaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<int> codigosPedidos, Dominio.Entidades.Empresa transportador)
        {
            if (codigosPedidos.Count == 0)
                return;

            string sql = @" UPDATE T_CANHOTO_NOTA_FISCAL SET CAR_CODIGO = :codigoCarga, EMP_CODIGO = :codigoEmpresa WHERE PED_CODIGO in (:codigosPedidos)";

            ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameterList("codigosPedidos", codigosPedidos);
            query.SetParameter("codigoCarga", carga.Codigo);
            query.SetParameter("codigoEmpresa", transportador.Codigo);

            query.ExecuteUpdate();
        }

        #endregion

        #region Métodos Públicos - Relatórios

        #region Métodos Assíncronos

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.Canhoto>> ConsultarRelatorioCanhotoAsync(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            ISQLQuery consultaCanhoto = new ConsultaCanhoto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCanhoto.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.Canhoto)));

            return await consultaCanhoto.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.Canhoto>();
        }
        #endregion

        public int ContarConsultaRelatorioCanhoto(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            ISQLQuery consultaCanhoto = new ConsultaCanhoto().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCanhoto.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.Canhoto> ConsultarRelatorioCanhoto(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            ISQLQuery consultaCanhoto = new ConsultaCanhoto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCanhoto.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.Canhoto)));

            return consultaCanhoto.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.Canhoto>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> MontarQuery(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            //query = query.Where(obj => (obj.TipoCanhoto == TipoCanhoto.CTeSubcontratacao && obj.CTeSubcontratacao.Ativo) ||
            //                           (obj.TipoCanhoto == TipoCanhoto.NFe && obj.XMLNotaFiscal.nfAtiva) ||
            //                           (obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso.Ativo) ||
            //                           (obj.TipoCanhoto == TipoCanhoto.CTe && obj.CargaCTe.CTe.Status == "A"));

            query = query.Where(obj => obj.Carga.CargaFechada == true);

            if (!filtro.Malote.HasValue || filtro.Malote.Value == 0)
            {
                query = query.Where(obj => (obj.TipoCanhoto == TipoCanhoto.CTeSubcontratacao && obj.CTeSubcontratacao.Ativo) ||
                                 (obj.TipoCanhoto == TipoCanhoto.NFe && obj.XMLNotaFiscal.nfAtiva) ||
                                 (obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso.Ativo) ||
                                 (obj.TipoCanhoto == TipoCanhoto.CTe && obj.CargaCTe.CTe.Status == "A"));
            }
            else
            {
                query = query.Where(obj => (obj.TipoCanhoto == TipoCanhoto.CTeSubcontratacao) ||
                                 (obj.TipoCanhoto == TipoCanhoto.NFe) ||
                                 (obj.TipoCanhoto == TipoCanhoto.Avulso) ||
                                 (obj.TipoCanhoto == TipoCanhoto.CTe));
            }

            if (filtro.OrigemDigitalizacao > 0)
                query = query.Where(obj => obj.OrigemDigitalizacao == filtro.OrigemDigitalizacao);

            if (filtro.Situacao.HasValue && filtro.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Todas)
                query = query.Where(obj => obj.SituacaoCanhoto == filtro.Situacao);

            if (filtro.Situacoes?.Count > 0)
                query = query.Where(obj => filtro.Situacoes.Contains(obj.SituacaoCanhoto));

            if (filtro.SituacaoDigitalizacaoCanhoto.HasValue && filtro.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Todas)
                query = query.Where(obj => obj.SituacaoDigitalizacaoCanhoto == filtro.SituacaoDigitalizacaoCanhoto);

            if (filtro.SituacoesDigitalizacaoCanhoto?.Count > 0)
                query = query.Where(obj => filtro.SituacoesDigitalizacaoCanhoto.Contains(obj.SituacaoDigitalizacaoCanhoto));

            if (filtro.SituacaoPgtoCanhoto.HasValue && filtro.SituacaoPgtoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Todas)
                query = query.Where(obj => obj.SituacaoPgtoCanhoto == filtro.SituacaoPgtoCanhoto);

            if (filtro.Motorista > 0)
                query = query.Where(obj => obj.MotoristasResponsaveis.Any(o => o.Codigo == filtro.Motorista));

            if (filtro.Pessoa > 0)
                query = query.Where(obj => obj.Emitente.CPF_CNPJ == filtro.Pessoa);

            if (filtro.NumeroCanhoto > 0)
                query = query.Where(obj => obj.Numero == filtro.NumeroCanhoto);

            if (filtro.GrupoPessoa > 0)
                query = query.Where(obj => obj.Emitente.GrupoPessoas.Codigo == filtro.GrupoPessoa);

            if (!string.IsNullOrEmpty(filtro.Chave))
                query = query.Where(obj => obj.XMLNotaFiscal.Chave.Equals(filtro.Chave));

            if (filtro.TipoCanhoto.HasValue && filtro.TipoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Todos)
                query = query.Where(obj => obj.TipoCanhoto == filtro.TipoCanhoto);

            if (filtro.NumeroNFe > 0)
            {
                if (filtro.TipoCanhoto == TipoCanhoto.Avulso)
                    query = query.Where(canhoto => canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Any(notaCanhotoAvulso => notaCanhotoAvulso.XMLNotaFiscal.Numero == filtro.NumeroNFe));
                else if (filtro.TipoCanhoto == TipoCanhoto.NFe)
                    query = query.Where(canhoto => canhoto.XMLNotaFiscal.Numero == filtro.NumeroNFe);
                else if (filtro.TipoCanhoto == TipoCanhoto.Todos)
                    query = query.Where(canhoto => canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Any(notaCanhotoAvulso => notaCanhotoAvulso.XMLNotaFiscal.Numero == filtro.NumeroNFe) || canhoto.XMLNotaFiscal.Numero == filtro.NumeroNFe);
            }

            if (filtro.Filiais != null && filtro.Filiais.Any(codigo => codigo == -1))
                query = query.Where(obj => filtro.Filiais.Contains(obj.Filial.Codigo) || obj.Carga.Pedidos.Any(ped => ped.Recebedor != null && filtro.Recebedores.Contains(ped.Recebedor.CPF_CNPJ)));
            else if (filtro.Filiais?.Count > 0)
                query = query.Where(obj => filtro.Filiais.Contains(obj.Filial.Codigo));

            if (filtro.ObrigatorioFilial)
                query = query.Where(obj => obj.Filial != null);

            if (filtro.Empresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == filtro.Empresa);

            if (filtro.Empresas?.Count > 0)
                query = query.Where(obj => filtro.Empresas.Contains(obj.Empresa.Codigo));

            if (filtro.Terceiro > 0)
                query = query.Where(obj => obj.TerceiroResponsavel.Codigo == filtro.Terceiro);

            if (filtro.Numeros?.Count > 0)
                query = query.Where(obj => filtro.Numeros.Contains(obj.Codigo));

            if (filtro.Numero > 0)
                query = query.Where(obj => obj.Numero == filtro.Numero);

            if (!string.IsNullOrEmpty(filtro.CodigoCargaEmbarcador))
                query = query.Where(obj => obj.Carga.CodigoCargaEmbarcador == filtro.CodigoCargaEmbarcador);

            if (filtro.CodigosCargaEmbarcador?.Count > 0)
                query = query.Where(obj => filtro.CodigosCargaEmbarcador.Contains(obj.Carga.Codigo));

            if (filtro.Carga != 0)
                query = query.Where(o => o.Carga.Codigo == filtro.Carga);

            if (filtro.DataInicio != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Date >= filtro.DataInicio);

            if (filtro.DataFim != DateTime.MinValue)
                query = query.Where(obj => obj.DataEmissao.Date <= filtro.DataFim);

            if (filtro.DataInicioDigitalizacao.HasValue || filtro.DataFimDigitalizacao.HasValue)
            {
                query = query.Where(obj => obj.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao);

                if (filtro.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (filtro.DataInicioDigitalizacao.HasValue)
                        query = query.Where(obj => obj.DataDigitalizacao.Value.Date >= filtro.DataInicioDigitalizacao.Value);

                    if (filtro.DataFimDigitalizacao.HasValue)
                        query = query.Where(obj => obj.DataDigitalizacao.Value.Date <= filtro.DataFimDigitalizacao.Value);
                }
                else
                {
                    if (filtro.DataInicioDigitalizacao.HasValue)
                        query = query.Where(obj => obj.DataEnvioCanhoto.Date >= filtro.DataInicioDigitalizacao.Value);

                    if (filtro.DataFimDigitalizacao.HasValue)
                        query = query.Where(obj => obj.DataEnvioCanhoto.Date <= filtro.DataFimDigitalizacao.Value);
                }
            }

            if (filtro.CodigosConhecimentos != null && filtro.CodigosConhecimentos.Count > 0)
            {
                if (filtro.TipoCanhoto.HasValue && filtro.TipoCanhoto != TipoCanhoto.CTe && filtro.TipoCanhoto != TipoCanhoto.Todos)
                    query = query.Where(obj => obj.XMLNotaFiscal.CTEs.Any(o => filtro.CodigosConhecimentos.Contains(o.Codigo)));
                else if (filtro.TipoCanhoto.HasValue && filtro.TipoCanhoto == TipoCanhoto.CTe)
                    query = query.Where(obj => filtro.CodigosConhecimentos.Contains(obj.CargaCTe.CTe.Codigo));
                else
                    query = query.Where(obj => obj.XMLNotaFiscal.CTEs.Any(o => filtro.CodigosConhecimentos.Contains(o.Codigo)) || filtro.CodigosConhecimentos.Contains(obj.CargaCTe.CTe.Codigo));
            }
            else if (filtro.CodigoCTe > 0)
                query = query.Where(obj => obj.XMLNotaFiscal.CTEs.Any(o => o.Codigo == filtro.CodigoCTe) || obj.CargaCTe.CTe.Codigo == filtro.CodigoCTe);

            if (filtro.CodigosCanhotos != null && filtro.CodigosCanhotos.Count > 0)
                query = query.Where(o => filtro.CodigosCanhotos.Contains(o.Codigo));

            if (filtro.CodigoLocalArmazenamento > 0)
                query = query.Where(o => o.LocalArmazenamentoCanhoto.Codigo == filtro.CodigoLocalArmazenamento);

            if (filtro.TiposCarga?.Count > 0)
                query = query.Where(o => filtro.TiposCarga.Contains(o.Carga.TipoDeCarga.Codigo));

            if (filtro.TiposOperacao?.Count > 0)
                query = query.Where(o => filtro.TiposOperacao.Contains(o.Carga.TipoOperacao.Codigo));

            if (filtro.Recebedor > 0d)
                query = query.Where(o => o.XMLNotaFiscal.Recebedor.CPF_CNPJ == filtro.Recebedor);

            if (filtro.Destinatario?.Count > 0)
                query = query.Where(o => filtro.Destinatario.Contains(o.Destinatario.CPF_CNPJ));

            if (filtro.Pacote > 0)
                query = query.Where(o => o.PacoteArmazenado == filtro.Pacote);

            if (filtro.Posicao > 0)
                query = query.Where(o => o.PosicaoNoPacote == filtro.Posicao);

            if (filtro.Serie > 0)
                query = query.Where(o => o.Serie == filtro.Serie.ToString());

            if (filtro.SemMalote)
            {
                query = query.Where(o => o.Malote == null || o.Malote.Situacao == SituacaoMaloteCanhoto.Inconsistente);
                //var queryMalote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.MaloteCanhoto>();
                //var resultMalote = from o in queryMalote where o.Malote.Situacao != SituacaoMaloteCanhoto.Cancelado select o.Canhoto;
                //result = result.Where(o => !resultMalote.Contains(o));
            }

            if (filtro.Malote.HasValue)
            {
                IQueryable<Dominio.Entidades.Embarcador.Canhotos.MaloteCanhoto> queryMalote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.MaloteCanhoto>();

                queryMalote = queryMalote.Where(o => o.Malote.Codigo == filtro.Malote.Value);

                query = query.Where(o => queryMalote.Select(m => m.Canhoto).Contains(o));
            }

            if (filtro.DataEmissaoCTeInicial.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.CTEs.Any(c => c.DataEmissao.Value.Date >= filtro.DataEmissaoCTeInicial.Value.Date));

            if (filtro.DataEmissaoCTeFinal.HasValue)
                query = query.Where(o => o.XMLNotaFiscal.CTEs.Any(c => c.DataEmissao.Value.Date <= filtro.DataEmissaoCTeFinal.Value.Date));

            if (filtro.Usuario > 0)
                query = query.Where(o => o.Usuario.Codigo == filtro.Usuario);

            if (filtro.DataInicioEnvio.HasValue)
                query = query.Where(o => o.DataEnvioCanhoto.Date >= filtro.DataInicioEnvio.Value.Date);

            if (filtro.DataFimEnvio.HasValue)
                query = query.Where(o => o.DataEnvioCanhoto.Date <= filtro.DataFimEnvio.Value.Date);

            if (filtro.TipoLocalPrestacao != TipoLocalPrestacao.todos)
            {
                if (filtro.TipoLocalPrestacao == TipoLocalPrestacao.intraMunicipal)
                    query = query.Where(o => o.XMLNotaFiscal.CTEs.Any(c => c.Remetente.Localidade.Codigo == c.Destinatario.Localidade.Codigo));
                else if (filtro.TipoLocalPrestacao == TipoLocalPrestacao.interMunicipal)
                    query = query.Where(o => o.XMLNotaFiscal.CTEs.Any(c => c.Remetente.Localidade.Codigo != c.Destinatario.Localidade.Codigo));
            }


            if (filtro.CodigosVeiculo?.Count > 0)
                query = query.Where(o => filtro.CodigosVeiculo.Contains(o.Carga.Veiculo.Codigo) || o.Carga.VeiculosVinculados.Any(v => filtro.CodigosVeiculo.Contains(v.Codigo)));

            if (filtro.SituacoesCarga?.Count > 0)
            {
                if (filtro.SituacoesCarga.Count == 1 && filtro.SituacoesCarga.Contains(SituacaoCarga.NaLogistica))
                {
                    query = query.Where(o => (o.Carga.SituacaoCarga != SituacaoCarga.Encerrada &&
                                              o.Carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                                              o.Carga.SituacaoCarga != SituacaoCarga.LiberadoPagamento) || o.Carga.AgConfirmacaoUtilizacaoCredito);
                }
                else
                    query = query.Where(o => filtro.SituacoesCarga.Contains(o.Carga.SituacaoCarga));
            }

            if (filtro.SituacoesCargaMercante?.Count > 0)
            {
                Expression<Func<Dominio.Entidades.Embarcador.Canhotos.Canhoto, bool>> predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Cancelada))
                    predicateOr = predicateOr.Or(obj => obj.Carga.SituacaoCarga == SituacaoCarga.Cancelada);

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Anulada))
                    predicateOr = predicateOr.Or(obj => obj.Carga.SituacaoCarga == SituacaoCarga.Anulada);

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.AguardandoEmissao))
                    predicateOr = predicateOr.Or(obj => obj.Carga.SituacaoCarga == SituacaoCarga.AgNFe && obj.Carga.DataRecebimentoUltimaNFe.HasValue);

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteEmissaoCTe))
                    predicateOr = predicateOr.Or(obj => obj.Carga.SituacaoCarga == SituacaoCarga.AgNFe && !obj.Carga.DataRecebimentoUltimaNFe.HasValue);

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMDFe))
                    predicateOr = predicateOr.Or(obj => ((bool?)obj.Carga.MDFeAquaviarioVinculado ?? false) == false && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                        obj.Carga.Pedidos.Any(ped => ped.TipoCobrancaMultimodal == TipoCobrancaMultimodal.CTEAquaviario));

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMercante))
                    predicateOr = predicateOr.Or(obj => ((bool?)obj.Carga.TodosCTesComMercante ?? false) == false && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                        obj.Carga.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro || ped.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalProprio));

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteFaturamento))
                    predicateOr = predicateOr.Or(obj => ((bool?)obj.Carga.TodosCTesFaturados ?? false) == false && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                        obj.Carga.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorta || ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorta ||
                            ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder));

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoCTe))
                    predicateOr = predicateOr.Or(obj => obj.Carga.SituacaoCarga == SituacaoCarga.AgIntegracao);

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoFatura))
                    predicateOr = predicateOr.Or(obj => ((bool?)obj.Carga.TodosCTesFaturadosIntegrados ?? false) == false && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                        obj.Carga.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorta || ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorta ||
                            ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder));

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteSVM))
                {
                    IQueryable<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal> querySVM = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>();
                    predicateOr = predicateOr.Or(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                        obj.Carga.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorta) &&
                        obj.Carga.CargaCTes.Any(c => !querySVM.Any(s => s.CTeSVM.Status == "A" && s.CTeMultimodal.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && s.CTeMultimodal.Codigo == c.CTe.Codigo)));
                }

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.ComErro))
                    predicateOr = predicateOr.Or(obj => obj.Carga.SituacaoCarga == SituacaoCarga.AgIntegracao || obj.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos || obj.Carga.problemaCTE);// || obj.PossuiPendencia

                if (filtro.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Finalizada))
                {
                    predicateOr = predicateOr.Or(obj => obj.Carga.SituacaoCarga == SituacaoCarga.Encerrada &&
                        (obj.Carga.TodosCTesComMercante || !obj.Carga.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro)) &&

                        (obj.Carga.TodosCTesComManifesto || !obj.Carga.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro)) &&

                        (obj.Carga.TodosCTesFaturados || !obj.Carga.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorta || ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorta ||
                                ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto || ped.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder)) &&

                        (obj.Carga.MDFeAquaviarioVinculado || !obj.Carga.Pedidos.Any(ped => ped.TipoCobrancaMultimodal == TipoCobrancaMultimodal.CTEAquaviario))
                        );
                }

                query = query.Where(predicateOr);
            }

            if (filtro.SituacoesNotaFiscal?.Count > 0)
            {
                query = query.Where(obj => filtro.SituacoesNotaFiscal.Contains(obj.XMLNotaFiscal.SituacaoEntregaNotaFiscal));
            }

            if (!string.IsNullOrEmpty(filtro.NumeroDocumentoOriginario))
                query = query.Where(o => o.NumeroDocumentoOriginario.Contains(filtro.NumeroDocumentoOriginario));


            if (!string.IsNullOrEmpty(filtro.EscritorioVendas))
            {
                List<string> listaEscritorios = filtro.EscritorioVendas.Split(',').ToList();

                IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> queryClienteComplementar = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>();
                queryClienteComplementar = queryClienteComplementar.Where(cliente => listaEscritorios.Contains(cliente.EscritorioVendas));

                query = query.Where(o => queryClienteComplementar.Select(cliente => cliente.Cliente.Codigo).Any(codigo => codigo == o.Destinatario.Codigo));
            }


            if (!string.IsNullOrEmpty(filtro.Matriz))
            {
                List<string> listaEscritorios = filtro.EscritorioVendas.Split(',').ToList();

                IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> queryClienteComplementar = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>();
                queryClienteComplementar = queryClienteComplementar.Where(cliente => listaEscritorios.Contains(cliente.Matriz));

                query = query.Where(o => queryClienteComplementar.Select(cliente => cliente.Cliente.Codigo).Any(codigo => codigo == o.Destinatario.Codigo));
            }

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ConsultarParaVinculo(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhotoParaVinculo filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> consultaCanhoto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                .Where(canhoto => canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao);

            if (filtrosPesquisa.Numero > 0)
                consultaCanhoto = consultaCanhoto.Where(canhoto => canhoto.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.CodigoEntrega > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> consultaCargaEntregaNotaFiscal = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>()
                    .Where(notaEntrega => notaEntrega.CargaEntrega.Codigo == filtrosPesquisa.CodigoEntrega);

                consultaCanhoto = consultaCanhoto.Where(canhoto =>
                    consultaCargaEntregaNotaFiscal.Any(notaEntrega =>
                        (canhoto.Carga.Codigo == notaEntrega.CargaEntrega.Carga.Codigo) &&
                        (
                            (canhoto.TipoCanhoto == TipoCanhoto.NFe && canhoto.XMLNotaFiscal.Codigo == notaEntrega.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo) ||
                            (canhoto.TipoCanhoto == TipoCanhoto.Avulso && canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Any(notaCanhotoAvulso => notaCanhotoAvulso.Codigo == notaEntrega.PedidoXMLNotaFiscal.Codigo))
                        )
                    )
                );
            }

            return consultaCanhoto;
        }

        private string RetornarOrigemDestino(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, int origemDestino)
        {
            if (origemDestino == 1)
                return canhoto.Carga.Pedidos.Count() > 0 ? canhoto.Carga.Pedidos.FirstOrDefault() != null ? canhoto.Carga.Pedidos.FirstOrDefault().Origem != null ? !string.IsNullOrEmpty(canhoto.Carga.Pedidos.FirstOrDefault().Origem.Descricao) ? canhoto.Carga.Pedidos.FirstOrDefault().Origem.Descricao : "" : "" : "" : "";
            else
                return canhoto.Carga.Pedidos.Count() > 0 ? canhoto.Carga.Pedidos.FirstOrDefault() != null ? canhoto.Carga.Pedidos.FirstOrDefault().Destino != null ? !string.IsNullOrEmpty(canhoto.Carga.Pedidos.FirstOrDefault().Destino.Descricao) ? canhoto.Carga.Pedidos.FirstOrDefault().Destino.Descricao : "" : "" : "" : "";
        }

        private string GetSQLConsultaCanhoto(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa)
        {
            /* Essa consulta abaixo está dividida em secções justamente pra saber onde que fica cada propriedade de qual classe.
             * Se for inserir nova propriedade pelo amor de Santo Deus, pegue e coloque no lugar certo.
             * Os Joins Possuem local correto para adição, por favor adicione no Método getSQLJoinsConsultaCanhoto()
             */

            string selectMotorista = @"SUBSTRING((SELECT ', ' + motorista1.FUN_NOME + 
                                                    (CASE WHEN motorista1.FUN_FONE is null or motorista1.FUN_FONE = '' THEN '' 
                                                          ELSE ' (' + motorista1.FUN_FONE  + ')' END) 
                                                    FROM T_CARGA_MOTORISTA motoristaCarga1 
                                                    INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO 
                                                    WHERE motoristaCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) Motorista";
            if (filtrosPesquisa.Motorista > 0)
            {
                selectMotorista = @"SUBSTRING((SELECT ', ' + motorista1.FUN_NOME + " +
                    "(CASE WHEN motorista1.FUN_FONE is null or motorista1.FUN_FONE = '' THEN '' " +
                    "ELSE ' (' + motorista1.FUN_FONE  + ')' END)" +
                    " FROM T_CARGA_MOTORISTA motoristaCarga1  " +
                    "INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO " +
                    $"WHERE motoristaCarga1.CAR_CODIGO = Carga.CAR_CODIGO and motorista1.FUN_CODIGO = {filtrosPesquisa.Motorista} FOR XML PATH('')), 3, 1000) Motorista";

            }

            return $@"                
                SELECT 
                    LocalArmazenamentoCanhoto.LAC_CODIGO CodigoLocalArmazenamento,
                    LocalArmazenamentoCanhoto.LAC_DESCRICAO DescricaoLocalArmazenamento,
                    CanhotoNotaFiscal.CNF_DATA_ENVIO_CANHOTO DataEnvioCanhoto,
                    CanhotoNotaFiscal.CNF_SITUACAO_CANHOTO SituacaoCanhoto,
                    CanhotoNotaFiscal.CNF_SITUACAO_DIGITALIZACAO_CANHOTO SituacaoDigitalizacaoCanhoto,
                    CanhotoNotaFiscal.CNF_DATA_DIGITALIZACAO DataDigitalizacao,
                    COALESCE(
                        CanhotoNotaFiscal.CNF_DATA_ENTREGA_NOTA_CLIENTE,
		                 (select CargaEntrega.CEN_DATA_ENTREGA
                         from
                             T_CARGA_ENTREGA_NOTA_FISCAL CargaEntregaNotaFiscal
                         inner join
                             T_CARGA_ENTREGA CargaEntrega
                                 on CargaEntrega.CEN_CODIGO = CargaEntregaNotaFiscal.CEN_CODIGO
                         inner join
                             T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal
                                 on PedidoXmlNotaFiscal.PNF_CODIGO = CargaEntregaNotaFiscal.PNF_CODIGO
                         inner join
                             T_XML_NOTA_FISCAL XMLNotaFiscalInterno
                                 on XMLNotaFiscalInterno.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO
                         where
                             CargaEntrega.CAR_CODIGO = CanhotoNotaFiscal.CAR_CODIGO
                             and XMLNotaFiscalInterno.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO
                             and CargaEntrega.CEN_COLETA = 0
                             ORDER BY CURRENT_TIMESTAMP OFFSET 0 ROWS FETCH FIRST 1 ROWS ONLY)) AS DataEntregaCliente,
                    CanhotoNotaFiscal.CNF_CODIGO Codigo,
                    CanhotoNotaFiscal.CNF_TIPO_CANHOTO TipoCanhoto,
                    CanhotoNotaFiscal.CNF_NUMERO Numero,
                    CanhotoNotaFiscal.CNF_SERIE Serie,
                    CanhotoNotaFiscal.CNF_DATA_EMISSAO DataEmissao,
                    CanhotoNotaFiscal.CNF_NOME_ARQUIVO NomeArquivo,
                    CanhotoNotaFiscal.CNF_PACOTE_ARMAZENADO PacoteArmazenado,
                    CanhotoNotaFiscal.CNF_POSICAO_NO_PACOTE PosicaoNoPacote,
                    CanhotoNotaFiscal.CNF_NOME_ARQUIVO GuidNomeArquivo,
                    CanhotoNotaFiscal.CNF_GUID_NOME_ARQUIVO NomeArquivo,
                    CanhotoNotaFiscal.CNF_VALOR Valor,
                    CanhotoNotaFiscal.CNF_OBSERVACAO Observacao,
                    CanhotoNotaFiscal.CNF_NUMERO_DOCUMENTO_ORIGINARIO NumeroDocumentoOriginario,
                    CanhotoNotaFiscal.CNF_SITUACAO_PGTO_CANHOTO SituacaoPgtoCanhoto,
                    CanhotoNotaFiscal.CNF_NUMERO_PROTOCOLO NumeroProtocolo,
                    CanhotoNotaFiscal.CNF_DIGITALIZACAO_INTEGRADA DigitalizacaoIntegrada,
                    CanhotoNotaFiscal.CNF_VALIDACAO_CANHOTO ValidacaoCanhoto,
                    CanhotoNotaFiscal.CNF_ORIGEM_SITUACAO_DIGITALIZACAO_CANHOTO OrigemSituacaoDigitalizacaoCanhoto,
                    CanhotoNotaFIscal.CNF_QUANTIDADE_ENVIO_DIGITALIZACAO_CANHOTO QuantidadeEnvioDigitalizacaoCanhoto,

                    CanhotoNotaFiscal.CNF_POSSUI_INTEGRACAO_COMPROVEI PossuiIntegracaoComprovei,
                    CanhotoNotaFiscal.CNF_VALIDACAO_CANHOTO ValidacaoCanhotoComprovei,
                    CanhotoNotaFiscal.CNF_VALIDACAO_NUMERO ValidacaoNumeroComprovei,
                    CanhotoNotaFiscal.CNF_VALIDACAO_ENCONTROU_DATA ValidacaoEncontrouDataComprovei,
                    CanhotoNotaFiscal.CNF_VALIDACAO_ASSINATURA ValidacaoAssinaturaComprovei,

                    XMLNotaFiscal.NF_CHAVE Chave,
                    XMLNotaFiscal.NF_DATA_EMISSAO DataNotaFiscal,
                    XMLNotaFiscal.NF_SITUACAO_ENTREGA SituacaoNotaFiscal,
                    XMLNotaFiscal.NF_TIPO_NOTA_FISCAL_INTEGRADA TipoNotaFiscalIntegrada,
                    CTeTerceiro.CPS_CHAVE_ACESSO ChaveAcessoCTeTerceiroVinculoCanhoto,
                    Filial.FIL_CODIGO CodigoFilial,
                    Filial.FIL_DESCRICAO Filial,
                    Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                    {selectMotorista},
                    Carga.CAR_SITUACAO SituacaoCarga,
                    ((SELECT vei.VEI_PLACA from T_VEICULO vei WHERE vei.VEI_CODIGO = CAR_VEICULO) + ISNULL(
                        (SELECT ', ' + veiculo1.VEI_PLACA 
                        FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 
                        INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO 
                        WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')) Veiculo,
                    SUBSTRING((SELECT DISTINCT ', ' + CONVERT(NVARCHAR(20), _localidade.LOC_DESCRICAO) 
                                FROM T_CARGA_PEDIDO _cargaPedido
                                JOIN T_LOCALIDADES _localidade ON _localidade.LOC_CODIGO = _cargaPedido.LOC_CODIGO_ORIGEM
                                WHERE _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) Origem,
					SUBSTRING((SELECT DISTINCT ', ' + CONVERT(NVARCHAR(20), _localidade.LOC_DESCRICAO) 
                                FROM T_CARGA_PEDIDO _cargaPedido
                                JOIN T_LOCALIDADES _localidade ON _localidade.LOC_CODIGO = _cargaPedido.LOC_CODIGO_DESTINO
                                WHERE _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) Destino,

                    TipoDeCarga.TCG_DESCRICAO TipoCarga,
                    Emitente.CLI_CGCCPF CPFCNPJEmitente,
                    Emitente.CLI_NOME NomeEmitente,
                    Emitente.CLI_FISJUR TipoEmitente,
                    Destinatario.CLI_NOME Destinatario,
                    Destinatario.CLI_CGCCPF CNPJDestinatario,
                    Destinatario.CLI_FISJUR TipoDestinatario,
                    Destinatario.CLI_OBRIGAR_INFORMAR_DATA_ENTREGA_CLIENTE_AO_BAIXAR_CANHOTOS ObrigarInformarDataEntregaClienteAoBaixarCanhotos,
                    Empresa.EMP_RAZAO Empresa,
                    TipoOperacao.TOP_DESCRICAO CentroResultadoCarga,
	                CASE WHEN CanhotoNotaFiscal.CNF_TIPO_CANHOTO = 4
		                 THEN CAST(CanhotoNotaFiscal.CNF_NUMERO as NVARCHAR(20))
                         ELSE CTeSubQuery.NumeroCTe 
                    END AS NumeroCTe,
                    (SELECT TOP 1 ClienteComplementar.CLC_ESCRITORIO_VENDAS FROM T_CARGA_PEDIDO CargaPedido
		                    JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		                    JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO
		                    JOIN T_CLIENTE_COMPLEMENTAR ClienteComplementar ON ClienteComplementar.CLI_CODIGO = DestinatarioPedido.CLI_CGCCPF
	                    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                          AND CargaPedido.CPE_CODIGO = CanhotoNotaFiscal.CPE_CODIGO
                    ) AS EscritorioVendasComplementar,
                    (SELECT TOP 1 ClienteComplementar.CLC_MATRIZ FROM T_CARGA_PEDIDO CargaPedido
		                    JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		                    JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO
		                    JOIN T_CLIENTE_COMPLEMENTAR ClienteComplementar ON ClienteComplementar.CLI_CODIGO = DestinatarioPedido.CLI_CGCCPF
	                    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                          AND CargaPedido.CPE_CODIGO = CanhotoNotaFiscal.CPE_CODIGO
                    ) AS MatrizComplementar,
                    (SELECT TOP 1 GrupoPessoaFaturaCanhoto.GRP_HABILITAR FROM T_GRUPO_PESSOAS_FATURA_CANHOTO AS GrupoPessoaFaturaCanhoto
		                WHERE GrupoPessoaFaturaCanhoto.GRP_CODIGO = Emitente.GRP_CODIGO) AS EnvioCanhotoFaturaHabilitado,
                    (SELECT top 1 CanhotoIntegracao.INT_SITUACAO_INTEGRACAO from T_CANHOTO_INTEGRACAO CanhotoIntegracao
                        JOIN T_TIPO_INTEGRACAO TipoIntegracao ON TipoIntegracao.TPI_CODIGO = CanhotoIntegracao.TPI_CODIGO and TPI_TIPO = {(int)TipoIntegracao.Comprovei}
                        WHERE CanhotoIntegracao.CNF_CODIGO = CanhotoNotaFiscal.CNF_CODIGO order by CanhotoIntegracao.INT_DATA_INTEGRACAO desc ) AS TipoSituacaoIA,   
                    ConfChamado.CCH_FINALIZAR_AUTOMATICAMENTE_ATENDIMENTO_NFE_ENTREGUE as CancelarAtendimentoAutomaticamente

                FROM T_CANHOTO_NOTA_FISCAL as CanhotoNotaFiscal
            ";
        }

        private string GetSQLJoinsConsultaCanhoto(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa)
        {
            /* Abaixo estão os JOIN'S que serão fixos, tenha cuidado ao mexer nessa consulta
             */

            return $@"
                    LEFT JOIN T_LOCAL_ARMAZENAMENTO_CANHOTO AS LocalArmazenamentoCanhoto ON LocalArmazenamentoCanhoto.LAC_CODIGO = CanhotoNotaFiscal.LAC_CODIGO 
                    LEFT JOIN T_XML_NOTA_FISCAL AS XMLNotaFiscal ON XMLNotaFiscal.NFX_CODIGO = CanhotoNotaFiscal.NFX_CODIGO
                    LEFT JOIN T_CTE_TERCEIRO AS CTeTerceiro ON CTeTerceiro.CPS_CODIGO = CanhotoNotaFiscal.CPS_CODIGO
                    LEFT JOIN T_FILIAL AS Filial ON Filial.FIL_CODIGO = CanhotoNotaFiscal.FIL_CODIGO
                    LEFT JOIN T_CARGA AS Carga ON Carga.CAR_CODIGO = CanhotoNotaFiscal.CAR_CODIGO
                    LEFT JOIN T_CLIENTE AS Emitente ON Emitente.CLI_CGCCPF = CanhotoNotaFiscal.CLI_CODIGO_EMITENTE
                    LEFT JOIN T_CLIENTE AS Destinatario ON Destinatario.CLI_CGCCPF = CanhotoNotaFiscal.CLI_CODIGO_DESTINATARIO
                    LEFT JOIN T_EMPRESA AS Empresa ON Empresa.EMP_CODIGO = CanhotoNotaFiscal.EMP_CODIGO
                    LEFT JOIN T_TIPO_OPERACAO AS TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                    LEFT JOIN T_CANHOTO_AVULSO AS CanhotoAvulso ON CanhotoAvulso.CAV_CODIGO = CanhotoNotaFiscal.CAV_CODIGO
                    LEFT JOIN T_CARGA_CTE AS CargaCTe ON CargaCTe.CCT_CODIGO = CanhotoNotaFiscal.CCT_CODIGO
                    LEFT JOIN T_CTE AS CTeCargaCTeCanhoto ON CTeCargaCTeCanhoto.CON_CODIGO = CargaCTe.CON_CODIGO
                    LEFT JOIN T_GRUPO_PESSOAS AS GrupoPessoas ON GrupoPessoas.GRP_CODIGO = Emitente.GRP_CODIGO
                    LEFT JOIN T_CLIENTE AS TerceiroResponsavel ON TerceiroResponsavel.CLI_CGCCPF = CanhotoNotaFiscal.CLI_CGCCPF_TERCEIRO_RESPONSAVEL
                    LEFT JOIN T_MALOTE_CANHOTO AS MaloteCanhoto ON MaloteCanhoto.MCA_CODIGO = CanhotoNotaFiscal.MCA_CODIGO
                    LEFT JOIN T_TIPO_DE_CARGA As TipoDeCarga on TipoDeCarga.TCG_CODIGO = Carga.TCG_CODIGO 
                    LEFT JOIN T_CONFIGURACAO_TIPO_OPERACAO_CHAMADO ConfChamado on ConfChamado.CCH_CODIGO = TipoOperacao.CCH_CODIGO
                    OUTER APPLY (SELECT STUFF((
                                    SELECT DISTINCT ', ' + CONVERT(NVARCHAR(20), CTeSub.CON_NUM) AS [text()]
                                    FROM T_CTE_XML_NOTAS_FISCAIS CTeXMLNotasFiscaisSub
                                    JOIN T_CTE CTeSub ON CTeSub.con_codigo = CTeXMLNotasFiscaisSub.CON_CODIGO
                                    WHERE CanhotoNotaFiscal.NFX_CODIGO = CTeXMLNotasFiscaisSub.NFX_CODIGO
                                    AND CTeSub.CON_STATUS NOT IN ('C','I','Z') FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') as NumeroCTe
                                ) as CTeSubQuery 
            ";
        }

        private (string WhereClause, List<ParametroSQL> Parametros) ObterFiltrosConsultaCanhoto(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega)
        {
            /* Aqui abaixo fica a secção de Filtros da Consulta de Canhoto
             * Se você for fazer alguma consulta aqui que precise de JOIN, lembre que ele tem local pra fazer corretamente.
             */
            string pattern = "yyyy-MM-dd";
            string filtro = "WHERE 1 = 1 ";

            var parametros = new List<ParametroSQL>();

            if (!filtrosPesquisa.ExibirCanhotosSemVinculoComCarga)
                filtro += " AND carga.CAR_CARGA_FECHADA = 1 ";

            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            if (!filtrosPesquisa.Malote.HasValue || filtrosPesquisa.Malote.Value == 0)
            {
                filtro += @" AND ((CanhotoNotaFiscal.CNF_TIPO_CANHOTO = 3 AND CTeTerceiro.CPS_ATIVO = 1) 
                            OR (CanhotoNotaFiscal.CNF_TIPO_CANHOTO = 1 AND XMLNotaFiscal.NF_ATIVA = 1) 
                            OR (CanhotoNotaFiscal.CNF_TIPO_CANHOTO = 2 AND CanhotoAvulso.CAV_ATIVO = 1) 
                            OR (CanhotoNotaFiscal.CNF_TIPO_CANHOTO = 4 AND CTeCargaCTeCanhoto.CON_STATUS = 'A'))";
            }
            else
            {
                filtro += @" AND (CanhotoNotaFiscal.CNF_TIPO_CANHOTO = 3 
                            OR CanhotoNotaFiscal.CNF_TIPO_CANHOTO = 1
                            OR CanhotoNotaFiscal.CNF_TIPO_CANHOTO = 2 
                            OR CanhotoNotaFiscal.CNF_TIPO_CANHOTO = 4)";
            }

            if (filtrosPesquisa.OrigemDigitalizacao > 0)
                filtro += $" AND CanhotoNotaFiscal.CNF_ORIGEM_DIGITALIZACAO = {filtrosPesquisa.OrigemDigitalizacao}";

            if (filtrosPesquisa.Situacao.HasValue && filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Todas)
                filtro += $" AND CanhotoNotaFiscal.CNF_SITUACAO_CANHOTO = {filtrosPesquisa.Situacao}";

            if (filtrosPesquisa.Situacoes?.Count > 0)
                filtro += $" AND CanhotoNotaFiscal.CNF_SITUACAO_CANHOTO in ({string.Join(", ", filtrosPesquisa.Situacoes.Select(o => o.ToString("D")))})";

            if (filtrosPesquisa.SituacoesDigitalizacaoCanhoto?.Count > 0)
                filtro += $" AND CanhotoNotaFiscal.CNF_SITUACAO_DIGITALIZACAO_CANHOTO in ({string.Join(", ", filtrosPesquisa.SituacoesDigitalizacaoCanhoto.Select(o => o.ToString("D")))})";

            if (filtrosPesquisa.SituacaoPgtoCanhoto.HasValue && filtrosPesquisa.SituacaoPgtoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Todas)
                filtro += $" AND CanhotoNotaFiscal.CNF_SITUACAO_PGTO_CANHOTO = {(int)filtrosPesquisa.SituacaoPgtoCanhoto}";

            if (filtrosPesquisa.Motorista > 0)
                filtro += $" and EXISTS(SELECT TOP 1 1 FROM T_CARGA_MOTORISTA motoristaCarga1\r\n\t\t\t\tINNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO\r\n\t\t\t\tWHERE motoristaCarga1.CAR_CODIGO = Carga.CAR_CODIGO\r\n\t\t\t\tAND motorista1.FUN_CODIGO = {filtrosPesquisa.Motorista})"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Pessoa > 0)
                filtro += $" AND Emitente.CLI_CGCCPF = {filtrosPesquisa.Pessoa}";

            if (filtrosPesquisa.NumeroCanhoto > 0)
                filtro += $" AND CanhotoNotaFiscal.CNF_NUMERO = {filtrosPesquisa.NumeroCanhoto}";

            if (filtrosPesquisa.GrupoPessoa > 0)
                filtro += $" AND GrupoPessoas.GRP_CODIGO = {filtrosPesquisa.GrupoPessoa}";

            if (!string.IsNullOrEmpty(filtrosPesquisa.Chave))
                filtro += $" AND XMLNotaFiscal.NF_CHAVE like '{filtrosPesquisa.Chave}'";

            if (filtrosPesquisa.TipoCanhoto.HasValue && filtrosPesquisa.TipoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Todos)
                filtro += $" AND CanhotoNotaFiscal.CNF_TIPO_CANHOTO = {(int)filtrosPesquisa.TipoCanhoto}";

            if (filtrosPesquisa.NumeroNFe > 0)
            {
                if (filtrosPesquisa.TipoCanhoto == TipoCanhoto.Avulso)
                {
                    filtro += $@" AND EXISTS (
                    SELECT top 1 1
                    FROM T_XML_NOTA_FISCAL XMLNotaFiscalA
					JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal ON PedidoXmlNotaFiscal.NFX_CODIGO = XMLNotaFiscalA.NFX_CODIGO
					JOIN T_CANHOTO_AVULSO_PEDIDO_XML_NOTA_FISCAL CanhotoAvulsoPedidoXMLNotaFiscal ON CanhotoAvulsoPedidoXMLNotaFiscal.PNF_CODIGO = PedidoXmlNotaFiscal.PNF_CODIGO
			        WHERE CanhotoAvulsoPedidoXMLNotaFiscal.CAV_CODIGO = CanhotoAvulso.CAV_CODIGO AND XMLNotaFiscalA.NF_NUMERO = {filtrosPesquisa.NumeroNFe}) ";

                }
                else if (filtrosPesquisa.TipoCanhoto == TipoCanhoto.Todos)
                {
                    filtro += $@" AND ( EXISTS (
                    SELECT top 1 1
                    FROM T_XML_NOTA_FISCAL XMLNotaFiscalA
					JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal ON PedidoXmlNotaFiscal.NFX_CODIGO = XMLNotaFiscalA.NFX_CODIGO
					JOIN T_CANHOTO_AVULSO_PEDIDO_XML_NOTA_FISCAL CanhotoAvulsoPedidoXMLNotaFiscal ON CanhotoAvulsoPedidoXMLNotaFiscal.PNF_CODIGO = PedidoXmlNotaFiscal.PNF_CODIGO
			        WHERE CanhotoAvulsoPedidoXMLNotaFiscal.CAV_CODIGO = CanhotoAvulso.CAV_CODIGO AND XMLNotaFiscalA.NF_NUMERO = {filtrosPesquisa.NumeroNFe} ) OR XMLNotaFiscal.NF_NUMERO = {filtrosPesquisa.NumeroNFe}) ";
                }
                else
                {
                    filtro += $" AND XMLNotaFiscal.NF_NUMERO = {filtrosPesquisa.NumeroNFe}";
                }
            }

            if (filtrosPesquisa.Filiais != null && filtrosPesquisa.Filiais.Exists(codigo => codigo == -1))
                filtro += $@" AND (CanhotoNotaFiscal.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.Filiais)}) or EXISTS (
                     select top 1 1 from T_CARGA_PEDIDO _CargaPedido
                     where _CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _CargaPedido.CLI_CODIGO_RECEBEDOR in ({string.Join(", ", filtrosPesquisa.Recebedores)})
                 ))";
            else if (filtrosPesquisa.Filiais?.Count > 0)
                filtro += $@" AND CanhotoNotaFiscal.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.Filiais)}) ";

            if (filtrosPesquisa.ObrigatorioFilial)
                filtro += $" AND CanhotoNotaFiscal.FIL_CODIGO > 0";

            if (filtrosPesquisa.Empresa > 0)
                filtro += $" AND Empresa.EMP_CODIGO = {filtrosPesquisa.Empresa}";

            if (filtrosPesquisa.Empresas?.Count > 0)
                filtro += $" AND Empresa.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.Empresas)})";

            if (filtrosPesquisa.Terceiro > 0)
                filtro += $" AND TerceiroResponsavel.CLI_CGCCPF = {filtrosPesquisa.Terceiro}";

            if (filtrosPesquisa.Numeros?.Count > 0)
                filtro += $" AND CanhotoNotaFiscal.CNF_CODIGO in ({string.Join(", ", filtrosPesquisa.Numeros)})";

            if (filtrosPesquisa.Numero > 0)
                filtro += $" AND CanhotoNotaFiscal.CNF_NUMERO = {filtrosPesquisa.Numero}";

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoCargaEmbarcador))
                filtro += $" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = {filtrosPesquisa.CodigoCargaEmbarcador}";

            if (filtrosPesquisa.CodigosCargaEmbarcador?.Count > 0)
                filtro += $@" AND Carga.CAR_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosCargaEmbarcador)})";

            if (filtrosPesquisa.Carga > 0)
                filtro += $@" AND Carga.CAR_CODIGO = {filtrosPesquisa.Carga}";

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                filtro += $" AND CAST(CanhotoNotaFiscal.CNF_DATA_EMISSAO AS DATE) >= '{filtrosPesquisa.DataInicio.ToString(pattern)}'";

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                filtro += $" AND CAST(CanhotoNotaFiscal.CNF_DATA_EMISSAO AS DATE) <= '{filtrosPesquisa.DataFim.ToString(pattern)}'";

            if (filtrosPesquisa.DataInicioDigitalizacao.HasValue || filtrosPesquisa.DataFimDigitalizacao.HasValue)
            {
                if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (filtrosPesquisa.DataInicioDigitalizacao.HasValue)
                        filtro += $" AND CanhotoNotaFiscal.CNF_DATA_DIGITALIZACAO >= '{filtrosPesquisa.DataInicioDigitalizacao.Value.ToString(pattern)}'";

                    if (filtrosPesquisa.DataFimDigitalizacao.HasValue)
                        filtro += $" AND CanhotoNotaFiscal.CNF_DATA_DIGITALIZACAO <= '{filtrosPesquisa.DataFimDigitalizacao.Value.AddDays(1).ToString(pattern)}'";
                }
                else
                {
                    if (filtrosPesquisa.DataInicioDigitalizacao.HasValue)
                        filtro += $" AND CanhotoNotaFiscal.CNF_DATA_ENVIO_CANHOTO >= '{filtrosPesquisa.DataInicioDigitalizacao.Value.ToString(pattern)}'";

                    if (filtrosPesquisa.DataFimDigitalizacao.HasValue)
                        filtro += $" AND CanhotoNotaFiscal.CNF_DATA_ENVIO_CANHOTO <= '{filtrosPesquisa.DataFimDigitalizacao.Value.AddDays(1).ToString(pattern)}'";
                }
            }

            if (filtrosPesquisa.CodigosConhecimentos != null && filtrosPesquisa.CodigosConhecimentos.Count > 0)
            {
                if (filtrosPesquisa.TipoCanhoto.HasValue && filtrosPesquisa.TipoCanhoto != TipoCanhoto.CTe && filtrosPesquisa.TipoCanhoto != TipoCanhoto.Todos)
                    filtro += $" AND EXISTS (select top 1 1 from T_CTE_XML_NOTAS_FISCAIS _CTeXmlNotaFiscal where _CTeXmlNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO and _CTeXmlNotaFiscal.CON_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosConhecimentos)}))"; // SQL-INJECTION-SAFE

                else if (filtrosPesquisa.TipoCanhoto.HasValue && filtrosPesquisa.TipoCanhoto == TipoCanhoto.CTe)
                    filtro += $" AND EXISTS (select top 1 1 from T_CARGA_CTE _CargaCTe where _CargaCTe.CCT_CODIGO = CanhotoNotaFiscal.CCT_CODIGO and _CargaCTe.CON_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosConhecimentos)}))"; // SQL-INJECTION-SAFE
                else
                    filtro += $@" AND (EXISTS (select top 1 1 from T_CARGA_CTE _CargaCTe where _CargaCTe.CCT_CODIGO = CanhotoNotaFiscal.CCT_CODIGO and _CargaCTe.CON_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosConhecimentos)}))
                                   OR EXISTS (select top 1 1 from T_CTE_XML_NOTAS_FISCAIS _CTeXmlNotaFiscal where _CTeXmlNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO and _CTeXmlNotaFiscal.CON_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosConhecimentos)})))";
            }
            else if (filtrosPesquisa.CodigoCTe > 0)
                filtro += $@" AND (EXISTS (select top 1 1 from T_CARGA_CTE _CargaCTe where _CargaCTe.CCT_CODIGO = CanhotoNotaFiscal.CCT_CODIGO and _CargaCTe.CON_CODIGO = {filtrosPesquisa.CodigoCTe})
                                   OR EXISTS (select top 1 1 from T_CTE_XML_NOTAS_FISCAIS _CTeXmlNotaFiscal where _CTeXmlNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO and _CTeXmlNotaFiscal.CON_CODIGO = {filtrosPesquisa.CodigoCTe}))";

            if (filtrosPesquisa.CodigosCanhotos != null && filtrosPesquisa.CodigosCanhotos.Count > 0)
                filtro += $" AND CanhotoNotaFiscal.CNF_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosCanhotos)})";

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
                filtro += $" AND CanhotoNotaFiscal.LAC_CODIGO = {filtrosPesquisa.CodigoLocalArmazenamento}";

            if (filtrosPesquisa.TiposCarga?.Count > 0)
                filtro += $" AND Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.TiposCarga)})";

            if (filtrosPesquisa.TiposOperacao?.Count > 0)
                filtro += $" AND Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.TiposOperacao)})";

            if (filtrosPesquisa.Recebedor > 0d)
                filtro += $@" AND XMLNotaFiscal.CLI_CODIGO_RECEBEDOR = {filtrosPesquisa.Recebedor}";

            if (filtrosPesquisa.Destinatario?.Count > 0)
                filtro += $" AND Destinatario.CLI_CGCCPF in ({string.Join(", ", filtrosPesquisa.Destinatario)})";

            if (filtrosPesquisa.Pacote > 0)
                filtro += $" AND CanhotoNotaFiscal.CNF_PACOTE_ARMAZENADO = {filtrosPesquisa.Pacote}";

            if (filtrosPesquisa.Posicao > 0)
                filtro += $" AND CanhotoNotaFiscal.CNF_POSICAO_NO_PACOTE = {filtrosPesquisa.Posicao}";

            if (filtrosPesquisa.Serie > 0)
                filtro += $" AND CanhotoNotaFiscal.CNF_SERIE = '{filtrosPesquisa.Serie}'";

            if (filtrosPesquisa.SemMalote)
                filtro += $" AND (CanhotoNotaFiscal.MCA_CODIGO = NULL OR MaloteCanhoto.MCA_SITUACAO = {SituacaoMaloteCanhoto.Inconsistente})";

            if (filtrosPesquisa.Malote.HasValue)
                filtro += $" AND EXISTS (select top 1 1 from T_MALOTE_CANHOTO_CANHOTO _MaloteCanhoto where _MaloteCanhoto.MCA_CODIGO = {filtrosPesquisa.Malote.Value} and CanhotoNotaFiscal.CNF_CODIGO = _MaloteCanhoto.CNF_CODIGO)"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.DataEmissaoCTeInicial.HasValue)
                filtro += $" AND EXISTS (select top 1 1 from T_CTE_XML_NOTAS_FISCAIS _CTeXmlNotaFiscal left join T_CTE _CTe on _CTe.CON_CODIGO = _CTeXmlNotaFiscal.CON_CODIGO where _CTeXmlNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO and _CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoCTeInicial.Value.ToString(pattern)}'"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.DataEmissaoCTeFinal.HasValue)
                filtro += $" AND EXISTS (select top 1 1 from T_CTE_XML_NOTAS_FISCAIS _CTeXmlNotaFiscal left join T_CTE _CTe on _CTe.CON_CODIGO = _CTeXmlNotaFiscal.CON_CODIGO where _CTeXmlNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO and _CTe.CON_DATAHORAEMISSAO <= '{filtrosPesquisa.DataEmissaoCTeFinal.Value.ToString(pattern)}'"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Usuario > 0)
                filtro += $" AND CanhotoNotaFiscal.FUN_CODIGO = {filtrosPesquisa.Usuario} ";

            if (filtrosPesquisa.DataInicioEnvio.HasValue)
                filtro += $" AND CanhotoNotaFiscal.CNF_DATA_ENVIO_CANHOTO >= '{filtrosPesquisa.DataInicioEnvio.Value.ToString(pattern)}'";

            if (filtrosPesquisa.DataFimEnvio.HasValue)
                filtro += $" AND CanhotoNotaFiscal.CNF_DATA_ENVIO_CANHOTO <= '{filtrosPesquisa.DataFimEnvio.Value.ToString(pattern)}'";

            if (filtrosPesquisa.TipoLocalPrestacao != TipoLocalPrestacao.todos)
            {
                if (filtrosPesquisa.TipoLocalPrestacao == TipoLocalPrestacao.intraMunicipal)
                    filtro += $@" AND EXISTS (select top 1 1 from T_CTE_XML_NOTAS_FISCAIS _CTeXmlNotaFiscal
                            left join T_CTE _CTe on _CTe.CON_CODIGO = _CTeXmlNotaFiscal.CON_CODIGO 
                            left join T_CTE_PARTICIPANTE _Destinatario on _Destinatario.PCT_CODIGO = _CTe.CON_DESTINATARIO_CTE
                            left join T_CTE_PARTICIPANTE _Remetente on _Remetente.PCT_CODIGO = _CTe.CON_REMETENTE_CTE
                            where _CTeXmlNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO and _Destinatario.LOC_CODIGO = _Remetente.LOC_CODIGO)";

                else if (filtrosPesquisa.TipoLocalPrestacao == TipoLocalPrestacao.interMunicipal)
                    filtro += $@" AND EXISTS (select top 1 1 from T_CTE_XML_NOTAS_FISCAIS _CTeXmlNotaFiscal
                            left join T_CTE _CTe on _CTe.CON_CODIGO = _CTeXmlNotaFiscal.CON_CODIGO 
                            left join T_CTE_PARTICIPANTE _Destinatario on _Destinatario.PCT_CODIGO = _CTe.CON_DESTINATARIO_CTE
                            left join T_CTE_PARTICIPANTE _Remetente on _Remetente.PCT_CODIGO = _CTe.CON_REMETENTE_CTE
                            where _CTeXmlNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO and _Destinatario.LOC_CODIGO != _Remetente.LOC_CODIGO)";
            }

            if (filtrosPesquisa.CodigosVeiculo?.Count > 0)
                filtro += $@" AND (Carga.CAR_VEICULO in ({string.Join(", ", filtrosPesquisa.CodigosVeiculo)}) or EXISTS 
                            (select top 1 1 from T_CARGA_VEICULOS_VINCULADOS _CargaVeiculo
                            where _CargaVeiculo.CAR_CODIGO = Carga.CAR_CODIGO and _CargaVeiculo.VEI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosVeiculo)})))";

            if (filtrosPesquisa.SituacoesCargaMercante?.Count > 0)
            {
                filtro += $" AND (";

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.Cancelada))
                    filtro += $" Carga.CAR_SITUACAO = 13 OR";

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.Anulada))
                    filtro += $" Carga.CAR_SITUACAO = 11 OR";

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.AguardandoEmissao))
                    filtro += $" (Carga.CAR_SITUACAO = 5 AND Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is not null) OR";

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.PendenteEmissaoCTe))
                    filtro += $" (Carga.CAR_SITUACAO = 5 AND Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is null) OR";

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.PendenteMDFe))
                    filtro += $" (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO = 1 AND Carga.CAR_SITUACAO not in (18, 13) and exists (select top 1 1 from T_CARGA_PEDIDO _CargaPedido where _CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _CargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5)) OR";

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.PendenteMercante))
                    filtro += $@" (Carga.CAR_TODOS_CTES_COM_MERCANTE = 0 AND Carga.CAR_SITUACAO not in (18, 13) and exists 
                                    (select top 1 1 from T_CARGA_PEDIDO _CargaPedido where _CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and (_CargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 5 OR _CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL in (4, 5)))) OR";

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.PendenteFaturamento))
                    filtro += $@" (Carga.CAR_TODOS_CTES_FATURADOS = 0 AND Carga.CAR_SITUACAO not in (18, 13) and exists 
                                    (select top 1 1 from T_CARGA_PEDIDO _CargaPedido where _CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and (_CargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL in (1, 3, 4) OR _CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))) OR";

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.PendenteIntegracaoCTe))
                    filtro += $" Carga.CAR_SITUACAO = 15 OR";


                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.PendenteIntegracaoFatura))
                    filtro += $@" (Carga.CAR_TODOS_CTES_FATURADOS_INTEGRADOS = 0 AND Carga.CAR_SITUACAO not in (18, 13) and exists 
                                    (select top 1 1 from T_CARGA_PEDIDO _CargaPedido where _CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and (_CargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL in (1, 3, 4) OR _CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))) OR";

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.PendenteSVM))
                {
                    filtro += $@" (Carga.CAR_SITUACAO not in (18, 13) and exists 
                                    (select top 1 1 from T_CARGA_PEDIDO _CargaPedido where _CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _CargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3) and not exists 
                                    (select top 1 1 from T_CARGA_CTE _CargaCTe
                                    left join T_CTE_SVM_MULTIMODAL _CTeSVMMultiModal on _CTeSVMMultiModal.CON_CODIGO_MULTIMODAL = _CargaCTe.CON_CODIGO
                                    left join T_CTE _CTeSVM on _CTeSVMMultiModal.CON_CODIGO_SVM = _CargaCTe.CON_CODIGO
                                    left join T_CTE _CTeMultiModal on _CTeSVMMultiModal.CON_CODIGO_MULTIMODAL = _CargaCTe.CON_CODIGO
                                    where _CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and _CTeSVM.CON_STATUS = 'A' and _CTeMultiModal.CON_TIPO_CTE = 0)
                                    ) OR";

                    //var querySVM = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>();
                    //predicateOr = predicateOr.Or(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    //obj.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    //    obj.Carga.Pedidos.Any(ped => ped.ModalPropostaMultimodal == ModalPropostaMultimodal.PortaPorta) &&
                    //    obj.Carga.CargaCTes.Any(c => !querySVM.Any(s => s.CTeSVM.Status == "A" && s.CTeMultimodal.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && s.CTeMultimodal.Codigo == c.CTe.Codigo)));

                }

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.ComErro))
                    filtro += $" Carga.CAR_SITUACAO in (15, 6) OR Carga.CAR_PROBLEMA_CTE = 1 OR";

                if (filtrosPesquisa.SituacoesCargaMercante.Exists(o => o == SituacaoCargaMercante.Finalizada))
                {
                    filtro += $@" (Carga.CAR_SITUACAO = 11 and
                                  (Carga.CAR_TODOS_CTES_COM_MERCANTE = 1 or not exists (select top 1 1 from T_CARGA_PEDIDO _CargaPedido where _CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and (_CargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4))) and
                                  (Carga.CAR_TODOS_CTES_COM_MANIFESTO = 1 or not exists (select top 1 1 from T_CARGA_PEDIDO _CargaPedido where _CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and (_CargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4))) and
                                  (Carga.CAR_TODOS_CTES_FATURADOS = 1 or not exists (select top 1 1 from T_CARGA_PEDIDO _CargaPedido where _CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and (_CargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL in (1, 3, 4) or _CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))) and
                                  (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO = 1 or not exists (select top 1 1 from T_CARGA_PEDIDO _CargaPedido where _CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _CargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))
                    ) OR";
                }

                if (filtro.Length >= 2 && filtro.EndsWith("OR"))
                    filtro = filtro.Substring(0, filtro.Length - 2);

                filtro += $")";
            }

            if (filtrosPesquisa.SituacoesNotaFiscal?.Count > 0)
                filtro += $" AND XMLNotaFiscal.NF_SITUACAO_ENTREGA in ({string.Join(", ", filtrosPesquisa.SituacoesNotaFiscal.Select(x => (int)x).ToList())})";

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroDocumentoOriginario))
                filtro += $" AND CanhotoNotaFiscal.CNF_NUMERO_DOCUMENTO_ORIGINARIO like '%{filtrosPesquisa.NumeroDocumentoOriginario}%'";

            if (!filtrosPesquisa.Malote.HasValue || filtrosPesquisa.Malote.Value == 0 && !(filtrosPesquisa.CodigosCargaEmbarcador?.Count > 0))
                filtro += $" AND (CanhotoNotaFiscal.CAR_CODIGO is null or (Carga.CAR_SITUACAO <> 13 and Carga.CAR_SITUACAO <> 18 {(filtrosPesquisa.ExibirCanhotosSemVinculoComCarga ? ($" and Carga.CAR_CARGA_FECHADA = 1") : "")}))";

            if (filtrosPesquisa.ClienteComplementar?.Count > 0)
                filtro += @$"AND EXISTS (
	                            SELECT 1 FROM T_CARGA_PEDIDO CargaPedido
		                            JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		                            JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO
	                            WHERE Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO AND DestinatarioPedido.CLI_CGCCPF IN ({string.Join(", ", filtrosPesquisa.ClienteComplementar)})
                            )"; // SQL-INJECTION-SAFE

            filtro += " AND (CanhotoNotaFiscal.CNF_OCULTO = 0 OR CanhotoNotaFiscal.CNF_OCULTO IS NULL)";

            if (filtrosPesquisa.TipoNotaFiscalIntegrada.HasValue && filtrosPesquisa.TipoNotaFiscalIntegrada != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.Todos)
                filtro += $" AND XMLNotaFiscal.NF_TIPO_NOTA_FISCAL_INTEGRADA = {(int)filtrosPesquisa.TipoNotaFiscalIntegrada}";

            if (filtrosPesquisa.ValidacaoCanhoto.HasValue)
                filtro += $" AND CanhotoNotaFiscal.CNF_VALIDACAO_CANHOTO = {(filtrosPesquisa.ValidacaoCanhoto.Value ? 0 : 1)}";

            if (filtrosPesquisa.TipoSituacaoIA.HasValue)
                filtro += @$" AND (SELECT top 1 CanhotoIntegracao.INT_SITUACAO_INTEGRACAO from T_CANHOTO_INTEGRACAO CanhotoIntegracao
                                JOIN T_TIPO_INTEGRACAO TipoIntegracao ON TipoIntegracao.TPI_CODIGO = CanhotoIntegracao.TPI_CODIGO and TPI_TIPO = {(int)TipoIntegracao.Comprovei}
                                WHERE CanhotoIntegracao.CNF_CODIGO = CanhotoNotaFiscal.CNF_CODIGO order by CanhotoIntegracao.INT_DATA_INTEGRACAO desc) = {(int)filtrosPesquisa.TipoSituacaoIA.Value}"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.TipoRejeicaoPelaIA?.Count > 0)
            {
                Dictionary<TipoRejeicaoPelaIA, string> filtrosMapeados = new Dictionary<TipoRejeicaoPelaIA, string>
                    {
                        { TipoRejeicaoPelaIA.Comprovante, "CanhotoNotaFiscal.CNF_VALIDACAO_CANHOTO = 0" },
                        { TipoRejeicaoPelaIA.NumeroDoc, "CanhotoNotaFiscal.CNF_VALIDACAO_NUMERO = 0" },
                        { TipoRejeicaoPelaIA.Data, "CanhotoNotaFiscal.CNF_VALIDACAO_ENCONTROU_DATA = 0" },
                        { TipoRejeicaoPelaIA.Assinatura, "CanhotoNotaFiscal.CNF_VALIDACAO_ASSINATURA = 0" }
                    };

                List<string> condicoes = filtrosPesquisa.TipoRejeicaoPelaIA
                    .Where(filtrosMapeados.ContainsKey)
                    .Select(filtro => filtrosMapeados[filtro])
                    .ToList();

                if (condicoes.Any())
                {
                    filtro += $" AND ({string.Join(" OR ", condicoes)})";
                }
            }

            if (filtrosPesquisa.DigitalizacaoIntegrada.HasValue && (configuracaoQualidadeEntrega?.VerificarDataConfirmacaoIntervaloRaio ?? false))
                filtro += $" AND CanhotoNotaFiscal.CNF_DIGITALIZACAO_INTEGRADA = {(filtrosPesquisa.DigitalizacaoIntegrada.Value ? 0 : 1)} and CanhotoNotaFiscal.CNF_DISPONIVEL_PARA_CONSULTA = 1 and CanhotoNotaFiscal.NFX_CODIGO is not null";
            else if (filtrosPesquisa.DigitalizacaoIntegrada.HasValue && !(configuracaoQualidadeEntrega?.VerificarDataConfirmacaoIntervaloRaio ?? false))
                filtro += $" AND CanhotoNotaFiscal.CNF_DIGITALIZACAO_INTEGRADA = {(filtrosPesquisa.DigitalizacaoIntegrada.Value ? 0 : 1)} and CanhotoNotaFiscal.NFX_CODIGO is not null";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EscritorioVendas))
            {
                List<string> splitEscritorioVenda = filtrosPesquisa.EscritorioVendas.Split(',').Select(m => m.Trim()).ToList();

                filtro += @$" AND EXISTS (
	                            SELECT 1 FROM T_CARGA_PEDIDO CargaPedido
		                        INNER JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		                        INNER JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO
                                INNER JOIN T_CLIENTE_COMPLEMENTAR ClienteComplementar ON ClienteComplementar.CLI_CODIGO = DestinatarioPedido.CLI_CGCCPF
	                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                                AND (ClienteComplementar.CLC_ESCRITORIO_VENDAS like '%{filtrosPesquisa.EscritorioVendas}%' 
                                    OR ClienteComplementar.CLC_ESCRITORIO_VENDAS IN (:CLIENTECOMPLEMENTAR_CLC_MATRIZ)))";

                parametros.Add(new ParametroSQL("CLIENTECOMPLEMENTAR_CLC_MATRIZ", splitEscritorioVenda));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Matriz))
            {
                List<string> splitMatriz = filtrosPesquisa.Matriz.Split(',').Select(m => m.Trim()).ToList();

                filtro += @$" AND EXISTS (
	                            SELECT 1 FROM T_CARGA_PEDIDO CargaPedido
		                        INNER JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		                        INNER JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO
                                INNER JOIN T_CLIENTE_COMPLEMENTAR ClienteComplementar ON ClienteComplementar.CLI_CODIGO = DestinatarioPedido.CLI_CGCCPF
	                            WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                                AND (ClienteComplementar.CLC_MATRIZ like '%{filtrosPesquisa.Matriz}%' 
                                    OR ClienteComplementar.CLC_MATRIZ IN (:CLIENTECOMPLEMENTAR_CLC_MATRIZ)))";

                parametros.Add(new ParametroSQL("CLIENTECOMPLEMENTAR_CLC_MATRIZ", splitMatriz));
            }

            return (filtro, parametros);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> BaseCanhotosDigitalizadoseAgAprovacao(bool apenasLiberadoPgto, int empresaFilialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.Canhoto> cargasTodosCanhotosOK = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> queryCargaEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            query = query.Where(obj => !obj.DigitalizacaoIntegrada && obj.XMLNotaFiscal != null);

            List<SituacaoDigitalizacaoCanhoto> situacoesLiberadas = new List<SituacaoDigitalizacaoCanhoto>() { SituacaoDigitalizacaoCanhoto.Digitalizado, SituacaoDigitalizacaoCanhoto.AgAprovocao };
            query = query.Where(obj => situacoesLiberadas.Contains(obj.SituacaoDigitalizacaoCanhoto));

            if (apenasLiberadoPgto)
            {
                query = query.Where(obj => obj.SituacaoPgtoCanhoto == SituacaoPgtoCanhoto.Liberado);

                //Seleção dos CTes sem canhotos pendente pagamento
                IQueryable<Dominio.Entidades.ConhecimentoDeTransporteEletronico> queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                queryCTes = queryCTes.Where(o => !o.XMLNotaFiscais.Any(x => x.Canhoto.SituacaoPgtoCanhoto != SituacaoPgtoCanhoto.Liberado));

                //Seleção das notas dos CTes 
                IQueryable<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> queryNotas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                queryNotas = queryNotas.Where(obj => queryCTes.Any(c => c.XMLNotaFiscais.Any(o => o.Codigo == obj.Codigo)));

                //Seleção das canhotos das notas 
                query = query.Where(obj => (!obj.Carga.TipoOperacao.RetornarCanhotoQuandoTodasNotasDoCTeEstiveremConformadasPagamento) ||
                                           (obj.Carga.TipoOperacao.RetornarCanhotoQuandoTodasNotasDoCTeEstiveremConformadasPagamento &&
                                            queryNotas.Any(n => n.Codigo == obj.XMLNotaFiscal.Codigo)));

            }

            if (empresaFilialEmissora > 0)
                query = query.Where(obj => obj.EmpresaFilialEmissora.Codigo == empresaFilialEmissora);

            //#17673: Validação para não retornar canhotos que fazem parte de uma carga que contenha canhotos em situação diferente de Ag.Aprovação ou Digitalizados.
            query = query
                .Where(obj =>
                    !cargasTodosCanhotosOK
                        .Where(canhoto1 =>
                            !situacoesLiberadas.Contains(canhoto1.SituacaoDigitalizacaoCanhoto) &&
                            canhoto1.Carga.Codigo == obj.Carga.Codigo
                        )
                        .Any()
                );

            //Validação para existir carga entrega, e que nenhuma carga entrega esteja em aberto.
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesEntregaPendente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaHelper.ObterListaSituacaoEntregaEmAberto();
            query = query.Where(canhoto => queryCargaEntrega.Where(entrega => entrega.Carga.Codigo == canhoto.Carga.Codigo).Any() &&
                                           !queryCargaEntrega.Where(entrega => entrega.Carga.Codigo == canhoto.Carga.Codigo && situacoesEntregaPendente.Contains(entrega.Situacao)).Any());

            return query;
        }
        #endregion
    }
}