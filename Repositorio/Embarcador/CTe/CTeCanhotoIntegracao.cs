using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.CTe
{
    public class CTeCanhotoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao>
    {
        #region Construtores

        public CTeCanhotoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao> BuscarPorCodigoSituacaoFalhaIntegracao(List<int> codigos)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao>();
            var result = query.Where(obj => codigos.Contains(obj.Codigo) && obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao>();
            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao BuscarPorCTeETipoIntegracao(int CTe, int tipoIntegracao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao>();
            var result = query.Where(obj => obj.CTe.Codigo == CTe && obj.TipoIntegracao.Codigo == tipoIntegracao);
            return result.FirstOrDefault();
        }

        public bool ExistePorCTeETipoRegistroIntegracao(int CTe, TipoRegistroIntegracaoCTeCanhoto tipoRegistroIntegracao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao>();
            var result = query.Where(obj => obj.CTe.Codigo == CTe && obj.TipoRegistro == tipoRegistroIntegracao && obj.SituacaoIntegracao == SituacaoIntegracao.Integrado);
            return result.Any();
        }

        public List<int> BuscarIntegracoesPendentes(int tentativas, int intervaloTempoRejeitadas, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao>()
                .Where(o => o.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao && o.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                            (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                            && o.TipoIntegracao.Tentativas > 0 && o.NumeroTentativas < tentativas && o.DataIntegracao <= DateTime.Now.AddHours(-intervaloTempoRejeitadas))))
                .OrderBy(o => o.DataIntegracao);
            return query.Take(limite).Select(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao BuscarIntegracaoAguardandoRetornoPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao>()
                .Where(o => o.CTe.Codigo == codigoCTe && o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno && 
                (o.TipoRegistro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistroIntegracaoCTeCanhoto.Imagem || o.TipoRegistro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistroIntegracaoCTeCanhoto.Confirmacao));
                
            return query.Select(o => o).OrderBy(o => o.TipoRegistro).ToList().FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao> _Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCTeCanhotoIntegracao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.CodigoCarga > 0)
                result = result.Where(o => o.CTe.CargaCTes.Any(p => p.Carga.Codigo == filtrosPesquisa.CodigoCarga));

            if (filtrosPesquisa.Emitente > 0)
                result = result.Where(o => o.CTe.Empresa.Codigo == filtrosPesquisa.Emitente);

            if (filtrosPesquisa.NumeroDocumento > 0)
                result = result.Where(o => o.CTe.Numero == filtrosPesquisa.NumeroDocumento);

            if (filtrosPesquisa.DataEmissaoNFeInicial != DateTime.MinValue)
                result = result.Where(o => o.CTe.XMLNotaFiscais.Any(p => p.DataEmissao >= filtrosPesquisa.DataEmissaoNFeInicial));

            if (filtrosPesquisa.DataEmissaoNFeFinal != DateTime.MinValue)
                result = result.Where(o => o.CTe.XMLNotaFiscais.Any(p => p.DataEmissao <= filtrosPesquisa.DataEmissaoNFeFinal.Date.Add(DateTime.MaxValue.TimeOfDay)));

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataIntegracao >= filtrosPesquisa.DataInicio);

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                result = result.Where(o => o.DataIntegracao <= filtrosPesquisa.DataFim.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.DataEntregaInicial != DateTime.MinValue)
                result = result.Where(o => o.CTe.XMLNotaFiscais.Any(p => p.Canhoto.DataEntregaNotaCliente.Value >= filtrosPesquisa.DataEntregaInicial));

            if (filtrosPesquisa.DataEntregaFinal != DateTime.MinValue)
                result = result.Where(o => o.CTe.XMLNotaFiscais.Any(p => p.Canhoto.DataEntregaNotaCliente.Value <= filtrosPesquisa.DataEntregaFinal));

            if (filtrosPesquisa.DataDigitalizacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.CTe.XMLNotaFiscais.Any(p => p.Canhoto.DataDigitalizacao.Value >= filtrosPesquisa.DataDigitalizacaoInicial));

            if (filtrosPesquisa.DataDigitalizacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.CTe.XMLNotaFiscais.Any(p => p.Canhoto.DataDigitalizacao.Value <= filtrosPesquisa.DataDigitalizacaoFinal));

            if (filtrosPesquisa.DataAprovacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.CTe.XMLNotaFiscais.Any(p => p.Canhoto.DataAprovacaoDigitalizacao.Value >= filtrosPesquisa.DataAprovacaoInicial));

            if (filtrosPesquisa.DataAprovacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.CTe.XMLNotaFiscais.Any(p => p.Canhoto.DataAprovacaoDigitalizacao.Value <= filtrosPesquisa.DataAprovacaoFinal));

            if (filtrosPesquisa.Situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.TipoRegistro.HasValue)
                result = result.Where(o => o.TipoRegistro == filtrosPesquisa.TipoRegistro.Value);

            return result.Fetch(obj => obj.CTe).Fetch(obj => obj.TipoIntegracao);
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCTeCanhotoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = _Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);

        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCTeCanhotoIntegracao filtrosPesquisa)
        {
            var result = _Consultar(filtrosPesquisa);

            return result.Count();
        }
    }
}
