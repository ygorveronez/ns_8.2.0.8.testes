using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NFS
{
    public class RegrasAutorizacaoNFSManual : RepositorioBase<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual>
    {
        #region Construtores

        public RegrasAutorizacaoNFSManual(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> _ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, bool? emVigencia, int regraNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual>();
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

            if (emVigencia.HasValue && emVigencia.Value)
                result = result.Where(o => o.Vigencia >= DateTime.Now || o.Vigencia == null);

            if(regraNFSManual > 0)
                result = result.Where(o => o.Codigo != regraNFSManual);

            return result;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, bool? emVigencia, int regraNFSManual, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao, emVigencia, regraNFSManual);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, bool? emVigencia, int regraNFSManual)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, descricao, emVigencia, regraNFSManual);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> BuscarRegraPorFilial(int codigoFilial, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && obj.Filial.Codigo == codigoFilial) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && obj.Filial.Codigo != codigoFilial)
                         select obj.RegrasAutorizacaoNFSManual;

            result = result.Where(o => o.Ativo ==true && o.RegraPorFilial == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> BuscarRegraPorTransportadora(int codigoTransportadora, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.RegrasTransportadoraNFSManual>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && obj.Transportadora.Codigo == codigoTransportadora) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && obj.Transportadora.Codigo != codigoTransportadora)
                         select obj.RegrasAutorizacaoNFSManual;

            result = result.Where(o => o.Ativo == true && o.RegraPorTransportadora == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> BuscarRegraPorTomador(double codigoTomador, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.RegrasTomadorNFSManual>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && obj.Tomador.CPF_CNPJ == codigoTomador) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && obj.Tomador.CPF_CNPJ != codigoTomador)
                         select obj.RegrasAutorizacaoNFSManual;

            result = result.Where(o => o.Ativo == true && o.RegraPorTomador == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> BuscarRegraPorValorPrestacaoServico(decimal valor, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.RegrasValorPrestacaoServicoNFSManual>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.IgualA && obj.Valor == valor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.DiferenteDe && obj.Valor != valor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MaiorIgualQue && obj.Valor <= valor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MaiorQue && obj.Valor < valor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MenorIgualQue && obj.Valor >= valor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual.MenorQue && obj.Valor > valor)
                         select obj.RegrasAutorizacaoNFSManual;

            result = result.Where(o => o.Ativo == true && o.RegraPorValorPrestacaoServico == true && (o.Vigencia >= data || o.Vigencia == null));

            return result.ToList();
        }

        #endregion
    }
}

