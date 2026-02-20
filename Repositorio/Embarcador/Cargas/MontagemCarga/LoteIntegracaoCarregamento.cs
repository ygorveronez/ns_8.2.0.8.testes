using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;


namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class LoteIntegracaoCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento>
    {
        #region Construtores

        public LoteIntegracaoCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento> Consultar(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string numeroCarregamento, int codigoEmpresa, string carga)
        {
            var consultaCarregamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento>();

            if (situacao.HasValue)
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            if (dataInicio != DateTime.MinValue)
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.DataIntegracao >= dataInicio);

            if (dataFim != DateTime.MinValue)
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.DataIntegracao <= dataFim);

            if (codigoEmpresa > 0)
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.Carregamentos.Any(obj => obj.Empresa.Codigo == codigoEmpresa));

            if (!string.IsNullOrEmpty(numeroCarregamento))
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.Carregamentos.Any(obj => obj.NumeroCarregamento == numeroCarregamento));

            if (!string.IsNullOrEmpty(carga))
            {
                IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> subQueryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>().Where(obj => obj.CodigoCargaEmbarcador == carga);
                List<int> carregamentosCarga = subQueryCarga.Select(x => x.Carregamento.Codigo).ToList();
                consultaCarregamentoIntegracao = consultaCarregamentoIntegracao.Where(o => o.Carregamentos.Any(obj => carregamentosCarga.Contains(obj.Codigo)));
            }

            return consultaCarregamentoIntegracao.Fetch(obj => obj.Carregamentos).Fetch(obj => obj.TipoIntegracao);
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento> BuscarLoteCarregamentoIntegracaoPendente(int limiteRegistros)
        {
            DateTime dataLimiteProximaTentativa = DateTime.Now.AddMinutes(-5d);
            int numeroTentativasLimite = 3;

            var consultaCarregamentoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento>()
                .Where(o =>
                    o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                    (
                        o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                        o.NumeroTentativas < numeroTentativasLimite &&
                        o.DataIntegracao <= dataLimiteProximaTentativa
                    )
                );

            consultaCarregamentoIntegracao = consultaCarregamentoIntegracao
                .Fetch(obj => obj.TipoIntegracao);

            return consultaCarregamentoIntegracao
                .OrderBy(o => o.Codigo)
                .Skip(0)
                .Take(limiteRegistros)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento BuscarPorCodigoESituacao(int codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? SituacaoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento>();
            var result = from obj in query where obj.Codigo == codigoLote select obj;

            if (SituacaoIntegracao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == SituacaoIntegracao);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.LoteIntegracaoCarregamento> Consultarlotes(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string numeroCarregamento, int codigoEmpresa, string carga, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(dataInicio, dataFim, situacao, numeroCarregamento, codigoEmpresa, carga);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string numeroCarregamento, int codigoEmpresa, string carga)
        {
            var result = Consultar(dataInicio, dataFim, situacao, numeroCarregamento, codigoEmpresa, carga);

            return result.Count();
        }

        #endregion
    }
}
