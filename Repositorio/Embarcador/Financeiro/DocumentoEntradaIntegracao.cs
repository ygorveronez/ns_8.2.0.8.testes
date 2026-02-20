using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoEntradaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao>
    {
        public DocumentoEntradaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao BuscarIntegracaoPorCodigoDocumentoEntrada(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao>();

            var result = from obj in query where obj.DocumentoEntrada.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao BuscarIntegracaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public int ContarConsulta(int codigoDocumentoEntrada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoDocumentoEntrada, situacao);

            return consultaIntegracoes.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao> Consultar(int codigoDocumentoEntrada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaDocumentoEntradaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao>()
                .Where(o => o.DocumentoEntrada.Codigo == codigoDocumentoEntrada);

            if (situacao.HasValue)
                consultaDocumentoEntradaIntegracao = consultaDocumentoEntradaIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaDocumentoEntradaIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao> Consultar(int codigoDocumentoEntrada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoDocumentoEntrada, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao> BuscarPendentesIntegracao(int quantideRegistro, int numeroTentativas, int minutosACadaTentativa)
        {
            var integracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao>()
                .Fetch(o => o.TipoIntegracao)
                .Fetch(o => o.DocumentoEntrada)
                .Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                           (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                           o.NumeroTentativas < numeroTentativas && o.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)))
                .Take(quantideRegistro)
                .ToList();
            return integracao;
        }
    }
}
