using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoEmbarcador : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador>
    {
        public CargaIntegracaoEmbarcador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaIntegracaoEmbarcador(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador BuscarPorProtocoloEGrupoPessoas(int protocolo, int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador>();

            query = query.Where(o => o.Protocolo == protocolo && o.TipoOperacao.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return query.FirstOrDefault();
        }

        public List<long> BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador situacao, int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador>();

            query = query.Where(o => o.Situacao == situacao);

            return query.OrderBy(o => o.Codigo).Select(o => o.Codigo).Take(quantidadeRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaCargaIntegracaoEmbarcador filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> query = ObterQueryConsulta(filtro, parametroConsulta);

            return query.OrderBy(parametroConsulta.PropriedadeOrdenar + " " + parametroConsulta.DirecaoOrdenar).Fetch(o => o.Carga).Fetch(o => o.Empresa).ThenFetch(o => o.Localidade).Fetch(o => o.CargaCancelamento).Skip(parametroConsulta.InicioRegistros).Take(parametroConsulta.LimiteRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> ConsultarSemFetch(Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaCargaIntegracaoEmbarcador filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> query = ObterQueryConsulta(filtro, parametroConsulta);

            return ObterLista(query,parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaCargaIntegracaoEmbarcador filtro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> query = ObterQueryConsulta(filtro);

            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.Embarcador.FiltroPesquisaCargaIntegracaoEmbarcador filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador>();

            if (filtro.CodigoVeiculo != null && filtro.CodigoVeiculo.Count() > 0)
                query = query.Where(o => filtro.CodigoVeiculo.Contains(o.Veiculo.Codigo) || o.VeiculosVinculados.Any(vv => filtro.CodigoVeiculo.Contains(vv.Codigo)));

            if (filtro.CodigoMotorista != null && filtro.CodigoMotorista.Count() > 0)
                query = query.Where(o => o.Motoristas.Any(m => filtro.CodigoMotorista.Contains(m.Codigo)));

            if (filtro.CodigoEmpresa != null && filtro.CodigoEmpresa.Count() > 0)
                query = query.Where(o => filtro.CodigoEmpresa.Contains(o.Empresa.Codigo));

            if (filtro.Situacao != null && filtro.Situacao.Count() > 0)
                query = query.Where(o => filtro.Situacao.Contains(o.Situacao));

            if (!string.IsNullOrWhiteSpace(filtro.NumeroCarga))
                query = query.Where(o => o.Carga.CodigoCargaEmbarcador == filtro.NumeroCarga);

            if (!string.IsNullOrWhiteSpace(filtro.NumeroCargaEmbarcador))
                query = query.Where(o => o.NumeroCarga == filtro.NumeroCargaEmbarcador);

            if (filtro.DataInicialCarga.HasValue)
                query = query.Where(o => o.DataCriacaoCarga >= filtro.DataInicialCarga.Value);

            if (filtro.DataFinalCarga.HasValue)
                query = query.Where(o => o.DataCriacaoCarga < filtro.DataFinalCarga.Value.AddDays(1));

            if (filtro.NumeroCTe.HasValue)
                query = query.Where(o => o.CTes.Any(c => c.Numero == filtro.NumeroCTe));

            if (filtro.NumeroMDFe.HasValue)
                query = query.Where(o => o.MDFes.Any(m => m.Numero == filtro.NumeroMDFe));

            if (filtro.SituacaoCarga != null && filtro.SituacaoCarga.Count() > 0)
                query = query.Where(o => filtro.SituacaoCarga.Contains(o.Carga.SituacaoCarga));

            if (filtro.SituacaoCancelamento != null && filtro.SituacaoCancelamento.Count() > 0)
                query = query.Where(o => filtro.SituacaoCancelamento.Contains(o.CargaCancelamento.Situacao));

            if (filtro.CodigoTipoOperacao != null && filtro.CodigoTipoOperacao.Count() > 0)
                query = query.Where(o => filtro.CodigoTipoOperacao.Contains(o.TipoOperacao.Codigo));

            return query;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador BuscarPorProtocoloETipoOperacao(int protocolo, int codigoTipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador>();

            query = query.Where(o => o.Protocolo == protocolo && o.TipoOperacao.Codigo == codigoTipoOperacao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }
    }
}
