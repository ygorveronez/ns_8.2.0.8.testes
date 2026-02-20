using Dominio.ObjetosDeValor.Embarcador.Chamado;
using Infrastructure.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;

namespace SGT.WebAdmin.Controllers
{

    public class CacheModulo
    {
        public CacheModulo(int codigoModulo, CacheModulo moduloPai)
        {
            this.CodigoModulo = codigoModulo;
            this.ModuloPai = moduloPai;
        }
        public CacheModulo ModuloPai { get; set; }
        public int CodigoModulo { get; set; }
    }

    public class CacheFormulario
    {
        public CacheFormulario(string caminhoFormulario, int codigoFormulario, CacheModulo cacheModulo, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas)
        {
            this.CaminhoFormulario = caminhoFormulario;
            this.CodigoFormulario = codigoFormulario;
            this.CacheModulo = cacheModulo;
            this.PermissoesPersonalizadas = permissoesPersonalizadas;
        }
        public string CaminhoFormulario { get; set; }
        public int CodigoFormulario { get; set; }
        public CacheModulo CacheModulo { get; set; }
        public List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> PermissoesPersonalizadas { get; set; }
    }


    public class Modulos : BaseController
    {
        #region Construtores

        public Modulos(Conexao conexao) : base(conexao) { }

        public Modulos() : base(null) { }

        #endregion


        public bool VerificarModulosPaiLiberadoRecursivamente(CacheModulo cacheModulo, List<int> modulosLiberados)
        {
            if (cacheModulo.ModuloPai != null)
            {
                if (modulosLiberados.Contains(cacheModulo.ModuloPai.CodigoModulo))
                    return true;
                else
                    return VerificarModulosPaiLiberadoRecursivamente(cacheModulo.ModuloPai, modulosLiberados);
            }
            else
            {
                return false;
            }
        }

        public List<CacheFormulario> RetornarFormulariosEmCache(bool reset = false)
        {
            List<CacheFormulario> cacheFormulario = null;
            string keyCache = "_FORMULARIOS_" + Cliente.Codigo;
            List<CacheFormulario> objCacheFormulario = CacheProvider.Instance.Get<List<CacheFormulario>>(keyCache);

            if (reset || objCacheFormulario == null || objCacheFormulario.Count == 0)
            {
                var modulos = RetornarListaModulosFormularios();
                cacheFormulario = CriarCacheFormularios(modulos);
            }
            else
            {
                cacheFormulario = objCacheFormulario;
            }
            return cacheFormulario;
        }

        public List<CacheFormulario> CriarCacheFormularios(dynamic modulos)
        {
            List<CacheFormulario> cacheFormulario = new List<CacheFormulario>();
            armazenarRecursivamenteFormulariosEmChache(modulos, ref cacheFormulario, null);
            string keyCache = "_FORMULARIOS_" + Cliente.Codigo;
            CacheProvider.Instance.Add(keyCache, cacheFormulario, TimeSpan.FromHours(24));
            return cacheFormulario;
        }

        private void armazenarRecursivamenteFormulariosEmChache(dynamic modulos, ref List<CacheFormulario> cacheFormulario, CacheModulo moduloPai)
        {
            foreach (var modulo in modulos)
            {
                CacheModulo cacheModulo = new CacheModulo((int)modulo.CodigoModulo, moduloPai);
                foreach (var formulario in modulo.Formularios)
                {
                    List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = new List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada>();
                    foreach (var dynPermissa in formulario.PermissoesPersonalizadas)
                        permissoesPersonalizadas.Add((AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada)(int)dynPermissa.CodigoPermissao);

                    cacheFormulario.Add(new CacheFormulario(formulario.CaminhoPagina, (int)formulario.CodigoFormulario, cacheModulo, permissoesPersonalizadas));
                }
                armazenarRecursivamenteFormulariosEmChache(modulo.ModulosFilho, ref cacheFormulario, cacheModulo);
            }
        }

        public dynamic RetornarListaModulosFormulariosPorTipo(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = Cliente;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWork);
            AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada repPermissaoPersonalizada = new AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada(unitOfWork);
            AdminMultisoftware.Repositorio.Modulos.ClienteFormulario repClienteFormulario = new AdminMultisoftware.Repositorio.Modulos.ClienteFormulario(unitOfWork);
            AdminMultisoftware.Repositorio.Modulos.ClienteModulo repClienteModulo = new AdminMultisoftware.Repositorio.Modulos.ClienteModulo(unitOfWork);
            AdminMultisoftware.Repositorio.Modulos.Modulo repModulo = new AdminMultisoftware.Repositorio.Modulos.Modulo(unitOfWork);

            List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios = repFormulario.BuscarPorClieteETipoServico(cliente, tipoServicoMultisoftware, ClienteAcesso.URLHomologacao);
            List<AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada> permissoesPersonalizadas = repPermissaoPersonalizada.BuscarPorClieteETipoServico(cliente, tipoServicoMultisoftware, ClienteAcesso.URLHomologacao);
            List<AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario> clienteFormularioPersonalizado = repClienteFormulario.BuscarTodosComDescricaoDiferenciadaPorCliente(cliente.Codigo);
            List<AdminMultisoftware.Dominio.Entidades.Modulos.Modulo> modulos = repModulo.BuscarPorClieteETipoServico(cliente, tipoServicoMultisoftware, ClienteAcesso.URLHomologacao);

