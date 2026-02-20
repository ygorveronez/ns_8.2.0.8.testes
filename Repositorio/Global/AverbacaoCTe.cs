using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class AverbacaoCTe : RepositorioBase<Dominio.Entidades.AverbacaoCTe>, Dominio.Interfaces.Repositorios.AverbacaoCTe
    {
        public AverbacaoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.AverbacaoCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.AverbacaoCTe BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.AverbacaoCTe BuscarPorCodigoArquivoCancelamento(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.ArquivosTransacaoCancelamento.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.AverbacaoCTe BuscarPorCodigoECarga(int codigo, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();
            var result = from obj in query
                         where
                            obj.Codigo == codigo &&
                            obj.Carga.Codigo == carga
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query
                         where
                            obj.Carga.Codigo == carga
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCargaESituacao(int carga, Dominio.Enumeradores.StatusAverbacaoCTe situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query
                         where
                            obj.Carga.Codigo == carga &&
                            obj.Status == situacao
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.AverbacaoCTe> BuscarAverbacoesFechamento(int transportador, int tipoOperacao, DateTime? dataInicio, DateTime? dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query
                         where
                         obj.SituacaoFechamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento.EmAberto &&
                         obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso
                         select obj;

            if (transportador > 0)
                result = result.Where(o => o.Carga.Empresa.Codigo == transportador);

            if (tipoOperacao > 0)
                result = result.Where(o => o.Carga.TipoOperacao.Codigo == tipoOperacao);

            if (dataInicio.HasValue && dataInicio.Value != null)
                result = result.Where(o => o.DataRetorno.Value.Date >= dataInicio.Value);
            if (dataFim.HasValue && dataFim.Value != null)
                result = result.Where(o => o.DataRetorno.Value.Date <= dataFim.Value);

            return result.ToList();
        }

        public int ContarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            return result.Count();
        }

        public Dominio.Entidades.AverbacaoCTe BuscarPorChaveCTeEProtocolo(string chaveCTe, string protocolo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.CTe.Chave == chaveCTe && obj.Protocolo == protocolo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCTe(string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.CTe.Chave == chaveCTe && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCTes(List<string> chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where chaveCTe.Contains(obj.CTe.Chave) && obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso select obj;

            return result.OrderBy(o => o.Codigo).Distinct().ToList();
        }

        //public List<Dominio.Entidades.AverbacaoCTe> BuscarPraReenviar(int[] ctes, Dominio.Enumeradores.StatusAverbacaoCTe status)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

        //    var result = from obj in query where ctes.Contains(obj.CTe.Codigo) && obj.Status == status select obj;

        //    return result.OrderBy(o => o.Codigo).ToList();
        //}

        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa select obj;

            return result.OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCodigoCTes(List<int> codigoCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>()
                .Where(obj =>
                    codigoCTes.Contains(obj.CTe.Codigo) &&
                    obj.Status != Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado
                );

            return query.ToList();
        }


        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorApoliceSeguroAverbacao(int codigoApoliceSeguroAverbacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.ApoliceSeguroAverbacao.Codigo == codigoApoliceSeguroAverbacao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCTesAutorizadosESituacao(int codigoEmpresa, int codigoCTe, Dominio.Enumeradores.StatusAverbacaoCTe situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query
                         where
                            obj.CTe.Codigo == codigoCTe &&
                            obj.CTe.Status == "A" && // Filtra apenas CTes autorizados
                            obj.Status == situacao
                         select obj;

            return result.Timeout(120).ToList();
        }

        //public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCTesCanceladosESituacao(int codigoEmpresa, int codigoCTe, Dominio.Enumeradores.StatusAverbacaoCTe situacao)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

        //    var result = from obj in query
        //                 where
        //                    obj.CTe.Codigo == codigoCTe &&
        //                    obj.CTe.Status == "C" && // Filtra apenas CTes cancelados
        //                    obj.Status == situacao
        //                 select obj;

        //    return result.ToList();
        //}

        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCTeESituacao(int codigoCTe, Dominio.Enumeradores.StatusAverbacaoCTe situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query
                         where
                            obj.CTe.Codigo == codigoCTe &&
                            obj.Status == situacao
                         select obj;

            return result.ToList();
        }

        public bool ContemAverbacaoAutorizada(int codigoCTe, Dominio.Enumeradores.FormaAverbacaoCTE forma)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query
                         where
                            obj.CTe.Codigo == codigoCTe &&
                            obj.Forma == forma &&
                            (obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso)
                         select obj;

            return result.Any();
        }

        public Dominio.Entidades.AverbacaoCTe BuscarAverbacaoPendenteIntegracao(int codigoCTe, Dominio.Enumeradores.FormaAverbacaoCTE forma)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query
                         where
                            obj.CTe.Codigo == codigoCTe &&
                            obj.Forma == forma &&
                            (obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao || obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Enviado || obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Pendente || obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao)
                         select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorCTeTipoEStatus(int codigoCTe, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Enumeradores.StatusAverbacaoCTe[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Tipo == tipo && status.Contains(obj.Status) select obj;

            return result.Count();
        }

        public int ContarPorCargaTipoEStatus(int codigoCarga, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Enumeradores.StatusAverbacaoCTe status, bool averbacaoOperacaoContainer = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Tipo == tipo && obj.Status == status select obj;

            if (averbacaoOperacaoContainer)



                result = result.Where(o => o.CTe.DocumentoOperacaoContainer.Value);
            else
                result = result.Where(o => o.CTe == null || !o.CTe.DocumentoOperacaoContainer.Value || o.CTe.DocumentoOperacaoContainer == null);


            return result.Count();
        }


        public int ContarPorCargaTipoEStatus(int codigoCarga, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Enumeradores.StatusAverbacaoCTe[] status, bool averbacaoOperacaoContainer = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Tipo == tipo && status.Contains(obj.Status) select obj;

            if (averbacaoOperacaoContainer)
                result = result.Where(o => o.CTe.DocumentoOperacaoContainer.Value);
            else
                result = result.Where(o => o.CTe == null || !o.CTe.DocumentoOperacaoContainer.Value || o.CTe.DocumentoOperacaoContainer == null);


            return result.Count();
        }

        public bool ExistePorCargaTipoEStatusDiff(int codigoCarga, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Enumeradores.StatusAverbacaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Tipo == tipo && obj.Status != status select obj;

            return result.Any();
        }

        public bool ExistePorCargaTipoEStatus(int codigoCarga, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Enumeradores.StatusAverbacaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Tipo == tipo && obj.Status == status select obj;

            return result.Any();
        }

        public bool ExistePorCargaTipoEStatus(int codigoCarga, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Dominio.Enumeradores.StatusAverbacaoCTe[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Tipo == tipo && status.Contains(obj.Status) select obj;

            return result.Any();
        }

        public bool ExistePorCargaEStatus(int codigoCarga, Dominio.Enumeradores.StatusAverbacaoCTe[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && status.Contains(obj.Status) && obj.AverbacaoImportada != true select obj;

            return result.Any();
        }

        public bool ExistePorCargaEStatus(int codigoCarga, Dominio.Enumeradores.StatusAverbacaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Status == status && obj.AverbacaoImportada != true select obj;

            return result.Any();
        }

        //public int ContarPorCTeEStatus(int codigoCTe, Dominio.Enumeradores.StatusAverbacaoCTe status)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

        //    var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Status == status select obj;

        //    return result.Count();
        //}

        //public int ContarPorCargaEStatus(int codigoCarga, Dominio.Enumeradores.StatusAverbacaoCTe status)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

        //    var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Status == status select obj;

        //    return result.Count();
        //}

        public List<Dominio.Entidades.AverbacaoCTe> ConsultaPorCarga(int carga, int codigoCancelamentoCarga, int numero, string apolice, Dominio.Enumeradores.StatusAverbacaoCTe? situacao, bool? averbacaoFilialEmissora, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros, bool retornarDocumentoOperacaoContainer)
        {
            var result = _ConsultaPorCarga(carga, codigoCancelamentoCarga, numero, apolice, situacao, averbacaoFilialEmissora, retornarDocumentoOperacaoContainer);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.CTe)
                .Fetch(obj => obj.ApoliceSeguroAverbacao)
                .ThenFetch(obj => obj.ApoliceSeguro)
                .ThenFetch(obj => obj.Seguradora)
                .ToList();
        }

        public int ContarConsultaPorCarga(int carga, int codigoCancelamentoCarga, int numero, string apolice, Dominio.Enumeradores.StatusAverbacaoCTe? situacao, bool? averbacaoFilialEmissora, bool retornarDocumentoOperacaoContainer)
        {
            var result = _ConsultaPorCarga(carga, codigoCancelamentoCarga, numero, apolice, situacao, averbacaoFilialEmissora, retornarDocumentoOperacaoContainer);

            return result.Count();
        }

        public List<int> BuscarAverbacoes(Dominio.Enumeradores.StatusAverbacaoCTe situacaoAverbacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            query = query.Where(o => o.Status == situacaoAverbacao);

            return query.Select(o => o.Codigo).OrderBy(o => o).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.AverbacaoCTe> BuscarAverbacoesPendentesEmbarcador(Dominio.Enumeradores.StatusAverbacaoCTe situacaoAverbacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            query = query.Where(o => o.Status == situacaoAverbacao
                                && o.CTe.Status.Equals("A")
                                && o.Carga != null
                                && o.Carga.TipoOperacao != null
                                && o.Carga.TipoOperacao.ConfiguracaoEmissao != null
                                && o.Carga.TipoOperacao.ConfiguracaoEmissao.AverbarCTeImportadoDoEmbarcador);

            return query.OrderBy(o => o).Take(maximoRegistros).ToList();
        }

        public List<int> BuscarAverbacoesInverso(Dominio.Enumeradores.StatusAverbacaoCTe situacaoAverbacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            query = query.Where(o => o.Status == situacaoAverbacao);

            return query.OrderBy("Codigo descending").Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.AverbacaoCTe> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaCTe = Consultar(filtrosPesquisa);

            return ObterLista(consultaCTe, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote filtrosPesquisa)
        {
            var consultaCTe = Consultar(filtrosPesquisa);

            return consultaCTe.Count();
        }

        public List<Dominio.Entidades.AverbacaoCTe> BuscarPorCargaCancelamento(int codigoCarga, int codigoCargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CargaCancelamento == null select obj;

            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resultQueryCargaCTe = from obj in queryCargaCTe where obj.CargaCancelamento.Codigo == codigoCargaCancelamento select obj;
            result = result.Where(o => resultQueryCargaCTe.Where(a => a.CTe.Codigo == o.CTe.Codigo).Any());

            return result.ToList();
        }

        #endregion

        #region Relatório de CTes Averbados

        public IList<Dominio.Relatorios.Embarcador.DataSource.Seguros.CTesAverbados> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCte = new ConsultaCTeAverbacao().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCte.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Seguros.CTesAverbados)));

            return consultaCte.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Seguros.CTesAverbados>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaCTeAverbacao().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.AverbacaoCTe> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoCTeLote filtrosPesquisa)
        {
            var consultaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                consultaCTe = consultaCTe.Where(o => o.CTe.DataEmissao.Value.Date >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                consultaCTe = consultaCTe.Where(o => o.CTe.DataEmissao.Value.Date <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.NumeroInicial > 0)
                consultaCTe = consultaCTe.Where(o => o.CTe.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                consultaCTe = consultaCTe.Where(o => o.CTe.Numero <= filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.TipoModal != null && filtrosPesquisa.TipoModal.Count > 0)
            {
                if (filtrosPesquisa.TipoModal.Count == 1)
                    consultaCTe = consultaCTe.Where(o => o.CTe.TipoModal == filtrosPesquisa.TipoModal[0]);
                else
                    consultaCTe = consultaCTe.Where(o => filtrosPesquisa.TipoModal.Contains(o.CTe.TipoModal));
            }

            if (filtrosPesquisa.StatusAverbacao.Count > 0)
                consultaCTe = consultaCTe.Where(o => filtrosPesquisa.StatusAverbacao.Contains(o.Status));
            else
                consultaCTe = consultaCTe.Where(o => o.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaCTe = consultaCTe.Where(o => o.CTe.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                consultaCTe = consultaCTe.Where(o => o.CTe.PortoOrigem.Codigo == filtrosPesquisa.CodigoPortoOrigem);

            if (filtrosPesquisa.CodigoTerminalOrigem > 0)
                consultaCTe = consultaCTe.Where(o => o.CTe.TerminalOrigem.Codigo == filtrosPesquisa.CodigoTerminalOrigem);

            if (filtrosPesquisa.CodigoTerminalDestino > 0)
                consultaCTe = consultaCTe.Where(o => o.CTe.TerminalDestino.Codigo == filtrosPesquisa.CodigoTerminalDestino);

            if (filtrosPesquisa.CodigoViagem > 0)
                consultaCTe = consultaCTe.Where(o => o.CTe.Viagem.Codigo == filtrosPesquisa.CodigoViagem);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                consultaCTe = consultaCTe.Where(o => o.CTe.NumeroBooking == filtrosPesquisa.NumeroBooking);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                consultaCTe = consultaCTe.Where(o => o.CTe.NumeroOS == filtrosPesquisa.NumeroOS);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                consultaCTe = consultaCTe.Where(o => o.CTe.NumeroControle == filtrosPesquisa.NumeroControle);

            if (filtrosPesquisa.CodigoContainer > 0)
                consultaCTe = consultaCTe.Where(o => o.CTe.Containers.Any(c => c.Container.Codigo == filtrosPesquisa.CodigoContainer));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroNF))
                consultaCTe = consultaCTe.Where(o => o.CTe.Documentos.Any(c => c.Numero == filtrosPesquisa.NumeroNF));

            return consultaCTe.Fetch(o => o.CTe).ThenFetch(o => o.Remetente)
                        .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeInicioPrestacao)
                        .Fetch(o => o.CTe).ThenFetch(o => o.PortoOrigem)
                        .Fetch(o => o.CTe).ThenFetch(o => o.TerminalOrigem)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Viagem);
        }

        private IQueryable<Dominio.Entidades.AverbacaoCTe> _ConsultaPorCarga(int carga, int codigoCancelamentoCarga, int numero, string apolice, Dominio.Enumeradores.StatusAverbacaoCTe? situacao, bool? averbacaoFilialEmissora, bool retornarDocumentoOperacaoContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            var result = from obj in query
                         where obj.Carga.Codigo == carga
                         select obj;

            if (codigoCancelamentoCarga > 0)
                result = result.Where(o => o.CargaCancelamento.Codigo == codigoCancelamentoCarga || o.CargaCancelamento == null);
            else
                result = result.Where(o => o.CargaCancelamento == null || o.CargaCancelamento.TipoCancelamentoCargaDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCargaDocumento.Carga);

            if (averbacaoFilialEmissora.HasValue)
                result = result.Where(obj => obj.ApoliceSeguroAverbacao == null || obj.ApoliceSeguroAverbacao.SeguroFilialEmissora == averbacaoFilialEmissora.Value);

            if (numero > 0)
                result = result.Where(o => o.CTe.Numero == numero || o.XMLNotaFiscal.Numero == numero);

            if (!string.IsNullOrWhiteSpace(apolice))
                result = result.Where(o => o.ApoliceSeguroAverbacao.ApoliceSeguro.NumeroApolice.Contains(apolice));

            if (situacao.HasValue && situacao != Dominio.Enumeradores.StatusAverbacaoCTe.Todos)
                result = result.Where(o => o.Status == situacao.Value);

            if (retornarDocumentoOperacaoContainer)
                result = result.Where(obj => obj.CTe.DocumentoOperacaoContainer.Value);
            else
                result = result.Where(obj => obj.CTe == null || !obj.CTe.DocumentoOperacaoContainer.Value || obj.CTe.DocumentoOperacaoContainer == null);


            return result;
        }

        /*private IQueryable<Dominio.Entidades.AverbacaoCTe> _ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AverbacaoCTe>();

            query = from o in query where o.CTe != null select o;

            if (filtrosPesquisa.Status != Dominio.Enumeradores.StatusAverbacaoCTe.Todos)
                query = query.Where(o => o.Status == filtrosPesquisa.Status);

            if (filtrosPesquisa.SituacaoFechamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento.Todas)
                query = query.Where(o => o.SituacaoFechamento == filtrosPesquisa.SituacaoFechamento);

            if (filtrosPesquisa.CodigoTransportador > 0)
                query = query.Where(o => o.CTe.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoSeguradora > 0)
                query = query.Where(o => o.ApoliceSeguroAverbacao.ApoliceSeguro.Seguradora.Codigo == filtrosPesquisa.CodigoSeguradora);

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                query = query.Where(o => o.CTe.DataEmissao.Value.Date >= filtrosPesquisa.DataInicialEmissao.Date);
            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                query = query.Where(o => o.CTe.DataEmissao.Value.Date <= filtrosPesquisa.DataFinalEmissao.Date);

            if (filtrosPesquisa.CodigoModeloDocumentoFiscal > 0)
                query = query.Where(o => o.CTe.ModeloDocumentoFiscal.Codigo == filtrosPesquisa.CodigoModeloDocumentoFiscal);

            if (filtrosPesquisa.CodigoClienteProvedorOS > 0)
                query = query.Where(o => o.CTe.ClienteProvedorOS.CPF_CNPJ == filtrosPesquisa.CodigoClienteProvedorOS);

            if (filtrosPesquisa.DataServicoInicial != DateTime.MinValue)
                query = query.Where(o => o.CTe.CargaCTes.Any(c => c.Carga.DataCarregamentoCarga.Value.Date >= filtrosPesquisa.DataServicoInicial.Date));
            if (filtrosPesquisa.DataServicoFinal != DateTime.MinValue)
                query = query.Where(o => o.CTe.CargaCTes.Any(c => c.Carga.DataCarregamentoCarga.Value.Date <= filtrosPesquisa.DataServicoFinal.Date));

            return query;
        }*/

        #endregion
    }
}
