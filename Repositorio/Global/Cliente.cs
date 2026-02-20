using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Entidades.Embarcador.Cargas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using Repositorio.Global;

namespace Repositorio
{
    public class Cliente : RepositorioBase<Dominio.Entidades.Cliente>, Dominio.Interfaces.Repositorios.Cliente
    {
        #region Métodos Construtores
        private CancellationToken _token;
        public Cliente(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Cliente(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { this._token = cancellationToken; }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Cliente> Consultar(Dominio.ObjetosDeValor.FiltroPesquisaCliente filtrosPesquisa, List<double> codigos = null)
        {
            var consultaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            if (codigos != null && codigos.Count > 0)
                consultaCliente = consultaCliente.Where(o => codigos.Contains(o.CPF_CNPJ));

            if (filtrosPesquisa.ListaCnpj?.Count > 0)
                consultaCliente = consultaCliente.Where(o => filtrosPesquisa.ListaCnpj.Contains(o.CPF_CNPJ));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Nome))
            {
                if (filtrosPesquisa.FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoaDentroDeEnderecos)
                {
                    var codigosClientes1 = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                    .Where(o => o.Nome.Contains(filtrosPesquisa.Nome) || o.CodigoIntegracao == filtrosPesquisa.Nome)
                    .Select(c => c.CPF_CNPJ);

                    var codigosClientes2 = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco>()
                     .Where(e => e.CodigoEmbarcador == filtrosPesquisa.Nome)
                     .Select(c => c.Cliente.CPF_CNPJ);

                    consultaCliente = consultaCliente.Where(o => codigosClientes1.Contains(o.CPF_CNPJ) || codigosClientes2.Contains(o.CPF_CNPJ));
                }
                else
                {
                    var codigosClientes1 = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                    .Where(o => o.Nome.Contains(filtrosPesquisa.Nome) || o.CodigoIntegracao == filtrosPesquisa.Nome)
                    .Select(c => c.CPF_CNPJ);

                    consultaCliente = consultaCliente.Where(o => codigosClientes1.Contains(o.CPF_CNPJ));
                }
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
            {
                var codigoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                    .Where(o => o.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao) || o.CodigoIntegracao == filtrosPesquisa.CodigoIntegracao)
                    .Select(c => c.CodigoIntegracao);

                consultaCliente = consultaCliente.Where(o => codigoIntegracao.Contains(o.CodigoIntegracao));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NomeFantasia))
            {
                var codigosClientesNomeFantasia = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(o => o.NomeFantasia.Contains(filtrosPesquisa.NomeFantasia) || o.CodigoIntegracao == filtrosPesquisa.NomeFantasia)
                .Select(c => c.CPF_CNPJ);

                consultaCliente = consultaCliente.Where(o => codigosClientesNomeFantasia.Contains(o.CPF_CNPJ));
            }

            if (filtrosPesquisa.SomenteFilial)
            {
                var consultaFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>()
                    .Where(o => o.CNPJ != null);
                consultaCliente = consultaCliente.Where(obj => consultaFilial.Any(f => double.Parse(f.CNPJ) == obj.CPF_CNPJ));
            }

