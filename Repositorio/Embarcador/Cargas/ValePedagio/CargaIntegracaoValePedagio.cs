using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ValePedagio
{
    public class CargaIntegracaoValePedagio : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>
    {
        #region Construtores

        public CargaIntegracaoValePedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio BuscarPorCargaECodigoIntegracao(int codigoCarga, string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CodigoIntegracaoValePedagio == codigoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public bool ExisteValePedagioPamCardPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Pamcard);
            return query.Any();
        }

        public bool ExisteValePedagioPorCargaETipoIntegracao(int codigoCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao)
        {
            return this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>()
                .Where(obj =>
                    obj.Carga.Codigo == codigoCarga &&
                    obj.SituacaoIntegracao == SituacaoIntegracao.Integrado &&
                    tiposIntegracao.Contains(obj.TipoIntegracao.Tipo))
                .Any();
        }
        public async Task<bool> ExisteValePedagioPorCargaETipoIntegracaoAsync(int codigoCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && tiposIntegracao.Contains(obj.TipoIntegracao.Tipo));
            return await query.AnyAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>()
                .Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        public bool VerificarSeExisteValePedagioPorStatus(int codigoCarga, SituacaoValePedagio situacaoValePedagio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SituacaoValePedagio == situacaoValePedagio select obj;

            return result.Any();
        }

        public bool VerificarVPnaoCanceladoPorCarga(int codigoCarga, bool ignorarIntegradoEmbarcador = false)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>(){
                   Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Repom
                   , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PagBem
                   , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DBTrans
                   , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Pamcard
                   , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete
                   , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Extratta
                   , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom
                   , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ambipar
                   , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NDDCargo
                };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query
                         where obj.Carga.Codigo == codigoCarga && ((obj.SituacaoValePedagio == SituacaoValePedagio.Confirmada)
                         || (obj.SituacaoValePedagio == SituacaoValePedagio.Comprada && tipoIntegracao.Contains(obj.TipoIntegracao.Tipo)))
                         select obj;

            if (ignorarIntegradoEmbarcador)
                result = result.Where(o => o.PedagioIntegradoEmbarcador != true);

            return result.Any();
        }

        public bool VerificarVPNaoCompradoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao != SituacaoIntegracao.Integrado select obj;

            return result.Any();
        }

        public bool VerificarEmCanceladoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SituacaoValePedagio == SituacaoValePedagio.EmCancelamento select obj;

            return result.Any();
        }

        public bool VerificarExistePorCargaAgIntegracao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao select obj;

            return result.Any();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Codigo;

            return result.Any();
        }

        public Task<bool> ExistePorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Codigo;

            return result.AnyAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio BuscarPorTipoIntegracao(int carga, int tipoIntegracao, bool valePedagioRetorno)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == carga && obj.TipoIntegracao.Codigo == tipoIntegracao && obj.CompraComEixosSuspensos == valePedagioRetorno select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> BuscarValePedagioPorTipoIntegracao(int tipoIntegracao, int limiteRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>()
                .Where(obj => obj.TipoIntegracao.Codigo == tipoIntegracao && obj.NumeroValePedagio != null && obj.NumeroValePedagio != string.Empty && obj.SituacaoIntegracao == SituacaoIntegracao.Integrado);

            var queryExtratoValePedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio>();
            query = query.Where(obj => !queryExtratoValePedagio.Any(e => e.ValePedagio.Codigo == obj.Codigo));

            return query.Take(limiteRegistros).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> BuscarPorCarga(int codigoCarga, bool ignorarIntegradoEmbarcador = false)
        {
            return BuscarPorCarga(codigoCarga, situacao: null, ignorarIntegradoEmbarcador);
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>> BuscarPorCargaAsync(int codigoCarga, bool ignorarIntegradoEmbarcador = false)
        {
            return await BuscarPorCargaAsync(codigoCarga, situacao: null, ignorarIntegradoEmbarcador);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> BuscarPorCarga(int codigoCarga, SituacaoIntegracao? situacao, bool ignorarIntegradoEmbarcador = false)
        {
            var consultaCargaIntegracaoValePedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                consultaCargaIntegracaoValePedagio = consultaCargaIntegracaoValePedagio.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (ignorarIntegradoEmbarcador)
                consultaCargaIntegracaoValePedagio = consultaCargaIntegracaoValePedagio.Where(o => o.PedagioIntegradoEmbarcador != true);

            return consultaCargaIntegracaoValePedagio.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>> BuscarPorCargaAsync(int codigoCarga, SituacaoIntegracao? situacao, bool ignorarIntegradoEmbarcador = false)
        {
            var consultaCargaIntegracaoValePedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                consultaCargaIntegracaoValePedagio = consultaCargaIntegracaoValePedagio.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (ignorarIntegradoEmbarcador)
                consultaCargaIntegracaoValePedagio = consultaCargaIntegracaoValePedagio.Where(o => o.PedagioIntegradoEmbarcador != true);

            return await consultaCargaIntegracaoValePedagio.ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio BuscarPorUnicaCarga(int codigoCarga)
        {
            var consultaCargaIntegracaoValePedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaIntegracaoValePedagio.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> BuscarPorProtocoloCarga(int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            query = query.Where(o => o.Carga.Protocolo == protocoloCarga);

            return query
                .Fetch(obj => obj.Carga)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> BuscarPorProtocoloCarga(List<int> protocolosCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            query = query.Where(o => protocolosCarga.Contains(o.Carga.Protocolo));

            return query
                .Fetch(obj => obj.Carga)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> BuscarPorCargaAgIntegracao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Carga.IntegrandoValePedagio select obj;

            return result.ToList();
        }

        public List<int> BuscarCargarAgIntegracaoValePedagio(int quantidadeRegistros, int tentativasLimite, int tempoProximaTentativaMinutos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();
            var result = from obj in query
                         where (obj.Carga.IntegrandoValePedagio || (obj.Carga.ProblemaIntegracaoValePedagio && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                               && obj.SituacaoValePedagio == SituacaoValePedagio.AguardandoCadastroRota && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos)))
                               && obj.Carga.CargaFechada && obj.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos
                         select obj;

            return result
                .Fetch(o => o.Carga)
                .OrderBy(o => o.Codigo)
                .Select(o => o.Codigo)
                .Take(quantidadeRegistros)
                .ToList();
        }

        public List<int> BuscarPorCargaEmCancelamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.SituacaoValePedagio == SituacaoValePedagio.EmCancelamento select obj;

            return result.Select(x => x.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> BuscarCTeIntegracaoPendente(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where !obj.Carga.GerandoIntegracoes && obj.TipoIntegracao.TipoEnvio == tipoEnvio && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos))) select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public int ContarPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Count();
        }

        public List<int> BuscarValePedagioAgRetorno(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where !obj.Carga.IntegrandoValePedagio && obj.SituacaoIntegracao == SituacaoIntegracao.AgRetorno && obj.Carga.CargaFechada select obj;

            return result.Fetch(o => o.TipoIntegracao).Select(x => x.Codigo).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarTipoIntegracaoPendente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == tipoEnvio && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) select obj.TipoIntegracao;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> Consultar(int codigoCarga, int codigoCancelamentoCarga, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite, bool retornarDocumentoOperacaoContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            if (codigoCancelamentoCarga > 0)
                query = query.Where(o => o.CargaCancelamento.Codigo == codigoCancelamentoCarga || o.CargaCancelamento == null);
            else
                query = query.Where(o => o.CargaCancelamento == null || o.CargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Carga);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            if (retornarDocumentoOperacaoContainer)
                result = result.Where(obj => obj.OperacaoContainer.Value);
            else
                result = result.Where(obj => !obj.OperacaoContainer.Value || obj.OperacaoContainer == null);


            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoCarga, int codigoCancelamentoCarga, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, bool retornarDocumentoOperacaoContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            if (codigoCancelamentoCarga > 0)
                query = query.Where(o => o.CargaCancelamento.Codigo == codigoCancelamentoCarga || o.CargaCancelamento == null);
            else
                query = query.Where(o => o.CargaCancelamento == null || o.CargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Carga);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            if (retornarDocumentoOperacaoContainer)
                result = result.Where(obj => obj.OperacaoContainer.Value);
            else
                result = result.Where(obj => !obj.OperacaoContainer.Value || obj.OperacaoContainer == null);


            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> Consulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaValePedagio filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtroPesquisa);

            string direcaoOrdenar = parametrosConsulta.DirecaoOrdenar.ToLower();
            switch (parametrosConsulta.PropriedadeOrdenar)
            {
                case "CodigoCargaEmbarcador":
                    query = (direcaoOrdenar == "asc") ? query.OrderBy(obj => obj.Carga.CodigoCargaEmbarcador) : query.OrderByDescending(obj => obj.Carga.CodigoCargaEmbarcador);
                    break;
                case "DataCriacaoCarga":
                    query = (direcaoOrdenar == "asc") ? query.OrderBy(obj => obj.Carga.DataCriacaoCarga) : query.OrderByDescending(obj => obj.Carga.DataCriacaoCarga);
                    break;
                case "Placa":
                    query = (direcaoOrdenar == "asc") ? query.OrderBy(obj => obj.Carga.Veiculo.Placa) : query.OrderByDescending(obj => obj.Carga.Veiculo.Placa);
                    break;
                default:
                    direcaoOrdenar = (direcaoOrdenar == "asc" ? " ascending" : " descending");
                    query = query.OrderBy(parametrosConsulta.PropriedadeOrdenar + direcaoOrdenar);
                    break;
            }

            return query
                .Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Veiculo)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaValePedagio filtroPesquisa)
        {
            var query = Consultar(filtroPesquisa);
            return query.Count();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.Count();
        }

        public int ContarPorCargaESituacaoDiff(int codigoCarga, SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao != situacaoDiff select obj;

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga, SituacaoIntegracao[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && situacao.Contains(obj.SituacaoIntegracao) select obj;

            return result.Count();
        }

        public List<int> BuscarCodigosValePedagiosSemPararPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar select obj;

            return result.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosValePedagiosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.Select(o => o.Codigo).ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaValePedagio filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.CodigoCargaEmbarcador))
                result = result.Where(o => o.Carga.CodigoCargaEmbarcador == filtroPesquisa.CodigoCargaEmbarcador);

            if (filtroPesquisa.DataCargaInicial != null)
                result = result.Where(o => o.Carga.DataCriacaoCarga >= filtroPesquisa.DataCargaInicial);

            if (filtroPesquisa.DataCargaFinal != null)
                result = result.Where(o => o.Carga.DataCriacaoCarga <= filtroPesquisa.DataCargaFinal);

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.NumeroValePedagio))
                result = result.Where(o => o.NumeroValePedagio == filtroPesquisa.NumeroValePedagio);

            if (filtroPesquisa.SituacaoValePedagio != null)
                result = result.Where(o => o.SituacaoValePedagio == filtroPesquisa.SituacaoValePedagio);

            if (filtroPesquisa.SituacaoIntegracao != null)
                result = result.Where(o => o.SituacaoIntegracao == filtroPesquisa.SituacaoIntegracao);

            if (filtroPesquisa.DataIntegracaoInicial != null)
                result = result.Where(o => o.DataIntegracao >= filtroPesquisa.DataIntegracaoInicial);

            if (filtroPesquisa.DataIntegracaoFinal != null)
                result = result.Where(o => o.DataIntegracao <= filtroPesquisa.DataIntegracaoFinal);

            if (filtroPesquisa.CodigoTipoIntegracao > 0)
                result = result.Where(o => o.TipoIntegracao.Codigo == filtroPesquisa.CodigoTipoIntegracao);

            if (filtroPesquisa.Filiais.Any(codigo => codigo == -1))
                result = result.Where(o => filtroPesquisa.Filiais.Contains(o.Carga.Filial.Codigo) || o.Carga.Pedidos.Any(ped => ped.Recebedor != null && filtroPesquisa.Recebedores.Contains(ped.Recebedor.CPF_CNPJ)));

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.NumeroParcialCarga))
                result = result.Where(o => o.Carga.CodigoCargaEmbarcador.StartsWith(filtroPesquisa.NumeroParcialCarga));

            return result;
        }

        #endregion
    }
}
