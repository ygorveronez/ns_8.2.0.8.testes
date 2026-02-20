using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Seguros
{
    public class ApoliceSeguro : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>
    {
        #region Construtores

        public ApoliceSeguro(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ApoliceSeguro(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> _ConsultarRelatorio(int seguradora, bool emVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro? responsavel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao? averbadora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            var result = from obj in query select obj;

            if (seguradora > 0)
                result = result.Where(o => o.Seguradora.Codigo == seguradora);

            if (emVigencia)
                result = result.Where(o => o.InicioVigencia.Date <= DateTime.Now.Date && o.FimVigencia.Date >= DateTime.Now.Date);

            if (responsavel != null)
                result = result.Where(o => o.Responsavel == responsavel.Value);

            if (averbadora != null)
                result = result.Where(o => o.SeguradoraAverbacao == averbadora.Value);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> Query(bool validarPessoaEGrupo, string numeroApolice, string numeroAverbacao, int codigoSeguradora, int codigoGrupoPessoas, Dominio.Entidades.Cliente pessoa, DateTime inicioVigencia, DateTime fimVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro? responsavel, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa ativa, string descricaoApolice, bool somenteNaoVencidos, bool exibirEmbarcador, bool usuarioMultiTransportador, List<int> codigosFilialMatriz)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(numeroApolice))
                result = result.Where(o => o.NumeroApolice.Contains(numeroApolice));

            if (!string.IsNullOrWhiteSpace(numeroAverbacao))
                result = result.Where(o => o.NumeroAverbacao.Contains(numeroAverbacao));

            if (codigoSeguradora > 0)
                result = result.Where(o => o.Seguradora.Codigo == codigoSeguradora);

            if (somenteNaoVencidos)
                result = result.Where(o => o.FimVigencia >= DateTime.Now.Date);

            if (empresa > 0 && !usuarioMultiTransportador)
            {
                if (exibirEmbarcador)
                    result = result.Where(o => o.Empresa.Codigo == empresa || o.Responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador);
                else
                    result = result.Where(o => o.Empresa.Codigo == empresa);

            }
            else if (empresa > 0 && usuarioMultiTransportador)
            {
                result = result.Where(o => codigosFilialMatriz.Contains(o.Empresa.Codigo));

            }

            if (codigoGrupoPessoas > 0)
            {
                if (validarPessoaEGrupo)
                    result = result.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas || (o.GrupoPessoas == null && o.Pessoa == null));
                else
                    result = result.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);
            }

            if (pessoa != null)
            {
                if (validarPessoaEGrupo)
                    result = result.Where(obj => obj.Pessoa.CPF_CNPJ == pessoa.CPF_CNPJ || (obj.GrupoPessoas == null && obj.Pessoa == null) || (obj.GrupoPessoas != null && pessoa.GrupoPessoas == obj.GrupoPessoas));
                else
                    result = result.Where(obj => obj.Pessoa.CPF_CNPJ == pessoa.CPF_CNPJ);
            }

            if (inicioVigencia != DateTime.MinValue)
                result = result.Where(o => o.InicioVigencia <= inicioVigencia.Date);

            if (fimVigencia != DateTime.MinValue)
                result = result.Where(o => o.FimVigencia >= fimVigencia.Date);

            if (responsavel != null)
                result = result.Where(o => o.Responsavel == responsavel);

            if (ativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa.Ativa)
                result = result.Where(o => o.Ativa == true);
            else if (ativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa.Inativa)
                result = result.Where(o => o.Ativa == false);

            if (!string.IsNullOrEmpty(descricaoApolice))
                result = result.Where(o => o.DescricaoApolice.Contains(descricaoApolice));


            return result;
        }

        #endregion

        #region Métodos Públicos

        public bool BuscarSePossuiApoliceCadastradaPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>()
                .Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return query.Count() > 0;
        }

        public Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro BuscarPorApoliceSeguradora(string apolice)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            var result = from obj in query where obj.NumeroApolice == apolice && obj.Ativa == true select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro BuscarPorApoliceSeguradoraVigencia(string apolice, int codigoSeuradora, DateTime dataInicio, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            var result = from obj in query where obj.NumeroApolice == apolice && obj.Seguradora.Codigo == codigoSeuradora && obj.InicioVigencia == dataInicio && obj.FimVigencia == dataFim select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> BuscarPorCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.Fetch(o => o.Seguradora).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> BuscarApolicesSeguroAlertar(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta filtrosPesquisa)
        {
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataVigenciaAlertar = dataAtual.AddDays(filtrosPesquisa.DiasAlertarAntesVencimento);

            var consultaApoliceSeguro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>()
                .Where(o => o.FimVigencia <= dataVigenciaAlertar && o.Ativa);

            if (!filtrosPesquisa.AlertarAposVencimento)
                consultaApoliceSeguro = consultaApoliceSeguro.Where(o => o.FimVigencia >= dataAtual);

            if (filtrosPesquisa.DiasRepetirAlerta > 0)
            {
                DateTime dataVigenciaRepetirAlerta = dataAtual.AddDays(-filtrosPesquisa.DiasRepetirAlerta);

                consultaApoliceSeguro = consultaApoliceSeguro.Where(o => (o.DataUltimoAlerta == null) || (o.DataUltimoAlerta <= dataVigenciaRepetirAlerta));
            }
            else
                consultaApoliceSeguro = consultaApoliceSeguro.Where(o => o.DataUltimoAlerta == null);

            return consultaApoliceSeguro.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> Consultar(bool validarPessoaEGrupo, string numeroApolice, string numeroAverbacao, int codigoSeguradora, int codigoGrupoPessoas, Dominio.Entidades.Cliente pessoa, DateTime inicioVigencia, DateTime fimVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro? responsavel, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa ativa, string descricaoApolice, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool somenteNaoVencidos, bool exibirEmbarcador, bool usuarioMultiTransportador, List<int> codigosFilialMatriz)
        {
            var query = Query(validarPessoaEGrupo, numeroApolice, numeroAverbacao, codigoSeguradora, codigoGrupoPessoas, pessoa, inicioVigencia, fimVigencia, responsavel, empresa, ativa, descricaoApolice, somenteNaoVencidos, exibirEmbarcador, usuarioMultiTransportador, codigosFilialMatriz);
            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(bool validarPessoaEGrupo, string numeroApolice, string numeroAverbacao, int codigoSeguradora, int codigoGrupoPessoas, Dominio.Entidades.Cliente pessoa, DateTime inicioVigencia, DateTime fimVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro? responsavel, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa ativa, string descricaoApolice, bool somenteNaoVencidos, bool exibirEmbarcador, bool usuarioMultiTransportador, List<int> codigosFilialMatriz)
        {
            var query = Query(validarPessoaEGrupo, numeroApolice, numeroAverbacao, codigoSeguradora, codigoGrupoPessoas, pessoa, inicioVigencia, fimVigencia, responsavel, empresa, ativa, descricaoApolice, somenteNaoVencidos, exibirEmbarcador, usuarioMultiTransportador, codigosFilialMatriz);
            return query.Count();
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Seguros.ReportApolices> ConsultarRelatorio(int seguradora, bool emVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro? responsavel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao? averbadora, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = _ConsultarRelatorio(seguradora, emVigencia, responsavel, averbadora);

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                query = query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            var list = query.ToList();

            var result = from o in list
                         select new Dominio.Relatorios.Embarcador.DataSource.Seguros.ReportApolices()
                         {
                             Codigo = o.Codigo,
                             TipoSeguradoraAverbacao = o.SeguradoraAverbacao,
                             Seguradora = o.Seguradora.Nome,
                             NumeroApolice = o.NumeroApolice,
                             NumeroAverbacao = o.NumeroAverbacao,
                             InicioVigencia = o.InicioVigencia.ToString("dd/MM/yyyy"),
                             FimVigencia = o.FimVigencia.ToString("dd/MM/yyyy"),
                             ValorLimite = o.ValorLimiteApolice,
                             ResponsavelSeguro = o.Responsavel,
                             NomeEmpresa = o.Empresa?.RazaoSocial ?? "",
                             CNPJTransportadorSemFormato = o.Empresa?.CNPJ ?? "",
                             Pessoa = o.Pessoa?.Nome ?? string.Empty,
                             GrupoPessoa = o.GrupoPessoas?.Descricao ?? string.Empty,
                             VencimentoCertificadoDigital = o.Empresa?.DataFinalCertificado?.ToString("dd/MM/yyyy") ?? string.Empty
                         };

            return result.ToList();
        }

        public int ContarConsultaRelatorio(int seguradora, bool emVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro? responsavel, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao? averbadora)
        {
            var query = _ConsultarRelatorio(seguradora, emVigencia, responsavel, averbadora);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> ConsultarTodas(int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            return query.OrderBy(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();

            //var list = query.ToList();

            //var result = from o in list
            //             select new Dominio.ObjetosDeValor.WebService.Seguro.ApoliceSeguro()
            //             {
            //                 Codigo = o.Codigo,
            //                 Seguradora 
            //                 TipoSeguradoraAverbacao = o.SeguradoraAverbacao,
            //                 Seguradora = o.Seguradora.Nome,
            //                 NumeroApolice = o.NumeroApolice,
            //                 NumeroAverbacao = o.NumeroAverbacao,
            //                 InicioVigencia = o.InicioVigencia.ToString("dd/MM/yyyy"),
            //                 FimVigencia = o.FimVigencia.ToString("dd/MM/yyyy"),
            //                 ValorLimite = o.ValorLimiteApolice,
            //                 ResponsavelSeguro = o.Responsavel,
            //                 NomeEmpresa = o.Empresa?.RazaoSocial ?? "",
            //                 CNPJTransportadorSemFormato = o.Empresa?.CNPJ ?? "",
            //                 Pessoa = o.Pessoa?.Nome ?? string.Empty,
            //                 GrupoPessoa = o.GrupoPessoas?.Descricao ?? string.Empty
            //             };

            //return result.ToList();
        }

        public int ContarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro BuscarPrimeiraApoliceVigenteEmbarcador(DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();

            var result = from obj in query where obj.Ativa && obj.InicioVigencia <= data && obj.FimVigencia >= data && obj.Responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> BuscarVigentePorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>()
                .Where(obj =>
                    obj.Empresa.Codigo == codigoEmpresa &&
                    obj.InicioVigencia.Date <= DateTime.Now.Date &&
                    obj.FimVigencia >= DateTime.Now.Date &&
                    obj.Ativa
                );

            return query.ToList();
        }

        #endregion
    }
}