            List<AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo> clienteModuloPersonalizado = repClienteModulo.BuscarTodosComDescricaoDiferenciadaPorCliente(cliente.Codigo);

            var retorno = GerarHierarquiaModulosFormularios(null, modulos, formularios, permissoesPersonalizadas, clienteFormularioPersonalizado, clienteModuloPersonalizado);
            unitOfWork.Dispose();
            return retorno;
        }

        public dynamic RetornarListaModulosFormularios()
        {
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = Cliente;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWork);
            AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada repPermissaoPersonalizada = new AdminMultisoftware.Repositorio.Modulos.PermissaoPersonalizada(unitOfWork);
            AdminMultisoftware.Repositorio.Modulos.ClienteFormulario repClienteFormulario = new AdminMultisoftware.Repositorio.Modulos.ClienteFormulario(unitOfWork);
            AdminMultisoftware.Repositorio.Modulos.ClienteModulo repClienteModulo = new AdminMultisoftware.Repositorio.Modulos.ClienteModulo(unitOfWork);
            AdminMultisoftware.Repositorio.Modulos.Modulo repModulo = new AdminMultisoftware.Repositorio.Modulos.Modulo(unitOfWork);

            List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios = repFormulario.BuscarPorClieteETipoServico(cliente, TipoServicoMultisoftware, ClienteAcesso.URLHomologacao);
            List<AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada> permissoesPersonalizadas = repPermissaoPersonalizada.BuscarPorClieteETipoServico(cliente, TipoServicoMultisoftware, ClienteAcesso.URLHomologacao);
            List<AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario> clienteFormularioPersonalizado = repClienteFormulario.BuscarTodosComDescricaoDiferenciadaPorCliente(cliente.Codigo);
            List<AdminMultisoftware.Dominio.Entidades.Modulos.Modulo> modulos = repModulo.BuscarPorClieteETipoServico(cliente, TipoServicoMultisoftware, ClienteAcesso.URLHomologacao);

            List<AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo> clienteModuloPersonalizado = repClienteModulo.BuscarTodosComDescricaoDiferenciadaPorCliente(cliente.Codigo);

            var retorno = GerarHierarquiaModulosFormularios(null, modulos, formularios, permissoesPersonalizadas, clienteFormularioPersonalizado, clienteModuloPersonalizado);
            unitOfWork.Dispose();
            return retorno;
        }

        public dynamic GerarHierarquiaModulosFormularios(AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo, List<AdminMultisoftware.Dominio.Entidades.Modulos.Modulo> modulos, List<AdminMultisoftware.Dominio.Entidades.Modulos.Formulario> formularios, List<AdminMultisoftware.Dominio.Entidades.Modulos.PermissaoPersonalizada> permissoesPersonalizadas, List<AdminMultisoftware.Dominio.Entidades.Modulos.ClienteFormulario> clienteFormularioPersonalizado, List<AdminMultisoftware.Dominio.Entidades.Modulos.ClienteModulo> clienteModuloPersonalizado)
        {
            Localization.Service.Common svcLocalization = new Localization.Service.Common();
            bool traduzir = System.Globalization.CultureInfo.CurrentCulture.Name != "pt-BR";
            List<AdminMultisoftware.Dominio.Entidades.Modulos.Modulo> modulosFilhos = (from obj in modulos where obj.ModuloPai == modulo select obj).OrderBy(obj => obj.Sequencia).ToList();

            int codigoCliente = Cliente.Codigo;
            var retorno = (from obj in modulosFilhos
                           select new
                           {
                               obj.CodigoModulo,
                               Descricao = (from cliMod in clienteModuloPersonalizado where !traduzir && cliMod.Modulo.Codigo == obj.Codigo select cliMod.Descricao).FirstOrDefault() ?? svcLocalization.GetTranslationByResourcePath(obj.TranslationResourcePath, obj.Descricao, traduzir),
                               obj.TranslationResourcePath,
                               Icone = obj.IconeNovo,
                               obj.Sequencia,
                               Formularios = (from form in formularios
                                              where form.Modulo.Codigo == obj.Codigo
                                              orderby form.Sequencia
                                              select new
                                              {
                                                  form.CaminhoPagina,
                                                  form.CodigoFormulario,
                                                  Descricao = (from cliFor in clienteFormularioPersonalizado where !traduzir && cliFor.Formulario.Codigo == form.Codigo select cliFor.Descricao).FirstOrDefault() ?? svcLocalization.GetTranslationByResourcePath(form.TranslationResourcePath, form.Descricao, traduzir),
                                                  form.TranslationResourcePath,
                                                  PermissoesPersonalizadas = (from opc in permissoesPersonalizadas
                                                                              where opc.Formulario.Codigo == form.Codigo
                                                                              select new
                                                                              {
                                                                                  opc.CodigoPermissao,
                                                                                  Descricao = !traduzir ? opc.Descricao : svcLocalization.GetTranslationByResourcePath(opc.TranslationResourcePath, opc.Descricao, traduzir)
                                                                              }).ToList(),
                                              }).ToList(),
                               ModulosFilho = GerarHierarquiaModulosFormularios(obj, modulos, formularios, permissoesPersonalizadas, clienteFormularioPersonalizado, clienteModuloPersonalizado)
                           }).ToList();
            return retorno;
        }
    }
}

