using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Canhotos
{
    public class CanhotoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao>
    {
        #region Construtores

        public CanhotoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CanhotoIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        public Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao> BuscarPorCodigoSituacaoFalhaIntegracao(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao>();
            var result = query
                .Where(obj => codigos.Contains(obj.Codigo) &&
                             obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                .OrderByDescending(obj => obj.DataIntegracao)
                .ToList();

            return result
                .GroupBy(obj => obj.Codigo)
                .Select(group => group.First())
                .ToList();
        }

        public Task<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao> BuscarPorCanhotoETipoIntegracaoAsync(int canhoto, int tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao>()
                .Where(obj => obj.Canhoto.Codigo == canhoto && obj.TipoIntegracao.Codigo == tipoIntegracao);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao>> BuscarCanhotoIntegracaoPendenteAsync(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao>();

            var result = from obj in query
                         where obj.TipoIntegracao.TipoEnvio == tipoEnvio &&
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                        (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                         obj.NumeroTentativas < tentativasLimite &&
                         obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos))
                         select obj;


            if (result.Any() && configuracaoEmbarcador.ValidarSituacaoEnvioProgramadoIntegracaoCanhoto)
            {
                var subquery = from envioProgramado in this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>()
                               where envioProgramado.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado &&
                                     envioProgramado.Carga != null
                               select envioProgramado.Carga.Codigo;

                result = result.Where(x => x.Canhoto != null && x.Canhoto.Carga != null && subquery.Contains(x.Canhoto.Carga.Codigo));
            }

            return result
                .Fetch(obj => obj.Canhoto)
                .ThenFetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.Canhoto)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToListAsync(cancellationToken);
        }

        public DateTime BuscarDataEntregaCliente(int codigoCanhoto)
        {
            string sql = @$"select CargaEntrega.CEN_DATA_ENTREGA
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
                                XMLNotaFiscalInterno.NFX_CODIGO = {codigoCanhoto}
                                and CargaEntrega.CEN_COLETA = 0
                                ORDER BY CURRENT_TIMESTAMP OFFSET 0 ROWS FETCH FIRST 1 ROWS ONLY"; // SQL-INJECTION-SAFE

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            return consulta.UniqueResult<DateTime>();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao> _Consultar(Dominio.ObjetosDeValor.Embarcador.CanhotoIntegracao.FiltroPesquisaCanhotoIntegracao filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao>();

            var result = from obj in query select obj;

            // Filtros
            if (filtroPesquisa.Situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == filtroPesquisa.Situacao);

            if (filtroPesquisa.DataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataIntegracao >= filtroPesquisa.DataInicio);

            if (filtroPesquisa.DataFim != DateTime.MinValue)
                result = result.Where(o => o.DataIntegracao <= filtroPesquisa.DataFim);

            if (filtroPesquisa.Emitente > 0)
                result = result.Where(o => o.Canhoto.Emitente.CPF_CNPJ == filtroPesquisa.Emitente);

            if (filtroPesquisa.Carga > 0)
                result = result.Where(o => o.Canhoto.Carga.Codigo == filtroPesquisa.Carga);

            if (filtroPesquisa.Filial > 0)
                result = result.Where(o => o.Canhoto.Filial.Codigo == filtroPesquisa.Filial);

            if (filtroPesquisa.Transportador > 0)
                result = result.Where(o => o.Canhoto.Carga.Empresa.Codigo == filtroPesquisa.Transportador);

            if (filtroPesquisa.NumeroCTe > 0)
                result = result.Where(o => o.Canhoto.XMLNotaFiscal.CTEs.Any(cte => cte.Numero == filtroPesquisa.NumeroCTe));

            if (filtroPesquisa.CodigoEmpresa > 0)
                result = result.Where(o => o.Canhoto.Empresa.Codigo == filtroPesquisa.CodigoEmpresa);

            if (filtroPesquisa.NumeroDocumento > 0)
                result = result.Where(o => o.Canhoto.Numero == filtroPesquisa.NumeroDocumento);

            if (filtroPesquisa.CodigoCanhoto > 0)
                result = result.Where(o => o.Canhoto.Codigo == filtroPesquisa.CodigoCanhoto);

            if (filtroPesquisa.CodigoTipoIntegracao > 0)
                result = result.Where(o => o.TipoIntegracao.Codigo == filtroPesquisa.CodigoTipoIntegracao);

            return result.Fetch(obj => obj.Canhoto).ThenFetch(obj => obj.Emitente).ThenFetch(obj => obj.Localidade).Fetch(obj => obj.TipoIntegracao);
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.CanhotoIntegracao.FiltroPesquisaCanhotoIntegracao filtroPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(filtroPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Canhoto)
                .ThenFetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.CanhotoIntegracao.FiltroPesquisaCanhotoIntegracao filtroPesquisa)
        {
            var result = _Consultar(filtroPesquisa);

            return result.Count();
        }

    }
}
