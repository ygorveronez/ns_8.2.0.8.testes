using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class Pesagem : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.Pesagem>
    {
        public Pesagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.Pesagem BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Pesagem>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Pesagem> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPesagem filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPesagem filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorIntegracao(int codigo, int inicio, int limite)
        {
            var queryPesagemIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao>();
            var resultPesagemIntegracao = from obj in queryPesagemIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultPesagemIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntegracao(int codigo)
        {
            var queryPesagemIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao>();
            var resultPesagemIntegracao = from obj in queryPesagemIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultPesagemIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo BuscarIntegracaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public bool ExistePorGuarita(int codigoGuarita)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Pesagem>();

            query = query.Where(o => o.Guarita.Codigo == codigoGuarita);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Logistica.Pesagem BuscarPorGuarita(int codigoGuarita)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Pesagem>();
            var result = query.Where(obj => obj.Guarita.Codigo == codigoGuarita);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.Pesagem BuscarPorCodigoTicket(string codigoTicket)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Pesagem>();
            var result = query.Where(obj => obj.CodigoPesagem == codigoTicket);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.Pesagem BuscarPorCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Pesagem>();
            var result = query.Where(obj => obj.Guarita.Carga.Codigo == codigoCarga);
            return result.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.Pesagem> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPesagem filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Pesagem>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoPesagem))
                result = result.Where(obj => obj.CodigoPesagem.Contains(filtrosPesquisa.CodigoPesagem));

            if (filtrosPesquisa.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBalanca.Todos)
                result = result.Where(o => o.StatusBalanca == filtrosPesquisa.Status);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                result = result.Where(o => o.Guarita.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (filtrosPesquisa.DataPesagemInicial != DateTime.MinValue)
                result = result.Where(o => o.DataPesagem.Date >= filtrosPesquisa.DataPesagemInicial);

            if (filtrosPesquisa.DataPesagemFinal != DateTime.MinValue)
                result = result.Where(o => o.DataPesagem.Date <= filtrosPesquisa.DataPesagemFinal);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                result = result.Where(o => o.Guarita.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo);

            if (filtrosPesquisa.CodigoTransportador > 0)
                result = result.Where(o => o.Guarita.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoMotorista > 0)
                result = result.Where(o => o.Guarita.Carga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista));

            return result;
        }

        #endregion
    }
}
