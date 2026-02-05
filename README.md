## ⚙️ Business Rules

Foram implementadas regras de negócio para validação e controle dos dados diretamente no formulário.

---

### ✔️ Regra 1 – Validação da Quantidade do Pedido

Garante que a quantidade informada seja maior que zero.

Se a quantidade for menor ou igual a zero:
- O sistema exibe mensagem de erro
- O salvamento do registro é impedido

📷 Estrutura da regra:

![BR Quantidade Designer](images/business-rules/BR-validar-quantidade-designer.png)

📷 Funcionamento no formulário:

![BR Quantidade Erro](images/business-rules/BR-validar-quantidade-erro-form.png)

---

### ✔️ Regra 2 – Bloqueio de Campos ao Confirmar Pedido

Quando o status do pedido é alterado para **Confirmado**:

- Cliente, Produto e Quantidade são bloqueados
- Caso o pedido volte para Rascunho, os campos são liberados novamente

📷 Estrutura da regra:

![BR Bloqueio Designer](images/business-rules/BR-bloquear-campos-confirmado-designer.png)

📷 Funcionamento com Pedido Confirmado:

![BR Campos Bloqueados](images/business-rules/BR-bloquear-campos-confirmado-form.png)

📷 Funcionamento com Pedido em Rascunho:

![BR Campos Liberados](images/business-rules/BR-bloquear-campos-rascunho-form.png)
