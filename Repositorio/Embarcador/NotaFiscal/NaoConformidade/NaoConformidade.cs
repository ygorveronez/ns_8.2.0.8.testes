using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NaoConformidade : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>
    {
        #region Construtores

        public NaoConformidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> BuscarPorCodigos(List<int> codigos)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(naoConformidade => codigos.Contains(naoConformidade.Codigo));

            return consultaNaoConformidade.ToList();
        }

        public void AtualizarSituacao(int codigoNaoConformidade, SituacaoNaoConformidade novaSituacao)
        {
            UnitOfWork.Sessao
                .CreateQuery("update NaoConformidade set Situacao = :situacao where Codigo = :codigo")
                .SetInt32("codigo", codigoNaoConformidade)
                .SetEnum("situacao", novaSituacao)
                .ExecuteUpdate();
        }

        public void AtualizarSituacao(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidade filtrosPesquisa, SituacaoNaoConformidade novaSituacao)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("update T_NAO_CONFORMIDADE");
            sql.Append($"  set NCF_SITUACAO = {(int)novaSituacao} ");
            sql.Append($"where CPE_CODIGO = {filtrosPesquisa.CodigoCargaPedido} ");
            sql.Append($"  and INC_CODIGO = {filtrosPesquisa.CodigoItemNaoConformidade} ");

            if (filtrosPesquisa.CodigoXMLNotaFiscal > 0)
                sql.Append($" and NFX_CODIGO = {filtrosPesquisa.CodigoXMLNotaFiscal} ");

            if (filtrosPesquisa.TipoParticipante.HasValue)
                sql.Append($" and NCF_TIPO_PARTICIPANTE = {(int)filtrosPesquisa.TipoParticipante.Value} ");
            else
                sql.Append($" and NCF_TIPO_PARTICIPANTE is null ");

            UnitOfWork.Sessao
                .CreateSQLQuery(sql.ToString())
                .ExecuteUpdate();
        }

        public Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaoConformidade BuscarDadosNaoConformidade(int codigoNaoConformidade)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(naoConformidade => naoConformidade.Codigo == codigoNaoConformidade);

            return consultaNaoConformidade
                .Select(naoConformidade => new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaoConformidade()
                {
                    Codigo = naoConformidade.Codigo,
                    CodigoCFOP = naoConformidade.CargaPedido == null || naoConformidade.CargaPedido.CFOP == null ? 0 : naoConformidade.CargaPedido.CFOP.Codigo,
                    CodigoFilial = naoConformidade.CargaPedido == null || naoConformidade.CargaPedido.Carga == null || naoConformidade.CargaPedido.Carga.Filial == null ? 0 : naoConformidade.CargaPedido.Carga.Filial.Codigo,
                    CodigoItemNaoConformidade = naoConformidade.ItemNaoConformidade.Codigo,
                    CodigoTipoOperacao = naoConformidade.CargaPedido == null || naoConformidade.CargaPedido.Carga == null || naoConformidade.CargaPedido.Carga.TipoOperacao == null ? 0 : naoConformidade.CargaPedido.Carga.TipoOperacao.Codigo,
                    NumeroNotaFiscal = naoConformidade.XMLNotaFiscal == null ? 0 : naoConformidade.XMLNotaFiscal.Numero,
                    Situacao = naoConformidade.Situacao,
                    Grupo = naoConformidade.ItemNaoConformidade.Grupo,
                    SubGrupo = naoConformidade.ItemNaoConformidade.SubGrupo,
                    Area = naoConformidade.ItemNaoConformidade.Area
                })
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> BuscarPorNotaFiscal(int codigoNotaFiscal)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(naoConformidade => naoConformidade.XMLNotaFiscal.Codigo == codigoNotaFiscal);

            return consultaNaoConformidade.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(naoConformidade => naoConformidade.CargaPedido.Codigo == codigoCargaPedido);

            return consultaNaoConformidade.ToList();
        }

        public List<(int CodigoXmlNotaFiscal, SituacaoNaoConformidade Situacao)> BuscarSituacoesNotasFiscaisPorCarga(int codigoCarga)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(naoConformidade => naoConformidade.CargaPedido.Carga.Codigo == codigoCarga && naoConformidade.XMLNotaFiscal != null);

            return consultaNaoConformidade
                .Select(naoConformidade => ValueTuple.Create(naoConformidade.XMLNotaFiscal.Codigo, naoConformidade.Situacao))
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaPorItemNaoConformidadeENota(int codigoItemNaoConformidade, int numeroNota)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(o => o.XMLNotaFiscal.Numero == numeroNota && o.ItemNaoConformidade.Codigo == codigoItemNaoConformidade).Select(obj => obj.CargaPedido.Carga);

            return consultaNaoConformidade.FirstOrDefault();
        }

        public bool ExisteNaoConformidade(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidade filtrosPesquisa)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(naoConformidade => naoConformidade.CargaPedido.Codigo == filtrosPesquisa.CodigoCargaPedido && naoConformidade.ItemNaoConformidade.Codigo == filtrosPesquisa.CodigoItemNaoConformidade);

            if (filtrosPesquisa.CodigoXMLNotaFiscal > 0)
                consultaNaoConformidade = consultaNaoConformidade.Where(naoConformidade => naoConformidade.XMLNotaFiscal.Codigo == filtrosPesquisa.CodigoXMLNotaFiscal);
            else
                consultaNaoConformidade = consultaNaoConformidade.Where(naoConformidade => naoConformidade.XMLNotaFiscal == null);

            if (filtrosPesquisa.TipoParticipante.HasValue)
                consultaNaoConformidade = consultaNaoConformidade.Where(naoConformidade => naoConformidade.TipoParticipante == filtrosPesquisa.TipoParticipante.Value);
            else
                consultaNaoConformidade = consultaNaoConformidade.Where(naoConformidade => naoConformidade.TipoParticipante == null);

            return consultaNaoConformidade.Count() > 0;
        }

        public bool ExistePorNotasFiscais(List<string> chaves)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>();

            consultaNaoConformidade = consultaNaoConformidade.Where(obj => chaves.Contains(obj.XMLNotaFiscal.Chave));

            return consultaNaoConformidade.Any();
        }

        public bool ExistePorNotaFiscal(string chave)
        {
            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>();

            consultaNaoConformidade = consultaNaoConformidade.Where(obj => obj.XMLNotaFiscal.Chave == chave);

            return consultaNaoConformidade.Any();
        }

        public bool ExisteNaoConformidadePendente(int codigoCarga)
        {
            List<SituacaoNaoConformidade> situacoesPendentes = SituacaoNaoConformidadeHelper.ObterSituacoesPendentes();

            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(naoConformidade => naoConformidade.CargaPedido.Carga.Codigo == codigoCarga && situacoesPendentes.Contains(naoConformidade.Situacao));

            return consultaNaoConformidade.Count() > 0;
        }

        public bool ExisteNaoConformidadePendenteComBloqueio(int codigoCarga, int codigoTipoOperacao)
        {
            var consultaBloqueioNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade>()
                .Where(bloqueio => bloqueio.Situacao == true);

            if (codigoTipoOperacao > 0)
                consultaBloqueioNaoConformidade = consultaBloqueioNaoConformidade.Where(bloqueio =>
                    bloqueio.TiposOperacao.Count() == 0 ||
                    bloqueio.TiposOperacao.Any(tipoOperacao => tipoOperacao.Codigo == codigoTipoOperacao)
                );

            List<SituacaoNaoConformidade> situacoesPendentes = SituacaoNaoConformidadeHelper.ObterSituacoesPendentes();

            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(naoConformidade =>
                    naoConformidade.CargaPedido.Carga.Codigo == codigoCarga &&
                    situacoesPendentes.Contains(naoConformidade.Situacao) &&
                    consultaBloqueioNaoConformidade.Any(bloqueio => bloqueio.TipoNaoConformidade.Codigo == naoConformidade.ItemNaoConformidade.Codigo)
                );

            return consultaNaoConformidade.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> ObterPendentesDeEnvio(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>();

            query = query.Where(obj => obj.Situacao != SituacaoNaoConformidade.Concluida);
            query = query.Where(obj => !(obj.EmailEnviado ?? false));

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);
            return query.ToList();
        }

        #endregion Métodos Públicos
    }
}
