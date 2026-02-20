using Avro.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.AbastecimentoInterno
{
    public class LiberacaoAbastecimentoAutomatizadoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao>
    {
        public LiberacaoAbastecimentoAutomatizadoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao> BuscarIntegracoesPendentesPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao>();

            var result = from obj in query where obj.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento == situacao && 
                         ( obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao || obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) && obj.NumeroTentativas <= 10 select obj;

            return result.OrderBy(o => o.Codigo).Take(5).ToList();
        }
        public List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao> BuscarIntegracoesPendentesDIFFSituacoesOcioso(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento> situacoes, int minutos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao>();

            var result = from obj in query
                         where !situacoes.Contains(obj.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento) &&
                         obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao &&
                         obj.DataIntegracao.AddMinutes(minutos) <= DateTime.Now
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(5).ToList();
        }
        public List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao> BuscarPorLiberacaoAbetecimentoAutomatizado(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao>();

            var result = from obj in query
                         where obj.LiberacaoAbastecimentoAutomatizado.Codigo == codigo
                         select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao BuscarPorReserveID(string reserveID)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao>();

            var result = from obj in query
                         where obj.ReserveID == reserveID
                         select obj;

            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao BuscarPorReserveIDEAuthID(string reserveID, string authID)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao>();

            var result = from obj in query
                         where obj.ReserveID == reserveID && obj.AuthID == authID
                         select obj;

            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorLiberacaoAbetecimentoAutomatizado(int codigo, int inicio, int limite)
        {
            var queryLiberacaoAbastecimentoAutomatizadoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao>();
            var resultLiberacaoAbastecimentoAutomatizadoIntegracao = from obj in queryLiberacaoAbastecimentoAutomatizadoIntegracao where obj.LiberacaoAbastecimentoAutomatizado.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultLiberacaoAbastecimentoAutomatizadoIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }
        public int ContarBuscarArquivosPorLiberacaoAbetecimentoAutomatizado(int codigo)
        {
            var queryLiberacaoAbastecimentoAutomatizadoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao>();
            var resultLiberacaoAbastecimentoAutomatizadoIntegracao = from obj in queryLiberacaoAbastecimentoAutomatizadoIntegracao where obj.LiberacaoAbastecimentoAutomatizado.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultLiberacaoAbastecimentoAutomatizadoIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Count();
        }
        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarIntegracaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
    }
}