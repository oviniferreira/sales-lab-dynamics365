using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace SalesLab.Plugins
{
    public class Pedido_PreOp_BaixarEstoqueAoConfirmar : IPlugin
    {

        private const string PedidoLogicalName = "dev_pedido";

        private const string CampoStatusPedido = "dev_statusdopedido";
        private const int StatusConfirmado = 775730001;

        private const string CampoQuantidade = "dev_quantidade";
        private const string CampoProdutoLookup = "dev_produto";

        private const string ProdutoLogicalName = "dev_produto";
        private const string ProdutoCampoEstoque = "dev_estoque";


        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            ITracingService tracing =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory factory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service =
                factory.CreateOrganizationService(context.UserId);

            try
            {
                // Create && Update
                bool isCreate = string.Equals(context.MessageName, "Create", StringComparison.OrdinalIgnoreCase);
                bool isUpdate = string.Equals(context.MessageName, "Update", StringComparison.OrdinalIgnoreCase);

                if (!isCreate && !isUpdate)
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

                // PreImage
                Entity preImage = null;
                if (isUpdate)
                {
                    if (context.PreEntityImages != null && context.PreEntityImages.Contains("PreImage"))
                        preImage = context.PreEntityImages["PreImage"];

                    if (preImage == null)
                        throw new InvalidPluginExecutionException("PreImage não configurada no STEP de Update. Alias deve ser 'PreImage'.");
                }

                // Só age quando o update realmente trouxe o campo de status (tentando confirmar)
                // (Como seu step vai filtrar por dev_stausdopedido, aqui também fica coerente)
                if (!target.Contains(CampoStatusPedido))
                    return;

                bool confirmadoAntes = IsConfirmado(preImage);
                bool confirmadoDepois = IsConfirmadoDepois(target);

                // Precisamos da transição: NÃO confirmado -> confirmado
                if (confirmadoAntes)
                    return;

                if (!confirmadoDepois)
                    return;

                // Pegar produto e quantidade (target ou preImage)
                EntityReference produtoRef = GetLookup(target, preImage, CampoProdutoLookup);
                if (produtoRef == null)
                    throw new InvalidPluginExecutionException("Não é possível confirmar: Produto é obrigatório.");

                int? qtd = GetInt(target, preImage, CampoQuantidade);
                if (!qtd.HasValue || qtd.Value <= 0)
                    throw new InvalidPluginExecutionException("Não é possível confirmar: Quantidade deve ser maior que zero.");

                // Buscar estoque atual do produto
                Entity produto = service.Retrieve(
                    ProdutoLogicalName,
                    produtoRef.Id,
                    new ColumnSet(ProdutoCampoEstoque)
                );

                int estoqueAtual = produto.Contains(ProdutoCampoEstoque)
                    ? produto.GetAttributeValue<int>(ProdutoCampoEstoque)
                    : 0;

                // Validar saldo
                if (qtd.Value > estoqueAtual)
                {
                    throw new InvalidPluginExecutionException(
                        "Estoque insuficiente. Estoque atual: " + estoqueAtual + " | Quantidade solicitada: " + qtd.Value
                    );
                }

                int novoEstoque = estoqueAtual - qtd.Value;

                // Atualizar Produto
                Entity produtoUpdate = new Entity(ProdutoLogicalName, produtoRef.Id);
                produtoUpdate[ProdutoCampoEstoque] = novoEstoque;
                service.Update(produtoUpdate);

                tracing.Trace("Estoque baixado. ProdutoId={0} Estoque {1}->{2}", produtoRef.Id, estoqueAtual, novoEstoque);
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                tracing.Trace("Erro no plugin Pedido_PreOp_BaixarEstoqueAoConfirmar: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("Erro ao baixar estoque na confirmação do pedido.");
            }
        }

        private bool IsConfirmado(Entity e)
        {
            if (e == null) return false;

            if (e.Contains(CampoStatusPedido))
            {
                OptionSetValue os = e.GetAttributeValue<OptionSetValue>(CampoStatusPedido);
                return (os != null && os.Value == StatusConfirmado);
            }

            return false;
        }

        private bool IsConfirmadoDepois(Entity target)
        {
            if (target == null) return false;

            OptionSetValue os = target.GetAttributeValue<OptionSetValue>(CampoStatusPedido);
            return (os != null && os.Value == StatusConfirmado);
        }

        private EntityReference GetLookup(Entity target, Entity preImage, string attr)
        {
            if (target != null && target.Contains(attr))
                return target.GetAttributeValue<EntityReference>(attr);

            if (preImage != null && preImage.Contains(attr))
                return preImage.GetAttributeValue<EntityReference>(attr);

            return null;
        }

        private int? GetInt(Entity target, Entity preImage, string attr)
        {
            if (target != null && target.Contains(attr))
                return target.GetAttributeValue<int>(attr);

            if (preImage != null && preImage.Contains(attr))
                return preImage.GetAttributeValue<int>(attr);

            return null;
        }
    }
}
