using Dominio.Entidades.Embarcador.Logistica;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class QuantidadePorTipoDeCargaDescarregamentoTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga>
    {
        public QuantidadePorTipoDeCargaDescarregamentoTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> BuscarPorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga>()
                .Where(obj => obj.QuantidadePorTipoDeCargaDescarregamento.CentroDescarregamento.Codigo == codigoCentroDescarregamento);
            
            return query
                .Fetch(obj => obj.QuantidadePorTipoDeCargaDescarregamento)
                .Fetch(obj => obj.TipoCarga)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> BuscarPorTipoCargaDiaCentroDescarregamento(List<int> codigosTipoCarga, int codigoCentroDescarregamento, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga>();

            query = query.Where(o => codigosTipoCarga.Contains(o.TipoCarga.Codigo) && o.QuantidadePorTipoDeCargaDescarregamento.CentroDescarregamento.Codigo == codigoCentroDescarregamento && o.QuantidadePorTipoDeCargaDescarregamento.Dia == (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana)((int)data.DayOfWeek) + 1);

            return query
                .Fetch(obj => obj.TipoCarga)
                .Fetch(obj => obj.QuantidadePorTipoDeCargaDescarregamento)
                .ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> BuscarTiposDeCargaPorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga>();

            query = query.Where(o => o.QuantidadePorTipoDeCargaDescarregamento.CentroDescarregamento.Codigo == codigoCentroDescarregamento);

            return query
                .Select(obj => obj.TipoCarga)
                .Distinct()
                .ToList();
        }

        public List<QuantidadePorTipoDeCargaTipoDeCarga> BuscarPorQuantidadeTipoCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga>();

            query = query.Where(o => o.QuantidadePorTipoDeCargaDescarregamento.Codigo == codigo);

            return query
                .Distinct()
                .ToList();
        }

        public List<QuantidadePorTipoDeCargaTipoDeCarga> BuscarPorExcecao(int codigo)
        { 
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga>();
            
            query = query.Where(o => o.QuantidadePorTipoDeCargaDescarregamento.ExcecaoCapacidadeDescarregamento.Codigo == codigo);
            
            return query
                .Fetch(obj => obj.QuantidadePorTipoDeCargaDescarregamento)
                .Distinct()
                .ToList();
        }

        public List<QuantidadePorTipoDeCargaTipoDeCarga> BuscarPorExcecaoTiposDeCarga(int codigo, List<int> tiposDeCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga>();

            query = query.Where(o => o.QuantidadePorTipoDeCargaDescarregamento.ExcecaoCapacidadeDescarregamento.Codigo == codigo && tiposDeCarga.Contains(o.TipoCarga.Codigo));
            
            return query
                .Fetch(obj => obj.QuantidadePorTipoDeCargaDescarregamento)
                .Distinct()
                .ToList();
        }
    }
}
