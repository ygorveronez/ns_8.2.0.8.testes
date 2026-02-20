using System;
using System.Linq;

namespace Servicos.Embarcador.Pessoa
{
    public class GrupoPessoasFaturaCanhoto
    {
        #region Atributos

        Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores
        public GrupoPessoasFaturaCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public string ObtemCertificadoChavePrivada(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto grupoFaturaCanhoto)
        {
            byte[] arquivo = new byte[0];

            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> repAnexoGrupoFaturaCanhoto = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>(_unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto anexoCanhoto = repAnexoGrupoFaturaCanhoto.BuscarPorEntidade(grupoFaturaCanhoto.Codigo).FirstOrDefault();

            if (anexoCanhoto != null)
            {
                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> servicoAnexoLayoutEDI = new Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>(_unitOfWork);
                arquivo = servicoAnexoLayoutEDI.DownloadAnexo(anexoCanhoto, _unitOfWork);
            }
            return System.Text.Encoding.UTF8.GetString(arquivo);
        }

        public string ObtemCertificadoChavePrivadaBase64(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto grupoFaturaCanhoto)
        {
            string arquivo = string.Empty;

            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> repAnexoGrupoFaturaCanhoto = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>(_unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto anexoCanhoto = repAnexoGrupoFaturaCanhoto.BuscarPorEntidade(grupoFaturaCanhoto.Codigo).FirstOrDefault();

            if (anexoCanhoto != null)
            {
                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> servicoAnexoLayoutEDI = new Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>(_unitOfWork);
                arquivo = Convert.ToBase64String(servicoAnexoLayoutEDI.DownloadAnexo(anexoCanhoto, _unitOfWork));
            }
            return arquivo;
        }

        #endregion

        #region Métodos Privados
        #endregion
    }
}
