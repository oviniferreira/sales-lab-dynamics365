using Microsoft.Xrm.Sdk;
using System;

namespace SalesLab.Plugins
{
    public class Pedido_PostOp_Confirmacao : IPlugin
    {
        private const string PedidoLogicalName = "dev_pedido";

        // Confirmação por OptionSet
        private const string CampoStatusPedido = "dev_statusdopedido"; 
        private const int StatusPedidoConfirmado = 775730001;

        // Campos protegidos após confirmação
        private const string CampoQuantidade = "dev_quantidade";
        private const string CampoProdutoLookup = "dev_produto";
        private const string CampoTotal = "dev_valortotal";

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                // Update + PreOperation
                if (!string.Equals(context.MessageName, "Update", StringComparison.OrdinalIgnoreCase))
                    return;

                if (context.Stage != 20) // PreOperation
                    return;

                if (!context.InputParameters.Contains("Target"))
                    return;

                Entity target = context.InputParameters["Target"] as Entity;
                if (target == null)
                    return;

                if (!string.Equals(target.LogicalName, PedidoLogicalName, StringComparison.OrdinalIgnoreCase))
                    return;

                // PreImage obrigatória (para saber estado anterior e campos que não vierem no Target)
                Entity preImage = null;
                if (context.PreEntityImages != null && context.PreEntityImages.Contains("PreImage"))
                    preImage = context.PreEntityImages["PreImage"];

                if (preImage == null)
                    throw new InvalidPluginExecutionException("PreImage não configurada. Configure a image com alias 'PreImage'.");

                bool confirmadoAntes = IsConfirmado(preImage);
                bool confirmadoDepois = IsConfirmadoDepois(target, preImage);

                bool estaConfirmandoAgora = (confirmadoAntes == false && confirmadoDepois == true);

                // 1) Se já estava confirmado, bloquear alterações em Produto/Quantidade/Total
                if (confirmadoAntes)
                {
                    bool mudouQuantidade = target.Attributes.Contains(CampoQuantidade);
                    bool mudouProduto = target.Attributes.Contains(CampoProdutoLookup);
                    bool mudouTotal = target.Attributes.Contains(CampoTotal);

                    if (mudouQuantidade || mudouProduto || mudouTotal)
                        throw new InvalidPluginExecutionException("Pedido confirmado não permite alteração de Produto/Quantidade/Total.");
                }

                // 2) Se está tentando confirmar agora, validar se está completo
                if (estaConfirmandoAgora)
                {
                    EntityReference produto = GetLookup(target, preImage, CampoProdutoLookup);
                    if (produto == null)
                        throw new InvalidPluginExecutionException("Não é possível confirmar: Produto é obrigatório.");

                    int? qtd = GetInt(target, preImage, CampoQuantidade);
                    if (!qtd.HasValue || qtd.Value <= 0)
                        throw new InvalidPluginExecutionException("Não é possível confirmar: Quantidade deve ser maior que zero.");

                    Money total = GetMoney(target, preImage, CampoTotal);
                    if (total == null || total.Value <= 0m)
                        throw new InvalidPluginExecutionException("Não é possível confirmar: Total do pedido inválido.");
                }
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                tracing.Trace("Erro no plugin Pedido_ConfirmarEBloquear: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("Erro ao processar confirmação do pedido.");
            }
        }

        // ===== Helpers =====

        private bool IsConfirmado(Entity e)
        {
            if (e == null) return false;

            if (e.Attributes.Contains(CampoStatusPedido))
            {
                OptionSetValue os = e.GetAttributeValue<OptionSetValue>(CampoStatusPedido);
                return (os != null && os.Value == StatusPedidoConfirmado);
            }

            return false;
        }

        private bool IsConfirmadoDepois(Entity target, Entity preImage)
        {
            // Se o Update trouxe o status, usa ele
            if (target != null && target.Attributes.Contains(CampoStatusPedido))
            {
                OptionSetValue os = target.GetAttributeValue<OptionSetValue>(CampoStatusPedido);
                return (os != null && os.Value == StatusPedidoConfirmado);
            }

            // Se não trouxe, mantém como estava
            return IsConfirmado(preImage);
        }

        private EntityReference GetLookup(Entity target, Entity preImage, string attr)
        {
            if (target != null && target.Attributes.Contains(attr))
                return target.GetAttributeValue<EntityReference>(attr);

            if (preImage != null && preImage.Attributes.Contains(attr))
                return preImage.GetAttributeValue<EntityReference>(attr);

            return null;
        }

        private int? GetInt(Entity target, Entity preImage, string attr)
        {
            if (target != null && target.Attributes.Contains(attr))
                return target.GetAttributeValue<int>(attr);

            if (preImage != null && preImage.Attributes.Contains(attr))
                return preImage.GetAttributeValue<int>(attr);

            return null;
        }

        private Money GetMoney(Entity target, Entity preImage, string attr)
        {
            if (target != null && target.Attributes.Contains(attr))
                return target.GetAttributeValue<Money>(attr);

            if (preImage != null && preImage.Attributes.Contains(attr))
                return preImage.GetAttributeValue<Money>(attr);

            return null;
        }
    }
}
