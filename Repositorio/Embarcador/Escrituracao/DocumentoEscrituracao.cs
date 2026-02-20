using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace Repositorio.Embarcador.Escrituracao
{
    public class DocumentoEscrituracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>
    {
        #region Construtores

        public DocumentoEscrituracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public DocumentoEscrituracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracao filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> consultaDocumentoEscrituracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();

            if (filtrosPesquisa.TipoServicoMultisoftware.HasValue && filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                string[] statusPermitidos = new string[] { "A", "C" };

                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(obj => statusPermitidos.Contains(obj.CTe.Status));
            }
            else
            {
                string[] statusPermitidos = new string[] { "A" };

                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(obj => statusPermitidos.Contains(obj.CTe.Status));
            }

            if (filtrosPesquisa.CodigoLoteEscrituracao > 0)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(obj => obj.LoteEscrituracao.Codigo == filtrosPesquisa.CodigoLoteEscrituracao);
            else
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(obj => (obj.LoteEscrituracao == null) && (obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.AgEscrituracao));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(o => o.CTe.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoModeloDocumento > 0)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(o => o.CTe.ModeloDocumentoFiscal.Codigo == filtrosPesquisa.CodigoModeloDocumento);

            if (filtrosPesquisa.CpfCnpjTomador > 0)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(o => o.CTe.TomadorPagador.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(obj => obj.CTe.DataEmissao >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(obj => obj.CTe.DataEmissao < filtrosPesquisa.DataLimite.Value.AddDays(1).Date);

            if (filtrosPesquisa.SomentePagamentoLiberado)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(o => !o.AguardandoAutorizacao);

            if (filtrosPesquisa.IntervaloParaEscrituracaoDocumento > 0)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(o => o.DataCriacao.Value.AddHours(filtrosPesquisa.IntervaloParaEscrituracaoDocumento) <= DateTime.Now);

            consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Where(obj => obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.Bloqueado);

            return consultaDocumentoEscrituracao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaDocumentoEscrituracao = Consultar(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

            if (parametrosConsulta.InicioRegistros > 0)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                consultaDocumentoEscrituracao = consultaDocumentoEscrituracao.Take(parametrosConsulta.LimiteRegistros);

            return consultaDocumentoEscrituracao
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.CargaOcorrencia)
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.FechamentoFrete)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Cliente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Serie)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoEscrituracao filtrosPesquisa)
        {
            var consultaDocumentoEscrituracao = Consultar(filtrosPesquisa);

            return consultaDocumentoEscrituracao.Count();
        }

        public List<int> BuscarCodigosCTesPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();
            var resut = from obj in query where obj.Carga.Codigo == carga && obj.CargaOcorrencia == null select obj;

            return resut.Select(obj => obj.CTe.Codigo).ToList();
        }

        public List<int> BuscarCodigosCTesPorLote(int loteEscrituracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();
            var resut = from obj in query where obj.LoteEscrituracao.Codigo == loteEscrituracao && obj.CargaOcorrencia == null select obj;

            return resut.Select(obj => obj.CTe.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao BuscarPorCte(int CTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();

            query = query.Where(obj => obj.CTe.Codigo == CTe);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> BuscarPorCtesComSituacaoBloqueado(List<int> CTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();

            query = query.Where(obj => CTes.Contains(obj.CTe.Codigo) && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscrituracaoDocumento.Bloqueado);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao BuscarPorCTeECarga(int codigoCarga, int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();

            query = query.Where(obj => obj.CTe.Codigo == codigoCTe && obj.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        //public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ObterTodasNotasDoLote(int loteEscrituracao)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();
        //    query = query.Where(obj => obj.LoteEscrituracao.Codigo == loteEscrituracao);

        //    query.Select(obj => obj.CTe.XMLNotaFiscais).SelectMany(nf => nf).ToList();
        //}

        public List<int> BuscarCodigosPorLote(int loteEscrituracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();

            query = query.Where(o => o.LoteEscrituracao.Codigo == loteEscrituracao);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> BuscarPorloteEscrituracao(int loteEscrituracao)
        {
            List<int> codigosLotesEscrituracao = null;
            int limite = 200; //Criado para situação de erros de max row size devido a quantidade de registros
            int registrosTotais = 0;
            if (limite > 0)
            {
                codigosLotesEscrituracao = BuscarCodigosPorLote(loteEscrituracao);
                registrosTotais = codigosLotesEscrituracao.Count();
            }
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> result = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();
            int start = 0;



            while (start < codigosLotesEscrituracao?.Count)
            {
                List<int> tmp = codigosLotesEscrituracao.Skip(start).Take(limite).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();

                var filter01 = from obj in query
                               where tmp.Contains(obj.Codigo)
                               select obj;
                filter01
                .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.CFOP)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.ModeloDocumentoFiscal)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.Serie)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.Empresa)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.Empresa)
               .ThenFetch(obj => obj.EmpresaPai)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.Remetente)
               .ThenFetch(obj => obj.Cliente)
               .ThenFetch(obj => obj.Localidade)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.Remetente)
               .ThenFetch(obj => obj.Cliente)
               .ThenFetch(obj => obj.GrupoPessoas)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.Remetente)
               .ThenFetch(obj => obj.Atividade)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.Destinatario)
               //.ThenFetch(obj => obj.Cliente)
               //.ThenFetch(obj => obj.Localidade)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.TomadorPagador)
               //.ThenFetch(obj => obj.Cliente)
               //.ThenFetch(obj => obj.Localidade)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.TomadorPagador)
               //.ThenFetch(obj => obj.Cliente)
               //.ThenFetch(obj => obj.GrupoPessoas)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.LocalidadeInicioPrestacao)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
               .Timeout(120)
               .ToList();

                var filter02 = from obj in query
                               where tmp.Contains(obj.Codigo)
                               select obj;
                filter02
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.CFOP)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.ModeloDocumentoFiscal)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.Serie)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.Empresa)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.Empresa)
               //.ThenFetch(obj => obj.EmpresaPai)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.Remetente)
               //.ThenFetch(obj => obj.Cliente)
               //.ThenFetch(obj => obj.Localidade)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.Remetente)
               //.ThenFetch(obj => obj.Cliente)
               //.ThenFetch(obj => obj.GrupoPessoas)
               //.Fetch(obj => obj.CTe)
               //.ThenFetch(obj => obj.Remetente)
               //.ThenFetch(obj => obj.Atividade)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.Destinatario)
               .ThenFetch(obj => obj.Cliente)
               .ThenFetch(obj => obj.Localidade)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.TomadorPagador)
               .ThenFetch(obj => obj.Cliente)
               .ThenFetch(obj => obj.Localidade)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.TomadorPagador)
               .ThenFetch(obj => obj.Cliente)
               .ThenFetch(obj => obj.GrupoPessoas)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
               .Fetch(obj => obj.CTe)
               .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
               .Timeout(120)
               .ToList();
                foreach (var q1 in filter01)
                {
                    foreach (var q2 in filter02.Where(obj => obj.Codigo == q1.Codigo))
                    {
                        if (q1.CTe == null || q2.CTe == null)
                            continue;
                        if (q2.CTe.Destinatario != null)
                            q1.CTe.Destinatario = q2.CTe.Destinatario;

                        if (q2.CTe.TomadorPagador != null)
                            q1.CTe.TomadorPagador = q2.CTe.TomadorPagador;

                        if (q2.CTe.LocalidadeInicioPrestacao != null)
                            q1.CTe.LocalidadeInicioPrestacao = q2.CTe.LocalidadeInicioPrestacao;

                        if (q2.CTe.LocalidadeTerminoPrestacao != null)
                            q1.CTe.LocalidadeTerminoPrestacao = q2.CTe.LocalidadeTerminoPrestacao;
                    }
                }
                result.AddRange(filter01);
                start += limite;
            }
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> ObterTipoOperacaoPadraoEscrituracao(int loteEscrituracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();
            var resut = from obj in query
                        where obj.LoteEscrituracao.Codigo == loteEscrituracao && obj.Carga != null && obj.Carga.TipoOperacao != null
                        select obj.Carga.TipoOperacao;

            return resut.Distinct().ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>> ObterTipoOperacaoPadraoEscrituracaoAsync(int loteEscrituracao, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();
            var resut = from obj in query
                        where obj.LoteEscrituracao.Codigo == loteEscrituracao && obj.Carga != null && obj.Carga.TipoOperacao != null
                        select obj.Carga.TipoOperacao;

            return resut.Distinct().ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> ObterTipoOperacaoPadraoEscrituracaoOcorrencia(int loteEscrituracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();
            var resut = from obj in query
                        where obj.LoteEscrituracao.Codigo == loteEscrituracao && obj.CargaOcorrencia != null && obj.CargaOcorrencia != null && obj.Carga == null
                        select obj.CargaOcorrencia.Carga.TipoOperacao;

            return resut.Distinct().ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>> ObterTipoOperacaoPadraoEscrituracaoOcorrenciaAsync(int loteEscrituracao, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao>();
            var resut = from obj in query
                        where obj.LoteEscrituracao.Codigo == loteEscrituracao && obj.CargaOcorrencia != null && obj.CargaOcorrencia != null && obj.Carga == null
                        select obj.CargaOcorrencia.Carga.TipoOperacao;

            return resut.Distinct().ToListAsync(cancellationToken);
        }

        public void LiberarEscrituracaoPorCarga(int codigoCarga)
        {
            UnitOfWork.Sessao.CreateQuery("update DocumentoEscrituracao set AguardandoAutorizacao = 0 where Carga.Codigo = :codigoCarga")
                .SetInt32("codigoCarga", codigoCarga)
                .ExecuteUpdate();
        }

        public void LiberarEscrituracaoPorCargas(List<int> codigosCarga)
        {
            UnitOfWork.Sessao.CreateQuery("update DocumentoEscrituracao set AguardandoAutorizacao = 0 where Carga.Codigo in (:codigosCarga)")
                .SetParameterList("codigoCarga", codigosCarga)
                .ExecuteUpdate();
        }

        #endregion

        #region Relatório Frete Cannabis

        public IList<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteContabil> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil filtro, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sql = ObterSelectConsultaRelatorio(filtro, false, propriedades, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteContabil)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteContabil>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil filtro, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorio(filtro, true, propriedades, "", "", "", "", 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private void SetarWhereRelatorioConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil filtro, StringBuilder where, StringBuilder groupBy, StringBuilder joins)
        {
            string pattern = "yyyy-MM-dd";


            if (filtro.Filial > 0)
            {
                where.Append(" AND Filial.FIL_CODIGO = " + filtro.Filial.ToString());

                if (!joins.Contains(" Filial "))
                    joins.Append(" LEFT JOIN T_FILIAL Filial on Filial.FIL_CODIGO = DocumentoContabil.FIL_CODIGO ");
            }

            if (filtro.Transportador > 0)
            {
                where.Append(" AND Empresa.EMP_CODIGO = " + filtro.Transportador.ToString());

                if (!joins.Contains(" Empresa "))
                    joins.Append(" LEFT JOIN T_EMPRESA Empresa on Empresa.EMP_CODIGO = DocumentoContabil.EMP_CODIGO ");
            }

            if (filtro.DataEmissaoInicial != DateTime.MinValue || filtro.DataEmissaoFinal != DateTime.MinValue)
            {
                if (filtro.DataEmissaoInicial != DateTime.MinValue)
                    where.Append(" AND DocumentoContabil.DCB_DATA_EMISSAO >= '" + filtro.DataEmissaoInicial.ToString(pattern) + "'");
                if (filtro.DataEmissaoFinal != DateTime.MinValue)
                    where.Append(" AND DocumentoContabil.DCB_DATA_EMISSAO <= '" + filtro.DataEmissaoFinal.ToString(pattern) + " 23:59:59'");
            }

            if (filtro.Tomador > 0)
            {
                where.Append(" AND Tomador.CLI_CGCCPF = " + filtro.Tomador.ToString());

                if (!joins.Contains(" Tomador "))
                    joins.Append(" LEFT JOIN T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoContabil.CLI_TOMADOR ");
            }

            if (filtro.DataLancamentoInicial != DateTime.MinValue || filtro.DataLancamentoFinal != DateTime.MinValue)
            {
                if (filtro.DataLancamentoInicial != DateTime.MinValue)
                    where.Append(" AND DocumentoContabil.DPV_DATA_LANCAMENTO >= '" + filtro.DataLancamentoInicial.ToString(pattern) + "'");
                if (filtro.DataLancamentoFinal != DateTime.MinValue)
                    where.Append(" AND DocumentoContabil.DPV_DATA_LANCAMENTO <= '" + filtro.DataLancamentoFinal.ToString(pattern) + " 23:59:59'");
            }

            if (filtro.CentroResultado.Count > 0)
            {
                if (filtro.CentroResultado.Count == 1)
                    where.Append(" AND CentroResultado.CRE_CODIGO = " + filtro.CentroResultado.FirstOrDefault());
                else
                    where.Append(" AND CentroResultado.CRE_CODIGO IN( " + String.Join(", ", filtro.CentroResultado) + ")");

                if (!joins.Contains(" CentroResultado "))
                    joins.Append(" LEFT JOIN T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = DocumentoContabil.CRE_CODIGO ");
            }

            if (filtro.ContaContabil.Count > 0)
            {
                if (filtro.ContaContabil.Count == 1)
                    where.Append(" AND PlanoConta.PLA_CODIGO = " + filtro.ContaContabil.FirstOrDefault());
                else
                    where.Append(" AND PlanoConta.PLA_CODIGO IN( " + String.Join(", ", filtro.ContaContabil) + ")");

                if (!joins.Contains(" PlanoConta "))
                    joins.Append(" LEFT JOIN T_PLANO_DE_CONTA PlanoConta on PlanoConta.PLA_CODIGO = DocumentoContabil.PLA_CODIGO ");
            }

            if (filtro.CodigoCarga > 0)
            {
                where.Append(" AND Carga.CAR_CODIGO = " + filtro.CodigoCarga);

                SetarJoinsCarga(joins);
            }
        }

        private string ObterSelectConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil filtro, bool somenteContarNumeroRegistros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder orderBy = new StringBuilder();
            StringBuilder select = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedade in propriedades)
                SetarSelectRelatorioConsulta(propriedade.Propriedade, select, groupBy, joins);

            SetarWhereRelatorioConsulta(filtro, where, groupBy, joins);

            string campos = select.ToString().Trim();
            string agrupamentos = groupBy.ToString().Trim();
            string condicoes = where.ToString().Trim();

            if (somenteContarNumeroRegistros)
                sql.Append("SELECT DISTINCT(COUNT(0) OVER ()) ");
            else
                sql.Append($"SELECT {(campos.Length > 0 ? campos.Substring(0, campos.Length - 1) : "")} ");

            sql.Append(" FROM T_DOCUMENTO_CONTABIL DocumentoContabil ");
            sql.Append(joins.ToString());

            if (condicoes.Length > 0)
                sql.Append(" where ").Append(condicoes.Substring(4)).Append(" ");

            if (agrupamentos.Length > 0)
                sql.Append(" group by ").Append(agrupamentos.Substring(0, agrupamentos.Length - 1)).Append(" ");

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(propOrdena) && select.Contains(propOrdena) && propOrdena != "Codigo")
                orderBy.Append((orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena);

            if (!somenteContarNumeroRegistros)
            {
                sql.Append(" order by ").Append(orderBy.Length > 0 ? orderBy.ToString() : "1 asc");

                if (limite > 0)
                    sql.Append(" offset ").Append(inicio).Append(" rows fetch next ").Append(limite).Append(" rows only;");
            }


            return sql.ToString(); 
        }

        private void SetarSelectRelatorioConsulta(string propriedade, StringBuilder select, StringBuilder groupBy, StringBuilder joins)
        {
            switch (propriedade)
            {
                case "CNPJTomador":
                case "NomeTomador":
                case "Tomador":
                    if (!select.Contains(" CNPJTomador, "))
                    {
                        select.Append("Tomador.CLI_CGCCPF CNPJTomador, ");
                        groupBy.Append("Tomador.CLI_CGCCPF, ");
                    }
                    if (!select.Contains(" TipoTomador, "))
                    {
                        select.Append("Tomador.CLI_FISJUR TipoTomador, ");
                        groupBy.Append("Tomador.CLI_FISJUR, ");
                    }
                    if (!select.Contains(" NomeTomador, "))
                    {
                        select.Append("Tomador.CLI_NOME NomeTomador, ");
                        groupBy.Append("Tomador.CLI_NOME, ");
                    }

                    if (!joins.Contains(" Tomador "))
                        joins.Append(" LEFT JOIN T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoContabil.CLI_TOMADOR ");
                    break;
                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");
                    }

                    if (!joins.Contains(" TipoOperacao "))
                        joins.Append(" LEFT JOIN T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = DocumentoContabil.TOP_CODIGO ");
                    break;
                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");
                    }

                    if (!joins.Contains(" Filial "))
                        joins.Append(" LEFT JOIN T_FILIAL Filial on Filial.FIL_CODIGO = DocumentoContabil.FIL_CODIGO ");
                    break;
                case "CNPJEmpresa":
                case "RazaoEmpresa":
                case "Empresa":
                    if (!select.Contains(" CNPJEmpresa, "))
                    {
                        select.Append("Empresa.EMP_CNPJ CNPJEmpresa, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");
                    }
                    if (!select.Contains(" RazaoEmpresa, "))
                    {
                        select.Append("Empresa.EMP_RAZAO RazaoEmpresa, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");
                    }

                    if (!joins.Contains(" Empresa "))
                        joins.Append(" LEFT JOIN T_EMPRESA Empresa on Empresa.EMP_CODIGO = DocumentoContabil.EMP_CODIGO ");
                    break;
                case "CidadeOrigem":
                case "UFOrigem":
                case "Origem":
                    if (!select.Contains(" CidadeOrigem, "))
                    {
                        select.Append("Origem.LOC_DESCRICAO CidadeOrigem, ");
                        groupBy.Append("Origem.LOC_DESCRICAO, ");
                    }
                    if (!select.Contains(" UFOrigem, "))
                    {
                        select.Append("Origem.UF_SIGLA UFOrigem, ");
                        groupBy.Append("Origem.UF_SIGLA, ");
                    }

                    if (!joins.Contains(" Origem "))
                        joins.Append(" LEFT JOIN T_LOCALIDADES Origem on Origem.LOC_CODIGO = DocumentoContabil.LOC_ORIGEM ");
                    break;
                case "CidadeDestino":
                case "UFDestino":
                case "Destino":
                    if (!select.Contains(" CidadeDestino, "))
                    {
                        select.Append("Destino.LOC_DESCRICAO CidadeDestino, ");
                        groupBy.Append("Destino.LOC_DESCRICAO, ");
                    }
                    if (!select.Contains(" UFDestino, "))
                    {
                        select.Append("Destino.UF_SIGLA UFDestino, ");
                        groupBy.Append("Destino.UF_SIGLA, ");
                    }

                    if (!joins.Contains(" Destino "))
                        joins.Append(" LEFT JOIN T_LOCALIDADES Destino on Destino.LOC_CODIGO = DocumentoContabil.LOC_DESTINO ");
                    break;
                case "NumeroOcorrencia":
                    if (!select.Contains(" Ocorrencia, "))
                    {
                        select.Append("Ocorrencia.COC_NUMERO_CONTRATO Ocorrencia, ");
                        groupBy.Append("Ocorrencia.COC_NUMERO_CONTRATO, ");
                    }

                    if (!joins.Contains(" Ocorrencia "))
                        joins.Append(" LEFT JOIN T_CARGA_OCORRENCIA Ocorrencia on Ocorrencia.COC_CODIGO = DocumentoContabil.COC_CODIGO ");
                    break;
                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }

                    SetarJoinsCarga(joins);
                    break;
                case "Pagamento":
                    if (!select.Contains(" Pagamento, "))
                    {
                        select.Append("Pagamento.PAG_NUMERO Pagamento, ");
                        groupBy.Append("Pagamento.PAG_NUMERO, ");
                    }

                    if (!joins.Contains(" Pagamento "))
                        joins.Append(" LEFT JOIN T_PAGAMENTO Pagamento on Pagamento.PAG_CODIGO = DocumentoContabil.PAG_CODIGO ");
                    break;
                case "CancelamentoPagamento":
                    if (!select.Contains(" CancelamentoPagamento, "))
                    {
                        select.Append("CancelamentoPagamento.CPG_NUMERO CancelamentoPagamento, ");
                        groupBy.Append("CancelamentoPagamento.CPG_NUMERO, ");
                    }

                    if (!joins.Contains(" CancelamentoPagamento "))
                        joins.Append(" LEFT JOIN T_CANCELAMENTO_PAGAMENTO CancelamentoPagamento on CancelamentoPagamento.CPG_CODIGO = DocumentoContabil.CPG_CODIGO ");
                    break;
                case "Provisao":
                    if (!select.Contains(" Provisao, "))
                    {
                        select.Append("Provisao.PRV_NUMERO Provisao, ");
                        groupBy.Append("Provisao.PRV_NUMERO, ");
                    }

                    if (!joins.Contains(" Provisao "))
                        joins.Append(" LEFT JOIN T_PROVISAO Provisao on Provisao.PRV_CODIGO = DocumentoContabil.PRV_CODIGO ");
                    break;
                case "CancelamentoProvisao":
                    if (!select.Contains(" CancelamentoProvisao, "))
                    {
                        select.Append("CancelamentoProvisao.CPV_NUMERO CancelamentoProvisao, ");
                        groupBy.Append("CancelamentoProvisao.CPV_NUMERO, ");
                    }

                    if (!joins.Contains(" Provisao "))
                        joins.Append(" LEFT JOIN T_PROVISAO_CANCELAMENTO CancelamentoProvisao on CancelamentoProvisao.CPV_CODIGO = DocumentoContabil.CPV_CODIGO ");
                    break;
                case "TipoDeContabilizacao":
                    if (!select.Contains(" Pagamento, "))
                    {
                        select.Append("Pagamento.PAG_NUMERO Pagamento, ");
                        groupBy.Append("Pagamento.PAG_NUMERO, ");
                    }

                    if (!joins.Contains(" Pagamento "))
                        joins.Append(" LEFT JOIN T_PAGAMENTO Pagamento on Pagamento.PAG_CODIGO = DocumentoContabil.PAG_CODIGO ");

                    if (!select.Contains(" CancelamentoPagamento, "))
                    {
                        select.Append("CancelamentoPagamento.CPG_NUMERO CancelamentoPagamento, ");
                        groupBy.Append("CancelamentoPagamento.CPG_NUMERO, ");
                    }

                    if (!joins.Contains(" CancelamentoPagamento "))
                        joins.Append(" LEFT JOIN T_CANCELAMENTO_PAGAMENTO CancelamentoPagamento on CancelamentoPagamento.CPG_CODIGO = DocumentoContabil.CPG_CODIGO ");

                    if (!select.Contains(" Provisao, "))
                    {
                        select.Append("Provisao.PRV_NUMERO Provisao, ");
                        groupBy.Append("Provisao.PRV_NUMERO, ");
                    }

                    if (!joins.Contains(" Provisao "))
                        joins.Append(" LEFT JOIN T_PROVISAO Provisao on Provisao.PRV_CODIGO = DocumentoContabil.PRV_CODIGO ");

                    if (!select.Contains(" CancelamentoProvisao, "))
                    {
                        select.Append("CancelamentoProvisao.CPV_NUMERO CancelamentoProvisao, ");
                        groupBy.Append("CancelamentoProvisao.CPV_NUMERO, ");
                    }

                    if (!joins.Contains(" CancelamentoProvisao "))
                        joins.Append(" LEFT JOIN T_PROVISAO_CANCELAMENTO CancelamentoProvisao on CancelamentoProvisao.CPV_CODIGO = DocumentoContabil.CPV_CODIGO ");

                    break;
                case "DataEmissao":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("DocumentoContabil.DCB_DATA_EMISSAO DataEmissao, ");
                        groupBy.Append("DocumentoContabil.DCB_DATA_EMISSAO, ");
                    }

                    break;
                case "CNPJRemetenteFormatado":
                case "Remetente":
                case "RemetenteCTe":
                    if (!select.Contains(" CNPJRemetente, "))
                    {
                        select.Append("Remetente.CLI_CGCCPF CNPJRemetente, ");
                        groupBy.Append("Remetente.CLI_CGCCPF, ");
                    }
                    if (!select.Contains(" TipoRemetente, "))
                    {
                        select.Append("Remetente.CLI_FISJUR TipoRemetente, ");
                        groupBy.Append("Remetente.CLI_FISJUR, ");
                    }
                    if (!select.Contains(" NomeRemetente, "))
                    {
                        select.Append("Remetente.CLI_NOME NomeRemetente, ");
                        groupBy.Append("Remetente.CLI_NOME, ");
                    }
                    if (!select.Contains(" RemetenteCTe, "))
                    {
                        select.Append("Remetente.CLI_NOMEFANTASIA RemetenteCTe, ");
                        groupBy.Append("Remetente.CLI_NOMEFANTASIA, ");
                    }

                    if (!joins.Contains(" Remetente "))
                        joins.Append(" LEFT JOIN T_CLIENTE Remetente on Remetente.CLI_CGCCPF = DocumentoContabil.CLI_CODIGO_REMETENTE ");
                    break;
                case "CNPJDestinatarioFormatado":
                case "Destinatario":
                case "DestinatarioCTe":
                    if (!select.Contains(" CNPJDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_CGCCPF CNPJDestinatario, ");
                        groupBy.Append("Destinatario.CLI_CGCCPF, ");
                    }
                    if (!select.Contains(" TipoDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_FISJUR TipoDestinatario, ");
                        groupBy.Append("Destinatario.CLI_FISJUR, ");
                    }
                    if (!select.Contains(" NomeDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_NOME NomeDestinatario, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");
                    }
                    if (!select.Contains(" DestinatarioCTe, "))
                    {
                        select.Append("Destinatario.CLI_NOMEFANTASIA DestinatarioCTe, ");
                        groupBy.Append("Destinatario.CLI_NOMEFANTASIA, ");
                    }

                    if (!joins.Contains(" Destinatario "))
                        joins.Append(" LEFT JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = DocumentoContabil.CLI_CODIGO_DESTINATARIO ");
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("DocumentoContabil.DCB_NUMERO_DOCUMENTO Numero, ");
                        groupBy.Append("DocumentoContabil.DCB_NUMERO_DOCUMENTO, ");
                    }

                    break;
                case "Serie":
                    if (!select.Contains(" Serie, "))
                    {
                        select.Append("DocumentoContabil.DCB_SERIE_DOCUMENTO Serie, ");
                        groupBy.Append("DocumentoContabil.DCB_SERIE_DOCUMENTO, ");
                    }

                    break;
                case "Modelo":
                    if (!select.Contains(" Modelo, "))
                    {
                        select.Append("ModeloDocumentoFiscal.MOD_DESCRICAO Modelo, ");
                        groupBy.Append("ModeloDocumentoFiscal.MOD_DESCRICAO, ");
                    }

                    if (!joins.Contains(" ModeloDocumentoFiscal "))
                        joins.Append(" LEFT JOIN T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = DocumentoContabil.CON_MODELODOC ");
                    break;
                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe, "))
                    {
                        select.Append("CTe.CON_NUM NumeroCTe, ");
                        groupBy.Append("CTe.CON_NUM, ");
                    }

                    if (!joins.Contains(" CTe "))
                        joins.Append(" LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = DocumentoContabil.CON_CODIGO ");
                    break;
                case "SerieCTe":
                    if (!select.Contains(" SerieCTe, "))
                    {
                        select.Append("Serie.ESE_NUMERO SerieCTe, ");
                        groupBy.Append("Serie.ESE_NUMERO, ");
                    }

                    if (!joins.Contains(" CTe "))
                        joins.Append(" LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = DocumentoContabil.CON_CODIGO ");
                    if (!joins.Contains(" Serie "))
                        joins.Append(" LEFT JOIN T_EMPRESA_SERIE Serie on Serie.ESE_CODIGO = CTe.CON_SERIE ");
                    break;
                case "CodigoCentroResultado":
                    if (!select.Contains(" CodigoCentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_PLANO_CONTABILIDADE CodigoCentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_PLANO_CONTABILIDADE, ");
                    }

                    if (!joins.Contains(" CentroResultado "))
                        joins.Append(" LEFT JOIN T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = DocumentoContabil.CRE_CODIGO ");
                    break;
                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO CentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");
                    }

                    if (!joins.Contains(" CentroResultado "))
                        joins.Append(" LEFT JOIN T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = DocumentoContabil.CRE_CODIGO ");
                    break;
                case "DataLancamento":
                case "AnoLancamento":
                case "MesLancamento":
                    if (!select.Contains(" DataLancamento, "))
                    {
                        select.Append("DocumentoContabil.DPV_DATA_LANCAMENTO DataLancamento, ");
                        groupBy.Append("DocumentoContabil.DPV_DATA_LANCAMENTO, ");
                    }

                    break;
                case "DataLancamentoCancelamentoProvisao":
                    if (!select.Contains(" DataLancamentoCancelamentoProvisao, "))
                    {
                        select.Append("CancelamentoProvisao.CPV_DATA_LANCAMENTO DataLancamentoCancelamentoProvisao, ");
                        groupBy.Append("CancelamentoProvisao.CPV_DATA_LANCAMENTO, ");
                    }

                    if (!joins.Contains(" DocumentoProvisao "))
                        joins.Append(" LEFT JOIN T_DOCUMENTO_PROVISAO DocumentoProvisao on DocumentoProvisao.DPV_CODIGO = DocumentoContabil.DPV_CODIGO_REFERENCIA ");

                    if (!joins.Contains(" CancelamentoProvisao "))
                        joins.Append(" LEFT JOIN T_PROVISAO_CANCELAMENTO CancelamentoProvisao ON CancelamentoProvisao.CPV_CODIGO = DocumentoProvisao.CPV_CODIGO ");
                    break;
                case "CodigoPlanoConta":
                    if (!select.Contains(" CodigoPlanoConta, "))
                    {
                        select.Append("PlanoConta.PLA_PLANO_CONTABILIDADE CodigoPlanoConta, ");
                        groupBy.Append("PlanoConta.PLA_PLANO_CONTABILIDADE, ");
                    }

                    if (!joins.Contains(" PlanoConta "))
                        joins.Append(" LEFT JOIN T_PLANO_DE_CONTA PlanoConta on PlanoConta.PLA_CODIGO = DocumentoContabil.PLA_CODIGO ");
                    break;
                case "PlanoConta":
                    if (!select.Contains(" PlanoConta, "))
                    {
                        select.Append("PlanoConta.PLA_DESCRICAO PlanoConta, ");
                        groupBy.Append("PlanoConta.PLA_DESCRICAO, ");
                    }

                    if (!joins.Contains(" PlanoConta "))
                        joins.Append(" LEFT JOIN T_PLANO_DE_CONTA PlanoConta on PlanoConta.PLA_CODIGO = DocumentoContabil.PLA_CODIGO ");
                    break;
                case "TipoContabilizacao":
                case "ValorLancamento":
                case "Credito":
                case "Debito":
                    if (!select.Contains(" _ValorLancamento, "))
                    {
                        select.Append("SUM(DocumentoContabil.DCB_VALOR_CONTABILIZACAO) _ValorLancamento, ");
                    }
                    if (!select.Contains(" _TipoContabilizacao, "))
                    {
                        select.Append("DocumentoContabil.CCT_TIPO_CONTABILIZACAO _TipoContabilizacao, ");
                        groupBy.Append("DocumentoContabil.CCT_TIPO_CONTABILIZACAO, ");
                    }

                    break;
                case "ValorISS":
                case "ValorISSRetido":
                    if (!select.Contains(" _ValorISS, "))
                    {
                        select.Append("SUM(DocumentoProvisao.DPV_VALOR_ISS) _ValorISS, ");
                    }

                    if (!select.Contains(" ValorISSRetido, "))
                    {
                        select.Append("SUM(DocumentoProvisao.DPV_VALOR_RETENCAO_ISS) ValorISSRetido, ");
                    }

                    if (!joins.Contains(" DocumentoProvisao "))
                        joins.Append(" LEFT JOIN T_DOCUMENTO_PROVISAO DocumentoProvisao on DocumentoProvisao.DPV_CODIGO = DocumentoContabil.DPV_CODIGO_REFERENCIA ");
                    break;
                case "Aliquota":
                    if (!select.Contains(" Aliquota, "))
                    {
                        select.Append("SUM(DocumentoProvisao.DPV_PERCENTUAL_ALICOTA) Aliquota, ");
                    }

                    if (!joins.Contains(" DocumentoProvisao "))
                        joins.Append(" LEFT JOIN T_DOCUMENTO_PROVISAO DocumentoProvisao on DocumentoProvisao.DPV_CODIGO = DocumentoContabil.DPV_CODIGO_REFERENCIA ");
                    break;
                case "AliquotaISS":
                    if (!select.Contains(" AliquotaISS, "))
                    {
                        select.Append("SUM(DocumentoProvisao.DPV_PERCENTUAL_ALICOTA_ISS) AliquotaISS, ");
                    }

                    if (!joins.Contains(" DocumentoProvisao "))
                        joins.Append(" LEFT JOIN T_DOCUMENTO_PROVISAO DocumentoProvisao on DocumentoProvisao.DPV_CODIGO = DocumentoContabil.DPV_CODIGO_REFERENCIA ");
                    break;
                case "CST":
                case "ICMS":
                case "ICMSRetido":
                    if (!select.Contains(" CST, "))
                    {
                        select.Append("DocumentoProvisao.DPV_CST CST, ");
                        groupBy.Append("DocumentoProvisao.DPV_CST, ");
                    }
                    if (!select.Contains(" _ICMS, "))
                    {
                        select.Append("SUM(DocumentoProvisao.DPV_VALOR_ICMS) _ICMS, ");
                    }

                    if (!joins.Contains(" DocumentoProvisao "))
                        joins.Append(" LEFT JOIN T_DOCUMENTO_PROVISAO DocumentoProvisao on DocumentoProvisao.DPV_CODIGO = DocumentoContabil.DPV_CODIGO_REFERENCIA ");
                    break;
                case "MotivoCancelamento":
                    if (!select.Contains(" MotivoCancelamento, "))
                    {
                        select.Append("MotivoCancelamentoPagamento.MCP_DESCRICAO MotivoCancelamento, ");
                        groupBy.Append("MotivoCancelamentoPagamento.MCP_DESCRICAO, ");
                    }

                    if (!joins.Contains(" CancelamentoPagamento "))
                        joins.Append(" LEFT JOIN T_CANCELAMENTO_PAGAMENTO CancelamentoPagamento on CancelamentoPagamento.CPG_CODIGO = DocumentoContabil.CPG_CODIGO ");

                    if (!joins.Contains(" MotivoCancelamentoPagamento "))
                        joins.Append(" LEFT JOIN T_MOTIVO_CANCELAMENTO_PAGAMENTO MotivoCancelamentoPagamento on MotivoCancelamentoPagamento.MCP_CODIGO = CancelamentoPagamento.MCP_CODIGO ");
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoDeCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoDeCarga.TCG_DESCRICAO, ");
                    }

                    SetarJoinsCarga(joins);

                    if (!joins.Contains(" TipoDeCarga "))
                        joins.Append(" LEFT JOIN T_TIPO_DE_CARGA TipoDeCarga on TipoDeCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
                    break;

                case "ModeloVeicularCarga":
                    if (!select.Contains(" ModeloVeicularCarga, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO ModeloVeicularCarga, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");
                    }

                    SetarJoinsCarga(joins);

                    if (!joins.Contains(" ModeloVeicularCarga "))
                        joins.Append(" LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO ");
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA Veiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");
                    }

                    SetarJoinsCarga(joins);

                    if (!joins.Contains(" Veiculo "))
                        joins.Append(" LEFT JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
                    break;

                case "PesoCTe":
                    if (!select.Contains(" PesoCTe, "))
                    {
                        select.Append("CASE WHEN CTe.CON_PESO > 0 THEN CTe.CON_PESO ELSE (select SUM(pesoKgCTe.ICA_QTD) from T_CTE_INF_CARGA pesoKgCTe where pesoKgCTe.ICA_UN = '01' and pesoKgCTe.CON_CODIGO = CTe.CON_CODIGO ) END PesoCTe, ");
                        groupBy.Append("CTe.CON_PESO, CTe.CON_CODIGO, ");
                    }

                    if (!joins.Contains(" CTe "))
                        joins.Append(" LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = DocumentoContabil.CON_CODIGO ");
                    break;

                case "NumeroVP":

                    if (!select.Contains(" NumeroVP, "))
                    {
                        select.Append("NumeroVP.CVP_NUMERO_COMPROVANTE NumeroVP, ");
                        groupBy.Append("NumeroVP.CVP_NUMERO_COMPROVANTE, ");
                    }

                    SetarJoinsCarga(joins);

                    if (!joins.Contains(" NumeroVP "))
                        joins.Append(" LEFT JOIN T_CARGA_VALE_PEDAGIO NumeroVP on Carga.CAR_CODIGO = NumeroVP.CAR_CODIGO");
                    break;

                case "ValorVPFormatado":
                    if (!select.Contains(" ValorVP, "))
                    {
                        select.Append("ValorVP.CVP_VALOR ValorVP, ");
                        groupBy.Append("ValorVP.CVP_VALOR, ");
                    }

                    SetarJoinsCarga(joins);

                    if (!joins.Contains(" ValorVP "))
                        joins.Append(" LEFT JOIN T_CARGA_VALE_PEDAGIO ValorVP on Carga.CAR_CODIGO = ValorVP.CAR_CODIGO");
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append("DadosSumarizados.CDS_EXPEDIDORES Expedidor, ");
                        groupBy.Append("DadosSumarizados.CDS_EXPEDIDORES, ");
                    }
                    SetarJoinsCargaDadosSumarizados(joins);
                    break;
                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("DadosSumarizados.CDS_RECEBEDORES Recebedor, ");
                        groupBy.Append("DadosSumarizados.CDS_RECEBEDORES, ");
                    }
                    SetarJoinsCargaDadosSumarizados(joins);
                    break;
                case "TipoOcorrencia":
                    if (!select.Contains(" TipoOcorrencia, "))
                    {
                        select.Append("TipoOcorrencia.OCO_DESCRICAO TipoOcorrencia, ");
                        groupBy.Append("TipoOcorrencia.OCO_DESCRICAO, ");
                    }
                    if (!joins.Contains(" Ocorrencia "))
                        joins.Append(" LEFT JOIN T_CARGA_OCORRENCIA Ocorrencia on Ocorrencia.COC_CODIGO = DocumentoContabil.COC_CODIGO ");
                    if (!joins.Contains(" TipoOcorrencia "))
                        joins.Append("LEFT JOIN T_OCORRENCIA TipoOcorrencia on TipoOcorrencia.OCO_CODIGO = Ocorrencia.OCO_CODIGO ");
                    break;

                case "DataCriacaoCargaFormatada":
                    if (!select.Contains(" DataCriacaoCarga, "))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO DataCriacaoCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");
                    }

                    break;
                case "DataEmissaoCTeFormatada":
                    if (!select.Contains(" DataEmissaoCTe, "))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissaoCTe, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");
                    }
                    if (!joins.Contains(" CTe "))
                        joins.Append(" LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = DocumentoContabil.CON_CODIGO ");
                    break;
                case "CNPJTransportadorFormatado":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append("CNPJTransportador.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("CNPJTransportador.EMP_CNPJ, ");
                    }

                    if (!joins.Contains(" CNPJTransportador "))
                        joins.Append(" LEFT JOIN T_EMPRESA CNPJTransportador ON CNPJTransportador.EMP_CODIGO = DocumentoContabil.EMP_CODIGO ");
                    break;
                case "KMCargaFormatado":
                    if (!select.Contains(" KMCarga, "))
                    {
                        select.Append("KMCarga.MON_DISTANCIA_PREVISTA KMCarga, ");
                        groupBy.Append("KMCarga.MON_DISTANCIA_PREVISTA, ");
                    }

                    SetarJoinsCarga(joins);

                    if (!joins.Contains(" KMCarga "))
                        joins.Append(" LEFT JOIN T_MONITORAMENTO KMCarga on KMCarga.CAR_CODIGO = Carga.CAR_CODIGO ");
                    break;
                case "PesoNF":
                    if (!select.Contains(" PesoNF, "))
                    {
                        select.Append("PesoNF.NF_PESO PesoNF, ");
                        groupBy.Append("PesoNF.NF_PESO, ");

                        groupBy.Append("Carga.CAR_CODIGO, ");
                    }

                    if (!joins.Contains(" PesoNF "))
                        joins.Append(" LEFT JOIN T_XML_NOTA_FISCAL PesoNF on PesoNF.NFX_CODIGO = DocumentoContabil.NFX_CODIGO");
                    break;
                case "PesoCarga":
                    if (!select.Contains(" PesoCarga, "))
                    {
                        select.Append("DadosSumarizados.CDS_PESO_TOTAL PesoCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO, DadosSumarizados.CDS_PESO_TOTAL, ");
                    }
                    SetarJoinsCargaDadosSumarizados(joins);
                    break;
                case "KmRodado":
                    if (!select.Contains(" KmRodado, "))
                    {
                        select.Append("DadosSumarizados.CDS_DISTANCIA KmRodado, ");
                        groupBy.Append("Carga.CAR_CODIGO, DadosSumarizados.CDS_DISTANCIA, ");
                    }
                    SetarJoinsCargaDadosSumarizados(joins);
                    break;
                default:
                    break;
            }
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" DadosSumarizados "))
                joins.Append(" LEFT JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" LEFT JOIN T_CARGA Carga on Carga.CAR_CODIGO = DocumentoContabil.CAR_CODIGO ");
        }

        #endregion

        #region Relatório Competencia

        public IList<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteCompetencia> ConsultarRelatorioCompetencia(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia filtro, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaDocumentoEscrituracao().ObterSqlPesquisa(filtro, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteCompetencia)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteCompetencia>();
        }

        public int ContarConsultaRelatorioCompetencia(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia filtro, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaDocumentoEscrituracao().ObterSqlContarPesquisa(filtro, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Relatório Saldo Provisão

        public IList<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.SaldoProvisao> ConsultarRelatorioSaldoProvisao(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioSaldoProvisao filtro, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioSaldoProvisao(filtro, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Escrituracao.SaldoProvisao)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.SaldoProvisao>();
        }

        public int ContarConsultaRelatorioSaldoProvisao(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioSaldoProvisao filtro, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena)
        {
            string sql = ObterSelectConsultaRelatorioSaldoProvisao(filtro, true, propriedades, propAgrupa, dirAgrupa, propOrdena, "", 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioSaldoProvisao(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioSaldoProvisao filtro, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaSaldoProvisao(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaSaldoProvisao(filtro, ref where, ref groupBy, ref joins);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaSaldoProvisao(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_DOCUMENTO_PROVISAO DocumentoProvisao ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaSaldoProvisao(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "CNPJCompanhia":
                case "NomeCompanhia":
                case "Companhia":
                    if (!select.Contains(" CNPJCompanhia, "))
                    {
                        select += "Tomador.CLI_CGCCPF CNPJCompanhia, ";
                        groupBy += "Tomador.CLI_CGCCPF, ";
                    }
                    if (!select.Contains(" TipoCompanhia, "))
                    {
                        select += "Tomador.CLI_FISJUR TipoCompanhia, ";
                        groupBy += "Tomador.CLI_FISJUR, ";
                    }
                    if (!select.Contains(" NomeCompanhia, "))
                    {
                        select += "Tomador.CLI_NOME NomeCompanhia, ";
                        groupBy += "Tomador.CLI_NOME, ";
                    }

                    if (!joins.Contains(" Tomador "))
                        joins += " LEFT JOIN T_CLIENTE Tomador ON Tomador.CLI_CGCCPF = DocumentoProvisao.CLI_TOMADOR ";
                    break;
                case "CNPJTransportadora":
                case "NomeTransportadora":
                case "Transportadora":
                    if (!select.Contains(" CNPJTransportadora, "))
                    {
                        select += "Transportador.EMP_CNPJ CNPJTransportadora, ";
                        groupBy += "Transportador.EMP_CNPJ, ";
                    }
                    if (!select.Contains(" NomeTransportadora, "))
                    {
                        select += "Transportador.EMP_RAZAO NomeTransportadora, ";
                        groupBy += "Transportador.EMP_RAZAO, ";
                    }

                    if (!joins.Contains(" Transportador "))
                        joins += " LEFT JOIN T_EMPRESA Transportador ON Transportador.EMP_CODIGO = DocumentoProvisao.EMP_CODIGO ";
                    break;
                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select += @"CASE
		                                WHEN DocumentoProvisao.LOC_ORIGEM IS NULL THEN ''
		                                ELSE CONCAT(Origem.LOC_DESCRICAO, ' - ', Origem.UF_SIGLA)
	                                END Origem,";
                        groupBy += "Origem.LOC_DESCRICAO, Origem.UF_SIGLA, DocumentoProvisao.LOC_ORIGEM, ";
                    }

                    if (!joins.Contains(" Origem "))
                        joins += " LEFT JOIN T_LOCALIDADES Origem ON Origem.LOC_CODIGO = DocumentoProvisao.LOC_ORIGEM ";
                    break;
                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select += @"CASE
		                                WHEN DocumentoProvisao.LOC_DESTINO IS NULL THEN ''
		                                ELSE CONCAT(Destino.LOC_DESCRICAO, ' - ', Destino.UF_SIGLA)
	                                END Destino,";
                        groupBy += "Destino.LOC_DESCRICAO, Destino.UF_SIGLA, DocumentoProvisao.LOC_DESTINO, ";
                    }

                    if (!joins.Contains(" Destino "))
                        joins += " LEFT JOIN T_LOCALIDADES Destino ON Destino.LOC_CODIGO = DocumentoProvisao.LOC_DESTINO ";
                    break;
                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select += "Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ";
                        groupBy += "Carga.CAR_CODIGO_CARGA_EMBARCADOR, ";
                    }

                    if (!joins.Contains(" Carga "))
                        joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = DocumentoProvisao.CAR_CODIGO ";
                    break;
                case "Ocorrencia":
                    if (!select.Contains(" Ocorrencia, "))
                    {
                        select += "Ocorrencia.COC_NUMERO_CONTRATO Ocorrencia, ";
                        groupBy += "Ocorrencia.COC_NUMERO_CONTRATO, ";
                    }

                    if (!joins.Contains(" Ocorrencia "))
                        joins += " LEFT JOIN T_CARGA_OCORRENCIA Ocorrencia ON Ocorrencia.COC_CODIGO = DocumentoProvisao.COC_CODIGO ";
                    break;
                case "CNPJEmitente":
                case "RazaoEmitente":
                case "Emitente":
                    if (!select.Contains(" CNPJEmitente, "))
                    {
                        select += "Tomador.CLI_CGCCPF CNPJEmitente, ";
                        groupBy += "Tomador.CLI_CGCCPF, ";
                    }
                    if (!select.Contains(" TipoEmitente, "))
                    {
                        select += "Tomador.CLI_FISJUR TipoEmitente, ";
                        groupBy += "Tomador.CLI_FISJUR, ";
                    }
                    if (!select.Contains(" NomeEmitente, "))
                    {
                        select += "Tomador.CLI_NOME NomeEmitente, ";
                        groupBy += "Tomador.CLI_NOME, ";
                    }

                    if (!joins.Contains(" Emitente "))
                        joins += " LEFT JOIN T_CLIENTE Emitente ON Emitente.CLI_CGCCPF = DocumentoProvisao.CLI_CODIGO_DESTINATARIO ";
                    break;
                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select += "Filial.FIL_DESCRICAO Filial, ";
                        groupBy += "Filial.FIL_DESCRICAO, ";
                    }

                    if (!joins.Contains(" Filial "))
                        joins += " LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = DocumentoProvisao.FIL_CODIGO ";
                    break;
                case "NotaFiscal":
                    if (!select.Contains(" NotaFiscal, "))
                    {
                        select += "DocumentoProvisao.DPV_NUMERO_DOCUMENTO NotaFiscal, ";
                        groupBy += "DocumentoProvisao.DPV_NUMERO_DOCUMENTO, ";
                    }

                    break;
                case "MotivoCancelamento":
                    if (!select.Contains(" MotivoCancelamento, "))
                    {
                        select += "CONCAT(CancelamentoProvisao.CPV_NUMERO, '') MotivoCancelamento, ";
                        groupBy += "CancelamentoProvisao.CPV_NUMERO, ";
                    }

                    if (!joins.Contains(" CancelamentoProvisao "))
                        joins += " LEFT JOIN T_PROVISAO_CANCELAMENTO CancelamentoProvisao ON CancelamentoProvisao.CPV_CODIGO = DocumentoProvisao.CPV_CODIGO ";
                    break;
                case "DataEmissao":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select += "DocumentoProvisao.DPV_DATA_EMISSAO DataEmissao, ";
                        groupBy += "DocumentoProvisao.DPV_DATA_EMISSAO, ";
                    }
                    break;
                case "CTe":
                    if (!select.Contains(" CTe, "))
                    {
                        select += "DocumentoProvisao.DPV_NUMERO_DOCUMENTO CTe, ";
                        groupBy += "DocumentoProvisao.DPV_NUMERO_DOCUMENTO, ";
                    }
                    break;
                case "Serie":
                    if (!select.Contains(" Serie, "))
                    {
                        select += "DocumentoProvisao.DPV_SERIE_DOCUMENTO Serie, ";
                        groupBy += "DocumentoProvisao.DPV_SERIE_DOCUMENTO, ";
                    }
                    break;
                case "Aliquota":
                    if (!select.Contains(" Aliquota, "))
                    {
                        select += "DocumentoProvisao.DPV_PERCENTUAL_ALICOTA Aliquota, ";
                        groupBy += "DocumentoProvisao.DPV_PERCENTUAL_ALICOTA, ";
                    }
                    break;
                case "ValorICMS":
                    if (!select.Contains(" ValorICMS, "))
                    {
                        select += "DocumentoProvisao.DPV_VALOR_ICMS ValorICMS, ";
                        groupBy += "DocumentoProvisao.DPV_VALOR_ICMS, ";
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaSaldoProvisao(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioSaldoProvisao filtro, ref string where, ref string groupBy, ref string joins)
        {
            string pattern = "yyyy-MM-dd";


            //if (filtro.Filial > 0)
            //{
            //    where += " AND Filial.FIL_CODIGO = " + filtro.Filial.ToString();

            //    if (!joins.Contains(" Filial "))
            //        joins += " LEFT JOIN T_FILIAL Filial on Filial.FIL_CODIGO = DocumentoContabil.FIL_CODIGO ";
            //}

            //if (filtro.Transportador > 0)
            //{
            //    where += " AND Empresa.EMP_CODIGO = " + filtro.Transportador.ToString();

            //    if (!joins.Contains(" Empresa "))
            //        joins += " LEFT JOIN T_EMPRESA Empresa on Empresa.EMP_CODIGO = DocumentoContabil.EMP_CODIGO ";
            //}

            //if (filtro.DataEmissaoInicial != DateTime.MinValue || filtro.DataEmissaoFinal != DateTime.MinValue)
            //{
            //    if (filtro.DataEmissaoInicial != DateTime.MinValue)
            //        where += " AND DocumentoContabil.DCB_DATA_EMISSAO >= '" + filtro.DataEmissaoInicial.ToString(pattern) + "'";
            //    if (filtro.DataEmissaoFinal != DateTime.MinValue)
            //        where += " AND DocumentoContabil.DCB_DATA_EMISSAO <= '" + filtro.DataEmissaoFinal.ToString(pattern) + " 23:59:59'";
            //}

            //if (filtro.Tomador > 0)
            //{
            //    where += " AND Tomador.CLI_CGCCPF = " + filtro.Tomador.ToString();

            //    if (!joins.Contains(" Tomador "))
            //        joins += " LEFT JOIN T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoContabil.CLI_TOMADOR ";
            //}

            //if (filtro.DataLancamentoInicial != DateTime.MinValue || filtro.DataLancamentoFinal != DateTime.MinValue)
            //{
            //    if (filtro.DataLancamentoInicial != DateTime.MinValue)
            //        where += " AND DocumentoContabil.DPV_DATA_LANCAMENTO >= '" + filtro.DataLancamentoInicial.ToString(pattern) + "'";
            //    if (filtro.DataLancamentoFinal != DateTime.MinValue)
            //        where += " AND DocumentoContabil.DPV_DATA_LANCAMENTO <= '" + filtro.DataLancamentoFinal.ToString(pattern) + " 23:59:59'";
            //}

            //if (filtro.CentroResultado.Count > 0)
            //{
            //    if (filtro.CentroResultado.Count == 1)
            //        where += " AND CentroResultado.CRE_CODIGO = " + filtro.CentroResultado.FirstOrDefault();
            //    else
            //        where += " AND CentroResultado.CRE_CODIGO IN( " + String.Join(", ", filtro.CentroResultado) + ")";

            //    if (!joins.Contains(" CentroResultado "))
            //        joins += " LEFT JOIN T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = DocumentoContabil.CRE_CODIGO ";
            //}

            //if (filtro.ContaContabil.Count > 0)
            //{
            //    if (filtro.ContaContabil.Count == 1)
            //        where += " AND PlanoConta.PLA_CODIGO = " + filtro.ContaContabil.FirstOrDefault();
            //    else
            //        where += " AND PlanoConta.PLA_CODIGO IN( " + String.Join(", ", filtro.ContaContabil) + ")";

            //    if (!joins.Contains(" PlanoConta "))
            //        joins += " LEFT JOIN T_PLANO_DE_CONTA PlanoConta on PlanoConta.PLA_CODIGO = DocumentoContabil.PLA_CODIGO ";
            //}
        }

        #endregion
    }
}
