using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class RegrasAutorizacaoAvaria : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria>
    {
        public RegrasAutorizacaoAvaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> _ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria>();
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

            if (etapaAutorizacaoAvaria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Todas)
                result = result.Where(o => o.EtapaAutorizacaoAvaria == etapaAutorizacaoAvaria);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao, etapaAutorizacaoAvaria);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao, etapaAutorizacaoAvaria);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> BuscarRegraPorMotivoAvaria(int codigoMotivoAvaria, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.MotivoAvaria.Codigo == codigoMotivoAvaria) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.MotivoAvaria.Codigo != codigoMotivoAvaria)
                         select obj.RegrasAutorizacaoAvaria;

            result = result.Where(o => o.RegraPorMotivoAvaria == true && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoAvaria == etapaAutorizacaoAvaria);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> BuscarRegraPorOrigem(List<int> codigoOrigem, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && codigoOrigem.Contains(obj.Origem.Codigo)) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && !codigoOrigem.Contains(obj.Origem.Codigo))
                         select obj.RegrasAutorizacaoAvaria;

            result = result.Where(o => o.RegraPorOrigem == true && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoAvaria == etapaAutorizacaoAvaria);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> BuscarRegraPorDestino(List<int> codigoDestino, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasDestino>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && codigoDestino.Contains(obj.Destino.Codigo)) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && !codigoDestino.Contains(obj.Destino.Codigo))
                         select obj.RegrasAutorizacaoAvaria;

            result = result.Where(o => o.RegraPorDestino == true && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoAvaria == etapaAutorizacaoAvaria);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> BuscarRegraPorFilial(int codigoFilial, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasFilial>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.Filial.Codigo == codigoFilial) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.Filial.Codigo != codigoFilial)
                         select obj.RegrasAutorizacaoAvaria;

            result = result.Where(o => o.RegraPorFilial == true && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoAvaria == etapaAutorizacaoAvaria);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> BuscarRegraPorTransportadora(int codigoTransportadora, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.Transportadora.Codigo == codigoTransportadora) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.Transportadora.Codigo != codigoTransportadora)
                         select obj.RegrasAutorizacaoAvaria;

            result = result.Where(o => o.RegraPorTransportadora == true && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoAvaria == etapaAutorizacaoAvaria);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> BuscarRegraPorTipoOperacao(int codigoOperacao, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasTipoOperacao>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.TipoOperacao.Codigo == codigoOperacao) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.TipoOperacao.Codigo != codigoOperacao)
                         select obj.RegrasAutorizacaoAvaria;

            result = result.Where(o => o.RegraPorTipoOperacao == true && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoAvaria == etapaAutorizacaoAvaria);

            return result.ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> BuscarRegraPorValor(decimal valorAvaria, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && obj.Valor == valorAvaria ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && obj.Valor != valorAvaria ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MaiorIgualQue && obj.Valor <= valorAvaria ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MaiorQue && obj.Valor < valorAvaria ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MenorIgualQue && obj.Valor >= valorAvaria ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.MenorQue && obj.Valor > valorAvaria)
                         select obj.RegrasAutorizacaoAvaria;

            result = result.Where(o => o.RegraPorValor == true && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoAvaria == etapaAutorizacaoAvaria);

            return result.ToList();
        }
    }
}

