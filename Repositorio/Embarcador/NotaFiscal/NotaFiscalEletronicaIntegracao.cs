using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalEletronicaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao>
    {
        public NotaFiscalEletronicaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao BuscarIntegracaoPorCodigoNotaFiscal(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao>();

            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao BuscarIntegracaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public int ContarConsulta(int codigoDocumentoEntrada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoDocumentoEntrada, situacao);

            return consultaIntegracoes.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao> Consultar(int codigoNotaFiscalEletronica, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaNotaFiscalEletronicaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao>()
                .Where(o => o.NotaFiscal.Codigo == codigoNotaFiscalEletronica);

            if (situacao.HasValue)
                consultaNotaFiscalEletronicaIntegracao = consultaNotaFiscalEletronicaIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaNotaFiscalEletronicaIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao> Consultar(int codigoNotaFiscalEletronica, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoNotaFiscalEletronica, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao> BuscarPendentesIntegracao(int quantideRegistro, int numeroTentativas, int minutosACadaTentativa)
        {
            var integracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao>()
                .Fetch(o => o.TipoIntegracao)
                .Fetch(o => o.NotaFiscal)
                .Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                           (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                           o.NumeroTentativas < numeroTentativas && o.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)))
                .Take(quantideRegistro)
                .ToList();
            return integracao;
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> BuscarNotasFiscaisProntasParaIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>()
                .Where(obj => obj.Status == Dominio.Enumeradores.StatusNFe.Autorizado || obj.Status == Dominio.Enumeradores.StatusNFe.Cancelado);
            
            var result = from obj in query select obj;

            return result.ToList();
        }
    }
}
