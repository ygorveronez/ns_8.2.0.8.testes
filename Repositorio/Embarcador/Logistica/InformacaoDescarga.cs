using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Logistica
{
    public class InformacaoDescarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga>
    {
        public InformacaoDescarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarPesoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;            
            return result.Sum(obj => (decimal?)obj.PesoDescarga) ?? 0m;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga> _Consultar(DateTime data, int notaFiscal, string placa, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(o => o.Placa.Contains(placa));

            if (notaFiscal > 0)
                result = result.Where(o => o.NotaFiscal == notaFiscal);

            if (codigoCarga > 0)
                result = result.Where(o => o.Carga.Codigo == codigoCarga);

            if (data > DateTime.MinValue)
                result = result.Where(o => o.Data == data);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.InformacaoDescarga> Consultar(DateTime data, int notaFiscal, string placa, int codigoCarga, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(data, notaFiscal, placa, codigoCarga);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(DateTime data, int notaFiscal, string placa, int codigoCarga)
        {
            var result = _Consultar(data, notaFiscal, placa, codigoCarga);

            return result.Count();
        }
    }
}