            if (filtrosPesquisa.SomenteFronteira)
                consultaCliente = consultaCliente.Where(obj => obj.FronteiraAlfandega);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Tipo))
                consultaCliente = consultaCliente.Where(o => o.Tipo.Equals(filtrosPesquisa.Tipo));

            if (filtrosPesquisa.Localidade != null)
                consultaCliente = consultaCliente.Where(o => o.Localidade == filtrosPesquisa.Localidade);

            if (filtrosPesquisa.CodigoPais > 0)
                consultaCliente = consultaCliente.Where(o => o.Pais.Codigo == filtrosPesquisa.CodigoPais || o.Localidade.Pais.Codigo == filtrosPesquisa.CodigoPais);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Telefone))
                consultaCliente = consultaCliente.Where(o => o.Telefone1.Equals(filtrosPesquisa.Telefone));

            if (filtrosPesquisa.CpfCnpj > 0)
                consultaCliente = consultaCliente.Where(o => o.CPF_CNPJ == filtrosPesquisa.CpfCnpj);

            if (filtrosPesquisa.BaixaPagar?.QuantidadeGrupoPessoa > 0 && filtrosPesquisa.BaixaPagar?.CodigoGrupoPessoa > 0)
                consultaCliente = consultaCliente.Where(o => o.GrupoPessoas.Codigo == filtrosPesquisa.BaixaPagar.CodigoGrupoPessoa);

            if (filtrosPesquisa.Modalidades?.Count > 0)
            {
                var queryModalidades = from obj in this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas>() where filtrosPesquisa.Modalidades.Contains(obj.TipoModalidade) select obj.Cliente.CPF_CNPJ;

                consultaCliente = consultaCliente.Where(o => queryModalidades.Contains(o.CPF_CNPJ));
            }

            if (filtrosPesquisa.SomenteSemValorDescarga)
                consultaCliente = consultaCliente.Where(o => !o.ClienteDescargas.Any());

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                consultaCliente = consultaCliente.Where(o => o.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.RaizCNPJ))
                consultaCliente = consultaCliente.Where(o => o.CPF_CNPJ >= Convert.ToDouble(filtrosPesquisa.RaizCNPJ.PadRight(14, '0')) && o.CPF_CNPJ <= Convert.ToDouble(filtrosPesquisa.RaizCNPJ.PadRight(14, '9')));

            if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Ativo)
                consultaCliente = consultaCliente.Where(obj => obj.Ativo);
            else if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Inativo)
                consultaCliente = consultaCliente.Where(obj => !obj.Ativo);

            if (filtrosPesquisa.GeoLocalizacaoTipo != GeoLocalizacaoTipo.Todos)
                consultaCliente = consultaCliente.Where(obj => obj.GeoLocalizacaoTipo == filtrosPesquisa.GeoLocalizacaoTipo);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NomeFantasia))
                consultaCliente = consultaCliente.Where(o => o.NomeFantasia.Contains(filtrosPesquisa.NomeFantasia));

            if (filtrosPesquisa.ListaRaizCnpj?.Count > 0)
            {
                string raizCNPJ1 = "";
                string raizCNPJ2 = "";
                string raizCNPJ3 = "";
                string raizCNPJ4 = "";
                string raizCNPJ5 = "";
                string raizCNPJ6 = "";
                string raizCNPJ7 = "";
                string raizCNPJ8 = "";
                string raizCNPJ9 = "";
                string raizCNPJ10 = "";

                for (int i = 0; i < filtrosPesquisa.ListaRaizCnpj.Count(); i++)
                {
                    if (i == 0)
                        raizCNPJ1 = filtrosPesquisa.ListaRaizCnpj[i];
                    else if (i == 1)
                        raizCNPJ2 = filtrosPesquisa.ListaRaizCnpj[i];
                    else if (i == 2)
                        raizCNPJ3 = filtrosPesquisa.ListaRaizCnpj[i];
                    else if (i == 3)
                        raizCNPJ4 = filtrosPesquisa.ListaRaizCnpj[i];
                    else if (i == 4)
                        raizCNPJ5 = filtrosPesquisa.ListaRaizCnpj[i];
                    else if (i == 5)
                        raizCNPJ6 = filtrosPesquisa.ListaRaizCnpj[i];
                    else if (i == 6)
                        raizCNPJ7 = filtrosPesquisa.ListaRaizCnpj[i];
                    else if (i == 7)
                        raizCNPJ8 = filtrosPesquisa.ListaRaizCnpj[i];
                    else if (i == 8)
                        raizCNPJ9 = filtrosPesquisa.ListaRaizCnpj[i];
                    else if (i >= 9)
                        raizCNPJ10 = filtrosPesquisa.ListaRaizCnpj[i];
                }

                if (!string.IsNullOrWhiteSpace(raizCNPJ1) && !string.IsNullOrWhiteSpace(raizCNPJ2) && !string.IsNullOrWhiteSpace(raizCNPJ3) && !string.IsNullOrWhiteSpace(raizCNPJ4) && !string.IsNullOrWhiteSpace(raizCNPJ5) && !string.IsNullOrWhiteSpace(raizCNPJ6) && !string.IsNullOrWhiteSpace(raizCNPJ7) && !string.IsNullOrWhiteSpace(raizCNPJ8) && !string.IsNullOrWhiteSpace(raizCNPJ9) && !string.IsNullOrWhiteSpace(raizCNPJ10))
                    consultaCliente = consultaCliente.Where(o =>
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ1.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ1.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ2.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ2.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ3.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ3.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ4.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ4.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ5.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ5.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ6.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ6.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ7.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ7.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ8.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ8.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ9.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ9.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ10.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ10.PadRight(14, '9')))
                        );
                else if (!string.IsNullOrWhiteSpace(raizCNPJ1) && !string.IsNullOrWhiteSpace(raizCNPJ2) && !string.IsNullOrWhiteSpace(raizCNPJ3) && !string.IsNullOrWhiteSpace(raizCNPJ4) && !string.IsNullOrWhiteSpace(raizCNPJ5) && !string.IsNullOrWhiteSpace(raizCNPJ6) && !string.IsNullOrWhiteSpace(raizCNPJ7) && !string.IsNullOrWhiteSpace(raizCNPJ8) && !string.IsNullOrWhiteSpace(raizCNPJ9))
                    consultaCliente = consultaCliente.Where(o =>
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ1.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ1.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ2.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ2.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ3.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ3.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ4.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ4.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ5.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ5.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ6.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ6.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ7.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ7.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ8.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ8.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ9.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ9.PadRight(14, '9')))
                        );
                else if (!string.IsNullOrWhiteSpace(raizCNPJ1) && !string.IsNullOrWhiteSpace(raizCNPJ2) && !string.IsNullOrWhiteSpace(raizCNPJ3) && !string.IsNullOrWhiteSpace(raizCNPJ4) && !string.IsNullOrWhiteSpace(raizCNPJ5) && !string.IsNullOrWhiteSpace(raizCNPJ6) && !string.IsNullOrWhiteSpace(raizCNPJ7) && !string.IsNullOrWhiteSpace(raizCNPJ8))
                    consultaCliente = consultaCliente.Where(o =>
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ1.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ1.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ2.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ2.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ3.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ3.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ4.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ4.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ5.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ5.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ6.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ6.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ7.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ7.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ8.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ8.PadRight(14, '9')))
                        );
                else if (!string.IsNullOrWhiteSpace(raizCNPJ1) && !string.IsNullOrWhiteSpace(raizCNPJ2) && !string.IsNullOrWhiteSpace(raizCNPJ3) && !string.IsNullOrWhiteSpace(raizCNPJ4) && !string.IsNullOrWhiteSpace(raizCNPJ5) && !string.IsNullOrWhiteSpace(raizCNPJ6))
                    consultaCliente = consultaCliente.Where(o =>
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ1.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ1.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ2.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ2.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ3.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ3.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ4.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ4.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ5.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ5.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ6.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ6.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ7.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ7.PadRight(14, '9')))
                        );
                else if (!string.IsNullOrWhiteSpace(raizCNPJ1) && !string.IsNullOrWhiteSpace(raizCNPJ2) && !string.IsNullOrWhiteSpace(raizCNPJ3) && !string.IsNullOrWhiteSpace(raizCNPJ4) && !string.IsNullOrWhiteSpace(raizCNPJ5))
                    consultaCliente = consultaCliente.Where(o =>
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ1.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ1.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ2.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ2.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ3.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ3.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ4.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ4.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ5.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ5.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ6.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ6.PadRight(14, '9')))
                        );
                else if (!string.IsNullOrWhiteSpace(raizCNPJ1) && !string.IsNullOrWhiteSpace(raizCNPJ2) && !string.IsNullOrWhiteSpace(raizCNPJ3) && !string.IsNullOrWhiteSpace(raizCNPJ4))
                    consultaCliente = consultaCliente.Where(o =>
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ1.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ1.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ2.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ2.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ3.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ3.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ4.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ4.PadRight(14, '9')))
                        );
                else if (!string.IsNullOrWhiteSpace(raizCNPJ1) && !string.IsNullOrWhiteSpace(raizCNPJ2) && !string.IsNullOrWhiteSpace(raizCNPJ3))
                    consultaCliente = consultaCliente.Where(o =>
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ1.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ1.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ2.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ2.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ3.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ3.PadRight(14, '9')))
                        );
                else if (!string.IsNullOrWhiteSpace(raizCNPJ1) && !string.IsNullOrWhiteSpace(raizCNPJ2))
                    consultaCliente = consultaCliente.Where(o =>
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ1.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ1.PadRight(14, '9'))) ||
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ2.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ2.PadRight(14, '9')))
                        );
                else if (!string.IsNullOrWhiteSpace(raizCNPJ1))
                    consultaCliente = consultaCliente.Where(o =>
                            (o.CPF_CNPJ >= double.Parse(raizCNPJ1.PadRight(14, '0')) && o.CPF_CNPJ <= double.Parse(raizCNPJ1.PadRight(14, '9')))
                        );
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Estado) && filtrosPesquisa.Estado != "0")
                consultaCliente = consultaCliente.Where(o => o.Localidade.Estado.Sigla == filtrosPesquisa.Estado);

            if (filtrosPesquisa.ComGeolocalizacao)
                consultaCliente = consultaCliente.Where(o => o.Latitude != "" && o.Longitude != "");

            if (filtrosPesquisa.AlvoEstrategico)
                consultaCliente = consultaCliente.Where(o => o.AlvoEstrategico);

            if (filtrosPesquisa.CodigoCategoria > 0)
                consultaCliente = consultaCliente.Where(o => o.Categoria.Codigo == filtrosPesquisa.CodigoCategoria);

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                var queryDadosCliente = this.SessionNHiBernate.Query<Dominio.Entidades.DadosCliente>();
                var resultQueryDadosCliente = from obj in queryDadosCliente select obj;
                consultaCliente = consultaCliente.Where(o => resultQueryDadosCliente.Where(d => d.Cliente.CPF_CNPJ == o.CPF_CNPJ && d.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa).Any());
            }

            if (filtrosPesquisa.SomenteAreaRedex)
                consultaCliente = consultaCliente.Where(o => o.AreaRedex);

            if (filtrosPesquisa.SomenteArmador)
                consultaCliente = consultaCliente.Where(o => o.Armador);

            if (filtrosPesquisa.SomenteSupridores)
                consultaCliente = consultaCliente.Where(o => o.HabilitarSolicitacaoSuprimentoDeGas);

            if (filtrosPesquisa.SomenteSemGeolocalizacao)
            {
                consultaCliente = consultaCliente.Where(obj => (obj.Latitude == string.Empty || obj.Longitude == string.Empty || obj.Latitude == null || obj.Longitude == null) && obj.GeoLocalizacaoStatus == Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.NaoGerado);
            }

            if (filtrosPesquisa.PossuiExcecaoCheckinFilaH)
            {
                consultaCliente = consultaCliente.Where(obj => obj.ExcecaoCheckinFilaH == true);
            }

            if (filtrosPesquisa.ListaCodigosRecebedores?.Count > 0)
                consultaCliente = consultaCliente.Where(o => filtrosPesquisa.ListaCodigosRecebedores.Contains(o.CPF_CNPJ));

            if (filtrosPesquisa.ListaCodigosExpedidores?.Count > 0)
                consultaCliente = consultaCliente.Where(o => filtrosPesquisa.ListaCodigosExpedidores.Contains(o.CPF_CNPJ));

            if (filtrosPesquisa.ApenasVinculadosACentroDescarregamento)
            {
                var consultaCentroDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();
                var resultConsultaCentroDescarregamento = from obj in consultaCentroDescarregamento select obj;
                consultaCliente = consultaCliente.Where(o => resultConsultaCentroDescarregamento.Where(d => d.Destinatario.CPF_CNPJ == o.CPF_CNPJ).Any());
            }

            return consultaCliente;
        }

        public CargaCTeIntegracaoArquivo BuscarIntergacaoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<CargaCTeIntegracaoArquivo> BuscarArquivosPorIntegacao(int codigo, int inicio, int limite)
        {
            var queryPessoaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>();
            var resultPessoaIntegracao = from obj in queryPessoaIntegracao where obj.Codigo == codigo select obj;

            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultPessoaIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultCargaCTeIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        public int ContarBuscarArquivosPorIntergacao(int codigo)
        {
            var queryPessoaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao>();
            var resultPessoaIntegracao = from obj in queryPessoaIntegracao where obj.Codigo == codigo select obj;
            var queryCargaCTeIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultCargaCTeIntegracaoArquivo = from obj in queryCargaCTeIntegracaoArquivo where resultPessoaIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;
            return resultCargaCTeIntegracaoArquivo.Count();
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Função criada pois o retorno da roteirização foi adicionado o codigo_cliente e em alguns casos o código está com um valor que não é cliente 
        /// e ocorre erro de foreign key ao tentar inserir nos pontos de passagem.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="codigo_cliente"></param>
        /// <returns></returns>
        public double ValidaCPFCNPJCliente(double codigo, double codigo_cliente)
        {
            string sql = @"
 select top 1 CLI_CGCCPF 
   FROM ( select 1 as ORDEM, CLI_CGCCPF from T_CLIENTE where CLI_CGCCPF = :codigo 
          union all
          select 2 as ORDEM, CLI_CGCCPF from T_CLIENTE where CLI_CGCCPF = :codigo_cliente 
          union all
          select 3 as ORDEM, 0 as CLI_CGCCPF
        ) as CLI 
 order by ordem ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("codigo", codigo);
            query.SetParameter("codigo_cliente", codigo_cliente);
            return query.UniqueResult<double>();
        }

        public Dominio.Entidades.Cliente BuscarPorCPFCNPJ(double cpfCnpj, Dominio.ObjetosDeValor.Embarcador.CTe.CacheObjetoValorCTe cacheObjetoValor = null, List<Dominio.Entidades.Cliente> lstCliente = null)
        {
            if (cacheObjetoValor != null && cacheObjetoValor.lstCacheIndexClientes != null && !cacheObjetoValor.lstCacheIndexClientes.ElementAt(0).BuscarGrupoCnpjCpf(cpfCnpj, cacheObjetoValor.lstCacheIndexClientes))
                return null;

            if (cacheObjetoValor != null && cacheObjetoValor.lstClientes != null)
            {
                var teste = cacheObjetoValor.lstClientes.Where(x => x.CPF_CNPJ == cpfCnpj).FirstOrDefault();
                if (teste != null)
                    return teste;
            }

            if (lstCliente != null)
                return lstCliente.Where(obj => obj.CPF_CNPJ == cpfCnpj).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CPF_CNPJ == cpfCnpj select obj;
            return result
                .Fetch(obj => obj.Pais)
                .Fetch(obj => obj.Localidade)
               .ThenFetch(obj => obj.Estado)
               .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.GrupoPessoas).FirstOrDefault();
        }



        public List<Dominio.Entidades.Cliente> BuscarPorCPFCNPJsParaCache(List<double> cpfCnpjs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where cpfCnpjs.Contains(obj.CPF_CNPJ) select obj;
            return result
                .Fetch(obj => obj.Pais)
                .Fetch(obj => obj.Localidade)
               .ThenFetch(obj => obj.Estado)
               .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.GrupoPessoas).ToList();
        }




        public Dominio.Entidades.Cliente BuscarPorCPFCNPJ(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CPF_CNPJ == cpfCnpj select obj;
            return result
                .Fetch(obj => obj.Localidade)
               .ThenFetch(obj => obj.Estado)
               .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.GrupoPessoas).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Cliente> BuscarPorCPFCNPJAsync(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CPF_CNPJ == cpfCnpj select obj;
            return result
                .Fetch(obj => obj.Localidade)
               .ThenFetch(obj => obj.Estado)
               .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.GrupoPessoas).FirstOrDefaultAsync(_token);
        }

        public Dominio.Entidades.Cliente BuscarPorCPFCNPJSemFetch(double cpfCnpj)
        {
            var consultaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(o => o.CPF_CNPJ == cpfCnpj);

            return consultaCliente.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Cliente> BuscarPorCPFCNPJSemFetchAsync(double cpfCnpj, CancellationToken cancellationToken)
        {
            var consultaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(o => o.CPF_CNPJ == cpfCnpj);

            return consultaCliente.FirstOrDefaultAsync(cancellationToken);
        }

        public bool ExistePorCPFCNPJ(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CPF_CNPJ == cpfCnpj select obj;
            return result.Any();
        }

        public Dominio.Entidades.Cliente BuscarPorNomeVisaoBI(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.NomeVisoesBI == codigo select obj;

            return result
                .Fetch(obj => obj.Pais)
                .Fetch(obj => obj.Localidade)
               .ThenFetch(obj => obj.Estado)
               .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.GrupoPessoas).FirstOrDefault();
        }

        public List<Dominio.Entidades.Cliente> BuscarPorGrupoPessoa(int codigoGrupo, List<double> cnpjDoGrupo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where !cnpjDoGrupo.Contains(obj.CPF_CNPJ) && obj.GrupoPessoas.Codigo == codigoGrupo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarPorGrupoPessoa(int codigoGrupo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.GrupoPessoas.Codigo == codigoGrupo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarDestinatariosCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(obj => obj.Codigo == codigoCarga);

            return query
                .SelectMany(obj => obj.Pedidos)
                .Select(obj => obj.Pedido.Destinatario)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarPorCPFCNPJ(List<double> cpfCnpjs)
        {
            IQueryable<Dominio.Entidades.Cliente> query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            int quantidadeRegistrosConsultarPorVez = 2000;
            int quantidadeConsultas = cpfCnpjs.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Cliente> registrosRetornar = new List<Dominio.Entidades.Cliente>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                registrosRetornar.AddRange(query.Where(o => cpfCnpjs.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CPF_CNPJ))
                                                .Fetch(o => o.GrupoPessoas)
                                                .ToList());

            return registrosRetornar;
        }

        public List<Dominio.Entidades.Cliente> BuscarClientesSemCoordenada(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(obj => (obj.Latitude == string.Empty || obj.Longitude == string.Empty || obj.Latitude == null || obj.Longitude == null) && obj.GeoLocalizacaoStatus == Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.NaoGerado)
                .Fetch(obj => obj.Localidade)
                .Take(limite);

            return query.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarClientesValidarRaioLocalidade(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(obj => obj.Latitude != string.Empty && obj.Latitude != null && obj.Longitude != string.Empty && obj.Longitude != null && obj.GeoLocalizacaoRaioLocalidade == GeoLocalizacaoRaioLocalidade.NaoValidado)
                .Fetch(obj => obj.Localidade)
                .Take(limite);

            return query.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarPorCPFCNPJs(List<double> CPFCNPJs)
        {
            List<Dominio.Entidades.Cliente> result = new List<Dominio.Entidades.Cliente>();
            int take = 1000;
            int start = 0;
            while (start < CPFCNPJs?.Count)
            {
                List<double> tmp = CPFCNPJs.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
                query = query.Where(o => tmp.Contains(o.CPF_CNPJ));

                result.AddRange(query.ToList());
                start += take;
            }
            return result;
        }

        public async Task<List<Dominio.Entidades.Cliente>> BuscarPorCPFCNPJsAsync(List<double> CPFCNPJs)
        {
            List<Dominio.Entidades.Cliente> result = new List<Dominio.Entidades.Cliente>();
            int take = 1000;
            int start = 0;
            while (start < CPFCNPJs?.Count)
            {
                List<double> tmp = CPFCNPJs.Skip(start).Take(take).ToList();
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
                query = query.Where(o => tmp.Contains(o.CPF_CNPJ));

                result.AddRange(await query.ToListAsync(_token));
                start += take;
            }
            return result;
        }

        public List<Dominio.Entidades.Cliente> BuscarPorVariosCPFCNPJ(List<double> cpfCnpjs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where cpfCnpjs.Contains(obj.CPF_CNPJ) select obj;

            return result.ToList();
        }

        public List<string> BuscarNomesPorCPFCNPJs(List<double> cpfCnpjs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where cpfCnpjs.Contains(obj.CPF_CNPJ) select obj;

            return result.Select(o => o.Nome).ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarPorCodigos(List<double> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where codigos.Contains(obj.CPF_CNPJ) select obj;
            return result
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .ThenFetch(obj => obj.Pais)
                .ToList();
        }

        public async Task<List<Dominio.Entidades.Cliente>> BuscarPorCodigosIntegracaoAsync(List<string> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Cliente>();

            List<Dominio.Entidades.Cliente> resultado = new List<Dominio.Entidades.Cliente>();

            for (int i = 0; i < codigos.Count; i += pageSize)
            {
                List<string> bloco = codigos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                    .Where(obj => bloco.Contains(obj.CodigoIntegracao));

                List<Dominio.Entidades.Cliente> clientes = await query.ToListAsync(CancellationToken);
                resultado.AddRange(clientes);
            }

            return resultado;
        }


        public List<Dominio.Entidades.Cliente> BuscarClientesDigitalizamCanhoto()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.DigitalizaCanhoto == true select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Cliente BuscarPorNome(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.Nome.Equals(nome) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPorNomeEndereco(string nome, string endereco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.Nome.Equals(nome) && obj.Endereco.Equals(endereco) && obj.Tipo == "E" select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPorCPFCNPJEndereco(double cpfCnpj, string endereco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CPF_CNPJ == cpfCnpj && obj.Endereco.Equals(endereco) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPorCodigoIntegracao(string codigoIntegracao, List<Dominio.Entidades.ClienteIndex> lstClienteCodigoIntegracao = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Cliente> BuscarPorCodigoIntegracaoAsync(string codigoIntegracao, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) select obj;
            return result.FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Cliente BuscarPorCodigoIntegracao(string codigoIntegracao, List<Dominio.Entidades.Cliente> lstClientes)
        {
            if (lstClientes != null && lstClientes.Count > 0)
                return lstClientes.Where(obj => obj.CodigoIntegracao.Equals(codigoIntegracao)).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) select obj;
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Cliente BuscarPorCodigoAlternativo(string codigoAlternativo)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CodigoAlternativo.Equals(codigoAlternativo) select obj;
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Cliente BuscarPorCodigoIntegracaoComEndereco(string codigoIntegracao, List<Dominio.Entidades.ClienteIndex> lstClienteCodigoIntegracao = null)
        {

            //angelopendenciadeindice

            //if (lstClienteCodigoIntegracao != null)
            //    return lstClienteCodigoIntegracao.Where(x => x.CodigoIntegracao.Equals(codigoIntegracao)).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) || obj.Enderecos.Any(x => x.CodigoEmbarcador == codigoIntegracao) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Cliente> BuscarPorCodigosIntegracao(List<string> codigosIntegracao)
        {

            List<Dominio.Entidades.Cliente> lstCliente = new List<Dominio.Entidades.Cliente>();
            int quantidadeRegistrosConsultarPorVez = 2000;
            int quantidadeConsultas = codigosIntegracao.Count / quantidadeRegistrosConsultarPorVez;

            for (int i = 0; i <= quantidadeConsultas; i++)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>().Where(x => codigosIntegracao.Skip(i * quantidadeRegistrosConsultarPorVez).
                Take(quantidadeRegistrosConsultarPorVez).Contains(x.CodigoIntegracao));
                query = query
                    .Fetch(obj => obj.Pais)
                    .Fetch(obj => obj.Localidade)
                   .ThenFetch(obj => obj.Estado)
                   .ThenFetch(obj => obj.Pais)
                    .Fetch(obj => obj.GrupoPessoas);
                lstCliente.AddRange(query.ToList());
            }
            return lstCliente;
        }


        public List<Dominio.Entidades.Cliente> BuscarPorCNPJsCPFs(List<double> CNPJsCPFs)
        {

            List<Dominio.Entidades.Cliente> lstCliente = new List<Dominio.Entidades.Cliente>();
            int quantidadeRegistrosConsultarPorVez = 2000;
            int quantidadeConsultas = CNPJsCPFs.Count / quantidadeRegistrosConsultarPorVez;

            for (int i = 0; i <= quantidadeConsultas; i++)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>().Where(x => CNPJsCPFs.Skip(i * quantidadeRegistrosConsultarPorVez).
                Take(quantidadeRegistrosConsultarPorVez).Contains(x.CPF_CNPJ));
                query = query
                    .Fetch(obj => obj.Pais)
                    .Fetch(obj => obj.Localidade)
                   .ThenFetch(obj => obj.Estado)
                   .ThenFetch(obj => obj.Pais)
                    .Fetch(obj => obj.GrupoPessoas);
                lstCliente.AddRange(query.ToList());
            }
            return lstCliente;
        }


        public Dominio.Entidades.Cliente BuscarPorCodigoDocumento(string codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.CodigoDocumento.Equals(codigoDocumento) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPorCodigoDocumentoETipo(string codigoDocumento, string tipoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.CodigoDocumento.Equals(codigoDocumento) && obj.Tipo == tipoPessoa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Cliente> BuscarClientesPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Cliente BuscarPorCodigoIntegracaoVtex(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                query = query.Where(o => o.OutrosCodigosIntegracao.Contains(codigoIntegracao));

            return query.Select(obj => obj).FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPorPassaporte(string passaporte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.RG_Passaporte == passaporte && obj.Tipo == "E" select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarFronteiraPorLocalidade(int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.FronteiraAlfandega == true && obj.Localidade.Codigo == codigoLocalidade select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Cliente> BuscarFronteiraPorCPFCNPJAsync(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CPF_CNPJ == cpfCnpj select obj;
            return result
                .Fetch(obj => obj.Localidade)
               .ThenFetch(obj => obj.Estado)
               .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.GrupoPessoas).FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Cliente BuscarFronteiraPorCPFCNPJ(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CPF_CNPJ == cpfCnpj select obj;
            return result
                .Fetch(obj => obj.Localidade)
               .ThenFetch(obj => obj.Estado)
               .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.GrupoPessoas).FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPorRazaoExterior(string razaoCliente, string endereco, List<Dominio.Entidades.ClienteIndex> lstClienteCodigoIntegracao = null)
        {
            //angelopendenciadeindice
            //if (lstClienteCodigoIntegracao != null)
            //    return lstClienteCodigoIntegracao.Where(obj => obj.Tipo == "E" && obj.Ativo && obj.Nome.ToLower().Equals(razaoCliente.ToLower()) && obj.Endereco.ToLower().Equals(endereco.ToLower())).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.Tipo == "E" && obj.Ativo && obj.Nome.ToLower().Equals(razaoCliente.ToLower()) && obj.Endereco.ToLower().Equals(endereco.ToLower()) select obj;
            return result.Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarUltimoClienteEntregaPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            var result = query.Where(entrega => entrega.Carga.Codigo == codigoCarga);

            return result
                .OrderByDescending(cargaEntrega => cargaEntrega.Ordem)
                .Select(cargaEntrega => cargaEntrega.Cliente)
                .Fetch(cliente => cliente.Localidade)
                .ThenFetch(localidade => localidade.Estado)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPorNomeIgualOuParecido(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var select1 = from obj in query
                          where
        obj.Nome.ToLower().Equals(nome.ToLower()) ||
        obj.NomeFantasia.ToLower().Equals(nome.ToLower())
                          select obj;

            var result1 = select1.FirstOrDefault();

            if (result1 != null)
                return result1;

            var select2 = from obj in query
                          where
        obj.Nome.ToLower().Contains(nome.ToLower()) ||
        obj.NomeFantasia.ToLower().Contains(nome.ToLower())
                          select obj;
            var result2 = select2.FirstOrDefault();

            return result2;
        }

        public double BuscarPorProximoExterior()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.Tipo == "E" select obj;

            double? retorno = result.Max(o => (double?)o.CPF_CNPJ);

            return retorno.HasValue && retorno.Value > 0d ? retorno.Value + 1 : 100000000000000;
        }

        public List<Dominio.Entidades.Cliente> Consultar(Dominio.ObjetosDeValor.FiltroPesquisaCliente filtrosPesquisa, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, List<double> codigos = null)
        {
            var consultaCliente = Consultar(filtrosPesquisa, codigos);

            propriedadeOrdenacao = string.IsNullOrWhiteSpace(propriedadeOrdenacao) ? "Nome" : propriedadeOrdenacao;
            direcaoOrdenacao = (direcaoOrdenacao == "asc") || string.IsNullOrWhiteSpace(direcaoOrdenacao) ? "ascending" : "descending";

            consultaCliente = consultaCliente.Fetch(o => o.Localidade).OrderBy($"{propriedadeOrdenacao} {direcaoOrdenacao}");

            if ((inicioRegistros > 0) || (maximoRegistros > 0))
                consultaCliente = consultaCliente.Skip(inicioRegistros).Take(maximoRegistros);

            return consultaCliente.Timeout(120)
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.Atividade)
                .Fetch(obj => obj.RecebedorColeta)
                .Fetch(obj => obj.GrupoPessoas)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.GrupoPessoas)
                .ThenFetch(obj => obj.RecebedorColeta)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.FiltroPesquisaCliente filtrosPesquisa)
        {
            var consultaCliente = Consultar(filtrosPesquisa);

            return consultaCliente.Count();
        }

        public List<Dominio.Entidades.Cliente> BuscarPorRaizCNPJ(string raizCNPJ)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query select obj;

            result = result.Where(o => o.Tipo.Equals("J"));
            result = result.Where(o => o.CPF_CNPJ >= Convert.ToDouble(raizCNPJ.PadRight(14, '0')) && o.CPF_CNPJ <= Convert.ToDouble(raizCNPJ.PadRight(14, '9')));

            return result.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarPorRaizCNPJ(string raizCNPJ, int codigoGrupoPessoa)
        {
            //string query = "FROM Cliente WHERE Tipo = 'J' AND (GrupoPessoas.Codigo IS NULL or GrupoPessoas.Codigo = "+ Convert.ToString(codigoGrupoPessoa) + ") ";
            //query += " AND CPF_CNPJ BETWEEN " + Convert.ToString(Convert.ToDouble(raizCNPJ.PadRight(14, '0'))) + " AND " + Convert.ToString(Convert.ToDouble(raizCNPJ.PadRight(14, '9')));
            //var hql = this.SessionNHiBernate.CreateQuery(query);
            //return hql.List<Dominio.Entidades.Cliente>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query select obj;

            result = result.Where(o => o.Tipo.Equals("J"));
            result = result.Where(o => o.GrupoPessoas.Codigo.Equals(codigoGrupoPessoa) || o.GrupoPessoas == null);
            result = result.Where(o => o.CPF_CNPJ >= Convert.ToDouble(raizCNPJ.PadRight(14, '0')) && o.CPF_CNPJ <= Convert.ToDouble(raizCNPJ.PadRight(14, '9')));

            return result.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarPorDataAtualizacao(DateTime dataAtualizacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query select obj;

            result = result.Where(o => !o.Integrado.HasValue || !o.Integrado.Value);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarBuscarPorDataAtualizacao(DateTime dataAtualizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query select obj;

            result = result.Where(o => !o.Integrado.HasValue || !o.Integrado.Value);

            return result.Count();
        }

        public List<Dominio.Entidades.Cliente> BuscarTransportadorTerceiroPorDataAtualizacao(DateTime dataAtualizacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>();

            var result = from obj in query select obj;

            if (dataAtualizacao > DateTime.MinValue)
                result = result.Where(o => !o.Integrado.HasValue || !o.Integrado.Value);

            return result.Select(o => o.ModalidadePessoas.Cliente).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarTransportadorTerceiroBuscarPorDataAtualizacao(DateTime dataAtualizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas>();

            var result = from obj in query select obj;

            if (dataAtualizacao > DateTime.MinValue)
                result = result.Where(o => !o.Integrado.HasValue || !o.Integrado.Value);

            return result.Count();
        }

        public List<double> BuscarCodigosTransportadoresTerceiros()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query select obj;

            result = result.Where(o => o.Modalidades.Any(m => m.TipoModalidade == TipoModalidade.TransportadorTerceiro));

            return result.Select(obj => obj.CPF_CNPJ).Distinct().ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarPorRaizCNPJ(string raizCNPJ, int codigoGrupoPessoa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query select obj;

            result = result.Where(o => o.Tipo.Equals("J"));
            result = result.Where(o => o.GrupoPessoas.Codigo.Equals(codigoGrupoPessoa) || o.GrupoPessoas == null);
            result = result.Where(o => o.CPF_CNPJ >= Convert.ToDouble(raizCNPJ.PadRight(14, '0')) && o.CPF_CNPJ <= Convert.ToDouble(raizCNPJ.PadRight(14, '9')));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarBuscarPorRaizCNPJ(string raizCNPJ, int codigoGrupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query select obj;

            result = result.Where(o => o.Tipo.Equals("J"));
            result = result.Where(o => o.GrupoPessoas.Codigo.Equals(codigoGrupoPessoa) || o.GrupoPessoas == null);
            result = result.Where(o => o.CPF_CNPJ >= Convert.ToDouble(raizCNPJ.PadRight(14, '0')) && o.CPF_CNPJ <= Convert.ToDouble(raizCNPJ.PadRight(14, '9')));

            return result.Count();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioClientes> BuscarRemetentesPorCTes(int[] series, DateTime dataInicial, DateTime dataFinal, string cpfCnpjCliente)
        {
            string query = @"select DISTINCT
	                                r.pct_cpf_cnpj CNPJ,
	                                cl.cli_ierg IE,
	                                cl.cli_nome Razao,
	                                cl.cli_nomefantasia Fantasia,
	                                cl.cli_endereco Endereco,
                                    cl.cli_numero Numero,
                                    cl.cli_bairro Bairro,
	                                cl.cli_cep CEP,
	                                l.loc_descricao Cidade,
	                                l.uf_sigla UF,
	                                cl.cli_email Email,
	                                cl.cli_fone Telefone,	
                                    cl.cli_datacad DataCadastro,  
	                                tp.vti_descricao FreteTipoVeiculo,
	                                fr.ftv_advalore AdValorem,
	                                fr.ftv_valor_descarga ValorDescarga,
	                                fr.ftv_valor_pedagio ValorPedagio,
	                                fr.ftv_valor_frete ValorFrete,
                                    fr.FTV_PERCENTUAL_GRIS GRIS
                                from t_cte c
                                join t_cte_participante r on c.con_remetente_cte = r.pct_codigo
                                join t_cliente cl on cl.cli_cgccpf = r.pct_cpf_cnpj
                                join t_localidades l on cl.loc_codigo = l.loc_codigo
                                left join t_frete_tipo_veiculo fr on fr.ftv_cliente_origem = cl.cli_cgccpf and ftv_status = 'A'
                                left join t_veiculo_tipo tp on tp.vti_codigo = fr.vti_codigo
                              where c.con_status = 'A' ";

            if (!string.IsNullOrWhiteSpace(cpfCnpjCliente))
                query += " AND cl.cli_cgccpf = " + cpfCnpjCliente;

            if (dataInicial != DateTime.MinValue)
                query += " AND cl.cli_datacad >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND cl.cli_datacad < '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if (series != null && series.Count() > 0)
            {
                if (series.Count() == 1)
                    query += " AND c.CON_SERIE = '" + series.FirstOrDefault() + "'";
                else
                {
                    string listaSeries = string.Join(",", series);
                    query += " AND c.CON_SERIE IN (" + listaSeries + ")";
                }
            }

            query += " order by cl.cli_nome";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioClientes)));

            return nhQuery.List<Dominio.ObjetosDeValor.Relatorios.RelatorioClientes>();

            //result = result.Where(o => series.Contains(o.Serie.Codigo));

        }

        public Dominio.ObjetosDeValor.Cliente[] BuscarTodosComGeolocalizacao(bool apenasFronteiraAlfandega = false, bool EnderecosSecundarios = false, bool apenasJuridico = false)
        {
            string sql = @"
                select
                    Cliente.CLI_CGCCPF Codigo,
					Cliente.CLI_NOME Nome,
					Cliente.CLI_LATIDUDE Latitude,
					Cliente.CLI_LONGITUDE Longitude,
					Cliente.CLI_TIPO_AREA TipoArea,
					Cliente.CLI_RAIO_METROS Raio,
					Cliente.CLI_AREA Area
                from
	                T_CLIENTE Cliente 
                where
                    Cliente.CLI_LATIDUDE != ''
                    and Cliente.CLI_LONGITUDE != '' ";

            if (apenasFronteiraAlfandega)
            {
                sql += " and Cliente.CLI_FRONTEIRA_ALFANDEGA = 1 ";
            }

            //Feito validação devido a quantidade de cliente na Riachuelo e maroni (marcar nas sessoes de troca de alvo e psicoes a tag GeolocalizacaoApenasJuridico = true
            if (apenasJuridico)
                sql += " and Cliente.CLI_FISJUR = 'J' ";

            if (EnderecosSecundarios)
            {
                sql += @"
                UNION

                select
                    Cliente.CLI_CGCCPF Codigo,
	                Cliente.CLI_NOME Nome,
	                endereco.COE_LATIDUDE Latitude,
	                endereco.COE_LONGITUDE Longitude,
	                endereco.COE_TIPO_AREA TipoArea,
	                endereco.COE_RAIO_METROS Raio,
	                endereco.COE_AREA Area 
	                from T_CLIENTE_OUTRO_ENDERECO endereco
                    INNER join T_CLIENTE cliente on cliente.CLI_CGCCPF = endereco.CLI_CGCCPF
                where
	                endereco.COE_LATIDUDE != ''
					AND endereco.COE_LONGITUDE != '' 
                    AND Cliente.CLI_LATIDUDE != ''
                    AND Cliente.CLI_LONGITUDE != '' ";

                if (apenasFronteiraAlfandega)
                {
                    sql += " and Cliente.CLI_FRONTEIRA_ALFANDEGA = 1 ";
                }

                //Feito validação devido a quantidade de cliente na Riachuelo
                //sql += " and Cliente.CLI_FISJUR = 'J' ";
            }

            sql += " order by Cliente.CLI_CGCCPF";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            var clientesDynamic = query.List<dynamic>().ToArray();

            System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = "." };
            List<Dominio.ObjetosDeValor.Cliente> clientes = new List<Dominio.ObjetosDeValor.Cliente>();
            int total = clientesDynamic.Length;
            for (int i = 0; i < total; i++)
            {
                try
                {
                    clientes.Add(new Dominio.ObjetosDeValor.Cliente
                    {
                        Codigo = clientesDynamic[i][0],
                        Nome = clientesDynamic[i][1],
                        Latitude = Convert.ToDouble(clientesDynamic[i][2], provider),
                        Longitude = Convert.ToDouble(clientesDynamic[i][3], provider),
                        TipoArea = (clientesDynamic[i][4] != null) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea)clientesDynamic[i][4] : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Raio,
                        Raio = clientesDynamic[i][5] ?? 0,
                        Area = clientesDynamic[i][6],
                    });
                }
                catch (Exception)
                {
                    //VAMOS ENTERRAR A EX pois caso o cliente tem um erro na lat e log esse nao deve ser validado.
                }

            }
            return clientes.ToArray();

        }

        public bool PossuiClientesRedex()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            query = query.Where(o => o.AreaRedex);

            return query.Any();
        }

        public bool ValidarCodigoExiste(string codigoPessoaEmbarcador, long codigoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query
                         where
                             obj.OutrosCodigosIntegracao.Contains(codigoPessoaEmbarcador)
                             && obj.Ativo
                             && obj.CPF_CNPJ != codigoPessoa
                         select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Cliente> BuscarClientePontoApoio()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query select obj;

            result = result.Where(o => o.EhPontoDeApoio == true);

            return result.ToList();
        }

        public List<double> BuscarCpfCnpjClienteArmazemResponsavel(double CpfcnpjArmazem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query select obj;

            result = result.Where(o => o.ArmazemResponsavel.CPF_CNPJ == CpfcnpjArmazem);

            return result.Select(x => x.CPF_CNPJ).ToList();
        }
        public async Task<List<double>> BuscarCpfCnpjClienteArmazemResponsavelAsync(double CpfcnpjArmazem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query select obj;

            result = result.Where(o => o.ArmazemResponsavel.CPF_CNPJ == CpfcnpjArmazem);

            return await result.Select(x => x.CPF_CNPJ).ToListAsync();
        }

        public List<Dominio.Entidades.Cliente> ObterFiliaisClientesRelacionadas(double cpfCnpjCliente)
        {
            var consultaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(cliente => cliente.FilialCliente.Any(filialCliente => filialCliente.CPF_CNPJ == cpfCnpjCliente));

            return consultaCliente.ToList();
        }

        public bool ExisteClienteComFiliaisClientesRelacionadas()
        {
            var consultaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(cliente => cliente.PossuiFilialCliente == true);

            return consultaCliente.Count() > 0;
        }

        public List<Dominio.Entidades.Cliente> BuscarTodosFornecedoresAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();

            var result = from obj in query
                         where obj.ModalidadePessoas.Cliente.Ativo
                            && obj.ModalidadePessoas.Cliente.Email != string.Empty
                            && obj.ModalidadePessoas.Cliente.Email != null
                         select obj;

            return result.Select(o => o.ModalidadePessoas.Cliente).ToList();
        }

        public List<Dominio.Entidades.ClienteIndex> BuscarClienteIndex(int tamanhoGrupo)
        {
            Dominio.Entidades.ClienteIndex indice = new Dominio.Entidades.ClienteIndex(tamanhoGrupo);
            var lstCodigos = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>().Select(o => o.CPF_CNPJ).ToList();
            var lstCodigosStr = lstCodigos.Select(o => o.ToString()).ToList();
            return indice.MontarListaIndex(lstCodigosStr);
        }

        public void HabilitarAcessoFornecedorParaTodosClientes()
        {
            string sql = @"
              UPDATE T_CLIENTE
              SET CLI_ATIVAR_ACESSO_FORNECEDOR = 1
              WHERE CLI_ATIVAR_ACESSO_FORNECEDOR = 0
              AND CLI_ATIVO = 1;";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.ExecuteUpdate();

            this.SessionNHiBernate.Flush();
        }


        public List<Dominio.Entidades.Cliente> BuscarPessoasSemCadastroUsuario(int batchSize)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from cliente in query
                         where cliente.AtivarAcessoFornecedor == true
                            && !this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                                .Any(usuario => usuario.ClienteFornecedor.CPF_CNPJ == cliente.CPF_CNPJ
                                                && usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Fornecedor)
                         orderby cliente.Nome
                         select cliente;

            return result.Take(batchSize).ToList();
        }


        #endregion

        #region Relatório de Pessoas

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pessoas.Pessoa> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaPessoa().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pessoas.Pessoa)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.Pessoa>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaPessoa().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.ObjetosDeValor.Cliente> BuscarNomesClientes(List<double> codigosClientes)
        {
            if (codigosClientes.Count > 0)
            {
                string sql = $@"
                    select
                        Cliente.CLI_CGCCPF Codigo,
					    Cliente.CLI_NOME Nome,
                        Cliente.CLI_NOMEFANTASIA NomeFantasia,
						Cliente.CLI_CODIGO_INTEGRACAO CopdigoIntegracao,
						Localidade.LOC_DESCRICAO Cidade,
						Localidade.UF_SIGLA UF
                    from
	                    T_CLIENTE Cliente
                    left join
						T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Cliente.LOC_CODIGO
                    where
	                    Cliente.CLI_CGCCPF in ({string.Join(",", codigosClientes)})
                    order by 
                        Cliente.CLI_CGCCPF";

                var query = this.SessionNHiBernate.CreateSQLQuery(sql);
                IList<dynamic> clientesDynamic = query.List<dynamic>();

                System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = "." };
                List<Dominio.ObjetosDeValor.Cliente> clientes = new List<Dominio.ObjetosDeValor.Cliente>();
                int total = clientesDynamic.Count;
                for (int i = 0; i < total; i++)
                {
                    clientes.Add(new Dominio.ObjetosDeValor.Cliente
                    {
                        Codigo = clientesDynamic[i][0],
                        Nome = clientesDynamic[i][1],
                        NomeFantasia = clientesDynamic[i][2],
                        CodigoIntegracao = clientesDynamic[i][3],
                        Cidade = clientesDynamic[i][4],
                        UF = clientesDynamic[i][5]
                    });
                }
                return clientes;
            }
            return null;
        }

        public List<Dominio.Entidades.Cliente> BuscarComAnexosPorCPFCNPJ(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CPF_CNPJ == cpfCnpj select obj;
            return result.Fetch(o => o.Anexos).ToList();
        }

        public Dominio.Entidades.Cliente BuscarPorCodigoIntegracaoENome(string codigoIntegracao, string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) && obj.Nome.Contains(nome) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Cliente BuscarPorCPFCNPJComStatus(double cpfCnpj, bool status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            var result = from obj in query where obj.CPF_CNPJ == cpfCnpj && obj.Ativo == status select obj;

            return result.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.GeoLocalizacaoClienteSubArea> BuscarClientesGeolocalizacaoAtivosPorFilais(List<double> codigosFiliais)
        {
            var codigosFiliaisStr = string.Join(", ", codigosFiliais);

            var sql = $@"SELECT
                 Cliente.CLI_CGCCPF ClienteCPFCNPJ,
                 Cliente.CLI_LATIDUDE Latitude,
                 Cliente.CLI_LONGITUDE Longitude,
                 Cliente.CLI_NOMEFANTASIA NomeFantasia,
                 Cliente.CLI_AREA AreaCliente,
                 Cliente.CLI_RAIO_METROS RaioCliente,
                 (SELECT SubAreaCliente.SAC_AREA as Area
                 FROM T_SUBAREA_CLIENTE SubAreaCliente 
                 WHERE SubAreaCliente.CLI_CGCCPF = Cliente.CLI_CGCCPF and SubAreaCliente.SAC_ATIVO = 1 
                 FOR JSON PATH) SubAreaCliente
                 FROM T_CLIENTE Cliente
                 where Cliente.CLI_ATIVO = 1 and Cliente.CLI_LATIDUDE IS NOT NULL AND Cliente.CLI_LONGITUDE IS NOT NULL AND Cliente.CLI_CGCCPF in ({codigosFiliaisStr}) ;";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.GeoLocalizacaoClienteSubArea)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.GeoLocalizacaoClienteSubArea>();
        }

        public Dominio.Entidades.Localidade BuscarLocalidadePorCPFCNPJ(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(obj => obj.CPF_CNPJ == cpfCnpj);

            return query
                .Select(o => o.Localidade)
                .FirstOrDefault();
        }

        #endregion
    }

}
