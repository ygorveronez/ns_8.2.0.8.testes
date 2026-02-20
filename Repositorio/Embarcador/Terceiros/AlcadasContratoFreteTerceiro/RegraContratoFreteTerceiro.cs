using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class RegraContratoFreteTerceiro : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro>
    {
        public RegraContratoFreteTerceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> _ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (aprovador != null)
                result = result.Where(o => o.Aprovadores.Contains(aprovador));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao);

            if (inicioRegistros > 0 && maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result
                    .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                    .ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> AlcadasPorValor(decimal valor, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Valor != valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Valor == valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && obj.Valor <= valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && obj.Valor < valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && obj.Valor >= valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && obj.Valor > valor)
                         select obj.RegraContratoFreteTerceiro;

            result = result.Where(o => o.RegraPorValorContrato == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> AlcadasPorAcrescimo(decimal valor, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Valor != valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Valor == valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && obj.Valor <= valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && obj.Valor < valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && obj.Valor >= valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && obj.Valor > valor)
                         select obj.RegraContratoFreteTerceiro;

            result = result.Where(o => o.RegraPorValorContrato == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> AlcadasPorDesconto(decimal valor, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Valor != valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Valor == valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && obj.Valor <= valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && obj.Valor < valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && obj.Valor >= valor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && obj.Valor > valor)
                         select obj.RegraContratoFreteTerceiro;

            result = result.Where(o => o.RegraPorValorContrato == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> AlcadasPorTerceiro(double terceiro, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Terceiro.CPF_CNPJ == terceiro) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Terceiro.CPF_CNPJ != terceiro)
                         select obj.RegraContratoFreteTerceiro;

            result = result.Where(o => o.RegraPorTerceiros == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }
    }
}
