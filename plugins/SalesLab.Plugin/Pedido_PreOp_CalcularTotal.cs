using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace SalesLab.Plugins
{
    public class Pedido_PreOp_CalcularTotal : IPlugin
    {
        private const string CampoQuantidade = "dev_quantidade";
        private const string CampoProdutoLookup = "dev_produto";
        private const string CampoTotal = "dev_valortotal";

        private const string ProdutoCampoPreco = "dev_preco";

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);

            try
            {
                // Garantir que é Pre-Operation (Stage 20)
                if (context.Stage != 20) return;

                if (!context.InputParameters.Contains("Target"))
                    return;

                Entity target = context.InputParameters["Target"] as Entity;
                if (target == null)
                    return;

                if (target.LogicalName != "dev_pedido") return;

                var msg = context.MessageName;
                if (msg != "Create" && msg != "Update") return;

                // No Update: se não mexeu em Quantidade nem Produto, não recalcula e não valida
                if (msg == "Update" && !target.Contains(CampoQuantidade) && !target.Contains(CampoProdutoLookup))
                    return;

                Entity preImage = null;
                if (context.PreEntityImages.Contains("PreImage"))
                    preImage = context.PreEntityImages["PreImage"];

                // ====== Quantidade ======
                int? quantidade = null;

                if (target.Contains(CampoQuantidade))
                    quantidade = target.GetAttributeValue<int>(CampoQuantidade);
                else if (preImage != null && preImage.Contains(CampoQuantidade))
                    quantidade = preImage.GetAttributeValue<int>(CampoQuantidade);

                if (!quantidade.HasValue)
                    throw new InvalidPluginExecutionException("Quantidade não encontrada. Configure a PreImage com dev_quantidade.");

                if (quantidade.Value <= 0)
                    throw new InvalidPluginExecutionException("Quantidade deve ser maior que zero.");

                // ====== Produto ======
                EntityReference produtoRef = null;

                if (target.Contains(CampoProdutoLookup))
                    produtoRef = target.GetAttributeValue<EntityReference>(CampoProdutoLookup);
                else if (preImage != null && preImage.Contains(CampoProdutoLookup))
                    produtoRef = preImage.GetAttributeValue<EntityReference>(CampoProdutoLookup);

                if (produtoRef == null)
                    throw new InvalidPluginExecutionException("Produto é obrigatório para calcular o total.");

                // Busca preço do Produto
                var produto = service.Retrieve(
                    produtoRef.LogicalName,
                    produtoRef.Id,
                    new ColumnSet(ProdutoCampoPreco)
                );

                var precoMoney = produto.GetAttributeValue<Money>(ProdutoCampoPreco);
                var preco = precoMoney?.Value ?? 0m;

                var total = quantidade.Value * preco;

                target[CampoTotal] = new Money(total);
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                tracing.Trace("Erro no plugin Pedido_PreOp_CalcularTotal: {0}", ex);
                throw new InvalidPluginExecutionException("Erro ao calcular total do pedido.");
            }
        }
    }
}
