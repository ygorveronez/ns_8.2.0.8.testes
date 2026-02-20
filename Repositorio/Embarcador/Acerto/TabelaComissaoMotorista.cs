using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class TabelaComissaoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista>
    {
        public TabelaComissaoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista BuscarPorTabelaAcertoViagem(DateTime dataAcerto, int codigoSegmento, int codigoModelo, List<int> codigosTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista>();
            var result = from obj in query
                         where obj.Ativo == true && obj.DataVigencia.Value.Date >= dataAcerto.Date
                         select obj;

            if (codigoSegmento > 0)
                result = result.Where(obj => (obj.Segmentos.Any(o => o.SegmentoVeiculo.Codigo == codigoSegmento)));

            if (codigoModelo > 0)
                result = result.Where(obj => (obj.Modelos.Any(o => o.Modelo.Codigo == codigoModelo)));

            if (codigosTipoOperacao != null && codigosTipoOperacao.Count > 0)
            {
                var queryTipos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaTipoOperacao>();
                queryTipos = queryTipos.Where(obj => codigosTipoOperacao.Contains(obj.TipoOperacao.Codigo));

                result = result.Where(obj => queryTipos.Any(c => c.TabelaComissaoMotorista.Codigo == obj.Codigo));
            }

            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo == true);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => o.Ativo == false);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, status);

            return result.Count();
        }
    }
}