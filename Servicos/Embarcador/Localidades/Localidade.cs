using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Localidades
{
    public class Localidade : ServicoBase
    {        
        public Localidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Localidade() : base() { }

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Localidade ConverterObjetoLocalidade(Dominio.Entidades.Localidade localidade, Dominio.Entidades.Pais pais = null)
        {
            if (localidade != null)
            {
                Dominio.ObjetosDeValor.Localidade cidade = new Dominio.ObjetosDeValor.Localidade
                {
                    Codigo = localidade.Codigo,
                    CodigoIntegracao = localidade.CodigoLocalidadeEmbarcador,
                    Descricao = localidade.Descricao,
                    IBGE = localidade.CodigoIBGE,
                    SiglaUF = localidade.Estado.Sigla,
                    CodigoDocumento = localidade.CodigoDocumento
                };

                if (localidade.Pais != null)
                {
                    cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais();
                    cidade.Pais.CodigoPais = localidade.Pais.Codigo;
                    cidade.Pais.NomePais = localidade.Pais.Nome;
                    cidade.Pais.SiglaPais = localidade.Pais.Abreviacao;
                }
                else if (pais != null)
                {
                    cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais();
                    cidade.Pais.CodigoPais = pais.Codigo;
                    cidade.Pais.NomePais = pais.Nome;
                    cidade.Pais.SiglaPais = pais.Abreviacao;
                }

                if (localidade.Regiao != null)
                {
                    cidade.Regiao = new Dominio.ObjetosDeValor.Embarcador.Localidade.Regiao();
                    cidade.Regiao.CodigoIntegracao = localidade.Regiao.CodigoIntegracao;
                    cidade.Regiao.Descricao = localidade.Regiao.Descricao;
                    if (localidade.Regiao.LocalidadePolo != null)
                    {
                        cidade.Regiao.CodigoIntegracaoLocalidadePolo = localidade.Regiao.LocalidadePolo.CodigoLocalidadeEmbarcador;
                        cidade.Regiao.IBGELocalidadePolo = localidade.Regiao.LocalidadePolo.CodigoIBGE;
                    }
                }

                return cidade;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Localidade ConverterObjetoLocalidadeExterior(dynamic participanteCTe)
        {
            if (participanteCTe != null)
            {
                Dominio.ObjetosDeValor.Localidade cidade = new Dominio.ObjetosDeValor.Localidade
                {
                    Descricao = participanteCTe.LocalidadeExterior,
                    IBGE = 9999999,
                    SiglaUF = "EX",
                    Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais
                    {
                        CodigoPais = participanteCTe.Pais
                    }
                };

                return cidade;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.Fronteira ConverterObjetoFronteira(Dominio.Entidades.Cliente fronteira)
        {
            if (fronteira != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.Fronteira dynFronteira = new Dominio.ObjetosDeValor.Embarcador.Logistica.Fronteira();
                dynFronteira.CodigoIntegracao = fronteira.CodigoIntegracao;
                dynFronteira.Descricao = fronteira.Descricao;
                dynFronteira.Localidade = ConverterObjetoLocalidade(fronteira.Localidade);
                dynFronteira.CPF_CNPJ = fronteira.Codigo;

                return dynFronteira;
            }
            else
            {
                return null;
            }
        }

        public dynamic BuscarEnderecoPorCEP(string cep, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Localidades.Endereco endereco = repEndereco.BuscarCEP(cep);

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Dominio.Entidades.Localidade localidade = null;
            if (endereco != null && endereco.Localidade != null && !string.IsNullOrWhiteSpace(endereco.Localidade.CodigoIBGE))
                localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(Utilidades.String.OnlyNumbers(endereco.Localidade.CodigoIBGE)));

            if (endereco != null)
            {
                Servicos.Embarcador.Integracao.MapRequest.Geocoding serGeocoding = new Servicos.Embarcador.Integracao.MapRequest.Geocoding();
                //Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas = serMapRequestAPI.BuscarCoordenadasEndereco(endereco.Localidade.CodigoIBGE, endereco.Localidade.Estado.UF, endereco.Logradouro, endereco.CEP, endereco.Bairro.Descricao);
                Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas(); //serGeocoding.BuscarCoordenadasEndereco(localidade, endereco.Logradouro, "");
                var dynEndereco = new
                {
                    Bairro = endereco.Bairro == null ? string.Empty : endereco.Bairro.Descricao,
                    endereco.CEP,
                    Complemento = endereco.Complemento == null ? string.Empty : endereco.Complemento,
                    Local = endereco.Local == null ? string.Empty : endereco.Local,
                    endereco.Logradouro,
                    TipoLogradouro = endereco.TipoLogradouro == null ? string.Empty : endereco.TipoLogradouro,
                    DescricaoCidadeEstado = localidade != null ? localidade.DescricaoCidadeEstado : string.Empty,
                    CodigoCidade = localidade != null ? localidade.Codigo : 0,
                    Latitude = coordenadas != null ? coordenadas.latitude : string.Empty,
                    Longitude = coordenadas != null ? coordenadas.longitude : string.Empty,
                    TipoLocalizacao = coordenadas.tipoLocalizacao,
                    DescricaoCidade = localidade != null ? localidade.Descricao : string.Empty,
                    EnumTipoLogradouro = RetornarEnumTipoLogradouro(endereco.TipoLogradouro == null ? string.Empty : endereco.TipoLogradouro)
                };
                return dynEndereco;
            }
            else
                return null;
        }

        public static void VerificarCargasEmitidasAnteriormente(int codigoLocalidade, bool alterarTodasCargas, DbConnection connection, DbTransaction transaction)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandTimeout = 300;
            command.Transaction = transaction;

            command.CommandText = @"UPDATE CC
                        SET CC.CAR_CARGA_INTEGRADA_EMBARCADOR = 0
                        FROM T_CTE_PARTICIPANTE P
                        JOIN T_CTE C ON C.CON_REMETENTE_CTE = P.PCT_CODIGO
                        JOIN T_CARGA_CTE CA ON CA.CON_CODIGO = C.CON_CODIGO
                        JOIN T_CARGA CC ON CC.CAR_CODIGO = CA.CAR_CODIGO
                        WHERE P.LOC_CODIGO = " + Utilidades.String.OnlyNumbers(codigoLocalidade.ToString("n0"));
            if (!alterarTodasCargas)
                command.CommandText += " AND C.CON_DATAHORAEMISSAO >= (GETDATE() - 30)";

            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE CC
                        SET CC.CAR_CARGA_INTEGRADA_EMBARCADOR = 0
                        FROM T_CTE_PARTICIPANTE P
                        JOIN T_CTE C ON C.CON_DESTINATARIO_CTE = P.PCT_CODIGO
                        JOIN T_CARGA_CTE CA ON CA.CON_CODIGO = C.CON_CODIGO
                        JOIN T_CARGA CC ON CC.CAR_CODIGO = CA.CAR_CODIGO
                        WHERE P.LOC_CODIGO = " + Utilidades.String.OnlyNumbers(codigoLocalidade.ToString("n0"));
            if (!alterarTodasCargas)
                command.CommandText += " AND C.CON_DATAHORAEMISSAO >= (GETDATE() - 30)";

            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE CC
                        SET CC.CAR_CARGA_INTEGRADA_EMBARCADOR = 0
                        FROM T_CTE_PARTICIPANTE P
                        JOIN T_CTE C ON C.CON_EXPEDIDOR_CTE = P.PCT_CODIGO
                        JOIN T_CARGA_CTE CA ON CA.CON_CODIGO = C.CON_CODIGO
                        JOIN T_CARGA CC ON CC.CAR_CODIGO = CA.CAR_CODIGO
                        WHERE P.LOC_CODIGO = " + Utilidades.String.OnlyNumbers(codigoLocalidade.ToString("n0"));
            if (!alterarTodasCargas)
                command.CommandText += " AND C.CON_DATAHORAEMISSAO >= (GETDATE() - 30)";

            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE CC
                        SET CC.CAR_CARGA_INTEGRADA_EMBARCADOR = 0
                        FROM T_CTE_PARTICIPANTE P
                        JOIN T_CTE C ON C.CON_RECEBEDOR_CTE = P.PCT_CODIGO 
                        JOIN T_CARGA_CTE CA ON CA.CON_CODIGO = C.CON_CODIGO
                        JOIN T_CARGA CC ON CC.CAR_CODIGO = CA.CAR_CODIGO
                        WHERE P.LOC_CODIGO = " + Utilidades.String.OnlyNumbers(codigoLocalidade.ToString("n0"));
            if (!alterarTodasCargas)
                command.CommandText += " AND C.CON_DATAHORAEMISSAO >= (GETDATE() - 30)";

            command.ExecuteNonQuery();

            command.CommandText = @"UPDATE CC
                        SET CC.CAR_CARGA_INTEGRADA_EMBARCADOR = 0
                        FROM T_CTE_PARTICIPANTE P
                        JOIN T_CTE C ON C.CON_TOMADOR_CTE = P.PCT_CODIGO
                        JOIN T_CARGA_CTE CA ON CA.CON_CODIGO = C.CON_CODIGO
                        JOIN T_CARGA CC ON CC.CAR_CODIGO = CA.CAR_CODIGO
                        WHERE P.LOC_CODIGO = " + Utilidades.String.OnlyNumbers(codigoLocalidade.ToString("n0"));
            if (!alterarTodasCargas)
                command.CommandText += " AND C.CON_DATAHORAEMISSAO >= (GETDATE() - 30)";

            command.CommandText = @"UPDATE CC
                        SET CC.CAR_CARGA_INTEGRADA_EMBARCADOR = 0
                        FROM T_CTE C
                        JOIN T_CARGA_CTE CA ON CA.CON_CODIGO = C.CON_CODIGO
                        JOIN T_CARGA CC ON CC.CAR_CODIGO = CA.CAR_CODIGO
                        WHERE C.CON_LOCINICIOPRESTACAO = " + Utilidades.String.OnlyNumbers(codigoLocalidade.ToString("n0"));
            if (!alterarTodasCargas)
                command.CommandText += " AND C.CON_DATAHORAEMISSAO >= (GETDATE() - 30)";

            command.CommandText = @"UPDATE CC
                        SET CC.CAR_CARGA_INTEGRADA_EMBARCADOR = 0
                        FROM T_CTE C
                        JOIN T_CARGA_CTE CA ON CA.CON_CODIGO = C.CON_CODIGO
                        JOIN T_CARGA CC ON CC.CAR_CODIGO = CA.CAR_CODIGO
                        WHERE C.CON_LOCTERMINOPRESTACAO = " + Utilidades.String.OnlyNumbers(codigoLocalidade.ToString("n0"));
            if (!alterarTodasCargas)
                command.CommandText += " AND C.CON_DATAHORAEMISSAO >= (GETDATE() - 30)";

            command.ExecuteNonQuery();
        }

        public void ObterLocalidadesValePedagioCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, out Dominio.Entidades.Localidade localidadeOrigem, out Dominio.Entidades.Localidade localidadeDestino, out List<Dominio.Entidades.Localidade> pontosPassagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            localidadeOrigem = null;
            localidadeDestino = null;
            pontosPassagem = new List<Dominio.Entidades.Localidade>();

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota = null;

            if (cargaRotaFrete != null)
                pontosRota = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            if (pontosRota == null || pontosRota.Count == 0)
                throw new ServicoException("Não foi possível definir os pontos da rota.");

            localidadeOrigem = ObterLocalidade(pontosRota.FirstOrDefault());
            localidadeDestino = ObterLocalidade(pontosRota.LastOrDefault());

            if (localidadeOrigem == null)
                throw new ServicoException("Não foi possível definir a localidade origem da rota.");

            if (localidadeDestino == null)
                throw new ServicoException("Não foi possível definir a localidade destino da rota.");

            for (int i = 0; i < pontosRota.Count; i++)
            {
                if (i > 0 && i < pontosRota.Count - 1) // Não envia o primeiro e o ultimo ponto                
                {
                    Dominio.Entidades.Localidade localidade = ObterLocalidade(pontosRota[i]);
                    Dominio.Entidades.Localidade localidadeAnterior = ObterLocalidade(pontosRota[i - 1]);
                    if (localidade != null)
                    {
                        if ((localidadeAnterior == null || localidade.Codigo != localidadeAnterior.Codigo) && localidade.Codigo != localidadeOrigem.Codigo && localidade.Codigo != localidadeDestino.Codigo)
                        {
                            pontosPassagem.Add(localidade);
                        }
                    }
                }
            }
        }

        public Dominio.Entidades.Localidade ObterLocalidade(Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaFretePontosPassagem)
        {
            if (cargaRotaFretePontosPassagem == null)
                return null;

            if (cargaRotaFretePontosPassagem.ClienteOutroEndereco != null)
                return cargaRotaFretePontosPassagem.ClienteOutroEndereco.Localidade;

            if (cargaRotaFretePontosPassagem.Cliente != null)
                return cargaRotaFretePontosPassagem.Cliente.Localidade;

            if (cargaRotaFretePontosPassagem.Localidade != null)
                return cargaRotaFretePontosPassagem.Localidade;

            return null;
        }

        public void ObterLatitudeLongitudeValePedagioCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, out (decimal? Latitude, decimal? Longitude) latitudeLongitudeOrigem, out (decimal? Latitude, decimal? Longitude) latitudeLongitudeDestino, out List<(decimal? Latitude, decimal? Longitude)> pontosPassagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repositorioCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            latitudeLongitudeOrigem = (null, null);
            latitudeLongitudeDestino = (null, null);
            pontosPassagem = new List<(decimal? Latitude, decimal? Longitude)>();

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repositorioCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota = null;

            if (cargaRotaFrete != null)
                pontosRota = repositorioCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            if (pontosRota == null || pontosRota.Count == 0)
                throw new ServicoException("Não foi possível definir os pontos da rota.");

            latitudeLongitudeOrigem = ObterLatitudeLongitude(pontosRota.FirstOrDefault());
            latitudeLongitudeDestino = ObterLatitudeLongitude(pontosRota.LastOrDefault());

            if (latitudeLongitudeOrigem.Latitude == null || latitudeLongitudeOrigem.Longitude == null)
                throw new ServicoException("Não foi possível definir a latitude ou longitude da origem da rota.");

            if (latitudeLongitudeDestino.Latitude == null || latitudeLongitudeDestino.Longitude == null)
                throw new ServicoException("Não foi possível definir a latitude ou longitude do destino da rota.");

            for (int i = 0; i < pontosRota.Count; i++)
            {
                if (i > 0 && i < pontosRota.Count - 1) // Não envia o primeiro e o último ponto                
                {
                    (decimal? Latitude, decimal? Longitude) ponto = ObterLatitudeLongitude(pontosRota[i]);
                    (decimal? Latitude, decimal? Longitude) pontoAnterior = ObterLatitudeLongitude(pontosRota[i - 1]);

                    if (ponto.Latitude != null && ponto.Longitude != null)
                    {
                        bool diferentePontoAnterior = pontoAnterior.Latitude == null || ponto.Latitude != pontoAnterior.Latitude;
                        bool diferentePontoOrigem = ponto.Latitude != latitudeLongitudeOrigem.Latitude;
                        bool diferentePontoDestino = ponto.Latitude != latitudeLongitudeDestino.Latitude;

                        if (diferentePontoAnterior && diferentePontoOrigem && diferentePontoDestino)
                            pontosPassagem.Add(ponto);
                    }
                }
            }
        }

        public (decimal? Latitude, decimal? Longitude) ObterLatitudeLongitude(Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaFretePontosPassagem)
        {
            if (cargaRotaFretePontosPassagem == null)
                return (null, null);

            if (cargaRotaFretePontosPassagem.ClienteOutroEndereco != null)
                return ValueTuple.Create(cargaRotaFretePontosPassagem.ClienteOutroEndereco.Latitude.ToNullableDecimal(), cargaRotaFretePontosPassagem.ClienteOutroEndereco.Longitude.ToNullableDecimal());

            if (cargaRotaFretePontosPassagem.Cliente != null)
                return ValueTuple.Create(cargaRotaFretePontosPassagem.Cliente.Latitude.ToNullableDecimal(), cargaRotaFretePontosPassagem.Cliente.Longitude.ToNullableDecimal());

            if (cargaRotaFretePontosPassagem.Localidade != null)
                return ValueTuple.Create(cargaRotaFretePontosPassagem.Localidade.Latitude, cargaRotaFretePontosPassagem.Localidade.Longitude);

            return (null, null);
        }

        #endregion

        #region Métodos Privados

        private TipoLogradouro RetornarEnumTipoLogradouro(string tipo)
        {
            TipoLogradouro enumTipo = TipoLogradouro.Rua;

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                if (tipo == "Rua")
                    enumTipo = TipoLogradouro.Rua;
                else if (tipo == "Avenida")
                    enumTipo = TipoLogradouro.Avenida;
                else if (tipo == "Rodovia")
                    enumTipo = TipoLogradouro.Rodovia;
                else if (tipo == "Estrada")
                    enumTipo = TipoLogradouro.Estrada;
                else if (tipo == "Praca" || tipo == "Praça")
                    enumTipo = TipoLogradouro.Praca;
                else if (tipo == "Travessa")
                    enumTipo = TipoLogradouro.Travessa;
                else
                    enumTipo = TipoLogradouro.Outros;
            }

            return enumTipo;
        }

        #endregion
    }
}
