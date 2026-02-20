using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ValePedagio
{
    public class CargaConsultaValorPedagioIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>
    {
        #region Construtores

        public CargaConsultaValorPedagioIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaConsultaValorPedagioIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Metodos Publicos

        public bool VerificarExistePorCargaAgIntegracao(int codigoCarga)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>()
                 .Where(o => o.Carga.Codigo == codigoCarga && o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);

            return consulta.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao BuscarPorCodigo(int codigo)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>()
                 .Where(o => o.Codigo == codigo);

            return consulta
                .Fetch(o => o.Carga).ThenFetch(o => o.Veiculo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao ConsultaIntegracaoPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>()
                 .Where(o => o.Carga.Codigo == codigoCarga);

            if (situacao != null)
                consulta = consulta.Where(o => o.SituacaoIntegracao == situacao);

            return consulta.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao> ConsultaIntegracaoPorCargas(List<int> codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>()
                 .Where(o => codigoCarga.Contains(o.Carga.Codigo));

            if (situacao != null)
                consulta = consulta.Where(o => o.SituacaoIntegracao == situacao);

            return consulta.Fetch(o => o.Carga).ToList();
        }

        public List<int> ConsultaIntegracaoAgIntegracao(int maximoRegistros, int tentativasLimite, int tempoProximaTentativaMinutos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>();
            var result = from obj in query
                         where (obj.Carga.IntegrandoValePedagio && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.Carga.ProblemaIntegracaoValePedagio && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos)))
                               && obj.Carga.CargaFechada && obj.Carga.SituacaoRoteirizacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido
                               && (obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete || !obj.Carga.ExigeNotaFiscalParaCalcularFrete)
                         select obj;
            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaIntegracaoAgIntegracaoPorCarga(int codigoCarga)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>()
                 .Where(o => o.Carga.Codigo == codigoCarga && o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);

            return consulta.Select(o => o.Codigo).Count();
        }
        public async Task<int> ContarConsultaIntegracaoAgIntegracaoPorCargaAsync(int codigoCarga)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>()
                 .Where(o => o.Carga.Codigo == codigoCarga && o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);

            return await consulta.Select(o => o.Codigo).CountAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao BuscarIntegracaoConsultaporProtocoloCarga(int protocoloCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>();

            var result = from obj in query where obj.Carga.Protocolo == protocoloCarga select obj;

            return result.FirstOrDefault();
        }

        public void RemoverIntegracaoPorCarga(int codigoCarga)
        {
            string sqlArquivo = "DELETE FROM T_CARGA_CONSULTA_VALOR_PEDAGIO_INTEGRACAO_ARQUIVO_ARQUIVO WHERE CCP_CODIGO in (select CCP_CODIGO from T_CARGA_CONSULTA_VALOR_PEDAGIO_INTEGRACAO WHERE CAR_CODIGO = " + codigoCarga + ")"; // SQL-INJECTION-SAFE
            var queryarquivos = this.SessionNHiBernate.CreateSQLQuery(sqlArquivo);
            queryarquivos.ExecuteUpdate();

            string hql = "DELETE FROM T_CARGA_CONSULTA_VALOR_PEDAGIO_INTEGRACAO WHERE CAR_CODIGO = " + codigoCarga; // SQL-INJECTION-SAFE
            var query = this.SessionNHiBernate.CreateSQLQuery(hql);
            query.ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao BuscarPorCargaETipoIntegracao(int codigoCarga, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>()
                .Where(o => o.Carga.Codigo == codigoCarga && tiposIntegracao.Contains(o.TipoIntegracao));

            return query.FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CodigoRoteiroPercurso BuscarRoteiroEPercursoPorCargaTipoIntegracao(int codigoCarga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao == tipoIntegracao)
                .Select(x => new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CodigoRoteiroPercurso
                {
                    CodigoRoteiro = x.CodigoRoteiro,
                    CodigoPercurso = x.CodigoPercurso
                }).FirstOrDefault();
        }
        #endregion
    }
}
