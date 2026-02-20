using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class MotivoChamado : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>
    {
        public MotivoChamado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MotivoChamado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Chamados.MotivoChamado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Chamados.MotivoChamado BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>()
                .Where(o => o.Descricao == descricao && o.Status);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> BuscarPorDescricaoAsync(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>()
                .Where(o => o.Descricao == descricao && o.Status);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Chamados.MotivoChamado BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao && o.Status);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> BuscarPorCodigoIntegracaoAsync(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao && o.Status);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Chamados.MotivoChamado BuscarPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Codigo == codigoChamado select obj.MotivoChamado;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Chamados.MotivoChamado BuscarPorTipoMotivoAtendimento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento tipoMotivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();
            var result = from obj in query where obj.TipoMotivoAtendimento == tipoMotivo && obj.Status select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Chamados.MotivoChamado BuscarPorIntegracao(int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();
            var result = query.Where(obj => obj.TipoIntegracao.Any(i => i.Codigo == codigoTipoIntegracao)).FirstOrDefault();

            return result;
        }
        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status, codigoCarga, tipoServicoMultisoftware);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var result = _Consultar(descricao, status, codigoCarga, tipoServicoMultisoftware);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> ConsultarMobile()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();

            var result = from obj in query
                         where (obj.TipoMotivoAtendimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento.Reentrega ||
                               obj.TipoMotivoAtendimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento.Retencao) &&
                               obj.Status
                         select obj;

            return result.ToList();
        }

        public bool ExistePorTipoMotivoAtendimento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento tipoMotivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();
            var result = from obj in query where obj.TipoMotivoAtendimento == tipoMotivo && obj.Status select obj;
            return result.Any();
        }

        public int BuscarCodigoTipoPagamentoMotoristaPorMotivoChamado(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();
            var result = from obj in query where obj.Codigo == codigo select obj.PagamentoMotoristaTipo;
            return result.Select(o => (int?)o.Codigo).FirstOrDefault() ?? 0;
        }

        public int BuscarCodigoMotivoPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.Chamado>();
            var result = from obj in query where obj.Codigo == codigoChamado select obj.MotivoChamado.Codigo;
            return result.FirstOrDefault();
        }

        public bool VerificarSeExisteEscalationList()
        {
            var consultaEscalationList = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();

            return consultaEscalationList.Count() > 0;
        }

        public bool VerificarIntegracoesDuplcadas(List<int> codigos, int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();

            var result = query.Where(obj => obj.TipoIntegracao.Any(tipo => codigos.Contains(tipo.Codigo)) && obj.Codigo == codigoMotivoChamado);

            return result.Any();

        }

        public Dominio.Entidades.Embarcador.Chamados.MotivoChamado BuscarPrimeiroPorGrupoMotivo(int codigoGrupoMotivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();

            var result = query.Where(motivo => motivo.GrupoMotivoChamado.Codigo == codigoGrupoMotivo);

            return result.FirstOrDefault();

        }

        public bool VerificarIntegracoesDuplcadasMotivosDiferente(List<int> codigos, int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();

            var result = query.Where(obj => obj.TipoIntegracao.Any(tipo => codigos.Contains(tipo.Codigo)) && obj.Codigo != codigoMotivoChamado);

            return result.Any();

        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                bool statusBool = status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                result = result.Where(o => o.Status == statusBool);
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                result = result.Where(o => !o.ChamadoDeveSerAbertoPeloEmbarcador);

            if (codigoCarga > 0)
            {
                var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>().Where(o => o.Codigo == codigoCarga);
                result = result.Where(o => queryCarga.Any(c => ((c.TipoOperacao == null) || (c.TipoOperacao != null && c.TipoOperacao.ConfiguracaoTipoOperacaoChamado == null) || (c.TipoOperacao != null && c.TipoOperacao.ConfiguracaoTipoOperacaoChamado != null && !c.TipoOperacao.ConfiguracaoTipoOperacaoChamado.PermitirSelecionarApenasAlgunsMotivosAtendimento) || (c.TipoOperacao != null && c.TipoOperacao.ConfiguracaoTipoOperacaoChamado != null && c.TipoOperacao.ConfiguracaoTipoOperacaoChamado.MotivosChamados.Contains(o)))));
            }

            return result;
        }

        #endregion
    }
}
