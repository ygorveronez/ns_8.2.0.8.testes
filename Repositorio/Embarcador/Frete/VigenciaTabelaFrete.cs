using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class VigenciaTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>
    {
        public VigenciaTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> BuscarPorTabela(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete select obj;

            return result.Fetch(obj => obj.Empresa).ThenFetch(obj => obj.Localidade).OrderByDescending(o => o.DataInicial).ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete ValidarVigenciaCompativel(int vigencia, int tabelaFrete, DateTime dataInicial, DateTime? dataFinal, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>();

            var result = from obj in query
                         where
                            obj.Codigo != vigencia
                            && obj.TabelaFrete.Codigo == tabelaFrete
                            && (
                                // Inicia dentro de uma existente ou fim vigencia infinita 
                                (obj.DataInicial <= dataInicial && (dataInicial <= obj.DataFinal && !obj.DataFinal.HasValue)) ||
                                // Finaliza dentro de uma existente
                                (obj.DataInicial <= dataFinal && dataFinal <= obj.DataFinal) ||
                                // Vai conter o inicio de uma existente
                                (dataInicial <= obj.DataInicial && (obj.DataInicial <= dataFinal || !dataFinal.HasValue)) ||
                                // Vai conter o fim de uma existente
                                (dataInicial <= obj.DataFinal && obj.DataFinal <= dataFinal) ||

                                // Validações iguais
                                dataInicial == obj.DataInicial ||
                                (dataInicial == obj.DataFinal && obj.DataFinal.HasValue) ||
                                dataFinal == obj.DataInicial ||
                                (dataFinal == obj.DataFinal && obj.DataFinal.HasValue)
                            )
                         select obj;

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);
            else
                result = result.Where(obj => obj.Empresa == null);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> Buscar(DateTime data, int codigoTabelaFrete, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete && (data.Date >= obj.DataInicial && (data.Date <= obj.DataFinal || !obj.DataFinal.HasValue)) select obj;

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa || obj.Empresa == null);
            else
                result = result.Where(obj => obj.Empresa == null);

            return result.OrderByDescending(obj => obj.Empresa).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> BuscarDataInicial(DateTime data, int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete && data.Date >= obj.DataInicial select obj;

            return result.OrderByDescending(o => o.DataInicial).ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete BuscarVigenciaPorData(DateTime data, int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete && data.Date >= obj.DataInicial && (obj.DataFinal == null || data.Date <= obj.DataFinal) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete BuscarPorTabelaFreteCliente(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(obj => obj.Codigo == codigoTabelaFrete);

            return query
                .Select(o => o.Vigencia)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete BuscarPorData(DateTime dataInicio, DateTime dataFim, int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete && obj.DataInicial.Date == dataInicio.Date && obj.DataFinal == dataFim.Date select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete BuscarPorDataInicio(DateTime dataInicio, int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete && obj.DataInicial.Date == dataInicio.Date && obj.DataFinal == null select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete BuscarCompativel(DateTime dataInicio, DateTime dataFim, int codigoTabelaFrete, int codigoEmpresa)
        {
            var consultaVigenciaTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>()
                .Where(vigencia => vigencia.TabelaFrete.Codigo == codigoTabelaFrete && vigencia.DataInicial.Date == dataInicio.Date && vigencia.DataFinal == dataFim.Date);

            if (codigoEmpresa > 0)
                consultaVigenciaTabelaFrete = consultaVigenciaTabelaFrete.Where(vigencia => vigencia.Empresa.Codigo == codigoEmpresa);
            else
                consultaVigenciaTabelaFrete = consultaVigenciaTabelaFrete.Where(vigencia => vigencia.Empresa == null);

            return consultaVigenciaTabelaFrete.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> _Consultar(DateTime dataInicial, DateTime dataFinal, int codigoTabelaFrete, int empresa, int codigoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataInicial >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataFinal <= dataFinal.AddDays(1).Date);

            if (empresa > 0)
                result = result.Where(o => o.Empresa.Codigo == empresa || o.Empresa == null);

            if (codigoContrato > 0)
            {
                var subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                var codigosvigencia = subQuery.Where(obj => obj.ContratoTransporteFrete.Codigo == codigoContrato
                                    && obj.TabelaFrete.Codigo == codigoTabelaFrete).Select(x => x.Vigencia.Codigo);

                result = result.Where(o => codigosvigencia.Contains(o.Codigo));
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFrete> Consultar(DateTime dataInicial, DateTime dataFinal, int codigoTabelaFrete, int empresa, int codigoContrato, string propriedadeOrdenacao, string dirOrdena, int inicio, int limite)
        {
            var result = _Consultar(dataInicial, dataFinal, codigoTabelaFrete, empresa, codigoContrato);

            return result.Fetch(obj => obj.Empresa).ThenFetch(obj => obj.Localidade).OrderBy(propriedadeOrdenacao, dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(DateTime dataInicial, DateTime dataFinal, int codigoTabelaFrete, int empresa, int codigoContrato)
        {
            var result = _Consultar(dataInicial, dataFinal, codigoTabelaFrete, empresa, codigoContrato);

            return result.Count();
        }
    }
}
