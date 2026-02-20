using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ComprovanteCarga
{
    public class ComprovanteCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga>
    {
        #region Construtores

        public ComprovanteCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaComprovanteCarga filtrosPesquisa)
        {
            var consultaComprovanteCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga>();

            if (filtrosPesquisa.Codigo > 0)
                consultaComprovanteCarga = consultaComprovanteCarga.Where(obj => obj.Codigo == filtrosPesquisa.Codigo);

            if (filtrosPesquisa.Carga > 0)
                consultaComprovanteCarga = consultaComprovanteCarga.Where(obj => obj.Carga.Codigo == filtrosPesquisa.Carga);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaComprovanteCarga = consultaComprovanteCarga.Where(obj => obj.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.TipoComprovante > 0)
                consultaComprovanteCarga = consultaComprovanteCarga.Where(obj => obj.TipoComprovante.Codigo == filtrosPesquisa.TipoComprovante);

            if (filtrosPesquisa.MotoristaCarga.Count > 0)
                consultaComprovanteCarga = consultaComprovanteCarga.Where(obj => obj.Carga.Motoristas.Any(m => filtrosPesquisa.MotoristaCarga.Contains(m.Codigo)));

            if (filtrosPesquisa.VeiculosCarga.Count > 0)
                consultaComprovanteCarga = consultaComprovanteCarga.Where(obj => filtrosPesquisa.VeiculosCarga.Contains(obj.Carga.Veiculo.Codigo));

            if (filtrosPesquisa.DataCarga != DateTime.MinValue)
            {
                consultaComprovanteCarga = consultaComprovanteCarga.Where(obj => obj.Carga.DataCriacaoCarga.Date.Equals(filtrosPesquisa.DataCarga.Date));
            }

            consultaComprovanteCarga = consultaComprovanteCarga.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            return consultaComprovanteCarga;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga BuscarPorCodigo(int codigo)
        {
            var consultaComprovanteCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga>()
                .Where(obj => obj.Codigo == codigo);

            return consultaComprovanteCarga.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga BuscarPorCodigoETipoRecebido(int codigo, int codigoTipoComprovante)
        {
            var consultaComprovanteCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga>()
                .Where(obj => obj.Codigo == codigo && obj.TipoComprovante.Codigo == codigoTipoComprovante && obj.Situacao == SituacaoComprovanteCarga.Recebido);

            return consultaComprovanteCarga.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga> ObterComprovantesPendentesPorCarga(List<int> idCargas)
        {
            var consultaComprovanteCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga>()
                .Where(obj => idCargas.Contains( obj.Carga.Codigo) && obj.Situacao == SituacaoComprovanteCarga.Pendente);
            return consultaComprovanteCarga.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaComprovanteCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaComprovanteCarga = Consultar(filtrosPesquisa);

            return ObterLista(consultaComprovanteCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaComprovanteCarga filtrosPesquisa)
        {
            var consultaComprovanteCarga = Consultar(filtrosPesquisa);

            return consultaComprovanteCarga.Count();
        }

        #endregion Métodos Públicos
    }
}
