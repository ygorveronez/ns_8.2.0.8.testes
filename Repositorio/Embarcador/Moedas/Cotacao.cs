using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Moedas
{
    public class Cotacao : RepositorioBase<Dominio.Entidades.Embarcador.Moedas.Cotacao>
    {
        public Cotacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Cotacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Moedas.Cotacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Cotacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Moedas.Cotacao> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Cotacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Moedas.Cotacao BuscarCotacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacao, DateTime dataBase)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Cotacao>();
            query = query.Where(obj => obj.CotacaoAtiva && (obj.MoedaCotacaoBancoCentral == moedaCotacao && obj.DataVigenciaInicial <= dataBase && obj.DataVigenciaFinal >= dataBase));

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Moedas.Cotacao> BuscarCotacaoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacao, DateTime dataBase)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Cotacao>();
            query = query.Where(obj => obj.CotacaoAtiva && (obj.MoedaCotacaoBancoCentral == moedaCotacao && obj.DataVigenciaInicial <= dataBase && obj.DataVigenciaFinal >= dataBase));

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Moedas.Cotacao BuscarCotacaoAtiva(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacao, DateTime dataBase, int codigoGrupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Cotacao>();

            query = query.Where(obj => obj.CotacaoAtiva && (obj.MoedaCotacaoBancoCentral == moedaCotacao && obj.DataVigenciaInicial <= dataBase && obj.DataVigenciaFinal >= dataBase));

            if (codigoGrupoPessoa > 0)
                query = query.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupoPessoa);
            else
                query = query.Where(obj => obj.GrupoPessoas == null && obj.Cliente == null);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Moedas.Cotacao BuscarCotacaoAtiva(double cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Cotacao>();
            var result = from obj in query where obj.CotacaoAtiva select obj;

            if (cliente > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);
            else
                result = result.Where(obj => obj.Cliente == null);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Moedas.Cotacao BuscarCotacaoAtiva(double cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Cotacao>();
            var result = from obj in query where obj.CotacaoAtiva select obj;

            if (cliente > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);
            else
                result = result.Where(obj => obj.Cliente == null);

            if (moedaCotacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Todas)
                result = result.Where(obj => obj.MoedaCotacaoBancoCentral == moedaCotacao);

            return result.FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.Embarcador.Moedas.Cotacao>> ConsultarAsync(int codigoGrupo, double cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacao, DateTime dataVigencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(codigoGrupo, cliente, ativo, moedaCotacao, dataVigencia);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToListAsync(CancellationToken);
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToListAsync(CancellationToken);
        }

        public Task<int> ContarConsultaAsync(int codigoGrupo, double cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacao, DateTime dataVigencia)
        {
            var result = Consultar(codigoGrupo, cliente, ativo, moedaCotacao, dataVigencia);

            return result.CountAsync(CancellationToken);
        }

        public Task<bool> PossuiCotacaoMesmoPeriodoAsync(int codigo, double cliente, int codigoGrupoPessoa, DateTime? dataVigenciaInicial, DateTime? dataVigenciaFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Cotacao>();
            var result = from obj in query where obj.CotacaoAtiva && obj.Codigo != codigo && obj.MoedaCotacaoBancoCentral == moedaCotacao select obj;

            if (cliente > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);
            else
                result = result.Where(obj => obj.Cliente == null);

            if (codigoGrupoPessoa > 0)
                result = result.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupoPessoa);
            else
                result = result.Where(obj => obj.GrupoPessoas == null);

            result = result.Where(obj => (obj.DataVigenciaInicial.Value <= dataVigenciaInicial.Value && obj.DataVigenciaFinal.Value >= dataVigenciaInicial.Value) ||
                                         (obj.DataVigenciaInicial.Value <= dataVigenciaFinal.Value && obj.DataVigenciaFinal.Value >= dataVigenciaFinal.Value) ||
                                         (obj.DataVigenciaInicial.Value >= dataVigenciaInicial.Value && obj.DataVigenciaFinal.Value <= dataVigenciaFinal.Value));

            return result.AnyAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Moedas.Cotacao> ListarCotacoes(DateTime dataBase)
        {
            var query = SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Moedas.Cotacao>()
                .Where(obj => obj.CotacaoAtiva && obj.DataVigenciaInicial <= dataBase && obj.DataVigenciaFinal >= dataBase);

            return [.. query];
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Moedas.Cotacao> Consultar(int codigoGrupo, double cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moedaCotacao, DateTime dataVigencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.Cotacao>();

            var result = from obj in query select obj;

            if (cliente > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);

            if (codigoGrupo > 0)
                result = result.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupo);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.CotacaoAtiva == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.CotacaoAtiva == false);

            if (moedaCotacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Todas)
                result = result.Where(obj => obj.MoedaCotacaoBancoCentral == moedaCotacao);

            if (dataVigencia != DateTime.MinValue)
                result = result.Where(obj => obj.DataVigenciaInicial <= dataVigencia && obj.DataVigenciaFinal >= dataVigencia);

            return result;
        }

        #endregion Métodos Privados
    }
}
