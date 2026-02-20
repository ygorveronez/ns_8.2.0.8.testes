using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class RegrasRequisicaoMercadoria : RepositorioBase<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria>
    {
        public RegrasRequisicaoMercadoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }        

        private IQueryable<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> _ConsultarRegras(int codigoEmpresa, DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (aprovador != null)
                result = result.Where(o => o.Aprovadores.Contains(aprovador));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> ConsultarRegras(int codigoEmpresa, DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(codigoEmpresa, dataInicio, dataFim, aprovador, descricao);

            if (inicioRegistros > 0 && maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result
                    .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                    .ToList();
        }

        public int ContarConsultaRegras(int codigoEmpresa, DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var result = _ConsultarRegras(codigoEmpresa, dataInicio, dataFim, aprovador, descricao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> AlcadasPorFilial(int filial, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadaFilial>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Filial.Codigo == filial) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Filial.Codigo != filial)
                         select obj.RegrasRequisicaoMercadoria;

            result = result.Where(o => o.RegraPorFilial == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> AlcadasPorMotivo(int motivo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadaMotivo>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Motivo.Codigo == motivo) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Motivo.Codigo != motivo)
                         select obj.RegrasRequisicaoMercadoria;

            result = result.Where(o => o.RegraPorMotivo == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }
    }
}
