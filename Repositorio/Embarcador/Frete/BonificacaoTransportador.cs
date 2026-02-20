using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class BonificacaoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador>
    {
        #region Construtores

        public BonificacaoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaBonificacaoTransportador filtrosPesquisa)
        {
            var consultaBonificacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador>();

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoTipoCarga > 0)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(o => o.TiposDeCarga.Any(t => t.Codigo == filtrosPesquisa.CodigoTipoCarga));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(o => o.Filiais.Any(f => f.Codigo == filtrosPesquisa.CodigoFilial));

            if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Ativo)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(o => o.Ativo == true);
            else if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Inativo)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(o => o.Ativo == false);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(o => o.DataInicial >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(o => o.DataFinal <= filtrosPesquisa.DataFinal.Value);

            return consultaBonificacaoTransportador;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador BuscarBonificacaoPorCarga(int codigoTransportador, int codigoTipoCarga, int codigoFilial, DateTime data)
        {
            var consultaBonificacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador>()
                .Where(o =>
                    (o.Ativo == true) &&
                    (o.DataInicial <= data.Date || o.DataInicial == null) &&
                    (o.DataFinal >= data.Date || o.DataFinal == null) &&
                    (o.Empresa.Codigo == codigoTransportador) &&
                    (o.TiposDeCarga.Any(t => t.Codigo == codigoTipoCarga) || o.TiposDeCarga.Count == 0) &&
                    (o.Filiais.Any(f => f.Codigo == codigoFilial) || o.Filiais.Count == 0)
                );

            return consultaBonificacaoTransportador.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaBonificacaoTransportador filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaBonificacaoTransportador = Consultar(filtrosPesquisa);

            return ObterLista(consultaBonificacaoTransportador, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaBonificacaoTransportador filtrosPesquisa)
        {
            var consultaBonificacaoTransportador = Consultar(filtrosPesquisa);

            return consultaBonificacaoTransportador.Count();
        }

        public bool ExisteBonificacaoTransportador(int codigoBonificacaoTransportadorDesconsiderar, int codigoTransportador, List<int> codigosTipoCarga, List<int> codigosFilial, DateTime? dataInicial, DateTime? dataFinal)
        {
            var consultaBonificacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador>()
                .Where(o => o.Codigo != codigoBonificacaoTransportadorDesconsiderar && o.Ativo);

            consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(obj => obj.Empresa.Codigo == codigoTransportador);

            if (codigosTipoCarga.Count > 0)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(obj => obj.TiposDeCarga.Any(o => codigosTipoCarga.Contains(o.Codigo)) || obj.TiposDeCarga.Count == 0);
            else
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(obj => obj.Filiais.Count >= 0);

            if (codigosFilial.Count > 0)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(obj => obj.Filiais.Any(o => codigosFilial.Contains(o.Codigo) || obj.Filiais.Count == 0));
            else
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(obj => obj.Filiais.Count >= 0);

            if (dataInicial.HasValue)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(obj => obj.DataInicial >= dataInicial || obj.DataInicial == null);
            else
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(obj => obj.DataInicial != null || obj.DataInicial == null);

            if (dataFinal.HasValue)
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(obj => obj.DataFinal <= dataFinal || obj.DataFinal == null);
            else
                consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(obj => obj.DataFinal != null || obj.DataFinal == null);

            return consultaBonificacaoTransportador.Any();
        }

        public List<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador> BuscarPorCodigos(List<int> codigos)
        {
            var consultaBonificacaoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador>();

            consultaBonificacaoTransportador = consultaBonificacaoTransportador.Where(o => codigos.Contains(o.Codigo));

            return consultaBonificacaoTransportador.ToList();
        }

        #endregion Métodos Públicos
    }
}
