using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Pessoas
{
    public class PessoaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>
    {
        #region Construtores

        public PessoaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> Consultar(long codigoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaPessoaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>()
                .Where(o => o.Pessoa.CPF_CNPJ == codigoPessoa);

            if (situacao.HasValue)
                consultaPessoaIntegracao = consultaPessoaIntegracao.Where(o => o.SituacaoIntegracao == situacao);

            return consultaPessoaIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public void GerarIntegracaoPessoa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Cliente pessoa)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            // adicionar aqui novos tipos
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesAGerarPorTipoDocumento = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() {
                   Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SIC
                   , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM
                   , Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus
                };

            //throw new ServicoException("Não é possível gerar integração.");
            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracoesAGerarPorTipoDocumento)
            {
                // pendencia testar se ambiente esta configurado para realizar integração   
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao existeTipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);
                if (existeTipoIntegracao == null)
                    continue;

                //validações por tipo de integração  
                if (existeTipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SIC)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoSIC repositorioIntegracaoSIC = new Repositorio.Embarcador.Configuracoes.IntegracaoSIC(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC = repositorioIntegracaoSIC.BuscarPrimeiroRegistro();

                    // pelas regras: Se tiver a flag marcada #63900 E se não tiver código de integração informado
                    if (!(configuracaoIntegracaoSIC?.RealizarIntegracaoNovosCadastrosPessoaSIC ?? false))
                        continue;
                }
                else if (existeTipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

                    if (!(configuracaoIntegracaoKMM?.PossuiIntegracao ?? false))
                        continue;
                }
                else if (existeTipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoGlobus repositorioIntegracaoGlobus = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus configuracaoIntegracaoGlobus = repositorioIntegracaoGlobus.BuscarPrimeiroRegistro();

                    if (!(configuracaoIntegracaoGlobus?.PossuiIntegracao ?? false))
                        continue;
                }


                // testa se registro de integração existe caso não incluir caso  nao testa se deve atualizar 
                Repositorio.Embarcador.Pessoas.PessoaIntegracao repIntegracao = new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao domIntegracao = repIntegracao.BuscarPorPessoaETipo(pessoa.Codigo, existeTipoIntegracao.Tipo);
                if (domIntegracao == null || existeTipoIntegracao.Tipo == TipoIntegracao.KMM)
                {// não existe integração 
                    if (existeTipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SIC && !string.IsNullOrEmpty(pessoa.CodigoIntegracao))
                        continue;

                    Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao PessoaIntegracao = new Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao()
                    {
                        TipoIntegracao = existeTipoIntegracao,
                        DataIntegracao = DateTime.Now,
                        ProblemaIntegracao = "",
                        NumeroTentativas = 0,
                        Pessoa = pessoa,
                        Protocolo = "",
                        SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                        StatusIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusIntegracaoSIC.ArmazenamentoSolicitado,
                    };
                    repIntegracao.Inserir(PessoaIntegracao);
                }
                else
                {// atualizar se necessario
                    if (!(string.IsNullOrEmpty(pessoa.CodigoIntegracao)) && domIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                    {
                        domIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        repIntegracao.Atualizar(domIntegracao);
                    }
                }
            }
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> BuscarPorPessoa(double Pessoa)
        {
            var PessoaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>()
                .Where(o => o.Pessoa.CPF_CNPJ == Pessoa)
                .ToList();
            return PessoaIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> BuscarPendentesIntegracao(int quantideRegistro, int numeroTentativas, int minutosACadaTentativa)
        {
            var PessoaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>()
                .Fetch(o => o.TipoIntegracao)
                .Fetch(o => o.Pessoa)
                .Where(o => (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                           o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)&& 
                           o.NumeroTentativas < numeroTentativas && o.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                .Take(quantideRegistro)
                .ToList();
            return PessoaIntegracao;
        }


        public Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao BuscarPorPessoaETipo(long Pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var PessoaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>()
                .Where(o => o.Pessoa.CPF_CNPJ == Pessoa && o.TipoIntegracao.Tipo == tipo)
                .FirstOrDefault();

            return PessoaIntegracao;
        }


        public Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao BuscarPorId(int Id)
        {
            var PessoaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>()
                .Where(o => o.Codigo == Id)
                .FirstOrDefault();

            return PessoaIntegracao;
        }



        public Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao BuscarPorPessoaETipoIntegracao(int Pessoa, int codigoTipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>();

            query = query.Where(o => o.Pessoa.CPF_CNPJ == Pessoa && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao BuscarPorCodigo(int codigo)
        {
            var PessoaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return PessoaIntegracao;
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> Consultar(long codigoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(codigoPessoa, situacao);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(long codigoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var consultaIntegracoes = Consultar(codigoPessoa, situacao);

            return consultaIntegracoes.Count();
        }


        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> BuscarPendentesIntegracaoPorTipo(int numeroTentativas, double minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            /*
              pendencia
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>();

            query = query.Where(obj => (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                        (
                                            obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                                            && obj.NumeroTentativas < numeroTentativas
                                            && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)
                                        )) && obj.TipoIntegracao.Tipo == tipo && obj.TipoIntegracao.Ativo);

            return query.Fetch(o => o.GrupoPessoas).Fetch(o => o.Pessoa).OrderBy(o => o.Codigo).Take(25).ToList();
            */
            return null;
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> BuscarPendentesIntegracaoPorDataHora(DateTime dataParametro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>();

            var result = from obj in query
                         where
                            obj.DataIntegracao <= dataParametro
                            && obj.TipoIntegracao.Tipo == tipo && obj.TipoIntegracao.Ativo
                         select obj;

            return result.OrderBy(o => o.Codigo).Take(25).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> BuscarPendentesIntegracaoTrocaMotorista(int numeroTentativas, double minutosACadaTentativa)
        {
            /*
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>();

            query = query.Where(obj => (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                       (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                        obj.NumeroTentativas < numeroTentativas &&
                                        obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))) &&
                                        obj.TipoIntegracao.Ativo &&
                                        obj.TipoIntegracao.IntegrarPessoaTrocaMotorista);
            return query.Fetch(o => o.GrupoPessoas).Fetch(o => o.Pessoa).OrderBy(o => o.Codigo).Take(25).ToList();
            */
            return null;
        }

        #endregion
    }
}

